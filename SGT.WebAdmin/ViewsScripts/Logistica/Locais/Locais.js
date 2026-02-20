/// <reference path="../../Consultas/Filial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDLocais;
var _Locais;
var _pesquisaLocais;
var _gridLocais;
var _mapaLocais;

var _TipoLocal = [
    { text: "Área de risco", value: 1 },
    { text: "Pernoite", value: 2 },
    { text: "Micro Região Roteirização", value: 3 },
    { text: "Ponto de apoio", value: 4 },
    { text: "Balança", value: 5 },
    { text: "Zona de exclusão de Rotas", value: 6 },
    { text: "Raios de proximidade", value: 7 }
];

/*
 * Declaração das Classes
 */

var CRUDLocais = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.AdicionarNovoRaio = PropertyEntity({ eventClick: adicionarNovoRaio, type: types.event, text: "Novo Raio", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Locais/Importar",
        UrlConfiguracao: "Locais/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O061_Pedidos,
    });
}

var Locais = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoLocal = PropertyEntity({ text: "*Tipo local: ", val: ko.observable(0), options: _TipoLocal, def: 1 });// eventChange: changeTipoLocal, 
    this.TipoArea = PropertyEntity({ text: "*Tipo área: ", val: ko.observable(EnumTipoArea.Raio), options: EnumTipoArea.obterOpcoes(), def: EnumTipoArea.Raio, enable: ko.observable(true) });
    this.Area = PropertyEntity();
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.RaiosProximidade = PropertyEntity();
    this.TipoLocal.val.subscribe(changeTipoLocal);
}

var PesquisaLocais = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridLocais, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

//Se tipo local = Micro região roteirização... filial visible e required.
function changeTipoLocal() {

    if (_Locais == null || _mapaLocais == null) return;

    if (_Locais.TipoLocal.val() == 3 || _Locais.TipoLocal.val() == 4 || _Locais.TipoLocal.val() == 5) { // Micro região ou Ponto de apoio
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _Locais.Filial.visible(true);
            _Locais.Filial.required(true);
        }
        else {
            _Locais.Filial.visible(false);
            _Locais.Filial.required(false);
        }
        if (_Locais.TipoLocal.val() == 3 || _Locais.TipoLocal.val() == 6) {
            _mapaLocais.draw.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
            _Locais.TipoArea.val(2); //Poligono
        } else {
            _mapaLocais.draw.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            _Locais.TipoArea.val(3); //Ponto
        }
        if (_Locais.TipoLocal.val() != 5) {
            _Locais.TipoArea.enable(false);
        } else {
            _Locais.TipoArea.enable(true);
        }
    } else {
        _mapaLocais.draw.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
        _Locais.Filial.visible(false);
        _Locais.Filial.required(false);
        _Locais.TipoArea.enable(true);
    }

    if (_Locais.TipoLocal.val() == 7) {
        _CRUDLocais.AdicionarNovoRaio.visible(true);
    } else {
        _CRUDLocais.AdicionarNovoRaio.visible(false);
        $('#divCadastroRaiosProximidade').innerHTML = "";
    }
}

function loadGridLocais() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "Locais/ExportarPesquisa", titulo: "Locais" };

    _gridLocais = new GridViewExportacao(_pesquisaLocais.Pesquisar.idGrid, "Locais/Pesquisa", _pesquisaLocais, menuOpcoes, configuracoesExportacao);
    _gridLocais.CarregarGrid();
}

function loadLocais() {
    _Locais = new Locais();
    KoBindings(_Locais, "knockoutLocais");

    new BuscarFilial(_Locais.Filial);

    HeaderAuditoria("Locais", _Locais);

    _CRUDLocais = new CRUDLocais();
    KoBindings(_CRUDLocais, "knockoutCRUDLocais");

    _pesquisaLocais = new PesquisaLocais();
    KoBindings(_pesquisaLocais, "knockoutPesquisaLocais", false, _pesquisaLocais.Pesquisar.id);

    loadGridLocais();
    loadMapa();
}

function loadMapa() {
    _mapaLocais = new MapaGoogle("map", true);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    _Locais.Area.val(_mapaLocais.draw.getJson());
    _Locais.RaiosProximidade.val(buscarRaiosProximidade());

    if (!ValidarCamposObrigatoriosRaioProximidade()) {
        return;
    }

    if (!validarMarkerGeolocalizacaoParaRaioProximidade())
        return;

    Salvar(_Locais, "Locais/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridLocais();
                limparCamposLocais();
                LimparRaiosProximidade();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    _Locais.Area.val(_mapaLocais.draw.getJson());

    LimparCampo(_Locais.RaiosProximidade);
    _Locais.RaiosProximidade.val(buscarRaiosProximidade());

    if (!validarMarkerGeolocalizacaoParaRaioProximidade())
        return;


    if (!ValidarCamposObrigatoriosRaioProximidade()) {
        return;
    }
    Salvar(_Locais, "Locais/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridLocais();
                limparCamposLocais();
                LimparRaiosProximidade();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposLocais();
    LimparRaiosProximidade();
}

function editarClick(registroSelecionado) {
    limparCamposLocais();

    _Locais.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_Locais, "Locais/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaLocais.ExibirFiltros.visibleFade(false);
                _Locais.TipoArea.val(retorno.Data.TipoArea);
                _Locais.TipoLocal.val(retorno.Data.Tipo);

                LimparRaiosProximidade();
                var listaRetornoRaiosProximidade = retorno.Data.RaiosProximidade;
                if (_Locais.TipoLocal.val() == 7) {
                    for (var i = 0; i < retorno.Data.RaiosProximidade.length; i++) {
                        adicionarNovoRaio(listaRetornoRaiosProximidade[i]);
                    }
                }

                changeTipoLocal();
                //Desenhando as demais micro região da mesma filial..
                if (retorno.Data.DemaisMicroRegioes != null) {
                    var areas = retorno.Data.DemaisMicroRegioes;
                    for (var i = 0; i < areas.length; i++) {
                        _mapaLocais.draw.setJson(areas[i], true);
                    }
                }
                //Desenhando as areas do local
                _mapaLocais.draw.setJson(_Locais.Area.val());
                var isEdicao = true;
                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}


function abaLocaisGeoLocalizacaoClick() {
    if (_mapaLocais) {
        setTimeout(function () {
            _mapaLocais.draw.centerShapes();
        }, 300);
    }
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_Locais, "Locais/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridLocais();
                    limparCamposLocais();
                    LimparRaiosProximidade();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDLocais.Atualizar.visible(isEdicao);
    _CRUDLocais.Excluir.visible(isEdicao);
    _CRUDLocais.Cancelar.visible(isEdicao);
    _CRUDLocais.Adicionar.visible(!isEdicao);
}

function limparCamposLocais() {
    var isEdicao = false;
    _mapaLocais.clear();
    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_Locais);
}

function recarregarGridLocais() {
    _gridLocais.CarregarGrid();
}

function validarMarkerGeolocalizacaoParaRaioProximidade() {
    if (_Locais.TipoLocal.val() === 7) {

        if (_Locais.Area.val() === "") {
            exibirMensagem(tipoMensagem.atencao, "Necessário escolher uma base no mapa", "Favor escolher uma base no mapa da aba 'Geolocalização'");
            return false;
        }

        var jsonMarkerMapaGeolocalizacao = JSON.parse(_Locais.Area.val());
        var markerMapaGeolocalizacao = jsonMarkerMapaGeolocalizacao.find(item => item.type === "marker");

        if (markerMapaGeolocalizacao == undefined) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Favor escolher um local base (pin) no mapa da aba 'Geolocalização'");
            return false;
        }

    }

    return true;
}