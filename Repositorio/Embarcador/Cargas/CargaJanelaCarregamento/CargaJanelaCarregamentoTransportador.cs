using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Logistica.AcompanhamentoChecklist;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaJanelaCarregamentoTransportador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ConsultarPorTransportador(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {

            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    (o.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador || o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Disponivel) &&
                    (o.CargaJanelaCarregamento.Excedente == false || o.CargaJanelaCarregamento.CentroCarregamento.PermiteTransportadorSelecionarHorarioCarregamento) &&
                    (o.HorarioLiberacao == null || o.HorarioLiberacao <= DateTime.Now) &&
                    (o.Bloqueada == false) &&
                    (o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null) &&
                    (o.CargaJanelaCarregamento.Carga.CargaVinculada == null) &&
                    (o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Anulada) &&
                    (
                        filtrosPesquisa.ExibirCargaSemValorFrete ||
                        o.CargaJanelaCarregamento.Carga.ValorFreteAPagar > 0 ||
                        o.CargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete ||
                        o.CargaJanelaCarregamento.Carga.TipoOperacao.FretePorContadoCliente
                    )
                );

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o =>
                    o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador
                    || (o.CargaJanelaCarregamento.CentroCarregamento.PermitirMatrizSelecionarFilial && o.Transportador.Matriz.Any(t => t.Codigo == filtrosPesquisa.CodigoTransportador))
                    || (o.CargaJanelaCarregamento.CentroCarregamento.PermitirMatrizSelecionarFilial && o.Transportador.Filiais.Any(t => t.Codigo == filtrosPesquisa.CodigoTransportador))
                );

                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.
                    Where(o => o.Transportador.FiliaisEmbarcadorHabilitado.Count() == 0
                    || o.Transportador.FiliaisEmbarcadorHabilitado.Select(x => x.Codigo).Contains(o.CargaJanelaCarregamento.Carga.Filial.Codigo));
            }

            if (configuracaoGeralCarga?.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual ?? false)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga> consultaMensagemAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga>();
                consultaMensagemAlerta = consultaMensagemAlerta.Where(o =>
                   o.Confirmada == false
                   && ((bool?)o.Bloquear ?? false) == true
                   && o.Tipo == TipoMensagemAlerta.CargaAguardandoDesbloqueio
                );

                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o =>
                     o.CargaJanelaCarregamento.Carga.Empresa != null
                     && !consultaMensagemAlerta.Any(alerta => alerta.Entidade.Codigo == o.CargaJanelaCarregamento.Carga.Codigo)
                 );
            }


            if (filtrosPesquisa.CodigoClienteTerceiro > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.Terceiro.CPF_CNPJ == filtrosPesquisa.CodigoClienteTerceiro);

            if (filtrosPesquisa.CodigoOrigem > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.Pedidos.Where(p => p.Origem.Codigo == filtrosPesquisa.CodigoOrigem).Any());

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.Pedidos.Where(p => p.Destino.Codigo == filtrosPesquisa.CodigoDestino).Any());

            if (filtrosPesquisa.CodigoRota > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.Rota.Codigo == filtrosPesquisa.CodigoRota);

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicular);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => (o.CargaJanelaCarregamento.InicioCarregamento >= filtrosPesquisa.DataInicial.Value.Date)); //|| o.CargaJanelaCarregamento.Carga.DataCarregamentoCarga == null

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => (o.CargaJanelaCarregamento.InicioCarregamento < filtrosPesquisa.DataFinal.Value.AddDays(1).Date)); //|| o.CargaJanelaCarregamento.Carga.DataCarregamentoCarga == null

            if (filtrosPesquisa.Situacao.HasValue)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.TipoLiberacao == TipoLiberacaoCargaJanelaCarregamento.Normal)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => ((bool?)o.CargaJanelaCarregamento.CargaLiberadaCotacao ?? false) == false);
            else if (filtrosPesquisa.TipoLiberacao == TipoLiberacaoCargaJanelaCarregamento.Cotacao)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.CargaLiberadaCotacao == true);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador));

            if (filtrosPesquisa.CodigosCargasVinculadas != null)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => filtrosPesquisa.CodigosCargasVinculadas.Contains(o.CargaJanelaCarregamento.Carga.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroExp))
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.Pedidos.Any(p => p.Pedido.NumeroEXP == filtrosPesquisa.NumeroExp));

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.TipoDeCarga.Codigo == filtrosPesquisa.CodigoTipoCarga);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigosFiliais.Count > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => filtrosPesquisa.CodigosFiliais.Contains(o.CargaJanelaCarregamento.Carga.Filial.Codigo));

            if (filtrosPesquisa.NaoRetornarCargasMarcadoSemInteresse)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o => !o.SemInteresseTransportador.Value || o.SemInteresseTransportador == null);

            return consultaCargaJanelaCarregamentoTransportador;
        }
        private NHibernate.IQuery ConsultaPorChecklist(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
            {
                sql.Append(@"SELECT COUNT(DISTINCT cargaJanelaCarregamentoTransportador.JCT_CODIGO)");
            }
            else
            {
                sql.AppendLine(@"                
                            SELECT DISTINCT
                                cargaJanelaCarregamentoTransportador.JCT_CODIGO AS Codigo,
                                carga.CAR_CODIGO AS CodigoCarga,
                                carga.CAR_CODIGO_CARGA_EMBARCADOR AS Carga,
                                filial.FIL_DESCRICAO AS Filial,
                                (
                                    SELECT _veiculo.VEI_PLACA 
                                    FROM T_VEICULO _veiculo 
                                    WHERE _veiculo.VEI_CODIGO = carga.CAR_VEICULO
                                ) + ISNULL(( 
                                    SELECT ', ' + _veiculo.VEI_PLACA 
                                    FROM T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga 
                                    JOIN T_VEICULO _veiculo ON _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO 
                                    WHERE _veiculovinculadocarga.CAR_CODIGO = carga.CAR_CODIGO 
                                    FOR XML PATH('')
                                ), '') AS Veiculos,
                                empresa.EMP_RAZAO AS TransportadorNome,
                                empresa.EMP_CNPJ AS TransportadorCNPJ,
                                (
                                    SELECT TOP 1 checklist.CTC_CHECKLIST_VISUALIZADO
                                    FROM T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST checklist
                                    WHERE checklist.CJT_CODIGO = cargaJanelaCarregamentoTransportador.JCT_CODIGO
                                    ORDER BY ISNULL(checklist.CTC_CHECKLIST_VISUALIZADO, 0) ASC
                                ) AS Situacao
                ");
            }

            sql.Append(ObterJoinsAcompanhamentoChecklist());
            sql.Append(ObterFiltrosAcompanhamentoChecklist(filtroPesquisa));

            if (!somenteContarNumeroRegistros)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                {
                    sql.AppendLine(" ORDER BY " + parametrosConsulta.PropriedadeOrdenar + " " +
                        (parametrosConsulta.DirecaoOrdenar == "asc" ? "ASC" : "DESC"));
                }
                else
                {
                    sql.AppendLine(" ORDER BY cargaJanelaCarregamentoTransportador.JCT_CODIGO DESC");
                }

                if (parametrosConsulta.LimiteRegistros > 0)
                {
                    sql.AppendLine(" OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH NEXT " +
                        parametrosConsulta.LimiteRegistros + " ROWS ONLY");
                }
            }

            return SessionNHiBernate.CreateSQLQuery(sql.ToString());
        }

        private NHibernate.IQuery ObterSqlDetalhesChecklist(int codigoJanelaCarregamentoTransportador, List<int> codigoVeiculo, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
            {
                sql.AppendLine($@"SELECT DISTINCT(COUNT(0) OVER ())");
            }
            else
            {
                sql.AppendLine($@"SELECT 
                                    checklist.CTC_ORDEM_CARGA_CHECKLIST                                  AS OrdemChecklist,
                                    veiculo.VEI_PLACA                                                    AS Placa,
                                 grupoProduto.GRP_DESCRICAO                                              AS Produto,
                                 checklist.CTC_REGIME_LIMPEZA			                                 AS RegimeLimpezaCarga
                ");
            }

            sql.AppendLine($@" FROM T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR cargaJanelaCarregamentoTransportador
                                 JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST checklist		 ON cargaJanelaCarregamentoTransportador.JCT_CODIGO = checklist.CJT_CODIGO
                                 JOIN T_CARGA_JANELA_CARREGAMENTO cargaJanelaCarregamento 				 ON cargaJanelaCarregamentoTransportador.CJC_CODIGO = cargaJanelaCarregamento.CJC_CODIGO
                                 JOIN T_CARGA carga 												     ON cargaJanelaCarregamento.CAR_CODIGO = carga.CAR_CODIGO
                                 JOIN T_VEICULO veiculo 											     ON checklist.VEI_CODIGO = veiculo.VEI_CODIGO  
                                 JOIN T_GRUPO_PRODUTO grupoProduto										 ON checklist.GPR_CODIGO = grupoProduto.GPR_CODIGO
                                 WHERE  cargaJanelaCarregamentoTransportador.JCT_CODIGO  = :codigo AND AND CodigoVeiculo IN @codigosVeiculo""");

            return SessionNHiBernate.CreateSQLQuery(sql.ToString())
                .SetParameter("codigo", codigoJanelaCarregamentoTransportador)
                .SetParameter("codigosVeiculo", codigoVeiculo);
        }

        private string ObterFiltrosAcompanhamentoChecklist(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist filtroPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" WHERE 1 = 1 ");

            if (filtroPesquisa.Filial > 0)
                sql.AppendLine("  AND filial.FIL_CODIGO = " + filtroPesquisa.Filial);

            if (filtroPesquisa.TipoOperacao > 0)
                sql.AppendLine("  AND carga.TOP_CODIGO = " + filtroPesquisa.TipoOperacao);

            if (filtroPesquisa.Transportador > 0)
                sql.AppendLine("  AND empresa.EMP_CODIGO = " + filtroPesquisa.Transportador);

            if (filtroPesquisa.Situacao.HasValue)
            {
                if (filtroPesquisa.Situacao.Value)
                {
                    sql.AppendLine("  AND checklist.CTC_CHECKLIST_VISUALIZADO = 1");
                }
                else
                {
                    sql.AppendLine("  AND (checklist.CTC_CHECKLIST_VISUALIZADO IS NULL OR checklist.CTC_CHECKLIST_VISUALIZADO != 1)");
                }
            }

            if (filtroPesquisa.DataCarregamento != DateTime.MinValue)
                sql.AppendLine("  AND CONVERT(date, carga.CAR_DATA_CARREGAMENTO) = '" +
                    filtroPesquisa.DataCarregamento.ToString("yyyy-MM-dd") + "'");

            if (filtroPesquisa.CodigosCarga.Count > 0)
                sql.AppendLine($" AND carga.CAR_CODIGO IN ({string.Join(", ", filtroPesquisa.CodigosCarga)})");

            return sql.ToString();
        }

        private string ObterJoinsAcompanhamentoChecklist()
        {
            StringBuilder from = new StringBuilder();

            from.AppendLine("FROM T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR cargaJanelaCarregamentoTransportador");
            from.AppendLine("JOIN T_CARGA_JANELA_CARREGAMENTO cargaJanelaCarregamento ON cargaJanelaCarregamentoTransportador.CJC_CODIGO = cargaJanelaCarregamento.CJC_CODIGO");
            from.AppendLine("JOIN T_CARGA carga ON cargaJanelaCarregamento.CAR_CODIGO = carga.CAR_CODIGO");
            from.AppendLine("JOIN T_FILIAL filial ON carga.FIL_CODIGO = filial.FIL_CODIGO");
            from.AppendLine("JOIN T_EMPRESA empresa ON cargaJanelaCarregamentoTransportador.EMP_CODIGO = empresa.EMP_CODIGO");
            from.AppendLine("JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST checklist ON cargaJanelaCarregamentoTransportador.JCT_CODIGO = checklist.CJT_CODIGO");
            from.AppendLine("JOIN T_VEICULO veiculo ON carga.CAR_VEICULO = veiculo.VEI_CODIGO");

            return from.ToString();
        }

        #endregion

        #region Métodos Públicos

        public int ContarNumeroCargasAguardandAceiteOuConfirmacaoPorTipoCargaEPallets(int numeroPallet, int transportador, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                   (o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite || o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao) &&
                   o.Transportador.Codigo == transportador &&
                   o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null &&
                   o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                 );

            consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o =>
                possiveisModelos.Contains(o.CargaJanelaCarregamento.Carga.ModeloVeicularCarga) &&
                o.CargaJanelaCarregamento.Carga.ModeloVeicularCarga.NumeroPaletes >= numeroPallet
            );

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public int ContarNumeroCargasAguardandAceiteOuConfirmacaoPorTipoCargaEPalletsTransportadorTerceiro(int numeroPallet, double codigoTransportadorTerceiro, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                   (o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite || o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao) &&
                   o.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro &&
                   o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null &&
                   o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                 );

            consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Where(o =>
                possiveisModelos.Contains(o.CargaJanelaCarregamento.Carga.ModeloVeicularCarga) &&
                o.CargaJanelaCarregamento.Carga.ModeloVeicularCarga.NumeroPaletes >= numeroPallet
            );

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public List<int> BuscarTransportadoresDisponibilizadosRejeitaram(int janelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == janelaCarregamento && o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return consultaCargaJanelaCarregamentoTransportador.Select(obj => obj.Transportador.Codigo).ToList();
        }

        public List<int> BuscarTransportadoresDisponibilizadosRejeitaram(int janelaCarregamento, TipoCargaJanelaCarregamentoTransportador tipo)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == janelaCarregamento && o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Rejeitada && o.Tipo == tipo);

            return consultaCargaJanelaCarregamentoTransportador.Select(obj => obj.Transportador.Codigo).ToList();
        }

        public List<SituacaoCargaJanelaCarregamentoTransportador> BuscarSituacoesTransportadoresSecundarios(int janelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == janelaCarregamento && o.Tipo != TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRota);

            return consultaCargaJanelaCarregamentoTransportador.Select(o => o.Situacao).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> BuscarTotalInteressados(List<int> codigosCargaJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
            var result = from obj in query
                         where codigosCargaJanela.Contains(obj.CargaJanelaCarregamento.Codigo)
                         && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador.ComInteresse
                         select obj;

            return result.GroupBy(o => new { o.CargaJanelaCarregamento.Codigo }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento()
            {
                Codigo = obj.Key.Codigo,
                NumeroInteressados = obj.Count()
            }).ToList();
        }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> BuscarTotalVisualizacoes(List<int> codigosCargaJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>();
            var result = from obj in query
                         where codigosCargaJanela.Contains(obj.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo)
                         && obj.Tipo == TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga
                         select obj;

            return result.GroupBy(o => new { o.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento()
            {
                Codigo = obj.Key.Codigo,
                NumeroVisualizacoes = obj.Count()
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarCargasComTabelaPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == carga && obj.PossuiFreteCalculado && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCodigo(int codigoCargaJanelaCarregamentoTransportador)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.Codigo == codigoCargaJanelaCarregamentoTransportador);

            return consultaCargaJanelaCarregamentoTransportador
                .Fetch(o => o.CargaJanelaCarregamento).ThenFetch(o => o.Carga)
                .Fetch(o => o.Transportador)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarSemRejeicao(int codigoCargaJanelaCarregamento, int codigoTransportador)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.Transportador.Codigo == codigoTransportador && o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return consultaCargaJanelaCarregamentoTransportador
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarSemRejeicao(int codigoCargaJanelaCarregamento, double codigoTransportadorTerceiro)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro && o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return consultaCargaJanelaCarregamentoTransportador
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public List<int> BuscarCodigosPorTempoAceiteEncerrado(int limiteRegistros)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.HorarioLimiteConfirmarCarga < DateTime.Now &&
                    o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite &&
                    (
                        (o.CargaJanelaCarregamento.Carga != null && o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null) ||
                        (o.CargaJanelaCarregamento.PreCarga != null && o.CargaJanelaCarregamento.Carga == null)
                    )
                );

            return consultaCargaJanelaCarregamentoTransportador
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<int> BuscarCodigosPorTempoInteresseEncerrado(int limiteRegistros)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.HorarioLimiteConfirmarCarga < DateTime.Now &&
                    o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Disponivel &&
                    o.Tipo == TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCargaSecundario &&
                    (
                        (o.CargaJanelaCarregamento.Carga != null && o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null) ||
                        (o.CargaJanelaCarregamento.PreCarga != null && o.CargaJanelaCarregamento.Carga == null)
                    )
                );

            return consultaCargaJanelaCarregamentoTransportador
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }
        public List<int> BuscarCodigosPorTempoInteresseEncerradoTransportadorTerceiro(int limiteRegistros)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.HorarioLimiteConfirmarCarga < DateTime.Now &&
                    o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Disponivel &&
                    o.Tipo == TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorTerceiroCargaSecundario &&
                    (
                        (o.CargaJanelaCarregamento.Carga != null && o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null) ||
                        (o.CargaJanelaCarregamento.PreCarga != null && o.CargaJanelaCarregamento.Carga == null)
                    )
                );

            return consultaCargaJanelaCarregamentoTransportador
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<int> BuscarCodigosPorTempoAceiteExpirandoParaNotificarPorEmail(int limiteRegistros)
        {
            int tempoAntecedenciaPrazoEncerradoEmMinutos = 60;
            int tempoMinimoAguardarConfirmacaoTransportadorParaPermitirNotificacaoEmMinutos = 80;
            DateTime horarioNotificacaoTempoAceiteExpirando = DateTime.Now.AddMinutes(tempoAntecedenciaPrazoEncerradoEmMinutos);

            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.HorarioLimiteConfirmarCarga < horarioNotificacaoTempoAceiteExpirando &&
                    o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite &&
                    (o.CargaJanelaCarregamento.Carga != null || o.CargaJanelaCarregamento.PreCarga != null) &&
                    ((bool?)o.EmailTempoAceiteExpirandoEnviado ?? false) == false &&
                    o.CargaJanelaCarregamento.CentroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente >= tempoMinimoAguardarConfirmacaoTransportadorParaPermitirNotificacaoEmMinutos
                );

            return consultaCargaJanelaCarregamentoTransportador
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public void SetarCargaJanelaCarregamentoTransportadoInteresseRejeitado(int cargaJanelaCarregamento, int codigoJanelaTransportadorDesconsiderar)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO(JCT_CODIGO, JTH_DATA, JTH_DESCRICAO, JTH_TIPO)
                       select JCT_CODIGO, getdate(), 'Interesse na carga rejeitado para o transportador', {(int)TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao}
                         from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR
                        where CJC_CODIGO = {cargaJanelaCarregamento}
                          and JCT_SITUACAO <> {(int)SituacaoCargaJanelaCarregamentoTransportador.Disponivel}
                          and JCT_SITUACAO <> {(int)SituacaoCargaJanelaCarregamentoTransportador.Rejeitada}
                          and JCT_CODIGO <> {codigoJanelaTransportadorDesconsiderar}"
                )
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateQuery(
                    @"update CargaJanelaCarregamentoTransportador
                         set Situacao = :situacao,
                             HorarioLimiteConfirmarCarga = null
                       where CargaJanelaCarregamento.Codigo = :cargaJanelaCarregamento
                         and Situacao <> :situacaoWhere
                         and Codigo <> :codigoJanelaTransportadorDesconsiderar"
                )
                .SetInt32("cargaJanelaCarregamento", cargaJanelaCarregamento)
                .SetEnum("situacao", SituacaoCargaJanelaCarregamentoTransportador.Rejeitada)
                .SetEnum("situacaoWhere", SituacaoCargaJanelaCarregamentoTransportador.Disponivel)
                .SetInt32("codigoJanelaTransportadorDesconsiderar", codigoJanelaTransportadorDesconsiderar)
                .ExecuteUpdate();
        }

        public void SetarCargaJanelaCarregamentoTransportadoRejeitada(int cargaJanelaCarregamento, int codigoJanelaTransportadorDesconsiderar)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO(JCT_CODIGO, JTH_DATA, JTH_DESCRICAO, JTH_TIPO)
                       select JCT_CODIGO, getdate(), 'Carga rejeitada para o transportador', {(int)TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao}
                         from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR
                        where CJC_CODIGO = {cargaJanelaCarregamento}
                          and JCT_SITUACAO <> {(int)SituacaoCargaJanelaCarregamentoTransportador.Rejeitada}
                          and JCT_CODIGO <> {codigoJanelaTransportadorDesconsiderar}"
                )
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateQuery(
                    @"update CargaJanelaCarregamentoTransportador
                         set Situacao = :situacao,
                             HorarioLimiteConfirmarCarga = null
                       where CargaJanelaCarregamento.Codigo = :cargaJanelaCarregamento
                         and Codigo <> :codigoJanelaTransportadorDesconsiderar"
                )
                .SetInt32("cargaJanelaCarregamento", cargaJanelaCarregamento)
                .SetEnum("situacao", SituacaoCargaJanelaCarregamentoTransportador.Rejeitada)
                .SetInt32("codigoJanelaTransportadorDesconsiderar", codigoJanelaTransportadorDesconsiderar)
                .ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPrimeiraJanelaOfertada(int cargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == cargaJanelaCarregamento && obj.PrimeiroTransportadorOfertado == true select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> Buscar(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaESituacao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Situacao == situacao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaESituacaoDiferente(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Situacao != situacao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaESituacoesDiferente(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador> situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && !situacoes.Contains(o.Situacao));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCargaJanelaCarregamentoESituacao(int codigoCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Situacao == situacao select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaJanelaCarregamentoAsync(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento select obj;

            return result.FirstOrDefaultAsync();
        }

        public int ContarPorCargaJanelaCarregamentoESituacao(int codigoCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Situacao == situacao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaJanelaCarregamentoESituacao(int codigoCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && situacao.Contains(obj.Situacao) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPendenteEnviarEmailPorPrazoEsgotado(int tempoEmMinutosEnviarEmailAntesInicioCarregamento, int limiteRegistros)
        {
            DateTime inicioCarregamentoPrazoEsgotado = DateTime.Now.AddMinutes(tempoEmMinutosEnviarEmailAntesInicioCarregamento);

            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.EmailEnviado == false &&
                    (
                        o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao ||
                        o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Confirmada
                    ) &&
                    o.CargaJanelaCarregamento.InicioCarregamento < inicioCarregamentoPrazoEsgotado &&
                    o.CargaJanelaCarregamento.InicioCarregamento >= DateTime.Now.Date &&
                    o.CargaJanelaCarregamento.Carga.CargaAgrupamento == null &&
                    o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (
                        o.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador ||
                        o.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgEncosta ||
                        o.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento
                    )
                );

            return consultaCargaJanelaCarregamentoTransportador.Take(limiteRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCargaETransportador(int codigoCarga, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Transportador.Codigo == codigoTransportador && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.CentroCarregamento)
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.Carga)
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCargaETransportadorTerceiro(int codigoCarga, double codigoTransportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.CentroCarregamento)
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.Carga)
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCargaJanelaCarregamentoETransportador(int codigoCargaJanelaCarregamento, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Transportador.Codigo == codigoTransportador && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.CentroCarregamento)
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.Carga)
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador BuscarPorCargaJanelaCarregamentoEOrdem(int codigoCargaJanelaCarregamento, int ordem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(obj => obj.Ordem == ordem && obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargasETransportadores(List<int> codigosCarga, List<int> codigoTransportadores)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where codigosCarga.Contains(obj.CargaJanelaCarregamento.Carga.Codigo) && codigoTransportadores.Contains(obj.Transportador.Codigo) && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargasETransportadorTerceiro(List<int> codigosCarga, double codigoTransportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where codigosCarga.Contains(obj.CargaJanelaCarregamento.Carga.Codigo) && obj.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro && obj.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaJanelaCarregamentoDisponivel(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(obj => obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Disponivel)
                .Fetch(obj => obj.Transportador);

            return query.ToList();
        }

        public void BloquearTodasPorPrioridadeRotaGrupo(int codigoCargaJanelaCarregamento, int codigoCargaJanelaCarregamentoTransportadorDesconsiderar)
        {
            UnitOfWork.Sessao
                .CreateQuery(@"
                    update CargaJanelaCarregamentoTransportador
                       set Bloqueada = 1
                     where CargaJanelaCarregamento.Codigo = :codigoJanelaCarregamento
                       and Codigo <> :codigoJanelaCarregamentoTransportadorDesconsiderar
                       and Tipo = :tipo
                       and Situacao <> :situacao"
                )
                .SetInt32("codigoJanelaCarregamento", codigoCargaJanelaCarregamento)
                .SetInt32("codigoJanelaCarregamentoTransportadorDesconsiderar", codigoCargaJanelaCarregamentoTransportadorDesconsiderar)
                .SetEnum("tipo", TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo)
                .SetEnum("situacao", SituacaoCargaJanelaCarregamentoTransportador.Rejeitada)
                .ExecuteUpdate();
        }

        public void DesbloquearTodasPorPrioridadeRotaGrupo(int codigoCargaJanelaCarregamento)
        {
            UnitOfWork.Sessao
                .CreateQuery(@"
                    update CargaJanelaCarregamentoTransportador
                       set Bloqueada = 0
                     where CargaJanelaCarregamento.Codigo = :codigoJanelaCarregamento
                       and Tipo = :tipo
                       and Bloqueada = 1"
                )
                .SetInt32("codigoJanelaCarregamento", codigoCargaJanelaCarregamento)
                .SetEnum("tipo", TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresPorPrioridadeRotaGrupo(int codigoFilial, int codigoTipoCarga, int diasHistorico)
        {
            var consultaModeloVeicularCargaPermitido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>()
                .Where(o => o.TipoDeCarga.Codigo == codigoTipoCarga)
                .Select(o => o.ModeloVeicularCarga);

            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o =>
                    o.Empresa != null &&
                    o.Filial.Codigo == codigoFilial &&
                    o.TipoDeCarga.Codigo == codigoTipoCarga &&
                    o.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.SituacaoCarga != SituacaoCarga.Cancelada
                );

            if (diasHistorico > 0)
            {
                DateTime dataEmissaoInicial = DateTime.Today.AddDays(-diasHistorico);
                DateTime dataEmissaoLimite = DateTime.Today.Add(DateTime.MaxValue.TimeOfDay);

                consultaCarga = consultaCarga.Where(o => o.DataFinalizacaoEmissao != null && o.DataFinalizacaoEmissao >= dataEmissaoInicial && o.DataFinalizacaoEmissao <= dataEmissaoLimite);
            }

            var consultaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(o =>
                    o.Empresa != null &&
                    o.Empresa.Status == "A" &&
                    o.Ativo &&
                    (o.ModeloVeicularCarga == null || consultaModeloVeicularCargaPermitido.Any(m => m.Codigo == o.ModeloVeicularCarga.Codigo)) &&
                    consultaCarga.Any(c => c.Empresa.Codigo == o.Empresa.Codigo)
                );

            return consultaVeiculo
                .Select(o => o.Empresa)
                .Distinct()
                .ToList();
        }

        public bool VerificarExiste(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento);

            return consultaCargaJanelaCarregamentoTransportador.Count() > 0;
        }

        public bool VerificarExisteBloqueadasPorPrioridadeRotaGrupo(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento &&
                    o.Tipo == TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo &&
                    o.Bloqueada == true
                );

            return consultaCargaJanelaCarregamentoTransportador.Count() > 0;
        }

        public bool VerificarExisteDesbloqueadasSemRejeicaoPorPrioridadeRotaGrupo(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento &&
                    o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada &&
                    o.Tipo == TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo &&
                    o.Bloqueada == false
                );

            return consultaCargaJanelaCarregamentoTransportador.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ConsultarPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento &&
                    (o.TabelaFreteCliente == null || o.TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete != TipoFreteTabelaFrete.Proprio)
                );

            consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador
                .Fetch(o => o.CargaJanelaCarregamento).ThenFetch(o => o.Carga)
                .Fetch(o => o.TabelaFreteCliente).ThenFetch(o => o.TabelaFrete)
                .Fetch(o => o.Transportador);

            if (parametrosConsulta.InicioRegistros > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador.Take(parametrosConsulta.LimiteRegistros);

            return consultaCargaJanelaCarregamentoTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ConsultarPorTransportador(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            var result = ConsultarPorTransportador(filtrosPesquisa, configuracaoGeralCarga);

            result = result.OrderBy(o => o.CargaJanelaCarregamento.InicioCarregamento);

            if (parametrosConsulta.InicioRegistros > 0)
                result = result.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                result = result.Take(parametrosConsulta.LimiteRegistros);

            return result
                .Fetch(obj => obj.CargaJanelaCarregamento)
                .ThenFetch(obj => obj.Carga)
                .ToList();
        }

        public int ContarConsultaPorTransportador(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            var result = ConsultarPorTransportador(filtrosPesquisa, configuracaoGeralCarga);

            return result.Count();
        }

        public int ContarConsultaPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento &&
                    (o.TabelaFreteCliente == null || o.TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete != TipoFreteTabelaFrete.Proprio)
                );

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public int ContarConsultaPorCargaJanelaCarregamentoDesbloqueada(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && o.Bloqueada == false);

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public int ContarPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento);

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public List<int> BuscarTransportadoresPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento select obj.Transportador.Codigo;

            return result.ToList();
        }

        public List<double> BuscarTransportadoresTerceirosPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento select obj.Terceiro.CPF_CNPJ;

            return result.ToList();
        }

        public int ContarPorSituacoes(List<SituacaoCargaJanelaCarregamentoTransportador> situacoes, int codigoTransportador)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => situacoes.Contains(o.Situacao) && o.Transportador.Codigo == codigoTransportador);

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public int ContarPorSituacoesTransportadorTerceiro(List<SituacaoCargaJanelaCarregamentoTransportador> situacoes, double codigoTransportadorTerceiro)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => situacoes.Contains(o.Situacao) && o.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro);

            return consultaCargaJanelaCarregamentoTransportador.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarCargaComInteressePorCarga(int transportador, int numeroPallet, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador.ComInteresse && obj.Transportador.Codigo == transportador && obj.CargaJanelaCarregamento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;

            if (numeroPallet > 0)
                result = result.Where(obj => obj.CargaJanelaCarregamento.Carga.ModeloVeicularCarga.NumeroPaletes <= numeroPallet);

            if (possiveisModelos != null && possiveisModelos.Count > 0)
                result = result.Where(obj => possiveisModelos.Contains(obj.CargaJanelaCarregamento.Carga.ModeloVeicularCarga));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarCargaComInteressePorCargaTransportardorTerceiro(double codigoTransportadorTerceiro, int numeroPallet, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> possiveisModelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador.ComInteresse && obj.Terceiro.CPF_CNPJ == codigoTransportadorTerceiro && obj.CargaJanelaCarregamento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;

            if (numeroPallet > 0)
                result = result.Where(obj => obj.CargaJanelaCarregamento.Carga.ModeloVeicularCarga.NumeroPaletes <= numeroPallet);

            if (possiveisModelos != null && possiveisModelos.Count > 0)
                result = result.Where(obj => possiveisModelos.Contains(obj.CargaJanelaCarregamento.Carga.ModeloVeicularCarga));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargaComInteresse(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            var result = from obj in query
                         where
                             obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador.ComInteresse
                             && obj.CargaJanelaCarregamento.Carga.Codigo == codigoCarga
                             && obj.CargaJanelaCarregamento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorInteressadosComValorInformado(int codigoJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.ComInteresse &&
                    o.CargaJanelaCarregamento.Codigo == codigoJanelaCarregamento &&
                    o.ValorFreteTransportador > 0
                );

            consultaCargaJanelaCarregamentoTransportador = consultaCargaJanelaCarregamentoTransportador
                .Fetch(o => o.Transportador)
                .Fetch(o => o.DadosTransporte).ThenFetch(o => o.Veiculo).ThenFetch(o => o.ModeloCarroceria);

            return consultaCargaJanelaCarregamentoTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargasESituacao(List<int> codigosCarga, SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => codigosCarga.Contains(o.CargaJanelaCarregamento.Carga.Codigo) && o.Situacao == situacao);

            return consultaCargaJanelaCarregamentoTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorCargasESituacoesDiferente(List<int> codigosCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador> situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => codigosCarga.Contains(o.CargaJanelaCarregamento.Carga.Codigo) && !situacoes.Contains(o.Situacao));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorJanelasCarregamentoESituacao(List<int> codigosCargaJanelaCarregamento, SituacaoCargaJanelaCarregamentoTransportador situacao)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => codigosCargaJanelaCarregamento.Contains(o.CargaJanelaCarregamento.Codigo) && o.Situacao == situacao);

            return consultaCargaJanelaCarregamentoTransportador.ToList();
        }

        public bool PossuiSituacaoAguardandoAceite(int codigoTransportador)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.Transportador.Codigo == codigoTransportador &&
                    o.CargaJanelaCarregamento.CentroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras == true &&
                    o.CargaJanelaCarregamento.CentroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente > 0 &&
                    o.CargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    (o.CargaJanelaCarregamento.Carga.ValorFreteAPagar > 0 || o.CargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete)
                );

            return consultaCargaJanelaCarregamentoTransportador.Count() > 0;
        }

        /// <summary>
        /// Metodo expecifico para geracao da carga. favor nao usar
        /// </summary>
        public void InsertSQLListaCargajanelaCarregamentoTrasnportador(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargajanelaCarregamentoTrasnportadorInserir)
        {
            if (listaCargajanelaCarregamentoTrasnportadorInserir != null && listaCargajanelaCarregamentoTrasnportadorInserir.Count > 0)
            {
                int take = 200;
                int start = 0;

                while (start < listaCargajanelaCarregamentoTrasnportadorInserir.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> Listatemp = listaCargajanelaCarregamentoTrasnportadorInserir.Skip(start).Take(take).ToList();

                    string parameros = "( :CJC_CODIGO_[X], :JCT_HORARIO_LIBERACAO_[X], :EMP_CODIGO_[X], :JCT_SITUACAO_[X], :JCT_PENDENTE_CALCULAR_FRETE_[X], :JCT_TIPO_[X], :JCT_BLOQUEADA_[X], :CLI_CGCCPF_TERCEIRO_[X], :JCT_HORARIO_LIMITE_CONFIRMAR_CARGA_[X])";

                    string sqlQuery = @"
                        INSERT INTO T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR ( CJC_CODIGO, JCT_HORARIO_LIBERACAO, EMP_CODIGO, JCT_SITUACAO, JCT_PENDENTE_CALCULAR_FRETE, JCT_TIPO, JCT_BLOQUEADA, CLI_CGCCPF_TERCEIRO, JCT_HORARIO_LIMITE_CONFIRMAR_CARGA ) values " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < Listatemp.Count; i++)
                        sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                    object cod = DBNull.Value;
                    if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[0].CargaJanelaCarregamento.Codigo.ToString()))
                        cod = Listatemp[0].CargaJanelaCarregamento.Codigo;

                    int? codigoTransportador = null;
                    if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[0].Transportador?.Codigo.ToString()))
                        codigoTransportador = Listatemp[0].Transportador.Codigo;

                    double? codigoTerceiro = null;
                    if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[0].Terceiro?.CPF_CNPJ.ToString()))
                        codigoTerceiro = Listatemp[0].Terceiro.CPF_CNPJ;

                    query.SetParameter("CJC_CODIGO_0", cod);
                    query.SetParameter("JCT_HORARIO_LIBERACAO_0", Listatemp[0].HorarioLiberacao);
                    query.SetParameter<int?>("EMP_CODIGO_0", codigoTransportador);
                    query.SetParameter("JCT_SITUACAO_0", Listatemp[0].Situacao);
                    query.SetParameter("JCT_PENDENTE_CALCULAR_FRETE_0", Listatemp[0].PendenteCalcularFrete);
                    query.SetParameter("JCT_TIPO_0", Listatemp[0].Tipo);
                    query.SetParameter("JCT_BLOQUEADA_0", Listatemp[0].Bloqueada);
                    query.SetParameter<double?>("CLI_CGCCPF_TERCEIRO_0", codigoTerceiro);
                    query.SetParameter("JCT_HORARIO_LIMITE_CONFIRMAR_CARGA_0", Listatemp[0].HorarioLimiteConfirmarCarga);

                    for (int i = 1; i < Listatemp.Count; i++)
                    {
                        object codigoJanela = DBNull.Value;
                        if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[i].CargaJanelaCarregamento.Codigo.ToString()))
                            codigoJanela = Listatemp[i].CargaJanelaCarregamento.Codigo;

                        int? codigoTransp = null;
                        if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[i].Transportador?.Codigo.ToString()))
                            codigoTransp = Listatemp[i].Transportador.Codigo;

                        double? codigoTer = null;
                        if (Listatemp[0].CargaJanelaCarregamento != null && !string.IsNullOrEmpty(Listatemp[i].Terceiro?.CPF_CNPJ.ToString()))
                            codigoTer = Listatemp[i].Terceiro.CPF_CNPJ;

                        query.SetParameter("CJC_CODIGO_" + i.ToString(), codigoJanela);
                        query.SetParameter("JCT_HORARIO_LIBERACAO_" + i.ToString(), Listatemp[i].HorarioLiberacao);
                        query.SetParameter<int?>("EMP_CODIGO_" + i.ToString(), codigoTransp);
                        query.SetParameter("JCT_SITUACAO_" + i.ToString(), Listatemp[i].Situacao);
                        query.SetParameter("JCT_PENDENTE_CALCULAR_FRETE_" + i.ToString(), Listatemp[i].PendenteCalcularFrete);
                        query.SetParameter("JCT_TIPO_" + i.ToString(), Listatemp[i].Tipo);
                        query.SetParameter("JCT_BLOQUEADA_" + i.ToString(), Listatemp[i].Bloqueada);
                        query.SetParameter<double?>("CLI_CGCCPF_TERCEIRO_" + i.ToString(), codigoTer);
                        query.SetParameter("JCT_HORARIO_LIMITE_CONFIRMAR_CARGA_" + i.ToString(), Listatemp[i].HorarioLimiteConfirmarCarga);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }
        }

        /// <summary>
        /// Metodo expecifico para adicionar os históricos da geracao da carga. favor nao usar
        /// </summary>
        public void InsertSQLListaCargaJanelaCarregamentoTransportadoHistorico(int cargaJanelaCarregamento)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO(JCT_CODIGO, JTH_DATA, JTH_DESCRICAO, JTH_TIPO)
                       select JanelaTransportador.JCT_CODIGO, getdate(), 'Carga disponibilizada para o transportador', {(int)TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao}
                         from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JanelaTransportador
                        where JanelaTransportador.CJC_CODIGO = {cargaJanelaCarregamento}
                          and not exists (
                                  select 1
                                    from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO Historico
                                   where Historico.JCT_CODIGO = JanelaTransportador.JCT_CODIGO
                              )"
                )
                .ExecuteUpdate();
        }

        public void DeletarPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorComponenteFrete ComponenteFrete where ComponenteFrete.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorComponente Componente where Componente.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorTermoAceite TermoAceite where TermoAceite.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery("delete from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE_VEICULOS_VINCULADOS where JTD_CODIGO in (select DadosTransporte.JTD_CODIGO from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE DadosTransporte join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JanelaTransportador on JanelaTransportador.JCT_CODIGO = DadosTransporte.JCT_CODIGO where JanelaTransportador.CJC_CODIGO = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorDadosTransporte DadosTransporte where DadosTransporte.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorHistoricoOferta HistoricoOferta where HistoricoOferta.CargaJanelaCarregamentoTransportadorHistorico.Codigo in (select Historico.Codigo from CargaJanelaCarregamentoTransportadorHistorico Historico where Historico.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorHistorico Historico where Historico.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportador where Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorComponenteFrete ComponenteFrete where ComponenteFrete.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorComponente Componente where Componente.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorTermoAceite TermoAceite where TermoAceite.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("delete from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE_VEICULOS_VINCULADOS where JTD_CODIGO in (select DadosTransporte.JTD_CODIGO from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE DadosTransporte join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JanelaTransportador on JanelaTransportador.JCT_CODIGO = DadosTransporte.JCT_CODIGO where JanelaTransportador.CJC_CODIGO = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorDadosTransporte DadosTransporte where DadosTransporte.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorHistoricoOferta HistoricoOferta where HistoricoOferta.CargaJanelaCarregamentoTransportadorHistorico.Codigo in (select Historico.Codigo from CargaJanelaCarregamentoTransportadorHistorico Historico where Historico.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportadorHistorico Historico where Historico.CargaJanelaCarregamentoTransportador.Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("delete from CargaJanelaCarregamentoTransportador where Codigo in (select JanelaTransportador.Codigo from CargaJanelaCarregamentoTransportador JanelaTransportador where JanelaTransportador.CargaJanelaCarregamento.Codigo = :codigoCargaJanelaCarregamento)").SetInt32("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public IList<AcompanhamentoChecklist> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQuery query = ConsultaPorChecklist(filtroPesquisa, parametroConsulta, false);

            return query
                .SetResultTransformer(Transformers.AliasToBean<AcompanhamentoChecklist>())
                .List<AcompanhamentoChecklist>();
        }

        public Task<IList<AcompanhamentoChecklistDetalhes>> ConsultarDetalhesAsync(int codigoJanelaCarregamentoTransportador, List<int> codigoVeiculo)
        {
            IQuery query = ObterSqlDetalhesChecklist(codigoJanelaCarregamentoTransportador, codigoVeiculo, false);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(AcompanhamentoChecklistDetalhes)));

            return query.ListAsync<AcompanhamentoChecklistDetalhes>();
        }

        public Task<int> ContarPorChecklistAsync(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist filtrosPesquisa)
        {
            NHibernate.IQuery consulta = ConsultaPorChecklist(filtrosPesquisa, null, true);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public async Task<int> ContarDetalhesAsync(int codigoJanelaCarregamentoTransportador, List<int> codigoVeiculo)
        {
            IQuery query = ObterSqlDetalhesChecklist(codigoJanelaCarregamentoTransportador, codigoVeiculo, true);

            return await query.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public List<int> BuscarVeiculosPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga);

            var veiculosVinculado = query.SelectMany(o => o.CargaJanelaCarregamento.Carga.VeiculosVinculados.Select(v => v.Codigo)).ToList();
            veiculosVinculado.Add(query.Select(o => o.CargaJanelaCarregamento.Carga.Veiculo.Codigo).FirstOrDefault());

            return veiculosVinculado.Distinct().ToList();
        }

        public void MarcarChecklistComoVisualizado(int codigoCargaJanelaCarregamento)
        {
            using ISession session = this.SessionNHiBernate;
            session.CreateSQLQuery(@"
                UPDATE T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST
                SET CTC_CHECKLIST_VISUALIZADO = 1
                WHERE CJT_CODIGO = :codigoCargaJanelaCarregamento")
            .SetParameter("codigoCargaJanelaCarregamento", codigoCargaJanelaCarregamento)
            .ExecuteUpdate();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPendenciaMotorista = new Repositorio.Embarcador.Cargas.ConsultaCargasComInteresseTransportadorTerceiro().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPendenciaMotorista.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro.CargasComInteresseTransportadorTerceiro> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPendenciaMotorista = new Repositorio.Embarcador.Cargas.ConsultaCargasComInteresseTransportadorTerceiro().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPendenciaMotorista.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro.CargasComInteresseTransportadorTerceiro)));

            return consultaPendenciaMotorista.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro.CargasComInteresseTransportadorTerceiro>();
        }

        public int ContarConsultaRelatorioHistoricoJanelaCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPendenciaMotorista = new Repositorio.Embarcador.Logistica.ConsultaHistoricoJanelaCarregamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPendenciaMotorista.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.HistoricoJanelaCarregamento> ConsultarRelatorioHistoricoJanelaCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPendenciaMotorista = new Repositorio.Embarcador.Logistica.ConsultaHistoricoJanelaCarregamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPendenciaMotorista.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.HistoricoJanelaCarregamento)));

            return consultaPendenciaMotorista.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.HistoricoJanelaCarregamento>();
        }
        #endregion

        #region Métodos Públicos para Disponibilizar Carga Automaticamente por Menor Valor de Frete Calculado por Tabela

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarCargasAguardandoCalculoFrete(int limite)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.PendenteCalcularFrete == true && o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return consultaCargaJanelaCarregamentoTransportador.Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarPorValorFreteTabelaCalculado(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o =>
                    o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento &&
                    o.Bloqueada == true &&
                    o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada &&
                    o.PendenteCalcularFrete == false &&
                    o.ValorFreteTabela > 0m &&
                    o.PossuiFreteCalculado == true &&
                    o.FreteCalculadoComProblemas == false &&
                    (o.TabelaFreteCliente == null || o.TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete != TipoFreteTabelaFrete.Proprio)
                );

            return consultaCargaJanelaCarregamentoTransportador.ToList();
        }

        public bool PossuiCargasAguardandoCalculoFrete(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                .Where(o => o.PendenteCalcularFrete == true && o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && o.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);

            return consultaCargaJanelaCarregamentoTransportador.Count() > 0;
        }

        public int DisponibilizarTodasBloqueadasPorCalculoFrete(int codigoCargaJanelaCarregamento)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO(JCT_CODIGO, JTH_DATA, JTH_DESCRICAO, JTH_TIPO)
                       select JCT_CODIGO, getdate(), 'Carga disponibilizada para o transportador', {(int)TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao}
                         from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR
                        where JCT_CODIGO in (
                                  select JanelaTransportador.JCT_CODIGO
                                    from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JanelaTransportador
                                    left join T_TABELA_FRETE_CLIENTE TabelaFreteCliente on TabelaFreteCliente.TFC_CODIGO = JanelaTransportador.TFC_CODIGO
                                    left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                                   where JanelaTransportador.CJC_CODIGO = {codigoCargaJanelaCarregamento}
                                     and JanelaTransportador.JCT_BLOQUEADA = 1
                                     and isnull(TabelaFrete.TBF_TIPO_FRETE_TABELA_FRETE, {(int)TipoFreteTabelaFrete.NaoInformado}) <> {(int)TipoFreteTabelaFrete.Proprio}
                              )"
                )
                .ExecuteUpdate();

            return UnitOfWork.Sessao
                .CreateQuery(@"
                    update CargaJanelaCarregamentoTransportador
                       set Bloqueada = 0
                     where Codigo in (
                               select Codigo
                                 from CargaJanelaCarregamentoTransportador 
                                where CargaJanelaCarregamento.Codigo = :codigo
                                  and TabelaFreteCliente is null
                                  and Bloqueada = 1
                           )
                        or Codigo in (
                               select Codigo
                                 from CargaJanelaCarregamentoTransportador 
                                where CargaJanelaCarregamento.Codigo = :codigo
                                  and TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete <> :tipoFrete
                                  and Bloqueada = 1
                           )"
                )
                .SetInt32("codigo", codigoCargaJanelaCarregamento)
                .SetEnum("tipoFrete", TipoFreteTabelaFrete.Proprio)
                .ExecuteUpdate();
        }

        public IList<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> BuscarCargaJanelaCarregamentoTransportadorPorOrdenacaoDeRegraParaOfertarCarga(List<int> codigosCargaJanelaCarregamento, List<RegraOfertaCarga> regrasOfertaCarga)
        {

            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR CargaJanelaCarregamentoTransportador");
            sql.AppendLine($"WHERE CargaJanelaCarregamentoTransportador.JCT_CODIGO in ({string.Join(", ", codigosCargaJanelaCarregamento)})");

            System.Text.StringBuilder ordenacao = new System.Text.StringBuilder();

            foreach (RegraOfertaCarga regra in regrasOfertaCarga)
            {
                if (!sql.ToString().Contains("ORDER BY "))
                    sql.AppendLine("ORDER BY ");

                switch (regra)
                {
                    case RegraOfertaCarga.ValorFrete:
                        if (!string.IsNullOrWhiteSpace((ordenacao.ToString())))
                            ordenacao.AppendLine("AND ");

                        ordenacao.Append(@"CargaJanelaCarregamentoTransportador.JCT_VALOR_FRETE_TABELA ");

                        break;

                    case RegraOfertaCarga.Share:
                        if (!string.IsNullOrWhiteSpace(ordenacao.ToString()))
                            ordenacao.AppendLine("AND ");

                        ordenacao.Append("(SELECT TOP 1 RotaFreteTransportador.RFE_PERCENTUAL_CARGAS_DA_ROTA FROM T_ROTA_FRETE_TRANSPORTADOR RotaFreteTransportador");
                        ordenacao.AppendLine("LEFT JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaCarregamentoTransportador.CAR_CODIGO");
                        ordenacao.AppendLine("WHERE RotaFreteTransportador.ROF_CODIGO = Carga.ROF_CODIGO) ");

                        break;

                    case RegraOfertaCarga.NivelServico:
                        if (!string.IsNullOrWhiteSpace(ordenacao.ToString()))
                            ordenacao.AppendLine("AND ");

                        ordenacao.Append("(SELECT Transportador.EMP_PONTUACAO_FIXA FROM T_EMPRESA Transportador");
                        ordenacao.Append("WHERE Transportador.EMP_CODIGO = CargaJanelaCarregamentoTransportador.EMP_CODIGO) ");

                        break;
                    default:
                        break;
                }

                sql.Append("ASC");
            }

            var query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador)));

            return query.SetTimeout(6000).List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
        }

        #endregion
    }
}
