/// <reference path="../../Veiculos/Veiculo/Motorista.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLiberacaoAbastecimentoAutomatizado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDLiberacaoAbastecimentoAutomatizado;
var _liberacaoAbastecimentoAutomatizado;
var _pesquisaLiberacaoAbastecimentoAutomatizado;
var _gridLiberacaoAbastecimentoAutomatizado;
var _pesquisaHistoricoIntegracao;
var _tempoParaConsulta = 4000;
var _timeOutPainel;
var _pesquisouNovamente = false;
var _habilitarPainel = false;
var _habilitarPesquisaAutomatica = false;
let _interval;
/*
 * Declaração das Classes
 */

var CRUDLiberacaoAbastecimentoAutomatizado = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.NovoRegistro = PropertyEntity({ eventClick: NovoRegistroClick, type: types.event, text: Localization.Resources.Gerais.Geral.NovoRegistro, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.ProcessandoAbastecimento = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.AguardandoProcessamentoAbastecimento, visible: ko.observable(false) });
    this.TimerProcessandoAbastecimento = PropertyEntity({ visible: ko.observable(false) });

};

var LiberacaoAbastecimentoAutomatizado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataAbastecimento = PropertyEntity({ text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.DataAbastecimento.getRequiredFieldDescription()), getType: typesKnockout.date, enable: ko.observable(false), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.Veiculo.getRequiredFieldDescription()), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.Motorista.getRequiredFieldDescription()), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.BombaAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.BombaAbastecimento.getRequiredFieldDescription()), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.UltimaQuilometragem = PropertyEntity({ getType: typesKnockout.int, text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.UltimaQuilometragem.getRequiredFieldDescription()), required: false, configInt: { precision: 0, allowZero: true }, visible: ko.observable(true), enable: ko.observable(false) });
    this.QuilometragemAtual = PropertyEntity({ getType: typesKnockout.int, text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.QuilometragemAtual.getRequiredFieldDescription()), required: true, configInt: { precision: 0, allowZero: true }, visible: ko.observable(true), enable: ko.observable(true) });

    this.QuilometrosRodados = PropertyEntity({
        getType: typesKnockout.int,
        text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.QuilometrosRodados.getRequiredFieldDescription()),
        required: false, configInt: { precision: 0, allowZero: true },
        visible: ko.observable(true),
        enable: ko.observable(false),
        val: ko.computed({
            read: CalcularQuilometrosRodados,
            write: CalcularQuilometrosRodados,
            owner: this
        }) });
    this.QuantidadeAbastecida = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), required: true, text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.QuantidadeAbastecida.getRequiredFieldDescription()), configDecimal: { precision: 4, allowZero: false }, maxlength: 10, enable: ko.observable(false), visible: ko.observable(false) });
    this.CodigoAutorizacao = PropertyEntity({ text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.CodigoDeAutorizacao.getRequiredFieldDescription()), required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false), visible: ko.observable(false) });
    

};

var PesquisaLiberacaoAbastecimentoAutomatizado = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.Veiculo), idBtnSearch: guid(), required: false });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.Motorista), idBtnSearch: guid(), required: false });
    this.BombaAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.BombaAbastecimento), idBtnSearch: guid(), required: false });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLiberacaoAbastecimentoAutomatizado.AgRetornoAbastecimento), def: "", options: EnumSituacaoLiberacaoAbastecimentoAutomatizado.ObterOpcoesPesquisa(), text: ko.observable(Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.Situacao) });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridLiberacaoAbastecimentoAutomatizado, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};
var PesquisaHistoricoIntegracaoAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};
/*
 * Declaração das Funções de Inicialização
 */

function LoadGridLiberacaoAbastecimentoAutomatizado() {

    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesAbastecimentoClick, icone: "" };
    var opcaoVisualizar = { descricao: Localization.Resources.Gerais.Geral.Visualizar, id: guid(), evento: "onclick", metodo: VisualizarClick, tamanho: "10", icone: "" };
    var opcaoReenviarIntegracao = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), evento: "onclick", metodo: ReenviarIntegracao, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoVisualizar, historico] }; 

    _gridLiberacaoAbastecimentoAutomatizado = new GridViewExportacao(_pesquisaLiberacaoAbastecimentoAutomatizado.Pesquisar.idGrid, "LiberacaoAbastecimentoAutomatizado/Pesquisa", _pesquisaLiberacaoAbastecimentoAutomatizado, menuOpcoes);
    _gridLiberacaoAbastecimentoAutomatizado.CarregarGrid();
}

function LoadLiberacaoAbastecimentoAutomatizado() {
    _liberacaoAbastecimentoAutomatizado = new LiberacaoAbastecimentoAutomatizado();
    KoBindings(_liberacaoAbastecimentoAutomatizado, "knockoutLiberacaoAbastecimentoAutomatizado");

    _CRUDLiberacaoAbastecimentoAutomatizado = new CRUDLiberacaoAbastecimentoAutomatizado();
    KoBindings(_CRUDLiberacaoAbastecimentoAutomatizado, "knockoutCRUDLiberacaoAbastecimentoAutomatizado");

    _pesquisaLiberacaoAbastecimentoAutomatizado = new PesquisaLiberacaoAbastecimentoAutomatizado();
    KoBindings(_pesquisaLiberacaoAbastecimentoAutomatizado, "knockoutPesquisaLiberacaoAbastecimentoAutomatizado", false, _pesquisaLiberacaoAbastecimentoAutomatizado.Pesquisar.id);

    BuscarVeiculos(_liberacaoAbastecimentoAutomatizado.Veiculo, RetornoConsultaVeiculo);
    BuscarVeiculos(_pesquisaLiberacaoAbastecimentoAutomatizado.Veiculo);

    BuscarMotorista(_liberacaoAbastecimentoAutomatizado.Motorista);
    BuscarMotorista(_pesquisaLiberacaoAbastecimentoAutomatizado.Motorista);

    BuscarBombaAbastecimento(_liberacaoAbastecimentoAutomatizado.BombaAbastecimento);
    BuscarBombaAbastecimento(_pesquisaLiberacaoAbastecimentoAutomatizado.BombaAbastecimento);

    LoadGridLiberacaoAbastecimentoAutomatizado();

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_liberacaoAbastecimentoAutomatizado, "LiberacaoAbastecimentoAutomatizado/Adicionar", function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Msg);

                //LimparCamposLiberacaoAbastecimentoAutomatizado();
                LoadGridLiberacaoAbastecimentoAutomatizado();
                ControlarBotoesHabilitados(true);

                _habilitarPesquisaAutomatica = true;
                executarPesquisaTimeOutAbastecimento();

                _liberacaoAbastecimentoAutomatizado.Codigo.val(retorno.Data.Codigo);

                _CRUDLiberacaoAbastecimentoAutomatizado.NovoRegistro.visible(false);

                consultarBuscarPorCodigo();
                startTimer();
            }
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                _CRUDLiberacaoAbastecimentoAutomatizado.TimerProcessandoAbastecimento.visible(false);

                NovoRegistroClick();
                LoadGridLiberacaoAbastecimentoAutomatizado();
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                _habilitarPesquisaAutomatica = false;
            }
        }
        else {
            LimparCamposLiberacaoAbastecimentoAutomatizado();
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            _habilitarPesquisaAutomatica = false;
        }
    }, sender);
}

function NovoRegistroClick() {
    _liberacaoAbastecimentoAutomatizado.DataAbastecimento.visible(false);
    _liberacaoAbastecimentoAutomatizado.QuantidadeAbastecida.visible(false);
    _liberacaoAbastecimentoAutomatizado.CodigoAutorizacao.visible(false);
    ControlarBotoesHabilitados(true);
    HabilitarEdicaoDosCampos();

    var nomeBombaAbastecimento = _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.val();
    var codigoBombaAbastecimento = _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.codEntity();

    LimparCampos(_liberacaoAbastecimentoAutomatizado);

    _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.val(nomeBombaAbastecimento);
    _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.codEntity(codigoBombaAbastecimento);
}

function CancelarClick() {
    LimparCampos(_liberacaoAbastecimentoAutomatizado);
    _CRUDLiberacaoAbastecimentoAutomatizado.Cancelar.visible(false);
}

function VisualizarClick(registroSelecionado) {
    ControlarProcessamentoAbastecimento(false);
    LimparCamposLiberacaoAbastecimentoAutomatizado();

    _liberacaoAbastecimentoAutomatizado.Codigo.val(registroSelecionado.Codigo);
    _liberacaoAbastecimentoAutomatizado.DataAbastecimento.visible(true);
    _liberacaoAbastecimentoAutomatizado.QuantidadeAbastecida.visible(true);
    _liberacaoAbastecimentoAutomatizado.CodigoAutorizacao.visible(true);

    BuscarPorCodigo(_liberacaoAbastecimentoAutomatizado, "LiberacaoAbastecimentoAutomatizado/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaLiberacaoAbastecimentoAutomatizado.ExibirFiltros.visibleFade(false);

                var isEdicao = false;

                DesabilitarEdicaoDosCampos();
                ControlarBotoesHabilitados(isEdicao);

                if (retorno.Data.SituacaoAbastecimento !== EnumSituacaoLiberacaoAbastecimentoAutomatizado.Autorizado && retorno.Data.SituacaoIntegracao !== EnumSituacaoIntegracao.ProblemaIntegracao && retorno.Data.SituacaoAbastecimento !== EnumSituacaoLiberacaoAbastecimentoAutomatizado.AgRetornoAbastecimento && retorno.Data.SituacaoAbastecimento !== EnumSituacaoLiberacaoAbastecimentoAutomatizado.Finalizado) 
                {
                    _habilitarPainel = true;
                    executarPesquisaTimeOutAbastecimento();

                    _CRUDLiberacaoAbastecimentoAutomatizado.NovoRegistro.visible(false);

                    consultarBuscarPorCodigo();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_liberacaoAbastecimentoAutomatizado, "LiberacaoAbastecimentoAutomatizado/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridLiberacaoAbastecimentoAutomatizado();
                    LimparCamposLiberacaoAbastecimentoAutomatizado();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */
function ControlarProcessamentoAbastecimento(visible) {

    ControlarBotoesHabilitados(!visible);
    _CRUDLiberacaoAbastecimentoAutomatizado.ProcessandoAbastecimento.visible(visible);
    _habilitarPesquisaAutomatica = visible;
}

function consultarBuscarPorCodigo(callback) {
    if (_liberacaoAbastecimentoAutomatizado.Codigo.val() == 0) {
        _habilitarPesquisaAutomatica = false;
        NovoRegistroClick();
        LoadGridLiberacaoAbastecimentoAutomatizado();
        return;
    }
        
    executarReST("LiberacaoAbastecimentoAutomatizado/BuscarPorCodigo", { Codigo: _liberacaoAbastecimentoAutomatizado.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaLiberacaoAbastecimentoAutomatizado.ExibirFiltros.visibleFade(false);

                if (retorno.Data.SituacaoAbastecimento === EnumSituacaoLiberacaoAbastecimentoAutomatizado.AgRetornoAbastecimento || retorno.Data.SituacaoAbastecimento === EnumSituacaoLiberacaoAbastecimentoAutomatizado.Autorizado || retorno.Data.SituacaoIntegracao === EnumSituacaoIntegracao.ProblemaIntegracao) {
                    ControlarProcessamentoAbastecimento(false);
                    _habilitarPesquisaAutomatica = false;
                    NovoRegistroClick();
                    LoadGridLiberacaoAbastecimentoAutomatizado();

                    abortTimer();
                    if (retorno.Data.SituacaoAbastecimento === EnumSituacaoLiberacaoAbastecimentoAutomatizado.AgRetornoAbastecimento || retorno.Data.SituacaoAbastecimento === EnumSituacaoLiberacaoAbastecimentoAutomatizado.Autorizado)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.AbastecimentoAutorizadoComSucesso);
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.OcorreuUmaFalhaAoEfetuarAbastecimento);
                }
                else {
                    ControlarProcessamentoAbastecimento(true);
                    _habilitarPesquisaAutomatica = true;
                    _CRUDLiberacaoAbastecimentoAutomatizado.NovoRegistro.visible(false);

                    executarPesquisaTimeOutAbastecimento();

                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}


function executarPesquisaTimeOutAbastecimento() {
    if (_habilitarPesquisaAutomatica) {
        _timeOutPainel = setTimeout(function () {
            consultarBuscarPorCodigo(executarPesquisaTimeOutAbastecimento);
        }, _tempoParaConsulta);
    }   
}

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDLiberacaoAbastecimentoAutomatizado.NovoRegistro.visible(!isEdicao);
    _CRUDLiberacaoAbastecimentoAutomatizado.Adicionar.visible(isEdicao);
}

function DesabilitarEdicaoDosCampos() {
    _liberacaoAbastecimentoAutomatizado.Veiculo.enable(false);
    _liberacaoAbastecimentoAutomatizado.Motorista.enable(false);
    _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.enable(false);
    _liberacaoAbastecimentoAutomatizado.UltimaQuilometragem.enable(false);
    _liberacaoAbastecimentoAutomatizado.QuilometragemAtual.enable(false);
    _liberacaoAbastecimentoAutomatizado.QuilometrosRodados.enable(false);
    _liberacaoAbastecimentoAutomatizado.QuantidadeAbastecida.enable(false);
    _liberacaoAbastecimentoAutomatizado.CodigoAutorizacao.enable(false);
}

function HabilitarEdicaoDosCampos() {
    _liberacaoAbastecimentoAutomatizado.Veiculo.enable(true);
    _liberacaoAbastecimentoAutomatizado.Motorista.enable(true);
    _liberacaoAbastecimentoAutomatizado.BombaAbastecimento.enable(true);
    _liberacaoAbastecimentoAutomatizado.UltimaQuilometragem.enable(false);
    _liberacaoAbastecimentoAutomatizado.QuilometragemAtual.enable(true);
    _liberacaoAbastecimentoAutomatizado.QuilometrosRodados.enable(false);
}

function LimparCamposLiberacaoAbastecimentoAutomatizado() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_liberacaoAbastecimentoAutomatizado);
}

function RecarregarGridLiberacaoAbastecimentoAutomatizado() {
    _gridLiberacaoAbastecimentoAutomatizado.CarregarGrid();
}
function exibirHistoricoIntegracoesAbastecimentoClick(integracao){
    BuscarHistoricoIntegracaoAbastecimento(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}
function BuscarHistoricoIntegracaoAbastecimento(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracaoAbastecimento();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAbastecimentoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "LiberacaoAbastecimentoAutomatizadoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}
function DownloadArquivosHistoricoIntegracaoAbastecimentoClick(historicoConsulta) {
    executarDownload("LiberacaoAbastecimentoAutomatizadoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function ReenviarIntegracao(data) {
    executarReST("LiberacaoAbastecimentoAutomatizado/ReenviarIntegracao", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridLiberacaoAbastecimentoAutomatizado.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function RetornoConsultaVeiculo(dados) {
    _liberacaoAbastecimentoAutomatizado.Veiculo.val(dados.Descricao);
    _liberacaoAbastecimentoAutomatizado.Veiculo.codEntity(dados.Codigo);

    _liberacaoAbastecimentoAutomatizado.Motorista.val(dados.NomeMotorista);
    _liberacaoAbastecimentoAutomatizado.Motorista.codEntity(dados.CodigoMotorista);    

    _liberacaoAbastecimentoAutomatizado.UltimaQuilometragem.val(dados.KMAtual);
}

function CalcularQuilometrosRodados() {
    var ultimaQuilometragem = _ParseIntHelper(this.UltimaQuilometragem.val()) || 0;
    var quilometragemAtual = _ParseIntHelper(this.QuilometragemAtual.val()) || 0;
    var quilometrosRodados = (quilometragemAtual > 0) ? (quilometragemAtual - ultimaQuilometragem) : 0;

    return _FormatHelper(quilometrosRodados);
}

function _ParseIntHelper(val) {
    return Globalize.parseInt(val + "");
}

function _FormatHelper(val, format) {
    format = format || "n0";
    if (isNaN(val))
        val = 0;

    return Globalize.format(val, format);
}


function startTimer() {
    _CRUDLiberacaoAbastecimentoAutomatizado.TimerProcessandoAbastecimento.visible(true);
    let time = 120; // 2 minutos em segundos
    const timerElement = document.getElementById('timer');

    _interval = setInterval(() => {
        let minutes = Math.floor(time / 60);
        let seconds = time % 60;

        seconds = seconds < 10 ? '0' + seconds : seconds;
        timerElement.textContent = `${minutes}:${seconds}`;

        if (time === 0) {
            clearInterval(_interval);
            expirarAbastecimentoPorCodigo();
        }

        time--;
    }, 1000);
}

function abortTimer() {
    clearInterval(_interval);
    document.getElementById('timer').textContent = "02:00";
    _CRUDLiberacaoAbastecimentoAutomatizado.TimerProcessandoAbastecimento.visible(false);
}

function expirarAbastecimentoPorCodigo() {
    executarReST("LiberacaoAbastecimentoAutomatizado/ExpirarAbastecimentoPorCodigo", { Codigo: _liberacaoAbastecimentoAutomatizado.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _CRUDLiberacaoAbastecimentoAutomatizado.TimerProcessandoAbastecimento.visible(false);
                ControlarProcessamentoAbastecimento(false);
                _habilitarPesquisaAutomatica = false;
                NovoRegistroClick();
                LoadGridLiberacaoAbastecimentoAutomatizado();

                if (arg.Data.SituacaoAbastecimento === EnumSituacaoLiberacaoAbastecimentoAutomatizado.Autorizado)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.AbastecimentoAutorizadoComSucesso);
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado.OcorreuUmaFalhaAoEfetuarAbastecimento);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                ReinicarProcesso();

            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            ReinicarProcesso();
        }
    });
}

function ReinicarProcesso() {
    ControlarProcessamentoAbastecimento(false);
    _habilitarPesquisaAutomatica = false;
    NovoRegistroClick();
    LoadGridLiberacaoAbastecimentoAutomatizado();
}