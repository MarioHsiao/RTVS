﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Languages.Core.Formatting;
using Microsoft.Languages.Core.Settings;

namespace Microsoft.Languages.Editor.Settings {
    public static class CommonSettings {
        public const string AutoFormatKey = "AutoFormat";
        public const string CompletionEnabledKey = "CompletionEnabled";
        public const string FormatterIndentSizeKey = "FormatterIndentSize";
        public const string FormatterTabSizeKey = "FormatterTabSize";
        public const string FormatterIndentTypeKey = "FormatterIndentType";
        public const string IndentStyleKey = "IndentStyle";
        public const string InsertMatchingBracesKey = "InsertMatchingBraces";
        public const string SignatureHelpEnabledKey = "SignatureHelpEnabled";
        public const string ValidationEnabledKey = "ValidationEnabled";
        public const string ShowInternalFunctionsKey = "ShowInternalFunctions";
        public const string ShowTclFunctionsKey = "ShowTclFunctions";

        public static bool GetAutoFormat(IEditorSettingsStorage storage) {
            return storage.GetBoolean(AutoFormatKey, true);
        }

        public static bool GetCompletionEnabled(IEditorSettingsStorage storage) {
            return storage.GetBoolean(CompletionEnabledKey, true);
        }

        public static int GetFormatterIndentSize(IEditorSettingsStorage storage) {
            return storage.GetInteger(FormatterIndentSizeKey, 4);
        }

        public static IndentType GetFormatterIndentType(IEditorSettingsStorage storage) {
            return (IndentType)storage.GetInteger(FormatterIndentTypeKey, (int)IndentType.Spaces);
        }

        public static int GetFormatterTabSize(IEditorSettingsStorage storage) {
            return storage.GetInteger(FormatterTabSizeKey, 4);
        }

        public static IndentStyle GetIndentStyle(IEditorSettingsStorage storage) {
            return (IndentStyle)storage.GetInteger(IndentStyleKey, (int)IndentStyle.Smart);
        }

        public static bool GetSignatureHelpEnabled(IEditorSettingsStorage storage) {
            return storage.GetBoolean(SignatureHelpEnabledKey, true);
        }

        public static bool GetValidationEnabled(IEditorSettingsStorage storage) {
            return storage.GetBoolean(ValidationEnabledKey, true);
        }
    }
}
