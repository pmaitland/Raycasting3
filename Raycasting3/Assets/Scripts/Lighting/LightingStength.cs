using System.Collections.Generic;

public static class LightingStrength {

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

    private static List<LightingType> strengths = new List<LightingType>(strengthsArray);

    public static bool IsStronger(LightingType typeA, LightingType typeB) {
        return strengths.IndexOf(typeA) > strengths.IndexOf(typeB);
    }

    public static LightingType GetStrongestLight(params LightingType[] types) {
        LightingType strongest = types[0];
        foreach (LightingType type in types) {
            if (strengths.IndexOf(type) > strengths.IndexOf(strongest)) strongest = type;
        }
        return strongest;
    }

}