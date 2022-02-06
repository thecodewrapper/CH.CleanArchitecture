using System.Collections.Generic;
using AutoMapper;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CH.CleanArchitecture.Presentation.Web.Mappings
{
    public class RolesToMultiSelectResolver<T> : IValueResolver<UserReadModel, T, IEnumerable<SelectListItem>>
    {
        private readonly IHtmlHelper _htmlHelper;
        public RolesToMultiSelectResolver(IHtmlHelper htmlHelper) {
            _htmlHelper = htmlHelper;
        }

        public IEnumerable<SelectListItem> Resolve(UserReadModel source, T destination, IEnumerable<SelectListItem> destinationMember, ResolutionContext context) {
            var result = _htmlHelper.GetEnumSelectList<RoleEnum>();
            //foreach (SelectListItem role in result)
            //{
            //    if (source.Roles.Contains(role.Text))
            //        role.Selected = true;
            //}
            return result;
        }
    }
}
