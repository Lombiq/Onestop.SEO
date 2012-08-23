using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Onestop.Seo.Services {
    public class PrefixedEditorManager : IPrefixedEditorManager {
        private readonly IContentManager _contentManager;

        public HashSet<int> ItemIds { get; private set; }

        public PrefixedEditorManager(IContentManager contentManager) {
            _contentManager = contentManager;
            ItemIds = new HashSet<int>();
        }

        public dynamic BuildShape(IContent content, Func<IContent, dynamic> shapeFactory) {
            ItemIds.Add(content.ContentItem.Id);
            return shapeFactory(content);
        }

        public dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "") {
            return _contentManager.UpdateEditor(content, new PrefixedUpdater(updater, content.ContentItem.Id), groupId);
        }

        public static string AttachPrefixToPrefix(int itemId, string currentPrefix) {
            return itemId + "_" + currentPrefix;
        }

        private class PrefixedUpdater : IUpdateModel {
            private readonly IUpdateModel _updater;
            private readonly int _itemId;

            public PrefixedUpdater(IUpdateModel updater, int itemId) {
                _updater = updater;
                _itemId = itemId;
            }

            bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
                return _updater.TryUpdateModel<TModel>(model, AttachPrefixToPrefix(_itemId, prefix), includeProperties, excludeProperties);
            }

            void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
                _updater.AddModelError(key, errorMessage);
            }
        }
    }
}