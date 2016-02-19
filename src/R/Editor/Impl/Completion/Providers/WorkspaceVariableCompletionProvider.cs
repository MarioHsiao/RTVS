﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.Languages.Editor.Imaging;
using Microsoft.Languages.Editor.Shell;
using Microsoft.R.Editor.Completion.Definitions;
using Microsoft.R.Support.Help.Definitions;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.R.Editor.Completion.Providers {
    /// <summary>
    /// Provides list of installed packages for completion inside 
    /// library(...) statement. List of packages is  obtained from 
    /// ~\Program Files\R and from ~\Documents\R folders
    /// </summary>
    public sealed class WorkspaceVariableCompletionProvider : IRCompletionListProvider {
        [Import]
        private IVariablesProvider VariablesProvider { get; set; }

        public WorkspaceVariableCompletionProvider() {
            EditorShell.Current.CompositionService.SatisfyImportsOnce(this);
        }

        #region IRCompletionListProvider
        public bool AllowSorting { get; } = true;

        public IReadOnlyCollection<RCompletion> GetEntries(RCompletionContext context) {
            List<RCompletion> completions = new List<RCompletion>();
            ImageSource functionGlyph = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic);
            ImageSource variableGlyph = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);

            string variableName = RCompletionContext.GetVariableName(context.Session.TextView, context.TextBuffer.CurrentSnapshot);
            if (variableName.IndexOfAny(new char[] { '$', '@' }) < 0) {
                variableName = string.Empty;
            }

            VariablesProvider.Initialize();
            int memberCount = VariablesProvider.GetMemberCount(variableName);
            IReadOnlyCollection<INamedItemInfo> members = VariablesProvider.GetMembers(variableName, 200);

            // Get list of functions in the package
            foreach (INamedItemInfo v in members) {
                Debug.Assert(v != null);

                if (v.Name.Length > 0 && v.Name[0] != '[') {
                    ImageSource glyph = v.ItemType == NamedItemType.Variable ? variableGlyph : functionGlyph;
                    var completion = new RCompletion(v.Name, CompletionUtilities.BacktickName(v.Name), v.Description, glyph);
                    completions.Add(completion);
                }
            }

            return completions;
        }
        #endregion
    }
}
