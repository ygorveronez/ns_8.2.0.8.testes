/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="JanelaColetaLocalidade.js" />
/// <reference path="JanelaColetaCliente.js" />
/// <reference path="JanelaColetaUF.js" />

var _pesquisaJanelaColeta
var _gridJanelaColeta

var PesquisaJanelaColeta = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação:" });
    this.Observacao = PropertyEntity({ text: "Observação:" });
    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJanelaColeta.CarregarGrid();
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

var JanelaColeta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 400 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    
    this.Localidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.UFs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Clientes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PeriodosColeta = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}


var CRUDJanelaColeta = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

function LoadJanelaColeta() {
    _pesquisaJanelaColeta = new PesquisaJanelaColeta();
    KoBindings(_pesquisaJanelaColeta, "knockoutPesquisaJanelaColeta", false, _pesquisaJanelaColeta.Pesquisar.id);

    _janelaColeta= new JanelaColeta();
    KoBindings(_janelaColeta, "knockoutDetalhesJanelaColeta");

    _crudJanelaColeta = new CRUDJanelaColeta();
    KoBindings(_crudJanelaColeta, "knockoutCRUDJanelaColeta");

    LoadJanelaColetaLocalidade();
    LoadJanelaColetaUF();
    LoadJanelaColetaCliente();
    LoadJanelaColetaPeriodo();

    BuscarJanelaColeta();
}

//*******EVENTOS*******


function BuscarJanelaColeta() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarJanelaColeta, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridJanelaColeta = new GridView(_pesquisaJanelaColeta.Pesquisar.idGrid, "JanelaColeta/Pesquisa", _pesquisaJanelaColeta, menuOpcoes, null);
    _gridJanelaColeta.CarregarGrid();

};

function EditarJanelaColeta(JanelaColetaGrid) {
    LimparCamposJanelaColeta();
    _janelaColeta.Codigo.val(JanelaColetaGrid.Codigo);
    BuscarPorCodigo(_janelaColeta, "JanelaColeta/BuscarPorCodigo", function (arg) {
        _pesquisaJanelaColeta.ExibirFiltros.visibleFade(false);
        _crudJanelaColeta.Atualizar.visible(true);
        _crudJanelaColeta.Cancelar.visible(true);
        _crudJanelaColeta.Excluir.visible(true);
        _crudJanelaColeta.Adicionar.visible(false);

        RecarregarGridJanelaColetaCliente();
        RecarregarGridJanelaColetaUF();
        RecarregarGridJanelaColetaLocalidade();
        RecarregarJanelaColetaPeriodo();

    }, null);
}


function LimparCamposJanelaColeta() {
    _crudJanelaColeta.Atualizar.visible(false);
    _crudJanelaColeta.Cancelar.visible(false);
    _crudJanelaColeta.Excluir.visible(false);
    _crudJanelaColeta.Adicionar.visible(true);
    LimparCampos(_janelaColeta);
    limparCamposJanelaColetaPeriodo();

    RecarregarGridJanelaColetaCliente();
    RecarregarGridJanelaColetaUF();
    RecarregarGridJanelaColetaLocalidade();
    RecarregarJanelaColetaPeriodo()
    
    ResetarTabs();
}


function ResetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}


function AdicionarClick(e, sender) {
    _janelaColeta.Localidades.val(JSON.stringify(_janelaColetaLocalidade.Localidade.basicTable.BuscarRegistros()));
    _janelaColeta.UFs.val(JSON.stringify(_janelaColetaUF.UF.basicTable.BuscarRegistros()));
    _janelaColeta.Clientes.val(JSON.stringify(_janelaColetaCliente.Cliente.basicTable.BuscarRegistros()));
    _janelaColeta.PeriodosColeta.val(JSON.stringify(obterJanelasColetaPeriodo()));

    Salvar(_janelaColeta, "JanelaColeta/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                _gridJanelaColeta.CarregarGrid();
                LimparCamposJanelaColeta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        ResetarTabs();
    });
}

function AtualizarClick(e, sender) {
    _janelaColeta.Localidades.val(JSON.stringify(_janelaColetaLocalidade.Localidade.basicTable.BuscarRegistros()));
    _janelaColeta.UFs.val(JSON.stringify(_janelaColetaUF.UF.basicTable.BuscarRegistros()));
    _janelaColeta.Clientes.val(JSON.stringify(_janelaColetaCliente.Cliente.basicTable.BuscarRegistros()));
    _janelaColeta.PeriodosColeta.val(JSON.stringify(obterJanelasColetaPeriodo()));


    Salvar(_janelaColeta, "JanelaColeta/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
                _gridJanelaColeta.CarregarGrid();
                LimparCamposJanelaColeta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        ResetarTabs();
    });
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a janela de descarga " + _janelaColeta.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_janelaColeta, "JanelaColeta/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                    _gridJanelaColeta.CarregarGrid();
                    LimparCamposJanelaColeta();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposJanelaColeta();
}
