/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Configuracao.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO*******

var _pesquisaCargaDadosEmissaoConfiguracaoPedidos;
var _cargaDadosEmissaoConfiguracaoPedidos;
var _gridPedidosConfiguracaoEmissao;

var PesquisaCargaDadosEmissaoConfiguracaoPedidos = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario, idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.SomentePendentes = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Cargas.Carga.SomentePendentesDeConfiguracao, def: true, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SelecionarTodos });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedidosConfiguracaoEmissao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

var CargaDadosEmissaoConfiguracaoPedidos = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.FormulaRateio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.FormulaDoRateioDoFrete.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoEmissaoCTeParticipantes = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeParticipantes.Normal), options: EnumTipoEmissaoCTeParticipantes.obterOpcoes(), text: Localization.Resources.Cargas.Carga.ParticipantesDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeParticipantes.Normal, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoRateio = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoes(), text: Localization.Resources.Cargas.Carga.RateioDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeDocumentos.NaoInformado, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Recebedor.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Expedidor.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: alterarDadosEmissaoConfiguracaoPedidosClick, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarConfiguracoes, visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoEmissaoCTeParticipantes.val.subscribe(function (novoValor) {
        TipoEmissaoCTeParticipantesConfiguracaoPedidosChange(novoValor);
    });
}

function loadCargaDadosEmissaoConfiguracaoPedidos() {
    _pesquisaCargaDadosEmissaoConfiguracaoPedidos = new PesquisaCargaDadosEmissaoConfiguracaoPedidos();
    _cargaDadosEmissaoConfiguracaoPedidos = new CargaDadosEmissaoConfiguracaoPedidos();

    KoBindings(_pesquisaCargaDadosEmissaoConfiguracaoPedidos, "knoutPesquisaCargaDadosEmissaoConfiguracaoPedidos", false, _pesquisaCargaDadosEmissaoConfiguracaoPedidos.Pesquisar.id);
    KoBindings(_cargaDadosEmissaoConfiguracaoPedidos, "knoutCargaDadosEmissaoConfiguracaoPedidos");

    new BuscarRateioFormulas(_cargaDadosEmissaoConfiguracaoPedidos.FormulaRateio);
    new BuscarClientes(_cargaDadosEmissaoConfiguracaoPedidos.Expedidor);
    new BuscarClientes(_cargaDadosEmissaoConfiguracaoPedidos.Recebedor);

    new BuscarClientes(_pesquisaCargaDadosEmissaoConfiguracaoPedidos.Destinatario);

    BuscarCargasPedidosParaConfiguracao();
}


function abrirModalEditarPedidos() {
    if (_cargaDadosEmissaoConfiguracaoPedidos == undefined || _pesquisaCargaDadosEmissaoConfiguracaoPedidos == undefined)
        loadCargaDadosEmissaoConfiguracaoPedidos();

    LimparCampos(_pesquisaCargaDadosEmissaoConfiguracaoPedidos);
    LimparCampos(_cargaDadosEmissaoConfiguracaoPedidos);

    _cargaDadosEmissaoConfiguracaoPedidos.Carga.val(_cargaAtual.Codigo.val());
    _pesquisaCargaDadosEmissaoConfiguracaoPedidos.Carga.val(_cargaAtual.Codigo.val());

    _gridPedidosConfiguracaoEmissao.CarregarGrid(function () {
        Global.abrirModal("divModalAjustePedidosCarga");
    });
}


function TipoEmissaoCTeParticipantesConfiguracaoPedidosChange(valor) {
    if (valor == EnumTipoEmissaoCTeParticipantes.ComRecebedor) {
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.visible(true);
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.required = true;
    }
    else {
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.visible(false);
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.required = false;
    }

    if (valor == EnumTipoEmissaoCTeParticipantes.ComExpedidor) {
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.visible(true);
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.required = true;
    }
    else {
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.visible(false);
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.required = false;
    }

    if (valor == EnumTipoEmissaoCTeParticipantes.ComExpedidorERecebedor) {
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.visible(true);
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.visible(true);
        _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.required = true;
        _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.required = true;
    }
}

function alterarDadosEmissaoConfiguracaoPedidosClick() {

    var documentosSelecionadas = _gridPedidosConfiguracaoEmissao.ObterMultiplosSelecionados();
    var documentosNaoSelecionadas = _gridPedidosConfiguracaoEmissao.ObterMultiplosNaoSelecionados();

    if (documentosSelecionadas.length > 0 || (_pesquisaCargaDadosEmissaoConfiguracaoPedidos.SelecionarTodos.val() && documentosNaoSelecionadas.length < _gridPedidosConfiguracaoEmissao.NumeroRegistros())) {
        if (ValidarCamposObrigatorios(_cargaDadosEmissaoConfiguracaoPedidos)) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaalterarConfiguracaoDosPedidosSelecionados, function () {
                var data = {
                    DocumentosSelecionadas: JSON.stringify(documentosSelecionadas),
                    DocumentosNaoSelecionadas: JSON.stringify(documentosNaoSelecionadas),
                    Carga: _cargaDadosEmissaoConfiguracaoPedidos.Carga.val(),
                    FormulaRateio: _cargaDadosEmissaoConfiguracaoPedidos.FormulaRateio.codEntity(),
                    TipoEmissaoCTeParticipantes: _cargaDadosEmissaoConfiguracaoPedidos.TipoEmissaoCTeParticipantes.val(),
                    TipoRateio: _cargaDadosEmissaoConfiguracaoPedidos.TipoRateio.val(),
                    Recebedor: _cargaDadosEmissaoConfiguracaoPedidos.Recebedor.codEntity(),
                    Expedidor: _cargaDadosEmissaoConfiguracaoPedidos.Expedidor.codEntity(),
                    SelecionarTodos: _pesquisaCargaDadosEmissaoConfiguracaoPedidos.SelecionarTodos.val(),
                    SomentePendentes: _pesquisaCargaDadosEmissaoConfiguracaoPedidos.SomentePendentes.val()
                };
                executarReST("DadosEmissaoConfiguracao/AtualizarConfiguracoesPedidos", data, function (arg) {
                    if (arg.Success) {
                        if (arg.Data !== false) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ConfiguracaoAtualizadaComSucesso);
                            atualizarStatusConfiguracoes(documentosSelecionadas, documentosNaoSelecionadas);
                            BuscarCargasPedidosParaConfiguracao();
                            _gridPedidosConfiguracaoEmissao.CarregarGrid();

                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }

                });
            });
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.ObrigatorioSelecionarAoMenosUmPedido);
    }
}

function atualizarStatusConfiguracoes(documentosSelecionadas, documentosNaoSelecionadas) {
    for (var i = 0; i < _cargaAtual.Pedidos.val.length; i++) {
        var pedido = _cargaAtual.Pedidos.val[i];
        if (_pesquisaCargaDadosEmissaoConfiguracaoPedidos.SelecionarTodos.val()) {
            var desmarcar = true;
            for (var j = 0; j < documentosNaoSelecionadas.length; j++) {
                if (pedido.CodigoCargaPedido == documentosNaoSelecionadas[j].Codigo) {
                    desmarcar = false;
                    break;
                }
            }
            if (desmarcar)
                DesmacarPendenciaConfiguracao(i);
        } else {
            var desmarcar = false;
            for (var j = 0; j < documentosSelecionadas.length; j++) {
                if (pedido.CodigoCargaPedido == documentosSelecionadas[j].Codigo) {
                    desmarcar = true;
                    break;
                }
            }
            if (desmarcar)
                DesmacarPendenciaConfiguracao(i);
        }
    }
}

function BuscarCargasPedidosParaConfiguracao() {

    var menuOpcoes = null;
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCargaDadosEmissaoConfiguracaoPedidos.SelecionarTodos,
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };
    _gridPedidosConfiguracaoEmissao = new GridView(_pesquisaCargaDadosEmissaoConfiguracaoPedidos.Pesquisar.idGrid, "DadosEmissaoConfiguracao/PesquisaCargasPedidoConfiguracao", _pesquisaCargaDadosEmissaoConfiguracaoPedidos, menuOpcoes, null, 10, null, null, null, multiplaescolha);
}