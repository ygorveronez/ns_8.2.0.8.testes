using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;

namespace ReportApi.Reports;

[UseReportType(ReportType.ImpressaoPedido)]
public class ImpressaoPedidoReport : ReportBase
{
    public ImpressaoPedidoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        bool planejamentoPedido = extraData.GetValue<bool>("planejamentoPedido");
        bool carregamento = extraData.GetValue<bool>("carregamento");
        bool impressaoCarga = extraData.GetValue<bool>("impressaoCarga");
        int codigo = extraData.GetValue<int>("codigo");
        bool ordemColeta = extraData.GetValue<bool>("ordemColeta");
        bool planoViagem = extraData.GetValue<bool>("planoViagem");
        string stringConexaoPar = extraData.GetValue<string>("stringConexaoPar");
        string nomeFantasia = extraData.GetValue<string>("nomeFantasia");
        Dominio.Entidades.Usuario usuario = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario"));
        bool gerarPorThread = extraData.GetValue<bool>("gerarPorThread");
        int codigoPedido = extraData.GetValue<int>("codigoPedido");
        int codigoCarga = extraData.GetValue<int>("codigoCarga");

        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware =
            extraData.GetInfo().TipoServico;

        string msg = "";
        string guidRelatorio = "";
        CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno = null;

        Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

        Repositorio.Embarcador.Relatorios.Relatorio repRelatorio =
            new Repositorio.Embarcador.Relatorios.Relatorio(_unitOfWork);
        Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido =
            new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS =
            repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
            new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);

        if (usuario == null)
            usuario = repUsuario.BuscarPrimeiro();

        string codigosPedidos = "";
        if (carregamento)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidos =
                repCarregamentoPedido.BuscarPorCarregamento(codigo);
            if (pedidos.Count > 0)
                codigosPedidos = string.Join(", ", pedidos.Select(obj => obj.Pedido.Codigo)).ToString();
            if (string.IsNullOrWhiteSpace(codigosPedidos))
                throw new ServerException("Nenhum registro de pedido para regar o relatório.");
        }
        else if (impressaoCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            if (pedidos.Count > 0)
                codigosPedidos = string.Join(", ", pedidos.Select(obj => obj.Pedido.Codigo)).ToString();
            if (string.IsNullOrWhiteSpace(codigosPedidos))
                throw new ServerException("Nenhum registro de pedido para regar o relatório.");
        }

        TipoImpressaoPedido tipoImpressaoPedido = configuracaoTMS.TipoImpressaoPedido;
        TipoImpressaoPedidoPrestacaoServico
            tipoImpressaoPrestacao = configuracaoTMS.TipoImpressaoPedidoPrestacaoServico;

        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio =
            repRelatorio.BuscarPadraoPorCodigoControleRelatorio(
                Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                tipoServicoMultisoftware);
        if (relatorio == null)
        {
            if (planoViagem)
                relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                    Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                    tipoServicoMultisoftware, "Plano de Viagem", "Pedidos", "PlanoViagemTMS.rpt",
                    Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0,
                    _unitOfWork, false, false);
            else if (ordemColeta)
            {
                if (tipoImpressaoPedido == TipoImpressaoPedido.AutorizacaoEntrega)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Ordem de Coleta", "Pedidos", "OrdemColetaTMSEntrega.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
                else if (tipoImpressaoPedido == TipoImpressaoPedido.AutorizacaoCarregamento)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Ordem de Coleta", "Pedidos", "OrdemColetaTMS.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
                else if (tipoImpressaoPedido == TipoImpressaoPedido.OrdemServicoOrdemColeta)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Ordem de Coleta", "Pedidos", "OrdemColetaOrdemServico.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
                else if (tipoImpressaoPedido == TipoImpressaoPedido.Simplificada)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Ordem de Coleta", "Pedidos", "OrdemColetaSimplificada.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
            }
            else
            {
                if (tipoImpressaoPrestacao == TipoImpressaoPedidoPrestacaoServico.PrestacaoSemViaCliente)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Relatorio de Prestação de Serviço", "Pedidos",
                        "PedidoPrestacaoServicoSemViaCliente.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
                else
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R096_PrestacaoServico,
                        tipoServicoMultisoftware, "Relatorio de Prestação de Serviço", "Pedidos",
                        "PedidoPrestacaoServico.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, _unitOfWork, false, false);
            }
        }

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao =
            serRelatorio.AdicionarRelatorioParaGeracao(relatorio, usuario,
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, _unitOfWork);

        string stringConexao = stringConexaoPar;
        string nomeCliente = nomeFantasia;
        string caminhoPagina = ObterCaminhoPaginaRelatorioTMS(planejamentoPedido, impressaoCarga, carregamento);
        //int codigoPedido = pedido?.Codigo ?? 0;

        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = carga?.GrupoPessoaPrincipal ??
                                                                         pedido?.GrupoPessoas ??
                                                                         pedido?.ObterTomador()?.GrupoPessoas ??
                                                                         pedido?.Remetente?.GrupoPessoas;
        if (!planoViagem && ordemColeta && (grupoPessoas?.GerarImpressaoOrdemColetaExclusiva ?? false))
        {
            bool valido = ImpressaoOrdemColetaExclusiva(out string mensagem, caminhoPagina, grupoPessoas.Codigo,
                grupoPessoas.TipoImpressaoOrdemColetaExclusiva, codigoPedido, codigosPedidos, impressaoCarga,
                stringConexao, relatorioControleGeracao, out reportRetorno, gerarPorThread, out guidRelatorio);
            if (!valido)
            {
                msg = mensagem;
                throw new ServerException(mensagem);
            }
        }
        else
        {
            string viaImpressao = ordemColeta ? string.Empty : tipoImpressaoPrestacao == TipoImpressaoPedidoPrestacaoServico.PrestacaoComViaCliente ? " VIA CLIENTE" : " VIA OPERACIONAL";
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> dadosPedido = repPedido.RelatorioPedido(codigoPedido, codigosPedidos, impressaoCarga, viaImpressao);
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico> dadosImportacaoPrestacaoServico = repPedido.RelatorioImportacaoPedido(codigoPedido, codigosPedidos);
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico> dadosMotoristasPrestacaoServico = repPedido.RelatorioMotoristaPedido(codigoPedido, codigosPedidos, impressaoCarga);
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> dadosONUPrestacaoServico = repPedido.RelatorioONUPedido(codigoPedido, codigosPedidos);
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> dadosProdutosPrestacaoServico = repPedido.RelatorioProdutoPedido(codigoPedido, codigosPedidos);

            Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta ordemServicoOrdemColeta = null;
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada> dadosOrdemColetaSimplificada = null;
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista> dadosOrdemColetaSimplificadaMotorista = null;

            if (ordemColeta && !planoViagem)
            {
                if (tipoImpressaoPedido == TipoImpressaoPedido.OrdemServicoOrdemColeta)
                    ordemServicoOrdemColeta = ObterDetalhesOrdemServicoOrdemColeta(codigoPedido, carga?.Codigo ?? 0);
                else if (tipoImpressaoPedido == TipoImpressaoPedido.Simplificada)
                {
                    dadosOrdemColetaSimplificada =
                        repPedido.RelatorioOrdemColetaSimplificada(codigoPedido, codigosPedidos, impressaoCarga);
                    dadosOrdemColetaSimplificadaMotorista =
                        repPedido.RelatorioOrdemColetaSimplificadaMotorista(codigoPedido, codigosPedidos, impressaoCarga);
                }
            }

            if (dadosPedido.Count == 0)
            {
                msg = "Nenhum registro de pedido para gerar o relatório.";
                throw new ServerException(msg);
                //return false;
            }

            if (!ordemColeta && !planoViagem &&
                tipoImpressaoPrestacao == TipoImpressaoPedidoPrestacaoServico.PrestacaoComViaCliente)
                ImpressaoOrdemColetaComViaCliente(dadosPedido, dadosImportacaoPrestacaoServico,
                    dadosMotoristasPrestacaoServico, dadosONUPrestacaoServico, dadosProdutosPrestacaoServico);

            if (gerarPorThread)
                Task.Factory.StartNew(() => GerarRelatorioPedido(caminhoPagina, nomeCliente, stringConexao,
                    relatorioControleGeracao, dadosPedido, dadosImportacaoPrestacaoServico, dadosMotoristasPrestacaoServico,
                    dadosONUPrestacaoServico, dadosProdutosPrestacaoServico, ordemServicoOrdemColeta,
                    dadosOrdemColetaSimplificada, dadosOrdemColetaSimplificadaMotorista, ordemColeta, planoViagem,
                    tipoImpressaoPedido, tipoImpressaoPrestacao, _unitOfWork));
            else
                GerarRelatorioPedido(caminhoPagina, nomeCliente, stringConexao, relatorioControleGeracao, dadosPedido,
                    dadosImportacaoPrestacaoServico, dadosMotoristasPrestacaoServico, dadosONUPrestacaoServico,
                    dadosProdutosPrestacaoServico, ordemServicoOrdemColeta, dadosOrdemColetaSimplificada,
                    dadosOrdemColetaSimplificadaMotorista, ordemColeta, planoViagem, tipoImpressaoPedido,
                    tipoImpressaoPrestacao, out reportRetorno, out guidRelatorio, _unitOfWork);
            return PrepareReportResult(FileType.PDF, guidRelatorio);
        }

        if (guidRelatorio != null)
        {
            return PrepareReportResult(FileType.PDF, guidRelatorio);
        }
        else
        {
            return PrepareReportResult(FileType.PDF);
        }

    }


    #region Métodos Privados

    private void ImpressaoOrdemColetaComViaCliente(
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> dadosPedido,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico>
            dadosImportacaoPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico>
            dadosMotoristasPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> dadosONUPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> dadosProdutosPrestacaoServico)
    {
        List<int> codigos = new List<int>();
        for (int i = 0; i < dadosPedido.Count; i++)
            codigos.Add(dadosPedido[i].CodigoPedido);

        for (int k = 0; k < dadosPedido.Count; k++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico dadoPedido = dadosPedido[k];
            for (int i = 0; i < codigos.Count; i++)
            {
                if (dadoPedido.CodigoPedido == codigos[i])
                {
                    string veiculos = string.IsNullOrWhiteSpace(dadoPedido.VeiculosPedidos)
                        ? dadoPedido.VeiculosCarga
                        : dadoPedido.VeiculosPedidos;
                    dadosPedido[k].Veiculos = veiculos;

                    string numeroFrotaVeiculos = string.IsNullOrWhiteSpace(dadoPedido.NumerosFrotaVeiculosPedido)
                        ? dadoPedido.NumerosFrotaVeiculosCarga
                        : dadoPedido.NumerosFrotaVeiculosPedido;
                    dadosPedido[k].NumerosFrotaVeiculos = numeroFrotaVeiculos;

                    dadosPedido.Add(new Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico()
                    {
                        Cotacao = dadoPedido.Cotacao,
                        ValorFreteCotado = dadoPedido.ValorFreteCotado,
                        CNPJEmpresa = dadoPedido.CNPJEmpresa,
                        RazaoEmpresa = dadoPedido.RazaoEmpresa,
                        CEPEmpresa = dadoPedido.CEPEmpresa,
                        RuaEmpresa = dadoPedido.RuaEmpresa,
                        BairroEmpresa = dadoPedido.BairroEmpresa,
                        NumeroEmpresa = dadoPedido.NumeroEmpresa,
                        ComplementoEmpresa = dadoPedido.ComplementoEmpresa,
                        IEEmpresa = dadoPedido.IEEmpresa,
                        FoneEmpresa = dadoPedido.FoneEmpresa,
                        CidadeEmpresa = dadoPedido.CidadeEmpresa,
                        EstadoEmpresa = dadoPedido.EstadoEmpresa,
                        Motoristas = dadoPedido.Motoristas,
                        Veiculos = veiculos,
                        Produtos = dadoPedido.Produtos,
                        Modelo = dadoPedido.Modelo,
                        NumerosFrotaVeiculos = numeroFrotaVeiculos,
                        LimitePesoModeloVeicular = dadoPedido.LimitePesoModeloVeicular,
                        Ajudantes = dadoPedido.Ajudantes,
                        CnpjCpfRemetente = dadoPedido.CnpjCpfRemetente,
                        TipoRemetente = dadoPedido.TipoRemetente,
                        NomeRemetente = dadoPedido.NomeRemetente,
                        EnderecoRemetente = dadoPedido.EnderecoRemetente,
                        CidadeRemetente = dadoPedido.CidadeRemetente,
                        EstadoRemetente = dadoPedido.EstadoRemetente,
                        CidadeOrigemOriginal = dadoPedido.CidadeOrigemOriginal,
                        EstadoOrigemOriginal = dadoPedido.EstadoOrigemOriginal,
                        EnderecoOrigem = dadoPedido.EnderecoOrigem,
                        BairroOrigem = dadoPedido.BairroOrigem,
                        CEPOrigem = dadoPedido.CEPOrigem,
                        ComplementoOrigem = dadoPedido.ComplementoOrigem,
                        NumeroOrigem = dadoPedido.NumeroOrigem,
                        FoneOrigem = dadoPedido.FoneOrigem,
                        CidadeOrigem = dadoPedido.CidadeOrigem,
                        EstadoOrigem = dadoPedido.EstadoOrigem,

                        CnpjCpfDestinatario = dadoPedido.CnpjCpfDestinatario,
                        TipoDestinatario = dadoPedido.TipoDestinatario,
                        NomeDestinatario = dadoPedido.NomeDestinatario,
                        EnderecoDestino = dadoPedido.EnderecoDestino,
                        BairroDestino = dadoPedido.BairroDestino,
                        CEPDestino = dadoPedido.CEPDestino,
                        ComplementoDestino = dadoPedido.ComplementoDestino,
                        NumeroDestino = dadoPedido.NumeroDestino,
                        FoneDestino = dadoPedido.FoneDestino,
                        CidadeDestino = dadoPedido.CidadeDestino,
                        EstadoDestino = dadoPedido.EstadoDestino,

                        CnpjCpfRemetenteParticipante = dadoPedido.CnpjCpfRemetenteParticipante,
                        TipoRemetenteParticipante = dadoPedido.TipoRemetenteParticipante,
                        NomeRemetenteParticipante = dadoPedido.NomeRemetenteParticipante,
                        EnderecoRemetenteParticipante = dadoPedido.EnderecoRemetenteParticipante,
                        BairroRemetenteParticipante = dadoPedido.BairroRemetenteParticipante,
                        CEPRemetenteParticipante = dadoPedido.CEPRemetenteParticipante,
                        ComplementoRemetenteParticipante = dadoPedido.ComplementoRemetenteParticipante,
                        NumeroRemetenteParticipante = dadoPedido.NumeroRemetenteParticipante,
                        FoneRemetenteParticipante = dadoPedido.FoneRemetenteParticipante,
                        CidadeRemetenteParticipante = dadoPedido.CidadeRemetenteParticipante,
                        EstadoRemetenteParticipante = dadoPedido.EstadoRemetenteParticipante,

                        CnpjCpfDestinatarioParticipante = dadoPedido.CnpjCpfDestinatarioParticipante,
                        TipoDestinatarioParticipante = dadoPedido.TipoDestinatarioParticipante,
                        NomeDestinatarioParticipante = dadoPedido.NomeDestinatarioParticipante,
                        EnderecoDestinatarioParticipante = dadoPedido.EnderecoDestinatarioParticipante,
                        BairroDestinatarioParticipante = dadoPedido.BairroDestinatarioParticipante,
                        CEPDestinatarioParticipante = dadoPedido.CEPDestinatarioParticipante,
                        ComplementoDestinatarioParticipante = dadoPedido.ComplementoDestinatarioParticipante,
                        NumeroDestinatarioParticipante = dadoPedido.NumeroDestinatarioParticipante,
                        FoneDestinatarioParticipante = dadoPedido.FoneDestinatarioParticipante,
                        CidadeDestinatarioParticipante = dadoPedido.CidadeDestinatarioParticipante,
                        EstadoDestinatarioParticipante = dadoPedido.EstadoDestinatarioParticipante,

                        EscoltaArmada = dadoPedido.EscoltaArmada,
                        EscoltaMunicipal = dadoPedido.EscoltaMunicipal,
                        Ajudante = dadoPedido.Ajudante,
                        TipoCarga = dadoPedido.TipoCarga,
                        TipoOperacao = dadoPedido.TipoOperacao,
                        Observacao = dadoPedido.Observacao,
                        ObservacaoCTe = dadoPedido.ObservacaoCTe,
                        QtdEntregas = dadoPedido.QtdEntregas,
                        QtdVolumes = dadoPedido.QtdVolumes,
                        PesoCarga = dadoPedido.PesoCarga,
                        PesoPaletes = dadoPedido.PesoPaletes,
                        QtdPaletes = dadoPedido.QtdPaletes,
                        Usuario = dadoPedido.Usuario,
                        Operador = dadoPedido.Operador,
                        DataCarregamento = dadoPedido.DataCarregamento,
                        DataPrevisaoFim = dadoPedido.DataPrevisaoFim,
                        DataPrevisaoInicio = dadoPedido.DataPrevisaoInicio,
                        DataPrevisaoSaida = dadoPedido.DataPrevisaoSaida,
                        Numero = Utilidades.String.OnlyNumbers(dadoPedido.Numero),
                        NumeroCliente = dadoPedido.NumeroCliente,
                        CodigoPedido = dadoPedido.CodigoPedido * -1,
                        ContemProduto = dadoPedido.ContemProduto,
                        ContemONUProduto = dadoPedido.ContemONUProduto,
                        ContemMotorista = dadoPedido.ContemMotorista,
                        ContemImportacao = dadoPedido.ContemImportacao,
                        NumeroCarga = !string.IsNullOrWhiteSpace(dadoPedido.NumeroCarga)
                            ? dadoPedido.NumeroCarga.Replace("VIA CLIENTE", "") + " VIA OPERACIONAL"
                            : "VIA OPERACIONAL",
                        DataCarga = dadoPedido.DataCarga,
                        TipoPagamento = dadoPedido.TipoPagamento,
                        TipoTomador = dadoPedido.TipoTomador,
                        NomeOutroTomador = dadoPedido.NomeOutroTomador,
                        EnderedoOutroTomador = dadoPedido.EnderedoOutroTomador,
                        CidadeOutroTomador = dadoPedido.CidadeOutroTomador,
                        EstadoOutroTomador = dadoPedido.EstadoOutroTomador,
                        BairroOutroTomador = dadoPedido.BairroOutroTomador,
                        CEPOutroTomador = dadoPedido.CEPOutroTomador,
                        ComplementoOutroTomador = dadoPedido.ComplementoOutroTomador,
                        NumeroOutroTomador = dadoPedido.NumeroOutroTomador,
                        FoneOutroTomador = dadoPedido.FoneOutroTomador,
                        ObservacaoGeral = dadoPedido.ObservacaoGeral,
                        CodigoPedidoCliente = dadoPedido.CodigoPedidoCliente,
                        DataColeta = dadoPedido.DataColeta,

                        NomeLocalPaletizacao = dadoPedido.NomeLocalPaletizacao,
                        EnderecoLocalPaletizacao = dadoPedido.EnderecoLocalPaletizacao,
                        BairroLocalPaletizacao = dadoPedido.BairroLocalPaletizacao,
                        CEPLocalPaletizacao = dadoPedido.CEPLocalPaletizacao,
                        ComplementoLocalPaletizacao = dadoPedido.ComplementoLocalPaletizacao,
                        NumeroLocalPaletizacao = dadoPedido.NumeroLocalPaletizacao,
                        FoneLocalPaletizacao = dadoPedido.FoneLocalPaletizacao,
                        CidadeLocalPaletizacao = dadoPedido.CidadeLocalPaletizacao,
                        EstadoLocalPaletizacao = dadoPedido.EstadoLocalPaletizacao,
                    });
                }
            }
        }

        for (int k = 0; k < dadosImportacaoPrestacaoServico.Count; k++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico dadoImportacaoPrestacaoServico =
                dadosImportacaoPrestacaoServico[k];
            for (int i = 0; i < codigos.Count; i++)
            {
                if (dadoImportacaoPrestacaoServico.CodigoPedido == codigos[i])
                {
                    dadosImportacaoPrestacaoServico.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico()
                        {
                            NumeroDI = dadoImportacaoPrestacaoServico.NumeroDI,
                            CodigoImportacao = dadoImportacaoPrestacaoServico.CodigoImportacao,
                            PontoReferencia = dadoImportacaoPrestacaoServico.PontoReferencia,
                            ValorCarga = dadoImportacaoPrestacaoServico.ValorCarga,
                            Volume = dadoImportacaoPrestacaoServico.Volume,
                            Peso = dadoImportacaoPrestacaoServico.Peso,
                            CodigoPedido = dadoImportacaoPrestacaoServico.CodigoPedido * -1,
                            NumeroContainer = dadoImportacaoPrestacaoServico.NumeroContainer,
                            NumeroBL = dadoImportacaoPrestacaoServico.NumeroBL,
                            NumeroNavio = dadoImportacaoPrestacaoServico.NumeroNavio,
                            CNPJPorto = dadoImportacaoPrestacaoServico.CNPJPorto,
                            NomePorto = dadoImportacaoPrestacaoServico.NomePorto,
                            EnderecoEntrega = dadoImportacaoPrestacaoServico.EnderecoEntrega,
                            BairroEntrega = dadoImportacaoPrestacaoServico.BairroEntrega,
                            CEPEntrega = dadoImportacaoPrestacaoServico.CEPEntrega,
                            PontoReferenciaEntrega = dadoImportacaoPrestacaoServico.PontoReferenciaEntrega,
                            CodigoCidade = dadoImportacaoPrestacaoServico.CodigoCidade,
                            Cidade = dadoImportacaoPrestacaoServico.Cidade,
                            Estado = dadoImportacaoPrestacaoServico.Estado,
                            DataVencimento = dadoImportacaoPrestacaoServico.DataVencimento,
                            Armador = dadoImportacaoPrestacaoServico.Armador,
                            CodigoTerminal = dadoImportacaoPrestacaoServico.CodigoTerminal,
                            Terminal = dadoImportacaoPrestacaoServico.Terminal,
                        });
                }
            }
        }

        for (int k = 0; k < dadosMotoristasPrestacaoServico.Count; k++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico dadoMotoristasPrestacaoServico =
                dadosMotoristasPrestacaoServico[k];
            for (int i = 0; i < codigos.Count; i++)
            {
                if (dadoMotoristasPrestacaoServico.CodigoPedido == codigos[i])
                {
                    if (dadoMotoristasPrestacaoServico.UtilizaInformacoesAdicionionais)
                    {
                        dadosMotoristasPrestacaoServico.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico()
                        {
                            CPF = dadoMotoristasPrestacaoServico.CPF,
                            Nome = dadoMotoristasPrestacaoServico.Nome,
                            CodigoPedido = dadoMotoristasPrestacaoServico.CodigoPedido * -1,
                            CNH = dadoMotoristasPrestacaoServico.CNH,
                            RegistroCNH = dadoMotoristasPrestacaoServico.RegistroCNH,
                            CategoriaCNH = dadoMotoristasPrestacaoServico.CategoriaCNH,
                            EmissaoCNH = dadoMotoristasPrestacaoServico.EmissaoCNH,
                            ValidadeCNH = dadoMotoristasPrestacaoServico.ValidadeCNH,
                            PrimeiraCNH = dadoMotoristasPrestacaoServico.PrimeiraCNH,
                            Telefone = dadoMotoristasPrestacaoServico.Telefone,
                            UtilizaInformacoesAdicionionais = dadoMotoristasPrestacaoServico.UtilizaInformacoesAdicionionais
                        });
                    }
                    else
                    {
                        dadosMotoristasPrestacaoServico.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico()
                        {
                            CPF = dadoMotoristasPrestacaoServico.CPF,
                            Nome = dadoMotoristasPrestacaoServico.Nome,
                            CodigoPedido = dadoMotoristasPrestacaoServico.CodigoPedido * -1,
                            CNH = string.Empty,
                            RegistroCNH = string.Empty,
                            CategoriaCNH = string.Empty,
                            EmissaoCNH = DateTime.MinValue,
                            ValidadeCNH = DateTime.MinValue,
                            PrimeiraCNH = DateTime.MinValue,
                            Telefone = string.Empty
                        });
                    }

                }
            }
        }

        for (int k = 0; k < dadosONUPrestacaoServico.Count; k++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico dadoONUPrestacaoServico =
                dadosONUPrestacaoServico[k];
            for (int i = 0; i < codigos.Count; i++)
            {
                if (dadoONUPrestacaoServico.CodigoPedido == codigos[i])
                {
                    dadosONUPrestacaoServico.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico()
                        {
                            Observacao = dadoONUPrestacaoServico.Observacao,
                            NumeroONU = dadoONUPrestacaoServico.NumeroONU,
                            ClasseRisco = dadoONUPrestacaoServico.ClasseRisco,
                            RiscoSubsiriario = dadoONUPrestacaoServico.RiscoSubsiriario,
                            NumeroRisco = dadoONUPrestacaoServico.NumeroRisco,
                            GrupoEmbarcado = dadoONUPrestacaoServico.GrupoEmbarcado,
                            ProvisoesEspeciais = dadoONUPrestacaoServico.ProvisoesEspeciais,
                            LimiteKG = dadoONUPrestacaoServico.LimiteKG,
                            LimiteLT = dadoONUPrestacaoServico.LimiteLT,
                            InstrucaoEmbalagem = dadoONUPrestacaoServico.InstrucaoEmbalagem,
                            ProvisoesEmbalagem = dadoONUPrestacaoServico.ProvisoesEmbalagem,
                            InstrucaoTanque = dadoONUPrestacaoServico.InstrucaoTanque,
                            ProvisoesTanque = dadoONUPrestacaoServico.ProvisoesTanque,
                            CodigoPedido = dadoONUPrestacaoServico.CodigoPedido * -1
                        });
                }
            }
        }

        for (int k = 0; k < dadosProdutosPrestacaoServico.Count; k++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico dadoProdutosPrestacaoServico =
                dadosProdutosPrestacaoServico[k];
            for (int i = 0; i < codigos.Count; i++)
            {
                if (dadoProdutosPrestacaoServico.CodigoPedido == codigos[i])
                {
                    dadosProdutosPrestacaoServico.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico()
                        {
                            Descricao = dadoProdutosPrestacaoServico.Observacao,
                            Peso = dadoProdutosPrestacaoServico.Peso,
                            Palet = dadoProdutosPrestacaoServico.Palet,
                            Quantidade = dadoProdutosPrestacaoServico.Quantidade,
                            Altura = dadoProdutosPrestacaoServico.Altura,
                            Largura = dadoProdutosPrestacaoServico.Largura,
                            Comprimento = dadoProdutosPrestacaoServico.Comprimento,
                            MetroCubico = dadoProdutosPrestacaoServico.MetroCubico,
                            Observacao = dadoProdutosPrestacaoServico.Observacao,
                            CodigoPedido = dadoProdutosPrestacaoServico.CodigoPedido * -1
                        });
                }
            }
        }
    }

    private bool ImpressaoOrdemColetaExclusiva(out string mensagem, string caminhoPagina, int codigoGrupoPessoas,
        TipoImpressaoOrdemColetaExclusiva tipoImpressaoOrdemColetaExclusiva, int codigoPedido, string codigosPedidos,
        bool impressaoCarga, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno, bool gerarPorThread,
        out string guidRelatorio)
    {
        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

        mensagem = null;
        reportRetorno = null;
        guidRelatorio = "";

        if (tipoImpressaoOrdemColetaExclusiva == TipoImpressaoOrdemColetaExclusiva.Paisagem)
        {
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem>
                dadosOrdemColetaExclusiva =
                    repPedido.RelatorioOrdemColetaExclusivaPaisagemPedido(codigoPedido, codigosPedidos, impressaoCarga);
            if (dadosOrdemColetaExclusiva.Count == 0)
            {
                mensagem = "Nenhum registro de pedido para gerar o relatório.";
                return false;
            }

            if (gerarPorThread)
                Task.Factory.StartNew(() => GerarOrdemColetaExclusivaPaisagem(caminhoPagina, codigoGrupoPessoas,
                    stringConexao, relatorioControleGeracao, dadosOrdemColetaExclusiva, _unitOfWork));
            else
                GerarOrdemColetaExclusivaPaisagem(caminhoPagina, codigoGrupoPessoas, stringConexao,
                    relatorioControleGeracao, dadosOrdemColetaExclusiva, out reportRetorno, out guidRelatorio,
                    _unitOfWork);
        }
        else
        {
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva> dadosOrdemColetaExclusiva =
                repPedido.RelatorioOrdemColetaExclusivaPedido(codigoPedido, codigosPedidos, impressaoCarga);
            if (dadosOrdemColetaExclusiva.Count == 0)
            {
                mensagem = "Nenhum registro de pedido para gerar o relatório.";
                return false;
            }

            if (gerarPorThread)
                Task.Factory.StartNew(() => GerarOrdemColetaExclusiva(caminhoPagina, codigoGrupoPessoas, stringConexao,
                    relatorioControleGeracao, dadosOrdemColetaExclusiva, _unitOfWork));
            else
                GerarOrdemColetaExclusiva(caminhoPagina, codigoGrupoPessoas, stringConexao, relatorioControleGeracao,
                    dadosOrdemColetaExclusiva, out reportRetorno, out guidRelatorio, _unitOfWork);
        }

        return true;
    }

    private string ObterCaminhoPaginaRelatorioTMS(bool planejamentoPedido, bool impressaoCarga, bool carregamento)
    {
        string caminhoPagina;

        if (planejamentoPedido)
            caminhoPagina = "Pedidos/PlanejamentoPedidoTMS";
        else if (impressaoCarga)
            caminhoPagina = "Cargas/Carga";
        else if (carregamento)
            caminhoPagina = "Cargas/MontagemCarga";
        else
            caminhoPagina = "Pedidos/Pedido";

        return caminhoPagina;
    }

    private void GerarRelatorioPedido(string caminhoPagina, string nomeEmpresa, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> dadosPedido,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico>
            dadosImportacaoPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico>
            dadosMotoristasPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> dadosONUPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> dadosProdutosPrestacaoServico,
        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta ordemServicoOrdemColeta,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada> dadosOrdemColetaSimplificada,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista>
            dadosOrdemColetaSimplificadaMotorista, bool ordemColeta, bool planoViagem,
        TipoImpressaoPedido tipoImpressaoPedido, TipoImpressaoPedidoPrestacaoServico tipoImpressaoPedidoPrestacao,
        Repositorio.UnitOfWork unitOfWork)
    {
        GerarRelatorioPedido(caminhoPagina, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedido,
            dadosImportacaoPrestacaoServico, dadosMotoristasPrestacaoServico, dadosONUPrestacaoServico,
            dadosProdutosPrestacaoServico, ordemServicoOrdemColeta, dadosOrdemColetaSimplificada,
            dadosOrdemColetaSimplificadaMotorista, ordemColeta, planoViagem, tipoImpressaoPedido,
            tipoImpressaoPedidoPrestacao, out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno,
            out string guidRelatorio, unitOfWork);
    }

    private void GerarRelatorioPedido(string caminhoPagina, string nomeEmpresa, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> dadosPedido,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico>
            dadosImportacaoPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico>
            dadosMotoristasPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> dadosONUPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> dadosProdutosPrestacaoServico,
        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta ordemServicoOrdemColeta,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada> dadosOrdemColetaSimplificada,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista>
            dadosOrdemColetaSimplificadaMotorista, bool ordemColeta, bool planoViagem,
        TipoImpressaoPedido tipoImpressaoPedido, TipoImpressaoPedidoPrestacaoServico tipoImpressaoPedidoPrestacao,
        out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno, out string guidRelatorio,
        Repositorio.UnitOfWork unitOfWork)
    {
        reportRetorno = null;
        guidRelatorio = "";
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
            new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

        //try
        //{
        string mensagem = "";
        CrystalDecisions.CrystalReports.Engine.ReportDocument report;

        if (!planoViagem && ordemColeta && (tipoImpressaoPedido == TipoImpressaoPedido.OrdemServicoOrdemColeta ||
                                            tipoImpressaoPedido == TipoImpressaoPedido.Simplificada))
            report = GerarRelatorioPedidoTiposExclusivos(out mensagem, tipoImpressaoPedido, ordemServicoOrdemColeta,
                dadosOrdemColetaSimplificada, dadosOrdemColetaSimplificadaMotorista);
        else
            report = GerarRelatorioPedido(nomeEmpresa, dadosPedido, dadosImportacaoPrestacaoServico,
                dadosMotoristasPrestacaoServico, dadosONUPrestacaoServico, dadosProdutosPrestacaoServico,
                ordemServicoOrdemColeta, out mensagem, ordemColeta, planoViagem, tipoImpressaoPedido,
                tipoImpressaoPedidoPrestacao);

        if (relatorioControleGeracao == null || report == null)
            return;

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            guidRelatorio = relatorioControleGeracao.GuidArquivo;
            reportRetorno = report;
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, caminhoPagina, unitOfWork);
        }
        else
            serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, mensagem);
        //}
        //catch (Exception ex)
        //{
        //    if (relatorioControleGeracao != null)
        //        serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
        //}
    }

    private void GerarOrdemColetaExclusiva(string caminhoPagina, int codigoGrupoPessoas, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva> dadosOrdemColetaExclusiva,
        Repositorio.UnitOfWork unitOfWork)
    {
        GerarOrdemColetaExclusiva(caminhoPagina, codigoGrupoPessoas, stringConexao, relatorioControleGeracao,
            dadosOrdemColetaExclusiva, out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno,
            out string guidRelatorio, unitOfWork);
    }

    private void GerarOrdemColetaExclusiva(string caminhoPagina, int codigoGrupoPessoas, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva> dadosOrdemColetaExclusiva,
        out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno, out string guidRelatorio,
        Repositorio.UnitOfWork unitOfWork)
    {
        //Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

        reportRetorno = null;
        guidRelatorio = "";
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
            new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

        //try
        //{
        string mensagem = "";

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarOrdemColetaExclusiva(dadosOrdemColetaExclusiva, codigoGrupoPessoas);
        if (relatorioControleGeracao == null)
            return;

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            guidRelatorio = relatorioControleGeracao.GuidArquivo;
            reportRetorno = report;
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, caminhoPagina, unitOfWork);
        }
        else
            serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, mensagem);
        //}
        //catch (Exception ex)
        //{
        //    if (relatorioControleGeracao != null)
        //        serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
        //}
        //finally
        //{
        //    unitOfWork.Dispose();
        //}
    }

    private void GerarOrdemColetaExclusivaPaisagem(string caminhoPagina, int codigoGrupoPessoas, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem> dadosOrdemColetaExclusiva,
        Repositorio.UnitOfWork unitOfWork)
    {
        GerarOrdemColetaExclusivaPaisagem(caminhoPagina, codigoGrupoPessoas, stringConexao, relatorioControleGeracao,
            dadosOrdemColetaExclusiva, out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno,
            out string guidRelatorio, unitOfWork);
    }

    private void GerarOrdemColetaExclusivaPaisagem(string caminhoPagina, int codigoGrupoPessoas, string stringConexao,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem> dadosOrdemColetaExclusiva,
        out CrystalDecisions.CrystalReports.Engine.ReportDocument reportRetorno, out string guidRelatorio,
        Repositorio.UnitOfWork unitOfWork)
    {
        //Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

        reportRetorno = null;
        guidRelatorio = "";
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
            new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

        //try
        //{
        string mensagem = "";

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarOrdemColetaExclusivaPaisagem(dadosOrdemColetaExclusiva, codigoGrupoPessoas);
        if (relatorioControleGeracao == null)
            return;

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            guidRelatorio = relatorioControleGeracao.GuidArquivo;
            reportRetorno = report;
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, caminhoPagina, unitOfWork);
        }
        else
            serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, mensagem);
        //}
        //catch (Exception ex)
        //{
        //    if (relatorioControleGeracao != null)
        //        serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
        //}
        //finally
        //{
        //    unitOfWork.Dispose();
        //}
    }

    private CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioPedido(string nomeEmpresa,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> dadosPedido,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico>
            dadosImportacaoPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico>
            dadosMotoristasPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> dadosONUPrestacaoServico,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> dadosProdutosPrestacaoServico,
        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta ordemServicoOrdemColeta,
        out string mensagemErro, bool ordemColeta, bool planoViagem, TipoImpressaoPedido tipoImpressao,
        TipoImpressaoPedidoPrestacaoServico tipoImpressaoPrestacao)
    {
        mensagemErro = null;

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        if (dadosImportacaoPrestacaoServico.Count == 0)
        {
            dadosImportacaoPrestacaoServico.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico()
                {
                    CodigoPedido = 1
                });
        }

        if (dadosMotoristasPrestacaoServico.Count == 0)
        {
            dadosMotoristasPrestacaoServico.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico()
                {
                    CodigoPedido = 1
                });
        }

        if (dadosONUPrestacaoServico.Count == 0)
        {
            dadosONUPrestacaoServico.Add(new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico()
            {
                CodigoPedido = 1
            });
        }

        if (dadosProdutosPrestacaoServico.Count == 0)
        {
            dadosProdutosPrestacaoServico.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico()
                {
                    CodigoPedido = 1
                });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PedidoPrestacaoServicoImportacao.rpt",
                DataSet = dadosImportacaoPrestacaoServico
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PedidoPrestacaoServicoMotorista.rpt",
                DataSet = dadosMotoristasPrestacaoServico
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PedidoPrestacaoServicoONU.rpt",
                DataSet = dadosONUPrestacaoServico
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds4 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PedidoPrestacaoServicoProduto.rpt",
                DataSet = dadosProdutosPrestacaoServico
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);
        subReports.Add(ds4);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosPedido,
                Parameters = parametros,
                SubReports = subReports
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report;
        if (planoViagem)
            report = RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Pedidos\PlanoViagemTMS.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        else if (ordemColeta)
        {
            if (tipoImpressao == TipoImpressaoPedido.AutorizacaoEntrega)
                report = RelatorioSemPadraoReportService.GerarCrystalReport(
                    @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaTMSEntrega.rpt",
                    Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            else if (tipoImpressao == TipoImpressaoPedido.AutorizacaoCarregamento ||
                     tipoImpressao == TipoImpressaoPedido.AutorizacaoCarregamento_v2)
                report = RelatorioSemPadraoReportService.GerarCrystalReport(
                    @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaTMS.rpt",
                    Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            else
                report = null;
        }
        else
        {
            if (tipoImpressaoPrestacao == TipoImpressaoPedidoPrestacaoServico.PrestacaoSemViaCliente)
                report = RelatorioSemPadraoReportService.GerarCrystalReport(
                    @"Areas\Relatorios\Reports\Default\Pedidos\PedidoPrestacaoServicoSemViaCliente.rpt",
                    Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            else
            {
                if (tipoImpressao == TipoImpressaoPedido.AutorizacaoCarregamento_v2)
                    report = RelatorioSemPadraoReportService.GerarCrystalReport(
                        @"Areas\Relatorios\Reports\Default\Pedidos\PedidoPrestacaoServicoV2.rpt",
                        Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
                else
                    report = RelatorioSemPadraoReportService.GerarCrystalReport(
                        @"Areas\Relatorios\Reports\Default\Pedidos\PedidoPrestacaoServico.rpt",
                        Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            }
        }

        return report;
    }

    private CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioPedidoTiposExclusivos(
        out string mensagemErro, TipoImpressaoPedido tipoImpressao,
        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta ordemServicoOrdemColeta,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada> dadosOrdemColetaSimplificada,
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista>
            dadosOrdemColetaSimplificadaMotorista)
    {
        mensagemErro = null;

        if (tipoImpressao == TipoImpressaoPedido.OrdemServicoOrdemColeta)
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta>()
                        { ordemServicoOrdemColeta },
                    Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                };

            return RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaOrdemServico.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        }
        else
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "Motorista",
                    DataSet = dadosOrdemColetaSimplificadaMotorista
                };

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
                new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
            subReports.Add(ds1);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    DataSet = dadosOrdemColetaSimplificada,
                    SubReports = subReports
                };

            return RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaSimplificada.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        }
    }

    private CrystalDecisions.CrystalReports.Engine.ReportDocument GerarOrdemColetaExclusiva(
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva> dadosOrdemColetaExclusiva,
        int codigoGrupoPessoas)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        string caminhoLogoGrupoPessoas = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Logo", "GrupoPessoas" });
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro logoGrupoPessoas =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        logoGrupoPessoas.NomeParametro = "CaminhoLogoGrupoPessoas";
        logoGrupoPessoas.ValorParametro =
            Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoLogoGrupoPessoas, $"{codigoGrupoPessoas}.*").FirstOrDefault() ??
            string.Empty;
        parametros.Add(logoGrupoPessoas);

        int codigoMotorista = dadosOrdemColetaExclusiva.FirstOrDefault().CodigoMotorista;
        string caminhoFotoMotorista = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro fotoMotorista =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        fotoMotorista.NomeParametro = "CaminhoFotoMotorista";
        fotoMotorista.ValorParametro =
            Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoFotoMotorista, $"{codigoMotorista}.*").FirstOrDefault() ?? string.Empty;
        parametros.Add(fotoMotorista);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosOrdemColetaExclusiva,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaExclusiva.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }

    private CrystalDecisions.CrystalReports.Engine.ReportDocument GerarOrdemColetaExclusivaPaisagem(
        IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem> dadosOrdemColetaExclusiva,
        int codigoGrupoPessoas)
    {
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosOrdemColetaExclusiva
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColetaExclusivaPaisagem.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }

    private Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta
        ObterDetalhesOrdemServicoOrdemColeta(int codigoPedido, int codigoCarga)
    {
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista =
            new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS =
            repConfiguracaoTMS.BuscarConfiguracaoPadrao();

        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido =
            codigoPedido > 0 ? repPedido.BuscarPorCodigo(codigoPedido) : null;
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

        if (pedido == null && carga != null)
            pedido = carga.Pedidos != null && carga.Pedidos.Count > 0 ? carga.Pedidos.FirstOrDefault().Pedido : null;

        Dominio.Entidades.Veiculo veiculo = pedido == null ? null :
            pedido.VeiculoTracao != null ? pedido.VeiculoTracao :
            pedido.Veiculos != null && pedido.Veiculos.Count > 0 ? pedido.Veiculos.FirstOrDefault() : null;
        if (carga != null && carga.Veiculo != null)
            veiculo = carga.Veiculo;

        Dominio.Entidades.Cliente proprietario =
            veiculo != null && veiculo.Proprietario != null ? veiculo.Proprietario : null;
        Dominio.Entidades.Usuario motorista = carga != null && carga.Motoristas != null && carga.Motoristas.Count > 0
            ?
            carga.Motoristas.FirstOrDefault()
            : veiculo != null && repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo) != null
                ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo)
                : null;

        Dominio.Entidades.Cliente entrega = pedido.Recebedor ?? pedido.Destinatario ?? null;
        Dominio.Entidades.Cliente coleta = pedido.Expedidor ?? pedido.Remetente ?? null;

        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta dataSet =
            new Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemServicoOrdemColeta();

        if (pedido != null)
        {
            dataSet.CabEndereco = $"{pedido.Empresa?.Endereco ?? ""} - {pedido.Empresa?.CEP ?? ""}";
            dataSet.CabLocalidade = pedido.Empresa?.LocalidadeUF ?? "";
            dataSet.CabCNPJ_IE = $"{pedido.Empresa?.CNPJ_Formatado ?? ""} - {pedido.Empresa?.InscricaoEstadual ?? ""}";
            dataSet.CabTelefone = pedido.Empresa?.Telefone ?? "";
            dataSet.CabNumeroPedido = pedido.Numero.ToString() ?? "";
            dataSet.CabTipoOperacao = pedido.TipoOperacao?.Descricao ?? "";
            dataSet.CabDataEmissaoCarga = pedido.DataCriacao?.ToString("dd/MM/yyyy HH:mm") ?? "";

            dataSet.CabPedidoNumeroCarga = carga != null ? carga.CodigoCargaEmbarcador : pedido.Numero.ToString() ?? "";
            dataSet.CabPedidoOperacao = pedido.TipoOperacao?.Descricao ?? "";
            dataSet.CabPedidoMercadoria = pedido.ProdutoPredominante ?? "";
            dataSet.CabPedidoDestino = coleta?.Localidade?.Descricao ??
                                       (pedido.Destinatario?.Localidade?.DescricaoCidadeEstado ?? "");
            dataSet.CabPedidoCNPJ_Empresa_Filial = pedido.Empresa?.CNPJ_Formatado ?? "";
            dataSet.CabPedidoPeso =
                carga != null && carga.DadosSumarizados != null && carga.DadosSumarizados.PesoTotal > 0
                    ? carga.DadosSumarizados.PesoTotal.ToString("n2")
                    : (pedido.PesoTotal.ToString("n2") ?? "");
            dataSet.CabPedidoColeta = pedido.Expedidor != null ? pedido.Expedidor.Descricao :
                pedido.Remetente != null ? pedido.Remetente.Descricao : "";
            dataSet.CabPedidoTipoCarga = pedido.TipoDeCarga != null ? pedido.TipoDeCarga.Descricao : "";

            // Contratante
            dataSet.ContRazaoSocial = pedido.Empresa?.RazaoSocial ?? "";
            dataSet.ContEndereco = pedido.Empresa?.Endereco ?? "";
            dataSet.ContBairro = pedido.Empresa?.Bairro ?? "";
            dataSet.ContMunicipio = pedido.Empresa?.Localidade?.Descricao ?? "";
            dataSet.ContTelefone = pedido.Empresa?.Telefone ?? "";
            dataSet.ContCEP = pedido.Empresa?.CEP ?? "";
            dataSet.ContUF = pedido.Empresa?.Localidade?.Estado?.Sigla ?? "";

            // Destinatario Carga
            dataSet.DestRazaoSocial = pedido.Destinatario?.Nome ?? ""; //pedido.Empresa?.RazaoSocial ?? "";
            dataSet.DestEndereco = pedido.Destinatario?.Endereco ?? ""; //pedido.Empresa?.Endereco ?? "";
            dataSet.DestBairro = pedido.Destinatario?.Bairro ?? ""; //pedido.Empresa?.Bairro ?? "";
            dataSet.DestMunicipio =
                pedido.Destinatario?.Localidade?.Descricao ?? ""; //pedido.Empresa?.Localidade?.Descricao ?? "";
            dataSet.DestTelefone = pedido.Destinatario?.Telefone1 ?? ""; //pedido.Empresa?.Telefone ?? "";
            dataSet.DestCEP = pedido.Destinatario?.CEP ?? ""; //pedido.Empresa?.CEP ?? "";
            dataSet.DestUF =
                pedido.Destinatario?.Localidade?.Estado?.Sigla ?? ""; //pedido.Empresa?.Localidade?.Estado?.Sigla ?? "";

            // Expedidor ou Emitente
            dataSet.ColRazaoSocial = coleta?.Nome ?? "";
            dataSet.ColEndereco = coleta?.Endereco ?? "";
            dataSet.ColBairro = coleta?.Bairro ?? "";
            dataSet.ColMunicipio = coleta?.Localidade?.Descricao ?? "";
            dataSet.ColTelefone = coleta?.Telefone1 ?? "";
            dataSet.ColCEP = coleta?.CEP ?? "";
            dataSet.ColUF = coleta?.Localidade?.Estado?.Sigla ?? "";

            // Recebedor ou Destinatário
            dataSet.EntRazaoSocial = entrega?.Nome ?? "";
            dataSet.EntEndereco = entrega?.Endereco ?? "";
            dataSet.EntBairro = entrega?.Bairro ?? "";
            dataSet.EntMunicipio = entrega?.Localidade?.Descricao ?? "";
            dataSet.EntTelefone = entrega?.Telefone1 ?? "";
            dataSet.EntCEP = entrega?.CEP ?? "";
            dataSet.EntUF = entrega?.Localidade?.Estado?.Sigla ?? "";

            dataSet.TraRazaoSocial = proprietario?.NomeCNPJ ?? "";
            dataSet.TraMotorista = motorista?.Nome ?? "";
            dataSet.TraPlacaCavalo = veiculo?.Placa ?? "";
            dataSet.TraRNTRCCavalo = veiculo?.RNTRC.ToString() ?? "";
            dataSet.TraPlacaCarreta =
                veiculo != null ? string.Join(", ", veiculo?.VeiculosVinculados.Select(o => o.Placa)) : "";
            dataSet.TraRNTRCCarreta =
                veiculo != null ? string.Join(", ", veiculo?.VeiculosVinculados.Select(o => o.RNTRC)) : "";
            dataSet.TraModeloCavalo = veiculo?.Modelo?.Descricao ?? "";
            dataSet.TraAnoCavalo = veiculo?.AnoFabricacao.ToString() ?? "";
            dataSet.TraChassiCarreta =
                veiculo != null ? string.Join(", ", veiculo?.VeiculosVinculados.Select(o => o.Chassi)) : "";
            dataSet.TraCNH = motorista?.NumeroHabilitacao ?? "";
            dataSet.TraUFCavalo = veiculo?.LocalidadeAtual?.Descricao ?? "";
            dataSet.TraChassiCavalo = veiculo?.Chassi ?? "";
            dataSet.TraRenavanCarreta = veiculo?.Renavam ?? "";
            dataSet.TraIdentidade = motorista?.RG ?? "";
            dataSet.TraCPF = motorista?.CPF_Formatado ?? "";
            dataSet.TraUFCarreta = veiculo != null
                ? string.Join(", ", veiculo?.VeiculosVinculados.Select(o => o.LocalidadeAtual?.Estado?.Sigla))
                : "";
            dataSet.TraRenavanCavalo = veiculo?.Renavam ?? "";

            dataSet.CarRota = pedido.RotaFrete?.Descricao ?? "";
            dataSet.CarTarifa = pedido.ValorFreteToneladaNegociado.ToString("n2") ?? "";
            dataSet.CarPercAdiantamento = configuracaoTMS.PercentualAdiantamentoTerceiroPadrao.ToString("n2") ?? "";
            dataSet.CarCartaoTransportador = motorista?.NumeroCartao ?? "";
            dataSet.CarPedagio = pedido.ValorPedagioRota > 0 ? "Sim" : "Não";
            dataSet.CarRazaoSocialEmpresaFilial = pedido.Empresa?.RazaoSocial ?? "";
        }

        return dataSet;
    }

    #endregion
}