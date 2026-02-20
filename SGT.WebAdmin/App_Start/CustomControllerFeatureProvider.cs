//using System.Reflection;
//using Microsoft.AspNetCore.Mvc.Controllers;

//namespace SGT.WebAdmin;

//public class CustomControllerFeatureProvider : ControllerFeatureProvider
//{
//    protected override bool IsController(TypeInfo typeInfo)
//    {
//        if (base.IsController(typeInfo))
//        {
//            // Verifica se o namespace contém "Areas"
//            var namespaceSegments = typeInfo.Namespace?.Split('.');
//            if (namespaceSegments != null && namespaceSegments.Contains("Areas"))
//            {
//                return true;
//            }
//        }
//        return false;
//    }
//}
