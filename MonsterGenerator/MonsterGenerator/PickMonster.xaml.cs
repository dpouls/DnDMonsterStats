using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MonsterGenerator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickMonster : ContentPage
    {
        List<string> monsterNames = new List<string>();
        MonsterList monsterList = new MonsterList();


        /// <summary>
        /// loads the monsters into the picker, auto selects the first monster. 
        /// </summary>
        public PickMonster()
        {
            InitializeComponent();
            LoadMonsters();
            PckerMonsters.SelectedIndex = 0;
        }
        /// <summary>
        /// NEW:
        /// Makes an api call for a list of the monsters in the database. (Over 300).
        /// Adds them to a picker.
        /// </summary>
        private void LoadMonsters()
        {
            using (WebClient wc = new WebClient())
            {
                string monsterListJson = wc.DownloadString($"https://www.dnd5eapi.co/api/monsters/");
               // JObject parsedMonsterList = JObject.Parse(monsterListJson);
                try
                {
                     monsterList = JsonConvert.DeserializeObject<MonsterList>(monsterListJson);
                    for (int i = 0; i < monsterList.results.Length; i++)
                    {
                        monsterNames.Add(monsterList.results[i].name);
                    }
                    PckerMonsters.ItemsSource = monsterNames;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Oops", $"Problem loading monster names, \n {ex.Message}", "Close");

                }
            }
        }
        /// <summary>
        /// 
        //verifies something was selected, assigns its index (JSON key called index) to the SelectedMonster class property called SelectedMonsterIndex. This will be used when the mainpage reloads after we close the modal.
        //closes the modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectMonster_Clicked(object sender, EventArgs e)
        {
           
            if (PckerMonsters.SelectedIndex > -1)
            {
                SelectedMonster.SelectedMonsterIndex = monsterList.results[PckerMonsters.SelectedIndex].index;
                MainPage mp = new MainPage();
                Application.Current.MainPage.Navigation.PopModalAsync();
            } else
            {
                DisplayAlert("Oops", "Please select a monster before continuing", "Close");
            }
        }
        /// <summary>
        /// closes the modal without selecting anything. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopModalAsync();

        }
    }
}