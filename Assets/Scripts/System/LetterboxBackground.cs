using UnityEngine;

/// <summary>
/// Marcador para a câmera de fundo que pinta a tela inteira de preto atrás do
/// jogo. O <see cref="LetterboxController"/> ignora câmeras com este componente,
/// para que elas mantenham o viewport cheio (0,0,1,1) e cubram as barras.
///
/// Configuração esperada na câmera: Depth bem baixo (ex.: -100),
/// Clear Flags = Solid Color (preto, alpha 1), Culling Mask = Nothing.
/// </summary>
[RequireComponent(typeof(Camera))]
public class LetterboxBackground : MonoBehaviour
{
}
