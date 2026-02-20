using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    using System.Collections.Generic;

    public class MonitoramentoConstantesConsulta
    {

        public const string CTE_CARGA_MAIS_RECENTE = @"CargaMaisRecente AS (
    SELECT 
        Monitoramento.MON_CODIGO,
        Monitoramento.CAR_CODIGO,
        Carga.CAR_DATA_CRIACAO,
        ROW_NUMBER() OVER (PARTITION BY Monitoramento.CAR_CODIGO ORDER BY Carga.CAR_DATA_CRIACAO DESC) AS RowNum
    FROM T_MONITORAMENTO Monitoramento
    JOIN T_VEICULO Veiculo ON Monitoramento.VEI_CODIGO = Veiculo.VEI_CODIGO
    LEFT JOIN T_CARGA Carga ON Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
   
)";

        public const string CTE_HISTORICO_MONITORAMENTO_STATUS = @"
HistoricoMonitoramentoStatus AS (
    SELECT 
        HistoricoMonitoramento.MON_CODIGO,
        HistoricoMonitoramento.MSV_CODIGO,
        MIN(HistoricoMonitoramento.MHS_DATA_INICIO) AS DataInicioStatusAtual,
        MIN(HistoricoMonitoramento.MHS_DATA_FIM) AS DataFimStatusAtual
    FROM T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM HistoricoMonitoramento
    GROUP BY HistoricoMonitoramento.MON_CODIGO, HistoricoMonitoramento.MSV_CODIGO
)";

        public const string CTE_ESTATISTICAS_CARGA_ENTREGA_AUX = @"
EstatisticasCargaEntregaAux AS (
    SELECT 
        CargaEntrega.CAR_CODIGO,
        CargaEntrega.CEN_TENDENCIA AS TendenciaEntrega,
        CASE WHEN CargaEntrega.CEN_SITUACAO != 2 AND CargaEntrega.CEN_COLETA = 0 
             THEN CargaEntrega.CEN_DATA_ENTREGA_PREVISTA END AS DataEntregaPlanejadaProximaEntrega
    FROM T_CARGA_ENTREGA CargaEntrega
    WHERE CargaEntrega.CEN_SITUACAO != 2
)";

        public const string CTE_ESTATISTICAS_CARGA_ENTREGA = @"
EstatisticasCargaEntrega AS (
    SELECT 
        CargaEntrega.CAR_CODIGO,
        COUNT(CASE WHEN CargaEntrega.CEN_COLETA = 1 THEN 1 END) AS TotalColetas,
        COUNT(CASE WHEN CargaEntrega.CEN_COLETA = 1 AND CargaEntrega.CEN_SITUACAO = 2 THEN 1 END) + 1 AS ColetaAtual,
        COUNT(CASE WHEN CargaEntrega.CEN_COLETA = 0 THEN 1 END) AS TotalEntregas,
        COUNT(CASE WHEN CargaEntrega.CEN_SITUACAO = 2 THEN 1 END) AS TotalEntregasEntregues,
        COUNT(CASE WHEN CargaEntrega.CEN_SITUACAO = 3 THEN 1 END) AS TotalEntregasRejeitadas,
        SUM(CASE WHEN CargaEntrega.CEN_ORDEM = CargaEntrega.CEN_ORDEM_REALIZADA AND CargaEntrega.CEN_SITUACAO = 2 THEN 1 ELSE 0 END) AS TotalEntregasAderencia,
        SUM(CAST(CargaEntrega.CEN_ENTREGA_NO_RAIO AS INT)) AS TotalEntregasNoRaio,
        COUNT(CASE WHEN CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_SITUACAO = 2 THEN 1 END) + 1 AS EntregaAtual,
        MIN(CASE WHEN CargaEntrega.CEN_SITUACAO != 2 AND CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_POSTO_FISCAL = 0 
                 THEN ISNULL(CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, CargaEntrega.CEN_DATA_ENTREGA_PREVISTA) END) AS DataEntregaReprogramadaProximaEntrega,
        MIN(CASE WHEN CargaEntrega.CEN_SITUACAO != 2 AND CargaEntrega.CEN_COLETA = 0 THEN CargaEntrega.CEN_CODIGO END) AS CodigoProximaEntrega,
        MIN(CASE WHEN CargaEntrega.CEN_COLETA = 1 AND CargaEntrega.CEN_DATA_SAIDA_RAIO IS NOT NULL THEN CargaEntrega.CEN_DATA_SAIDA_RAIO END) AS DataSaidaOrigem,
        MIN(CASE WHEN CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_DATA_ENTRADA_RAIO IS NOT NULL THEN CargaEntrega.CEN_DATA_ENTRADA_RAIO END) AS DataChegadaDestino,
        MAX(ISNULL(CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, CargaEntrega.CEN_DATA_ENTREGA_PREVISTA)) AS PrevisaoFimViagem,
        MIN(EstatisticasCargaEntregaAux.TendenciaEntrega) AS TendenciaEntrega,
        MIN(EstatisticasCargaEntregaAux.DataEntregaPlanejadaProximaEntrega) AS DataEntregaPlanejadaProximaEntrega
    FROM T_CARGA_ENTREGA CargaEntrega
    LEFT JOIN EstatisticasCargaEntregaAux ON EstatisticasCargaEntregaAux.CAR_CODIGO = CargaEntrega.CAR_CODIGO
    GROUP BY CargaEntrega.CAR_CODIGO
)";

        public const string CTE_PEDIDOS_CARGA = @"
PedidosCarga AS (
    SELECT 
        cp.CAR_CODIGO,
        (
            SELECT STRING_AGG(CAST(pedidoNum AS NVARCHAR(MAX)), ', ')
            FROM (
                SELECT DISTINCT 
                    SUBSTRING(
                        TRIM(PEDIDO.PED_NUMERO_PEDIDO_EMBARCADOR), 
                        CHARINDEX('_', TRIM(PEDIDO.PED_NUMERO_PEDIDO_EMBARCADOR)) + 1, 
                        LEN(TRIM(PEDIDO.PED_NUMERO_PEDIDO_EMBARCADOR))
                    ) AS pedidoNum
                FROM T_CARGA_PEDIDO cp2
                JOIN T_PEDIDO PEDIDO ON PEDIDO.PED_CODIGO = cp2.PED_CODIGO
                WHERE cp2.CAR_CODIGO = cp.CAR_CODIGO
            ) AS sub
        ) AS Pedidos,
        STRING_AGG(CAST(Pedido.PED_NUMERO_EXP AS NVARCHAR(MAX)), ', ') AS NumeroEXP,
        STRING_AGG(CAST(RTRIM(LTRIM(Pedido.PED_ORDEM)) AS NVARCHAR(MAX)), ', ') AS Ordens,
        SUM(NotaFiscal.NF_VALOR) AS ValorTotalNFe,
        MAX(Pedido.PED_PREVISAO_ENTREGA) AS DataPrevisaoEntregaPedido,
        MAX(Pedido.PED_DATA_INICIAL_COLETA) AS DataProgramadaColeta,
        (
            SELECT TOP 1 CargaPedidoAux.TBF_TIPO_COBRANCA_MULTIMODAL 
            FROM T_CARGA_PEDIDO CargaPedidoAux 
            WHERE CargaPedidoAux.CAR_CODIGO = cp.CAR_CODIGO
        ) AS TipoModalTransporte,
        STRING_AGG(CAST(CentroResultado.CRE_DESCRICAO AS NVARCHAR(MAX)), ', ') AS CentroResultado,
        MIN(Pedido.PEP_DATA_PREVISAO_SAIDA_DESTINATARIO) AS PrevisaoSaidaDestino
    FROM T_CARGA_PEDIDO cp
    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = cp.PED_CODIGO
    LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = cp.CPE_CODIGO
    LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
    LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO
    GROUP BY cp.CAR_CODIGO
)";

        public const string CTE_DADOS_MOTORISTAS = @"
DadosMotoristas AS (
    SELECT 
        CargaMotorista.CAR_CODIGO,
        STRING_AGG(CAST(Motorista.FUN_NOME  AS NVARCHAR(MAX)) , ', ') AS Motoristas,
        STRING_AGG(CAST(Motorista.FUN_CPF AS NVARCHAR(MAX)), ', ') AS CPFMotoristas,
        STRING_AGG(CAST(Motorista.FUN_VERSAO_APP AS NVARCHAR(MAX)), ', ') AS VersaoAppMotorista
    FROM T_CARGA_MOTORISTA CargaMotorista
    JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
    GROUP BY CargaMotorista.CAR_CODIGO
)";

        public const string CTE_REBOQUES_VINCULADOS = @"
ReboquesVinculados AS (
    SELECT 
        CargaVeiculos.CAR_CODIGO,
        STRING_AGG(CAST(Veiculo.VEI_PLACA AS NVARCHAR(MAX)), ', ') AS Reboques
    FROM T_CARGA_VEICULOS_VINCULADOS CargaVeiculos
    JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = CargaVeiculos.VEI_CODIGO
    GROUP BY CargaVeiculos.CAR_CODIGO
)";

        public const string CTE_DESTINOS_ENTREGA_AUX = @"
DestinosEntregaAux AS (
    SELECT 
        CargaEntregaDestino.CAR_CODIGO,
        CASE WHEN ISNULL(Cliente.CLI_NOMEFANTASIA, '') = '' THEN Cliente.CLI_NOME ELSE Cliente.CLI_NOMEFANTASIA END AS ProximoDestino
    FROM T_CARGA_ENTREGA CargaEntregaDestino
    JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntregaDestino.CLI_CODIGO_ENTREGA
    WHERE CargaEntregaDestino.CEN_SITUACAO != 2 AND CargaEntregaDestino.CEN_COLETA = 0
)";

        public const string CTE_DESTINOS_ENTREGA = @"
DestinosEntrega AS (
    SELECT 
        CargaEntregaDestino.CAR_CODIGO,
        STRING_AGG(CAST(CASE WHEN ISNULL(Cliente.CLI_NOMEFANTASIA, '') = '' 
                        THEN  Cliente.CLI_NOME ELSE Cliente.CLI_NOMEFANTASIA END 
                   + ' (' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA + ')' AS NVARCHAR(MAX)), ', ') AS Destinos,
        STRING_AGG(CAST(Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA AS NVARCHAR(MAX)), ', ') AS CidadeDestino,
        MIN(DestinosEntregaAux.ProximoDestino) AS ProximoDestino
    FROM T_CARGA_ENTREGA CargaEntregaDestino
    JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntregaDestino.CLI_CODIGO_ENTREGA
    JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Cliente.LOC_CODIGO
    LEFT JOIN DestinosEntregaAux ON DestinosEntregaAux.CAR_CODIGO = CargaEntregaDestino.CAR_CODIGO
    WHERE CargaEntregaDestino.CEN_COLETA = 0
    GROUP BY CargaEntregaDestino.CAR_CODIGO
)";

        public const string CTE_CATEGORIAS_CLIENTES = @"
CategoriasClientes AS (
    SELECT 
        PosicaoAlvo.POS_CODIGO,
        STRING_AGG(CAST(CategoriaCliente.CTP_DESCRICAO AS NVARCHAR(MAX)), ', ') AS Categorias
    FROM T_POSICAO_ALVO PosicaoAlvo
    JOIN T_CLIENTE ClienteAlvo ON ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF                                           
    LEFT JOIN T_CATEGORIA_PESSOA CategoriaCliente ON CategoriaCliente.CTP_CODIGO = ClienteAlvo.CTP_CODIGO 
    GROUP BY PosicaoAlvo.POS_CODIGO
)";

        public const string CTE_ULTIMAS_OCORRENCIAS = @"
UltimasOcorrencias AS (
    SELECT 
        CargaEntregaOcorrencia.CAR_CODIGO,
        MAX(Ocorrencia.OCO_DESCRICAO + ' - (' + Ocorrencia.OCO_DESCRICAO_PORTAL + ')') AS UltimaOcorrencia
    FROM T_CARGA_ENTREGA CargaEntregaOcorrencia
    INNER JOIN T_OCORRENCIA_COLETA_ENTREGA OcorrenciaColetaEntrega ON CargaEntregaOcorrencia.CEN_CODIGO = OcorrenciaColetaEntrega.CEN_CODIGO
    INNER JOIN T_OCORRENCIA Ocorrencia ON OcorrenciaColetaEntrega.OCO_CODIGO = Ocorrencia.OCO_CODIGO
    GROUP BY CargaEntregaOcorrencia.CAR_CODIGO
)";

        public const string CTE_RESPONSAVEL_VEICULO = @"
ResponsavelVeiculo AS (
    SELECT 
        Veiculo.VEI_CODIGO,
        Funcionario.FUN_NOME AS NomeCompletoResponsavelVeiculo,
        Funcionario.FUN_CPF AS CPFResponsavelVeiculo
    FROM T_VEICULO Veiculo
    LEFT JOIN T_FUNCIONARIO Funcionario ON Veiculo.FUN_CODIGO_RESPONSAVEL = Funcionario.FUN_CODIGO
)";

        public const string CTE_NOTAS_FISCAIS_CARGA = @"
NotasFiscaisCarga AS (
    SELECT 
        CargaPedidoNota.CAR_CODIGO,
        STRING_AGG(CAST(NotaFiscalCarga.NF_NUMERO AS nvarchar(max)), ', ') AS ListaNotasFiscais
    FROM T_CARGA_PEDIDO CargaPedidoNota
    LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedidoNota.CPE_CODIGO
    LEFT JOIN T_XML_NOTA_FISCAL NotaFiscalCarga ON NotaFiscalCarga.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
    GROUP BY CargaPedidoNota.CAR_CODIGO
)";

        public const string CTE_CONTRATOS_FRETE = @"
ContratosFrete AS (
    SELECT DISTINCT
        ContratoFreteTransportadorVeiculo.VEI_CODIGO,
        1 AS PossuiContratoFrete
    FROM T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
    JOIN T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo 
        ON ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
    WHERE ContratoFreteTransportador.CFT_ATIVO = 1
        AND ContratoFreteTransportador.CFT_SITUACAO IN (1, 2)
        AND CURRENT_TIMESTAMP BETWEEN ContratoFreteTransportador.CFT_DATA_INICIAL AND ContratoFreteTransportador.CFT_DATA_FINAL
)";

        public const string CTE_ORIGEM_CARGA_AUX = @"
OrigemCargaAux AS (
    SELECT 
        CargaPedido.CAR_CODIGO,
        CASE 
            WHEN CargaPedido.CLI_CODIGO_EXPEDIDOR IS NOT NULL 
                 THEN CASE WHEN ISNULL(Cliente1.CLI_NOMEFANTASIA, '') = '' 
                           THEN Cliente1.CLI_NOME ELSE Cliente1.CLI_NOMEFANTASIA END
            ELSE CASE WHEN ISNULL(Cliente2.CLI_NOMEFANTASIA, '') = '' 
                      THEN Cliente2.CLI_NOME ELSE Cliente2.CLI_NOMEFANTASIA END
        END AS ClienteOrigem,
        CASE 
            WHEN CargaPedido.CLI_CODIGO_EXPEDIDOR IS NOT NULL 
                 THEN Localidade1.LOC_DESCRICAO + '/' + Localidade1.UF_SIGLA
            ELSE Localidade2.LOC_DESCRICAO + '/' + Localidade2.UF_SIGLA
        END AS CidadeOrigem
    FROM T_CARGA_PEDIDO CargaPedido
    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
    LEFT JOIN T_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
    LEFT JOIN T_LOCALIDADES Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
    LEFT JOIN T_CLIENTE Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
    LEFT JOIN T_LOCALIDADES Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
)";

        public const string CTE_ORIGEM_CARGA = @"
OrigemCarga AS (
    SELECT 
        CAR_CODIGO,
        MAX(ClienteOrigem) AS ClienteOrigem,
        MAX(CidadeOrigem) AS CidadeOrigem
    FROM OrigemCargaAux
    GROUP BY CAR_CODIGO
)";

        public const string CTE_FRONTEIRA_ROTA = @"
FronteiraRota AS (
    SELECT 
        CargaFronteira.CAR_CODIGO,
        STRING_AGG(CAST(ClienteFronteira.CLI_NOME AS NVARCHAR(MAX)), ', ') AS FronteiraRotaFrete
    FROM T_CARGA_FRONTEIRA CargaFronteira
    LEFT JOIN T_CLIENTE ClienteFronteira ON ClienteFronteira.CLI_CGCCPF = CargaFronteira.CLI_CGCCPF
    GROUP BY CargaFronteira.CAR_CODIGO
)";

        public const string CTE_ALERTAS_CARGA_AUX = @"
AlertasCargaAux AS (
    SELECT 
        Alerta.CAR_CODIGO,
        MonitoramentoEvento.MEV_DESCRICAO,
        COUNT(*) AS QtdEventos
    FROM T_ALERTA_MONITOR Alerta
    INNER JOIN T_MONITORAMENTO_EVENTO MonitoramentoEvento 
        ON MonitoramentoEvento.MEV_TIPO_ALERTA = Alerta.ALE_TIPO
    WHERE MonitoramentoEvento.MEV_ATIVO = 1
    GROUP BY Alerta.CAR_CODIGO, MonitoramentoEvento.MEV_DESCRICAO
)";

        public const string CTE_ALERTAS_CARGA = @"
AlertasCarga AS (
    SELECT 
        AlertaMonitor.CAR_CODIGO,
        COUNT(CASE WHEN EventoMonitoramento.MEV_CONSIDERAR_SEMAFORO = 1 THEN 1 END) AS TotalAlertas,
        COUNT(CASE WHEN EventoMonitoramento.MEV_CONSIDERAR_SEMAFORO = 1 AND TratativaAlerta.ALE_CODIGO IS NOT NULL THEN 1 END) AS TotalAlertasTratados,
        COUNT(CASE WHEN EventoMonitoramento.MEV_CONSIDERAR_SEMAFORO = 1 AND AcaoAlertaTratativa.ATC_ACAO_MONITORADA = 1 THEN 1 END) AS TotalAlertaTratativaEspecifica,
        (
            SELECT STRING_AGG(CAST(AlertaText AS nvarchar(MAX)), ', ')
            FROM (
                SELECT DISTINCT 
                    EventoMonitoramentoSub.MEV_DESCRICAO + ' (' + CAST(COUNT(*) OVER(PARTITION BY EventoMonitoramentoSub.MEV_DESCRICAO) AS NVARCHAR(10)) + ')' AS AlertaText
                FROM T_ALERTA_MONITOR AS AlertaMonitorSub
                INNER JOIN T_MONITORAMENTO_EVENTO AS EventoMonitoramentoSub 
                    ON EventoMonitoramentoSub.MEV_CODIGO = AlertaMonitorSub.MEV_CODIGO AND EventoMonitoramentoSub.MEV_ATIVO = 1
                WHERE AlertaMonitorSub.CAR_CODIGO = AlertaMonitor.CAR_CODIGO
            ) AS SubAlertas
        ) AS Alertas
    FROM T_ALERTA_MONITOR AS AlertaMonitor
    INNER JOIN T_MONITORAMENTO_EVENTO AS EventoMonitoramento 
        ON EventoMonitoramento.MEV_TIPO_ALERTA = AlertaMonitor.ALE_TIPO
    LEFT JOIN T_ALERTA_TRATATIVA AS TratativaAlerta 
        ON AlertaMonitor.ALE_CODIGO = TratativaAlerta.ALE_CODIGO
    LEFT JOIN T_ALERTA_TRATATIVA_ACAO AS AcaoAlertaTratativa 
        ON TratativaAlerta.ATC_CODIGO = AcaoAlertaTratativa.ATC_CODIGO
    GROUP BY AlertaMonitor.CAR_CODIGO
)";

        public const string CTE_JANELA_DESCARREGAMENTO = @"
JanelaDescarregamento AS (
    SELECT 
        Janela.CAR_CODIGO,
        MIN(Janela.CJD_INICIO_DESCARREGAMENTO) AS DataPrevisaoDescargaJanela
    FROM T_CARGA_JANELA_DESCARREGAMENTO Janela
    JOIN T_CENTRO_DESCARREGAMENTO Centro ON Centro.CED_CODIGO = Janela.CED_CODIGO
    JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CAR_CODIGO = Janela.CAR_CODIGO 
         AND Centro.CLI_CGCCPF_DESTINATARIO = CargaEntrega.CLI_CODIGO_ENTREGA
    WHERE ISNULL(Janela.CJD_CANCELADA, 0) = 0
        AND CargaEntrega.CEN_DATA_ENTREGA IS NULL
    GROUP BY Janela.CAR_CODIGO
)";

        public const string CTE_CONTAINER_CARGA = @"
ContainerCarga AS (
    SELECT 
        ColetaContainer.CAR_CODIGO_ATUAL as CAR_CODIGO,
        STRING_AGG(CAST(Container.CTR_NUMERO AS NVARCHAR(MAX)), ', ') As NumeroContainer
    FROM T_COLETA_CONTAINER ColetaContainer
    LEFT JOIN T_CONTAINER Container ON Container.CTR_CODIGO = ColetaContainer.CTR_CODIGO
    WHERE ColetaContainer.CTR_CODIGO IS NOT NULL
    GROUP BY ColetaContainer.CAR_CODIGO_ATUAL 
)";

        public const string CTE_PROTOCOLOS_INTEGRACAO = @"
ProtocolosIntegracao AS (
    SELECT 
        Carga.CAR_CODIGO,
        COALESCE(
            STRING_AGG(CAST(CASE WHEN CargaDadosTransporteIntegracao.CDI_PROTOCOLO IS NOT NULL AND CargaDadosTransporteIntegracao.CDI_PROTOCOLO <> '' THEN CargaDadosTransporteIntegracao.CDI_PROTOCOLO END AS NVARCHAR(MAX)), ', '),
            STRING_AGG(CAST(CASE WHEN CargaCargaIntegracao.CCA_PROTOCOLO IS NOT NULL AND CargaCargaIntegracao.CCA_PROTOCOLO <> '' THEN CargaCargaIntegracao.CCA_PROTOCOLO END AS NVARCHAR(MAX)), ', '),
            ''
        ) AS NumeroProtocoloIntegracaoCarga
    FROM T_CARGA Carga
    LEFT JOIN T_CARGA_DADOS_TRANSPORTE_INTEGRACAO CargaDadosTransporteIntegracao ON Carga.CAR_CODIGO = CargaDadosTransporteIntegracao.CAR_CODIGO
    LEFT JOIN T_CARGA_CARGA_INTEGRACAO CargaCargaIntegracao ON Carga.CAR_CODIGO = CargaCargaIntegracao.CAR_CODIGO
    GROUP BY Carga.CAR_CODIGO
)";

        public const string MAIN_QUERY_SELECT = @"
        SELECT 
            Monitoramento.MON_CODIGO AS Codigo,
            Monitoramento.MON_DATA_CRIACAO AS Data,
            ISNULL(Monitoramento.MON_DATA_INICIO, Monitoramento.MON_DATA_CRIACAO) AS DataInicioMonitoramento,
            Monitoramento.MON_DATA_FIM AS DataFimMonitoramento,
            Monitoramento.MON_DISTANCIA_PREVISTA AS DistanciaPrevista,
            Monitoramento.MON_DISTANCIA_REALIZADA AS DistanciaRealizada,
            Monitoramento.MON_DISTANCIA_ATE_ORIGEM AS DistanciaAteOrigem,
            Monitoramento.MON_DISTANCIA_ATE_DESTINO AS DistanciaAteDestino,
            Monitoramento.MON_DISTANCIA_PREVISTA AS DistanciaTotal,
            Monitoramento.MON_STATUS AS Status,
            Monitoramento.MON_NUMERO_TEMPERATURA_RECEBIDA AS TotalTemperaturasRecebidas,
            Monitoramento.MON_TEMPERATURA_NA_FAIXA AS TotalTemperaturasDentroFaixa,
            MonitoramentoStatus.MSV_DESCRICAO AS StatusViagem,
            MonitoramentoStatus.MSV_TIPO_REGRA AS TiporRegraViagem,
            HistoricoMonitoramentoStatus.DataInicioStatusAtual,
            HistoricoMonitoramentoStatus.DataFimStatusAtual,
            TipoOperacao.TOP_DESCRICAO AS TipoOperacao,
            GrupoTipoOperacao.GTO_DESCRICAO AS GrupoTipoOperacao,
            Monitoramento.MON_PERCENTUAL_VIAGEM AS PercentualViagem,
            Carga.CAR_CODIGO AS Carga,
            Carga.CAR_CODIGO_CARGA_EMBARCADOR AS CargaEmbarcador,
            Carga.CAR_DATA_TERMINO_CARGA AS DataPrevisaoTerminoCarga,
            Carga.CAR_DATA_INICIO_VIAGEM AS DataInicioViagem,
            Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA AS DataInicioViagemPrevista,
            Carga.CAR_DATA_REAGENDAMENTO AS DataReagendamento,
            Carga.CAR_DATA_CARREGAMENTO AS DataCarregamentoCarga,
            CargaDadosSumarizados.CDS_PESO_TOTAL AS PesoTotalCarga,
            CASE WHEN CargaDadosSumarizados.CDS_EXPEDIDORES <> '' 
                 THEN CargaDadosSumarizados.CDS_EXPEDIDORES + '-' + CargaDadosSumarizados.CDS_ORIGENS ELSE '' END AS Expedidor,
            CASE WHEN CargaDadosSumarizados.CDS_RECEBEDORES <> '' 
                 THEN CargaDadosSumarizados.CDS_RECEBEDORES + '-' + CargaDadosSumarizados.CDS_DESTINOS ELSE '' END AS Recebedor,
            JanelaCarregamento.DataInicioCarregamentoJanela,
            Carga.CAR_DATA_FIM_VIAGEM_PREVISTA AS DataPrevisaoChegada,
            Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM AS DataPrevisaoChegadaPlanta,
            Veiculo.VEI_CODIGO AS Veiculo,
            Empresa.EMP_RAZAO AS RazaoSocialTransportador,
            Empresa.EMP_FANTASIA AS NomeFantasiaTransportador,
            Posicao.POS_DESCRICAO AS Posicao,
            Posicao.POS_LATITUDE AS Latitude,
            Posicao.POS_LONGITUDE AS Longitude,
            Posicao.POS_VELOCIDADE AS Velocidade,
            Posicao.POS_IGNICAO AS Ignicao,
            Posicao.POS_ID_EQUIPAMENTO AS IDEquipamento,
            Posicao.POS_TEMPERATURA AS Temperatura,
            Posicao.POS_RASTREADOR AS TecnologiaRastreador,
            Posicao.POS_GERENCIADORA AS Gerenciadora,
            Monitoramento.MON_ULTIMA_TEMPERATURA AS TemperaturaMonitoramento,
            Posicao.POS_NIVEL_SINAL_GPS AS NivelGPS,
            FaixaTemperatura.FTE_DESCRICAO AS DescricaoFaixaTemperatura,
            FaixaTemperatura.FTE_FAIXA_INICIAL AS TemperaturaFaixaInicial,
            FaixaTemperatura.FTE_FAIXA_FINAL AS TemperaturaFaixaFinal,
            Filial.FIL_CODIGO_FILIAL_EMBARCADOR AS CodigoFilial,
            Filial.FIL_DESCRICAO AS Filial,
            Posicao.POS_DATA_VEICULO AS DataPosicaoAtual,
            Veiculo.VEI_PLACA AS Tracao,
            Monitoramento.MON_OBSERVACAO AS Observacao,
            TipoTrecho.TTR_DESCRICAO AS TipoTrecho,
            Terceiro.CLI_NOME AS Subcontratado,
            ReboquesVinculados.Reboques,
            DadosMotoristas.Motoristas,
            DadosMotoristas.CPFMotoristas,
            DestinosEntrega.Destinos,
            DestinosEntrega.CidadeDestino,
            EstatisticasCargaEntrega.TotalColetas,
            EstatisticasCargaEntrega.ColetaAtual,
            EstatisticasCargaEntrega.TotalEntregas,
            EstatisticasCargaEntrega.TotalEntregasEntregues,
            EstatisticasCargaEntrega.TotalEntregasRejeitadas,
            EstatisticasCargaEntrega.TotalEntregasAderencia,
            EstatisticasCargaEntrega.TotalEntregasNoRaio,
            EstatisticasCargaEntrega.EntregaAtual,
            EstatisticasCargaEntrega.DataEntregaReprogramadaProximaEntrega,
            EstatisticasCargaEntrega.CodigoProximaEntrega,
            EstatisticasCargaEntrega.DataSaidaOrigem,
            EstatisticasCargaEntrega.DataChegadaDestino,
            EstatisticasCargaEntrega.PrevisaoFimViagem,
            PedidosCarga.Pedidos,
            PedidosCarga.NumeroEXP,
            PedidosCarga.Ordens,
            PedidosCarga.ValorTotalNFe,
            PedidosCarga.DataPrevisaoEntregaPedido,
            PedidosCarga.DataProgramadaColeta,
            PedidosCarga.TipoModalTransporte,
            DadosMotoristas.VersaoAppMotorista,
            CargaDadosSumarizados.CDS_NUMERO_PEDIDO_EMBARCADOR AS NumeroPedidoEmbarcadorSumarizado,
            Carga.CAR_DATA_PREVISAO_TERMINO_VIAGEM AS PrevisaoTerminoViagem,
            Carga.CAR_DATA_PREVISAO_STOP_TRACKING AS PrevisaoStopTranking,
            _ConfiguracaoTMS.TempoSemPosicaoParaVeiculoPerderSinal,
            _ConfiguracaoTMS.DataBaseCalculoPrevisaoControleEntrega,
            CategoriasClientes.Categorias AS CategoriasAlvos,
            Carga.CAR_DATA_CRIACAO AS DataCriacaoCarga,
            UltimasOcorrencias.UltimaOcorrencia AS UltimaOcorrencia,
            ResponsavelVeiculo.NomeCompletoResponsavelVeiculo AS NomeResponsavelVeiculo,
            ResponsavelVeiculo.CPFResponsavelVeiculo AS CPFResponsavelVeiculo,
            NotasFiscaisCarga.ListaNotasFiscais AS NotasFiscais,
            EstatisticasCargaEntrega.TendenciaEntrega,
            DestinosEntrega.ProximoDestino,
            CASE WHEN CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS <> '' 
                 THEN CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS + ' - ' + CargaDadosSumarizados.CDS_DESTINOS ELSE '' END AS CodigoIntegracaoDestino,
            EstatisticasCargaEntrega.DataEntregaPlanejadaProximaEntrega,
            JanelaDescarregamento.DataPrevisaoDescargaJanela,
            ContratosFrete.PossuiContratoFrete,
            CAST(Monitoramento.MON_CRITICO AS BIT) AS Critico,
            OrigemCarga.ClienteOrigem,
            OrigemCarga.CidadeOrigem,
            PedidosCarga.CentroResultado,
            FronteiraRota.FronteiraRotaFrete,
            AlertasCarga.TotalAlertas,
            AlertasCarga.TotalAlertasTratados,
            AlertasCarga.TotalAlertaTratativaEspecifica,
            AlertasCarga.Alertas,
            Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR AS NumeroEquipamentoRastreador,
            TecnologiaRastreador.TRA_TIPO_INTEGRACAO AS TipoIntegracaoTecnologiaRastreador,
            ProtocolosIntegracao.NumeroProtocoloIntegracaoCarga,
            ContainerCarga.NumeroContainer,
            Veiculo.VEI_NUMERO_FROTA AS NumeroFrota,
            PedidosCarga.PrevisaoSaidaDestino
        ";

        public static string ObterJoinsMonitoramento(bool somenteUltimoCarga)
        {
            string joinCrossApply = somenteUltimoCarga ? @" JOIN CargaMaisRecente ON CargaMaisRecente.MON_CODIGO = Monitoramento.MON_CODIGO AND CargaMaisRecente.RowNum = 1" : string.Empty;

            return @$"
            {joinCrossApply}
            JOIN T_VEICULO Veiculo ON Monitoramento.VEI_CODIGO = Veiculo.VEI_CODIGO
            LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatus ON MonitoramentoStatus.MSV_CODIGO = Monitoramento.MSV_CODIGO
            LEFT JOIN T_CARGA Carga ON Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados ON Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO
            LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
            LEFT JOIN T_FAIXA_TEMPERATURA FaixaTemperatura ON FaixaTemperatura.FTE_CODIGO = COALESCE(Carga.FTE_CODIGO, TipoCarga.FTE_CODIGO)
            LEFT JOIN T_POSICAO Posicao ON Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
            LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO
            LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Carga.EMP_CODIGO
            LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
            LEFT JOIN T_GRUPO_TIPO_OPERACAO GrupoTipoOperacao ON GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO
            LEFT JOIN T_TIPO_TRECHO TipoTrecho ON TipoTrecho.TTR_CODIGO = Carga.TTR_CODIGO
            LEFT JOIN T_CLIENTE Terceiro ON Terceiro.CLI_CGCCPF = Carga.CLI_CGCCPF_TERCEIRO
            LEFT JOIN T_RASTREADOR_TECNOLOGIA TecnologiaRastreador ON TecnologiaRastreador.TRA_CODIGO = Veiculo.TRA_CODIGO
            LEFT JOIN HistoricoMonitoramentoStatus ON HistoricoMonitoramentoStatus.MON_CODIGO = Monitoramento.MON_CODIGO AND HistoricoMonitoramentoStatus.MSV_CODIGO = MonitoramentoStatus.MSV_CODIGO
            LEFT JOIN EstatisticasCargaEntrega ON EstatisticasCargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN PedidosCarga ON PedidosCarga.CAR_CODIGO = Monitoramento.CAR_CODIGO
            LEFT JOIN DadosMotoristas ON DadosMotoristas.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN ReboquesVinculados ON ReboquesVinculados.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN DestinosEntrega ON DestinosEntrega.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN CategoriasClientes ON CategoriasClientes.POS_CODIGO = Posicao.POS_CODIGO
            LEFT JOIN UltimasOcorrencias ON UltimasOcorrencias.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN ResponsavelVeiculo ON ResponsavelVeiculo.VEI_CODIGO = Monitoramento.VEI_CODIGO
            LEFT JOIN NotasFiscaisCarga ON NotasFiscaisCarga.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN ContratosFrete ON ContratosFrete.VEI_CODIGO = Veiculo.VEI_CODIGO
            LEFT JOIN OrigemCarga ON OrigemCarga.CAR_CODIGO = Monitoramento.CAR_CODIGO
            LEFT JOIN FronteiraRota ON FronteiraRota.CAR_CODIGO = Monitoramento.CAR_CODIGO
            LEFT JOIN AlertasCarga ON AlertasCarga.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN JanelaDescarregamento ON JanelaDescarregamento.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN ContainerCarga ON ContainerCarga.CAR_CODIGO = Carga.CAR_CODIGO
            LEFT JOIN ProtocolosIntegracao ON ProtocolosIntegracao.CAR_CODIGO = Carga.CAR_CODIGO
            OUTER APPLY (
                SELECT TOP 1 CJC_INICIO_CARREGAMENTO AS DataInicioCarregamentoJanela
                FROM T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento
                WHERE JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO AND JanelaCarregamento.CEC_CODIGO IS NOT NULL
            ) JanelaCarregamento
            CROSS JOIN (
                SELECT TOP 1
                    CEM_TEMPO_SEM_POSICAO_PARA_VEICULO_PERDER_SINAL AS TempoSemPosicaoParaVeiculoPerderSinal,
                    CEM_DATA_BASE_PARA_CALCULO_PREVISAO_CONTROLE_ENTREGA AS DataBaseCalculoPrevisaoControleEntrega
                FROM T_CONFIGURACAO_EMBARCADOR
            ) _ConfiguracaoTMS
            ";
        }

        private static string ObterFromMonitoramento()
        {
            return @" FROM T_MONITORAMENTO Monitoramento ";
        }

        public static string ObterConsultaCompletaMonitoramento(bool somenteUltimoCarga, bool isCount)
        {
            var ctes = new List<string>
            {
                CTE_HISTORICO_MONITORAMENTO_STATUS,
                CTE_ESTATISTICAS_CARGA_ENTREGA_AUX,
                CTE_ESTATISTICAS_CARGA_ENTREGA,
                CTE_PEDIDOS_CARGA,
                CTE_DADOS_MOTORISTAS,
                CTE_REBOQUES_VINCULADOS,
                CTE_DESTINOS_ENTREGA_AUX,
                CTE_DESTINOS_ENTREGA,
                CTE_CATEGORIAS_CLIENTES,
                CTE_ULTIMAS_OCORRENCIAS,
                CTE_RESPONSAVEL_VEICULO,
                CTE_NOTAS_FISCAIS_CARGA,
                CTE_CONTRATOS_FRETE,
                CTE_ORIGEM_CARGA_AUX,
                CTE_ORIGEM_CARGA,
                CTE_FRONTEIRA_ROTA,
                CTE_ALERTAS_CARGA_AUX,
                CTE_ALERTAS_CARGA,
                CTE_JANELA_DESCARREGAMENTO,
                CTE_CONTAINER_CARGA,
                CTE_PROTOCOLOS_INTEGRACAO
            };

            if (somenteUltimoCarga)
                ctes.Add(CTE_CARGA_MAIS_RECENTE);

            var sb = new StringBuilder();

            // Usa string.Join para montar os CTEs
            sb.AppendLine("WITH ");

            sb.AppendLine(string.Join(",\n", ctes));

            if (!isCount)
                sb.AppendLine(MAIN_QUERY_SELECT);
            else
                sb.AppendLine("SELECT COUNT(1) AS TotalRegistros");

            // Acrescenta o FROM e os JOINs
            sb.AppendLine(ObterFromMonitoramento());
            sb.AppendLine(ObterJoinsMonitoramento(somenteUltimoCarga));

            return sb.ToString();
        }
    }

}
