using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    /*
     * Chuck any parameter values in here, to be referenced via world
     */
    //[Serializable,HideInInspector]
    public class Params
    {
        public int mapGen_sizeX = 32;
        public int mapGen_sizeY = 24;
        public double mapGen_proportionOfMapForHumans = 0.65;
        public float mapGen_minHabitabilityForHumans = 0.15f;
        public int mapGen_stepsPerIsland = 12;
        public int mapGen_maxBrushSize = 5;
        public int mapGen_burnInSteps = 50;

        public int overmind_maxPower = 24;
        public float overmind_powerRegen = 1f;
        public bool overmind_singleAbilityPerTurn = true;

        public double econ_multFromBuff = 0.75;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        
        public double relObj_defaultLiking = 5;
        public double relObj_decayRate = 0.98;

        public double combat_prestigeLossFromConquest = 0.333;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.5;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land
        public double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations
        public double combat_defensivePostureDmgMult = 0.666;

        public int war_defaultLength = 10;
        public float minInformationAvailability = 0.2f;

        public double utility_econEffect = 0.5;
        public double utility_econEffectOther = 0.20;
        public double utility_militaryTargetRelStrengthOffensive = 250;
        public double utility_militaryTargetRelStrengthDefensive = 300;
        public double utility_vassaliseReluctance = -100;
        public double utility_vassaliseMilMult = 80;
        public double utility_vassaliseThreatMult = 0.75;
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

        public double person_maxLikingGainFromVoteAccordance = 30;
        public double person_maxLikingLossFromVoteDiscord = -50;
        public double person_prestigeDeltaPerTurn = 0.5;
        public double person_threatMult = 100;
        public double person_defaultPrestige = 5;
        public double person_evidencePerShadow = 0.025;
        public double person_evidenceExponent = 1.5;
        public double person_suspicionPerEvidence = 0.06;
        public double person_dislikingFromSuspicion = -200;
        public double person_shadowContagionMult = 0.05;
        public double person_threatFromSuspicion = 400;
        public double person_shadowDecayPerTurn = 0.005;

        public int econ_buffDuration = 50;

        public int soc_untitledPeople = 3;
        public int society_votingDuration = 2;
        public double society_votingRelChangePerUtilityPositive = 0.085;//If benefitted by a vote
        public double society_votingRelChangePerUtilityNegative = 0.14;//If harmed by a vote
        public int society_instablityTillRebellion = 10;
        public int society_rebelLikingThreshold = -5;
        public int society_zeitDuration = 3;
        public double society_sovreignPrestige = 10;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 30;
        public int society_nPeopleForEmpire = 21;
        public int society_nPeopleForKingdom = 12;
        public double society_introversionStabilityGain = 1.2;

        public double temporaryThreatDecay = 0.95;
        public double threat_takeLocation = 5;
        public double victory_targetEnshadowmentAvrg = 0.75;

        public double dark_evilThreatMult = 1.5;
        public double dark_fleshThreatMult = 2;
        public double dark_fishmanStartingThreatMult = -0.25;
        
        public int ability_uncannyGlamourEvidence = 10;
        public int ability_uncannyGlamourGain = 15;
        public int ability_uncannyGlamourCost = 10;
        public int ability_uncannyGlamourCooldown = 15;
        public int ability_sowDissentCost = 12;
        public int ability_sowDissentLikingChange = -25;

        public double ability_darkEmpireShadowPerTurn = 0.05;
        public int ability_denounceOtherCooldown = 32;
        public int ability_proposeVoteCooldown = 7;
        public double ability_growFleshThreatAdd = 5;
        public int ability_shareEvidenceLikingCost = 20;
        public int ability_switchVoteLikingCost = 20;
        public double ability_shareEvidencePercentage = 0.75;
        public int ability_enshadowCost = 4;
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
        public int ability_shortMemories = 5;
        public double society_pExpandIntoEmpty = 0.1;
        public double temporaryThreatConversion = 0.01;
        public int ability_fleshScreamThreatAdd = 10;
        public int ability_fleshScreamCost = 7;
        public int ability_fleshScreamSanity = 4;
        public double threat_temporaryDreadDecay = 0.97;
        public int ability_informationBlackoutCost = 4;
        public int ability_informationBlackoutDuration = 15;
        public int ability_FishmanCultDuration = 15;
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
        public int ability_sharedGloryLikingGain = 8;
        public int ability_sharedGloryAmount = 5;

        public double insanity_sanityRegen = 0.5;
        internal int insanity_nParanoiaTargets = 4;
        internal int insanity_relHit = -10;
        internal double insanity_lashOutProbability = 0.07;
        internal int insanity_maxSanity = 17;

        public double trait_incautious = 0.5;
        public double trait_aware = 2;
        public double trait_charismatic = 15;
        public double trait_unlikable = -15;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
