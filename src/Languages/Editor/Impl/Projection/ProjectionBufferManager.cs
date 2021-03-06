// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information

using System;
using System.Collections.Generic;
using Microsoft.Common.Core.Shell;
using Microsoft.Languages.Editor.EditorFactory;
using Microsoft.Languages.Editor.Extensions;
using Microsoft.Languages.Editor.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.Languages.Editor.Projection {
    /// <summary>
    /// Manages the projection buffer for the primary language
    /// </summary>
    public sealed class ProjectionBufferManager : IProjectionBufferManager {
        private const string _inertContentTypeName = "inert";

        private readonly IContentTypeRegistryService _contentTypeRegistryService;
        private int? _savedCaretPosition;

        public ProjectionBufferManager(ITextBuffer diskBuffer,
                                       IProjectionBufferFactoryService projectionBufferFactoryService,
                                       IContentTypeRegistryService contentTypeRegistryService,
                                       ICoreShell coreShell,
                                       string topLevelContentTypeName,
                                       string secondaryContentTypeName) {
            DiskBuffer = diskBuffer;

            _contentTypeRegistryService = contentTypeRegistryService;

            var contentType = _contentTypeRegistryService.GetContentType(topLevelContentTypeName);
            ViewBuffer = projectionBufferFactoryService.CreateProjectionBuffer(null, new List<object>(0), ProjectionBufferOptions.None, contentType);

            contentType = _contentTypeRegistryService.GetContentType(secondaryContentTypeName);
            ContainedLanguageBuffer = projectionBufferFactoryService.CreateProjectionBuffer(null, new List<object>(0), ProjectionBufferOptions.WritableLiteralSpans, contentType);

            ServiceManager.AddService<IProjectionBufferManager>(this, DiskBuffer, coreShell);
            ServiceManager.AddService<IProjectionBufferManager>(this, ViewBuffer, coreShell);
        }

        public static IProjectionBufferManager FromTextBuffer(ITextBuffer textBuffer) {
            return ServiceManager.GetService<IProjectionBufferManager>(textBuffer);
        }

        #region IProjectionBufferManager
        //  Graph:
        //      View Buffer [ContentType = RMD Projection]
        //        |      \
        //        |    Secondary [ContentType = R]
        //        |      /
        //       Disk Buffer [ContentType = RMD]

        public IProjectionBuffer ViewBuffer { get; }
        public IProjectionBuffer ContainedLanguageBuffer { get; }
        public ITextBuffer DiskBuffer { get; }

        public event EventHandler MappingsChanged;

        public void SetProjectionMappings(string secondaryContent, IReadOnlyList<ProjectionMapping> mappings) {
            // Changing projections can move caret to a visible area unexpectedly.
            // Save caret position so we can place it at the same location when 
            // projections are re-established.
            SaveCaretPosition();

            // While we are changing mappings map everything to the view
            mappings = mappings ?? new List<ProjectionMapping>();
            MapEverythingToView();

            // Now update language spans
            var secondarySpans = CreateSecondarySpans(secondaryContent, mappings);
            ContainedLanguageBuffer.ReplaceSpans(0, ContainedLanguageBuffer.CurrentSnapshot.SpanCount, secondarySpans, EditOptions.DefaultMinimalChange, this);
            if (secondarySpans.Count > 0) {
                // Update primary (view) buffer projected spans. View buffer spans are all tracking spans:
                // they either come from primary content or secondary content. Inert spans do not participate.
                var viewSpans = CreateViewSpans(mappings);
                if (viewSpans.Count > 0) {
                    ViewBuffer.ReplaceSpans(0, ViewBuffer.CurrentSnapshot.SpanCount, viewSpans, EditOptions.DefaultMinimalChange, this);
                }
            }

            RestoreCaretPosition();
            MappingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void MapEverythingToView() {
            ITextSnapshot diskSnap = DiskBuffer.CurrentSnapshot;
            SnapshotSpan everything = new SnapshotSpan(diskSnap, 0, diskSnap.Length);
            ITrackingSpan trackingSpan = diskSnap.CreateTrackingSpan(everything, SpanTrackingMode.EdgeInclusive);
            ViewBuffer.ReplaceSpans(0, ViewBuffer.CurrentSnapshot.SpanCount, new List<object>() { trackingSpan }, EditOptions.None, this);
        }

        public void Dispose() {
            ServiceManager.RemoveService<IProjectionBufferManager>(DiskBuffer);
            ServiceManager.RemoveService<IProjectionBufferManager>(ViewBuffer);
        }
        #endregion

        private List<object> CreateSecondarySpans(string secondaryText, IReadOnlyList<ProjectionMapping> mappings) {
            var spans = new List<object>(mappings.Count);

            int secondaryIndex = 0;
            Span span;

            for (int i = 0; i < mappings.Count; i++) {
                ProjectionMapping mapping = mappings[i];
                if (mapping.Length > 0) {
                    span = Span.FromBounds(secondaryIndex, mapping.ProjectionRange.Start);
                    if (!span.IsEmpty) {
                        spans.Add(secondaryText.Substring(span.Start, span.Length)); // inert
                    }
                    span = new Span(mapping.SourceStart, mapping.Length);
                    // Active span comes from the disk buffer
                    spans.Add(new CustomTrackingSpan(DiskBuffer.CurrentSnapshot, span, PointTrackingMode.Positive, PointTrackingMode.Positive)); // active
                    secondaryIndex = mapping.ProjectionRange.End;
                }
            }

            // Add the final inert text after the last span
            span = Span.FromBounds(secondaryIndex, secondaryText.Length);
            if (!span.IsEmpty) {
                spans.Add(secondaryText.Substring(span.Start, span.Length)); // inert
            }
            return spans;
        }

        private List<object> CreateViewSpans(IReadOnlyList<ProjectionMapping> mappings) {
            var spans = new List<object>(mappings.Count);
            var diskSnapshot = DiskBuffer.CurrentSnapshot;
            int primaryIndex = 0;
            Span span;

            for (int i = 0; i < mappings.Count; i++) {
                ProjectionMapping mapping = mappings[i];
                if (mapping.Length > 0) {
                    span = Span.FromBounds(primaryIndex, mapping.SourceStart);
                    spans.Add(new CustomTrackingSpan(diskSnapshot, span, i == 0 ? PointTrackingMode.Negative : PointTrackingMode.Positive, PointTrackingMode.Positive)); // Markdown
                    primaryIndex = mapping.SourceRange.End;

                    span = new Span(mapping.ProjectionStart, mapping.Length);
                    spans.Add(new CustomTrackingSpan(ContainedLanguageBuffer.CurrentSnapshot, span, PointTrackingMode.Positive, PointTrackingMode.Positive)); // R
                }
            }
            // Add the final section after the last span
            span = Span.FromBounds(primaryIndex, diskSnapshot.Length);
            if (!span.IsEmpty) {
                spans.Add(new CustomTrackingSpan(diskSnapshot, span, PointTrackingMode.Positive, PointTrackingMode.Positive)); // Markdown
            }
            return spans;
        }

        private ITextCaret GetCaret() {
            var document = ServiceManager.GetService<IEditorDocument>(DiskBuffer);
            var textView = document?.TextBuffer.GetFirstView();
            return textView?.Caret;
        }

        private int? GetCaretPosition() {
            return GetCaret()?.Position.BufferPosition.Position;
        }

        private void SaveCaretPosition() {
            _savedCaretPosition = GetCaretPosition();
        }

        private void RestoreCaretPosition() {
            if (_savedCaretPosition.HasValue) {
                var caretPosition = GetCaretPosition();
                if (caretPosition.HasValue && caretPosition.Value != _savedCaretPosition.Value) {
                    var document = ServiceManager.GetService<IEditorDocument>(DiskBuffer);
                    var textView = document?.TextBuffer.GetFirstView();
                    if (textView != null) {
                        textView.Caret.MoveTo(new SnapshotPoint(textView.TextBuffer.CurrentSnapshot, _savedCaretPosition.Value));
                    }
                    _savedCaretPosition = null;
                }
            }
        }
    }
}
