/// <reference path="FilaCarregamentoAdicionar.js" />
/// <reference path="FilaCarregamentoDetalhes.js" />
/// <reference path="FilaCarregamentoResumo.js" />
/// <reference path="FilaCarregamentoSignalR.js" />
/// <reference path="FilaCarregamentoSituacao.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/ConfiguracaoProgramacaoCarga.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoModeloVeicular.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/MotivoAlteracaoPosicaoFilaCarregamento.js" />
/// <reference path="../../Consultas/MotivoRetiradaFilaCarregamento.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculoPesquisa.js" />
/// <reference path="../../Logistica/PosicaoAtualVeiculo/PosicaoAtualVeiculo.js" />

// #region Objetos Globais do Arquivo

var _conjuntoMotorista;
var _conjuntoVeiculo;
var _fila;
var _filaAlocarCarga;
var _filaAlterarCentroCarregamento;
var _filaCarregamentoLegenda;
var _filaCarregamentoResumoPorSituacao;
var _filaCarregamentoSituacao;
var _filaEmTransicao;
var _filaEmTransicaoCadastro;
var _filaLiberacaoSaida;
var _filaMotorista;
var _filaMotoristaCadastro;
var _filaReposicionar;
var _gridFilaCarregamento;
var _gridFilaEmTransicao;
var _gridFilaMotorista;
var _listaTipoRetornoCarga;
var _pesquisaFilaCarregamento;
var _pesquisaFilaCarregamentoAuxiliar;
var _pesquisaFilaCarregamentoCargaAlocar;
var _transportadorLogado;
var _usuarioPossuiPermissaoAdicionar = false;
var _usuarioPossuiPermissaoAlterarCentroCarregamento = false;
var _usuarioPossuiPermissaoInformarConjuntoMotorista = false;
var _usuarioPossuiPermissaoRemover = false;
var _informarEquipamento = false;

var situacoesPreSelecionadas = [
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.AguardandoAceite,
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.AguardandoConjuntos,
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.AguardandoAceitePreCarga,
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.Vazio,
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.EmViagem,
    EnumSituacaoFilaCarregamentoVeiculoPesquisa.AguardandoCarga
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConjuntoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: conjuntoMotoristaClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
};

var ConjuntoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tração:", idBtnSearch: guid(), required: true });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0) });

    this.Adicionar = PropertyEntity({ eventClick: conjuntoVeiculoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
};

var InformarEquipamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Equipamento:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: informarEquipamentoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}
var PesquisaFilaCarregamento = function () {
    const dataProgramadaInicial = _CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? Global.DataAtual() : "";
    const dataProgramadaFinal = _CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? moment().add(_CONFIGURACAO_TMS.DiasFiltrarDataProgramada, 'days').format("DD/MM/YYYY") : "";
    const situacoes = _CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? situacoesPreSelecionadas : [];

    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", issue: 320, idBtnSearch: guid(), required: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga, visible: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga, visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga, multiplesEntitiesConfig: { propCodigo: "Codigo", propDescricao: "CodigoCargaEmbarcador" } });
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Configuração de Pré Planejamento:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.DataProgramadaInicial = PropertyEntity({ text: "Data de Chegada de:", getType: typesKnockout.date, val: ko.observable(dataProgramadaInicial), def: dataProgramadaInicial, visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.DataProgramadaFinal = PropertyEntity({ text: "Data de Chegada até:", getType: typesKnockout.date, val: ko.observable(dataProgramadaFinal), def: dataProgramadaFinal, visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Modelo Veicular:", idBtnSearch: guid(), visible: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.ModeloVeicular = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cidade de Destino:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Região de Destino:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação", getType: typesKnockout.selectMultiple, val: ko.observable(situacoes), options: EnumSituacaoFilaCarregamentoVeiculoPesquisa.obterOpcoesPesquisa(), def: situacoes, visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });

    this.DataProgramadaInicial.dateRangeLimit = this.DataProgramadaFinal;
    this.DataProgramadaFinal.dateRangeInit = this.DataProgramadaInicial;

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaFilaCarregamento)) {
                $("#fila-carregamento-container").removeClass("d-none");
                _pesquisaFilaCarregamento.ExibirFiltros.visibleFade(false);
                atualizarFiltrosUltimaPesquisa();
                recarregarFilaCarregamento();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var PesquisaFilaCarregamentoAuxiliar = function () {
    this.Carga = PropertyEntity({ type: types.multiplesEntities });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.multiplesEntities });
    this.DataProgramadaInicial = PropertyEntity({ getType: typesKnockout.date });
    this.DataProgramadaFinal = PropertyEntity({ getType: typesKnockout.date });
    this.Destino = PropertyEntity({ type: types.multiplesEntities });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), });
    this.ModeloVeicular = PropertyEntity({ type: types.multiplesEntities });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities });
    this.Situacao = PropertyEntity({ getType: typesKnockout.selectMultiple });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities });
};

var PesquisaFilaCarregamentoCargaAlocar = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
};

var Fila = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaModalClick, type: types.event, text: ko.observable("Adicionar"), visible: ((_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros && _usuarioPossuiPermissaoAdicionar) || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.TransportadorTerceiro) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar Veículos",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "FilaCarregamento/Importar",
        UrlConfiguracao: "FilaCarregamento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O059_FilaCarregamentoVeiculo,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false,
                CentroCarregamento: _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity(),
                Filial: _pesquisaFilaCarregamentoAuxiliar.Filial.codEntity(),
                Tipo: obterTipoPadrao()
            };
        },
        CallbackImportacao: function () {
            /* . . . */
        }
    });
};

var FilaAlocarCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga: ", idBtnSearch: guid(), required: true });
    this.Observacao = PropertyEntity({ text: "*Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });

    this.Alocar = PropertyEntity({ eventClick: alocarCargaClick, type: types.event, text: ko.observable("Alocar"), visible: true });
};

var FilaAlterarCentroCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });

    this.Alterar = PropertyEntity({ eventClick: alterarCentroCarregamentoClick, type: types.event, text: ko.observable("Alterar"), visible: true });
};

var FilaCarregamentoLegenda = function () {
    this.UtilizarFilaCarregamentoReversa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamentoReversa) });
    this.UtilizarProgramacaoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });

    this.AguardandoAceite = PropertyEntity({ text: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? "Ag. Aceite Pré Carga" : "Ag. Aceite"), visible: ko.observable(true) });
    this.AguardandoAceitePreCarga = PropertyEntity({ text: ko.observable("Ag. Aceite Pré Planejamento"), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.AguardandoCarga = PropertyEntity({ text: ko.observable("Ag. Carga"), visible: ko.observable(true) });
    this.AguardandoConfirmacao = PropertyEntity({ text: ko.observable("Ag. Confirmação"), visible: ko.observable(true) });
    this.AguardandoConjuntos = PropertyEntity({ text: ko.observable("Ag. Conjuntos"), visible: ko.observable(true) });
    this.AguardandoDesatrelar = PropertyEntity({ text: ko.observable("Ag. Desatrelar"), visible: ko.observable(true) });
    this.CargaCancelada = PropertyEntity({ text: ko.observable("Carga Cancelada"), visible: ko.observable(true) });
    this.CargaRecusada = PropertyEntity({ text: ko.observable("Carga Recusada"), visible: ko.observable(!_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.EmChecklist = PropertyEntity({ text: ko.observable("Em Checklist"), visible: ko.observable(true) });
    this.EmRemocao = PropertyEntity({ text: ko.observable("Em Remoção"), visible: ko.observable(true) });
    this.EmReversa = PropertyEntity({ text: ko.observable("Em Reversa"), visible: ko.observable(true) });
    this.EmViagem = PropertyEntity({ text: ko.observable("Em Viagem"), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.PerdeuSenha = PropertyEntity({ text: ko.observable("Perdeu Senha"), visible: ko.observable(true) });
    this.Vazio = PropertyEntity({ text: ko.observable("Vazio"), visible: ko.observable(true) });
};

var FilaCarregamentoResumoPorSituacao = function () {
    this.ExibirResumo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.UtilizarFilaCarregamentoReversa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamentoReversa) });
    this.UtilizarProgramacaoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });

    this.AguardandoAceite = PropertyEntity({ text: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? "Ag. Aceite Pré Carga" : "Ag. Aceite"), visible: ko.observable(true) });
    this.AguardandoAceitePreCarga = PropertyEntity({ text: ko.observable("Ag. Aceite Pré Planejamento"), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.AguardandoCarga = PropertyEntity({ text: ko.observable("Ag. Carga"), visible: ko.observable(true) });
    this.AguardandoConfirmacao = PropertyEntity({ text: ko.observable("Ag. Confirmação"), visible: ko.observable(true) });
    this.AguardandoConjuntos = PropertyEntity({ text: ko.observable("Ag. Conjuntos"), visible: ko.observable(true) });
    this.AguardandoDesatrelar = PropertyEntity({ text: ko.observable("Ag. Desatrelar"), visible: ko.observable(true) });
    this.CargaCancelada = PropertyEntity({ text: ko.observable("Carga Cancelada"), visible: ko.observable(true) });
    this.CargaRecusada = PropertyEntity({ text: ko.observable("Carga Recusada"), visible: ko.observable(!_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.EmChecklist = PropertyEntity({ text: ko.observable("Em Checklist"), visible: ko.observable(true) });
    this.EmRemocao = PropertyEntity({ text: ko.observable("Em Remoção"), visible: ko.observable(true) });
    this.EmReversa = PropertyEntity({ text: ko.observable("Em Reversa"), visible: ko.observable(true) });
    this.EmViagem = PropertyEntity({ text: ko.observable("Em Viagem"), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) });
    this.PerdeuSenha = PropertyEntity({ text: ko.observable("Perdeu Senha"), visible: ko.observable(true) });
    this.Vazio = PropertyEntity({ text: ko.observable("Vazio"), visible: ko.observable(true) });

    this.ExibirDados = PropertyEntity({ eventClick: function (e) { e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade()); }, type: types.event, idFade: guid(), visibleFade: ko.observable(true) });
};

var FilaEmTransicao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaEmTransicaoModalClick, type: types.event, text: ko.observable("Adicionar"), visible: _usuarioPossuiPermissaoAdicionar });
};

var FilaEmTransicaoCadastro = function () {
    const tipo = obterTipoPadrao();

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), required: false });
    this.Tipo = PropertyEntity({ val: ko.observable(tipo), options: _listaTipoRetornoCarga, def: tipo, text: "*Tipo: ", required: true, visible: !(tipo) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaEmTransicaoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
};

var FilaLiberacaoSaida = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: "Motivo:", getType: typesKnockout.string, val: ko.observable(""), enable: false });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Aceitar = PropertyEntity({ eventClick: aceitarSolicitacaoSaidaFilaClick, type: types.event, text: ko.observable("Aceitar"), visible: true });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarSolicitacaoSaidaFilaClick, type: types.event, text: ko.observable("Rejeitar"), visible: true });
};

var FilaMotorista = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaMotoristaModalClick, type: types.event, text: ko.observable("Adicionar"), _usuarioPossuiPermissaoAdicionar });
};

var FilaMotoristaCadastro = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaMotoristaClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
};

var FilaReposicionar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true });
    this.Posicao = PropertyEntity({ text: "*Posição: ", val: ko.observable(""), def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 4, required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: false });
    this.Reposicionar = PropertyEntity({ eventClick: reposicionarFilaCarregamentoClick, type: types.event, text: ko.observable("Reposicionar"), visible: true });
};

// #endregion Classes

// #region Funções de Inicialização

function loadConfiguracaoFilaCarregamento(callback) {
    executarReST("FilaCarregamento/ObterConfiguracao", undefined, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function loadDroppableAlteracaoCentroCarregamento() {
    if (_usuarioPossuiPermissaoAlterarCentroCarregamento) {
        $("#container-grid-fila-carregamento, #container-grid-fila-em-transicao").droppable({
            drop: itemSoltado,
            hoverClass: "ui-state-active backgroundDropHover",
        });
    }
}

function loadDroppableVincularConjuntoMotorista() {
    if (_usuarioPossuiPermissaoInformarConjuntoMotorista) {
        $("#grid-fila-carregamento tbody tr").droppable({
            drop: vincularFilaCarregamentoMotorista,
            hoverClass: "ui-state-active backgroundDropHover",
        });
    }
}

function loadFilaCarregamento() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            loadListaTipoRetornoCarga(function () {
                loadConfiguracaoFilaCarregamento(function (configuracaoFilaCarregamento) {
                    _pesquisaFilaCarregamento = new PesquisaFilaCarregamento();
                    KoBindings(_pesquisaFilaCarregamento, "knockoutPesquisaFilaCarregamento", false, _pesquisaFilaCarregamento.Pesquisar.id);

                    _filaCarregamentoLegenda = new FilaCarregamentoLegenda();
                    KoBindings(_filaCarregamentoLegenda, "knockoutFilaCarregamentoLegenda")
                    PreencherObjetoKnoutLegenda(_filaCarregamentoLegenda, configuracaoFilaCarregamento.Legendas);

                    _filaCarregamentoResumoPorSituacao = new FilaCarregamentoResumoPorSituacao();
                    KoBindings(_filaCarregamentoResumoPorSituacao, "knockoutFilaCarregamentoResumoPorSituacao")
                    PreencherObjetoKnoutLegenda(_filaCarregamentoResumoPorSituacao, configuracaoFilaCarregamento.Legendas);

                    _transportadorLogado = configuracaoFilaCarregamento.Transportador;
                    _pesquisaFilaCarregamentoAuxiliar = new PesquisaFilaCarregamentoAuxiliar();

                    _usuarioPossuiPermissaoAdicionar = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAdicionar, _PermissoesPersonalizadas);
                    _usuarioPossuiPermissaoAlterarCentroCarregamento = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarCentroCarregamento, _PermissoesPersonalizadas);
                    _usuarioPossuiPermissaoInformarConjuntoMotorista = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista, _PermissoesPersonalizadas);

                    // Valida apenas as permissões normais e naõ o "TipoServicoMultisoftware"" logado
                    _usuarioPossuiPermissaoRemover = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemover, _PermissoesPersonalizadas);
                    //_usuarioPossuiPermissaoRemover = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemover, _PermissoesPersonalizadas);

                    if (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga) {
                        _usuarioPossuiPermissaoAdicionar = true;
                        _usuarioPossuiPermissaoInformarConjuntoMotorista = true;
                        _usuarioPossuiPermissaoRemover = true;
                    }

                    BuscarCargas(_pesquisaFilaCarregamento.Carga);
                    BuscarCentrosCarregamento(_pesquisaFilaCarregamento.CentroCarregamento, retornoConsultaCentroCarregamento);
                    BuscarConfiguracaoProgramacaoCarga(_pesquisaFilaCarregamento.ConfiguracaoProgramacaoCarga);
                    BuscarFilial(_pesquisaFilaCarregamento.Filial, null, null, null, null, true);
                    BuscarGrupoModeloVeicular(_pesquisaFilaCarregamento.GrupoModeloVeicular);
                    BuscarModelosVeicularesCarga(_pesquisaFilaCarregamento.ModeloVeicular);
                    BuscarLocalidades(_pesquisaFilaCarregamento.Destino);
                    BuscarEstados(_pesquisaFilaCarregamento.EstadoDestino);
                    BuscarRegioes(_pesquisaFilaCarregamento.RegiaoDestino);
                    BuscarTiposdeCarga(_pesquisaFilaCarregamento.TipoCarga);
                    BuscarTiposOperacao(_pesquisaFilaCarregamento.TipoOperacao);
                    BuscarTransportadores(_pesquisaFilaCarregamento.Transportador);
                    BuscarVeiculos(_pesquisaFilaCarregamento.Veiculo, null, null, null, null, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);

                    loadFilaCarregamentoSignalR();
                    loadFilaCarregamentoDetalhes();
                    loadFilaCarregamentoDetalhesMotorista();
                    loadFilaCarregamentoEnvioNotificacao();
                    loadFilaCarregamentoAdicionar();
                    loadPreCargaDadosParaTransporte();
                    loadPosicaoAtualVeiculo();
                    loadGridFilaCarregamento();
                    loadFilaCarregamentoEnvioNotificacaoSMS();
                    loadEnviarNotificacaoApp();

                    _fila = new Fila();
                    KoBindings(_fila, "knockoutFila");

                    _filaLiberacaoSaida = new FilaLiberacaoSaida();
                    KoBindings(_filaLiberacaoSaida, "knockoutLiberacaoSaida");

                    _filaCarregamentoSituacao = new FilaCarregamentoSituacao();
                    KoBindings(_filaCarregamentoSituacao, "knockoutFilaCarregamentoSituacaoRemocao");

                    _conjuntoMotorista = new ConjuntoMotorista();
                    KoBindings(_conjuntoMotorista, "knockoutConjuntoMotorista");

                    _conjuntoVeiculo = new ConjuntoVeiculo();
                    KoBindings(_conjuntoVeiculo, "knockoutConjuntoVeiculo");

                    _informarEquipamento = new InformarEquipamento();
                    KoBindings(_informarEquipamento, "knockoutInformarEquipamento")

                    BuscarMotivoRetiradaFilaCarregamento(_filaCarregamentoSituacao.Motivo);
                    BuscarMotoristasMobile(_conjuntoMotorista.Motorista);

                    var knoutTransportador = _CONFIGURACAO_TMS.FiltrarBuscaVeiculosPorEmpresa ? _conjuntoVeiculo.Transportador : undefined;

                    BuscarVeiculos(
                        _conjuntoVeiculo.Tracao,
                        undefined,
                        knoutTransportador,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        "0",
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        true
                    );

                    if (isExibirTodasFilasCarregamento()) {
                        window.onscroll = filasPosicaoFixaOnScroll;

                        _filaEmTransicao = new FilaEmTransicao();
                        KoBindings(_filaEmTransicao, "knockoutFilaEmTransicao");

                        _filaEmTransicaoCadastro = new FilaEmTransicaoCadastro();
                        KoBindings(_filaEmTransicaoCadastro, "knockoutCadastroFilaEmTransicao");

                        _filaMotorista = new FilaMotorista();
                        KoBindings(_filaMotorista, "knockoutFilaMotorista");

                        _filaMotoristaCadastro = new FilaMotoristaCadastro();
                        KoBindings(_filaMotoristaCadastro, "knockoutCadastroFilaMotorista");

                        _filaReposicionar = new FilaReposicionar();
                        KoBindings(_filaReposicionar, "knockoutReposicionar");

                        _filaAlterarCentroCarregamento = new FilaAlterarCentroCarregamento();
                        KoBindings(_filaAlterarCentroCarregamento, "knockoutAlterarCentroCarregamento");

                        _filaAlocarCarga = new FilaAlocarCarga();
                        KoBindings(_filaAlocarCarga, "knockoutAlocarCarga");

                        _pesquisaFilaCarregamentoCargaAlocar = new PesquisaFilaCarregamentoCargaAlocar();

                        BuscarCargaParaFilaCarregamento(_filaAlocarCarga.Carga, null, null, _pesquisaFilaCarregamentoCargaAlocar.Filial, _pesquisaFilaCarregamentoCargaAlocar.ModeloVeicular);
                        BuscarCentrosCarregamento(_filaAlterarCentroCarregamento.CentroCarregamento);
                        BuscarMotoristasMobile(_filaEmTransicaoCadastro.Motorista);
                        BuscarMotoristasMobile(_filaMotoristaCadastro.Motorista);
                        BuscarMotivoAlteracaoPosicaoFilaCarregamento(_filaReposicionar.Motivo);
                        BuscarVeiculos(_filaEmTransicaoCadastro.Veiculo, undefined, undefined, undefined, _filaEmTransicaoCadastro.Motorista, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);
                        BuscarVeiculos(_filaMotoristaCadastro.Veiculo, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, "0", undefined, undefined, undefined, undefined, undefined, true);
                        BuscarEquipamentos(_informarEquipamento.Equipamento);

                        loadFilaCarregamentoResumo();
                        loadGridFilaEmTransicao();
                        loadGridFilaMotorista();
                        loadDroppableAlteracaoCentroCarregamento();
                    }
                    else {
                        $("#fila-carregamento-container-grafico").remove();
                        $("#container-fila-conteudo-fixar").remove();
                        $("#container-fila-conteudo").removeClass("col-md-8");
                    }
                });
            });
        });
    });
}

function loadGridFilaCarregamento() {
    const limiteRegistros = 20;
    const totalRegistrosPorPagina = 20;
    const ordenacaoPadrao = { column: 0, dir: orderDir.asc };
    const opcaoAceitarCarga = { descricao: "Aceitar Carga", id: guid(), evento: "onclick", metodo: aceitarCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirAceitarCarga };
    const opcaoAceitarPreCarga = { descricao: "Aceitar Pré Planejamento", id: guid(), evento: "onclick", metodo: aceitarPreCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirAceitarOuRecusarPreCarga };
    const opcaoAlocarCarga = { descricao: "Alocar Carga", id: guid(), evento: "onclick", metodo: alocarCargaModalClick, tamanho: "10", icone: "" };
    const opcaoAlterarCentroCarregamento = { descricao: "Alterar CD", id: guid(), evento: "onclick", metodo: alterarCentroCarregamentoModalClick, tamanho: "10", icone: "" };
    const opcaoConfirmarChegada = { descricao: "Confirmar Chegada", id: guid(), evento: "onclick", metodo: confirmarChegadaClick, tamanho: "10", icone: "" };
    const opcaoDesatrelarTracao = { descricao: "Desatrelar Tração", id: guid(), evento: "onclick", metodo: desatrelarTracaoClick, tamanho: "10", icone: "" };
    const opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesClick, tamanho: "10", icone: "" };
    const opcaoDetalhesCarga = { descricao: "Detalhes da Carga", id: guid(), evento: "onclick", metodo: exibirDetalhesCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirVisualizarDetalhesCarga };
    const opcaoDetalhesMotorista = { descricao: "Detalhes do Motorista", id: guid(), evento: "onclick", metodo: detalhesMotoristaClick, tamanho: "10", icone: "" };
    const opcaoDetalhesPreCarga = { descricao: "Detalhes do Pré Planejamento", id: guid(), evento: "onclick", metodo: exibirDetalhesPreCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirVisualizarDetalhesPreCarga };
    const opcaoEnviarNotificacao = { descricao: "Enviar Notificação", id: guid(), evento: "onclick", metodo: enviarNotificacaoClick, tamanho: "10", icone: "" };
    const opcaoConjuntoMotorista = { descricao: "Informar Motorista", id: guid(), evento: "onclick", metodo: conjuntoMotoristaModalClick, tamanho: "10", icone: "", visibilidade: isPermitirInformarConjuntoMotorista };
    const opcaoConjuntoVeiculo = { descricao: "Informar Tração", id: guid(), evento: "onclick", metodo: conjuntoVeiculoModalClick, tamanho: "10", icone: "", visibilidade: isPermitirInformarConjuntoVeiculo };
    const opcaoLiberar = { descricao: "Liberar", id: guid(), evento: "onclick", metodo: liberarClick, tamanho: "10", icone: "" };
    const opcaoLiberarSaidaFila = { descricao: "Liberar Saída", id: guid(), evento: "onclick", metodo: liberarSaidaFilaModalClick, tamanho: "10", icone: "", visibilidade: isPermitirLiberarSaidaFila };
    const opcaoPosicaoAtualVeiculo = { descricao: "Posição Atual do Veículo", id: guid(), evento: "onclick", metodo: exibirPosicaoAtualVeiculoClick, tamanho: "10", icone: "", visibilidade: isPermitirExibirPosicaoAtualVeiculo };
    const opcaoPrimeiraPosicao = { descricao: "Primeira Posição", id: guid(), evento: "onclick", metodo: alterarPrimeiraPosicaoFilaCarregamentoClick, tamanho: "10", icone: "" };
    const opcaoRecusarCarga = { descricao: "Recusar Carga", id: guid(), evento: "onclick", metodo: recusarCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirRecusarCarga };
    const opcaoRecusarPreCarga = { descricao: "Recusar Pré Planejamento", id: guid(), evento: "onclick", metodo: recusarPreCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirAceitarOuRecusarPreCarga };
    const opcaoRemover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFilaCarregamentoClick, tamanho: "10", icone: "" };
    const opcaoRemoverConjuntoMotorista = { descricao: "Remover Motorista", id: guid(), evento: "onclick", metodo: removerConjuntoMotoristaClick, tamanho: "10", icone: "" };
    const opcaoRemoverReversa = { descricao: "Remover Reversa", id: guid(), evento: "onclick", metodo: removerReversaClick, tamanho: "10", icone: "" };
    const opcaoRemoverTracao = { descricao: "Remover Tração", id: guid(), evento: "onclick", metodo: removerTracaoClick, tamanho: "10", icone: "" };
    const opcaoReposicionar = { descricao: "Reposicionar", id: guid(), evento: "onclick", metodo: reposicionarFilaCarregamentoModalClick, tamanho: "10", icone: "" };
    const opcaoUltimaPosicao = { descricao: "Última Posição", id: guid(), evento: "onclick", metodo: alterarUltimaPosicaoFilaCarregamentoClick, tamanho: "10", icone: "" };
    const opcaoEnviarNotificacaoMotorista = { descricao: "Enviar Notificação SMS", id: guid(), evento: "onclick", metodo: enviarNotificacaoSMSClick, tamanho: "10", icone: "" };
    const opcaoEnviarNotificacaoSuperAppMotorista = { descricao: "Enviar Notificação Super App", id: guid(), evento: "onclick", metodo: fcEnviarNotificacaoSuperAppClick, tamanho: "10", icone: "", visibilidade: isPermitirEnviarNotificacaoSuperApp };
    const opcaoInformarEquipamento = { descricao: "Informar Equipamento", id: guid(), evento: "onclick", metodo: informarEquipamentoModalClick, tamanho: "10", icone: "", visibilidade: isPermitirInformarEquipamento };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: new Array(), tamanho: 10, };

    const configExportacao = {
        url: "FilaCarregamento/ExportarPesquisa",
        titulo: "Filas de Carregamento"
    };

    if (isExibirTodasFilasCarregamento()) {
        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAceitarCarga, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoAceitarCarga);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlocarCarga, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoAlocarCarga);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarCentroCarregamento, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoAlterarCentroCarregamento);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirConfirmarChegada, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoConfirmarChegada);

        if (_CONFIGURACAO_TMS.UtilizarFilaCarregamentoReversa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirDesatrelarTracao, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoDesatrelarTracao);

        menuOpcoes.opcoes.push(opcaoDetalhes);
        menuOpcoes.opcoes.push(opcaoDetalhesCarga);
        menuOpcoes.opcoes.push(opcaoDetalhesMotorista);
        menuOpcoes.opcoes.push(opcaoDetalhesPreCarga);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirEnviarNotificacao, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoEnviarNotificacao);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoConjuntoMotorista);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoVeiculo, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoConjuntoVeiculo);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberar, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoLiberar);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberarSaidaFila, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoLiberarSaidaFila);

        menuOpcoes.opcoes.push(opcaoPosicaoAtualVeiculo);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarPrimeiraPosicao, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoPrimeiraPosicao);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRecusarCarga, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoRecusarCarga);

        if (_usuarioPossuiPermissaoRemover)
            menuOpcoes.opcoes.push(opcaoRemover);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverConjuntoMotorista, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoRemoverConjuntoMotorista);

        if (!_CONFIGURACAO_TMS.UtilizarFilaCarregamentoReversa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverReversa, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoRemoverReversa);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverTracao, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoRemoverTracao);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirReposicionar, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoReposicionar);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarUltimaPosicao, _PermissoesPersonalizadas))
            menuOpcoes.opcoes.push(opcaoUltimaPosicao);

        menuOpcoes.opcoes.push(opcaoEnviarNotificacaoMotorista);
        menuOpcoes.opcoes.push(opcaoEnviarNotificacaoSuperAppMotorista);
        menuOpcoes.opcoes.push(opcaoInformarEquipamento);
    }
    else {
        if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros) && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAceitarCarga, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga))
            menuOpcoes.opcoes.push(opcaoAceitarCarga);

        if (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
            menuOpcoes.opcoes.push(opcaoAceitarPreCarga);

        menuOpcoes.opcoes.push(opcaoDetalhes);
        menuOpcoes.opcoes.push(opcaoDetalhesCarga);
        menuOpcoes.opcoes.push(opcaoDetalhesMotorista);
        menuOpcoes.opcoes.push(opcaoDetalhesPreCarga);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros) {
            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoConjuntoMotorista);

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoVeiculo, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoConjuntoVeiculo);

            if (!_CONFIGURACAO_TMS.UtilizarProgramacaoCarga && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberarSaidaFila, _PermissoesPersonalizadas))
                menuOpcoes.opcoes.push(opcaoLiberarSaidaFila);

            menuOpcoes.opcoes.push(opcaoPosicaoAtualVeiculo);

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRecusarCarga, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoRecusarCarga);

            if (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoRecusarPreCarga);

            if (_usuarioPossuiPermissaoRemover)
                menuOpcoes.opcoes.push(opcaoRemover);

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverConjuntoMotorista, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoRemoverConjuntoMotorista);

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverTracao, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.UtilizarProgramacaoCarga)
                menuOpcoes.opcoes.push(opcaoRemoverTracao);
        }
        else if (_usuarioPossuiPermissaoRemover)
            menuOpcoes.opcoes.push(opcaoRemover);
    }

    _gridFilaCarregamento = new GridView("grid-fila-carregamento", "FilaCarregamento/Pesquisa", _pesquisaFilaCarregamentoAuxiliar, menuOpcoes, ordenacaoPadrao, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, configExportacao, undefined, undefined, callbackRowFilaCarregamento, callbackColumnDefaultFilaCarregamento);
    _gridFilaCarregamento.SetPermitirEdicaoColunas(true);
    _gridFilaCarregamento.SetSalvarPreferenciasGrid(true);
}

function loadGridFilaEmTransicao() {
    const limiteRegistros = 10;
    const totalRegistrosPorPagina = 5;
    const opcaoRemover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFilaCarregamentoEmTransicaoClick, tamanho: "10", icone: "" };
    const opcoes = new Array();
    let menuOpcoes = null;

    if (_usuarioPossuiPermissaoRemover)
        opcoes.push(opcaoRemover);

    if (opcoes.length > 0)
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoRemover], tamanho: 10, };

    _gridFilaEmTransicao = new GridView("grid-fila-em-transicao", "FilaCarregamento/PesquisaEmTransicao", _pesquisaFilaCarregamentoAuxiliar, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowFilaCarregamentoEmTransicao);
}

function loadGridFilaMotorista() {
    const limiteRegistros = 10;
    const totalRegistrosPorPagina = 5;
    const opcaoRemover = { descricao: "Remover Motorista", id: guid(), evento: "onclick", metodo: removerFilaCarregamentoMotoristaClick, tamanho: "10", icone: "" };
    const opcoes = new Array();
    let menuOpcoes = null;

    if (_usuarioPossuiPermissaoRemover)
        opcoes.push(opcaoRemover);

    if (opcoes.length > 0)
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoRemover], tamanho: 10, };

    _gridFilaMotorista = new GridView("grid-fila-motorista", "FilaCarregamento/PesquisaMotorista", _pesquisaFilaCarregamentoAuxiliar, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowFilaCarregamentoMotorista);
}

function loadListaTipoRetornoCarga(callback) {
    executarReST("TipoRetornoCarga/PesquisaRetornoCargaTipo", {}, function (retorno) {
        if (retorno.Success) {
            _listaTipoRetornoCarga = retorno.Data;

            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function unloadDroppableAlteracaoCentroCarregamento() {
    if (_usuarioPossuiPermissaoAlterarCentroCarregamento)
        $("#container-grid-fila-carregamento, #container-grid-fila-em-transicao").droppable("destroy");
}

function unloadDroppableVincularConjuntoMotorista() {
    if (_usuarioPossuiPermissaoInformarConjuntoMotorista)
        $("#grid-fila-carregamento tbody tr").droppable("destroy");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aceitarCargaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente aceitar a carga?", function () {
        executarReST("FilaCarregamento/AceitarCarga", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function aceitarPreCargaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente aceitar o pré planejamento?", function () {
        executarReST("FilaCarregamento/AceitarPreCarga", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function aceitarSolicitacaoSaidaFilaClick() {
    if (ValidarCamposObrigatorios(_filaLiberacaoSaida)) {
        executarReST("FilaCarregamento/AceitarSolicitacaoSaida", RetornarObjetoPesquisa(_filaLiberacaoSaida), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Saída da fila aceita com sucesso");

                    fecharLiberacaoSaidaModal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarFilaEmTransicaoClick() {
    if (ValidarCamposObrigatorios(_filaEmTransicaoCadastro)) {
        const filaCadastrar = {
            Motorista: _filaEmTransicaoCadastro.Motorista.codEntity(),
            Tipo: _filaEmTransicaoCadastro.Tipo.val(),
            Veiculo: _filaEmTransicaoCadastro.Veiculo.codEntity()
        };

        executarReST("FilaCarregamento/AdicionarFilaEmTransicao", filaCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado registro a fila Em Transição com sucesso");

                    fecharFilaEmTransicaoModal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarFilaEmTransicaoModalClick() {
    if (_usuarioPossuiPermissaoAdicionar && (_pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity() > 0)) {
        Global.abrirModal('divModalCadastroFilaEmTransicao');
        $("#divModalCadastroFilaEmTransicao").one('hidden.bs.modal', function () {
            LimparCampos(_filaEmTransicaoCadastro);
        });
    }
}

function adicionarFilaMotoristaClick() {
    if (ValidarCamposObrigatorios(_filaMotoristaCadastro)) {
        const filaMotoristaCadastrar = {
            Motorista: _filaMotoristaCadastro.Motorista.codEntity(),
            Veiculo: _filaMotoristaCadastro.Veiculo.codEntity(),
            CentroCarregamento: _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity()
        };

        executarReST("FilaCarregamento/AdicionarFilaMotorista", filaMotoristaCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado registro com sucesso");

                    fecharFilaMotoristaModal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarFilaMotoristaModalClick() {
    if (_usuarioPossuiPermissaoAdicionar && (_pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity() > 0)) {
        Global.abrirModal('divModalCadastroFilaMotorista');
        $("#divModalCadastroFilaMotorista").one('hidden.bs.modal', function () {
            LimparCampos(_filaMotoristaCadastro);
        });
    }
}

function alocarCargaClick() {
    if (ValidarCamposObrigatorios(_filaAlocarCarga)) {
        executarReST("FilaCarregamento/AlocarCarga", RetornarObjetoPesquisa(_filaAlocarCarga), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    fecharAlocarCargaModal();
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function alocarCargaModalClick(filaSelecionada) {
    _filaAlocarCarga.Codigo.val(filaSelecionada.Codigo);

    _pesquisaFilaCarregamentoCargaAlocar.Filial.codEntity(_pesquisaFilaCarregamentoAuxiliar.Filial.codEntity());
    _pesquisaFilaCarregamentoCargaAlocar.Filial.val(_pesquisaFilaCarregamentoAuxiliar.Filial.val());
    _pesquisaFilaCarregamentoCargaAlocar.ModeloVeicular.codEntity(filaSelecionada.CodigoModeloVeicularCarga);
    _pesquisaFilaCarregamentoCargaAlocar.ModeloVeicular.val(filaSelecionada.ModeloVeicularCarga);

    Global.abrirModal('divModalAlocarCarga');
    $("#divModalAlocarCarga").one('hidden.bs.modal', function () {
        LimparCampos(_filaAlocarCarga);
    });
}

function alterarCentroCarregamentoClick() {
    if (ValidarCamposObrigatorios(_filaAlterarCentroCarregamento)) {
        executarReST("FilaCarregamento/AlterarCentroCarregamento", RetornarObjetoPesquisa(_filaAlterarCentroCarregamento), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    fecharAlterarCentroCarregamentoModal();
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function alterarCentroCarregamentoModalClick(filaSelecionada) {
    _filaAlterarCentroCarregamento.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalAlterarCentroCarregamento');
    $("#divModalAlterarCentroCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_filaAlterarCentroCarregamento);
    });
}

function alterarPrimeiraPosicaoFilaCarregamentoClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente alterar para a primeira posição da fila de carregamento?", function () {
        executarReST("FilaCarregamento/AlterarPrimeiraPosicaoFila", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function alterarUltimaPosicaoFilaCarregamentoClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente alterar para a última posição da fila de carregamento?", function () {
        executarReST("FilaCarregamento/AlterarUltimaPosicaoFila", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function confirmarChegadaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente confirmar a chegada de veículo?", function () {
        executarReST("FilaCarregamento/ConfirmarChegadaVeiculo", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Chegada de veículo confirmada com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function conjuntoMotoristaClick() {
    executarReST("FilaCarregamento/InformarConjuntoMotorista", RetornarObjetoPesquisa(_conjuntoMotorista), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Informado o conjunto do motorista com sucesso");

                fecharConjuntoMotoristaModal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function conjuntoMotoristaModalClick(filaSelecionada) {
    _conjuntoMotorista.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalConjuntoMotorista');
    $("#divModalConjuntoMotorista").one('hidden.bs.modal', function () {
        LimparCampos(_conjuntoMotorista);
    });
}

function informarEquipamentoModalClick(filaSelecionada) {
    _informarEquipamento.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalInformarEquipamento');
    $("#divModalInformarEquipamento").one('hidden.bs.modal', function () {
        LimparCampos(_informarEquipamento);
    });
}

function conjuntoVeiculoClick() {
    executarReST("FilaCarregamento/InformarConjuntoVeiculo", RetornarObjetoPesquisa(_conjuntoVeiculo), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Informado o conjunto do veículo com sucesso");

                fecharConjuntoVeiculoModal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function informarEquipamentoClick() {
    executarReST("FilaCarregamento/InformarEquipamento", RetornarObjetoPesquisa(_informarEquipamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Equipamento informado com sucesso");

                recarregarGrids();
                fecharInformarEquipamentoModal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function conjuntoVeiculoModalClick(filaSelecionada) {
    _conjuntoVeiculo.Codigo.val(filaSelecionada.Codigo);

    var codigosTransportadores = filaSelecionada.CodigosTransportador.includes(',') ? filaSelecionada.CodigosTransportador.split(',') : [filaSelecionada.CodigosTransportador];
    _conjuntoVeiculo.Transportador.multiplesEntities(codigosTransportadores.map(codigoTransportador => ({ Codigo: codigoTransportador, Descricao: '' })));

    Global.abrirModal('divModalConjuntoVeiculo');
    $("#divModalConjuntoVeiculo").one('hidden.bs.modal', function () {
        LimparCampos(_conjuntoVeiculo);
    });
}

function desatrelarTracaoClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente desatrelar a tração do veículo?", function () {
        executarReST("FilaCarregamento/DesatrelarTracao", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Tração desatrelada com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function detalhesClick(filaSelecionada) {
    ExibirDetalhes(filaSelecionada);
}

function detalhesMotoristaClick(filaSelecionada) {
    ExibirDetalhesMotorista(filaSelecionada);
}

function enviarNotificacaoClick(filaSelecionada) {
    ExibirModalFilaCarregamentoEnvioNotificacao(filaSelecionada);
}

function exibirPosicaoAtualVeiculoClick(filaSelecionada) {
    exibirPosicaoAtualVeiculoPorVeiculo(filaSelecionada.CodigoTracao);
}
function enviarNotificacaoSMSClick(filaSelecionada) {
    ExibirModalFilaCarregamentoEnvioNotificacaoSMS(filaSelecionada);
}
function fcEnviarNotificacaoSuperAppClick(filaSelecionada) {
    let motoristas = [{
        Codigo: filaSelecionada.Codigo,
        Carga: filaSelecionada.CodigoCarga,
        CargaEmbarcador: filaSelecionada.CargaEmbarcador,
        Tracao: filaSelecionada.Tracao,
        CPFMotoristas: filaSelecionada.CPFMotoristas,
        Motorista: filaSelecionada.Motorista,
        Transportador: filaSelecionada.Transportador
    }];
    exibirModalEnviarNotificacaoApp(motoristas);
}

function liberarClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente liberar a fila de carregamento?", function () {
        executarReST("FilaCarregamento/Liberar", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Liberado com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function liberarSaidaFilaModalClick(filaSelecionada) {
    executarReST("FilaCarregamento/ObterDetalhesSolicitacaoSaida", { Codigo: filaSelecionada.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_filaLiberacaoSaida, retorno);

                Global.abrirModal('divModalLiberacaoSaida');
                $("#divModalLiberacaoSaida").one('hidden.bs.modal', function () {
                    LimparCampos(_filaLiberacaoSaida);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function filasPosicaoFixaOnScroll() {
    const topAdicionarScroll = $("#container-fila-conteudo-fixar").offset().top - 14;

    if ((document.body.scrollTop > topAdicionarScroll) || (document.documentElement.scrollTop > topAdicionarScroll))
        $("#fila-conteudo-fixar").addClass("fila-conteudo-fixo");
    else
        $("#fila-conteudo-fixar").removeClass("fila-conteudo-fixo");
}

function rejeitarSolicitacaoSaidaFilaClick() {
    if (ValidarCamposObrigatorios(_filaLiberacaoSaida)) {
        executarReST("FilaCarregamento/RecusarSolicitacaoSaida", RetornarObjetoPesquisa(_filaLiberacaoSaida), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Saída da fila rejeitada com sucesso");

                    fecharLiberacaoSaidaModal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function recusarCargaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente recusar a carga?", function () {
        executarReST("FilaCarregamento/RecusarCarga", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function recusarPreCargaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente recusar o pré planejamento?", function () {
        executarReST("FilaCarregamento/RecusarPreCarga", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (!retorno.Data)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function removerConjuntoMotoristaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o motorista?", function () {
        executarReST("FilaCarregamento/RemoverConjuntoMotorista", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Motorista removido com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function removerFilaCarregamentoClick(filaSelecionada) {
    _filaCarregamentoSituacao.remover(filaSelecionada.Codigo);
}

function removerFilaCarregamentoEmTransicaoClick(filaSelecionada) {
    _filaCarregamentoSituacao.remover(filaSelecionada.Codigo);
}

function removerFilaCarregamentoMotoristaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o motorista?", function () {
        executarReST("FilaCarregamento/RemoverFilaCarregamentoMotorista", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Motorista removido com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function removerReversaClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover a reversa?", function () {
        executarReST("FilaCarregamento/RemoverReversa", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reversa removida com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function removerTracaoClick(filaSelecionada) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover a tração?", function () {
        executarReST("FilaCarregamento/RemoverTracao", { Codigo: filaSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Tração removida com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function reposicionarFilaCarregamentoClick() {
    if (ValidarCamposObrigatorios(_filaReposicionar)) {
        executarReST("FilaCarregamento/Reposicionar", RetornarObjetoPesquisa(_filaReposicionar), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    fecharReposicionarModal();
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function reposicionarFilaCarregamentoModalClick(filaSelecionada) {
    _filaReposicionar.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalReposicionar');
    $("#divModalReposicionar").one('hidden.bs.modal', function () {
        LimparCampos(_filaReposicionar);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity(_pesquisaFilaCarregamento.CentroCarregamento.codEntity());
    _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.val(_pesquisaFilaCarregamento.CentroCarregamento.val());
    _pesquisaFilaCarregamentoAuxiliar.DataProgramadaInicial.val(_pesquisaFilaCarregamento.DataProgramadaInicial.val());
    _pesquisaFilaCarregamentoAuxiliar.DataProgramadaFinal.val(_pesquisaFilaCarregamento.DataProgramadaFinal.val());
    _pesquisaFilaCarregamentoAuxiliar.Filial.codEntity(_pesquisaFilaCarregamento.Filial.codEntity());
    _pesquisaFilaCarregamentoAuxiliar.Filial.val(_pesquisaFilaCarregamento.Filial.val());
    _pesquisaFilaCarregamentoAuxiliar.GrupoModeloVeicular.codEntity(_pesquisaFilaCarregamento.GrupoModeloVeicular.codEntity());
    _pesquisaFilaCarregamentoAuxiliar.GrupoModeloVeicular.val(_pesquisaFilaCarregamento.GrupoModeloVeicular.val());
    _pesquisaFilaCarregamentoAuxiliar.Carga.multiplesEntities(_pesquisaFilaCarregamento.Carga.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.ConfiguracaoProgramacaoCarga.multiplesEntities(_pesquisaFilaCarregamento.ConfiguracaoProgramacaoCarga.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.Destino.multiplesEntities(_pesquisaFilaCarregamento.Destino.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.EstadoDestino.multiplesEntities(_pesquisaFilaCarregamento.EstadoDestino.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.ModeloVeicular.multiplesEntities(_pesquisaFilaCarregamento.ModeloVeicular.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.RegiaoDestino.multiplesEntities(_pesquisaFilaCarregamento.RegiaoDestino.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.Situacao.val(_pesquisaFilaCarregamento.Situacao.val());
    _pesquisaFilaCarregamentoAuxiliar.TipoCarga.multiplesEntities(_pesquisaFilaCarregamento.TipoCarga.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.TipoOperacao.multiplesEntities(_pesquisaFilaCarregamento.TipoOperacao.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.Transportador.multiplesEntities(_pesquisaFilaCarregamento.Transportador.multiplesEntities());
    _pesquisaFilaCarregamentoAuxiliar.Veiculo.multiplesEntities(_pesquisaFilaCarregamento.Veiculo.multiplesEntities());
}

function callbackColumnDefaultFilaCarregamento(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "TempoFila") {
        if (dadosLinha.DataEntradaFila) {
            setTimeout(function () {
                $('#' + cabecalho.name + '-' + dadosLinha.DT_RowId)
                    .countdown(moment(dadosLinha.DataEntradaFila, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        if (event.offset.totalDays > 0)
                            $(this).text(event.strftime('%-Dd %H:%M:%S'));
                        else
                            $(this).text(event.strftime('%H:%M:%S'));
                    });
            }, 1000);
        }

        return '<span id="' + cabecalho.name + '-' + dadosLinha.DT_RowId + '"></span>';
    }
}

function callbackRowFilaCarregamento(nRow) {
    if (_usuarioPossuiPermissaoAlterarCentroCarregamento) {
        $("#grid-fila-carregamento").addClass("tableCursorMove");

        $(nRow).draggable(obterObjetoDragglable());
    }
}

function callbackRowFilaCarregamentoEmTransicao(nRow) {
    if (isExibirTodasFilasCarregamento() && _usuarioPossuiPermissaoAlterarCentroCarregamento) {
        $("#grid-fila-em-transicao").addClass("tableCursorMove");

        $(nRow).draggable(obterObjetoDragglable());
    }
}

function callbackRowFilaCarregamentoMotorista(nRow, aData) {
    if (isExibirTodasFilasCarregamento() && _usuarioPossuiPermissaoInformarConjuntoMotorista && (aData.Situacao == EnumSituacaoFilaCarregamentoMotorista.Disponivel)) {
        $(nRow).css("cursor", "move");

        $(nRow).draggable({
            cursor: "move",
            helper: function (event) {
                let html = '';

                $(event.currentTarget).children().each(function (i, coluna) {
                    html += '<td style="width: ' + ($(coluna).width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
                });

                const corLinha = $(event.currentTarget).css("background-color");
                const corLinhaSelecionada = "#ecf3f8";
                const coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
                const isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

                return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
            },
            revert: 'invalid',
            start: function () {
                unloadDroppableAlteracaoCentroCarregamento();
                loadDroppableVincularConjuntoMotorista();
            },
            stop: function () {
                loadDroppableAlteracaoCentroCarregamento();
                unloadDroppableVincularConjuntoMotorista();
            }
        });
    }
}

function exibirDetalhesCargaClick(filaSelecionada) {
    executarReST("Carga/BuscarCargaPorCodigo", { Carga: filaSelecionada.CodigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: -1px; top: -4px;"><i class="fal fa-times"></i></button>');
                const _cargaAtual = GerarTagHTMLDaCarga("fdsCarga", retorno.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");

                if (retorno.Data.DadosTransporte.TipoCarga.Codigo <= 0 || retorno.Data.DadosTransporte.ModeloVeicularCarga.Codigo <= 0)
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();
                else if (retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemValorFrete)
                    $("#" + _cargaAtual.EtapaFreteEmbarcador.idTab).click();
                else if (
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgAceiteTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento
                )
                    $("#" + _cargaAtual.EtapaDadosTransportador.idTab).click();
                else
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();

                Global.abrirModal('divModalDetalhesCarga');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirDetalhesPreCargaClick(filaSelecionada) {
    exibirPreCargaDadosParaTransporte(filaSelecionada.CodigoPreCarga);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function fecharAlocarCargaModal() {
    Global.fecharModal("divModalAlocarCarga");
}

function fecharAlterarCentroCarregamentoModal() {
    Global.fecharModal("divModalAlterarCentroCarregamento");
}

function fecharConjuntoMotoristaModal() {
    Global.fecharModal("divModalConjuntoMotorista");
}

function fecharConjuntoVeiculoModal() {
    Global.fecharModal("divModalConjuntoVeiculo");
}

function fecharFilaEmTransicaoModal() {
    Global.fecharModal("divModalCadastroFilaEmTransicao");
}

function fecharFilaMotoristaModal() {
    Global.fecharModal("divModalCadastroFilaMotorista");
}

function fecharLiberacaoSaidaModal() {
    Global.fecharModal("divModalLiberacaoSaida");
}

function fecharReposicionarModal() {
    Global.fecharModal("divModalReposicionar");
}
function fecharInformarEquipamentoModal() {
    Global.fecharModal("divModalInformarEquipamento");
}

function itemSoltado(event, ui) {
    const idContainerDestino = event.target.id;
    const idContainerOrigem = "container-" + $(ui.draggable[0]).parent().parent()[0].id;

    if (idContainerOrigem !== idContainerDestino)
        _filaCarregamentoSituacao.alterarSituacaoFila(ui.draggable[0].id, _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity(), idContainerDestino);
}

function isExibirTodasFilasCarregamento() {
    return (
        (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) &&
        (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros) &&
        (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.TransportadorTerceiro) &&
        !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga
    );
}

function isPermitirAceitarCarga(filaSelecionada) {
    return (filaSelecionada.Situacao == EnumSituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga) || (filaSelecionada.Situacao == EnumSituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao);
}

function isPermitirAceitarOuRecusarPreCarga(filaSelecionada) {
    return filaSelecionada.Situacao == EnumSituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga;
}

function isPermitirExibirPosicaoAtualVeiculo(filaSelecionada) {
    return filaSelecionada.CodigoTracao > 0;
}

function isPermitirInformarConjuntoMotorista(filaSelecionada) {
    return !filaSelecionada.ConjuntoMotoristaCompleto;
}

function isPermitirInformarEquipamento(filaSelecionada) {
    return filaSelecionada.InformarEquipamento;
}
function isPermitirEnviarNotificacaoSuperApp(filaSelecionada) {
    return filaSelecionada.PermiteEnviarNotificacaoSuperApp;
}

function isPermitirInformarConjuntoVeiculo(filaSelecionada) {
    return !filaSelecionada.ConjuntoVeiculoCompleto;
}

function isPermitirLiberarSaidaFila(filaSelecionada) {
    return filaSelecionada.EmRemocao;
}

function isPermitirRecusarCarga(filaSelecionada) {
    return (filaSelecionada.Situacao == EnumSituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga);
}

function isPermitirVisualizarDetalhesCarga(filaSelecionada) {
    return filaSelecionada.CodigoCarga > 0;
}

function isPermitirVisualizarDetalhesPreCarga(filaSelecionada) {
    return filaSelecionada.CodigoPreCarga > 0;
}

function obterObjetoDragglable() {
    return {
        cursor: "move",
        helper: function (event) {
            let html = '';

            $(event.currentTarget).children().each(function (i, coluna) {
                const $coluna = $(coluna);

                html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
            });

            const corLinha = $(event.currentTarget).css("background-color");
            const corLinhaSelecionada = "#ecf3f8";
            const coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
            const isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

            return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
        },
        revert: 'invalid'
    };
}

function obterTipoPadrao() {
    return (_listaTipoRetornoCarga.length == 1) ? _listaTipoRetornoCarga[0].value : "";
}

function recarregarFilaCarregamento() {
    recarregarGraficoFilaCarregamentoResumo();
    recarregarTotalizadoresPorSituacaoFilaCarregamento();

    if (isExibirTodasFilasCarregamento())
        filasPosicaoFixaOnScroll();

    recarregarGrids();
}

function recarregarGrids() {
    _gridFilaCarregamento.CarregarGrid();

    if (isExibirTodasFilasCarregamento()) {
        _gridFilaEmTransicao.CarregarGrid();
        _gridFilaMotorista.CarregarGrid();
    }
}

function recarregarTotalizadoresPorSituacaoFilaCarregamento() {
    executarReST("FilaCarregamento/ObterTotalizadoresPorSituacao", RetornarObjetoPesquisa(_pesquisaFilaCarregamentoAuxiliar), function (retorno) {
        if (retorno.Success)
            PreencherObjetoKnout(_filaCarregamentoResumoPorSituacao, retorno);
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            LimparCampos(_filaCarregamentoResumoPorSituacao);
        }
    });
}

function retornoConsultaCentroCarregamento(centroCarregamentoSelecionado) {
    _pesquisaFilaCarregamento.CentroCarregamento.codEntity(centroCarregamentoSelecionado.Codigo);
    _pesquisaFilaCarregamento.CentroCarregamento.entityDescription(centroCarregamentoSelecionado.Descricao);
    _pesquisaFilaCarregamento.CentroCarregamento.val(centroCarregamentoSelecionado.Descricao);
    _pesquisaFilaCarregamento.Filial.codEntity(centroCarregamentoSelecionado.CodigoFilial);
    _pesquisaFilaCarregamento.Filial.entityDescription(centroCarregamentoSelecionado.Filial);
    _pesquisaFilaCarregamento.Filial.val(centroCarregamentoSelecionado.Filial);
}

function vincularFilaCarregamentoMotorista(event, ui) {
    const codigoFilaCarregamentoVeiculo = event.target.id;
    const codigoFilaCarregamentoMotorista = ui.draggable[0].id;

    exibirConfirmacao("Atenção!", "Deseja realmente vincular o motorista a fila de carregamento?", function () {
        executarReST("FilaCarregamento/VincularFilaCarregamentoMotorista", { FilaCarregamentoVeiculo: codigoFilaCarregamentoVeiculo, FilaCarregamentoMotorista: codigoFilaCarregamentoMotorista }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Motorista vinculado com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}
