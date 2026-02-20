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

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "ExigeConfirmacaoTracao", visible: false },
        { data: "NumeroReboques", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Carga", width: "18%", className: "text-align-center" },
        { data: "OrigemDestino", title: "Origem e Destino", width: "50%", className: "text-align-left" },
        { data: "Veiculo", title: "Veiculo", width: "20%", className: "text-align-left" }
    ];

    _gridCargas = new BasicDataTable(_cargasMDFeManual.CargasInfo.idGrid, header, menuOpcoes);
    _cargasMDFeManual.CargasInfo.basicTable = _gridCargas;

    _cargasMDFeManual.ListaCargas.val(data);
    _gridCargas.CarregarGrid(data);

    new BuscarCargasPermiteMDFeManual(_cargasMDFeManual.CargasInfo, _cargaMDFeManual.Empresa, retornoCargas, _gridCargas);
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
        VerificarRegraVeiculo();
    } else {
        exibirMensagem(tipoMensagem.falha, "A seleção não retornou o(s) campo(s) " + colunasNaoEncontradas.join(", ") + ". Solicitar a Multisoftware que adicione o(s) campo(s) no retorno, ou crie um callback exclusivo para os dados retornados");
    }
}

function buscarDestinosCargas() {
    _cargaMDFeManual.ListaValePedagio.val([]);
    _gridValePedagioMDFeManual.CarregarGrid(_cargaMDFeManual.ListaValePedagio.val());

    _cargaMDFeManual.ListaCIOT.val([]);
    _gridCIOTMDFeManual.CarregarGrid(_cargaMDFeManual.ListaCIOT.val());

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

    if (codigos.length == 0 && codigosCTes.length == 0)
        return;

    var data = {
        Cargas: JSON.stringify(codigos),
        CTes: JSON.stringify(codigosCTes),
        Origem: _cargaMDFeManual.Origem.codEntity()
    };
    executarReST("CargaMDFeManualCargas/BuscarDadosDasCargas", data, function (arg) {
        if (arg.Success) {
            let retorno = arg.Data;

            if (_cargaMDFeManual.Origem.val() == "") {
                _cargaMDFeManual.Origem.val(retorno.Origem.Descricao);
                _cargaMDFeManual.Origem.codEntity(retorno.Origem.Codigo);
            }

            if (retorno.PossuiDestinoExterior === true) {
                _cargaMDFeManual.UsarDadosCTe.val(false);
                usarDadosCTeClick();
                if (retorno.Destinos.length > 0) {
                    _cargaMDFeManual.Destino.codEntity(retorno.Destinos[0].Codigo);
                    _cargaMDFeManual.Destino.val(retorno.Destinos[0].Descricao);
                }
            }

            SetarInformacoesVeiculo(retorno.Veiculo, retorno.Reboques);

            let motoristaGrid = _cargaMDFeManual.Motorista.basicTable.BuscarRegistros();
            if (motoristaGrid == null || motoristaGrid.length <= 0)
                _cargaMDFeManual.Motorista.basicTable.CarregarGrid([{ Codigo: retorno.Motorista.Codigo, Descricao: retorno.Motorista.Descricao, PercentualExecucao: retorno.Motorista.PercentualExecucao }]);

            _cargaMDFeManual.ListaDestinos.val(retorno.Destinos);

            if (retorno.ValePedagio != null && retorno.ValePedagio.length > 0) {
                var valePedagio = _cargaMDFeManual.ListaValePedagio.val();
                for (var i = 0; i < retorno.ValePedagio.length; i++) {
                    var ObjValePedagio = {
                        Codigo: guid(),
                        FornecedorValePedagio: retorno.ValePedagio[i].FornecedorValePedagio,
                        ConsultarFornecedorValePedagio: retorno.ValePedagio[i].ConsultarFornecedorValePedagio,
                        ResponsavelValePedagio: retorno.ValePedagio[i].ResponsavelValePedagio,
                        ConsultarResponsavelValePedagio: retorno.ValePedagio[i].ConsultarResponsavelValePedagio,
                        ComprovanteValePedagio: retorno.ValePedagio[i].ComprovanteValePedagio,
                        ValorValePedagio: retorno.ValePedagio[i].ValorValePedagio,
                    }
                    valePedagio.push(ObjValePedagio);

                    _cargaMDFeManual.ListaValePedagio.val(valePedagio);
                    _gridValePedagioMDFeManual.CarregarGrid(_cargaMDFeManual.ListaValePedagio.val());
                }
            }
            if (retorno.CIOT != null && retorno.CIOT.length > 0) {
                var ciot = _cargaMDFeManual.ListaCIOT.val();
                for (var i = 0; i < retorno.CIOT.length; i++) {

                    var ObjCIOT = {
                        Codigo: guid(),
                        NumeroCIOT: retorno.CIOT[i].NumeroCIOT,
                        ResponsavelCIOT: retorno.CIOT[i].ResponsavelCIOT,
                        ConsultarResponsavelCIOT: retorno.CIOT[i].ConsultarResponsavelCIOT,
                    }
                    ciot.push(ObjCIOT);

                    _cargaMDFeManual.ListaCIOT.val(ciot);
                    _gridCIOTMDFeManual.CarregarGrid(_cargaMDFeManual.ListaCIOT.val());
                }
            }

            recarregarGridReorder();
            BuscarPercursoMDFe();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
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
        VerificarRegraVeiculo();
    });
}