using System;
using System.Collections.Generic;
using Infrastructure.Services.Cache;

namespace Servicos.Cache
{
    public class TokenSettingsService
    {
        private const string Key = "TokensSettings";

        public List<TokenSettings> Obter()
        {
            return CacheProvider.Instance.GetOrCreate(
                Key, 
                () => new List<TokenSettings>(), 
                TimeSpan.FromHours(1)
            );
        }
    }
}
