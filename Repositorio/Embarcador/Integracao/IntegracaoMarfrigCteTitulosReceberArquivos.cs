using System.Collections.Generic;
using System.Linq;
namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoMarfrigCteTitulosReceberArquivos : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo>
    {
        public IntegracaoMarfrigCteTitulosReceberArquivos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo> BuscarArquivosPorIntergacao(long codigo, int inicio, int limite)
        {
            var queryIntegracao = this.SessionNHiBernate.Query <Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();
            var resultIntegracao = from obj in queryIntegracao where obj.Codigo == codigo select obj;

            var querIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo>();
            var resultIntegracaoArquivo = from obj in querIntegracaoArquivo where resultIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

    }
}
