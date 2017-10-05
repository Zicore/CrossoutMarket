namespace Crossout.Data.Stats.Main
{
    partial class PartStatsCabin : PartStatsBase
    {
        // Custom Properties

        [Stat("Power Score", 0, CustomClasses = StatAttribute.PowerScoreClasses)]
        public int StatPowerScore
        {
            get { return universal_rating; }
        }

        [Stat("Required Level", 5)]
        public int StatRequiredLevel
        {
            get { return require_faction_level; }
        }

        //------

        [Stat("Engine Speed", 10)]
        public int StatEngineSpeed
        {
            get { return engine_max_speed; }
        }

        [Stat("Max. Speed", 20)]
        public int StatMaxSpeed
        {
            get { return speed_limit; }
        }

        [Stat("Power", 30)]
        public int StatEnginePower
        {
            get { return engine_power; }
        }

        [Stat("Tonnage", 40)]
        public int StatTonnage
        {
            get { return tonnage; }
        }

        [Stat("Mass Limit", 50)]
        public int StatMassLimit
        {
            get { return critical_mass; }
        }

        [Stat("Adds Energy", 60)]
        public int StatAddsEnergy
        {
            get { return power; }
        }

        //------

        [Stat("Structure", 70)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Mass", 80)]
        public double StatMass
        {
            get { return physics_mass; }
        }

        //------

        [Stat("Radar Radius", 100, Type = StatType.Hidden)]
        public int StatRadarRadius
        {
            get { return radar_radius; }
        }

        [Stat("Radio Radius", 110, Type = StatType.Hidden)]
        public int StatRadioRadius
        {
            get { return radio_radius; }
        }

        [Stat("Max. Velocity", 120, Type = StatType.Hidden)]
        public int StatMaxVelocity
        {
            get { return max_velocity; }
        }

        [Stat("Brake Value", 130, Type = StatType.Hidden)]
        public int StatBrakeValue
        {
            get { return brake_value; }
        }
    }
}
