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
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.BoletimViagemEmbarque)]
public class BoletimViagemEmbarqueReport : ReportBase
{
    public BoletimViagemEmbarqueReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque>
            DSBoletimViagemEmbarque =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque>();
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega =
            repCargaEntrega.BuscarPorCarga(codigoCarga);

        PreencherBoletimViagemEmbarque(DSBoletimViagemEmbarque, cargasEntrega, codigoCarga, parametros);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = DSBoletimViagemEmbarque,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Cargas\BoletimViagemEmbarque.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        var pdfContent =
            RelatorioSemPadraoReportService.ObterBufferReport(report, Dominio.Enumeradores.TipoArquivoRelatorio.PDF);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }

    private void PreencherBoletimViagemEmbarque(
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque>
            DSBoletimViagemEmbarque,
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, int codigoCarga,
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros)
    {
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto =
            new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList =
            new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListPerguntasDesembarque = null;
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checklistEntrega = null;

        foreach (var cargaEntrega in cargasEntrega)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> listaPerguntas =
                repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo,
                    TipoCheckList.Desembarque);

            if (listaPerguntas.Count == 0)
                continue;

            checkListPerguntasDesembarque = servicoCheckList.ObterObjetoMobileCheckList(listaPerguntas);

            if (checkListPerguntasDesembarque != null)
                break;
        }

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

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
        {
            AdicionarParametrosAssinaturas(parametros, cargaEntrega);

            if (cargaEntrega.Coleta)
            {
                Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque boletimViagem =
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas =
                    repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo,
                        TipoCheckList.Coleta);
                List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkListEmbarque =
                    servicoCheckList.ObterObjetoMobileCheckList(perguntas);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos =
                    repCargaEntregaProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos =
                    repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga =
                    repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);

                if (checkListEmbarque != null)
                {
                    foreach (var checklistPergunta in checkListEmbarque)
                    {
                        PreencherRespostaPergunta(boletimViagem, checklistPergunta.Perguntas);
                    }
                }

                if (checklistEntrega != null)
                {
                    foreach (var checklistPergunta in checklistEntrega)
                    {
                        PreencherRespostaPergunta(boletimViagem, checklistPergunta.Perguntas);
                    }
                }

                if (checkListPerguntasDesembarque != null)
                {
                    foreach (var checklistDesembarque in checkListPerguntasDesembarque)
                    {
                        PreencherRespostaPergunta(boletimViagem, checklistDesembarque.Perguntas);
                    }
                }

                dynamic produtos = ObterListaProduto(cargaEntrega, cargaEntregaProdutos, cargaEntregaPedidos,
                    listaCargaPedidoProdutoCarga, _unitOfWork);

                if (produtos.Count > 0)
                {
                    if (produtos[0].DivisoesCapacidade.Count > 3)
                    {
                        boletimViagem.Carreta2AndaresColuna1Andar1 =
                            ((string)produtos[0].DivisoesCapacidade[0].Quantidade).Replace(",00", "");
                        boletimViagem.Carreta2AndaresColuna2Andar1 =
                            ((string)produtos[0].DivisoesCapacidade[1].Quantidade).Replace(",00", "");
                        boletimViagem.Carreta2AndaresColuna3Andar1 =
                            ((string)produtos[0].DivisoesCapacidade[2].Quantidade).Replace(",00", "");
                        boletimViagem.Carreta2AndaresColuna1Andar2 =
                            ((string)produtos[0].DivisoesCapacidade[3].Quantidade).Replace(",00", "");
                        boletimViagem.Carreta2AndaresColuna2Andar2 =
                            ((string)produtos[0].DivisoesCapacidade[4].Quantidade).Replace(",00", "");
                        boletimViagem.Carreta2AndaresColuna3Andar2 =
                            ((string)produtos[0].DivisoesCapacidade[5].Quantidade).Replace(",00", "");
                    }
                    else
                    {
                        boletimViagem.Carreta1AndarColuna1 =
                            ((string)produtos[0].DivisoesCapacidade[0].Quantidade).Replace(",00", "");
                        if (produtos[0].DivisoesCapacidade.Count > 1)
                            boletimViagem.Carreta1AndarColuna2 =
                            ((string)produtos[0].DivisoesCapacidade[1].Quantidade).Replace(",00", "");
                        if (produtos[0].DivisoesCapacidade.Count > 2)
                            boletimViagem.Carreta1AndarColuna3 =
                                ((string)produtos[0].DivisoesCapacidade[2].Quantidade).Replace(",00", "");
                    }
                }

                boletimViagem.Transportador = cargaEntrega.Cliente?.Nome ?? string.Empty;
                boletimViagem.Placa = carga.RetornarPlacas ?? string.Empty;
                boletimViagem.Motorista = carga.Veiculo?.NomePrimeiroMotorista ?? string.Empty;
                boletimViagem.TipoVeiculo = carga.ModeloVeicularCarga?.Descricao ?? string.Empty;
                boletimViagem.Pecuarista = cargaEntrega.Cliente?.Nome ?? string.Empty;
                boletimViagem.PecuaristaCPFCNPJ = cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? string.Empty;
                boletimViagem.Fazenda =
                    (from obj in cargaEntregaPedidos
                     where obj.CargaEntrega.Codigo == cargaEntrega.Codigo
                     select obj.CargaPedido?.Pedido?.Resumo).FirstOrDefault() ?? string.Empty;
                boletimViagem.Cidade = cargaEntrega.Cliente?.Localidade?.Descricao ?? string.Empty;
                boletimViagem.Estado = cargaEntrega.Cliente?.Localidade?.Estado?.Descricao ?? string.Empty;
                boletimViagem.NumeroCarga = carga.Numero;
                boletimViagem.DataConfirmacaoColeta = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ??
                                                      cargaEntrega.DataReprogramada?.ToString("dd/MM/yyyy HH:mm") ??
                                                      cargaEntrega.DataFimPrevista?.ToString("dd/MM/yyyy HH:mm");
                boletimViagem.Observacoes = cargaEntrega.Observacao;

                DSBoletimViagemEmbarque.Add(boletimViagem);
            }
        }
    }

    private void AdicionarParametrosAssinaturas(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros,
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
    {
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
            repCargaEntregaAssinaturaResponsavel =
                new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura =
            repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargaEntrega.Codigo);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaProdutor =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaRecebedor =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();

        if (cargaEntrega.Coleta)
        {
            string caminhoAssinaturaProdutor = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
            assinaturaProdutor.NomeParametro = "CaminhoAssinaturaProdutor";
            assinaturaProdutor.ValorParametro = cargaEntregaAssinatura != null
                ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinaturaProdutor,
                    cargaEntregaAssinatura.GuidArquivo + "-miniatura" +
                    System.IO.Path.GetExtension(cargaEntregaAssinatura.NomeArquivo))
                : string.Empty;
            parametros.Add(assinaturaProdutor);
        }
        else
        {
            string caminhoAssinaturaRecebedor = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
            assinaturaRecebedor.NomeParametro = "CaminhoAssinaturaRecebedor";
            assinaturaRecebedor.ValorParametro = cargaEntregaAssinatura != null
                ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinaturaRecebedor,
                    cargaEntregaAssinatura.GuidArquivo + "-miniatura" +
                    System.IO.Path.GetExtension(cargaEntregaAssinatura.NomeArquivo))
                : string.Empty;
            parametros.Add(assinaturaRecebedor);
        }
    }

    private void PreencherRespostaPergunta(
        Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemEmbarque boletimViagem,
        List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta> perguntas)
    {
        foreach (var pergunta in perguntas)
        {
            if (pergunta.Descricao.Contains("O pecuarista ou responsável estava presente no embarque?"))
                boletimViagem.PecuaristaPresente = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Condição do embarcadouro"))
                boletimViagem.CondicaoEmbracadorFazenda = ObterRespostaAlternativa(pergunta);
            else if (pergunta.Descricao.Contains(
                         "O piso do curral de manejo estava com excesso de lama e/ ou escorregadio?"))
                boletimViagem.PisoCurral = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains(
                         "Foi observado qualquer ato de abuso contra os animais durante o manejo de embarque? (ex: chute, paulada, cutucões fortes  no animal, tentativa de quebrar a cauda, arrastar o animal ferido etc..)"))
                boletimViagem.ObservadoAbusoContraAnimais = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Instrumentos de manejo utilizados no embarque"))
                boletimViagem.InstrumentoManejoEmbarque = ObterRespostaAlternativa(pergunta);
            else if (pergunta.Descricao.Contains(
                         "Foi observado manejo de apartação durante o embarque, como pesagem ou separação de animais?"))
                boletimViagem.HouveManejoApartacao = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("A documentação para início da viagem foi entregue rapidamente?"))
                boletimViagem.DocumentacaoInicioEntregueRapidamente =
                    ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("A propriedade tem local adequado de espera dos motoristas?"))
                boletimViagem.PropriedadeLocalAdequadoEsperaMotoristas =
                    ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Há água para os motoristas?"))
                boletimViagem.AguaParaMotoristas = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Cuidados do motorista durante embarque(responsavel da fazenda)"))
                boletimViagem.CuidadosMotoristaDuranteEmbarque = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Qualidade da gaiola (responsavel da fazenda)"))
                boletimViagem.QualidadeGaiola = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Houve transbordo?"))
                boletimViagem.HouveTransbordo = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Veículo atolou/quebrou durante a viagem?"))
                boletimViagem.VeiculoQuebrouAtolouViagem = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Km propriedade/Frigorífico"))
                boletimViagem.KmProriedadeFrigorifico = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Km asfalto") || pergunta.Descricao.Contains("Km Asfalto"))
                boletimViagem.KmAsfalto = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Km terra") || pergunta.Descricao.Contains("Km Terra"))
                boletimViagem.KmTerra = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Condição da estrada dentro da fazenda"))
                boletimViagem.CondicaoEntradaDentroFazenda = ObterRespostaAlternativa(pergunta);
            else if (pergunta.Descricao.Contains("Condições da estrada fora da fazenda"))
                boletimViagem.CondicaoEstradaForaFazenda = ObterRespostaAlternativa(pergunta);
            else if (pergunta.Descricao.Contains("Número de paradas durante o trajeto"))
                boletimViagem.NumeroParadasTrajeto = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Animais deitaram durante a viagem?"))
                boletimViagem.AnimaisDeitaramViagem = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Densidade dos animais está dentro do padrão?"))
                boletimViagem.DensidadadeAnimaisDentroPadrao = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Número do curral de destino"))
                boletimViagem.Curral = (string)pergunta.Resposta;
            else if (pergunta.Descricao.Contains("Animais deitados no veículo?"))
                boletimViagem.AnimaisDeitadosDentroVeiculo = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Animais com sinal de pisoteio?"))
                boletimViagem.AnimaisSujosOuPisoteio = ((string)pergunta.Resposta).ToBool() ? "Sim" : "Não";
            else if (pergunta.Descricao.Contains("Instrumento de manejo utilizado no desembarque"))
                boletimViagem.InstrumentosManejoDesembarque = ObterRespostaAlternativa(pergunta);
        }

        if (boletimViagem.AnimaisDeitadosDentroVeiculo == "Sim")
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta novaPergunta =
                perguntas.Where(p => p.Descricao.Contains("Número de animais deitados no desembarque"))
                    .FirstOrDefault();
            if (novaPergunta != null)
                boletimViagem.NumeroAnimaisDeitadosDesembarque = (string)novaPergunta.Resposta;
        }


        if (boletimViagem.PecuaristaPresente == "Sim")
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta novaPergunta =
                perguntas.Where(p => p.Descricao.Contains("Nome pecuarista ou responsável")).FirstOrDefault();
            if (novaPergunta != null)
                boletimViagem.NomePecuarista = (string)novaPergunta.Resposta;
        }

        if (boletimViagem.ObservadoAbusoContraAnimais == "Sim")
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta novaPergunta =
                perguntas.Where(p => p.Descricao.Contains("Descreva o ato de abuso identificado")).FirstOrDefault();
            if (novaPergunta != null)
                boletimViagem.DescricaoAbuso = (string)novaPergunta.Resposta;
        }

        if (boletimViagem.AnimaisDeitaramViagem == "Sim")
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta novaPergunta =
                perguntas.Where(p => p.Descricao.Contains("Número de animais que deitaram durante a viagem"))
                    .FirstOrDefault();
            if (novaPergunta != null)
                boletimViagem.NumeroAnimaisDeitados = (string)novaPergunta.Resposta;
        }
    }

    private string ObterRespostaAlternativa(
        Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta pergunta)
    {
        List<string> selecionadas = new List<string>();
        foreach (var alternativa in pergunta.Alternativas)
        {
            if (alternativa.Marcado)
                selecionadas.Add(alternativa.Descricao);
        }

        if (selecionadas.Count > 0)
            return string.Join(", ", selecionadas);

        return "";
    }

    private List<dynamic> ObterListaProduto(
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos,
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos,
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga,
        Repositorio.UnitOfWork unitOfWork)
    {
        if (cargaEntregaProdutos.Count == 0)
            return new List<dynamic>();

        Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade
            repositorioCargaPedidoProdutoDivisaoCapacidade =
                new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisaoCapacidadeEntrega =
            repositorioCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(cargaEntrega.Carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> listaDivisaoCapacidade =
            cargaEntrega.Carga.ModeloVeicularCargaVeiculo?.DivisoesCapacidade?.ToList() ??
            new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();

        List<dynamic> listaProdutos = new List<dynamic>();

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in
                 cargaEntregaPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto =
                (from obj in listaCargaPedidoProdutoCarga
                 where obj.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo
                 select obj).ToList();

            if (!listaCargaPedidoProduto.Any(o => o.Produto.PossuiIntegracaoColetaMobile))
                continue;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in
                     listaCargaPedidoProduto)
            {
                List<dynamic> listaDivisaoCapacidadeProduto = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisaoCapacidade in
                         listaDivisaoCapacidade)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade divisaoCapacidadeProduto = (
                        from o in listaDivisaoCapacidadeEntrega
                        where o.CargaPedidoProduto.Codigo == cargaPedidoProduto.Codigo &&
                              o.ModeloVeicularCargaDivisaoCapacidade.Codigo == divisaoCapacidade.Codigo
                        select o
                    ).FirstOrDefault();

                    listaDivisaoCapacidadeProduto.Add(new
                    {
                        divisaoCapacidade.Codigo,
                        Descricao =
                            $"{divisaoCapacidade.Descricao} ({Localization.Resources.Cargas.ControleEntrega.Capacidade} {divisaoCapacidade.Quantidade.ToString("n2")} {divisaoCapacidade.UnidadeMedida.Sigla})",
                        Quantidade = divisaoCapacidadeProduto?.Quantidade > 0
                            ? divisaoCapacidadeProduto.Quantidade.ToString("n2")
                            : "",
                        QuantidadePlanejada = divisaoCapacidadeProduto?.QuantidadePlanejada > 0
                            ? divisaoCapacidadeProduto.QuantidadePlanejada.ToString("n2")
                            : "",
                        Piso = divisaoCapacidade.Piso,
                        Coluna = divisaoCapacidade.Coluna
                    });
                }

                listaProdutos.Add(new
                {
                    Produto = new
                    {
                        cargaPedidoProduto.Codigo,
                        CodigoProduto = cargaPedidoProduto.Produto.Codigo,
                        ProdutoDescricao = cargaPedidoProduto.Produto.Descricao,
                        NumeroPedido = cargaPedidoProduto.CargaPedido.Pedido.NumeroPedidoEmbarcador,
                        Quantidade = cargaPedidoProduto.Quantidade.ToString("n2"),
                        QuantidadePlanejada = cargaPedidoProduto.QuantidadePlanejada.ToString("n2"),
                        Temperatura = cargaPedidoProduto.Temperatura.ToString("n2"),
                        cargaPedidoProduto.Produto.ObrigatorioInformarTemperatura,
                        JustificativaTemperatura = cargaPedidoProduto.JustificativaTemperatura?.Descricao ?? "",
                        ImunoPlanejado = cargaPedidoProduto.ImunoPlanejado,
                        ImunoRealizado = cargaPedidoProduto.ImunoRealizado,

                        // Aves
                        QuantidadeCaixa = cargaPedidoProduto.QuantidadeCaixa,
                        QuantidadePorCaixaRealizada = cargaPedidoProduto.QuantidadePorCaixaRealizada,
                        QuantidadeCaixasVazias = cargaPedidoProduto.QuantidadeCaixasVazias,
                        QuantidadeCaixasVaziasRealizada = cargaPedidoProduto.QuantidadeCaixasVaziasRealizada,
                    },
                    DivisoesCapacidade = listaDivisaoCapacidadeProduto
                });
            }
        }

        return listaProdutos;
    }
}