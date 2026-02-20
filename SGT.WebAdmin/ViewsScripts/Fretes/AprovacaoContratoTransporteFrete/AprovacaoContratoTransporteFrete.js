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
/// <reference path="../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../ViewsScripts/Consultas/ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoAprovacaoContratoTransporteFrete;
var _pesquisaAprovacaoContratoTransporteFrete;
var _gridAprovacaoContratoTransporteFrete;

var PesquisaAprovacaoContratoTransporteFrete = function () {
    this.NumeroContrato = PropertyEntity({ text: "Número do Contrato:", getType: typesKnockout.int });
    this.ContratoExternoID = PropertyEntity({ text: "ID Contrato Externo:", getType: typesKnockout.int });
    this.Categoria = PropertyEntity({ text: "Categoria:", val: ko.observable(EnumCategoriaContratoTransporte.Todos), options: EnumCategoriaContratoTransporte.ObterOpcoesPesquisa(), def: EnumCategoriaContratoTransporte.Todos });
    this.SubCategoria = PropertyEntity({ text: "SubCategoria:", val: ko.observable(EnumSubCategoriaContratoTransporte.Todos), options: EnumSubCategoriaContratoTransporte.ObterOpcoesPesquisa(), def: EnumSubCategoriaContratoTransporte.Todos });
    this.Transportador = PropertyEntity({ text: "Transportador: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.PessoaJuridica = PropertyEntity({ text: "Pessoa Jurídica: ", val: ko.observable(EnumPessoaJuridicaContratoTransporte.Todos), options: EnumPessoaJuridicaContratoTransporte.ObterOpcoesPesquisa(), def: EnumPessoaJuridicaContratoTransporte.Todos });

    this.DataInicio = PropertyEntity({ text: "Data de Início:", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.StatusAprovacaoTransportador = PropertyEntity({ text: "Situação Aprovação Transportador:", val: ko.observable(EnumStatusAprovacaoTransportador.Todos), options: EnumStatusAprovacaoTransportador.ObterOpcoesPesquisa(), def: EnumStatusAprovacaoTransportador.Todos, enable: false });
    this.StatusAssinaturaContrato = PropertyEntity({ text: "Situação da Assinatura do Contrato: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAprovacaoContratoTransporteFrete.CarregarGrid();
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

var AprovacaoContratoTransporteFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroContrato = PropertyEntity({ text: "Número do Contrato:", enable: false, getType: typesKnockout.string });
    this.ContratoExternoID = PropertyEntity({ text: "ID Externo do Contrato:", enable: false, getType: typesKnockout.string });
    this.NomeContrato = PropertyEntity({ text: "Nome do Contrato:", enable: false, getType: typesKnockout.string });
    this.AprovacaoAdicionalRequerida = PropertyEntity({ text: "Aprovação Adicional Requerida:", enable: false, getType: typesKnockout.string });
    this.Pais = PropertyEntity({ text: "País:", enable: false, getType: typesKnockout.string });
    this.Empresa = PropertyEntity({ text: "Empresa:", enable: false, getType: typesKnockout.string });
    this.ConformidadeComRSP = PropertyEntity({ text: "Conformidade com RSP:", enable: false, getType: typesKnockout.string });
    this.DataInicio = PropertyEntity({ text: "Data de Início:", enable: false, getType: typesKnockout.string });
    this.DataFim = PropertyEntity({ text: "Data Final:", enable: false, getType: typesKnockout.string });
    this.ValorPrevistoContrato = PropertyEntity({ text: "Valor Previsto no Contrato:", enable: false, getType: typesKnockout.string });
    this.ClausulaPenal = PropertyEntity({ text: "Cláusula Penal:", enable: false, getType: typesKnockout.string });
    this.Observacao = PropertyEntity({ text: "Observação:", enable: false, getType: typesKnockout.string });
    this.Usuario = PropertyEntity({ text: "Usuário:", enable: false, getType: typesKnockout.string });
    this.Ativo = PropertyEntity({ text: "Situação:", enable: false, getType: typesKnockout.string });
    this.Moeda = PropertyEntity({ text: "Moeda:", enable: false, getType: typesKnockout.string });
    this.Network = PropertyEntity({ text: "Network:", enable: false, getType: typesKnockout.string });
    this.Equipe = PropertyEntity({ text: "Equipe:", enable: false, getType: typesKnockout.string });
    this.Categoria = PropertyEntity({ text: "Categoria:", enable: false, getType: typesKnockout.string });
    this.SubCategoria = PropertyEntity({ text: "Subcategoria:", enable: false, getType: typesKnockout.string });
    this.ModoContrato = PropertyEntity({ text: "Modo do Contrato:", enable: false, getType: typesKnockout.string });
    this.PessoaJuridica = PropertyEntity({ text: "Pessoa Jurídica:", enable: false, getType: typesKnockout.string });
    this.TipoContrato = PropertyEntity({ text: "Tipo de Contrato:", enable: false, getType: typesKnockout.string });
    this.ProcessoAprovacao = PropertyEntity({ text: "Processo de Aprovação:", enable: false, getType: typesKnockout.string });
    this.Cluster = PropertyEntity({ text: "Cluster:", enable: false, getType: typesKnockout.string });
    this.HubNonHub = PropertyEntity({ text: "Hub Non Hub:", enable: false, getType: typesKnockout.string });
    this.DominioOTM = PropertyEntity({ text: "Domínio OTM:", enable: false, getType: typesKnockout.string });
    this.Padrao = PropertyEntity({ text: "Padrão:", enable: false, getType: typesKnockout.string });
    this.TermosPagamento = PropertyEntity({ text: "Termos de Pagamento:", enable: false, getType: typesKnockout.string });
    this.StatusAssinaturaContrato = PropertyEntity({ text: "Situação da Assinatura do Contrato:", enable: false, getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos:", enable: false, getType: typesKnockout.string });
    this.StatusAprovacaoTransportador = PropertyEntity({ text: "Situação Aprovação do Transportador:", enable: false, getType: typesKnockout.string });

    this.StatusAprovacaoTransportador.val.subscribe(function (novoValor) {
        if (novoValor == "Aguardando Aprovação") {
            _contratoAprovacaoContratoTransporteFrete.Rejeitar.visible(true);
            _contratoAprovacaoContratoTransporteFrete.Aprovar.visible(true);
        } else {
            _contratoAprovacaoContratoTransporteFrete.Rejeitar.visible(false);
            _contratoAprovacaoContratoTransporteFrete.Aprovar.visible(false);
        }
    })

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarClick, type: types.event, text: "Aprovar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function loadContratoTransporteFrete() {
    _pesquisaAprovacaoContratoTransporteFrete = new PesquisaAprovacaoContratoTransporteFrete();
    KoBindings(_pesquisaAprovacaoContratoTransporteFrete, "knockoutPesquisaAprovacaoContratoTransporteFrete", false, _pesquisaAprovacaoContratoTransporteFrete.Pesquisar.id);

    _contratoAprovacaoContratoTransporteFrete = new AprovacaoContratoTransporteFrete();
    KoBindings(_contratoAprovacaoContratoTransporteFrete, "knockoutAprovacaoContratoTransporteFrete");

    new BuscarTransportadores(_pesquisaAprovacaoContratoTransporteFrete.Transportador);
    new BuscarStatusAssinaturaContrato(_pesquisaAprovacaoContratoTransporteFrete.StatusAssinaturaContrato);

    HeaderAuditoria("ContratoTransporteFrete", _contratoAprovacaoContratoTransporteFrete);

    BuscarAprovacaoContratoTransporteFrete();
}

function aprovarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente aprovar o contrato?", function () {
        executarReST("AprovacaoContratoTransporteFrete/AprovarContrato", { Codigo: _contratoAprovacaoContratoTransporteFrete.Codigo.val() }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Contrato aprovado com sucesso");
                LimparCamposAprovacaoContratoTransporteFrete();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function rejeitarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente rejeitar o contrato?", function () {
        executarReST("AprovacaoContratoTransporteFrete/ReprovarContrato", { Codigo: _contratoAprovacaoContratoTransporteFrete.Codigo.val() }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Contrato rejeitado");
                LimparCamposAprovacaoContratoTransporteFrete();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });


}

function cancelarClick(e) {
    LimparCamposAprovacaoContratoTransporteFrete();
}

function editarAprovacaoContratoTransporteFrete(itemGrid) {
    LimparCamposAprovacaoContratoTransporteFrete();

    _contratoAprovacaoContratoTransporteFrete.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_contratoAprovacaoContratoTransporteFrete, "AprovacaoContratoTransporteFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaAprovacaoContratoTransporteFrete.ExibirFiltros.visibleFade(false);

                alterarVisibilidadeBotoes(arg.Data.StatusAprovacaoTransportador);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function BuscarAprovacaoContratoTransporteFrete() {
    var editar = { descricao: "Visualizar", id: "clasEditar", evento: "onclick", metodo: editarAprovacaoContratoTransporteFrete, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridAprovacaoContratoTransporteFrete = new GridView(_pesquisaAprovacaoContratoTransporteFrete.Pesquisar.idGrid, "AprovacaoContratoTransporteFrete/Pesquisa", _pesquisaAprovacaoContratoTransporteFrete, menuOpcoes, null);
    _gridAprovacaoContratoTransporteFrete.CarregarGrid();
}

function LimparCamposAprovacaoContratoTransporteFrete() {
    _contratoAprovacaoContratoTransporteFrete.Aprovar.visible(false);
    _contratoAprovacaoContratoTransporteFrete.Cancelar.visible(false);
    _contratoAprovacaoContratoTransporteFrete.Rejeitar.visible(false);
    LimparCampos(_contratoAprovacaoContratoTransporteFrete);
    _gridAprovacaoContratoTransporteFrete.CarregarGrid();
}

function alterarVisibilidadeBotoes(statusAprovacao) {
    if (statusAprovacao == "Aguardando Aprovação") {
        _contratoAprovacaoContratoTransporteFrete.Rejeitar.visible(true);
        _contratoAprovacaoContratoTransporteFrete.Aprovar.visible(true);
    } else {
        _contratoAprovacaoContratoTransporteFrete.Rejeitar.visible(false);
        _contratoAprovacaoContratoTransporteFrete.Aprovar.visible(false);
    }

    _contratoAprovacaoContratoTransporteFrete.Cancelar.visible(true);
}