/// <reference path="Raca.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _formulario, _gridEspecie, _pesquisa, _cadastroEspecie, _crudEspecie;

var CriarFormulario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoEspecie = PropertyEntity({ text: "*Espécie:", required: true });
    this.Ativo = PropertyEntity({ text: "Status:", val: ko.observable(true), options: _status, def: true });

    this.Racas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.PosicaoGrid = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoRaca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoRaca = PropertyEntity({ text: "Raça:" });
    this.AtivoRaca = PropertyEntity({ text: "Status:", val: ko.observable(true), options: _status, def: true });
    this.AdicionarRaca = PropertyEntity({ eventClick: AdicionarRacaClick, type: types.event, text: ko.observable("Adicionar raça"), val: ko.observable(true), visible: ko.observable(true) });
};

var CriarCRUD = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var CriarFormularioPesquisa = function () {
    this.DescricaoEspecie = PropertyEntity({ text: "Espécie: " });
    this.Ativo = PropertyEntity({ val: ko.observable(_statusPesquisa.Todos), options: _statusPesquisa, def: _statusPesquisa.Todos, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEspecie.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

function loadEspecie() {
    _formulario = new CriarFormulario();
    KoBindings(_formulario, "knockoutCadastro");

    HeaderAuditoria("Especie", _formulario);

    _crudEspecie = new CriarCRUD();
    KoBindings(_crudEspecie, "knockoutCRUD");

    _pesquisa = new CriarFormularioPesquisa();
    KoBindings(_pesquisa, "knockoutPesquisaEspecie", false, _pesquisa.Pesquisar.id);

    BuscarEspecie();

    loadRacas();
}

function BuscarEspecie() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarEspecie, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridEspecie = new GridView(_pesquisa.Pesquisar.idGrid, "Especie/Pesquisar", _pesquisa, menuOpcoes, null);
    _gridEspecie.CarregarGrid();
}

function EditarEspecie(linhaGridPesquisa) {
    LimparCamposEspecie();
    LimparGridRacas();

    _formulario.Codigo.val(linhaGridPesquisa.Codigo);
    _formulario.DescricaoEspecie.val(linhaGridPesquisa.Descricao);
    _formulario.Ativo.val(linhaGridPesquisa.DescricaoAtivo == "Ativo");

    BuscarPorCodigo(_formulario, "Especie/BuscarPorCodigo", function (arg) {
        _pesquisa.ExibirFiltros.visibleFade(false);
        _crudEspecie.Atualizar.visible(true);
        _crudEspecie.Cancelar.visible(true);
        _crudEspecie.Excluir.visible(true);
        _crudEspecie.Adicionar.visible(false);
        RecarregarGridRacas();
    }, null);
}

function AdicionarClick(e, sender) {
    Salvar(_formulario, "Especie/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridEspecie.CarregarGrid();
                LimparCamposEspecie();
                LimparGridRacas();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_formulario, "Especie/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEspecie.CarregarGrid();
                LimparCamposEspecie();
                LimparGridRacas();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a espécie " + _formulario.DescricaoEspecie.val() + "?", function () {
        ExcluirPorCodigo(_formulario, "Especie/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridEspecie.CarregarGrid();
                    LimparCamposEspecie();
                    LimparGridRacas();
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
    LimparCamposEspecie();
    LimparGridRacas(); 
}

function LimparCamposEspecie() {

    _crudEspecie.Atualizar.visible(false);
    _crudEspecie.Cancelar.visible(false);
    _crudEspecie.Excluir.visible(false);
    _crudEspecie.Adicionar.visible(true);

    LimparCampos(_formulario);
}