namespace Domain.Services.SessionService
{
    public class UserSessionModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int RefreshTokenId { get; set; }
    }
}
