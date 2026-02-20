/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Global/Ajuda/ajuda.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumTipoCargaMDFe.js" />
/// <reference path="../../Enumeradores/EnumModalPropostaMultimodal.js" />
/// <reference path="Krona.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="TempoDescargaPorFaixaPeso.js" />
/// <reference path="Integracao.js" />
/// <reference path="Opentech.js" />
/// <reference path="TipoLicenca.js" />
/// <reference path="EFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCarga;
var _tipoCarga;
var _crudTipoCarga;
var _pesquisaTipoCarga;

var PesquisaTipoCarga = function () {
    var visibilidadeGrupo = false;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        visibilidadeGrupo = true;

    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.CodigoTipoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 50, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, visible: visibilidadeGrupo, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: visibilidadeGrupo, text: Localization.Resources.Cargas.TipoCarga.Pessoa.getFieldDescription(), idBtnSearch: guid() });
    this.PrioridadeCarga = PropertyEntity({ type: types.entity, visible: _CONFIGURACAO_TMS.Carga.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.PrioridadeCarga.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoCarga.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Cargas.TipoCarga.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(visibilidadeGrupo)
    });
};

var TipoCarga = function () {

    var visibilidadeGrupo = false;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        visibilidadeGrupo = true;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true });

    this.TipoTempoDescarga = PropertyEntity({ val: ko.observable(true), options: EnumListaTipoTempoDescarga.obterOpcoes(), def: true, text: Localization.Resources.Cargas.TipoCarga.TipoDoTempoDeDescarga.getFieldDescription() });

    this.TipoTempoDescarga.val.subscribe((a) => {
        if (a == 1) {
            $("#liModeloVeicular").show();
            $("#liTempoDescargaPorPeso").hide();
        } else if (a == 2) {
            $("#liModeloVeicular").hide();
            $("#liTempoDescargaPorPeso").show();
        }
    })

    this.ModalProposta = PropertyEntity({ val: ko.observable(EnumModalPropostaMultimodal.Nenhum), options: EnumModalPropostaMultimodal.obterOpcoes(), text: Localization.Resources.Cargas.TipoCarga.ModalProposta.getFieldDescription(), def: EnumModalPropostaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });

    this.CodigoTipoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 250, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), issue: 58, visible: ko.observable(visibilidadeGrupo) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.Pessoa.getFieldDescription(), idBtnSearch: guid(), issue: 52, visible: ko.observable(visibilidadeGrupo) });
    this.ControlaTemperatura = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Cargas.TipoCarga.ControlaTemperatura, issue: 577 });
    this.FaixaTemperatura = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.FaixaDeTemperatura.getFieldDescription(), val: ko.observable(""), def: "", options: ko.observable([]) });
    this.ExigeVeiculoRastreado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Cargas.TipoCarga.ExigeVeiculoRastreado, issue: 580 });
    this.Ajuda = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.TipoCarga.Ajuda, visible: ko.observable(true) });
    this.NCM = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.NCM.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NBS = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.NBS.getFieldDescription(), maxlength: 9 });

    this.TipoCargaMDFe = PropertyEntity({ val: ko.observable(EnumTipoCargaMDFe.CargaGeral), options: EnumTipoCargaMDFe.obterOpcoes(), def: EnumTipoCargaMDFe.CargaGeral, text: Localization.Resources.Cargas.TipoCarga.TipoCargaMDFe.getFieldDescription() });
    this.IdentificacaoMercadoriaInfolog = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.TipoCarga.IdentificacaoDeMercadoriaInfolog.getFieldDescription()), maxlength: 50, visible: ko.observable(false) });
    this.ProdutoPredominante = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.ProdutoPredominante.getFieldDescription(), maxlength: 150, visible: ko.observable(true) });

    this.PossuiCargaPerigosa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.EsteTipoDeCargaPossuiProdutosPerigosos, enable: ko.observable(true), visible: ko.observable(true) });
    this.IndisponivelMontagemCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.EsteTipoDeCargaDeveFicarIndisponivelNaMontagemDeCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.BloquearLiberacaoParaTransportadores = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.EsteTipoDeCargaNaoDeveSerLiberadoParaTransportadores, enable: ko.observable(true), visible: ko.observable(true) });
    this.NaoPermitirFornecedorEscolherNoAgendamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.NaoPermitirQueFornecedorEscolhaEsseTipoDeOperacaoNoAgendamento, enable: ko.observable(true), visible: ko.observable(true) });
    this.NaoValidarDataCheckList = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.NaoValidarDataDeChecklistParaEsseTipoDeCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.Paletizado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.EsteTipoDeCargaPaletizado });
    this.BloquearMontagemCargaComPedidoProvisorio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.BloquearAgendamentoCargaComPedidoProvisorio });
    this.ValidarLicencasNCM = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.TipoCarga.ValidarLicencasNCM, enable: ko.observable(true), visible: ko.observable(visibilidadeGrupo) });
    this.ClasseONU = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.ClasseONU.getRequiredFieldDescription(), maxlength: 100, visible: ko.observable(false), required: ko.observable(false) });
    this.SequenciaONU = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.SequenciaONU.getRequiredFieldDescription(), maxlength: 100, visible: ko.observable(false), required: ko.observable(false) });
    this.CodigoPsnONU = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.CodigoPSNONU.getRequiredFieldDescription(), maxlength: 100, visible: ko.observable(false), required: ko.observable(false) });
    this.ObservacaoONU = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.ObservacaoONU.getRequiredFieldDescription(), maxlength: 500, visible: ko.observable(false), required: ko.observable(false) });

    this.Principal = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Cargas.TipoCarga.EsteTipoPrincipal, visible: ko.observable(true) });
    this.TipoCargaPrincipal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.TipoDeCargaPrincipal.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });

    //Listas
    this.ModelosVeicularesCargas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.ListaFaixasTempoDescargaPorPeso = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });
    this.ListaTiposLicenca = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });

    this.ValidarLicencasNCM.val.subscribe((a) => {
        if (a == 1)
            $("#liTiposLicenca").show();
        else
            $("#liTiposLicenca").hide();
    })

    this.Principal.val.subscribe(function (novoValor) {
        _tipoCarga.TipoCargaPrincipal.visible(!novoValor);
        _tipoCarga.TipoCargaPrincipal.required(!novoValor);
        if (novoValor) {
            _tipoCarga.TipoCargaPrincipal.codEntity(0);
            _tipoCarga.TipoCargaPrincipal.val("");
        }
    });

    this.GrupoPessoas.codEntity.subscribe(function (novoValor) {
        changeGrupoPessoas(novoValor);
    });

    this.Pessoa.codEntity.subscribe(function (novoValor) {
        changePessoa();
    });

    this.PossuiCargaPerigosa.val.subscribe(function (novoValor) {
        if (!novoValor) {
            limparCamposCargaPerigosa();
        } else if (novoValor) {
            alterarVisibleRequiredCamposCargaPerigosa(true);
        }
    });

    this.CodigoNaturezaCIOT = PropertyEntity({ text: "Código Natureza CIOT:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false) });
    this.PrioridadeCarga = PropertyEntity({ text: "Prioridade da Carga:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false) });
    this.ImprimirTabelaTemperaturaNoVersoCTe = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Imprimir tabela de temperaturas no verso do CTe" });
};

var CRUDTipoCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoCarga() {
    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, "knockoutCadastroTipoCarga", null, true);

    _pesquisaTipoCarga = new PesquisaTipoCarga();
    KoBindings(_pesquisaTipoCarga, "knockoutPesquisaTipoCarga", false, _pesquisaTipoCarga.Pesquisar.id);

    HeaderAuditoria("TipoDeCarga", _tipoCarga);

    _crudTipoCarga = new CRUDTipoCarga();
    KoBindings(_crudTipoCarga, "knoutCRUDTipoCarga");
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _tipoCarga.GrupoPessoas.visible(false);
        _tipoCarga.Pessoa.visible(false);
        _pesquisaTipoCarga.BuscaAvancada.visible(false);
        _tipoCarga.CodigoNaturezaCIOT.visible(true);
        _tipoCarga.PrioridadeCarga.visible(_CONFIGURACAO_TMS.Carga.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE)
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _tipoCarga.Principal.visible(false);
        new BuscarClientes(_pesquisaTipoCarga.Pessoa, null, true);
        new BuscarGruposPessoas(_pesquisaTipoCarga.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        new BuscarClientes(_tipoCarga.Pessoa, null, true);
        new BuscarGruposPessoas(_tipoCarga.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    }

    new BuscarNCMS(_tipoCarga.NCM, RetornoConsultaNCM);
    new BuscarTiposdeCarga(_tipoCarga.TipoCargaPrincipal, null, null, null, null, null, null, null, true);

    buscarTiposCargas();

    loadTipoModeloVeicularCarga();
    loadOpenTech();
    loadKrona();
    LoadTempoDescargaPorFaixaPeso();
    loadIntegracao();
    loadTiposLicenca();
    LoadEFrete();
    carregarCodigosIntegracaoTipoCarga();

    buscarFaixaTemperatura();
    configurarTiposIntegracao();

    $("#" + _tipoCarga.GrupoPessoas.id).focusout(function () {
        focusOutGrupoPessoas();
    });

    $("#" + _tipoCarga.Pessoa.id).focusout(function () {
        focusOutPessoa();
    });
}

function buscarFaixaTemperatura() {
    executarReST("FaixaTemperatura/BuscarTodas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                _tipoCarga.FaixaTemperatura.options(arg.Data.FaixasTemperatura);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function RetornoConsultaNCM(data) {
    _tipoCarga.NCM.val(data.Descricao);
    _tipoCarga.NCM.codEntity(data.Descricao);
}

function changeGrupoPessoas() {
    if (Globalize.parseInt(_tipoCarga.GrupoPessoas.codEntity().toString()) > 0) {
        $("#" + _tipoCarga.Pessoa.id).prop("disabled", true);
        $("#" + _tipoCarga.Pessoa.idBtnSearch).prop("disabled", true);
    } else {
        $("#" + _tipoCarga.Pessoa.id).prop("disabled", false);
        $("#" + _tipoCarga.Pessoa.idBtnSearch).prop("disabled", false);
    }
}

function focusOutGrupoPessoas() {
    if (_tipoCarga.GrupoPessoas.val().trim() == "")
        _tipoCarga.GrupoPessoas.codEntity(0);
}

function changePessoa() {
    if (Globalize.parseInt(_tipoCarga.Pessoa.codEntity().toString()) > 0) {
        $("#" + _tipoCarga.GrupoPessoas.id).prop("disabled", true);
        $("#" + _tipoCarga.GrupoPessoas.idBtnSearch).prop("disabled", true);
    } else {
        $("#" + _tipoCarga.GrupoPessoas.id).prop("disabled", false);
        $("#" + _tipoCarga.GrupoPessoas.idBtnSearch).prop("disabled", false);
    }
}

function focusOutPessoa() {
    if (_tipoCarga.Pessoa.val().trim() == "")
        _tipoCarga.Pessoa.codEntity(0);
}

function alterarVisibleRequiredCamposCargaPerigosa(val) {
    _tipoCarga.ClasseONU.visible(val);
    _tipoCarga.ClasseONU.required(val);
    _tipoCarga.SequenciaONU.visible(val);
    _tipoCarga.SequenciaONU.required(val);
    _tipoCarga.CodigoPsnONU.visible(val);
    _tipoCarga.CodigoPsnONU.required(val);
    _tipoCarga.ObservacaoONU.visible(val);
    _tipoCarga.ObservacaoONU.required(val);
}

function limparCamposCargaPerigosa() {
    _tipoCarga.ClasseONU.val("");
    _tipoCarga.SequenciaONU.val("");
    _tipoCarga.CodigoPsnONU.val("");
    _tipoCarga.ObservacaoONU.val("");
    alterarVisibleRequiredCamposCargaPerigosa(false);
}

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_tipoCarga))
        return exibirCamposObrigatorio();

    executarReST("TipoCarga/Adicionar", obterTipoCargaSalvar(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridTipoCarga.CarregarGrid();
                limparCamposTipoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_tipoCarga))
        return exibirCamposObrigatorio();

    executarReST("TipoCarga/Atualizar", obterTipoCargaSalvar(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridTipoCarga.CarregarGrid();
                limparCamposTipoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.TipoCarga.RealmenteDesejaExcluirTipoDeCarga + _tipoCarga.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoCarga, "TipoCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridTipoCarga.CarregarGrid();
                    limparCamposTipoCarga();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick() {
    limparCamposTipoCarga();
}

//*******MÉTODOS*******

function obterTipoCargaSalvar() {
    reordenarPosicoesModelos();
    preencherListaFaixasTempoDescargaPorPesoParaBackEnd();
    preencherTiposLicencaParaBackEnd();

    var tipoCarga = RetornarObjetoPesquisa(_tipoCarga);

    preencherIntegracaoSalvar(tipoCarga);

    return tipoCarga;
}

function editarTipoCarga(tipoCargaGrid) {
    limparCamposTipoCarga();
    _tipoCarga.Codigo.val(tipoCargaGrid.Codigo);

    BuscarPorCodigo(_tipoCarga, "TipoCarga/BuscarPorCodigo", function (arg) {
        _pesquisaTipoCarga.ExibirFiltros.visibleFade(false);
        _crudTipoCarga.Atualizar.visible(true);
        _crudTipoCarga.Cancelar.visible(true);
        _crudTipoCarga.Excluir.visible(true);
        _crudTipoCarga.Adicionar.visible(false);

        reordenarPosicoesModelos();
        recarregarGridReorder();
        RecarregarListaFaixasPeso();
        preencherIntegracao(arg.Data.Integracoes);
        recarregarGridTiposLicenca();
        preencherGridCodigosIntegracaoTipoCarga(arg.Data.CodigosIntegracao);
    }, null);
}

function buscarTiposCargas() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarTipoCarga, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoCarga = new GridView(_pesquisaTipoCarga.Pesquisar.idGrid, "TipoCarga/Pesquisa", _pesquisaTipoCarga, menuOpcoes, null);

    _gridTipoCarga.CarregarGrid();
}

function limparCamposTipoCarga() {
    _crudTipoCarga.Atualizar.visible(false);
    _crudTipoCarga.Cancelar.visible(false);
    _crudTipoCarga.Excluir.visible(false);
    _crudTipoCarga.Adicionar.visible(true);

    LimparCampos(_tipoCarga);
    limparCamposOpenTech();
    limparCamposIntegracao();
    limparCamposTiposLicenca();
    limparCamposCodigosIntegracaoTipoCarga();

    _tipoCarga.ModelosVeicularesCargas.list = new Array();
    _gridReorder.LimparGrid();

    Global.ResetarAbas();
}

function exibirCamposObrigatorio() {
    Global.ResetarAbas();
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function configurarTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", { Tipos: JSON.stringify([EnumTipoIntegracao.OpenTech, EnumTipoIntegracao.Krona, EnumTipoIntegracao.Infolog, EnumTipoIntegracao.Buonny, EnumTipoIntegracao.Intercab, EnumTipoIntegracao.CIOT]) }, function (r) {
        if (r.Success) {
            if (r.Data) {

                for (var i = 0; i < r.Data.length; i++) {
                    switch (r.Data[i].Codigo) {
                        case EnumTipoIntegracao.OpenTech:
                            obterTipoSensorOpentech();
                            $("#liOpenTech").show();
                            break;

                        case EnumTipoIntegracao.Krona:
                            $("#liKrona").show();
                            break;

                        case EnumTipoIntegracao.Infolog:
                            _tipoCarga.IdentificacaoMercadoriaInfolog.visible(true);
                            _tipoCarga.IdentificacaoMercadoriaInfolog.text(Localization.Resources.Cargas.TipoCarga.IdentificacaoDeMercadoriaInfolog.getFieldDescription())
                            break;

                        case EnumTipoIntegracao.Buonny:
                            _tipoCarga.IdentificacaoMercadoriaInfolog.visible(true);
                            _tipoCarga.IdentificacaoMercadoriaInfolog.text(Localization.Resources.Cargas.TipoCarga.IdentificacaoDeProdutoBuonny.getFieldDescription())
                            break;

                        case EnumTipoIntegracao.Intercab:
                            if (r.Data[i].DefinirModalPeloTipoCarga)
                                _tipoCarga.ModalProposta.visible(false)
                            break;

                        case EnumTipoIntegracao.CIOT:
                            $("#liEFrete").show();
                            break;
                    }
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);

        p.done();
    });

    return p;
}