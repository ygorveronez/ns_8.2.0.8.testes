using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class ConsultaHistoricoVeiculoVinculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo>
    {
        #region Construtores

        public ConsultaHistoricoVeiculoVinculo() : base(tabela: "T_HISTORICO_VEICULO_VINCULO as Vinculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Vinculo.FUN_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Vinculo.VEI_CODIGO ");
        }

        private void SetarJoinsMarcaVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" MarcaVeiculo "))
                joins.Append(" left join T_VEICULO_MARCA MarcaVeiculo on MarcaVeiculo.VMA_CODIGO = Veiculo.VMA_CODIGO ");
        }

        private void SetarJoinsVinculoCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" VinculoCentroResultado "))
                joins.Append(" left outer join T_HISTORICO_VEICULO_VINCULO_CENTRO_RESULTADO VinculoCentroResultado on VinculoCentroResultado.HVV_CODIGO = Vinculo.HVV_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            SetarJoinsVinculoCentroResultado(joins);

            if (!joins.Contains(" CentroResultado "))
                joins.Append(" left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = VinculoCentroResultado.CRE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }

                    if (!select.Contains(" CodigoVinculoVeiculo, "))
                    {
                        select.Append("Vinculo.HVV_CODIGO as CodigoVinculoVeiculo, ");

                        if (!groupBy.Contains("Vinculo.HVV_CODIGO, "))
                            groupBy.Append("Vinculo.HVV_CODIGO, ");
                    }
                    break;

                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota, "))
                    {
                        select.Append("Veiculo.VEI_NUMERO_FROTA as NumeroFrota, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_FROTA, ");
                    }
                    break;

                case "DataHoraVinculo":
                case "DataHoraVinculoFormatada":
                    if (!select.Contains(" DataHoraVinculo, "))
                    {
                        select.Append("Vinculo.HVV_DATA_HORA as DataHoraVinculo, ");
                        groupBy.Append("Vinculo.HVV_DATA_HORA, ");
                    }
                    break;

                case "TipoPropriedade":
                    if (!select.Contains(" TipoPropriedade, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_TIPO = 'P' THEN 'Próprio' 
                                                WHEN Veiculo.VEI_TIPO = 'T' THEN 'Terceiro' 
                                            ELSE '' 
                                            END as TipoPropriedade, ");
                        groupBy.Append("Veiculo.VEI_TIPO, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + Motorista.FUN_NOME ");
                        select.Append("      from T_FUNCIONARIO Motorista ");
                        select.Append("      join T_HISTORICO_VEICULO_VINCULO_MOTORISTA VinculoMotorista on VinculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO ");
                        select.Append("     where VinculoMotorista.HVV_CODIGO = Vinculo.HVV_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Motorista, ");

                        if (!groupBy.Contains("Vinculo.HVV_CODIGO, "))
                            groupBy.Append("Vinculo.HVV_CODIGO, ");
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques, "))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + Reboque.VEI_PLACA ");
                        select.Append("      from T_VEICULO Reboque ");
                        select.Append("      join T_HISTORICO_VEICULO_VINCULO_REBOQUE VinculoReboque on VinculoReboque.VEI_CODIGO = Reboque.VEI_CODIGO ");
                        select.Append("     where VinculoReboque.HVV_CODIGO = Vinculo.HVV_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Reboques, ");

                        if (!groupBy.Contains("Vinculo.HVV_CODIGO, "))
                            groupBy.Append("Vinculo.HVV_CODIGO, ");
                    }
                    break;

                case "Equipamentos":
                    if (!select.Contains(" Equipamentos, "))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    SELECT ', ' + Equipamento.EQP_DESCRICAO ");
                        select.Append("      FROM T_EQUIPAMENTO Equipamento ");
                        select.Append("      INNER JOIN T_HISTORICO_VEICULO_VINCULO_EQUIPAMENTO VinculoEquipamento on VinculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO ");
                        select.Append("      WHERE VinculoEquipamento.HVV_CODIGO = Vinculo.HVV_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Equipamentos, ");

                        if (!groupBy.Contains("Vinculo.HVV_CODIGO, "))
                            groupBy.Append("Vinculo.HVV_CODIGO, ");
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("MarcaVeiculo.VMA_DESCRICAO as Marca, ");
                        groupBy.Append("MarcaVeiculo.VMA_DESCRICAO, ");

                        SetarJoinsMarcaVeiculo(joins);
                    }
                    break;

                case "QtdDiasVinculo":
                    if (!select.Contains(" QtdDiasVinculo, "))
                    {
                        select.Append("Vinculo.HVV_DIAS_VINCULADO as QtdDiasVinculo, ");
                        groupBy.Append("Vinculo.HVV_DIAS_VINCULADO, ");
                    }
                    break;

                case "KMRodadoVinculo":
                    if (!select.Contains(" KMRodadoVinculo, "))
                    {
                        select.Append("Vinculo.HVV_KM_RODADO as KMRodadoVinculo, ");
                        groupBy.Append("Vinculo.HVV_KM_RODADO, ");
                    }
                    break;

                case "KMVeiculoRealizarVinculo":
                    if (!select.Contains(" KMVeiculoRealizarVinculo, "))
                    {
                        select.Append("Vinculo.HVV_KM_ATUAL_MODIFICACAO as KMVeiculoRealizarVinculo, ");
                        groupBy.Append("Vinculo.HVV_KM_ATUAL_MODIFICACAO, ");
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
                        select.Append("VinculoCentroResultado.HVM_DATA_HORA as DataHoraCentroResultado, ");
                        groupBy.Append("VinculoCentroResultado.HVM_DATA_HORA, ");

                        SetarJoinsVinculoCentroResultado(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string patternHour = "yyyy-MM-dd HH:mm";

            if (filtrosPesquisa.DataHoraVinculoInicialHistoricoVeiculo != DateTime.MinValue)
                where.Append($" AND Vinculo.HVV_DATA_HORA >= '{filtrosPesquisa.DataHoraVinculoInicialHistoricoVeiculo.ToString(patternHour)}'");

            if (filtrosPesquisa.DataHoraVinculoFinalHistoricoVeiculo != DateTime.MinValue)
                where.Append($" AND Vinculo.HVV_DATA_HORA <= '{filtrosPesquisa.DataHoraVinculoFinalHistoricoVeiculo.ToString(patternHour)}:59'");

            if (filtrosPesquisa.DataInicialVinculoCentroResultado != DateTime.MinValue)
            {
                SetarJoinsVinculoCentroResultado(joins);

                where.Append($" AND VinculoCentroResultado.HVM_DATA_HORA >= '{filtrosPesquisa.DataInicialVinculoCentroResultado.ToString(patternHour)}'");
            }

            if (filtrosPesquisa.DataFinalVinculoCentroResultado != DateTime.MinValue)
            {
                SetarJoinsVinculoCentroResultado(joins);

                where.Append($" AND VinculoCentroResultado.HVM_DATA_HORA <= '{filtrosPesquisa.DataFinalVinculoCentroResultado.ToString(patternHour)}'");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and Vinculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");

            if (filtrosPesquisa.CodigoReboque > 0)
                where.Append($@"AND EXISTS (SELECT VinculoReboque.HVV_CODIGO FROM T_HISTORICO_VEICULO_VINCULO_REBOQUE VinculoReboque
                                 where VinculoReboque.VEI_CODIGO = {filtrosPesquisa.CodigoReboque} and VinculoReboque.HVV_CODIGO = Vinculo.HVV_CODIGO) ");

            if (filtrosPesquisa.CodigoEquipamento > 0)
                where.Append($@"AND EXISTS (SELECT VinculoEquipamento.HVV_CODIGO FROM T_HISTORICO_VEICULO_VINCULO_EQUIPAMENTO VinculoEquipamento
                                 where VinculoEquipamento.EQP_CODIGO = {filtrosPesquisa.CodigoEquipamento} and VinculoEquipamento.HVV_CODIGO = Vinculo.HVV_CODIGO) ");

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($@"AND EXISTS (SELECT VinculoMotorista.HVV_CODIGO FROM T_HISTORICO_VEICULO_VINCULO_MOTORISTA VinculoMotorista
                                 where VinculoMotorista.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista} and VinculoMotorista.HVV_CODIGO = Vinculo.HVV_CODIGO) ");
        }

        #endregion
    }
}
