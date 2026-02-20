/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCotacao;
var _cotacao;
var _pesquisaCotacao;

var PesquisaCotacao = function () {
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Todas), options: EnumMoedaCotacaoBancoCentral.obterOpcoesPesquisa(), def: EnumMoedaCotacaoBancoCentral.Todas, text: "Moeda Banco Central: " });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CotacaoAtiva = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação da Cotação: " });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Pessoa: ", eventChange: TipoPessoaPesquisaChange });
    this.DataVigencia = PropertyEntity({ text: "Data Vigência: ", getType: typesKnockout.dateTime, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCotacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Cotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.DolarVenda), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.DolarVenda, text: "*Moeda Banco Central: " });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "*Tipo de Pessoa: ", eventChange: TipoPessoaChange, issue: 306, enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CotacaoAtiva = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação da Cotação: " });
    this.CotacaoAutomaticaViaWS = PropertyEntity({ val: ko.observable(false), def: false, text: "Usar cotação do dia? " });
    this.UtilizarCotacaoRetroativa = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Usar cotação moeda retroativa? " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuario:", idBtnSearch: guid() });
    this.DataCotacao = PropertyEntity({ text: "Data Cotação: " });
    this.ValorMoeda = PropertyEntity({ text: "*Valor da Moeda:", configDecimal: { precision: 10, allowZero: false }, maxlength: 12, required: true, fadeVisible: ko.observable(true), type: types.map, getType: typesKnockout.decimal });

    this.DataVigenciaInicial = PropertyEntity({ text: "Data Vigência Inicial: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataVigenciaFinal = PropertyEntity({ text: "Data Vigência Final: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataVigenciaInicial.dateRangeLimit = this.DataVigenciaFinal;
    this.DataVigenciaFinal.dateRangeInit = this.DataVigenciaInicial;

    this.PesquisarCotacaoBancoCentral = PropertyEntity({ eventClick: pesquisarCotacaoBancoCentrarClick, type: types.event, text: "Buscar cotação da moeda no Banco Central", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadCotacao() {
    _cotacao = new Cotacao();
    KoBindings(_cotacao, "knockoutCadastroCotacao");

    HeaderAuditoria("Cotacao", _cotacao);

    _pesquisaCotacao = new PesquisaCotacao();
    KoBindings(_pesquisaCotacao, "knockoutPesquisaCotacao", false, _pesquisaCotacao.Pesquisar.id);

    new BuscarClientes(_cotacao.Cliente, null, true);
    new BuscarGruposPessoas(_cotacao.GrupoPessoa);

    new BuscarGruposPessoas(_pesquisaCotacao.GrupoPessoa);
    new BuscarClientes(_pesquisaCotacao.Cliente, null);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _cotacao.DataVigenciaInicial.val(moment().format("DD/MM/YYYY 00:00"));
        _cotacao.DataVigenciaFinal.val(moment().format("DD/MM/YYYY 23:59"));
    }

    buscarCotacaos();
}

function TipoPessoaPesquisaChange(e, sender) {
    if (_pesquisaCotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaCotacao.Cliente.visible(true);
        _pesquisaCotacao.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaCotacao.GrupoPessoa);
    } else if (_pesquisaCotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaCotacao.Cliente.visible(false);
        _pesquisaCotacao.GrupoPessoa.visible(true);
        LimparCampoEntity(_pesquisaCotacao.Cliente);
    }
}

function TipoPessoaChange(e, sender) {
    if (_cotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        _cotacao.Cliente.required = false;
        _cotacao.Cliente.visible(true);
        _cotacao.GrupoPessoa.required = false;
        _cotacao.GrupoPessoa.visible(false);
        LimparCampoEntity(_cotacao.GrupoPessoa);
    } else if (_cotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _cotacao.Cliente.required = false;
        _cotacao.Cliente.visible(false);
        _cotacao.GrupoPessoa.required = false;
        _cotacao.GrupoPessoa.visible(true);
        LimparCampoEntity(_cotacao.Cliente);
    }
}

function pesquisarCotacaoBancoCentrarClick(e, arg) {
    var data = { MoedaCotacaoBancoCentral: _cotacao.MoedaCotacaoBancoCentral.val() };
    executarReST("Cotacao/BuscarCotacaoMoedaBancoCentral", data, function (arg) {
        if (arg.Success) {
            _cotacao.ValorMoeda.val(Globalize.format(arg.Data, "n10"));
        } else {
            exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
        }
    });
}

function adicionarClick(e, sender) {
    if (e.CotacaoAutomaticaViaWS.val())
        e.ValorMoeda.required = false;
    else
        e.ValorMoeda.required = true;

    Salvar(e, "Cotacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCotacao.CarregarGrid();
                limparCamposCotacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (e.CotacaoAutomaticaViaWS.val())
        e.ValorMoeda.required = false;
    else
        e.ValorMoeda.required = true;

    Salvar(e, "Cotacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCotacao.CarregarGrid();
                limparCamposCotacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir essa cotação?", function () {
        ExcluirPorCodigo(_cotacao, "Cotacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCotacao.CarregarGrid();
                    limparCamposCotacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCotacao();
}

//*******MÉTODOS*******


function buscarCotacaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCotacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "Cotacao/ExportarPesquisa",
        titulo: "Cotações"
    };

    _gridCotacao = new GridViewExportacao(_pesquisaCotacao.Pesquisar.idGrid, "Cotacao/Pesquisa", _pesquisaCotacao, menuOpcoes, configExportacao);
    _gridCotacao.CarregarGrid();
}

function editarCotacao(cotacaoGrid) {
    limparCamposCotacao();
    _cotacao.Codigo.val(cotacaoGrid.Codigo);
    BuscarPorCodigo(_cotacao, "Cotacao/BuscarPorCodigo", function (arg) {
        _pesquisaCotacao.ExibirFiltros.visibleFade(false);
        _cotacao.Atualizar.visible(true);
        _cotacao.Cancelar.visible(true);
        _cotacao.Excluir.visible(true);
        _cotacao.Adicionar.visible(false);

        if (_cotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
            _cotacao.Cliente.required = false;
            _cotacao.Cliente.visible(true);
            _cotacao.GrupoPessoa.required = false;
            _cotacao.GrupoPessoa.visible(false);
            LimparCampoEntity(_cotacao.GrupoPessoa);
        } else if (_cotacao.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
            _cotacao.Cliente.required = false;
            _cotacao.Cliente.visible(false);
            _cotacao.GrupoPessoa.required = false;
            _cotacao.GrupoPessoa.visible(true);
            LimparCampoEntity(_cotacao.Cliente);
        }
    }, null);
}

function limparCamposCotacao() {
    _cotacao.Atualizar.visible(false);
    _cotacao.Cancelar.visible(false);
    _cotacao.Excluir.visible(false);
    _cotacao.Adicionar.visible(true);
    _cotacao.ValorMoeda.required = true;
    _cotacao.ValorMoeda.fadeVisible(true);
    LimparCampos(_cotacao);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _cotacao.DataVigenciaInicial.val(moment().format("DD/MM/YYYY 00:00"));
        _cotacao.DataVigenciaFinal.val(moment().format("DD/MM/YYYY 23:59"));
    }
}
