using UnityEngine;

namespace Framework
{
    public static class Vector3Extensions
    {
        public static bool IsValid(this Vector3 self)
        {
            return VectorHelper.invalide3 != self;
        }
        
        public static Vector3 X(this Vector3 self)
        {
            return new Vector3(self.x, 0.0f, 0.0f);
        }
        
        public static Vector3 Y(this Vector3 self)
        {
            return new Vector3(0.0f, self.y, 0.0f);
        }
        
        public static Vector3 Z(this Vector3 self)
        {
            return new Vector3(0.0f, 0.0f, self.z);
        }

        public static Vector3 XY(this Vector3 self)
        {
            return new Vector3(self.x, self.y, 0.0f);
        }
        
        public static Vector3 XZ(this Vector3 self)
        {
            return new Vector3(self.x, 0.0f, self.z);
        }
        
        public static Vector2 XYtoVector2(this Vector3 self)
        {
            return new Vector2(self.x,self.y);
        }
        
        public static Vector2 XZtoVector2(this Vector3 self)
        {
            return new Vector2(self.x,self.z);
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 Abs(this Vector2 self)
        {
            return new Vector2(Mathf.Abs(self.x), Mathf.Abs(self.y));
        }
    }
}