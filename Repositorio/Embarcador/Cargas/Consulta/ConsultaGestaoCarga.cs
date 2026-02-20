using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaGestaoCarga : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga>
    {
        #region Construtores

        public ConsultaGestaoCarga() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados        

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT OUTER JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" LEFT OUTER JOIN T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Pedido.TCG_CODIGO");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append(" LEFT OUTER JOIN T_FAIXA_TEMPERATURA FaixaTemperatura on FaixaTemperatura.FTE_CODIGO = TipoCarga.FTE_CODIGO");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append(" JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
        }

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.Append(" JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO");
        }

        private void SetarJoinsNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" NotaFiscal "))
                joins.Append(" JOIN T_XML_NOTA_FISCAL NotaFiscal on NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" LEFT OUTER JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CTe "))
                joins.Append(" LEFT OUTER JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES Origem on Origem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO");
        }

        private void SetarJoinsPaisOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PaisOrigem "))
                joins.Append(" LEFT OUTER JOIN T_PAIS PaisOrigem on PaisOrigem.PAI_CODIGO = Origem.PAI_CODIGO");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            if (!joins.Contains(" Destino "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO");
        }

        private void SetarJoinsPaisDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PaisDestino "))
                joins.Append(" LEFT OUTER JOIN T_PAIS PaisDestino on PaisDestino.PAI_CODIGO = Destino.PAI_CODIGO");
        }

        private void SetarJoinsCavalo(StringBuilder joins)
        {
            if (!joins.Contains(" Cavalo "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO Cavalo on Cavalo.VEI_CODIGO = Carga.CAR_VEICULO");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Tomador.GRP_CODIGO");
        }

        private void SetarJoinsOrigemEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" OrigemEntrega "))
                joins.Append(@" LEFT OUTER JOIN (SELECT T.Etapa, T.Inicio, T.Fim, T.Total, (T.Freetime * 60) Freetime, 
		                    CASE 
		                    WHEN Total > (T.Freetime * 60) then Total - (T.Freetime * 60)
		                    ELSE 0
		                    END Exedente, T.CodigoCarga
		                    FROM  (
		                    SELECT CargaEntrega.CEN_ORDEM_REALIZADA Ordem,
		                    CASE 
		                    WHEN CEN_COLETA = 1 THEN 'Coleta'
		                    WHEN CEN_FRONTEIRA = 1 THEN Cliente.CLI_NOME
                            WHEN CEN_PARQUEAMENTO = 1 THEN 'Parqueamento'
		                    ELSE 'Entrega'
		                    END Etapa,
		                    CargaEntrega.CEN_DATA_INICIO_ENTREGA Inicio,
		                    CargaEntrega.CEN_DATA_FIM_ENTREGA Fim,
		                    datediff(minute, CargaEntrega.CEN_DATA_INICIO_ENTREGA , CargaEntrega.CEN_DATA_FIM_ENTREGA) Total,
		                    CASE
		                    WHEN CEN_COLETA = 1 THEN ISNULL(Rota.ROF_TEMPO_CARREGAMENTO_TICKS, 0) / 36000000000
		                    WHEN CEN_FRONTEIRA = 1 THEN ISNULL((SELECT TOP(1) Fronteira.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA FROM T_ROTA_FRETE_FRONTEIRA Fronteira where Fronteira.ROF_CODIGO = Rota.ROF_CODIGO and Fronteira.CLI_CGCCPF = Cliente.CLI_CGCCPF), 0) / 60
		                    ELSE ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000
		                    END Freetime, CargaTempo.CAR_CODIGO CodigoCarga
			                    FROM T_CARGA_ENTREGA CargaEntrega
			                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
			                    JOIN T_CARGA CargaTempo on CargaTempo.CAR_CODIGO = CargaEntrega.CAR_CODIGO
			                    LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = CargaTempo.ROF_CODIGO
		                    WHERE CargaEntrega.CEN_ORDEM_REALIZADA = 0 AND CargaEntrega.CEN_COLETA = 1) AS T) OrigemEntrega on OrigemEntrega.CodigoCarga = Carga.CAR_CODIGO");
        }

        private void SetarJoinsFronteiraOrigemEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" FronteiraOrigemEntrega "))
                joins.Append(@" LEFT OUTER JOIN (SELECT T.Etapa, T.Inicio, T.Fim, T.Total, (T.Freetime * 60) Freetime, 
		            CASE 
		            WHEN Total > (T.Freetime * 60) then Total - (T.Freetime * 60)
		            ELSE 0
		            END Exedente, T.CodigoCarga
		            FROM  (
		            SELECT CargaEntrega.CEN_ORDEM_REALIZADA Ordem,
		            CASE 
		            WHEN CEN_COLETA = 1 THEN 'Coleta'
		            WHEN CEN_FRONTEIRA = 1 THEN Cliente.CLI_NOME
                    WHEN CEN_PARQUEAMENTO = 1 THEN 'Parqueamento'
		            ELSE 'Entrega'
		            END Etapa,
		            CargaEntrega.CEN_DATA_INICIO_ENTREGA Inicio,
		            CargaEntrega.CEN_DATA_FIM_ENTREGA Fim,
		            datediff(minute, CargaEntrega.CEN_DATA_INICIO_ENTREGA , CargaEntrega.CEN_DATA_FIM_ENTREGA) Total,
		            CASE
		            WHEN CEN_COLETA = 1 THEN ISNULL(Rota.ROF_TEMPO_CARREGAMENTO_TICKS, 0) / 36000000000
		            WHEN CEN_FRONTEIRA = 1 THEN ISNULL((SELECT TOP(1) Fronteira.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA FROM T_ROTA_FRETE_FRONTEIRA Fronteira where Fronteira.ROF_CODIGO = Rota.ROF_CODIGO and Fronteira.CLI_CGCCPF = Cliente.CLI_CGCCPF), 0) / 60
		            ELSE ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000
		            END Freetime, Carga.CAR_CODIGO CodigoCarga
			            FROM T_CARGA_ENTREGA CargaEntrega
			            JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
			            JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
			            LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = Carga.ROF_CODIGO
		            WHERE CargaEntrega.CEN_ORDEM_REALIZADA = 1 AND CargaEntrega.CEN_FRONTEIRA = 1) AS T) FronteiraOrigemEntrega on FronteiraOrigemEntrega.CodigoCarga = Carga.CAR_CODIGO");
        }

        private void SetarJoinsFronteiraDestinoEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" FronteiraDestinoEntrega "))
                joins.Append(@" LEFT OUTER JOIN (SELECT T.Etapa, T.Inicio, T.Fim, T.Total, (T.Freetime * 60) Freetime, 
		                CASE 
		                WHEN Total > (T.Freetime * 60) then Total - (T.Freetime * 60)
		                ELSE 0
		                END Exedente, T.CodigoCarga
		                FROM  (
		                SELECT CargaEntrega.CEN_ORDEM_REALIZADA Ordem,
		                CASE 
		                WHEN CEN_COLETA = 1 THEN 'Coleta'
		                WHEN CEN_FRONTEIRA = 1 THEN Cliente.CLI_NOME
                        WHEN CEN_PARQUEAMENTO = 1 THEN 'Parqueamento'
		                ELSE 'Entrega'
		                END Etapa,
		                CargaEntrega.CEN_DATA_INICIO_ENTREGA Inicio,
		                CargaEntrega.CEN_DATA_FIM_ENTREGA Fim,
		                datediff(minute, CargaEntrega.CEN_DATA_INICIO_ENTREGA , CargaEntrega.CEN_DATA_FIM_ENTREGA) Total,
		                CASE
		                WHEN CEN_COLETA = 1 THEN ISNULL(Rota.ROF_TEMPO_CARREGAMENTO_TICKS, 0) / 36000000000
		                WHEN CEN_FRONTEIRA = 1 THEN ISNULL((SELECT TOP(1) Fronteira.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA FROM T_ROTA_FRETE_FRONTEIRA Fronteira where Fronteira.ROF_CODIGO = Rota.ROF_CODIGO and Fronteira.CLI_CGCCPF = Cliente.CLI_CGCCPF), 0) / 60
		                ELSE ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000
		                END Freetime, Carga.CAR_CODIGO CodigoCarga
			                FROM T_CARGA_ENTREGA CargaEntrega
			                JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
			                JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
			                LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = Carga.ROF_CODIGO
		                WHERE CargaEntrega.CEN_ORDEM_REALIZADA = 2 AND CargaEntrega.CEN_FRONTEIRA = 1) AS T) FronteiraDestinoEntrega on FronteiraDestinoEntrega.CodigoCarga = Carga.CAR_CODIGO");
        }

        private void SetarJoinsDestinoEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" DestinoEntrega "))
                joins.Append(@" LEFT OUTER JOIN (SELECT T.Etapa, T.Inicio, T.Fim, T.Total, (T.Freetime * 60) Freetime, 
		                        CASE 
		                        WHEN Total > (T.Freetime * 60) then Total - (T.Freetime * 60)
		                        ELSE 0
		                        END Exedente, T.CodigoCarga
		                        FROM  (
		                        SELECT CargaEntrega.CEN_ORDEM_REALIZADA Ordem,
		                        CASE 
		                        WHEN CEN_COLETA = 1 THEN 'Coleta'
		                        WHEN CEN_FRONTEIRA = 1 THEN Cliente.CLI_NOME
                                WHEN CEN_PARQUEAMENTO = 1 THEN 'Parqueamento'
		                        ELSE 'Entrega'
		                        END Etapa,
		                        CargaEntrega.CEN_DATA_INICIO_ENTREGA Inicio,
		                        CargaEntrega.CEN_DATA_FIM_ENTREGA Fim,
		                        datediff(minute, CargaEntrega.CEN_DATA_INICIO_ENTREGA , CargaEntrega.CEN_DATA_FIM_ENTREGA) Total,
		                        CASE
		                        WHEN CEN_COLETA = 1 THEN ISNULL(Rota.ROF_TEMPO_CARREGAMENTO_TICKS, 0) / 36000000000
		                        WHEN CEN_FRONTEIRA = 1 THEN ISNULL((SELECT TOP(1) Fronteira.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA FROM T_ROTA_FRETE_FRONTEIRA Fronteira where Fronteira.ROF_CODIGO = Rota.ROF_CODIGO and Fronteira.CLI_CGCCPF = Cliente.CLI_CGCCPF), 0) / 60
		                        ELSE ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000
		                        END Freetime, Carga.CAR_CODIGO CodigoCarga
			                        FROM T_CARGA_ENTREGA CargaEntrega
			                        JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
			                        JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
			                        LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = Carga.ROF_CODIGO
		                        WHERE CargaEntrega.CEN_FRONTEIRA = 0 and CargaEntrega.CEN_COLETA = 0) AS T) DestinoEntrega on DestinoEntrega.CodigoCarga = Carga.CAR_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Tipo":
                    if (!select.Contains("Tipo, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO Tipo, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;
                case "Status":
                    if (!select.Contains("Status, "))
                    {
                        select.Append(@"CASE
                                        WHEN CargaCTe.CAR_CODIGO IS NULL THEN 'PENDENTE'
										WHEN DestinoEntrega.Fim IS NOT NULL THEN 'FINALIZADO'
										WHEN DestinoEntrega.Inicio IS NOT NULL AND DestinoEntrega.Fim IS NULL THEN 'EM DESCARGA'
										WHEN OrigemEntrega.Fim IS NOT NULL THEN 'EM VIAGEM'
										ELSE 'EM CARREGAMENTO'
										END Status, ");

                        SetarJoinsDestinoEntrega(joins);
                        SetarJoinsOrigemEntrega(joins);
                    }
                    break;
                case "Grupo":
                    if (!select.Contains(" Grupo, "))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO Grupo, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsTomador(joins);
                        SetarJoinsGrupoPessoa(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains("Filial, "))
                    {
                        select.Append("Empresa.EMP_CODIGO_INTEGRACAO Filial, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "ProgVei":
                    if (!select.Contains("ProgVei, "))
                    {
                        select.Append("CAST(Pedido.PED_NUMERO AS VARCHAR(20)) ProgVei, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains("Remetente, "))
                    {
                        select.Append("Remetente.PCT_NOME Remetente, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsRemetente(joins);
                    }
                    break;
                case "Origem":
                    if (!select.Contains("Origem, "))
                    {
                        select.Append("Origem.LOC_DESCRICAO Origem, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsOrigem(joins);
                    }
                    break;
                case "PaisOrigem":
                    if (!select.Contains("PaisOrigem, "))
                    {
                        select.Append("PaisOrigem.PAI_NOME PaisOrigem, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsOrigem(joins);
                        SetarJoinsPaisOrigem(joins);
                    }
                    break;
                case "FronteiraOrigem":
                    if (!select.Contains("FronteiraOrigem, "))
                    {
                        select.Append("FronteiraOrigemEntrega.Etapa FronteiraOrigem, ");

                        SetarJoinsFronteiraOrigemEntrega(joins);
                    }
                    break;
                case "FronteiraDestino":
                    if (!select.Contains("FronteiraDestino, "))
                    {
                        select.Append("FronteiraDestinoEntrega.Etapa FronteiraDestino, ");

                        SetarJoinsFronteiraDestinoEntrega(joins);
                    }
                    break;
                case "Cavalo":
                    if (!select.Contains("Cavalo, "))
                    {
                        select.Append("Cavalo.VEI_PLACA Cavalo, ");

                        SetarJoinsCavalo(joins);
                    }
                    break;
                case "Carretas":
                    if (!select.Contains("Carretas, "))
                    {
                        select.Append("ISNULL(SUBSTRING((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), '') Carretas, ");
                    }
                    break;
                case "ChegadaOrigem":
                case "ChegadaOrigemFormatado":
                    if (!select.Contains("ChegadaOrigem, "))
                    {
                        select.Append("OrigemEntrega.Inicio ChegadaOrigem, ");

                        SetarJoinsOrigemEntrega(joins);
                    }
                    break;
                case "SaidaOrigem":
                case "SaidaOrigemFormatado":
                    if (!select.Contains("SaidaOrigem, "))
                    {
                        select.Append("OrigemEntrega.Fim SaidaOrigem, ");

                        SetarJoinsOrigemEntrega(joins);
                    }
                    break;
                case "TempoOrigem":
                case "TempoOrigemFormatado":
                    if (!select.Contains("TempoOrigem, "))
                    {
                        select.Append("OrigemEntrega.Total TempoOrigem, ");

                        SetarJoinsOrigemEntrega(joins);
                    }
                    break;
                case "ChegadaFronteiraOrigem":
                case "ChegadaFronteiraOrigemFormatado":
                    if (!select.Contains("ChegadaFronteiraOrigem, "))
                    {
                        select.Append("FronteiraOrigemEntrega.Inicio ChegadaFronteiraOrigem, ");

                        SetarJoinsFronteiraOrigemEntrega(joins);
                    }
                    break;
                case "SaidaFronteiraOrigem":
                case "SaidaFronteiraOrigemFormatado":
                    if (!select.Contains("SaidaFronteiraOrigem, "))
                    {
                        select.Append("FronteiraOrigemEntrega.Fim SaidaFronteiraOrigem, ");

                        SetarJoinsFronteiraOrigemEntrega(joins);
                    }
                    break;
                case "TempoFronteiraOrigem":
                case "TempoFronteiraOrigemFormatado":
                    if (!select.Contains("TempoFronteiraOrigem, "))
                    {
                        select.Append("FronteiraOrigemEntrega.Total TempoFronteiraOrigem, ");

                        SetarJoinsFronteiraOrigemEntrega(joins);
                    }
                    break;
                case "ChegadaFronteiraDestino":
                case "ChegadaFronteiraDestinoFormatado":
                    if (!select.Contains("ChegadaFronteiraDestino, "))
                    {
                        select.Append("FronteiraDestinoEntrega.Inicio ChegadaFronteiraDestino, ");

                        SetarJoinsFronteiraDestinoEntrega(joins);
                    }
                    break;
                case "SaidaFronteiraDestino":
                case "SaidaFronteiraDestinoFormatado":
                    if (!select.Contains("SaidaFronteiraDestino, "))
                    {
                        select.Append("FronteiraDestinoEntrega.Fim SaidaFronteiraDestino, ");

                        SetarJoinsFronteiraDestinoEntrega(joins);
                    }
                    break;
                case "TempoFronteiraDestino":
                case "TempoFronteiraDestinoFormatado":
                    if (!select.Contains("TempoFronteiraDestino, "))
                    {
                        select.Append("FronteiraDestinoEntrega.Total TempoFronteiraDestino, ");

                        SetarJoinsFronteiraDestinoEntrega(joins);
                    }
                    break;
                case "ChegadaDestino":
                case "ChegadaDestinoFormatado":
                    if (!select.Contains("ChegadaDestino, "))
                    {
                        select.Append("DestinoEntrega.Inicio ChegadaDestino, ");

                        SetarJoinsDestinoEntrega(joins);
                    }
                    break;
                case "SaidaDestino":
                case "SaidaDestinoFormatado":
                    if (!select.Contains("SaidaDestino, "))
                    {
                        select.Append("DestinoEntrega.Fim SaidaDestino, ");

                        SetarJoinsDestinoEntrega(joins);
                    }
                    break;
                case "TempoDestino":
                case "TempoDestinoFormatado":
                    if (!select.Contains("TempoDestino, "))
                    {
                        select.Append("DestinoEntrega.Total TempoDestino, ");

                        SetarJoinsDestinoEntrega(joins);
                    }
                    break;
                case "TempoViagem":
                case "TempoViagemFormatado":
                    if (!select.Contains("TempoViagem, "))
                    {
                        select.Append("DATEDIFF(hour, OrigemEntrega.Inicio, DestinoEntrega.Fim) TempoViagem, ");

                        SetarJoinsOrigemEntrega(joins);
                        SetarJoinsDestinoEntrega(joins);
                    }
                    break;
                case "VlrDiaria":
                    if (!select.Contains("VlrDiaria, "))
                    {
                        select.Append("0.0 VlrDiaria, ");
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains("NumeroPedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedido(joins);
                    }
                    break;
                case "NumeroPedido":
                    if (!select.Contains("NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO NumeroPedido, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedido(joins);
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains("Destinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME Destinatario, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;
                case "Destino":
                    if (!select.Contains("Destino, "))
                    {
                        select.Append("Destino.LOC_DESCRICAO Destino, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsDestinatario(joins);
                        SetarJoinsDestino(joins);
                    }
                    break;
                case "PaisDestino":
                    if (!select.Contains("PaisDestino, "))
                    {
                        select.Append("PaisDestino.PAI_NOME PaisDestino, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                        SetarJoinsDestinatario(joins);
                        SetarJoinsDestino(joins);
                        SetarJoinsPaisDestino(joins);
                    }
                    break;
                case "PrevisaoEmbarque":
                case "PrevisaoEmbarqueFormatado":
                    if (!select.Contains("PrevisaoEmbarque, "))
                    {
                        select.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO PrevisaoEmbarque, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedido(joins);
                    }
                    break;
                case "ValorFrete":
                    if (!select.Contains("ValorFrete, "))
                    {
                        select.Append("CTe.CON_VALOR_FRETE ValorFrete, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                    }
                    break;
                case "Produto":
                    if (!select.Contains("Produto, "))
                    {
                        select.Append("(TipoCarga.TCG_DESCRICAO + ' Temp. ' + CAST(isnull(FaixaTemperatura.FTE_FAIXA_INICIAL, 0) as nvarchar(max)) + ' a ' + CAST(isnull(FaixaTemperatura.FTE_FAIXA_FINAL, 0) as nvarchar(max))) Produto, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedido(joins);
                        SetarJoinsTipoCarga(joins);
                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;
                case "Temperatura":
                    if (!select.Contains("Temperatura, "))
                    {
                        select.Append("FaixaTemperatura.FTE_DESCRICAO Temperatura, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedido(joins);
                        SetarJoinsTipoCarga(joins);
                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains("Motoristas, "))
                    {
                        select.Append("SUBSTRING((SELECT ', ' + motorista1.FUN_NOME + (CASE WHEN motorista1.FUN_FONE is null or motorista1.FUN_FONE = '' THEN '' ELSE ' (' + motorista1.FUN_FONE  + ')' END) FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motoristas, ");
                    }
                    break;
                case "DataUltimaPosicao":
                case "DataUltimaPosicaoFormatado":
                    if (!select.Contains("DataUltimaPosicao, "))
                    {
                        select.Append(@"(SELECT MAX(POA_DATA) DataUltimaPosicao from T_POSICAO_ATUAL 
                                        inner join T_VEICULO on T_VEICULO.VEI_CODIGO = T_POSICAO_ATUAL.VEI_CODIGO
                                        inner join T_RASTREADOR_TECNOLOGIA on T_VEICULO.TRA_CODIGO = T_RASTREADOR_TECNOLOGIA.TRA_CODIGO
				                        WHERE T_VEICULO.VEI_CODIGO = Carga.CAR_VEICULO) DataUltimaPosicao, ");
                    }
                    break;
                case "LocalUltimaPosicao":
                    if (!select.Contains("LocalUltimaPosicao, "))
                    {
                        select.Append(@"(SELECT TOP(1) 'http://maps.google.com/?q=' + LTRIM(STR(POA_LATITUDE, 25, 6)) + ',' + LTRIM(STR(POA_LONGITUDE, 25, 6))  from T_POSICAO_ATUAL 
                                        inner join T_VEICULO on T_VEICULO.VEI_CODIGO = T_POSICAO_ATUAL.VEI_CODIGO
                                        inner join T_RASTREADOR_TECNOLOGIA on T_VEICULO.TRA_CODIGO = T_RASTREADOR_TECNOLOGIA.TRA_CODIGO
				                        WHERE T_VEICULO.VEI_CODIGO = Carga.CAR_VEICULO order by POA_DATA DESC)
				                        LocalUltimaPosicao, ");
                    }
                    break;
                case "DescricaoLocalUltimaPosicao":
                    if (!select.Contains("DescricaoLocalUltimaPosicao, "))
                    {
                        select.Append(@"(SELECT TOP(1) POA_DESCRICAO DataUltimaPosicao from T_POSICAO_ATUAL 
                                        inner join T_VEICULO on T_VEICULO.VEI_CODIGO = T_POSICAO_ATUAL.VEI_CODIGO
                                        inner join T_RASTREADOR_TECNOLOGIA on T_VEICULO.TRA_CODIGO = T_RASTREADOR_TECNOLOGIA.TRA_CODIGO
				                        WHERE T_VEICULO.VEI_CODIGO = Carga.CAR_VEICULO order by POA_DATA DESC) DescricaoLocalUltimaPosicao, ");
                    }
                    break;
                case "DataUltimaOcorrencia":
                case "DataUltimaOcorrenciaFormatado":
                    if (!select.Contains("DataUltimaOcorrencia, "))
                    {
                        select.Append(@"GETDATE() DataUltimaOcorrencia, ");
                    }
                    break;
                case "DescricaoUltimaOcorrencia":
                    if (!select.Contains("DescricaoUltimaOcorrencia, "))
                    {
                        select.Append("'' DescricaoUltimaOcorrencia, ");
                    }
                    break;
                case "NumeroNF":
                    if (!select.Contains("NumeroNF, "))
                    {
                        select.Append("NotaFiscal.NF_NUMERO NumeroNF, ");

                        SetarJoinsCargaPedido(joins);
                        SetarJoinsPedidoNotaFiscal(joins);
                        SetarJoinsNotaFiscal(joins);
                    }
                    break;
                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");

                        SetarJoinsCargaCTe(joins);
                        SetarJoinsCTe(joins);
                    }
                    break;
                default:

                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append($" and Carga.CAR_SITUACAO IN (0,1,2,5,6,9,11)");

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
            {
                SetarJoinsCargaPedido(joins);
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.CRE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) ");
            }

            if (filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
            {
                SetarJoinsCargaCTe(joins);
                SetarJoinsCTe(joins);
                SetarJoinsTomador(joins);
                SetarJoinsGrupoPessoa(joins);
                where.Append($" and (Tomador.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoa)}) or Carga.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoa)}))");
            }

            if (filtrosPesquisa.CNPJsTomador?.Count > 0)
            {
                SetarJoinsCargaCTe(joins);
                SetarJoinsCTe(joins);
                SetarJoinsTomador(joins);
                SetarJoinsGrupoPessoa(joins);
                where.Append($" and Tomador.CLI_CODIGO IN ({string.Join(", ", filtrosPesquisa.CNPJsTomador)}) ");
            }

            if (!filtrosPesquisa.IncluirOperacoesDeslocamentoVazio)
            {
                SetarJoinsTipoOperacao(joins);
                where.Append($" and (TipoOperacao.TOP_DESLOCAMENTO_VAZIO = 0 or TipoOperacao.TOP_DESLOCAMENTO_VAZIO IS NULL) ");
            }

            if (filtrosPesquisa.StatusGestaoCarga == StatusGestaoCarga.Pendente)
                where.Append($" and CargaCTe.CAR_CODIGO IS NULL ");
            else if (filtrosPesquisa.StatusGestaoCarga == StatusGestaoCarga.Finalizado)
                where.Append($" and DestinoEntrega.Fim IS NOT NULL and CargaCTe.CAR_CODIGO IS NOT NULL ");
            else if (filtrosPesquisa.StatusGestaoCarga == StatusGestaoCarga.EmViagem)
                where.Append($" and OrigemEntrega.Fim IS NOT NULL and DestinoEntrega.Fim IS NULL and CargaCTe.CAR_CODIGO IS NOT NULL ");
            else if (filtrosPesquisa.StatusGestaoCarga == StatusGestaoCarga.EmDescarga)
                where.Append($" and DestinoEntrega.Inicio IS NOT NULL AND  DestinoEntrega.Fim IS NULL and CargaCTe.CAR_CODIGO IS NOT NULL ");
            else if (filtrosPesquisa.StatusGestaoCarga == StatusGestaoCarga.EmCarregamento)
                where.Append($" and OrigemEntrega.Fim IS NULL and CargaCTe.CAR_CODIGO IS NOT NULL ");

            if (filtrosPesquisa.NumeroNF > 0)
            {

                if (joins.Contains(" NotaFiscal "))
                {
                    where.Append($" and NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNF} ");
                }
                else
                {
                    where.Append(" and Carga.CAR_CODIGO in (");
                    where.Append("         select _cargapedido.CAR_CODIGO ");
                    where.Append("           from T_CARGA_PEDIDO _cargapedido");
                    where.Append("           inner join T_PEDIDO_XML_NOTA_FISCAL _pex on _pex.CPE_CODIGO = _cargapedido.CPE_CODIGO ");
                    where.Append("           inner join T_XML_NOTA_FISCAL _nfx on _nfx.NFX_CODIGO = _pex.NFX_CODIGO ");
                    where.Append($"         where _nfx.NF_NUMERO = {filtrosPesquisa.NumeroNF} ");
                    where.Append("     ) ");
                }
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargaCTe.CAR_CODIGO ");
                where.Append("           from T_CARGA_CTE _cargaCTe");
                where.Append("           inner join T_CTE _cte on _cte.CON_CODIGO = _cargaCTe.CON_CODIGO ");
                where.Append($"         where _cte.CON_NUM = {filtrosPesquisa.NumeroCTe} ");
                where.Append("     ) ");
            }
        }

        #endregion
    }
}
