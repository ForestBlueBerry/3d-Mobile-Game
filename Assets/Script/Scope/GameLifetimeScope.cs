using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<InputService>(Lifetime.Singleton)
            .As<IInputService>()
            .As<ITickable>()
            ;
        builder.RegisterComponentInHierarchy<ControllerVechicle>();
        builder.RegisterComponentInHierarchy<ControlAutoturret>();
        builder.RegisterComponentInHierarchy<GameStateService>();
        builder.RegisterComponentInHierarchy<EnemySpawner>();
        builder.RegisterComponentInHierarchy<UIOverlayView>();
        builder.RegisterComponentInHierarchy<FinishTrigger>();
    }
}
