"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { Calendar, Clock, FileText, Send, AlertTriangle, CalendarX } from "lucide-react";
import { config } from '../../../config';

interface RequestOffPageProps {
  userId: string;
  startDate: string;
  setStartDate: (value: string) => void;
  endDate: string;
  setEndDate: (value: string) => void;
  reason: string;
  setReason: (value: string) => void;
  leaveReasons: { id: string; name: string }[];
  description: string;
  setDescription: (value: string) => void;
  handleSubmitRequestOff: () => void;
}

type ApiVacation = {
  vacationId: string;
  residentId: string;
  firstName: string;
  lastName: string;
  date: string;
  reason: string;
  status: "Pending" | "Approved" | "Denied" | string;
  details?: string | null;
  groupId?: string | null;
};

type GroupedRequest = {
  groupId: string;
  reason: string;
  status: string;
  details?: string | null;
  dates: string[];
};

const RequestOffPage: React.FC<RequestOffPageProps> = ({
  userId,
  startDate,
  setStartDate,
  endDate,
  setEndDate,
  reason,
  setReason,
  leaveReasons,
  description,
  setDescription,
  handleSubmitRequestOff,
}) => {
  const [showConfirmation, setShowConfirmation] = useState(false);
  const isFormValid = startDate && endDate && reason;

  // Fetched requests state
  const [loadingRequests, setLoadingRequests] = useState(false);
  const [requests, setRequests] = useState<ApiVacation[]>([]);
  const [errorRequests, setErrorRequests] = useState<string | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  // Fetch this resident's requests
 useEffect(() => {
  if (!userId) {
    setRequests([]);
    setErrorRequests("Missing userId.");
    return;
  }

  let abort = false;

  (async () => {
    setLoadingRequests(true);
    setErrorRequests(null);
    try {
      const url = `${config.apiUrl}/api/vacations?residentId=${encodeURIComponent(userId)}`;
      const res = await fetch(url, { cache: "no-store" });

      if (res.status === 404) {
        if (!abort) setRequests([]);
        return;
      }
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || `HTTP ${res.status}`);
      }

      const data: ApiVacation[] = await res.json();
      if (!abort) setRequests(Array.isArray(data) ? data : [data]);
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : "Failed to load requests.";
      if (!abort) setErrorRequests(msg);
    } finally {
      if (!abort) setLoadingRequests(false);
    }
  })();

  return () => {
    abort = true;
  };
}, [userId, refreshKey]);

  // Group by submission (GroupId), newest group last-date first
  const grouped: GroupedRequest[] = useMemo(() => {
    const mp = new Map<string, GroupedRequest>();
    for (const v of requests) {
      const key = v.groupId || v.vacationId; // single-day fallback
      const g = mp.get(key);
      if (!g) {
        mp.set(key, {
          groupId: key,
          reason: v.reason,
          status: v.status,
          details: v.details ?? null,
          dates: [v.date],
        });
      } else {
        g.dates.push(v.date);
        // normalize status: Denied > Pending > Approved
        if (v.status === "Denied") g.status = "Denied";
        else if (v.status === "Pending" && g.status !== "Denied") g.status = "Pending";
      }
    }
    const arr = Array.from(mp.values());
    arr.forEach((g) => g.dates.sort((a, b) => +new Date(a) - +new Date(b)));
    arr.sort(
      (A, B) =>
        +new Date(B.dates[B.dates.length - 1]) - +new Date(A.dates[A.dates.length - 1])
    );
    return arr;
  }, [requests]);

  const handleInitialSubmit = () => {
    if (isFormValid) {
      setShowConfirmation(true);
    }
  };

  const handleConfirmSubmit = () => {
    handleSubmitRequestOff();
    setShowConfirmation(false);
  };

  const handleCancelSubmit = () => {
    setShowConfirmation(false);
  };

  // Calculate number of days
  const calculateDays = () => {
    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      const diffTime = Math.abs(end.getTime() - start.getTime());
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
      return diffDays;
    }
    return 0;
  };

  const StatusPill: React.FC<{ status: string }> = ({ status }) => {
    const base = "px-2 py-0.5 text-xs font-semibold rounded-full border";
    if (status === "Approved")
      return (
        <span className={`${base} bg-emerald-50 dark:bg-emerald-900/20 border-emerald-200 dark:border-emerald-800 text-emerald-700 dark:text-emerald-300`}>
          Approved
        </span>
      );
    if (status === "Denied")
      return (
        <span className={`${base} bg-rose-50 dark:bg-rose-900/20 border-rose-200 dark:border-rose-800 text-rose-700 dark:text-rose-300`}>
          Denied
        </span>
      );
    return (
      <span className={`${base} bg-amber-50 dark:bg-amber-900/20 border-amber-200 dark:border-amber-800 text-amber-700 dark:text-amber-300`}>
        Pending
      </span>
    );
  };

  const fmt = (d: string) => {
    // Apply timezone offset to keep date local (same as calendar)
    const date = new Date(d);
    date.setMinutes(date.getMinutes() + date.getTimezoneOffset());
    return date.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      year: "numeric",
    });
  };

  return (
    <div className="w-full h-full bg-background p-4 overflow-hidden">
      <div className="max-w-xl mx-auto h-full flex flex-col">
        {/* Header */}
        <div className="text-center mb-5">
          <div className="flex justify-center mb-2">
            <div className="p-2.5 bg-primary/10 rounded-full">
              <CalendarX className="h-5 w-5 text-primary" />
            </div>
          </div>
          <h1 className="text-2xl font-bold text-foreground mb-1">Request Time Off</h1>
          <p className="text-sm text-muted-foreground">
            Submit your time off request below. Please specify the dates and reason.
          </p>
        </div>

        {/* Main Form Card */}
        <Card className="p-5 shadow-lg border border-border flex-1 flex flex-col">
          <div className="space-y-5 flex-1">
            {/* Start Date */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-primary" />
                <label htmlFor="start-date" className="text-sm font-semibold text-foreground">
                  Start Date
                </label>
              </div>
              <input
                id="start-date"
                type="date"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                min={new Date().toISOString().split('T')[0]}
              />
            </div>

            {/* End Date */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-primary" />
                <label htmlFor="end-date" className="text-sm font-semibold text-foreground">
                  End Date
                </label>
              </div>
              <input
                id="end-date"
                type="date"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                min={startDate || new Date().toISOString().split('T')[0]}
              />
            </div>

            {/* Reason Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Clock className="h-4 w-4 text-primary" />
                <label htmlFor="reason-select" className="text-sm font-semibold text-foreground">
                  Reason for Request
                </label>
              </div>
              <select
                id="reason-select"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
              >
                <option value="" disabled>Select a reason</option>
                {leaveReasons.map((r) => (
                  <option key={r.id} value={r.name}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Additional Details */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <FileText className="h-4 w-4 text-primary" />
                <label htmlFor="description-box" className="text-sm font-semibold text-foreground">
                  Additional Details <span className="text-muted-foreground font-normal">(Optional)</span>
                </label>
              </div>
              <textarea
                id="description-box"
                rows={2}
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors resize-none"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="e.g., specific duties, contact info, special considerations..."
              />
            </div>

            {/* Form Summary */}
            {isFormValid && (
              <div className="bg-muted/50 rounded-lg p-2.5 border border-border">
                <h3 className="font-semibold text-foreground mb-1.5 flex items-center gap-1.5 text-sm">
                  <CalendarX className="h-4 w-4" />
                  Request Summary
                </h3>
                <div className="space-y-0.5 text-sm">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Start Date:</span>
                    <span className="font-medium text-foreground">
                      {new Date(startDate).toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric',
                        timeZone: 'UTC'
                      })}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">End Date:</span>
                    <span className="font-medium text-foreground">
                      {new Date(endDate).toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric',
                        timeZone: 'UTC' 
                      })}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Duration:</span>
                    <span className="font-medium text-foreground">
                      {calculateDays()} day{calculateDays() !== 1 ? 's' : ''}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Reason:</span>
                    <span className="font-medium text-foreground">
                      {reason}
                    </span>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Submit Button / Confirmation */}
          <div className="pt-3 mt-auto">
            {!showConfirmation ? (
              <>
                <Button 
                  onClick={handleInitialSubmit} 
                  className="w-full py-2 text-sm font-semibold flex items-center justify-center gap-2 transition-all duration-200"
                  disabled={!isFormValid}
                >
                  <Send className="h-3.5 w-3.5" />
                  <span>Submit Request</span>
                </Button>
                {!isFormValid && (
                  <p className="text-xs text-muted-foreground mt-1.5 text-center">
                    Complete all required fields to submit
                  </p>
                )}
              </>
            ) : (
              <div className="space-y-2.5">
                <div className="bg-yellow-50 dark:bg-yellow-950/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-2.5">
                  <div className="flex items-center gap-2 mb-1.5">
                    <AlertTriangle className="h-3.5 w-3.5 text-yellow-600 dark:text-yellow-400" />
                    <h3 className="text-xs font-semibold text-yellow-800 dark:text-yellow-200">
                      Confirm Time Off Request
                    </h3>
                  </div>
                  <p className="text-xs text-yellow-700 dark:text-yellow-300 mb-1.5">
                    Are you sure you want to submit this time off request? This will be sent to your supervisor for approval.
                  </p>
                  <div className="text-xs text-yellow-600 dark:text-yellow-400">
                    <strong>Duration:</strong> {new Date(startDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric', timeZone: 'UTC' })} - {new Date(endDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric', timeZone: 'UTC' })} ({calculateDays()} day{calculateDays() !== 1 ? 's' : ''})<br/>
                    <strong>Reason:</strong> {reason}
                    {description && (
                      <>
                        <br/>
                        <strong>Details:</strong> {description.length > 40 ? description.substring(0, 40) + '...' : description}
                      </>
                    )}
                  </div>
                </div>
                <div className="flex gap-2">
                  <Button 
                    onClick={handleCancelSubmit}
                    variant="outline"
                    className="flex-1 py-2 text-sm"
                  >
                    Cancel
                  </Button>
                  <Button 
                    onClick={handleConfirmSubmit}
                    className="flex-1 py-2 text-sm font-semibold flex items-center justify-center gap-2"
                  >
                    <Send className="h-3.5 w-3.5" />
                    Confirm & Send
                  </Button>
                </div>
              </div>
            )}

          {/* Resident Submitted Requests */}
            <div className="mt-6">
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center gap-2">
                  <h2 className="text-lg font-semibold">Your Submitted Requests</h2>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setRefreshKey((k) => k + 1)}
                  className="gap-1"
                  title="Refresh"
                >
                  Refresh
                </Button>
              </div>

              <Card className="p-3 shadow-lg border border-border">
                {loadingRequests ? (
                  <div className="text-sm text-muted-foreground p-3">Loading your requests…</div>
                ) : errorRequests ? (
                  <div className="text-sm text-rose-600 p-3">Error: {errorRequests}</div>
                ) : grouped.length === 0 ? (
                  <div className="text-sm text-muted-foreground p-3">
                    No requests yet. Your submissions will appear here.
                  </div>
                ) : (
                  <ul className="space-y-2">
                    {grouped.map((g) => {
                      const start = g.dates[0];
                      const end = g.dates[g.dates.length - 1];
                      const days = g.dates.length;
                      return (
                        <li
                          key={g.groupId}
                          className="border border-border rounded-lg p-3 hover:bg-muted/40 transition-colors"
                        >
                          <div className="flex items-start justify-between gap-3">
                            <div className="space-y-0.5">
                              <div className="flex items-center gap-2">
                                <Calendar className="h-4 w-4 text-primary" />
                                <span className="font-medium">
                                  {fmt(start)}
                                  {end !== start ? ` — ${fmt(end)}` : ""}
                                </span>
                                <span className="text-xs text-muted-foreground">
                                  • {days} day{days !== 1 ? "s" : ""}
                                </span>
                              </div>
                              <div className="text-sm">
                                <span className="text-muted-foreground">Reason: </span>
                                <span className="font-medium">{g.reason}</span>
                              </div>
                              {g.details ? (
                                <div className="text-xs text-muted-foreground">
                                  {g.details.length > 100
                                    ? g.details.slice(0, 100) + "…"
                                    : g.details}
                                </div>
                              ) : null}
                            </div>
                            <StatusPill status={g.status} />
                          </div>
                        </li>
                      );
                    })}
                  </ul>
                )}
              </Card>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}

export default RequestOffPage; 