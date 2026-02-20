/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Seguradora.js" />
/// <reference path="../../Enumeradores/EnumResponsavelSeguro.js" />
/// <reference path="../../Enumeradores/EnumSeguradoraAverbacao.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridApoliceSeguro;
var _apoliceSeguro;
var _CRUDApoliceSeguro;
var _pesquisaApoliceSeguro;
var _bindConfiguracoesAverbadoras;
var _codigoApoliceSeguroEditada;

var _seguradoraAverbacao = [
    { value: EnumSeguradoraAverbacao.NaoDefinido, text: "Não Definido" },
    { value: EnumSeguradoraAverbacao.ATM, text: "ATM" },
    { value: EnumSeguradoraAverbacao.Bradesco, text: "Bradesco" },
    { value: EnumSeguradoraAverbacao.PortoSeguro, text: "Porto Seguro" },
    { value: EnumSeguradoraAverbacao.ELT, text: "ELT" },
    { value: EnumSeguradoraAverbacao.Senig, text: "Senig" }
];

var PesquisaApoliceSeguro = function () {

    this.Responsavel = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumResponsavelSeguro.obterOpcoesPesquisa(), enable: ko.observable(true), text: "Responsável:" });
    this.NumeroApolice = PropertyEntity({ text: "Número da Apólice:", maxlength: 50 });
    this.NumeroAverbacao = PropertyEntity({ text: "Número da Averbação:", maxlength: 50 });
    this.InicioVigencia = PropertyEntity({ text: "Início da Vigência:", issue: 591, getType: typesKnockout.date });
    this.FimVigencia = PropertyEntity({ text: "Fim da Vigência:", getType: typesKnockout.date });
    this.Ativa = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusFemPesquisa, def: 0, visible: ko.observable(true) });
    this.Seguradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Seguradora:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), text: "Empresa:", idBtnSearch: guid() });


    this.InicioVigencia.dateRangeLimit = this.FimVigencia;
    this.FimVigencia.dateRangeInit = this.InicioVigencia;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridApoliceSeguro.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ApoliceSeguro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Responsavel = PropertyEntity({ val: ko.observable(EnumResponsavelSeguro.Transportador), def: EnumResponsavelSeguro.Transportador, options: EnumResponsavelSeguro.obterOpcoes(), text: "*Responsável:", issue: 261, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.NumeroApolice = PropertyEntity({ text: "*Número da Apólice:", issue: 589, maxlength: 20, visible: ko.observable(true), required: true });
    this.NumeroAverbacao = PropertyEntity({ text: "Número da Averbação:", issue: 590, maxlength: 40, visible: ko.observable(true) });
    this.InicioVigencia = PropertyEntity({ text: "*Início da Vigência:", issue: 591, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.FimVigencia = PropertyEntity({ text: "*Fim da Vigência:", issue: 591, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.Ativa = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _statusFem, def: true, visible: ko.observable(true), required: true });
    this.Seguradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Seguradora: ", issue: 262, idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58, visible: ko.observable(false), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 52, visible: ko.observable(false), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", issue: 595, idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.SeguradoraAverbacao = PropertyEntity({ val: ko.observable(EnumSeguradoraAverbacao.NaoDefinido), def: EnumSeguradoraAverbacao.NaoDefinido, options: _seguradoraAverbacao, text: "Averbadora:", issue: 594 });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, maxlength: 300, visible: ko.observable(true), required: false });
    this.LimitirValorApolice = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: true });
    this.ValorLimiteApolice = PropertyEntity({ text: "Valor Limite da Apólice: ", getType: typesKnockout.decimal, maxlength: 16, required: false, visibleFade: ko.observable(false), enable: ko.observable(false) });
    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 300, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS), required: false });
    this.ValorFixoAverbacaoEnable = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: true });
    this.ValorFixoAverbacao = PropertyEntity({ text: "Valor Fixo de Averbação: ", getType: typesKnockout.decimal, maxlength: 16, required: false, visibleFade: ko.observable(false), enable: ko.observable(false) });

    this.Descontos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.GrupoPessoas.codEntity.subscribe(function (novoValor) {
        changeGrupoPessoas(novoValor);
    });

    this.Pessoa.codEntity.subscribe(function (novoValor) {
        changePessoa();
    });

    this.Responsavel.val.subscribe(function (novoValor) {
        changeResponsavel();
    });

    this.SeguradoraAverbacao.val.subscribe(function (novoValor) {
        changeSeguradoraAverbacao();
    });
}

var CRUDApoliceSeguro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ eventClick: gerenciarAnexosClick, type: types.event, text: "Anexos", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadApoliceSeguro() {
    _apoliceSeguro = new ApoliceSeguro();
    KoBindings(_apoliceSeguro, "knockoutCadastroApoliceSeguro");

    _CRUDApoliceSeguro = new CRUDApoliceSeguro();
    KoBindings(_CRUDApoliceSeguro, "knoutCRUDApoliceSeguro");

    _pesquisaApoliceSeguro = new PesquisaApoliceSeguro();
    KoBindings(_pesquisaApoliceSeguro, "knockoutPesquisaApoliceSeguro", _pesquisaApoliceSeguro.Pesquisar.id);

    HeaderAuditoria("ApoliceSeguro", _apoliceSeguro);

    new BuscarClientes(_apoliceSeguro.Pessoa, null, true);
    new BuscarGruposPessoas(_apoliceSeguro.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarSeguradoras(_apoliceSeguro.Seguradora, null, true);
    new BuscarEmpresa(_apoliceSeguro.Empresa);

    new BuscarClientes(_pesquisaApoliceSeguro.Pessoa, null, true);
    new BuscarGruposPessoas(_pesquisaApoliceSeguro.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarSeguradoras(_pesquisaApoliceSeguro.Seguradora);
    new BuscarEmpresa(_pesquisaApoliceSeguro.Empresa);

    $("#" + _apoliceSeguro.LimitirValorApolice.id).click(limitirValorApoliceClick);
    $("#" + _apoliceSeguro.ValorFixoAverbacaoEnable.id).click(ValorFixoAverbacaoEnableClick);

    buscarApolicesSeguro();

    $("#" + _apoliceSeguro.GrupoPessoas.id).focusout(function () {
        focusOutGrupoPessoas();
    });

    $("#" + _apoliceSeguro.Pessoa.id).focusout(function () {
        focusOutPessoa();
    });

    changeResponsavel();

    // Carrega configuracao
    loadConfiguracaoATM();
    loadConfiguracaoBradeco();
    loadConfiguracaoPortoSeguro();
    loadConfiguracaoSenig();
    loadApoliceSeguroDesconto();
    loadAnexosApolice()

    bindConfiguracoesAverbadoras();

    validacaoPortalMultiTransportador();
}

function limitirValorApoliceClick(e) {
    verificarSeLimitaValorApolice();
}

function ValorFixoAverbacaoEnableClick() {
    verificarValorFixoAverbacaoEnable();
}

function changeGrupoPessoas(data) {
    if (Globalize.parseInt(_apoliceSeguro.GrupoPessoas.codEntity().toString()) > 0) {
        _apoliceSeguro.Pessoa.enable(false);
    } else {
        _apoliceSeguro.Pessoa.enable(true);
    }
}

function changeResponsavel() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _apoliceSeguro.GrupoPessoas.visible(true);
        _apoliceSeguro.Pessoa.visible(true);

        return;
    }

    if (_apoliceSeguro.Responsavel.val() == EnumResponsavelSeguro.Embarcador && PrecisaGrupoPessoa()) {
        _apoliceSeguro.GrupoPessoas.visible(true);
        _apoliceSeguro.Pessoa.visible(true);
    } else {
        _apoliceSeguro.GrupoPessoas.visible(false);
        _apoliceSeguro.Pessoa.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaApoliceSeguro.Empresa.visible(true).enable(true);

        if (_apoliceSeguro.Responsavel.val() == EnumResponsavelSeguro.Transportador) {
            _apoliceSeguro.Empresa.visible(true).enable(true);
        } else {
            _apoliceSeguro.Empresa.visible(false).enable(false);
        }
    } else {
        _pesquisaApoliceSeguro.Empresa.visible(false).enable(false);

    }
}

function changeSeguradoraAverbacao() {
    // Esconde todas configuração de averbação
    $(".configuracao-averbacao").hide();

    if (_apoliceSeguro.SeguradoraAverbacao.val() == EnumSeguradoraAverbacao.ATM)
        $("#liATM").show();
    else if (_apoliceSeguro.SeguradoraAverbacao.val() == EnumSeguradoraAverbacao.Bradesco)
        $("#liBradesco").show();
    else if (_apoliceSeguro.SeguradoraAverbacao.val() == EnumSeguradoraAverbacao.PortoSeguro)
        $("#liPortoSeguro").show();
    else if (_apoliceSeguro.SeguradoraAverbacao.val() == EnumSeguradoraAverbacao.Senig)
        $("#liSenig").show();
}

function focusOutGrupoPessoas() {
    if (_apoliceSeguro.GrupoPessoas.val().trim() == "")
        _apoliceSeguro.GrupoPessoas.codEntity(0);
}

function changePessoa(data) {
    if (Globalize.parseInt(_apoliceSeguro.Pessoa.codEntity().toString()) > 0) {
        _apoliceSeguro.GrupoPessoas.enable(false);
    } else {
        _apoliceSeguro.GrupoPessoas.enable(true);
    }
}

function focusOutPessoa() {
    if (_apoliceSeguro.Pessoa.val().trim() == "")
        _apoliceSeguro.Pessoa.codEntity(0);
}

function ValidarApoliceSeguro() {
    if (EnumSeguradoraAverbacao.NaoDefinido != _apoliceSeguro.SeguradoraAverbacao.val()) {
        // Pega a configuracao da averbacao espeficia (ATM/BRADESCO)
        var _averbadora = GetConfiguracaoAverbadora();

        // Valida se as config da averbadora estao ok
        if (_averbadora != null && !ValidarCamposObrigatorios(_averbadora))
            return exibirMensagem(tipoMensagem.atencao, "Atenção!", "A configurações de averbação não estão válidas.");
    }

    if (_apoliceSeguro.Responsavel.val() == EnumResponsavelSeguro.Embarcador && PrecisaGrupoPessoa()) {
        if (Globalize.parseInt(_apoliceSeguro.Pessoa.codEntity().toString()) <= 0 && Globalize.parseInt(_apoliceSeguro.GrupoPessoas.codEntity().toString()) <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "É necessário selecionar um grupo de pessoas ou uma pessoa para a apólice de seguro.");
            return false;
        }
    }

    return true;
}

function adicionarClick(e, sender) {
    if (ValidarApoliceSeguro() === true) {

        _apoliceSeguro.Descontos.val(obterApoliceSeguroDesconto());

        Salvar(ObjetoAverbacao(), "ApoliceSeguro/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    EnviarArquivosAnexadosApolice(arg.Data);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridApoliceSeguro.CarregarGrid();
                    limparCamposApoliceSeguro();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    if (ValidarApoliceSeguro() === true) {
        _apoliceSeguro.Descontos.val(obterApoliceSeguroDesconto());

        Salvar(ObjetoAverbacao(), "ApoliceSeguro/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                    _gridApoliceSeguro.CarregarGrid();
                    limparCamposApoliceSeguro();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta apolice de seguro?", function () {
        ExcluirPorCodigo(_apoliceSeguro, "ApoliceSeguro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridApoliceSeguro.CarregarGrid();
                    limparCamposApoliceSeguro();
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
    limparCamposApoliceSeguro();
}

//*******MÉTODOS*******


function verificarSeLimitaValorApolice() {
    if (_apoliceSeguro.LimitirValorApolice.val()) {
        _apoliceSeguro.ValorLimiteApolice.required = true;
        _apoliceSeguro.ValorLimiteApolice.enable(true);
    } else {
        _apoliceSeguro.ValorLimiteApolice.required = false;
        _apoliceSeguro.ValorLimiteApolice.enable(false);
        _apoliceSeguro.ValorLimiteApolice.val("");
    }
}

function verificarValorFixoAverbacaoEnable() {
    if (_apoliceSeguro.ValorFixoAverbacaoEnable.val()) {
        _apoliceSeguro.ValorFixoAverbacao.required = true;
        _apoliceSeguro.ValorFixoAverbacao.enable(true);
    } else {
        _apoliceSeguro.ValorFixoAverbacao.required = false;
        _apoliceSeguro.ValorFixoAverbacao.enable(false);
        _apoliceSeguro.ValorFixoAverbacao.val("");
    }
}

function editarApoliceSeguro(tipoCargaGrid) {
    limparCamposApoliceSeguro();
    _apoliceSeguro.Codigo.val(tipoCargaGrid.Codigo);
    BuscarPorCodigo(_apoliceSeguro, "ApoliceSeguro/BuscarPorCodigo", function (arg) {
        _pesquisaApoliceSeguro.ExibirFiltros.visibleFade(false);
        _CRUDApoliceSeguro.Atualizar.visible(true);
        _CRUDApoliceSeguro.Cancelar.visible(true);
        _CRUDApoliceSeguro.Excluir.visible(true);
        _CRUDApoliceSeguro.Adicionar.visible(false);
        verificarSeLimitaValorApolice();
        verificarValorFixoAverbacaoEnable();
        CarregarAnexosApolice(arg.Data);
        _codigoApoliceSeguroEditada = _apoliceSeguro.Codigo.val();
        _listaApoliceSeguroDesconto.ImportarDescontos.visible(true);

        // Carrega informação averbação
        var averbacao = GetConfiguracaoAverbadora();
        if (averbacao != null)
            PreencherObjetoKnout(averbacao, { Data: arg.Data.ConfiguracaoAverbacao })
        recarregarGridApoliceSeguroDesconto();

    }, null);
}

function buscarApolicesSeguro() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarApoliceSeguro, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridApoliceSeguro = new GridView(_pesquisaApoliceSeguro.Pesquisar.idGrid, "ApoliceSeguro/Pesquisa", _pesquisaApoliceSeguro, menuOpcoes, null);
    _gridApoliceSeguro.CarregarGrid();
}

function limparCamposApoliceSeguro() {
    _CRUDApoliceSeguro.Atualizar.visible(false);
    _CRUDApoliceSeguro.Cancelar.visible(false);
    _CRUDApoliceSeguro.Excluir.visible(false);
    _CRUDApoliceSeguro.Adicionar.visible(true);
    _apoliceSeguro.ValorLimiteApolice.enable(false);
    _listaApoliceSeguroDesconto.ImportarDescontos.visible(false);
    LimparCampos(_apoliceSeguro);
    limparAnexosApolice();
    recarregarGridApoliceSeguroDesconto();

    // Limpar os campos de configrações
    _bindConfiguracoesAverbadoras.forEach(function (averb) {
        var _averbadora = averb.Callback();
        if (_averbadora != null)
            LimparCampos(_averbadora);
    });

    $("#myTab a:first").click();
}

function PrecisaGrupoPessoa() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador;
}

function bindConfiguracoesAverbadoras() {
    _bindConfiguracoesAverbadoras = [
        {
            Enum: EnumSeguradoraAverbacao.ATM,
            Callback: function () { return _configuracaoATM; }
        },
        {
            Enum: EnumSeguradoraAverbacao.Bradesco,
            Callback: function () { return _configuracaoBradeco; }
        },
        {
            Enum: EnumSeguradoraAverbacao.PortoSeguro,
            Callback: function () { return _configuracaoPortoSeguro; }
        }, 
        {
            Enum: EnumSeguradoraAverbacao.Senig,
            Callback: function () { return _configuracaoSenig; }
        },
    ];
}

function GetConfiguracaoAverbadora() {
    var enumAverbadora = _apoliceSeguro.SeguradoraAverbacao.val();
    var _averbadora;

    _bindConfiguracoesAverbadoras.forEach(function (averb) {
        if (averb.Enum == enumAverbadora)
            _averbadora = averb.Callback();
    });

    return _averbadora;
}

function ObjetoAverbacao() {
    // Junta o KO do _apolice com a configuração da aberbação
    var _objetoko = {};

    var averbadora = GetConfiguracaoAverbadora();
    if (averbadora != null)
        for (var i in averbadora)
            _objetoko[i] = averbadora[i];

    for (var i in _apoliceSeguro)
        _objetoko[i] = _apoliceSeguro[i];

    return _objetoko;
}

function obterApoliceSeguroDesconto() {
    return JSON.stringify(_gridApoliceSeguroDesconto.BuscarRegistros());
}

function recarregarGridApoliceSeguroDesconto() {
    _gridApoliceSeguroDesconto.CarregarGrid(_apoliceSeguro.Descontos.val() || []);
}

function validacaoPortalMultiTransportador() {
    if (!(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe))
        return;
        
    executarReST("ApoliceSeguro/ValidarPortalMultiTransportador", null, function (retorno) {
        if (retorno.Success)
            if (retorno.Data)
                _pesquisaApoliceSeguro.Empresa.codEntity(retorno.Data.CodigoEmpresa);
    });

    _pesquisaApoliceSeguro.Empresa.visible(false);
    _pesquisaApoliceSeguro.Empresa.enable(false);

    _pesquisaApoliceSeguro.Responsavel.val(EnumResponsavelSeguro.Transportador);

    _apoliceSeguro.Empresa.visible(false);
    _apoliceSeguro.Empresa.enable(false);

    _apoliceSeguro.Responsavel.val(EnumResponsavelSeguro.Transportador);
    _apoliceSeguro.Responsavel.enable(false);

    _gridApoliceSeguro.CarregarGrid();
}