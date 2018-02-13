/* 
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * AUTHOR: Rémi Fusade
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCode
{
    POTION,
    POKEBALL,
    ROCK
}

[System.Serializable]
public class Item
{
    public ItemCode code;
    public int quantity;
}

/// <summary>
/// This class contains and manages data of the game : list of existing Pokémons, list of Pokémons owned by Player, and list of Items owned by Player.
/// </summary>
public class MainGameEngineBehaviour : MonoBehaviour
{
    [Header("Existing Pokemons")]
    public List<Pokemon> listOfAllAvailablePokemons;
    private Dictionary<int, Pokemon> dictionaryOfAvailablePokemons;

    private List<Pokemon> listOfAllExistingPokemons;

    [Header("Pokemons owned by player")]
    public List<Pokemon> listOfPokemonsOwnedByPlayer;

    [Header("Items owned by player")]
    public List<Item> listOfItemsInInventory;

    [Header("References to scene objects")]
    public BattleGameEngineBehaviour battleGameEngine;

    public EventEngineBehaviour eventEngine;

    public GameObject titlePanel;

    public AudioSource backgroundMusicAudioSource;

    public BattleOverlayPanelBehaviour battleOverlay;

    [Header("Play intro ?")]
    public bool playIntro;


    public Pokemon AddExistingPokemonFromID(int pokemonID)
    {
        return AddExistingPokemonFromAvailablePokemon(dictionaryOfAvailablePokemons[pokemonID]);
    }

    public Pokemon AddExistingPokemonFromAvailablePokemon(Pokemon modelPkmn)
    {
        Pokemon newPkmn = Pokemon.Copy(modelPkmn);

        newPkmn.ID = listOfAllExistingPokemons.Count+1;

        // Create relations of this new pokemon

        newPkmn.relationWithPlayer = new Relation() { love = -0.1f, trust = -0.1f, fear = 0.1f }; // relations with player start neutral
        newPkmn.relationWithAntagonist = new Relation() { love = -0.4f, trust = -0.3f, fear = 0.2f }; // relations with antagonist start low

        foreach (Pokemon pkmn in listOfAllExistingPokemons)
        {
            float loveFromNewPkmnToPkmn = Random.Range(pkmn.goodness - 0.3f, pkmn.goodness + 0.3f);
            loveFromNewPkmnToPkmn = loveFromNewPkmnToPkmn > 1 ? 1 : (loveFromNewPkmnToPkmn < -1 ? -1 : loveFromNewPkmnToPkmn);
            float trustFromNewPkmnToPkmn = Random.Range(pkmn.honesty - 0.3f, pkmn.honesty + 0.3f);
            trustFromNewPkmnToPkmn = trustFromNewPkmnToPkmn > 1 ? 1 : (trustFromNewPkmnToPkmn < -1 ? -1 : trustFromNewPkmnToPkmn);
            float fearFromNewPkmnToPkmn = Random.Range(pkmn.power - 0.3f, pkmn.power + 0.3f);
            fearFromNewPkmnToPkmn = fearFromNewPkmnToPkmn > 1 ? 1 : (fearFromNewPkmnToPkmn < -1 ? -1 : fearFromNewPkmnToPkmn);
            Relation relationWithPkmn = new Relation() { target = pkmn, love = loveFromNewPkmnToPkmn, trust = trustFromNewPkmnToPkmn, fear = fearFromNewPkmnToPkmn };
            newPkmn.relationsWithOtherPokemons.Add(relationWithPkmn);

            float loveFromPkmnToNewPkmn = Random.Range(newPkmn.goodness - 0.3f, newPkmn.goodness + 0.3f);
            loveFromPkmnToNewPkmn = loveFromPkmnToNewPkmn > 1 ? 1 : (loveFromPkmnToNewPkmn < -1 ? -1 : loveFromPkmnToNewPkmn);
            float trustFromPkmnToNewPkmn = Random.Range(newPkmn.honesty - 0.3f, newPkmn.honesty + 0.3f);
            trustFromPkmnToNewPkmn = trustFromPkmnToNewPkmn > 1 ? 1 : (trustFromPkmnToNewPkmn < -1 ? -1 : trustFromPkmnToNewPkmn);
            float fearFromPkmnToNewPkmn = Random.Range(newPkmn.power - 0.3f, newPkmn.power + 0.3f);
            fearFromPkmnToNewPkmn = fearFromPkmnToNewPkmn > 1 ? 1 : (fearFromPkmnToNewPkmn < -1 ? -1 : fearFromPkmnToNewPkmn);
            Relation relationWithPkmnReciprocate = new Relation() { target = newPkmn, love = loveFromPkmnToNewPkmn, trust = trustFromPkmnToNewPkmn, fear = fearFromPkmnToNewPkmn };
            pkmn.relationsWithOtherPokemons.Add(relationWithPkmnReciprocate);
        }

        listOfAllExistingPokemons.Add(newPkmn);

        return newPkmn;
    }

    public void StartBattleAgainstPokemon(Pokemon enemyPokemon, float delayBattle, float delayOverlay)
    {
        battleOverlay.ShowBattleOverlay(delayOverlay);
        StartCoroutine(WaitAndStartBattle(delayBattle, enemyPokemon));
    }

    public void CapturePokemon(Pokemon capturedPkmn, string nickname)
    {
        if (!string.IsNullOrEmpty(nickname))
        {
            capturedPkmn.name = nickname;
        }
        listOfPokemonsOwnedByPlayer.Add(capturedPkmn);
    }

    // Use this for initialization
    void Start()
    {
        Screen.SetResolution(800, 600, false);

        foreach (Transform mainPanel in GameObject.Find("Canvas").transform)
        {
            mainPanel.gameObject.SetActive(true);
        }
        

        dictionaryOfAvailablePokemons = new Dictionary<int, Pokemon>();
        foreach (Pokemon pkmn in listOfAllAvailablePokemons)
        {
            dictionaryOfAvailablePokemons.Add(pkmn.ID, pkmn);
        }

        listOfAllExistingPokemons = new List<Pokemon>();

        //listOfPokemonsOwnedByPlayer = new List<Pokemon>();

        if (!playIntro)
        {
            StartGame();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartBattle(Pokemon enemyPkmn)
    {
        battleGameEngine.StartBattle(listOfPokemonsOwnedByPlayer[0], enemyPkmn, false);
    }

    private IEnumerator WaitAndStartBattle(float delay, Pokemon enemyPkmn)
    {
        yield return new WaitForSeconds(delay);
        StartBattle(enemyPkmn);
    }

    public void StartGame()
    {
        backgroundMusicAudioSource.Stop();
        titlePanel.SetActive(false);
        eventEngine.ShowFirstEvent();
    }

    public void SwitchLanguage()
    {
        LocalizedItemBehaviour.SwitchLanguage();
    }

    #region Items

    public void RemoveItems(ItemCode code, int quantity)
    {
        int indexOfItem = -1;
        int index = 0;
        foreach (Item item in listOfItemsInInventory)
        {
            if (item.code == code)
            {
                indexOfItem = index;
                break;
            }
            index++;
        }

        listOfItemsInInventory[indexOfItem].quantity -= quantity;
    }

    #endregion
}
