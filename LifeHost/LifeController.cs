using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using LifeHost.Properties;

namespace LifeHost
{
    public class LifeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var player = new Player();
            var color = "'" + ColorTranslator.ToHtml(player.Color) + "'";
            var html = Resources.ui.Replace("##color##", color).Replace("##player##", player.Id.ToString());

            var response = new HttpResponseMessage();
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private object _lockObject = new object();
        private object _lockObject2 = new object();

        private static Hashtable _nextStep = new Hashtable();

        private static int _lastWorldStepCount;
        private static string _lastWorld = "";
        private static int _lastCreatureCount;

        [HttpGet]
        public string GetNextStep(string data)
        {
            lock (_lockObject)
            {
                if (_nextStep.Contains(data))
                    return "";

                _nextStep.Add(data, true);
            }

            try
            {
                if (World.Instance.StepCount == _lastWorldStepCount && World.Instance.CreatureCount == _lastCreatureCount)
                    return _lastWorld;

                lock (_lockObject2)
                {
                    var dt = DateTime.Now;

                    var list = World.Instance.GetCreatures();

                    var result = list.Aggregate("", (current, c) =>
                        current + c.X + "," + c.Y + "," + ColorTranslator.ToHtml(c.Color) + "," + c.Owner.Id + ",");

                    result += ":";

                    var players = Player.Get().Where(p => p.Power > 0).OrderByDescending(p => p.Power);

                    result += players.Aggregate("", (current, p) =>
                        current + p.Id + "," + ColorTranslator.ToHtml(p.Color) + "," + p.Power + ",");

                    Console.WriteLine($"Return data for player {data} : {(DateTime.Now - dt).Milliseconds} ms");

                    _lastWorld = result;
                    _lastWorldStepCount = World.Instance.StepCount;
                    _lastCreatureCount = World.Instance.CreatureCount;
                }

                return _lastWorld;
            }
            finally
            {
                lock(_lockObject)
                    _nextStep.Remove(data);
            }
        }

        [HttpGet]
        public string SendData(string data)
        {
            var list = data.Split(',');

            var offsetX = Convert.ToInt32(list[0]);
            var offsetY = Convert.ToInt32(list[1]);
            var preset = (PresetType) Convert.ToInt32(list[2]);
            var player = Player.Get(Convert.ToInt32(list[3]));

            Sanctuary.Populate(preset, player, offsetX, offsetY);

            World.Instance.TryToPopulate();

            Console.WriteLine($"Player {player.Id} add the {nameof(preset)}");

            // small optimisation for better response from the overcrowded world
            if (_lastCreatureCount < 1000) 
                return GetNextStep(player.Id.ToString());

            return "";
        }
    }
}