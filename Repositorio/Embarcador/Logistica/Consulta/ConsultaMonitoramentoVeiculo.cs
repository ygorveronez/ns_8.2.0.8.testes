using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica
{
    sealed class ConsultaMonitoramentoVeiculo : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo>
    {
        #region Construtores

        public ConsultaMonitoramentoVeiculo() : base(tabela: "T_VEICULO as Veiculo") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoVeiculo":
                    select.Append("Veiculo.VEI_CODIGO as CodigoVeiculo, ");
                    break;

                case "Placa":
                    select.Append("Veiculo.VEI_PLACA as Placa, ");
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                        SetarJoinEmpresa(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            where.Append($" and Veiculo.VEI_ATIVO = 1");

            if (filtrosPesquisa.CodigosVeiculo != null && filtrosPesquisa.CodigosVeiculo.Count > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosVeiculo)})");
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Veiculo.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");
                SetarJoinEmpresa(joins);
            }

            if ( filtrosPesquisa.CodigosContratoFrete?.Count > 0)
            {
                where.Append(" and exists ( ");
                where.Append("   SELECT ");
                where.Append("       ContratoFreteTransportador.CFT_CODIGO ");
                where.Append("   FROM ");
                where.Append("       T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador ");
                where.Append("   JOIN ");
                where.Append("       T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo ON ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO ");
                where.Append("   WHERE ");
                where.Append("       ContratoFreteTransportador.CFT_ATIVO = 1 ");
                where.Append("       AND ContratoFreteTransportador.CFT_SITUACAO in (1,2) ");
                where.Append("       AND CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL ");
                where.Append("       AND ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO ");

                if (filtrosPesquisa.CodigosContratoFrete?.Count > 0)
                    where.Append($"       AND ContratoFreteTransportador.CFT_CODIGO in({ string.Join(", ", filtrosPesquisa.CodigosContratoFrete) }) ");

                where.Append(") ");
            }
        }

        #endregion
    }
}
