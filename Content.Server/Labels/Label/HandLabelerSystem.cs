using Content.Shared.Labels.EntitySystems;
using Content.Server._TBDStation.SlurFilter; // TBDStation

namespace Content.Server.Labels.Label;

public sealed class HandLabelerSystem : SharedHandLabelerSystem
{
    [Dependency] private readonly SlurFilterManager _slurFilterMan = default!; // TBDStation

    public override bool ContainsSlur(string label) // TBDStation
    {
        var containsSlur = _slurFilterMan.ContainsSlur(label);
        return containsSlur;
    }
}
