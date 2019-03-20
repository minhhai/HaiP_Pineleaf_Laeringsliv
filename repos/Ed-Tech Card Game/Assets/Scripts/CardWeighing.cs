using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the automatic weighting of cards as you draw them
/// </summary>
public class CardWeighing : MonoBehaviour
{



    List<float> originalCardWeights = new List<float>();
    List<float> cardWeights = new List<float>();

    Dictionary<int, float> modCardValues = new Dictionary<int, float>();

    public float cardWeightIncrease = 0.05f; // Weight increase per draw for a card that's drawn



    /// <summary>
    /// Initialize card weight values
    /// </summary>
    /// <param name="cardCount"></param>
    /// <param name="weightIncrease"></param>
    public void InitValues(int cardCount, float weightIncrease) {
        cardWeights.Clear();
        originalCardWeights.Clear();
        modCardValues.Clear();
        for (int i = 0; i < cardCount; i++) {
            originalCardWeights.Add(1f);
            cardWeights.Add(1f);
        }
        cardWeightIncrease = weightIncrease;

    }

    public void SaveValues() {

    }

    public void LoadValues() {

    }


    public void UpdateModCardValues() {


        List<int> keys = new List<int>();

        foreach(int key in modCardValues.Keys) {
            keys.Add(key);



            
        }
        foreach (int key in keys) {
            if (modCardValues[key] < originalCardWeights[key]) {
                modCardValues[key] += cardWeightIncrease;
                if (modCardValues[key] > originalCardWeights[key]) {
                    cardWeights[key] = originalCardWeights[key];
                    modCardValues.Remove(key);

                } else {
                    cardWeights[key] = modCardValues[key];
                }

            } else if (modCardValues[key] > originalCardWeights[key]) {
                modCardValues[key] -= cardWeightIncrease;
                if (modCardValues[key] < originalCardWeights[key]) {
                    cardWeights[key] = originalCardWeights[key];
                    modCardValues.Remove(key);
                } else {
                    cardWeights[key] = modCardValues[key];
                }
            }
            
        }
    }




    public void ResetCardWeight(int index) {
        cardWeights[index] = originalCardWeights[index];
        modCardValues.Remove(index);
    }

    public void ChangeCardWeight(int cardIndex, float change) {
        cardWeights[cardIndex] += change;
        if (modCardValues.ContainsKey(cardIndex)) {
            modCardValues[cardIndex] = cardWeights[cardIndex];
        }else {
            modCardValues.Add(cardIndex, cardWeights[cardIndex]);
        }
        
    }

    public void SetCardWeight(int cardIndex, float newValue) {
        cardWeights[cardIndex] = newValue;
        if (modCardValues.ContainsKey(cardIndex)) {
            modCardValues[cardIndex] = cardWeights[cardIndex];
        } else {
            modCardValues.Add(cardIndex, cardWeights[cardIndex]);
        }
    }

    // Draw with weights, returns card index
    public int DrawCardWeighted() {

        

        // Total the weight
        float totalWeight = 0;
        foreach(float f in cardWeights) {
            if (f > 0) {
                totalWeight += f;
            }
            
        }

        float ranNum = Random.Range(0, totalWeight);
        float totalValue = 0;
        int selectedCardID = 0;


        for (int i = 0; i < cardWeights.Count; i++) {
            totalValue += cardWeights[i];
            if (ranNum < totalValue) {
                selectedCardID = i;
                break;
            }
            
        }
        SetCardWeight(selectedCardID, 0f);

        return selectedCardID;
    }

}
