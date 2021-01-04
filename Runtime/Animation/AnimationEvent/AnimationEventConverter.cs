using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Framework
{
    public static class AnimationEventConverter
    {
        public static AnimationEvent[] ToAnimationEvents(this AnimationClipEvents animationClipEvents)
        {
            List<AnimationEvent> animationEventList = new List<AnimationEvent>();

            foreach (var animationEventObjectPair in animationClipEvents.AnimationEventObjectMap)
            {
                AnimationEvent animationEvent = new AnimationEvent();

                animationEvent.time = animationEventObjectPair.Value.Time;
                animationEvent.functionName = animationEventObjectPair.Value.AnimationEventType.ToString();
                animationEvent.objectReferenceParameter = animationEventObjectPair.Value;
                animationEventList.Add(animationEvent);
            }

            return animationEventList.ToArray();
        }

        public static void ImportAnimationEvents(this EasyAnimation easyAnimation, AnimationEvents animationEvents)
        {
            foreach (var animationClip in easyAnimation.AnimationClips)
            {
                foreach (var animationClipEvent in animationEvents.AnimationClipEventsList)
                {
                    if (animationClipEvent.AnimationName == animationClip.name)
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            animationClip.events = animationClipEvent.ToAnimationEvents();   
                        }
                        else
                        {
                            AnimationUtility.SetAnimationEvents(animationClip, animationClipEvent.ToAnimationEvents());   
                        }
#else
                        animationClip.events = animationClipEvent.ToAnimationEvents();
#endif
                        break;
                    }
                }
            }
        }
        
#if UNITY_EDITOR
        public static void ClearAnimationEvents(this EasyAnimation easyAnimation)
        {
            foreach (var animationClip in easyAnimation.AnimationClips)
            {
                AnimationUtility.SetAnimationEvents(animationClip, null);
            }
        }
#endif
    }
}