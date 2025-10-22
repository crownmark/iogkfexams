using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using IOGKFExams.Server.Data;

namespace IOGKFExams.Server
{
    public partial class IOGKFExamsDbService
    {
        IOGKFExamsDbContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly IOGKFExamsDbContext context;
        private readonly NavigationManager navigationManager;

        public IOGKFExamsDbService(IOGKFExamsDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportCountriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCountriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCountriesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Country> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Country>> GetCountries(Query query = null)
        {
            var items = Context.Countries.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCountriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCountryGet(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnGetCountryByCountryId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Country> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> GetCountryByCountryId(int countryid)
        {
            var items = Context.Countries
                              .AsNoTracking()
                              .Where(i => i.CountryId == countryid);

 
            OnGetCountryByCountryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCountryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCountryCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> CreateCountry(IOGKFExams.Server.Models.IOGKFExamsDb.Country country)
        {
            OnCountryCreated(country);

            var existingItem = Context.Countries
                              .Where(i => i.CountryId == country.CountryId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Countries.Add(country);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(country).State = EntityState.Detached;
                throw;
            }

            OnAfterCountryCreated(country);

            return country;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> CancelCountryChanges(IOGKFExams.Server.Models.IOGKFExamsDb.Country item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCountryUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> UpdateCountry(int countryid, IOGKFExams.Server.Models.IOGKFExamsDb.Country country)
        {
            OnCountryUpdated(country);

            var itemToUpdate = Context.Countries
                              .Where(i => i.CountryId == country.CountryId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(country);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCountryUpdated(country);

            return country;
        }

        partial void OnCountryDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> DeleteCountry(int countryid)
        {
            var itemToDelete = Context.Countries
                              .Where(i => i.CountryId == countryid)
                              .Include(i => i.Exams)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCountryDeleted(itemToDelete);


            Context.Countries.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCountryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamAnswersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamAnswersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamAnswersRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>> GetExamAnswers(Query query = null)
        {
            var items = Context.ExamAnswers.AsQueryable();

            items = items.Include(i => i.ExamQuestion);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamAnswersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamAnswerGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnGetExamAnswerByExamAnswerId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> GetExamAnswerByExamAnswerId(int examanswerid)
        {
            var items = Context.ExamAnswers
                              .AsNoTracking()
                              .Where(i => i.ExamAnswerId == examanswerid);

            items = items.Include(i => i.ExamQuestion);
 
            OnGetExamAnswerByExamAnswerId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamAnswerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> CreateExamAnswer(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer examanswer)
        {
            OnExamAnswerCreated(examanswer);

            var existingItem = Context.ExamAnswers
                              .Where(i => i.ExamAnswerId == examanswer.ExamAnswerId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamAnswers.Add(examanswer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examanswer).State = EntityState.Detached;
                throw;
            }

            OnAfterExamAnswerCreated(examanswer);

            return examanswer;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> CancelExamAnswerChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> UpdateExamAnswer(int examanswerid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer examanswer)
        {
            OnExamAnswerUpdated(examanswer);

            var itemToUpdate = Context.ExamAnswers
                              .Where(i => i.ExamAnswerId == examanswer.ExamAnswerId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examanswer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamAnswerUpdated(examanswer);

            return examanswer;
        }

        partial void OnExamAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> DeleteExamAnswer(int examanswerid)
        {
            var itemToDelete = Context.ExamAnswers
                              .Where(i => i.ExamAnswerId == examanswerid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamAnswerDeleted(itemToDelete);


            Context.ExamAnswers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamAnswerDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamQuestionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examquestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examquestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamQuestionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examquestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examquestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamQuestionsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>> GetExamQuestions(Query query = null)
        {
            var items = Context.ExamQuestions.AsQueryable();

            items = items.Include(i => i.Exam);
            items = items.Include(i => i.Language);
            items = items.Include(i => i.Rank);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamQuestionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamQuestionGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnGetExamQuestionByExamQuestionsId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> GetExamQuestionByExamQuestionsId(int examquestionsid)
        {
            var items = Context.ExamQuestions
                              .AsNoTracking()
                              .Where(i => i.ExamQuestionsId == examquestionsid);

            items = items.Include(i => i.Exam);
            items = items.Include(i => i.Language);
            items = items.Include(i => i.Rank);
 
            OnGetExamQuestionByExamQuestionsId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamQuestionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> CreateExamQuestion(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion examquestion)
        {
            OnExamQuestionCreated(examquestion);

            var existingItem = Context.ExamQuestions
                              .Where(i => i.ExamQuestionsId == examquestion.ExamQuestionsId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamQuestions.Add(examquestion);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examquestion).State = EntityState.Detached;
                throw;
            }

            OnAfterExamQuestionCreated(examquestion);

            return examquestion;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> CancelExamQuestionChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> UpdateExamQuestion(int examquestionsid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion examquestion)
        {
            OnExamQuestionUpdated(examquestion);

            var itemToUpdate = Context.ExamQuestions
                              .Where(i => i.ExamQuestionsId == examquestion.ExamQuestionsId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examquestion);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamQuestionUpdated(examquestion);

            return examquestion;
        }

        partial void OnExamQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> DeleteExamQuestion(int examquestionsid)
        {
            var itemToDelete = Context.ExamQuestions
                              .Where(i => i.ExamQuestionsId == examquestionsid)
                              .Include(i => i.ExamAnswers)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamQuestionDeleted(itemToDelete);


            Context.ExamQuestions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamQuestionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/exams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/exams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/exams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/exams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>> GetExams(Query query = null)
        {
            var items = Context.Exams.AsQueryable();

            items = items.Include(i => i.Country);
            items = items.Include(i => i.ExamStatus);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamGet(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnGetExamByExamId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> GetExamByExamId(int examid)
        {
            var items = Context.Exams
                              .AsNoTracking()
                              .Where(i => i.ExamId == examid);

            items = items.Include(i => i.Country);
            items = items.Include(i => i.ExamStatus);
 
            OnGetExamByExamId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> CreateExam(IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            OnExamCreated(exam);

            var existingItem = Context.Exams
                              .Where(i => i.ExamId == exam.ExamId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Exams.Add(exam);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(exam).State = EntityState.Detached;
                throw;
            }

            OnAfterExamCreated(exam);

            return exam;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> CancelExamChanges(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> UpdateExam(int examid, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            OnExamUpdated(exam);

            var itemToUpdate = Context.Exams
                              .Where(i => i.ExamId == exam.ExamId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(exam);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamUpdated(exam);

            return exam;
        }

        partial void OnExamDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> DeleteExam(int examid)
        {
            var itemToDelete = Context.Exams
                              .Where(i => i.ExamId == examid)
                              .Include(i => i.ExamQuestions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamDeleted(itemToDelete);


            Context.Exams.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamStatusesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>> GetExamStatuses(Query query = null)
        {
            var items = Context.ExamStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamStatusGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnGetExamStatusByExamStatusId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> GetExamStatusByExamStatusId(int examstatusid)
        {
            var items = Context.ExamStatuses
                              .AsNoTracking()
                              .Where(i => i.ExamStatusId == examstatusid);

 
            OnGetExamStatusByExamStatusId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamStatusCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> CreateExamStatus(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examstatus)
        {
            OnExamStatusCreated(examstatus);

            var existingItem = Context.ExamStatuses
                              .Where(i => i.ExamStatusId == examstatus.ExamStatusId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamStatuses.Add(examstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterExamStatusCreated(examstatus);

            return examstatus;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> CancelExamStatusChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamStatusUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> UpdateExamStatus(int examstatusid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examstatus)
        {
            OnExamStatusUpdated(examstatus);

            var itemToUpdate = Context.ExamStatuses
                              .Where(i => i.ExamStatusId == examstatus.ExamStatusId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamStatusUpdated(examstatus);

            return examstatus;
        }

        partial void OnExamStatusDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> DeleteExamStatus(int examstatusid)
        {
            var itemToDelete = Context.ExamStatuses
                              .Where(i => i.ExamStatusId == examstatusid)
                              .Include(i => i.Exams)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamStatusDeleted(itemToDelete);


            Context.ExamStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamStatusDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamTemplateAnswersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplateanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplateanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamTemplateAnswersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplateanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplateanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamTemplateAnswersRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>> GetExamTemplateAnswers(Query query = null)
        {
            var items = Context.ExamTemplateAnswers.AsQueryable();

            items = items.Include(i => i.ExamTemplateQuestion);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamTemplateAnswersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamTemplateAnswerGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnGetExamTemplateAnswerByExamTemplateAnswerId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> GetExamTemplateAnswerByExamTemplateAnswerId(int examtemplateanswerid)
        {
            var items = Context.ExamTemplateAnswers
                              .AsNoTracking()
                              .Where(i => i.ExamTemplateAnswerId == examtemplateanswerid);

            items = items.Include(i => i.ExamTemplateQuestion);
 
            OnGetExamTemplateAnswerByExamTemplateAnswerId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamTemplateAnswerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamTemplateAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> CreateExamTemplateAnswer(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examtemplateanswer)
        {
            OnExamTemplateAnswerCreated(examtemplateanswer);

            var existingItem = Context.ExamTemplateAnswers
                              .Where(i => i.ExamTemplateAnswerId == examtemplateanswer.ExamTemplateAnswerId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamTemplateAnswers.Add(examtemplateanswer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examtemplateanswer).State = EntityState.Detached;
                throw;
            }

            OnAfterExamTemplateAnswerCreated(examtemplateanswer);

            return examtemplateanswer;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> CancelExamTemplateAnswerChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamTemplateAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> UpdateExamTemplateAnswer(int examtemplateanswerid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examtemplateanswer)
        {
            OnExamTemplateAnswerUpdated(examtemplateanswer);

            var itemToUpdate = Context.ExamTemplateAnswers
                              .Where(i => i.ExamTemplateAnswerId == examtemplateanswer.ExamTemplateAnswerId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examtemplateanswer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamTemplateAnswerUpdated(examtemplateanswer);

            return examtemplateanswer;
        }

        partial void OnExamTemplateAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> DeleteExamTemplateAnswer(int examtemplateanswerid)
        {
            var itemToDelete = Context.ExamTemplateAnswers
                              .Where(i => i.ExamTemplateAnswerId == examtemplateanswerid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamTemplateAnswerDeleted(itemToDelete);


            Context.ExamTemplateAnswers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamTemplateAnswerDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamTemplateQuestionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplatequestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplatequestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamTemplateQuestionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplatequestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplatequestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamTemplateQuestionsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>> GetExamTemplateQuestions(Query query = null)
        {
            var items = Context.ExamTemplateQuestions.AsQueryable();

            items = items.Include(i => i.ExamTemplate);
            items = items.Include(i => i.Language);
            items = items.Include(i => i.Rank);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamTemplateQuestionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamTemplateQuestionGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnGetExamTemplateQuestionByExamTemplateQuestionsId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> GetExamTemplateQuestionByExamTemplateQuestionsId(int examtemplatequestionsid)
        {
            var items = Context.ExamTemplateQuestions
                              .AsNoTracking()
                              .Where(i => i.ExamTemplateQuestionsId == examtemplatequestionsid);

            items = items.Include(i => i.ExamTemplate);
            items = items.Include(i => i.Language);
            items = items.Include(i => i.Rank);
 
            OnGetExamTemplateQuestionByExamTemplateQuestionsId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamTemplateQuestionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamTemplateQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> CreateExamTemplateQuestion(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examtemplatequestion)
        {
            OnExamTemplateQuestionCreated(examtemplatequestion);

            var existingItem = Context.ExamTemplateQuestions
                              .Where(i => i.ExamTemplateQuestionsId == examtemplatequestion.ExamTemplateQuestionsId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamTemplateQuestions.Add(examtemplatequestion);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examtemplatequestion).State = EntityState.Detached;
                throw;
            }

            OnAfterExamTemplateQuestionCreated(examtemplatequestion);

            return examtemplatequestion;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> CancelExamTemplateQuestionChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamTemplateQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> UpdateExamTemplateQuestion(int examtemplatequestionsid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examtemplatequestion)
        {
            OnExamTemplateQuestionUpdated(examtemplatequestion);

            var itemToUpdate = Context.ExamTemplateQuestions
                              .Where(i => i.ExamTemplateQuestionsId == examtemplatequestion.ExamTemplateQuestionsId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examtemplatequestion);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamTemplateQuestionUpdated(examtemplatequestion);

            return examtemplatequestion;
        }

        partial void OnExamTemplateQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> DeleteExamTemplateQuestion(int examtemplatequestionsid)
        {
            var itemToDelete = Context.ExamTemplateQuestions
                              .Where(i => i.ExamTemplateQuestionsId == examtemplatequestionsid)
                              .Include(i => i.ExamTemplateAnswers)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamTemplateQuestionDeleted(itemToDelete);


            Context.ExamTemplateQuestions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamTemplateQuestionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExamTemplatesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplates/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplates/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExamTemplatesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplates/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplates/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExamTemplatesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>> GetExamTemplates(Query query = null)
        {
            var items = Context.ExamTemplates.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnExamTemplatesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExamTemplateGet(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnGetExamTemplateByExamTemplateId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> GetExamTemplateByExamTemplateId(int examtemplateid)
        {
            var items = Context.ExamTemplates
                              .AsNoTracking()
                              .Where(i => i.ExamTemplateId == examtemplateid);

 
            OnGetExamTemplateByExamTemplateId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnExamTemplateGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExamTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> CreateExamTemplate(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate examtemplate)
        {
            OnExamTemplateCreated(examtemplate);

            var existingItem = Context.ExamTemplates
                              .Where(i => i.ExamTemplateId == examtemplate.ExamTemplateId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ExamTemplates.Add(examtemplate);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(examtemplate).State = EntityState.Detached;
                throw;
            }

            OnAfterExamTemplateCreated(examtemplate);

            return examtemplate;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> CancelExamTemplateChanges(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExamTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> UpdateExamTemplate(int examtemplateid, IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate examtemplate)
        {
            OnExamTemplateUpdated(examtemplate);

            var itemToUpdate = Context.ExamTemplates
                              .Where(i => i.ExamTemplateId == examtemplate.ExamTemplateId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(examtemplate);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExamTemplateUpdated(examtemplate);

            return examtemplate;
        }

        partial void OnExamTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> DeleteExamTemplate(int examtemplateid)
        {
            var itemToDelete = Context.ExamTemplates
                              .Where(i => i.ExamTemplateId == examtemplateid)
                              .Include(i => i.ExamTemplateQuestions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExamTemplateDeleted(itemToDelete);


            Context.ExamTemplates.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExamTemplateDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportLanguagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLanguagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLanguagesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Language>> GetLanguages(Query query = null)
        {
            var items = Context.Languages.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnLanguagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLanguageGet(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnGetLanguageByLanguageId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> GetLanguageByLanguageId(int languageid)
        {
            var items = Context.Languages
                              .AsNoTracking()
                              .Where(i => i.LanguageId == languageid);

 
            OnGetLanguageByLanguageId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLanguageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLanguageCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> CreateLanguage(IOGKFExams.Server.Models.IOGKFExamsDb.Language language)
        {
            OnLanguageCreated(language);

            var existingItem = Context.Languages
                              .Where(i => i.LanguageId == language.LanguageId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Languages.Add(language);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(language).State = EntityState.Detached;
                throw;
            }

            OnAfterLanguageCreated(language);

            return language;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> CancelLanguageChanges(IOGKFExams.Server.Models.IOGKFExamsDb.Language item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLanguageUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> UpdateLanguage(int languageid, IOGKFExams.Server.Models.IOGKFExamsDb.Language language)
        {
            OnLanguageUpdated(language);

            var itemToUpdate = Context.Languages
                              .Where(i => i.LanguageId == language.LanguageId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(language);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLanguageUpdated(language);

            return language;
        }

        partial void OnLanguageDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> DeleteLanguage(int languageid)
        {
            var itemToDelete = Context.Languages
                              .Where(i => i.LanguageId == languageid)
                              .Include(i => i.ExamQuestions)
                              .Include(i => i.ExamTemplateQuestions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnLanguageDeleted(itemToDelete);


            Context.Languages.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLanguageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportRanksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/ranks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/ranks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRanksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/ranks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/ranks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRanksRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> items);

        public async Task<IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>> GetRanks(Query query = null)
        {
            var items = Context.Ranks.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnRanksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRankGet(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnGetRankByRankId(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> items);


        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> GetRankByRankId(int rankid)
        {
            var items = Context.Ranks
                              .AsNoTracking()
                              .Where(i => i.RankId == rankid);

 
            OnGetRankByRankId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRankGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRankCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> CreateRank(IOGKFExams.Server.Models.IOGKFExamsDb.Rank rank)
        {
            OnRankCreated(rank);

            var existingItem = Context.Ranks
                              .Where(i => i.RankId == rank.RankId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Ranks.Add(rank);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rank).State = EntityState.Detached;
                throw;
            }

            OnAfterRankCreated(rank);

            return rank;
        }

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> CancelRankChanges(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRankUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> UpdateRank(int rankid, IOGKFExams.Server.Models.IOGKFExamsDb.Rank rank)
        {
            OnRankUpdated(rank);

            var itemToUpdate = Context.Ranks
                              .Where(i => i.RankId == rank.RankId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rank);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRankUpdated(rank);

            return rank;
        }

        partial void OnRankDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> DeleteRank(int rankid)
        {
            var itemToDelete = Context.Ranks
                              .Where(i => i.RankId == rankid)
                              .Include(i => i.ExamQuestions)
                              .Include(i => i.ExamTemplateQuestions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRankDeleted(itemToDelete);


            Context.Ranks.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRankDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}