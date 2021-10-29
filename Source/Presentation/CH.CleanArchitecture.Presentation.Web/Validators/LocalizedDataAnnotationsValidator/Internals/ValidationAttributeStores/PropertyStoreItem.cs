// This source code is fork from https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/ValidationAttributeStore.cs
// The .NET Foundation licenses the original file of this forked file to you under the MIT license.
// See the LICENSE file for the original file of this forked file: https://github.com/dotnet/corefx/blob/master/LICENSE.TXT

using System;
using System.Collections.Generic;

namespace CH.CleanArchitecture.Presentation.Web.Validators.Internals
{
    internal class PropertyStoreItem : StoreItem
    {
        internal Type PropertyType { get; }

        internal PropertyStoreItem(Type propertyType, IEnumerable<Attribute> attributes) : base(attributes) {
            PropertyType = propertyType;
        }
    }
}
