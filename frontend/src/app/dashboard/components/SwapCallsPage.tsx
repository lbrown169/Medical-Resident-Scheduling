"use client";

import React, { useEffect, useState } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { config } from '../../../config';
import { toast } from "../../../lib/use-toast";
import { Calendar, Users, FileText, Repeat, Send, ArrowRightLeft, AlertTriangle, CornerDownRight } from "lucide-react";
import { CalendarEvent } from "@/lib/models/CalendarEvent";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { SwapValidationResponse } from "../../../lib/models/SwapValidationResponse";

interface SwapCallsPageProps {
  userId: string;
  yourShiftDate: string;
  partnerShiftDate: string;
  selectedResident: string;
  setSelectedResident: (value: string) => void;
  residents: { id: string; name: string }[];
  selectedShift: string;
  partnerShift: string;
  userShiftEvents: CalendarEvent[];
  partnerShiftEvents: CalendarEvent[];
  onSelectUserShift: (date: string, callType: string) => void;
  onSelectPartnerShift: (date: string, callType: string) => void;
  handleSubmitSwap: () => Promise<SwapValidationResponse | null>;
  description: string;
  setDescription: (value: string) => void;
}

type SwapRequestStatus = {
  id: number;
  description: string;
};

type ApiSwaps = {
  swapRequestId: string;
  scheduleId: string;
  requesterId: string;
  requesteeId: string;
  requesterDate: string;
  requesteeDate: string;
  status: SwapRequestStatus;
  createdAt: string;
  updatedAt: string;
  details?: string | null;
};

function formatDate(dateStr: string, opts: Intl.DateTimeFormatOptions) {
  if (!dateStr) return '';
  const [year, month, day] = dateStr.split('-');
  return new Date(Number(year), Number(month) - 1, Number(day)).toLocaleDateString('en-US', opts);
}

function academicYearOf(date: Date): number {
  return date.getMonth() >= 6 ? date.getFullYear() : date.getFullYear() - 1;
}

function eventToOption(e: CalendarEvent): { value: string; label: string } {
  const dateStr = e.start instanceof Date
    ? e.start.toISOString().split('T')[0]
    : String(e.start).split('T')[0];
  const callType = e.extendedProps?.callType ?? '';
  const hours = e.extendedProps?.callTypeId === 99 ? ` (${e.extendedProps?.hours}h)` : '';
  const fullCallType = callType + hours;
  const label = formatDate(dateStr, { weekday: 'short', month: 'short', day: 'numeric' });
  return { value: `${dateStr}|${fullCallType}`, label: `${label} — ${fullCallType}` };
}

const SwapCallsPage: React.FC<SwapCallsPageProps> = ({
  userId,
  yourShiftDate,
  partnerShiftDate,
  selectedResident,
  setSelectedResident,
  residents,
  selectedShift,
  partnerShift,
  userShiftEvents,
  partnerShiftEvents,
  onSelectUserShift,
  onSelectPartnerShift,
  description,
  setDescription,
  handleSubmitSwap,
}) => {
  const [showConfirmation, setShowConfirmation] = useState(false);
  const [swapViolations, setSwapViolations] = useState<SwapValidationResponse | null>(null);
  const isFormValid = yourShiftDate && partnerShiftDate && selectedResident && selectedShift && partnerShift;

  const [loadingSwaps, setLoadingSwaps] = useState(false);
  const [swapRequests, setSwapRequests] = useState<ApiSwaps[]>([]);
  const [pendingSwapRequests, setPendingSwapRequests] = useState<ApiSwaps[]>([]);
  const [errorSwaps, setErrorSwaps] = useState<string | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  useEffect(() => {
    if (!userId) {
      setSwapRequests([]);
      setErrorSwaps("Missing userId.");
      return;
    }

    let abort = false;

    (async () => {
      setLoadingSwaps(true);
      setErrorSwaps(null);

      try {
        const url = `${config.apiUrl}/api/swaprequests?requester_id=${encodeURIComponent(userId)}`;
        const bUrl = `${config.apiUrl}/api/swaprequests?requestee_id=${encodeURIComponent(userId)}`;
        const res = await fetch(url, { cache: "no-store" });
        const bRes = await fetch(bUrl, { cache: "no-store" });

        if (res.status === 404 || bRes.status === 404) {
          if (!abort) setSwapRequests([]);
          return;
        }

        if (!res.ok) {
          const txt = await res.text();
          throw new Error(txt || `HTTP ${res.status}`);
        }

        if (!bRes.ok) {
          const txt = await bRes.text();
          throw new Error(txt || `HTTP ${bRes.status}`);
        }

        const data: ApiSwaps[] = await res.json();
        const bData: ApiSwaps[] = await bRes.json();
        const combinedData: ApiSwaps[] = [...data, ...bData];

        if (!abort) {
          const arr = Array.isArray(combinedData) ? combinedData : [combinedData];
          arr.sort(
            (a, b) =>
              +new Date(b.updatedAt || b.createdAt) - +new Date(a.updatedAt || a.createdAt)
          );
          const filteredArr = arr.filter((swap) => (swap.requesterId === userId || (swap.requesteeId === userId && swap.status.description !== "Denied")));
          setSwapRequests(filteredArr);

          const pending = arr.filter((swap) => swap.status.description === "Pending");
          setPendingSwapRequests(pending);

        }
      } catch (e: unknown) {
        const msg = e instanceof Error ? e.message : "Failed to load swap requests.";
        if (!abort) setErrorSwaps(msg);
      } finally {
        if (!abort) setLoadingSwaps(false);
      }
    })();

    return () => {
      abort = true;
    };
  }, [userId, refreshKey]);

  const userShiftValue = yourShiftDate && selectedShift ? `${yourShiftDate}|${selectedShift}` : "";
  const partnerShiftValue = partnerShiftDate && partnerShift ? `${partnerShiftDate}|${partnerShift}` : "";

  const handleSelect = (value: string, setter: (date: string, callType: string) => void) => {
    const idx = value.indexOf('|');
    if (idx === -1) return;
    setSwapViolations(null);
    setter(value.slice(0, idx), value.slice(idx + 1));
  };

  const handleInitialSubmit = () => {
    setSwapViolations(null);
    if (isFormValid) setShowConfirmation(true);
  };

  const handleConfirmSubmit = async () => {
    const result = await handleSubmitSwap();
    if (result) {
      // Violations or error returned — display them
      setSwapViolations(result);
    } else {
      // Success — close confirmation and refresh
      setSwapViolations(null);
      setShowConfirmation(false);
      setRefreshKey((k) => k + 1);
    }
  };

  const [denyModalOpen, setDenyModalOpen] = useState(false);
  const [pendingDenyId, setPendingDenyId] = useState<string | null>(null);
  const [actionLoading, setActionLoading] = useState(false);

  const handleApprove = async (swapId: string) => {
    setActionLoading(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/swaprequests/${swapId}/approve`, { method: "POST" });
      if (response.ok) {
        toast({
          title: "Swap Approved",
          description: "The swap request has been approved successfully.",
          variant: "success"
        });
      }
    } catch (error) {
      console.error('Error approving swap:', error);
      toast({
        title: "Error",
        description: "Failed to approve swap request.",
        variant: "destructive"
      });
    } finally {
      setActionLoading(false);
      setRefreshKey((k) => k + 1);
    }
  };

  const handleDeny = (swapId: string) => {
    setPendingDenyId(swapId);
    setDenyModalOpen(true);
  };

  const submitDeny = async () => {
    if (!pendingDenyId) return;
    setActionLoading(true);
    try {
      await fetch(`${config.apiUrl}/api/swaprequests/${pendingDenyId}/deny`, { method: "POST" });
      setDenyModalOpen(false);
      setPendingDenyId(null);
    } finally {
      setActionLoading(false);
      setRefreshKey((k) => k + 1);
    }
  };

  const StatusPill: React.FC<{ status: string }> = ({ status }) => {
    const base = "px-2 py-0.5 text-xs font-semibold rounded-full border";

    if (status === "Approved") {
      return (
        <span
          className={`${base} bg-emerald-50 dark:bg-emerald-900/20 border-emerald-200 dark:border-emerald-800 text-emerald-700 dark:text-emerald-300`}
        >
          Approved
        </span>
      );
    }

    if (status === "Denied") {
      return (
        <span
          className={`${base} bg-rose-50 dark:bg-rose-900/20 border-rose-200 dark:border-rose-800 text-rose-700 dark:text-rose-300`}
        >
          Denied
        </span>
      );
    }

    return (
      <span
        className={`${base} bg-amber-50 dark:bg-amber-900/20 border-amber-200 dark:border-amber-800 text-amber-700 dark:text-amber-300`}
      >
        Pending
      </span>
    );
  };

  const fmt = (d: string) => {
    const [year, month, day] = d.split("-");
    const localDate = new Date(Number(year), Number(month) - 1, Number(day));
    return localDate.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      year: "numeric",
    });
  };

  const getResidentName = (residentId: string) =>
    residents.find((r) => r.id === residentId)?.name || residentId;

  return (
    <div className="w-full h-full bg-background p-4 overflow-hidden">
      <div className="max-w-xl mx-auto h-full flex flex-col">
        {/* Header */}
        <div className="text-center mb-5">
          <div className="flex justify-center mb-2">
            <div className="p-2.5 bg-primary/10 rounded-full">
              <Repeat className="h-6 w-6 text-primary" />
            </div>
          </div>
          <h1 className="text-2xl font-bold text-foreground mb-1">Request Call Swap</h1>
          <p className="text-sm text-muted-foreground">
            Exchange shifts with a colleague by completing the form below.
          </p>
        </div>

        <Card className="p-5 shadow-lg border border-border flex-1 flex flex-col">
          <div className="space-y-5 flex-1">
            {/* Your Shift */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-primary" />
                <label htmlFor="your-shift" className="text-sm font-semibold text-foreground">Your Shift</label>
              </div>
              <select
                id="your-shift"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer disabled:opacity-50"
                value={userShiftValue}
                onChange={(e) => handleSelect(e.target.value, onSelectUserShift)}
                disabled={userShiftEvents.length === 0}
              >
                <option value="" disabled>
                  {userShiftEvents.length === 0 ? 'No upcoming shifts found' : 'Select your shift'}
                </option>
                {userShiftEvents.map((e, i) => {
                  const opt = eventToOption(e);
                  return <option key={i} value={opt.value}>{opt.label}</option>;
                })}
              </select>
            </div>

            {/* Swap Partner */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Users className="h-4 w-4 text-primary" />
                <label htmlFor="resident-select" className="text-sm font-semibold text-foreground">Swap Partner</label>
              </div>
              <select
                id="resident-select"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
                value={selectedResident}
                onChange={(e) => { setSwapViolations(null); setSelectedResident(e.target.value); }}
              >
                <option value="" disabled>Choose a resident to swap with</option>
                {residents.map((resident) => (
                  <option key={resident.id} value={resident.id}>{resident.name}</option>
                ))}
              </select>
            </div>

            {/* Partner's Shift */}
            {(() => {
              const userShiftAcademicYear = yourShiftDate
                ? academicYearOf(new Date(yourShiftDate))
                : null;
              const filteredPartnerEvents = userShiftAcademicYear !== null
                ? partnerShiftEvents.filter(e => academicYearOf(e.start instanceof Date ? e.start : new Date(e.start)) === userShiftAcademicYear)
                : partnerShiftEvents;
              return (
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 text-primary" />
                    <label htmlFor="partner-shift" className="text-sm font-semibold text-foreground">Partner&apos;s Shift</label>
                  </div>
                  <select
                    id="partner-shift"
                    className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer disabled:opacity-50"
                    value={partnerShiftValue}
                    onChange={(e) => handleSelect(e.target.value, onSelectPartnerShift)}
                    disabled={!selectedResident || filteredPartnerEvents.length === 0}
                  >
                    <option value="" disabled>
                      {!selectedResident
                        ? 'Select a partner first'
                        : filteredPartnerEvents.length === 0
                        ? 'No upcoming shifts found'
                        : "Select partner's shift"}
                    </option>
                    {filteredPartnerEvents.map((e, i) => {
                      const opt = eventToOption(e);
                      return <option key={i} value={opt.value}>{opt.label}</option>;
                    })}
                  </select>
                </div>
              );
            })()}
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
                rows={4}
                maxLength={255}
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors resize-none"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="e.g., specific duties, contact info, special considerations..."
              />
              <div className="flex justify-end">
                <span className={`text-xs ${description.length >= 255 ? 'text-red-500 font-semibold' : 'text-muted-foreground'}`}>
                  {description.length}/255
                </span>
              </div>
            </div>
            {/* Form Summary */}
            {isFormValid && (
              <div className="bg-muted/50 rounded-lg p-3 border border-border">
                <h3 className="font-semibold text-foreground mb-2 flex items-center gap-2 text-sm">
                  <ArrowRightLeft className="h-3 w-3" />
                  Swap Summary
                </h3>
                <div className="space-y-1 text-xs">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Your Shift:</span>
                    <span className="font-medium text-foreground">
                      {formatDate(yourShiftDate, { month: 'short', day: 'numeric' })} — {selectedShift}
                      {(() => { const pgy = userShiftEvents.find(e => eventToOption(e).value === userShiftValue)?.extendedProps?.pgyLevel; return pgy != null ? <span className="text-muted-foreground font-normal"> · PGY{pgy}</span> : null; })()}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Partner&apos;s Shift:</span>
                    <span className="font-medium text-foreground">
                      {formatDate(partnerShiftDate, { month: 'short', day: 'numeric' })} — {partnerShift}
                      {(() => { const pgy = partnerShiftEvents.find(e => eventToOption(e).value === partnerShiftValue)?.extendedProps?.pgyLevel; return pgy != null ? <span className="text-muted-foreground font-normal"> · PGY{pgy}</span> : null; })()}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Partner:</span>
                    <span className="font-medium text-foreground">
                      {residents.find(r => r.id === selectedResident)?.name || 'N/A'}
                    </span>
                  </div>
                  {description.trim() && (
                    <div className="flex justify-between">
                      <span className="text-muted-foreground">Additional Details:</span>
                      <span className="font-medium text-foreground">
                        {description}
                      </span>
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>

          {/* Submit / Confirmation */}
          <div className="pt-4 mt-auto">
            {!showConfirmation ? (
              <>
                <Button
                  onClick={handleInitialSubmit}
                  className="w-full py-2.5 text-sm font-semibold flex items-center justify-center gap-2 transition-all duration-200"
                  disabled={!isFormValid}
                >
                  <Send className="h-4 w-4" />
                  <span>Submit Swap Request</span>
                </Button>
                {!isFormValid && (
                  <p className="text-xs text-muted-foreground mt-2 text-center">
                    Complete all required fields to submit
                  </p>
                )}
              </>
            ) : (
              <div className="space-y-3">
                <div className="bg-yellow-50 dark:bg-yellow-950/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-3">
                  <div className="flex items-center gap-2 mb-2">
                    <AlertTriangle className="h-4 w-4 text-yellow-600 dark:text-yellow-400" />
                    <h3 className="text-sm font-semibold text-yellow-800 dark:text-yellow-200">Confirm Swap Request</h3>
                  </div>
                  <p className="text-xs text-yellow-700 dark:text-yellow-300 mb-2">
                    Are you sure you want to submit this swap request? This action will notify your selected partner.
                  </p>
                  <div className="text-xs text-yellow-600 dark:text-yellow-400">
                    <strong>Your Shift:</strong> {formatDate(yourShiftDate, { month: 'short', day: 'numeric', year: 'numeric' })} — {selectedShift}<br />
                    <strong>Partner&apos;s Shift:</strong> {formatDate(partnerShiftDate, { month: 'short', day: 'numeric', year: 'numeric' })} — {partnerShift}<br />
                    <strong>Partner:</strong> {residents.find(r => r.id === selectedResident)?.name}<br />
                    {description.trim() && (
                      <>
                        <strong>Additional Details:</strong> {description}
                        <br />
                      </>
                    )}
                  </div>
                </div>
                {/* Swap violation display */}
                {swapViolations && (
                  <div className="rounded border border-red-300 bg-red-50 dark:bg-red-950/30 dark:border-red-800 p-3 space-y-2">
                    {swapViolations.requester?.violations?.violations?.some(v => v.isViolated) || swapViolations.requestee?.violations?.violations?.some(v => v.isViolated) ? (
                      <>
                        <p className="text-sm font-medium text-red-700 dark:text-red-400">
                          This swap cannot be completed due to scheduling conflicts:
                        </p>
                        {swapViolations.requester?.violations?.violations?.filter(v => v.isViolated).map((v, i) => (
                          <div key={`req-${i}`} className="text-sm flex items-start gap-1.5 text-red-700 dark:text-red-400">
                            <span className="mt-0.5 flex-shrink-0">{"\u2715"}</span>
                            <span><strong>You:</strong> {v.message}</span>
                          </div>
                        ))}
                        {swapViolations.requestee?.violations?.violations?.filter(v => v.isViolated).map((v, i) => (
                          <div key={`ree-${i}`} className="text-sm flex items-start gap-1.5 text-red-700 dark:text-red-400">
                            <span className="mt-0.5 flex-shrink-0">{"\u2715"}</span>
                            <span><strong>{residents.find(r => r.id === selectedResident)?.name ?? "Partner"}:</strong> {v.message}</span>
                          </div>
                        ))}
                      </>
                    ) : (
                      <p className="text-sm text-red-700 dark:text-red-400">{swapViolations.message}</p>
                    )}
                  </div>
                )}
                <div className="flex gap-2">
                  <Button onClick={() => { setShowConfirmation(false); setSwapViolations(null); }} variant="outline" className="flex-1 py-2.5 text-sm">
                    Cancel
                  </Button>
                  <Button onClick={handleConfirmSubmit} disabled={!!swapViolations} className="flex-1 py-2.5 text-sm font-semibold flex items-center justify-center gap-2">
                    <Send className="h-4 w-4" />
                    Confirm & Send
                  </Button>
                </div>
              </div>
            )}
          </div>

          {/* Resident Pending Swap Requests */}
          {loadingSwaps ? (<></>) : errorSwaps ? (<></>) : pendingSwapRequests.length === 0 ? (<></>) : (
            <div className="mt-2">
              <div className="flex items-center mb-2">
                <h2 className="text-lg font-semibold">Pending Swap Requests</h2>
              </div>

              <Card className="p-3 shadow-lg border border-border">
                <ul className="space-y-2">
                  {pendingSwapRequests.map((swap) => (
                    <li
                      key={swap.swapRequestId}
                      className="border border-border rounded-lg p-3 hover:bg-muted/40 transition-colors"
                    >
                      {swap.requesterId === userId ? (
                        <div className="flex items-start justify-between gap-3">
                          <div className="space-y-1 min-w-0">
                            <div className="flex items-center gap-2">
                              <ArrowRightLeft className="h-4 w-4 text-primary shrink-0" />
                              <span className="font-medium break-words">
                                {getResidentName(swap.requesteeId)} (Outgoing)
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Your Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesterDate)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Partner&apos;s Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesteeDate)}
                              </span>
                            </div>

                            {swap.details ? (
                              <div className="text-xs text-muted-foreground break-all">
                                {swap.details}
                              </div>
                            ) : null}
                          </div>

                          <StatusPill status={swap.status.description} />
                        </div>
                      ): swap.requesteeId === userId ? (
                        <div className="flex items-stretch justify-between gap-3">
                          <div className="space-y-1 min-w-0">
                            <div className="flex items-center gap-2">
                              <CornerDownRight className="h-4 w-4 text-primary shrink-0" />
                              <span className="font-medium break-words">
                                {getResidentName(swap.requesterId)} (Incoming)
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Your Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesteeDate)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Partner&apos;s Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesterDate)}
                              </span>
                            </div>

                            {swap.details ? (
                              <div className="text-xs text-muted-foreground break-all">
                                {swap.details}
                              </div>
                            ) : null}
                          </div>

                          <div className="flex flex-col items-end min-w-0">
                            <StatusPill status={swap.status.description} />

                            <div className="flex gap-2 mt-auto">
                              <Button size="sm" disabled={actionLoading} onClick={() => handleApprove(swap.swapRequestId)} className="bg-green-600 text-white hover:bg-green-700 cursor-pointer">Approve</Button>
                              <Button size="sm" disabled={actionLoading} onClick={() => handleDeny(swap.swapRequestId)} className="bg-red-600 text-white hover:bg-red-700 cursor-pointer">Deny</Button>
                            </div>
                          </div>
                        </div>
                      ) : (<></>)}
                    </li>
                  ))}
                </ul>
              </Card>
            </div>
          )}

          {/* Resident Submitted Swap Requests */}
          <div className="mt-2">
            <div className="flex items-center mb-2">
              <h2 className="text-lg font-semibold">Swap Request History</h2>
            </div>

            <Card className="p-3 shadow-lg border border-border">
              {loadingSwaps ? (
                <div className="text-sm text-muted-foreground p-3">Loading your swap requests...</div>
              ) : errorSwaps ? (
                <div className="text-sm text-rose-600 p-3">Error: {errorSwaps}</div>
              ) : swapRequests.filter((swap) => swap.status.description !== "Pending").length === 0 ? (
                <div className="text-sm text-muted-foreground p-3">
                  Completed requests and requests you accept from others will appear here.
                </div>
              ) : (
                <ul className="space-y-2">
                  {swapRequests.filter((swap) => swap.status.description !== "Pending").map((swap) => (
                    <li
                      key={swap.swapRequestId}
                      className="border border-border rounded-lg p-3 hover:bg-muted/40 transition-colors"
                    >
                      {swap.requesterId === userId ? (
                        <div className="flex items-start justify-between gap-3">
                          <div className="space-y-1 min-w-0">
                            <div className="flex items-center gap-2">
                              <ArrowRightLeft className="h-4 w-4 text-primary shrink-0" />
                              <span className="font-medium break-words">
                                {getResidentName(swap.requesteeId)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Your Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesterDate)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Partner&apos;s Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesteeDate)}
                              </span>
                            </div>

                            {swap.details ? (
                              <div className="text-xs text-muted-foreground break-all">
                                {swap.details}
                              </div>
                            ) : null}
                          </div>

                          <StatusPill status={swap.status.description} />
                        </div>
                      ) : swap.requesteeId === userId ? (
                        <div className="flex items-start justify-between gap-3">
                          <div className="space-y-1 min-w-0">
                            <div className="flex items-center gap-2">
                              <CornerDownRight className="h-4 w-4 text-primary shrink-0" />
                              <span className="font-medium break-words">
                                {getResidentName(swap.requesterId)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Your Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesteeDate)}
                              </span>
                            </div>

                            <div className="text-sm">
                              <span className="text-muted-foreground">Partner&apos;s Shift: </span>
                              <span className="font-medium break-words">
                                {fmt(swap.requesterDate)}
                              </span>
                            </div>

                            {swap.details ? (
                              <div className="text-xs text-muted-foreground break-all">
                                {swap.details}
                              </div>
                            ) : null}
                          </div>

                          <StatusPill status={swap.status.description} />
                        </div>
                      ) : (<></>)}
                    </li>
                  ))}
                </ul>
              )}
            </Card>
          </div>
        </Card>
      </div>

      <ConfirmDialog
        open={denyModalOpen}
        onOpenChange={setDenyModalOpen}
        title="Deny swap?"
        message={`Are you sure you want to deny this swap request? This action cannot be undone.`}
        confirmText="Deny"
        cancelText="Cancel"
        onConfirm={submitDeny}
        variant="danger"
      />
    </div>
  );
};

export default SwapCallsPage;
