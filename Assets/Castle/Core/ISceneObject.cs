namespace Castle.Core
{
    public interface ISceneObject
    {
        void Init(int i);
        void ObjectStart();
    }
    public interface ISceneObjectWithUpdate : ISceneObject
    {
        void ObjectUpdate();
    }
}