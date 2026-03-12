"use client";

import React, { useEffect, useState } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { config } from '../../../config';
import { Calendar, Users, Clock, FileText, Repeat, Send, ArrowRightLeft, AlertTriangle } from "lucide-react";

interface SwapCallsPageProps {
  userId: string;
  yourShiftDate: string;
  setYourShiftDate: (value: string) => void;
  partnerShiftDate: string;
  setPartnerShiftDate: (value: string) => void;
  selectedResident: string;
  setSelectedResident: (value: string) => void;
  residents: { id: string; name: string }[];
  selectedShift: string;
  setSelectedShift: (value: string) => void;
  partnerShift: string;
  setPartnerShift: (value: string) => void;
  shifts: { id: string; name: string }[];
  handleSubmitSwap: () => void;
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

function mapShiftType(shift: string) {
  if (shift === "Saturday") return ["Saturday (24h)", "Saturday (12h)"];
  if (shift === "Sunday") return ["Sunday (12h)"];
  return [shift]; // "Short" stays "Short"
}

const SwapCallsPage: React.FC<SwapCallsPageProps> = ({
  userId,
  yourShiftDate,
  setYourShiftDate,
  partnerShiftDate,
  setPartnerShiftDate,
  selectedResident,
  setSelectedResident,
  residents,
  selectedShift,
  setSelectedShift,
  partnerShift,
  setPartnerShift,
  shifts,
  description,
  setDescription,
  handleSubmitSwap,
}) => {
  const [showConfirmation, setShowConfirmation] = useState(false);
  const isFormValid = yourShiftDate && partnerShiftDate && selectedResident && selectedShift && partnerShift;

  const [loadingSwaps, setLoadingSwaps] = useState(false);
  const [swapRequests, setSwapRequests] = useState<ApiSwaps[]>([]);
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
        const res = await fetch(url, { cache: "no-store" });

        if (res.status === 404) {
          if (!abort) setSwapRequests([]);
          return;
        }

        if (!res.ok) {
          const txt = await res.text();
          throw new Error(txt || `HTTP ${res.status}`);
        }

        const data: ApiSwaps[] = await res.json();
        if (!abort) {
          const arr = Array.isArray(data) ? data : [data];
          arr.sort(
            (a, b) =>
              +new Date(b.createdAt || b.updatedAt) - +new Date(a.createdAt || a.updatedAt)
          );
          setSwapRequests(arr);
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

  const handleInitialSubmit = () => {
    if (isFormValid) {
      setShowConfirmation(true);
    }
  };

  const handleConfirmSubmit = () => {
    handleSubmitSwap();
    setShowConfirmation(false);
    setRefreshKey((k) => k + 1);
  };

  const handleCancelSubmit = () => {
    setShowConfirmation(false);
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

        {/* Main Form Card */}
        <Card className="p-5 shadow-lg border border-border flex-1 flex flex-col">
          <div className="space-y-5 flex-1">
            {/* Your Shift Date Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-primary" />
                <label htmlFor="your-shift-date" className="text-sm font-semibold text-foreground">
                  Your Shift Date
                </label>
              </div>
              <input
                id="your-shift-date"
                type="date"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                value={yourShiftDate}
                onChange={(e) => setYourShiftDate(e.target.value)}
                min={new Date().toISOString().split('T')[0]}
              />
            </div>
            {/* Partner&apos;s Shift Date Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-primary" />
                <label htmlFor="partner-shift-date" className="text-sm font-semibold text-foreground">
                  Partner&apos;s Shift Date
                </label>
              </div>
              <input
                id="partner-shift-date"
                type="date"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                value={partnerShiftDate}
                onChange={(e) => setPartnerShiftDate(e.target.value)}
                min={new Date().toISOString().split('T')[0]}
              />
            </div>
            {/* Resident Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Users className="h-4 w-4 text-primary" />
                <label htmlFor="resident-select" className="text-sm font-semibold text-foreground">
                  Swap Partner
                </label>
              </div>
              <select
                id="resident-select"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
                value={selectedResident}
                onChange={(e) => setSelectedResident(e.target.value)}
              >
                <option value="" disabled>Choose a resident to swap with</option>
                {residents.map((resident) => (
                  <option key={resident.id} value={resident.id}>
                    {resident.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Your Shift Type Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Clock className="h-4 w-4 text-primary" />
                <label htmlFor="shift-select" className="text-sm font-semibold text-foreground">
                  Your Shift Type
                </label>
              </div>
              <select
                id="shift-select"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
                value={selectedShift}
                onChange={(e) => setSelectedShift(e.target.value)}
              >
                <option value="" disabled>Select shift type</option>
                {shifts.map((shift) => (
                  <option key={shift.id} value={shift.id}>
                    {shift.name}
                  </option>
                ))}
              </select>
            </div>
            {/* Partner&apos;s Shift Type Selection */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Clock className="h-4 w-4 text-primary" />
                <label htmlFor="partner-shift-select" className="text-sm font-semibold text-foreground">
                  Partner&apos;s Shift Type
                </label>
              </div>
              <select
                id="partner-shift-select"
                className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
                value={partnerShift}
                onChange={(e) => setPartnerShift(e.target.value)}
              >
                <option value="" disabled>Select shift type</option>
                {shifts.map((shift) => (
                  <option key={shift.id} value={shift.id}>
                    {shift.name}
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
                      {yourShiftDate ? (() => {
                        const [year, month, day] = yourShiftDate.split('-');
                        const localDate = new Date(Number(year), Number(month) - 1, Number(day));
                        return localDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
                      })() : ''} - {selectedShift ? mapShiftType(shifts.find(s => s.id === selectedShift)?.name || '').join(' / ') : 'N/A'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Partner&apos;s Shift:</span>
                    <span className="font-medium text-foreground">
                      {partnerShiftDate ? (() => {
                        const [year, month, day] = partnerShiftDate.split('-');
                        const localDate = new Date(Number(year), Number(month) - 1, Number(day));
                        return localDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
                      })() : ''} - {partnerShift ? mapShiftType(shifts.find(s => s.id === partnerShift)?.name || '').join(' / ') : 'N/A'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Partner:</span>
                    <span className="font-medium text-foreground">
                      {residents.find(r => r.id === selectedResident)?.name || 'N/A'}
                    </span>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Submit Button / Confirmation */}
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
                    <h3 className="text-sm font-semibold text-yellow-800 dark:text-yellow-200">
                      Confirm Swap Request
                    </h3>
                  </div>
                  <p className="text-xs text-yellow-700 dark:text-yellow-300 mb-2">
                    Are you sure you want to submit this swap request? This action will notify your selected partner.
                  </p>
                  <div className="text-xs text-yellow-600 dark:text-yellow-400">
                    <strong>Your Shift:</strong> {yourShiftDate ? (() => {
                      const [year, month, day] = yourShiftDate.split('-');
                      const localDate = new Date(Number(year), Number(month) - 1, Number(day));
                      return localDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
                    })() : ''} - {selectedShift ? mapShiftType(shifts.find(s => s.id === selectedShift)?.name || '').join(' / ') : 'N/A'}<br/>
                      <strong>Partner&apos;s Shift:</strong> {partnerShiftDate ? (() => {
                      const [year, month, day] = partnerShiftDate.split('-');
                      const localDate = new Date(Number(year), Number(month) - 1, Number(day));
                      return localDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
                    })() : ''} - {partnerShift ? mapShiftType(shifts.find(s => s.id === partnerShift)?.name || '').join(' / ') : 'N/A'}<br/>
                    <strong>Partner:</strong> {residents.find(r => r.id === selectedResident)?.name}
                  </div>
                </div>
                <div className="flex gap-2">
                  <Button 
                    onClick={handleCancelSubmit}
                    variant="outline"
                    className="flex-1 py-2.5 text-sm"
                  >
                    Cancel
                  </Button>
                  <Button 
                    onClick={handleConfirmSubmit}
                    className="flex-1 py-2.5 text-sm font-semibold flex items-center justify-center gap-2"
                  >
                    <Send className="h-4 w-4" />
                    Confirm & Send
                  </Button>
                </div>
              </div>
            )}
          </div>

          {/* Resident Submitted Swap Requests */}
          <div className="mt-2">
            <div className="flex items-center mb-2">
              <h2 className="text-lg font-semibold">Your Submitted Swap Requests</h2>
            </div>

            <Card className="p-3 shadow-lg border border-border">
              {loadingSwaps ? (
                <div className="text-sm text-muted-foreground p-3">Loading your swap requests...</div>
              ) : errorSwaps ? (
                <div className="text-sm text-rose-600 p-3">Error: {errorSwaps}</div>
              ) : swapRequests.length === 0 ? (
                <div className="text-sm text-muted-foreground p-3">
                  No swap requests yet. Your submissions will appear here.
                </div>
              ) : (
                <ul className="space-y-2">
                  {swapRequests.map((swap) => (
                    <li
                      key={swap.swapRequestId}
                      className="border border-border rounded-lg p-3 hover:bg-muted/40 transition-colors"
                    >
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
                    </li>
                  ))}
                </ul>
              )}
            </Card>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default SwapCallsPage; 