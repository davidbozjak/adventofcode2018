using System.Text.RegularExpressions;

//var imuneSystemArmyRecord = new List<BattleGroupRecord>()
//{
//    new BattleGroupRecord(17, 5390, 2, 4507, "fire", new[] { "radiation", "bludgeoning" }, Enumerable.Empty<string>()),
//    new BattleGroupRecord(989, 1274, 3, 25, "slashing", new[] { "bludgeoning", "slashing" }, new[] { "fire" })
//};

//var infectionArmyRecord = new List<BattleGroupRecord>()
//{
//    new BattleGroupRecord(801, 4706, 1, 116, "bludgeoning", new[] { "radiation" }, Enumerable.Empty<string>()),
//    new BattleGroupRecord(4485, 2961, 4, 12, "slashing", new[] { "fire", "cold" }, new[] { "radiation" })
//};

var imuneSystemArmyRecord = new InputProvider<BattleGroupRecord?>("ImuneSystemArmies.txt", GetBattleGroupRecord).Where(w => w != null).Cast<BattleGroupRecord>().ToList();
var infectionArmyRecord = new InputProvider<BattleGroupRecord?>("InfectionArmies.txt", GetBattleGroupRecord).Where(w => w != null).Cast<BattleGroupRecord>().ToList();

var imuneSystemArmy = imuneSystemArmyRecord.Select(w => new BattleGroup(w)).ToList();
var infectionArmy = infectionArmyRecord.Select(w => new BattleGroup(w)).ToList();

var winningArmy = ExecuteFight(imuneSystemArmy, infectionArmy);
Console.WriteLine($"Part 1: {winningArmy.Sum(w => w.NumberOfRemainingUnits)}");

var maxBoost = int.MaxValue / imuneSystemArmyRecord.Max(w => w.NumberOfUnits);  //making sure we avoid ínteger overflow when calculating EffectivePower. Plus it should be enough
var minBoost = FindMinBoost(1, maxBoost, imuneSystemArmyRecord, infectionArmyRecord);

imuneSystemArmy = imuneSystemArmyRecord.Select(w => new BattleGroup(new BattleGroupRecordWithBoost(w, minBoost))).ToList();
infectionArmy = infectionArmyRecord.Select(w => new BattleGroup(w)).ToList();

winningArmy = ExecuteFight(imuneSystemArmy, infectionArmy);

if ((winningArmy != imuneSystemArmy) || (winningArmy.Count <= 0))
    throw new Exception();

Console.WriteLine($"Part 2: {winningArmy.Sum(w => w.NumberOfRemainingUnits)}");

static int FindMinBoost(int minBoost, int maxBoost, IEnumerable<BattleGroupRecord> imuneSystemArmyRecord, IEnumerable<BattleGroupRecord> infectionArmyRecord)
{
    var distance = maxBoost - minBoost;
    int boost;

    if (distance == 0)
    {
        return minBoost;
    }
    if (distance == 1)
    {
        boost = maxBoost;
    }
    else
    {
        boost = minBoost + ((maxBoost - minBoost) / 2);
    }

    var boostRecords = imuneSystemArmyRecord.Select(w => new BattleGroupRecordWithBoost(w, boost));

    var imuneSystemArmy = boostRecords.Select(w => new BattleGroup(w)).ToList();
    var infectionArmy = infectionArmyRecord.Select(w => new BattleGroup(w)).ToList();

    var winner = ExecuteFight(imuneSystemArmy, infectionArmy);

    if (distance == 1)
    {
        if (winner == imuneSystemArmy) return maxBoost;
        else return minBoost;
    }
    else
    {
        if (winner == imuneSystemArmy)
        {
            return FindMinBoost(minBoost, boost, imuneSystemArmyRecord, infectionArmyRecord);
        }
        else
        {
            return FindMinBoost(boost, maxBoost, imuneSystemArmyRecord, infectionArmyRecord);
        }
    }
}

static List<BattleGroup> ExecuteFight(List<BattleGroup> imuneSystemArmy, List<BattleGroup> infectionArmy)
{
    var armies = new[] { imuneSystemArmy, infectionArmy };
    int stalemateRoundsCount = 0;

    while (imuneSystemArmy.Count > 0 && infectionArmy.Count > 0)
    {
        var imuneSystemTargets = imuneSystemArmy.ToList();
        var infectionTargets = infectionArmy.ToList();

        var actors = armies.SelectMany(w => w).ToList();

        foreach (var bg in actors)
        {
            bg.ResetTargetSelection();
        }

        GroupSelectTarget(imuneSystemArmy, infectionTargets);
        GroupSelectTarget(infectionArmy, imuneSystemTargets);

        int numberOfDefeatedUnits = 0;
        while (actors.Count > 0)
        {
            var attacker = actors.OrderByDescending(w => w.Initiative).First();
            actors.Remove(attacker);

            if (attacker.Target != null)
            {
                var defender = attacker.Target;

                var newDefender = BattleGroup.Fight(attacker, defender);

                bool reAddDefender = actors.Remove(defender);

                var parentArmy = armies.Where(w => w.Contains(defender)).First();
                parentArmy.Remove(defender);

                if (newDefender != null)
                {
                    numberOfDefeatedUnits += defender.NumberOfRemainingUnits - newDefender.NumberOfRemainingUnits;
                    parentArmy.Add(newDefender);

                    if (reAddDefender)
                    {
                        actors.Add(newDefender);
                    }
                }
            }
        }

        if (numberOfDefeatedUnits == 0)
        {
            stalemateRoundsCount++;
        }
        if (stalemateRoundsCount >= 3)
        {
            //Stalemate detected, returning empty list to indicate noone is winning this fight
            return new List<BattleGroup>();
        }
    }

    var winningArmy = armies.Where(w => w.Count > 0).First();
    return winningArmy;

    static void GroupSelectTarget(IEnumerable<BattleGroup> army, List<BattleGroup> targets)
    {
        foreach (var battleGroup in army.OrderByDescending(w => w.EffectivePower).ThenByDescending(w => w.Initiative))
        {
            var selectedTarget = battleGroup.SetTarget(targets);
            targets.Remove(selectedTarget);
        }
    }
}

static bool GetBattleGroupRecord(string? input, out BattleGroupRecord? value)
{
    value = null;

    if (input == null) return false;

    input = input.ToLower();

    Regex numRegex = new(@"\d+");
    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    Regex attackTypeRegex = new(@" ([a-z]*) damage");
    var attackType = attackTypeRegex.Match(input).Groups[1].Value;

    var immunities = input.Contains("immune to ") ? input[(input.IndexOf("immune to ") + 10)..IndexOfEnd("immune to")].Split(',').Select(w => w.Trim()).ToArray() : Enumerable.Empty<string>();
    var weaknesses = input.Contains("weak to ") ? input[(input.IndexOf("weak to ") + 8)..IndexOfEnd("weak to")].Split(',').Select(w => w.Trim()).ToArray() : Enumerable.Empty<string>();

    value = new BattleGroupRecord(numbers[0], numbers[1], numbers[3], numbers[2], attackType, weaknesses, immunities);

    return true;

    int IndexOfEnd(string start)
    {
        var startIndex = input.IndexOf(start);
        var endIndex = input.IndexOf(";", startIndex) > -1 ? input.IndexOf(";", startIndex) : input.IndexOf(")", startIndex);
        return endIndex;
    }
}

record BattleGroupRecord(int NumberOfUnits, int HitPointsPerUnit, int Initiative, int AttackDamage, string AttackType, IEnumerable<string> Weaknesses, IEnumerable<string> Immunities)
{
    public override string ToString()
        => $"{NumberOfUnits} units each with {HitPointsPerUnit} hit points (immune to {string.Join(',', Immunities)}; weak to {string.Join(',', Weaknesses)}) with an attack that does {AttackDamage} {AttackType} damage at initiative {Initiative}";
}

record BattleGroupRecordWithBoost : BattleGroupRecord
{
    public BattleGroupRecordWithBoost(BattleGroupRecord record, int boost) 
        : base(record.NumberOfUnits, record.HitPointsPerUnit, record.Initiative, record.AttackDamage + boost, record.AttackType, record.Weaknesses, record.Immunities)
    {
    }
}

[System.Diagnostics.DebuggerDisplayAttribute("Units remaining {NumberOfRemainingUnits} EffectivePower {EffectivePower} {AttackType}")]
class BattleGroup
{
    private readonly BattleGroupRecord record;

    public int NumberOfRemainingUnits { get; init; }

    public int AttackDamage => this.record.AttackDamage;

    public int EffectivePower => this.NumberOfRemainingUnits * this.AttackDamage;

    public int Initiative => this.record.Initiative;

    public string AttackType => this.record.AttackType;

    public IEnumerable<string> Weaknesses => this.record.Weaknesses;

    public IEnumerable<string> Immunities => this.record.Immunities;

    public int HitPointsPerUnit => this.record.HitPointsPerUnit;

    private bool hasEverSetTarget = false;
    private BattleGroup? selectedTarget;
    public BattleGroup? Target
    {
        get
        {
            if (!hasEverSetTarget) throw new Exception("Never evaluated targets");
            return selectedTarget;
        }
        private set
        {
            if (hasEverSetTarget) throw new Exception("This instance has already evaluated target");
            hasEverSetTarget = true;
            selectedTarget = value;
        }
    }

    public BattleGroup(BattleGroupRecord record)
    {
        this.record = record;
        this.NumberOfRemainingUnits = record.NumberOfUnits;
    }

    protected BattleGroup(BattleGroup original, int numberOfRemainingUnits)
    {
        this.record = original.record;
        this.NumberOfRemainingUnits = numberOfRemainingUnits;
    }

    public BattleGroup? SetTarget(IEnumerable<BattleGroup> enemies)
    {
        var targetDamages = enemies.Select(w => new { Group = w, TotalDamage = GetTotalDamageDelt(this, w) })
            .Where(w => w.TotalDamage > 0)         // from instructions: "If it cannot deal any defending groups damage, it does not choose a target."
            .OrderByDescending(w => w.TotalDamage)
            .ThenByDescending(w => w.Group.EffectivePower)
            .ThenByDescending(w => w.Group.Initiative);

        if (!targetDamages.Any())
        {
            this.Target = null;
        }
        else
        {
            this.Target = targetDamages.Select(w => w.Group).First();
        }

        return this.Target;
    }

    public void ResetTargetSelection()
    {
        this.selectedTarget = null;
        this.hasEverSetTarget = false;
    }

    public override string ToString()
        => $"I{Initiative}: {NumberOfRemainingUnits} left, doing {AttackType} damage";

    public static BattleGroup? Fight(BattleGroup attackers, BattleGroup defenders)
    {
        int totalDamage = GetTotalDamageDelt(attackers, defenders);

        int slainDefenders = totalDamage / defenders.HitPointsPerUnit;

        int remainingDefenders = defenders.NumberOfRemainingUnits - slainDefenders;

        if (remainingDefenders <= 0)
            return null;

        return new BattleGroup(defenders, remainingDefenders) { Target = defenders.Target };
    }

    private static int GetTotalDamageDelt(BattleGroup attacker, BattleGroup defender)
    {
        int totalDamage = attacker.EffectivePower;

        if (defender.Immunities.Contains(attacker.AttackType))
        {
            totalDamage = 0;
        }
        else if (defender.Weaknesses.Contains(attacker.AttackType))
        {
            totalDamage *= 2;
        }

        return totalDamage;
    }
}