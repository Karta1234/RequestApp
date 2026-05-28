public class Request {
  private Request() {}
  public Request(int employeeId, int typeId, string reason, int quantity, string? customTemplate)
  {
    EmployeeId = employeeId;
    TypeId = typeId;
    Reason = reason;
    Quantity = quantity;
    CustomTemplate = customTemplate;
    CreatedAt = DateTime.UtcNow;
    ChangeStatus(RequestStatuses.New);
  }
  public int Id { get; private set; }
  public int StatusId { get; private set; }
  public bool IsActive { get; private set; }
  public RequestStatus Status { get; private set; } = null!;
  public int TypeId { get; private set; }
  public RequestType Type { get; set; } = null!;
  public string Reason { get; set; } = "";
  public string? CustomTemplate { get; set; }
  public int Quantity { get; set; }
  public int EmployeeId { get; private set; }

  public DateTime CreatedAt { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  public void ChangeStatus(RequestStatus newStatus) {
    StatusId = newStatus.Id;
    IsActive = !newStatus.IsTerminal;
    UpdatedAt = DateTime.UtcNow;
  }
}