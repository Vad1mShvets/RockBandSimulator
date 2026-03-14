using System.Collections.Generic;

[System.Serializable]
public class NPCRoutineStep
{
    public NPCActivitySpotGroup SpotGroup;

    public string Animation;

    public float Duration = 10f;

    public void Build(NPCActor actor, Queue<INPCActivity> plan)
    {
        if (SpotGroup.RequestFree(actor, out var spot))
        {
            plan.Enqueue(new UseSpotActivity(
                spot,
                Duration,
                Animation
            ));
        }
    }
}