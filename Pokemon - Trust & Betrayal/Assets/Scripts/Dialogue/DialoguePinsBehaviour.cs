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
/// This delegate is used to deal with a click on a Pin (that actually does something in the case of an editable Pin)
/// </summary>
public delegate void HandleClickOnPins(DialoguePinsTypeCode typeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode);

/// <summary>
/// Each dialogue Pins should have a Component of type DialoguePinsBehaviour.
/// This class deals with the content of the Pin, and the format and visibility status of its tooltip.
/// </summary>
public class DialoguePinsBehaviour : MonoBehaviour
{
    public Image pinsImage;

    public GameObject pinsTooltipGameObject;
    public Text pinsTooltipText;

    public GameObject pinsConjunctionGameObject;


    private HandleClickOnPins clickOnPinsDelegate;

    private DialoguePinsTypeCode pinsTypeCode;
    private DialogueSimplePhrasePosition positionInPhrase;

    private DialogueSubjectCode? subjectCode;
    private DialogueVerbCode? verbCode;
    private DialogueTraitCode? traitCode;
    private DialogueQuantifierCode? quantifierCode;

    void Start()
    {
        string formattedTooltip;
        Vector2 sizeOfString = SizeOfText(pinsTooltipText.text, out formattedTooltip);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(sizeOfString.x * 7 + 23) / 2, sizeOfString.x * 7 + 23);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5, sizeOfString.y * 15);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
        pinsTooltipText.text = formattedTooltip;
    }

    private static Vector2 SizeOfText(string originalString, out string formattedString)
    {
        int width = 1;
        int height = 1;
        List<char> separators = new List<char>() { ' ', '\n' };
        string[] words = originalString.Split(separators.ToArray());
        if (words.Length == 1)
        {
            height = 1;
            width = words[0].Length;
            formattedString = originalString;
        }
        else if (words.Length == 2)
        {
            height = 2;
            width = Mathf.Max (words[0].Length, words[1].Length);
            formattedString = words[0] + "\n" + words[1];
        }
        else if (words.Length == 3)
        {
            height = 2;
            if (words[0].Length > words[2].Length)
            {
                width = Mathf.Max(words[0].Length, words[1].Length + words[2].Length + 1);
                formattedString = words[0] + "\n" + words[1] + " " + words[2];
            }
            else
            {
                width = Mathf.Max(words[0].Length + words[1].Length + 1, words[2].Length);
                formattedString = words[0] + " " + words[1] + "\n" + words[2];
            }
        }
        else
        {
            height = 4;
            width = Mathf.Max(Mathf.Max(words[0].Length, words[1].Length),Mathf.Max(words[2].Length, words[3].Length));
            formattedString = words[0] + "\n" + words[1] + "\n" + words[2] + "\n" + words[3];
            Debug.LogWarning("sizeOfText: not fully implemented");
        }

        return new Vector2(width, height);
    }

    public void SetDelegateAndCode(HandleClickOnPins clickOnPinsDelegate, DialoguePinsTypeCode pinsTypeCode, DialogueSimplePhrasePosition positionInPhrase, DialogueSubjectCode? subjectCode, DialogueVerbCode? verbCode, DialogueTraitCode? traitCode, DialogueQuantifierCode? quantifierCode)
    {
        this.clickOnPinsDelegate = clickOnPinsDelegate;
        this.pinsTypeCode = pinsTypeCode;
        this.positionInPhrase = positionInPhrase;
        this.subjectCode = subjectCode;
        this.verbCode = verbCode;
        this.traitCode = traitCode;
        this.quantifierCode = quantifierCode;
    }

    public void SetPins(Sprite pinsSprite, string pinsTooltip)
    {
        pinsImage.sprite = pinsSprite;

        string formattedTooltip;
        Vector2 sizeOfString = SizeOfText(pinsTooltip, out formattedTooltip);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(sizeOfString.x * 7 + 23 )/ 2, sizeOfString.x * 7 + 23);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5, sizeOfString.y * 15);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);

        pinsConjunctionGameObject.SetActive(false);

        pinsTooltipText.text = formattedTooltip;
    }

    public void SetConjunctionPins(Sprite pinsSprite, string conjunctionStr, string pinsTooltip)
    {
        pinsImage.sprite = pinsSprite;

        string formattedTooltip;
        Vector2 sizeOfString = SizeOfText(pinsTooltip, out formattedTooltip);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(sizeOfString.x * 7 + 23) / 2, sizeOfString.x * 7 + 23);
        pinsTooltipGameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5, sizeOfString.y * 15);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        pinsTooltipGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);

        pinsConjunctionGameObject.SetActive(true);
        pinsConjunctionGameObject.GetComponent<Text>().text = conjunctionStr;

        pinsTooltipText.text = formattedTooltip;
    }

    public void OnCursorEnter()
    {
        pinsTooltipGameObject.SetActive(true);
    }

    public void OnCursorExit()
    {
        pinsTooltipGameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (clickOnPinsDelegate != null)
        {
            clickOnPinsDelegate(pinsTypeCode, positionInPhrase, subjectCode, verbCode, traitCode, quantifierCode);
        }
    }

}
