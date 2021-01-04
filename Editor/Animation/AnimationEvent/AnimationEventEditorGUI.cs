namespace FrameworkEditor
{
    public abstract class AnimationEventEditorGUI
    {
        protected AnimationEventEditorWindowInfo EditorWindowInfo { get; private set; }

        public virtual void OnGUI(AnimationEventEditorWindowInfo editorWindowInfo)
        {
            EditorWindowInfo = editorWindowInfo;
        }
    }
}