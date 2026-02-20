using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteValoresIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao>
    {
        public ContratoFreteValoresIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao> BuscarPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigo select obj;

            return result.OrderByDescending(x =>x.DataIntegracao).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryContratoFreteValoresIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao>();
            var resultContratoFreteValoresIntegracao = from obj in queryContratoFreteValoresIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultContratoFreteValoresIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryContratoFreteValoresIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao>();
            var resultContratoFreteValoresIntegracao = from obj in queryContratoFreteValoresIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultContratoFreteValoresIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

    }
}
