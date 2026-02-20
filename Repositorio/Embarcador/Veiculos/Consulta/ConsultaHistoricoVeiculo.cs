using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class ConsultaHistoricoVeiculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico>
    {
        #region Construtores

        public ConsultaHistoricoVeiculo() : base(tabela: "T_VEICULO_HISTORICO as VeiculoHistorico") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = VeiculoHistorico.FUN_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoHistorico.VEI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtroPesquisa)
        {
            switch (propriedade)
            {

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Data":
                    if (!select.Contains(" Data "))
                    {
                        select.Append("VeiculoHistorico.VHI_DATA as Data, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao "))
                    {
                        select.Append("CASE WHEN VeiculoHistorico.VHI_SITUACAO = 1 THEN 'Ativo' else 'Inativo' end as Situacao, ");
                        
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario "))
                    {
                        select.Append("Usuario.FUN_NOME as Usuario, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string patternHour = "yyyy-MM-dd HH:mm";
            if (filtrosPesquisa.DataInicial != null && filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" AND VeiculoHistorico.VHI_DATA >= '{filtrosPesquisa.DataInicial?.ToString(patternHour)}'");

            if (filtrosPesquisa.DataInicial != null && filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" AND VeiculoHistorico.VHI_DATA <= '{filtrosPesquisa.DataFinal?.ToString(patternHour)}'");

            if (filtrosPesquisa.CodigoVeiculo != null && filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" AND Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
        }

        #endregion
    }
}
