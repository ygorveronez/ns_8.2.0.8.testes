using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.DetalhesGestaoPallet;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsultaMovimentacaoPallet = Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet;

namespace Repositorio.Embarcador.GestaoPallet
{
    public class MovimentacaoPallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>
    {

        #region Construtores

        public MovimentacaoPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MovimentacaoPallet(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Task<IList<ConsultaMovimentacaoPallet>> ObterControlePalletAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = QueryConsultaControlePallet(filtrosPesquisaEnvioDevolucaoPallet, parametrosConsulta, somenteContar: false);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ConsultaMovimentacaoPallet)));

            return consulta.SetTimeout(600).ListAsync<ConsultaMovimentacaoPallet>(CancellationToken);
        }

        public Task<int> ContarControlePalletAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = QueryConsultaControlePallet(filtrosPesquisaEnvioDevolucaoPallet, parametrosConsulta, somenteContar: true);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Task<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.TotalizadoresEnvioDevolucaoPallets> ObterTotalizadoresControlePalletAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet)
        {
            var consulta = QueryConsultaTotalizadoresControlePallet(filtrosPesquisaEnvioDevolucaoPallet);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.TotalizadoresEnvioDevolucaoPallets)));

            return consulta.SetTimeout(600).UniqueResultAsync<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.TotalizadoresEnvioDevolucaoPallets>(CancellationToken);
        }

        public int ContarConsultaControleSaldoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa)
        {
            var sql = QueryConsultaControleSaldo(filtrosPesquisa, true);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.DadosControleSaldoPallet> ConsultaControleSaldoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var sql = QueryConsultaControleSaldo(filtrosPesquisa, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.DadosControleSaldoPallet)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.DadosControleSaldoPallet>();
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.TotalizadoresControleSaldoPallet ConsultarTotalizadoresControleSaldoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa)
        {
            var sql = QueryConsultaTotalizadoresControleSaldo(filtrosPesquisa);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.TotalizadoresControleSaldoPallet)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.TotalizadoresControleSaldoPallet>().FirstOrDefault();
        }

        public async Task<DetalhesGestaoPallet> BuscarPorMovimentacaoPalletAsync(int codigoMovimentacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>()
                .Where(movimentacaoPallet => movimentacaoPallet.Codigo == codigoMovimentacao);

            DetalhesGestaoPallet detalhesGestaoPallet = await query.Select(movimentacaoPallet => new DetalhesGestaoPallet
            {
                Codigo = movimentacaoPallet.Codigo,
                Situacao = movimentacaoPallet.Situacao.ObterDescricao(),
                QuantidadePallets = movimentacaoPallet.QuantidadePallets,
                Observacao = movimentacaoPallet.Observacao,
                Carga = new Carga
                {
                    CodigoCargaEmbarcador = movimentacaoPallet.Carga.CodigoCargaEmbarcador,
                    DadosSumarizados = new CargaDadosSumarizados
                    {
                        Origem = movimentacaoPallet.Carga.DadosSumarizados.Origens,
                        Destino = movimentacaoPallet.Carga.DadosSumarizados.Destinos
                    }
                },
                XMLNotaFiscal = movimentacaoPallet.XMLNotaFiscal != null
                ? new XMLNotaFiscal
                {
                    Codigo = movimentacaoPallet.XMLNotaFiscal.Codigo,
                }
                : null
            }).FirstOrDefaultAsync(CancellationToken);

            if (detalhesGestaoPallet.XMLNotaFiscal != null)
            {
                IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> queryGestaoDevolucaoNotaFiscalOrigem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await queryGestaoDevolucaoNotaFiscalOrigem
                    .Where(gestaoDevolucaoNotaFiscalOrigem => gestaoDevolucaoNotaFiscalOrigem.XMLNotaFiscal.Codigo == detalhesGestaoPallet.XMLNotaFiscal.Codigo)
                    .Select(gestaoDevolucaoNotaFiscalOrigem => gestaoDevolucaoNotaFiscalOrigem.GestaoDevolucao)
                    .FirstOrDefaultAsync(CancellationToken);

                if (gestaoDevolucao != null)
                {
                    IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> queryGestaoDevolucaoNotaFiscalDevolucao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalDevolucao = await queryGestaoDevolucaoNotaFiscalDevolucao
                        .Where(gestaoDevolucaoNotaFiscaDevolucao => gestaoDevolucaoNotaFiscaDevolucao.GestaoDevolucao.Codigo == gestaoDevolucao.Codigo)
                        .Select(gestaoDevolucaoNotaFiscaDevolucao => gestaoDevolucaoNotaFiscaDevolucao.XMLNotaFiscal)
                        .FirstOrDefaultAsync(CancellationToken);

                    if (xmlNotaFiscalDevolucao != null)
                    {
                        detalhesGestaoPallet.NumeroNotaFiscalDevolucao = xmlNotaFiscalDevolucao.Numero;
                        detalhesGestaoPallet.SerieNotaFiscalDevolucao = xmlNotaFiscalDevolucao.Serie;
                    }
                    else
                    {
                        detalhesGestaoPallet.NumeroNotaFiscalDevolucao = gestaoDevolucao.NumeroNotaFiscalDevolucao;
                        detalhesGestaoPallet.SerieNotaFiscalDevolucao = gestaoDevolucao.SerieNotaFiscalDevolucao;
                    }

                    detalhesGestaoPallet.NumeroNotaFiscalPermuta = gestaoDevolucao.NumeroNotaFiscalPermuta;
                    detalhesGestaoPallet.SerieNotaFiscalPermuta = gestaoDevolucao.SerieNotaFiscalPermuta;
                }
            }

            return detalhesGestaoPallet;
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarPorNotaECargaPedido(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Codigo == codigoCargaPedido && obj.Situacao != SituacaoGestaoPallet.Cancelada);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarAutomaticoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoLancamento == TipoLancamento.Automatico && obj.Situacao != SituacaoGestaoPallet.Cancelada);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> BuscarMovimentosCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> BuscarPorCargaDiffTransportador(int codigoCarga, int codigoTransportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                obj.TipoLancamento == TipoLancamento.Automatico &&
                                obj.Situacao != SituacaoGestaoPallet.Cancelada &&
                                obj.Transportador.Codigo != codigoTransportador);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarUltimaMovimentacaoPorControlePallet(int codigoControlePallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.ControleEstoquePallet.Codigo == codigoControlePallet);

            return query.OrderByDescending(obj => obj.Codigo).FirstOrDefault();
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaSaldoControlePallet>> ObterSaldoControlePalletAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = QueryConsultaSaldoControlePallet(filtrosPesquisaSaldoControlePallet, parametrosConsulta, somenteContar: false);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaSaldoControlePallet)));

            return consulta.SetTimeout(600).ListAsync<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaSaldoControlePallet>(CancellationToken);
        }

        public Task<int> ContarSaldoControlePalletAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaEnvioDevolucaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = QueryConsultaSaldoControlePallet(filtrosPesquisaEnvioDevolucaoPallet, parametrosConsulta, somenteContar: true);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarPorNotaESituacao(int codigoXMLNotaFiscal, SituacaoGestaoPallet situacaoGestaoPallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.Situacao == situacaoGestaoPallet);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> BuscarPorNotasESituacao(List<int> codigosXMLNotasFiscais, SituacaoGestaoPallet situacaoGestaoPallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => codigosXMLNotasFiscais.Contains(obj.XMLNotaFiscal.Codigo) && obj.Situacao == situacaoGestaoPallet);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> BuscarPorNotasEntregaESituacao(List<int> codigosXMLNotasFiscais, SituacaoGestaoPallet situacaoGestaoPallet)
        {
            List<RegraPallet> regraPalletsIgnorar = new List<RegraPallet> { RegraPallet.CanhotoAssinado, RegraPallet.ValePallet };

            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => codigosXMLNotasFiscais.Contains(obj.XMLNotaFiscal.Codigo) && obj.Situacao == situacaoGestaoPallet && !regraPalletsIgnorar.Contains(obj.RegraPallet));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarPorNotaCanhotoESituacao(int codigoXMLNotaFiscal, SituacaoGestaoPallet situacaoGestaoPallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.Situacao == situacaoGestaoPallet && obj.RegraPallet == RegraPallet.CanhotoAssinado);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet BuscarReservaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Situacao == SituacaoGestaoPallet.Reserva);

            return query.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.MovimentosPalletPendentes> BuscarMovimentosPendentes()
        {
            var sql = QueryConsultaMovimentosPendentes();
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.MovimentosPalletPendentes)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.MovimentosPalletPendentes>();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string QueryConsultaControleSaldo(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa, bool somenteContarNumeroRegistros, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"SELECT
                                movimentacaoPallet.MPT_CODIGO                               AS Codigo,
	                            xmlNotaFiscal.NF_NUMERO						                AS NumeroNota,
	                            carga.CAR_CODIGO_CARGA_EMBARCADOR			                AS NumeroCarga,
	                            movimentacaoPallet.MPT_QUANTIDADE_PALLETS	                AS QuantidadePallets,
	                            movimentacaoPallet.MPT_DATA_RECEBIMENTO		                AS DataRecebimento,
	                            movimentacaoPallet.MPT_REGRA_PALLET			                AS RegraPallet,
	                            movimentacaoPallet.MPT_SITUACAO			                    AS SituacaoPallet,
		                        dadosSumarizadosCarga.CDS_ORIGENS			                AS Origem,
	                            dadosSumarizadosCarga.CDS_DESTINOS			                AS Destino,
	                            filialCarga.FIL_DESCRICAO			                        AS Filial,
	                            movimentacaoPallet.MPT_QUEBRA_REGRA_INFORMADA		        AS QuebraRegra,
	                            movimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET		AS Responsavel,
	                            movimentacaoPallet.MPT_TIPO_LANCAMENTO                      AS TipoLancamento,
	                            MovimentacaoPallet.MPT_TIPO_MOVIMENTACAO                    AS TipoMovimentacao,
                                gestaoDevolucao.GDV_TIPO									AS TipoDevolucao,
	                            {filtrosPesquisa.DiasLimiteParaDevolucao}		            AS DiasLimiteParaDevolucao,
	                            xmlNotaFiscal.NF_DATA_RECEBIMENTO							AS DataRecebimentoNota,
                                (case
		                            when gestaoDevolucao.GDV_CODIGO is null and xmlNotaFiscal.NF_DATA_RECEBIMENTO < '{filtrosPesquisa.DataLimiteGeracaoDevolucao:yyyyMMdd HH:mm:ss}' then 'Vencido'
		                            when gestaoDevolucao.GDV_CODIGO is null and xmlNotaFiscal.NFX_CODIGO is not null then 'Pendente'
		                            when ultimaEtapaDevolucao.GDE_SITUACAO_ETAPA = {(int)SituacaoEtapaGestaoDevolucao.Finalizada} then 'Devolvido'
		                            when gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.NaoDefinido} then 'Em analise'
		                            when gestaoDevolucao.GDV_APROVADA = 1 then 'Aceito'
		                            when gestaoDevolucao.GDV_APROVADA = 0 then 'Recusado'
		                            else 'Indeterminado'
	                            end)                                                       AS SituacaoDevolucao
                ");

            sql.Append(" from T_MOVIMENTACAO_PALLET movimentacaoPallet ");

            sql.Append(ObterJoinsControleSaldo);
            sql.Append(ObterFiltrosControleSaldo(filtrosPesquisa));

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                sql.Append($" ORDER BY {propOrdenacao} {dirOrdenacao}");

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql.Append($" OFFSET {inicioRegistros} ROWS FETCH NEXT {maximoRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        private string QueryConsultaMovimentosPendentes()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select 
                          movimentoPallet.MPT_CODIGO CodigoMovimentoPallet, 
                          movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET TipoResponsavel, 
                          carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, 
                          xmlnf.NF_DATA_EMISSAO DataEmissaoNota, 
                          xmlnf.NF_NUMERO NumeroNota,
                          (
                            case 
                                when movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 3 then filial.FIL_CNPJ
                                when movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 1 then LTRIM(STR(cliente.CLI_CGCCPF, 25, 0)) 
                                else empresa.EMP_CNPJ end
                          ) Responsavel,
                          (case 
	                          when movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 3 then filial.FIL_EMAIL 
                              when movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 1 then cliente.CLI_EMAIL
                              else empresa.EMP_EMAIL end
                          ) EmailResponsavel
                        from 
                          T_MOVIMENTACAO_PALLET movimentoPallet 
                          left join T_FILIAL filial on movimentoPallet.FIL_CODIGO = filial.FIL_CODIGO 
                          left join T_EMPRESA empresa on movimentoPallet.EMP_CODIGO = empresa.EMP_CODIGO 
                          left join T_CLIENTE cliente on movimentoPallet.CLI_CGCCPF = cliente.CLI_CGCCPF 
                          left join T_XML_NOTA_FISCAL xmlnf on movimentoPallet.NFX_CODIGO = xmlnf.NFX_CODIGO 
                          left join T_CARGA carga on movimentoPallet.CAR_CODIGO = carga.CAR_CODIGO 
                        where 
                          MPT_SITUACAO = 1 
                          and 
                          (
	                          (movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 3 and filial.FIL_EMAIL <> '') or
	                          (movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 1 and cliente.CLI_EMAIL <> '') or
	                          (movimentoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 2 and empresa.EMP_EMAIL <> '')
                          )
                        order by 
                          Responsavel, 
                          TipoResponsavel");

            return sql.ToString();
        }

        private string QueryConsultaTotalizadoresControleSaldo(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append($@"
                SELECT
                    SUM(CASE WHEN movimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Pendente} THEN 1 ELSE 0 END) AS TotalPendente,
                    SUM(CASE WHEN movimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Concluido} THEN 1 ELSE 0 END) AS TotalDevolvido,
                    SUM(CASE WHEN movimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Pendente} THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS else 0 END) as PalettsPendente,
                    SUM(CASE WHEN movimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Reserva} THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS else 0 END) as PalettsReservado,

		            SUM(CASE WHEN gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO > '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}' THEN 1 ELSE 0 END) as NotasNoPrazo,
		            SUM(CASE WHEN gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO < '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}' THEN 1 ELSE 0 END) as NotasVencido,
		            SUM(CASE WHEN gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.Agendamento} and gestaoDevolucaoDadosComplementares.GDC_DATA_ANALISE_AGENDAMENTO is not null THEN 1 ELSE 0 END) as NotasAgendado,
		            SUM(CASE WHEN gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.PermutaPallet} and gestaoDevolucao.GDV_APROVADA = 1 THEN 1 ELSE 0 END) as NotasPermuta,

		            SUM(CASE WHEN gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO > '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}' THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS ELSE 0 END) as PalettsNoPrazo,
                    SUM(CASE WHEN gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO < '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}' THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS ELSE 0 END) as PalettsVencido,
		            SUM(CASE WHEN gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.Agendamento} and gestaoDevolucaoDadosComplementares.GDC_DATA_ANALISE_AGENDAMENTO is not null THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS ELSE 0 END) as PalettsAgendado,
		            SUM(CASE WHEN gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.PermutaPallet} and gestaoDevolucao.GDV_APROVADA = 1 THEN movimentacaoPallet.MPT_QUANTIDADE_PALLETS ELSE 0 END) as PalettsPermuta,

                    ({ObterSqlTotalSaldo(filtrosPesquisa)}) AS TotalSaldo
            ");

            sql.Append(" from T_MOVIMENTACAO_PALLET movimentacaoPallet ");

            sql.Append(ObterJoinsControleSaldo);
            sql.Append(ObterFiltrosControleSaldo(filtrosPesquisa));

            return sql.ToString();
        }

        private string ObterJoinsControleSaldo => @"
            left join T_XML_NOTA_FISCAL xmlNotaFiscal ON xmlNotaFiscal.NFX_CODIGO = movimentacaoPallet.NFX_CODIGO
            left join T_CARGA carga ON carga.CAR_CODIGO = movimentacaoPallet.CAR_CODIGO
		        left join T_FILIAL filialCarga ON filialCarga.FIL_CODIGO = carga.FIL_CODIGO
		        left join T_CARGA_DADOS_SUMARIZADOS dadosSumarizadosCarga ON dadosSumarizadosCarga.CDS_CODIGO = carga.CDS_CODIGO
            left join T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL gestaoDevolucaoNotaFiscal ON gestaoDevolucaoNotaFiscal.NFX_CODIGO = xmlNotaFiscal.NFX_CODIGO
		        left join T_GESTAO_DEVOLUCAO gestaoDevolucao ON gestaoDevolucao.GDV_CODIGO = gestaoDevolucaoNotaFiscal.GDV_CODIGO
		        left join (select TOP 1 GDE_SITUACAO_ETAPA, GDV_CODIGO FROM T_GESTAO_DEVOLUCAO_ETAPA order by GDE_ORDEM desc) AS ultimaEtapaDevolucao ON ultimaEtapaDevolucao.GDV_CODIGO = gestaoDevolucao.GDV_CODIGO
                left join T_GESTAO_DEVOLUCAO_DADOS_COMPLEMENTARES gestaoDevolucaoDadosComplementares ON gestaoDevolucaoDadosComplementares.GDV_CODIGO = gestaoDevolucao.GDV_CODIGO";

        private string ObterSqlTotalSaldo(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder("SELECT sum(CPT_QUANTIDADE_TOTAL_PALLETS) FROM T_CONTROLE_ESTOQUE_PALLET");

            sql.Append($" where CPT_TIPO_ESTOQUE_PALLET = {(int)TipoEstoquePallet.Movimentacao}");

            if (filtrosPesquisa.ResponsavelPallet == ResponsavelPallet.Transportador)
                sql.Append($" and EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.ResponsavelPallet == ResponsavelPallet.Cliente)
                sql.Append($" and CLI_CGCCPF = {filtrosPesquisa.CodigoCliente}");

            return sql.ToString();
        }

        private string ObterFiltrosControleSaldo(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" WHERE 1 = 1 ");

            sql.Append($" AND movimentacaoPallet.MPT_SITUACAO <> {(int)SituacaoGestaoPallet.Cancelada}");

            if (filtrosPesquisa.ResponsavelPallet.HasValue)
            {
                sql.Append($" AND movimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)filtrosPesquisa.ResponsavelPallet}");

                if (filtrosPesquisa.ResponsavelPallet == ResponsavelPallet.Transportador)
                    sql.Append($" AND movimentacaoPallet.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                if (filtrosPesquisa.ResponsavelPallet == ResponsavelPallet.Cliente)
                    sql.Append($" AND movimentacaoPallet.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente}");
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                sql.Append($" AND (filialCarga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))");

            if (filtrosPesquisa.DataInicialCriacaoCarga != default)
                sql.Append($" AND carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicialCriacaoCarga.Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.DataFinalCriacaoCarga != default)
                sql.Append($" AND carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataFinalCriacaoCarga.Date:yyyyMMdd HH:mm:ss}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                sql.Append($" AND carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%{filtrosPesquisa.NumeroCarga}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNotaFiscal))
                sql.Append($" AND xmlNotaFiscal.NF_NUMERO LIKE '%{filtrosPesquisa.NumeroNotaFiscal}%'");

            if (filtrosPesquisa.SituacaoPallet != null)
                sql.Append($" AND movimentacaoPallet.MPT_SITUACAO = {(int)filtrosPesquisa.SituacaoPallet.Value}");

            if (filtrosPesquisa.SituacaoPalletGestaoDevolucao != null)
            {
                if (filtrosPesquisa.SituacaoPalletGestaoDevolucao == SituacaoPalletGestaoDevolucao.NoPrazo)
                    sql.Append($" AND gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO > '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}'");

                if (filtrosPesquisa.SituacaoPalletGestaoDevolucao == SituacaoPalletGestaoDevolucao.Vencido)
                    sql.Append($" AND gestaoDevolucao.GDV_CODIGO is not null and gestaoDevolucao.GDV_DATA_CRIACAO < '{DateTime.Now.AddHours(72):yyyyMMdd HH:mm:ss}'");

                if (filtrosPesquisa.SituacaoPalletGestaoDevolucao == SituacaoPalletGestaoDevolucao.Agendado)
                    sql.Append($" AND gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.Agendamento} and gestaoDevolucaoDadosComplementares.GDC_DATA_ANALISE_AGENDAMENTO is not null");

                if (filtrosPesquisa.SituacaoPalletGestaoDevolucao == SituacaoPalletGestaoDevolucao.Permuta)
                    sql.Append($" AND gestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.PermutaPallet} and gestaoDevolucao.GDV_APROVADA = 1");
            }

            return sql.ToString();
        }

        private NHibernate.IQuery QueryConsultaControlePallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContar)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContar)
                sql.Append(@"select distinct(count(0) over ())");
            else
            {
                sql.Append(@$"
                select
                       MovimentacaoPallet.MPT_CODIGO as Codigo,
                       MovimentacaoPallet.MPT_DATA_RECEBIMENTO as DataRecebimento,
                       MovimentacaoPallet.MPT_QUANTIDADE_PALLETS as QuantidadePallets,
                       MovimentacaoPallet.MPT_SITUACAO as Situacao,
                       MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET as ResponsavelMovimentacaoPallet,
                       MovimentacaoPallet.MPT_REGRA_PALLET as RegraPallet,
                       MovimentacaoPallet.MPT_OBSERVACAO as Observacao,
                       MovimentacaoPallet.MPT_TIPO_LANCAMENTO as TipoLancamento,
                       XmlNotaFiscal.NF_NUMERO as NumeroNotaFiscal,
                       XmlNotaFiscal.NF_DATA_RECEBIMENTO as DataRecebimentoNotaFiscal,
                       Cliente.CLI_NOME as NomeCliente, 
                       Cliente.CLI_CGCCPF as CNPJCliente,
                       Empresa.EMP_RAZAO as Transportador,
                       Empresa.EMP_CNPJ as CNPJTransportador,
	                   Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga,
                       LocalidadeRemetente.UF_SIGLA as UFOrigem,
	                   LocalidadeDestinatario.UF_SIGLA as UFDestino,
	                   LocalidadeDestinatario.LOC_DESCRICAO as CidadeDestino,
	                   LocalidadeRemetente.LOC_DESCRICAO as CidadeOrigem,
	                   Filial.FIL_DESCRICAO as Filial, 
                       Filial.FIL_CNPJ as CNPJFilial,
	                   MovimentacaoPallet.MPT_QUEBRA_REGRA_INFORMADA as QuebraRegra,
	                   MovimentacaoPallet.MPT_TIPO_MOVIMENTACAO as TipoMovimentacao,
                       MovimentacaoPallet.CPE_CODIGO as CodigoCargaPedido, 
                       GestaoDevolucao.GDV_TIPO as TipoDevolucao,
	                   {filtrosPesquisaEnvioDevolucaoPallet.DiasLimiteParaDevolucao} as DiasLimiteParaDevolucao,
                       (case
                           when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Cancelada} then 'Indeterminado'
		                   when GestaoDevolucao.GDV_CODIGO is null and XmlNotaFiscal.NF_DATA_RECEBIMENTO < '{filtrosPesquisaEnvioDevolucaoPallet.DataLimiteGeracaoDevolucao:yyyyMMdd HH:mm:ss}' then 'Vencido'
		                   when GestaoDevolucao.GDV_CODIGO is null and XmlNotaFiscal.NFX_CODIGO is not null then 'Pendente'
		                   when UltimaEtapaDevolucao.GDE_SITUACAO_ETAPA = {(int)SituacaoEtapaGestaoDevolucao.Finalizada} then 'Devolvido'
		                   when GestaoDevolucao.GDV_TIPO = {(int)TipoGestaoDevolucao.NaoDefinido} then 'Em analise'
		                   when GestaoDevolucao.GDV_APROVADA = 1 then 'Aceito'
		                   when GestaoDevolucao.GDV_APROVADA = 0 then 'Recusado'
		                   else 'Indeterminado'
	                   end) as SituacaoDevolucao,
                       XmlNotaFiscal.NF_SERIE as {nameof(ConsultaMovimentacaoPallet.SerieNfe)},
                       STUFF(
                           (
                               select distinct ',' + CAST(CargaPedido.PED_TIPO_TOMADOR as varchar(50)) 
                               from T_CARGA_PEDIDO CargaPedido
                               join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on CargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO
                               where XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                               for xml path('')
                           ), 1, 1, ''
                       ) as {nameof(ConsultaMovimentacaoPallet.TiposTomador)},
                       STUFF(
                           (
                               select distinct ',' + CAST(CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL as varchar(50))
                               from T_CARGA_PEDIDO CargaPedido
                               join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on CargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO
                               where XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                               for xml path('')
                           ), 1, 1, ''
                       ) as {nameof(ConsultaMovimentacaoPallet.TiposModal)},
                       XmlNotaFiscal.NF_CFOP as {nameof(ConsultaMovimentacaoPallet.CfopNfe)},
                       XmlNotaFiscal.NF_DATA_EMISSAO as {nameof(ConsultaMovimentacaoPallet.DataEmissaoNfe)},
                       RecebimentoValePallet.RVP_DATA_VENCIMENTO as {nameof(ConsultaMovimentacaoPallet.DataVencimenoValePallet)},
                       STUFF(
                           (
                               select distinct ', ' + Pedido.PED_CODIGO_PEDIDO_CLIENTE 
                               from T_PEDIDO Pedido
                               join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                               join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on CargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO
                               where XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                               for xml path('')
                           ), 1, 1, ''
                       ) as {nameof(ConsultaMovimentacaoPallet.NumerosPedidoCliente)},
                       STUFF(
                           (
                               select distinct ', ' + CanalVenda.CNV_DESCRICAO
                               from T_CANAL_VENDA CanalVenda
                               join T_CARGA_PEDIDO CargaPedido on CanalVenda.CNV_CODIGO = CargaPedido.CNV_CODIGO
                               join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on CargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO
                               where XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                               for xml path('')
                           ), 1, 1, ''
                       ) as {nameof(ConsultaMovimentacaoPallet.CanaisVenda)},
                       veiculo.VEI_PLACA as {nameof(ConsultaMovimentacaoPallet.PlacaTracao)},
                       STUFF(
                           (
                               select distinct ', ' + Veiculo.VEI_PLACA
                               from T_VEICULO Veiculo
                               join T_CARGA_VEICULOS_VINCULADOS CargaVeiculoVinculado on Veiculo.VEI_CODIGO = CargaVeiculoVinculado.VEI_CODIGO
                               where CargaVeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO
                               for xml path('')
                           ), 1, 1, ''
                       ) as {nameof(ConsultaMovimentacaoPallet.PlacasReboque)},
                       CanhotoNotalFscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO as {nameof(ConsultaMovimentacaoPallet.SituacaoCanhoto)},
                       case when MovimentacaoPallet.RVP_CODIGO is null then 'Pendente' else 'Recebido' end as {nameof(ConsultaMovimentacaoPallet.SituacaoValePallet)},
                       CanhotoNotalFscal.CNF_DATA_RECEBIMENTO as {nameof(ConsultaMovimentacaoPallet.DataRecebimentoCanhoto)},
                       CanhotoNotalFscal.CNF_DATA_DIGITALIZACAO as {nameof(ConsultaMovimentacaoPallet.DataDigitalizacaoCanhoto)},
                       RecebimentoValePallet.RVP_DATA_CRIACAO as {nameof(ConsultaMovimentacaoPallet.DataRecebimentoValePallet)},
                       Chamado.CHA_NUMERO as {nameof(ConsultaMovimentacaoPallet.NumeroAtendimento)},
                       Chamado.CHA_SITUACAO as {nameof(ConsultaMovimentacaoPallet.SituacaoAtendimento)},
                       ClienteComplementar.CLC_ESCRITORIO_VENDAS as {nameof(ConsultaMovimentacaoPallet.EscritorioVendas)},
                       GestaoDevolucaoLaudo.GDL_DATA_CRIACAO as {nameof(ConsultaMovimentacaoPallet.DataLaudo)},
                       MovimentacaoPallet.GDL_CODIGO as {nameof(ConsultaMovimentacaoPallet.NumeroLaudo)},
                       GestaoDevolucao.GDV_NUMERO_NOTA_FISCAL_PERMUTA as {nameof(ConsultaMovimentacaoPallet.NumeroNotaFiscalPermuta)},
                       GestaoDevolucao.GDV_SERIE_NOTA_FISCAL_PERMUTA as {nameof(ConsultaMovimentacaoPallet.SerieNotaFiscalPermuta)},
                       coalesce(Nfd.NF_NUMERO, GestaoDevolucao.GDV_NUMERO_NOTA_FISCAL_DEVOLUCAO) as {nameof(ConsultaMovimentacaoPallet.NumeroNotaFiscalDevolucao)},
                       coalesce(Nfd.NF_SERIE, GDV_SERIE_NOTA_FISCAL_DEVOLUCAO) as {nameof(ConsultaMovimentacaoPallet.SerieNotaFiscalDevolucao)},
                       GestaoDevolucao.GDV_ORIGEM_RECEBIMENTO as {nameof(ConsultaMovimentacaoPallet.OrigemNotaFiscalDevolucao)},
                       Funcionario.FUN_NOME as {nameof(ConsultaMovimentacaoPallet.ResponsavelDevolucao)},
                       GestaoDevolucaoDadosComplementares.GDC_DATA_CARREGAMENTO as {nameof(ConsultaMovimentacaoPallet.DataColeta)},
                       GestaoDevolucaoDadosComplementares.GDC_DATA_DESCARREGAMENTO as {nameof(ConsultaMovimentacaoPallet.DataPrevisaoChegada)},
                       CONCAT(ClienteColeta.CLI_ENDERECO, case when ClienteColeta.CLI_ENDERECO is not null then ', ' end, ClienteColeta.CLI_BAIRRO, case when ClienteColeta.CLI_BAIRRO is not null then ', ' end, ClienteColeta.CLI_NUMERO) as {nameof(ConsultaMovimentacaoPallet.EnderecoColeta)},
                       LocalidadeClienteColeta.LOC_DESCRICAO as {nameof(ConsultaMovimentacaoPallet.CidadeColeta)},
                       Descarregamento.CED_DESCRICAO as {nameof(ConsultaMovimentacaoPallet.CentroDescarregamento)},
                       Nfd.NF_DATA_RECEBIMENTO as {nameof(ConsultaMovimentacaoPallet.DataRecebimentoNFD)},
                       GestaoDevolucao.GDV_CODIGO as {nameof(ConsultaMovimentacaoPallet.CodigoDevolucao)},
                       GestaoDevolucaoDadosComplementares.GDC_DATA_DESCARREGAMENTO as {nameof(ConsultaMovimentacaoPallet.DataDescarregamento)},
                       convert(varchar(5), PeriodoDescarregamento.PED_HORA_INICIO, 108) as {nameof(ConsultaMovimentacaoPallet.PeriodoDescarregamentoHoraInicio)},
                       convert(varchar(5), PeriodoDescarregamento.PED_HORA_TERMINO, 108) as {nameof(ConsultaMovimentacaoPallet.PeriodoDescarregamentoHoraTermino)}
                "); // SQL-INJECTION-SAFE
            }

            sql.Append(ObterJoinsControlePallet());

            sql.Append(ObterFiltrosControlePallet(filtrosPesquisaEnvioDevolucaoPallet));

            if (!somenteContar)
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return SessionNHiBernate.CreateSQLQuery(sql.ToString());
        }

        private NHibernate.IQuery QueryConsultaSaldoControlePallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContar)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContar)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
            {
                sql.Append(@$"
                select 
                    resultado.Responsavel,
                    resultado.ResponsavelCNPJ,
                    sum(resultado.PalettsPendente) as PalettsPendente,
                    sum(resultado.QuantidadeTotalPallets) as QuantidadeTotalPallets");
            }

            sql.Append(" from (")
                .Append(ObterSubConsultaSaldoEstoque(filtrosPesquisaSaldoControlePallet))
                .Append(" union all ")
                .Append(ObterSubConsultaSaldoMovimentacao(filtrosPesquisaSaldoControlePallet))
                .Append(") as resultado ")
                .Append(" group by resultado.Responsavel, resultado.ResponsavelCNPJ ");

            if (!somenteContar)
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");
                sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return SessionNHiBernate.CreateSQLQuery(sql.ToString());
        }

        private NHibernate.IQuery QueryConsultaTotalizadoresControlePallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@$"
                    select 
                        count(*) as Todos,
                        sum(case when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Pendente} then 1 else 0 end) as Pendente,
                        sum(case when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Concluido} then 1 else 0 end) as Concluido,
                        sum(case when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Cancelada} then 1 else 0 end) as Cancelado,
                        sum(case when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Reserva} then 1 else 0 end) as {nameof(TotalizadoresEnvioDevolucaoPallets.Reserva)},
                        sum(case when MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.AguardandoAvaliacao} then 1 else 0 end) as {nameof(TotalizadoresEnvioDevolucaoPallets.AguardandoAvaliacao)}
            "); // SQL-INJECTION-SAFE

            sql.Append(ObterJoinsControlePallet());

            sql.Append(ObterFiltrosControlePallet(filtrosPesquisaEnvioDevolucaoPallet));

            return this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
        }

        private string ObterJoinsControlePallet()
        {
            StringBuilder from = new StringBuilder();

            from.Append(@" from T_MOVIMENTACAO_PALLET MovimentacaoPallet");
            from.Append(@" left join T_XML_NOTA_FISCAL XmlNotaFiscal on MovimentacaoPallet.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO");
            from.Append(@" left join T_CLIENTE Destinatario on XmlNotaFiscal.CLI_CODIGO_DESTINATARIO = Destinatario.CLI_CGCCPF");
            from.Append(@" left join T_CLIENTE Remetente on XmlNotaFiscal.CLI_CODIGO_REMETENTE = Remetente.CLI_CGCCPF");
            from.Append(@" left join T_LOCALIDADES LocalidadeDestinatario on Destinatario.LOC_CODIGO = LocalidadeDestinatario.LOC_CODIGO");
            from.Append(@" left join T_LOCALIDADES LocalidadeRemetente on Remetente.LOC_CODIGO = LocalidadeRemetente.LOC_CODIGO");
            from.Append(@" left join T_CLIENTE Cliente on MovimentacaoPallet.CLI_CGCCPF = Cliente.CLI_CGCCPF");
            from.Append(@" left join T_CLIENTE_COMPLEMENTAR ClienteComplementar on Cliente.CLI_CGCCPF = ClienteComplementar.CLI_CODIGO");
            from.Append(@" left join T_CARGA Carga on MovimentacaoPallet.CAR_CODIGO = Carga.CAR_CODIGO");
            from.Append(@" left join T_RECEBIMENTO_VALE_PALLET RecebimentoValePallet on MovimentacaoPallet.RVP_CODIGO = RecebimentoValePallet.RVP_CODIGO");
            from.Append(@" left join T_VEICULO veiculo on Carga.CAR_VEICULO = veiculo.VEI_CODIGO");
            from.Append(@" left join T_CANHOTO_NOTA_FISCAL CanhotoNotalFscal on XmlNotaFiscal.NFX_CODIGO = CanhotoNotalFscal.NFX_CODIGO");
            from.Append(@" left join T_GESTAO_DEVOLUCAO_LAUDO GestaoDevolucaoLaudo on MovimentacaoPallet.GDL_CODIGO = GestaoDevolucaoLaudo.GDL_CODIGO");
            from.Append(@$"
                            left join 
                                (
                                    select top 1 ChamadoXmlNotaFiscal.NFX_CODIGO, Chamados.CHA_NUMERO, Chamados.CHA_SITUACAO 
                                    from T_CHAMADOS Chamados 
                                    join T_CHAMADO_XML_NOTA_FISCAL ChamadoXmlNotaFiscal on Chamados.CHA_CODIGO = ChamadoXmlNotaFiscal.CHA_CODIGO
                                ) as Chamado on Chamado.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO
                ");
            from.Append(@$"
                            left join 
                                (
                                    select CentroDescarregamento.CED_DESCRICAO, CargaJanelaDescarregamento.CAR_CODIGO
                                    from T_CENTRO_DESCARREGAMENTO CentroDescarregamento
                                    join T_CARGA_JANELA_DESCARREGAMENTO CargaJanelaDescarregamento on CentroDescarregamento.CED_CODIGO = CargaJanelaDescarregamento.CED_CODIGO
                                    join T_GESTAO_DEVOLUCAO GestaoDevolucao on CargaJanelaDescarregamento.CAR_CODIGO = GestaoDevolucao.CAR_CODIGO_DEVOLUCAO
                                ) as Descarregamento on Descarregamento.CAR_CODIGO = Carga.CAR_CODIGO
                ");

            from.Append(@" left join T_FILIAL Filial on MovimentacaoPallet.FIL_CODIGO = Filial.FIL_CODIGO");
            from.Append(@" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO");
            from.Append(@" left join T_CARGA_PEDIDO CargaPedido on MovimentacaoPallet.CPE_CODIGO = CargaPedido.CPE_CODIGO");
            from.Append(@" left join T_EMPRESA Empresa on MovimentacaoPallet.EMP_CODIGO = Empresa.EMP_CODIGO");

            from.Append(@" left join T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL GestaoDevolucaoNotaFiscal ON GestaoDevolucaoNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO");
            from.Append(@" left join T_GESTAO_DEVOLUCAO GestaoDevolucao ON GestaoDevolucao.GDV_CODIGO = GestaoDevolucaoNotaFiscal.GDV_CODIGO");
            from.Append(@" left join (select TOP 1 GDE_SITUACAO_ETAPA, GDV_CODIGO FROM T_GESTAO_DEVOLUCAO_ETAPA order by GDE_ORDEM desc) AS UltimaEtapaDevolucao ON UltimaEtapaDevolucao.GDV_CODIGO = GestaoDevolucao.GDV_CODIGO");
            from.Append(@$"
                        left join 
                            (
                                select top 1 GestaoDevolucaoXmlNotaFiscalDevolucao.GDV_CODIGO, XmlNotaFiscal.NF_NUMERO, XmlNotaFiscal.NF_SERIE, NF_DATA_RECEBIMENTO
                                from T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL_DEVOLUCAO GestaoDevolucaoXmlNotaFiscalDevolucao
                                join T_XML_NOTA_FISCAL XmlNotaFiscal on GestaoDevolucaoXmlNotaFiscalDevolucao.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO
                            ) as Nfd on Nfd.GDV_CODIGO = GestaoDevolucao.GDV_CODIGO
            ");
            from.Append(@" left join T_GESTAO_DEVOLUCAO_DADOS_COMPLEMENTARES GestaoDevolucaoDadosComplementares on GestaoDevolucao.GDC_CODIGO = GestaoDevolucaoDadosComplementares.GDC_CODIGO");
            from.Append(@" left join T_CLIENTE ClienteColeta on GestaoDevolucaoDadosComplementares.CLI_DESTINO_COLETA = ClienteColeta.CLI_CGCCPF");
            from.Append(@" left join T_LOCALIDADES LocalidadeClienteColeta on ClienteColeta.LOC_CODIGO = LocalidadeClienteColeta.LOC_CODIGO");
            from.Append(@" left join T_FUNCIONARIO Funcionario on GestaoDevolucaoDadosComplementares.GDC_USUARIO_APROVACAO = Funcionario.FUN_CODIGO");
            from.Append(@" left join T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO PeriodoDescarregamento on GestaoDevolucaoDadosComplementares.PED_CODIGO = PeriodoDescarregamento.PED_CODIGO");

            return from.ToString();
        }

        private string ObterFiltrosControlePallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtrosPesquisaEnvioDevolucaoPallet)
        {
            StringBuilder clausulaWhere = new StringBuilder(" WHERE 1 = 1");
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisaEnvioDevolucaoPallet.NotaFiscal > 0)
                clausulaWhere.Append($" and XmlNotaFiscal.NF_NUMERO = {filtrosPesquisaEnvioDevolucaoPallet.NotaFiscal}");

            if (!string.IsNullOrEmpty(filtrosPesquisaEnvioDevolucaoPallet.Carga))
                clausulaWhere.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisaEnvioDevolucaoPallet.Carga}'");

            if (filtrosPesquisaEnvioDevolucaoPallet.DataInicialCriacaoCarga != DateTime.MinValue)
                clausulaWhere.Append($" and Carga.CAR_DATA_CRIACAO >= {filtrosPesquisaEnvioDevolucaoPallet.DataInicialCriacaoCarga.ToString(pattern)}");

            if (filtrosPesquisaEnvioDevolucaoPallet.DataFinalCriacaoCarga != DateTime.MinValue)
                clausulaWhere.Append($" and Carga.CAR_DATA_CRIACAO <= {filtrosPesquisaEnvioDevolucaoPallet.DataFinalCriacaoCarga.ToString(pattern)}");

            if (filtrosPesquisaEnvioDevolucaoPallet.DataInicialNotaFiscal != DateTime.MinValue)
                clausulaWhere.Append($" and XmlNotaFiscal.NF_DATA_EMISSAO >= ${filtrosPesquisaEnvioDevolucaoPallet.DataInicialNotaFiscal.ToString(pattern)}");

            if (filtrosPesquisaEnvioDevolucaoPallet.DataFinalNotaFiscal != DateTime.MinValue)
                clausulaWhere.Append($" and XmlNotaFiscal.NF_DATA_EMISSAO <= {filtrosPesquisaEnvioDevolucaoPallet.DataFinalNotaFiscal.ToString(pattern)}");

            if (filtrosPesquisaEnvioDevolucaoPallet.Transportador > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.EMP_CODIGO = {filtrosPesquisaEnvioDevolucaoPallet.Transportador}");

            if (filtrosPesquisaEnvioDevolucaoPallet.Cliente > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.CLI_CGCCPF = {filtrosPesquisaEnvioDevolucaoPallet.Cliente}");

            if (filtrosPesquisaEnvioDevolucaoPallet.Filial > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.FIL_CODIGO = {filtrosPesquisaEnvioDevolucaoPallet.Filial}");

            if (filtrosPesquisaEnvioDevolucaoPallet.Situacao.HasValue)
                clausulaWhere.Append($" and MovimentacaoPallet.MPT_SITUACAO = {(int)filtrosPesquisaEnvioDevolucaoPallet.Situacao}");

            if (filtrosPesquisaEnvioDevolucaoPallet.RegraPallet.HasValue)
                clausulaWhere.Append($" and movimentacaoPallet.MPT_REGRA_PALLET = {(int)filtrosPesquisaEnvioDevolucaoPallet.RegraPallet}");

            if (filtrosPesquisaEnvioDevolucaoPallet.ResponsavelPallet.HasValue)
                clausulaWhere.Append($" and MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)filtrosPesquisaEnvioDevolucaoPallet.ResponsavelPallet}");

            if (filtrosPesquisaEnvioDevolucaoPallet.UFOrigem.Count > 0)
                clausulaWhere.Append($" and LocalidadeRemetente.UF_SIGLA IN ('{string.Join("','", filtrosPesquisaEnvioDevolucaoPallet.UFOrigem)}')");

            if (filtrosPesquisaEnvioDevolucaoPallet.UFDestino.Count > 0)
                clausulaWhere.Append($" and LocalidadeDestinatario.UF_SIGLA IN ('{string.Join("','", filtrosPesquisaEnvioDevolucaoPallet.UFDestino)}')");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisaEnvioDevolucaoPallet.EscritorioVendas))
                clausulaWhere.Append($" and ClienteComplementar.CLC_ESCRITORIO_VENDAS = '{filtrosPesquisaEnvioDevolucaoPallet.EscritorioVendas}'");

            return clausulaWhere.ToString();
        }

        private string ObterSubFiltrosSaldoEstoque(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet)
        {
            StringBuilder clausulaWhere = new StringBuilder();

            clausulaWhere.Append($"where ControleEstoquePallet.CPT_TIPO_ESTOQUE_PALLET = {(int)TipoEstoquePallet.Movimentacao}");

            if (filtrosPesquisaSaldoControlePallet.Transportador > 0)
                clausulaWhere.Append($" and ControleEstoquePallet.EMP_CODIGO = {filtrosPesquisaSaldoControlePallet.Transportador}");

            if (filtrosPesquisaSaldoControlePallet.Cliente > 0)
                clausulaWhere.Append($" and ControleEstoquePallet.CLI_CGCCPF = {filtrosPesquisaSaldoControlePallet.Cliente}");

            if (filtrosPesquisaSaldoControlePallet.Filial > 0)
                clausulaWhere.Append($" and ControleEstoquePallet.FIL_CODIGO = {filtrosPesquisaSaldoControlePallet.Filial}");

            if (filtrosPesquisaSaldoControlePallet.ResponsavelMovimentacaoPallet.HasValue)
                clausulaWhere.Append($" and ControleEstoquePallet.CPT_RESPONSAVEL_PALLET = {(int)filtrosPesquisaSaldoControlePallet.ResponsavelMovimentacaoPallet}");

            return clausulaWhere.ToString();
        }

        private string ObterSubFiltrosSaldoMovimentacao(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet)
        {
            StringBuilder clausulaWhere = new StringBuilder();

            clausulaWhere.Append($"where MovimentacaoPallet.MPT_SITUACAO = {(int)SituacaoGestaoPallet.Pendente}");

            if (filtrosPesquisaSaldoControlePallet.Transportador > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.EMP_CODIGO = {filtrosPesquisaSaldoControlePallet.Transportador}");

            if (filtrosPesquisaSaldoControlePallet.Cliente > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.CLI_CGCCPF = {filtrosPesquisaSaldoControlePallet.Cliente}");

            if (filtrosPesquisaSaldoControlePallet.Filial > 0)
                clausulaWhere.Append($" and MovimentacaoPallet.FIL_CODIGO = {filtrosPesquisaSaldoControlePallet.Filial}");

            if (filtrosPesquisaSaldoControlePallet.ResponsavelMovimentacaoPallet.HasValue)
                clausulaWhere.Append($" and MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)filtrosPesquisaSaldoControlePallet.ResponsavelMovimentacaoPallet}");

            return clausulaWhere.ToString();
        }

        private StringBuilder ObterSubConsultaSaldoMovimentacao(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@$"
                select 
                    (case 
                        when MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)ResponsavelPallet.Filial} then Filial.FIL_DESCRICAO 
                        when MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)ResponsavelPallet.Cliente} then Cliente.CLI_NOME
                        else Empresa.EMP_RAZAO 
                    end) as Responsavel,
                    (case 
                        when MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)ResponsavelPallet.Filial} then Filial.FIL_CNPJ 
                        when MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = {(int)ResponsavelPallet.Cliente} then LTRIM(STR(Cliente.CLI_CGCCPF, 25, 0))
                        else Empresa.EMP_CNPJ 
                    end) as ResponsavelCNPJ,
                    sum(MovimentacaoPallet.MPT_QUANTIDADE_PALLETS) as PalettsPendente,
                    0 as QuantidadeTotalPallets "
            );

            sql.Append(@"from 
                    T_MOVIMENTACAO_PALLET MovimentacaoPallet
                left join
                    T_FILIAL Filial on MovimentacaoPallet.FIL_CODIGO = Filial.FIL_CODIGO
                left join
                    T_EMPRESA Empresa on MovimentacaoPallet.EMP_CODIGO = Empresa.EMP_CODIGO
                left join
                    T_CLIENTE Cliente on MovimentacaoPallet.CLI_CGCCPF = Cliente.CLI_CGCCPF "
            );

            sql.Append(ObterSubFiltrosSaldoMovimentacao(filtrosPesquisaSaldoControlePallet));

            sql.Append(@" group by 
                    MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET,
                    Filial.FIL_DESCRICAO,
                    Empresa.EMP_RAZAO,
                    Cliente.CLI_NOME,
	                Filial.FIL_CNPJ,
	                Cliente.CLI_CGCCPF,
	                Empresa.EMP_CNPJ"
            );

            return sql;
        }

        private StringBuilder ObterSubConsultaSaldoEstoque(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtrosPesquisaSaldoControlePallet)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@$"
                select 
                    (case
                        when ControleEstoquePallet.CPT_RESPONSAVEL_PALLET = {(int)ResponsavelPallet.Filial} then Filial.FIL_DESCRICAO 
                        when ControleEstoquePallet.CPT_RESPONSAVEL_PALLET = {(int)ResponsavelPallet.Cliente} then Cliente.CLI_NOME
                        else Empresa.EMP_RAZAO 
                    end) as Responsavel,
                    (case 
                        when ControleEstoquePallet.CPT_RESPONSAVEL_PALLET = {(int)ResponsavelPallet.Filial} then Filial.FIL_CNPJ 
                        when ControleEstoquePallet.CPT_RESPONSAVEL_PALLET = {(int)ResponsavelPallet.Cliente} then LTRIM(STR(Cliente.CLI_CGCCPF, 25, 0))
                        else Empresa.EMP_CNPJ 
                    end) as ResponsavelCNPJ,
                    0 as PalettsPendente,
                    max(ControleEstoquePallet.CPT_QUANTIDADE_TOTAL_PALLETS) as QuantidadeTotalPallets "
            );

            sql.Append(@"from 
                    T_CONTROLE_ESTOQUE_PALLET ControleEstoquePallet
                left join
                    T_FILIAL Filial on ControleEstoquePallet.FIL_CODIGO = Filial.FIL_CODIGO
                left join
                    T_EMPRESA Empresa on ControleEstoquePallet.EMP_CODIGO = Empresa.EMP_CODIGO
                left join
                    T_CLIENTE Cliente on ControleEstoquePallet.CLI_CGCCPF = Cliente.CLI_CGCCPF "
            );

            sql.Append(ObterSubFiltrosSaldoEstoque(filtrosPesquisaSaldoControlePallet));

            sql.Append(@" group by 
                    ControleEstoquePallet.CPT_RESPONSAVEL_PALLET,
                    Filial.FIL_DESCRICAO,
                    Empresa.EMP_RAZAO,
                    Cliente.CLI_NOME,
	                Filial.FIL_CNPJ,
	                Cliente.CLI_CGCCPF,
	                Empresa.EMP_CNPJ"
            );

            return sql;
        }

        #endregion Métodos Privados
    }
}
