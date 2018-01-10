using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemClassificationNode : IEquatable<IWorkItemClassificationNode>, IIdentifiable<int>
    {
        bool IsAreaNode { get; }
        bool IsIterationNode { get; }
        string Name { get; }
        Uri Uri { get; }
        string Path { get; }
    }
}