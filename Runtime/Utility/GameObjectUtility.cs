using UnityEngine;

namespace Framework
{
    public static class GameObjectUtility
    {
        public static Transform RecursiveFindChild(Transform parent, string childName)
        {
            Transform child = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                child = parent.GetChild(i);
                if (child.name == childName)
                {
                    break;
                }

                child = RecursiveFindChild(child, childName);
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }
    }    
}

