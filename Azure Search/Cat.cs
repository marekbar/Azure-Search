namespace Azure_Search
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;

    [SerializePropertyNamesAsCamelCase]
    public class Cat
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsFilterable, IsSortable, IsFacetable, IsSearchable]
        public string Name { get; set; }

        [IsSearchable]
        public string Description { get; set; }

        public override string ToString()
        {
            return new StringBuilder().Append(nameof(Id)).Append(": ").Append(Id).AppendLine().Append(nameof(Name))
                .Append(": ").Append(Name).AppendLine().Append(nameof(Description)).Append(": ").Append(Description).ToString();
        }
    }
}