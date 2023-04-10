using System.Collections.Generic;
using UnityEngine;

public static class Lighting {

    private static LightingType[] strengthsArray = {
        LightingType.DARKNESS,
        LightingType.TORCH_4,
        LightingType.FIREBALL_SPELL_2,
        LightingType.LIGHT_SPELL_4,
        LightingType.TORCH_3,
        LightingType.FIREBALL_SPELL_1,
        LightingType.LIGHT_SPELL_3,
        LightingType.TORCH_2,
        LightingType.FIREBALL_SPELL_0,
        LightingType.LIGHT_SPELL_2,
        LightingType.TORCH_1,
        LightingType.TORCH_0,
        LightingType.LIGHT_SPELL_1,
        LightingType.LIGHT_SPELL_0
    };

    private static LightingType[] torchStrengthsArray = {
        LightingType.TORCH_4,
        LightingType.TORCH_3,
        LightingType.TORCH_2,
        LightingType.TORCH_1,
        LightingType.TORCH_0
    };

    private static LightingType[] lightSpellStrengthsArray = {
        LightingType.LIGHT_SPELL_4,
        LightingType.LIGHT_SPELL_3,
        LightingType.LIGHT_SPELL_2,
        LightingType.LIGHT_SPELL_1,
        LightingType.LIGHT_SPELL_0
    };

    private static LightingType[] fireballSpellStrengthsArray = {
        LightingType.FIREBALL_SPELL_2,
        LightingType.FIREBALL_SPELL_1,
        LightingType.FIREBALL_SPELL_0,
    };

    private static List<LightingType> strengths = new List<LightingType>(strengthsArray);
    private static List<LightingType> torchStrengths = new List<LightingType>(torchStrengthsArray);
    private static List<LightingType> lightSpellStrengths = new List<LightingType>(lightSpellStrengthsArray);
    private static List<LightingType> fireballSpellStrengths = new List<LightingType>(fireballSpellStrengthsArray);

    private static Color DARKNESS = new Color(0.5f, 0.5f, 0.5f);

    private static Color TORCH_0 = new Color(2.0f, 1.5f, 1.0f);
    private static Color TORCH_1 = new Color(1.7f, 1.3f, 0.9f);
    private static Color TORCH_2 = new Color(1.4f, 1.1f, 0.8f);
    private static Color TORCH_3 = new Color(1.1f, 0.9f, 0.7f);
    private static Color TORCH_4 = new Color(0.8f, 0.7f, 0.6f);

    private static Color LIGHT_SPELL_0 = new Color(2.0f, 2.0f, 2.0f);
    private static Color LIGHT_SPELL_1 = new Color(1.7f, 1.7f, 1.7f);
    private static Color LIGHT_SPELL_2 = new Color(1.4f, 1.4f, 1.4f);
    private static Color LIGHT_SPELL_3 = new Color(1.1f, 1.1f, 1.1f);
    private static Color LIGHT_SPELL_4 = new Color(0.8f, 0.8f, 0.8f);

    private static Color FIREBALL_SPELL_0 = new Color(1.4f, 1.1f, 0.8f);
    private static Color FIREBALL_SPELL_1 = new Color(1.1f, 0.9f, 0.7f);
    private static Color FIREBALL_SPELL_2 = new Color(0.8f, 0.7f, 0.6f);

    public static bool IsStronger(LightingType typeA, LightingType typeB) {
        return strengths.IndexOf(typeA) >= strengths.IndexOf(typeB);
    }

    public static LightingType GetStrongestLight(params LightingType[] types) {
        LightingType strongest = types[0];
        foreach (LightingType type in types) {
            if (IsStronger(type, strongest)) strongest = type;
        }
        return strongest;
    }

    public static LightingType GetDarker(LightingType lightingType) {
        List<LightingType> array = strengths;

        if (torchStrengths.Contains(lightingType)) array = torchStrengths;
        else if (lightSpellStrengths.Contains(lightingType)) array = lightSpellStrengths;
        else if (fireballSpellStrengths.Contains(lightingType)) array = fireballSpellStrengths;

        if (array.IndexOf(lightingType) - 1 >= 0) return array[array.IndexOf(lightingType) - 1];
        
        return LightingType.DARKNESS; 
    }

    public static Color GetColor(LightingType? type) {
        switch (type) {
            case LightingType.DARKNESS: return DARKNESS;

            case LightingType.TORCH_0: return TORCH_0;
            case LightingType.TORCH_1: return TORCH_1;
            case LightingType.TORCH_2: return TORCH_2;
            case LightingType.TORCH_3: return TORCH_3;
            case LightingType.TORCH_4: return TORCH_4;

            case LightingType.LIGHT_SPELL_0: return LIGHT_SPELL_0;
            case LightingType.LIGHT_SPELL_1: return LIGHT_SPELL_1;
            case LightingType.LIGHT_SPELL_2: return LIGHT_SPELL_2;
            case LightingType.LIGHT_SPELL_3: return LIGHT_SPELL_3;
            case LightingType.LIGHT_SPELL_4: return LIGHT_SPELL_4;

            case LightingType.FIREBALL_SPELL_0: return FIREBALL_SPELL_0;
            case LightingType.FIREBALL_SPELL_1: return FIREBALL_SPELL_1;
            case LightingType.FIREBALL_SPELL_2: return FIREBALL_SPELL_2;

            default: return DARKNESS;
        }
    }

}