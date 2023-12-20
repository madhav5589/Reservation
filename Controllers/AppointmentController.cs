using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using Reservation.Models;
using System.Collections.Generic;
using System.Linq;

namespace Reservation.Controllers
{
    

    [Route("api/providers")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public ProvidersController(IMemoryCache cache)
        {
            _cache = cache;

            // Initialize providers and appointments if not already in cache
            if (!_cache.TryGetValue("Providers", out List<Provider>? providers))
            {
                providers = new List<Provider>
            {
                new Provider { Id = 1, Name = "Dr. Jekyll" }
                // Add more providers as needed
            };
                _cache.Set("Providers", providers);
            }

            if (!_cache.TryGetValue("Appointments", out List<Appointment>? appointments))
            {
                appointments = new List<Appointment>
            {
                new Appointment { Id = 1, ProviderId = 1, StartTime = new DateTime(2023, 8, 13, 8, 0, 0), EndTime = new DateTime(2023, 8, 13, 15, 0, 0), IsReserved = false }
                // Add more sample appointments as needed
            };
                _cache.Set("Appointments", appointments);
            }
        }

        
        [HttpPost("submit")]
        public IActionResult SubmitAvailability([FromBody] Appointment newAppointment)
        {
            var providers = _cache.Get<List<Provider>>("Providers");
            //var provider = providers.FirstOrDefault(p => p.Id == newAppointment.ProviderId);

            //if (provider == null)
            //{
            //    return NotFound("Provider not found.");
            //}

            var appointments = _cache.Get<List<Appointment>>("Appointments");

            // Divide the appointment into 15-minute slots
            var timeSlots = GetTimeSlots(newAppointment.StartTime, newAppointment.EndTime);

            // Assign unique IDs to each time slot
            var appointmentId = GetUniqueAppointmentId(appointments);

            // Create appointments for each time slot
            var timeSlotAppointments = timeSlots.Select(slot => new Appointment
            {
                Id = appointmentId++,
                ProviderId = newAppointment.ProviderId,
                StartTime = slot,
                EndTime = slot.AddMinutes(15),
                ReservationTime = null,
                ConfirmationTime = null,
                IsReserved = false
            }).ToList();

            appointments?.AddRange(timeSlotAppointments);
            providers?.Add(new Provider { Id = newAppointment.ProviderId });

            _cache.Set("Appointments", appointments);
            _cache.Set("Providers", providers);

            return Ok("Provider's availability submitted successfully.");
        }

        private List<DateTime> GetTimeSlots(DateTime start, DateTime end)
        {
            var timeSlots = new List<DateTime>();

            for (DateTime current = start; current < end; current = current.AddMinutes(15))
            {
                timeSlots.Add(current);
            }

            return timeSlots;
        }

        private int GetUniqueAppointmentId(List<Appointment>? appointments)
        {
            return appointments?.Count > 0 ? appointments.Max(a => a.Id) + 1 : 1;
        }


        [HttpGet("{providerId}/available")]
        public IActionResult GetAvailableSlots(int providerId)
        {
            var providers = _cache.Get<List<Provider>>("Providers");
            var provider = providers?.FirstOrDefault(p => p.Id == providerId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }

            var appointments = _cache.Get<List<Appointment>>("Appointments");

            // Check and expire reservations that are older than 30 minutes and haven't been confirmed yet
            var expiredReservations = appointments?
                .Where(a => a.ProviderId == providerId && a.IsReserved && a.ReservationTime.HasValue && !a.IsConfirmed && DateTime.Now.Subtract(a.ReservationTime.Value).TotalMinutes >= 30)
                .ToList();

            for (int i = 0; i < expiredReservations?.Count; i++)
            {
                Appointment? expiredReservation = expiredReservations[i];
                expiredReservation.IsReserved = false;
                expiredReservation.ReservationTime = null;
            }

            var availableSlots = appointments?
                .Where(appointment => appointment.ProviderId == providerId && !appointment.IsReserved)
                .Select(appointment => new Appointment
                {
                    Id = appointment.Id,
                    ProviderId = appointment.ProviderId,
                    IsReserved = appointment.IsReserved,
                    IsConfirmed = appointment.IsConfirmed,
                    ReservationTime = appointment.ReservationTime,
                    ConfirmationTime = appointment.ConfirmationTime,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.StartTime.AddMinutes(15)
                });

            return Ok(availableSlots);
        }

        [HttpPost("reserve/{id}")]
        public IActionResult ReserveSlot(int id)
        {
            var appointments = _cache.Get<List<Appointment>>("Appointments");

            var appointment = appointments?.FirstOrDefault(a => a.Id == id);

            if (appointment == null || appointment.IsReserved)
            {
                return NotFound("Appointment not found or already reserved.");
            }

            // Check if the reservation can be made at least 24 hours in advance
            var timeUntilAppointment = appointment.StartTime.Subtract(DateTime.Now);
            if (timeUntilAppointment.TotalHours < 24)
            {
                return BadRequest("Reservations must be made at least 24 hours in advance.");
            }

            appointment.IsReserved = true;
            appointment.ReservationTime = DateTime.Now;

            _cache.Set("Appointments", appointments);

            return Ok(appointment);
        }

        [HttpPost("confirm/{id}")]
        public IActionResult ConfirmReservation(int id)
        {
            var appointments = _cache.Get<List<Appointment>>("Appointments");

            var appointment = appointments?.FirstOrDefault(a => a.Id == id);

            if (appointment == null || !appointment.IsReserved || appointment.IsConfirmed)
            {
                return NotFound("Reservation not found or already confirmed.");
            }

            appointment.IsConfirmed = true;
            appointment.ConfirmationTime = DateTime.Now;

            _cache.Set("Appointments", appointments);

            return Ok(appointment);
        }
    }

}
