namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    public sealed class SceneDataReference : BaseReference<SceneData, SceneDataVariable>
    {
        public SceneDataReference() : base() { }
        public SceneDataReference(SceneData value) : base(value) { }
    }
}