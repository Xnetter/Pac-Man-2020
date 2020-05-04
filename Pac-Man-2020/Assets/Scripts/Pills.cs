using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pills : MonoBehaviour
{
    public static bool isOnePlayerGame = true;

	public static int livesPlayerOne;
	public static int livesPlayerTwo;

	public static int playerOnePelletsConsumed = 0;
	public static int playerTwoPelletsConsumed = 0;

    public bool isPortal;

    public bool isPellet;
    public bool isLargePellet;
    private bool consumed;
    public bool isJailEntrance;
    public bool isBonusItem;

	public int pointValue;

    public GameObject portalReceiver;

    public bool Consumed { get => consumed; set => consumed = value; }
}
