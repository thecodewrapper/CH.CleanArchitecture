using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public partial class TabGroup : ComponentBase
    {
        #region Parameters

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Animate { get; set; } = true;

        #endregion

        #region Private Properties

        public List<TabPage> Pages { get; set; } = new List<TabPage>();

        public TabPage ActivePage { get; set; }

        private string _componentId = Guid.NewGuid().ToString("n");

        #endregion

        #region Internal Methods

        internal void AddPage(TabPage page) {
            Pages.Add(page);
            if (Pages.Count == 1) {
                ActivePage = page;
            }
            StateHasChanged();
        }

        private string GetButtonClass(TabPage tab) {
            var classes = new List<string>();
            classes.Add("tabItem");

            classes.Add(tab == ActivePage ? "active" : "inactive");
            classes.Add(Animate ? "animate" : "");

            return string.Join(' ', classes.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        internal void ActivatePage(TabPage page) {
            ActivePage = page;
        }

        internal string GetTabCssClass(TabPage page) {
            return page == ActivePage ? "active" : string.Empty;
        }

        #endregion
    }
}
