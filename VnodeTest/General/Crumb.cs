using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.General
{
    public class Crumb
    {
        public string Name { get; }
        public Rendermode Rendermode { get; set; }
        public LocalRendermode LocalRendermode { get; set; }

        public Crumb(string name, Rendermode rendermode, LocalRendermode localRendermode)
        {
            Name = name;
            Rendermode = rendermode;
            LocalRendermode = localRendermode;
        }

       
    }
}
