import React, { useState, useEffect, useMemo } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { CalendarX, Clock, UserCheck, RotateCcw, Repeat, CalendarCheck, Bell, Users } from "lucide-react";
import { config } from "../../../config";
import { CalendarEvent } from "@/lib/models/CalendarEvent";

interface HomeProps {
  displayName: string;
  rotation: string | null;
  rotationEndDate: string | null;
  monthlyHours: number | string | null;
  hasData: boolean;
  onNavigateToSwapCalls: () => void;
  onNavigateToRequestOff: () => void;
  onNavigateToSchedule: () => void;
  userId: string;
  calendarEvents?: CalendarEvent[];
  onRefreshCalendar?: () => void;
  isAdmin: boolean;
}

interface DashboardData {
  monthlyHours: number;
  upcomingShifts: Array<{
    date: string;
    type: string;
  }>;
  recentActivity: Array<{
    id: string;
    type: string;
    message: string;
    date: string;
  }>;
  teamUpdates: Array<{
    id: string;
    message: string;
    date: string;
  }>;
}

const HomePage: React.FC<HomeProps & { calendarEvents?: CalendarEvent[]; userId: string; onRefreshCalendar?: () => void }> = ({
  displayName,
  onNavigateToSwapCalls,
  onNavigateToRequestOff,
  onNavigateToSchedule,
  userId,
  calendarEvents,
  onRefreshCalendar,
  isAdmin,
}) => {
  const [dashboardData, setDashboardData] = useState<DashboardData>({
    monthlyHours: 0,
    upcomingShifts: [],
    recentActivity: [],
    teamUpdates: []
  });
  const [loading, setLoading] = useState(true);

  const upcomingShifts = useMemo(() => {
    if (!calendarEvents?.length) return dashboardData.upcomingShifts;
    const now = new Date();
    now.setHours(0, 0, 0, 0);
    return calendarEvents
      .filter(e => {
        const start = e.start instanceof Date ? e.start : new Date(e.start);
        return e.extendedProps?.residentId === userId && start >= now;
      })
      .sort((a, b) => {
        const aDate = a.start instanceof Date ? a.start : new Date(a.start);
        const bDate = b.start instanceof Date ? b.start : new Date(b.start);
        return aDate.getTime() - bDate.getTime();
      })
      .slice(0, 5)
      .map(e => ({
        date: (e.start instanceof Date ? e.start : new Date(e.start)).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
        type: `${e.extendedProps?.callType ?? ''}${e.extendedProps?.callTypeId === 99 ? ` (${e.extendedProps.hours}h)` : ''}`,
      }));
  }, [calendarEvents, userId, dashboardData.upcomingShifts]);


  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        // Fetch all dashboard data in a single API call
        const dashboardResponse = await fetch(`${config.apiUrl}/api/dashboard/resident/${userId}`);
        
        if (dashboardResponse.ok) {
          const data = await dashboardResponse.json();
          setDashboardData({
            monthlyHours: data.monthlyHours,
            upcomingShifts: data.upcomingShifts,
            recentActivity: data.recentActivity,
            teamUpdates: data.teamUpdates
          });
        } else if (dashboardResponse.status === 404) {
          console.log('No dashboard data found for user:', userId);
          // Keep default "No rotation assigned" state
        } else {
          console.error('Error fetching dashboard data:', dashboardResponse.status, dashboardResponse.statusText);
        }
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
      } finally {
        setLoading(false);
      }
    };

    if (userId) {
      fetchDashboardData();
    }
  }, [userId]);

  if (loading) {
    return (
      <div className="w-full pt-4 flex flex-col items-center">
        <div className="w-full max-w-5xl">
          <h1 className="text-3xl font-bold mb-6">Hello, {displayName}!</h1>
          <div className="animate-pulse space-y-6">
            <div className="h-32 bg-muted rounded-2xl"></div>
            <div className="h-24 bg-muted rounded-2xl"></div>
          </div>
        </div>
      </div>
    );
  }

  const filteredRecentActivity = dashboardData.recentActivity.filter(
    (activity) => activity.type === 'swap_pending'
  );

  return (
    <div className="w-full pt-4 flex flex-col items-center">
      <div className="mb-6 w-full max-w-3xl mx-auto">
        <h1 className="text-3xl font-bold">Hello, {displayName}!</h1>
        <p className="text-muted-foreground mt-2">
          Welcome back! Here&apos;s your dashboard overview.
        </p>
      </div>
      
      <div className="w-full max-w-3xl flex flex-col gap-6">
        {/* Main Summary Card */}
        <Card className="p-6 bg-card shadow-lg rounded-2xl">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-0 mb-6 w-full justify-between">
            {/* Hours This Month - Left */}
            <div className="flex flex-col items-center text-center pr-4">
              <h2 className="text-xl font-semibold mb-2 flex items-center gap-2">
                <Clock className="h-5 w-5" />
                Hours This Month
              </h2>
              <p className="text-3xl font-bold text-primary w-full text-center">
                {dashboardData.monthlyHours}
              </p>
              <p className="text-sm text-muted-foreground w-full text-center">Total hours</p>
            </div>

            {/* Upcoming Shifts - Center */}
            <div className="flex flex-col items-center text-center w-full">
              <h2 className="text-xl font-semibold mb-2 flex items-center justify-center gap-2">
                <CalendarCheck className="h-5 w-5" />
                Upcoming Shifts
              </h2>
              <div className="space-y-1">
                {upcomingShifts.length > 0 ? (
                  upcomingShifts.map((shift, index) => (
                    <div key={index} className="text-sm">
                      <span className="font-medium">{shift.date}</span>
                      <span className="text-muted-foreground ml-2">{shift.type}</span>
                    </div>
                  ))
                ) : (
                  <p className="text-sm text-muted-foreground">No upcoming shifts</p>
                )}
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-col items-center md:flex-row md:justify-center md:items-center gap-4 mt-2">
            <Button 
              onClick={onNavigateToSwapCalls}
              className="p-8 w-full max-w-55 bg-card text-card-foreground border border-border hover:bg-accent hover:text-accent-foreground rounded-xl shadow-sm hover:shadow-md transition-all duration-200"
              variant="outline"
            >
              <div className="text-left w-full">
                <div className="flex items-center gap-3 mb-2">
                  <Repeat className="h-4 w-4" />
                  <span className="font-semibold text-base">Request Call Swap</span>
                </div>
                <p className="text-sm text-muted-foreground">Submit a swap request</p>
              </div>
            </Button>
            
            <Button 
              onClick={onNavigateToRequestOff}
              className="p-8 w-full max-w-55 bg-card text-card-foreground border border-border hover:bg-accent hover:text-accent-foreground rounded-xl shadow-sm hover:shadow-md transition-all duration-200"
              variant="outline"
            >
              <div className="text-left w-full">
                <div className="flex items-center gap-3 mb-2">
                  <CalendarX className="h-4 w-4" />
                  <span className="font-semibold text-base">Request Time Off</span>
                </div>
                <p className="text-sm text-muted-foreground ">Plan your time off</p>
              </div>
            </Button>
            
            {/* View My Schedule button: only show if not admin */}
            {!isAdmin && (
              <Button 
                onClick={onNavigateToSchedule}
                className="p-8 w-full max-w-55 bg-card text-card-foreground border border-border hover:bg-accent hover:text-accent-foreground rounded-xl shadow-sm hover:shadow-md transition-all duration-200"
                variant="outline"
              >
                <div className="text-left w-full">
                  <div className="flex items-center gap-3 mb-2">
                    <UserCheck className="h-4 w-4" />
                    <span className="font-semibold text-base">Check My Schedule</span>
                  </div>
                  <p className="text-sm text-muted-foreground mt-2">See your full schedule</p>
                </div>
              </Button>
            )}
          </div>
        </Card>

        {/* Activity and Updates */}
        <div className="w-full max-w-5xl mx-auto flex flex-col md:flex-row gap-6">
          {/* Recent Activity Card */}
          <Card className="p-6 bg-card shadow-lg rounded-2xl min-h-[300px] flex flex-col flex-1">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <Bell className="h-5 w-5" />
              Swap Notifications
            </h2>
            <div className="space-y-3 flex-1">
              {filteredRecentActivity.length > 0 ? (
                <>
                  {filteredRecentActivity.map((activity) => (
                    <div key={activity.id} className="flex items-start gap-3 p-3 rounded-lg bg-muted/50">
                      <div className="p-2 bg-primary/10 rounded-full">
                        <RotateCcw className="h-4 w-4 text-primary" />
                      </div>
                      <div className="flex-1">
                        <p className="text-sm font-medium">{activity.message}</p>
                        <p className="text-xs text-muted-foreground">Received: {activity.date}</p>
                      </div>
                    </div>
                  ))}
                  <Button size="sm" variant="outline" onClick={onNavigateToSwapCalls} className="w-full mt-2">
                    Respond in Swap Calls
                  </Button>
                </>
              ) : (
                <div className="flex-1 flex items-center justify-center">
                  <p className="text-muted-foreground text-center">No pending swap requests</p>
                </div>
              )}
            </div>
          </Card>

          {/* Team Updates Card */}
          <Card className="p-6 bg-card shadow-lg rounded-2xl min-h-[300px] flex flex-col flex-1">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <Users className="h-5 w-5" />
              Team Updates
            </h2>
            <div className="space-y-3 flex-1">
              {dashboardData.teamUpdates.length > 0 ? (
                dashboardData.teamUpdates.map((update) => (
                  <div key={update.id} className="p-3 bg-muted/50 rounded-lg">
                    <p className="text-sm font-medium mb-1">{update.message}</p>
                    <p className="text-xs text-muted-foreground">{update.date}</p>
                  </div>
                ))
              ) : (
                <div className="flex-1 flex items-center justify-center">
                  <p className="text-muted-foreground text-center">No team updates</p>
                </div>
              )}
            </div>
          </Card>
        </div>
      </div>

    </div>
  );
};

export default HomePage;