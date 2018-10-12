using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VSMC.Controls
{
    public abstract class VirtualizingPanelBase : VirtualizingPanel, IScrollInfo
    {
        protected Size Extent = new Size(0.0, 0.0);
        protected Size Viewport = new Size(0.0, 0.0);
        protected Point Offset = new Point(0.0, 0.0);
        protected Size ItemSize = new Size(20.0, 20.0);
        public const double ScrollLineDelta = 16.0;
        public const double MouseWheelDelta = 48.0;

        public ScrollViewer ScrollOwner { get; set; }

        public bool CanVerticallyScroll { get; set; }

        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth
        {
            get
            {
                return this.Extent.Width;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return this.Extent.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return this.Viewport.Width;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return this.Viewport.Height;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this.Offset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return this.Offset.Y;
            }
        }

        public bool IsVirtualizing
        {
            get
            {
                return VirtualizingPanel.GetIsVirtualizing((DependencyObject)this.ItemsControl);
            }
            set
            {
                VirtualizingPanel.SetIsVirtualizing((DependencyObject)this.ItemsControl, value);
            }
        }

        public VirtualizationMode VirtualizationMode
        {
            get
            {
                return VirtualizingPanel.GetVirtualizationMode((DependencyObject)this.ItemsControl);
            }
            set
            {
                VirtualizingPanel.SetVirtualizationMode((DependencyObject)this.ItemsControl, value);
            }
        }

        public VirtualizationCacheLength CacheLength
        {
            get
            {
                return VirtualizingPanel.GetCacheLength((DependencyObject)this.ItemsControl);
            }
            set
            {
                VirtualizingPanel.SetCacheLength((DependencyObject)this.ItemsControl, value);
            }
        }

        public VirtualizationCacheLengthUnit CacheLengthUnit
        {
            get
            {
                return VirtualizingPanel.GetCacheLengthUnit((DependencyObject)this.ItemsControl);
            }
            set
            {
                VirtualizingPanel.SetCacheLengthUnit((DependencyObject)this.ItemsControl, value);
            }
        }

        public ScrollUnit ScrollUnit
        {
            get
            {
                return VirtualizingPanel.GetScrollUnit((DependencyObject)this.ItemsControl);
            }
            set
            {
                VirtualizingPanel.SetScrollUnit((DependencyObject)this.ItemsControl, value);
            }
        }

        protected ItemsControl ItemsControl
        {
            get
            {
                return ItemsControl.GetItemsOwner((DependencyObject)this);
            }
        }

        protected IRecyclingItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return (IRecyclingItemContainerGenerator)base.ItemContainerGenerator;
            }
        }

        public void UpdateScrollInfo(Size availableSize, Size extent)
        {
            if (this.ViewportHeight != 0.0 && this.VerticalOffset != 0.0 && this.VerticalOffset + this.ViewportHeight + 1.0 >= this.ExtentHeight)
            {
                this.Offset.Y = extent.Height - availableSize.Height;
                this.ScrollOwner?.InvalidateScrollInfo();
            }
            if (this.ViewportWidth != 0.0 && this.HorizontalOffset != 0.0 && this.HorizontalOffset + this.ViewportWidth + 1.0 >= this.ExtentWidth)
            {
                this.Offset.X = extent.Width - availableSize.Width;
                this.ScrollOwner?.InvalidateScrollInfo();
            }
            if (extent != this.Extent)
            {
                this.Extent = extent;
                this.ScrollOwner?.InvalidateScrollInfo();
            }
            if (!(availableSize != this.Viewport))
                return;
            this.Viewport = availableSize;
            this.ScrollOwner?.InvalidateScrollInfo();
        }

        public virtual void SetVerticalOffset(double offset)
        {
            if (offset < 0.0 || this.Viewport.Height >= this.Extent.Height)
                offset = 0.0;
            else if (offset + this.Viewport.Height >= this.Extent.Height)
                offset = this.Extent.Height - this.Viewport.Height;
            this.Offset.Y = offset;
            this.ScrollOwner?.InvalidateScrollInfo();
        }

        public virtual void SetHorizontalOffset(double offset)
        {
            if (offset < 0.0 || this.Viewport.Width >= this.Extent.Width)
                offset = 0.0;
            else if (offset + this.Viewport.Width >= this.Extent.Width)
                offset = this.Extent.Width - this.Viewport.Width;
            this.Offset.X = offset;
            this.ScrollOwner?.InvalidateScrollInfo();
        }

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.GetLineHeight());
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.GetLineHeight());
        }

        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.GetLineWidth());
        }

        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.GetLineWidth());
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        }

        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
        }

        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.GetMouseWheelHeight());
        }

        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.GetMouseWheelHeight());
        }

        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.GetMouseWheelWidth());
        }

        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.GetMouseWheelWidth());
        }

        private double GetLineHeight()
        {
            if (this.ScrollUnit != ScrollUnit.Pixel)
                return this.ItemSize.Height;
            return 16.0;
        }

        private double GetLineWidth()
        {
            if (this.ScrollUnit != ScrollUnit.Pixel)
                return this.ItemSize.Width;
            return 16.0;
        }

        private double GetMouseWheelHeight()
        {
            if (this.ScrollUnit != ScrollUnit.Pixel)
                return 2.0 * this.ItemSize.Height;
            return 48.0;
        }

        private double GetMouseWheelWidth()
        {
            if (this.ScrollUnit != ScrollUnit.Pixel)
                return 2.0 * this.ItemSize.Height;
            return 48.0;
        }

        public virtual Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        

    }
}
