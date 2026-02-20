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
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoNaturezaOperacaoContabilizacao.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoNaturezaOperacaoContabilizacao;
var _gridConfiguracaoNaturezaOperacaoContabilizacao;
var _configuracaoNaturezaOperacaoContabilizacao;

var ConfiguracaoNaturezaOperacaoContabilizacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NaturezaOperacao = PropertyEntity({ text: "*Natureza da Operação:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoContaContabil = PropertyEntity({ val: ko.observable(EnumTipoContaContabil.FreteLiquido), options: EnumTipoContaContabil.obterOpcoes(), def: EnumTipoContaContabil.FreteLiquido, text: "Tipo da Conta Contábil: ", required: true, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoNaturezaOperacaoContabilizacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoNaturezaOperacaoContabilizacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoNaturezaOperacaoContabilizacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoNaturezaOperacaoContabilizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoNaturezaOperacaoContabilizacao(callback) {
    BuscarHTMLConfiguracaoNaturezaOperacaoContabilizacao(function () {
        $("#tabContabilizacao").html(_htmlConfiguracaoNaturezaOperacaoContabilizacao);
        _configuracaoNaturezaOperacaoContabilizacao = new ConfiguracaoNaturezaOperacaoContabilizacao()
        _configuracaoNaturezaOperacaoContabilizacao.Codigo.val(guid());
        KoBindings(_configuracaoNaturezaOperacaoContabilizacao, "knockoutCadastroConfiguracaoNaturezaOperacaoContabilizacao");

        new BuscarNaturezasOperacoes(_configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarConfiguracaoNaturezaOperacaoContabilizacaoClick }] };
        var header = [{ data: "Codigo", visible: false },
            { data: "NaturezaOperacao", title: "Natureza da Operação", width: "40%" },
            { data: "TipoContaContabil", title: "Tipo da Conta", width: "20%" }
        ];

        _gridConfiguracaoNaturezaOperacaoContabilizacao = new BasicDataTable(_configuracaoNaturezaOperacaoContabilizacao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#knockoutCadastroConfiguracaoNaturezaOperacaoContabilizacao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoNaturezaOperacaoContabilizacao(callback) {
    $.get("Content/Static/ConfiguracaoContabil/ConfiguracaoNaturezaOperacaoContabilizacao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoNaturezaOperacaoContabilizacao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoNaturezaOperacaoContabilizacao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoNaturezaOperacaoContabilizacao.length; i++) {
        if (_tipoConfiguracaoNaturezaOperacaoContabilizacao[i].value == tipo)
            return _tipoConfiguracaoNaturezaOperacaoContabilizacao[i].text;
    }
}

function recarregarGridConfiguracaoNaturezaOperacaoContabilizacao() {
    //if (_permissao_acesso_configuracaoNaturezaOperacaoContabilizacaos == null || !_permissao_acesso_configuracaoNaturezaOperacaoContabilizacaos.Acesso) 
    //    return;

    var data = new Array();
    $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list, function (i, configuracaoNaturezaOperacaoContabilizacao) {
        var configuracaoNaturezaOperacaoContabilizacaoGrid = new Object();
        configuracaoNaturezaOperacaoContabilizacaoGrid.Codigo = configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.codEntity;
        configuracaoNaturezaOperacaoContabilizacaoGrid.NaturezaOperacao = configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.val;
        configuracaoNaturezaOperacaoContabilizacaoGrid.TipoContaContabil = EnumTipoContaContabil.obterDescricao(configuracaoNaturezaOperacaoContabilizacao.TipoContaContabil.val);
        data.push(configuracaoNaturezaOperacaoContabilizacaoGrid);
    });
    _gridConfiguracaoNaturezaOperacaoContabilizacao.CarregarGrid(data);
}


function editarConfiguracaoNaturezaOperacaoContabilizacaoClick(data) {
    var dadosEditar = null;
    $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list, function (i, configuracaoNaturezaOperacaoContabilizacao) {
        if (data.Codigo == configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.codEntity) {
            dadosEditar = _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list[i];
            return false;
        }
    });

    _configuracaoNaturezaOperacaoContabilizacao.Atualizar.visible(true);
    _configuracaoNaturezaOperacaoContabilizacao.Cancelar.visible(true);
    _configuracaoNaturezaOperacaoContabilizacao.Excluir.visible(true);
    _configuracaoNaturezaOperacaoContabilizacao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoNaturezaOperacaoContabilizacao, dadosEditar);
}


function adicionarConfiguracaoNaturezaOperacaoContabilizacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoNaturezaOperacaoContabilizacao);
    if (tudoCerto) {
        var existe = false;
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list, function (i, configuracaoNaturezaOperacaoContabilizacao) {
            if (configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.codEntity == _configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list.push(SalvarListEntity(_configuracaoNaturezaOperacaoContabilizacao));
            recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();
            $("#" + _configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Contabilização já configurada", "Já existe uma configuração para a conta contábil " + _configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.val() + ".");
        }
        limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }

}

function atualizarConfiguracaoNaturezaOperacaoContabilizacaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoNaturezaOperacaoContabilizacao);
    if (tudoCerto) {
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list, function (i, configuracaoNaturezaOperacaoContabilizacao) {
            if (configuracaoNaturezaOperacaoContabilizacao.Codigo.val == _configuracaoNaturezaOperacaoContabilizacao.Codigo.val()) {
                _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list[i] = SalvarListEntity(_configuracaoNaturezaOperacaoContabilizacao);
                //AtualizarListEntity(_configuracaoNaturezaOperacaoContabilizacao, configuracaoNaturezaOperacaoContabilizacao)
                return false;
            }
        });
        recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();
        limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirConfiguracaoNaturezaOperacaoContabilizacaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração para a conta contábil " + data.NaturezaOperacao.val() + "?", function () {
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list, function (i, configuracao) {
            if (_configuracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.codEntity() == configuracao.NaturezaOperacao.codEntity)
                _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacaos.list.splice(i, 1);
        });
        limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
        recarregarGridConfiguracaoNaturezaOperacaoContabilizacao();
    });
}

function cancelarConfiguracaoNaturezaOperacaoContabilizacaoClick(e) {
    limparCamposConfiguracaoNaturezaOperacaoContabilizacao();
}

function limparCamposConfiguracaoNaturezaOperacaoContabilizacao() {
    _configuracaoNaturezaOperacaoContabilizacao.Codigo.val(guid());
    _configuracaoNaturezaOperacaoContabilizacao.Atualizar.visible(false);
    _configuracaoNaturezaOperacaoContabilizacao.Cancelar.visible(false);
    _configuracaoNaturezaOperacaoContabilizacao.Excluir.visible(false);
    _configuracaoNaturezaOperacaoContabilizacao.Adicionar.visible(true);
    LimparCampos(_configuracaoNaturezaOperacaoContabilizacao);
}
