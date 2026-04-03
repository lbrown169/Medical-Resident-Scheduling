"use client";

import React from "react";
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogFooter,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { AlertTriangle, X } from "lucide-react";

interface ClearConfirmDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: () => void;
  isLoading?: boolean;
}

export const ClearConfirmDialog: React.FC<ClearConfirmDialogProps> = ({
  open,
  onOpenChange,
  onConfirm,
  isLoading = false,
}) => {
  const handleConfirm = () => {
    onConfirm();
    onOpenChange(false);
  };

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-[400px] bg-white dark:bg-neutral-900 p-0 overflow-hidden">
        {/* Close button */}
        <button
          onClick={() => onOpenChange(false)}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <X className="w-5 h-5" />
        </button>

        {/* Header with Warning Icon */}
        <AlertDialogHeader className="px-6 pt-6 pb-4">
          <AlertDialogTitle className="flex items-center gap-2 text-lg font-semibold">
            <div className="w-8 h-8 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center">
              <AlertTriangle className="w-5 h-5 text-red-500" />
            </div>
            <span>Warning</span>
          </AlertDialogTitle>
        </AlertDialogHeader>

        {/* Content */}
        <div className="px-6 pb-4 text-center">
          <p className="text-sm text-gray-700 dark:text-gray-300">
            You are about to clear all submissions.
          </p>
          <p className="text-sm text-gray-700 dark:text-gray-300">
            This cannot be undone.
          </p>
          <p className="text-sm text-gray-700 dark:text-gray-300 mt-4">
            Do you still want to clear all?
          </p>
        </div>

        {/* Footer */}
        <AlertDialogFooter className="px-6 pb-6 flex justify-center">
          <Button
            onClick={handleConfirm}
            disabled={isLoading}
            className="bg-red-500 hover:bg-red-600 text-white px-6 py-2 rounded-md flex items-center gap-2"
          >
            <X className="w-4 h-4" />
            {isLoading ? "Clearing..." : "Clear"}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};

export default ClearConfirmDialog;
