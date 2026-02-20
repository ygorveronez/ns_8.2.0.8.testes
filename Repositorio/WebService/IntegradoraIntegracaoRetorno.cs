using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.WebService
{
    public class IntegradoraIntegracaoRetorno : RepositorioBase<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>
    {
        #region Construtores 

        public IntegradoraIntegracaoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa, string propOrdenar = null, string dirOrdenar = null, int? inicio = null, int? limite = null)
        {
            IQueryable<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno> query = SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();

            if (filtroPesquisa.Sucesso.HasValue)
                query = query.Where(o => o.Sucesso == filtroPesquisa.Sucesso.Value);

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroIdentificacao))
                query = query.Where(o => o.NumeroIdentificacao == filtroPesquisa.NumeroIdentificacao || o.Carga.CodigoCargaEmbarcador == filtroPesquisa.NumeroIdentificacao);

            if (filtroPesquisa.CodigoIntegradora > 0)
                query = query.Where(o => o.Integradora.Codigo == filtroPesquisa.CodigoIntegradora);

            if (filtroPesquisa.DataInicial.HasValue)
                query = query.Where(o => o.Data >= filtroPesquisa.DataInicial);

            if (filtroPesquisa.DataFinal.HasValue)
                query = query.Where(o => o.Data < filtroPesquisa.DataFinal);

            if (filtroPesquisa.PossuiCarga.HasValue)
                query = filtroPesquisa.PossuiCarga.Value ? query.Where(o => o.Carga != null) : query.Where(o => o.Carga == null);

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio.Value).Take(limite.Value);

            return query;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            return ObterQueryConsulta(filtroPesquisa, propOrdenar, dirOrdenar, inicio, limite)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                 .Fetch(obj => obj.Integradora)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa)
        {
            return ObterQueryConsulta(filtroPesquisa).Count();
        }

        public List<long> BuscarIntegracoesAguardando(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();
            var result = from obj in query
                         where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.Carga == null
                         select obj;
            return result
                .OrderBy(o => o.Codigo)
                .Select(o => o.Codigo)
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public List<long> BuscarCargasPendenteIntegracao(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();
            var result = from obj in query
                         where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.Carga != null
                         select obj;
            return result
                .OrderBy(o => o.Codigo)
                .Select(o => o.Codigo)
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> BuscarCargasPendenteIntegracaoServico(int numeroRegistrosPorVez, int numeroMaximoTentativas, int minutosEsperaIntegracoesQueFalharam)
        {
            string hql = $@"SELECT RETORNO.CAR_CODIGO CODIGOCARGA, MAX(RETORNO.IIR_CODIGO) CODIGOARQUIVO, MIN(RETORNO.IIR_DATA) AS MINDATA 
                            FROM T_INTEGRADORA_INTEGRACAO_RETORNO RETORNO
                            WHERE RETORNO.CAR_CODIGO IS NOT NULL AND 
                            (
                                RETORNO.IIR_SITUACAO = 0
                                OR (
                                    RETORNO.IIR_SITUACAO = 2 AND RETORNO.IIR_NUMERO_TENTATIVAS < {numeroMaximoTentativas}
                                    AND CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'UTC' AT TIME ZONE 'E. SOUTH AMERICA STANDARD TIME' AS DATETIME) > DATEADD(MINUTE,{minutosEsperaIntegracoesQueFalharam},ISNULL(IIR_DATA_ULTIMA_TENTATIVA, DATEADD(MINUTE, -5, GETDATE())))
                                    )
                            )  
                            GROUP BY RETORNO.CAR_CODIGO ORDER BY {(numeroRegistrosPorVez == 1 ? "MINDATA ASC" : " RETORNO.CAR_CODIGO DESC ")} OFFSET 0 ROWS FETCH NEXT " + numeroRegistrosPorVez + " ROWS ONLY ";

            var query = this.SessionNHiBernate.CreateSQLQuery(hql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte)));

            return query.List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte>();
        }

        public Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();
            var result = from obj in query
                         where obj.Codigo == codigo
                         select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno BuscarUltimaPorIdentificador(string identificador, int codigoIntegradora, long codigoIntegracaoAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();
            var result = from obj in query
                         where obj.Integradora.Codigo == codigoIntegradora &&
                               obj.NumeroIdentificacao == identificador
                         select obj;

            if (situacao != null)
                result = result.Where(o => o.Situacao == situacao);

            if (codigoIntegracaoAtual > 0)
                result = result.Where(o => o.Codigo != codigoIntegracaoAtual);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }



        public Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno BuscarUltimaPorCarga(int idCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();
            var result = from obj in query
                         where obj.Carga.Codigo == idCarga
                         select obj;

            if (situacao != null)
                result = result.Where(o => o.Situacao == situacao);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }


        public Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno BuscarPorCodigoArquivoRetorno(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();

            var result = from obj in query where obj.ArquivosIntegracaoRetorno.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public void SanitizarIntegracoesCargaAnteriores(Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte arquivoProcessado)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_INTEGRADORA_INTEGRACAO_RETORNO SET IIR_SITUACAO = 1 WHERE IIR_SITUACAO in(0,2) AND CAR_CODIGO = :codigoCarga and IIR_CODIGO < :codigoArquivo;")
                .SetInt32("codigoCarga", arquivoProcessado.CodigoCarga)
                .SetInt64("codigoArquivo", arquivoProcessado.CodigoArquivo).ExecuteUpdate();
        }


        #endregion
    }
}
