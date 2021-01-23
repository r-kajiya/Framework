using UnityEngine;

namespace Framework
{
    public static class Vector3Extensions
    {
        public static bool IsValid(this Vector3 self)
        {
            return VectorHelper.invalide3 != self;
        }
    }
}