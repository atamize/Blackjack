using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BetUI : MonoBehaviour
{
    public static readonly int BET_INCREMENT = 5;

    public TextMeshProUGUI prompt;
    public TextMeshProUGUI betText;

    public int BetAmount { get; private set; }

    private void Start()
    {
        BetAmount = BET_INCREMENT;
        UpdateBet();
    }

    public void IncreaseBet()
    {
        BetAmount += BET_INCREMENT;
        UpdateBet();
    }

    public void DecreaseBet()
    {
        BetAmount = Mathf.Max(0, BetAmount - BET_INCREMENT);
        UpdateBet();
    }

    void UpdateBet()
    {
        betText.text = string.Format("${0}", BetAmount);
    }
}
