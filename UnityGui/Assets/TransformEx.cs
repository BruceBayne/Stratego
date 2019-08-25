using UnityEngine;

namespace Assets
{
    public static class TransformEx
    {
        public static Transform DestroyChild(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
            return transform;
        }
    }
}