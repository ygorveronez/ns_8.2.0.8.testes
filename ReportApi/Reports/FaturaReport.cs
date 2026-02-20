using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.Fatura)]
public class FaturaReport : ReportBase
{
    public FaturaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }
    
    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);

        var codigoFatura = extraData.GetValue<int>("CodigoFatura");
        var fatura = repFatura.BuscarPorCodigo(codigoFatura);
        var fileId = ObterFaturaPdf(fatura);
        return PrepareReportResult(FileType.PDF, fileId);
    }
    
    private string ObterFaturaPdf(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
    {
        if (fatura == null)
            throw new ArgumentException(nameof(fatura));
        
        Servicos.Embarcador.Fatura.FaturaImpressao servicoFaturaImpressao =
            Servicos.Embarcador.Fatura.FaturaImpressaoFactory.Criar(fatura, _unitOfWork,
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        string guidArquivoUltimoRelatorioGerado =
            servicoFaturaImpressao.ObterGuidArquivoUltimoRelatorioGerado(fatura);
        
        if (!string.IsNullOrWhiteSpace(guidArquivoUltimoRelatorioGerado))
        {
            string caminhoPDF = GetFullPath(guidArquivoUltimoRelatorioGerado, FileType.PDF);
            if (_storage.Exists(caminhoPDF))
            {
                return guidArquivoUltimoRelatorioGerado;
            }
            var repConfiguracaoTMS =
                new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            
            var relatorio =
                servicoFaturaImpressao.ObterRelatorio();

            if (fatura.Carga?.TipoOperacao?.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false)
                relatorio.Titulo += " - " + fatura.Carga.TipoOperacao.TipoPropostaMultimodal.ObterDescricao();

            var relatorioControleGeracao =
                _servicoRelatorioReportService.AdicionarRelatorioParaGeracao(relatorio, fatura.Usuario,
                    Dominio.Enumeradores.TipoArquivoRelatorio.PDF, _unitOfWork, fatura.Codigo);
            var relatorioTemporario =
                servicoFaturaImpressao.ObterRelatorioTemporario(relatorio);
            var configuracao =
                repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            servicoFaturaImpressao.GerarRelatorio(fatura, relatorioControleGeracao, relatorioTemporario,
                configuracao.TipoImpressaoFatura);

            guidArquivoUltimoRelatorioGerado =
                servicoFaturaImpressao.ObterGuidArquivoUltimoRelatorioGerado(fatura);
            return guidArquivoUltimoRelatorioGerado;
        }

        return string.Empty;
    }
}