using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ReportApi.Reports;

[UseReportType(ReportType.AcertoFechamento)]
public class AcertoFechamentoReport : ReportBase
{
    public AcertoFechamentoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoAcerto = extraData.GetValue<int>("CodigoAcerto");
        if (codigoAcerto == 0)
            codigoAcerto = extraData.GetValue<int>("Codigo");
        var relatorio = extraData.GetValue<string>("Relatorio");
        var info = extraData.GetInfo();
        var usuario = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario"));

        Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagemo = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagemo.BuscarPorCodigo(codigoAcerto);
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);

        var mdlRelatorio = new ReportApi.Models.Grid.Relatorio();

        Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(_unitOfWork);

        Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(relatorio);
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, _unitOfWork);

        _unitOfWork.CommitChanges();

        ReportApi.Models.Grid.Relatorio gridRelatorio = new ReportApi.Models.Grid.Relatorio();
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, info.TipoServico);

        gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

        ReportApi.Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportApi.Models.Grid.Grid>(dynRelatorio.Grid);

        string propOrdena = relatorioTemp.PropriedadeOrdena;

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

        relatorioTemp.PropriedadeOrdena = propOrdena;

        string stringConexao = _unitOfWork.StringConexao;
        bool acertoDeViagemImpressaoDetalhada = ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada;

        Task.Factory.StartNew(() => GerarRelatorioAcertoViagem(acertoViagem, agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, acertoDeViagemImpressaoDetalhada));

        return PrepareReportResult(FileType.PDF);

    }
    private void GerarRelatorioAcertoViagem(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, bool acertoDeViagemImpressaoDetalhada)
    {
        System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

        Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
        Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();
        Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
        Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

        try
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.RelatorioAcertoViagem> listaRelatorioAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagem(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargasAcertoViagem> listaCargaAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagemCargas(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            if (listaCargaAcertoViagem.Count == 0)
            {
                listaCargaAcertoViagem.Add(new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargasAcertoViagem()
                {
                    BonificacaoCliente = 0,
                    CNPJEmitente = "",
                    Codigo = acertoViagem.Codigo,
                    CodigoAcerto = acertoViagem.Codigo,
                    CodigoCarga = acertoViagem.Codigo,
                    Data = DateTime.Now,
                    Emitente = "",
                    NumeroCarga = "",
                    PedagioCarga = 0,
                    PercentualCarga = 0,
                    Peso = 0,
                    Placa = "",
                    ValorBrutoCarga = 0,
                    ValorComponenteFrete = 0,
                    ValorFrete = 0,
                    ValorICMS = 0,
                    ValorICMSCarga = 0,
                    ValorPedagioCredito = 0
                });
            }
            for (int i = 0; i < listaCargaAcertoViagem.Count(); i++)
                listaCargaAcertoViagem[i].Emitente = serCargaDadosSumarizados.ObterOrigemDestinos(repCarga.BuscarPorCodigo(listaCargaAcertoViagem[i].CodigoCarga), true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);

            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AbastecimentosAcertoViagem> listaAbastecimentosAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagemAbastecimentos(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;            
            for (int i = 0; i < listaAbastecimentosAcertoViagem.Count(); i++)
            {
                Enum.TryParse(Convert.ToString(listaAbastecimentosAcertoViagem[i].TipoAbastecimento), out tipoAbastecimento);
                listaAbastecimentosAcertoViagem[i].KmInicial = repAcertoResumoAbastecimento.BuscarKMInicialPorCodigoAcertoVeiculoTipo(listaAbastecimentosAcertoViagem[i].CodigoAcerto, listaAbastecimentosAcertoViagem[i].CodigoVeiculo, tipoAbastecimento);
                listaAbastecimentosAcertoViagem[i].MediaKM = repAcertoResumoAbastecimento.BuscarMediaDoAcertoPorVeiculo(acertoViagem.Codigo, listaAbastecimentosAcertoViagem[i].CodigoVeiculo);
            }
            if (listaAbastecimentosAcertoViagem.Count() == 0)
            {
                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AbastecimentosAcertoViagem abastecimento = new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AbastecimentosAcertoViagem();
                abastecimento.Cidade = "";
                abastecimento.CNPJPosto = 0;
                abastecimento.Codigo = 0;
                abastecimento.CodigoAcerto = acertoViagem.Codigo;
                abastecimento.CodigoProduto = 0;
                abastecimento.CodigoVeiculo = 0;
                abastecimento.Data = DateTime.Now;
                abastecimento.Documento = "";
                abastecimento.Estado = "";
                abastecimento.Kilometragem = 0;
                abastecimento.KMAnterior = 0;
                abastecimento.KmInicial = 0;
                abastecimento.KmTotal = 0;
                abastecimento.KmTotalAjustado = 0;
                abastecimento.Litros = 0;
                abastecimento.NomePosto = "";
                abastecimento.PercentualAjusteKM = 0;
                abastecimento.Placa = "";
                abastecimento.Produto = "";
                abastecimento.ValorDigitado = 0;
                abastecimento.ValorTotal = 0;
                abastecimento.ValorUnitario = 0;
                abastecimento.MediaKM = 0;

                listaAbastecimentosAcertoViagem.Add(abastecimento);
            }

            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.PedagiosAcertoViagem> listaPedagiosAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagemPedagios(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DespesasAcertoViagem> listaDespesasAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagemDespesas(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.OcorrenciasAcertoViagem> listaOcorrenciasAcertoViagem = repAcertoViagem.BuscarRelatorioConsultaAcertoViagemOcorrencias(acertoViagem, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
            List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.FichaMotoristaAcertoViagem> listaFichaMotoristaAcertoViagem = new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.FichaMotoristaAcertoViagem>();
            List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DocumentosAcertoViagem> listaDocumentosAcertoViagem = new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DocumentosAcertoViagem>();

            decimal kmRodadoTotal = 0;
            decimal kmRodadoCarregado = 0;
            decimal kmRodadoVazio = 0;
            decimal custoMedioPorLitro = 0;
            decimal qtdeCombustivel = 0;
            decimal kmsMedioPorLitro = 0;
            decimal qtdeCombustivelCjto = 0;
            decimal litrosMedioPorHora = 0;

            decimal vlrDespesasPagas = 0;
            decimal vlrOutrosCreditos = 0;
            decimal vlrFretesRecebidos = 0;
            decimal vlrAdiantamento = 0;
            decimal subTotalValeViagem = 0;
            decimal vlrComissao = 0;
            decimal saldoMotorista = 0;

            var listaOcorrencias = (from p in listaOcorrenciasAcertoViagem select DynOcorrenciasAcertoViagem(p, unitOfWork)).ToList();

            if (acertoDeViagemImpressaoDetalhada)
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoDesconto> descontos = repAcertoDesconto.BuscarPorAcerto(acertoViagem.Codigo);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> bonificacoes = repAcertoBonificacao.BuscarPorAcerto(acertoViagem.Codigo);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> adiantamentos = repAcertoAdiantamento.BuscarPorAcerto(acertoViagem.Codigo);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> cargas = repAcertoCarga.BuscarPorCodigoAcerto(acertoViagem.Codigo);
                foreach (var desc in descontos)
                {
                    listaFichaMotoristaAcertoViagem.Add(new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.FichaMotoristaAcertoViagem()
                    {
                        Codigo = desc.Codigo,
                        CodigoAcerto = acertoViagem.Codigo,
                        DataEmissao = desc.Data.ToString("dd/MM/yyyy"),
                        DebitoCredito = 0,
                        Descricao = "Desconto - " + desc.Justificativa.Descricao + " " + desc.Motivo,
                        Numero = desc.Codigo.ToString("0n"),
                        Valor = desc.ValorDesconto * -1
                    });
                }
                foreach (var adi in adiantamentos)
                {
                    listaFichaMotoristaAcertoViagem.Add(new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.FichaMotoristaAcertoViagem()
                    {
                        Codigo = adi.Codigo,
                        CodigoAcerto = acertoViagem.Codigo,
                        DataEmissao = adi.PagamentoMotoristaTMS.DataPagamento.ToString("dd/MM/yyyy"),
                        DebitoCredito = 0,
                        Descricao = "Adiantamento - " + adi.PagamentoMotoristaTMS.Observacao,
                        Numero = adi.PagamentoMotoristaTMS.Numero.ToString("0n"),
                        Valor = adi.PagamentoMotoristaTMS.TotalPagamento(configuracaoEmbarcador.NaoDescontarValorSaldoMotorista) * -1
                    });
                }
                foreach (var boni in bonificacoes)
                {
                    listaFichaMotoristaAcertoViagem.Add(new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.FichaMotoristaAcertoViagem()
                    {
                        Codigo = boni.Codigo,
                        CodigoAcerto = acertoViagem.Codigo,
                        DataEmissao = boni.Data.ToString("dd/MM/yyyy"),
                        DebitoCredito = 1,
                        Descricao = "Bonificação - " + boni.Justificativa.Descricao + " " + boni.Motivo,
                        Numero = boni.Codigo.ToString("0n"),
                        Valor = boni.ValorBonificacao
                    });
                }

                foreach (var carga in cargas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarConhecimentoPorCarga(carga.Carga.Codigo);
                    foreach (var cte in listaCargaCTe)
                    {
                        if (cte.CTe.Status == "A" && cte.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao)
                        {
                            double valorComissao = (double)cte.CTe.ValorFrete;
                            if (carga.PercentualAcerto < 100)
                                valorComissao = ((valorComissao * (double)(carga.PercentualAcerto)) / 100);
                            valorComissao = 0.1 * valorComissao;
                            vlrComissao += (decimal)valorComissao;
                            listaDocumentosAcertoViagem.Add(new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DocumentosAcertoViagem()
                            {
                                Chave = cte.CTe.Chave,
                                Cliente = cte.CTe.TomadorPagador?.Nome ?? string.Empty,
                                Codigo = cte.CTe.Codigo,
                                CodigoAcerto = acertoViagem.Codigo,
                                DataEmissao = cte.CTe.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                LocalColeta = cte.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                LocalEntrega = cte.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                Modelo = cte.CTe.ModeloDocumentoFiscal.Numero,
                                PercentualComissao = 10,
                                Numero = cte.CTe.Numero,
                                Serie = cte.CTe.Serie.Numero,
                                ValorBase = cte.CTe.ValorFrete,
                                ValorComissao = (decimal)valorComissao,
                                ValorRecebido = 0,
                                DataEmissaoSemFormato = cte.CTe.DataEmissao.Value
                            });
                        }
                    }
                }

                listaDocumentosAcertoViagem = listaDocumentosAcertoViagem.OrderBy(o => o.DataEmissaoSemFormato).ToList();

                if (listaOcorrencias != null && listaOcorrencias.Count > 0)
                {
                    foreach (var ocorrencia in listaOcorrencias)
                    {
                        vlrComissao += (decimal)ocorrencia.ValorComissao;
                    }
                }

                kmRodadoTotal = repAcertoResumoAbastecimento.BuscarKMTotalAcerto(acertoViagem.Codigo, "0");
                kmRodadoCarregado = 0;
                kmRodadoVazio = 0;
                custoMedioPorLitro = repAcertoAbastecimento.MediaValorUnitarioAbastecimentos(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, "0");
                qtdeCombustivel = repAcertoAbastecimento.QuantidadeLitrosAbastecimentos(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, "0");
                if (qtdeCombustivel > 0 && kmRodadoTotal > 0)
                    kmsMedioPorLitro = kmRodadoTotal / qtdeCombustivel;
                qtdeCombustivelCjto = repAcertoAbastecimento.QuantidadeLitrosAbastecimentos(acertoViagem.Codigo, "1");
                litrosMedioPorHora = 0;

                vlrDespesasPagas = repAcertoOutraDespesa.ReceitaDespesaOutraDespesa(acertoViagem.Codigo, false);
                vlrOutrosCreditos = repAcertoBonificacao.TotalBonificacaoPorAcerto(acertoViagem.Codigo);
                vlrFretesRecebidos = 0;
                vlrAdiantamento = repAcertoAdiantamento.BuscarValorTotalPorAcerto(acertoViagem.Codigo);
                vlrAdiantamento += descontos != null && descontos.Count > 0 ? descontos.Sum(o => o.ValorDesconto) : 0m;
                subTotalValeViagem = vlrDespesasPagas + vlrOutrosCreditos - vlrFretesRecebidos - vlrAdiantamento;
                saldoMotorista = subTotalValeViagem + vlrComissao;
            }

            foreach (var rel in listaRelatorioAcertoViagem)
            {
                if (listaFichaMotoristaAcertoViagem != null && listaFichaMotoristaAcertoViagem.Count > 0)
                    rel.FichaMotorista = 1;
                else
                    rel.FichaMotorista = 0;
                if (listaDocumentosAcertoViagem != null && listaDocumentosAcertoViagem.Count > 0)
                    rel.Documentos = 1;
                else
                    rel.Documentos = 0;

                if (listaOcorrencias != null && listaOcorrencias.Count > 0)
                    rel.Ocorrencias = 1;
                else
                    rel.Ocorrencias = 0;
            }
            List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> listaReceitaAcertoViagem = servAcertoViagem.RetornaObjetoReceitaViagem(acertoViagem.Codigo, unitOfWork, configuracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, configuracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, configuracaoEmbarcador.GerarTituloFolhaPagamento, configuracaoEmbarcador.GerarReciboAcertoViagemDetalhado, configuracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));
            CrystalDecisions.CrystalReports.Engine.ReportDocument report;

            bool contemPedagio = listaPedagiosAcertoViagem.Count > 0;

            if (listaAbastecimentosAcertoViagem.Count > 0 && listaPedagiosAcertoViagem.Count > 0)
                report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaRelatorioAcertoViagem, unitOfWork, null,
                    new List<KeyValuePair<string, dynamic>>() {
                            new KeyValuePair<string, dynamic>("AcertoViagem_Cargas.rpt", listaCargaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Abastecimentos.rpt",listaAbastecimentosAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Pedagios.rpt", listaPedagiosAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Despesas.rpt", listaDespesasAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Resumo.rpt", listaReceitaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Ocorrencias.rpt", listaOcorrencias),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Ficha.rpt", listaFichaMotoristaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Documento.rpt", listaDocumentosAcertoViagem)
                    });
            else if (listaAbastecimentosAcertoViagem.Count > 0)
                report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaRelatorioAcertoViagem, unitOfWork, null,
                    new List<KeyValuePair<string, dynamic>>() {
                                new KeyValuePair<string, dynamic>("AcertoViagem_Cargas.rpt", listaCargaAcertoViagem),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Abastecimentos.rpt",listaAbastecimentosAcertoViagem),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Despesas.rpt", listaDespesasAcertoViagem),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Resumo.rpt", listaReceitaAcertoViagem),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Ocorrencias.rpt", listaOcorrencias),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Ficha.rpt", listaFichaMotoristaAcertoViagem),
                                new KeyValuePair<string, dynamic>("AcertoViagem_Documento.rpt", listaDocumentosAcertoViagem)
                        });
            else
                report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaRelatorioAcertoViagem, unitOfWork, null,
                    new List<KeyValuePair<string, dynamic>>() {
                            new KeyValuePair<string, dynamic>("AcertoViagem_Cargas.rpt", listaCargaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Despesas.rpt", listaDespesasAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Resumo.rpt", listaReceitaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Ocorrencias.rpt", listaOcorrencias),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Ficha.rpt", listaFichaMotoristaAcertoViagem),
                            new KeyValuePair<string, dynamic>("AcertoViagem_Documento.rpt", listaDocumentosAcertoViagem)
                    });

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(acertoViagem.Motorista.Codigo);
            Dominio.Entidades.Usuario operador = repUsuario.BuscarPorCodigo(acertoViagem.Operador.Codigo);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista.Nome, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", acertoViagem.DescricaoPeriodo, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Etapa", acertoViagem.DescricaoEtapa, !acertoDeViagemImpressaoDetalhada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", acertoViagem.DescricaoSituacao, true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AprovadoPedagio", acertoViagem.DescricaoAprovacaoPedagio, !acertoDeViagemImpressaoDetalhada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AprovadoAbastecimento", acertoViagem.DescricaoAprovacaoAbastecimento, !acertoDeViagemImpressaoDetalhada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", operador.Nome, !acertoDeViagemImpressaoDetalhada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", acertoViagem.DataAcerto.ToString("dd/MM/yyyy"), !acertoDeViagemImpressaoDetalhada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CPFMotorista", motorista.CPF_Formatado, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroViagens", Convert.ToString(listaReceitaAcertoViagem[0].NumeroViagens), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("QtdViagensCompartilhada", Convert.ToString(listaReceitaAcertoViagem[0].NumeroViagensCompartilhada), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorViagensCompartilhada", listaReceitaAcertoViagem[0].ValorViagensCompartilhada.ToString("n2"), false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("KMRodadoTotal", kmRodadoTotal.ToString("n0"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("KMRodadoCarregado", kmRodadoCarregado.ToString("n0"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("KMRodadoVazio", kmRodadoVazio.ToString("n0"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CustoMedioporLitro", custoMedioPorLitro.ToString("n4"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("QtdeCombustivel", qtdeCombustivel.ToString("n2"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("KMsMedioPorLitro", kmsMedioPorLitro.ToString("n4"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("QtdeCombustivelCjto", qtdeCombustivelCjto.ToString("n2"), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LitrosMedioPorHora", litrosMedioPorHora.ToString("n4"), false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VlrDespesasPagas", vlrDespesasPagas.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VlrOutrosCreditos", vlrOutrosCreditos.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VlrFretesRecebidos", vlrFretesRecebidos.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VlrAdiantamento", vlrAdiantamento.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SubTotalValeViagem", subTotalValeViagem.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VlrComissao", vlrComissao.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SaldoMotorista", saldoMotorista.ToString("n2", cultura), false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContemPedagio", (contemPedagio ? "SIM" : "NÃO"), false));

            _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Acertos/AcertoViagem", unitOfWork);
        }
        catch (Exception ex)
        {
            serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
        }
        finally
        {
            unitOfWork.Dispose();
        }
    }

    private dynamic DynOcorrenciasAcertoViagem(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.OcorrenciasAcertoViagem p, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(p.Codigo);

        double valorComissao = (double)p.Valor;
        valorComissao = (0.1 * valorComissao);
        decimal vlrComissao = (decimal)valorComissao;

        return new
        {
            p.CodigoAcerto,
            p.Codigo,
            p.NumeroOcorrencia,
            DataOcorrencia = p.DataOcorrencia,
            CodigoCargaEmbarcador = p.CodigoCargaEmbarcador,
            Veiculo = ocorrencia.Carga?.PlacasVeiculos ?? string.Empty,
            TipoOcorrencia = ocorrencia.TipoOcorrencia?.Descricao ?? string.Empty,
            Motorista = ocorrencia.Carga?.NomeMotoristas ?? string.Empty,
            Componente = ocorrencia.ComponenteFrete?.Descricao ?? "Sem complemento",
            Valor = p.Valor,
            p.DescricaoSituacao,
            ValorComissao = vlrComissao
        };
    }
}