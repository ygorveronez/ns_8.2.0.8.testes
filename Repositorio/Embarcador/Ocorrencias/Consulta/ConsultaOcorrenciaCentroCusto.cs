using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Ocorrencias
{
    sealed class ConsultaOcorrenciaCentroCusto : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto>
    {
        #region Construtores

        public ConsultaOcorrenciaCentroCusto() : base(tabela: ObterTabela()) { }

        #endregion

        #region Métodos Privados

        private static string ObterTabela()
        {
            return
                @"( 
                    select CargaOcorrencia.COC_NUMERO_CONTRATO as NumeroOcorrencia,
                           CargaOcorrencia.COC_DATA_OCORRENCIA as DataOcorrencia,
                           Carga.CAR_CODIGO as CodigoCarga,
                           Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga,
                           Transportador.EMP_CODIGO CodigoTransportador,
                           Transportador.EMP_RAZAO RazaoSocialTransportador,
                           Transportador.EMP_CNPJ CnpjTransportador,
                           Cte.CON_NUM as NumeroCte,
                           Cte.CON_VALOR_RECEBER as ValorReceberCte
                      from T_CARGA_OCORRENCIA CargaOcorrencia
                      join T_CARGA_OCORRENCIA_DOCUMENTO Documento on Documento.COC_CODIGO = CargaOcorrencia.COC_CODIGO
                      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = Documento.CCT_CODIGO
                      join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
                      join T_CARGA Carga on Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                      left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO
                ) as Ocorrencia";
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Ocorrencia.CodigoCarga ");
        }

        private void SetarJoinsCentroCusto(StringBuilder joins)
        {
            SetarJoinsPedidoCentroCusto(joins);

            if (!joins.Contains(" CentroCusto "))
                joins.Append(" join T_CLIENTE CentroCusto on CentroCusto.CLI_CGCCPF = PedidoCentroCusto.CLI_CGCCPF ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsPedidoCentroCusto(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" PedidoCentroCusto "))
                joins.Append(" join T_PEDIDO_CLIENTE PedidoCentroCusto on PedidoCentroCusto.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CnpjTransportador":
                case "CnpjTransportadorFormatado":
                    if (!select.Contains(" CnpjTransportador,"))
                    {
                        select.Append("Ocorrencia.CnpjTransportador as CnpjTransportador, ");
                        groupBy.Append("Ocorrencia.CnpjTransportador, ");
                    }
                    break;

                case "CodigoIntegracaoCentroCusto":
                    if (!select.Contains(" CodigoIntegracaoCentroCusto,"))
                    {
                        select.Append("CentroCusto.CLI_CODIGO_INTEGRACAO as CodigoIntegracaoCentroCusto, ");
                        groupBy.Append("CentroCusto.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsCentroCusto(joins);
                    }
                    break;

                case "CpfCnpjCentroCusto":
                case "CpfCnpjCentroCustoFormatado":
                    if (!select.Contains(" CpfCnpjCentroCusto,"))
                    {
                        select.Append("CentroCusto.CLI_CGCCPF as CpfCnpjCentroCusto, CentroCusto.CLI_FISJUR as TipoCentroCusto, ");
                        groupBy.Append("CentroCusto.CLI_CGCCPF, CentroCusto.CLI_FISJUR, ");

                        SetarJoinsCentroCusto(joins);
                    }
                    break;

                case "DescricaoCentroCusto":
                    if (!select.Contains(" DescricaoCentroCusto,"))
                    {
                        select.Append("CentroCusto.CLI_NOME as DescricaoCentroCusto, ");
                        groupBy.Append("CentroCusto.CLI_NOME, ");

                        SetarJoinsCentroCusto(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append("Ocorrencia.NumeroCarga as NumeroCarga, ");
                        groupBy.Append("Ocorrencia.NumeroCarga, ");
                    }
                    break;

                case "NumeroCte":
                    if (!select.Contains(" NumeroCte,"))
                    {
                        select.Append("Ocorrencia.NumeroCte as NumeroCte, ");
                        groupBy.Append("Ocorrencia.NumeroCte, ");
                    }
                    break;

                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia,"))
                    {
                        select.Append("Ocorrencia.NumeroOcorrencia as NumeroOcorrencia, ");
                        groupBy.Append("Ocorrencia.NumeroOcorrencia, ");
                    }
                    break;

                case "RazaoSocialTransportador":
                    if (!select.Contains(" RazaoSocialTransportador,"))
                    {
                        select.Append("Ocorrencia.RazaoSocialTransportador as RazaoSocialTransportador, ");
                        groupBy.Append("Ocorrencia.RazaoSocialTransportador, ");
                    }
                    break;

                case "ValorReceberCte":
                    if (!select.Contains(" ValorReceberCte,"))
                    {
                        select.Append("Ocorrencia.ValorReceberCte as ValorReceberCte, ");
                        groupBy.Append("Ocorrencia.ValorReceberCte, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Ocorrencia.CodigoTransportador = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.DataOcorrenciaInicial.HasValue)
                where.Append($" and Ocorrencia.DataOcorrencia >= '{filtrosPesquisa.DataOcorrenciaInicial.Value.ToString("MM/dd/yyyy")}'");

            if (filtrosPesquisa.DataOcorrenciaLimite.HasValue)
                where.Append($" and Ocorrencia.DataOcorrencia <= '{filtrosPesquisa.DataOcorrenciaLimite.Value.AddDays(1).ToString("MM/dd/yyyy")}'");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and Ocorrencia.CodigoCarga = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.NumeroOcorrencia > 0)
                where.Append($" and Ocorrencia.NumeroOcorrencia = {filtrosPesquisa.NumeroOcorrencia}");
        }

        #endregion
    }
}
