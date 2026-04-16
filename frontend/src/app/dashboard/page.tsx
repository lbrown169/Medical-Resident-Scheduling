"use client";
import React, { useState, useEffect, ReactElement, useCallback, useMemo } from "react";
import {
  SidebarProvider,
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarTrigger,
  SidebarGroupLabel,
  SidebarSeparator,
} from "../../components/ui/sidebar";
import { SidebarUserCard } from "./components/SidebarUserCard";
import { Repeat, CalendarDays, CalendarX, UserCheck, Shield, Settings, Home, LogOut, User as UserIcon, ChevronDown, Moon, Sun, ClipboardList, CalendarRange, Calendar1, LayoutList } from "lucide-react";
import ProtectedRoute from '../../components/ProtectedRoute';
import { useRouter } from "next/navigation";
import { toast } from '../../lib/use-toast';
import { Toaster } from '../../components/ui/toaster';
import { config } from '../../config';
import { removeAuthToken, getUser, verifyAdminStatus, User } from '../../lib/auth';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../../components/ui/dropdown-menu";
import { useTheme } from "next-themes";
import HomePage from "./components/HomePage";
import CalendarPage from "./components/CalendarPage";
import SettingsPage from "./components/SettingsPage";
import SwapCallsPage from "./components/SwapCallsPage";
import RequestOffPage from "./components/RequestOffPage";
import CheckSchedulePage from "./components/CheckSchedulePage";
import AdminPage from "./components/AdminPage";
import PGY3RotationFormPage from "./components/PGY3RotationFormPage";
import PGY4RotationPage from "./components/PGY4RotationPage";
import PGY4SchedulePage from "../dashboard/pgy4-schedule/page";
import SchedulesPage from "./components/SchedulesPage";
import PGY12RotationPage from "./components/PGY12RotationPage";

import MobileHeader from "./components/MobileHeader";
import MobileUserMenu from "./components/MobileUserMenu";

import { VacationResponse } from "@/lib/models/VacationResponse";
import { CallType } from "@/lib/models/CallType";
import { SwapValidationResponse } from "@/lib/models/SwapValidationResponse";
import { DateResponse } from "@/lib/models/DateResponse";
import { CalendarEvent } from "@/lib/models/CalendarEvent";

type MenuItem = {
  title: string;
  icon: ReactElement;
};

interface Resident {
  resident_id: string;
  first_name: string;
  last_name: string;
  graduate_yr: number;
  email: string;
  phone_number?: string;
  hospital_role_profile?: number;
  total_hours: number;
  chief_type: string;
}

interface Admin {
  admin_id: string;
  first_name: string;
  last_name: string;
  email: string;
  phone_num?: string;
}

interface ScheduleItem {
  id: string;
  date: string;
  time: string;
  shift: string;
  location: string;
}

// menu items
const menuItems: MenuItem[] = [
  { title: "Home", icon: <Home className="w-6 h-6 mr-3" /> },
  { title: "Calendar", icon: <CalendarDays className="w-6 h-6 mr-3" /> },
  { title: "Schedules", icon: <LayoutList className="w-6 h-6 mr-3" /> },
  { title: "Rotations", icon: <CalendarRange className="w-6 h-6 mr-3" /> },
  { title: "Swap Calls", icon: <Repeat className="w-6 h-6 mr-3" /> },
  { title: "Request Off", icon: <CalendarX className="w-6 h-6 mr-3" /> },
  { title: "Check My Schedule", icon: <UserCheck className="w-6 h-6 mr-3" /> },
  { title: "Admin", icon: <Shield className="w-6 h-6 mr-3" /> },
  { title: "PGY-4 Form", icon: <ClipboardList className="w-6 h-6 mr-3" /> },
  { title: "PGY-4 Schedule", icon: <Calendar1 className="w-6 h-6 mr-3" /> },
  { title: "Dashboard", icon: <CalendarRange className="w-6 h-6 mr-3" /> },
  { title: "Settings", icon: <Settings className="w-6 h-6 mr-3" /> }
];

const leaveReasons: { id: string; name: string; halfDay?: string }[] = [
  { id: "vacation", name: "Vacation" },
  { id: "sick", name: "Sick Leave" },
  { id: "sick-am", name: "Sick Leave (AM)", halfDay: "A" },
  { id: "sick-pm", name: "Sick Leave (PM)", halfDay: "P" },
  { id: "cme", name: "ED (Education Days)" },
  { id: "personal", name: "Personal Leave" },
  { id: "other", name: "Other" },
];

function SidebarFloatingTrigger() {
  return (
    <div
      className={` z-50 left-65 top-13 -translate-y-1/2 transition-all duration-300`}
      style={{ pointerEvents: 'auto' }}
    >
      <SidebarTrigger />
    </div>
  );
}


function Dashboard() {
  const router = useRouter();
  const { setTheme } = useTheme();
  
  // Core state
  const [selected, setSelected] = useState<string>("Home");
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [phoneNumber, setPhoneNumber] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [isAdmin, setIsAdmin] = useState<boolean>(false);
  
  // Mobile state
  const [mobileUserMenuOpen, setMobileUserMenuOpen] = useState<boolean>(false);

  // Calendar state
  const [calendarEvents, setCalendarEvents] = useState<CalendarEvent[]>([]);

  // Hours each resident is scheduled for the current published semester
  const semesterHours = useMemo(() => {
    const now = new Date();
    const year = now.getFullYear();
    const isSpring = now.getMonth() < 6;
    const start = isSpring ? new Date(year, 0, 1) : new Date(year, 6, 1);
    const end   = isSpring ? new Date(year, 5, 30, 23, 59, 59) : new Date(year, 11, 31, 23, 59, 59);
    const map: Record<string, number> = {};
    calendarEvents.forEach(e => {
      const d = e.start instanceof Date ? e.start : new Date(e.start);
      if (d >= start && d <= end) {
        const id = e.extendedProps?.residentId;
        if (id) map[id] = (map[id] ?? 0) + (e.extendedProps?.hours ?? 0);
      }
    });
    return map;
  }, [calendarEvents]);

  // Swap calls form state
  const [selectedResident, setSelectedResident] = useState<string>("");
  const [selectedShift, setSelectedShift] = useState<string>("");
  const [yourShiftDate, setYourShiftDate] = useState<string>("");
  const [partnerShiftDate, setPartnerShiftDate] = useState<string>("");
  const [partnerShift, setPartnerShift] = useState<string>("");
  const [swapDescription, setSwapDescription] = useState<string>("");


  // Request off form state
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");
  const [reason, setReason] = useState<string>("");
  const [description, setDescription] = useState<string>("");

  // Admin state
  const [inviteEmail, setInviteEmail] = useState<string>("");

  const [userInvitations, setUserInvitations] = useState<{
    id: string;
    email: string;
    status: "Pending" | "Member" | "Expired";
  }[]>([]);

  // Data state
  const [residents, setResidents] = useState<Resident[]>([]);
  const [mySchedule, setMySchedule] = useState<ScheduleItem[]>([]);
  const [users, setUsers] = useState<{ id: string; first_name: string; last_name: string; email: string; role: string }[]>([]);

  // Add state for adminSwapRequests, myTimeOffRequests, and shifts
  const [myTimeOffRequests, setMyTimeOffRequests] = useState<VacationResponse[]>([]);
  const [shifts, setShifts] = useState<{
    id: string;
    name: string;
  }[]>([]);

  // Helper functions
  const formatPhoneNumber = (value: string) => {
    if (!value) return "";
    // Remove all non-digit characters
    const phoneNumber = value.replace(/\D/g, '');
    
    // Format as XXX-XXX-XXXX
    if (phoneNumber.length <= 3) {
      return phoneNumber;
    } else if (phoneNumber.length <= 6) {
      return `${phoneNumber.slice(0, 3)}-${phoneNumber.slice(3)}`;
    } else {
      return `${phoneNumber.slice(0, 3)}-${phoneNumber.slice(3, 6)}-${phoneNumber.slice(6, 10)}`;
    }
  };

  function currentAcademicYear(): number {
    const now = new Date();
    return now.getMonth() >= 6 ? now.getFullYear() : now.getFullYear() - 1;
  }

  function academicYearOf(date: Date): number {
    return date.getMonth() >= 6 ? date.getFullYear() : date.getFullYear() - 1;
  }

  // Updated color function to use graduate_yr directly
  const getEventColor = (callType: CallType, graduateYear?: number) => {
    // Use graduate_yr directly for PGY-based coloring
    if (graduateYear != null) {
      switch (graduateYear) {
        case 1:
          return '#ef4444'; // red for PGY 1
        case 2:
          return '#f97316'; // orange for PGY 2
        case 3:
          return '#8b5cf6'; // purple for PGY 3
        default:
          return '#6b7280'; // gray for unknown
      }
    }
    
    // Fallback to call type coloring if no graduate_yr
    switch (callType.id) {
      case 0: // short
        return '#3b82f6'; // blue
      case 1: // saturday 24
      case 2: // saturday 12
        return '#10b981'; // green
      case 3: // sunday 12
        return '#f59e0b'; // amber
      default:
        return '#6b7280'; // gray
    }
  };

  function parseLocalDate(dateStr: string) {
    if (!dateStr) return null;
    const [year, month, day] = dateStr.split('-');
    return new Date(Number(year), Number(month) - 1, Number(day));
  }

  function isSameDay(date1: Date | string, date2: Date | string) {
    const d1 = (typeof date1 === 'string') ? parseLocalDate(date1) : new Date(date1);
    const d2 = (typeof date2 === 'string') ? parseLocalDate(date2) : new Date(date2);
    return (
      d1.getFullYear() === d2.getFullYear() &&
      d1.getMonth() === d2.getMonth() &&
      d1.getDate() === d2.getDate()
    );
  }

  // API functions
  const fetchResidents = useCallback(async () => {
    try {
      const [residentsResponse, adminsResponse] = await Promise.all([
        fetch(`${config.apiUrl}/api/residents`),
        fetch(`${config.apiUrl}/api/Admins`),
      ]);
      if (residentsResponse.ok) {
        const residentsData = await residentsResponse.json() as Resident[];
        setResidents(residentsData);
        if (adminsResponse.ok) {
          const admins = await adminsResponse.json() as Admin[];
          setUsers([
            ...residentsData.map((r: Resident) => ({ id: r.resident_id, first_name: r.first_name, last_name: r.last_name, email: r.email, role: 'resident' })),
            ...admins.map((a: Admin) => ({ id: a.admin_id, first_name: a.first_name, last_name: a.last_name, email: a.email, role: 'admin' })),
          ]);
        }
      }
    } catch (error) {
      console.error('Error fetching residents:', error);
    }
  }, []);

  const fetchMySchedule = useCallback(async () => {
    if (!user?.id) return;
    try {
      const response = await fetch(`${config.apiUrl}/api/dates/published`);
      if (response.ok) {
        const dates = await response.json() as DateResponse[];
        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0); // Start of today

        // Filter for current user and future dates only, and with a real callType
        const userSchedule = dates
          .filter((date) => {
            const dateObj = new Date(date.shiftDate);
            return date.residentId === user.id && dateObj >= currentDate && date.callType;
          })
          .sort((a, b) => new Date(a.shiftDate).getTime() - new Date(b.shiftDate).getTime())
          .slice(0, 20)
          .map((date) => ({
            id: date.dateId,
            date: date.shiftDate,
            time: "All Day",
            shift: `${date.callType.description}${date.callType.id === 99 ? ` (${date.hours}h)` : ''} Call`,
            location: "Hospital"
          }));

        setMySchedule(userSchedule);
      } else {
        console.error('Failed to fetch schedule');
      }
    } catch (error) {
      console.error('Error fetching schedule:', error);
    }
  }, [user?.id]);

  const fetchCalendarEvents = useCallback(async () => {
    try {
      // Fetch all dates - the backend doesn't support month/year filtering yet
      const response = await fetch(`${config.apiUrl}/api/dates/published`);
      if (response.ok) {
        const dates = await response.json() as DateResponse[];
        const events = dates.map((date: DateResponse) => {
          // Only show the resident's name on the calendar
          const fullName = date.firstName && date.lastName
            ? `${date.firstName} ${date.lastName}`
            : date.residentId;

          // Find the resident to get graduate_yr directly (for details only)
          const resident = residents.find(r => r.resident_id === date.residentId);
          const graduateYear = resident?.graduate_yr;

          const d = new Date(date.shiftDate)
          // date comes in as UTC and gets changed to previous day in local time. keep everything local
          d.setMinutes(d.getMinutes() + d.getTimezoneOffset());

          // Offset PGY by how far ahead this shift's academic year is vs. today
          const pgyOffset = academicYearOf(d) - currentAcademicYear();
          const effectivePgy = graduateYear != null ? graduateYear + pgyOffset : undefined;
          const eventColor = getEventColor(date.callType, effectivePgy);

          return {
            id: date.dateId,
            title: fullName || '', // Only name
            start: d,
            end: d,
            backgroundColor: eventColor,
            extendedProps: {
              scheduleId: date.scheduleId,
              residentId: date.residentId,
              firstName: date.firstName,
              lastName: date.lastName,
              callType: date.callType.description,
              callTypeId: date.callType.id,
              dateId: date.dateId,
              pgyLevel: effectivePgy,
              hours: date.hours,
            }
          };
        });

        setCalendarEvents(events);
      } else {
        console.error('Failed to fetch calendar events');
        toast({
          variant: "destructive",
          title: "Error",
          description: "Failed to load calendar events",
        });
      }
    } catch (e) {
      console.error(e, 'Error fetching calendar events');
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to load calendar events",
      });
    }
  }, [residents]);


  // Fetch time off requests
  const fetchMyTimeOffRequests = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/vacations`);
      if (response.ok) {
        const data = await response.json();
        setMyTimeOffRequests(data);
      } else {
        setMyTimeOffRequests([]);
      }
    } catch {
      setMyTimeOffRequests([]);
    }
  }, []);

  // Fetch shifts (rotations)
  const fetchShifts = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/rotations`);
      if (response.ok) {
        const data = await response.json();
        setShifts(data);
      } else {
        setShifts([]);
      }
    } catch {
      setShifts([]);
    }
  }, []);

  // Event handlers
  const handleUpdatePhoneNumber = async () => {
    // Updated regex to match XXX-XXX-XXXX format
    const phoneRegex = /^\d{3}-\d{3}-\d{4}$/;
    if (!phoneRegex.test(phoneNumber)) {
      toast({
        variant: "destructive",
        title: "Invalid Phone Number",
        description: "Please enter a valid phone number in format 123-456-7890.",
      });
      return;
    }

    if (!user?.id) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Unable to update phone number. Please try logging in again.",
      });
      return;
    }

    try {
      // Update with existing data but new phone number
      const response = await fetch(`${config.apiUrl}/api/residents/${user.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          phone_num: phoneNumber,
        })
      });

      if (response.ok) {
        // Update the user object in state and localStorage
        const updatedUser = { ...user, phone_num: phoneNumber };
        setUser(updatedUser);
        localStorage.setItem('user', JSON.stringify(updatedUser));

        toast({
          variant: "success",
          title: "Phone Number Updated",
          description: `Your phone number has been updated to ${phoneNumber}.`,
        });
      } else {
        throw new Error('Failed to update phone number');
      }
    } catch (error) {
      console.error('Error updating phone number:', error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to update phone number. Please try again.",
      });
    }
  };

  const handleUpdateEmail = async () => {
    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      toast({
        variant: "destructive",
        title: "Invalid Email",
        description: "Please enter a valid email address.",
      });
      return;
    }

    if (!user?.id) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Unable to update email. Please try logging in again.",
      });
      return;
    }

    try {
      // Update with existing data but new email
      const response = await fetch(`${config.apiUrl}/api/residents/${user.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: email,
        })
      });

      if (response.ok) {
        // Update the user object in state and localStorage
        const updatedUser = { ...user, email: email };
        setUser(updatedUser);
        localStorage.setItem('user', JSON.stringify(updatedUser));

        toast({
          variant: "success",
          title: "Email Updated",
          description: `Your email has been updated to ${email}.`,
        });
      } else {
        throw new Error('Failed to update email');
      }
    } catch (error) {
      console.error('Error updating email:', error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to update email. Please try again.",
      });
    }
  };

  const fetchInvitations = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/invite`);
      if (response.ok) {
        const data: { email: string; expires: string; used: boolean }[] = await response.json();
        const now = new Date();
        setUserInvitations(data.map((inv) => ({
          id: inv.email,
          email: inv.email,
          status: inv.used ? "Member" as const : new Date(inv.expires) < now ? "Expired" as const : "Pending" as const,
        })));
      }
    } catch (error) {
      console.error("Failed to fetch invitations:", error);
    }
  }, []);

  const handleSendInvite = async () => {
    if (!inviteEmail.trim()) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Please enter an email address.",
      });
      return;
    }
  
    try {
      const response = await fetch(`${config.apiUrl}/api/invite/send`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: inviteEmail.trim(),
        }),
      });
  
      if (response.ok) {
        await fetchInvitations();
        setInviteEmail("");
        toast({
          variant: "success",
          title: "Invitation Sent",
          description: `Invitation sent to ${inviteEmail.trim()}.`,
        });
      } else {
        const body = await response.json().catch(() => null);
        const message = body?.message || body?.Message || "Failed to send invitation. Please check the email or try again.";
        toast({
          variant: "destructive",
          title: "Error",
          description: message,
        });
      }
    } catch (error) {
      console.error("Send invitation error:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to send invitation. Please check the email or try again.",
      });
    }
  };
  

  const handleResendInvite = async (email: string) => {
    try {
      const deleteRes = await fetch(`${config.apiUrl}/api/invite/${encodeURIComponent(email)}`, { method: "DELETE" });
      if (!deleteRes.ok) throw new Error("Failed to delete old invitation");

      const sendRes = await fetch(`${config.apiUrl}/api/invite/send`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });
      if (!sendRes.ok) throw new Error("Failed to send new invitation");

      await fetchInvitations();
      toast({ variant: "success", title: "Invitation Resent", description: `A new invitation has been sent to ${email}.` });
    } catch (error) {
      console.error("Resend invite error:", error);
      toast({ variant: "destructive", title: "Error", description: "Failed to resend invitation. Please try again." });
    }
  };

  const handleDeleteInvite = async (email: string) => {
    try {
      const response = await fetch(`${config.apiUrl}/api/invite/${encodeURIComponent(email)}`, {
        method: "DELETE",
      });
      if (response.ok) {
        await fetchInvitations();
        toast({ variant: "success", title: "Invitation Deleted", description: `Invitation for ${email} has been deleted.` });
      } else {
        throw new Error("Failed to delete invitation");
      }
    } catch (error) {
      console.error("Delete invitation error:", error);
      toast({ variant: "destructive", title: "Error", description: "Failed to delete invitation. Please try again." });
    }
  };

  const handleApproveRequest = async (groupId: string) => {
    try {

      const response = await fetch(`${config.apiUrl}/api/vacations/group/${groupId}/status/approve`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
      });
  
      if (!response.ok) {
        const errorText = await response.text();
        console.error("Failed to approve request:", response.status, errorText);
        throw new Error("Failed to approve request.");
      }
  
      toast({
        variant: "success",
        title: "Request Approved",
        description: "The vacation request has been approved.",
      });
  
      fetchMyTimeOffRequests();
      fetchResidents();
    } catch (err) {
      console.error("Error approving vacation request group:", err);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to approve request group. Please try again.",
      });
    }
  };
  
  const handleDenyRequest = async (groupId: string) => {
    try {
      const response = await fetch(`${config.apiUrl}/api/vacations/group/${groupId}/status/deny`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
      });
  
      if (!response.ok) {
        const errorText = await response.text();
        console.error("Failed to deny request:", response.status, errorText);
        throw new Error("Failed to deny request.");
      }
  
      toast({
        variant: "destructive",
        title: "Request Denied",
        description: "The vacation request has been denied.",
      });
  
      fetchMyTimeOffRequests();
      fetchResidents();
    } catch (err) {
      console.error("Error denying vacation request group:", err);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to deny request group. Please try again.",
      });
    }
  };
  

  const handleSubmitSwap = async (): Promise<SwapValidationResponse | null> => {
    if (!selectedResident || !selectedShift || !yourShiftDate || !partnerShiftDate || !partnerShift) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Please select both residents, both shifts, and both dates.",
      });
      return null;
    }

    try {
      const myShift = calendarEvents.find((event) =>
        (event.extendedProps?.residentId || "").trim() === (user?.id || "").trim() &&
        isSameDay(event.start, yourShiftDate)
      );

      const partnerShiftEvent = calendarEvents.find((event) =>
        (event.extendedProps?.residentId || "").trim() === (selectedResident || "").trim() &&
        isSameDay(event.start, partnerShiftDate)
      );

      if (!myShift || !partnerShiftEvent) {
        toast({
          variant: "destructive",
          title: "Error",
          description: "Could not find both shifts to swap.",
        });
        return null;
      }
      // Create a swap request (pending approval)
      const swapRequest = {
        RequesterId: user?.id,
        RequesteeId: selectedResident,
        RequesterDate: yourShiftDate,
        RequesteeDate: partnerShiftDate,
        Details: swapDescription.trim()
      };
      const response = await fetch(`${config.apiUrl}/api/swaprequests`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(swapRequest)
      });
      if (response.ok) {
        toast({
          variant: "success",
          title: "Swap Request Sent",
          description: "Your swap request has been sent and is pending approval.",
        });
        setSelectedResident("");
        setSelectedShift("");
        setYourShiftDate("");
        setPartnerShiftDate("");
        setPartnerShift("");
        setSwapDescription("");
        return null;
      } else {
        const data: SwapValidationResponse = await response.json().catch(() => ({
          success: false,
          message: "Failed to create swap request.",
        }));
        return data;
      }
    } catch (error) {
      console.error('Error creating swap request:', error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to create swap request. Please try again.",
      });
      return null;
    }
  };

  const handleSubmitRequestOff = async () => {
    if (!user?.id) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "User ID is missing. Please log in again.",
      });
      return;
    }

    // console.log('[RequestOff] Submitting vacation request for residentId:', user?.id);

    if (!startDate || !endDate || !reason) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Please fill out all fields.",
      });
      return;
    }
  
    if (new Date(startDate) > new Date(endDate)) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "End date cannot be before start date.",
      });
      return;
    }
  
    try {
      // Parse dates and apply timezone offset to keep them local
      const start = new Date(startDate);
      start.setMinutes(start.getMinutes() + start.getTimezoneOffset());
      const end = new Date(endDate);
      end.setMinutes(end.getMinutes() + end.getTimezoneOffset());

      const groupId = crypto.randomUUID(); //group ID
      const requests = [];

      // Helper to format date as YYYY-MM-DD without timezone conversion
      const formatLocalDate = (date: Date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
      };

      const selectedReason = leaveReasons.find((r) => r.id === reason);
      const apiReason = selectedReason?.id?.startsWith("sick")
        ? "Sick Leave"
        : (selectedReason?.name ?? reason);
      const halfDay = selectedReason?.halfDay ?? null;

      for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
        requests.push({
          GroupId: groupId, //New request
          ResidentId: user.id,
          Date: formatLocalDate(d),
          Reason: apiReason,
          Details: description || '',
          ...(halfDay ? { HalfDay: halfDay } : {}),
        });
      }
  
      for (const request of requests) {
        const response = await fetch(`${config.apiUrl}/api/vacations`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(request),
        });
  
        if (!response.ok) {
          const errorText = await response.text();
          console.error("Failed to submit request:", response.status, errorText);
          throw new Error("One of the vacation requests failed");
        }
      }
  
      toast({
        variant: "success",
        title: "Request Submitted",
        description: `Time off request submitted for ${startDate} to ${endDate}.`,
      });
  
      //Clear form
      setStartDate("");
      setEndDate("");
      setReason("");
      setDescription("");
  
      //refresh list
      fetchMyTimeOffRequests();
  
    } catch (err) {
      console.error('Error submitting vacation request:', err);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to submit request. Please try again.",
      });
    }
  };
  
  

  const handleLogout = async () => {
    removeAuthToken();
    
    toast({
      variant: "success",
      title: "Success",
      description: "Logged out successfully",
    });

    await new Promise(resolve => setTimeout(resolve, 1500));
    router.push("/");
  };

  // Mobile navigation handlers
  const handleOpenMobileUserMenu = () => {
    setMobileUserMenuOpen(true);
  };

  const handleCloseMobileUserMenu = () => {
    setMobileUserMenuOpen(false);
  };



  const handleDeleteUser = async (user: { id: string; role: string }) => {
    try {
      const endpoint = user.role === 'admin' ? 'Admins' : 'Residents';
      const response = await fetch(`${config.apiUrl}/api/${endpoint}/${user.id}`, {
        method: 'DELETE',
      });

      if (response.ok) {
        setUsers(prev => prev.filter(u => u.id !== user.id));
        toast({
          variant: 'success',
          title: 'User Deleted',
          description: 'User has been successfully deleted.'
        });
      } else {
        throw new Error('Failed to delete user');
      }
    } catch {
      console.error('Error deleting user');
      toast({
        variant: 'destructive',
        title: 'Error',
        description: 'Failed to delete user.'
      });
    }
  };

  // const refreshCalendar = async () => {
  //   await fetchCalendarEvents();
  // };

  const mappedResidents = useMemo(() =>
    residents.map(r => ({ id: r.resident_id, name: `${r.first_name} ${r.last_name}`, email: r.email, pgyLevel: r.graduate_yr, hospitalRole: r.hospital_role_profile ?? undefined, hours: semesterHours[r.resident_id] ?? 0 })),
    [residents, semesterHours]
  );

  const mappedShifts = useMemo(() =>
    shifts.map(s => ({ id: s.id, name: s.name })),
    [shifts]
  );

  // Render main content based on selected menu item
  const renderMainContent = () => {
    switch (selected) {
case "Home":
  if (isAdmin) {
    return (
      <AdminPage
        residents={mappedResidents}
        onRefreshResidents={fetchResidents}
        myTimeOffRequests={myTimeOffRequests}
        shifts={mappedShifts}
        handleApproveRequest={handleApproveRequest}
        handleDenyRequest={handleDenyRequest}
        userInvitations={userInvitations}
        inviteEmail={inviteEmail}
        setInviteEmail={setInviteEmail}
        handleSendInvite={handleSendInvite}
        handleResendInvite={handleResendInvite}
        handleDeleteInvite={handleDeleteInvite}
        onRefreshInvitations={fetchInvitations}
        users={users}
        handleDeleteUser={handleDeleteUser}
        inviteRole={inviteRole}
        setInviteRole={setInviteRole}
        userId={user?.id || ""}
      />
    );
  }
  else{
    return (
      <HomePage
        displayName={displayName}
        rotation={null}
        rotationEndDate={null}
        monthlyHours={null}
        hasData={false}
        onNavigateToSwapCalls={() => setSelected("Swap Calls")}
        onNavigateToRequestOff={() => setSelected("Request Off")}
        onNavigateToSchedule={() => setSelected("Check My Schedule")}
        userId={user?.id || ""}
        calendarEvents={calendarEvents}
        isAdmin={isAdmin}
        onRefreshCalendar={() => {
          fetchCalendarEvents();
        }}
      />
    );
  }

      case "Calendar":
        return (
          <CalendarPage
            events={calendarEvents}
            onNavigateToSwapCalls={() => setSelected("Swap Calls")}
            onNavigateToRequestOff={() => setSelected("Request Off")}
            onNavigateToCheckSchedule={() => setSelected("Check My Schedule")}
            onNavigateToSettings={() => setSelected("Settings")}
            onNavigateToHome={() => setSelected("Home")}
            onNavigateToSchedules={() => setSelected("Schedules")}
            onNavigateToRotations={() => setSelected("Rotations")}
            isAdmin={isAdmin}
          />
        );

      case "Settings":
        return (
          <SettingsPage
            firstName={user?.firstName || ""}
            lastName={user?.lastName || ""}
            userId={user?.id || ""}
            email={email}
            setEmail={setEmail}
            handleUpdateEmail={handleUpdateEmail}
            phoneNumber={phoneNumber}
            setPhoneNumber={setPhoneNumber}
            handleUpdatePhoneNumber={handleUpdatePhoneNumber}
            isAdmin={isAdmin}
          />
        );

      case "Swap Calls": {
        const now = new Date(); now.setHours(0, 0, 0, 0);
        const filterShiftEvents = (residentId: string) =>
          calendarEvents
            .filter(e => (e.extendedProps?.residentId || "").trim() === residentId.trim() && new Date(e.start) >= now)
            .sort((a, b) => new Date(a.start).getTime() - new Date(b.start).getTime());
        const swapAcademicYearOf = (date: Date) => date.getMonth() >= 6 ? date.getFullYear() : date.getFullYear() - 1;
        const selectedShiftYear = yourShiftDate ? swapAcademicYearOf(new Date(yourShiftDate)) : null;
        const pgyMatchedResidents = residents
          .filter(r => r.resident_id !== user?.id)
          .filter(r => {
            const shifts = filterShiftEvents(r.resident_id);
            if (selectedShiftYear !== null) {
              return shifts.some(e => swapAcademicYearOf(e.start instanceof Date ? e.start : new Date(e.start)) === selectedShiftYear);
            }
            return shifts.length > 0;
          })
          .map(r => ({ id: r.resident_id, name: `${r.first_name} ${r.last_name}` }));
        const userShiftEvents = user?.id ? filterShiftEvents(user.id) : [];
        const partnerShiftEvents = selectedResident ? filterShiftEvents(selectedResident) : [];
        return (
          <SwapCallsPage
            userId={user?.id || ""}
            yourShiftDate={yourShiftDate}
            partnerShiftDate={partnerShiftDate}
            selectedResident={selectedResident}
            setSelectedResident={(v) => { setSelectedResident(v); setPartnerShiftDate(""); setPartnerShift(""); }}
            residents={pgyMatchedResidents}
            selectedShift={selectedShift}
            partnerShift={partnerShift}
            userShiftEvents={userShiftEvents}
            partnerShiftEvents={partnerShiftEvents}
            onSelectUserShift={(date, callType) => { setYourShiftDate(date); setSelectedShift(callType); setPartnerShiftDate(""); setPartnerShift(""); }}
            onSelectPartnerShift={(date, callType) => { setPartnerShiftDate(date); setPartnerShift(callType); }}
            description={swapDescription}
            setDescription={setSwapDescription}
            handleSubmitSwap={handleSubmitSwap}
          />
        );
      }

      case "Request Off":
        return (
          <RequestOffPage
            userId={user?.id || ""}
            startDate={startDate}
            setStartDate={setStartDate}
            endDate={endDate}
            setEndDate={setEndDate}
            reason={reason}
            setReason={setReason}
            leaveReasons={leaveReasons}
            description={description}
            setDescription={setDescription}
            handleSubmitRequestOff={handleSubmitRequestOff}
          />
        );

      case "Check My Schedule":
        return <CheckSchedulePage mySchedule={mySchedule} />;

      case "Admin":
        if (!isAdmin) {
          return (
            <div className="w-full pt-4 flex flex-col items-center">
              <h1 className="text-2xl font-bold mb-6">Access Denied</h1>
              <p className="text-center text-gray-600 dark:text-gray-400">
                You do not have permission to access the admin panel.
              </p>
            </div>
          );
        }
        return (
          <AdminPage
            residents={mappedResidents}
            onRefreshResidents={fetchResidents}
            myTimeOffRequests={myTimeOffRequests}
            shifts={mappedShifts}
            handleApproveRequest={handleApproveRequest}
            handleDenyRequest={handleDenyRequest}
            userInvitations={userInvitations}
            inviteEmail={inviteEmail}
            setInviteEmail={setInviteEmail}
            handleSendInvite={handleSendInvite}
            handleResendInvite={handleResendInvite}
            handleDeleteInvite={handleDeleteInvite}
            onRefreshInvitations={fetchInvitations}
            users={users}
            handleDeleteUser={handleDeleteUser}
            inviteRole={inviteRole}
            setInviteRole={setInviteRole}
            userId={user?.id || ""}
          />
        );

      case "Schedules":
        if (!isAdmin) {
          return (
            <div className="w-full pt-4 flex flex-col items-center">
              <h1 className="text-2xl font-bold mb-6">Access Denied</h1>
              <p className="text-center text-gray-600 dark:text-gray-400">
                You do not have permission to access the schedules page.
              </p>
            </div>
          );
        }
        return (
          <SchedulesPage
            onNavigateToCalendar={() => {
              setSelected("Calendar");
              fetchCalendarEvents();
            }}
          />
        );

      case "Rotations":
        if (!isAdmin) {
          return (
            <div className="w-full pt-4 flex flex-col items-center">
              <h1 className="text-2xl font-bold mb-6">Access Denied</h1>
              <p className="text-center text-gray-600 dark:text-gray-400">
                You do not have permission to access this page.
              </p>
            </div>
          );
        }
        return <PGY12RotationPage />;

      case "PGY-4 Form":
        return (
          <PGY3RotationFormPage
            userId={user?.id || ""}
            userPGY={currentUserPGY || 0}
          />
        );
      
      case "PGY-4 Schedule":
        // Only PGY-4 residents can view this page
        if (currentUserPGY !== 4) {
          return (
            <div className="w-full pt-4 flex flex-col items-center">
              <h1 className="text-2xl font-bold mb-6">Access Denied</h1>
              <p className="text-center text-gray-600 dark:text-gray-400">
                This page is only accessible to PGY-4 residents.
              </p>
            </div>
          );
        }
        return <PGY4SchedulePage />;
      
      case "Dashboard":
        if (!isAdmin) {
          return (
            <div className="w-full pt-4 flex flex-col items-center">
              <h1 className="text-2xl font-bold mb-6">Access Denied</h1>
              <p className="text-center text-gray-600 dark:text-gray-400">
                You do not have permission to access the PGY-4 admin panel.
              </p>
            </div>
          );
        }
        return (
          <PGY4RotationPage
          residents={residents.map(r => ({ id: r.resident_id, name: `${r.first_name} ${r.last_name}`, email: r.email, pgyLevel: r.graduate_yr, chiefType: r.chief_type }))}
          />
        );

      default:
        return null;
    }
  };

  // Effects
  useEffect(() => {
    const initialize = async () => {
      const userData = getUser();
      setUser(userData);
      
      if (userData) {
        const adminStatus = await verifyAdminStatus();
        setIsAdmin(adminStatus);
        // Initialize phone number with user's current phone number, formatted
        setPhoneNumber(formatPhoneNumber(userData.phone_num || ""));
        // Initialize email with user's current email
        setEmail(userData.email || "");
      }
      
      setLoading(false);
    };
    
    initialize();
  }, []);

  // Initial fetch of residents and calendar events
  useEffect(() => {
    if (user && !loading) {
      fetchResidents();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user, loading]);

  // Fetch calendar events after residents are loaded
  useEffect(() => {
    if (residents.length > 0) {
      fetchCalendarEvents();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [residents]);

  // Handle page-specific data fetching
  useEffect(() => {
    if (selected === "Check My Schedule") {
      fetchMySchedule();
    }
    if (selected === "Calendar") {
      fetchCalendarEvents();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selected]);

  // Fetch data when Admin page is selected
  useEffect(() => {
    if (selected === "Admin") {
      fetchMyTimeOffRequests();
      fetchShifts();
      fetchInvitations();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selected]);

  // Computed values
  const displayName = user ? `${user.firstName} ${user.lastName}` : "John Doe";
  const displayEmail = user?.email || "john.doe@email.com";
  
  // Get current user's PGY level
  const currentUserPGY = residents.find(r => r.resident_id === user?.id)?.graduate_yr;
  
  const filteredMenuItems = menuItems.filter(item => {
    if (item.title === "Admin") return false; //hide admin option
    if (item.title === "Request Off") return !isAdmin; // residents only
    if (item.title === "Check My Schedule") return !isAdmin; // residents only
    if (item.title === "Swap Calls") return !isAdmin; // residents only
    if (item.title === "Schedules") return isAdmin; // admin only
    if (item.title === "Rotations") return isAdmin; // admin only
    if (item.title === "PGY-4 Form") return currentUserPGY === 3; // pgy3 resident only
    if (item.title === "PGY-4 Schedule") return currentUserPGY === 4; // pgy4 resident only
    if (item.title === "Dashboard") return isAdmin; // admin only
    return true;
  });

  const sidebarGroups = [
    {
      label: null,      // title for sidebar group header
      showLabel: false, // only shows if true, menu items will show regardless
      items: ["Home"],  // all menu items must be in a group 
    },
    {
      label: "PGY 1-3 Residents",
      showLabel: isAdmin,
      items: ["Calendar", "Request Off", "Check My Schedule", "Swap Calls", "Schedules", "Rotations"],
    },
    {
      label: "PGY 4 Residents",
      showLabel: isAdmin,
      items: ["PGY-4 Form", "PGY-4 Schedule", "Dashboard"],
    },
    {
      label: null,
      showLabel: false,
      items: ["Settings"],
    },
  ];

  

  const [inviteRole, setInviteRole] = useState<string>("resident");

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <ProtectedRoute>
      <SidebarProvider defaultOpen={true}>
        <div className={`flex min-h-screen w-full`}>
          <Toaster />
          
          {/* Mobile Header */}
          <MobileHeader
            selected={selected}
            onOpenUserMenu={handleOpenMobileUserMenu}
            onLogout={handleLogout}
          />
          
          {/* Left Sidebar Trigger (moves with sidebar, only on calendar page) */}
          {selected === "Calendar" && <SidebarFloatingTrigger />}
          
          {/* Sidebar Navigation - Desktop only */}
          <div className="hidden md:block">
            {selected !== "Calendar" && (
              <Sidebar className="z-50">
                <SidebarHeader>
                  <div className="flex items-center justify-center py-2">
                    <span className="text-3xl font-bold tracking-wide">PSYCALL</span>
                  </div>
                </SidebarHeader>
                <SidebarContent>
                  {sidebarGroups.map((group, i) => {
                    const visibleItems = filteredMenuItems.filter(item =>
                      group.items.includes(item.title)
                    );

                    const showGroup = visibleItems.length > 0 || (group.showLabel && group.label);
                    if (!showGroup) return null;

                    return (
                      <SidebarGroup key={i}>
                        {group.label && group.showLabel && (
                          <>
                            <SidebarSeparator />
                            <SidebarGroupLabel>{group.label}</SidebarGroupLabel>
                          </>
                        )}
                        {!group.showLabel && i > 0 && visibleItems.length > 0 && (
                          <SidebarSeparator />
                        )}
                        <SidebarGroupContent>
                          <SidebarMenu>
                            {visibleItems.map((item) => (
                              <SidebarMenuItem key={item.title}>
                                <SidebarMenuButton asChild>
                                  <span
                                    className={`flex items-center text-xl cursor-pointer rounded-lg px-2 py-1 transition-colors ${
                                      selected === item.title
                                        ? "font-bold text-gray-800 dark:text-gray-200 bg-gray-300 dark:bg-gray-700"
                                        : "hover:bg-gray-900 dark:hover:bg-gray-700"
                                    }`}
                                    onClick={() => setSelected(item.title)}
                                  >
                                    {item.icon}
                                    {item.title}
                                  </span>
                                </SidebarMenuButton>
                              </SidebarMenuItem>
                            ))}
                          </SidebarMenu>
                        </SidebarGroupContent>
                      </SidebarGroup>
                    );
                  })}
                </SidebarContent>
                <SidebarFooter>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <div className="cursor-pointer flex items-center gap-2 group relative" title="Account options">
                        <SidebarUserCard
                          name={displayName}
                          email={displayEmail}
                        />
                        <ChevronDown className="h-5 w-5 text-gray-400 group-hover:text-gray-700 dark:group-hover:text-gray-200 transition-colors" />
                      </div>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-56">
                      <DropdownMenuItem
                        className="flex items-center gap-2 cursor-pointer"
                        onClick={() => setSelected("Settings")}
                      >
                        <UserIcon className="h-4 w-4" />
                        <span>Profile</span>
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => setTheme("light")} className="flex items-center gap-2 cursor-pointer">
                        <Sun className="h-4 w-4" />
                        <span>Light</span>
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => setTheme("dark")} className="flex items-center gap-2 cursor-pointer">
                        <Moon className="h-4 w-4" />
                        <span>Dark</span>
                      </DropdownMenuItem>
                      <DropdownMenuItem
                        className="flex items-center gap-2 text-red-600 focus:text-red-600 cursor-pointer"
                        onClick={handleLogout}
                      >
                        <LogOut className="h-4 w-4" />
                        <span>Log out</span>
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </SidebarFooter>
              </Sidebar>
            )}
          </div>
          
          {/* Main Content Area */}
          <div className={`flex-1 flex flex-col`}>
            <main
              className={`w-full ${
                selected === "Calendar" 
                  ? "h-screen" 
                  : "p-4 md:p-8 pb-24 md:pb-8 pt-16 md:pt-8" // Add top padding for mobile header, bottom padding for mobile navigation
              }`}
            >
              {renderMainContent()}
            </main>
          </div>
          
          {/* Mobile Navigation - Hidden */}
          {/* <MobileNavigation
            selected={selected}
            setSelected={setSelected}
            isAdmin={isAdmin}
          /> */}
          
          {/* Mobile User Menu */}
          <MobileUserMenu
            isOpen={mobileUserMenuOpen}
            onClose={handleCloseMobileUserMenu}
            displayName={displayName}
            displayEmail={displayEmail}
            onLogout={handleLogout}
          />
        </div>
      </SidebarProvider>
    </ProtectedRoute>
  );
}

export default Dashboard;
