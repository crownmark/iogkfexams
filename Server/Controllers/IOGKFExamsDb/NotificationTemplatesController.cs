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
    [Route("odata/IOGKFExamsDb/NotificationTemplates")]
    public partial class NotificationTemplatesController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public NotificationTemplatesController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> GetNotificationTemplates()
        {
            var items = this.context.NotificationTemplates.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate>();
            this.OnNotificationTemplatesRead(ref items);

            return items;
        }

        partial void OnNotificationTemplatesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> items);

        partial void OnNotificationTemplateGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/NotificationTemplates(NotificationTemplateId={NotificationTemplateId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> GetNotificationTemplate(int key)
        {
            var items = this.context.NotificationTemplates.Where(i => i.NotificationTemplateId == key);
            var result = SingleResult.Create(items);

            OnNotificationTemplateGet(ref result);

            return result;
        }
        partial void OnNotificationTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);
        partial void OnAfterNotificationTemplateDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);

        [HttpDelete("/odata/IOGKFExamsDb/NotificationTemplates(NotificationTemplateId={NotificationTemplateId})")]
        public IActionResult DeleteNotificationTemplate(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.NotificationTemplates
                    .Where(i => i.NotificationTemplateId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnNotificationTemplateDeleted(item);
                this.context.NotificationTemplates.Remove(item);
                this.context.SaveChanges();
                this.OnAfterNotificationTemplateDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnNotificationTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);
        partial void OnAfterNotificationTemplateUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);

        [HttpPut("/odata/IOGKFExamsDb/NotificationTemplates(NotificationTemplateId={NotificationTemplateId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutNotificationTemplate(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.NotificationTemplateId != key))
                {
                    return BadRequest();
                }
                this.OnNotificationTemplateUpdated(item);
                this.context.NotificationTemplates.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.NotificationTemplates.Where(i => i.NotificationTemplateId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Language");
                this.OnAfterNotificationTemplateUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/NotificationTemplates(NotificationTemplateId={NotificationTemplateId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchNotificationTemplate(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.NotificationTemplates.Where(i => i.NotificationTemplateId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnNotificationTemplateUpdated(item);
                this.context.NotificationTemplates.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.NotificationTemplates.Where(i => i.NotificationTemplateId == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Language");
                this.OnAfterNotificationTemplateUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnNotificationTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);
        partial void OnAfterNotificationTemplateCreated(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate item)
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

                this.OnNotificationTemplateCreated(item);
                this.context.NotificationTemplates.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.NotificationTemplates.Where(i => i.NotificationTemplateId == item.NotificationTemplateId);

                Request.QueryString = Request.QueryString.Add("$expand", "Language");

                this.OnAfterNotificationTemplateCreated(item);

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
