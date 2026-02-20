using Microsoft.AspNetCore.Authorization;

namespace SGT.WebAdmin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AllowAnonymousAttribute: Attribute, IAllowAnonymous
    {
        public AllowAnonymousAttribute() { }
    }
}
