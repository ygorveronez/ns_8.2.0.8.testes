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
/// <reference path="../../Enumeradores/EnumTipoContatoAtendimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProspeccao.js" />
/// <reference path="../../Enumeradores/EnumNivelSatisfacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _prospeccao;
var _pesquisaProspeccao;
var _gridProspeccao;

var KEY_TAB = 9;

var _tipoContato = [
    { text: "Telefone", value: EnumTipoContatoAtendimento.Telefone },
    { text: "Email", value: EnumTipoContatoAtendimento.Email },
    { text: "Skype", value: EnumTipoContatoAtendimento.Skype },
    { text: "Chat Web", value: EnumTipoContatoAtendimento.ChatWeb }
];

var _satisfacao = [
    { text: "Não Avaliado", value: EnumNivelSatisfacao.NaoAvaliado },
    { text: "Ótimo", value: EnumNivelSatisfacao.Otimo },
    { text: "Bom", value: EnumNivelSatisfacao.Bom },
    { text: "Ruim", value: EnumNivelSatisfacao.Ruim }
];
var _faturado = [
    { text: "Sim", value: 1 },
    { text: "Não", value: 0 }
];

var Prospeccao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataLancamento = PropertyEntity({ text: "*Data Lançamento: ", required: true, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), getType: typesKnockout.dateTime, enable: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: "Usuário: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: false });
    this.Produto = PropertyEntity({ text: "*Produto: ", required: true, val: ko.observable(""), codEntity: ko.observable(0), idBtnSearch: guid(), type: types.entity, enable: ko.observable(true) });

    this.Nome = PropertyEntity({ text: "*Cliente: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ idBtnSearch: guid(), type: types.entity, val: ko.observable(""), codEntity: ko.observable(0) });
    this.CNPJ = PropertyEntity({ text: "CNPJ: ", val: ko.observable(""), def: "", getType: typesKnockout.cnpj, enable: ko.observable(true), required: function () { return _prospeccao.Situacao.val() == EnumSituacaoProspeccao.Vendido; } });
    this.Contato = PropertyEntity({ text: "*Contato: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: "*Email: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.email, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: "*Telefone: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Cidade = PropertyEntity({ text: "Cidade: ", val: ko.observable(""), codEntity: ko.observable(0), idBtnSearch: guid(), type: types.entity, enable: ko.observable(true) });
    this.TipoContato = PropertyEntity({ text: "Tipo do Contato: ", val: ko.observable(0), def: EnumTipoContatoAtendimento.Telefone, options: _tipoContato, enable: ko.observable(true) });
    this.OrigemContato = PropertyEntity({ text: "Origem Contato: ", val: ko.observable(""), codEntity: ko.observable(0), idBtnSearch: guid(), type: types.entity, enable: ko.observable(true) });

    this.Valor = PropertyEntity({ text: "Valor: ", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoProspeccao.Pendente), def: EnumSituacaoProspeccao.Pendente, options: EnumSituacaoProspeccao.obterOpcoes(), enable: ko.observable(true) });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(EnumSituacaoProspeccao.Pendente), def: EnumSituacaoProspeccao.Pendente });

    this.Satisfacao = PropertyEntity({ text: "Satisfação: ", val: ko.observable(0), def: EnumNivelSatisfacao.NaoAvaliado, options: _satisfacao, enable: ko.observable(true) });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno: ", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, enable: ko.observable(true) });
    this.Faturado = PropertyEntity({ text: "Faturado: ", val: ko.observable(0), def: 0, options: _faturado, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true), maxlength: 2000 });

    this.Anexo = PropertyEntity({ text: "Anexo", val: ko.observable(""), def: "", eventClick: gerenciarAnexosClick });
    this.Historico = PropertyEntity({ text: "Histórico Prospecção", idGrid: guid(), visible: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProspeccao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ text: "Nome cliente:", val: ko.observable(""), def: "" });
    this.DataLancamento = PropertyEntity({ text: "Data Lançamento: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoProspeccao.Todos), options: EnumSituacaoProspeccao.obterOpcoesPesquisa(), def: EnumSituacaoProspeccao.Todos, text: "Situação: " });
    this.Usuario = PropertyEntity({ text: "Usuário: ", val: ko.observable(""), codEntity: ko.observable(0), idBtnSearch: guid(), type: types.entity, visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProspeccao.CarregarGrid();
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
}

//*******EVENTOS*******
function loadProspeccao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProspeccao = new PesquisaProspeccao();
    KoBindings(_pesquisaProspeccao, "knockoutPesquisaProspeccao", false, _pesquisaProspeccao.Pesquisar.id);

    // Instancia objeto principal
    _prospeccao = new Prospeccao();
    KoBindings(_prospeccao, "knockoutProspeccao");

    // Instancia buscas
    //new BuscarServicoTMS(_prospeccao.Servico);
    new BuscarProdutoProspect(_prospeccao.Produto);
    new BuscarLocalidades(_prospeccao.Cidade);
    new BuscarClienteProspect(_prospeccao.Cliente, PreencheCliente);
    new BuscarOrigemContatoClienteProspect(_prospeccao.OrigemContato);
    new BuscarFuncionario(_pesquisaProspeccao.Usuario);

    loadAnexos();
    loadHistorico();

    // Inicia busca
    BuscarProspeccao();
    CarregaUsuarioLogado();

    _prospeccao.Nome.get$().on('change', function () {
        BuscarClienteProspectPorNome();
    });
}

function adicionarClick(e, sender) {
    Salvar(_prospeccao, "Prospeccao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProspeccao.CarregarGrid();

                _prospeccao.Codigo.val(arg.Data);
                EnviarArquivosAnexados(function () {
                    LimparCamposProspeccao(false);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_prospeccao, "Prospeccao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProspeccao.CarregarGrid();
                LimparCamposProspeccao(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    LimparCamposProspeccao(false);
}

function editarProspeccaoClick(itemGrid) {
    // Limpa os campos
    LimparCamposProspeccao(true);

    // Seta o codigo do objeto
    _prospeccao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_prospeccao, "Prospeccao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProspeccao.ExibirFiltros.visibleFade(false);

                // -- Load
                EditarProspeccao();

                // -- Anexos
                CarregarAnexos(arg.Data);

                // Alternas os campos de CRUD
                _prospeccao.Atualizar.visible(true);
                _prospeccao.Cancelar.visible(true);
                _prospeccao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarProspeccao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProspeccaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProspeccao = new GridView(_pesquisaProspeccao.Pesquisar.idGrid, "Prospeccao/Pesquisa", _pesquisaProspeccao, menuOpcoes, null);
    _gridProspeccao.CarregarGrid();
}

function LimparCamposProspeccao(alterando) {
    _prospeccao.Atualizar.visible(false);
    _prospeccao.Cancelar.visible(false);
    _prospeccao.Adicionar.visible(true);
    LimparCampos(_prospeccao);
    limparAnexos();
    GridHistorico();
    if (alterando == false)
        CarregaUsuarioLogado();
    ControleCampos(true);
}

function CarregaUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _prospeccao.Usuario.val(arg.Data.Nome);

                if (arg.Data.UsuarioAdministrador)
                    _pesquisaProspeccao.Usuario.visible(true);
            }
        }
    });
}

function PreencheCliente(data) {
    _prospeccao.Cliente.val(data.Descricao)
    _prospeccao.Cliente.codEntity(parseInt(data.Codigo));
    _prospeccao.Nome.val(data.Descricao);
    _prospeccao.CNPJ.val(data.CNPJ);
    _prospeccao.Contato.val(data.Contato);
    _prospeccao.Email.val(data.Email);
    _prospeccao.Telefone.val(data.Telefone);
    _prospeccao.Cidade.val(data.Cidade);
    _prospeccao.Cidade.codEntity(data.CodigoCidade);
}

function BuscarClienteProspectPorNome() {
    var nome = _prospeccao.Nome.val();

    if (nome == "")
        return _prospeccao.Cliente.codEntity(0);

    executarReST("ClienteProspect/Pesquisa", { Descricao: nome, Grid: "{}" }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                if (arg.Data.recordsTotal == 1)
                    PreencheCliente(arg.Data.data[0]);
                else {
                    _prospeccao.Cliente.codEntity(0);
                    exibirMensagem(tipoMensagem.aviso, "Cliente", "Não foi encontrado nenhum cliente já cadastrado.");
                }
            }
        }
    });
}

function ControleCampos(enable) {
    _prospeccao.DataLancamento.enable(enable);
    _prospeccao.Produto.enable(enable);
    _prospeccao.Nome.enable(enable);
    _prospeccao.CNPJ.enable(enable);
    _prospeccao.Contato.enable(enable);
    _prospeccao.Email.enable(enable);
    _prospeccao.Telefone.enable(enable);
    _prospeccao.Cidade.enable(enable);
    _prospeccao.TipoContato.enable(enable);
    _prospeccao.OrigemContato.enable(enable);
    _prospeccao.Valor.enable(enable);
    _prospeccao.Situacao.enable(enable);
    _prospeccao.Satisfacao.enable(enable);
    _prospeccao.DataRetorno.enable(enable);
    _prospeccao.Observacao.enable(enable);

    if (_prospeccao.DataLancamento.val() != null && _prospeccao.DataLancamento.val() != "")
        _prospeccao.DataLancamento.enable(false);

    if (_prospeccao.DataRetorno.val() != null && _prospeccao.DataRetorno.val() != "")
        _prospeccao.DataRetorno.enable(false);
}

function EditarProspeccao() {
    var liberado = _prospeccao.EnumSituacao.val() == EnumSituacaoProspeccao.Pendente;

    ControleCampos(liberado);
}