/// <reference path="../../Enumeradores/EnumTipoFeriado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFeriado, _feriado, _pesquisaFeriado, _crudFeriado;

var PesquisaFeriado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Tipo = PropertyEntity({ text: "Tipo:", options: EnumTipoFeriado.obterOpcoesPesquisa(), val: ko.observable(EnumTipoFeriado.Todos), def: EnumTipoFeriado.Todos, issue: 0, visible: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFeriado.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Feriado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", required: false });
    this.Dia = PropertyEntity({ text: "*Dia: ", required: true, maxlength: 2 });
    this.Mes = PropertyEntity({ text: "*Mês: ", required: true, maxlength: 2 });
    this.Ano = PropertyEntity({ text: "Ano: ", maxlength: 4 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Tipo = PropertyEntity({ text: "*Tipo:", options: EnumTipoFeriado.obterOpcoes(), val: ko.observable(EnumTipoFeriado.Nacional), def: EnumTipoFeriado.Nacional, issue: 0, visible: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Estado:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });

    this.Tipo.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoFeriado.Nacional) {
            _feriado.Localidade.visible(false);
            _feriado.Localidade.required = false;
            _feriado.Estado.visible(false);
            _feriado.Estado.required = false;
        } else if (novoValor == EnumTipoFeriado.Estadual) {
            _feriado.Localidade.visible(false);
            _feriado.Localidade.required = false;
            _feriado.Estado.visible(true);
            _feriado.Estado.required = true;
        } else if (novoValor == EnumTipoFeriado.Municipal) {
            _feriado.Localidade.visible(true);
            _feriado.Localidade.required = true;
            _feriado.Estado.visible(false);
            _feriado.Estado.required = false;
        }
    });
}

var CRUDFeriado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadFeriado() {

    _feriado = new Feriado();
    KoBindings(_feriado, "knockoutCadastro");

    _crudFeriado = new CRUDFeriado();
    KoBindings(_crudFeriado, "knockoutCRUD");

    _pesquisaFeriado = new PesquisaFeriado();
    KoBindings(_pesquisaFeriado, "knockoutPesquisaFeriado", false, _pesquisaFeriado.Pesquisar.id);

    new BuscarLocalidadesBrasil(_feriado.Localidade);
    new BuscarEstados(_feriado.Estado);

    new BuscarLocalidadesBrasil(_pesquisaFeriado.Localidade);
    new BuscarEstados(_pesquisaFeriado.Estado);

    HeaderAuditoria("Feriado", _feriado, "Codigo");

    buscarFeriado();
}

function adicionarClick(e, sender) {
    Salvar(_feriado, "Feriado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFeriado.CarregarGrid();
                limparCamposFeriado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_feriado, "Feriado/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _gridFeriado.CarregarGrid();
            limparCamposFeriado();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o feriado " + _feriado.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_feriado, "Feriado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridFeriado.CarregarGrid();
                    limparCamposFeriado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFeriado();
}

//*******MÉTODOS*******

function buscarFeriado() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarFeriado, tamanho: "9", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFeriado = new GridView(_pesquisaFeriado.Pesquisar.idGrid, "Feriado/Pesquisa", _pesquisaFeriado, menuOpcoes, null);
    _gridFeriado.CarregarGrid();
}

function editarFeriado(arquivoGrid) {
    limparCamposFeriado();
    _feriado.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_feriado, "Feriado/BuscarPorCodigo", function (arg) {
        _pesquisaFeriado.ExibirFiltros.visibleFade(false);
        _crudFeriado.Atualizar.visible(true);
        _crudFeriado.Cancelar.visible(true);
        _crudFeriado.Excluir.visible(true);
        _crudFeriado.Adicionar.visible(false);
        
    }, null);
}

function limparCamposFeriado() {
    _crudFeriado.Atualizar.visible(false);
    _crudFeriado.Cancelar.visible(false);
    _crudFeriado.Excluir.visible(false);
    _crudFeriado.Adicionar.visible(true);

    LimparCampos(_feriado);
}