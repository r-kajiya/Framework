#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    internal enum HighLevelEvent
    {
        None,
        Click,
        DoubleClick,
        ContextClick,
        BeginDrag,
        Drag,
        EndDrag,
        Delete,
        SelectionChanged,
    }

    internal class EditorGUIExt
    {
        private static EditorGUIExt.Styles ms_Styles = new EditorGUIExt.Styles();
        private static int repeatButtonHash = "repeatButton".GetHashCode();
        private static float nextScrollStepTime = 0.0f;
        private static int firstScrollWait = 250;
        private static int scrollWait = 30;
        private static int kFirstScrollWait = 250;
        private static int kScrollWait = 30;
        private static DateTime s_NextScrollStepTime = DateTime.Now;
        private static Vector2 s_MouseDownPos = Vector2.zero;

        private static EditorGUIExt.DragSelectionState
            s_MultiSelectDragSelection = EditorGUIExt.DragSelectionState.None;

        private static Vector2 s_StartSelectPos = Vector2.zero;
        private static List<bool> s_SelectionBackup = (List<bool>) null;
        private static List<bool> s_LastFrameSelections = (List<bool>) null;
        internal static int s_MinMaxSliderHash = "MinMaxSlider".GetHashCode();
        private static bool adding = false;
        private static int initIndex = 0;
        private static int scrollControlID;
        private static EditorGUIExt.MinMaxSliderState s_MinMaxSliderState;
        private static bool[] initSelections;

        private static bool DoRepeatButton(
            Rect position,
            GUIContent content,
            GUIStyle style,
            FocusType focusType)
        {
            int controlId = GUIUtility.GetControlID(EditorGUIExt.repeatButtonHash, focusType, position);
            switch (Event.current.GetTypeForControl(controlId))
            {
                case UnityEngine.EventType.MouseDown:
                    if (position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }

                    return false;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl != controlId)
                        return false;
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    return position.Contains(Event.current.mousePosition);
                case UnityEngine.EventType.Repaint:
                    style.Draw(position, content, controlId, false, position.Contains(Event.current.mousePosition));
                    return controlId == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
                default:
                    return false;
            }
        }

        private static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
        {
            bool flag1 = false;
            if (EditorGUIExt.DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
            {
                bool flag2 = EditorGUIExt.scrollControlID != scrollerID;
                EditorGUIExt.scrollControlID = scrollerID;
                if (flag2)
                {
                    flag1 = true;
                    EditorGUIExt.nextScrollStepTime =
                        Time.realtimeSinceStartup + 1f / 1000f * (float) EditorGUIExt.firstScrollWait;
                }
                else if ((double) Time.realtimeSinceStartup >= (double) EditorGUIExt.nextScrollStepTime)
                {
                    flag1 = true;
                    EditorGUIExt.nextScrollStepTime =
                        Time.realtimeSinceStartup + 1f / 1000f * (float) EditorGUIExt.scrollWait;
                }

                if (Event.current.type == UnityEngine.EventType.Repaint)
                    HandleUtility.Repaint();
            }

            return flag1;
        }

        public static void MinMaxScroller(
            Rect position,
            int id,
            ref float value,
            ref float size,
            float visualStart,
            float visualEnd,
            float startLimit,
            float endLimit,
            GUIStyle slider,
            GUIStyle thumb,
            GUIStyle leftButton,
            GUIStyle rightButton,
            bool horiz)
        {
            float num1 = !horiz ? size * 10f / position.height : size * 10f / position.width;
            Rect position1;
            Rect rect1;
            Rect rect2;
            if (horiz)
            {
                position1 = new Rect(position.x + leftButton.fixedWidth, position.y,
                    position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
                rect1 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth,
                    position.height);
            }
            else
            {
                position1 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width,
                    position.height - leftButton.fixedHeight - rightButton.fixedHeight);
                rect1 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width,
                    rightButton.fixedHeight);
            }

            float num2 = Mathf.Min(visualStart, value);
            float num3 = Mathf.Max(visualEnd, value + size);
            EditorGUIExt.MinMaxSlider(position1, ref value, ref size, num2, num3, num2, num3, slider, thumb, horiz);
            bool flag = false;
            if (Event.current.type == UnityEngine.EventType.MouseUp)
                flag = true;
            if (EditorGUIExt.ScrollerRepeatButton(id, rect1, leftButton))
                value -= num1 * ((double) visualStart < (double) visualEnd ? 1f : -1f);
            if (EditorGUIExt.ScrollerRepeatButton(id, rect2, rightButton))
                value += num1 * ((double) visualStart < (double) visualEnd ? 1f : -1f);
            if (flag && Event.current.type == UnityEngine.EventType.Used)
                EditorGUIExt.scrollControlID = 0;
            if ((double) startLimit < (double) endLimit)
                value = Mathf.Clamp(value, startLimit, endLimit - size);
            else
                value = Mathf.Clamp(value, endLimit, startLimit - size);
        }

        public static void MinMaxSlider(
            Rect position,
            ref float value,
            ref float size,
            float visualStart,
            float visualEnd,
            float startLimit,
            float endLimit,
            GUIStyle slider,
            GUIStyle thumb,
            bool horiz)
        {
            EditorGUIExt.DoMinMaxSlider(position,
                GUIUtility.GetControlID(EditorGUIExt.s_MinMaxSliderHash, FocusType.Passive), ref value, ref size,
                visualStart, visualEnd, startLimit, endLimit, slider, thumb, horiz);
        }

        private static float ThumbSize(bool horiz, GUIStyle thumb)
        {
            return horiz
                ? ((double) thumb.fixedWidth != 0.0 ? thumb.fixedWidth : (float) thumb.padding.horizontal)
                : ((double) thumb.fixedHeight != 0.0 ? thumb.fixedHeight : (float) thumb.padding.vertical);
        }

        internal static void DoMinMaxSlider(
            Rect position,
            int id,
            ref float value,
            ref float size,
            float visualStart,
            float visualEnd,
            float startLimit,
            float endLimit,
            GUIStyle slider,
            GUIStyle thumb,
            bool horiz)
        {
            Event current = Event.current;
            bool flag = (double) size == 0.0;
            float min1 = Mathf.Min(visualStart, visualEnd);
            float max = Mathf.Max(visualStart, visualEnd);
            float min2 = Mathf.Min(startLimit, endLimit);
            float num1 = Mathf.Max(startLimit, endLimit);
            EditorGUIExt.MinMaxSliderState minMaxSliderState = EditorGUIExt.s_MinMaxSliderState;
            if (GUIUtility.hotControl == id && minMaxSliderState != null)
            {
                min1 = minMaxSliderState.dragStartLimit;
                min2 = minMaxSliderState.dragStartLimit;
                max = minMaxSliderState.dragEndLimit;
                num1 = minMaxSliderState.dragEndLimit;
            }

            float num2 = 0.0f;
            float num3 = Mathf.Clamp(value, min1, max);
            float num4 = Mathf.Clamp(value + size, min1, max) - num3;
            float num5 = (double) visualStart > (double) visualEnd ? -1f : 1f;
            if (slider == null || thumb == null)
                return;
            Rect rect = thumb.margin.Remove(slider.padding.Remove(position));
            float num6 = EditorGUIExt.ThumbSize(horiz, thumb);
            float num7;
            Rect position1;
            Rect position2;
            Rect position3;
            float num8;
            if (horiz)
            {
                float height = (double) thumb.fixedHeight != 0.0 ? thumb.fixedHeight : rect.height;
                num7 = (float) (((double) position.width - (double) slider.padding.horizontal - (double) num6) /
                                ((double) max - (double) min1));
                position1 = new Rect((num3 - min1) * num7 + rect.x, rect.y, num4 * num7 + num6, height);
                position2 = new Rect(position1.x, position1.y, (float) thumb.padding.left, position1.height);
                position3 = new Rect(position1.xMax - (float) thumb.padding.right, position1.y,
                    (float) thumb.padding.right, position1.height);
                num8 = current.mousePosition.x - position.x;
            }
            else
            {
                float width = (double) thumb.fixedWidth != 0.0 ? thumb.fixedWidth : rect.width;
                num7 = (float) (((double) position.height - (double) slider.padding.vertical - (double) num6) /
                                ((double) max - (double) min1));
                position1 = new Rect(rect.x, (num3 - min1) * num7 + rect.y, width, num4 * num7 + num6);
                position2 = new Rect(position1.x, position1.y, position1.width, (float) thumb.padding.top);
                position3 = new Rect(position1.x, position1.yMax - (float) thumb.padding.bottom, position1.width,
                    (float) thumb.padding.bottom);
                num8 = current.mousePosition.y - position.y;
            }

            switch (current.GetTypeForControl(id))
            {
                case UnityEngine.EventType.MouseDown:
                    if (current.button != 0 || !position.Contains(current.mousePosition) ||
                        (double) min1 - (double) max == 0.0)
                        break;
                    if (minMaxSliderState == null)
                        minMaxSliderState = EditorGUIExt.s_MinMaxSliderState = new EditorGUIExt.MinMaxSliderState();
                    minMaxSliderState.dragStartLimit = startLimit;
                    minMaxSliderState.dragEndLimit = endLimit;
                    if (position1.Contains(current.mousePosition))
                    {
                        minMaxSliderState.dragStartPos = num8;
                        minMaxSliderState.dragStartValue = value;
                        minMaxSliderState.dragStartSize = size;
                        minMaxSliderState.dragStartValuesPerPixel = num7;
                        minMaxSliderState.whereWeDrag = !position2.Contains(current.mousePosition)
                            ? (!position3.Contains(current.mousePosition) ? 0 : 2)
                            : 1;
                        GUIUtility.hotControl = id;
                        current.Use();
                        break;
                    }

                    if (slider == GUIStyle.none)
                        break;
                    if ((double) size != 0.0 & flag)
                    {
                        if (horiz)
                        {
                            if ((double) num8 > (double) position1.xMax - (double) position.x)
                                value += (float) ((double) size * (double) num5 * 0.8999999761581421);
                            else
                                value -= (float) ((double) size * (double) num5 * 0.8999999761581421);
                        }
                        else if ((double) num8 > (double) position1.yMax - (double) position.y)
                            value += (float) ((double) size * (double) num5 * 0.8999999761581421);
                        else
                            value -= (float) ((double) size * (double) num5 * 0.8999999761581421);

                        minMaxSliderState.whereWeDrag = 0;
                        GUI.changed = true;
                        EditorGUIExt.s_NextScrollStepTime =
                            DateTime.Now.AddMilliseconds((double) EditorGUIExt.kFirstScrollWait);
                        float num9 = horiz ? current.mousePosition.x : current.mousePosition.y;
                        float num10 = horiz ? position1.x : position1.y;
                        minMaxSliderState.whereWeDrag = (double) num9 > (double) num10 ? 4 : 3;
                    }
                    else
                    {
                        value = !horiz
                            ? (float) (((double) num8 - (double) position1.height * 0.5) / (double) num7 +
                                (double) min1 - (double) size * 0.5)
                            : (float) (((double) num8 - (double) position1.width * 0.5) / (double) num7 +
                                (double) min1 - (double) size * 0.5);
                        minMaxSliderState.dragStartPos = num8;
                        minMaxSliderState.dragStartValue = value;
                        minMaxSliderState.dragStartSize = size;
                        minMaxSliderState.dragStartValuesPerPixel = num7;
                        minMaxSliderState.whereWeDrag = 0;
                        GUI.changed = true;
                    }

                    GUIUtility.hotControl = id;
                    value = Mathf.Clamp(value, min2, num1 - size);
                    current.Use();
                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl != id)
                        break;
                    current.Use();
                    GUIUtility.hotControl = 0;
                    break;
                case UnityEngine.EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                        break;
                    float num11 = (num8 - minMaxSliderState.dragStartPos) / minMaxSliderState.dragStartValuesPerPixel;
                    switch (minMaxSliderState.whereWeDrag)
                    {
                        case 0:
                            value = Mathf.Clamp(minMaxSliderState.dragStartValue + num11, min2, num1 - size);
                            break;
                        case 1:
                            value = minMaxSliderState.dragStartValue + num11;
                            size = minMaxSliderState.dragStartSize - num11;
                            if ((double) value < (double) min2)
                            {
                                size -= min2 - value;
                                value = min2;
                            }

                            if ((double) size < (double) num2)
                            {
                                value -= num2 - size;
                                size = num2;
                                break;
                            }

                            break;
                        case 2:
                            size = minMaxSliderState.dragStartSize + num11;
                            if ((double) value + (double) size > (double) num1)
                                size = num1 - value;
                            if ((double) size < (double) num2)
                            {
                                size = num2;
                                break;
                            }

                            break;
                    }

                    GUI.changed = true;
                    current.Use();
                    break;
                case UnityEngine.EventType.Repaint:
                    slider.Draw(position, GUIContent.none, id);
                    thumb.Draw(position1, GUIContent.none, id);
                    EditorGUIUtility.AddCursorRect(position2,
                        horiz ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical,
                        minMaxSliderState == null || minMaxSliderState.whereWeDrag != 1 ? -1 : id);
                    EditorGUIUtility.AddCursorRect(position3,
                        horiz ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical,
                        minMaxSliderState == null || minMaxSliderState.whereWeDrag != 2 ? -1 : id);
                    if (GUIUtility.hotControl != id || !position.Contains(current.mousePosition) ||
                        (double) min1 - (double) max == 0.0)
                        break;
                    if (position1.Contains(current.mousePosition))
                    {
                        if (minMaxSliderState == null ||
                            minMaxSliderState.whereWeDrag != 3 && minMaxSliderState.whereWeDrag != 4)
                            break;
                        GUIUtility.hotControl = 0;
                        break;
                    }

                    if (DateTime.Now < EditorGUIExt.s_NextScrollStepTime)
                        break;
                    int num12 = (horiz ? (double) current.mousePosition.x : (double) current.mousePosition.y) >
                                (horiz ? (double) position1.x : (double) position1.y)
                        ? 4
                        : 3;
                    if (minMaxSliderState != null && num12 != minMaxSliderState.whereWeDrag)
                        break;
                    if ((double) size != 0.0 & flag)
                    {
                        if (horiz)
                        {
                            if ((double) num8 > (double) position1.xMax - (double) position.x)
                                value += (float) ((double) size * (double) num5 * 0.8999999761581421);
                            else
                                value -= (float) ((double) size * (double) num5 * 0.8999999761581421);
                        }
                        else if ((double) num8 > (double) position1.yMax - (double) position.y)
                            value += (float) ((double) size * (double) num5 * 0.8999999761581421);
                        else
                            value -= (float) ((double) size * (double) num5 * 0.8999999761581421);

                        if (minMaxSliderState != null)
                            minMaxSliderState.whereWeDrag = -1;
                        GUI.changed = true;
                    }

                    value = Mathf.Clamp(value, min2, num1 - size);
                    EditorGUIExt.s_NextScrollStepTime = DateTime.Now.AddMilliseconds((double) EditorGUIExt.kScrollWait);
                    break;
            }
        }

        public static bool DragSelection(Rect[] positions, ref bool[] selections, GUIStyle style)
        {
            int controlId = GUIUtility.GetControlID(34553287, FocusType.Keyboard);
            Event current = Event.current;
            int b = -1;
            for (int index = positions.Length - 1; index >= 0; --index)
            {
                if (positions[index].Contains(current.mousePosition))
                {
                    b = index;
                    break;
                }
            }

            switch (current.GetTypeForControl(controlId))
            {
                case UnityEngine.EventType.MouseDown:
                    if (current.button == 0 && b >= 0)
                    {
                        GUIUtility.keyboardControl = 0;
                        bool flag1 = false;
                        if (selections[b])
                        {
                            int num = 0;
                            foreach (bool flag2 in selections)
                            {
                                if (flag2)
                                {
                                    ++num;
                                    if (num > 1)
                                        break;
                                }
                            }

                            if (num == 1)
                                flag1 = true;
                        }

                        if (!current.shift && !EditorGUI.actionKey)
                        {
                            for (int index = 0; index < positions.Length; ++index)
                                selections[index] = false;
                        }

                        EditorGUIExt.initIndex = b;
                        EditorGUIExt.initSelections = (bool[]) selections.Clone();
                        EditorGUIExt.adding = true;
                        if ((current.shift || EditorGUI.actionKey) && selections[b])
                            EditorGUIExt.adding = false;
                        selections[b] = !flag1 && EditorGUIExt.adding;
                        GUIUtility.hotControl = controlId;
                        current.Use();
                        return true;
                    }

                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        break;
                    }

                    break;
                case UnityEngine.EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId && current.button == 0)
                    {
                        if (b < 0)
                        {
                            Rect rect = new Rect(positions[0].x, positions[0].y - 200f, positions[0].width, 200f);
                            if (rect.Contains(current.mousePosition))
                                b = 0;
                            rect.y = positions[positions.Length - 1].yMax;
                            if (rect.Contains(current.mousePosition))
                                b = selections.Length - 1;
                        }

                        if (b < 0)
                            return false;
                        int num1 = Mathf.Min(EditorGUIExt.initIndex, b);
                        int num2 = Mathf.Max(EditorGUIExt.initIndex, b);
                        for (int index = 0; index < selections.Length; ++index)
                        {
                            int num3 = index < num1 ? 0 : (index <= num2 ? 1 : 0);
                            selections[index] = num3 == 0 ? EditorGUIExt.initSelections[index] : EditorGUIExt.adding;
                        }

                        current.Use();
                        return true;
                    }

                    break;
                case UnityEngine.EventType.Repaint:
                    for (int index = 0; index < positions.Length; ++index)
                        style.Draw(positions[index], GUIContent.none, controlId, selections[index]);
                    break;
            }

            return false;
        }

        private static bool Any(bool[] selections)
        {
            for (int index = 0; index < selections.Length; ++index)
            {
                if (selections[index])
                    return true;
            }

            return false;
        }

        public static HighLevelEvent MultiSelection(
            Rect rect,
            Rect[] positions,
            GUIContent content,
            Rect[] hitPositions,
            ref bool[] selections,
            bool[] readOnly,
            out int clickedIndex,
            out Vector2 offset,
            out float startSelect,
            out float endSelect,
            GUIStyle style)
        {
            int controlId = GUIUtility.GetControlID(41623453, FocusType.Keyboard);
            Event current = Event.current;
            offset = Vector2.zero;
            clickedIndex = -1;
            startSelect = endSelect = 0.0f;
            if (current.type == UnityEngine.EventType.Used)
                return HighLevelEvent.None;
            bool flag1 = false;
            if (Event.current.type != UnityEngine.EventType.Layout && GUIUtility.keyboardControl == controlId)
                flag1 = true;
            switch (current.GetTypeForControl(controlId))
            {
                case UnityEngine.EventType.MouseDown:
                    if (current.button == 0)
                    {
                        GUIUtility.hotControl = controlId;
                        GUIUtility.keyboardControl = controlId;
                        EditorGUIExt.s_StartSelectPos = current.mousePosition;
                        int indexUnderMouse = EditorGUIExt.GetIndexUnderMouse(hitPositions, readOnly);
                        if (Event.current.clickCount == 2 && indexUnderMouse >= 0)
                        {
                            for (int index = 0; index < selections.Length; ++index)
                                selections[index] = false;
                            selections[indexUnderMouse] = true;
                            current.Use();
                            clickedIndex = indexUnderMouse;
                            return HighLevelEvent.DoubleClick;
                        }

                        if (indexUnderMouse >= 0)
                        {
                            if (!current.shift && !EditorGUI.actionKey && !selections[indexUnderMouse])
                            {
                                for (int index = 0; index < hitPositions.Length; ++index)
                                    selections[index] = false;
                            }

                            int num = current.shift ? 1 : (EditorGUI.actionKey ? 1 : 0);
                            selections[indexUnderMouse] = num == 0 || !selections[indexUnderMouse];
                            EditorGUIExt.s_MouseDownPos = current.mousePosition;
                            EditorGUIExt.s_MultiSelectDragSelection = EditorGUIExt.DragSelectionState.None;
                            current.Use();
                            clickedIndex = indexUnderMouse;
                            return HighLevelEvent.SelectionChanged;
                        }

                        bool flag2;
                        if (!current.shift && !EditorGUI.actionKey)
                        {
                            for (int index = 0; index < hitPositions.Length; ++index)
                                selections[index] = false;
                            flag2 = true;
                        }
                        else
                            flag2 = false;

                        EditorGUIExt.s_SelectionBackup = new List<bool>((IEnumerable<bool>) selections);
                        EditorGUIExt.s_LastFrameSelections = new List<bool>((IEnumerable<bool>) selections);
                        EditorGUIExt.s_MultiSelectDragSelection = EditorGUIExt.DragSelectionState.DragSelecting;
                        current.Use();
                        return flag2 ? HighLevelEvent.SelectionChanged : HighLevelEvent.None;
                    }

                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        if (EditorGUIExt.s_StartSelectPos != current.mousePosition)
                            current.Use();
                        if (EditorGUIExt.s_MultiSelectDragSelection == EditorGUIExt.DragSelectionState.None)
                        {
                            clickedIndex = EditorGUIExt.GetIndexUnderMouse(hitPositions, readOnly);
                            if (current.clickCount == 1)
                                return HighLevelEvent.Click;
                            break;
                        }

                        EditorGUIExt.s_MultiSelectDragSelection = EditorGUIExt.DragSelectionState.None;
                        EditorGUIExt.s_SelectionBackup = (List<bool>) null;
                        EditorGUIExt.s_LastFrameSelections = (List<bool>) null;
                        return HighLevelEvent.EndDrag;
                    }

                    break;
                case UnityEngine.EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId)
                    {
                        if (EditorGUIExt.s_MultiSelectDragSelection == EditorGUIExt.DragSelectionState.DragSelecting)
                        {
                            float num1 = Mathf.Min(EditorGUIExt.s_StartSelectPos.x, current.mousePosition.x);
                            float num2 = Mathf.Max(EditorGUIExt.s_StartSelectPos.x, current.mousePosition.x);
                            EditorGUIExt.s_SelectionBackup.CopyTo(selections);
                            for (int index = 0; index < hitPositions.Length; ++index)
                            {
                                if (!selections[index])
                                {
                                    float num3 = hitPositions[index].x + hitPositions[index].width * 0.5f;
                                    if ((double) num3 >= (double) num1 && (double) num3 <= (double) num2)
                                        selections[index] = true;
                                }
                            }

                            current.Use();
                            startSelect = num1;
                            endSelect = num2;
                            bool flag2 = false;
                            for (int index = 0; index < selections.Length; ++index)
                            {
                                if (selections[index] != EditorGUIExt.s_LastFrameSelections[index])
                                {
                                    flag2 = true;
                                    EditorGUIExt.s_LastFrameSelections[index] = selections[index];
                                }
                            }

                            return flag2 ? HighLevelEvent.SelectionChanged : HighLevelEvent.None;
                        }

                        offset = current.mousePosition - EditorGUIExt.s_MouseDownPos;
                        current.Use();
                        if (EditorGUIExt.s_MultiSelectDragSelection != EditorGUIExt.DragSelectionState.None)
                            return HighLevelEvent.Drag;
                        EditorGUIExt.s_MultiSelectDragSelection = EditorGUIExt.DragSelectionState.Dragging;
                        return HighLevelEvent.BeginDrag;
                    }

                    break;
                case UnityEngine.EventType.KeyDown:
                    if (flag1 && (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete))
                    {
                        current.Use();
                        return HighLevelEvent.Delete;
                    }

                    break;
                case UnityEngine.EventType.Repaint:
                    if (GUIUtility.hotControl == controlId && EditorGUIExt.s_MultiSelectDragSelection ==
                        EditorGUIExt.DragSelectionState.DragSelecting)
                    {
                        float num1 = Mathf.Min(EditorGUIExt.s_StartSelectPos.x, current.mousePosition.x);
                        float num2 = Mathf.Max(EditorGUIExt.s_StartSelectPos.x, current.mousePosition.x);
                        Rect position = new Rect(0.0f, 0.0f, rect.width, rect.height);
                        position.x = num1;
                        position.width = num2 - num1;
                        if ((double) position.width > 1.0)
                            GUI.Box(position, "", EditorGUIExt.ms_Styles.selectionRect);
                    }

                    Color color = GUI.color;
                    for (int index = 0; index < positions.Length; ++index)
                    {
                        GUI.color = readOnly == null || !readOnly[index]
                            ? (!selections[index]
                                ? color * new Color(0.9f, 0.9f, 0.9f, 1f)
                                : color * new Color(0.3f, 0.55f, 0.95f, 1f))
                            : color * new Color(0.9f, 0.9f, 0.9f, 0.5f);
                        style.Draw(positions[index], content, controlId, selections[index]);
                    }

                    GUI.color = color;
                    break;
                case UnityEngine.EventType.ValidateCommand:
                case UnityEngine.EventType.ExecuteCommand:
                    if (flag1)
                    {
                        bool flag2 = current.type == UnityEngine.EventType.ExecuteCommand;
                        if (current.commandName == "Delete")
                        {
                            current.Use();
                            if (flag2)
                                return HighLevelEvent.Delete;
                        }

                        break;
                    }

                    break;
                case UnityEngine.EventType.ContextClick:
                    int indexUnderMouse1 = EditorGUIExt.GetIndexUnderMouse(hitPositions, readOnly);
                    if (indexUnderMouse1 >= 0)
                    {
                        clickedIndex = indexUnderMouse1;
                        GUIUtility.keyboardControl = controlId;
                        current.Use();
                        return HighLevelEvent.ContextClick;
                    }

                    break;
            }

            return HighLevelEvent.None;
        }

        private static int GetIndexUnderMouse(Rect[] hitPositions, bool[] readOnly)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            for (int index = hitPositions.Length - 1; index >= 0; --index)
            {
                if ((readOnly == null || !readOnly[index]) && hitPositions[index].Contains(mousePosition))
                    return index;
            }

            return -1;
        }

        internal static Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if ((double) rect.width < 0.0)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }

            if ((double) rect.height < 0.0)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }

            return rect;
        }

        private class Styles
        {
            public GUIStyle selectionRect = (GUIStyle) "SelectionRect";
        }

        private class MinMaxSliderState
        {
            public float dragStartPos = 0.0f;
            public float dragStartValue = 0.0f;
            public float dragStartSize = 0.0f;
            public float dragStartValuesPerPixel = 0.0f;
            public float dragStartLimit = 0.0f;
            public float dragEndLimit = 0.0f;
            public int whereWeDrag = -1;
        }

        private enum DragSelectionState
        {
            None,
            DragSelecting,
            Dragging,
        }
        
        static readonly int s_SliderHash = "Slider".GetHashCode();
        static int s_ScrollControlId;

        public static float Scroller(
            Rect position,
            float value,
            float size,
            float leftValue,
            float rightValue,
            GUIStyle slider,
            GUIStyle thumb,
            GUIStyle leftButton,
            GUIStyle rightButton,
            bool horiz)
        {
            int controlId = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
            Rect position1;
            Rect rect1;
            Rect rect2;
            if (horiz)
            {
                position1 = new Rect(position.x + leftButton.fixedWidth, position.y,
                    position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
                rect1 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth,
                    position.height);
            }
            else
            {
                position1 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width,
                    position.height - leftButton.fixedHeight - rightButton.fixedHeight);
                rect1 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width,
                    rightButton.fixedHeight);
            }

            value = GUI.Slider(position1, value, size, leftValue, rightValue, slider, thumb, horiz, controlId,
                (GUIStyle) null);
            bool flag = Event.current.type == EventType.MouseUp;
            if (ScrollerRepeatButton(controlId, rect1, leftButton))
                value -= (float) (10.0 * ((double) leftValue < (double) rightValue ? 1.0 : -1.0));
            if (ScrollerRepeatButton(controlId, rect2, rightButton))
                value += (float) (10.0 * ((double) leftValue < (double) rightValue ? 1.0 : -1.0));
            if (flag && Event.current.type == EventType.Used)
                s_ScrollControlId = 0;
            value = (double) leftValue >= (double) rightValue
                ? Mathf.Clamp(value, rightValue, leftValue - size)
                : Mathf.Clamp(value, leftValue, rightValue - size);
            return value;
        }
    }
}

#endif