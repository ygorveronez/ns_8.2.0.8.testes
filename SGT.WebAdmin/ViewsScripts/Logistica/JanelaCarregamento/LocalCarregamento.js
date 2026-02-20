/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/AreaVeiculoPosicao.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _localCarregamento;

/*
 * Declaração das Classes
 */

var LocalCarregamento = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.LocalCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LocalCarregamento, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });

    this.Salvar = PropertyEntity({ eventClick: salvarLocalCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLocalCarregamento() {
    _localCarregamento = new LocalCarregamento();
    KoBindings(_localCarregamento, "knockoutLocalCarregamento");

    new BuscarAreaVeiculoPosicao(_localCarregamento.LocalCarregamento, null, _localCarregamento.CentroCarregamento, null, null, EnumTipoAreaVeiculo.Doca);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarLocalCarregamentoClick() {
    if (!ValidarCamposObrigatorios(_localCarregamento)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    executarReST("JanelaCarregamento/SalvarLocalCarregamento", RetornarObjetoPesquisa(_localCarregamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LocalDeCarregamentoAdicionadoComSucesso);
                fecharModalLocalCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalLocalCarregamento(codigoJanelaCarregamento) {
    _localCarregamento.Codigo.val(codigoJanelaCarregamento);
    _localCarregamento.CentroCarregamento.codEntity(_dadosPesquisaCarregamento.CentroCarregamento);
    _localCarregamento.CentroCarregamento.val(_dadosPesquisaCarregamento.CentroCarregamento);

    Global.abrirModal('divModalLocalCarregamento');
    $("#divModalLocalCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_localCarregamento);
    });
}

/*
 * Declaração das Funções Privadas
 */

function fecharModalLocalCarregamento() {
    Global.fecharModal('divModalLocalCarregamento');
}
