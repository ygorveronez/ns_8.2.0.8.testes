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
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="Anexos.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPendenciaMotorista.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridPendenciaMotorista;
var _pendenciaMotorista;
var _pesquisaPendenciaMotorista;
var _acrescimoDesconto;
var _valorPendencia;

var _situacaoPendenciaMotorista = [{ text: "Todos", value: EnumSituacaoPendenciaMotorista.Todos }, { text: "Ativo", value: EnumSituacaoPendenciaMotorista.Ativo }, { text: "Estornado", value: EnumSituacaoPendenciaMotorista.Estornado }];


var PesquisaPendenciaMotorista = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), val: ko.observable("") });
    this.ValorInicial = PropertyEntity({ text: "Valor Inicial:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.ValorFinal = PropertyEntity({ text: "Valor Final:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoPendenciaMotorista.Ativo), options: _situacaoPendenciaMotorista, def: EnumSituacaoPendenciaMotorista.Ativo });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPendenciaMotorista.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PendenciaMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true });
    this.Pendencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Pendência:"), idBtnSearch: guid(), required: true, enable: ko.observable(false) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Justificativa:"), idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ text: "*Valor:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Data = PropertyEntity({ text: "*Data do Movimento:", required: true, getType: typesKnockout.date });
    this.ValorPendencia = PropertyEntity({ text: "Valor Pendência:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10, enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 500 });
    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosPendenciaMotoristaClick, type: types.event, text: "Anexos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Estornar = PropertyEntity({ eventClick: estornarClick, type: types.event, text: "Estornar", visible: ko.observable(false) });

}


//*******EVENTOS*******


function loadPendenciaMotorista() {

    _pendenciaMotorista = new PendenciaMotorista();
    KoBindings(_pendenciaMotorista, "knockoutPendenciaMotorista");

    _pesquisaPendenciaMotorista = new PesquisaPendenciaMotorista();
    KoBindings(_pesquisaPendenciaMotorista, "knockoutPesquisaPendenciaMotorista", false, _pesquisaPendenciaMotorista.Pesquisar.id);

    HeaderAuditoria("PendenciaMotorista", _pendenciaMotorista);
    loadAnexosPendenciaMotorista();
    new BuscarMotoristas(_pesquisaPendenciaMotorista.Motorista);
    new BuscarMotoristas(_pendenciaMotorista.Motorista, callbackretornoMotorista);
    new BuscarJustificativas(_pendenciaMotorista.Justificativa, callbackJustificativa, null, [EnumTipoFinalidadeJustificativa.PendenciaMotorista]);
    new BuscarPagamentoMotoristaTMSPendente(_pendenciaMotorista.Pendencia, _pendenciaMotorista.Motorista, callbackretorno);

    _valorPendencia = _pendenciaMotorista.Valor;
    buscarPendenciaMotorista();
    loadAnexosPendenciaMotorista();
}

function callbackJustificativa(justificativa) {
    _pendenciaMotorista.Justificativa.val(justificativa.Descricao);
    _pendenciaMotorista.Justificativa.codEntity(justificativa.Codigo);

    _acrescimoDesconto = justificativa.DescricaoTipoJustificativa;
}
function callbackretorno(pendencia) {
    _pendenciaMotorista.Pendencia.val(pendencia.Numero + " - " + pendencia.Motorista);
    _pendenciaMotorista.Pendencia.codEntity(pendencia.Codigo);

    _pendenciaMotorista.ValorPendencia.val(Globalize.format(Globalize.parseFloat(pendencia.Valor), "n2"));

}


function callbackretornoMotorista(motorista) {
    _pendenciaMotorista.Motorista.val(motorista.Descricao);
    _pendenciaMotorista.Motorista.codEntity(motorista.Codigo);
    _pendenciaMotorista.Pendencia.enable(_pendenciaMotorista.Motorista.codEntity() > 0);
}


function adicionarClick(e, sender) {
    Salvar(e, "PendenciaMotorista/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pendenciaMotorista.Codigo.val(arg.Data);

                EnviarArquivosAnexadosPendenciaMotorista(function () {
                    buscarPendenciaMotoristaPorCodigo(arg.Data);
                });

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPendenciaMotorista.CarregarGrid();
                limparPendenciaMotorista();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}


function cancelarClick(e) {
    limparPendenciaMotorista();
}

function estornarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar essa pendência?", function () {
        ExcluirPorCodigo(_pendenciaMotorista, "PendenciaMotorista/EstornarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Estornado com sucesso");
                    _gridPendenciaMotorista.CarregarGrid();
                    limparPendenciaMotorista();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }

        }, null);
    });
}

//*******MÉTODOS*******



function buscarPendenciaMotoristaPorCodigo(codigo, callback) {
    limparPendenciaMotorista();
    executarReST("PendenciaMotorista/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            if (callback instanceof Function)
                callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function buscarPendenciaMotorista() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPendenciaMotorista, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPendenciaMotorista = new GridView(_pesquisaPendenciaMotorista.Pesquisar.idGrid, "PendenciaMotorista/Pesquisa", _pesquisaPendenciaMotorista, menuOpcoes, null);
    _gridPendenciaMotorista.CarregarGrid();


}

function editarPendenciaMotorista(pendenciaMotoristaGrid) {

    limparPendenciaMotorista();
    _pendenciaMotorista.Codigo.val(pendenciaMotoristaGrid.Codigo);
    BuscarPorCodigo(_pendenciaMotorista, "PendenciaMotorista/BuscarPorCodigo", function (arg) {
        var pendenciaMotoristaAtivo = arg.Data.Situacao == EnumSituacaoPendenciaMotorista.Ativo;
        CarregarAnexosPendenciaMotorista(arg.Data);
        _pesquisaPendenciaMotorista.ExibirFiltros.visibleFade(false);
        _pendenciaMotorista.Cancelar.visible(true);
        _pendenciaMotorista.Estornar.visible(pendenciaMotoristaAtivo);
        _pendenciaMotorista.Adicionar.visible(false);
    }, null);

    _valorPendencia = _pendenciaMotorista.ValorPendencia.val();
}

function limparPendenciaMotorista() {
    _pendenciaMotorista.Cancelar.visible(false);
    _pendenciaMotorista.Estornar.visible(false);
    _pendenciaMotorista.Adicionar.visible(true);
    _pendenciaMotorista.Justificativa.text("*Justificativa:");
    _pendenciaMotorista.Pendencia.text("*Pendência:");
    LimparCampos(_pendenciaMotorista);
    limparOcorrenciaAnexosPendenciaMotorista();
}