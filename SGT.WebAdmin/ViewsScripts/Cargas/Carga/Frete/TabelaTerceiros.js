/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="Complemento.js" />
/// <reference path="Componente.js" />
/// <reference path="EtapaFrete.js" />
/// <reference path="Frete.js" />
/// <reference path="SemTabela.js" />
/// <reference path="TabelaCliente.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaRota.js" />
/// <reference path="TabelaSubContratacao.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteCliente.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteComissao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoComplementoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalheFretePorSubContratacaoTerceiro;
function loadCargaFreteSubcontratacaoTerceiro() {
    $.get("Content/Static/Carga/DestalhesFreteSubcontratacaoTerceiros.html?dyn=" + guid(), function (data) {
        _HTMLDetalheFretePorSubContratacaoTerceiro = data;
    });
}

var DetalheFreteSubcontratacaoTerceiro = function () {

    this.ValorTotalDaPrestacaoSubContratacao = PropertyEntity({ type: types.local });
    this.ValorPedagioSubContratacao = PropertyEntity({ type: types.local });
    this.ValorFreteSubContratacaoTabelaDeFrete = PropertyEntity({ type: types.local });
    this.ValorFreteSubcontratacao = PropertyEntity({ type: types.local });
    this.PercentualDescontoTerceiro = PropertyEntity({ type: types.local });

    this.PercentualAdiantamento = PropertyEntity({ type: types.local });
    this.PercentualAbastecimento = PropertyEntity({ type: types.local });
    this.PercentualSaldo = PropertyEntity({ type: types.local });
    this.Desconto = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorAdiantamento = PropertyEntity({ type: types.local });
    this.ValorDesconto = PropertyEntity({ type: types.local });
    this.ValorSaldo = PropertyEntity({ type: types.local });
    this.ValorAbastecimento = PropertyEntity({ type: types.local });
    this.DiasVencimentoAdiantamento = PropertyEntity({ type: types.local });
    this.DiasVencimentoSaldo = PropertyEntity({ type: types.local });
    this.DataVencimentoAdiantamento = PropertyEntity({ type: types.local });
    this.DataVencimentoSaldo = PropertyEntity({ type: types.local });
    this.TabelaFrete = PropertyEntity({ type: types.local, visible: ko.observable(false) });

    this.Carga = PropertyEntity({ type: types.map });
    this.ValorPedagioSubcontratacaoManual = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.InformarValorDoPegadioManualmente.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorFreteSubcontratacaoManual = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.InformarValorDaSubcontratacaoManualmente.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualAdiantamentoSubcontratacaoManual = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.InformarPorcetagemDeAdiantamentoManualmente.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true), maxlength: ko.observable(_CONFIGURACAO_TMS.PermitirInformarPercentual100AdiantamentoCarga ? 6 : 5), val: ko.observable(0) });
    this.AtualizarValorFreteSubcontratacao = PropertyEntity({ eventClick: atualizarValorFreteSubcontratacaoTerceiroClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true), enable: ko.observable(true) });
    this.RecalcularValorFreteSubcontratacao = PropertyEntity({ eventClick: RecalcularValorFreteSubcontratacaoTerceiroClick, type: types.event, text: Localization.Resources.Cargas.Carga.Recalcular, visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarValor = PropertyEntity({ eventClick: AbrirTelaCargaTabelaTerceiroValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ObservacaoManual = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAlterarObservacaoContratoFreteTerceiro, _PermissoesPersonalizadasCarga)), enable: ko.observable(true) });
    this.PermiteAlterarValor = PropertyEntity({ type: types.local, getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });

    //Variaveis Traduções
    this.TabelaDeFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TabelaDeFrete });
    this.ValorDoFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorDoFrete });
    this.ValorDePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorDePedagio });
    this.PorcentagemAdiantamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PorcentagemAdiantamento });
    this.Adiantamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Adiantamento });
    this.VencimentoAdiantamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.VencimentoAdiantamento });
    this.DescontoEmTabela = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DescontoEmTabela });
    this.PorcentagemAbastecimento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PorcentagemAbastecimento });
    this.Abastecimento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Abastecimento });
    this.ValorPreviamenteAcordado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorPreviamenteAcordado });
    this.ValorTotalDaPrestacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorTotalDaPrestacao });
    this.PorcentagemSaldo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PorcentagemSaldo });
    this.Saldo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Saldo });
    this.VencimentoSaldo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.VencimentoSaldo });
    this.AcrescimentoDesconto = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AcrescimentoDesconto });

    this.Mensagem = PropertyEntity({ type: types.local, warning: ko.observable(true), danger: ko.observable(true), success: ko.observable(true), visible: ko.observable(false) });
    this.HistoricoAcrescimoDesconto = PropertyEntity({ eventClick: HistoricoAcrescimoDescontoClick, type: types.event, text: Localization.Resources.Cargas.Carga.HistoricoIntegracaoAcrescimoDesconto.getFieldDescription(), idFade: guid(), visible: ko.observable(true), enable: ko.observable(true) });


    this.PercentualAdiantamentoSubcontratacaoManual.val.subscribe((valor) => {
        validarPercentual(valor, this.PercentualAdiantamentoSubcontratacaoManual);
    });
};

//*******EVENTOS*******

function atualizarValorFreteSubcontratacaoTerceiroClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAlterarValorDoFreteSubcontratacao, function () {
        Salvar(e, "CargaFreteTerceiro/InformarValorSubContratacaoFreteManual", function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    informarValoresDeSubContratacao(e, arg.Data);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAlteradoComSucesso);
                    _gridCargaTabelaTerceiroValor.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function RecalcularValorFreteSubcontratacaoTerceiroClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRecalcularValorDoFreteSubcontratacaoOsAcrescimosDescontosSeraoRemovidos, function () {
        Salvar(e, "CargaFreteTerceiro/RecalcularValorFreteSubContratacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    informarValoresDeSubContratacao(e, arg.Data);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAlteradoComSucesso);
                    _gridCargaTabelaTerceiroValor.CarregarGrid();
                    ExibirMensagemConsultaAcrescimoDesconto(arg.Data, e);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function preencherRetornoFreteSubcontratacaoTerceiro(e, freteSubcontratacao) {

    $("#tabTerceiros_" + e.DadosEmissaoFrete.id + "_li").show();

    $("#" + e.EtapaFreteTMS.idTerceiros).html(_HTMLDetalheFretePorSubContratacaoTerceiro.replace(/#knoutDetalhesFreteSubContratacao/g, "knoutDetalhesFreteSubContratacao_" + e.EtapaFreteTMS.idTerceiros));

    var cargaFreteSubcontratacaoTerceiro = new DetalheFreteSubcontratacaoTerceiro();

    cargaFreteSubcontratacaoTerceiro.Carga.val(e.Codigo.val());

    informarValoresDeSubContratacao(cargaFreteSubcontratacaoTerceiro, freteSubcontratacao);

    KoBindings(cargaFreteSubcontratacaoTerceiro, "knoutDetalhesFreteSubContratacao_" + e.EtapaFreteTMS.idTerceiros);

    cargaFreteSubcontratacaoTerceiro.AtualizarValorFreteSubcontratacao.visible(e.EtapaFreteEmbarcador.enable());
    cargaFreteSubcontratacaoTerceiro.RecalcularValorFreteSubcontratacao.visible(e.EtapaFreteEmbarcador.enable());
    cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacaoManual.visible(e.EtapaFreteEmbarcador.enable());
    cargaFreteSubcontratacaoTerceiro.ValorPedagioSubcontratacaoManual.visible(e.EtapaFreteEmbarcador.enable());
    cargaFreteSubcontratacaoTerceiro.PercentualAdiantamentoSubcontratacaoManual.visible(e.EtapaFreteEmbarcador.enable());

    var permiteAlterarValores = false;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete, _PermissoesPersonalizadasCarga) || !cargaFreteSubcontratacaoTerceiro.PermiteAlterarValor.val())
        cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacaoManual.visible(false);
    else
        permiteAlterarValores = true;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorPedagio, _PermissoesPersonalizadasCarga) || !cargaFreteSubcontratacaoTerceiro.PermiteAlterarValor.val())
        cargaFreteSubcontratacaoTerceiro.ValorPedagioSubcontratacaoManual.visible(false);
    else
        permiteAlterarValores = true;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto, _PermissoesPersonalizadasCarga) || !cargaFreteSubcontratacaoTerceiro.PermiteAlterarValor.val())
        cargaFreteSubcontratacaoTerceiro.AdicionarValor.visible(false);
    else
        permiteAlterarValores = true;

    if (!_CONFIGURACAO_TMS.InformarPercentualAdiantamentoCarga || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarPercentualAdiantamento, _PermissoesPersonalizadasCarga) || !cargaFreteSubcontratacaoTerceiro.PermiteAlterarValor.val())
        cargaFreteSubcontratacaoTerceiro.PercentualAdiantamentoSubcontratacaoManual.visible(false);
    else
        permiteAlterarValores = true;

    if (!permiteAlterarValores)
        cargaFreteSubcontratacaoTerceiro.AtualizarValorFreteSubcontratacao.visible(false);

    SetarInformacoesCargaTabelaTerceiroValor(cargaFreteSubcontratacaoTerceiro);
}

function informarValoresDeSubContratacao(cargaFreteSubcontratacaoTerceiro, freteSubcontratacao) {

    if (_retorno != null)
        _retorno["freteSubContratacao"] = freteSubcontratacao;

    if (freteSubcontratacao.TipoFreteEscolhido == EnumTipoFreteEscolhido.Operador)
        cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacao.val(Globalize.format(freteSubcontratacao.ValorLiquidoFreteTerceiro, "n2") + " (" + Localization.Resources.Cargas.Carga.InformadoPeloOperador + ")");
    else
        cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacao.val(Globalize.format(freteSubcontratacao.ValorLiquidoFreteTerceiro, "n2"));

    cargaFreteSubcontratacaoTerceiro.ValorTotalDaPrestacaoSubContratacao.val(Globalize.format(freteSubcontratacao.ValorTotalPrestacao, "n2"));
    cargaFreteSubcontratacaoTerceiro.ValorPedagioSubContratacao.val(Globalize.format(freteSubcontratacao.ValorPedagio, "n2"));
    cargaFreteSubcontratacaoTerceiro.PercentualDescontoTerceiro.val(freteSubcontratacao.PercentualDescontoTerceiro);
    cargaFreteSubcontratacaoTerceiro.ValorFreteSubContratacaoTabelaDeFrete.val(Globalize.format(freteSubcontratacao.ValorFreteSubContratacaoTabelaDeFrete, "n2"));
    cargaFreteSubcontratacaoTerceiro.PercentualAdiantamentoSubcontratacaoManual.val(Globalize.format(freteSubcontratacao.PercentualAdiantamento, "n2"));

    cargaFreteSubcontratacaoTerceiro.PercentualAdiantamento.val(Globalize.format(freteSubcontratacao.PercentualAdiantamento, "n2"));
    cargaFreteSubcontratacaoTerceiro.PercentualAbastecimento.val(Globalize.format(freteSubcontratacao.PercentualAbastecimento, "n2"));
    cargaFreteSubcontratacaoTerceiro.PercentualSaldo.val(Globalize.format(freteSubcontratacao.PercentualSaldo, "n2"));
    cargaFreteSubcontratacaoTerceiro.ValorAdiantamento.val(Globalize.format(freteSubcontratacao.ValorAdiantamento, "n2"));
    cargaFreteSubcontratacaoTerceiro.ValorSaldo.val(Globalize.format(freteSubcontratacao.ValorSaldo, "n2"));

    if (freteSubcontratacao.Desconto > 0) {
        cargaFreteSubcontratacaoTerceiro.Desconto.val(Globalize.format(freteSubcontratacao.Desconto, "n2"));
        cargaFreteSubcontratacaoTerceiro.Desconto.visible(true);
    }

    cargaFreteSubcontratacaoTerceiro.ValorAbastecimento.val(Globalize.format(freteSubcontratacao.ValorAbastecimento, "n2"));
    cargaFreteSubcontratacaoTerceiro.DiasVencimentoAdiantamento.val(freteSubcontratacao.DiasVencimentoAdiantamento);
    cargaFreteSubcontratacaoTerceiro.DiasVencimentoSaldo.val(freteSubcontratacao.DiasVencimentoSaldo);
    cargaFreteSubcontratacaoTerceiro.DataVencimentoAdiantamento.val(freteSubcontratacao.DataVencimentoAdiantamento);
    cargaFreteSubcontratacaoTerceiro.DataVencimentoSaldo.val(freteSubcontratacao.DataVencimentoSaldo);
    cargaFreteSubcontratacaoTerceiro.ObservacaoManual.val(freteSubcontratacao.ObservacaoManual);
    cargaFreteSubcontratacaoTerceiro.PermiteAlterarValor.val(freteSubcontratacao.PermiteAlterarValor);

    cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacaoManual.val(cargaFreteSubcontratacaoTerceiro.ValorFreteSubcontratacao.val());
    cargaFreteSubcontratacaoTerceiro.ValorPedagioSubcontratacaoManual.val(cargaFreteSubcontratacaoTerceiro.ValorPedagioSubContratacao.val());

    if (!string.IsNullOrWhiteSpace(freteSubcontratacao.TabelaFrete) || !string.IsNullOrWhiteSpace(freteSubcontratacao.TabelaFreteCliente)) {
        cargaFreteSubcontratacaoTerceiro.TabelaFrete.val(freteSubcontratacao.TabelaFrete + " (" + freteSubcontratacao.TabelaFreteCliente + ")");
        cargaFreteSubcontratacaoTerceiro.TabelaFrete.visible(true);
    }
}

function ExibirMensagemConsultaAcrescimoDesconto(cargaFreteSubcontratacaoTerceiro, _detalheFreteSubcontratacaoTerceiro) {
    if (cargaFreteSubcontratacaoTerceiro.IntegrouValoresAcrescimoDesconto != null) {

        let success = false;
        let danger = false;
        let msg = "";
        if (cargaFreteSubcontratacaoTerceiro.IntegrouValoresAcrescimoDesconto) {
            msg = Localization.Resources.Cargas.Carga.ConsultaAcrescimosDescontosRealizadaComSucesso.getFieldDescription();
            success = true;
        } else {
            msg = Localization.Resources.Cargas.Carga.OcorreramProblemasConsultarAcrescimosDescontosParaCarga.getFieldDescription();
            danger = true;
        }

        _detalheFreteSubcontratacaoTerceiro.Mensagem
            .warning(false)
            .danger(danger)
            .success(success)
            .val(msg)
            .visible(true);

        _detalheFreteSubcontratacaoTerceiro.HistoricoAcrescimoDesconto.visible(true);
    }
    else {
        _detalheFreteSubcontratacaoTerceiro.Mensagem.visible(false);
    }
}
function HistoricoAcrescimoDescontoClick(e) {
    loadHistoricoIntegracaoAcrescimoDesconto(e);
}
function validarPercentual(novoValor, percentualAdiantamentoSubcontratacaoManual) {
    if (novoValor != null) {
        var valor = Globalize.parseFloat(novoValor);
        if (isNaN(valor) || valor > 100) {
            percentualAdiantamentoSubcontratacaoManual.val(Globalize.format(100, "n2"));
        }
    }
}
