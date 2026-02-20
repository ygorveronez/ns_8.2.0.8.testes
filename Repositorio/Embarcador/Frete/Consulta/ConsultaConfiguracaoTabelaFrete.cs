using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaConfiguracaoTabelaFrete : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete>
    {
        #region Construtores

        public ConsultaConfiguracaoTabelaFrete() : base(tabela: "T_TABELA_FRETE as TabelaFrete") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinVigencia(StringBuilder joins)
        {
            if (!joins.Contains(" Vigencia "))
                joins.Append("left join T_TABELA_FRETE_VIGENCIA Vigencia on Vigencia.TBF_CODIGO = TabelaFrete.TBF_CODIGO ");
        }

        private void SetarJoinGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append("left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = TabelaFrete.GRP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("TabelaFrete.TBF_CODIGO as Codigo, ");
                        groupBy.Append("TabelaFrete.TBF_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao,"))
                    {
                        select.Append("TabelaFrete.TBF_DESCRICAO as Descricao, ");
                        groupBy.Append("TabelaFrete.TBF_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao,"))
                    {
                        select.Append("TabelaFrete.TBF_CODIGO_INTEGRACAO as CodigoIntegracao, ");
                        groupBy.Append("TabelaFrete.TBF_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "DescricaoSituacao":
                case "Situacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("TabelaFrete.TBF_ATIVO as Situacao, ");
                        groupBy.Append("TabelaFrete.TBF_ATIVO, ");
                    }
                    break;

                case "DescricaoDataInicial":
                case "DataInicial":
                    if (!select.Contains(" DataInicial,"))
                    {
                        select.Append("Vigencia.TFV_DATA_INICIAL as DataInicial, ");
                        groupBy.Append("Vigencia.TFV_DATA_INICIAL, ");

                        SetarJoinVigencia(joins);
                    }
                    break;

                case "DescricaoDataFinal":
                case "DataFinal":
                    if (!select.Contains(" DataFinal,"))
                    {
                        select.Append("Vigencia.TFV_DATA_FINAL as DataFinal, ");
                        groupBy.Append("Vigencia.TFV_DATA_FINAL, ");

                        SetarJoinVigencia(joins);
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO as GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinGrupoPessoas(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoGrupoPessoas?.Count() > 0)
                where.Append($" and TabelaFrete.GRP_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigoGrupoPessoas)})");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" and TabelaFrete.TBF_ATIVO = ({(filtrosPesquisa.Situacao.Value ? "1" : "0")})");

            if (filtrosPesquisa.DataInicial.HasValue)
            {
                where.Append($" and Vigencia.TFV_DATA_INICIAL >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyy-MM-dd")}' and Vigencia.TFV_DATA_FINAL < '{filtrosPesquisa.DataInicial.Value.AddDays(1).ToString("yyyy-MM-dd")}'");

                SetarJoinVigencia(joins);
            }

            if (filtrosPesquisa.DataFinal.HasValue)
            {
                where.Append($" and Vigencia.TFV_DATA_INICIAL <= '{filtrosPesquisa.DataFinal.Value.ToString("yyyy-MM-dd")}' and Vigencia.TFV_DATA_FINAL >= '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString("yyyy-MM-dd")}'");
                
                SetarJoinVigencia(joins);
            }

            if (filtrosPesquisa.TabelasVigentes.HasValue)
            {
                if (filtrosPesquisa.TabelasVigentes.Value)
                    where.Append($" and Vigencia.TFV_DATA_INICIAL <= '{DateTime.Now.ToString("yyyy-MM-dd")}' and Vigencia.TFV_DATA_FINAL >= '{DateTime.Now.ToString("yyyy-MM-dd")}'");
                else
                    where.Append($" and (Vigencia.TFV_DATA_INICIAL > '{DateTime.Now.ToString("yyyy-MM-dd")}' or Vigencia.TFV_DATA_FINAL < '{DateTime.Now.ToString("yyyy-MM-dd")}')");

                SetarJoinVigencia(joins);
            }
        }

        #endregion
    }
}
