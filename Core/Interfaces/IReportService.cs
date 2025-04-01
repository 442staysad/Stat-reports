using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces
{
    public interface IReportService
    {
       Task<IEnumerable<Report>> GetAllReportsAsync();
      Task<ReportDto> GetReportByIdAsync(int id);
        Task<ReportDto> CreateReportAsync(ReportDto reportDto);
        Task<ReportDto> UpdateReportAsync(int id, ReportDto reportDto);
        Task<Report> CreateReportAsync(Report report);
        Task<Report> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(int id);
        Task<IEnumerable<Report>> GetReportsByBranchAsync(int branchId);
        Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file);
        Task<byte[]> DownloadReportAsync(int reportId);
        Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus newStatus, string? remarks = null);
        Task<bool> AddReportCommentAsync(int reportId, string comment);
        Task<List<PendingTemplateDto>> GetPendingTemplatesAsync(int? branchId);
        Task<Dictionary<string, Dictionary<string, List<List<string>>>>> ReadExcelFileAsync(int reportId);
    }
}