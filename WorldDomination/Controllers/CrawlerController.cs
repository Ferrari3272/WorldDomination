using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldDomination.Models;

namespace WorldDomination.Controllers
{
    public class CrawlerController
    {
        public Tree GetTree(string url, int depth)
        {
            var tree = new Tree();

            var root = new Node();
            root.Url = url;
            root.Depth = 0;
            root.Parent = null;

            tree.Nodes.Add(root);

            for (int i = 0; i < depth; i++)
            {
                var nodes = tree.Nodes.Where(x => x.Depth == i).ToList();
                foreach (var node in nodes)
                {
                    AddChilds(node, tree);
                }
            }

            return tree;
        }

        private void AddChilds(Node parent, Tree tree)
        {
            var childs = new List<Node>();

            var links = GetLinks(parent.Url);

            if (links != null && links.Count > 0)
            {
                foreach (var link in links)
                {
                    var child = new Node(link, parent, parent.Depth + 1);
                    parent.AddChild(child);
                    tree.Nodes.Add(child);
                }
            }
        }

        private List<string> GetLinks(string url)
        {
            try
            {
                var links = new List<string>();

                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = node.Attributes["href"];
                    var link = att.Value;
                    if (link.Contains("http") && !link.Contains(url))
                    {
                        links.Add(link);
                    }
                }

                return links;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
