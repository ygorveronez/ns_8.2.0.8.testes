/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TiposCausadoresOcorrencia.js" />
/// <reference path="../../Consultas/CausasMotivoChamado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="Abertura.js" />
/// <reference path="Analise.js" />
/// <reference path="Devolucao.js" />
/// <reference path="Etapas.js" />
/// <reference path="ResumoChamado.js" />
/// <reference path="SignalR.js" />
/// <reference path="ValePallet.js" />
/// <reference path="Integracao.js" />
/// <reference path="ChamadoNotaFiscal.js" />
/// <reference path="ChatChamado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridChamados;
var _chamado;
var _CRUDChamado;
var _pesquisaChamados;
var _motivoChamadoConfiguracao = {};
var _origemTelaChamado;
var _PermissoesPersonalizadasChamado = null;
var _liberarAssumirChamadoValorAprovado = false;
/*
 * Declaração das Classes
 */

var Chamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CargaDevolucao = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.PossuiResponsavel = PropertyEntity({ val: ko.observable(false), def: false });
    this.PodeEditar = PropertyEntity({ val: ko.observable(false), def: false });
    this.GerarCargaDevolucao = PropertyEntity({ val: ko.observable(false), def: false });
    this.VeiculoCarregado = PropertyEntity({ val: ko.observable(false), def: false });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Responsavel.getFieldDescription() }); // Uso Apenas informativo
    this.Notificado = PropertyEntity({ val: ko.observable(false), def: false });
    this.BloquearFinalizar = PropertyEntity({ val: ko.observable(false), def: false });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoChamado.Todas), def: EnumSituacaoChamado.Todas, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.CargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.PossuiIntegracao = PropertyEntity({ val: ko.observable(false), def: false });
    this.NivelAtendimento = PropertyEntity({ val: ko.observable(EnumEscalationList.Nenhum) });
    this.AguardandoTratativaDoCliente = PropertyEntity({ val: ko.observable(false), def: false });
    this.Estadia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Estadia.getFieldDescription(), val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Nao, visible: ko.observable(true) });
};

var CRUDChamado = function () {
    this.Imprimir = PropertyEntity({ eventClick: ImprimirAtendimentoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Imprimir, visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparChamadoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.LimparGerarNovoAtendimento, idGrid: guid(), visible: ko.observable(false) });
    this.AssumirChamado = PropertyEntity({ eventClick: assumirChamadoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AssumirAtendimento, idGrid: guid(), visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: reabrirClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Reabrir, visible: ko.observable(false) });
};

var PesquisaChamados = function () {
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicialAgendamentoPedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataInicialAgendamentoPedido.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinalAgendamentoPedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataFinalAgendamentoPedido.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicialAgendamentoPedido.dateRangeLimit = this.DataFinalAgendamentoPedido;
    this.DataFinalAgendamentoPedido.dateRangeInit = this.DataInicialAgendamentoPedido;

    this.DataInicialColetaPedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataInicialColetaPedido.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinalColetaPedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataFinalColetaPedido.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicialColetaPedido.dateRangeLimit = this.DataFinalColetaPedido;
    this.DataFinalColetaPedido.dateRangeInit = this.DataInicialColetaPedido;

    this.Carga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroCarga.getFieldDescription(), visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pedido.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.PedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PedidoEmbarcador.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroPedidoCliente.getFieldDescription(), maxlength: 150, required: false });
    this.SomenteCargasCriticas = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Chamado.ChamadoOcorrencia.SomenteCargasCriticas });
    this.NumeroInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroInicial.getFieldDescription(), getType: typesKnockout.string, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NumeroFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroFinal.getFieldDescription(), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NotaFiscal.getFieldDescription(), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NumeroLote = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroLote.getFieldDescription(), getType: typesKnockout.int, val: ko.observable("") });

    this.SomenteAtendimentoComMsgNaoLida = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Chamado.ChamadoOcorrencia.SomenteAtendimentoComMsgNaoLida });

    this.SituacaoChamado = PropertyEntity({ val: ko.observable(EnumSituacaoChamado.Aberto), options: EnumSituacaoChamado.obterOpcoesPesquisa(), def: EnumSituacaoChamado.Aberto, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: ko.observable(true) });
    this.ComOcorrencia = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: Localization.Resources.Cargas.ControleEntrega.ComOcorrencia, visible: ko.observable(true) });
    this.ComDevolucao = PropertyEntity({ val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano("Parcial", "Total"), def: "", text: Localization.Resources.Cargas.ControleEntrega.ComDevolucao, visible: ko.observable(true) });
    this.ComNotaFiscalServico = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ComAnexoNFS, val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "", visible: ko.observable(false) });
    this.ComResponsavel = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ComResponsavel, val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "", visible: ko.observable(true) });
    this.ComNovaMovimentacao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ComNovaMovimentacao, val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "", visible: ko.observable(true) });

    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Setor.getFieldDescription()), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.FilialVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Responsavel.getFieldDescription(), issue: 210, idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivoChamado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motivo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Cliente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Tomador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.GrupoPessoasCliente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.GrupoPessoasTomador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.GrupoPessoasDestinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Ocorrencia.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ClienteComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoPessoaResponsavel = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Cargas.ControleEntrega.TipoPessoaResponsavel.getFieldDescription(), visible: ko.observable(false), eventChange: tipoPessoaResponsavelPesquisaChange });
    this.GrupoPessoasResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.GrupoPessoasResponsavel.getFieldDescription(), visible: ko.observable(false), idBtnSearch: guid() });
    this.ClienteResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.ClienteResponsavel.getFieldDescription(), visible: ko.observable(false), idBtnSearch: guid() });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.CanalVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.SetorEscalationList = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.SetorEscalationList.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Vendedores.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.MesoRegiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Mesoregiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Regiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.UFDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.EscritorioVendas = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.EscritorioVendas.getFieldDescription(), maxlength: 50, required: false });
    this.Matriz = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Matriz.getFieldDescription(), maxlength: 50, required: false });
    this.Parqueada = PropertyEntity({ val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "", text: Localization.Resources.Cargas.ControleEntrega.Parqueada, visible: ko.observable(true) });
    this.TiposCausadoresOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.CausasMotivoChamado = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausasMotivoAtendimento.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarGridChamados();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
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
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function () {
            limparFiltrosChamadoOcorrencia();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadChamado(telaChamado) {
    $.get("Content/Static/Chamado/ModaisChamado.html?dyn=" + guid(), function (htmlModaisChamado) {
        $("#ModaisChamado").html(htmlModaisChamado);
        $.get("Content/Static/Chat/ChatMotorista.html?dyn=" + guid(), function (htmlModal) {
            $("#chatMotorista").html(htmlModal);

            $.get("Content/Static/Chamado/Chamado.html?dyn=" + guid(), function (htmlChamado) {
                $("#conteudo-chamado-ocorrencia").html(htmlChamado);

                _origemTelaChamado = Boolean(telaChamado);
                _chamado = new Chamado();

                _CRUDChamado = new CRUDChamado();
                KoBindings(_CRUDChamado, "knockoutCRUD");

                _CRUDChamado.Limpar.visible(_origemTelaChamado);

                if (_origemTelaChamado) {
                    _pesquisaChamados = new PesquisaChamados();
                    KoBindings(_pesquisaChamados, "knockoutPesquisaChamados", false, _pesquisaChamados.Pesquisar.id);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                        _pesquisaChamados.Filial.visible(false);
                        _pesquisaChamados.FilialVenda.visible(false);
                        _pesquisaChamados.Transportador.text(Localization.Resources.Cargas.ControleEntrega.EmpresaFilial.getFieldDescription());
                        _pesquisaChamados.TipoPessoaResponsavel.visible(true);
                        _pesquisaChamados.ClienteResponsavel.visible(true);
                        _pesquisaChamados.ComNotaFiscalServico.visible(true);
                    }
                    else if (_CONFIGURACAO_TMS.ExigirClienteResponsavelPeloAtendimento)
                        _pesquisaChamados.ClienteResponsavel.visible(true);

                    BuscarClientes(_pesquisaChamados.Cliente);
                    BuscarClientes(_pesquisaChamados.Tomador);
                    BuscarClientes(_pesquisaChamados.Destinatario);
                    BuscarGruposPessoas(_pesquisaChamados.GrupoPessoasCliente);
                    BuscarGruposPessoas(_pesquisaChamados.GrupoPessoasTomador);
                    BuscarGruposPessoas(_pesquisaChamados.GrupoPessoasDestinatario);
                    BuscarFilial(_pesquisaChamados.Filial, null, null, true);
                    BuscarSetorFuncionario(_pesquisaChamados.Setor);
                    BuscarCanaisVenda(_pesquisaChamados.CanalVenda);
                    BuscarFilial(_pesquisaChamados.FilialVenda);
                    BuscarMotivoChamado(_pesquisaChamados.MotivoChamado);
                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
                        BuscarFuncionario(_pesquisaChamados.Responsavel);
                    else
                        BuscarOperador(_pesquisaChamados.Responsavel);
                    BuscarTransportadores(_pesquisaChamados.Transportador);
                    BuscarVeiculos(_pesquisaChamados.Veiculo);
                    BuscarOcorrencias(_pesquisaChamados.Ocorrencia);
                    BuscarMotorista(_pesquisaChamados.Motorista);
                    BuscarClientes(_pesquisaChamados.ClienteResponsavel);
                    BuscarGruposPessoas(_pesquisaChamados.GrupoPessoasResponsavel);
                    BuscarTiposOperacao(_pesquisaChamados.TipoOperacao);
                    BuscarClienteComplementar(_pesquisaChamados.ClienteComplementar)
                    BuscarSetorFuncionario(_pesquisaChamados.SetorEscalationList);
                    BuscarFuncionario(_pesquisaChamados.Vendedores);
                    BuscarMesoRegiao(_pesquisaChamados.MesoRegiao);
                    BuscarRegioes(_pesquisaChamados.Regiao);
                    BuscarEstados(_pesquisaChamados.UFDestino);
                    BuscarTiposCausadoresOcorrencia(_pesquisaChamados.TiposCausadoresOcorrencia);
                    BuscarCausasMotivoChamado(_pesquisaChamados.CausasMotivoChamado);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
                        _pesquisaChamados.Pedido.visible(true);
                        _pesquisaChamados.Carga.visible(false);
                        BuscarPedidos(_pesquisaChamados.Pedido);
                    }

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                        _pesquisaChamados.PedidoEmbarcador.visible(true);
                        _pesquisaChamados.Carga.visible(true);
                        BuscarPedidosEmbarcador(_pesquisaChamados.PedidoEmbarcador, null, null, null);
                    }

                    if (_CONFIGURACAO_TMS.OcultarTomadorNoAtendimento) {
                        _pesquisaChamados.Tomador.visible(false);
                        _pesquisaChamados.GrupoPessoasTomador.visible(false);
                    }

                }

                loadEtapasChamado();
                loadAbertura(_origemTelaChamado);
                loadAnalise();
                LoadResumoChamado();
                LoadCargasChamado();
                LoadValePalletChamado();
                loadChamadoSignalR();
                loadIntegracoesChamado();
                LoadModalResponsaveisAtendimento();
                LoadConexaoSignalRChatNovo();
                obterSeExisteRegraUsuarioResponsavel(_origemTelaChamado);

                if (_origemTelaChamado) {
                    buscarChamados();
                }
            });
        });
    });

}


/*
 * Declaração das Funções Associadas a Eventos
 */

function assumirChamadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaAssumirAtendimentoEleSeraAtualizadoParaSuaResponsabilidade, function () {
        executarReST("ChamadoOcorrencia/AssumirChamado", { Codigo: _chamado.Codigo.val() }, function (arg) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.ChamadoAssumido, Localization.Resources.Cargas.ControleEntrega.ResponsavelPeloChamadoPassaSerVoce);
                _chamado.PodeEditar.val(true);
                _CRUDChamado.AssumirChamado.visible(false);
                AvaliarRegras();
                limparAnaliseClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        }, null);
    });
}

function editarChamadoClick(itemGrid) {
    _pesquisaChamados.ExibirFiltros.visibleFade(false);

    buscarChamadoPorCodigo(itemGrid.Codigo, null);

}

function limparChamadoClick() {
    limparCamposChamado();
}

function ImprimirAtendimentoClick(e, sender) {
    executarDownload("ChamadoOcorrencia/DownloadRelatorioAtendimento", { Codigo: _chamado.Codigo.val() });
}


function reabrirClick() {
    executarReST("ChamadoOcorrencia/ReabrirChamado", { Codigo: _chamado.Codigo.val() }, function (arg) {
        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.ChamadoReaberto, Localization.Resources.Cargas.ControleEntrega.ChamadoAgoraEstaAberto);
            buscarChamadoPorCodigo(_chamado.Codigo.val());
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    }, null);
}

function tipoPessoaResponsavelPesquisaChange() {
    if (_pesquisaChamados.TipoPessoaResponsavel.val() === EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaChamados.ClienteResponsavel.visible(true);
        _pesquisaChamados.GrupoPessoasResponsavel.visible(false);

        LimparCampoEntity(_pesquisaChamados.GrupoPessoasResponsavel);
    }
    else if (_pesquisaChamados.TipoPessoaResponsavel.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaChamados.GrupoPessoasResponsavel.visible(true);
        _pesquisaChamados.ClienteResponsavel.visible(false);

        LimparCampoEntity(_pesquisaChamados.ClienteResponsavel);
    }
}

/*
 * Declaração das Funções Públicas
 */

function buscarChamadoPorCodigo(codigo, callback, tempoExcedido) {
    limparCamposChamado();

    executarReST("ChamadoOcorrencia/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            ObterConfiguracaoMotivoChamado(arg.Data.MotivoChamado, function () {

                editarChamado(arg.Data);
                EditarAbertura(arg.Data);
                CarregarAnexosChamados(arg.Data);
                EditarAnalise(arg.Data);
                PreecherResumoChamado(arg.Data.Abertura.ResumoChamado);
                PreecherResponsaveisAtendimentoNivel(arg.Data.ResponsaveisPorNivel);
                SetarEtapaChamado();
                PreecherAnaliseDevolucao(codigo, arg.Data.CargaEntrega);
                PreecherAberturaDevolucao(codigo, arg.Data.CargaEntrega);
                MostrarJustificativaOcorrencia(arg.Data.PermiteInserirJustificativaOcorrencia);
                _analise.Arvore.visible(arg.Data.MotivoPossuiArvore);
                ControleCamposCliente(!arg.Data.AguardandoTratativaDoCliente);
                AtivarAlertaDeNivelChamado(arg.Data.TempoDoChamadoExpirou);
                AtivarRelogioAlertaDeNivelChamado(arg.Data.DataLimiteTratativa, arg.Data.NivelAtendimento, arg.Data.Situacao);

                _koTabs.ExibirNFeDevolucao.val(_motivoChamadoConfiguracao.PermiteInformarNFD);

                mostrarGridNFeProdutos(_gridNotaFiscal.BuscarRegistros());

                if (callback instanceof Function)
                    callback();


                _pesquisaOcorrenciaNoChamado.Chamado.val(arg.Data.Codigo);
                _pesquisaOcorrenciaNoChamado.Chamado.codEntity(arg.Data.Codigo);
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)
                    _pesquisaOcorrenciaNoChamado.Estornar.enable(!arg.Data.BloquearEstornoAtendimentosFinalizadosPortalTransportador);
                _gridOcorrenciaNoChamado.CarregarGrid();

                if (_motivoChamadoConfiguracao.PossibilitarInclusaoAnexoAoEscalarAtendimento)
                    _analise.ExibirAnexosEscaladas.visible(true);
                else
                    _analise.ExibirAnexosEscaladas.visible(false);


                if (_motivoChamadoConfiguracao.PermitirAtualizarInformacoesPedido)
                    $("#liTabAnaliseAlteracaoInformacao").show();
                else
                    $("#liTabAnaliseAlteracaoInformacao").hide();

                ObterSobrasPorCodigoOcorrencia();
                ValidarCamposVisiveisEtapaAnalise();
                BuscarValorCargaDescarga();
            });
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function ControleCamposCliente(status) {
    _analise.LiberarParaCliente.enable(status);

    if (!status)
        _analise.LiberarParaCliente.text(Localization.Resources.Chamado.ChamadoOcorrencia.LiberadoParaCliente);
    else
        _analise.LiberarParaCliente.text(Localization.Resources.Chamado.ChamadoOcorrencia.LiberarParaCliente);
}

function limparCamposChamado() {
    LimparCampos(_chamado);
    _CRUDChamado.AssumirChamado.visible(false);
    _CRUDChamado.Imprimir.visible(false);
    _CRUDChamado.Reabrir.visible(false);

    SetarEtapaInicioChamado();
    LimparCamposAbertura();
    LimparCamposAnalise();
    LimparResumoChamado();
    limparOcorrenciaAnexosChamados();
    DefinirEtapa2();
    DefinirEtapa3();
    LimparModalResponsaveis();
    Global.ResetarAbas();
}

function ObterConfiguracaoMotivoChamado(codigo, cb) {
    executarReST("MotivoChamado/ConfiguracaoDoMotivo", { Codigo: codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _motivoChamadoConfiguracao = arg.Data;

                if (cb)
                    cb();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function recarregarGridChamados() {
    if (_origemTelaChamado)
        _gridChamados.CarregarGrid();
}

function CriarNovoChamado(dadosAbertura, idModal) {
    _abertura.Carga.val(dadosAbertura.Carga.val());
    _abertura.Carga.codEntity(dadosAbertura.Carga.val());
    _abertura.Carga.enable(false);

    Global.abrirModal(idModal);
}

function CriarNovoAtendimento(dados, idModal) {
    limparCamposChamado();

    _abertura.Carga.val(dados.carga);
    _abertura.Carga.codEntity(dados.codigo);
    _abertura.Carga.enable(false);
    _abertura.CodigoCargaEntrega.val(dados.codigoCargaEntrega);

    TrocaComponentesModalBuscas(false);

    _buscaCliente.CarregarClienteCargaSelecionada();
    _buscaTomador.CarregarTomadorCargaSelecionada();
    _buscaDestinatario.CarregarDestinatarioCargaSelecionada(dados.destinatario);

    if (dados.destinatario != "" && _abertura.Destinatario.val() != "")
        _abertura.Destinatario.enable(false);

    if (_motivoChamadoConfiguracao.PermiteSelecionarMotorista)
        _buscaMotorista.CarregarMotoristaCargaSelecionada();

    if (_motivoChamadoConfiguracao.PossibilitarInclusaoAnexoAoEscalarAtendimento)
        _analise.ExibirAnexosEscaladas.visible(true);
    else
        _analise.ExibirAnexosEscaladas.visible(false);

    obterDetalhesCarga();

    Global.abrirModal(idModal);
}

/*
 * Declaração das Funções Privadas
 */

function buscarChamados() {
    var editar = { descricao: (_FormularioSomenteLeitura ? Localization.Resources.Cargas.ControleEntrega.Consultar : Localization.Resources.Gerais.Geral.Editar), id: guid(), evento: "onclick", metodo: editarChamadoClick, tamanho: 5, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    var configuracoesExportacao = { url: "ChamadoOcorrencia/ExportarPesquisa", titulo: "Atendimentos" };
    var ordenacaoPadrao = { column: 5, dir: orderDir.desc };

    _gridChamados = new GridViewExportacao("grid-pesquisa-chamados", "ChamadoOcorrencia/Pesquisa", _pesquisaChamados, menuOpcoes, configuracoesExportacao, ordenacaoPadrao, 10);
    _gridChamados.SetPermitirEdicaoColunas(true);
    _gridChamados.SetSalvarPreferenciasGrid(true);
    _gridChamados.SetHabilitarScrollHorizontal(true, 200);
    _gridChamados.SetCallbackDrawGridView(callbackDrawGridView);
    _gridChamados.CarregarGrid();
}

function callbackDrawGridView(settings, api, scrollHorizontal) {
    //Por enquanto vai ficar fixo apenas a coluna Editar. Depois podemos pensar em alguma forma dinâmica para o usuário escolher.
    let columnName = "Editar";
    let columnIndex = -1;

    api.columns().every(function (index) {
        if (api.column(index).header().textContent === columnName) {
            columnIndex = index;
        }
    });

    if (columnIndex > 0) {
        //Adiciona Classe para fixação da coluna na grid.
        api.rows().every(function (rowIdx) {
            let cell = api.cell(rowIdx, columnIndex).node();
            if (scrollHorizontal)
                $(cell).addClass('grid-view-fixed-column-scroll');
            else
                $(cell).removeClass('grid-view-fixed-column-scroll');
        });

        let cellHeader = $(api.columns().header()[columnIndex]);
        if (scrollHorizontal)
            cellHeader.addClass('grid-view-fixed-column-scroll');
        else
            cellHeader.removeClass('grid-view-fixed-column-scroll');
    }
}
function editarChamado(data) {
    _chamado.Codigo.val(data.Codigo);
    _chamado.Situacao.val(data.Situacao);
    _chamado.PodeEditar.val(data.PodeEditar);
    _chamado.PossuiResponsavel.val(data.PossuiResponsavel);
    _chamado.GerarCargaDevolucao.val(data.GerarCargaDevolucao);
    _chamado.VeiculoCarregado.val(data.VeiculoCarregado);
    _chamado.Notificado.val(data.Notificado);
    _chamado.CargaDevolucao.val(data.CargaDevolucao);
    _chamado.CargaEntrega.val(data.CargaEntrega);
    _chamado.BloquearFinalizar.val(data.BloquearFinalizar);
    _chamado.PossuiIntegracao.val(data.PossuiIntegracao);
    _chamado.NivelAtendimento.val(data.NivelAtendimento);
    _chamado.Estadia.val(data.Estadia);

    _CRUDChamado.Imprimir.visible(true);

    if (data.Responsavel != null) {
        _chamado.Responsavel.val(data.Responsavel.Descricao);
        _chamado.Responsavel.codEntity(data.Responsavel.Codigo);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        if (_chamado.PossuiResponsavel.val()) {
            var permitirAssumirChamadoDeOutroResponsavel = (_chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa) && (data.CodigoResponsavel != _CONFIGURACAO_TMS.CodigoUsuarioLogado) &&
                (_CONFIGURACAO_TMS.PermitirAssumirChamadoDeOutroResponsavel || data.PermiteAssumirChamadoMesmoSetor || (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAssumirAtendimento, _PermissoesPersonalizadasChamado) && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS));

            _CRUDChamado.AssumirChamado.visible(permitirAssumirChamadoDeOutroResponsavel);
        }
        else
            _CRUDChamado.AssumirChamado.visible((VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAssumirAtendimento, _PermissoesPersonalizadasChamado) || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS));

        if (_chamado.Situacao.val() === EnumSituacaoChamado.Cancelada)
            _CRUDChamado.Reabrir.visible(true);
    }
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor && data.CodigoResponsavel != _CONFIGURACAO_TMS.CodigoUsuarioLogado)
        _CRUDChamado.AssumirChamado.visible(true);

    if (!data.LiberarAssumirChamado)
        _CRUDChamado.AssumirChamado.visible(false);
}

function AtivarAlertaDeNivelChamado(excedeuTempo) {
    if (!excedeuTempo)
        return;
    $("#numeroChamadoNivelAlerta").empty();
    $("#numeroChamadoNivelAlerta").append(_resumoChamado.Numero.val());
    Global.abrirModal("modalNivelAlerta");
}

function AtivarRelogioAlertaDeNivelChamado(dataLimiteTratativa, nivelAtendimento, situacao) {
    let relogio = $("#relogioAlertaDeNivelChamado");
    relogio.empty();

    let listaSituacao = [EnumSituacaoChamado.Aberto, EnumSituacaoChamado.EmTratativa];

    if (nivelAtendimento == EnumEscalationList.Nenhum || !dataLimiteTratativa || !listaSituacao.includes(situacao))
        return;

    relogio.text(Localization.Resources.Cargas.ControleEntrega.TempoLimiteParaTratativaDoNivelAtingidoEscalarAtendimento);
    relogio.addClass("Delayed");
    relogio.removeClass("OnTime");

    relogio
        .countdown(moment(dataLimiteTratativa, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: false, precision: 1000 })
        .on('update.countdown', function (event) {
            let totalHoras = String(event.offset.totalDays * 24 + event.offset.hours).padStart(2, '0');
            let totalMinutos = String(event.offset.minutes).padStart(2, '0');
            let totalSegundos = String(event.offset.seconds).padStart(2, '0');
            $(this).text(Localization.Resources.Cargas.ControleEntrega.TempoRestanteParaTratativaNesseNivel + ": " + totalHoras + ":" + totalMinutos + ":" + totalSegundos);
            $(this).addClass("OnTime");
            $(this).removeClass("Delayed");
        })
        .on('finish.countdown', function (event) {
            $(this).text(Localization.Resources.Cargas.ControleEntrega.TempoLimiteParaTratativaDoNivelAtingidoEscalarAtendimento);
            $(this).addClass("Delayed");
            $(this).removeClass("OnTime");
        });
}

function CloseModalAlertaNivel() {
    Global.fecharModal("modalNivelAlerta");
}

function CloseModalChamadoCancelado() {
    buscarChamadoPorCodigo(_chamado.Codigo.val());
    Global.fecharModal("modalChamadoCancelado");
}

function obterSeExisteRegraUsuarioResponsavel(telaChamado) {
    executarReST("ChamadoOcorrencia/ObterSeExisteRegraUsuarioResponsavel", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.ExisteRegraUsuarioResponsavel && telaChamado)
                _pesquisaChamados.Responsavel.visible(false);
        }
    });
}

function limparFiltrosChamadoOcorrencia() {
    LimparCampos(_pesquisaChamados);
}