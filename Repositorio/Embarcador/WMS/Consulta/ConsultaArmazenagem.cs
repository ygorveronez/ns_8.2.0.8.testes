using Dominio.ObjetosDeValor.Embarcador.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaArmazenagem : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaArmazenagem>
    {
        #region Construtores

        public ConsultaArmazenagem() : base(tabela: ObterTabela()) { } 

        #endregion

        #region Métodos Privados

        private static string ObterTabela()
        {
            return
                @"(
                    SELECT 
                        CAST(NotaFiscal.NF_NUMERO AS VARCHAR(20)) NumeroNota,
                        CAST(NotaFiscal.NF_SERIE  AS VARCHAR(20)) Serie,
                        NotaFiscal.NF_DATA_EMISSAO DataSaida,
                        Destinatario.CLI_NOME Destinatario,
                        CAST(NotaFiscal.NF_VOLUMES AS VARCHAR(20)) QuantidadeItens,
                        NotaFiscal.NF_PESO PesoLiquido,
                        NotaFiscal.NF_PESO_LIQUIDO PesoBruto,
                        NotaFiscal.NF_VALOR ValorTotal,
                        4 StatusNF,
                        'Entrada' TipoNF
                    FROM T_CARGA Carga
                        JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                        JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNota on PedidoNota.CPE_CODIGO = CargaPedido.CPE_CODIGO
                        JOIN T_XML_NOTA_FISCAL NotaFiscal on NotaFiscal.NFX_CODIGO = PedidoNota.NFX_CODIGO
                        LEFT OUTER JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_DESTINATARIO
                    WHERE Carga.CAR_SITUACAO not in (13, 18)

                    UNION ALL

                    SELECT 
                        CAST(NotaSaida.NFI_NUMERO AS VARCHAR(20)) NumeroNota,
                        CAST(Serie.ESE_NUMERO AS VARCHAR(20)) Serie,
                        NotaSaida.NFI_DATA_SAIDA DataSaida,
                        Destinatario.CLI_NOME Destinatario,
                        CAST(NotaSaida.NFI_TRANSP_VOLUME AS VARCHAR(20)) QuantidadeItens,
                        NotaSaida.NFI_TRANSP_PESO_LIQUIDO PesoLiquido,
                        NotaSaida.NFI_TRANSP_PESO_BRUTO PesoBruto,
                        NotaSaida.NFI_VALOR_TOTAL_NOTA ValorTotal,
                        NotaSaida.NFI_STATUS StatusNF,
                        'Saída' TipoNF
                    FROM T_NOTA_FISCAL NotaSaida
                        JOIN T_CARGA_NFE CargaNFe on CargaNFe.NFI_CODIGO = NotaSaida.NFI_CODIGO
                        JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = NotaSaida.CLI_CGCCPF
                        JOIN T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = NotaSaida.ESE_CODIGO
                ) as Armazenagem ";
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaArmazenagem filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroNota":
                    if (!select.Contains(" NumeroNota, "))
                    {
                        select.Append("Armazenagem.NumeroNota NumeroNota, ");
                        groupBy.Append("Armazenagem.NumeroNota, ");
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("Armazenagem.Serie Serie,");
                        groupBy.Append("Armazenagem.Serie, ");
                    }
                    break;

                case "DataSaida":
                    if (!select.Contains(" DataSaida, "))
                    {
                        select.Append("Armazenagem.DataSaida DataSaida, ");
                        groupBy.Append("Armazenagem.DataSaida, ");
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Armazenagem.Destinatario Destinatario, ");
                        groupBy.Append("Armazenagem.Destinatario, ");
                    }
                    break;

                case "QuantidadeItens":
                    if (!select.Contains(" QuantidadeItens, "))
                    {
                        select.Append("Armazenagem.QuantidadeItens QuantidadeItens, ");
                        groupBy.Append("Armazenagem.QuantidadeItens, ");
                    }
                    break;

                case "PesoLiquido":
                    if (!select.Contains(" PesoLiquido, "))
                    {
                        select.Append("Armazenagem.PesoLiquido PesoLiquido, ");
                        groupBy.Append("Armazenagem.PesoLiquido, ");
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("Armazenagem.PesoBruto PesoBruto, ");
                        groupBy.Append("Armazenagem.PesoBruto, ");
                    }
                    break;

                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select.Append("Armazenagem.ValorTotal ValorTotal, ");
                        groupBy.Append("Armazenagem.ValorTotal, ");
                    }
                    break;

                case "StatusNF":
                    if (!select.Contains(" StatusNF, "))
                    {
                        select.Append("Armazenagem.StatusNF StatusNF, ");
                        groupBy.Append("Armazenagem.StatusNF, ");
                    }
                    break;

                case "TipoNF":
                    if (!select.Contains(" TipoNF, "))
                    {
                        select.Append("Armazenagem.TipoNF TipoNF, ");
                        groupBy.Append("Armazenagem.TipoNF, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaArmazenagem filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND Armazenagem.TipoNF is not null ");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" AND Armazenagem.DataSaida >= '{filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" AND Armazenagem.DataSaida <= '{filtrosPesquisa.DataFinal.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.StatusNF > 0)
            {
                if (filtrosPesquisa.StatusNF == Dominio.Enumeradores.StatusNFe.Cancelado)
                    where.Append($" AND Armazenagem.StatusNF in (2,3)");
                else
                    where.Append($" AND Armazenagem.StatusNF = {(int)filtrosPesquisa.StatusNF}");
            }

            if (filtrosPesquisa.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada)
                where.Append($" AND Armazenagem.TipoNF = 'Entrada'");
            else if (filtrosPesquisa.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida)
                where.Append($" AND Armazenagem.TipoNF = 'Saída'");
        }

        #endregion
    }
}
