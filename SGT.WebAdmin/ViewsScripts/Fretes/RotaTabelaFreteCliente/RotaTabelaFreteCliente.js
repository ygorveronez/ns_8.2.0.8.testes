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
/// <reference path="../../Consultas/RotaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _rotaTabelaFrete;
var _pesquisaRotaTabelaFrete;
var _gridRotaTabelaFrete;

var RotaTabelaFrete = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.Rota.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });


    this.RotasFreteEmbarcador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.Atualizar), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.Cancelar), visible: ko.observable(false) });
}

var PesquisaRotaTabelaFrete = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFrete.getFieldDescription(), issue: 78, idBtnSearch: guid() });
    this.Vigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Vigencia.getFieldDescription(), idBtnSearch: guid(), issue: 82 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Codigo.getFieldDescription(), maxlength: 50 });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.RegiaoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Região de Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.RegiaoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.RegiaoDeDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: Localization.Resources.Fretes.TabelaFreteCliente.EstadoDeOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: Localization.Resources.Fretes.TabelaFreteCliente.EstadoDeDestino.getFieldDescription(), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Transportador.getFieldDescription(), issue: 69, idBtnSearch: guid(), visible: ko.observable(false) });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Rota.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Tomador.getFieldDescription(), issue: 972, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.TipoDeOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid() });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamentoEmissao.obterOpcoesPesquisa(), def: "", text: Localization.Resources.Fretes.TabelaFreteCliente.TipoDePagamento.getFieldDescription(), issue: 120 });
    this.PossuiRota = PropertyEntity({ val: ko.observable(""), options: EnumOpcaoRota.obterOpcoesPesquisa(), def: "", text: Localization.Resources.Fretes.TabelaFreteCliente.PossuiRota.getFieldDescription(), issue: 120, });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Situacao.getFieldDescription() });
    this.CEPOrigem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepDeOrigem.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestino = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepDeDestino.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.SomenteEmVigencia = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.SomenteTabelasVigentes, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.SituacaoAlteracao = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoTabelaFrete.Todas), options: EnumSituacaoAlteracaoTabelaFrete.obterOpcoesPesquisaTabelaFreteCliente(), def: EnumSituacaoAlteracaoTabelaFrete.Todas, text: Localization.Resources.Fretes.TabelaFreteCliente.SituacaoAprovacao.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Terceiro.getFieldDescription(), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.CanalEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.ContratoTransporteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoTabelaFreteCliente.Todas), options: EnumSituacaoIntegracaoTabelaFreteCliente.obterOpcoesPesquisa(), def: EnumSituacaoIntegracaoTabelaFreteCliente.Todas, text: Localization.Resources.Fretes.TabelaFreteCliente.SituacaoIntegracao.getFieldDescription(), visible: ko.observable(true) });


    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRotaTabelaFrete.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadRotaTabelaFrete() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaRotaTabelaFrete = new PesquisaRotaTabelaFrete();
    KoBindings(_pesquisaRotaTabelaFrete, "knockoutPesquisaRotaTabelaFrete", false, _pesquisaRotaTabelaFrete.Pesquisar.id);

    // Instancia ProdutoAvaria
    _rotaTabelaFrete = new RotaTabelaFrete();
    KoBindings(_rotaTabelaFrete, "knockoutRotaTabelaFrete");

    HeaderAuditoria("RotaTabelaFrete", _rotaTabelaFrete);

    buscarRotaTabelaFrete();

    BuscarRotasFrete(_rotaTabelaFrete.RotaFrete);

    //Pesquisa
    BuscarTabelasDeFrete(_pesquisaRotaTabelaFrete.TabelaFrete);
    BuscarVigenciasTabelaFrete(_pesquisaRotaTabelaFrete.Vigencia);
    BuscarClientes(_pesquisaRotaTabelaFrete.Remetente);
    BuscarClientes(_pesquisaRotaTabelaFrete.Destinatario);
    BuscarClientes(_pesquisaRotaTabelaFrete.Tomador);
    BuscarLocalidades(_pesquisaRotaTabelaFrete.LocalidadeOrigem);
    BuscarLocalidades(_pesquisaRotaTabelaFrete.LocalidadeDestino);
    BuscarRegioes(_pesquisaRotaTabelaFrete.RegiaoOrigem);
    BuscarRegioes(_pesquisaRotaTabelaFrete.RegiaoDestino);
    BuscarRotasFrete(_pesquisaRotaTabelaFrete.RotaFrete);
    BuscarTiposOperacao(_pesquisaRotaTabelaFrete.TipoOperacao);
    BuscarCanaisEntrega(_pesquisaRotaTabelaFrete.CanalEntrega);
    BuscarContratosTransporteFrete(_pesquisaRotaTabelaFrete.ContratoTransporteFrete);
    BuscarClientes(_pesquisaRotaTabelaFrete.TransportadorTerceiro, null, null, [EnumModalidadePessoa.TransportadorTerceiro]);
}

function atualizarClick(e, sender) {
    Salvar(_rotaTabelaFrete, "TabelaFreteCliente/AtualizarRota", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFreteCliente.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.CadastradoComSucesso);
                _gridRotaTabelaFrete.CarregarGrid();
                limparCamposRotaTabelaFrete();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFreteCliente.Falha, retorno.Msg);
        }
    }, sender);
}



function cancelarClick(e) {
    limparCamposRotaTabelaFrete();
}

function editarRotaTabelaFreteClick(itemGrid) {
    // Limpa os campos
    limparCamposRotaTabelaFrete();

    // Seta o codigo do ProdutoAvaria
    _rotaTabelaFrete.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_rotaTabelaFrete, "TabelaFreteCliente/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaRotaTabelaFrete.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _rotaTabelaFrete.RotaFrete.visible(true);
                _rotaTabelaFrete.Atualizar.visible(true);
                _rotaTabelaFrete.Cancelar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarRotaTabelaFrete() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRotaTabelaFreteClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    // Inicia Grid de busca
    _gridRotaTabelaFrete = new GridView(_pesquisaRotaTabelaFrete.Pesquisar.idGrid, "TabelaFreteCliente/Pesquisa", _pesquisaRotaTabelaFrete, menuOpcoes);
    _gridRotaTabelaFrete.CarregarGrid();
}

function limparCamposRotaTabelaFrete() {
    _rotaTabelaFrete.Atualizar.visible(false);
    _rotaTabelaFrete.Cancelar.visible(false);
    _rotaTabelaFrete.RotaFrete.visible(false);
    LimparCampos(_rotaTabelaFrete);
}