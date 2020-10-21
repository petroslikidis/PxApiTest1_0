using DbSource;
using Microsoft.AspNetCore.Mvc;
using PCAxis.Menu;
using PCAxis.Menu.Implementations;
using PCAxis.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PxApiTest;
using PCAxis.Api.Serializers;

namespace PxApiTest.Controllers
{
    //[Route("api/v1/{language}/{db?}/{*path}")]
    [ApiController]
    [Route("api/v1")]
    public class LegacyApiController : ControllerBase
    {

        private IDatasource _dbSource;

        public LegacyApiController(IDatasource datasource)
        {
            _dbSource = datasource;
        }

        [HttpGet("{language}")]
        public IActionResult Get(string language)
        {
            //Check language validity
            if (!_dbSource.Languages.Contains(language))
            {
                //TODO log invalid language
                return NotFound();
            }

            //List databases
            return Ok(new[] { new MetaDb() {Id = _dbSource.DatabaseId, Text ="TODO DATABASE NAME" } });
        }

        [HttpGet("{language}/{db}/{*path}")]
        public IActionResult Get(string language, string db, string path)
        {
            //Check language validity
            if (!_dbSource.Languages.Contains(language))
            {
                //TODO log invalid language
                return NotFound();
            }

            //Check database validity
            if (db != _dbSource.DatabaseId)
            {
                //TODO log invalid database
                return NotFound();
            }

            //Resolve metadata
            var item = _dbSource.GetMenu(db, language, path);

            if (item is PxMenuItem)
            {
                return Ok(((PxMenuItem)item).GetMetaList().ToJSON(true));
            }
            else if (item is TableLink)
            {
                var builder = _dbSource.GetBuilder(language, path);
                builder.SetPreferredLanguage(language);
                return Ok(builder.GetTableMeta());
            }

            //TODO serach for it
            return NotFound();
        }

        [HttpPost("{language}/{db}/{*path}")]
        public IActionResult Post(string language, string db, string path, [FromBody] TableQuery tableQuery)
        {
            if (!_dbSource.Languages.Contains(language))
            {
                //TODO log invalid language
                return NotFound();
            }
            //Check database validity
            if (db != _dbSource.DatabaseId)
            {
                //TODO log invalid database
                return NotFound();
            }
            //Check that it is a table
            var item = _dbSource.GetMenu(db, language, path);
            if (!(item is TableLink))
            {
                //TODO log not a table
                return NotFound();
            }

            var builder = _dbSource.GetBuilder(language, path);
            builder.DoNotApplyCurrentValueSet = true;  // DoNotApplyCurrentValueSet means the "client that made the request" is an api(, not a GUI) so that
                                                       // CNMM2.4 property DefaultInGUI (for Valueset/grouping) should not be used  
            builder.SetPreferredLanguage(language);
            builder.BuildForSelection();

            // Process selections
            var selections = builder.BuildSelections(tableQuery);

            if (selections == null)
            {
                //TODO logg parameter error
                return NotFound();
            }

            // Check that the number of selected cells do not exceed the limit
            long cellCount = 1;
            foreach (var sel in selections)
            {
                if (sel.ValueCodes.Count > 0)
                {
                    cellCount *= sel.ValueCodes.Count;
                }
            }
            
            //if (cellCount > Settings.Current.FetchCellLimit)
            //{
            //    Write403Response(context);
            //    return;
            //}

            builder.BuildForPresentation(selections.ToArray());

            IWebSerializer serializer;
            switch (tableQuery.Response.Format != null ? tableQuery.Response.Format.ToLower() : null)
            {
                case null:
                case "px":
                    serializer = new PxSerializer();
                    break;
                //case "csv":
                //    serializer = new CsvSerializer();
                //    break;
                //case "json":
                //    serializer = new JsonSerializer();
                //    break;
                //case "json-stat":
                //    serializer = new JsonStatSeriaizer();
                //    break;
                //case "json-stat2":
                //    serializer = new JsonStat2Seriaizer();
                //    break;
                //case "xlsx":
                //    serializer = new XlsxSerializer();
                //    break;
                //case "sdmx":
                //    serializer = new SdmxDataSerializer();
                //    break;
                default:
                    throw new NotImplementedException("Serialization for " + tableQuery.Response.Format + " is not implemented");
            }
            var data = serializer.Serialize(builder.Model);
            serializer.Serialize(builder.Model);
            Response.ContentType = serializer.ContentType;
            Response.Body.WriteAsync(data, 0, data.Length);
            return Ok();

            //return Ok("Welcome to PxWeb API v1");
        }

       

    }
}
