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

//*******MAPEAMENTO KNOUCKOUT*******


var _delegarSelecionados;
var _delegar;

var DelegarSelecionados = function () {
    this.Usuario = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarAvariaSelecionadosClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var Delegar = function () {
    this.Usuario = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    _delegarSelecionados = new DelegarSelecionados();
    KoBindings(_delegarSelecionados, "knockoutDelegarLote");

    new BuscarFuncionario(_delegar.Usuario);
    new BuscarFuncionario(_delegarSelecionados.Usuario);
}

function delegarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar o lote?", function () {
        var dados = {
            Lote:_lote.Codigo.val(),
            Usuario: _delegar.Usuario.codEntity(),
        };

        executarReST("LotesPendentes/DelegarLote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    BuscarLotes();
                    LimparDelegar();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function delegarAvariaSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar todas os lotes selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLotes);
        var delegar = RetornarObjetoPesquisa(_delegarSelecionados);

        dados.Usuario = delegar.Usuario;
        dados.SelecionarTodos = _pesquisaLotes.SelecionarTodos.val();
        dados.LotesSelecionados = JSON.stringify(_gridLote.ObterMultiplosSelecionados());
        dados.LotesNaoSelecionados = JSON.stringify(_gridLote.ObterMultiplosNaoSelecionados());

        executarReST("LotesPendentes/DelegarMultiplosLotes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    BuscarLotes();
                    cancelarDelegarSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarDelegarSelecionadosClick() {
    LimparCampos(_delegarSelecionados);
    Global.fecharModal("divModalDelegarLote");
}




//*******MÉTODOS*******

function LimparDelegar() {
    LimparCampos(_delegar);
}