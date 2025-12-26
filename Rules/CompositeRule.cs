using System;
using antunity.GameData;
using UnityEngine;

namespace antunity.GameSystems.Rules
{
    public enum LogicalOperation { AND, OR }

    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct CompositeRuleStruct : IRule, IUseGameDataDrawer
    {
        [SerializeField] private Rule rule1;

        [SerializeField] private LogicalOperation operation;

        [SerializeField] private Rule rule2;

        [SerializeField] private bool invert;

        public RuleResult Evaluate(IGameContext context)
        {
            RuleResult result;

            switch (operation)
            {
                case LogicalOperation.AND:
                    result = rule1.Evaluate(context) && rule2.Evaluate(context);
                    break;
                case LogicalOperation.OR:
                    result = rule1.Evaluate(context) || rule2.Evaluate(context);
                    break;
                default:
                    result = RuleResult.UnknownFailure();
                    break;
            }

            if (invert)
                result = !result;

            return result;
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_COMPOSITE, menuName = MENU_PATH.RULE_COMPOSITE)]
    public class CompositeRule : Rule
    {
        [SerializeField] private CompositeRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}