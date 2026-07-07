namespace KaelKodes.KaelsKleave;

public class KaelsKleaveConfig
{
    public float MinRange { get; set; } = 2.0f;

    public int MaxExtraTargets { get; set; } = 2;

    public float SecondaryDamageMultiplier { get; set; } = 1.0f;

    public bool ExcludeRepeatedMelee { get; set; } = true;

    /// <summary>
    /// When true, melee cleave also applies to players (friendly-fire rules still apply).
    /// When false, only zombies can be cleaved.
    /// </summary>
    public bool IncludePlayers { get; set; }

    /// <summary>
    /// Half-angle of the cleave cone in front of the attacker.
    /// </summary>
    public float CleaveAngleDegrees { get; set; } = 70f;
}
