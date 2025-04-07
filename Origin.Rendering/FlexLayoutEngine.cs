using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Origin.DOM;

namespace Origin.Rendering
{
    public class FlexLayoutEngine
    {
        private class FlexItem
        {
            public LayoutNode Node { get; set; }
            public float FlexGrow { get; set; }
            public float FlexShrink { get; set; }
            public float FlexBasis { get; set; }
            public float MainSize { get; set; }
            public float CrossSize { get; set; }
        }

        public LayoutNode CalculateFlexLayout(Element element, int containerWidth, int containerHeight)
        {
            var flexContainer = new LayoutNode { Element = element };
            
            // Determine main/cross axis based on flex-direction
            bool isRow = element.ComputedStyle.FlexDirection == "row" || 
                        element.ComputedStyle.FlexDirection == "row-reverse";
            bool isReverse = element.ComputedStyle.FlexDirection.EndsWith("reverse");
            bool isWrap = element.ComputedStyle.FlexWrap == "wrap" || 
                         element.ComputedStyle.FlexWrap == "wrap-reverse";

            // Collect and measure flex items
            var flexItems = new List<FlexItem>();
            foreach (var child in element.Children)
            {
                var item = new FlexItem
                {
                    Node = new LayoutNode { Element = child },
                    FlexGrow = ParseFloat(child.ComputedStyle.FlexGrow, 0),
                    FlexShrink = ParseFloat(child.ComputedStyle.FlexShrink, 1),
                    FlexBasis = ParseLength(child.ComputedStyle.FlexBasis, isRow ? containerWidth : containerHeight)
                };
                flexItems.Add(item);
            }

            // Calculate available space
            float mainSize = isRow ? containerWidth : containerHeight;
            float crossSize = isRow ? containerHeight : containerWidth;
            float usedMainSpace = 0;
            float usedCrossSpace = 0;

            // First pass - calculate base sizes
            foreach (var item in flexItems)
            {
                // Calculate hypothetical main size
                item.MainSize = Math.Max(item.FlexBasis, GetContentSize(item.Node.Element, isRow));
                usedMainSpace += item.MainSize;
            }

            // Determine if we need to grow or shrink
            float freeSpace = mainSize - usedMainSpace;
            if (freeSpace > 0)
            {
                // Distribute positive free space
                DistributeFreeSpace(flexItems, freeSpace, grow: true);
            }
            else if (freeSpace < 0)
            {
                // Distribute negative free space
                DistributeFreeSpace(flexItems, -freeSpace, grow: false);
            }

            // Calculate cross sizes
            foreach (var item in flexItems)
            {
                item.CrossSize = GetContentSize(item.Node.Element, !isRow);
                usedCrossSpace = Math.Max(usedCrossSpace, item.CrossSize);
            }

            // Apply justify-content alignment
            float pos = 0;
            switch (element.ComputedStyle.JustifyContent)
            {
                case "flex-end":
                    pos = freeSpace;
                    break;
                case "center":
                    pos = freeSpace / 2;
                    break;
                case "space-between":
                    pos = 0;
                    if (flexItems.Count > 1)
                    {
                        float gap = freeSpace / (flexItems.Count - 1);
                        for (int i = 0; i < flexItems.Count; i++)
                        {
                            if (i > 0) pos += gap;
                            SetItemPosition(flexItems[i], pos, isRow, isReverse);
                            pos += flexItems[i].MainSize;
                        }
                        continue;
                    }
                    break;
                case "space-around":
                    pos = freeSpace / (flexItems.Count * 2);
                    break;
            }

            // Position items
            foreach (var item in flexItems)
            {
                SetItemPosition(item, pos, isRow, isReverse);
                pos += item.MainSize;
                if (element.ComputedStyle.JustifyContent == "space-around")
                {
                    pos += freeSpace / flexItems.Count;
                }
            }

            // Add items to container
            foreach (var item in flexItems)
            {
                flexContainer.Children.Add(item.Node);
            }

            return flexContainer;
        }

        private void DistributeFreeSpace(List<FlexItem> items, float space, bool grow)
        {
            float totalFactor = items.Sum(i => grow ? i.FlexGrow : i.FlexShrink);
            if (totalFactor > 0)
            {
                foreach (var item in items)
                {
                    float factor = grow ? item.FlexGrow : item.FlexShrink;
                    item.MainSize += space * (factor / totalFactor);
                }
            }
        }

        private void SetItemPosition(FlexItem item, float pos, bool isRow, bool isReverse)
        {
            if (isRow)
            {
                item.Node.Bounds = new RectangleF(
                    isReverse ? pos + item.MainSize : pos,
                    0,
                    item.MainSize,
                    item.CrossSize);
            }
            else
            {
                item.Node.Bounds = new RectangleF(
                    0,
                    isReverse ? pos + item.MainSize : pos,
                    item.CrossSize,
                    item.MainSize);
            }
        }

        private float GetContentSize(Element element, bool isMainAxis)
        {
            // TODO: Implement actual content size calculation
            return isMainAxis ? 100 : 50; // Default sizes for demo
        }

        private float ParseLength(string value, float containerSize)
        {
            if (string.IsNullOrEmpty(value) || value == "auto") return 0;
            if (value.EndsWith("px")) return float.Parse(value[..^2]);
            if (value.EndsWith("%")) return containerSize * float.Parse(value[..^1]) / 100f;
            return float.Parse(value);
        }

        private float ParseFloat(string value, float defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return float.Parse(value);
        }
    }
}
