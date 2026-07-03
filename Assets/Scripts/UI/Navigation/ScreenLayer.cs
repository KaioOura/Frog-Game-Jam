/// <summary>
/// Camada de uma tela.
/// Root: só uma ativa por vez; abrir uma Root fecha a Root anterior e limpa a pilha de overlays.
/// Overlay: abre por cima da Root atual (que continua ativa por baixo) e entra na pilha de histórico.
/// </summary>
public enum ScreenLayer
{
    Root,
    Overlay
}
