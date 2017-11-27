using UnityEngine;

public static class MaterialExtensions
{
    public static void SetColor(this Material material, Color color)
    {
        material.color = color;
    }
}

public static class GameObjectExtensions
{
    public static void SetColor(this GameObject gameObject, Color color)
    {
        gameObject.GetComponent<Renderer>()?.material.SetColor(color);
    }
}
