using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Game_State : MonoBehaviour
{

    public Button zoom_Added_Button;
    public Button zoom_Decrease_Button;

    public Button quit_Button;
    public Button quit_Confrim_Button;
    public Button quit_Cancel_Button;

    List<Button> buttons = new List<Button>();

    public enum Gamestate
    {
        UnKown = -1,
        Fire_start,
        Zoom_Added,
        Zoom_Decrease,
        Reload,
        Breath_start,
        Breath_end,
        Quit,
        Quitconfrim,
        Quitcancel
    }

    private Gamestate game_state = Gamestate.UnKown;

    private void Awake()
    {
        Gather_button();
    }
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button_name = buttons[buttons.IndexOf(buttons[i])].name;

            buttons[i].onClick.AddListener(delegate () { State_Change(button_name); });
        }
    }   

    public void Gather_button()
    {
        buttons.Add(zoom_Added_Button);
        buttons.Add(zoom_Decrease_Button);
        buttons.Add(quit_Button);

        buttons.Add(quit_Confrim_Button);
        buttons.Add(quit_Cancel_Button);
    }

    public void State_Change(string button_name)//状态转换
    {
        switch (button_name)
        {
            case "Fire_Button_Down":
                game_state = Gamestate.Fire_start;
                break;
            case "Zoom_Added_Button":
                game_state = Gamestate.Zoom_Added;
                break;
            case "zoom_Decrease_Button":
                game_state = Gamestate.Zoom_Decrease;
                break;
            case "Reload_Button":
                game_state = Gamestate.Reload;
                break;
            case "Breath_Button_down":
                game_state = Gamestate.Breath_start;
                break;
            case "Breath_Button_up":
                game_state = Gamestate.Breath_end;
                break;
            case "Quit_Button":
                game_state = Gamestate.Quit;
                break;
            case "Quit_confrim_Button":
                game_state = Gamestate.Quitconfrim;
                break;
            default:
                game_state = Gamestate.Quitcancel;
                break;
        }
        switch (game_state)
        {
            case Gamestate.UnKown:
                break;
            case Gamestate.Fire_start:
                UI_Management.ui_Management.Fire();
                break;
            case Gamestate.Zoom_Added:
                UI_Management.ui_Management.Zoom_Added();
                break;
            case Gamestate.Zoom_Decrease:
                UI_Management.ui_Management.Zoom_Decrease();
                break;
            case Gamestate.Reload:
                UI_Management.ui_Management.Reload();
                break;
            case Gamestate.Breath_start:
                UI_Management.ui_Management.Down_breath_Button();
                break;
            case Gamestate.Breath_end:
                UI_Management.ui_Management.UP_breath_Button();
                break;
            case Gamestate.Quit:
                UI_Management.ui_Management.Quit();
                break;
            case Gamestate.Quitconfrim:
                UI_Management.ui_Management.Quit_Confrim();
                break;
            default:
                UI_Management.ui_Management.Quit_Cancel();
                break;
        }
    }
}
