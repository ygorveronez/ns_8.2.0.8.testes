using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica
{
    sealed class ConsultaControleTempoViagem : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem>
    {
        #region Construtores

        public ConsultaControleTempoViagem() : base(tabela: "T_PEDIDO_XML_NOTA_FISCAL as PedidoXmlNotaFiscal") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsXmlNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" XmlNotaFiscal "))
                joins.Append(" INNER JOIN T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" INNER JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsFluxoGestaoPatio(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" FluxoGestaoPatio "))
                joins.Append($" INNER JOIN T_FLUXO_GESTAO_PATIO FluxoGestaoPatio ON FluxoGestaoPatio.CAR_CODIGO = Carga.CAR_CODIGO and FluxoGestaoPatio.FGE_TIPO = {(int)TipoFluxoGestaoPatio.Origem} and FluxoGestaoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO <> {(int)SituacaoEtapaFluxoGestaoPatio.Cancelado} ");
        }

        private void SetarJoinsCanhoto(StringBuilder joins)
        {
            SetarJoinsXmlNotaFiscal(joins);

            if (!joins.Contains(" CanhotoNotaFiscal "))
                joins.Append(" INNER JOIN T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal ON CanhotoNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsClienteDestinatario(StringBuilder joins)
        {
            SetarJoinsXmlNotaFiscal(joins);

            if (!joins.Contains(" ClienteDestinatario "))
                joins.Append(" INNER JOIN T_CLIENTE ClienteDestinatario ON ClienteDestinatario.CLI_CGCCPF = XmlNotaFiscal.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsClienteDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" INNER JOIN T_LOCALIDADES LocalidadeDestinatario ON LocalidadeDestinatario.LOC_CODIGO = ClienteDestinatario.LOC_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" INNER JOIN T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados ON CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" INNER JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtroPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroNF":
                    if (!select.Contains(" NumeroNF, "))
                    {
                        select.Append("XmlNotaFiscal.NF_NUMERO as NumeroNF, ");
                        groupBy.Append("XmlNotaFiscal.NF_NUMERO, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }

                    break;

                case "DataFaturaFormatada":
                    if (!select.Contains(" DataFatura, "))
                    {
                        select.Append("XmlNotaFiscal.NF_DATA_EMISSAO as DataFatura, ");
                        groupBy.Append("XmlNotaFiscal.NF_DATA_EMISSAO, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }

                    break;

                case "PrevisaoEntregaFormatada":
                    if (!select.Contains(" PrevisaoEntrega, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA AS PrevisaoEntrega, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins);
                    }

                    break;

                case "DataEntregaRealFormatada":
                    if (!select.Contains(" DataEntregaReal, "))
                    {
                        select.Append("CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE as DataEntregaReal, ");
                        groupBy.Append("CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE, ");

                        SetarJoinsCanhoto(joins);
                    }

                    break;

                case "Performance":
                    if (!select.Contains(" Performance, "))
                    {
                        select.Append("DATEDIFF(day, Pedido.PED_PREVISAO_ENTREGA, CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE) Performance, ");

                        SetarJoinsPedido(joins);
                        SetarJoinsCanhoto(joins);
                    }

                    break;

                case "RetornoComprovanteFormatada":
                    if (!select.Contains(" RetornoComprovante, "))
                    {
                        select.Append("CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO as RetornoComprovante, ");
                        groupBy.Append("CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO, ");
                     
                        SetarJoinsCanhoto(joins);
                    }

                    break;

                case "DiasRetornoComprovante":
                    if (!select.Contains(" DiasRetornoComprovante, "))
                    {
                        select.Append("DATEDIFF(day, CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO, CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE) DiasRetornoComprovante, ");

                        SetarJoinsCanhoto(joins);
                    }

                    break;

                case "DocumentoVenda":
                    if (!select.Contains(" DocumentoVenda, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_NUMERO_ORDEM as DocumentoVenda, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_NUMERO_ORDEM, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }

                    break;

                case "RazaoSocialDestinatario":
                    if (!select.Contains(" RazaoSocialDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_NOME as RazaoSocialDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_NOME, ");

                        SetarJoinsClienteDestinatario(joins);
                    }

                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }

                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("LocalidadeDestinatario.LOC_DESCRICAO + ' - ' + LocalidadeDestinatario.UF_SIGLA as Destino, ");
                        groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }

                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }

                    break;

                case "ValorNota":
                    if (!select.Contains(" ValorNota, "))
                    {
                        select.Append("XmlNotaFiscal.NF_VALOR as ValorNota, ");
                        groupBy.Append("XmlNotaFiscal.NF_VALOR, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }

                    break;

                case "TempoViagem":
                    if (!select.Contains(" DataEntregaCanhoto, "))
                    {
                        select.Append("CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE DataEntregaCanhoto, ");
                        groupBy.Append("CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE, ");
                        
                        SetarJoinsCanhoto(joins);
                    }

                    if (!select.Contains(" DataUltimaEtapa, "))
                    {
                        select.Append("FluxoGestaoPatio.FGP_TRAVA_CHAVE DataUltimaEtapa, ");
                        groupBy.Append("FluxoGestaoPatio.FGP_TRAVA_CHAVE, ");

                        SetarJoinsFluxoGestaoPatio(joins);
                    }
                    break;

                default:
                    break;
                    
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigosCargas?.Count > 0)
            {
                where.Append($" AND Carga.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCargas)})");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.NumerosNota?.Count > 0)
            {
                where.Append($" AND XmlNotaFiscal.NFX_CODIGO in ({string.Join(", ", filtrosPesquisa.NumerosNota)})");
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.Destinos?.Count > 0)
            {
                where.Append($" AND LocalidadeDestinatario.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.Destinos)})");
                SetarJoinsLocalidadeDestinatario(joins);
            }

            if (filtrosPesquisa.Transportadores?.Count > 0)
            {
                where.Append($" AND Transportador.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.Transportadores)})");
                SetarJoinsTransportador(joins);
            }

            if (filtrosPesquisa.DataFaturaInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(XmlNotaFiscal.NF_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataFaturaInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.DataFaturaFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(XmlNotaFiscal.NF_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFaturaFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.PrevisaoEntregaInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(Pedido.PED_PREVISAO_ENTREGA AS DATE) >= '{filtrosPesquisa.PrevisaoEntregaInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(Pedido.PED_PREVISAO_ENTREGA AS DATE) <= '{filtrosPesquisa.PrevisaoEntregaFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataEntregaRealInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE AS DATE) >= '{filtrosPesquisa.DataEntregaRealInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsCanhoto(joins);
            }

            if (filtrosPesquisa.DataEntregaRealFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE AS DATE) <= '{filtrosPesquisa.DataEntregaRealFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsCanhoto(joins);
            }

            if (filtrosPesquisa.DataRetornoComprovanteInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO AS DATE) >= '{filtrosPesquisa.DataRetornoComprovanteInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsCanhoto(joins);
            }

            if (filtrosPesquisa.DataRetornoComprovanteFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO AS DATE) <= '{filtrosPesquisa.DataRetornoComprovanteFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsCanhoto(joins);
            }

            if (filtrosPesquisa.ValorNotaInicial > 0)
            {
                where.Append($" AND XmlNotaFiscal.NF_VALOR >= " + filtrosPesquisa.ValorNotaInicial.ToString().Replace(",", "."));
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.ValorNotaFinal > 0)
            {
                where.Append($" AND XmlNotaFiscal.NF_VALOR <= " + filtrosPesquisa.ValorNotaFinal.ToString().Replace(",", "."));
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.RazaoSocialDestinatario))
            {
                where.Append($" AND ClienteDestinatario.CLI_NOME LIKE '%" + filtrosPesquisa.RazaoSocialDestinatario + "%'");
                SetarJoinsClienteDestinatario(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DocumentoVenda))
            {
                where.Append($" AND CargaDadosSumarizados.CDS_NUMERO_ORDEM LIKE '%" + filtrosPesquisa.DocumentoVenda + "%'");
                SetarJoinsCargaDadosSumarizados(joins);
            }

            if (filtrosPesquisa.DiasRetornoComprovante > 0)
            {
                where.Append($" AND DATEDIFF(day, CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO, CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE) = " + filtrosPesquisa.DiasRetornoComprovante.ToString());
                SetarJoinsCanhoto(joins);
            }

            if (filtrosPesquisa.Performance > 0)
            {
                where.Append($" AND DATEDIFF(day, Pedido.PED_PREVISAO_ENTREGA, CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE) = " + filtrosPesquisa.Performance.ToString());
                SetarJoinsPedido(joins);
                SetarJoinsCanhoto(joins);
            }

        }

        #endregion
    }
}
