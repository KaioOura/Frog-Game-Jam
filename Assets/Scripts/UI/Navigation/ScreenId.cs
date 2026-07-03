/// <summary>
/// Identificador de cada tela do jogo. Usado pelos botões (via ScreenButton)
/// e pelo ScreenManager para abrir/registrar telas. Aparece como dropdown no inspector.
/// Para adicionar uma tela nova, basta acrescentar um valor aqui.
/// </summary>
public enum ScreenId
{
    Menu,
    Game,
    Pause,
    PostGame,
    SecondChance,
    Settings,
    Book,
    Credits,
    QuitConfirm
}
