// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Common.Core.Extensions;
using Microsoft.Common.Core.Shell;
using Microsoft.Common.Core.Test.Fixtures;
using Microsoft.Common.Core.Test.StubBuilders;
using Microsoft.Languages.Editor.Shell;
using Microsoft.Languages.Editor.Test.Fakes.Shell;
using Microsoft.UnitTests.Core.Mef;
using Microsoft.UnitTests.Core.XUnit;
using Xunit.Sdk;

namespace Microsoft.Languages.Editor.Test {
    // Fixture doesn't import itself. Use AssemblyFixtureImportAttribute
    [ExcludeFromCodeCoverage]
    public class LanguagesEditorMefCatalogFixture : AssemblyMefCatalogFixture {
        public virtual IExportProvider Create(CoreServicesFixture coreServices) => new LanguagesEditorTestExportProvider(CreateContainer(), coreServices);

        protected class LanguagesEditorTestExportProvider : TestExportProvider {
            private readonly CoreServicesFixture _coreServices;
            public LanguagesEditorTestExportProvider(CompositionContainer compositionContainer, CoreServicesFixture coreServices) : base(compositionContainer) {
                _coreServices = coreServices;
            }

            public override Task<Task<RunSummary>> InitializeAsync(ITestInput testInput, IMessageBus messageBus) {
                var editorShell = new TestEditorShell(CompositionContainer, _coreServices);
                var batch = new CompositionBatch()
                    .AddValue(FileSystemStubFactory.CreateDefault())
                    .AddValue<ICoreShell>(editorShell)
                    .AddValue<IEditorShell>(editorShell)
                    .AddValue(editorShell);
                CompositionContainer.Compose(batch);
                return base.InitializeAsync(testInput, messageBus);
            }
        }
    }
}
