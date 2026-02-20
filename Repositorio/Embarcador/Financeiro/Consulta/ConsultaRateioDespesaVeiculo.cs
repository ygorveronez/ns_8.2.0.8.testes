using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaRateioDespesaVeiculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo>
    {
        #region Construtores

        public ConsultaRateioDespesaVeiculo() : base(tabela: "T_RATEIO_DESPESA_VEICULO as RateioDespesaVeiculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsTipoDespesa(StringBuilder joins)
        {
            if (!joins.Contains(" TipoDespesa "))
                joins.Append(" LEFT JOIN T_TIPO_DESPESA_FINANCEIRA TipoDespesa ON TipoDespesa.TID_CODIGO = RateioDespesaVeiculo.TID_CODIGO ");
        }

        private void SetarJoinsGrupoDespesa(StringBuilder joins)
        {
            SetarJoinsTipoDespesa(joins);

            if (!joins.Contains(" GrupoDespesa "))
                joins.Append(" LEFT JOIN T_GRUPO_DESPESA_FINANCEIRA GrupoDespesa ON GrupoDespesa.TGD_CODIGO = TipoDespesa.TGD_CODIGO ");
        }

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append(" LEFT JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = RateioDespesaVeiculo.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtroPesquisa)
        {

            switch (propriedade)
            {
                case "DataInicialFormatada":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_DATA_INICIAL as DataInicial, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_DATA_INICIAL, ");
                    }
                    break;

                case "DataFinalFormatada":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_DATA_FINAL as DataFinal, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_DATA_FINAL, ");
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_NUMERO_DOCUMENTO as NumeroDocumento, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_NUMERO_DOCUMENTO, ");
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_TIPO_DOCUMENTO as TipoDocumento, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_TIPO_DOCUMENTO, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_VALOR as Valor,  ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_VALOR, ");
                    }
                    break;

                case "TipoDespesa":
                    if (!select.Contains(" TipoDespesa, "))
                    {
                        select.Append("TipoDespesa.TID_DESCRICAO as TipoDespesa,  ");
                        groupBy.Append("TipoDespesa.TID_DESCRICAO, ");

                        SetarJoinsTipoDespesa(joins);
                    }
                    break;

                case "GrupoDespesa":
                    if (!select.Contains(" GrupoDespesa, "))
                    {
                        select.Append("GrupoDespesa.TGD_DESCRICAO as GrupoDespesa,  ");
                        groupBy.Append("GrupoDespesa.TGD_DESCRICAO, ");

                        SetarJoinsGrupoDespesa(joins);
                    }
                    break;

                case "Segmentos":
                    if (!select.Contains(" Segmentos,"))
                    {
                        select.Append($" substring((select ', ' + SegmentoVeiculo.VSE_DESCRICAO ");
                        select.Append($"    from T_RATEIO_DESPESA_VEICULO_SEGMENTO_VEICULO RateioSegmentoVeiculo ");
                        select.Append($"    inner join T_VEICULO_SEGMENTO SegmentoVeiculo ON SegmentoVeiculo.VSE_CODIGO = RateioSegmentoVeiculo.VSE_CODIGO ");
                        select.Append($"    where RateioSegmentoVeiculo.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO for xml path('')), 3, 1000) Segmentos, ");

                        if (!groupBy.Contains("RateioDespesaVeiculo.TRD_CODIGO"))
                            groupBy.Append("RateioDespesaVeiculo.TRD_CODIGO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos,"))
                    {
                        select.Append($" substring((select ', ' + Veiculo.VEI_PLACA ");
                        select.Append($"    from T_RATEIO_DESPESA_VEICULO_VEICULO RateioVeiculo ");
                        select.Append($"    inner join T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = RateioVeiculo.VEI_CODIGO ");
                        select.Append($"    where RateioVeiculo.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO for xml path('')), 3, 1000) Veiculos, ");

                        if (!groupBy.Contains("RateioDespesaVeiculo.TRD_CODIGO"))
                            groupBy.Append("RateioDespesaVeiculo.TRD_CODIGO, ");
                    }
                    break;

                case "CentrosResultados":
                    if (!select.Contains(" CentrosResultados,"))
                    {
                        select.Append($" substring((select ', ' + CentroResultado.CRE_DESCRICAO ");
                        select.Append($"    from T_RATEIO_DESPESA_VEICULO_CENTRO_RESULTADO RateioCentroResultado ");
                        select.Append($"    inner join T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = RateioCentroResultado.CRE_CODIGO ");
                        select.Append($"    where RateioCentroResultado.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO for xml path('')), 3, 1000) CentrosResultados, ");

                        if (!groupBy.Contains("RateioDespesaVeiculo.TRD_CODIGO"))
                            groupBy.Append("RateioDespesaVeiculo.TRD_CODIGO, ");
                    }
                    break;

                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        select.Append("Pessoa.CLI_NOME as Pessoa, ");
                        groupBy.Append("Pessoa.CLI_NOME, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "DataLancamentoFormatada":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_DATA_LANCAMENTO as DataLancamento, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_DATA_LANCAMENTO, ");
                    }
                    break;

                case "OrigemRateioFormatada":
                    if (!select.Contains(" OrigemRateio, "))
                    {
                        select.Append("RateioDespesaVeiculo.TRD_ORIGEM as OrigemRateio, ");
                        groupBy.Append("RateioDespesaVeiculo.TRD_ORIGEM, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" AND CAST(RateioDespesaVeiculo.TRD_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" AND CAST(RateioDespesaVeiculo.TRD_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.CodigosTipoDespesa?.Count > 0)
            {
                where.Append($" AND TipoDespesa.TID_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoDespesa)})");
                SetarJoinsTipoDespesa(joins);
            }

            if (filtrosPesquisa.CodigosGrupoDespesa?.Count > 0)
            {
                where.Append($" AND GrupoDespesa.TGD_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoDespesa)})");
                SetarJoinsGrupoDespesa(joins);
            }

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
                where.Append($" and exists (select RateioVeiculo.TRD_CODIGO from T_RATEIO_DESPESA_VEICULO_VEICULO RateioVeiculo where RateioVeiculo.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)}) and RateioVeiculo.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO)");

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
                where.Append($" and exists (select RateioCentroResultado.TRD_CODIGO from T_RATEIO_DESPESA_VEICULO_CENTRO_RESULTADO RateioCentroResultado where RateioCentroResultado.CRE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) and RateioCentroResultado.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO)"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
                where.Append($" and exists (select RateioSegmentoVeiculo.TRD_CODIGO from T_RATEIO_DESPESA_VEICULO_SEGMENTO_VEICULO RateioSegmentoVeiculo where RateioSegmentoVeiculo.VSE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosSegmentoVeiculo)}) and RateioSegmentoVeiculo.TRD_CODIGO = RateioDespesaVeiculo.TRD_CODIGO)"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CpfCnpjPessoa > 0)
                where.Append($" and RateioDespesaVeiculo.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjPessoa}");

            if (filtrosPesquisa.DataLancamentoInicial != DateTime.MinValue)
                where.Append($" AND CAST(RateioDespesaVeiculo.TRD_DATA_LANCAMENTO AS DATE) >= '{filtrosPesquisa.DataLancamentoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataLancamentoFinal != DateTime.MinValue)
                where.Append($" AND CAST(RateioDespesaVeiculo.TRD_DATA_LANCAMENTO AS DATE) <= '{filtrosPesquisa.DataLancamentoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.OrigemRateio.HasValue)
                where.Append($" and RateioDespesaVeiculo.TRD_ORIGEM = {filtrosPesquisa.OrigemRateio.Value.ToString("d")}");
        }

        #endregion
    }
}

