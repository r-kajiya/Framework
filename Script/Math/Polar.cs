using UnityEngine;
using System;

namespace Framework
{
    /// <summary>
    /// 球面座標系に沿った動きを扱いやすくしたもの
    /// </summary>
	public struct Polar : IEquatable<Polar>
    {
        const float DEFAULT_DISTACNE = 5;

        public float vertical;
        public float horizontal;
        public float distance;
        public Vector3 center;
        public Vector3 smoothDumpCenterVelocity;
        public float smoothDumpVerticalVelocity;
        public float smoothDumpHorizontalVelocity;
        public float smoothDumpDistanceVelocity;

        public Polar(float vertical) : this(vertical, 0) { }
        public Polar(float vertical, float horizontal) : this(vertical, horizontal, DEFAULT_DISTACNE) { }
        public Polar(float vertical, float horizontal, float distance) : this(vertical, horizontal, distance, Vector3.zero){}
        public Polar(float vertical, float horizontal, float distance, Vector3 center)
        {
            this.vertical = vertical;
            this.horizontal = horizontal;
            this.distance = distance;
            this.center = center;
            smoothDumpCenterVelocity = VectorHelper.ZERO_3;
            smoothDumpVerticalVelocity = 0f;
            smoothDumpHorizontalVelocity = 0f;
            smoothDumpDistanceVelocity = 0f;
        }

        /// <summary>
        /// 方向から仰角と方位角を計算
        /// 内部で正規化
        /// </summary>
        /// <param name="dir">方向</param>
        public void FromDir(Vector3 dir)
        {
            dir*=-1f;
            dir.Normalize();

            horizontal = Mathf.Atan2(dir.z, dir.x);
            float lxz = Mathf.Sqrt(dir.x * dir.x + dir.z * dir.z);
            vertical = Mathf.Atan2(dir.y, lxz);

            vertical *= Mathf.Rad2Deg;
            horizontal *= Mathf.Rad2Deg;
        }

        /// <summary>
        /// Polarのパラメーターを元に座標を計算
        /// TODO: 掛け算しすぎて誤差がでてる？カメラとかがくがくする
        /// </summary>
        /// <returns>計算した座標</returns>
        public Vector3 ComputePosition()
        {
            float radV = vertical * Mathf.Deg2Rad;
            float radH = horizontal * Mathf.Deg2Rad;
            float t = distance * Mathf.Cos(radV);

            Vector3 result = new Vector3(
                t * Mathf.Cos(radH),
                distance * Mathf.Sin(radV),
                t * Mathf.Sin(radH));

            return result + center;
        }

        /// <summary>
        /// 目的の値に補間する
        /// </summary>
        /// <param name="dest">目的の中心</param>
        /// <param name="time">補間時間</param>
        public void SmoothDumpCenter(Vector3 dest, float time)
        {
            center = Vector3.SmoothDamp(center, dest, ref smoothDumpCenterVelocity, time);
        }

        /// <summary>
        /// 目的の値に補間する
        /// </summary>
        /// <param name="dest">目的の仰角</param>
        /// <param name="time">補間時間</param>
        public void SmoothDumpVertical(float dest, float time)
        {
            vertical = Mathf.SmoothDampAngle(vertical, dest, ref smoothDumpVerticalVelocity, time);
        }

        /// <summary>
        /// 目的の値に補間する
        /// </summary>
        /// <param name="dest">目的の方位角</param>
        /// <param name="time">補間時間</param>
        public void SmoothDumpHorizontal(float dest, float time)
        {
            horizontal = Mathf.SmoothDampAngle(horizontal, dest, ref smoothDumpHorizontalVelocity, time);
        }

        /// <summary>
        /// 目的の値の長さに補間する
        /// </summary>
        /// <param name="dest">目的の長さ</param>
        /// <param name="time">補間時間</param>
        public void SmoothDumpDistance(float dest, float time)
        {
            distance = Mathf.SmoothDamp(distance, dest, ref smoothDumpDistanceVelocity, time);
        }

        public override string ToString()
        {
            return string.Format("中心:{0} 仰角:{1} 方位角:{2} 長さ:{3}", center, vertical, horizontal, distance);
        }

        #region IEquatable

        public bool Equals(Polar other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return vertical.GetHashCode() ^ horizontal.GetHashCode() ^ distance.GetHashCode() ^ center.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is Polar))
            {
                return false;
            }

            return Equals((Polar)other);
        }

        #endregion


        #region 演算子

        public static Polar operator +(Polar a, Polar b)
        {
            return new Polar(
                a.vertical + b.vertical,
                a.horizontal + b.horizontal,
                a.distance + b.distance,
                a.center + b.center);
        }

        public static Polar operator -(Polar a, Polar b)
        {
            return new Polar(
                a.vertical - b.vertical,
                a.horizontal - b.horizontal,
                a.distance - b.distance,
                a.center - b.center);
        }

        public static Polar operator *(Polar a, float d)
        {
            return new Polar(
                a.vertical,
                a.horizontal,
                a.distance * d,
                a.center);
        }

        public static Polar operator /(Polar a, float d)
        {
            return new Polar(
                a.vertical,
                a.horizontal,
                a.distance / d,
                a.center);
        }

        public static bool operator ==(Polar lhs, Polar rhs)
        {
            if (lhs.vertical - rhs.vertical > MathHelper.EPSILON)
            {
                return false;
            }

            if (lhs.horizontal - rhs.horizontal > MathHelper.EPSILON)
            {
                return false;
            }

            if (lhs.distance - rhs.distance > MathHelper.EPSILON)
            {
                return false;
            }

            if (lhs.center == rhs.center)
            {
                return false;
            }

            return true;
            
        }

        public static bool operator !=(Polar lhs, Polar rhs)
        {
            if (lhs == rhs)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}