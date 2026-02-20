using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaFreteTerceirizadoValePedagio : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio>
    {
        #region Construtores

        public ConsultaFreteTerceirizadoValePedagio() : base(tabela: "T_CONTRATO_FRETE_TERCEIRO as Contrato ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ");
        }

        private void SetarJoinsCargaValePedagio(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaValePedagio "))
                joins.Append(" left join T_CARGA_VALE_PEDAGIO CargaValePedagio on CargaValePedagio.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaIntegracaoValePedagio(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaIntegracaoValePedagio "))
                joins.Append(" left join T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio on CargaIntegracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO and CargaIntegracaoValePedagio.CVP_SITUACAO_VALE_PEDAGIO <> 8 and CargaIntegracaoValePedagio.CVP_SITUACAO_VALE_PEDAGIO <> 0 ");
        }

        private void SetarJoinsTipoIntegracao(StringBuilder joins)
        {
            SetarJoinsCargaIntegracaoValePedagio(joins);

            if (!joins.Contains(" TipoIntegracao "))
                joins.Append(" left join T_TIPO_INTEGRACAO TipoIntegracao on TipoIntegracao.TPI_CODIGO = CargaIntegracaoValePedagio.TPI_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        private void SetarJoinsTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" Terceiro "))
                joins.Append(" left join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ");
        }

        private void SetarJoinsTransportadorTerceiro(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TransportadorTerceiro "))
                joins.Append(" left join T_CLIENTE TransportadorTerceiro on TransportadorTerceiro.CLI_CGCCPF = Carga.CLI_CGCCPF_TERCEIRO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa)
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

                case "NumeroCTes":
                    if (!select.Contains(" NumeroCTes, "))
                    {
                        select.Append("SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS NVARCHAR(20)) FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL ");
                        select.Append(filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty);
                        select.Append(" FOR XML PATH('')), 3, 1000) NumeroCTes, ");

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

                case "DataNascimentoTerceiroFormatada":
                    if (!select.Contains(" DataNascimentoTerceiro, "))
                    {
                        select.Append("Terceiro.CLI_DATA_NASCIMENTO DataNascimentoTerceiro, ");
                        groupBy.Append("Terceiro.CLI_DATA_NASCIMENTO, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;

                case "PISPASEPTerceiro":
                    if (!select.Contains(" PISPASEPTerceiro, "))
                    {
                        select.Append("Terceiro.CLI_PIS_PASEP PISPASEPTerceiro, ");
                        groupBy.Append("Terceiro.CLI_PIS_PASEP, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;

                case "ValorPago":
                    if (!select.Contains(" ValorPago, "))
                    {
                        select.Append("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorPago, ");

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy.Append("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ");
                    }
                    break;

                case "ValorINSS":
                    if (!select.Contains(" ValorINSS, "))
                    {
                        select.Append("Contrato.CFT_VALOR_INSS ValorINSS, ");

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy.Append("Contrato.CFT_VALOR_INSS, ");
                    }
                    break;

                case "ValorIRRF":
                    if (!select.Contains(" ValorIRRF, "))
                    {
                        select.Append("Contrato.CFT_VALOR_IRRF ValorIRRF, ");

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy.Append("Contrato.CFT_VALOR_IRRF, ");
                    }
                    break;

                case "ValorSEST":
                    if (!select.Contains(" ValorSEST, "))
                    {
                        select.Append("Contrato.CFT_VALOR_SEST ValorSEST, ");

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy.Append("Contrato.CFT_VALOR_SEST, ");
                    }
                    break;

                case "ValorSENAT":
                    if (!select.Contains(" ValorSENAT, "))
                    {
                        select.Append("Contrato.CFT_VALOR_SENAT ValorSENAT, ");

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy.Append("Contrato.CFT_VALOR_SENAT, ");
                    }
                    break;

                case "NumeroValePedagio":
                    if (!select.Contains(" NumeroValePedagio, "))
                    {
                        select.Append("ISNULL(CargaValePedagio.CVP_NUMERO_COMPROVANTE, CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO) NumeroValePedagio, ");
                        groupBy.Append("CargaValePedagio.CVP_NUMERO_COMPROVANTE, CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO, ");

                        SetarJoinsCargaValePedagio(joins);
                        SetarJoinsCargaIntegracaoValePedagio(joins);
                    }
                    break;

                case "ValorValePedagio":
                    if (!select.Contains(" ValorValePedagio, "))
                    {
                        select.Append("ISNULL(CargaValePedagio.CVP_VALOR, CargaIntegracaoValePedagio.CVP_VALOR_VALE_PEDAGIO) ValorValePedagio, ");
                        groupBy.Append("CargaValePedagio.CVP_VALOR, CargaIntegracaoValePedagio.CVP_VALOR_VALE_PEDAGIO, ");

                        SetarJoinsCargaValePedagio(joins);
                        SetarJoinsCargaIntegracaoValePedagio(joins);
                    }
                    break;

                case "TipoIntegracaoFormatada":
                    if (!select.Contains(" TipoIntegracao, "))
                    {
                        select.Append("TipoIntegracao.TPI_TIPO TipoIntegracao, ");
                        groupBy.Append("TipoIntegracao.TPI_TIPO, ");

                        SetarJoinsTipoIntegracao(joins);
                    }
                    break;

                case "TransportadorValePedagio":
                    if (!select.Contains(" Terceiro, "))
                    {
                        select.Append("Terceiro.CLI_NOME Terceiro, ");
                        groupBy.Append("Terceiro.CLI_NOME, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;

                case "TransportadorTerceiro":
                    if (!select.Contains(" TransportadorTerceiro, "))
                    {
                        select.Append("TransportadorTerceiro.CLI_NOME TransportadorTerceiro, ");
                        groupBy.Append("TransportadorTerceiro.CLI_NOME, ");

                        SetarJoinsTransportadorTerceiro(joins);
                    }
                    break;

                case "NomeTransportador":
                    if (!select.Contains(" NomeTransportador, "))
                    {
                        select.Append("CargaIntegracaoValePedagio.CVP_NOME_TRANSPORTADOR NomeTransportador, ");
                        groupBy.Append("CargaIntegracaoValePedagio.CVP_NOME_TRANSPORTADOR, ");

                        SetarJoinsCargaIntegracaoValePedagio(joins);
                    }
                    break;


            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCargaValePedagio(joins);
            SetarJoinsCargaIntegracaoValePedagio(joins);

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0d)
                where.Append($" and Contrato.CLI_CGCCPF_TERCEIRO in ({string.Join(",", filtrosPesquisa.CpfCnpjTerceiros)})");

            if (filtrosPesquisa.DataEmissaoContratoInicial != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEmissaoContratoInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataEmissaoContratoFinal != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEmissaoContratoFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.NumeroContrato > 0)
                where.Append(" and Contrato.CFT_NUMERO_CONTRATO = " + filtrosPesquisa.NumeroContrato.ToString());

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append(" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.NumeroCarga + "'");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.Append($@" AND EXISTS (select CTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                            inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO 
                                            where CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO AND CON_NUM = {filtrosPesquisa.NumeroCTe})");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" and (Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or exists (select VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = Carga.CAR_CODIGO and VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}))"); // SQL-INJECTION-SAFE

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
            {
                where.Append($" and (ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicular} or exists (select VeiculoVinculado.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado INNER JOIN T_VEICULO Veiculo ON VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO and ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicular}))"); // SQL-INJECTION-SAFE

                SetarJoinsModeloVeicular(joins);
            }

            if (filtrosPesquisa.Situacao.Count > 0)
                where.Append($" and Contrato.CFT_CONTRATO_FRETE in ({string.Join(",", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.DataAprovacaoInicial != DateTime.MinValue)
                where.Append(" and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) >= '" + filtrosPesquisa.DataAprovacaoInicial.ToString(pattern) + "')"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataAprovacaoFinal != DateTime.MinValue)
                where.Append(" and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) <= '" + filtrosPesquisa.DataAprovacaoFinal.ToString(pattern) + "')"); 

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEncerramentoInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                where.Append(" and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEncerramentoFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append(" and Carga.TOP_CODIGO = " + filtrosPesquisa.CodigoTipoOperacao);

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue)
                where.Append($@" AND EXISTS (SELECT CFT_CODIGO FROM T_CARGA_CIOT cargaCIOT 
                                inner join T_CIOT ciot ON cargaCIOT.CIO_CODIGO = ciot.CIO_CODIGO 
                                where cargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO 
                                {(filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue ? $" AND CAST(ciot.CIO_DATA_ABERTURA AS DATE) >= '{filtrosPesquisa.DataAberturaCIOTInicial.ToString(pattern)}'" : string.Empty)}
                                {(filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue ? $" AND CAST(ciot.CIO_DATA_ABERTURA AS DATE) <= '{filtrosPesquisa.DataAberturaCIOTFinal.ToString(pattern)}'" : string.Empty)}
                                {(filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue ? $" AND CAST(ciot.CIO_DATA_ENCERRAMENTO AS DATE) >= '{filtrosPesquisa.DataEncerramentoCIOTInicial.ToString(pattern)}'" : string.Empty)}
                                {(filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue ? $" AND CAST(ciot.CIO_DATA_ENCERRAMENTO AS DATE) <= '{filtrosPesquisa.DataEncerramentoCIOTFinal.ToString(pattern)}'" : string.Empty)})");
        }

        #endregion
    }
}
