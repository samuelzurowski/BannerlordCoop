﻿using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.Settlements.Messages;

/// <summary>
/// Notifies game interface that a change needs to happen.
/// </summary>
public record ChangeSettlementEnemiesSpotted : ICommand
{
    public string SettlementId { get; }
    public float NumberOfEnemiesSpottedAround { get; }

    public ChangeSettlementEnemiesSpotted(string settlementId, float numberOfEnemiesSpottedAround)
    {
        SettlementId = settlementId;
        NumberOfEnemiesSpottedAround = numberOfEnemiesSpottedAround;
    }
}