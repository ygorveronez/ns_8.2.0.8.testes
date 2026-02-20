
var _modeloFiltroPesquisaAdicionar;

var ModeloFiltroPesquisaAdicionar = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true });
    this.ModeloExclusivoUsuario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ModeloExclusivoUsuario, val: ko.observable(true), def: true, required: true });
    this.ModeloExclusivoUsuario.val.subscribe(function (valor) {
        if (valor)
            _modeloFiltroPesquisaAdicionar.ModeloPadrao.text(Localization.Resources.Gerais.Geral.ModeloPadraoDoUsuario);
        else
            _modeloFiltroPesquisaAdicionar.ModeloPadrao.text(Localization.Resources.Gerais.Geral.ModeloPadraoDaPesquisa);
    });
    this.ModeloPadrao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.ModeloPadraoDoUsuario), val: ko.observable(false), def: false, required: true });
    this.AvancarDatasAutomaticamente = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.AvancarDatasAutomaticamente), val: ko.observable(false), def: false, required: true });

    this.AdicionarModeloFiltroPesquisa = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: adicionarModeloFiltroPesquisa });
}

function loadModeloFiltroPesquisaAdicionar() {
    _modeloFiltroPesquisaAdicionar = new ModeloFiltroPesquisaAdicionar();
    KoBindings(_modeloFiltroPesquisaAdicionar, "knockoutModeloFiltroPesquisaAdicionar");

    $("#modal-modelo-filtro-pesquisa-adicionar").on('hidden.bs.modal', function () {
        limparCamposModeloFiltroPesquisaAdicionar();
    });
}

function adicionarModeloFiltroPesquisa(e, sender) {
    if (!ValidarCamposObrigatorios(_modeloFiltroPesquisaAdicionar)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return false;
    }

    var data = {
        TipoFiltro: _configuracaoModeloFiltroPesquisa.TipoFiltro.val(),
        Filtros: _configuracaoModeloFiltroPesquisa.Filtros.val(),
        Descricao: _modeloFiltroPesquisaAdicionar.Descricao.val(),
        ModeloExclusivoUsuario: _modeloFiltroPesquisaAdicionar.ModeloExclusivoUsuario.val(),
        ModeloPadrao: _modeloFiltroPesquisaAdicionar.ModeloPadrao.val(),
        AvancarDatasAutomaticamente: _modeloFiltroPesquisaAdicionar.AvancarDatasAutomaticamente.val()
    };

    executarReST("ModeloFiltroPesquisa/AdicionarModeloFiltroPesquisa", data, function (res) {
        if (res.Success) {
            if (res.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                buscarModelosFiltrosPesquisa();
                limparCamposModeloFiltroPesquisaAdicionar();
                Global.fecharModal("modal-modelo-filtro-pesquisa-adicionar");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, res.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, res.Msg);
        }
    });
}

function limparCamposModeloFiltroPesquisaAdicionar() {
    LimparCampos(_modeloFiltroPesquisaAdicionar);
}