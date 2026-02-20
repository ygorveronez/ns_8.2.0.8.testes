using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    sealed class ConsultaCargaEntregaChecklist : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist>
    {
        #region Construtores

        public ConsultaCargaEntregaChecklist() : base(tabela: "T_CARGA_ENTREGA_CHECKLIST as CargaEntregaChecklist") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCheckListTipo(StringBuilder joins)
        {
            if (!joins.Contains(" CheckListTipo "))
                joins.Append(" left join T_CHECK_LIST_TIPOS CheckListTipo on CheckListTipo.CLT_CODIGO = CargaEntregaChecklist.CLT_CODIGO");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS  CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO  = Carga.CDS_CODIGO");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append(" left join T_MONITORAMENTO Monitoramento on Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("CargaEntregaChecklist.CEC_CODIGO as Codigo, ");
                        groupBy.Append("CargaEntregaChecklist.CEC_CODIGO, ");
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa,"))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.Append("Transportador.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "TipoChecklist":
                    if (!select.Contains(" TipoChecklist,"))
                    {
                        select.Append("CheckListTipo.CLT_DESCRICAO as TipoChecklist, ");
                        groupBy.Append("CheckListTipo.CLT_DESCRICAO, ");

                        SetarJoinsCheckListTipo(joins);
                    }
                    break;

                case "DataCriacaoCarga":
                    if (!select.Contains(" carga.CAR_DATA_CRIACAO DataCriacaoCarga, "))
                    {
                        select.Append("carga.CAR_DATA_CRIACAO as DataCriacaoCarga, ");
                        groupBy.Append("carga.CAR_DATA_CRIACAO, ");

                    }
                    break;

                case "DataDeEmbarque":
                    if (!select.Contains(" carga.CAR_DATA_CARREGAMENTO DataDeEmbarque, "))
                    {
                        select.Append("carga.CAR_DATA_CARREGAMENTO DataDeEmbarque, ");
                        groupBy.Append("carga.CAR_DATA_CARREGAMENTO, ");

                    }
                    break;

                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista, "))
                    {
                        select.Append(@"substring(( 
                            select ', ' + Usuario.FUN_NOME 
                                from T_FUNCIONARIO Usuario 
                                join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO 
                                where Usuario.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA 
                               for xml path('') 
                        ), 3, 1000) NomeMotorista, ");

                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Local":
                    if (!select.Contains(" Local, "))
                    {
                        select.Append("CONCAT('Lon: ',carga.CAR_LONGITUDE_INICIO_VIAGEM,' Lat: ',carga.CAR_LATITUDE_INICIO_VIAGEM) Local, ");
                        groupBy.Append("carga.CAR_LONGITUDE_INICIO_VIAGEM,carga.CAR_LATITUDE_INICIO_VIAGEM, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "KMRealizados":
                    if (!select.Contains(" KMRealizados, "))
                    {
                        select.Append("Monitoramento.MON_DISTANCIA_REALIZADA KMRealizados, ");
                        groupBy.Append("Monitoramento.MON_DISTANCIA_REALIZADA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "KMPlanejados":
                    if (!select.Contains(" KMPlanejado, "))
                    {
                        select.Append("(select top 1 Pedido.PED_DISTANCIA from T_PEDIDO Pedido WHERE Pedido.PED_CODIGO = CargaPedido.PED_CODIGO)  KMPlanejados, ");

                        if (!groupBy.Contains("CargaPedido.PED_CODIGO"))
                            groupBy.Append("CargaPedido.PED_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "InicioViagem":
                    if (!select.Contains(" InicioViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM InicioViagem, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");
                    }
                    break;

                case "DataEntrega":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, ");
                    }
                    break;

                case "FuncionarioResponsavelEntrega":
                    if (!select.Contains(" FuncionarioResponsavelEntrega, "))
                    {
                        select.Append("(select FUN_NOME from T_FUNCIONARIO WHERE FUN_CODIGO = CargaEntrega.FUN_RESPONSAVEL_FINALIZACAO_MANUAL ) FuncionarioResponsavelEntrega, ");
                        groupBy.Append("CargaEntrega.FUN_RESPONSAVEL_FINALIZACAO_MANUAL, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            joins.Insert(0, @"
                join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = CargaEntregaChecklist.CEN_CODIGO
                join T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO "
            );

            where.Append($" and Carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}) ");
            where.Append("  and Carga.CAR_CARGA_FECHADA = 1 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%'");
                else
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");

            if (filtrosPesquisa.CodigoCheckListTipo > 0)
                where.Append($" and CargaEntregaChecklist.CLT_CODIGO = {filtrosPesquisa.CodigoCheckListTipo} ");

            if (filtrosPesquisa.DataCarregamentoInicial.HasValue)
            {
                where.Append($" and CAST(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO AS DATE) >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(pattern)}'");
                SetarJoinsCargaJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue)
            {
                where.Append($" and CAST(CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO AS DATE) <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(pattern)}'");
                SetarJoinsCargaJanelaCarregamento(joins);
            }
            if (filtrosPesquisa.Filiais.Contains(-1))
            {
                where.Append($@" and ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)})  OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )");
            }
            else if (filtrosPesquisa.Filiais.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.CodigoFilial)}) ");
            else if (filtrosPesquisa.CodigoFilial > 0)
                where.Append($" and Carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.CodigosMotoristas.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in ( ");
                where.Append("         select _cargamotorista.CAR_CODIGO ");
                where.Append("           from T_CARGA_MOTORISTA _cargamotorista ");
                where.Append($"         where _cargamotorista.CAR_MOTORISTA in ({string.Join(", ", filtrosPesquisa.CodigosMotoristas)}) ");
                where.Append("     )");
            }

            if (filtrosPesquisa.DataCargaInicial.HasValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCargaInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCargaFinal.HasValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataCargaFinal.Value.ToString(pattern)}'");
        }

        #endregion
    }
}
