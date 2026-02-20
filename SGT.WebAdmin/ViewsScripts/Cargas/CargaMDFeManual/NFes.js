//*******MAPEAMENTO KNOUCKOUT*******

var _gridNFes, _nfesMDFeManual;

var NFesMDFeManual = function () {
    this.NFesInfo = PropertyEntity({ type: types.map, required: false, text: "Informar NF-e", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.ListaNFes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
};

function LoadNFes() {
    _nfesMDFeManual = new NFesMDFeManual();
    KoBindings(_nfesMDFeManual, "knockoutNFes");
    RecarregarNFes();
}

function RecarregarNFes(data) {
    $("#" + _nfesMDFeManual.NFesInfo.idBtnSearch).unbind();

    if (data == null)
        data = new Array();

    if (_gridNFes != null) {
        _gridNFes.Destroy();
        _gridNFes = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: function (data) {
            RemoverNFeClick(_nfesMDFeManual.NFesInfo, data)
        },
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },                
        { data: "Chave", title: "Chave", width: "90%", className: "text-align-left" }
    ];

    _gridNFes = new BasicDataTable(_nfesMDFeManual.NFesInfo.idGrid, header, menuOpcoes);
    _nfesMDFeManual.NFesInfo.basicTable = _gridNFes;

    _nfesMDFeManual.ListaNFes.val(data);
    _gridNFes.CarregarGrid(data);

    new BuscarNFesPermiteMDFeManual(_nfesMDFeManual.NFesInfo, _cargaMDFeManual.Empresa, retornoNFes, _gridNFes);
}

function retornoNFes(nfes) {

    var arrayEscolhidos = new Array();
    for (var i = 0; i < _gridNFes.BasicTable().rows().data().length; i++) {
        arrayEscolhidos.push(_gridNFes.BasicTable().rows().data()[i]);
    }
    var colunasNaoEncontradas = [];
    for (var i = 0; i < nfes.length; i++) {
        var obj = new Object();
        for (var j = 0; j < _gridNFes.BasicTable().columns().data().context[0].aoColumns.length; j++) {
            var indiceTabela = _gridNFes.BasicTable().columns().data().context[0].aoColumns[j].data;
            if (indiceTabela != null) {
                obj[indiceTabela] = nfes[i][indiceTabela];
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
        _gridNFes.CarregarGrid(arrayEscolhidos);
        buscarDestinosNFes();
        VerificarRegraVeiculo();
    } else {
        exibirMensagem(tipoMensagem.falha, "A seleção não retornou o(s) campo(s) " + colunasNaoEncontradas.join(", ") + ". Solicitar a Multisoftware que adicione o(s) campo(s) no retorno, ou crie um callback exclusivo para os dados retornados");
    }
}

function RemoverNFeClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a NF-e " + sender.Chave + "?", function () {
        var nfeGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < nfeGrid.length; i++) {
            if (sender.Codigo == nfeGrid[i].Codigo) {
                nfeGrid.splice(i, 1);
                break;
            }
        }

        e.basicTable.CarregarGrid(nfeGrid);
        _nfesMDFeManual.ListaNFes.val(nfeGrid);

        var arrayEscolhidos = new Array();
        for (var i = 0; i < _gridNFes.BasicTable().rows().data().length; i++) {
            arrayEscolhidos.push(_gridNFes.BasicTable().rows().data()[i]);
        }

        buscarDestinosNFes();
        VerificarRegraVeiculo();
    });
}


function buscarDestinosNFes() {
    var arrayEscolhidosNFes = new Array();
    for (var i = 0; i < _gridNFes.BasicTable().rows().data().length; i++) {
        arrayEscolhidosNFes.push(_gridNFes.BasicTable().rows().data()[i]);
    }

    var chaves = new Array();
    for (var i = 0; i < arrayEscolhidosNFes.length; i++) {
        chaves.push(arrayEscolhidosNFes[i].Chave);
    }

    if (chaves.length == 0)
        return;

    var data = {
        NFes: JSON.stringify(chaves),
        Origem: _cargaMDFeManual.Origem.codEntity()
    };
    executarReST("CargaMDFeManualCargas/BuscarDadosDasCargas", data, function (arg) {
        if (arg.Success) {
            let retorno = arg.Data;

            if (_cargaMDFeManual.Origem.val() == "" && retorno.Origem != null && retorno.Origem != undefined) {
                _cargaMDFeManual.Origem.val(retorno.Origem.Descricao);
                _cargaMDFeManual.Origem.codEntity(retorno.Origem.Codigo);
            }

            if (retorno.PossuiDestinoExterior === true) {
                _cargaMDFeManual.UsarDadosCTe.val(false);
                usarDadosCTeClick();
                if (retorno.Destinos != null && retorno.Destinos != undefined && retorno.Destinos.length > 0) {
                    _cargaMDFeManual.Destino.codEntity(retorno.Destinos[0].Codigo);
                    _cargaMDFeManual.Destino.val(retorno.Destinos[0].Descricao);
                }
            }

            if (retorno.Destinos != null && retorno.Destinos != undefined)
            _cargaMDFeManual.ListaDestinos.val(retorno.Destinos);

            recarregarGridReorder();
            BuscarPercursoMDFe();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}