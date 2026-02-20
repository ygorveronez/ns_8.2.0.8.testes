/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumResponsavelAvaria.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeMotivoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoAvaria;
var _CRUDMotivoAvaria;
var _pesquisaMotivoAvaria;
var _gridMotivoAvaria;

var _responsavel = [
    { text: "Transportador", value: EnumResponsavelAvaria.Transportador },
    { text: "Carregamento/Descarregamento", value: EnumResponsavelAvaria.CarregamentoDescarregamento }
];
var _finalidadePesquisa = [
    { text: "Todas", value: EnumFinalidadeMotivoAvaria.Todas },
    { text: "Motivo Avaria", value: EnumFinalidadeMotivoAvaria.MotivoAvaria },
    { text: "Autorização Avaria", value: EnumFinalidadeMotivoAvaria.AutorizacaoAvaria }
];
var _finalidade = [
    { text: "Motivo Avaria", value: EnumFinalidadeMotivoAvaria.MotivoAvaria },
    { text: "Autorização Avaria", value: EnumFinalidadeMotivoAvaria.AutorizacaoAvaria }
];

var MotivoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Responsavel = PropertyEntity({ text: "Responsável: ", issue: 952, val: ko.observable(EnumResponsavelAvaria.Transportador), options: _responsavel, def: EnumResponsavelAvaria.Transportador, visible: ko.observable(true) });
    this.Finalidade = PropertyEntity({ text: "Finalidade: ", issue: 951, val: ko.observable(EnumFinalidadeMotivoAvaria.MotivoAvaria), options: _finalidade, def: EnumFinalidadeMotivoAvaria.MotivoAvaria });
    this.Finalidade.val.subscribe(finalidadeChange);
    this.ContaContabil = PropertyEntity({ text: "*Conta Contábil:", issue: 950, required: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    this.TipoOcorrencia = PropertyEntity({ text: "Tipo da Ocorrência:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.GerarOcorrenciaAutomaticamente = PropertyEntity({ text: "Gerar ocorrência automaticamente?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ObrigarInformarValorParaLiberarOcorrencia = PropertyEntity({ text: "Este motivo obriga informar o valor para liberar ocorrência?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.GerarCTeValorAnteriorTratativaReentrega = PropertyEntity({ text: "Gerar CTe com mesmo valor do anterior quando tratativa for Reentrega?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ObrigarAnexo = PropertyEntity({ text: "Obrigar anexo?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.NaoPermitirAberturaAvariasMesmoMotivoECarga = PropertyEntity({ text: "Não permitir abertura de avarias para mesmo motivo e carga?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.PermitirInformarQuantidadeMaiorMercadoriaAvariada = PropertyEntity({ text: "Permitir informar quantidade maior de mercadoria avariada?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.DesabilitarBotaoTermo = PropertyEntity({ text: "Desabilitar botão do Termo?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.CalcularOcorrenciaPorTabelaFrete = PropertyEntity({ text: "Calcular ocorrência por tabela de frete?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.GerarOcorrenciaAutomaticamente.val.subscribe(function (novoValor) {
        _motivoAvaria.CalcularOcorrenciaPorTabelaFrete.visible(false);

        if (novoValor)
            _motivoAvaria.CalcularOcorrenciaPorTabelaFrete.visible(true);
    });
};

var CRUDMotivoAvaria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMotivoAvaria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Finalidade = PropertyEntity({ text: "Finalidade: ", issue: 951, val: ko.observable(EnumFinalidadeMotivoAvaria.Todas), options: _finalidadePesquisa, def: EnumFinalidadeMotivoAvaria.Todas });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoAvaria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadMotivoAvaria() {

    _pesquisaMotivoAvaria = new PesquisaMotivoAvaria();
    KoBindings(_pesquisaMotivoAvaria, "knockoutPesquisaMotivoAvaria", false, _pesquisaMotivoAvaria.Pesquisar.id);

    _motivoAvaria = new MotivoAvaria();
    KoBindings(_motivoAvaria, "knockoutMotivoAvaria");

    HeaderAuditoria("MotivoAvaria", _motivoAvaria);

    _CRUDMotivoAvaria = new CRUDMotivoAvaria();
    KoBindings(_CRUDMotivoAvaria, "knockoutCRUDMotivoAvaria");

    BuscarPlanoConta(_motivoAvaria.ContaContabil);
    BuscarTipoOcorrencia(_motivoAvaria.TipoOcorrencia);

    finalidadeChange();
    configurarLayoutMotivoAvariaPorTipoSistema();

    buscarMotivoAvaria();
}

function configurarLayoutMotivoAvariaPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _motivoAvaria.TipoOcorrencia.visible(true);
        _motivoAvaria.GerarOcorrenciaAutomaticamente.visible(true);
        _motivoAvaria.ObrigarInformarValorParaLiberarOcorrencia.visible(true);
        _motivoAvaria.ObrigarAnexo.visible(true);
        _motivoAvaria.PermitirInformarQuantidadeMaiorMercadoriaAvariada.visible(true);
        _motivoAvaria.NaoPermitirAberturaAvariasMesmoMotivoECarga.visible(true);
        _motivoAvaria.DesabilitarBotaoTermo.visible(true);
    }
}

function finalidadeChange() {
    if (_motivoAvaria.Finalidade.val() == EnumFinalidadeMotivoAvaria.AutorizacaoAvaria) {
        _motivoAvaria.ContaContabil.visible(false);
        _motivoAvaria.ContaContabil.required(false);
        _motivoAvaria.Responsavel.visible(false);
    }
    else if (_motivoAvaria.Finalidade.val() == EnumFinalidadeMotivoAvaria.MotivoAvaria) {
        _motivoAvaria.ContaContabil.visible(true);
        _motivoAvaria.ContaContabil.required(true);
        _motivoAvaria.Responsavel.visible(true);
    }
}

function adicionarClick(e, sender) {
    Salvar(_motivoAvaria, "MotivoAvaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoAvaria.CarregarGrid();
                limparCamposMotivoAvaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoAvaria, "MotivoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoAvaria.CarregarGrid();
                limparCamposMotivoAvaria();
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
        ExcluirPorCodigo(_motivoAvaria, "MotivoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoAvaria.CarregarGrid();
                    limparCamposMotivoAvaria();
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
    limparCamposMotivoAvaria();
}

//*******MÉTODOS*******

function editarMotivoAvariaClick(itemGrid) {
    limparCamposMotivoAvaria();

    _motivoAvaria.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivoAvaria, "MotivoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoAvaria.ExibirFiltros.visibleFade(false);

                _CRUDMotivoAvaria.Atualizar.visible(true);
                _CRUDMotivoAvaria.Excluir.visible(true);
                _CRUDMotivoAvaria.Cancelar.visible(true);
                _CRUDMotivoAvaria.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function buscarMotivoAvaria() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoAvariaClick, tamanho: "10", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    let configExportacao = {
        url: "MotivoAvaria/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };

    _gridMotivoAvaria = new GridViewExportacao(_pesquisaMotivoAvaria.Pesquisar.idGrid, "MotivoAvaria/Pesquisa", _pesquisaMotivoAvaria, menuOpcoes, configExportacao);
    _gridMotivoAvaria.CarregarGrid();
}

function limparCamposMotivoAvaria() {
    _CRUDMotivoAvaria.Atualizar.visible(false);
    _CRUDMotivoAvaria.Cancelar.visible(false);
    _CRUDMotivoAvaria.Excluir.visible(false);
    _CRUDMotivoAvaria.Adicionar.visible(true);
    LimparCampos(_motivoAvaria);
}