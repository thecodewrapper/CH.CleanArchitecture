using System.Collections.Generic;
using System.Linq;
using CH.CleanArchitecture.Common;
using Newtonsoft.Json;

namespace CH.CleanArchitecture.Presentation.Framework
{
    public class DataTablesParameters
    {
        [JsonProperty("draw")]
        public int Draw { get; set; }
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("columns")]
        public IList<Column> Columns { get; set; }
        [JsonProperty("search")]
        public Search Search { get; set; }
        [JsonProperty("order")]
        public IList<Order> Order { get; set; }

        public QueryOptions ToQueryOptions() {
            var options = new QueryOptions
            {
                PageSize = Length,
                Skip = Start
            };

            if (Order.Any()) {
                var orderBy = Order.First();
                options.OrderBy = Columns[orderBy.Column].Data;
                options.IsAscending = orderBy.Dir == "asc";
            }

            if (!string.IsNullOrWhiteSpace(Search?.Value)) {
                options.SearchTerm = Search.Value;
            }

            return options;
        }
    }

    public class Column
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("searchable")]
        public bool Searchable { get; set; }
        [JsonProperty("orderable")]
        public bool Orderable { get; set; }
        [JsonProperty("search")]
        public Search Search { get; set; }
    }

    public class Search
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("regex")]
        public string Regex { get; set; }
    }

    public class Order
    {
        [JsonProperty("column")]
        public int Column { get; set; }
        [JsonProperty("dir")]
        public string Dir { get; set; }
    }
}
