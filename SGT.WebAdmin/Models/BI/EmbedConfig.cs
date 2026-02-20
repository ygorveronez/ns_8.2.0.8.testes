using Microsoft.PowerBI.Api.Models;
using System;

namespace SGT.WebAdmin.Models.BI
{
    public class EmbedConfig
    {
        public string Id { get; set; }

        public string EmbedUrl { get; set; }

        public EmbedToken EmbedToken { get; set; }

        public int MinutesToExpiration
        {
            get
            {
                if (EmbedToken == null)
                    return 0;

                return (int)(EmbedToken.Expiration - DateTime.UtcNow).TotalMinutes;
            }
        }

        public bool? IsEffectiveIdentityRolesRequired { get; set; }

        public bool? IsEffectiveIdentityRequired { get; set; }

        public bool EnableRLS { get; set; }

        public string Username { get; set; }

        public string Roles { get; set; }

        public string ErrorMessage { get; internal set; }
    }
}
