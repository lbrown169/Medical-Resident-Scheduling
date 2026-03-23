"use client";

import React, { useState } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { Calendar, Users, Repeat, Send, ArrowRightLeft, AlertTriangle } from "lucide-react";
import { CalendarEvent } from "@/lib/models/CalendarEvent";

interface SwapCallsPageProps {
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
  handleSubmitSwap: () => void;
}

function formatDate(dateStr: string, opts: Intl.DateTimeFormatOptions) {
  if (!dateStr) return '';
  const [year, month, day] = dateStr.split('-');
  return new Date(Number(year), Number(month) - 1, Number(day)).toLocaleDateString('en-US', opts);
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
  handleSubmitSwap,
}) => {
  const [showConfirmation, setShowConfirmation] = useState(false);
  const isFormValid = yourShiftDate && partnerShiftDate && selectedResident && selectedShift && partnerShift;

  const userShiftValue = yourShiftDate && selectedShift ? `${yourShiftDate}|${selectedShift}` : "";
  const partnerShiftValue = partnerShiftDate && partnerShift ? `${partnerShiftDate}|${partnerShift}` : "";

  const handleSelect = (value: string, setter: (date: string, callType: string) => void) => {
    const idx = value.indexOf('|');
    if (idx === -1) return;
    setter(value.slice(0, idx), value.slice(idx + 1));
  };

  const handleInitialSubmit = () => {
    if (isFormValid) setShowConfirmation(true);
  };

  const handleConfirmSubmit = () => {
    handleSubmitSwap();
    setShowConfirmation(false);
  };

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
                onChange={(e) => setSelectedResident(e.target.value)}
              >
                <option value="" disabled>Choose a resident to swap with</option>
                {residents.map((resident) => (
                  <option key={resident.id} value={resident.id}>{resident.name}</option>
                ))}
              </select>
            </div>

            {/* Partner's Shift */}
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
                disabled={!selectedResident || partnerShiftEvents.length === 0}
              >
                <option value="" disabled>
                  {!selectedResident
                    ? 'Select a partner first'
                    : partnerShiftEvents.length === 0
                    ? 'No upcoming shifts found'
                    : "Select partner's shift"}
                </option>
                {partnerShiftEvents.map((e, i) => {
                  const opt = eventToOption(e);
                  return <option key={i} value={opt.value}>{opt.label}</option>;
                })}
              </select>
            </div>

            {/* Summary */}
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
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Partner&apos;s Shift:</span>
                    <span className="font-medium text-foreground">
                      {formatDate(partnerShiftDate, { month: 'short', day: 'numeric' })} — {partnerShift}
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
                  <p className="text-xs text-muted-foreground mt-2 text-center">Complete all fields to submit</p>
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
                    <strong>Partner:</strong> {residents.find(r => r.id === selectedResident)?.name}
                  </div>
                </div>
                <div className="flex gap-2">
                  <Button onClick={() => setShowConfirmation(false)} variant="outline" className="flex-1 py-2.5 text-sm">
                    Cancel
                  </Button>
                  <Button onClick={handleConfirmSubmit} className="flex-1 py-2.5 text-sm font-semibold flex items-center justify-center gap-2">
                    <Send className="h-4 w-4" />
                    Confirm & Send
                  </Button>
                </div>
              </div>
            )}
          </div>
        </Card>
      </div>
    </div>
  );
};

export default SwapCallsPage;
