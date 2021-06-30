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


    public const int NUM_CARDS = 28; // For determining which card is shown
    private int shownCard;

    private static int numInstances = 0; // For assigning unique debug ID's to each instance of the module
    private int moduleInstanceID;

    public KMSelectable[] Arrows = new KMSelectable[2]; // Left/Right arrows
    public KMSelectable SubmitButton;
    public KMBombModule TheModule; // Refers to the specific instance of PurchasingProperties, handles strikes/solves
    public KMBombInfo TheBomb; // Refers to the entire bomb, handles querying various properties of the bomb
    public TextMesh CardDisplay;

    private string correctColor;
    private string correctPrice;
    private int correctProperty;

    private string[][] propertyArray = new string[][] {
        new string[] {"purple", "cheap"}, new string[] {"purple", "expensive"},
        new string[] {"railroad", "reading"},
        new string[] {"sky", "cheap"}, new string[] {"sky", "middle"}, new string[] {"sky", "expensive"},
        new string[] {"pink", "cheap"}, new string[] {"utility", "electric"}, new string[] {"pink", "middle"}, new string[] {"pink", "expensive"},
        new string[] {"railroad", "pennsylvania"},
        new string[] {"orange", "cheap"}, new string[] {"orange", "middle"}, new string[] {"orange", "expensive"},
        new string[] {"red", "cheap"}, new string[] {"red", "middle"}, new string[] {"red", "expensive"},
        new string[] {"railroad", "B&O"},
        new string[] {"yellow", "cheap"}, new string[] {"yellow", "middle"}, new string[] {"utility", "water"}, new string[] {"yellow", "expensive"},
        new string[] {"green", "cheap"}, new string[] {"green", "middle"}, new string[] {"green", "expensive"},
        new string[] {"railroad", "short"},
        new string[] {"blue", "cheap"}, new string[] {"blue", "expensive"}
    };



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
    private int numAABatteries;

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
            if (bat.numbatteries == 2) numAABatteries += 2;
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
        bool reachedCar = false;
        bool reachedUtil = false;

        #region Colour Rules
        if (hasParallel) correctColor = "pink";
        else if ((hasMSA && !hasLitMSA) || (hasLitNSA)) correctColor = "green";
        else if ((hasNSA && !hasLitNSA) || (hasLitMSA)) correctColor = "blue";
        else if (numBatteries == 1) correctColor = "yellow";
        else if (hasPS2 && hasRJ45) correctColor = "orange";
        else if (hasCAR)
        {
            reachedCar = true;
            correctColor = "railroad";
            correctPrice = (new string[4] {"reading", "pennsylvania", "B&O", "short"})[numPortPlates % 4];
        }
        else if (numPortPlates >= 3) correctColor = "purple";
        else if (numBatteries % 2 == 0) correctColor = "sky";
        else if (numAABatteries >= 4) correctColor = "red";
        else
        {
            reachedUtil = true;
            if (isPrime(numBatteries)) correctPrice = "water";
            else correctPrice = "electric";
        }
        #endregion


        #region Price Rules
        if (reachedCar || reachedUtil) { }
        else if (hasLitIND) correctPrice = "cheap";
        else if (hasFRK && !hasLitFRK) correctPrice = "expensive";
        else if ((serialNumber[5] - '0') % 2 == 0) correctPrice = "cheap";
        else if (hasIND || hasFRQ) correctPrice = "cheap";
        else if ((serialNumber.Contains("0")) && (correctColor != "purple" && correctColor != "blue")) correctPrice = "middle";
        else if (hasCAR)
        {
            reachedCar = true;
            correctColor = "railroad";
            correctPrice = (new string[4] { "reading", "pennsylvania", "B&O", "short" })[numPortPlates % 4];
        }
        else if (numBatteries % 2 == 1) correctPrice = "expensive";
        else if ((hasTRN) && (correctColor != "purple" && correctColor != "blue")) correctPrice = "middle";
        else if (correctColor != "pink" && correctColor != "sky") correctPrice = "expensive";
        else correctPrice = "middle";
        #endregion


        //correctProperty = Array.FindIndex(propertyArray, val => val.Equals(new string[] {correctColor, correctPrice}));
        correctProperty = Array.FindIndex(propertyArray, val => (val[0] == correctColor && val[1] == correctPrice));

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



    private static bool isPrime(int n)
    {
        if ((new List<int> {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37}).Contains(n)) return true;
        else return false;
    }



    // Update is called once per frame
    void Update () {}
}
