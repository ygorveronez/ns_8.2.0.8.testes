var _alertas;
var _gridAlertas;
var _mapaAlerta;
var _tratativasAlerta;

var TratativasAlerta = function () {
    this.Codigo = PropertyEntity({ text: "Codigo" });
    this.CodigoAlerta = PropertyEntity({ text: "CodigoAlerta" });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Alerta.getFieldDescription() });
    this.Data = PropertyEntity({ getType: typesKnockout.string, text: "Data início" });
    this.DataFim = PropertyEntity({ getType: typesKnockout.string, text: "Data fim" });
    this.ObservacaoMotorista = PropertyEntity({ getType: typesKnockout.string, text: "Observação do motorista", visible: ko.observable(false) });
    this.TipoAlerta = PropertyEntity({});
    this.Latitude = PropertyEntity({});
    this.Longitude = PropertyEntity({});
    this.Observacao = PropertyEntity({ text: "Observação", getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable(""), maxlength: 2000, visible: ko.observable(false) });
    this.Tratativa = PropertyEntity({ text: "Tratativa", val: ko.observable(true), enable: false, options: ko.observable([]), def: 1, visible: ko.observable(false) });
    this.UtilizaTratativa = PropertyEntity({ val: ko.observable(true) });
    this.ValorAlerta = PropertyEntity({ getType: typesKnockout.string, text: "Valor", visible: ko.observable(false) });
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarTratativaAlertaTransportadorClick, text: "Confirmar", visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: fecharTratativaAlertaTransportadorClick, text: "Fechar", visible: ko.observable(true) });

}

var Alertas = function () {
}

var PesquisaAlertasTransportador = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.Placa = PropertyEntity({ text: "Placa:" });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.AlertaMonitorStatus = PropertyEntity({ text: "Status:", val: ko.observable(null), options: EnumAlertaMonitorStatus.obterOpcoesPesquisa(), def: null });
    this.TipoAlerta = PropertyEntity({ text: "Tipo:", val: ko.observable(null), options: EnumTipoAlerta.obterOpcoesPesquisa(), def: null });
    this.ExibirApenasComPosicaoTardia = PropertyEntity({ text: "Com Posição Retroativa", getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAlertas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

function loadAlertas() {
    _alertas = new Alertas();
    KoBindings(_alertas, "knockoutAlertas");

    _tratativasAlerta = new TratativasAlerta();
    KoBindings(_tratativasAlerta, "knockoutTratativasAlerta");

    _pesquisaAlertasTransportador = new PesquisaAlertasTransportador();
    KoBindings(_pesquisaAlertasTransportador, "knockoutPesquisaAlertasTransportador");

    BuscarTransportadores(_pesquisaAlertasTransportador.Transportador, null, null, true);
    BuscarFuncionario(_pesquisaAlertasTransportador.Motorista);
    loadGridAlertasTransportador();
}

function loadGridAlertasTransportador() {

    var opcaoDetalheMonitoramento = { descricao: "Tratativas", id: guid(), evento: "onclick", metodo: buscarAcoesTratativaAlerta, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoDetalheMonitoramento], tamanho: 5 };
    var draggableRows = false;
    var limiteRegistros = 20;
    var totalRegistrosPorPagina = 20;
    var configuracoesExportacao = { url: "AlertasTransportador/ExportarPesquisa", titulo: "Alertas" };

    _gridAlertas = new GridViewExportacao("grid-alertas-transportador", "AlertasTransportador/Pesquisa", _pesquisaAlertasTransportador, menuOpcoes, configuracoesExportacao, null, totalRegistrosPorPagina, null, limiteRegistros, null);
    _gridAlertas.SetSalvarPreferenciasGrid(true);
    _gridAlertas.SetPermitirEdicaoColunas(true);
    _gridAlertas.SetPermitirReordenarColunas(true);
    _gridAlertas.SetHabilitarModelosGrid(true);
    _gridAlertas.CarregarGrid();

}
function buscarAcoesTratativaAlerta(row) {
    loadTratativaAlerta({ CodigoAlerta: parseInt(row.Codigo) }, [row.Codigo]);
}

function confirmarTratativaAlertaTransportadorClick(e, sender) {

    Salvar(_tratativasAlerta, "AlertaTratativa/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                Global.fecharModal("divModalAlertasTransportador");
                _gridAlertas.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function fecharTratativaAlertaTransportadorClick() {
    Global.fecharModal("divModalAlertasTransportador");

}


function setarTratamentoPorTipoAlerta(alerta) {

    //alerta do tipo Parada Nao Programada (tem uma tratativa diferente) #25952
    alerta.Data.text(Localization.Resources.Cargas.ControleEntrega.DataAlerta);
    alerta.DataFim.text(Localization.Resources.Cargas.ControleEntrega.DataTratativa);
    var valor = alerta.ValorAlerta.val();
    if (valor != null && valor.length > 5) {
        alerta.Data.val(alerta.ValorAlerta.val());
        alerta.TempoParado.val(valor.substr(valor.length - 5, 5));
        alerta.TempoParado.visible(true);
    }

}

function setarVisibilidadeCampos(visivel) {
    if (visivel) {
        _tratativasAlerta.Observacao.visible(true);
        _tratativasAlerta.Tratativa.visible(true);
    }

    var finalizado = !_tratativasAlerta.DataFim.val() != "";

    _tratativasAlerta.Confirmar.visible(finalizado);
    _tratativasAlerta.Tratativa.enable = finalizado;
    _tratativasAlerta.Observacao.enable = finalizado;

}

function carregarMapaAlertaTransportador() {

    if (_mapaAlerta == null) {
        opcoesMapa = new OpcoesMapa(false, false);

        _mapaAlerta = new MapaGoogle("mapaControleEntregaAlerta", false, opcoesMapa);
    }

    _mapaAlerta.clear();
}

function recarregarGridAlertasTransportador() {
    _gridAlertas.CarregarGrid();
}

function criarMakerAlerta(coordenada) {

    if ((coordenada.lat == 0) && (coordenada.lng == 0))
        return;

    if ((typeof coordenada.lat) == "string")
        coordenada.lat = Globalize.parseFloat(coordenada.lat);

    if ((typeof coordenada.lng) == "string")
        coordenada.lng = Globalize.parseFloat(coordenada.lng);

    var marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.icon = _mapaAlerta.draw.icons.truck();
    marker.title = '';
    _mapaAlerta.draw.addShape(marker);
    _mapaAlerta.direction.centralizar(coordenada.lat, coordenada.lng);
}
