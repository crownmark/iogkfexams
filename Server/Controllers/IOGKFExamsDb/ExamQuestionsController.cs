using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOGKFExams.Server.Controllers.IOGKFExamsDb
{
    [Route("odata/IOGKFExamsDb/ExamQuestions")]
    public partial class ExamQuestionsController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamQuestionsController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> GetExamQuestions()
        {
            var items = this.context.ExamQuestions.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>();
            this.OnExamQuestionsRead(ref items);

            return items;
        }

        partial void OnExamQuestionsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> items);

        partial void OnExamQuestionGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamQuestions(ExamQuestionsId={ExamQuestionsId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> GetExamQuestion(int key)
        {
            var items = this.context.ExamQuestions.Where(i => i.ExamQuestionsId == key);
            var result = SingleResult.Create(items);

            OnExamQuestionGet(ref result);

            return result;
        }
        partial void OnExamQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamQuestions(ExamQuestionsId={ExamQuestionsId})")]
        public IActionResult DeleteExamQuestion(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamQuestions
                    .Where(i => i.ExamQuestionsId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamQuestionDeleted(item);
                this.context.ExamQuestions.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamQuestionDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        [HttpPut("/odata/IOGKFExamsDb/ExamQuestions(ExamQuestionsId={ExamQuestionsId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamQuestion(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamQuestionsId != key))
                {
                    return BadRequest();
                }
                this.OnExamQuestionUpdated(item);
                this.context.ExamQuestions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamQuestions.Where(i => i.ExamQuestionsId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Exam,Language,Rank");
                this.OnAfterExamQuestionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamQuestions(ExamQuestionsId={ExamQuestionsId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamQuestion(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamQuestions.Where(i => i.ExamQuestionsId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamQuestionUpdated(item);
                this.context.ExamQuestions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamQuestions.Where(i => i.ExamQuestionsId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Exam,Language,Rank");
                this.OnAfterExamQuestionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);
        partial void OnAfterExamQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnExamQuestionCreated(item);
                this.context.ExamQuestions.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamQuestions.Where(i => i.ExamQuestionsId == item.ExamQuestionsId);

                Request.QueryString = Request.QueryString.Add("$expand", "Exam,Language,Rank");

                this.OnAfterExamQuestionCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
