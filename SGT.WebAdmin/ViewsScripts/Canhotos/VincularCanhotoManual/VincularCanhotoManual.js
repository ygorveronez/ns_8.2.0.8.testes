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

var _pesquisaCanhoto;
var _gridCanhoto;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCanhotoAberto = -1;

var _situacaoImagemCanhoto = [
    { text: "Todas", value: "" },
    { text: "Imagem Não Reconhecida", value: EnumSituacaoleituraImagemCanhoto.ImagemNaoReconhecida },
    { text: "Ag. Processamento", value: EnumSituacaoleituraImagemCanhoto.AgProcessamento },
    { text: "Canhoto Vinculado", value: EnumSituacaoleituraImagemCanhoto.CanhotoVinculado },
    { text: "Descartada", value: EnumSituacaoleituraImagemCanhoto.Descartada },
    { text: "Sem Canhoto", value: EnumSituacaoleituraImagemCanhoto.SemCanhoto },
    { text: "Falha ao Processar", value: EnumSituacaoleituraImagemCanhoto.FalhaProcessamento },
    { text: "Ag. Imagem", value: EnumSituacaoleituraImagemCanhoto.AgImagem }
];

var PesquisaCanhoto = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual,  getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (isMultiCTe() ? "*" : "") + "Emissor NF-e:", issue: 52, idBtnSearch: guid() });
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), text: "Número Documento: " });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoleituraImagemCanhoto.SemCanhoto), options: _situacaoImagemCanhoto, text: "Situação: ",issue: 2362 });

    this.DataEntrega = PropertyEntity({ getType: typesKnockout.date, text: "*Data Entrega: ", val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _buscandoApenasNaoReconhecidos = (_pesquisaCanhoto.Situacao.val() == EnumSituacaoleituraImagemCanhoto.ImagemNaoReconhecida || _pesquisaCanhoto.Situacao.val() == EnumSituacaoleituraImagemCanhoto.SemCanhoto || _pesquisaCanhoto.Situacao.val() == EnumSituacaoleituraImagemCanhoto.FalhaProcessamento);
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
function loadVincularCanhotoManual() {
    _pesquisaCanhoto = new PesquisaCanhoto();
    KoBindings(_pesquisaCanhoto, "knockoutPesquisaCanhoto", false, _pesquisaCanhoto.Pesquisar.id);

    new BuscarClientes(_pesquisaCanhoto.Cliente);

    loadCanhoto();
    BuscarCanhotoParaVinculo();

    _pesquisaCanhoto.Upload.file = document.getElementById(_pesquisaCanhoto.Upload.idFile);

    if (_CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos && (isMultiCTe() || isMultiEmbarcador()))
        _pesquisaCanhoto.DataEntrega.visible(true);
}

function UploadChange() {
    if (_pesquisaCanhoto.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _pesquisaCanhoto.Upload.file.files.length; i < s; i++) {
            data.append("Imagem", _pesquisaCanhoto.Upload.file.files[i]);
        }

        _pesquisaCanhoto.DataEntrega.requiredClass("form-control ");
        _pesquisaCanhoto.Cliente.requiredClass("form-control ");

        if (isMultiCTe() && (_pesquisaCanhoto.Cliente.codEntity() == 0)) {
            exibirMensagem(tipoMensagem.aviso, "Campo Obrigatório", "O emissor das notas fiscais deve ser informado.");
            _pesquisaCanhoto.Cliente.requiredClass("form-control  is-invalid");
            $("#" + _pesquisaCanhoto.Upload.idFile).val("");
        } else if (_CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos && (isMultiCTe() || isMultiEmbarcador()) && ((_pesquisaCanhoto.DataEntrega.val() === null) || (_pesquisaCanhoto.DataEntrega.val() === undefined) || (_pesquisaCanhoto.DataEntrega.val().replace(/\//g, "") === ""))) {
            exibirMensagem(tipoMensagem.aviso, "Campo Obrigatório", "Necessário informar a data de entrega.");
            _pesquisaCanhoto.DataEntrega.requiredClass("form-control  is-invalid");
            $("#" + _pesquisaCanhoto.Upload.idFile).val("");
        } else {
            var dados = {
                Cliente: _pesquisaCanhoto.Cliente.codEntity(),
                DataEntrega: _pesquisaCanhoto.DataEntrega.val()
            };
            
            enviarArquivo("VincularCanhotoManual/UploadImagens", dados, data, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        if (arg.Data.Adicionados > 0) {
                            if (arg.Data.Adicionados > 1)
                                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Adicionados + " imagens adicionadas.");
                            else
                                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Adicionados + " imagem adicionada.");
                        }

                        if (arg.Data.Erros.length > 0)
                            exibirMensagem(tipoMensagem.aviso, "Aviso", "Ocorreu " + arg.Data.Erros + " erro(s).");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

                $("#" + _pesquisaCanhoto.Upload.idFile).val("");
            });
        }
    }
}

//*******MÉTODOS*******
function BuscarCanhotoParaVinculo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: abrirCanhotoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCanhoto = new GridView(_pesquisaCanhoto.Pesquisar.idGrid, "VincularCanhotoManual/Pesquisa", _pesquisaCanhoto, menuOpcoes, null, 10);
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
    _indexCanhotoAberto = -1;
    if (_atualizarAoFechar) {
        RecarregarGrid();
        _atualizarAoFechar = false;
    }
}

function RecarregarGrid(cb) {
    _gridCanhoto.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}