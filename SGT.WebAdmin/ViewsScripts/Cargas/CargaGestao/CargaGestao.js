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
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Veiculos.js" />
/// <reference path="Motoristas.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaGestaoCarga;
var _HTMLCargaGestao;
var _AnosSemanas;
var _gridCargaCodigo;

var Carga = function () {
    this.DivCarga = PropertyEntity({ type: types.local });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.NaLogistica), def: EnumSituacoesCarga.NaLogistica });
    this.OrigemDestinos = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "" });
    this.DescricaoSituacaoCarga = PropertyEntity({ type: types.map, required: false, text: "Status: " });
    this.ValorFrete = PropertyEntity({ val: ko.observable(""), def: "", text: "Frete: " });
    this.DataCarregamento = PropertyEntity({ val: ko.observable(""), def: "", text: "Col: " });
    this.Motoristas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0, text: "Mot:" });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Placa:" });
    this.VeiculosVinculados = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0 });
    this.Pedidos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });

    this.Remetentes = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0, text: "Rem:" });
    this.Destinatarios = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0, text: "Dest:" });

    this.PrevisaoEntrega = PropertyEntity({ val: ko.observable(""), def: "", type: types.local, text: "Prev: " });
    this.Fronteira = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Front: " });

}

var PesquisaGestaoCarga = function () {
    this.Codigo = PropertyEntity({ text: "Número da Carga:", visible: ko.observable(false) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Polo:", idBtnSearch: guid() });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País:", idBtnSearch: guid() });
    this.Semana = PropertyEntity({ text: "Semana:", val: ko.observable(_AnosSemanas.semanaCorrente), def: _AnosSemanas.semanaCorrente, visible: ko.observable(true) });
    this.Ano = PropertyEntity({ val: ko.observable(_AnosSemanas.anoCorrente), def: _AnosSemanas.anoCorrente, text: "Ano: " });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatario:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCargas(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadCargaGestao() {

    var $scrollingDiv = $("#divDeOpcoes");
    $(window).scroll(function () {
        $scrollingDiv
            .stop()
            .animate({ "marginTop": ($(window).scrollTop()) + "px" }, "slow");
    });

    executarReST("CargaGestao/BuscarSemanasAnos", null, function (e) {
        if (e.Success) {
            _AnosSemanas = e.Data;
            $.get("Content/Static/Carga/CargaGestao.html?dyn=" + guid(), function (data) {
                loadVeiculos();
                loadMotoristas();
                _HTMLCargaGestao = data;
                _pesquisaGestaoCarga = new PesquisaGestaoCarga();
                var anos = [
                    { text: _AnosSemanas.anoAnterior, value: _AnosSemanas.anoAnterior },
                    { text: _AnosSemanas.anoCorrente, value: _AnosSemanas.anoCorrente },
                    { text: _AnosSemanas.proximoAno, value: _AnosSemanas.proximoAno }
                ];
                _pesquisaGestaoCarga.Ano.options = anos;

                KoBindings(_pesquisaGestaoCarga, "knockoutPesquisa", false, _pesquisaGestaoCarga.Pesquisar.id);

                $("#" + _pesquisaGestaoCarga.Semana.id).attr("min", "1").attr("max", _AnosSemanas.ultimaSemanaAno);
                _pesquisaGestaoCarga.Semana.val(_AnosSemanas.semanaCorrente)

                new BuscarLocalidadesPolo(_pesquisaGestaoCarga.CidadePolo);
                new BuscarPaises(_pesquisaGestaoCarga.Pais);
                new BuscarClientes(_pesquisaGestaoCarga.Remetente);
                new BuscarClientes(_pesquisaGestaoCarga.Destinatario);
                new BuscarFilial(_pesquisaGestaoCarga.Filial)
                buscarCargas();

            });
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}

//*******MÉTODOS*******


function buscarCargas() {

    _pesquisaGestaoCarga.Semana.val($("#" + _pesquisaGestaoCarga.Semana.id).val());

    if (_pesquisaGestaoCarga.Semana.val() > _AnosSemanas.ultimaSemanaAno)
        _pesquisaGestaoCarga.Semana.val(_AnosSemanas.ultimaSemanaAno);

    var data = RetornarObjetoPesquisa(_pesquisaGestaoCarga);

    executarReST("CargaGestao/PesquisaCargas", data, function (e) {
        if (e.Success) {
            //_pesquisaGestaoCarga.ExibirFiltros.visibleFade(false);
            $("#fdsCargas").html("");
            _gridCargaCodigo = new Array();

            $.each(e.Data, function (i, cargaPais) {

                var htmlHead = "</hr><h6 style='padding:7px; margin-bottom:5px;'>";
                if (cargaPais.Abreviacao != "")
                    htmlHead += "<img class='flag flag-" + cargaPais.Abreviacao + "' src='img/blank.gif' alt='" + cargaPais.Pais + "'>&nbsp;&nbsp;";
                htmlHead += cargaPais.Pais + " Semana " + cargaPais.Semana + " <small style='color:#111'>(" + cargaPais.DataInicioSemana + " até " + cargaPais.DataFimSemana + ")</small></h6>";


                $("#fdsCargas").append(htmlHead);
                $.each(cargaPais.Cargas, function (i, carga) {
                    var knoutCarga = new Carga();
                    preecherObjetoKnoutCarga(knoutCarga, carga);
                    knoutCarga.DivCarga.id = knoutCarga.Codigo.idGrid;
                    var html = "<div id='conteudo_" + knoutCarga.DivCarga.id + "'></div>";
                    $("#fdsCargas").append(html);
                    preencherDadosCarga(knoutCarga, carga);
                    _gridCargaCodigo.push({ Codigo: carga.Codigo, idGrid: knoutCarga.DivCarga.id });
                });

            });
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}


function preencherDadosCarga(knoutCarga, carga) {
    var idKnout = knoutCarga.DivCarga.id + "_" + knoutCarga.Codigo.val();
    var conteudo = _HTMLCargaGestao.replace(/#idDivCarga/g, idKnout);
    $("#conteudo_" + knoutCarga.DivCarga.id).html(conteudo);

    var descricaoMotorista = "";
    $.each(carga.Motoristas, function (i, motorista) {
        descricaoMotorista += motorista.Descricao;
        if (i < carga.Motoristas.length - 1)
            descricaoMotorista += ", "
    });
    knoutCarga.Motoristas.val(descricaoMotorista);

    var descricaoRemetente = "";
    $.each(carga.Remetentes, function (i, remetente) {
        descricaoRemetente += remetente.Descricao;
        if (i < carga.Remetentes.length - 1)
            descricaoRemetente += ", "
    });
    knoutCarga.Remetentes.val(descricaoRemetente);

    var descricaoDestinatario = "";
    $.each(carga.Destinatarios, function (i, destinatario) {
        descricaoDestinatario += destinatario.Descricao;
        if (i < carga.Destinatarios.length - 1)
            descricaoDestinatario += ", "
    });

    var descricaoVeiculo = knoutCarga.Veiculo.val();
    $.each(carga.VeiculosVinculados, function (i, veiculoVinculado) {
        descricaoVeiculo += ", " + veiculoVinculado.Descricao;
    });
    knoutCarga.Veiculo.val(descricaoVeiculo);

    if (descricaoVeiculo != "" && descricaoMotorista != "") {
        $("#" + idKnout).css("background", "rgba(222, 239, 215, 1)");
    } else {
        $("#" + idKnout).css("background", "rgba(254, 254, 229, 1)");
    }


    knoutCarga.Destinatarios.val(descricaoDestinatario);
    knoutCarga.PrevisaoEntrega.val(carga.PrevisaoEntrega);


    KoBindings(knoutCarga, idKnout);
    $("#" + idKnout).droppable({
        drop: droppableItem,
        hoverClass: "ui-state-active backgroundDropHover"
    });
}


function droppableItem(event, ui) {
    var id = ui.draggable[0].id.split("_")[0];
    var operacao = ui.draggable[0].id.split("_")[1];

    var carga = event.target.id.split("_")[1];
    if (operacao == "veiculo")
        informarVeiculo(carga, id);
    if(operacao == "motorista")
        informarMotorista(carga, id);
}

function informarMotorista(carga, motorista) {
    var data = {
        Carga: carga, Motorista: motorista
    }

    executarReST("CargaGestao/InformarMotorista", data, function (e) {
        if (e.Success) {
            if (e.Data != false) {
                alterarCarga(e.Data[0]);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}

function informarVeiculo(carga, veiculo) {
    var data = {
        Carga: carga, Veiculo: veiculo
    }

    executarReST("CargaGestao/InformarVeiculo", data, function (e) {
        if (e.Success) {
            if (e.Data != false) {
                alterarCarga(e.Data[0]);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}


function alterarCarga(carga) {
    $.each(_gridCargaCodigo, function (i, gridCargaCodigo) {
        if (gridCargaCodigo.Codigo == carga.Codigo) {
            var knoutCarga = new Carga();
            preecherObjetoKnoutCarga(knoutCarga, carga);
            knoutCarga.DivCarga.id = gridCargaCodigo.idGrid;
            preencherDadosCarga(knoutCarga, carga);
            return false;
        }
    });
}

function preecherObjetoKnoutCarga(knoutCarga, carga) {
    var dataCarga = { Data: carga };
    PreencherObjetoKnout(knoutCarga, dataCarga);
}
