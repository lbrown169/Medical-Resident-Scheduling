"use client";

import React, { useState, useEffect, useMemo, useCallback } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { CalendarX, Send, Check, X, Shield, Users, Repeat, Search, Trash2 } from "lucide-react";
import { config } from '../../../config';
import { toast } from "../../../lib/use-toast";
import { VacationResponse } from "@/lib/models/VacationResponse";

// Interfaces

interface AdminPageProps {
  residents: { id: string; name: string; email: string; pgyLevel: number | string; hospitalRole?: number; hours: number }[];
  myTimeOffRequests: VacationResponse[];
  shifts: { id: string; name: string }[];
  handleApproveRequest: (id: string) => void;
  handleDenyRequest: (id: string) => void;
  userInvitations: { id: string; email: string; status: "Pending" | "Member" | "Not Invited"; }[];
  inviteEmail: string;
  setInviteEmail: (value: string) => void;
  handleSendInvite: () => void;
  handleResendInvite: (id: string) => void;
  inviteRole: string;
  setInviteRole: (value: string) => void;
  users: { id: string; first_name: string; last_name: string; email: string; role: string }[];
  handleDeleteUser: (user: { id: string; first_name: string; last_name: string; email: string; role: string }) => void;
  latestVersion?: string;
  userId: string;
  onRefreshResidents?: () => void;
}

interface Request {
  id: string;
  firstName: string;
  lastName: string;
  reason: string;
  halfDay?: string | null;
  status: string;
  date: string;
  startDate?: string;
  endDate?: string;
  residentId?: string;
  details?: string;
  groupId: string;
}

interface SwapRequest {
  swapRequestId: string;
  scheduleId: string;
  requesterId: string;
  requesteeId: string;
  requesterDate: string;
  requesteeDate: string;
  status: SwapRequestStatus;
  createdAt: string;
  updatedAt: string;
  isRead: boolean;
  details?: string;
}

interface SwapRequestStatus {
  id: number;
  description: string;
}

interface Announcement {
  announcementId: string;
  message: string;
  createdAt?: string;
}

// Shared helpers

function formatDate(dateStr: string) {
  if (!dateStr) return 'N/A';
  const date = new Date(dateStr);
  date.setMinutes(date.getMinutes() + date.getTimezoneOffset());
  if (isNaN(date.getTime())) return 'N/A';
  return `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}`;
}

// Modal

function Modal({ open, onClose, title, children }: { open: boolean; onClose: () => void; title: string; children: React.ReactNode }) {
  if (!open) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="bg-card p-8 rounded-xl shadow-lg max-w-2xl w-full relative">
        <button onClick={onClose} className="absolute top-4 right-4 text-xl font-bold">&times;</button>
        <h2 className="text-2xl font-bold mb-4">{title}</h2>
        <div className="overflow-y-auto max-h-[60vh]">{children}</div>
      </div>
    </div>
  );
}

// AnnouncementForm

const AnnouncementForm: React.FC<{ userId: string; onPosted: () => void }> = ({ userId, onPosted }) => {
  const [text, setText] = useState('');
  const [posting, setPosting] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleConfirm = async () => {
    setPosting(true);
    setError(null);
    try {
      const res = await fetch(`${config.apiUrl}/api/announcements`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: text, authorId: userId }),
      });
      if (!res.ok) throw new Error();
      setText('');
      onPosted();
    } catch {
      setError('Could not post announcement. Please try again.');
    } finally {
      setPosting(false);
    }
  };

  return (
    <>
      <form onSubmit={e => { e.preventDefault(); setShowConfirm(true); }} className="flex flex-col gap-4 mb-6">
        <textarea
          className="w-full p-3 border border-gray-300 dark:border-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100"
          placeholder="Write a new announcement..."
          value={text}
          onChange={e => setText(e.target.value)}
          rows={3}
          disabled={posting}
        />
        <Button type="submit" disabled={posting || !text.trim()} className="self-end px-6 py-2">
          {posting ? 'Posting...' : 'Post Announcement'}
        </Button>
      </form>
      <ConfirmDialog
        open={showConfirm}
        onOpenChange={setShowConfirm}
        title="Post this announcement?"
        message={text}
        confirmText="Post"
        cancelText="Cancel"
        onConfirm={handleConfirm}
        loading={posting}
      />
      {error && <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">{error}</div>}
    </>
  );
};

// SwapHistoryTab

interface SwapHistoryTabProps {
  idToName: Record<string, string>;
  onPendingCountChange: (count: number) => void;
}

const SwapHistoryTab: React.FC<SwapHistoryTabProps> = ({ idToName, onPendingCountChange }) => {
  const [swapHistory, setSwapHistory] = useState<SwapRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [warnReadId, setWarnReadId] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    fetch(`${config.apiUrl}/api/swaprequests`)
      .then(res => {
        if (!res.ok) throw new Error();
        return res.json();
      })
      .then((data: SwapRequest[]) => {
        setSwapHistory(data);
        onPendingCountChange(data.filter(s => s.status.id === 0).length);
      })
      .catch(() => setSwapHistory([]))
      .finally(() => setLoading(false));
  }, [onPendingCountChange]);

  const handleClearReadSwaps = async () => {
    const readSwapIds = swapHistory.filter((swap) => swap.isRead).map((swap) => swap.swapRequestId);

    if (readSwapIds.length === 0) {
      toast({
        title: "No swaps to delete",
        description: "There are no read swap requests to delete.",
      });
      return;
    }

    try {
      const response = await fetch(`${config.apiUrl}/api/SwapRequests`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(readSwapIds),
      });

      if (response.ok) {
        const result = await response.json();

        if (result.notDeleted && result.notDeleted.length > 0) {
          toast({
            title: "Partial success",
            description: `${readSwapIds.length - result.notDeleted.length} swaps deleted. ${result.notDeleted.length} failed to delete.`,
            variant: "destructive",
          });
        } else {
          toast({
            title: "Success",
            description: "All read swap requests have been deleted.",
          });
        }

        setSwapHistory((prev) =>
          prev.filter((swap) => !swap.isRead)
        );

      } else {
        toast({
          title: "Error",
          description: "Failed to delete swap requests.",
          variant: "destructive",
        });
      }
    } catch {
      toast({
        title: "Error",
        description: "An error occurred while deleting swaps.",
        variant: "destructive",
      });
    }
  };

  const handleWarnMarkAsRead = (swapId: string) => {
    setWarnReadId(swapId);
  };

  const handleMarkAsRead = async (swapId: string) => {
    try {
      const response = await fetch(`${config.apiUrl}/api/SwapRequests/${swapId}`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          isRead: true,
        }),
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(error || "Failed to mark swap request as read.");
      }

      setSwapHistory((prev) =>
        prev.map((swap) =>
          swap.swapRequestId === swapId
            ? { ...swap, isRead: true }
            : swap
        )
      );
    } catch (err) {
      console.error("Error marking swap as read:", err);
      toast({
        variant: "destructive",
        title: "Error",
        description: err instanceof Error ? err.message : "Failed to mark as read.",
      });
    }
  }

  const sortSwaps = (swaps: typeof swapHistory) =>
    [...swaps].sort((a, b) => {
      const pendingA = a.status.id === 0 ? 0 : 1;
      const pendingB = b.status.id === 0 ? 0 : 1;
      if (pendingA !== pendingB) return pendingA - pendingB;
      return new Date(a.requesterDate || "").getTime() - new Date(b.requesterDate || "").getTime();
    });

  const orderedSwapHistory = [
    ...sortSwaps(swapHistory.filter((swap) => !swap.isRead)),
    ...sortSwaps(swapHistory.filter((swap) => swap.isRead)),
  ];

  return (
    <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-2">
        <h2 className="text-lg sm:text-xl font-bold mb-4">Swap Call History</h2>
        <ConfirmDialog
          triggerText={<><Trash2 className="h-4 w-4" /><span>Delete Requests</span></>}
          title="Delete all read swap requests?"
          message="Unread requests will not be deleted. This action cannot be undone."
          confirmText="Delete"
          cancelText="Cancel"
          onConfirm={handleClearReadSwaps}
          variant="danger"
          />
      </div>
      <div className="overflow-x-auto max-h-96 overflow-y-auto w-full">
        <table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead className="bg-gray-100 dark:bg-neutral-800">
            <tr>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Requester</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Requestee</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Partner</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Details</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
            {loading ? (
              <tr>
                <td colSpan={7} className="px-6 py-4 text-center text-gray-400 italic">Loading...</td>
              </tr>
            ) : orderedSwapHistory.length > 0 ? (
                orderedSwapHistory.map((swap, idx) => (
                  <tr key={swap.swapRequestId || idx} className={`${!swap.isRead ? 'bg-yellow-50 dark:bg-yellow-900/20 hover:bg-yellow-100 dark:hover:bg-yellow-900/40' : 'hover:bg-gray-50 dark:hover:bg-neutral-800'}`}>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{swap.requesterDate ? formatDate(swap.requesterDate) : ''}</td>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                      {idToName[swap.requesterId] || `Resident ${swap.requesterId}`}
                    </td>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                      {idToName[swap.requesteeId] || `Resident ${swap.requesteeId}`}
                    </td>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{swap.requesteeDate ? formatDate(swap.requesteeDate) : ''}</td>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 text-sm text-gray-500 dark:text-gray-400 max-w-xs break-all">{swap.details || '-'}</td>
                    <td className={`px-1 sm:px-1 py-3 sm:py-4 whitespace-nowrap text-sm font-semibold ${
                      swap.status.id === 1 ? 'text-green-600' : swap.status.id === 2 ? 'text-red-600' : 'text-yellow-600'
                      }`}>
                      {`${swap.status.description}`}
                    </td>
                    <td className="px-1 sm:px-3 py-3 sm:py-4 whitespace-nowrap text-sm">
                      {!swap.isRead ? (
                        <Button className="inline-flex items-center px-3 py-1.5 text-sm font-medium rounded-md border border-blue-600 text-blue-600 hover:bg-blue-500 hover:text-white bg-transparent shadow-none cursor-pointer" onClick={() => {
                          if (swap.status.id === 0) {
                            handleWarnMarkAsRead(swap.swapRequestId)
                          } else {
                            handleMarkAsRead(swap.swapRequestId)
                          }
                        }}>
                          <Check className="h-3 w-3 mr-1" /> Mark as Read
                        </Button>
                      ) : null}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={7} className="px-6 py-4 text-center text-gray-500 italic">No swap call history found.</td>
                </tr>
            )}
          </tbody>
        </table>
      </div>
      <ConfirmDialog
        open={!!warnReadId}
        onOpenChange={(open) => {
          if (!open) setWarnReadId(null);
        }}
        title="Mark pending request as read?"
        message="This swap request is still pending. Marking it as read makes it possible to delete. Mark as read anyway?"
        confirmText="Mark as Read"
        cancelText="Cancel"
        onConfirm={async () => {
          if (warnReadId) {
            await handleMarkAsRead(warnReadId);
            setWarnReadId(null);
          }
        }}
      />
    </Card>
  );
};

// VacationRequestsTab

interface VacationRequestsTabProps {
  handleApproveRequest: (id: string) => void;
  handleDenyRequest: (id: string) => void;
  onPendingCountChange: (count: number) => void;
}

function groupRequests(requests: Request[]) {
  if (!requests || requests.length === 0) return [];

  const groupedMap = new Map<string, Request[]>();
  for (const req of requests) {
    const key = req.groupId || req.id;
    if (!groupedMap.has(key)) groupedMap.set(key, []);
    groupedMap.get(key)!.push(req);
  }

  const result = [];
  for (const [, entries] of groupedMap.entries()) {
    const sorted = entries.sort((a, b) =>
      new Date(a.startDate || "").getTime() - new Date(b.startDate || "").getTime()
    );
    const first = sorted[0];
    const last = sorted[sorted.length - 1];
    result.push({
      id: first.id,
      residentId: first.residentId,
      firstName: first.firstName,
      lastName: first.lastName,
      reason: first.reason,
      halfDay: first.halfDay ?? null,
      status: first.status,
      startDate: first.startDate,
      endDate: last.endDate,
      groupId: first.groupId,
      details: first.details,
    });
  }

  result.sort((a, b) => {
    const pendingA = a.status === "Pending" ? 0 : 1;
    const pendingB = b.status === "Pending" ? 0 : 1;
    if (pendingA !== pendingB) return pendingA - pendingB;
    return new Date(a.startDate || "").getTime() - new Date(b.startDate || "").getTime();
  });
  return result;
}

const VacationRequestsTab: React.FC<VacationRequestsTabProps> = ({ handleApproveRequest, handleDenyRequest, onPendingCountChange }) => {
  const [requests, setRequests] = useState<Request[]>([]);
  const [loading, setLoading] = useState(true);
  const [showSearchModal, setShowSearchModal] = useState(false);
  const [searchDate, setSearchDate] = useState('');

  const fetchVacations = useCallback(() => {
    setLoading(true);
    fetch(`${config.apiUrl}/api/vacations`)
      .then(res => res.json())
      .then((data) => {
        const mapped: Request[] = data.map((vac: {
          vacationId: string;
          firstName: string;
          lastName: string;
          date: string;
          reason: string;
          halfDay?: string | null;
          status: string;
          residentId: string;
          details?: string;
          groupId: string;
        }) => ({
          id: vac.vacationId,
          firstName: vac.firstName,
          lastName: vac.lastName,
          date: vac.date,
          startDate: vac.date,
          endDate: vac.date,
          reason: vac.reason,
          halfDay: vac.halfDay ?? null,
          status: vac.status,
          residentId: vac.residentId,
          details: vac.details,
          groupId: vac.groupId,
        }));
        setRequests(mapped);
      })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => { fetchVacations(); }, [fetchVacations]);

  const groupedRequests = useMemo(() => groupRequests(requests), [requests]);

  useEffect(() => {
    onPendingCountChange(groupedRequests.filter(r => r.status === 'Pending').length);
  }, [groupedRequests, onPendingCountChange]);

  const handleClearAllRequests = async () => {
    const vacationIds = requests.filter(r => r.status !== 'Pending').map(r => r.id);
    if (vacationIds.length === 0) {
      toast({ title: 'No requests to delete', description: 'There are no vacation requests to delete.' });
      return;
    }
    try {
      const response = await fetch(`${config.apiUrl}/api/vacations`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(vacationIds),
      });
      if (response.ok) {
        const result = await response.json();
        if (result.notDeleted && result.notDeleted.length > 0) {
          toast({
            title: 'Partial success',
            description: `${vacationIds.length - result.notDeleted.length} requests deleted. ${result.notDeleted.length} failed to delete.`,
            variant: 'destructive',
          });
        } else {
          toast({ title: 'Success', description: 'All vacation requests have been deleted.' });
        }
        fetchVacations();
      } else {
        toast({ title: 'Error', description: 'Failed to delete vacation requests.', variant: 'destructive' });
      }
    } catch {
      toast({ title: 'Error', description: 'An error occurred while deleting requests.', variant: 'destructive' });
    }
  };

  const getRequestDate = (request: Request) => {
    if (request.startDate && request.endDate) {
      return request.startDate === request.endDate
        ? formatDate(request.startDate)
        : `${formatDate(request.startDate)} - ${formatDate(request.endDate)}`;
    }
    return 'N/A';
  };

  const getResidentName = (request: Request) => {
    if (request.firstName && request.lastName) return `${request.firstName} ${request.lastName}`;
    return 'N/A';
  };

  const requestsTable = (
    <table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
      <thead className="bg-gray-100 dark:bg-neutral-800">
        <tr>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date Range</th>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Resident</th>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Reason</th>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Details</th>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
          <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
        </tr>
      </thead>
      <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
        {loading ? (
          <tr>
            <td colSpan={6} className="px-6 py-4 text-center text-gray-400 italic">Loading...</td>
          </tr>
        ) : groupedRequests.length > 0 ? (
          groupedRequests.map((request: Request, idx: number) => (
            <tr key={request.id || `${request.startDate || request.date || ''}-${getResidentName(request)}-${idx}`}
              className={`${request.status === 'Pending' ? 'bg-yellow-50 dark:bg-yellow-900/20 hover:bg-yellow-100 dark:hover:bg-yellow-900/40' : 'hover:bg-gray-50 dark:hover:bg-neutral-800'}`}>
              <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">{getRequestDate(request)}</td>
              <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{getResidentName(request)}</td>
              <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {request.reason}{request.halfDay === "A" ? " (AM)" : request.halfDay === "P" ? " (PM)" : ""}
              </td>
              <td className="px-2 sm:px-6 py-4 text-sm text-gray-500 dark:text-gray-400 max-w-xs break-all">{request.details || '-'}</td>
              <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{request.status}</td>
              <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                {request.status === "Pending" && (
                  <div className="flex flex-col sm:flex-row items-center space-y-2 sm:space-y-0 sm:space-x-2">
                    <ConfirmDialog
                      triggerText={<><Check className="h-4 w-4 mr-1" /> Approve</>}
                      className="inline-flex items-center px-3 py-1.5 text-sm font-medium rounded-md border border-green-600 text-green-600 hover:bg-green-500 hover:text-white bg-transparent shadow-none"
                      title="Approve this request?"
                      message={`Approve time off for ${request.firstName} ${request.lastName}?`}
                      confirmText="Approve"
                      cancelText="Cancel"
                      onConfirm={async () => { await Promise.resolve(handleApproveRequest(request.groupId || '')); fetchVacations(); }}
                    />
                    <ConfirmDialog
                      triggerText={<><X className="h-4 w-4 mr-1" /> Deny</>}
                      className="inline-flex items-center px-3 py-1.5 text-sm font-medium rounded-md border border-red-600 text-red-600 hover:bg-red-500 hover:text-white bg-transparent shadow-none"
                      title="Deny this request?"
                      message={`Deny time off for ${request.firstName} ${request.lastName}?`}
                      confirmText="Deny"
                      cancelText="Cancel"
                      onConfirm={async () => { await Promise.resolve(handleDenyRequest(request.groupId || '')); fetchVacations(); }}
                    />
                  </div>
                )}
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={6} className="px-6 py-4 text-center text-gray-500 italic">No time off requests found.</td>
          </tr>
        )}
      </tbody>
    </table>
  );

  return (
    <>
      <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-2">
          <h2 className="text-lg sm:text-xl font-bold">Time Off Requests</h2>
          <div className="flex gap-2">
            <Button variant="outline" className="flex items-center gap-2 px-1 sm:px-6 py-1 sm:py-3 text-xs sm:text-sm lg:text-base"
              onClick={() => setShowSearchModal(true)}>
              <Search className="h-4 w-4" />
              <span>Search</span>
            </Button>
            <ConfirmDialog
              triggerText={<><Trash2 className="h-4 w-4" /><span>Delete Requests</span></>}
              title="Delete all vacation requests?"
              message="Pending requests will not be deleted. This action cannot be undone."
              confirmText="Delete"
              cancelText="Cancel"
              onConfirm={handleClearAllRequests}
              variant="danger"
            />
          </div>
        </div>
        <div className="overflow-x-auto max-h-96 overflow-y-auto w-full">
          {requestsTable}
        </div>
      </Card>
      <Modal open={showSearchModal} onClose={() => { setShowSearchModal(false); setSearchDate(''); }} title="Search Date">
        <div className="flex flex-col gap-4">
          <div className="flex items-center gap-3">
            <label className="text-sm font-medium text-gray-700 dark:text-gray-300 whitespace-nowrap">Select date:</label>
            <input
              type="date"
              value={searchDate}
              onChange={e => setSearchDate(e.target.value)}
              className="border border-gray-300 dark:border-gray-600 rounded-md px-3 py-1.5 text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100"
            />
          </div>
          {searchDate && (() => {
            const selected = new Date(searchDate);
            selected.setMinutes(selected.getMinutes() + selected.getTimezoneOffset());
            const matched = requests.filter(r => {
              const start = new Date(r.startDate || r.date || '');
              start.setMinutes(start.getMinutes() + start.getTimezoneOffset());
              const end = new Date(r.endDate || r.date || '');
              end.setMinutes(end.getMinutes() + end.getTimezoneOffset());
              return selected >= start && selected <= end;
            });
            return matched.length === 0 ? (
              <p className="text-sm text-green-600 font-medium">No requests found for this date.</p>
            ) : (
              <div className="flex flex-col gap-2">
                <p className="text-sm text-gray-500">{matched.length} request{matched.length !== 1 ? 's' : ''} found:</p>
                <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                  <thead className="bg-gray-100 dark:bg-neutral-800">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Resident</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Reason</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
                    {matched.map((r, i) => (
                      <tr key={`${r.residentId}-${i}`} className="hover:bg-gray-50 dark:hover:bg-neutral-800">
                        <td className="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">{r.firstName} {r.lastName}</td>
                        <td className="px-4 py-3 text-sm text-gray-500 dark:text-gray-400">{r.reason}{r.halfDay === 'A' ? ' (AM)' : r.halfDay === 'P' ? ' (PM)' : ''}</td>
                        <td className={`px-4 py-3 text-sm font-medium ${r.status === 'Pending' ? 'text-yellow-600' : r.status === 'Denied' ? 'text-red-600' : 'text-green-600'}`}>{r.status}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            );
          })()}
        </div>
      </Modal>
    </>
  );
};

// UserManagementTab

type UserRecord = { id: string; first_name: string; last_name: string; email: string; role: string };

interface UserManagementTabProps {
  users: UserRecord[];
  userInvitations: { id: string; email: string; status: "Pending" | "Member" | "Not Invited"; }[];
  inviteEmail: string;
  setInviteEmail: (value: string) => void;
  handleSendInvite: () => void;
  handleResendInvite: (id: string) => void;
  handleDeleteUser: (user: UserRecord) => void;
}

const UserManagementTab: React.FC<UserManagementTabProps> = ({
  users,
  userInvitations,
  inviteEmail,
  setInviteEmail,
  handleSendInvite,
  handleResendInvite,
  handleDeleteUser,
}) => {
  const [confirmDelete, setConfirmDelete] = useState<{ open: boolean; user: UserRecord | null }>({ open: false, user: null });
  const [switchingRole, setSwitchingRole] = useState<string | null>(null);
  const [showInvitationsModal, setShowInvitationsModal] = useState(false);

  const handleDeleteUserWithConfirm = (user: UserRecord) => setConfirmDelete({ open: true, user });
  const handleConfirmDelete = () => {
    if (confirmDelete.user) {
      handleDeleteUser(confirmDelete.user);
      toast({ title: "User deleted", description: `${confirmDelete.user.first_name} ${confirmDelete.user.last_name} has been deleted.`, variant: "success" });
    }
    setConfirmDelete({ open: false, user: null });
  };
  const handleCancelDelete = () => setConfirmDelete({ open: false, user: null });

  const handleSwitchRole = async (user: UserRecord, newRole: string) => {
    if (user.role === newRole) return;
    setSwitchingRole(user.id);
    try {
      const endpoint = user.role === 'admin' ? 'Residents/demote-admin' : 'Admins/promote-resident';
      const response = await fetch(`${config.apiUrl}/api/${endpoint}/${user.id}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      });
      if (response.ok) {
        toast({ title: "Role Updated", description: `${user.first_name} ${user.last_name} has been switched to ${newRole}.`, variant: "success" });
        window.location.reload();
      } else {
        const error = await response.text();
        toast({ title: "Error", description: error || "Failed to switch user role.", variant: "destructive" });
      }
    } catch {
      toast({ title: "Error", description: "Failed to switch user role. Please try again.", variant: "destructive" });
    } finally {
      setSwitchingRole(null);
    }
  };

  const invitationsTable = (
    <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
      <thead className="bg-gray-100 dark:bg-neutral-800">
        <tr>
          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
        </tr>
      </thead>
      <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
        {userInvitations.length > 0 ? (
          userInvitations.map((invite) => (
            <tr key={invite.id} className="hover:bg-gray-50 dark:hover:bg-neutral-800">
              <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">{invite.email}</td>
              <td className={`px-6 py-4 whitespace-nowrap text-sm font-semibold ${invite.status === "Pending" ? "text-yellow-600" : invite.status === "Member" ? "text-green-600" : "text-gray-500"}`}>
                {invite.status}
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                {invite.status === "Pending" && (
                  <Button variant="outline" size="sm" className="text-blue-600 border-blue-600 hover:bg-blue-500 hover:text-white" onClick={() => handleResendInvite(invite.id || '')}>
                    Resend
                  </Button>
                )}
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={3} className="px-6 py-4 text-center text-gray-500 italic">No pending invitations.</td>
          </tr>
        )}
      </tbody>
    </table>
  );

  return (
    <>
      <Card className="p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-8 mb-8 border border-gray-200 dark:border-gray-800">
        {/* User Invitations Section */}
        <div>
          <h2 className="text-xl font-bold mb-2">User Invitations</h2>
          <div className="flex flex-col sm:flex-row gap-4 mb-4">
            <input
              type="email"
              placeholder="Enter resident email address"
              className="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100"
              value={inviteEmail}
              onChange={(e) => setInviteEmail(e.target.value)}
            />
            <Button onClick={handleSendInvite} className="py-2 flex items-center justify-center gap-2">
              <Send className="h-5 w-5" />
              <span>Send Invitation</span>
            </Button>
          </div>
          <div className="overflow-x-auto max-h-60 overflow-y-auto mb-6">
            {invitationsTable}
          </div>
        </div>

        {/* User Management Table Section */}
        <div>
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-bold">User Management</h2>
          </div>
          <div className="overflow-x-auto max-h-96 overflow-y-auto">
            <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
              <thead className="bg-gray-100 dark:bg-neutral-800">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
                {users.length > 0 ? (
                  [...users].sort((a, b) => a.first_name.localeCompare(b.first_name) || a.last_name.localeCompare(b.last_name)).map((user) => (
                    <tr key={user.id} className="hover:bg-gray-50 dark:hover:bg-neutral-800">
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">{user.first_name} {user.last_name}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{user.email}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <select
                          value={user.role}
                          onChange={(e) => handleSwitchRole(user, e.target.value)}
                          disabled={switchingRole === user.id}
                          className="px-2 py-1 border border-gray-300 dark:border-gray-600 rounded text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        >
                          <option value="resident">{switchingRole === user.id ? 'Switching...' : 'Resident'}</option>
                          <option value="admin">{switchingRole === user.id ? 'Switching...' : 'Admin'}</option>
                        </select>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <Button variant="outline" size="sm" className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white" onClick={() => handleDeleteUserWithConfirm(user)}>
                          <Trash2 className="h-4 w-4 mr-1 inline" />Delete
                        </Button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={4} className="px-6 py-4 text-center text-gray-500 italic">No users found.</td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </Card>

      <Modal open={showInvitationsModal} onClose={() => setShowInvitationsModal(false)} title="All User Invitations">
        <div className="overflow-x-auto">{invitationsTable}</div>
      </Modal>

      <Modal open={confirmDelete.open} onClose={handleCancelDelete} title="Confirm Delete">
        <div className="space-y-4">
          <p>Are you sure you want to delete {confirmDelete.user?.first_name} {confirmDelete.user?.last_name}?</p>
          <div className="flex gap-2 justify-end">
            <Button variant="outline" onClick={handleCancelDelete}>Cancel</Button>
            <Button className="bg-red-600 text-white hover:bg-red-700" onClick={handleConfirmDelete}>Delete</Button>
          </div>
        </div>
      </Modal>
    </>
  );
};

// ResidentInfoTab

interface ResidentInfoTabProps {
  residents: { id: string; name: string; email: string; pgyLevel: number | string; hospitalRole?: number; hours: number }[];
  onRefreshResidents?: () => void;
}

const ResidentInfoTab: React.FC<ResidentInfoTabProps> = ({ residents, onRefreshResidents }) => {
  const [residentRows, setResidentRows] = useState(residents);
  useEffect(() => setResidentRows(residents), [residents]);

  const sortedResidentRows = useMemo(
    () => [...residentRows].sort((a, b) => a.name.localeCompare(b.name)),
    [residentRows]
  );

  const [savingPGY, setSavingPGY] = useState<Record<string, boolean>>({});

  const handleUpdatePGY = async (residentId: string, newPGY: number) => {
    setSavingPGY(prev => ({ ...prev, [residentId]: true }));
    setResidentRows(prev => prev.map(r => r.id === residentId ? { ...r, pgyLevel: newPGY } : r));
    try {
      const res = await fetch(`${config.apiUrl}/api/residents/${residentId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ graduate_yr: newPGY }),
      });
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || 'Failed to update PGY');
      }
      toast({ title: 'PGY updated', description: `Resident set to PGY ${newPGY}.`, variant: 'success' });
      onRefreshResidents?.();
    } catch (error) {
      setResidentRows(prev => prev.map(r => r.id === residentId ? { ...r, pgyLevel: residents.find(x => x.id === residentId)?.pgyLevel ?? r.pgyLevel } : r));
      toast({ title: 'Update failed', description: (error as Error)?.message || 'Could not update PGY.', variant: 'destructive' });
    } finally {
      setSavingPGY(prev => ({ ...prev, [residentId]: false }));
    }
  };

  return (
    <Card className="p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-1 mb-8 border border-gray-200 dark:border-gray-800">
      <div className="flex flex-row justify-between items-center mb-4 gap-2">
        <h2 className="text-lg sm:text-xl font-bold">Resident Information</h2>
        <ConfirmDialog
          triggerText={<><Users className="h-4 w-4" /><span>Promote Residents</span></>}
          title="Promote all residents?"
          message="This will increase all PGY levels by 1."
          confirmText="Promote"
          cancelText="Cancel"
          onConfirm={async () => {
            try {
              const res = await fetch(`${config.apiUrl}/api/residents/promote-pgy`, { method: 'POST' });
              if (!res.ok) throw new Error();
              toast({ title: 'Residents promoted', description: 'All PGY levels have been increased by 1.', variant: 'success' });
              onRefreshResidents?.();
            } catch {
              toast({ title: 'Promotion failed', description: 'Could not promote residents. Please try again.', variant: 'destructive' });
            }
          }}
        />
      </div>
      <div className="overflow-x-auto max-h-96 overflow-y-auto w-full">
        <table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead className="bg-gray-100 dark:bg-neutral-800">
            <tr>
              <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Resident</th>
              <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
              <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Current PGY Status</th>
              <th className="px-2 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Hours Scheduled This Semester</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
            {sortedResidentRows.length > 0 ? (
              sortedResidentRows.map((resident) => (
                <tr key={resident.id} className="hover:bg-gray-50 dark:hover:bg-neutral-800">
                  <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{resident.name}</td>
                  <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">{resident.email}</td>
                  <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                    <select
                      value={resident.pgyLevel ?? ''}
                      onChange={(e) => handleUpdatePGY(resident.id, Number(e.target.value))}
                      disabled={!!savingPGY[resident.id]}
                      className="px-2 py-1 border border-gray-300 dark:border-gray-600 rounded text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 cursor-pointer"
                    >
                      {[0, 1, 2, 3, 4].map(n => (
                        <option key={n} value={n}>PGY {n}</option>
                      ))}
                    </select>
                  </td>
                  <td className="px-2 sm:px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{resident.hours}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4} className="px-6 py-4 text-center text-gray-500 italic">No residents found.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </Card>
  );
};

// AdminPage

const AdminPage: React.FC<AdminPageProps> = ({
  residents,
  handleApproveRequest,
  handleDenyRequest,
  userInvitations,
  inviteEmail,
  setInviteEmail,
  handleSendInvite,
  handleResendInvite,
  users,
  handleDeleteUser,
  userId,
  onRefreshResidents,
}) => {
  const [activeTab, setActiveTab] = useState<'swaps' | 'requests' | 'users' | 'residents' | 'announcements'>('swaps');
  const [pendingSwapsCount, setPendingSwapsCount] = useState(0);
  const [pendingRequestsCount, setPendingRequestsCount] = useState(0);
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);
  const [deletingAnnouncement, setDeletingAnnouncement] = useState<string | null>(null);

  const idToName = useMemo(() => {
    const mapping: Record<string, string> = {};
    users.forEach(user => { mapping[user.id] = `${user.first_name} ${user.last_name}`; });
    return mapping;
  }, [users]);

  useEffect(() => {
    fetch(`${config.apiUrl}/api/swaprequests`)
      .then(res => res.ok ? res.json() : [])
      .then((data: SwapRequest[]) => setPendingSwapsCount(data.filter(s => s.status.id === 0).length))
      .catch(() => {});
    fetch(`${config.apiUrl}/api/vacations`)
      .then(res => res.ok ? res.json() : [])
      .then((data: { status: string; groupId: string }[]) => {
        const pendingGroups = new Set(data.filter(v => v.status === 'Pending').map(v => v.groupId));
        setPendingRequestsCount(pendingGroups.size);
      })
      .catch(() => {});
  }, []);

  useEffect(() => {
    if (activeTab === 'announcements') {
      fetch(`${config.apiUrl}/api/announcements`)
        .then(res => res.ok ? res.json() : [])
        .then(setAnnouncements)
        .catch(() => {});
    }
  }, [activeTab]);

  const refreshAnnouncements = async () => {
    const data = await fetch(`${config.apiUrl}/api/announcements`).then(r => r.json());
    setAnnouncements(data);
  };

  const handleDeleteAnnouncement = async (announcementId: string) => {
    setDeletingAnnouncement(announcementId);
    try {
      const res = await fetch(`${config.apiUrl}/api/announcements/${announcementId}`, { method: 'DELETE' });
      if (!res.ok) throw new Error();
      const data = await fetch(`${config.apiUrl}/api/announcements`).then(r => r.json());
      setAnnouncements(data);
    } catch {
      toast({ title: 'Could not delete announcement', variant: 'destructive' });
    } finally {
      setDeletingAnnouncement(null);
    }
  };

  const handlePendingSwapsChange = useCallback((count: number) => setPendingSwapsCount(count), []);
  const handlePendingRequestsChange = useCallback((count: number) => setPendingRequestsCount(count), []);

  return (
    <div className="w-full pt-4 h-[calc(100vh-4rem)] flex flex-col items-center px-4 md:pl-8">
      {/* Dashboard Overview Card */}
      <Card className="mb-8 p-6 flex flex-col gap-4 items-center justify-between bg-white dark:bg-neutral-900 shadow-lg rounded-2xl border border-gray-200 dark:border-gray-800">
        <h2 className="text-2xl font-bold flex items-center gap-2 justify-center w-full mb-2">
          <Shield className="w-6 h-6 text-blue-600" />
          Admin Dashboard
        </h2>
        <div className="flex flex-col md:flex-row items-center w-full justify-between gap-4">
          <div />
          <div className="flex flex-col sm:flex-row gap-4 md:gap-8 items-center">
            <div className="flex flex-col items-center">
              <div className="flex items-center gap-2 mb-1">
                <Users className="w-5 h-5 text-blue-500" />
                <span className="text-2xl font-bold text-gray-900 dark:text-white">{residents.length}</span>
              </div>
              <span className="text-xs text-gray-500">Residents</span>
            </div>
            <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
              <div className="flex items-center gap-2 mb-1">
                <Repeat className="w-5 h-5 text-yellow-500" />
                <span className="text-2xl font-bold text-gray-900 dark:text-white">{pendingSwapsCount}</span>
              </div>
              <span className="text-xs text-gray-500">Pending Swaps</span>
            </div>
            <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
              <div className="flex items-center gap-2 mb-1">
                <CalendarX className="w-5 h-5 text-green-500" />
                <span className="text-2xl font-bold text-gray-900 dark:text-white">{pendingRequestsCount}</span>
              </div>
              <span className="text-xs text-gray-500">Pending Time Off</span>
            </div>
          </div>
        </div>
      </Card>

      {/* Tab Navigation */}
      <div className="w-full max-w-6xl flex flex-col sm:flex-row gap-1 sm:gap-2 mb-4 sm:mb-6">
        <Button
          variant={activeTab === 'swaps' ? 'default' : 'outline'}
          className={`flex-1 cursor-pointer rounded-b-none sm:rounded-br-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'swaps' ? 'shadow-md' : ''}`}
          onClick={() => setActiveTab('swaps')}
        >
          Swap Call History
        </Button>
        <Button
          variant={activeTab === 'requests' ? 'default' : 'outline'}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'requests' ? 'shadow-md' : ''}`}
          onClick={() => setActiveTab('requests')}
        >
          Time Off Requests
        </Button>
        <Button
          variant={activeTab === 'users' ? 'default' : 'outline'}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'users' ? 'shadow-md' : ''}`}
          onClick={() => setActiveTab('users')}
        >
          User Management
        </Button>
        <Button
          variant={activeTab === 'residents' ? 'default' : 'outline'}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'residents' ? 'shadow-md' : ''}`}
          onClick={() => setActiveTab('residents')}
        >
          Resident Information
        </Button>
        <Button
          variant={activeTab === 'announcements' ? 'default' : 'outline'}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'announcements' ? 'shadow-md' : ''}`}
          onClick={() => setActiveTab('announcements')}
        >
          Announcements
        </Button>
      </div>

      {/* Tab Content */}
      <div className="w-full">
        {activeTab === 'swaps' && (
          <SwapHistoryTab idToName={idToName} onPendingCountChange={handlePendingSwapsChange} />
        )}
        {activeTab === 'requests' && (
          <VacationRequestsTab
            handleApproveRequest={handleApproveRequest}
            handleDenyRequest={handleDenyRequest}
            onPendingCountChange={handlePendingRequestsChange}
          />
        )}
        {activeTab === 'users' && (
          <UserManagementTab
            users={users}
            userInvitations={userInvitations}
            inviteEmail={inviteEmail}
            setInviteEmail={setInviteEmail}
            handleSendInvite={handleSendInvite}
            handleResendInvite={handleResendInvite}
            handleDeleteUser={handleDeleteUser}
          />
        )}
        {activeTab === 'residents' && (
          <ResidentInfoTab residents={residents} onRefreshResidents={onRefreshResidents} />
        )}
        {activeTab === 'announcements' && (
          <Card className="p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-8 mb-8 border border-gray-200 dark:border-gray-800">
            <h2 className="text-xl font-bold mb-4">Announcements</h2>
            <AnnouncementForm userId={userId} onPosted={refreshAnnouncements} />
            <div className="flex flex-col gap-4">
              {announcements.length === 0 && (
                <div className="text-gray-500">No announcements yet.</div>
              )}
              {announcements.map((a, idx) => (
                <div key={a.announcementId || idx} className="p-4 bg-white dark:bg-neutral-800 border border-gray-200 dark:border-gray-700 rounded-md shadow-sm">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="text-gray-900 dark:text-gray-100 mb-1">{a.message}</div>
                      <div className="text-xs text-gray-500">{a.createdAt ? new Date(a.createdAt).toLocaleString() : ''}</div>
                    </div>
                    <Button
                      variant="outline"
                      size="sm"
                      className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white ml-4"
                      onClick={() => handleDeleteAnnouncement(a.announcementId)}
                      disabled={deletingAnnouncement === a.announcementId}
                    >
                      <Trash2 className="h-4 w-4 mr-1 inline" />{deletingAnnouncement === a.announcementId ? 'Deleting...' : 'Delete'}
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </Card>
        )}
      </div>
    </div>
  );
};

export default AdminPage;
