using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.PdfComprovanteColeta)]
public class PdfComprovanteColetaReport : ReportBase
{
    public PdfComprovanteColetaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

        var CodigoCarga = extraData.GetValue<int>("CodigoCargaEntrega");

        var cargaEntrega = repCargaEntrega.BuscarPorCodigo(CodigoCarga);

        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repositorioCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntrega.Carga;
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura = repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> fotosCargaEntrega = repCargaFoto.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal> listGta = repCargaEntregaGuiaTransporteAnimal.ConsultarCargasEntrega(cargaEntrega.Codigo, null);
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.QuantidadePorDivisao> listaQuantidadePorDivisao = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.QuantidadePorDivisao>();
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscals = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal> cargaEntregaFotoNotasFiscais = repCargaEntregaFotoNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto = repositorioCargaPedidoProduto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisoesCapacidade = repositorioCargaPedidoProdutoDivisaoCapacidade.BuscarPorCargasPedidoProduto(listaCargaPedidoProduto.Select(obj => obj.Codigo).ToList());

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosNotasFiscais> listaFotosNotasFiscais = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosNotasFiscais>();

        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListPerguntasEmbarque = ObterCheckListCargaEntrega(cargaEntrega, TipoCheckList.Coleta, _unitOfWork);
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaPerguntas> listaColetaPerguntasDS = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaPerguntas>();

        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListPerguntasDesembarque = ObterCheckListCargaEntrega(cargaEntrega, TipoCheckList.Desembarque, _unitOfWork);
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.DesembarquePerguntas> listaDesembarquePerguntasDS = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.DesembarquePerguntas>();

        bool possuiPerguntasColeta = false;
        bool possuiPerguntasDesembarque = false;

        if (checkListPerguntasEmbarque != null)
            possuiPerguntasColeta = checkListPerguntasEmbarque.Count > 0 ? true : false;
        if (checkListPerguntasDesembarque != null)
            possuiPerguntasDesembarque = checkListPerguntasDesembarque.Count > 0 ? true : false;

        bool possuiFotoNotasFiscais = cargaEntregaFotoNotasFiscais.Count > 0 ? true : false;
        bool possuiFotoEmbarque = fotosCargaEntrega.Count > 0 ? true : false;

        Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColeta dataSourceComprovanteColeta =
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColeta()
            {
                Cliente = cargaEntrega.Cliente.Descricao,
                Transportador = carga.Empresa.RazaoSocial,
                Filial = (!string.IsNullOrWhiteSpace(carga.Filial.CodigoFilialEmbarcador)
                    ? carga.Filial.CodigoFilialEmbarcador + " - "
                    : string.Empty) + carga.Filial.Descricao,
                Motorista = carga.NomeMotoristas,
                Veiculo = carga.PlacasVeiculos,
                DataInicioCarregamento = cargaEntrega.DataInicio?.ToDateTimeString() ?? string.Empty,
                DataFimCarregamento = cargaEntrega.DataFim?.ToDateTimeString() ?? string.Empty,
                DataConfirmacaoColeta = cargaEntrega.DataConfirmacao?.ToDateTimeString() ?? string.Empty,
                DataChegada = cargaEntrega.DataEntradaRaio?.ToDateTimeString() ?? string.Empty,
                QuantidadePlanejada = servicoControleEntrega
                    .ObterQuantidadePlanejada(cargaEntrega, cargaEntregaPedidos, listaCargaPedidoProdutoCarga)
                    ?.ToString("n2") ?? null,
                QuantidadeColetada = servicoControleEntrega
                    .ObterQuantidadeTotal(cargaEntrega, cargaEntregaPedidos, listaCargaPedidoProdutoCarga)
                    ?.ToString("n2") ?? null,
                NumeroCargaEmbarcador = carga.CodigoCargaEmbarcador,
                TipoCarga = carga?.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOpercao = carga?.TipoOperacao?.Descricao ?? string.Empty,
                Rota = carga?.DadosSumarizados?.RotaEmbarcador ?? string.Empty,
                Origem = carga?.DadosSumarizados?.Origens ?? string.Empty,
                Destino = carga?.DadosSumarizados?.Destinos ?? string.Empty,
                Remetente = carga?.DadosSumarizados?.Remetentes ?? string.Empty,
                Destinatario = carga?.DadosSumarizados?.Destinatarios ?? string.Empty,
                ModeloVeicular = carga?.ModeloVeicularPlaca ?? string.Empty,
                DadosGta = string.Join(", ", listGta.Select(g => g.CodigoBarras).ToList()),
                CpfMotorista = carga.CPFPrimeiroMotorista,
                QuantidadeAnimais = string.Join(" / ",
                    listaDivisoesCapacidade.Select(l => l.Quantidade.ToString("n0")).ToList()),
                //QuantidadeAnimais = listaCargaPedidoProdutoCarga.Select(l => l.Quantidade).ToList().Aggregate((acc, x) => acc + x).ToString("n0"),
                NotasFiscais = string.Join(", ",
                    from notas in cargaEntregaNotaFiscals select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero),
                PossuiPerguntasColeta = possuiPerguntasColeta,
                PossuiPerguntasDesembarque = possuiPerguntasDesembarque,
                PossuiFotoNotasFiscais = possuiFotoNotasFiscais,
                PossuiFotosEmbarque = possuiFotoEmbarque,
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        string caminhoAssinaturaProdutor = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaProdutor =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        assinaturaProdutor.NomeParametro = "CaminhoAssinaturaProdutor";
        assinaturaProdutor.ValorParametro = cargaEntregaAssinatura != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinaturaProdutor,
                cargaEntregaAssinatura.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinatura.NomeArquivo))
            : string.Empty;
        parametros.Add(assinaturaProdutor);

        if (checkListPerguntasEmbarque != null)
        {
            foreach (var checklistPergunta in checkListPerguntasEmbarque)
            {
                foreach (var pergunta in checklistPergunta.Perguntas)
                {
                    dynamic resposta = TratarResposta(pergunta);

                    listaColetaPerguntasDS.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaPerguntas()
                        {
                            Pergunta = pergunta.Descricao,
                            Resposta = resposta
                        });
                }
            }
        }

        if (checkListPerguntasDesembarque != null)
        {
            foreach (var checklistPergunta in checkListPerguntasDesembarque)
            {
                foreach (var pergunta in checklistPergunta.Perguntas)
                {
                    dynamic resposta = TratarResposta(pergunta);

                    listaDesembarquePerguntasDS.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.DesembarquePerguntas()
                        {
                            Pergunta = pergunta.Descricao,
                            Resposta = pergunta.Resposta
                        });
                }
            }
        }

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosNotasFiscais> listaDsFotosNF = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosNotasFiscais>();
        string caminhoFotoNF = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "NotasFiscais" });
        if (cargaEntregaFotoNotasFiscais != null)
        {
            foreach (var fotoNotaFiscal in cargaEntregaFotoNotasFiscais)
            {
                listaDsFotosNF.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.
                        ComprovanteColetaFotosNotasFiscais()
                    {
                        CaminhoFoto = fotoNotaFiscal != null
                                ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoFotoNF, fotoNotaFiscal.GuidArquivo + "-miniatura" +  Path.GetExtension(fotoNotaFiscal.NomeArquivo))
                                : string.Empty
                    });
            }
        }

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosDesembarque> listaDsFotosDesembarque = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColetaFotosDesembarque>();
        string caminhoFotoEntrega = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
        if (fotosCargaEntrega != null)
        {
            foreach (var fotoCargaEntrega in fotosCargaEntrega)
            {
                listaDsFotosDesembarque.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.
                        ComprovanteColetaFotosDesembarque()
                    {
                        CaminhoDaFoto = fotoCargaEntrega != null
                                ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoFotoEntrega, fotoCargaEntrega.GuidArquivo + "-miniatura" + Path.GetExtension(fotoCargaEntrega.NomeArquivo))
                                : string.Empty
                    });
            }
        }

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "ComprovanteColetaPerguntas",
                DataSet = listaColetaPerguntasDS
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FotosNotasFiscais",
                DataSet = listaDsFotosNF
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "DesembarquePerguntas",
                DataSet = listaDesembarquePerguntasDS
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds4 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FotosDesembarque",
                DataSet = listaDsFotosDesembarque
            };

        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);
        subReports.Add(ds4);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ComprovanteColeta>(){ dataSourceComprovanteColeta },
                Parameters = parametros,
                SubReports = subReports
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\ComprovanteColeta.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o comprovante de coleta.");

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> ObterCheckListCargaEntrega(
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, TipoCheckList tipoCheckList,
        Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas =
            repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo,
                tipoCheckList);

        if (perguntas.Count() == 0)
            return null;

        Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList =
            new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork);
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkList =
            servicoCheckList.ObterObjetoMobileCheckList(perguntas);

        return checkList;
    }


    private dynamic TratarResposta(dynamic pergunta)
    {
        if ((TipoOpcaoCheckList)pergunta.Tipo == TipoOpcaoCheckList.SimNao)
            return pergunta.Resposta == "true" ? "Sim" : "Não";
        if ((TipoOpcaoCheckList)pergunta.Tipo == TipoOpcaoCheckList.Informativo)
            return pergunta.Resposta;
        if ((TipoOpcaoCheckList)pergunta.Tipo == TipoOpcaoCheckList.Opcoes || (TipoOpcaoCheckList)pergunta.Tipo == TipoOpcaoCheckList.Escala)
        {
            var resposta = string.Empty;
            bool registrouReposta = false;

            foreach (var alternativa in pergunta.Alternativas)
                if (alternativa.Marcado)
                    if (registrouReposta)
                        resposta += $", {alternativa.Descricao}";
                    else
                    {
                        resposta = alternativa.Descricao;
                        registrouReposta = true;
                    }

            return resposta;
        }
        return "";
    }
}