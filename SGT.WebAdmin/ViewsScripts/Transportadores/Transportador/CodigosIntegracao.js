var _transportadorCodigosIntegracao,
    _gridTransportadorCodigosIntegracao;

var TransportadorCodigosIntegracao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 50, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTransportadorCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

function loadTransportadorCodigosIntegracao() {
    _transportadorCodigosIntegracao = new TransportadorCodigosIntegracao();

    KoBindings(_transportadorCodigosIntegracao, "knockoutTabCodigosIntegracao");
    
    loadGridTransportadorCodigosIntegracao();
}

function loadGridTransportadorCodigosIntegracao() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirTransportadorCodigoIntegracaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "80%" }
    ];

    _gridTransportadorCodigosIntegracao = new BasicDataTable(_transportadorCodigosIntegracao.Grid.id, header, menuOpcoes);
    _gridTransportadorCodigosIntegracao.CarregarGrid([]);
}

function excluirTransportadorCodigoIntegracaoClick(registroSelecionado) {
    var registros = _gridTransportadorCodigosIntegracao.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _gridTransportadorCodigosIntegracao.CarregarGrid(registros);
}

function adicionarTransportadorCodigoIntegracaoClick() {
    if (!ValidarCamposObrigatorios(_transportadorCodigosIntegracao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var codigoIntegracaoCadastrar = _transportadorCodigosIntegracao.CodigoIntegracao.val();
    
    if (verificarExistenciaCodigoIntegracao(codigoIntegracaoCadastrar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.CodigoIntegracaoJaCadastrado);
        return;
    }

    var registros = _gridTransportadorCodigosIntegracao.BuscarRegistros();

    registros.push({
        Codigo: guid(),
        CodigoIntegracao: codigoIntegracaoCadastrar
    });
    
    _gridTransportadorCodigosIntegracao.CarregarGrid(registros);
    
    LimparCampo(_transportadorCodigosIntegracao.CodigoIntegracao);
}

function verificarExistenciaCodigoIntegracao(codigoIntegracao) {
    return _gridTransportadorCodigosIntegracao.BuscarRegistros().some(function (v) {
        return v.CodigoIntegracao === codigoIntegracao;
    });
}

function limparCamposTransportadorCodigosIntegracao() {
    LimparCampo(_transportadorCodigosIntegracao.CodigoIntegracao);
    _gridTransportadorCodigosIntegracao.CarregarGrid([]);
}

function preencherGridTransportadorCodigosIntegracao(data) {
    _gridTransportadorCodigosIntegracao.CarregarGrid(data);
}