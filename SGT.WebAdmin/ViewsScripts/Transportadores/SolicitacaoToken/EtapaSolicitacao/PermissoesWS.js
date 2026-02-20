/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../Consultas/PermissaoWebService.js" />


var _gridPermissoesWSSolicitacaoToken;
var _solicitacaoTokenPermissoesWS;
var _CRUDSolicitacaoTokenPermissoesWS;

var SolicitacaoTokenPermissoesWS = function () {
    this.Grid = PropertyEntity({ text: 'Permissões WebService:', idGrid: guid() });

    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, visible: false });
    this.Metodo = PropertyEntity({ text: ko.observable('*Método'), codEntity: ko.observable(0), enable: ko.observable(false), type: types.entity, required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true) });
    this.RequisicoesMinuto = PropertyEntity({ text: '*Requisição por minuto:', val: ko.observable(), enable: ko.observable(false), getType: typesKnockout.int, required: ko.observable(false), visible: ko.observable(true), configInt: { allowZero: true } });
    this.QuantidadeRequisicoes = PropertyEntity({ text: '*Quantidade de Requisições:', val: ko.observable(), enable: ko.observable(false), getType: typesKnockout.int, required: ko.observable(false), visible: ko.observable(true), configInt: { allowZero: true } });
    this.TodosMetodosLiberados = PropertyEntity({ text: 'Limpar lista e adicionar todos os Métodos disponíveis', enable: ko.observable(true), val: ko.observable(true), getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TodosMetodosLiberados.val.subscribe((value) => {
        if (value) {
            this.Metodo.enable(false);
            this.Metodo.required(false);
            this.Metodo.codEntity(0);
            this.Metodo.val('');

            this.QuantidadeRequisicoes.enable(false);
            this.QuantidadeRequisicoes.required(false);

            this.RequisicoesMinuto.enable(false);
            this.RequisicoesMinuto.required(false);

            _CRUDSolicitacaoTokenPermissoesWS.Adicionar.text("Adicionar Permissões");
        }
        else {
            this.Metodo.enable(true);
            this.Metodo.required(true);

            this.QuantidadeRequisicoes.enable(true);
            this.QuantidadeRequisicoes.required(true);

            this.RequisicoesMinuto.enable(true);
            this.RequisicoesMinuto.required(true);

            _CRUDSolicitacaoTokenPermissoesWS.Adicionar.text("Adicionar Permissão");
        }
    });
}

var CRUDSolicitacaoTokenPermissoesWS = function () {
    this.Adicionar = PropertyEntity({ text: ko.observable('Adicionar Permissão'), type: types.event, eventClick: adicionarPermissaoWSSolicitacaoToken, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: 'Atualizar Permissão', type: types.event, eventClick: atualizarPermissaoWSSolicitacaoTokenClick, visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ text: 'Limpar Campos', type: types.event, eventClick: LimparCamposPermissoesWSSolicitacaoToken, visible: ko.observable(true) });
}

var loadPermissoesWSSolicitacaoToken = function () {
    _solicitacaoTokenPermissoesWS = new SolicitacaoTokenPermissoesWS();
    KoBindings(_solicitacaoTokenPermissoesWS, 'knockoutSolicitacaoTokenPermissoesWS');

    _CRUDSolicitacaoTokenPermissoesWS = new CRUDSolicitacaoTokenPermissoesWS();
    KoBindings(_CRUDSolicitacaoTokenPermissoesWS, 'knockoutCRUDSolicitacaoTokenPermissoesWS');

    loadGridPermissoesWSSolicitacaoToken();

    BuscarPermissoesWS(_solicitacaoTokenPermissoesWS.Metodo, callbackPermissaoWS, null, true)
}

var loadGridPermissoesWSSolicitacaoToken = function() {
    const linhasPorPaginas = 10;
    const opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarPermissaoWSSolicitacaoTokenClick, icone: "" };
    const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerPermissaoWSSolicitacaoTokenClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoEditar, opcaoRemover] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoMetodo", visible: false },
        { data: "Metodo", title: "Método", width: "50%", className: "text-align-left" },
        { data: "RequisicoesMinuto", title: "Requisições por minuto", width: "20%", className: "text-align-center" },
        { data: "QuantidadeRequisicoes", title: "Quantidade de Requisições", width: "20%", className: "text-align-center" },
    ];

    _gridPermissoesWSSolicitacaoToken = new BasicDataTable(_solicitacaoTokenPermissoesWS.Grid.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    _gridPermissoesWSSolicitacaoToken.CarregarGrid([]);
}

function obterListaPermissoesWSSolicitacaoToken() {
    return _gridPermissoesWSSolicitacaoToken.BuscarRegistros();
}

function removerPermissaoWSSolicitacaoTokenClick(registroSelecionado) {
    const pms = obterListaPermissoesWSSolicitacaoToken();

    for (let i = 0; i < pms.length; i++) {
        if (registroSelecionado.CodigoMetodo == pms[i].CodigoMetodo) {
            pms.splice(i, 1);
            break;
        }
    }

    _gridPermissoesWSSolicitacaoToken.CarregarGrid(pms);
}

function adicionarPermissaoWSSolicitacaoToken() {
    const list = _gridPermissoesWSSolicitacaoToken.BuscarRegistros();

    const pm = {
        Codigo: 0,
        CodigoMetodo: _solicitacaoTokenPermissoesWS.Metodo.codEntity(),
        Metodo: _solicitacaoTokenPermissoesWS.Metodo.val(),
        RequisicoesMinuto: _solicitacaoTokenPermissoesWS.RequisicoesMinuto.val(),
        QuantidadeRequisicoes: _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.val(),
    }

    if (!validarPermissaoSolicitacaoToken(pm, list, true)) {
        return;
    }

    if (_solicitacaoTokenPermissoesWS.TodosMetodosLiberados.val()) {
        inserirTodosMetodosNaGrid();
        return;
    }

    list.push(pm);

    _gridPermissoesWSSolicitacaoToken.CarregarGrid(list);
    LimparCamposPermissoesWSSolicitacaoToken();
}

function editarPermissaoWSSolicitacaoTokenClick(registroSelecionado) {
    _solicitacaoTokenPermissoesWS.Codigo.val(registroSelecionado.Codigo);
    _solicitacaoTokenPermissoesWS.Metodo.codEntity(registroSelecionado.CodigoMetodo);
    _solicitacaoTokenPermissoesWS.Metodo.val(registroSelecionado.Metodo);
    _solicitacaoTokenPermissoesWS.RequisicoesMinuto.val(registroSelecionado.RequisicoesMinuto);
    _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.val(registroSelecionado.QuantidadeRequisicoes);

    _CRUDSolicitacaoTokenPermissoesWS.Adicionar.visible(false);
    _CRUDSolicitacaoTokenPermissoesWS.Atualizar.visible(true);
    _solicitacaoTokenPermissoesWS.TodosMetodosLiberados.visible(false);
    _solicitacaoTokenPermissoesWS.TodosMetodosLiberados.val(false);
}

function atualizarPermissaoWSSolicitacaoTokenClick() {
    const pms = obterListaPermissoesWSSolicitacaoToken();

    for (let i = 0; i < pms.length; i++) {
        if (_solicitacaoTokenPermissoesWS.Metodo.codEntity() == pms[i].CodigoMetodo) {
            pms[i] = {
                Codigo: _solicitacaoTokenPermissoesWS.Codigo.val(),
                CodigoMetodo: _solicitacaoTokenPermissoesWS.Metodo.codEntity(),
                Metodo: _solicitacaoTokenPermissoesWS.TodosMetodosLiberados.val() ? 'Todos' : _solicitacaoTokenPermissoesWS.Metodo.val(),
                RequisicoesMinuto: _solicitacaoTokenPermissoesWS.RequisicoesMinuto.val(),
                QuantidadeRequisicoes: _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.val(),
            }

            if (!validarPermissaoSolicitacaoToken()) {
                return;
            }
        }
    }

    _gridPermissoesWSSolicitacaoToken.CarregarGrid(pms);
    LimparCamposPermissoesWSSolicitacaoToken();

}

function renderizarGridPermissoesWSSolicitacaoToken() {
    const pms = obterListaPermissoesWSSolicitacaoToken();

    _gridPermissoesWSSolicitacaoToken.CarregarGrid(pms);
}

function recarregarGridPermissoesWSSolicitacaoToken() {
    _gridPermissoesWSSolicitacaoToken.CarregarGrid([]);
}

function LimparCamposPermissoesWSSolicitacaoToken() {
    _solicitacaoTokenPermissoesWS.Metodo.enable(false);
    _solicitacaoTokenPermissoesWS.Metodo.required(false);
    _solicitacaoTokenPermissoesWS.Metodo.codEntity(0);
    _solicitacaoTokenPermissoesWS.Metodo.val('');

    _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.enable(false);
    _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.required(false);

    _solicitacaoTokenPermissoesWS.RequisicoesMinuto.enable(false);
    _solicitacaoTokenPermissoesWS.RequisicoesMinuto.required(false);

    _solicitacaoTokenPermissoesWS.TodosMetodosLiberados.val(true);
    _solicitacaoTokenPermissoesWS.TodosMetodosLiberados.visible(true);

    _CRUDSolicitacaoTokenPermissoesWS.Adicionar.visible(true);
    _CRUDSolicitacaoTokenPermissoesWS.Atualizar.visible(false);
}

function validarPermissaoSolicitacaoToken(pm, list, adicionar) {
    let valido = true;

    if (!ValidarCamposObrigatorios(_solicitacaoTokenPermissoesWS)) {
        exibirMensagem(tipoMensagem.atencao, 'Campos Obrigatórios', 'Favor preencher os campos obrigatórios');
        valido = false;
    }

    if (adicionar) {
        list.forEach(x => {
            if (x.Metodo == pm["Metodo"] && x != pm) {
                exibirMensagem(tipoMensagem.atencao, 'Duplicidade de Método', `O Método: ${x.Metodo} já está adicionado na Lista`);
                valido = false;
            }
        });
    }

    return valido;
}

function callbackPermissaoWS(registro) {
    _solicitacaoTokenPermissoesWS.Metodo.codEntity(registro.Codigo);
    _solicitacaoTokenPermissoesWS.Metodo.val(registro.NomeMetodo);

    _solicitacaoTokenPermissoesWS.RequisicoesMinuto.val(registro.RequisicoesMinuto);
    _solicitacaoTokenPermissoesWS.QuantidadeRequisicoes.val(registro.QuantidadeRequisicoes);
}

function inserirTodosMetodosNaGrid() {
    executarReSTGET('PermissaoWebService/ObterRegistros', { DistinguirPorNomeMetodo: true }, (r) => {
        if (r.Data) {
            _gridPermissoesWSSolicitacaoToken.CarregarGrid(r.Data);
            LimparCamposPermissoesWSSolicitacaoToken();
        }
        else
            exibirMensagem(tipoMensagem.aviso, 'Sem registros', 'Nenhum método encontrado');
    });
}