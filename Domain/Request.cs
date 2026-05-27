public class Request {
  public int Id { get; set; }
  public int StatusId { get; private set; }
  public bool IsActive { get; private set; }
  public RequestStatus Status { get; set; } = null!;
  public int TypeId { get; set; }
  public RequestType Type { get; set; } = null!;
  public string Reason { get; set; } = "";
  public string? CustomTemplate { get; set; }
  public int Quantity { get; set; }
  public int EmployeeId { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public void ChangeStatus(RequestStatus newStatus) {
    StatusId = newStatus.Id;
    IsActive = !newStatus.IsTerminal;
    UpdatedAt = DateTime.UtcNow;
  }
}