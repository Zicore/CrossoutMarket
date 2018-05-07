namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsWeapon
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

        [Stat("Damage Rating", 10, ShowProgressBar = true)]
        public double StatPercentDamageRating
        {
            get { return damage_rating * 100.0f; }
        }

        [Stat("Fire Rate Rating", 20, ShowProgressBar = true)]
        public double StatPercentFireRateRating
        {
            get { return fire_rate_rating * 100.0f; }
        }

        [Stat("Range Rating", 30, ShowProgressBar = true)]
        public double StatPercentRangeRating
        {
            get { return range_rating * 100.0f; }
        }

        [Stat("Accuracy Rating", 40, ShowProgressBar = true)]
        public double StatPercentAccuracyRating
        {
            get { return accuracy_rating * 100.0f; }
        }

        [Stat("Overheat Rating", 50, ShowProgressBar = true)]
        public double StatPercentOverheatRating
        {
            get { return overheat_rating * 100.0f; }
        }

        [Stat("Ammo", 55)]
        public int StatAmmo
        {
            get { return ammo; }
        }

        //------

        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Energy Drain", 70)]
        public int StatEnergyDrain
        {
            get { return power_require; }
        }

        [Stat("Mass", 80)]
        public double StatMass
        {
            get { return physics_mass; }
        }

        //------ Hidden Stats

        [Stat("Ballistic Damage", 100, Type = StatType.Hidden)]
        public double StatBallisticDamage
        {
            get { return damage; }
        }

        [Stat("Collision Damage", 105, Type = StatType.Hidden)]
        public int StatCollisionDamage
        {
            get { return collision_damage; }
        }

        [Stat("Blast Damage", 110, Type = StatType.Hidden)]
        public double StatBlastDamage
        {
            get { return blast_damage; }
        }

        [Stat("Projectile Velocity", 111, Type = StatType.Hidden)]
        public double ProjectileVelocity
        {
            get { return projectile_speed; }
        }

        [Stat("Ballisitic Impulse", 120, Type = StatType.Hidden)]
        public double StatBallisticImpulse
        {
            get { return hit_impulse; }
        }

        [Stat("Blast Impulse", 121, Type = StatType.Hidden)]
        public int StatBlastImpulse
        {
            get { return blast_impulse; }
        }

        [Stat("Blast Radius", 125, Type = StatType.Hidden)]
        public double StatBlastRadius
        {
            get { return blast_radius; }
        }

        [Stat("Fire Rate", 130, Type = StatType.Hidden)]
        public int StatFireRate
        {
            get { return fire_rate; }
        }

        [Stat("Shooting Cooldown", 135, Type = StatType.Hidden)]
        public double StatShootingCooldown
        {
            get { return shooting_cooldown; }
        }

        [Stat("Optimal Range", 140, Type = StatType.Hidden)]
        public double StatOptimalRange
        {
            get { return optimal_range; }
        }

        [Stat("Max Range", 150, Type = StatType.Hidden)]
        public double StatMaxRange
        {
            get { return max_range; }
        }

        [Stat("Spread Static", 160, Type = StatType.Hidden)]
        public double StatSpreadStatic
        {
            get { return spread_stat; }
        }

        [Stat("Maximum Spread Static", 165, Type = StatType.Hidden)]
        public double StatMaximumSpreadStatic
        {
            get { return spread_stat_max; }
        }

        [Stat("Spread Moving", 170, Type = StatType.Hidden)]
        public double StatSpreadMoving
        {
            get { return spread_move; }
        }

        [Stat("Maximum Spread Moving", 175, Type = StatType.Hidden)]
        public double StatMaximumSpreadMoving
        {
            get { return spread_move_max; }
        }

        [Stat("Spread Increase", 180, Type = StatType.Hidden)]
        public double StatSpreadIncrease
        {
            get { return spread_inc; }
        }

        [Stat("Spread Decrease", 185, Type = StatType.Hidden)]
        public double StatSpreadDecrese
        {
            get { return spread_dec; }
        }

        [Stat("Spread Rotation Increase", 190, Type = StatType.Hidden)]
        public double StatSpreadRotationIncrease
        {
            get { return spread_rot_inc; }
        }

        [Stat("Recoil", 195, Type = StatType.Hidden)]
        public double StatRecoil
        {
            get { return recoil_impulse; }
        }

        [Stat("Maximum Heat", 200, Type = StatType.Hidden)]
        public int StatMaximumHeat
        {
            get { return heat_max; }
        }

        [Stat("Heat Increase", 210, Type = StatType.Hidden)]
        public double StatHeatIncrease
        {
            get { return heat_inc; }
        }

        [Stat("Heat Decrease", 215, Type = StatType.Hidden)]
        public double StatHeatDecrease
        {
            get { return heat_dec; }
        }

        [Stat("Gun Elevation", 215, Type = StatType.Hidden)]
        public int StatGunElevation
        {
            get { return max_pitch; }
        }

        [Stat("Gun Depression", 215, Type = StatType.Hidden)]
        public int StatGunDepression
        {
            get { return min_pitch; }
        }

        [Stat("Gun Traverse Right", 220, Type = StatType.Hidden)]
        public int StatGunTraverseRight
        {
            get { return max_yaw; }
        }

        [Stat("Gun Traverse Left", 225, Type = StatType.Hidden)]
        public int StatGunTraverseLeft
        {
            get { return min_yaw; }
        }

        [Stat("Gun Rotation Speed", 225, Type = StatType.Hidden)]
        public double StatGunRotationSpeed
        {
            get { return rot_speed; }
        }

        [Stat("Collision Resistance", 240, Type = StatType.Hidden)]
        public int StatCollisionResistance
        {
            get { return collision_resist; }
        }

    }
}
