/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/PeriodoDescarregamento.js" />
/// <reference path="JanelaDescarga.js" />

// #region Objetos Globais do Arquivo

var _adicionarJanelaDescarregamentoPeriodo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AdicionarJanelaDescarregamentoPeriodo = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga", idBtnSearch: guid(), required: true });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataDescarregamento = PropertyEntity({});
    this.PeriodoDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Horário de Descarregamento", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarJanelaDescarregamentoPeriodoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAdicionarJanelaDescarregamentoPeriodo() {
    _adicionarJanelaDescarregamentoPeriodo = new AdicionarJanelaDescarregamentoPeriodo();
    KoBindings(_adicionarJanelaDescarregamentoPeriodo, "knockoutAdicionarJanelaDescarregamentoPeriodo");

    new BuscarCargas(_adicionarJanelaDescarregamentoPeriodo.Carga, null, null, null, null, null, null, null, null, null, null, true, _adicionarJanelaDescarregamentoPeriodo.CentroDescarregamento);
    new BuscarPeriodoDescarregamento(_adicionarJanelaDescarregamentoPeriodo.PeriodoDescarregamento, retornoConsultaAdicionarJanelaDescarregamentoPeriodo, _adicionarJanelaDescarregamentoPeriodo.DataDescarregamento, _adicionarJanelaDescarregamentoPeriodo.CentroDescarregamento, true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarJanelaDescarregamentoPeriodoClick() {
    adicionarJanelaDescarregamentoPeriodo();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAdicionarJanelaDescarregamentoPeriodo() {
    _adicionarJanelaDescarregamentoPeriodo.CentroDescarregamento.codEntity(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _adicionarJanelaDescarregamentoPeriodo.CentroDescarregamento.val(_dadosPesquisaDescarregamento.CentroDescarregamento);

    Global.abrirModal('divModalAdicionarJanelaDescarregamentoPeriodo');
    $("#divModalAdicionarJanelaDescarregamentoPeriodo").one('hidden.bs.modal', function () {
        LimparCampos(_adicionarJanelaDescarregamentoPeriodo);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function adicionarJanelaDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido) {
    if (!ValidarCamposObrigatorios(_adicionarJanelaDescarregamentoPeriodo)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var dados = {
        Carga: _adicionarJanelaDescarregamentoPeriodo.Carga.codEntity(),
        CentroDescarregamento: _adicionarJanelaDescarregamentoPeriodo.CentroDescarregamento.codEntity(),
        DataDescarregamento: _adicionarJanelaDescarregamentoPeriodo.DataDescarregamento.val(),
        PeriodoDescarregamento: _adicionarJanelaDescarregamentoPeriodo.PeriodoDescarregamento.codEntity(),
        PermitirHorarioDescarregamentoComLimiteAtingido: Boolean(permitirHorarioDescarregamentoComLimiteAtingido),
        PermitirHorarioDescarregamentoInferiorAoAtual: Boolean(permitirHorarioDescarregamentoInferiorAoAtual)
    };

    executarReST("JanelaDescarga/AdicionarDescarregamentoPorPeriodo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.HorarioDescarregamentoInferiorAtual) {
                    exibirConfirmacao("Confirmação", retorno.Msg + " Deseja sobrepor essa regra?", function () {
                        permitirHorarioDescarregamentoInferiorAoAtual = true;
                        adicionarJanelaDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido);
                    });
                    return;
                }

                if (retorno.Data.HorarioLimiteDescarregamentoAtingido) {
                    exibirConfirmacao("Confirmação", retorno.Msg + " Deseja sobrepor essa regra?", function () {
                        permitirHorarioDescarregamentoComLimiteAtingido = true;
                        adicionarJanelaDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido);
                    });
                    return;
                }

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Janela de descarregamento adicionada com sucesso!");
                fecharModalAdicionarJanelaDescarregamentoPeriodo();
                _tabelaDescarregamento.Load();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function fecharModalAdicionarJanelaDescarregamentoPeriodo() {
    Global.fecharModal('divModalAdicionarJanelaDescarregamentoPeriodo');
}

function retornoConsultaAdicionarJanelaDescarregamentoPeriodo(registroSelecionado) {
    _adicionarJanelaDescarregamentoPeriodo.DataDescarregamento.val(registroSelecionado.DataDescarregamento)
    _adicionarJanelaDescarregamentoPeriodo.PeriodoDescarregamento.codEntity(registroSelecionado.Codigo)
    _adicionarJanelaDescarregamentoPeriodo.PeriodoDescarregamento.entityDescription(registroSelecionado.Descricao);
    _adicionarJanelaDescarregamentoPeriodo.PeriodoDescarregamento.val(registroSelecionado.Descricao);
}

// #endregion Funções Privadas
