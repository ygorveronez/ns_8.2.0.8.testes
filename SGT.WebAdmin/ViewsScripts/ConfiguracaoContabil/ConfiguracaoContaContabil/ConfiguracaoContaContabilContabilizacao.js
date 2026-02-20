/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ModelosVeicularesCargas.js" />
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoContaContabilContabilizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoContabilizacao.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoContaContabilContabilizacao;
var _gridConfiguracaoContaContabilContabilizacao;
var _configuracaoContaContabilContabilizacao;

var ConfiguracaoContaContabilContabilizacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PlanoConta = PropertyEntity({ text: "*Conta Contábil:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoContaContraPartidaProvisao = PropertyEntity({ text: "Contrapartida Provisão:", required: false, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoContabilizacao = PropertyEntity({ val: ko.observable(EnumTipoContabilizacao.Credito), options: EnumTipoContabilizacao.obterOpcoesPesquisa(), def: EnumTipoContabilizacao.Credito, text: "Tipo de Contabilização: ", required: true, visible: ko.observable(true) });
    this.TipoContaContabil = PropertyEntity({ val: ko.observable(EnumTipoContaContabil.FreteLiquido), options: EnumTipoContaContabil.obterOpcoes(), def: EnumTipoContaContabil.FreteLiquido, text: "Tipo da Conta Contábil: ", required: true, visible: ko.observable(true) });
    this.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao = PropertyEntity({ text: "Componentes de frete do tipo desconto não devem somar na contabilização", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoContaContabilContabilizacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoContaContabilContabilizacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoContaContabilContabilizacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoContaContabilContabilizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoContaContabilContabilizacao(callback) {
    BuscarHTMLConfiguracaoContaContabilContabilizacao(function () {
        $("#tabContabilizacao").html(_htmlConfiguracaoContaContabilContabilizacao);
        _configuracaoContaContabilContabilizacao = new ConfiguracaoContaContabilContabilizacao()
        _configuracaoContaContabilContabilizacao.Codigo.val(guid());
        KoBindings(_configuracaoContaContabilContabilizacao, "knockoutCadastroConfiguracaoContaContabilContabilizacao");

        new BuscarPlanoConta(_configuracaoContaContabilContabilizacao.PlanoConta);
        new BuscarPlanoConta(_configuracaoContaContabilContabilizacao.PlanoContaContraPartidaProvisao);
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarConfiguracaoContaContabilContabilizacaoClick }] };
        var header = [
            { data: "Codigo", visible: false },
            { data: "CodTipoContaContabil", visible: false },
            { data: "PlanoConta", title: "Conta Contábil", width: "30%" },
            { data: "PlanoContaContraPartidaProvisao", title: "Contrapartida Provisão", width: "30%" },
            { data: "TipoContaContabil", title: "Tipo da Conta", width: "15%" },
            { data: "TipoContabilizacao", title: "Tipo de Contabilização", width: "15%" }
        ];

        _gridConfiguracaoContaContabilContabilizacao = new BasicDataTable(_configuracaoContaContabilContabilizacao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoContaContabilContabilizacao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#knockoutCadastroConfiguracaoContaContabilContabilizacao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoContaContabilContabilizacao(callback) {
    $.get("Content/Static/ConfiguracaoContabil/ConfiguracaoContaContabilContabilizacao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoContaContabilContabilizacao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoContaContabilContabilizacao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoContaContabilContabilizacao.length; i++) {
        if (_tipoConfiguracaoContaContabilContabilizacao[i].value == tipo)
            return _tipoConfiguracaoContaContabilContabilizacao[i].text;
    }
}

function recarregarGridConfiguracaoContaContabilContabilizacao() {
    //if (_permissao_acesso_configuracaoContaContabilContabilizacaos == null || !_permissao_acesso_configuracaoContaContabilContabilizacaos.Acesso) 
    //    return;

    var data = new Array();
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list, function (i, configuracaoContaContabilContabilizacao) {
        var configuracaoContaContabilContabilizacaoGrid = new Object();
        configuracaoContaContabilContabilizacaoGrid.Codigo = configuracaoContaContabilContabilizacao.PlanoConta.codEntity;
        configuracaoContaContabilContabilizacaoGrid.PlanoConta = configuracaoContaContabilContabilizacao.PlanoConta.val;
        configuracaoContaContabilContabilizacaoGrid.PlanoContaContraPartidaProvisao = configuracaoContaContabilContabilizacao.PlanoContaContraPartidaProvisao.val;
        configuracaoContaContabilContabilizacaoGrid.TipoContaContabil = EnumTipoContaContabil.obterDescricao(configuracaoContaContabilContabilizacao.TipoContaContabil.val);
        configuracaoContaContabilContabilizacaoGrid.CodTipoContaContabil = configuracaoContaContabilContabilizacao.TipoContaContabil.val;
        configuracaoContaContabilContabilizacaoGrid.TipoContabilizacao = EnumTipoContabilizacao.obterDescricao(configuracaoContaContabilContabilizacao.TipoContabilizacao.val);
        configuracaoContaContabilContabilizacaoGrid.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao = configuracaoContaContabilContabilizacao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao.val;
        data.push(configuracaoContaContabilContabilizacaoGrid);
    });
    _gridConfiguracaoContaContabilContabilizacao.CarregarGrid(data);
}


function editarConfiguracaoContaContabilContabilizacaoClick(data) {
    var dadosEditar = null;
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list, function (i, configuracaoContaContabilContabilizacao) {
        if (data.Codigo == configuracaoContaContabilContabilizacao.PlanoConta.codEntity && configuracaoContaContabilContabilizacao.TipoContaContabil.val == data.CodTipoContaContabil) {
            dadosEditar = _configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list[i];
            return false;
        }
    });

    _configuracaoContaContabilContabilizacao.Atualizar.visible(true);
    _configuracaoContaContabilContabilizacao.Cancelar.visible(true);
    _configuracaoContaContabilContabilizacao.Excluir.visible(true);
    _configuracaoContaContabilContabilizacao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoContaContabilContabilizacao, dadosEditar);
}


function adicionarConfiguracaoContaContabilContabilizacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilContabilizacao);
    if (tudoCerto) {
        var existe = false;
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list, function (i, configuracaoContaContabilContabilizacao) {
            if (configuracaoContaContabilContabilizacao.PlanoConta.codEntity == _configuracaoContaContabilContabilizacao.PlanoConta.codEntity() && configuracaoContaContabilContabilizacao.TipoContaContabil.val == _configuracaoContaContabilContabilizacao.TipoContaContabil.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list.push(SalvarListEntity(_configuracaoContaContabilContabilizacao));
            recarregarGridConfiguracaoContaContabilContabilizacao();
            $("#" + _configuracaoContaContabilContabilizacao.PlanoConta.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Contabilização já configurada", "Já existe uma configuração para a conta contábil " + _configuracaoContaContabilContabilizacao.PlanoConta.val() + ".");
        }
        limparCamposConfiguracaoContaContabilContabilizacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }

}

function atualizarConfiguracaoContaContabilContabilizacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilContabilizacao);
    if (tudoCerto) {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list, function (i, configuracaoContaContabilContabilizacao) {
            if (configuracaoContaContabilContabilizacao.Codigo.val == _configuracaoContaContabilContabilizacao.Codigo.val()) {
                _configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list[i] = SalvarListEntity(_configuracaoContaContabilContabilizacao);
                //AtualizarListEntity(_configuracaoContaContabilContabilizacao, configuracaoContaContabilContabilizacao)
                return false;
            }
        });
        recarregarGridConfiguracaoContaContabilContabilizacao();
        limparCamposConfiguracaoContaContabilContabilizacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirConfiguracaoContaContabilContabilizacaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração para a conta contábil " + data.PlanoConta.val() + "?", function () {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list, function (i, configuracao) {
            if (_configuracaoContaContabilContabilizacao.PlanoConta.codEntity() == configuracao.PlanoConta.codEntity && configuracao.TipoContaContabil.val == _configuracaoContaContabilContabilizacao.TipoContaContabil.val())
                _configuracaoContaContabil.ConfiguracaoContaContabilContabilizacaos.list.splice(i, 1);
        });
        limparCamposConfiguracaoContaContabilContabilizacao();
        recarregarGridConfiguracaoContaContabilContabilizacao();
    });
}

function cancelarConfiguracaoContaContabilContabilizacaoClick(e) {
    limparCamposConfiguracaoContaContabilContabilizacao();
}

function limparCamposConfiguracaoContaContabilContabilizacao() {
    _configuracaoContaContabilContabilizacao.Codigo.val(guid());
    _configuracaoContaContabilContabilizacao.Atualizar.visible(false);
    _configuracaoContaContabilContabilizacao.Cancelar.visible(false);
    _configuracaoContaContabilContabilizacao.Excluir.visible(false);
    _configuracaoContaContabilContabilizacao.Adicionar.visible(true);
    LimparCampos(_configuracaoContaContabilContabilizacao);
}
