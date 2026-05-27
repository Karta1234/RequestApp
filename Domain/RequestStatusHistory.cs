public class RequestStatusHistory
{
  public int Id { get; set; }
  public int RequestId { get; set; }
  public Request Request { get; set; } = null!;
  public int? FromStatusId { get; set; }
  public RequestStatus? FromStatus { get; set; }
  public int ToStatusId { get; set; }
    public RequestStatus ToStatus { get; set; } = null!;
  public int ChangedBy { get; set; }
  public DateTime ChangedAt { get; set; }
}