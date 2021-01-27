using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    public interface IWorkflowRegistry
    {
        Task RegisterWorkflow(IWorkflow workflow, CancellationToken cancellationToken = default);
        Task RegisterWorkflow(WorkflowDefinition definition, CancellationToken cancellationToken = default);
        Task RegisterWorkflow<TData>(IWorkflow<TData> workflow, CancellationToken cancellationToken = default) where TData : new();
        Task<WorkflowDefinition> GetDefinition(string workflowId, int? version = null, CancellationToken cancellationToken = default);
        Task<bool> IsRegistered(string workflowId, int version, CancellationToken cancellationToken = default);
        Task DeregisterWorkflow(string workflowId, int version, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowDefinition>> GetAllDefinitions(CancellationToken cancellationToken = default);
    }
}
