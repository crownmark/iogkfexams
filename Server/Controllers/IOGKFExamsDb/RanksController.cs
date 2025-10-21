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
    [Route("odata/IOGKFExamsDb/Ranks")]
    public partial class RanksController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public RanksController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> GetRanks()
        {
            var items = this.context.Ranks.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>();
            this.OnRanksRead(ref items);

            return items;
        }

        partial void OnRanksRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> items);

        partial void OnRankGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/Ranks(RankId={RankId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> GetRank(int key)
        {
            var items = this.context.Ranks.Where(i => i.RankId == key);
            var result = SingleResult.Create(items);

            OnRankGet(ref result);

            return result;
        }
        partial void OnRankDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        [HttpDelete("/odata/IOGKFExamsDb/Ranks(RankId={RankId})")]
        public IActionResult DeleteRank(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Ranks
                    .Where(i => i.RankId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnRankDeleted(item);
                this.context.Ranks.Remove(item);
                this.context.SaveChanges();
                this.OnAfterRankDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnRankUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        [HttpPut("/odata/IOGKFExamsDb/Ranks(RankId={RankId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutRank(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.Rank item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.RankId != key))
                {
                    return BadRequest();
                }
                this.OnRankUpdated(item);
                this.context.Ranks.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Ranks.Where(i => i.RankId == key);
                
                this.OnAfterRankUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/Ranks(RankId={RankId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchRank(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Ranks.Where(i => i.RankId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnRankUpdated(item);
                this.context.Ranks.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Ranks.Where(i => i.RankId == key);
                
                this.OnAfterRankUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnRankCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);
        partial void OnAfterRankCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Rank item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.Rank item)
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

                this.OnRankCreated(item);
                this.context.Ranks.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Ranks.Where(i => i.RankId == item.RankId);

                

                this.OnAfterRankCreated(item);

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
