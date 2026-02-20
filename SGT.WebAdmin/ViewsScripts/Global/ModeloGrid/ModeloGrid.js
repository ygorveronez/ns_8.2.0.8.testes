
var _modeloGrid;
var _gridModelosGrid;

var ModeloGrid = function () {
    this.UrlGrid = PropertyEntity({ val: ko.observable(""), def: "" });
    this.IdGrid = PropertyEntity({ val: ko.observable(""), def: "" });
    this.DadosGrid = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CallbackAplicar = function () { };
    this.CallbackAplicarUsuario = function () { };

    this.GridModelos = PropertyEntity({ getTypes: typesKnockout.basicTable, type: types.local });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: adicionarModeloGridClick });
    this.AplicarPreferenciaUsuario = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.AplicarPreferenciaUsuario, eventClick: aplicarPreferenciaUsuario, visible: ko.observable(false) });
}

function loadModeloGrid() {
    _modeloGrid = new ModeloGrid();
    KoBindings(_modeloGrid, "knockoutModeloGrid");

    loadModeloGridAdicionar();
    LoadGridModelosGrid();

    $modalModeloGrid = $("#modal-modelo-grid");
    $modalModeloGrid.on('show.bs.modal', function () {
        BuscarModelosGrid();
    });
    $modalModeloGrid.on('hidden.bs.modal', function () {
        _gridModelosGrid.CarregarGrid([]);
        LimparCampos(_modeloGrid);
    });
}

function LoadGridModelosGrid() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModelo", visible: false },
        { data: "DescricaoModelo", title: Localization.Resources.Gerais.Geral.Modelo, width: "50%", className: "text-align-left" },
        { data: "ModeloPadrao", title: Localization.Resources.Gerais.Geral.ModeloPadrao, width: "30%", className: "text-align-center" },
    ];

    var opcaoAplicarModelo = { descricao: Localization.Resources.Gerais.Geral.Aplicar, id: guid(), metodo: aplicarModeloGrid };
    var opcaoRemoverModelo = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerModeloGrid };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 20, opcoes: [opcaoAplicarModelo, opcaoRemoverModelo] };

    _gridModelosGrid = new BasicDataTable(_modeloGrid.GridModelos.id, header, menuOpcoes, null, null, 5);
    _gridModelosGrid.CarregarGrid([]);
}

function removerModeloGrid(data) {
    executarReST("Preferencias/RemoverModeloGrid", { CodigoModelo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                BuscarModelosGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function aplicarModeloGrid(data) {
    Global.fecharModal("modal-modelo-grid");
    _modeloGrid.CallbackAplicar(data.Codigo);
}

function adicionarModeloGridClick() {
    Global.abrirModal("modal-modelo-grid-adicionar");
}

function aplicarPreferenciaUsuario() {
    Global.fecharModal("modal-modelo-grid");
    _modeloGrid.CallbackAplicarUsuario();
}

function BuscarModelosGrid() {
    executarReST("Preferencias/BuscarModelosGrid", { UrlGrid: _modeloGrid.UrlGrid.val(), IdGrid: _modeloGrid.IdGrid.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _modeloGrid.AplicarPreferenciaUsuario.visible(r.Data.PossuiConfiguraoPorUsuario);
                _gridModelosGrid.CarregarGrid(r.Data.Modelos);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}