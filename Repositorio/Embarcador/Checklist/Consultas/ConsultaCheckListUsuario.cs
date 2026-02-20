using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Checklist.Consultas
{
    sealed class ConsultaCheckListUsuario : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario>
    {
        #region Construtores

        public ConsultaCheckListUsuario() : base(tabela: "T_CHECKLIST_RESPOSTA as Checklist") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on Checklist.FUN_CODIGO = Funcionario.FUN_CODIGO ");
        }

        private void SetarJoinTipoGROT(StringBuilder joins)
        {
            if (!joins.Contains(" TipoGROT "))
                joins.Append(" left join T_CHECKLIST TipoGROT on TipoGROT.CKL_CODIGO = Checklist.CKL_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("Checklist.CLR_CODIGO Codigo, ");
                groupBy.Append("Checklist.CLR_CODIGO, ");
            }

            switch (propriedade)
            {
                case "DataPreenchimentoFormatada":
                    if (!select.Contains(" DataPreenchimento, "))
                    {
                        select.Append("Checklist.CLR_DATA DataPreenchimento, ");
                        groupBy.Append("Checklist.CLR_DATA, ");
                    }
                    break;
                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Funcionario.FUN_NOME Usuario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinUsuario(joins);
                    }
                    break;
                case "TipoGROT":
                    if (!select.Contains(" TipoGROT, "))
                    {
                        select.Append("TipoGROT.CKL_DESCRICAO TipoGROT, ");
                        groupBy.Append("TipoGROT.CKL_DESCRICAO, ");

                        SetarJoinTipoGROT(joins);
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("Checklist.CLR_SITUACAO Situacao, ");
                        groupBy.Append("Checklist.CLR_SITUACAO, ");
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Checklist.CLR_OBSERVACAO Observacao, ");
                        groupBy.Append("Checklist.CLR_OBSERVACAO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoTipoGROT > 0)
                where.Append($" and TipoGROT.CKL_CODIGO = {filtrosPesquisa.CodigoTipoGROT}");

            if (filtrosPesquisa.CodigoUsuario > 0)
                where.Append($" and Funcionario.FUN_CODIGO = {filtrosPesquisa.CodigoUsuario}");

            if (filtrosPesquisa.DataPreenchimentoInicial != DateTime.MinValue)
                where.Append($" and CAST(Checklist.CLR_DATA AS DATE) >= '{filtrosPesquisa.DataPreenchimentoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataPreenchimentoFinal != DateTime.MinValue)
                where.Append($" and CAST(Checklist.CLR_DATA AS DATE) <= '{filtrosPesquisa.DataPreenchimentoFinal.ToString(pattern)}'");

        }

        #endregion
    }
}
