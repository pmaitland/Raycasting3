using System.Collections.Generic;

using UnityEngine;

public static class Lighting
{

    public enum Type
    {
        DARKNESS,

        TORCH_0,
        TORCH_1,
        TORCH_2,
        TORCH_3,
        TORCH_4,

        LIGHT_SPELL_0,
        LIGHT_SPELL_1,
        LIGHT_SPELL_2,
        LIGHT_SPELL_3,
        LIGHT_SPELL_4,

        FIREBALL_SPELL_0,
        FIREBALL_SPELL_1,
        FIREBALL_SPELL_2,
    }

    private static readonly List<Type> Strengths = new() {
        Type.DARKNESS,
        Type.TORCH_4,
        Type.FIREBALL_SPELL_2,
        Type.LIGHT_SPELL_4,
        Type.TORCH_3,
        Type.FIREBALL_SPELL_1,
        Type.LIGHT_SPELL_3,
        Type.TORCH_2,
        Type.FIREBALL_SPELL_0,
        Type.LIGHT_SPELL_2,
        Type.TORCH_1,
        Type.TORCH_0,
        Type.LIGHT_SPELL_1,
        Type.LIGHT_SPELL_0
    };

    private static readonly List<Type> TorchStrengths = new() {
        Type.TORCH_4,
        Type.TORCH_3,
        Type.TORCH_2,
        Type.TORCH_1,
        Type.TORCH_0
    };

    private static readonly List<Type> LightSpellStrengths = new() {
        Type.LIGHT_SPELL_4,
        Type.LIGHT_SPELL_3,
        Type.LIGHT_SPELL_2,
        Type.LIGHT_SPELL_1,
        Type.LIGHT_SPELL_0
    };

    private static readonly List<Type> FireballSpellStrengths = new() {
        Type.FIREBALL_SPELL_2,
        Type.FIREBALL_SPELL_1,
        Type.FIREBALL_SPELL_0,
    };

    private static Color s_darkness = new(0.5f, 0.5f, 0.5f);

    private static Color s_torch_0 = new(2.0f, 1.5f, 1.0f);
    private static Color s_torch_1 = new(1.7f, 1.3f, 0.9f);
    private static Color s_torch_2 = new(1.4f, 1.1f, 0.8f);
    private static Color s_torch_3 = new(1.1f, 0.9f, 0.7f);
    private static Color s_torch_4 = new(0.8f, 0.7f, 0.6f);

    private static Color s_light_spell_0 = new(2.0f, 2.0f, 2.0f);
    private static Color s_light_spell_1 = new(1.7f, 1.7f, 1.7f);
    private static Color s_light_spell_2 = new(1.4f, 1.4f, 1.4f);
    private static Color s_light_spell_3 = new(1.1f, 1.1f, 1.1f);
    private static Color s_light_spell_4 = new(0.8f, 0.8f, 0.8f);

    private static Color s_fireball_spell_0 = new(1.4f, 1.1f, 0.8f);
    private static Color s_fireball_spell_1 = new(1.1f, 0.9f, 0.7f);
    private static Color s_fireball_spell_2 = new(0.8f, 0.7f, 0.6f);

    public static bool IsStronger(Type typeA, Type typeB)
    {
        return Strengths.IndexOf(typeA) >= Strengths.IndexOf(typeB);
    }

    public static Type GetStrongestLight(params Type[] types)
    {
        Type strongest = types[0];
        foreach (Type type in types)
        {
            if (IsStronger(type, strongest)) strongest = type;
        }
        return strongest;
    }

    public static Type GetDarker(Type type)
    {
        List<Type> array = Strengths;

        if (TorchStrengths.Contains(type)) array = TorchStrengths;
        else if (LightSpellStrengths.Contains(type)) array = LightSpellStrengths;
        else if (FireballSpellStrengths.Contains(type)) array = FireballSpellStrengths;

        return array.IndexOf(type) - 1 >= 0 ? array[array.IndexOf(type) - 1] : Type.DARKNESS;
    }

    public static Color GetColor(Type? type)
    {
        return type switch
        {
            Type.DARKNESS => s_darkness,
            Type.TORCH_0 => s_torch_0,
            Type.TORCH_1 => s_torch_1,
            Type.TORCH_2 => s_torch_2,
            Type.TORCH_3 => s_torch_3,
            Type.TORCH_4 => s_torch_4,
            Type.LIGHT_SPELL_0 => s_light_spell_0,
            Type.LIGHT_SPELL_1 => s_light_spell_1,
            Type.LIGHT_SPELL_2 => s_light_spell_2,
            Type.LIGHT_SPELL_3 => s_light_spell_3,
            Type.LIGHT_SPELL_4 => s_light_spell_4,
            Type.FIREBALL_SPELL_0 => s_fireball_spell_0,
            Type.FIREBALL_SPELL_1 => s_fireball_spell_1,
            Type.FIREBALL_SPELL_2 => s_fireball_spell_2,
            _ => s_darkness
        };
    }

}