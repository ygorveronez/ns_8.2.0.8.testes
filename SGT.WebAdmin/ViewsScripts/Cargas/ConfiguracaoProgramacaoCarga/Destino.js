/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="ConfiguracaoProgramacaoCarga.js" />

// #region Objetos Globais do Arquivo

var _configuracaoProgramacaoCargaDestino;
var _gridDestino;
var _gridEstadoDestino;
var _gridRegiaoDestino;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoProgramacaoCargaDestino = function () {
    this.Destino = PropertyEntity({ type: types.event, text: "Adicionar Destino", idBtnSearch: guid(), idGrid: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), idGrid: guid() });
    this.RegiaoDestino = PropertyEntity({ type: types.event, text: "Adicionar Região", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDestino() {
    _configuracaoProgramacaoCargaDestino = new ConfiguracaoProgramacaoCargaDestino();
    KoBindings(_configuracaoProgramacaoCargaDestino, "knockoutCadastroConfiguracaoProgramacaoCargaDestino");

    loadGridDestino();
    loadGridEstadoDestino();
    loadGridRegiaoDestino();
}

function loadGridDestino() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoClick(_gridDestino, registroSelecionado); } }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridDestino = new BasicDataTable(_configuracaoProgramacaoCargaDestino.Destino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_configuracaoProgramacaoCargaDestino.Destino, null, null, null, _gridDestino, controlarVisibilidadeAbasDestino);

    _gridDestino.CarregarGrid([]);
}

function loadGridEstadoDestino() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoClick(_gridEstadoDestino, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridEstadoDestino = new BasicDataTable(_configuracaoProgramacaoCargaDestino.EstadoDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_configuracaoProgramacaoCargaDestino.EstadoDestino, null, _gridEstadoDestino, controlarVisibilidadeAbasDestino);

    _gridEstadoDestino.CarregarGrid([]);
}

function loadGridRegiaoDestino() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoClick(_gridRegiaoDestino, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridRegiaoDestino = new BasicDataTable(_configuracaoProgramacaoCargaDestino.RegiaoDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_configuracaoProgramacaoCargaDestino.RegiaoDestino, null, _gridRegiaoDestino, controlarVisibilidadeAbasDestino);

    _gridRegiaoDestino.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirDestinoClick(gridDestino, registroSelecionado) {
    var listaDestino = gridDestino.BuscarRegistros().slice();

    for (var i = 0; i < listaDestino.length; i++) {
        if (registroSelecionado.Codigo == listaDestino[i].Codigo) {
            listaDestino.splice(i, 1);
            break;
        }
    }

    gridDestino.CarregarGrid(listaDestino);
    controlarVisibilidadeAbasDestino();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposDestino() {
    _gridDestino.CarregarGrid([]);
    _gridEstadoDestino.CarregarGrid([]);
    _gridRegiaoDestino.CarregarGrid([]);

    controlarVisibilidadeAbasDestino();

    $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
}

function possuiDestinoInformado() {
    var totalDestinos = _gridDestino.BuscarRegistros().length;
    var totalEstadosDestino = _gridEstadoDestino.BuscarRegistros().length;
    var totalRegioesDestino = _gridRegiaoDestino.BuscarRegistros().length;

    return (totalDestinos > 0) || (totalEstadosDestino > 0) || (totalRegioesDestino > 0);
}

function preencherDestino(configuracaoProgramacaoCarga) {
    _gridDestino.CarregarGrid(configuracaoProgramacaoCarga.Destino);
    _gridEstadoDestino.CarregarGrid(configuracaoProgramacaoCarga.EstadoDestino);
    _gridRegiaoDestino.CarregarGrid(configuracaoProgramacaoCarga.RegiaoDestino);

    controlarVisibilidadeAbasDestino();
}

function preencherDestinoSalvar(configuracaoProgramacaoCarga) {
    configuracaoProgramacaoCarga["Destino"] = JSON.stringify(_gridDestino.BuscarRegistros().slice());
    configuracaoProgramacaoCarga["EstadoDestino"] = JSON.stringify(_gridEstadoDestino.BuscarRegistros().slice());
    configuracaoProgramacaoCarga["RegiaoDestino"] = JSON.stringify(_gridRegiaoDestino.BuscarRegistros().slice());
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarVisibilidadeAbasDestino() {
    if (_gridDestino.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").show();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();

        $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
    }
    else if (_gridEstadoDestino.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabEstadosDestino").show();
        $("#liTabRegioesDestino").hide();

        $(".nav-tabs a[href='#tabEstadosDestino']").tab('show');
    }
    else if (_gridRegiaoDestino.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").show();

        $(".nav-tabs a[href='#tabRegioesDestino']").tab('show');
    }
    else {
        $("#liTabCidadesDestino").show();
        $("#liTabEstadosDestino").show();
        $("#liTabRegioesDestino").show();
    }
}

// #endregion Funções Privadas

