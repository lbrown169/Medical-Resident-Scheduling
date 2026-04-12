export interface ConstraintResultResponse {
  isViolated: boolean;
  message: string | null;
  isOverridable: boolean | null;
}

export interface ViolationResultResponse {
  isViolation: boolean;
  isOverridable: boolean | null;
  violations: ConstraintResultResponse[];
}

export interface DateValidationResponse {
  success: boolean;
  message: string;
  violationResultResponse: ViolationResultResponse | null;
}