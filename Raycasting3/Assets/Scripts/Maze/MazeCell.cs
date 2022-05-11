using System.Collections.Generic;
using UnityEngine;

public class MazeCell {
    
    private int x;
    private int y;
    private MazeCellType type;

    private List<GameObject> mazePieces;
    private LightingType lightingLower;
    private LightingType lightingUpper;

    public MazeCell(int x, int y, MazeCellType type) {
        this.x = x;
        this.y = y;
        this.type = type;

        this.mazePieces = new List<GameObject>();
        this.lightingLower = LightingType.DARKNESS;
        this.lightingUpper = LightingType.DARKNESS;
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

    private void UpdateLighting() {
        foreach (GameObject mazePiece in mazePieces) {
            Material material = mazePiece.GetComponent<MeshRenderer>().material;
            if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling") || mazePiece.transform.position.y == 1)
                material.color = Lighting.GetColor(lightingUpper);
            else
                material.color = Lighting.GetColor(lightingLower);
        }
    }

    public void AddToMazePieces(GameObject mazePiece) {
        if (mazePiece.GetComponent<MeshRenderer>() != null) mazePieces.Add(mazePiece);
        foreach (Transform child in mazePiece.transform) AddToMazePieces(child.gameObject);
    }
}
