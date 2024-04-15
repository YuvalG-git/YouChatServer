using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YouChatServer.CaptchaHandler;
using YouChatServer.JsonClasses;

namespace YouChatServer
{
    //internal class ClientCaptchaRotationImagesAttemptsState
    //{
    //    private Client Client;
    //    public bool IsBanned { get; set; }
    //    public double CurrentBanDuration { get; set; }
    //    private Queue<double> BanDuration;
    //    private readonly System.Timers.Timer Timer = new System.Timers.Timer();
    //    private TimeSpan TimerTickTimeSpan;
    //    private TimeSpan CountDownTimeSpan;

    //    private int score;
    //    private const int attempts = 5;
    //    public ClientCaptchaRotationImagesAttemptsState(Client client)
    //    {
    //        Client = client;
    //        IsBanned = false;
    //        CurrentBanDuration = 0;
    //        score = 0;
    //        BanDuration = new Queue<double>();
    //        double[] WaitingTimeArray = { 0.25, 0.5, 1, 2, 3, 5, 10, 15, 20, 30 };
    //        foreach (double timePeriod in WaitingTimeArray)
    //        {
    //            BanDuration.Enqueue(timePeriod);
    //        }
    //        Timer.Interval = 1000;
    //        Timer.Elapsed += Timer_Tick;
    //        TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval);

    //    }
    //    private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
    //    {

    //        CountDownTimeSpan -= TimerTickTimeSpan;
    //        if (CountDownTimeSpan.TotalMilliseconds <= 0)
    //        {
    //            Timer.Stop();
    //            IsBanned = false;
    //            CaptchaRotationImageDetails captchaRotationImageDetails = Client.GetCaptchaRotationImageDetails(score, attempts);
    //            JsonObject banMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.LoginBanFinish, captchaRotationImageDetails);
    //            string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
    //            {
    //                TypeNameHandling = TypeNameHandling.Auto
    //            });
    //            Client.SendMessage(banMessageJson);
    //        }
    //    }

    //    public void HandleBan(int score) 
    //    {
    //        this.score = score;
    //        IsBanned = true;
    //        if (BanDuration.Count > 0)
    //        {
    //            CurrentBanDuration = BanDuration.Dequeue();
    //        }
    //        CountDownTimeSpan = TimeSpan.FromMinutes(CurrentBanDuration);

    //        Timer.Start();
    //    }
    //}
}
