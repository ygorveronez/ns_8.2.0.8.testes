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

[UseReportType(ReportType.RomaneioEntrega)]
public class RomaneioEntregaReport : ReportBase
{
    public RomaneioEntregaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

        int codigoContratoFrete = extraData.GetValue<int>("codigoContratoFrete");
        var contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete);

        Dominio.Entidades.Usuario motorista = contratoFrete.Carga.Motoristas.FirstOrDefault();

        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntrega> romaneiosEntrega =
            new List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntrega>();

        romaneiosEntrega.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntrega()
        {
            CEPProprietario = contratoFrete.TransportadorTerceiro.CEP,
            CidadeEmpresa = contratoFrete.Carga.Empresa.Localidade.DescricaoCidadeEstado,
            CidadeProprietario = contratoFrete.TransportadorTerceiro.Localidade.DescricaoCidadeEstado,
            CPFCNPJProprietario = contratoFrete.TransportadorTerceiro.CPF_CNPJ,
            CPFMotorista = motorista.CPF_Formatado,
            DataSaida = contratoFrete.DataEmissaoContrato,
            Destino = string.Join(", ",
                contratoFrete.Carga.CargaCTes.Select(o => o.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado)
                    .Distinct()),
            EnderecoProprietario = contratoFrete.TransportadorTerceiro.Endereco,
            Frota = contratoFrete.Carga.Veiculo?.NumeroFrota ?? "",
            NomeEmpresa = contratoFrete.Carga.Empresa.RazaoSocial,
            NomeMotorista = motorista.Nome,
            NomeProprietario = contratoFrete.TransportadorTerceiro.Nome,
            Numero = contratoFrete.NumeroContrato,
            NumeroProprietario = contratoFrete.TransportadorTerceiro.Numero,
            Observacao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterObservacao(contratoFrete),
            Origem = contratoFrete.Carga.Empresa.Localidade.DescricaoCidadeEstado,
            PlacaCarreta = contratoFrete.Carga.Veiculo?.Placa ?? "",
            RGMotorista = motorista.RG,
            TipoPessoaProprietario = contratoFrete.TransportadorTerceiro.Tipo,
            UFEmpresa = contratoFrete.Carga.Empresa.Localidade.Estado.Sigla,
            UFProprietario = contratoFrete.TransportadorTerceiro.Localidade.Estado.Sigla,
            NumeroCarga = contratoFrete.Carga.CodigoCargaEmbarcador
        });

        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntregaItem> itensRomaneioEntrega =
            new List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntregaItem>();

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in contratoFrete.Carga.CargaCTes)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe notaFiscal in
                     cargaCTe.NotasFiscais)
            {
                itensRomaneioEntrega.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.RomaneioEntregaItem()
                {
                    CidadeDestino = cargaCTe.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                    CPFCNPJDestinatario = cargaCTe.CTe.Destinatario.CPF_CNPJ_Formatado,
                    CPFCNPJRemetente = cargaCTe.CTe.Remetente.CPF_CNPJ_Formatado,
                    NomeDestinatario = cargaCTe.CTe.Destinatario.Nome,
                    EnderecoDestinatario = cargaCTe.CTe.Destinatario.Endereco,
                    NomeRemetente = cargaCTe.CTe.Remetente.Nome,
                    NumeroCTe = cargaCTe.CTe.Numero,
                    NumeroNF = notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                    NumeroPedido = cargaCTe.NotasFiscais
                        .Select(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador).FirstOrDefault(),
                    Peso = notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                    UFDestino = cargaCTe.CTe.LocalidadeTerminoPrestacao.Estado.Sigla,
                    Volumes = notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes
                });
            }
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = romaneiosEntrega,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "RomaneioEntrega_Itens.rpt",
                        DataSet = itensRomaneioEntrega
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Terceiros\RomaneioEntrega.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}