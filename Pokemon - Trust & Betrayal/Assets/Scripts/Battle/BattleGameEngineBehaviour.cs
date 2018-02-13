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
/// Every possible state during a battle.
/// Their order can change depending on events.
/// </summary>
public enum BattleTurnState
{
    ENEMY_INTRO1,
    ENEMY_INTRO2,
    ENEMY_PKMN_TALKS,
    ENEMY_PKMN_ANSWERS,
    ENEMY_PKMN_PLAYS,
    PLAYER_PKMN_TALKS_TOPLAYER,
    PLAYER_PKMN_ANSWERS_TOPLAYER,
    PLAYER_PKMN_TALKS_TOENEMY,
    PLAYER_PKMN_ANSWERS_TOENEMY,
    PLAYER_PKMN_PLAYS,
    PLAYER_PLAYS,
    BATTLE_END
}

/// <summary>
/// THe Battle Engine deals with everything that has to do with a Pokémon battle : current state, info written, last phrase said by each protagonist, audio events...
/// </summary>
public class BattleGameEngineBehaviour : MonoBehaviour
{
    BattleTurnState currentTurnState;

    public GameObject battleBackground;

    public BattlePokemonBehaviour enemyBattlePkmn;
    public BattlePokemonBehaviour playerBattlePkmn;

    public DialoguePanelBehaviour enemyPkmnDialoguePanel;
    public DialoguePanelBehaviour playerPkmnDialoguePanel;
    public DialoguePanelBehaviour playerDialoguePanel;

    public BattleMainInputPanelBehaviour mainInputPanel;

    private DialoguePhrase lastPhraseFromPlayer;
    private DialoguePhrase lastPhraseFromPlayerPkmn;
    private DialoguePhrase lastPhraseFromEnemyPkmn;

    public EventEngineBehaviour eventEngine;

    [Header("Audio")]
    public AudioSource attackAudioSource;
    public AudioSource typeAttackAudioSource;
    public AudioSource defenseAudioSource;
    public AudioSource playerPkmnCriAudioSource;
    public AudioSource enemyPkmnCriAudioSource;
    public AudioSource battleMusicAudioSource; // should already be playing
    public AudioSource battleWinMusicAudioSource;
    public AudioSource battleLoseMuicAudioSource;


    [Header("Animation")]
    public Animator screenOverlayAnimator;

    // Use this for initialization
    void Start ()
    {
        battleBackground.SetActive(false);
        enemyBattlePkmn.gameObject.SetActive(false);
        playerBattlePkmn.gameObject.SetActive(false);
        enemyPkmnDialoguePanel.gameObject.SetActive(false);
        playerPkmnDialoguePanel.gameObject.SetActive(false);
        playerDialoguePanel.gameObject.SetActive(false);
    }

    public void StartBattle(Pokemon playerPkmn, Pokemon enemyPkmn, bool isEnemy)
    {
        enemyBattlePkmn.SetPokemon(enemyPkmn, false);
        playerBattlePkmn.SetPokemon(playerPkmn, true);

        playerPkmnCriAudioSource.clip = playerPkmn.pokemonCriAudioClip;
        enemyPkmnCriAudioSource.clip = enemyPkmn.pokemonCriAudioClip;

        enemyBattlePkmn.gameObject.SetActive(true);
        playerBattlePkmn.gameObject.SetActive(true);
        mainInputPanel.gameObject.SetActive(true);
        battleBackground.SetActive(true);

        enemyBattlePkmn.MakePokemonAppear(0.1f);
        playerBattlePkmn.MakePokemonAppear(1.0f);

        currentTurnState = BattleTurnState.ENEMY_INTRO1;
        StartCoroutine(WaitAndResolveState(0.5f));

        screenOverlayAnimator.SetBool("OverlayVisible", false);
    }

    public void OpenPlayerInputDialogueWindow()
    {
        playerDialoguePanel.SetDefaultInputPhrase(DialogueSubjectCode.PLAYER);
        playerDialoguePanel.gameObject.SetActive(true);
    }
    
    public void PlayerSaysPhrase(DialoguePhrase phrase)
    {
        lastPhraseFromPlayer = phrase;
        //lastPhraseFromPlayer_timestamp = System.DateTime.Now.Ticks;

        mainInputPanel.RemoveText();
        currentTurnState = BattleTurnState.PLAYER_PKMN_ANSWERS_TOPLAYER;
        StartCoroutine(WaitAndResolveState(0.1f));
    }

    public void PlayerUsesPotion()
    {
        int hpHealed = playerBattlePkmn.Heal();

        List<string> actionsDescriptionList;

        if (Localization.currentLanguage == Language.FRENCH)
        {
            actionsDescriptionList = new List<string>()
            {
                "Tu as utilisé une Potion",
                playerBattlePkmn.pokemonObject.name.ToUpper() + " récupère " + hpHealed + " PV!"
            };
        }
        else
        {
            actionsDescriptionList = new List<string>()
            {
                "YOU used a Potion",
                playerBattlePkmn.pokemonObject.name.ToUpper() + " healed " + hpHealed + " HP!"
            };    
        }


        mainInputPanel.DisplayText(actionsDescriptionList, false);
        
        lastPhraseFromPlayer = null;
        currentTurnState = BattleTurnState.PLAYER_PKMN_ANSWERS_TOPLAYER;
        StartCoroutine(WaitAndResolveState(2.0f * actionsDescriptionList.Count));
    }

    #region Player Pokemon Actions
    
    private void DisplayPhraseFromPlayerPkmn(DialoguePhrase phrase)
    {
        playerPkmnDialoguePanel.gameObject.SetActive(true);
        playerPkmnDialoguePanel.SetCurrentPhrase(phrase);
    }

    public void PlayerPkmnSaysPhrase(DialoguePhrase phrase)
    {
        DisplayPhraseFromPlayerPkmn(phrase);
        lastPhraseFromPlayerPkmn = phrase;
        //lastPhraseFromPlayerPkmn_timestamp = System.DateTime.Now.Ticks;
        playerPkmnCriAudioSource.Stop();
        playerPkmnCriAudioSource.Play();
    }

    public void PlayerPkmnDefends()
    {
        List<string> textList = new List<string>() {
            Localization.PokemonUseSkill(playerBattlePkmn.pokemonObject.name.ToUpper(), playerBattlePkmn.pokemonObject.defenseName.ToUpper(), null)
        };
        mainInputPanel.DisplayText(textList, true);
    }

    #endregion

    #region Enemy Pokemon Actions

    private void DisplayPhraseFromEnemyPkmn(DialoguePhrase phrase)
    {
        enemyPkmnDialoguePanel.gameObject.SetActive(true);
        enemyPkmnDialoguePanel.SetCurrentPhrase(phrase);
    }

    public void EnemyPkmnSaysPhrase(DialoguePhrase phrase)
    {
        DisplayPhraseFromEnemyPkmn(phrase);
        lastPhraseFromEnemyPkmn = phrase;
        //lastPhraseFromEnemyPkmn_timestamp = System.DateTime.Now.Ticks;
        enemyPkmnCriAudioSource.Stop();
        enemyPkmnCriAudioSource.Play();
    }

    #endregion

    #region Attack / Defend

    private List<string> Attack(BattlePokemonBehaviour attacker, BattlePokemonBehaviour victim, bool typeAttack, int intensity, out int hpLost)
    {
        List<string> actionsDescriptionList = new List<string>();

        // Animation
        attacker.Attack(0.1f, typeAttack);

        if (victim != null)
        {
            // Attack a Pokemon
            actionsDescriptionList.Add(Localization.PokemonUseSkill(attacker.pokemonObject.name.ToUpper(), (typeAttack ? attacker.pokemonObject.typeAttackName.ToUpper() : attacker.pokemonObject.normalAttackName.ToUpper()), victim.pokemonObject.name.ToUpper()));

            // Animation
            victim.GetHit(1.0f);

            // HP lost
            hpLost = victim.GetsHit(attacker.pokemonObject, typeAttack, intensity);
            if (intensity == 1)
            {
                if (Localization.currentLanguage == Language.FRENCH)
                {
                    actionsDescriptionList.Add(attacker.pokemonObject.name.ToUpper() + " a à peine touché " + victim.pokemonObject.name.ToUpper() + ".");
                }
                else
                {
                    actionsDescriptionList.Add(attacker.pokemonObject.name.ToUpper() + " barely touched " + victim.pokemonObject.name.ToUpper() + ".");
                }
            }
            else if (intensity == 3)
            {
                if (Localization.currentLanguage == Language.FRENCH)
                {
                    actionsDescriptionList.Add("Coup critique!");
                }
                else
                {
                    actionsDescriptionList.Add("Critical hit!");
                }
            }

            if (typeAttack)
            {
                if (victim.pokemonObject.IsTypeEffectiveAgainstMe(attacker.pokemonObject.mainType) == -1)
                {

                    if (Localization.currentLanguage == Language.FRENCH)
                    {
                        actionsDescriptionList.Add("Ce n'est pas très efficace.");
                    }
                    else
                    {
                        actionsDescriptionList.Add("It's not really effective.");
                    }
                }
                else if (victim.pokemonObject.IsTypeEffectiveAgainstMe(attacker.pokemonObject.mainType) == 1)
                {

                    if (Localization.currentLanguage == Language.FRENCH)
                    {
                        actionsDescriptionList.Add("C'est super efficace!");
                    }
                    else
                    {
                        actionsDescriptionList.Add("It's super effective!");
                    }
                }
            }

            if (Localization.currentLanguage == Language.FRENCH)
            {
                actionsDescriptionList.Add(victim.pokemonObject.name.ToUpper() + " a perdu " + hpLost + " PV");
            }
            else
            {
                actionsDescriptionList.Add(victim.pokemonObject.name.ToUpper() + " lost " + hpLost + " HP");
            }

            // Victim face expression
            victim.ForceRandomExpressionWithDelay(1.0f, 0, 1, 1, 1, 1);
        }
        else
        {
            // Attack Player

            if (Localization.currentLanguage == Language.FRENCH)
            {
                actionsDescriptionList.Add(attacker.pokemonObject.name.ToUpper() + " utilise " + Localization.SkillName(typeAttack ? attacker.pokemonObject.typeAttackName.ToUpper() : attacker.pokemonObject.normalAttackName.ToUpper()) + " sur TOI !");
            }
            else
            {
                actionsDescriptionList.Add(attacker.pokemonObject.name.ToUpper() + " used " + (typeAttack ? attacker.pokemonObject.typeAttackName.ToUpper() : attacker.pokemonObject.normalAttackName.ToUpper()) + " on YOU!");
            }


            // HP lost
            hpLost = Random.Range(intensity * 20, intensity * 25);
            if (intensity == 3)
            {
                if (Localization.currentLanguage == Language.FRENCH)
                {
                    actionsDescriptionList.Add("Coup critique!");
                }
                else
                {
                    actionsDescriptionList.Add("Critical hit!");
                }
            }
            if (Localization.currentLanguage == Language.FRENCH)
            {
                actionsDescriptionList.Add("Tu as perdu " + hpLost + " PV");
            }
            else
            {
                actionsDescriptionList.Add("You lost " + hpLost + " HP");
            }

            // Enemy pkmn face expression
            enemyBattlePkmn.ForceRandomExpressionWithDelay(1.0f, 2, 0, 0.5f, 2, 1);
        }

        // Sound effects
        attackAudioSource.Stop();
        attackAudioSource.volume = (intensity == 1) ? 0.5f : ((intensity == 2) ? 0.8f : 1.0f);
        attackAudioSource.Play();

        return actionsDescriptionList;
    }

    private List<string> Defend(BattlePokemonBehaviour defender)
    {
        List<string> actionsDescriptionList = new List<string>();

        actionsDescriptionList.Add(Localization.PokemonUseSkill(defender.pokemonObject.name.ToUpper(), defender.pokemonObject.defenseName.ToUpper(), null));

        if (Localization.currentLanguage == Language.FRENCH)
        {
            actionsDescriptionList.Add("La DEFENSE de " + defender.pokemonObject.name.ToUpper() + " augmente.");
        }
        else
        {
            actionsDescriptionList.Add(defender.pokemonObject.name.ToUpper() + " DEFENSE increases.");
        }

        // Animation
        defender.Defends(0.1f);

        // Sound effects
        defenseAudioSource.Stop();
        defenseAudioSource.Play();

        return actionsDescriptionList;
    }

    #endregion

    public void CloseAllDialoguePanels()
    {
        playerDialoguePanel.gameObject.SetActive(false);
        playerPkmnDialoguePanel.gameObject.SetActive(false);
        enemyPkmnDialoguePanel.gameObject.SetActive(false);
    }

    private void EndBattle(bool win)
    {
        List<string> textList;
        if (win)
        {
            enemyBattlePkmn.MakePokemonDisappear(0.1f);
            int xpWon = Random.Range(playerBattlePkmn.pokemonObject.level * 5, playerBattlePkmn.pokemonObject.level * 7);

            int hp, atk, def, sped, spec;
            playerBattlePkmn.pokemonObject.LevelUP(out hp, out atk, out def, out sped, out spec);
            
            if (Localization.currentLanguage == Language.FRENCH)
            {
                textList = new List<string>()
                {
                    enemyBattlePkmn.pokemonObject.name.ToUpper() + " a été vaincu!",
                    "Tu remportes le combat!",
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " gagne " + xpWon + " EXP",
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " gagne un niveau!\nHP Max+" + hp + ", ATK+" + atk + ", DEF+"+def+", VIT+"+sped+", SPE+"+spec
                };
            }
            else
            {
                textList = new List<string>()
                {
                    enemyBattlePkmn.pokemonObject.name.ToUpper() + " has been defeated!",
                    "You won the battle!",
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " earns " + xpWon + " XP",
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " levels up!\nHP Max+" + hp + ", ATK+" + atk + ", DEF+"+def+", SPEED+"+sped+", SPECIAL+"+spec
                };
            }
        }
        else
        {
            playerBattlePkmn.MakePokemonDisappear(0.1f);

            if (Localization.currentLanguage == Language.FRENCH)
            {
                textList = new List<string>()
                {
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " a été mis KO!",
                    "Tu n'as plus d'autres POKéMONs.",
                    "Tu as perdu le combat."
                };
            }
            else
            {
                textList = new List<string>()
                {
                    playerBattlePkmn.pokemonObject.name.ToUpper() + " has been knocked out!",
                    "You don't have any other POKéMON.",
                    "You lost the battle."
                };
            }
        }
        mainInputPanel.DisplayText(textList, false);
        eventEngine.ShowAllEvents(textList.Count * 2.5f);
        eventEngine.ShowEventAfterBattle(win, textList.Count * 2.5f);

        StartCoroutine(WaitAndShowOverlay(textList.Count * 2.5f - 1));

        if (win)
        {
            battleWinMusicAudioSource.Stop();
            battleWinMusicAudioSource.volume = 1;
            battleWinMusicAudioSource.Play();
            StartCoroutine(FadeOutAudioSource(battleWinMusicAudioSource, textList.Count * 2.5f + 1, 0));
        }
        else
        {
            battleLoseMuicAudioSource.Stop();
            battleLoseMuicAudioSource.volume = 1;
            battleLoseMuicAudioSource.Play();
            StartCoroutine(FadeOutAudioSource(battleLoseMuicAudioSource, textList.Count * 2.5f + 1, 0));
        }
    }

    private IEnumerator WaitAndShowOverlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        screenOverlayAnimator.SetBool("OverlayVisible", true);
    }

    private IEnumerator FadeOutAudioSource(AudioSource source, float delayBeforeFadeOut, float fadeOutDuration)
    {
        yield return new WaitForSeconds(delayBeforeFadeOut);
        if (fadeOutDuration <= 0)
        {
            source.Stop();
        }
        else
        {
            float nextCallDelay = 0.05f;
            float deltaVolume = (nextCallDelay / fadeOutDuration);
            source.volume -= deltaVolume;
            if (source.volume <= 0 || fadeOutDuration <= 0)
            {
                source.Stop();
            }
            else
            {
                StartCoroutine(FadeOutAudioSource(source, nextCallDelay, fadeOutDuration - nextCallDelay));
            }
        }
    }

    /// <summary>
    /// This methods is called once every turn.
    /// It resolves the current state and choose the next one.
    /// </summary>
    public void ResolveCurrentState()
    {
        DialoguePhrase phrase;
        int hpLost = 0;
        PokemonActionInBattle action;
        List<string> actionsDescriptionList = new List<string>();
        bool necessitateAnswer;

        switch (currentTurnState)
        {
            case BattleTurnState.ENEMY_INTRO1:
                playerBattlePkmn.SetPokemonFacePlayer(false);
                if (Localization.currentLanguage == Language.FRENCH)
                {
                    mainInputPanel.DisplayText("Un " + enemyBattlePkmn.pokemonObject.name.ToUpper() + " apparaît !", false);
                }
                else
                {
                    mainInputPanel.DisplayText("A " + enemyBattlePkmn.pokemonObject.name.ToUpper() + " appears!", false);
                }
                currentTurnState = BattleTurnState.ENEMY_INTRO2;
                StartCoroutine(WaitAndResolveState(1.5f));
                break;
            case BattleTurnState.ENEMY_INTRO2:
                playerBattlePkmn.SetPokemonFacePlayer(false);
                mainInputPanel.RemoveText();
                DialogueTraitCode traitCode = (enemyBattlePkmn.pokemonObject.power >= 0.5f) ? DialogueTraitCode.POWER : (enemyBattlePkmn.pokemonObject.LovesPokemon(playerBattlePkmn.pokemonObject) ? DialogueTraitCode.LOVE : DialogueTraitCode.TRUST);
                phrase = DialogueEngineBehaviour.instance.GetPhrase_Greet(DialogueSubjectCode.ENEMY_PKMN, DialogueSubjectCode.PLAYER_PKMN, traitCode);
                switch(traitCode)
                {
                    case DialogueTraitCode.LOVE:
                        enemyBattlePkmn.ForceExpression(PokemonFacialExpression.HAPPY);
                        break;
                    case DialogueTraitCode.TRUST:
                        enemyBattlePkmn.ForceExpression(PokemonFacialExpression.SAD);
                        break;
                    case DialogueTraitCode.POWER:
                        enemyBattlePkmn.ForceExpression(PokemonFacialExpression.ANGRY);
                        break;
                }
                EnemyPkmnSaysPhrase(phrase);
                currentTurnState = BattleTurnState.PLAYER_PLAYS;
                break;

            case BattleTurnState.ENEMY_PKMN_TALKS:
                // no need to worry about what have been told before
                // enemy pokemon will say something random to player pokemon
                playerBattlePkmn.SetPokemonFacePlayer(false);
                mainInputPanel.RemoveText();
                phrase = enemyBattlePkmn.SayRandomPhrase(playerBattlePkmn.pokemonObject, 1, 2, 2, 2, 1, out necessitateAnswer);
                EnemyPkmnSaysPhrase(phrase);
                if (necessitateAnswer)
                {
                    currentTurnState = BattleTurnState.PLAYER_PKMN_ANSWERS_TOENEMY;
                }
                else
                {
                    currentTurnState = BattleTurnState.PLAYER_PLAYS;
                }
                break;

            case BattleTurnState.ENEMY_PKMN_ANSWERS:
                // player pokemon told something that requests an answer from enemy pokemon
                // enemy pokemon will answer, then he will act
                playerBattlePkmn.SetPokemonFacePlayer(false);
                mainInputPanel.RemoveText();
                phrase = enemyBattlePkmn.AnswerToEnemyPkmn(lastPhraseFromPlayerPkmn, playerBattlePkmn.pokemonObject);
                EnemyPkmnSaysPhrase(phrase);
                currentTurnState = BattleTurnState.PLAYER_PKMN_PLAYS;
                break;

            case BattleTurnState.ENEMY_PKMN_PLAYS:
                playerBattlePkmn.SetPokemonFacePlayer(false);
                action = enemyBattlePkmn.ChooseActionToPerform(playerBattlePkmn.pokemonObject);
                actionsDescriptionList.Clear();
                switch (action.actionType)
                {
                    case PokemonActionType.DEFEND:
                        actionsDescriptionList = Defend(enemyBattlePkmn);
                        break;
                    case PokemonActionType.ATTACK:
                        actionsDescriptionList = Attack(enemyBattlePkmn, playerBattlePkmn, false, action.intensity, out hpLost);
                        break;
                    case PokemonActionType.ATTACK_TYPE:
                        actionsDescriptionList = Attack(enemyBattlePkmn, playerBattlePkmn, true, action.intensity, out hpLost);
                        break;
                    default:

                        if (Localization.currentLanguage == Language.FRENCH)
                        {
                            actionsDescriptionList.Add(enemyBattlePkmn.pokemonObject.name.ToUpper() + " attend.");
                        }
                        else
                        {
                            actionsDescriptionList.Add(enemyBattlePkmn.pokemonObject.name.ToUpper() + " is waiting.");
                        }

                        break;
                }
                mainInputPanel.DisplayText(actionsDescriptionList, false);
                enemyBattlePkmn.lastActionPerformed_timestamp = System.DateTime.Now.Ticks;

                if (playerBattlePkmn.pokemonObject.HP <= 0)
                {
                    currentTurnState = BattleTurnState.BATTLE_END;
                }
                else
                {
                    currentTurnState = BattleTurnState.PLAYER_PKMN_TALKS_TOENEMY;
                }
                StartCoroutine(WaitAndResolveState(2.0f * actionsDescriptionList.Count));
                break;

            case BattleTurnState.PLAYER_PKMN_PLAYS:
                playerBattlePkmn.SetPokemonFacePlayer(false);
                action = playerBattlePkmn.ChooseActionToPerform(enemyBattlePkmn.pokemonObject);
                actionsDescriptionList.Clear();
                switch (action.actionType)
                {
                    case PokemonActionType.DEFEND:
                        actionsDescriptionList = Defend(playerBattlePkmn);
                        break;
                    case PokemonActionType.ATTACK:
                        actionsDescriptionList = Attack(playerBattlePkmn, enemyBattlePkmn, false, action.intensity, out hpLost);
                        break;
                    case PokemonActionType.ATTACK_TYPE:
                        actionsDescriptionList = Attack(playerBattlePkmn, enemyBattlePkmn, true, action.intensity, out hpLost);
                        break;
                    case PokemonActionType.ATTACK_ON_PLAYER:
                        actionsDescriptionList = Attack(playerBattlePkmn, null, false, 3, out hpLost);
                        playerBattlePkmn.ForceExpression(PokemonFacialExpression.ANGRY);
                        playerBattlePkmn.SetPokemonFacePlayer(true);
                        enemyBattlePkmn.ForceRandomExpressionWithDelay(0.5f, 2, 0, 0, 1, 3);
                        break;
                    default:

                        if (Localization.currentLanguage == Language.FRENCH)
                        {
                            actionsDescriptionList.Add(playerBattlePkmn.pokemonObject.name.ToUpper() + " attend.");
                        }
                        else
                        {
                            actionsDescriptionList.Add(playerBattlePkmn.pokemonObject.name.ToUpper() + " is waiting.");
                        }
                        
                        break;
                }
                mainInputPanel.DisplayText(actionsDescriptionList, false);
                playerBattlePkmn.lastActionPerformed_timestamp = System.DateTime.Now.Ticks;

                if (enemyBattlePkmn.pokemonObject.HP <= 0)
                {
                    currentTurnState = BattleTurnState.BATTLE_END;
                }
                else
                {
                    if (Random.Range(0,2) == 0)
                    {
                        currentTurnState = BattleTurnState.ENEMY_PKMN_TALKS;
                    }
                    else
                    {
                        currentTurnState = BattleTurnState.PLAYER_PKMN_TALKS_TOPLAYER;
                    }
                }
                
                StartCoroutine(WaitAndResolveState(2.0f * actionsDescriptionList.Count));

                break;

            case BattleTurnState.PLAYER_PKMN_TALKS_TOPLAYER:
                mainInputPanel.RemoveText();
                phrase = playerBattlePkmn.SayRandomPhrase(null, 1,1,1,0,1, out necessitateAnswer);
                playerBattlePkmn.SetPokemonFacePlayer(true);
                PlayerPkmnSaysPhrase(phrase);
                currentTurnState = BattleTurnState.PLAYER_PLAYS;
                break;

            case BattleTurnState.PLAYER_PKMN_ANSWERS_TOPLAYER:
                mainInputPanel.RemoveText();
                phrase = playerBattlePkmn.AnswerToPlayer(lastPhraseFromPlayer);
                playerBattlePkmn.SetPokemonFacePlayer(true);
                PlayerPkmnSaysPhrase(phrase);
                currentTurnState = BattleTurnState.ENEMY_PKMN_PLAYS;
                break;

            case BattleTurnState.PLAYER_PKMN_TALKS_TOENEMY:
                mainInputPanel.RemoveText();
                phrase = playerBattlePkmn.SayRandomPhrase(enemyBattlePkmn.pokemonObject, 0, 1, 1, 1, 1, out necessitateAnswer);
                playerBattlePkmn.SetPokemonFacePlayer(false);
                PlayerPkmnSaysPhrase(phrase);
                if (necessitateAnswer)
                {
                    currentTurnState = BattleTurnState.ENEMY_PKMN_ANSWERS;
                }
                else
                {
                    currentTurnState = BattleTurnState.PLAYER_PKMN_PLAYS;
                }
                break;

            case BattleTurnState.PLAYER_PKMN_ANSWERS_TOENEMY:
                mainInputPanel.RemoveText();
                phrase = playerBattlePkmn.AnswerToEnemyPkmn(lastPhraseFromEnemyPkmn, enemyBattlePkmn.pokemonObject);
                playerBattlePkmn.SetPokemonFacePlayer(false);
                PlayerPkmnSaysPhrase(phrase);
                currentTurnState = BattleTurnState.PLAYER_PLAYS;
                break;

            case BattleTurnState.PLAYER_PLAYS:
                playerBattlePkmn.SetPokemonFacePlayer(true);
                mainInputPanel.ShowMenu();
                currentTurnState = BattleTurnState.PLAYER_PKMN_ANSWERS_TOPLAYER;
                break;

            case BattleTurnState.BATTLE_END:
                EndBattle( playerBattlePkmn.pokemonObject.HP > 0 );
                break;

            default:
                currentTurnState = BattleTurnState.BATTLE_END;
                break;
        }
    }

    private IEnumerator WaitAndResolveState(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResolveCurrentState();
    }
}
