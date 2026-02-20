using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.MinutaTransporte)]
public class MinutaTransporteReport : ReportBase
{
    public MinutaTransporteReport(UnitOfWork unitOfWork, IStorage storage, RelatorioReportService servicoRelatorioReportService) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

        int CodigoCarga = extraData.GetValue<int>("CodigoCarga");

        var carga = repCarga.BuscarPorCodigo(CodigoCarga);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.MinutaTransporte> dsMinutaTransporte =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.MinutaTransporte>();

        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        Repositorio.Embarcador.Pedidos.Pedido
            repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
            repCargaEntregaAssinaturaResponsavel =
                new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(_unitOfWork);

        Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList =
            new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega =
            repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);
        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorUnicaCarga(carga.Codigo);
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
            cargaEntregaAssinaturaResponsavel =
                repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargasEntrega.Where(o => o.Coleta)
                    .FirstOrDefault()?.Codigo ?? 0);
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
            cargaEntregaAssinaturaRecebedor =
                repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargasEntrega.Where(o => !o.Coleta)
                    .FirstOrDefault()?.Codigo ?? 0);
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checklistEntrega = null;
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListPerguntasDesembarque = null;

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntasEntrega =
                repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo,
                    TipoCheckList.Entrega);

            if (perguntasEntrega.Count == 0)
                continue;

            checklistEntrega = servicoCheckList.ObterObjetoMobileCheckList(perguntasEntrega);

            if (checklistEntrega != null)
                break;
        }

        foreach (var cargaEntrega in cargasEntrega)
        {
            checkListPerguntasDesembarque =
                ObterCheckListCargaEntrega(cargaEntrega, TipoCheckList.Desembarque, _unitOfWork);

            if (checkListPerguntasDesembarque != null)
                continue;
        }

        foreach (var cargaEntrega in cargasEntrega)
        {
            if (cargaEntrega.Coleta)
            {
                Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.MinutaTransporte minutaTransporte =
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.MinutaTransporte();
                List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListPerguntasEmbarque =
                    ObterCheckListCargaEntrega(cargaEntrega, TipoCheckList.Coleta, _unitOfWork);

                if (checkListPerguntasEmbarque != null)
                {
                    foreach (var checklistPergunta in checkListPerguntasEmbarque)
                    {
                        foreach (var pergunta in checklistPergunta.Perguntas)
                        {
                            if (pergunta.Descricao.Contains("Horário de Início do Embarque"))
                                minutaTransporte.HorarioInicioEmbarque = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Horário de Término do Embarque"))
                                minutaTransporte.HorarioFimEmbarque = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Quantidade de animais embarcados"))
                                minutaTransporte.QuantidadeAnimaisEmbarcados = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains(
                                         "Foi embarcado algum animal machucado? (mancando, sangrando, com lesão, muito magro etc)"))
                                minutaTransporte.AnimalMachucado = pergunta.Resposta == "true" ? "Sim" : "Não";
                            else if (pergunta.Descricao.Contains(
                                         "Qual o horario do fechamento do gado? (Fazenda Pecuarista)"))
                                minutaTransporte.HorarioFechamentoGado = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains(
                                         "Horário de saída da fazenda. (Horário que  seguiu viagem)"))
                                minutaTransporte.HorarioSaidaFazenda = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Km propriedade/Frigorífico"))
                                minutaTransporte.KMPropriedadeFrigorifico = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Km Asfalto"))
                                minutaTransporte.KmAsfalto = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Km Terra"))
                                minutaTransporte.KmTerra = pergunta.Resposta;

                            if (minutaTransporte.AnimalMachucado == "Sim")
                                foreach (var novaPergunta in checklistPergunta.Perguntas)
                                {
                                    if (novaPergunta.Descricao.Contains("Quantos animais machucados foram embarcados?"))
                                        minutaTransporte.QuantidadeMachucado = pergunta.Resposta;
                                }
                        }
                    }
                }

                foreach (var checklistPergunta in checklistEntrega)
                {
                    foreach (var pergunta in checklistPergunta.Perguntas)
                    {
                        if (pergunta.Descricao.Contains("Km propriedade/Frigorífico"))
                            minutaTransporte.KMPropriedadeFrigorifico = pergunta.Resposta;
                        else if (pergunta.Descricao.Contains("Km asfalto"))
                            minutaTransporte.KmAsfalto = pergunta.Resposta;
                        else if (pergunta.Descricao.Contains("Km terra"))
                            minutaTransporte.KmTerra = pergunta.Resposta;
                    }
                }

                if (checkListPerguntasDesembarque != null)
                {
                    foreach (var checklistDesembarque in checkListPerguntasDesembarque)
                    {
                        foreach (var pergunta in checklistDesembarque.Perguntas)
                        {
                            if (pergunta.Descricao.Contains("Data desembarque"))
                                minutaTransporte.DataDesembarque = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Horário desembarque"))
                                minutaTransporte.HorarioDesembarque = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Quantidade de animais desembarcados"))
                                minutaTransporte.QuantidadeAnimaisDesembarque = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Número do curral de destino"))
                                minutaTransporte.NumeroCurral = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Houve mistura de categorias?"))
                                minutaTransporte.HouveMistura = pergunta.Resposta == "true" ? "Sim" : "Não";
                            else if (pergunta.Descricao.Contains("Animais machucados  ou com dificuldade de andar?"))
                                minutaTransporte.AnimaisDificuldadeAndar = pergunta.Resposta == "true" ? "Sim" : "Não";
                            else if (pergunta.Descricao.Contains("Houve abate de emergência?"))
                                minutaTransporte.HouveAbateEmergencia = pergunta.Resposta == "true" ? "Sim" : "Não";
                            else if (pergunta.Descricao.Contains("Algum animal chegou morto?"))
                                minutaTransporte.AlgumAnimalMorto = pergunta.Resposta == "true" ? "Sim" : "Não";
                            else if (pergunta.Descricao.Contains("Km propriedade/Frigorífico"))
                                minutaTransporte.KMPropriedadeFrigorifico = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Km Asfalto"))
                                minutaTransporte.KmAsfalto = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Km Terra"))
                                minutaTransporte.KmTerra = pergunta.Resposta;
                            else if (pergunta.Descricao.Contains("Categoria animal"))
                            {
                                List<string> alternativas = pergunta.Alternativas.Where(alternativa => alternativa.Marcado).Select(resposta => resposta.Descricao).ToList();
                                minutaTransporte.Categoria = String.Join(", ", alternativas);
                            }
                            else if (pergunta.Descricao.Contains("Raça bovina"))
                            {
                                List<string> alternativas = pergunta.Alternativas.Where(alternativa => alternativa.Marcado).Select(resposta => resposta.Descricao).ToList();
                                minutaTransporte.RacaBovina = String.Join(", ", alternativas);
                            }
                            else if (pergunta.Descricao.Contains("Horário de chegada no frigorífico"))
                                minutaTransporte.HorarioChegadaFrigorifico = pergunta.Resposta;

                            if (minutaTransporte.AnimaisDificuldadeAndar == "Sim")
                                foreach (var novaPergunta in checklistDesembarque.Perguntas)
                                {
                                    if (pergunta.Descricao.Contains(
                                            "Número de animais machucados ou com dificuldade de andar"))
                                        minutaTransporte.QuantidadeDificuladeAndar = pergunta.Resposta;
                                }

                            if (minutaTransporte.HouveAbateEmergencia == "Sim")
                                foreach (var novaPergunta in checklistDesembarque.Perguntas)
                                {
                                    if (pergunta.Descricao.Contains(
                                            "Número de animais que passaram por abate de emergência"))
                                        minutaTransporte.QuantidadeAbateEmergencia = pergunta.Resposta;
                                }

                            if (minutaTransporte.AlgumAnimalMorto == "Sim")
                                foreach (var novaPergunta in checklistDesembarque.Perguntas)
                                {
                                    if (pergunta.Descricao.Contains("Número de animais que chegaram mortos"))
                                        minutaTransporte.QuantidadeChegaramMortos = pergunta.Resposta;
                                }
                        }
                    }
                }

                minutaTransporte.Pecuarista = cargaEntrega.Cliente?.Nome ?? string.Empty;
                minutaTransporte.Cidade = cargaEntrega.Cliente?.Localidade?.Descricao ?? string.Empty;
                minutaTransporte.NumeroCarga = carga.Numero;
                minutaTransporte.Transportador = carga.Empresa?.NomeFantasia ?? string.Empty;
                minutaTransporte.Placa = carga.DadosSumarizados?.Veiculos ?? string.Empty;
                minutaTransporte.Motorista = carga.Veiculo?.NomePrimeiroMotorista ?? string.Empty;
                minutaTransporte.Fazenda = pedido?.Resumo ?? string.Empty;
                minutaTransporte.TipoVeiculo = carga.ModeloVeicularCarga?.Descricao ?? string.Empty;
                minutaTransporte.UF = cargaEntrega.Cliente?.Localidade?.Estado?.Sigla ?? string.Empty;
                minutaTransporte.DataColeta = cargaEntrega.DataInicio?.ToDateTimeString();
                minutaTransporte.CPFCNPJ = cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? string.Empty;
                
                dsMinutaTransporte.Add(minutaTransporte);
            }
        }

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        string caminhoAssinatura = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });

        string caminhoPadrao = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], "crystal.png");
        string caminhoLogo = string.Empty;
        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPadrao))
        {
            caminhoLogo = caminhoPadrao;
        }
       
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaResponsavel =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        assinaturaResponsavel.NomeParametro = "CaminhoAssinaturaResponsavel";
        assinaturaResponsavel.ValorParametro = cargaEntregaAssinaturaResponsavel != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinatura,
                cargaEntregaAssinaturaResponsavel.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinaturaResponsavel.NomeArquivo))
            : string.Empty;
        parametros.Add(assinaturaResponsavel);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaRecebedor =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        assinaturaRecebedor.NomeParametro = "CaminhoAssinaturaRecebedor";
        assinaturaRecebedor.ValorParametro = cargaEntregaAssinaturaRecebedor != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinatura,
                cargaEntregaAssinaturaRecebedor.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinaturaRecebedor.NomeArquivo))
            : caminhoLogo;
        parametros.Add(assinaturaRecebedor);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsMinutaTransporte,
                Parameters = parametros,
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\MinutaTransporte.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o comprovante de coleta.");

        var result = PrepareReportResult(FileType.PDF, pdf);

        return result;
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
}