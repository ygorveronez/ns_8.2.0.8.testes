/// <reference path="../../Consultas/Cliente.js" />
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
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />
/// <reference path="OrdemCompra.js" />
/// <reference path="Etapas.js" />
/// <reference path="Aprovacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdem;
var _pesquisaOrdem;
var _gridRequisicoes;
var _PermissoesPersonalizadas;
var _modalOrdemCompra;
var _modalLoteOrdemCompra;
var _modalLicencaVencida;
var _crudLoteOrdemCompra;
var _crudLicencaVencida;

var PesquisaOrdem = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoOrdemCompra.Todas), options: EnumSituacaoOrdemCompra.obterOpcoesPesquisa(), def: EnumSituacaoOrdemCompra.Todas });

    this.DataGeracaoInicio = PropertyEntity({ text: "Data Geração: ", getType: typesKnockout.date });
    this.DataGeracaoFim = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataGeracaoInicio.dateRangeLimit = this.DataGeracaoFim;
    this.DataGeracaoFim.dateRangeInit = this.DataGeracaoInicio;

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid(), visible: true });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador: ", idBtnSearch: guid(), visible: true });

    this.DataRetornoInicio = PropertyEntity({ text: "Data Retorno: ", getType: typesKnockout.date });
    this.DataRetornoFim = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataRetornoInicio.dateRangeLimit = this.DataRetornoFim;
    this.DataRetornoFim.dateRangeInit = this.DataRetornoInicio;

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid(), visible: true });

    this.NovaOrdem = PropertyEntity({ eventClick: NovaOrdemClick, type: types.event, text: "Nova O.C", icon: ko.observable("fal fa-plus"), visible: ko.observable(true) });
    this.ImportarLicenca = PropertyEntity({ eventClick: ImportarLicencaClick, type: types.event, text: "Importar Licenças Vencidas", icon: ko.observable("fal fa-plus"), visible: ko.observable(true) });
    this.ImportarRequisicao = PropertyEntity({ type: types.map, required: false, text: "Importar de Requisição", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.NumeroCotacao = PropertyEntity({ text: "Número da Cotação: ", getType: typesKnockout.int });
    this.NumeroRequisicao = PropertyEntity({ text: "Número de Requisição: ", getType: typesKnockout.int });
    this.LoteOrdemCompra = PropertyEntity({ eventClick: LoteOrdemCompraClick, type: types.event, text: "O.C. em Lote", icon: ko.observable("fal fa-plus"), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOrdem.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var CRUDOrdemCompra = function () {
    this.Finalizar = PropertyEntity({ type: types.event, eventClick: FinalizarClick, text: "Finalizar", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ type: types.event, eventClick: SalvarClick, text: "Salvar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: CancelarClick, text: "Cancelar", visible: ko.observable(true) });
};

var CRUDLicencaVencida = function () {
    this.Gerar = PropertyEntity({ type: types.event, eventClick: SalvarRequisicaoClick, text: "Gerar Requisições", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: LicencaCancelarClick, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadTelaOrdemCompra() {
    CarregarConteudosOrdemHTML(carregarOrdemCompra);
}

function carregarOrdemCompra() {
    _pesquisaOrdem = new PesquisaOrdem();
    KoBindings(_pesquisaOrdem, "knockoutPesquisaOrdem", false, _pesquisaOrdem.Pesquisar.id);

    _crudOrdemCompra = new CRUDOrdemCompra();
    KoBindings(_crudOrdemCompra, "knockoutCRUDOrdem");

    _crudLicencaVencida = new CRUDLicencaVencida();
    KoBindings(_crudLicencaVencida, "knockoutCRUDLicencaVencida");

    HeaderAuditoria("OrdemCompra", {});

    _gridRequisicoes = new BasicDataTable(_pesquisaOrdem.ImportarRequisicao.idGrid, []);
    _gridRequisicoes.CarregarGrid([]);

    new BuscarClientes(_pesquisaOrdem.Fornecedor);
    new BuscarProdutoTMS(_pesquisaOrdem.Produto);
    new BuscarFuncionario(_pesquisaOrdem.Operador);
    new BuscarClientes(_pesquisaOrdem.Transportador);
    new BuscarRequisicaoCompra(_pesquisaOrdem.ImportarRequisicao, retornoRequisicoesCompras, null, _gridRequisicoes);
    new BuscarVeiculos(_pesquisaOrdem.Veiculo);

    BuscarOrdens();

    LoadEtapasOrdemCompra();
    LoadOrdemCompra();
    LoadAutorizacao();

    LoadLoteOrdemCompra();
}

function AbrirModalOrdemCompra() {
    if (!_modalOrdemCompra)
        _modalOrdemCompra = new bootstrap.Modal(document.getElementById("divModalOrdem"), { keyboard: true, backdrop: 'static' });

    _modalOrdemCompra.show();
}

function AbrirModalLoteOrdemCompra() {
    if (!_modalLoteOrdemCompra)
        _modalLoteOrdemCompra = new bootstrap.Modal(document.getElementById("divModalLoteOrdemCompra"), { keyboard: true, backdrop: 'static' });

    _modalLoteOrdemCompra.show();
}

function AbrirModalLicencaVencida() {
    LoadLicencaVencida();

    if (!_modalLicencaVencida)
        _modalLicencaVencida = new bootstrap.Modal(document.getElementById("divModalLicencaVencida"), { keyboard: true, backdrop: 'static' });

    _modalLicencaVencida.show();
}

function ImportarLicencaClick(e, sender) {
    AbrirModalLicencaVencida();
    LimparCamposLoadOrdem();
    SetInitState();
}

function NovaOrdemClick(e, sender) {
    AbrirModalOrdemCompra();
    LimparCamposLoadOrdem();
    SetInitState();
}

function LoteOrdemCompraClick() {
    LimparCamposLoteOrdemCompra();    
    PreencherUsuarioLogadoLoteOC();
    AbrirModalLoteOrdemCompra();
    _loteOrdemCompra.Codigo.val(0);
}

function retornoRequisicoesCompras(requisicoes) {
    if (!$.isArray(requisicoes)) requisicoes = [requisicoes];
    requisicoes = requisicoes.map(function (req) {
        return req.Codigo;
    });

    LimparCamposLoadOrdem();
    SetInitState();
    _ordemCompra.Requisicoes.val(JSON.stringify(requisicoes));

    executarReST("OrdemCompra/ImportarDeRequisicoes", { Codigos: _ordemCompra.Requisicoes.val() }, function (arg) {
        PreencherObjetoKnout(_ordemCompra, arg);

        CarregarProdutosDaOrdem(arg.Data.Produtos);

        AbrirModalOrdemCompra();
    });
}

function editarOrdemClick(itemGrid) {
    EditarOrdemPorCodigo(itemGrid.Codigo);
}

function duplicarOrdemClick(itemGrid) {
    EditarOrdemPorCodigo(itemGrid.Codigo, LimparParaDuplicar);
}

function imprimirOrdemClick(itemGrid) {
    executarDownload("OrdemCompra/Imprimir", { Codigo: itemGrid.Codigo });
}

function cancelarOrdemClick(itemGrid) {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar a Ordem de Compra " + itemGrid.Numero + "?", function () {
        executarReST("OrdemCompra/Cancelar", { Codigo: itemGrid.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    _gridOrdem.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function reabrirOrdemClick(itemGrid) {
    exibirConfirmacao("Confirmação", "Você realmente deseja reabrir a Ordem de Compra " + itemGrid.Numero + "?", function () {
        executarReST("OrdemCompra/Reabrir", { Codigo: itemGrid.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reaberta com sucesso");
                    _gridOrdem.CarregarGrid();
                    EditarOrdemPorCodigo(itemGrid.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function reenviarEmailOrdemClick(itemGrid) {
    exibirConfirmacao("Confirmação", "Você realmente deseja reenviar o e-mail da Ordem de Compra " + itemGrid.Numero + "?", function () {
        executarReST("OrdemCompra/ReenviarEmail", { Codigo: itemGrid.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de reenvio realizada com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function SalvarClick(e, sender) {
    Salvar(_ordemCompra, "OrdemCompra/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridOrdem.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                EditarOrdemPorCodigo(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function FinalizarClick(e, sender) {
    Salvar(_ordemCompra, "OrdemCompra/Finalizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                _gridOrdem.CarregarGrid();
                _modalOrdemCompra.hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function SalvarRequisicaoClick(itemGrid) {
    exibirConfirmacao("Confirmação", "Você realmente deseja gerar requisições para todas as licenças selecionadas?", function () {
        let dados = RetornarObjetoPesquisa(_licencaVencida);

        dados.SelecionarTodos = _licencaVencida.SelecionarTodos.val();
        dados.LicencasSelecionadas = JSON.stringify(_gridLicencasVencidas.ObterMultiplosSelecionados());
        dados.LicencasNaoSelecionadas = JSON.stringify(_gridLicencasVencidas.ObterMultiplosNaoSelecionados());

        executarReST("OrdemCompra/ImportarDeLicencas", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.aviso, "Sucesso", "Requisição(ões) " + arg.Data.numeros + " foram geradas com sucesso.");
                    BuscarLicencasVencidas();
                    BuscarOrdens();
                    ResetaValores();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });

}

function BuscarLicencasVencidas(ApenasSemOrdemAberta) {
  
    //-- Multi escolha
    let multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _licencaVencida.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoesOC,
        callbackSelecionado: exibirMultiplasOpcoesOC,
        callbackSelecionarTodos: exibirMultiplasOpcoesOC,
        somenteLeitura: false
    }

    let ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    _gridLicencasVencidas = new GridView(_licencaVencida.LicencasVencidasGrid.idGrid, "Motorista/BuscaLicencaMotoristasAVencer", _licencaVencida, menuOpcoes, ordenacaoPadrao, 25, null, null, null, multiplaescolha, null, null, null);
    _gridLicencasVencidas.CarregarGrid();

}

function CancelarClick(e, sender) {
    LimparCamposLoadOrdem();
    _modalOrdemCompra.hide();
}
function LicencaCancelarClick(e, sender) {
    //LimparCamposLoadOrdem();
    _modalLicencaVencida.hide();
}


//*******MÉTODOS*******

function ResetaValores() {
    //-- Reseta
    _licencaVencida.SelecionarTodos.val(false);
    _crudLicencaVencida.Gerar.visible(false);
}

function VisibilidadeReabrirOrdem(itemGrid) {
    return itemGrid.CodigoSituacao === EnumSituacaoOrdemCompra.AgAprovacao && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas));
}

function VisibilidadeReenviarEmailOrdem(itemGrid) {
    return itemGrid.CodigoSituacao === EnumSituacaoOrdemCompra.Aprovada;
}

function BuscarOrdens() {
    var editarOrdem = { descricao: "Editar", id: guid(), metodo: editarOrdemClick, icone: "" };
    var duplicarOrdem = { descricao: "Duplicar", id: guid(), metodo: duplicarOrdemClick, icone: "" };
    var enviarEmailOrdem = { descricao: "Imprimir", id: guid(), metodo: imprimirOrdemClick, icone: "" };
    var cancelarOrdem = { descricao: "Cancelar", id: guid(), metodo: cancelarOrdemClick, icone: "" };
    var reabrirOrdem = { descricao: "Reabrir", id: guid(), metodo: reabrirOrdemClick, icone: "", visibilidade: VisibilidadeReabrirOrdem };
    var reenviarEmailOrdem = { descricao: "Reenviar E-mail", id: guid(), metodo: reenviarEmailOrdemClick, icone: "", visibilidade: VisibilidadeReenviarEmailOrdem };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [editarOrdem, duplicarOrdem, enviarEmailOrdem, cancelarOrdem, reabrirOrdem, reenviarEmailOrdem]
    };

    _gridOrdem = new GridView(_pesquisaOrdem.Pesquisar.idGrid, "OrdemCompra/Pesquisa", _pesquisaOrdem, menuOpcoes, null);
    _gridOrdem.CarregarGrid();
}

function CarregarConteudosOrdemHTML(callback) {
    $.get("Content/Static/Compras/OrdemCompra.html?dyn=" + guid(), function (data) {
        $("#ModaisOrdemCompra").html(data);
        callback();
    });
}

function EditarOrdemPorCodigo(codigo, cb) {
    LimparCamposLoadOrdem();

    _ordemCompra.Codigo.val(codigo);

    BuscarPorCodigo(_ordemCompra, "OrdemCompra/BuscarPorCodigo", function (arg) {
        if (arg.Data != null) {
            ListarAprovacoes(arg.Data);

            CarregarProdutosDaOrdem(arg.Data.Produtos);

            SetarEtapasOrdemCompra();

            if (_ordemCompra.Situacao.val() === EnumSituacaoOrdemCompra.Aberta) {
                if (arg.Data.BloquearEdicaoOrdemCompraPorAbastecimento) {
                    ControleCampos(false)
                }
                else {
                    ControleCampos(true);
                }
            } else {
                ControleCampos(false);
            }

            AbrirModalOrdemCompra();

            if (cb) cb();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparParaDuplicar() {
    _ordemCompra.Codigo.val(0);
    _ordemCompra.Situacao.val(EnumSituacaoOrdemCompra.Aberta);
    _ordemCompra.Numero.val("");
    ControleCampos(true);

    SetarEtapaInicioOrdemCompra();

    var produtos = GetProdutos().map(function (prd) {
        prd.Codigo.val = guid();

        return prd;
    });

    SetProdutos(produtos);
    RecarregarGridProdutos();
}

function ControleCampos(status) {
    _crudOrdemCompra.Salvar.visible(status);
    _crudOrdemCompra.Finalizar.visible(status);

    ControleCamposOrdemCompra(status);
}

function LimparCamposLoadOrdem() {
    SetarEtapaInicioOrdemCompra();
    LimparCamposOrdemCompra();
}

function SetInitState() {
    _ordemCompra.Codigo.val(0);
    _crudOrdemCompra.Salvar.visible(true);
    _crudOrdemCompra.Finalizar.visible(true);
    PreencheUsuarioLogado();
}
