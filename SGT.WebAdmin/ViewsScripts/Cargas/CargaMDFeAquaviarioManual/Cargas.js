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
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargas, _cargasMDFeManual;

var CargasMDFeManual = function () {
    this.CargasInfo = PropertyEntity({ type: types.map, required: false, text: "Informar Cargas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.ListaCargas = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
};

function LoadCargas() {
    _cargasMDFeManual = new CargasMDFeManual();
    KoBindings(_cargasMDFeManual, "knockoutCargas");
    RecarregarCargas();
}

function RecarregarCargas(data) {
    $("#" + _cargasMDFeManual.CargasInfo.idBtnSearch).unbind();

    if (data == null)
        data = new Array();

    if (_gridCargas != null) {
        _gridCargas.Destroy();
        _gridCargas = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: function (data) {
            RemoverCargaClick(_cargasMDFeManual.CargasInfo, data)
        },
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeAquaviario.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Carga", width: "18%", className: "text-align-center" },
        { data: "OrigemDestino", title: "Origem e Destino", width: "50%", className: "text-align-left" },
        { data: "Veiculo", title: "Veiculo", width: "20%", className: "text-align-left" }
    ];

    _gridCargas = new BasicDataTable(_cargasMDFeManual.CargasInfo.idGrid, header, menuOpcoes);
    _cargasMDFeManual.CargasInfo.basicTable = _gridCargas;

    _cargasMDFeManual.ListaCargas.val(data);
    _gridCargas.CarregarGrid(data);

    new BuscarCargasPermiteMDFeManual(_cargasMDFeManual.CargasInfo, _terminais.Empresa, retornoCargas, _gridCargas);
}



function retornoCargas(cargas) {

    var arrayEscolhidos = new Array();
    for (var i = 0; i < _gridCargas.BasicTable().rows().data().length; i++) {
        arrayEscolhidos.push(_gridCargas.BasicTable().rows().data()[i]);
    }
    var colunasNaoEncontradas = [];
    for (var i = 0; i < cargas.length; i++) {
        var obj = new Object();
        for (var j = 0; j < _gridCargas.BasicTable().columns().data().context[0].aoColumns.length; j++) {
            var indiceTabela = _gridCargas.BasicTable().columns().data().context[0].aoColumns[j].data;
            if (indiceTabela != null) {
                obj[indiceTabela] = cargas[i][indiceTabela];
                if (obj[indiceTabela] == null) {
                    colunasNaoEncontradas.push(indiceTabela);
                }
            }
        }
        if (colunasNaoEncontradas.length == 0) {
            arrayEscolhidos.push(obj);
        } else {
            break;
        }
    }

    if (colunasNaoEncontradas.length == 0) {
        _gridCargas.CarregarGrid(arrayEscolhidos);
        buscarDestinosCargas();
    } else {
        exibirMensagem(tipoMensagem.falha, "A seleção não retornou o(s) campo(s) " + colunasNaoEncontradas.join(", ") + ". Solicitar a Multisoftware que adicione o(s) campo(s) no retorno, ou crie um callback exclusivo para os dados retornados");
    }


}

function buscarDestinosCargas() {

    var arrayEscolhidosCargas = new Array();
    for (var i = 0; i < _gridCargas.BasicTable().rows().data().length; i++) {
        arrayEscolhidosCargas.push(_gridCargas.BasicTable().rows().data()[i]);
    }

    var arrayEscolhidosCTe = new Array();
    for (var i = 0; i < _gridCtes.BasicTable().rows().data().length; i++) {
        arrayEscolhidosCTe.push(_gridCtes.BasicTable().rows().data()[i]);
    }

    var codigos = new Array();
    for (var i = 0; i < arrayEscolhidosCargas.length; i++) {
        codigos.push(arrayEscolhidosCargas[i].Codigo);
    }

    var codigosCTes = new Array();
    for (var i = 0; i < arrayEscolhidosCTe.length; i++) {
        codigosCTes.push(arrayEscolhidosCTe[i].CodigoCTE);
    }

    var data = { Cargas: JSON.stringify(codigos), CTes: JSON.stringify(codigosCTes), Origem: _cargaMDFeAquaviario.Origem.codEntity() };
    if (codigos.length > 0 || codigosCTes.length > 0) {
        executarReST("CargaMDFeManualCargas/BuscarDadosDasCargas", data, function (arg) {
            if (arg.Success) {
                var retorno = arg.Data;
                if (_cargaMDFeAquaviario.Origem.val() == "") {
                    _cargaMDFeAquaviario.Origem.val(retorno.Origem.Descricao);
                    _cargaMDFeAquaviario.Origem.codEntity(retorno.Origem.Codigo);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function RemoverCargaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a carga " + sender.CodigoCargaEmbarcador + "?", function () {
        var cargaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < cargaGrid.length; i++) {
            if (sender.Codigo == cargaGrid[i].Codigo) {
                cargaGrid.splice(i, 1);
                break;
            }
        }

        e.basicTable.CarregarGrid(cargaGrid);
        _cargasMDFeManual.ListaCargas.val(cargaGrid);

        var arrayEscolhidos = new Array();
        for (var i = 0; i < _gridCargas.BasicTable().rows().data().length; i++) {
            arrayEscolhidos.push(_gridCargas.BasicTable().rows().data()[i]);
        }

        buscarDestinosCargas();

    });
}