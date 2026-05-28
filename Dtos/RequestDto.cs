public record RequestDto(
  int Id, int TypeId, string TypeName,
  int StatusId, string StatusName,string Reason,
  int Quantity, string? CustomTemplate, int EmployeeId,
  bool IsActive
) {
  public static RequestDto MapToDto(Request r, RequestType type, RequestStatus status)
  {
    return new RequestDto(
      r.Id, r.TypeId, type.Name,
      r.StatusId, status.Name,
      r.Reason, r.Quantity, r.CustomTemplate,
      r.EmployeeId, r.IsActive);
  }
};

