using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Servicos.ServicoPamCard;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.PamcardReciboVP)]
public class PamcardReciboVPReport : ReportBase
{
    public PamcardReciboVPReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        fieldTO[] retorno = extraData.GetValue<string>("retorno").FromJson<fieldTO[]>();
        RetornoImpressaoValePedagio retornoValePedagio = ProcessarRetornoValePedagio(retorno);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<RetornoImpressaoValePedagio>() { retornoValePedagio },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Documentos",
                        DataSet = ProcessarDocumentosViagem(retorno)
                    },

                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Pracas",
                        DataSet = ProcessarPracas(retorno)
                    },

                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Parcelas",
                        DataSet = ProcessarParcelas(retorno)
                    },

                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Favorecidos",
                        DataSet = ProcessarFavorecidos(retorno)
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\ValePedagio\PamcardReciboVP.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }

    private List<RetornoImpressaoValePedagioFavorecidos> ProcessarFavorecidos(fieldTO[] retorno)
    {
        List<RetornoImpressaoValePedagioFavorecidos> favorecidos = new List<RetornoImpressaoValePedagioFavorecidos>();
        int pessoasFavorecidas = (from obj in retorno where obj.key.Equals("viagem.favorecido.qtde") select obj.value).FirstOrDefault().ToInt();

        for (int i = 0; i < pessoasFavorecidas; i++)
        {
            int registro = pessoasFavorecidas - i;
            favorecidos.Add(new RetornoImpressaoValePedagioFavorecidos()
            {
                ViagemFavorecidoNumeroCartao = retorno.Where(obj => obj.key.Equals("viagem.favorecido" + registro + ".cartao.numero")).Select(obj => obj.value).FirstOrDefault(),
                ViagemFavorecidoQuantidadeDocumento = retorno.Where(obj => obj.key.Equals("viagem.favorecido" + registro + ".documento.qtde")).Select(obj => obj.value).FirstOrDefault(),
                ViagemFavorecidoMeioPagamento = retorno.Where(obj => obj.key.Equals("viagem.favorecido" + registro + ".meio.pagamento")).Select(obj => obj.value).FirstOrDefault(),
                ViagemFavorecidoNome = retorno.Where(obj => obj.key.Equals("viagem.favorecido" + registro + ".nome")).Select(obj => obj.value).FirstOrDefault(),
                ViagemFavorecidoTipo = retorno.Where(obj => obj.key.Equals("viagem.favorecido" + registro + ".tipo")).Select(obj => obj.value).FirstOrDefault()
            });
        }

        return favorecidos;
    }


    private List<RetornoImpressaoValePedagioParcelas> ProcessarParcelas(fieldTO[] retorno)
    {
        List<RetornoImpressaoValePedagioParcelas> parcelas = new List<RetornoImpressaoValePedagioParcelas>();
        int parcelasPedagio = retorno.Where(obj => obj.key.Equals("viagem.parcela.qtde")).Select(obj => obj.value).FirstOrDefault().ToInt();

        for (int i = 0; i < parcelasPedagio; i++)
        {
            int registro = parcelasPedagio - i;
            parcelas.Add(new RetornoImpressaoValePedagioParcelas()
            {
                ViagemParcelaData = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".data")).Select(obj => obj.value).FirstOrDefault().ToDateTime().ToDateTimeString(),
                ViagemParcelaEfetivacaoTipo = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".efetivacao.tipo")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelaFavorecidoTipoID = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".favorecido.tipo.id")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelaNumero = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".numero")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelOrigem = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".origem")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelaStatus = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".status.id")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelaTipo = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".tipo")).Select(obj => obj.value).FirstOrDefault(),
                ViagemParcelaValor = retorno.Where(obj => obj.key.Equals("viagem.parcela" + registro + ".valor")).Select(obj => obj.value).FirstOrDefault().ToDecimal()
            });
        }

        return parcelas;
    }

    private List<RetornoImpressaoValePedagioPracas> ProcessarPracas(fieldTO[] retorno)
    {
        List<RetornoImpressaoValePedagioPracas> pracas = new List<RetornoImpressaoValePedagioPracas>();
        int pracasPedagio = retorno.Where(obj => obj.key.Equals("viagem.pedagio.praca.qtde")).Select(obj => obj.value).FirstOrDefault().ToInt();

        for (int i = 0; i < pracasPedagio; i++)
        {
            int registro = pracasPedagio - i;
            pracas.Add(new RetornoImpressaoValePedagioPracas()
            {
                ViagemPedagioPracaKM = retorno.Where(obj => obj.key.Equals("viagem.pedagio" + registro + ".praca.km")).Select(obj => obj.value).FirstOrDefault().ToInt(),
                ViagemPedagioPracaNome = retorno.Where(obj => obj.key.Equals("viagem.pedagio" + registro + ".praca.nome")).Select(obj => obj.value).FirstOrDefault(),
                ViagemPedagioPracaSequencial = retorno.Where(obj => obj.key.Equals("viagem.pedagio" + registro + ".praca.seq")).Select(obj => obj.value).FirstOrDefault().ToInt(),
                ViagemPedagioPracaValor = retorno.Where(obj => obj.key.Equals("viagem.pedagio" + registro + ".praca.valor")).Select(obj => obj.value).FirstOrDefault().ToDecimal()
            });
        }

        return pracas;
    }

    private List<RetornoImpressaoValePedagioDocumentosViagem> ProcessarDocumentosViagem(fieldTO[] retorno)
    {
        List<RetornoImpressaoValePedagioDocumentosViagem> documentosViagem = new List<RetornoImpressaoValePedagioDocumentosViagem>();
        int docsViagem = retorno.Where(obj => obj.key.Equals("viagem.documento.qtde")).Select(obj => obj.value).FirstOrDefault().ToInt();

        for (int i = 0; i < docsViagem; i++)
        {
            int registro = docsViagem - i;
            documentosViagem.Add(new RetornoImpressaoValePedagioDocumentosViagem()
            {
                ViagemNumeroDocumento = retorno.Where(obj => obj.key.Equals("viagem.documento" + registro + ".numero")).Select(obj => obj.value).FirstOrDefault(),
                ViagemTipoDocumento = retorno.Where(obj => obj.key.Equals("viagem.documento" + registro + ".tipo")).Select(obj => obj.value).FirstOrDefault()
            });
        }

        return documentosViagem;
    }


    private RetornoImpressaoValePedagio ProcessarRetornoValePedagio(fieldTO[] retorno)
    {
        RetornoImpressaoValePedagio valePedagioImpressao = new RetornoImpressaoValePedagio()
        {
            MensagemCodigo = retorno.Where(obj => obj.key.Equals("mensagem.codigo")).Select(obj => obj.value).FirstOrDefault().ToInt(),
            MensagemDescricao = retorno.Where(obj => obj.key.Equals("mensagem.codigo")).Select(obj => obj.value).FirstOrDefault(),
            ViagemNumeroCartao = retorno.Where(obj => obj.key.Equals("viagem.cartao.numero")).Select(obj => obj.value).FirstOrDefault(),
            ViagemNumeroCartaoPortadorDocumento = retorno.Where(obj => obj.key.Equals("viagem.cartao.portador.documento.numero")).Select(obj => obj.value).FirstOrDefault(),
            ViagemNomePortadorCartao = retorno.Where(obj => obj.key.Equals("viagem.cartao.portador.nome")).Select(obj => obj.value).FirstOrDefault(),
            ViagemTipoCartao = retorno.Where(obj => obj.key.Equals("viagem.cartao.tipo")).Select(obj => obj.value).FirstOrDefault(),
            ViagemDataPartida = (from obj in retorno where obj.key.Equals("viagem.data.partida") select obj.value.ToDateTime().ToDateString()).FirstOrDefault(),
            ViagemNomeCidadeDestino = (from obj in retorno where obj.key.Equals("viagem.destino.cidade.nome") select obj.value).FirstOrDefault(),
            ViagemNomeEstadoDestino = (from obj in retorno where obj.key.Equals("viagem.destino.estado.nome") select obj.value).FirstOrDefault(),
            ViagemNomePaisDestino = (from obj in retorno where obj.key.Equals("viagem.destino.pais.nome") select obj.value).FirstOrDefault(),
            ViagemDigito = (from obj in retorno where obj.key.Equals("viagem.digito") select obj.value).FirstOrDefault().ToInt(),
            ViagemDocumentoQuantidade = (from obj in retorno where obj.key.Equals("viagem.documento.qtde") select obj.value).FirstOrDefault().ToInt(),
            ViagemFavorecidoQuantidade = (from obj in retorno where obj.key.Equals("viagem.favorecido.qtde") select obj.value).FirstOrDefault().ToInt(),
            ViagemID = (from obj in retorno where obj.key.Equals("viagem.id") select obj.value).FirstOrDefault(),
            ViagemOrigemCidadeNome = (from obj in retorno where obj.key.Equals("viagem.origem.cidade.nome") select obj.value).FirstOrDefault(),
            ViagemOrigemEstadoNome = (from obj in retorno where obj.key.Equals("viagem.origem.estado.nome") select obj.value).FirstOrDefault(),
            ViagemOrigemPaisNome = (from obj in retorno where obj.key.Equals("viagem.origem.pais.nome") select obj.value).FirstOrDefault(),
            ViagemParcelaQuantidade = (from obj in retorno where obj.key.Equals("viagem.parcela.qtde") select obj.value).FirstOrDefault().ToInt(),
            ViagemPedagioKM = (from obj in retorno where obj.key.Equals("viagem.pedagio.km") select obj.value).FirstOrDefault().ToInt(),
            ViagemPedagioOrigem = (from obj in retorno where obj.key.Equals("viagem.pedagio.origem") select obj.value).FirstOrDefault(),
            ViagemPedagioPracaQuantidade = (from obj in retorno where obj.key.Equals("viagem.pedagio.praca.qtde") select obj.value).FirstOrDefault().ToInt(),
            ViagemPedagioSolucaoID = (from obj in retorno where obj.key.Equals("viagem.pedagio.solucao.id") select obj.value).FirstOrDefault(),
            ViagemPedagioTempoPercurso = (from obj in retorno where obj.key.Equals("viagem.pedagio.tempo.percurso") select obj.value).FirstOrDefault(),
            ViagemPedagioValor = (from obj in retorno where obj.key.Equals("viagem.pedagio.valor") select obj.value).FirstOrDefault().ToDecimal(),
            ViagemPontoQuantidade = (from obj in retorno where obj.key.Equals("viagem.ponto.qtde") select obj.value).FirstOrDefault().ToInt(),
            ViagemStatus = (from obj in retorno where obj.key.Equals("viagem.status") select obj.value).FirstOrDefault(),
            ViagemUnidadeDocumentoNumero = (from obj in retorno where obj.key.Equals("viagem.unidade.documento.numero") select obj.value).FirstOrDefault(),
            ViagemUnidadeDocumentoTipo = (from obj in retorno where obj.key.Equals("viagem.unidade.documento.tipo") select obj.value).FirstOrDefault(),
            ViagemVaflexIndicador = (from obj in retorno where obj.key.Equals("viagem.vaflex.indicador") select obj.value).FirstOrDefault(),
            ViagemValor = (from obj in retorno where obj.key.Equals("viagem.pedagio.valor.carregado") select obj.value).FirstOrDefault().ToDecimal(),
            ViagemVeiculoCategoria = (from obj in retorno where obj.key.Equals("viagem.veiculo.categoria") select obj.value).FirstOrDefault(),
            ViagemVeiculoPlaca = (from obj in retorno where obj.key.Equals("viagem.veiculo.placa") select obj.value).FirstOrDefault(),
        };

        return valePedagioImpressao;
    }
}