using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu: Chefrog / Diagnose Screen System
/// Imprime no Console o estado atual do sistema de telas:
///   - Se ScreenManager existe e onde está
///   - Quais GameObjects têm UIScreen (id, layer)
///   - Quais GameObjects têm ScreenButton (target, goBack)
///   - Todos os GameObjects de UI na cena (para identificar painéis sem UIScreen)
/// </summary>
public static class DiagnoseScreenSystem
{
    [MenuItem("Chefrog/Diagnose Screen System")]
    private static void Run()
    {
        Debug.Log("═══ [DiagnoseScreenSystem] ════════════════════════════════");

        // ── ScreenManager ─────────────────────────────────────────────────────
        var sm = Object.FindFirstObjectByType<ScreenManager>(FindObjectsInactive.Include);
        if (sm != null)
            Debug.Log($"✅ ScreenManager encontrado em: {GetPath(sm.gameObject)}");
        else
            Debug.LogWarning("❌ ScreenManager NÃO encontrado na cena.");

        // ── UIScreen ──────────────────────────────────────────────────────────
        var screens = Object.FindObjectsByType<UIScreen>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (screens.Length == 0)
            Debug.LogWarning("❌ Nenhum UIScreen encontrado. Rode Chefrog > Wire Screen System.");
        else
        {
            Debug.Log($"✅ UIScreens encontrados ({screens.Length}):");
            foreach (var s in screens)
                Debug.Log($"   • {GetPath(s.gameObject)}  id={s.Id}  layer={s.Layer}  ativo={s.gameObject.activeSelf}");
        }

        // ── ScreenButton ──────────────────────────────────────────────────────
        var btns = Object.FindObjectsByType<ScreenButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (btns.Length == 0)
            Debug.LogWarning("⚠ Nenhum ScreenButton encontrado.");
        else
        {
            Debug.Log($"✅ ScreenButtons encontrados ({btns.Length}):");
            var so_type = typeof(ScreenButton);
            foreach (var b in btns)
            {
                var so = new SerializedObject(b);
                bool goBack = so.FindProperty("goBack").boolValue;
                int targetIdx = so.FindProperty("target").enumValueIndex;
                string target = ((ScreenId)targetIdx).ToString();
                Debug.Log($"   • {GetPath(b.gameObject)}  goBack={goBack}  target={target}");
            }
        }

        // ── Todos os GameObjects em Canvas (para identificar painéis não wired) ──
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"ℹ Canvases na cena ({canvases.Length}):");
        foreach (var canvas in canvases)
        {
            Debug.Log($"   Canvas: {GetPath(canvas.gameObject)}");
            // Filhos diretos do canvas
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                var child = canvas.transform.GetChild(i);
                var hasUIScreen = child.GetComponent<UIScreen>() != null;
                Debug.Log($"      └─ {child.name}  (ativo={child.gameObject.activeSelf})  UIScreen={hasUIScreen}");
            }
        }

        // ── ScoreUI / HealthUI ────────────────────────────────────────────────
        var scoreUI  = Object.FindFirstObjectByType<ScoreUI>(FindObjectsInactive.Include);
        var healthUI = Object.FindFirstObjectByType<HealthUI>(FindObjectsInactive.Include);
        Debug.Log(scoreUI  != null ? $"✅ ScoreUI em: {GetPath(scoreUI.gameObject)}"  : "❌ ScoreUI NÃO encontrado.");
        Debug.Log(healthUI != null ? $"✅ HealthUI em: {GetPath(healthUI.gameObject)}" : "❌ HealthUI NÃO encontrado.");

        // ── Setting_In_Game_Button ─────────────────────────────────────────────
        var settingBtn = FindGO("Setting_In_Game_Button");
        if (settingBtn != null)
        {
            var sb = settingBtn.GetComponent<ScreenButton>();
            Debug.Log(sb != null
                ? $"✅ Setting_In_Game_Button tem ScreenButton  target={((ScreenId)new SerializedObject(sb).FindProperty("target").enumValueIndex)}"
                : $"❌ Setting_In_Game_Button NÃO tem ScreenButton  path={GetPath(settingBtn)}");
        }
        else
            Debug.LogWarning("⚠ GameObject 'Setting_In_Game_Button' não encontrado pelo nome exato.");

        Debug.Log("═══ [DiagnoseScreenSystem] FIM ════════════════════════════");
    }

    private static GameObject FindGO(string name)
    {
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            if (go.scene.isLoaded && go.name == name) return go;
        return null;
    }

    private static string GetPath(GameObject go)
    {
        var path = go.name;
        var t = go.transform.parent;
        while (t != null) { path = t.name + "/" + path; t = t.parent; }
        return path;
    }
}
