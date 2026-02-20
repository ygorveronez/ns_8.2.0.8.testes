using System;
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

[UseReportType(ReportType.CIOT)]
public class CIOTReport : ReportBase
{
    public CIOTReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCIOT = extraData.GetValue<int>("CodigoCiot");
        Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
        Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidade =
            new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro =
            new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(_unitOfWork);

        Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao =
            repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete =
            repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

        //if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
        //{
        //    mensagemErro = "O contrato só pode ser gerado para um CIOT encerrado.";
        //    return null;
        //}

        Dominio.Entidades.Veiculo veiculo = ciot.CargaCIOT.FirstOrDefault()?.Carga?.Veiculo ??
                                            repVeiculo.BuscarPorProprietario(ciot.Transportador.CPF_CNPJ)
                                                .FirstOrDefault();
        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador =
            repModalidade.BuscarPorPessoa(ciot.Transportador.CPF_CNPJ);

        Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOT dsCIOT =
            new Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOT
            {
                AnoVeiculo = veiculo.AnoFabricacao,
                EstadoVeiculo = veiculo.Estado.Sigla + " - " + veiculo.Estado.Nome,
                PlacaVeiculo = veiculo.Placa,

                CidadeContratante = ciot.Contratante?.Localidade.DescricaoCidadeEstado,
                CNPJContratante = ciot.Contratante?.CNPJ_Formatado,
                EnderecoContratante = ciot.Contratante?.Endereco + ", " + ciot.Contratante?.Numero + " - " +
                                      ciot.Contratante?.Localidade.DescricaoCidadeEstado,
                TelefoneContratante = ciot.Contratante?.Telefone,
                NomeContratante = ciot.Contratante?.RazaoSocial,

                CPFCNPJContratado = ciot.Transportador.CPF_CNPJ_Formatado,
                EnderecoContratado = ciot.Transportador.Endereco + ", " + ciot.Transportador.Numero + " - " +
                                     ciot.Transportador.Localidade.DescricaoCidadeEstado,
                NomeContratado = ciot.Transportador.Nome,
                RNTRCContratado = modalidadeTransportador?.RNTRC ?? "",
                RGContratado = ciot.Transportador.IE_RG,
                TelefoneContratado = ciot.Transportador.Telefone1,

                RGMotorista = ciot.Motorista.RG,
                PISMotorista = ciot.Motorista.PIS,
                CNHMotorista = ciot.Motorista.NumeroHabilitacao,
                CPFMotorista = ciot.Motorista.CPF_Formatado,
                EnderecoMotorista = ciot.Motorista.Endereco + ", " + ciot.Motorista.NumeroEndereco + " - " +
                                    ciot.Motorista?.Localidade?.DescricaoCidadeEstado ?? "",
                Motorista = ciot.Motorista.Nome,
                NumeroCartaoMotorista = ciot.Motorista.NumeroCartao,
                TelefoneMotorista = ciot.Motorista.Telefone,

                NumeroCIOT = ciot.Numero + " / " + ciot.CodigoVerificador,
                ProtocoloAutorizacao = ciot.ProtocoloAutorizacao +
                                       (!string.IsNullOrWhiteSpace(ciot.Digito) ? "-" + ciot.Digito : "")
            };

        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesCIOT =
            ObterCTesDeCargaCIOT(ciot.CargaCIOT.ToList());

        decimal seguro = 0m;
        decimal pedagioDesconto = ciot.CargaCIOT.Where(o =>
                o.ContratoFrete.TipoIntegracaoValePedagio != null &&
                !o.ContratoFrete.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato &&
                !(o.ContratoFrete.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ??
                  false) &&
                !(configuracaoContratoFrete.NaoSubtrairValePedagioDoContrato))
            ?.Sum(o => (o.ContratoFrete?.ValorPedagio ?? 0m)) ?? 0m;
        decimal pedagio = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorPedagio ?? 0m);
        decimal descontos = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.Descontos ?? 0m);
        decimal tarifa = ciot.CargaCIOT.Sum(o =>
            (o.ContratoFrete?.TarifaSaque ?? 0m) + (o.ContratoFrete?.TarifaTransferencia ?? 0m));
        decimal acrescimos = ciot.CargaCIOT.Sum(o =>
            o.ContratoFrete.ValorTotalAcrescimoAdiantamento + o.ContratoFrete.ValorTotalAcrescimoSaldo);

        dsCIOT.ValorTotalFreteContratado = configuracaoContratoFrete.EmAcrescimoDescontoCiotNaoAlteraImpostos
            ? ciot.CargaCIOT.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m))
            : ciot.CargaCIOT.Sum(o => (o.ContratoFrete?.ValorFreteSubContratacaoTabelaFrete ?? 0m));
        dsCIOT.INSS = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorINSS ?? 0m);
        dsCIOT.IR = configuracaoContratoFrete.EmAcrescimoDescontoCiotNaoAlteraImpostos
           ? 0m
           : ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorIRRF ?? 0m);
        dsCIOT.SEST = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorSEST ?? 0m);
        dsCIOT.SENAT = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorSENAT ?? 0m);
        dsCIOT.Descontos = seguro + descontos + tarifa + pedagioDesconto;
        dsCIOT.Saldo = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.SaldoAReceber ?? 0m);
        dsCIOT.Adiantamento = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAdiantamento ?? 0m);
        dsCIOT.Abastecimento = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAbastecimento ?? 0m);
        dsCIOT.Acrescimos = acrescimos;

        dsCIOT.Operadora = ciot.Operadora;
        dsCIOT.DataEmissao = ciot.DataAbertura?.ToString("dd/MM/yyyy") ?? string.Empty;
        dsCIOT.Origem = "";
        dsCIOT.Destino = "";

        List<Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOT> ciots =
            new List<Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOT>() { dsCIOT };
        int qtdCTes = ctesCIOT?.Count > 0 ? ctesCIOT.Count : 1;

        List<Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTCTe> ctes = (from obj in ciot.CTes
                                                                            select new Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTCTe()
                                                                            {
                                                                                CodigoCTe = obj.CargaCTe.CTe.Codigo,
                                                                                Especie = "KG",
                                                                                Mercadoria = obj.CargaCTe.CTe.ProdutoPredominante,
                                                                                NumeroCTe = obj.CargaCTe.CTe.Numero.ToString() + "/" + obj.CargaCTe.CTe.Serie.Numero.ToString(),
                                                                                NumeroNotaFiscal = string.Join("/", from doc in obj.CargaCTe.CTe.Documentos select doc.Numero),
                                                                                Quantidade = (from qtd in obj.CargaCTe.CTe.QuantidadesCarga
                                                                                              where qtd.UnidadeMedida == "01"
                                                                                              select qtd.Quantidade).Sum(),
                                                                                ValorFrete = configuracao.GerarContratoTerceiroSemInformacaoDoFrete
                                                                                    ? dsCIOT.ValorTotalFreteContratado
                                                                                    : obj.CargaCTe.CTe.ValorAReceber,
                                                                                ValorMercadoria = obj.CargaCTe.CTe.ValorTotalMercadoria,
                                                                                Adiantamento = dsCIOT.Adiantamento / qtdCTes,
                                                                                Seguro = seguro / qtdCTes,
                                                                                Tarifa = tarifa / qtdCTes,
                                                                                Pedagio = pedagio / qtdCTes,
                                                                                IRRF = dsCIOT.IR / qtdCTes,
                                                                                INSS = dsCIOT.INSS / qtdCTes,
                                                                                SEST = dsCIOT.SEST / qtdCTes,
                                                                                SENAT = dsCIOT.SENAT / qtdCTes,
                                                                                Descontos = dsCIOT.Descontos / qtdCTes
                                                                            }).ToList();

        if (ctesCIOT?.Count > 0)
        {
            foreach (var cteCIOT in ctesCIOT)
            {
                if (ctes == null || ctes.Count == 0 || !ctes.Any(c => c.CodigoCTe == cteCIOT.Codigo))
                {
                    ctes.Add(new Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTCTe()
                    {
                        Especie = "KG",
                        Mercadoria = cteCIOT.ProdutoPredominante,
                        NumeroCTe = cteCIOT.Numero.ToString() + "/" + cteCIOT.Serie.Numero.ToString(),
                        NumeroNotaFiscal = string.Join("/", from doc in cteCIOT.Documentos select doc.Numero),
                        Quantidade = (from qtd in cteCIOT.QuantidadesCarga
                                      where qtd.UnidadeMedida == "01"
                                      select qtd.Quantidade).Sum(),
                        ValorFrete = configuracao.GerarContratoTerceiroSemInformacaoDoFrete
                            ? dsCIOT.ValorTotalFreteContratado
                            : cteCIOT.ValorAReceber,
                        ValorMercadoria = cteCIOT.ValorTotalMercadoria,
                        Adiantamento = dsCIOT.Adiantamento / ctesCIOT.Count,
                        Seguro = seguro / ctesCIOT.Count,
                        Tarifa = tarifa / ctesCIOT.Count,
                        Pedagio = pedagio / ctesCIOT.Count,
                        IRRF = dsCIOT.IR / ctesCIOT.Count,
                        INSS = dsCIOT.INSS / ctesCIOT.Count,
                        SEST = dsCIOT.SEST / ctesCIOT.Count,
                        SENAT = dsCIOT.SENAT / ctesCIOT.Count,
                        Descontos = dsCIOT.Descontos / ctesCIOT.Count,
                    });
                }
            }
        }

        List<Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTPedido> pedidos =
            new List<Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTPedido>();

        bool exibirPedidos = configuracaoContratoFrete.ExibirPedidosImpressaoContratoFrete;
        if (exibirPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargasCIOT =
                ObterPedidosDeCargaCIOT(ciot.CargaCIOT.ToList());

            pedidos = (from obj in pedidosCargasCIOT
                       select new Dominio.Relatorios.Embarcador.DataSource.CIOT.CIOTPedido()
                       {
                           CodigoPedido = obj.Codigo,
                           Cliente = obj.Destinatario.Descricao,
                           LocalidadeCliente = obj.Destinatario.Localidade.DescricaoCidadeEstado,
                           DataPrevisaoEntrega = obj.PrevisaoEntrega?.ToDateTimeString()
                       }).ToList();
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = ciots,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "CIOT_Documentos.rpt",
                        DataSet = ctes
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Pedidos",
                        DataSet = pedidos
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro()
                    {
                        NomeParametro = "ExibirPedidos",
                        ValorParametro = exibirPedidos ? "Sim" : "Não"
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Documentos\CIOT.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }


    private static List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTesDeCargaCIOT(List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargasCIOT)
    {
        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesCIOT = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

        foreach (var cargaCIOT in cargasCIOT)
            ctesCIOT.AddRange((from o in cargaCIOT.Carga.CargaCTes select o.CTe).ToList());

        return ctesCIOT.Distinct().ToList();
    }

    private static List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosDeCargaCIOT(List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargasCIOT)
    {
        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

        foreach (var cargaCIOT in cargasCIOT)
            pedidos.AddRange((from o in cargaCIOT.Carga.Pedidos select o.Pedido).ToList());

        return pedidos.Distinct().ToList();
    }
}