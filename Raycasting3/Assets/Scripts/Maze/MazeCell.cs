using System.Collections.Generic;
using UnityEngine;

public class MazeCell {
    
    private int x;
    private int y;
    private MazeCellType type;

    private List<GameObject> mazePieces;
    private LightingType lighting;

    public MazeCell(int x, int y, MazeCellType type) {
        this.x = x;
        this.y = y;
        this.type = type;

        this.mazePieces = new List<GameObject>();
        this.lighting = LightingType.DARKNESS;
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

    public LightingType GetLighting() {
        return lighting;
    }

    public void SetCellType(MazeCellType newType) {
        type = newType;
    }

    public void SetLighting(LightingType newLighting) {
        lighting = newLighting;

        foreach (GameObject mazePiece in mazePieces) {
            switch (lighting) {
                case LightingType.DARKNESS:
                    mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.DARKNESS;
                    break;

                case LightingType.TORCH_0:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_1;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_0;
                    break;
                case LightingType.TORCH_1:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_2;
                    else
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_1;
                    break;
                case LightingType.TORCH_2:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_3;
                    else
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_2;
                    break;
                case LightingType.TORCH_3:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_4;
                    else
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_3;
                    break;
                case LightingType.TORCH_4:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.DARKNESS;
                    else
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.TORCH_4;
                    break;

                case LightingType.LIGHT_SPELL_0:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling"))
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_1;
                    else
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_0;
                    break;
                case LightingType.LIGHT_SPELL_1:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_2;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_1;
                    break;
                case LightingType.LIGHT_SPELL_2:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_3;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_2;
                    break;
                case LightingType.LIGHT_SPELL_3:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_4;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_3;
                    break;
                case LightingType.LIGHT_SPELL_4:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.DARKNESS;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.LIGHT_SPELL_4;
                    break;

                case LightingType.FIREBALL_SPELL_0:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.FIREBALL_SPELL_1;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.FIREBALL_SPELL_0;
                    break;
                case LightingType.FIREBALL_SPELL_1:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.FIREBALL_SPELL_2;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.FIREBALL_SPELL_1;
                    break;
                case LightingType.FIREBALL_SPELL_2:
                    if (mazePiece.name.Contains("L1") || mazePiece.name.Contains("Ceiling")) 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.DARKNESS;
                    else 
                        mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.FIREBALL_SPELL_2;
                    break;

                default:
                    mazePiece.GetComponent<MeshRenderer>().material.color = LightingColor.DARKNESS;
                    break;
            }
        }
    }

    public void AddToMazePieces(GameObject mazePiece) {
        mazePieces.Add(mazePiece);
    }
}
