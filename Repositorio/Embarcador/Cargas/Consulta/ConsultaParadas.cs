using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaParadas : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas>
    {
        #region Construtores

        public ConsultaParadas() : base(tabela: "T_CARGA_ENTREGA AS CargaEntrega") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");

            if (!joins.Contains(" CargaAgrupada "))
            {
                joins.Append(" LEFT JOIN T_CARGA CargaAgrupada ON CargaAgrupada.CAR_CODIGO = Carga.CAR_CODIGO_AGRUPAMENTO ");
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA AS CargaEntregaAgrupada ON CargaEntregaAgrupada.CAR_CODIGO = CargaAgrupada.CAR_CODIGO And CargaEntregaAgrupada.CLI_CODIGO_ENTREGA = CargaEntrega.CLI_CODIGO_ENTREGA ");
                joins.Append(" LEFT JOIN T_MONITORAMENTO AS MonitoramentoAgrupado ON MonitoramentoAgrupado.CAR_CODIGO = CargaAgrupada.CAR_CODIGO ");
            }
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");
        }

        private void SetarJoinsClienteOutroEndereco(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteOutroEndereco "))
                joins.Append(" LEFT JOIN T_CLIENTE_OUTRO_ENDERECO ClienteOutroEndereco ON ClienteOutroEndereco.COE_CODIGO = CargaEntrega.COE_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" LocalicadeCliente "))
                joins.Append(" JOIN T_LOCALIDADES LocalicadeCliente ON Cliente.LOC_CODIGO = LocalicadeCliente.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeOutroEndereco(StringBuilder joins)
        {
            SetarJoinsClienteOutroEndereco(joins);

            if (!joins.Contains(" LocalicadeClienteOutroEndereco "))
                joins.Append(" LEFT JOIN T_LOCALIDADES LocalicadeClienteOutroEndereco ON LocalicadeClienteOutroEndereco.LOC_CODIGO = ClienteOutroEndereco.LOC_CODIGO ");
        }

        private void SetarJoinsMotivoAvaliacao(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoAvaliacao "))
                joins.Append(" LEFT JOIN T_MOTIVO_AVALIACAO MotivoAvaliacao ON MotivoAvaliacao.TMA_CODIGO = CargaEntrega.TMA_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO Monitoramento ON Monitoramento.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsConfiguracaoTipoOperacaoControleEntrega(StringBuilder joins)
        {
            SetarJoinsTipoOperacao(joins);

            if (!joins.Contains(" ConfiguracaoTipoOperacaoControleEntrega "))
                joins.Append(" LEFT JOIN T_CONFIGURACAO_TIPO_OPERACAO_CONTROLE_ENTREGA ConfiguracaoTipoOperacaoControleEntrega ON ConfiguracaoTipoOperacaoControleEntrega.COE_CODIGO = TipoOperacao.COE_CODIGO ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntregaPedido "))
                joins.Append(" left join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsRemetentePedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" RemetentePedido "))
                joins.Append(" left join T_CLIENTE RemetentePedido on RemetentePedido.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsLocalidadeRemetente(StringBuilder joins)
        {
            SetarJoinsRemetentePedido(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeRemetente on LocalidadeRemetente.LOC_CODIGO = RemetentePedido.LOC_CODIGO ");
        }

        private void SetarJoinsCargaEntregaChecklistPergunta(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntregaChecklist "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA_CHECKLIST CargaEntregaChecklist ON CargaEntregaChecklist.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");

            if (!joins.Contains(" CargaEntregaChecklistPergunta "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA_CHECKLIST_PERGUNTA CargaEntregaChecklistPergunta ON CargaEntregaChecklistPergunta.CEC_CODIGO = CargaEntregaChecklist.CEC_CODIGO ");

        }

        private void SetarJoinsUltimaPosicao(StringBuilder joins)
        {

            if (!joins.Contains(" Posicao "))
                joins.Append(" LEFT JOIN T_POSICAO Posicao on Posicao.POS_CODIGO = ISNULL( MonitoramentoAgrupado.POS_ULTIMA_POSICAO, Monitoramento.POS_ULTIMA_POSICAO) ");
        }

        private void SetarJoinsMotivoRetificacao(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoRetificacao "))
                joins.Append(" LEFT JOIN T_MOTIVO_RETIFICACAO_COLETA MotivoRetificacao ON MotivoRetificacao.MRC_CODIGO = CargaEntrega.MRC_CODIGO ");
        }

        private void SetarJoinsPrimeiroPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" PrimeiroPedido "))
            {
                joins.Append(@" left join T_CARGA_PEDIDO CargaPedidoPrimeiro on CargaPedidoPrimeiro.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO 
                               left join T_PEDIDO PrimeiroPedido on PrimeiroPedido.PED_CODIGO = CargaPedidoPrimeiro.PED_CODIGO 
                               AND PrimeiroPedido.PED_CODIGO = (
                                   SELECT MIN(P.PED_CODIGO)
                                   FROM T_CARGA_ENTREGA_PEDIDO CEP2
                                   JOIN T_CARGA_PEDIDO CP2 ON CP2.CPE_CODIGO = CEP2.CPE_CODIGO
                                   JOIN T_PEDIDO P ON P.PED_CODIGO = CP2.PED_CODIGO
                                   WHERE CEP2.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                               ) ");
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtroPesquisa)
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

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataCriacaoCarga":
                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCriacaoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("CASE WHEN Carga.EMP_CODIGO IS NULL THEN '' ELSE CONCAT(Transportador.EMP_RAZAO, ' - (', Transportador.EMP_CNPJ, ')') END Transportador, ");
                        groupBy.Append("Carga.EMP_CODIGO, Transportador.EMP_RAZAO, Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT DISTINCT ', ' + CAST((");
                        select.Append("        SUBSTRING(_motorista.FUN_CPF, 1, 3) + '.' +");
                        select.Append("        SUBSTRING(_motorista.FUN_CPF, 4, 3) + '.' +");
                        select.Append("        SUBSTRING(_motorista.FUN_CPF, 7, 3) + '-' +");
                        select.Append("        SUBSTRING(_motorista.FUN_CPF, 10, 3) + ' - ' +");
                        select.Append("        _motorista.FUN_NOME");
                        select.Append("    ) AS NVARCHAR(4000))");
                        select.Append("    FROM T_CARGA_MOTORISTA _cargaMotorista");
                        select.Append("    JOIN T_FUNCIONARIO _motorista ON _cargaMotorista.CAR_MOTORISTA = _motorista.FUN_CODIGO");
                        select.Append("    WHERE _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append("    FOR XML PATH('')");
                        select.Append("), 3, 4000) Motoristas, ");

                        groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
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
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append(" ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");
                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "SituacaoEntrega":
                case "SituacaoEntregaDescricao":
                    if (!select.Contains(" SituacaoEntrega, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_SITUACAO, CargaEntrega.CEN_SITUACAO) SituacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_SITUACAO, CargaEntregaAgrupada.CEN_SITUACAO, ");
                    }
                    break;

                case "OrigemSituacaoEntrega":
                case "OrigemSituacaoEntregaDescricao":
                    if (!select.Contains(" OrigemSituacaoEntrega, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO, CargaEntrega.CEN_ORIGEM_SITUACAO) OrigemSituacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_ORIGEM_SITUACAO, CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO, ");
                    }
                    break;

                case "UsuarioFinalizador":
                    if (!select.Contains(" UsuarioFinalizador, "))
                    {
                        select.Append(@" CASE 
                                         WHEN CargaEntrega.CEN_ORIGEM_SITUACAO = 1 THEN (
                                            SELECT T_SETOR.SET_DESCRICAO
                                            FROM T_FUNCIONARIO
                                            left JOIN T_SETOR ON T_SETOR.SET_CODIGO = T_FUNCIONARIO.SET_CODIGO
                                            WHERE T_FUNCIONARIO.FUN_CODIGO = CargaEntrega.FUN_RESPONSAVEL_FINALIZACAO_MANUAL)
                                         WHEN CargaEntrega.CEN_ORIGEM_SITUACAO = 2 THEN 'Motorista'
                                         ELSE ''
                                         END AS UsuarioFinalizador, ");
                        groupBy.Append(" CargaEntrega.FUN_RESPONSAVEL_FINALIZACAO_MANUAL, ");

                        if (!select.Contains(" OrigemSituacaoEntrega, "))
                            groupBy.Append("CargaEntrega.CEN_ORIGEM_SITUACAO, ");

                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains(" NumeroPedidoCliente, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT ', ' + _pedido.PED_CODIGO_PEDIDO_CLIENTE");
                        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append("    JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO");
                        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append("        FOR XML PATH('')");
                        select.Append("), 3, 1000) NumeroPedidoCliente,");

                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "OrdemPrevista":
                    if (!select.Contains(" OrdemPrevista, "))
                    {
                        select.Append("CargaEntrega.CEN_ORDEM OrdemPrevista, ");
                        groupBy.Append("CargaEntrega.CEN_ORDEM, ");
                    }
                    break;

                case "OrdemExecutada":
                    if (!select.Contains(" OrdemExecutada, "))
                    {
                        select.Append("CargaEntrega.CEN_ORDEM_REALIZADA OrdemExecutada, ");
                        groupBy.Append("CargaEntrega.CEN_ORDEM_REALIZADA, ");
                    }
                    break;

                case "Aderencia":
                    if (!select.Contains(" Aderencia, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.CEN_ORDEM = CargaEntrega.CEN_ORDEM_REALIZADA THEN 1 ELSE 0 END Aderencia, ");
                        groupBy.Append("CargaEntrega.CEN_ORDEM, CargaEntrega.CEN_ORDEM_REALIZADA, ");
                    }
                    break;

                case "DataChegadaCliente":
                case "DataChegadaClienteFormatada":
                    if (!select.Contains(" DataChegadaCliente, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_ENTRADA_RAIO, CargaEntrega.CEN_DATA_ENTRADA_RAIO) DataChegadaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO, CargaEntregaAgrupada.CEN_DATA_ENTRADA_RAIO, ");
                    }
                    break;

                case "DataSaidaCliente":
                case "DataSaidaClienteFormatada":
                    if (!select.Contains(" DataSaidaCliente, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_SAIDA_RAIO, CargaEntrega.CEN_DATA_SAIDA_RAIO) DataSaidaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_SAIDA_RAIO, CargaEntregaAgrupada.CEN_DATA_SAIDA_RAIO, ");
                    }
                    break;

                case "DataEntrega":
                case "DataEntregaFormatada":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_ENTREGA, CargaEntrega.CEN_DATA_ENTREGA) DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, CargaEntregaAgrupada.CEN_DATA_ENTREGA, ");
                    }
                    break;

                case "TempoPermanencia":
                    if (!select.Contains(" DataSaidaCliente, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_SAIDA_RAIO, CargaEntrega.CEN_DATA_SAIDA_RAIO) DataSaidaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_SAIDA_RAIO, CargaEntregaAgrupada.CEN_DATA_SAIDA_RAIO, ");
                    }

                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_ENTREGA, CargaEntrega.CEN_DATA_ENTREGA) DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, CargaEntregaAgrupada.CEN_DATA_ENTREGA, ");
                    }
                    break;

                case "CPFCNPJCliente":
                case "CPFCNPJClienteFormatado":
                    if (!select.Contains(" CPFCNPJCliente, "))
                    {
                        select.Append("Cliente.CLI_CGCCPF CPFCNPJCliente, ");
                        groupBy.Append("Cliente.CLI_CGCCPF, ");

                        SetarJoinsCliente(joins);
                    }

                    if (!select.Contains(" TipoCliente, "))
                    {
                        select.Append("Cliente.CLI_FISJUR TipoCliente, ");
                        groupBy.Append("Cliente.CLI_FISJUR, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Cliente.CLI_NOME Cliente, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "Endereco":
                    if (!select.Contains(" Endereco, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.COE_CODIGO IS NOT NULL THEN ClienteOutroEndereco.COE_ENDERECO ELSE Cliente.CLI_ENDERECO END AS Endereco, ");
                        groupBy.Append("Cliente.CLI_ENDERECO, ClienteOutroEndereco.COE_ENDERECO, CargaEntrega.COE_CODIGO,  ");

                        SetarJoinsCliente(joins);
                        SetarJoinsClienteOutroEndereco(joins);
                    }
                    break;

                case "Bairro":
                    if (!select.Contains(" Bairro, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.COE_CODIGO IS NOT NULL THEN ClienteOutroEndereco.COE_BAIRRO ELSE Cliente.CLI_BAIRRO END AS Bairro, ");
                        groupBy.Append("Cliente.CLI_BAIRRO, ClienteOutroEndereco.COE_BAIRRO, CargaEntrega.COE_CODIGO,  ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        if (!groupBy.Contains("CargaEntrega.COE_CODIGO,"))
                            groupBy.Append("CargaEntrega.COE_CODIGO, ");

                        SetarJoinsCliente(joins);
                        SetarJoinsClienteOutroEndereco(joins);
                    }
                    break;

                case "CEP":
                    if (!select.Contains(" CEP, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.COE_CODIGO IS NOT NULL THEN ClienteOutroEndereco.COE_CEP ELSE Cliente.CLI_CEP END AS CEP, ");
                        groupBy.Append("Cliente.CLI_CEP, ClienteOutroEndereco.COE_CEP, CargaEntrega.COE_CODIGO,  ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        if (!groupBy.Contains("CargaEntrega.COE_CODIGO,"))
                            groupBy.Append("CargaEntrega.COE_CODIGO, ");

                        SetarJoinsCliente(joins);
                        SetarJoinsClienteOutroEndereco(joins);
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.COE_CODIGO IS NOT NULL THEN LocalicadeClienteOutroEndereco.LOC_DESCRICAO ELSE LocalicadeCliente.LOC_DESCRICAO END AS Cidade, ");
                        groupBy.Append("LocalicadeCliente.LOC_DESCRICAO, LocalicadeClienteOutroEndereco.LOC_DESCRICAO, CargaEntrega.COE_CODIGO,  ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        if (!groupBy.Contains("CargaEntrega.COE_CODIGO,"))
                            groupBy.Append("CargaEntrega.COE_CODIGO, ");

                        SetarJoinsCliente(joins);
                        SetarJoinsLocalidade(joins);
                        SetarJoinsClienteOutroEndereco(joins);
                        SetarJoinsLocalidadeOutroEndereco(joins);
                    }
                    break;

                case "Estado":
                    if (!select.Contains(" Estado, "))
                    {
                        select.Append("CASE WHEN CargaEntrega.COE_CODIGO IS NOT NULL THEN LocalicadeClienteOutroEndereco.UF_SIGLA ELSE LocalicadeCliente.UF_SIGLA END AS Estado, ");
                        groupBy.Append("LocalicadeCliente.UF_SIGLA, LocalicadeClienteOutroEndereco.UF_SIGLA, CargaEntrega.COE_CODIGO,  ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        if (!groupBy.Contains("CargaEntrega.COE_CODIGO,"))
                            groupBy.Append("CargaEntrega.COE_CODIGO, ");

                        SetarJoinsCliente(joins);
                        SetarJoinsLocalidade(joins);
                        SetarJoinsClienteOutroEndereco(joins);
                        SetarJoinsLocalidadeOutroEndereco(joins);
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT ', ' + CAST(_XMLNotaFiscal.NF_NUMERO AS NVARCHAR(20)) ");
                        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append("    LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoXMLNotaFiscal ON _pedidoXMLNotaFiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO ");
                        select.Append("    LEFT JOIN T_XML_NOTA_FISCAL _XMLNotaFiscal ON _XMLNotaFiscal.NFX_CODIGO = _pedidoXMLNotaFiscal.NFX_CODIGO ");
                        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
                        select.Append("    FOR XML PATH('') ");
                        select.Append("), 3, 1000) NotasFiscais,");

                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "Pedidos":
                    if (!select.Contains(" Pedidos, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR");
                        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append("    JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO");
                        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append("        FOR XML PATH('')");
                        select.Append("), 3, 1000) Pedidos,");

                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("( ");
                        select.Append("    SELECT SUM(_cargapedido.PED_PESO) ");
                        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
                        select.Append(") PesoBruto,");

                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "DataHoraAvaliacaoFormatada":
                    if (!select.Contains(" DataHoraAvaliacao, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_AVALIACAO DataHoraAvaliacao, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_AVALIACAO, ");
                    }
                    break;

                case "ResultadoAvaliacao":
                    if (!select.Contains(" ResultadoAvaliacao, "))
                    {
                        select.Append("CargaEntrega.CEN_AVALIACAO ResultadoAvaliacao, ");
                        groupBy.Append("CargaEntrega.CEN_AVALIACAO, ");
                    }
                    break;

                case "ObservacaoAvaliacao":
                    if (!select.Contains(" ObservacaoAvaliacao, "))
                    {
                        select.Append("CargaEntrega.CEN_OBSERVACAO_AVALIACAO ObservacaoAvaliacao, ");
                        groupBy.Append("CargaEntrega.CEN_OBSERVACAO_AVALIACAO, ");
                    }
                    break;

                case "MotivoAvaliacao":
                    if (!select.Contains(" MotivoAvaliacao, "))
                    {
                        select.Append("MotivoAvaliacao.TMA_DESCRICAO MotivoAvaliacao, ");
                        groupBy.Append("MotivoAvaliacao.TMA_DESCRICAO, ");

                        SetarJoinsMotivoAvaliacao(joins);
                    }
                    break;

                case "EntregaForaDoRaioFormatada":
                    if (!select.Contains(" RaioCliente, "))
                    {
                        select.Append("Cliente.CLI_RAIO_METROS RaioCliente, Cliente.CLI_LATIDUDE LatitudeCliente, Cliente.CLI_LONGITUDE LongitudeCliente, Cliente.CLI_TIPO_AREA TipoArea, ");
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_LATITUDE_FINALIZADA, CargaEntrega.CEN_LATITUDE_FINALIZADA, 0) LatitudeEntregaFinalizada, " +
                                      "COALESCE(CargaEntregaAgrupada.CEN_LONGITUDE_FINALIZADA, CargaEntrega.CEN_LONGITUDE_FINALIZADA, 0) LongitudeEntregaFinalizada, ");
                        groupBy.Append("Cliente.CLI_RAIO_METROS, Cliente.CLI_LATIDUDE, Cliente.CLI_LONGITUDE, CargaEntrega.CEN_LATITUDE_FINALIZADA, CargaEntrega.CEN_LONGITUDE_FINALIZADA, Cliente.CLI_TIPO_AREA, CargaEntregaAgrupada.CEN_LATITUDE_FINALIZADA, CargaEntregaAgrupada.CEN_LONGITUDE_FINALIZADA, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "TipoParadaFormatada":
                    if (!select.Contains(" TipoParada, "))
                    {
                        select.Append("CargaEntrega.CEN_COLETA TipoParada, ");
                        groupBy.Append("CargaEntrega.CEN_COLETA, ");
                    }
                    break;

                case "ValorTotalNotas":
                    if (!select.Contains(" ValorTotalNotas, "))
                    {
                        select.Append("( ");
                        select.Append("    SELECT SUM(XMLNotaFiscal.NF_VALOR) ");
                        select.Append("    FROM T_XML_NOTA_FISCAL XMLNotaFiscal");
                        select.Append("    JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotafiscal ON PedidoXMLNotafiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO");
                        select.Append("    JOIN T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal ON CargaEntregaNotaFiscal.PNF_CODIGO = PedidoXMLNotafiscal.PNF_CODIGO");
                        select.Append("    WHERE CargaEntregaNotaFiscal.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
                        select.Append(") ValorTotalNotas,");

                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "CodigoIntegracaoCliente":
                    if (!select.Contains(" CodigoIntegracaoCliente, "))
                    {
                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO CodigoIntegracaoCliente, ");
                        groupBy.Append("Cliente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "MonitoramentoStatus":
                case "DescricaoMonitoramentoStatus":
                    if (!select.Contains(" MonitoramentoStatus , "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_STATUS, Monitoramento.MON_STATUS) MonitoramentoStatus, ");
                        groupBy.Append("Monitoramento.MON_STATUS, MonitoramentoAgrupado.MON_STATUS, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "RealizadaNoPrazo":
                case "RealizadaNoPrazoFormatada":
                    if (!select.Contains(" RealizadaNoPrazo , "))
                    {
                        select.Append("CargaEntrega.CEN_REALIZADA_NO_PRAZO RealizadaNoPrazo, ");
                        groupBy.Append("CargaEntrega.CEN_REALIZADA_NO_PRAZO, ");
                    }
                    break;

                case "MotivoRetificacao":
                    if (!select.Contains(" MotivoRetificacao , "))
                    {
                        SetarJoinsMotivoRetificacao(joins);
                        select.Append("MotivoRetificacao.MRC_DESCRICAO MotivoRetificacao, ");
                        groupBy.Append("MotivoRetificacao.MRC_DESCRICAO, ");
                    }
                    break;

                case "DataConfirmacaoEntrega":
                case "DataConfirmacaoEntregaFormatada":
                    if (!select.Contains(" DataConfirmacaoEntrega , "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_CONFIRMACAO_ENTREGA DataConfirmacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_CONFIRMACAO_ENTREGA, ");
                    }
                    break;

                case "DataColeta":
                case "DataColetaFormatada":
                    if (!select.Contains(" DataColeta , "))
                    {
                        select.Append("PrimeiroPedido.PED_DATA_INICIAL_COLETA DataColeta, ");
                        groupBy.Append("PrimeiroPedido.PED_DATA_INICIAL_COLETA, ");

                        SetarJoinsPrimeiroPedido(joins);
                    }
                    break;

                //case "Quantidades":
                //    if (!select.Contains(" Quantidades, "))
                //    {
                //        select.Append("( ");
                //        select.Append("    SELECT SUM(_pedidoProduto.PRP_QUANTIDADE) ");
                //        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                //        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                //        select.Append("    JOIN T_PEDIDO_PRODUTO _pedidoProduto ON _pedidoProduto.PED_CODIGO = _cargaPedido.PED_CODIGO");
                //        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
                //        select.Append(") Quantidades,");

                //        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                //    }
                //    break;

                //case "Volumes":
                //    if (!select.Contains(" Volumes, "))
                //    {
                //        select.Append("( ");
                //        select.Append("    SELECT COUNT(_pedidoProduto.PRP_CODIGO) ");
                //        select.Append("    FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
                //        select.Append("    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
                //        select.Append("    JOIN T_PEDIDO_PRODUTO _pedidoProduto ON _pedidoProduto.PED_CODIGO = _cargaPedido.PED_CODIGO");
                //        select.Append("    WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
                //        select.Append(") Volumes,");

                //        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                //    }
                //    break;

                case "KMPlanejado":
                    if (!select.Contains(" KMPlanejado, "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_DISTANCIA_PREVISTA, Monitoramento.MON_DISTANCIA_PREVISTA) KMPlanejado, ");
                        groupBy.Append("Monitoramento.MON_DISTANCIA_PREVISTA, MonitoramentoAgrupado.MON_DISTANCIA_PREVISTA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "KMRealizado":
                    if (!select.Contains(" KMRealizado, "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_DISTANCIA_REALIZADA, Monitoramento.MON_DISTANCIA_REALIZADA) KMRealizado, ");
                        groupBy.Append("Monitoramento.MON_DISTANCIA_REALIZADA, MonitoramentoAgrupado.MON_DISTANCIA_REALIZADA, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("COALESCE(CargaAgrupada.CAR_DATA_INICIO_VIAGEM, Carga.CAR_DATA_INICIO_VIAGEM) DataInicioViagem, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, CargaAgrupada.CAR_DATA_INICIO_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataFimViagemFormatada":
                    if (!select.Contains(" DataFimViagem, "))
                    {
                        select.Append("COALESCE(CargaAgrupada.CAR_DATA_FIM_VIAGEM, Carga.CAR_DATA_FIM_VIAGEM) DataFimViagem, ");
                        groupBy.Append("Carga.CAR_DATA_FIM_VIAGEM, CargaAgrupada.CAR_DATA_FIM_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataConfirmacaoChegadaFormatada":
                    if (!select.Contains(" DataConfirmacaoChegada, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_ENTRADA_RAIO, CargaEntrega.CEN_DATA_ENTRADA_RAIO) DataConfirmacaoChegada, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO, CargaEntregaAgrupada.CEN_DATA_ENTRADA_RAIO, ");
                    }
                    break;

                case "DataInicioCarregamentoFormatada":
                    if (!select.Contains(" DataInicioCarregamento, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_INICIO_ENTREGA, CargaEntrega.CEN_DATA_INICIO_ENTREGA) DataInicioCarregamento, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, CargaEntregaAgrupada.CEN_DATA_INICIO_ENTREGA, ");
                    }
                    break;

                case "DataTerminoCarregamentoFormatada":
                    if (!select.Contains(" DataTerminoCarregamento, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_ENTREGA, CargaEntrega.CEN_DATA_ENTREGA) DataTerminoCarregamento, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, CargaEntregaAgrupada.CEN_DATA_ENTREGA, ");
                    }
                    break;

                case "DataInicioDescargaFormatada":
                    if (!select.Contains(" DataInicioDescarga, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_INICIO_ENTREGA, CargaEntrega.CEN_DATA_INICIO_ENTREGA) DataInicioDescarga, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, CargaEntregaAgrupada.CEN_DATA_INICIO_ENTREGA, ");
                    }
                    break;

                case "DataTerminoDescargaFormatada":
                    if (!select.Contains(" DataTerminoDescarga, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_DATA_FIM_ENTREGA, CargaEntrega.CEN_DATA_FIM_ENTREGA) DataTerminoDescarga, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_FIM_ENTREGA, CargaEntregaAgrupada.CEN_DATA_FIM_ENTREGA, ");
                    }
                    break;

                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "EncerramentoManualViagemFormatado":
                    if (!select.Contains(" EncerramentoManualViagem, "))
                    {
                        select.Append("COALESCE(CargaEntregaAgrupada.CEN_FINALIZADA_MANUALMENTE, CargaEntrega.CEN_FINALIZADA_MANUALMENTE) EncerramentoManualViagem,  ");
                        groupBy.Append("CargaEntrega.CEN_FINALIZADA_MANUALMENTE, CargaEntregaAgrupada.CEN_FINALIZADA_MANUALMENTE, ");
                    }
                    break;

                case "ConfirmacaoViaApp":
                    if (!select.Contains(" ConfirmacaoViaApp, "))
                    {
                        select.Append("Case when CargaEntrega.CEN_ORIGEM_SITUACAO = 3 then 'Sim' else 'Não' END ConfirmacaoViaApp, ");
                        groupBy.Append("CargaEntrega.CEN_ORIGEM_SITUACAO, ");
                    }
                    break;

                case "PrevisaoEntregaCliente":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, ");
                    }
                    if (!select.Contains(" PrevisaoEntregaCliente, "))
                    {
                        select.Append("COALESCE(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA,CargaEntrega.CEN_DATA_AGENDAMENTO) PrevisaoEntregaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_AGENDAMENTO, CargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");
                    }

                    if (!select.Contains(" DesconsiderarHorarioPrazo, "))
                    {
                        SetarJoinsConfiguracaoTipoOperacaoControleEntrega(joins);
                        select.Append("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA DesconsiderarHorarioPrazo, ");
                    }

                    if (!groupBy.Contains("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA,"))
                        groupBy.Append("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA, ");

                    break;

                case "AgendamentoEntregaCliente":
                case "AgendamentoEntregaClienteFormatada":
                    if (!select.Contains(" AgendamentoEntregaCliente, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_AGENDAMENTO AgendamentoEntregaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_AGENDAMENTO, ");
                    }

                    break;

                case "PrevisaoEntregaTransportador":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, ");
                    }
                    if (!select.Contains(" PrevisaoEntregaTransportador, "))
                    {
                        select.Append("COALESCE(CargaEntrega.CEN_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR, CargaEntrega.CEN_DATA_PREVISAO_ENTREGA_TRANSPORTADOR) PrevisaoEntregaTransportador, ");
                        select.Append("CargaEntrega.CEN_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR AgendamentoEntregaTransportador, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_PREVISAO_ENTREGA_TRANSPORTADOR, CargaEntrega.CEN_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR, ");
                    }

                    if (!select.Contains(" DesconsiderarHorarioPrazo, "))
                    {
                        SetarJoinsConfiguracaoTipoOperacaoControleEntrega(joins);
                        select.Append("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA DesconsiderarHorarioPrazo, ");
                    }

                    if (!groupBy.Contains("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA,"))
                        groupBy.Append("ConfiguracaoTipoOperacaoControleEntrega.COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA, ");

                    break;

                case "CPFCNPJRemetente":
                case "CPFCNPJRemetenteFormatado":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("RemetentePedido.CLI_CGCCPF CPFCNPJRemetente, ");
                        groupBy.Append("RemetentePedido.CLI_CGCCPF, ");

                        SetarJoinsRemetentePedido(joins);
                    }

                    if (!select.Contains(" TipoCliente, "))
                    {
                        select.Append("RemetentePedido.CLI_FISJUR TipoCliente, ");
                        groupBy.Append("RemetentePedido.CLI_FISJUR, ");

                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "ClienteRemetente":
                    if (!select.Contains(" ClienteRemetente, "))
                    {
                        select.Append("RemetentePedido.CLI_NOME ClienteRemetente, ");
                        groupBy.Append("RemetentePedido.CLI_NOME, ");

                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "EnderecoRemetente":
                    if (!select.Contains(" EnderecoRemetente, "))
                    {
                        select.Append("RemetentePedido.CLI_ENDERECO EnderecoRemetente, ");
                        groupBy.Append("RemetentePedido.CLI_ENDERECO, ");

                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "BairroRemetente":
                    if (!select.Contains(" BairroRemetente, "))
                    {
                        select.Append("RemetentePedido.CLI_BAIRRO BairroRemetente, ");
                        groupBy.Append("RemetentePedido.CLI_BAIRRO, ");

                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "EstadoRemetente":
                    if (!select.Contains(" EstadoRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA EstadoRemetente, ");
                        groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "CEPRemetente":
                    if (!select.Contains(" CEPRemetente, "))
                    {
                        select.Append("RemetentePedido.CLI_CEP CEPRemetente, ");
                        groupBy.Append("RemetentePedido.CLI_CEP, ");

                        SetarJoinsRemetentePedido(joins);
                    }
                    break;

                case "CidadeRemetente":
                    if (!select.Contains(" CidadeRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.LOC_DESCRICAO CidadeRemetente, ");
                        groupBy.Append("LocalidadeRemetente.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;
                case "RaioMedioViagem":
                    if (!select.Contains(" RaioMedioViagem, "))
                    {
                        select.Append(" Case when CargaEntrega.CEN_DISTANCIA > 0 then ((CargaEntrega.CEN_DISTANCIA / 2) / 1000) else 0 end RaioMedioViagem, ");
                        groupBy.Append("CargaEntrega.CEN_DISTANCIA, ");
                    }
                    break;
                case "QuantidadeAnimais":
                    if (!select.Contains(" QuantidadeAnimais, "))
                    {
                        select.Append(@"ISNULL(");
                        select.Append("  ( ");
                        select.Append("     SELECT TOP 1 CargaEntregaChecklistPergunta.CEP_RESPOSTA ");
                        select.Append("     FROM T_CARGA_ENTREGA_CHECKLIST CargaEntregaChecklist ");
                        select.Append("     JOIN T_CARGA_ENTREGA_CHECKLIST_PERGUNTA CargaEntregaChecklistPergunta ");
                        select.Append("     ON CargaEntregaChecklistPergunta.CEC_CODIGO = CargaEntregaChecklist.CEC_CODIGO ");
                        select.Append("     WHERE CargaEntregaChecklistPergunta.CEP_DESCRICAO = 'Quantidade de animais' AND CargaEntregaChecklist.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append("     ORDER BY CargaEntregaChecklistPergunta.CEP_CODIGO DESC");
                        select.Append("  ), '0') AS QuantidadeAnimais,");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO, "))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;
                case "QuantidadeMortalidade":
                    if (!select.Contains(" QuantidadeMortalidade, "))
                    {

                        select.Append(@"ISNULL(");
                        select.Append("  ( ");
                        select.Append("     SELECT TOP 1 CargaEntregaChecklistPergunta.CEP_RESPOSTA ");
                        select.Append("     FROM T_CARGA_ENTREGA_CHECKLIST CargaEntregaChecklist ");
                        select.Append("     JOIN T_CARGA_ENTREGA_CHECKLIST_PERGUNTA CargaEntregaChecklistPergunta ");
                        select.Append("     ON CargaEntregaChecklistPergunta.CEC_CODIGO = CargaEntregaChecklist.CEC_CODIGO ");
                        select.Append("     WHERE CargaEntregaChecklistPergunta.CEP_DESCRICAO = 'Quantidade de Mortalidade de Animais' AND CargaEntregaChecklist.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append("     ORDER BY CargaEntregaChecklistPergunta.CEP_CODIGO DESC");
                        select.Append("  ), '0') AS QuantidadeMortalidade,");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO, "))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "DataInicioMonitoramento":
                case "DataInicioMonitoramentoFormatada":
                    if (!select.Contains(" DataInicioMonitoramento, "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_DATA_INICIO, Monitoramento.MON_DATA_INICIO) DataInicioMonitoramento, ");
                        groupBy.Append("Monitoramento.MON_DATA_INICIO, MonitoramentoAgrupado.MON_DATA_INICIO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "DataFimMonitoramento":
                case "DataFimMonitoramentoFormatada":
                    if (!select.Contains(" DataFimMonitoramento, "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_DATA_FIM, Monitoramento.MON_DATA_FIM) DataFimMonitoramento, ");
                        groupBy.Append("Monitoramento.MON_DATA_FIM, MonitoramentoAgrupado.MON_DATA_FIM, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "PercentualViagemMonitoramentoDescricao":

                    if (!select.Contains(" PercentualViagemMonitoramento, "))
                    {
                        select.Append("COALESCE(MonitoramentoAgrupado.MON_PERCENTUAL_VIAGEM, Monitoramento.MON_PERCENTUAL_VIAGEM) PercentualViagemMonitoramento, ");
                        groupBy.Append("Monitoramento.MON_PERCENTUAL_VIAGEM, MonitoramentoAgrupado.MON_PERCENTUAL_VIAGEM, ");

                        SetarJoinsMonitoramento(joins);
                    }

                    break;

                case "Rota":
                    if (!select.Contains(" Rota, "))
                    {
                        select.Append("rota.ROF_DESCRICAO as Rota, ");
                        groupBy.Append("rota.ROF_DESCRICAO, ");

                        if (!joins.Contains(" rota "))
                            joins.Append(" LEFT JOIN T_ROTA_FRETE rota on rota.ROF_CODIGO = ISNULL( CargaAgrupada.ROF_CODIGO, Carga.ROF_CODIGO) ");

                        if (!groupBy.Contains("CargaAgrupada.ROF_CODIGO, "))
                            groupBy.Append("CargaAgrupada.ROF_CODIGO, ");

                        if (!groupBy.Contains("Carga.ROF_CODIGO, "))
                            groupBy.Append("Carga.ROF_CODIGO, ");

                    }
                    break;

                case "LatitudeUltimaPosicaoDescricao":
                case "LongitudeUltimaPosicaoDescricao":

                    if (!select.Contains(" LatitudeUltimaPosicao, "))
                    {
                        select.Append("CONVERT(VARCHAR, Posicao.POS_LATITUDE) AS LatitudeUltimaPosicao, ");
                        groupBy.Append("Posicao.POS_LATITUDE, ");
                    }

                    if (!select.Contains(" LongitudeUltimaPosicao, "))
                    {
                        select.Append("CONVERT(VARCHAR, Posicao.POS_LONGITUDE) AS LongitudeUltimaPosicao, ");
                        groupBy.Append("Posicao.POS_LONGITUDE, ");
                    }

                    SetarJoinsMonitoramento(joins);
                    SetarJoinsUltimaPosicao(joins);

                    if (!groupBy.Contains("Monitoramento.POS_ULTIMA_POSICAO, "))
                        groupBy.Append("Monitoramento.POS_ULTIMA_POSICAO, ");

                    if (!groupBy.Contains("MonitoramentoAgrupado.POS_ULTIMA_POSICAO, "))
                        groupBy.Append("MonitoramentoAgrupado.POS_ULTIMA_POSICAO, ");



                    break;

                case "OrigemMonitoramentoDescricao":

                    if (!select.Contains(" OrigemSituacaoEntregaFinalizada, "))
                    {
                        select.Append(@"CASE 
                                        WHEN CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO_FIM_VIAGEM IS NOT NULL THEN 
                                            CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO_FIM_VIAGEM
	                                    WHEN CargaEntrega.CEN_ORIGEM_SITUACAO_FIM_VIAGEM IS NOT NULL THEN
		                                    CargaEntrega.CEN_ORIGEM_SITUACAO_FIM_VIAGEM
	                                    WHEN CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO IS NOT NULL THEN
		                                    CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO 
                                        ELSE 
                                             CargaEntrega.CEN_ORIGEM_SITUACAO
                                    END as OrigemSituacaoEntregaFinalizada, ");

                        groupBy.Append("CargaEntrega.CEN_ORIGEM_SITUACAO_FIM_VIAGEM, CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO_FIM_VIAGEM, CargaEntregaAgrupada.CEN_ORIGEM_SITUACAO , CargaEntrega.CEN_ORIGEM_SITUACAO, ");
                    }
                    break;

                case "ProximaCargaProgramada":

                    if (!select.Contains(" ProximaCargaProgramada, "))
                    {
                        select.Append("( SELECT TOP 1 Carga.CAR_CODIGO_CARGA_EMBARCADOR " +
                            " FROM T_CARGA _carga " +
                            "WHERE _carga.CAR_CODIGO = ISNULL(MonitoramentoAgrupado.CAR_CODIGO, Monitoramento.CAR_CODIGO) AND ISNULL(MonitoramentoAgrupado.MON_STATUS, Monitoramento.MON_STATUS) = 0 " +
                            " ORDER BY ISNULL(MonitoramentoAgrupado.MON_DATA_CRIACAO, Monitoramento.MON_DATA_CRIACAO ) desc ) AS ProximaCargaProgramada, ");

                        if (!groupBy.Contains(" MonitoramentoAgrupado.CAR_CODIGO, "))
                            groupBy.Append("MonitoramentoAgrupado.CAR_CODIGO, ");

                        if (!groupBy.Contains(" Monitoramento.CAR_CODIGO, "))
                            groupBy.Append("Monitoramento.CAR_CODIGO, ");

                        if (!groupBy.Contains(" MonitoramentoAgrupado.MON_STATUS, "))
                            groupBy.Append("MonitoramentoAgrupado.MON_STATUS, ");

                        if (!groupBy.Contains(" Monitoramento.MON_STATUS, "))
                            groupBy.Append("Monitoramento.MON_STATUS, ");

                        if (!groupBy.Contains(" MonitoramentoAgrupado.MON_DATA_CRIACAO, "))
                            groupBy.Append("MonitoramentoAgrupado.MON_DATA_CRIACAO, ");

                        if (!groupBy.Contains(" Monitoramento.MON_DATA_CRIACAO, "))
                            groupBy.Append("Monitoramento.MON_DATA_CRIACAO, ");

                        SetarJoinsMonitoramento(joins);
                    }

                    break;

                case "MotivoFimMonitoramentoDescricao":

                    if (!select.Contains(" MotivoFimMonitoramento, "))
                    {
                        select.Append("CONVERT(VARCHAR, COALESCE(MonitoramentoAgrupado.MON_MOTIVO_FINALIZACAO, Monitoramento.MON_MOTIVO_FINALIZACAO)) AS MotivoFimMonitoramento,  ");
                        groupBy.Append("Monitoramento.MON_MOTIVO_FINALIZACAO, MonitoramentoAgrupado.MON_MOTIVO_FINALIZACAO, ");

                        SetarJoinsMonitoramento(joins);
                    }

                    break;

                case "Transbordo":
                    if (!select.Contains(" Transbordo, "))
                    {
                        select.Append("(CASE Carga.CAR_CARGA_TRANSBORDO WHEN 1 THEN 'Sim' ELSE 'Não' END) Transbordo, ");
                        groupBy.Append("Carga.CAR_CARGA_TRANSBORDO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataPrimeiroEspelhamentoFormatada":
                case "DataPrimeiroEspelhamento":
                    if (!select.Contains(" DataPrimeiroEspelhamento, "))
                    {
                        select.Append(@"
                                        (
                                            SELECT TOP 1 POS.POS_DATA_VEICULO
                                            FROM T_MONITORAMENTO_VEICULO MONVEI
                                            LEFT JOIN T_MONITORAMENTO_VEICULO_POSICAO MONVEIPOS ON MONVEI.MOV_CODIGO = MONVEIPOS.MOV_CODIGO
                                            LEFT JOIN T_POSICAO POS ON MONVEIPOS.POS_CODIGO = POS.POS_CODIGO
                                            WHERE Monitoramento.MON_CODIGO = MONVEI.MON_CODIGO
                                            ORDER BY POS.POS_DATA_VEICULO ASC
                                        ) AS DataPrimeiroEspelhamento,
                                     ");
                        groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                        SetarJoinsCarga(joins);
                        SetarJoinsUltimaPosicao(joins);
                    }
                    break;

                case "DataUltimoEspelhamentoFormatada":
                case "DataUltimoEspelhamento":
                    if (!select.Contains(" DataUltimoEspelhamento, "))
                    {
                        select.Append("Posicao.POS_DATA_VEICULO as DataUltimoEspelhamento, ");
                        groupBy.Append("Posicao.POS_DATA_VEICULO, ");

                        SetarJoinsUltimaPosicao(joins);
                    }
                    break;

                case "ProtocoloIntegracaoCarga":
                    if (!select.Contains(" ProtocoloIntegracaoCarga, "))
                    {
                        select.Append("Carga.CAR_PROTOCOLO ProtocoloIntegracaoCarga, ");
                        groupBy.Append("Carga.CAR_PROTOCOLO, ");
                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ProtocoloPedido":
                    if (!select.Contains(" ProtocoloPedido, "))
                    {
                        // Join necessário para acessar CargaPedido
                        select.Append(@"(
                            SELECT TOP 1 Pedido.PED_PROTOCOLO
                            FROM T_CARGA_ENTREGA_PEDIDO CEP
                            JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CEP.CPE_CODIGO
                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CP.PED_CODIGO
                            WHERE CEP.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                        ) ProtocoloPedido, ");
                        groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                        SetarJoinsPedido(joins);
                    }
                    break;

                case "PrevisaoChegada":
                    if (!select.Contains(" PrevisaoChegada, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA PrevisaoChegada, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");
                    }
                    break;

                case "PrevisaoChegadaRecalculadaETA":
                    if (!select.Contains(" PrevisaoChegadaRecalculadaETA, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA PrevisaoChegadaRecalculadaETA, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");
                    }
                    break;

                default:
                    if (propriedade.Length == 0)
                        return;
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string utcDateTimePattern = "yyyy-MM-ddTHH:mm:ss.fffZ";

            StringBuilder selectPedidosEntrega = new StringBuilder();
            StringBuilder wherePedidosEntrega = new StringBuilder();
            selectPedidosEntrega.Append(" SELECT _cargaEntregaPedido.CEN_CODIGO");
            selectPedidosEntrega.Append(" FROM T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido");
            selectPedidosEntrega.Append(" JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO");
            selectPedidosEntrega.Append(" JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO");
            selectPedidosEntrega.Append(" WHERE _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");

            where.Append($" AND Carga.CAR_SITUACAO NOT IN (13, 18) ");
            SetarJoinsCarga(joins);

            if (filtrosPesquisa.ExibirCargasAgrupadas)
                where.Append(" and Carga.CAR_CARGA_FECHADA = 1 ");
            else
                where.Append(" and ((Carga.CAR_CARGA_FECHADA = 1 and Carga.CAR_CARGA_AGRUPADA = 0 ) or (Carga.CAR_CARGA_FECHADA = 0 and Carga.CAR_CODIGO_AGRUPAMENTO is not null)) ");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.DataEntregaInicial.HasValue)
                where.Append($" and CargaEntrega.CEN_DATA_ENTREGA >= '{filtrosPesquisa.DataEntregaInicial.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.DataEntregaFinal.HasValue)
                where.Append($" and CargaEntrega.CEN_DATA_ENTREGA < '{filtrosPesquisa.DataEntregaFinal.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.NumeroCargas.Count > 0)
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and (Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE {string.Join(" OR Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE ", (from o in filtrosPesquisa.NumeroCargas select $"'%{o}%'"))})");
                else
                    where.Append($" and (Carga.CAR_CODIGO_CARGA_EMBARCADOR IN ({string.Join(",", (from o in filtrosPesquisa.NumeroCargas select $"'{o}'"))}) or Carga.CAR_CODIGO_AGRUPAMENTO in (select _agrupamento.CAR_CODIGO_AGRUPAMENTO from T_CARGA _agrupamento where _agrupamento.CAR_CODIGO_CARGA_EMBARCADOR in ({string.Join(",", (from o in filtrosPesquisa.NumeroCargas select $"'{o}'"))})))");
            }

            if (filtrosPesquisa.CodigosTransportadores.Count > 0)
                where.Append($" and Carga.EMP_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosTransportadores)})");

            if (filtrosPesquisa.CodigosFiliais.Contains(-1))
            {
                where.Append($@" and ( Carga.FIL_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosFiliais)}) OR  EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)}) ))");
            }
            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
                where.Append($" and Carga.FIL_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosFiliais)})");

            if (filtrosPesquisa.CodigosVeiculos.Count > 0)
            {
                StringBuilder selectVeiculos = new StringBuilder();
                selectVeiculos.Append(" SELECT _veiculoVinculadoCarga.CAR_CODIGO");
                selectVeiculos.Append(" FROM T_CARGA_VEICULOS_VINCULADOS _veiculoVinculadoCarga");
                selectVeiculos.Append(" WHERE _veiculoVinculadoCarga.CAR_CODIGO = Carga.CAR_CODIGO");
                selectVeiculos.Append($" AND _veiculoVinculadoCarga.VEI_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosVeiculos)})");

                where.Append($" and (Carga.CAR_VEICULO IN ({string.Join(",", filtrosPesquisa.CodigosVeiculos)}) OR Carga.CAR_CODIGO IN ({selectVeiculos}))");
            }

            if (filtrosPesquisa.CodigosMotoristas.Count > 0)
            {
                StringBuilder selectMotoristas = new StringBuilder();
                selectMotoristas.Append(" SELECT _cargaMotorista.CAR_CODIGO");
                selectMotoristas.Append(" FROM T_CARGA_MOTORISTA _cargaMotorista");
                selectMotoristas.Append(" WHERE _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO");
                selectMotoristas.Append($" AND _cargaMotorista.CAR_MOTORISTA IN ({string.Join(",", filtrosPesquisa.CodigosMotoristas)})");

                where.Append($" and Carga.CAR_CODIGO IN ({selectMotoristas})");
            }

            if (filtrosPesquisa.CodigosTipoCargas.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCargas)}){(filtrosPesquisa.CodigosTipoCargas.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigosTipoOperacoes.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacoes)}){(filtrosPesquisa.CodigosTipoOperacoes.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.TipoParada.HasValue)
                where.Append($" AND CargaEntrega.CEN_COLETA = {(filtrosPesquisa.TipoParada.Value ? "1" : "0")}");

            if (filtrosPesquisa.CodigosGrupoPessoas.Count > 0)
                where.Append($" and (Carga.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoas)}){(filtrosPesquisa.CodigosGrupoPessoas.Contains(-1) ? " or Carga.GRP_CODIGO is null" : "")})");

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.EscritorioVendas))
            {
                if (!joins.Contains("Cliente"))
                    SetarJoinsCliente(joins);

                StringBuilder selectEscritorioVendas = new StringBuilder();
                selectEscritorioVendas.Append(" SELECT 1");
                selectEscritorioVendas.Append(" FROM T_CLIENTE_COMPLEMENTAR ClienteComplementar");
                selectEscritorioVendas.Append(" WHERE ClienteComplementar.CLI_CODIGO = Cliente.CLI_CGCCPF");
                selectEscritorioVendas.Append($" AND ClienteComplementar.CLC_ESCRITORIO_VENDAS = '{filtrosPesquisa.EscritorioVendas}'");

                where.Append($" AND EXISTS ({selectEscritorioVendas})");
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.ProtocoloIntegracaoSM))
            {

                StringBuilder selectProtocoloIntegracao = new StringBuilder();
                selectProtocoloIntegracao.Append(" EXISTS (");
                selectProtocoloIntegracao.Append(" SELECT 1");
                selectProtocoloIntegracao.Append("  FROM T_CARGA_DADOS_TRANSPORTE_INTEGRACAO CargaDadosTransporteIntegracao");
                selectProtocoloIntegracao.Append("  WHERE CargaDadosTransporteIntegracao.CAR_CODIGO = Carga.CAR_CODIGO");
                selectProtocoloIntegracao.Append($"  AND CargaDadosTransporteIntegracao.CDI_PROTOCOLO = '{filtrosPesquisa.ProtocoloIntegracaoSM}'");
                selectProtocoloIntegracao.Append($")");

                selectProtocoloIntegracao.Append(" OR EXISTS (");
                selectProtocoloIntegracao.Append(" SELECT 1");
                selectProtocoloIntegracao.Append("   FROM T_CARGA_CARGA_INTEGRACAO CargaCargaIntegracao");
                selectProtocoloIntegracao.Append("   WHERE CargaCargaIntegracao.CAR_CODIGO = Carga.CAR_CODIGO");
                selectProtocoloIntegracao.Append($"   AND CargaCargaIntegracao.CCA_PROTOCOLO = '{filtrosPesquisa.ProtocoloIntegracaoSM}'");
                selectProtocoloIntegracao.Append($")");


                where.Append($" AND ({selectProtocoloIntegracao})");
            }
            //Filtros referente aos pedidos
            if (filtrosPesquisa.CpfsCnpjsRemetentes.Count > 0)
                wherePedidosEntrega.Append($" AND _pedido.CLI_CODIGO_REMETENTE IN ({string.Join(",", (from o in filtrosPesquisa.CpfsCnpjsRemetentes select $"'{o}'"))})");

            if (filtrosPesquisa.CpfsCnpjsDestinatarios.Count > 0)
                wherePedidosEntrega.Append($" AND _pedido.CLI_CODIGO IN ({string.Join(",", (from o in filtrosPesquisa.CpfsCnpjsDestinatarios select $"'{o}'"))})");

            if (filtrosPesquisa.CodigoOrigem > 0)
                wherePedidosEntrega.Append($" AND _pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoOrigem}");

            if (filtrosPesquisa.CodigoDestino > 0)
                wherePedidosEntrega.Append($" AND _pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoDestino}");

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedidoCliente))
                wherePedidosEntrega.Append($" AND _pedido.PED_CODIGO_PEDIDO_CLIENTE IN ('{string.Join(",", (filtrosPesquisa.NumeroPedidoCliente))}')");

            if (wherePedidosEntrega.Length > 0)
            {
                selectPedidosEntrega.Append(wherePedidosEntrega);
                where.Append($" and CargaEntrega.CEN_CODIGO in ({selectPedidosEntrega})");
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracaoCliente))
                where.Append($" and Cliente.CLI_CODIGO_INTEGRACAO = '{filtrosPesquisa.CodigoIntegracaoCliente}'");

            if (filtrosPesquisa.DataEntregaPlanejadaInicio.HasValue)
                where.Append($" and CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.DataEntregaPlanejadaInicio.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.DataEntregaPlanejadaFinal.HasValue)
                where.Append($" and CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.DataEntregaPlanejadaFinal.Value.ToString(utcDateTimePattern)}'");

            if (filtrosPesquisa.MonitoramentoStatus != null && filtrosPesquisa.MonitoramentoStatus.Count > 0)
            {
                SetarJoinsMonitoramento(joins);

                var statusCodes = filtrosPesquisa.MonitoramentoStatus.Cast<int>();

                where.Append($" AND COALESCE(MonitoramentoAgrupado.MON_STATUS, Monitoramento.MON_STATUS) IN ({string.Join(", ", statusCodes)})");
            }

            if (filtrosPesquisa.Transbordo.HasValue)
            {
                if (filtrosPesquisa.Transbordo.Value)
                    where.Append(" and Carga.CAR_CARGA_TRANSBORDO = 1 ");
                else
                    where.Append(" and (Carga.CAR_CARGA_TRANSBORDO = 0 or Carga.CAR_CARGA_TRANSBORDO is null) ");

                SetarJoinsCarga(joins);
            }
        }

        #endregion
    }
}
