using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models; // Ważne: dodaj ten using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReklamacjeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Zgłoszenia "W toku"
        [HttpGet("complaints/processing")]
        public async Task<IActionResult> GetProcessingComplaints()
        {
            var complaints = await _context.Zgloszenia
                .Include(z => z.Klient)
                .Include(z => z.Produkt)
                .Where(z => z.StatusOgolny == "Procesowana")
                .OrderByDescending(z => z.Id)
                .ToListAsync();

            var dtos = complaints.Select(z => new DashboardComplaintDto
            {
                Id = z.Id,
                NrZgloszenia = z.NrZgloszenia,
                // Teraz zadziała, bo dodaliśmy NazwaFirmy do modelu Klient
                Klient = !string.IsNullOrEmpty(z.Klient?.NazwaFirmy)
                         ? z.Klient.NazwaFirmy
                         : (z.Klient?.ImieNazwisko ?? "Brak klienta"),

                // POPRAWKA: Zmiana z NazwaKrotka na Nazwa
                Produkt = z.Produkt?.Nazwa ?? "Brak produktu",

                DniPoZgloszeniu = (DateTime.Now - z.DataZgloszenia).Days,
                Status = z.StatusOgolny
            }).ToList();

            return Ok(ApiResponse<List<DashboardComplaintDto>>.SuccessResponse(dtos));
        }

        // 2. Przypomnienia
        [HttpGet("reminders")]
        public async Task<IActionResult> GetReminders()
        {
            // Teraz zadziała, bo dodaliśmy DbSet<Przypomnienie> do Contextu
            var reminders = await _context.Przypomnienia
                .Where(r => r.Status == "Nowe" || r.Status == "Active" || string.IsNullOrEmpty(r.Status))
                .ToListAsync();

            var dtos = reminders.Select(r => {
                string cat = ClassifyCategory(r.Tresc);
                return new DashboardReminderDto
                {
                    Id = r.Id,
                    Tresc = r.Tresc,
                    DotyczyZgloszenia = r.DotyczyZgloszenia ?? "",
                    Kategoria = cat,
                    Kolor = GetColorForCategory(cat, r.Tresc)
                };
            }).ToList();

            return Ok(ApiResponse<List<DashboardReminderDto>>.SuccessResponse(dtos));
        }

        private string ClassifyCategory(string t)
        {
            if (string.IsNullOrEmpty(t)) return "Ręczne";
            t = t.ToUpper();

            if (t.Contains("[PROBLEM]") || t.Contains("[ZWROT]") || t.Contains("[ZGUBIONA]") ||
                t.Contains("[PRZESYŁKA]") || t.Contains("[W DORĘCZENIU]") || t.Contains("DPD") || t.Contains("KURIER"))
                return "Kurier";

            if (t.StartsWith("[AUTO]") || t.Contains("PILNE") || t.Contains("TERMIN") || t.Contains("DECYZJ"))
                return "Czas na decyzję";

            return "Ręczne";
        }

        private string GetColorForCategory(string cat, string t)
        {
            t = t.ToUpper();
            if (t.Contains("[PROBLEM]") || t.Contains("[ZWROT]") || t.Contains("[ZGUBIONA]")) return "#CD5C5C";
            if (cat == "Kurier") return "#6495ED";
            if (cat == "Czas na decyzję") return "#FFA500";
            return "#D3D3D3";
        }
    }
}