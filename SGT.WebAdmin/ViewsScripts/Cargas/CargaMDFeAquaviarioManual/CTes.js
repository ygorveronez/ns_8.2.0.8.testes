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
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCtes, _ctesMDFeManual;

var CTeMDFeManual = function () {
    this.CtesInfo = PropertyEntity({ type: types.map, required: false, text: "Informar CT-es", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.ListaCTes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.BuscarCTes = PropertyEntity({ eventClick: BuscarCTesClick, type: types.event, text: "Buscar CT-es automaticamente", visible: ko.observable(true), enable: ko.observable(true) });
    this.LimparCTes = PropertyEntity({ eventClick: LimparCTesClick, type: types.event, text: "Limpar CT-es", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadCTes() {

    _ctesMDFeManual = new CTeMDFeManual();
    KoBindings(_ctesMDFeManual, "knockoutCTes");

    RecarregarCTes();
}

function LimparCTesClick(e, data){
    exibirConfirmacao("Confirmação", "Deseja realmente remover todos os CT-es lançados neste MDF-e?", function () {
        RemoverTodosCTes();
    });
}

function BuscarCTesClick(e, data) {
    if (ValidarCamposObrigatorios(_cargaMDFeAquaviario)) {
        exibirConfirmacao("Confirmação", "Deseja buscar os conhecimentos de forma automática com os filtros selecionados?", function () {

            var data = { PedidoViagemNavio: _terminais.PedidoViagemNavio.codEntity(), PortoOrigem: _cargaMDFeAquaviario.PortoOrigem.codEntity(), PortoDestino: _cargaMDFeAquaviario.PortoDestino.codEntity(), ListaTerminalOrigem: PreencherListaCodigosTerminalOrigem(), ListaTerminalDestino: PreencherListaCodigosTerminalDestino() };
            executarReST("CargaMDFeAquaviarioManual/BuscarCTesAutomaticamente", data, function (arg) {
                if (arg.Success) {

                    RemoverTodosCTes();

                    var cteGrid = _ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros();
                    codigosCtes = new Array();

                    for (var i = 0; i < cteGrid.length; i++) {
                        codigosCtes.push(cteGrid[i].Codigo);
                    }

                    var retorno = arg.Data;
                    var ctes = _ctesMDFeManual.ListaCTes.val();

                    for (var i = 0; i < retorno.length; i++) {
                        if ($.inArray(retorno[i].Codigo, codigosCtes) < 0)
                            ctes.push(retorno[i]);
                    }

                    _ctesMDFeManual.ListaCTes.val(ctes);
                    _gridCtes.CarregarGrid(ctes);
                    buscarDestinosCargas();

                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios antes de buscar os conhecimentos.");
    }
}

function AdicionarCTes(data) {
    var ctes = _ctesMDFeManual.ListaCTes.val();

    for (var i = 0; i < data.length; i++) {

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

    if (_cargaMDFeAquaviario.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
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
        new BuscarCargaCTe(_ctesMDFeManual.CtesInfo, AdicionarCTes, _gridCtes, _terminais.Empresa, true);
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

function PreencherListaCodigosTerminalDestino() {
    var codigosTerminalDestino = new Array();

    $.each(_gridTerminaisDescarregamento.BuscarRegistros(), function (i, titulo) {
        codigosTerminalDestino.push({ Codigo: titulo.Codigo });
    });

    return JSON.stringify(codigosTerminalDestino);
}

function PreencherListaCodigosTerminalOrigem() {
    var codigosTerminalOrigem = new Array();

    $.each(_gridTerminaisCarregamento.BuscarRegistros(), function (i, titulo) {
        codigosTerminalOrigem.push({ Codigo: titulo.Codigo });
    });

    return JSON.stringify(codigosTerminalOrigem);
}

function RemoverTodosCTes() {
    var data = new Array();

    _ctesMDFeManual.CtesInfo.basicTable.CarregarGrid(data);
    _ctesMDFeManual.ListaCTes.val(data);
}