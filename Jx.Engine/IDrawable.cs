namespace Jx.Engine
{
    public interface IDrawable
    {
        bool IsDrawing { get; }
        void Draw(ITickEvent tickEvent);
    }
}