using Dominio.ObjetosDeValor.Embarcador.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaRastreabilidadeVolumes : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes>
    {

        #region Construtor

        public ConsultaRastreabilidadeVolumes() : base (tabela: "T_RECEBIMENTO as Recebimento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsMercadoria(StringBuilder joins)
        {
            if (!joins.Contains(" Mercadoria "))
                joins.Append(" JOIN T_RECEBIMENTO_MERCADORIA Mercadoria on Mercadoria.RME_CODIGO = Recebimento.RME_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT OUTER JOIN T_CARGA Carga on Carga.CAR_CODIGO = Recebimento.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT OUTER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" LEFT OUTER JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsUsuarioRecebimento(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioRecebimento "))
                joins.Append(" LEFT OUTER JOIN T_FUNCIONARIO UsuarioRecebimento on UsuarioRecebimento.FUN_CODIGO = Recebimento.FUN_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsMercadoria(joins);

            if (!joins.Contains(" Destinatario "))
                joins.Append(" JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Mercadoria.CPF_CNPJ_DESTINATARIO ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsMercadoria(joins);

            if (!joins.Contains(" Remetente "))
                joins.Append(" JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Mercadoria.CPF_CNPJ_REMETENTE ");
        }

        private void SetarJoinsLocalidadeRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeRemetente on LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO ");
        }

        private void SetarJoinsProduto(StringBuilder joins)
        {
            if (!joins.Contains(" Produto "))
                joins.Append(" LEFT OUTER JOIN T_PRODUTO_EMBARCADOR Produto on Produto.PRO_CODIGO = Recebimento.PRO_CODIGO ");
        }

        private void SetarJoinsPosicao(StringBuilder joins)
        {
            SetarJoinsMercadoria(joins);

            if (!joins.Contains(" Posicao "))
                joins.Append(" LEFT OUTER JOIN T_DEPOSITO_POSICAO Posicao on Posicao.DPO_CODIGO = Mercadoria.DPO_CODIGO ");
        }

        private void SetarJoinsBloco(StringBuilder joins)
        {
            SetarJoinsPosicao(joins);

            if (!joins.Contains(" Bloco "))
                joins.Append(" LEFT OUTER JOIN T_DEPOSITO_BLOCO Bloco on Bloco.DEB_CODIGO = Posicao.DEB_CODIGO ");
        }

        private void SetarJoinsRua(StringBuilder joins)
        {
            SetarJoinsBloco(joins);

            if (!joins.Contains(" Rua "))
                joins.Append(" LEFT OUTER JOIN T_DEPOSITO_RUA Rua on Rua.DER_CODIGO = Bloco.DER_CODIGO ");
        }

        private void SetarJoinsDeposito(StringBuilder joins)
        {
            SetarJoinsRua(joins);

            if (!joins.Contains(" Deposito "))
                joins.Append(" LEFT OUTER JOIN T_DEPOSITO Deposito on Deposito.DEP_CODIGO = Rua.DEP_CODIGO ");
        }

        private void SetarJoinsConferencia(StringBuilder joins)
        {
            SetarJoinsMercadoria(joins);

            if (!joins.Contains(" Conferencia "))
                joins.Append(" LEFT OUTER JOIN T_CONFERENCIA_SEPARACAO Conferencia on Conferencia.COS_CODIGO_BARRAS = Mercadoria.REM_IDENTIFICACAO ");
        }

        private void SetarJoinsExpedicao(StringBuilder joins)
        {
            SetarJoinsConferencia(joins);

            if (!joins.Contains(" Expedicao "))
                joins.Append(" LEFT OUTER JOIN T_CARGA_CONTROLE_EXPEDICAO Expedicao on Expedicao.CCX_CODIGO = Conferencia.CCX_CODIGO ");
        }

        private void SetarJoinsCargaExpedicao(StringBuilder joins)
        {
            SetarJoinsExpedicao(joins);

            if (!joins.Contains(" CargaExpedicao "))
                joins.Append(" LEFT OUTER JOIN T_CARGA CargaExpedicao on CargaExpedicao.CAR_CODIGO = Expedicao.CAR_CODIGO ");
        }

        private void SetarJoinsUsuarioExpedicao(StringBuilder joins)
        {
            SetarJoinsExpedicao(joins);

            if (!joins.Contains(" UsuarioExpedicao "))
                joins.Append(" LEFT OUTER JOIN T_FUNCIONARIO UsuarioExpedicao on UsuarioExpedicao.FUN_CODIGO = Expedicao.FUN_CODIGO");
        }

        private void SetarJoinsCargaPedidoExpedicao(StringBuilder joins)
        {
            SetarJoinsCargaExpedicao(joins);

            if (!joins.Contains(" CargaPedidoExpedicao "))
                joins.Append(" LEFT OUTER JOIN T_CARGA_PEDIDO CargaPedidoExpedicao on CargaPedidoExpedicao.CAR_CODIGO = CargaExpedicao.CAR_CODIGO ");
        }

        private void SetarJoinsPedidoExpedicao(StringBuilder joins)
        {
            SetarJoinsCargaPedidoExpedicao(joins);

            if (!joins.Contains(" PedidoExpedicao "))
                joins.Append(" LEFT OUTER JOIN T_PEDIDO PedidoExpedicao on PedidoExpedicao.PED_CODIGO = CargaPedidoExpedicao.PED_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            SetarJoinsCargaPedidoExpedicao(joins);

            if (!joins.Contains(" Expedidor "))
                joins.Append(" LEFT OUTER JOIN T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedidoExpedicao.CLI_CODIGO_EXPEDIDOR ");
        }

        private void SetarJoinsLocalidadeExpedidor(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" LocalidadeExpedidor "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES LocalidadeExpedidor on LocalidadeExpedidor.LOC_CODIGO = Expedidor.LOC_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCargaExpedicao(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT OUTER JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = CargaExpedicao.TOP_CODIGO ");
        }

        private void SetarJoinsUnidadeMedida(StringBuilder joins)
        {
            SetarJoinsProduto(joins);

            if (!joins.Contains(" Unidade "))
                joins.Append(" LEFT OUTER JOIN T_UNIDADE_MEDIDA Unidade ON Unidade.UNI_CODIGO = Produto.UNI_CODIGO ");
        }

        private void SetarJoinsLote(StringBuilder joins)
        {
            SetarJoinsProduto(joins);
            SetarJoinsPosicao(joins);
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" Lote "))
                joins.Append(" LEFT OUTER JOIN T_PRODUTO_EMBARCADOR_LOTE Lote ON Lote.PRO_CODIGO = Produto.PRO_CODIGO and Lote.DPO_CODIGO = Posicao.DPO_CODIGO AND Lote.CPF_CNPJ_REMETENTE = Remetente.CLI_CGCCPF ");
        }

        private void SetarJoinsVeiculoExpedicao(StringBuilder joins)
        {
            SetarJoinsCargaExpedicao(joins);

            if (!joins.Contains(" VeiculoExpedicao "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO VeiculoExpedicao on VeiculoExpedicao.VEI_CODIGO = CargaExpedicao.CAR_VEICULO ");
        }

        private void SetarJoinsTerceiro(StringBuilder joins)
        {
            SetarJoinsCargaExpedicao(joins);

            if (!joins.Contains(" Terceiro "))
                joins.Append(" LEFT OUTER JOIN T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = CargaExpedicao.CLI_CGCCPF_TERCEIRO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa)
        {
            switch(propriedade)
            {
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
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


                case "DataPedido":
                case "DataPedidoFormatada":
                    if (!select.Contains(" DataPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataRecebimento":
                case "DataRecebimentoFormatada":
                    if (!select.Contains(" DataRecebimento, "))
                    {
                        select.Append("Recebimento.RME_DATA DataRecebimento, ");
                        groupBy.Append("Recebimento.RME_DATA, ");
                    }
                    break;

                case "UsuarioRecebimento":
                    if (!select.Contains(" UsuarioRecebimento, "))
                    {
                        select.Append("UsuarioRecebimento.FUN_NOME UsuarioRecebimento, ");
                        groupBy.Append("UsuarioRecebimento.FUN_NOME, ");

                        SetarJoinsUsuarioRecebimento(joins);
                    }
                    break;

                case "NumeroNF":
                    if (!select.Contains(" NumeroNF, "))
                    {
                        select.Append("Mercadoria.REM_NUMERO_NF NumeroNF, ");
                        groupBy.Append("Mercadoria.REM_NUMERO_NF, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("Mercadoria.REM_SERIE_NF Serie, ");
                        groupBy.Append("Mercadoria.REM_SERIE_NF, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        select.Append("Mercadoria.REM_QUANTIDADE_LOTE Quantidade, ");
                        groupBy.Append("Mercadoria.REM_QUANTIDADE_LOTE, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "PesoLiquido":
                    if (!select.Contains(" PesoLiquido, "))
                    {
                        select.Append("Mercadoria.REM_PESO_LIQUIDO PesoLiquido, ");
                        groupBy.Append("Mercadoria.REM_PESO_LIQUIDO, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("Mercadoria.REM_PESO_BRUTO PesoBruto, ");
                        groupBy.Append("Mercadoria.REM_PESO_BRUTO, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "ValorMercadoria":
                    if (!select.Contains(" ValorMercadoria, "))
                    {
                        select.Append("Mercadoria.REM_VALOR_MERCADORIA ValorMercadoria, ");
                        groupBy.Append("Mercadoria.REM_VALOR_MERCADORIA, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "TipoNF":
                case "TipoNFDescricao":
                    if (!select.Contains(" TipoNF, "))
                    {
                        select.Append("Recebimento.RME_TIPO_RECEBIMENTO TipoNF, ");
                        groupBy.Append("Recebimento.RME_TIPO_RECEBIMENTO, ");
                    }
                    break;

                case "QtdItensRecebidos":
                    if (!select.Contains(" QtdItensRecebidos, "))
                    {
                        select.Append("Mercadoria.REM_QUANTIDADE_CONFERIDA QtdItensRecebidos, ");
                        groupBy.Append("Mercadoria.REM_QUANTIDADE_CONFERIDA, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "QtdItensFaltantes":
                    if (!select.Contains(" QtdItensFaltantes, "))
                    {
                        select.Append("Mercadoria.REM_QUANTIDADE_FALTANTE QtdItensFaltantes, ");
                        groupBy.Append("Mercadoria.REM_QUANTIDADE_FALTANTE, ");

                        SetarJoinsMercadoria(joins);
                    }
                    break;

                case "Ocorrencias":
                    if (!select.Contains(" Ocorrencias, "))
                    {
                        select.Append("'' Ocorrencias, ");
                        //groupBy.Append(", ");
                    }
                    break;

                case "DataArmazen":
                case "DataArmazenFormatada":
                    if (!select.Contains(" DataArmazen, "))
                    {
                        select.Append("Recebimento.RME_DATA DataArmazen, ");
                        groupBy.Append("Recebimento.RME_DATA, ");
                    }
                    break;

                case "OperadorArmazen":
                    if (!select.Contains(" OperadorArmazen, "))
                    {
                        select.Append("UsuarioRecebimento.FUN_NOME OperadorArmazen, ");
                        groupBy.Append("UsuarioRecebimento.FUN_NOME, ");

                        SetarJoinsUsuarioRecebimento(joins);
                    }
                    break;

                case "ProdutoEmbarcador":
                    if (!select.Contains(" ProdutoEmbarcador, "))
                    {
                        select.Append("Produto.GRP_DESCRICAO ProdutoEmbarcador, ");
                        groupBy.Append("Produto.GRP_DESCRICAO, ");

                        SetarJoinsProduto(joins);
                    }
                    break;

                case "Deposito":
                    if (!select.Contains(" Deposito, "))
                    {
                        select.Append("Deposito.DEP_DESCRICAO Deposito, ");
                        groupBy.Append("Deposito.DEP_DESCRICAO, ");

                        SetarJoinsDeposito(joins);
                    }
                    break;

                case "Bloco":
                    if (!select.Contains(" Bloco, "))
                    {
                        select.Append("Bloco.DEB_DESCRICAO Bloco, ");
                        groupBy.Append("Bloco.DEB_DESCRICAO, ");

                        SetarJoinsBloco(joins);
                    }
                    break;

                case "Rua":
                    if (!select.Contains(" Rua, "))
                    {
                        select.Append("Rua.DER_DESCRICAO Rua, ");
                        groupBy.Append("Rua.DER_DESCRICAO, ");

                        SetarJoinsRua(joins);
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("Posicao.DPO_DESCRICAO Posicao, ");
                        groupBy.Append("Posicao.DPO_DESCRICAO, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "Local":
                    if (!select.Contains(" Local, "))
                    {
                        select.Append("Posicao.DPO_ABREVIACAO Local, ");
                        groupBy.Append("Posicao.DPO_ABREVIACAO, ");

                        SetarJoinsPosicao(joins);
                    }
                    break;

                case "CodigoBarras":
                    if (!select.Contains(" CodigoBarras, "))
                    {
                        select.Append("'' CodigoBarras, ");
                        //groupBy.Append(", ");
                    }
                    break;

                case "EtiquetaMasterPallet":
                    if (!select.Contains(" EtiquetaMasterPallet, "))
                    {
                        select.Append("'' EtiquetaMasterPallet, ");
                        //groupBy.Append(", ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Lote.PEL_DESCRICAO Descricao, ");
                        groupBy.Append("Lote.PEL_DESCRICAO, ");

                        SetarJoinsLote(joins);
                    }
                    break;

                case "DataVencimento":
                case "DataVencimentoFormatada":
                    if (!select.Contains(" DataVencimento, "))
                    {
                        select.Append("Lote.PEL_DATA_VENCIMENTO DataVencimento, ");
                        groupBy.Append("Lote.PEL_DATA_VENCIMENTO, ");

                        SetarJoinsLote(joins);
                    }
                    break;

                case "TipoMercadoria":
                    if (!select.Contains(" TipoMercadoria, "))
                    {
                        select.Append(@"CASE
                                            WHEN Lote.PEL_TIPO_RECEBIMENTO = 2 THEN 'Volume'
                                            ELSE 'Mercadoria'
                                        END TipoMercadoria, ");
                        groupBy.Append("Lote.PEL_TIPO_RECEBIMENTO, ");

                        SetarJoinsLote(joins);
                    }
                    break;

                case "UN":
                    if (!select.Contains(" UN, "))
                    {
                        select.Append("Unidade.UNI_CODIGO_UNIDADE UN, ");
                        groupBy.Append("Unidade.UNI_CODIGO_UNIDADE, ");

                        SetarJoinsUnidadeMedida(joins);
                    }
                    break;

                case "QtdLote":
                    if (!select.Contains(" QtdLote, "))
                    {
                        select.Append("Lote.PEL_QUANTIDADE_LOTE QtdLote, ");
                        groupBy.Append("Lote.PEL_QUANTIDADE_LOTE, ");

                        SetarJoinsLote(joins);
                    }
                    break;

                case "RegistroOcorrencia":
                    if (!select.Contains(" RegistroOcorrencia, "))
                    {
                        select.Append("'' RegistroOcorrencia, ");
                        //groupBy.Append(", ");
                    }
                    break;

                case "QtdDevolvida":
                    if (!select.Contains(" QtdDevolvida, "))
                    {
                        select.Append("0.0 QtdDevolvida, ");
                        //groupBy.Append(", ");
                    }
                    break;

                case "QtdDisponivel":
                    if (!select.Contains(" QtdDisponivel, "))
                    {
                        select.Append("Lote.PEL_QUANTIDADE_ATUAL QtdDisponivel, ");
                        groupBy.Append("Lote.PEL_QUANTIDADE_ATUAL, ");

                        SetarJoinsLote(joins);
                    }
                    break;

                case "DataExpedicao":
                case "DataExpedicaoFormatada":
                    if (!select.Contains(" DataExpedicao, "))
                    {
                        select.Append("Expedicao.CCX_DATA_CONFIRMACAO DataExpedicao, ");
                        groupBy.Append("Expedicao.CCX_DATA_CONFIRMACAO, ");

                        SetarJoinsExpedicao(joins);
                    }
                    break;

                case "UsuarioExpedicao":
                    if (!select.Contains(" UsuarioExpedicao, "))
                    {
                        select.Append("UsuarioExpedicao.FUN_NOME UsuarioExpedicao, ");
                        groupBy.Append("UsuarioExpedicao.FUN_NOME, ");

                        SetarJoinsUsuarioExpedicao(joins);
                    }
                    break;

                case "EtiquetaExpedicao":
                    if (!select.Contains(" EtiquetaExpedicao, "))
                    {
                        select.Append("Conferencia.COS_CODIGO_BARRAS EtiquetaExpedicao, ");
                        groupBy.Append("Conferencia.COS_CODIGO_BARRAS, ");

                        SetarJoinsConferencia(joins);
                    }
                    break;

                case "Volumes":
                    if (!select.Contains(" Volumes, "))
                    {
                        select.Append("Conferencia.COS_VOLUMES_CARGA Volumes, ");
                        groupBy.Append("Conferencia.COS_VOLUMES_CARGA, ");

                        SetarJoinsConferencia(joins);
                    }
                    break;

                case "Embarcados":
                    if (!select.Contains(" Embarcados, "))
                    {
                        select.Append("Conferencia.COS_QUANTIDADE Embarcados, ");
                        groupBy.Append("Conferencia.COS_QUANTIDADE, ");

                        SetarJoinsConferencia(joins);
                    }
                    break;

                case "Falta":
                    if (!select.Contains(" Falta, "))
                    {
                        select.Append("Conferencia.COS_QUANTIDADE_FALTANTE Falta, ");
                        groupBy.Append("Conferencia.COS_QUANTIDADE_FALTANTE, ");

                        SetarJoinsConferencia(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("Remetente.CLI_NOME Remetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "UfRemetente":
                    if (!select.Contains(" UfRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA UfRemetente, ");
                        groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "UfDestinatario":
                    if (!select.Contains(" UfDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UfDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("Expedidor.CLI_NOME Expedidor, ");
                        groupBy.Append("Expedidor.CLI_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "UfExpedidor":
                    if (!select.Contains(" UfExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.UF_SIGLA UfExpedidor, ");
                        groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsLocalidadeExpedidor(joins);
                    }
                    break;

                case "CargaTransbordo":
                    if (!select.Contains(" CargaTransbordo, "))
                    {
                        select.Append("CargaExpedicao.CAR_CODIGO_CARGA_EMBARCADOR CargaTransbordo, ");
                        groupBy.Append("CargaExpedicao.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCargaExpedicao(joins);
                    }
                    break;

                case "DataConferencia":
                case "DataConferenciaFormatada":
                    if (!select.Contains(" DataConferencia, "))
                    {
                        select.Append("Expedicao.CJC_DATA_INICIO_CARREGAMENTO DataConferencia, ");
                        groupBy.Append("Expedicao.CJC_DATA_INICIO_CARREGAMENTO, ");

                        SetarJoinsExpedicao(joins);
                    }
                    break;

                case "DataEmbarque":
                case "DataEmbarqueFormatada":
                    if (!select.Contains(" DataEmbarque, "))
                    {
                        select.Append("Expedicao.CCX_DATA_CONFIRMACAO DataEmbarque, ");
                        groupBy.Append("Expedicao.CCX_DATA_CONFIRMACAO, ");

                        SetarJoinsExpedicao(joins);
                    }
                    break;

                case "MDFe":
                    if (!select.Contains(" MDFe, "))
                    {
                        select.Append(@"SUBSTRING((
                                            SELECT DISTINCT ', ' + 
                                                CAST(MDFe.MDF_NUMERO AS NVARCHAR(20))
                                            FROM T_CARGA_MDFE CargaMDFe
                                                inner join T_MDFE MDFe ON MDFe.MDF_CODIGO = CargaMDFe.MDF_CODIGO 
                                            WHERE CargaMDFe.CAR_CODIGO = CargaExpedicao.CAR_CODIGO 
                                            FOR XML PATH('')), 
                                        3, 1000) MDFe, ");

                        if (!groupBy.Contains("CargaExpedicao.CAR_CODIGO,"))
                            groupBy.Append("CargaExpedicao.CAR_CODIGO, ");

                        SetarJoinsCargaExpedicao(joins);
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

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("VeiculoExpedicao.VEI_NUMERO_FROTA Veiculo, ");
                        groupBy.Append("VeiculoExpedicao.VEI_NUMERO_FROTA, ");

                        SetarJoinsVeiculoExpedicao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Terceiro.CLI_NOME Transportador, ");
                        groupBy.Append("Terceiro.CLI_NOME, ");

                        SetarJoinsTerceiro(joins);
                    }
                    break;


                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("VeiculoExpedicao.VEI_PLACA PlacaVeiculo, ");
                        groupBy.Append("VeiculoExpedicao.VEI_PLACA, ");

                        SetarJoinsVeiculoExpedicao(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append(@"SUBSTRING((
                                            SELECT DISTINCT ', ' + 
                                                Motorista.FUN_NOME
                                            FROM T_CARGA_MOTORISTA CargaMotorista
                                                inner join T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                            WHERE CargaMotorista.CAR_CODIGO = CargaExpedicao.CAR_CODIGO 
                                            FOR XML PATH('')), 
                                        3, 1000) Motorista, ");

                        if (!groupBy.Contains("CargaExpedicao.CAR_CODIGO,"))
                            groupBy.Append("CargaExpedicao.CAR_CODIGO, ");

                        SetarJoinsCargaExpedicao(joins);
                    }
                    break;

                case "ChegadaFilial":
                case "ChegadaFilialFormatada":
                    if (!select.Contains(" ChegadaFilial, "))
                    {
                        select.Append("PedidoExpedicao.PED_DATA_AGENDAMENTO ChegadaFilial, ");
                        groupBy.Append("PedidoExpedicao.PED_DATA_AGENDAMENTO, ");

                        SetarJoinsPedidoExpedicao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND Recebimento.RME_TIPO_RECEBIMENTO = 2 ");

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedido))
            {
                where.Append($" AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}' ");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.Carga > 0)
            {
                where.Append($" AND Carga.CAR_CODIGO = {filtrosPesquisa.Carga} ");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.ProdutoEmbarcador > 0)
            {
                where.Append($" AND Produto.PRO_CODIGO = { filtrosPesquisa.ProdutoEmbarcador} ");

                SetarJoinsProduto(joins);
            }

            if (filtrosPesquisa.DataPedidoInicial != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataPedidoInicial.ToString("yyyy-MM-dd")}' ");

                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.DataPedidoFinal != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_CRIACAO < '{filtrosPesquisa.DataPedidoFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.DataRecebimentoInicial != DateTime.MinValue)
                where.Append($" AND Recebimento.RME_DATA >= '{filtrosPesquisa.DataRecebimentoInicial.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataRecebimentoFinal != DateTime.MinValue)
                where.Append($" AND Recebimento.RME_DATA < '{filtrosPesquisa.DataRecebimentoFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");
        }

        #endregion
    }
}
