﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.R.Host.Client {
    public class RHostStartupInfo {
        public RHostStartupInfo(string cranMirrorName = null
            , string workingDirectory = null
            , int codePage = 0
            , int terminalWidth = 80
            , bool enableAutosave = false
            , bool useRHostCommandLineArguments = false) {

            CranMirrorName = cranMirrorName;
            WorkingDirectory = workingDirectory;
            CodePage = codePage;
            TerminalWidth = terminalWidth;
            EnableAutosave = enableAutosave;
            UseRHostCommandLineArguments = useRHostCommandLineArguments;
        }

        public string CranMirrorName { get; }
        public string WorkingDirectory { get; }
        public int CodePage { get; }
        public int TerminalWidth { get; }
        public bool EnableAutosave { get; }
        public bool UseRHostCommandLineArguments { get; }
    }
}
