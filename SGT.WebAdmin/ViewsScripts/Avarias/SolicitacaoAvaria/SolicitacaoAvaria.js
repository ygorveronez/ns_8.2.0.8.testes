/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="Integracao.js" />
/// <reference path="Lote.js" />
/// <reference path="Lote.js" />
/// <reference path="Aprovadores.js" />
/// <reference path="ResumoAvaria.js" />
/// <reference path="Anexos.js" />
/// <reference path="EtapasAvaria.js" />
/// <reference path="ProdutosAvariados.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridAvaria;
var _solicitacaoAvaria;
var _CRUDAvaria;
var _pesquisaAvaria;

var _SituacaoAvaria = [
    { text: "Todas", value: EnumSituacaoAvaria.Todas },
    { text: "Em Criação", value: EnumSituacaoAvaria.EmCriacao },
    { text: "Ag Aprovação", value: EnumSituacaoAvaria.AgAprovacao },
    { text: "Cancelada", value: EnumSituacaoAvaria.Cancelada },
    { text: "Ag Lote", value: EnumSituacaoAvaria.AgLote },
    { text: "Lote Gerado", value: EnumSituacaoAvaria.LoteGerado },
    { text: "Lote Rejeitado", value: EnumSituacaoAvaria.RejeitadaLote },
    { text: "Sem Regra de Aprovação", value: EnumSituacaoAvaria.SemRegraAprovacao },
    { text: "Sem Regra de Lote", value: EnumSituacaoAvaria.SemRegraLote },
    { text: "Finalizada", value: EnumSituacaoAvaria.Finalizada },
    { text: "Rejeitada", value: EnumSituacaoAvaria.RejeitadaAutorizacao }
];

var PesquisaAvaria = function () {
    this.DataInicio = PropertyEntity({ text: "Data Avaria início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Avaria limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.NumeroAvaria = PropertyEntity({ text: "Número da Avaria:", val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.NumeroNota = PropertyEntity({ text: "Número da Nota:", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true) });
    this.Transportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportadora:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Avaria:",issue: 943, idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoAvaria = PropertyEntity({ val: ko.observable(EnumSituacaoAvaria.Todas), options: _SituacaoAvaria, def: EnumSituacaoAvaria.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAvaria.CarregarGrid();
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

var SolicitacaoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroAvaria = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Número: ", def: "", enable: false });
    this.DataAvaria = PropertyEntity({ text: "*Data da Avaria: ", getType: typesKnockout.date, val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({text: "*Data da Solicitação: ",getType: typesKnockout.dateTime,val: ko.observable(""),  required: false, enable: ko.observable(false)});

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Viagem:",issue: 195, idBtnSearch: guid(), enable: ko.observable(true) });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, enable: ko.observable(true), text: "*Motivo da Avaria:",issue: 943, idBtnSearch: guid() });
   
    this.Motorista = PropertyEntity({ text: "Motorista: ", issue: 145, maxlength: 200, required: true, enable: ko.observable(true) });
    this.RGMotorista = PropertyEntity({ text: "RG: ", maxlength: 15, getType: typesKnockout.int, required: false, enable: ko.observable(true) });
    this.CPFMotorista = PropertyEntity({ text: "CPF: ", maxlength: 14, required: true, enable: ko.observable(true) });

    this.Justificativa = PropertyEntity({ text: "Justificativa: ", maxlength: 500, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAvaria.Todas), def: EnumSituacaoAvaria.Todas, getType: typesKnockout.int });

    this.ProdutosAvariados = PropertyEntity({
        text: "Produtos Avariados", type: types.local, val: ko.observable(""), idGrid: guid(),
        Legendas: [
            /*{
                Nome: "Produto com avaria",
                Cor: "#dfd0ef"
            },
            {
                Nome: "Produto diferente da Carga",
                Cor: "#edefd0"
            },*/
        ]
    });

    this.DescricaoProduto = PropertyEntity({ map: types.local, text: "Descrição: " });
    this.AdicionarProduto = PropertyEntity({ eventClick: produtoClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutosAvariados.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var CRUDAvaria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Solicitar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.Termo = PropertyEntity({ eventClick: termoClick, type: types.event, text: "Termo", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadSolicitacaoAvaria() {
    _solicitacaoAvaria = new SolicitacaoAvaria();
    KoBindings(_solicitacaoAvaria, "knockoutCadastroAvaria");

    _pesquisaAvaria = new PesquisaAvaria();
    KoBindings(_pesquisaAvaria, "knockoutPesquisaAvarias", false, _pesquisaAvaria.Pesquisar.id);

    _CRUDAvaria = new CRUDAvaria();
    KoBindings(_CRUDAvaria, "knockoutCRUDCadastroAvaria");
    
    loadProdutosAvariados();
    loadCadastroProdutosAvariados();
    loadEtapasAvaria();
    loadResumoAvaria();
    loadAnexos();
    loadAutorizadores();
    loadLote();
    loadIntegracao();

    const dicionario = {
        DataSolicitacao: "Data Solicitação"
    };
    HeaderAuditoria("SolicitacaoAvaria", _solicitacaoAvaria, null, GerarDicionarioKnout(_resumoAvaria, dicionario));

    // Inicia as buscas
    BuscarCargaAvarias(_solicitacaoAvaria.Carga, cargaCallback);
    BuscarMotivoAvaria(_solicitacaoAvaria.MotivoAvaria, EnumFinalidadeMotivoAvaria.MotivoAvaria);
    BuscarMotivoAvaria(_pesquisaAvaria.MotivoAvaria, EnumFinalidadeMotivoAvaria.MotivoAvaria);
    BuscarTransportadores(_pesquisaAvaria.Transportadora);

    buscarAvarias();

    // Mascara
    $("#" + _solicitacaoAvaria.CPFMotorista.id).mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });

    //-- Controle de anexos
    _solicitacaoAvaria.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexos();
    });

    //-- Limpa formulário
    ControleDeCampos();
}

function loadProdutosAvariados(cb) {
    const editar = {
        descricao: "Editar",
        id: guid(),
        evento: "onclick",
        metodo: editarProduto,
        visibilidade: function () { return _solicitacaoAvaria.Situacao.val() == EnumSituacaoAvaria.EmCriacao; },
        tamanho: "5",
        icone: ""
    };
    const auditar = {
        descricao: "Auditar",
        id: guid(),
        evento: "onclick",
        metodo: OpcaoAuditoria("ProdutosAvariados", null, _produtosAvariados),
        tamanho: "5",
        icone: ""
    };
    let menuOpcoes = {
        tipo: PermiteAuditar() ? TypeOptionMenu.list : TypeOptionMenu.link,
        descricao: "Opções",
        tamanho: 5,
        opcoes: [editar]
    };
    if (PermiteAuditar()) 
        menuOpcoes.opcoes.push(auditar);

    // Callback
    if (cb == null)
        cb = function () { };

    // Nao mostra opcao de edicao de acordo com a situacao
    const situacao = _solicitacaoAvaria.Situacao.val();
    if (situacao != EnumSituacaoAvaria.EmCriacao && !PermiteAuditar())
        menuOpcoes = null;

    // Destroi a grid
    if (_gridProdutosAvariados != null)
        _gridProdutosAvariados.Destroy();

    // Cria a nova tabela
    _gridProdutosAvariados = new GridView(_solicitacaoAvaria.ProdutosAvariados.idGrid, "SolicitacaoAvaria/ProdutosAvariados", _solicitacaoAvaria, menuOpcoes);
    _gridProdutosAvariados.CarregarGrid(cb);
}

function cargaCallback(carga) {
    // Seta carga normalmente
    _solicitacaoAvaria.Carga.val(carga.CodigoCargaEmbarcador);
    _solicitacaoAvaria.Carga.codEntity(carga.Codigo);
    _solicitacaoAvaria.Motorista.val(carga.Motorista);
    _solicitacaoAvaria.RGMotorista.val(carga.RGMotorista);
    _solicitacaoAvaria.CPFMotorista.val(carga.CPFMotorista);
}

function editarProduto(dataRow) {
    // Vincula cod
    _produtoAvariado.Codigo.val(dataRow.Codigo);
    _produtoAvariado.Solicitacao.val(_solicitacaoAvaria.Codigo.val());

    // Libera crud e campos
    _produtoAvariado.Produto.enable(false);
    _produtoAvariado.Atualizar.visible(true);
    _produtoAvariado.Excluir.visible(true);
    _produtoAvariado.Cancelar.visible(true);
    _produtoAvariado.Adicionar.visible(false);

    // Carrega informacoes
    BuscarProdutoPorCodigo(AbrirModalProdutosAvariados);
}

function adicionarClick(e, sender) {
    Salvar(_solicitacaoAvaria, "SolicitacaoAvaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                // Adiciona codigo apenas apra enviar anexos
                _solicitacaoAvaria.Codigo.val(arg.Data.Codigo);
                EnviarArquivosAnexados();

                // Carrega toda solicitacao
                editarSolicitacao(arg.Data);
                _gridAvaria.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function finalizarClick(e, sender) {
    // Valida produtos avariados
    if (_gridProdutosAvariados.NumeroRegistros() == 0)
        return exibirMensagem(tipoMensagem.aviso, "Produtos Avariados", "Nenhum produto cadastrado.");

    // Converte dados
    var _solicitacao = RetornarObjetoPesquisa(_solicitacaoAvaria);

    exibirConfirmacao("Confirmação", "Realmente deseja enviar?", function () {
        executarReST("SolicitacaoAvaria/FinalizarSolicitacao", _solicitacao, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação efetuada com sucesso.");

                    // Carrega toda solicitacao
                    editarSolicitacao(arg.Data);
                    _gridAvaria.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function atualizarClick() {
    Salvar(_solicitacaoAvaria, "SolicitacaoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _solicitacaoAvaria.Codigo.val(arg.Data.Codigo);
                BuscarSolicitacaoPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function limparClick() {
    limparCamposSolicitacao();
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar essa avaria?", function () {
        ExcluirPorCodigo(_solicitacaoAvaria, "SolicitacaoAvaria/CancelarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAvaria.CarregarGrid();
                    limparCamposSolicitacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir essa avaria?", function () {
        ExcluirPorCodigo(_solicitacaoAvaria, "SolicitacaoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAvaria.CarregarGrid();
                    limparCamposSolicitacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function termoClick() {
    const data = { Codigo: _solicitacaoAvaria.Codigo.val() };
    executarDownload("SolicitacaoAvaria/TermoSolicitacaoAvaria", data);
}

function reprocessarClick() {
    BuscarRegrasDaEtapa();
}

function produtoClick(e, sender) {
    // Limpa antes de abrir
    LimparCamposProdutoAvariado()

    // Seta o codigo da solicitacao avaria
    _produtoAvariado.Solicitacao.val(_solicitacaoAvaria.Codigo.val());

    // Chame metodos da tela de produtos avariados
    AbrirModalProdutosAvariados();
}


//*******MÉTODOS*******
function buscarAvarias() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSolicitacao, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    const configExportacao = {
        url: "SolicitacaoAvaria/ExportarPesquisa",
        titulo: "Solicitação de Avaria"
    };

    _gridAvaria = new GridViewExportacao(_pesquisaAvaria.Pesquisar.idGrid, "SolicitacaoAvaria/Pesquisa", _pesquisaAvaria, menuOpcoes, configExportacao);
    _gridAvaria.CarregarGrid();
}

function editarSolicitacao(solicitacaoGrid) {
    // Seta Codigo
    _solicitacaoAvaria.Codigo.val(solicitacaoGrid.Codigo);
    _resumoAvaria.Codigo.val(solicitacaoGrid.Codigo);
    _resumoAvaria.Codigo.val(solicitacaoGrid.Codigo);

    // Esconde filtros
    _pesquisaAvaria.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarSolicitacaoPorCodigo();
    PreecherResumoAvaria();
    VerificaLote();
}

function BuscarSolicitacaoPorCodigo(cb) {
    BuscarPorCodigo(_solicitacaoAvaria, "SolicitacaoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Data != null) {
            _anexos.Anexos.val(arg.Data.Anexos);
            _solicitacaoAvaria.AdicionarProduto.visible(true);
            _gridAutorizacoes.CarregarGrid();

            // Permite Excluir
            _CRUDAvaria.Cancelar.visible(arg.Data.PodeCancelar);

            if (arg.Data.Detalhes != null) {
                PreencherObjetoKnout(_detalhesSolicitacao, { Data: arg.Data.Detalhes });
                _detalhesSolicitacao.DetalhesAvaria.visible(true);
            }
            else
                _detalhesSolicitacao.DetalhesAvaria.visible(false);

            loadProdutosAvariados();
            MensagemIntegracao();

            setarEtapasAvaria();
            ControleDeCampos();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    }, null);
}

function limparCamposSolicitacao() {
    _CRUDAvaria.Limpar.visible(true);
    _CRUDAvaria.Adicionar.visible(true);
    _CRUDAvaria.Excluir.visible(false);
    _CRUDAvaria.Cancelar.visible(false);
    _CRUDAvaria.Finalizar.visible(false);
    _CRUDAvaria.Reprocessar.visible(false);
    _CRUDAvaria.Termo.visible(false);

    _detalhesSolicitacao.MensagemEtapaSemRegra.visible(false);

    LimparCampos(_solicitacaoAvaria);
    limparResumo();
    limparAnexosTela();
    limparLote();
    loadProdutosAvariados();
    setarEtapaInicioAvaria();

    $("#liTabDados a").click();

    ControleDeCampos();
    MensagemIntegracao();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ControleDeCampos() {
    const situacaoAvaria = _solicitacaoAvaria.Situacao.val();

    if (situacaoAvaria == EnumSituacaoAvaria.Todas) {
        // Todos Campos Disponíveis
        _solicitacaoAvaria.DataAvaria.enable(true);
        _solicitacaoAvaria.Carga.enable(true);
        _solicitacaoAvaria.MotivoAvaria.enable(true);
        _solicitacaoAvaria.Justificativa.enable(true);

        _solicitacaoAvaria.Motorista.enable(true);
        _solicitacaoAvaria.RGMotorista.enable(true);
        _solicitacaoAvaria.CPFMotorista.enable(true);

        _solicitacaoAvaria.AdicionarProduto.visible(false)
        _CRUDAvaria.Limpar.visible(false);
        _CRUDAvaria.Termo.visible(false);

    } else if (situacaoAvaria == EnumSituacaoAvaria.EmCriacao) {
        // Nenhum Campo disponivel
        _solicitacaoAvaria.Carga.enable(false);
        _solicitacaoAvaria.DataAvaria.enable(false);
        _solicitacaoAvaria.MotivoAvaria.enable(false);
        _solicitacaoAvaria.Justificativa.enable(false);

        _solicitacaoAvaria.Motorista.enable(false);
        _solicitacaoAvaria.RGMotorista.enable(false);
        _solicitacaoAvaria.CPFMotorista.enable(false);


        _CRUDAvaria.Limpar.visible(true);
        _CRUDAvaria.Excluir.visible(true);
        _CRUDAvaria.Finalizar.visible(true);
        _CRUDAvaria.Adicionar.visible(false);

    } else if (
        situacaoAvaria == EnumSituacaoAvaria.AgAprovacao ||
        situacaoAvaria == EnumSituacaoAvaria.AgLote ||
        situacaoAvaria == EnumSituacaoAvaria.Rejeitada ||
        situacaoAvaria == EnumSituacaoAvaria.SemRegraAprovacao ||
        situacaoAvaria == EnumSituacaoAvaria.SemRegraLote ||
        situacaoAvaria == EnumSituacaoAvaria.LoteGerado ||
        situacaoAvaria == EnumSituacaoAvaria.AgIntegracao ||
        situacaoAvaria == EnumSituacaoAvaria.RejeitadaAutorizacao ||
        situacaoAvaria == EnumSituacaoAvaria.Cancelada
        ) {
        // Nenhum Campo disponivel
        _solicitacaoAvaria.Carga.enable(false);
        _solicitacaoAvaria.DataAvaria.enable(false);
        _solicitacaoAvaria.MotivoAvaria.enable(false);
        _solicitacaoAvaria.Justificativa.enable(false);

        _solicitacaoAvaria.Motorista.enable(false);
        _solicitacaoAvaria.RGMotorista.enable(false);
        _solicitacaoAvaria.CPFMotorista.enable(false);


        _CRUDAvaria.Limpar.visible(true);
        _CRUDAvaria.Excluir.visible(false);
        _CRUDAvaria.Adicionar.visible(false);
        _CRUDAvaria.Finalizar.visible(false);
        _solicitacaoAvaria.AdicionarProduto.visible(false);
        AlternaTelaDeAnexos();

        // Subcontrole
        if (situacaoAvaria == EnumSituacaoAvaria.SemRegraAprovacao || situacaoAvaria == EnumSituacaoAvaria.SemRegraLote)
            EtapaSemRegra();
        if (situacaoAvaria == EnumSituacaoAvaria.AgIntegracao)
            _lote.Mensagem.visible(true);
        if (situacaoAvaria == EnumSituacaoAvaria.Cancelada)
            _CRUDAvaria.Termo.visible(false);
    }

    if (situacaoAvaria != EnumSituacaoAvaria.Todas)
        _CRUDAvaria.Termo.visible(true);
}