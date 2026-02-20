using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CRM
{
    sealed class ConsultaAgendaTarefas : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas>
    {
        #region Construtores

        public ConsultaAgendaTarefas() : base(tabela: "T_AGENDA_TAREFA as AgendaTarefa") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = AgendaTarefa.FUN_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" left join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = AgendaTarefa.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("AgendaTarefa.ATA_CODIGO as Codigo, ");
                        groupBy.Append("AgendaTarefa.ATA_CODIGO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("AgendaTarefa.ATA_OBSERVACAO as Observacao, ");
                        groupBy.Append("AgendaTarefa.ATA_OBSERVACAO, ");
                    }
                    break;

                case "DataInicial":
                case "DataInicialFormatada":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select.Append("AgendaTarefa.ATA_DATA_INICIAL as DataInicial, ");
                        groupBy.Append("AgendaTarefa.ATA_DATA_INICIAL, ");
                    }
                    break;

                case "DataFinal":
                case "DataFinalFormatada":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select.Append("AgendaTarefa.ATA_DATA_FINAL as DataFinal, ");
                        groupBy.Append("AgendaTarefa.ATA_DATA_FINAL, ");
                    }
                    break;

                case "Colaborador":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Usuario.FUN_NOME as Colaborador, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Cliente.CLI_NOME as Cliente, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoCliente > 0)
            {
                where.Append(" AND Cliente.CGCCPF = " + filtrosPesquisa.CodigoCliente);
                SetarJoinsCliente(joins);
            }

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                where.Append(" AND Usuario.FUN_CODIGO = " + filtrosPesquisa.CodigoUsuario);
                SetarJoinsUsuario(joins);
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append(" and CAST(AgendaTarefa.ATA_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append(" and CAST(AgendaTarefa.ATA_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Observacao))
                where.Append($" AND UPPER(AgendaTarefa.ATA_OBSERVACAO) LIKE UPPER('%{filtrosPesquisa.Observacao}%')");

            if (filtrosPesquisa.Status.HasValue && filtrosPesquisa.Status.Value != 0)
                where.Append($" AND AgendaTarefa.ATA_STATUS = ('{filtrosPesquisa.Status.Value.ToString("d")}')");

        }

        #endregion
    }
}