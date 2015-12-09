﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.R.Package.DataInspect {
    /// <summary>
    /// Item container in <see cref="DynamicGridRow"/>
    /// </summary>
    public class DynamicGridCell : ContentControl {
        static DynamicGridCell() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicGridCell), new FrameworkPropertyMetadata(typeof(DynamicGridCell)));
        }

        public DynamicGridCell() {
            Track = new LinkedListNode<DynamicGridCell>(this);
        }

        internal DynamicGridRow Owner { get; private set; }

        internal LinkedListNode<DynamicGridCell> Track { get; private set; }

        internal MaxDouble ColumnWidth { get; set; }

        internal virtual void Prepare(DynamicGridRow owner, MaxDouble columnWidth) {
            Owner = owner;

            if (ColumnWidth != null) {
                ColumnWidth.MaxChanged -= LayoutSize_MaxChanged;
            }

            ColumnWidth = columnWidth;
            ColumnWidth.MaxChanged += LayoutSize_MaxChanged;
        }

        private void LayoutSize_MaxChanged(object sender, EventArgs e) {
            InvalidateMeasure();
        }

        /// <summary>
        /// Clean up data when virtualized
        /// </summary>
        internal virtual void CleanUp() {
            this.Content = null;

            if (ColumnWidth != null) {
                ColumnWidth.MaxChanged -= LayoutSize_MaxChanged;
            }
            ColumnWidth = null;

            this.Owner = null;
        }

        protected override Size MeasureOverride(Size constraint) {
            constraint.Width = constraint.Width;

            Size desired = base.MeasureOverride(constraint);

            ColumnWidth.Max = desired.Width;
            desired.Width = ColumnWidth.Max;

            return desired;
        }

        protected override Size ArrangeOverride(Size arrangeBounds) {
            arrangeBounds.Width = ColumnWidth.Max;

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}