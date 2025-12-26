using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct HasDataRuleStruct : IRule
    {
        [SerializeField] private bool invert;

        [SerializeField] private GameDataSource source;

        [SerializeField] private GameDataAsset<uint> data;

        public GameDataSource Source => source;

        public IGameDataBase Data => data;

        public RuleResult Evaluate(IGameContext context)
        {
            var resultRaw = context.Resolve<bool>(source, data);
            var result = invert ? !resultRaw : resultRaw;
            return result ? RuleResult.Success() : RuleResult.DataPresenceViolation(data, resultRaw ? 1f : 0f);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_HAS_DATA, menuName = MENU_PATH.RULE_HAS_DATA)]
    public class HasDataRule : Rule
    {
        [SerializeField] private HasDataRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}