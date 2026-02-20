using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoNivelServico : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico>
    {
        #region Construtores

        public ConsultaMonitoramentoNivelServico() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append("join T_CARGA_ENTREGA CargaEntrega on Carga.Car_Codigo = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append("left join T_CLIENTE Cliente on CargaEntrega.CLI_CODIGO_ENTREGA = Cliente.CLI_CGCCPF ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        private void SetarJoinsLocalidadeLoja(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" LocalidadeDestino "))
                joins.Append("left join T_LOCALIDADES LocalidadeDestino on Cliente.LOC_CODIGO = LocalidadeDestino.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeOrigem(StringBuilder joins)
        {
            SetarJoinsOrigem(joins);

            if (!joins.Contains(" LocalidadeOrigem "))
                joins.Append("left join T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = Origem.LOC_CODIGO ");
        }

        private void SetarJoinsModeloCarroceria(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloCarroceria "))
                joins.Append("left join T_MODELO_CARROCERIA ModeloCarroceria on ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicular on Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append("join T_MONITORAMENTO Monitoramento on Carga.Car_Codigo = Monitoramento.CAR_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            SetarJoinsRota(joins);

            if (!joins.Contains(" Origem "))
                joins.Append("left join T_CLIENTE Origem on Origem.CLI_CGCCPF = Rota.CLI_CGCCPF ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append("left join T_ROTA_FRETE Rota on Carga.ROF_CODIGO = Rota.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append("left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsClienteDescarga(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" ClienteDescarga "))
                joins.Append("left join T_CLIENTE_DESCARGA ClienteDescarga on ClienteDescarga.CLI_CGCCPF = Cliente.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                        select.Append("Carga.CAR_CODIGO as Codigo, ");
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                    break;

                case "DataDaCargaFormatada":
                    if (!select.Contains(" DataDaCarga, "))
                        select.Append("isnull(Carga.CAR_DATA_CARREGAMENTO, Carga.CAR_DATA_CRIACAO) as DataDaCarga, ");
                    break;

                case "SM":
                    if (!select.Contains(" SM, "))
                    {
                        select.Append("Monitoramento.MON_CODIGO as SM, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaVeiculo, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO as ModeloVeicular, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "DescricaoTipoRodado":
                    if (!select.Contains(" TipoRodado, "))
                    {
                        select.Append("Veiculo.VEI_TIPORODADO as TipoRodado, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "CD":
                    if (!select.Contains(" CD, "))
                    {
                        select.Append("Origem.CLI_NOME CD, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "CDCODIGO":
                    if (!select.Contains(" CDCODIGO, "))
                    {
                        select.Append("Origem.CLI_CODIGO_INTEGRACAO CDCodigo, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "CDDescricao":
                    {
                        if (!select.Contains(" CD, "))
                        {
                            select.Append("Origem.CLI_NOME CD, ");

                            SetarJoinsOrigem(joins);
                        }

                        if (!select.Contains(" CDCODIGO, "))
                        {
                            select.Append("Origem.CLI_CODIGO_INTEGRACAO CDCodigo, ");

                            SetarJoinsOrigem(joins);
                        }
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("LocalidadeOrigem.UF_SIGLA as UFOrigem, ");

                        SetarJoinsLocalidadeOrigem(joins);
                    }
                    break;

                case "Regiao":
                    if (!select.Contains(" Regiao, "))
                    {
                        //verificar
                    }
                    break;

                case "DataSaidaCDFormatada":
                    if (!select.Contains(" DataSaidaCD, "))
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM as DataSaidaCD, ");
                    break;

                case "Loja":
                    if (!select.Contains(" Loja, "))
                    {
                        select.Append("Cliente.CLI_NOME Loja, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "LojaCodigo":
                    if (!select.Contains(" LojaCodigo, "))
                    {
                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO LojaCodigo, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "LojaDescricao":
                    if (!select.Contains(" Loja, "))
                    {
                        select.Append("Cliente.CLI_NOME Loja, ");

                        SetarJoinsCliente(joins);
                    }

                    if (!select.Contains(" LojaCodigo, "))
                    {
                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO LojaCodigo, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("LocalidadeDestino.UF_SIGLA UFDestino, ");

                        SetarJoinsLocalidadeLoja(joins);
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

                case "TipoDeTransporte":
                    if (!select.Contains(" TipoDeTransporte, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoDeTransporte, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "NFS":
                    if (!select.Contains(" NFS, "))
                    {
                        select.Append("(select count(1) from T_PEDIDO_XML_NOTA_FISCAL Nota join T_CARGA_PEDIDO CargaPedido on Nota.CPE_CODIGO = CargaPedido.CPE_CODIGO and CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) NFS, ");
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_MOTORISTAS NomeMotorista, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "JanelaDescarga":
                    if (!select.Contains(" JanelaDescarga, "))
                    {
                        select.Append("ClienteDescarga.CLD_HORA_LIMETE_DESCARGA JanelaDescarga, ");

                        SetarJoinsClienteDescarga(joins);
                    }
                    break;

                case "NumeroTransporte":
                    if (!select.Contains("  NumeroTransporte, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + cast(nfx.NF_NUMERO_TRANSPORTE as nvarchar(20)) ");
                        select.Append("      from T_CARGA car ");
                        select.Append("      left join T_CARGA_PEDIDO cpe on cpe.CAR_CODIGO = car.CAR_CODIGO ");
                        select.Append("      left join T_PEDIDO_XML_NOTA_FISCAL pex on pex.CPE_CODIGO = cpe.CPE_CODIGO ");
                        select.Append("      left join T_XML_NOTA_FISCAL nfx on nfx.NFX_CODIGO = pex.NFX_CODIGO ");
                        select.Append("     where car.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroTransporte, ");

                    }
                    break;

                case "DataConfirmacaoDocumentoFormatada":
                    if (!select.Contains(" DataConfirmacaoDocumento, "))
                        select.Append("Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS as DataConfirmacaoDocumento, ");
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsMonitoramento(joins);
            SetarJoinsCargaEntrega(joins);
            SetarJoinsVeiculo(joins);

            where.Append($" and CargaEntrega.CEN_DATA_INICIO_ENTREGA is not null and CargaEntrega.CEN_DATA_ENTREGA is not null");

            if (filtrosPesquisa.CodigoCargaEmbarcador != "")
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%'");
                else
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");
            }

            if (filtrosPesquisa.PlacaVeiculo != "")
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.PlacaVeiculo}%'");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(Monitoramento.MON_DATA_INICIO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(Monitoramento.MON_DATA_FIM AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataConfirmacaoDocumentosInicial != DateTime.MinValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) >= '{filtrosPesquisa.DataConfirmacaoDocumentosInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataConfirmacaoDocumentosFinal != DateTime.MinValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) <= '{filtrosPesquisa.DataConfirmacaoDocumentosFinal.ToString(pattern)}'");

            if (filtrosPesquisa.Filiais.Contains(-1))
                where.Append($@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(  SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )");
            else if (filtrosPesquisa.Filiais.Count > 0)
                where.Append($@" AND Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)})");
        }

        #endregion
    }
}

