using System;

namespace ReportApi.ReportService
{
    public class RelatorioFactory
    {
        #region Atributos

        protected readonly Repositorio.UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly ReportServiceBase _reportServiceBase;

        #endregion

        #region Construtores 

        public RelatorioFactory(ReportServiceBase reportServiceBase, Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = ConnectionFactory.TipoServicoMultisoftware;
            _clienteMultisoftware = ConnectionFactory.Cliente;
            _reportServiceBase = reportServiceBase;
        }

        #endregion

        #region Métodos Públicos

        public void GerarRelatorio(int codigoRelatorioControleGeracao)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(codigoRelatorioControleGeracao);

            GerarRelatorioPorTipoRelatorio(relatorioControleGeracao);
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioPorTipoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio parametrosGeracao = null;
            string errorMessage = string.Empty;

            try
            {
                switch (relatorioControleGeracao.Relatorio.CodigoControleRelatorios)
                {
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R000_Nenhum:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R001_Filiais:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R002_CTESEmitidosPorEmbarcador:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CTeEmitido(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R003_FretePorDestinatario:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R004_TabelaFreteValor:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R005_CTes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R006_FechamentoAcertoViagem:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R007_FaturamentoCTesPorGrupoPessoas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.FaturamentoPorGrupoPessoas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R008_Minutas:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R009_ResultadoAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.ResultadoAcertoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R010_Fatura:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R011_ContratoDeFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R012_Cargas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.Carga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R280_GestaoCarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.GestaoCarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R013_PlanoConta:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R014_ExtratoConta:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoConta(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R015_TempoCarregamento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R016_CargaComponenteFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R017_DANFE:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R018_NotasEmitidas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.NotasEmitidas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R019_EstoqueProdutos:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.EstoqueProdutos(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R020_HistoricoEstoque:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.HistoricoEstoque(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R021_CCeNFe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R022_NotasEmitidasAdministrativo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R023_DirecionamentoOperador:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.DirecionamentoOperador(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R024_QuantidadeCarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.Quantidade(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R025_ComissaoGrupoProduto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R026_ComissaoProduto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R027_TaxaOcupacaoVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.TaxaOcupacaoVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R028_TaxaIncidenciaFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R029_ComissaoMotoristas:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R030_Titulo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.Titulo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R031_Canhotos:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Canhotos.Canhoto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R032_ClassificacaoVeiculo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R033_ValorDescarga:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R034_SimulacaoFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R035_DivergenciaPreFatura:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R036_Veiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Veiculos.Veiculos(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R037_Ocorrencias:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Ocorrencias.Ocorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R038_DevolucaoPallets:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R039_PedidoVenda:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R040_ExtratoMotorista:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoMotorista(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R041_BalanceteGerencial:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.BalanceteGerencial(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R042_PosicaoContasReceber:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R043_DescontoAcrescimoCTe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R044_TipoMovimento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R045_AliquotaICMSCTe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.AliquotaICMSCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R046_DescontoAcrescimoFatura:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.DescontoAcrescimoFatura(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R047_Faturamento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R048_Pallets_Estoque_Filial:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R049_Pallets_Estoque_Transportador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R050_Movimento_Motorista:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R051_Avaria_Analitico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Avarias.Analitico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R052_Manutencao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Veiculos.Manutencao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R053_DespesaAcertoViagem:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R054_TabelaFreteRota:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R055_FreteTerceirizado:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizado(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R056_PosicaoCTe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R057_Abastecimento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.Abastecimento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R058_CTesAverbados:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CTesAverbados(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R059_FechamentoPalletTransportador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R060_AcertoDeViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.AcertoDeViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R061_OrdemServicoVenda:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R062_Motorista:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.Motorista(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R063_Tomador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R064_ExpedicaoProdutos:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R065_RetornoBoleto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.RetornoBoleto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R066_Transportador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R067_PedidoOrdemVenda:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R068_Apolices:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R069_CTeTituloReceber:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.CTeTituloReceber(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R070_DocumentoFaturamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.DocumentoFaturamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R071_PosicaoDocumentoReceber:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R072_EmpresasFaturamento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R073_Chamado:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Atendimento.Chamado(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R074_GiroEstoque:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.GiroEstoque(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R075_FaturamentoMensal:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R076_FaturamentoMensalGrafico:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R077_CurvaABCProduto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R078_CurvaABCPessoa:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R079_HistoricoProduto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R080_CompraVendaNCM:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R081_ProdutoSemMovimentacao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.ProdutoSemMovimentacao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R082_CobrancaMensal:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R083_VendasReduzidas:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R084_NotasDetalhadas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.NotasDetalhadas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R085_VencimentoCertificado:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R086_PedidoNota:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R087_PerfilCliente:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R088_PreDACTE:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R089_NFes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFes.NFes(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R090_ResultadoAnualAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.ResultadoAnualAcertoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R091_Pedidos:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.Pedido(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R092_Encaixe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R093_TituloAcrescimoDesconto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.TituloAcrescimoDesconto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R094_ContatoCliente:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R095_DRE:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R097_CargaCTeIntegracao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CargaCTeIntegracao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R098_RelacaoEntrega:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R099_TemposGestaoPatio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.GestaoPatio.TemposGestaoPatio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R100_RelacaoSeparacaoVolume:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R101_ConferenciaVolume:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.WMS.ConferenciaVolume(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R102_ExpedicaoVolume:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.WMS.ExpedicaoVolume(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R103_EtiquetaVolume:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R104_SaldoArmazenamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.WMS.SaldoArmazenamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R105_ContratoFreteTransportador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R106_TransportadoresSemContrato:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R107_Recibo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R108_ReciboPagamentoAgregado:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R109_Pedagio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.Pedagio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R110_RelacaoEntrega:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R111_Usuarios:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Usuarios.Usuario(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R112_PerfilAcesso:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.PerfilAcesso(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R113_PedidoVendaContrato:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R114_Prospeccoes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CRM.Prospeccao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R115_CargaCompartilhada:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.CargaCompartilhada(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R116_CotaoCompraFornecedor:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R117_OrdemCompra:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Compras.OrdemCompra(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R118_NotaEntradaOrdemCompra:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Compras.NotaEntradaOrdemCompra(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R119_ChamadosOcorrencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Chamado.ChamadoOcorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R120_PontuacaoComprador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R121_PontuacaoFornecedor:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R122_SugestaoCompra:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R123_FluxoHorario:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R124_Francesinha:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R125_DocumentoEmissaoNFSManual:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaDocumentoEmissaoNFSManual(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R126_TipoOcorrencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Ocorrencias.TipoOcorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R127_DREGerencial:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R128_PlanoOrcamentario:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R129_GuaritaTMS:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R130_FluxoCaixa:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R131_TituloSemMovimento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R132_RetornoPagamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.RetornoPagamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R133_LogEnvioEmail:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Administrativo.LogEnvioEmail(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R134_FaturaCIOT:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R135_CargaCIOT:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R136_Pessoa:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.Pessoa(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R137_ComponenteFreteCTe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R138_ControleReformaPallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R139_ControleTransferenciaPallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R140_ControleValePallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R141_ControleEntradaSaidaPallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R142_ControleAvariaPallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R143_EstoqueCompraPallet:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R144_PagamentoAgregado:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R145_Infracao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.Multa(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R146_CargaProduto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaProduto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R147_FilaCarregamentoHistorico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.FilaCarregamentoHistorico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R148_DiariaAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.DiariaAcertoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R149_FreteContabil:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R150_Licenca:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Administrativo.Licenca(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R151_DespesaVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.DespesaVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R152_DespesaOrdemServico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.DespesaOrdemServico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R153_RequisicaoMercadoria:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Compras.RequisicaoMercadoria(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R154_RelRequisicaoMercadoria:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Compras.RequisicaoMercadoria(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R155_FreteContabil:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Escrituracao.FreteContabil(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R156_PneuHistorico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.PneuHistorico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R157_MovimentacaoPneuVeiculo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R158_AbastecimentoNotaEntrada:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R159_Cheque:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.Cheque(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R160_Competencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Escrituracao.Competencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R161_Mdfe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.MDFes.MDFes(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R162_ReceitaDespesaVeiculo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R163_ManutencaoVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.ManutencaoVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R164_Pallets_Estoque_Cliente:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R165_TermoRecolhimentoMaterial:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R166_TermoBaixaMaterial:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R167_TermoResponsabilidade:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R168_Bem:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R169_MapaDepreciacao:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R170_SaldoProvisao:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R171_BaixaTitulo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R172_CaixaFuncionario:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R173_Notas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.NFe.Notas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R174_FuncionarioComissao:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R175_VolumesFaltantes:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R176_ArquivoRetornoBoleto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R177_MonitoramentoVeiculoAlvo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R178_MonitoramentoPosicaoDaFrota:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R179_MonitoramentoNivelServico:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R180_DespesaMensal:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R181_CotacaoPedido:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R182_GuaritaCheckList:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R183_ComissaoAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.ComissaoAcertoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R184_ControleVisita:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.GestaoPatio.ControleVisita(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R185_RotaFrete:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.RotaFrete(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R186_ColaboradorSituacaoLancamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.ColaboradorSituacaoLancamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R187_VendaDireta:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R188_MonitoramentoTratativaAlerta:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R189_NaturezaDaOperacao:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R190_CFOP:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.CFOP(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R191_PedidoProduto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.PedidoProduto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R324_ChamadoDevolucao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Chamado.ChamadoDevolucao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R192_RegrasAutorizacaoOcorrencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Ocorrencias.RegrasAutorizacaoOcorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R193_TipoContatoCliente:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Contatos.TipoContatoCliente(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R194_MotoristaExtratoSaldo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.MotoristaExtratoSaldo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R195_ConfiguracaoTabelaFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R196_AceiteContrato:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R197_ResponsavelVeiculo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R198_OcorrenciaCentroCusto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R199_FaturaPorCte:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R200_FaturaMultimodal:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R201_AFRMM:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.AFRMMControl(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R202_OrdemServico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.OrdemServico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R203_CotacaoCompra:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R204_Container:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.Container(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R205_LogEnvioSMS:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R206_UltimoAcertoMotorista:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.UltimoAcertoMotorista(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R207_DocumentosCarga:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R208_ExtratoBancario:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R209_AutorizacaoPagamentoTitulo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R210_AlteracaoFrete:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R211_Equipamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Veiculos.Equipamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R212_Produto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Produtos.Produto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R213_ApuracaoICMS:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R214_MonitoramentoHistoricoTemperatura:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoHistoricoTemperatura(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R215_Subcontratacao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.Subcontratacao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R216_IndicadorIntegracaoCTe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R217_FaturamentoPorCTe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.FaturamentoPorCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R218_NFeCTeContainer:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.NFeCTeContainer(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R219_ServicoVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.ServicoVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R220_FaturaPorDocumentos:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R221_TempoDeViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.TempoDeViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R222_JornadaMotorista:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.JornadaMotorista(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R223_PreCarga:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R224_Armazenagem:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R225_PagamentoMotoristaTMS:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.PagamentosMotoristas.PagamentosMotoristas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R226_CheckList:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R337_ComissaoFuncionario:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.RH.ComissaoFuncionario(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R227_HistoricoParadas:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R228_ReciboCompleto:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R229_DespesaDetalhadaOrdemServico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.DespesaDetalhadaOrdemServico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R230_ProdutoEmbarcador:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R231_Localidade:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Localidades.Localidade(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R232_Paradas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.Paradas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R233_AFRMMControlMercante:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.AFRMMControlMercante(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R234_EtiquetaVolumeNFe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R235_AvaliacaoEntregaPedido:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R236_DadosDocsys:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Documentos.DadosDocsys(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R237_JanelaAgendamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.JanelaAgendamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R238_MonitoramentoTempoVeiculo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R239_MonitoramentoPosicaoFrotaRastreamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoPosicaoFrotaRastreamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R240_JanelaDisponivelAgendamento:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R241_AgendaCancelada:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R242_CargaIntegracao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaIntegracao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R243_FaturaParcelasSeparadas:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R244_ExtratoAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoAcertoViagem(true, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R245_EtiquetaDeposito:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R246_RastreabilidadeVolumes:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R247_MonitoramentoAlerta:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoAlerta(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R248_CargaPedidoEmbarcador:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaPedidoEmbarcador(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R249_RelatorioValePedagio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.ValePedagio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R250_CargaEntregaPedido:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaEntregaPedido(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R252_PosicaoContasPagar:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.PosicaoContasPagar(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R253_ConsolidacaoGas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.ConsolidacaoGas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R254_QuantidadeCargaDescarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.QuantidadeDescarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R255_MDFesAverbados:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.MDFes.MDFesAverbados(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R256_RotaControleEntrega:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.ControleEntrega.RotaControleEntrega(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R257_AuditoriaUsuario:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Auditoria.AuditoriaUsuario(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R258_BonificacaoAcertoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.AcertoViagem.BonificacaoAcertoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R259_MovimentoFinanceiro:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.MovimentoFinanceiro(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R260_SerieDocumentos:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Documentos.SerieDocumentos(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R261_ClienteDescarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.PessoaDescarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R262_RateioDespesaVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.RateioDespesaVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R263_TakeOrPay:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.TakeOrPay(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R264_ControleTempoViagem:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.ControleTempoViagem(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R265_AgendamentoEntregaPedido:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.AgendamentoEntregaPedido(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R266_MonitoramentoControleEntrega:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoControleEntrega(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R267_FreteTerceirizadoAcrescimoDesconto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoAcrescimoDesconto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R268_CTesSubcontratados:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CTeSubcontratado(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R269_RetornoAbastecimentoAngellira:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.RetornoAbastecimentoAngellira(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R270_ConciliacaoBancaria:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R271_TorreControleConsultaPorNotaFiscal:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.TorreControle.ConsultaPorNotaFiscal(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R272_EtiquetaControleTacografo:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R273_FolhaLancamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.RH.FolhaLancamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R274_AuditoriaCTe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.AuditoriaCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R275_MultaParcela:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.MultaParcela(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R276_ConferenciaFiscal:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.ConferenciaFiscal(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R278_PneuCustoEstoque:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.PneuCustoEstoque(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R279_CargaCIOTPedido:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaCIOTPedido(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R282_PedidoOcorrencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pedido.PedidoOcorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R283_ControleEntrega:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.ControleEntrega.ControleEntrega(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R284_ComissaoVendedorCTe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.ComissaoVendedorCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R285_FreteTerceirizadoValePedagio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoValePedagio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R286_PedidoDevolucao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pedido.PedidoDevolucao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R287_PracaPedagio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.PracaPedagio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R288_GrupoPessoas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.GrupoPessoas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R289_CargaEntregaChecklist:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.ControleEntrega.CargaEntregaChecklist(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R290_Pneu:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.Pneu(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R291_HistoricoMovimentacaoCanhoto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Canhotos.HistoricoMovimentacaoCanhoto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R292_Booking:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pedido.PedidoDadosTransporteMaritimo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R293_OrdemServicoPorMecanico:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.OrdemServicoPorMecanico(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R294_ContratoFreteAcrescimoDesconto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.ContratoFreteAcrescimoDesconto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R295_RegraICMS:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.ICMS.RegraICMS(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R296_ContratoFinanceiro:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.ContratoFinanceiro(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R297_PlanejamentoFrotaDia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.PlanejamentoFrotaDia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R298_ConfiguracaoCentroResultado:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.ConfiguracaoContabil.ConfiguracaoCentroResultado(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R299_PneuPorVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.PneuPorVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R300_ConfiguracaoOperadores:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Operacional.ConfiguracaoOperadores(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R301_CargaIntegracaoDadosTransportes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaIntegracaoDadosTransportes(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R302_LicencaVeiculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Administrativo.LicencaVeiculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R303_DespesaOrdemServicoProduto:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frotas.DespesaOrdemServicoProduto(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R304_IntegracaoLotePagamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Escrituracao.IntegracaoLotePagamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R305_Sinistro:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Frota.Sinistro(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R306_TaxasDescarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pallets.TaxasDescarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R307_AbastecimentoTicketLog:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pallets.TaxasDescarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R308_CargaViagemEventos:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaViagemEventos(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R310_ProvisaoVolumetria:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.ProvisaoVolumetria(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R311_Pacotes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.Pacotes(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R312_PedidoRetornoOcorrencia:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pedido.PedidoRetornoOcorrencia(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R313_JanelaCarregamentoIntegracao:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.JanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R314_EnderecoSecundario:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Pessoas.EnderecoSecundario(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R315_ControleContainer:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Containers.ControleContainer(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R316_ConfiguracoesNFSe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Transportadores.ConfiguracoesNFSe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R317_ConsolidadoEntregas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.TorreControle.ConsolidadoEntregas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R319_CargaProdutoTransportador:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargaProdutoTransportador(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R321_FreteTerceirizadoPorCTe:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoPorCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R322_CondicoesPagamentoTransportador:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.CondicoesPagamentoTransportador(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R323_AgendaTarefas:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CRM.AgendaTarefas(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R330_Navio:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.Navio(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R334_LogAcesso:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Administrativo.LogAcesso(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R335_ExtracaoMassivaTabelaFrete:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Fretes.ExtracaoMassivaTabelaFrete(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R338_LiberacaoPagamentoProvedor:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Financeiros.LiberacaoPagamentoProvedor(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R339_FaturaChaveCTe:
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R336_PreCTes:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.PreCTes.PreCTe(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R343_MonitoramentoNovo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.MonitoramentoNovo.MonitoramentoNovo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R345_HistoricoVinculo:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.HistoricoVinculo(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R349_CargasComInteresseTransportadorTerceiro:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Carga.CargasComInteresseTransportadorTerceiro(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R350_HistoricoJanelaCarregamento:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Logistica.HistoricoJanelaCarregamento(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R351_CustoRentabilidadeCteCrt:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.CTes.CustoRentabilidadeCteCrt(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R352_OcorrenciasEntrega:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Ocorrencias.OcorrenciaEntrega(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R353_Permanencias:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.TorreControle.Permanencias(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R354_ModeloVeicularCarga:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.Cargas.ModeloVeicularCarga(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    case Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R355_DevolucaoNotasFiscais:
                        parametrosGeracao = new Servicos.Embarcador.Relatorios.TorreControle.DevolucaoNotasFiscais(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware).GerarRelatorioPorServico(relatorioControleGeracao);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao gerar relatório por tipo : " + ex);
                errorMessage = ex.Message;
            }

            _reportServiceBase.FinalizarGeracaoReport(relatorioControleGeracao, parametrosGeracao, errorMessage);
        }

        #endregion
    }
}
