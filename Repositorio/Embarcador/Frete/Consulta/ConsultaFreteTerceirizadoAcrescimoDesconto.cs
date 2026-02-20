using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaFreteTerceirizadoAcrescimoDesconto : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto>
    {
        #region Construtores

        public ConsultaFreteTerceirizadoAcrescimoDesconto() : base(tabela: "T_CONTRATO_FRETE_TERCEIRO as Contrato ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsContratoValor(StringBuilder joins)
        {
            if (!joins.Contains(" ContratoValor "))
                joins.Append(" join T_CONTRATO_FRETE_TERCEIRO_VALOR ContratoValor on ContratoValor.CFT_CODIGO = Contrato.CFT_CODIGO ");
        }

        private void SetarJoinsJustificativa(StringBuilder joins)
        {
            SetarJoinsContratoValor(joins);

            if (!joins.Contains(" Justificativa "))
                joins.Append(" left join T_JUSTIFICATIVA Justificativa on Justificativa.JUS_CODIGO = ContratoValor.JUS_CODIGO ");
        }

        private void SetarJoinsTaxaTerceiro(StringBuilder joins)
        {
            SetarJoinsContratoValor(joins);

            if (!joins.Contains(" TaxaTerceiro "))
                joins.Append(" left join T_TAXA_TERCEIRO TaxaTerceiro on TaxaTerceiro.TAT_CODIGO = ContratoValor.TAT_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ");
        }

        private void SetarJoinsCargaCIOT(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCIOT "))
                joins.Append(" left join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ");
        }

        private void SetarJoinsCIOT(StringBuilder joins)
        {
            SetarJoinsCargaCIOT(joins);

            if (!joins.Contains(" CIOT "))
                joins.Append(" left join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ");
        }

        private void SetarJoinsTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" Terceiro "))
                joins.Append(" left join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "ContratoFrete":
                    if (!select.Contains(" ContratoFrete, "))
                    {
                        select.Append("Contrato.CFT_NUMERO_CONTRATO ContratoFrete, ");
                        groupBy.Append("Contrato.CFT_NUMERO_CONTRATO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("Contrato.CFT_DATA_EMISSAO_CONTRATO DataEmissao, ");
                        groupBy.Append("Contrato.CFT_DATA_EMISSAO_CONTRATO, ");
                    }
                    break;

                case "NumeroCIOT":
                    if (!select.Contains(" NumeroCIOT, "))
                    {
                        select.Append("CIOT.CIO_NUMERO NumeroCIOT, ");
                        groupBy.Append("CIOT.CIO_NUMERO, ");

                        SetarJoinsCIOT(joins);
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

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_NOME FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) Motorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("((select vei.VEI_PLACA from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Veiculo, ");
                        groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Terceiro":
                    if (!select.Contains(" Terceiro, "))
                    {
                        select.Append("Terceiro.CLI_NOME Terceiro, ");
                        groupBy.Append("Terceiro.CLI_NOME, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;

                case "CPFCNPJTerceiroFormatado":
                    if (!select.Contains(" CPFCNPJTerceiro, "))
                    {
                        select.Append("Terceiro.CLI_CGCCPF CPFCNPJTerceiro, Terceiro.CLI_FISJUR TipoTerceiro, ");
                        groupBy.Append("Terceiro.CLI_CGCCPF, Terceiro.CLI_FISJUR, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;

                case "ValorAcrescimo":
                    if (!select.Contains(" ValorAcrescimo, "))
                    {
                        select.Append("(CASE WHEN ContratoValor.CFV_TIPO_JUSTIFICATIVA = 2 THEN ContratoValor.CFV_VALOR ELSE 0 END) ValorAcrescimo, ");

                        if (!groupBy.Contains("ContratoValor.CFV_TIPO_JUSTIFICATIVA"))
                            groupBy.Append("ContratoValor.CFV_TIPO_JUSTIFICATIVA, ");

                        if (!groupBy.Contains("ContratoValor.CFV_VALOR"))
                            groupBy.Append("ContratoValor.CFV_VALOR, ");

                        if (!groupBy.Contains("ContratoValor.CFV_CODIGO"))
                            groupBy.Append("ContratoValor.CFV_CODIGO, ");
                    }
                    break;

                case "ValorDesconto":
                    if (!select.Contains(" ValorDesconto, "))
                    {
                        select.Append("(CASE WHEN ContratoValor.CFV_TIPO_JUSTIFICATIVA = 1 THEN ContratoValor.CFV_VALOR ELSE 0 END) ValorDesconto, ");

                        if (!groupBy.Contains("ContratoValor.CFV_TIPO_JUSTIFICATIVA"))
                            groupBy.Append("ContratoValor.CFV_TIPO_JUSTIFICATIVA, ");

                        if (!groupBy.Contains("ContratoValor.CFV_VALOR"))
                            groupBy.Append("ContratoValor.CFV_VALOR, ");

                        if (!groupBy.Contains("ContratoValor.CFV_CODIGO"))
                            groupBy.Append("ContratoValor.CFV_CODIGO, ");
                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("Justificativa.JUS_DESCRICAO Justificativa, ");
                        groupBy.Append("Justificativa.JUS_DESCRICAO, ");

                        SetarJoinsJustificativa(joins);
                    }
                    break;

                case "TaxaTerceiro":
                    if (!select.Contains(" TaxaTerceiro, "))
                    {
                        select.Append("TaxaTerceiro.TAT_DESCRICAO TaxaTerceiro, ");
                        groupBy.Append("TaxaTerceiro.TAT_DESCRICAO, ");

                        SetarJoinsTaxaTerceiro(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsContratoValor(joins);

            if (filtrosPesquisa.CpfCnpjTerceiro > 0d)
                where.Append(" and Contrato.CLI_CGCCPF_TERCEIRO = " + filtrosPesquisa.CpfCnpjTerceiro.ToString());

            if (filtrosPesquisa.DataEmissaoContratoInicial != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEmissaoContratoInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataEmissaoContratoFinal != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEmissaoContratoFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.NumeroContrato > 0)
                where.Append(" and Contrato.CFT_NUMERO_CONTRATO = " + filtrosPesquisa.NumeroContrato.ToString());

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append(" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.NumeroCarga + "'");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Veiculo > 0)
            {
                where.Append($" and (Carga.CAR_VEICULO = {filtrosPesquisa.Veiculo} or exists (select VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = Carga.CAR_CODIGO and VEI_CODIGO = {filtrosPesquisa.Veiculo}))"); // SQL-INJECTION-SAFE

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Situacao.Count > 0)
                where.Append($" and Contrato.CFT_CONTRATO_FRETE in ({string.Join(",", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})");
        }

        #endregion
    }
}
