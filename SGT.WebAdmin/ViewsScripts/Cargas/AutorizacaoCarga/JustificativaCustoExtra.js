/*
 * Declaração de Objetos Globais do Arquivo
 */

var _justificativaCustoExtra;
var _registroUnico = false;
var _multiplasCargas = false;
var _multiplasAprovacoes = false;

/*
 * Declaração das Classes
 */

var JustificativaCustoExtra = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.JustificativaCustoExtra = PropertyEntity({ text: "*Justificativa do Custo Extra:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.SetorResponsavel = PropertyEntity({ text: "*Setor Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 1000 });

    this.Cancelar = PropertyEntity({ eventClick: cancelarJustificativaCustoExtraClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: salvarJustificativaCustoExtraClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadJustificativaCustoExtra() {
    _justificativaCustoExtra = new JustificativaCustoExtra();
    KoBindings(_justificativaCustoExtra, "knockoutJustificativaCustoExtra");

    BuscarJustificativaCustoExtra(_justificativaCustoExtra.JustificativaCustoExtra);
    BuscarSetorFuncionario(_justificativaCustoExtra.SetorResponsavel);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarJustificativaCustoExtraClick() {
    _justificativaCustoExtra.Carga.val(_carga.Codigo.val());

    if (!ValidarCamposObrigatorios(_justificativaCustoExtra)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (_registroUnico || _multiplasAprovacoes) {
        Salvar(_justificativaCustoExtra, "AutorizacaoCarga/SalvarCustoExra", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Custo extra salvo com sucesso.");
                    if (_registroUnico)
                        AprovarAlteracaoFreteCarga({ Codigo: _justificativaCustoExtra.Codigo.val() });
                    else
                        aprovarMultiplasRegrasClick();

                    cancelarJustificativaCustoExtraClick();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }


    if (_multiplasCargas) {
        var dados = RetornarObjetoPesquisa(_pesquisaCarga);

        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());
        dados.JustificativaCustoExtra = _justificativaCustoExtra.JustificativaCustoExtra.codEntity();
        dados.SetorResponsavel = _justificativaCustoExtra.SetorResponsavel.codEntity();
        dados.Observacao = _justificativaCustoExtra.Observacao.val();

        executarReST("AutorizacaoCarga/SalvarCustoExraMultiplasCargas", dados, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Custo extra salvo com sucesso.");
                aprovarMultiplasCargas();

                cancelarJustificativaCustoExtraClick();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function cancelarJustificativaCustoExtraClick() {
    LimparCampos(_justificativaCustoExtra);
    Global.fecharModal("divModalJustificativaCustoExtra");
    _justificativaCustoExtra.Codigo.val(0);
    _registroUnico = false;
    _multiplasCargas = false;
    _multiplasAprovacoes = false;
}

function TestarJustificativaCustoExtraUnicoRegistro(registroSelecionado) {
    if (registroSelecionado.ExigirInformarJustificativaCustoExtraCadastrado) {
        Global.abrirModal('divModalJustificativaCustoExtra');
        _registroUnico = true;
        _justificativaCustoExtra.Codigo.val(registroSelecionado.Codigo);
    }
    else {
        AprovarAlteracaoFreteCarga(registroSelecionado);
    }
}

function TestarJustificativaCustoExtraMultiplasCargas() {
    var dados = RetornarObjetoPesquisa(_pesquisaCarga);

    dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoCarga/VerificarExigeCustoExtraMultiplasCargas", dados, function (retorno) {
        if (retorno.Success) {
            Global.abrirModal('divModalJustificativaCustoExtra');
            _multiplasCargas = true;
        }
        else
            aprovarMultiplasCargas();
    });
}

function TestarJustificativaCustoExtraMultiplosItens() {
    executarReST("AutorizacaoCarga/VerificarExigeCustoExtraMultiplosItens", { Codigo: _carga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            Global.abrirModal('divModalJustificativaCustoExtra');
            _multiplasAprovacoes = true;
        }
        else
            aprovarMultiplasRegrasClick();
    });
}