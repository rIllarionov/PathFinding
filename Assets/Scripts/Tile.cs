using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Color _allowingColor;
    [SerializeField]
    private Color _forbiddingColor;
    
    private List<Material> _materials = new();

    private void Awake()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in renderers)
        {
            _materials.Add(meshRenderer.material);
        }
    }

    public void SetColor(bool available)
    {
        if (available)
        {
            foreach (var material in _materials)
            {
                material.color = _allowingColor;
            }
        }
        else
        {
            foreach (var material in _materials)
            {
                material.color = _forbiddingColor;
            }
        }
    }

    public void ResetColor()
    {
        foreach (var material in _materials)
        {
            material.color = Color.white;
        }
    }
}