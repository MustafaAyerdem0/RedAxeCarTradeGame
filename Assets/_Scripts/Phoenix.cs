using UnityEditor;
using UnityEngine;

public class Phoenix : MonoBehaviour
{
    [MenuItem("Phoenix/RectTransform/Archors To Pivot  #1")]
    static void ArchorsToPivot()
    {
        foreach (RectTransform t in Selection.transforms)
        {
            RectTransform pt = t.parent as RectTransform;
            if (t == null || pt == null) return;
            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width, t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width, t.anchorMax.y + t.offsetMax.y / pt.rect.height);
            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

    }

    [MenuItem("Phoenix/RectTransform/Pivot To Archors #2")]
    static void PivotToArchors()
    {
        foreach (RectTransform item in Selection.transforms)
        {
            if (item != null)
                item.offsetMin = item.offsetMax = new Vector2(0, 0);
        }
    }    
}