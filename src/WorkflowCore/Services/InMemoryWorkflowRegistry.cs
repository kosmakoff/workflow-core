using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services
{
    public class InMemoryWorkflowRegistry : IWorkflowRegistry
    {
        private readonly IWorkflowBuilder _workflowBuilder;
        private readonly ConcurrentHashSet<WorkflowDefinition> _registry = new ConcurrentHashSet<WorkflowDefinition>(WorkflowDefinition.IdVersionComparer);

        public InMemoryWorkflowRegistry(IWorkflowBuilder workflowBuilder)
        {
            _workflowBuilder = workflowBuilder;
        }

        public Task<WorkflowDefinition> GetDefinition(string workflowId, int? version = null, CancellationToken cancellationToken = default)
        {
            if (version.HasValue)
            {
                var workflowDefinition = _registry.FirstOrDefault(x => x.Id == workflowId && x.Version == version.Value);
                return Task.FromResult(workflowDefinition);
            }
            else
            {
                var workflowDefinition = _registry.Where(x => x.Id == workflowId)
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault();
                return Task.FromResult(workflowDefinition);
            }
        }

        public Task DeregisterWorkflow(string workflowId, int version, CancellationToken cancellationToken)
        {
            var definition = _registry.FirstOrDefault(x => x.Id == workflowId && x.Version == version);
            if (definition != null)
            {
                _registry.TryRemove(definition);
            }

            return Task.CompletedTask;
        }

        public Task RegisterWorkflow(IWorkflow workflow, CancellationToken cancellationToken)
        {
            if (_registry.Any(x => x.Id == workflow.Id && x.Version == workflow.Version))
            {
                throw new InvalidOperationException($"Workflow {workflow.Id} version {workflow.Version} is already registered");
            }

            var builder = _workflowBuilder.UseData<object>();
            workflow.Build(builder);
            var workflowDefinition = builder.Build(workflow.Id, workflow.Version);
            _registry.Add(workflowDefinition);

            return Task.CompletedTask;
        }

        public Task RegisterWorkflow(WorkflowDefinition definition, CancellationToken cancellationToken)
        {
            if (_registry.Any(x => x.Id == definition.Id && x.Version == definition.Version))
            {
                throw new InvalidOperationException($"Workflow {definition.Id} version {definition.Version} is already registered");
            }

            _registry.Add(definition);

            return Task.CompletedTask;
        }

        public Task RegisterWorkflow<TData>(IWorkflow<TData> workflow, CancellationToken cancellationToken)
            where TData : new()
        {
            if (_registry.Any(x => x.Id == workflow.Id && x.Version == workflow.Version))
            {
                throw new InvalidOperationException($"Workflow {workflow.Id} version {workflow.Version} is already registered");
            }

            var builder = _workflowBuilder.UseData<TData>();
            workflow.Build(builder);
            var workflowDefinition = builder.Build(workflow.Id, workflow.Version);
            _registry.Add(workflowDefinition);

            return Task.CompletedTask;
        }

        public Task<bool> IsRegistered(string workflowId, int version, CancellationToken cancellationToken)
        {
            var definition = _registry.FirstOrDefault(x => x.Id == workflowId && x.Version == version);
            return Task.FromResult(definition != null);
        }

        public Task<IEnumerable<WorkflowDefinition>> GetAllDefinitions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_registry.AsEnumerable());
        }
    }
}
