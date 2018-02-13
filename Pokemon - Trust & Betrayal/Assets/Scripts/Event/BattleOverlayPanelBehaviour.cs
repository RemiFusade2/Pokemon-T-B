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
/// This class deals solely with the black overlay that appears before a battle.
/// </summary>
public class BattleOverlayPanelBehaviour : MonoBehaviour
{
    public GameObject blackSquare;

    public void HideBattleOverlay()
    {
        foreach(Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

	public void ShowBattleOverlay(float delay)
    {
        float deltaTime = delay / (8 * 6);
        float currentDelay = 0;
        for (int y = 0; y < 600; y+=100)
        {
            for (int x = 0; x < 800; x += 100)
            {
                Vector2 position = new Vector2(x, y);
                StartCoroutine(WaitAndAddBlackSquare(currentDelay, position));
                currentDelay += deltaTime;
            }
        }
    }

    private IEnumerator WaitAndAddBlackSquare(float delay, Vector2 position)
    {
        yield return new WaitForSeconds(delay);
        GameObject newBlackSquare = Instantiate(blackSquare, this.transform);
        newBlackSquare.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x, 100);
        newBlackSquare.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, position.y, 100);
    }
}
