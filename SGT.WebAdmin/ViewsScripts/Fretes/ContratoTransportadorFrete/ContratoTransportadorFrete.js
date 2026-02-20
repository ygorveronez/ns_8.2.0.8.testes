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
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumNetworkContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumDominioOTM.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumCategoriaContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumCluster.js" />
/// <reference path="../../Enumeradores/EnumEquipeContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumHubNonHub.js" />
/// <reference path="../../Enumeradores/EnumModoContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumPessoaJuridicaContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumProcessoAprovacaoContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumStatusAprovacaoTransportador.js" />
/// <reference path="../../Enumeradores/EnumSubCategoriaContratoTransporte.js" />
/// <reference path="../../Enumeradores/EnumTipoContratoTransporte.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoTransporteFrete;
var _pesquisaContratoTransporteFrete;
var _gridContratoTransporteFrete;
var _pesquisaHistoricoIntegracao;

var ContratoTransporteFrete = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.NumeroContrato = PropertyEntity({ text: "*Número do Contrato:", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: ko.observable(validarBloqueioNumeroContrato()), required: ko.observable(validarBloqueioNumeroContrato()) });
    this.ContratoExternoId = PropertyEntity({ text: "Id contrato externo:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.NomeDoContrato = PropertyEntity({ text: "*Nome do contrato:", required: ko.observable(true), maxlength: 100, enable: ko.observable(true) });
    this.AprovacaoAdiconalRequerida = PropertyEntity({ text: ko.observable("Aprovação adicional requerida:"), val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: ko.observable(EnumSimNao.Sim), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)});
    this.Cluster = PropertyEntity({ text: ko.observable("Cluster:"), val: ko.observable(EnumCluster.Brasil), options: EnumCluster.ObterOpcoes(), def: EnumCluster.Brasil, enable: false, required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)});
    this.Pais = PropertyEntity({ text: ko.observable("*País:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.Network = PropertyEntity({ text: ko.observable("Network:"), val: ko.observable(EnumNetworkContratoTransporte.MarketSouthAmerica), options: EnumNetworkContratoTransporte.ObterOpcoes(), def: EnumNetworkContratoTransporte.MarketSouthAmerica, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)});
    this.Equipe = PropertyEntity({ text: ko.observable("Equipe:"), val: ko.observable(EnumEquipeContratoTransporte.AfricaProcurementLogistics), options: EnumEquipeContratoTransporte.ObterOpcoes(), def: ko.observable(EnumEquipeContratoTransporte.AfricaProcurementLogistics), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.Categoria = PropertyEntity({ text: ko.observable("Categoria:"), val: ko.observable(EnumCategoriaContratoTransporte.Inbound), options: ko.observable(EnumCategoriaContratoTransporte.ObterOpcoes()), def: ko.observable(EnumCategoriaContratoTransporte.Inbound), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.SubCategoria = PropertyEntity({ text: ko.observable("SubCategoria:"), val: ko.observable(EnumSubCategoriaContratoTransporte.Freight), options: ko.observable(EnumSubCategoriaContratoTransporte.ObterOpcoes()), def: ko.observable(EnumSubCategoriaContratoTransporte.Freight), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.ModoContrato = PropertyEntity({ text: ko.observable("Modo Contrato:"), val: ko.observable(EnumModoContratoTransporte.BRRodoviario), options: ko.observable(EnumModoContratoTransporte.ObterOpcoes()), def: ko.observable(EnumModoContratoTransporte.BRRodoviario), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.Transportador = PropertyEntity({ text: ko.observable("*Transportador:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.ConformidadeComRSP = PropertyEntity({ text: ko.observable("Conformidade com RSP?"), val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)});
    this.PessoaJuridica = PropertyEntity({ text: ko.observable("Pessoa Juridica:"), val: ko.observable(EnumPessoaJuridicaContratoTransporte.ULBRUnileverBrasilLtda), options: ko.observable(EnumPessoaJuridicaContratoTransporte.ObterOpcoes()), def: ko.observable(EnumPessoaJuridicaContratoTransporte.ULBRUnileverBrasilLtda), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.TipoContrato = PropertyEntity({ text: ko.observable("Tipo Contrato:"), val: ko.observable(EnumTipoContratoTransporte.CTC), options: EnumTipoContratoTransporte.ObterOpcoes(), def: EnumTipoContratoTransporte.CTC, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.HubNonHub = PropertyEntity({ text: ko.observable("Hub/Non-Hub:"), val: ko.observable(EnumHubNonHub.NonHub), options: EnumHubNonHub.ObterOpcoes(), def: EnumHubNonHub.NonHub, enable: false });
    this.DominioOTM = PropertyEntity({ text: ko.observable("Domínio OTM:"), val: ko.observable(EnumDominioOTM.SAO), options: EnumDominioOTM.obterOpcoes(), def: EnumDominioOTM.SAO, enable: false });
    this.DataInicial = PropertyEntity({ text: ko.observable("*Data Inicial:"), getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: ko.observable("*Data Final:"), getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.Moeda = PropertyEntity({ text: ko.observable("Moeda :"), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.ValorPrevistoContrato = PropertyEntity({ text: ko.observable("Valor previsto contrato: "), def: ko.observable("0,00"), val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.Padrao = PropertyEntity({ text: ko.observable("Padrão?"), val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, enable: false });
    this.TermosPagamento = PropertyEntity({ text: ko.observable("Termos Pagamento:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.ClausulaPenal = PropertyEntity({ text: ko.observable("Clausula Penal:"), val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Nao, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.Observacao = PropertyEntity({ text: ko.observable("Observação:"), maxlength: 255, enable: ko.observable(true) });
    this.UsuarioContrato = PropertyEntity({ text: ko.observable("Usuário contrato:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.StatusAprovacaoTransportador = PropertyEntity({ text: ko.observable("Status Aprovação Transportador:"), val: ko.observable(EnumStatusAprovacaoTransportador.AguardandoAprovacao), options: EnumStatusAprovacaoTransportador.ObterOpcoes(), def: EnumStatusAprovacaoTransportador.AguardandoAprovacao, enable: false });
    this.StatusAssinaturaContrato = PropertyEntity({ text: ko.observable("Status Assinatura Contrato:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.ProcessoAprovacao = PropertyEntity({ text: ko.observable("Processo Aprovação:"), val: ko.observable(EnumProcessoAprovacaoContratoTransporte.AprovacaoDiretaVisivelFornecedor), options: EnumProcessoAprovacaoContratoTransporte.ObterOpcoes(), def: EnumProcessoAprovacaoContratoTransporte.AprovacaoDiretaVisivelFornecedor, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.Situacao = PropertyEntity({ text: ko.observable("Situação:"), val: ko.observable(true), options: _status, def: true, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)});
    
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Anexos = PropertyEntity({ eventClick: AbrirTelaAnexoClick, type: types.event, text: "Anexos", visible: ko.observable(true), icon: "fa fa-file-zip-o" });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaContratoTransporteFrete = function () {
    this.NumeroContrato = PropertyEntity({ text: "Número do Contrato:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(true) });
    this.ContratoExternoId = PropertyEntity({ text: "Id contrato externo:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Categoria = PropertyEntity({ text: "Categoria:", val: ko.observable(EnumCategoriaContratoTransporte.Todos), options: EnumCategoriaContratoTransporte.ObterOpcoesPesquisa(), def: EnumCategoriaContratoTransporte.Todos });
    this.SubCategoria = PropertyEntity({ text: "SubCategoria:", val: ko.observable(EnumSubCategoriaContratoTransporte.Todos), options: EnumSubCategoriaContratoTransporte.ObterOpcoesPesquisa(), def: EnumSubCategoriaContratoTransporte.Todos });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.PessoaJuridica = PropertyEntity({ text: "Pessoa Juridica:", val: ko.observable(EnumPessoaJuridicaContratoTransporte.Todos), options: ko.observable(EnumPessoaJuridicaContratoTransporte.ObterOpcoesPesquisa()), def: EnumPessoaJuridicaContratoTransporte.Todos });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, enable: ko.observable(true) });
    this.StatusAprovacaoTransportador = PropertyEntity({ text: "Status Aprovação Transportador:", val: ko.observable(EnumStatusAprovacaoTransportador.Todos), options: EnumStatusAprovacaoTransportador.ObterOpcoesPesquisa(), def: EnumStatusAprovacaoTransportador.Todos, enable: false });
    this.StatusAssinaturaContrato = PropertyEntity({ text: ko.observable("Status Assinatura Contrato:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.SomenteContratosVigentes = PropertyEntity({ text: "Somente Contratos Vigentes:", val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.ReenviarIntegracaoLote = PropertyEntity({ eventClick: reenviarContratosIntegracao, type: types.event, text: "Reenviar Contratos", idGrid: guid(), visible: ko.observable(true) });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(EnumSituacaoIntegracao.Todas), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoTransporteFrete.CarregarGrid();
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

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};


//*******EVENTOS*******
function loadContratoTransportadorFrete() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaContratoTransporteFrete = new PesquisaContratoTransporteFrete();
    KoBindings(_pesquisaContratoTransporteFrete, "knockoutPesquisaContratoTransporteFrete", false, _pesquisaContratoTransporteFrete.Pesquisar.id);

    // Instancia objeto principal
    _contratoTransporteFrete = new ContratoTransporteFrete();
    KoBindings(_contratoTransporteFrete, "knockoutContratoTransporteFrete");

    // Inicia busca
    verificarIntegracaoLBC(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
    buscarContratoTransporteFrete();
    loadAnexoContratoTransportadorFrete();

    new BuscarTransportadores(_pesquisaContratoTransporteFrete.Transportador);
    new BuscarTransportadores(_contratoTransporteFrete.Transportador);
    new BuscarFuncionario(_contratoTransporteFrete.UsuarioContrato);
    new BuscarPaises(_contratoTransporteFrete.Pais);
    new BuscarTermosPagamento(_contratoTransporteFrete.TermosPagamento);
    new BuscarStatusAssinaturaContrato(_contratoTransporteFrete.StatusAssinaturaContrato);
    new BuscarStatusAssinaturaContrato(_pesquisaContratoTransporteFrete.StatusAssinaturaContrato);
}

function adicionarClick(e, sender) {
    if (!verificarObrigatoriedadeAnexos())
        return;

    Salvar(_contratoTransporteFrete, "ContratoTransporteFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                enviarArquivosAnexados(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridContratoTransporteFrete.CarregarGrid();
                limparCamposContratoTransporteFrete();
                validarBloqueioCampos(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!verificarObrigatoriedadeAnexos())
        return;

    Salvar(_contratoTransporteFrete, "ContratoTransporteFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                enviarArquivosAnexados(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContratoTransporteFrete.CarregarGrid();
                limparCamposContratoTransporteFrete();
                validarBloqueioCampos(arg.Data);
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
        ExcluirPorCodigo(_contratoTransporteFrete, "ContratoTransporteFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridContratoTransporteFrete.CarregarGrid();
                    limparCamposContratoTransporteFrete();
                    validarBloqueioCampos(arg.Data);
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
    limparCamposContratoTransporteFrete();
    validarBloqueioCampos(null, true);
}

function editarContratoTransporteFreteClick(itemGrid) {
    // Limpa os campos
    limparCamposContratoTransporteFrete();

    // Seta o codigo do objeto
    _contratoTransporteFrete.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_contratoTransporteFrete, "ContratoTransporteFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaContratoTransporteFrete.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _anexo.Anexos.val(arg.Data.Anexos);
                _contratoTransporteFrete.Atualizar.visible(true);
                _contratoTransporteFrete.Excluir.visible(true);
                _contratoTransporteFrete.Cancelar.visible(true);
                _contratoTransporteFrete.Adicionar.visible(false);
                validarBloqueioCampos(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function historicoContratoTransporteFreteClick(data) {
    BuscarHistoricoIntegracao(data);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function reenviarContratoTransporteFreteClick(data) {
    executarReST("ContratoTransporteFrete/ReenviarIntegracaoContrato", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Contrato enviado para a fila de integração.");
            _gridContratoTransporteFrete.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}


//*******MÉTODOS*******
function buscarContratoTransporteFrete() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoTransporteFreteClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de integrações", id: guid(), metodo: historicoContratoTransporteFreteClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarContratoTransporteFreteClick, tamanho: "20", icone: "" });

    var configuracoesExportacao = { url: "ContratoTransporteFrete/ExportarPesquisa", titulo: "Contrato de Transporte de Frete" };

    _gridContratoTransporteFrete = new GridViewExportacao(_pesquisaContratoTransporteFrete.Pesquisar.idGrid, "ContratoTransporteFrete/Pesquisa", _pesquisaContratoTransporteFrete, menuOpcoes, configuracoesExportacao);
    _gridContratoTransporteFrete.CarregarGrid();
}

function limparCamposContratoTransporteFrete() {
    _contratoTransporteFrete.Atualizar.visible(false);
    _contratoTransporteFrete.Cancelar.visible(false);
    _contratoTransporteFrete.Excluir.visible(false);
    _contratoTransporteFrete.Adicionar.visible(true);
    LimparCampos(_contratoTransporteFrete);
    limparCamposAnexo();
    _gridContratoTransporteFrete.CarregarGrid();
}

function validarBloqueioCampos(data, cancelar) {
    if (data != null && data.PermiteEdicao != null) {
        _contratoTransporteFrete.NumeroContrato.enable(data.PermiteEdicao && validarBloqueioNumeroContrato());
        _contratoTransporteFrete.NomeDoContrato.enable(data.PermiteEdicao);
        _contratoTransporteFrete.AprovacaoAdiconalRequerida.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Pais.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Network.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Equipe.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Categoria.enable(data.PermiteEdicao);
        _contratoTransporteFrete.SubCategoria.enable(data.PermiteEdicao);
        _contratoTransporteFrete.ModoContrato.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Transportador.enable(data.PermiteEdicao);
        _contratoTransporteFrete.ConformidadeComRSP.enable(data.PermiteEdicao);
        _contratoTransporteFrete.PessoaJuridica.enable(data.PermiteEdicao);
        _contratoTransporteFrete.TipoContrato.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Moeda.enable(data.PermiteEdicao);
        _contratoTransporteFrete.DataInicial.enable(data.PermiteEdicao);
        _contratoTransporteFrete.DataFinal.enable(data.PermiteEdicao);
        _contratoTransporteFrete.ValorPrevistoContrato.enable(data.PermiteEdicao);
        _contratoTransporteFrete.TermosPagamento.enable(data.PermiteEdicao);
        _contratoTransporteFrete.UsuarioContrato.enable(data.PermiteEdicao);
        _contratoTransporteFrete.ClausulaPenal.enable(data.PermiteEdicao);
        _contratoTransporteFrete.ProcessoAprovacao.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Situacao.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Observacao.enable(data.PermiteEdicao);
        _contratoTransporteFrete.Atualizar.visible(data.PermiteEdicao);
    } else {
        _contratoTransporteFrete.NumeroContrato.enable(true && validarBloqueioNumeroContrato());
        _contratoTransporteFrete.NomeDoContrato.enable(true);
        _contratoTransporteFrete.AprovacaoAdiconalRequerida.enable(true);
        _contratoTransporteFrete.Pais.enable(true);
        _contratoTransporteFrete.Network.enable(true);
        _contratoTransporteFrete.Equipe.enable(true);
        _contratoTransporteFrete.Categoria.enable(true);
        _contratoTransporteFrete.SubCategoria.enable(true);
        _contratoTransporteFrete.ModoContrato.enable(true);
        _contratoTransporteFrete.Transportador.enable(true);
        _contratoTransporteFrete.ConformidadeComRSP.enable(true);
        _contratoTransporteFrete.PessoaJuridica.enable(true);
        _contratoTransporteFrete.TipoContrato.enable(true);
        _contratoTransporteFrete.Moeda.enable(true);
        _contratoTransporteFrete.DataInicial.enable(true);
        _contratoTransporteFrete.DataFinal.enable(true);
        _contratoTransporteFrete.ValorPrevistoContrato.enable(true);
        _contratoTransporteFrete.TermosPagamento.enable(true);
        _contratoTransporteFrete.UsuarioContrato.enable(true);
        _contratoTransporteFrete.ClausulaPenal.enable(true);
        _contratoTransporteFrete.ProcessoAprovacao.enable(true);
        _contratoTransporteFrete.Situacao.enable(true);
        _contratoTransporteFrete.Observacao.enable(true);

        if (cancelar)
            _contratoTransporteFrete.Atualizar.visible(false);
        else
            _contratoTransporteFrete.Atualizar.visible(true);
    }
}

function verificarObrigatoriedadeAnexos() {
    if (_ObrigarAnexosContratoTransportadorFrete && !isPossuiAnexo()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório adicionar anexos para salvar o contrato.");
        return false;
    }

    return true;
}

function validarBloqueioNumeroContrato() {
    return !_GerarNumeroContratoTransportadorFreteSequencial;
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "ContratoTransporteFrete/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historico) {
    executarDownload("ContratoTransporteFrete/DownloadArquivosHistoricoIntegracao", { Codigo: historico.Codigo });
}

function reenviarContratosIntegracao() {
    executarReST("ContratoTransporteFrete/ReenviarIntegracoesFalha", null, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Contratos reenviados para a fila de integração.");
            _gridContratoTransporteFrete.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function verificarIntegracaoLBC(config) {
    if (config) {
        _contratoTransporteFrete.AprovacaoAdiconalRequerida.text("*Aprovação adicional requerida:");
        _contratoTransporteFrete.AprovacaoAdiconalRequerida.val(EnumSimNao.Nao);
        _contratoTransporteFrete.AprovacaoAdiconalRequerida.def(EnumSimNao.Nao);
        _contratoTransporteFrete.Cluster.text("*Cluster:");
        _contratoTransporteFrete.Network.text("*Newwork:");
        _contratoTransporteFrete.Equipe.text("*Equipe:");
        _contratoTransporteFrete.Equipe.val(EnumEquipeContratoTransporte.SouthAmericaSupplyChainProcurement);
        _contratoTransporteFrete.Equipe.def(EnumEquipeContratoTransporte.SouthAmericaSupplyChainProcurement);
        _contratoTransporteFrete.Categoria.text("*Categoria");
        _contratoTransporteFrete.Categoria.options(EnumCategoriaContratoTransporte.ObterOpcoesPesquisaIntegracao());
        _contratoTransporteFrete.Categoria.val(EnumCategoriaContratoTransporte.Selecione);
        _contratoTransporteFrete.Categoria.def(EnumCategoriaContratoTransporte.Selecione);
        _contratoTransporteFrete.SubCategoria.text("*SubCategoria");
        _contratoTransporteFrete.SubCategoria.options(EnumSubCategoriaContratoTransporte.ObterOpcoesPesquisaIntegracao());
        _contratoTransporteFrete.SubCategoria.options(EnumSubCategoriaContratoTransporte.ObterOpcoesPesquisaIntegracao());
        _contratoTransporteFrete.SubCategoria.val(EnumSubCategoriaContratoTransporte.Selecione);
        _contratoTransporteFrete.SubCategoria.def(EnumSubCategoriaContratoTransporte.Selecione);
        _contratoTransporteFrete.ModoContrato.text("*Modo Contrato"); 
        _contratoTransporteFrete.ModoContrato.options(EnumModoContratoTransporte.ObterOpcoesPesquisaIntegracao());
        _contratoTransporteFrete.ModoContrato.val(EnumModoContratoTransporte.Selecione);
        _contratoTransporteFrete.ModoContrato.def(EnumModoContratoTransporte.Selecione);
        _contratoTransporteFrete.ConformidadeComRSP.text("*Conformidade com RSP");
        _contratoTransporteFrete.PessoaJuridica.text("*Pessoa Juridica");
        _contratoTransporteFrete.PessoaJuridica.options(EnumPessoaJuridicaContratoTransporte.ObterOpcoesPesquisaIntegracao());
        _contratoTransporteFrete.PessoaJuridica.val(EnumPessoaJuridicaContratoTransporte.Selecione);
        _contratoTransporteFrete.PessoaJuridica.def(EnumPessoaJuridicaContratoTransporte.Selecione);
        _contratoTransporteFrete.TipoContrato.text("*Tipo de Contrato");
        _contratoTransporteFrete.Moeda.text("*Moeda");
        _contratoTransporteFrete.ValorPrevistoContrato.text("*Valor Previsto do Contrato");
        _contratoTransporteFrete.TermosPagamento.text("*Termo Pagamento");
        _contratoTransporteFrete.UsuarioContrato.text("*Usuário Contrato");
        _contratoTransporteFrete.ClausulaPenal.text("*Cláusula Penal");
        _contratoTransporteFrete.ProcessoAprovacao.text("*Processo Aprovação");
        _contratoTransporteFrete.Situacao.text("*Situação");
        _contratoTransporteFrete.ValorPrevistoContrato.def("");
        _contratoTransporteFrete.ValorPrevistoContrato.val("");
    }
}