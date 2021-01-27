using FakeItEasy;
using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Services;
using FluentAssertions;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace WorkflowCore.UnitTests.Services
{
    public class InMemoryWorkflowRegistryFixture
    {
        protected IWorkflowBuilder WorkflowBuilder { get; }
        protected InMemoryWorkflowRegistry Subject { get; }
        protected WorkflowDefinition Definition { get; }

        public InMemoryWorkflowRegistryFixture()
        {
            WorkflowBuilder = A.Fake<IWorkflowBuilder>();
            Subject = new InMemoryWorkflowRegistry(WorkflowBuilder);

            Definition = new WorkflowDefinition{
                Id = "TestWorkflow",
                Version = 1,
            };
            Subject.RegisterWorkflow(Definition, CancellationToken.None).GetAwaiter().GetResult();
        }

        [Fact(DisplayName = "Should return existing workflow")]
        public async Task getdefinition_should_return_existing_workflow()
        {
            (await Subject.GetDefinition(Definition.Id)).Should().Be(Definition);
            (await Subject.GetDefinition(Definition.Id, Definition.Version)).Should().Be(Definition);
        }

        [Fact(DisplayName = "Should return null on unknown workflow")]
        public async Task getdefinition_should_return_null_on_unknown()
        {
            (await Subject.GetDefinition("UnkownWorkflow")).Should().BeNull();
            (await Subject.GetDefinition("UnkownWorkflow", 1)).Should().BeNull();
        }

        [Fact(DisplayName = "Should return highest version of existing workflow")]
        public async Task getdefinition_should_return_highest_version_workflow()
        {
            var definition2 = new WorkflowDefinition{
                Id = Definition.Id,
                Version = Definition.Version + 1,
            };
            await Subject.RegisterWorkflow(definition2, CancellationToken.None);

            (await Subject.GetDefinition(Definition.Id)).Should().Be(definition2);
            (await Subject.GetDefinition(Definition.Id, definition2.Version)).Should().Be(definition2);
            (await Subject.GetDefinition(Definition.Id, Definition.Version)).Should().Be(Definition);
        }
    }
}