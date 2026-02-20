using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ProcessamentoNotaFiscalAssincrono : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>
    {
        #region Construtores 

        public ProcessamentoNotaFiscalAssincrono(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        //private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa, string propOrdenar = null, string dirOrdenar = null, int? inicio = null, int? limite = null)
        //{
        //    IQueryable<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno> query = SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno>();

        //    if (filtroPesquisa.Sucesso.HasValue)
        //        query = query.Where(o => o.Sucesso == filtroPesquisa.Sucesso.Value);

        //    if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroIdentificacao))
        //        query = query.Where(o => o.NumeroIdentificacao == filtroPesquisa.NumeroIdentificacao || o.Carga.CodigoCargaEmbarcador == filtroPesquisa.NumeroIdentificacao);

        //    if (filtroPesquisa.CodigoIntegradora > 0)
        //        query = query.Where(o => o.Integradora.Codigo == filtroPesquisa.CodigoIntegradora);

        //    if (filtroPesquisa.DataInicial.HasValue)
        //        query = query.Where(o => o.Data >= filtroPesquisa.DataInicial);

        //    if (filtroPesquisa.DataFinal.HasValue)
        //        query = query.Where(o => o.Data < filtroPesquisa.DataFinal);

        //    if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
        //        query = query.OrderBy(propOrdenar + " " + dirOrdenar);

        //    if (inicio > 0 || limite > 0)
        //        query = query.Skip(inicio.Value).Take(limite.Value);

        //    return query;
        //}

        private string ObterSqlSelect()
        {
            string select =
            @"select 
                Requisicao.PNA_CODIGO Codigo,
                Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                Requisicao.PNA_MENSAGEM Retorno,
                Requisicao.PNA_DATA_RECEBIMENTO DataIntegracao,
                Requisicao.PNA_SITUACAO SituacaoIntegracao
";

            return select;
        }

        private string ObterSqlFrom()
        {
            string select =
            @"
            from T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO Requisicao
                left join T_CARGA Carga on Carga.CAR_CODIGO = Requisicao.CAR_CODIGO 
                left join T_PEDIDO Pedido on Pedido.PED_CODIGO = Requisicao.PNA_PROTOCOLO_PEDIDO
";

            return select;
        }

        private string ObterSqlWhere(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd HH:mm";
            string where = "where 1=1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where += $"and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                where += $@"and exists (select top 1 1 from T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO_CHAVE_RECEBIDAS _chaveNotas 
			                        left join T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO_CHAVE_RECEBIDA _chaveNota on _chaveNota.PNC_CODIGO = _chaveNotas.PNC_CODIGO 
				                    where _chaveNotas.PNA_CODIGO = Requisicao.PNA_CODIGO and _chaveNota.PNC_CHAVE = '{filtrosPesquisa.Chave}') ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                where += $@"and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}' ";

            if (filtrosPesquisa.DataFinal.HasValue)
                where += $"and Requisicao.PNA_DATA_RECEBIMENTO <= '{filtrosPesquisa.DataFinal.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.DataInicial.HasValue)
                where += $"and Requisicao.PNA_DATA_RECEBIMENTO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.SituacaoIntegracao.Count > 0)
                where += $"and Requisicao.PNA_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoIntegracao.Select(x => (int)x).ToList())}) ";

            return where;
        }

        private string ObterConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null, bool somenteContar = false)
        {
            string sql;

            sql = (somenteContar ? "select count(0) " : ObterSqlSelect()) + ObterSqlFrom() + ObterSqlWhere(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, null, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>();

            var result = from obj in query where obj.ArquivosIntegracaoRetorno.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        //public List<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa, string propOrdenar, string dirOrdenar, int inicio, int limite)
        //{
        //    return ObterQueryConsulta(filtroPesquisa, propOrdenar, dirOrdenar, inicio, limite)
        //        .Fetch(obj => obj.Carga)
        //        .ThenFetch(obj => obj.TipoDeCarga)
        //        .Fetch(obj => obj.Carga)
        //        .ThenFetch(obj => obj.Filial)
        //         .Fetch(obj => obj.Integradora)
        //        .ToList();
        //}

        //public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa)
        //{
        //    return ObterQueryConsulta(filtroPesquisa).Count();
        //}

        public List<int> BuscarIntegracoesAguardando(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>();
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

        public List<int> BuscarCargasPendenteIntegracao(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>();
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

        public Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono BuscarPendentePorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>()
            .Where(obj => obj.Codigo == codigo && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
            .FirstOrDefault();
        }

        //public IList<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> BuscarCargasPendenteIntegracaoServico(int numeroRegistrosPorVez)
        //{

        //    string hql = @"select CAR_CODIGO CodigoCarga, max(IIR_CODIGO) CodigoArquivo from T_INTEGRADORA_INTEGRACAO_RETORNO where IIR_SITUACAO = 0  and CAR_CODIGO is not null
        //                   group by CAR_CODIGO order by CAR_CODIGO desc offset 0 rows fetch next " + numeroRegistrosPorVez + " rows only ";

        //    var query = this.SessionNHiBernate.CreateSQLQuery(hql);
        //    query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte)));

        //    return query.List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte>();

        //}

        public Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>();
            var result = from obj in query
                         where obj.Codigo == codigo
                         select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosPendentes(int limiteTentativas, int limite)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono>()
            .Where(obj => obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.Tentativas < limiteTentativas)
            .Take(limite).Select(x => x.Codigo).ToList();
        }

        #endregion
    }
}
