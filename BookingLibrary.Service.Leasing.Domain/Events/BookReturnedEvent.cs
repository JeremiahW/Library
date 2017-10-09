using System;
using BookingLibrary.Domain.Core;

namespace BookingLibrary.Service.Leasing.Domain.Events
{
    public class BookReturnedEvent : DomainEvent
    {
        public Guid BookId { get; set; }

        public Guid CustomerId { get; set; }
    }
}