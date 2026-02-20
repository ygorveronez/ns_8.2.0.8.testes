using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SGT.WebService.REST.Middleware
{
    public class ExibicaoMetodoFilter : IDocumentFilter
    {
        #region Atributos Privados

        private readonly Dictionary<string, List<string>> _modulosPermitidos;
        private readonly Dictionary<string, List<string>> _modulosBloqueados;
        private readonly HashSet<string> _modulosRestritos;

        #endregion Atributos Privados

        #region Construtores

        public ExibicaoMetodoFilter(IConfiguration configuration)
        {
            IEnumerable<IConfigurationSection> modulosConfig = configuration.GetSection("ModulosRestritos").GetChildren();

            _modulosPermitidos = new Dictionary<string, List<string>>();
            _modulosBloqueados = new Dictionary<string, List<string>>();
            _modulosRestritos = new HashSet<string>();

            foreach (IConfigurationSection configuracao in modulosConfig)
            {
                string modulo = configuracao.GetValue<string>("Modulo");
                List<string> metodosPermitidos = configuracao.GetSection("MetodosPermitidos").Get<List<string>>() ?? new List<string>();
                List<string> metodosBloqueados = configuracao.GetSection("MetodosBloqueados").Get<List<string>>() ?? new List<string>();

                _modulosPermitidos[modulo] = metodosPermitidos;
                _modulosBloqueados[modulo] = metodosBloqueados;
                _modulosRestritos.Add(modulo);
            }
        }

        #endregion Construtores

        #region Métodos Públicos

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription apiDescription in context.ApiDescriptions)
            {
                string url = apiDescription.RelativePath.TrimEnd('/');
                string[] partesUrl = url.Split('/');

                if (partesUrl.Length < 2)
                    continue;

                string nomeModulo = partesUrl[0];
                string nomeMetodo = partesUrl[1];

                if (!_modulosRestritos.Contains(nomeModulo))
                    continue;

                if (_modulosPermitidos.TryGetValue(nomeModulo, out var metodosPermitidos) && metodosPermitidos.Count > 0 && !metodosPermitidos.Contains(nomeMetodo))
                {
                    swaggerDoc.Paths.Remove($"/{url}");
                    continue;
                }

                if (_modulosBloqueados.TryGetValue(nomeModulo, out var metodosBloqueados) && metodosBloqueados.Contains(nomeMetodo))
                    swaggerDoc.Paths.Remove($"/{url}");
            }
        }

        #endregion Métodos Públicos
    }
}
