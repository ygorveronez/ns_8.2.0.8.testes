using System;
using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.CheckList)]
public class CheckListReport : ReportBase
{
    public CheckListReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repCheckList = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);

        var checklist = repCheckList.BuscarPorCodigo(extraData.GetValue<int>("CodigoCheckList"));

        Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa =
            new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(_unitOfWork);
        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa =
            new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);

        Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio =
            servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(checklist.FluxoGestaoPatio);
        List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas =
            (from o in checklist.Perguntas select o).ToList();

        byte[] pdf;

        if (sequenciaGestaoPatio?.TipoChecklistImpressao != null)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal =
                new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            perguntas = perguntas.Where(o => o.RelacaoPergunta != null).ToList();
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativas =
                repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(perguntas.Select(x => x.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais =
                repositorioXMLNotaFiscal.BuscarPorCarga((from o in perguntas select o.CheckListCarga?.Carga?.Codigo ?? 0).FirstOrDefault());
            dynamic dsInformacoes = ObterDataSourceCheckList(sequenciaGestaoPatio.TipoChecklistImpressao.LayoutCheckList, perguntas, checklist.FluxoGestaoPatio, notasFiscais, alternativas, checklist);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    DataSet = dsInformacoes
                };

            pdf = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\" + sequenciaGestaoPatio.TipoChecklistImpressao.Caminho,
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);
        }
        else
        {
            Repositorio.Embarcador.GestaoPatio.CheckListObservacao repositorioChecklistObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedidos = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase = checklist.CargaBase;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = checklist.Carga;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repositorioPedidos.BuscarPorCarga(carga.Codigo);

            List<string> listaNumeroPedidosEmbarcador = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAtual in listaPedidos)
            {
                string[] posFinal = pedidoAtual.NumeroPedidoEmbarcador.Split('_');
                if (posFinal.Length > 1)
                {
                    string pedidoAdicionar = posFinal[1];
                    listaNumeroPedidosEmbarcador.Add(pedidoAdicionar);
                }
                else
                    listaNumeroPedidosEmbarcador.Add(pedidoAtual.NumeroPedidoEmbarcador);
            }

            List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCarga> dsInformacoes =
                new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCarga>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCarga()
                    {
                        Chegada = checklist.DataAbertura,
                        Liberacao = checklist.DataLiberacao?.ToString("dd /MM/yyyy") ?? string.Empty,
                        Viagem = carga?.CodigoCargaEmbarcador ?? string.Empty,
                        Placa = cargaBase.Veiculo?.Placa ?? string.Empty,
                        Transportadora = cargaBase.Empresa != null
                            ? $"{cargaBase.Empresa.CNPJ_Formatado} - {cargaBase.Empresa.RazaoSocial}" : string.Empty,
                        Motorista = cargaBase.RetornarDescricaoTelefoneMotoristas,
                        QuantidadePaletesVazios = 0,
                        QuantidadePaletesComProduto = 0,
                        NotaFiscal = carga != null ? ObterNumeroNotasFiscais(carga) : string.Empty,
                        QuantidadeCaixasDevolucao = 0,
                        TipoDevolucao = "",
                        Motivo = "",
                        Lacre = carga?.Lacres != null
                            ? String.Join(" ,", (from o in carga.Lacres select o.Numero).ToList()) : string.Empty,
                        NumerosPedidosEmbarcador = string.Join(", ", listaNumeroPedidosEmbarcador)
                    }
                };
            List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCargaPergunta> dsPerguntas =
                new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCargaPergunta>();
            List<CategoriaOpcaoCheckList> categorias = (from o in checklist.Perguntas select o.Categoria).Distinct().ToList();
            List<int> codigosPerguntas = (from o in checklist.Perguntas select o.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativas =
                repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(codigosPerguntas);

            foreach (CategoriaOpcaoCheckList categoria in categorias)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao observacaoCategoria =
                    repositorioChecklistObservacao.BuscarPorCategoria(categoria);
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasPorCategoria = (
                    from o in checklist.Perguntas
                    where o.Categoria == categoria
                    orderby o.Codigo
                    select o
                ).ToList();
                List<SubCategoriaOpcaoCheckList> subcategorias =
                    perguntasPorCategoria.Select(o => o.Subcategoria).Distinct().ToList();

                foreach (SubCategoriaOpcaoCheckList subcategoria in subcategorias)
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasPorSubategoria =
                        perguntasPorCategoria.Where(o => o.Subcategoria == subcategoria).ToList();

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta in
                             perguntasPorSubategoria)
                    {
                        Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCargaPergunta dsPergunta =
                            new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListCargaPergunta()
                            {
                                Categoria = ObterDescricaoPorSubcategoria(checklist, categoria, subcategoria),
                                Descricao = pergunta.Descricao,
                                ObservacaoCategoria = observacaoCategoria?.Observacao
                            };

                        switch (pergunta.Tipo)
                        {
                            case TipoOpcaoCheckList.Aprovacao:
                                dsPergunta.Opcao1 = "Aprovada";
                                dsPergunta.Opcao2 = "Reprovada";
                                dsPergunta.Opcao1Selecionada = pergunta.Resposta.HasValue &&
                                                               pergunta.Resposta == CheckListResposta.Aprovada;
                                dsPergunta.Opcao2Selecionada = pergunta.Resposta.HasValue &&
                                                               pergunta.Resposta == CheckListResposta.Reprovada;
                                dsPergunta.Observacao = pergunta.Observacao;
                                break;

                            case TipoOpcaoCheckList.Informativo:
                                dsPergunta.Observacao = pergunta.Observacao;
                                break;

                            case TipoOpcaoCheckList.Opcoes:
                            case TipoOpcaoCheckList.Selecoes:
                                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>
                                    alternativasPorPergunta = (
                                        from o in alternativas
                                        where o.CheckListCargaPergunta.Codigo == pergunta.Codigo
                                        orderby o.Ordem
                                        select o
                                    ).ToList();

                                for (int i = 0; i < alternativasPorPergunta.Count; i++)
                                {
                                    Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa
                                        alternativa = alternativasPorPergunta[i];

                                    dsPergunta.GetType().GetProperty($"Opcao{(i + 1)}")
                                        ?.SetValue(dsPergunta, alternativa.Descricao);
                                    dsPergunta.GetType().GetProperty($"Opcao{(i + 1)}Selecionada")
                                        ?.SetValue(dsPergunta, alternativa.Marcado);
                                }

                                break;

                            case TipoOpcaoCheckList.SimNao:
                                dsPergunta.Opcao1 = "Sim";
                                dsPergunta.Opcao2 = "Não";
                                dsPergunta.Opcao1Selecionada = pergunta.Resposta.HasValue &&
                                                               pergunta.Resposta == CheckListResposta.Aprovada;
                                dsPergunta.Opcao2Selecionada = pergunta.Resposta.HasValue &&
                                                               pergunta.Resposta == CheckListResposta.Reprovada;
                                break;
                        }

                        dsPerguntas.Add(dsPergunta);
                    }
                }
            }

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
                new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura repositorioChecklistAssinatura =
                new Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura> assinaturas =
                repositorioChecklistAssinatura.BuscarPorCheckList(checklist.Codigo);

            string caminho = ObterCaminhoArquivoAssinatura(_unitOfWork);


            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinaturaMotorista =
                assinaturas.Where(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.Motorista).FirstOrDefault();
            Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroAssinaturaMotorista =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
            parametroAssinaturaMotorista.NomeParametro = "CaminhoAssinaturaMotorista";
            parametroAssinaturaMotorista.ValorParametro = assinaturaMotorista != null
                ? Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{assinaturaMotorista.GuidArquivo}.png").FirstOrDefault() ??
                  string.Empty
                : string.Empty;
            parametros.Add(parametroAssinaturaMotorista);

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinaturaCarregador =
                assinaturas.Where(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.Carregador).FirstOrDefault();
            Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroAssinaturaCarregador =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
            parametroAssinaturaCarregador.NomeParametro = "CaminhoAssinaturaCarregador";
            parametroAssinaturaCarregador.ValorParametro = assinaturaCarregador != null
                ? Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{assinaturaCarregador.GuidArquivo}.png").FirstOrDefault() ??
                  string.Empty
                : string.Empty;
            parametros.Add(parametroAssinaturaCarregador);

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinaturaResponsavelAprovacao =
                assinaturas.Where(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.ResponsavelAprovacao).FirstOrDefault();
            Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroAssinaturaResponsavelAprovacao =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
            parametroAssinaturaResponsavelAprovacao.NomeParametro = "CaminhoAssinaturaResponsavelAprovacao";
            parametroAssinaturaResponsavelAprovacao.ValorParametro = assinaturaResponsavelAprovacao != null
                ? Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{assinaturaResponsavelAprovacao.GuidArquivo}.png").FirstOrDefault() ?? string.Empty : string.Empty;
            parametros.Add(parametroAssinaturaResponsavelAprovacao);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    DataSet = dsInformacoes,
                    Parameters = parametros,
                    SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                    {
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                        {
                            Key = "CheckListCargaPergunta.rpt",
                            DataSet = dsPerguntas
                        }
                    }
                };

            pdf = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\GestaoPatio\CheckListCarga.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        }

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private dynamic ObterDataSourceCheckList(LayoutCheckList layout,
        List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas,
        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio,
        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais,
        List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativas,
        Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checkList)
    {
        switch (layout)
        {
            case LayoutCheckList.NaturalOne:
                return new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListNaturalOne>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListNaturalOne()
                    {
                        //Campos preenchidos no fluxo de pátio
                        LarguraContainer =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Largura")
                                select o.Observacao).FirstOrDefault(),
                        AlturaContainer =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Altura")
                                select o.Observacao).FirstOrDefault(),
                        ComprimentoContainer =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Comprimento")
                                select o.Observacao).FirstOrDefault(),
                        QuantidadeFardosSeparados =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Fardos Separados")
                                select o.Observacao).FirstOrDefault().ToDouble().ToString("n0").Replace(",", "."),
                        Lacre = (from o in perguntas
                            where o.RelacaoPergunta.Descricao.Contains("LACRE")
                            select o.Observacao).FirstOrDefault(),
                        Doca = (from o in perguntas
                            where o.RelacaoPergunta.Descricao.Contains("DOCA")
                            select o.Observacao).FirstOrDefault(),
                        Stage = (from o in perguntas
                            where o.RelacaoPergunta.Descricao.Contains("STAGE")
                            select o.Observacao).FirstOrDefault(),
                        TemperaturaInicio =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Temperatura – Início")
                                select o.Observacao).FirstOrDefault(),
                        TemperaturaMeio =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Temperatura – Meio")
                                select o.Observacao).FirstOrDefault(),
                        TemperaturaFim =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Temperatura – Fim")
                                select o.Observacao).FirstOrDefault(),
                        ObservacaoGeral =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Observação")
                                select o.Observacao).FirstOrDefault(),
                        ResponsavelCarregamento =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Responsável pelo Carregamento")
                                select o.Observacao).FirstOrDefault(),
                        MotoristaAcompanhouCarregamento =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("acompanhou o carregamento")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        HouveEstouroCarga =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("estouro da carga")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        HouveRemontagem =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("remontagem")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        ObservacaoCheckList =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Observações")
                                select o.Observacao).FirstOrDefault(),
                        RegraLotoFoiCumprida =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Regra do Loto foi cumprida?")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        VeiculoComOdor =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Veículo com odor?")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        ThermoFuncionando =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Thermo funcionando?")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        PossuiDivisoria =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Possui divisória?")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        CondicaoLimpezaVeiculo = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta =>
                                        pergunta.RelacaoPergunta.Descricao.Contains("Condição de limpeza do veículo"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),

                        //Campos automáticos
                        NumeroCarga = (from o in perguntas select o.CheckListCarga?.Carga?.CodigoCargaEmbarcador ?? "")
                            .FirstOrDefault(),
                        Transportador = (from o in perguntas select o.CheckListCarga?.Carga?.Empresa?.Descricao ?? "")
                            .FirstOrDefault(),
                        PlacaCavalo = (from o in perguntas select o.CheckListCarga?.Carga?.Veiculo?.Placa ?? "")
                            .FirstOrDefault(),
                        PlacaCarreta = string.Join(", ",
                            perguntas.FirstOrDefault().CheckListCarga?.Carga?.VeiculosVinculados?.Select(o => o.Placa)),
                        Motorista =
                            perguntas.FirstOrDefault().CheckListCarga?.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? "",
                        TelefoneMotorista = perguntas.FirstOrDefault().CheckListCarga?.Carga?.Motoristas
                            ?.FirstOrDefault()?.Telefone_Formatado ?? "",
                        CapacidadePallets =
                            (from o in perguntas select o.CheckListCarga?.Carga?.Veiculo?.CapacidadeM3.ToString() ?? "")
                            .FirstOrDefault(),
                        PerfilVeiculo =
                            (from o in perguntas select o.CheckListCarga?.Carga?.ModeloVeicularCarga?.Descricao ?? "")
                            .FirstOrDefault(),
                        CnhMotorista = perguntas.FirstOrDefault().CheckListCarga?.Carga?.Motoristas?.FirstOrDefault()
                            ?.NumeroHabilitacao ?? "",
                        QuantidadeFardosPlanejados = perguntas.FirstOrDefault().CheckListCarga?.Carga?.DadosSumarizados
                            ?.VolumesTotal.ToString("n0").Replace(",", ".") ?? "",
                        QuantidadeFardosFaturados = perguntas.FirstOrDefault().CheckListCarga?.Carga?.DadosSumarizados
                            ?.QuantidadeVolumesNF?.ToString("n0").Replace(",", ".") ?? "",
                        PesoDaCargaPlanejado = perguntas.FirstOrDefault().CheckListCarga?.Carga?.DadosSumarizados
                            ?.PesoTotal.ToString("n2") ?? "",
                        PesoDaCargaFaturado = notasFiscais.Sum(o => o.Peso).ToString("n2"),
                        ChegadaPatio = fluxoPatio?.DataSolicitacaoVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        LiberacaoDocumentos = fluxoPatio?.DataFimCheckList?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Notas = string.Join(", ", notasFiscais.Select(o => o.Numero)),
                        AutorizadoDpp = fluxoPatio?.DataSolicitacaoVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        InicioCarregamento = fluxoPatio?.DataInicioCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        FimCarregamento = fluxoPatio?.DataFimCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? ""
                    }
                };

            case LayoutCheckList.Pam:
                return new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListPam>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListPam()
                    {
                        //Campos preenchidos no fluxo de pátio
                        Epi = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("EPI") select o.Resposta)
                            .FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        Assoalho = alternativas.Where(obj => obj.Marcado == true && obj.CheckListCargaPergunta.Codigo ==
                                perguntas.Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Assoalho"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        PneusCarreta = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Pneus Carreta"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        LuzDeFreiosSeta = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta =>
                                        pergunta.RelacaoPergunta.Descricao.Contains("Luz de freios e setas"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        Estepe = alternativas.Where(obj => obj.Marcado == true && obj.CheckListCargaPergunta.Codigo ==
                                perguntas.Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Estepe"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        TipoAssoalho = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Tipo Assoalho"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        ParabrisaTrincado = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta =>
                                        pergunta.RelacaoPergunta.Descricao.Contains("Parabrisa trincado"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        Carroceria = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Carroceria"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        PneusCavalo = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Pneus Cavalo"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        FaroisLantera = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Faróis / Lanterna"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        LonaEncerada = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Lona de encerada"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        PeriodoChuva =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Período de Chuva")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        TipoCarroceria = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Tipo Carroceria"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        VazamentoOleo = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Vazamento de Óleo"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        Grampos = alternativas.Where(obj => obj.Marcado == true && obj.CheckListCargaPergunta.Codigo ==
                                perguntas.Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Grampos"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        TravaTampas = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Trava das Tampas"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        Barrotes = alternativas.Where(obj => obj.Marcado == true && obj.CheckListCargaPergunta.Codigo ==
                                perguntas.Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Barrotes"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        CordasCintasCatracas = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta =>
                                        pergunta.RelacaoPergunta.Descricao.Contains("Cordas/Cintas/Catracas"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        MedidaCarroceria =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Medida de Carroceria")
                                select o.Observacao).FirstOrDefault(),
                        TelaProtecao = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Tela de Proteção"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)
                            ?.FirstOrDefault(),
                        InformacoesModeloVeicularCarga =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Informações Modelo Veicular")
                                select $"{o.Descricao}: {o.Resposta?.ObterDescricaoSimNao()}").FirstOrDefault(),

                        //CondicoesExtCavalo = alternativas.Where(obj => obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas.Where(pergunta => pergunta.RelacaoPergunta.Descricao.Contains("Condições ext. do cavalo")).Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)?.FirstOrDefault(),
                        //LatariaAmassada = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Lataria amassada") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //PortaNaoFecha = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Porta não fecha") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //BancoMotoristaRuim = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Banco do motorista ruim") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //FaltaRetrovisores = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Falta de retrovisores") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //CapoNaoFecha = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Capô não fecha") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //FaltaVidroLateral = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Falta vidro lateral") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //PinturaRuim = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Pintura ruim") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //FaltaVidroNaPorta = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Falta vidro na porta") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //ParaChoqueDanificado = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Pára-choque danificado") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //VidroRetrovisorQuebrado = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Vidro retrovisor quebrado") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //Observacao = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Observação") select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricaoSimNao(),
                        //Motivos = (from o in perguntas where o.RelacaoPergunta.Descricao.Contains("Motivos") select o.Observacao).FirstOrDefault(),
                        StatusInspecao =
                            (from o in perguntas
                                where o.RelacaoPergunta.Descricao.Contains("Status Inspeção")
                                select o.Resposta).FirstOrDefault().GetValueOrDefault().ObterDescricao(),

                        //Campos automáticos
                        DataInicio = checkList?.DataLiberacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataFim = fluxoPatio?.DataFimCheckList?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Placa = fluxoPatio?.Carga?.Veiculo?.Placa ?? "",
                        Condutor = fluxoPatio?.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? "",
                        CpfCondutor = fluxoPatio?.Carga?.Motoristas?.FirstOrDefault()?.CPF ?? "",
                        TipoVeiculo = fluxoPatio?.Carga?.Veiculo?.ModeloVeicularCarga?.Descricao ?? "",
                        Vistoriador = checkList?.Aprovador?.Nome ?? "",
                        Transportadora = fluxoPatio?.Carga?.Empresa?.Descricao ?? "",
                        TipoFrete = fluxoPatio?.Carga?.TipoFreteEscolhido == TipoFreteEscolhido.Cliente ? "FOB" : "CIF",
                        TipoTransporte = alternativas.Where(obj =>
                                obj.Marcado == true && obj.CheckListCargaPergunta.Codigo == perguntas
                                    .Where(pergunta =>
                                        pergunta.RelacaoPergunta.Descricao.Contains("Tipo de Transporte"))
                                    .Select(pergunta => pergunta.Codigo).FirstOrDefault())?.Select(obj => obj.Descricao)?
                            .FirstOrDefault() ?? "Particular",
                    }
                };
        }

        return new List<dynamic>();
    }

    private string ObterNumeroNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga)
    {
        List<string> notas = new List<string>();

        foreach (var pedido in carga.Pedidos)
            notas.AddRange((from o in pedido.NotasFiscais select o.XMLNotaFiscal.Numero.ToString()).ToList());

        return String.Join(", ", notas);
    }

    private string ObterDescricaoPorSubcategoria(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist,
        CategoriaOpcaoCheckList categoria, SubCategoriaOpcaoCheckList subcategoria)
    {
        string descricao = "";

        if (subcategoria == SubCategoriaOpcaoCheckList.NaoDefinido)
            return categoria.ObterDescricao();

        if (subcategoria == SubCategoriaOpcaoCheckList.Reboque)
            descricao = $"Reboque {checklist.CargaBase.VeiculosVinculados?.ElementAtOrDefault(0).Placa_Formatada}";

        if (subcategoria == SubCategoriaOpcaoCheckList.SegundoReboque)
            descricao =
                $"Segundo Reboque {checklist.CargaBase.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa_Formatada}";

        return descricao;
    }

    private string ObterCaminhoArquivoAssinatura(Repositorio.UnitOfWork unitOfWork)
    {
        return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CheckListCarga", "Assinaturas" });
    }
}