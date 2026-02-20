/// <reference path="GrupoPessoas.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumFormaValorDescarga.js" />
/// <reference path="../../Enumeradores/EnumFormaAberturaOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoPrazoCobrancaChamado.js" />
/// <reference path="../../Enumeradores/EnumNivelToleranciaValor.js" />

var _grupoPessoasPerfilChamado;
var _gridTabelaGrupoPessoasPerfilChamado;

var GrupoPessoasPerfilChamado = function () {
    this.ClienteNaoNecessitaAutorizacaoAtendimento = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EsteClienteNaoNecessitaAutorizacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GeraNumeroOcorrenciaAutorizacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EstePerfilGeraNumeroOcorrenciaAutorizacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.DiaPrazoCobrancaChamado = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DiaPrazoCobranca.getFieldDescription(), getType: typesKnockout.int, enable: ko.observable(true) });

    this.FormaValorDescarga = PropertyEntity({ val: ko.observable(EnumFormaValorDescarga.Todos), options: EnumFormaValorDescarga.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.FormaValorDescarga.getFieldDescription(), def: EnumFormaValorDescarga.Todos, required: false, enable: ko.observable(true) });
    this.FormaAberturaOcorrencia = PropertyEntity({ val: ko.observable(EnumFormaAberturaOcorrencia.Todos), options: EnumFormaAberturaOcorrencia.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.FormaValorDescarga.getFieldDescription(), def: EnumFormaAberturaOcorrencia.Todos, required: false, enable: ko.observable(true) });
    this.TipoPrazoCobrancaChamado = PropertyEntity({ val: ko.observable(EnumTipoPrazoCobrancaChamado.Todos), options: EnumTipoPrazoCobrancaChamado.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.FormaValorDescarga.getFieldDescription(), def: EnumTipoPrazoCobrancaChamado.Todos, required: false, enable: ko.observable(true) });
    this.NivelToleranciaValorCliente = PropertyEntity({ val: ko.observable(EnumNivelToleranciaValor.NaoAceitaDivergencia), options: EnumNivelToleranciaValor.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.NivelToleranciaClienteDiferencaValor.getFieldDescription(), def: EnumNivelToleranciaValor.NaoAceitaDivergencia, required: false, enable: ko.observable(true) });
    this.NivelToleranciaValorMotorista = PropertyEntity({ val: ko.observable(EnumNivelToleranciaValor.NaoAceitaDivergencia), options: EnumNivelToleranciaValor.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.NivelToleranciaMotoristaDiferencaValor.getFieldDescription(), def: EnumNivelToleranciaValor.NaoAceitaDivergencia, required: false, enable: ko.observable(true) });

    //Aba tabela de valores
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.Valor = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Valor.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: true });
    this.SalvarTabela = PropertyEntity({ eventClick: SalvarTabelaGrupoPessoasPerfilChamadoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.SalvarTabela });

    //Aba Configurações
    this.QuantidadeMaximaDiasDataReciboAbertura = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeMaximaDiasDivergentes.getFieldDescription(), getType: typesKnockout.int, enable: ko.observable(true) });
    this.ValorMaximoDiferencaRecibo = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ValorMinimoDiferencaRecibo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true) });

    this.AssuntoEmailChamado = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.AssuntoEmail.getFieldDescription(), maxlength: 1000, enable: ko.observable(true) });
    this.TagNumeroChamadoAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#NumeroChamado"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroChamado, enable: ko.observable(true) });
    this.TagDataEmissaoViagemAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#DataEmissaoViagem"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.DataEmissaoViagem, enable: ko.observable(true) });
    this.TagNumeroSerieCTeAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#NumeroSerieCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroSerieCTe, enable: ko.observable(true) });
    this.TagNumeroCargaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroCarga, enable: ko.observable(true) });
    this.TagNumeroPedidoEmbarcadorAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroPedidoEmbarcador, enable: ko.observable(true) });
    this.TagInicioPrestacaoAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#InicioPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.InicioPrestacao, enable: ko.observable(true) });
    this.TagRemetenteAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Destinatario, enable: ko.observable(true) });
    this.TagFrotaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#Frota"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Frota, enable: ko.observable(true) });
    this.TagVeiculoAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#Veiculo"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Veiculo, enable: ko.observable(true) });
    this.TagValorTotalAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#ValorTotal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorTotal, enable: ko.observable(true) });
    this.TagFimPrestacaoAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#FimPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FimPrestacao, enable: ko.observable(true) });
    this.TagModeloVeicularAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#ModeloVeicular"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular, enable: ko.observable(true) });
    this.TagFormaCobrancaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#FormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FormaCobranca, enable: ko.observable(true) });
    this.TagValorUnitarioAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#ValorUnitario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorUnitario, enable: ko.observable(true) });
    this.TagMotoristaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#Motorista"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Motorista, enable: ko.observable(true) });
    this.TagValorCobrarAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#ValorCobrar"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorCobrar, enable: ko.observable(true) });
    this.TagQuantidadeFormaCobrancaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#QuantidadeFormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeFormaCobranca, enable: ko.observable(true) });
    this.TagCustosAdicionaisAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#CustosAdicionais"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CustosAdicionais, enable: ko.observable(true) });
    this.TagValorInclusoFreteAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#ValorInclusoFrete"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorInclusoFrete, enable: ko.observable(true) });
    this.TagNotaFiscalAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.AssuntoEmailChamado.id, "#NotaFiscal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NotaFiscal, enable: ko.observable(true) });

    this.CorpoEmailChamado = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CorpoEmail.getFieldDescription(), maxlength: 2000, enable: ko.observable(true) });
    this.TagNumeroChamadoCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#NumeroChamado"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroChamado, enable: ko.observable(true) });
    this.TagDataEmissaoViagemCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#DataEmissaoViagem"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.DataEmissaoViagem, enable: ko.observable(true) });
    this.TagNumeroSerieCTeCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#NumeroSerieCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroSerieCTe, enable: ko.observable(true) });
    this.TagNumeroCargaCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroCarga, enable: ko.observable(true) });
    this.TagNumeroPedidoEmbarcadorCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroPedidoEmbarcador, enable: ko.observable(true) });
    this.TagInicioPrestacaoCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#InicioPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.InicioPrestacao, enable: ko.observable(true) });
    this.TagRemetenteCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Destinatario, enable: ko.observable(true) });
    this.TagFrotaCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#Frota"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Frota, enable: ko.observable(true) });
    this.TagVeiculoCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#Veiculo"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Veiculo, enable: ko.observable(true) });
    this.TagValorTotalCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#ValorTotal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorTotal, enable: ko.observable(true) });
    this.TagFimPrestacaoCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#FimPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FimPrestacao, enable: ko.observable(true) });
    this.TagModeloVeicularCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#ModeloVeicular"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular, enable: ko.observable(true) });
    this.TagFormaCobrancaCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#FormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FormaCobranca, enable: ko.observable(true) });
    this.TagValorUnitarioCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#ValorUnitario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorUnitario, enable: ko.observable(true) });
    this.TagMotoristaCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#Motorista"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Motorista, enable: ko.observable(true) });
    this.TagValorCobrarCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#ValorCobrar"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorCobrar, enable: ko.observable(true) });
    this.TagQuantidadeFormaCobrancaCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#QuantidadeFormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeFormaCobranca, enable: ko.observable(true) });
    this.TagCustosAdicionaisCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#CustosAdicionais"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CustosAdicionais, enable: ko.observable(true) });
    this.TagValorInclusoFreteCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#ValorInclusoFrete"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorInclusoFrete, enable: ko.observable(true) });
    this.TagNotaFiscalCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.CorpoEmailChamado.id, "#NotaFiscal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NotaFiscal, enable: ko.observable(true) });

    this.MensagemPadraoOrientacaoMotorista = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.MensagemPadraoOrientacaoMotorista.getFieldDescription(), maxlength: 2000, enable: ko.observable(true) });
    this.TagNumeroChamadoMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#NumeroChamado"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroChamado, enable: ko.observable(true) });
    this.TagDataEmissaoViagemMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#DataEmissaoViagem"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.DataEmissaoViagem, enable: ko.observable(true) });
    this.TagNumeroSerieCTeMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#NumeroSerieCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroSerieCTe, enable: ko.observable(true) });
    this.TagNumeroCargaMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroCarga, enable: ko.observable(true) });
    this.TagNumeroPedidoEmbarcadorMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroPedidoEmbarcador, enable: ko.observable(true) });
    this.TagInicioPrestacaoMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#InicioPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.InicioPrestacao, enable: ko.observable(true) });
    this.TagRemetenteMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Destinatario, enable: ko.observable(true) });
    this.TagFrotaMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#Frota"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Frota, enable: ko.observable(true) });
    this.TagVeiculoMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#Veiculo"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Veiculo, enable: ko.observable(true) });
    this.TagValorTotalMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#ValorTotal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorTotal, enable: ko.observable(true) });
    this.TagFimPrestacaoMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#FimPrestacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FimPrestacao, enable: ko.observable(true) });
    this.TagModeloVeicularMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#ModeloVeicular"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular, enable: ko.observable(true) });
    this.TagFormaCobrancaMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#FormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FormaCobranca, enable: ko.observable(true) });
    this.TagValorUnitarioMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#ValorUnitario"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorUnitario, enable: ko.observable(true) });
    this.TagMotoristaMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#Motorista"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Motorista, enable: ko.observable(true) });
    this.TagValorCobrarMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#ValorCobrar"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorCobrar, enable: ko.observable(true) });
    this.TagQuantidadeFormaCobrancaMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#QuantidadeFormaCobranca"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeFormaCobranca, enable: ko.observable(true) });
    this.TagCustosAdicionaisMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#CustosAdicionais"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CustosAdicionais, enable: ko.observable(true) });
    this.TagValorInclusoFreteMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#ValorInclusoFrete"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ValorInclusoFrete, enable: ko.observable(true) });
    this.TagNotaFiscalMensagemPadraoOrientacaoMotorista = PropertyEntity({ eventClick: function () { InserirTag(_grupoPessoasPerfilChamado.MensagemPadraoOrientacaoMotorista.id, "#NotaFiscal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NotaFiscal, enable: ko.observable(true) });
};

function LoadGrupoPessoasPerfilChamado() {
    _grupoPessoasPerfilChamado = new GrupoPessoasPerfilChamado();
    KoBindings(_grupoPessoasPerfilChamado, "knockoutPerfilChamado");

    new BuscarModelosVeicularesCarga(_grupoPessoasPerfilChamado.ModeloVeicularCarga);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: ExcluirTabelaGrupoPessoasPerfilChamadoClick, tamanho: 10 }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicularCarga", visible: false },
        { data: "ModeloVeicularCargaDescricao", title: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular, width: "60%" },
        { data: "Valor", title: Localization.Resources.Pessoas.GrupoPessoas.Valor, width: "30%" }
    ];
    _gridTabelaGrupoPessoasPerfilChamado = new BasicDataTable(_grupoPessoasPerfilChamado.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    RecarregarGridTabelaGrupoPessoasPerfilChamado();
}

function RecarregarGridTabelaGrupoPessoasPerfilChamado() {

    var data = new Array();
    $.each(_grupoPessoas.TabelaValores.list, function (i, tabela) {
        var tabelaGrid = new Object();

        tabelaGrid.Codigo = tabela.Codigo.val;
        tabelaGrid.ModeloVeicularCarga = tabela.ModeloVeicularCarga.codEntity;
        tabelaGrid.ModeloVeicularCargaDescricao = tabela.ModeloVeicularCarga.val;
        tabelaGrid.Valor = tabela.Valor.val;

        data.push(tabelaGrid);
    });

    _gridTabelaGrupoPessoasPerfilChamado.CarregarGrid(data);
}

function SalvarTabelaGrupoPessoasPerfilChamadoClick() {

    var valido = ValidarCamposObrigatorios(_grupoPessoasPerfilChamado);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    _grupoPessoasPerfilChamado.Codigo.val(guid());
    _grupoPessoas.TabelaValores.list.push(SalvarListEntity(_grupoPessoasPerfilChamado));

    LimparTabelaGrupoPessoasPerfilChamado();
}

function ExcluirTabelaGrupoPessoasPerfilChamadoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Atencao, Localization.Resources.Pessoas.GrupoPessoas.DesejaRealmenteExcluirTabela, function () {
        for (var i = 0; i < _grupoPessoas.TabelaValores.list.length; i++) {
            if (e.Codigo === _grupoPessoas.TabelaValores.list[i].Codigo.val) {
                _grupoPessoas.TabelaValores.list.splice(i, 1);
                break;
            }
        }

        LimparTabelaGrupoPessoasPerfilChamado();
    });
}

function LimparTabelaGrupoPessoasPerfilChamado() {
    LimparCampoEntity(_grupoPessoasPerfilChamado.ModeloVeicularCarga);
    _grupoPessoasPerfilChamado.Valor.val("");
    RecarregarGridTabelaGrupoPessoasPerfilChamado();
}