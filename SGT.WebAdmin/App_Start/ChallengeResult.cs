//using Microsoft.Owin.Security;
//
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.App_Start
//{
//    public class ChallengeResult : HttpUnauthorizedResult
//    {
//        private const string XsrfKey = "PulaBoiPulaCavaloPulaCavaloEBoi";

//        public ChallengeResult(string provider, string redirectUri)
//            : this(provider, redirectUri, null)
//        {
//        }

//        public ChallengeResult(string provider, string redirectUri, string userId)
//        {
//            LoginProvider = provider;
//            RedirectUri = redirectUri;
//            UserId = userId;
//        }

//        public string LoginProvider { get; set; }
//        public string RedirectUri { get; set; }
//        public string UserId { get; set; }

//        public override void ExecuteResult(ControllerContext context)
//        {
//            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
//            if (UserId != null)
//                properties.Dictionary[XsrfKey] = UserId;

//            var owin = context.HttpContext.GetOwinContext();
//            owin.Authentication.Challenge(properties, LoginProvider);
//        }
//    }
//}