/// <reference path="../../Enumeradores/EnumTipoPalletizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPalletizacao, _palletizacao, _pesquisaPalletizacao, _crudPalletizacao;

var PesquisaPalletizacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo","Inatvo"), def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPalletizacao.CarregarGrid();
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

var Palletizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", required: false });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inatvo"), def: true, text: "*Situação: " });
    this.TipoPessoa = PropertyEntity({text: "Tipo Pessoa:", val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, enable: ko.observable(false)});
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false)});
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.TipoPalletizacao = PropertyEntity({ text: "*Tipo Palletização:", options: EnumTipoPalletizacao.obterOpcoes(), val: ko.observable(EnumTipoPalletizacao.Pallet), def: EnumTipoPalletizacao.Pallet, visible: ko.observable(true) });
    this.Altura = PropertyEntity({ text: "Altura(MT):", required: false, getType: typesKnockout.decimal});
    this.Largura = PropertyEntity({ text: "Largura(MT):", required: false, getType: typesKnockout.decimal});
    this.Comprimento = PropertyEntity({ text: "Comprimento(MT):", required: false, getType: typesKnockout.decimal });
    this.PalletMisto = PropertyEntity({ text: "Pallet Misto", required: false, getType: typesKnockout.bool });



    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _palletizacao.GrupoPessoas.visible(false);
            _palletizacao.GrupoPessoas.required(false);
            _palletizacao.Pessoa.visible(true);
            _palletizacao.Pessoa.required(true);
        } else {
            _palletizacao.Pessoa.visible(false);
            _palletizacao.GrupoPessoas.visible(true);
            _palletizacao.GrupoPessoas.required(true);
            _palletizacao.Pessoa.required(false);
        }
    });
}

var CRUDPalletizacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadPalletizacao() {

    _palletizacao = new Palletizacao();
    KoBindings(_palletizacao, "knockoutCadastroConfiguracoesPalletizacao");

    _crudPalletizacao = new CRUDPalletizacao();
    KoBindings(_crudPalletizacao, "knockoutCRUDConfiguracoesDePalletizacao");

    _pesquisaPalletizacao = new PesquisaPalletizacao();
    KoBindings(_pesquisaPalletizacao, "knockoutPesquisaConfiguracoesDePalletizacao", false, _pesquisaPalletizacao.Pesquisar.id);

    new BuscarGruposPessoas(_palletizacao.GrupoPessoas);
    new BuscarClientes(_palletizacao.Pessoa);


    HeaderAuditoria("Palletizacao", _palletizacao, "Codigo");

    buscarPalletizacao();
}

function adicionarClick(e, sender) {
    Salvar(_palletizacao, "Palletizacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPalletizacao.CarregarGrid();
                limparCamposPalletizacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_palletizacao, "Palletizacao/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _gridPalletizacao.CarregarGrid();
            limparCamposPalletizacao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Palletizacao " + _palletizacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_palletizacao, "Palletizacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPalletizacao.CarregarGrid();
                    limparCamposPalletizacao();
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
    limparCamposPalletizacao();
}

//*******MÉTODOS*******

function buscarPalletizacao() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarPalletizacao, tamanho: "9", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPalletizacao = new GridView(_pesquisaPalletizacao.Pesquisar.idGrid, "Palletizacao/Pesquisa", _pesquisaPalletizacao, menuOpcoes, null);
    _gridPalletizacao.CarregarGrid();
}

function editarPalletizacao(arquivoGrid) {
    limparCamposPalletizacao();
    _palletizacao.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_palletizacao, "Palletizacao/BuscarPorCodigo", function (arg) {
        _pesquisaPalletizacao.ExibirFiltros.visibleFade(false);
        _crudPalletizacao.Atualizar.visible(true);
        _crudPalletizacao.Cancelar.visible(true);
        _crudPalletizacao.Excluir.visible(true);
        _crudPalletizacao.Adicionar.visible(false);
        
    }, null);
}

function limparCamposPalletizacao() {
    _crudPalletizacao.Atualizar.visible(false);
    _crudPalletizacao.Cancelar.visible(false);
    _crudPalletizacao.Excluir.visible(false);
    _crudPalletizacao.Adicionar.visible(true);

    LimparCampos(_palletizacao);
}