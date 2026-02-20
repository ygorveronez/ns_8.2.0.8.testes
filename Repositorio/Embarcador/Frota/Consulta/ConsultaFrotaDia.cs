using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaFrotaDia : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia>
    {
        #region Construtores

        public ConsultaFrotaDia() : base(tabela: "T_PLANEJAMENTO_FROTA_DIA_VEICULO as PlanejamentoFrotaDiaVeiculo ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsPlanejamentoFrotaDia(joins);

            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on PlanejamentoFrotaDia.FIL_CODIGO = Filial.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on PlanejamentoFrotaDiaVeiculo.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("left join T_VEICULO Veiculo on PlanejamentoFrotaDiaVeiculo.CAR_VEICULO = Veiculo.VEI_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
            {
                SetarJoinsModeloVeicularCarga(joins);
                joins.Append("left join T_EMPRESA Transportador on ModeloVeicularCarga.EMP_CODIGO = Transportador.EMP_CODIGO ");
            }
        }

        private void SetarJoinsPlanejamentoFrotaDia(StringBuilder joins)
        {
            if (!joins.Contains(" PlanejamentoFrotaDia "))
            {
                joins.Append("left join T_PLANEJAMENTO_FROTA_DIA PlanejamentoFrotaDia on PlanejamentoFrotaDiaVeiculo.PFD_CODIGO = PlanejamentoFrotaDia.PFD_CODIGO ");
            }
        } 
        
        private void SetarJoinsJustificativaDeIndisponibilidadeDeFrota(StringBuilder joins)
        {
            if (!joins.Contains(" JustificativaDeIndisponibilidadeDeFrota "))
            {
                joins.Append("left join T_JUSTIFICATIVA_INDISPONIBILIDADE_FROTA JustificativaDeIndisponibilidadeDeFrota on PlanejamentoFrotaDiaVeiculo.JIF_CODIGO = JustificativaDeIndisponibilidadeDeFrota.JIF_CODIGO ");
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PlanejamentoFrotaDiaVeiculo.PDV_CODIGO Codigo, ");
                        groupBy.Append("PlanejamentoFrotaDiaVeiculo.PDV_CODIGO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "PlacaFormatada":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("PlanejamentoFrotaDiaVeiculo.PDV_PLACA Placa, ");
                        groupBy.Append("PlanejamentoFrotaDiaVeiculo.PDV_PLACA, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "Capacidade":
                    if (!select.Contains(" Capacidade, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_CAPACIDADE_PESO_TRANSPORTE Capacidade, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CAPACIDADE_PESO_TRANSPORTE, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "AnoModelo":
                    if (!select.Contains(" AnoModelo, "))
                    {
                        select.Append("Veiculo.VEI_ANO AnoModelo, ");
                        groupBy.Append("Veiculo.VEI_ANO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + VeiculoMotorista.VMT_NOME ");
                        select.Append("      from T_VEICULO_MOTORISTA VeiculoMotorista ");
                        select.Append("     where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Motorista, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "CPFFormatado":
                    if (!select.Contains(" CPF, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + VeiculoMotorista.VMT_CPF ");
                        select.Append("      from T_VEICULO_MOTORISTA VeiculoMotorista ");
                        select.Append("     where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CPF, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TelefoneFormatado":
                    if (!select.Contains(" Telefone, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + Funcionario.FUN_FONE ");
                        select.Append("      from T_VEICULO_MOTORISTA VeiculoMotorista ");
                        select.Append("      left join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = VeiculoMotorista.FUN_CODIGO ");
                        select.Append("     where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Telefone, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "BrasilRiskMotorista":
                    if (!select.Contains(" BrasilRiskMotorista, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + cast(VeiculoMotorista.VMT_CODIGO as varchar(20)) ");
                        select.Append("      from T_VEICULO_MOTORISTA VeiculoMotorista ");
                        select.Append("     where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) BrasilRiskMotorista, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "BrasilRiskVeiculo":
                    if (!select.Contains(" BrasilRiskVeiculo, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + cast(VeiculoMotorista.VEI_CODIGO as varchar(20)) ");
                        select.Append("      from T_VEICULO_MOTORISTA VeiculoMotorista ");
                        select.Append("     where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) BrasilRiskVeiculo, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TesteFrio":
                    if (!select.Contains(" TesteFrio, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + LicencasVeiculo.VLI_NUMERO ");
                        select.Append("      from T_VEICULO_LICENCA LicencasVeiculo ");
                        select.Append("     where LicencasVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) TesteFrio, ");

                        if (!select.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ProprioOuAgregadoDescricao":
                    if (!select.Contains(" ProprioOuAgregado, "))
                    {
                        select.Append("Veiculo.VEI_TIPO ProprioOuAgregado, ");
                        groupBy.Append("Veiculo.VEI_TIPO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DisponivelEmFormatada":
                    if (!select.Contains(" DisponivelEm, "))
                    {
                        select.Append("PlanejamentoFrotaDia.PFD_DATA DisponivelEm, ");
                        groupBy.Append("PlanejamentoFrotaDia.PFD_DATA, ");

                        SetarJoinsPlanejamentoFrotaDia(joins);
                    }
                    break;

                case "MotivoIndisponibilidade":
                    if (!select.Contains(" MotivoIndisponibilidade, "))
                    {
                        select.Append("JustificativaDeIndisponibilidadeDeFrota.JIF_DESCRICAO MotivoIndisponibilidade, ");
                        groupBy.Append("JustificativaDeIndisponibilidadeDeFrota.JIF_DESCRICAO, ");

                        SetarJoinsJustificativaDeIndisponibilidadeDeFrota(joins);
                    }
                    break;

                case "ObservacaoTransportador":
                    if (!select.Contains(" ObservacaoTransportador, "))
                    {
                        select.Append("PlanejamentoFrotaDiaVeiculo.PDV_OBSERVACAO_TRANSPORTADOR ObservacaoTransportador, ");
                        groupBy.Append("PlanejamentoFrotaDiaVeiculo.PDV_OBSERVACAO_TRANSPORTADOR, ");
                    }
                    break;

                case "ObservacaoMarfrig":
                    if (!select.Contains(" ObservacaoMarfrig, "))
                    {
                        select.Append("PlanejamentoFrotaDiaVeiculo.PDV_OBSERVACAO_TRANSPORTADOR ObservacaoMarfrig, ");
                        groupBy.Append("PlanejamentoFrotaDiaVeiculo.PDV_OBSERVACAO_TRANSPORTADOR, ");
                    }
                    break;

                case "PaletizadoDescricao":
                    if (!select.Contains(" Paletizado, "))
                    {
                        select.Append("Veiculo.VEI_PALETIZADO Paletizado, ");
                        groupBy.Append("Veiculo.VEI_PALETIZADO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DataUltimoEmbarqueFormatada":
                    if (!select.Contains(" DataUltimoEmbarque, "))
                    {
                        select.Append("PlanejamentoFrotaDiaVeiculo.PDV_ULTIMO_EMBARQUE DataUltimoEmbarque, ");
                        groupBy.Append("PlanejamentoFrotaDiaVeiculo.PDV_ULTIMO_EMBARQUE, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigosFilial.Count > 0)
            {
                where.Append($" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                SetarJoinsFilial(joins);
            }

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
            {
                where.Append($" and Transportador.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

                SetarJoinsTransportador(joins);
            }

            if (filtrosPesquisa.PeriodoInicio != DateTime.MinValue)
            {
                where.Append(" and CAST(PlanejamentoFrotaDia.PFD_DATA AS DATE) >= '" + filtrosPesquisa.PeriodoInicio.ToString(pattern) + "'");

                SetarJoinsPlanejamentoFrotaDia(joins);
            }

            if (filtrosPesquisa.PeriodoFim != DateTime.MinValue)
            {
                where.Append(" and CAST(PlanejamentoFrotaDia.PFD_DATA AS DATE) <= '" + filtrosPesquisa.PeriodoFim.ToString(pattern) + "'");

                SetarJoinsPlanejamentoFrotaDia(joins);
            }

            if (filtrosPesquisa.CodigosVeiculo.Count > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)})");

                SetarJoinsVeiculo(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                where.Append($" and PlanejamentoFrotaDiaVeiculo.PDV_PLACA = '{filtrosPesquisa.Placa}'");

            if (filtrosPesquisa.Roteirizado.HasValue)
                where.Append($" and PlanejamentoFrotaDiaVeiculo.PFV_ROTEIRIZADO = " + Convert.ToInt32(filtrosPesquisa.Roteirizado.Value));

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" and PlanejamentoFrotaDiaVeiculo.PFV_INDISPONIVEL = " + Convert.ToInt32(filtrosPesquisa.Situacao.Value));

        }

        #endregion
    }
}
