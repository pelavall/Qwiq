using System;
using System.Diagnostics;

namespace Microsoft.Qwiq
{
    public class WorkItemClassificationNode : IWorkItemClassificationNode
    {
        private readonly Lazy<string> _path;

        public WorkItemClassificationNode(
            int id,
            WorkItemClassificationNodeType type,
            string name,
            Uri uri,
            Lazy<INode<IWorkItemClassificationNode, int>> lazyParent = null
            )
        : this(id, type == WorkItemClassificationNodeType.Area, type == WorkItemClassificationNodeType.Iteration, name, uri, lazyParent)
        {
        }

        public WorkItemClassificationNode(
            int id,
            bool isAreaNode,
            bool isIterationNode,
            string name,
            Uri uri,
            Lazy<INode<IWorkItemClassificationNode, int>> lazyParent = null)
        {
            Id = id;
            IsAreaNode = isAreaNode;
            IsIterationNode = isIterationNode;
            Uri = uri;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _path = new Lazy<string>(() => ((lazyParent?.Value?.Value?.Path ?? string.Empty) + "\\" + Name).Trim('\\'));
        }

        public int Id { get; }
        public bool IsAreaNode { get; }
        public bool IsIterationNode { get; }
        public string Name { get; }
        public Uri Uri { get; }
        public virtual string Path => _path.Value;

        [DebuggerStepThrough]
        public bool Equals(IWorkItemClassificationNode other)
        {
            return AreaOrIterationComparer.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return AreaOrIterationComparer.Default.Equals(this, obj as IWorkItemClassificationNode);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return AreaOrIterationComparer.Default.GetHashCode(this);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return Path;
        }
    }
}