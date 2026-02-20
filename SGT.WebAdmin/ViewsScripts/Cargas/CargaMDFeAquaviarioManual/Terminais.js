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
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTerminaisCarregamento = null;
var _gridTerminaisDescarregamento = null;
var _terminais;

var Terminais = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), issue: 69, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Navio/Viagem/Direção:"), idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(true) });

    this.TerminalCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Terminal de Carregamento", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TerminalDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Terminal de Descarregamento", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });

}

//*******EVENTOS*******

function loadTerminais() {
    _terminais = new Terminais();
    KoBindings(_terminais, "tabTerminais");

    new BuscarTransportadores(_terminais.Empresa, callbackTransportador);
    new BuscarPedidoViagemNavio(_terminais.PedidoViagemNavio);

    var menuOpcoesCarregamento = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCarregamentoClick(_terminais.TerminalCarregamento, data);
            }
        }]
    };

    var headerCarregamento = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTerminaisCarregamento = new BasicDataTable(_terminais.TerminalCarregamento.idGrid, headerCarregamento, menuOpcoesCarregamento, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerminalImportacao(_terminais.TerminalCarregamento, RetornoTerminalCarregamento, _gridTerminaisCarregamento);

    _terminais.TerminalCarregamento.basicTable = _gridTerminaisCarregamento;
    _terminais.TerminalCarregamento.basicTable.CarregarGrid(new Array());

    var menuOpcoesDescarregamento = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDescarregamentoClick(_terminais.TerminalDescarregamento, data);
            }
        }]
    };

    var headerDescarregamento = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTerminaisDescarregamento = new BasicDataTable(_terminais.TerminalDescarregamento.idGrid, headerDescarregamento, menuOpcoesDescarregamento, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerminalImportacao(_terminais.TerminalDescarregamento, RetornoTerminalDescarregamento, _gridTerminaisDescarregamento);

    _terminais.TerminalDescarregamento.basicTable = _gridTerminaisDescarregamento;
    _terminais.TerminalDescarregamento.basicTable.CarregarGrid(new Array());
}

function RetornoTerminalCarregamento(data) {
    if (data != null && data.length > 0) {
        if (data[0].CodigoEmpresa > 0) {
            _terminais.Empresa.val(data[0].Empresa);
            _terminais.Empresa.codEntity(data[0].CodigoEmpresa);

            SetarEnableCamposKnockout(_cargasMDFeManual, true);
            SetarEnableCamposKnockout(_ctesMDFeManual, true);
            _terminais.Empresa.enable(false);
        }

        if (data[0].CodigoPorto > 0) {
            _cargaMDFeAquaviario.PortoOrigem.val(data[0].Porto);
            _cargaMDFeAquaviario.PortoOrigem.codEntity(data[0].CodigoPorto);
        }

        if (data[0].CodigoLocalidade > 0) {
            _cargaMDFeAquaviario.Origem.val(data[0].Localidade);
            _cargaMDFeAquaviario.Origem.codEntity(data[0].CodigoLocalidade);
        }
        _gridTerminaisCarregamento.CarregarGrid(data);
    }
}

function RetornoTerminalDescarregamento(data) {
    if (data != null && data.length > 0) {
        if (data[0].CodigoPorto > 0) {
            _cargaMDFeAquaviario.PortoDestino.val(data[0].Porto);
            _cargaMDFeAquaviario.PortoDestino.codEntity(data[0].CodigoPorto);
        }
        _gridTerminaisDescarregamento.CarregarGrid(data);
    }
}

function ExcluirCarregamentoClick(knoutCarregamento, data) {
    var carregamentoGrid = knoutCarregamento.basicTable.BuscarRegistros();

    for (var i = 0; i < carregamentoGrid.length; i++) {
        if (data.Codigo == carregamentoGrid[i].Codigo) {
            carregamentoGrid.splice(i, 1);
            break;
        }
    }

    knoutCarregamento.basicTable.CarregarGrid(carregamentoGrid);
}

function ExcluirDescarregamentoClick(knoutDescarregamento, data) {
    var descarregamentoGrid = knoutDescarregamento.basicTable.BuscarRegistros();

    for (var i = 0; i < descarregamentoGrid.length; i++) {
        if (data.Codigo == descarregamentoGrid[i].Codigo) {
            descarregamentoGrid.splice(i, 1);
            break;
        }
    }

    knoutDescarregamento.basicTable.CarregarGrid(descarregamentoGrid);
}