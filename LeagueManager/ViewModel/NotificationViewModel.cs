namespace LeagueManager.ViewModel
{
    public class NotificationViewModel
    {
        public int Round { get; set; } = 1;
        public DateTime? Deadline { get; set; } = DateTime.Now;
    }
}
