namespace Assets.Scripts.Both.Creature.Attackable
{
    public enum SkillName
    {
        FireBall,
        Ragnarok,
        BatSummon,
        Slash,
        Shield,
        BatBite,
        TreeAttack,
        TriFireBall,
        SunBoost,
        Dash,
        TeleDash,
    }

    public enum TagType
    {
        Attack,
        Effect, //buff, debuff for EffectStatsCalc
        Special, //summon, teleport, immortal
    }

    public enum AttackTag
    {
        Normal, //in this creature: slash, shield, spear...
        Bullet,
        SelfArea,
        TargetArea, //optional
    }

    public enum EffectTag
    {
        Add,
        Multiple,
        Substract,
        Divide,
    }

    public enum SpecialTag
    {
        Summon,
        Teleport,
        Immortal,
    }

    public enum SummonPlace
    {
        Position,
        Target,
    }

    public enum TeleportPlace
    {
        ObstaclesStop,
        ThroughAll,
        Point,// Choice in Range?
    }

    public enum CastDirection
    {
        Orientation,
        Direction,
    }
}