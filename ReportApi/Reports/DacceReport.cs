using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.Dacce)]
public class DacceReport : ReportBase
{
    public DacceReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoDocumento = extraData.GetValue<int>("CodigoDocumento");
        var documentoDestinadoEmpresaRepositorio =
            new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(_unitOfWork);
        var documento = documentoDestinadoEmpresaRepositorio.BuscarPorCodigo(codigoDocumento);
        
        Dominio.Entidades.Cliente remetente = documento.CPFCNPJRemetente == null ? null : new Repositorio.Cliente(_unitOfWork).BuscarPorCPFCNPJ(documento.CPFCNPJRemetente.ToDouble());

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = ObterDataSetDACCe(documento, remetente);

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\NFe\DACCe.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
    
    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetDACCe(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, Dominio.Entidades.Cliente remetente)
{
    Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
    {
        DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.NFe.DACCe>()
        {
            new Dominio.Relatorios.Embarcador.DataSource.NFe.DACCe()
            {
                NomeEmitente = string.IsNullOrEmpty(documento.NomeEmitente) ? documento?.Emitente?.Nome ?? string.Empty : documento.NomeEmitente,
                EnderecoEmitente = documento?.Emitente?.EnderecoCompletoCidadeeEstadoFone ?? string.Empty,
                ChaveNFe = documento?.Chave ?? string.Empty,
                IEEmitente = documento?.Emitente?.IE_RG,
                CPFCNPJEmitente = string.IsNullOrEmpty(documento.CPFCNPJEmitente_Formatado) ? documento?.Emitente?.CPF_CNPJ_Formatado ?? string.Empty : documento.CPFCNPJEmitente_Formatado,
                Modelo = "NFe",
                Serie = documento?.Serie.ToString() ?? string.Empty,
                NumeroNFe = documento?.Numero.ToString() ?? string.Empty,
                MesEmissao = documento.DataEmissao.HasValue ? $"{documento.DataEmissao.Value.Date.Month}/{documento.DataEmissao.Value.Date.Year}" : string.Empty,
                Folha = "1/1",

                NomeRemetente = string.IsNullOrEmpty(documento.NomeRemetente) ? remetente?.Nome ?? string.Empty : documento.NomeRemetente,
                CPFCNPJRementente = string.IsNullOrEmpty(documento.CPFCNPJRemetente) ? remetente?.CPF_CNPJ_Formatado ?? string.Empty : documento.CPFCNPJRemetente,
                EnderecoRemetente = remetente?.Endereco ?? string.Empty,
                BairroRemetente = remetente?.Bairro ?? string.Empty,
                CEPRemetente = remetente?.Localidade?.CEP ?? remetente?.CEP ?? string.Empty,
                MunicipioRemetente = remetente?.Localidade?.Descricao ?? remetente?.Cidade ?? string.Empty,
                UFRemetente = remetente?.Localidade.Estado.Descricao ?? string.Empty,
                FoneRemetente = remetente?.Telefone1 ?? remetente?.Telefone2 ?? string.Empty,
                IERemetente = remetente?.IE_RG ?? string.Empty,

                Seq = documento?.NumeroSequencialEvento.ToString() ?? string.Empty,
                Status = "Autorizado",
                DataRegistro = documento.DataEmissao.HasValue ? documento.DataEmissao.Value.ToDateTimeString(true) : string.Empty,
                NumeroProtocolo = documento?.Protocolo ?? string.Empty,
                Correcao = documento?.Correcao ?? string.Empty
            }
        }
    };

    return dataSet;
}
}