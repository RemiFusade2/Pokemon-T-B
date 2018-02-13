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

[System.Serializable]
public enum Language
{
    FRENCH,
    ENGLISH
}

[System.Serializable]
public class LocalizedText
{
    public Language lang;
    public string text;
}

/// <summary>
/// This class manage all visible texts that must be translated.
/// Each visible Text object in Unity Scene must have a Component LocalizedItemBehaviour containing every available language and its associated string of text.
/// This class will then choose the appropriate string of text according to the user preferences.
/// </summary>
public class LocalizedItemBehaviour : MonoBehaviour
{
    public static List<LocalizedItemBehaviour> listOfLocalizedItems;

    public static void AddLocalizedItem(LocalizedItemBehaviour item)
    {
        if (listOfLocalizedItems == null)
        {
            listOfLocalizedItems = new List<LocalizedItemBehaviour>();
        }
        listOfLocalizedItems.Add(item);
    }

    public Text localizableText;

    public List<LocalizedText> listOfLocalizedTexts;


    // Use this for initialization
    void Start ()
    {
        this.Localize(Localization.currentLanguage);
        AddLocalizedItem(this);
    }

    public void Localize(Language language)
    {
        string locString = "";
        foreach (LocalizedText locText in listOfLocalizedTexts)
        {
            if (locText.lang == language)
            {
                locString = locText.text;
                break;
            }
        }

        localizableText.text = locString.Replace("\\n", "\n");
    }

    public static void SetLanguage(Language language)
    {
        Localization.currentLanguage = language;

        foreach (LocalizedItemBehaviour localizedItem in listOfLocalizedItems)
        {
            localizedItem.Localize(language);
        }
    }

    public static void SwitchLanguage()
    {
        if (Localization.currentLanguage == Language.ENGLISH)
        {
            SetLanguage(Language.FRENCH);
        }
        else if (Localization.currentLanguage == Language.FRENCH)
        {
            SetLanguage(Language.ENGLISH);
        }
    }

    public static string TranslateString(string originalTextInEnglish)
    {
        string translation = originalTextInEnglish;
        if (Localization.currentLanguage == Language.FRENCH)
        {
            if (originalTextInEnglish.Equals("BULBAZABAZAUR"))
            {
                translation = "BULBAZABIZARRE";
            }
            else if(originalTextInEnglish.Equals("SQUIRTLITULI"))
            {
                translation = "CARACAPARAPUCE";
            }
            else if(originalTextInEnglish.Equals("CHARMANANDER"))
            {
                translation = "SALASAMALAMECHE";
            }
            else
            {
                Debug.LogWarning("String was not translated: " + originalTextInEnglish);
            }
        }
        return translation;
    }
}

public class Localization
{
    public static Language currentLanguage;

    public static string ConjugateVerb(string originalVerbInEnglish, int subject)
    {
        string translation = originalVerbInEnglish;
        if (originalVerbInEnglish.Equals("be"))
        {
            if (currentLanguage == Language.FRENCH)
            {
                if (subject == 1)
                {
                    translation = "suis";
                }
                else if (subject == 2)
                {
                    translation = "es";
                }
                else if(subject == 3)
                {
                    translation = "est";
                }
            }
            else
            {
                if (subject == 1)
                {
                    translation = "am";
                }
                else if (subject == 2)
                {
                    translation = "are";
                }
                else if (subject == 3)
                {
                    translation = "is";
                }
            }

        }
        else if (originalVerbInEnglish.Equals("defend"))
        {
            if (currentLanguage == Language.FRENCH)
            {
                if (subject == 1)
                {
                    translation = "me défends";
                }
                else if (subject == 2)
                {
                    translation = "te défends";
                }
                else if (subject == 3)
                {
                    translation = "se défend";
                }
            }
            else
            {
                if (subject == 1)
                {
                    translation = "defend";
                }
                else if (subject == 2)
                {
                    translation = "defend";
                }
                else if (subject == 3)
                {
                    translation = "defends";
                }
            }
        }
        else
        {
            Debug.LogWarning("Verb was not translated: " + originalVerbInEnglish);
        }
        return translation;
    }

    public static string PokemonUseSkill(string pkmnName, string skillName, string targetPkmnName)
    {
        string phrase = "";
        if (currentLanguage == Language.FRENCH)
        {
            phrase = pkmnName + " utilise " + Localization.SkillName(skillName) ;
        }
        else
        {
            phrase = pkmnName + " used " + Localization.SkillName(skillName) ;
        }

        if (targetPkmnName == null)
        {
            phrase += "!";
        }
        else
        {
            if (currentLanguage == Language.FRENCH)
            {
                phrase += " sur " + targetPkmnName + "!";
            }
            else
            {
                phrase += " on " + targetPkmnName + "!";
            }
        }

        return phrase;
    }

    public static string SkillName(string skillNameInEnglish)
    {
        string translatedSkill = skillNameInEnglish;
        if (currentLanguage == Language.FRENCH)
        {
            if (skillNameInEnglish.Equals("SCRATCH"))
            {
                translatedSkill = "GRIFFE";
            }
            else if (skillNameInEnglish.Equals("EMBER"))
            {
                translatedSkill = "FLAMMECHE";
            }
            else if (skillNameInEnglish.Equals("DEFENSE CURL"))
            {
                translatedSkill = "BOUL'ARMURE";
            }
            else if (skillNameInEnglish.Equals("CHARGE"))
            {
                translatedSkill = "CHARGE";
            }
            else if (skillNameInEnglish.Equals("BUBBLE"))
            {
                translatedSkill = "ECUME";
            }
            else if (skillNameInEnglish.Equals("HARDEN"))
            {
                translatedSkill = "ARMURE";
            }
            else if (skillNameInEnglish.Equals("HEAD BUMP"))
            {
                translatedSkill = "COUP D'TETE";
            }
            else if (skillNameInEnglish.Equals("VINE WHIP"))
            {
                translatedSkill = "FOUET LIANE";
            }
            else if (skillNameInEnglish.Equals("COTTON GUARD"))
            {
                translatedSkill = "GARDE COTON";
            }
            else
            {
                Debug.LogWarning("Skill not translated in French : " + skillNameInEnglish);
            }
        }
        return translatedSkill;
    }

    public static string GetLocalizedString(List<LocalizedText> localizedTexts)
    {
        string result = "";
        foreach (LocalizedText locText in localizedTexts)
        {
            if (locText.lang == currentLanguage)
            {
                result = locText.text;
                break;
            }
        }
        return result;
    }

    public static string PokemonName(PokemonCode pkmnCode)
    {
        string name ="MISSINGNO";
        switch (pkmnCode)
        {
            case PokemonCode.BULBAZABAZAUR:
                if (currentLanguage == Language.FRENCH)
                {
                    name = "BULBALBIZARRE";
                }
                else
                {
                    name = "BULBAZABAZAUR";
                }
                break;
            case PokemonCode.SQUIRTLITULLI:
                if (currentLanguage == Language.FRENCH)
                {
                    name = "CARACAPAPUCE";
                }
                else
                {
                    name = "SQUIRTLITULLI";
                }
                break;
            case PokemonCode.CHARMANANDER:
                if (currentLanguage == Language.FRENCH)
                {
                    name = "SALASAMAMECHE";
                }
                else
                {
                    name = "CHARMANANDER";
                }
                break;
        }
        return name;
    }
}
