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
    [Route("odata/IOGKFExamsDb/ExamAnswers")]
    public partial class ExamAnswersController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamAnswersController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> GetExamAnswers()
        {
            var items = this.context.ExamAnswers.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>();
            this.OnExamAnswersRead(ref items);

            return items;
        }

        partial void OnExamAnswersRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> items);

        partial void OnExamAnswerGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamAnswers(ExamAnswerId={ExamAnswerId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> GetExamAnswer(int key)
        {
            var items = this.context.ExamAnswers.Where(i => i.ExamAnswerId == key);
            var result = SingleResult.Create(items);

            OnExamAnswerGet(ref result);

            return result;
        }
        partial void OnExamAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamAnswers(ExamAnswerId={ExamAnswerId})")]
        public IActionResult DeleteExamAnswer(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamAnswers
                    .Where(i => i.ExamAnswerId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamAnswerDeleted(item);
                this.context.ExamAnswers.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamAnswerDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        [HttpPut("/odata/IOGKFExamsDb/ExamAnswers(ExamAnswerId={ExamAnswerId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamAnswer(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamAnswerId != key))
                {
                    return BadRequest();
                }
                this.OnExamAnswerUpdated(item);
                this.context.ExamAnswers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamAnswers.Where(i => i.ExamAnswerId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamQuestion");
                this.OnAfterExamAnswerUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamAnswers(ExamAnswerId={ExamAnswerId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamAnswer(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamAnswers.Where(i => i.ExamAnswerId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamAnswerUpdated(item);
                this.context.ExamAnswers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamAnswers.Where(i => i.ExamAnswerId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamQuestion");
                this.OnAfterExamAnswerUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);
        partial void OnAfterExamAnswerCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer item)
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

                this.OnExamAnswerCreated(item);
                this.context.ExamAnswers.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamAnswers.Where(i => i.ExamAnswerId == item.ExamAnswerId);

                Request.QueryString = Request.QueryString.Add("$expand", "ExamQuestion");

                this.OnAfterExamAnswerCreated(item);

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
