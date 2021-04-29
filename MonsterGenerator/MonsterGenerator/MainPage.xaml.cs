using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MonsterGenerator
{
    public partial class MainPage : ContentPage
    {
        Random rand = new Random();
        
        public MainPage()
        {
            InitializeComponent();
            StatVisability(false);
        }
        /// <summary>
        /// we make the call to the API /monsters/ endpoint to get a list of monsters. From there we can get the index of each monster so we make a second API call to /monsters/{index of monster, they're strings not numbers'} using a random index from the array in the first api call.
        /// 
        /// From there we change all the label's texts to display the monster's information. 
        /// We check to make sure there is a walking speed and special abilities before displaying information. 
        /// We make all the labels visible. 
        /// 
        /// It appears the catch method fires if you click the button again too quickly. I'm not sure what's going on there. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGetMonster_Clicked(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                
                string monsterList = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/");
                    JObject parsedMonsterList = JObject.Parse(monsterList);
                string jsonData = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/{parsedMonsterList["results"][rand.Next(0,331)]["index"]}/");
               // string jsonData = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/green-hag/");
                try
                {
                    //deserialize JSON
                    MonsterData monsterData = JsonConvert.DeserializeObject<MonsterData>(jsonData);
                                       
                    SetGenericStats(monsterData);
                    SetACHPSpeed(monsterData);
                    SetStats(monsterData);
                    StatVisability(true) ;
                    SetSpecialAbilities(monsterData);
                    SetProficiencies(monsterData);
                    SetImmunitiesSenses(monsterData);
                    SetLanguagesCR(monsterData);
                    
                } 
                catch (Exception ex)
                {
                    DisplayAlert("Oh no!",$"Some goblins must have sabotaged the server! \n ({ex.Message})", "Close");
                }
            }
        }

        private void SetLanguagesCR(MonsterData monsterData)
        {
            string languages = "";
            if (monsterData.languages.Length > 0)
            {               
                SLProficiencies.Children.Add(new Label { Text = $"Languages: {monsterData.languages}", FontSize = 25, TextColor = Xamarin.Forms.Color.Black });
            }
            if (monsterData.challenge_rating > 0)
            {
                SLProficiencies.Children.Add(new Label { Text = $"Challenge: {monsterData.challenge_rating} ({monsterData.xp.ToString()}XP)", FontSize = 25, TextColor = Xamarin.Forms.Color.Black });
            }

        }

        private void SetImmunitiesSenses(MonsterData monsterData)
        {
            string damageImmunities = "";
            string senses = "";

            if (monsterData.damage_immunities.Length > 0)
            {
                for (int i = 0; i < monsterData.damage_immunities.Length; i++)
                {
                    damageImmunities = $"{damageImmunities} {monsterData.damage_immunities[i]}, ";
                }
                SLProficiencies.Children.Add(new Label { Text = $"Damage immunities: {damageImmunities.Substring(0, damageImmunities.Length - 2)}", FontSize = 25, TextColor = Xamarin.Forms.Color.Black});
            }

            if (monsterData.senses != null)
            {
                if (monsterData.senses.darkvision != null) senses = $"{senses} Darkvision {monsterData.senses.darkvision}, ";
                if (monsterData.senses.blindsight != null) senses = $"{senses} Blindsight {monsterData.senses.blindsight}, ";
                if (monsterData.senses.tremorsense != null) senses = $"{senses} Tremorsense {monsterData.senses.tremorsense}, "; 
                if (monsterData.senses.truesight != null) senses = $"{senses} Truesight {monsterData.senses.truesight}, "; 
                if (monsterData.senses.passive_perception > 0) senses = $"{senses} Passive Perception {monsterData.senses.passive_perception.ToString()}, ";
            }
            if (senses.Length > 0)
            {
               SLProficiencies.Children.Add((new Label
                {
                    Text = $"Senses: {senses.Substring(0, senses.Length - 2)}",
                    FontSize = 25,
                    TextColor = Xamarin.Forms.Color.Black
                }));
            }
                
        }


        /// <summary>
        /// Goes through the proficiencies array in the JSON and sees what kinds of saving throws and skills are there and adds them to a string.
        /// I'm proud of this one!
        /// </summary>
        /// <param name="monsterData"></param>
        private void SetProficiencies(MonsterData monsterData)
        {
            SLProficiencies.Children.Clear();
            string savThrows = "";
            string skills = "";

            if (monsterData.proficiencies.Length > 0)
            {
                BVProf.IsVisible = true;
                for (int  i = 0;  i <monsterData.proficiencies.Length;  i++)
                {
                    if (monsterData.proficiencies[i].proficiency.index.Contains("skill"))
                    {
                        string skillName = monsterData.proficiencies[i].proficiency.name;
                        skillName = skillName.Remove(0, 7);
                        skills = $"{skills} {skillName} +{monsterData.proficiencies[i].value.ToString()},";
                    }
                    if (monsterData.proficiencies[i].proficiency.index.Contains("saving"))
                    {
                        string savThrowName = monsterData.proficiencies[i].proficiency.name;
                        savThrowName = savThrowName.Substring(savThrowName.Length - 3);
                        savThrows = $"{savThrows} {savThrowName} + {monsterData.proficiencies[i].value.ToString()},";
                    }
                       
                }   
            } else
            {
                BVProf.IsVisible = false;
            }

            if (savThrows.Length > 0)
            {
                SLProficiencies.Children.Add(new Label
                {
                    Text = $"Saving Throws: {savThrows}",
                    FontSize = 25,
                    TextColor = Xamarin.Forms.Color.Black
                });
            }
            if (skills.Length > 0)
            {
             SLProficiencies.Children.Add(new Label
             {
                Text = $"Skills: {skills}",
                FontSize = 25,
                TextColor = Xamarin.Forms.Color.Black
             });
            }
            
        }

        private void SetGenericStats(MonsterData monsterData)
        {
            LblName.Text = $"{monsterData.name}";
            LblSizeAlignment.Text = $"{monsterData.size.ToString()} {monsterData.type}, {monsterData.alignment}";
            LblACHPSpeed.Text = $"Armor Class: {monsterData.armor_class.ToString()} \n HP: \n {monsterData.hit_points.ToString()} ({monsterData.hit_dice}) \n";
        }

        private void StatVisability(bool tf)
        {
            SLMonsterStats.IsVisible = tf;       
        }

        private void SetSpecialAbilities(MonsterData monsterData)
        {
            SLSpecialAbilities.Children.Clear();
            if (monsterData.special_abilities != null)
            {
                for (int i = 0; i < monsterData.special_abilities.Length; i++)
            {
                SLSpecialAbilities.Children.Add(new Label { Text = $"{monsterData.special_abilities[i].name}",FontAttributes =  FontAttributes.Bold, FontSize = 28, TextColor = Xamarin.Forms.Color.Black });
                SLSpecialAbilities.Children.Add(new Label { Text = $"{monsterData.special_abilities[i].desc}", FontSize = 22, TextColor = Xamarin.Forms.Color.Black });
            }
            }
            
          
        }

        private void SetStats(MonsterData monsterData)
        {
            //Find dice roll modifier based on the monsters stat in each category. 
            int strMod = (int)Math.Floor(((decimal)monsterData.strength - 10) / 2);
            int dexMod = (int)Math.Floor(((decimal)monsterData.dexterity - 10) / 2);
            int conMod = (int)Math.Floor(((decimal)monsterData.constitution - 10) / 2);
            int intMod = (int)Math.Floor(((decimal)monsterData.intelligence - 10) / 2);
            int wisMod = (int)Math.Floor(((decimal)monsterData.wisdom - 10) / 2);
            int charMod = (int)Math.Floor(((decimal)monsterData.charisma - 10) / 2);
            //determine if its positive or negative and add a plus (+) sign in front if it's positive. The minus (-) sign automatically is placed.
            string sMod = (strMod / 2 > 0) ? $"+{strMod}" : strMod.ToString();
            string dMod = (dexMod / 2 > 0) ? $"+{dexMod}" : dexMod.ToString();
            string cMod = (conMod / 2 > 0) ? $"+{conMod}" : conMod.ToString();
            string iMod = (intMod / 2 > 0) ? $"+{intMod}" : intMod.ToString();
            string wMod = (wisMod / 2 > 0) ? $"+{wisMod}" : wisMod.ToString();
            string chMod = (charMod / 2 > 0) ? $"+{charMod}" : charMod.ToString();
            //assign each stat its score and modifier.
            LblStrength.Text = $"  STR    \n{monsterData.strength.ToString()} ({sMod})";
            LblDex.Text = $"  DEX    \n{monsterData.dexterity.ToString()} ({dMod})";
            LblCon.Text = $"  CON    \n{monsterData.constitution.ToString()} ({cMod})";
            LblInt.Text = $"  INT    \n{monsterData.intelligence.ToString()} ({iMod})";
            LblWis.Text = $"  WIS    \n{monsterData.wisdom.ToString()} ({wMod})";
            LblChar.Text = $"  CHAR    \n{monsterData.charisma.ToString()} ({chMod})";

        } 

        private void SetACHPSpeed(MonsterData monsterData)
        {
            if (monsterData.speed.walk != null)
            {
                if (monsterData.speed.fly != null)
                {
                    if (monsterData.speed.swim != null)
                    {
                        LblACHPSpeed.Text = $"Armor Class: {monsterData.armor_class.ToString()} \nHit Points:  {monsterData.hit_points.ToString()} ({monsterData.hit_dice}) \nSpeed: {monsterData.speed.walk}, fly {monsterData.speed.fly}, swim {monsterData.speed.swim}";

                    }
                    else
                    {
                        LblACHPSpeed.Text = $"Armor Class: {monsterData.armor_class.ToString()} \nHit Points:  {monsterData.hit_points.ToString()} ({monsterData.hit_dice}) \nSpeed: {monsterData.speed.walk}, fly {monsterData.speed.fly}";
                    }
                }
                else
                {
                    LblACHPSpeed.Text = $"Armor Class: {monsterData.armor_class.ToString()} \nHit Points: {monsterData.hit_points.ToString()} ({monsterData.hit_dice}) \nSpeed: {monsterData.speed.walk}";
                }
            }
            else
            {
                LblACHPSpeed.Text = $"Speed: No Data";
            }
        }
    }
}
