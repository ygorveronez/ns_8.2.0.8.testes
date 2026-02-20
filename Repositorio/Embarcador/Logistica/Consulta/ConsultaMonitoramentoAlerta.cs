using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoAlerta : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta>
    {
        #region Construtores

        public ConsultaMonitoramentoAlerta() : base(tabela: "T_ALERTA_MONITOR as Alerta") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinAlertaTratativa(StringBuilder joins)
        {
            if (!joins.Contains(" AlertaTratativa "))
            {
                joins.Append("left join T_ALERTA_TRATATIVA AlertaTratativa on AlertaTratativa.ALE_CODIGO = Alerta.ALE_CODIGO  ");
                joins.Append("left join T_ALERTA_TRATATIVA_ACAO AlertaTratativaAcao on AlertaTratativaAcao.ATC_CODIGO = AlertaTratativa.ATC_CODIGO ");
                joins.Append("left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = AlertaTratativa.FUN_CODIGO ");
            }
        }

        private void SetarJoinCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = Alerta.CAR_CODIGO ");
        }

        private void SetarJoinVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Alerta.VEI_CODIGO ");
        }

        private void SetarJoinMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
            {
                SetarJoinCarga(joins);
                joins.Append("left join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                joins.Append("left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = ISNULL(Alerta.FUN_CODIGO_MOTORISTA, CargaMotorista.CAR_MOTORISTA) ");
            }
        }

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinMonitoramentoEvento(StringBuilder joins)
        {
            if (!joins.Contains(" MonitoramentoEvento "))
                joins.Append("left join T_MONITORAMENTO_EVENTO MonitoramentoEvento on MonitoramentoEvento.MEV_CODIGO = Alerta.MEV_CODIGO ");
        }

        private void SetarJoinResponsavel(StringBuilder joins)
        {
            if (!joins.Contains(" Responsavel "))
                joins.Append("left join T_FUNCIONARIO Responsavel on Responsavel.FUN_CODIGO = Alerta.FUN_CODIGO ");
        }


        private void SetarJoinCausa(StringBuilder joins)
        {
            if (!joins.Contains(" Causa "))
                joins.Append("left join T_MONITORAMENTO_EVENTO_CAUSA Causa on Causa.MEC_CODIGO =  AlertaTratativa.MEC_CODIGO");

        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtroPesquisa)
        {

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Alerta.ALE_CODIGO Codigo, ");
                    }
                    break;
                case "DataFormatada":
                case "Data":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("Alerta.ALE_DATA as Data, ");
                    }
                    break;
                case "DataCriacaoFormatada":
                    if (!select.Contains(" DataCriacao, "))
                    {
                        select.Append("Alerta.ALE_DATA_CADASTRO as DataCriacao, ");
                    }
                    break;
                case "DataTratativaFormatada":
                    if (!select.Contains(" DataTratativa, "))
                    {
                        select.Append("Alerta.ALE_DATA_FIM as DataTratativa, ");
                    }
                    break;
                case "LatitudeFormatada":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("Alerta.ALE_LATITUDE as Latitude, ");
                    }
                    break;
                case "LongitudeFormatada":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("Alerta.ALE_LONGITUDE as Longitude, ");
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Alerta.ALE_DESCRICAO as Descricao, ");
                    }
                    break;
                case "StatusDescricao":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("Alerta.ALE_STATUS as Status, ");
                    }
                    break;
                case "TipoDescricao":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("Alerta.ALE_TIPO as Tipo, ");
                    }
                    break;
                case "NomeAlerta":
                    if (!select.Contains(" NomeAlerta, "))
                    {
                        SetarJoinMonitoramentoEvento(joins);
                        select.Append("MonitoramentoEvento.MEV_DESCRICAO as NomeAlerta, ");
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        SetarJoinAlertaTratativa(joins);
                        select.Append("AlertaTratativa.ATA_OBSERVACAO as Observacao, ");
                    }
                    break;
                case "Acao":
                    if (!select.Contains(" Acao, "))
                    {
                        SetarJoinAlertaTratativa(joins);
                        select.Append(@"(CASE 
                                        	WHEN Alerta.ALE_TRATATIVA_AUTOMATICA = 1 THEN 'Automática'
                                        	ELSE AlertaTratativaAcao.ATC_DESCRICAO
                                        END) as Acao, ");
                    }
                    break;
                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        SetarJoinAlertaTratativa(joins);
                        select.Append(@"(CASE 
                                            WHEN Alerta.ALE_TRATATIVA_AUTOMATICA = 1 THEN 'Sistema'
                                            ELSE Usuario.FUN_NOME
                                         END) AS Usuario, ");
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        SetarJoinVeiculo(joins);
                        select.Append("Veiculo.VEI_PLACA as Placa, ");
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        SetarJoinMotorista(joins);
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        SetarJoinCarga(joins);
                        SetarJoinEmpresa(joins);
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                    }
                    break;
                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador, "))
                    {
                        SetarJoinCarga(joins);
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        SetarJoinCarga(joins);
                        SetarJoinTipoOperacao(joins);
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                    }
                    break;
                case "AlertaPossuiPosicaoRetroativaDescricao":
                    if (!select.Contains(" AlertaPossuiPosicaoRetroativa, "))
                    {
                        SetarJoinTipoOperacao(joins);
                        select.Append("Alerta.ALE_POSICAO_RETROATIVA as AlertaPossuiPosicaoRetroativa, ");
                    }
                    break;
                case "FinalizadoSemRetornoSinalDescricao":
                    if (!select.Contains(" FinalizadoSemRetornoSinal "))
                    {
                        select.Append(" (select top 1 pps_perda_sinal_aberto from T_PERDA_SINAL_MONITORAMENTO perda where perda.ALE_CODIGO = Alerta.ALE_CODIGO and perda.pps_perda_sinal_aberto = 1) as FinalizadoSemRetornoSinal, ");
                    }
                    break;
                case "CPFMotorista":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        SetarJoinMotorista(joins);
                        select.Append("Motorista.FUN_CPF AS CPFMotorista, ");
                    }
                    break;
                case "CentroResultadoCarga":
                    if (!select.Contains(" CentroResultadoCarga, "))
                    {
                        select.Append("(SELECT TOP 1 CentroResultado.CRE_DESCRICAO FROM T_CARGA_PEDIDO CargaPedido LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = CargaPedido.CRE_CODIGO WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) AS CentroResultadoCarga, ");
                    }
                    break;
                case "Responsavel":
                    if (!select.Contains(" Responsavel, "))
                    {
                        SetarJoinResponsavel(joins);
                        select.Append("Responsavel.FUN_NOME AS Responsavel, ");
                    }
                    break;
                case "Causa":
                    if (!select.Contains(" Causa, "))
                    {
                        SetarJoinCausa(joins);
                        select.Append("Causa.MEC_DESCRICAO AS Causa, ");
                    }
                    break;
            }

        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.AlertaMonitorStatus != Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos)
            {
                where.Append($" and Alerta.ALE_STATUS = {(int)filtrosPesquisa.AlertaMonitorStatus}");
            }
            if (filtrosPesquisa.ApenasComPosicaoTardia)
            {
                where.Append($" and Alerta.ALE_POSICAO_RETROATIVA = 1");
            }
            if (filtrosPesquisa.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
            {
                where.Append($" and Alerta.ALE_TIPO = {(int)filtrosPesquisa.TipoAlerta}");
            }
            if (filtrosPesquisa.DataInicial != null)
            {
                where.Append($" and Alerta.ALE_DATA >= convert(datetime, '{filtrosPesquisa.DataInicial.Value.ToString("yyyy-MM-dd HH:mm:ss")}', 102) ");
            }
            if (filtrosPesquisa.DataFinal != null)
            {
                where.Append($" and Alerta.ALE_DATA <= convert(datetime, '{filtrosPesquisa.DataFinal.Value.ToString("yyyy-MM-dd HH:mm:ss")}', 102) ");
            }
            if (filtrosPesquisa.CodigoCargaEmbarcador != "")
            {
                SetarJoinCarga(joins);

                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%'");
                else
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");
            }
            if (filtrosPesquisa.Transportador > 0)
            {
                SetarJoinCarga(joins);
                SetarJoinEmpresa(joins);
                where.Append($" and Empresa.EMP_CODIGO = {filtrosPesquisa.Transportador}");
            }
            if (filtrosPesquisa.Placa != "")
            {
                SetarJoinVeiculo(joins);
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.Placa}%'");
            }
            if (filtrosPesquisa.Motorista > 0)
            {
                SetarJoinMotorista(joins);
                where.Append($" and Motorista.FUN_CODIGO = {filtrosPesquisa.Motorista}");
            }

            if (filtrosPesquisa.Filiais.Any(codigo => codigo == -1))
            {
                where.Append($@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )");
            }
            else if (filtrosPesquisa.Filiais.Count > 0)
                where.Append($@" AND Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)})");
        }

        #endregion
    }
}

