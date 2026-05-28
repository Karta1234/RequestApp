  public static class RequestStatuses
  {
      public static readonly RequestStatus New      = new() { Id = 1, Name = "New",      IsTerminal = false };
      public static readonly RequestStatus InReview = new() { Id = 2, Name = "InReview", IsTerminal = false };
      public static readonly RequestStatus Approved = new() { Id = 3, Name = "Approved", IsTerminal = true  };
      public static readonly RequestStatus Rejected = new() { Id = 4, Name = "Rejected", IsTerminal = true  };

      public static IReadOnlyList<RequestStatus> All { get; } =
          [New, InReview, Approved, Rejected];

  }