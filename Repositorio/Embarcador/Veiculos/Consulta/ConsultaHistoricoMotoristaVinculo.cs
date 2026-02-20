using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class ConsultaHistoricoMotoristaVinculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista>
    {
        #region Construtores

        public ConsultaHistoricoMotoristaVinculo() : base(tabela: "T_HISTORICO_MOTORISTA_VINCULO as Vinculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Vinculo.FUN_CODIGO_USUARIO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append(" left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Vinculo.FUN_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsVinculoCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" VinculoCentroResultado "))
                joins.Append(" left outer join T_HISTORICO_MOTORISTA_VINCULO_CENTRO_RESULTADO VinculoCentroResultado on VinculoCentroResultado.HMV_CODIGO = Vinculo.HMV_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            SetarJoinsVinculoCentroResultado(joins);

            if (!joins.Contains(" CentroResultado "))
                joins.Append(" left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = VinculoCentroResultado.CRE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }

                    if (!select.Contains(" CodigoVinculoMotorista, "))
                    {
                        select.Append("Vinculo.HMV_CODIGO as CodigoVinculoMotorista, ");

                        if (!groupBy.Contains("Vinculo.HMV_CODIGO, "))
                            groupBy.Append("Vinculo.HMV_CODIGO, ");
                    }
                    break;

                    break;

                case "DataHoraVinculo":
                case "DataHoraVinculoFormatada":
                    if (!select.Contains(" DataHoraVinculo, "))
                    {
                        select.Append("Vinculo.HMV_DATA_HORA as DataHoraVinculo, ");
                        groupBy.Append("Vinculo.HMV_DATA_HORA, ");
                    }
                    break;

                case "QtdDiasVinculo":
                    if (!select.Contains(" QtdDiasVinculo, "))
                    {
                        select.Append("Vinculo.HMV_DIAS_VINCULADO as QtdDiasVinculo, ");
                        groupBy.Append("Vinculo.HMV_DIAS_VINCULADO, ");
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario "))
                    {
                        select.Append("Usuario.FUN_NOME as Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO as CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "DataHoraCentroResultadoFormatada":
                    if (!select.Contains(" DataHoraCentroResultado, "))
                    {
                        select.Append("VinculoCentroResultado.HMM_DATA_HORA as DataHoraCentroResultado, ");
                        groupBy.Append("VinculoCentroResultado.HMM_DATA_HORA, ");

                        SetarJoinsVinculoCentroResultado(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string patternHour = "yyyy-MM-dd HH:mm";

            if (filtrosPesquisa.DataHoraVinculoInicialHistoricoMotorista != DateTime.MinValue)
                where.Append($" AND Vinculo.HMV_DATA_HORA >= '{filtrosPesquisa.DataHoraVinculoInicialHistoricoMotorista.ToString(patternHour)}'");

            if (filtrosPesquisa.DataHoraVinculoFinalHistoricoMotorista != DateTime.MinValue)
                where.Append($" AND Vinculo.HMV_DATA_HORA <= '{filtrosPesquisa.DataHoraVinculoFinalHistoricoMotorista.ToString(patternHour)}:59'");

            if (filtrosPesquisa.DataInicialVinculoCentroResultado != DateTime.MinValue)
            {
                SetarJoinsVinculoCentroResultado(joins);

                where.Append($" AND VinculoCentroResultado.HMM_DATA_HORA >= '{filtrosPesquisa.DataInicialVinculoCentroResultado.ToString(patternHour)}'");
            }

            if (filtrosPesquisa.DataFinalVinculoCentroResultado != DateTime.MinValue)
            {
                SetarJoinsVinculoCentroResultado(joins);

                where.Append($" AND VinculoCentroResultado.HMM_DATA_HORA <= '{filtrosPesquisa.DataFinalVinculoCentroResultado.ToString(patternHour)}'");
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" and Vinculo.FUN_CODIGO_MOTORISTA = {filtrosPesquisa.CodigoMotorista}");
        }

        #endregion
    }
}
