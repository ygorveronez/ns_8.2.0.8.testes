//*******MAPEAMENTO KNOUCKOUT*******

var _gridCtes, _ctesMDFeManual;

var CTeMDFeManual = function () {
    this.CtesInfo = PropertyEntity({ type: types.map, required: false, text: "Informar CT-es", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.ListaCTes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
};

function LoadCTes() {

    _ctesMDFeManual = new CTeMDFeManual();
    KoBindings(_ctesMDFeManual, "knockoutCTes");

    RecarregarCTes();
}

function AdicionarCTes(data) {
    var ctes = _ctesMDFeManual.ListaCTes.val();

    for (var i = 0; i < data.length; i++) {

        //if (_cargaMDFeManual.Origem.val() == "") {
        //    _cargaMDFeManual.Origem.val(data[i].Origem);
        //    _cargaMDFeManual.Origem.codEntity(data[i].CodigoOrigem);
        //    BuscarPercursoMDFe();
        //}

        //if (!_cargaMDFeManual.UsarDadosCTe.val()) {
        //    if (_cargaMDFeManual.Destino.val() == "") {
        //        _cargaMDFeManual.Destino.val(data[i].Destino);
        //        _cargaMDFeManual.Destino.codEntity(data[i].CodigoDestino);
        //        BuscarPercursoMDFe();
        //    }
        //} 

        //if (data[i].NumeroApolice != "") {
        //    _seguroMDFeManual.TipoSeguro.val(data[i].TipoSeguro);
        //    _seguroMDFeManual.CNPJSeguradoraSeguro.val(data[i].CNPJSeguradora);
        //    _seguroMDFeManual.NomeSeguradoraSeguro.val(data[i].NomeSeguradora);
        //    _seguroMDFeManual.NomeResponsavelSeguro.val(data[i].CNPJEmpresa);
        //    _seguroMDFeManual.ApoliceSeguro.val(data[i].NumeroApolice);
        //    _seguroMDFeManual.AverbacaoSeguro.val(data[i].NumeroAverbacao);

        //    var seguro = _cargaMDFeManual.ListaSeguro.val();
        //    _seguroMDFeManual.Codigo.val(guid());
        //    seguro.push(RetornarObjetoPesquisa(_seguroMDFeManual));
        //    _cargaMDFeManual.ListaSeguro.val(seguro);
        //    _gridSeguroMDFeManual.CarregarGrid(_cargaMDFeManual.ListaSeguro.val());
        //    LimparCamposSeguro();
        //    //AdicionarSeguroClick();            
        //}        

        ctes.push(data[i]);
    }

    _ctesMDFeManual.ListaCTes.val(ctes);
    _gridCtes.CarregarGrid(ctes);
    buscarDestinosCargas();
}

function RecarregarCTes(data) {
    $("#" + _ctesMDFeManual.CtesInfo.idBtnSearch).unbind();

    if (data == null)
        data = new Array();

    if (_gridCtes != null) {
        _gridCtes.Destroy();
        _gridCtes = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: function (data) {
            RemoverCTeClick(_ctesMDFeManual.CtesInfo, data)
        },
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCTE", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "Carga", title: "Carga", width: "18%", className: "text-align-left" },
        { data: "Numero", title: "CT-e", width: "18%", className: "text-align-left" },
        { data: "Notas", title: "Notas", width: "27%", className: "text-align-left" },
        { data: "Destino", title: "Destino", width: "30%", className: "text-align-left" }
    ];

    _gridCtes = new BasicDataTable(_ctesMDFeManual.CtesInfo.idGrid, header, menuOpcoes);
    _ctesMDFeManual.CtesInfo.basicTable = _gridCtes;

    _ctesMDFeManual.ListaCTes.val(data);
    _gridCtes.CarregarGrid(data);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        new BuscarCargaCTeMultiCTe(_ctesMDFeManual.CtesInfo, AdicionarCTes, _gridCtes);
    else
        new BuscarCargaCTe(_ctesMDFeManual.CtesInfo, AdicionarCTes, _gridCtes, _cargaMDFeManual.Empresa);
}

function RemoverCTeClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o CT-e " + sender.Numero + "?", function () {
        var cteGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < cteGrid.length; i++) {
            if (sender.Codigo == cteGrid[i].Codigo) {
                cteGrid.splice(i, 1);
                break;
            }
        }

        e.basicTable.CarregarGrid(cteGrid);
        _ctesMDFeManual.ListaCTes.val(cteGrid);

        buscarDestinosCargas();
    });
}

function HabilitarSelecaoCTe() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        $("#tabCTes").show();
        Global.ExibirAba("knockoutCTes");
    }
}

function DesativarSelecaoCTe() {
    $("#tabCTes").hide();
    Global.ExibirAba("knockoutCargas");
}