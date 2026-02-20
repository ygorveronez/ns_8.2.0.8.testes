using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaNavio : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio>
    {
        #region Construtores

        public ConsultaNavio() : base(tabela: "T_NAVIO as Navio ") { }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Navio.NAV_DESCRICAO Descricao, ");
                        groupBy.Append("Navio.NAV_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("Navio.NAV_CODIGO_INTEGRACAO CodigoIntegracao, ");
                        groupBy.Append("Navio.NAV_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "Status":
                case "StatusDescricao":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("Navio.NAV_STATUS Status, ");
                        groupBy.Append("Navio.NAV_STATUS, ");
                    }
                    break;

                case "CodigoIrin":
                    if (!select.Contains(" CodigoIrin, "))
                    {
                        select.Append("Navio.NAV_IRIN CodigoIrin, ");
                        groupBy.Append("Navio.NAV_IRIN, ");
                    }
                    break;

                case "CodigoEmbarcacao":
                    if (!select.Contains(" CodigoEmbarcacao, "))
                    {
                        select.Append("Navio.NAV_CODIGO_EMBARCACAO CodigoEmbarcacao, ");
                        groupBy.Append("Navio.NAV_CODIGO_EMBARCACAO, ");
                    }
                    break;

                case "TipoEmbarcacao":
                case "TipoEmbarcacaoDescricao":
                    if (!select.Contains(" TipoEmbarcacao, "))
                    {
                        select.Append("Navio.NAV_TIPO_EMBARCACAO TipoEmbarcacao, ");
                        groupBy.Append("Navio.NAV_TIPO_EMBARCACAO, ");
                    }
                    break;

                case "CodigoDocumentacao":
                    if (!select.Contains(" CodigoDocumentacao, "))
                    {
                        select.Append("Navio.NAV_CODIGO_DOCUMENTO CodigoDocumentacao, ");
                        groupBy.Append("Navio.NAV_CODIGO_DOCUMENTO, ");
                    }
                    break;

                case "CodigoIMO":
                    if (!select.Contains(" CodigoIMO, "))
                    {
                        select.Append("Navio.NAV_CODIGO_IMO CodigoIMO, ");
                        groupBy.Append("Navio.NAV_CODIGO_IMO, ");
                    }
                    break;

                case "NavioID":
                    if (!select.Contains(" NavioID, "))
                    {
                        select.Append("Navio.NAV_NAVIO_ID NavioID, ");
                        groupBy.Append("Navio.NAV_NAVIO_ID, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                where.Append($" and Navio.NAV_DESCRICAO like '%{filtrosPesquisa.Descricao}%'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                where.Append($" and Navio.NAV_CODIGO_INTEGRACAO like '%{filtrosPesquisa.CodigoIntegracao}%'");

            if(filtrosPesquisa.Status != SituacaoAtivoPesquisa.Todos)
                where.Append($" and Navio.NAV_STATUS {(filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo ? "=" : "!=")} 1");

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIrin))
                where.Append($" and Navio.NAV_IRIN like '%{filtrosPesquisa.CodigoIrin}%'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoEmbarcacao))
                where.Append($" and Navio.NAV_CODIGO_EMBARCACAO like '%{filtrosPesquisa.CodigoEmbarcacao}%'");

            if (filtrosPesquisa.TipoEmbarcacao.Count > 0)
                where.Append($" and Navio.NAV_TIPO_EMBARCACAO in ({string.Join(", ", filtrosPesquisa.TipoEmbarcacao.Select(o => (int)o).ToList())})");

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoDocumentacao))
                where.Append($" and Navio.NAV_CODIGO_DOCUMENTO like '%{filtrosPesquisa.CodigoDocumentacao}%'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIMO))
                where.Append($" and Navio.NAV_CODIGO_IMO like '%{filtrosPesquisa.CodigoIMO}%'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.NavioID))
                where.Append($" and Navio.NAV_NAVIO_ID like '%{filtrosPesquisa.NavioID}%'");

        }

        #endregion
    }
}
