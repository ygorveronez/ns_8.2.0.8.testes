using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaHistoricoVinculo : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo>
    {
        #region Construtores

        public ConsultaHistoricoVinculo() : base(tabela: "T_HISTORICO_VINCULO as HistoricoVinculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsHistoricoReboque(StringBuilder joins)
        {
            if (!joins.Contains(" HistoricoReboque "))
                joins.Append(" LEFT JOIN T_HISTORICO_VINCULO_REBOQUES HistoricoReboque ON HistoricoReboque.THV_CODIGO = HistoricoVinculo.THV_CODIGO ");
        }

        private void SetarJoinsHistoricoMotoristas(StringBuilder joins)
        {
            if (!joins.Contains(" HistoricoMotoristas "))
                joins.Append(" LEFT JOIN T_HISTORICO_VINCULO_MOTORISTAS HistoricoMotoristas ON HistoricoMotoristas.THV_CODIGO = HistoricoVinculo.THV_CODIGO ");
        }

        private void SetarJoinsVeiculoTracao(StringBuilder joins)
        {
            if (!joins.Contains(" VeiculoTracao "))
                joins.Append(" LEFT JOIN T_VEICULO VeiculoTracao ON VeiculoTracao.VEI_CODIGO = HistoricoVinculo.VEI_CODIGO_TRACAO ");
        }

        private void SetarJoinsVeiculoReboque(StringBuilder joins)
        {
            SetarJoinsHistoricoReboque(joins);
            if (!joins.Contains(" VeiculoReboque "))
                joins.Append(" LEFT JOIN T_VEICULO VeiculoReboque ON VeiculoReboque.VEI_CODIGO = HistoricoReboque.VEI_CODIGO ");
        }

        private void SetarJoinsMotoristas(StringBuilder joins)
        {
            SetarJoinsHistoricoMotoristas(joins);
            if (!joins.Contains(" Motorista "))
                joins.Append(" LEFT JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = HistoricoMotoristas.FUN_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append(" LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = HistoricoVinculo.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = HistoricoVinculo.CAR_CODIGO ");
        }

        private void SetarJoinsFilaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" FilaCarregamento "))
                joins.Append(" LEFT JOIN T_FILA_CARREGAMENTO_VEICULO FilaCarregamento ON FilaCarregamento.FLV_CODIGO = HistoricoVinculo.FLV_CODIGO ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "VeiculoTracao":
                    if (!select.Contains(" VeiculoTracao, "))
                    {
                        select.Append("VeiculoTracao.VEI_PLACA as VeiculoTracao, ");
                        groupBy.Append("VeiculoTracao.VEI_PLACA, ");

                    }
                    SetarJoinsVeiculoTracao(joins);

                    break;

                case "VeiculoReboque":
                    if (!select.Contains(" VeiculoReboque, "))
                    {
                        select.Append("STRING_AGG(VeiculoReboque.VEI_PLACA, ', ') as VeiculoReboque, ");

                    }
                    SetarJoinsVeiculoReboque(joins);

                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("STRING_AGG(Motorista.FUN_NOME, ', ') as Motorista, ");
                    }
                    SetarJoinsMotoristas(joins);

                    break;

                case "LocalVinculo":
                case "LocalVinculoFormatada":
                    if (!select.Contains(" LocalVinculo, "))
                    {
                        select.Append("HistoricoVinculo.THV_LOCAL_VINCULO as LocalVinculo, ");
                        groupBy.Append("HistoricoVinculo.THV_LOCAL_VINCULO, ");
                    };

                    break;

                case "DataVinculo":
                case "DataVinculoFormatada":
                    if (!select.Contains(" DataVinculo, "))
                    {
                        select.Append("HistoricoVinculo.THV_DATA_HORA_VINCULO as DataVinculo, ");
                        groupBy.Append("HistoricoVinculo.THV_DATA_HORA_VINCULO, ");
                    }

                    break;

                case "DataDesvinculo":
                case "DataDesvinculoFormatada":
                    if (!select.Contains(" DataDesvinculo, "))
                    {
                        select.Append("HistoricoVinculo.THV_DATA_HORA_DESVINCULO as DataDesvinculo, ");
                        groupBy.Append("HistoricoVinculo.THV_DATA_HORA_DESVINCULO, ");
                    }

                    break;

                case "Pedido":
                    if (!select.Contains(" Pedido, "))
                    {
                        select.Append("CASE WHEN Pedido.PED_NUMERO_PEDIDO_EMBARCADOR <> '' THEN Pedido.PED_NUMERO_PEDIDO_EMBARCADOR "+
                                           "ELSE CAST(Pedido.PED_NUMERO AS VARCHAR) "+
                                      "END AS Pedido, ");

                        if(!groupBy.Contains("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR"))
                            groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        if (!groupBy.Contains("Pedido.PED_NUMERO,"))
                            groupBy.Append("Pedido.PED_NUMERO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    SetarJoinsCarga(joins);

                    break;

                case "FilaCarregamento":
                    if (!select.Contains(" FilaCarregamento, "))
                    {
                        select.Append("cast(FilaCarregamento.FLV_CODIGO as nvarchar) as FilaCarregamento, ");
                        groupBy.Append("FilaCarregamento.FLV_CODIGO, ");
                    }
                    SetarJoinsFilaCarregamento(joins);

                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("HistoricoVinculo.THV_OBSERVACAO as Observacao, ");
                        groupBy.Append("HistoricoVinculo.THV_OBSERVACAO, ");
                    }

                    break;

                default:
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.Veiculo > 0)
            {
                where.Append($" AND (VeiculoTracao.VEI_CODIGO = {filtrosPesquisa.Veiculo}" +
                                  $" OR VeiculoReboque.VEI_CODIGO = {filtrosPesquisa.Veiculo} ) ");
                SetarJoinsVeiculoTracao(joins);
                SetarJoinsVeiculoReboque(joins);
            }

            if (filtrosPesquisa.Motorista > 0)
            {
                where.Append($" AND Motorista.FUN_CODIGO = {filtrosPesquisa.Motorista} ");
                SetarJoinsMotoristas(joins);
            }

            if (filtrosPesquisa.LocalVinculo.HasValue && ((int)filtrosPesquisa.LocalVinculo > 0))
            {
                where.Append($" AND HistoricoVinculo.THV_LOCAL_VINCULO = {(int)filtrosPesquisa.LocalVinculo} ");
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                where.Append($" AND (HistoricoVinculo.THV_DATA_HORA_VINCULO >= '{filtrosPesquisa.DataInicial.ToString(pattern)} 00:00:00' " +
                                   $" OR HistoricoVinculo.THV_DATA_HORA_DESVINCULO >= '{filtrosPesquisa.DataInicial.ToString(pattern)} 00:00:00' ) ");
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                where.Append($" AND (HistoricoVinculo.THV_DATA_HORA_VINCULO <= '{filtrosPesquisa.DataInicial.ToString(pattern)} 23:59:59' " +
                                   $" OR HistoricoVinculo.THV_DATA_HORA_DESVINCULO <= '{filtrosPesquisa.DataInicial.ToString(pattern)} 23:59:59' ) ");
            }

            if (filtrosPesquisa.Pedido > 0)
            {
                where.Append($" AND Pedido.PED_CODIGO = '{filtrosPesquisa.Pedido}' ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.Carga > 0)
            {
                where.Append($" AND Carga.CAR_CODIGO = '{filtrosPesquisa.Carga}' ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.FilaCarregamento > 0)
            {
                where.Append($" AND FilaCarregamento.FLV_CODIGO = '{filtrosPesquisa.FilaCarregamento}' ");
                SetarJoinsFilaCarregamento(joins);
            }

        }

        #endregion
    }
}
