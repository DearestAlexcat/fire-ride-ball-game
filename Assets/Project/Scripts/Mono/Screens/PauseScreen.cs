using Client;
using Leopotam.EcsLite;

public class PauseScreen : Screen
{
    public void Continue() // OnClick event
    {
        if (Service<RuntimeData>.Get().PrevGameState != GameState.TAPTOSTART)
        {
            Service<EcsWorld>.Get().ChangeState(GameState.PLAYING);
        }
        else
        {
            Service<EcsWorld>.Get().ChangeState(GameState.TAPTOSTART);
        }
    }

    public void Restart() // OnClick event
    {
        Levels.RestartLevel();
    }
}
