using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Stat_reports.Controllers
{
    [Route("api/summary-reports")]
    [ApiController]
    public class SummaryReportController : ControllerBase
    {
        private readonly ISummaryReportService _summaryReportService;

        public SummaryReportController(ISummaryReportService summaryReportService)
        {
            _summaryReportService = summaryReportService;
        }

        // Получить все сводные отчеты
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SummaryReportDto>>> GetAll()
        {
            var reports = await _summaryReportService.GetAllSummaryReportsAsync();
            return Ok(reports);
        }

        // Получить сводный отчет по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<SummaryReportDto>> GetById(int id)
        {
            var report = await _summaryReportService.GetSummaryReportByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        // Создать сводный отчет
        [HttpPost]
        public async Task<ActionResult<SummaryReportDto>> Create(SummaryReportDto reportDto)
        {
            var createdReport = await _summaryReportService.CreateSummaryReportAsync(reportDto);
            return CreatedAtAction(nameof(GetById), new { id = createdReport.Id }, createdReport);
        }

        // Удалить сводный отчет
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _summaryReportService.DeleteSummaryReportAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}