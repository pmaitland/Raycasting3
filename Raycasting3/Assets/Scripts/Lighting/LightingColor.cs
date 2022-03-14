using UnityEngine;

public static class LightingColor {
    public static Color DARKNESS = new Color(0.5f, 0.5f, 0.5f);

    public static Color TORCH_0 = new Color(2.0f, 1.5f, 1.0f);
    public static Color TORCH_1 = new Color(1.7f, 1.3f, 0.9f);
    public static Color TORCH_2 = new Color(1.4f, 1.1f, 0.8f);
    public static Color TORCH_3 = new Color(1.1f, 0.9f, 0.7f);
    public static Color TORCH_4 = new Color(0.8f, 0.7f, 0.6f);

    public static Color LIGHT_SPELL_0 = new Color(2.0f, 2.0f, 2.0f);
    public static Color LIGHT_SPELL_1 = new Color(1.7f, 1.7f, 1.7f);
    public static Color LIGHT_SPELL_2 = new Color(1.4f, 1.4f, 1.4f);
    public static Color LIGHT_SPELL_3 = new Color(1.1f, 1.1f, 1.1f);
    public static Color LIGHT_SPELL_4 = new Color(0.8f, 0.8f, 0.8f);

    public static Color FIREBALL_SPELL_0 = new Color(1.4f, 1.1f, 0.8f);
    public static Color FIREBALL_SPELL_1 = new Color(1.1f, 0.9f, 0.7f);
    public static Color FIREBALL_SPELL_2 = new Color(0.8f, 0.7f, 0.6f);

    public static Color GetLightingColor(LightingType type) {
        switch (type) {
            case LightingType.DARKNESS:
                return DARKNESS;

            case LightingType.TORCH_0:
                return TORCH_0;
            case LightingType.TORCH_1:
                return TORCH_1;
            case LightingType.TORCH_2:
                return TORCH_2;
            case LightingType.TORCH_3:
                return TORCH_3;
            case LightingType.TORCH_4:
                return TORCH_4;

            case LightingType.LIGHT_SPELL_0:
                return LIGHT_SPELL_0;
            case LightingType.LIGHT_SPELL_1:
                return LIGHT_SPELL_1;
            case LightingType.LIGHT_SPELL_2:
                return LIGHT_SPELL_2;
            case LightingType.LIGHT_SPELL_3:
                return LIGHT_SPELL_3;
            case LightingType.LIGHT_SPELL_4:
                return LIGHT_SPELL_4;

            case LightingType.FIREBALL_SPELL_0:
                return FIREBALL_SPELL_0;
            case LightingType.FIREBALL_SPELL_1:
                return FIREBALL_SPELL_1;
            case LightingType.FIREBALL_SPELL_2:
                return FIREBALL_SPELL_2;

            default:
                return DARKNESS;
        }
    }
}
