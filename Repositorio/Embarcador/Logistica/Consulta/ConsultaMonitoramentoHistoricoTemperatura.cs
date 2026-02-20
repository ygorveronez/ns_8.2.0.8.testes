using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoHistoricoTemperatura : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura>
    {
        #region Construtores

        public ConsultaMonitoramentoHistoricoTemperatura() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Monitoramento.VEI_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append(" join T_MONITORAMENTO Monitoramento on Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsPosicao(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Posicao "))
                joins.Append(" join T_POSICAO Posicao on Posicao.VEI_CODIGO = Veiculo.VEI_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            SetarJoinsTipoCarga(joins);

            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append(" join T_FAIXA_TEMPERATURA FaixaTemperatura on FaixaTemperatura.FTE_CODIGO = COALESCE(Carga.FTE_CODIGO, TipoCarga.FTE_CODIGO) ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append(" left join T_ROTA_FRETE Rota on Carga.ROF_CODIGO = Rota.ROF_CODIGO ");
        }

        private void SetarJoinOrigem(StringBuilder joins)
        {
            SetarJoinsRota(joins);

            if (!joins.Contains(" Origem "))
                joins.Append(" left join T_CLIENTE Origem on Origem.CLI_CGCCPF = Rota.CLI_CGCCPF ");
        }

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        private void SetarJoinDestino(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);
            if (!joins.Contains(" Destino "))
                joins.Append(" left join T_CLIENTE Destino on Destino.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtroPesquisa)
        {
            SetarJoinsMonitoramento(joins);
            SetarJoinsVeiculo(joins);
            SetarJoinsPosicao(joins);
            SetarJoinsFaixaTemperatura(joins);

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Posicao.POS_CODIGO as Codigo, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques, "))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + _veiculo.VEI_PLACA
		                                from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga
                                        join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO
		                                where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) AS Reboques, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");

                        SetarJoinEmpresa(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + Motorista.FUN_NOME ");
                        select.Append("      FROM T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      JOIN T_FUNCIONARIO Motorista ");
                        select.Append("        ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Motoristas, ");
                    }
                    break;

                case "DataEventoFormatada":
                    if (!select.Contains(" DataEvento, "))
                    {
                        select.Append("Posicao.POS_DATA_VEICULO as DataEvento, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "FaixaTemperatura":
                    if (!select.Contains(" FaixaTemperatura, "))
                    {
                        select.Append("FaixaTemperatura.FTE_DESCRICAO as FaixaTemperatura, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "FaixaInicial":
                    if (!select.Contains(" FaixaInicial, "))
                    {
                        select.Append("FaixaTemperatura.FTE_FAIXA_INICIAL as FaixaInicial, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "FaixaFinal":
                    if (!select.Contains(" FaixaFinal, "))
                    {
                        select.Append("FaixaTemperatura.FTE_FAIXA_FINAL as FaixaFinal, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "CDOrigem":
                    if (!select.Contains(" CDOrigem, "))
                    {
                        select.Append("Origem.CLI_NOME CDOrigem, ");

                        SetarJoinOrigem(joins);
                    }
                    break;

                case "PosicaoDescricao":
                    if (!select.Contains(" PosicaoDescricao, "))
                    {
                        select.Append("Posicao.POS_DESCRICAO as PosicaoDescricao, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("Destino.CLI_NOME Destino, ");

                        SetarJoinDestino(joins);
                    }
                    break;

                case "DataEntradaLojaFormatada":
                    if (!select.Contains(" DataEntradaLoja, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA as DataEntradaLoja, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataSaidaLojaFormatada":
                    if (!select.Contains(" DataSaidaLoja, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA as DataSaidaLoja, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "Latitude":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("Posicao.POS_LATITUDE as Latitude, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "Longitude":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("Posicao.POS_LONGITUDE as Longitude, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "Temperatura":
                    if (!select.Contains(" Temperatura, "))
                    {
                        select.Append("Posicao.POS_TEMPERATURA as Temperatura, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "DataCriacaoCarga":
                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO as DataCriacaoCarga, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd HH:mm";

            if (filtrosPesquisa.DataCriacaoCargaInicial != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoCargaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoCargaFinal != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoCargaFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and Posicao.POS_DATA_VEICULO >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and Posicao.POS_DATA_VEICULO <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DuranteMonitoramento)
                where.Append(@" and Monitoramento.MON_DATA_INICIO is not null and (
                    (
                        Monitoramento.MON_DATA_FIM is not null and Posicao.POS_DATA_VEICULO between Monitoramento.MON_DATA_INICIO and Monitoramento.MON_DATA_FIM
                    )
                    or
                    (
                        Monitoramento.MON_DATA_FIM is null and Posicao.POS_DATA_VEICULO > Monitoramento.MON_DATA_INICIO
                    )
                )");

            if (filtrosPesquisa.StatusMonitoramento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Todos)
                where.Append($" and Monitoramento.MON_STATUS = {filtrosPesquisa.StatusMonitoramento.ToString("d")}");

            if (filtrosPesquisa.ForaFaixa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                where.Append($" and Posicao.POS_TEMPERATURA between FaixaTemperatura.FTE_FAIXA_INICIAL and FaixaTemperatura.FTE_FAIXA_FINAL");
            if (filtrosPesquisa.ForaFaixa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                where.Append($" and (Posicao.POS_TEMPERATURA < FaixaTemperatura.FTE_FAIXA_INICIAL or Posicao.POS_TEMPERATURA > FaixaTemperatura.FTE_FAIXA_FINAL)");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");
                where.Append($" and Posicao.POS_DATA_VEICULO >= Monitoramento.MON_DATA_INICIO");
                where.Append($" and (Monitoramento.MON_DATA_FIM is null or Posicao.POS_DATA_VEICULO <= Monitoramento.MON_DATA_FIM) ");
            }

            if (filtrosPesquisa.CodigoFaixaTemperatura > 0)
                where.Append($" and FaixaTemperatura.FTE_CODIGO = {filtrosPesquisa.CodigoFaixaTemperatura}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Veiculo.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");

                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.EntregasRealizadas != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                SetarJoinsCargaEntrega(joins);

                if (filtrosPesquisa.EntregasRealizadas == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                    where.Append($" and CargaEntrega.CEN_DATA_INICIO_ENTREGA is null and CargaEntrega.CEN_DATA_ENTREGA is null");
                else
                    where.Append($" and CargaEntrega.CEN_DATA_INICIO_ENTREGA is not null and CargaEntrega.CEN_DATA_ENTREGA is not null");
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinsCargaEntrega(joins);
            }
            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)})");

            if (filtrosPesquisa.CodigosStatusViagem != null && filtrosPesquisa.CodigosStatusViagem.Count > 0)
            {
                where.Append($" and (");
                if (filtrosPesquisa.CodigosStatusViagem.Contains(-1))
                    where.Append($" Monitoramento.MSV_CODIGO is null or ");
                where.Append($" Monitoramento.MSV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosStatusViagem)}) )");
            }
        }

        #endregion
    }
}
