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

[UseReportType(ReportType.CanhotoAvulso)]
public class CanhotoAvulsoReport : ReportBase

{
    public CanhotoAvulsoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCanhoto = extraData.GetValue<int>("CodigoCanhoto");

        //var unitOfWork = new UnitOfWork(_unitOfWork.StringConexao);

        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
        Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

        Dominio.Entidades.Embarcador.Filiais.Filial matriz = repFilial.BuscarMatriz();
        double cnpj = 0;

        if (matriz != null)
            double.TryParse(matriz.CNPJ, out cnpj);
        else if (canhoto.Filial != null)
            double.TryParse(canhoto.Filial.CNPJ, out cnpj);

        Dominio.Entidades.Cliente filial = repCliente.BuscarPorCPFCNPJ(cnpj);

        byte[] qrCode = Utilidades.QRcode.Gerar(canhoto.CanhotoAvulso.QRCode);

        List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.CanhotoAvulso> dsCanhotos =
            new List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.CanhotoAvulso>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.CanhotoAvulso()
                {
                    CEPEmpresa = filial?.CEP ?? "",
                    CidadeEmpresa = filial?.Localidade?.Descricao ?? "",
                    CNPJEmpresa = filial?.CPF_CNPJ_Formatado ?? "",
                    EnderecoEmpresa = (filial?.Endereco ?? "") + ", " + (filial?.Numero ?? "") + " - " +
                                      (filial?.Bairro ?? ""),
                    EstadoEmpresa = filial?.Localidade?.Estado.Nome ?? "",
                    IEEmpresa = filial?.IE_RG ?? "",
                    NomeEmpresa = filial?.Nome ?? "",
                    TelefoneEmpresa = filial?.Telefone1 ?? "",
                    Filial = canhoto.Filial != null ? canhoto.Filial.Descricao : "",
                    CNPJDestinatario = canhoto.Destinatario.CPF_CNPJ_Formatado,
                    DataEmissao = canhoto.DataEmissao,
                    Destinatario = canhoto.Destinatario.Nome,
                    Placa = canhoto.Carga != null ? canhoto.Carga.RetornarPlacas : "",
                    Motorista = canhoto.Carga != null
                        ? (canhoto.Carga.DadosSumarizados?.Motoristas ?? "")
                        : (canhoto.MotoristasResponsaveis != null
                            ? string.Join(", ", (from obj in canhoto.MotoristasResponsaveis select obj.Nome).ToList())
                            : ""),
                    notasFiscais = string.Join(", ",
                        (from obj in canhoto.CanhotoAvulso.PedidosXMLNotasFiscais
                            select obj.XMLNotaFiscal.Numero + "-" + obj.XMLNotaFiscal.Serie).ToList()),
                    NumeroCanhotoAvulso = canhoto.Numero,
                    PesoTotal = canhoto.Peso,
                    Transportador = canhoto.Empresa != null
                        ? "(" + canhoto.Empresa.CNPJ_Formatado + ")" + canhoto.Empresa.RazaoSocial
                        : "",
                    ValorTotal = canhoto.Valor,
                    QRCode = qrCode
                }
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsCanhotos
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Canhotos\canhotoAvulso.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}