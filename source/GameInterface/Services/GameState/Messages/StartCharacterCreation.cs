﻿using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.Services.GameState.Messages
{
    public readonly struct StartCreateCharacter : ICommand
    {
    }

    public readonly struct CharacterCreationFinished : IEvent
    {
    }
}
