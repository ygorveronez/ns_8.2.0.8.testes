using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.MDFe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.MDFe;

public class MDFeInformacoesPagamento : ServicoBase
{

    public MDFeInformacoesPagamento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
    {
    }


    public void GerarInformacoesBancariasMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
    {
        Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(_unitOfWork);
        Repositorio.Embarcador.MDFE.MDFePagamentoComponente repPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);
        Repositorio.Embarcador.MDFE.MDFePagamentoParcela repPagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaCIOT repcargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(_unitOfWork);
        Repositorio.ManifestoEletronicoDeDocumentosFiscais repManifestoEletronicoDeDocumentosFiscais = new(_unitOfWork);

        Dominio.Entidades.VeiculoMDFe veiculo = new Repositorio.VeiculoMDFe(_unitOfWork).BuscarPorMDFe(mdfe.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repcargaCIOT.BuscarPorCarga(carga.Codigo);
        Dominio.Entidades.Empresa contratante = veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1" ? cargaCIOT?.CIOT?.Contratante ?? mdfe.Empresa : mdfe.Empresa;
        Dominio.Entidades.Cliente proprietario = new Repositorio.Cliente(_unitOfWork).BuscarPorCPFCNPJSemFetch(veiculo.CPFCNPJProprietario.ToDouble());
        Dominio.Entidades.Global.CargaInformacoesBancarias cargaInformacoesBancarias = new Repositorio.Embarcador.Cargas.CargaInformacoesBancarias(_unitOfWork).BuscarPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> complementosFrete = repCargaComplementoFrete.BuscarPorCargaSemComponenteCompoeFreteValor(carga.Codigo);

        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentes = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork).BuscarTodosPorCarga(carga.Codigo);
        decimal valorPedagio = new Repositorio.ValePedagioMDFe(_unitOfWork).BuscarValorValePedagioPorMDFe(mdfe.Codigo);
        decimal valorOutros = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork).BuscarPorCargaValorOutros(carga?.Codigo ?? 0, false) + (complementosFrete.IsNullOrEmpty() ? 0 : complementosFrete.Sum(x => x.ValorComplemento));
        decimal valorICMS = componentes.IsNullOrEmpty() ? 0 : componentes.Where(x => x.TipoComponenteFrete == TipoComponenteFrete.ICMS)?.Sum(x => x.ValorComponente) ?? 0;
        
        decimal valorFrete;
        if ((cargaCIOT?.ValorFrete ?? 0) > 0)
            valorFrete = cargaCIOT.ValorFrete;
        else
            valorFrete = carga.ValorFrete > 0 ? carga.ValorFrete : repManifestoEletronicoDeDocumentosFiscais.BuscarValorTotalFrete(mdfe.Codigo);

        decimal valorContrato = valorFrete + (cargaCIOT != null ? 0 : valorICMS + valorOutros) + valorPedagio;

        Dominio.Entidades.MDFeInformacoesBancarias informacoesBancariasMDFe = repositorioMDFeInformacoesBancarias.BuscarPorMDFe(mdfe.Codigo);

        informacoesBancariasMDFe ??= new Dominio.Entidades.MDFeInformacoesBancarias();

        informacoesBancariasMDFe.MDFe = mdfe;
        informacoesBancariasMDFe.Agencia = !string.IsNullOrEmpty(cargaInformacoesBancarias?.Agencia ?? "") ? cargaInformacoesBancarias.Agencia : (veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1") && proprietario != null ? (!string.IsNullOrEmpty(proprietario.Agencia) ? proprietario.Agencia : "000") : !string.IsNullOrEmpty(contratante.Agencia) ? contratante.Agencia : "000";
        informacoesBancariasMDFe.Conta = !string.IsNullOrEmpty(cargaInformacoesBancarias?.Conta ?? "") ? cargaInformacoesBancarias.Conta : (veiculo.TipoProprietario == "0" || veiculo.TipoProprietario == "1") && proprietario != null ? (proprietario.Banco?.Numero > 0 ? proprietario.Banco?.Numero.ToString("D3") ?? "000" : "000") : contratante.Banco?.Numero > 0 ? contratante.Banco?.Numero.ToString("D3") ?? "000" : "000";
        informacoesBancariasMDFe.ChavePIX = cargaInformacoesBancarias?.ChavePIX ?? carga?.Empresa?.ChavePIX ?? "";
        informacoesBancariasMDFe.Ipef = cargaInformacoesBancarias?.Ipef ?? cargaCIOT?.CIOT?.ConfiguracaoCIOT?.CNPJOperadora ?? carga?.Empresa?.CnpjIpef ?? "";
        informacoesBancariasMDFe.TipoInformacaoBancaria = cargaInformacoesBancarias?.TipoInformacaoBancaria;
        informacoesBancariasMDFe.TipoPagamento = cargaInformacoesBancarias?.TipoPagamento ?? (cargaCIOT?.ValorAdiantamento > 0 ? FormasPagamento.Prazo : FormasPagamento.Avista);
        informacoesBancariasMDFe.ValorAdiantamento = cargaInformacoesBancarias?.ValorAdiantamento ?? cargaCIOT?.ValorAdiantamento ?? 0;
        informacoesBancariasMDFe.IndicadorAltoDesempenho = (carga.Veiculo?.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ?? false) ? true : null;


        if (informacoesBancariasMDFe.Codigo > 0)
            repositorioMDFeInformacoesBancarias.Atualizar(informacoesBancariasMDFe);
        else
            repositorioMDFeInformacoesBancarias.Inserir(informacoesBancariasMDFe);

        if (informacoesBancariasMDFe.TipoPagamento == FormasPagamento.Prazo)
        {
            List<Dominio.Entidades.MDFePagamentoParcela> parcelasExistentes = repPagamentoParcela.BuscarPorInformacoesBancarias(informacoesBancariasMDFe.Codigo);

            ParcelaPagamento parcelaPagamento = new ParcelaPagamento()
            {
                ParcelaNumero = 1,
                ParcelaVencimento = cargaInformacoesBancarias?.DataPagamento ?? cargaCIOT.CIOT.DataFinalViagem,
                ParcelaValor = valorContrato - informacoesBancariasMDFe.ValorAdiantamento ?? 0
            };

            GerarParcelasMDFe(informacoesBancariasMDFe, parcelasExistentes, new List<ParcelaPagamento>() { parcelaPagamento });
        }

        List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes = repPagamentoComponente.BuscarPorInformacoesBancarias(informacoesBancariasMDFe.Codigo);

        if (valorFrete > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor, valorFrete);

        if (cargaCIOT == null && valorICMS > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Impostos, valorICMS);

        if (valorPedagio > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.ValePedagio, valorPedagio);

        if (cargaCIOT == null && valorOutros > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Outros, valorOutros);
    }

    public void GerarInformacoesPagamentoMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, int codigoCarga, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagiosIntegrar, Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT CargaMDFeManualCIOT)
    {
        Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(_unitOfWork);
        Repositorio.Embarcador.MDFE.MDFePagamentoParcela repositorioMDFePagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(_unitOfWork);
        Repositorio.Embarcador.MDFE.MDFePagamentoComponente repositorioPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);
        Repositorio.ManifestoEletronicoDeDocumentosFiscais repManifestoEletronicoDeDocumentosFiscais = new(_unitOfWork);

        Dominio.Entidades.VeiculoMDFe veiculo = new Repositorio.VeiculoMDFe(_unitOfWork).BuscarPorMDFe(mdfe.Codigo);

        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentes = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork).BuscarTodosPorCarga(codigoCarga);
        decimal valorICMS = componentes.IsNullOrEmpty() ? 0 : componentes.Where(x => x.TipoComponenteFrete == TipoComponenteFrete.ICMS)?.Sum(x => x.ValorComponente) ?? 0;
        decimal valorPedagio = valesPedagiosIntegrar?.Sum(x => x.ValorValePedagio) ?? 0;


        decimal valorFrete;
        string[] tiposProprietarioVeiculoTac = new string[]
        {
            TipoProprietarioVeiculo.TACAgregado.ToString("d"),
            TipoProprietarioVeiculo.TACIndependente.ToString("d")
        };
        if (tiposProprietarioVeiculoTac.Contains(veiculo?.TipoProprietario ?? "") && (CargaMDFeManualCIOT?.ValorFrete ?? 0) > 0)
            valorFrete = CargaMDFeManualCIOT?.ValorFrete ?? 0;
        else
        {
            decimal valorTotalFreteMdfe = repManifestoEletronicoDeDocumentosFiscais.BuscarValorTotalFrete(mdfe.Codigo);
            valorFrete = valorTotalFreteMdfe > 0 ? valorTotalFreteMdfe : (ctes?.Sum(x => x.ValorFrete) ?? 0);
        }
               
        decimal valorContrato = valorFrete + (CargaMDFeManualCIOT != null ? 0 : valorICMS) + valorPedagio;

        Dominio.Entidades.MDFeInformacoesBancarias informacoesBancariasMDFe = repositorioMDFeInformacoesBancarias.BuscarPorMDFe(mdfe.Codigo);
        informacoesBancariasMDFe ??= new Dominio.Entidades.MDFeInformacoesBancarias();
        informacoesBancariasMDFe.MDFe = mdfe;
        informacoesBancariasMDFe.Agencia = cargaMDFeManual?.Agencia ?? "000";
        informacoesBancariasMDFe.Conta = cargaMDFeManual?.Conta ?? "000";
        informacoesBancariasMDFe.ChavePIX = cargaMDFeManual?.ChavePIX ?? "";
        informacoesBancariasMDFe.Ipef = cargaMDFeManual?.CNPJInstituicaoPagamento;
        informacoesBancariasMDFe.TipoInformacaoBancaria = cargaMDFeManual?.TipoPagamento;
        informacoesBancariasMDFe.TipoPagamento = CargaMDFeManualCIOT?.FormaPagamento ?? (CargaMDFeManualCIOT?.ValorAdiantamento > 0 ? FormasPagamento.Prazo : FormasPagamento.Avista);
        informacoesBancariasMDFe.ValorAdiantamento = CargaMDFeManualCIOT?.ValorAdiantamento ?? 0;

        if (informacoesBancariasMDFe.Codigo <= 0)
            repositorioMDFeInformacoesBancarias.Inserir(informacoesBancariasMDFe);
        else
            repositorioMDFeInformacoesBancarias.Atualizar(informacoesBancariasMDFe);


        if (informacoesBancariasMDFe.TipoPagamento == FormasPagamento.Prazo && (CargaMDFeManualCIOT?.ValorAdiantamento ?? 0) > 0)
        {
            List<Dominio.Entidades.MDFePagamentoParcela> parcelasExistentes = repositorioMDFePagamentoParcela.BuscarPorInformacoesBancarias(informacoesBancariasMDFe.Codigo);

            ParcelaPagamento parcelaPagamento = new ParcelaPagamento()
            {
                ParcelaNumero = 1,
                ParcelaVencimento = CargaMDFeManualCIOT?.DataVencimento ?? DateTime.Now.AddDays(1),
                ParcelaValor = valorContrato - (CargaMDFeManualCIOT?.ValorAdiantamento ?? 0)
            };

            GerarParcelasMDFe(informacoesBancariasMDFe, parcelasExistentes, new List<ParcelaPagamento>() { parcelaPagamento });
        }


        List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes = repositorioPagamentoComponente.BuscarPorInformacoesBancarias(informacoesBancariasMDFe.Codigo);

        if (valorFrete > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor, valorFrete);

        if (CargaMDFeManualCIOT == null && valorICMS > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Impostos, valorICMS);

        if (valorPedagio > 0)
            GerarComponentePagamento(informacoesBancariasMDFe, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.ValePedagio, valorPedagio);
    }


    public void GerarDadosPagamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento, Dominio.Entidades.Veiculo veiculo)
    {
        try
        {
            Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias;

            if (informacoesPagamento != null && informacoesPagamento.IndicadorPagamento != null &&
                ((informacoesPagamento.TipoInformacaoBancaria == TipoPagamentoMDFe.PIX && !string.IsNullOrWhiteSpace(informacoesPagamento.ChavePix)) ||
                 (informacoesPagamento.TipoInformacaoBancaria == TipoPagamentoMDFe.Ipef && !string.IsNullOrWhiteSpace(informacoesPagamento.Ipef) && informacoesPagamento.Ipef.ObterSomenteNumeros().Length == 14) ||
                 (informacoesPagamento.TipoInformacaoBancaria == TipoPagamentoMDFe.Banco && !string.IsNullOrWhiteSpace(informacoesPagamento.Banco) && informacoesPagamento.Banco.Length <= 5 && !string.IsNullOrWhiteSpace(informacoesPagamento.Agencia))))
            {
                informacoesBancarias = new Dominio.Entidades.MDFeInformacoesBancarias()
                {
                    MDFe = mdfe,
                    ChavePIX = informacoesPagamento.ChavePix,
                    Conta = informacoesPagamento.Banco,
                    Agencia = informacoesPagamento.Agencia,
                    Ipef = informacoesPagamento.Ipef,
                    TipoInformacaoBancaria = informacoesPagamento.TipoInformacaoBancaria,
                    TipoPagamento = informacoesPagamento.IndicadorPagamento,
                    ValorAdiantamento = informacoesPagamento.ValorAdiantamento,
                    IndicadorAltoDesempenho = informacoesPagamento.IndicadorAltoDesempenho
                };
            }
            else
            {
                if (veiculo == null)
                {
                    Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Repositorio.VeiculoMDFe(_unitOfWork).BuscarPorMDFe(mdfe.Codigo);
                    if (veiculoMDFe != null)
                    {
                        Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
                        veiculo = repositorioVeiculo.BuscarPorPlaca(mdfe.Empresa?.Codigo ?? 0, veiculoMDFe.Placa) ?? repositorioVeiculo.BuscarPorPlaca(0, veiculoMDFe.Placa);
                    }
                }

                if (veiculo == null) return;

                List<Dominio.Enumeradores.TipoProprietarioVeiculo> listaTipoProprietario = new List<Dominio.Enumeradores.TipoProprietarioVeiculo>()
                {
                    Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado,
                    Dominio.Enumeradores.TipoProprietarioVeiculo.TACIndependente
                };

                string cnpjIpef = (!string.IsNullOrWhiteSpace(veiculo.Empresa?.CnpjIpef) && veiculo.Empresa?.CnpjIpef.ObterSomenteNumeros().Length == 14) ? veiculo.Empresa?.CnpjIpef : 
                    (!string.IsNullOrWhiteSpace(veiculo.CNPJInstituicaoPagamentoCIOT) && veiculo.CNPJInstituicaoPagamentoCIOT.ObterSomenteNumeros().Length == 14) ? veiculo.CNPJInstituicaoPagamentoCIOT : 
                    (!string.IsNullOrWhiteSpace(veiculo.Proprietario?.CnpjIpef) && veiculo.Proprietario?.CnpjIpef.ObterSomenteNumeros().Length == 14) ? veiculo.Proprietario.CnpjIpef : "";

                if (veiculo.Tipo == "P" && (!string.IsNullOrWhiteSpace(cnpjIpef) || (!string.IsNullOrWhiteSpace(veiculo.Empresa?.Banco?.Numero.ToString() ?? "") && !string.IsNullOrWhiteSpace(veiculo.Empresa?.Agencia ?? "")) || !string.IsNullOrWhiteSpace(veiculo.Empresa?.ChavePIX ?? "")))
                {
                    bool banco = (!string.IsNullOrWhiteSpace(veiculo.Empresa?.Banco?.Numero.ToString() ?? "") && !string.IsNullOrWhiteSpace(veiculo.Empresa?.Agencia ?? ""));

                    informacoesBancarias = new Dominio.Entidades.MDFeInformacoesBancarias()
                    {
                        MDFe = mdfe,
                        ChavePIX = veiculo.Empresa?.ChavePIX ?? "",
                        Conta = veiculo.Empresa?.Banco?.Numero.ToString().Left(3).PadLeft(3, '0') ?? "",
                        Agencia = veiculo.Empresa?.Agencia.Left(5) ?? "",
                        Ipef = cnpjIpef,
                        TipoInformacaoBancaria = banco ? TipoPagamentoMDFe.Banco : (!string.IsNullOrWhiteSpace(veiculo.Empresa?.ChavePIX ?? "") ? TipoPagamentoMDFe.PIX : TipoPagamentoMDFe.Ipef),
                        TipoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento.Avista,
                        ValorAdiantamento = 0,
                        IndicadorAltoDesempenho = veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ? true : null
                    };
                }
                else if (listaTipoProprietario.Contains(veiculo.TipoProprietario) && veiculo.TipoPagamentoCIOT != null && veiculo.FormaPagamentoCIOT != null &&
                        ((veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.PIX && !string.IsNullOrWhiteSpace(veiculo.ChavePIXCIOT)) ||
                        (veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.Ipef && !string.IsNullOrWhiteSpace(cnpjIpef)) || (veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.Banco && !string.IsNullOrWhiteSpace(veiculo.ContaCIOT) && !string.IsNullOrWhiteSpace(veiculo.AgenciaCIOT))))
                {
                    informacoesBancarias = new Dominio.Entidades.MDFeInformacoesBancarias()
                    {
                        MDFe = mdfe,
                        ChavePIX = veiculo.ChavePIXCIOT,
                        Conta = veiculo.ContaCIOT.Left(3).PadLeft(3, '0'),
                        Agencia = veiculo.AgenciaCIOT.Left(5),
                        Ipef = cnpjIpef,
                        TipoInformacaoBancaria = veiculo.TipoPagamentoCIOT,
                        TipoPagamento = veiculo.FormaPagamentoCIOT,
                        ValorAdiantamento = veiculo.ValorAdiantamentoCIOT,
                        IndicadorAltoDesempenho = veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ? true : null
                    };
                }
                else
                    informacoesBancarias = new Dominio.Entidades.MDFeInformacoesBancarias()
                    {
                        MDFe = mdfe,
                        ChavePIX = string.Empty,
                        Conta = "000",
                        Agencia = "00000",
                        Ipef = string.Empty,
                        TipoInformacaoBancaria = TipoPagamentoMDFe.Banco,
                        TipoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento.Avista,
                        ValorAdiantamento = 0,
                        IndicadorAltoDesempenho = (veiculo?.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ?? false) ? true : null
                    };
            }

            Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(_unitOfWork);

            repositorioMDFeInformacoesBancarias.Inserir(informacoesBancarias);

            GerarParcelasMDFe(informacoesBancarias, new List<Dominio.Entidades.MDFePagamentoParcela>() { }, informacoesPagamento?.ParcelasPagamento);
            GerarComponentePagamentoMDFe(informacoesBancarias, informacoesPagamento?.ComponentesPagamento, mdfe, ctes);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro("Erro ao gerar dados pagamento: " + ex);
        }
    }

    public void AtualizarInformacoesPagamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
    {
        Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(_unitOfWork);
        Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork);
        Repositorio.Embarcador.MDFE.MDFePagamentoComponente repPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);

        Dominio.Entidades.Veiculo veiculo = null;
        Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Repositorio.VeiculoMDFe(_unitOfWork).BuscarPorMDFe(mdfe.Codigo);
        Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias = repositorioMDFeInformacoesBancarias.BuscarPorMDFe(mdfe.Codigo);

        if (veiculoMDFe != null)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            veiculo = repositorioVeiculo.BuscarPorPlaca(mdfe.Empresa?.Codigo ?? 0, veiculoMDFe.Placa) ?? repositorioVeiculo.BuscarPorPlaca(0, veiculoMDFe.Placa);
        }

        if (veiculo == null)
            return;

        List<Dominio.Enumeradores.TipoProprietarioVeiculo> listaTipoProprietario = new List<Dominio.Enumeradores.TipoProprietarioVeiculo>()
        {
            Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado,
            Dominio.Enumeradores.TipoProprietarioVeiculo.TACIndependente
        };

        string cnpjIpef = (!string.IsNullOrWhiteSpace(veiculo.Empresa?.CnpjIpef) && veiculo.Empresa?.CnpjIpef.ObterSomenteNumeros().Length == 14) ? veiculo.Empresa?.CnpjIpef :
                    (!string.IsNullOrWhiteSpace(veiculo.CNPJInstituicaoPagamentoCIOT) && veiculo.CNPJInstituicaoPagamentoCIOT.ObterSomenteNumeros().Length == 14) ? veiculo.CNPJInstituicaoPagamentoCIOT :
                    (!string.IsNullOrWhiteSpace(veiculo.Proprietario.CnpjIpef) && veiculo.Proprietario.CnpjIpef.ObterSomenteNumeros().Length == 14) ? veiculo.Proprietario.CnpjIpef : "";

        if (veiculo.Tipo == "P" && (!string.IsNullOrWhiteSpace(cnpjIpef) || (!string.IsNullOrWhiteSpace(veiculo.Empresa?.Banco?.Numero.ToString() ?? "") && !string.IsNullOrWhiteSpace(veiculo.Empresa?.Agencia ?? "")) || !string.IsNullOrWhiteSpace(veiculo.Empresa?.ChavePIX ?? "")))
        {
            bool banco = (!string.IsNullOrWhiteSpace(veiculo.Empresa?.Banco?.Numero.ToString() ?? "") && !string.IsNullOrWhiteSpace(veiculo.Empresa?.Agencia ?? ""));

            informacoesBancarias ??= new Dominio.Entidades.MDFeInformacoesBancarias();

            informacoesBancarias.MDFe = mdfe;
            informacoesBancarias.ChavePIX = veiculo.Empresa?.ChavePIX ?? "";
            informacoesBancarias.Conta = veiculo.Empresa?.Banco?.Numero.ToString().Left(3).PadLeft(3, '0') ?? "";
            informacoesBancarias.Agencia = veiculo.Empresa?.Agencia.Left(5) ?? "";
            informacoesBancarias.Ipef = cnpjIpef;
            informacoesBancarias.TipoInformacaoBancaria = banco ? TipoPagamentoMDFe.Banco : (!string.IsNullOrWhiteSpace(veiculo.Empresa?.ChavePIX ?? "") ? TipoPagamentoMDFe.PIX : TipoPagamentoMDFe.Ipef);
            informacoesBancarias.TipoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento.Avista;
            informacoesBancarias.ValorAdiantamento = 0;
            informacoesBancarias.IndicadorAltoDesempenho = veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ? true : null;
        }
        else if (listaTipoProprietario.Contains(veiculo.TipoProprietario) && veiculo.TipoPagamentoCIOT != null && veiculo.FormaPagamentoCIOT != null &&
                ((veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.PIX && !string.IsNullOrWhiteSpace(veiculo.ChavePIXCIOT)) ||
                (veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.Ipef && !string.IsNullOrWhiteSpace(cnpjIpef)) || (veiculo.TipoPagamentoCIOT == TipoPagamentoMDFe.Banco && !string.IsNullOrWhiteSpace(veiculo.ContaCIOT) && !string.IsNullOrWhiteSpace(veiculo.AgenciaCIOT))))
        {
            informacoesBancarias ??= new Dominio.Entidades.MDFeInformacoesBancarias();

            informacoesBancarias.MDFe = mdfe;
            informacoesBancarias.ChavePIX = veiculo.ChavePIXCIOT;
            informacoesBancarias.Conta = veiculo.ContaCIOT.Left(3).PadLeft(3, '0');
            informacoesBancarias.Agencia = veiculo.AgenciaCIOT.Left(5);
            informacoesBancarias.Ipef = cnpjIpef;
            informacoesBancarias.TipoInformacaoBancaria = veiculo.TipoPagamentoCIOT;
            informacoesBancarias.TipoPagamento = veiculo.FormaPagamentoCIOT;
            informacoesBancarias.ValorAdiantamento = veiculo.ValorAdiantamentoCIOT;
            informacoesBancarias.IndicadorAltoDesempenho = veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ? true : null;
        }
        else
        {
            informacoesBancarias ??= new Dominio.Entidades.MDFeInformacoesBancarias();

            informacoesBancarias.MDFe = mdfe;
            informacoesBancarias.ChavePIX = string.Empty;
            informacoesBancarias.Conta = "000";
            informacoesBancarias.Agencia = "00000";
            informacoesBancarias.Ipef = string.Empty;
            informacoesBancarias.TipoInformacaoBancaria = TipoPagamentoMDFe.Banco;
            informacoesBancarias.TipoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento.Avista;
            informacoesBancarias.ValorAdiantamento = 0;
            informacoesBancarias.IndicadorAltoDesempenho = (veiculo?.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas ?? false) ? true : null;
        }

        if (informacoesBancarias.Codigo > 0)
            repositorioMDFeInformacoesBancarias.Atualizar(informacoesBancarias);
        else
            repositorioMDFeInformacoesBancarias.Inserir(informacoesBancarias);

        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumento.BuscarCTesPorMDFe(mdfe.Codigo);
        List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes = repPagamentoComponente.BuscarPorInformacoesBancarias(informacoesBancarias.Codigo);

        GerarComponentesPagamentoMDFeDocumento(informacoesBancarias, componentesPagamentoExistentes, mdfe, ctes);
    }

    private void GerarParcelasMDFe(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, List<Dominio.Entidades.MDFePagamentoParcela> parcelasExistentes, List<ParcelaPagamento> parcelasPagamento)
    {
        if (parcelasPagamento.IsNullOrEmpty())
            return;

        Repositorio.Embarcador.MDFE.MDFePagamentoParcela repPagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(_unitOfWork);

        foreach (Dominio.ObjetosDeValor.MDFe.ParcelaPagamento parcelaPagamento in parcelasPagamento)
        {
            Dominio.Entidades.MDFePagamentoParcela pagamentoParcela = parcelasExistentes.FirstOrDefault(o => o.NumeroParcela == parcelaPagamento.ParcelaNumero);

            pagamentoParcela ??= new Dominio.Entidades.MDFePagamentoParcela();

            pagamentoParcela.InformacoesBancarias = informacoesBancarias;
            pagamentoParcela.NumeroParcela = parcelaPagamento.ParcelaNumero;
            pagamentoParcela.DataVencimentoParcela = parcelaPagamento.ParcelaVencimento;
            pagamentoParcela.ValorParcela = parcelaPagamento.ParcelaValor;

            if (pagamentoParcela.Codigo > 0)
                repPagamentoParcela.Atualizar(pagamentoParcela);
            else
                repPagamentoParcela.Inserir(pagamentoParcela);
        }
    }

    private void GerarComponentePagamentoMDFe(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, List<Dominio.ObjetosDeValor.MDFe.ComponentePagamento> componentesPagamento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
    {
        Repositorio.Embarcador.MDFE.MDFePagamentoComponente repPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);
        List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes = repPagamentoComponente.BuscarPorInformacoesBancarias(informacoesBancarias.Codigo);

        if (componentesPagamento.IsNullOrEmpty())
        {
            GerarComponentesPagamentoMDFeDocumento(informacoesBancarias, componentesPagamentoExistentes, mdfe, ctes);

            return;
        }

        foreach (Dominio.ObjetosDeValor.MDFe.ComponentePagamento componentePagamento in componentesPagamento)
        {
            GerarComponentePagamento(informacoesBancarias, componentesPagamentoExistentes, componentePagamento.TipoComponente, componentePagamento.ValorComponente);
        }
    }

    private void GerarComponentesPagamentoMDFeDocumento(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
    {
        decimal valorPedagio = new Repositorio.ValePedagioMDFe(_unitOfWork).BuscarPorMDFe(mdfe.Codigo).Sum(x => x.ValorValePedagio);
        bool existeCIOTPorMDFe = new Repositorio.MDFeCIOT(_unitOfWork).ExistePorMDFe(mdfe.Codigo);

        if (valorPedagio > 0)
            GerarComponentePagamento(informacoesBancarias, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.ValePedagio, valorPedagio);

        if (ctes.Any())
        {
            decimal valorFrete = ctes.Sum(x => x.ValorFrete);
            if (valorFrete > 0)
                GerarComponentePagamento(informacoesBancarias, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor, valorFrete);

            if (existeCIOTPorMDFe)
                return;

            decimal valorICMS = ctes.Sum(x => x.ValorICMS);
            if (valorICMS > 0)
                GerarComponentePagamento(informacoesBancarias, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Impostos, valorICMS);

            IEnumerable<int> codigosCTes = ctes.Select(o => o.Codigo);
            string[] ignorarValores = new string[] { "valor frete", "frete valor", "impostos" };
            decimal valorOutros = new Repositorio.ComponentePrestacaoCTE(_unitOfWork).ObterSomaValoresComponentesPorCTe(codigosCTes, ignorarValores);

            if (valorOutros > 0)
                GerarComponentePagamento(informacoesBancarias, componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.Outros, valorOutros);
        }
    }

    private void GerarComponentePagamento(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, List<Dominio.Entidades.MDFePagamentoComponente> componentesPagamentoExistentes, Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento tipoComponente, decimal valorComponente)
    {
        Repositorio.Embarcador.MDFE.MDFePagamentoComponente repPagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);

        Dominio.Entidades.MDFePagamentoComponente pagamentoComponente = componentesPagamentoExistentes.FirstOrDefault(o => o.TipoComponente == tipoComponente);

        pagamentoComponente ??= new Dominio.Entidades.MDFePagamentoComponente();

        pagamentoComponente.InformacoesBancarias = informacoesBancarias;
        pagamentoComponente.TipoComponente = tipoComponente;
        pagamentoComponente.ValorComponente = valorComponente;

        if (pagamentoComponente.Codigo > 0)
            repPagamentoComponente.Atualizar(pagamentoComponente);
        else
            repPagamentoComponente.Inserir(pagamentoComponente);
    }
}
