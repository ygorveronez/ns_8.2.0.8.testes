using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace SGT.WebAdmin.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ValidationBootstrap(this IHtmlHelper htmlHelper, string alertType = "danger",
            string heading = "", bool showCloseButton = true)
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"alert alert-{0} alert-dismissible mb-2 \">", alertType);

            if (showCloseButton)
                sb.Append("<button class=\"close\" data-dismiss=\"alert\"><span aria-hidden=\"true\">x</span></button>");

            if (!string.IsNullOrWhiteSpace(heading))
                sb.AppendFormat("<strong class=\"alert-heading\">{0}</strong>", heading);

            sb.Append(htmlHelper.ValidationSummary().GetHtmlString());
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

		public static string GetHtmlString(this IHtmlContent htmlContent)
		{
			using (StringWriter writer = new StringWriter())
			{
				htmlContent.WriteTo(writer, HtmlEncoder.Default);
				return writer.ToString();
			}
		}
	}
}