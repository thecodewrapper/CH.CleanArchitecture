namespace NEvaldas.Blazor.Select2.Models
{
    internal class Select2Pagination
    {
        public Select2Pagination(bool more) => More = more;

        public bool More { get; set; }
    }
}
