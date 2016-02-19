﻿using System;
using Microsoft.Languages.Editor.Controller.Command;
using Microsoft.R.Components.Controller;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.VisualStudio.R.Package.Commands;
using Microsoft.VisualStudio.R.Packages.R;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.R.Package.Repl.Commands {
    class ExecuteCurrentCodeCommand : RExecuteCommand {

        public ExecuteCurrentCodeCommand(ITextView textView, IRInteractiveWorkflow interactiveWorkflow) :
            base(textView, interactiveWorkflow, new CommandId(RGuidList.RCmdSetGuid, RPackageCommandId.icmdRexecuteReplCmd)) {
        }

        public override CommandResult Invoke(Guid group, int id, object inputArg, ref object outputArg) {
            var window = InteractiveWorkflow.ActiveWindow;
            if (window == null) {
                return CommandResult.Disabled;
            }

            
            var text = GetText(window);

            if (text != null) {
                InteractiveWorkflow.Operations.EnqueueExpression(text, false);
            }

            return CommandResult.Executed;
        }
    }
}
