using LaeringslivCore;
using UnityEngine;

/// <summary>
/// Manager class taking care of player related properties
/// </summary>
public class PlayerManager : MonoBehaviour
{

    // Singleton
    public static PlayerManager Instance;

    private int subStatStartValue = 0;
    private int mainStatStartValue = 0;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    GUIManager guiManager => GUIManager.Instance;


    [SerializeField]
    int[] subStats = new int[20];
    [SerializeField]
    int[] mainStats = new int[4];

    public string[] minorStatNames = { "Initiativ", "Ansvarskompetanse", "Tillit", "Trygghet", "Motivasjon", "Nysgjerrighet", "Byggende", "Lyttende", "Imøtekommende", "Strukturert", "Behovsorientert", "Kommunikativ", "Relasjonell", "Oppdragsforståelse", "Utholdenhet", "Økonomi", "Name17", "Name18", "Name19", "Name20" };

    public string GetMinorName(int nameID) {
        return minorStatNames[nameID];
    }

    // Initialize starting values
    private void Start() {
        ResetStats();
    }

    public void ResetStats() {
        for (int i = 0; i < subStats.Length; i++) {
            subStats[i] = subStatStartValue;
        }
        for (int i = 0; i < mainStats.Length; i++) {
            mainStats[i] = mainStatStartValue;
        }
    }


    public void ChangeStats(FeatureVector statChanges) {
        for (int i = 0; i < 20; i++) {
            subStats[i] += statChanges[i];
            if (subStats[i] < 0) subStats[i] = 0;
        }

        int mainStat1Change = 0; // Collection of substats 1-5
        int mainStat2Change = 0; // Collection of substats 6-10
        int mainStat3Change = 0; // Collection of substats 11-15
        int mainStat4Change = 0; // Collection of substats 16-20

        for (int i = 0; i < 4; i++) {
            mainStats[i] = 0;
        }
        


        for (int i = 0; i < 5; i++)
        {
            mainStat1Change += statChanges[i];
            mainStats[0] += subStats[i];
        }
        for (int i = 5; i < 10; i++)
        {
            mainStat2Change += statChanges[i];
            mainStats[1] += subStats[i];
        }
        for (int i = 10; i < 15; i++)
        {
            mainStat3Change += statChanges[i];
            mainStats[2] += subStats[i];
        }
        for (int i = 15; i < 20; i++)
        {
            mainStat4Change += statChanges[i];
            mainStats[3] += subStats[i];
        }

        //mainStats[0] += mainStat1Change;
        //mainStats[1] += mainStat2Change;
        //mainStats[2] += mainStat3Change;
        //mainStats[3] += mainStat4Change;


        // Handle Change events

        if (mainStat1Change > 0)
        {
            // Do something to show the stat went up
            guiManager.HandleStatChangeGraphics(0, mainStat1Change, mainStats[0]);
        }
        else if (mainStat1Change < 0)
        {
            // Do something to show the stat went down
            guiManager.HandleStatChangeGraphics(0, mainStat1Change, mainStats[0]);
        }
        if (mainStat2Change > 0)
        {
            // Do something to show the stat went up
            guiManager.HandleStatChangeGraphics(1, mainStat2Change, mainStats[1]);
        }
        else if (mainStat2Change < 0)
        {
            // Do something to show the stat went down
            guiManager.HandleStatChangeGraphics(1, mainStat2Change, mainStats[1]);
        }
        if (mainStat3Change > 0)
        {
            // Do something to show the stat went up
            guiManager.HandleStatChangeGraphics(2, mainStat3Change, mainStats[2]);
        }
        else if (mainStat3Change < 0)
        {
            // Do something to show the stat went down
            guiManager.HandleStatChangeGraphics(2, mainStat3Change, mainStats[2]);
        }
        if (mainStat4Change > 0)
        {
            // Do something to show the stat went up
            guiManager.HandleStatChangeGraphics(3, mainStat4Change, mainStats[3]);
        }
        else if (mainStat4Change < 0)
        {
            // Do something to show the stat went down
            guiManager.HandleStatChangeGraphics(3, mainStat4Change, mainStats[3]);
        }
    }

    public int GetMainStat(int index) {
        return mainStats[index];
    }

    public int GetSubStat(int index) {
        return subStats[index];
    }

    public int[] GetSubStatsList() {
        return subStats;
    }

    public float GetMainStatPercent(int index) {
        return (float)mainStats[index] / 500f;
    }

    #region SuperUser stat stuff
    

    public void SetStat(int index, int newValue) {
        
        if (newValue < 0) newValue = 0;
        else if (newValue > 100) newValue = 100;

        int oldStatvalue = subStats[index];
        subStats[index] = newValue;
        int statChange = newValue - oldStatvalue;
        if (statChange != 0) {
            if (index < 5) {
                mainStats[0] += statChange;
                guiManager.SuperUserHandleStatChangeGraphics(0, statChange, mainStats[0]);
            } else if (index < 10) {
                mainStats[1] += statChange;
                guiManager.SuperUserHandleStatChangeGraphics(1, statChange, mainStats[1]);
            } else if (index < 15) {
                mainStats[2] += statChange;
                guiManager.SuperUserHandleStatChangeGraphics(2, statChange, mainStats[2]);
            } else if (index < 20) {
                mainStats[3] += statChange;
                guiManager.SuperUserHandleStatChangeGraphics(3, statChange, mainStats[3]);
            }
        }
        
    }



    #endregion


    public void SavePlayerStats() {
        PlayerStateSaveManager.Instance.SaveMinorStats(subStats);
    }

    public void LoadPlayerStats() {
        for (int i = 0; i < 20; i++) {
            subStats[i] = PlayerStateSaveManager.Instance.LoadMinorStat(i);
        }

        int mainStat1Change = 0; // Collection of substats 1-5
        int mainStat2Change = 0; // Collection of substats 6-10
        int mainStat3Change = 0; // Collection of substats 11-15
        int mainStat4Change = 0; // Collection of substats 16-20

        for (int i = 0; i < 5; i++) {
            mainStat1Change += subStats[i];
        }
        for (int i = 5; i < 10; i++) {
            mainStat2Change += subStats[i];
        }
        for (int i = 10; i < 15; i++) {
            mainStat3Change += subStats[i];
        }
        for (int i = 15; i < 20; i++) {
            mainStat4Change += subStats[i];
        }

        for (int i = 0; i < 4; i++) {
            mainStats[i] = 0;
        }
       
        mainStats[0] += mainStat1Change;
        mainStats[1] += mainStat2Change;
        mainStats[2] += mainStat3Change;
        mainStats[3] += mainStat4Change;

        guiManager.UpdateMainStatGraphics(mainStats);
    }

    public void UpdatePlayerVisualScores() {
        guiManager.UpdateMainStatGraphics(mainStats);
    }
}
