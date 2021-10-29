namespace NEvaldas.Blazor.Select2.Models
{
    internal class Select2Item
    {
        public Select2Item(string id, string text, bool disabled) {
            Id = id;
            Text = text;
            Disabled = disabled;
        }

        public string Id { get; }
        public bool Disabled { get; }
        public bool Selected { get; set; }
        public string Text { get; }
        public string Html { get; set; }
    }
}
