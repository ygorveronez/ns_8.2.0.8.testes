/// <reference path="EtapasFluxoAvaria.js" />
/// <reference path="Anexos.js" />
/// <reference path="Aprovadores.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Empresa.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoFluxoAvaria.js" />
/// <reference path="../../Enumeradores/EnumFormaPreenchimentoCentroResultadoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridAvaria;
var _fluxoAvaria;
var _CRUDAvaria;
var _pesquisaAvaria;

var PesquisaAvaria = function () {
    this.NumeroAvaria = PropertyEntity({ text: "Número da Avaria:", val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Avaria:", issue: 943, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Avaria início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Avaria limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

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

var FluxoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataAvaria = PropertyEntity({ text: "*Data da Avaria: ", getType: typesKnockout.date, val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Carga:",issue: 195, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista: ", issue: 145, maxlength: 200, required: true, enable: ko.observable(true) });
    this.RGMotorista = PropertyEntity({ text: "RG: ", maxlength: 15, getType: typesKnockout.int, required: false, enable: ko.observable(true) });
    this.CPFMotorista = PropertyEntity({ text: "CPF: ", maxlength: 14, required: true, enable: ko.observable(true) });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, enable: ko.observable(true), text: "*Motivo da Avaria:", issue: 943, idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Centro de Resultado", idBtnSearch: guid(), enable: ko.observable(true) });
    this.FilialEmbarque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Filial Emissão:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ text: "Justificativa: ", maxlength: 500, enable: ko.observable(true) });
    this.SituacaoFluxo = PropertyEntity({ val: ko.observable(EnumSituacaoFluxoAvaria.Dados), options: EnumSituacaoFluxoAvaria.obterOpcoes(), def: EnumSituacaoFluxoAvaria.Dados });
    this.Lote = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });  
}

var CRUDAvaria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.AvancarTermo = PropertyEntity({ eventClick: avancarTermoClick, type: types.event, text: "Avançar", visible: ko.observable(false) });
    this.GerarLote = PropertyEntity({ eventClick: gerarLoteClick, type: types.event, text: "Gerar Lote", visible: ko.observable(false) });
    this.FinalizarLote = PropertyEntity({ eventClick: finalizarLoteClick, type: types.event, text: "Finalizar Lote", visible: ko.observable(false) });
    this.FinalizarDestinacao = PropertyEntity({ eventClick: finalizarDestinoAvariaClick, type: types.event, text: "Finalizar Destinação", visible: ko.observable(false) });
}

let Termo = function () {
    this.Termo = PropertyEntity({ val: ko.observable(false), enable: ko.observable(true), eventClick: BaixarTermo, type: types.event, text: "Termo", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadFluxoAvaria() {
    _fluxoAvaria = new FluxoAvaria();
    KoBindings(_fluxoAvaria, "knockoutCadastroAvaria");

    _pesquisaAvaria = new PesquisaAvaria();
    KoBindings(_pesquisaAvaria, "knockoutPesquisaAvarias", false, _pesquisaAvaria.Pesquisar.id);

    _termo = new Termo();
    KoBindings(_termo, "knockoutTermoAvaria");

    _CRUDAvaria = new CRUDAvaria();
    KoBindings(_CRUDAvaria, "knockoutCRUDCadastroAvaria");

    loadResumoAvaria();
    loadEtapasFluxoAvaria();
    loadAnexos();
    loadProdutosAvariados();
    loadAutorizadores();
    loadLote();
    loadIntegracao();
    LoadDestinoAvarias();

    // Inicia as buscas
    BuscarCargaAvarias(_fluxoAvaria.Carga, cargaCallback);
    BuscarMotivoAvaria(_fluxoAvaria.MotivoAvaria, EnumFinalidadeMotivoAvaria.MotivoAvaria);
    BuscarMotivoAvaria(_pesquisaAvaria.MotivoAvaria, EnumFinalidadeMotivoAvaria.MotivoAvaria);
    BuscarCargaAvarias(_pesquisaAvaria.Carga);
    BuscarEmpresa(_fluxoAvaria.FilialEmbarque);
    BuscarCentroResultado(_fluxoAvaria.CentroResultado, null, null, null, EnumAnaliticoSintetico.Analitico);

    buscarAvarias();

    // Mascara
    $("#" + _fluxoAvaria.CPFMotorista.id).mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });
    ControleDeCampos();
}

function cargaCallback(carga) {
    // Seta carga normalmente
    _fluxoAvaria.Carga.val(carga.CodigoCargaEmbarcador);
    _fluxoAvaria.Carga.codEntity(carga.Codigo);
    _fluxoAvaria.Motorista.val(carga.Motorista);
    _fluxoAvaria.RGMotorista.val(carga.RGMotorista);
    _fluxoAvaria.CPFMotorista.val(carga.CPFMotorista);
    _fluxoAvaria.FilialEmbarque.codEntity(carga.Empresa);
    _fluxoAvaria.FilialEmbarque.val(carga.NomeEmpresa);
    _fluxoAvaria.CentroResultado.codEntity(carga.CentroResultadoCodigo);
    _fluxoAvaria.CentroResultado.val(carga.CentroResultadoDescricao);    
}

function adicionarClick(e, sender) {

    // Busca a lista
    let anexos = GetAnexos();
    let data = { qtdAnexo: anexos.length, codigoMotivoAvaria: _fluxoAvaria.MotivoAvaria.codEntity() };

    let retWS = executarReST("/FluxoAvaria/ValidarAnexos", data, function (arg) {

        let retornoValidarAnexo = true;

        if (arg.Success) {
            if (arg.Data) {
                retornoValidarAnexo = true;
            } else {
                exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg);
                retornoValidarAnexo = false;
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            retornoValidarAnexo = false;
        }

        if (retornoValidarAnexo) {
            Salvar(_fluxoAvaria, "FluxoAvaria/Adicionar", function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        // Adiciona codigo apenas apra enviar anexos
                        _fluxoAvaria.Codigo.val(arg.Data.Codigo);
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
    });



}

function finalizarClick(e, sender) {
    
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o fluxo de avaria?", function () {
        executarReST("/FluxoAvaria/FinalizarSolicitacao", { Avaria: _fluxoAvaria.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalização efetuada com sucesso.");
                    BuscarFluxocaoPorCodigo();
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
    Salvar(_fluxoAvaria, "SolicitacaoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _fluxoAvaria.Codigo.val(arg.Data.Codigo);
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
    exibirConfirmacao("Confirmação", "Realmente deseja limpar os campos?", function () {
        limparCamposFluxoAvaria();
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar essa avaria?", function () {
        ExcluirPorCodigo(_fluxoAvaria, "FluxoAvaria/CancelarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAvaria.CarregarGrid();
                    limparCamposFluxoAvaria();
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
        ExcluirPorCodigo(_fluxoAvaria, "FluxoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAvaria.CarregarGrid();
                    limparCamposFluxoAvaria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function BaixarTermo() {
    let data = { Codigo: _fluxoAvaria.Codigo.val() };
    executarDownload("FluxoAvaria/TermoSolicitacaoAvaria", data);
}

function avancarTermoClick() {
    executarReST("/FluxoAvaria/ConfirmarEtapaTermo", { Codigo: _fluxoAvaria.Codigo.val() }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
            BuscarFluxocaoPorCodigo();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function gerarLoteClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja gerar o lote?", function () {
        executarReST("/FluxoAvaria/GerarLote", { Codigo: _fluxoAvaria.Codigo.val() }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote gerado com sucesso.");
                BuscarFluxocaoPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function finalizarLoteClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja confirmar a etapa de lote?", function () {
        let dados = {
            Codigo: _fluxoAvaria.Codigo.val()
        }
        executarReST("FluxoAvaria/ConfirmarEtapaLote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                    BuscarFluxocaoPorCodigo();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function finalizarDestinoAvariaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar os destinos das avarias?", function () {
        executarReST("FluxoAvaria/FinalizarDestinoAvaria", { Codigo: _fluxoAvaria.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function reprocessarClick() {
    BuscarRegrasDaEtapa();
}

//*******MÉTODOS*******
function buscarAvarias() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSolicitacao, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    let configExportacao = {
        url: "FluxoAvaria/ExportarPesquisa",
        titulo: "Fluxo de Avaria"
    };

    _gridAvaria = new GridViewExportacao(_pesquisaAvaria.Pesquisar.idGrid, "FluxoAvaria/Pesquisa", _pesquisaAvaria, menuOpcoes, configExportacao);
    _gridAvaria.CarregarGrid();
}

function editarSolicitacao(solicitacaoGrid) {
    // Seta Codigo
    _fluxoAvaria.Codigo.val(solicitacaoGrid.Codigo);

    // Esconde filtros
    _pesquisaAvaria.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarFluxocaoPorCodigo();

    setarEtapasFluxoAvaria();
}

function BuscarFluxocaoPorCodigo(cb) {
    BuscarPorCodigo(_fluxoAvaria, "FluxoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Data != null) {
            _anexos.Anexos.val(arg.Data.Anexos);

            // Permite Excluir
            _CRUDAvaria.Cancelar.visible(arg.Data.PodeCancelar);

            _CRUDAvaria.Limpar.visible(true);
            _CRUDAvaria.Adicionar.visible(false);
            _CRUDAvaria.Excluir.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos);
            _CRUDAvaria.Cancelar.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgAprovacao || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.RejeitadaAutorizacao || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.SemRegraAprovacao);
            _CRUDAvaria.Finalizar.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos);
            _CRUDAvaria.AvancarTermo.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Termo);
            _CRUDAvaria.GerarLote.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgLote);
            _CRUDAvaria.FinalizarLote.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.LoteGerado);
            _CRUDAvaria.FinalizarDestinacao.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Destinacao);
            _CRUDAvaria.Reprocessar.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.SemRegraAprovacao);

            _resumoAvaria.Codigo.val(_fluxoAvaria.Codigo.val());

            _termo.Termo.val(arg.Data.MotivoAvaria.DesabilitarBotaoTermo);

            PreecherResumoAvaria();
            
            setarEtapasFluxoAvaria();
            ControleDeCampos();

            gridProdutosAvariados();
            _gridProdutos.CarregarGrid();

            loadGridNotas();
            obterNotasPorProdutos();

            if (arg.Data.Detalhes != null) {
                PreencherObjetoKnout(_detalhesFluxoAprovacao, { Data: arg.Data.Detalhes });
                _detalhesFluxoAprovacao.DetalhesAvaria.visible(true);
            }
            else
                _detalhesFluxoAprovacao.DetalhesAvaria.visible(false);

            _gridAutorizacoes.CarregarGrid();

            _gridLote.CarregarGrid();
            _lote.Codigo.val(_fluxoAvaria.Lote.val());
            VerificaLote();

            CarregaIntegracao();
            _gridIntegracao.CarregarGrid();

            CarregarDestinoAvarias();
            DefinirTab();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    }, null);
}

function limparCamposFluxoAvaria() {
    _CRUDAvaria.Limpar.visible(false);
    _CRUDAvaria.Adicionar.visible(true);
    _CRUDAvaria.Excluir.visible(false);
    _CRUDAvaria.Cancelar.visible(false);
    _CRUDAvaria.Finalizar.visible(false);
    _CRUDAvaria.Reprocessar.visible(false);
    _CRUDAvaria.FinalizarLote.visible(false);
    _CRUDAvaria.FinalizarDestinacao.visible(false);

    _resumoAvaria.NumeroAvaria.visible(false);

    limparAnexosTela();

    setarEtapaInicioAvaria();
    LimparCampos(_fluxoAvaria);
    $("#liTabDados a").click();
    ControleDeCampos();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ControleDeCampos() {
    _fluxoAvaria.DataAvaria.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.Carga.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.Motorista.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.RGMotorista.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.CPFMotorista.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.MotivoAvaria.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.FilialEmbarque.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _fluxoAvaria.Justificativa.enable(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados);
    _notas.ListaNotas.visible(_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos);
    _termo.Termo.enable(_termo.Termo.val() == null || !_termo.Termo.val());
}