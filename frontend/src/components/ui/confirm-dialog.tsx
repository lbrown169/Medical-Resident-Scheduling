"use client";
import React from "react";
import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";

interface ConfirmDialogProps {
  triggerText?: React.ReactNode;
  confirmText?: React.ReactNode;
  cancelText?: React.ReactNode;
  title?: React.ReactNode;
  message?: React.ReactNode;
  onConfirm: () => Promise<void> | void; 
  loading?: boolean;
  variant?: "default" | "danger" | "outline";
  className?: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  triggerText,
  confirmText = "Yes",
  cancelText = "No",
  title = "Are you sure?",
  message = "This action cannot be undone.",
  onConfirm,
  loading = false,
  variant = "default",
  className,
  open,
  onOpenChange,
}) => {
  const [internalOpen, setInternalOpen] = React.useState(false);
  const controlled = open !== undefined;

  const dialogOpen = controlled ? (open as boolean) : internalOpen;
  const setDialogOpen = controlled ? (onOpenChange as (o: boolean) => void) : setInternalOpen;

  const baseBtnClass =
    "px-1 sm:px-6 py-1 sm:py-3 font-semibold rounded-xl shadow transition " +
    "whitespace-nowrap w-full sm:w-auto text-xs sm:text-sm lg:text-base flex items-center justify-center";

  const colorClass =
    variant === "danger"
      ? "bg-red-600 text-white hover:bg-red-700"
      : variant === "outline"
      ? "border text-gray-900 hover:bg-gray-100"
      : "bg-blue-600 text-white hover:bg-blue-700";

  return (
    <AlertDialog open={dialogOpen} onOpenChange={setDialogOpen}>
      {/* Render the trigger only if provided */}
      {triggerText && (
        <AlertDialogTrigger asChild>
          <Button
            className={`${baseBtnClass} ${colorClass} ${className ?? ""}`}
            onClick={() => setDialogOpen(true)}
            disabled={loading}
          >
            {triggerText}
          </Button>
        </AlertDialogTrigger>
      )}

      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>
          <AlertDialogDescription>{message}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={loading}>{cancelText}</AlertDialogCancel>
          <AlertDialogAction
            onClick={async () => {
              await onConfirm();
              setDialogOpen(false);
            }}
            disabled={loading}
          >
            {loading ? "Processing…" : confirmText}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};
