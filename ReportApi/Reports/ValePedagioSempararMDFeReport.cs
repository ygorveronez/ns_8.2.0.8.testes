using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.ValePedagioSempararMDFe)]
public class ValePedagioSempararMDFeReport : ReportBase
{
    public ValePedagioSempararMDFeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoValePedagioMDFe = extraData.GetValue<int>("codigoValePedagioMDFe");

        Repositorio.ValePedagioMDFeCompra repositorioValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(_unitOfWork);

        Servicos.SemParar servicoValePedagioSemParar = new Servicos.SemParar(_unitOfWork);

        Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra = repositorioValePedagioMDFeCompra.BuscarPorCodigo(codigoValePedagioMDFe);
        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValePedagioSemParar.Autenticar(valePedagioMDFeCompra, _unitOfWork);

        if (!credencial.Autenticado)
        {
            Log.TratarErro("Retorno do Sem Parar:</br>" + credencial.Retorno, "GerarImpressaoValePedagio");
            return null;
        }

        Servicos.SemPararValePedagio.Recibo recibo = servicoValePedagioSemParar.ObterReciboViagem(valePedagioMDFeCompra.NumeroComprovante, credencial, out string _, out string _, _unitOfWork);

        if (recibo.status != 2 && recibo.status != 0)
        {
            Log.TratarErro("Retorno do Sem Parar:</br>" + credencial.Retorno, "GerarImpressaoValePedagio");
            return null;
        }

        string logo = recibo.logo.Split('.')[0];
        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", "LogoSemParar"), logo + ".png");

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
                    Observacao1 = "",
                    Observacao2 = "",
                    Observacao3 = "",
                    Observacao4 = "",
                    Observacao5 = "",
                    Observacao6 = "",
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

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsInformacoes,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoValePedagio", caminhoLogo, true)
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

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\ValePedagio\SemParar.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}