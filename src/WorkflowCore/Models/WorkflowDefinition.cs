using System;
using System.Collections.Generic;


namespace WorkflowCore.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; set; }

        public int Version { get; set; }

        public string Description { get; set; }

        public WorkflowStepCollection Steps { get; set; } = new WorkflowStepCollection();

        public Type DataType { get; set; }

        public WorkflowErrorHandling DefaultErrorBehavior { get; set; }

        public Type OnPostMiddlewareError { get; set; }

        public TimeSpan? DefaultErrorRetryInterval { get; set; }

        private sealed class IdVersionEqualityComparer : IEqualityComparer<WorkflowDefinition>
        {
            public bool Equals(WorkflowDefinition x, WorkflowDefinition y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Id, y.Id, StringComparison.InvariantCultureIgnoreCase) && x.Version == y.Version;
            }

            public int GetHashCode(WorkflowDefinition obj)
            {
                unchecked
                {
                    return (StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Id) * 397) ^ obj.Version;
                }
            }
        }

        public static IEqualityComparer<WorkflowDefinition> IdVersionComparer { get; } = new IdVersionEqualityComparer();
    }

    public enum WorkflowErrorHandling
    {
        Retry = 0,
        Suspend = 1,
        Terminate = 2,
        Compensate = 3
    }
}
