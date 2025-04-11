using UnityEngine;
using Zenject;

public class ChatInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Bind our LogService implementation
        Container.Bind<ILogService>().To<LogService>().AsSingle();
        
        // Bind ChatService for future AI integration
        Container.Bind<IChatService>().To<SimpleChatService>().AsSingle();
        
        Debug.Log("Chat dependencies installed via Zenject");
    }
}
