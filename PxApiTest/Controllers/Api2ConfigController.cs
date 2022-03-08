using Microsoft.AspNetCore.Mvc;
using System;

namespace PxApiTest.Controllers
{
    [ApiController]
    [Route("api/v2/tables")]
    public class Api2ConfigController : Controller
    {
        [HttpGet()]
        public string[] ListTables([FromQuery] string query = null, [FromQuery] DateTime? updatedSince = null)
        {
            if (query != null)
            {
                return new string[] { "query result 1", "query result 2" };
            }
            if (updatedSince.HasValue)
            {
                return new string[] { "Updated table 1" };
            }
            return new string[] { "Table 1", "Table 2", "Table 3" };
        }

        [HttpGet("{tableId}")]
        public string ListTableMetadata([FromRoute] string tableId)
        {
            return $"Metadata for table: {tableId}.";
        }

        [HttpGet("{tableId}/data")]
        public string FetchTableData([FromRoute] string tableId)
        {
            return $"Data for table: {tableId}.";
        }

        [HttpGet("{tableId}/filters")]
        public string FetchTableFilters([FromRoute] string tableId)
        {
            return $"filters for table: {tableId}.";
        }

        [HttpGet("{tableId}/filters/{filterId}")]
        public string FetchTableFilter([FromRoute] string tableId, [FromRoute] string filterId)
        {
            return $"Show filter: {filterId} for table: {tableId}.";
        }
    }
}
