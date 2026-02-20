/// <reference path="../../../Consultas/MontagemContainer.js" />
/// <reference path="../../../Enumeradores/EnumStatusMontagemContainer.js" />
/// <reference path="../../../Enumeradores/EnumTipoPedidoVinculado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoVinculado, _gridCargaCTeEncaixe;

var _tipoPedidoVinculado = [
    { text: "Encaixe de Carga (Redespacho)", value: EnumTipoPedidoVinculado.EncaixeSubContratacao },
    { text: "Encaixe de Pedido (Redespacho)", value: EnumTipoPedidoVinculado.EncaixePedidoSubContratacao }
];

var _padraoTipoPedidoVinculado = EnumTipoPedidoVinculado.EncaixeSubContratacao;

var PedidoVinculado = function () {
    this.AtualizarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.int });
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.NumeroDoPedidoDoEmbarcador.getRequiredFieldDescription()), def: "", visible: ko.observable(false), getType: typesKnockout.string, maxlength: 50, required: false });
    this.TipoPedidoVinculado = PropertyEntity({ val: ko.observable(_padraoTipoPedidoVinculado), eventChange: tipoPedidoVinculadoChange, visible: ko.observable(true), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.TipoDePedido.getRequiredFieldDescription(), options: _tipoPedidoVinculado, def: _padraoTipoPedidoVinculado });
    this.OcultarStatusCTe = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.DescricaoCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaPedidos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Pedido.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.CargaPedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Pedido.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.CTesSubContratacaoFilialEmissora = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Remetente.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.EnderecoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.NovoEnderecoDeDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.EnderecoRecebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.NovoEnderecoDoRecebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.EnderecoExpedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.NovoEnderecoDoExpedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.DataPrevisaoSaida = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataPrevisaoDeSaida.getFieldDescription()), getType: typesKnockout.dateTime, required: ko.observable(false), issue: 2, visible: ko.observable(false) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataPrevisaoDeEntrega.getFieldDescription()), getType: typesKnockout.dateTime, required: ko.observable(false), issue: 2, visible: ko.observable(false) });
    this.DataAgendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataDeAgendamento.getFieldDescription()), getType: typesKnockout.dateTime, issue: 0, enable: ko.observable(true), visible: ko.observable(false) });

    this.MontagemContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.MontagemContainer.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroBooking.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 150, getType: typesKnockout.string });
    this.TaraContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Tara.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100, getType: typesKnockout.int });
    this.LacreContainerUm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerUm.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.LacreContainerDois = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerDois.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.LacreContainerTres = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerTres.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.ContainerTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoContainer.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ContainerTipoReserva = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoContainerReserva.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false), enable: ko.observable(false) });
    this.NumeroPallets = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPalletsControlePallets, required: false, visible: ko.observable(true), getType: typesKnockout.int, def: ko.observable(0), val: ko.observable(0) });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoInterna.getFieldDescription(), required: false, visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? true : false)), getType: typesKnockout.string });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoVinculadoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.ConsultarContainerEMP = PropertyEntity({ eventClick: consultarContainerEMPClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.ConsultarContainerEMP), visible: ko.observable(false) });
    this.ConsultarContainerOSMae = PropertyEntity({ eventClick: consultarContainerOSMaeClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.ConsultarContainerOSMae), visible: ko.observable(false) });
    this.CTes = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visibleFade: ko.observable(false) });

    this.TipoPedidoVinculado.val.subscribe(function (novoValor) {
        tipoPedidoVinculadoChange(novoValor);
    });

    this.dispose = function () {
        RunObjectDispose(this);
    }
}

//*******EVENTOS*******

function tipoPedidoVinculadoChange(tipoPedidoVinculado) {
    if (tipoPedidoVinculado == EnumTipoPedidoVinculado.EncaixePedidoSubContratacao) {
        ConfigurarPedidoVinculado_EncaixePedidoSubContratacao();
    } else if (tipoPedidoVinculado == EnumTipoPedidoVinculado.EncaixeSubContratacao) {
        ConfigurarPedidoVinculado_EncaixeSubContratacao();
    } else if (tipoPedidoVinculado == EnumTipoPedidoVinculado.Normal || tipoPedidoVinculado == EnumTipoPedidoVinculado.Subcontratacao) {
        ConfigurarPedidoVinculado_NormalOuSubcontratacao();
    }
}

function loadPedidoVinculado() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _cargaAtual.PossuiMontagemContainer.val()) {
        _tipoPedidoVinculado = [
            { text: Localization.Resources.Cargas.Carga.Normal, value: EnumTipoPedidoVinculado.Normal },
            { text: Localization.Resources.Cargas.Carga.Subcontratacao, value: EnumTipoPedidoVinculado.Subcontratacao }
        ];

        _padraoTipoPedidoVinculado = EnumTipoPedidoVinculado.Normal;

    }

    if (_pedidoVinculado != null)
        _pedidoVinculado.dispose();

    _pedidoVinculado = new PedidoVinculado();
    KoBindings(_pedidoVinculado, "knoutModalAdicionarPedidoVinculado");

    new BuscarCargaParaEncaixeDeSubcontratacao(_pedidoVinculado.Carga, retornoCargaPedidoVinculado);
    new BuscarCargaPedidoParaEncaixeDeSubcontratacao(_pedidoVinculado.CargaPedidos, null, retornoMultiplasCargasPedido);
    new BuscarCargaPedidoParaEncaixeDeSubcontratacao(_pedidoVinculado.CargaPedido, retornoCargaPedido);

    new BuscarClientes(_pedidoVinculado.Expedidor);
    new BuscarClientes(_pedidoVinculado.Remetente);
    new BuscarClientes(_pedidoVinculado.Destinatario);
    new BuscarClientes(_pedidoVinculado.Recebedor);
    new BuscarContainers(_pedidoVinculado.Container, retornoContainerPedidoVinculado, null, true);
    new BuscarClienteOutroEndereco(_pedidoVinculado.EnderecoDestinatario, null, _pedidoVinculado.Destinatario);
    new BuscarClienteOutroEndereco(_pedidoVinculado.EnderecoRecebedor, null, _pedidoVinculado.Recebedor);
    new BuscarClienteOutroEndereco(_pedidoVinculado.EnderecoExpedidor, null, _pedidoVinculado.Expedidor);
    new BuscarTiposContainer(_pedidoVinculado.ContainerTipo);
    new BuscarMontagemContainer(_pedidoVinculado.MontagemContainer, retornoMontagemContainer, EnumStatusMontagemContainer.Finalizado);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pedidoVinculado.DataPrevisaoSaida.visible(true);
        _pedidoVinculado.DataPrevisaoEntrega.visible(true);
        _pedidoVinculado.DataAgendamento.visible(true);

        if (_cargaAtual.TipoOperacao != null && _cargaAtual.TipoOperacao != undefined && _cargaAtual.TipoOperacao.naoPermiteAvancarCargaSemDataPrevisaoDeEntrega) {
            _pedidoVinculado.DataPrevisaoSaida.required(true);
            _pedidoVinculado.DataPrevisaoEntrega.required(true);
            _pedidoVinculado.DataPrevisaoSaida.text(Localization.Resources.Cargas.Carga.DataPrevisaoDeSaida.getRequiredFieldDescription());
            _pedidoVinculado.DataPrevisaoEntrega.text(Localization.Resources.Cargas.Carga.DataPrevisaoEntrega.getRequiredFieldDescription());
        }
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || _cargaAtual.ObrigatorioVincularContainerCarga.val()) {
        _pedidoVinculado.ContainerTipo.visible(true);
        _pedidoVinculado.ContainerTipoReserva.visible(true);
        _pedidoVinculado.Container.visible(true);
        _pedidoVinculado.TaraContainer.visible(true);
        _pedidoVinculado.LacreContainerUm.visible(true);
        _pedidoVinculado.LacreContainerDois.visible(true);
        _pedidoVinculado.LacreContainerTres.visible(true);
        _pedidoVinculado.DataPrevisaoSaida.visible(false);
        _pedidoVinculado.DataPrevisaoEntrega.visible(false);
        _pedidoVinculado.DataAgendamento.visible(false);
    }

    _gridCargaCTeEncaixe = new GridView(_pedidoVinculado.CTes.idGrid, "CargaCTe/ConsultarCargaCTe", _pedidoVinculado);

    tipoPedidoVinculadoChange(_padraoTipoPedidoVinculado);
    //_gridCargaCTeEncaixe.CarregarGrid();
}

function retornoContainerPedidoVinculado(data) {
    _pedidoVinculado.Container.codEntity(data.Codigo);
    _pedidoVinculado.Container.val(data.Descricao);
    if (data.Tara !== null && data.Tara !== undefined && data.Tara !== "")
        _pedidoVinculado.TaraContainer.val(data.Tara);
}

function retornoMontagemContainer(data) {
    _pedidoVinculado.MontagemContainer.codEntity(data.Codigo);
    _pedidoVinculado.MontagemContainer.val(data.Descricao);

    if (data.ContainerCodigo > 0) {
        _pedidoVinculado.Container.codEntity(data.ContainerCodigo);
        _pedidoVinculado.Container.val(data.ContainerDescricao);
    }
}

function retornoCargaPedidoVinculado(carga) {
    _pedidoVinculado.Carga.codEntity(carga.Codigo);
    _pedidoVinculado.Carga.val(carga.OrigemDestino);
    _pedidoVinculado.CargaPedidos.codEntity(0);
    _pedidoVinculado.CargaPedidos.multiplesEntities([]);
    _pedidoVinculado.CargaPedidos.val("");
    _gridCargaCTeEncaixe.CarregarGrid(retornoCTesEncaixe);

}

function retornoCargaPedido(cargaPedido) {
    _pedidoVinculado.CargaPedido.codEntity(cargaPedido.Codigo);
    _pedidoVinculado.CargaPedido.val(cargaPedido.CodigoPedidoEmbarcador + " - " + cargaPedido.Destinatario);
    _pedidoVinculado.Carga.codEntity(0);
    _pedidoVinculado.Carga.val("");
    _pedidoVinculado.CargaPedidos.multiplesEntities([]);
    _gridCargaCTeEncaixe.CarregarGrid(retornoCTesEncaixe);
}

function retornoMultiplasCargasPedido() {
    _pedidoVinculado.Carga.codEntity(0);
    _pedidoVinculado.Carga.val("");
    _pedidoVinculado.CargaPedido.codEntity(0);
    _pedidoVinculado.CargaPedido.val("");
    _gridCargaCTeEncaixe.CarregarGrid(retornoCTesEncaixe);
}

function retornoCTesEncaixe() {
    _pedidoVinculado.CTes.visibleFade(true);
}

function adicionarPedidoVinculadoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_pedidoVinculado)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }
    Salvar(e, "PedidoVinculado/AdicionarPedidoVinculado", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                veririficarSeCargaMudouTipoContratacao(arg.Data.Carga);
                if (_cargaDadosEmissaoGeral != null) {
                    preencherTabsPedidos(_cargaDadosEmissaoGeral.Pedido.idTab, "carregarDadosEmissaoGeral", _indiceGlobalPedidoDadosEmissao, _cargaDadosEmissaoGeral.Pedido.enable(), _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos, visualizarAtualizacaoNumeroPedido);
                } else if (_cargaDadosEmissaoConfiguracao != null) {
                    preencherTabsPedidos(_cargaDadosEmissaoConfiguracao.Pedido.idTab, "carregarDadosEmissaoConfiguracao", _indiceGlobalPedidoConfiguracaoEmissao, _cargaDadosEmissaoConfiguracao.Pedido.enable(), _cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos);
                } else if (_documentoEmissao != null) {
                    preencherTabsPedidos(_documentoEmissao.Pedido.idTab, "carregarDocumentosParaEmissaoPedido", _indiceGlobalDocumentos);
                }
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Sucesso);
                limparCamposPedidoVinculado();
                carregarGridDocumentosParaEmissao();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);

}

function cancelarPedidosSemNota() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaCancelarTodosOsPedidosQueNaoPossuemDocumentosVinculados, function () {
        executarReST("PedidoVinculado/ExcluirPedidosSemDocumento", { Codigo: _cargaAtual.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    veririficarSeCargaMudouTipoContratacao(retorno.Data.Carga);

                    if (_cargaDadosEmissaoGeral != null) {
                        preencherTabsPedidos(_cargaDadosEmissaoGeral.Pedido.idTab, "carregarDadosEmissaoGeral", _indiceGlobalPedidoDadosEmissao, _cargaDadosEmissaoGeral.Pedido.enable(), _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos, visualizarAtualizacaoNumeroPedido);
                    } else if (_cargaDadosEmissaoConfiguracao != null) {
                        preencherTabsPedidos(_cargaDadosEmissaoConfiguracao.Pedido.idTab, "carregarDadosEmissaoConfiguracao", _indiceGlobalPedidoConfiguracaoEmissao, _cargaDadosEmissaoConfiguracao.Pedido.enable(), _cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos);
                    } else if (_documentoEmissao != null) {
                        preencherTabsPedidos(_documentoEmissao.Pedido.idTab, "carregarDocumentosParaEmissaoPedido", _indiceGlobalDocumentos);
                    }

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PedidosRemovidosComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        });
    });
}

function abrirModalAdicionarNovoPedido() {
    _pedidoVinculado.CTes.visibleFade(false);
    retornarDadosCargaPedido(0, function (pedido) {
        limparCamposPedidoVinculado();

        _pedidoVinculado.CargaOrigem.val(_cargaAtual.Codigo.val());

        if (pedido.Expedidor != null) {
            _pedidoVinculado.Expedidor.val(pedido.Expedidor.Descricao);
            _pedidoVinculado.Expedidor.codEntity(pedido.Expedidor.Codigo);
        } else {
            _pedidoVinculado.Expedidor.val(pedido.Remetente.Descricao);
            _pedidoVinculado.Expedidor.codEntity(pedido.Remetente.Codigo);
        }

        _pedidoVinculado.NumeroBooking.val(pedido.NumeroBooking);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {

            _pedidoVinculado.Expedidor.val(pedido.Expedidor.Descricao + " (" + pedido.Expedidor.CPF_CNPJ + ")");
            _pedidoVinculado.Expedidor.codEntity(pedido.Expedidor.Codigo);

            _pedidoVinculado.Remetente.val(pedido.Remetente.Descricao + " (" + pedido.Remetente.CPF_CNPJ + ")");
            _pedidoVinculado.Remetente.codEntity(pedido.Remetente.Codigo);

            _pedidoVinculado.Destinatario.val(pedido.Cliente.Descricao + " (" + pedido.Cliente.CPF_CNPJ + ")");
            _pedidoVinculado.Destinatario.codEntity(pedido.Cliente.Codigo);

            _pedidoVinculado.EnderecoDestinatario.codEntity(pedido.EnderecoDestinatario.Codigo);
            _pedidoVinculado.EnderecoDestinatario.val(pedido.EnderecoDestinatario.Descricao);

            _pedidoVinculado.EnderecoRecebedor.codEntity(pedido.EnderecoRecebedor.Codigo);
            _pedidoVinculado.EnderecoRecebedor.val(pedido.EnderecoRecebedor.Descricao);

            _pedidoVinculado.EnderecoExpedidor.codEntity(pedido.EnderecoExpedidor.Codigo);
            _pedidoVinculado.EnderecoExpedidor.val(pedido.EnderecoExpedidor.Descricao);
        }

        Global.abrirModal("divModalAdicionarPedidoVinculado");
    });
}

function limparCamposPedidoVinculado() {
    _pedidoVinculado.Adicionar.text(Localization.Resources.Gerais.Geral.Adicionar);
    $("#tituloAdicionarPedidoVinculado").text(Localization.Resources.Cargas.Carga.AdicionarNovoPedido);
    Global.fecharModal("divModalAdicionarPedidoVinculado");
    tipoPedidoVinculadoChange(_padraoTipoPedidoVinculado);
    LimparCampos(_pedidoVinculado);
}

function EditarPedidoAdicionadoManualmente(event, index) {
    event.stopPropagation();
    event.preventDefault();

    _pedidoVinculado.CTes.visibleFade(false);
    retornarDadosCargaPedido(index, function (pedido) {
        limparCamposPedidoVinculado();

        _pedidoVinculado.Adicionar.text(Localization.Resources.Gerais.Geral.Atualizar);
        $("#tituloAdicionarPedidoVinculado").text(Localization.Resources.Cargas.Carga.AtualizarPedido + " - " + pedido.NumeroPedidoEmbarcador);

        _pedidoVinculado.AtualizarPedido.val(true);
        _pedidoVinculado.CargaPedido.val(pedido.CodigoCargaPedido);
        _pedidoVinculado.CodigoPedido.val(pedido.Codigo);
        _pedidoVinculado.CargaOrigem.val(_cargaAtual.Codigo.val());

        _pedidoVinculado.NumeroPedido.val(pedido.NumeroPedidoEmbarcador);
        _pedidoVinculado.NumeroBooking.val(pedido.NumeroBooking);

        if (pedido.Expedidor != null) {
            _pedidoVinculado.Expedidor.val(pedido.Expedidor.Descricao);
            _pedidoVinculado.Expedidor.codEntity(pedido.Expedidor.Codigo);
        } else {
            _pedidoVinculado.Expedidor.val(pedido.Remetente.Descricao);
            _pedidoVinculado.Expedidor.codEntity(pedido.Remetente.Codigo);
        }

        _pedidoVinculado.Recebedor.val(pedido.Recebedor.Descricao);
        _pedidoVinculado.Recebedor.codEntity(pedido.Recebedor.Codigo);

        _pedidoVinculado.NumeroPallets.val(pedido.numeroPaletes);
        _pedidoVinculado.ObservacaoInterna.val(pedido.observacaoInterna);
        _pedidoVinculado.ConsultarContainerEMP.visible(pedido.HabilitarConsultaContainerEMP);
        _pedidoVinculado.ConsultarContainerOSMae.visible(pedido.HabilitarConsultaContainerEMP);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _pedidoVinculado.TipoPedidoVinculado.visible(false);
            _pedidoVinculado.Carga.visible(false);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {

            _pedidoVinculado.Expedidor.val(pedido.Expedidor.Descricao + " (" + pedido.Expedidor.CPF_CNPJ + ")");
            _pedidoVinculado.Expedidor.codEntity(pedido.Expedidor.Codigo);

            _pedidoVinculado.Remetente.val(pedido.Remetente.Descricao + " (" + pedido.Remetente.CPF_CNPJ + ")");
            _pedidoVinculado.Remetente.codEntity(pedido.Remetente.Codigo);

            _pedidoVinculado.Destinatario.val(pedido.Cliente.Descricao + " (" + pedido.Cliente.CPF_CNPJ + ")");
            _pedidoVinculado.Destinatario.codEntity(pedido.Cliente.Codigo);

            _pedidoVinculado.Container.val(pedido.Container.Descricao);
            _pedidoVinculado.Container.codEntity(pedido.Container.Codigo);

            _pedidoVinculado.ContainerTipo.val(pedido.ContainerTipo.Descricao);
            _pedidoVinculado.ContainerTipo.codEntity(pedido.ContainerTipo.Codigo);

            _pedidoVinculado.ContainerTipoReserva.val(pedido.ContainerTipoReserva.Descricao);
            _pedidoVinculado.ContainerTipoReserva.codEntity(pedido.ContainerTipoReserva.Codigo);

            _pedidoVinculado.TaraContainer.val(pedido.TaraContainer);
            _pedidoVinculado.LacreContainerUm.val(pedido.LacreContainerUm);
            _pedidoVinculado.LacreContainerDois.val(pedido.LacreContainerDois);
            _pedidoVinculado.LacreContainerTres.val(pedido.LacreContainerTres);

            _pedidoVinculado.DataPrevisaoSaida.val(pedido.DataPrevisaoSaida);
            _pedidoVinculado.DataPrevisaoEntrega.val(pedido.DataPrevisaoEntrega);
            _pedidoVinculado.DataAgendamento.val(pedido.DataAgendamento);

            _pedidoVinculado.EnderecoDestinatario.codEntity(pedido.EnderecoDestinatario.Codigo);
            _pedidoVinculado.EnderecoDestinatario.val(pedido.EnderecoDestinatario.Descricao);

            _pedidoVinculado.EnderecoRecebedor.codEntity(pedido.EnderecoRecebedor.Codigo);
            _pedidoVinculado.EnderecoRecebedor.val(pedido.EnderecoRecebedor.Descricao);

            _pedidoVinculado.EnderecoExpedidor.codEntity(pedido.EnderecoExpedidor.Codigo);
            _pedidoVinculado.EnderecoExpedidor.val(pedido.EnderecoExpedidor.Descricao);
        }

        Global.abrirModal("divModalAdicionarPedidoVinculado");
    });
}

function ExcluirPedidoAdicionadoManualmente(event, index) {
    event.stopPropagation();
    event.preventDefault();

    var pedidoExcluir = _cargaAtual.Pedidos.val[index];

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirPedido.format(pedidoExcluir.NumeroPedidoEmbarcador), function () {
        executarReST("PedidoVinculado/ExcluirPedidoVinculado", { CodigoCargaPedido: pedidoExcluir.CodigoCargaPedido }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    veririficarSeCargaMudouTipoContratacao(r.Data.Carga);

                    if (_cargaDadosEmissaoGeral != null) {
                        preencherTabsPedidos(_cargaDadosEmissaoGeral.Pedido.idTab, "carregarDadosEmissaoGeral", _indiceGlobalPedidoDadosEmissao, _cargaDadosEmissaoGeral.Pedido.enable(), _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos, visualizarAtualizacaoNumeroPedido);
                    } else if (_cargaDadosEmissaoConfiguracao != null) {
                        preencherTabsPedidos(_cargaDadosEmissaoConfiguracao.Pedido.idTab, "carregarDadosEmissaoConfiguracao", _indiceGlobalPedidoConfiguracaoEmissao, _cargaDadosEmissaoConfiguracao.Pedido.enable(), _cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos);
                    } else if (_documentoEmissao != null) {
                        preencherTabsPedidos(_documentoEmissao.Pedido.idTab, "carregarDocumentosParaEmissaoPedido", _indiceGlobalDocumentos);
                    }

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PedidoRemovidoComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg, 200000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function consultarContainerEMPClick() {
    executarReST("PedidoVinculado/ConsultarContainerEMP", { Codigo: _pedidoVinculado.CargaPedido.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherDadosContainer(retorno.Data);

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DadosContainerObtidosSucesso);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
    });
}

function consultarContainerOSMaeClick() {
    executarReST("PedidoVinculado/ConsultarContainerOSMae", { Codigo: _pedidoVinculado.CargaPedido.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherDadosContainer(retorno.Data);

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DadosContainerObtidosSucesso);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
    });
}

//*******EVENTOS*******

function ConfigurarPedidoVinculado_EncaixePedidoSubContratacao() {
    _pedidoVinculado.Carga.visible(false);
    _pedidoVinculado.CargaPedido.visible(false);
    _pedidoVinculado.CargaPedidos.visible(true);
    _pedidoVinculado.Expedidor.visible(true);
    _pedidoVinculado.Destinatario.visible(false);
    _pedidoVinculado.Remetente.visible(false);
    _pedidoVinculado.NumeroPedido.visible(false);
    _pedidoVinculado.CTes.visibleFade(false);

    _pedidoVinculado.NumeroPedido.required = false;
    _pedidoVinculado.Destinatario.required = false;
    _pedidoVinculado.Remetente.required = false;

    _pedidoVinculado.TaraContainer.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerUm.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerDois.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerTres.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.NumeroBooking.visible(false);
    _pedidoVinculado.Container.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.MontagemContainer.visible(false);
}

function ConfigurarPedidoVinculado_EncaixeSubContratacao() {
    _pedidoVinculado.Carga.visible(true);
    _pedidoVinculado.Expedidor.visible(true);
    _pedidoVinculado.CargaPedido.visible(false);
    _pedidoVinculado.CargaPedidos.visible(false);
    _pedidoVinculado.Destinatario.visible(false);
    _pedidoVinculado.Remetente.visible(false);
    _pedidoVinculado.NumeroPedido.visible(false);
    _pedidoVinculado.CTes.visibleFade(false);

    _pedidoVinculado.NumeroPedido.required = false;
    _pedidoVinculado.Destinatario.required = false;
    _pedidoVinculado.Remetente.required = false;

    _pedidoVinculado.TaraContainer.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerUm.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerDois.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.LacreContainerTres.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.NumeroBooking.visible(false);
    _pedidoVinculado.Container.visible(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    _pedidoVinculado.MontagemContainer.visible(false);
}

function ConfigurarPedidoVinculado_NormalOuSubcontratacao() {
    _pedidoVinculado.Carga.visible(false);
    _pedidoVinculado.CargaPedido.visible(false);
    _pedidoVinculado.CargaPedidos.visible(false);
    _pedidoVinculado.Expedidor.visible(false);
    _pedidoVinculado.Destinatario.visible(true);
    _pedidoVinculado.Remetente.visible(true);
    _pedidoVinculado.NumeroPedido.visible(true);
    _pedidoVinculado.CTes.visibleFade(false);
    _pedidoVinculado.Recebedor.visible(true);

    _pedidoVinculado.NumeroPedido.required = true;
    _pedidoVinculado.Remetente.required = true;
    _pedidoVinculado.Destinatario.required = true;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pedidoVinculado.EnderecoDestinatario.visible(true);
        _pedidoVinculado.NumeroPedido.required = false;
        _pedidoVinculado.NumeroPedido.text(Localization.Resources.Cargas.Carga.NumeroDoPedidoDoEmbarcador.getFieldDescription());
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) {
        _pedidoVinculado.Expedidor.visible(true);
        _pedidoVinculado.Destinatario.required = false;
        _pedidoVinculado.NumeroPedido.required = false;
        _pedidoVinculado.EnderecoRecebedor.visible(true);
        _pedidoVinculado.EnderecoExpedidor.visible(true);
    }
    else {
        _pedidoVinculado.Destinatario.required = true;
        _pedidoVinculado.EnderecoRecebedor.visible(false);
        _pedidoVinculado.EnderecoExpedidor.visible(false);
    }

    if (_cargaAtual.PossuiMontagemContainer.val() || _cargaAtual.ObrigatorioVincularContainerCarga.val()) {

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
            _pedidoVinculado.NumeroPedido.visible(false);
            _pedidoVinculado.Recebedor.visible(false);
            _pedidoVinculado.Remetente.visible(false);
            _pedidoVinculado.Destinatario.visible(false);

            _pedidoVinculado.Remetente.required = false;
            _pedidoVinculado.Destinatario.required = false;
        }

        _pedidoVinculado.NumeroPedido.required = false;
        _pedidoVinculado.TaraContainer.visible(true);
        _pedidoVinculado.LacreContainerUm.visible(true);
        _pedidoVinculado.LacreContainerDois.visible(true);
        _pedidoVinculado.LacreContainerTres.visible(true);
        _pedidoVinculado.NumeroBooking.visible(true);
        _pedidoVinculado.Container.visible(true);
        _pedidoVinculado.MontagemContainer.visible(true);

    }

}

function PreencherDadosContainer(dados) {
    _pedidoVinculado.Container.val(dados.Container.Descricao);
    _pedidoVinculado.Container.codEntity(dados.Container.Codigo);
    _pedidoVinculado.LacreContainerUm.val(dados.LacreContainerUm);
    _pedidoVinculado.TaraContainer.val(dados.TaraContainer);
}