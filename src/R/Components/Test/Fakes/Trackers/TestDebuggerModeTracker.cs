using System.ComponentModel.Composition;
using Microsoft.R.Components.InteractiveWorkflow;

namespace Microsoft.R.Components.Test.Fakes.Trackers {
    [Export(typeof(IDebuggerModeTracker))]
    [Export(typeof(TestDebuggerModeTracker))]
    public sealed class TestDebuggerModeTracker : IDebuggerModeTracker {
        public bool IsEnteredBreakMode { get; set; }
    }
}