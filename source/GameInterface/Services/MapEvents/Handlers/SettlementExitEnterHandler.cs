﻿using Common.Messaging;
using Common.Network;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.MapEvents.Handlers
{
    /// <summary>
    /// Handles changes to parties for settlement entry and exit.
    /// </summary>
    public class SettlementExitEnterHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public SettlementExitEnterHandler(IMessageBroker messageBroker, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.network = network;

            messageBroker.Subscribe<SettlementEnterAllowed>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<SettlementEnterAllowed>(Handle);
        }

        private void Handle(MessagePayload<SettlementEnterAllowed> obj)
        {
            MobileParty mobileParty = null;
            foreach (MobileParty party in MobileParty.All)
            {
                if (party.StringId == obj.What.StringId)
                {
                    mobileParty = party;
                }
                
            }

            Settlement settlement = Settlement.Find(obj.What.StringId);

            EncounterManager.StartSettlementEncounter(mobileParty, settlement);
        }
    }
}
