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
    [Route("odata/IOGKFExamsDb/Exams")]
    public partial class ExamsController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamsController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> GetExams()
        {
            var items = this.context.Exams.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>();
            this.OnExamsRead(ref items);

            return items;
        }

        partial void OnExamsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> items);

        partial void OnExamGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/Exams(ExamId={ExamId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> GetExam(int key)
        {
            var items = this.context.Exams.Where(i => i.ExamId == key);
            var result = SingleResult.Create(items);

            OnExamGet(ref result);

            return result;
        }
        partial void OnExamDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        [HttpDelete("/odata/IOGKFExamsDb/Exams(ExamId={ExamId})")]
        public IActionResult DeleteExam(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Exams
                    .Where(i => i.ExamId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamDeleted(item);
                this.context.Exams.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        [HttpPut("/odata/IOGKFExamsDb/Exams(ExamId={ExamId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExam(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.Exam item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamId != key))
                {
                    return BadRequest();
                }
                this.OnExamUpdated(item);
                this.context.Exams.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Exams.Where(i => i.ExamId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamStatus");
                this.OnAfterExamUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/Exams(ExamId={ExamId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExam(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Exams.Where(i => i.ExamId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamUpdated(item);
                this.context.Exams.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Exams.Where(i => i.ExamId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ExamStatus");
                this.OnAfterExamUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);
        partial void OnAfterExamCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Exam item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.Exam item)
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

                this.OnExamCreated(item);
                this.context.Exams.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Exams.Where(i => i.ExamId == item.ExamId);

                Request.QueryString = Request.QueryString.Add("$expand", "ExamStatus");

                this.OnAfterExamCreated(item);

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
