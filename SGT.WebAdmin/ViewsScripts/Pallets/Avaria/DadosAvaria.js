/// <reference path="Anexo.js" />
/// <reference path="avaria.js" />
/// <reference path="DadosAvariaQuantidade.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoAvariaPallet.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosAvariaPallet;

/*
 * Declaração das Classes
 */

var DadosAvariaPallet = function () {
    var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: !isTMS, text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: !isTMS });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Motivo Avaria:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false });
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação: ", maxlength: 400, enable: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Setor:", idBtnSearch: guid(), enable: ko.observable(false), visible: !isTMS });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: isTMS, text: "*Empresa/Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: isTMS });

    this.Filial.codEntity.subscribe(controlarSetorDadosAvaria);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosAvariaPallet() {
    _dadosAvariaPallet = new DadosAvariaPallet();
    KoBindings(_dadosAvariaPallet, "knockoutDadosAvariaPallet");

    new BuscarFilial(_dadosAvariaPallet.Filial);
    new BuscarMotivoAvariaPallet(_dadosAvariaPallet.MotivoAvaria);
    new BuscarSetorFuncionario(_dadosAvariaPallet.Setor, null, _dadosAvariaPallet.Filial);
    new BuscarTransportadores(_dadosAvariaPallet.Transportador);
}

/*
 * Declaração das Funções
 */

function adicionarDadosAvaria() {
    exibirConfirmacao("Confirmação", "Realmente deseja adicionar os dados da avaria de pallets?", function () {
        if (ValidarCamposObrigatorios(_dadosAvariaPallet)) {
            if (validarSituacoesInformadas()) {
                var dadosAvaria = RetornarObjetoPesquisa(_dadosAvariaPallet);

                dadosAvaria["QuantidadesAvariadas"] = obterSituacoes();

                executarReST("Avaria/AdicionarDadosAvaria", dadosAvaria, function (retorno) {
                    if (retorno.Success) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados da avaria adicionados com sucesso");

                        _gridAvariaPallet.CarregarGrid();

                        var anexos = obterAnexos();

                        limparCamposAvariaPallet();
                        buscarSituacoes();
                        enviarArquivosAnexados(retorno.Data, anexos);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }, null);
            }
        }
        else
            exibirMensagemCamposObrigatorio();
    });
}

function controlarCamposDadosAvariaHabilitados() {
    var habilitarCampo = (_avariaPallet.Situacao.val() === EnumSituacaoAvariaPallet.Todas);

    _dadosAvariaPallet.Filial.enable(habilitarCampo);
    _dadosAvariaPallet.MotivoAvaria.enable(habilitarCampo);
    _dadosAvariaPallet.Setor.enable(false);
    _dadosAvariaPallet.Observacao.enable(habilitarCampo);
    _dadosAvariaPallet.Transportador.enable(habilitarCampo);
}

function controlarSetorDadosAvaria(codigoFilial) {
    _dadosAvariaPallet.Setor.enable(codigoFilial > 0);
    _dadosAvariaPallet.Setor.val("");
    _dadosAvariaPallet.Setor.codEntity(0);
}

function limparCamposDadosAvaria() {
    LimparCampos(_dadosAvariaPallet);
}

function preencherDadosAvaria(dadosAvaria) {
    PreencherObjetoKnout(_dadosAvariaPallet, { Data: dadosAvaria });
}