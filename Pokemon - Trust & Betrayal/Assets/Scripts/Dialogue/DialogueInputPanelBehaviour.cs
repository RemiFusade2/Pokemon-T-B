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
/// This class manages the Dialogue Input Panel visible when the Player clicks on an editable Pin (when trying to create a phrase with the TALK ability).
/// </summary>
public class DialogueInputPanelBehaviour : MonoBehaviour
{
    public DialogueEngineBehaviour dialogueEngine;

    public DialoguePanelBehaviour dialoguePanel;

    public RectTransform inputMainPanel;
    public RectTransform contentPanel;

    public GameObject pinsPrefab;

    private void ClickOnPins(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        dialoguePanel.SelectPins(pinsTypeCode, positionInPhrase, subjectCode, verbCode, traitCode, quantifierCode);
        ClearContent();
        this.gameObject.SetActive(false);
    }

    public void CancelInput()
    {
        ClearContent();
        this.gameObject.SetActive(false);
    }

    private int NumberOfPinsByRow(int numberOfPins)
    {
        if (numberOfPins <= 6)
        {
            return 2;
        }
        else if (numberOfPins <= 12)
        {
            return 3;
        }
        else
        {
            return 4;
        }

    }

    private void SetInputPanelSizeAndPosition(int numberOfPins, int numberOfPinsByRow, Vector2 positionOfCursor)
    {
        float width = 50 + numberOfPinsByRow * 75;
        float height = 50 + Mathf.CeilToInt(numberOfPins * 1.0f / numberOfPinsByRow) * 75;

        Vector2 sizeOfDialoguePanel = dialoguePanel.GetComponent<RectTransform>().sizeDelta;
        Vector2 positionOfDialoguePanel = dialoguePanel.GetComponent<RectTransform>().anchoredPosition;

        Vector2 positionOfCursorInDialoguePanelFrameReference = new Vector2(positionOfCursor.x - positionOfDialoguePanel.x, -positionOfCursor.y + positionOfDialoguePanel.y + sizeOfDialoguePanel.y);

        Vector2 pinsPosition = (positionOfCursorInDialoguePanelFrameReference - 65 * new Vector2(1, 1)) / 75;

        Vector2 positionOfArrow = new Vector2 ( 
            Mathf.RoundToInt(pinsPosition.x) * 75 + 65, 
            Mathf.RoundToInt(pinsPosition.y) * 75 + 65);
        
        inputMainPanel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, positionOfArrow.x - inputMainPanel.pivot.x * width + 35, width);
        inputMainPanel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, positionOfArrow.y - inputMainPanel.pivot.y * height, height);
    }

    private void ClearContent()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddPins(Vector2 position, Sprite sprite, string tooltip, DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        GameObject pins = (GameObject)Instantiate(pinsPrefab, contentPanel.transform);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x, 60);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, position.y, 60);
        pins.GetComponent<DialoguePinsBehaviour>().SetPins(sprite, tooltip);
        
        pins.GetComponent<DialoguePinsBehaviour>().SetDelegateAndCode(this.ClickOnPins, pinsTypeCode, positionInPhrase, subjectCode, verbCode, traitCode, quantifierCode);
    }

    public void SetAvailablePins(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase)
    {
        try
        {
            if (pinsTypeCode == DialoguePinsTypeCode.SUBJECT || pinsTypeCode == DialoguePinsTypeCode.OBJECT)
            {
                ShowAllSubjects(pinsTypeCode, positionInPhrase);
            }
            else if (pinsTypeCode == DialoguePinsTypeCode.VERB)
            {
                ShowAllVerbs(pinsTypeCode, positionInPhrase);
            }
            else if (pinsTypeCode == DialoguePinsTypeCode.TRAIT)
            {
                ShowAllTraits(pinsTypeCode, positionInPhrase);
            }
            else if (pinsTypeCode == DialoguePinsTypeCode.QUANTIFIER)
            {
                ShowAllQuantifiers(pinsTypeCode, positionInPhrase);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Receive click on a pin that can't be resolved. Ex = " + ex.Message);
        }
    }

    private void ShowAllSubjects(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase)
    {
        ClearContent();
        SetInputPanelSizeAndPosition(4, 2, Input.mousePosition);

        List<Vector2> positionsList = new List<Vector2>()
        {
            new Vector2(10, 10),
            new Vector2(85, 10),
            new Vector2(10, 85),
            new Vector2(85, 85)
        };

        int index = 0;

        AddPins(positionsList[index], dialogueEngine.playerSubject.sprite, dialogueEngine.playerSubject.Name(false), pinsTypeCode, positionInPhrase, DialogueSubjectCode.PLAYER, null, null, null);
        index++;

        if (dialogueEngine.isThereEnemy)
        {
            AddPins(positionsList[index], dialogueEngine.enemySubject.sprite, dialogueEngine.enemySubject.Name(false), pinsTypeCode, positionInPhrase, DialogueSubjectCode.ENEMY, null, null, null);
            index++;
        }

        AddPins(positionsList[index], dialogueEngine.playerPokemonSubject.sprite, dialogueEngine.playerPokemonSubject.Name(false), pinsTypeCode, positionInPhrase, DialogueSubjectCode.PLAYER_PKMN, null, null, null);
        index++;

        AddPins(positionsList[index], dialogueEngine.enemyPokemonSubject.sprite, dialogueEngine.enemyPokemonSubject.Name(false), pinsTypeCode, positionInPhrase, DialogueSubjectCode.ENEMY_PKMN, null, null, null);
    }

    private void ShowAllTraits(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase)
    {
        ClearContent();
        SetInputPanelSizeAndPosition(3, 1, Input.mousePosition);
        
        List<DialogueTrait> listOfTraits = dialogueEngine.possibleTraits;

        Vector2 position = new Vector2(10, 10);
        foreach (DialogueTrait trait in listOfTraits)
        {
            AddPins(position, trait.sprite, trait.Trait(), pinsTypeCode, positionInPhrase, null, null, trait.code, null);
            position += Vector2.up * 75;
        }
    }

    private void ShowAllQuantifiers(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase)
    {
        ClearContent();
        SetInputPanelSizeAndPosition(4, 2, Input.mousePosition);

        List<DialogueQuantifier> listOfQuantifiers = dialogueEngine.possibleQuantifiers;

        List<Vector2> positionsList = new List<Vector2>()
        {
            new Vector2(10, 10),
            new Vector2(85, 10),
            new Vector2(10, 85),
            new Vector2(85, 85)
        };

        int index = 0;

        foreach (DialogueQuantifier quantifier in listOfQuantifiers)
        {
            AddPins(positionsList[index], quantifier.sprite, quantifier.Quantifier(), pinsTypeCode, positionInPhrase, null, null, null, quantifier.code);
            index++;
        }
    }

    private void ShowAllVerbs(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase)
    {
        ClearContent();

        List<DialogueVerb> listOfAllVerbs = dialogueEngine.possibleVerbs;
        List<DialogueVerb> listOfUsableVerbs = new List<DialogueVerb>();

        foreach (DialogueVerb verb in listOfAllVerbs)
        {
            if ( (positionInPhrase == DialogueSimplePhrasePosition.MAIN && verb.usableInMainPhrase) ||
                ( (positionInPhrase == DialogueSimplePhrasePosition.PROPOSITION || positionInPhrase == DialogueSimplePhrasePosition.CONDITION_PROPOSITION) && verb.usableInProposition) ||
                (positionInPhrase == DialogueSimplePhrasePosition.CONDITION && verb.usableInCondition) )
            {
                listOfUsableVerbs.Add(verb);
            }
        }

        int numberOfPinsByRow = NumberOfPinsByRow(listOfUsableVerbs.Count);
        SetInputPanelSizeAndPosition(listOfUsableVerbs.Count, numberOfPinsByRow, Input.mousePosition);
        Vector2 position = new Vector2(10, 10);
        foreach (DialogueVerb verb in listOfUsableVerbs)
        {
            AddPins(position, verb.sprite, verb.Verb(), pinsTypeCode, positionInPhrase, null, verb.code, null, null);
            position += Vector2.right * 75;
            if (position.x > (numberOfPinsByRow*75) )
            {
                position.x = 10;
                position += Vector2.up * 75;
            }
        }

    }
}
