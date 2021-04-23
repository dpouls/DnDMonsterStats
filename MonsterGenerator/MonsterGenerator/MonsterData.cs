using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterGenerator
{

    public class MonsterData
    {
        public string index { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string alignment { get; set; }
        public int armor_class { get; set; }
        public int hit_points { get; set; }
        public string hit_dice { get; set; }
        public Speed speed { get; set; }
        public int strength { get; set; }
        public int dexterity { get; set; }
        public int constitution { get; set; }
        public int intelligence { get; set; }
        public int wisdom { get; set; }
        public int charisma { get; set; }
        public Proficiency[] proficiencies { get; set; }
        public object[] damage_vulnerabilities { get; set; }
        public object[] damage_resistances { get; set; }
        public object[] damage_immunities { get; set; }
        public object[] condition_immunities { get; set; }
        public Senses senses { get; set; }
        public string languages { get; set; }
        public float challenge_rating { get; set; }
        public int xp { get; set; }
        public Special_Abilities[] special_abilities { get; set; }
        public Action[] actions { get; set; }
        public string url { get; set; }
    }

    public class Speed
    {
        public string walk { get; set; }
    }

    public class Senses
    {
        public string darkvision { get; set; }
        public int passive_perception { get; set; }
    }

    public class Proficiency
    {
        public int value { get; set; }
        public Proficiency1 proficiency { get; set; }
    }

    public class Proficiency1
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Special_Abilities
    {
        public string name { get; set; }
        public string desc { get; set; }
    }

    public class Action
    {
        public string name { get; set; }
        public string desc { get; set; }
        public int attack_bonus { get; set; }
        public Damage[] damage { get; set; }
    }

    public class Damage
    {
        public Damage_Type damage_type { get; set; }
        public string damage_dice { get; set; }
    }

    public class Damage_Type
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

}
