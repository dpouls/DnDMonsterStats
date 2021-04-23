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
            LblName.IsVisible = false;
            LblSize.IsVisible = false;
            LblType.IsVisible = false;
            LblAC.IsVisible = false;
            LblHP.IsVisible = false;
            LblSpeed.IsVisible = false;
            LblAlignment.IsVisible = false;
            LblStats.IsVisible = false;
            LblStrength.IsVisible = false;
            LblDex.IsVisible = false;
            LblCon.IsVisible = false;
            LblInt.IsVisible = false;
            LblWis.IsVisible = false;
            LblChar.IsVisible = false;
            LblSA.IsVisible = false;
            LblSADesc.IsVisible = false;
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
                
                try
                {
                    //JObject parsedJson = JObject.Parse(jsonData);
                    //LblName.Text = parsedJson["name"].ToString();
                    //I went with creating a class and deserializing the JSON
                    MonsterData monsterData = JsonConvert.DeserializeObject<MonsterData>(jsonData);
                    LblName.Text = $"{monsterData.name}";

                    LblSize.Text = $"Size: \n{monsterData.size.ToString()}";
                    LblType.Text = $"Type: \n{monsterData.type}";
                    LblAC.Text = $"Armor Class: \n{monsterData.armor_class.ToString()}";
                    LblHP.Text = $"HP: \n {monsterData.hit_points.ToString()} or {monsterData.hit_dice}";
                    if (monsterData.speed.walk != null)
                    {
                    LblSpeed.Text = $"Speed: \n{monsterData.speed.walk}";
                    } else
                    {
                        LblSpeed.Text = $"Speed: \n No Data";
                    }
                    LblAlignment.Text = $"Alignment: \n{monsterData.alignment}";
                    LblStrength.Text = $"Strength: \n{monsterData.strength.ToString()}";
                    LblDex.Text = $"Dexterity: \n{monsterData.dexterity.ToString()}";
                    LblCon.Text = $"Constitution: \n{monsterData.constitution.ToString()}";
                    LblInt.Text = $"Intelligence: \n{monsterData.intelligence.ToString()}";
                    LblWis.Text = $"Wisdom: \n{monsterData.wisdom.ToString()}";
                    LblChar.Text = $"Charisma: \n{monsterData.charisma.ToString()}";
                    if (monsterData.special_abilities[0] != null)
                    {
                        LblSA.IsVisible = true;
                        LblSADesc.IsVisible = true;
                        LblSA.Text = $"Special Ability: {monsterData.special_abilities[0].name}";
                        LblSADesc.Text = $"{monsterData.special_abilities[0].desc}";
                    } else
                    {
                        LblSA.IsVisible = true;
                        LblSA.Text = "No Special Abilities.";
                        LblSADesc.IsVisible = false;
                    }
                    LblName.IsVisible = true;
                    LblSize.IsVisible = true;
                    LblType.IsVisible = true;
                    LblAC.IsVisible = true;
                    LblHP.IsVisible = true;
                    LblSpeed.IsVisible = true;
                    LblAlignment.IsVisible = true;
                    LblStats.IsVisible = true;
                    LblStrength.IsVisible = true;
                    LblDex.IsVisible = true;
                    LblCon.IsVisible = true;
                    LblInt.IsVisible = true;
                    LblWis.IsVisible = true;
                    LblChar.IsVisible = true;
                    
                } 
                catch (Exception ex)
                {
                    DisplayAlert("Oh no!",$"Some goblins must have sabotaged the server! \n ({ex.Message})", "Close");
                }
            }
        }
    }
}
