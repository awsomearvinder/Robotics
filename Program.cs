using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http.Headers;
using System.Globalization;
using Newtonsoft.Json;
using GmailLib;
using New_Folder.DiscordClasses;
namespace GmailQuickstart
{
    class Program 
    {
        /*
            GOALS: 
                [X]Find a way to filter out time and date
                [X]Make discord bot portion
                [X]Add way to know which messages have been sent and which haven't, (Perhaps some method through gmail API with something like folders?)
                []Less hardcoding, ability to be used in other scenarios (Discord portion done, Gmail side still hardcoded.)
                []Multiserver support?
                [X]Make better classes (for neater code)
                [] Make Code clean enough for non-programmers to understand and not a giant fucking mess.
        */

        //List of messages that have already been sent
        public static List<string> SentMessages =new List<string> {};
        private static void Main(string[] args) 
        {
            //If the save file for sent messages exists, read it, otherwise assume initial run and start without loading it.
            if (File.Exists(@"data.bin")){
                //Loads in List to SentMessages
                Load<List<string>>("data.bin",out SentMessages);
            }
            //Calls async function MainBotLogic
            MainBotLogic().Wait();
        }

        private static async Task MainBotLogic()
        {
            List<string> EmailIdsToSend = new List<string>();
            List<Embed> EmbedsToSend = new List<Embed>();
             while (true)
            {
                //Gets emails, First part of string array has Email, second is email ID (gross but it works)
                List<string[]> Emails = GmailLib.GmailFunctions.GetEmails();
                //for every email, check if its sent, if it isn't, send the date.
                foreach(string[] Email in Emails)
                {
                    //Date and place is on 5th line (arrays start at 0 whew), array indexed by lines, replace with regex later.
                    string DateForRoboticsStr = Email[0].Split("\n\r")[4];
                    if (!SentMessages.Contains(Email[1]))
                    {   
                        /*
                        TODO: Years change.
                         */
                        DateTime DateForRoboticsDateTimeObj;
                        DateForRoboticsDateTimeObj = ConvertToDateTimeObj(DateForRoboticsStr);
                        //if the date for robotics meeting has not passed and the date is only 1 day away, add date and time to embeds to send.
                        if(DateForRoboticsDateTimeObj>DateTime.Now&&DateForRoboticsDateTimeObj-DateTime.Now<new TimeSpan(1,0,0,0))
                        {
                            EmbedsToSend.Add(new Embed {
                                    title = "There is a Upcoming Robotics Meeting!",
                                    description = "Date/Time: "+ DateForRoboticsStr,
                                    color = 16731254
                                });
                            Console.WriteLine("Date/Time: " + DateForRoboticsStr);
                            //Acts as a buffer for emails that are going to be sent, but not yet sent.
                            EmailIdsToSend.Add(Email[1]);
                        }
                    }
                }
                //Make and send webhook
                Webhook WebhookToSend = new Webhook()
                {
                    username = "Nick",
                    embeds = EmbedsToSend,
                    avatar_url = "https://cdn.discordapp.com/avatars/453944196307615746/9a0bf2d580b7efa4f676196fb638c362.png?size=256"
                };
                //Sends message
                var response = await WebhookToSend.SendWebhook(ReadConfig());
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Message Succesfully sent!");
                }
                else
                {
                    Console.WriteLine("Discord's return Json: \n" + response);
                }
                //Adds EmailIds as sent.
                SentMessages.AddRange(EmailIdsToSend);
                Save<List<string>>(SentMessages, "data.bin");
                EmailIdsToSend.Clear();
                EmbedsToSend.Clear();
                System.Threading.Thread.Sleep(1000*3600);
                //restart
            }
        }
        private static void Save<T>(T ObjectToSave, string ObjectPath){
            using (Stream stream = File.Open(ObjectPath, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, ObjectToSave);
            }
        }
        private static void Load<T>(string ObjectPath, out T OutputValue)
        {
            using (Stream stream = File.Open(ObjectPath, FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();
                OutputValue = (T)bin.Deserialize(stream);
            }
        }
        private static string ReadConfig()
        {
            return File.ReadAllText(@"Config.txt");
        }
        private static DateTime ConvertToDateTimeObj(string DateForRoboticsStr)
        {
            //Time starts at char 1 and ends at char 10, sent in format 3 letter day, number day, month abbreviation. Culture dosen't matter.
            return DateTime.ParseExact(DateForRoboticsStr.Substring(1,10).Replace("-"," "),"ddd dd MMM",System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
