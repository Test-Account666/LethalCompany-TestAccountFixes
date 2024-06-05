using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace TestAccountFixes.Fixes.SpiderDamage.Transpilers;

[HarmonyPatch(typeof(SandSpiderAI))]
public static class SandSpiderAITranspiler {
    [HarmonyPatch(nameof(SandSpiderAI.OnCollideWithPlayer))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AdjustSpooderDamageValue(IEnumerable<CodeInstruction> instructions) {
        foreach (var codeInstruction in instructions) {
            if (codeInstruction.opcode != OpCodes.Ldc_I4_S) {
                yield return codeInstruction;
                continue;
            }

            yield return new(OpCodes.Call, AccessTools.Method(typeof(SpiderDamageFix), nameof(SpiderDamageFix.GetSpooderDamage)));
        }
    }
}