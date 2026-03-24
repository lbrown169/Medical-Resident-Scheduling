"use client";
import React from "react";
import RotationForm from "./RotationForm";
import { ClipboardList } from "lucide-react";

interface PGY3RotationFormPageProps {
  userId: string;
  userPGY: number;
}

const PGY3RotationFormPage: React.FC<PGY3RotationFormPageProps> = ({ userId, userPGY }) => {

 // Deadline configuration, just may for now (this is configurable later)
 const deadline = new Date("2026-05-15T23:59:00-05:00"); // ! later have this be adjustable by admin
 const isDeadlinePassed = new Date() > deadline;

 // Check if user is not PGY-3
 if (userPGY !== 3) {
   return (
     <div className="max-w-4xl mx-auto">
       <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
         <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
         <h2 className="text-2xl font-bold text-foreground mb-2">Access Denied</h2>
         <p className="text-muted-foreground">
           This page is only accessible to PGY-3 residents.
         </p>
       </div>
     </div>
   );
 }

 // Check if deadline has passed
 if (isDeadlinePassed) {
   return (
     <div className="max-w-4xl mx-auto">
       <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
         <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
         <h2 className="text-2xl font-bold text-foreground mb-2">Submission Deadline Passed</h2>
         <p className="text-muted-foreground">
           The deadline for submitting PGY-4 rotation preferences was {deadline.toLocaleDateString('en-US', { day: 'numeric', month: 'long', year: 'numeric' })} at {deadline.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true, timeZoneName: 'short' })}.
           The form is now closed.
         </p>
       </div>
     </div>
   );
 }

 return (
  <div className="max-w-4xl mx-auto">
    <div className="flex items-center gap-3 mb-6">
      <ClipboardList className="h-8 w-8 text-primary" />
      <h1 className="text-3xl font-bold text-foreground">Rotation Request Form</h1>
    </div>

    <div className="mb-6">
      <p className="text-muted-foreground mb-2">
        Submit preferred shifts for 4th year residency. Please submit by deadline, failure to do so may result in random rotations.
      </p>
      <p className="text-muted-foreground mb-4">
        You may return at anytime prior to the deadline to make changes.
      </p>
      <p className="text-red-600 dark:text-red-400 font-semibold">
        Due: {deadline.toLocaleDateString('en-US', { day: 'numeric', month: 'long', year: 'numeric' })} at {deadline.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true, timeZoneName: 'short' })}
      </p>
    </div>
    <RotationForm
     userId={userId}
     userPGY={userPGY}
     requiredPGY={3}
     rotationPgyYear={4}
     deadline={deadline}
     submitEndpoint="api/rotation-pref-request"
     fetchEndpoint="api/rotation-pref-request/resident"
    />
  </div>
 );
};
export default PGY3RotationFormPage;
