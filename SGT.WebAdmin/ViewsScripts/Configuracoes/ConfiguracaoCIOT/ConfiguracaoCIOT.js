/// <reference path="../../Enumeradores/EnumOperadoraCIOT.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDConfiguracaoCIOT;
var _configuracaoCIOT;
var _pesquisaConfiguracaoCIOT;
var _gridConfiguracaoCIOT;

/*
 * Declaração das Classes
 */

var CRUDConfiguracaoCIOT = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var ConfiguracaoCIOT = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.OperadoraCIOT = PropertyEntity({ text: "*Operadora:", options: EnumOperadoraCIOT.ObterOpcoes(), val: ko.observable(EnumOperadoraCIOT.eFrete), def: EnumOperadoraCIOT.eFrete, issue: 0, visible: ko.observable(true) });
    this.TarifaSaque = PropertyEntity({ text: "Tarifa de Saque:", getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 6 });
    this.TarifaTransferencia = PropertyEntity({ text: "Tarifa de Transferência:", getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 6 });
    this.CNPJOperadora = PropertyEntity({ text: "CNPJ do Operador", val: ko.observable(true), getType: typesKnockout.cpfCnpj, enable: ko.observable(true) });

    this.AbrirCIOTAntesEmissaoCTe = PropertyEntity({ text: "Abrir o CIOT antes da emissão dos CT-es", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.ExigeRotaCadastrada = PropertyEntity({ text: "Exige que a rota esteja cadastrada ao emitir a carga", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarUmCIOTPorViagem = PropertyEntity({ text: "Gerar um CIOT para cada viagem", val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.IntegrarMotoristaNoCadastro = PropertyEntity({ text: "Integrar o motorista no cadastro", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.IntegrarVeiculoNoCadastro = PropertyEntity({ text: "Integrar o veículo no cadastro", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.ValorPedagioRetornadoIntegradora = PropertyEntity({ text: "O valor do pedágio é retornado pela integradora", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.ConsultarFaturas = PropertyEntity({ text: "Consultar faturas", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.EncerrarCIOTManualmente = PropertyEntity({ text: "Encerrar CIOT Manualmente", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.HabilitarConciliacaoFinanceira = PropertyEntity({ text: "Habilitar conciliação financeira (validar se a operadora possui conciliação implementada)", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarTitulosContratoFrete = PropertyEntity({ text: "Gerar títulos para o contrato de frete", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.PermiteVariosCIOTsAbertos = PropertyEntity({ text: "Permite mais de um CIOT aberto para o terceiro", val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.PermiteVariosCIOTsAbertosTipoTerceiro = PropertyEntity({ text: "Permite mais de um CIOT aberto para:", options: EnumTipoProprietarioVeiculo.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.UtilizarDataAtualComoInicioTerminoCIOT = PropertyEntity({ text: "Utilizar data atual para início e término do CIOT", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.DiasTerminoCIOT = PropertyEntity({ text: "Dias para o término do CIOT:", getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 3 });
    this.ConfiguracaoMovimentoFinanceiro = PropertyEntity({ text: "Configurar movimento financeiro pelo tipo de pagamento", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.HabilitarDataFixaVencimentoCIOT = PropertyEntity({ text: "Habilitar data fixa para Vencimento do Ciot", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.HabilitarQuitacaoAutomaticaPagamentosPendentes = PropertyEntity({ text: "Habilitar quitação automática dos pagamentos pendentes", val: ko.observable(false), def: false, enable: ko.observable(true) });
 
    this.ConfiguracaoEFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoRepom = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoPamcard = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoPagbem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoTarget = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoExtratta = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoBBC = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoFinanceira = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoDataFixaVencimentoCiot = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoAmbipar = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoRodocred = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoRepomFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ConfiguracaoTruckPad = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });


    this.OperadoraCIOT.val.subscribe(function (operadora) {
        if (operadora == EnumOperadoraCIOT.Ambipar) {
            _configuracaoAmbipar.Usuario.required(true);
            _configuracaoAmbipar.Senha.required(true);
            _configuracaoAmbipar.URL.required(true);
        } else {
            _configuracaoAmbipar.Usuario.required(false);
            _configuracaoAmbipar.Senha.required(false);
            _configuracaoAmbipar.URL.required(false);
        }
        OperadoraChange(operadora);
    });
};

var PesquisaConfiguracaoCIOT = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridConfiguracaoCIOT, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridConfiguracaoCIOT() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ConfiguracaoCIOT/ExportarPesquisa", titulo: "Configurações de CIOT" };

    _gridConfiguracaoCIOT = new GridViewExportacao(_pesquisaConfiguracaoCIOT.Pesquisar.idGrid, "ConfiguracaoCIOT/Pesquisa", _pesquisaConfiguracaoCIOT, menuOpcoes, configuracoesExportacao);
    _gridConfiguracaoCIOT.CarregarGrid();
}

function LoadConfiguracaoCIOT() {
    _configuracaoCIOT = new ConfiguracaoCIOT();
    KoBindings(_configuracaoCIOT, "tabConfiguracao");

    HeaderAuditoria("ConfiguracaoCIOT", _configuracaoCIOT);

    _CRUDConfiguracaoCIOT = new CRUDConfiguracaoCIOT();
    KoBindings(_CRUDConfiguracaoCIOT, "knockoutCRUDConfiguracaoCIOT");

    _pesquisaConfiguracaoCIOT = new PesquisaConfiguracaoCIOT();
    KoBindings(_pesquisaConfiguracaoCIOT, "knockoutPesquisaConfiguracaoCIOT", false, _pesquisaConfiguracaoCIOT.Pesquisar.id);

    _configuracaoCIOT.ConfiguracaoMovimentoFinanceiro.val.subscribe(ativarConfiguracaoFinanceira);
    _configuracaoCIOT.HabilitarDataFixaVencimentoCIOT.val.subscribe(ativarConfiguracaoDataFixaVencimento);

    LoadGridConfiguracaoCIOT();
    LoadConfiguracaoRepom();
    LoadConfiguracaoPamcard();
    LoadConfiguracaoEFrete();
    LoadConfiguracaoPagbem();
    LoadConfiguracaoTarget();
    LoadConfiguracaoExtratta();
    LoadConfiguracaoFinanceira();
    LoadConfiguracaoDataFixaVencimentoCiot();
    LoadConfiguracaoBBC();
    LoadConfiguracaoAmbipar();
    LoadConfiguracaoRodocred();
    LoadConfiguracaoRepomFrete();
    LoadConfiguracaoTruckPad();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function OperadoraChange(operadora) {

    Global.ResetarMultiplasAbas();

    $("#liTabPamcard").addClass("d-none");
    $("#liTabEFrete").addClass("d-none");
    $("#liTabRepom").addClass("d-none");
    $("#liTabPagbem").addClass("d-none");
    $("#liTabTarget").addClass("d-none");
    $("#liTabExtratta").addClass("d-none");
    $("#liTabAmbipar").addClass("d-none");
    $("#liTabRodocred").addClass("d-none");
    $("#liTabRepomFrete").addClass("d-none");
    $("#liTabTruckPad").addClass("d-none");

    switch (operadora) {
        case EnumOperadoraCIOT.eFrete:
            $("#liTabEFrete").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Pamcard:
            $("#liTabPamcard").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Repom:
            $("#liTabRepom").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Pagbem:
            $("#liTabPagbem").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Target:
            $("#liTabTarget").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Extratta:
            $("#liTabExtratta").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Ambipar:
            $("#liTabAmbipar").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.Rodocred:
            $("#liTabRodocred").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.RepomFrete:
            $("#liTabRepomFrete").removeClass("d-none");
            break;
        case EnumOperadoraCIOT.TruckPad:
            $("#liTabTruckPad").removeClass("d-none");
            break;
        default:
            break;
    }
}

function AdicionarClick(e, sender) {
    SetarConfiguracoesOperadorasCIOT();
    CarregarConfiguracaoFinanceira();
    CarregarDataFixaVencimentoCiot();
    var validarCamposAmbipar = ValidarCamposObrigatorios(_configuracaoAmbipar);

    if (validarCamposAmbipar) {
        Salvar(_configuracaoCIOT, "ConfiguracaoCIOT/Adicionar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                    RecarregarGridConfiguracaoCIOT();
                    LimparCamposConfiguracaoCIOT();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, sender);
    } else {
        ExibirCamposObrigatorios();
    }
}

function AtualizarClick(e, sender) {
    SetarConfiguracoesOperadorasCIOT();
    CarregarConfiguracaoFinanceira();
    CarregarDataFixaVencimentoCiot();
    var validarCamposAmbipar = ValidarCamposObrigatorios(_configuracaoAmbipar);

    if (validarCamposAmbipar) {
        Salvar(_configuracaoCIOT, "ConfiguracaoCIOT/Atualizar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                    RecarregarGridConfiguracaoCIOT();
                    LimparCamposConfiguracaoCIOT();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, sender);
    } else {
        ExibirCamposObrigatorios();
    }
}

function CancelarClick() {
    LimparCamposConfiguracaoCIOT();
}

function EditarClick(registroSelecionado) {
    LimparCamposConfiguracaoCIOT();

    _configuracaoCIOT.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_configuracaoCIOT, "ConfiguracaoCIOT/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoCIOT.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_configuracaoRepom, { Data: retorno.Data.ConfiguracaoRepom });
                PreencherObjetoKnout(_configuracaoEFrete, { Data: retorno.Data.ConfiguracaoEFrete });
                PreencherObjetoKnout(_configuracaoPamcard, { Data: retorno.Data.ConfiguracaoPamcard });
                PreencherObjetoKnout(_configuracaoPagbem, { Data: retorno.Data.ConfiguracaoPagbem });
                PreencherObjetoKnout(_configuracaoTarget, { Data: retorno.Data.ConfiguracaoTarget });
                PreencherObjetoKnout(_configuracaoExtratta, { Data: retorno.Data.ConfiguracaoExtratta });
                PreencherObjetoKnout(_configuracaoBBC, { Data: retorno.Data.ConfiguracaoBBC });
                PreencherObjetoKnout(_configuracaoAmbipar, { Data: retorno.Data.ConfiguracaoAmbipar });
                PreencherObjetoKnout(_configuracaoRodocred, { Data: retorno.Data.ConfiguracaoRodocred });
                PreencherObjetoKnout(_configuracaoRepomFrete, { Data: retorno.Data.ConfiguracaoRepomFrete });
                PreencherObjetoKnout(_configuracaoTruckPad, { Data: retorno.Data.ConfiguracaoTruckPad });

                RecarregarGridConfiguracaoFinanceira();
                CarregaGridDataFixaVencimentoCiot();

                OperadoraChange(retorno.Data.OperadoraCIOT);
                ControlarBotoesHabilitados(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_configuracaoCIOT, "ConfiguracaoCIOT/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridConfiguracaoCIOT();
                    LimparCamposConfiguracaoCIOT();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDConfiguracaoCIOT.Atualizar.visible(isEdicao);
    _CRUDConfiguracaoCIOT.Excluir.visible(isEdicao);
    _CRUDConfiguracaoCIOT.Cancelar.visible(isEdicao);
    _CRUDConfiguracaoCIOT.Adicionar.visible(!isEdicao);
}

function LimparCamposConfiguracaoCIOT() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_configuracaoCIOT);
    LimparCamposConfiguracaoEFrete();
    LimparCamposConfiguracaoPamcard();
    LimparCamposConfiguracaoRepom();
    LimparCamposConfiguracaoPagbem();
    LimparCamposConfiguracaoTarget();
    LimparCamposConfiguracaoExtratta();
    LimparCamposConfiguracaoBBC();
    LimparCamposConfiguracaoAmbipar();
    $("#liConfiguracaoFinanceira").hide();
    $("#liTabDataFixaVencimentoCiot").hide();
    LimparCamposConfiguracaoRodocred();
    LimparCamposConfiguracaoRepomFrete();
    LimparCamposConfiguracaoTruckPad();
    LimpaGridDataFixaVencimentoCiot();
    LimparCamposConfiguracaoFinanceira();

    Global.ResetarMultiplasAbas();
}

function RecarregarGridConfiguracaoCIOT() {
    _gridConfiguracaoCIOT.CarregarGrid();
}

function SetarConfiguracoesOperadorasCIOT() {
    _configuracaoCIOT.ConfiguracaoEFrete.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoEFrete)));
    _configuracaoCIOT.ConfiguracaoPamcard.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoPamcard)));
    _configuracaoCIOT.ConfiguracaoRepom.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoRepom)));
    _configuracaoCIOT.ConfiguracaoPagbem.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoPagbem)));
    _configuracaoCIOT.ConfiguracaoTarget.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoTarget)));
    _configuracaoCIOT.ConfiguracaoExtratta.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoExtratta)));
    _configuracaoCIOT.ConfiguracaoBBC.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoBBC)));
    _configuracaoCIOT.ConfiguracaoAmbipar.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoAmbipar)));
    _configuracaoCIOT.ConfiguracaoRodocred.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoRodocred)))
    _configuracaoCIOT.ConfiguracaoRepomFrete.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoRepomFrete)));
    _configuracaoCIOT.ConfiguracaoTruckPad.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoTruckPad)));
}

const ativarConfiguracaoFinanceira = (visible) => visible ? $("#liConfiguracaoFinanceira").show() : $("#liConfiguracaoFinanceira").hide();
const ativarConfiguracaoDataFixaVencimento = (visible) => visible ? $("#liTabDataFixaVencimentoCiot").show() : $("#liTabDataFixaVencimentoCiot").hide();