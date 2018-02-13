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
/// This class manages a Dialogue Panel (the one of the Player, of their Pokémon, or of the enemy Pokémon).
/// </summary>
public class DialoguePanelBehaviour : MonoBehaviour
{
    [Header("Panel Size and Position")]
    public RectTransform dialogueMainPanel;

    public Vector2 smallPhrase_panelSize;
    public Vector2 smallPhrase_panelPosition;

    public Vector2 widePhrase_panelSize;
    public Vector2 widePhrase_panelPosition;

    public Vector2 tallPhrase_panelSize;
    public Vector2 tallPhrase_panelPosition;

    public Vector2 bigPhrase_panelSize;
    public Vector2 bigPhrase_panelPosition;

    [Header("Panels")]
    public RectTransform dialogueContentPanel;
    public RectTransform dialogueBkgContentPanel;
    public RectTransform dialogueTranslationPanel;

    [Header("Translation")]
    public Text englishTranslationText;

    [Header("Prefabs")]
    public GameObject dialoguePinsPrefab; // 60x60
    public GameObject dialogueArrowPrefab; // 24x24

    [Header("Specific pins")]
    public Sprite conjunctionPinsSprite;
    public List<LocalizedText> conjunctionThat;
    public List<LocalizedText> conjunctionIf;

    [Header("Placeholder pins")]
    public Sprite blankMandatorySubjectPinsSprite;
    public Sprite blankMandatoryVerbPinsSprite;
    public Sprite blankMandatoryTraitPinsSprite;
    public Sprite blankMandatoryQuantifierPinsSprite;

    public Sprite blankOptionalySubjectPinsSprite;
    public Sprite blankOptionalVerbPinsSprite;
    public Sprite blankOptionalTraitPinsSprite;
    public Sprite blankOptionalQuantifierPinsSprite;
    public Sprite blankOptionalInterogationPinsSprite;

    public bool interactable;

    private DialoguePhrase currentPhrase;

    public DialogueEngineBehaviour dialogueEngine;

    public DialogueInputPanelBehaviour dialogueInputPanel;

    public BattleGameEngineBehaviour battleGameEngine;

    public GameObject continueOrSayButton;

    public void SelectPins(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        try
        {
            switch (positionInPhrase)
            {
                case DialogueSimplePhrasePosition.MAIN:
                    switch (pinsTypeCode)
                    {
                        case DialoguePinsTypeCode.SUBJECT:
                            currentPhrase.mainSubject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.VERB:
                            currentPhrase.mainVerb = dialogueEngine.GetVerbFromCode(verbCode.Value);
                            break;
                        case DialoguePinsTypeCode.OBJECT:
                            currentPhrase.mainObject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.TRAIT:
                            currentPhrase.mainVerbTrait = dialogueEngine.GetTraitFromCode(traitCode.Value);
                            break;
                        case DialoguePinsTypeCode.QUANTIFIER:
                            currentPhrase.mainVerbTraitQuantifier = dialogueEngine.GetQuantifierFromCode(quantifierCode.Value);
                            break;
                    }
                    break;
                case DialogueSimplePhrasePosition.PROPOSITION:
                    switch (pinsTypeCode)
                    {
                        case DialoguePinsTypeCode.SUBJECT:
                            currentPhrase.proposition.mainSubject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.VERB:
                            currentPhrase.proposition.mainVerb = dialogueEngine.GetVerbFromCode(verbCode.Value);
                            break;
                        case DialoguePinsTypeCode.OBJECT:
                            currentPhrase.proposition.mainObject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.TRAIT:
                            currentPhrase.proposition.mainVerbTrait = dialogueEngine.GetTraitFromCode(traitCode.Value);
                            break;
                        case DialoguePinsTypeCode.QUANTIFIER:
                            currentPhrase.proposition.mainVerbTraitQuantifier = dialogueEngine.GetQuantifierFromCode(quantifierCode.Value);
                            break;
                    }
                    break;
                case DialogueSimplePhrasePosition.CONDITION:
                    switch (pinsTypeCode)
                    {
                        case DialoguePinsTypeCode.SUBJECT:
                            currentPhrase.condition.mainSubject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.VERB:
                            currentPhrase.condition.mainVerb = dialogueEngine.GetVerbFromCode(verbCode.Value);
                            break;
                        case DialoguePinsTypeCode.OBJECT:
                            currentPhrase.condition.mainObject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.TRAIT:
                            currentPhrase.condition.mainVerbTrait = dialogueEngine.GetTraitFromCode(traitCode.Value);
                            break;
                        case DialoguePinsTypeCode.QUANTIFIER:
                            currentPhrase.condition.mainVerbTraitQuantifier = dialogueEngine.GetQuantifierFromCode(quantifierCode.Value);
                            break;
                    }
                    break;
                case DialogueSimplePhrasePosition.CONDITION_PROPOSITION:
                    switch (pinsTypeCode)
                    {
                        case DialoguePinsTypeCode.SUBJECT:
                            currentPhrase.condition.proposition.mainSubject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.VERB:
                            currentPhrase.condition.proposition.mainVerb = dialogueEngine.GetVerbFromCode(verbCode.Value);
                            break;
                        case DialoguePinsTypeCode.OBJECT:
                            currentPhrase.condition.proposition.mainObject = dialogueEngine.GetSubjectFromCode(subjectCode.Value);
                            break;
                        case DialoguePinsTypeCode.TRAIT:
                            currentPhrase.condition.proposition.mainVerbTrait = dialogueEngine.GetTraitFromCode(traitCode.Value);
                            break;
                        case DialoguePinsTypeCode.QUANTIFIER:
                            currentPhrase.condition.proposition.mainVerbTraitQuantifier = dialogueEngine.GetQuantifierFromCode(quantifierCode.Value);
                            break;
                    }
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Receive click on a pin that can't be resolved. Ex = " + ex.Message);
        }

        SetCurrentPhrase(currentPhrase);
    }

    private void ClickOnPins(DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        dialogueInputPanel.gameObject.SetActive(true);
        dialogueInputPanel.SetAvailablePins(pinsTypeCode, positionInPhrase);
    }

    public void SetCurrentPhrase(DialoguePhrase newPhrase)
    {
        currentPhrase = newPhrase;
        ShowPhrase(currentPhrase, interactable);
    }

    private void ClearDialogueContent()
    {
        foreach (Transform child in dialogueContentPanel.transform)
        {
            if (!child.name.Equals("Bkg Content Panel"))
            {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in dialogueBkgContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddConjunctionPins(Vector2 position, string conjunctionStr, string tooltip)
    {
        GameObject pins = (GameObject)Instantiate(dialoguePinsPrefab, dialogueContentPanel.transform);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x, 60);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, position.y, 60);
        pins.GetComponent<DialoguePinsBehaviour>().SetConjunctionPins(conjunctionPinsSprite, conjunctionStr, tooltip);
    }

    private void AddPins(Vector2 position, Sprite sprite, string tooltip, DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        GameObject pins = (GameObject)Instantiate(dialoguePinsPrefab, dialogueContentPanel.transform);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x, 60);
        pins.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, position.y, 60);
        pins.GetComponent<DialoguePinsBehaviour>().SetPins(sprite, tooltip);

        if (interactable && (pinsTypeCode != DialoguePinsTypeCode.PREPOSITION))
        {
            pins.GetComponent<DialoguePinsBehaviour>().SetDelegateAndCode(this.ClickOnPins, pinsTypeCode, positionInPhrase, subjectCode, verbCode, traitCode, quantifierCode);
        }
    }

    private void AddArrow(Vector2 positionA, Vector2 positionB)
    {
        GameObject arrow = (GameObject)Instantiate(dialogueArrowPrefab, dialogueBkgContentPanel.transform);
        Vector2 arrowPosition = Vector2.Lerp(positionA, positionB, 0.5f);
        arrow.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, arrowPosition.x +18, 24);
        arrow.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, arrowPosition.y +18, 24);
        float arrowAngle = (positionA.y == positionB.y) ? ( (positionA.x > positionB.x) ? 180 : 0) : ( (positionA.y > positionB.y) ? 90 : 270 );
        arrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.forward * arrowAngle);
    }

    private void ShowSimplePhrase(DialoguePhrase simplePhrase, Vector2 subjectPosition, DialogueSimplePhrasePosition positionInPhrase, bool isInputPlayer)
    {
        // A simple phrase doesn't have a proposition or condition
        // It's only :
        // Subject -> Verb -> Object
        //              ^
        //            Trait <- Quantifier
        
        Vector2 verbPosition = subjectPosition + Vector2.right * 75;
        Vector2 objectPosition = verbPosition + Vector2.right * 75;
        Vector2 traitPosition = verbPosition + Vector2.up * 75;
        Vector2 quantifierPosition = traitPosition + Vector2.right * 75;

        // Subject
        if (simplePhrase.mainSubject != null)
        {
            AddPins(subjectPosition, simplePhrase.mainSubject.sprite, simplePhrase.mainSubject.Name(true), DialoguePinsTypeCode.SUBJECT, positionInPhrase, simplePhrase.mainSubject.code, null, null, null);
        }
        else
        {
            AddPins(subjectPosition, blankMandatorySubjectPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Qui ?" : "Who?", DialoguePinsTypeCode.SUBJECT, positionInPhrase, null, null, null, null);
        }

        // Verb
        if (simplePhrase.mainVerb != null)
        {
            AddArrow(subjectPosition, verbPosition);
            AddPins(verbPosition, simplePhrase.mainVerb.sprite, simplePhrase.mainVerb.Verb(), DialoguePinsTypeCode.VERB, positionInPhrase, null, simplePhrase.mainVerb.code, null, null);
        }
        else
        {
            AddArrow(subjectPosition, verbPosition);
            AddPins(verbPosition, blankMandatoryVerbPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Fait quoi ?" : "Does what?", DialoguePinsTypeCode.VERB, positionInPhrase, null, null, null, null);
        }

        // Object
        if (simplePhrase.mainObject != null)
        {
            AddArrow(verbPosition, objectPosition);
            AddPins(objectPosition, simplePhrase.mainObject.sprite, simplePhrase.mainObject.Name(false), DialoguePinsTypeCode.OBJECT, positionInPhrase, simplePhrase.mainObject.code, null, null, null);
        }
        else
        {
            if (simplePhrase.mainVerb != null && isInputPlayer)
            {
                if (simplePhrase.mainVerb.mandatoryObject_Character)
                {
                    AddArrow(verbPosition, objectPosition);
                    AddPins(objectPosition, blankMandatorySubjectPinsSprite, Localization.currentLanguage == Language.FRENCH ? "A qui ?" : "To whom?", DialoguePinsTypeCode.OBJECT, positionInPhrase, null, null, null, null);
                }
                else if (simplePhrase.mainVerb.optionalObject_Character)
                {
                    AddArrow(verbPosition, objectPosition);
                    AddPins(objectPosition, blankOptionalySubjectPinsSprite, Localization.currentLanguage == Language.FRENCH ? "A qui ?" : "To whom?", DialoguePinsTypeCode.OBJECT, positionInPhrase, null, null, null, null);
                }
            }
        }

        // Trait
        if (simplePhrase.proposition == null)
        {
            if (simplePhrase.mainVerbTrait != null)
            {
                AddArrow(traitPosition, verbPosition);
                AddPins(traitPosition, simplePhrase.mainVerbTrait.sprite, simplePhrase.mainVerbTrait.Trait(), DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, simplePhrase.mainVerbTrait.code, null);
            }
            else
            {
                if (simplePhrase.mainVerb != null && isInputPlayer)
                {
                    if (simplePhrase.mainVerb.mandatoryTrait)
                    {
                        AddArrow(traitPosition, verbPosition);
                        AddPins(traitPosition, blankMandatoryTraitPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Avec quelle influence?" : "With influence?", DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, null, null);
                    }
                    else if (simplePhrase.mainVerb.optionalTrait)
                    {
                        AddArrow(traitPosition, verbPosition);
                        AddPins(traitPosition, blankOptionalTraitPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Avec quelle influence?" : "With influence?", DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, null, null);
                    }
                }
            }
        }
        else
        {
            Vector2 prepositionPosition = traitPosition;
            traitPosition += Vector2.right * 75;
            if (simplePhrase.mainVerbTrait != null)
            {
                AddArrow(traitPosition, prepositionPosition);
                AddPins(traitPosition, simplePhrase.mainVerbTrait.sprite, simplePhrase.mainVerbTrait.Trait(), DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, simplePhrase.mainVerbTrait.code, null);
            }
            else
            {
                if (simplePhrase.mainVerb != null && isInputPlayer)
                {
                    if (simplePhrase.mainVerb.mandatoryTrait)
                    {
                        AddArrow(traitPosition, prepositionPosition);
                        AddPins(traitPosition, blankMandatoryTraitPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Avec quelle influence?" : "With influence?", DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, null, null);
                    }
                    else if (simplePhrase.mainVerb.optionalTrait)
                    {
                        AddArrow(traitPosition, prepositionPosition);
                        AddPins(traitPosition, blankOptionalTraitPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Avec quelle influence?" : "With influence?", DialoguePinsTypeCode.TRAIT, positionInPhrase, null, null, null, null);
                    }
                }
            }
        }

        // Quantifier
        if (simplePhrase.mainVerbTraitQuantifier != null)
        {
            if (simplePhrase.mainVerb.code == DialogueVerbCode.ATTACK)
            {
                AddArrow(traitPosition, verbPosition);
                AddPins(traitPosition, simplePhrase.mainVerbTraitQuantifier.sprite, simplePhrase.mainVerbTraitQuantifier.Quantifier(), DialoguePinsTypeCode.QUANTIFIER, positionInPhrase, null, null, null, simplePhrase.mainVerbTraitQuantifier.code);
            }
            else
            {
                AddArrow(quantifierPosition, traitPosition);
                AddPins(quantifierPosition, simplePhrase.mainVerbTraitQuantifier.sprite, simplePhrase.mainVerbTraitQuantifier.Quantifier(), DialoguePinsTypeCode.QUANTIFIER, positionInPhrase, null, null, null, simplePhrase.mainVerbTraitQuantifier.code);
            }
        }
        else
        {
            if (simplePhrase.mainVerb != null && isInputPlayer)
            {
                if (simplePhrase.mainVerbTrait != null)
                {
                    if (simplePhrase.mainVerb.mandatoryQuantifier)
                    {
                        AddArrow(quantifierPosition, traitPosition);
                        AddPins(quantifierPosition, blankMandatoryQuantifierPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Combien ?" : "How much?", DialoguePinsTypeCode.QUANTIFIER, positionInPhrase, null, null, null, null);
                    }
                    else if (simplePhrase.mainVerb.optionalQuantifier)
                    {
                        AddArrow(quantifierPosition, traitPosition);
                        AddPins(quantifierPosition, blankOptionalQuantifierPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Combien ?" : "How much?", DialoguePinsTypeCode.QUANTIFIER, positionInPhrase, null, null, null, null);
                    }
                }
                else
                {
                    if (simplePhrase.mainVerb.code == DialogueVerbCode.ATTACK)
                    {
                        AddArrow(traitPosition, verbPosition);
                        AddPins(traitPosition, blankOptionalQuantifierPinsSprite, Localization.currentLanguage == Language.FRENCH ? "Quelle puissance ?" : "How much?", DialoguePinsTypeCode.QUANTIFIER, positionInPhrase, null, null, null, null);
                    }
                }
            }
        }
    }

    private void SetDialoguePanelSize(DialoguePhrase phrase)
    {
        // Dialogue panel
        Vector2 panelSize = smallPhrase_panelSize;
        Vector2 panelPosition = smallPhrase_panelPosition;

        if (phrase.proposition != null)
        {
            if (phrase.condition != null)
            {
                panelSize = bigPhrase_panelSize;
                panelPosition = bigPhrase_panelPosition;
            }
            else
            {
                panelSize = tallPhrase_panelSize;
                panelPosition = tallPhrase_panelPosition;
            }
        }
        else
        {
            if (phrase.condition != null)
            {
                if (phrase.condition.proposition != null)
                {
                    panelSize = bigPhrase_panelSize;
                    panelPosition = bigPhrase_panelPosition;
                }
                else
                {
                    panelSize = widePhrase_panelSize;
                    panelPosition = widePhrase_panelPosition;
                }
            }
            else
            {
                panelSize = smallPhrase_panelSize;
                panelPosition = smallPhrase_panelPosition;
            }
        }
        
        dialogueMainPanel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, panelPosition.x, panelSize.x);
        dialogueMainPanel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, panelPosition.y, panelSize.y);

        // Translation panel
        string translation = phrase.ToString();
        int characterCountByLine = 24;
        if (phrase.condition != null)
        {
            characterCountByLine = 33;
        }
        int translationLineCount = Mathf.CeilToInt( translation.Length * 1.0f / characterCountByLine );
        dialogueTranslationPanel.pivot = new Vector2(0.5f, 1.0f);
        float panelHeight = translationLineCount * 24 + 52;
        dialogueTranslationPanel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 40 - panelHeight, panelHeight);
    }

    private DialoguePhrase GetFixedPhrase(DialoguePhrase oldPhrase)
    {
        DialoguePhrase phrase = oldPhrase;

        if (phrase.mainVerb != null)
        {
            if (phrase.mainObject != null && !phrase.mainVerb.mandatoryObject_Character && !phrase.mainVerb.optionalObject_Character && !phrase.mainVerb.mandatoryObject_State && !phrase.mainVerb.optionalObject_State)
            {
                phrase.mainObject = null;
            }
            if (phrase.mainVerbTrait != null && !phrase.mainVerb.mandatoryTrait && !phrase.mainVerb.optionalTrait)
            {
                phrase.mainVerbTrait = null;
            }
            if (phrase.mainVerbTraitQuantifier != null && !phrase.mainVerb.mandatoryQuantifier && !phrase.mainVerb.optionalQuantifier)
            {
                phrase.mainVerbTraitQuantifier = null;
            }
        }

        if (phrase.mainVerb != null)
        {
            if (phrase.mainVerb.mandatoryProposition_differentSubject)
            {
                if (phrase.proposition == null)
                {
                    phrase.proposition = new DialoguePhrase();
                }
            }
            else if (phrase.mainVerb.mandatoryProposition_sameSubject)
            {
                if (phrase.proposition == null)
                {
                    phrase.proposition = new DialoguePhrase();
                }
                phrase.proposition.mainSubject = phrase.mainSubject;
            }
            else if (phrase.mainVerb.optionalProposition_differentSubject)
            {
                if (phrase.proposition == null)
                {
                    phrase.proposition = new DialoguePhrase();
                }
            }
            else if (phrase.mainVerb.optionalProposition_sameSubject)
            {
                if (phrase.proposition == null)
                {
                    phrase.proposition = new DialoguePhrase();
                }
                phrase.proposition.mainSubject = phrase.mainSubject;
            }
            else
            {
                phrase.proposition = null;
            }
        }

        if (phrase.mainVerb != null)
        {
            if (phrase.mainVerb.mandatoryCondition)
            {
                if (phrase.condition == null)
                {
                    phrase.condition = new DialoguePhrase();
                }
            }
            else if (phrase.mainVerb.optionalCondition && phrase.condition == null)
            {
                if (phrase.condition == null)
                {
                    phrase.condition = new DialoguePhrase();
                }
            }
            else
            {
                phrase.condition = null;
            }
        }

        if (phrase.condition != null && phrase.condition.mainVerb != null)
        {
            if (phrase.condition.mainVerb.mandatoryProposition_differentSubject)
            {
                if (phrase.condition.proposition == null)
                {
                    phrase.condition.proposition = new DialoguePhrase();
                }
            }
            else if (phrase.condition.mainVerb.mandatoryProposition_sameSubject)
            {
                if (phrase.condition.proposition == null)
                {
                    phrase.condition.proposition = new DialoguePhrase();
                }
                phrase.condition.proposition.mainSubject = phrase.condition.mainSubject;
            }
            else if (phrase.condition.mainVerb.optionalProposition_differentSubject)
            {
                if (phrase.condition.proposition == null)
                {
                    phrase.condition.proposition = new DialoguePhrase();
                }
            }
            else if (phrase.condition.mainVerb.optionalProposition_sameSubject)
            {
                if (phrase.condition.proposition == null)
                {
                    phrase.condition.proposition = new DialoguePhrase();
                }
                phrase.condition.proposition.mainSubject = phrase.condition.mainSubject;
            }
            else
            {
                phrase.condition.proposition = null;
            }
        }

        return phrase;
    }

    private void ShowPhrase(DialoguePhrase oldPhrase, bool isInputPlayer)
    {
        ClearDialogueContent();

        // Fix phrase according to mandatory stuffity stuff
        DialoguePhrase phrase = GetFixedPhrase(oldPhrase);

        // Main Phrase
        Vector2 subjectPosition = new Vector2(10, 10);
        ShowSimplePhrase(phrase, subjectPosition, DialogueSimplePhrasePosition.MAIN, isInputPlayer);

        // Proposition
        if (phrase.proposition != null)
        {
            // simple phrase representing proposition
            Vector2 subjectPropositionPosition = subjectPosition + Vector2.right * 75 + Vector2.up * 75 * 2;
            ShowSimplePhrase(phrase.proposition, subjectPropositionPosition, DialogueSimplePhrasePosition.PROPOSITION, isInputPlayer);

            // Pins "That"
            Vector2 thatPinsPosition = subjectPosition + Vector2.right * 75 + Vector2.up * 75;            
            AddConjunctionPins(thatPinsPosition, Localization.GetLocalizedString(conjunctionThat).ToUpper(), Localization.GetLocalizedString(conjunctionThat).ToLower() + "...");

            // Arrows
            AddArrow(thatPinsPosition - Vector2.up * 75, thatPinsPosition);
            AddArrow(thatPinsPosition, subjectPropositionPosition);
        }

        // Condition
        if (phrase.condition != null)
        {
            // simple phrase representing condition
            Vector2 subjectConditionPosition = subjectPosition + Vector2.right * 75 * 4;
            ShowSimplePhrase(phrase.condition, subjectConditionPosition, DialogueSimplePhrasePosition.CONDITION, isInputPlayer);

            // Pins "If"
            Vector2 ifPinsPosition = subjectPosition + Vector2.right * 75 * 3;
            AddConjunctionPins(ifPinsPosition, Localization.GetLocalizedString(conjunctionIf).ToUpper(), Localization.GetLocalizedString(conjunctionIf).ToLower() + "...");

            // Arrows
            AddArrow(ifPinsPosition - Vector2.right * 75, ifPinsPosition);
            AddArrow(ifPinsPosition, subjectConditionPosition);
        }

        // Proposition of Condition
        if (phrase.condition != null && phrase.condition.proposition != null)
        {
            // simple phrase representing proposition
            Vector2 conditionPropositionPosition = subjectPosition + Vector2.right * 75 * 5 + Vector2.up * 75 * 2;
            ShowSimplePhrase(phrase.condition.proposition, conditionPropositionPosition, DialogueSimplePhrasePosition.CONDITION_PROPOSITION, isInputPlayer);

            // Pins "That"
            Vector2 thatPinsPosition = subjectPosition + Vector2.right * 75 * 5 + Vector2.up * 75;
            AddConjunctionPins(thatPinsPosition, Localization.GetLocalizedString(conjunctionThat).ToUpper(), Localization.GetLocalizedString(conjunctionThat).ToLower() + "...");

            // Arrows
            AddArrow(thatPinsPosition - Vector2.up * 75, thatPinsPosition);
            AddArrow(thatPinsPosition, conditionPropositionPosition);
        }

        SetDialoguePanelSize(phrase);

        // Translation
        englishTranslationText.text = phrase.ToString();

        // Send button status
        continueOrSayButton.GetComponent<Button>().interactable = phrase.IsComplete() || !this.interactable;
    }

    public void OnSendClick()
    {
        // Send phrase to Battle engine
        this.gameObject.SetActive(false);
        battleGameEngine.PlayerSaysPhrase(this.currentPhrase);
    }

    public void OnContinueClick()
    {
        // Make battle engine proceed
        battleGameEngine.CloseAllDialoguePanels();
        battleGameEngine.ResolveCurrentState();
    }

    public void SetDefaultInputPhrase(DialogueSubjectCode defaultSubject)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = dialogueEngine.GetSubjectFromCode(defaultSubject);
        SetCurrentPhrase(phrase);
    }
}
