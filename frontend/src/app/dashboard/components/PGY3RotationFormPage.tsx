"use client";
import React, { useEffect, useState } from "react";
import RotationForm from "./RotationForm";
import { ClipboardList } from "lucide-react";
import { config } from "../../../config";

interface PGY3RotationFormPageProps {
  userId: string;
  userPGY: number;
}

interface SubmissionWindow {
  academicYear: number;
  availableDate: string | null;
  dueDate: string | null;
}

const isInvalidDate = (d: Date) => Number.isNaN(d.getTime());

const PGY3RotationFormPage: React.FC<PGY3RotationFormPageProps> = ({
  userId,
  userPGY,
}) => {
  const [submissionWindow, setSubmissionWindow] =
    useState<SubmissionWindow | null>(null);
  const [loading, setLoading] = useState(true);
  const [fetchError, setFetchError] = useState(false);

  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch(
          `${config.apiUrl}/api/rotation-request-submission-window`,
        );
        if (!res.ok) {
          setFetchError(true);
          return;
        }
        const data: SubmissionWindow = await res.json();
        setSubmissionWindow(data);
      } catch {
        setFetchError(true);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  // Check if user is not PGY-3
  if (userPGY !== 3) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Access Denied
          </h2>
          <p className="text-muted-foreground">
            This page is only accessible to PGY-3 residents.
          </p>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </div>
    );
  }

  if (fetchError || !submissionWindow) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Form Unavailable
          </h2>
          <p className="text-muted-foreground">
            The rotation preference form is not available yet.
          </p>
        </div>
      </div>
    );
  }

  const now = new Date();
  const availableDate = new Date(submissionWindow.availableDate + "Z");
  const dueDate = new Date(submissionWindow.dueDate + "Z");

  if (isInvalidDate(availableDate) || isInvalidDate(dueDate)) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Form Unavailable
          </h2>
          <p className="text-muted-foreground">
            The rotation preference form is not available yet.
          </p>
        </div>
      </div>
    );
  }

  const formatDate = (d: Date) =>
    d.toLocaleDateString("en-US", {
      day: "numeric",
      month: "long",
      year: "numeric",
    });

  if (now < availableDate) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Not Yet Open
          </h2>
          <p className="text-muted-foreground">
            The rotation preference form opens on {formatDate(availableDate)}.
          </p>
        </div>
      </div>
    );
  }

  // Check if deadline has passed
  if (now >= dueDate) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Submission Deadline Passed
          </h2>
          <p className="text-muted-foreground">
            The deadline for submitting PGY-4 rotation preferences was{" "}
            {formatDate(dueDate)}. The form is now closed.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <div className="flex items-center gap-3 mb-6">
        <ClipboardList className="h-8 w-8 text-primary" />
        <h1 className="text-3xl font-bold text-foreground">
          Rotation Request Form
        </h1>
      </div>

      <div className="mb-6">
        <p className="text-muted-foreground mb-2">
          Submit preferred shifts for 4th year residency. Please submit by
          deadline, failure to do so may result in random rotations.
        </p>
        <p className="text-muted-foreground mb-4">
          You may return at anytime prior to the deadline to make changes.
        </p>
        <p className="text-red-600 dark:text-red-400 font-semibold">
          Due:{" "}
          {dueDate.toLocaleString("en-US", {
            month: "long",
            day: "numeric",
            year: "numeric",
            hour: "numeric",
            minute: "2-digit",
            hour12: true,
          })}
        </p>
      </div>
      <RotationForm
        userId={userId}
        userPGY={userPGY}
        requiredPGY={3}
        rotationPgyYear={4}
        deadline={dueDate}
        submitEndpoint="api/rotation-pref-request"
        fetchEndpoint="api/rotation-pref-request/resident"
      />
    </div>
  );
};
export default PGY3RotationFormPage;
