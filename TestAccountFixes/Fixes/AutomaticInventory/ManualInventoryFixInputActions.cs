using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace TestAccountFixes.Fixes.AutomaticInventory;

public class ManualInventoryFixInputActions : LcInputActions {
    [InputAction(KeyboardControl.Comma, Name = "ManualInventoryFix")]
    public InputAction? ManualInventoryFixKey { get; set; }
}