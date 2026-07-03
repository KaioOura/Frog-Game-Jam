using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Menu: Chefrog / Fix Setting Button
/// 1. Lista todos os filhos de Canvas para identificar o painel de settings
/// 2. Adiciona ScreenButton no Setting_In_Game_Button
/// Depois de identificar o painel de settings no log, rode:
///   Chefrog / Wire Settings Panel   (segunda função abaixo)
/// e passe o nome exato do GameObject no campo do inspector.
/// </summary>
public static class FixSettingButton
{
    // ── Passo 1: descobrir nome do painel ─────────────────────────────────────
    [MenuItem("Chefrog/1 - List Canvas Children (achar painel settings)")]
    private static void ListCanvasChildren()
    {
        Debug.Log("═══ Canvas children (todos) ═══");
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (!go.scene.isLoaded) continue;
            if (go.GetComponent<Canvas>() == null) continue;

            Debug.Log($"[Canvas] {GetPath(go)}");
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                bool hasScreen = child.GetComponent<UIScreen>() != null;
                bool hasButton = child.GetComponent<ScreenButton>() != null;
                Debug.Log($"  └─ [{i}] \"{child.name}\"  ativo={child.gameObject.activeSelf}  UIScreen={hasScreen}  ScreenButton={hasButton}");
            }
        }
        Debug.Log("═══ FIM — anote o nome do painel de settings e rode o Passo 2 ═══");
    }

    // ── Passo 2: passar o nome do painel e fazer o fix ────────────────────────
    // EDITE o valor de settingsPanelName abaixo com o nome exato que apareceu no log acima
    private const string settingsPanelName = "PREENCHER_AQUI";

    [MenuItem("Chefrog/2 - Wire Setting_In_Game_Button + Settings Panel")]
    private static void WireSettingButton()
    {
        if (settingsPanelName == "PREENCHER_AQUI")
        {
            Debug.LogError("[FixSettingButton] Preencha 'settingsPanelName' com o nome do painel antes de rodar.");
            return;
        }

        // 1. ScreenButton no botão
        var btnGO = FindGO("Setting_In_Game_Button");
        if (btnGO != null)
        {
            var sb = btnGO.GetComponent<ScreenButton>() ?? Undo.AddComponent<ScreenButton>(btnGO);
            var sbSo = new SerializedObject(sb);
            sbSo.FindProperty("goBack").boolValue      = false;
            sbSo.FindProperty("target").enumValueIndex = (int)ScreenId.Settings;
            sbSo.ApplyModifiedProperties();
            EditorUtility.SetDirty(sb);
            Debug.Log($"✅ ScreenButton adicionado em Setting_In_Game_Button → target=Settings");
        }
        else
            Debug.LogError("❌ 'Setting_In_Game_Button' não encontrado. Verifique o nome exato.");

        // 2. UIScreen no painel de settings
        var panelGO = FindGO(settingsPanelName);
        if (panelGO != null)
        {
            var screen = panelGO.GetComponent<UIScreen>() ?? Undo.AddComponent<UIScreen>(panelGO);
            var sSo = new SerializedObject(screen);
            sSo.FindProperty("id").enumValueIndex    = (int)ScreenId.Settings;
            sSo.FindProperty("layer").enumValueIndex = (int)ScreenLayer.Overlay;
            sSo.ApplyModifiedProperties();
            EditorUtility.SetDirty(screen);
            Debug.Log($"✅ UIScreen(Settings, Overlay) adicionado em '{settingsPanelName}'");
        }
        else
            Debug.LogError($"❌ Painel '{settingsPanelName}' não encontrado.");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("✅ Salve a cena (Ctrl+S).");
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
