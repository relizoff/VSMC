using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VSMC.Controls
{
    public class VirtualizingWrapPanel : VirtualizingPanelBase
    {
        public static readonly DependencyProperty EnableSpacingProperty = DependencyProperty.Register(nameof(EnableSpacing), typeof(bool), typeof(VirtualizingWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ChildrenSizeProperty = DependencyProperty.Register(nameof(ChildrenSize), typeof(Size), typeof(VirtualizingWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));
        private Size _childSize = new Size(100.0, 100.0);
        private int _startIndex;
        private int _endIndex;

        public bool EnableSpacing
        {
            get
            {
                return (bool)this.GetValue(VirtualizingWrapPanel.EnableSpacingProperty);
            }
            set
            {
                this.SetValue(VirtualizingWrapPanel.EnableSpacingProperty, (object)value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(VirtualizingWrapPanel.OrientationProperty);
            }
            set
            {
                this.SetValue(VirtualizingWrapPanel.OrientationProperty, (object)value);
            }
        }

        public Size ChildrenSize
        {
            get
            {
                return (Size)this.GetValue(VirtualizingWrapPanel.ChildrenSizeProperty);
            }
            set
            {
                this.SetValue(VirtualizingWrapPanel.ChildrenSizeProperty, (object)value);
            }
        }

        private ItemsControl ItemsOwner
        {
            get
            {
                return ItemsControl.GetItemsOwner((DependencyObject)this);
            }
        }

        public override void SetVerticalOffset(double offset)
        {
            base.SetVerticalOffset(offset);
            this.InvalidateMeasure();
        }

        public override void SetHorizontalOffset(double offset)
        {
            base.SetHorizontalOffset(offset);
            this.InvalidateMeasure();
        }

        public new void UpdateScrollInfo(Size availableSize, Size childSize)
        {
            int num = (int)Math.Ceiling((double)this.ItemsOwner.Items.Count / (double)this.GetItemsPerRow(availableSize));
            Size size = this.CreateSize(Math.Max(this.GetWidth(availableSize), this.GetWidth(childSize)), Math.Max(availableSize.Height, childSize.Height * (double)num));
            base.UpdateScrollInfo(availableSize, size);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.UpdateScrollInfo(availableSize, this._childSize);
            this.VirtualizeItems(availableSize);
            this.CleanupItems();
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.UpdateScrollInfo(finalSize, this._childSize);
            for (int index = 0; index < this.InternalChildren.Count; ++index)
            {
                UIElement internalChild = this.InternalChildren[index];
                int num1 = this.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(index, 0));
                int itemsPerRow = this.GetItemsPerRow(finalSize);
                double num2 = this.GetWidth(finalSize) > this.GetWidth(this._childSize) ? this.GetWidth(finalSize) % this.GetWidth(this._childSize) / (double)(itemsPerRow + 1) : 0.0;
                double num3 = (double)(num1 % itemsPerRow) * this.GetWidth(this._childSize);
                if (this.EnableSpacing)
                    num3 += (double)(num1 % itemsPerRow + 1) * num2;
                double num4 = (double)(num1 / itemsPerRow) * this.GetHeight(this._childSize);
                Rect rect = this.CreateRect(num3 - this.GetX(this.Offset), num4 - this.GetY(this.Offset), this._childSize.Width, this._childSize.Height);
                internalChild.Arrange(rect);
            }
            return finalSize;
        }

        public override Rect MakeVisible(Visual visual, Rect rectangle)
        {
            double num1 = (double)(this.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(this.InternalChildren.IndexOf((UIElement)VirtualizingWrapPanel.FindContainer((DependencyObject)visual)), 0)) / this.GetItemsPerRow(this.Viewport)) * this.GetHeight(this._childSize);
            double num2 = num1 + this.GetHeight(this._childSize);
            if (num1 < this.GetY(this.Offset))
                this.SetOffset(this.GetY(this.Offset) - (this.GetY(this.Offset) - num1));
            if (num2 > this.GetY(this.Offset) + this.GetHeight(this.Viewport))
                this.SetOffset(this.GetY(this.Offset) + num2 - (this.GetY(this.Offset) + this.GetHeight(this.Viewport)));
            return rectangle;
        }

        protected static ListBoxItem FindContainer(DependencyObject element)
        {
            for (; element != null; element = VisualTreeHelper.GetParent(element))
            {
                if (element is ListBoxItem)
                    return (ListBoxItem)element;
            }
            return (ListBoxItem)null;
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    this.RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                    break;
            }
        }

        private void VirtualizeItems(Size availableSize)
        {
            this.UpdateIndexRange();
            this.GetItemsPerRow(availableSize);
            int count = this.InternalChildren.Count;
            GeneratorPosition position = this.ItemContainerGenerator.GeneratorPositionFromIndex(this._startIndex);
            int index = position.Offset == 0 ? position.Index : position.Index + 1;
            using (this.ItemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true))
            {
                int startIndex = this._startIndex;
                while (startIndex <= this._endIndex)
                {
                    bool isNewlyRealized;
                    UIElement next = (UIElement)this.ItemContainerGenerator.GenerateNext(out isNewlyRealized);
                    if (isNewlyRealized || this.VirtualizationMode == VirtualizationMode.Recycling && !this.InternalChildren.Contains(next))
                    {
                        if (index >= this.InternalChildren.Count)
                            this.AddInternalChild(next);
                        else
                            this.InsertInternalChild(index, next);
                        this.ItemContainerGenerator.PrepareItemContainer((DependencyObject)next);
                        if (this.ChildrenSize == Size.Empty)
                            next.Measure(this.CreateSize(this.GetWidth(availableSize), double.MaxValue));
                        else
                            next.Measure(this.ChildrenSize);
                    }
                    ++startIndex;
                    ++index;
                }
            }
            if (this.ChildrenSize == Size.Empty)
            {
                if (this.InternalChildren.Count <= 0)
                    return;
                this._childSize = this.InternalChildren[0].DesiredSize;
                this.ItemSize = this._childSize;
            }
            else
            {
                this._childSize = this.ChildrenSize;
                this.ItemSize = this._childSize;
            }
        }

        private void CleanupItems()
        {
            for (int index = this.InternalChildren.Count - 1; index >= 0; --index)
            {
                GeneratorPosition position = new GeneratorPosition(index, 0);
                int num = this.ItemContainerGenerator.IndexFromGeneratorPosition(position);
                if (num < this._startIndex || num > this._endIndex)
                {
                    if (this.VirtualizationMode == VirtualizationMode.Recycling)
                        this.ItemContainerGenerator.Recycle(position, 1);
                    else
                        this.ItemContainerGenerator.Remove(position, 1);
                    this.RemoveInternalChildRange(index, 1);
                }
            }
        }

        private void UpdateIndexRange()
        {
            if (this.IsVirtualizing)
            {
                int itemsPerRow = this.GetItemsPerRow(this.Viewport);
                VirtualizationCacheLength cacheLength;
                double num1;
                double num2;
                if (this.CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
                {
                    double y = this.GetY(this.Offset);
                    cacheLength = this.CacheLength;
                    double cacheBeforeViewport = cacheLength.CacheBeforeViewport;
                    num1 = Math.Max(y - cacheBeforeViewport, 0.0);
                    double num3 = this.GetY(this.Offset) + this.GetHeight(this.Viewport);
                    cacheLength = this.CacheLength;
                    double cacheAfterViewport = cacheLength.CacheAfterViewport;
                    num2 = Math.Min(num3 + cacheAfterViewport, this.GetHeight(this.Extent));
                }
                else
                {
                    num1 = this.GetY(this.Offset);
                    num2 = this.GetY(this.Offset) + this.GetHeight(this.Viewport);
                }
                int val2_1 = (int)Math.Floor(num1 / this.GetHeight(this._childSize)) * itemsPerRow;
                int val2_2 = (int)Math.Ceiling(num2 / this.GetHeight(this._childSize)) * itemsPerRow - 1;
                if (this.CacheLengthUnit == VirtualizationCacheLengthUnit.Page)
                {
                    int num3 = itemsPerRow * (int)(this.GetHeight(this.Viewport) / this.GetHeight(this._childSize));
                    int num4 = val2_1;
                    cacheLength = this.CacheLength;
                    int num5 = (int)cacheLength.CacheBeforeViewport * num3;
                    val2_1 = num4 - num5;
                    int num6 = val2_2;
                    cacheLength = this.CacheLength;
                    int num7 = (int)cacheLength.CacheAfterViewport * num3;
                    val2_2 = num6 + num7;
                }
                else if (this.CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
                {
                    int num3 = val2_1;
                    cacheLength = this.CacheLength;
                    int cacheBeforeViewport = (int)cacheLength.CacheBeforeViewport;
                    val2_1 = num3 - cacheBeforeViewport;
                    int num4 = val2_2;
                    cacheLength = this.CacheLength;
                    int cacheAfterViewport = (int)cacheLength.CacheAfterViewport;
                    val2_2 = num4 + cacheAfterViewport;
                }
                this._startIndex = Math.Max(0, val2_1);
                this._endIndex = Math.Min(this.ItemsOwner.Items.Count - 1, val2_2);
            }
            else
            {
                this._startIndex = 0;
                this._endIndex = this.ItemsOwner.Items.Count - 1;
            }
        }

        private double GetX(Point point)
        {
            if (this.Orientation != Orientation.Vertical)
                return point.Y;
            return point.X;
        }

        private double GetY(Point point)
        {
            if (this.Orientation != Orientation.Vertical)
                return point.X;
            return point.Y;
        }

        private double GetWidth(Size size)
        {
            if (this.Orientation != Orientation.Vertical)
                return size.Height;
            return size.Width;
        }

        private double GetHeight(Size size)
        {
            if (this.Orientation != Orientation.Vertical)
                return size.Width;
            return size.Height;
        }

        private Size CreateSize(double width, double height)
        {
            if (this.Orientation != Orientation.Vertical)
                return new Size(height, width);
            return new Size(width, height);
        }

        private Rect CreateRect(double x, double y, double width, double height)
        {
            if (this.Orientation != Orientation.Vertical)
                return new Rect(y, x, width, height);
            return new Rect(x, y, width, height);
        }

        private void SetOffset(double offset)
        {
            if (this.Orientation == Orientation.Vertical)
                this.SetVerticalOffset(offset);
            else
                this.SetHorizontalOffset(offset);
        }

        private int GetItemsPerRow(Size size)
        {
            return (int)Math.Max(1.0, Math.Floor(this.GetWidth(size) / this.GetWidth(this._childSize)));
        }
    }
}
