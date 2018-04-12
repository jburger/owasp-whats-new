namespace vulnerable.Domain
{
    public class EvaluationResult {
        public string RejectReason { get; private set; }
        public EvaluationStatus Status { get; private set; }
        public EvaluationResult(EvaluationStatus status, string rejectionReason = "")
        {
           Status = status;
           RejectReason = rejectionReason; 
        }
    }
}