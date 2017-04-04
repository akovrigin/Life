using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace LifeHost
{
    class Program
    {
        private static Timer _timer;

        static void Main(string[] args)
        {
            var isAlive = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsAlive"));

            if (isAlive)
            {
                Player player = new Player();
                var rnd = new Random(DateTime.Now.Millisecond);
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 100; i++)
                        Sanctuary.Populate(PresetType.Pentadecathlon, player, rnd.Next(1024), rnd.Next(800));
                    player = new Player();
                }
                Sanctuary.Populate(PresetType.Pentadecathlon, player, rnd.Next(1024), rnd.Next(800));
                World.Instance.TryToPopulate();
            }

            _timer = new Timer(1000);

            _timer.Elapsed += (sender, timerArgs) =>
            {
                World.Instance.Step();
                Console.WriteLine($"Step: {World.Instance.StepCount}, Population: {World.Instance.CreatureCount}");
            };

            _timer.Start();

            var url = ConfigurationManager.AppSettings.Get("server");

            if (string.IsNullOrEmpty(url))
                url = "http://localhost:5555";

            var selfHostConfiguraiton = new HttpSelfHostConfiguration(url);

            selfHostConfiguraiton.Routes.MapHttpRoute(
                name: "SendDataApiRoute",
                routeTemplate: "api/{controller}/{action}/{data}",
                defaults: null
            );

            selfHostConfiguraiton.Routes.MapHttpRoute(
                name: "ActionApiRoute",
                routeTemplate: "api/{controller}/{action}",
                defaults: null
            );

            selfHostConfiguraiton.Routes.MapHttpRoute(
                name: "IndexApiRoute",
                routeTemplate: "",
                defaults: new { controller = "Life", action = "Index" }
            );

            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.WriteLine($"Server started: {url}");
                Console.WriteLine();
                Console.ReadLine();
                server.CloseAsync().Wait();
            }
        }
    }
}
