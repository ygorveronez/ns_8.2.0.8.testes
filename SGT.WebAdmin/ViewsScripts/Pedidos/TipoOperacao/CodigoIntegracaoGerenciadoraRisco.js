/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

let _codigosIntegracaoTipoOperacao;
let _gridCodigosIntegracaoTipoOperacao;

const CodigosIntegracaoTipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 50, required: true });
    this.EtapaCarga = PropertyEntity({ options: EnumSituacoesCarga.obterOpcoesIntegracaoApisul(), def: EnumSituacoesCarga.Todas, text: Localization.Resources.Gerais.Geral.EtapaCarga, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCodigoIntegracaoTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

function carregarCodigosIntegracaoTipoOperacao() {
    _codigosIntegracaoTipoOperacao = new CodigosIntegracaoTipoOperacao();

    KoBindings(_codigosIntegracaoTipoOperacao, "tabGerenciamentoRisco");

    carregarGridCodigosIntegracaoTipoOperacao();
}

function carregarGridCodigosIntegracaoTipoOperacao() {
    const menuExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirCodigoIntegracaoTipoOperacaoClick };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [menuExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "60%" },
        { data: "EtapaCarga", visible: false },
        { data: "EtapaCargaDescricao", title: Localization.Resources.Gerais.Geral.EtapaCarga, width: "60%" }
    ];

    _gridCodigosIntegracaoTipoOperacao = new BasicDataTable(_codigosIntegracaoTipoOperacao.Grid.id, header, menuOpcoes);
    _gridCodigosIntegracaoTipoOperacao.CarregarGrid([]);
}

function excluirCodigoIntegracaoTipoOperacaoClick(registroSelecionado) {
    const registros = _gridCodigosIntegracaoTipoOperacao.BuscarRegistros();

    for (let i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _gridCodigosIntegracaoTipoOperacao.CarregarGrid(registros);
}

function adicionarCodigoIntegracaoTipoOperacaoClick() {
    if (!ValidarCamposObrigatorios(_codigosIntegracaoTipoOperacao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    const registros = _gridCodigosIntegracaoTipoOperacao.BuscarRegistros();
    const codigoIntegracaoCadastrar = _codigosIntegracaoTipoOperacao.CodigoIntegracao.val();
    const etapaCargaCadastrar = _codigosIntegracaoTipoOperacao.EtapaCarga.val();

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

    _gridCodigosIntegracaoTipoOperacao.CarregarGrid(registros);

    LimparCampo(_codigosIntegracaoTipoOperacao.CodigoIntegracao);
}

function limparCamposCodigosIntegracaoTipoOperacao() {
    LimparCampo(_codigosIntegracaoTipoOperacao.CodigoIntegracao);
    _gridCodigosIntegracaoTipoOperacao.CarregarGrid([]);
}

function preencherGridCodigosIntegracaoTipoOperacao(data) {
    _gridCodigosIntegracaoTipoOperacao.CarregarGrid(data);
}