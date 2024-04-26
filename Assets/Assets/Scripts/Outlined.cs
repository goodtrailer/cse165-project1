using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Outlined : MonoBehaviour
{
    [field: SerializeField]
    public Color OutlineColor { get; set; } = Color.white;

    [field: SerializeField]
    public Outline.Mode OutlineMode { get; set; } = Outline.Mode.OutlineAndSilhouette;

    [field: SerializeField]
    public float OutlineWidth { get; set; } = 4f;

    [field: SerializeField]
    public bool OutlineEnabled { get; set; } = false;

    private Manipulatable manipulatable;
    private bool isHovered = false;
    private bool isSelected = false;

    void Start()
    {
        manipulatable = GetComponent<Manipulatable>();

        manipulatable.OnHovered += (_, _) =>
        {
            isHovered = true;
            UpdateOutline();
        };

        manipulatable.OnUnhovered += (_, _) =>
        {
            isHovered = false;
            UpdateOutline();
        };

        manipulatable.OnSelected += (_, _) =>
        {
            isSelected = true;
            UpdateOutline();
        };

        manipulatable.OnUnselected += (_, _) =>
        {
            isSelected = false;
            UpdateOutline();
        };
    }

    public void UpdateOutline()
    {
        Outline outline = GetComponent<Outline>();

        if (outline == null)
            return;

        if (isSelected)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
            outline.OutlineMode = OutlineMode;
            outline.OutlineWidth = 4f;
            outline.UpdateMaterialProperties();
        }
        else if (isHovered)
        {
            outline.enabled = true;
            outline.OutlineColor = new Color(0f, 0.8703723f, 1f, 1f);
            outline.OutlineMode = OutlineMode;
            outline.OutlineWidth = 4f;
        }
        else
        {
            outline.enabled = OutlineEnabled;
            outline.OutlineColor = OutlineColor;
            outline.OutlineMode = OutlineMode;
            outline.OutlineWidth = OutlineWidth;
        }
        outline.UpdateMaterialProperties();
    }
}
