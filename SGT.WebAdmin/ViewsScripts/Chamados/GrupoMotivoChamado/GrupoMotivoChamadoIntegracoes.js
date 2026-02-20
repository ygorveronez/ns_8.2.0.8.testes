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
/// <reference path="GrupoMotivoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoMotivoChamadoIntegracoes, _gridGrupoMotivoChamadoIntegracoes, _crudGrupoMotivoChamadoIntegracoes;

var GrupoMotivoChamadoIntegracoes = function () {
    this.Grid = PropertyEntity({ text: 'Integrações', type: types.local, idGrid: guid() });

    this.SistemaIntegracao = PropertyEntity({ text: 'Sistema de Integração', required: true, val: ko.observable(_CONFIGURACAO_TMS.IntegracoesAtivas[0]), def: _CONFIGURACAO_TMS.IntegracoesAtivas[0], options: _CONFIGURACAO_TMS.IntegracoesAtivas });
};

var CRUDGrupoMotivoChamadoIntegracoes = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoMotivoChamadoIntegracoesClick, type: types.event, text: 'Adicionar Integração', visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadGrupoMotivoChamadoIntegracoes() {
    if (!_CONFIGURACAO_TMS.ExisteIntegracaoUnilever)
        return;
    else
        $('#liTabGrupoMotivoChamadoIntegracoes').show();

    _grupoMotivoChamadoIntegracoes = new GrupoMotivoChamadoIntegracoes();
    KoBindings(_grupoMotivoChamadoIntegracoes, "knockoutGrupoMotivoChamadoIntegracoes");

    _crudGrupoMotivoChamadoIntegracoes = new CRUDGrupoMotivoChamadoIntegracoes();
    KoBindings(_crudGrupoMotivoChamadoIntegracoes, "knockoutCRUDGrupoMotivoChamadoIntegracoes");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerGrupoMotivoChamadoIntegracoesClick }
        ]
    };

    var header = [
        { data: "CodigoSistemaIntegracao", visible: false },
        { data: "DescricaoSistemaIntegracao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%" },
    ];

    _gridGrupoMotivoChamadoIntegracoes = new BasicDataTable(_grupoMotivoChamadoIntegracoes.Grid.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridGrupoMotivoChamadoIntegracoes.CarregarGrid([]);
}

//*******MÉTODOS*******

function adicionarGrupoMotivoChamadoIntegracoesClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_grupoMotivoChamadoIntegracoes);
    let list = _gridGrupoMotivoChamadoIntegracoes.BuscarRegistros();

    let codigo = _grupoMotivoChamadoIntegracoes.SistemaIntegracao.val();

    if (valido) {
        list.push({
            CodigoSistemaIntegracao: codigo,
            DescricaoSistemaIntegracao: _CONFIGURACAO_TMS.IntegracoesAtivas.find(x => x.value === codigo).text
        });

        limparCamposGrupoMotivoChamadoIntegracoes();
        _gridGrupoMotivoChamadoIntegracoes.CarregarGrid(list);
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function removerGrupoMotivoChamadoIntegracoesClick(registro) {
    let list = _gridGrupoMotivoChamadoIntegracoes.BuscarRegistros();

    list = list.filter(x => x.CodigoSistemaIntegracao != registro.CodigoSistemaIntegracao);
    _gridGrupoMotivoChamadoIntegracoes.CarregarGrid(list);
}

function limparCamposGrupoMotivoChamadoIntegracoes() {
    LimparCampos(_grupoMotivoChamadoIntegracoes);
    _gridGrupoMotivoChamadoIntegracoes.CarregarGrid([]);
    _crudGrupoMotivoChamadoIntegracoes.Adicionar.visible(true);
}