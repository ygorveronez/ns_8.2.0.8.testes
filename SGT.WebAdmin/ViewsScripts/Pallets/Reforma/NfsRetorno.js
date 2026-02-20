/// <reference path="Reforma.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridNfsRetorno;
var _nfsRetornoReformaPallet;
var _listaNfsRetornoReformaPallet;

/*
 * Declaração das Classes
 */

var NfsRetornoReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: true, getType: typesKnockout.int, val: ko.observable(""), def: "", text: "*Número:", enable: ko.observable(true) });
    this.Serie = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: true, getType: typesKnockout.int, val: ko.observable(""), def: "", text: "*Série:", issue: 756, enable: ko.observable(true) });
    this.ValorPrestacaoServico = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable(""), def: "", required: true, text: "*Valor Prestação do Serviço:", eventChange: AtualizarBCISS, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ type: types.map, required: true, getType: typesKnockout.date, text: "*Data Emissão:", enable: ko.observable(true) });
    this.AliquotaISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 6, def: "", required: true, text: "*Alíquota ISS:", enable: ko.observable(true), eventChange: AtualizarBCISS });
    this.ValorISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor ISS:", enable: ko.observable(false) });
    this.BaseCalculo = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Base de Cálculo:", enable: ko.observable(false) });
    this.PercentualRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 6, def: "0,00", text: "Percentual Retenção:", enable: ko.observable(true), eventChange: CalcularRetencao, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor Retenção:", enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.IncluirValorBC = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true, text: "Incluir o valor do ISS na BC:", enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Observação:", enable: ko.observable(true) });

    this.IncluirValorBC.val.subscribe(AtualizarBCISS);

    this.Adicionar = PropertyEntity({ eventClick: adicionarNfsRetornoClick, type: types.event, text: ko.observable("Adicionar") });
}

var ListaNfsRetornoReformaPallet = function () {
    this.PermiteAdicionarNfs = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaNfsRetorno = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaNfsRetorno.val.subscribe(function () {
        recarregarGridNfsRetorno();
    });

    this.AdicionarNfs = PropertyEntity({ eventClick: adicionarNfsRetornoModalClick, type: types.event, text: ko.observable("Adicionar NFS"), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridNfsRetorno() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerNfsRetorno, icone: "", visibilidade: isSituacaoPermiteGerenciarNfsRetorno };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "30%", className: "text-align-left" },
        { data: "Serie", title: "Série", width: "30%", className: "text-align-left" }
    ];

    _gridNfsRetorno = new BasicDataTable(_listaNfsRetornoReformaPallet.ListaNfsRetorno.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridNfsRetorno.CarregarGrid([]);
}

function loadNfsRetornoReformaPallet() {
    _nfsRetornoReformaPallet = new NfsRetornoReformaPallet();
    KoBindings(_nfsRetornoReformaPallet, "knockoutCadastroNfsRetornoReformaPallet");

    _listaNfsRetornoReformaPallet = new ListaNfsRetornoReformaPallet();
    KoBindings(_listaNfsRetornoReformaPallet, "knockoutNfsRetornoReformaPallet");

    loadGridNfsRetorno();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarNfsRetornoClick() {
    if (isSituacaoPermiteGerenciarNfsRetorno()) {
        if (ValidarCamposObrigatorios(_nfsRetornoReformaPallet)) {
            _nfsRetornoReformaPallet.Codigo.val(_reformaPallet.Codigo.val());

            var nfsRetorno = RetornarObjetoPesquisa(_nfsRetornoReformaPallet);

            executarReST("Reforma/AdicionarNfsRetorno", nfsRetorno, function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS adicionada com sucesso");

                    preencherNfsRetorno(retorno.Data.ListaNfsRetorno);
                    fecharNfsRetornoModal();
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    }
}

function adicionarNfsRetornoModalClick() {
    Global.abrirModal('divModalNfsRetorno');
    $("#divModalNfsRetorno").one('hidden.bs.modal', function () {
        LimparCampos(_nfsRetornoReformaPallet);
    });
}

/*
 * Declaração das Funções
 */

function AtualizarBCISS() {
    _nfsRetornoReformaPallet.BaseCalculo.val(_nfsRetornoReformaPallet.ValorPrestacaoServico.val() || "0,00");

    CalcularISS();
}

function CalcularISS() {
    if (_nfsRetornoReformaPallet.BaseCalculo.val() && _nfsRetornoReformaPallet.AliquotaISS.val()) {
        var aliquota = Globalize.parseFloat(_nfsRetornoReformaPallet.AliquotaISS.val());
        var baseCalculo = Globalize.parseFloat(_nfsRetornoReformaPallet.BaseCalculo.val());

        if (_nfsRetornoReformaPallet.IncluirValorBC.val())
            baseCalculo += aliquota > 0 ? ((baseCalculo / ((100 - aliquota) / 100)) - baseCalculo) : 0;

        var valorISS = baseCalculo * (aliquota / 100);

        _nfsRetornoReformaPallet.BaseCalculo.val(Globalize.format(baseCalculo, "n2"));
        _nfsRetornoReformaPallet.ValorISS.val(Globalize.format(valorISS, "n2"));

        CalcularRetencao();
    }
}

function CalcularRetencao() {
    if (_nfsRetornoReformaPallet.ValorISS.val() && _nfsRetornoReformaPallet.PercentualRetencao.val() != _nfsRetornoReformaPallet.PercentualRetencao.def) {
        var percentualRetencao = Globalize.parseFloat(_nfsRetornoReformaPallet.PercentualRetencao.val());

        if (percentualRetencao <= 100) {
            var valorISS = Globalize.parseFloat(_nfsRetornoReformaPallet.ValorISS.val());
            var valorRetencao = valorISS * (percentualRetencao / 100);

            _nfsRetornoReformaPallet.ValorRetencao.val(Globalize.format(valorRetencao, "n2"));
        } else {
            _nfsRetornoReformaPallet.PercentualRetencao.val(_nfsRetornoReformaPallet.PercentualRetencao.def);
            _nfsRetornoReformaPallet.ValorRetencao.val(_nfsRetornoReformaPallet.ValorRetencao.def);
        }
    }
    else
        _nfsRetornoReformaPallet.ValorRetencao.val("0,00");
}

function fecharNfsRetornoModal() {
    Global.fecharModal("divModalNfsRetorno");
}

function isSituacaoPermiteGerenciarNfsRetorno() {
    return (_reformaPallet.Situacao.val() === EnumSituacaoReformaPallet.AguardandoRetorno);
}

function limparNfsRetorno() {
    _listaNfsRetornoReformaPallet.PermiteAdicionarNfs.val(false);
    _listaNfsRetornoReformaPallet.ListaNfsRetorno.val(new Array());
}

function obterListaNfsRetorno() {
    return _listaNfsRetornoReformaPallet.ListaNfsRetorno.val().slice();
}

function preencherNfsRetorno(dadosNfsRetorno) {
    _listaNfsRetornoReformaPallet.ListaNfsRetorno.val(dadosNfsRetorno);
    _listaNfsRetornoReformaPallet.PermiteAdicionarNfs.val(isSituacaoPermiteGerenciarNfsRetorno());
}

function recarregarGridNfsRetorno() {
    var listaNfsRetorno = obterListaNfsRetorno();

    _gridNfsRetorno.CarregarGrid(listaNfsRetorno);
}

function removerNfsRetorno(registroSelecionado) {
    if (isSituacaoPermiteGerenciarNfsRetorno()) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a da NFS", function () {
            executarReST("Reforma/ExcluirNfsRetorno", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Data) {
                    removerNfsRetornoLocal(registroSelecionado);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        });
    }
}

function removerNfsRetornoLocal(registroSelecionado) {
    var listaNfsRetorno = obterListaNfsRetorno();

    listaNfsRetorno.forEach(function (nfsRetorno, i) {
        if (registroSelecionado.Codigo == nfsRetorno.Codigo) {
            listaNfsRetorno.splice(i, 1);
        }
    });

    _listaNfsRetornoReformaPallet.ListaNfsRetorno.val(listaNfsRetorno);
}

function validarNfsRetornoInformadas() {
    if (_listaNfsRetornoReformaPallet.ListaNfsRetorno.val().length == 0) {
        exibirMensagem("atencao", "NFS de Retorno", "Por Favor, cadastre ao menos uma NFS de Retorno");

        return false;
    }

    return true;
}