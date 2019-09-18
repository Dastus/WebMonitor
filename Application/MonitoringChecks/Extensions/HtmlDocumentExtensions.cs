using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Monitor.Application.MonitoringChecks.Extensions
{
    public static class HtmlDocumentExtensions
    {
        public static string GetTitle(this HtmlDocument htmlResult)
        {
            var title = htmlResult.DocumentNode.Descendants("title").FirstOrDefault();
            var titleEncoded = title?.InnerText;
            return System.Net.WebUtility.HtmlDecode(titleEncoded);
        }

        public static string GetMetaTagContent(this HtmlDocument htmlResult, string tagName)
        {
            var metaNodes = htmlResult.DocumentNode.Descendants("meta").ToList();
            var targetNode = metaNodes.Find(node => (node.HasAttributes &&
                (node.Attributes?.Where(a => a.Name == "name" && a.Value == tagName)).Count() > 0));
            var content = targetNode?.Attributes.Where(a => a.Name == "content").FirstOrDefault()?.Value;

            return content;
        }

        public static string GetLinkContent(this HtmlDocument htmlResult, string rel)
        {
            var metaNodes = htmlResult.DocumentNode.Descendants("link").ToList();
            var targetNode = metaNodes.Find(node => (node.HasAttributes &&
                (node.Attributes?.Where(a => a.Name == "rel" && a.Value == rel)).Count() > 0));
            var content = targetNode?.Attributes.Where(a => a.Name == "href").FirstOrDefault()?.Value;

            return content;
        }

        public static List<HtmlNode> FindNodesByName(this HtmlDocument htmlResult, string nodeName)
        {
            var nodes = htmlResult.DocumentNode.Descendants(nodeName).ToList();
            return nodes;
        }

        public static List<HtmlNode> FindNodesByClassName(this HtmlDocument htmlResult, string nodeName, string className)
        {
            var nodes = htmlResult.DocumentNode.Descendants(nodeName)
                .Where( e => e.GetAttributeValue("class", "") == className).ToList();
            return nodes;
        }
    }
}
