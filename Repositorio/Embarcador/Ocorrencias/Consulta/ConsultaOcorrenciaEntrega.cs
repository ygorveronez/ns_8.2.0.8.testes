using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Ocorrencias
{
    sealed class ConsultaOcorrenciaEntrega : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega>
    {
        #region Construtores

        public ConsultaOcorrenciaEntrega() : base(tabela: ObterTabela()) { }

        #endregion

        #region Métodos Privados

        private static string ObterTabela()
        {
            return @" T_OCORRENCIA_COLETA_ENTREGA ColetaEntrega
                   join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = ColetaEntrega.CEN_CODIGO
                   join T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                   left join T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = ColetaEntrega.OCO_CODIGO
                   left JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                   left JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Cliente.LOC_CODIGO";
        }

        private void SetarJoinsGrupoOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoOcorrencia "))
                joins.Append("left join T_GRUPO_TIPO_OCORRENCIA GrupoOcorrencia on GrupoOcorrencia.GTO_CODIGO = TipoOcorrencia.GTO_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append("left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsSolicitacaoCredito(StringBuilder joins)
        {
            if (!joins.Contains(" SolicitacaoCredito "))
                joins.Append("left join T_SOLICITACAO_CREDITO SolicitacaoCredito on SolicitacaoCredito.SCR_CODIGO = ocorrenciaEntrega.CodigoSolicitante ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append("left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append("left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }


        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtroPesquisa)
        {
            switch (propriedade)
            {
                case "DataOcorrencia":
                case "DataOcorrenciaFormatada":
                    if (!select.Contains(" DataOcorrencia,"))
                    {
                        select.Append("ColetaEntrega.OCE_DATA_OCORRENCIA as DataOcorrencia, ");
                        groupBy.Append("ColetaEntrega.OCE_DATA_OCORRENCIA, ");
                    }
                    break;
                case "Latitude":
                    if (!select.Contains(" Latitude,"))
                    {
                        select.Append("ColetaEntrega.OCE_LATITUDE as Latitude, ");
                        groupBy.Append("ColetaEntrega.OCE_LATITUDE, ");
                    }
                    break;
                case "Longitude":
                    if (!select.Contains(" Longitude,"))
                    {
                        select.Append("ColetaEntrega.OCE_LONGITUDE as Longitude, ");
                        groupBy.Append("ColetaEntrega.OCE_LONGITUDE, ");
                    }
                    break;
                case "GrupoOcorrencia":
                    if (!select.Contains(" GrupoOcorrencia, "))
                    {
                        select.Append("GrupoOcorrencia.GTO_DESCRICAO as GrupoOcorrencia, ");
                        groupBy.Append("GrupoOcorrencia.GTO_DESCRICAO, ");

                        SetarJoinsGrupoOcorrencia(joins);
                    }
                    break;
                case "DataPosicao":
                case "DataPosicaoFormatada":
                    if (!select.Contains(" DataPosicao,"))
                    {
                        select.Append("ColetaEntrega.OCE_DATA_POSICAO as DataPosicao, ");
                        groupBy.Append("ColetaEntrega.OCE_DATA_POSICAO, ");
                    }
                    break;
                case "DataPrevisaoReprogramadaDaEntrega":
                case "DataPrevisaoReprogramadaDaEntregaFormatada":
                    if (!select.Contains(" DataPrevisaoReprogramadaDaEntrega,"))
                    {
                        select.Append("ColetaEntrega.OCE_DATA_PREVISAO_RECALCULADA as DataPrevisaoReprogramadaDaEntrega, ");
                        groupBy.Append("ColetaEntrega.OCE_DATA_PREVISAO_RECALCULADA, ");
                    }
                    break;
                case "TempoPercurso":
                    if (!select.Contains(" TempoPercurso,"))
                    {
                        select.Append("ColetaEntrega.OCE_TEMPO_PERCURSO as TempoPercurso, ");
                        groupBy.Append("ColetaEntrega.OCE_TEMPO_PERCURSO, ");
                    }
                    break;
                case "DistanciaAteDestino":
                case "DistanciaCalculada":
                    if (!select.Contains(" DistanciaAteDestino,"))
                    {
                        select.Append("ColetaEntrega.OCE_DISTANCIA_DESTINO as DistanciaAteDestino, ");
                        groupBy.Append("ColetaEntrega.OCE_DISTANCIA_DESTINO, ");
                    }
                    break;
                case "Pacote":
                    if (!select.Contains(" Pacote,"))
                    {
                        select.Append("ColetaEntrega.OCE_PACOTE as Pacote, ");
                        groupBy.Append("ColetaEntrega.OCE_PACOTE, ");
                    }
                    break;
                case "Volumes":
                    if (!select.Contains(" Volumes,"))
                    {
                        select.Append("ColetaEntrega.OCE_VOLUMES as Volumes, ");
                        groupBy.Append("ColetaEntrega.OCE_VOLUMES, ");
                    }
                    break;
                case "OrigemOcorrencia":
                case "OrigemFormatada":
                    if (!select.Contains(" OrigemOcorrencia,"))
                    {
                        select.Append("ColetaEntrega.OCE_ORIGEM_OCORRENCIA as OrigemOcorrencia, ");
                        groupBy.Append("ColetaEntrega.OCE_ORIGEM_OCORRENCIA, ");
                    }
                    break;
                case "ClienteLocalidade":
                    if (!select.Contains(" ClienteLocalidade,"))
                    {
                        select.Append("Localidade.LOC_DESCRICAO as ClienteLocalidade, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");
                    }
                    break;
                case "DescricaoCompleta":
                case "DescricaoOcorrenciaFormatada":
                    if (!select.Contains(" DescricaoCompleta,"))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO + TipoOcorrencia.OCO_DESCRICAO_PORTAL as DescricaoCompleta, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO_PORTAL, ");

                        if (!groupBy.Contains("TipoOcorrencia.OCO_DESCRICAO,"))
                            groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");
                    }
                    if (!select.Contains(" DescricaoTipoOcorrencia,"))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO as DescricaoTipoOcorrencia, ");

                        if (!groupBy.Contains("TipoOcorrencia.OCO_DESCRICAO,"))
                            groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");
                    }
                    if (!select.Contains(" RemetenteNome,"))
                    {
                        select.Append(@"(select top 1 Remetente.CLI_NOME FROM T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
				                inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO 
				                inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
							    inner join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
                                WHERE CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO) as RemetenteNome, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    if (!select.Contains(" ClienteLocalidade,"))
                    {
                        select.Append(@"(select top 1 LocalidadeRemetente.LOC_DESCRICAO FROM T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
				                inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO 
				                inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
							    inner join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
								inner join T_LOCALIDADES LocalidadeRemetente ON LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO           
                                WHERE CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO) as RemetenteLocalidade, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;
                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia,"))
                    {
                        select.Append("ColetaEntrega.OCE_CODIGO as NumeroOcorrencia, ");
                        groupBy.Append("ColetaEntrega.OCE_CODIGO, ");
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO as GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;
                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais,"))
                    {
                        select.AppendLine(@" SUBSTRING(");
                        select.AppendLine(@"             (SELECT DISTINCT ', ' + convert(nvarchar(20), NotaFiscal.NF_NUMERO)");
                        select.AppendLine(@"              FROM T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal");
                        select.AppendLine(@"              JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO");
                        select.AppendLine(@"              JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO");
                        select.AppendLine(@"              WHERE CargaEntregaNotaFiscal.CEN_CODIGO = ColetaEntrega.CEN_CODIGO");
                        select.AppendLine(@"                FOR XML path('')), 3, 1000) AS NotasFiscais,");

                        if (!groupBy.Contains("ColetaEntrega.CEN_CODIGO,"))
                            groupBy.Append("ColetaEntrega.CEN_CODIGO, ");
                    }
                    break;

                case "SerieNotasFiscais":
                    if (!select.Contains(" SerieNotasFiscais,"))
                    {
                        select.AppendLine(@" SUBSTRING(");
                        select.AppendLine(@"             (SELECT DISTINCT ', ' + convert(nvarchar(20), NotaFiscal.NF_SERIE)");
                        select.AppendLine(@"              FROM T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal");
                        select.AppendLine(@"              JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO");
                        select.AppendLine(@"              JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO");
                        select.AppendLine(@"              WHERE CargaEntregaNotaFiscal.CEN_CODIGO = ColetaEntrega.CEN_CODIGO");
                        select.AppendLine(@"                FOR XML path('')), 3, 1000) AS SerieNotasFiscais,");

                        if (!groupBy.Contains("ColetaEntrega.CEN_CODIGO,"))
                            groupBy.Append("ColetaEntrega.CEN_CODIGO, ");
                    }
                    break;
                case "Chamado":
                    if (!select.Contains(" Chamado, "))
                    {
                        select.Append(@"SUBSTRING(
                                            (SELECT DISTINCT ', ' + CAST(Chamado.CHA_NUMERO AS NVARCHAR(20))
                                            FROM T_CHAMADOS Chamado
				                            WHERE Chamado.CAR_CODIGO = Carga.CAR_CODIGO AND Chamado.CLI_CGCCPF = Cliente.CLI_CGCCPF
                                                FOR XML PATH('')), 3, 2000) Chamado, ");

                        if (!groupBy.Contains("Cliente.CLI_CGCCPF,"))
                            groupBy.Append("Cliente.CLI_CGCCPF, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "Transportadora":
                    if (!select.Contains(" Transportadora, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportadora, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                        //SetarJoinsOutroEmitente(joins);
                    }
                    break;
                case "CodigoIntegracaoDestinatarios": // é bom verificar com o rodolfo
                    if (!select.Contains(" CodigoIntegracaoDestinatarios, "))
                    {
                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO as CodigoIntegracaoDestinatarios, ");
                        groupBy.Append("Cliente.CLI_CODIGO_INTEGRACAO, ");
                    }
                    break;
                case "Destinatarios":
                    if (!select.Contains(" Destinatarios,"))
                    {
                        select.Append("Cliente.CLI_NOME as Destinatarios, ");
                        if (!groupBy.Contains("Cliente.CLI_NOME,"))
                            groupBy.Append("Cliente.CLI_NOME, ");

                    }
                    break;
                case "CNPJDestinatariosFormatado": // VERIFICAR COM O RDOLFO
                    if (!select.Contains(" CNPJDestinatarios, "))
                    {
                        select.Append("CAST(LTRIM(STR(Cliente.CLI_CGCCPF,50)) AS NVARCHAR(50)) as CNPJDestinatarios, ");

                        if (!groupBy.Contains("Cliente.CLI_CGCCPF,"))
                            groupBy.Append("Cliente.CLI_CGCCPF, ");
                    }
                    break;

                case "CNPJEmpresa":
                case "CNPJTransportadora":
                    if (!select.Contains(" CNPJEmpresa,"))
                    {
                        select.Append("Transportador.EMP_CNPJ as CNPJEmpresa, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append(" ((case when isnull(Cliente.CLI_CODIGO_INTEGRACAO, '') <> '' then Cliente.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + Cliente.CLI_NOME) as Cliente, ");

                        if (!groupBy.Contains("Cliente.CLI_CODIGO_INTEGRACAO,"))
                            groupBy.Append("Cliente.CLI_CODIGO_INTEGRACAO, ");

                        if (!groupBy.Contains("Cliente.CLI_NOME,"))
                            groupBy.Append("Cliente.CLI_NOME, ");
                    }
                    break;
                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_EXPEDIDORES as Expedidor, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_EXPEDIDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_RECEBEDORES as Recebedor, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_RECEBEDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "DataCarga":
                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga,"))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO as DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("ColetaEntrega.OCE_OBSERVACAO_OCORRENCIA as Observacao, ");
                        groupBy.Append("ColetaEntrega.OCE_OBSERVACAO_OCORRENCIA, ");
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + Motorista.FUN_NOME + case Motorista.FUN_FONE when null then '' else ' (' + Motorista.FUN_FONE  + ')' end ");
                        select.Append("      from T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      join T_FUNCIONARIO Motorista on CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     where CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Motorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa,"))
                    {
                        select.Append("( ");
                        select.Append("    ( ");
                        select.Append("        select Veiculo.VEI_PLACA ");
                        select.Append("          from T_VEICULO Veiculo ");
                        select.Append("         where Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append("    ) +  ");
                        select.Append("    ISNULL(( ");
                        select.Append("        select ', ' + Veiculo.VEI_PLACA ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado ");
                        select.Append("          join T_VEICULO Veiculo on VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("         where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") as Placa, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO,"))
                            groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                    }
                    break;
                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) ( ");
                        select.Append("               case ");
                        select.Append("                   when Veiculo.MVC_CODIGO is not null then ModeloVeicularVeiculo.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ");
                        select.Append("                   else ModeloVeicularCarga.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ");
                        select.Append("               end ");
                        select.Append("           ) ");
                        select.Append("      from T_VEICULO Veiculo ");
                        select.Append("      left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
                        select.Append("      left join T_MODELO_VEICULAR_CARGA ModeloVeicularVeiculo on ModeloVeicularVeiculo.MVC_CODIGO = Veiculo.MVC_CODIGO ");
                        select.Append("     where Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append(") as TipoVeiculo, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO,"))
                            groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Carga.MVC_CODIGO,"))
                            groupBy.Append("Carga.MVC_CODIGO, ");
                    }
                    break;
                case "CodigoIntegracaoFilial":
                    if (!select.Contains(" CodigoIntegracaoFilial,"))
                    {
                        select.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR as CodigoIntegracaoFilial, ");
                        groupBy.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilialFormatado":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append("Filial.FIL_CNPJ as CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;
                case "TipoOperacaoCarga":
                    if (!select.Contains(" TipoOperacaoCarga, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacaoCarga, ");

                        if (!groupBy.Contains("TipoOperacao.TOP_DESCRICAO,"))
                            groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;
                case "CargaAgrupada":
                    if (!select.Contains(" CargaAgrupada,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + AG.CAR_CODIGO_CARGA_AGRUPADO ");
                        select.Append("      from T_CARGA_CODIGOS_AGRUPADOS AG ");
                        select.Append("      where AG.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("      GROUP BY AG.CAR_CODIGO_CARGA_AGRUPADO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CargaAgrupada, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "NomeFantasiaDestinatarios":
                    if (!select.Contains(" NomeFantasiaDestinatarios, "))
                    {
                        select.Append("Cliente.CLI_NOMEFANTASIA as NomeFantasiaDestinatarios, ");

                        if (!groupBy.Contains("Cliente.CLI_NOMEFANTASIA,"))
                            groupBy.Append("Cliente.CLI_NOMEFANTASIA, ");
                    }
                    break;

                case "Pedidos":
                case "PedidosFormatado":
                    if (!select.Contains(" Pedidos, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + 
										                cast(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as varchar(30)) 
										                FROM T_CARGA_ENTREGA_PEDIDO cep
										                inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = cep.CPE_CODIGO 
										                inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
										                WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO 
						                for xml path('')),3,1000) Pedidos, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");
                    }
                    break;
                case "Destinos":
                    if (!select.Contains(" Destinos, "))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + 
									                CASE isnull(ClienteEntrega.CLI_NOMEFANTASIA, '') WHEN '' THEN ClienteEntrega.CLI_NOME ELSE ClienteEntrega.CLI_NOMEFANTASIA END
									                + ' (' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA + ')' AS[text()]
									                FROM T_CARGA_ENTREGA CargaEntrega
									                JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
									                JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
									                WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
									                ORDER BY CargaEntrega.CEN_ORDEM
						                FOR XML PATH, TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) Destinos,");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
                case "DataCarregamentoFormatada":
                case "DataCarregamento":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;
                case "CPFCNPJClienteDescricao":
                    if (!select.Contains(" CPFCNPJCliente, "))
                    {
                        select.Append("Cliente.CLI_CGCCPF as CPFCNPJCliente, ");
                    }
                    break;
                case "CPFMotorista":
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + Motorista.FUN_CPF ");
                        select.Append("      from T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      join T_FUNCIONARIO Motorista on CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     where CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CPFMotorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataPrevisaoEntregaAjustadaFormatada":
                    if (!select.Contains(" DataPrevisaoEntregaAjustada,"))
                    {
                        select.Append("cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA DataPrevisaoEntregaAjustada, ");

                        if (!groupBy.Contains("cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA,"))
                            groupBy.Append("cargaEntrega.CEN_DATA_PREVISAO_ENTREGA_AJUSTADA, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!filtrosPesquisa.DataCriacaoInicial.IsNullOrMinValue() &&
                !filtrosPesquisa.DataCriacaoFinal.IsNullOrMinValue())
                where.Append($" and ColetaEntrega.OCE_DATA_OCORRENCIA BETWEEN '{filtrosPesquisa.DataCriacaoInicial:yyyy-MM-dd HH:mm:ss}' AND '{filtrosPesquisa.DataCriacaoFinal:yyyy-MM-dd HH:mm:ss}'");
            else if (!filtrosPesquisa.DataCriacaoInicial.IsNullOrMinValue())
                where.Append($" and ColetaEntrega.OCE_DATA_OCORRENCIA >= '{filtrosPesquisa.DataCriacaoInicial:yyyy-MM-dd HH:mm:ss}'");
            else if (!filtrosPesquisa.DataCriacaoFinal.IsNullOrMinValue())
                where.Append($" and ColetaEntrega.OCE_DATA_OCORRENCIA <= '{filtrosPesquisa.DataCriacaoFinal:yyyy-MM-dd HH:mm:ss}'");


            if (filtrosPesquisa.CodigosGrupoOcorrencia.Count > 0)
            {
                where.Append($" and GrupoOcorrencia.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoOcorrencia)})");
                SetarJoinsGrupoOcorrencia(joins);
            }


            if (filtrosPesquisa.CodigosOcorrencia.Count > 0)
                where.Append($" and ColetaEntrega.OCO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOcorrencia)})");

            if (filtrosPesquisa.NumeroOcorrenciaInicial > 0 && filtrosPesquisa.NumeroOcorrenciaFinal > 0)
                where.Append($" and ColetaEntrega.OCE_CODIGO BETWEEN {filtrosPesquisa.NumeroOcorrenciaInicial} AND {filtrosPesquisa.NumeroOcorrenciaFinal}");
            else if (filtrosPesquisa.NumeroOcorrenciaInicial > 0)
                where.Append($" and ColetaEntrega.OCE_CODIGO >= {filtrosPesquisa.NumeroOcorrenciaInicial}");
            else if (filtrosPesquisa.NumeroOcorrenciaFinal > 0)
                where.Append($" and ColetaEntrega.OCE_CODIGO <= {filtrosPesquisa.NumeroOcorrenciaFinal}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CargaAgrupada))
            {
                where.Append("  AND exists (select top(1) 1 ");
                where.Append("      from T_CARGA_CODIGOS_AGRUPADOS AG ");
                where.Append("      where AG.CAR_CODIGO = Carga.CAR_CODIGO AND ");
                where.Append($"     AG.CAR_CODIGO_CARGA_AGRUPADO = '{filtrosPesquisa.CargaAgrupada}') ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");


            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append($" and Carga.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas}");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append(" and ( ");
                where.Append($"        Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or ");
                where.Append("         exists ( ");
                where.Append("             select top(1) 1 ");
                where.Append("               from T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados ");
                where.Append("              where VeiculosVinculados.CAR_CODIGO = Carga.CAR_VEICULO ");
                where.Append($"               and VeiculosVinculados.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");
                where.Append("         ) ");
                where.Append("     )");
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                where.AppendLine(@" and exists  (SELECT top(1) 1 ");
                where.AppendLine(@"              FROM T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal");
                where.AppendLine(@"              JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO");
                where.AppendLine(@"              JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO");
                where.AppendLine($@"              WHERE NotaFiscal.NF_NUMERO = '{filtrosPesquisa.NumeroNotaFiscal}') ");
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_MOTORISTA CargaMotorista ");
                where.Append("          where CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"           and CargaMotorista.CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.CodigosTransportadorCarga?.Count > 0)
                where.Append($" and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportadorCarga)}) ");

            if (filtrosPesquisa.TiposOperacaoCarga.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposOperacaoCarga)})");

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_PEDIDO CargaPedido ");
                where.Append("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                where.Append("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"           and Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjPessoa.ToString("F0")} ");
                where.Append("     ) ");
            }
        }
        #endregion

    }
}
