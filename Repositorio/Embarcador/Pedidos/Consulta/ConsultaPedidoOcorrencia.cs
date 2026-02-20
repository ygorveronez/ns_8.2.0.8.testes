using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaPedidoOcorrencia : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia>
    {
        #region Construtores

        public ConsultaPedidoOcorrencia() : base(tabela: "T_PEDIDO_OCORRENCIA_COLETA_ENTREGA AS PedidoOcorrencia") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append("left join T_PEDIDO Pedido on Pedido.PED_CODIGO = PedidoOcorrencia.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = PedidoOcorrencia.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" TipoDeOcorrencia "))
                joins.Append(" left join T_OCORRENCIA TipoDeOcorrencia ON TipoDeOcorrencia.OCO_CODIGO = PedidoOcorrencia.OCO_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados ON CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = PedidoOcorrencia.CAR_CODIGO AND CargaPedido.PED_CODIGO = PedidoOcorrencia.PED_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CAR_CODIGO = PedidoOcorrencia.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 0 ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CargaEntregaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido ON CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO AND CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa)
        {
            SetarJoinsPedido(joins);
            SetarJoinsCarga(joins);

            switch (propriedade)
            {
                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append("TipoDeOcorrencia.OCO_DESCRICAO TipoOcorrencia, ");
                        groupBy.Append("TipoDeOcorrencia.OCO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);
                    }
                    break;

                case "DataOcorrencia":
                case "DataOcorrenciaFormatada":
                    if (!select.Contains(" DataOcorrencia, "))
                    {
                        select.Append("PedidoOcorrencia.POC_DATA_OCORRENCIA DataOcorrencia, ");
                        groupBy.Append("PedidoOcorrencia.POC_DATA_OCORRENCIA, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("PedidoOcorrencia.POC_OBSERVACAO Observacao, ");
                        groupBy.Append("PedidoOcorrencia.POC_OBSERVACAO, ");
                    }
                    break;

                //case "Remetente":
                //    if (!select.Contains(" Remetente, "))
                //    {
                //        select.Append("CargaDadosSumarizados.CDS_REMETENTES Remetente, ");
                //        groupBy.Append("CargaDadosSumarizados.CDS_REMETENTES, ");

                //        SetarJoinsCargaDadosSumarizados(joins);
                //    }
                //    break;

                //case "Destinatario":
                //    if (!select.Contains(" Destinatario, "))
                //    {
                //        select.Append("CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario, ");
                //        groupBy.Append("CargaDadosSumarizados.CDS_DESTINATARIOS, ");

                //        SetarJoinsCargaDadosSumarizados(joins);
                //    }
                //    break;

                case "Remetente":
                    SetarSelect("RemetenteNome", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("RemetenteCPFCNPJ", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "RemetenteNome":
                    if (!select.Contains(" RemetenteNome, "))
                    {
                        select.Append("Remetente.CLI_NOME RemetenteNome, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "RemetenteCPFCNPJ":
                    if (!select.Contains(" RemetenteCPFCNPJ, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF RemetenteCPFCNPJ, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Destinatario":
                    SetarSelect("DestinatarioNome", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("DestinatarioCPFCNPJ", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "DestinatarioNome":
                    if (!select.Contains(" DestinatarioNome, "))
                    {
                        select.Append("Destinatario.CLI_NOME DestinatarioNome, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "DestinatarioCPFCNPJ":
                    if (!select.Contains(" DestinatarioCPFCNPJ, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF DestinatarioCPFCNPJ, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append("SUBSTRING(( " +
                            "   SELECT ', ' + " +
                            "       CAST(NotaFiscal.NF_NUMERO AS VARCHAR(20))" +
                            "   FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal " +
                            "       INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO " +
                            "   WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO  " +
                            "   FOR XML PATH(''))," +
                            "3, 1000) NotasFiscais, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO"))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "DataPedidoFormatada":
                    if (!select.Contains(" DataPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");
                    }
                    break;

                case "Filial":
                    SetarSelect("RazaoFilial", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("CNPJFilial", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "RazaoFilial":
                    if (!select.Contains(" RazaoFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO RazaoFilial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Transportador":
                    SetarSelect("RazaoTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("CNPJTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "RazaoTransportador":
                    if (!select.Contains(" RazaoTransportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO RazaoTransportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST(( ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 1, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 4, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 7, 3) + '-' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 10, 3) + ' - ' + ");
                        select.Append("               Motorista.FUN_NOME ");
                        select.Append("           ) AS NVARCHAR(4000)) ");
                        select.Append("      FROM T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      JOIN T_FUNCIONARIO Motorista ");
                        select.Append("        ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Motoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                    }
                    break;

                case "Veiculo":
                    if(!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");

                        if (!groupBy.Contains("Veiculo.VEI_PLACA"))
                            groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "SituacaoEntrega":
                    if (!select.Contains(" SituacaoEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_SITUACAO SituacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_SITUACAO, ");

                        SetarJoinsCargaEntregaPedido(joins);
                    }
                    break;


                case "NaturezaOP":
                    if (!select.Contains(" NaturezaOP, "))
                    {
                        select.Append(
                                 @"(
                                    select top 1 _xmlnotafiscal.NF_NATUREZA_OP
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
		                             where _pedidoXml.PED_CODIGO = PedidoOcorrencia.PED_CODIGO
                                ) NaturezaOP, "
                            );

                        groupBy.Append("PedidoOcorrencia.PED_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
            {
                where.Append($" AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}' ");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataOcorrenciaInicial != DateTime.MinValue)
                where.Append($" AND PedidoOcorrencia.POC_DATA_OCORRENCIA >= '{filtrosPesquisa.DataOcorrenciaInicial.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataOcorrenciaFinal != DateTime.MinValue)
                where.Append($" AND PedidoOcorrencia.POC_DATA_OCORRENCIA < '{filtrosPesquisa.DataOcorrenciaFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.CodigoTipoOcorrencia > 0)
                where.Append($" AND PedidoOcorrencia.OCO_CODIGO = {filtrosPesquisa.CodigoTipoOcorrencia} ");

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosTransportadores?.Count > 0)
            {
                where.Append($" AND Carga.EMP_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosTransportadores)}) ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
            {
                where.Append($" AND Pedido.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjRemetente} ");
                SetarJoinsPedido(joins);
            } 

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosFiliais)})))");
                SetarJoinsCarga(joins);
            }
            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
            {
                where.Append($" AND Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoOrigem > 0)
            {
                where.Append($" AND Pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoOrigem} ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CodigoDestino > 0)
            {
                where.Append($" AND Pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoDestino} ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                where.Append(" AND EXISTS(" +
                    "   SELECT PNF_CODIGO " +
                    "   FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal " +
                    "       INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO " +
                    $"   WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO AND NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNotaFiscal} " +
                    ") ");
                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.SituacaoEntrega != null)
            {
                where.Append($" AND CargaEntrega.CEN_SITUACAO = {(int)filtrosPesquisa.SituacaoEntrega} ");
                SetarJoinsCargaEntregaPedido(joins);
            }
        }

        #endregion
    }
}
