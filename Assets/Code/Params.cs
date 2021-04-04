using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Assets.Code
{
    /*
     * Chuck any parameter values in here, to be referenced via world
     */
    //[Serializable,HideInInspector]
    public class Params
    {
        public int option_edgeScroll = 1;
        public int option_useAdvancedGraphics = 1;
        public int mapGen_sizeX = 32;
        public int mapGen_sizeY = 24;
        public double mapGen_proportionOfMapForHumans = 0.6;
        public double mapGen_minHabitabilityForHumans = 0.15;
        public int mapGen_stepsPerIsland = 12;
        public int mapGen_maxBrushSize = 5;
        public int mapGen_burnInSteps = 150;
        public double map_tempTemperatureReversion = 0.001f;
        public double map_coastalHabBonus = 0.1;

        public int useLightbringer = 0;
        public int useAwareness = 1;
        public int usePaladins = 0;
        public bool flashEnthrallables = true;

        public double panic_panicPerPower = 0.0025;
        public double panic_dropPerTurn = 0.003;
        public double panic_canInvestigate = 0.1;
        public double panic_letterWritingLevel = 0.25;
        public double panic_letterWritingToAllLevel = 0.35;
        public double panic_researchAtUniWithoutAwareness = 0.35;
        public double panic_cleanseSoulLevel = 0.25;
        public double panic_panicAtFullExtinction = 3;
        public double panic_panicAtFullShadow = 1.4;
        public double panic_canAlly = 0.5;
        public double panic_fromClueFound = 0.01;
        public double panic_paladinSpawn_1 = 0.15;
        public double panic_paladinSpawn_2 = 0.5;
        public double panic_lightbringerRitualPermitted = 0.85;
        public double panic_canCallNobleCrisis = 0.25;

        public double awareness_canProposeLightAlliance = 1;
        public double awareness_master_speed = 1;
        public double awarenessBaseWeighting = 1;
        public double awareness_cleanseSoulLevel = 0.3;
        public double awarenessUniversityBonusMult = 2;
        public double awareness_letterWritingLevel = 0.25;
        public int awareness_letterWritingInterval = 12;
        public double awareness_letterWritingAwarenessGain = 0.35;
        public double awarenessInvestigationDetectMult = 4;
        public double awareness_increasePerCost = 0.015;
        public double awareness_canInvestigate = 0.1;
        public double awareness_decay = 0.002;
        public int awareness_turnsForLightRitual = 49;
        public int awareness_minLocsForLightbringer = 3;
        public int awareness_lightbringerMaxLocs = 4;
        public double awarenessCanCallNobleCrisis  = 0.5;
        public int awarenessMinNobleCrisisPeriod = 32;
        public int awareness_simplePaladinGracePeriod = 40;

        public double action_research_expectedAwarenessPerTurn = 0.15;
        public double action_research_pSanityHit = 0.5;
        public int action_letterWritingTurns = 5;
        public int action_investigateTurns = 7;
        public int action_cleanseSoulTurns = 7;
        public double action_cleanseSoulAmount = 0.1;
        public double action_investigateEvidence = 0.2;

        public int overmind_allowDirectEnthralling = 1;
        public int overmind_maxPower = 24;
        public double overmind_powerRegen = 0.75;
        public bool overmind_singleAbilityPerTurn = true;
        public int overmind_enthrallmentUseRegainPeriod = 32;
        public int overmind_maxEnthralled = 2;

        public double econ_multFromBuff = 1.2;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised

        public double relObj_suspicionLimiterMult = 2.5;
        public double relObj_defaultLiking = 0;
        public double relObj_decayRate = 0.97;
        public double relObj_decayRateSlow = 0.99;

        public double combat_prestigeLossFromConquest = 0.0;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.2;
        public double combat_lethalityDefensive = 0.1;
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land
        public double combat_maxMilitaryCapExponent = 0.82;//Used to reduce the power of larger nations
        public double combat_defensivePostureDmgMult = 0.666;

        public int war_defaultLength = 16;
        public double minInformationAvailability = 0.2;

        public double utility_econEffect = 0.75;
        public double utility_econEffectOther = 0.05;
        public double utility_militaryTargetRelStrengthOffensive = 350;
        public double utility_militaryTargetCompleteProvince = 80;
        public double utility_militaryTargetExpansion = 150;
        public double utility_militaryOverexpansionHalt = 225;
        public double utility_militaryTargetRelStrengthDefensive = 300;
        public double utility_vassaliseReluctance = -100;
        public double utility_vassaliseMilMult = 80;
        public double utility_vassaliseThreatMult = 0.75;
        public double utility_lightAllianceMult = 200;
        public double utility_lightAllianceMilMult = 2;
        public double utility_lightAlliancSusMult = 50;
        public double utility_introversionFromInstability = 130;
        public double utility_militarism = 100;
        public double utility_landedTitleMult = 0.08;
        public double utility_unlandedTitleMult = 0.08;
        public double utility_introversionFromSuspicion = 3;
        public double utility_killSuspectFromSuspicion = 200;
        public double utility_exileSuspectFromSuspicion = 300;
        public double utility_exileSuspectRelucatance = 25;
        public double utility_killSuspectRelucatance = 75;
        public double utility_wouldBeOutvotedMult = 0.25;
        public double utility_landedTitleAssignBaseline = 75;
        public double utility_prestigeMultForTitle = 1.5;
        public double utility_dismissFromCourt = 2;
        public double utility_fromThreat = 1.8;
        public double utility_greatestThreatDelta = 25;
        public double utility_defendEvidenceProvince = 20;
        public double utility_agentDefendProvinceLikingMult = 0.18;
        public double utility_defendHomelandVSAgents = 20;
        public double utility_evidenceResonseBaseline = 30;
        public double utility_extremeismScaling = 2;
        public double utility_swapAgentRolesMult = 100;
        public double utility_selfInterestFromThreat = -0.5;
        public double utility_plagueResponseMultPerRiskItem = 0.5;
        public double utility_singleUseCure = 0.66;
        public double utility_lightbringerRitual = 150;
        public double utility_lightbringerRitualReluctance = -50;
        public double utility_corruptHatesInquisitor = -75;
        public double utility_honorableHatesWitchHunt = -75;
        public double utility_warlikeLikesWitchHunt = 50;

        public double person_likingFromBeingInvaded = -30;
        public double person_maxLikingGainFromVoteAccordance = 25;
        public double person_maxLikingLossFromVoteDiscord = -30;
        public double person_prestigeDeltaPerTurn = 0.5;
        public double person_threatMult = 100;
        public double person_defaultPrestige = 5;
        public double person_evidencePerShadow = 0.015;
        public double person_evidenceExponent = 1.5;
        public double person_evidenceReduceEnthralled = 0.45;
        public double person_suspicionPerEvidence = 0.1;
        public double person_dislikingFromSuspicion = -200;
        public double person_shadowContagionMult = 0.05;
        public double person_threatFromSuspicion = 400;
        public double person_shadowDecayPerTurn = 0.005;
        public double person_threatFromBeingOffensiveTarget = 0.5;
        public int person_fearLevel_afraid = 100;
        public int person_fearLevel_terrified = 150;
        public double person_dislikeFromNotBeingVotedFor = -30;
        public double person_votingRelChangePerUtilityPositive = 0.1;//If benefitted by a vote
        public double person_votingRelChangePerUtilityNegative = 0.9;//If harmed by a vote

        public int econ_buffDuration = 45;

        public int soc_untitledPeople = 3;
        public int society_votingDuration = 1;
        public int society_instablityTillRebellion = 12;
        public int society_rebelLikingThreshold = -25;
        public int society_zeitDuration = 3;
        public double society_sovereignPrestige = 20;
        public double society_dukePrestige = 10;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 40;
        public int society_nPeopleForEmpire = 21;
        public int society_nPeopleForKingdom = 12;
        public int society_maxDukes = 3;
        public double society_introversionStabilityGain = 1.2;
        public double society_votingEconHiddenBiasMult = 1.5;
        public int society_billsBetweenLandAssignments = 3;
        public double society_pExpandIntoEmpty = 0.1;
        public double society_prestigeFromVotingFailure = 0;
        public double society_prestigeFromVotingSuccess =  0;
        public double society_dislikeFromFailedProposal = 0;
        public double society_houseLiking = 15;
        public int society_securityBuffDuration = 12;
        public int society_quarantineDuration = 12;
        public int society_medAidDuration = 12;
        public int society_securityBuffMajor = 4;
        public int society_securityBuffMinor = 1;
        public int society_lockdownPrestigeHit = -10;
        public int society_lockdownBuffDuration = 12;
        public double society_electoralPrestigeWeight = 2;//Comparing prestige to liking. If we want 50 prestige to equal 100 liking, go for 2
        public int society_prestigeFromPlagueRedDeath = 0;
        public int society_crisis_plagueCrisisCooldown = 10;
        public int society_cooldownGeneralVoting = 18;

        public double temporaryThreatDecay = 0.95;
        public double threat_takeLocation = 56;
        public double threat_temporaryDreadDecay = 0.98;
        //public double threat_evidencePresented = 25;
        public double threat_evidencePresentedFallingOff = 40;
        public double threat_agentFearDecayPerTurn = 2;
        public int threat_smallSettlementVsDisease = 50;
        public double threat_dreadFromNobleInvestigation = 66;
        public double victory_targetEnshadowmentAvrg = 0.75;

        public double dark_evilThreatMult = 2;
        public double dark_fleshThreatMult = 2;
        public double dark_fishmanStartingThreatMult = -0.25;


        public int ability_enthrallUnitCost = 0;
        public int ability_disruptAgentCost = 8;

        public int ability_uncannyGlamourEvidence = 10;
        public int ability_uncannyGlamourGain = 15;
        public int ability_uncannyGlamourCost = 10;
        public int ability_uncannyGlamourCooldown = 15;
        public int ability_sowDissentCost = 12;
        public int ability_hateTheLightCost = 8;
        public int ability_hateTheLightMult = 20;
        public int ability_sowDissentLikingChange = -25;

        public int ability_shareTruthCooldown = 10;
        public double ability_avrgDarkEmpireShadowPerTurn = 0.075;
        public int ability_denounceOtherCooldown = 32;
        public int ability_proposeVoteCooldown = 15;
        public double ability_growFleshThreatAdd = 5;
        public int ability_shareEvidenceLikingCost = 20;
        public int ability_switchVoteLikingCost = 15;
        public int ability_changePoliticsLikingCost = 15;
        public double ability_shareEvidencePercentage = 0.75;
        public int ability_enshadowCost = 4;
        public int ability_reduceEvidenceCost = 10;
        public int ability_militaryAidDur = 20;
        public int ability_militaryAidAmount = 5;
        public int ability_militaryAidCost = 2;
        public int ability_fishmanRaidCost = 5;
        public int ability_fishmanRaidMilAdd = 4;
        public int ability_fishmanRaidTemporaryThreat = 20;
        public int ability_fishmanLairCost = 8;
        public int ability_fishmanCultOfTheDeep = 7;
        public int ability_boycottVoteCost = 15;
        public int ability_cancelVoteCost = 10;
        public int ability_shortMemoriesCost = 5;
        public int ability_fleshScreamThreatAdd = 10;
        public int ability_fleshScreamCost = 7;
        public int ability_fleshScreamSanity = 4;
        public int ability_fleshGrowCost = 5;
        public int ability_fleshSeedCost = 12;
        public int ability_informationBlackoutCost = 4;
        public int ability_informationBlackoutDuration = 15;
        public int ability_FishmanCultDuration = 15;
        public int ability_FishmanMadnessDuration = 15;
        public int ability_fishmanCultMilRegen = 2;
        public double ability_fishmanCultTempThreat = 2;
        public double ability_fishmanCultDread = 6;
        public int ability_trustingFoolCost = 5;
        public int ability_trustingFoolCooldown = 25;
        public int ability_fearmongerTempThreat = 25;
        public int ability_fearmongerCooldown = 15;
        public int ability_apoptosisCost = 15;
        public int ability_darkEmpireCost = 12;
        public double ability_darkEmpireThreatMultGain = 0.5;
        public int ability_sharedGloryLikingGain = 16;
        public int ability_addLikingAmount = 20;
        public int ability_addLikingCost = 4;
        public int ability_reduceSuspicionCost = 5;
        public int ability_sharedGloryAmount = 5;
        public int ability_auraOfLunacyEvidence = 14;
        public int ability_auraOfLunacyHit = 7;
        public int ability_auraOfLunacyCost = 7;
        public double ability_fishmanInfiltrate = 0.02;
        public int ability_fishmanHauntingSongCost = 7;
        public int ability_fishmanHauntingSongHit = 5;
        public int ability_fishmanHauntingAbyssalSirensCost = 14;
        public double ability_fishmanHauntingAbyssalSirensShadowPerTurn = 0.02;
        public int ability_fishmanHauntingAbyssalSirensDur = 50;
        public double ability_provincialSentimentLikingChangePositive = 10;
        public double ability_provincialSentimentLikingChangeNegative = -20;
        public int ability_provincialSentimentLikingChangeCooldown = 20;
        public double ability_coldAsDeathTempChange = -0.15;
        public double ability_iceBloodTempChange = -0.25;
        public double ability_deathOfTheSunTempChange = -0.008;
        public int ability_coldAsDeathCost = 5;
        public int ability_coldAsDeathCooldown = 10;
        public int ability_iceBloodCost = 10;
        public int ability_iceBloodCooldown = 5;
        public int ability_blizzardCost = 0;
        public int ability_blizzardCooldown = 0;
        public int ability_blizzardDuration = 12;
        public int ability_deathOfTheSunCost = 0;
        public int ability_runawayCatastropheCost = 5;
        public int ability_runawayCatastropheCooldown = 10;
        public double ability_runawayCatastrophyTempChange = -0.05;
        public double ability_runawayCatastropheRequirement = -0.12;
        public int ability_deathOfTheSunCooldown = 10;
        public double ability_denounceLeaderLikingMult = 0.5;
        public double ability_denounceLeaderMax = 35;
        public int ability_denounceLeaderCooldown = 10;
        public int ability_instillDreadCooldown = 5;
        public int ability_instillDreadCost = 5;
        public int ability_callToViolenceCooldown = 5;
        public int ability_breakMindCooldown = 21;
        public int ability_spreadFearCooldown = 5;
        public double ability_instillDreadMult = 0.33;
        public int ability_breakMindMaxSanity = 7;
        public int ability_polariseByFearCooldown = 20;
        public double ability_polariseByFearMult = 0.5;
        public int ability_denouncePacifistsCooldown = 15;
        public double ability_denouncePacisfistsLiking = -25;
        public int ability_delayVoteTurnsAdded = 4;
        public int ability_delayVoteCost = 8;
        public int ability_delayVoteCooldown = 24;
        public int ability_disruptActionCost = 3;
        public int ability_disruptActionDuration = 7;
        public int ability_unit_bloodCostInfiltrate = 7;
        public int ability_unit_bloodCostInsanity = 7;
        public double ability_unit_insanityInfiltrationReq = 0.2;
        public int ability_enthrallUnitCooldown = 0;
        public int ability_createUnitCooldown = 64;
        public int ability_fleshWithdrawBonus = 10;

        public int ability_unit_falseAccusationCharges = 5;
        public double ability_unit_falseAccusationEffect = 0.25;
        public double ability_unit_enthrallNobleReq = 0.9;
        public double ability_unit_breakNobleReq = 0.66;
        public double ability_unit_spreadShadowInfiltrationReq = 0.5;
        public int ability_unit_spreadShadowMinLiking = 35;
        public double ability_unit_falseEvidenceInfiltration = 0.4;
        public int ability_redDeath_originCooldown = 32;
        public int ability_redDeath_cureCooldown = 8;

        public int ability_fog_wellOfFogCost = 8;
        public int ability_fog_trapDuration = 16;
        public int ability_fog_trapCost = 3;
        public int fog_securityDecrease = -3;
        public double fog_suspicionIncreaseMult = 0.3;
        public int ability_fog_convocationCost = 1;
        public int ability_fog_convocationCooldown = 6;

        public double insanity_sanityRegen = 0.1;
        public int insanity_nParanoiaTargets = 4;
        public int insanity_relHit = -10;
        public double insanity_lashOutProbability = 0.07;
        public int insanity_maxSanity = 14;

        public double trait_incautious = 0.5;
        public double trait_incautious_awarenessMult = 0.33;
        public double trait_aware = 2;
        public double trait_charismatic = 15;
        public double trait_unlikable = -15;
        public double trait_hateful = -25;
        public double trait_warmaster = 10;
        public double trait_defender = 10;
        public double trait_badCommander = -10;
        public double trait_incompetent_loss = 5;
        public double trait_competent_gain = 5;
        public double trait_incompetent_desirability = -75;
        public double trait_competent_desirability = 75;
        public double trait_basic_desirability = 30;
        public double trait_basic_undesirability = -30;
        public double trait_aware_awarenessMult = 1.5;

        public double city_popMaxPerHabilitability = 50;
        public double city_popMaxPerHabilitabilityMinor = 25;
        public double city_infraDmg = 5;
        public int city_level_metropole = 45;
        public int city_level_city = 35;
        public int city_level_town = 25;
        public int city_level_village = 18;
        public int city_popMin = 12;

        public double temporaryThreatConversion = 0.01;
        public int war_battleDeadDur = 4;
        public int combat_popDamageMax = 5;
        public int combat_infraDamageMax = 10;

        public int voting_maxLocationsForVassalisation = 7;
        public int voting_powerToSwitchMin = 4;
        public int voting_likingToSwitchMin = 5;
        public double voting_powerToSwitchPerU = 0.2;
        public double voting_likingToSwitchPerU = 0.25;

        public double unit_recruitEvidence = 0.1;
        public int unit_investigatorsSeeEnthralled = 1;
        public double unit_investigatorsPerProvince = 0.5;
        public int unit_falseEvidenceCooldown = 8;
        public int unit_disruptDuration = 5;
        public int unit_disruptCooldown = 10;
        public double unit_targetUnitsPerLoc = 0.1;
        public int unit_investigateTime = 3;
        public int unit_investigateNobleTime = 3;
        public double unit_investigateNobleEvidenceGain = 0.3;
        public int unit_investigateNobleCooldown = 16;
        public int unit_shareSuspicionTime = 1;
        public int unit_socialiseAtCourtTime = 3;
        public int unit_socialiseAtCourtGain = 30;
        public int unit_pleadCaseTime = 3;
        public int unit_infiltrateTime = 6;
        public int unit_infiltrateWeakTime = 8;
        public double unit_infiltrateAmount = 1.5;
        public double unit_divisorOffset = 2;
        public double unit_majorEvidence = 0.25;
        public double unit_minorEvidence = 0.1;
        public int unit_spreadShadowTime = 5;
        public int unit_spreadShadowWeakTime = 7;
        public double unit_spreadShadowAmount = 0.6;
        public double unit_spreadShadowWeakAmount = 0.25;
        public double unit_suspicionFromProximity = 0.1;
        public int unit_armyOfDeadMaxHP = 37;
        public int unit_paladin_trackDuration = 6;
        public int unit_inv_trackDuration = 6;
        public int unit_paladin_promotionDuration = 27;
        public int unit_newIdentityTime = 10;
        public int unit_promoteCooldown = 32;
        public int unit_establishNewSettlementTime = 8;
        public double unit_merchantChanceToExpandIntoNeighbouring = 0.25;
        public double unit_merchantsPerCity = 0.125;
        public int unit_merchant_corruptGoldCost = 35;
        public double unit_armyHPMult = 1;


        public int unit_seeker_nReqSecrets = 12;
        public int unit_seeker_nCreatedSecrets = 8;
        public int unit_seeker_uncoverTime = 5;
        public double unit_seeker_libraryInfiltrationReq = 0.25;
        public double unit_vamp_infiltrateAmount = 1.6;
        public double unit_doctor_legalInfiltrationReq = 0.1;
        public int unit_doctor_lootBodiesAmount = 12;
        public int unit_doctor_emptyGravesDuration = 16;
        public int unit_doctor_sowCorpserootCooldown = 5;
        public int unit_vampire_maxBlood = 37;
        public int unit_vampire_drinkReqLiking = 0;
        public double unit_seeker_truthShadow = 0.35;
        public int unit_headless_pumpkinDur = 9;
        public double unit_knightCombatBonus = 1.5;
        public double unit_agentCombatBonus = 1.25;
        public double flesh_armyStrength = 12;
        public int unit_heir_saccPowerGain = 17;

        public int unit_rd_redDeathPlagueDur = 20;
        public double unit_rd_redDeathPlaguePSpread = 0.05;
        public int unit_rd_redDeathPlagueImmunityDur = 16;
        public int unit_rd_redDeathMilCapHit = -10;

        public void saveToFile()
        {
            StreamWriter writer = new StreamWriter(".\\params.txt");

            FieldInfo[] fields = this.GetType().GetFields();
            for (int i = 0; i != fields.Length; ++i)
            {
                writer.Write(fields[i].Name);
                writer.Write(": ");
                writer.WriteLine(fields[i].GetValue(this));
            }

            writer.Close();
        }

        public void loadFromFile()
        {
            string[] fields;
            try
            {
                fields = System.IO.File.ReadAllLines(".\\params.txt");
            }
            catch (Exception e)
            {
                World.log("poop");
                return;
            }

            for (int i = 0; i != fields.Length; ++i)
            {
                try {
                    string[] parts = fields[i].Split(':');
                    if (parts.Length != 2) { continue; }

                    FieldInfo field = this.GetType().GetField(parts[0].Trim());
                    if (field == null) { continue; }

                    if (field.FieldType == typeof(int))
                    {
                        int value = int.Parse(parts[1].Trim());
                        field.SetValue(this, value);
                    }
                    if (field.FieldType == typeof(bool))
                    {
                        bool value = bool.Parse(parts[1].Trim());
                        field.SetValue(this, value);
                    }
                    if (field.FieldType == typeof(double))
                    {
                        double value = double.Parse(parts[1].Trim());
                        field.SetValue(this, value);
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }
    }
}
