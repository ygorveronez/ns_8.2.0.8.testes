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

//*******MAPEAMENTO KNOUCKOUT*******

var _estadoDestino;
var _gridEstadoDestino;

var EstadoDestino = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.EstadoDestino, idBtnSearch: guid(), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador, idBtnSearch: guid() });
    this.EmiteMDFeFilialEmissoraPorEstadoDestino = PropertyEntity({ val: ko.observable(false), visible: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.EmitirMDFeCargasPorEstaFilialEmissora, issue: 1296, def: false });
    this.Adicionar = PropertyEntity({ eventClick: adicionarEstadoDestinoEmpresaEmissora, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: true });
}


//*******EVENTOS*******
function loadEstadoDestino() {
    _estadoDestino = new EstadoDestino();
    KoBindings(_estadoDestino, "knockoutEstadoDestino");
    loadGridEstadoDestino();

}

function loadGridEstadoDestino() {
    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirEstadoDestino }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Estado, width: "85%" },
        { data: "EmpresaCodigo", visible: false },
        { data: "EmpresaDescricao", title: Localization.Resources.Gerais.Geral.Transportador, width: "85%" },
    ];

    _gridEstadoDestino = new BasicDataTable(_estadoDestino.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estadoDestino.EstadoDestino);
    new BuscarEmpresa(_estadoDestino.Empresa);


    _gridEstadoDestino.CarregarGrid([]);
}

function adicionarEstadoDestinoEmpresaEmissora() {
    if (ValidarCamposObrigatorios(_estadoDestino)) {

        var estadoDestino = _gridEstadoDestino.BuscarRegistros();

        estadoDestino.push(obterEmpresaEmissoraSalvar());

        _gridEstadoDestino.CarregarGrid(estadoDestino);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function obterEmpresaEmissoraSalvar() {
    return {
        Codigo: _estadoDestino.EstadoDestino.codEntity(),
        Descricao: _estadoDestino.EstadoDestino.val(),
        EmpresaCodigo: _estadoDestino.Empresa.codEntity(),
        EmpresaDescricao: _estadoDestino.Empresa.val(),
    };
}

function excluirEstadoDestino(registroSelecionado) {
    var listaEstadoDestino = _gridEstadoDestino.BuscarRegistros().slice();

    for (var i = 0; i < listaEstadoDestino.length; i++) {
        if (registroSelecionado.Codigo == listaEstadoDestino[i].Codigo) {
            listaEstadoDestino.splice(i, 1);
            break;
        }
    }

    _gridEstadoDestino.CarregarGrid(listaEstadoDestino);
}

function recarregarGridEstadosDestino(retorno) {
    _gridEstadoDestino.CarregarGrid(retorno);
}

//*******METODOS*******
function limparCamposEstadoDestino() {
    LimparCampos(_estadoDestino);
}