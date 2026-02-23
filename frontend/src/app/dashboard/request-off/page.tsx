"use client";
import React, { useEffect, useState } from 'react';
import RequestOffPage from '../components/RequestOffPage';
import { getUser, User } from '../../../lib/auth';

const leaveReasons: { id: string; name: string; halfDay?: string }[] = [
  { id: "vacation", name: "Vacation" },
  { id: "sick", name: "Sick Leave" },
  { id: "sick-am", name: "Sick AM", halfDay: "A" },
  { id: "sick-pm", name: "Sick PM", halfDay: "P" },
  { id: "cme", name: "ED (Education Days)" },
  { id: "personal", name: "Personal Leave" },
  { id: "other", name: "Other" },
];

export default function Page() {
  const [user, setUser] = useState<User | null>(null);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [reason, setReason] = useState("");
  const [description, setDescription] = useState("");

useEffect(() => {
    setUser(getUser()); 
  }, []);

  const handleSubmitRequestOff = async () => {
    alert("Request submitted!");
  };

  return (
    <RequestOffPage
      userId={user?.id || ""}
      startDate={startDate}
      setStartDate={setStartDate}
      endDate={endDate}
      setEndDate={setEndDate}
      reason={reason}
      setReason={setReason}
      leaveReasons={leaveReasons}
      description={description}
      setDescription={setDescription}
      handleSubmitRequestOff={handleSubmitRequestOff}
    />
  );
} 