/* 
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * AUTHOR: Rémi Fusade
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PokemonSprites
{
    public PokemonCode code;

    public Sprite bodyFrontSprite;
    public Sprite bodyBackSprite;

    public Sprite faceNoneSprite;
    public Sprite faceHappySprite;
    public Sprite faceSadSprite;
    public Sprite faceWeirdSprite;
    public Sprite faceAngrySprite;
    public Sprite faceWoundedSprite;
    public Sprite faceScaredSprite;

    public Sprite subjectPinsSprite;
}

public enum DialogueSimplePhrasePosition
{
    MAIN,
    PROPOSITION,
    CONDITION,
    CONDITION_PROPOSITION
}

public enum DialoguePinsTypeCode
{
    SUBJECT,
    VERB,
    OBJECT,
    TRAIT,
    QUANTIFIER,
    PREPOSITION
}

public enum DialogueSubjectCode
{
    PLAYER,
    ENEMY,
    PLAYER_PKMN,
    ENEMY_PKMN
}

[System.Serializable]
public class DialogueSubject
{
    public DialogueSubjectCode code;

    public string Name(bool isSubject)
    {
        string result = GetLocalizedSubjectName(Localization.currentLanguage);
        if (result.Equals("TOI") && isSubject)
        {
            result = "TU";
        }
        return result;
    }

    public List<LocalizedText> localizedSubjectNames;

    private string GetLocalizedSubjectName(Language lang)
    {
        foreach (LocalizedText locText in localizedSubjectNames)
        {
            if (locText.lang == lang)
            {
                return locText.text;
            }
        }
        return "Missing subject name";
    }

    public Sprite sprite;
}

/*
public enum DialogueStateCode
{
    HAPPY,
    SAD,
    HONEST, 
    AFRAID
}*/

public enum DialogueVerbCode
{
    BE,
    GREET,
    THINK,
    DEAL,
    EXPRESS_FEELING,
    ASK,
    PROMISE,
    AGREE,
    DISAGREE,
    ATTACK,
    DEFEND,
    CAPTURE,
    HEAL
}

[System.Serializable]
public class DialogueVerb
{
    public DialogueVerbCode code;
    
    public string Verb()
    {
        return GetLocalizedVerb(Localization.currentLanguage);
    }

    public List<LocalizedText> localizedVerbs;

    private string GetLocalizedVerb(Language lang)
    {
        foreach (LocalizedText locText in localizedVerbs)
        {
            if (locText.lang == lang)
            {
                return locText.text;
            }
        }
        return "Missing verb";
    }

    public Sprite sprite;

    public bool mandatoryTrait;
    public bool optionalTrait;

    public bool mandatoryQuantifier;
    public bool optionalQuantifier;

    public bool mandatoryObject_Character;
    public bool optionalObject_Character;
    public bool mandatoryObject_State;
    public bool optionalObject_State;

    public bool mandatoryProposition_sameSubject;
    public bool optionalProposition_sameSubject;
    public bool mandatoryProposition_differentSubject;
    public bool optionalProposition_differentSubject;

    public bool mandatoryCondition;
    public bool optionalCondition;

    public bool usableInMainPhrase;
    public bool usableInProposition;
    public bool usableInCondition;
}

public enum DialogueTraitCode
{
    LOVE,
    TRUST,
    POWER
}

[System.Serializable]
public class DialogueTrait
{
    public DialogueTraitCode code;

    public string Trait()
    {
        return GetLocalizedTraitName(Localization.currentLanguage);
    }

    public List<LocalizedText> localizedTraitName;

    private string GetLocalizedTraitName(Language lang)
    {
        foreach (LocalizedText locText in localizedTraitName)
        {
            if (locText.lang == lang)
            {
                return locText.text;
            }
        }
        return "Missing trait name";
    }

    public Sprite sprite;
}

public enum DialogueQuantifierCode
{
    NONE,
    FEW,
    SOME,
    MUCH
}

[System.Serializable]
public class DialogueQuantifier
{
    public DialogueQuantifierCode code;
    
    public string Quantifier()
    {
        return GetLocalizedQuantifier(Localization.currentLanguage);
    }

    public List<LocalizedText> localizedQuantifiers;

    private string GetLocalizedQuantifier(Language lang)
    {
        foreach (LocalizedText locText in localizedQuantifiers)
        {
            if (locText.lang == lang)
            {
                return locText.text;
            }
        }
        return "Missing quantifier";
    }

    public Sprite sprite;
}

/// <summary>
/// This class defines a phrase and compute its translation in French or English.
/// This class is recursive: an instance can contain a proposition or a condition (that are others DialoguePhrase objects)
/// A proposition can't have a condition nor another proposition
/// A condition can't have another condition (but it can have a proposition)
/// </summary>
public class DialoguePhrase
{
    public DialogueSubject mainSubject;
    public DialogueVerb mainVerb;

    public DialogueSubject mainObject;
    public DialogueTrait mainVerbTrait;
    public DialogueQuantifier mainVerbTraitQuantifier;

    public DialoguePhrase proposition;
    public DialoguePhrase condition;

    public static string VerbConjugationEN(string verb, DialogueSubject subject, bool e)
    {
        if (verb.Equals("be"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "are";
            }
            else
            {
                return "is";
            }
        }
        else
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return verb;
            }
            else
            {
                return verb + (e ? "es" : "s");
            }
        }
    }

    public static string VerbConjugationFR(string verb, DialogueSubject subject)
    {
        if (verb.Equals("etre"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "es";
            }
            else
            {
                return "est";
            }
        }
        else if (verb.Equals("se méfier"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "te méfies";
            }
            else
            {
                return "se méfie";
            }
        }
        else if (verb.Equals("faire"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "fais";
            }
            else
            {
                return "fait";
            }
        }
        else if (verb.Equals("se fier"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "te fies";
            }
            else
            {
                return "se fie";
            }
        }
        else if (verb.Equals("se défendre"))
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return "te défends";
            }
            else
            {
                return "se défend";
            }
        }        
        else
        {
            if (subject != null && subject.code == DialogueSubjectCode.PLAYER)
            {
                return verb + "s";
            }
            else
            {
                return verb;
            }
        }
    }

    public string ToStringEN()
    {
        string translation = "";

        if (mainSubject != null)
        {
            translation += mainSubject.Name(true) + " ";
        }
        else
        {
            translation += "[WHO] ";
        }

        if (mainVerb != null)
        {
            if (mainVerb.code == DialogueVerbCode.BE)
            {
                if (mainVerbTrait == null)
                {
                    translation += VerbConjugationEN("be", mainSubject, false) + " ";
                }
                else
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("be", mainSubject, true) + " powerful";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " insignificant";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " weak";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " strong";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " almighty";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.LOVE:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("be", mainSubject, true) + " kind";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " bad";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " mean";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " nice";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " adorable";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.TRUST:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("be", mainSubject, true) + " honest";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " a liar";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " not always honest";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " mostly honest";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " to be trusted";
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (mainVerb.code == DialogueVerbCode.EXPRESS_FEELING)
            {
                if (mainVerbTrait == null)
                {
                    translation += VerbConjugationEN("express", mainSubject, true) + " feeling about ";
                }
                else
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("express", mainSubject, true) + " fear about ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("despise", mainSubject, false) + " ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " not afraid of ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " wary of ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("be", mainSubject, false) + " terrified by ";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.LOVE:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("express", mainSubject, true) + " love about ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("hate", mainSubject, false) + " ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("do", mainSubject, true) + "n't like ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationEN("like", mainSubject, false) + " ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("love", mainSubject, false) + " ";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.TRUST:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationEN("express", mainSubject, true) + " trust about ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationEN("mistrust", mainSubject, false) + " ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationEN("do", mainSubject, true) + "n't rely on ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += "somehow " + VerbConjugationEN("trust", mainSubject, false) + " ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationEN("believe", mainSubject, false) + " in ";
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (mainVerb.code == DialogueVerbCode.DEAL)
            {
                translation += VerbConjugationEN("offer", mainSubject, false) + " deal to ";
            }
            else if (mainVerb.code == DialogueVerbCode.ASK)
            {
                if (mainVerbTrait != null)
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            translation += VerbConjugationEN("order", mainSubject, false) + " ";
                            break;
                        case DialogueTraitCode.LOVE:
                            translation += VerbConjugationEN("kindly ask", mainSubject, false) + " ";
                            break;
                        case DialogueTraitCode.TRUST:
                            translation += VerbConjugationEN("politely request", mainSubject, false) + " ";
                            break;
                    }
                }
                else
                {
                    translation += VerbConjugationEN("ask", mainSubject, false) + " ";
                }
            }
            else if (mainVerb.code == DialogueVerbCode.ATTACK)
            {
                if (mainVerbTraitQuantifier != null)
                {
                    switch (mainVerbTraitQuantifier.code)
                    {
                        case DialogueQuantifierCode.NONE:
                            translation += VerbConjugationEN("do", mainSubject, true) + "n't attack ";
                            break;
                        case DialogueQuantifierCode.FEW:
                            translation += "weakly " + VerbConjugationEN("attack", mainSubject, false) + " ";
                            break;
                        case DialogueQuantifierCode.SOME:
                            translation += VerbConjugationEN("attack", mainSubject, false) + " ";
                            break;
                        case DialogueQuantifierCode.MUCH:
                            translation += "strongly " + VerbConjugationEN("attack", mainSubject, false) + " ";
                            break;
                    }
                }
                else
                {
                    translation += VerbConjugationEN("attack", mainSubject, false) + " ";
                }
            }
            else
            {
                translation += VerbConjugationEN(mainVerb.Verb(), mainSubject, false) + " ";
            }
        }
        else
        {
            if (mainSubject != null && mainSubject.code == DialogueSubjectCode.PLAYER)
            {
                translation += "[DO WHAT] ";
            }
            else
            {
                translation += "[DOES WHAT] ";
            }
        }

        if (mainObject != null)
        {
            translation += mainObject.Name(false) + " ";
        }
        else
        {
            if (mainVerb != null && mainVerb.mandatoryObject_Character)
            {
                translation += "[TO WHOM] ";
            }
            else if (mainVerb != null && mainVerb.mandatoryObject_State)
            {
                translation += "[WHAT] ";
            }
        }

        if (mainVerb != null && mainVerb.code != DialogueVerbCode.EXPRESS_FEELING && mainVerb.code != DialogueVerbCode.ASK && mainVerb.code != DialogueVerbCode.BE && mainVerb.code != DialogueVerbCode.ATTACK)
        {
            if (mainVerbTraitQuantifier != null)
            {
                translation += "with " + mainVerbTraitQuantifier.Quantifier() + " ";
                if (mainVerbTrait != null)
                {
                    translation += "of " + mainVerbTrait.Trait() + " ";
                }
            }
            else
            {
                if (mainVerbTrait != null)
                {
                    translation += "with " + mainVerbTrait.Trait() + " ";
                }
            }
        }

        if (proposition != null)
        {
            translation += "that " + proposition.ToString() + "";
        }

        if (condition != null)
        {
            translation += "if " + condition.ToString() + " ";
        }

        return translation;
    }

    public string ToStringFR()
    {
        string translation = "";

        if (mainSubject != null)
        {
            translation += mainSubject.Name(true) + " ";
        }
        else
        {
            translation += "[QUI] ";
        }

        if (mainVerb != null)
        {
            if (mainVerb.code == DialogueVerbCode.BE)
            {
                if (mainVerbTrait == null)
                {
                    translation += VerbConjugationFR("etre", mainSubject) + " ";
                }
                else
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("etre", mainSubject) + " fort";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("etre", mainSubject) + " insignifiant";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationFR("etre", mainSubject) + " faible";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("etre", mainSubject) + " fort";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("etre", mainSubject) + " tout-puissant";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.LOVE:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("etre", mainSubject) + " gentil";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("etre", mainSubject) + " cruel";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationFR("etre", mainSubject) + " méchant";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("etre", mainSubject) + " gentil";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("etre", mainSubject) + " adorable";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.TRUST:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("etre", mainSubject) + " honnête";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("etre", mainSubject) + " un menteur";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationFR("etre", mainSubject) + " pas toujours honnête";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("etre", mainSubject) + " souvent honnête";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("etre", mainSubject) + " digne de confiance";
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (mainVerb.code == DialogueVerbCode.EXPRESS_FEELING)
            {
                if (mainVerbTrait == null)
                {
                    translation += VerbConjugationFR("exprime", mainSubject) + " un sentiment au sujet de ";
                }
                else
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("exprime", mainSubject) + " de la peur au sujet de ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("méprise", mainSubject) + " ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += "n'" + VerbConjugationFR("a", mainSubject) + " pas peur de ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("etre", mainSubject) + " inquiété par ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("etre", mainSubject) + " terrifié par ";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.LOVE:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("exprime", mainSubject) + " de l'amour pour ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("déteste", mainSubject) + " ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += "n'" + VerbConjugationFR("aime", mainSubject) + " pas ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("aime", mainSubject) + " bien ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("adore", mainSubject) + " ";
                                        break;
                                }
                            }
                            break;
                        case DialogueTraitCode.TRUST:
                            if (mainVerbTraitQuantifier == null)
                            {
                                translation += VerbConjugationFR("exprime", mainSubject) + " de la confiance envers ";
                            }
                            else
                            {
                                switch (mainVerbTraitQuantifier.code)
                                {
                                    case DialogueQuantifierCode.NONE:
                                        translation += VerbConjugationFR("se méfier", mainSubject) + " de ";
                                        break;
                                    case DialogueQuantifierCode.FEW:
                                        translation += VerbConjugationFR("doute", mainSubject) + " de ";
                                        break;
                                    case DialogueQuantifierCode.SOME:
                                        translation += VerbConjugationFR("faire", mainSubject) + " confiance à ";
                                        break;
                                    case DialogueQuantifierCode.MUCH:
                                        translation += VerbConjugationFR("se fier", mainSubject) + " aveuglément à ";
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (mainVerb.code == DialogueVerbCode.DEAL)
            {
                translation += VerbConjugationFR("faire", mainSubject) + " une proposition à ";
            }
            else if (mainVerb.code == DialogueVerbCode.ASK)
            {
                if (mainVerbTrait != null)
                {
                    switch (mainVerbTrait.code)
                    {
                        case DialogueTraitCode.POWER:
                            translation += VerbConjugationFR("ordonne", mainSubject) + " à ";
                            break;
                        case DialogueTraitCode.LOVE:
                            translation += VerbConjugationFR("demande", mainSubject) + " gentiment à ";
                            break;
                        case DialogueTraitCode.TRUST:
                            translation += VerbConjugationFR("demande", mainSubject) + " poliment à ";
                            break;
                    }
                }
                else
                {
                    translation += VerbConjugationFR("demande", mainSubject) + " à ";
                }
            }
            else if (mainVerb.code == DialogueVerbCode.ATTACK)
            {
                if (mainVerbTraitQuantifier != null)
                {
                    switch (mainVerbTraitQuantifier.code)
                    {
                        case DialogueQuantifierCode.NONE:
                            translation += VerbConjugationFR("n'attaque", mainSubject) + " pas ";
                            break;
                        case DialogueQuantifierCode.FEW:
                            translation += VerbConjugationFR("donne", mainSubject) + " une pichenette à ";
                            break;
                        case DialogueQuantifierCode.SOME:
                            translation += VerbConjugationFR("attaque", mainSubject) + " ";
                            break;
                        case DialogueQuantifierCode.MUCH:
                            translation += VerbConjugationFR("massacre", mainSubject) + " ";
                            break;
                    }
                }
                else
                {
                    translation += VerbConjugationFR("attaque", mainSubject) + " ";
                }
            }
            else if (mainVerb.code == DialogueVerbCode.DEFEND)
            {
                {
                    translation += VerbConjugationFR("se défendre", mainSubject) + " ";
                }
            }
            else if (mainVerb.code == DialogueVerbCode.GREET)
            {
                {
                    translation += VerbConjugationFR("acceuille", mainSubject) + " ";
                }
            }
            else if (mainVerb.code == DialogueVerbCode.PROMISE)
            {
                {
                    translation += VerbConjugationFR("promet", mainSubject) + " à ";
                }
            }
            else
            {
                translation += VerbConjugationFR(mainVerb.Verb(), mainSubject) + " ";
            }
        }
        else
        {
            if (mainSubject != null && mainSubject.code == DialogueSubjectCode.PLAYER)
            {
                translation += "[FAIS QUOI] ";
            }
            else
            {
                translation += "[FAIT QUOI] ";
            }
        }

        if (mainObject != null)
        {
            translation += mainObject.Name(false) + " ";
        }
        else
        {
            if (mainVerb != null && mainVerb.mandatoryObject_Character)
            {
                translation += "[QUI] ";
            }
            else if (mainVerb != null && mainVerb.mandatoryObject_State)
            {
                translation += "[QUOI] ";
            }
        }

        if (mainVerb != null && mainVerb.code != DialogueVerbCode.EXPRESS_FEELING && mainVerb.code != DialogueVerbCode.ASK && mainVerb.code != DialogueVerbCode.BE && mainVerb.code != DialogueVerbCode.ATTACK)
        {
            if (mainVerbTraitQuantifier != null)
            {
                translation += "avec " + mainVerbTraitQuantifier.Quantifier() + " ";
                if (mainVerbTrait != null)
                {
                    translation += "de " + mainVerbTrait.Trait() + " ";
                }
            }
            else
            {
                if (mainVerbTrait != null)
                {
                    translation += "avec " + mainVerbTrait.Trait() + " ";
                }
            }
        }

        if (proposition != null)
        {
            translation += "que " + proposition.ToString() + "";
        }

        if (condition != null)
        {
            translation += "si " + condition.ToString() + " ";
        }

        return translation;
    }

    public override string ToString()
    {
        if (Localization.currentLanguage == Language.FRENCH)
        {
            return this.ToStringFR();
        }
        else
        {
            return this.ToStringEN();
        }
    }

    public bool IsComplete()
    {
        bool phraseIsComplete = false;
        if (this.mainSubject != null && this.mainVerb != null)
        {
            if (this.mainObject != null || (!this.mainVerb.mandatoryObject_Character && !this.mainVerb.mandatoryObject_State))
            {
                if (this.mainVerbTrait != null || !this.mainVerb.mandatoryTrait)
                {
                    if (this.mainVerbTraitQuantifier != null || !this.mainVerb.mandatoryQuantifier)
                    {
                        bool otherPhrasesAreComplete = true;

                        if (this.proposition != null)
                        {
                            otherPhrasesAreComplete &= this.proposition.IsComplete();
                        }
                        else
                        {
                            if (this.mainVerb.mandatoryProposition_differentSubject || this.mainVerb.mandatoryProposition_sameSubject)
                            {
                                return false;
                            }
                        }

                        if (this.condition != null)
                        {
                            otherPhrasesAreComplete &= this.condition.IsComplete();
                        }
                        else
                        {
                            if (this.mainVerb.mandatoryCondition)
                            {
                                return false;
                            }
                        }

                        phraseIsComplete = otherPhrasesAreComplete;
                    }
                }
            }
        }
        return phraseIsComplete;
    }
}

/// <summary>
/// The Dialogue Engine contains all possible subjects, verbs, objects, traits and quantifiers that can be used in a DialoguePhrase.
/// It also contains all possible Sprites of Pokémons.
/// This class offers several methods to create working DialoguePhrase instances (handy).
/// </summary>
public class DialogueEngineBehaviour : MonoBehaviour
{
    public static DialogueEngineBehaviour instance;

    [Header("Protagonists")]
    public DialogueSubject playerSubject;
    public DialogueSubject playerPokemonSubject;
    public bool isThereEnemy;
    public DialogueSubject enemySubject; // optional
    public DialogueSubject enemyPokemonSubject;

    [Header("Vocabulary (sprites)")]
    public List<DialogueVerb> possibleVerbs;
    public List<DialogueTrait> possibleTraits;
    public List<DialogueQuantifier> possibleQuantifiers;

    private Dictionary<DialogueSubjectCode, DialogueSubject> subjectDictionary;
    private Dictionary<DialogueVerbCode, DialogueVerb> verbsDictionary;
    private Dictionary<DialogueTraitCode, DialogueTrait> traitsDictionary;
    private Dictionary<DialogueQuantifierCode, DialogueQuantifier> quantifiersDictionary;
    
    [Header("Pokemons (sprites)")]
    public List<PokemonSprites> possiblePokemons;
    private Dictionary<PokemonCode, PokemonSprites> pokemonDictionary;

    [Header("Dialogue Panels")]
    public DialoguePanelBehaviour enemyPkmnDialoguePanel;
    public DialoguePanelBehaviour playerPkmnDialoguePanel;
    public DialoguePanelBehaviour playerDialoguePanel;

    void Start()
    {
        instance = this;

        subjectDictionary = new Dictionary<DialogueSubjectCode, DialogueSubject>();
        subjectDictionary.Add(DialogueSubjectCode.PLAYER, playerSubject);
        if (enemySubject != null)
        {
            subjectDictionary.Add(DialogueSubjectCode.ENEMY, enemySubject);
        }
        subjectDictionary.Add(DialogueSubjectCode.PLAYER_PKMN, playerPokemonSubject);
        subjectDictionary.Add(DialogueSubjectCode.ENEMY_PKMN, enemyPokemonSubject);

        verbsDictionary = new Dictionary<DialogueVerbCode, DialogueVerb>();
        foreach (DialogueVerb verb in possibleVerbs)
        {
            verbsDictionary.Add(verb.code, verb);
        }

        traitsDictionary = new Dictionary<DialogueTraitCode, DialogueTrait>();
        foreach (DialogueTrait trait in possibleTraits)
        {
            traitsDictionary.Add(trait.code, trait);
        }

        quantifiersDictionary = new Dictionary<DialogueQuantifierCode, DialogueQuantifier>();
        foreach (DialogueQuantifier quantifier in possibleQuantifiers)
        {
            quantifiersDictionary.Add(quantifier.code, quantifier);
        }

        pokemonDictionary = new Dictionary<PokemonCode, PokemonSprites>();
        foreach (PokemonSprites pokemon in possiblePokemons)
        {
            pokemonDictionary.Add(pokemon.code, pokemon);
        }
    }

    public void SetPokemon(DialogueSubjectCode subjectCode, Pokemon pkmn)
    {
        if (subjectCode == DialogueSubjectCode.PLAYER_PKMN)
        {
            playerPokemonSubject.code = subjectCode;
            playerPokemonSubject.localizedSubjectNames.Add(new LocalizedText() { lang = Localization.currentLanguage, text = Localization.PokemonName(pkmn.code) });
            playerPokemonSubject.sprite = GetPokemonSpriteFromCode(pkmn.code).subjectPinsSprite;
        }
        else
        {
            enemyPokemonSubject.code = subjectCode;
            enemyPokemonSubject.localizedSubjectNames.Add(new LocalizedText() { lang = Localization.currentLanguage, text = Localization.PokemonName(pkmn.code) });
            enemyPokemonSubject.sprite = GetPokemonSpriteFromCode(pkmn.code).subjectPinsSprite;
        }
    }

    public DialogueSubject GetSubjectFromCode(DialogueSubjectCode code)
    {
        switch (code)
        {
            case DialogueSubjectCode.PLAYER:
                return playerSubject;
            case DialogueSubjectCode.ENEMY:
                return enemySubject;
            case DialogueSubjectCode.PLAYER_PKMN:
                return playerPokemonSubject;
            case DialogueSubjectCode.ENEMY_PKMN:
                return enemyPokemonSubject;
        }
        return null;
    }

    public DialogueVerb GetVerbFromCode(DialogueVerbCode code)
    {
        return verbsDictionary[code];
    }

    public DialogueTrait GetTraitFromCode(DialogueTraitCode code)
    {
        return traitsDictionary[code];
    }

    public DialogueQuantifier GetQuantifierFromCode(DialogueQuantifierCode code)
    {
        return quantifiersDictionary[code];
    }

    public PokemonSprites GetPokemonSpriteFromCode(PokemonCode code)
    {
        return pokemonDictionary[code];
    }

    #region Random Trait / Quantifier

    public DialogueTraitCode GetRandomTrait(float weightOnLove, float weightOnTrust, float weightOnFear)
    {
        float sumOfWeights = weightOnLove + weightOnTrust + weightOnFear;
        float rand = Random.Range(0, sumOfWeights);
        if (rand < weightOnLove)
        {
            return DialogueTraitCode.LOVE;
        }
        else if (rand <= (weightOnLove + weightOnTrust))
        {
            return DialogueTraitCode.TRUST;
        }
        else
        {
            return DialogueTraitCode.POWER;
        }
    }

    public DialogueQuantifierCode GetRandomQuantifier(DialogueTraitCode trait, bool positive)
    {
        int rand = Random.Range(0, 100);
        if (rand < 70)
        {
            return positive ? DialogueQuantifierCode.MUCH : DialogueQuantifierCode.NONE;
        }
        else
        {
            return positive ? DialogueQuantifierCode.SOME : DialogueQuantifierCode.FEW;
        }
    }

    #endregion

    #region Create phrases

    public DialoguePhrase GetPhrase_ResponseToAsk(DialogueSubjectCode subject, DialogueSubjectCode target, DialoguePhrase proposition, DialogueTraitCode trait, Relation relationWithAsker, Relation relationWithObject, out PokemonFacialExpression expression, out bool accept)
    {
        DialoguePhrase responseToAsk = GetPhrase_Promise(subject, target, target, proposition, true);
        
        if (proposition.mainVerb.code == DialogueVerbCode.ATTACK && relationWithObject != null)
        {
            // proposition is to attack someone!
            // either we love or fear the target of the attack (then we won't accept to attack)
            // or otherwise, we can attack
            if (trait == DialogueTraitCode.POWER)
            {
                // someone asked with power, so I'll accept the request only if I fear asker more than I love target
                accept = (relationWithAsker.fear > 0.5f) || relationWithAsker.fear > (relationWithObject.love-0.3f);
                if (accept)
                {
                    // I accept, I fear asker
                    expression = PokemonFacialExpression.SCARED;
                }
                else
                {
                    // I refuse, I love target
                    expression = PokemonFacialExpression.SAD;
                    responseToAsk = GetPhrase_ExpressFeeling(subject, proposition.mainObject.code, DialogueTraitCode.LOVE, true);
                }
            }
            else if (trait == DialogueTraitCode.LOVE)
            {
                // someone asked kindly, so I'll accept the request only if I love asker more than I fear target
                accept = (relationWithAsker.love > 0.5f) || relationWithAsker.love > (relationWithObject.fear - 0.3f);
                if (accept)
                {
                    // I accept, I love asker
                    expression = PokemonFacialExpression.HAPPY;
                }
                else
                {
                    // I refuse, I'm scared of target
                    expression = PokemonFacialExpression.SCARED;
                    responseToAsk = GetPhrase_ExpressFeeling(subject, proposition.mainObject.code, DialogueTraitCode.POWER, true);
                }
            }
            else
            {
                // someone trust me in this, so I'll accept the request if I trust asker or if I don't trust target
                accept = (relationWithAsker.trust > 0) || (relationWithObject.trust < -0.2f);
                if (accept)
                {
                    // I accept, I trust asker
                    expression = PokemonFacialExpression.HAPPY;
                }
                else
                {
                    // I refuse, I don't trust asker
                    expression = PokemonFacialExpression.ANGRY;
                    responseToAsk = GetPhrase_ExpressFeeling(subject, target, DialogueTraitCode.TRUST, false);
                }
            }

            if (accept)
            {
                // To accept a request, we can make a promise !
                // I make a promise to you about you (don't care, proposition is already made)
                responseToAsk = GetPhrase_Promise(subject, target, target, proposition, true);
            }
        }
        else
        {
            // do we accept request ?
            switch (trait)
            {
                // Do we love/trust/fear asker ?
                case DialogueTraitCode.LOVE:
                    if (relationWithAsker.love > 0)
                    {
                        accept = true;
                        expression = PokemonFacialExpression.HAPPY;
                    }
                    else
                    {
                        accept = false;
                        expression = PokemonFacialExpression.ANGRY;
                    }
                    break;
                case DialogueTraitCode.TRUST:
                    if (relationWithAsker.trust > 0)
                    {
                        accept = true;
                        expression = PokemonFacialExpression.HAPPY;
                    }
                    else
                    {
                        accept = false;
                        expression = PokemonFacialExpression.SAD;
                    }
                    break;
                case DialogueTraitCode.POWER:
                    if (relationWithAsker.fear > 0)
                    {
                        accept = true;
                        expression = PokemonFacialExpression.SCARED;
                    }
                    else
                    {
                        accept = false;
                        expression = PokemonFacialExpression.ANGRY;
                    }
                    break;
                default:
                    accept = false;
                    expression = PokemonFacialExpression.WEIRD;
                    break;
            }
            if (accept)
            {
                // To accept a request, we can make a promise !
                // I make a promise to you about you (don't care, proposition is already made)
                responseToAsk = GetPhrase_Promise(subject, target, target, proposition, true);
            }
            else
            {
                // To refuse a request, we can state that we don't love/trust/fear asker
                responseToAsk = GetPhrase_ExpressFeeling(subject, target, trait, false);
            }
        }

        return responseToAsk;
    }

    public DialoguePhrase GetPhrase_ResponseToDeal(DialogueSubjectCode subject, DialogueSubjectCode target, bool accept, bool positiveDeal, out PokemonFacialExpression expression)
    {
        DialoguePhrase phrase = new DialoguePhrase();

        if (accept)
        {
            // Accept deal
            int rand = Random.Range(0, 10);
            if (rand < 5)
            {
                // Simple accept
                phrase.mainSubject = subjectDictionary[subject];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.AGREE];
                expression = positiveDeal ? PokemonFacialExpression.HAPPY : PokemonFacialExpression.ANGRY;
            }
            else if (rand < 7)
            {
                // I trust you
                phrase.mainSubject = subjectDictionary[subject];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.EXPRESS_FEELING];
                phrase.mainObject = subjectDictionary[target];
                phrase.mainVerbTrait = traitsDictionary[DialogueTraitCode.TRUST];
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.MUCH];
                expression = PokemonFacialExpression.HAPPY;
            }
            else
            {
                // you are someone to be trusted
                phrase.mainSubject = subjectDictionary[target];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.BE];
                phrase.mainVerbTrait = traitsDictionary[DialogueTraitCode.TRUST];
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.MUCH];
                expression = PokemonFacialExpression.HAPPY;
            }
        }
        else
        {
            // Refuse deal
            int rand = Random.Range(0, 10);
            if (rand < 5)
            {
                // Simple refuse
                phrase.mainSubject = subjectDictionary[subject];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.DISAGREE];
                expression = positiveDeal ? PokemonFacialExpression.SAD : PokemonFacialExpression.ANGRY;
            }
            else if (rand < 7)
            {
                // I don't trust you
                phrase.mainSubject = subjectDictionary[subject];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.EXPRESS_FEELING];
                phrase.mainObject = subjectDictionary[target];
                phrase.mainVerbTrait = traitsDictionary[DialogueTraitCode.TRUST];
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.NONE];
                expression = PokemonFacialExpression.ANGRY;
            }
            else
            {
                // you are not honest
                phrase.mainSubject = subjectDictionary[target];
                phrase.mainVerb = verbsDictionary[DialogueVerbCode.BE];
                phrase.mainVerbTrait = traitsDictionary[DialogueTraitCode.TRUST];
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.NONE];
                expression = PokemonFacialExpression.ANGRY;
            }
        }

        return phrase;
    }

    public DialoguePhrase GetPhrase_OfferDeal(DialogueSubjectCode subject, DialogueSubjectCode offerDealTo, DialogueSubjectCode offerDealIf, DialogueSubjectCode offerDealAbout, DialoguePhrase proposition, DialoguePhrase condition, out PokemonFacialExpression expression)
    {
        DialoguePhrase phrase = new DialoguePhrase();

        // I offer deal to you
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.DEAL];
        phrase.mainObject = subjectDictionary[offerDealTo];

        expression = PokemonFacialExpression.HAPPY;
        if (proposition != null && condition != null)
        {
            phrase.proposition = proposition;
            phrase.condition = condition;
        }
        else
        {
            // random deal
            int rand = Random.Range(0, 100);
            bool positiveDeal = rand < 50 ? true : false;
            if (positiveDeal)
            {
                // positive proposition if positive condition
                expression = PokemonFacialExpression.HAPPY;
            }
            else
            {
                // negative proposition if negative condition
                expression = PokemonFacialExpression.ANGRY;
            }

            // proposition
            rand = Random.Range(0, 100);
            if (rand < 30)
            {
                // Deal, if you..., I express feeling for that person
                DialogueTraitCode traitCode = GetRandomTrait(1,1,1);
                DialogueQuantifierCode quantifierCode = GetRandomQuantifier(traitCode, positiveDeal);
                phrase.proposition = GetPhrase_ExpressFeeling(subject, offerDealAbout, traitCode, quantifierCode);
            }
            else if (!positiveDeal)
            {
                // Deal, if you..., I attack that person
                phrase.proposition = GetPhrase_Attack(subject, offerDealAbout, false);
            }
            else
            {
                if (rand < 70)
                {
                    // I defend
                    phrase.proposition = GetPhrase_Defend(subject);
                }
                else
                {
                    // Deal, if you..., I attack that person
                    phrase.proposition = GetPhrase_Attack(subject, offerDealAbout, true);
                }
            }

            // condition
            rand = Random.Range(0, 100);
            DialoguePhrase conditionContent;
            if (rand < 30)
            {
                // Deal, if you express feeling for that person, then I...
                DialogueTraitCode traitCode = GetRandomTrait(1, 1, 1);
                DialogueQuantifierCode quantifierCode = GetRandomQuantifier(traitCode, positiveDeal);
                conditionContent = GetPhrase_ExpressFeeling(offerDealTo, offerDealIf, traitCode, quantifierCode);
            }
            else if (!positiveDeal)
            {
                // Deal, if you attack that person, then I...
                if (offerDealTo == DialogueSubjectCode.PLAYER)
                {
                    conditionContent = GetPhrase_Attack(offerDealTo, DialogueSubjectCode.ENEMY_PKMN, false);
                }
                else
                {
                    conditionContent = GetPhrase_Attack(offerDealTo, offerDealIf, false);
                }
            }
            else
            {
                if (rand < 70)
                {
                    // if that person defend
                    conditionContent = GetPhrase_Defend(offerDealIf);
                }
                else
                {
                    // Deal, if you attack that person, then I...
                    if (offerDealTo == DialogueSubjectCode.PLAYER)
                    {
                        conditionContent = GetPhrase_Attack(offerDealTo, DialogueSubjectCode.ENEMY_PKMN, true);
                    }
                    else
                    {
                        conditionContent = GetPhrase_Attack(offerDealTo, offerDealIf, true);
                    }
                }
            }
            rand = Random.Range(0, 100);
            if (rand < 20)
            {
                // Deal, if you promise me something about that person
                phrase.condition = GetPhrase_Promise(offerDealTo, subject, offerDealIf, conditionContent, false);
            }
            else
            {
                phrase.condition = conditionContent;
            }
        }
        
        return phrase;
    }

    public DialoguePhrase GetPhrase_Attack(DialogueSubjectCode subject, DialogueSubjectCode target, bool niceAttack)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.ATTACK];
        phrase.mainObject = subjectDictionary[target];
        int rand = Random.Range(0, 100);
        if (niceAttack)
        {
            if (rand < 50)
            {
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.NONE];
            }
            else
            {
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.FEW];
            }
        }
        else
        {
            if (rand < 50)
            {

            }
            else if (rand < 90)
            {
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.MUCH];
            }
            else
            {
                phrase.mainVerbTraitQuantifier = quantifiersDictionary[DialogueQuantifierCode.SOME];
            }
        }
        return phrase;
    }
    public DialoguePhrase GetPhrase_Heal(DialogueSubjectCode subject, DialogueSubjectCode target)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.HEAL];
        phrase.mainObject = subjectDictionary[target];
        return phrase;
    }
    public DialoguePhrase GetPhrase_Defend(DialogueSubjectCode subject)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.DEFEND];
        return phrase;
    }

    public DialoguePhrase GetPhrase_Think(DialogueSubjectCode subject, DialoguePhrase proposition)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.THINK];
        phrase.proposition = proposition;
        return phrase;
    }

    #region Express Feeling

    public DialoguePhrase GetPhrase_ExpressFeeling(DialogueSubjectCode subject, DialogueSubjectCode target, DialogueTraitCode withTrait, DialogueQuantifierCode withQuantifier)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.EXPRESS_FEELING];
        phrase.mainObject = subjectDictionary[target];
        phrase.mainVerbTrait = traitsDictionary[withTrait];
        phrase.mainVerbTraitQuantifier = quantifiersDictionary[withQuantifier];
        return phrase;
    }

    public DialoguePhrase GetPhrase_ExpressFeeling(DialogueSubjectCode subject, DialogueSubjectCode target, DialogueTraitCode withTrait, bool positive)
    {
        int rand = Random.Range(positive ? 2 : 0, positive ? 4 : 2);
        DialogueQuantifierCode withQuantifier = (DialogueQuantifierCode)rand;
        return GetPhrase_ExpressFeeling(subject, target, withTrait, withQuantifier);
    }

    #endregion

    public DialoguePhrase GetPhrase_Greet(DialogueSubjectCode subject, DialogueSubjectCode target, DialogueTraitCode? withTrait)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.GREET];
        phrase.mainObject = subjectDictionary[target];
        if (withTrait != null)
        {
            phrase.mainVerbTrait = traitsDictionary[withTrait.Value];
        }
        return phrase;
    }

    public DialoguePhrase GetPhrase_RandomProposition(DialogueSubjectCode subject, DialogueSubjectCode target, bool niceProposition)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        int rand = Random.Range(0, (niceProposition ? 100 : 65) );
        if (rand < 30)
        {
            DialogueTraitCode traitCode = GetRandomTrait(1, 1, 1);
            phrase = GetPhrase_ExpressFeeling(subject, target, traitCode, GetRandomQuantifier(traitCode, niceProposition));
        }
        else if (rand <= 65)
        {
            phrase = GetPhrase_Attack(subject, target, niceProposition);
        }
        else
        {
            phrase = GetPhrase_Defend(subject);
        }
            return phrase;
    }

    public DialoguePhrase GetPhrase_Ask(DialogueSubjectCode subject, DialogueSubjectCode target, DialogueTraitCode withTrait, DialoguePhrase proposition, bool niceAsk)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.ASK];
        phrase.mainObject = subjectDictionary[target];
        phrase.mainVerbTrait = traitsDictionary[withTrait];
        if (proposition != null)
        {
            phrase.proposition = proposition;
        }
        else
        {
            phrase.proposition = GetPhrase_RandomProposition(target, subject, niceAsk);
        }
        return phrase;
    }

    public DialoguePhrase GetPhrase_Promise(DialogueSubjectCode subject, DialogueSubjectCode promiseTo, DialogueSubjectCode promiseAbout, DialoguePhrase proposition, bool nicePromise)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.PROMISE];
        phrase.mainObject = subjectDictionary[promiseTo];
        if (proposition != null)
        {
            phrase.proposition = proposition;
        }
        else
        {
            phrase.proposition = GetPhrase_RandomProposition(subject, promiseAbout, nicePromise);
        }
        return phrase;
    }

    public DialoguePhrase GetPhrase_Agree(DialogueSubjectCode subject)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.AGREE];
        return phrase;
    }
    public DialoguePhrase GetPhrase_Disagree(DialogueSubjectCode subject)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.DISAGREE];
        return phrase;
    }

    #region BE

    public DialoguePhrase GetPhrase_Be(DialogueSubjectCode subject, DialogueTraitCode trait, DialogueQuantifierCode quantifier)
    {
        DialoguePhrase phrase = new DialoguePhrase();
        phrase.mainSubject = subjectDictionary[subject];
        phrase.mainVerb = verbsDictionary[DialogueVerbCode.BE];
        phrase.mainVerbTrait = traitsDictionary[trait];
        phrase.mainVerbTraitQuantifier = quantifiersDictionary[quantifier];
        return phrase;
    }

    public DialoguePhrase GetPhrase_Be(DialogueSubjectCode subject, DialogueTraitCode trait, float value, float honesty)
    {
        DialoguePhrase phrase;
        DialogueQuantifierCode quantifier;
        if (honesty < -0.8f)
        {
            // lie
            if (value > 0)
            {
                quantifier = (Random.Range(0, 2) == 0) ? DialogueQuantifierCode.FEW : DialogueQuantifierCode.NONE;
            }
            else
            {
                quantifier = (Random.Range(0, 2) == 0) ? DialogueQuantifierCode.SOME : DialogueQuantifierCode.MUCH;
            }
        }
        else
        {
            // tell truth
            quantifier = (value < -0.5f) ? DialogueQuantifierCode.NONE : ((value < 0) ? DialogueQuantifierCode.FEW : ((value <= 0.5f) ? DialogueQuantifierCode.SOME : DialogueQuantifierCode.MUCH ));
        }
        phrase = GetPhrase_Be(subject, trait, quantifier);
        return phrase;
    }

    #endregion
    
    #endregion
}
