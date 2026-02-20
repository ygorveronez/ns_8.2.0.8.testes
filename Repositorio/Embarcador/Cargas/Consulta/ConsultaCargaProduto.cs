using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaProduto : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto>
    {
        #region Construtores

        public ConsultaCargaProduto() : base(tabela: "T_CARGA_PEDIDO_PRODUTO as CargaPedidoProduto", true) { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append("join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinDadosSumarizados(StringBuilder joins)
        {

            SetarJoinsCarga(joins);

            if (!joins.Contains(" DadosSumarizados "))
                joins.Append("join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append("join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            SetarJoinsProdutoEmbarcador(joins);

            if (!joins.Contains(" GrupoProduto "))
                joins.Append("inner join T_GRUPO_PRODUTO AS GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsJanelaCarregamento(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" JanelaCarregamento "))
                joins.Append("join T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento on JanelaCarregamento.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append("join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsProdutoEmbarcador(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoEmbarcador "))
                joins.Append("inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Destinatario "))
                joins.Append("join T_CLIENTE as Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append("join T_LOCALIDADES as LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        public void SetarJoinsRecebedorCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" RecebedorCargaPedido "))
                joins.Append(" LEFT JOIN T_CLIENTE RecebedorCargaPedido on RecebedorCargaPedido.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ");
        }

        public void SetarJoinsLocalidadeRecebedorCargaPedido(StringBuilder joins)
        {
            SetarJoinsRecebedorCargaPedido(joins);

            if (!joins.Contains(" LocalidadeRecebedorCargaPedido "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES LocalidadeRecebedorCargaPedido on LocalidadeRecebedorCargaPedido.LOC_CODIGO = RecebedorCargaPedido.LOC_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Pedido.FIL_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                        groupBy.Append("Carga.CAR_CODIGO, Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "UnidadeMedida":
                    if (!select.Contains(" UnidadeMedida"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_SIGLA_UNIDADE as UnidadeMedida, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO, ProdutoEmbarcador.PRO_SIGLA_UNIDADE, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "CodigoProdutoEmbarcador":
                    if (!select.Contains(" CodigoProdutoEmbarcador"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR as CodigoProdutoEmbarcador, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO, ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo"))
                    {
                        select.Append("CargaPedidoProduto.CPP_CODIGO as Codigo, ");
                        groupBy.Append("CargaPedidoProduto.CPP_CODIGO, ");
                    }
                    break;

                case "CnpjTransportador":
                    if (!select.Contains(" CnpjTransportador"))
                    {
                        select.Append("Empresa.EMP_CNPJ as CnpjTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "DescricaoProduto":
                    if (!select.Contains(" DescricaoProduto"))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO as DescricaoProduto, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador"))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "PesoUnitario":
                    if (!select.Contains(" PesoUnitario"))
                    {
                        select.Append("CargaPedidoProduto.CPP_PESO_UNITARIO as PesoUnitario, ");
                        groupBy.Append("CargaPedidoProduto.CPP_PESO_UNITARIO, ");
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade"))
                    {
                        select.Append("SUM(CargaPedidoProduto.CPP_QUANTIDADE) as Quantidade, ");
                    }
                    break;

                case "RazaoSocialTransportador":
                    if (!select.Contains(" RazaoSocialTransportador"))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocialTransportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF as CPFCNPJDestinatario, Destinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME as Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "LocalidadeDestinatario":
                    if (!select.Contains(" LocalidadeDestinatario, "))
                    {
                        select.Append("(LocalidadeDestinatario.LOC_DESCRICAO + ' - ' + LocalidadeDestinatario.UF_SIGLA) as LocalidadeDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "SituacaoJanelaCarregamento":
                case "DescricaoSituacaoJanelaCarregamento":
                    if (!select.Contains(" SituacaoJanelaCarregamento, "))
                    {
                        select.Append("JanelaCarregamento.CJC_SITUACAO as SituacaoJanelaCarregamento, ");
                        groupBy.Append("JanelaCarregamento.CJC_SITUACAO, ");

                        if (!select.Contains(" SituacaoCarga, "))
                        {
                            select.Append("Carga.CAR_SITUACAO as SituacaoCarga, ");
                            groupBy.Append("Carga.CAR_SITUACAO, ");

                            SetarJoinsCarga(joins);
                        }

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "DataCarregamentoCarga":
                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamentoCarga"))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO as DataCarregamentoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TotalPallets":
                    if (!select.Contains(" TotalPallets"))
                    {
                        select.Append("(Pedido.PED_NUMERO_PALETES + Pedido.PED_NUMERO_PALETES_FRACIONADO) as TotalPallets, ");
                        groupBy.Append("Pedido.PED_NUMERO_PALETES, Pedido.PED_NUMERO_PALETES_FRACIONADO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CargaProdutoDescricao":
                    if (!select.Contains(" CargaProdutoDescricao"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR + ' / ' + ProdutoEmbarcador.GRP_DESCRICAO as CargaProdutoDescricao, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsCarga(joins);
                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "TipoFrete":
                    if (!select.Contains(" TipoFrete"))
                    {
                        select.Append(@"(SELECT CASE WHEN Pedido.PED_TIPO_TOMADOR = 0 THEN 'CIF' WHEN Pedido.PED_TIPO_TOMADOR = 3 THEN 'FOB' ELSE '' END) as TipoFrete, ");
                        groupBy.Append("Pedido.PED_TIPO_TOMADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "SequenciaRoteirizacao":
                    if (!select.Contains(" SequenciaRoteirizacao, "))
                    {
                        select.Append(" CargaPedido.PED_ORDEM_ENTREGA SequenciaRoteirizacao, ");
                        groupBy.Append("CargaPedido.PED_ORDEM_ENTREGA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "Bloco":
                    if (!select.Contains("Bloco"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_BLOCO FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as Bloco, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "OrdemCarregamento":
                    if (!select.Contains("OrdemCarregamento"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_ORDEM_CARREGAMENTO FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as OrdemCarregamento, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "OrdemEntrega":
                    if (!select.Contains("OrdemEntrega"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_ORDEM_ENTREGA FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as OrdemEntrega, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DescricaoGrupoProduto":
                    if (!select.Contains("DescricaoGrupoProduto"))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO as DescricaoGrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;

                case "CubagemPedido":
                    if (!select.Contains("CubagemPedido"))
                    {
                        select.Append("Pedido.PED_CUBAGEM_TOTAL as CubagemPedido, ");
                        groupBy.Append("Pedido.PED_CUBAGEM_TOTAL, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                case "CPFCNPJRecebedorFormatado":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_CGCCPF as CPFCNPJRecebedor, RecebedorCargaPedido.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_CGCCPF, RecebedorCargaPedido.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "RecebedorDescricao":
                    if (!select.Contains(" RecebedorDescricao, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_NOME RecebedorDescricao, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_NOME, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "LocalidadeRecebedor":
                    if (!select.Contains(" LocalidadeRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedorCargaPedido.LOC_DESCRICAO LocalidadeRecebedor, ");
                        groupBy.Append("LocalidadeRecebedorCargaPedido.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeRecebedorCargaPedido(joins);
                    }
                    break;

                case "DataCriacao":
                case "DataCriacaoFormatada":
                    if (!select.Contains(" DataCriacaoCarga,"))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataCriacao, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente,"))
                    {
                        select.Append("Remetente.CLI_NOME Remetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CPFCNPJRemetente":
                case "CPFCNPJRemetenteFormatado":
                    if (!select.Contains(" Remetente,"))
                    {
                        select.Append("Remetente.CLI_CGCCPF CPFCNPJRemetente, Remetente.CLI_FISJUR TipoRemetente, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, Remetente.CLI_FISJUR, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "ValorMercadoria":
                    if (!select.Contains(" ValorMercadoria,"))
                    {
                        select.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoria, ");
                        groupBy.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Saldo":
                    if (!select.Contains(" Saldo, "))
                    {
                        select.Append("Pedido.PED_SALDO_VOLUMES_RESTANTE Saldo, ");
                        groupBy.Append("Pedido.PED_SALDO_VOLUMES_RESTANTE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataInicioJanela":
                case "DataInicioJanelaFormatada":
                    if (!select.Contains(" DataInicioJanela, "))
                    {
                        select.Append("Pedido.PED_DATA_INICIO_JANELA_DESCARGA DataInicioJanela, ");
                        groupBy.Append("Pedido.PED_DATA_INICIO_JANELA_DESCARGA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataFimJanela":
                case "DataFimJanelaFormatada":
                    if (!select.Contains(" DataFimJanela, "))
                    {
                        select.Append("Pedido.PED_DATA_VALIDADE DataFimJanela, ");
                        groupBy.Append("Pedido.PED_DATA_VALIDADE, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CodigoGrupoProduto":
                    if (!select.Contains("CodigoGrupoProduto"))
                    {
                        select.Append("GrupoProduto.GRP_CODIGO_GRUPO_PRODUTO_EMBARCADOR as CodigoGrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_CODIGO_GRUPO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;

                case "PedidoComAgenda":
                    if (!select.Contains(" PedidoComAgenda, "))
                    {
                        select.Append("(CASE WHEN EXISTS (SELECT _AgendamendoPedido.ACP_CODIGO FROM T_AGENDAMENTO_COLETA_PEDIDO _AgendamendoPedido WHERE _AgendamendoPedido.PED_CODIGO = Pedido.PED_CODIGO) THEN 'Sim' ELSE 'Não' END) PedidoComAgenda, ");
                        groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "PlacaVeiculos":
                    if (!select.Contains(" PlacaVeiculos, "))
                    {
                        select.Append("DadosSumarizados.CDS_VEICULOS PlacaVeiculos, ");
                        groupBy.Append("DadosSumarizados.CDS_VEICULOS, ");

                        SetarJoinDadosSumarizados(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING((SELECT ', ' + '(' + FORMAT(cast(motorista1.FUN_CPF as numeric), '000\\.000\\.000\\-00') + ') - ' + motorista1.FUN_NOME FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motoristas, ");
                        groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            where.Append($" and (Carga.CAR_SITUACAO <> 13 AND Carga.CAR_SITUACAO <> 18) ");
            SetarJoinsCarga(joins);

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
            {
                where.Append($" and JanelaCarregamento.CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento}");

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.CodigoPedido > 0)
            {
                where.Append($" and Pedido.PED_CODIGO = {filtrosPesquisa.CodigoPedido}");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                where.Append($" and ProdutoEmbarcador.PRO_CODIGO = {filtrosPesquisa.CodigoProduto}");

                SetarJoinsProdutoEmbarcador(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                SetarJoinsEmpresa(joins);
            }

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                where.Append($" and Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjDestinatario}");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataInicial.HasValue)
            {
                where.Append($" and JanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd HH:mm:ss")}'");

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.DataLimite.HasValue)
            {
                where.Append($" and JanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

                SetarJoinsJanelaCarregamento(joins);
            }

            string situacoesCargaNaoFaturadas = " in (" + string.Join(", ", SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada().Cast<int>()) + ") ";
            string condicional = " and ";
            string situacoesJanela = $" JanelaCarregamento.CJC_SITUACAO in ({string.Join(", ", filtrosPesquisa.Situacao.Select(x => (int)x))})";

            if (filtrosPesquisa.SituacaoFaturada ^ filtrosPesquisa.SituacaoNaoFaturada)
            {
                condicional = " or ";

                where.Append($" and (Carga.CAR_SITUACAO {(filtrosPesquisa.SituacaoFaturada ? "not" : "")} {situacoesCargaNaoFaturadas}");

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.Situacao?.Count > 0)
            {
                where.Append(condicional + situacoesJanela + (condicional == " or " ? ")" : ""));

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.CodigosFilial.Exists(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)})");
        }

        #endregion
    }
}
