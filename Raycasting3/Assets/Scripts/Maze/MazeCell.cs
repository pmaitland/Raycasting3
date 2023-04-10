using System.Collections.Generic;
using UnityEngine;

public class MazeCell {
    
    private int x;
    private int y;
    private MazeCellType type;

    private List<GameObject> mazePieces;

    private LightingType lightingLower;
    private LightingType lightingUpper;

    private LightingType temporaryLightingLower;
    private LightingType temporaryLightingUpper;

    private Color detailColour;

    public MazeCell(int x, int y, MazeCellType type, Color detailColour) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.detailColour = detailColour;

        this.mazePieces = new List<GameObject>();

        this.lightingLower = LightingType.DARKNESS;
        this.lightingUpper = LightingType.DARKNESS;

        this.temporaryLightingLower = LightingType.DARKNESS;
        this.temporaryLightingUpper = LightingType.DARKNESS;
    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }

    public MazeCellType GetCellType() {
        return type;
    }

    public LightingType GetLightingLower() {
        return lightingLower;
    }

    public LightingType GetLightingUpper() {
        return lightingUpper;
    }

    public LightingType GetTemporaryLightingLower() {
        return temporaryLightingLower;
    }

    public LightingType GetTemporaryLightingUpper() {
        return temporaryLightingUpper;
    }

    public LightingType GetCurrentLightingLower() {
        return Lighting.GetStrongestLight(lightingLower, temporaryLightingLower);
    }

    public void SetCellType(MazeCellType newType) {
        type = newType;
    }

    public void SetLightingLower(LightingType newLighting) {
        lightingLower = newLighting;
        if (Lighting.IsStronger(Lighting.GetDarker(lightingLower), lightingUpper)) lightingUpper = Lighting.GetDarker(lightingLower);
        UpdateLighting();
    }

    public void SetLightingUpper(LightingType newLighting) {
        lightingUpper = newLighting;
        if (Lighting.IsStronger(Lighting.GetDarker(lightingUpper), lightingLower)) lightingLower = Lighting.GetDarker(lightingUpper);
        UpdateLighting();
    }

    public void SetTemporaryLightingLower(LightingType newLighting) {
        temporaryLightingLower = newLighting;
        if (Lighting.IsStronger(Lighting.GetDarker(temporaryLightingLower), temporaryLightingUpper)) temporaryLightingUpper = Lighting.GetDarker(temporaryLightingLower);
        UpdateLighting();
    }

    public void SetTemporaryLightingUpper(LightingType newLighting) {
        temporaryLightingUpper = newLighting;
        if (Lighting.IsStronger(Lighting.GetDarker(temporaryLightingUpper), temporaryLightingLower)) temporaryLightingLower = Lighting.GetDarker(temporaryLightingUpper);
        UpdateLighting();
    }

    private void UpdateLighting() {
        LightingType upper = Lighting.GetStrongestLight(lightingUpper, temporaryLightingUpper);
        LightingType lower = Lighting.GetStrongestLight(lightingLower, temporaryLightingLower);

        foreach (GameObject mazePiece in mazePieces) {
            Material material = mazePiece.GetComponent<MeshRenderer>().materials[0];
            if (mazePiece.name.Contains("L1")) {
                material.color = Lighting.GetColor(upper);
            } else if (mazePiece.name.Contains("Carpet") || mazePiece.name.Contains("Banner")) {
                material.color = detailColour;
                material.color += (Lighting.GetColor(lower) / 2f);
            } else {
                material.color = Lighting.GetColor(lower);
            }
        }
    }

    public void AddToMazePieces(GameObject mazePiece) {
        if (mazePiece.GetComponent<MeshRenderer>() != null) mazePieces.Add(mazePiece);
        foreach (Transform child in mazePiece.transform) AddToMazePieces(child.gameObject);
    }
}
