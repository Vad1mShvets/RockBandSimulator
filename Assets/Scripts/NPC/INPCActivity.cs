public interface INPCActivity
{
    void Start(NPCActor actor);
    void Tick(NPCActor actor, float deltaTime);
    void Stop(NPCActor actor);
    bool IsFinished { get; }
}