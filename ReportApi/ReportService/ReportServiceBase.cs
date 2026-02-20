using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Servicos;
using Relatorio = Servicos.Embarcador.Relatorios.Relatorio;

namespace ReportApi.ReportService;

public class ReportServiceBase
{
    #region Atributos

    protected readonly Repositorio.UnitOfWork _unitOfWork;
    protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
    protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
    private readonly Servicos.Embarcador.Relatorios.RelatorioReportService _relatorioReportService;
    #endregion

    #region Construtores 

    public ReportServiceBase(Repositorio.UnitOfWork unitOfWork, Servicos.Embarcador.Relatorios.RelatorioReportService relatorioReportService)
    {
        _unitOfWork = unitOfWork;
        _tipoServicoMultisoftware = ConnectionFactory.TipoServicoMultisoftware;
        _clienteMultisoftware = ConnectionFactory.Cliente;
        _relatorioReportService = relatorioReportService;
    }

    #endregion

    #region Metodos Publicos 

    public void FinalizarGeracaoReport(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio parametrosGeracaoRelatorio, string errorMessage)
    {
        
        try
        {
            if(!string.IsNullOrWhiteSpace(errorMessage))
                throw new ServicoException($"A geração do relatório falhou com a seguinte mensagem {errorMessage}");            

            if (parametrosGeracaoRelatorio == null)
                throw new ServicoException($"Geração do Relatorio {relatorioControleGeracao.Relatorio.Titulo} não configurada!");

            CrystalDecisions.CrystalReports.Engine.ReportDocument report = _relatorioReportService.CriarRelatorio(relatorioControleGeracao, parametrosGeracaoRelatorio.ConfiguracaoRelatorio, parametrosGeracaoRelatorio.ListaRegistros, _unitOfWork, null, parametrosGeracaoRelatorio.SubReportDataSources, true, _tipoServicoMultisoftware, relatorioControleGeracao.Empresa?.CaminhoLogoDacte);

            _relatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, parametrosGeracaoRelatorio.ConfiguracaoRelatorio, parametrosGeracaoRelatorio.Parametros);
            _relatorioReportService.GerarRelatorio(report, relatorioControleGeracao, parametrosGeracaoRelatorio.CaminhoRelatorio, _unitOfWork);
        }
        catch (Exception excecao)
        {
            Log.TratarErro(excecao);
            _relatorioReportService.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, excecao);
        }

        try
        {
            EnviarRelatorioPorEmail(relatorioControleGeracao);
            EnviarRelatorioParaFTP(relatorioControleGeracao);
        }
        catch (Exception ex) { 
            Log.TratarErro("Erro ao enviar o relatorio "+ex.Message);
        }
    }

    #endregion

    #region Metodos Privados 

    private void EnviarRelatorioParaFTP(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
    {
        if (!(relatorioControleGeracao?.AutomatizacaoGeracaoRelatorio?.EnviarParaFTP ?? false))
            return;

        if (relatorioControleGeracao.SituacaoGeracaoRelatorio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado)
            return;

        Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP repositorioConfiguracaoFTP = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP(_unitOfWork);
        Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP configuracaoFTP = repositorioConfiguracaoFTP.BuscarPorAutomatizacao(relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.Codigo);

        if (configuracaoFTP == null)
            return;

        Relatorio servicoRelatorio = new Relatorio(_clienteMultisoftware);
        string caminhoRelatorio = servicoRelatorio.ObterCaminhoArquivoRelatorio(relatorioControleGeracao, _unitOfWork);

        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
            return;

        System.IO.MemoryStream relatorioGerado = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoRelatorio));
        string extensaoArquivo = System.IO.Path.GetExtension(caminhoRelatorio);
        string mensagemErro;
        string nomeArquivo;

        if (string.IsNullOrWhiteSpace(configuracaoFTP.Nomenclatura))
            nomeArquivo = $"{System.IO.Path.GetFileNameWithoutExtension(relatorioControleGeracao.Relatorio.ArquivoRelatorio)}_{relatorioControleGeracao.DataFinalGeracao:yyyyMMdd_HHmmss}";
        else
            nomeArquivo = configuracaoFTP.Nomenclatura
                .Replace("#Ano#", relatorioControleGeracao.DataFinalGeracao.ToString("yyyy"))
                .Replace("#Mes#", relatorioControleGeracao.DataFinalGeracao.ToString("MM"))
                .Replace("#Dia#", relatorioControleGeracao.DataFinalGeracao.ToString("dd"))
                .Replace("#Hora#", relatorioControleGeracao.DataFinalGeracao.ToString("HH"))
                .Replace("#Minutos#", relatorioControleGeracao.DataFinalGeracao.ToString("mm"))
                .Replace("#Segundos#", relatorioControleGeracao.DataFinalGeracao.ToString("ss"));

        FTP.EnviarArquivo(relatorioGerado, $"{nomeArquivo}{extensaoArquivo}", configuracaoFTP.EnderecoFTP, configuracaoFTP.Porta, configuracaoFTP.Diretorio, configuracaoFTP.Usuario, configuracaoFTP.Senha, configuracaoFTP.Passivo, configuracaoFTP.SSL, out mensagemErro, configuracaoFTP.UtilizarSFTP);

        if (string.IsNullOrWhiteSpace(mensagemErro))
            Log.TratarErro(mensagemErro);
    }

    private void EnviarRelatorioPorEmail(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
    {
        if (!(relatorioControleGeracao?.AutomatizacaoGeracaoRelatorio?.EnviarPorEmail ?? false))
            return;

        if (relatorioControleGeracao.SituacaoGeracaoRelatorio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado)
            return;

        Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioconfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
        Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioconfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

        if (configuracaoEmail == null)
            return;

        Relatorio servicoRelatorio = new Relatorio(_clienteMultisoftware);
        string caminhoRelatorio = servicoRelatorio.ObterCaminhoArquivoRelatorio(relatorioControleGeracao, _unitOfWork);

        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
            return;

        Servicos.Email servicoEmail = new Servicos.Email();
        System.IO.MemoryStream stream = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoRelatorio));
        List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(stream, relatorioControleGeracao.Relatorio.Titulo.Replace(" ", "_") + System.IO.Path.GetExtension(caminhoRelatorio)) };
        string assuntoEmail = $"{relatorioControleGeracao.DataFinalGeracao:dd/MM/yyyy HH:mm} - {relatorioControleGeracao.Relatorio.Titulo}";
        string conteudoEmail = $"Data da Geração: {relatorioControleGeracao.DataFinalGeracao:dd/MM/yyyy HH:mm:ss} <br/>Remetente: {_clienteMultisoftware.NomeFantasia.ToUpperInvariant()} <br/>Relatório: {relatorioControleGeracao.Relatorio.Titulo} <br/><br/>E-mail gerado automaticamente. Por favor, não responda este e-mail.";

        servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.Email, "", "", assuntoEmail, conteudoEmail, configuracaoEmail.Smtp, anexos, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp);
    }

    #endregion
}