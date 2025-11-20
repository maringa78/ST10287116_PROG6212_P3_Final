using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using iTextDocument = iText.Layout.Document; // Add an alias for iText's Document to resolve ambiguity

namespace ST10287116_PROG6212_POE_P2.Areas.HR.Pages.Reports
{
    [Authorize(Roles = "HR")]
    public class ApprovedModel : PageModel
    {
        private readonly ClaimService _claimService;
        public List<ApprovedRow> Rows { get; private set; } = new();
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }

        public record ApprovedRow(string ClaimId, string LecturerEmail, int Hours, decimal Rate, decimal Total, DateTime Date);

        public ApprovedModel(ClaimService claimService) => _claimService = claimService;

        public void OnGet()
        {
            var data = _claimService.GetApprovedClaimsReport(From, To);
            Rows = data.Select(r => new ApprovedRow(r.ClaimId, r.LecturerEmail, r.Hours, r.Rate, r.Total, r.Date)).ToList();
        }

        public IActionResult OnPostExportPdf()
        {
            var data = _claimService.GetApprovedClaimsReport(From, To);
            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            {
                // Use the alias to specify iText's Document
                var doc = new iTextDocument(pdf);
                doc.Add(new Paragraph("Approved Claims Report").SetBold().SetFontSize(16));
                doc.Add(new Paragraph($"Period: {(From?.ToShortDateString() ?? "Start")} - {(To?.ToShortDateString() ?? "Now")}"));
                var table = new Table(6).SetWidth(UnitValue.CreatePercentValue(100));
                table.AddHeaderCell("ClaimId");
                table.AddHeaderCell("Lecturer Email");
                table.AddHeaderCell("Hours");
                table.AddHeaderCell("Rate");
                table.AddHeaderCell("Total");
                table.AddHeaderCell("Date");
                foreach (var row in data)
                {
                    table.AddCell(row.ClaimId);
                    table.AddCell(row.LecturerEmail);
                    table.AddCell(row.Hours.ToString());
                    table.AddCell(row.Rate.ToString("F2"));
                    table.AddCell(row.Total.ToString("F2"));
                    table.AddCell(row.Date.ToShortDateString());
                }
                doc.Add(table);
                var grandTotal = data.Sum(d => d.Total);
                doc.Add(new Paragraph($"Grand Total: {grandTotal:F2}").SetBold());
                doc.Close();
            }
            return File(ms.ToArray(), "application/pdf", "ApprovedClaims.pdf");
        }
    }
}