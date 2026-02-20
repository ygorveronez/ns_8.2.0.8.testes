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
/// <reference path="Aprovacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasLiberacaoPagamentoProvedor;

//*******EVENTOS*******

const RegraPagamentoProvedor = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAprovacaoAlcadaRegra = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

const JustificativaAprovacaoRegra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarRegraPagamentoProvedor, type: types.event, text: "Aprovar", visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarRegraPagamentoProvedor, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: fecharModalJustificativaRegraPagamentoProvedor, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

function loadRegrasAprovacao() {
    _regraPagamentoProvedor = new RegraPagamentoProvedor();
    KoBindings(_regraPagamentoProvedor, "knockoutAutorizacaoPagamentoProvedor");

    _justificativaAprovacaoRegra = new JustificativaAprovacaoRegra();
    KoBindings(_justificativaAprovacaoRegra, "knockoutJustificativaAprovacaoRegra");

    GridRegrasAprovacao();
}

function cancelarRejeicaoClick() {
    limparRejeicao();
}

//*******MÉTODOS*******
function aprovarRegraPagamentoProvedor() {
    var dados = {
        Codigo: _justificativaAprovacaoRegra.Codigo.val(),
        Justificativa: _justificativaAprovacaoRegra.Justificativa.val()
    };
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar a regra?", function () {
        executarReST("AutorizacaoProvedor/Aprovar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                    fecharModalJustificativaRegraPagamentoProvedor();

                    AtualizarGridRegrasAutorizacaoLiberacaoPagamentoProvedor();
                    _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();

                    SetarEtapasPagamentoProvedor(EnumEtapaLiberacaoPagamentoProvedor.Liberacao, EnumSituacaoLiberacaoPagamentoProvedor.Finalizada);

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function abrirModalRegraPagamentoProvedor(dataRow, acao) {
    limparJustificativaAprovacaoRegra();

    Global.abrirModal("divModalJustificativaAprovacaoRegra");

    _justificativaAprovacaoRegra.Codigo.val(dataRow.Codigo);

    if (acao === 'aprovar') {
        _justificativaAprovacaoRegra.Aprovar.visible(true);
        _justificativaAprovacaoRegra.Rejeitar.visible(false);
    }
    else if (acao === 'rejeitar') {
        _justificativaAprovacaoRegra.Aprovar.visible(false);
        _justificativaAprovacaoRegra.Rejeitar.visible(true);
    }
}


function fecharModalJustificativaRegraPagamentoProvedor() {
    Global.fecharModal("divModalJustificativaAprovacaoRegra");
}

function rejeitarRegraPagamentoProvedor(dataRow) {
    var dados = {
        Codigo: _justificativaAprovacaoRegra.Codigo.val(),
        Justificativa: _justificativaAprovacaoRegra.Justificativa.val()
    };
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar a regra?", function () {
        executarReST("AutorizacaoProvedor/Reprovar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                    fecharModalJustificativaRegraPagamentoProvedor();

                    AtualizarGridRegrasAutorizacaoLiberacaoPagamentoProvedor();
                    _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();

                    SetarEtapasPagamentoProvedor(EnumEtapaLiberacaoPagamentoProvedor.Aprovacao, EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada);

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function limparRegras() {
    limparRejeicao();
}

function limparJustificativaAprovacaoRegra() {
    LimparCampos(_justificativaAprovacaoRegra);
}

function abrirModalRegraPagamentoProvedorParaAprovar(dataRow) {
    abrirModalRegraPagamentoProvedor(dataRow, 'aprovar');
}

function abrirModalRegraPagamentoProvedorParaRejeitar(dataRow) {
    abrirModalRegraPagamentoProvedor(dataRow, 'rejeitar');
}

function GridRegrasAprovacao() {
    var aprovar = { descricao: "Aprovar", id: guid(), evento: "onclick", visibilidade: function (dataRow) { return dataRow.PodeAprovar ? true : false; }, metodo: abrirModalRegraPagamentoProvedorParaAprovar };
    var rejeitar = { descricao: "Rejeitar", id: guid(), evento: "onclick", visibilidade: function (dataRow) { return dataRow.PodeAprovar ? true : false; }, metodo: abrirModalRegraPagamentoProvedorParaRejeitar };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [aprovar, rejeitar] };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        menuOpcoes = null;
    }

    _gridRegrasLiberacaoPagamentoProvedor = new GridView(_regraPagamentoProvedor.Regras.idGrid, "AutorizacaoProvedor/RegrasAprovacao", _regraPagamentoProvedor, menuOpcoes);
}
