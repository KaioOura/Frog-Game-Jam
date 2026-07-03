using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menu: Chefrog / Wire Screen System
/// Roda UMA VEZ na cena Gameplay e faz todo o wiring do novo sistema de telas:
///   - Cria ScreenManager num GameObject "ScreenManager"
///   - Adiciona UIScreen nos painéis (Menu, Game, PostGame, SecondChance, PauseUI)
///   - Adiciona ScoreUI e HealthUI (move os campos TMP / lifeImages do antigo UIManager)
///   - Adiciona ScreenButton nos botões que o script conseguir identificar por nome
///   - Reponta GameManager (ScoreUI, HealthUI, LeaderboardUI, MobileInputUI, BellyDisplayUI)
/// Depois de rodar: salve a cena (Ctrl+S) e delete este arquivo.
/// </summary>
public class WireScreenSystem : MonoBehaviour
{
    [MenuItem("Chefrog/Wire Screen System")]
    private static void Run()
    {
        var scene = EditorSceneManager.GetActiveScene();
        Debug.Log($"[WireScreenSystem] Iniciando wiring na cena: {scene.name}");

        // ── 1. Encontrar GameManager ──────────────────────────────────────────
        var gameManager = FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
        if (gameManager == null) { Debug.LogError("[WireScreenSystem] GameManager não encontrado. Abra a cena Gameplay."); return; }

        // ── 2. Encontrar painéis pelo nome (ajuste aqui se os nomes forem diferentes) ──
        // Nomes esperados dos GameObjects raiz de cada tela
        var menuGO       = FindGO("menu")       ?? FindGO("Menu");
        var gameGO       = FindGO("game")        ?? FindGO("Game");
        var postGameGO   = FindGO("postGame")    ?? FindGO("PostGame")    ?? FindGO("GameOver");
        var secondChGO   = FindGO("secondChance") ?? FindGO("SecondChance");
        var pauseGO      = FindGO("pauseUI")     ?? FindGO("PauseUI")     ?? FindGO("Pause");

        LogFound("menu",        menuGO);
        LogFound("game",        gameGO);
        LogFound("postGame",    postGameGO);
        LogFound("secondChance",secondChGO);
        LogFound("pauseUI",     pauseGO);

        // ── 3. Criar / encontrar GameObject "ScreenManager" ──────────────────
        var smGO = GameObject.Find("ScreenManager");
        if (smGO == null)
        {
            smGO = new GameObject("ScreenManager");
            Undo.RegisterCreatedObjectUndo(smGO, "Create ScreenManager");
            Debug.Log("[WireScreenSystem] GameObject 'ScreenManager' criado.");
        }

        var screenManager = smGO.GetComponent<ScreenManager>();
        if (screenManager == null)
        {
            screenManager = Undo.AddComponent<ScreenManager>(smGO);
            Debug.Log("[WireScreenSystem] Componente ScreenManager adicionado.");
        }

        // initialScreen = Menu (valor 0 no enum)
        var smSo = new SerializedObject(screenManager);
        smSo.FindProperty("initialScreen").enumValueIndex = (int)ScreenId.Menu;
        smSo.ApplyModifiedProperties();

        // ── 4. Adicionar UIScreen em cada painel ─────────────────────────────
        AddUIScreen(menuGO,     ScreenId.Menu,         ScreenLayer.Root);
        AddUIScreen(gameGO,     ScreenId.Game,         ScreenLayer.Root);
        AddUIScreen(postGameGO, ScreenId.PostGame,     ScreenLayer.Root);
        AddUIScreen(secondChGO, ScreenId.SecondChance, ScreenLayer.Overlay);
        AddUIScreen(pauseGO,    ScreenId.Pause,        ScreenLayer.Overlay);
        // onOpen/onClose do Pause precisam ser wired manualmente no inspector:
        // onOpen  → GameFlowManager.PauseGame(true)
        // onClose → GameFlowManager.PauseGame(false)
        if (pauseGO != null)
            Debug.LogWarning("[WireScreenSystem] PENDENTE MANUAL: UIScreen do Pause → onOpen: GameFlowManager.PauseGame(true) / onClose: GameFlowManager.PauseGame(false)");

        // ── 5. Criar ScoreUI ─────────────────────────────────────────────────
        var scoreUIGO = GameObject.Find("ScoreUI");
        if (scoreUIGO == null)
        {
            scoreUIGO = new GameObject("ScoreUI");
            Undo.RegisterCreatedObjectUndo(scoreUIGO, "Create ScoreUI GO");
            // Colocar dentro do canvas Game se existir
            if (gameGO != null) scoreUIGO.transform.SetParent(gameGO.transform, false);
        }
        var scoreUI = scoreUIGO.GetComponent<ScoreUI>() ?? Undo.AddComponent<ScoreUI>(scoreUIGO);
        Debug.Log("[WireScreenSystem] ScoreUI pronto. PENDENTE MANUAL: arrastar os campos TMP (currentScore, finalScore, secondChanceScore, finalHighScore, secondChanceHighScore) no inspector do ScoreUI.");

        // ── 6. Criar HealthUI ─────────────────────────────────────────────────
        var healthUIGO = GameObject.Find("HealthUI");
        if (healthUIGO == null)
        {
            healthUIGO = new GameObject("HealthUI");
            Undo.RegisterCreatedObjectUndo(healthUIGO, "Create HealthUI GO");
            if (gameGO != null) healthUIGO.transform.SetParent(gameGO.transform, false);
        }
        var healthUI = healthUIGO.GetComponent<HealthUI>() ?? Undo.AddComponent<HealthUI>(healthUIGO);
        Debug.Log("[WireScreenSystem] HealthUI pronto. PENDENTE MANUAL: arrastar os campos lifeImages no inspector do HealthUI.");

        // ── 7. Repontar GameManager ───────────────────────────────────────────
        var gmSo = new SerializedObject(gameManager);

        SetObjectRef(gmSo, "<ScoreUI>k__BackingField",   scoreUI);
        SetObjectRef(gmSo, "<HealthUI>k__BackingField",  healthUI);

        // LeaderboardUI, MobileInputUI, BellyDisplayUI — procurar na cena
        var leaderboardUI = FindFirstObjectByType<LeaderboardUI>(FindObjectsInactive.Include);
        var mobileInputUI = FindFirstObjectByType<MobileInputUI>(FindObjectsInactive.Include);
        var bellyDisplayUI = FindFirstObjectByType<BellyDisplayUI>(FindObjectsInactive.Include);

        if (leaderboardUI)  SetObjectRef(gmSo, "<LeaderboardUI>k__BackingField",  leaderboardUI);
        if (mobileInputUI)  SetObjectRef(gmSo, "<MobileInputUI>k__BackingField",  mobileInputUI);
        if (bellyDisplayUI) SetObjectRef(gmSo, "<BellyDisplayUI>k__BackingField", bellyDisplayUI);

        gmSo.ApplyModifiedProperties();
        EditorUtility.SetDirty(gameManager);

        Debug.Log("[WireScreenSystem] GameManager repontado: ScoreUI, HealthUI" +
            (leaderboardUI  ? ", LeaderboardUI"  : " (LeaderboardUI NÃO encontrado — arraste no inspector)") +
            (mobileInputUI  ? ", MobileInputUI"  : " (MobileInputUI NÃO encontrado — arraste no inspector)") +
            (bellyDisplayUI ? ", BellyDisplayUI" : " (BellyDisplayUI NÃO encontrado — arraste no inspector)"));

        // ── 8. Adicionar ScreenButton em botões conhecidos ───────────────────
        WireButton("BtnStart",   ScreenId.Game,    false);
        WireButton("BtnPlay",    ScreenId.Game,    false);
        WireButton("BtnPause",   ScreenId.Pause,   false);
        WireButton("BtnResume",  ScreenId.Pause,   true);   // goBack
        WireButton("BtnBack",    ScreenId.Menu,    true);   // goBack
        WireButton("BtnVoltar",  ScreenId.Menu,    true);   // goBack
        WireButton("BtnMenu",    ScreenId.Menu,    false);
        WireButton("BtnRestart", ScreenId.Menu,    false);

        // ── 9. Salvar cena ────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[WireScreenSystem] ✅ Wiring concluído! Verifique os avisos PENDENTE MANUAL acima e salve a cena (Ctrl+S).");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void AddUIScreen(GameObject go, ScreenId id, ScreenLayer layer)
    {
        if (go == null) return;
        var screen = go.GetComponent<UIScreen>() ?? Undo.AddComponent<UIScreen>(go);
        var so = new SerializedObject(screen);
        so.FindProperty("id").enumValueIndex    = (int)id;
        so.FindProperty("layer").enumValueIndex = (int)layer;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(screen);
        Debug.Log($"[WireScreenSystem] UIScreen adicionado: {go.name} → id={id}, layer={layer}");
    }

    private static void WireButton(string goName, ScreenId target, bool goBack)
    {
        var go = FindGO(goName);
        if (go == null) return;
        var btn = go.GetComponent<Button>();
        if (btn == null) btn = go.GetComponentInChildren<Button>();
        if (btn == null) { Debug.LogWarning($"[WireScreenSystem] '{goName}' encontrado mas sem Button component."); return; }

        var sb = go.GetComponent<ScreenButton>() ?? Undo.AddComponent<ScreenButton>(go);
        var so = new SerializedObject(sb);
        so.FindProperty("goBack").boolValue      = goBack;
        so.FindProperty("target").enumValueIndex = (int)target;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(sb);
        Debug.Log($"[WireScreenSystem] ScreenButton: {goName} → goBack={goBack}, target={target}");
    }

    private static void SetObjectRef(SerializedObject so, string propName, Object obj)
    {
        var prop = so.FindProperty(propName);
        if (prop != null) { prop.objectReferenceValue = obj; }
        else Debug.LogWarning($"[WireScreenSystem] Propriedade não encontrada no GameManager: '{propName}'");
    }

    private static GameObject FindGO(string name)
    {
        // Procura inclusive em inativos
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.scene.isLoaded && go.name == name)
                return go;
        }
        return null;
    }

    private static void LogFound(string label, GameObject go) =>
        Debug.Log(go != null
            ? $"[WireScreenSystem] '{label}' → {go.name} (path: {GetPath(go)})"
            : $"[WireScreenSystem] ⚠ '{label}' NÃO encontrado pelo nome. Adicione UIScreen manualmente.");

    private static string GetPath(GameObject go)
    {
        var path = go.name;
        var t = go.transform.parent;
        while (t != null) { path = t.name + "/" + path; t = t.parent; }
        return path;
    }
}
