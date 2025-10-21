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
    [Route("odata/IOGKFExamsDb/Languages")]
    public partial class LanguagesController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public LanguagesController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> GetLanguages()
        {
            var items = this.context.Languages.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Language>();
            this.OnLanguagesRead(ref items);

            return items;
        }

        partial void OnLanguagesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> items);

        partial void OnLanguageGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Language> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/Languages(LanguageId={LanguageId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Language> GetLanguage(int key)
        {
            var items = this.context.Languages.Where(i => i.LanguageId == key);
            var result = SingleResult.Create(items);

            OnLanguageGet(ref result);

            return result;
        }
        partial void OnLanguageDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        [HttpDelete("/odata/IOGKFExamsDb/Languages(LanguageId={LanguageId})")]
        public IActionResult DeleteLanguage(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Languages
                    .Where(i => i.LanguageId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnLanguageDeleted(item);
                this.context.Languages.Remove(item);
                this.context.SaveChanges();
                this.OnAfterLanguageDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnLanguageUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        [HttpPut("/odata/IOGKFExamsDb/Languages(LanguageId={LanguageId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutLanguage(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.Language item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.LanguageId != key))
                {
                    return BadRequest();
                }
                this.OnLanguageUpdated(item);
                this.context.Languages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Languages.Where(i => i.LanguageId == key);
                
                this.OnAfterLanguageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/Languages(LanguageId={LanguageId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchLanguage(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.Language> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Languages.Where(i => i.LanguageId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnLanguageUpdated(item);
                this.context.Languages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Languages.Where(i => i.LanguageId == key);
                
                this.OnAfterLanguageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnLanguageCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);
        partial void OnAfterLanguageCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Language item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.Language item)
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

                this.OnLanguageCreated(item);
                this.context.Languages.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Languages.Where(i => i.LanguageId == item.LanguageId);

                

                this.OnAfterLanguageCreated(item);

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
