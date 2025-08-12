namespace FNZ_ChatBot.Models
{
    public class UserManagementViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public int TotalUsers { get; set; }
        public int AdminUsers { get; set; }
        public int RegularUsers { get; set; }
    }
}