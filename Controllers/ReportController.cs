using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ST10287116_PROG6212_POE_P2.Services;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace ST10287116_PROG6212_POE_P2.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HR")]
    public class ReportController : Controller
    {
        private readonly ClaimService _claimService;
        public ReportController(ClaimService claimService) => _claimService = claimService;

        [HttpGet]
        public IActionResult ApprovedPdf(DateTime? from, DateTime? to)
        {
            var data = _claimService.GetApprovedClaimsReport(from, to);

            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            {
                using var pdf = new PdfDocument(writer);
                var doc = new Document(pdf);
                doc.Add(new Paragraph("Approved Claims Report").SetBold().SetFontSize(16));
                doc.Add(new Paragraph($"Period: {(from?.ToShortDateString() ?? "Start")} - {(to?.ToShortDateString() ?? "Now")}"));
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
                doc.Close();
            }
            return File(ms.ToArray(), "application/pdf", "ApprovedClaims.pdf");
        }
    }
}