using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzgulOguz.Challenges
{
    public interface IChallenge
    {
        void Run(Input input, Output output);
    }
}
