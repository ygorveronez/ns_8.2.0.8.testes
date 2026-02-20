using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    sealed class ConsultaRotaControleEntrega : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega>
    {
        #region Construtores

        public ConsultaRotaControleEntrega() : base(tabela: "T_CARGA as CARGA") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = CARGA.FIL_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CARGA.CAR_VEICULO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = CARGA.EMP_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = CARGA.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
            {
                SetarJoinsCargaPedido(joins);
                joins.Append(" left join T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
            }
        }

        private void SetarJoinsEmitente(StringBuilder joins)
        {
            if (!joins.Contains(" Emitente "))
            {
                SetarJoinsPedido(joins);
                joins.Append(" left join T_CLIENTE Emitente ON Emitente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
            }
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
            {
                SetarJoinsPedido(joins);
                joins.Append(" left join T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
            }
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = CARGA.CDS_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CARGA.CAR_CODIGO as Codigo, ");
                        groupBy.Append("CARGA.CAR_CODIGO, ");
                    }
                    break;

                case "DescricaoFilial":
                    if (!select.Contains(" DescricaoFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO as DescricaoFilial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("CARGA.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                        groupBy.Append("CARGA.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "VeiculoPlaca":
                    if (!select.Contains(" VeiculoPlaca, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as VeiculoPlaca, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "EntregasRealizadas":
                    if (!select.Contains(" EntregasRealizadas, "))
                    {
                        select.Append(@"(SELECT COUNT(_cargaEntrega.CEN_CODIGO) FROM T_CARGA_ENTREGA _cargaEntrega 
                                        WHERE _cargaEntrega.CEN_COLETA = 0
                                        AND _cargaEntrega.CEN_DATA_ENTREGA is not null
                                        AND _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO) EntregasRealizadas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PesoEntregue":
                    if (!select.Contains(" PesoEntregue, "))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_cargapedido.PED_PESO) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("     where _cargapedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") PesoEntregue, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "KMRealizados":
                    if (!select.Contains(" KMRealizados, "))
                    {
                        select.Append("SUM(DadosSumarizados.CDS_DISTANCIA) KMRealizados, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "StatusFormatada":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("Carga.CAR_SITUACAO Status, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");
                    }
                    break;

                case "FimRealizadoFormatada":
                    if (!select.Contains(" FimRealizado, "))
                    {
                        select.Append(@"(SELECT MAX(_cargaEntrega.CEN_DATA_ENTREGA)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) FimRealizado, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "InicioRealizadoFormatada":
                    if (!select.Contains(" InicioRealizado, "))
                    {
                        select.Append(@"(SELECT MIN(_cargaEntrega.CEN_DATA_CRIACAO)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) InicioRealizado, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ChegadaRealizadaFormatada":
                    if (!select.Contains(" ChegadaRealizada, "))
                    {
                        select.Append(@"(SELECT MIN(_cargaEntrega.CEN_DATA_ENTRADA_RAIO)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) ChegadaRealizada, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SaidaRealizadaFormatada":
                    if (!select.Contains(" SaidaRealizada, "))
                    {
                        select.Append(@"(SELECT MAX(_cargaEntrega.CEN_DATA_SAIDA_RAIO)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) SaidaRealizada, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "InicioDescargaFormatada":
                    if (!select.Contains(" InicioDescarga, "))
                    {
                        select.Append(@"(SELECT MIN(_cargaEntrega.CEN_DATA_INICIO_JANELA)
                                        FROM T_CARGA_ENTREGA _cargaEntrega 
                                        inner join T_CARGA _carga on _carga.CAR_CODIGO = _cargaEntrega.CAR_CODIGO
                                        where _carga.CAR_CODIGO = Carga.CAR_CODIGO) InicioDescarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "PesoDevolvido":
                    break;

                case "Reentregas":
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";
            string patternDataHora = "yyyy-MM-dd HH:mm:ss";

            where.Append($" AND Carga.CAR_CARGA_FECHADA = 1 ");
            where.Append($" AND (Carga.CAR_SITUACAO NOT IN (13, 18) OR Carga.CAR_SITUACAO IS NULL) ");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.CodigosFilial.Contains(-1))
            {
                where.Append($@" and ( Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ))");

                SetarJoinsFilial(joins);
            }
            else if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                SetarJoinsFilial(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.CpfCnpjEmitentes?.Count > 0)
            {
                where.Append($" and Pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.CpfCnpjEmitentes)})");
                SetarJoinsEmitente(joins);
            }

            if (filtrosPesquisa.CpfCnpjDestinatarios?.Count > 0)
            {
                where.Append($" and Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CpfCnpjDestinatarios)})");
                SetarJoinsDestinatario(joins);
            }

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
            {
                where.Append($" and Transportador.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");
                SetarJoinsTransportador(joins);
            }

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)})");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.NumerosPedido?.Count > 0)
            {
                where.Append($" and Pedido.PED_CODIGO in ({string.Join(", ", filtrosPesquisa.NumerosPedido)})");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargapedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargapedido");
                where.Append("           inner join T_PEDIDO_XML_NOTA_FISCAL _pex on _pex.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                where.Append("           inner join T_XML_NOTA_FISCAL _nfx on _nfx.NFX_CODIGO = _pex.NFX_CODIGO ");
                where.Append($"         where _nfx.NF_NUMERO in ({string.Join(", ", filtrosPesquisa.NumeroNotasFiscais)})");
                where.Append("     ) ");
            }

            StringBuilder wherePedido = new StringBuilder();

            if (filtrosPesquisa.DataEntregaPedidoInicial != DateTime.MinValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_ENTREGA AS DATE) >= '{filtrosPesquisa.DataEntregaPedidoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue)
                wherePedido.Append($" and CAST(_pedido.PED_DATA_ENTREGA AS DATE) <= '{filtrosPesquisa.DataEntregaPedidoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataPrevisaoEntregaPedidoInicial != DateTime.MinValue)
                wherePedido.Append($" and _cargapedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataPrevisaoEntregaPedidoInicial.ToString(patternDataHora)}'");

            if (filtrosPesquisa.DataPrevisaoEntregaPedidoFinal != DateTime.MinValue)
                wherePedido.Append($" and _cargapedido.PED_PREVISAO_ENTREGA <= '{filtrosPesquisa.DataPrevisaoEntregaPedidoFinal.ToString(patternDataHora)}'");

            if (filtrosPesquisa.NumerosPedido?.Count > 0)
                wherePedido.Append($" and _pedido.PED_CODIGO in ({string.Join(", ", filtrosPesquisa.NumerosPedido)})");

            if (wherePedido.Length > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargapedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargapedido");
                where.Append("           join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                where.Append($"         where {wherePedido.ToString().Trim().Substring(3)} ");
                where.Append("     ) ");
            }

        }

        #endregion
    }
}
