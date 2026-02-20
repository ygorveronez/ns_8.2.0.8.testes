/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="AutorizacaoControleReajusteFretePlanilha.js" />
/// <reference path="AutorizarRegras.js" />

// #region Objetos Globais do Arquivo

var _delegarSelecionados;
var _delegar;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Delegar = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
}

var DelegarSelecionados = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarSelecionadosClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    _delegarSelecionados = new DelegarSelecionados();
    KoBindings(_delegarSelecionados, "knockoutDelegarSelecionados");

    new BuscarFuncionario(_delegar.UsuarioDelegado);
    new BuscarFuncionario(_delegarSelecionados.UsuarioDelegado);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function delegarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar o reajuste?", function () {
        var dados = {
            Codigo: _controleReajusteFretePlanilha.Codigo.val(),
            UsuarioDelegado: _delegar.UsuarioDelegado.codEntity(),
        };

        executarReST("AutorizacaoControleReajusteFretePlanilha/Delegar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridControleReajuste();
                    atualizarControleReajusteFretePlanilha()
                    AtualizarGridRegras();
                    LimparCampos(_delegar);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function delegarSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar todos os reajustes selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaControle);

        dados.SelecionarTodos = _pesquisaControle.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosNaoSelecionados());
        dados.UsuarioDelegado = _delegarSelecionados.UsuarioDelegado.codEntity();

        executarReST("AutorizacaoControleReajusteFretePlanilha/DelegarMultiplas", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridControleReajuste();
                    cancelarDelegarSelecionadosClick();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarDelegarSelecionadosClick() {
    LimparCampos(_delegarSelecionados);
    Global.fecharModal("divModalDelegarSelecionados");
}

// #endregion Funções Associadas a Eventos
