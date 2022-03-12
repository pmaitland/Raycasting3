using UnityEngine;

public static class LightingColor {
    public static Color DARKNESS = new Color(0.5f, 0.5f, 0.5f);

    public static Color TORCH = new Color(2.00f, 1.50f, 1.00f);
    public static Color TORCH_1 = new Color(1.70f, 1.30f, 0.90f);
    public static Color TORCH_2 = new Color(1.40f, 1.10f, 0.80f);
    public static Color TORCH_3 = new Color(1.10f, 0.90f, 0.70f);
    public static Color TORCH_4 = new Color(0.80f, 0.70f, 0.60f);

    public static Color GetLightingColor(LightingType type) {
        switch (type) {
            case LightingType.DARKNESS:
                return DARKNESS;
            case LightingType.TORCH:
                return TORCH;
            case LightingType.TORCH_1:
                return TORCH_1;
            case LightingType.TORCH_2:
                return TORCH_2;
            case LightingType.TORCH_3:
                return TORCH_3;
            case LightingType.TORCH_4:
                return TORCH_4;
            default:
                return DARKNESS;
        }
    }
}
