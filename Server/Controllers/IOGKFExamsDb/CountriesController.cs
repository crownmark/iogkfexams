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
    [Route("odata/IOGKFExamsDb/Countries")]
    public partial class CountriesController : ODataController
    {
        private IOGKFExams.Server.Data.IOGKFExamsDbContext context;

        public CountriesController(IOGKFExams.Server.Data.IOGKFExamsDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Country> GetCountries()
        {
            var items = this.context.Countries.AsQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Country>();
            this.OnCountriesRead(ref items);

            return items;
        }

        partial void OnCountriesRead(ref IQueryable<IOGKFExams.Server.Models.IOGKFExamsDb.Country> items);

        partial void OnCountryGet(ref SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Country> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/IOGKFExamsDb/Countries(CountryId={CountryId})")]
        public SingleResult<IOGKFExams.Server.Models.IOGKFExamsDb.Country> GetCountry(int key)
        {
            var items = this.context.Countries.Where(i => i.CountryId == key);
            var result = SingleResult.Create(items);

            OnCountryGet(ref result);

            return result;
        }
        partial void OnCountryDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryDeleted(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        [HttpDelete("/odata/IOGKFExamsDb/Countries(CountryId={CountryId})")]
        public IActionResult DeleteCountry(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Countries
                    .Where(i => i.CountryId == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnCountryDeleted(item);
                this.context.Countries.Remove(item);
                this.context.SaveChanges();
                this.OnAfterCountryDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnCountryUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryUpdated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        [HttpPut("/odata/IOGKFExamsDb/Countries(CountryId={CountryId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutCountry(int key, [FromBody]IOGKFExams.Server.Models.IOGKFExamsDb.Country item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.CountryId != key))
                {
                    return BadRequest();
                }
                this.OnCountryUpdated(item);
                this.context.Countries.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Countries.Where(i => i.CountryId == key);
                
                this.OnAfterCountryUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/IOGKFExamsDb/Countries(CountryId={CountryId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchCountry(int key, [FromBody]Delta<IOGKFExams.Server.Models.IOGKFExamsDb.Country> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Countries.Where(i => i.CountryId == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnCountryUpdated(item);
                this.context.Countries.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Countries.Where(i => i.CountryId == key);
                
                this.OnAfterCountryUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnCountryCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);
        partial void OnAfterCountryCreated(IOGKFExams.Server.Models.IOGKFExamsDb.Country item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] IOGKFExams.Server.Models.IOGKFExamsDb.Country item)
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

                this.OnCountryCreated(item);
                this.context.Countries.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Countries.Where(i => i.CountryId == item.CountryId);

                

                this.OnAfterCountryCreated(item);

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
