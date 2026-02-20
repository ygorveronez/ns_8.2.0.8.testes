using Dominio.ObjetosDeValor.Embarcador.Frotas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frotas.Consulta
{
    sealed class ConsultaOrdemServicoPorMecanico : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioOrdemServicoPorMecanico>
    {
        #region Construtores

        public ConsultaOrdemServicoPorMecanico() : base(tabela: "T_FROTA_ORDEM_SERVICO as OrdemServico") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsOrdemServicoOrcamento(StringBuilder joins)
        {
            if (!joins.Contains(" OrdemServicoOrcamento "))
                joins.Append("LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsOrdemServicoServicoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" OrdemServicoServicoVeiculo "))
                joins.Append("LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoServicoVeiculo ON OrdemServicoServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsOrcamentoServico(StringBuilder joins)
        {
            if (!joins.Contains(" OrcamentoServico "))
                joins.Append("LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO_SERVICO OrcamentoServico on OrcamentoServico.OSO_CODIGO = OrdemServicoOrcamento.OSO_CODIGO AND OrdemServicoServicoVeiculo.OSS_CODIGO = OrcamentoServico.OSS_CODIGO ");
        }

        private void SetarJoinsOrdemServicoFechamento(StringBuilder joins)
        {
            if (!joins.Contains(" OrdemServicoFechamento "))
                joins.Append("LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsProduto(StringBuilder joins)
        {
            SetarJoinsOrdemServicoFechamento(joins);

            if (!joins.Contains(" Produto "))
                joins.Append("LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoProduto "))
                joins.Append("LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Produto.GPR_CODIGO ");
        }

        private void SetarJoinsResponsavel(StringBuilder joins)
        {
            if (!joins.Contains(" Responsavel "))
                joins.Append("LEFT OUTER JOIN T_FUNCIONARIO Responsavel ON Responsavel.FUN_CODIGO = OrdemServico.FUN_RESPONSAVEL ");
        }
        
        private void SetarJoinsLocalArmazenamentoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" LocalArmazenamentoProduto "))
                joins.Append("LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamentoProduto ON LocalArmazenamentoProduto.LAP_CODIGO = OrdemServicoFechamento.LAP_CODIGO ");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            if (!joins.Contains(" Operador "))
                joins.Append("LEFT OUTER JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = OrdemServico.FUN_OPERADOR ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append("LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = OrdemServico.CRE_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        private void SetarJoinsEquipamento(StringBuilder joins)
        {
            if (!joins.Contains(" Equipamento "))
                joins.Append("LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO ");
        }

        private void SetarJoinsTipo(StringBuilder joins)
        {
            if (!joins.Contains(" Tipo "))
                joins.Append("LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_TIPO Tipo ON Tipo.FOT_CODIGO = OrdemServico.FOT_CODIGO ");
        }

        private void SetarJoinsServico(StringBuilder joins)
        {
            SetarJoinsOrdemServicoServicoVeiculo(joins);

            if (!joins.Contains(" Servico "))
                joins.Append("LEFT OUTER JOIN T_FROTA_SERVICO_VEICULO Servico ON Servico.SEV_CODIGO = OrdemServicoServicoVeiculo.SEV_CODIGO ");
        }

        private void SetarJoinsLocalManutencao(StringBuilder joins)
        {
            if (!joins.Contains(" LocalManutencao "))
                joins.Append("LEFT OUTER JOIN T_CLIENTE LocalManutencao ON LocalManutencao.CLI_CGCCPF = OrdemServico.CLI_CGCCPF ");
        }

        private void SetarJoinsMarca(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Marca "))
                joins.Append("LEFT OUTER JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = Veiculo.VMA_CODIGO ");
        }

        private void SetarJoinsModelo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Modelo "))
                joins.Append("LEFT OUTER JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO ");
        }

        private void SetarJoinsServicoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" ServicoVeiculo "))
                joins.Append("JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo on ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }

        private void SetarJoinsTempoExecucao(StringBuilder joins)
        {
            SetarJoinsOrdemServicoServicoVeiculo(joins);

            if (!joins.Contains(" TempoExecucao "))
                joins.Append("JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = OrdemServicoServicoVeiculo.OSS_CODIGO ");
        }
        
        private void SetarJoinsMecanico(StringBuilder joins)
        {
            if (!joins.Contains(" Mecanico "))
                joins.Append("LEFT OUTER JOIN T_FUNCIONARIO Mecanico on Mecanico.FUN_CODIGO = TempoExecucao.FUN_MECANICO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioOrdemServicoPorMecanico filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("OrdemServico.OSE_CODIGO Codigo, ");
                        groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "ValorOrcadoEmServicos":
                    if (!select.Contains(" ValorOrcadoEmServicos, "))
                    {
                        select.Append("SUM(OrcamentoServico.OOS_VALOR_MAO_OBRA) ValorOrcadoEmServicos, ");
                        groupBy.Append("OrcamentoServico.OOS_VALOR_MAO_OBRA, ");

                        SetarJoinsOrcamentoServico(joins);
                    }
                    break;

                case "ValorOrcadoEmProdutos":

                    SetarJoinsOrdemServicoServicoVeiculo(joins);

                    if (!select.Contains(" ValorOrcadoEmProdutos, "))
                    {
                        select.Append("SUM(OrcamentoServico.OOS_VALOR_PRODUTOS) ValorOrcadoEmProdutos, ");
                        groupBy.Append("OrcamentoServico.OOS_VALOR_PRODUTOS, ");

                        SetarJoinsOrcamentoServico(joins);
                    }
                    break;

                case "GrupoProduto":

                    SetarJoinsProduto(joins);

                    if (!select.Contains(" GrupoProduto, "))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO GrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;

                case "ObservacaoServicos":
                    if (!select.Contains(" ObservacaoServicos, "))
                    {
                        select.Append("ServicoVeiculo.OSS_OBSERVACAO ObservacaoServicos, ");
                        groupBy.Append("ServicoVeiculo.OSS_OBSERVACAO, ");

                        SetarJoinsServicoVeiculo(joins);
                    }
                    break;

                case "Responsavel":
                    if (!select.Contains(" Responsavel, "))
                    {
                        select.Append("Responsavel.FUN_NOME Responsavel, ");
                        groupBy.Append("Responsavel.FUN_NOME, ");

                        SetarJoinsResponsavel(joins);
                    }
                    break;

                case "LocalArmazenamento":
                    if (!select.Contains(" LocalArmazenamento, "))
                    {
                        select.Append("LocalArmazenamentoProduto.LAP_DESCRICAO LocalArmazenamento, ");
                        groupBy.Append("LocalArmazenamentoProduto.LAP_DESCRICAO, ");

                        SetarJoinsLocalArmazenamentoProduto(joins);
                    }
                    break;

                case "ValorProduto":
                    if (!select.Contains(" ValorProduto, "))
                    {
                        select.Append("ISNULL(OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, 0) ValorProduto, ");
                        groupBy.Append("OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, ");

                        SetarJoinsOrdemServicoFechamento(joins);
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        select.Append("Operador.FUN_NOME Operador, ");
                        groupBy.Append("Operador.FUN_NOME, ");

                        SetarJoinsOperador(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "NotaFiscal":

                    SetarJoinsOrdemServicoFechamento(joins);
                    SetarJoinsProduto(joins);

                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select.Append("SUBSTRING((SELECT ");
                        select.Append("DISTINCT ', ' + CAST(docEntrada.TDE_NUMERO_LONG AS NVARCHAR(160)) ");
                        select.Append("FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO prods ");
                        select.Append("JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_DOCUMENTO docs on docs.OSE_CODIGO = prods.OSE_CODIGO ");
                        select.Append("JOIN T_TMS_DOCUMENTO_ENTRADA docEntrada on docEntrada.TDE_CODIGO = docs.TDE_CODIGO ");
                        select.Append("JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM itenEntrada on itenEntrada.TDE_CODIGO = docEntrada.TDE_CODIGO and itenEntrada.PRO_CODIGO = prods.PRO_CODIGO ");
                        select.Append("where docs.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
                        select.Append("and prods.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
                        select.Append("and prods.PRO_CODIGO = Produto.PRO_CODIGO FOR XML PATH('')), 3, 1000) NotaFiscal, ");

                        if (!groupBy.Contains("Produto.PRO_CODIGO"))
                            groupBy.Append("Produto.PRO_CODIGO, ");

                        SetarSelect("Codigo", 0,  select, joins, groupBy, false, filtrosPesquisa);
                    }
                    break;


                case "QuantidadeProduto":
                    if (!select.Contains(" QuantidadeProduto, "))
                    {
                        select.Append("ISNULL(OrdemServicoFechamento.OFP_QUANTIDADE_DOCUMENTO, 0) QuantidadeProduto, ");
                        groupBy.Append("OrdemServicoFechamento.OFP_QUANTIDADE_DOCUMENTO, ");

                        SetarJoinsOrdemServicoFechamento(joins);
                    }
                    break;

                case "CodigoProduto":

                    SetarJoinsOrdemServicoFechamento(joins);
                    SetarJoinsProduto(joins);

                    if (!select.Contains(" CodigoProduto, "))
                    {
                        select.Append("ISNULL(Produto.PRO_COD_PRODUTO, CAST(Produto.PRO_CODIGO AS VARCHAR(100))) CodigoProduto, ");
                        
                        if (!groupBy.Contains("Produto.PRO_COD_PRODUTO, Produto.PRO_CODIGO"))
                            groupBy.Append("Produto.PRO_COD_PRODUTO, Produto.PRO_CODIGO, ");

                       
                    }
                    break;

                case "KMUltimoAbastecimento":
                    if (!select.Contains(" KMUltimoAbastecimento, "))
                    {
                        select.Append("(SELECT TOP(1) Abastecimento.ABA_KM FROM T_ABASTECIMENTO Abastecimento ");
                        select.Append("WHERE Abastecimento.VEI_CODIGO = OrdemServico.VEI_CODIGO ");
                        select.Append("order by Abastecimento.ABA_DATA desc) KMUltimoAbastecimento, ");

                        if (!groupBy.Contains("OrdemServico.VEI_CODIGO"))
                            groupBy.Append("OrdemServico.VEI_CODIGO, ");
                    }
                    break;

                case "HorimetroExecucao":
                    if (!select.Contains(" HorimetroExecucao, "))
                    {
                        select.Append("OrdemServico.OSE_HORIMETRO HorimetroExecucao, ");
                        groupBy.Append("OrdemServico.OSE_HORIMETRO, ");
                    }
                    break;

                case "KMExecucao":
                    if (!select.Contains(" KMExecucao, "))
                    {
                        select.Append("OrdemServico.OSE_QUILOMETRAGEM_VEICULO KMExecucao, ");
                        groupBy.Append("OrdemServico.OSE_QUILOMETRAGEM_VEICULO, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("OrdemServico.OSE_OBSERVACAO Observacao, ");
                        groupBy.Append("OrdemServico.OSE_OBSERVACAO, ");
                    }
                    break;

                case "Equipamento":
                    if (!select.Contains(" Equipamento, "))
                    {
                        select.Append("Equipamento.EQP_DESCRICAO Equipamento, ");
                        groupBy.Append("Equipamento.EQP_DESCRICAO, ");

                        SetarJoinsEquipamento(joins);
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("OrdemServico.OSE_SITUACAO Situacao, ");
                        groupBy.Append("OrdemServico.OSE_SITUACAO, ");
                    }
                    break;

                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("ISNULL(Tipo.FOT_DESCRICAO, 'NÃO INFORMADO') as Tipo, ");
                        groupBy.Append(" Tipo.FOT_DESCRICAO, ");

                        SetarJoinsTipo(joins);
                    }
                    break;

                case "ValorTotalFechamento":
                    if (!select.Contains(" ValorTotalFechamento, "))
                    {
                        select.Append("(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO),0) ");
                        select.Append("FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F ");
                        select.Append("JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO ");
                        select.Append("where OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorTotalFechamento, ");

                        if (!groupBy.Contains("OrdemServico.OSE_CODIGO"))
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");
                    }
                    break;

                case "ValorServicosFechamento":
                    if (!select.Contains(" ValorServicosFechamento, "))
                    {
                        select.Append("(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) ");
                        select.Append("FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F ");
                        select.Append("JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO ");
                        select.Append("where PRO_CATEGORIA_PRODUTO = 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorServicosFechamento, ");

                        if (!groupBy.Contains("OrdemServico.OSE_CODIGO"))
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");

                    }
                    break;

                case "ValorProdutosFechamento":
                    if (!select.Contains(" ValorProdutosFechamento, "))
                    {
                        select.Append("(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) ");
                        select.Append("FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F ");
                        select.Append("JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO ");
                        select.Append("where PRO_CATEGORIA_PRODUTO <> 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorProdutosFechamento, ");

                        if (!groupBy.Contains("OrdemServico.OSE_CODIGO"))
                            groupBy.Append("OrdemServico.OSE_CODIGO, ");

                    }
                    break;

                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        select.Append("Produto.PRO_DESCRICAO Produto, ");
                        groupBy.Append("Produto.PRO_DESCRICAO, ");

                        SetarJoinsProduto(joins);
                    }
                    break;

                case "Servico":
                    if (!select.Contains(" Servico, "))
                    {
                        select.Append(" Servico.SEV_DESCRICAO Servico, ");
                        groupBy.Append("Servico.SEV_DESCRICAO, ");

                        SetarJoinsServico(joins);
                    }
                    break;

                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_ORCADO) ValorTotal, ");

                        SetarJoinsOrdemServicoOrcamento(joins);
                    }
                    break;

                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_MAO_OBRA) ValorServicos, ");

                        SetarJoinsOrdemServicoOrcamento(joins);
                    }
                    break;

                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        select.Append("SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_PRODUTOS) ValorProdutos, ");

                        SetarJoinsOrdemServicoOrcamento(joins);
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("OrdemServico.OSE_NUMERO Numero, ");
                        groupBy.Append("OrdemServico.OSE_NUMERO, ");
                    }
                    break;

                case "MesAnoOS":
                    if (!select.Contains(" MesAnoOS, "))
                    {
                        select.Append("REPLICATE('0', 2 - LEN(CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)))) ");
                        select.Append("+ CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)) ");
                        select.Append("+ '-' ");
                        select.Append("+ CAST(YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(4)) MesAnoOS, ");
                        groupBy.Append("MONTH(OrdemServico.OSE_DATA_PROGRAMADA), YEAR(OrdemServico.OSE_DATA_PROGRAMADA), ");
                    }
                    break;

                case "AnoOS":
                    if (!select.Contains(" AnoOS, "))
                    {
                        select.Append("YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AnoOS, ");
                        groupBy.Append("YEAR(OrdemServico.OSE_DATA_PROGRAMADA), ");
                    }
                    break;

                case "MesOS":
                    if (!select.Contains(" MesOS, "))
                    {
                        select.Append("MONTH(OrdemServico.OSE_DATA_PROGRAMADA) MesOS, ");
                        groupBy.Append("OrdemServico.OSE_DATA_PROGRAMADA, ");

                    }
                    break;

                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("OrdemServico.OSE_DATA_PROGRAMADA Data, ");
                        groupBy.Append("OrdemServico.OSE_DATA_PROGRAMADA, ");
                    }
                    break;

                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao, "))
                    {
                        select.Append(" LocalManutencao.CLI_NOME LocalManutencao, ");
                        groupBy.Append("LocalManutencao.CLI_NOME, ");

                        SetarJoinsLocalManutencao(joins);
                    }
                    break;

                case "Ano":
                    if (!select.Contains(" Ano, "))
                    {
                        select.Append("Veiculo.VEI_ANO Ano, ");
                        groupBy.Append("Veiculo.VEI_ANO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("Marca.VMA_DESCRICAO Marca, ");
                        groupBy.Append("Marca.VMA_DESCRICAO, ");

                        SetarJoinsMarca(joins);
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("Modelo.VMO_DESCRICAO Modelo, ");
                        groupBy.Append("Modelo.VMO_DESCRICAO, ");

                        SetarJoinsModelo(joins);
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Mecanicos":
                    SetarJoinsTempoExecucao(joins);

                    if (!select.Contains(" Mecanicos, "))
                    {
                        select.Append("Mecanico.FUN_NOME Mecanicos, ");
                        groupBy.Append("Mecanico.FUN_NOME, ");

                        SetarJoinsMecanico(joins);
                    }
                    break;

                case "TempoExecutado":
                    if (!select.Contains(" TempoExecutado, "))
                    {
                        select.Append("TempoExecucao.OTE_TEMPO_EXECUTADO TempoExecutado, ");
                        groupBy.Append("TempoExecucao.OTE_TEMPO_EXECUTADO, ");

                        SetarJoinsTempoExecucao(joins);
                    }
                    break;

                case "TempoPrevisto":
                    if (!select.Contains(" TempoPrevisto, "))
                    {
                        select.Append("ServicoVeiculo.OSS_TEMPO_ESTIMADO TempoPrevisto, ");
                        groupBy.Append("ServicoVeiculo.OSS_TEMPO_ESTIMADO, ");

                        SetarJoinsServicoVeiculo(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioOrdemServicoPorMecanico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.TipoOrdemServico != null & filtrosPesquisa.TipoOrdemServico.Count > 0)
                where.Append($"AND OrdemServico.FOT_CODIGO in ({string.Join(", ", filtrosPesquisa.TipoOrdemServico)}) ");

            if (filtrosPesquisa.Empresa > 0)
                where.Append($"AND OrdemServico.EMP_CODIGO = '" + filtrosPesquisa.Empresa.ToString() + "' " );

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($"AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' " );

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($"AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "' " );

            if (filtrosPesquisa.NumeroOS > 0)
                where.Append($"AND OrdemServico.OSE_NUMERO = {filtrosPesquisa.NumeroOS}" );

            if (filtrosPesquisa.Veiculo != null && filtrosPesquisa.Veiculo.Count > 0)
                where.Append($" AND OrdemServico.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.Veiculo)}) ");

            if (filtrosPesquisa.ModeloVeiculo != null & filtrosPesquisa.ModeloVeiculo.Count > 0)
            {
                SetarJoinsVeiculo(joins);

                where.Append($" AND Veiculo.VMO_CODIGO in ({string.Join(", ", filtrosPesquisa.ModeloVeiculo)}) ");
            }

            if (filtrosPesquisa.LocalManutencao != null && filtrosPesquisa.LocalManutencao.Count > 0)
                where.Append($" AND OrdemServico.CLI_CGCCPF IN  ({string.Join(", ", filtrosPesquisa.LocalManutencao)}) ");

            if (filtrosPesquisa.Situacoes?.Count > 0)
                where.Append($" AND OrdemServico.OSE_SITUACAO IN ({ string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ToString("D"))) }) ");

            if (filtrosPesquisa.MarcaVeiculo != null && filtrosPesquisa.MarcaVeiculo.Count > 0)
            {
                SetarJoinsVeiculo(joins);

                where.Append($" AND Veiculo.VMA_CODIGO in ({string.Join(", ", filtrosPesquisa.MarcaVeiculo)}) ");
            }

            if (filtrosPesquisa.Servico != null & filtrosPesquisa.Servico.Count > 0)
            {
                SetarJoinsServico(joins);

                where.Append($" AND Servico.SEV_CODIGO IN ({string.Join(", ", filtrosPesquisa.Servico)}) ");
            }

            if (filtrosPesquisa.Equipamento != null & filtrosPesquisa.Equipamento.Count > 0)
            {
                where.Append(@" AND OrdemServico.EQP_CODIGO IN (" + string.Join(", ", filtrosPesquisa.Equipamento) + ") ");
            }

            if (filtrosPesquisa.Produto != null & filtrosPesquisa.Produto.Count > 0)
            {
                where.Append(@" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO 
                                                            WHERE PRO_CODIGO IN (" + string.Join(", ", filtrosPesquisa.Produto) + ")) ");
            }

            if (filtrosPesquisa.Responsavel > 0)
            {
                where.Append($" AND OrdemServico.FUN_RESPONSAVEL = {filtrosPesquisa.Responsavel} ");
            }

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
            {
                where.Append($" AND GrupoProduto.GPR_CODIGO = {filtrosPesquisa.CodigoGrupoProduto} ");

                SetarJoinsOrdemServicoFechamento(joins);

                SetarJoinsProduto(joins);

                SetarJoinsGrupoProduto(joins);
            }

            if (filtrosPesquisa.CentroResultado?.Count > 0)
                where.Append($" AND OrdemServico.CRE_CODIGO IN ({string.Join(", ", filtrosPesquisa.CentroResultado)}) ");

            if (filtrosPesquisa.Mecanicos?.Count > 0)
            {
                where.Append($" AND TempoExecucao.FUN_MECANICO IN ({string.Join(", ", filtrosPesquisa.Mecanicos)}) "); // SQL-INJECTION-SAFE

                SetarJoinsTempoExecucao(joins);
            }

        }

        #endregion
    }
}
