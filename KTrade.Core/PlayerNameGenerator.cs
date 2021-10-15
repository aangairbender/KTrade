using System;
using System.Collections.Generic;

namespace KTrade.Core
{
    public class PlayerNameGenerator
    {
        private readonly List<string> _names = new List<string>
        {
            "БРАНОЛИК",
            "НАРГАН",
            "ЭТЕЛЬРИС",
            "ИЛЬФОНИ",
            "РОМЕР",
            "ИЛВУД",
            "АЛЬБOАН",
            "ИЗЕНМА",
            "ФРЕДАНА",
            "КЕРУНА",
            "ИПУЛЬФА",
            "ГРИУРИ",
            "АЛЬБИНИО",
            "НАОЛЬ"
        };

        private int _cur;
        private int _overs;

        public PlayerNameGenerator()
        {
            var rand = new Random();
            for (int i = 0; i < _names.Count; ++i)
            {
                var j = rand.Next(_names.Count);

                var tmp = _names[i];
                _names[i] = _names[j];
                _names[j] = tmp;
            }

            _cur = _overs = 0;
        }

        public string NextName()
        {
            var name = _names[_cur];
            if (_overs > 0)
                name += $"({_overs})";
            _cur += 1;
            if (_cur == _names.Count)
            {
                _cur = 0;
                _overs += 1;
            }

            return name;
        }
    }
}