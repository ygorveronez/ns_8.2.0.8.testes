/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoCanhoto.js" />
/// <reference path="../../Consultas/Localidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _baixarCanhoto;
var _gridCanhotos;
var _knoutArquivo;

var _enumTipoCanhoto = [
    { text: "NF-e", value: EnumTipoCanhoto.NFe },
    { text: "Avulso", value: EnumTipoCanhoto.Avulso }
];

var BaixarCanhoto = function () {
    this.DadosBaixa = PropertyEntity({ text: "*Informação para Baixa: ", required: true, idBtnSearch: guid(), eventClick: baixarCanhotoClick, enable: ko.observable(true) });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.NFe), options: _enumTipoCanhoto, def: EnumTipoCanhoto.NFe, text: "Tipo do Canhoto: ", visible: ko.observable(true), eventChange: buscarLocalArmazenamentoAtual });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CapacidadeArmazenagem = PropertyEntity({ text: "Capacidade:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Local de Armazenamento:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PacoteAtual = PropertyEntity({ text: "Pacote Atual:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.TotalDePacotes = PropertyEntity({ text: "Pacote Atual:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DividirEmPacotesDe = PropertyEntity({ text: "Dividir em Pacotes de:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.QuantidadeArmazenada = PropertyEntity({ text: "Quantidade já armazenada:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.LocalCheio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCanhotos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

function loadBaixarCanhoto() {
    _baixarCanhoto = new BaixarCanhoto();
    KoBindings(_baixarCanhoto, "knockoutBaixarCanhoto", false, _baixarCanhoto.DadosBaixa.idBtnSearch);

    AlterarLayoutTipoServico();
    buscarLocalArmazenamentoAtual();

    _gridCanhotos = new GridView(_baixarCanhoto.Pesquisar.idGrid, "Canhoto/ConsultarPorLocalArmazenamento", _baixarCanhoto, null, null, null, null, null, null, null, 5);
}

function baixarCanhotoClick() {
    var data = { DadosBaixa: _baixarCanhoto.DadosBaixa.val() };
    executarReST("Canhoto/ValidaObrigatoriedadeDataEntregaCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data == true) {
                Global.abrirModal('divModalDataEntregaCanhoto');
            } else {
                setTimeout(function () {
                    Salvar(_baixarCanhoto, "Canhoto/BaixarCanhotoFisico", function (arg) {
                        _baixarCanhoto.DadosBaixa.val("");
                        if (arg.Success) {
                            if (arg.Data !== false) {
                                if (arg.Data.Mensagem != "")
                                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.Mensagem, 10000);
                                $("#" + _baixarCanhoto.DadosBaixa.id).focus();
                                if (arg.Data.LocalArmazenamentAtual != null) {
                                    var loc = { Data: arg.Data.LocalArmazenamentAtual };
                                    preencherDadosLocalArmazenamento(loc);
                                    _baixarCanhoto.TipoCanhoto.val(loc.TipoCanhoto);
                                }
                            } else {
                                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                            }
                        } else {
                            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                        }
                    });
                }, 30);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarLocalArmazenamentoAtual() {
    var data = { TipoCanhoto: _baixarCanhoto.TipoCanhoto.val() };
    executarReST("LocalArmazenamentoCanhoto/BuscarLocalArmazenamentoAtual", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                preencherDadosLocalArmazenamento(arg);
            } else {
                _baixarCanhoto.DadosBaixa.enable(false);
                _baixarCanhoto.Descricao.visible(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function preencherDadosLocalArmazenamento(arg) {
    LimparCampos(_baixarCanhoto);
    PreencherObjetoKnout(_baixarCanhoto, arg);
    if (_baixarCanhoto.LocalCheio.val()) {
        _baixarCanhoto.DadosBaixa.enable(false);
    } else {
        _baixarCanhoto.DadosBaixa.enable(true);
    }
    if (_baixarCanhoto.Observacao.val() == "")
        _baixarCanhoto.Observacao.visible(false);
    else
        _baixarCanhoto.Observacao.visible(true);

    if (_baixarCanhoto.DividirEmPacotesDe.val() == 0) {
        _baixarCanhoto.DividirEmPacotesDe.visible(false);
        _baixarCanhoto.PacoteAtual.visible(false);
    } else {
        _baixarCanhoto.DividirEmPacotesDe.visible(true);
        _baixarCanhoto.PacoteAtual.visible(true);
    }
    buscarCanhotosLocalArmazenamento();
}

function buscarCanhotosLocalArmazenamento() {
    desativarControleRequisicao();
    //_gridCanhotos.Destroy();
    _gridCanhotos.CarregarGrid(ativarControleRequisicao);
}

function AlterarLayoutTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _baixarCanhoto.TipoCanhoto.visible(false);
        _baixarCanhoto.TipoCanhoto.eventChange = function () { };
    }
}