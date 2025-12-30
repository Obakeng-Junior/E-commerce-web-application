using DHSOnlineStore.Models;

namespace DHSOnlineStore.ViewModels
{
    public class BookingHistoryViewModel
    {
        public CustomerServiceBooking Booking { get; set; }
        public List<CustomerServiceBooking.BookingStatusHistory> StatusHistory { get; set; }
    }
}
