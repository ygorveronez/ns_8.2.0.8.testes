/// <reference path="GridMonitoramento.js" />

var _raioXCargaInformacoes;
var _mapaRaioXCarga;
var _gridPedidos;
var _gridDocumentos;
var _gridDetalhesDeEntradaESaidasNoRaio;

var RaioXCargaInformacoes = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.NomeTransportador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Motoristas = PropertyEntity({ val: ko.observableArray([]) });
    this.ModeloVeiculo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Cavalo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Reboque = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.UltimaPosicao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.UltimaPosicaoData = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });

    this.Paradas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.Documentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.CargaInicioViagem = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.CargaParadas = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(0) });
    this.CargaFimViagem = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.PesoTotal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("") });
    this.PesoPendente = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("") });
    this.NumeroParadasPendentes = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });
}

function loadRaioXCarga() {
    _raioXCargaInformacoes = new RaioXCargaInformacoes();
    KoBindings(_raioXCargaInformacoes, "knockoutRaioXCargaInformacoes");
}

function exibirModalRaioXCarga(dados) {
    $('#divModalRaioXCarga').one('show.bs.modal', function (event) {
        var modal = $(this);
        modal.one('hidden.bs.modal', function () {
            modal.find('a[href="#tab-informacoes-raio-x-carga"]').click();
        });
    });

    loadMapaRaioXCarga();
    carregarDadosMapaRaioXCarga(dados);

    _raioXCargaInformacoes.Carga.val(dados.Carga);

    executarReST("Carga/BuscarRaioXCarga", { Codigo: _raioXCargaInformacoes.Carga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_raioXCargaInformacoes, arg);

                loadGridParadas();
                loadGridDocumentos();
                $("#divModalRaioXCarga").modal().find('.modal-title').text(Localization.Resources.Cargas.ControleEntrega.RaioXDaCarga + " " + arg.Data.CodigoCargaEmbarcador);
                Global.abrirModal("divModalRaioXCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function loadMapaRaioXCarga() {
    if (!_mapaRaioXCarga) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaRaioXCarga = new MapaGoogle("mapaRaioXCarga", false, opcoesmapa);
    }
}

function loadGridParadas() {
    let configuracoesExportacao = { url: "ControleEntregaRaiox/ExportarPesquisaParadas", titulo: Localization.Resources.Cargas.ControleEntrega.Paradas };

    let ordenacao = {
        column: 7,
        dir: orderDir.asc,
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Consultas.MonitoramentoDetalhes.Opcoes,
        opcoes: [
            { descricao: Localization.Resources.Cargas.ControleEntrega.DetalhesDeEntradaESaidasNoRaio, id: guid(), evento: "onclick", metodo: loadGridDetalhesEntradaESaidaNoRaio, tamanho: "5", icone: "" }
        ],
        tamanho: 7,
    };

    _gridPedidos = new GridViewExportacao(_raioXCargaInformacoes.Paradas.idGrid, "ControleEntregaRaioX/PesquisaParadas", _raioXCargaInformacoes, menuOpcoes, configuracoesExportacao, ordenacao, 1000);
    _gridPedidos.CarregarGrid();
}

function loadGridDetalhesEntradaESaidaNoRaio(data) {
    Global.abrirModal("modalDetalhesDeEntradaESaidasNoRaio");
    _gridDetalhesDeEntradaESaidasNoRaio = new GridView("grid-detalhe-entrada-saida-raio", "Monitoramento/ObterDetalhesMonitoramentoPermanenciaCliente?carga=" + data.CodigoCarga +"&entrega="+data.Codigo, null, null, null, 10, null, true, null, null, 1000, true, null, null, true, null, false);
    _gridDetalhesDeEntradaESaidasNoRaio.CarregarGrid();
}

function loadGridDocumentos() {
    var configuracoesExportacao = { url: "ControleEntregaRaiox/ExportarPesquisaDocumentos", titulo: Localization.Resources.Cargas.ControleEntrega.Documentos };

    var ordenacao = {
        column: 1,
        dir: orderDir.asc,
    };

    _gridDocumentos = new GridViewExportacao(_raioXCargaInformacoes.Documentos.idGrid, "ControleEntregaRaiox/PesquisaDocumentos", _raioXCargaInformacoes, null, configuracoesExportacao, ordenacao);
    _gridDocumentos.CarregarGrid();
}

function carregarDadosMapaRaioXCarga(filaselecionada) {
    _mapaRaioXCarga.clear();
    var data = { Carga: filaselecionada.Carga, Veiculo: filaselecionada.Veiculo, IDEquipamento: filaselecionada.IDEquipamento };
    executarReST("Monitoramento/ObterDadosMapa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                setTimeout(function () {
                    _mapaRaioXCarga.direction.desenharPolilinha(arg.Data.PolilinhaPrevista);
                    _mapaRaioXCarga.direction.desenharPolilinha(arg.Data.PolilinhaRealizada, false, "#FF6961");

                    if (arg.Data.Entregas.length > 0)
                        TrackingDesenharEntregasMonitoramento(_mapaRaioXCarga, arg.Data.Entregas);
                    else
                        _mapaRaioXCarga.direction.adicionarMarcadorComPontosDaRota(arg.Data.PontosPrevistos, true);

                    _mapaRaioXCarga.PolilinhaPrevista = arg.Data.PolilinhaPrevista;
                    _mapaRaioXCarga.PolilinhaRealizada = arg.Data.PolilinhaRealizada;

                    _raioXCargaInformacoes.UltimaPosicao.val(arg.Data.UltimaPosicao);
                    _raioXCargaInformacoes.UltimaPosicaoData.val(arg.Data.UltimaPosicaoData);

                    if (arg.Data.Entregas.length > 0) {
                        var latitudeZoom = arg.Data.Entregas[arg.Data.Entregas.length - 1].Latitude;
                        var longitudeZoom = arg.Data.Entregas[arg.Data.Entregas.length - 1].Longitude;

                        _mapaRaioXCarga.panTo(latitudeZoom, longitudeZoom);
                    }

                    _mapaRaioXCarga.direction.setZoom(5);

                    desenharAreasControleEntregaMonitoramento(_mapaRaioXCarga, arg.Data.Areas);

                    criarMarkerRaioXCarga(arg.Data)

                }, 500);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function criarMarkerRaioXCarga(info) {
    if ((typeof info.Latitude) == "string")
        info.Latitude = Globalize.parseFloat(info.Latitude);

    if ((typeof info.Longitude) == "string")
        info.Longitude = Globalize.parseFloat(info.Longitude);


    if (info.Latitude == 0 || info.Longitude == 0)
        return;

    var marker = new ShapeMarker();
    marker.setPosition(info.Latitude, info.Longitude);
    marker.icon = _mapaRaioXCarga.draw.icons.truck();
    marker.title =
        '<div>' + Localization.Resources.Cargas.ControleEntrega.Veiculos.getFieldDescription() + ' ' + info.PlacaVeiculo + '</div>' +
        '<div>' + Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription() + ' ' + info.Data + '</div>' +
        '<div>' + Localization.Resources.Cargas.ControleEntrega.Informacoes.getFieldDescription() + ' ' + info.Descricao + ' (' + info.Latitude + ',' + info.Longitude + ')' + '<div>';

    _mapaRaioXCarga.draw.addShape(marker);
}
