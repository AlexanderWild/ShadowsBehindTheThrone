using System;
using System.IO;
using System.Reflection;

namespace Assets.Code
{
    /*
     * Chuck any parameter values in here, to be referenced via world
     */
    //[Serializable,HideInInspector]
    public class Params
    {
        public int option_edgeScroll = 1;
        public int mapGen_sizeX = 32;
        public int mapGen_sizeY = 24;
        public double mapGen_proportionOfMapForHumans = 0.65;
        public double mapGen_minHabitabilityForHumans = 0.15;
        public int mapGen_stepsPerIsland = 12;
        public int mapGen_maxBrushSize = 5;
        public int mapGen_burnInSteps = 150;
        public float map_tempTemperatureReversion = 0.002f;

        public int useAwareness = 1;

        public double panic_panicPerPower = 0.0025;
        public double panic_dropPerTurn = 0.001;
        public double panic_canInvestigate = 0;
        public double panic_letterWritingLevel = 0.15;
        public double panic_letterWritingToAllLevel = 0.25;
        public double panic_researchAtUniWithoutAwareness = 0.35;
        public double panic_cleanseSoulLevel = 0.25;
        public double panic_panicAtFullExtinction = 2;
        public double panic_panicAtFullShadow = 1.75;
        public double panic_canAlly = 0.5;

        public double awareness_canProposeLightAlliance = 1;
        public double awareness_master_speed = 1;
        public double awarenessBaseWeighting = 1;
        public double awareness_cleanseSoulLevel = 0.3;
        public double awarenessUniversityBonusMult = 2;
        public double awareness_letterWritingLevel = 0.25;
        public int awareness_letterWritingInterval = 4;
        public double awareness_letterWritingAwarenessGain = 0.35;
        public double awarenessInvestigationDetectMult = 4;
        public double awareness_increasePerCost = 0.02;
        public double awareness_canInvestigate = 0.1;
        public double action_research_expectedAwarenessPerTurn = 0.15;
        public double action_research_pSanityHit = 0.5;
        public double awareness_decay = 0.002;

        public int action_letterWritingTurns = 5;
        public int action_investigateTurns = 7;
        public int action_cleanseSoulTurns = 7;
        public double action_cleanseSoulAmount = 0.1;
        public double action_investigateEvidence = 0.25;

        public bool flashEnthrallables = true;
        public int overmind_maxPower = 24;
        public double overmind_powerRegen = 1.0;
        public bool overmind_singleAbilityPerTurn = true;

        public double econ_multFromBuff = 1.33;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        
        public double relObj_defaultLiking = 5;
        public double relObj_decayRate = 0.98;

        public double combat_prestigeLossFromConquest = 0.0;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.5;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land
        public double combat_maxMilitaryCapExponent = 0.82;//Used to reduce the power of larger nations
        public double combat_defensivePostureDmgMult = 0.666;

        public int war_defaultLength = 16;
        public double minInformationAvailability = 0.2;

        public double utility_econEffect = 0.5;
        public double utility_econEffectOther = 0.10;
        public double utility_militaryTargetRelStrengthOffensive = 300;
        public double utility_militaryTargetCompleteProvince = 50;
        public double utility_militaryTargetExpansion = 60;
        public double utility_militaryTargetRelStrengthDefensive = 250;
        public double utility_vassaliseReluctance = -90;
        public double utility_vassaliseMilMult = 80;
        public double utility_vassaliseThreatMult = 0.75;
        public double utility_lightAllianceMult = 200;
        public double utility_lightAllianceMilMult = 2;
        public double utility_lightAlliancSusMult = 50;
        public double utility_introversionFromInstability = 50;
        public double utility_militarism = 100;
        public double utility_landedTitleMult = 0.2;
        public double utility_unlandedTitleMult = 0.1;
        public double utility_introversionFromSuspicion = 3;
        public double utility_killSuspectFromSuspicion = 350;
        public double utility_killSuspectRelucatance = 66;
        public double utility_wouldBeOutvotedMult = 0.25;
        public double utility_landedTitleAssignBaseline = 100;
        public double utility_prestigeMultForTitle = 1.5;
        public double utility_dismissFromCourt = 2;

        public double person_likingFromBeingInvaded = -25;
        public double person_maxLikingGainFromVoteAccordance = 30;
        public double person_maxLikingLossFromVoteDiscord = -50;
        public double person_prestigeDeltaPerTurn = 0.5;
        public double person_threatMult = 100;
        public double person_defaultPrestige = 5;
        public double person_evidencePerShadow = 0.025;
        public double person_evidenceExponent = 1.5;
        public double person_evidenceReduceEnthralled = 0.66;
        public double person_suspicionPerEvidence = 0.1;
        public double person_dislikingFromSuspicion = -200;
        public double person_shadowContagionMult = 0.05;
        public double person_threatFromSuspicion = 400;
        public double person_shadowDecayPerTurn = 0.005;
        public double person_threatFromBeingOffensiveTarget = 75;
        public int person_fearLevel_afraid = 100;
        public int person_fearLevel_terrified = 150;

        public int econ_buffDuration = 50;

        public int soc_untitledPeople = 3;
        public int society_votingDuration = 2;
        public double society_votingRelChangePerUtilityPositive = 0.085;//If benefitted by a vote
        public double society_votingRelChangePerUtilityNegative = 0.14;//If harmed by a vote
        public int society_instablityTillRebellion = 10;
        public int society_rebelLikingThreshold = -5;
        public int society_zeitDuration = 3;
        public double society_sovreignPrestige = 15;
        public double society_dukePrestige = 10;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 40;
        public int society_nPeopleForEmpire = 21;
        public int society_nPeopleForKingdom = 12;
        public int society_maxDukes = 4;
        public double society_introversionStabilityGain = 1.2;
        public double society_votingEconHiddenBiasMult = 1.5;
        public int society_billsBetweenLandAssignments = 3;
        public double society_pExpandIntoEmpty = 0.1;

        public double temporaryThreatDecay = 0.95;
        public double threat_takeLocation = 5;
        public double victory_targetEnshadowmentAvrg = 0.75;

        public double dark_evilThreatMult = 2;
        public double dark_fleshThreatMult = 2;
        public double dark_fishmanStartingThreatMult = -0.25;
        
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
        public int ability_proposeVoteCooldown = 7;
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
        public double threat_temporaryDreadDecay = 0.97;
        public int ability_informationBlackoutCost = 4;
        public int ability_informationBlackoutDuration = 15;
        public int ability_FishmanCultDuration = 15;
        public int ability_FishmanMadnessDuration = 15;
        public double ability_fishmanCultMilRegen = 0.5;
        public double ability_fishmanCultTempThreat = 1;
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
        public int ability_fishmanHauntingSongCost = 7;
        public int ability_fishmanHauntingSongHit = 5;
        public int ability_fishmanHauntingAbyssalSirensCost = 14;
        public double ability_fishmanHauntingAbyssalSirensShadowPerTurn = 0.02;
        public int ability_fishmanHauntingAbyssalSirensDur = 50;
        public double ability_provincialSentimentLikingChangePositive = 10;
        public double ability_provincialSentimentLikingChangeNegative = -20;
        public int ability_provincialSentimentLikingChangeCooldown = 20;
        public double ability_coldAsDeathTempChange = -0.15;
        public double ability_deathOfTheSunTempChange = -0.003;
        public int ability_coldAsDeathCost = 5;
        public int ability_coldAsDeathCooldown = 10;
        public int ability_deathOfTheSunCost = 0;
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

        public double city_popMaxPerHabilitability = 100;
        public double city_popDmg = 5;
        public double city_infraDmg = 5;

        public int city_level_metropole = 90;
        public int city_level_city = 60;
        public int city_level_town = 40;
        public int city_level_village = 20;
        public double temporaryThreatConversion = 0.01;
        public int war_battleDeadDur = 4;
        public int combat_popDamageMax = 10;
        public int combat_infraDamageMax = 10;

        public void saveToFile()
        {
            StreamWriter writer = new StreamWriter("params.txt");

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
            string[] fields = System.IO.File.ReadAllLines("params.txt");
            for (int i = 0; i != fields.Length; ++i)
            {
                string[] parts = fields[i].Split(':');
                if (parts.Length != 2) { continue; }

                FieldInfo field = this.GetType().GetField(parts[0].Trim());
                if (field == null) { continue; }

                try
                {
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
