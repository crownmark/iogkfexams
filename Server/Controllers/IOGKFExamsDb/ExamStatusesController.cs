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
    [Route("odata/IOGKFExamsDb/ExamStatuses")]
    public partial class ExamStatusesController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamStatusesController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> GetExamStatuses()
        {
            var items = this.context.ExamStatuses.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>();
            this.OnExamStatusesRead(ref items);

            return items;
        }

        partial void OnExamStatusesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> items);

        partial void OnExamStatusGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamStatuses(ExamStatusId={ExamStatusId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> GetExamStatus(int key)
        {
            var items = this.context.ExamStatuses.Where(i => i.ExamStatusId == key);
            var result = SingleResult.Create(items);

            OnExamStatusGet(ref result);

            return result;
        }
        partial void OnExamStatusDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamStatuses(ExamStatusId={ExamStatusId})")]
        public IActionResult DeleteExamStatus(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamStatuses
                    .Where(i => i.ExamStatusId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamStatusDeleted(item);
                this.context.ExamStatuses.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamStatusDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamStatusUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        [HttpPut("/odata/IOGKFExamsDb/ExamStatuses(ExamStatusId={ExamStatusId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamStatus(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamStatusId != key))
                {
                    return BadRequest();
                }
                this.OnExamStatusUpdated(item);
                this.context.ExamStatuses.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamStatuses.Where(i => i.ExamStatusId == key);
                
                this.OnAfterExamStatusUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamStatuses(ExamStatusId={ExamStatusId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamStatus(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamStatuses.Where(i => i.ExamStatusId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamStatusUpdated(item);
                this.context.ExamStatuses.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamStatuses.Where(i => i.ExamStatusId == key);
                
                this.OnAfterExamStatusUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamStatusCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);
        partial void OnAfterExamStatusCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus item)
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

                this.OnExamStatusCreated(item);
                this.context.ExamStatuses.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamStatuses.Where(i => i.ExamStatusId == item.ExamStatusId);

                

                this.OnAfterExamStatusCreated(item);

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
