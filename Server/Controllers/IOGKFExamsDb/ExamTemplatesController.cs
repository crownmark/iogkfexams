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
    [Route("odata/IOGKFExamsDb/ExamTemplates")]
    public partial class ExamTemplatesController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamTemplatesController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> GetExamTemplates()
        {
            var items = this.context.ExamTemplates.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>();
            this.OnExamTemplatesRead(ref items);

            return items;
        }

        partial void OnExamTemplatesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> items);

        partial void OnExamTemplateGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamTemplates(ExamTemplateId={ExamTemplateId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> GetExamTemplate(int key)
        {
            var items = this.context.ExamTemplates.Where(i => i.ExamTemplateId == key);
            var result = SingleResult.Create(items);

            OnExamTemplateGet(ref result);

            return result;
        }
        partial void OnExamTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamTemplates(ExamTemplateId={ExamTemplateId})")]
        public IActionResult DeleteExamTemplate(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamTemplates
                    .Where(i => i.ExamTemplateId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamTemplateDeleted(item);
                this.context.ExamTemplates.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamTemplateDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        [HttpPut("/odata/IOGKFExamsDb/ExamTemplates(ExamTemplateId={ExamTemplateId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamTemplate(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamTemplateId != key))
                {
                    return BadRequest();
                }
                this.OnExamTemplateUpdated(item);
                this.context.ExamTemplates.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplates.Where(i => i.ExamTemplateId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Language");
                this.OnAfterExamTemplateUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamTemplates(ExamTemplateId={ExamTemplateId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamTemplate(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamTemplates.Where(i => i.ExamTemplateId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamTemplateUpdated(item);
                this.context.ExamTemplates.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplates.Where(i => i.ExamTemplateId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Language");
                this.OnAfterExamTemplateUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);
        partial void OnAfterExamTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate item)
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

                this.OnExamTemplateCreated(item);
                this.context.ExamTemplates.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamTemplates.Where(i => i.ExamTemplateId == item.ExamTemplateId);

                Request.QueryString = Request.QueryString.Add("$expand", "Language");

                this.OnAfterExamTemplateCreated(item);

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
