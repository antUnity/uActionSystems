using System;
using uGameData;

namespace uActionSystems
{
    public enum ContextSource
    {
        Environment,    // e.g. ability book, inventory owner
        Instigator,     // e.g. an external trigger for an action, e.g. a customer in a vendor transaction. if omitted, it matches the environment
        Subject,        // e.g. ability, item, ability
    }

    public interface IQueryable
    {
        T Query<T>(IGameDataBase data);
    }

    public interface IActionContext : IGameDataBase
    {
        IQueryable Instigator { get; set; }

        IQueryable Subject { get; set; }

        IQueryable Environment { get; set; }

        T Resolve<T>(ContextSource source, IGameDataBase gameData);
    }

    public class DefaultActionContext<TAction> : GameData<TAction>, IActionContext where TAction : struct
    {
        public DefaultActionContext(TAction index) : base(index) { }

        #region IActionContext

        public IQueryable Instigator { get; set; } = null;

        public IQueryable Subject { get; set; } = null;

        public IQueryable Environment { get; set; } = null;

        public TResult Resolve<TResult>(ContextSource source, IGameDataBase data)
        {
            IQueryable sourceTarget;
            switch (source)
            {
                case ContextSource.Environment:
                    sourceTarget = Environment;
                    break;
                case ContextSource.Instigator:
                    sourceTarget = Instigator;
                    break;
                case ContextSource.Subject:
                    sourceTarget = Subject;
                    break;
                default:
                    throw new NotSupportedException(nameof(source));
            }

            if (sourceTarget == null)
                throw new NullReferenceException($"{Index}: {nameof(ContextSource)} '{source}' is not specified in this context.");

            return sourceTarget.Query<TResult>(data);
        }

        #endregion IActionContext
    }
}