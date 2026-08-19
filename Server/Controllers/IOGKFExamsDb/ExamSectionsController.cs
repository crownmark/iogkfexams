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
    [Route("odata/IOGKFExamsDb/ExamSections")]
    public partial class ExamSectionsController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public ExamSectionsController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection> GetExamSections()
        {
            var items = this.context.ExamSections.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection>();
            this.OnExamSectionsRead(ref items);

            return items;
        }

        partial void OnExamSectionsRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection> items);

        partial void OnExamSectionGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/ExamSections(ExamSectionId={ExamSectionId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection> GetExamSection(int key)
        {
            var items = this.context.ExamSections.Where(i => i.ExamSectionId == key);
            var result = SingleResult.Create(items);

            OnExamSectionGet(ref result);

            return result;
        }
        partial void OnExamSectionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);
        partial void OnAfterExamSectionDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);

        [HttpDelete("/odata/IOGKFExamsDb/ExamSections(ExamSectionId={ExamSectionId})")]
        public IActionResult DeleteExamSection(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.ExamSections
                    .Where(i => i.ExamSectionId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnExamSectionDeleted(item);
                this.context.ExamSections.Remove(item);
                this.context.SaveChanges();
                this.OnAfterExamSectionDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamSectionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);
        partial void OnAfterExamSectionUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);

        [HttpPut("/odata/IOGKFExamsDb/ExamSections(ExamSectionId={ExamSectionId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutExamSection(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ExamSectionId != key))
                {
                    return BadRequest();
                }
                this.OnExamSectionUpdated(item);
                this.context.ExamSections.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamSections.Where(i => i.ExamSectionId == key);
                
                this.OnAfterExamSectionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/ExamSections(ExamSectionId={ExamSectionId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchExamSection(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.ExamSections.Where(i => i.ExamSectionId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnExamSectionUpdated(item);
                this.context.ExamSections.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamSections.Where(i => i.ExamSectionId == key);
                
                this.OnAfterExamSectionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnExamSectionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);
        partial void OnAfterExamSectionCreated(IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection item)
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

                this.OnExamSectionCreated(item);
                this.context.ExamSections.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ExamSections.Where(i => i.ExamSectionId == item.ExamSectionId);

                

                this.OnAfterExamSectionCreated(item);

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
