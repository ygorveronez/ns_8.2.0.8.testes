
var _configuracaoModeloFiltroPesquisa;
var _gridModeloFiltroPesquisa;

var ConfiguracaoModeloFiltroPesquisa = function () {
    this.TipoFiltro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Filtros = PropertyEntity({ val: ko.observable(""), def: "" });

    this.GridModelosFiltroPesquisa = PropertyEntity({ getTypes: typesKnockout.basicTable, type: types.local });
    this.NovoModeloFiltroPesquisa = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: novoModeloFiltroPesquisaClick });
};

function abrirConfiguracaoModeloFiltroPesquisa(codigoTipoFiltroPesquisa, knockoutFiltros) {
    var filtros;
    if (Boolean(knockoutFiltros) && Boolean(knockoutFiltros.ModeloFiltrosPesquisa)) {
        var knockoutFiltrosAux = $.extend(false, {}, knockoutFiltros); // Faz a copia do knockout de filtros para não deletar a propriedade 'ModeloFiltrosPesquisa' da referência do objeto
        delete knockoutFiltrosAux.ModeloFiltrosPesquisa;
        filtros = RetornarJsonFiltroPesquisa(knockoutFiltrosAux); // Obtem o json do objeto sem 'ModeloFiltrosPesquisa'
    }

    if (_configuracaoModeloFiltroPesquisa) {
        _configuracaoModeloFiltroPesquisa.TipoFiltro.val(codigoTipoFiltroPesquisa);
        _configuracaoModeloFiltroPesquisa.Filtros.val(filtros);
    }

    Global.abrirModal("modal-configuracao-modelo-filtro-pesquisa");  
}

function loadConfiguracaoModeloFiltroPesquisa() {
    _configuracaoModeloFiltroPesquisa = new ConfiguracaoModeloFiltroPesquisa();
    KoBindings(_configuracaoModeloFiltroPesquisa, "knockoutConfiguracaoModeloFiltroPesquisa");

    loadModeloFiltroPesquisaAdicionar();
    loadGridModeloFiltroPesquisa();

    $("#modal-configuracao-modelo-filtro-pesquisa")
        .on('hidden.bs.modal', function () { limparCamposConfiguracoaModeloFiltroPesquisa(); })
        .on('show.bs.modal', function () { buscarModelosFiltrosPesquisa() });
}

function loadGridModeloFiltroPesquisa() {
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: removerModeloFiltroPesquisa, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoRemover], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoUsuario", visible: false },
        { data: "ModeloPadrao", visible: false },
        { data: "ModeloExclusivoUsuario", visible: false },
        { data: "ModeloDescricao", title: Localization.Resources.Gerais.Geral.Modelo, width: "50%", className: "text-align-left" },
        { data: "ModeloPadraoDescricao", title: Localization.Resources.Gerais.Geral.ModeloPadrao, width: "20%", className: "text-align-center" },
        { data: "ModeloExclusivoUsuarioDescricao", title: Localization.Resources.Gerais.Geral.ModeloExclusivoUsuario, width: "20%", className: "text-align-center" },
    ];
    
    _gridModeloFiltroPesquisa = new BasicDataTable(_configuracaoModeloFiltroPesquisa.GridModelosFiltroPesquisa.id, header, menuOpcoes, null, null, 5);
}

function removerModeloFiltroPesquisa(data) {
    executarReST("ModeloFiltroPesquisa/RemoverModeloFiltroPesquisa", { CodigoModelo: data.Codigo }, function (res) {
        if (res.Success) {
            if (res.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                buscarModelosFiltrosPesquisa();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, res.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, res.Msg);
        }
    });
}

function buscarModelosFiltrosPesquisa() {
    executarReST("ModeloFiltroPesquisa/PesquisarModelos", { TipoFiltro: _configuracaoModeloFiltroPesquisa.TipoFiltro.val() }, function (res) {
        if (res.Success) {
            if (res.Data !== false) {
                _gridModeloFiltroPesquisa.CarregarGrid(res.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, res.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, res.Msg);
        }
    });
};

function limparCamposConfiguracoaModeloFiltroPesquisa() {
    _gridModeloFiltroPesquisa.CarregarGrid([]);
    LimparCampos(_configuracaoModeloFiltroPesquisa);
}

function novoModeloFiltroPesquisaClick() {
    Global.abrirModal("modal-modelo-filtro-pesquisa-adicionar");
}