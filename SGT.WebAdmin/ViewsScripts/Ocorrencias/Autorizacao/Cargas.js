
//*******MAPEAMENTO*******

var _anexosCarga;
var _carga;
var _gridCargas;
var _gridAnexosCarga;

var AnexosCarga = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosCargas();
    });
}

var Carga = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cargas = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadAnexosCargas() {
    _anexosCarga = new AnexosCarga();
    KoBindings(_anexosCarga, "knockoutAnexosCarga");

    _carga = new Carga();
    KoBindings(_carga, "knockoutCargas");

    GridCargas();
    GridAnexos();
}

function downloadAnexoCargaClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaAnexos/DownloadAnexoCarga", data);
}

function verAnexosCargaClick(itemGrid) {
    _anexosCarga.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    RenderizarGridAnexosCargas();
    Global.abrirModal('divModalGerenciarAnexosCarga');
}

//*******MÉTODOS*******
function GridCargas() {
    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Anexos, id: guid(), metodo: verAnexosCargaClick, icone: "" };
    var detalhar = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Detalhar, id: guid(), metodo: detalhesCargaSumarizada, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Opcoes, tamanho: 7, opcoes: [detalhar, download] };

    // Grid
    var configExport = {
        url: "CargaOcorrencia/ExportarDetalhesCargas",
        titulo: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DetalhesDocumentos
    };
    _gridCargas = new GridViewExportacao(_carga.Cargas.idGrid, "CargaOcorrencia/CargaOcorrenciaSumarizado", _carga, menuOpcoes, configExport);
}

function GridAnexos() {
    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Download, id: guid(), metodo: downloadAnexoCargaClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Opcoes, tamanho: 9, opcoes: [download] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosCarga = new BasicDataTable(_anexosCarga.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosCarga.CarregarGrid([]);
}

function GetAnexosCargas(todosAnexos) {
    var codigoVeiculo = _anexosCarga.CodigoVeiculo.val();

    // Retorna um clone do array para não prender a referencia
    var lista = (_anexosCarga.Anexos.val() || []).slice();

    return lista.filter(function (anexo) {
        if (todosAnexos || anexo.CodigoVeiculo == codigoVeiculo)
            return anexo;
    });
}

function RenderizarGridAnexosCargas() {
    // Busca a lista
    var anexos = GetAnexosCargas();

    // E chama o metodo da grid
    _gridAnexosCarga.CarregarGrid(anexos);
}


function CarregarAnexosCargas() {
    executarReST("OcorrenciaAnexos/BuscarAnexosCargaPorCodigo", { Ocorrencia: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            _anexosCarga.Anexos.val(arg.Data.Anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    });
}

function CarregarCargas(ocorrencia, situacao, ocorrenciaPorPeriodo) {
    _carga.Ocorrencia.val(ocorrencia);
    _gridCargas.CarregarGrid(function () {
        if (_gridCargas.NumeroRegistros() > 0)
            $("#liCargas").show();
        else
            $("#liCargas").hide();
    });
}