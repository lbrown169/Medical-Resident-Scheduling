"use client";
/*Example:

import { ConfirmDialog } from "@/app/dashboard/components/ConfirmDialog";

    <ConfirmDialog
      triggerText={
        <span className="flex items-center">
          <Trash2 className="h-4 w-4 mr-2" />
          Delete Current Schedule
        </span>
      }

      title="Delete current schedule?"
      message="This action cannot be undone."
      confirmText="Delete"
      cancelText="Cancel"
      onConfirm={handleDeleteSchedule}
      loading={deletingSchedule}
      variant="danger" // "default" = blue, "danger" = red, "outline" = gray
    />
*/
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
  triggerText: React.ReactNode;
  confirmText?: React.ReactNode;
  cancelText?: React.ReactNode;
  title?: React.ReactNode;
  message?: React.ReactNode;
  onConfirm: () => Promise<void> | void;
  loading?: boolean;
  variant?: "default" | "danger" | "outline";
  className?: string; // optional external override
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
}) => {
  const [open, setOpen] = React.useState(false);

  // match the Generate button sizing/shape exactly
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
    <AlertDialog open={open} onOpenChange={setOpen}>
      <AlertDialogTrigger asChild>
        <Button
          className={`${baseBtnClass} ${colorClass} ${className ?? ""}`}
          onClick={() => setOpen(true)}
          disabled={loading}
        >
          {triggerText}
        </Button>
      </AlertDialogTrigger>

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
              setOpen(false);
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
