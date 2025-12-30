using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface ICustomerServiceBooking
    {
        Task AddServiceBooking(CustomerServiceBooking booking);
        Task LogBookingStatusChange(int bookingId, string status, string? reason = null);
        Task<IEnumerable<CustomerServiceBooking>> GetServiceBookingsByUser(string userId);
        Task<IEnumerable<CustomerServiceBooking>> GetAllServiceBookings();
        Task<CustomerServiceBooking> GetServiceBookingById(int bookingId);
        Task UpdateBookingStatus(int bookingId, string status);
    }
}
