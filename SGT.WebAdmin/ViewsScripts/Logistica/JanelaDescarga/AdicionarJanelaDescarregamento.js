/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="JanelaDescarga.js" />

// #region Objetos Globais do Arquivo

var _adicionarJanelaDescarregamento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AdicionarJanelaDescarregamento = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga", idBtnSearch: guid(), required: true });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataDescarregamento = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Horário de Descarregamento", required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarJanelaDescarregamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAdicionarJanelaDescarregamento() {
    _adicionarJanelaDescarregamento = new AdicionarJanelaDescarregamento();
    KoBindings(_adicionarJanelaDescarregamento, "knockoutAdicionarJanelaDescarregamento");

    new BuscarCargas(_adicionarJanelaDescarregamento.Carga, null, null, null, null, null, null, null, null, null, null, true, _adicionarJanelaDescarregamento.CentroDescarregamento);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarJanelaDescarregamentoClick() {
    if (!ValidarCamposObrigatorios(_adicionarJanelaDescarregamento)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var handleAdicionarJanela = function () {
        var dados = {
            Carga: _adicionarJanelaDescarregamento.Carga.codEntity(),
            CentroDescarregamento: _adicionarJanelaDescarregamento.CentroDescarregamento.codEntity(),
            DataDescarregamento: _adicionarJanelaDescarregamento.DataDescarregamento.val()
        };

        executarReST("JanelaDescarga/AdicionarDescarregamento", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Janela de descarregamento adicionada com sucesso!");
                    fecharModalAdicionarJanelaDescarregamento();
                    _tabelaDescarregamento.Load();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    };

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.JanelaDescarga_SobreporRegras, _PermissoesPersonalizadasCarga))
        return handleAdicionarJanela();

    exibirConfirmacao("Adicionar Carga?", "Você possui permissão para sobrepor regras de agendamento. Realmente deseja adicionar esta carga?", function () {
        handleAdicionarJanela();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAdicionarJanelaDescarregamento() {
    _adicionarJanelaDescarregamento.CentroDescarregamento.codEntity(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _adicionarJanelaDescarregamento.CentroDescarregamento.val(_dadosPesquisaDescarregamento.CentroDescarregamento);

    Global.abrirModal('divModalAdicionarJanelaDescarregamento');
    $("#divModalAdicionarJanelaDescarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_adicionarJanelaDescarregamento);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function fecharModalAdicionarJanelaDescarregamento() {
    Global.fecharModal('divModalAdicionarJanelaDescarregamento');
}

// #endregion Funções Privadas
