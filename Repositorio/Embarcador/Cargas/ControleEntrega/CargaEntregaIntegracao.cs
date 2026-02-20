using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao>
    {
        public CargaEntregaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao> BuscarIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result
                .Fetch(obj => obj.Carga)
                .Skip(0).Take(maximoRegistros).ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }



        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string codigocarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = retornaConsulta(dataInicio, dataFim, situacao, codigocarga);

            //if (!string.IsNullOrWhiteSpace(propOrdena))
            //    result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string codigocarga)
        {
            var result = retornaConsulta(dataInicio, dataFim, situacao, codigocarga);

            return result.Count();
        }


        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao> retornaConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string CodigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao <= dataFim);


            if (!string.IsNullOrEmpty(CodigoCarga))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == CodigoCarga);

            return result.Fetch(obj => obj.Carga).Fetch(obj => obj.TipoIntegracao);
        }

        #endregion

    }
}
