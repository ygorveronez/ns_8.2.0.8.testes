let _codigosIntegracaoTipoCarga;
let _gridCodigosIntegracaoTipoCarga;

const CodigosIntegracaoTipoCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 50, required: true });
    this.EtapaCarga = PropertyEntity({ options: EnumSituacoesCarga.obterOpcoesIntegracaoApisul(), def: EnumSituacoesCarga.Todas, text: Localization.Resources.Gerais.Geral.EtapaCarga, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCodigoIntegracaoTipoCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

function carregarCodigosIntegracaoTipoCarga() {
    _codigosIntegracaoTipoCarga = new CodigosIntegracaoTipoCarga();

    KoBindings(_codigosIntegracaoTipoCarga, "knockoutTabCodigosIntegracao");

    carregarGridCodigosIntegracaoTipoCarga();
}

function carregarGridCodigosIntegracaoTipoCarga() {
    const menuExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirCodigoIntegracaoTipoCargaClick };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [menuExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "60%" },
        { data: "EtapaCarga", visible: false },
        { data: "EtapaCargaDescricao", title: Localization.Resources.Gerais.Geral.EtapaCarga, width: "60%" }
    ];

    _gridCodigosIntegracaoTipoCarga = new BasicDataTable(_codigosIntegracaoTipoCarga.Grid.id, header, menuOpcoes);
    _gridCodigosIntegracaoTipoCarga.CarregarGrid([]);
}

function excluirCodigoIntegracaoTipoCargaClick(registroSelecionado) {
    const registros = _gridCodigosIntegracaoTipoCarga.BuscarRegistros();

    for (let i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _gridCodigosIntegracaoTipoCarga.CarregarGrid(registros);
}

function adicionarCodigoIntegracaoTipoCargaClick() {
    if (!ValidarCamposObrigatorios(_codigosIntegracaoTipoCarga)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    const registros = _gridCodigosIntegracaoTipoCarga.BuscarRegistros();
    const codigoIntegracaoCadastrar = _codigosIntegracaoTipoCarga.CodigoIntegracao.val();
    const etapaCargaCadastrar = _codigosIntegracaoTipoCarga.EtapaCarga.val();

    if (registros.some((registro) => registro.CodigoIntegracao === codigoIntegracaoCadastrar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.TipoCarga.CodigoIntegracaoJaCadastrado);
        return;
    }

    if (registros.some((registro) => (registro.EtapaCarga == EnumSituacoesCarga.Todas || etapaCargaCadastrar == EnumSituacoesCarga.Todas) && registros.length >= 1 || registro.EtapaCarga == etapaCargaCadastrar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.TipoCarga.CodigoIntegracaoEtapaRepetida);
        return;
    }

    registros.push({
        Codigo: guid(),
        CodigoIntegracao: codigoIntegracaoCadastrar,
        EtapaCarga: etapaCargaCadastrar,
        EtapaCargaDescricao: EnumSituacoesCarga.obterDescricao(etapaCargaCadastrar)
    });

    _gridCodigosIntegracaoTipoCarga.CarregarGrid(registros);

    LimparCampo(_codigosIntegracaoTipoCarga.CodigoIntegracao);
}

function limparCamposCodigosIntegracaoTipoCarga() {
    LimparCampo(_codigosIntegracaoTipoCarga.CodigoIntegracao);
    _gridCodigosIntegracaoTipoCarga.CarregarGrid([]);
}

function preencherGridCodigosIntegracaoTipoCarga(data) {
    _gridCodigosIntegracaoTipoCarga.CarregarGrid(data);
}