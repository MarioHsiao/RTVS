﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Languages.Core.Settings;
using Microsoft.R.Components.ContentTypes;
using Microsoft.R.Editor.ContentType;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.R.Editor.Test.Settings
{
    [ExcludeFromCodeCoverage]
    [Export(typeof(IWritableSettingsStorage))]
    [ContentType(RContentTypeDefinition.ContentType)]
    [Name("R Test Editor settings")]
    [Order(Before = "Visual Studio R Editor settings")]
    public class TestSettingsStorage : IWritableSettingsStorage
    {
        Dictionary<string, object> _settings = new Dictionary<string, object>();

        public void BeginBatchChange()
        {
        }

        public void EndBatchChange()
        {
        }

        public bool GetBoolean(string name, bool defaultValue)
        {
            if (_settings.ContainsKey(name))
                return (bool)_settings[name];

            return defaultValue;
        }

        public int GetInteger(string name, int defaultValue)
        {
            if (_settings.ContainsKey(name))
                return (int)_settings[name];

            return defaultValue;
        }

        public string GetString(string name, string defaultValue)
        {
            if (_settings.ContainsKey(name))
                return (string)_settings[name];

            return defaultValue;
        }

        public void LoadFromStorage()
        {
        }

        public void ResetSettings()
        {
        }

        public void SetBoolean(string name, bool value)
        {
            _settings[name] = value;
        }

        public void SetInteger(string name, int value)
        {
            _settings[name] = value;
        }

        public void SetString(string name, string value)
        {
            _settings[name] = value;
        }

#pragma warning disable 67
        public event EventHandler<EventArgs> SettingsChanged;
    }
}
