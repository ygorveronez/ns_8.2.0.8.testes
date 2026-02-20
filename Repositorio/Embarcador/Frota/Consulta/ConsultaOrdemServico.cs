using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaOrdemServico : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico>
    {
        #region Construtores

        public ConsultaOrdemServico() : base(tabela: "T_FROTA_ORDEM_SERVICO as OrdemServico") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO ");
        }

        private void SetarJoinsEquioamento(StringBuilder joins)
        {
            if (!joins.Contains(" Equipamento "))
                joins.Append("LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO ");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            if (!joins.Contains(" Operador "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Operador on Operador.FUN_CODIGO = OrdemServico.FUN_OPERADOR ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = OrdemServico.FUN_MOTORISTA ");
        }

        private void SetarJoinsOrcamento(StringBuilder joins)
        {
            if (!joins.Contains(" OrdemServicoOrcamento "))
                joins.Append("LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsLocalManutencao(StringBuilder joins)
        {
            if (!joins.Contains(" LocalManutencao "))
                joins.Append("LEFT JOIN T_CLIENTE LocalManutencao on LocalManutencao.CLI_CGCCPF = OrdemServico.CLI_CGCCPF ");
        }

        private void SetarJoinsLocalManutencaoCPFCNPJ(StringBuilder joins)
        {
            if (!joins.Contains(" CPFCNPJPessoa "))
                joins.Append("LEFT JOIN T_CLIENTE LocalPessoa on LocalPessoa.CLI_CGCCPF = OrdemServico.CLI_CGCCPF ");
        }

        private void SetarJoinsLocalidadeLocalManutencao(StringBuilder joins)
        {
            SetarJoinsLocalManutencao(joins);

            if (!joins.Contains(" LocalidadeLocalManutencao "))
                joins.Append("LEFT JOIN T_LOCALIDADES LocalidadeLocalManutencao on LocalidadeLocalManutencao.LOC_CODIGO = LocalManutencao.LOC_CODIGO ");
        }

        private void SetarJoinsTipo(StringBuilder joins)
        {
            if (!joins.Contains(" Tipo "))
                joins.Append("LEFT JOIN T_FROTA_ORDEM_SERVICO_TIPO Tipo on Tipo.FOT_CODIGO = OrdemServico.FOT_CODIGO ");
        }

        private void SetarJoinsGrupoServico(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoServico "))
                joins.Append("LEFT JOIN T_GRUPO_SERVICO GrupoServico ON GrupoServico.GSF_CODIGO = OrdemServico.GSF_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append("LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = OrdemServico.CRE_CODIGO ");
        }

        private void SetarJoinsVeiculoSegmento(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" VeiculoSegmento "))
                joins.Append("LEFT JOIN T_VEICULO_SEGMENTO VeiculoSegmento on VeiculoSegmento.VSE_CODIGO = Veiculo.VSE_CODIGO ");
        }

        private void SetarJoinsDocumentoEntrada(StringBuilder joins)
        {
            if (!joins.Contains(" DocEntrada "))
                joins.Append("LEFT JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_DOCUMENTO DocEntrada on DocEntrada.OSE_CODIGO = OrdemServico.OSE_CODIGO ");

            if (!joins.Contains(" DocumentoEntrada "))
                joins.Append("LEFT JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocEntrada.TDE_CODIGO ");
        }

        private void SetarJoinsDocumentoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" DocProdutos "))
                joins.Append("LEFT JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO DocProdutos on DocProdutos.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsOperadorFinalizou(StringBuilder joins)
        {
            if (!joins.Contains(" OperadorFechamento "))
                joins.Append("LEFT JOIN T_FUNCIONARIO OperadorFechamento ON OperadorFechamento.FUN_CODIGO = OrdemServico.FUN_OPERADOR_FECHAMENTO ");
        }

        private void SetarJoinsFornecedorDocumentoEntrada(StringBuilder joins)
        {
            SetarJoinsDocumentoEntrada(joins);
            if (!joins.Contains(" FornecedorDocumentoEntrada  "))
                joins.Append("LEFT JOIN T_CLIENTE FornecedorDocumentoEntrada ON FornecedorDocumentoEntrada.CLI_CGCCPF = DocumentoEntrada.CLI_CGCCPF ");
        }

        private void SetarJoinsOrdemCompraDocumentoEntrada(StringBuilder joins)
        {
            SetarJoinsDocumentoEntrada(joins);
            if (!joins.Contains(" OrdemCompraDocumentoEntrada  "))
                joins.Append("LEFT JOIN T_ORDEM_COMPRA OrdemCompraDocumentoEntrada on OrdemCompraDocumentoEntrada.ORC_CODIGO = DocumentoEntrada.ORC_CODIGO ");
        }

        private void SetarJoinsOrdemCompraProduto(StringBuilder joins)
        {
            SetarJoinsDocumentoProduto(joins);
            if (!joins.Contains(" CodigoProduto  "))
                joins.Append("LEFT JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = DocProdutos.PRO_CODIGO");
            
        }


        private void SetarJoinsProduto(StringBuilder joins)
        {
            SetarJoinsDocumentoProduto(joins);

            if (!joins.Contains(" OperadorFechamento "))
                joins.Append("LEFT JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = DocProdutos.PRO_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("OrdemServico.OSE_CODIGO as Codigo, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero,"))
                    {
                        select.Append("OrdemServico.OSE_NUMERO as Numero, ");
                        groupBy.Append("OrdemServico.OSE_NUMERO, ");
                    }
                    break;

                case "DataFormatada":
                    if (!select.Contains(" Data,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_PROGRAMADA as Data, ");
                        groupBy.Append("OrdemServico.OSE_DATA_PROGRAMADA, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("OrdemServico.OSE_OBSERVACAO as Observacao, ");
                        groupBy.Append("OrdemServico.OSE_OBSERVACAO, ");
                    }
                    break;

                case "DescricaoTipoManutencao":
                    if (!select.Contains(" TipoManutencao,"))
                    {
                        select.Append("OrdemServico.OSE_TIPO_MANUTENCAO as TipoManutencao, ");
                        groupBy.Append("OrdemServico.OSE_TIPO_MANUTENCAO, ");
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("OrdemServico.OSE_SITUACAO as Situacao, ");
                        groupBy.Append("OrdemServico.OSE_SITUACAO, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        SetarJoinsVeiculo(joins);

                        select.Append("Veiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");
                    }
                    break;

                case "Equipamento":
                    if (!select.Contains(" Equipamento,"))
                    {
                        SetarJoinsEquioamento(joins);

                        select.Append("Equipamento.EQP_DESCRICAO as Equipamento, ");
                        groupBy.Append("Equipamento.EQP_DESCRICAO, ");
                    }
                    break;

                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota,"))
                    {
                        SetarJoinsVeiculo(joins);

                        select.Append("Veiculo.VEI_NUMERO_FROTA as NumeroFrota, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_FROTA, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        SetarJoinsMotorista(joins);

                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador,"))
                    {
                        SetarJoinsOperador(joins);

                        select.Append("Operador.FUN_NOME as Operador, ");
                        groupBy.Append("Operador.FUN_NOME, ");
                    }
                    break;

                case "Tipo":
                    if (!select.Contains(" Tipo,"))
                    {
                        SetarJoinsTipo(joins);

                        select.Append("Tipo.FOT_DESCRICAO as Tipo, ");
                        groupBy.Append("Tipo.FOT_DESCRICAO, ");
                    }
                    break;

                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao, "))
                    {
                        select.Append("LocalManutencao.CLI_NOME as LocalManutencao, ");
                        groupBy.Append("LocalManutencao.CLI_NOME, ");

                        SetarJoinsLocalManutencao(joins);
                    }
                    break;

                case "CidadeLocalManutencao":
                    if (!select.Contains(" CidadeLocalManutencao, "))
                    {
                        select.Append("LocalidadeLocalManutencao.LOC_DESCRICAO as CidadeLocalManutencao, ");
                        groupBy.Append("LocalidadeLocalManutencao.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeLocalManutencao(joins);
                    }
                    break;

                case "UFLocalManutencao":
                    if (!select.Contains(" UFLocalManutencao, "))
                    {
                        select.Append("LocalidadeLocalManutencao.UF_SIGLA as UFLocalManutencao, ");
                        groupBy.Append("LocalidadeLocalManutencao.UF_SIGLA, ");

                        SetarJoinsLocalidadeLocalManutencao(joins);
                    }
                    break;

                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        SetarJoinsOrcamento(joins);
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_PRODUTOS) ValorProdutos, ");
                    }
                    break;
                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        SetarJoinsOrcamento(joins);
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_MAO_OBRA) ValorServicos, ");
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        SetarJoinsOrcamento(joins);
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_ORCADO) ValorTotal, ");
                    }
                    break;
                case "ValorProdutosFechamento":
                    if (!select.Contains(" ValorProdutosFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select.Append("OrdemServico.OSE_CODIGO Codigo, ");
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");
                        }
                        select.Append(@"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO <> 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorProdutosFechamento, ");
                    }
                    break;
                case "ValorServicosFechamento":
                    if (!select.Contains(" ValorServicosFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select.Append("OrdemServico.OSE_CODIGO Codigo, ");
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");
                        }
                        select.Append(@"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO = 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorServicosFechamento, ");
                    }
                    break;
                case "ValorTotalFechamento":
                    if (!select.Contains(" ValorTotalFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select.Append("OrdemServico.OSE_CODIGO Codigo, ");
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");
                        }
                        select.Append(@"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorTotalFechamento, ");
                    }
                    break;

                case "Servicos":
                    if (!select.Contains(" Servicos, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + servico.SEV_DESCRICAO ");
                        select.Append("      from T_FROTA_SERVICO_VEICULO servico ");
                        select.Append("      join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO servicoVeiculo on servicoVeiculo.SEV_CODIGO = servico.SEV_CODIGO ");
                        select.Append($"     where servicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
                        if (filtroPesquisa.Servicos?.Count > 0)
                            select.Append($" and servicoVeiculo.SEV_CODIGO in ({string.Join(", ", filtroPesquisa.Servicos)})");
                        select.Append(" for XML PATH('')), 3, 1000) as Servicos, ");

                        if (!groupBy.Contains("OrdemServico.OSE_CODIGO, "))
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "QuilometragemVeiculo":
                    if (!select.Contains(" QuilometragemVeiculo, "))
                    {
                        select.Append("OrdemServico.OSE_QUILOMETRAGEM_VEICULO as QuilometragemVeiculo, ");
                        groupBy.Append("OrdemServico.OSE_QUILOMETRAGEM_VEICULO, ");
                    }
                    break;

                case "Horimetro":
                    if (!select.Contains(" Horimetro, "))
                    {
                        select.Append("OrdemServico.OSE_HORIMETRO as Horimetro, ");
                        groupBy.Append("OrdemServico.OSE_HORIMETRO, ");
                    }
                    break;

                case "GrupoServico":
                    if (!select.Contains(" GrupoServico, "))
                    {
                        SetarJoinsGrupoServico(joins);

                        select.Append("GrupoServico.GSF_DESCRICAO AS GrupoServico, ");
                        groupBy.Append("GrupoServico.GSF_DESCRICAO, ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        SetarJoinsCentroResultado(joins);

                        select.Append("CentroResultado.CRE_DESCRICAO AS CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");
                    }
                    break;

                case "Segmento":
                    if (!select.Contains(" Segmento, "))
                    {
                        SetarJoinsVeiculoSegmento(joins);

                        select.Append("VeiculoSegmento.VSE_DESCRICAO AS Segmento, ");
                        groupBy.Append("VeiculoSegmento.VSE_DESCRICAO, ");
                    }
                    break;

                case "CPFCNPJPessoa":
                    if (!select.Contains(" CPFCNPJPessoa, "))
                    {
                        SetarJoinsLocalManutencaoCPFCNPJ(joins);

                        select.Append("LTRIM(STR(LocalPessoa.CLI_CGCCPF, 25, 0)) AS CPFCNPJPessoa, ");
                        groupBy.Append("LocalPessoa.CLI_CGCCPF, ");
                    }
                    break;

                case "DocumentoEntrada":
                    if (!select.Contains(" DocumentoEntrada "))
                    {
                        SetarJoinsDocumentoEntrada(joins);

                        select.Append("CONVERT(NVARCHAR(30), DocumentoEntrada.TDE_NUMERO_LONG) AS DocumentoEntrada, ");
                        groupBy.Append("DocumentoEntrada.TDE_NUMERO_LONG, ");
                    }
                    break;

                case "DiferencaTotais":
                    if (!select.Contains(" DiferencaTotais "))
                    {
                        SetarJoinsDocumentoProduto(joins);

                        select.Append("SUM( (DocProdutos.OFP_QUANTIDADE_DOCUMENTO * DocProdutos.OFP_VALOR_DOCUMENTO) - (DocProdutos.OFP_QUANTIDADE_ORCADA * DocProdutos.OFP_VALOR_ORCADO) ) AS DiferencaTotais, ");
                    }
                    break;

                case "OperadorFechamento":
                    if (!select.Contains(" OperadorFechamento "))
                    {
                        SetarJoinsOperadorFinalizou(joins);

                        select.Append("OperadorFechamento.FUN_NOME AS OperadorFechamento, ");
                        groupBy.Append("OperadorFechamento.FUN_NOME, ");
                    }
                    break;

                case "DataHoraInclusaoFormatada":
                    if (!select.Contains(" DataHoraInclusao,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_CRIACAO as DataHoraInclusao, ");
                        groupBy.Append("OrdemServico.OSE_DATA_CRIACAO, ");
                    }
                    break;

                case "DiferencaValorOrcadoRealizado":
                    SetarSelect("ValorTotal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorTotalFechamento", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "Mecanicos":
                    if (!select.Contains(" Mecanicos,"))
                    {
                        select.Append("SUBSTRING((SELECT ', ' + Mecanico.FUN_NOME " +
                            "FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                            "JOIN T_FUNCIONARIO Mecanico on Mecanico.FUN_CODIGO = TempoExecucao.FUN_MECANICO " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO " +
                            "FOR XML PATH('')), 3, 1000) AS Mecanicos,");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "TempoPrevisto":
                    if (!select.Contains(" TempoPrevisto,"))
                    {
                        select.Append("ISNULL((SELECT SUM(ServicoVeiculo.OSS_TEMPO_ESTIMADO) FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO), 0) TempoPrevisto, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "TempoExecutado":
                    if (!select.Contains(" TempoExecutado,"))
                    {
                        select.Append("ISNULL((SELECT SUM(TempoExecucao.OTE_TEMPO_EXECUTADO) FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO), 0) TempoExecutado, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "CondicaoPagamento":
                    if (!select.Contains(" CondicaoPagamento,"))
                    {
                        select.Append("OrdemServico.OSE_CONDICAO_PAGAMENTO as CondicaoPagamento, ");
                        groupBy.Append("OrdemServico.OSE_CONDICAO_PAGAMENTO, ");
                    }
                    break;

                case "PrioridadeDescricao":
                    if (!select.Contains(" Prioridade,"))
                    {
                        select.Append("OrdemServico.OSE_PRIORIDADE as Prioridade, ");
                        groupBy.Append("OrdemServico.OSE_PRIORIDADE, ");
                    }
                    break;

                case "DataLimiteExecucaoFormatada":
                    if (!select.Contains(" DataLimiteExecucao,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_LIMITE_EXECUCAO as DataLimiteExecucao, ");
                        groupBy.Append("OrdemServico.OSE_DATA_LIMITE_EXECUCAO, ");
                    }
                    break;

                case "DataLiberacaoFormatada":
                    if (!select.Contains(" DataLiberacao,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_LIBERACAO as DataLiberacao, ");
                        groupBy.Append("OrdemServico.OSE_DATA_LIBERACAO, ");
                    }
                    break;

                case "DataFechamentoFormatada":
                    if (!select.Contains(" DataFechamento,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_FECHAMENTO as DataFechamento, ");
                        groupBy.Append("OrdemServico.OSE_DATA_FECHAMENTO, ");
                    }
                    break;

                case "DataReaberturaFormatada":
                    if (!select.Contains(" DataReabertura,"))
                    {
                        select.Append("OrdemServico.OSE_DATA_REABERTURA as DataReabertura, ");
                        groupBy.Append("OrdemServico.OSE_DATA_REABERTURA, ");
                    }
                    break;

                case "Produtos":
                    if (!select.Contains(" Produtos,"))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + Produto.PRO_DESCRICAO
                                        from T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO produtoOrdem
                                        join T_PRODUTO Produto on Produto.PRO_CODIGO = produtoOrdem.PRO_CODIGO
                                        where produtoOrdem.OSE_CODIGO = OrdemServico.OSE_CODIGO
                                        for XML PATH('')), 3, 1000) as Produtos, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "GruposProdutos":
                    if (!select.Contains(" GruposProdutos,"))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + Grupo.GRP_DESCRICAO
                                        from T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO produtoOrdem
                                        join T_PRODUTO Produto on Produto.PRO_CODIGO = produtoOrdem.PRO_CODIGO    
                                        join T_GRUPO_PRODUTO_TMS Grupo on Grupo.GPR_CODIGO = Produto.GPR_CODIGO
                                        where produtoOrdem.OSE_CODIGO = OrdemServico.OSE_CODIGO
                                        for XML PATH('')), 3, 1000) as GrupoProdutos, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "QuantidadeProduto":
                    if (!select.Contains(" QuantidadeProduto,"))
                    {
                        SetarJoinsDocumentoProduto(joins);

                        select.Append(@"SUM(DocProdutos.OFP_QUANTIDADE_DOCUMENTO) as QuantidadeProduto, ");
                        groupBy.Append("DocProdutos.OSE_CODIGO, ");
                    }
                    break;

                case "ValorUnitarioProduto":
                    if (!select.Contains(" ValorUnitarioProduto,"))
                    {
                        SetarJoinsDocumentoProduto(joins);

                        select.Append(@"SUM(DocProdutos.OFP_VALOR_UNITARIO) as ValorUnitarioProduto, ");
                        groupBy.Append("DocProdutos.OFP_CODIGO, ");
                    }
                    break;
                case "NomeFornecedorDocumentoEntrada":
                    if (!select.Contains(" NomeFornecedorDocumentoEntrada,"))
                    {
                        SetarJoinsFornecedorDocumentoEntrada(joins);
                        select.Append("FornecedorDocumentoEntrada.CLI_NOME as NomeFornecedorDocumentoEntrada, ");
                        groupBy.Append("FornecedorDocumentoEntrada.CLI_NOME, ");
                    }
                    break;
                case "ValorNF":
                    if (!select.Contains(" ValorNF,"))
                    {
                        SetarJoinsDocumentoEntrada(joins);
                        select.Append("DocumentoEntrada.TDE_VALOR_TOTAL as ValorNF, ");
                        groupBy.Append("DocumentoEntrada.TDE_VALOR_TOTAL, ");
                    }
                    break;
                case "NumeroOrdemCompra":
                    if (!select.Contains(" NumeroOrdemCompra,"))
                    {
                        SetarJoinsOrdemCompraDocumentoEntrada(joins);
                        select.Append("OrdemCompraDocumentoEntrada.ORC_NUMERO as NumeroOrdemCompra, ");
                        groupBy.Append("OrdemCompraDocumentoEntrada.ORC_NUMERO, ");
                    }
                    break;
                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto,"))
                    {
                        SetarJoinsOrdemCompraProduto(joins);
                        select.Append("(Produto.PRO_COD_PRODUTO + ' | ' + Produto.PRO_DESCRICAO) as CodigoProduto, ");
                        groupBy.Append("Produto.PRO_DESCRICAO, Produto.PRO_COD_PRODUTO, ");
                    }
                    break;


            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append($" and OrdemServico.OSE_NUMERO >= {filtrosPesquisa.NumeroInicial}");

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append($" and OrdemServico.OSE_NUMERO <= {filtrosPesquisa.NumeroFinal}");

            if (filtrosPesquisa.LocaisManutencao?.Count > 0)
                where.Append($" and OrdemServico.CLI_CGCCPF in ({ string.Join(", ", filtrosPesquisa.LocaisManutencao.Select(o => o.ToString("F0")))})");

            if (filtrosPesquisa.Motoristas?.Count > 0)
                where.Append($" and OrdemServico.FUN_MOTORISTA in ({string.Join(", ", filtrosPesquisa.Motoristas.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.Tipos?.Count > 0)
                where.Append($" and OrdemServico.FOT_CODIGO in ({string.Join(", ", filtrosPesquisa.Tipos.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.Veiculos?.Count > 0)
                where.Append($" and OrdemServico.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.Veiculos.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.Equipamentos?.Count > 0)
                where.Append($" and OrdemServico.EQP_CODIGO in ({string.Join(", ", filtrosPesquisa.Equipamentos.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.Situacao?.Count > 0)
                where.Append($" and OrdemServico.OSE_SITUACAO in ({string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.TipoManutencao?.Count > 0)
                where.Append($" and OrdemServico.OSE_TIPO_MANUTENCAO in ({string.Join(", ", filtrosPesquisa.TipoManutencao.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.Servicos?.Count > 0)
                where.Append($" and EXISTS (SELECT OSE_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO WHERE OSE_CODIGO = OrdemServico.OSE_CODIGO AND SEV_CODIGO IN ({string.Join(", ", filtrosPesquisa.Servicos.Select(o => o.ToString("G")))}))"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoOrdemServico.HasValue)
                where.Append($" and OrdemServico.OSE_TIPO_OFICINA = {(int)filtrosPesquisa.TipoOrdemServico.Value}");

            if (filtrosPesquisa.MarcaVeiculo > 0)
                where.Append($" and EXISTS (SELECT VMA_DESCRICAO FROM T_FROTA_ORDEM_SERVICO OrdemServico " +
                    $"JOIN T_VEICULO Veiculo on OrdemServico.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                    $"JOIN T_VEICULO_MARCA VeiculoMarca on VeiculoMarca.VMA_CODIGO = Veiculo.VMA_CODIGO" +
                    $" AND VeiculoMarca.VMA_CODIGO = {filtrosPesquisa.MarcaVeiculo})");

            if (filtrosPesquisa.ModeloVeiculo > 0)
                where.Append($" and EXISTS (SELECT VMO_DESCRICAO FROM T_FROTA_ORDEM_SERVICO OrdemServico " +
                    $"JOIN T_VEICULO Veiculo1 on OrdemServico.VEI_CODIGO = Veiculo1.VEI_CODIGO " +
                    $"JOIN T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO = Veiculo1.VMO_CODIGO" +
                    $" AND VeiculoModelo.VMO_CODIGO = {filtrosPesquisa.ModeloVeiculo}" +
                    $" AND Veiculo1.VEI_CODIGO = Veiculo.VEI_CODIGO)");

            if (filtrosPesquisa.GrupoServicos?.Count > 0)
                where.Append($" and OrdemServico.GSF_CODIGO in ({string.Join(", ", filtrosPesquisa.GrupoServicos.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.CentroResultados?.Count > 0)
                where.Append($" and OrdemServico.CRE_CODIGO in ({string.Join(", ", filtrosPesquisa.CentroResultados.Select(o => o.ToString("G")))})");

            if (filtrosPesquisa.Segmentos.Count > 0)
            {
                where.Append($" AND VeiculoSegmento.VSE_CODIGO in ({string.Join(", ", filtrosPesquisa.Segmentos)})");

                SetarJoinsVeiculoSegmento(joins);
            }

            if (filtrosPesquisa.CidadesPessoa?.Count > 0)
                where.Append($" and EXISTS (select CLI_CIDADE FROM T_FROTA_ORDEM_SERVICO OrdemServico " +
                    $"JOIN T_CLIENTE Cliente on OrdemServico.CLI_CGCCPF = Cliente.CLI_CGCCPF " +
                    $" AND Cliente.CLI_CIDADE in ({string.Join(", ", filtrosPesquisa.CidadesPessoa.Select(o => o.ToString("D")))}))");

            if (filtrosPesquisa.UFsPessoa?.Count > 0)
                where.Append($" and EXISTS (select UF_RG FROM T_FROTA_ORDEM_SERVICO OrdemServico " +
                    $"JOIN T_CLIENTE Cliente on OrdemServico.CLI_CGCCPF = Cliente.CLI_CGCCPF " +
                    $" AND Cliente.UF_RG in ('{string.Join(", ", filtrosPesquisa.UFsPessoa)}'))");

            if (filtrosPesquisa.OperadorLancamentoDocumento > 0)
                where.Append($" AND OrdemServico.FUN_OPERADOR = " + filtrosPesquisa.OperadorLancamentoDocumento.ToString());

            if (filtrosPesquisa.OperadorFinalizouDocumento > 0)
                where.Append($" AND OrdemServico.FUN_OPERADOR_FECHAMENTO = " + filtrosPesquisa.OperadorFinalizouDocumento.ToString());

            if (filtrosPesquisa.DataInicialInclusao.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataInicialInclusao.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalInclusao.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataFinalInclusao.Value.ToString(pattern)}'");

            if (filtrosPesquisa.Mecanicos?.Count > 0)
            {
                where.Append($" AND OrdemServico.OSE_CODIGO IN (SELECT ServicoVeiculo.OSE_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                    $" JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                    $" WHERE TempoExecucao.FUN_MECANICO IN ({string.Join(", ", filtrosPesquisa.Mecanicos)}))"); // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.DataInicialLimiteExecucao.HasValue)
                where.Append($" AND CAST(OrdemServico.OSE_DATA_LIMITE_EXECUCAO AS DATE) >= '{filtrosPesquisa.DataInicialLimiteExecucao.Value.ToString(pattern)}'"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataFinalLimiteExecucao.HasValue)
                where.Append($" AND CAST(OrdemServico.OSE_DATA_LIMITE_EXECUCAO AS DATE) <= '{filtrosPesquisa.DataFinalLimiteExecucao.Value.ToString(pattern)}'"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Prioridade > 0)
                where.Append($" AND OrdemServico.OSE_PRIORIDADE = {(int)filtrosPesquisa.Prioridade}");

            if (filtrosPesquisa.DataLiberacaoInicio.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_LIBERACAO AS DATE) >= '{filtrosPesquisa.DataLiberacaoInicio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataLiberacaoFim.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_LIBERACAO AS DATE) <= '{filtrosPesquisa.DataLiberacaoFim.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFechamentoInicio.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_FECHAMENTO AS DATE) >= '{filtrosPesquisa.DataFechamentoInicio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFechamentoFim.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_FECHAMENTO AS DATE) <= '{filtrosPesquisa.DataFechamentoFim.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataReaberturaInicio.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_REABERTURA AS DATE) >= '{filtrosPesquisa.DataReaberturaInicio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataReaberturaFim.HasValue)
                where.Append($" and CAST(OrdemServico.OSE_DATA_REABERTURA AS DATE) <= '{filtrosPesquisa.DataReaberturaFim.Value.ToString(pattern)}'");

            if (filtrosPesquisa.CodigosProdutoTMS?.Count > 0)
            {
                where.Append($" AND DocProdutos.PRO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosProdutoTMS)})");

                SetarJoinsDocumentoProduto(joins);
            }

            if (filtrosPesquisa.CodigosGrupoProdutoTMS?.Count > 0)
            {
                where.Append($" AND Produto.GPR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoProdutoTMS)})");

                SetarJoinsProduto(joins);
            }

        }

        #endregion
    }
}
