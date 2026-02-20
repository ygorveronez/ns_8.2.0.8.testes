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
/// <reference path="../../Enumeradores/EnumProcessoMovimento.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridProcessoMovimento;
var _processoMovimento;
var _pesquisaProcessoMovimento;

var _TipoProcessoPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Emissão CTE/NFSe", value: EnumProcessoMovimento.EmissaoCTENFSe },
    { text: "Cancela CTE/NFSe", value: EnumProcessoMovimento.CancelamentoCTENFSe },
    { text: "Anulação CTE/NFSe", value: EnumProcessoMovimento.AnulacaoCTENFSe },
    { text: "Vale Pedágio CTE/NFSe", value: EnumProcessoMovimento.ValePedagioCTENFSe },
    { text: "Cancela Vale Pedágio CTE/NFSe", value: EnumProcessoMovimento.CancelamentoValePedagioCTENFSe },
    { text: "Emissão SubContratação", value: EnumProcessoMovimento.EmissaodeSubContratacao },
    { text: "Cancela SubContratação", value: EnumProcessoMovimento.CancelamentodeSubContratacao },
    { text: "Pagamento Contrato SubContratação", value: EnumProcessoMovimento.PagamentoContratoSubContratacao },
    { text: "Adiantamento Terceiro", value: EnumProcessoMovimento.AdiantamentoTerceiros },
    { text: "Cancela Adiantamento Terceiro", value: EnumProcessoMovimento.CancelamentodaBaixadeAdiantamentoTerceiro },
    { text: "Ocorrencia", value: EnumProcessoMovimento.Ocorrencias },
    { text: "Cancela Ocorrencia", value: EnumProcessoMovimento.CancelamentodeOcorrencias },
    { text: "Ocorrencia 999", value: EnumProcessoMovimento.Ocorrencias999 },
    { text: "Cancela Ocorrencia 999", value: EnumProcessoMovimento.CancelamentoOcorrencias999 },
    { text: "Fatura", value: EnumProcessoMovimento.GeracaodeFatura },
    { text: "Cancela Fatura", value: EnumProcessoMovimento.CancelamentoFatura },
    { text: "Desconto Fatura", value: EnumProcessoMovimento.DescontosFatura },
    { text: "Cancela Desconto Fatura", value: EnumProcessoMovimento.CancelamentoDescontosFatura },
    { text: "Acréscimo Fatura", value: EnumProcessoMovimento.AcrescimosFatura },
    { text: "Cancela Acréscimo Fatura", value: EnumProcessoMovimento.CancelamentoAcrescimoFatura },
    { text: "Baixa de Título", value: EnumProcessoMovimento.BaixadeTitulosaReceber },
    { text: "Desconto na Baixa", value: EnumProcessoMovimento.DescontoBaixadeTitulusaReceber },
    { text: "Acréscimo na Baixa", value: EnumProcessoMovimento.AcrescimoBaixadeTitulosaReceber },
    { text: "Cancela Baixa de Título", value: EnumProcessoMovimento.CancelamentoBaixadeTitulosaReceber },
    { text: "Cancela Acréscimo na Baixa", value: EnumProcessoMovimento.CancelamentoAcrescimoBaixadeTitulosaReceber },
    { text: "Cancela Desconto na Baixa", value: EnumProcessoMovimento.CancelamentoDescontoBaixadeTitulusaReceber },
    { text: "Geração de Novas Percelas de Negociação", value: EnumProcessoMovimento.GeracaodeParcelasdeNegociacao },
    { text: "Pedágio Recebido no Acerto", value: EnumProcessoMovimento.ValePedagioRecebidodeClienteAcerto },
    { text: "Cancela Pedágio Recebido no Acerto", value: EnumProcessoMovimento.CancelamentoPedagioRecebidodeClienteAcerto },
    { text: "Abastecimento do Motorista no Acerto", value: EnumProcessoMovimento.AbastecimentoPagoMotoristaAcerto },
    { text: "Cancela Abastecimento do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentoAbastecimentoPagoMotoristaAcerto },
    { text: "Pagamento Pedágio do Motorista no Acerto", value: EnumProcessoMovimento.PagamentoPedagiopeloMotoristaAcerto },
    { text: "Cancela Pagamento Pedágio do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentoPagamentoPedagioPeloMotoristaAcerto },
    { text: "Outras Despesas no Acerto", value: EnumProcessoMovimento.OutrasDespesasAcerto },
    { text: "Cancela Outras Despesas no Acerto", value: EnumProcessoMovimento.CancelamentoOutrasDespesasAcerto },
    { text: "Desconto no Acerto", value: EnumProcessoMovimento.DescontodoAcerto },
    { text: "Cancela Desconto no Acerto", value: EnumProcessoMovimento.CancelaDescontodoAcerto },
    { text: "Bonificação do Motorista no Acerto", value: EnumProcessoMovimento.BonificacaoMotoristaAcerto },
    { text: "Cancela Bonificação do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentodaBonificacaodoMotoristaAcerto },
    { text: "Comissão no Acerto", value: EnumProcessoMovimento.ComissaoAcerto },
    { text: "Cancela Comissão no Acerto", value: EnumProcessoMovimento.CancelaComissaoAcerto },
    { text: "Cancela de Percelas de Negociação", value: EnumProcessoMovimento.CancelamentoGeracaodeParcelasdeNegociacao },
    { text: "Emissão de Documento de Entrada", value: EnumProcessoMovimento.EmissaoDocumentoEntrada },
    { text: "Extorno de Documento de Entrada", value: EnumProcessoMovimento.ExtornoDocumentoEntrada },
];

var _TipoProcesso = [
    { text: "Emissão CTE/NFSe", value: EnumProcessoMovimento.EmissaoCTENFSe },
    { text: "Cancela CTE/NFSe", value: EnumProcessoMovimento.CancelamentoCTENFSe },
    { text: "Anulação CTE/NFSe", value: EnumProcessoMovimento.AnulacaoCTENFSe },
    { text: "Vale Pedágio CTE/NFSe", value: EnumProcessoMovimento.ValePedagioCTENFSe },
    { text: "Cancela Vale Pedágio CTE/NFSe", value: EnumProcessoMovimento.CancelamentoValePedagioCTENFSe },
    { text: "Emissão SubContratação", value: EnumProcessoMovimento.EmissaodeSubContratacao },
    { text: "Cancela SubContratação", value: EnumProcessoMovimento.CancelamentodeSubContratacao },
    { text: "Pagamento Contrato SubContratação", value: EnumProcessoMovimento.PagamentoContratoSubContratacao },
    { text: "Adiantamento Terceiro", value: EnumProcessoMovimento.AdiantamentoTerceiros },
    { text: "Cancela Adiantamento Terceiro", value: EnumProcessoMovimento.CancelamentodaBaixadeAdiantamentoTerceiro },
    { text: "Ocorrencia", value: EnumProcessoMovimento.Ocorrencias },
    { text: "Cancela Ocorrencia", value: EnumProcessoMovimento.CancelamentodeOcorrencias },
    { text: "Ocorrencia 999", value: EnumProcessoMovimento.Ocorrencias999 },
    { text: "Cancela Ocorrencia 999", value: EnumProcessoMovimento.CancelamentoOcorrencias999 },
    { text: "Fatura", value: EnumProcessoMovimento.GeracaodeFatura },
    { text: "Cancela Fatura", value: EnumProcessoMovimento.CancelamentoFatura },
    { text: "Desconto Fatura", value: EnumProcessoMovimento.DescontosFatura },
    { text: "Cancela Desconto Fatura", value: EnumProcessoMovimento.CancelamentoDescontosFatura },
    { text: "Acréscimo Fatura", value: EnumProcessoMovimento.AcrescimosFatura },
    { text: "Cancela Acréscimo Fatura", value: EnumProcessoMovimento.CancelamentoAcrescimoFatura },
    { text: "Baixa de Título", value: EnumProcessoMovimento.BaixadeTitulosaReceber },
    { text: "Desconto na Baixa", value: EnumProcessoMovimento.DescontoBaixadeTitulusaReceber },
    { text: "Acréscimo na Baixa", value: EnumProcessoMovimento.AcrescimoBaixadeTitulosaReceber },
    { text: "Cancela Baixa de Título", value: EnumProcessoMovimento.CancelamentoBaixadeTitulosaReceber },
    { text: "Cancela Acréscimo na Baixa", value: EnumProcessoMovimento.CancelamentoAcrescimoBaixadeTitulosaReceber },
    { text: "Cancela Desconto na Baixa", value: EnumProcessoMovimento.CancelamentoDescontoBaixadeTitulusaReceber },
    { text: "Geração de Novas Percelas de Negociação", value: EnumProcessoMovimento.GeracaodeParcelasdeNegociacao },
    { text: "Pedágio Recebido no Acerto", value: EnumProcessoMovimento.ValePedagioRecebidodeClienteAcerto },
    { text: "Cancela Pedágio Recebido no Acerto", value: EnumProcessoMovimento.CancelamentoPedagioRecebidodeClienteAcerto },
    { text: "Abastecimento do Motorista no Acerto", value: EnumProcessoMovimento.AbastecimentoPagoMotoristaAcerto },
    { text: "Cancela Abastecimento do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentoAbastecimentoPagoMotoristaAcerto },
    { text: "Pagamento Pedágio do Motorista no Acerto", value: EnumProcessoMovimento.PagamentoPedagiopeloMotoristaAcerto },
    { text: "Cancela Pagamento Pedágio do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentoPagamentoPedagioPeloMotoristaAcerto },
    { text: "Outras Despesas no Acerto", value: EnumProcessoMovimento.OutrasDespesasAcerto },
    { text: "Cancela Outras Despesas no Acerto", value: EnumProcessoMovimento.CancelamentoOutrasDespesasAcerto },
    { text: "Desconto no Acerto", value: EnumProcessoMovimento.DescontodoAcerto },
    { text: "Cancela Desconto no Acerto", value: EnumProcessoMovimento.CancelaDescontodoAcerto },
    { text: "Bonificação do Motorista no Acerto", value: EnumProcessoMovimento.BonificacaoMotoristaAcerto },
    { text: "Cancela Bonificação do Motorista no Acerto", value: EnumProcessoMovimento.CancelamentodaBonificacaodoMotoristaAcerto },
    { text: "Comissão no Acerto", value: EnumProcessoMovimento.ComissaoAcerto },
    { text: "Cancela Comissão no Acerto", value: EnumProcessoMovimento.CancelaComissaoAcerto },
    { text: "Cancela de Percelas de Negociação", value: EnumProcessoMovimento.CancelamentoGeracaodeParcelasdeNegociacao },
    { text: "Emissão de Documento de Entrada", value: EnumProcessoMovimento.EmissaoDocumentoEntrada },
    { text: "Extorno de Documento de Entrada", value: EnumProcessoMovimento.ExtornoDocumentoEntrada },
];


var PesquisaProcessoMovimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.TipoProcesso = PropertyEntity({ val: ko.observable(0), options: _TipoProcessoPesquisa, def: 0, text: "Tipo Processo: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProcessoMovimento.CarregarGrid();
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

var ProcessoMovimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.TipoProcesso = PropertyEntity({ val: ko.observable(EnumProcessoMovimento.EmissaoCTe), options: _TipoProcesso, text: "*Tipo de Processo: ", def: EnumProcessoMovimento.EmissaoCTe });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadProcessoMovimento() {

    _processoMovimento = new ProcessoMovimento();
    KoBindings(_processoMovimento, "knockoutCadastroProcessoMovimento");

    _pesquisaProcessoMovimento = new PesquisaProcessoMovimento();
    KoBindings(_pesquisaProcessoMovimento, "knockoutPesquisaProcessoMovimento", false, _pesquisaProcessoMovimento.Pesquisar.id);

    HeaderAuditoria("ProcessoMovimento", _processoMovimento);

    buscarProcessoMovimentos();
}

function adicionarClick(e, sender) {
    Salvar(e, "ProcessoMovimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridProcessoMovimento.CarregarGrid();
                limparCamposProcessoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ProcessoMovimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProcessoMovimento.CarregarGrid();
                limparCamposProcessoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Processo de Movimento " + _processoMovimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_processoMovimento, "ProcessoMovimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridProcessoMovimento.CarregarGrid();
                limparCamposProcessoMovimento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProcessoMovimento();
}

//*******MÉTODOS*******


function buscarProcessoMovimentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProcessoMovimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridProcessoMovimento = new GridView(_pesquisaProcessoMovimento.Pesquisar.idGrid, "ProcessoMovimento/Pesquisa", _pesquisaProcessoMovimento, menuOpcoes, null);
    _gridProcessoMovimento.CarregarGrid();
}

function editarProcessoMovimento(processoMovimentoGrid) {
    limparCamposProcessoMovimento();
    _processoMovimento.Codigo.val(processoMovimentoGrid.Codigo);
    BuscarPorCodigo(_processoMovimento, "ProcessoMovimento/BuscarPorCodigo", function (arg) {
        _pesquisaProcessoMovimento.ExibirFiltros.visibleFade(false);
        _processoMovimento.Atualizar.visible(true);
        _processoMovimento.Cancelar.visible(true);
        _processoMovimento.Excluir.visible(true);
        _processoMovimento.Adicionar.visible(false);
    }, null);
}

function limparCamposProcessoMovimento() {
    _processoMovimento.Atualizar.visible(false);
    _processoMovimento.Cancelar.visible(false);
    _processoMovimento.Excluir.visible(false);
    _processoMovimento.Adicionar.visible(true);
    LimparCampos(_processoMovimento);
}
