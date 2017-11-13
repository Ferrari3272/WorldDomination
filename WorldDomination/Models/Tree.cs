using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldDomination.Models
{
    public class Tree
    {
        public List<Node> Nodes { get; set; }

        public Tree()
        {
            this.Nodes = new List<Node>();
        }
    }
}
