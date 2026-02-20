/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="DocumentoComplementar.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotasComplementar = null;

var NotasComplementar = function ()
{
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.CTes.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.Destinatario.getFieldDescription() , idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NotasComplementaresGlobal = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.Ocorrencia.MarcarDesmarcarTodos, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _notasComplementar.SelecionarTodos.val(true);
            _gridNotasComplementar.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function carregarGridNotasComplementar(dataRow, row, table) {
    _notasComplementar = new NotasComplementar();
    KoBindings(_notasComplementar, "knockoutNotasComplementares");

    _notasComplementar.CTe.codEntity(dataRow.CodigoCTE);
    _notasComplementar.CTe.val(dataRow.Numero);

    if (dataRow.NotasCTeGlobalizadoSelecionarTodos != null)
        _notasComplementar.SelecionarTodos.val(dataRow.NotasCTeGlobalizadoSelecionarTodos);

    _notasComplementar.Confirmar.eventClick = function () { ConfirmarSelecaoNotasParaRemover(dataRow, row, table) };

    new BuscarClientes(_notasComplementar.Destinatario);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        naoSelecionados: new Array(),
        selecionados: new Array(),
        SelecionarTodosKnout: _notasComplementar.SelecionarTodos,
        somenteLeitura: false,
    }

    var menuOpcoes = null;
    _gridNotasComplementar = new GridView(_notasComplementar.NotasComplementaresGlobal.idGrid, "XMLNotaFiscal/PesquisaNotasComplemento", _notasComplementar, menuOpcoes, null, null, null, null, null, multiplaescolha, 10);
    _gridNotasComplementar.CarregarGrid();
    
    if (dataRow.NotasCTeGlobalizado != null) {
        if (_notasComplementar.SelecionarTodos.val())
            _gridNotasComplementar.AtualizarRegistrosNaoSelecionados(dataRow.NotasCTeGlobalizado);
        else
            _gridNotasComplementar.AtualizarRegistrosSelecionados(dataRow.NotasCTeGlobalizado);
    }    
}

function ConfirmarSelecaoNotasParaRemover(dataRow, row, table) {
    if (_notasComplementar.SelecionarTodos.val()) {
        if (_gridNotasComplementar.ObterMultiplosNaoSelecionados().length > 0) {
            dataRow.NotasCTeGlobalizado = _gridNotasComplementar.ObterMultiplosNaoSelecionados();
            dataRow.NotasCTeGlobalizadoSelecionarTodos = true;
            AtualizarDataRow(table, row, dataRow, null)
        }
    }
    else {
        if (_gridNotasComplementar.ObterMultiplosSelecionados().length > 0) {
            dataRow.NotasCTeGlobalizado = _gridNotasComplementar.ObterMultiplosSelecionados();
            dataRow.NotasCTeGlobalizadoSelecionarTodos = false;
            AtualizarDataRow(table, row, dataRow, null)
        }
    }

    Global.fecharModal("divModalNotasComplementares");
}
//*******MÉTODOS*******