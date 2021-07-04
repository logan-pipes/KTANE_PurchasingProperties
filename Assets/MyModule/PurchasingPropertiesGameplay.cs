using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PurchasingPropertiesGameplay : MonoBehaviour {
    
    #region Edgework Utility Classes
    private class Battery
    {
        public int numbatteries = 0;
    }
    private class Serial
    {
        public string serial;
    }
    private class Indicator
    {
        public string label = "NUL";
        public string on = "false";
    }
    private class Port
    {
        public string[] PresentPorts;
    }
    #endregion

    #region Initializations
    private const int NUM_CARDS = 28; // For determining which card is shown
    private int shownCard;

    private static int numInstances = 0; // For assigning unique debug ID's to each instance of the module
    private int moduleInstanceID;

    public KMSelectable[] Arrows = new KMSelectable[2]; // Left/Right arrows
    public KMSelectable SubmitButton;
    public KMBombModule TheModule; // Refers to the specific instance of PurchasingProperties, handles strikes/solves
    public KMBombInfo TheBomb; // Refers to the entire bomb, handles querying various properties of the bomb
    public Sprite UtilityWater;
    public Sprite UtilityElectric;

    private const int NUM_LINES_TEXT = 9;
    public TextMesh[] CardDisplay = new TextMesh[NUM_LINES_TEXT];

    // private List<int> nonStandardCards;
    private string[][] cardText = new string[NUM_CARDS][];

    private string correctColor;
    private string correctPrice;
    private int correctProperty;

    private string[][] propertyArray;


    #region Edgework variables
    private int numPortPlates;
    private int numPorts;

    private bool hasParallel;
    private bool hasPS2;
    private bool hasRJ45;

    private string serialNumber;

    private int numBatteries;
    private int numAABatteries;

    private int numLitIndicators = 0;

	private bool hasCAR;
	private bool hasIND;
	private bool hasFRQ;
	private bool hasNSA;
	private bool hasMSA;
	private bool hasTRN;
    private bool hasFRK;

    private bool hasLitIND;
    private bool hasLitNSA;
    private bool hasLitMSA;
    private bool hasLitFRK;
    #endregion
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


    // Fills out the property string arrays, runs in Start()
    private void FillProperties()
    {
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
        cardText[0] = new string[] {
        "MEDITERRANEAN AVE.",
"RENT $2.",
"With 1 House         $   10.",
"With 2 Houses            30.",
"With 3 Houses            90.",
"With 4 Houses          160.",
"With HOTEL $250.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses"};
        cardText[1] = new string[] {
"BALTIC AVE.",
"RENT $4.",
"With 1 House         $   20.",
"With 2 Houses            60.",
"With 3 Houses          180.",
"With 4 Houses          320.",
"With HOTEL $450.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses"};
        cardText[3] = new string[] {
"ORIENTAL AVE.",
"RENT $6.",
"With 1 House         $   30.",
"With 2 Houses            90.",
"With 3 Houses          270.",
"With 4 Houses          400.",
"With HOTEL $550.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses"};
        cardText[4] = new string[] {
"VERMONT AVE.",
"RENT $6.",
"With 1 House         $   30.",
"With 2 Houses            90.",
"With 3 Houses          270.",
"With 4 Houses          400.",
"With HOTEL $550.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses"};
        cardText[5] = new string[] {
"CONNECTICUT AVE.",
"RENT $8.",
"With 1 House         $   40.",
"With 2 Houses          100.",
"With 3 Houses          300.",
"With 4 Houses          450.",
"With HOTEL $600.",
"Houses cost $50. each",
"Hotels, $50. plus 4 houses"};
        cardText[6] = new string[] {
"ST. CHARLES PLACE",
"RENT $10.",
"With 1 House         $   50.",
"With 2 Houses          150.",
"With 3 Houses          450.",
"With 4 Houses          625.",
"With HOTEL $750.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[8] = new string[] {
"STATES AVE.",
"RENT $10.",
"With 1 House         $   50.",
"With 2 Houses          150.",
"With 3 Houses          450.",
"With 4 Houses          625.",
"With HOTEL $750.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[9] = new string[] {
"VIRGINIA AVE.",
"RENT $12.",
"With 1 House         $   60.",
"With 2 Houses          180.",
"With 3 Houses          500.",
"With 4 Houses          700.",
"With HOTEL $900.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[11] = new string[] {
"ST. JAMES PLACE",
"RENT $14.",
"With 1 House         $   70.",
"With 2 Houses          200.",
"With 3 Houses          550.",
"With 4 Houses          750.",
"With HOTEL $950.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[12] = new string[] {
"TENNESSEE AVE.",
"RENT $14.",
"With 1 House         $   70.",
"With 2 Houses          200.",
"With 3 Houses          550.",
"With 4 Houses          750.",
"With HOTEL $950.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[13] = new string[] {
"NEW YORK AVE.",
"RENT $16.",
"With 1 House         $   80.",
"With 2 Houses          220.",
"With 3 Houses          600.",
"With 4 Houses          800.",
"With HOTEL $1000.",
"Houses cost $100. each",
"Hotels, $100. plus 4 houses"};
        cardText[14] = new string[] {
"KENTUCKY AVE.",
"RENT $18.",
"With 1 House         $   90.",
"With 2 Houses          250.",
"With 3 Houses          700.",
"With 4 Houses          875.",
"With HOTEL $1050.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[15] = new string[] {
"INDIANA AVE.",
"RENT $18.",
"With 1 House         $   90.",
"With 2 Houses          250.",
"With 3 Houses          700.",
"With 4 Houses          875.",
"With HOTEL $1050.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[16] = new string[] {
"ILLINOIS AVE.",
"RENT $20.",
"With 1 House         $ 100.",
"With 2 Houses          300.",
"With 3 Houses          750.",
"With 4 Houses          925.",
"With HOTEL $1100.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[18] = new string[] {
"ATLANTIC AVE.",
"RENT $22.",
"With 1 House         $ 110.",
"With 2 Houses          330.",
"With 3 Houses          800.",
"With 4 Houses          975.",
"With HOTEL $1150.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[19] = new string[] {
"VENTNOR AVE.",
"RENT $22.",
"With 1 House         $ 110.",
"With 2 Houses          330.",
"With 3 Houses          800.",
"With 4 Houses          975.",
"With HOTEL $1150.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[21] = new string[] {
"MARVIN GARDENS",
"RENT $24.",
"With 1 House       $   120.",
"With 2 Houses          360.",
"With 3 Houses          850.",
"With 4 Houses        1025.",
"With HOTEL $1200.",
"Houses cost $150. each",
"Hotels, $150. plus 4 houses"};
        cardText[22] = new string[] {
"PACIFIC AVE.",
"RENT $26.",
"With 1 House       $   130.",
"With 2 Houses          390.",
"With 3 Houses          900.",
"With 4 Houses        1100.",
"With HOTEL $1275.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses"};
        cardText[23] = new string[] {
"NO. CAROLINA AVE.",
"RENT $26.",
"With 1 House       $   130.",
"With 2 Houses          390.",
"With 3 Houses          900.",
"With 4 Houses        1100.",
"With HOTEL $1275.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses"};
        cardText[24] = new string[] {
"PENNSYLVANIA AVE.",
"RENT $28.",
"With 1 House       $   150.",
"With 2 Houses          450.",
"With 3 Houses        1000.",
"With 4 Houses        1200.",
"With HOTEL $1400.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses"};
        cardText[26] = new string[] {
"PARK PLACE",
"RENT $35.",
"With 1 House       $   175.",
"With 2 Houses          500.",
"With 3 Houses        1100.",
"With 4 Houses        1300.",
"With HOTEL $1500.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses"};
        cardText[27] = new string[] {
"BOARDWALK",
"RENT $50.",
"With 1 House       $   200.",
"With 2 Houses          600.",
"With 3 Houses        1400.",
"With 4 Houses        1700.",
"With HOTEL $2000.",
"Houses cost $200. each",
"Hotels, $200. plus 4 houses"};
        #endregion

        //nonStandardCards = new List<int> { 2, 7, 10, 17, 20, 25 };
        #region Non-Standard Card Text
        cardText[2]  = new string[] {"",
"READING RAILROAD", "","",
"Rent                            $25.",
"If 2 R.R.'s are owned    50.",
"If 3     \"     \"     \"           100.",
"If 4     \"     \"     \"           200.", ""};
        cardText[10] = new string[] {"",
"PENNSYLVANIA R.R.", "","",
"Rent                            $25.",
"If 2 R.R.'s are owned    50.",
"If 3     \"     \"     \"           100.",
"If 4     \"     \"     \"           200.", ""};
        cardText[17] = new string[] {"",
"B.&O. RAILROAD", "","",
"Rent                            $25.",
"If 2 R.R.'s are owned    50.",
"If 3     \"     \"     \"           100.",
"If 4     \"     \"     \"           200.", ""};
        cardText[25] = new string[] {"",
"SHORT LINE R.R.", "","",
"Rent                            $25.",
"If 2 R.R.'s are owned    50.",
"If 3     \"     \"     \"           100.",
"If 4     \"     \"     \"           200.", ""};

        cardText[7] = new string[] {"",
"WATER WORKS", "",
"If one Utility is owned",
"rent is 4 times amount",
"shown on dice.",
"If both Utilities are owned",
"rent is 10 times amount",
"shown on dice."};
        cardText[20] = new string[] {"",
"ELECTRIC COMPANY", "",
"If one Utility is owned",
"rent is 4 times amount",
"shown on dice.",
"If both Utilities are owned",
"rent is 10 times amount",
"shown on dice."};
        #endregion
    }


    // Assigns variables based on bomb info, runs in Start()
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
                if (elem == "Parallel") hasParallel = true;
                if (elem == "PS2") hasPS2 = true;
                if (elem == "RJ45") hasRJ45 = true;
                #endregion
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

        // ----- Indicators -----
        List<string> indicatorList = TheBomb.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        foreach (string indicator in indicatorList)
        {
            Indicator ind = JsonConvert.DeserializeObject<Indicator>(indicator);
            #region hasIndicators
            if (ind.on == "True" && ind.label == "IND") hasLitIND = true;
            if (ind.on == "True" && ind.label == "NSA") hasLitNSA = true;
            if (ind.on == "True" && ind.label == "MSA") hasLitMSA = true;
            if (ind.on == "True" && ind.label == "FRK") hasLitFRK = true;

            if (ind.label == "CAR") hasCAR = true;
            if (ind.label == "IND") hasIND = true;
            if (ind.label == "FRQ") hasFRQ = true;
            if (ind.label == "NSA") hasNSA = true;
            if (ind.label == "MSA") hasMSA = true;
            if (ind.label == "TRN") hasTRN = true;
            if (ind.label == "FRK") hasFRK = true;
            #endregion

            if (ind.on == "True") numLitIndicators++;
        }

        CalculatePropToPurchase();
    }


    // Calculates which property is the oslution to the module, given the edgework of the bomb, runs in GrabEdgeWork()
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


        correctProperty = Array.FindIndex(propertyArray, val => (val[0] == correctColor && val[1] == correctPrice));

        return false;
    }


    // Changes which card is shown, runs OnInteract with the up/down buttons
    private bool CycleCardDisplay(int dir)
    {
        shownCard = (shownCard + dir) % NUM_CARDS; // Change the card index
        if (shownCard < 0) shownCard += NUM_CARDS;
        Debug.Log("Module " + moduleInstanceID + " cycled the display in the " + dir + " direction, and now card " + shownCard + " is showing.");

        for (int i = 0; i < NUM_LINES_TEXT; i++)
        {
            CardDisplay[i].text = cardText[shownCard][i];
        }
        if (shownCard == 7)
        {
            //UtilityWater.show
        }
        if (shownCard == 20)
        {
            //stuff
        }
        
        // <TextMesh>.fontStyle = FontStyle.<bold/normal>
        // if (nonStandardCards.Contains(shownCard)) CardDisplay[1].fontStyle = FontStyle.Bold;
        // else CardDisplay[1].fontStyle = FontStyle.Normal;

        return false;
    }


    // Checks for a solution, issuing a strike or solving the bomb, runs OnInteract with the submit button
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


    // Helper method to determine if a number is prime, used in CalculatePropToPurchase()
    private static bool isPrime(int n)
    {
        if (n <= 1) return false;
        if (n == 2) return true;
        int sq = (int)Math.Floor(Math.Sqrt(n));

        for (int i = 3; i <= sq; i += 2)
        {
            if (n % i == 0) return false;
        }

        return true;
    }

    
    // Update is called once per frame
    void Update () {}
}
