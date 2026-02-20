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

//*******MAPEAMENTO*******

var _anexosContratoVeiculo;
var _contratoVeiculo;
var _gridContratoVeiculo;
var _gridAnexosContratoVeiculo;

var AnexosContratoVeiculo = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosContratoVeiculo();
    });
}

var ContratoVeiculo = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ContratoVeiculo = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadAnexosContratoVeiculo() {
    _anexosContratoVeiculo = new AnexosContratoVeiculo();
    KoBindings(_anexosContratoVeiculo, "knockoutAnexosContratoVeiculo");

    _contratoVeiculo = new ContratoVeiculo();
    KoBindings(_contratoVeiculo, "knockoutContratoVeiculo");

    GridContratoVeiculo();
    GridAnexosContratoVeiculo();
}

function downloadAnexoContratoVeiculoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaContratoVeiculoAnexo/DownloadAnexo", data);
}

function verAnexosContratoVeiculoClick(itemGrid) {
    _anexosContratoVeiculo.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    RenderizarGridAnexosContratoVeiculo();
    Global.abrirModal('divModalGerenciarAnexosContratoVeiculo');
}

//*******MÉTODOS*******
function GridContratoVeiculo() {
    //-- Grid Anexos
    // Menu
    var download = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Anexos,
        id: "clasEditar",
        evento: "onclick",
        metodo: verAnexosContratoVeiculoClick,
        tamanho: 9,
        icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    // Grid
    _gridContratoVeiculo = new GridView(_contratoVeiculo.ContratoVeiculo.idGrid, "OcorrenciaContratoVeiculo/Pesquisa", _contratoVeiculo, menuOpcoes);
}

function GridAnexosContratoVeiculo() {
    //-- Grid Anexos
    // Opcoes
    var download = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Download,
        id: guid(),
        evento: "onclick",
        metodo: downloadAnexoContratoVeiculoClick,
        tamanho: 9,
        icone: ""
    };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosContratoVeiculo = new BasicDataTable(_anexosContratoVeiculo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosContratoVeiculo.CarregarGrid([]);
}

function GetAnexosContratoVeiculo(todosAnexos) {
    var codigoVeiculo = _anexosContratoVeiculo.CodigoVeiculo.val();

    // Retorna um clone do array para não prender a referencia
    var lista = (_anexosContratoVeiculo.Anexos.val() || []).slice();

    return lista.filter(function (anexo) {
        if (todosAnexos || anexo.CodigoVeiculo == codigoVeiculo)
            return anexo;
    });
}

function RenderizarGridAnexosContratoVeiculo() {
    // Busca a lista
    var anexos = GetAnexosContratoVeiculo();

    // E chama o metodo da grid
    _gridAnexosContratoVeiculo.CarregarGrid(anexos);
}


function CarregarAnexosContratoVeiculo() {
    executarReST("OcorrenciaContratoVeiculoAnexo/BuscarAnexosVeiculoContratoPorCodigo", { Ocorrencia: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            _anexosContratoVeiculo.Anexos.val(arg.Data.Anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    });
}

function CarregarContratoVeiculo(ocorrencia) {
    _contratoVeiculo.Ocorrencia.val(ocorrencia);
    _gridContratoVeiculo.CarregarGrid(function () {
        if (_gridContratoVeiculo.NumeroRegistros() > 0)
            $("#liContratoVeiculo").show();
        else
            $("#liContratoVeiculo").hide();
    });
}