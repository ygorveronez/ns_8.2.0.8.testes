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
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoContaContabilProvisao.js" />
/// <reference path="../../Enumeradores/EnumTipoContabilizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoContaContabil.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoContaContabilProvisao;
var _gridConfiguracaoContaContabilProvisao;
var _configuracaoContaContabilProvisao;

var ConfiguracaoContaContabilProvisao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PlanoConta = PropertyEntity({ text: "*Conta Contábil:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoContabilizacao = PropertyEntity({ val: ko.observable(EnumTipoContabilizacao.Credito), options: EnumTipoContabilizacao.obterOpcoesPesquisa(), def: EnumTipoContabilizacao.Credito, text: "Tipo de Contabilização: ", required: true, visible: ko.observable(true) });
    this.TipoContaContabil = PropertyEntity({ val: ko.observable(EnumTipoContaContabil.FreteLiquido), options: ko.observable(EnumTipoContaContabil.obterOpcoes()), def: EnumTipoContaContabil.FreteLiquido, text: "Tipo da Conta Contábil: ", required: true, visible: ko.observable(true) });
    this.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao = PropertyEntity({ text: "Componentes de frete do tipo desconto não devem somar na provisão", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoContaContabilProvisaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoContaContabilProvisaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoContaContabilProvisaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoContaContabilProvisaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******



function loadConfiguracaoContaContabilProvisao(callback) {
    BuscarHTMLConfiguracaoContaContabilProvisao(function () {
        $("#tabProvisao").html(_htmlConfiguracaoContaContabilProvisao);
        _configuracaoContaContabilProvisao = new ConfiguracaoContaContabilProvisao();
        _configuracaoContaContabilProvisao.Codigo.val(guid());
        KoBindings(_configuracaoContaContabilProvisao, "knockoutCadastroConfiguracaoContaContabilProvisao");

        new BuscarPlanoConta(_configuracaoContaContabilProvisao.PlanoConta);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarConfiguracaoContaContabilProvisaoClick }] };
        var header = [
            { data: "Codigo", visible: false },
            { data: "CodTipoContaContabil", visible: false },
            { data: "PlanoConta", title: "Conta Contábil", width: "40%" },
            { data: "TipoContaContabil", title: "Tipo da Conta", width: "20%" },
            { data: "TipoContabilizacao", title: "Tipo de Contabilização", width: "20%" }
        ];

        _gridConfiguracaoContaContabilProvisao = new BasicDataTable(_configuracaoContaContabilProvisao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoContaContabilProvisao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#knockoutCadastroConfiguracaoContaContabilProvisao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoContaContabilProvisao(callback) {
    $.get("Content/Static/ConfiguracaoContabil/ConfiguracaoContaContabilProvisao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoContaContabilProvisao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoContaContabilProvisao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoContaContabilProvisao.length; i++) {
        if (_tipoConfiguracaoContaContabilProvisao[i].value == tipo)
            return _tipoConfiguracaoContaContabilProvisao[i].text;
    }
}

function recarregarGridConfiguracaoContaContabilProvisao() {
    //if (_permissao_acesso_ConfiguracaoContaContabilProvisoes == null || !_permissao_acesso_ConfiguracaoContaContabilProvisoes.Acesso) 
    //    return;

    var data = new Array();
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list, function (i, configuracaoContaContabilProvisao) {
        var configuracaoContaContabilProvisaoGrid = new Object();
        configuracaoContaContabilProvisaoGrid.Codigo = configuracaoContaContabilProvisao.PlanoConta.codEntity;
        configuracaoContaContabilProvisaoGrid.PlanoConta = configuracaoContaContabilProvisao.PlanoConta.val;
        configuracaoContaContabilProvisaoGrid.TipoContaContabil = EnumTipoContaContabil.obterDescricao(configuracaoContaContabilProvisao.TipoContaContabil.val);
        configuracaoContaContabilProvisaoGrid.CodTipoContaContabil = configuracaoContaContabilProvisao.TipoContaContabil.val;
        configuracaoContaContabilProvisaoGrid.TipoContabilizacao = EnumTipoContabilizacao.obterDescricao(configuracaoContaContabilProvisao.TipoContabilizacao.val);
        configuracaoContaContabilProvisaoGrid.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao = configuracaoContaContabilProvisao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao.val;
        data.push(configuracaoContaContabilProvisaoGrid);
    });
    _gridConfiguracaoContaContabilProvisao.CarregarGrid(data);
}


function editarConfiguracaoContaContabilProvisaoClick(data) {
    var dadosEditar = null;
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list, function (i, configuracaoContaContabilProvisao) {
        if (data.Codigo == configuracaoContaContabilProvisao.PlanoConta.codEntity && configuracaoContaContabilProvisao.TipoContaContabil.val == data.CodTipoContaContabil) {
            dadosEditar = _configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list[i];
            return false;
        }
    });

    _configuracaoContaContabilProvisao.Atualizar.visible(true);
    _configuracaoContaContabilProvisao.Cancelar.visible(true);
    _configuracaoContaContabilProvisao.Excluir.visible(true);
    _configuracaoContaContabilProvisao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoContaContabilProvisao, dadosEditar);
}


function adicionarConfiguracaoContaContabilProvisaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilProvisao);
    if (tudoCerto) {
        var existe = false;
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list, function (i, configuracaoContaContabilProvisao) {
            if (configuracaoContaContabilProvisao.PlanoConta.codEntity == _configuracaoContaContabilProvisao.PlanoConta.codEntity() && configuracaoContaContabilProvisao.TipoContaContabil.val == _configuracaoContaContabilProvisao.TipoContaContabil.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list.push(SalvarListEntity(_configuracaoContaContabilProvisao));
            recarregarGridConfiguracaoContaContabilProvisao();
            $("#" + _configuracaoContaContabilProvisao.PlanoConta.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Escrituração já configurada", "Já existe uma configuração para a conta contábil " + _configuracaoContaContabilProvisao.PlanoConta.val() + ".");
        }
        limparCamposConfiguracaoContaContabilProvisao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }

}

function atualizarConfiguracaoContaContabilProvisaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilProvisao);
    if (tudoCerto) {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list, function (i, configuracaoContaContabilProvisao) {
            if (configuracaoContaContabilProvisao.Codigo.val == _configuracaoContaContabilProvisao.Codigo.val()) {
                _configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list[i] = SalvarListEntity(_configuracaoContaContabilProvisao);
                //AtualizarListEntity(_configuracaoContaContabilProvisao, configuracaoContaContabilProvisao)
                return false;
            }
        });
        recarregarGridConfiguracaoContaContabilProvisao();
        limparCamposConfiguracaoContaContabilProvisao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirConfiguracaoContaContabilProvisaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração para a conta contábil " + data.PlanoConta.val() + "?", function () {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list, function (i, configuracao) {
            if (_configuracaoContaContabilProvisao.PlanoConta.codEntity() == configuracao.PlanoConta.codEntity && configuracao.TipoContaContabil.val == _configuracaoContaContabilProvisao.TipoContaContabil.val())
                _configuracaoContaContabil.ConfiguracaoContaContabilProvisoes.list.splice(i, 1);
        });
        limparCamposConfiguracaoContaContabilProvisao();
        recarregarGridConfiguracaoContaContabilProvisao();
    });
}

function cancelarConfiguracaoContaContabilProvisaoClick(e) {
    limparCamposConfiguracaoContaContabilProvisao();
}

function limparCamposConfiguracaoContaContabilProvisao() {
    _configuracaoContaContabilProvisao.Codigo.val(guid());
    _configuracaoContaContabilProvisao.Atualizar.visible(false);
    _configuracaoContaContabilProvisao.Cancelar.visible(false);
    _configuracaoContaContabilProvisao.Excluir.visible(false);
    _configuracaoContaContabilProvisao.Adicionar.visible(true);
    LimparCampos(_configuracaoContaContabilProvisao);
}
