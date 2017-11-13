using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldDomination.Models
{
    public class Node
    {
        public string Url { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }
        public List<Node> Childs { get; set; }

        public Node()
        {
            this.Childs = new List<Node>();
        }
        public Node(string url, Node parent, int depth)
        {
            this.Url = url;
            this.Parent = parent;
            this.Depth = depth;
            this.Childs = new List<Node>();
        }

        public void AddChild(Node node)
        {
            node.Parent = this;
            Childs.Add(node);
        }
    }
}
