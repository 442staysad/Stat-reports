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
        private readonly IExcelSplitterService _excelToJsonService;
        private readonly IReportService _reportService;

        public ReportApiController(IReportService reportService, IExcelSplitterService excelToJsonService)
        {
            _reportService = reportService;
            _excelToJsonService = excelToJsonService;
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcelFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не загружен.");

            var filePath = Path.Combine("uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           // var jsonResult = await _excelToJsonService.ConvertToJSONAsync(filePath);
            return Ok();
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
            if (reportDto == null) return BadRequest("Неправильные данные.");
            var createdReport = await _reportService.CreateReportAsync(reportDto);
            return CreatedAtAction(nameof(GetReportById), new { id = createdReport.Id }, createdReport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportDto reportDto)
        {
            if (reportDto == null) return BadRequest("Неправильные данные.");

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



        [HttpPut("{reportId}/comment")]
        public async Task<IActionResult> AddReportComment(int reportId, [FromBody] string comment)
        {
            var result = await _reportService.AddReportCommentAsync(reportId, comment);
            return result ? Ok(new { Message = "Комментарий добавлен" }) : NotFound();
        }
        /*
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingTemplates()
        {
            var templates = _reportService.GetPendingTemplatesAsync();
            return Ok(templates);
        }*/
    }
}