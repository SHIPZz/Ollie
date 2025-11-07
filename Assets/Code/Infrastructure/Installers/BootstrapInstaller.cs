using Code.Features.Common.Time;
using Code.Features.Holidays.Services;
using Code.Infrastructure.Loading;
using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.States.States;
using Zenject;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.UI.Windows;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        public override void InstallBindings()
        {
            Container.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
            
            Container.Bind<IAssetsService>().To<AssetsService>().AsSingle();
            Container.Bind<IConfigService>().To<ConfigService>().AsSingle();
            Container.Bind<IHolidayService>().To<HolidayService>().AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.BindInterfacesTo<WindowsRegistry>().AsSingle();
            Container.BindInterfacesTo<StateFactory>().AsSingle();
            Container.BindInterfacesTo<SceneLoadService>().AsSingle();

            Container.BindInterfacesTo<AppStateMachine>().AsSingle();
            
            Container.BindInterfacesTo(GetType()).FromInstance(this).AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IStateMachine>().EnterAsync<StartupState>();
        }
    }
}
