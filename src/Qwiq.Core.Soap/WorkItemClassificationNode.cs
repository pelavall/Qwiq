using System;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class WorkItemClassificationNode : Qwiq.WorkItemClassificationNode
    {
        internal WorkItemClassificationNode(
            int id,
            WorkItemClassificationNodeType type,
            string name,
            Uri uri,
            string path
            )
        : base(id, type, name, uri)
        {
            Path = path;
        }

        internal WorkItemClassificationNode(
            int id,
            bool isAreaNode,
            bool isIterationNode,
            string name,
            Uri uri,
            string path)
            : base(id, isAreaNode, isIterationNode, name, uri)
        {
            Path = path;
        }

        public override string Path { get; }
    }
}