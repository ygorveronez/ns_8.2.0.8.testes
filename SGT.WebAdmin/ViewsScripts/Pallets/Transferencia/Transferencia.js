/// <reference path="Aprovacao.js" />
/// <reference path="CruzamentoInformacoes.js" />
/// <reference path="Envio.js" />
/// <reference path="Etapas.js" />
/// <reference path="Recebimento.js" />
/// <reference path="Resumo.js" />
/// <reference path="Solicitacao.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Enumeradores/EnumAbaTransferenciaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _CRUDTransferenciaPallet;
var _dadosSetorUsuario;
var _gridTransferenciaPallet;
var _pesquisaTransferenciaPallet;
var _transferenciaPallet;

/*
 * Declaração das Classes
 */

var DadosSetorUsuario = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
}

var PesquisaTransferenciaPallet = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Solicitação início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Solicitação limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTransferenciaPallet.Todas), options: EnumSituacaoTransferenciaPallet.obterOpcoes(), def: EnumSituacaoTransferenciaPallet.Todas, text: "Situação: " });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Turno:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTransferenciaPallet.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var TransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTransferenciaPallet.Todas), def: EnumSituacaoTransferenciaPallet.Todas, text: "*Situação: ", required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}

var CRUDTransferenciaPallet = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova", visible: ko.observable(true) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTransferenciasPallets() {
    var editarRegistro = {
        descricao: "Editar",
        id: "clasEditar",
        evento: "onclick",
        metodo: editarClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editarRegistro]
    }

    var configuracaoExportacao = {
        url: "Transferencia/ExportarPesquisa",
        titulo: "Transferência de Pallets"
    };

    _gridTransferenciaPallet = new GridViewExportacao(_pesquisaTransferenciaPallet.Pesquisar.idGrid, "Transferencia/Pesquisa", _pesquisaTransferenciaPallet, menuOpcoes, configuracaoExportacao);
    _gridTransferenciaPallet.CarregarGrid();
}

function loadTransferenciaPallet() {
    _transferenciaPallet = new TransferenciaPallet();
    HeaderAuditoria("TransferenciaPallet", _transferenciaPallet);

    _pesquisaTransferenciaPallet = new PesquisaTransferenciaPallet();
    KoBindings(_pesquisaTransferenciaPallet, "knockoutPesquisaTransferenciaPallet", false, _pesquisaTransferenciaPallet.Pesquisar.id);

    _dadosSetorUsuario = new DadosSetorUsuario();

    new BuscarFilial(_pesquisaTransferenciaPallet.Filial);
    new BuscarSetorFuncionario(_pesquisaTransferenciaPallet.Setor);
    new BuscarTurno(_pesquisaTransferenciaPallet.Turno);

    _CRUDTransferenciaPallet = new CRUDTransferenciaPallet();
    KoBindings(_CRUDTransferenciaPallet, "knockoutCRUDCadastroTransferenciaPallet");

    loadEtapaTransferenciaPallet();
    loadResumoTransferenciaPallet();
    loadSolicitacaoTransferenciaPallet();
    loadEnvioTransferenciaPallet();
    loadAprovacaoTransferenciaPallet();
    loadRecebimentoTransferenciaPallet();
    loadCruzamentoInformacoesTransferenciaPallet();

    buscarDadosSetorUsuario(function () {
        loadGridTransferenciasPallets();
        setarEtapaInicial();
        controlarComponentesHabilitados();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    switch (_abaAtiva) {
        case EnumAbaTransferenciaPallet.Solicitacao:
            adicionarSolicitacao();
            break;

        case EnumAbaTransferenciaPallet.Envio:
            adicionarEnvio();
            break;
        case EnumAbaTransferenciaPallet.Recebimento:
            adicionarRecebimento();
            break;
    }
}

function cancelarClick(e, sender) {
    if (_abaAtiva === EnumAbaTransferenciaPallet.Envio) {
        exibirConfirmacao("Confirmação", "Realmente deseja cancelar essa transferência de pallets?", function () {
            executarReST("Transferencia/CancelarPorCodigo", { Codigo: _transferenciaPallet.Codigo.val() }, function (arg) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");

                    _gridTransferenciaPallet.CarregarGrid();

                    limparCamposTransferenciaPallet();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, null);
        });
    }
}

function editarClick(transferenciaPalletSelecionada) {
    _pesquisaTransferenciaPallet.ExibirFiltros.visibleFade(false);

    editar(transferenciaPalletSelecionada);
}

function limparClick() {
    limparCamposTransferenciaPallet();
}

function reprocessarRegrasClick() {
    executarReST("Transferencia/ReprocessarRegras", { Codigo: _transferenciaPallet.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                buscarTransferenciaPalletPorCodigo(_transferenciaPallet.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Transferência de Pallets.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções
 */

function buscarDadosSetorUsuario(callback) {
    executarReST("Usuario/DadosSetorUsuarioLogado", undefined, function (retorno) {
        if (retorno.Data)
            PreencherObjetoKnout(_dadosSetorUsuario, retorno);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        if (callback)
            callback();
    }, null);
}

function buscarTransferenciaPalletPorCodigo(codigo) {
    executarReST("Transferencia/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            limparCamposTransferenciaPallet();
            preencherTransferencia(retorno.Data);
            setarEtapas();
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function editar(transferenciaPallet) {
    buscarTransferenciaPalletPorCodigo(transferenciaPallet.Codigo);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function controlarBotoesHabilitados() {
    var botaoAdicionarVisivel = false;
    var botaoCancelarVisivel = false;
    var botaoLimparVisivel = true;
    var botaoReprocessarRegrasVisivel = false;
    var textoBotaoAdicionar = "Adicionar";

    switch (_transferenciaPallet.Situacao.val()) {
        case EnumSituacaoTransferenciaPallet.AguardandoEnvio:
            if (_abaAtiva === EnumAbaTransferenciaPallet.Envio) {
                botaoAdicionarVisivel = true;
                botaoCancelarVisivel = true;
                textoBotaoAdicionar = "Enviar";
            }
            break;

        case EnumSituacaoTransferenciaPallet.AguardandoRecebimento:
            if (_abaAtiva === EnumAbaTransferenciaPallet.Recebimento) {
                botaoAdicionarVisivel = true;
                textoBotaoAdicionar = "Finalizar";
            }
            break;

        case EnumSituacaoTransferenciaPallet.Todas:
            textoBotaoAdicionar = "Solicitar";
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoTransferenciaPallet.SemRegraAprovacao:
            if (_abaAtiva === EnumAbaTransferenciaPallet.Aprovacao) {
                botaoReprocessarRegrasVisivel = true;
            }
            break;
    }

    _CRUDTransferenciaPallet.Adicionar.text(textoBotaoAdicionar);
    _CRUDTransferenciaPallet.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDTransferenciaPallet.Cancelar.visible(botaoCancelarVisivel);
    _CRUDTransferenciaPallet.Limpar.visible(botaoLimparVisivel);
    _CRUDTransferenciaPallet.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
}

function ControlarCamposHabilitados() {
    controlarCamposSolicitacaoHabilitados();
    controlarCamposEnvioHabilitados();
    controlarCamposRecebimentoHabilitados();
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function limparCamposTransferenciaPallet() {
    LimparCampos(_transferenciaPallet);
    limparResumo();
    limparCamposSolicitacao();
    limparCamposEnvio();
    limparCamposAprovacao();
    limparCamposRecebimento();
    limparCruzamentoInformacoes()
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

function preencherTransferencia(dados) {
    _transferenciaPallet.Codigo.val(dados.Codigo);
    _transferenciaPallet.Situacao.val(dados.Situacao);

    preencherResumo(dados.Resumo);
    preencherSolicitacao(dados.Solicitacao);
    preencherEnvio(dados.Envio);
    preencherAprovacao(dados.ResumoAprovacao, dados.Codigo, dados.Situacao);
    preencherRecebimento(dados.Recebimento);
}