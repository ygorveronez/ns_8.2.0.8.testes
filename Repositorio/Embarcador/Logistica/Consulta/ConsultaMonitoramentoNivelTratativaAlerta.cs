using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoTratativaAlerta : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTratativaAlerta>
    {
        #region Construtores

        public ConsultaMonitoramentoTratativaAlerta() : base(tabela: "T_ALERTA_TRATATIVA as TratativaAlerta") { }

        #endregion

        #region Métodos Privados 


        private void SetarJoinsAlertaMonitor(StringBuilder joins)
        {
            if (!joins.Contains(" AlertaMonitor "))
                joins.Append("left join T_Alerta_Monitor AlertaMonitor on AlertaMonitor.ALE_CODIGO = TratativaAlerta.ALE_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = AlertaMonitor.VEI_CODIGO ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append("left join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = TratativaAlerta.FUN_CODIGO ");
        }

        private void SetarJoinsTratativaAcao(StringBuilder joins)
        {
            if (!joins.Contains(" TratativaAcao "))
                joins.Append("left join T_ALERTA_TRATATIVA_ACAO TratativaAcao on TratativaAcao.ATC_CODIGO = TratativaAlerta.ATC_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_Carga Carga on Carga.CAR_CODIGO = AlertaMonitor.CAR_CODIGO ");
        }



        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTratativaAlerta filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("TratativaAlerta.ATA_CODIGO as Codigo, ");
                        groupBy.Append("TratativaAlerta.ATA_CODIGO, ");
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");
                        SetarJoinsAlertaMonitor(joins);
                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DataAlertaFormatado":
                    if (!select.Contains(" DataAlerta, "))
                    {
                        select.Append("AlertaMonitor.ALE_DATA as DataAlerta, ");
                        groupBy.Append("AlertaMonitor.ALE_DATA, ");

                        SetarJoinsAlertaMonitor(joins);
                    }
                    break;

                case "DataTratativaFormatada":
                    if (!select.Contains(" DataTratativa, "))
                    {
                        select.Append("TratativaAlerta.ATA_DATA as DataTratativa, ");
                        groupBy.Append("TratativaAlerta.ATA_DATA, ");
                    }
                    break;

                case "Tratativa":
                    if (!select.Contains(" Tratativa, "))
                    {
                        select.Append("TratativaAcao.ATC_DESCRICAO as Tratativa, ");
                        groupBy.Append("TratativaAcao.ATC_DESCRICAO, ");

                        SetarJoinsTratativaAcao(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("TratativaAlerta.ATA_OBSERVACAO as Observacao, ");
                        groupBy.Append("TratativaAlerta.ATA_OBSERVACAO, ");
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Funcionario.FUN_NOME as Usuario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;
            }

        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTratativaAlerta filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoCargaEmbarcador != "")
            {
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");
                SetarJoinsAlertaMonitor(joins);
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.PlacaVeiculo != "")
            {
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.PlacaVeiculo}%'");

                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
            {
                where.Append($" and TratativaAlerta.ATA_DATA >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            }

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                where.Append($" and TratativaAlerta.ATA_DATA <= '{filtrosPesquisa.DataFinal.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            }

            if (filtrosPesquisa.Filiais.Contains(-1))
            {
                where.Append($@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(  SELECT _cargaPedidoRecebedor.CAR_CODIGO 
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

