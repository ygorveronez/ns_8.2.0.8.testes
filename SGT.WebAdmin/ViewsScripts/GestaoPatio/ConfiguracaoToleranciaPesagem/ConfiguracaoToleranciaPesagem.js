/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumConfiguracaoToleranciaPesagem.js" />
/// <reference path="../../Enumeradores/EnumTipoRegraAutorizacaoToleranciaPesagem.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="Filial.js" />
/// <reference path="ModeloVeicularCarga.js" />

/*Declaração Objetos*/
var _CRUDConfiguracao;
var _configuracaoToleranciaPesagem;
var _pesquisaConfiguracaoToleranciaPesagem;
var _gridConfiguracaoToleranciaPesagem;

var _situacaoOptions = [
    { text: "Inativo", value: 0 },
    { text: "Ativo", value: 1 }
];

var CRUDConfiguracao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
};

var PesquisaConfiguracaoToleranciaPesagem = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: "Descrição:" });
    this.Filiais = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Filia: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Tipo De Carga: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Tipo De Operação: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Modelo Veicular: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumConfiguracaoToleranciaPesagem.ObterOpcoesPesquisa() });
    this.Pesquisar = PropertyEntity({ eventClick: PesquisarConvite, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

var ConfiguracaoToleranciaPesagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "*Situação:", required: true, getType: typesKnockout.dynamic, options: _situacaoOptions, val: ko.observable(1), def: 1, enable: ko.observable(true) });
    this.TipoRegraAutorizacaoToleranciaPesagem = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoRegraAutorizacaoToleranciaPesagem.Todos), options: EnumTipoRegraAutorizacaoToleranciaPesagem.obterOpcoes(), def: EnumTipoRegraAutorizacaoToleranciaPesagem.Peso });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.ToleranciaPesoSuperior = PropertyEntity({ getType: typesKnockout.decimal, text: "Tolerancia peso superior: ", val: ko.observable(0), visible: ko.observable(false) });
    this.ToleranciaPesoInferior = PropertyEntity({ getType: typesKnockout.decimal, text: "Tolerancia peso inferior: ", val: ko.observable(0), visible: ko.observable(false) });
    this.PercentualToleranciaSuperior = PropertyEntity({ getType: typesKnockout.decimal, text: "Percentual tolerancia superior ", val: ko.observable(0), visible: ko.observable(false) });
    this.PercentualToleranciaInferior = PropertyEntity({ getType: typesKnockout.decimal, text: "Percentual tolerancia inferior: ", val: ko.observable(0), visible: ko.observable(false) });

    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.Filial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });


    this.TipoRegraAutorizacaoToleranciaPesagem.val.subscribe(function () {

        if (_configuracaoToleranciaPesagem.TipoRegraAutorizacaoToleranciaPesagem.val() == EnumTipoRegraAutorizacaoToleranciaPesagem.Peso) {
            _configuracaoToleranciaPesagem.ToleranciaPesoSuperior.visible(true);
            _configuracaoToleranciaPesagem.ToleranciaPesoInferior.visible(true);
            _configuracaoToleranciaPesagem.PercentualToleranciaSuperior.visible(false);
            _configuracaoToleranciaPesagem.PercentualToleranciaInferior.visible(false);
        } else {
            _configuracaoToleranciaPesagem.ToleranciaPesoSuperior.visible(false);
            _configuracaoToleranciaPesagem.ToleranciaPesoInferior.visible(false);
            _configuracaoToleranciaPesagem.PercentualToleranciaSuperior.visible(true);
            _configuracaoToleranciaPesagem.PercentualToleranciaInferior.visible(true);
        }
    });        
}

//Métodos Globais
function LoadConfiguracaoToleranciaPesagem() {
    _configuracaoToleranciaPesagem = new ConfiguracaoToleranciaPesagem();
    KoBindings(_configuracaoToleranciaPesagem, "knockoutCadastroConfiguracaoToleranciaPesagem");

    HeaderAuditoria("ConfiguracaoToleranciaPesagem", _configuracaoToleranciaPesagem);

    _CRUDConfiguracao = new CRUDConfiguracao();
    KoBindings(_CRUDConfiguracao, "knockoutCRUDConfiguracao");

    _pesquisaConfiguracaoToleranciaPesagem = new PesquisaConfiguracaoToleranciaPesagem();
    KoBindings(_pesquisaConfiguracaoToleranciaPesagem, "knockoutPesquisaConfiguracaoToleranciaPesagem");

    LoadGridConfiguracaoToleranciaPesagem();

    BuscarTiposdeCarga(_pesquisaConfiguracaoToleranciaPesagem.TipoDeCarga);
    BuscarTiposOperacao(_pesquisaConfiguracaoToleranciaPesagem.TipoOperacao);
    BuscarModelosVeicularesCarga(_pesquisaConfiguracaoToleranciaPesagem.ModeloVeicularCarga);
    BuscarFilial(_pesquisaConfiguracaoToleranciaPesagem.Filiais);

    LoadTipoCarga();
    LoadTipoOperacao();
    LoadFilial();
    LoadModeloVeicularCarga();
}

//Métodos Privados
function LoadGridConfiguracaoToleranciaPesagem() {
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, false); }, tamanho: "10", icone: "" };

    const menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar], tamanho: "10" };
    _gridConfiguracaoToleranciaPesagem = new GridViewExportacao(_pesquisaConfiguracaoToleranciaPesagem.Pesquisar.idGrid, "ConfiguracaoToleranciaPesagem/Pesquisa", _pesquisaConfiguracaoToleranciaPesagem, menuOpcoes);
    _gridConfiguracaoToleranciaPesagem.CarregarGrid();
}

// Eventos de Clique
function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function PesquisarConvite() {
    _gridConfiguracaoToleranciaPesagem.CarregarGrid();
}

function EditarClick(registroSelecionado) {
    LimparCamposConfiguracao();
    

    _configuracaoToleranciaPesagem.Descricao.val(registroSelecionado.Descricao);    

    executarReST("ConfiguracaoToleranciaPesagem/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                _configuracaoToleranciaPesagem.Codigo.val(retorno.Data.Codigo);
                _pesquisaConfiguracaoToleranciaPesagem.ExibirFiltros.visibleFade(false);
                _configuracaoToleranciaPesagem.Situacao.val(retorno.Data.Situacao);
                _configuracaoToleranciaPesagem.TipoRegraAutorizacaoToleranciaPesagem.val(retorno.Data.TipoRegraAutorizacaoToleranciaPesagem);
                _configuracaoToleranciaPesagem.ToleranciaPesoSuperior.val(retorno.Data.ToleranciaPesoSuperior);
                _configuracaoToleranciaPesagem.ToleranciaPesoInferior.val(retorno.Data.ToleranciaPesoInferior);
                _configuracaoToleranciaPesagem.PercentualToleranciaSuperior.val(retorno.Data.PercentualToleranciaSuperior);
                _configuracaoToleranciaPesagem.PercentualToleranciaInferior.val(retorno.Data.PercentualToleranciaInferior);   
                _configuracaoToleranciaPesagem.Descricao.val(retorno.Data.Descricao);   
                _configuracaoToleranciaPesagem.Filial.val(retorno.Data.CodigosFiliais);   
                _configuracaoToleranciaPesagem.ModeloVeicularCarga.val(retorno.Data.CodigosModeloVeicular);   
                _configuracaoToleranciaPesagem.TiposCarga.val(retorno.Data.CodigosTipoCarga);   
                _configuracaoToleranciaPesagem.TiposOperacao.val(retorno.Data.CodigosTipoOperacao);   

                _CRUDConfiguracao.Adicionar.visible(false);
                _CRUDConfiguracao.Atualizar.visible(true);

                RecarregarGridTipoCarga();
                RecarregarOpcoesTipoCarga();
                RecarregarGridTipoOperacao();
                RecarregarOpcoesTipoOperacao();
                RecarregarGridFilial();
                RecarregarOpcoesFilial();
                RecarregarGridModeloVeicularCarga();
                RecarregarOpcoesModeloVeicularCarga();
                return;
            }
            return exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function AdicionarClick(e, sender) {

    if (!ValidarCamposObrigatorios(_configuracaoToleranciaPesagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    adicionarDadosConfiguracao();
}

function LimparClick() {
    LimparCamposConfiguracao();
}

function AtualizarClick() {

    if (!ValidarCamposObrigatorios(_configuracaoToleranciaPesagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    executarReST("ConfiguracaoToleranciaPesagem/Atualizar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Configuração Atualizada com sucesso");
                _gridConfiguracaoToleranciaPesagem.CarregarGrid();
                LimparCamposConfiguracao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}


// Funções privadas

function adicionarDadosConfiguracao() {
    executarReST("ConfiguracaoToleranciaPesagem/Adicionar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Configuração adicionada com sucesso");

                _gridConfiguracaoToleranciaPesagem.CarregarGrid();
                enviarArquivosAnexados(retorno.Data.Codigo);
                LimparCamposConfiguracao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function obterDados() {

    _configuracaoToleranciaPesagem.TiposCarga.val(JSON.stringify(_tipoCarga.Tipo.basicTable.BuscarRegistros()));
    _configuracaoToleranciaPesagem.TiposOperacao.val(JSON.stringify(_tipoOperacao.Tipo.basicTable.BuscarRegistros()));
    _configuracaoToleranciaPesagem.Filial.val(JSON.stringify(_filial.Tipo.basicTable.BuscarRegistros()));
    _configuracaoToleranciaPesagem.ModeloVeicularCarga.val(JSON.stringify(_modeloVeicularCarga.Tipo.basicTable.BuscarRegistros()));

    return RetornarObjetoPesquisa(_configuracaoToleranciaPesagem)
}

function LimparCamposConfiguracao() {
    _CRUDConfiguracao.Adicionar.visible(true);
    _CRUDConfiguracao.Atualizar.visible(false);
    LimparCampos(_configuracaoToleranciaPesagem);
    LimparCamposTipoOperacao();
    LimparCamposTipoCarga();
    LimparCamposFilial();
    LimparCamposModeloVeicularCarga();

    RecarregarOpcoesModeloVeicularCarga();
    RecarregarOpcoesFilial();
    RecarregarOpcoesTipoOperacao();
    RecarregarOpcoesTipoCarga();
}
