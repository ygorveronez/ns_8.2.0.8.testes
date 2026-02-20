/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/UnidadeMedida.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoLicitacao.js" />
/// <reference path="../../Enumeradores/EnumPessoaLocalidade.js" />
/// <reference path="SolicitacaoLicitacaoEtapa.js" />
/// <reference path="SolicitacaoLicitacaoCotacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSolicitacaoLicitacao;
var _solicitacaoLicitacao;
var _CRUDSolicitacaoLicitacao;
var _pesquisaSolicitacaoLicitacao;
var _gridProduto;

var PesquisaSolicitacaoLicitacao = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.FuncionarioSolicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Solicitante:", idBtnSearch: guid() });
    this.FuncionarioCotacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Cotação:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSolicitacaoLicitacao.Todos), options: EnumSituacaoSolicitacaoLicitacao.obterOpcoesPesquisa(), def: EnumSituacaoSolicitacaoLicitacao.Todos, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSolicitacaoLicitacao.CarregarGrid();
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

var SolicitacaoLicitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UsuarioLogadoCriouACotacao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.TipoOrigem = PropertyEntity({ val: ko.observable(EnumPessoaLocalidade.Pessoa), options: EnumPessoaLocalidade.obterOpcoes(), def: EnumPessoaLocalidade.Pessoa, text: "Tipo Origem: ", enable: ko.observable(true) });
    this.TipoDestino = PropertyEntity({ val: ko.observable(EnumPessoaLocalidade.Pessoa), options: EnumPessoaLocalidade.obterOpcoes(), def: EnumPessoaLocalidade.Pessoa, text: "Tipo Destino: ", enable: ko.observable(true) });
    this.DescricaoCotacao = PropertyEntity({ text: "Descrição Cotação:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });

    this.ClienteOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade Origem:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.ClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente Destino:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade Destino:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.UnidadeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Unidade de Medida:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.EnderecoOrigem = PropertyEntity({ text: "Endereço Origem: ", val: ko.observable(""), visible: this.ClienteOrigem.visible, getType: typesKnockout.string });
    this.EnderecoDestino = PropertyEntity({ text: "Endereço Destino: ", val: ko.observable(""), visible: this.ClienteDestino.visible, getType: typesKnockout.string });

    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo De Carga:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número:", visible: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "Usuário Solicitante:", visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "*Quantidade/Volume:", getType: typesKnockout.decimal, maxlength: 18, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Comprimento = PropertyEntity({ text: "Comprimento:", getType: typesKnockout.decimal, val: ko.observable(0), enable: ko.observable(true) })
    this.Altura = PropertyEntity({ text: "Altura:", getType: typesKnockout.decimal, val: ko.observable(0), enable: ko.observable(true) })
    this.Largura = PropertyEntity({ text: "Largura:", getType: typesKnockout.decimal, val: ko.observable(0), enable: ko.observable(true) })

    this.DataInicioEmbarque = PropertyEntity({ text: "*Data Início Embarque: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataFimEmbarque = PropertyEntity({ text: "*Data Fim Embarque: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataPrazoResposta = PropertyEntity({ text: "*Data Prazo Resposta: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });

    this.DataInicioEmbarque.dateRangeLimit = this.DataFimEmbarque;
    this.DataFimEmbarque.dateRangeInit = this.DataInicioEmbarque;

    this.GridProdutos = PropertyEntity({ type: types.local });
    this.Produto = PropertyEntity({ type: types.event, text: "Adicionar Produto", idBtnSearch: guid(), enable: ko.observable(true) });

    this.TipoOrigem.val.subscribe(function (valor) {
        _solicitacaoLicitacao.ClienteOrigem.visible(false);
        _solicitacaoLicitacao.ClienteOrigem.required(false);
        _solicitacaoLicitacao.LocalidadeOrigem.visible(false);
        _solicitacaoLicitacao.LocalidadeOrigem.required(false);

        if (valor === EnumPessoaLocalidade.Localidade) {
            _solicitacaoLicitacao.LocalidadeOrigem.visible(true);
            _solicitacaoLicitacao.LocalidadeOrigem.required(true);
            LimparCampoEntity(_solicitacaoLicitacao.ClienteOrigem);
        } else {
            _solicitacaoLicitacao.ClienteOrigem.visible(true);
            _solicitacaoLicitacao.ClienteOrigem.required(true);
            LimparCampoEntity(_solicitacaoLicitacao.LocalidadeOrigem);
        }
    });

    this.ClienteOrigem.codEntity.subscribe(function (valor) {
        if (valor == 0)
            _solicitacaoLicitacao.EnderecoOrigem.val("");
    });

    this.ClienteDestino.codEntity.subscribe(function (valor) {
        if (valor == 0)
            _solicitacaoLicitacao.EnderecoDestino.val("");
    });

    this.TipoDestino.val.subscribe(function (valor) {
        _solicitacaoLicitacao.ClienteDestino.visible(false);
        _solicitacaoLicitacao.ClienteDestino.required(false);
        _solicitacaoLicitacao.LocalidadeDestino.visible(false);
        _solicitacaoLicitacao.LocalidadeDestino.required(false);

        if (valor === EnumPessoaLocalidade.Localidade) {
            _solicitacaoLicitacao.LocalidadeDestino.visible(true);
            _solicitacaoLicitacao.LocalidadeDestino.required(true);
            LimparCampoEntity(_solicitacaoLicitacao.ClienteDestino);
        } else {
            _solicitacaoLicitacao.ClienteDestino.visible(true);
            _solicitacaoLicitacao.ClienteDestino.required(true);
            LimparCampoEntity(_solicitacaoLicitacao.LocalidadeDestino);
        }
    });
};

var CRUDSolicitacaoLicitacao = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCamposSolicitacaoLicitacaoClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Solicitar = PropertyEntity({ eventClick: solicitarLicitacaoClick, type: types.event, text: "Solicitar Cotação", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarSolicitacaoLicitacaoClick, type: types.event, text: "Finalizar Cotação", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarSolicitacaoLicitacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarSolicitacaoLicitacaoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadSolicitacaoLicitacao() {
    _solicitacaoLicitacao = new SolicitacaoLicitacao();
    KoBindings(_solicitacaoLicitacao, "knockoutSolicitacao");

    HeaderAuditoria("SolicitacaoLicitacao", _solicitacaoLicitacao);

    _CRUDSolicitacaoLicitacao = new CRUDSolicitacaoLicitacao();
    KoBindings(_CRUDSolicitacaoLicitacao, "knockoutCRUDSolicitacaoLicitacao");

    _pesquisaSolicitacaoLicitacao = new PesquisaSolicitacaoLicitacao();
    KoBindings(_pesquisaSolicitacaoLicitacao, "knockoutPesquisaSolicitacaoLicitacao", false, _pesquisaSolicitacaoLicitacao.Pesquisar.id);

    loadSolicitacaoLicitacaoCotacao();
    LoadEtapasSolicitacaoLicitacao();

    new BuscarTransportadores(_pesquisaSolicitacaoLicitacao.Empresa);
    new BuscarFuncionario(_pesquisaSolicitacaoLicitacao.FuncionarioSolicitante);
    new BuscarFuncionario(_pesquisaSolicitacaoLicitacao.FuncionarioCotacao);
    new BuscarClientes(_solicitacaoLicitacao.ClienteOrigem, retornoClienteOrigem);
    new BuscarClientes(_solicitacaoLicitacao.ClienteDestino, retornoClienteDestino);
    new BuscarLocalidades(_solicitacaoLicitacao.LocalidadeOrigem);
    new BuscarLocalidades(_solicitacaoLicitacao.LocalidadeDestino);
    new BuscarUnidadesMedida(_solicitacaoLicitacao.UnidadeMedida);
    new BuscarTiposdeCarga(_solicitacaoLicitacao.TipoDeCarga);

    buscarSolicitacaoLicitacao();

    carregarGridProdutos();
}

function solicitarLicitacaoClick() {
    if (_solicitacaoLicitacao.Produto.basicTable.BuscarRegistros().length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório a seleção de pelo menos 1 produto.");
        return;
    }

    var data = {
        SolicitacaoLicitacao: JSON.stringify(RetornarObjetoPesquisa(_solicitacaoLicitacao)),
        Produtos: JSON.stringify(_solicitacaoLicitacao.Produto.basicTable.BuscarRegistros())
    };
    

    executarReST("SolicitacaoLicitacao/Adicionar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridSolicitacaoLicitacao.CarregarGrid();
                limparCamposSolicitacaoLicitacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarSolicitacaoLicitacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a solicitação criada?", function () {
        executarReST("SolicitacaoLicitacao/Cancelar", { Codigo: _solicitacaoLicitacao.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    _gridSolicitacaoLicitacao.CarregarGrid();
                    limparCamposSolicitacaoLicitacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function limparCamposSolicitacaoLicitacaoClick() {
    limparCamposSolicitacaoLicitacao();
}

//*******MÉTODOS*******

function carregarGridProdutos() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirProdutoClick(_solicitacaoLicitacao.Produto, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridProduto = new BasicDataTable(_solicitacaoLicitacao.GridProdutos.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    new BuscarProdutos(_solicitacaoLicitacao.Produto, null, null, null, null, null, null, _gridProduto);
    _solicitacaoLicitacao.Produto.basicTable = _gridProduto;

    recarregarGridProduto([]);
}

function recarregarGridProduto(data) {
    _gridProduto.CarregarGrid(data);
}

function excluirProdutoClick(knoutProduto, data) {
    var produtoGrid = knoutProduto.basicTable.BuscarRegistros();

    for (var i = 0; i < produtoGrid.length; i++) {
        if (data.Codigo == produtoGrid[i].Codigo) {
            produtoGrid.splice(i, 1);
            break;
        }
    }

    knoutProduto.basicTable.CarregarGrid(produtoGrid);
}

function retornoClienteOrigem(data) {
    _solicitacaoLicitacao.ClienteOrigem.val(data.Descricao);
    _solicitacaoLicitacao.ClienteOrigem.codEntity(data.Codigo);
    _solicitacaoLicitacao.EnderecoOrigem.val(data.Endereco + " N° " + data.Numero + " (" + data.Localidade + ")");
}

function retornoClienteDestino(data) {
    _solicitacaoLicitacao.ClienteDestino.val(data.Descricao);
    _solicitacaoLicitacao.ClienteDestino.codEntity(data.Codigo);
    _solicitacaoLicitacao.EnderecoDestino.val(data.Endereco + " N° " + data.Numero + " (" + data.Localidade + ")");
}

function buscarSolicitacaoLicitacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSolicitacaoLicitacao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridSolicitacaoLicitacao = new GridView(_pesquisaSolicitacaoLicitacao.Pesquisar.idGrid, "SolicitacaoLicitacao/Pesquisa", _pesquisaSolicitacaoLicitacao, menuOpcoes);
    _gridSolicitacaoLicitacao.CarregarGrid();
}

function editarSolicitacaoLicitacao(itemGrid) {
    limparCamposSolicitacaoLicitacao();
    _solicitacaoLicitacao.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_solicitacaoLicitacao, "SolicitacaoLicitacao/BuscarPorCodigo", function (arg) {
        _pesquisaSolicitacaoLicitacao.ExibirFiltros.visibleFade(false);
        SetarEtapasSolicitacaoLicitacao();
        controleCamposCRUDSolicitacaoLicitacao();
        recarregarGridProduto(arg.Data.Produtos);

        _solicitacaoLicitacao.Numero.visible(true);
        _solicitacaoLicitacao.Usuario.visible(true);
        SetarEnableCamposKnockout(_solicitacaoLicitacao, false);

        if (arg.Data.Cotacao !== null && arg.Data.Cotacao !== undefined) {
            PreencherObjetoKnout(_solicitacaoLicitacaoCotacao, { Data: arg.Data.Cotacao });
        }
    }, null);
}

function limparCamposSolicitacaoLicitacao() {
    LimparCampos(_solicitacaoLicitacao);
    SetarEnableCamposKnockout(_solicitacaoLicitacao, true);
    controleCamposCRUDSolicitacaoLicitacao();
    SetarEtapaInicioSolicitacaoLicitacao();
    recarregarGridProduto([]);

    _solicitacaoLicitacao.Numero.visible(false);
    _solicitacaoLicitacao.Usuario.visible(false);
    limparCamposSolicitacaoLicitacaoCotacao();
}

function controleCamposCRUDSolicitacaoLicitacao() {
    var situacao = _solicitacaoLicitacao.Situacao.val();

    _CRUDSolicitacaoLicitacao.Limpar.visible(true);
    _CRUDSolicitacaoLicitacao.Solicitar.visible(true);
    _CRUDSolicitacaoLicitacao.Finalizar.visible(false);
    _CRUDSolicitacaoLicitacao.Cancelar.visible(false);
    _CRUDSolicitacaoLicitacao.Rejeitar.visible(false);

    if (_solicitacaoLicitacao.UsuarioLogadoCriouACotacao.val() && situacao === EnumSituacaoSolicitacaoLicitacao.AgCotacao) {
        _CRUDSolicitacaoLicitacao.Solicitar.visible(false);
        _CRUDSolicitacaoLicitacao.Cancelar.visible(true);
    }
    else if (situacao === EnumSituacaoSolicitacaoLicitacao.AgCotacao) {
        _CRUDSolicitacaoLicitacao.Solicitar.visible(false);
        _CRUDSolicitacaoLicitacao.Finalizar.visible(true);
        _CRUDSolicitacaoLicitacao.Rejeitar.visible(true);
    } else if (situacao === EnumSituacaoSolicitacaoLicitacao.Finalizada || situacao === EnumSituacaoSolicitacaoLicitacao.Rejeitada || situacao === EnumSituacaoSolicitacaoLicitacao.Cancelada) {
        _CRUDSolicitacaoLicitacao.Solicitar.visible(false);
        SetarEnableCamposKnockout(_solicitacaoLicitacaoCotacao, false);
        _solicitacaoLicitacaoCotacao.UsuarioCotacao.visible(true);
    }
}