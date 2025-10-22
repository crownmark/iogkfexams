using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using IOGKFExams.Server.Data;

namespace IOGKFExams.Server.Controllers
{
    public partial class ExportIOGKFExamsDbController : ExportController
    {
        private readonly IOGKFExamsDbContext context;
        private readonly IOGKFExamsDbService service;

        public ExportIOGKFExamsDbController(IOGKFExamsDbContext context, IOGKFExamsDbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/IOGKFExamsDb/countries/csv")]
        [HttpGet("/export/IOGKFExamsDb/countries/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCountriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCountries(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/countries/excel")]
        [HttpGet("/export/IOGKFExamsDb/countries/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCountriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCountries(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examanswers/csv")]
        [HttpGet("/export/IOGKFExamsDb/examanswers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamAnswersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamAnswers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examanswers/excel")]
        [HttpGet("/export/IOGKFExamsDb/examanswers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamAnswersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamAnswers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examquestions/csv")]
        [HttpGet("/export/IOGKFExamsDb/examquestions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamQuestionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamQuestions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examquestions/excel")]
        [HttpGet("/export/IOGKFExamsDb/examquestions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamQuestionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamQuestions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/exams/csv")]
        [HttpGet("/export/IOGKFExamsDb/exams/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExams(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/exams/excel")]
        [HttpGet("/export/IOGKFExamsDb/exams/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExams(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examstatuses/csv")]
        [HttpGet("/export/IOGKFExamsDb/examstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examstatuses/excel")]
        [HttpGet("/export/IOGKFExamsDb/examstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplateanswers/csv")]
        [HttpGet("/export/IOGKFExamsDb/examtemplateanswers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplateAnswersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamTemplateAnswers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplateanswers/excel")]
        [HttpGet("/export/IOGKFExamsDb/examtemplateanswers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplateAnswersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamTemplateAnswers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplatequestions/csv")]
        [HttpGet("/export/IOGKFExamsDb/examtemplatequestions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplateQuestionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamTemplateQuestions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplatequestions/excel")]
        [HttpGet("/export/IOGKFExamsDb/examtemplatequestions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplateQuestionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamTemplateQuestions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplates/csv")]
        [HttpGet("/export/IOGKFExamsDb/examtemplates/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplatesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExamTemplates(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/examtemplates/excel")]
        [HttpGet("/export/IOGKFExamsDb/examtemplates/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExamTemplatesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExamTemplates(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/languages/csv")]
        [HttpGet("/export/IOGKFExamsDb/languages/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLanguagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLanguages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/languages/excel")]
        [HttpGet("/export/IOGKFExamsDb/languages/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLanguagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLanguages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/ranks/csv")]
        [HttpGet("/export/IOGKFExamsDb/ranks/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRanksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRanks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/IOGKFExamsDb/ranks/excel")]
        [HttpGet("/export/IOGKFExamsDb/ranks/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRanksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRanks(), Request.Query, false), fileName);
        }
    }
}
