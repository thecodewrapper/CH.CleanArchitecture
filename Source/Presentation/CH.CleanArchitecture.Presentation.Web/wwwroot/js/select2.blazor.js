if (!window.select2Blazor) {
    window.select2Blazor = {};
}
window.select2Blazor = {
    init: function (id, dotnetHelper, options, getData, parentId, searchingText = "Searching", noResultsText = "No results found") {
        options = JSON.parse(options);
        if (getData) {
            options.ajax = {
                delay: 250,
                data: function (params) {
                    return {
                        type: params._type,
                        term: params.term,
                        page: params.page || 1,
                        size: params.size || 10
                    };
                },
                transport: function (params, success, failure) {
                    var request = dotnetHelper.invokeMethodAsync(getData, params);
                    return request.then(success).catch(failure);
                },
                processResults: data => JSON.parse(data)
            };
        }
        options.escapeMarkup = function (markup) {
            return markup;
        };
        options.templateResult = function (data, container) {
            return data.html || data.text;
        };
        options.templateSelection = function (data, container) {
            return data.text;
        };

        options.language = {
            searching: function () {
                return searchingText;
            },
            noResults: function () {
                return noResultsText;
            }
        };

        if (parentId) {
            options.dropdownParent = $('#'.concat(parentId));

            //var parent = $('#'.concat(parentId));
            //parent.on('scroll', function (event) {
            //    parent.find(".select2").each(function () {
            //        parent.select2({ dropdownParent: $(this).parent() });
            //    });
            //});
        }

        $('#' + id).select2(options);


    },
    select: function (id, value) {
        if (value) {
            var $select2 = $('#' + id);
            var selection = $('#' + id).select2('data');

            var isAlreadySelected = selection.filter(x => x.id === value.id).length > 0;
            if (isAlreadySelected) {
                return;
            }

            // Set the value, creating a new option if necessary
            if ($select2.find(`option[value='${value.id}']`).length) {
                $select2.val(value.id).trigger('change');
            } else {
                // Create a DOM Option and pre-select by default
                var newOption = new Option(value.text, value.id, true, true);
                // Append it to the select
                $select2.append(newOption).trigger('change');
            }
        }
    },
    onChange: function (id, dotnetHelper, nameFunc) {
        $('#' + id).on('change.select2', function (e) {
            dotnetHelper.invokeMethodAsync(nameFunc, $('#' + id).val());
        });
    },
    resetSelection: function (id) {
        $('#' + id).val(null).trigger("change")
    }
};