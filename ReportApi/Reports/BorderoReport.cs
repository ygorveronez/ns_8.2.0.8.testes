using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.Bordero)]
public class BorderoReport : ReportBase
{
    public BorderoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repBordeiro = new Repositorio.Embarcador.Financeiro.Bordero(_unitOfWork);

        var bordero = repBordeiro.BuscarPorCodigo(extraData.GetValue<int>("CodigoBordero"));

        Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento =
            new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(_unitOfWork);

        Dominio.Relatorios.Embarcador.DataSource.Financeiros.Bordero borderoRelatorio =
            new Dominio.Relatorios.Embarcador.DataSource.Financeiros.Bordero()
            {
                Codigo = bordero.Codigo,
                Numero = bordero.Numero,
                ValorACobrar = bordero.ValorACobrar,
                ValorTotalAcrescimo = bordero.ValorTotalAcrescimo,
                ValorTotalDesconto = bordero.ValorTotalDesconto,
                ValorTotalACobrar = bordero.ValorTotalACobrar,
                DataEmissao = bordero.DataEmissao,
                DataVencimento = bordero.DataVencimento,
                ValorPorExtenso = Utilidades.Conversor.DecimalToWords(bordero.ValorTotalACobrar),
                ImprimirObservacao = bordero.ImprimirObservacao,
                Observacao = bordero.Observacao,

                NomeEmpresa = bordero.Empresa?.RazaoSocial,
                CNPJEmpresa = bordero.Empresa?.CNPJ_Formatado,
                IEEmpresa = bordero.Empresa?.InscricaoEstadual,
                EnderecoEmpresa = bordero.Empresa?.Endereco + ", " + bordero.Empresa?.Numero + " - Bairro " +
                                  bordero.Empresa?.Bairro,
                CidadeEmpresa = bordero.Empresa?.Localidade.DescricaoCidadeEstado,
                CEPEmpresa = bordero.Empresa?.CEP,
                TelefoneEmpresa = bordero.Empresa?.Telefone,

                NomeTomador = bordero.Tomador?.Nome,
                CNPJTomador = bordero.Tomador?.CPF_CNPJ_Formatado,
                IETomador = bordero.Tomador?.IE_RG,
                EnderecoTomador = bordero.Tomador?.Endereco + ", " + bordero.Tomador?.Numero + " - Bairro " +
                                  bordero.Tomador?.Bairro,
                CidadeTomador = bordero.Tomador?.Localidade.DescricaoCidadeEstado,
                CEPTomador = bordero.Tomador?.CEP,
                TelefoneTomador = bordero.Tomador?.Telefone1,

                Agencia = bordero.Agencia,
                Banco = bordero.Banco != null ? bordero.Banco.Numero + " - " + bordero.Banco.Descricao : null,
                NumeroConta = bordero.NumeroConta,
                TipoConta = bordero.DescricaoTipoConta
            };

        List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> documentos =
            repBorderoTituloDocumento.BuscarPorBordero(bordero.Codigo);

        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BorderoDocumento> documentosRelatorio =
            (from obj in documentos
             select new Dominio.Relatorios.Embarcador.DataSource.Financeiros.BorderoDocumento()
             {
                 Numero = obj.TituloDocumento.NumeroDocumento,
                 ValorACobrar = obj.ValorACobrar,
                 ValorTotalACobrar = obj.ValorTotalACobrar,
                 ValorTotalAcrescimo = obj.ValorTotalAcrescimo,
                 ValorTotalDesconto = obj.ValorTotalDesconto
             }).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Bordero>() { borderoRelatorio },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Bordero_Documentos.rpt",
                        DataSet = documentosRelatorio
                    }
                }
            };

        byte[] relatorio = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Financeiros\Bordero.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, true);

        return PrepareReportResult(FileType.PDF, relatorio);
    }
}