using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PurchasingPropertiesGameplay : MonoBehaviour {
    
    #region Edgework Utility Classes
    private class Battery
    {
        public int numbatteries;
    }
    private class Serial
    {
        public string serial;
    }
    private class Indicator
    {
        public string label;
        public string on;
    }
    private class Port
    {
        public string[] PresentPorts;
    }
    #endregion


    public static readonly int NUM_CARDS = 12; // For determining which card is shown
    private int shownCard;

    private static int numInstances = 0; // For assigning unique debug ID's to each instance of the module
    private int moduleInstanceID;

    public KMSelectable[] Arrows = new KMSelectable[2]; // Left/Right arrows
    public KMSelectable SubmitButton;
    public KMBombModule TheModule; // Refers to the specific instance of PurchasingProperties, handles strikes/solves
    public KMBombInfo TheBomb; // Refers to the entire bomb, handles querying various properties of the bomb
    public TextMesh CardDisplay;

    private int correctProperty;


    #region Edgework variables
    private int numPortPlates;
    private int numPorts;
    private bool hasEmptyPlate;

    private bool hasDVI;
    private bool hasParallel;
    private bool hasPS2;
    private bool hasRJ45;
    private bool hasSerialPort;
    private bool hasStereoRCA;

    private string serialNumber;

    private int numBatteries;
    private int numBatteryHolders;

    private int numIndicators;
    private int numLitIndicators = 0;

    private bool hasSND;
	private bool hasCLR;
	private bool hasCAR;
	private bool hasIND;
	private bool hasFRQ;
	private bool hasSIG;
	private bool hasNSA;
	private bool hasMSA;
	private bool hasTRN;
	private bool hasBOB;
    private bool hasFRK;

    private bool hasLitSND;
    private bool hasLitCLR;
    private bool hasLitCAR;
    private bool hasLitIND;
    private bool hasLitFRQ;
    private bool hasLitSIG;
    private bool hasLitNSA;
    private bool hasLitMSA;
    private bool hasLitTRN;
    private bool hasLitBOB;
    private bool hasLitFRK;
    #endregion



    // Runs on per module on module creation
    void Start () {
        moduleInstanceID = numInstances++; // Assign each instance a unique ID for deebugging, increment the num total instances
        Arrows[0].OnInteract += () => { CycleCardDisplay(1); return false; } ; // Add the methods to call when the buttons are interacted with to the stack of things to call
        Arrows[1].OnInteract += () => { CycleCardDisplay(-1); return false; } ;
        SubmitButton.OnInteract += Submit;
        TheModule.OnActivate += GrabEdgeWork;
    }



    private void GrabEdgeWork()
    {
        // JSON conversions from Flamanis

        // ----- Port -----
        List<string> portList = TheBomb.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string port in portList)
        {
            Port plate = JsonConvert.DeserializeObject<Port>(port);
            foreach (string elem in plate.PresentPorts)
            {
                #region hasPorts
                if (elem == "DVI") hasDVI = true;
                if (elem == "Parallel") hasParallel = true;
                if (elem == "PS2") hasPS2 = true;
                if (elem == "RJ45") hasRJ45 = true;
                if (elem == "Serial") hasSerialPort = true;
                if (elem == "StereoRCA") hasStereoRCA = true;
                #endregion

                if (elem == "") hasEmptyPlate = true;
            }
            numPorts += plate.PresentPorts.Length;
        }
        numPortPlates = portList.Count;
        
        // ----- Serial Number -----
        Serial serialNum = JsonConvert.DeserializeObject<Serial>(TheBomb.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null)[0]);
        serialNumber = serialNum.serial.ToLower();

        // ----- Batteries -----
        List<string> batteryList = TheBomb.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        foreach (string battery in batteryList)
        {
            Battery bat = JsonConvert.DeserializeObject<Battery>(battery);
            numBatteries += bat.numbatteries;
        }
        numBatteryHolders = batteryList.Count;

        // ----- Indicators -----
        List<string> indicatorList = TheBomb.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        foreach (string indicator in indicatorList)
        {
            Indicator ind = JsonConvert.DeserializeObject<Indicator>(indicator);
            #region hasIndicators
            if (ind.on == "True" && ind.label == "SND") hasLitSND = true;
            if (ind.on == "True" && ind.label == "CLR") hasLitCLR = true;
            if (ind.on == "True" && ind.label == "CAR") hasLitCAR = true;
            if (ind.on == "True" && ind.label == "IND") hasLitIND = true;
            if (ind.on == "True" && ind.label == "FRQ") hasLitFRQ = true;
            if (ind.on == "True" && ind.label == "SIG") hasLitSIG = true;
            if (ind.on == "True" && ind.label == "NSA") hasLitNSA = true;
            if (ind.on == "True" && ind.label == "MSA") hasLitMSA = true;
            if (ind.on == "True" && ind.label == "TRN") hasLitTRN = true;
            if (ind.on == "True" && ind.label == "BOB") hasLitBOB = true;
            if (ind.on == "True" && ind.label == "FRK") hasLitFRK = true;

            if (ind.label == "SND") hasSND = true;
            if (ind.label == "CLR") hasCLR = true;
            if (ind.label == "CAR") hasCAR = true;
            if (ind.label == "IND") hasIND = true;
            if (ind.label == "FRQ") hasFRQ = true;
            if (ind.label == "SIG") hasSIG = true;
            if (ind.label == "NSA") hasNSA = true;
            if (ind.label == "MSA") hasMSA = true;
            if (ind.label == "TRN") hasTRN = true;
            if (ind.label == "BOB") hasBOB = true;
            if (ind.label == "FRK") hasFRK = true;
            #endregion

            if (ind.on == "True") numLitIndicators++;
        }
        numIndicators = indicatorList.Count;

        // Implement all the logic to determine from the edgwork, which property needs to be purchased
        CalculatePropToPurchase();
    }



    private bool CalculatePropToPurchase()
    {
        if (hasParallel) correctProperty = 0;
        else if (hasBOB && !hasLitBOB) correctProperty = 1;
        else if (numBatteries > 3) correctProperty = 2;
        else if (numBatteries == 1) correctProperty = 3;
        else correctProperty = 4;

        return false;
    }



    private bool CycleCardDisplay(int dir)
    {
        shownCard = (shownCard + dir) % NUM_CARDS; // Change the card index
        if (shownCard < 0) shownCard += NUM_CARDS;
        Debug.Log("Module " + moduleInstanceID + " cycled the display in the " + dir + " direction, and now card " + shownCard + " is showing.");

        CardDisplay.text = shownCard.ToString(); // Change the actual text on the bomb

        return false;
    }



    private bool Submit()
    {
        if (shownCard == correctProperty)
        {
            TheModule.HandlePass();
            Debug.Log("Module " + moduleInstanceID + " has been solved with the purchase of property " + shownCard + ".");
        }
        else
        {
            TheModule.HandleStrike();
            Debug.Log("Module " + moduleInstanceID + " has incurred a strike with the attempted purchase of property " + shownCard + ".");
        }

        return false;
    }



    // Update is called once per frame
    void Update () {}
}
