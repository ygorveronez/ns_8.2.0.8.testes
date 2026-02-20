using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaPedidoRetornoOcorrencia : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia>
    {
        #region Construtores

        public ConsultaPedidoRetornoOcorrencia() : base(tabela: "T_PEDIDO AS Pedido") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append("left join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Localidade "))
                joins.Append(" LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO ");
        }

        private void SetarJoinsTipoDeCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoDeCarga "))
                joins.Append(" Left Join T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Pedido.TCG_CODIGO ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append(@"(Select Top(1)TipoDeOcorrencia.OCO_DESCRICAO
                                         FROM T_OCORRENCIA TipoDeOcorrencia
                                            LEFT JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA PedidoOcorrencia on PedidoOcorrencia.OCO_CODIGO = TipoDeOcorrencia.OCO_CODIGO
                                         Where PedidoOcorrencia.PED_CODIGO = Pedido.PED_CODIGO order by PedidoOcorrencia.POC_CODIGO desc) TipoOcorrencia, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "GrupoOcorrencia":
                    if (!select.Contains(" GrupoOcorrencia, "))
                    {
                        select.Append(@"(Select Top(1)GrupoOcorrencia.GTO_DESCRICAO
		                                FROM T_GRUPO_TIPO_OCORRENCIA GrupoOcorrencia 
			                                LEFT JOIN T_OCORRENCIA TipoDeOcorrencia ON TipoDeOcorrencia.GTO_CODIGO = GrupoOcorrencia.GTO_CODIGO
			                                LEFT JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA PedidoOcorrencia on PedidoOcorrencia.OCO_CODIGO = TipoDeOcorrencia.OCO_CODIGO
		                                Where PedidoOcorrencia.PED_CODIGO = Pedido.PED_CODIGO order by PedidoOcorrencia.POC_CODIGO desc) GrupoOcorrencia, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "DataOcorrenciaFormatada":
                    if (!select.Contains(" DataOcorrencia, "))
                    {
                        select.Append(@"(Select Top(1)PedidoOcorrencia.POC_DATA_OCORRENCIA
                                         FROM T_PEDIDO_OCORRENCIA_COLETA_ENTREGA PedidoOcorrencia
                                         Where PedidoOcorrencia.PED_CODIGO = Pedido.PED_CODIGO order by PedidoOcorrencia.POC_CODIGO desc) DataOcorrencia, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select.Append(@"SUBSTRING((
                               SELECT ', ' + 
                                  CAST(NotaFiscal.NF_NUMERO AS VARCHAR(20))
                               FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal
                                  INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO 
                               WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO  
                               FOR XML PATH('')),
                            3, 1000) NotaFiscal, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO"))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorNF":
                    if (!select.Contains(" ValorNF, "))
                    {
                        select.Append(@"( SELECT SUM(NotaFiscal.NF_VALOR)
		                        FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal 
			                        INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
                                WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO) ValorNF, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO"))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "Filial":
                    SetarSelect("RazaoFilial", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("CNPJFilial", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "RazaoFilial":
                    if (!select.Contains(" RazaoFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO RazaoFilial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Transportador":
                    SetarSelect("RazaoTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("CNPJTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "RazaoTransportador":
                    if (!select.Contains(" RazaoTransportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO RazaoTransportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CidadeDestino":
                    if (!select.Contains(" CidadeDestino, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO CidadeDestino, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("Localidade.UF_SIGLA UFDestino, ");
                        groupBy.Append("Localidade.UF_SIGLA, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoDeCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoDeCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoDeCarga(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
            {
                List<string> splitNumeroPedido = filtrosPesquisa.NumeroPedido.Split(',').Select(o => o.Trim()).ToList();
                where.Append($" AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR in ('{string.Join("','", splitNumeroPedido)}') ");
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" AND Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" AND Pedido.PED_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");


        }

        #endregion
    }
}
