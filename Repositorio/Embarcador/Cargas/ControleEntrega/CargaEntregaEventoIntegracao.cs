using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaEventoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>
    {
        #region Construtores

        public CargaEntregaEventoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>()
                .Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> BuscarPorCodigoEmbarcadorCarga(string numeroCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>()
                .Where(o => o.CargaEntregaEvento.Carga.CodigoCargaEmbarcador == numeroCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));
            return query.FirstOrDefault();
        }

        public List<int> BuscarIntegracoesPorSituacao(int intervaloTempoRejeitadas, int limite)
        {
            int numeroTentativas = 3;

            StringBuilder sql = new();

            sql.Append(@$"SELECT SUB.INT_CODIGO
                         FROM (
                             SELECT CARGAENTRE0_.INT_CODIGO,
                                    CARGAENTRE0_.INT_DATA_INTEGRACAO
                             FROM T_CARGA_ENTREGA_EVENTO_INTEGRACAO CARGAENTRE0_              
                             WHERE  CARGAENTRE0_.INT_SITUACAO_INTEGRACAO = 0

                             UNION ALL

                             SELECT CARGAENTRE0_.INT_CODIGO,
                                    CARGAENTRE0_.INT_DATA_INTEGRACAO
                             FROM T_CARGA_ENTREGA_EVENTO_INTEGRACAO CARGAENTRE0_
                             WHERE CARGAENTRE0_.INT_SITUACAO_INTEGRACAO = 2
                               AND CARGAENTRE0_.INT_NUMERO_TENTATIVAS <= :numeroTentativas
                               AND CARGAENTRE0_.INT_DATA_INTEGRACAO <= :dataLimite
                         ) AS SUB
                         ORDER BY SUB.INT_DATA_INTEGRACAO ASC
                         OFFSET 0 ROWS FETCH NEXT :limite ROWS ONLY;
            ");

            var dataLimite = DateTime.Now.AddHours(-intervaloTempoRejeitadas);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
            query.SetParameter("numeroTentativas", numeroTentativas); 
            query.SetParameter("dataLimite", dataLimite);
            query.SetParameter("limite", limite);

            query.SetTimeout(120);
            var codigos = query.List<int>().ToList();
            return codigos;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> BuscarIntegracoesPorSituacaoEData(SituacaoIntegracao situacao, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>()
                .Where(obj => obj.SituacaoIntegracao == situacao &&
                       obj.DataIntegracao.Date >= dataInicial.Date &&
                       obj.DataIntegracao.Date <= dataFinal.Date);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            query = query.Fetch(obj => obj.CargaEntregaEvento)
                .ThenFetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.CargaEntregaEvento)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CargaEntregaEvento)
                .ThenFetch(obj => obj.CargaEntrega)
                .ThenFetch(obj => obj.Cliente)
                .Fetch(obj => obj.TipoIntegracao);

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametrosConsulta.InicioRegistros > 0)
                    query = query.Skip(parametrosConsulta.InicioRegistros);

                if (parametrosConsulta.LimiteRegistros > 0)
                    query = query.Take(parametrosConsulta.LimiteRegistros);
            }

            return query.WithOptions(o => { o.SetTimeout(120); }).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                query = query.Where(o => o.CargaEntregaEvento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.CargaEntregaEvento.CargaEntrega.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.CodigoTipoDeOcorrencia > 0)
                query = query.Where(o => o.CargaEntregaEvento.TipoDeOcorrencia.Codigo == filtrosPesquisa.CodigoTipoDeOcorrencia);

            if (filtrosPesquisa.CodigoTipoIntegracao > 0)
                query = query.Where(o => o.TipoIntegracao.Codigo == filtrosPesquisa.CodigoTipoIntegracao);

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(o => o.DataIntegracao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                query = query.Where(o => o.DataIntegracao <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

            return query;
        }

        #endregion
    }
}
