using System;
using UnityEngine;

public class Manipulatable : MonoBehaviour
{
    public event EventHandler OnSelected;

    public event EventHandler OnUnselected;

    public event EventHandler OnHovered;

    public event EventHandler OnUnhovered;

    public void Hover()
    {
        OnHovered?.Invoke(this, null);
    }

    public void Unhover()
    {
        OnUnhovered?.Invoke(this, null);
    }

    public void Select()
    {
        OnSelected?.Invoke(this, null);
    }

    public void Unselect()
    {
        OnUnselected?.Invoke(this, null);
    }
}
