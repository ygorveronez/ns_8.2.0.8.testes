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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="DetalhesGeolocalicao.js" />
/// <reference path="Geolocalizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridLocalidade;
var _localidade;
var _pesquisaLocalidade;
var _adicionarLocalidade;


var PesquisaLocalidade = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription()});

    this.AdicionarLocalidade = PropertyEntity({ eventClick: AdicionarLocalidadeClick, type: types.event, text: Localization.Resources.Localidades.Localidade.AdicionarNovaLocalidade, visible: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLocalidade.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var Localidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.Descricao.getFieldDescription(), required: true, maxlength: 150, enable: ko.observable(false) });
    this.CodigoCidade = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoCidade.getFieldDescription(), required: false, maxlength: 150, enable: ko.observable(false) });
    this.CodigoDocumento = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoDoc, required: false, maxlength: 150, enable: ko.observable(true) });
    this.Estado = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.Estado.getFieldDescription(), required: true, maxlength: 150, enable: ko.observable(false) });
    this.CodigoIBGE = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoDoIBGE.getFieldDescription(), required: true, enable: ko.observable(false), maxlength: 8 });
    this.OutrasDescricoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: true, text: Localization.Resources.Localidades.Localidade.OutraDescricao.getRequiredFieldDescription(), codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.CodigoURF = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoURF, required: false, maxlength: 150, enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigoRA = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoRA, required: false, maxlength: 150, enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigoZonaTarifaria = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoZonaTarifaria.getFieldDescription(), required: false, maxlength: 80, enable: ko.observable(true) });
    this.LatitudeEntrega = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 100 });
    this.LongitudeEntrega = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 100 });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo Integração:", required: false, enable: ko.observable(true), maxlength: 50 });
    
    this.AdicionarOutraDescricao = PropertyEntity({ eventClick: adicionarOutraDescricaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: Localization.Resources.Localidades.Localidade.TipoDocumentoEmitidoFretesMunicipais.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Map = PropertyEntity();
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
    this.LimparGeolocalizacao = PropertyEntity({ type: types.event, eventClick: limparGeolocalizacaoClick, text: Localization.Resources.Localidades.Localidade.LimparGeolocalizacao, visible: ko.observable(true) });

    this.Latitude = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.Latitude, required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });
    this.Longitude = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.Longitude, required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });
    this.BuscarLatitudeLongitude = PropertyEntity({ eventClick: BuscarLatitudeLongitude, type: types.event, text: Localization.Resources.Localidades.Localidade.BuscarLatitudeLongitude, visible: ko.observable(true), enable: ko.observable(true) });
    this.MapDetalhes = PropertyEntity();
    this.BuscarCoordenadas = PropertyEntity({ eventClick: obterGeolocalizacaoAtual, type: types.event, text: Localization.Resources.Localidades.Localidade.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });

    this.RKST = PropertyEntity({ text: "RKST: ", required: ko.observable(false), maxlength: 10 });

}

var AdicionarLocalidade = function () {
    this.Descricao = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: Localization.Resources.Localidades.Localidade.Descricao.getFieldDescription() });
    this.CodigoCidade = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoCidade.getFieldDescription(), required: false, maxlength: 150, enable: ko.observable(false) });
    this.CEP = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 8, text: Localization.Resources.Localidades.Localidade.CEP.getFieldDescription() });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Localidades.Localidade.Estado.getFieldDescription(), idBtnSearch: guid(), required: true });
    this.CodigoIBGE = PropertyEntity({ getType: typesKnockout.int, required: true, text: Localization.Resources.Localidades.Localidade.CodigoDoIBGE.getFieldDescription(), maxlength: 8 });
    this.CodigoURF = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoURF, required: false, maxlength: 150, enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigoRA = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoRA, required: false, maxlength: 150, enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigoZonaTarifaria = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoZonaTarifaria.getFieldDescription(), required: false, maxlength: 80, enable: ko.observable(true) });
    this.CodigoDocumento = PropertyEntity({ text: Localization.Resources.Localidades.Localidade.CodigoDoc.getFieldDescription(), required: false, maxlength: 150, enable: ko.observable(true) });
    this.LocalidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Localidades.Localidade.LocalidadePolo.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Localidades.Localidade.Pais.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: Localization.Resources.Localidades.Localidade.TipoDocumentoEmitidoFretesMunicipais.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo Integração:", required: false, getType: typesKnockout.string, enable: ko.observable(true), maxlength: 50 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarNovaLocalidadeClick, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadLocalidade() {

    _localidade = new Localidade();
    KoBindings(_localidade, "knockoutCadastroLocalidade");

    _adicionarLocalidade = new AdicionarLocalidade();
    KoBindings(_adicionarLocalidade, "knoutAdicionarLocalidade");

    new BuscarLocalidades(_adicionarLocalidade.LocalidadePolo);
    new BuscarEstados(_adicionarLocalidade.Estado);
    new BuscarPaises(_adicionarLocalidade.Pais);

    HeaderAuditoria("Localidade", _localidade);

    _pesquisaLocalidade = new PesquisaLocalidade();
    KoBindings(_pesquisaLocalidade, "knockoutPesquisaLocalidade", false, _pesquisaLocalidade.Pesquisar.id);

    buscarLocalidades();
    loadOutraDescricao();
    loadDetalhesGeolocalizacao();
    if (controlarVisibilidadeAbaGeolocalizacao()) {
        loadGeolocalizacao();
    }
}

function controlarVisibilidadeAbaGeolocalizacao() {
    if (_CONFIGURACAO_TMS.PermiteCadastrarLatLngEntregaLocalidade)
        $("#liGeoLocalizacao").show();
    return _CONFIGURACAO_TMS.PermiteCadastrarLatLngEntregaLocalidade;
}

function AdicionarLocalidadeClick(e, data) {
    
    LimparCampos(_adicionarLocalidade);
        
    Global.abrirModal('divAdicionarLocalidade');
}

function AdicionarNovaLocalidadeClick(e, sender) {
    Salvar(_adicionarLocalidade, "Localidade/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridLocalidade.CarregarGrid();
                Global.fecharModal('divAdicionarLocalidade');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(e, "Localidade/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridLocalidade.CarregarGrid();
                limparCamposLocalidade();
            } else {
                resetarTabs();
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            resetarTabs();
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposLocalidade();
}

//*******MÉTODOS*******


function buscarLocalidades() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLocalidade, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    var configuracoesExportacao = { url: "Localidade/ExportarPesquisa", titulo: "Localidade" };
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLocalidade = new GridViewExportacao(_pesquisaLocalidade.Pesquisar.idGrid, "Localidade/Pesquisa", _pesquisaLocalidade, menuOpcoes, configuracoesExportacao);
    _gridLocalidade.CarregarGrid();
}

function editarLocalidade(localidadeGrid) {
    limparCamposLocalidade();
    resetarTabs();
    _localidade.Codigo.val(localidadeGrid.Codigo);
    BuscarPorCodigo(_localidade, "Localidade/BuscarPorCodigo", function (arg) {
        _pesquisaLocalidade.ExibirFiltros.visibleFade(false);
        _localidade.Atualizar.visible(true);
        _localidade.Cancelar.visible(true);
        recarregarGridOutrasDescricoes();
        carregarGeolocalizacao();
        BuscarLatitudeLongitude();
        $("#divDadosLocalidades").show();
    }, null);
}

function limparCamposLocalidade() {
    resetarTabs();
    _localidade.Atualizar.visible(false);
    _localidade.Cancelar.visible(false);

    _localidade.OutrasDescricoes.list = new Array();
    recarregarGridOutrasDescricoes();
    LimparCampos(_localidade);
    $("#divDadosLocalidades").hide();
    _pesquisaLocalidade.ExibirFiltros.visibleFade(true);

    limparCamposGeolocalizacao();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function BuscarLatitudeLongitude() {
    var lat = parseFloat(String(_localidade.Latitude.val()).replace(',', '.'));
    var long = parseFloat(String(_localidade.Longitude.val()).replace(',', '.'));
    if (!isNaN(lat) != 0 && !isNaN(long) != 0) {
        setLatLngMap(lat, long);
        //_localidade.Latitude.val(lat);
        //_localidade.Longitude.val(long);
        //var position = new google.maps.LatLng(lat, long);
        //if (_markerDetalhes != null) {
        //    _markerDetalhes.setPosition(position);
        //    _mapDetalhes.panTo(position);
        //}
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.LatitudeLongitudeInformadaEstaoInvalidas);
    }
}