using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Global.Consulta
{
    sealed class ConsultaLogAcesso : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogAcesso>
    {
        #region Construtores

        public ConsultaLogAcesso() : base(tabela: "T_LOG_ACESSO as LogAcesso") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("JOIN T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = LogAcesso.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogAcesso filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "IPAcesso":
                    if (!select.Contains(" IPAcesso, "))
                    {
                        select.Append("LogAcesso.LOG_IPACESSO IPAcesso, ");
                        groupBy.Append("LogAcesso.LOG_IPACESSO, ");
                    }
                    break;

                case "Login":
                    if (!select.Contains(" Login, "))
                    {
                        select.Append("LogAcesso.LOG_LOGIN Login, ");
                        groupBy.Append("LogAcesso.LOG_LOGIN, ");
                    }
                    break;

                case "Senha":
                    if (!select.Contains(" Senha, "))
                    {
                        select.Append("LogAcesso.LOG_SENHA Senha, ");
                        groupBy.Append("LogAcesso.LOG_SENHA, ");
                    }
                    break;

                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("LogAcesso.LOG_DATA Data, ");
                        groupBy.Append("LogAcesso.LOG_DATA, ");
                    }
                    break;

                case "SessionID":
                    if (!select.Contains(" SessionID, "))
                    {
                        select.Append("LogAcesso.LOG_SESSIONID SessionID, ");
                        groupBy.Append("LogAcesso.LOG_SESSIONID, ");
                    }
                    break;

                case "TipoFormatado":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("LogAcesso.LOG_TIPO Tipo, ");
                        groupBy.Append("LogAcesso.LOG_TIPO, ");
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Usuario.FUN_NOME as Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "CPFUsuarioFormatado":
                    if (!select.Contains(" CPFUsuario, "))
                    {
                        select.Append("Usuario.FUN_CPF as CPFUsuario, ");
                        groupBy.Append("Usuario.FUN_CPF, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogAcesso filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append(" and CAST(LogAcesso.LOG_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append(" and CAST(LogAcesso.LOG_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.TipoAcesso != null)
                where.Append($" AND LogAcesso.LOG_TIPO = {filtrosPesquisa.TipoAcesso.GetHashCode()} ");

            if (filtrosPesquisa.CodigoUsuario > 0)
                where.Append($" AND LogAcesso.FUN_CODIGO = {filtrosPesquisa.CodigoUsuario} ");
        }

        #endregion
    }
}
