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

/// <summary>
/// The main imput panel during a battle is where info are written.
/// It also serves as the main menu (TALK, BAG, PKMN, RUN)
/// </summary>
public class BattleMainInputPanelBehaviour : MonoBehaviour
{
    public GameObject menuPanel;

    public GameObject mainTextPanel;
    public Text mainText;

    public BattleGameEngineBehaviour battleEngine;

    public MainGameEngineBehaviour gameEngine;

    public float delayTextVisibility;

    void Start()
    {
        menuPanel.SetActive(false);
        mainTextPanel.SetActive(false);
    }

    public void DisplayText(string text, bool backToMenu)
    {
        float delay = 0;
        StartCoroutine(WaitAndShowText(delay, text));
        delay += delayTextVisibility;
        if (backToMenu)
        {
            StartCoroutine(WaitAndShowMenu(delay));
        }
    }

    public void DisplayText(List<string> textList, bool backToMenu)
    {
        float delay = 0;
        foreach (string txt in textList)
        {
            StartCoroutine(WaitAndShowText(delay, txt));
            delay += delayTextVisibility;
        }
        if (backToMenu)
        {
            StartCoroutine(WaitAndShowMenu(delay));
        }
    }

    public void RemoveText()
    {
        ShowText("");
    }

    private void ShowText(string str)
    {
        menuPanel.SetActive(false);
        mainText.text = str;
        mainTextPanel.SetActive(true);
    }

    private IEnumerator WaitAndShowText(float delay, string text)
    {
        yield return new WaitForSeconds(delay);
        ShowText(text);
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        mainTextPanel.SetActive(false);
    }

    private IEnumerator WaitAndShowMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowMenu();
    }

    public void ClickOnTalkButton()
    {
        if (Localization.currentLanguage == Language.FRENCH)
        {
            ShowText("Utilise le langage Pokémon pour leur parler.");
        }
        else
        {
            ShowText("Use Pokémon language to talk");
        }
        battleEngine.OpenPlayerInputDialogueWindow();
    }

    public void ClickOnPkmnButton()
    {
        List<Pokemon> allPokemonsOfPlayer = gameEngine.listOfPokemonsOwnedByPlayer;
        Pokemon inBattlePokemon = battleEngine.playerBattlePkmn.pokemonObject;

        List<Pokemon> availablePokemons = new List<Pokemon>();
        foreach (Pokemon pkmn in allPokemonsOfPlayer)
        {
            if (pkmn.ID != inBattlePokemon.ID)
            {
                if (pkmn.HP > 0)
                {
                    // this pokemon can fight !
                    availablePokemons.Add(pkmn);
                }
            }
        }

        if (Localization.currentLanguage == Language.FRENCH)
        {
            if (availablePokemons.Count > 0)
            {
                DisplayText("Tu as " + availablePokemons.Count + " Pokémons disponibles", true);
            }
            else
            {
                DisplayText("Tu n'as pas d'autres Pokémons disponibles.", true);
            }
        }
        else
        {
            if (availablePokemons.Count > 0)
            {
                DisplayText("You have " + availablePokemons.Count + " available Pokémons", true);
            }
            else
            {
                DisplayText("You don't have other Pokémons", true);
            }
        }
    }

    #region BAG

    [Header("BAG")]
    public GameObject bagPanel;
    public GameObject potionButton;
    public Text potionText;
    public GameObject rockButton;
    public Text rockText;
    public GameObject pokeballButton;
    public Text pokeballText;

    [Header("BAG - Potion")]
    public GameObject potionUsedPanel;
    public DialoguePinsBehaviour playerPokemonPins;

    public void ClickOnBagButton()
    {
        if (bagPanel.activeInHierarchy)
        {
            bagPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        else
        {
            List<Item> itemsInBag = gameEngine.listOfItemsInInventory;

            int potionQuantityInBag = 0;
            int rockQuantityInBag = 0;
            int pokeballQuantityInBag = 0;

            foreach (Item item in itemsInBag)
            {
                switch (item.code)
                {
                    case ItemCode.POKEBALL:
                        pokeballQuantityInBag = item.quantity;
                        break;
                    case ItemCode.POTION:
                        potionQuantityInBag = item.quantity;
                        break;
                    case ItemCode.ROCK:
                        rockQuantityInBag = item.quantity;
                        break;
                }
            }

            if (potionQuantityInBag <= 0 && rockQuantityInBag <= 0 && pokeballQuantityInBag <= 0)
            {
                if (Localization.currentLanguage == Language.FRENCH)
                {
                    DisplayText("Ton SAC est vide.", true);
                }
                else
                {
                    DisplayText("You don't have items in your BAG.", true);
                }
            }
            else
            {
                potionButton.SetActive(potionQuantityInBag > 0);
                potionText.text = "Potion x" + potionQuantityInBag;

                pokeballButton.SetActive(pokeballQuantityInBag > 0);
                pokeballText.text = "Pokéball x" + pokeballQuantityInBag;

                rockButton.SetActive(rockQuantityInBag > 0);
                rockText.text = "Rock x" + rockQuantityInBag;

                bagPanel.SetActive(true);
                menuPanel.SetActive(false);
            }
        }
    }

    public void ClickOnPotionButton()
    {
        bagPanel.SetActive(false);
        gameEngine.RemoveItems(ItemCode.POTION, 1);
        //playerPokemonPins.SetPins(DialogueEngineBehaviour.instance.GetPokemonSpriteFromCode(battleEngine.playerBattlePkmn.pokemonObject.code).subjectPinsSprite, battleEngine.playerBattlePkmn.pokemonObject.name.ToUpper());
        //potionUsedPanel.SetActive(true);
        //StartCoroutine(WaitAndHideGameObject(potionUsedPanel, delayTextVisibility));
        battleEngine.PlayerUsesPotion();
    }

    private IEnumerator WaitAndHideGameObject(GameObject objectToHide, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectToHide.SetActive(false);
    }

    public void ClickOnRockButton()
    {
        bagPanel.SetActive(false);
    }
    public void ClickOnPokeballButton()
    {
        bagPanel.SetActive(false);
    }

    #endregion

    public void ClickOnRunButton()
    {
        if (Localization.currentLanguage == Language.FRENCH)
        {
            DisplayText("Passe en version PREMIUM pour utiliser cette compétence.", true);
        }
        else
        {
            DisplayText("Buy the full game to unlock this skill.", true);
        }
    }
}
