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
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoNaturezaOperacaoEscrituracao.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _htmlConfiguracaoNaturezaOperacaoEscrituracao;
var _gridConfiguracaoNaturezaOperacaoEscrituracao;
var _configuracaoNaturezaOperacaoEscrituracao;

var ConfiguracaoNaturezaOperacaoEscrituracao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NaturezaOperacao = PropertyEntity({ text: "*Natureza de Operação:", required: true, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoContaContabil = PropertyEntity({ val: ko.observable(EnumTipoContaContabil.FreteLiquido), options: EnumTipoContaContabil.obterOpcoes(), def: EnumTipoContaContabil.FreteLiquido, text: "Tipo da Conta Contábil: ", required: true, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoNaturezaOperacaoEscrituracaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoNaturezaOperacaoEscrituracaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoNaturezaOperacaoEscrituracaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoNaturezaOperacaoEscrituracaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******



function loadConfiguracaoNaturezaOperacaoEscrituracao(callback) {
    BuscarHTMLConfiguracaoNaturezaOperacaoEscrituracao(function () {
        $("#tabEscrituracao").html(_htmlConfiguracaoNaturezaOperacaoEscrituracao);
        _configuracaoNaturezaOperacaoEscrituracao = new ConfiguracaoNaturezaOperacaoEscrituracao()
        _configuracaoNaturezaOperacaoEscrituracao.Codigo.val(guid());
        KoBindings(_configuracaoNaturezaOperacaoEscrituracao, "knockoutCadastroConfiguracaoNaturezaOperacaoEscrituracao");

        new BuscarNaturezasOperacoes(_configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarConfiguracaoNaturezaOperacaoEscrituracaoClick }] };
        var header = [{ data: "Codigo", visible: false },
            { data: "NaturezaOperacao", title: "Natureza da Operação", width: "40%" },
            { data: "TipoContaContabil", title: "Tipo da Conta", width: "20%" },
        ];

        _gridConfiguracaoNaturezaOperacaoEscrituracao = new BasicDataTable(_configuracaoNaturezaOperacaoEscrituracao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.desc });
        recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#knockoutCadastroConfiguracaoNaturezaOperacaoEscrituracao").show();

        callback();
    });
}

function BuscarHTMLConfiguracaoNaturezaOperacaoEscrituracao(callback) {
    $.get("Content/Static/ConfiguracaoContabil/ConfiguracaoNaturezaOperacaoEscrituracao.html?dyn=" + guid(), function (data) {
        _htmlConfiguracaoNaturezaOperacaoEscrituracao = data;
        if (callback != null)
            callback();
    });
}
function DescricaoTipoConfiguracaoNaturezaOperacaoEscrituracao(tipo) {
    for (var i = 0; i < _tipoConfiguracaoNaturezaOperacaoEscrituracao.length; i++) {
        if (_tipoConfiguracaoNaturezaOperacaoEscrituracao[i].value == tipo)
            return _tipoConfiguracaoNaturezaOperacaoEscrituracao[i].text;
    }
}

function recarregarGridConfiguracaoNaturezaOperacaoEscrituracao() {
    //if (_permissao_acesso_configuracaoNaturezaOperacaoEscrituracaos == null || !_permissao_acesso_configuracaoNaturezaOperacaoEscrituracaos.Acesso) 
    //    return;

    var data = new Array();
    $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list, function (i, configuracaoNaturezaOperacaoEscrituracao) {
        var configuracaoNaturezaOperacaoEscrituracaoGrid = new Object();
        configuracaoNaturezaOperacaoEscrituracaoGrid.Codigo = configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.codEntity;
        configuracaoNaturezaOperacaoEscrituracaoGrid.NaturezaOperacao = configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.val;
        configuracaoNaturezaOperacaoEscrituracaoGrid.TipoContaContabil = EnumTipoContaContabil.obterDescricao(configuracaoNaturezaOperacaoEscrituracao.TipoContaContabil.val);
        data.push(configuracaoNaturezaOperacaoEscrituracaoGrid);
    });
    _gridConfiguracaoNaturezaOperacaoEscrituracao.CarregarGrid(data);
}


function editarConfiguracaoNaturezaOperacaoEscrituracaoClick(data) {
    var dadosEditar = null;
    $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list, function (i, configuracaoNaturezaOperacaoEscrituracao) {
        if (data.Codigo == configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.codEntity) {
            dadosEditar = _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list[i];
            return false;
        }
    });

    _configuracaoNaturezaOperacaoEscrituracao.Atualizar.visible(true);
    _configuracaoNaturezaOperacaoEscrituracao.Cancelar.visible(true);
    _configuracaoNaturezaOperacaoEscrituracao.Excluir.visible(true);
    _configuracaoNaturezaOperacaoEscrituracao.Adicionar.visible(false);
    PreencherEditarListEntity(_configuracaoNaturezaOperacaoEscrituracao, dadosEditar);
}


function adicionarConfiguracaoNaturezaOperacaoEscrituracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoNaturezaOperacaoEscrituracao);
    if (tudoCerto) {
        var existe = false;
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list, function (i, configuracaoNaturezaOperacaoEscrituracao) {
            if (configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.codEntity == _configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list.push(SalvarListEntity(_configuracaoNaturezaOperacaoEscrituracao));
            recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();
            $("#" + _configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Escrituração já configurada", "Já existe uma configuração para a conta contábil " + _configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.val() + ".");
        }
        limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }

}

function atualizarConfiguracaoNaturezaOperacaoEscrituracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_configuracaoNaturezaOperacaoEscrituracao);
    if (tudoCerto) {
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list, function (i, configuracaoNaturezaOperacaoEscrituracao) {
            if (configuracaoNaturezaOperacaoEscrituracao.Codigo.val == _configuracaoNaturezaOperacaoEscrituracao.Codigo.val()) {
                _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list[i] = SalvarListEntity(_configuracaoNaturezaOperacaoEscrituracao);
                //AtualizarListEntity(_configuracaoNaturezaOperacaoEscrituracao, configuracaoNaturezaOperacaoEscrituracao)
                return false;
            }
        });
        recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();
        limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirConfiguracaoNaturezaOperacaoEscrituracaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração para a natureza da operação " + data.NaturezaOperacao.val() + "?", function () {
        $.each(_configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list, function (i, configuracao) {
            if (_configuracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.codEntity() == configuracao.NaturezaOperacao.codEntity)
                _configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracaos.list.splice(i, 1);
        });
        limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
        recarregarGridConfiguracaoNaturezaOperacaoEscrituracao();
    });
}

function cancelarConfiguracaoNaturezaOperacaoEscrituracaoClick(e) {
    limparCamposConfiguracaoNaturezaOperacaoEscrituracao();
}

function limparCamposConfiguracaoNaturezaOperacaoEscrituracao() {
    _configuracaoNaturezaOperacaoEscrituracao.Codigo.val(guid());
    _configuracaoNaturezaOperacaoEscrituracao.Atualizar.visible(false);
    _configuracaoNaturezaOperacaoEscrituracao.Cancelar.visible(false);
    _configuracaoNaturezaOperacaoEscrituracao.Excluir.visible(false);
    _configuracaoNaturezaOperacaoEscrituracao.Adicionar.visible(true);
    LimparCampos(_configuracaoNaturezaOperacaoEscrituracao);
}
