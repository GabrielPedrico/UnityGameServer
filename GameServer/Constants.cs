using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    class Constants // Classe para definir os ticks por segundo do server
    {
        public const int TICKS_PER_SEC = 30; // aqui é definido os ticks por segundo
        public const float MS_PER_TICK = 1000f / TICKS_PER_SEC;
    }
}
