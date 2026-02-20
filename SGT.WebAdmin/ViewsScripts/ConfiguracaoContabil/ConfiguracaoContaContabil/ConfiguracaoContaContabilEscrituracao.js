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
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoContaContabilEscrituracao.js" />
/// <reference path="../../Enumeradores/EnumTipoContabilizacao.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoContaContabilEscrituracao;
var _gridConfiguracaoContaContabilEscrituracao;
var _configuracaoContaContabilEscrituracao;

var ConfiguracaoContaContabilEscrituracao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PlanoConta = PropertyEntity({ text: "*Conta Contábil:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoContabilizacao = PropertyEntity({ val: ko.observable(EnumTipoContabilizacao.Credito), options: EnumTipoContabilizacao.obterOpcoesPesquisa(), def: EnumTipoContabilizacao.Credito, text: "Tipo de Contabilização: ", required: true, visible: ko.observable(true) });
    this.TipoContaContabil = PropertyEntity({ val: ko.observable(EnumTipoContaContabil.FreteLiquido), options: EnumTipoContaContabil.obterOpcoes(), def: EnumTipoContaContabil.FreteLiquido, text: "Tipo da Conta Contábil: ", required: true, visible: ko.observable(true) });
    this.SempreGerarRegistro = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Sempre gerar o registro para documentos que podem conter o tipo do imposto?", def: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoContaContabilEscrituracaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoContaContabilEscrituracaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoContaContabilEscrituracaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoContaContabilEscrituracaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******



function loadConfiguracaoContaContabilEscrituracao(callback) {
    BuscarHTMLConfiguracaoContaContabilEscrituracao(function () {
        $("#tabEscrituracao").html(_htmlConfiguracaoContaContabilEscrituracao);
        _configuracaoContaContabilEscrituracao = new ConfiguracaoContaContabilEscrituracao()
        _configuracaoContaContabilEscrituracao.Codigo.val(guid());
        KoBindings(_configuracaoContaContabilEscrituracao, "knockoutCadastroConfiguracaoContaContabilEscrituracao");

        new BuscarPlanoConta(_configuracaoContaContabilEscrituracao.PlanoConta);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarConfiguracaoContaContabilEscrituracaoClick }] };
        var header = [{ data: "Codigo", visible: false },
            { data: "CodTipoContaContabil", visible: false },
            { data: "SempreGerarRegistro", visible: false },
            { data: "PlanoConta", title: "Conta Contábil", width: "40%" },
            { data: "TipoContaContabil", title: "Tipo da Conta", width: "20%" },
            { data: "TipoContabilizacao", title: "Tipo de Contabilização", width: "20%" }
        ];

        _gridConfiguracaoContaContabilEscrituracao = new BasicDataTable(_configuracaoContaContabilEscrituracao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoContaContabilEscrituracao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#knockoutCadastroConfiguracaoContaContabilEscrituracao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoContaContabilEscrituracao(callback) {
    $.get("Content/Static/ConfiguracaoContabil/ConfiguracaoContaContabilEscrituracao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoContaContabilEscrituracao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoContaContabilEscrituracao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoContaContabilEscrituracao.length; i++) {
        if (_tipoConfiguracaoContaContabilEscrituracao[i].value == tipo)
            return _tipoConfiguracaoContaContabilEscrituracao[i].text;
    }
}

function recarregarGridConfiguracaoContaContabilEscrituracao() {
    //if (_permissao_acesso_configuracaoContaContabilEscrituracaos == null || !_permissao_acesso_configuracaoContaContabilEscrituracaos.Acesso) 
    //    return;

    var data = new Array();
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list, function (i, configuracaoContaContabilEscrituracao) {
        var configuracaoContaContabilEscrituracaoGrid = new Object();
        configuracaoContaContabilEscrituracaoGrid.Codigo = configuracaoContaContabilEscrituracao.PlanoConta.codEntity;
        configuracaoContaContabilEscrituracaoGrid.PlanoConta = configuracaoContaContabilEscrituracao.PlanoConta.val;
        configuracaoContaContabilEscrituracaoGrid.TipoContaContabil = EnumTipoContaContabil.obterDescricao(configuracaoContaContabilEscrituracao.TipoContaContabil.val);
        configuracaoContaContabilEscrituracaoGrid.CodTipoContaContabil = configuracaoContaContabilEscrituracao.TipoContaContabil.val;
        configuracaoContaContabilEscrituracaoGrid.SempreGerarRegistro = configuracaoContaContabilEscrituracao.SempreGerarRegistro.val;
        configuracaoContaContabilEscrituracaoGrid.TipoContabilizacao = EnumTipoContabilizacao.obterDescricao(configuracaoContaContabilEscrituracao.TipoContabilizacao.val);
        data.push(configuracaoContaContabilEscrituracaoGrid);
    });
    _gridConfiguracaoContaContabilEscrituracao.CarregarGrid(data);
}


function editarConfiguracaoContaContabilEscrituracaoClick(data) {
    var dadosEditar = null;
    $.each(_configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list, function (i, configuracaoContaContabilEscrituracao) {
        if (data.Codigo == configuracaoContaContabilEscrituracao.PlanoConta.codEntity && configuracaoContaContabilEscrituracao.TipoContaContabil.val == data.CodTipoContaContabil) {
            dadosEditar = _configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list[i];
            return false;
        }
    });

    _configuracaoContaContabilEscrituracao.Atualizar.visible(true);
    _configuracaoContaContabilEscrituracao.Cancelar.visible(true);
    _configuracaoContaContabilEscrituracao.Excluir.visible(true);
    _configuracaoContaContabilEscrituracao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoContaContabilEscrituracao, dadosEditar);
}


function adicionarConfiguracaoContaContabilEscrituracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilEscrituracao);
    if (tudoCerto) {
        var existe = false;
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list, function (i, configuracaoContaContabilEscrituracao) {
            if (configuracaoContaContabilEscrituracao.PlanoConta.codEntity == _configuracaoContaContabilEscrituracao.PlanoConta.codEntity() && configuracaoContaContabilEscrituracao.TipoContaContabil.val == _configuracaoContaContabilEscrituracao.TipoContaContabil.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list.push(SalvarListEntity(_configuracaoContaContabilEscrituracao));
            recarregarGridConfiguracaoContaContabilEscrituracao();
            $("#" + _configuracaoContaContabilEscrituracao.PlanoConta.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Escrituração já configurada", "Já existe uma configuração para a conta contábil " + _configuracaoContaContabilEscrituracao.PlanoConta.val() + ".");
        }
        limparCamposConfiguracaoContaContabilEscrituracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }

}

function atualizarConfiguracaoContaContabilEscrituracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoContaContabilEscrituracao);
    if (tudoCerto) {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list, function (i, configuracaoContaContabilEscrituracao) {
            if (configuracaoContaContabilEscrituracao.Codigo.val == _configuracaoContaContabilEscrituracao.Codigo.val()) {
                _configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list[i] = SalvarListEntity(_configuracaoContaContabilEscrituracao);
                //AtualizarListEntity(_configuracaoContaContabilEscrituracao, configuracaoContaContabilEscrituracao)
                return false;
            }
        });
        recarregarGridConfiguracaoContaContabilEscrituracao();
        limparCamposConfiguracaoContaContabilEscrituracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirConfiguracaoContaContabilEscrituracaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração para a conta contábil " + data.PlanoConta.val() + "?", function () {
        $.each(_configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list, function (i, configuracao) {
            if (_configuracaoContaContabilEscrituracao.PlanoConta.codEntity() == configuracao.PlanoConta.codEntity && configuracao.TipoContaContabil.val == _configuracaoContaContabilEscrituracao.TipoContaContabil.val())
                _configuracaoContaContabil.ConfiguracaoContaContabilEscrituracaos.list.splice(i, 1);
        });
        limparCamposConfiguracaoContaContabilEscrituracao();
        recarregarGridConfiguracaoContaContabilEscrituracao();
    });
}

function cancelarConfiguracaoContaContabilEscrituracaoClick(e) {
    limparCamposConfiguracaoContaContabilEscrituracao();
}

function limparCamposConfiguracaoContaContabilEscrituracao() {
    _configuracaoContaContabilEscrituracao.Codigo.val(guid());
    _configuracaoContaContabilEscrituracao.Atualizar.visible(false);
    _configuracaoContaContabilEscrituracao.Cancelar.visible(false);
    _configuracaoContaContabilEscrituracao.Excluir.visible(false);
    _configuracaoContaContabilEscrituracao.Adicionar.visible(true);
    LimparCampos(_configuracaoContaContabilEscrituracao);
}
