public interface INPCActivity
{
    void Start(NPCActor actor);
    void Tick(NPCActor actor, float dt);
    void Stop(NPCActor actor);
    bool IsFinished { get; }
}