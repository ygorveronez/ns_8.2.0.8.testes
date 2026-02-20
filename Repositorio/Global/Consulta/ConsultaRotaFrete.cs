using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaRotaFrete : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio>
    {
        #region Construtores

        public ConsultaRotaFrete() : base(tabela: "T_ROTA_FRETE as RotaFrete") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append("LEFT JOIN T_GRUPO_PESSOAS GrupoPessoas ON GrupoPessoas.GRP_CODIGO = RotaFrete.GRP_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append("LEFT JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = RotaFrete.CLI_CGCCPF ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("LEFT JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = RotaFrete.TOP_CODIGO ");
        }

        public void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append("LEFT JOIN T_ROTA_FRETE_TRANSPORTADOR Transportador ON RotaFrete.ROF_CODIGO = Transportador.ROF_CODIGO ");
        }

        public void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Transportador.EMP_CODIGO ");
        }

        public void SetarJoinsDistribuidor(StringBuilder joins)
        {
            if (!joins.Contains(" Distribuidor "))
                joins.Append("LEFT JOIN T_CLIENTE Distribuidor ON Distribuidor.CLI_CGCCPF = RotaFrete.CLI_CGCCPF_DISTRIBUIDOR ");
        }

        public void SetarJoinsFronteiras(StringBuilder joins)
        {
            if (!joins.Contains(" Fronteiras "))
                joins.Append("LEFT JOIN T_ROTA_FRETE_FRONTEIRA Fronteiras ON Fronteiras.ROF_CODIGO = RotaFrete.ROF_CODIGO ");
        }

        public void SetarJoinsClienteFronteira(StringBuilder joins)
        {
            SetarJoinsFronteiras(joins);

            if (!joins.Contains(" ClienteFronteira "))
                joins.Append("LEFT JOIN T_CLIENTE ClienteFronteira ON ClienteFronteira.CLI_CGCCPF = Fronteiras.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("RotaFrete.ROF_CODIGO Codigo, ");
                        groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao,"))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO Descricao, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao,"))
                    {
                        select.Append("RotaFrete.ROF_CODIGO_INTEGRACAO CodigoIntegracao, ");
                        groupBy.Append("RotaFrete.ROF_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "TempoViagemHoras":
                    if (!select.Contains(" TempoViagemHoras,"))
                    {
                        select.Append("RotaFrete.ROF_TEMPO_VIAGEM_EM_MINUTOS TempoViagemHoras, ");
                        groupBy.Append("RotaFrete.ROF_TEMPO_VIAGEM_EM_MINUTOS, ");
                    }
                    break;

                case "Quilometros":
                    if (!select.Contains(" Quilometros,"))
                    {
                        select.Append("RotaFrete.ROF_QUILOMETROS Quilometros, ");
                        groupBy.Append("RotaFrete.ROF_QUILOMETROS, ");
                    }
                    break;

                case "VelocidadeMediaCarregado":
                    if (!select.Contains(" VelocidadeMediaCarregado,"))
                    {
                        select.Append("RotaFrete.ROF_VELOCIDADE_MEDIA_CARREGADO VelocidadeMediaCarregado, ");
                        groupBy.Append("RotaFrete.ROF_VELOCIDADE_MEDIA_CARREGADO, ");
                    }
                    break;

                case "VelocidadeMediaVazio":
                    if (!select.Contains(" VelocidadeMediaVazio,"))
                    {
                        select.Append("RotaFrete.ROF_VELOCIDADE_MEDIA_VAZIO VelocidadeMediaVazio, ");
                        groupBy.Append("RotaFrete.ROF_VELOCIDADE_MEDIA_VAZIO, ");
                    }
                    break;


                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("RotaFrete.ROF_OBSERVACAO Observacao, ");
                        groupBy.Append("RotaFrete.ROF_OBSERVACAO, ");
                    }
                    break;

                case "Detalhes":
                    if (!select.Contains(" Detalhes,"))
                    {
                        select.Append("RotaFrete.ROF_DETALHES Detalhes, ");
                        groupBy.Append("RotaFrete.ROF_DETALHES, ");
                    }
                    break;

                case "DescricaoTipoRota":
                    if (!select.Contains(" TipoRota,"))
                    {
                        select.Append("RotaFrete.ROF_TIPO_ROTA TipoRota, ");
                        groupBy.Append("RotaFrete.ROF_TIPO_ROTA, ");
                    }
                    break;

                case "DescricaoTipoCarregamentoIda":
                    if (!select.Contains(" TipoCarregamentoIda,"))
                    {
                        select.Append("RotaFrete.ROF_TIPO_CARREGAMENTO_IDA TipoCarregamentoIda, ");
                        groupBy.Append("RotaFrete.ROF_TIPO_CARREGAMENTO_IDA, ");
                    }
                    break;

                case "DescricaoTipoCarregamentoVolta":
                    if (!select.Contains(" TipoCarregamentoVolta,"))
                    {
                        select.Append("RotaFrete.ROF_TIPO_CARREGAMENTO_VOLTA TipoCarregamentoVolta, ");
                        groupBy.Append("RotaFrete.ROF_TIPO_CARREGAMENTO_VOLTA, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("CASE WHEN RotaFrete.ROF_ATIVO = 1 THEN 'Ativo' ELSE 'Inativo' END Situacao, ");
                        groupBy.Append("RotaFrete.ROF_ATIVO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente,"))
                    {
                        select.Append(@"Remetente.CLI_NOME + ' (' + 
                                        CASE 
                                        WHEN Remetente.CLI_FISJUR = 'J' THEN 
                                           FORMAT(Remetente.CLI_CGCCPF, '00\.000\.000/0000-00')
                                        WHEN  Remetente.CLI_FISJUR = 'F' THEN
                                           FORMAT(Remetente.CLI_CGCCPF, '000\.000\.000-00')
                                        ELSE 
                                           ''
                                        END + ')' Remetente, ");
                        groupBy.Append("Remetente.CLI_NOME, Remetente.CLI_FISJUR, Remetente.CLI_CGCCPF, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + LocalidadeOrigem.LOC_DESCRICAO + ' - ' + LocalidadeOrigem.UF_SIGLA
                                            FROM T_ROTA_FRETE_LOCALIDADE_ORIGEM RotaFreteLocalidadeOrigem 
                                            INNER JOIN T_LOCALIDADES LocalidadeOrigem ON LocalidadeOrigem.LOC_CODIGO = RotaFreteLocalidadeOrigem.LOC_CODIGO
                                            WHERE RotaFreteLocalidadeOrigem.ROF_CODIGO = RotaFrete.ROF_CODIGO FOR XML PATH('')
                                        ), 3, 1000) Origem, ");

                        if (!groupBy.Contains("RotaFrete.ROF_CODIGO,"))
                            groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        //select.Append(@"SUBSTRING((SELECT ', ' + LocalidadeDestino.LOC_DESCRICAO + ' - ' + LocalidadeDestino.UF_SIGLA
                        //                    FROM T_ROTA_FRETE_DESTINATARIO_ORDEM RotaFreteLocalidadeDestino
                        //                    INNER JOIN T_CLIENTE Dest on Dest.CLI_CGCCPF = RotaFreteLocalidadeDestino.CLI_CGCCPF
                        //                    INNER JOIN T_LOCALIDADES LocalidadeDestino ON LocalidadeDestino.LOC_CODIGO = Dest.LOC_CODIGO
                        //                    WHERE RotaFreteLocalidadeDestino.ROF_CODIGO = RotaFrete.ROF_CODIGO FOR XML PATH('')
                        //                ), 3, 1000) Destino, ");

                        select.Append(@"SUBSTRING((SELECT ', ' + LocalidadeDestino.LOC_DESCRICAO + ' - ' + LocalidadeDestino.UF_SIGLA
                                            FROM T_ROTA_FRETE_LOCALIDADE RotaFreteLocalidade
                                            INNER JOIN T_LOCALIDADES LocalidadeDestino ON LocalidadeDestino.LOC_CODIGO = RotaFreteLocalidade.LOC_CODIGO
                                            WHERE RotaFreteLocalidade.ROF_CODIGO = RotaFrete.ROF_CODIGO 
                                            FOR XML PATH('')
                                        ), 3, 1000) Destino, ");

                        if (!groupBy.Contains("RotaFrete.ROF_CODIGO,"))
                            groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + Destinatario.CLI_NOME + ' (' + 
                                        CASE WHEN Destinatario.CLI_FISJUR = 'J' THEN 
                                           FORMAT(Destinatario.CLI_CGCCPF, '00\.000\.000/0000-00')
                                        WHEN Destinatario.CLI_FISJUR = 'F' THEN
                                           FORMAT(Destinatario.CLI_CGCCPF, '000\.000\.000-00')
                                        ELSE 
                                           ''
                                        END + ')'
                                        FROM T_ROTA_FRETE_DESTINATARIO_ORDEM RotaFreteDestinatario
                                        INNER JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = RotaFreteDestinatario.CLI_CGCCPF
                                        WHERE RotaFreteDestinatario.ROF_CODIGO = RotaFrete.ROF_CODIGO FOR XML PATH('')
                                        ), 3, 1000) Destinatario, ");

                        if (!groupBy.Contains("RotaFrete.ROF_CODIGO,"))
                            groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "EstadoDestino":
                    if (!select.Contains(" EstadoDestino,"))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + RotaFreteEstadoDestino.UF_SIGLA
                                            FROM T_ROTA_FRETE_ESTADO RotaFreteEstadoDestino
                                            WHERE RotaFreteEstadoDestino.ROF_CODIGO = RotaFrete.ROF_CODIGO FOR XML PATH('')
                                        ), 3, 1000) EstadoDestino, ");

                        if (!groupBy.Contains("RotaFrete.ROF_CODIGO,"))
                            groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "CodigoIntegracaoValePedagio":
                    if (!select.Contains(" CodigoIntegracaoValePedagio,"))
                    {
                        select.Append("RotaFrete.ROF_CODIGO_INTEGRACAO_VALE_PEDAGIO CodigoIntegracaoValePedagio, ");
                        groupBy.Append("RotaFrete.ROF_CODIGO_INTEGRACAO_VALE_PEDAGIO, ");
                    }
                    break;

                case "CEP":
                    if (!select.Contains(" CEP,"))
                    {
                        select.Append(@"SUBSTRING(
                                       (SELECT ', ' + ('de ' + CAST(rotafreteCEP.ROC_CEP_INICIAL AS VARCHAR(10)) + ' até ' + CAST(rotafreteCEP.ROC_CEP_FINAL AS VARCHAR(10)))
                                        FROM T_ROTA_FRETE_CEP rotafreteCEP
                                        WHERE rotafreteCEP.ROF_CODIGO = RotaFrete.ROF_CODIGO
                                          FOR XML PATH('')), 3, 1000) CEP,");

                        if (!groupBy.Contains("RotaFrete.ROF_CODIGO,"))
                            groupBy.Append("RotaFrete.ROF_CODIGO, ");
                    }
                    break;

                case "LeadTimeDias":
                    if (!select.Contains(" LeadTimeDias,"))
                    {
                        select.Append(@"(RotaFrete.ROF_TEMPO_VIAGEM_EM_MINUTOS / 1440) LeadTimeDias, ");

                        if (!groupBy.Contains("RotaFrete.ROF_TEMPO_VIAGEM_EM_MINUTOS,"))
                            groupBy.Append("RotaFrete.ROF_TEMPO_VIAGEM_EM_MINUTOS, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains("Transportador,"))
                    {
                        select.Append("Empresa.EMP_RAZAO Transportador, ");

                        if (!groupBy.Contains("Empresa.EMP_RAZAO,"))
                            groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "PercentualCarga":
                    if (!select.Contains("PercentualCarga,"))
                    {
                        select.Append("Transportador.RFE_PERCENTUAL_CARGAS_DA_ROTA PercentualCarga, ");

                        if (!groupBy.Contains("Transportador.RFE_PERCENTUAL_CARGAS_DA_ROTA,"))
                            groupBy.Append("Transportador.RFE_PERCENTUAL_CARGAS_DA_ROTA, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Distribuidor":
                    if (!select.Contains("Distribuidor,"))
                    {
                        select.Append(@"Distribuidor.CLI_NOME + ' (' + 
                                        CASE
                                        WHEN Distribuidor.CLI_FISJUR = 'J' THEN
                                            FORMAT(Distribuidor.CLI_CGCCPF, '00\.000\.000/0000-00')
                                        WHEN  Distribuidor.CLI_FISJUR = 'F' THEN
                                            FORMAT(Distribuidor.CLI_CGCCPF, '000\.000\.000-00')
                                        ELSE
                                            ''
                                        END + ')' Distribuidor, ");

                        groupBy.Append("Distribuidor.CLI_NOME, Distribuidor.CLI_FISJUR, Distribuidor.CLI_CGCCPF, ");

                        SetarJoinsDistribuidor(joins);
                    }
                    break;

                case "RotaExclusivaCompraValePedagioFormatada":
                    if (!select.Contains(" RotaExclusivaCompraValePedagio, "))
                    {
                        select.Append("ISNULL(RotaFrete.ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO, 0) RotaExclusivaCompraValePedagio, ");
                        groupBy.Append("RotaFrete.ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO, ");
                    }
                    break;

                case "TempoCarregamento":
                    if (!select.Contains(" TempoCarregamento, "))
                    {
                        select.Append("CAST(RotaFrete.ROF_TEMPO_CARREGAMENTO AS VARCHAR(5)) TempoCarregamento, ");
                        groupBy.Append("RotaFrete.ROF_TEMPO_CARREGAMENTO, ");
                    }
                    break;

                case "TempoDescarga":
                    if (!select.Contains(" TempoDescarga, "))
                    {
                        select.Append("CAST(RotaFrete.ROF_TEMPO_DESCARGA AS VARCHAR(5)) TempoDescarga, ");
                        groupBy.Append("RotaFrete.ROF_TEMPO_DESCARGA, ");
                    }
                    break;

                case "Fronteira":
                    if (!select.Contains(" Fronteira, "))
                    {
                        select.Append("ClienteFronteira.CLI_NOME Fronteira, ");
                        groupBy.Append("Fronteiras.RFF_CODIGO, ClienteFronteira.CLI_NOME, ");

                        SetarJoinsClienteFronteira(joins);
                    }
                    break;

                case "TempoFronteira":
                case "TempoMedioPermanenciaFronteira":
                    if (!select.Contains(" TempoMedioPermanenciaFronteira, "))
                    {
                        select.Append("ISNULL(Fronteiras.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA, 0) TempoMedioPermanenciaFronteira, ");
                        groupBy.Append("Fronteiras.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA, ");

                        SetarJoinsFronteiras(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                where.Append($" AND RotaFrete.ROF_DESCRICAO LIKE '%{filtrosPesquisa.Descricao}%'");

            if (filtrosPesquisa.Destinatario > 0D)
                where.Append($" AND EXISTS (SELECT ROF_CODIGO FROM T_ROTA_FRETE_DESTINATARIO WHERE ROF_CODIGO = RotaFrete.ROF_CODIGO AND CLI_CGCCPF = {filtrosPesquisa.Destinatario.ToString("F0")})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
                where.Append($" AND EXISTS (SELECT ROF_CODIGO FROM T_ROTA_FRETE_LOCALIDADE WHERE ROF_CODIGO = RotaFrete.ROF_CODIGO AND LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestino)}))");

            if (filtrosPesquisa.GrupoPessoas > 0)
                where.Append($" AND RotaFrete.GRP_CODIGO = {filtrosPesquisa.GrupoPessoas}");

            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
                where.Append($" AND EXISTS (SELECT ROF_CODIGO FROM T_ROTA_FRETE_LOCALIDADE_ORIGEM WHERE ROF_CODIGO = RotaFrete.ROF_CODIGO AND LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)}))");

            if (filtrosPesquisa.Remetente > 0D)
                where.Append($" AND RotaFrete.CLI_CGCCPF = {filtrosPesquisa.Remetente.ToString("F0")}");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" AND RotaFrete.ROF_ATIVO = {(filtrosPesquisa.Situacao.Value ? "1" : "0")}");

            if (filtrosPesquisa.TipoOperacao > 0)
                where.Append($" AND RotaFrete.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}");

            if (filtrosPesquisa.CodigosUFDestino?.Count > 0) 
            { 
                where.Append($" AND EXISTS (SELECT ROF_CODIGO FROM T_ROTA_FRETE_ESTADO WHERE ROF_CODIGO = RotaFrete.ROF_CODIGO AND UF_SIGLA in (:UF_SIGLA))");
                parametros.Add(new Embarcador.Consulta.ParametroSQL("UF_SIGLA", string.Join("', '", filtrosPesquisa.CodigosUFDestino)));
            }

            if (filtrosPesquisa.RotaExclusivaCompraValePedagio.HasValue)
            {
                if (filtrosPesquisa.RotaExclusivaCompraValePedagio.Value)
                    where.Append($" AND RotaFrete.ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO = 1");
                else
                    where.Append($" AND (RotaFrete.ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO is null OR RotaFrete.ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO = 0)");
            }
        }

        #endregion
    }
}
