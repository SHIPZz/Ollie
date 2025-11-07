using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;
using Zenject;

namespace Code.Infrastructure.States.Factory
{
    public class StateFactory : IStateFactory
    {
        private readonly IInstantiator _instantiator;
        
        public StateFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public T CreateState<T>() where T : class, IState
        {
            return _instantiator.Instantiate<T>();
        }
    }
}