#!/usr/bin/env bash
# Rode na RAIZ do projeto Unity, nos DOIS PCs (via Git Bash):  bash fingerprint.sh
# Depois compare os arquivos gerados (fingerprint_<hostname>.txt) entre os PCs.
OUT="fingerprint_$(hostname).txt"
{
  echo "===================== FINGERPRINT ====================="
  echo "PC (hostname) : $(hostname)"
  echo "Data          : $(date)"
  echo
  echo "----- Unity version (tem que ser IDENTICO) -----"
  cat ProjectSettings/ProjectVersion.txt 2>/dev/null
  echo
  echo "----- Git (commit + working tree) -----"
  echo "Commit: $(git rev-parse HEAD 2>/dev/null)"
  echo "Branch: $(git rev-parse --abbrev-ref HEAD 2>/dev/null)"
  echo "--- git status (TEM QUE ESTAR VAZIO nos dois) ---"
  git status --short
  echo
  echo "----- Color Space (1=Linear) -----"
  grep m_ActiveColorSpace ProjectSettings/ProjectSettings.asset
  echo
  echo "----- Graphics APIs por plataforma -----"
  grep -nE "m_BuildTargetGraphicsAPIs|m_APIs:|m_UseDefaultGraphicsAPIs" ProjectSettings/ProjectSettings.asset
  echo
  echo "----- Quality tier ativo -----"
  grep m_CurrentQuality ProjectSettings/QualitySettings.asset
  echo
  echo "----- URP assets: HDR / grading / LUT -----"
  for f in High Medium Low; do
    echo "[$f]"
    grep -E "m_SupportsHDR:|m_ColorGradingMode:|m_ColorGradingLutSize:" "Assets/Settings/UniversalRP-${f}Quality.asset"
  done
  echo
  echo "----- Cameras da cena Gameplay (PP / tipo / renderer / volume mask) -----"
  grep -nE "m_CameraType:|m_RenderPostProcessing:|m_RendererIndex:|m_Bits:|m_Cameras:" "Assets/Scenes/Gameplay.unity"
  echo
  echo "----- Renderer Features -----"
  grep -nE "m_RendererFeatures:" Assets/Settings/ForwardRenderer.asset Assets/Settings/UIRenderer.asset 2>/dev/null
  echo "======================================================="
} | tee "$OUT"
echo
echo ">>> Gerado: $OUT  (compare este arquivo entre os dois PCs)"
