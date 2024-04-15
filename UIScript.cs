using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class UIScript : MonoBehaviour
{
    public bool isTestMode;

    public DataManager Gdata;
    public GameManagement Gmanager;
    public MainBaseScript BaseData;
    public bool isDead;

    public RawImage blackScreen;
    public Color blackScreenCol;

    public float ScreenWidthOrigin;
    public float ScreenHeightOrigin;

    public TextMeshProUGUI FPSDisplayText;
    float deltaTime;

    [Header("CrossHair Components")]
    public GameObject CrosshairParent;
    public RectTransform Crosshair_1;
    public RectTransform Crosshair_2;
    public RectTransform Crosshair_3;
    public RectTransform Crosshair_4;
    public RawImage Crosshair_1_Image;
    public RawImage Crosshair_2_Image;
    public RawImage Crosshair_3_Image;
    public RawImage Crosshair_4_Image;
    public RawImage Crosshair_Dot_Image;
    public RawImage Crosshair_SmallDot_Image;
    public Slider Crosshair_TowerCooldownSlider;
    public RawImage DamageHitImage;
    public RawImage CriticalHitImage;
    public float DamageHitImage_Transparency;
    public float CriticalHitImage_Transparency;
    public Color DamageHitColor;

    [Header("TowerStat")]
    public GameObject TowerStatusUIParent;
    public RawImage TowerIcon;
    public RawImage UpgradeIcon;
    public RawImage ArmorPiercingIcon;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI FirerateText;
    public TextMeshProUGUI RangeText;
    public TextMeshProUGUI DownedText;
    public RawImage UpgradeButtonImage;
    public RawImage SellButtonImage;
    public TextMeshProUGUI UpgradeText;
    public TextMeshProUGUI SellText;
    public GameObject ResourceImagesParent;
    public TextMeshProUGUI Upgrade_ScrapsText;
    public TextMeshProUGUI Upgrade_AdvancedResourcesText;
    public TextMeshProUGUI Upgrade_EnergiesText;
    public TextMeshProUGUI Upgrade_TimerText;
    public TextMeshProUGUI UpgradeNameText;
    public TextMeshProUGUI UpgradeInfoText;
    public int RemainingTimeToRepair;

    [Header("WeaponUI")]
    public GameObject WeaponUIParent;
    public RawImage WeaponIcon;
    public TextMeshProUGUI AmmoText;
    public TextMeshProUGUI Weapon_NameText;
    public TextMeshProUGUI LeftEnergyText;
    public TextMeshProUGUI CostText;
    public Slider HPSlider;
    public Slider ShieldSlider;
    public Slider EnergySlider;
    public Slider TowerCooldownSlider;
    public RectTransform WeaponUIRectTransform;
    public Vector3 WeaponUIOriginTransform;

    [Header("Resources")]
    public GameObject ResourceUIParent;
    public TextMeshProUGUI ScrapText;
    public TextMeshProUGUI AdvancedResourcesText;
    public TextMeshProUGUI EnergyText;
    public TextMeshProUGUI NotEnoughEnergyText;
    public RectTransform ResourceUIRectTransform;
    public Vector3 ResourceUIOriginTransform;

    [Header("BuildSystemUI")]
    public GameObject BuildSystemUIParent;
    public RawImage Cost_TowerIcon;
    public RawImage Cost_ScrapsImage;
    public RawImage Cost_AdvancedResourcesImage;
    public RawImage Cost_EnergiesImage;
    public TextMeshProUGUI Cost_ScrapsText;
    public TextMeshProUGUI Cost_AdvancedResourcesText;
    public TextMeshProUGUI Cost_EnergiesText;
    public TextMeshProUGUI Cost_TowerName;
    public TextMeshProUGUI Cost_NotResearchedText;
    public TextMeshProUGUI Cost_TimeText;

    [Header("BaseInteractionUI")]
    public GameObject BaseInteractionUIParent;
    public RectTransform BaseInteractionUIRect;
    public GameObject BI_ResourcesParent;
    public TextMeshProUGUI BI_ScrapText;
    public TextMeshProUGUI BI_AdvancedResourceText;
    public TextMeshProUGUI BI_EnergyText;

    [Header("ResearchTabUI")]
    public GameObject ResearchTabParent;
    public TextMeshProUGUI R_AdvancedResourceText;
    public TextMeshProUGUI R_EnergyText;
    public TextMeshProUGUI R_TimerText;
    public GameObject AlreadyResearchedTextObject;
    public TextMeshProUGUI ResearchInProgressText;
    public TextMeshProUGUI R_Name;
    public RawImage R_Icon;
    public RawImage R_ArmorPiercingIcon;
    public Slider R_TimerSlider;
    public List<ResearchData> ResearchDataList = new List<ResearchData>();
    public int CurrentListNum;
    public bool Researching;

    [Header("BaseInfo")]
    public GameObject BaseInfoParent;
    public Slider BaseHPSlider;
    public TextMeshProUGUI EnergyGeneratesPerSecText;
    public TextMeshProUGUI BaseHPText;
    public TextMeshProUGUI RepairCostText;
    public Button RepairButton;
    public TextMeshProUGUI UpgradeGenCostText;
    public Button UpgradeGenButton;

    [Header("AdvancedResourceGenerateTab")]
    public GameObject AdvancedResourceGenerateTabParent;
    public Slider TimeSlider;
    public bool GeneratingAdRe;

    [Header("WeaponResearchTab")]
    public GameObject Weapon_ResearchTabParent;
    public TextMeshProUGUI Weapon_R_AdvancedResourceText;
    public TextMeshProUGUI Weapon_R_EnergyText;
    public TextMeshProUGUI Weapon_R_TimerText;
    public GameObject Weapon_AlreadyResearchedTextObject;
    public TextMeshProUGUI Weapon_ResearchInProgressText;
    public TextMeshProUGUI Weapon_R_Name;
    public RawImage Weapon_R_Icon;
    public Slider Weapon_R_TimerSlider;
    public List<GunData> Weapon_ResearchDataList = new List<GunData>();
    public int Weapon_CurrentListNum;
    public bool Weapon_Researching;
    
    [Header("AdvancedResourceSynthesisTab")]
    public GameObject AdReSynthTabParent;
    public Slider AdReSynth_TimerSlider;
    public Button AdReSynth_GenButton;
    public TextMeshProUGUI AdReSynth_TimeText;
    public TextMeshProUGUI AdReSynth_ScrapText;
    public TextMeshProUGUI AdReSynth_EnergyText;
    public GameObject AdReSynth_GeneratingTextObject;

    [Header("PlayerResourceRechargeTab")]
    public GameObject PlayerResourceRechargeTabParent;
    public TextMeshProUGUI WeaponEnergiesText;
    public TextMeshProUGUI ShieldsText;
    public TextMeshProUGUI EnergyCost_WeaponEnergyRecharge;
    public TextMeshProUGUI EnergyCost_ShieldRecharge;
    public TextMeshProUGUI WeaponEnergyUpgradeCostText;
    public TextMeshProUGUI ShieldUpgradeCostText;


    [Header("WaveUI")]
    public GameObject WaveUIParent;
    public RectTransform WaveUIRect;
    public Vector3 WaveUIOrigin;
    public TextMeshProUGUI RemainingEnemiesText;
    public TextMeshProUGUI CurrentWave;
    public TextMeshProUGUI WaveStoppedText;
    public Slider WaveFreeTimeSlider;

    [Header("BossBar")]
    public GameObject BossBarParent;
    public Slider BossBarSlider;
    public TextMeshProUGUI BossBarText;
    public GameObject BossObject;
    public EnemyController BossController;

    [Header("ObjectInteractionUI")]
    public GameObject InteractionUIParent;
    public Slider OI_Timer;
    public TextMeshProUGUI OI_Text;

    public float NEETTransparency;

    public GunScript gunScript;
    public PlayerController_CharacterController playerController;

    public float spread_lerp;
    public float crosshair_opacity_lerp;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        blackScreen = transform.Find("BlackScreen").GetComponent<RawImage>();
        blackScreenCol = new Color(0, 0, 0, 1);
        blackScreen.color = blackScreenCol;
        if (!isTestMode)
        {
            StartCoroutine(blackScreenOff(0.01f));
        }

        ScreenWidthOrigin = Screen.width;
        ScreenHeightOrigin = Screen.height;

        CurrentListNum = 0;
        Researching = false;
        GeneratingAdRe = false;
        Gdata = GameObject.Find("DataManager").GetComponent<DataManager>();
        Gmanager = GameObject.Find("DataManager").GetComponent<GameManagement>();
        if(!isTestMode)
            BaseData = GameObject.Find("MainBase").GetComponent<MainBaseScript>();
        FPSDisplayText = transform.Find("FPSDisplay").GetComponent<TextMeshProUGUI>();
        deltaTime = 0;

        FPSDisplayText.enabled = Gdata.ShowFPS;

        spread_lerp = 0;
        crosshair_opacity_lerp = 0;

        CrosshairParent = transform.Find("Crosshairs").gameObject;
        Crosshair_1 = transform.Find("Crosshairs").transform.Find("1").GetComponent<RectTransform>();
        Crosshair_2 = transform.Find("Crosshairs").transform.Find("2").GetComponent<RectTransform>();
        Crosshair_3 = transform.Find("Crosshairs").transform.Find("3").GetComponent<RectTransform>();
        Crosshair_4 = transform.Find("Crosshairs").transform.Find("4").GetComponent<RectTransform>();

        Crosshair_1_Image = transform.Find("Crosshairs").transform.Find("1").GetComponent<RawImage>();
        Crosshair_2_Image = transform.Find("Crosshairs").transform.Find("2").GetComponent<RawImage>();
        Crosshair_3_Image = transform.Find("Crosshairs").transform.Find("3").GetComponent<RawImage>();
        Crosshair_4_Image = transform.Find("Crosshairs").transform.Find("4").GetComponent<RawImage>();
        Crosshair_Dot_Image = transform.Find("Crosshairs").transform.Find("Dot").GetComponent<RawImage>();
        Crosshair_SmallDot_Image = transform.Find("Crosshairs").transform.Find("SmallDot").GetComponent<RawImage>();
        DamageHitImage = transform.Find("DamageHit").GetComponent<RawImage>();
        CriticalHitImage = transform.Find("CriticalHit").GetComponent<RawImage>();

        DamageHitImage_Transparency = 0;
        CriticalHitImage_Transparency = 0;

        Crosshair_TowerCooldownSlider = transform.Find("Crosshairs").transform.Find("TowerCooldownSlider").GetComponent<Slider>();
        Crosshair_TowerCooldownSlider.value = 0;

        Crosshair_Dot_Image.enabled = false;

        gunScript = GameObject.Find("CameraHolder").transform.Find("Parent_GunHolder").transform.Find("GunHolder").transform.Find("Gun").GetComponent<GunScript>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        if (!isTestMode)
        {
            //Tower Status UI Components
            TowerStatusUIParent = transform.Find("TowerStatusUI").gameObject;
            TowerIcon = TowerStatusUIParent.transform.Find("Icon").GetComponent<RawImage>();
            UpgradeIcon = TowerStatusUIParent.transform.Find("UpgradeIcon").GetComponent<RawImage>();
            ArmorPiercingIcon = TowerStatusUIParent.transform.Find("ArmorPiercingIcon").GetComponent<RawImage>();
            NameText = TowerStatusUIParent.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            HPText = TowerStatusUIParent.transform.Find("HP").GetComponent<TextMeshProUGUI>();
            DamageText = TowerStatusUIParent.transform.Find("Damage").GetComponent<TextMeshProUGUI>();
            FirerateText = TowerStatusUIParent.transform.Find("Firerate").GetComponent<TextMeshProUGUI>();
            RangeText = TowerStatusUIParent.transform.Find("Range").GetComponent<TextMeshProUGUI>();
            DownedText = TowerStatusUIParent.transform.Find("DownedText").GetComponent<TextMeshProUGUI>();
            UpgradeButtonImage = TowerStatusUIParent.transform.Find("UpgradeButtonImage").GetComponent<RawImage>();
            SellButtonImage = TowerStatusUIParent.transform.Find("SellButtonImage").GetComponent<RawImage>();
            UpgradeText = UpgradeButtonImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            SellText = SellButtonImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            UpgradeNameText = TowerStatusUIParent.transform.Find("UpgradeNameText").GetComponent<TextMeshProUGUI>();
            UpgradeInfoText = TowerStatusUIParent.transform.Find("UpgradeInfoText").GetComponent<TextMeshProUGUI>();
            //Upgrade Info of Tower Data Status Components
            ResourceImagesParent = TowerStatusUIParent.transform.Find("ResourceImages").gameObject;
            Upgrade_ScrapsText = ResourceImagesParent.transform.Find("ScrapImage").transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Upgrade_AdvancedResourcesText = ResourceImagesParent.transform.Find("AdReImage").transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Upgrade_EnergiesText = ResourceImagesParent.transform.Find("EnergyImage").transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Upgrade_TimerText = ResourceImagesParent.transform.Find("TimeImage").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }
        //Weapon UI Components
        WeaponUIParent = transform.Find("WeaponUI").gameObject;
        AmmoText = WeaponUIParent.transform.Find("Texts").transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
        Weapon_NameText = WeaponUIParent.transform.Find("Texts").transform.Find("Name").GetComponent<TextMeshProUGUI>();
        NotEnoughEnergyText = WeaponUIParent.transform.Find("NotEnoughEnergyText").GetComponent<TextMeshProUGUI>();
        LeftEnergyText = WeaponUIParent.transform.Find("Texts").transform.Find("EnergyLeft").GetComponent<TextMeshProUGUI>();
        CostText = WeaponUIParent.transform.Find("Texts").transform.Find("Cost").GetComponent<TextMeshProUGUI>();
        WeaponUIRectTransform = WeaponUIParent.GetComponent<RectTransform>();
        WeaponUIOriginTransform = WeaponUIRectTransform.position;
        WeaponIcon = WeaponUIParent.transform.Find("Image").GetComponent<RawImage>();
        //Other Stats in Weapon UI Components
        HPSlider = WeaponUIParent.transform.Find("HPSlider").GetComponent<Slider>();
        ShieldSlider = WeaponUIParent.transform.Find("ShieldSlider").GetComponent<Slider>();
        EnergySlider = WeaponUIParent.transform.Find("EnergySlider").GetComponent<Slider>();
        if (!isTestMode)
            TowerCooldownSlider = TowerStatusUIParent.transform.Find("CooldownSlider").GetComponent<Slider>();
        if (!isTestMode)
        {
            //Resource UI Components
            ResourceUIParent = transform.Find("ResourceUI").gameObject;
            ScrapText = ResourceUIParent.transform.Find("ScrapText").GetComponent<TextMeshProUGUI>();
            AdvancedResourcesText = ResourceUIParent.transform.Find("AdReText").GetComponent<TextMeshProUGUI>();
            EnergyText = ResourceUIParent.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            ResourceUIRectTransform = ResourceUIParent.GetComponent<RectTransform>();
            ResourceUIOriginTransform = ResourceUIRectTransform.position;
        }
        if (!isTestMode)
        {
            TowerStatusUIParent.SetActive(false);
            //BuildSystem UI Components
            BuildSystemUIParent = transform.Find("BuildSystemUI").gameObject;
            Cost_TowerIcon = BuildSystemUIParent.transform.Find("Icon").GetComponent<RawImage>();
            Cost_ScrapsImage = BuildSystemUIParent.transform.Find("Images").transform.Find("ScrapImage").GetComponent<RawImage>();
            Cost_AdvancedResourcesImage = BuildSystemUIParent.transform.Find("Images").transform.Find("AdReImage").GetComponent<RawImage>();
            Cost_EnergiesImage = BuildSystemUIParent.transform.Find("Images").transform.Find("EnergyImage").GetComponent<RawImage>();
            Cost_ScrapsText = Cost_ScrapsImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Cost_AdvancedResourcesText = Cost_AdvancedResourcesImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Cost_EnergiesText = Cost_EnergiesImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Cost_TimeText = BuildSystemUIParent.transform.Find("Images").transform.Find("TimeImage").transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Cost_TowerName = BuildSystemUIParent.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            Cost_NotResearchedText = BuildSystemUIParent.transform.Find("NotResearched").GetComponent<TextMeshProUGUI>();
            //BaseInteraction UI Components
            BaseInteractionUIParent = transform.Find("BaseInteractionUI").gameObject;
            BaseInteractionUIRect = BaseInteractionUIParent.GetComponent<RectTransform>();
            BI_ResourcesParent = BaseInteractionUIParent.transform.Find("Resources").gameObject;
            BI_ScrapText = BI_ResourcesParent.transform.Find("ScrapText").GetComponent<TextMeshProUGUI>();
            BI_AdvancedResourceText = BI_ResourcesParent.transform.Find("AdReText").GetComponent<TextMeshProUGUI>();
            BI_EnergyText = BI_ResourcesParent.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            //ResearchTab UI Components
            ResearchTabParent = BaseInteractionUIParent.transform.Find("ResearchTab").gameObject;
            R_AdvancedResourceText = ResearchTabParent.transform.Find("AdReCostText").GetComponent<TextMeshProUGUI>();
            R_EnergyText = ResearchTabParent.transform.Find("EnergyCostText").GetComponent<TextMeshProUGUI>();
            R_TimerText = ResearchTabParent.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
            AlreadyResearchedTextObject = ResearchTabParent.transform.Find("AlreadyResearchedText").gameObject;
            ResearchInProgressText = ResearchTabParent.transform.Find("Researching").GetComponent<TextMeshProUGUI>();
            R_Name = ResearchTabParent.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            R_Icon = ResearchTabParent.transform.Find("Icon").GetComponent<RawImage>();
            R_ArmorPiercingIcon = ResearchTabParent.transform.Find("ArmorPiercingIcon").GetComponent<RawImage>();
            R_TimerSlider = ResearchTabParent.transform.Find("TimerSlider").GetComponent<Slider>();
            //WeaponResearchTab UI Components
            Weapon_ResearchTabParent = BaseInteractionUIParent.transform.Find("WeaponResearchTab").gameObject;
            Weapon_R_AdvancedResourceText = Weapon_ResearchTabParent.transform.Find("AdReCostText").GetComponent<TextMeshProUGUI>();
            Weapon_R_EnergyText = Weapon_ResearchTabParent.transform.Find("EnergyCostText").GetComponent<TextMeshProUGUI>();
            Weapon_R_TimerText = Weapon_ResearchTabParent.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
            Weapon_AlreadyResearchedTextObject = Weapon_ResearchTabParent.transform.Find("AlreadyResearchedText").gameObject;
            Weapon_ResearchInProgressText = Weapon_ResearchTabParent.transform.Find("Researching").GetComponent<TextMeshProUGUI>();
            Weapon_R_Name = Weapon_ResearchTabParent.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            Weapon_R_Icon = Weapon_ResearchTabParent.transform.Find("Icon").GetComponent<RawImage>();
            Weapon_R_TimerSlider = Weapon_ResearchTabParent.transform.Find("TimerSlider").GetComponent<Slider>();
            //BaseInfoTab UI Components
            BaseInfoParent = BaseInteractionUIParent.transform.Find("BaseInfoTab").gameObject;
            BaseHPSlider = BaseInfoParent.transform.Find("BaseHPSlider").GetComponent<Slider>();
            EnergyGeneratesPerSecText = BaseInfoParent.transform.Find("EnergyGeneratesPerSecText").GetComponent<TextMeshProUGUI>();
            BaseHPText = BaseInfoParent.transform.Find("BaseHPText").GetComponent<TextMeshProUGUI>();
            RepairCostText = BaseInfoParent.transform.Find("RepairBaseCostText").GetComponent<TextMeshProUGUI>();
            RepairButton = BaseInfoParent.transform.Find("RepairBaseButton").GetComponent<Button>();
            UpgradeGenCostText = BaseInfoParent.transform.Find("UpgradeGeneratePerSecondsCostText").GetComponent<TextMeshProUGUI>();
            UpgradeGenButton = BaseInfoParent.transform.Find("UpgradeGeneratePerSeconds").GetComponent<Button>();
            //AdvancedResourcesSynthesisTab UI Components
            AdReSynthTabParent = BaseInteractionUIParent.transform.Find("AdvancedResourceGenerateTab").gameObject;
            AdReSynth_TimerSlider = AdReSynthTabParent.transform.Find("TimeSlider").GetComponent<Slider>();
            AdReSynth_GenButton = AdReSynthTabParent.transform.Find("GenerateButton").GetComponent<Button>();
            AdReSynth_TimeText = AdReSynthTabParent.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
            AdReSynth_ScrapText = AdReSynthTabParent.transform.Find("ScrapText").GetComponent<TextMeshProUGUI>();
            AdReSynth_EnergyText = AdReSynthTabParent.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            AdReSynth_GeneratingTextObject = AdReSynthTabParent.transform.Find("GeneratingText").gameObject;
            //PlayerResourcesTab UI Components
            PlayerResourceRechargeTabParent = BaseInteractionUIParent.transform.Find("PlayerResourceRechargeTab").gameObject;
            WeaponEnergiesText = PlayerResourceRechargeTabParent.transform.Find("WeaponEnergiesText").GetComponent<TextMeshProUGUI>();
            ShieldsText = PlayerResourceRechargeTabParent.transform.Find("ShieldText").GetComponent<TextMeshProUGUI>();
            EnergyCost_WeaponEnergyRecharge = PlayerResourceRechargeTabParent.transform.Find("EnergyCost_WeaponEnergyRecharge").GetComponent<TextMeshProUGUI>();
            EnergyCost_ShieldRecharge = PlayerResourceRechargeTabParent.transform.Find("EnergyCost_ShieldRecharge").GetComponent<TextMeshProUGUI>();
            WeaponEnergyUpgradeCostText = PlayerResourceRechargeTabParent.transform.Find("WeaponEnergyUpgradeCostText").GetComponent<TextMeshProUGUI>();
            ShieldUpgradeCostText = PlayerResourceRechargeTabParent.transform.Find("ShieldUpgradeCostText").GetComponent<TextMeshProUGUI>();
            //Wave UI Components
            WaveUIParent = transform.Find("WaveUI").gameObject;
            WaveUIRect = WaveUIParent.GetComponent<RectTransform>();
            WaveUIOrigin = WaveUIRect.position;
            BossBarParent = WaveUIParent.transform.Find("BossBar").gameObject;
            BossBarSlider = BossBarParent.transform.Find("HPBar").GetComponent<Slider>();
            RemainingEnemiesText = WaveUIParent.transform.Find("RemainingEnemies").GetComponent<TextMeshProUGUI>();
            CurrentWave = WaveUIParent.transform.Find("CurrentWave").GetComponent<TextMeshProUGUI>();
            WaveStoppedText = WaveUIParent.transform.Find("WaveStoppedText").GetComponent<TextMeshProUGUI>();
            WaveFreeTimeSlider = WaveUIParent.transform.Find("WaveFreeTimeSlider").GetComponent<Slider>();
            BossBarText = BossBarParent.transform.Find("HPText").GetComponent<TextMeshProUGUI>();
            //ObjectInteraction(OI)UI Components
            InteractionUIParent = CrosshairParent.transform.Find("InteractionTimer").gameObject;
            OI_Timer = InteractionUIParent.GetComponent<Slider>();
            OI_Text = InteractionUIParent.transform.Find("InfoText").GetComponent<TextMeshProUGUI>();

            AdReSynth_TimeText.text = BaseData.AdReSynthCost_Time.ToString();
            AdReSynth_ScrapText.text = BaseData.AdReSynthCost_Scraps.ToString();
            AdReSynth_EnergyText.text = BaseData.AdReSynthCost_Energies.ToString();

            BossBarParent.SetActive(false);
            HideObjectInteractionUI();

            foreach (ResearchData r in ResearchDataList)
            {
                r.ResearchingTowerData.Researched = r.AlreadyResearchedOnStart;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Gdata.ShowFPS)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            FPSDisplayText.text = (1 / deltaTime) + " FPS";
        }

        if (playerController.HP > 0)
            HPSlider.value = Mathf.Lerp(HPSlider.value, playerController.HP / playerController.MaxHP, Time.deltaTime * 3);
        else
            HPSlider.value = Mathf.Lerp(HPSlider.value, 0, Time.deltaTime * 3);

        if (playerController.Shield > 0)
            ShieldSlider.value = Mathf.Lerp(ShieldSlider.value, playerController.Shield / playerController.MaxShield, Time.deltaTime * 3);
        else
            ShieldSlider.value = Mathf.Lerp(ShieldSlider.value, 0, Time.deltaTime * 3);

        if (playerController.Energy > 0)
            EnergySlider.value = Mathf.Lerp(EnergySlider.value, playerController.Energy / playerController.MaxEnergy, Time.deltaTime * 3);
        else
            EnergySlider.value = Mathf.Lerp(EnergySlider.value, 0, Time.deltaTime * 3);

        DamageHitImage_Transparency = Mathf.Lerp(DamageHitImage_Transparency, 0, Time.deltaTime * 3);
        DamageHitImage.color = new Color(DamageHitImage.color.r, DamageHitImage.color.g, DamageHitImage.color.b, DamageHitImage_Transparency);

        CriticalHitImage_Transparency = Mathf.Lerp(CriticalHitImage_Transparency, 0, Time.deltaTime * 3);
        CriticalHitImage.color = new Color(CriticalHitImage.color.r, CriticalHitImage.color.g, CriticalHitImage.color.b, CriticalHitImage_Transparency);

        NEETTransparency = Mathf.Lerp(NEETTransparency, 0, Time.deltaTime / 2);
        NotEnoughEnergyText.color = new Color(1, 1, 1, NEETTransparency);

        WeaponIcon.texture = playerController.gunScript.gunData.WeaponIcon;
        AmmoText.text = playerController.gunScript.gunData.currentAmmo + " / " + playerController.gunScript.gunData.magSize;
        Weapon_NameText.text = playerController.gunScript.gunData.WeaponName;

        LeftEnergyText.text = ((int)playerController.Energy).ToString();
        CostText.text = playerController.gunScript.gunData.reloadCostEnergy.ToString();

        WeaponUIParent.GetComponent<RectTransform>().rotation = Quaternion.Slerp(WeaponUIParent.GetComponent<RectTransform>().rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
        ResourceUIParent.GetComponent<RectTransform>().rotation = Quaternion.Slerp(ResourceUIParent.GetComponent<RectTransform>().rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);

        if (isTestMode)
            return;
        blackScreen.color = blackScreenCol;


        if (Gmanager.isLastWave && !(BossController == null))
        {
            UpdateBossBars();
        }

        UpdateWaveUI();
        UnityEngine.Rendering.Universal.DepthOfField Dof;
        if (!Gdata.Vol.TryGet(out Dof)) throw new System.NullReferenceException(nameof(Dof));

        if (playerController.Mode == 3)
        {
            Dof.focalLength.value = 60f;
            Vector3 newpos_wui = new Vector3(WeaponUIOriginTransform.x - (2 * (Screen.width / ScreenWidthOrigin)), WeaponUIOriginTransform.y, WeaponUIOriginTransform.z);
            Vector3 newpos_rui = new Vector3(ResourceUIOriginTransform.x - (2 * (Screen.width / ScreenWidthOrigin)), ResourceUIOriginTransform.y, ResourceUIOriginTransform.z);
            Vector3 newpos_waveui = new Vector3(WaveUIOrigin.x, WaveUIOrigin.y + (2 * (Screen.height / ScreenWidthOrigin)), WaveUIOrigin.z);
            WeaponUIRectTransform.position = Vector3.Lerp(WeaponUIRectTransform.position, newpos_wui, Time.deltaTime * 8);
            ResourceUIRectTransform.position = Vector3.Lerp(ResourceUIRectTransform.position, newpos_rui, Time.deltaTime * 8);
            WaveUIRect.position = Vector3.Lerp(WaveUIRect.position, newpos_waveui, Time.deltaTime * 8);

            BaseInteractionUIRect.localPosition = Vector3.Lerp(BaseInteractionUIRect.localPosition, new Vector3(0, 0, 0),Time.deltaTime*8);

            UpdateBaseInteractionUI();
            if (Input.GetKeyDown(Gdata.BuildGuide_TurnLeft))
                ResearchTab_Left();

            if (Input.GetKeyDown(Gdata.BuildGuide_TurnRight))
                ResearchTab_Right();
        }
        else
        {
            Dof.focalLength.value = Mathf.Lerp(Dof.focalLength.value, 0, Time.deltaTime * 2);

            WeaponUIRectTransform.position = Vector3.Lerp(WeaponUIRectTransform.position, WeaponUIOriginTransform, Time.deltaTime * 8);
            ResourceUIRectTransform.position = Vector3.Lerp(ResourceUIRectTransform.position, ResourceUIOriginTransform, Time.deltaTime * 8);
            WaveUIRect.position = Vector3.Lerp(WaveUIRect.position, WaveUIOrigin, Time.deltaTime * 8);

            BaseInteractionUIRect.localPosition = Vector3.Lerp(BaseInteractionUIRect.localPosition, new Vector3(0, -Screen.height * 2, 0), Time.deltaTime * 8);
        }

        CurrentWave.text = "Wave " + Gmanager.CurrentWave;
        RemainingEnemiesText.text = Gmanager.NumOfEnemies.ToString();

        

        if(BuildSystemUIParent.activeInHierarchy)
            BuildSystemUIParent.GetComponent<RectTransform>().rotation = Quaternion.Slerp(BuildSystemUIParent.GetComponent<RectTransform>().rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);

        if(TowerStatusUIParent.activeInHierarchy)
            TowerStatusUIParent.GetComponent<RectTransform>().rotation = Quaternion.Slerp(TowerStatusUIParent.GetComponent<RectTransform>().rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);


        

        ScrapText.text = playerController.Scraps.ToString();
        AdvancedResourcesText.text = playerController.AdvancedResources.ToString();
        EnergyText.text = playerController.BaseEnergies.ToString();


        if (playerController.Mode == 2)
        {
            TowerDataInGuideObject towerData = playerController.Build_Guide_Object.transform.Find(playerController.Selected_Tower.ToString()).GetComponent<TowerDataInGuideObject>();

            BuildSystemUIParent.SetActive(true);
            Cost_TowerName.text = towerData.towerdata.name;

            Cost_TowerIcon.texture = towerData.towerdata.Icon;

            Cost_ScrapsText.text = towerData.towerdata.ScrapCosts[0].ToString();
            Cost_AdvancedResourcesText.text = towerData.towerdata.AdvancedResourcesCosts[0].ToString();
            Cost_EnergiesText.text = towerData.towerdata.EnergyCosts[0].ToString();
            Cost_TimeText.text = towerData.towerdata.UpgradeTime[0].ToString();

            Cost_NotResearchedText.enabled = !towerData.towerdata.Researched;
        }
        else
        {
            BuildSystemUIParent.SetActive(false);
        }


        if (playerController.SelectedTower == null)
            return;
        TowerScript TData = playerController.SelectedTower.GetComponent<TowerScript>();

        

        if (playerController.isRiding)
        {
            if (TData.lastattackedtime > 0)
            {
                Crosshair_TowerCooldownSlider.value = 1f - (TData.lastattackedtime /
                    TData.towerData.fireRate[TData.Level - 1]);
            }
            else
                Crosshair_TowerCooldownSlider.value = 0;
        }
        else
            Crosshair_TowerCooldownSlider.value = 0;

        if (DownedText.enabled && TowerStatusUIParent.activeInHierarchy)
        {
            RemainingTimeToRepair = (int)TData.DownTimer;
            DownedText.text = "Tower is Downed.\nRepairs in "+RemainingTimeToRepair + "\n("+ Gdata.Ride +") "+
                TData.towerData.EnergyCosts[TData.Level-1] * 0.5f + " Energies - Instant Repair";
        }

        if (TowerStatusUIParent.activeInHierarchy)
        {
            HPText.text = "HP : " + TData.towerData.HP[TData.Level - 1] + " / " + TData.HP;

            if (TData.lastattackedtime > 0)
                TowerCooldownSlider.value = 1f - (TData.lastattackedtime / TData.towerData.fireRate[TData.Level - 1]);
            else
                TowerCooldownSlider.value = 0;
        }


    }

    IEnumerator blackScreenOn(float speed, bool isGameEnded)
    {
        for (int i = 0; i < 300; i++)
        {
            yield return new WaitForFixedUpdate();
            blackScreenCol.a += speed;
        }
        blackScreenCol.a = 1;

        if (isGameEnded)
        {
            yield return new WaitForSeconds(0.1f);
            Gmanager.GameEnded();
        }
    }

    IEnumerator blackScreenOff(float speed)
    {
        for (int i = 0; i < 300; i++)
        {
            yield return new WaitForFixedUpdate();
            blackScreenCol.a -= speed;
        }
        blackScreenCol.a = 0;
    }

    public void Update_Crosshair_Spread(float spread)
    {
        spread_lerp = Mathf.Lerp(spread_lerp, spread, Time.deltaTime * 10);
        if(gunScript.isZoomed || playerController.Mode==3)
            crosshair_opacity_lerp = Mathf.Lerp(crosshair_opacity_lerp, 0, Time.deltaTime * 10);
        else
            crosshair_opacity_lerp = Mathf.Lerp(crosshair_opacity_lerp, 1, Time.deltaTime * 10);

        Crosshair_1_Image.color = new Color(Crosshair_1_Image.color.r, Crosshair_1_Image.color.g, Crosshair_1_Image.color.b, crosshair_opacity_lerp);
        Crosshair_2_Image.color = new Color(Crosshair_2_Image.color.r, Crosshair_2_Image.color.g, Crosshair_2_Image.color.b, crosshair_opacity_lerp);
        Crosshair_3_Image.color = new Color(Crosshair_3_Image.color.r, Crosshair_3_Image.color.g, Crosshair_3_Image.color.b, crosshair_opacity_lerp);
        Crosshair_4_Image.color = new Color(Crosshair_4_Image.color.r, Crosshair_4_Image.color.g, Crosshair_4_Image.color.b, crosshair_opacity_lerp);
        Crosshair_SmallDot_Image.color = new Color(Crosshair_SmallDot_Image.color.r, Crosshair_SmallDot_Image.color.g, Crosshair_SmallDot_Image.color.b, crosshair_opacity_lerp);

        Crosshair_1.anchoredPosition = new Vector3(0, spread_lerp * 1500, 0);
        Crosshair_2.anchoredPosition = new Vector3(0, -(spread_lerp * 1500), 0);
        Crosshair_3.anchoredPosition = new Vector3(-(spread_lerp * 1500), 0, 0);
        Crosshair_4.anchoredPosition = new Vector3(spread_lerp * 1500, 0, 0);
    }

    public void ShowTowerStatus(string Name, float HP, float MaxHP, float Damage, float Firerate, float Range, int Level, bool isDowned)
    {
        TowerScript TData = playerController.SelectedTower.GetComponent<TowerScript>();

        ArmorPiercingIcon.enabled = TData.towerData.ArmorPiercing;

        TowerStatusUIParent.SetActive(true);
        NameText.text = Name;
        HPText.text = "HP : " + MaxHP + " / " + HP;
        DamageText.text = "Damage : " + Damage;
        FirerateText.text = "Firerate : " + Firerate;
        RangeText.text = "Range : " + Range;

        if (Level == 5)
        {
            UpgradeText.text = "Maxed";
        }
        else
        {
            UpgradeText.text = "(" + Gdata.Upgrade + ") Upgrade";
        }
        SellText.text = "(" + Gdata.Sell + ") Sell";

        if (isDowned)
        {
            DownedText.enabled = true;
        }
        else
        {
            DownedText.enabled = false;
        }

        TowerIcon.texture = TData.towerData.Icon;
        UpgradeIcon.texture = TData.towerData.UpgradeIcons[Level - 1];

        string info = TData.towerData.UpgradeInfo[Level - 1].Replace("\\n", "\n");
        UpgradeInfoText.text = info;

        UpgradeNameText.text = TData.towerData.UpgradeName[Level - 1];
        if (Level >= 5)
        {
            Upgrade_ScrapsText.text = "Maxed";
            Upgrade_AdvancedResourcesText.text = "Maxed";
            Upgrade_EnergiesText.text = "Maxed";
        }
        else
        {
            Upgrade_ScrapsText.text = TData.towerData.ScrapCosts[Level].ToString();
            Upgrade_AdvancedResourcesText.text = TData.towerData.AdvancedResourcesCosts[Level].ToString();
            Upgrade_EnergiesText.text = TData.towerData.EnergyCosts[Level].ToString();
            Upgrade_TimerText.text = TData.towerData.UpgradeTime[Level].ToString();
        }
        
    }

    public void HideTowerStatus()
    {
        TowerStatusUIParent.SetActive(false);
    }

    public void CrossHairModeChange(int type) //1 : Crosshair, 2 : Dot
    {
        if (type == 1)
        {
            Crosshair_1_Image.enabled = true;
            Crosshair_2_Image.enabled = true;
            Crosshair_3_Image.enabled = true;
            Crosshair_4_Image.enabled = true;

            Crosshair_Dot_Image.enabled = false;
        }
        else if(type==2)
        {
            Crosshair_1_Image.enabled = false;
            Crosshair_2_Image.enabled = false;
            Crosshair_3_Image.enabled = false;
            Crosshair_4_Image.enabled = false;

            Crosshair_Dot_Image.enabled = true;
        }
    }

    public void ShakeUI(float Strength)
    {
        UnityEngine.Rendering.Universal.DepthOfField Dof;
        if (!Gdata.Vol.TryGet(out Dof)) throw new System.NullReferenceException(nameof(Dof));

        //Dof.focalLength = new UnityEngine.Rendering.ClampedFloatParameter {value=60,max=100,min=1,overrideState=false};
        Dof.focalLength.value = 60f;

        WeaponUIParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -Strength);
        ResourceUIParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Strength);

        if (BuildSystemUIParent.activeInHierarchy)
            BuildSystemUIParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Strength);

        if(TowerStatusUIParent.activeInHierarchy)
            TowerStatusUIParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -Strength);
    }

    public void NOTENOUGHENERGY()
    {
        NEETTransparency = 0.5f;
    }
    
    public void NOTENOUGHRESOURCES()
    {
        Debug.Log("NOTENOUGH!!!");
    }

    public void DamageTaken(bool isCritical)
    {
        DamageHitImage_Transparency = 0.5f;
        if (isCritical)
        {
            DamageHitImage.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            DamageHitImage.color = new Color(1, 0, 0);
            CriticalHitImage.color = new Color(1, 0, 0);
            CriticalHitImage_Transparency = 0.25f;
        }
        else
        {
            DamageHitImage.rectTransform.localScale = new Vector3(1, 1, 1);
            DamageHitImage.color = new Color(1, 1, 1);
        }
    }

    public void ResearchTab_Left()
    {
        if (Researching)
            return;

        if(CurrentListNum-1 >= 0)
        {
            CurrentListNum--;
        }
        else
        {
            CurrentListNum = ResearchDataList.Count - 1;
        }
    }

    public void ResearchTab_Right()
    {
        if (Researching)
            return;

        if (CurrentListNum + 1 < ResearchDataList.Count)
        {
            CurrentListNum++;
        }
        else
        {
            CurrentListNum = 0;
        }
    }

    public void Weapon_ResearchTab_Left()
    {
        if (Weapon_Researching)
            return;

        if (Weapon_CurrentListNum - 1 >= 0)
        {
            Weapon_CurrentListNum--;
        }
        else
        {
            Weapon_CurrentListNum = Weapon_ResearchDataList.Count - 1;
        }
    }

    public void Weapon_ResearchTab_Right()
    {
        if (Weapon_Researching)
            return;

        if (Weapon_CurrentListNum + 1 < Weapon_ResearchDataList.Count)
        {
            Weapon_CurrentListNum++;
        }
        else
        {
            Weapon_CurrentListNum = 0;
        }
    }

    public void Research()
    {
        if(playerController.AdvancedResources >= ResearchDataList[CurrentListNum].AdvancedResourceCost &&
            playerController.BaseEnergies >= ResearchDataList[CurrentListNum].EnergyCost && !Researching &&
            !ResearchDataList[CurrentListNum].ResearchingTowerData.Researched)
        {
            playerController.AdvancedResources -= ResearchDataList[CurrentListNum].AdvancedResourceCost;
            playerController.BaseEnergies -= ResearchDataList[CurrentListNum].EnergyCost;
            StartCoroutine(Research_Coroutine(ResearchDataList[CurrentListNum]));
        }
        else
        {
            Debug.Log("NOT ENOUGH!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    IEnumerator Research_Coroutine(ResearchData rData)
    {
        ResearchInProgressText.text = "Research in Progress";
        Researching = true;
        for (float i = 0; i < rData.ResearchTime * 10; i += 1)
        {
            if (i > 0)
                R_TimerSlider.value = (i / 10) / rData.ResearchTime;
            else
                R_TimerSlider.value = 0;
            yield return new WaitForSeconds(0.1f);
        }
        rData.ResearchingTowerData.Researched = true;
        ResearchInProgressText.text = "Ready to Research";
        Researching = false;
    }

    public void Weapon_Research()
    {
        if (playerController.AdvancedResources >= Weapon_ResearchDataList[Weapon_CurrentListNum].ResearchCost_AdvancedResource &&
            playerController.BaseEnergies >= Weapon_ResearchDataList[Weapon_CurrentListNum].ResearchCost_Energy && !Weapon_Researching &&
            !Weapon_ResearchDataList[Weapon_CurrentListNum].Researched)
        {
            playerController.AdvancedResources -= Weapon_ResearchDataList[Weapon_CurrentListNum].ResearchCost_AdvancedResource;
            playerController.BaseEnergies -= Weapon_ResearchDataList[Weapon_CurrentListNum].ResearchCost_Energy;
            StartCoroutine(Weapon_Research_Coroutine(Weapon_ResearchDataList[Weapon_CurrentListNum]));
        }
        else
        {
            Debug.Log("NOT ENOUGH!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    IEnumerator Weapon_Research_Coroutine(GunData gData)
    {
        Weapon_ResearchInProgressText.text = "Research in Progress";
        Weapon_Researching = true;
        for (float i = 0; i < gData.ResearchTime * 10; i += 1)
        {
            if (i > 0)
                Weapon_R_TimerSlider.value = (i / 10) / gData.ResearchTime;
            else
                Weapon_R_TimerSlider.value = 0;
            yield return new WaitForSeconds(0.1f);
        }
        gData.Researched = true;
        Weapon_ResearchInProgressText.text = "Ready to Research";
        Weapon_Researching = false;
    }

    public void UpdateBaseInteractionUI()
    {
        BI_ScrapText.text = playerController.Scraps.ToString();
        BI_AdvancedResourceText.text = playerController.AdvancedResources.ToString();
        BI_EnergyText.text = playerController.BaseEnergies.ToString();

        UpdateResearchTab(ResearchDataList[CurrentListNum]);
        UpdateWeaponResearchTab(Weapon_ResearchDataList[Weapon_CurrentListNum]);
        UpdateBaseInfoTab();
        UpdatePlayerResourceTab();

        if (GeneratingAdRe)
        {
            AdReSynth_GeneratingTextObject.SetActive(true);
        }
        else
        {
            AdReSynth_GeneratingTextObject.SetActive(false);
            AdReSynth_TimerSlider.value = Mathf.Lerp(AdReSynth_TimerSlider.value, 0, Time.deltaTime * 5);
        }
    }

    public void UpdateResearchTab(ResearchData rData)
    {
        R_AdvancedResourceText.text = rData.AdvancedResourceCost.ToString();
        R_EnergyText.text = rData.EnergyCost.ToString();
        R_TimerText.text = rData.ResearchTime.ToString();
        AlreadyResearchedTextObject.SetActive(rData.ResearchingTowerData.Researched);
        R_Name.text = rData.Name.ToString();
        R_Icon.texture = rData.ResearchingTowerData.Icon;

        R_ArmorPiercingIcon.enabled = rData.ResearchingTowerData.ArmorPiercing;

        if (!Researching)
            R_TimerSlider.value = Mathf.Lerp(R_TimerSlider.value, 0, Time.deltaTime * 5);
    }

    public void UpdateWeaponResearchTab(GunData gData)
    {
        Weapon_R_AdvancedResourceText.text = gData.ResearchCost_AdvancedResource.ToString();
        Weapon_R_EnergyText.text = gData.ResearchCost_Energy.ToString();
        Weapon_R_TimerText.text = gData.ResearchTime.ToString();
        Weapon_AlreadyResearchedTextObject.SetActive(gData.Researched);
        Weapon_R_Name.text = gData.WeaponName.ToString();
        Weapon_R_Icon.texture = gData.WeaponIcon;

        if (!Weapon_Researching)
            Weapon_R_TimerSlider.value = Mathf.Lerp(Weapon_R_TimerSlider.value, 0, Time.deltaTime * 5);
    }

    public void UpdateBaseInfoTab()
    {
        if(BaseData.HP > 0)
        {
            BaseHPSlider.value = BaseData.HP / BaseData.MaxHP;
        }
        else
        {
            BaseHPSlider.value = 0;
        }

        BaseHPText.text = "Base HP : " + BaseData.HP + " / " + BaseData.MaxHP;
        EnergyGeneratesPerSecText.text = "Generates per Second : " + BaseData.EnergyIncomePerSecond;
        RepairCostText.text = BaseData.Cost_Repair[BaseData.NumOfRepair].ToString();
        if (BaseData.NumOfUpgradedGen >= BaseData.Cost_GeneratePerSecond.Count)
            return;
        UpgradeGenCostText.text = BaseData.Cost_GeneratePerSecond[BaseData.NumOfUpgradedGen].ToString();
    }

    public void UpdatePlayerResourceTab()
    {
        WeaponEnergiesText.text = playerController.Energy + " / " + playerController.MaxEnergy;
        ShieldsText.text = playerController.Shield + " / " + playerController.MaxShield;

        if (BaseData.NumOfUpgradedWeaponEnergy < BaseData.Cost_AdRe_UpgradeWeaponEnergy.Count)
            WeaponEnergyUpgradeCostText.text = BaseData.Cost_AdRe_UpgradeWeaponEnergy[BaseData.NumOfUpgradedWeaponEnergy].ToString();
        else
            WeaponEnergyUpgradeCostText.text = "Max";

        if (BaseData.NumOfUpgradedShield < BaseData.Cost_AdRe_UpgradeShield.Count)
            ShieldUpgradeCostText.text = BaseData.Cost_AdRe_UpgradeShield[BaseData.NumOfUpgradedShield].ToString();
        else
            ShieldUpgradeCostText.text = "Max";

        EnergyCost_WeaponEnergyRecharge.text = Mathf.Abs(playerController.Energy - playerController.MaxEnergy).ToString();
        EnergyCost_ShieldRecharge.text = Mathf.Abs(playerController.Shield - playerController.MaxShield).ToString();
    }

    public void AdvancedResourceGenerate()
    {
        if(playerController.Scraps >= BaseData.AdReSynthCost_Scraps && playerController.BaseEnergies >= BaseData.AdReSynthCost_Energies && !GeneratingAdRe)
        {
            StartCoroutine(AdvancedResourceGenerate_Coroutine(BaseData.AdReSynthCost_Time));
        }
        else
        {
            Debug.Log("Not Enough Resources to do this.");
        }
    }

    IEnumerator AdvancedResourceGenerate_Coroutine(float t)
    {
        playerController.Scraps -= BaseData.AdReSynthCost_Scraps;
        playerController.BaseEnergies -= BaseData.AdReSynthCost_Energies;
        GeneratingAdRe = true;
        for (float i = 0; i < t * 10; i += 1)
        {
            if (i > 0)
                AdReSynth_TimerSlider.value = (i / 10) / t;
            else
                AdReSynth_TimerSlider.value = 0;
            yield return new WaitForSeconds(0.1f);
        }
        GeneratingAdRe = false;
        playerController.AdvancedResources++;
    }

    public void SelectWeapon()
    {
        if (Weapon_ResearchDataList[Weapon_CurrentListNum].Researched)
        {
            gunScript.ChangeWeapon(Weapon_CurrentListNum+1);
        }
    }

    void UpdateWaveUI()
    {
        WaveStoppedText.enabled = !Gmanager.isWave;

        CurrentWave.text = "Wave " + Gmanager.CurrentWave;
        RemainingEnemiesText.text = Gmanager.NumOfEnemies.ToString();

        if (!Gmanager.isWave)
        {
            WaveFreeTimeSlider.value = Gmanager.waveFreeTimeTimer / Gmanager.WaveFreeTime;
        }
        else
        {
            WaveFreeTimeSlider.value = 0;
        }
    }

    public void ShowObjectInteractionUI(string name, float value)
    {
        InteractionUIParent.SetActive(true);
        OI_Timer.value = value;
        OI_Text.text = "Interact - " + name;  
    }

    public void HideObjectInteractionUI()
    {
        InteractionUIParent.SetActive(false);
    }

    public void BossWaveStarted(GameObject g)
    {
        BossBarParent.SetActive(true);
        BossObject = g;
        BossController = BossObject.GetComponent<EnemyController>();
    }

    public void UpdateBossBars()
    {
        BossBarText.text = BossController.current_HP.ToString();
        if(BossController.current_HP != 0)
            BossBarSlider.value = Mathf.Lerp(BossBarSlider.value, BossController.current_HP / BossController.HP, 0.1f);
    }

    public void BlurOn()
    {
        UnityEngine.Rendering.Universal.DepthOfField Dof;
        if (!Gdata.Vol.TryGet(out Dof)) throw new System.NullReferenceException(nameof(Dof));

        Dof.focalLength.value = 60f;
    }
    public void Died()
    {
        isDead = true;
        StartCoroutine(blackScreenOn(0.01f, true));
    }
}
