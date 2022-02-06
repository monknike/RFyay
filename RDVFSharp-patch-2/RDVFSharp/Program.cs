using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace RDVFSharp
{
    class Program
    {
        public static RendezvousFighting RDV { get; set; }
        public static bool IsDebugging { get; set; } = false;

        static void Main()
        {

            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: false)
                                .Build();

            services.AddDbContext<DataContext.RDVFDataContext>(optionsBuilder => optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            var serviceProvider = services.BuildServiceProvider();

#if DEBUG
            var flistUsername = "monknike2000";
            var flistPassword = "madhusudan1965";
            var botCharacterName = "RF Alpha"; //The character used to log in with
            var adminName = "Mayank";
            var channelToWatch = new List<string>() { "adh-3ccb421a7495d4f72097" }; //Your testing channel code, obtainable with /code in the chat
            var bot = new FChatSharpLib.Bot(flistUsername, flistPassword, botCharacterName, adminName, true, 10000);
            bot.Connect();

            RDV = new RendezvousFighting(serviceProvider, channelToWatch, IsDebugging);
            RDV.Run();
#else
            var channelToWatch = new List<string>() { "adh-f07a7597de794247bd0f" };
            RDV = new RendezvousFighting(serviceProvider, channelToWatch, IsDebugging);
            RDV.Run();
#endif
        }
    }
}
