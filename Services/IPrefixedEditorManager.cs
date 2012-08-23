using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;

namespace Onestop.Seo.Services {
    public interface IPrefixedEditorManager : IDependency {
        IEnumerable<int> ItemIds { get; }
        dynamic BuildShape(IContent content, Func<IContent, dynamic> shapeFactory);
        dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "");
    }
}
