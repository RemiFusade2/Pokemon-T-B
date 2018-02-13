/* 
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * AUTHOR: Rémi Fusade
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manage the visibility status of Event Panels.
/// </summary>
public class EventEngineBehaviour : MonoBehaviour
{
    public Transform parentOfAllEvents;

    public EventPanelBehaviour startingEvent;

    private EventPanelBehaviour activeEventPanel;

    private bool inBattle;
    private EventPanelBehaviour nextEventIfBattleIsWon;
    private EventPanelBehaviour nextEventIfBattleIsLost;

    public AudioSource battleThemeAudioSource;

    public BattleOverlayPanelBehaviour overlayPanel;

    private Dictionary<string, Coroutine> dicoOfHideEventCoroutines;

    // Use this for initialization
    void Start()
    {
        dicoOfHideEventCoroutines = new Dictionary<string, Coroutine>();

        foreach (Transform eventParent in parentOfAllEvents)
        {
            eventParent.gameObject.SetActive(true);
            foreach (Transform child in eventParent)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ShowFirstEvent()
    {
        activeEventPanel = startingEvent;
        startingEvent.gameObject.SetActive(true);
        startingEvent.Show(false);
    }


    public void ShowEvent(EventPanelBehaviour eventPanel, bool instant)
    {
        // hide last event
        activeEventPanel.Hide(instant);
        if (dicoOfHideEventCoroutines.ContainsKey(activeEventPanel.name))
        {
            StopCoroutine(dicoOfHideEventCoroutines[activeEventPanel.name]);
            dicoOfHideEventCoroutines.Remove(activeEventPanel.name);
        }
        Coroutine hideEventCoroutine = StartCoroutine(WaitAndSetEventInactive(1.0f, activeEventPanel));
        dicoOfHideEventCoroutines.Add(activeEventPanel.name, hideEventCoroutine);

        // show new event
        if (eventPanel != null)
        {
            if (dicoOfHideEventCoroutines.ContainsKey(eventPanel.name))
            {
                StopCoroutine(dicoOfHideEventCoroutines[eventPanel.name]);
                dicoOfHideEventCoroutines.Remove(eventPanel.name);
            }
            eventPanel.gameObject.SetActive(true);
            eventPanel.Show(instant);
        }

        activeEventPanel = eventPanel;
    }

    public IEnumerator WaitAndSetEventInactive(float delay, EventPanelBehaviour eventPanel)
    {
        yield return new WaitForSeconds(delay);
        eventPanel.gameObject.SetActive(false);
    }

    public void ShowEventAfterBattle(bool battleIsWon, float delay)
    {
        StopBattleTheme();
        if (battleIsWon)
        {
            StartCoroutine(WaitAndShowEvent(delay, nextEventIfBattleIsWon, false));
        }
        else
        {
            StartCoroutine(WaitAndShowEvent(delay, nextEventIfBattleIsLost, false));
        }
    }

    private IEnumerator WaitAndShowEvent(float delay, EventPanelBehaviour eventPanel, bool instant)
    {
        yield return new WaitForSeconds(delay);
        ShowEvent(eventPanel, instant);
    }

    public void SetEventsAfterBattle(EventPanelBehaviour ifBattleIsWon, EventPanelBehaviour ifBattleIsLost)
    {
        this.nextEventIfBattleIsWon = ifBattleIsWon;
        this.nextEventIfBattleIsLost = ifBattleIsLost;
    }

    public void HideAllEvents(float delay)
    {
        StartCoroutine(WaitAndShowEventsParent(delay, false));
    }
    public void ShowAllEvents(float delay)
    {
        overlayPanel.HideBattleOverlay();
        StartCoroutine(WaitAndShowEventsParent(delay, true));
    }

    private IEnumerator WaitAndShowEventsParent(float delay, bool parentActive)
    {
        yield return new WaitForSeconds(delay);
        parentOfAllEvents.gameObject.SetActive(parentActive);
    }

    public void PlayBattleTheme()
    {
        battleThemeAudioSource.Play();
    }
    public void StopBattleTheme()
    {
        battleThemeAudioSource.Stop();
    }
}
