using FChatSharpLib.Entities.Plugin;
using Microsoft.Extensions.DependencyInjection;
using RDVFSharp.DataContext;
using RDVFSharp.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RDVFSharp
{
    public class RendezvousFighting : BasePlugin
    {

        public Battlefield CurrentBattlefield { get; set; }
        public TeamBattlefield CurrentTeamBattlefield { get; set; }
        public Fighter FirstFighter { get; set; }
        public Fighter SecondFighter { get; set; }
        public TeamFighter FirstTeamFighter { get; set; }
        public TeamFighter SecondTeamFighter { get; set; }
        public TeamFighter ThirdTeamFighter { get; set; }
        public TeamFighter FourthTeamFighter { get; set; }


        public RDVFDataContext Context
        {
            get
            {
                return ServiceProvider.GetService<RDVFDataContext>();
            }
        }
        public ServiceProvider ServiceProvider { get; set; }

        public RendezvousFighting(ServiceProvider serviceProvider, List<string> channels, bool debug = false, Battlefield currentBattlefield = null) : base(channels, debug)
        {
            ServiceProvider = serviceProvider;
            ResetFight(currentBattlefield);
        }

        public void ResetFight(Battlefield currentBattlefield = null)
        {
            if(currentBattlefield != null)
            {
                CurrentBattlefield = currentBattlefield;
            }
            else
            {
                CurrentBattlefield = new Battlefield(this);
            }
            
            FirstFighter = null;
            SecondFighter = null;
        }
    }
}
