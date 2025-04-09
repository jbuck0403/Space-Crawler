public class CollisionAwareMovementController : BaseMovementController
{
    public new CollisionAwareMovementHandler GetMovementHandler()
    {
        return (CollisionAwareMovementHandler)base.GetMovementHandler();
    }
}
