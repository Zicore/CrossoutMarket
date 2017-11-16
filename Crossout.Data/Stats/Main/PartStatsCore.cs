namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsCore : PartStatsBase
    {
        // Custom Properties

        [Stat("Power Score", 0, CustomClasses = StatAttribute.PowerScoreClasses)]
        public int StatPowerScore
        {
            get { return universal_rating; }
        }

        //------

        [Stat("Maximum Deployable Units", 5)]
        public int StatMaximumDeployablesUnits
        {
            get { return deploy_ammo; }
        }

        //------

        [Stat("Adds Energy", 10)]
        public double StatPower
        {
            get { return power; }
        }

        [Stat("Engine Speed Multiplier", 20)]
        public double StatEngineSpeedMultiplier
        {
            get { return engine_max_speed_mul; }
        }

        [Stat("Engine Power Multiplier", 30)]
        public double StatEnginePowerMultiplier
        {
            get { return engine_power_mul; }
        }

        [Stat("Mass Limit", 40)]
        public double StatMassLimit
        {
            get { return critical_mass_add; }
        }

        //------

        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Energy Drain", 70)]
        public double StatEnergyDrain
        {
            get { return power_require; }
        }

        [Stat("Mass", 80)]
        public double StatMass
        {
            get { return physics_mass; }
        }

        //------ Hidden

        [Stat("Shooting Cooldown Multiplier", 100, Type = StatType.Hidden)]
        public double StatShootingCooldownMultiplier
        {
            get { return shooting_cooldown_mul; }
        }

        [Stat("Heat Increase Multiplier", 110, Type = StatType.Hidden)]
        public double StatHeatIncreaseMultiplier
        {
            get { return heat_max_mul; }
        }

        [Stat("Heat Decrease Multiplier", 115, Type = StatType.Hidden)]
        public double StatHeatDecreaseMultiplier
        {
            get { return heat_dec_mul; }
        }

        [Stat("Ammo Multiplier", 120, Type = StatType.Hidden)]
        public double StatAmmoMultiplier
        {
            get { return ammo_mul; }
        }

        [Stat("Active Duration", 130, Type = StatType.Hidden)]
        public double StatDuration
        {
            get { return work_time; }
        }

        [Stat("Cooldown", 140, Type = StatType.Hidden)]
        public int StatCooldown
        {
            get { return cooldown; }
        }

        [Stat("Boost Power", 150, Type = StatType.Hidden)]
        public int StatBoostPower
        {
            get { return boost_value; }
        }

        [Stat("Destruction Blast Damage", 160, Type = StatType.Hidden)]
        public int StatDestructionBlastDamage
        {
            get { return death_blast_damage; }
        }

        [Stat("Destruction Blast Radius", 170, Type = StatType.Hidden)]
        public double StatDestructionBlastRadius
        {
            get { return death_blast_radius; }
        }

        [Stat("Radar Radius", 180, Type = StatType.Hidden)]
        public int StatRadarRadius
        {
            get { return radar_radius; }
        }

        [Stat("Infravision Radius", 190, Type = StatType.Hidden)]
        public int StatInfravisionRadius
        {
            get { return infravision_radius; }
        }

        [Stat("Radio Radius", 200, Type = StatType.Hidden)]
        public int StatRadioRadius
        {
            get { return radio_radius; }
        }
    }
}
