using Content.Server.Explosion.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server.Explosion.EntitySystems;

public sealed partial class TriggerSystem
{
    private void InitializeTimedCollide()
    {
        SubscribeLocalEvent<TriggerOnTimedCollideComponent, StartCollideEvent>(OnTimerCollide);
        SubscribeLocalEvent<TriggerOnTimedCollideComponent, EndCollideEvent>(OnTimerEndCollide);
        SubscribeLocalEvent<TriggerOnTimedCollideComponent, ComponentRemove>(OnComponentRemove);
    }

    private void OnTimerCollide(EntityUid uid, TriggerOnTimedCollideComponent component, ref StartCollideEvent args)
    {
        //Ensures the entity trigger will have an active component
        EnsureComp<ActiveTriggerOnTimedCollideComponent>(uid);
        var otherUID = args.OtherEntity;
        if (component.Colliding.ContainsKey(otherUID))
            return;
        component.Colliding.Add(otherUID, 0);
    }

    private void OnTimerEndCollide(EntityUid uid, TriggerOnTimedCollideComponent component, ref EndCollideEvent args)
    {
        var otherUID = args.OtherEntity;
        component.Colliding.Remove(otherUID);

        if (component.Colliding.Count == 0 && HasComp<ActiveTriggerOnTimedCollideComponent>(uid))
            RemComp<ActiveTriggerOnTimedCollideComponent>(uid);
    }

    private void OnComponentRemove(EntityUid uid, TriggerOnTimedCollideComponent component, ComponentRemove args)
    {
        if (HasComp<ActiveTriggerOnTimedCollideComponent>(uid))
            RemComp<ActiveTriggerOnTimedCollideComponent>(uid);
    }

    private void UpdateTimedCollide(float frameTime)
    {
        var query = EntityQueryEnumerator<ActiveTriggerOnTimedCollideComponent, TriggerOnTimedCollideComponent>();
        while (query.MoveNext(out var uid, out _, out var triggerOnTimedCollide))
        {
            foreach (var (collidingEntity, collidingTimer) in triggerOnTimedCollide.Colliding)
            {
                triggerOnTimedCollide.Colliding[collidingEntity] += frameTime;
                if (collidingTimer > triggerOnTimedCollide.Threshold)
                {
                    // Goobstation start
                    triggerOnTimedCollide.Colliding[collidingEntity] -= triggerOnTimedCollide.Threshold;
                    var attemptEv = new TriggerAttemptEvent(uid, collidingEntity);
                    RaiseLocalEvent(uid, attemptEv, true);
                    if (attemptEv.Cancelled)
                        continue;
                    RaiseLocalEvent(uid, new TriggerEvent(uid, collidingEntity), true);
                    // Goobstation end
                }
            }
        }
    }
}
