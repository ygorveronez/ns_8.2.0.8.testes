/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Enumeradores/EnumCategoriaOpcaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumAplicacaoOpcaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoCheckListGuarita.js" />
/// <reference path="../../Enumeradores/EnumCheckListNaturalOne.js" />
/// <reference path="../../Consultas/CheckListTipo.js" />
/// <reference path="../../Enumeradores/EnumCheckListResposta.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Opcoes.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _checkListOpcoes;
var _pesquisaCheckListOpcoes;
var _gridCheckListOpcoes;
var tagsDescricao = [{ texto: "Modelo Veicular Carga", valor: "#ModeloVeicularCarga", id: guid() }, { texto: "N° Eixos Mod. Veicular", valor: "#NumeroEixosModeloVeicularCarga", id: guid() }];

var _situacaoAplicacaoPesquisa = [
    { text: "Todas", value: '' },
    { text: "Sempre", value: EnumAplicacaoOpcaoCheckList.Sempre },
    { text: "Carregamento", value: EnumAplicacaoOpcaoCheckList.Carregamento },
    { text: "Descarregamento", value: EnumAplicacaoOpcaoCheckList.Descarregamento }
];

//var _tipoCheckListGuarita = [
//    { text: "Todas", value: EnumTipoCheckListGuarita.Todos },
//    { text: "Manutenção", value: EnumTipoCheckListGuarita.Manutencao },
//    { text: "Rastreamento", value: EnumTipoCheckListGuarita.Rastreamento }
//];

var _situacaoAplicacao = [
    { text: "Sempre", value: EnumAplicacaoOpcaoCheckList.Sempre },
    { text: "Carregamento", value: EnumAplicacaoOpcaoCheckList.Carregamento },
    { text: "Descarregamento", value: EnumAplicacaoOpcaoCheckList.Descarregamento }
];

var _respostasImpeditivas = [
    { text: "Nenhuma", value: 0 },
    { text: "Sim", value: EnumCheckListResposta.Aprovada },
    { text: "Não", value: EnumCheckListResposta.Reprovada }
];

var CheckListOpcoes = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ordem = PropertyEntity({ text: "Ordem:", val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, maxlength: 1000 });
    this.Categoria = PropertyEntity({ text: "Categoria: ", issue: 1164, val: ko.observable(EnumCategoriaOpcaoCheckList.Tracao), options: EnumCategoriaOpcaoCheckList.obterOpcoes(), def: EnumCategoriaOpcaoCheckList.Tracao });
    this.Assunto = PropertyEntity({ text: "Categoria (Assunto): ", visible: ko.observable(false) });
    this.Aplicacao = PropertyEntity({ text: "Aplicação: ", issue: 1166, val: ko.observable(EnumAplicacaoOpcaoCheckList.Sempre), options: _situacaoAplicacao, def: EnumAplicacaoOpcaoCheckList.Sempre });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração: ", val: ko.observable(null), getType: typesKnockout.int });
    //this.TipoCheckListGuarita = PropertyEntity({ text: "Tipo Check: ", val: ko.observable(EnumTipoCheckListGuarita.Todos), options: _tipoCheckListGuarita, def: EnumTipoCheckListGuarita.Todos });
    this.TipoOpcao = PropertyEntity({ text: "Tipo de Resposta: ", issue: 1547, val: ko.observable(EnumTipoOpcaoCheckList.Aprovacao), options: EnumTipoOpcaoCheckList.obterOpcoes(), def: EnumTipoOpcaoCheckList.Aprovacao });
    this.CheckListTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Check List: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.RelacaoPergunta = PropertyEntity({ text: "Relação Pergunta:", val: ko.observable(), options: ko.observable(), visible: ko.observable(false) });
    this.RelacaoCampo = PropertyEntity({ text: "Relação Campo:", val: ko.observable(0), def: 0, options: ko.observable(), visible: ko.observable(false) });
    this.TagIntegracao = PropertyEntity({ text: "Tag Integracao:", val: ko.observable(0), def: 0, options: ko.observable(), visible: ko.observable(false) });
    this.EtapaCheckList = PropertyEntity({ text: "Etapa do Checklist:", val: ko.observable(EnumEtapaChecklist.Checklist), options: ko.observable(EnumEtapaChecklist.obterOpcoes()), def: EnumEtapaChecklist.Checklist, visible: ko.observable(false) });


    this.Obrigatorio = PropertyEntity({ type: types.map, text: "Obrigatório", visible: ko.observable(true) });
    this.TipoData = PropertyEntity({ type: types.map, text: "Tipo Data", visible: ko.observable(false) });
    this.TipoHora = PropertyEntity({ type: types.map, text: "Tipo Hora", visible: ko.observable(false) });
    this.TipoDecimal = PropertyEntity({ type: types.map, text: "Tipo Decimal", visible: ko.observable(false) });
    this.PermiteNaoAplica = PropertyEntity({ type: types.map, text: "Permite opção \"Não se Aplica (N/A)\"", visible: ko.observable(true) });
    this.ExibirSomenteParaFretesOndeRemetenteForTomador = PropertyEntity({ type: types.map, text: "Exibir somente para fretes onde o remetente for tomador", visible: ko.observable(true) });

    this.RespostaImpeditiva = PropertyEntity({ text: "Resposta Impeditiva:", val: ko.observable(0), options: _respostasImpeditivas, def: 0, visible: ko.observable(false) });

    this.TipoOpcao.val.subscribe(function (tipo) {
        _checkListOpcoes.RespostaImpeditiva.visible(false);
    });

    this.CodigoOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OrdemOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoOpcao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 1000, eventKeydown: adicionarOpcaoEnter });
    this.OpcaoImpeditiva = PropertyEntity({ text: "Resposta Impeditiva", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.ValorOpcao = PropertyEntity({ text: "Valor:", getType: typesKnockout.int, visible: ko.observable(false), eventKeydown: adicionarOpcaoEnter });
    this.CodigoIntegracaoOpcao = PropertyEntity({ text: "Código de integração: ", val: ko.observable(null), getType: typesKnockout.int });
    this.Opcoes = PropertyEntity({ idGrid: guid(), val: ko.observable(""), list: [], visible: ko.observable(false) });

    this.AdicionarOpcao = PropertyEntity({ eventClick: adicionarOpcao, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CancelarOpcao = PropertyEntity({ eventClick: cancelarOpcao, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.AtualizarOpcao = PropertyEntity({ eventClick: atualizarOpcao, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.ExcluirOpcao = PropertyEntity({ eventClick: excluirOpcao, type: types.event, text: "Excluir", visible: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaCheckListOpcoes = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, getType: typesKnockout.string, maxlength: 1000 });
    this.Categoria = PropertyEntity({ text: "Categoria: ", issue: 1164, val: ko.observable(''), options: EnumCategoriaOpcaoCheckList.obterOpcoesPesquisa(), def: '' });
    this.Aplicacao = PropertyEntity({ text: "Aplicação: ", issue: 1166, val: ko.observable(''), options: _situacaoAplicacaoPesquisa, def: EnumAplicacaoOpcaoCheckList.Sempre });
    //this.TipoCheckListGuarita = PropertyEntity({ text: "Tipo Check: ", val: ko.observable(''), options: _tipoCheckListGuarita, def: EnumTipoCheckListGuarita.Todos });
    this.CheckListTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Check List: ", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckListOpcoes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


//*******EVENTOS*******
function loadCheckListOpcoes() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCheckListOpcoes = new PesquisaCheckListOpcoes();
    KoBindings(_pesquisaCheckListOpcoes, "knockoutPesquisaCheckListOpcoes", false, _pesquisaCheckListOpcoes.Pesquisar.id);

    // Instancia objeto principal
    _checkListOpcoes = new CheckListOpcoes();
    KoBindings(_checkListOpcoes, "knockoutCheckListOpcoes");

    HeaderAuditoria("CheckListOpcoes", _checkListOpcoes);

    PreencherRelacoesCampo();
    PreencherTagIntegracao();

    new BuscarCheckListTipo(_pesquisaCheckListOpcoes.CheckListTipo);
    new BuscarFilial(_pesquisaCheckListOpcoes.Filial);

    new BuscarCheckListTipo(_checkListOpcoes.CheckListTipo);
    new BuscarFilial(_checkListOpcoes.Filial, verificarTipoChecklistImpressao);

    GridOpcoes();
    _checkListOpcoes.TipoOpcao.val.subscribe(onTipoCheckListChange);
    _checkListOpcoes.Categoria.val.subscribe(onCategoriaChange);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaCheckListOpcoes.Filial.visible(false);
        _checkListOpcoes.Filial.visible(false);
    }


    // Inicia busca
    buscarCheckListOpcoes();

    criarTagsDescricao();
}

function adicionarClick(e, sender) {
    Salvar(_checkListOpcoes, "CheckListOpcoes/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCheckListOpcoes.CarregarGrid();
                limparCamposCheckListOpcoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_checkListOpcoes, "CheckListOpcoes/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCheckListOpcoes.CarregarGrid();
                limparCamposCheckListOpcoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_checkListOpcoes, "CheckListOpcoes/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCheckListOpcoes.CarregarGrid();
                    limparCamposCheckListOpcoes();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCheckListOpcoes();
}

function editarCheckListOpcoesClick(itemGrid) {
    // Limpa os campos
    limparCamposCheckListOpcoes();

    // Seta o codigo do objeto
    _checkListOpcoes.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_checkListOpcoes, "CheckListOpcoes/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCheckListOpcoes.ExibirFiltros.visibleFade(false);

                EditarOpcoes();
                preencherOpcoesRelacaoPergunta(arg.Data.RelacaoPerguntas);
                _checkListOpcoes.RelacaoPergunta.val(arg.Data.RelacaoPergunta);
                // Alternas os campos de CRUD
                _checkListOpcoes.Atualizar.visible(true);
                _checkListOpcoes.Excluir.visible(true);
                _checkListOpcoes.Cancelar.visible(true);
                _checkListOpcoes.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCheckListOpcoes() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCheckListOpcoesClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCheckListOpcoes = new GridView(_pesquisaCheckListOpcoes.Pesquisar.idGrid, "CheckListOpcoes/Pesquisa", _pesquisaCheckListOpcoes, menuOpcoes, null);
    _gridCheckListOpcoes.CarregarGrid();
}

function limparCamposCheckListOpcoes() {
    _checkListOpcoes.Atualizar.visible(false);
    _checkListOpcoes.Cancelar.visible(false);
    _checkListOpcoes.Excluir.visible(false);
    _checkListOpcoes.Adicionar.visible(true);
    _checkListOpcoes.RelacaoPergunta.visible(false);
    LimparCampos(_checkListOpcoes);
    LimparOpcoes();
}

function onCategoriaChange() {
    if (_checkListOpcoes.Categoria.val() === EnumCategoriaOpcaoCheckList.Outro)
        _checkListOpcoes.Assunto.visible(true);
    else
        _checkListOpcoes.Assunto.visible(false);
}

function onTipoCheckListChange() {
    var typesEnableOptions = [EnumTipoOpcaoCheckList.Selecoes, EnumTipoOpcaoCheckList.Opcoes, EnumTipoOpcaoCheckList.Escala];

    if (typesEnableOptions.indexOf(_checkListOpcoes.TipoOpcao.val()) >= 0)
        _checkListOpcoes.Opcoes.visible(true);
    else
        _checkListOpcoes.Opcoes.visible(false);

    if (isEscalaOption())
        _checkListOpcoes.ValorOpcao.visible(true);
    else
        _checkListOpcoes.ValorOpcao.visible(false);

    if (_checkListOpcoes.TipoOpcao.val() === EnumTipoOpcaoCheckList.Informativo) {
        _checkListOpcoes.TipoData.visible(true);
        _checkListOpcoes.TipoHora.visible(true);
        _checkListOpcoes.TipoDecimal.visible(true);
        _checkListOpcoes.RelacaoCampo.visible(true);
        _checkListOpcoes.TagIntegracao.visible(true);
    } else {
        _checkListOpcoes.TipoData.visible(false);
        _checkListOpcoes.TipoHora.visible(false);
        _checkListOpcoes.TipoDecimal.visible(false);
        _checkListOpcoes.RelacaoCampo.visible(false);
        _checkListOpcoes.TagIntegracao.visible(false);
    }
}

function isEscalaOption() {
    return _checkListOpcoes.TipoOpcao.val() === EnumTipoOpcaoCheckList.Escala;
}

function verificarTipoChecklistImpressao(retorno) {
    _checkListOpcoes.RelacaoPergunta.options([]);
    _checkListOpcoes.Filial.codEntity(retorno.Codigo);;
    _checkListOpcoes.Filial.val(retorno.Descricao);

    _checkListOpcoes.EtapaCheckList.visible(retorno.CheckListAtivado && retorno.AvaliacaoDescargaAtivado);

    if (retorno.TipoChecklistImpressao != "") {
        executarReST("CheckListOpcoes/BuscarRelacaoPergunta", { TipoChecklistImpressao: retorno.TipoChecklistImpressao }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    preencherOpcoesRelacaoPergunta(retorno.Data.RelacaoPergunta);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        }, null);
    } else {
        _checkListOpcoes.RelacaoPergunta.visible(false);
    }
}

function preencherOpcoesRelacaoPergunta(listaRelacaoPergunta) {
    var opcoesRelacaoPergunta = [];

    listaRelacaoPergunta.forEach(function (relacao) {
        opcoesRelacaoPergunta.push({
            text: relacao.Descricao,
            value: relacao.Codigo
        });
    });

    if (opcoesRelacaoPergunta.length > 0)
        _checkListOpcoes.RelacaoPergunta.visible(true);
    else
        _checkListOpcoes.RelacaoPergunta.visible(false);

    _checkListOpcoes.RelacaoPergunta.options(opcoesRelacaoPergunta);
}

function PreencherRelacoesCampo() {
    if (_relacoesCampo.length == 0)
        return;

    var opcoesRelacaoCampo = [];

    opcoesRelacaoCampo.push({
        text: "Nenhuma",
        value: 0
    });

    _relacoesCampo.forEach(function (relacao) {
        opcoesRelacaoCampo.push({
            text: relacao.Descricao,
            value: relacao.Codigo
        });
    });

    _checkListOpcoes.RelacaoCampo.visible(_checkListOpcoes.TipoOpcao.val() === EnumTipoOpcaoCheckList.Informativo);
    _checkListOpcoes.RelacaoCampo.options(opcoesRelacaoCampo);
}


function PreencherTagIntegracao() {
    var opcoesTagIntegracao = [];

    opcoesTagIntegracao.push({ text: "Nenhuma", value: "" });
    _TagIntegracao.forEach(function (relacao) {
        opcoesTagIntegracao.push({
            text: relacao.Descricao,
            value: relacao.Codigo
        });
    });


    _checkListOpcoes.TagIntegracao.visible(_checkListOpcoes.TipoOpcao.val() === EnumTipoOpcaoCheckList.Informativo);
    _checkListOpcoes.TagIntegracao.options(opcoesTagIntegracao);
}



function criarTagsDescricao() {
    var htmlDiv = "<div id=\"tag-text-field\" class=\"tag-text-field-items\" style=\"display: none;\"></div>";

    $("#" + _checkListOpcoes.Descricao.id).after(htmlDiv);

    for (var i = 0; i < tagsDescricao.length; i++) {
        var idElemento = tagsDescricao[i].id;
        var texto = tagsDescricao[i].texto;

        var item = "<div><input class=\"btn btn-labeled btn-default \" type=\"button\""
            + "value =\"" + texto + "\" id=\"" + idElemento + "\"></div>";

        $("#tag-text-field").append(item);

        $("#" + idElemento).on('click', function (e) {
            var valor = tagsDescricao.filter(function (elemento) {
                return elemento.id == e.target.id;
            })[0].valor;

            var valorAtualCampo = _checkListOpcoes.Descricao.val();
            _checkListOpcoes.Descricao.val(valorAtualCampo + valor);
        });
    }

    $("#" + _checkListOpcoes.Descricao.id).focusin(function () {
        if (!_checkListOpcoes.ExibirSomenteParaFretesOndeRemetenteForTomador.val())
            return;

        $("#tag-text-field").css('display', 'block');
    });

    $("#tag-text-field").mouseleave(function () {
        if ($(document.activeElement)[0].id != _checkListOpcoes.Descricao.id)
            $("#tag-text-field").css('display', 'none');
    });

    $("#" + _checkListOpcoes.Descricao.id).mouseleave(function () {
        if (!$("#tag-text-field").is(":hover"))
            $("#tag-text-field").css('display', 'none');
    });
}