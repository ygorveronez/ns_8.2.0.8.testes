using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaPracaPedagio : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioPracaPedagio>
    {
        #region Construtores

        public ConsultaPracaPedagio() : base(tabela: "T_PRACA_PEDAGIO as PracaPedagio ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPracaPedagioTarifa(StringBuilder joins)
        {
            if (!joins.Contains(" PracaPedagioTarifa "))
                joins.Append(" left join T_PRACA_PEDAGIO_TARIFA PracaPedagioTarifa on PracaPedagioTarifa.PRP_CODIGO = PracaPedagio.PRP_CODIGO ");
        }

        private void SetarJoinsPracaPedagioTarifaModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsPracaPedagioTarifa(joins);

            if (!joins.Contains(" ModeloVeicularCargaTarifa "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicularCargaTarifa on ModeloVeicularCargaTarifa.MVC_CODIGO = PracaPedagioTarifa.MVC_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioPracaPedagio filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("PracaPedagio.PRP_DESCRICAO Descricao, ");
                        groupBy.Append("PracaPedagio.PRP_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("PracaPedagio.PRP_CODIGO_INTEGRACAO CodigoIntegracao, ");
                        groupBy.Append("PracaPedagio.PRP_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "Rodovia":
                    if (!select.Contains(" Rodovia, "))
                    {
                        select.Append("PracaPedagio.PRP_RODOVIA Rodovia, ");
                        groupBy.Append("PracaPedagio.PRP_RODOVIA, ");
                    }
                    break;

                case "KM":
                    if (!select.Contains(" KM, "))
                    {
                        select.Append("PracaPedagio.PRP_KM KM, ");
                        groupBy.Append("PracaPedagio.PRP_KM, ");
                    }
                    break;

                case "DescricaoAtivo":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("PracaPedagio.PRP_ATIVO Situacao, ");
                        groupBy.Append("PracaPedagio.PRP_ATIVO, ");
                    }
                    break;

                case "Concessionaria":
                    if (!select.Contains(" Concessionaria, "))
                    {
                        select.Append("PracaPedagio.PRP_CONCESSIONARIA Concessionaria, ");
                        groupBy.Append("PracaPedagio.PRP_CONCESSIONARIA, ");
                    }
                    break;

                case "ValorTarifa":
                    if (!select.Contains(" ValorTarifa, "))
                    {
                        select.Append("PracaPedagioTarifa.PPT_TARIFA ValorTarifa, ");
                        groupBy.Append("PracaPedagioTarifa.PPT_TARIFA, ");

                        SetarJoinsPracaPedagioTarifa(joins);
                    }
                    break;

                case "DataTarifaFormatada":
                    if (!select.Contains(" DataTarifa, "))
                    {
                        select.Append("PracaPedagioTarifa.PPT_DATA DataTarifa, ");
                        groupBy.Append("PracaPedagioTarifa.PPT_DATA, ");

                        SetarJoinsPracaPedagioTarifa(joins);
                    }
                    break;

                case "ModeloVeicularCargaTarifa":
                    if (!select.Contains(" ModeloVeicularCargaTarifa, "))
                    {
                        select.Append("ModeloVeicularCargaTarifa.MVC_DESCRICAO ModeloVeicularCargaTarifa, ");
                        groupBy.Append("ModeloVeicularCargaTarifa.MVC_DESCRICAO, ");

                        SetarJoinsPracaPedagioTarifaModeloVeicularCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioPracaPedagio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                where.Append($" and PracaPedagio.PRP_DESCRICAO like '%{filtrosPesquisa.Descricao}%'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.Rodovia))
                where.Append($" and PracaPedagio.PRP_RODOVIA = '{filtrosPesquisa.Rodovia}'");

            if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo)
                where.Append($" and PracaPedagio.PRP_ATIVO = 1");
            else if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Inativo)
                where.Append($" and (PracaPedagio.PRP_ATIVO = 0 or PracaPedagio.PRP_ATIVO IS NULL)");

            if (filtrosPesquisa.DataTarifaInicial != DateTime.MinValue)
            {
                where.Append(" and CAST(PracaPedagioTarifa.PPT_DATA AS DATE) >= '" + filtrosPesquisa.DataTarifaInicial.ToString(pattern) + "'");

                SetarJoinsPracaPedagioTarifa(joins);
            }

            if (filtrosPesquisa.DataTarifaFinal != DateTime.MinValue)
            {
                where.Append(" and CAST(PracaPedagioTarifa.PPT_DATA AS DATE) <= '" + filtrosPesquisa.DataTarifaFinal.ToString(pattern) + "'");

                SetarJoinsPracaPedagioTarifa(joins);
            }
        }

        #endregion
    }
}
