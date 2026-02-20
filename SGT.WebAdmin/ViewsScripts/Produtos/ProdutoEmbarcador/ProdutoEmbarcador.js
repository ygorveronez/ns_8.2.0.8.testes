/// <reference path="../../Consultas/ClassificacaoRiscoONU.js" />
/// <reference path="../../Consultas/LinhaSeparacao.js" />
/// <reference path="../../Consultas/TipoEmbalagem.js" />
/// <reference path="../../Consultas/MarcaProduto.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="ProdutoEmbarcadorFiliais.js" />
/// <reference path="ProdutoEmbarcadorOrganizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoEmbarcador;
var _pesquisaProdutoEmbarcador;
var _gridProdutoEmbarcador;
var _crudProdutoEmbarcador;

var PesquisaProdutoEmbarcador = function () {
    this.ProdutoCss = PropertyEntity({ type: types.local, val: ko.observable("") });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.GrupoDeProduto.getFieldDescription(), idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutoEmbarcador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FitroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ProdutoEmbarcador = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false), visible: true });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: ko.observable(Localization.Resources.Produtos.ProdutoEmbarcador.TipoDePessoa.getRequiredFieldDescription()), issue: 306, eventChange: TipoPessoaChange, required: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Pessoa.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(true), issue: 52, visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.GrupoPessoas.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), issue: 58, visible: ko.observable(false) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.GrupoDeProduto.getRequiredFieldDescription(), issue: 60, idBtnSearch: guid(), required: true });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, maxlength: 250 });
    this.UnidadeDeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.UnidadeDeMedida.getFieldDescription(), issue: 88, idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.SiglaUnidade = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.SiglaUnidadeDeMedida.getFieldDescription(), maxlength: 10 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), required: true, maxlength: 50, issue: 15 });
    this.CodigoDocumentacao = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDocumentacao.getFieldDescription(), required: false, maxlength: 50 });
    this.CodigoNCM = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoNCM.getFieldDescription(), required: false, maxlength: 8 });
    this.ObrigatorioGuiaTransporteAnimal = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.UnidadeDeMedida.getFieldDescription() });
    this.ObrigatorioNFProdutor = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.ObrigatorioGuiaTransporteAnimal });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ObrigatorioNFProdutor, maxlength: 1000 });

    this.QuantidadeCaixaPorCamadaPallet = PropertyEntity({ text: "Qtd caixa por camada no Pallet:", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, maxlength: 14, required: false });
    this.ConfiguracaoPalletizacao = PropertyEntity({ text: "Configuração Palletização:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), val: ko.observable(""), required: false });
    this.QuantidadeCaixaPorPallet = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.QtdCaixaPorPallet.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.CheckList = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.TipoDeCarga.getFieldDescription(), idBtnSearch: guid(), required: false, issue: 53, visible: ko.observable(true) });

    this.PesoUnitario = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.Peso.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.PesoLiquidoUnitario = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.PesoLiquido.getFieldDescription(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarPesoProdutoParaCalcularPesoCarga) });
    this.QtdPalet = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.QtdDePalet.getFieldDescription(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.QuantidadeCaixa = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.QtdPorCaixa.getFieldDescription(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.AlturaCM = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.AlturaMT.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.LarguraCM = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.LarguraMT.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.ComprimentoCM = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.ComprimentoMT.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.MetroCubito = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.MetroCubito.getFieldDescription(), required: false, visible: ko.observable(true) });

    this.ClassificacaoRiscoONU = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.ClassificacaoONU.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.LinhaSeparacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.LinhaDeSeparacao.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.TipoEmbalagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.TipoEmbalagem.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.MarcaProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.MarcaDoProduto.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true) });

    this.TemperaturaTransporte = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.TemperaturaDeTransporte.getFieldDescription(), issue: 587, required: false, maxlength: 50 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });

    this.ExibirExpedicaoEmTempoReal = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.ExibirExpedicaoTempoRealParaProduto, issue: 588, visible: ko.observable(false) });
    this.DescontarPesoProdutoCalculoFrete = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.DescontarPesoProdutoCalculoFrete, visible: ko.observable(false) });
    this.DescontarValorProdutoCalculoFrete = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.DescontarValorProdutoCalculoFrete, visible: ko.observable(false) });

    this.PossuiIntegracaoColetaMobile = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.PossuiIntegracaoColetaMobile, visible: ko.observable(false) });
    this.ObrigatorioInformarTemperatura = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.EObrigatorioInformarTemperatura, visible: ko.observable(false) });
    this.ExigeInformarImunos = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.EObrigatorioInformarQuantidadeImunos, visible: ko.observable(false) });
    this.ExigeInformarCaixas = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.ProdutoEmbarcador.EObrigatorioInformarQuantidadeCaixas, visible: ko.observable(false) });

    this.Lotes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Volumes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Clientes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Filiais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Organizacao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.FornecedorProduto = PropertyEntity({ required: false, idGrid: guid(), codEntity: ko.observable(0), val: ko.observable([]) });
    this.TabelaConversao = PropertyEntity({ required: false, idGrid: guid(), codEntity: ko.observable(0), val: ko.observable([]) });

    this.CodigoEAN = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoEAN.getFieldDescription(), required: false, maxlength: 15 });
    this.CodigocEAN = PropertyEntity({ text: Localization.Resources.Produtos.ProdutoEmbarcador.CodigocEAN.getFieldDescription(), required: false, maxlength: 15 });
    this.FatorConversao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 5, allowZero: true, allowNegative: false }, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.FatorConversao.getFieldDescription(), required: false, visible: ko.observable(true) });
};

var CRUDProdutoEmbarcador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ProdutoEmbarcador/Importar",
        UrlConfiguracao: "ProdutoEmbarcador/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.I001_ProdutoEmbarcador,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        },
        CallbackImportacao: function () {
            _gridProdutoEmbarcador.CarregarGrid();
        }
    });
}

//*******EVENTOS*******

function loadProdutoEmbarcador() {
    _produtoEmbarcador = new ProdutoEmbarcador();
    KoBindings(_produtoEmbarcador, "knockoutCadastroProdutoEmbarcador");

    _crudProdutoEmbarcador = new CRUDProdutoEmbarcador();
    KoBindings(_crudProdutoEmbarcador, "knockoutCRUDProdutoEmbarcador");

    HeaderAuditoria("ProdutoEmbarcador", _produtoEmbarcador);

    new BuscarClientes(_produtoEmbarcador.Pessoa);
    new BuscarGruposPessoas(_produtoEmbarcador.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposProdutos(_produtoEmbarcador.GrupoProduto);
    new BuscarTiposdeCarga(_produtoEmbarcador.TipoCarga, null, _produtoEmbarcador.GrupoPessoa, null, _produtoEmbarcador.Pessoa);
    new BuscarUnidadesMedida(_produtoEmbarcador.UnidadeDeMedida);
    new BuscarClassificacaoRiscoONU(_produtoEmbarcador.ClassificacaoRiscoONU);
    new BuscarLinhasSeparacao(_produtoEmbarcador.LinhaSeparacao);
    new BuscarTipoEmbalagem(_produtoEmbarcador.TipoEmbalagem);
    new BuscarMarcaProduto(_produtoEmbarcador.MarcaProduto);

    _pesquisaProdutoEmbarcador = new PesquisaProdutoEmbarcador();
    KoBindings(_pesquisaProdutoEmbarcador, "knockoutPesquisaProdutoEmbarcador", false, _pesquisaProdutoEmbarcador.Pesquisar.id);

    new BuscarClientes(_pesquisaProdutoEmbarcador.Pessoa);
    new BuscarGruposPessoas(_pesquisaProdutoEmbarcador.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposProdutos(_pesquisaProdutoEmbarcador.GrupoProduto);

    configurarLayoutProdutoEmbarcadorPorTipoSistema();

    buscarProdutosEmbarcador();
    loadLotesProdutoEmbarcador();
    loadVolumesProdutoEmbarcador();
    loadClientesProdutoEmbarcador();
    loadChecklist();
    loadFiliais();
    loadOrganizacao();
    LoadProdutosConversoes();
    LoadFornecedorProduto();
    LoadProdutosPalletizacao();
    resetarTabs();
}

function configurarLayoutProdutoEmbarcadorPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _produtoEmbarcador.ExibirExpedicaoEmTempoReal.visible(true);
        _produtoEmbarcador.DescontarPesoProdutoCalculoFrete.visible(true);
        _produtoEmbarcador.DescontarValorProdutoCalculoFrete.visible(true);
        _produtoEmbarcador.PossuiIntegracaoColetaMobile.visible(true);
    } else {
        $("#liTabChecklist").hide();
    }

    if (_CONFIGURACAO_TMS.ControlarOrganizacaoProdutos) {
        $("#liTabOrganizacao").show();
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
            $("#liTabFiliais").show();
        else
            $("#liTabFiliais").hide();
    } else {
        $("#liTabFiliais").hide();
        $("#liTabOrganizacao").hide();
    }


    if (_CONFIGURACAO_TMS.PessoasNaoObrigatorioProdutoEmbarcador) {
        _produtoEmbarcador.TipoPessoa.required(false);
        _produtoEmbarcador.Pessoa.required(false);
        _produtoEmbarcador.GrupoPessoa.required(false);
        _produtoEmbarcador.TipoPessoa.text(Localization.Resources.Produtos.ProdutoEmbarcador.TipoDePessoa.getFieldDescription());
        _produtoEmbarcador.Pessoa.text(Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription());
        _produtoEmbarcador.GrupoPessoa.text(Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription());
    }

    ControleCampos();
}

function calcularMetroCubico(e, sender) {
    if (Globalize.parseFloat(_produtoEmbarcador.AlturaCM.val()) > 0 && Globalize.parseFloat(_produtoEmbarcador.LarguraCM.val()) > 0 && Globalize.parseFloat(_produtoEmbarcador.ComprimentoCM.val()) > 0) {
        var volume = Globalize.parseFloat(_produtoEmbarcador.AlturaCM.val()) * Globalize.parseFloat(_produtoEmbarcador.LarguraCM.val()) * Globalize.parseFloat(_produtoEmbarcador.ComprimentoCM.val());
        _produtoEmbarcador.MetroCubito.val((volume.toFixed(3).replace(".", ",")));
    }
}

function TipoPessoaChange(e, sender) {
    if (_produtoEmbarcador.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || !_produtoEmbarcador.TipoPessoa.required())
            _produtoEmbarcador.Pessoa.required(false);
        else
            _produtoEmbarcador.Pessoa.required(true);
        _produtoEmbarcador.Pessoa.visible(true);
        _produtoEmbarcador.GrupoPessoa.required(false);
        _produtoEmbarcador.GrupoPessoa.visible(false);
        LimparCampoEntity(_produtoEmbarcador.GrupoPessoa);
    } else if (_produtoEmbarcador.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _produtoEmbarcador.Pessoa.required(false);
        _produtoEmbarcador.Pessoa.visible(false);
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || !_produtoEmbarcador.TipoPessoa.required())
            _produtoEmbarcador.GrupoPessoa.required(false);
        else
            _produtoEmbarcador.GrupoPessoa.required(true);
        _produtoEmbarcador.GrupoPessoa.visible(true);
        LimparCampoEntity(_produtoEmbarcador.Pessoa);
    }
    //LimparCampoEntity(_produtoEmbarcador.TipoCarga);
}

function adicionarClick(e, sender) {
    SalvarChecklist();
    ObterDadosDaGridConversaoFormatados();
    SetarProdutoFornecedor();
    SalvarConfiguracaoPalletizacao();
    Salvar(_produtoEmbarcador, "ProdutoEmbarcador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridProdutoEmbarcador.CarregarGrid();
                limparCamposProdutoEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    SalvarChecklist();
    ObterDadosDaGridConversaoFormatados();
    SetarProdutoFornecedor();
    SalvarConfiguracaoPalletizacao();
    Salvar(_produtoEmbarcador, "ProdutoEmbarcador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridProdutoEmbarcador.CarregarGrid();
                limparCamposProdutoEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirProduto + _produtoEmbarcador.Descricao.val(), function () {
        ExcluirPorCodigo(_produtoEmbarcador, "ProdutoEmbarcador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridProdutoEmbarcador.CarregarGrid();
                    limparCamposProdutoEmbarcador();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProdutoEmbarcador();
}

//*******MÉTODOS*******

function buscarProdutosEmbarcador() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarProdutoEmbarcador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ProdutoEmbarcador/ExportarPesquisa",
        titulo: "Produtos Embarcador"
    };

    _gridProdutoEmbarcador = new GridViewExportacao(_pesquisaProdutoEmbarcador.Pesquisar.idGrid, "ProdutoEmbarcador/Pesquisa", _pesquisaProdutoEmbarcador, menuOpcoes, configExportacao);
    _gridProdutoEmbarcador.CarregarGrid();
}

function editarProdutoEmbarcador(produtoEmbarcadorGrid) {
    limparCamposProdutoEmbarcador();
    _produtoEmbarcador.Codigo.val(produtoEmbarcadorGrid.Codigo);
    BuscarPorCodigo(_produtoEmbarcador, "ProdutoEmbarcador/BuscarPorCodigo", function (arg) {
        editarChecklist();
        resetarTabs();
        _pesquisaProdutoEmbarcador.ExibirFiltros.visibleFade(false);
        _crudProdutoEmbarcador.Atualizar.visible(true);
        _crudProdutoEmbarcador.Cancelar.visible(true);
        _crudProdutoEmbarcador.Excluir.visible(true);
        _crudProdutoEmbarcador.Adicionar.visible(false);
        TipoPessoaChange();
        ControleCampos();
        recarregarGridLotesProdutoEmbarcado();
        recarregarGridVolumesProdutoEmbarcado();
        recarregarGridClientesProdutoEmbarcado();
        recarregarGridFiliais();
        recarregarGridOrganizacao();
        RecarregarGridFornecedorProduto();
        LimparTodosOsCampos();
        EditarConfiguracaoPalletizacao();
        CarregarGridProdutoConversao(arg.Data.TabelaConversao);
    }, null);
}

function limparCamposProdutoEmbarcador() {
    _crudProdutoEmbarcador.Atualizar.visible(false);
    _crudProdutoEmbarcador.Cancelar.visible(false);
    _crudProdutoEmbarcador.Excluir.visible(false);
    _crudProdutoEmbarcador.Adicionar.visible(true);
    LimparCampos(_produtoEmbarcador);
    limparCamposChecklist();
    limparCamposFilial();
    limparCamposOrganizacao();
    _produtoEmbarcador.Pessoa.required(true);
    _produtoEmbarcador.Pessoa.visible(true);
    _produtoEmbarcador.GrupoPessoa.visible(false);
    _produtoEmbarcador.GrupoPessoa.required(false);
    _produtoEmbarcador.TipoPessoa.val(EnumTipoPessoaGrupo.Pessoa);
    recarregarGridLotesProdutoEmbarcado();
    recarregarGridVolumesProdutoEmbarcado();
    recarregarGridClientesProdutoEmbarcado();
    recarregarGridFiliais();
    recarregarGridOrganizacao();
    RecarregarGridFornecedorProduto();
    LimparTodosOsCamposFornecedor();
    LimparTodosOsCampos();
    ControleCampos();
    resetarTabs();
}

function ControleCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        // Cadastro
        _produtoEmbarcador.TipoPessoa.visible(false);
        _produtoEmbarcador.TipoPessoa.required(false);
        _produtoEmbarcador.Pessoa.visible(false);
        _produtoEmbarcador.Pessoa.required(false);
        _produtoEmbarcador.GrupoPessoa.visible(false);
        _produtoEmbarcador.GrupoPessoa.required(false);
        _produtoEmbarcador.TipoCarga.visible(false);
        _produtoEmbarcador.TipoCarga.required = false;

        // Pesquisa
        _pesquisaProdutoEmbarcador.Pessoa.visible(false);
        _pesquisaProdutoEmbarcador.GrupoPessoa.visible(false);
        _pesquisaProdutoEmbarcador.ProdutoCss.val("col col-xs-12 col-sm-12 col-md-12 col-lg-3");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _produtoEmbarcador.Pessoa.required(false);
    }
    else {
        _pesquisaProdutoEmbarcador.ProdutoCss.val("col col-xs-12 col-sm-12 col-md-12 col-lg-4");
    }
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function resetarTabs() {
    Global.ResetarAba("myPanelContent");
}