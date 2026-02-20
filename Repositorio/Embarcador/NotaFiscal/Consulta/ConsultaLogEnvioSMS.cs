using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.NotaFiscal
{
    sealed class ConsultaLogEnvioSMS : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS>
    {
        #region Construtores

        public ConsultaLogEnvioSMS() : base(tabela: "T_LOG_ENVIO_SMS as LogEnvioSMS") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append(" left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = LogEnvioSMS.CLI_CGCCPF ");
        }

        private void SetarJoinsNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" NotaFiscal "))
                joins.Append(" left join T_NOTA_FISCAL NotaFiscal on NotaFiscal.NFI_CODIGO = LogEnvioSMS.NFI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("LogEnvioSMS.LES_CODIGO as Codigo, ");
                        groupBy.Append("LogEnvioSMS.LES_CODIGO, ");
                    }
                    break;

                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("LogEnvioSMS.LES_DATA as Data, ");
                        groupBy.Append("LogEnvioSMS.LES_DATA, ");
                    }
                    break;

                case "Celular":
                    if (!select.Contains(" Celular, "))
                    {
                        select.Append("LogEnvioSMS.LES_CELULAR as Celular, ");
                        groupBy.Append("LogEnvioSMS.LES_CELULAR, ");
                    }
                    break;

                case "Link":
                    if (!select.Contains(" Link, "))
                    {
                        select.Append("LogEnvioSMS.LES_LINK as Link, ");
                        groupBy.Append("LogEnvioSMS.LES_LINK, ");
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

                case "Nota":
                    if (!select.Contains(" Nota, "))
                    {
                        select.Append("NotaFiscal.NFI_NUMERO as Nota, ");
                        groupBy.Append("NotaFiscal.NFI_NUMERO, ");

                        SetarJoinsNotaFiscal(joins);
                    }
                    break;

                case "Status":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("CASE WHEN LogEnvioSMS.LES_STATUS_ENVIO = 1 THEN 'Sim' ELSE 'Não' END as Status, ");
                        groupBy.Append("LogEnvioSMS.LES_STATUS_ENVIO, ");
                    }
                    break;

                case "Mensagem":
                    if (!select.Contains(" Mensagem, "))
                    {
                        select.Append("LogEnvioSMS.LES_MENSAGEM_ENVIO as Mensagem, ");
                        groupBy.Append("LogEnvioSMS.LES_MENSAGEM_ENVIO, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(LogEnvioSMS.LES_DATA AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(datePattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(LogEnvioSMS.LES_DATA AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(datePattern)}'");

            if (filtrosPesquisa.NumeroNotaInicial > 0)
            {
                where.Append($" and NotaFiscal.NFI_NUMERO >= {filtrosPesquisa.NumeroNotaInicial}");
                SetarJoinsNotaFiscal(joins);
            }

            if (filtrosPesquisa.NumeroNotaFinal > 0)
            {
                where.Append($" and NotaFiscal.NFI_NUMERO <= {filtrosPesquisa.NumeroNotaFinal}");
                SetarJoinsNotaFiscal(joins);
            }

            if (filtrosPesquisa.CodigoPessoa > 0)
                where.Append($" and LogEnvioSMS.CLI_CGCCPF = {filtrosPesquisa.CodigoPessoa}");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and LogEnvioSMS.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");
        }

        #endregion
    }
}
