using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoFrota : RepositorioBase<Dominio.Entidades.EntidadeBase>
    {

        #region Propriedades privadas com os trechos do SQL

        //    private string querySelect = $@"
        //        select 
        //            Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador,
        //            Carga.CAR_CODIGO CodigoCarga,
        //            PosicaoAtual.POA_STATUS as Situacao,
        //            Veiculo.VEI_PLACA as PlacaVeiculo,
        //            Veiculo.VEI_CODIGO as CodigoVeiculo,
        //            VeiculoMotorista.VMT_NOME as Motorista,
        //            PosicaoAtual.POS_CODIGO CodigoPosicao,
        //            PosicaoAtual.POA_LATITUDE as Latitude,
        //         PosicaoAtual.POA_LONGITUDE as Longitude,
        //            PosicaoAtual.POA_DATA_VEICULO as DataDaPosicao,
        //            PosicaoAtual.POA_DESCRICAO as Descricao,
        //            PosicaoAtual.POA_EM_ALVO EmAlvo,
        //            Posicao.POS_RASTREADOR Rastreador,
        //      SUBSTRING((SELECT ',' + convert(varchar, convert(bigint, CLI_CGCCPF)) AS [text()]
        //                        FROM T_POSICAO_ALVO PosicaoAlvo
        //                        WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
        //                        FOR XML PATH ('')), 2, 2000) CodigosClientesAlvos,
        //            SUBSTRING((SELECT ', ' + CASE isnull(ClienteAlvo.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE ClienteAlvo.CLI_CODIGO_INTEGRACAO + '-' END  + ' - ' + ClienteAlvo.CLI_NOME AS [text()]
        //                        FROM T_POSICAO_ALVO PosicaoAlvo
        //                        JOIN t_CLIENTE ClienteAlvo on ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF
        //                        WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
        //                        ORDER BY ClienteAlvo.CLI_NOME
        //                        FOR XML PATH ('')), 3,2000) ClientesAlvos,
        //            SUBSTRING((SELECT', ' + CategoriaCliente.CTP_DESCRICAO AS [text()]
        //                        FROM T_POSICAO_ALVO PosicaoAlvo
        //                        JOIN t_CLIENTE ClienteAlvo on ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF
        //                        join T_CATEGORIA_PESSOA as CategoriaCliente on CategoriaCliente.CTP_CODIGO = ClienteAlvo.CTP_CODIGO
        //                        WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
        //                        ORDER BY CategoriaCliente.CTP_DESCRICAO
        //                        FOR XML PATH ('')), 3,2000) CategoriasClientesAlvos,
        //            Filial.FIL_DESCRICAO as Filial,
        //            isnull(Carga.CAR_DATA_CARREGAMENTO, Carga.CAR_DATA_CRIACAO) as DataDaCarga,
        //         Monitoramento.MON_CODIGO as SM,
        //            Monitoramento.MON_CRITICO as MonitoramentoCritico,
        //            GrupoStatusViagem.MGV_CODIGO as GrupoStatusViagemCodigo,
        //            GrupoStatusViagem.MGV_DESCRICAO as GrupoStatusViagemDescricao,
        //            GrupoStatusViagem.MGV_COR as GrupoStatusViagemCor,
        //            GrupoTipoOperacao.GTO_CODIGO as GrupoTipoOperacaoCodigo,
        //            GrupoTipoOperacao.GTO_DESCRICAO as GrupoTipoOperacaoDescricao,
        //            GrupoTipoOperacao.GTO_COR as GrupoTipoOperacaoCor,
        //            StatusViagem.MSV_DESCRICAO as Status,
        //         ModeloVeicular.MVC_DESCRICAO as ModeloVeicular,
        //         ModeloVeicular.MVC_TIPO as TipoModeloVeicular,
        //         TipoOperacao.TOP_DESCRICAO as TipoDeTransporte,
        //            Monitoramento.MON_STATUS as StatusMonitoramento, 
        //            Carga.CAR_CARGA_FECHADA as CargaFechada,
        //            Carga.CAR_CARGA_DE_PRE_CARGA as CargaPreCarga,

        //         SUBSTRING((SELECT Empresa.EMP_RAZAO  + ' (' + Localidades.LOC_DESCRICAO + '/' + Localidades.UF_SIGLA + ')' AS [text()]
        //         FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 0, 2000) Transportador,
        //            SUBSTRING((SELECT DISTINCT ', ' + VV.VEI_PLACA
        //                        FROM T_VEICULO_CONJUNTO C
        //                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = C.VEC_CODIGO_FILHO
        //			WHERE C.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 2000) Reboque,
        //            CASE
        //	WHEN VEI_SITUACAO_VEICULO = 1 THEN 'Vazio'
        //	WHEN VEI_SITUACAO_VEICULO = 2 THEN 'Aviso p/ Carregamento'
        //	WHEN VEI_SITUACAO_VEICULO = 3 THEN 'Em Viagem'
        //	WHEN VEI_SITUACAO_VEICULO = 4 THEN 'Em Manutenção'
        //	ELSE 'Disponível'
        //END SituacaoVeiculo ";

        private string querySelect = $@"
                            WITH LastCarga AS (
                                SELECT
                                    CAR_CODIGO_CARGA_EMBARCADOR,
                                    CAR_CODIGO,
                                    CAR_DATA_CARREGAMENTO,
                                    CAR_DATA_CRIACAO,
                                    CAR_CARGA_FECHADA,
                                    CAR_SITUACAO,
                                    CAR_CARGA_DE_PRE_CARGA,
                                    CAR_VEICULO,
		                            TOP_CODIGO,
		                            FIL_CODIGO,
		                            CDS_CODIGO,
									CAR_DATA_INICIO_VIAGEM,
									CAR_DATA_FIM_VIAGEM,
                                    ROW_NUMBER() OVER (PARTITION BY CAR_VEICULO ORDER BY CAR_DATA_CRIACAO DESC) AS RN
                                FROM T_CARGA 
                            ),
                            ContagemEntregasCargaEntrega AS (
                                   SELECT CAR_CODIGO,
									    COUNT(*) As QuantidadeEntregas
                                    FROM T_CARGA_ENTREGA
									where CEN_COLETA = 0 AND CEN_FRONTEIRA = 0 
									group by CAR_CODIGO
                            ),
                            LastVeiculo AS (
                                SELECT
                                    VEI_CODIGO,
                                    VEI_PLACA,
		                            MVC_CODIGO,
		                            EMP_CODIGO,
		                            VEI_SITUACAO_VEICULO,
		                            VEI_ATIVO,
                                    TRA_CODIGO,
									VEI_POSSUI_RASTREADOR,
                                    CRE_CODIGO,
                                    GRP_CODIGO,
                                    FUN_CODIGO_RESPONSAVEL,
                                    ROW_NUMBER() OVER (PARTITION BY VEI_PLACA ORDER BY VEI_CODIGO DESC) AS RN
                                FROM T_VEICULO
                            ),
                            LastMonitoramento AS (
                                SELECT
                                    MON_CODIGO,
                                    MON_CRITICO,
                                    MON_STATUS,
                                    T_MONITORAMENTO.VEI_CODIGO,
		                            vei.VEI_PLACA,
		                            MSV_CODIGO,
                                    MON_PERCENTUAL_VIAGEM,
									CAR_CODIGO,
                                    ROW_NUMBER() OVER (PARTITION BY vei.VEI_PLACA ORDER BY MON_DATA_INICIO DESC) AS RN
                                FROM T_MONITORAMENTO
	                            inner join T_VEICULO vei on vei.VEI_CODIGO = T_MONITORAMENTO.VEI_CODIGO
                                WHERE MON_DATA_INICIO IS NOT NULL AND MON_DATA_FIM IS NULL
                            ),
                            LastPosicao AS (
                                SELECT
                                    POS_CODIGO,
                                    POA_STATUS,
                                    POA_LATITUDE,
                                    POA_LONGITUDE,
                                    POA_DATA_VEICULO,
                                    POA_DESCRICAO,
                                    POA_EM_ALVO,
                                    VEI_CODIGO,
                                    ROW_NUMBER() OVER (PARTITION BY VEI_CODIGO ORDER BY POA_DATA_VEICULO DESC) AS RN
                                FROM T_POSICAO_ATUAL
                            ),
                            LastCargaEntrega AS (
                                SELECT 
                                    CAR_CODIGO,
                                    CEN_DATA_ENTREGA_REPROGRAMADA,
                                    CLI_CODIGO_ENTREGA,
                                    LOC_CODIGO
                                FROM (
                                    SELECT 
                                        CAR_CODIGO,
                                        CEN_DATA_ENTREGA_REPROGRAMADA,
                                        CLI_CODIGO_ENTREGA,
                                        LOC_CODIGO,
                                        ROW_NUMBER() OVER (PARTITION BY CAR_CODIGO ORDER BY CEN_DATA_ENTREGA_REPROGRAMADA DESC) AS RN
                                    FROM T_CARGA_ENTREGA
                                ) AS ranked
                                WHERE RN = 1
                            ),
                            DadosDetalhados AS (SELECT
                                Carga.CAR_CODIGO_CARGA_EMBARCADOR AS CodigoCargaEmbarcador,
								CargaMonitoramento.CAR_CODIGO_CARGA_EMBARCADOR AS CodigoUltimaCargaEmbarcador,
                                CargaMonitoramento.CAR_CODIGO AS CodigoCargaVinculada,
                                Carga.CAR_CODIGO AS CodigoCarga,
                                Carga.CAR_SITUACAO AS SituacaoCarga,
                                PosicaoAtual.POA_STATUS AS Situacao,
                                Veiculo.VEI_PLACA AS PlacaVeiculo,
                                Veiculo.VEI_CODIGO AS CodigoVeiculo,
                                VeiculoMotorista.VMT_NOME AS Motorista,
                                PosicaoAtual.POS_CODIGO AS CodigoPosicao,
                                PosicaoAtual.POA_LATITUDE AS Latitude,
                                PosicaoAtual.POA_LONGITUDE AS Longitude,
                                PosicaoAtual.POA_DATA_VEICULO AS DataDaPosicao,
                                PosicaoAtual.POA_DESCRICAO AS Descricao,
                                PosicaoAtual.POA_EM_ALVO AS EmAlvo,
                                Posicao.POS_RASTREADOR AS Rastreador,
                                (CASE 
                                    WHEN Posicao.POS_DATA_VEICULO IS NULL THEN 1
                                    WHEN DATEDIFF(MINUTE, Posicao.POS_DATA_VEICULO, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') <= (select CEM_TEMPO_SEM_POSICAO_PARA_VEICULO_PERDER_SINAL from T_CONFIGURACAO_EMBARCADOR) THEN 3
                                    ELSE 4
                                END) as RastreadorOnlineOffline,
                                CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA AS PrevisaoEntregaAtualizada,
                                Localidade.LOC_DESCRICAO AS EnderecoDaEntrega,
                                Monitoramento.MON_PERCENTUAL_VIAGEM AS PercentualViagem,
                                SUBSTRING((
                                    SELECT
                                        ',' + CONVERT(VARCHAR, CONVERT(BIGINT, CLI_CGCCPF)) AS [text()]
                                    FROM T_POSICAO_ALVO PosicaoAlvo
                                    WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
                                    FOR XML PATH ('')
                                ), 2, 2000) AS CodigosClientesAlvos,
                                SUBSTRING((
                                    SELECT
                                        ', ' + CASE ISNULL(ClienteAlvo.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE ClienteAlvo.CLI_CODIGO_INTEGRACAO + '-' END + ' - ' + ClienteAlvo.CLI_NOME AS [text()]
                                    FROM T_POSICAO_ALVO PosicaoAlvo
                                    JOIN t_CLIENTE ClienteAlvo ON ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF
                                    WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
                                    ORDER BY ClienteAlvo.CLI_NOME
                                    FOR XML PATH ('')
                                ), 3, 2000) AS ClientesAlvos,
                                SUBSTRING((
                                    SELECT
                                        ', ' + CategoriaCliente.CTP_DESCRICAO AS [text()]
                                    FROM T_POSICAO_ALVO PosicaoAlvo
                                    JOIN t_CLIENTE ClienteAlvo ON ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF
                                    JOIN T_CATEGORIA_PESSOA AS CategoriaCliente ON CategoriaCliente.CTP_CODIGO = ClienteAlvo.CTP_CODIGO
                                    WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
                                    ORDER BY CategoriaCliente.CTP_DESCRICAO
                                    FOR XML PATH ('')
                                ), 3, 2000) AS CategoriasClientesAlvos,
                                Filial.FIL_DESCRICAO AS Filial,
                                ISNULL(Carga.CAR_DATA_CARREGAMENTO, Carga.CAR_DATA_CRIACAO) AS DataDaCarga,
                                Monitoramento.MON_CODIGO AS SM,
                                Monitoramento.MON_CRITICO AS MonitoramentoCritico,
                                GrupoStatusViagem.MGV_CODIGO AS GrupoStatusViagemCodigo,
                                GrupoStatusViagem.MGV_DESCRICAO AS GrupoStatusViagemDescricao,
                                GrupoStatusViagem.MGV_COR AS GrupoStatusViagemCor,
                                GrupoTipoOperacao.GTO_CODIGO AS GrupoTipoOperacaoCodigo,
                                GrupoTipoOperacao.GTO_DESCRICAO AS GrupoTipoOperacaoDescricao,
                                GrupoTipoOperacao.GTO_COR AS GrupoTipoOperacaoCor,
                                StatusViagem.MSV_DESCRICAO AS Status,
                                ModeloVeicular.MVC_DESCRICAO AS ModeloVeicular,
                                ModeloVeicular.MVC_TIPO AS TipoModeloVeicular,
                                TipoOperacao.TOP_DESCRICAO AS TipoDeTransporte,
                                Monitoramento.MON_STATUS AS StatusMonitoramento,
                                Carga.CAR_CARGA_FECHADA AS CargaFechada,
                                Carga.CAR_CARGA_DE_PRE_CARGA AS CargaPreCarga,
	                             SUBSTRING((
                                    SELECT
                                        ', ' + Empresa.EMP_RAZAO + ' (' + LOC.LOC_DESCRICAO + '/' + LOC.UF_SIGLA + ')' AS [text()]
                                    FROM T_EMPRESA Empresa
		                            JOIN T_LOCALIDADES LOC ON LOC.LOC_CODIGO = Empresa.LOC_CODIGO
                                    JOIN T_VEICULO VEI ON VEI.EMP_CODIGO = Empresa.EMP_CODIGO
		                            where 
		                            VEI.VEI_PLACA = Veiculo.VEI_PLACA
                                    FOR XML PATH ('')
                                ), 3, 2000) AS Transportador,
                                    SUBSTRING((SELECT
                                        DISTINCT ', ' + VV.VEI_PLACA
                                    FROM
                                        T_VEICULO_CONJUNTO C
                                    JOIN
                                        T_VEICULO VV
                                            ON VV.VEI_CODIGO = C.VEC_CODIGO_FILHO
                                    WHERE
                                        C.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO FOR XML PATH('')),
                                    3,
                                    2000) Reboque,
                                CASE
                                    WHEN VEI_SITUACAO_VEICULO = 1 THEN 'Vazio'
                                    WHEN VEI_SITUACAO_VEICULO = 2 THEN 'Aviso p/ Carregamento'
                                    WHEN VEI_SITUACAO_VEICULO = 3 THEN 'Em Viagem'
                                    WHEN VEI_SITUACAO_VEICULO = 4 THEN 'Em Manutenção'
                                    ELSE 'Disponível'
                                END AS SituacaoVeiculo, 
                                ISNULL(VEI_SITUACAO_VEICULO, 0) AS EnumSituacaoVeiculo,
								MonitoramentoHistorico.MHS_DATA_FIM as DataFimStatusAtual,
								MonitoramentoHistorico.MHS_DATA_INICIO as DataInicioStatusAtual,
                                CASE 
									WHEN DadosSumarizados.CDS_RECEBEDORES IS NOT NULL AND DadosSumarizados.CDS_RECEBEDORES <> '' AND LTRIM(RTRIM(DadosSumarizados.CDS_RECEBEDORES)) <> '' 
										THEN DadosSumarizados.CDS_RECEBEDORES 
									ELSE ''
								END
								+
								CASE 
									WHEN DadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS IS NOT NULL AND DadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS <> '' AND ConfiguracaoEmbarcador.CEM_NAO_EXIBIR_CODIGO_INTEGRACAO_DESTINATARIO_RESUMO_CARGA = 0
										THEN DadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS + ' - '
									ELSE ''
								END
								+
								ISNULL(DadosSumarizados.CDS_DESTINOS, '')
								+
								' (' + 
												CAST(ContagemEntregasCarga.QuantidadeEntregas AS VARCHAR) + ' ' + 
												CASE WHEN ContagemEntregasCarga.QuantidadeEntregas > 1 THEN 'Entregas' ELSE 'Entrega' END + 
												')'
								+
								CASE 
									WHEN DadosSumarizados.CDS_DISTANCIA > 0 
										THEN ' - ' + FORMAT(ROUND(DadosSumarizados.CDS_DISTANCIA, 2), 'N2') + ' KM'
									ELSE ''
								END AS DestinoCarga";


        private string queryFrom = $@"
            FROM T_VEICULO Veiculo
			JOIN T_POSICAO_ATUAL PosicaoAtual ON Veiculo.VEI_CODIGO = PosicaoAtual.VEI_CODIGO
			JOIN t_posicao Posicao ON Posicao.pos_codigo = PosicaoAtual.pos_codigo ";

        private string queryJoins = $@"
            LEFT JOIN T_VEICULO_MOTORISTA VeiculoMotorista ON VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO AND VeiculoMotorista.VMT_PRINCIPAL = 1
            LEFT JOIN T_FUNCIONARIO usuario ON VeiculoMotorista.FUN_CODIGO = usuario.FUN_CODIGO
            JOIN T_EMPRESA Empresa ON Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO
            LEFT JOIN T_LOCALIDADES Localidades ON Localidades.LOC_CODIGO = Empresa.LOC_CODIGO";

        private string queryJoinsTMS = $@"
            left outer join T_VEICULO_MOTORISTA VeiculoMotorista on VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO AND VeiculoMotorista.VMT_PRINCIPAL = 1 
            left outer join T_FUNCIONARIO usuario on VeiculoMotorista.FUN_CODIGO=usuario.FUN_CODIGO 
            left join T_RASTREADOR_TECNOLOGIA Rastreador on Veiculo.TRA_CODIGO = Rastreador.TRA_CODIGO and Rastreador.TRA_ATIVO = 1
            left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO
            left join T_LOCALIDADES Localidades on Localidades.LOC_CODIGO  = Empresa.LOC_CODIGO";

        private string queryLeftJoins = $@"
           LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO
           LEFT JOIN LastMonitoramento Monitoramento ON Monitoramento.VEI_CODIGO = Veiculo.VEI_CODIGO AND Monitoramento.RN = 1
           LEFT JOIN LastCarga Carga ON Carga.CAR_VEICULO = Veiculo.VEI_CODIGO AND Carga.RN = 1
		   LEFT JOIN T_CARGA CargaMonitoramento ON CargaMonitoramento.CAR_CODIGO = Monitoramento.CAR_CODIGO AND Monitoramento.MON_STATUS not in (2,3)
           LEFT JOIN T_FILIAL Filial on Carga.FIL_CODIGO = Filial.FIL_CODIGO
           LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
           LEFT JOIN T_GRUPO_TIPO_OPERACAO GrupoTipoOperacao ON GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO
           LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM StatusViagem ON StatusViagem.MSV_CODIGO = Monitoramento.MSV_CODIGO
           LEFT JOIN T_MONITORAMENTO_GRUPO_STATUS_VIAGEM GrupoStatusViagem ON GrupoStatusViagem.MGV_CODIGO = StatusViagem.MGV_CODIGO
           LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM MonitoramentoHistorico ON MonitoramentoHistorico.MON_CODIGO = Monitoramento.MON_CODIGO 
									and MonitoramentoHistorico.MSV_CODIGO = StatusViagem.MSV_CODIGO
           LEFT JOIN LastCargaEntrega CargaEntrega ON CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
           LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
           LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Cliente.LOC_CODIGO
           LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
           LEFT JOIN ContagemEntregasCargaEntrega ContagemEntregasCarga ON ContagemEntregasCarga.CAR_CODIGO = Carga.CAR_CODIGO
           JOIN T_CONFIGURACAO_EMBARCADOR ConfiguracaoEmbarcador ON 1 = 1";


        private string queryOrderBy = $@"
            order by 
                CodigoCargaEmbarcador,
                lastDadosDetalhados.PlacaVeiculo";

        #endregion

        #region Construtores

        public PosicaoFrota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados
        private SQLDinamico CreateSQL(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool retornaPrimeiroRegistro = false)
        {
            StringBuilder query = new StringBuilder();

            string querySelectModificado = querySelect;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador) && querySelectModificado.Contains(" FROM T_CARGA "))
                querySelectModificado = querySelectModificado.Replace(" FROM T_CARGA ", $" FROM T_CARGA WHERE CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' ");

            if (filtrosPesquisa.VeiculosDentroDoRaioFilial)
            {
                string whereFilial =
                    filtrosPesquisa.CodigosFilial.Count > 0
                        ? $" WHERE FIL_CODIGO IN " +
                        $"({string.Join(", ", filtrosPesquisa.CodigosFilial)})"
                        : "";

                int raioMetros = filtrosPesquisa.RaioFilial * 1000;

                string localizacaoFiliaisCte = $@"
                        LocalizacaoFiliais AS (
                            SELECT 
                                CLI_LATIDUDE AS Latidude,
                                cli_longitude AS Longitude,
                                ${raioMetros} AS Raio
                            FROM t_cliente Cliente
                            WHERE Cliente.CLI_CGCCPF IN (
                                SELECT FIL_CNPJ FROM t_FILIAL
                                {whereFilial} )
                            AND Cliente.Cli_latidude IS NOT NULL
                            AND Cliente.Cli_longitude IS NOT NULL
                        ),";

                querySelectModificado = querySelectModificado.Replace(
                    "WITH LastCarga",
                    $"WITH {localizacaoFiliaisCte}\nLastCarga"
                );
            }

            query.Append(querySelectModificado);
            query.Append(queryFrom);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!string.IsNullOrWhiteSpace(queryJoinsTMS))
                    query.Append(queryJoinsTMS);
            }
            else if (!string.IsNullOrWhiteSpace(queryJoins))
                query.Append(queryJoins);

            if (!string.IsNullOrWhiteSpace(queryLeftJoins))
                query.Append(queryLeftJoins);

            if (filtrosPesquisa.VeiculosDentroDoRaioFilial)
            {
                query.Append(@"
                        JOIN LocalizacaoFiliais l ON (
                            6371000 * 2 * ATAN(
                                SQRT(
                                    POWER(SIN((PosicaoAtual.POA_LATITUDE - l.Latidude) * PI() / 180) / 2, 2) +
                                    COS(l.Latidude * PI() / 180) * COS(PosicaoAtual.POA_LATITUDE * PI() / 180) *
                                    POWER(SIN((PosicaoAtual.POA_LONGITUDE - l.Longitude) * PI() / 180) / 2, 2)
                                ) / 
                                SQRT(
                                    1 -
                                    POWER(SIN((PosicaoAtual.POA_LATITUDE - l.Latidude) * PI() / 180) / 2, 2) +
                                    COS(l.Latidude * PI() / 180) * COS(PosicaoAtual.POA_LATITUDE * PI() / 180) *
                                    POWER(SIN((PosicaoAtual.POA_LONGITUDE - l.Longitude) * PI() / 180) / 2, 2)
                                )
                            ) <= l.Raio
                        )
                        ");
            }
            var whereObj = CreateWhereFilter(filtrosPesquisa);

            string where = whereObj.whereClause;
            if (!string.IsNullOrWhiteSpace(where))
                query.Append(where);


            query.AppendLine(@$")
                                , LastDadosDetalhados AS (
                                    SELECT *
                                    FROM (
                                        SELECT PlacaVeiculo,
                                       Latitude,
                                   Longitude,
                                   Situacao,
                                   Descricao,
                                   EmAlvo,
                                   CodigoVeiculo,
								   CodigoCargaVinculada,
								   CodigoUltimaCargaEmbarcador,
								   CodigoCargaEmbarcador,
                                   CodigoPosicao,
                                   Rastreador,
                                   SituacaoCarga,
                                   CodigoCarga,
                                   DestinoCarga,
                                   RastreadorOnlineOffline,
                                   CargaPreCarga,
                                   CargaFechada,
                                   DataDaPosicao,
                                   PercentualViagem,
                                   PrevisaoEntregaAtualizada,
                                   DataDaCarga,
                                   MonitoramentoCritico,
                                   GrupoStatusViagemCodigo,
                                   GrupoTipoOperacaoCodigo,
                                               ROW_NUMBER() OVER (PARTITION BY PlacaVeiculo ORDER BY DataDaPosicao DESC) AS rn
                                        FROM DadosDetalhados
                                    ) AS sub
                                    WHERE rn = 1
                                )
                                SELECT {(retornaPrimeiroRegistro ? "TOP 1" : "")}
                                    lastDadosDetalhados.PlacaVeiculo,
                                    lastDadosDetalhados.CodigoCargaEmbarcador,
                                    lastDadosDetalhados.CodigoUltimaCargaEmbarcador,
									lastDadosDetalhados.CodigoCargaVinculada,
                                    lastDadosDetalhados.CodigoCarga,
                                    lastDadosDetalhados.SituacaoCarga,
                                    lastDadosDetalhados.Situacao,
                                    lastDadosDetalhados.CodigoVeiculo,
                                    STRING_AGG(Motorista, ', ') AS Motorista,
                                    lastDadosDetalhados.CodigoPosicao,
                                    lastDadosDetalhados.Latitude,
                                    lastDadosDetalhados.Longitude,
                                    lastDadosDetalhados.DataDaPosicao,
                                    lastDadosDetalhados.Descricao,
                                    lastDadosDetalhados.EmAlvo,
                                    lastDadosDetalhados.Rastreador,
                                    lastDadosDetalhados.DestinoCarga,
                                    lastDadosDetalhados.RastreadorOnlineOffline,
                                    lastDadosDetalhados.PrevisaoEntregaAtualizada,
                                    STRING_AGG(EnderecoDaEntrega, ', ') AS EnderecoDaEntrega,
                                    lastDadosDetalhados.PercentualViagem,
                                    STRING_AGG(CodigosClientesAlvos, ', ') AS CodigosClientesAlvos,
                                    STRING_AGG(ClientesAlvos, ', ') AS ClientesAlvos,
                                    STRING_AGG(CategoriasClientesAlvos, ', ') AS CategoriasClientesAlvos,
                                    STRING_AGG(Filial, ', ') AS Filial,
                                    lastDadosDetalhados.DataDaCarga,
                                    STRING_AGG(CAST(SM AS VARCHAR), ', ') AS SM,
                                    lastDadosDetalhados.MonitoramentoCritico,
                                    lastDadosDetalhados.GrupoStatusViagemCodigo,
                                    STRING_AGG(GrupoStatusViagemDescricao, ', ') AS GrupoStatusViagemDescricao,
                                    STRING_AGG(GrupoStatusViagemCor, ', ') AS GrupoStatusViagemCor,
                                    lastDadosDetalhados.GrupoTipoOperacaoCodigo,
                                    STRING_AGG(GrupoTipoOperacaoDescricao, ', ') AS GrupoTipoOperacaoDescricao,
                                    STRING_AGG(GrupoTipoOperacaoCor, ', ') AS GrupoTipoOperacaoCor,
                                    STRING_AGG(Status, ', ') AS Status,
                                    STRING_AGG(ModeloVeicular, ', ') AS ModeloVeicular,
                                    STRING_AGG(TipoModeloVeicular, ', ') AS TiposModeloVeicular,
                                    STRING_AGG(TipoDeTransporte, ', ') AS TipoDeTransporte,
                                    STRING_AGG(StatusMonitoramento, ', ') AS StatusMonitoramento,
                                    lastDadosDetalhados.CargaFechada,
                                    lastDadosDetalhados.CargaPreCarga,
                                    STRING_AGG(Transportador, ', ') AS Transportador,
                                    STRING_AGG(Reboque, ', ') AS Reboque,
                                    STRING_AGG(SituacaoVeiculo, ', ') AS SituacaoVeiculo,
                                    STRING_AGG(CAST(EnumSituacaoVeiculo AS VARCHAR), ', ') AS SituacoesVeiculo,
	                                DataFimStatusAtual,
									DataInicioStatusAtual
                                FROM DadosDetalhados
                                LEFT JOIN LastDadosDetalhados lastDadosDetalhados ON lastDadosDetalhados.PlacaVeiculo = DadosDetalhados.PlacaVeiculo
                                GROUP BY  DadosDetalhados.DataDaPosicao, 
                                   lastDadosDetalhados.PlacaVeiculo,
                                   lastDadosDetalhados.PercentualViagem,
                                   lastDadosDetalhados.Latitude,
                                   lastDadosDetalhados.CodigoVeiculo,
                                   lastDadosDetalhados.DataDaCarga,
                                   lastDadosDetalhados.GrupoStatusViagemCodigo,
                                   lastDadosDetalhados.GrupoTipoOperacaoCodigo,
                                   lastDadosDetalhados.Longitude,
                                   lastDadosDetalhados.Situacao,
                                   lastDadosDetalhados.Descricao,
                                   lastDadosDetalhados.MonitoramentoCritico,
                                   lastDadosDetalhados.EmAlvo,
                                   lastDadosDetalhados.PrevisaoEntregaAtualizada,
                                   lastDadosDetalhados.CodigoPosicao,
                                   lastDadosDetalhados.SituacaoCarga,
                                   lastDadosDetalhados.Rastreador,
                                   lastDadosDetalhados.DestinoCarga,
                                   lastDadosDetalhados.RastreadorOnlineOffline,
                                   lastDadosDetalhados.CargaFechada,
                                   lastDadosDetalhados.DataDaPosicao,
                                   DadosDetalhados.Transportador,
                                   DadosDetalhados.DataFimStatusAtual,
                                   DadosDetalhados.DataInicioStatusAtual,
                                   lastDadosDetalhados.CargaPreCarga,
                                   lastDadosDetalhados.CodigoCarga,
								   lastDadosDetalhados.CodigoCargaVinculada,
								   lastDadosDetalhados.CodigoUltimaCargaEmbarcador,
								   lastDadosDetalhados.CodigoCargaEmbarcador");


            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar) 
                    && parametroConsulta.PropriedadeOrdenar is "TempoDaUltimaPosicaoFormatada" or "DataDaPosicaoFormatada")
                {
                    if (!query.ToString().Contains("order by"))
                        query.Append(" order by ");

                    if (parametroConsulta.PropriedadeOrdenar is "TempoDaUltimaPosicaoFormatada" or "DataDaPosicaoFormatada")
                        query.Append($"DataDaPosicao {parametroConsulta.DirecaoOrdenar}"); 
                } 
                else
                {
                    query.Append(queryOrderBy);
                }
            }

            return new SQLDinamico(query.ToString(), whereObj.parametros);
        }

        private (string whereClause, List<ParametroSQL> parametros) CreateWhereFilter(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa)
        {

            StringBuilder where = new StringBuilder();
            var parametros = new List<ParametroSQL>();

            where.Append($" and Veiculo.VEI_ATIVO = 1");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and Veiculo.VEI_PLACA = (SELECT VEI_PLACA FROM T_VEICULO SubVeiculo WHERE SubVeiculo.VEI_CODIGO =  '{filtrosPesquisa.CodigoVeiculo}')"); // SQL-INJECTION-SAFE

            if ((filtrosPesquisa.CodigosVeiculo?.Count ?? 0) > 0)
                where.Append($" and Veiculo.VEI_PLACA in ((SELECT STRING_AGG(CONCAT('', VEI_PLACA, ''), ', ') FROM T_VEICULO SubVeiculo WHERE SubVeiculo.VEI_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosVeiculo)}) ) ) "); // SQL-INJECTION-SAFE

            if ((filtrosPesquisa.CodigosVeiculoFiltro?.Count ?? 0) > 0)
                where.Append($" and Veiculo.VEI_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosVeiculoFiltro)})");

            if ((filtrosPesquisa.CodigosVeiculoFiltro?.Count ?? 0) > 0)
                where.Append($" and Veiculo.VEI_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosVeiculoFiltro)})");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
            {
                where.Append($" and Veiculo.VEI_PLACA = :VEICULO_VEI_PLACA");
                parametros.Add(new ParametroSQL("VEICULO_VEI_PLACA", filtrosPesquisa.PlacaVeiculo));
            }

            // Há apenas duas situações
            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count == 1 && (int)filtrosPesquisa.Situacoes[0] > 0)
            {
                where.Append($" and PosicaoAtual.POA_STATUS = {(int)filtrosPesquisa.Situacoes[0]}");
            }

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" and Empresa.EMP_CODIGO = '{filtrosPesquisa.Transportador}'");

            if ((filtrosPesquisa.CodigosTransportador?.Count ?? 0) > 0)
                where.Append($" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)}) ");

            if (filtrosPesquisa.Cliente > 0)
                where.Append($@" AND EXISTS (
                    SELECT 1 FROM T_POSICAO_ALVO PosicaoAlvo2
                    WHERE PosicaoAlvo2.POS_CODIGO = PosicaoAtual.POS_CODIGO AND PosicaoAlvo2.CLI_CGCCPF = {filtrosPesquisa.Cliente}
                )");

            if (filtrosPesquisa.CategoriaPessoa > 0)
                where.Append($@" and exists (
                    SELECT 1 FROM T_POSICAO_ALVO PosicaoAlvo1
                    JOIN T_CLIENTE ClienteAlvo1 ON ClienteAlvo1.CLI_CGCCPF = PosicaoAlvo1.CLI_CGCCPF
                    WHERE PosicaoAlvo1.POS_CODIGO = PosicaoAtual.POS_CODIGO AND ClienteAlvo1.CTP_CODIGO = {filtrosPesquisa.CategoriaPessoa}
		        )");

            if (filtrosPesquisa.DataInicio.HasValue)
                where.Append($" and PosicaoAtual.POA_DATA >= convert(date, '{filtrosPesquisa.DataInicio.Value.Date}', 103)");

            if (filtrosPesquisa.DataFim.HasValue)
                where.Append($" and PosicaoAtual.POA_DATA < convert(date, '{filtrosPesquisa.DataFim.Value.Date.AddDays(1)}', 103)");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))

                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%'");
                else
                    where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.CodigosGrupoStatusViagem != null && filtrosPesquisa.CodigosGrupoStatusViagem.Count > 0)
            {
                where.Append(" and (");
                if (filtrosPesquisa.CodigosGrupoStatusViagem.Contains(-1))
                {
                    where.Append(" GrupoStatusViagem.MGV_CODIGO is null or ");
                }
                else
                {
                    where.Append(" GrupoStatusViagem.MGV_CODIGO is not null and ");
                }
                where.Append($" GrupoStatusViagem.MGV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoStatusViagem)}) )");
            }

            if (filtrosPesquisa.CodigosGrupoTipoOperacao != null && filtrosPesquisa.CodigosGrupoTipoOperacao.Count > 0)
            {
                where.Append(" and (");
                if (filtrosPesquisa.CodigosGrupoTipoOperacao.Contains(-1))
                {
                    where.Append(" GrupoTipoOperacao.GTO_CODIGO is null or ");
                }
                else
                {
                    where.Append(" GrupoTipoOperacao.GTO_CODIGO is not null and ");
                }
                where.Append($" GrupoTipoOperacao.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)}) )");
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");

            if ((filtrosPesquisa.CodigosFilial?.Count ?? 0) > 0 && filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
                where.Append($@" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) or exists ( select
                        CargaPedido.CPE_CODIGO 
                    from
                        T_CARGA_PEDIDO CargaPedidoPrincipal,
                        T_CARGA_PEDIDO CargaPedido 
                    inner join
                        T_CLIENTE Recebedor 
                            on CargaPedido.CLI_CODIGO_RECEBEDOR=Recebedor.CLI_CGCCPF 
                    where
                        Carga.CAR_CODIGO=CargaPedidoPrincipal.CAR_CODIGO 
                        and CargaPedidoPrincipal.CPE_CODIGO=CargaPedido.CPE_CODIGO 
                        and (CargaPedido.CLI_CODIGO_RECEBEDOR is not null)and (Recebedor.CLI_CGCCPF in ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})) ) ");
            else if ((filtrosPesquisa.CodigosFilial?.Count ?? 0) > 0)
                where.Append($" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) ");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ");

            if (filtrosPesquisa.EmAlvo)
                where.Append($" and PosicaoAtual.POA_EM_ALVO = 1");

            if (filtrosPesquisa.MonitoramentoCritico)
                where.Append($" and Monitoramento.MON_CRITICO = 1");

            if (filtrosPesquisa.VeiculosComMonitoramento)
                where.Append($" and Monitoramento.MON_STATUS = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado}");

            if (filtrosPesquisa.VeiculosComContratoDeFrete || filtrosPesquisa.CodigosContratoFrete?.Count > 0 || filtrosPesquisa.CodigosTipoContratoFrete?.Count > 0)
            {
                where.Append(" and exists ( ");
                where.Append("   SELECT ");
                where.Append("       ContratoFreteTransportador.CFT_CODIGO ");
                where.Append("   FROM ");
                where.Append("       T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador ");
                where.Append("   JOIN ");
                where.Append("       T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo ON ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO ");
                where.Append("   WHERE ");
                where.Append("       ContratoFreteTransportador.CFT_ATIVO = 1 ");
                where.Append("       AND ContratoFreteTransportador.CFT_SITUACAO in (1,2) ");
                where.Append("       AND CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL ");
                where.Append("       AND ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO ");

                if (filtrosPesquisa.CodigosContratoFrete?.Count > 0)
                    where.Append($"       AND ContratoFreteTransportador.CFT_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosContratoFrete)}) ");

                if (filtrosPesquisa.CodigosTipoContratoFrete?.Count > 0)
                    where.Append($"       AND ContratoFreteTransportador.TCF_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosTipoContratoFrete)}) ");

                where.Append(") ");
            }


            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
            {
                where.Append(@" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    LEFT JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                    LEFT JOIN t_xml_nota_fiscal NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND NotaFiscal.NF_ATIVA = 1
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    where.Append($" and Pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ");

                where.Append(")");
            }


            if (filtrosPesquisa.CpfCnpjDestinatarios?.Count > 0)
            {
                where.Append(@" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");

                where.Append($" and Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CpfCnpjDestinatarios)}) ");
                where.Append(")");
            }

            if (filtrosPesquisa.CpfCnpjRemetentes?.Count > 0)
            {
                where.Append(@" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");

                where.Append($" and Pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.CpfCnpjRemetentes)}) ");
                where.Append(")");
            }


            if (filtrosPesquisa.SituacaoVeiculo != null && filtrosPesquisa.SituacaoVeiculo.Count > 0 && filtrosPesquisa.SituacaoVeiculo.Count < 5)
            {
                string situacoes = "";
                if (filtrosPesquisa.SituacaoVeiculo.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel))
                    situacoes += " 5,";
                if (filtrosPesquisa.SituacaoVeiculo.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao))
                    situacoes += " 4,";
                if (filtrosPesquisa.SituacaoVeiculo.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem))
                    situacoes += " 3,";
                if (filtrosPesquisa.SituacaoVeiculo.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Vazio))
                {
                    //situacoes += " 1,";
                    where.Append($" and Veiculo.VEI_VAZIO = 1");
                }
                if (filtrosPesquisa.SituacaoVeiculo.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.AvisoCarregamento))
                    situacoes += " 2,";
                if (situacoes.Length > 0)
                {
                    situacoes = situacoes.Remove(situacoes.Length - 1);
                    if (situacoes.Contains("5"))
                        where.Append($" and ( Veiculo.VEI_SITUACAO_VEICULO IN (" + situacoes + ") or Veiculo.VEI_SITUACAO_VEICULO is null)");
                    else
                        where.Append($" and Veiculo.VEI_SITUACAO_VEICULO IN (" + situacoes + ")");

                }
            }

            if (filtrosPesquisa.RastreadorOnlineOffline != null)
            {
                where.Append($@" AND (CASE 
                                        WHEN Posicao.POS_DATA_VEICULO IS NOT NULL 
                                                AND DATEDIFF(MINUTE, Posicao.POS_DATA_VEICULO, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') <= {filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal}
                                        THEN 1
                                 ELSE 0 END) = {filtrosPesquisa.RastreadorOnlineOffline.GetHashCode()}");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoUltimaCargaEmbarcador))
                where.Append($" and LastCarga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoUltimaCargaEmbarcador}' ");

            if (filtrosPesquisa.TipoModeloVeicular != null && filtrosPesquisa.TipoModeloVeicular.Count > 0)
            {
                where.Append($" and ( ");
                bool semModeloVeicular = false;
                string tiposModeloVeicular = "";
                int total = filtrosPesquisa.TipoModeloVeicular.Count;
                for (int i = 0; i < total; i++)
                {
                    if (filtrosPesquisa.TipoModeloVeicular[i] == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral ||
                        filtrosPesquisa.TipoModeloVeicular[i] == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque ||
                        filtrosPesquisa.TipoModeloVeicular[i] == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                    {
                        tiposModeloVeicular += ((int)filtrosPesquisa.TipoModeloVeicular[i]).ToString() + ",";
                    }
                    else if (filtrosPesquisa.TipoModeloVeicular[i] == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.SemModeloVeicular)
                    {
                        semModeloVeicular = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(tiposModeloVeicular))
                {
                    where.Append($" ModeloVeicular.MVC_TIPO in (" + tiposModeloVeicular.Substring(0, tiposModeloVeicular.Length - 1) + ")");
                }

                if (semModeloVeicular)
                {
                    if (!string.IsNullOrWhiteSpace(tiposModeloVeicular))
                    {
                        where.Append($" or ");
                    }
                    where.Append($" ModeloVeicular.MVC_TIPO is null");
                }
                where.Append($" ) ");
            }

            if (filtrosPesquisa.StatusViagemControleEntrega != null)
            {
                if (filtrosPesquisa.StatusViagemControleEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega.EmAndamento)
                    where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is not null and Carga.CAR_DATA_FIM_VIAGEM is null");
                else if (filtrosPesquisa.StatusViagemControleEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega.Finalizada)
                    where.Append(" and Carga.CAR_DATA_FIM_VIAGEM is not null");
                else if (filtrosPesquisa.StatusViagemControleEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega.Iniciada)
                    where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is not null");
                else if (filtrosPesquisa.StatusViagemControleEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega.NaoFinalizada)
                    where.Append(" and Carga.CAR_DATA_FIM_VIAGEM is null");
                else if (filtrosPesquisa.StatusViagemControleEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega.NaoIniciada)
                    where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is null");
            }

            if (filtrosPesquisa.Motoristas?.Count > 0)
                where.Append($" AND EXISTS (SELECT TCM_CODIGO FROM T_CARGA_MOTORISTA CargaMotorista WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO AND CAR_MOTORISTA in ({string.Join(", ", filtrosPesquisa.Motoristas)}))"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
                where.Append($" AND Veiculo.CRE_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) ");

            if (filtrosPesquisa.CodigosFuncionarioResponsavel?.Count > 0)
                where.Append($" AND Veiculo.FUN_CODIGO_RESPONSAVEL in({string.Join(", ", filtrosPesquisa.CodigosFuncionarioResponsavel)}) ");

            if (filtrosPesquisa.GrupoPessoas?.Count > 0)
                where.Append($" AND Veiculo.GRP_CODIGO in({string.Join(", ", filtrosPesquisa.GrupoPessoas)}) ");

            if (filtrosPesquisa.TecnologiaRastreador?.Count > 0)
                where.Append($" AND Veiculo.TRA_CODIGO in({string.Join(", ", filtrosPesquisa.TecnologiaRastreador)}) ");

            string whereString = where.ToString();
            if (!string.IsNullOrWhiteSpace(whereString))
            {
                whereString = Environment.NewLine + " where " + whereString.Substring(4);
            }
            return (whereString, parametros);
        }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.CreateSQL(filtrosPesquisa, tipoServicoMultisoftware, parametrosConsulta);
            var consultaPosicaoFrota = query.CriarSQLQuery(this.SessionNHiBernate);
            consultaPosicaoFrota.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota)));
            return consultaPosicaoFrota.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota ConsultarRegistroUnico(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.CreateSQL(filtrosPesquisa, tipoServicoMultisoftware, parametrosConsulta, true);
            var consultaPosicaoFrota = query.CriarSQLQuery(this.SessionNHiBernate);
            consultaPosicaoFrota.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota)));
            return consultaPosicaoFrota.SetTimeout(600).UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota>();
        }



        #endregion

    }


}
