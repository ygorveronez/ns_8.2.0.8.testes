using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    sealed class ConsultaPendenciaMotorista : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista>
    {
        #region Construtores

        public ConsultaPendenciaMotorista() : base(tabela: "T_PENDENCIA_MOTORISTA as PendenciaMotorista") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsJustificativa(StringBuilder joins)
        {
            if (!joins.Contains(" Justificativa "))
                joins.Append("left join T_JUSTIFICATIVA Justificativa on Justificativa.JUS_CODIGO = PendenciaMotorista.JUS_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append("left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = PendenciaMotorista.PEM_MOTORISTA ");
        }


        private void SetarJoinsPendencia(StringBuilder joins)
        {
            if (!joins.Contains(" Pendencia "))
                joins.Append("left join T_PAGAMENTO_MOTORISTA_TMS Pendencia on Pendencia.PAM_CODIGO = PendenciaMotorista.PAM_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtroPesquisa)
        {
            switch (propriedade)
            {
                case "DescricaoJustificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("CONCAT(Justificativa.JUS_CODIGO,' - ', Justificativa.JUS_DESCRICAO) as DescricaoJustificativa, ");
                        groupBy.Append("Justificativa.JUS_DESCRICAO, Justificativa.JUS_CODIGO, ");

                        SetarJoinsJustificativa(joins);
                    }
                    break;
                case "DescricaoPendencia":
                    if (!select.Contains(" Pendencia, "))
                    {
                        select.Append("CONCAT(Pendencia.PAM_CODIGO,' - ', Pendencia.PAM_OBSERVACAO) as DescricaoPendencia, Pendencia.PAM_VALOR as ValorPendencia, ");
                        groupBy.Append("Pendencia.PAM_OBSERVACAO, Pendencia.PAM_CODIGO, Pendencia.PAM_VALOR, ");

                        SetarJoinsPendencia(joins);
                    }
                    break;
                case "Situacao":
                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("PendenciaMotorista.PEM_SITUACAO as Situacao, ");
                        groupBy.Append("PendenciaMotorista.PEM_SITUACAO, ");
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("PendenciaMotorista.PEM_OBSERVACAO as Observacao, ");
                        groupBy.Append("PendenciaMotorista.PEM_OBSERVACAO, ");
                    }
                    break;

                case "CPFMotorista":
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        select.Append("Motorista.FUN_CPF as CPFMotorista, ");
                        groupBy.Append("Motorista.FUN_CPF, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;
                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("PendenciaMotorista.PEM_VALOR as Valor, ");
                        groupBy.Append("PendenciaMotorista.PEM_VALOR, ");
                    }
                    break;

                case "Data":
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("PendenciaMotorista.PEM_DATA as Data, ");
                        groupBy.Append("PendenciaMotorista.PEM_DATA, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" and PendenciaMotorista.PEM_MOTORISTA = {filtrosPesquisa.CodigoMotorista}");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CAST(PendenciaMotorista.PEM_DATA AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CAST(PendenciaMotorista.PEM_DATA AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.Situacao != SituacaoPendenciaMotorista.Todos)
                where.Append($" and PendenciaMotorista.PEM_SITUACAO = {(int)filtrosPesquisa.Situacao}");

            if (filtrosPesquisa.ValorInicial.HasValue && filtrosPesquisa.ValorInicial > 0)
                where.Append($" and CAST(PendenciaMotorista.PEM_VALOR AS NUMERIC) >= {filtrosPesquisa.ValorInicial.Value.ToString().Replace(",", ".")}");

            if (filtrosPesquisa.ValorFinal.HasValue && filtrosPesquisa.ValorFinal > 0)
                where.Append($" and CAST(PendenciaMotorista.PEM_VALOR AS NUMERIC) <= {filtrosPesquisa.ValorFinal.Value.ToString().Replace(",", ".")}");




        }

        #endregion
    }
}
