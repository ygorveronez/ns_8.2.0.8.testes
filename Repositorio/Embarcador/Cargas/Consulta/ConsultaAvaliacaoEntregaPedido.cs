using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaAvaliacaoEntregaPedido : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido>
    {
        #region Construtores

        public ConsultaAvaliacaoEntregaPedido() : base(tabela: "T_CARGA_ENTREGA AS CargaEntrega") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");
        }

        private void SetarJoinsMotivoAvaliacao(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoAvaliacao "))
                joins.Append(" LEFT JOIN T_MOTIVO_AVALIACAO MotivoAvaliacao ON MotivoAvaliacao.TMA_CODIGO = CargaEntrega.TMA_CODIGO ");
        }


        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CargaEntrega.CEN_CODIGO Codigo, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("(");
                        select.Append("    SELECT TOP 1 _pedido.PED_NUMERO_PEDIDO_EMBARCADOR FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append("    JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO");
                        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append(") NumeroPedido,");
                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "DataAvaliacao":
                    if (!select.Contains(" DataAvaliacao, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_AVALIACAO DataAvaliacao, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_AVALIACAO,"))
                            groupBy.Append("CargaEntrega.CEN_DATA_AVALIACAO, ");
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append(" SUBSTRING((SELECT', ' + convert(varchar,NotaFiscal.NF_NUMERO) + (CASE WHEN NotaFiscal.NF_SERIE IS NULL OR NotaFiscal.NF_SERIE = '' THEN '' ELSE '-' END) + NotaFiscal.NF_SERIE  AS [text()]");
                        select.Append(" FROM t_xml_nota_fiscal NotaFiscal");
                        select.Append(" JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO");
                        select.Append(" JOIN T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido ON PedidoNotaFiscal.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append(" JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append(" WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append(" ORDER BY NotaFiscal.NF_NUMERO");
                        select.Append(" FOR XML PATH ('')), 3, 2000) NotasFiscais,");
                    }
                   
                    break;

                case "Feedback":
                    if (!select.Contains(" Feedback, "))
                    {
                        select.Append("CargaEntrega.CEN_OBSERVACAO_AVALIACAO Feedback, ");
                        groupBy.Append("CargaEntrega.CEN_OBSERVACAO_AVALIACAO, ");
                    }
                    break;

                case "Motivo":
                    if (!select.Contains(" Motivo, "))
                    {
                        select.Append("MotivoAvaliacao.TMA_DESCRICAO Motivo, ");
                        groupBy.Append("MotivoAvaliacao.TMA_DESCRICAO, ");

                        SetarJoinsMotivoAvaliacao(joins);
                    }
                    break;

                case "TransportadorFormatado":
                    if (!select.Contains(" TransportadorRazao, "))
                    {
                        select.Append("Empresa.EMP_RAZAO TransportadorRazao, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");
                    }
                    if (!select.Contains(" TransportadorCNPJ, "))
                    {
                        select.Append("Empresa.EMP_CNPJ TransportadorCNPJ, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");
                    }

                    SetarJoinsEmpresa(joins);
                    break;

                case "DestinatarioFormatado":
                    if (!select.Contains(" DestinatarioNome, "))
                    {
                        select.Append("Cliente.CLI_NOME DestinatarioNome, ");
                        groupBy.Append("Cliente.CLI_NOME, ");
                    }
                    if (!select.Contains(" DestinatarioTipo, "))
                    {
                        select.Append("Cliente.CLI_FISJUR DestinatarioTipo, ");
                        groupBy.Append("Cliente.CLI_FISJUR, ");
                    }
                    if (!select.Contains(" DestinatarioCNPJ, "))
                    {
                        select.Append("Cliente.CLI_CGCCPF DestinatarioCNPJ, ");
                        groupBy.Append("Cliente.CLI_CGCCPF, ");
                    }

                    SetarJoinsCliente(joins);
                    break;

                case "Placas":
                    if (!select.Contains(" Placas, "))
                    {
                        select.Append("(");
                        select.Append("    (SELECT _veiculo.VEI_PLACA FROM T_VEICULO _veiculo WHERE _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) +");
                        select.Append("    ISNULL((");
                        select.Append("        SELECT ', ' + _veiculo.VEI_PLACA");
                        select.Append("        FROM T_CARGA_VEICULOS_VINCULADOS _veiculoVinculadoCarga");
                        select.Append("        JOIN T_VEICULO _veiculo ON _veiculoVinculadoCarga.VEI_CODIGO = _veiculo.VEI_CODIGO");
                        select.Append("        WHERE _veiculoVinculadoCarga.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append("        FOR XML PATH('')");
                        select.Append("    ), '')");
                        select.Append(") Placas,");

                        groupBy.Append("Carga.CAR_CODIGO, Carga.CAR_VEICULO, ");
                    }

                    SetarJoinsCarga(joins);
                    break;

                default:
                    if (propriedade.Length == 0)
                        return;
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append($" AND CargaEntrega.CEN_DATA_AVALIACAO IS NOT NULL ");

            if (filtrosPesquisa.DataAvaliacaoInicial.HasValue)
                where.Append($" AND CAST(CargaEntrega.CEN_DATA_AVALIACAO AS DATE) >= '{filtrosPesquisa.DataAvaliacaoInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataAvaliacaoFinal.HasValue)
                where.Append($" AND CAST(CargaEntrega.CEN_DATA_AVALIACAO AS DATE) <= '{filtrosPesquisa.DataAvaliacaoFinal.Value.ToString(pattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosTransportadores.Count > 0)
            {
                where.Append($" AND Carga.EMP_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosTransportadores)})");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosMotivos.Count > 0)
                where.Append($" AND CargaEntrega.TMA_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosMotivos)})");

            if (filtrosPesquisa.CnpjsDestinatarios.Count > 0)
            {
                where.Append($" AND CargaEntrega.CLI_CODIGO_ENTREGA IN ({string.Join(",", filtrosPesquisa.CnpjsDestinatarios)})");
            }

            if (filtrosPesquisa.CodigosVeiculos.Count > 0)
            {
                StringBuilder selectVeiculos = new StringBuilder();
                selectVeiculos.Append(" SELECT _veiculoVinculadoCarga.CAR_CODIGO");
                selectVeiculos.Append(" FROM T_CARGA_VEICULOS_VINCULADOS _veiculoVinculadoCarga");
                selectVeiculos.Append(" WHERE _veiculoVinculadoCarga.CAR_CODIGO = Carga.CAR_CODIGO");
                selectVeiculos.Append($" AND _veiculoVinculadoCarga.VEI_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosVeiculos)})");

                where.Append($" and (Carga.CAR_VEICULO IN ({string.Join(",", filtrosPesquisa.CodigosVeiculos)}) OR Carga.CAR_CODIGO IN ({selectVeiculos}))");
                SetarJoinsCarga(joins);
            }
        }

        #endregion
    }
}
