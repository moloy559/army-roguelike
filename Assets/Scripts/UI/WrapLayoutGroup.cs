using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrapLayoutGroup : LayoutGroup
{
    public float verticalSpacing = 5f;
    public float horizontalSpacing = 5f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
        Arrange();
    }

    public override void SetLayoutVertical()
    {
        Arrange();
    }

    void Arrange()
    {
        float parentWidth = rectTransform.rect.width;

        float x = padding.left;
        float y = padding.top;

        float rowHeight = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];

            float width = LayoutUtility.GetPreferredSize(child, 0);
            float height = LayoutUtility.GetPreferredSize(child, 1);

            // Wrap to next line
            if (x + width > parentWidth - padding.right)
            {
                x = padding.left;
                y += rowHeight + verticalSpacing;
                rowHeight = 0;
            }

            SetChildAlongAxis(child, 0, x, width);
            SetChildAlongAxis(child, 1, y, height);

            x += width + horizontalSpacing;

            if (height > rowHeight)
                rowHeight = height;
        }
    }
}
