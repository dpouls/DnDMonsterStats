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
        MonsterData monsterData = new MonsterData();
        
        public MainPage()
        {
            InitializeComponent();
            StatVisability(false);
        }
        /// <summary>
        /// NEW: This is for when I close the modal, it refreshes the mainpage and to display which monster was selected.
        /// </summary>
        protected override void  OnAppearing()
        {
            CheckSelectedStatus();

        }
        /// <summary>
        /// NEW: Checks the SelectedMonster class to see if the user picked a specific monster on the modal, if so, we will display its results by calling GetSelectedMonster.
        /// </summary>
        private void CheckSelectedStatus()
        {
            if (SelectedMonster.SelectedMonsterIndex != null)
            {
                GetSelectedMonster(SelectedMonster.SelectedMonsterIndex);
            }
        }

        /// <summary>
        /// OLD: Makes an API call to get the list of monsters, then uses that array to get a random monster and make a second api call with its index.
        /// NEW:
        /// Seperated all the stats into different methods. 
        /// Changed the display of the first few stats.
        /// Added lots of functionality to display the varying content of each monster. (Speeds/actions/abilities/legendary actions/ability modifiers ---see comments on each method)
        /// Created a new method to hide all data before it arrives. 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///
        private  void BtnGetMonster_Clicked(object sender, EventArgs e)
        {
            SLActions.Children.Clear();
            SLLegActions.Children.Clear();
            SLSpecialAbilities.Children.Clear();
            BtnLoadMore.Text = "LOAD ABILITIES AND ACTIONS";
            
            using (WebClient wc = new WebClient())
            {
                
                string monsterList =  wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/");
                    JObject parsedMonsterList = JObject.Parse(monsterList);
                string jsonData = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/{parsedMonsterList["results"][rand.Next(0,331)]["index"]}/");
                
                try
                {
                    //deserialize JSON
                    monsterData =  JsonConvert.DeserializeObject<MonsterData>(jsonData);
                    LblActions.IsVisible = false;
                    SetGenericStats(monsterData);
                    SetACHPSpeed(monsterData);
                    SetStats(monsterData);
                    SetProficiencies(monsterData);
                    SetImmunitiesSenses(monsterData);
                    SetLanguagesCR(monsterData);
                    StatVisability(true) ;
                    
                } 
                catch (Exception ex)
                {
                    DisplayAlert("Oh no!",$"Some goblins must have sabotaged the server! \n ({ex.Message})", "Close");
                }
            }
        }
        /// <summary>
        /// NEW: Dynamically generates the legendary actions from the array (if there is one.)
        /// Hides the label/stack layout if there is no array sent.
        /// Uses FormattedString() to bold part of the line. 
        /// </summary>
        /// <param name="monsterData"></param>
        private void SetLegendaryActions(MonsterData monsterData)
        {
            SLLegActions.Children.Clear();
            if (monsterData.legendary_actions != null)
            {
                LblLegendaryActions.IsVisible = true;
            for (int i = 0; i < monsterData.legendary_actions.Length; i++)
            {
                var fs = new FormattedString();
                fs.Spans.Add(new Span { Text = $"{monsterData.legendary_actions[i].name}. ", FontSize = 25, FontAttributes = FontAttributes.Bold | FontAttributes.Italic, TextColor = Xamarin.Forms.Color.Black });

                fs.Spans.Add(new Span { Text = $"{monsterData.legendary_actions[i].desc}. ", FontSize = 24, TextColor = Xamarin.Forms.Color.Black });

                var LblLegActions = new Label();
                LblLegActions.FormattedText = fs;
                SLLegActions.Children.Add(LblLegActions); ;
            }
            }
            LblLegendaryActions.IsVisible = false;
            
        }
        /// <summary>
        /// NEW: Dynamically generates the actions from the array (if there is one.)
        /// Hides the label/stack layout if there is no array sent.
        /// Uses FormattedString() to bold part of the line. 
        /// </summary>
        /// <param name="monsterData"></param>
        private void SetActions(MonsterData monsterData)
        {
            SLActions.Children.Clear();
            if (monsterData.actions != null)
            {

            for (int i = 0; i < monsterData.actions.Length; i++)
            {
                var fs = new FormattedString();
                fs.Spans.Add(new Span { Text = $"{monsterData.actions[i].name}. " , FontSize = 25, FontAttributes = FontAttributes.Bold | FontAttributes.Italic,  TextColor = Xamarin.Forms.Color.Black });

                fs.Spans.Add(new Span { Text = $"{monsterData.actions[i].desc}. ", FontSize = 24, TextColor = Xamarin.Forms.Color.Black });
                
                var LblActions = new Label();
                LblActions.FormattedText = fs;
                SLActions.Children.Add(LblActions);;
            }
            }
            else
            {
                LblActions.IsVisible = false;
                SLActions.IsVisible = false;
            }
        }
        /// <summary>
        /// NEW: If the monster speaks languages, assign the string a label.
        /// If it has a challenge rating, add it to a new label and concatenate the xp. 
        /// </summary>
        /// <param name="monsterData"></param>
        private void SetLanguagesCR(MonsterData monsterData)
        {
            
            if (monsterData.languages.Length > 0)
            {               
                SLProficiencies.Children.Add(new Label { Text = $"Languages: {monsterData.languages}", FontSize = 25, TextColor = Xamarin.Forms.Color.Black });
            }
            if (monsterData.challenge_rating > 0)
            {
                SLProficiencies.Children.Add(new Label { Text = $"Challenge: {monsterData.challenge_rating} ({monsterData.xp.ToString()}XP)", FontSize = 25, TextColor = Xamarin.Forms.Color.Black });
            }

        }
        /// <summary>
        /// NEW: 
        /// Creates a string that shows the damage imunities seperated by commas. Was hard because of the formatting of the strings in the JSON.
        /// Sees if the monster has one or more of the four extra senses and concatenates them into a string. 
        /// Both remove the last comma from the string.
        /// </summary>
        /// <param name="monsterData"></param>
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
        /// NEW: Goes through the proficiencies array in the JSON and sees what kinds of saving throws and skills are there and adds them to a string.
        /// I'm proud of this one!
        /// The JSON was annoyingly formatted and I had to remove parts of the strings for displaying. There are a ton of skills (20 ish) and proficiencies (20 ish)so I tried to find a way to display what the monster has without having to write an if statement for every single skill/proficiency. 
        /// 
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
                //removes red line
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
        /// <summary>
        /// NEW:
        /// These stats were in the old project but I have redone them so they look better and take up less space (both on the app and in the code )
        /// </summary>
        /// <param name="monsterData"></param>
        private void SetGenericStats(MonsterData monsterData)
        {
            LblName.Text = $"{monsterData.name}";
            LblSizeAlignment.Text = $"{monsterData.size.ToString()} {monsterData.type}, {monsterData.alignment}";
            LblACHPSpeed.Text = $"Armor Class: {monsterData.armor_class.ToString()} \n HP: \n {monsterData.hit_points.ToString()} ({monsterData.hit_dice}) \n";

        }
        /// <summary>
        /// NEW: Sets the visibility, it used to list all the labels individually, probably didn't need a method for this but it works. 
        /// </summary>
        /// <param name="tf"></param>
        private void StatVisability(bool tf)
        {
            SLMonsterStats.IsVisible = tf;       
        }
        /// <summary>
        /// NEW: 
        /// Thanks to your help, I figured out how to loop through the special abilities array (if there was one) and display all of them.
        /// </summary>
        /// <param name="monsterData"></param>
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
        /// <summary>
        /// OLD: Only had the original 6 stats that come in the JSON.
        /// NEW: Added modifiers. The JSON didn't have the modifiers so i took the ability score minus 10 then divide by two (rounded down) for an ability modifier on dice rolls. 
        /// </summary>
        /// <param name="monsterData"></param>
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
            string sMod = (strMod > 0) ? $"+{strMod}" : strMod.ToString();
            string dMod = (dexMod > 0) ? $"+{dexMod}" : dexMod.ToString();
            string cMod = (conMod > 0) ? $"+{conMod}" : conMod.ToString();
            string iMod = (intMod > 0) ? $"+{intMod}" : intMod.ToString();
            string wMod = (wisMod > 0) ? $"+{wisMod}" : wisMod.ToString();
            string chMod = (charMod > 0) ? $"+{charMod}" : charMod.ToString();
            //assign each stat its score and modifier.
            LblStrength.Text = $"  STR    \n{monsterData.strength.ToString()} ({sMod})";
            LblDex.Text = $"  DEX    \n{monsterData.dexterity.ToString()} ({dMod})";
            LblCon.Text = $"  CON    \n{monsterData.constitution.ToString()} ({cMod})";
            LblInt.Text = $"  INT    \n{monsterData.intelligence.ToString()} ({iMod})";
            LblWis.Text = $"  WIS    \n{monsterData.wisdom.ToString()} ({wMod})";
            LblChar.Text = $"  CHAR    \n{monsterData.charisma.ToString()} ({chMod})";

        } 
        /// <summary>
        /// OLD: Armor class, walking speed only
        /// NEW: 
        /// Checks to see what kinds of speed the monster has and displays all of them. 
        /// </summary>
        /// <param name="monsterData"></param>
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
        /// <summary>
        /// NEW:
        /// opens a modal to pick the monster. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPickMonster_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PickMonster());
        }
        /// <summary>
        /// NEW:
        /// This is for after the user picks a monster from the picker on the modal. It does almost the same thing as the  random BtnGetMonster_Clicked(). I'm thinking of rewriting the code to be able to handle both.
        /// </summary>
        /// <param name="index"></param>
        public void GetSelectedMonster(string index)
        {
            LblActions.IsVisible = false;
            SLActions.Children.Clear();
            SLLegActions.Children.Clear();
            SLSpecialAbilities.Children.Clear();
            BtnLoadMore.Text = "LOAD ABILITIES AND ACTIONS";
            using (WebClient wc = new WebClient())
            {
                
                string jsonData = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/{index}/");

                try
                {
                    //deserialize JSON
                   monsterData = JsonConvert.DeserializeObject<MonsterData>(jsonData);

                    SetGenericStats(monsterData);
                    SetACHPSpeed(monsterData);
                    SetStats(monsterData);
                    
                    SetProficiencies(monsterData);
                    SetImmunitiesSenses(monsterData);
                    SetLanguagesCR(monsterData);

                    StatVisability(true);

                }
                catch (Exception ex)
                {
                    DisplayAlert("Oh no!", $"Some goblins must have sabotaged the server! \n ({ex.Message})", "Close");
                }
            }
        }
        /// <summary>
        /// NEW: this is a result of me trying to figure out why the text wont load off the screen. If you click it twice it works. I think it's way above my head and i've worked on it for hours and google isn't helping so far. I tried to look up async await but couldnt get that to work either. So here's my hack job attempt so you at least can see all the data I've worked hard to display!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadMore_Clicked(object sender, EventArgs e)
        {
            SetSpecialAbilities(monsterData);
            SetActions(monsterData);
            SetLegendaryActions(monsterData);
            BtnLoadMore.Text = "DIDN'T LOAD? CLICK AGAIN :(";
        }
    }
}
