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

[UseReportType(ReportType.MicDta)]
public class MicDtaReport : ReportBase
{
    public MicDtaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        int codigoMicdta = extraData.GetValue<int>("CodigoMicdta");
        int codigoCargaIntegracao = extraData.GetValue<int>("CodigoCargaIntegracao");

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaMICDTA repositorioCargaMICDTA = new Repositorio.Embarcador.Cargas.CargaMICDTA(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
        Dominio.Entidades.Embarcador.Cargas.CargaMICDTA micdta = repositorioCargaMICDTA.BuscarPorCodigo(codigoMicdta, false);
        Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repositorioCargaCargaIntegracao.BuscarPorCodigo(codigoCargaIntegracao);
        Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
        Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados?.FirstOrDefault() ?? null;
        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault() ?? null;
        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido?.Pedido;
        Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = cargaPedido.ApoliceSeguroAverbacao?.FirstOrDefault()?.ApoliceSeguro ?? null;
        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = carga.CargaCTes?.FirstOrDefault()?.CTe ?? null;
        List<Dominio.Entidades.Cliente> listaFronteiras = repositorioPedidoFronteira.BuscarFronteirasPorPedido(pedido.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorCarga(carga);

        Dominio.Entidades.Cliente fronteiraOrigemImprimir = null;
        Dominio.Entidades.Cliente fronteiraDestinoImprimir = null;

        if (listaFronteiras.Count > 0)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem = repositorioCargaRotaFretePontosPassagem.BuscarPorCargaOrdenado(carga.Codigo);

            List<Dominio.Entidades.Cliente> listaFronteirasRoteirizadas = ValidarPontosPassagemFronteiras(listaFronteiras, cargaRotaFretePontosPassagem);

            fronteiraOrigemImprimir = listaFronteirasRoteirizadas.FirstOrDefault();
            fronteiraDestinoImprimir = listaFronteirasRoteirizadas.LastOrDefault();
        }
        else if (pedido.Fronteira != null)
        {
            fronteiraOrigemImprimir = pedido.Fronteira;
            fronteiraDestinoImprimir = pedido.Fronteira;
        }

        bool cargaDeslocamentoVazio = carga?.TipoOperacao?.DeslocamentoVazio ?? false;

        Dominio.Entidades.Localidade origem;
        string fronteiraOrigem = string.Empty;
        if ((cargaPedido.Origem?.Pais?.Abreviacao ?? "") == "BR")
        {
            origem = fronteiraOrigemImprimir.Localidade;
            fronteiraOrigem = fronteiraOrigemImprimir?.Nome ?? string.Empty;
        }
        else
            origem = pedido.Origem;

        if (origem == null)
            origem = pedido.Origem;

        Dominio.Entidades.Localidade destino;
        if ((cargaPedido.Destino?.Pais?.Abreviacao ?? "") == "BR")
            destino = fronteiraDestinoImprimir.Localidade;
        else
            destino = pedido.Destino;

        Dominio.Entidades.Cliente remetente = pedido.Remetente;
        Dominio.Entidades.Cliente destinatario = pedido.Destinatario;
        Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
        Dominio.Entidades.Cliente filial = repositorioCliente.BuscarPorCPFCNPJ(carga.Filial.CNPJ.ToDouble());

        string cnpjRemetente = remetente != null ? remetente.Tipo.Equals("E") ? "CUIT: " + remetente.NumeroCUITRUT : "CNPJ NUMBER " + remetente.CPF_CNPJ_Formatado : string.Empty;
        string cnpjDestinatario = destinatario != null ? destinatario.Tipo.Equals("E") ? "CUIT: " + destinatario.NumeroCUITRUT : "CNPJ NUMBER " + destinatario.CPF_CNPJ_Formatado : string.Empty;
        string cnpjTomador = tomador != null ? tomador.Tipo.Equals("E") ? "CUIT: " + tomador.NumeroCUITRUT : "CNPJ NUMBER " + tomador.CPF_CNPJ_Formatado : string.Empty;
        string cnpjFilial = filial != null ? filial.Tipo.Equals("E") ? filial.NumeroCUITRUT : filial.CPF_CNPJ_Formatado : string.Empty;

        (string valorDoFrete, string valorDoSeguro, string valorTotalDaMercadoria) = ObterValoresFOTFreteESeguro(carga, cte, cargasPedido);
        (string tipoVolumes, string tipoVolumesCampo1, string TipoVolumesCampo2) = SetarEmbalagemEVolumes(cargaPedido.TipoEmbalagem, cargaDeslocamentoVazio);
        (string qtdVolume, string pesoBruto) = SetarPesoEVolumeNotas(cargaDeslocamentoVazio, carga.Codigo);
        (string cnpjProprietarioVeiulo, string dadosProprietarioVeiculo) = ObterDadosProprietarioVeiculo(veiculo, carga.EmpresaSubcontrada, carga.Empresa);

        Dominio.Relatorios.Embarcador.DataSource.Cargas.MicDta.MICDTA micDta = new Dominio.Relatorios.Embarcador.DataSource.Cargas.MicDta.MICDTA()
        {
            DataEmissao = micdta.DataEmissao.ToDateString(),

            CnpjTransportador = carga.Empresa.CNPJ_SemFormato,
            Transportador = carga.Empresa.RazaoSocial,
            EnderecoTransportador =
                    $"{carga.Empresa.Endereco} {carga.Empresa.Numero} {carga.Empresa.Complemento} - {carga.Empresa.Localidade.Descricao} {carga.Empresa.Localidade.Pais.Descricao}" +
                        (apoliceSeguro != null ? (Environment.NewLine + $"N. Seguro: {apoliceSeguro.NumeroApolice} Venc. Seguro: {apoliceSeguro.FimVigencia.ToDateString()}") : string.Empty),
            Remetente = cargaDeslocamentoVazio ? "***EN LASTRE***   *** EN LASTRE***" : remetente.Nome,
            EnderecoRemetente = cargaDeslocamentoVazio
                    ? "***EN LASTRE***   *** EN LASTRE***"
                    : $"{remetente.Endereco} {remetente.Numero} {remetente.Complemento} - {remetente.Localidade.Descricao} {remetente.Localidade.Pais.Descricao} {cnpjRemetente}",
            Destinatario = cargaDeslocamentoVazio ? "***EN LASTRE***   *** EN LASTRE***" : destinatario.Nome,
            EnderecoDestinatario = cargaDeslocamentoVazio
                    ? "***EN LASTRE***   *** EN LASTRE***"
                    : $"{destinatario.Endereco} {destinatario.Numero} {destinatario.Complemento} - {destinatario.Localidade.Descricao} {destinatario.Localidade.Pais?.Descricao} {cnpjDestinatario}",
            Tomador = cargaDeslocamentoVazio ? "***EN LASTRE***   *** EN LASTRE***" : tomador.Nome,
            EnderecoTomador = cargaDeslocamentoVazio
                    ? "***EN LASTRE***   *** EN LASTRE***"
                    : $"{tomador.Endereco} {tomador.Numero} {tomador.Complemento} - {tomador.Localidade.Descricao} {tomador.Localidade.Pais.Descricao} {cnpjTomador}",

            Veiculo = veiculo?.Placa,
            AnoVeiculo = veiculo?.AnoFabricacao.ToString(),
            MarcaVeiculo = veiculo?.Marca?.Descricao ?? string.Empty,
            ChassiVeiculo = veiculo?.Chassi,
            CapacidadeTracaoVeiculo = veiculo?.CapacidadeKG > 0 ? (veiculo?.CapacidadeKG / 1000).ToString() : 1.ToString(),
            CNPJProprietarioVeiculo = cnpjProprietarioVeiulo,

            DadosProprietarioVeiculo = dadosProprietarioVeiculo,

            Reboque = reboque?.Placa ?? string.Empty,
            ReboqueCheck = carga.ModeloVeicularCarga?.Tipo == TipoModeloVeicularCarga.Reboque && (!carga.ModeloVeicularCarga?.TipoSemirreboque ?? false) ? "X" : string.Empty,
            SemirreboqueCheck = (carga.ModeloVeicularCarga?.TipoSemirreboque ?? false) ? "X" : string.Empty,
            Numero = ObterNumero(micdta, cargaCargaIntegracao, carga),
            NumeroConhecimento = ObterNumeroConhecimento(cte, remetente, carga, cargaDeslocamentoVazio),
            Origem = $"{fronteiraOrigem} - {origem.Descricao} - {origem.Pais.Descricao}",
            Destino = $"{(destino?.Descricao ?? "")} - {(destino?.Pais?.Descricao ?? "")}",
            SiglaEstrangeira = cargaDeslocamentoVazio ? "" : cargaPedido.Moeda?.ObterSiglaEstrangeira() ?? string.Empty,
            AlfandegaDestino = cargaDeslocamentoVazio ? "" : $"{fronteiraDestinoImprimir.NomeFantasia ?? ""}",
            CodigoAlfandegaDestino = cargaDeslocamentoVazio ? "" : string.Join("   ", fronteiraDestinoImprimir.CodigoAduanaDestino.ToCharArray(0, Math.Min(fronteiraDestinoImprimir.CodigoAduanaDestino.Length, 7)) ?? new char[0]),
            CodigoAlfandega = cargaDeslocamentoVazio ? "" : string.Join("   ", fronteiraOrigemImprimir.CodigoAduaneiro.ToCharArray(0, Math.Min(fronteiraOrigemImprimir.CodigoAduaneiro.Length, 5)) ?? new char[0]),
            PaisOrigemMercadorias = cargaDeslocamentoVazio ? "" : pedido.Origem.Pais.Descricao,
            ValorFOT = cargaDeslocamentoVazio ? "" : valorTotalDaMercadoria,
            ValorFrete = cargaDeslocamentoVazio ? "" : valorDoFrete,
            ValorSeguro = cargaDeslocamentoVazio ? "" : valorDoSeguro,
            QuantidadeVolumes = qtdVolume,
            TipoVolumes = tipoVolumes,
            TipoVolumesCampo1 = tipoVolumesCampo1,
            TipoVolumesCampo2 = TipoVolumesCampo2,
            PesoBruto = pesoBruto,
            NumeroLacres = cargaDeslocamentoVazio ? "***EN LASTRE***   *** EN LASTRE***" : string.Join(", ", carga.Lacres.Select(o => o.Numero)),
            DescricaoMercadorias = cargaDeslocamentoVazio ? "***EN LASTRE***   *** EN LASTRE***" : cargaPedido.DetalheMercadoria ?? string.Empty,
            DocumentosAnexos = cargaDeslocamentoVazio ? "" : string.IsNullOrWhiteSpace(cte.ObservacoesGerais) ? cargaPedido.Pedido.ObservacaoCTe : cte.ObservacoesGerais,
            TransitoAduaneiroSim = !cargaPedido.TransitoAduaneiro.HasValue || cargaPedido.TransitoAduaneiro == TransitoAduaneiro.Sim ? "X" : string.Empty,
            TransitoAduaneiroNao = cargaPedido.TransitoAduaneiro == TransitoAduaneiro.Nao ? "X" : string.Empty,
            DtaRotaPrazoDeTransporte = cargaPedido.DtaRotaPrazoTransporte ?? string.Empty,
            NomeCPFCNPJTransportador = $"{filial?.Nome} {cnpjFilial}"
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.MicDta.MICDTA>() { micDta }
        };

        byte[] arquivo = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Integracoes\MICDTA.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, arquivo);
    }

    private string ObterNumero(Dominio.Entidades.Embarcador.Cargas.CargaMICDTA micdta, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
    {
        if (micdta != null)
            return micdta.Numero;

        if (cargaCargaIntegracao != null && !string.IsNullOrWhiteSpace(cargaCargaIntegracao.NumeroMICDTA))
            return cargaCargaIntegracao.NumeroMICDTA;

        return carga.CodigoCargaEmbarcador;
    }

    private string ObterNumeroConhecimento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool cargaDeslocamentoVazio)
    {
        if (cargaDeslocamentoVazio)
            return string.Empty;

        if (cte != null && !string.IsNullOrWhiteSpace(cte.NumeroCRT))
            return cte.NumeroCRT;

        return ($"{remetente.Localidade.Pais.Abreviacao}.{(carga.Empresa.NumeroCertificadoIdoneidade ?? string.Empty).PadLeft(3, '0')}.{string.Format(@"{0:000\.000}", cte?.Numero ?? 0)}");
    }

    private (string valorDoFrete, string valorADValorem, string valorTotalDaMercadoria) ObterValoresFOTFreteESeguro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
    {
        Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new(_unitOfWork);
        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = carga.Moeda;
        string siglaMoeda = moeda?.ObterSigla() ?? "";
        decimal valorFOT = 0m, valorCotacaoMoeda = 0m, valorFrete = 0m, valorTotalMercadoria = 0m, valorOutrosDecimal = 0m, valorSeguroDecimal = 0m;
        string valorDoFrete = string.Empty, valorDoSeguro = string.Empty;
        string valorFreteCrt = string.Empty, valorSeguro = string.Empty;

        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoPago = cargasPedido.Where(o => o.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago).Select(o => o.Pedido).ToList();
        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoAPagar = cargasPedido.Where(o => o.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar).Select(o => o.Pedido).ToList();
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFretes = repCargaPedidoComponentesFrete.BuscarPorCarga(carga.Codigo, carga.EmpresaFilialEmissora != null);

        valorCotacaoMoeda = cte.ValorCotacaoMoeda ?? 0m;
        valorFOT = cte.ValorTotalMercadoria / (valorCotacaoMoeda > 0m ? valorCotacaoMoeda : 1m);

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteFrete in componentesFretes)
        {
            decimal valorComponente = (moeda == MoedaCotacaoBancoCentral.Real) ? componenteFrete.ValorComponente : componenteFrete.ValorTotalMoeda ?? componenteFrete.ValorComponente;

            if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                valorSeguroDecimal += valorComponente;
            else
                valorOutrosDecimal += valorComponente;
        }

        if (moeda == MoedaCotacaoBancoCentral.Real)
        {
            valorFreteCrt = carga.ValorFrete > 0 ? carga.ValorFrete.ToString("n2") : string.Empty;
            valorSeguro = valorSeguroDecimal > 0 ? valorSeguroDecimal.ToString("n2") : string.Empty;
            valorFrete = carga.ValorFreteAPagar;
        }
        else if (moeda == MoedaCotacaoBancoCentral.DolarVenda)
        {
            decimal valorFreteCrtTotalDecimal = cte.ValorTotalMoeda.HasValue ? cte.ValorTotalMoeda.Value : 0m;

            valorFreteCrt = valorFreteCrtTotalDecimal > 0 && valorFreteCrtTotalDecimal > valorOutrosDecimal ? (valorFreteCrtTotalDecimal - valorOutrosDecimal - valorSeguroDecimal).ToString("n2") : string.Empty;
            valorSeguro = valorSeguroDecimal > 0 ? valorSeguroDecimal.ToString("n2") : string.Empty;
            valorFrete = cte?.ValorTotalMoeda ?? 0m;
        }

        if (pedidosTipoPagamentoPago.Any())
        {
            valorDoFrete = valorFreteCrt;
            valorDoSeguro = valorSeguro;
            valorTotalMercadoria = valorFOT + valorFrete;
        }

        else if (pedidosTipoPagamentoAPagar.Any())
        {
            valorDoFrete = valorFreteCrt;
            valorDoSeguro = valorSeguro;
            valorTotalMercadoria = valorFOT;
        }

        return (valorDoFrete, valorDoSeguro, $"{siglaMoeda}{valorTotalMercadoria.ToString("n2")}");
    }

    private List<Dominio.Entidades.Cliente> ValidarPontosPassagemFronteiras(List<Dominio.Entidades.Cliente> listaFronteiras, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem)
    {
        if (listaFronteiras.Count == 1 || cargaRotaFretePontosPassagem.Count == 0)
        {
            return listaFronteiras;
        }

        List<Dominio.Entidades.Cliente> listaFronteirasOrdenadas = new List<Dominio.Entidades.Cliente>();
        foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem item in cargaRotaFretePontosPassagem)
        {
            if (listaFronteiras.Contains(item.Cliente))
                listaFronteirasOrdenadas.Add(item.Cliente);
        }

        return listaFronteirasOrdenadas;
    }

    private (string tipoVolumes, string tipoVolumeCampo1, string tipoVolumeCampo2) SetarEmbalagemEVolumes(Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem, bool cargaDeslocamentoVazio)
    {
        if (cargaDeslocamentoVazio || tipoEmbalagem == null)
            return (string.Empty, string.Empty, string.Empty);

        if (!tipoEmbalagem.CodigoIntegracao.IsSomenteNumeros())
            return (tipoEmbalagem.Descricao, string.Empty, string.Empty);

        string tipoVolumeCampo2 = string.Empty, tipoVolumeCampo1 = string.Empty;

        string somenteNumeros = tipoEmbalagem.CodigoIntegracao.ObterSomenteNumeros();

        int quantidadeNumeros = somenteNumeros.Length;

        if (quantidadeNumeros > 0)
            tipoVolumeCampo2 = somenteNumeros[quantidadeNumeros - 1].ToString();

        if (quantidadeNumeros > 1)
            tipoVolumeCampo1 = somenteNumeros[quantidadeNumeros - 2].ToString();

        return (tipoEmbalagem.Descricao, tipoVolumeCampo1, tipoVolumeCampo2);
    }

    private (string qtdVolume, string pesoBruto) SetarPesoEVolumeNotas(bool deslocamentoVazio, int codigoCarga)
    {
        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

        if (deslocamentoVazio)
            return (string.Empty, string.Empty);

        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repositorioPedidoXMLNotaFiscal.BuscarNotasPorCargaETipoFatura(codigoCarga, false);

        int qtdVolume = notasFiscais.Sum(obj => obj.XMLNotaFiscal.Volumes);
        decimal pesoBruto = notasFiscais.Sum(obj => obj.XMLNotaFiscal.Peso);

        return ($"{qtdVolume}", $"{pesoBruto:n3}");
    }

    private (string cnpjProprietarioVeiculo, string empresaProprietarioVeiculo) ObterDadosProprietarioVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresaSubcontratada, Dominio.Entidades.Empresa transportador)
    {
        Dominio.Entidades.Empresa empresa = empresaSubcontratada ?? transportador;

        if (veiculo == null)
            return BuscarDadosEmpresa(empresa);

        if (veiculo.Tipo.Equals("T"))
            return BuscarDadosProprietarioVeiculo(veiculo);

        return BuscarDadosEmpresa(empresa);
    }

    private (string cnpj, string empresa) BuscarDadosEmpresa(Dominio.Entidades.Empresa empresa)
    {
        if (empresa == null)
            return (string.Empty, string.Empty);

        string dados = FormatarDadosProprietario(
            empresa.RazaoSocial,
            empresa.Endereco,
            empresa.Numero,
            empresa.Complemento,
            empresa.Localidade?.Descricao,
            empresa.Localidade?.Pais?.Descricao
        );

        string cnpj = empresa.CNPJ_SemFormato ?? string.Empty;

        return (cnpj, dados);
    }

    private (string cnpj, string empresa) BuscarDadosProprietarioVeiculo(Dominio.Entidades.Veiculo veiculo)
    {
        if (veiculo.Proprietario == null)
            return (string.Empty, string.Empty);

        Dominio.Entidades.Cliente proprietario = veiculo.Proprietario;

        string dados = FormatarDadosProprietario(
            proprietario.Nome,
            proprietario.Endereco,
            proprietario.Numero,
            proprietario.Complemento,
            proprietario.Localidade?.Descricao,
            proprietario.Localidade?.Pais?.Descricao
        );

        string cnpj = proprietario.CPF_CNPJ_SemFormato ?? string.Empty;

        return (cnpj, dados);
    }

    private string FormatarDadosProprietario(string nome, string endereco, string numero, string complemento, string localidadeDescricao, string localidadePaisDescricao)
    {
        string dadosProprietario = string.Empty;

        if (!string.IsNullOrWhiteSpace(nome))
            dadosProprietario += $"{nome} - ";

        if (!string.IsNullOrWhiteSpace(endereco))
            dadosProprietario += $"{endereco} ";

        if (!string.IsNullOrWhiteSpace(numero))
            dadosProprietario += $"{numero} ";

        if (!string.IsNullOrWhiteSpace(complemento))
            dadosProprietario += $"{complemento} - ";

        if (!string.IsNullOrWhiteSpace(localidadeDescricao))
            dadosProprietario += $"{localidadeDescricao} ";

        if (!string.IsNullOrWhiteSpace(localidadePaisDescricao))
            dadosProprietario += $"{localidadePaisDescricao}";

        return dadosProprietario;
    }
}