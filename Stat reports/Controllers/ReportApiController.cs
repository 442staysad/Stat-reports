using System.Threading.Tasks;
using Core.DTO;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stat_reports.Controllers
{
    [Route("ReportApi")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportApiController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            return report == null ? NotFound() : Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReportDto reportDto)
        {
            if (reportDto == null) return BadRequest("Invalid report data.");

            var createdReport = await _reportService.CreateReportAsync(reportDto);
            return CreatedAtAction(nameof(GetReportById), new { id = createdReport.Id }, createdReport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportDto reportDto)
        {
            if (reportDto == null) return BadRequest("Invalid report data.");

            var updatedReport = await _reportService.UpdateReportAsync(id, reportDto);
            return updatedReport == null ? NotFound() : Ok(updatedReport);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var deleted = await _reportService.DeleteReportAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("upload/{templateId}/{branchId}/{uploadedById}")]
        public async Task<IActionResult> UploadReport(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл отсутствует или пуст.");

            var report = await _reportService.UploadReportAsync(templateId, branchId, uploadedById, file);
            return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
        }

        [HttpGet("download/{reportId}")]
        public async Task<IActionResult> Download(int reportId,string reportname)
        {
            var fileBytes = await _reportService.DownloadReportAsync(reportId);
            return /*fileBytes == null ? NotFound() :*/ File(fileBytes, "application/octet-stream", $"{reportname}.xlsx");
        }

        [Authorize(Roles = "Admin,Reviewer")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] ReportStatusUpdateDto statusDto)
        {
            if (statusDto == null) return BadRequest("Invalid data.");

            var result = await _reportService.UpdateReportStatusAsync(id, statusDto.Status, statusDto.Remarks);
            return result ? Ok() : NotFound();
        }

        [HttpPut("{reportId}/comment")]
        public async Task<IActionResult> AddReportComment(int reportId, [FromBody] string comment)
        {
            var result = await _reportService.AddReportCommentAsync(reportId, comment);
            return result ? Ok(new { Message = "Комментарий добавлен" }) : NotFound();
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingTemplates()
        {
            var templates = await _reportService.GetPendingTemplatesAsync();
            return Ok(templates);
        }
    }
}
