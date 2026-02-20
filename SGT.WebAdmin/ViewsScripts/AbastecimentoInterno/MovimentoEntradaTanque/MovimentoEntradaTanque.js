
/// <reference path="../../Consultas/Empresa.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMovimentoEntradaTanque;
var _movimentoEntradaTanque;
var _pesquisaMovimentoEntradaTanque;
var _gridMovimentoEntradaTanque;

/*
 * Declaração das Classes
 */

var CRUDMovimentoEntradaTanque = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });

};

var MovimentoEntradaTanque = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.Descricao.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.Empresa.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DocumentoEntrada.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataHoraNotaFiscal = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DataHoraNotaFiscal.getRequiredFieldDescription(), required: true, getType: typesKnockout.date, val: ko.observable(""), enable: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.Fornecedor.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.LocalArmazenamento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.DataHoraEntrada = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DataHoraEntrada.getRequiredFieldDescription(), required: true, getType: typesKnockout.dateTime });
    this.QuantidadeEntrada = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.QuantidadeEntrada.getRequiredFieldDescription(), required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.TipoOleo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
};

var PesquisaMovimentoEntradaTanque = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.Descricao, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.TipoOleo, idBtnSearch: guid()});
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.Empresa, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DocumentoEntrada, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.LocalArmazenamento, idBtnSearch: guid()});
    this.DataInicialEntrada = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DataInicialEntrada, getType: typesKnockout.date });
    this.DataFinalEntrada = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.DataFinalEntrada, getType: typesKnockout.date });


    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridMovimentoEntradaTanque, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridMovimentoEntradaTanque() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridMovimentoEntradaTanque = new GridViewExportacao(_pesquisaMovimentoEntradaTanque.Pesquisar.idGrid, "MovimentoEntradaTanque/Pesquisa", _pesquisaMovimentoEntradaTanque, menuOpcoes);
    _gridMovimentoEntradaTanque.CarregarGrid();
}

function LoadMovimentoEntradaTanque() {
    _movimentoEntradaTanque = new MovimentoEntradaTanque();
    KoBindings(_movimentoEntradaTanque, "knockoutMovimentoEntradaTanque");

    _CRUDMovimentoEntradaTanque = new CRUDMovimentoEntradaTanque();
    KoBindings(_CRUDMovimentoEntradaTanque, "knockoutCRUDMovimentoEntradaTanque");

    _pesquisaMovimentoEntradaTanque = new PesquisaMovimentoEntradaTanque();
    KoBindings(_pesquisaMovimentoEntradaTanque, "knockoutPesquisaMovimentoEntradaTanque", false, _pesquisaMovimentoEntradaTanque.Pesquisar.id);

    BuscarEmpresa(_movimentoEntradaTanque.Empresa);
    BuscarEmpresa(_pesquisaMovimentoEntradaTanque.Empresa);

    BuscarTipoOleo(_movimentoEntradaTanque.TipoOleo);
    BuscarTipoOleo(_pesquisaMovimentoEntradaTanque.TipoOleo);

    BuscarClientes(_movimentoEntradaTanque.Fornecedor);

    BuscarDocumentoEntrada(_movimentoEntradaTanque.DocumentoEntrada, callbackBuscarDocumentoEntrada);
    BuscarDocumentoEntrada(_pesquisaMovimentoEntradaTanque.DocumentoEntrada, callbackBuscarDocumentoEntradaPesquisa);

    BuscarLocalArmazenamentoProduto(_movimentoEntradaTanque.LocalArmazenamento, callbackLocalArmazenamentoProduto);
    BuscarLocalArmazenamentoProduto(_pesquisaMovimentoEntradaTanque.LocalArmazenamento);

    LoadGridMovimentoEntradaTanque();
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function callbackBuscarDocumentoEntrada(data) {
    _movimentoEntradaTanque.DocumentoEntrada.codEntity(data.Codigo);
    _movimentoEntradaTanque.DocumentoEntrada.val(data.Numero);

    if (data.DataEntrada != null) {
        _movimentoEntradaTanque.DataHoraNotaFiscal.val(data.DataEntrada);
        _movimentoEntradaTanque.DataHoraNotaFiscal.enable(false);
    }  else {
        _movimentoEntradaTanque.DataHoraNotaFiscal.enable(true);
    }

    if (data.CodigoFornecedor > 0) {
        _movimentoEntradaTanque.Fornecedor.val(data.Fornecedor);
        _movimentoEntradaTanque.Fornecedor.codEntity(data.CodigoFornecedor);
        _movimentoEntradaTanque.Fornecedor.enable(false);
    } else {
        _movimentoEntradaTanque.Fornecedor.enable(true);
    }
    
}
function callbackBuscarDocumentoEntradaPesquisa(data) {
    _pesquisaMovimentoEntradaTanque.DocumentoEntrada.codEntity(data.Codigo);
    _pesquisaMovimentoEntradaTanque.DocumentoEntrada.val(data.Numero);
}
function callbackLocalArmazenamentoProduto(data) {
    _movimentoEntradaTanque.LocalArmazenamento.codEntity(data.Codigo);
    _movimentoEntradaTanque.LocalArmazenamento.val(data.Descricao);

    if (data.CodigoTipoOleo > 0) {
        _movimentoEntradaTanque.TipoOleo.codEntity(data.CodigoTipoOleo);
        _movimentoEntradaTanque.TipoOleo.val(data.TipoOleo);
        _movimentoEntradaTanque.TipoOleo.enable(false);
    } else {
        _movimentoEntradaTanque.TipoOleo.enable(true);
    }
}
function AdicionarClick(e, sender) {
    Salvar(_movimentoEntradaTanque, "MovimentoEntradaTanque/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);


                RecarregarGridMovimentoEntradaTanque();
                LimparCamposMovimentoEntradaTanque();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);

        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_movimentoEntradaTanque, "MovimentoEntradaTanque/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);

                RecarregarGridMovimentoEntradaTanque();
                LimparCamposMovimentoEntradaTanque();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposMovimentoEntradaTanque();
}

function EditarClick(registroSelecionado) {
    LimparCamposMovimentoEntradaTanque();

    _movimentoEntradaTanque.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_movimentoEntradaTanque, "MovimentoEntradaTanque/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMovimentoEntradaTanque.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.AbastecimentoInterno.MovimentoEntradaTanque.RealmenteDesejaExcluirMovimentoEntrada, function () {
        ExcluirPorCodigo(_movimentoEntradaTanque, "MovimentoEntradaTanque/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);

                    RecarregarGridMovimentoEntradaTanque();
                    LimparCamposMovimentoEntradaTanque();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
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
    _CRUDMovimentoEntradaTanque.Atualizar.visible(isEdicao);
    _CRUDMovimentoEntradaTanque.Excluir.visible(isEdicao);
    _CRUDMovimentoEntradaTanque.Cancelar.visible(isEdicao);
    _CRUDMovimentoEntradaTanque.Adicionar.visible(!isEdicao);
}

function LimparCamposMovimentoEntradaTanque() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_movimentoEntradaTanque);
}

function RecarregarGridMovimentoEntradaTanque() {
    _gridMovimentoEntradaTanque.CarregarGrid();
}