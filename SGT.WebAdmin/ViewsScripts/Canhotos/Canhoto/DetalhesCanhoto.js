/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPgtoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDigitalizacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumTipoCanhoto.js" />

var _knoutDetalhesCanhoto;
var _gridHistoricoCanhotos;

var _map;
var _marker;

var latLngDefault = { lat: -10.861639, lng: -53.104038 };

var DetalhesCanhoto = function () {

    this.NotasAvuso = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.NotasCanhotoAvulso.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Chave = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ChaveDeAcesso.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Numero = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Numero.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DescricaoTipoCanhoto = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataEmissao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Destinatario.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });

    this.SituacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoCanhoto.Todas), def: EnumSituacaoCanhoto.Todas, visible: ko.observable(true) });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), def: EnumSituacaoDigitalizacaoCanhoto.Todas, visible: ko.observable(true) });
    this.SituacaoPgtoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoPgtoCanhoto.Todas), def: EnumSituacaoPgtoCanhoto.Todas, visible: ko.observable(true) });

    this.DescricaoSituacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDoCanhoto.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DescricaoDigitalizacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDigitalizacao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Empresa.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Valor.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Peso.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.NaturezaOP = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.NaturezaDaOperacao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DescricaoModalidadeFrete = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.TipoDePagamento.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Carga = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Carga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Motoristas = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Motoristas.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Filial.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Emitente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.LocalDeArmazenamentoDoCanhoto.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Justificativa.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.MotivoRejeicaoDigitalizacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.MotivoDaRejeicaoDaImagem.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.Todos), def: EnumTipoCanhoto.Todos, visible: ko.observable(true) });

    this.Latitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });

    this.Map = PropertyEntity();
    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });

    this.Auditar = PropertyEntity({ eventClick: auditarCanhotoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Auditar, visible: ko.observable(true) });

    this.ObservacaoRecebimentoFisico = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ObservacaoDoRecebimentoFisico.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataRecebimento = PropertyEntity({ text: "Data de Recebimento", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroProtocolo = PropertyEntity({ text: "Nº Protocolo", val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Observacao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
};

function loadDetalhesCanhoto() {

    _knoutDetalhesCanhoto = new DetalhesCanhoto();
    KoBindings(_knoutDetalhesCanhoto, "KnoutDetalhesCanhoto");
    HeaderAuditoria("Canhoto", _knoutDetalhesCanhoto, "Numero");

    _gridHistoricoCanhotos = new GridView(_knoutDetalhesCanhoto.Chave.idGrid, "Canhoto/ConsultarHistoricoCanhoto", _knoutArquivo, null);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor)
        $("#li_historico" + _knoutArquivo.Codigo.id).hide();

    CarregarMapa();
}

function auditarCanhotoClick(e, sender) {
    var data = { Codigo: _knoutArquivo.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("Canhoto", null, e);

    closureAuditoria(data);
}

function CarregarMapa() {
    if (_map == null) {
        setTimeout(function () {
            _map = new google.maps.Map(document.getElementById(_knoutDetalhesCanhoto.Map.id));
            _marker = new google.maps.Marker({
                map: _map,
                draggable: true
            });
            _marker.addListener("dragend", dragendEvent)
            //setarCoordenadasCanhoto();
        }, 200);
    };
}

function dragendEvent(event) {
    var latLng = _marker.getPosition();
    _knoutDetalhesCanhoto.Latitude.val(latLng.lat().toString());
    _knoutDetalhesCanhoto.Longitude.val(latLng.lng().toString());
}

function setarCoordenadasCanhoto() {
    if (_knoutDetalhesCanhoto.Latitude.val() == "" || _knoutDetalhesCanhoto.Longitude.val() == "" || _knoutDetalhesCanhoto.Latitude.val() == null || _knoutDetalhesCanhoto.Longitude.val() == null) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.CanhotoSemPosicionamentoCadastrado);
        return;
    } else {
        var latLng = { lat: -10.861639, lng: -53.104038 };
        var zoom = 4;
        if (_knoutDetalhesCanhoto.Latitude.val() != "" && _knoutDetalhesCanhoto.Longitude.val() != "") {
            latLng = { lat: parseFloat(_knoutDetalhesCanhoto.Latitude.val()), lng: parseFloat(_knoutDetalhesCanhoto.Longitude.val()) };
            zoom = 16;
        }
        _map.setZoom(zoom);
        _map.setCenter(latLng);
        _marker.setPosition(latLng);
    }
}

function limparCamposMapaRequest() {
    setarCoordenadasCanhoto();
}

function BuscarCoordenadasClick(e, sender) {
    if (_knoutDetalhesCanhoto.Latitude.val() != "" && _knoutDetalhesCanhoto.Longitude.val() != "" && _knoutDetalhesCanhoto.Latitude.val() != null && _knoutDetalhesCanhoto.Longitude.val() != null) {
        BuscarCoordenadas();
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.CanhotoSemGeoposicionamentoCadastrado);
}

function BuscarCoordenadas(callback) {
    setarCoordenadasCanhoto();
}

function BuscarDetalhesCanhoto(codigo, callback) {
    var dados = {
        Codigo: codigo
    }
    executarReST("Canhoto/BuscarDetalhesCanhoto", dados, function (arg) {
        if (arg.Success) {
            var retorno = { Data: arg.Data };
            PreencherObjetoKnout(_knoutDetalhesCanhoto, retorno);
            _knoutArquivo.Codigo.val(codigo);

            if (arg.Data.LocalArmazenamento == "")
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
            else {
                var strLocal = arg.Data.LocalArmazenamento;
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(true);
                if (arg.Data.PacoteArmazenado > 0)
                    strLocal += " " + Localization.Resources.Canhotos.Canhoto.NoPacoteNaPosicao.format(arg.Data.PacoteArmazenado, arg.Data.PosicaoNoPacote);
                _knoutDetalhesCanhoto.LocalArmazenamento.val(strLocal);
            }

            if (_knoutDetalhesCanhoto.SituacaoCanhoto.val() == EnumSituacaoCanhoto.Justificado)
                _knoutDetalhesCanhoto.Justificativa.visible(true);

            if (_knoutDetalhesCanhoto.SituacaoDigitalizacaoCanhoto.val() == EnumSituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada)
                _knoutDetalhesCanhoto.MotivoRejeicaoDigitalizacao.visible(true);


            if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.NFe) {
                _knoutDetalhesCanhoto.Chave.visible(true);
                _knoutDetalhesCanhoto.NotasAvuso.visible(false);
                _knoutDetalhesCanhoto.NaturezaOP.visible(true);
            } else if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.Avulso) {
                _knoutDetalhesCanhoto.Chave.visible(false);
                _knoutDetalhesCanhoto.NotasAvuso.visible(true);
            }


            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
                _knoutDetalhesCanhoto.Empresa.visible(false);
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                _knoutDetalhesCanhoto.Empresa.visible(false);
                _knoutDetalhesCanhoto.Filial.visible(false);
            }

            if (_knoutDetalhesCanhoto.Latitude.val() != "" && _knoutDetalhesCanhoto.Longitude.val() != "" && _knoutDetalhesCanhoto.Latitude.val() != null && _knoutDetalhesCanhoto.Longitude.val() != null) {
                setarCoordenadasCanhoto();
                $("#li_geoLocalizacao").show();
            }
            else
                $("#li_geoLocalizacao").hide();

            _knoutDetalhesCanhoto.ObservacaoRecebimentoFisico.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.ObservacaoRecebimentoFisico.val()));
            _knoutDetalhesCanhoto.DataRecebimento.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.DataRecebimento.val()));
            _knoutDetalhesCanhoto.NumeroProtocolo.visible(_knoutDetalhesCanhoto.NumeroProtocolo.val() > 0);
            _knoutDetalhesCanhoto.Observacao.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.Observacao.val()));

            _gridHistoricoCanhotos.CarregarGrid(function () {
                if (callback != null)
                    callback();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })
}