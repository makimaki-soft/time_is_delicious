using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RuleManager
{
    public class GameComponent
    {
        public Guid GUID { get; } = Guid.NewGuid();
    }
}
