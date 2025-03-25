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
     //  Task<bool> DeleteReportAsync(int id);

      //  Task<IEnumerable<Report>> GetAllReportsAsync();
       // Task<Report> GetReportByIdAsync(int id);
        Task<Report> CreateReportAsync(Report report);
        Task<Report> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(int id);

        Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file);
        Task<byte[]> DownloadReportAsync(int reportId);
        Task<bool> UpdateReportStatusAsync(int reportId, int status, string remarks);
        Task<bool> AddReportCommentAsync(int reportId, string comment);
        Task<List<PendingTemplateDto>> GetPendingTemplatesAsync();

    }
}