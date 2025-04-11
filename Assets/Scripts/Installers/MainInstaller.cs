using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Bind services
        Container.Bind<ILogService>().To<LogService>().AsSingle();
        
        Debug.Log("Zenject bindings installed successfully");
    }
}
