using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAMFaceEcomm.Models
{
    public static class GroupEnumerable
    {
        public static IList<Category> BuildTree(this IEnumerable<Category> source)
        {
            var groups = source.GroupBy(i => i.ParentCategory);

            var roots = groups.FirstOrDefault(g => g.Key.HasValue == false).ToList();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }

            return roots;
        }

        private static void AddChildren(Category node, IDictionary<int, List<Category>> source)
        {
            if (source.ContainsKey(node.Id))
            {
                node.Categories1 = source[node.Id];
                var _Data = node.Categories1.ToArray();
                int i = 0;
                foreach (var x in _Data.ToList())
                {
                    AddChildren(_Data[i], source);
                    i++;
                }
            }
            else
            {
                node.Categories1 = new List<Category>();
            }
        }
    }
}