using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NEvaldas.Blazor.Select2.Models;

namespace NEvaldas.Blazor.Select2
{
    public class Select2Base<TItem> : ComponentBase, IDisposable
    {
        private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
        private readonly JsonSerializerOptions _jsonSerializerOptions =
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        [Inject] private IJSRuntime JSRuntime { get; set; }
        private DotNetObjectReference<Select2Base<TItem>> _elementRef;
        private bool _previousParsingAttemptFailed;
        private ValidationMessageStore _parsingValidationMessages;
        private Type _nullableUnderlyingType;

        [CascadingParameter] public EditContext CascadingEditContext { get; set; }

        [Parameter] public EditContext EditContext { get; set; }

        [Parameter] public string Id { get; set; }
        [Parameter] public string ParentId { get; set; }
        [Parameter] public string SearchingText { get; set; }
        [Parameter] public string NoResultsText { get; set; }

        [Parameter] public bool IsDisabled { get; set; }

        [Parameter] public Func<TItem, bool> IsOptionDisabled { get; set; } = item => false;

        [Parameter] public List<TItem> Data { get; set; }

        [Parameter] public Func<Select2QueryData, Task<List<TItem>>> GetPagedData { get; set; }

        [Parameter] public Func<TItem, string> OptionTemplate { get; set; }

        [Parameter] public Func<TItem, string> TextExpression { get; set; } = item => item.ToString();

        [Parameter] public string Placeholder { get; set; } = "Select value";

        [Parameter] public string Theme { get; set; } = "bootstrap";

        [Parameter] public bool AllowClear { get; set; }
        /// <summary>
        /// Gets or sets an expression that identifies the bound value.
        /// </summary>
        [Parameter] public Expression<Func<TItem>> ValueExpression { get; set; }

        /// <summary>
        /// Gets or sets the value of the input. This should be used with two-way binding.
        /// </summary>
        /// <example>
        /// @bind-Value="model.PropertyName"
        /// </example>
        [Parameter]
        public TItem Value { get; set; }

        /// <summary>
        /// Gets or sets a callback that updates the bound value.
        /// </summary>
        [Parameter] public EventCallback<TItem> ValueChanged { get; set; }

        public void Refresh() {
            StateHasChanged();
        }

        public async Task ResetSelection() {
            await JSRuntime.InvokeVoidAsync("select2Blazor.resetSelection", Id);
        }

        /// <summary>
        /// Constructs an instance of <see cref="Select2Base{TItem}"/>.
        /// </summary>
        protected Select2Base() {
            _validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
        }

        protected Dictionary<string, TItem> InternallyMappedData { get; set; } = new Dictionary<string, TItem>();

        protected string FieldClass => GivenEditContext?.FieldCssClass(FieldIdentifier) ?? string.Empty;

        protected EditContext GivenEditContext { get; set; }

        /// <summary>
        /// Gets the <see cref="FieldIdentifier"/> for the bound value.
        /// </summary>
        protected FieldIdentifier FieldIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the current value of the input.
        /// </summary>
        protected TItem CurrentValue
        {
            get => Value;
            set
            {
                if (value is null) return;
                _ = SelectItem(value);

                var hasChanged = !EqualityComparer<TItem>.Default.Equals(value, Value);
                if (!hasChanged) return;

                Value = value;
                _ = ValueChanged.InvokeAsync(value);
                GivenEditContext?.NotifyFieldChanged(FieldIdentifier);
            }
        }

        protected bool TryParseValueFromString(string value, out TItem result) {
            result = default;

            if (value == "null" || string.IsNullOrEmpty(value))
                return AllowClear;

            if (!InternallyMappedData.ContainsKey(value))
                return false;

            result = InternallyMappedData[value];
            return true;
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            _elementRef = DotNetObjectReference.Create(this);
        }

        public override Task SetParametersAsync(ParameterView parameters) {
            parameters.SetParameterProperties(this);

            FieldIdentifier = FieldIdentifier.Create(ValueExpression);
            _nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TItem));
            GivenEditContext = EditContext ?? CascadingEditContext;
            if (GivenEditContext != null)
                GivenEditContext.OnValidationStateChanged += _validationStateChangedHandler;

            GetPagedData ??= GetStaticData;

            //CurrentValue = ValueExpression.Compile().Invoke();

            // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender) {
                var options = JsonSerializer.Serialize(new
                {
                    placeholder = Placeholder,
                    allowClear = AllowClear,
                    theme = Theme
                }, _jsonSerializerOptions);

                await JSRuntime.InvokeVoidAsync("select2Blazor.init",
                    Id, _elementRef, options, "select2Blazor_GetData", ParentId, SearchingText, NoResultsText);

                if (CurrentValue != null)
                    await SelectItem(CurrentValue);

                await JSRuntime.InvokeVoidAsync("select2Blazor.onChange",
                    Id, _elementRef, "select2Blazor_OnChange");
            }
        }

        private Task<List<TItem>> GetStaticData(Select2QueryData query) {
            if (query.Page != 1)
                return Task.FromResult(default(List<TItem>));

            var data = Data;
            var searchTerm = query.Term;
            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                data = data
                    .Where(x => TextExpression(x).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return Task.FromResult(data);
        }

        private async Task SelectItem(TItem item) {
            var mappedItem = MapToSelect2Item(item);
            InternallyMappedData[mappedItem.Id] = item;
            await JSRuntime.InvokeVoidAsync("select2Blazor.select", Id, mappedItem);
        }

        internal Select2Item MapToSelect2Item(TItem item) {
            var id = GetId(item);
            var select2Item = new Select2Item(id, TextExpression(item), IsOptionDisabled(item));
            if (OptionTemplate != null)
                select2Item.Html = OptionTemplate(item);
            if (Value != null)
                select2Item.Selected = GetId(Value) == id;
            return select2Item;
        }

        [JSInvokable("select2Blazor_GetData")]
        public async Task<string> Select2_GetDataWrapper(JsonElement element) {
            var json = element.GetRawText();
            var queryParams = JsonSerializer.Deserialize<Select2QueryParams>(json, _jsonSerializerOptions);

            var data = await GetPagedData(queryParams.Data);

            if (!queryParams.Data.Type.Contains("append", StringComparison.OrdinalIgnoreCase))
                InternallyMappedData.Clear();

            var response = new Select2Response();
            if (data != null) {
                foreach (var item in data) {
                    var mappedItem = MapToSelect2Item(item);
                    InternallyMappedData[mappedItem.Id] = item;
                    response.Results.Add(mappedItem);
                }
                response.Pagination.More = data.Count == queryParams.Data.Size;
            }

            return JsonSerializer.Serialize(response, _jsonSerializerOptions);
        }

        [JSInvokable("select2Blazor_OnChange")]
        public void Change(string value) {
            if (string.IsNullOrWhiteSpace(value)) return;

            _parsingValidationMessages?.Clear();

            bool parsingFailed;

            if (_nullableUnderlyingType != null && string.IsNullOrEmpty(value)) {
                // Assume if it's a nullable type, null/empty inputs should correspond to default(T)
                // Then all subclasses get nullable support almost automatically (they just have to
                // not reject Nullable<T> based on the type itself).
                parsingFailed = false;
                CurrentValue = default;
            }
            else if (TryParseValueFromString(value, out var parsedValue)) {
                parsingFailed = false;
                CurrentValue = parsedValue;
            }
            else {
                parsingFailed = true;

                if (_parsingValidationMessages == null) {
                    _parsingValidationMessages = new ValidationMessageStore(GivenEditContext);
                }

                _parsingValidationMessages.Add(FieldIdentifier, "Given value was not found");

                // Since we're not writing to CurrentValue, we'll need to notify about modification from here
                GivenEditContext?.NotifyFieldChanged(FieldIdentifier);
            }

            // We can skip the validation notification if we were previously valid and still are
            if (parsingFailed || _previousParsingAttemptFailed) {
                GivenEditContext?.NotifyValidationStateChanged();
                _previousParsingAttemptFailed = parsingFailed;
            }
        }

        private static string GetId(TItem item) => item.GetHashCode().ToString();

        protected virtual void Dispose(bool disposing) {
        }

        void IDisposable.Dispose() {
            if (GivenEditContext != null) {
                GivenEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
            }

            Dispose(disposing: true);
        }
    }
}
