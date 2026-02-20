using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.OutrosDocumentos)]
public class OutrosDocumentosReport : ReportBase
{
    public OutrosDocumentosReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCte.BuscarPorCodigo(extraData.GetValue<int>("codigoCte"));
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = ObterDataSet(cte);
        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(cte.ModeloDocumentoFiscal.Relatorio,
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, cte.ModeloDocumentoFiscal.RelatorioPossuiLogo, null,
            cte.ModeloDocumentoFiscal.RelatorioTarjaMensagem);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSet(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        VersaoRelatorioOutrosDocumentos? versaoRelatorio = ObterVersaoRelatorioOutrosDocumentos(cte.ModeloDocumentoFiscal);

        if (!versaoRelatorio.HasValue)
            return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet();

        switch (versaoRelatorio.Value)
        {
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao05:
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao06:
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao07:
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao01: return ObterDataSetNotaDebitoVersao01(cte);
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao02: return ObterDataSetNotaDebitoVersao02(cte);
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao03: return ObterDataSetNotaDebitoVersao03(cte);
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao04: return ObterDataSetNotaDebitoVersao04(cte);
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao08: return ObterDataSetNotaDebitoVersao08(cte);
            case VersaoRelatorioOutrosDocumentos.NotaDebitoVersao09: return ObterDataSetNotaDebitoVersao09(cte);
            case VersaoRelatorioOutrosDocumentos.RPS: return ObterDataSetRPS(cte);
            case VersaoRelatorioOutrosDocumentos.CRT: return ObterDataSetCRT(cte);
            default: return ObterDataSetVPS(cte);
        }
    }


    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetVPS(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe =
            new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCTe(cte.Codigo);
        Dominio.Entidades.Usuario motorista = cargaCTe.Carga.Motoristas?.FirstOrDefault();

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.VPS.VPS vps =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.VPS.VPS()
            {
                Caminhao = $"{(cargaCTe.Carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos.ToString() ?? "")} eixos",
                Data = cargaCTe.CTe.DataEmissao.Value,
                Destino =
                    $"{(cargaCTe.CTe.Destinatario?.Nome ?? " - ")} / {(cargaCTe.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? " - ")}",
                Motorista = motorista?.Nome ?? " - ",
                NotaFiscal = cargaCTe.NotasFiscais.First()?.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero ?? 0,
                Placa = cargaCTe.Carga.Veiculo?.Placa ?? " - ",
                RGMotorista = motorista?.RG ?? " - ",
                Valor = cargaCTe.CTe.ValorAReceber,
                ValorPorExtenso = Utilidades.Conversor.DecimalToWords(cargaCTe.CTe.ValorAReceber),
                Numero = cargaCTe.CTe.Numero
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.VPS.VPS>() { vps }
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetRPS(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe =
            new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork);
        Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe =
            repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo,
                cte.TomadorPagador.Localidade?.Codigo ?? 0, cte.TomadorPagador.Localidade?.Estado?.Sigla ?? "",
                cte.TomadorPagador.GrupoPessoas?.Codigo ?? 0, cte.TomadorPagador.Localidade?.Codigo ?? 0);
        if (configuracaoNFSe == null)
            configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo,
                cte.TomadorPagador.Localidade?.Codigo ?? 0, cte.TomadorPagador.Localidade?.Estado?.Sigla ?? "", 0, 0);
        if (configuracaoNFSe == null)
            configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo,
                cte.TomadorPagador.Localidade?.Codigo ?? 0, "", cte.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
        if (configuracaoNFSe == null)
            configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo,
                cte.TomadorPagador.Localidade?.Codigo ?? 0, "", 0, cte.TomadorPagador.Localidade?.Codigo ?? 0);
        if (configuracaoNFSe == null)
            configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo,
                cte.TomadorPagador.Localidade?.Codigo ?? 0, "", 0, 0);

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.RPS.RPS rps =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.RPS.RPS()
            {
                NomeTomador = cte.TomadorPagador.Nome,
                NomeTransportador = cte.Empresa.RazaoSocial,
                CNPJTomador = cte.TomadorPagador.CPF_CNPJ_Formatado,
                CNPJTransportador = cte.Empresa.CNPJ_Formatado,
                IETomador = cte.TomadorPagador.IE_RG,
                IETransportador = cte.Empresa.InscricaoEstadual,
                IMTomador = cte.TomadorPagador.InscricaoMunicipal,
                IMTransportador = cte.Empresa.InscricaoMunicipal,
                EnderecoTomador = $"{cte.TomadorPagador.Endereco}, {cte.TomadorPagador.Numero}",
                EnderecoTransportador = $"{cte.Empresa.Endereco}, {cte.Empresa.Numero}",
                BairroTomador = cte.TomadorPagador.Bairro,
                BairroTransportador = cte.Empresa.Bairro,
                CidadeTomador = cte.TomadorPagador.Localidade?.Descricao ?? string.Empty,
                CidadeTransportador = cte.Empresa.Localidade.Descricao,
                UFTomador = cte.TomadorPagador.Localidade?.Estado.Sigla ?? string.Empty,
                CidadeUFTransportador = cte.Empresa.Localidade.DescricaoCidadeEstado,
                CEPTransportador = cte.Empresa.CEP,
                NomeFantasiaTransportador = cte.Empresa.NomeFantasia,

                DataEmissao = cte.DataEmissao.Value.ToString("dd/MM/yyyy"),
                Numero = cte.Numero,
                Observacoes = (cte.ObservacoesGerais + " " +
                               (configuracaoNFSe?.ServicoNFSe?.Descricao ?? string.Empty) + " " +
                               (!string.IsNullOrWhiteSpace(cte.NumeroOS) ? ", OS " + cte.NumeroOS : string.Empty) +
                               " " +
                               (cte.Containers.Count > 0
                                   ? ", Container " + string.Join(", ",
                                       cte.Containers.Select(o => o.Container.Descricao).ToList())
                                   : string.Empty) + " " +
                               (!string.IsNullOrWhiteSpace(cte.NumeroBooking)
                                   ? ", Booking " + cte.NumeroBooking
                                   : string.Empty)).Trim(),
                Valor = cte.ValorAReceber,
                ValorISS = cte.ValorISS,
                AliquotaISS = cte.AliquotaISS,
                RetencaoISS = cte.ValorISSRetido,
                LocalidadeServico = cte.LocalidadeEmissao.Descricao,
                CondicaoPagamento = string.Empty,
                NaturezaOperacao = configuracaoNFSe?.NaturezaNFSe?.Descricao ?? string.Empty,
                NumeroOS = cte.NumeroOS,
                Viagem = cte.Viagem?.Descricao,
                Service = (configuracaoNFSe?.ServicoNFSe?.Descricao ?? string.Empty)
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.RPS.RPS>() { rps }
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetCRT(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
        Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaFronteira repFronteiras = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);
        Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCargaCTe.BuscarCargaPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = repFronteiras.BuscarPorCarga(carga.Codigo);

        string cnpjRemetente = cte.Remetente.Cliente.Tipo.Equals("E") ? "CUIT: " + cte.Remetente.Cliente.NumeroCUITRUT : "CNPJ NUMBER " + cte.Remetente.Cliente.CPF_CNPJ_Formatado;
        string cnpjDestinatario = cte.Destinatario.Cliente.Tipo.Equals("E") ? "CUIT: " + cte.Destinatario.Cliente.NumeroCUITRUT : "CNPJ NUMBER " + cte.Destinatario.Cliente.CPF_CNPJ_Formatado;
        string cnpjTomador = cte.Tomador.Cliente.Tipo.Equals("E") ? "CUIT: " + cte.Tomador.Cliente.NumeroCUITRUT : "CNPJ NUMBER " + cte.Tomador.Cliente.CPF_CNPJ_Formatado;
        string cnpjRecebedor = cte.Recebedor == null ? cnpjDestinatario : cte.Recebedor.Cliente.Tipo.Equals("E") ? "CUIT: " + cte.Recebedor.Cliente.NumeroCUITRUT : "CNPJ NUMBER " + cte.Recebedor.Cliente.CPF_CNPJ_Formatado;
        decimal valorCotacao = serCotacao.BuscarValorCotacaoCliente(MoedaCotacaoBancoCentral.DolarCompra, _unitOfWork, 0, cte.XMLNotaFiscais.Select(o => o.DataEmissao).FirstOrDefault());
        bool freteAoRemetente = cte.Tomador.CPF_CNPJ == cte.Destinatario.CPF_CNPJ ? false : true;
        string fronteira = string.Join(", ", fronteiras.Select(o => o.Fronteira.Nome));
        bool temCotacao = valorCotacao > 0;
        decimal valorMercadorias = carga.CargaOrigemPedidos?.Sum(o => o.ValorTotalMoeda) ?? 0;

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.CRT.CRT crt = new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.CRT.CRT()
        {
            Remetente = cte.Remetente.Nome,
            EnderecoRemetente = $"{cte.Remetente.Endereco} {cte.Remetente.Numero} {cte.Remetente.Complemento} - {cte.Remetente.Cliente.Localidade.Descricao} {cte.Remetente.Cliente.Localidade.Pais.Descricao} {cnpjRemetente}",
            Destinatario = cte.Destinatario.Nome,
            EnderecoDestinatario = $"{cte.Destinatario.Endereco} {cte.Destinatario.Numero} {cte.Destinatario.Complemento} - {cte.Destinatario.Cliente.Localidade.Descricao} {cte.Destinatario.Cliente.Localidade.Pais.Descricao} {cnpjDestinatario}",
            Tomador = cte.Tomador.Nome,
            EnderecoTomador = $"{cte.Tomador.Endereco} {cte.Tomador.Numero} {cte.Tomador.Complemento} - {cte.Tomador.Cliente.Localidade.Descricao} {cte.Tomador.Cliente.Localidade.Pais.Descricao} {cnpjTomador}",
            Emitente = cte.Empresa.RazaoSocial,
            EmitenteCNPJ = cte.Empresa.CNPJ_SemFormato,
            EnderecoEmitente = $"{cte.Empresa.Endereco} {cte.Empresa.Numero} - {cte.Empresa.Localidade.Descricao} ({cte.Empresa.Localidade.Pais.Abreviacao}) {cte.Empresa.Localidade.Pais.Descricao}",
            Origem = string.IsNullOrWhiteSpace(fronteira) ? $"{cte.Remetente.Cliente.Localidade.Descricao} - {cte.Remetente.Cliente.Localidade.Pais.Descricao}" : fronteira,
            OrigemDataEmissao = $"{cte.Remetente.Cliente.Localidade.Descricao} - {cte.Remetente.Cliente.Localidade.Pais.Descricao} - {cte.DataEmissao?.ToString("dd-MM-yyyy") ?? string.Empty}",
            DestinoDataEntrega = $"{cte.Destinatario.Cliente.Localidade.Descricao} - {cte.Destinatario.Cliente.Localidade.Pais.Descricao} - {cte.DataPrevistaEntrega?.ToString("dd-MM-yyyy") ?? string.Empty}",
            Observacao = $"{string.Join(", ", cte.ModeloDocumentoFiscal?.Descricao)} - Peso: {string.Join(", ", cte.XMLNotaFiscais?.Select(o => o.Peso))} - Volumes: {string.Join(", ", cte.XMLNotaFiscais?.Select(o => o?.Volumes.ToString()))} - Produto: {string.Join(", ", cte.XMLNotaFiscais?.Select(o => o?.Produto))}  - NCM: {string.Join(", ", cte.XMLNotaFiscais?.Select(o => o?.NCM))}",
            PesoBruto = cte.XMLNotaFiscais.Sum(o => o.Peso) > 0 ? cte.XMLNotaFiscais.Sum(o => o.Peso) : cte.Peso,
            PesoLiquido = cte.XMLNotaFiscais.Sum(o => o?.PesoLiquido ?? 0) > 0 ? cte.XMLNotaFiscais.Sum(o => o?.PesoLiquido ?? 0) : cte.PesoLiquido,
            Volumes = cte.XMLNotaFiscais.Sum(o => o?.MetrosCubicos ?? 0) > 0 ? cte.XMLNotaFiscais.Sum(o => o?.MetrosCubicos ?? 0) : cte.Volumes,
            ValorAReceber = valorMercadorias, //ValorAReceber = temCotacao ? documentos.Sum(o => o.XMLNotaFiscal?.ValorTotalProdutos * valorCotacao ?? 0) : documentos.Sum(o => o.XMLNotaFiscal?.ValorTotalProdutos ?? 0),
            ValorFrete = temCotacao ? cte.ValorFrete * valorCotacao : cte.ValorFrete,
            SiglaEstrangeira = temCotacao ? "U$" : cte.Moeda?.ObterSiglaEstrangeira(),
            ValorMercadoriasPorExtenso = temCotacao ? Utilidades.Conversor.DecimalToWords(cte.ValorTotalMercadoria, "Dólar") : Utilidades.Conversor.DecimalToWords(cte.ValorTotalMercadoria, cte.Moeda?.ObterDescricaoSimplificada()),
            Numero = !string.IsNullOrWhiteSpace(cte.NumeroCRT) ? cte.NumeroCRT : $"{cte.Remetente.Cliente.Localidade.Pais.Abreviacao}.{(cte.Empresa.NumeroCertificadoIdoneidade ?? string.Empty).PadLeft(3, '0')}.{string.Format(@"{0:000\.000}", cte.Numero)}",
            FreteAoRemetente = freteAoRemetente,
            Aduanas = fronteira,
            DataEmissaoDocumento = cte.XMLNotaFiscais.Select(o => o.DataEmissao.ToString("dd-MM-yyyy")).FirstOrDefault(),
            ObservacaoCTe = cte.ObservacoesGerais
        };

        crt.Recebedor = cte.Recebedor != null ? cte.Recebedor.Nome : crt.Destinatario;
        crt.EnderecoRecebedor = cte.Recebedor == null ? crt.EnderecoDestinatario : $"{cte.Recebedor.Endereco} {cte.Recebedor.Numero} {cte.Recebedor.Complemento} - {cte.Recebedor.Cliente.Localidade.Descricao} {cte.Recebedor.Cliente.Localidade.Pais.Descricao} {cnpjRecebedor}";
        crt.DocumentosCTe = $"{string.Join(", ", cte.XMLNotaFiscais.Select(o => o?.NumeroControleCliente))}";

        List<Dominio.Entidades.ComponentePrestacaoCTE> componentes = cte.ComponentesPrestacao.ToList();
        if (componentes.Count > 0)
        {
            crt.ComponenteFrete += componentes.Sum(o => o.ValorFrete);
            crt.ComponenteSeguro += componentes.Sum(o => o.ValorSeguro);
            crt.ComponenteOutros += componentes.Where(o => o.ValorFrete == 0 && o.ValorSeguro == 0).Sum(o => o.Valor);
            crt.TotalComponentes += componentes.Sum(o => o.Valor);

            crt.DadosComponentes = string.Join(Environment.NewLine, componentes.Select(o => o.Nome + ": " + crt.SiglaEstrangeira + " " + o.Valor.ToString("n2")));
            crt.DadosComponentes += Environment.NewLine + "FRETE TOTAL: " + crt.SiglaEstrangeira + " " + crt.TotalComponentes.ToString("n2");
        }

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.CRT.CRT>() { crt }
        };
    }


    private VersaoRelatorioOutrosDocumentos? ObterVersaoRelatorioOutrosDocumentos(
        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
    {
        string nomeRelatorio = modeloDocumentoFiscal?.Relatorio?.ToLower();

        if (string.IsNullOrWhiteSpace(nomeRelatorio))
            return null;

        if (nomeRelatorio.EndsWith("vps.rpt"))
            return VersaoRelatorioOutrosDocumentos.VPS;

        if (nomeRelatorio.EndsWith("notadebito.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao01;

        if (nomeRelatorio.EndsWith("notadebito_v2.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao02;

        if (nomeRelatorio.EndsWith("notadebito_v3.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao03;

        if (nomeRelatorio.EndsWith("notadebito_v4.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao04;

        if (nomeRelatorio.EndsWith("notadebito_v5.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao05;

        if (nomeRelatorio.EndsWith("notadebito_v6.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao06;

        if (nomeRelatorio.EndsWith("notadebito_v7.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao07;

        if (nomeRelatorio.EndsWith("notadebito_v8.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao08;

        if (nomeRelatorio.EndsWith("notadebito_v9.rpt"))
            return VersaoRelatorioOutrosDocumentos.NotaDebitoVersao09;

        if (nomeRelatorio.EndsWith("rps.rpt"))
            return VersaoRelatorioOutrosDocumentos.RPS;

        if (nomeRelatorio.EndsWith("crt.rpt"))
            return VersaoRelatorioOutrosDocumentos.CRT;

        return null;
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao01(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Dominio.Entidades.Empresa empresa = cte.Empresa;
        Dominio.Entidades.ParticipanteCTe tomadorPagador = cte.TomadorPagador;

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebito notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebito()
            {
                CidadeTomador = tomadorPagador.Localidade?.Descricao ?? tomadorPagador.Cidade,
                CidadeTransportador = empresa.Localidade?.Descricao ?? "",
                CNPJTomador = tomadorPagador.CPF_CNPJ_Formatado,
                CNPJTransportador = empresa.CNPJ_Formatado,
                Contato = empresa.Contato,
                DataEmissao = cte.DataEmissao.Value,
                EnderecoTomador = $"{tomadorPagador.Endereco}, {tomadorPagador.Numero}" +
                                  (!string.IsNullOrWhiteSpace(tomadorPagador.Complemento)
                                      ? ", " + tomadorPagador.Complemento
                                      : string.Empty),
                EnderecoTransportador = $"{empresa.Endereco}, {empresa.Numero}" +
                                        (!string.IsNullOrWhiteSpace(empresa.Complemento)
                                            ? ", " + empresa.Complemento
                                            : string.Empty),
                CEPTomador = tomadorPagador.CEP,
                CEPTransportador = empresa.CEP,
                BairroTomador = tomadorPagador.Bairro,
                BairroTransportador = empresa.Bairro,
                IETomador = tomadorPagador.IE_RG,
                IETransportador = empresa.InscricaoEstadual,
                IMTransportador = empresa.InscricaoMunicipal,
                NomeTomador = tomadorPagador.Nome,
                NomeTransportador = empresa.RazaoSocial,
                Numero = cte.Numero,
                Referencia = cte.ObservacoesGerais,
                NotasFiscaisReferencia = string.Join(", ", cte.Documentos.Select(o => o.Numero)),
                Telefone = empresa.TelefoneContato,
                UFTomador = tomadorPagador.Localidade?.Estado.Sigla ?? "",
                UFTransportador = empresa.Localidade?.Estado.Sigla ?? "",
                Valor = cte.ValorAReceber,
                ValorFrete = cte.ValorFrete,
                ValorICMS = cte.ValorICMS,
                Placas = string.Join(", ", cte.Veiculos.Select(o => o.Placa)),
                Banco = empresa.Banco?.Descricao ?? string.Empty,
                NumeroBanco = empresa.Banco?.Numero.ToString() ?? string.Empty,
                Agencia = empresa.Agencia +
                          (!string.IsNullOrEmpty(empresa.DigitoAgencia) ? "-" + empresa.DigitoAgencia : ""),
                Conta = empresa.NumeroConta,
                Observacao = cte.ObservacoesGerais,
                NomeFavorecido = empresa.EmpresaFavorecida != null
                    ? empresa.EmpresaFavorecida.RazaoSocial
                    : empresa.RazaoSocial,
                CNPJFavorecido = empresa.EmpresaFavorecida != null
                    ? empresa.EmpresaFavorecida.CNPJ_Formatado
                    : empresa.CNPJ_Formatado,
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebito>()
                { notaDebito },
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao02(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo =
            new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento =
            repositorioCargaCTeComplementoInfo.BuscarPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovadores =
            new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();

        if (complemento != null && complemento.CargaOcorrencia != null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao =
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            aprovadores = repositorioCargaOcorrenciaAutorizacao.BuscarAprovadoresOcorrencia(
                complemento.CargaOcorrencia.Codigo,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
        }

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2 notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2()
            {
                ModeloDebito =
                    cte.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito,
                Data = cte.DataEmissao.Value,
                Numero = cte.Numero.ToString(),
                Remetente = cte.Remetente.Descricao,
                EnderecoRemetente = cte.Remetente.Endereco,
                Destinatario = cte.Empresa.Descricao + " - " + cte.Empresa.CNPJ_Formatado,
                EnderecoDestinatario = cte.Empresa.Endereco,
                Fatura = "",
                Encomenda = "",
                Vendedor = "",
                Termos = "",
                AprovadoPor = String.Join(", ", (from a in aprovadores select a.Usuario.Nome).Distinct())
            };

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2_Item itens =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2_Item()
            {
                NumeroItem = cte.Numero,
                Drescricao = complemento?.CargaOcorrencia?.ObservacaoCTe.Trim() ?? "",
                PrecoUnitario = cte.ValorFrete,
                Quantidade = 1,
                ValorTotal = cte.ValorFrete
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                { notaDebito },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "NotaDebito_V2_Item.rpt",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                            NotaDebitoV2_Item>() { itens }
                }
            }
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao03(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo =
            new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
        Repositorio.Embarcador.Cargas.CTeProduto repCTeProduto =
            new Repositorio.Embarcador.Cargas.CTeProduto(_unitOfWork);
        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(_unitOfWork);
        Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repProdutos =
            new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosXMLNotaFiscal =
            repProdutos.BuscarPorCTe(cte.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento =
            repositorioCargaCTeComplementoInfo.BuscarPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovadores =
            new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = complemento?.CargaCTeComplementado?.CTe;
        List<Dominio.Entidades.Embarcador.Cargas.CTeProduto> produtos = repCTeProduto.ObterProdutosPorCTe(cte.Codigo);
        List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(cte.Codigo);

        bool possuiProdutos = produtos != null && produtos.Count > 0;

        if (complemento != null && complemento.CargaOcorrencia != null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao =
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            aprovadores = repositorioCargaOcorrenciaAutorizacao.BuscarAprovadoresOcorrencia(
                complemento.CargaOcorrencia.Codigo, EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia);
        }

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2 notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2()
            {
                ModeloDebito =
                    cte.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito,
                Data = cte.DataEmissao.Value,
                Numero = cte.Numero.ToString(),
                Remetente = cte.Remetente.Descricao,
                EnderecoRemetente = cte.Remetente.Endereco,
                Destinatario = cte.Empresa.Descricao + " - " + cte.Empresa.CNPJ_Formatado,
                EnderecoDestinatario = cte.Empresa.Endereco,
                Fatura = "",
                Encomenda = "",
                Vendedor = "",
                Termos = "",
                AprovadoPor = string.Join(", ", (from a in aprovadores select a.Usuario.Nome)),
                Serie = cte.Serie?.Numero.ToString() ?? "",
                ObservacaoAprovador = complemento?.CargaOcorrencia?.ObservacaoAprovador,
                PossuiProdutos = possuiProdutos
            };

        if (possuiProdutos)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03ItemComProdutos>
                itens =
                    new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                        NotaDebitoVersao03ItemComProdutos>();

            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CTeProduto cteProduto in produtos.Where(p =>
                             p.CTe.Codigo == documentoCTe.CTE.Codigo))
                {
                    decimal valorProduto = produtosXMLNotaFiscal.Where(o =>
                            o.XMLNotaFiscal.Codigo == cteProduto.XMLNotaFiscal.Codigo &&
                            o.Produto.Codigo == cteProduto.Produto.Codigo)
                        .FirstOrDefault().ValorProduto;
                    int quantidade = cteProduto.Quantidade;

                    itens.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                            NotaDebitoVersao03ItemComProdutos
                        {
                            Nota = documentoCTe.Numero,
                            Codigo = cteProduto.Produto.CodigoProdutoEmbarcador,
                            Produto = cteProduto.Produto.Descricao,
                            Quantidade = quantidade,
                            Valor = valorProduto,
                            ValorTotal = valorProduto * quantidade,
                        });
                }
            }

            return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                    { notaDebito },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ProdutosNotaDebitoV3",
                        DataSet = itens,
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Titulo),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Debito",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Debito),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Credito",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Credito),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Numero + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Serie + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("De",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.De + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Para",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Para + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AprovadoPor",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.AprovadoPor + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ObservacaoAprovador",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.ObservacaoAprovador + ":"),

                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NOcorrencia",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.NOcorrencia),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DescricaoObservacao",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.DescricaoObservacao),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeOrigem",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.CTeOrigem),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SerieSub",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Serie),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NFeOrigem",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.NFeOrigem),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorUnitario",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.ValorUnitario),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Qtde",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Qtde),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorTotal",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.ValorTotal),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Total",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Total),
                }
            };
        }
        else
        {
            Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item itens =
                new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item()
                {
                    Descricao =
                        $"{(complemento?.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? string.Empty).Trim()}{(string.IsNullOrWhiteSpace(complemento?.CargaOcorrencia?.ObservacaoCTe) ? "" : $" - {complemento.CargaOcorrencia.ObservacaoCTe.Trim()}")}",
                    NfeCteOrigem = cteComplementado?.NumeroNotas ?? "",
                    Numero = complemento?.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                    NumeroCteOrigem = cteComplementado?.Numero.ToString() ?? "",
                    PrecoUnitario = cte.ValorFrete,
                    Quantidade = 1,
                    SerieCteOrigem = cteComplementado?.Serie?.Numero.ToString() ?? "",
                    ValorTotal = cte.ValorFrete
                };

            return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                    { notaDebito },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "NotaDebito_V3_Item.rpt",
                        DataSet =
                            new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                                NotaDebitoVersao03Item>() { itens },
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Titulo),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Debito",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Debito),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Credito",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Credito),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Numero + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Serie + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("De",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.De + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Para",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.Para + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AprovadoPor",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.AprovadoPor + ":"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ObservacaoAprovador",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V2.ObservacaoAprovador + ":"),

                    #region Sub-relatório

                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NOcorrencia",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.NOcorrencia),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DescricaoObservacao",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.DescricaoObservacao),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeOrigem",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.CTeOrigem),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SerieSub",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Serie),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NFeOrigem",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.NFeOrigem),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorUnitario",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.ValorUnitario),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Qtde",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Qtde),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorTotal",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.ValorTotal),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Total",
                        Localization.Resources.Relatorios.NotasDebito.NotaDebito_V3_Item.Total),

                    #endregion
                }
            };
        }
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao09(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo =
            new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento =
            repositorioCargaCTeComplementoInfo.BuscarPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovadores =
            new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = complemento?.CargaCTeComplementado?.CTe;

        if (complemento != null && complemento.CargaOcorrencia != null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao =
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            aprovadores =
                repositorioCargaOcorrenciaAutorizacao.BuscarAprovadoresOcorrenciaSemEtapa(complemento.CargaOcorrencia
                    .Codigo);
        }

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2 notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2()
            {
                ModeloDebito =
                    cte.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito,
                Data = cte.DataEmissao.Value,
                Numero = cte.Numero.ToString(),
                Remetente = cte.Remetente.Descricao,
                EnderecoRemetente = cte.Remetente.Endereco,
                Destinatario = cte.Empresa.Descricao + " - " + cte.Empresa.CNPJ_Formatado,
                EnderecoDestinatario = cte.Empresa.Endereco,
                Fatura = "",
                Encomenda = "",
                Vendedor = "",
                Termos = "",
                AprovadoPor = string.Join(", ", (from a in aprovadores select a.Usuario.Nome)),
                Serie = cte.Serie?.Numero.ToString() ?? "",
                ObservacaoAprovador = complemento?.CargaOcorrencia?.ObservacaoAprovador
            };

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item itens =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item()
            {
                Descricao =
                    $"{(complemento?.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? string.Empty).Trim()}{(string.IsNullOrWhiteSpace(complemento?.CargaOcorrencia?.ObservacaoCTe) ? "" : $" - {complemento.CargaOcorrencia.ObservacaoCTe.Trim()}")}",
                NfeCteOrigem = cteComplementado?.NumeroNotas ?? "",
                Numero = complemento?.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                NumeroCteOrigem = cteComplementado?.Numero.ToString() ?? "",
                PrecoUnitario = complemento?.CargaOcorrencia?.Quantidade > 0
                    ? cte.ValorFrete / complemento.CargaOcorrencia.Quantidade
                    : cte.ValorFrete,
                Quantidade = complemento?.CargaOcorrencia?.Quantidade ?? 1,
                SerieCteOrigem = cteComplementado?.Serie?.Numero.ToString() ?? "",
                ValorTotal = cte.ValorFrete
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                { notaDebito },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "NotaDebito_V9_Item",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                            NotaDebitoVersao03Item>() { itens }
                }
            }
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao08(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo =
            new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento =
            repositorioCargaCTeComplementoInfo.BuscarPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovadores =
            new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = complemento?.CargaCTeComplementado?.CTe;

        if (complemento != null && complemento.CargaOcorrencia != null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao =
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            aprovadores =
                repositorioCargaOcorrenciaAutorizacao.BuscarAprovadoresOcorrencia(complemento.CargaOcorrencia.Codigo,
                    EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
        }

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2 notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2()
            {
                ModeloDebito =
                    cte.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito,
                Data = cte.DataEmissao.Value,
                Numero = cte.Numero.ToString(),
                Remetente = cte.TomadorPagador.Descricao,
                EnderecoRemetente = cte.TomadorPagador.Endereco,
                Destinatario = cte.Empresa.Descricao + " - " + cte.Empresa.CNPJ_Formatado,
                EnderecoDestinatario = cte.Empresa.Endereco,
                Fatura = "",
                Encomenda = "",
                Vendedor = "",
                Termos = "",
                AprovadoPor = string.Join(", ", (from a in aprovadores select a.Usuario.Nome)),
                Serie = cte.Serie?.Numero.ToString() ?? "",
                ObservacaoAprovador = complemento?.CargaOcorrencia?.ObservacaoAprovador
            };

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item itens =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao03Item()
            {
                Descricao =
                    $"{(complemento?.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? string.Empty).Trim()}{(string.IsNullOrWhiteSpace(complemento?.CargaOcorrencia?.ObservacaoCTe) ? "" : $" - {complemento.CargaOcorrencia.ObservacaoCTe.Trim()}")}",
                NfeCteOrigem = cteComplementado?.NumeroNotas ?? "",
                Numero = complemento?.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                NumeroCteOrigem = cteComplementado?.Numero.ToString() ?? "",
                PrecoUnitario = cte.ValorFrete,
                Quantidade = 1,
                SerieCteOrigem = cteComplementado?.Serie?.Numero.ToString() ?? "",
                ValorTotal = cte.ValorFrete
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                { notaDebito },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "NotaDebito_V3_Item.rpt",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                            NotaDebitoVersao03Item>() { itens }
                }
            }
        };
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetNotaDebitoVersao04(
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo =
            new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento =
            repositorioCargaCTeComplementoInfo.BuscarPorCTe(cte.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovadores =
            new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = complemento?.CargaCTeComplementado?.CTe;
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> notasFiscaisCteComplementado =
            complemento?.CargaCTeComplementado?.NotasFiscais?.ToList() ??
            new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos =
            (from notaFiscalCteComplementado in notasFiscaisCteComplementado
             select notaFiscalCteComplementado.PedidoXMLNotaFiscal.CargaPedido.Pedido).ToList();
        string ordens = string.Join(", ", (from pedido in pedidos select pedido.Ordem).ToList().Distinct());

        if (complemento != null && complemento.CargaOcorrencia != null)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao =
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            aprovadores =
                repositorioCargaOcorrenciaAutorizacao.BuscarAprovadoresOcorrencia(complemento.CargaOcorrencia.Codigo,
                    EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
        }

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2 notaDebito =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2()
            {
                ModeloDebito =
                    cte.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito,
                Data = cte.DataEmissao.Value,
                Numero = cte.Numero.ToString(),
                Remetente = cte.TomadorPagador.Descricao,
                EnderecoRemetente = cte.TomadorPagador.Endereco,
                Destinatario = cte.Empresa.Descricao + " - " + cte.Empresa.CNPJ_Formatado,
                EnderecoDestinatario = cte.Empresa.Endereco,
                Fatura = "",
                Encomenda = "",
                Vendedor = "",
                Termos = "",
                AprovadoPor = String.Join(", ", from a in aprovadores select a.Usuario.Empresa.RazaoSocial)
            };

        Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao04Item itens =
            new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoVersao04Item()
            {
                CodigoCargaEmbarcador = complemento?.CargaOcorrencia?.Carga?.CodigoCargaEmbarcador ?? "",
                Descricao = complemento?.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? "",
                NfeCteOrigem = cteComplementado?.NumeroNotas,
                Numero = complemento?.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                NumeroCteOrigem = cteComplementado?.Numero.ToString() ?? "",
                Ordens = ordens,
                PrecoUnitario = cte.ValorFrete,
                Quantidade = 1,
                ValorTotal = cte.ValorFrete
            };

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.NotaDebitoV2>()
                { notaDebito },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "NotaDebito_V4_Item.rpt",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito.
                            NotaDebitoVersao04Item>() { itens }
                }
            }
        };
    }
}