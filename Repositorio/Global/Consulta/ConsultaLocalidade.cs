using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Global
{
    sealed class ConsultaLocalidade : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade>
    {
        #region Construtores

        public ConsultaLocalidade() : base(tabela: "T_LOCALIDADES as Localidade") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsEstado(StringBuilder joins)
        {
            if (!joins.Contains(" Estado "))
                joins.Append(" left join T_UF Estado on Estado.UF_SIGLA = Localidade.UF_SIGLA ");
        }
        
        private void SetarJoinsPais(StringBuilder joins)
        {
            if (!joins.Contains(" Pais "))
                joins.Append(" left join T_PAIS Pais on Pais.PAI_CODIGO = Localidade.PAI_CODIGO ");
        }

        private void SetarJoinsRegiao(StringBuilder joins)
        {
            if (!joins.Contains(" Regiao "))
                joins.Append(" left join T_REGIAO Regiao on Regiao.REG_CODIGO = Localidade.REG_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" , "))
                    {
                        select.Append("Localidade.LOC_CODIGO as Codigo, ");
                        groupBy.Append("Localidade.LOC_CODIGO, ");
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO as Descricao, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");
                    }
                    break;
                case "Estado":
                    if (!select.Contains(" Estado, "))
                    {
                        select.Append("Estado.UF_SIGLA as Estado, ");
                        groupBy.Append("Estado.UF_SIGLA, ");

                        SetarJoinsEstado(joins);
                    }
                    break;
                case "Pais":
                    if (!select.Contains(" Pais, "))
                    {
                        select.Append("Pais.PAI_NOME as Pais, ");
                        groupBy.Append("Pais.PAI_NOME, ");

                        SetarJoinsPais(joins);
                    }
                    break;
                case "Regiao":
                    if (!select.Contains(" Regiao, "))
                    {
                        select.Append("Regiao.REG_DESCRICAO as Regiao, ");
                        groupBy.Append("Regiao.REG_DESCRICAO, ");

                        SetarJoinsRegiao(joins);
                    }
                    break;
                case "Ibge":
                    if (!select.Contains(" Ibge, "))
                    {
                        select.Append("Localidade.LOC_IBGE as Ibge, ");
                        groupBy.Append("Localidade.LOC_IBGE, ");
                    }
                    break;
                case "Cep":
                    if (!select.Contains(" Cep, "))
                    {
                        select.Append("Localidade.LOC_CEP as Cep, ");
                        groupBy.Append("Localidade.LOC_CEP, ");
                    }
                    break;
                case "Latitude":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("Localidade.LOC_LATITUDE as Latitude, ");
                        groupBy.Append("Localidade.LOC_LATITUDE, ");
                    }
                    break;
                case "Longitude":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("Localidade.LOC_LONGITUDE as Longitude, ");
                        groupBy.Append("Localidade.LOC_LONGITUDE, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!String.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                where.Append($" AND Localidade.LOC_DESCRICAO LIKE '%{filtrosPesquisa.Descricao}%'");

            if (filtrosPesquisa.Estados.Count > 0)
                where.Append($" AND Localidade.UF_SIGLA IN ({String.Join(", ", filtrosPesquisa.Estados.Select(o => $"'{o}'"))})");

            if (filtrosPesquisa.Paises.Count > 0)
                where.Append($" AND Localidade.PAI_CODIGO IN ({String.Join(", ", filtrosPesquisa.Paises)})");

            if (filtrosPesquisa.Regioes.Count > 0)
                where.Append($" AND Localidade.REG_CODIGO IN ({String.Join(", ", filtrosPesquisa.Regioes)})");
        }

        #endregion
    }
}
