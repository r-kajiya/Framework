using UnityEngine;

namespace Dot
{
	public static class VectorHelper
	{
        public static readonly Vector2 ZERO_2 = Vector2.zero;
        public static readonly Vector2 ONE_2 = Vector2.one;
        public static readonly Vector2 UP_2 = Vector2.up;
        public static readonly Vector2 DOWN_2 = Vector2.down;
        public static readonly Vector2 LEFT_2 = Vector2.left;
        public static readonly Vector2 RIGHT_2 = Vector2.right;

        public static readonly Vector3 ZERO_3 = Vector3.zero;
        public static readonly Vector3 ONE_3 = Vector3.one;
        public static readonly Vector3 FORWARD_3 = Vector3.forward;
        public static readonly Vector3 BACK_3 = Vector3.back;
        public static readonly Vector3 UP_3 = Vector3.up;
        public static readonly Vector3 DOWN_3 = Vector3.down;
        public static readonly Vector3 LEFT_3 = Vector3.left;
        public static readonly Vector3 RIGHT_3 = Vector3.right;

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