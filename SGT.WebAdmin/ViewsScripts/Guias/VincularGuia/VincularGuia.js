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


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaGuia;
var _gridGuia;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexGuiaAberto = -1;

var PesquisaGuia = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), text: "Número Documento: " });
    this.Situacao = PropertyEntity({ text: "Situação Guia:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoGuia.obterOpcoes(), visible: ko.observable(true) });

    this.DataEntrega = PropertyEntity({ getType: typesKnockout.date, text: "*Data Entrega: ", val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadVincularGuia() {
    _pesquisaGuia = new PesquisaGuia();
    KoBindings(_pesquisaGuia, "knockoutPesquisaGuia", false, _pesquisaGuia.Pesquisar.id);

    new BuscarCargas(_pesquisaGuia.Carga);

    loadGuia();
    BuscarGuiaParaVinculo();


    _pesquisaGuia.Upload.file = document.getElementById(_pesquisaGuia.Upload.idFile);

}

function UploadChange() {
    if (_pesquisaGuia.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _pesquisaGuia.Upload.file.files.length; i < s; i++) {
            data.append("Imagem", _pesquisaGuia.Upload.file.files[i]);
        }

        _pesquisaGuia.DataEntrega.requiredClass("form-control ");



        var dados = {
            Carga: _pesquisaGuia.Carga.codEntity(),
            DataEntrega: _pesquisaGuia.DataEntrega.val()
        };

        enviarArquivo("VincularGuia/UploadImagens", dados, data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.Adicionados > 0) {
                        if (arg.Data.Adicionados > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Adicionados + " imagens adicionadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Adicionados + " imagem adicionada.");
                    }
                    RecarregarGrid();

                    if (arg.Data.Erros.length > 0)
                        exibirMensagem(tipoMensagem.aviso, "Aviso", "Ocorreu " + arg.Data.Erros + " erro(s).");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            $("#" + _pesquisaGuia.Upload.idFile).val("");
        });

    }
}

//*******MÉTODOS*******
function BuscarGuiaParaVinculo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: abrirGuiaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridGuia = new GridView(_pesquisaGuia.Pesquisar.idGrid, "VincularGuia/Pesquisa", _pesquisaGuia, menuOpcoes, null, 10);
    RecarregarGrid();
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function isMultiCTe() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
}

function isMultiEmbarcador() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador;
}

function ModalOcultou() {
    _indexGuiaAberto = -1;
    if (_atualizarAoFechar) {
        RecarregarGrid();
        _atualizarAoFechar = false;
    }
}

function RecarregarGrid() {
    _gridGuia.CarregarGrid();
}

