using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class KeyBindRefs
{

    public static Dictionary<KeyConfig, List<string>> DefaultBindsRefs
    {
        get
        {
            var temp = new Dictionary<KeyConfig, List<string>>();
            temp.Add(new KeyConfig(KeyCode.Mouse0), new List<string> { "MouseLeftClick" });
            temp.Add(new KeyConfig(KeyCode.Tab), new List<string> { "KeyBindGUI" });
            temp.Add(new KeyConfig("Steering"), new List<string> { "Steering" });
            temp.Add(new KeyConfig("Brake"), new List<string> { "Brake" });

            return temp;
        }
    }

    public static Dictionary<string, ActionConfig> DefaultBinds
    {
        get
        {
            var temp = new Dictionary<string, ActionConfig>();
            temp.Add("MouseLeftClick", new KeyActionConfig(KeyType.Action, 0, null, null));
            temp.Add("KeyBindGUI", new KeyActionConfig(KeyType.Menu, 0, null, null));
            temp.Add("Steering", new AxisActionConfig(KeyType.Movement, 0, null));
            temp.Add("Brake", new AxisActionConfig(KeyType.Movement, 0, null));

            return temp;
        }
    }

    public static List<string> Axes
    {
        get
        {
            return new List<string>
            {   
				"MouseX",
				"MouseY",
                "Horizontal",
                "Vertical",
                "Horizontal_1",
                "Vertical_1",
                "Horizontal_2",
                "Vertical_2",
                "TriggerLeft",
                "TriggerRight",
                "Steering",
                "Brake"
            };
        }
    }

	public static string ChangingKey = "";
	public static KeyConfig LastInput;

}
