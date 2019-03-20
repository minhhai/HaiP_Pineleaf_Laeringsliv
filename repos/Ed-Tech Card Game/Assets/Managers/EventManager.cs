using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class for organizing all subscription based events. Use this class as a middleman for all such calls
/// </summary>
public static class EventManager {


    public static Action<int> inspectStatElement;
    public static Action inspectDropdownMenuElement;
    


    public static void InspectStatElement(int i) {
        inspectStatElement?.Invoke(i);
    }

    public static void InspectMenuDropdownElement() {
        inspectDropdownMenuElement?.Invoke();
    }
        
}
