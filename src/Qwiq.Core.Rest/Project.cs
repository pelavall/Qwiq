using System;
using System.Collections.Generic;
(??)using System.Linq;
using JetBrains.Annotations;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class Project : Qwiq.Project
    {
        //The VSTS Rest api for shared queries only allows for expanding folder structure 2 deep, https://www.visualstudio.com/en-us/docs/integrate/api/wit/queries
        private const int MaxQueryFolderExpansionDepth = 2;

        internal Project([NotNull] TeamProjectReference project, [NotNull] WorkItemStore store)
            : base(
                project.Id,
                project.Name,
                new Uri(project.Url),
                new Lazy<IWorkItemTypeCollection>(
                    () =>
                        {
                            var wits = store.NativeWorkItemStore
                                            .Value
                                            .GetWorkItemTypesAsync(project.Name)
                                            .GetAwaiter()
                                            .GetResult();

                            var wits2 = new List<IWorkItemType>(wits.Count);
                            for (var i = 0; i < wits.Count; i++)
                            {
                                var wit = wits[i];
                                wits2.Add(new WorkItemType(wit));
                            }

                            return new WorkItemTypeCollection(wits2);
                        }),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(() => WorkItemClassificationNodeCollectionBuilder.BuildAsync(store.NativeWorkItemStore.Value.GetClassificationNodeAsync(project.Name, TreeStructureGroup.Areas, null, int.MaxValue)).GetAwaiter().GetResult()),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(() => WorkItemClassificationNodeCollectionBuilder.BuildAsync(store.NativeWorkItemStore.Value.GetClassificationNodeAsync(project.Name, TreeStructureGroup.Iterations, null, int.MaxValue)).GetAwaiter().GetResult())
                )
        {
        }
    }
}
