namespace DHSOnlineStore.Models
{
    public class ContactViewModel
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Emails { get; set; }
        public string WorkingHours { get; set; }
        public List<ContactViewModel> Contacts { get; set; }
    }
}
