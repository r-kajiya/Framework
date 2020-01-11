using UnityEngine;

namespace Dot
{
	public static class MathHelper
	{
        public const float EPSILON = 0.0001f;

        public static bool EqualsZero(this float value)
        {
            if( value < EPSILON && 
                value > -EPSILON)
            {
                return true;
            }

            return false;
        }

        public static bool EqualsZero(this float value, float epsilon)
        {
            if (value < epsilon &&
                value > -epsilon)
            {
                return true;
            }

            return false;
        }

        public static bool IsHitSphere(Vector3 posA, Vector3 posB, float radius)
        {
            float x = (posB.x - posA.x) * (posB.x - posA.x);
            float y = (posB.y - posA.y) * (posB.y - posA.y);
            float z = (posB.z - posA.z) * (posB.z - posA.z);

            return x + y + z <= radius * radius;
        }
    }	
}