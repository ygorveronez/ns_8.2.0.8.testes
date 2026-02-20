using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.FaturaPadrao)]
public class FaturaPadraoReport : ReportBase
{
    public FaturaPadraoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        int codigoContratoFrete = extraData.GetValue<int>("CodigoContratoFrete");
        int codigoControleGeracao = extraData.GetValue<int>("CodigoControleGeracao");
        var relatorioTemp = extraData.GetValue<string>("RelatorioTemp").FromJson<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();

        bool origemTelaRelatorio = extraData.GetValue<bool>("OrigemTelaRelatorio");
        
        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete;

        if (codigoCarga > 0)
            contratoFrete = repContratoFrete.BuscarPorCarga(codigoCarga);
        else
            contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete);

        if (contratoFrete == null)
            throw new ServicoException("Não foi encontrado registro de Contrato Frete");

        Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(_unitOfWork);
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS =  repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
        Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);
        Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfigContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(_unitOfWork);

        contratoFrete = repContratoFrete.BuscarPorCodigo(contratoFrete.Codigo);

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = codigoControleGeracao > 0 ? repRelatorioControleGeracao.BuscarPorCodigo(codigoControleGeracao) : new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao();
        Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfigContratoFreteTerceiro.BuscarConfiguracaoPadrao();

        List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> contratoFreteCTes = repContratoFreteCTe.BuscarPorCargaFreteSubContratacao(contratoFrete.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentes = repCargaComponentesFrete.BuscarComponentesIncluirIntegralContratoFrete(contratoFrete.Carga?.Codigo ?? 0);
        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteCte> listaContratoFretesCTes = new List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteCte>();
        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete> ContratoFrete = ObterDadosContrato(contratoFrete, configuracaoTMS, usuario, _unitOfWork);

        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor> valores =
            new List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                {
                    Descricao = "Subcontratação de Terceiro: (+)", Valor = contratoFrete.ValorFreteSubcontratacao - (contratoFrete.TipoIntegracaoValePedagio != null
                                                                           && !contratoFrete.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato
                                                                           && (!contratoFrete.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false)
                                                                           && !(configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato) ? contratoFrete.ValorPedagio : 0m) -
                                                                           contratoFrete.ValoresAdicionais.Where(o =>
                                                                               o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal
                                                                               && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(o => o.Valor) +
                                                                           contratoFrete.ValoresAdicionais.Where(o =>
                                                                               o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal
                                                                               && o.TipoJustificativa ==  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(o => o.Valor)
                },
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                {
                    Descricao = contratoFrete.TipoIntegracaoValePedagio != null ? "Vale Pedágio:" : "Pedágio Pago: (+)",
                    Valor = contratoFrete.ValorPedagio
                },
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                    { Descricao = "Outros Descontos: (-)", Valor = contratoFrete.Descontos },
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                    { Descricao = "IRRF: (-)", Valor = contratoFrete.ValorIRRF },
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                    { Descricao = "INSS: (-)", Valor = contratoFrete.ValorINSS },
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                {
                    Descricao = "SEST/SENAT: (-)", Valor = contratoFrete.ValorSEST + contratoFrete.ValorSENAT
                }
            };

        if (contratoFrete.TarifaSaque > 0m)
            valores.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
            { Descricao = "Tarifa de Saque: (-)", Valor = contratoFrete.TarifaSaque });

        if (contratoFrete.TarifaTransferencia > 0m)
            valores.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
            { Descricao = "Tarifa de Transferência: (-)", Valor = contratoFrete.TarifaTransferencia });

        foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor valor in contratoFrete.ValoresAdicionais)
            valores.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
            {
                Descricao = valor.Justificativa.Descricao +
                            (valor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa
                                .Acrescimo
                                ? ": (+)"
                                : ": (-)"),
                Valor = valor.Valor
            });

        if (componentes != null && componentes.Count > 0)
        {
            foreach (var componenteCarga in componentes)
                valores.Add(new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteValor()
                {
                    Descricao = componenteCarga.ComponenteFrete?.Descricao ?? componenteCarga.OutraDescricaoCTe,
                    Valor = componenteCarga.ValorComponente
                });
        }


        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in contratoFrete.Carga.CargaCTes)
        {
            Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteCte contratoFreteCteDS =
                new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFreteCte();

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTE.BuscarPorCTe(cargaCTe.CTe.Codigo, 0, 10);
            contratoFreteCteDS.Documentos = "";
            foreach (Dominio.Entidades.DocumentosCTE docCte in documentosCTe)
            {
                string numeroDocumento = docCte.Numero.ToString();

                if (docCte.NumeroModelo == "55" && !string.IsNullOrWhiteSpace(docCte.ChaveNFE) &&
                    docCte.ChaveNFE.Length == 44)
                    numeroDocumento = docCte.ChaveNFE.Substring(25, 9);

                if (docCte.DataEmissao > DateTime.MinValue)
                    numeroDocumento += " - " + docCte.DataEmissao.ToString("dd/MM/yyyy");

                contratoFreteCteDS.Documentos += numeroDocumento + "; ";
            }

            contratoFreteCteDS.Codigo = cargaCTe.Codigo;
            contratoFreteCteDS.NumeroCTe = cargaCTe.CTe.Numero.ToString();

            if (cargaCTe.CTe.DataEmissao.HasValue && cargaCTe.CTe.DataEmissao.Value > DateTime.MinValue)
                contratoFreteCteDS.NumeroCTe += " - " + cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy");

            contratoFreteCteDS.PesoTotal = cargaCTe.CTe.Peso;

            if (contratoFreteCteDS.PesoTotal <= 0m)
                contratoFreteCteDS.PesoTotal = cargaCTe.CTe.XMLNotaFiscais.Sum(o => o.Peso);

            contratoFreteCteDS.ValorMercadoria = cargaCTe.CTe.ValorTotalMercadoria;
            contratoFreteCteDS.TipoCarga = cargaCTe.Carga.TipoDeCarga?.Descricao ?? "";

            listaContratoFretesCTes.Add(contratoFreteCteDS);
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
        IdentificacaoCamposRPT.GroupFooterSection = "";
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = _servicoRelatorioReportService.CriarRelatorio(
            relatorioControleGeracao, relatorioTemp, ContratoFrete, _unitOfWork, IdentificacaoCamposRPT,
            new List<KeyValuePair<string, dynamic>>()
            {
                new KeyValuePair<string, dynamic>("ContratoFreteCTe.rpt", listaContratoFretesCTes),
                new KeyValuePair<string, dynamic>("ContratoFreteValor.rpt", valores)
            }, false);

        if (!origemTelaRelatorio)
        {
            byte[] pdfContent = RelatorioSemPadraoReportService.ObterBufferReport(report, TipoArquivoRelatorio.PDF);
            return PrepareReportResult(FileType.PDF, pdfContent);
        }

        string paginaRelatorio = extraData.GetValue<string>("PaginaRelatorio");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros, "", "");

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, paginaRelatorio, _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }

    public List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete> ObterDadosContrato(
        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete,
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Usuario usuario,
        Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

        Repositorio.Cliente repGrupoPessoas = new Repositorio.Cliente(unitOfWork);
        Dominio.Entidades.Cliente pessoa = new Dominio.Entidades.Cliente(); 

        List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete> contratosFretes =
            new List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete>();

        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro =
            Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(contratoFrete.TransportadorTerceiro, unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = contratoFrete.Carga;

        Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete contratoDS =
            new Dominio.Relatorios.Embarcador.DataSource.Terceiros.ContratoFrete();
        contratoDS.AgenciaTerceiro = contratoFrete.TransportadorTerceiro.Agencia +
                                     (!string.IsNullOrWhiteSpace(contratoFrete.TransportadorTerceiro.DigitoAgencia)
                                         ? "-" + contratoFrete.TransportadorTerceiro.DigitoAgencia
                                         : string.Empty);
        contratoDS.NumeroContrato = contratoFrete.NumeroContrato;
        string anoModelo = carga.Veiculo?.AnoModelo > 0 ? carga.Veiculo?.AnoModelo.ToString() : "";
        string anoFabricacao = carga.Veiculo?.AnoFabricacao > 0 ? carga.Veiculo?.AnoFabricacao.ToString() : "";
        contratoDS.AnoModelo = anoFabricacao + "/" + anoModelo;
        contratoDS.BairoEmpresa = carga.Empresa?.Bairro ?? "";
        contratoDS.BancoTerceiro = contratoFrete.TransportadorTerceiro.Banco != null
            ? contratoFrete.TransportadorTerceiro.Banco.Descricao
            : "";
        contratoDS.CidadeTerceiro = contratoFrete.TransportadorTerceiro.Localidade.DescricaoCidadeEstado;
        contratoDS.OutrosDescontos = contratoFrete.Descontos + contratoFrete.ValorOutrosAdiantamento;
        contratoDS.Adiantamento = contratoFrete.ValorAdiantamento;
        contratoDS.Abastecimento = contratoFrete.ValorAbastecimento;

        string CPFmotoristas = "";
        string NomeMotoristas = "";
        string CNHMotoristas = "";
        string NumeroCartaoMotorista = "";
        string TipoChavePixMotorista = "";
        string ChavePixMotorista = "";

        Dominio.Entidades.Usuario ultimoMotorista =
            carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.Last() : null;
        if (carga.Motoristas != null && carga.Motoristas.Count > 0)
        {
            foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
            {
                double cpfCnpjMotorista = motorista.CPF != null ? double.Parse(motorista.CPF):0;
                pessoa = null; 

                if(cpfCnpjMotorista > 0)
                {
                    pessoa = repGrupoPessoas.BuscarPorCPFCNPJ(cpfCnpjMotorista);
                }

                CNHMotoristas += motorista.NumeroHabilitacao;
                CPFmotoristas += motorista.CPF_Formatado;
                NomeMotoristas += motorista.Nome;
                NumeroCartaoMotorista += motorista.NumeroCartao;
                TipoChavePixMotorista += pessoa != null && pessoa.TipoChavePix.HasValue ? pessoa.TipoChavePix.Value.ObterDescricao() : string.Empty;
                ChavePixMotorista += pessoa != null && pessoa.ChavePix_Formatado != null ? pessoa.ChavePix_Formatado : string.Empty;

                if (ultimoMotorista != null && ultimoMotorista.Codigo != motorista.Codigo)
                {
                    CNHMotoristas += "; ";
                    CPFmotoristas += "; ";
                    NomeMotoristas += "; ";
                    NumeroCartaoMotorista += "; ";
                    TipoChavePixMotorista += "; ";
                    ChavePixMotorista += "; ";
                }
            }
        }

        string placas = "";
        string modeloVeicular = "";
        if (carga.Veiculo != null)
        {
            contratoDS.RNTRC = carga.Veiculo.RNTRC.ToString();
            contratoDS.Renavam = carga.Veiculo.Renavam;
            placas += carga.Veiculo.Placa;
            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                {
                    placas += " " + reboque.Placa;
                    if (reboque.ModeloVeicularCarga != null &&
                        (reboque.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral 
                         || reboque.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque))
                        modeloVeicular = reboque.ModeloVeicularCarga?.Descricao ?? "";
                }
            }

            if (string.IsNullOrWhiteSpace(modeloVeicular))
            {
                if (carga.Veiculo.ModeloVeicularCarga != null)
                    modeloVeicular = carga.Veiculo.ModeloVeicularCarga.Descricao;
                else
                    modeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? "";
            }
        }
        else
        {
            modeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? "";
        }

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();
        contratoDS.CPFMotorista = CPFmotoristas;
        contratoDS.CNHMotorista = CNHMotoristas;
        contratoDS.ComplementoEmpresa = carga.Empresa?.Complemento ?? "";
        contratoDS.ContaTerceiro = contratoFrete.TransportadorTerceiro.NumeroConta;
        contratoDS.NumeroCartaoTerceiro = modalidadeTerceiro?.NumeroCartao ?? "";
        contratoDS.EnderecoEmpresa = carga.Empresa.Endereco + ", " + carga.Empresa.Numero;
        contratoDS.CPF_CNPJTerceiro = contratoFrete.TransportadorTerceiro.CPF_CNPJ_Formatado;
        contratoDS.Destinatario = cargaPedido.Pedido.Destinatario.Nome;
        contratoDS.Empresa = carga.Empresa.RazaoSocial;
        contratoDS.INSS = contratoFrete.ValorINSS;
        contratoDS.IRRF = contratoFrete.ValorIRRF;
        contratoDS.LeiSubcontratacao =
            "Constitui objeto do presente contrato a prestação de serviços de transportes de cargas pela SUBCONTRATADA, na forma de lei n0 11.442/2007, sem subordinação ou dependência, para qualquer localidade do território nacional ou internacional, via terrestre sob sua responsabilidade";
        contratoDS.LiquidoSemAdiantamento = contratoFrete.ValorLiquidoSemAdiantamento;

        contratoDS.LocalidadeEmpresa = carga.Empresa.Localidade.DescricaoCidadeEstado;
        contratoDS.ModeloVeicula = modeloVeicular;
        contratoDS.Motorista = NomeMotoristas;
        contratoDS.NumeroCartaoMotorista = NumeroCartaoMotorista;
        contratoDS.TipoChavePixMotorista = TipoChavePixMotorista;
        contratoDS.ChavePixMotorista = ChavePixMotorista;
        contratoDS.MunicipioDestino = cargaPedido.Destino.DescricaoCidadeEstado;
        contratoDS.MunicipioOrigem = cargaPedido.Origem.DescricaoCidadeEstado;
        contratoDS.NomeTerceiro = contratoFrete.TransportadorTerceiro.Nome;
        contratoDS.NumeroEmpresa = carga.Empresa.Numero;
        contratoDS.ObservacaoAdiantamento = Servicos.Embarcador.Terceiros.ContratoFrete.ObterObservacao(contratoFrete);
        contratoDS.TextoAdicional = contratoFrete.TextoAdicionalContratoFrete;
        contratoDS.Operador = carga.Operador?.Nome.ToString();
        contratoDS.EnderecoTerceiro = contratoFrete.TransportadorTerceiro.Endereco;
        decimal valorCarga = carga.ValorFrete;

        if (contratoFrete.Transbordo != null) //todo: rever essa regra.
        {
            valorCarga = contratoFrete.Transbordo.CargaCTesTransbordados.Sum(obj => obj.CTe.ValorFrete);
        }

        contratoDS.ValorPedagio = contratoFrete.ValorPedagio;
        contratoDS.ValorDescarga = contratoFrete.Carga.Componentes.Where(o =>
            o.ComponenteFrete.TipoComponenteFrete ==
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA).Sum(o => o.ValorComponente);

        decimal saldo = valorCarga - contratoFrete.ValorFreteSubcontratacao;

        decimal percentualVariacao = 0m;

        if ((configuracaoTMS.ExibirVariacaoNegativaContratoFreteTerceiro || saldo > 0m) && valorCarga > 0m)
            percentualVariacao =
                (saldo + (configuracaoTMS.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro
                    ? contratoDS.ValorPedagio + contratoDS.ValorDescarga
                    : 0m)) * 100 / valorCarga;

        contratoDS.PercentualVariacao = Math.Round(percentualVariacao, 0, MidpointRounding.AwayFromZero).ToString();
        contratoDS.Placas = placas;
        contratoDS.Remetente = cargaPedido.Pedido.Remetente.Nome;
        contratoDS.SaldoReceber = contratoFrete.SaldoAReceber;
        contratoDS.SaldoVariacao = saldo + (configuracaoTMS.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro
            ? contratoDS.ValorPedagio + contratoDS.ValorDescarga
            : 0m);
        contratoDS.SEST_SENAT = contratoFrete.ValorSEST + contratoFrete.ValorSENAT;
        contratoDS.TelefoneEmpresa = carga.Empresa.Telefone;
        contratoDS.TelefoneTerceiro = contratoFrete.TransportadorTerceiro.Telefone1;
        contratoDS.Terceiro = contratoFrete.TransportadorTerceiro.Nome;
        string tipoConta = "";
        if (contratoFrete.TransportadorTerceiro.TipoContaBanco.HasValue)
        {
            if (contratoFrete.TransportadorTerceiro.TipoContaBanco.Value ==
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente)
                tipoConta = "C";
            else
                tipoConta = "P";
        }

        contratoDS.TipoContaTerceiro = tipoConta;
        contratoDS.TipoPagamento = cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago
            ? "Pago"
            : "A Pagar";
        contratoDS.TitulaContaTerceiro = contratoFrete.TransportadorTerceiro.Nome;
        contratoDS.UFEmpresa = carga.Empresa.Localidade?.Estado?.Sigla ?? "";
        contratoDS.ValorFreteIngresso = valorCarga;

        if (contratoFrete.TipoIntegracaoValePedagio == null)
            contratoDS.ValorFretePago = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio;
        else
            contratoDS.ValorFretePago = contratoFrete.ValorFreteSubcontratacao;

        contratoDS.ValorSubcontratacao = contratoFrete.ValorFreteSubcontratacao;
        contratoDS.VencimentoTerceiro = "";
        contratoDS.NumeroCarga = carga.CodigoCargaEmbarcador;
        contratoDS.DataEmissao = contratoFrete.DataEmissaoContrato;
        contratoDS.ExibirVariacao = !configuracaoTMS.NaoExibirVariacaoContratoFrete;
        contratoDS.ConsiderarPedagioDescargaVariacao =
            configuracaoTMS.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro;

        contratoDS.PossuiCIOT = carga.CargaCIOTs.Any(o =>
            o.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado);
        contratoDS.ProtocoloAutorizacaoCIOT = carga.CargaCIOTs.Select(o =>
                o.CIOT.ProtocoloAutorizacao + (!string.IsNullOrWhiteSpace(o.CIOT.Digito) ? "-" + o.CIOT.Digito : ""))
            .FirstOrDefault();
        contratoDS.NumeroCIOT = carga.CargaCIOTs.Select(o => o.CIOT.Numero).FirstOrDefault();

        Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado =
            repCargaPedido.BuscarCentroResultadoPorCarga(carga.Codigo);

        contratoDS.CentroResultado = centroResultado?.Descricao;

        contratosFretes.Add(contratoDS);
        return contratosFretes;
    }
}