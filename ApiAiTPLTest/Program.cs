using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiAiSDK;
using System.Diagnostics;

namespace ApiAiTPLTest
{
    class Program
    {
        private const string SUBSCRIPTION_KEY = "cb9693af-85ce-4fbf-844a-5563722fc27f";
        private const string ACCESS_TOKEN = "3485a96fb27744db83e78b8c4bc9e7b7";

        static void Main(string[] args)
        {            
            var config = new AIConfiguration(SUBSCRIPTION_KEY, ACCESS_TOKEN, SupportedLanguage.English);
            var apiAi = new ApiAi(config);
            string rl = "";

            while (rl == "")
            {
                var sw = Stopwatch.StartNew();
                var response = apiAi.TextRequest("hello");
                sw.Stop();

                Console.WriteLine("api sync answer: {0}, time: {1} ms", response.Result.Action, sw.ElapsedMilliseconds.ToString());

                sw = Stopwatch.StartNew();
                apiAi.TextRequestStart("hello");
                System.Threading.Thread.Sleep(250);
                response = apiAi.TextRequestFinish();
                sw.Stop();

                Console.WriteLine("api async answer: {0}, time: {1} ms", response.Result.Action, sw.ElapsedMilliseconds.ToString());


                rl = Console.ReadLine();
            }
        }
    }
}
