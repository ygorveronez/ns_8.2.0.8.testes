/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/ContaTransportador.js" />
/// <reference path="../../Enumeradores/EnumStatusCheque.js" />
/// <reference path="../../Enumeradores/EnumTipoCheque.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDCheque;
var _cheque;
var _pesquisaCheque;
var _gridCheque;

/*
 * Declaração das Classes
 */

var Cheque = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Banco:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataCompensacao = PropertyEntity({ text: "*Data Compensação:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true) });
    this.DataTransacao = PropertyEntity({ text: "*Data Transação:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "*Data Vencimento:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.DigitoAgencia = PropertyEntity({ text: "Dígito Agência:", maxlength: 1, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Empresa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.Numero = PropertyEntity({ text: "Código:", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: false });
    this.NumeroAgencia = PropertyEntity({ text: "Agência:", maxlength: 10, enable: ko.observable(true) });
    this.NumeroCheque = PropertyEntity({ text: "*Número:", maxlength: 10, required: true, enable: ko.observable(true) });
    this.NumeroConta = PropertyEntity({ text: "Número Conta:", maxlength: 10, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.PessoaRepasse = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Pessoa do Repasse:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.ContaTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta Transportador:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false) });

    this.TipoMovimentoCompensacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Tipo Movimento de Compensação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(EnumStatusCheque.Normal), options: EnumStatusCheque.obterOpcoes(), def: EnumStatusCheque.Normal, required: true, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoCheque.Recebido), options: EnumTipoCheque.obterOpcoes(), def: EnumTipoCheque.Recebido, required: true, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 12, required: true, enable: ko.observable(true) });
    this.BaixasTitulo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.Status.val.subscribe(controlarExibicaoDadosCompensacao);
    this.Tipo.val.subscribe(controlarExibicaoTipos);
};

var CRUDCheque = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Extornar = PropertyEntity({ eventClick: extornarClick, type: types.event, text: "Estornar", visible: ko.observable(false) });
};

var PesquisaCheque = function () {
    this.NumeroCheque = PropertyEntity({ text: "Nº Cheque: " });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Pessoa:"), idBtnSearch: guid() });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCheque.Todos), options: EnumStatusCheque.obterOpcoesPesquisa(), def: EnumStatusCheque.Todos, text: "Status: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCheque.Todos), options: EnumTipoCheque.obterOpcoesPesquisa(), def: EnumTipoCheque.Todos, text: "Tipo: " });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCheque, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCheque() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "14", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "Cheque/ExportarPesquisa", titulo: "Cheques" };

    _gridCheque = new GridViewExportacao(_pesquisaCheque.Pesquisar.idGrid, "Cheque/Pesquisa", _pesquisaCheque, menuOpcoes, configuracoesExportacao);
    _gridCheque.CarregarGrid();
}

function carregarLancamentoCheque(idDivConteudo, callback) {
    $.get("Content/Static/Financeiro/Cheque.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _cheque = new Cheque();
        KoBindings(_cheque, "knockoutCheque");

        _CRUDCheque = new CRUDCheque();
        KoBindings(_CRUDCheque, "knockoutCRUDCheque");

        new BuscarBanco(_cheque.Banco);
        new BuscarClientes(_cheque.Pessoa);
        new BuscarClientes(_cheque.PessoaRepasse);
        new BuscarEmpresa(_cheque.Empresa);
        new BuscarTipoMovimento(_cheque.TipoMovimentoCompensacao);
        new BuscarContaTransportador(_cheque.ContaTransportador, RetornoContaTransportador);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            _cheque.Empresa.visible(false);
            _cheque.Empresa.required(false);
        }

        loadAnexo();

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function RetornoContaTransportador(data) {
    _cheque.Banco.val(data.Banco);
    _cheque.Banco.codEntity(data.CodigoBanco);

    _cheque.NumeroAgencia.val(data.NumeroAgencia);
    _cheque.NumeroConta.val(data.NumeroConta);
    _cheque.DigitoAgencia.val(data.DigitoAgencia);
}

function loadCheque() {
    carregarLancamentoCheque("conteudoCheque", loagPesquisaCheque);
}

function loagPesquisaCheque() {
    HeaderAuditoria("Cheque", _cheque);

    _pesquisaCheque = new PesquisaCheque();
    KoBindings(_pesquisaCheque, "knockoutPesquisaCheque", false, _pesquisaCheque.Pesquisar.id);

    new BuscarClientes(_pesquisaCheque.Pessoa);

    loadGridCheque();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_cheque, "Cheque/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                enviarArquivosAnexados(retorno.Data.Codigo);
                recarregarGridCheque();
                limparCamposCheque();

                if ($("#divModalCheque").length > 0)
                    Global.fecharModal("divModalCheque");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_cheque, "Cheque/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCheque();
                limparCamposCheque();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCheque();
}

function editarClick(registroSelecionado) {
    limparCamposCheque();

    executarReST("Cheque/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_cheque, retorno);

                _anexo.Anexos.val(retorno.Data.Anexos);
                _pesquisaCheque.ExibirFiltros.visibleFade(false);

                controlarExibicaoTipos();
                controlarComponentesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_cheque, "Cheque/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCheque();
                    limparCamposCheque();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function extornarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar o cheque?", function () {
        executarReST("Cheque/Extornar", { Codigo: _cheque.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cheque estornado com sucesso.");

                    recarregarGridCheque();
                    limparCamposCheque();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao, isPermiteEditar) {
    if (isEdicao) {
        _CRUDCheque.Adicionar.visible(false);

        if (isPermiteEditar) {
            _CRUDCheque.Atualizar.visible(true);
            _CRUDCheque.Excluir.visible(true);
            _CRUDCheque.Extornar.visible(false);
        }
        else {
            _CRUDCheque.Atualizar.visible(false);
            _CRUDCheque.Excluir.visible(false);
            _CRUDCheque.Extornar.visible(isChequeCompensado());
        }
    }
    else {
        _CRUDCheque.Adicionar.visible(true);
        _CRUDCheque.Atualizar.visible(false);
        _CRUDCheque.Excluir.visible(false);
        _CRUDCheque.Extornar.visible(false);
    }
}

function controlarCamposHabilitados(isEdicao, isPermiteEditar) {
    var habilitarCampos = !isEdicao || isPermiteEditar;

    _cheque.Banco.enable(habilitarCampos);
    _cheque.DataCompensacao.enable(habilitarCampos);
    _cheque.DataTransacao.enable(habilitarCampos);
    _cheque.DataVencimento.enable(habilitarCampos);
    _cheque.DigitoAgencia.enable(habilitarCampos);
    _cheque.Empresa.enable(habilitarCampos);
    _cheque.NumeroAgencia.enable(habilitarCampos);
    _cheque.NumeroCheque.enable(habilitarCampos);
    _cheque.NumeroConta.enable(habilitarCampos);
    _cheque.Observacao.enable(habilitarCampos);
    _cheque.Pessoa.enable(habilitarCampos);
    _cheque.TipoMovimentoCompensacao.enable(habilitarCampos);
    _cheque.Status.enable(habilitarCampos);
    _cheque.Tipo.enable(habilitarCampos);
    _cheque.Valor.enable(habilitarCampos);
    _cheque.PessoaRepasse.enable(habilitarCampos);
    _anexo.Anexos.visible(habilitarCampos);

    if (isChequeContemBaixa()) {
        _cheque.Valor.enable(false);
        _cheque.DataTransacao.enable(false);
    }
}

function controlarComponentesHabilitados() {
    var isEdicao = isRegistroEdicao();
    var isPermiteEditar = isPermitirEditarRegistro();

    controlarBotoesHabilitados(isEdicao, isPermiteEditar);
    controlarCamposHabilitados(isEdicao, isPermiteEditar);
}

function controlarExibicaoTipos() {
    var chequeRepassado = isChequeRepassado();
    var chequeEmitido = isChequeEmitido();

    _cheque.PessoaRepasse.visible(chequeRepassado);
    _cheque.PessoaRepasse.required(chequeRepassado);

    if (chequeEmitido && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _cheque.ContaTransportador.visible(true);
        _cheque.Empresa.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    } else {
        _cheque.ContaTransportador.visible(false);
        _cheque.Empresa.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    }
}

function controlarExibicaoDadosCompensacao() {
    var chequeCompensado = isChequeCompensado();

    if (chequeCompensado)
        $("#container-dados-compensacao").show();
    else
        $("#container-dados-compensacao").hide();

    _cheque.DataCompensacao.required(chequeCompensado);
    _cheque.TipoMovimentoCompensacao.required(chequeCompensado);
}

function isChequeRepassado() {
    return _cheque.Tipo.val() === EnumTipoCheque.Repassado;
}

function isChequeCompensado() {
    return _cheque.Status.val() === EnumStatusCheque.Compensado;
}

function isChequeEmitido() {
    return _cheque.Tipo.val() === EnumTipoCheque.Emitido;
}

function isPermitirEditarRegistro() {
    if (_cheque.Status.val() === EnumStatusCheque.Cancelado || _cheque.Status.val() === EnumStatusCheque.Compensado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O status do cheque não permite edição.");

        return false;
    }

    if (_cheque.BaixasTitulo.val().length > 0) {
        if (_cheque.BaixasTitulo.val().length > 1) {
            var mensagem = "O cheque está vinculado a " + _cheque.BaixasTitulo.val().length + " baixas, permitida a edição de apenas alguns campos.</br>";

            for (var i = 0; i < _cheque.BaixasTitulo.val().length; i++)
                mensagem += " " + _cheque.BaixasTitulo.val()[i].Descricao;

            exibirMensagem(tipoMensagem.aviso, "Aviso", mensagem);
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", "O cheque está vinculado a uma baixa, permitida a edição de apenas alguns campos.</br>" + _cheque.BaixasTitulo.val()[0].Descricao);
    }

    return true;
}

function isChequeContemBaixa() {
    return _cheque.BaixasTitulo.val().length > 0;
}

function isRegistroEdicao() {
    return _cheque.Codigo.val() > 0;
}

function limparCamposCheque() {
    LimparCampos(_cheque);
    limparCamposAnexo();

    Global.ResetarAbas();

    controlarComponentesHabilitados();
    controlarExibicaoTipos();
}

function recarregarGridCheque() {
    if (_gridCheque !== undefined && _gridCheque !== null)
        _gridCheque.CarregarGrid();
}