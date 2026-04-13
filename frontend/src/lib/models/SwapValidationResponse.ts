import { ViolationResultResponse } from "./DateValidationResponse";

export interface IndividualValidationResult {
  message: string;
  violations: ViolationResultResponse | null;
}

export interface SwapValidationResponse {
  success: boolean;
  message: string;
  requester?: IndividualValidationResult;
  requestee?: IndividualValidationResult;
}