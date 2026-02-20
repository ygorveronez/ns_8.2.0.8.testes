
//*******MAPEAMENTO KNOUCKOUT*******

var _gridPracaPedagioRotaFrete;
var _pracaPedagioRotaFrete;
var _pracaPedagioIdaVolta;
var _sentidoSelecionado;

var _sentidoPracaPedagio = [
    { text: EnumEixosSuspenso.obterDescricao(EnumEixosSuspenso.Ida), value: EnumEixosSuspenso.Ida },
    { text: EnumEixosSuspenso.obterDescricao(EnumEixosSuspenso.Volta), value: EnumEixosSuspenso.Volta }
];

var PracaPedagioRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PracaPedagio = PropertyEntity({ type: types.event, eventClick: pracaPedagioIdaVoltaClick, text: Localization.Resources.Logistica.RotaFrete.AdicionarPracaDePedagio });
    this.ObterPracas = PropertyEntity({ eventClick: obterPracasClick, type: types.event, text: Localization.Resources.Logistica.RotaFrete.BuscarPracasPedagioSemParar, visible: ko.observable(true) });

    this.HistoricoIntegracao = PropertyEntity({ eventClick: exibirHistoricoIntegracao, type: types.event, text: Localization.Resources.Gerais.Geral.HistoricoIntegracao, visible: ko.observable(false) });
};

var PracaPedagioIdaVolta = function () {
    this.Sentido = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Sentido.getFieldDescription(), options: _sentidoPracaPedagio, val: ko.observable(_sentidoPracaPedagio.Ida), def: _sentidoPracaPedagio.Ida });

    this.Pracas = PropertyEntity({ type: types.event, eventClick: buscarPracaPedagioIdaVolta, text: Localization.Resources.Logistica.RotaFrete.BuscarPracasPedagio, idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadPracaPedagioRotaFrete() {

    _pracaPedagioRotaFrete = new PracaPedagioRotaFrete();
    KoBindings(_pracaPedagioRotaFrete, "knockoutPracaPedagioRotaFrete");

    _pracaPedagioIdaVolta = new PracaPedagioIdaVolta();
    KoBindings(_pracaPedagioIdaVolta, "knockoutPracaPedagioIdaVolta");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirPracaPedagioRotaFreteClick(_pracaPedagioIdaVolta.Pracas, data);
            }
        }]
    };

    var header = [
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%" },
        { data: "Sentido", title: Localization.Resources.Logistica.RotaFrete.Sentido, width: "20%" },
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Rodovia", visible: false },
        { data: "KM", visible: false },
        { data: "Ordem", visible: false }
    ];

    _gridPracaPedagioRotaFrete = new BasicDataTable(_pracaPedagioRotaFrete.Grid.id, header, menuOpcoes, { column: 7, dir: orderDir.asc });

    new BuscarPracaPedagio(_pracaPedagioIdaVolta.Pracas, retornoBuscaPracaPedagio, _gridPracaPedagioRotaFrete);

    _pracaPedagioIdaVolta.Pracas.basicTable = _gridPracaPedagioRotaFrete;

    RecarregarGridPracaPedagioRotaFrete();
}


function BuscarPracasClick() {
    _rotamapa._setPracas(LimparBotaoPracaPedagio);
}

function LimparBotaoPracaPedagio() {
    if (_rotaFrete.Codigo.val() > 0)
        _crudRotaFrete.Atualizar.visible(true);
    else {
        _crudRotaFrete.Adicionar.visible(true);
        _crudRotaFrete.Cancelar.visible(false);
    }
    _crudRotaFrete.BuscarPracas.visible(false);
}

function ExibirBuscarPracaRota() {
    if (_integracaoSemParar) {
        _crudRotaFrete.Atualizar.visible(false);
        _crudRotaFrete.Adicionar.visible(false);
        _crudRotaFrete.BuscarPracas.visible(true);
        _crudRotaFrete.Cancelar.visible(true);
        _gridPracaPedagioRotaFrete.CarregarGrid(new Array());
        _rotamapa._mapa.adicionarPracasPedagio(new Array());
    }
}

function RecarregarGridPracaPedagioRotaFrete() {
    _gridPracaPedagioRotaFrete.CarregarGrid(_rotaFrete.PracaPedagios.val());
}

function ExcluirPracaPedagioRotaFreteClick(knoutPracaPedagio, data) {

    var pracaPedagiosGrid = knoutPracaPedagio.basicTable.BuscarRegistros();

    for (var i = 0; i < pracaPedagiosGrid.length; i++) {
        if (data.Codigo == pracaPedagiosGrid[i].Codigo) {
            pracaPedagiosGrid.splice(i, 1);
            break;
        }
    }

    knoutPracaPedagio.basicTable.CarregarGrid(pracaPedagiosGrid);
}

function LimparCamposPracaPedagioRotaFrete() {
    LimparCampos(_pracaPedagioRotaFrete);
    _gridPracaPedagioRotaFrete.CarregarGrid(new Array());
}


function obterPracasClick(e, sender) {
    var clientes = _destinatarioRotaFrete.Destinatario.basicTable.BuscarRegistros();

    var buscarPracas = true;

    if (clientes.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.ParaBuscarPracasPegagioInformarDestinatario);
        buscarPracas = false;
    }

    if (_rotaFrete.Remetente.codEntity() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.ParaBuscarPracasPedagioInformarRemetente);
        buscarPracas = false;
    }

    if (buscarPracas) {

        var ultimoPonto = _roteirizador.TipoUltimoPontoRoteirizacao.val();

        var dados = { Destinatarios: JSON.stringify(_destinatarioRotaFrete.Destinatario.basicTable.BuscarRegistros()), Remetente: _rotaFrete.Remetente.codEntity(), tipoUltimoPontoRoteirizacao: ultimoPonto, Codigo: _rotaFrete.Codigo.val() };
        var path = "RotaFrete/BuscarPracasPedagio"

        if (!string.IsNullOrWhiteSpace(_rotaFrete.PolilinhaRota.val())) {
            dados = { polilinha: _rotaFrete.PolilinhaRota.val(), pontosDaRota: _rotaFrete.PontosDaRota.val(), tipoUltimoPontoRoteirizacao: ultimoPonto, ApenasObterPracasPedagio: _rotaFrete.ApenasObterPracasPedagio.val(), Codigo: _rotaFrete.Codigo.val() };
            path = "RotaFrete/BuscarPracasPedagioPolilinha"
        }

        executarReST(path, dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.RotaFrete.PracasObtidasComSucesso);
                    _rotaFrete.PracaPedagios.val(arg.Data);
                    _gridPracaPedagioRotaFrete.CarregarGrid(_rotaFrete.PracaPedagios.val());
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, function () {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            ResetarTabs();
        });
    }

}

function pracaPedagioIdaVoltaClick() {
    if (_roteirizador.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem || _roteirizador.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.Retornando) {
        Global.abrirModal('divModalAdicionarPracaPedagio');
    } else {
        _pracaPedagioIdaVolta.Sentido.val(EnumEixosSuspenso.Ida);
        $("#" + _pracaPedagioIdaVolta.Pracas.idBtnSearch).click();
    }
}

function buscarPracaPedagioIdaVolta() {
    _sentidoSelecionado = _pracaPedagioIdaVolta.Sentido.val();
    Global.fecharModal('divModalAdicionarPracaPedagio');
}

function retornoBuscaPracaPedagio(dados) {
    for (i = 0; i < dados.length; i++) {
        dados[i].Sentido = _sentidoSelecionado == 2 ? EnumEixosSuspenso.obterDescricao(EnumEixosSuspenso.Volta) : EnumEixosSuspenso.obterDescricao(EnumEixosSuspenso.Ida);
    }

    var pracas = new Array();
    var registros = _pracaPedagioIdaVolta.Pracas.basicTable.BuscarRegistros();
    console.log(registros);
    console.log(dados);

    if (registros.length > 0)
        pracas = registros.concat(dados);
    else
        pracas = dados;

    console.log(pracas);
    _pracaPedagioIdaVolta.Pracas.basicTable.CarregarGrid(pracas);
}

function exibirHistoricoIntegracao() {
    BuscarHistoricoIntegracaoRotaFrete();
    $("#divModalHistoricoIntegracao").modal("show");
}