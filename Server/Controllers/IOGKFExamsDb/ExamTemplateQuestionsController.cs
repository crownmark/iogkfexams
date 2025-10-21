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
    [Route("odata/IOGKFExamsDb/ExamTemplateQuestions")]
    public partial class ExamTemplateQuestionsController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamTemplateQuestionsController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> GetExamTemplateQuestions()
        {
            var items = this.context.ExamTemplateQuestions.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>();
            this.OnExamTemplateQuestionsRead(ref items);

            return items;
        }

        partial void OnExamTemplateQuestionsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> items);

        partial void OnExamTemplateQuestionGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamTemplateQuestions(ExamTemplateQuestionsId={ExamTemplateQuestionsId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> GetExamTemplateQuestion(int key)
        {
            var items = this.context.ExamTemplateQuestions.Where(i => i.ExamTemplateQuestionsId == key);
            var result = SingleResult.Create(items);

            OnExamTemplateQuestionGet(ref result);

            return result;
        }
        partial void OnExamTemplateQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamTemplateQuestions(ExamTemplateQuestionsId={ExamTemplateQuestionsId})")]
        public IActionResult DeleteExamTemplateQuestion(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamTemplateQuestions
                    .Where(i => i.ExamTemplateQuestionsId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamTemplateQuestionDeleted(item);
                this.context.ExamTemplateQuestions.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamTemplateQuestionDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        [HttpPut("/odata/IOGKFExamsDb/ExamTemplateQuestions(ExamTemplateQuestionsId={ExamTemplateQuestionsId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamTemplateQuestion(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamTemplateQuestionsId != key))
                {
                    return BadRequest();
                }
                this.OnExamTemplateQuestionUpdated(item);
                this.context.ExamTemplateQuestions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateQuestions.Where(i => i.ExamTemplateQuestionsId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplate,Language,Rank");
                this.OnAfterExamTemplateQuestionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamTemplateQuestions(ExamTemplateQuestionsId={ExamTemplateQuestionsId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamTemplateQuestion(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamTemplateQuestions.Where(i => i.ExamTemplateQuestionsId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamTemplateQuestionUpdated(item);
                this.context.ExamTemplateQuestions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateQuestions.Where(i => i.ExamTemplateQuestionsId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplate,Language,Rank");
                this.OnAfterExamTemplateQuestionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);
        partial void OnAfterExamTemplateQuestionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion item)
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

                this.OnExamTemplateQuestionCreated(item);
                this.context.ExamTemplateQuestions.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateQuestions.Where(i => i.ExamTemplateQuestionsId == item.ExamTemplateQuestionsId);

                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplate,Language,Rank");

                this.OnAfterExamTemplateQuestionCreated(item);

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
