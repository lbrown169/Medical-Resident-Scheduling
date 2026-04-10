export interface SwapRequestStatus {
  id: number;
  description: string;
}

export interface SwapRequest {
  swapRequestId: string;
  scheduleId: string;
  requesterId: string;
  requesteeId: string;
  requesterDate: string;
  requesteeDate: string;
  status: SwapRequestStatus;
  createdAt: string;
  updatedAt: string;
  isRead?: boolean;
  details?: string | null;
}
