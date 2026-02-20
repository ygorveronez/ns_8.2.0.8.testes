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
/// <reference path="Cliente.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _downloadDocumentos, _gridCarga = null;

var DownloadDocumentos = function () {
    this.Carga = PropertyEntity({ type: types.map, required: false, text: "Informar Cargas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.ListaCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });
    this.Atualizar = PropertyEntity({ eventClick: downloadClick, type: types.event, text: "Download", visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadDownloadDocumentos() {
    HeaderAuditoria("DownloadDocumentos", _downloadDocumentos);
    _downloadDocumentos = new DownloadDocumentos();
    KoBindings(_downloadDocumentos, "knockoutDownloadDocumentos");
    LoadGridCarga();
}

function downloadClick(e, sender) {

    if (_downloadDocumentos.Carga.basicTable.BuscarRegistros().length == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Pelo menos uma carga deve ser adicionada");
        return;
    }

    preencherListaCarga();
    executarDownload("CargasDownloadDocumentos/DownloadDocumentos", { ListaCarga: _downloadDocumentos.ListaCarga.val() });
}

function LoadGridCarga() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverCargaClick(_downloadDocumentos.Carga, data);
        }, tamanho: "15", icone: ""
    };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Número da Carga", width: "20%", className: "text-align-left" },
        { data: "Filial", title: "Filial", width: "20%", className: "text-align-left" },
        { data: "Transportador", title: "Transportador", width: "20%", className: "text-align-left" },
        { data: "QuantidadeDocumentosFrete", title: "Quantidade Documentos Frete", width: "20%", className: "text-align-right" },
        { data: "ValorFrete", title: "Valor Frete", width: "20%", className: "text-align-right" },
        { data: "DataCriacaoCargaDescricao", title: "Data Criação Carga", width: "20%", className: "text-align-center" },
        { data: "DataEmissaoDocumentosDescricao", title: "Data Emissão Documentos", width: "20%", className: "text-align-center" },
        { data: "TipoOperacao", title: "Tipo de Operação", width: "20%", className: "text-align-left" },
        { data: "Veiculo", title: "Veículo", width: "20%", className: "text-align-left" },
        { data: "Motorista", title: "Motorista", width: "20%", className: "text-align-left" },
        { data: "SituacaoDescricao", title: "Situação", width: "20%", className: "text-align-left" },
    ];

    _gridCarga = new BasicDataTable(_downloadDocumentos.Carga.idGrid, header, menuOpcoes, null, null, 20);
    _downloadDocumentos.Carga.basicTable = _gridCarga;

    new BuscarCargas(_downloadDocumentos.Carga, null, null, null, null, null, _gridCarga, null, null, null, null, null, null, 20);
    RecarregarListaCarga();
}

function RemoverCargaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a carga  " + sender.CodigoCargaEmbarcador + "?", function () {
        var grupoPessoaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < grupoPessoaGrid.length; i++) {
            if (sender.Codigo == grupoPessoaGrid[i].Codigo) {
                grupoPessoaGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(grupoPessoaGrid);
    });
}

function RecarregarListaCarga() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_downloadDocumentos.ListaCarga.val())) {
        $.each(_downloadDocumentos.ListaCarga.val(), function (i, grupoPessoa) {
            var obj = new Object();

            obj.Codigo = grupoPessoa.Codigo;
            obj.Descricao = grupoPessoa.Descricao;

            data.push(obj);
        });
    }
    _gridCarga.CarregarGrid(data);
}

function preencherListaCarga() {
    _downloadDocumentos.ListaCarga.list = new Array();
    var cargas = new Array();

    $.each(_downloadDocumentos.Carga.basicTable.BuscarRegistros(), function (i, carga) {
        cargas.push(carga.Codigo);
    });

    _downloadDocumentos.ListaCarga.val(JSON.stringify(cargas));
}