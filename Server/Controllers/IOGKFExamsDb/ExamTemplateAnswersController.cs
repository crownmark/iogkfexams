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
    [Route("odata/IOGKFExamsDb/ExamTemplateAnswers")]
    public partial class ExamTemplateAnswersController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamTemplateAnswersController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> GetExamTemplateAnswers()
        {
            var items = this.context.ExamTemplateAnswers.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>();
            this.OnExamTemplateAnswersRead(ref items);

            return items;
        }

        partial void OnExamTemplateAnswersRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> items);

        partial void OnExamTemplateAnswerGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamTemplateAnswers(ExamTemplateAnswerId={ExamTemplateAnswerId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> GetExamTemplateAnswer(int key)
        {
            var items = this.context.ExamTemplateAnswers.Where(i => i.ExamTemplateAnswerId == key);
            var result = SingleResult.Create(items);

            OnExamTemplateAnswerGet(ref result);

            return result;
        }
        partial void OnExamTemplateAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamTemplateAnswers(ExamTemplateAnswerId={ExamTemplateAnswerId})")]
        public IActionResult DeleteExamTemplateAnswer(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamTemplateAnswers
                    .Where(i => i.ExamTemplateAnswerId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamTemplateAnswerDeleted(item);
                this.context.ExamTemplateAnswers.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamTemplateAnswerDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        [HttpPut("/odata/IOGKFExamsDb/ExamTemplateAnswers(ExamTemplateAnswerId={ExamTemplateAnswerId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamTemplateAnswer(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamTemplateAnswerId != key))
                {
                    return BadRequest();
                }
                this.OnExamTemplateAnswerUpdated(item);
                this.context.ExamTemplateAnswers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateAnswers.Where(i => i.ExamTemplateAnswerId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplateQuestion");
                this.OnAfterExamTemplateAnswerUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamTemplateAnswers(ExamTemplateAnswerId={ExamTemplateAnswerId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamTemplateAnswer(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamTemplateAnswers.Where(i => i.ExamTemplateAnswerId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamTemplateAnswerUpdated(item);
                this.context.ExamTemplateAnswers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateAnswers.Where(i => i.ExamTemplateAnswerId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplateQuestion");
                this.OnAfterExamTemplateAnswerUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);
        partial void OnAfterExamTemplateAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer item)
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

                this.OnExamTemplateAnswerCreated(item);
                this.context.ExamTemplateAnswers.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplateAnswers.Where(i => i.ExamTemplateAnswerId == item.ExamTemplateAnswerId);

                Request.QueryString = Request.QueryString.Add("$expand", "ExamTemplateQuestion");

                this.OnAfterExamTemplateAnswerCreated(item);

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
