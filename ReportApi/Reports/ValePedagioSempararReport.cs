using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ValePedagioSemparar)]
public class ValePedagioSempararReport : ReportBase
{
    private Servicos.Embarcador.Integracao.SemParar.ValePedagio _srvValePedagio;

    public ValePedagioSempararReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
        _srvValePedagio = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware =
            extraData.GetInfo().TipoServico;

        int codigoCargaValePedagio = extraData.GetValue<int>("codigoCargaValePedagio");
        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio =
            repCargaValePedagio.BuscarPorCodigo(codigoCargaValePedagio);

        Servicos.Embarcador.Frota.ValePedagio servicoValePedagio =
            new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar =
            new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar =
            servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);
        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio respositorioCargaIntegracaoValePedagio =
            new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

        string nomeRPT = integracaoSemParar != null && !string.IsNullOrWhiteSpace(integracaoSemParar.NomeRpt)
            ? integracaoSemParar.NomeRpt
            : "SemParar.rpt";

        // Servicos
        Servicos.Embarcador.Integracao.SemParar.ValePedagio serValePedagioSemParar =
            new Servicos.Embarcador.Integracao.SemParar.ValePedagio();

        // Auth
        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial =
            serValePedagioSemParar.Autenticar(cargaValePedagio, _unitOfWork, tipoServicoMultisoftware, false);
        if (!credencial.Autenticado)
        {
            Log.TratarErro("Retorno do Sem Parar:</br>" + credencial.Retorno, "GerarImpressaoValePedagio");
            return null;
        }


        // Obtem dados
        Servicos.SemPararValePedagio.Recibo recibo =
            _srvValePedagio.ObterReciboViagem(credencial, cargaValePedagio, out string request, out string response, _unitOfWork);
        if (recibo.status != 2 && recibo.status != 0)
        {
            Log.TratarErro("Retorno do Sem Parar:</br>" + credencial.Retorno, "GerarImpressaoValePedagio");
            return null;
        }

        // Gera rpt
        // Salva a logo
        string logo = recibo.logo.Split('.')[0];
        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(_srvValePedagio.CaminhoLogoValePedagio(_unitOfWork), logo + ".png");

        cargaValePedagio.NomeTransportador = recibo.nomeTransp != null && recibo.nomeTransp.Length > 50
            ? recibo.nomeTransp.Substring(0, 50)
            : recibo.nomeTransp;
        respositorioCargaIntegracaoValePedagio.Atualizar(cargaValePedagio);

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo> dsInformacoes =
            new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo()
                {
                    NumeroVale = recibo.viagem.ToString(),
                    Tipo = recibo.tipo,
                    Emissor = "Via Fácil",
                    Embarcador = recibo.nomeEmissor,
                    CNPJEmbarcador = recibo.cnpjEmissor,
                    Transportador = recibo.nomeTransp,
                    CNPJTransportador = recibo.cnpjTransp,
                    DataConfirmacao = recibo.dataCompra?.ToString("dd/MM/yyyy") ?? " - ",
                    DataExpiracao = recibo.dataExp?.ToString("dd/MM/yyyy") ?? " - ",
                    DataViagem = recibo.dataViagem?.ToString("dd/MM/yyyy") ?? " - ",
                    VeiculoCategoria = recibo.catVeiculo,
                    Rota = recibo.nomeRota,
                    Total = (from p in recibo.pracas where p.tarifa.HasValue select p.tarifa.Value).Sum(),
                    Observacao1 = cargaValePedagio.Observacao1,
                    Observacao2 = cargaValePedagio.Observacao2,
                    Observacao3 = cargaValePedagio.Observacao3,
                    Observacao4 = cargaValePedagio.Observacao4,
                    Observacao5 = cargaValePedagio.Observacao5,
                    Observacao6 = cargaValePedagio.Observacao6
                }
            };

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoReciboPracas> pracas =
            (from p in recibo.pracas
             select new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoReciboPracas
             {
                 Praca = p.nomePraca,
                 Rodovia = p.nomeRodovia,
                 Concessionaria = p.nomeConcessionaria,
                 Placa = p.placa,
                 NumeroTAG = p.tag,
                 Valor = p.tarifa ?? 0
             }).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsInformacoes,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoValePedagio", caminhoLogo,
                        true)
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "SemPararPracas.rpt",
                        DataSet = pracas
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\ValePedagio\" + nomeRPT, Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, false);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}