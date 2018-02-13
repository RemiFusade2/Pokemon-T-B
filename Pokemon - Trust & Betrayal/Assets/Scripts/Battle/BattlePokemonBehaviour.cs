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

public enum PokemonType
{
    NONE,
    NORMAL,
    FIRE,
    WATER,
    PLANT,
    ELECTRIC,
    FLY
}

public enum PokemonFacialExpression
{
    NONE,
    HAPPY, 
    SAD,
    WEIRD,
    ANGRY,
    WOUNDED,
    SCARED
}

public enum PokemonCode
{
    BULBAZABAZAUR,
    CHARMANANDER,
    SQUIRTLITULLI,
    PIKAPAKAPICHU,
    MAGIKARPACCIO,
    PIDGIPIDGEY,
    RATARARATATA
}

public enum PokemonActionType
{
    ATTACK,
    ATTACK_TYPE,
    ATTACK_ON_PLAYER,
    DEFEND,
    TALK,
    NOTHING
}

public class PokemonActionInBattle
{
    public PokemonActionType actionType;
    public int intensity;
}

/// <summary>
/// A Pokémon object contains all data on a Pokémon + methods to LevelUp or deal with hits.
/// </summary>
[System.Serializable]
public class Pokemon
{
    public int ID;

    public string name;
    public int level;
    public int HPMax;
    public int HP;

    public AudioClip pokemonCriAudioClip;

    public float goodness;
    public float honesty;
    public float power;

    public PokemonType mainType;
    public PokemonType secondaryType;

    public PokemonCode code;

    public int ATTACK;
    public int DEFENSE;
    public int SPEED;
    public int SPECIAL;

    public int DEFENSE_TMP_BOOST;

    public Relation relationWithPlayer;
    public Relation relationWithAntagonist;
    public List<Relation> relationsWithOtherPokemons;

    public string normalAttackName;
    public string typeAttackName;
    public string defenseName;

    [Header("Level UP")]
    public int MIN_HP_BOOST;
    public int MAX_HP_BOOST;
    public int MIN_ATTACK_BOOST;
    public int MAX_ATTACK_BOOST;
    public int MIN_DEFENSE_BOOST;
    public int MAX_DEFENSE_BOOST;
    public int MIN_SPEED_BOOST;
    public int MAX_SPEED_BOOST;
    public int MIN_SPECIAL_BOOST;
    public int MAX_SPECIAL_BOOST;

    public int Heal(int maxHPHealed)
    {
        int hpHealed = ( (this.HP + maxHPHealed) > this.HPMax) ? (this.HPMax - this.HP) : maxHPHealed;
        this.HP += hpHealed;
        return hpHealed;
    }

    public int IsTypeEffectiveAgainstMe(PokemonType otherType)
    {
        int effectiveness = 0;
        switch (mainType)
        {
            case PokemonType.ELECTRIC:
                switch (otherType)
                {
                    case PokemonType.ELECTRIC:
                        effectiveness = -1;
                        break;
                    case PokemonType.WATER:
                        effectiveness = -1;
                        break;
                }
                break;
            case PokemonType.FIRE:
                switch (otherType)
                {
                    case PokemonType.FIRE:
                        effectiveness = -1;
                        break;
                    case PokemonType.PLANT:
                        effectiveness = -1;
                        break;
                    case PokemonType.WATER:
                        effectiveness = 1;
                        break;
                }
                break;
            case PokemonType.FLY:
                switch (otherType)
                {
                    case PokemonType.ELECTRIC:
                        effectiveness = 1;
                        break;
                    case PokemonType.FLY:
                        effectiveness = 1;
                        break;
                    case PokemonType.PLANT:
                        effectiveness = -1;
                        break;
                }
                break;
            case PokemonType.NORMAL:
                switch (otherType)
                {
                    case PokemonType.FLY:
                        effectiveness = 1;
                        break;
                }
                break;
            case PokemonType.PLANT:
                switch (otherType)
                {
                    case PokemonType.ELECTRIC:
                        effectiveness = -1;
                        break;
                    case PokemonType.FLY:
                        effectiveness = 1;
                        break;
                    case PokemonType.PLANT:
                        effectiveness = -1;
                        break;
                    case PokemonType.FIRE:
                        effectiveness = 1;
                        break;
                    case PokemonType.WATER:
                        effectiveness = -1;
                        break;
                }
                break;
            case PokemonType.WATER:
                switch (otherType)
                {
                    case PokemonType.ELECTRIC:
                        effectiveness = 1;
                        break;
                    case PokemonType.PLANT:
                        effectiveness = 1;
                        break;
                    case PokemonType.FIRE:
                        effectiveness = -1;
                        break;
                    case PokemonType.WATER:
                        effectiveness = -1;
                        break;
                }
                break;
            default:
                break;
        }
        return effectiveness;
    }

    public float GetTraitValue(DialogueTraitCode code)
    {
        switch(code)
        {
            case DialogueTraitCode.LOVE:
                return this.goodness;
            case DialogueTraitCode.TRUST:
                return this.honesty;
            case DialogueTraitCode.POWER:
                return this.power;
        }
        return 0;
    }

    public int GetsHit(Pokemon attacker, bool attackUsesType, int intensity)
    {
        float typeMult = 1;
        float intensityMult = (intensity == 1) ? 0.5f : (intensity == 2) ? 1 : 1.3f;
        if (attackUsesType)
        {
            if (IsTypeEffectiveAgainstMe(attacker.mainType) == 1)
            {
                typeMult = 2;
            }
            else if (IsTypeEffectiveAgainstMe(attacker.mainType) == -1)
            {
                typeMult = 0.5f;
            }
        }

        float ATKDEFRatio = (attacker.ATTACK * 1.0f) / (this.DEFENSE + this.DEFENSE_TMP_BOOST);
        this.DEFENSE_TMP_BOOST -= 5;
        if (this.DEFENSE_TMP_BOOST < 0)
        {
            this.DEFENSE_TMP_BOOST = 0;
        }

        int hpLost = Mathf.RoundToInt (intensityMult * typeMult * ATKDEFRatio * 20 );
        this.HP -= hpLost;
        if (this.HP < 0)
        {
            this.HP = 0;
        }

        return hpLost;
    }

    public static Pokemon Copy(Pokemon original)
    {
        Pokemon newPkmn = new Pokemon();

        newPkmn.name = Localization.PokemonName(original.code);
        newPkmn.level = original.level;
        newPkmn.HPMax = original.HPMax;
        newPkmn.HP = original.HP;
        newPkmn.goodness = original.goodness;
        newPkmn.honesty = original.honesty;
        newPkmn.power = original.power;
        newPkmn.mainType = original.mainType;
        newPkmn.secondaryType = original.secondaryType;
        newPkmn.code = original.code;

        newPkmn.pokemonCriAudioClip = original.pokemonCriAudioClip;

        newPkmn.ATTACK = original.ATTACK;
        newPkmn.DEFENSE = original.DEFENSE;
        newPkmn.SPEED = original.SPEED;
        newPkmn.SPECIAL = original.SPECIAL;
        newPkmn.DEFENSE_TMP_BOOST = 0;
        
        newPkmn.MIN_HP_BOOST = original.MIN_HP_BOOST;
        newPkmn.MAX_HP_BOOST = original.MAX_HP_BOOST;
        newPkmn.MIN_ATTACK_BOOST = original.MIN_ATTACK_BOOST;
        newPkmn.MAX_ATTACK_BOOST = original.MAX_ATTACK_BOOST;
        newPkmn.MIN_DEFENSE_BOOST = original.MIN_DEFENSE_BOOST;
        newPkmn.MAX_DEFENSE_BOOST = original.MAX_DEFENSE_BOOST;
        newPkmn.MIN_SPEED_BOOST = original.MIN_SPEED_BOOST;
        newPkmn.MAX_SPEED_BOOST = original.MAX_SPEED_BOOST;
        newPkmn.MIN_SPECIAL_BOOST = original.MIN_SPECIAL_BOOST;
        newPkmn.MAX_SPECIAL_BOOST = original.MAX_SPECIAL_BOOST;

        newPkmn.normalAttackName = original.normalAttackName;
        newPkmn.typeAttackName = original.typeAttackName;
        newPkmn.defenseName = original.defenseName;

        newPkmn.relationsWithOtherPokemons = new List<Relation>();

        return newPkmn;
    }

    public void LevelUP(int nbLevel)
    {
        for (int i = 0; i < nbLevel; i++)
        {
            LevelUP();
        }
    }

    public void LevelUP()
    {
        int hp, atk, def, sped, spec;
        LevelUP(out hp, out atk, out def, out sped, out spec);
    }

    public void LevelUP(out int HPboost, out int attackBoost, out int defenseBoost, out int speedBoost, out int specialBoost)
    {
        this.level++;
        HPboost = Random.Range(MIN_HP_BOOST, MAX_HP_BOOST + 1);
        this.HPMax += HPboost;
        this.HP = this.HPMax;
        attackBoost = Random.Range(MIN_ATTACK_BOOST, MAX_ATTACK_BOOST + 1);
        this.ATTACK += attackBoost;
        defenseBoost = Random.Range(MIN_DEFENSE_BOOST, MAX_DEFENSE_BOOST + 1);
        this.DEFENSE += defenseBoost;
        speedBoost = Random.Range(MIN_SPEED_BOOST, MAX_SPEED_BOOST + 1);
        this.SPEED += speedBoost;
        specialBoost = Random.Range(MIN_SPECIAL_BOOST, MAX_SPECIAL_BOOST + 1);
        this.SPECIAL += specialBoost;
    }

    public bool LovesPlayer()
    {
        return ((relationWithPlayer.love + this.goodness) >= 0);
    }
    public bool TrustsPlayer()
    {
        return ((relationWithPlayer.trust + this.honesty) >= 0);
    }
    public bool FearsPlayer()
    {
        return ((relationWithPlayer.fear + this.power) >= 0);
    }

    public bool LovesPokemon(Pokemon pokemon)
    {
        Relation relation = GetRelationWithPokemon(pokemon);
        return (relation != null) ? ((relation.love + this.goodness) >= 0) : false;
    }
    public bool TrustsPokemon(Pokemon pokemon)
    {
        Relation relation = GetRelationWithPokemon(pokemon);
        return (relation != null) ? ((relation.trust + this.honesty) >= 0) : false;
    }
    public bool FearsPokemon(Pokemon pokemon)
    {
        Relation relation = GetRelationWithPokemon(pokemon);
        return (relation != null) ? ((relation.fear + this.power) >= 0) : false;
    }

    public Relation GetRelationWithPokemon(Pokemon pokemon)
    {
        foreach (Relation relation in relationsWithOtherPokemons)
        {
            if (relation.target.ID == pokemon.ID)
            {
                return relation;
            }
        }
        return null;
    }
}

/// <summary>
/// A Relation defines how a Pokémon feels about someone.
/// </summary>
[System.Serializable]
public class Relation
{
    // Attitude toward someone
    [System.NonSerialized]
    public Pokemon target;

    public float love;
    public float trust;
    public float fear;

    public float GetValueOfTrait(DialogueTraitCode trait)
    {
        switch (trait)
        {
            case DialogueTraitCode.LOVE:
                return love;
            case DialogueTraitCode.TRUST:
                return trust;
            case DialogueTraitCode.POWER:
                return fear;
        }
        return 0;
    }

    public void ChangeTrait(DialogueTraitCode trait, float howMuch)
    {
        switch (trait)
        {
            case DialogueTraitCode.LOVE:
                ChangeLove(howMuch);
                break;
            case DialogueTraitCode.TRUST:
                ChangeTrust(howMuch);
                break;
            case DialogueTraitCode.POWER:
                ChangeFear(howMuch);
                break;
        }
    }
    public void ChangeLove(float howMuch)
    {
        love += howMuch;
        love = (love >= 1.0f) ? 0.99f : love;
        love = (love <= -1.0f) ? -0.99f : love;
    }
    public void ChangeTrust(float howMuch)
    {
        trust += howMuch;
        trust = (trust >= 1.0f) ? 0.99f : trust;
        trust = (trust <= -1.0f) ? -0.99f : trust;
    }
    public void ChangeFear(float howMuch)
    {
        fear += howMuch;
        fear = (fear >= 1.0f) ? 0.99f : fear;
        fear = (fear <= -1.0f) ? -0.99f : fear;
    }
}

/// <summary>
/// This class updates info on a Pokémon during battle.
/// It offers methods to choose what to say or what to do according to the current status and relations of this Pokémon.
/// It deals with animations and facial expressions.
/// </summary>
public class BattlePokemonBehaviour : MonoBehaviour
{
    public Animator animatorController;

    public Text nameText;
    public Text lvlText;
    public Text hpValueText;

    public Slider hpSlider;
    public Image hpSliderFillImage;
    public Color hpSliderFillColor_OK;
    public Color hpSliderFillColor_Medium;
    public Color hpSliderFillColor_Alert;

    public Image pokemonBodyImage;
    public Image pokemonFaceImage;

    public DialogueEngineBehaviour dialogueEngine;

    public bool isPlayerPokemon;

    public Pokemon pokemonObject;

    public BattleGameEngineBehaviour battleGameEngine;

    public float lastActionPerformed_timestamp;

    private bool isFacingPlayer;
    private PokemonFacialExpression currentExpression;

    private List<DialoguePhrase> saidPhrases;

    void Start()
    {
        saidPhrases = new List<DialoguePhrase>();
    }

    private DialogueSubject GetSelfSubject()
    {
        return dialogueEngine.GetSubjectFromCode(isPlayerPokemon ? DialogueSubjectCode.PLAYER_PKMN : DialogueSubjectCode.ENEMY_PKMN);
    }

    #region Expression

    private void UpdateFaceToExpression()
    {
        PokemonSprites pokemonSprites = dialogueEngine.GetPokemonSpriteFromCode(pokemonObject.code);
        
        if (this.pokemonObject.HP < 0.2f * this.pokemonObject.HPMax)
        {
            currentExpression = PokemonFacialExpression.WOUNDED;
        }

        if (this.isFacingPlayer || !isPlayerPokemon)
        {
            switch (currentExpression)
            {
                case PokemonFacialExpression.NONE:
                    pokemonFaceImage.sprite = pokemonSprites.faceNoneSprite;
                    break;
                case PokemonFacialExpression.ANGRY:
                    pokemonFaceImage.sprite = pokemonSprites.faceAngrySprite;
                    break;
                case PokemonFacialExpression.HAPPY:
                    pokemonFaceImage.sprite = pokemonSprites.faceHappySprite;
                    break;
                case PokemonFacialExpression.SAD:
                    pokemonFaceImage.sprite = pokemonSprites.faceSadSprite;
                    break;
                case PokemonFacialExpression.SCARED:
                    pokemonFaceImage.sprite = pokemonSprites.faceScaredSprite;
                    break;
                case PokemonFacialExpression.WEIRD:
                    pokemonFaceImage.sprite = pokemonSprites.faceWeirdSprite;
                    break;
                case PokemonFacialExpression.WOUNDED:
                    pokemonFaceImage.sprite = pokemonSprites.faceWoundedSprite;
                    break;
            }
        }
        else
        {
            pokemonFaceImage.sprite = pokemonSprites.faceNoneSprite;
        }
    }

    public void SetRandomExpression(float happyProba, float angryProba, float sadProba, float scaredProba, float confusedProba)
    {
        float sumProba = happyProba + angryProba + sadProba + scaredProba + confusedProba;
        float rand = Random.Range(0, sumProba);

        if (rand < happyProba)
        {
            currentExpression = PokemonFacialExpression.HAPPY;
        }
        else if (rand < happyProba + angryProba)
        {
            currentExpression = PokemonFacialExpression.ANGRY;
        }
        else if (rand < happyProba + angryProba + sadProba)
        {
            currentExpression = PokemonFacialExpression.SAD;
        }
        else if (rand < happyProba + angryProba + sadProba + scaredProba)
        {
            currentExpression = PokemonFacialExpression.SCARED;
        }
        else if (rand < happyProba + angryProba + sadProba + scaredProba + confusedProba)
        {
            currentExpression = PokemonFacialExpression.WEIRD;
        }
        else
        {
            // Shouldn't execute this code
            currentExpression = PokemonFacialExpression.HAPPY;
        }

        if ((this.pokemonObject.HP * 1.0f / this.pokemonObject.HPMax) < 0.2f)
        {
            currentExpression = PokemonFacialExpression.WOUNDED;
        }
    }

    public void ForceRandomExpressionWithDelay(float delay, float happyProba, float angryProba, float sadProba, float scaredProba, float confusedProba)
    {
        StartCoroutine(WaitAndForceRandomExpression(delay, happyProba, angryProba, sadProba, scaredProba, confusedProba));
    }

    private IEnumerator WaitAndForceRandomExpression(float delay, float happyProba, float angryProba, float sadProba, float scaredProba, float confusedProba)
    {
        yield return new WaitForSeconds(delay);
        SetRandomExpression(happyProba, angryProba, sadProba, scaredProba, confusedProba);
        UpdateFaceToExpression();
    }

    public void ForceExpression(PokemonFacialExpression expression)
    {
        currentExpression = expression;
        UpdateFaceToExpression();
    }

    #endregion

    #region Action

    private Dictionary<PokemonActionInBattle, float> possibleActionsDictionary;

    private void InitPossibleActionsDictionary()
    {
        possibleActionsDictionary = new Dictionary<PokemonActionInBattle, float>();
    }

    private PokemonActionInBattle ChooseRandomAction()
    {
        List<float> possibleActionsWeights;
        List<PokemonActionInBattle> possibleActions = GetPossibleActions(out possibleActionsWeights);

        PokemonActionInBattle action = new PokemonActionInBattle() { actionType = PokemonActionType.NOTHING, intensity = 0 };
        float possibleActionsSumOfWeights = 0;
        for (int actionIndex = 0; actionIndex < possibleActions.Count; actionIndex++)
        {
            possibleActionsSumOfWeights += possibleActionsWeights[actionIndex];
        }

        float randomValue = Random.Range(0, possibleActionsSumOfWeights);
        float lastSumOfWeights = 0;
        for (int actionIndex = 0; actionIndex < possibleActions.Count; actionIndex++)
        {
            PokemonActionInBattle possibleAction = possibleActions[actionIndex];
            float possibleActionWeight = possibleActionsWeights[actionIndex];
            if (randomValue < (lastSumOfWeights + possibleActionWeight))
            {
                action = possibleAction;
                break;
            }
            lastSumOfWeights += possibleActionWeight;
        }

        return action;
    }

    private List<PokemonActionInBattle> GetPossibleActions(out List<float> weights)
    {
        List<PokemonActionInBattle> possibleActions = new List<PokemonActionInBattle>();
        weights = new List<float>();
        foreach (KeyValuePair<PokemonActionInBattle, float> keyValueInDico in possibleActionsDictionary)
        {
            possibleActions.Add(keyValueInDico.Key);
            weights.Add(keyValueInDico.Value);
        }
        return possibleActions;
    }

    private void AddPossibleAction(PokemonActionInBattle possibleAction, float weight)
    {
        if (possibleActionsDictionary.ContainsKey(possibleAction))
        {
            possibleActionsDictionary[possibleAction] = possibleActionsDictionary[possibleAction] + weight;
        }
        else
        {
            possibleActionsDictionary.Add(possibleAction, weight);
        }
    }

    public PokemonActionInBattle ChooseActionToPerform(Pokemon enemy)
    {
        InitPossibleActionsDictionary();

        PokemonActionInBattle possibleAction;

        int maxIntensity = isPlayerPokemon ? 3 : 2;

        // Build a list of possible actions
        if (isPlayerPokemon)
        {
            // How player pokemon should react ?
            if (this.pokemonObject.goodness < -0.5f && this.pokemonObject.relationWithPlayer.love < -0.5f && this.pokemonObject.relationWithPlayer.fear < -0.5f)
            {
                // Pokémon is a dick, he hates player, and consider him weak, he decides to attack him
                possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_ON_PLAYER, intensity = Random.Range(1, 4) };
                AddPossibleAction(possibleAction, 1);
            }
        }
        else
        {
            // How enemy pokemon should react ?
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.NOTHING, intensity = Random.Range(1, 4) };
            AddPossibleAction(possibleAction, (this.pokemonObject.power >= 0) ? 0.1f : (-this.pokemonObject.power) );
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.DEFEND, intensity = Random.Range(1, 4) };
            AddPossibleAction(possibleAction, 0.5f);
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = Random.Range(1, maxIntensity + 1) };
            AddPossibleAction(possibleAction, 0.5f);
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = Random.Range(1, maxIntensity + 1) };
            AddPossibleAction(possibleAction, 0.1f);
        }


        if (this.pokemonObject.FearsPokemon(enemy))
        {
            // Pokémon is afraid of enemy pokemon, he will defend
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.DEFEND, intensity = Random.Range(1, 4) };
            AddPossibleAction(possibleAction, this.pokemonObject.GetRelationWithPokemon(enemy).fear - this.pokemonObject.power);
        }
        else
        {
            // Pokémon is not afraid, he will attack
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = Random.Range(1, maxIntensity+1) };
            AddPossibleAction(possibleAction, 1);
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = Random.Range(1, maxIntensity + 1) };
            AddPossibleAction(possibleAction, 0.5f);
        }
        if (this.pokemonObject.LovesPokemon(enemy))
        {
            // Pokémon loves enemy pokemon, he will not do anything
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.NOTHING, intensity = Random.Range(1, 4) };
            AddPossibleAction(possibleAction, this.pokemonObject.GetRelationWithPokemon(enemy).love + this.pokemonObject.goodness);
        }
        else
        {
            // Pokemon doesn't love enemy, he will attack
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = Random.Range(1, maxIntensity + 1) };
            AddPossibleAction(possibleAction, 1);
            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = Random.Range(1, maxIntensity + 1) };
            AddPossibleAction(possibleAction, 0.5f);
        }

        int startPhraseIndex = saidPhrases.Count - 1;
        int maxPhrasesChecked = 3;
        int currentPhraseIndex = startPhraseIndex;
        while (currentPhraseIndex >= 0 && currentPhraseIndex > (startPhraseIndex - maxPhrasesChecked))
        {
            // Check phrase that pokemon said
            DialoguePhrase phrase = saidPhrases[currentPhraseIndex];
            if (saidPhrases.Count > 0 && phrase.mainVerb.code == DialogueVerbCode.PROMISE)
            {
                // Pokemon promised something !
                Relation relationWithTheOneIPromisedSomething;
                switch (phrase.mainObject.code)
                {
                    case DialogueSubjectCode.PLAYER:
                        // Pokemon promised something to player
                        relationWithTheOneIPromisedSomething = this.pokemonObject.relationWithPlayer;
                        break;
                    case DialogueSubjectCode.ENEMY_PKMN:
                    case DialogueSubjectCode.PLAYER_PKMN:
                        // Pokemon promised something to enemy pokemon
                        relationWithTheOneIPromisedSomething = this.pokemonObject.GetRelationWithPokemon(enemy);
                        break;
                    default:
                        relationWithTheOneIPromisedSomething = this.pokemonObject.relationWithPlayer;
                        break;
                }

                // between -7 and +7
                float honorPromiseRatio = (relationWithTheOneIPromisedSomething.trust + this.pokemonObject.honesty * 2);
                honorPromiseRatio += relationWithTheOneIPromisedSomething.love * 2 + this.pokemonObject.goodness;
                honorPromiseRatio += relationWithTheOneIPromisedSomething.fear;
                if (honorPromiseRatio > 0)
                {
                    switch (phrase.proposition.mainVerb.code)
                    {
                        case DialogueVerbCode.DEFEND:
                            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.DEFEND, intensity = Random.Range(1, 4) };
                            AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                            break;
                        case DialogueVerbCode.ATTACK:
                            if (phrase.proposition.mainVerbTraitQuantifier == null)
                            {
                                // No quantifier was given
                                possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = Random.Range(1, maxIntensity + 1) };
                                AddPossibleAction(possibleAction, 1 * honorPromiseRatio);
                                possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = Random.Range(1, maxIntensity + 1) };
                                AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                            }
                            else
                            {
                                switch (phrase.proposition.mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        // promised to not attack
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.NOTHING, intensity = Random.Range(1, 4) };
                                        AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.DEFEND, intensity = Random.Range(1, 4) };
                                        AddPossibleAction(possibleAction, 1 * honorPromiseRatio);
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        // promised to attack
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = 1 };
                                        AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        // promised to attack
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = 2 };
                                        AddPossibleAction(possibleAction, 1 * honorPromiseRatio);
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = 2 };
                                        AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        // promised to attack
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK, intensity = 3 };
                                        AddPossibleAction(possibleAction, 1 * honorPromiseRatio);
                                        possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.ATTACK_TYPE, intensity = Random.Range(2, maxIntensity + 1) };
                                        AddPossibleAction(possibleAction, 2 * honorPromiseRatio);
                                        break;
                                }
                            }
                            break;
                        default:
                            possibleAction = new PokemonActionInBattle() { actionType = PokemonActionType.NOTHING, intensity = Random.Range(1, 4) };
                            AddPossibleAction(possibleAction, 1 * honorPromiseRatio);
                            break;
                    }
                }
            }
            currentPhraseIndex--;
        }

        // Choose action
        PokemonActionInBattle action = ChooseRandomAction();

        return action;
    }

    #endregion

    private DialoguePhrase AnswerToDeal(DialoguePhrase deal, Relation relationWithDealer, out PokemonFacialExpression expression)
    {
        bool isPositiveDeal = !(
            deal.proposition.mainVerb.code == DialogueVerbCode.ATTACK || 
            deal.condition.mainVerb.code == DialogueVerbCode.ATTACK ||
            deal.condition.proposition != null && deal.condition.proposition.mainVerb.code == DialogueVerbCode.ATTACK);
        bool accept = relationWithDealer.love >= 0.5f || relationWithDealer.trust >= 0.5f;
        DialoguePhrase answer = dialogueEngine.GetPhrase_ResponseToDeal(GetSelfSubject().code, deal.mainSubject.code, accept, isPositiveDeal, out expression);
        if (relationWithDealer.fear >= 0.5f && !isPositiveDeal)
        {
            expression = PokemonFacialExpression.SCARED;
        }
        saidPhrases.Add(answer);
        return answer;
    }

    public DialoguePhrase AnswerToPlayer(DialoguePhrase phraseFromPlayer)
    {
        DialoguePhrase answerFromPokemon;
        bool necessitateAnswer;
        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
        PokemonFacialExpression expression;
        int rand;
        if (phraseFromPlayer != null)
        {
            if (phraseFromPlayer.mainSubject.code == DialogueSubjectCode.PLAYER)
            {
                // Player speaks for himself
                switch (phraseFromPlayer.mainVerb.code)
                {
                    case DialogueVerbCode.DEAL:
                        // Player offers deal !
                        // Answer should look like "I accept" or "I refuse"
                        answerFromPokemon = AnswerToDeal(phraseFromPlayer, this.pokemonObject.relationWithPlayer, out expression);
                        currentExpression = expression;
                        break;
                    case DialogueVerbCode.AGREE:
                        // Player agrees
                        // Either out of nowhere, or in response to a deal offered by enemy or ally Pokémon
                        // Pokémon could simply express satisfaction
                        this.pokemonObject.relationWithPlayer.ChangeLove(0.1f);
                        answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, true);
                        currentExpression = PokemonFacialExpression.HAPPY;
                        break;
                    case DialogueVerbCode.DISAGREE:
                        // Player disagrees
                        // Either out of nowhere, or in response to a deal offered by enemy or ally Pokémon
                        // Pokémon could simply express dissatisfaction
                        this.pokemonObject.relationWithPlayer.ChangeLove(-0.1f);
                        answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, false);
                        currentExpression = PokemonFacialExpression.ANGRY;
                        break;
                    case DialogueVerbCode.HEAL:
                        // Player heal someone
                        // this phrase should not be possible
                        if (phraseFromPlayer.mainObject.code == DialogueSubjectCode.ENEMY_PKMN)
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                        }
                        else
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                        }
                        break;
                    case DialogueVerbCode.DEFEND:
                        // Player defends
                        // this phrase should not be possible
                        this.pokemonObject.relationWithPlayer.ChangeTrust(0.1f);
                        answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.TRUST, true);
                        currentExpression = PokemonFacialExpression.HAPPY;
                        break;
                    case DialogueVerbCode.ASK:
                        // Player ask something to someone
                        switch (phraseFromPlayer.mainObject.code)
                        {
                            case DialogueSubjectCode.PLAYER:
                                answerFromPokemon = SayRandomPhrase(null, 0, 1, 0, 1, 1, out necessitateAnswer);
                                break;
                            case DialogueSubjectCode.ENEMY_PKMN:
                                answerFromPokemon = SayRandomPhrase(null, 0, 1, 0, 0, 1, out necessitateAnswer);
                                break;
                            case DialogueSubjectCode.PLAYER_PKMN:
                                bool acceptAsk;
                                Relation relationToRequestObject = null;
                                if (this.isPlayerPokemon)
                                {
                                    if (phraseFromPlayer.proposition.mainObject != null && phraseFromPlayer.proposition.mainObject.code == DialogueSubjectCode.ENEMY_PKMN)
                                    {
                                        relationToRequestObject = this.pokemonObject.GetRelationWithPokemon(battleGameEngine.enemyBattlePkmn.pokemonObject);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Error: enemypkmn should not answer to player");
                                }
                                answerFromPokemon = dialogueEngine.GetPhrase_ResponseToAsk(phraseFromPlayer.mainObject.code, phraseFromPlayer.mainSubject.code, phraseFromPlayer.proposition, phraseFromPlayer.mainVerbTrait.code, this.pokemonObject.relationWithPlayer, relationToRequestObject, out expression, out acceptAsk);
                                currentExpression = expression;
                                this.pokemonObject.relationWithPlayer.ChangeTrait(phraseFromPlayer.mainVerbTrait.code, acceptAsk ? 0.1f : -0.1f);
                                break;
                            default:
                                answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                                break;
                        }
                        break;
                    case DialogueVerbCode.ATTACK:
                        // Player attacks
                        // this phrase should not be possible
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.BE:
                        // Player declares that he is something
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.CAPTURE:
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.EXPRESS_FEELING:
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.GREET:
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.PROMISE:
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.THINK:
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    default:
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                }
            }
            else if (phraseFromPlayer.mainSubject.code == DialogueSubjectCode.PLAYER_PKMN)
            {
                // Player's pokémon is main subject of phrase
                switch (phraseFromPlayer.mainVerb.code)
                {
                    case DialogueVerbCode.BE:
                        // Player said his Pokemon was something
                        DialogueTraitCode trait = phraseFromPlayer.mainVerbTrait.code;
                        DialogueQuantifierCode quantifier = phraseFromPlayer.mainVerbTraitQuantifier.code;
                        switch (trait)
                        {
                            case DialogueTraitCode.LOVE:
                                switch (quantifier)
                                {
                                    case DialogueQuantifierCode.NONE:
                                    case DialogueQuantifierCode.FEW:
                                        // player said his pokemon was not nice
                                        if (this.pokemonObject.goodness < 0)
                                        {
                                            // and pokemon is indeed not nice
                                            // pokemon either confirm or say mean thing about player
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.ANGRY;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, false);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // and pokemon is nice in fact!
                                            // pokemon disagree or say he is nice
                                            this.pokemonObject.relationWithPlayer.ChangeLove(-0.2f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.SAD;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, DialogueTraitCode.LOVE, this.pokemonObject.goodness, this.pokemonObject.honesty);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                    case DialogueQuantifierCode.MUCH:
                                        // player said his pokemon was nice
                                        if (this.pokemonObject.goodness > 0)
                                        {
                                            // and pokemon is indeed nice
                                            // pokemon either confirm or say nice things about player
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.1f);
                                            this.pokemonObject.relationWithPlayer.ChangeLove(0.2f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.HAPPY;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, true);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // and pokemon is not nice in fact!
                                            // pokemon disagree or say he is not nice
                                            this.pokemonObject.relationWithPlayer.ChangeLove(-0.1f);
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(-0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.ANGRY;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, DialogueTraitCode.LOVE, this.pokemonObject.goodness, this.pokemonObject.honesty);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            case DialogueTraitCode.TRUST:
                                switch (quantifier)
                                {
                                    case DialogueQuantifierCode.NONE:
                                    case DialogueQuantifierCode.FEW:
                                        // player said his pokemon was not honest
                                        if (this.pokemonObject.honesty < 0)
                                        {
                                            // and pokemon is indeed not honest
                                            // pokemon either confirm or say mean thing about player
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.ANGRY;
                                            if (rand < 30)
                                            {
                                                // Pokemon confirms
                                                answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                            }
                                            else if (rand < 70)
                                            {
                                                // Pokemon says he doesn't like Player
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, false);
                                                if (Random.Range(0, 2) == 0)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                            else
                                            {
                                                // Pokemon says he doesn't trust Player
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.TRUST, false);
                                                if (Random.Range(0, 2) == 0)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // and pokemon is honest in fact!
                                            // pokemon disagree or say he is honest
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(-0.2f);
                                            this.pokemonObject.relationWithPlayer.ChangeLove(-0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.SAD;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, DialogueTraitCode.TRUST, this.pokemonObject.honesty, this.pokemonObject.honesty);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                    case DialogueQuantifierCode.MUCH:
                                        // player said his pokemon was honest
                                        if (this.pokemonObject.honesty > 0)
                                        {
                                            // and pokemon is indeed honest
                                            // pokemon either confirm or say nice things about player
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.2f);
                                            this.pokemonObject.relationWithPlayer.ChangeLove(0.1f);
                                            currentExpression = PokemonFacialExpression.HAPPY;
                                        }
                                        else
                                        {
                                            // and pokemon is not honest in fact!
                                            // pokemon disagree or say he is not honest
                                            this.pokemonObject.relationWithPlayer.ChangeLove(-0.1f);
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(-0.1f);
                                            currentExpression = PokemonFacialExpression.WEIRD;
                                        }
                                        rand = Random.Range(0, 100);
                                        if (rand < 40)
                                        {
                                            answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                        }
                                        else
                                        {
                                            DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.TRUST, true);
                                            if (rand < 70)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                            }
                                            else
                                            {
                                                answerFromPokemon = info;
                                            }
                                        }
                                        break;
                                }
                                break;
                            case DialogueTraitCode.POWER:
                            default:
                                switch (quantifier)
                                {
                                    case DialogueQuantifierCode.NONE:
                                    case DialogueQuantifierCode.FEW:
                                        // player said his pokemon was weak
                                        if (this.pokemonObject.power < 0)
                                        {
                                            // and pokemon is indeed weak
                                            // pokemon either confirm or say mean thing about himself
                                            this.pokemonObject.power -= 0.1f;
                                            this.pokemonObject.relationWithPlayer.ChangeFear(0.1f);
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.SAD;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                            }
                                            else if (rand < 70)
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.TRUST, true);
                                                answerFromPokemon = info;
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, GetSelfSubject().code, DialogueTraitCode.POWER, false);
                                                if (rand < 85)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // and pokemon is powerful in fact!
                                            // pokemon disagree or say he is powerful
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(-0.2f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.ANGRY;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, DialogueTraitCode.POWER, this.pokemonObject.power, 1.0f);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                    case DialogueQuantifierCode.MUCH:
                                        // player said his pokemon was powerful
                                        if (this.pokemonObject.power > 0)
                                        {
                                            // and pokemon is indeed powerful
                                            // pokemon either confirm or say nice things about player
                                            this.pokemonObject.relationWithPlayer.ChangeTrust(0.2f);
                                            this.pokemonObject.relationWithPlayer.ChangeLove(0.1f);
                                            this.pokemonObject.relationWithPlayer.ChangeFear(-0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.HAPPY;
                                            if (rand < 40)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Agree(GetSelfSubject().code);
                                            }
                                            else if (rand < 70)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.TRUST, true);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.POWER, true);
                                                if (rand < 90)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // and pokemon is weak in fact!
                                            // pokemon disagree or say he is weak
                                            this.pokemonObject.relationWithPlayer.ChangeLove(0.1f);
                                            rand = Random.Range(0, 100);
                                            currentExpression = PokemonFacialExpression.SAD;
                                            if (rand < 50)
                                            {
                                                answerFromPokemon = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
                                            }
                                            else
                                            {
                                                DialoguePhrase info = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, DialogueTraitCode.POWER, this.pokemonObject.power, this.pokemonObject.honesty);
                                                if (rand < 75)
                                                {
                                                    answerFromPokemon = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, info);
                                                }
                                                else
                                                {
                                                    answerFromPokemon = info;
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case DialogueVerbCode.DEAL:
                    case DialogueVerbCode.AGREE:
                    case DialogueVerbCode.DISAGREE:
                    case DialogueVerbCode.HEAL:
                    case DialogueVerbCode.DEFEND:
                    case DialogueVerbCode.ASK:
                    case DialogueVerbCode.ATTACK:
                    case DialogueVerbCode.CAPTURE:
                    case DialogueVerbCode.EXPRESS_FEELING:
                    case DialogueVerbCode.GREET:
                    case DialogueVerbCode.PROMISE:
                    case DialogueVerbCode.THINK:
                        // Player makes his Pokémon say something
                        // Pokémon could either express dissatisfaction or change the subject
                        rand = Random.Range(0, 100);
                        if (rand < 30)
                        {
                            // Pokémon loves the player a bit less
                            this.pokemonObject.relationWithPlayer.ChangeLove(-0.1f);
                            DialogueQuantifierCode quantifierLove = DialogueQuantifierCode.FEW;
                            if (this.pokemonObject.relationWithPlayer.love < -0.5f)
                            {
                                quantifierLove = DialogueQuantifierCode.NONE;
                            }
                            answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, DialogueSubjectCode.PLAYER, DialogueTraitCode.LOVE, quantifierLove);
                            currentExpression = PokemonFacialExpression.ANGRY;
                        }
                        else
                        {
                            // change subject
                            answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        }
                        break;
                    default:
                        // safe call
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                }
            }
            else if (phraseFromPlayer.mainSubject.code == DialogueSubjectCode.ENEMY_PKMN)
            {
                // Enemy Pokémon is main subject of sentence (clever way of making your pokemon think something about other pokémon)
                switch (phraseFromPlayer.mainVerb.code)
                {
                    case DialogueVerbCode.BE:
                        // Player said something about enemy Pokémon
                        // TODO
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.DEAL:
                        // Player said enemy Pokémon offers a deal
                        answerFromPokemon = dialogueEngine.GetPhrase_ResponseToDeal(DialogueSubjectCode.PLAYER_PKMN, DialogueSubjectCode.ENEMY_PKMN, false, false, out expression);
                        currentExpression = expression;
                        break;
                    case DialogueVerbCode.AGREE:
                        // Player said enemy Pokémon agrees
                        if (this.pokemonObject.honesty < 0)
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(DialogueSubjectCode.PLAYER_PKMN, DialogueSubjectCode.ENEMY_PKMN, DialogueTraitCode.TRUST, false);
                            currentExpression = PokemonFacialExpression.SAD;
                        }
                        else
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(DialogueSubjectCode.PLAYER_PKMN, DialogueSubjectCode.ENEMY_PKMN, DialogueTraitCode.TRUST, true);
                            currentExpression = PokemonFacialExpression.HAPPY;
                        }
                        break;
                    case DialogueVerbCode.DISAGREE:
                        // Player said enemy Pokémon disagrees
                        if (this.pokemonObject.honesty < 0)
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(DialogueSubjectCode.PLAYER_PKMN, DialogueSubjectCode.ENEMY_PKMN, DialogueTraitCode.LOVE, false);
                            currentExpression = PokemonFacialExpression.ANGRY;
                        }
                        else
                        {
                            answerFromPokemon = dialogueEngine.GetPhrase_ExpressFeeling(DialogueSubjectCode.PLAYER_PKMN, DialogueSubjectCode.ENEMY_PKMN, DialogueTraitCode.TRUST, true);
                            currentExpression = PokemonFacialExpression.SAD;
                        }
                        break;
                    case DialogueVerbCode.HEAL:
                    case DialogueVerbCode.DEFEND:
                    case DialogueVerbCode.ATTACK:
                    case DialogueVerbCode.CAPTURE:
                        // should be impossible
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.ASK:
                        // Players says enemy pokemon ask something
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.EXPRESS_FEELING:
                        // Players says enemy pokemon express feeling
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.GREET:
                        // Players says enemy pokemon greet
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.PROMISE:
                        // Players says enemy pokemon promise something
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    case DialogueVerbCode.THINK:
                        // Players says enemy pokemon thing something
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                    default:
                        answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
                        break;
                }
            }
            else
            {
                // main subject is Enemy ?
                answerFromPokemon = SayRandomPhrase(null, 1, 1, 1, 1, 1, out necessitateAnswer);
            }
        }
        else
        {
            answerFromPokemon = SayRandomPhrase(null, 0, 1, 1, 0, 1, out necessitateAnswer);
        }
        
        saidPhrases.Add(answerFromPokemon);
        return answerFromPokemon;
    }

    public DialoguePhrase AnswerToEnemyPkmn(DialoguePhrase phraseFromEnemyPkmn, Pokemon enemyPkmn)
    {
        Debug.Log("Answer to enemy pokemon ("+ enemyPkmn.name+") who said : " + phraseFromEnemyPkmn.ToString());
        DialoguePhrase answerFromPokemon;

        if (phraseFromEnemyPkmn.mainVerb.code == DialogueVerbCode.DEAL)
        {
            PokemonFacialExpression expression;
            answerFromPokemon = AnswerToDeal(phraseFromEnemyPkmn, this.pokemonObject.GetRelationWithPokemon(enemyPkmn), out expression);
            currentExpression = expression;
        }
        else if(phraseFromEnemyPkmn.mainVerb.code == DialogueVerbCode.ASK)
        {
            PokemonFacialExpression expression;
            bool accept;
            Relation relationToRequestObject = null;
            if (this.isPlayerPokemon)
            {
                if (phraseFromEnemyPkmn.proposition.mainObject != null && phraseFromEnemyPkmn.proposition.mainObject.code == DialogueSubjectCode.ENEMY_PKMN)
                {
                    relationToRequestObject = this.pokemonObject.GetRelationWithPokemon(battleGameEngine.enemyBattlePkmn.pokemonObject);
                }
                else  if (phraseFromEnemyPkmn.proposition.mainObject != null && phraseFromEnemyPkmn.proposition.mainObject.code == DialogueSubjectCode.PLAYER)
                {
                    relationToRequestObject = this.pokemonObject.relationWithPlayer;
                }
            }
            else
            {
                Debug.LogError("Error: enemypkmn should not answer to player");
            }
            answerFromPokemon = dialogueEngine.GetPhrase_ResponseToAsk(GetSelfSubject().code, phraseFromEnemyPkmn.mainSubject.code, phraseFromEnemyPkmn.proposition, phraseFromEnemyPkmn.mainVerbTrait.code, this.pokemonObject.GetRelationWithPokemon(enemyPkmn), relationToRequestObject, out expression, out accept);
            currentExpression = expression;
            saidPhrases.Add(answerFromPokemon);
        }
        else
        {
            bool necessitateAnswer;
            answerFromPokemon = SayRandomPhrase(enemyPkmn, 1, 2, 2, 2, 1, out necessitateAnswer);
        }

        return answerFromPokemon;
    }

    public DialoguePhrase SayRandomPhrase(Pokemon enemyPkmn, float probaOfRevealPersonalTrait, float probaOfExpressAFeeling, float probaOfMakingAPromise, float probaOfProposingADeal, float probaOfSharingAThought, out bool necessitateAnswer)
    {
        Debug.Log("Say random phrase to " + ((enemyPkmn == null) ? "Player" : enemyPkmn.name));

        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = GetSelfSubject();

        float sumOfWeights = probaOfRevealPersonalTrait + probaOfExpressAFeeling + probaOfMakingAPromise + probaOfProposingADeal + probaOfSharingAThought;

        float randomValue = Random.Range(0, sumOfWeights);
        
        int traitIndex = Random.Range(0, 3);
        DialogueTraitCode traitCode = (DialogueTraitCode)traitIndex;

        DialogueSubjectCode objectCode = (enemyPkmn == null) ? DialogueSubjectCode.PLAYER : (isPlayerPokemon ? DialogueSubjectCode.ENEMY_PKMN : DialogueSubjectCode.PLAYER_PKMN);
        DialogueSubjectCode enemyPkmnCode = (isPlayerPokemon ? DialogueSubjectCode.ENEMY_PKMN : DialogueSubjectCode.PLAYER_PKMN);

        necessitateAnswer = false;

        if (randomValue < probaOfRevealPersonalTrait)
        {
            // Reveal trait
            // Say he is Honest/Powerful/Nice
            phrase = dialogueEngine.GetPhrase_Be(GetSelfSubject().code, traitCode, this.pokemonObject.GetTraitValue(traitCode), this.pokemonObject.honesty);
            if (this.pokemonObject.GetTraitValue(traitCode) < 0)
            {
                currentExpression = PokemonFacialExpression.ANGRY;
            }
            else
            {
                currentExpression = PokemonFacialExpression.HAPPY;
            }
        }
        else if (randomValue < probaOfRevealPersonalTrait + probaOfExpressAFeeling)
        {
            // Express a feeling about someone
            Relation relationWithSomeone;
            if (Random.Range(0, 1) == 0)
            {
                relationWithSomeone = this.pokemonObject.relationWithPlayer;
            }
            else
            {
                relationWithSomeone = this.pokemonObject.GetRelationWithPokemon(enemyPkmn);
            }

            phrase = dialogueEngine.GetPhrase_ExpressFeeling(GetSelfSubject().code, objectCode, traitCode, relationWithSomeone.GetValueOfTrait(traitCode) > 0);
        }
        else if (randomValue < probaOfRevealPersonalTrait + probaOfExpressAFeeling + probaOfMakingAPromise)
        {
            // Promise something to interlocutor about enemy
            bool nicePromise = Random.Range(0, 10) < 5;
            phrase = dialogueEngine.GetPhrase_Promise(GetSelfSubject().code, objectCode, enemyPkmnCode, null, nicePromise);
            if (nicePromise)
            {
                currentExpression = PokemonFacialExpression.HAPPY;
            }
            else
            {
                currentExpression = PokemonFacialExpression.ANGRY;
            }            
        }
        else if (randomValue < probaOfRevealPersonalTrait + probaOfExpressAFeeling + probaOfMakingAPromise + probaOfProposingADeal)
        {
            // Offer deal to interlocutor about enemy, if interlocutor acts a certain way
            PokemonFacialExpression expression;
            phrase = dialogueEngine.GetPhrase_OfferDeal(GetSelfSubject().code, objectCode, GetSelfSubject().code, enemyPkmnCode, null, null, out expression);
            currentExpression = expression;
            necessitateAnswer = true;
        }
        else if (randomValue < probaOfRevealPersonalTrait + probaOfExpressAFeeling + probaOfMakingAPromise + probaOfProposingADeal + probaOfSharingAThought)
        {
            // Share thought
            DialoguePhrase proposition;
            int rand = Random.Range(0, 100);
            if (rand < 30)
            {
                proposition = dialogueEngine.GetPhrase_Attack(objectCode, GetSelfSubject().code, false);
                currentExpression = PokemonFacialExpression.SCARED;
            }
            else if(rand < 50)
            {
                proposition = dialogueEngine.GetPhrase_Defend(objectCode);
                currentExpression = PokemonFacialExpression.HAPPY;
            }
            else if (rand < 60)
            {
                proposition = dialogueEngine.GetPhrase_Heal(objectCode, objectCode);
                currentExpression = PokemonFacialExpression.ANGRY;
            }
            else
            {
                bool playerExpressPositiveFeelingTowardsHisPokemon = (GetSelfSubject().code != DialogueSubjectCode.ENEMY_PKMN) && (this.pokemonObject.LovesPlayer());
                proposition = dialogueEngine.GetPhrase_ExpressFeeling(DialogueSubjectCode.PLAYER, DialogueSubjectCode.PLAYER_PKMN, traitCode, playerExpressPositiveFeelingTowardsHisPokemon);
                if (GetSelfSubject().code == DialogueSubjectCode.ENEMY_PKMN)
                {
                    currentExpression = PokemonFacialExpression.ANGRY;
                }
                else
                {
                    if (playerExpressPositiveFeelingTowardsHisPokemon)
                    {
                        currentExpression = PokemonFacialExpression.HAPPY;
                    }
                    else
                    {
                        currentExpression = PokemonFacialExpression.SAD;
                    }
                }
            }

            phrase = dialogueEngine.GetPhrase_Think(GetSelfSubject().code, proposition);
        }
        else
        {
            // we should never arrive here
            phrase = dialogueEngine.GetPhrase_Disagree(GetSelfSubject().code);
            currentExpression = PokemonFacialExpression.WEIRD;
        }

        saidPhrases.Add(phrase);
        UpdateFaceToExpression();
        return phrase;
    }

    #region Pokemon orientation

    public void SetPokemonFacePlayer(bool facingPlayer)
    {
        this.isFacingPlayer = facingPlayer;
        PokemonSprites pokemonSprites = dialogueEngine.GetPokemonSpriteFromCode(pokemonObject.code);
        if (isPlayerPokemon && !facingPlayer)
        {
            pokemonBodyImage.sprite = pokemonSprites.bodyBackSprite;
        }
        else
        {
            pokemonBodyImage.sprite = pokemonSprites.bodyFrontSprite;
        }
        UpdateFaceToExpression();
    }

    #endregion

    #region Initialization

    public void SetPokemon(Pokemon pokemon, bool isPlayerPkmn)
    {
        this.dialogueEngine.SetPokemon(isPlayerPkmn ? DialogueSubjectCode.PLAYER_PKMN : DialogueSubjectCode.ENEMY_PKMN, pokemon);
        this.pokemonObject = pokemon;

        this.isPlayerPokemon = isPlayerPkmn;

        nameText.text = pokemonObject.name;
        lvlText.text = ":L" + ((pokemonObject.level < 10) ? "0" : "") + pokemonObject.level;
        lvlText.text = ":L" + pokemonObject.level;
        if (hpValueText != null)
        {
            hpValueText.text = pokemonObject.HP + "/ " + pokemonObject.HPMax;
        }
        hpSlider.minValue = 0;
        hpSlider.maxValue = pokemonObject.HPMax;
        hpSlider.value = pokemonObject.HP;

        this.pokemonObject.DEFENSE_TMP_BOOST = 0;

        SetRandomExpression(2,2,0.5f,1,1);
        SetPokemonFacePlayer(!isPlayerPkmn);
    }

    public void MakePokemonAppear(float delay)
    {
        StartCoroutine(WaitAndMakePokemonAppear(delay));
    }

    private IEnumerator WaitAndMakePokemonAppear(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateFaceToExpression();
        this.animatorController.SetBool("IsPresent", true);
    }

    public void MakePokemonDisappear(float delay)
    {
        StartCoroutine(WaitAndMakePokemonDisappear(delay));
    }

    private IEnumerator WaitAndMakePokemonDisappear(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateFaceToExpression();
        this.animatorController.SetBool("IsPresent", false);
    }


    #endregion

    #region Animations

    public void Defends(float delay)
    {
        this.pokemonObject.DEFENSE_TMP_BOOST += 10;
        StartCoroutine(WaitAndMakePokemonDefend(delay));
    }
    private IEnumerator WaitAndMakePokemonDefend(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.animatorController.SetTrigger("Defends");
    }

    public void Attack(float delay, bool typeAttack)
    {
        StartCoroutine(WaitAndMakePokemonAttack(delay, typeAttack));
    }
    private IEnumerator WaitAndMakePokemonAttack(float delay, bool typeAttack)
    {
        yield return new WaitForSeconds(delay);
        int typeInt = typeAttack ? (this.pokemonObject.mainType == PokemonType.FIRE ? 1 : (this.pokemonObject.mainType == PokemonType.WATER ? 2 : 3)) : 0;
        this.animatorController.SetInteger("TypeAttack", typeInt);
        this.animatorController.SetTrigger("Attacks");
    }

    public void GetHit(float delay)
    {
        StartCoroutine(WaitAndMakePokemonGetHit(delay));
    }
    private IEnumerator WaitAndMakePokemonGetHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.animatorController.SetTrigger("GetsHit");
    }
    
    public int GetsHit(Pokemon attacker, bool attackUsesType, int intensity)
    {
        int hpLost = this.pokemonObject.GetsHit(attacker, attackUsesType, intensity);
        StartCoroutine(WaitAndChangeHPValue(1.1f));

        this.pokemonObject.GetRelationWithPokemon(attacker).ChangeLove(-0.2f);
        this.pokemonObject.GetRelationWithPokemon(attacker).ChangeTrust(-0.1f);
        this.pokemonObject.GetRelationWithPokemon(attacker).ChangeFear(0.1f);
        attacker.GetRelationWithPokemon(this.pokemonObject).ChangeFear(-0.1f);

        return hpLost;
    }

    private void UpdateHPSliderColor()
    {
        int currentSlideValue = Mathf.RoundToInt(this.hpSlider.value);
        if ((currentSlideValue - 1) < (this.pokemonObject.HPMax * 0.2f))
        {
            hpSliderFillImage.color = hpSliderFillColor_Alert;
        }
        else if ((currentSlideValue - 1) < (this.pokemonObject.HPMax * 0.5f))
        {
            hpSliderFillImage.color = hpSliderFillColor_Medium;
        }
        else
        {
            hpSliderFillImage.color = hpSliderFillColor_OK;
        }
    }

    private IEnumerator WaitAndChangeHPValue(float delay)
    {
        yield return new WaitForSeconds(delay);

        int currentSlideValue = Mathf.RoundToInt(this.hpSlider.value);

        if (currentSlideValue > this.pokemonObject.HP)
        {
            int deltaHPChange = Mathf.CeilToInt((currentSlideValue - this.pokemonObject.HP) * 0.1f);
            this.hpSlider.value = currentSlideValue - deltaHPChange;
            UpdateHPSliderColor();
            if (this.hpValueText != null)
            {
                this.hpValueText.text = (currentSlideValue - deltaHPChange) + "/ " + this.pokemonObject.HPMax;
            }

            StartCoroutine(WaitAndChangeHPValue(0.05f));
        }
        else if (currentSlideValue < this.pokemonObject.HP)
        {
            int deltaHPChange = Mathf.CeilToInt((this.pokemonObject.HP - currentSlideValue) * 0.1f);
            this.hpSlider.value = currentSlideValue + deltaHPChange;
            UpdateHPSliderColor();
            if (this.hpValueText != null)
            {
                this.hpValueText.text = (currentSlideValue + deltaHPChange) + "/ " + this.pokemonObject.HPMax;
            }

            StartCoroutine(WaitAndChangeHPValue(0.05f));
        }
    }

    #endregion
    
    #region Heal

    public int Heal()
    {
        this.animatorController.SetTrigger("GetsHealed");
        int hpHealed = this.pokemonObject.Heal(100);
        StartCoroutine(WaitAndChangeHPValue(0.1f));
        return hpHealed;
    }

    #endregion
}
