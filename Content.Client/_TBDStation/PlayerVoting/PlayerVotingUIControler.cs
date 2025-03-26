using Content.Shared.PlayerVoting;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client._TBDStation.PlayerVoting;
using Content.Client.UserInterface.Systems.Character;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Network;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;

namespace Content.Client._TBDStation.PlayerVoting;

[UsedImplicitly]
public sealed partial class PlayerVotingMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
    [Dependency] private readonly IClientNetManager _net = default!;
    private MenuButton? PlayerVotingButton => UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.OpenPlayerVotingMenu;
    // Don't clear this window. It needs to be saved so the input doesn't get erased when it's closed!
    private PlayerVotingMenu _playerVotingMenu = default!;
    public override void Initialize(){}
    public void OnStateEntered(GameplayState state)
    {
        _playerVotingMenu = UIManager.CreateWindow<PlayerVotingMenu>();
        _playerVotingMenu.UpdateUI();
    }

    public void OnStateExited(GameplayState state)
    {
        _playerVotingMenu.Close();
    }
    private void ToggleWindow()
    {
        if (_playerVotingMenu.IsOpen)
        {
            _playerVotingMenu.Close();

            return;
        }

        _playerVotingMenu.OpenCentered();
    }
    public void LoadButton()
    {
        if (PlayerVotingButton == null)
            return;

        PlayerVotingButton.OnPressed += ButtonToggleWindow;
    }
    public void UnloadButton()
    {
        if (PlayerVotingButton == null)
            return;

        PlayerVotingButton.OnPressed -= ButtonToggleWindow;
    }
    private void ButtonToggleWindow(BaseButton.ButtonEventArgs obj)
    {
        ToggleWindow();
    }
}
