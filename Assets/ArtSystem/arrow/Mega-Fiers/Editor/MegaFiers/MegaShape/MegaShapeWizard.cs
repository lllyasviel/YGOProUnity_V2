
using UnityEngine;
using UnityEditor;

#if false
public class MegaShapeWizard : ScriptableWizard
{
public float range = 500;
public Color color = Color.red;

[MenuItem ("GameObject/Create Light Wizard")]
static void CreateWizard () {
ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create", "Apply");
//If you don't want to use the secondary button simply leave it out:
//ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
}
void OnWizardCreate () {
GameObject go = new GameObject ("New Light");
go.AddComponent("Light");
go.light.range = range;
go.light.color = color;
}
void OnWizardUpdate () {
helpString = "Please set the color of the light!";
}
// When the user pressed the "Apply" button OnWizardOtherButton is called.
void OnWizardOtherButton () {
if (Selection.activeTransform == null ||
Selection.activeTransform.light == null) return;
Selection.activeTransform.light.color = Color.red;
}
#endif