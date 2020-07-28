using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public static class HtmlHelperExtensions
    {
        public static AssetsHelper Assets(this IHtmlHelper htmlHelper)
        {
            return AssetsHelper.GetInstance(htmlHelper);
        }
    }

    public class AssetsHelper
    {
        public static AssetsHelper GetInstance(IHtmlHelper htmlHelper)
        {
            var instanceKey = "AssetsHelperInstance";

            var context = htmlHelper.ViewContext.HttpContext;
            if (context == null) return null;

            var assetsHelper = (AssetsHelper)context.Items[instanceKey];

            if (assetsHelper == null)
                context.Items.Add(instanceKey, assetsHelper = new AssetsHelper());

            return assetsHelper;
        }

        public ItemRegistrar Styles { get; private set; }
        public ItemRegistrar Scripts { get; private set; }

        public AssetsHelper()
        {
            Styles = new ItemRegistrar(ItemRegistrarFormatters.StyleFormat);
            Scripts = new ItemRegistrar(ItemRegistrarFormatters.ScriptFormat);
        }
    }

    public class ItemRegistrar
    {
        private readonly string _format;
        private readonly IList<string> _items;

        public ItemRegistrar(string format)
        {
            _format = format;
            _items = new List<string>();
        }

        public ItemRegistrar Add(string url)
        {
            if (!_items.Contains(url))
                _items.Add(url);

            return this;
        }

        public HtmlString Render()
        {
            var sb = new StringBuilder();

            foreach (var item in _items)
            {
                var fmt = string.Format(_format, item);
                sb.AppendLine(fmt);
            }

            return new HtmlString(sb.ToString());
        }
    }

    public class ItemRegistrarFormatters
    {
        public const string StyleFormat = "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />";
        public const string ScriptFormat = "<script src=\"{0}\" type=\"text/javascript\"></script>";
    }

    public static class HttpHelperExtensions
    {
        public static int GetSessionId(this int? id)
        {
            if (id == null)
            {
                id = 1;
            }
            return Convert.ToInt32(id);
        }

        public static Type HeuristicallyDetermineType(this IList myList)
        {
            var enumerable_type =
                myList.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GenericTypeArguments.Length == 1)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerable_type != null)
                return enumerable_type.GenericTypeArguments[0];

            if (myList.Count == 0)
                return null;

            return myList[0].GetType();
        }


        public static List<Words> ToWords(this IEnumerable<dynamic> dynamics)
        {
            var list = new List<Words>();
            foreach (IDictionary<string, object> row in dynamics)
            {
                list.Add(new Words { Id = Convert.ToInt32(row.Values.ElementAt(0)), Text = row.Values.ElementAt(1).ToString() });
            }
            return list;
        }
    }


}
