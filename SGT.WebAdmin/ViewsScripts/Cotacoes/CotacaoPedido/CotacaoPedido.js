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
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAtivoPesquisa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/Prospeccao.js" />
/// <reference path="Adicional.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="Etapa.js" />
/// <reference path="Importacao.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Percurso.js" />
/// <reference path="Resumo.js" />
/// <reference path="Valor.js" />
/// <reference path="Pedidos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _GRUDcotacaoPedido;
var _pesquisaCotacaoPedido;
var _gridCotacaoPedido;
var _cotacaoPedido;

var _SituacaoPesquisa = [{ text: "Aberto", value: EnumSituacaoPedido.Aberto },
{ text: "Cancelado", value: EnumSituacaoPedido.Cancelado },
{ text: "Finalizado", value: EnumSituacaoPedido.Finalizado },
{ text: "Ag. Aprovação", value: EnumSituacaoPedido.AgAprovacao },
{ text: "Rejeitado", value: EnumSituacaoPedido.Rejeitado },
{ text: "Autorização Pendente", value: EnumSituacaoPedido.AutorizacaoPendente },
{ text: "Todos", value: 0 }];

var GRUDCotacaoPedido = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Duplicar = PropertyEntity({ eventClick: DuplicarClick, type: types.event, text: "Duplicar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Limpar", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirClick, type: types.event, text: "Pedido", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "CotacaoPedido/Importar",
        UrlConfiguracao: "CotacaoPedido/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O063_CotacaoPedido,
        CallbackImportacao: function () {
            _gridCotacaoPedido.CarregarGrid();
        }
    });
}

var CotacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoPedido = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.Aberto), def: ko.observable(EnumSituacaoPedido.Aberto) });
    this.DescricaoSituacaoPedido = PropertyEntity();
    this.DescricaoEtapaPedido = PropertyEntity();

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true), required: false, enable: ko.observable(false), val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable(Global.DataAtual()) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.Previsao = PropertyEntity({ text: "*Previsão do Serviço:", getType: typesKnockout.dateTime, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoClienteCotacaoPedido = PropertyEntity({ val: ko.observable(EnumTipoClienteCotacaoPedido.ClienteAtivo), options: EnumTipoClienteCotacaoPedido.obterOpcoes(), def: EnumTipoClienteCotacaoPedido.ClienteAtivo, text: "*Tipo Cliente: ", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ClienteAtivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cliente Ativo:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ClienteInativo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Cliente Inativo:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ClienteNovo = PropertyEntity({ text: "*Novo Cliente: ", maxlength: 2000, required: false, visible: ko.observable(false), enable: ko.observable(true), val: ko.observable("") });
    this.ClienteProspect = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Prospect:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Origem:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Destinatário:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Tipo de Carga:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Solicitante = PropertyEntity({ text: ko.observable("Solicitante: "), maxlength: 2000, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Prospeccao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Prospecção:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailContato = PropertyEntity({ text: "E-mail Contato: ", maxlength: 2000, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TelefoneContato = PropertyEntity({ text: "Telefone Contato: ", maxlength: 100, required: false, visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.phone });
    this.StatusCotacaoPedido = PropertyEntity({ val: ko.observable(EnumStatusCotacaoPedido.EmAnalise), options: EnumStatusCotacaoPedido.obterOpcoes(), def: EnumStatusCotacaoPedido.EmAnalise, text: "*Status: ", required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: "Tomador:", idBtnSearch: guid(), issue: 52, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6"), enable: ko.observable(false) });
    this.UsarTipoTomadorCotacaoPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Usar Tipo Tomador:", def: false, enable: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoTomador.Remetente), options: EnumTipoTomador.obterOpcoesPadrao(), def: EnumTipoTomador.Remetente, text: "Tipo Tomador:", required: false });

    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Recebedor:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Expedidor:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoTomador.val.subscribe(function (novoValor) {
        _cotacaoPedido.Tomador.enable(novoValor == EnumTipoTomador.Outros);
        _cotacaoPedido.Tomador.required = novoValor == EnumTipoTomador.Outros;            
    });

    this.TipoClienteCotacaoPedido.val.subscribe(controlarTipoClienteCotacaoPedido);

    this.AbaAdicional = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AbaImportacao = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AbaDestino = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AbaOrigem = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AbaPercurso = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AbaValor = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
};

var PesquisaCotacaoPedido = function () {

    this.Numero = PropertyEntity({ text: "Número Pedido: ", getType: typesKnockout.int, maxlength: 50, visible: ko.observable(true), required: false, enable: ko.observable(false), val: ko.observable(""), def: ko.observable("") });
    this.DataPrevista = PropertyEntity({ text: "Data prevista ", getType: typesKnockout.date, required: false });
    this.TipoCliente = PropertyEntity({ val: ko.observable(EnumTipoClienteCotacaoPedido.Todos), options: EnumTipoClienteCotacaoPedido.obterOpcoesPesquisa(), def: EnumTipoClienteCotacaoPedido.Todos, text: "Tipo Cliente: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ClienteAtivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Cliente Ativo:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ClienteInativo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Cliente Inativo:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ClienteNovo = PropertyEntity({ text: "Novo Cliente: ", maxlength: 2000, required: false, visible: ko.observable(false), enable: ko.observable(true), val: ko.observable("") });
    this.ClienteProspect = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Prospect:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Destinatário:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Modal = PropertyEntity({ val: ko.observable(EnumTipoModal.Todos), options: EnumTipoModal.obterOpcoesPesquisa(), def: EnumTipoModal.Todos, text: "Tipo Modal: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.StatusCotacaoPedido = PropertyEntity({ val: ko.observable(EnumStatusCotacaoPedido.Todos), options: EnumStatusCotacaoPedido.obterOpcoesPesquisa(), def: EnumStatusCotacaoPedido.Todos, text: "Status: ", required: false, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _SituacaoPesquisa, def: 0, text: "Situação: " });

    this.TipoCliente.val.subscribe(controlarTipoClientePesquisa);

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCotacaoPedido.CarregarGrid();
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
}

//*******EVENTOS*******

function loadCotacaoPedido() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCotacaoPedido = new PesquisaCotacaoPedido();
    KoBindings(_pesquisaCotacaoPedido, "knockoutPesquisaCotacaoPedido", false, _pesquisaCotacaoPedido.Pesquisar.id);

    new BuscarClienteProspect(_pesquisaCotacaoPedido.ClienteProspect);
    new BuscarGruposPessoas(_pesquisaCotacaoPedido.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_pesquisaCotacaoPedido.Destinatario);
    new BuscarTiposdeCarga(_pesquisaCotacaoPedido.TipoDeCarga);
    new BuscarTiposOperacao(_pesquisaCotacaoPedido.TipoOperacao);
    new BuscarClientes(_pesquisaCotacaoPedido.ClienteAtivo, null, null, null, null, null, null, null, null, null, null, null, null, EnumSituacaoAtivoPesquisa.Ativo);
    new BuscarClientes(_pesquisaCotacaoPedido.ClienteInativo, null, null, null, null, null, null, null, null, null, null, null, null, EnumSituacaoAtivoPesquisa.Inativo);

    // Instancia objeto principal
    _cotacaoPedido = new CotacaoPedido();
    KoBindings(_cotacaoPedido, "knockoutCadastroCotacaoPedido");

    new BuscarClienteProspect(_cotacaoPedido.ClienteProspect);
    new BuscarGruposPessoas(_cotacaoPedido.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLocalidades(_cotacaoPedido.Origem, null, null, retornoConsultaOrigem);
    new BuscarLocalidades(_cotacaoPedido.Destino, null, null, retornoConsultaDestino);
    new BuscarClientes(_cotacaoPedido.Destinatario, retornoDestinatario);
    new BuscarProdutos(_cotacaoPedido.Produto);
    new BuscarModelosVeicularesCarga(_cotacaoPedido.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_cotacaoPedido.TipoDeCarga);
    new BuscarTiposOperacao(_cotacaoPedido.TipoOperacao);
    new BuscarProspeccao(_cotacaoPedido.Prospeccao, retornoProspeccao);
    new BuscarClientes(_cotacaoPedido.ClienteAtivo, retornoClienteAtivo, null, null, null, null, null, null, null, null, null, null, null, EnumSituacaoAtivoPesquisa.Ativo);
    new BuscarClientes(_cotacaoPedido.ClienteInativo, retornoClienteInativo, null, null, null, null, null, null, null, null, null, null, null, EnumSituacaoAtivoPesquisa.Inativo);
    new BuscarClientes(_cotacaoPedido.Tomador, RetornoTomador);
    new BuscarClientes(_cotacaoPedido.Recebedor);
    new BuscarClientes(_cotacaoPedido.Expedidor);
    _cotacaoPedido.Tomador.enable(_cotacaoPedido.TipoTomador == EnumTipoTomador.Outros);

    HeaderAuditoria("CotacaoPedido", _cotacaoPedido);

    _GRUDcotacaoPedido = new GRUDCotacaoPedido();
    KoBindings(_GRUDcotacaoPedido, "knockoutCRUDCotacaoPedido");

    loadResumoCotacaoPedido();
    loadEtapaCotacaoPedido();
    loadCotacaoPedidoAutorizacao();
    loadOrigemDestino();
    loadCotacaoPedidoAdicional();
    loadCotacaoPedidoValor();
    loadCotacaoPedidoPercurso();
    loadImportacao();
    loadCotacaoPedidoPedidos();

    ControlarCamposObrigatoriosCotacaoPedido();

    // Inicia busca
    buscarDadosCotacaoPedido();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteAlterarTipoTomador, _PermissoesPersonalizadas))
            _cotacaoPedido.UsarTipoTomadorCotacaoPedido.enable(false);
    }   

    ControlaCampoValorFrete();
}

function adicionarClick(e, sender) {
    ControlarCamposObrigatoriosAdicional();
    if (VerificarDadosAdicionais()) {
        PreecherAbasCadastro();
        Salvar(_cotacaoPedido, "CotacaoPedido/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridCotacaoPedido.CarregarGrid();
                    limparCamposCotacaoPedido();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function DuplicarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja duplicar a cotação selecionada?", function () {
        _GRUDcotacaoPedido.Atualizar.visible(false);
        _GRUDcotacaoPedido.Duplicar.visible(false);
        _GRUDcotacaoPedido.Cancelar.visible(true);
        _GRUDcotacaoPedido.Excluir.visible(false);
        _GRUDcotacaoPedido.Adicionar.visible(true);
        _GRUDcotacaoPedido.Imprimir.visible(false);

        buscarDadosCotacaoPedido();
        setarEtapaInicioCotacaoPedido();
        HabilitarCamposCotacaoPedido();
        resetarAbas();

        _cotacaoPedido.Codigo.val(0);
        _cotacaoPedido.StatusCotacaoPedido.val(EnumStatusCotacaoPedido.EmAnalise);

        _resumoCotacaoPedido.NumeroCotacaoPedido.visible(false);
        _resumoCotacaoPedido.Situacao.val("Aberto");
        _resumoCotacaoPedido.Status.val("Em Analise");
    });
}

function atualizarClick(e, sender) {
    ControlarCamposObrigatoriosAdicional();
    if (VerificarDadosAdicionais()) {
        PreecherAbasCadastro();
        Salvar(_cotacaoPedido, "CotacaoPedido/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridCotacaoPedido.CarregarGrid();
                    limparCamposCotacaoPedido();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_cotacaoPedido, "CotacaoPedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCotacaoPedido.CarregarGrid();
                    limparCamposCotacaoPedido();
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
    limparCamposCotacaoPedido();
}

function imprimirClick(e, sender) {
    var data = { Codigo: _cotacaoPedido.Codigo.val() };
    executarReST("CotacaoPedido/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}

function editarCotacaoPedidoClick(itemGrid) {
    // Limpa os campos
    limparCamposCotacaoPedido();

    // Seta o codigo do objeto
    _cotacaoPedido.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_cotacaoPedido, "CotacaoPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCotacaoPedido.ExibirFiltros.visibleFade(false);

                var dataPreencher = { Data: arg.Data.Resumo };
                preecherResumoCotacaoPedido(dataPreencher);

                dataPreencher = { Data: arg.Data.AbaAdicional };
                PreencherObjetoKnout(_cotacaoPedidoAdicional, dataPreencher);
                _cotacaoPedidoAdicional.Cubagens.list = new Array();
                _cotacaoPedidoAdicional.Cubagens.list = arg.Data.AbaAdicional.Cubagens;
                recarregarGridCubagens();
                controlaCamposPedidoFracionado();

                //dataPreencher = { Data: arg.Data.DadosAutorizacao };
                //PreencherObjetoKnout(_pedidoAutorizacao, dataPreencher);
                preecherCotacaoPedidoAutorizacao(arg.Data.DadosAutorizacao);

                dataPreencher = { Data: arg.Data.AbaImportacao };
                PreencherObjetoKnout(_importacao, dataPreencher);
                _importacao.GridDI.list = new Array();
                _importacao.GridDI.list = arg.Data.AbaImportacao.GridDI;
                recarregarGridDI();

                dataPreencher = { Data: arg.Data.AbaOrigem };
                PreencherObjetoKnout(_localidadeOrigem, dataPreencher);
                preecherDescricaoEnderecoOrigemUtilizado(dataPreencher.Data);

                if (_cotacaoPedido.ClienteAtivo !== null && _cotacaoPedido.ClienteAtivo.codEntity() > 0) {
                    preecherLocalidadeCliente(_cotacaoPedido.ClienteAtivo, _localidadeOrigem, "R");
                    _localidadeOrigem.Cliente.visible(true);
                } else if (_cotacaoPedido.ClienteInativo !== null && _cotacaoPedido.ClienteInativo.codEntity() > 0) {
                    preecherLocalidadeCliente(_cotacaoPedido.ClienteInativo, _localidadeOrigem, "R");
                    _localidadeOrigem.Cliente.visible(true);
                } else {
                    _localidadeOrigem.Cliente.visible(false);
                }

                dataPreencher = { Data: arg.Data.AbaDestino };
                PreencherObjetoKnout(_localidadeDestino, dataPreencher);
                preecherDescricaoEnderecoDestinoUtilizado(dataPreencher.Data);

                if (_cotacaoPedido.Destinatario !== null && _cotacaoPedido.Destinatario.codEntity() > 0) {
                    preecherLocalidadeCliente(_cotacaoPedido.Destinatario, _localidadeDestino, "D");
                    _localidadeDestino.Cliente.visible(true);
                } else {
                    _localidadeDestino.Cliente.visible(false);
                }

                dataPreencher = { Data: arg.Data.AbaPercurso };
                PreencherObjetoKnout(_cotacaoPedidoPercurso, dataPreencher);

                dataPreencher = { Data: arg.Data.AbaValor };
                PreencherObjetoKnout(_cotacaoPedidoValor, dataPreencher);
                _cotacaoPedidoValor.Componentes.list = new Array();
                _cotacaoPedidoValor.Componentes.list = arg.Data.AbaValor.Componentes;
                recarregarGridComponentes();

                // Alternas os campos de CRUD
                _GRUDcotacaoPedido.Atualizar.visible(true);
                _GRUDcotacaoPedido.Excluir.visible(true);
                _GRUDcotacaoPedido.Cancelar.visible(true);
                _GRUDcotacaoPedido.Adicionar.visible(false);
                _GRUDcotacaoPedido.Duplicar.visible(true);
                
                // Libera a visualização da aba Pedidos
                if (arg.Data.ConfiguracoesTipoOperacao.HabilitaInformarDadosDosPedidosNaContacao &&
                    arg.Data.SituacaoPedido == EnumSituacaoPedido.Finalizado &&
                    _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                    $("#liTabPedidos").show();
                    ExibirDetalhesPedidosFracionados();
                } else {
                    $("#liTabPedidos").hide();
                }

                setarEtapasCotacaoPedido();
                carregarDadosMapaPercursoCotacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function retornoProspeccao(data) {
    _cotacaoPedido.Prospeccao.val(data.Descricao);
    _cotacaoPedido.Prospeccao.codEntity(data.Codigo);
}

function buscarCotacaoPedido() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCotacaoPedidoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
        var configExportacao = {
            url: "CotacaoPedido/ExportarPesquisa",
            titulo: "Cotação de Pedido"
        };
    }

    // Inicia Grid de busca
    _gridCotacaoPedido = new GridViewExportacao(_pesquisaCotacaoPedido.Pesquisar.idGrid, "CotacaoPedido/Pesquisa", _pesquisaCotacaoPedido, menuOpcoes, configExportacao, null);
    _gridCotacaoPedido.CarregarGrid();
}

function buscarDadosCotacaoPedido() {
    executarReST("CotacaoPedido/BuscarDadosCotacaoPedido", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data !== null) {
                _cotacaoPedido.Usuario.val(arg.Data.Nome);
                _cotacaoPedido.Usuario.codEntity(arg.Data.Codigo);
                _cotacaoPedido.Numero.val(arg.Data.ProximoNumero);
                _cotacaoPedido.Data.val(Global.DataAtual());
                buscarCotacaoPedido();
            }
        }
    });
}

function PreecherAbasCadastro() {
    _cotacaoPedidoAdicional.ListaCubagem.val(JSON.stringify(_cotacaoPedidoAdicional.Cubagens.list));
    _cotacaoPedido.AbaAdicional.val(JSON.stringify(RetornarObjetoPesquisa(_cotacaoPedidoAdicional)));

    _importacao.ListaDI.val(JSON.stringify(_importacao.GridDI.list));
    _cotacaoPedido.AbaImportacao.val(JSON.stringify(RetornarObjetoPesquisa(_importacao)));

    _cotacaoPedido.AbaDestino.val(JSON.stringify(RetornarObjetoPesquisa(_localidadeDestino)));
    _cotacaoPedido.AbaOrigem.val(JSON.stringify(RetornarObjetoPesquisa(_localidadeOrigem)));
    _cotacaoPedido.AbaPercurso.val(JSON.stringify(RetornarObjetoPesquisa(_cotacaoPedidoPercurso)));

    _cotacaoPedidoValor.ListaComponente.val(JSON.stringify(_cotacaoPedidoValor.Componentes.list));
    _cotacaoPedido.AbaValor.val(JSON.stringify(RetornarObjetoPesquisa(_cotacaoPedidoValor)));
}

function controlarTipoClientePesquisa() {
    _pesquisaCotacaoPedido.ClienteAtivo.visible(false);
    _pesquisaCotacaoPedido.ClienteAtivo.required = false;
    LimparCampoEntity(_pesquisaCotacaoPedido.ClienteAtivo);
    _pesquisaCotacaoPedido.ClienteInativo.visible(false);
    _pesquisaCotacaoPedido.ClienteInativo.required = false;
    LimparCampoEntity(_pesquisaCotacaoPedido.ClienteInativo);
    _pesquisaCotacaoPedido.ClienteNovo.visible(false);
    _pesquisaCotacaoPedido.ClienteNovo.required = false;
    _pesquisaCotacaoPedido.ClienteNovo.val("");
    _pesquisaCotacaoPedido.ClienteProspect.visible(false);
    _pesquisaCotacaoPedido.ClienteProspect.required = false;
    LimparCampoEntity(_pesquisaCotacaoPedido.ClienteProspect);
    _pesquisaCotacaoPedido.GrupoPessoas.visible(false);
    _pesquisaCotacaoPedido.GrupoPessoas.required = false;
    LimparCampoEntity(_pesquisaCotacaoPedido.GrupoPessoas);

    if (_pesquisaCotacaoPedido.TipoCliente.val() === EnumTipoClienteCotacaoPedido.ClienteAtivo) {
        _pesquisaCotacaoPedido.ClienteAtivo.visible(true);
    } else if (_pesquisaCotacaoPedido.TipoCliente.val() === EnumTipoClienteCotacaoPedido.ClienteInativo) {
        _pesquisaCotacaoPedido.ClienteInativo.visible(true);
    } else if (_pesquisaCotacaoPedido.TipoCliente.val() === EnumTipoClienteCotacaoPedido.ClienteNovo) {
        _pesquisaCotacaoPedido.ClienteNovo.visible(true);
    } else if (_pesquisaCotacaoPedido.TipoCliente.val() === EnumTipoClienteCotacaoPedido.ClienteProspect) {
        _pesquisaCotacaoPedido.ClienteProspect.visible(true);
    } else if (_pesquisaCotacaoPedido.TipoCliente.val() === EnumTipoClienteCotacaoPedido.GrupoPessoa) {
        _pesquisaCotacaoPedido.GrupoPessoas.visible(true);
    }
}

function controlarTipoClienteCotacaoPedido() {

    _localidadeOrigem.Cliente.visible(false);
    _cotacaoPedido.ClienteAtivo.visible(false);
    _cotacaoPedido.ClienteAtivo.required = false;
    LimparCampoEntity(_cotacaoPedido.ClienteAtivo);
    _cotacaoPedido.ClienteInativo.visible(false);
    _cotacaoPedido.ClienteInativo.required = false;
    LimparCampoEntity(_cotacaoPedido.ClienteInativo);
    _cotacaoPedido.ClienteNovo.visible(false);
    _cotacaoPedido.ClienteNovo.required = false;
    _cotacaoPedido.ClienteNovo.val("");
    _cotacaoPedido.ClienteProspect.visible(false);
    _cotacaoPedido.ClienteProspect.required = false;
    LimparCampoEntity(_cotacaoPedido.ClienteProspect);
    _cotacaoPedido.GrupoPessoas.visible(false);
    _cotacaoPedido.GrupoPessoas.required = false;
    LimparCampoEntity(_cotacaoPedido.GrupoPessoas);

    if (_cotacaoPedido.TipoClienteCotacaoPedido.val() === EnumTipoClienteCotacaoPedido.ClienteAtivo) {
        _cotacaoPedido.ClienteAtivo.visible(true);
        _cotacaoPedido.ClienteAtivo.required = true;
    } else if (_cotacaoPedido.TipoClienteCotacaoPedido.val() === EnumTipoClienteCotacaoPedido.ClienteInativo) {
        _cotacaoPedido.ClienteInativo.visible(true);
        _cotacaoPedido.ClienteInativo.required = true;
    } else if (_cotacaoPedido.TipoClienteCotacaoPedido.val() === EnumTipoClienteCotacaoPedido.ClienteNovo) {
        _cotacaoPedido.ClienteNovo.visible(true);
        _cotacaoPedido.ClienteNovo.required = true;
    } else if (_cotacaoPedido.TipoClienteCotacaoPedido.val() === EnumTipoClienteCotacaoPedido.ClienteProspect) {
        _cotacaoPedido.ClienteProspect.visible(true);
        _cotacaoPedido.ClienteProspect.required = true;
    } else if (_cotacaoPedido.TipoClienteCotacaoPedido.val() === EnumTipoClienteCotacaoPedido.GrupoPessoa) {
        _cotacaoPedido.GrupoPessoas.visible(true);
        _cotacaoPedido.GrupoPessoas.required = true;
    }
}

function retornoDestinatario(data) {
    _cotacaoPedido.Destinatario.codEntity(data.Codigo);
    _cotacaoPedido.Destinatario.val(data.Descricao);
    preecherLocalidadeCliente(_cotacaoPedido.Destinatario, _localidadeDestino, "D");
    _localidadeDestino.Cliente.visible(true);
    carregarDadosMapaPercursoCotacao();
}

function retornoClienteAtivo(data) {
    _cotacaoPedido.ClienteAtivo.codEntity(data.Codigo);
    _cotacaoPedido.ClienteAtivo.val(data.Descricao);
    preecherLocalidadeCliente(_cotacaoPedido.ClienteAtivo, _localidadeOrigem, "R");
    _localidadeOrigem.Cliente.visible(true);
    carregarDadosMapaPercursoCotacao();
}

function retornoClienteInativo(data) {
    _cotacaoPedido.ClienteInativo.codEntity(data.Codigo);
    _cotacaoPedido.ClienteInativo.val(data.Descricao);
    preecherLocalidadeCliente(_cotacaoPedido.ClienteInativo, _localidadeOrigem, "R");
    _localidadeOrigem.Cliente.visible(true);
    carregarDadosMapaPercursoCotacao();
}

function limparCamposCotacaoPedido() {
    _GRUDcotacaoPedido.Atualizar.visible(false);
    _GRUDcotacaoPedido.Duplicar.visible(false);
    _GRUDcotacaoPedido.Cancelar.visible(true);
    _GRUDcotacaoPedido.Excluir.visible(false);
    _GRUDcotacaoPedido.Adicionar.visible(true);
    _GRUDcotacaoPedido.Imprimir.visible(false);
    LimparCampos(_cotacaoPedido);
    limparCotacaoPedidoAdicional();
    limparCotacaoPedidoAutorizacao();
    limparDI();
    limparCamposOrigemDestino();
    limparResumoCotacaoPedido();
    limparCamposPercurso();
    buscarDadosCotacaoPedido();
    limparCotacaoPedidoValor();
    setarEtapaInicioCotacaoPedido();
    HabilitarCamposCotacaoPedido();
    resetarAbas();
}

function ControlarCamposObrigatoriosCotacaoPedido() {
    if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido === true) {

        _cotacaoPedido.TipoDeCarga.text("*Tipo de Carga:");
        _cotacaoPedido.TipoDeCarga.required = true;   
        
        _cotacaoPedido.Solicitante.text("*Solicitante:");
        _cotacaoPedido.Solicitante.required = true;
        //_cotacaoPedidoAdicional.Observacao.text("*Observação:");
        //_cotacaoPedidoAdicional.Observacao.required = true;
        //_cotacaoPedidoAdicional.Observacao.visible(true);

        //_cotacaoPedidoAdicional.DataInicialColeta.text("*Previsão de Saída:");
        //_cotacaoPedidoAdicional.DataInicialColeta.required = true;

        //_cotacaoPedidoAdicional.DataFinalColeta.text("*Previsão de Retorno:");
        //_cotacaoPedidoAdicional.DataFinalColeta.required = true;

        //_cotacaoPedidoAdicional.NumeroPaletes.text("*Nº Pallets:");
        //_cotacaoPedidoAdicional.NumeroPaletes.required = true;

        //_cotacaoPedidoAdicional.PesoTotal.text("*Peso Bruto:");
        //_cotacaoPedidoAdicional.PesoTotal.required = true;

        //_cotacaoPedidoAdicional.QtdEntregas.text("*Qtd. Entregas:");
        //_cotacaoPedidoAdicional.QtdEntregas.required = true;

        //_cotacaoPedidoAdicional.TotalPesoCubado.text("*Peso Cubado:");
        //_cotacaoPedidoAdicional.TotalPesoCubado.required = true;        

        //_cotacaoPedidoAdicional.QuantidadeNotas.text("*Qtd. Notas:");
        //_cotacaoPedidoAdicional.QuantidadeNotas.visible(true);
        //_cotacaoPedidoAdicional.QuantidadeNotas.required = true;
    }
}

function resetarAbas() {
    $("#liTabPedidos").hide();
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}

function retornoConsultaDestino(registroSelecionado) {
    _cotacaoPedido.Destino.codEntity(registroSelecionado.Codigo);
    _cotacaoPedido.Destino.val(registroSelecionado.Descricao);
    _cotacaoPedido.Destino.entityDescription(registroSelecionado.Descricao);

    buscarKmTotalPorOrigemEDestino();
}

function retornoConsultaOrigem(registroSelecionado){
    _cotacaoPedido.Origem.codEntity(registroSelecionado.Codigo);
    _cotacaoPedido.Origem.val(registroSelecionado.Descricao);
    _cotacaoPedido.Origem.entityDescription(registroSelecionado.Descricao);

    buscarKmTotalPorOrigemEDestino();

}

function RetornoTomador(data) {
    if (data.GrupoPessoasBloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.GrupoPessoasBloqueadas, Localization.Resources.Pedidos.Pedido.GrupoPessoasClienteBloqueadas.format(data.GrupoPessoasMotivoBloqueio));

        _cotacaoPedido.Tomador.codEntity(0);
        _cotacaoPedido.Tomador.val('');

        return;
    }

    if (data.Bloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.PessoaBloqueada, Localization.Resources.Pedidos.Pedido.ClienteBloqueado.format(data.MotivoBloqueio));

        _cotacaoPedido.Tomador.codEntity(0);
        _cotacaoPedido.Tomador.val('');

        return;
    }

    _cotacaoPedido.Tomador.codEntity(data.Codigo);
    _cotacaoPedido.Tomador.val(data.Descricao); 
}