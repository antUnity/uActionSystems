using System;
using UnityEngine;
using uGameData;

namespace uActionSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct HasDataRule : IRule
    {
        [SerializeField] private bool invert;

        [SerializeField] private ContextSource source;

        [SerializeField] private GameDataAsset<uint> data;

        public ContextSource Source => source;

        public IGameDataBase Data => data;

        public RuleResult Evaluate(IActionContext context)
        {
            var resultRaw = context.Resolve<bool>(source, data);
            var result = invert ? !resultRaw : resultRaw;
            return result ? RuleResult.Success() : RuleResult.DataPresenceViolation(data, resultRaw ? 1f : 0f);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_HAS_DATA, menuName = MENU_PATH.RULE_HAS_DATA)]
    public class HasDataRuleAsset : RuleAsset
    {
        [SerializeField] private HasDataRule rule;

        public override RuleResult Evaluate(IActionContext context) => rule.Evaluate(context);
    }
}