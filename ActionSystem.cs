using uGameData;
using UnityEngine;
using uActionSystems.Rules;

namespace uActionSystems
{
    public interface IActionSystemBase
    {
        public IQueryable Environment { get; set; }

        public void Initialize(SystemTemplate template);
    }

    public interface IActionSystem<TAction> : IActionSystemBase where TAction : struct
    {
        public RuleResult EvaluateAction(TAction action, IQueryable subject, IQueryable instigator = null);

        public IActionContext GetActionContext(TAction action);

        public void SetActionContext(IActionContext context);

        public void SetActionRule(TAction action, RuleAsset rule);
    }

    public abstract class ActionSystem<TAction> : MonoBehaviour, IActionSystem<TAction> where TAction : struct
    {
        private GameDataRegistry<IActionContext> actionContexts = new();

        [SerializeField] private EnumDataValues<TAction, RuleAsset> rules = new();

        #region IActionSystemBase

        public IQueryable Environment { get; set; }

        public abstract void Initialize(SystemTemplate template);

        #endregion IActionSystemBase

        #region IActionSystem

        public RuleResult EvaluateAction(TAction action, IQueryable subject, IQueryable instigator = null)
        {
            if (!rules.ContainsIndex(action))
                return RuleResult.Success();

            var actionRule = rules[action];

            if (!actionRule)
                return RuleResult.Success();

            if (!actionContexts.TryGetData(action, out IActionContext context))
            {
                context = new DefaultActionContext<TAction>(action);
                actionContexts.Add(context);
            }

            // Default to the system's environment
            context.Environment ??= Environment;

            // Assign subject
            context.Subject = subject;

            // Assign the instigator if defined
            if (instigator != null)
                context.Instigator = instigator;
            else
                context.Instigator = context.Environment;
    
            return actionRule.Evaluate(context);
        }

        public IActionContext GetActionContext(TAction action) => actionContexts.TryGetData(action, out var context) ? context : default;

        public void SetActionContext(IActionContext context) => actionContexts.Add(context);

        public void SetActionRule(TAction action, RuleAsset rule)
        {
            if (rules.ContainsIndex(action))
                rules[action] = rule;
            else
                rules.Add(action, rule);
        }

        #endregion IActionSystem
    }
}