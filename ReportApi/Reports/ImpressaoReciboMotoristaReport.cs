using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ImpressaoReciboMotorista)]
public class ImpressaoReciboMotoristaReport : ReportBase
{
    public ImpressaoReciboMotoristaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(_unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);

        var info = extraData.GetInfo();
        var acerto = repAcertoViagem.BuscarPorCodigo(extraData.GetValue<int>("CodigoAcerto"));
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem =
            repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita =
            servAcertoViagem.RetornaObjetoReceitaViagem(acerto.Codigo, _unitOfWork,
                ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem,
                ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada,
                ConfiguracaoEmbarcador.GerarTituloFolhaPagamento,
                ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado,
                ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem,
                (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

        if (receita == null || receita.Count == 0)
            throw new ServicoException("Não foi possível encontrar o registro.");

        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> dsReciboAcertoViagem =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();

        decimal adiantamento = receita[0].AdiantamentoMotorista;
        decimal outrasDespesas = receita[0].OutraDespesaMotorista;
        decimal bonificacao = receita[0].DiariaMotorista + receita[0].BonificacoesMotorista;
        decimal descontos = receita[0].DescontosMotorista;
        decimal comissao = receita[0].ComissaoMotorista;

        string numerosFrotas = acerto.Veiculos != null
            ? string.Join(", ", (from p in acerto.Veiculos select p.Veiculo.NumeroFrota))
            : string.Empty;
        string placas = acerto.Veiculos != null
            ? string.Join(", ", (from p in acerto.Veiculos select p.Veiculo.Placa))
            : string.Empty;

        decimal saldoMotorista = adiantamento + descontos - outrasDespesas - bonificacao - comissao;
        bool saldoNegativo = false;
        if (saldoMotorista > 1)
        {
            saldoMotorista = saldoMotorista * -1;
            saldoNegativo = true;
        }

        string valorExtenso = "";
        if (saldoMotorista < 0)
            valorExtenso = Utilidades.Conversor.DecimalToWords(saldoMotorista * -1);
        else
            valorExtenso = Utilidades.Conversor.DecimalToWords(saldoMotorista);

        if (saldoMotorista < 0 && !saldoNegativo)
            saldoMotorista = saldoMotorista * -1;

        Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
            new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
        recibo.DataFechamentoAcerto = acerto.DataFechamento.HasValue ? acerto.DataFechamento.Value : DateTime.Now;
        recibo.DataFimAcerto = acerto.DataFinal.Value;
        recibo.DataInicioAcerto = acerto.DataInicial;
        recibo.DescricaoDespesa = " ";
        recibo.ValorDespesa = saldoMotorista;
        recibo.ValorExtenso = valorExtenso;
        recibo.ValorTotal = saldoMotorista;
        recibo.FrotaVeiculos = numerosFrotas;
        recibo.Motorista = acerto.Motorista.Nome;
        recibo.NumeroAcerto = acerto.Numero;
        recibo.NumeroRecibo = 1;
        recibo.Operador = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario")).Nome;
        recibo.Proprietario =
            acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                ?
                (BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"))?.RazaoSocial ?? "")
                : acerto.Motorista.Empresa != null
                    ? acerto.Motorista.Empresa.RazaoSocial
                    : string.Empty;
        recibo.Veiculos = placas;
        dsReciboAcertoViagem.Add(recibo);

        Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
            (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
        recibo2.NumeroRecibo = 2;
        dsReciboAcertoViagem.Add(recibo2);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "crystal.png");

        parametro.NomeParametro = "CaminhoImagem";
        parametro.ValorParametro = caminhoLogo;
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "OutasDespesas";
        parametro.ValorParametro = outrasDespesas.ToString("n2").Replace(".", "").Replace(",", ".");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Bonificacao";
        parametro.ValorParametro = bonificacao.ToString("n2").Replace(".", "").Replace(",", ".");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Adiantamento";
        parametro.ValorParametro = adiantamento.ToString("n2").Replace(".", "").Replace(",", ".");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Descontos";
        parametro.ValorParametro = descontos.ToString("n2").Replace(".", "").Replace(",", ".");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Comissao";
        parametro.ValorParametro = comissao.ToString("n2").Replace(".", "").Replace(",", ".");
        parametros.Add(parametro);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsReciboAcertoViagem,
                Parameters = parametros
            };

        // Gera pd
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\AcertoViagem\ImpressaoReciboMotorista.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}