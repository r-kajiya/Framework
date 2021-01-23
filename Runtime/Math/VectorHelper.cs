using UnityEngine;

namespace Framework
{
	public static class VectorHelper
	{
        public static readonly Vector2 zero2 = Vector2.zero;
        public static readonly Vector2 one2 = Vector2.one;
        public static readonly Vector2 up2 = Vector2.up;
        public static readonly Vector2 down2 = Vector2.down;
        public static readonly Vector2 left2 = Vector2.left;
        public static readonly Vector2 right2 = Vector2.right;

        public static readonly Vector3 zero3 = Vector3.zero;
        public static readonly Vector3 one3 = Vector3.one;
        public static readonly Vector3 forward3 = Vector3.forward;
        public static readonly Vector3 back3 = Vector3.back;
        public static readonly Vector3 up3 = Vector3.up;
        public static readonly Vector3 down3 = Vector3.down;
        public static readonly Vector3 left3 = Vector3.left;
        public static readonly Vector3 right3 = Vector3.right;
        public static readonly Vector3 invalide3 = new Vector3(9999999f,9999999f,9999999f);

        public static Vector3 AlongCamera(Vector3 vec, Camera camera)
        {
            Vector3 cameraForward = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraVec = cameraForward * vec.z + camera.transform.right * vec.x;
            return cameraVec;
        }

        public static Vector3 RandomSphere(float range = 1f)
        {
            return Random.insideUnitSphere * range;
        }

        public static Vector3 RandomCircleXZ(float range = 1f)
        {
            Vector3 random = RandomSphere(range);
            return new Vector3(random.x, 0f, random.z);
        }
    }	
}