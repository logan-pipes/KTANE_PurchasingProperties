using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

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


    private const int NUM_CARDS = 28; // For determining which card is shown
    private int shownCard;

    private static int numInstances = 0; // For assigning unique debug ID's to each instance of the module
    private int moduleInstanceID;

    public KMSelectable[] Arrows = new KMSelectable[2]; // Left/Right arrows
    public KMSelectable SubmitButton;
    public KMBombModule TheModule; // Refers to the specific instance of PurchasingProperties, handles strikes/solves
    public KMBombInfo TheBomb; // Refers to the entire bomb, handles querying various properties of the bomb

    private const int NUM_LINES_TEXT = 13;
    public TextMesh[] CardDisplay = new TextMesh[NUM_LINES_TEXT];

    private string[][] cardText = new string[NUM_CARDS][];

    private string correctColor;
    private string correctPrice;
    private int correctProperty;

    private string[][] propertyArray;

    private int[] nonStandardProperties;



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
        TheModule.OnActivate += FillProperties;
        TheModule.OnActivate += GrabEdgeWork;
    }



    private void FillProperties()
    {
        // for (int i = 0; i < NUM_LINES_TEXT; i++)
        // {
        //     CardDisplay[i] = new TextMesh();
        // }
        propertyArray = new string[][] {
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

        #region Standard Card Text
        cardText[0] = new string[] {"TITLE DEED",
        "MEDITERRANEAN AVE.",
"RENT $2.",
"With 1 House $ 10.",
"With 2 Houses  30.",
"With 3 Houses  90.",
"With 4 Houses  160.",
"With HOTEL $250.",
"Mortgage Value $30.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[1] = new string[] {"TITLE DEED",
"BALTIC AVE.",
"RENT $4.",
"With 1 House $ 20.",
"With 2 Houses  60.",
"With 3 Houses  180.",
"With 4 Houses  320.",
"With HOTEL $450.",
"Mortgage Value $30.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[3] = new string[] {"TITLE DEED",
"ORIENTAL AVE.",
"RENT $6.",
"With 1 House $ 30.",
"With 2 Houses  90.",
"With 3 Houses  270.",
"With 4 Houses  400.",
"With HOTEL $550.",
"Mortgage Value $50.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[4] = new string[] {"TITLE DEED",
"VERMONT AVE.",
"RENT $6.",
"With 1 House $ 30.",
"With 2 Houses  90.",
"With 3 Houses  270.",
"With 4 Houses  400.",
"With HOTEL $550.",
"Mortgage Value $50.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[5] = new string[] {"TITLE DEED",
"CONNECTICUT AVE.",
"RENT $8.",
"With 1 House $ 40.",
"With 2 Houses  100.",
"With 3 Houses  300.",
"With 4 Houses  450.",
"With HOTEL $600.",
"Mortgage Value $60.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[6] = new string[] {"TITLE DEED",
"St. CHARLES PLACE",
"RENT $10.",
"With 1 House $ 50.",
"With 2 Houses  150.",
"With 3 Houses  450.",
"With 4 Houses  625.",
"With HOTEL $750.",
"Mortgage Value $70.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[8] = new string[] {"TITLE DEED",
"STATES AVE.",
"RENT $10.",
"With 1 House $ 50.",
"With 2 Houses  150.",
"With 3 Houses  450.",
"With 4 Houses  625.",
"With HOTEL $750.",
"Mortgage Value $70.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[9] = new string[] {"TITLE DEED",
"VIRGINIA AVE.",
"RENT $12.",
"With 1 House $ 60.",
"With 2 Houses  180.",
"With 3 Houses  500.",
"With 4 Houses  700.",
"With HOTEL $900.",
"Mortgage Value $80.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[11] = new string[] {"TITLE DEED",
"ST. JAMES PLACE",
"RENT $14.",
"With 1 House $ 70.",
"With 2 Houses  200.",
"With 3 Houses  550.",
"With 4 Houses  750.",
"With HOTEL $950.",
"Mortgage Value $90.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[12] = new string[] {"TITLE DEED",
"TENNESSEE AVE.",
"RENT $14.",
"With 1 House $ 70.",
"With 2 Houses  200.",
"With 3 Houses  550.",
"With 4 Houses  750.",
"With HOTEL $950.",
"Mortgage Value $90.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[13] = new string[] {"TITLE DEED",
"NEW YORK AVE.",
"RENT $16.",
"With 1 House $ 80.",
"With 2 Houses  220.",
"With 3 Houses  600.",
"With 4 Houses  800.",
"With HOTEL $1000.",
"Mortgage Value $100.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[14] = new string[] {"TITLE DEED",
"KENTUCKY AVE.",
"RENT $18.",
"With 1 House $ 90.",
"With 2 Houses  250.",
"With 3 Houses  700.",
"With 4 Houses  875.",
"With HOTEL $1050.",
"Mortgage Value $110.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[15] = new string[] {"TITLE DEED",
"INDIANA AVE.",
"RENT $18.",
"With 1 House $ 90.",
"With 2 Houses  250.",
"With 3 Houses  700.",
"With 4 Houses  875.",
"With HOTEL $1050.",
"Mortgage Value $110.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[16] = new string[] {"TITLE DEED",
"ILLINOIS AVE.",
"RENT $20.",
"With 1 House $ 100.",
"With 2 Houses  300.",
"With 3 Houses  750.",
"With 4 Houses  925.",
"With HOTEL $1100.",
"Mortgage Value $120.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[18] = new string[] {"TITLE DEED",
"ATLANTIC AVE.",
"RENT $22.",
"With 1 House $ 110.",
"With 2 Houses  330.",
"With 3 Houses  800.",
"With 4 Houses  975.",
"With HOTEL $1150.",
"Mortgage Value $130.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[19] = new string[] {"TITLE DEED",
"VENTNOR AVE.",
"RENT $22.",
"With 1 House $ 110.",
"With 2 Houses  330.",
"With 3 Houses  800.",
"With 4 Houses  975.",
"With HOTEL $1150.",
"Mortgage Value $130.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[21] = new string[] {"TITLE DEED",
"MARVIN GARDENS",
"RENT $24.",
"With 1 House $ 120.",
"With 2 Houses  360.",
"With 3 Houses  850.",
"With 4 Houses  1025.",
"With HOTEL $1200.",
"Mortgage Value $140.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[22] = new string[] {"TITLE DEED",
"PACIFIC AVE.",
"RENT $26.",
"With 1 House $ 130.",
"With 2 Houses  390.",
"With 3 Houses  900.",
"With 4 Houses  1100.",
"With HOTEL $1275.",
"Mortgage Value $150.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[23] = new string[] {"TITLE DEED",
"NO. CAROLINA AVE.",
"RENT $26.",
"With 1 House $ 130.",
"With 2 Houses  390.",
"With 3 Houses  900.",
"With 4 Houses  1100.",
"With HOTEL $1275.",
"Mortgage Value $150.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[24] = new string[] {"TITLE DEED",
"PENNSYLVANIA AVE.",
"RENT $28.",
"With 1 House $ 150.",
"With 2 Houses  450.",
"With 3 Houses  1000.",
"With 4 Houses  1200.",
"With HOTEL $1400.",
"Mortgage Value $160.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[26] = new string[] {"TITLE DEED",
"PARK PLACE",
"RENT $35.",
"With 1 House $ 175.",
"With 2 Houses  500.",
"With 3 Houses  1100.",
"With 4 Houses  1300.",
"With HOTEL $1500.",
"Mortgage Value $175.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        cardText[27] = new string[] {"TITLE DEED",
"BOARDWALK",
"RENT $50.",
"With 1 House $ 200.",
"With 2 Houses  600.",
"With 3 Houses  1400.",
"With 4 Houses  1700.",
"With HOTEL $2000.",
"Mortgage Value $200.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses",
"If a player owns ALL the Lots of any Color-Group, the",
"rent is Doubled on Unimproved Lots in that group." };
        #endregion

        nonStandardProperties = new int[] { 2, 7, 10, 17, 20, 25 };
        #region Non-Standard Card Text
        cardText[2]  = new string[] {"","","",
"READING RAILROAD", "",
"Rent $25.",
"If 2 R.R.'s are owned 50.",
"If 3 \" \" \" 100.",
"If 4 \" \" \" 200.", "",
"Mortgage Value 100.", "", "" };
        cardText[10] = new string[] {"","","",
"PENNSYLVANIA R.R.", "",
"Rent $25.",
"If 2 R.R.'s are owned 50.",
"If 3 \" \" \" 100.",
"If 4 \" \" \" 200.", "",
"Mortgage Value 100.", "", "" };
        cardText[17] = new string[] {"","","",
"B.&O. RAILROAD", "",
"Rent $25.",
"If 2 R.R.'s are owned 50.",
"If 3 \" \" \" 100.",
"If 4 \" \" \" 200.", "",
"Mortgage Value 100.", "", "" };
        cardText[25] = new string[] {"","","",
"SHORT LINE R.R.", "",
"Rent $25.",
"If 2 R.R.'s are owned 50.",
"If 3 \" \" \" 100.",
"If 4 \" \" \" 200.", "",
"Mortgage Value 100.", "", "" };

        cardText[7] = new string[] {"","",
"WATER WORKS", "",
"If one \"Utility\" is owned",
"rent is 4 times amount shown",
"on dice.",
"If both \"Utilities\" are owned",
"rent is 10 times amount shown",
"on dice.",
"Mortgage Value $75.", "", ""};
        cardText[20] = new string[] {"","",
"ELECTRIC COMPANY", "",
"If one \"Utility\" is owned",
"rent is 4 times amount shown",
"on dice.",
"If both \"Utilities\" are owned",
"rent is 10 times amount shown",
"on dice.",
"Mortgage Value $75.", "", "" };
        #endregion
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

        for (int i = 0; i < NUM_LINES_TEXT; i++)
        {
            CardDisplay[i].text = cardText[shownCard][i];
        }
    
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
