
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; }
        public string UserName { get; set; } 
        public int? RelatedRequestId { get; set; } 
    }
