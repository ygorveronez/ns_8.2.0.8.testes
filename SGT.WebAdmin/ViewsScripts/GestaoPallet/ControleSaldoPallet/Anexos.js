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
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAnexos;
var _anexos;

var Anexos = function () {
    this.Solicitacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, val: ko.observable(""), text: "Anexo:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.Arquivo.val().replace('C:\\fakepath\\', '') }) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        const nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexos.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.config = PropertyEntity({
        url: {
            download: "ControlePalletAnexo/DownloadAnexo",
        }
    });
}

//*******EVENTOS*******

function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");

    //-- Grid Anexos
    // Opcoes
    const download = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: visibleDownload };

    // Menu
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 9, opcoes: [download] };

    // Cabecalho
    const header = [
        { data: "Codigo", visible: false },
        { data: "NomeArquivo", title: "Nome", width: "75%", className: "text-align-left" }
    ];

    // Grid
    const linhasPorPaginas = 7;
    _gridAnexos = new BasicDataTable(_anexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

function visibleDownload(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function downloadAnexoClick(dataRow) {
    const data = { Codigo: dataRow.Codigo };
    executarDownload(_anexos.config.url.download, data);
}

function gerenciarAnexosClick(e) {
    executarReST("ControlePalletAnexo/ObterAnexo", { Codigo: e.Codigo }, function (retorno) {
        if (retorno.Success) {
            _anexos.Anexos.val(retorno.Data.Anexos);
            Global.abrirModal('divModalGerenciarAnexos');
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

//*******MÉTODOS*******
function GetAnexos() {
    return _anexos.Anexos.val().slice();
}

function RenderizarGridAnexos() {
    const anexos = GetAnexos();

    _gridAnexos.CarregarGrid(anexos);
}

function limparAnexosTela() {
    LimparCampos(_anexos);
    _anexos.Anexos.val(_anexos.Anexos.def);
}