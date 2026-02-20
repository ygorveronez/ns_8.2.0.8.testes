/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="PreCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaFluxoColetaEntrega;
var _fluxoAtual;

var _EtapaFluxoColetaEntrega = [
    { text: "Todas", value: EnumEtapaFluxoColetaEntrega.Todas },
    { text: "Ag Senha GA", value: EnumEtapaFluxoColetaEntrega.AgSenha },
    { text: "Veículo Alocado", value: EnumEtapaFluxoColetaEntrega.VeiculoAlocado },
    { text: "Saída da CD", value: EnumEtapaFluxoColetaEntrega.SaidaCD },
    { text: "Integração", value: EnumEtapaFluxoColetaEntrega.Integracao },
    { text: "Chegada ao Fornecedor", value: EnumEtapaFluxoColetaEntrega.ChegadaFornecedor },
    { text: "CT-e", value: EnumEtapaFluxoColetaEntrega.CTe },
    { text: "MDF-e", value: EnumEtapaFluxoColetaEntrega.MDFe },
    { text: "CT-e Subcotratação", value: EnumEtapaFluxoColetaEntrega.CTeSubcontratacao },
    { text: "Saída Fornecedor", value: EnumEtapaFluxoColetaEntrega.SaidaFornecedor },
    { text: "Chegada a CD", value: EnumEtapaFluxoColetaEntrega.ChegadaCD },
    { text: "Na CD com Ocorrência", value: EnumEtapaFluxoColetaEntrega.Ocorrencia },
    { text: "Processo Finalizado", value: EnumEtapaFluxoColetaEntrega.Finalizado }
];

var PesquisaFluxoColetaEntrega = function () {

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.EtapaFluxoColetaEntrega = PropertyEntity({ val: ko.observable(EnumEtapaFluxoColetaEntrega.Todas), options: _EtapaFluxoColetaEntrega, text: "Situação: ", def: EnumEtapaFluxoColetaEntrega.Todas });
    
    this.ExibirCargasCanceladas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Exibir fluxo incluido as cargas canceladas?",def: false });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarFluxoCarga(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
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


function loadFluxoColetaEntrega() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            loadEtapasFluxoGestao(function () {
                loadEtapaAgSenha();
                loadEtapaVeiculoAlocado();
                loadEtapaProcessoFinalizado();
                loadEtapaChegadaFornecedor();
                loadEtapaSaidaCD();
                LoadEtapaIntegracaoFluxoOcorrencia();
                loadEtapaChegadaCD();
                loadEtapaSaidaFornecedor();
                loadEtapaAgPendenciaAlocarVeiculo();
                AuditoriaFluxoColetaEntrega();
                LoadConexaoSignalRFluxoColetaEntrega();
                BuscarFilialPadrao();
                _pesquisaFluxoColetaEntrega = new PesquisaFluxoColetaEntrega();
                KoBindings(_pesquisaFluxoColetaEntrega, "knoutPesquisaFluxoColetaEntrega");
                new BuscarFilial(_pesquisaFluxoColetaEntrega.Filial);
            });
        });
    });
}

function atualizarFluxoColetaEntrega() {
    if (_fluxoAtual != null) {
        var data = { Codigo: _fluxoAtual.Codigo.val() }
        executarReST("FluxoColetaEntrega/BuscarPorCodigo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    PreencherFluxoColetaEntrega(_fluxoAtual, arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function BuscarFilialPadrao() {
    executarReST("DadosPadrao/ObterFilial", {}, function (r) {
        if (r.Success && r.Data) {
            RetornoConsultaFilial(r.Data);
            pesquisarFluxoCarga(0, false);
        }
    });
}
function RetornoConsultaFilial(dados) {
    _pesquisaFluxoColetaEntrega.Filial.val(dados.Descricao);
    _pesquisaFluxoColetaEntrega.Filial.codEntity(dados.Codigo);
}



function pesquisarFluxoCarga(page, paginou) {
    var itensPorPagina = 10;
    if (_pesquisaFluxoColetaEntrega.Filial.codEntity() > 0) {
        var data = {
            inicio: itensPorPagina * (page - 1),
            limite: itensPorPagina,
            Filial: _pesquisaFluxoColetaEntrega.Filial.codEntity(),
            DataInicial: _pesquisaFluxoColetaEntrega.DataInicial.val(),
            DataFinal: _pesquisaFluxoColetaEntrega.DataFinal.val(),
            ExibirCargasCanceladas: _pesquisaFluxoColetaEntrega.ExibirCargasCanceladas.val(),
            EtapaFluxoColetaEntrega: _pesquisaFluxoColetaEntrega.EtapaFluxoColetaEntrega.val(),
            CodigoCargaEmbarcador: _pesquisaFluxoColetaEntrega.CodigoCargaEmbarcador.val()
        };
        executarReST("FluxoColetaEntrega/ObterFluxoColetaEntrega", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    $("#fdsFluxoGestaoPatio").html("");
                    for (var i = 0; i < arg.Data.length; i++) {
                        var fluxoCarregamento = arg.Data[i];
                        if (fluxoCarregamento.Etapas.length > 0) {
                            var etapaFluxoColetaEntrega = new EtapaFluxoColetaEntrega();
                            var resultado = 100 / fluxoCarregamento.Etapas.length;
                            etapaFluxoColetaEntrega.TamanhoEtapa.val(resultado.toString().replace(",", ".") + "%");
                            PreencherFluxoColetaEntrega(etapaFluxoColetaEntrega, fluxoCarregamento);
                            var idFluxo = fluxoCarregamento.Codigo + "_FluxoColetaEntrega"
                            $("#fdsFluxoGestaoPatio").append(_HTMLEtapasFluxoColetaEntrega.replace(/#idFluxoColetaEntrega/g, idFluxo));
                            KoBindings(etapaFluxoColetaEntrega, idFluxo);
                        }
                    }
                    if (!paginou) {
                        if (arg.QuantidadeRegistros > 0) {
                            $("#divPaginationFluxoColetaEntrega").html('<ul style="float:right" id="paginacaoFluxoColetaEntrega" class="pagination"></ul>');
                            var paginas = Math.ceil((arg.QuantidadeRegistros / itensPorPagina));
                            $('#paginacaoFluxoColetaEntrega').twbsPagination({
                                first: 'Primeiro',
                                prev: 'Anterior',
                                next: 'Próximo',
                                last: 'Último',
                                totalPages: paginas,
                                visiblePages: 5,
                                onPageClick: function (event, page) {
                                    pesquisarFluxoCarga(page, true);
                                }
                            });
                        } else {
                            $("#divPaginationFluxoColetaEntrega").html('<span>Nenhum Registro Encontrado</span>');
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })

    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório informar a filial");
    }

}

function PreencherFluxoColetaEntrega(etapaFluxoColetaEntrega, fluxoCarregamento) {
    etapaFluxoColetaEntrega.Codigo.val(fluxoCarregamento.Codigo);
    etapaFluxoColetaEntrega.Placas.val(fluxoCarregamento.Placas);
    etapaFluxoColetaEntrega.NumeroCarregamento.val(fluxoCarregamento.NumeroCarregamento);
    etapaFluxoColetaEntrega.Carga.val(fluxoCarregamento.Carga);
    etapaFluxoColetaEntrega.Destinatario.val(fluxoCarregamento.Destinatario);
    etapaFluxoColetaEntrega.Remetente.val(fluxoCarregamento.Remetente);
    etapaFluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega.val(fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega);
    etapaFluxoColetaEntrega.EtapaAtual.val(fluxoCarregamento.EtapaAtual);

    etapaFluxoColetaEntrega.DataAgSenha.val(fluxoCarregamento.DataAgSenha);
    etapaFluxoColetaEntrega.DataAgPendenciaAlocarVeiculo.val(fluxoCarregamento.DataAgPendenciaAlocarVeiculo);
    etapaFluxoColetaEntrega.DataVeiculoAlocado.val(fluxoCarregamento.DataVeiculoAlocado);
    etapaFluxoColetaEntrega.DataSaidaCD.val(fluxoCarregamento.DataSaidaCD);
    etapaFluxoColetaEntrega.DataIntegracao.val(fluxoCarregamento.DataIntegracao);
    etapaFluxoColetaEntrega.DataChegadaFornecedor.val(fluxoCarregamento.DataChegadaFornecedor);
    etapaFluxoColetaEntrega.DataEmissaoMDFe.val(fluxoCarregamento.DataEmissaoMDFe);
    etapaFluxoColetaEntrega.DataEmissaoCTe.val(fluxoCarregamento.DataEmissaoCTe);
    etapaFluxoColetaEntrega.DataEmissaoCTeSubContratacao.val(fluxoCarregamento.DataEmissaoCTeSubContratacao);
    etapaFluxoColetaEntrega.DataSaidaFornecedor.val(fluxoCarregamento.DataSaidaFornecedor);
    etapaFluxoColetaEntrega.DataChegadaCD.val(fluxoCarregamento.DataChegadaCD);
    etapaFluxoColetaEntrega.DataAgOcorrencia.val(fluxoCarregamento.DataAgOcorrencia);
    etapaFluxoColetaEntrega.DataFinalizacao.val(fluxoCarregamento.DataFinalizacao);

    etapaFluxoColetaEntrega.Agendamento.val(fluxoCarregamento.Agendamento);
    etapaFluxoColetaEntrega.NumeroPedido.val(fluxoCarregamento.NumeroPedido);
    etapaFluxoColetaEntrega.SenhaCarregamento.val(fluxoCarregamento.SenhaCarregamento);
    etapaFluxoColetaEntrega.NumeroPedidoCliente.val(fluxoCarregamento.NumeroPedidoCliente);
    etapaFluxoColetaEntrega.NumeroSM.val(fluxoCarregamento.NumeroSM);
    if (fluxoCarregamento.SituacaoChamado == "" || fluxoCarregamento.SituacaoChamado == null)
        etapaFluxoColetaEntrega.SituacaoChamado.visible(false);
    else
        etapaFluxoColetaEntrega.SituacaoChamado.visible(true);

    etapaFluxoColetaEntrega.SituacaoChamado.val(fluxoCarregamento.SituacaoChamado);
    etapaFluxoColetaEntrega.CodigoCD.val(fluxoCarregamento.CodigoCD);
    etapaFluxoColetaEntrega.Motorista.val(fluxoCarregamento.Motorista);
    etapaFluxoColetaEntrega.Coleta.val(fluxoCarregamento.Coleta);
    
    if (fluxoCarregamento.DataAgSenha != "")
        etapaFluxoColetaEntrega.DataAgSenha.visible(true);

    if (fluxoCarregamento.DataAgPendenciaAlocarVeiculo != "")
        etapaFluxoColetaEntrega.DataAgPendenciaAlocarVeiculo.visible(true);

    if (fluxoCarregamento.DataVeiculoAlocado != "")
        etapaFluxoColetaEntrega.DataVeiculoAlocado.visible(true);

    if (fluxoCarregamento.DataSaidaCD != "")
        etapaFluxoColetaEntrega.DataSaidaCD.visible(true);

    if (fluxoCarregamento.DataIntegracao != "")
        etapaFluxoColetaEntrega.DataIntegracao.visible(true);

    if (fluxoCarregamento.DataChegadaFornecedor != "")
        etapaFluxoColetaEntrega.DataChegadaFornecedor.visible(true);

    if (fluxoCarregamento.DataEmissaoCTe != "")
        etapaFluxoColetaEntrega.DataEmissaoCTe.visible(true);

    if (fluxoCarregamento.DataEmissaoMDFe != "")
        etapaFluxoColetaEntrega.DataEmissaoMDFe.visible(true);

    if (fluxoCarregamento.DataEmissaoCTeSubContratacao != "")
        etapaFluxoColetaEntrega.DataEmissaoCTeSubContratacao.visible(true);

    if (fluxoCarregamento.DataSaidaFornecedor != "")
        etapaFluxoColetaEntrega.DataSaidaFornecedor.visible(true);

    if (fluxoCarregamento.DataChegadaCD != "")
        etapaFluxoColetaEntrega.DataChegadaCD.visible(true);

    if (fluxoCarregamento.DataAgOcorrencia != "")
        etapaFluxoColetaEntrega.DataAgOcorrencia.visible(true);

    if (fluxoCarregamento.DataFinalizacao != "")
        etapaFluxoColetaEntrega.DataFinalizacao.visible(true);


    for (var j = 0; j < fluxoCarregamento.Etapas.length; j++) {
        var etapaDoFluxo = fluxoCarregamento.Etapas[j];
        
        switch (etapaDoFluxo.EtapaFluxoColetaEntrega) {
            case EnumEtapaFluxoColetaEntrega.AgSenha:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaAgSenha, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataAgSenha.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaAgSenha.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.PendenciaAlocarVeiculo:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaAgPendenciaAlocarVeiculo, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataAgPendenciaAlocarVeiculo.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaAgPendenciaAlocarVeiculo.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.VeiculoAlocado:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaVeiculoAlocado, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataVeiculoAlocado.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaVeiculoAlocado.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.SaidaCD:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaSaidaCD, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataSaidaCD.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaSaidaCD.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.Integracao:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaIntegracao, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataIntegracao.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaIntegracao.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.ChegadaFornecedor:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaChegadaFornecedor, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataChegadaFornecedor.val(), fluxoCarregamento.Coleta);
                etapaFluxoColetaEntrega.EtapaChegadaFornecedor.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.CTe:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaEmissaoCTe, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataEmissaoCTe.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaEmissaoCTe.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.MDFe:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaEmissaoMDFe, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataEmissaoMDFe.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaEmissaoMDFe.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.CTeSubcontratacao:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaEmissaoCTeSubContratacao, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataEmissaoCTeSubContratacao.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaEmissaoCTeSubContratacao.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.SaidaFornecedor:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaSaidaFornecedor, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataSaidaFornecedor.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaSaidaFornecedor.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.ChegadaCD:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaChegadaCD, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataChegadaCD.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaChegadaCD.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.Ocorrencia:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaAgOcorrencia, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataAgOcorrencia.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaAgOcorrencia.visible(true);
                break;
            case EnumEtapaFluxoColetaEntrega.Finalizado:
                validarEtapaFluxo(etapaFluxoColetaEntrega.EtapaFinalizacao, fluxoCarregamento.EtapaAtual, j, fluxoCarregamento.SituacaoEtapaFluxoColetaEntrega, etapaFluxoColetaEntrega.DataFinalizacao.val(), fluxoCarregamento.Agendamento);
                etapaFluxoColetaEntrega.EtapaFinalizacao.visible(true);
                break;
            default:
                break;
        }
    }
}

function validarEtapaFluxo(etapa, etapaAtual, etapaFluxo, situacao, dataEtapa, dataColeta) {
    if (etapaAtual > etapaFluxo) {
        if (dataEtapa != "" && dataColeta != "") {
            var dateEtapa = moment(dataEtapa).format("DD/MM/YYYY HH:mm");
            var dateColeta = moment(dataColeta).format("DD/MM/YYYY HH:mm");
            if (moment(dateEtapa).isAfter(dateColeta))
                SetarEtapaFluxoProblema(etapa);
            else
                SetarEtapaFluxoAprovada(etapa);
        } else {
            SetarEtapaFluxoAprovada(etapa);
        }
    } else if (etapaAtual == etapaFluxo) {
        if (situacao == EnumSituacaoEtapaFluxoColetaEntrega.Aguardando)
            SetarEtapaFluxoAguardando(etapa);
        else if (situacao == EnumSituacaoEtapaFluxoColetaEntrega.Rejeitado)
            SetarEtapaFluxoProblema(etapa);
        else {
            if (dataEtapa != "" && dataColeta != "") {
                var dateEtapa = moment(dataEtapa).format("DD/MM/YYYY HH:mm");
                var dateColeta = moment(dataColeta).format("DD/MM/YYYY HH:mm");
                if (moment(dateEtapa).isAfter(dateColeta))
                    SetarEtapaFluxoProblema(etapa);
                else
                    SetarEtapaFluxoAprovada(etapa);
            } else {
                SetarEtapaFluxoAprovada(etapa);
            }
        }
    } else
        SetarEtapaFluxoDesabilitada(etapa);
}

function ObterDetalhesCargaFluxoClick(e) {
    ObterDetalhesCargaFluxo(e.Carga.val());
}

function ObterDetalhesCargaFluxo(codigo) {
    var data = { Carga: codigo };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;                
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
