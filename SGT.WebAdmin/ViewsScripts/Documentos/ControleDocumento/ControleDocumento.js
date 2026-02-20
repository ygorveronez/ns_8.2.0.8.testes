/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="Desconto.js" />
/// <reference path="DetalhesControleDocumento.js" />
/// <reference path="CartaCorrecao.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Enumeradores/EnumTipoIrregularidade.js" />
/// <reference path="../../Enumeradores/EnumAcaoTratativaIrregularidade.js" />

// #region Objetos Globais do Arquivo

var _gridControleDocumento;
var _pesquisaControleDocumento;
var _rejeicao;
var _PermissoesPersonalizadasOcorrencia = {};
var _InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = false;
var _modalPagarOutroValor;
// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaControleDocumento = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: EnumSituacaoControleDocumento.obterOpcoes(), def: "", visible: ko.observable(true), getType: typesKnockout.selectMultiple });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int });
    this.NotaFiscal = PropertyEntity({ text: "Número NF-e:", getType: typesKnockout.int });
    this.NumeroDocumento = PropertyEntity({ text: "Número:", getType: typesKnockout.int });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Documento Fiscal:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Portfolio = PropertyEntity({ type: types.multiplesEntities, visible: ko.observable(true), codEntity: ko.observable(0), text: "Portfólio:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Irregularidade = PropertyEntity({ type: types.multiplesEntities, visible: ko.observable(true), codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.multiplesEntities, visible: ko.observable(true), codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataGeracaoIrregularidadeInicial = PropertyEntity({ text: "Data de Geração Irregularidade Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataGeracaoIrregularidadeFinal = PropertyEntity({ text: "Data de Geração Irregularidade Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.DataGeracaoIrregularidadeFinal.dateRangeInit = this.DataGeracaoIrregularidadeInicial;
    this.DataGeracaoIrregularidadeInicial.dateRangeLimit = this.DataGeracaoIrregularidadeFinal;


    this.Pesquisar = PropertyEntity({ eventClick: atualizarGridControleDocumentos, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(permitirEditarInformacoesControleDocumentos()) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ParquearSelecionados = PropertyEntity({ eventClick: exibirModalParqueamento, type: types.event, text: "Parquear selecionados", idGrid: guid(), visible: ko.observable(false) });
    this.DesparquearSelecionados = PropertyEntity({ eventClick: desparquearSelecionadosClick, type: types.event, text: "Desparquear selecionados", idGrid: guid(), visible: ko.observable(false) });
    this.AprovarSelecionados = PropertyEntity({ eventClick: exibirModalAprovacao, type: types.event, text: "Aprovar selecionados", idGrid: guid(), visible: ko.observable(false) });
    this.RejeitarSelecionados = PropertyEntity({ eventClick: exibirModalRejeicao, type: types.event, text: "Rejeitar selecionados", idGrid: guid(), visible: ko.observable(false) });
    this.InformarAnaliseSelecionados = PropertyEntity({ eventClick: exibirModalInformarAnalise, type: types.event, text: "Informar Análise", idGrid: guid(), visible: ko.observable(false) });
    this.InformarMotivoSelecionados = PropertyEntity({ eventClick: exibirModalInformarMotivo, type: types.event, text: "Informar Motivo", idGrid: guid(), visible: ko.observable(false) });
    this.PagarConformeOutroValor = PropertyEntity({ eventClick: pagarConformeOutroValorModalClick, type: types.event, text: "Pagar Conforme ", idGrid: guid(), visible: ko.observable(false) });
    this.PagarConformeDocumento = PropertyEntity({ eventClick: pagarConformeDocumentoClick, type: types.event, text: "Pagar Conforme Documento", idGrid: guid(), visible: ko.observable(false) });
    this.PagarConformeFRS = PropertyEntity({ eventClick: pagarConformeFRSClick, type: types.event, text: "Pagar Conforme FRS", idGrid: guid(), visible: ko.observable(false) });
    this.EnviarAoTransportador = PropertyEntity({ eventClick: enviarAoTransportadorClick, type: types.event, text: "Enviar para o Transportador", idGrid: guid(), visible: ko.observable(false) });
    this.RetornarAoEmbarcador = PropertyEntity({ eventClick: retornarAoEmbarcadorClick, type: types.event, text: "Retornar ao Embarcador", idGrid: guid(), visible: ko.observable(false) });

    this.Delegar = PropertyEntity({ /*eventClick: exibirModalDelegar,*/ type: types.event, text: "Delegar", idGrid: guid(), visible: ko.observable(false) });
    this.DelegarManual = PropertyEntity({ eventClick: exibirModalDelegar, type: types.event, text: "Manual", idGrid: guid(), visible: ko.observable(false), icon: "fal fa-hand-point-up" });
    this.DelegarAutomatico = PropertyEntity({ eventClick: delegarAutomaticoClick, type: types.event, text: "Automático", idGrid: guid(), visible: ko.observable(false), icon: "fal fa-cog" });

    this.Acoes = PropertyEntity({ /*eventClick: ,*/ type: types.event, text: "Ações", idGrid: guid(), visible: ko.observable(false), icon: "fal fa-remove" });
    this.Complemento = PropertyEntity({ eventClick: exibirModalComplemento, type: types.event, text: "Necessário Complemento", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-plus" });
    this.Desacordo = PropertyEntity({ eventClick: desacordoClick, type: types.event, text: "Desacordo", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-ban" });
    this.CartaCorrecao = PropertyEntity({ eventClick: cartaCorrecaoclick, type: types.event, text: "Necessário Carta de Correção", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-pencil" });
    this.SubstituirNFS = PropertyEntity({ eventClick: substituirNFSClick, type: types.event, text: "Necessário Substituir NFS", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-retweet" });

};

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarDocumentosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ParquearComMotivo = function () {
    this.Motivo = PropertyEntity({ text: "Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.ParquearComMotivo = PropertyEntity({ eventClick: parquearSelecionadosClick, type: types.event, text: "Enviar para Aprovação", idGrid: guid(), visible: ko.observable(true) });
}

var AprovarComMotivo = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo da Aprovação: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.AprovarDocumentos = PropertyEntity({ eventClick: aprovarSelecionadosClick, type: types.event, text: "Aprovar Todos", idGrid: guid(), visible: ko.observable(true) });
}

var RejeitarComMotivo = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo da Rejeição: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.RejeitarDocumentos = PropertyEntity({ eventClick: rejeitarSelecionadosClick, type: types.event, text: "Rejeitar Todos", idGrid: guid(), visible: ko.observable(true) });
}

var InformarAnalise = function () {
    this.Motivo = PropertyEntity({ text: "*Análise: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.AnalisarDocumentos = PropertyEntity({ eventClick: analisarSelecionadosClick, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true) });
}

var InformarMotivo = function () {
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.InformarMotivoDocumentos = PropertyEntity({ eventClick: informarMotivoSelecionadosClick, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true) });
}

var InformarRejeicaoCCe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Motivo = PropertyEntity({ text: "*Motivo da rejeição: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.RejeitarCCe = PropertyEntity({ eventClick: enviarRejeicaoCCe, type: types.event, text: "Rejeitar", idGrid: guid(), visible: ko.observable(true) });
}

var Delegar = function () {
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Setor:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", idGrid: guid(), visible: ko.observable(true) });
}

var CienteParqueamento = function () {
    this.MensagemCabecalho = PropertyEntity({ text: "Autorização de parquemanto realizada com sucesso. Atenção aos detalhes abaixo:"});
    this.MensagemAcao = PropertyEntity({ val: ko.observable("") });
    this.Ciente = ({ eventClick: fecharAprovarMotivoSelecionados, type: types.event, text: "Estou ciente", idGrid: guid(), visible: ko.observable(true) });
}

function ModalPagarOutroValor() {
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Pagar:", visible: ko.observable(true), required: false });
    this.Pagar = PropertyEntity({ eventClick: pagarConformeOutroValorClick, type: types.event, text: "Pagar", idGrid: guid(), visible: ko.observable(true) });
}
// #endregion Classes

// #region Funções de Inicialização

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoControleDocumentos)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaControleDocumento.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaControleDocumento.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadControleDocumento() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {

            _pesquisaControleDocumento = new PesquisaControleDocumento();
            KoBindings(_pesquisaControleDocumento, "knockoutPesquisaControleDocumento", false, _pesquisaControleDocumento.Pesquisar.id);

            _parquearComMotivo = new ParquearComMotivo();
            KoBindings(_parquearComMotivo, "divModalParqueamento");

            _aprovarComMotivo = new AprovarComMotivo();
            KoBindings(_aprovarComMotivo, "divModalAprovacao");

            _rejeitarComMotivo = new RejeitarComMotivo();
            KoBindings(_rejeitarComMotivo, "divModalRejeicao");

            _informarAnalise = new InformarAnalise();
            KoBindings(_informarAnalise, "divModalInformarAnalise");

            _informarMotivo = new InformarMotivo();
            KoBindings(_informarMotivo, "divModalInformarMotivo");

            _delegar = new Delegar();
            KoBindings(_delegar, "divModalDelegar");
            
            _cienteParqueamento = new CienteParqueamento();
            KoBindings(_cienteParqueamento, "divModalCienteParqueamento");

            _informarRejeicaoCCe = new InformarRejeicaoCCe();
            KoBindings(_informarRejeicaoCCe, "knockoutInformarRejeicaoCCe");

            _modalPagarOutroValor = new ModalPagarOutroValor();
            KoBindings(_modalPagarOutroValor, "koutModalOutroValorInformar");

            HeaderAuditoria("ControleDocumento");

            new BuscarTransportadores(_pesquisaControleDocumento.Empresa, null, null, true);
            new BuscarModeloDocumentoFiscal(_pesquisaControleDocumento.ModeloDocumentoFiscal);
            new BuscarCargas(_pesquisaControleDocumento.Carga);
            new BuscarFilial(_pesquisaControleDocumento.Filial);
            new BuscarFuncionario(_pesquisaControleDocumento.Usuario);
            new BuscarIrregularidades(_pesquisaControleDocumento.Irregularidade);
            new BuscarPortfolioModuloControle(_pesquisaControleDocumento.Portfolio);
            new BuscarSetorFuncionario(_pesquisaControleDocumento.Setor);
            new BuscarSetorFuncionario(_delegar.Setor);

            new BuscarMotivosIrregularidade(_informarMotivo.Motivo);

            $.get("Content/Static/Documentos/ModalDetalheGestaoDocumentoAprovacao.html?dyn=" + guid(), function (conteudo) {
                $('#divPaiModalDetalheGestaoDocumentoAprovacao').html(conteudo);
                removerHTMLInutil();
                loadDetalheControleDocumento();
                loadGridControleDocumento();
                loadDadosUsuarioLogado(atualizarGridControleDocumentos);
            });

            loadHistoricoIrregularidades();
            loadLinkarDocumento();

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                _pesquisaControleDocumento.Usuario.visible(false);
                _pesquisaControleDocumento.Empresa.visible(false);
            }

            $('#divModalOcorrencia').on('hidden.bs.modal', function () {
            });

            _chamadoOcorrenciaModalOcorrencia = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static' });
            carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");

            loadAnexosCartaCorrecao();
        });
    });
}

function loadGridControleDocumento() {
    var dadosCarga = { descricao: "Dados da Carga", id: guid(), metodo: exibirDadosCargaClick, icone: "", visibilidade: VisibilidadeDadosCarga };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("ControleDocumento"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: exibirDetalhesControleDocumentoClick, tamanho: "20", icone: "", visibilidade: VisibilidadeDetalhes };
    var detalhesParqueamento = { descricao: "Detalhes Parqueamento", id: guid(), evento: "onclick", metodo: exibirDetalhesParqueamentoClick, tamanho: "20", icone: "" };
    var verAnalise = { descricao: "Ver Análise", id: guid(), evento: "onclick", metodo: exibirAnaliseClick, tamanho: "20", icone: "" };
    var historicoIrregularidade = { descricao: "Histórico Irregularidades", id: guid(), evento: "onclick", metodo: abrirHistoricoIrregularide, tamanho: "20", icone: "" };
    var cartaCorrecao = { descricao: "Carta de Correção", id: guid(), evento: "onclick", metodo: abrirAnexosCartaCorrecaoClick, tamanho: "20", icone: "" };
    var aprovarCCe = { descricao: "Aprovar Carta de Correção", id: guid(), evento: "onclick", metodo: AprovarCCeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeAprovaRejeitaCCe };
    var rejeitarCCe = { descricao: "Rejeitar Carta de Correção", id: guid(), evento: "onclick", metodo: RejeitarCCeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeAprovaRejeitaCCe };
    var verMotivoRejeicaoCCe = { descricao: "Ver Rejeição Carta de Correção", id: guid(), evento: "onclick", metodo: exibirMotivoRejeicaoCCeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeMotivoRejeicaoCCe };
    var LinkarDocumento = { descricao: "Linkar Manualmente", id: guid(), evento: "onclick", metodo: abrirModalLinkarDocumento, tamanho: "20", icone: "", visibilidade: VisibilidadeLinkarDocumento };
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,

        opcoes: [dadosCarga, detalhes, auditar, detalhesParqueamento, verAnalise, historicoIrregularidade, cartaCorrecao, aprovarCCe, rejeitarCCe, verMotivoRejeicaoCCe, LinkarDocumento],
        tamanho: 7
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaControleDocumento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "ControleDocumento/ExportarPesquisa",
        titulo: "Gestão Documentos"
    };

    _gridControleDocumento = new GridViewExportacao("grid-controle-documento", "ControleDocumento/Pesquisa", _pesquisaControleDocumento, menuOpcoes, configExportacao, null, 10, multiplaEscolha, 50);
    _gridControleDocumento.SetPermitirEdicaoColunas(true);
    _gridControleDocumento.SetPermitirReordenarColunas(true);
    _gridControleDocumento.SetSalvarPreferenciasGrid(true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarTodosClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente aprovar os documentos selecionados?", function () {
        Salvar(_pesquisaControleDocumento, "ControleDocumento/AprovarDocumentos", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos aprovados.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function exibirDadosCargaClick(e) {
    var data = { Carga: e.CodigoCarga };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close mt-3 me-2" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function aprovarMultiplosDocumentosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os documentos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaControleDocumento);

        dados.SelecionarTodos = _pesquisaControleDocumento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridControleDocumento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridControleDocumento.ObterMultiplosNaoSelecionados());

        executarReST("ControleDocumento/Parquear", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridControleDocumentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}


function parquearSelecionadosClick() {
    var listaCodigos = new Array();

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    var motivoParqueamento = RetornarObjetoPesquisa(_parquearComMotivo);

    var motivoParquado = motivoParqueamento.Motivo;

    executarReST("ControleDocumento/ParquearDocumentos", { Codigos: JSON.stringify(listaCodigos), MotivoParqueamento: motivoParquado }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação enviada com sucesso.");
                fecharAprovarMotivoSelecionados();
                _gridControleDocumento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function exibirModalAcaoCienteParqueamento() {
    var mensagem = "";
    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        if (_gridControleDocumento.ObterMultiplosSelecionados()[i].TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFS || _gridControleDocumento.ObterMultiplosSelecionados()[i].TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe) {
            mensagem += "NFS Número " + _gridControleDocumento.ObterMultiplosSelecionados()[i].Numero + ": ";
            
            if (_gridControleDocumento.ObterMultiplosSelecionados()[i].DentroDoMes)
                mensagem += "Realizar a substituição da NFS";
            else
                mensagem += "Realizar cancelamento extemporâneo da NFS";
            mensagem += " <br/>"
        }
    }
    if (mensagem.length > 0) {
        $("#idAcoesNFS").html(mensagem);
        Global.abrirModal('divModalCienteParqueamento');
    }
}

function desparquearSelecionadosClick() {
    var listaCodigos = new Array();

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    exibirConfirmacao("Confirmação", "Você realmente deseja desparquear todos os documentos selecionados?", function () {

        executarReST("ControleDocumento/DesparquearDocumentos", { Codigos: JSON.stringify(listaCodigos) }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Desparqueamento realizado com sucesso.");
                    fecharAprovarMotivoSelecionados();
                    _gridControleDocumento.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function aprovarSelecionadosClick() {
    var listaCodigos = new Array();

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    var motivoAprovacao = RetornarObjetoPesquisa(_aprovarComMotivo);

    var motivoAprovado = motivoAprovacao.Motivo;

    executarReST("ControleDocumento/AprovarDocumentos", { Codigos: JSON.stringify(listaCodigos), MotivoAprovacao: motivoAprovado }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos aprovados");
                exibirModalAcaoCienteParqueamento();
                fecharAprovarMotivoSelecionados();
                _gridControleDocumento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function rejeitarSelecionadosClick() {
    var listaCodigos = new Array();

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    var motivoRejeicao = RetornarObjetoPesquisa(_rejeitarComMotivo);

    var motivoRejeitado = motivoRejeicao.Motivo;

    executarReST("ControleDocumento/RejeitarDocumentos", { Codigos: JSON.stringify(listaCodigos), MotivoRejeicao: motivoRejeitado }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos aprovados");
                fecharAprovarMotivoSelecionados();
                _gridControleDocumento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function analisarSelecionadosClick() {
    var listaCodigos = new Array();

    if (!ValidarCamposObrigatorios(_informarAnalise))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    var analise = RetornarObjetoPesquisa(_informarAnalise);

    executarReST("ControleDocumento/InformarAnaliseDocumentos", { Codigos: JSON.stringify(listaCodigos), Analise: analise.Motivo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Análise informada com sucesso");
                fecharAprovarMotivoSelecionados();
                atualizarGridControleDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function informarMotivoSelecionadosClick() {
    var listaCodigos = new Array();

    if (!ValidarCamposObrigatorios(_informarMotivo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    var motivo = RetornarObjetoPesquisa(_informarMotivo);

    executarReST("ControleDocumento/InformarMotivoDocumentos", { Codigos: JSON.stringify(listaCodigos), Motivo: motivo.Motivo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Motivo de irregularidade informado com sucesso");
                fecharAprovarMotivoSelecionados();
                atualizarGridControleDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function AprovarCCeClick(registro) {

    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar a carta de correção do documento?", function () {
        executarReST("ControleDocumento/AprovarCCe", { Codigo: registro.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carta de correção aprovada com sucesso");
                    atualizarGridControleDocumentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function RejeitarCCeClick(registro) {
    _informarRejeicaoCCe.Codigo.val(registro.Codigo);
    _informarRejeicaoCCe.Motivo.enable(true);
    _informarRejeicaoCCe.Motivo.val("");
    _informarRejeicaoCCe.RejeitarCCe.visible(true);
    Global.abrirModal('divModalInformarRejeicaoCCe');
}

function enviarRejeicaoCCe() {
    
    if (!ValidarCamposObrigatorios(_informarRejeicaoCCe))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    
    executarReST("ControleDocumento/RejeitarCCe", { Codigo: _informarRejeicaoCCe.Codigo.val(), Motivo: _informarRejeicaoCCe.Motivo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Carta de correção rejeitada com sucesso");
                fecharAprovarMotivoSelecionados();
                atualizarGridControleDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function enviarAoTransportadorClick() {
    var listaCodigos = new Array();

    exibirConfirmacao("Confirmação", "Realmente deseja enviar os documentos selecionados ao transporador?", function () {

        for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
            var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
            listaCodigos.push(registro.Codigo);
        }

        executarReST("ControleDocumento/RespostaResponsabilidade", { Responsavel: "Transportador", Codigos: JSON.stringify(listaCodigos) }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos enviados ao transportador com sucesso");
                    atualizarGridControleDocumentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function retornarAoEmbarcadorClick() {
    var listaCodigos = new Array();

    exibirConfirmacao("Confirmação", "Realmente deseja retornar os documentos selecionados ao embarcador?", function () {

        for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
            var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
            listaCodigos.push(registro.Codigo);
        }

        executarReST("ControleDocumento/RespostaResponsabilidade", { Responsavel: "Embarcador", Codigos: JSON.stringify(listaCodigos) }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos retornados ao embarcador com sucesso");
                    atualizarGridControleDocumentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function delegarClick() {
    if (!ValidarCamposObrigatorios(_delegar))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var delegar = RetornarObjetoPesquisa(_delegar);
    delegarRequisicao(delegar.Setor);
}

function delegarAutomaticoClick() {
    var listaCodigos = new Array();

    delegarRequisicao(0);
}

function delegarRequisicao(codigo) {
    var listaCodigos = codigosRegistrosSelecionados();

    executarReST("ControleDocumento/DelegarSetor", { Codigos: JSON.stringify(listaCodigos), Setor: codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento delegado ao setor selecionado com sucesso");
                fecharAprovarMotivoSelecionados();
                atualizarGridControleDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function gravarAcaoTratativa(acao) {
    var listaCodigos = codigosRegistrosSelecionados();

    executarReST("ControleDocumento/InformarAcaoTratativa", { Codigos: JSON.stringify(listaCodigos), Acao: acao }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ação informada com sucesso");
                atualizarGridControleDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function desacordoClick() {
    gravarAcaoTratativa(EnumAcaoTratativaIrregularidade.AplicarEventoDesacordo);
}
function cartaCorrecaoclick() {
    gravarAcaoTratativa(EnumAcaoTratativaIrregularidade.NecessarioCartaCorrecao);
}

function substituirNFSClick() {
    gravarAcaoTratativa(EnumAcaoTratativaIrregularidade.SubstituirDocumento);
}

function exibirModalComplemento() {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();

    if (registros.length > 0) {
        var data = { Codigo: registros[0].CodigoCarga, CodigoCargaEmbarcador: registros[0].Carga };

        limparCamposOcorrencia();

        _ocorrencia.NaoLimparCarga.val(true);
        _ocorrencia.Carga.enable(false);
        _ocorrencia.ControleDocumento.val(registros[0].Codigo);

        retornoCarga(data, function () {
            _chamadoOcorrenciaModalOcorrencia.show();
            visiblidadeValorOcorrencia();

            _ocorrencia.Observacao.enable(true);
            _ocorrencia.ObservacaoCTe.enable(true);

            _ocorrencia.ValorOcorrencia.enable(true);
        });
        _ocorrencia.OcorrenciaSalvaPelaTelaChamadoOcorrencia.val(true);
    }
}


function abrirOcorrencia(codigoDocumento, carga) {
    LimparCampos(_ocorrencia);
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();

    var data = { Codigo: carga };

    limparCamposOcorrencia();

    _ocorrencia.NaoLimparCarga.val(true);
    _ocorrencia.Carga.enable(false);
    _ocorrencia.ControleDocumento.val(codigoDocumento);

    retornoCarga(data, function () {
        _chamadoOcorrenciaModalOcorrencia.show();
        visiblidadeValorOcorrencia();

        _ocorrencia.Observacao.enable(true);
        _ocorrencia.ObservacaoCTe.enable(true);

        _ocorrencia.ValorOcorrencia.enable(true);
    });
    _ocorrencia.OcorrenciaSalvaPelaTelaChamadoOcorrencia.val(true);
}

function exibirModalParqueamento() {
    Global.abrirModal('divModalParqueamento');
}

function exibirModalAprovacao() {
    Global.abrirModal('divModalAprovacao');
}

function exibirModalRejeicao() {
    Global.abrirModal('divModalRejeicao');
}

function exibirModalInformarAnalise() {
    _informarAnalise.Motivo.enable(true);
    _informarAnalise.AnalisarDocumentos.visible(true);
    Global.abrirModal('divModalInformarAnalise');
}

function exibirAnaliseClick(registro) {
    _informarAnalise.Motivo.val(registro.Analise);
    _informarAnalise.Motivo.enable(false);
    _informarAnalise.AnalisarDocumentos.visible(false);
    Global.abrirModal('divModalInformarAnalise');
}

function exibirMotivoRejeicaoCCeClick(registro) {
    _informarRejeicaoCCe.Motivo.val(registro.MotivoRejeicaoCCe);
    _informarRejeicaoCCe.Motivo.enable(false);
    _informarRejeicaoCCe.RejeitarCCe.visible(false);
    Global.abrirModal('divModalInformarRejeicaoCCe');
}

function abrirModalLinkarDocumento(registro) {
    CarregarRegistroLinkarDocumento(registro.Codigo);
    Global.abrirModal('divModalLinkarDocumento');
}


function exibirModalInformarMotivo() {
    Global.abrirModal('divModalInformarMotivo');
}

function exibirModalDelegar() {
    var irregularidade = _gridControleDocumento.ObterMultiplosSelecionados()[0].CodigoEntidadeIrregularidade;
    Global.abrirModal('divModalDelegar');
}

function fecharAprovarMotivoSelecionados() {
    LimparCampos(_parquearComMotivo);
    LimparCampos(_aprovarComMotivo);
    LimparCampos(_rejeitarComMotivo);
    LimparCampos(_informarAnalise);
    LimparCampos(_informarMotivo);
    LimparCampos(_delegar);
    LimparCampos(_informarRejeicaoCCe);

    Global.fecharModal('divModalParqueamento');
    Global.fecharModal('divModalAprovacao');
    Global.fecharModal('divModalRejeicao');
    Global.fecharModal('divModalInformarAnalise');
    Global.fecharModal('divModalInformarMotivo');
    Global.fecharModal('divModalDelegar');
    Global.fecharModal('divModalCienteParqueamento');
    Global.fecharModal('divModalInformarRejeicaoCCe');
    Global.fecharModal('divModalLinkarDocumento');
}

function exibirDetalhesControleDocumentoClick(registroSelecionado) {
    exibirDetalheControleDocumento(registroSelecionado);
}

function exibirDetalhesParqueamentoClick(registroSelecionado) {
    exibirDetalheParqueamento(registroSelecionado);
}


// #endregion Funções Associadas a Eventos

//#region Acoes
function atualizarGridControleDocumentos() {
    _pesquisaControleDocumento.SelecionarTodos.val(false);
    _pesquisaControleDocumento.ParquearSelecionados.visible(false);
    _pesquisaControleDocumento.InformarAnaliseSelecionados.visible(false);
    _pesquisaControleDocumento.InformarMotivoSelecionados.visible(false);
    _pesquisaControleDocumento.RetornarAoEmbarcador.visible(false);
    _pesquisaControleDocumento.EnviarAoTransportador.visible(false);
    _pesquisaControleDocumento.Delegar.visible(false);
    _pesquisaControleDocumento.DelegarManual.visible(false);
    _pesquisaControleDocumento.DelegarAutomatico.visible(false);

    _pesquisaControleDocumento.Acoes.visible(false);
    _pesquisaControleDocumento.Complemento.visible(false);
    _pesquisaControleDocumento.Desacordo.visible(false);
    _pesquisaControleDocumento.CartaCorrecao.visible(false);
    _pesquisaControleDocumento.SubstituirNFS.visible(false);

    _gridControleDocumento.AtualizarRegistrosSelecionados();

    _gridControleDocumento.CarregarGrid();
}

function exibirMultiplasOpcoes() {
    var existemRegistrosSelecionados = _gridControleDocumento.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaControleDocumento.SelecionarTodos.val();

    _pesquisaControleDocumento.ParquearSelecionados.visible(false);
    _pesquisaControleDocumento.DesparquearSelecionados.visible(false);
    _pesquisaControleDocumento.AprovarSelecionados.visible(false);
    _pesquisaControleDocumento.RejeitarSelecionados.visible(false);

    if (permitirEditarInformacoesControleDocumentos() && (existemRegistrosSelecionados || selecionadoTodos)) {
        _pesquisaControleDocumento.ParquearSelecionados.visible(true);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ControleDocumento_PermiteDesparquearDocumentos, _PermissoesPersonalizadas)) {
            _pesquisaControleDocumento.DesparquearSelecionados.visible(verificarSituacaoParqueado());
        }
    }

    _pesquisaControleDocumento.InformarAnaliseSelecionados.visible(permitirInformarAnalise());
    _pesquisaControleDocumento.InformarMotivoSelecionados.visible(permitirInformarMotivo());
    _pesquisaControleDocumento.PagarConformeFRS.visible(permitirPagarConformeFRS());
    _pesquisaControleDocumento.PagarConformeDocumento.visible(permitirPagarConformeDocumento());
    _pesquisaControleDocumento.PagarConformeOutroValor.visible(permitirPagarConformeOutroValor());

    _pesquisaControleDocumento.RetornarAoEmbarcador.visible(permitirRetornarAoEmbarcador());
    _pesquisaControleDocumento.EnviarAoTransportador.visible(permitirOpcoesEmbarcador());

    _pesquisaControleDocumento.DelegarManual.visible(permitirOpcoesEmbarcador() && _gridControleDocumento.ObterMultiplosSelecionados().length == 1);
    _pesquisaControleDocumento.DelegarAutomatico.visible(permitirOpcoesEmbarcador() && _gridControleDocumento.ObterMultiplosSelecionados().length == 1);

    _pesquisaControleDocumento.Delegar.visible(_pesquisaControleDocumento.DelegarManual.visible() || verificarTipoDesacordo());

    _pesquisaControleDocumento.Complemento.visible(permitirTratativa(EnumAcaoTratativaIrregularidade.AnexarComplementar) && _gridControleDocumento.ObterMultiplosSelecionados().length == 1);
    _pesquisaControleDocumento.Desacordo.visible(permitirTratativa(EnumAcaoTratativaIrregularidade.AplicarEventoDesacordo));
    _pesquisaControleDocumento.CartaCorrecao.visible(permitirTratativa(EnumAcaoTratativaIrregularidade.NecessarioCartaCorrecao));
    _pesquisaControleDocumento.SubstituirNFS.visible(permitirTratativa(EnumAcaoTratativaIrregularidade.SubstituirDocumento));
    
    _pesquisaControleDocumento.Acoes.visible(visibilidadeAcoes());

    if (permitirAprovarParqueamentos() && (existemRegistrosSelecionados || selecionadoTodos)) {
        _pesquisaControleDocumento.AprovarSelecionados.visible(true);
        _pesquisaControleDocumento.RejeitarSelecionados.visible(true);
    }
}

function permitirEditarInformacoesControleDocumentos() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);
}

function permitirAprovarParqueamentos() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe);
}

function abrirHistoricoIrregularide(e) {
    Global.abrirModal("divModalVisualizarHistoricoIrregularidade");
    CarregarRegistroHistoricoIrregularidade(e.Codigo);
}

function VisibilidadeDadosCarga() {
    return true;
}

function VisibilidadeAprovaRejeitaCCe(r) {
    var irregularidadesCCe = [EnumGatilhoIrregularidade.CSTICMS,
        EnumGatilhoIrregularidade.NFeVinculadaAoFrete
    ];
    
    return irregularidadesCCe.indexOf(parseInt(r.CodigoIrregularidade)) > -1 && r.ResponsavelPelaIrregularidade == _InformacoesUsuario.Setor;
}

function VisibilidadeMotivoRejeicaoCCe(r) {
    var irregularidadesCCe = [EnumGatilhoIrregularidade.CSTICMS,
    EnumGatilhoIrregularidade.NFeVinculadaAoFrete
    ];

    return irregularidadesCCe.indexOf(parseInt(r.CodigoIrregularidade)) > -1 && r.CCeRejeitada;
}

function VisibilidadeLinkarDocumento(r) {
    var irregularidadesCCe = [EnumGatilhoIrregularidade.SemLink];

    return irregularidadesCCe.indexOf(parseInt(r.CodigoIrregularidade)) > -1;
}


//#endregion Acoes

//#region Visibilidade Açoes
function visibilidadeAcoes() {
    return (_pesquisaControleDocumento.Complemento.visible() || _pesquisaControleDocumento.Desacordo.visible() ||
        _pesquisaControleDocumento.CartaCorrecao.visible() || _pesquisaControleDocumento.SubstituirNFS.visible() ||
        _pesquisaControleDocumento.PagarConformeFRS.visible() || _pesquisaControleDocumento.PagarConformeDocumento.visible() ||
        _pesquisaControleDocumento.PagarConformeOutroValor.visible())
}
function permitirPagarConformeOutroValor() {
    return VerificarSePossuiAcaoNasTratativas(EnumAcaoTratativaIrregularidade.PagarConformeOutroValor);
}
function permitirPagarConformeDocumento() {
    return VerificarSePossuiAcaoNasTratativas(EnumAcaoTratativaIrregularidade.PagarConformeCTeNFS);
}
function permitirPagarConformeFRS() {
    return VerificarSePossuiAcaoNasTratativas(EnumAcaoTratativaIrregularidade.PagarConformeFRS);
}
function verificarTipoDesacordo() {
    return VerificarSePossuiAcaoNasTratativas(EnumAcaoTratativaIrregularidade.DesacordoSubstituicaoDocumento) || VerificarSePossuiAcaoNasTratativas(EnumAcaoTratativaIrregularidade.AplicarEventoDesacordo);
}
function verificarSituacaoParqueado() {
    let registros = _gridControleDocumento.ObterMultiplosSelecionados();
    let selecionados = registros.map(x => x.SituacaoControleDocumento);
    let selecionadosSemDuplicidade = [...new Set(selecionados)];
    return (selecionadosSemDuplicidade.includes("Parqueado Manualmente") || selecionadosSemDuplicidade.includes("Parqueado Automaticamente"));
}
function VerificarSePossuiAcaoNasTratativas(enumTratativa) {
    let registros = _gridControleDocumento.ObterMultiplosSelecionados();
    let acoesGeralRegistrosSelecionadas = registros.map(x => x.Tratativas).map(x => x.split("|"));
    let todasAsAcoesRegistros = [];

    for (var i = 0; i < acoesGeralRegistrosSelecionadas.length; i++) {
        let acoes = acoesGeralRegistrosSelecionadas[i];
        todasAsAcoesRegistros = [...todasAsAcoesRegistros, ...acoes];
    }
    let acoesSemDuplicidade = [...new Set(todasAsAcoesRegistros)];
    return acoesSemDuplicidade.includes(`${enumTratativa}`);
}

function VerificarSePossuiParqueados(enumTratativa) {
    let registros = _gridControleDocumento.ObterMultiplosSelecionados();
    let acoesGeralRegistrosSelecionadas = registros.map(x => x.Tratativas).map(x => x.split("|"));
    let todasAsAcoesRegistros = [];

    for (var i = 0; i < acoesGeralRegistrosSelecionadas.length; i++) {
        let acoes = acoesGeralRegistrosSelecionadas[i];
        todasAsAcoesRegistros = [...todasAsAcoesRegistros, ...acoes];
    }
    let acoesSemDuplicidade = [...new Set(todasAsAcoesRegistros)];
    return acoesSemDuplicidade.includes(`${enumTratativa}`);
}

function permitirInformarAnalise() {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();
    var opcoesAnalise = [EnumGatilhoIrregularidade.SemLink,
        EnumGatilhoIrregularidade.CTeCancelado,
        EnumGatilhoIrregularidade.AliquotaICMSValorICMS,
        EnumGatilhoIrregularidade.CNPJTransportadora,
        EnumGatilhoIrregularidade.CSTICMS,
        EnumGatilhoIrregularidade.NFeVinculadaAoFrete,
        EnumGatilhoIrregularidade.TomadorFreteUnilever,
        EnumGatilhoIrregularidade.MunicipioPrestacaoServico,
        EnumGatilhoIrregularidade.CFOP,
        EnumGatilhoIrregularidade.Participantes,
        EnumGatilhoIrregularidade.AliquotaISSValorISS,
    ];
    
    var permitir = registros.every(function (registro) {
        return opcoesAnalise.indexOf(parseInt(registro.CodigoIrregularidade)) > -1 && registro.ResponsavelPelaIrregularidade == "Transportador" && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
    }) && (registros.length > 0 || _pesquisaControleDocumento.SelecionarTodos.val());
    return permitir
}

function permitirInformarMotivo() {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();
    var opcoesMotivo = [EnumGatilhoIrregularidade.ValorPrestacaoServico,
        EnumGatilhoIrregularidade.ValorTotalReceber
    ];
    
    var permitir = registros.every(function (registro) {
        return opcoesMotivo.indexOf(parseInt(registro.CodigoIrregularidade)) > -1 && registro.ResponsavelPelaIrregularidade == "Transportador" && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
    }) && (registros.length > 0 || _pesquisaControleDocumento.SelecionarTodos.val());
    return permitir;
}

function permitirOpcoesEmbarcador() {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();
    return registros.every(function (registro) {
        return registro.Irregularidade != "" && registro.ResponsavelPelaIrregularidade != "Transportador" && registro.ResponsavelPelaIrregularidade != "" && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador;
    }) && (registros.length > 0 || _pesquisaControleDocumento.SelecionarTodos.val());
}
function permitirRetornarAoEmbarcador() {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();
    return registros.every(function (registro) {
        return registro.Irregularidade != "" && registro.ResponsavelPelaIrregularidade == "Transportador" && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
    }) && (registros.length > 0 || _pesquisaControleDocumento.SelecionarTodos.val());
}

function permitirTratativa(tratativa) {
    var registros = _gridControleDocumento.ObterMultiplosSelecionados();

    var retorno = registros.every(function (registro) {
        return registro.Irregularidade != "" && registro.ResponsavelPelaIrregularidade == _InformacoesUsuario.Setor && registro.ResponsavelPelaIrregularidade != "" && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && registro.Tratativas.split("|").indexOf(tratativa.toString()) >= 0;
    }) && (registros.length > 0 || _pesquisaControleDocumento.SelecionarTodos.val());
    return retorno;
}


function abrirHistoricoIrregularide(e) {
    Global.abrirModal("divModalVisualizarHistoricoIrregularidade");
    CarregarRegistroHistoricoIrregularidade(e.Codigo);
}

function VisibilidadeDetalhes(registro) {
    return registro.PossuiPreCTe;
}

function VisibilidadeDadosCarga() {
    return true;
}

function VisibilidadeCartaCorrecao(registro) {
    var opcoesCCe = [EnumGatilhoIrregularidade.CSTICMS, EnumGatilhoIrregularidade.NFeVinculadaAoFrete];

    if (opcoesCCe.indexOf(parseInt(registro.CodigoIrregularidade)))
    return true;
}

function removerHTMLInutil() {
    $('#liAutorizacao').remove();
    $('#liDelegar').remove();
    $('#knockoutAutorizacao').remove();
    $('#knockoutDelegar').remove();
}

function codigosRegistrosSelecionados() {
    var listaCodigos = new Array();

    for (var i = 0; i < _gridControleDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridControleDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }
    return listaCodigos;
}

//#endregion

//#region Pagamento Controle Documento

function pagarConformeFRSClick() {
    let registroSelecionados = _gridControleDocumento.ObterMultiplosSelecionados().map(x => x.Codigo);
    executarReST("ControleDocumento/PagarConformeFRS", { Codigo: JSON.stringify(registroSelecionados) }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        if (arg.Data.Parqueamento)
            return exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);

        if (arg.Data.GerarOcorrencia) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            abrirOcorrencia(arg.Data.CodigoDocumento, arg.Data.CodigCarga);
            return;
        }

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Sucesso ao Pagar Conforme FRS");
    })
}

function pagarConformeDocumentoClick() {
    let registroSelecionados = _gridControleDocumento.ObterMultiplosSelecionados().map(x => x.Codigo);
    executarReST("ControleDocumento/PagarConformeDocumento", { Codigo: JSON.stringify(registroSelecionados) }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    })
}

function pagarConformeOutroValorModalClick() {
    Global.abrirModal("divModalOutroValor");
}
function pagarConformeOutroValorClick(e) {
    let registroSelecionados = _gridControleDocumento.ObterMultiplosSelecionados().map(x => x.Codigo);
    executarReST("ControleDocumento/PagarConformeOutroValor", { Codigo: JSON.stringify(registroSelecionados), NovoValor: e.Valor.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    })
}
 //#endregion