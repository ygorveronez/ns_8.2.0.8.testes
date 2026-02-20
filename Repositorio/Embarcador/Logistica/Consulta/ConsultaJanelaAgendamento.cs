using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaJanelaAgendamento : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento>
    {
        #region Construtores

        public ConsultaJanelaAgendamento() : base(tabela: "T_PEDIDO as Pedido") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsAgendamentoColetaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" AgendamentoPedido "))
                joins.Append("JOIN T_AGENDAMENTO_COLETA_PEDIDO AgendamentoPedido ON Pedido.PED_CODIGO = AgendamentoPedido.PED_CODIGO ");
        }

        private void SetarJoinsAgendamentoColeta(StringBuilder joins)
        {
            SetarJoinsAgendamentoColetaPedido(joins);
            if (!joins.Contains(" Agendamento "))
                joins.Append("JOIN T_AGENDAMENTO_COLETA Agendamento on Agendamento.ACO_CODIGO = AgendamentoPedido.ACO_CODIGO ");
        }

        private void SetarJoinsJanelaDescarregamento(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" JanelaDescarregamento "))
                joins.Append("JOIN T_CARGA_JANELA_DESCARREGAMENTO JanelaDescarregamento on Agendamento.CAR_CODIGO = JanelaDescarregamento.CAR_CODIGO and isnull(JanelaDescarregamento.CJD_CANCELADA, 0) = 0 ");
        }

        private void SetarJoinsTipoDeCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoDeCarga "))
                joins.Append("LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Pedido.TCG_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsJanelaDescarregamento(joins);
            if (!joins.Contains(" Carga "))
                joins.Append("LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = JanelaDescarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Filial "))
                joins.Append("LEFT JOIN T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Agendamento.MVC_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" Remetente "))
                joins.Append("LEFT JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Agendamento.REM_CODIGO ");
        }

        private void SetarJoinsResponsavelConfirmacao(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" ResponsavelConfirmacao "))
                joins.Append("LEFT JOIN T_FUNCIONARIO ResponsavelConfirmacao ON ResponsavelConfirmacao.FUN_CODIGO = Agendamento.FUN_CODIGO ");
        }

        private void SetarJoinsOperadorCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" OperadorCarga "))
                joins.Append("LEFT JOIN T_FUNCIONARIO OperadorCarga ON Carga.CAR_OPERADOR = OperadorCarga.FUN_CODIGO ");
        }

        private void SetarJoinsOperadorSolicitante(StringBuilder joins)
        {
            if (!joins.Contains(" OperadorSolicitante "))
                joins.Append("LEFT JOIN T_FUNCIONARIO OperadorSolicitante ON Agendamento.FUN_SOLICITANTE = OperadorSolicitante.FUN_CODIGO ");
        }

        private void SetarJoinsProdutoEmbarcadorPrincipal(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoEmbarcadorPrincipal "))
                joins.Append("LEFT JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcadorPrincipal on ProdutoEmbarcadorPrincipal.PRO_CODIGO = Pedido.PRO_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            SetarJoinsProdutoEmbarcadorPrincipal(joins);

            if (!joins.Contains(" GrupoProduto "))
                joins.Append("LEFT JOIN T_GRUPO_PRODUTO GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcadorPrincipal.GRP_CODIGO ");
        }

        private void SetarJoinsCargaJanelaDescarregamentoPedido(StringBuilder joins)
        {
            SetarJoinsJanelaDescarregamento(joins);

            if (!joins.Contains(" CargaJanelaDescarregamentoPedido "))
                joins.Append("LEFT JOIN T_CARGA_JANELA_DESCARREGAMENTO_PEDIDO CargaJanelaDescarregamentoPedido on CargaJanelaDescarregamentoPedido.CJD_CODIGO = JanelaDescarregamento.CJD_CODIGO ");
        }

        private void SetarJoinsPedidoProduto(StringBuilder joins)
        {
            SetarJoinsAgendamentoColetaPedidoProduto(joins);

            if (!joins.Contains(" PedidoProduto "))
                joins.Append("LEFT JOIN T_PEDIDO_PRODUTO PedidoProduto on PedidoProduto.PRP_CODIGO = AgendamentoColetaPedidoProduto.PRP_CODIGO ");
        }

        private void SetarJoinsProdutoEmbarcador(StringBuilder joins)
        {
            SetarJoinsPedidoProduto(joins);

            if (!joins.Contains(" ProdutoEmbarcador "))
                joins.Append("LEFT JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = PedidoProduto.PRO_CODIGO ");
        }

        private void SetarJoinsAgendamentoColetaPedidoProduto(StringBuilder joins)
        {
            SetarJoinsAgendamentoColetaPedido(joins);

            if (!joins.Contains(" AgendamentoColetaPedidoProduto "))
                joins.Append("LEFT JOIN T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO AgendamentoColetaPedidoProduto on AgendamentoColetaPedidoProduto.ACP_CODIGO = AgendamentoPedido.ACP_CODIGO ");
        }

        private void SetarJoinsProdutosAgendamento(StringBuilder joins)
        {
            SetarJoinsAgendamentoColetaPedidoProduto(joins);
            SetarJoinsPedidoProduto(joins);
            SetarJoinsProdutoEmbarcador(joins);
        }

        private void SetarSelectQuantidadeCaixas(StringBuilder select, StringBuilder groupBy)
        {
            if (select.Contains(" QuantidadeCaixas, ") && !select.Contains("AgendamentoColetaPedidoProduto.APP_QUANTIDADE_DE_CAIXAS"))
            {
                select.Replace("AgendamentoPedido.ACP_VOLUMES_ENVIAR as QuantidadeCaixas, ", "AgendamentoColetaPedidoProduto.APP_QUANTIDADE_DE_CAIXAS as QuantidadeCaixas, ");
                groupBy.Append("AgendamentoColetaPedidoProduto.APP_QUANTIDADE_DE_CAIXAS, ");
            }
        }

        private string FormatarStringFiltros(string filtroInformado) // Formata filtros informados com vírgula para fazer um in
        {
            string[] listaItensFiltro = filtroInformado.Split(',');
            string filtroFormatado = null;
            if (listaItensFiltro.Length > 0)
            {
                string ultimoItem = listaItensFiltro.Last();
                foreach (var itemFiltro in listaItensFiltro)
                {
                    filtroFormatado += "'" + itemFiltro.Trim() + "'";
                    if (itemFiltro != ultimoItem)
                        filtroFormatado += ",";
                }
            }
            return filtroFormatado;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Pedido.PED_CODIGO as Codigo, ");
                        groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "DataDescarregamento":
                    if (!select.Contains(" DataDescarregamento, "))
                    {
                        select.Append("CONVERT(char(10), JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, 103) as DataDescarregamento, ");
                        groupBy.Append("JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");

                        SetarJoinsJanelaDescarregamento(joins);
                    }
                    break;

                case "HoraDescarregamento":
                    if (!select.Contains(" HoraDescarregamento, "))
                    {
                        select.Append("CONVERT(char(5), JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, 108) as HoraDescarregamento, ");
                        groupBy.Append("JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");

                        SetarJoinsJanelaDescarregamento(joins);
                    }
                    break;

                case "DataTentativa":
                    if (!select.Contains(" DataTentativa, "))
                    {
                        select.Append("CONVERT(char(10), Agendamento.ACO_DATA_AGENDAMENTO, 103) + ' ' + CONVERT(char(5), Agendamento.ACO_DATA_AGENDAMENTO, 108) as DataTentativa, ");
                        groupBy.Append("Agendamento.ACO_DATA_AGENDAMENTO, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "Senha":
                    if (!select.Contains(" Senha, "))
                    {
                        select.Append("Agendamento.ACO_SENHA as Senha, ");
                        groupBy.Append("Agendamento.ACO_SENHA, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        select.Append("Remetente.CLI_NOME as Fornecedor, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CnpjFornecedorFormatado":
                case "CnpjFornecedor":
                    if (!select.Contains(" CnpjFornecedor, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF as CnpjFornecedor, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");
                    }
                    break;

                case "Modalidade":
                    if (!select.Contains(" Modalidade, "))
                    {
                        select.Append("TipoDeCarga.TCG_DESCRICAO as Modalidade, ");
                        groupBy.Append("TipoDeCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoDeCarga(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "QuantidadeCaixas":
                    if (!select.Contains(" QuantidadeCaixas, "))
                    {
                        select.Append("AgendamentoPedido.ACP_VOLUMES_ENVIAR as QuantidadeCaixas, ");
                        groupBy.Append("AgendamentoPedido.ACP_VOLUMES_ENVIAR, ");

                        SetarJoinsAgendamentoColetaPedido(joins);
                    }
                    break;

                case "QuantidadeItens":
                    if (!select.Contains(" QuantidadeItens, "))
                    {
                        select.Append("AgendamentoPedido.ACP_SKU as QuantidadeItens, ");
                        groupBy.Append("AgendamentoPedido.ACP_SKU, ");

                        SetarJoinsAgendamentoColetaPedido(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO as ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "SituacaoAgendamento":
                case "DescricaoSituacaoAgendamento":
                    if (!select.Contains(" DescricaoSituacaoAgendamento, "))
                    {

                        select.Append("Carga.CAR_SITUACAO as SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        select.Append("Agendamento.ACO_SITUACAO as SituacaoAgendamento, ");
                        groupBy.Append("Agendamento.ACO_SITUACAO, ");

                        SetarJoinsCarga(joins);
                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "ResponsavelConfirmacao":
                    if (!select.Contains(" ResponsavelConfirmacao, "))
                    {
                        select.Append("ResponsavelConfirmacao.FUN_NOME as ResponsavelConfirmacao, ");
                        groupBy.Append("ResponsavelConfirmacao.FUN_NOME, ");

                        SetarJoinsResponsavelConfirmacao(joins);
                    }
                    break;

                case "AgendaExtra":
                case "AgendaExtraDescricao":
                    if (!select.Contains(" AgendaExtra, "))
                    {
                        select.Append("Carga.CAR_AGENDA_EXTRA as AgendaExtra, ");
                        groupBy.Append("Carga.CAR_AGENDA_EXTRA, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataSolicitacaoAgenda":
                case "DataSolicitacaoAgendaFormatada":
                    if (!select.Contains(" DataSolicitacaoAgenda, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO as DataSolicitacaoAgenda, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataConfirmacaoAgenda":
                case "DataConfirmacaoAgendaFormatada":
                    if (!select.Contains(" DataConfirmacaoAgenda, "))
                    {
                        select.Append("JanelaDescarregamento.CJD_DATA_CONFIRMACAO as DataConfirmacaoAgenda, ");
                        groupBy.Append("JanelaDescarregamento.CJD_DATA_CONFIRMACAO, ");

                        SetarJoinsJanelaDescarregamento(joins);
                    }
                    break;

                case "ValorTotalPedido":
                    if (!select.Contains(" ValorTotalPedido, "))
                    {
                        select.Append("CAST(ROUND(Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, 2, 1) AS DECIMAL(18,2)) as ValorTotalPedido, ");
                        groupBy.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, ");
                    }
                    break;

                case "QuantidadeCaixasPedido":
                    if (!select.Contains(" QuantidadeCaixasPedido, "))
                    {
                        select.Append("Pedido.PED_QUANTIDADE_VOLUMES as QuantidadeCaixasPedido, ");
                        groupBy.Append("Pedido.PED_QUANTIDADE_VOLUMES, ");
                    }
                    break;

                case "ValorMedioCaixa":
                    if (!select.Contains(" ValorMedioCaixa, "))
                    {
                        select.Append("CAST(ROUND(Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS/NULLIF(Pedido.PED_QUANTIDADE_VOLUMES, 0), 2, 1) AS DECIMAL(18,2)) as ValorMedioCaixa, ");
                        groupBy.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, Pedido.PED_QUANTIDADE_VOLUMES, ");
                    }
                    break;

                case "ValorAgendado":
                    if (!select.Contains(" ValorAgendado, "))
                    {
                        select.Append("CAST(ROUND(AgendamentoPedido.ACP_VOLUMES_ENVIAR * (Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS/NULLIF(Pedido.PED_QUANTIDADE_VOLUMES, 0)), 2, 1) AS DECIMAL(18,2)) as ValorAgendado, ");
                        groupBy.Append("Pedido.PED_QUANTIDADE_VOLUMES, AgendamentoPedido.ACP_VOLUMES_ENVIAR, Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, ");
                    }
                    break;

                case "OperadorAgendamento":
                    if (!select.Contains(" OperadorAgendamento, "))
                    {
                        select.Append("isnull(OperadorSolicitante.FUN_NOME, OperadorCarga.FUN_NOME) as OperadorAgendamento, ");
                        groupBy.Append("OperadorSolicitante.FUN_NOME, ");
                        groupBy.Append("OperadorCarga.FUN_NOME, ");

                        SetarJoinsOperadorSolicitante(joins);
                        SetarJoinsOperadorCarga(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Agendamento.ACO_OBSERVACAO as Observacao, ");
                        groupBy.Append("Agendamento.ACO_OBSERVACAO, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO as GrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;
                case "QuantidadeCaixasDevolvidas":
                case "QuantidadeCaixasNaoEntregues":
                    if (!select.Contains(" QuantidadeCaixasParcial,"))
                    {
                        select.AppendLine("CargaJanelaDescarregamentoPedido.JDP_QUANTIDADE as QuantidadeCaixasParcial, ");
                        select.AppendLine("CargaJanelaDescarregamentoPedido.JDP_TIPO_ACAO_PARCIAL as TipoAcaoParcial, ");
                        groupBy.Append("CargaJanelaDescarregamentoPedido.JDP_QUANTIDADE, CargaJanelaDescarregamentoPedido.JDP_TIPO_ACAO_PARCIAL, ");

                        SetarJoinsCargaJanelaDescarregamentoPedido(joins);
                    }
                    break;
                case "CodigoIntegracaoProduto":
                    if (!select.Contains(" CodigoIntegracaoProduto,"))
                    {

                        select.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR as CodigoIntegracaoProduto, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsProdutosAgendamento(joins);
                        SetarSelectQuantidadeCaixas(select, groupBy);
                    }
                    break;
                case "DescricaoProduto":
                    if (!select.Contains(" DescricaoProduto,"))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO as DescricaoProduto, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsProdutosAgendamento(joins);
                        SetarSelectQuantidadeCaixas(select, groupBy);
                    }
                    break;
                case "ValorProdutoAgendado":
                    if (!select.Contains(" ValorProdutoAgendado,"))
                    {
                        select.Append("(PedidoProduto.PRP_PRECO_UNITARIO * AgendamentoColetaPedidoProduto.APP_QUANTIDADE) as ValorProdutoAgendado, ");
                        groupBy.Append("PedidoProduto.PRP_PRECO_UNITARIO, ");

                        if (!groupBy.Contains(" AgendamentoColetaPedidoProduto.APP_QUANTIDADE, "))
                            groupBy.Append(" AgendamentoColetaPedidoProduto.APP_QUANTIDADE, ");

                        SetarJoinsProdutosAgendamento(joins);
                        SetarSelectQuantidadeCaixas(select, groupBy);
                    }
                    break;
                case "QtdProdutoAgendado":
                    if (!select.Contains(" QtdProdutoAgendado,"))
                    {
                        select.Append("AgendamentoColetaPedidoProduto.APP_QUANTIDADE as QtdProdutoAgendado, ");

                        if (!groupBy.Contains(" AgendamentoColetaPedidoProduto.APP_QUANTIDADE, "))
                            groupBy.Append(" AgendamentoColetaPedidoProduto.APP_QUANTIDADE, ");

                        SetarJoinsProdutosAgendamento(joins);
                        SetarSelectQuantidadeCaixas(select, groupBy);
                    }
                    break;
                case "SituacaoJanelaDescricao":
                    if (!select.Contains(" SituacaoJanela,"))
                    {
                        select.Append("JanelaDescarregamento.CJD_SITUACAO as SituacaoJanela, ");

                        if (!groupBy.Contains(" JanelaDescarregamento.CJD_SITUACAO , "))
                            groupBy.Append(" JanelaDescarregamento.CJD_SITUACAO , ");

                        SetarJoinsJanelaDescarregamento(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsJanelaDescarregamento(joins);
            SetarJoinsAgendamentoColeta(joins);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO >= '{filtrosPesquisa.DataInicial.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO <= '{filtrosPesquisa.DataFinal.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.TipoDeCarga > 0)
                where.Append($" and Pedido.TCG_CODIGO = {filtrosPesquisa.TipoDeCarga}");

            if (filtrosPesquisa.Fornecedor > 0)
                where.Append($" and Agendamento.REM_CODIGO = {filtrosPesquisa.Fornecedor}");

            if (filtrosPesquisa.JanelaExcedente == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
            {
                where.Append($" and JanelaDescarregamento.CJD_EXCEDENTE = 1");
            }
            else if (filtrosPesquisa.JanelaExcedente == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
            {
                where.Append($" and JanelaDescarregamento.CJD_EXCEDENTE = 0");
            }

            if (filtrosPesquisa.CentroDescarregamento > 0)
                where.Append($" and JanelaDescarregamento.CED_CODIGO = {filtrosPesquisa.CentroDescarregamento}");

            if (filtrosPesquisa.SituacaoAgendamento.Count > 0)
                where.Append($" and Agendamento.ACO_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoAgendamento.Select(o => (int)o))})");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.RaizCnpjFornecedor))
                where.Append($" and REPLACE(CAST(LEFT(CONVERT(VARCHAR(50), Agendamento.REM_CODIGO, 3), 9) AS VARCHAR), '.', '') = '{filtrosPesquisa.RaizCnpjFornecedor.Replace(".", "")}'");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                SetarJoinsFilial(joins);
                where.Append($" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                string filtroFormatado = FormatarStringFiltros(filtrosPesquisa.NumeroCarga);
                SetarJoinsCarga(joins);
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR in ({filtroFormatado})");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Senha))
            {
                string filtroFormatado = FormatarStringFiltros(filtrosPesquisa.Senha);
                SetarJoinsAgendamentoColeta(joins);
                where.Append($" and Agendamento.ACO_SENHA in ({filtroFormatado})");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
            {
                string filtroFormatado = FormatarStringFiltros(filtrosPesquisa.NumeroPedido);
                where.Append($" and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR in ({filtroFormatado})");
            }

            if (joins.Contains("AgendamentoColetaPedidoProduto"))
            {
                where.Append($" and PedidoProduto.PRP_CODIGO = AgendamentoColetaPedidoProduto.PRP_CODIGO");
            }

        }

        #endregion
    }
}