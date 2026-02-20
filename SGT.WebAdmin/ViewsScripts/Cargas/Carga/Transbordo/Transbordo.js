/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoTransbordo.js" />
/// <reference path="ContratoFrete.js" />

//*******MAPEAMENTO*******

var _HTMLTransbordo;
var _cargaTransbordo;
var _gridCTeTransbordo;
var _gridMDFeTransbordo;
var _gridTransbordos;
var strKnoutCargaTransbordo;

var CargaTransbordo = function () {
    this.Grid = PropertyEntity({ idGrid: guid() });
    this.CTe = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.MDFe = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaSVM = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.DataDoTransbordo.getFieldDescription(), def: "", getType: typesKnockout.dateTime, required: true, enable: ko.observable(true) });
    this.NumeroTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.Numero.getFieldDescription(), def: "", enable: false });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Veiculo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Motorista.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalidadeTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.LocalidadeTransbordo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.MotivoTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), required: true, text: Localization.Resources.Cargas.Carga.Motivo.getRequiredFieldDescription(), def: "", enable: ko.observable(true) });


    this.GerarTransbordo = PropertyEntity({ eventClick: gerarTransbordoClick, type: types.event, text: Localization.Resources.Cargas.Carga.GerarTransbordo, visible: ko.observable(true) });
    this.Voltar = PropertyEntity({ eventClick: VoltarClick, type: types.event, text: Localization.Resources.Cargas.Carga.Voltar, visible: ko.observable(false) });
    this.CancelarTransbordo = PropertyEntity({ eventClick: cancelarTransbordoClick, type: types.event, text: Localization.Resources.Cargas.Carga.CancelarTransbordo, visible: ko.observable(false) });
    this.BuscarNovamenteMDFe = PropertyEntity({ eventClick: buscarNovamenteMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarBarraAtualizar, visible: ko.observable(true) });

    this.Status = PropertyEntity({ val: ko.observable("A"), def: "A" });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Cargas.Carga.MarcarDesmarcarTodos, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

function cancelarTransbordoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaCancelarTransbordo.format(_cargaTransbordo.NumeroTransbordo.val()), function () {
        var data = { Codigo: _cargaTransbordo.Codigo.val() };
        executarReST("Transbordo/CancelarTransbordo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var transbordo = arg.Data;
                    if (transbordo.SituacaoTransbordo == EnumSituacaoTransbordo.Cancelado) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TransbordoCanceladoComSucesso);
                        limparCamposTransbordo();
                    } else {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TransbordoEmCancelamentoAguardandoCancelamentoDoMDFe);
                        _gridMDFeTransbordo.CarregarGrid();
                    }
                    _gridTransbordos.CarregarGrid();
                    PreencherTransbordo(transbordo);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        });
    })

}


function verificarTransbordoClick(e) {
    ocultarTodasAbas(e);
    _cargaAtual = e;

    _cargaTransbordo = new CargaTransbordo();
    _cargaTransbordo.Carga.val(_cargaAtual.Codigo.val());
    _cargaTransbordo.CargaSVM.val(_cargaAtual.CargaSVM.val());


    strKnoutCargaTransbordo = "knoutCargaTransbordo" + e.EtapaTransbordo.idGrid;

    $("#" + e.EtapaTransbordo.idGrid).html(_HTMLTransbordo.replace(/#knoutCargaTransbordo/g, strKnoutCargaTransbordo));
    KoBindings(_cargaTransbordo, strKnoutCargaTransbordo);

    LocalizeCurrentPage();

    new BuscarMotoristas(_cargaTransbordo.Motorista, null, null, null, true);
    new BuscarLocalidades(_cargaTransbordo.LocalidadeTransbordo);
    new BuscarVeiculos(_cargaTransbordo.Veiculo);

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarTransbordo, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    _gridTransbordos = new GridView(_cargaTransbordo.Grid.idGrid, "Transbordo/Pesquisa", _cargaTransbordo, menuOpcoes, null);
    _gridTransbordos.CarregarGrid();

    PreencherGridCTesParaTransbordo();
    loadTransbordoContratoFrete();
}

function PreencherGridCTesParaTransbordo() {
    _cargaTransbordo.SelecionarTodos.visible(true);
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _cargaTransbordo.SelecionarTodos,
        somenteLeitura: false,
    }
    _gridCTeTransbordo = new GridView(_cargaTransbordo.CTe.idGrid, "Transbordo/ConsultarCTesParaTransbordo", _cargaTransbordo, buscarMenuCTes(), null, null, null, null, null, multiplaescolha);
    _gridCTeTransbordo.CarregarGrid();
}

function PreencherGridCTesDoTransbordo() {
    _cargaTransbordo.SelecionarTodos.visible(false);
    _gridCTeTransbordo = new GridView(_cargaTransbordo.CTe.idGrid, "Transbordo/ConsultarCTesDoTransbordo", _cargaTransbordo, buscarMenuCTes());
    _gridCTeTransbordo.CarregarGrid();
}

function buscarMenuCTes() {
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDacteClick, icone: "" };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML] };
    return menuOpcoes;
}

function buscarNovamenteMDFeClick(e) {
    _gridMDFeTransbordo.CarregarGrid();
}

function PreencherGridTransbordoMDFe() {
    _cargaTransbordo.MDFe.visible(true);
    var retornoSefaz = { descricao: Localization.Resources.Cargas.Carga.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "" };
    var baixarDAMDFE = { descricao: Localization.Resources.Cargas.Carga.BaixarDamdfe, id: guid(), metodo: baixarDAMDFeClick };
    var baixarXMLMDFe = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLMDFeClick };
    var emitir = {
        descricao: Localization.Resources.Cargas.Carga.Emitir, id: guid(), metodo: function (datagrid) {
            emitirMDFeTransbordoClick(datagrid);
        }, icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDAMDFE, baixarXMLMDFe, retornoSefaz, emitir] };
    _gridMDFeTransbordo = new GridView(_cargaTransbordo.MDFe.idGrid, "Transbordo/ConsultarMDFeTransbordo", _cargaTransbordo, menuOpcoes, null, 1);
    _gridMDFeTransbordo.CarregarGrid();
}

var CargaCTeMapTransbordo = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
}

function emitirMDFeTransbordoClick(e) {
    if (e.Status == "Rejeição") {
        var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("Transbordo/EmitirNovamenteMDFe", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    $("#" + _idTabEtapaMDFe).trigger("click");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.SituacaoNaoPermiteEmissao, Localization.Resources.Cargas.Carga.AtualSituacaoDoMDFeNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}

function editarTransbordo(data) {
    limparCamposTransbordo();
    _cargaTransbordo.Codigo.val(data.Codigo);
    buscarPorCodigoTransbordo();
}

function buscarPorCodigoTransbordo(callback) {
    BuscarPorCodigo(_cargaTransbordo, "Transbordo/BuscarPorCodigo", function (arg) {
        PreencherTransbordo(arg.Data);
        if (callback != null)
            callback();
    }, null);
}

function VoltarClick() {
    limparCamposTransbordo();
    _cargaTransbordo.Carga.val(_cargaAtual.Codigo.val());
    PreencherGridCTesParaTransbordo();
}

function gerarTransbordoClick(e) {

    if (ValidarCamposObrigatorios(e)) {

        var ctesSelecionados;
        if (_cargaTransbordo.SelecionarTodos.val()) {
            ctesSelecionados = _gridCTeTransbordo.ObterMultiplosNaoSelecionados();
        } else {
            ctesSelecionados = _gridCTeTransbordo.ObterMultiplosSelecionados();
        }

        if (ctesSelecionados.length > 0 || _cargaTransbordo.SelecionarTodos.val()) {
            $.each(ctesSelecionados, function (i, cte) {
                var map = new CargaCTeMapTransbordo();
                map.Codigo.val = cte.Codigo;
                _cargaTransbordo.CTe.list.push(map);
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.ObrigatorioInformarPeloMenosUmCTeParaTransbordo);
            valido = false;
        }

        Salvar(_cargaTransbordo, "Transbordo/GerarTransbordo", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    PreencherTransbordo(arg.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TransbordoGeradoComSucesso);
                    _gridTransbordos.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function InformarTransbordoCargaAtualizadaEvent(retorno) {
    if (retorno.TipoAcao == EnumTipoAcaoCarga.Alterada) {
        if (_cargaTransbordo != null) {
            _RequisicaoIniciada = true;
            _gridTransbordos.CarregarGrid(function () {
                _gridMDFeTransbordo.CarregarGrid(function () {
                    if (_cargaTransbordo.Codigo.val() == retorno.Transbordo) {
                        buscarPorCodigoTransbordo(function () {
                            _RequisicaoIniciada = false;
                        });
                    } else {
                        _RequisicaoIniciada = false;
                    }
                });
            });
        }
    };
}

function PreencherTransbordo(transbordo) {
    _cargaTransbordo.Codigo.val(transbordo.Codigo);
    _cargaTransbordo.NumeroTransbordo.val(transbordo.NumeroTransbordo);
    _cargaTransbordo.DataTransbordo.val(transbordo.DataTransbordo);
    _cargaTransbordo.Veiculo.val(transbordo.Veiculo);
    _cargaTransbordo.Veiculo.codEntity(transbordo.Codigo);
    _cargaTransbordo.Veiculo.val(transbordo.Motoristas[0].Descricao);
    _cargaTransbordo.Veiculo.codEntity(transbordo.Motoristas[0].Codigo);
    _cargaTransbordo.LocalidadeTransbordo.codEntity(transbordo.LocalidadeTransbordo.Codigo);
    _cargaTransbordo.LocalidadeTransbordo.val(transbordo.LocalidadeTransbordo.Descricao);
    _cargaTransbordo.MotivoTransbordo.val(transbordo.MotivoTransbordo);
    _cargaTransbordo.DataTransbordo.enable(false);
    _cargaTransbordo.Veiculo.enable(false);
    _cargaTransbordo.Motorista.enable(false);
    _cargaTransbordo.LocalidadeTransbordo.enable(false);
    _cargaTransbordo.MotivoTransbordo.enable(false);

    if (transbordo.PossuiMDFe) {
        PreencherGridTransbordoMDFe();
    }
    PreencherGridCTesDoTransbordo();

    _cargaTransbordo.GerarTransbordo.visible(false);

    _cargaTransbordo.Voltar.visible(true);
    if (transbordo.SituacaoTransbordo == EnumSituacaoTransbordo.EmTransporte || transbordo.SituacaoTransbordo == EnumSituacaoTransbordo.CancelamentoRejeitado) {
        _cargaTransbordo.CancelarTransbordo.visible(true);

        if (transbordo.PossuiContrato) {
            PossuiContratoFrete(transbordo.ContratoFrete);
        } else {
            NaoPossuiContratoFrete();
        }
    } else if (transbordo.SituacaoTransbordo == EnumSituacaoTransbordo.AgContratoFrete) {
        PossuiContratoFrete(transbordo.ContratoFrete);
        _cargaTransbordo.CancelarTransbordo.visible(false);
    }
}


function PossuiContratoFrete(contratoFrete) {
    $("#li_contrato" + strKnoutCargaTransbordo).show();
    preecherDadosTransbordoContrato(contratoFrete);
}

function NaoPossuiContratoFrete() {
    $("#li_contrato" + strKnoutCargaTransbordo).hide();
}


function limparCamposTransbordo() {
    _cargaTransbordo.MDFe.visible(false);
    LimparCampos(_cargaTransbordo);
    _cargaTransbordo.CancelarTransbordo.visible(false);
    _cargaTransbordo.Voltar.visible(false);
    _cargaTransbordo.GerarTransbordo.visible(true);

    _cargaTransbordo.DataTransbordo.enable(true);
    _cargaTransbordo.Veiculo.enable(true);
    _cargaTransbordo.Motorista.enable(true);
    _cargaTransbordo.LocalidadeTransbordo.enable(true);
    _cargaTransbordo.MotivoTransbordo.enable(true);
}

function EtapaTransbordoDesabilitada(e) {
    $("#" + e.EtapaTransbordo.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaTransbordo.idTab + " .step").attr("class", "step");
    e.EtapaTransbordo.eventClick = function (e) { };
}

function EtapaTransbordoLiberada(e) {
    $("#" + e.EtapaTransbordo.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaTransbordo.idTab + " .step").attr("class", "step yellow");
    e.EtapaTransbordo.eventClick = verificarTransbordoClick;
}

function EtapaTransbordoAguardando(e) {
    $("#" + e.EtapaTransbordo.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaTransbordo.idTab + " .step").attr("class", "step yellow");
    e.EtapaTransbordo.eventClick = verificarTransbordoClick;
}


function EtapaTransbordoAprovada(e) {
    $("#" + e.EtapaTransbordo.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaTransbordo.idTab + " .step").attr("class", "step green");

    e.EtapaTransbordo.eventClick = verificarTransbordoClick;
}

function EtapaTransbordoProblema(e) {
    $("#" + e.EtapaTransbordo.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaTransbordo.idTab + " .step").attr("class", "step red");
    EtapaDadosTransportadorDesabilitada(e);
    e.EtapaTransbordo.eventClick = verificarTransbordoClick;
}

function EtapaTransbordoEdicaoDesabilitada(e) {
    e.EtapaFreteEmbarcador.enable(false);
    e.AutorizarEmissaoDocumentos.enable(false);
    e.EtapaTransbordo.eventClick = null;
}


