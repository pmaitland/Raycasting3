using System.Collections.Generic;
using System.ComponentModel;

using UnityEngine;

public class MazeCell
{

    public enum Type
    {
        // Basic types
        WALL,
        PASSAGE,
        DEAD_END,
        ROOM,
        HIDDEN_ROOM,
        DOOR,
        HIDDEN_DOOR,

        // Used for generation; not in final maze
        DISCONNECTED_DOOR,
        DISCONNECTED_HIDDEN_DOOR
    }

    public int X { get; private set; }
    public int Y { get; private set; }
    public Type CurrentType { get; set; }

    private readonly List<GameObject> _mazePieces;

    private Lighting.Type _lightingLower = Lighting.Type.DARKNESS;
    public Lighting.Type LightingLower
    {
        get
        {
            return _lightingLower;
        }
        set
        {
            _lightingLower = value;
            if (Lighting.IsStronger(Lighting.GetDarker(_lightingLower), _lightingUpper)) { _lightingUpper = Lighting.GetDarker(_lightingLower); }
            UpdateLighting();
        }
    }

    private Lighting.Type _lightingUpper = Lighting.Type.DARKNESS;
    public Lighting.Type LightingUpper
    {
        get
        {
            return _lightingUpper;
        }
        set
        {
            _lightingUpper = value;
            if (Lighting.IsStronger(Lighting.GetDarker(_lightingUpper), _lightingLower)) { _lightingLower = Lighting.GetDarker(_lightingUpper); }
            UpdateLighting();
        }
    }

    private Lighting.Type _temporaryLightingLower = Lighting.Type.DARKNESS;
    public Lighting.Type TemporaryLightingLower
    {
        get
        {
            return _temporaryLightingLower;
        }
        set
        {
            _temporaryLightingLower = value;
            if (Lighting.IsStronger(Lighting.GetDarker(_temporaryLightingLower), _temporaryLightingUpper)) { _temporaryLightingUpper = Lighting.GetDarker(_temporaryLightingLower); }
            UpdateLighting();
        }
    }

    private Lighting.Type _temporaryLightingUpper = Lighting.Type.DARKNESS;
    public Lighting.Type TemporaryLightingUpper
    {
        get
        {
            return _temporaryLightingUpper;
        }
        set
        {
            _temporaryLightingUpper = value;
            if (Lighting.IsStronger(Lighting.GetDarker(_temporaryLightingUpper), _temporaryLightingLower)) { _temporaryLightingLower = Lighting.GetDarker(_temporaryLightingUpper); }
            UpdateLighting();
        }
    }

    private Color _detailColour;

    public MazeCell(int x, int y, Type type, Color detailColour)
    {
        X = x;
        Y = y;
        CurrentType = type;
        _detailColour = detailColour;

        _mazePieces = new List<GameObject>();

        _lightingLower = Lighting.Type.DARKNESS;
        _lightingUpper = Lighting.Type.DARKNESS;

        _temporaryLightingLower = Lighting.Type.DARKNESS;
        _temporaryLightingUpper = Lighting.Type.DARKNESS;
    }

    public Lighting.Type GetCurrentLightingLower()
    {
        return Lighting.GetStrongestLight(_lightingLower, _temporaryLightingLower);
    }

    private void UpdateLighting()
    {
        Lighting.Type upper = Lighting.GetStrongestLight(_lightingUpper, _temporaryLightingUpper);
        Lighting.Type lower = Lighting.GetStrongestLight(_lightingLower, _temporaryLightingLower);

        foreach (GameObject mazePiece in _mazePieces)
        {
            Material material = mazePiece.GetComponent<MeshRenderer>().materials[0];
            if (mazePiece.name.Contains("L1"))
            {
                material.color = Lighting.GetColor(upper);
            }
            else if (mazePiece.name.Contains("Carpet") || mazePiece.name.Contains("Banner"))
            {
                material.color = _detailColour;
                material.color += Lighting.GetColor(lower) / 2f;
            }
            else
            {
                material.color = Lighting.GetColor(lower);
            }
        }
    }

    public void AddToMazePieces(GameObject mazePiece)
    {
        if (mazePiece.GetComponent<MeshRenderer>() != null) _mazePieces.Add(mazePiece);
        foreach (Transform child in mazePiece.transform) AddToMazePieces(child.gameObject);
    }
}
