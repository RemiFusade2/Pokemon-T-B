/* 
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * AUTHOR: Rémi Fusade
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EventType
{
    EVENT,
    BATTLE
}

[System.Serializable]
public class EventOrBattle
{
    public EventType type;

    public bool exitGame;

    // Pokémon joins the team
    [Header("New Pokemon")]
    public bool newPokemonJoinsTheTeam;
    public bool pokemonFromLastBattle;
    public bool pokemonFromScratch;
    public int newPokemonID;
    public float newPokemonStarttHPRatio;
    public int newPokemonStarttLevel;
    public Relation newPokemonRelationWithPlayer;

    // Event
    [Header("Event")]
    public EventPanelBehaviour eventPanel;
    public bool instantTransition;

    // Battle
    [Header("Battle")]
    public bool enemyPokemonIsStarterNemesis;
    public int enemyPokemonID;
    public float enemyPokemonStarttHPRatio;
    public int enemyPokemonStarttLevel;
    public Relation enemyPokemonRelationWithPlayer;

    public EventPanelBehaviour eventPanelIfBattleIsWon;
    public EventPanelBehaviour eventPanelIfBattleIsLost;
}

/// <summary>
/// An Event Panel contains a picture and some text + some buttons.
/// A click on one of these buttons should call a method from this class.
/// This class will then deal with the next Event Panel, or start a new battle.
/// </summary>
public class EventPanelBehaviour : MonoBehaviour
{
    public Animator eventPanelAnimator;
    public Image eventPanelImage;

    public EventOrBattle nextEvent;
    public EventOrBattle option1Event;
    public EventOrBattle option2Event;
    public EventOrBattle option3Event;

    public EventEngineBehaviour eventEngine;
    public MainGameEngineBehaviour gameEngine;

    public AudioSource audioToStopAtEndOfEvent;
    public AudioSource audioToPlayAtStartOfEvent;

    public float delayBeforeShowingEvent;
    public float delayBeforePlayingAudio;
    public float delayBeforeStopingAudio;
    public bool forceNoFadeInOnAudio;

    private Pokemon lastFoughtPokemon;
	
	public void Show(bool instant)
    {
        if (audioToPlayAtStartOfEvent != null)
        {
            StartCoroutine(WaitAndPlayAudioSource(delayBeforePlayingAudio, audioToPlayAtStartOfEvent, forceNoFadeInOnAudio));
        }

        eventPanelImage.raycastTarget = true;
        eventPanelAnimator.SetBool("Instant", instant);
        StartCoroutine(WaitAndChangeEventVisibility(delayBeforeShowingEvent, true));
        MakeAllButtonsInteractable(true);
    }

    public void Hide(bool instant)
    {
        float delay = 0;
        if (audioToStopAtEndOfEvent != null)
        {
            StartCoroutine(WaitAndStopAudioSource(delayBeforeStopingAudio, audioToStopAtEndOfEvent, true));
        }

        eventPanelImage.raycastTarget = false;
        eventPanelAnimator.SetBool("Instant", instant);
        StartCoroutine(WaitAndChangeEventVisibility(delay, false));
    }

    private IEnumerator WaitAndPlayAudioSource(float delay, AudioSource source, bool noFadeIn)
    {
        yield return new WaitForSeconds(delay);
        try
        {
            audioToPlayAtStartOfEvent.Stop();
            audioToPlayAtStartOfEvent.Play();
            source.GetComponent<Animator>().SetBool("Instant", noFadeIn);
            source.GetComponent<Animator>().SetBool("Playing", true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("No Animator on AudioSource : " + audioToPlayAtStartOfEvent.name);
            Debug.LogError("Exception : " + ex.Message);
        }
    }

    private IEnumerator WaitAndStopAudioSource(float delay, AudioSource source, bool noFadeOut)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
    }

    private IEnumerator WaitAndChangeEventVisibility(float delay, bool visibility)
    {
        yield return new WaitForSeconds(delay);
        eventPanelAnimator.SetBool("Visible", visibility);
    }

    private void StartEvent(EventPanelBehaviour eventPanel, bool instant)
    {
        eventEngine.ShowEvent(eventPanel, instant);
    }

    private void StartBattle(int pkmnID, float pkmnHPRatio, int pkmnLevel, Relation relationWithPlayer)
    {
        Pokemon newEnemyPokemon = gameEngine.AddExistingPokemonFromID(pkmnID);
        newEnemyPokemon.LevelUP(pkmnLevel - newEnemyPokemon.level);
        newEnemyPokemon.HP = Mathf.CeilToInt( pkmnHPRatio * newEnemyPokemon.HPMax );
        newEnemyPokemon.relationWithPlayer = relationWithPlayer;
        lastFoughtPokemon = newEnemyPokemon;
        eventEngine.PlayBattleTheme();
        gameEngine.StartBattleAgainstPokemon(lastFoughtPokemon, 3.0f, 2.0f);
    }

    private void MakeAllButtonsInteractable(bool interactable)
    {
        Transform buttonsPanel = this.transform.Find("Buttons Panel");
        Button[] listOfButtons = buttonsPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < listOfButtons.Length; i++)
        {
            listOfButtons[i].interactable = interactable;
        }
    }

    private void StartEventOrBattle(EventOrBattle eventOrBattle)
    {
        MakeAllButtonsInteractable(false);
        if (eventOrBattle.newPokemonJoinsTheTeam)
        {
            if (eventOrBattle.pokemonFromScratch)
            {
                Pokemon newPlayerPokemon = gameEngine.AddExistingPokemonFromID(eventOrBattle.newPokemonID);
                newPlayerPokemon.LevelUP(eventOrBattle.newPokemonStarttLevel - newPlayerPokemon.level);
                newPlayerPokemon.HP = Mathf.CeilToInt(eventOrBattle.newPokemonStarttHPRatio * newPlayerPokemon.HPMax);
                newPlayerPokemon.relationWithPlayer = eventOrBattle.newPokemonRelationWithPlayer;
                gameEngine.CapturePokemon(newPlayerPokemon, null);
            }
            else if (eventOrBattle.pokemonFromLastBattle)
            {
                gameEngine.CapturePokemon(Pokemon.Copy(lastFoughtPokemon), null);
                lastFoughtPokemon = null;
            }
        }

        if (eventOrBattle.type == EventType.EVENT)
        {
            StartEvent(eventOrBattle.eventPanel, eventOrBattle.instantTransition);
        }
        else
        {
            if (audioToStopAtEndOfEvent != null)
            {
                StartCoroutine(WaitAndStopAudioSource(delayBeforeStopingAudio, audioToStopAtEndOfEvent, true));
            }

            int pokemonID = eventOrBattle.enemyPokemonID;
            Relation enemyPkmnRelationWithPlayer = eventOrBattle.enemyPokemonRelationWithPlayer;
            if (eventOrBattle.enemyPokemonIsStarterNemesis)
            {
                switch (gameEngine.listOfPokemonsOwnedByPlayer[0].code)
                {
                    case PokemonCode.CHARMANANDER:
                        pokemonID = 2;
                        enemyPkmnRelationWithPlayer = new Relation() { love = 0, trust = -0.5f, fear = 0 };
                        break;
                    case PokemonCode.SQUIRTLITULLI:
                        pokemonID = 3;
                        enemyPkmnRelationWithPlayer = new Relation() { love = 0.3f, trust = 0, fear = -0.6f };
                        break;
                    case PokemonCode.BULBAZABAZAUR:
                        pokemonID = 1;
                        enemyPkmnRelationWithPlayer = new Relation() { love = -0.5f, trust = 0, fear = 0.5f };
                        break;
                }
            }
            StartBattle(pokemonID, eventOrBattle.enemyPokemonStarttHPRatio, eventOrBattle.enemyPokemonStarttLevel, enemyPkmnRelationWithPlayer);
            eventEngine.SetEventsAfterBattle(eventOrBattle.eventPanelIfBattleIsWon, eventOrBattle.eventPanelIfBattleIsLost);
            eventEngine.HideAllEvents(3.0f);
        }

        if (eventOrBattle.exitGame)
        {
            Application.Quit();
        }
    }

    public void ClickOnContinue()
    {
        StartEventOrBattle(nextEvent);
    }
    public void ClickOnOption1()
    {
        StartEventOrBattle(option1Event);
    }
    public void ClickOnOption2()
    {
        StartEventOrBattle(option2Event);
    }
    public void ClickOnOption3()
    {
        StartEventOrBattle(option3Event);
    }
}
