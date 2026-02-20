/// <reference path="Parametros.js" />
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
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacaoTabelaFrete;
var _crudImportacaoTabelaFrete;
var _tabelaFrete;
var _gridErrosTabelaFrete;

var ImportacaoTabelaFrete = function () {
    var layoutPadrao = !_CONFIGURACAO_TMS.UtilizarLayoutImportacaoGPA;

    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid(), issue: 78 });
    this.Vigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Vigência:", idBtnSearch: guid(), issue: 82, visible: ko.observable(layoutPadrao) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", idBtnSearch: guid(), issue: 121, visible: ko.observable(layoutPadrao) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(layoutPadrao) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.PossuiIntegracaoLBC, text: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC ? "*Transportador:" : "Transportador:"), idBtnSearch: guid(), visible: ko.observable(layoutPadrao) });

    this.ColunaOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna CEP Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaEstadoOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Estado Origem:", visible: ko.observable(layoutPadrao) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(layoutPadrao) });
    this.ColunaDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna CEP Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaEstadoDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Estado Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaClienteDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Destinatário:", visible: ko.observable(layoutPadrao) });
    this.ColunaClienteOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Remetente:", visible: ko.observable(layoutPadrao) });
    this.ColunaRotaDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Rota Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaRotaOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Rota Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaRegiaoDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Região Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaRegiaoOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Região Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaCodigoIntegracao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Código Integração:", visible: ko.observable(layoutPadrao) });
    this.ColunaTransportador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Transportador:", visible: ko.observable(layoutPadrao) });
    this.ColunaParametrosBase = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Parâmetros Base:", visible: ko.observable(layoutPadrao) });
    this.LinhaInicioDados = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int, text: "Linha Início Dados:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPDestinoDiasUteis = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Prazo (Dias Úteis) do CEP Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaTomadorDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tomador:", visible: ko.observable(layoutPadrao) });
    this.ColunaLeadTime = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Prazo (Dias Úteis):", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaTipoOperacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tipo de Operação:", visible: ko.observable(layoutPadrao) });
    this.ColunaTipoCarga = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tipo de Carga:", visible: ko.observable(layoutPadrao) });

    this.ColunaSeg = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Seg:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaTer = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Ter:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaQua = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Qua:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaQui = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Qui:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaSex = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Sex:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaSab = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Sab:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });
    this.ColunaDom = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Dom:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente) });

    this.ColunaPercentualRota = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Percentual Rota:", visible: ko.observable(layoutPadrao) });
    this.ColunaQuantidadeEntregas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Quantidade Entregas:", visible: ko.observable(layoutPadrao) });
    this.ColunaCapacidadeOTM = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Capacidade OTM:", visible: ko.observable(layoutPadrao) });
    this.ColunaDominioOTM = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Domínio OTM:", visible: ko.observable(layoutPadrao) });
    this.ColunaTipoIntegracao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tipo Integração:", visible: ko.observable(layoutPadrao) });
    this.ColunaPontoPlanejamentoTransporte = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Ponto Planejamento Transporte:", visible: ko.observable(layoutPadrao) });
    this.ColunaContratoTransportador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Contrato Transportador:", visible: ko.observable(layoutPadrao) });
    this.ColunaGrupoCarga = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Grupo de Carga:", visible: ko.observable(layoutPadrao) });
    this.ColunaGerenciarCapacidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Gerenciar Capacidade:", visible: ko.observable(layoutPadrao) });
    this.ColunaEstruturaTabela = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Estrutura de Tabela:", visible: ko.observable(layoutPadrao) });
    this.ColunaLeadTimeDias = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Lead Time (Dias):", visible: ko.observable(layoutPadrao) });
    this.ColunaObservacaoInterna = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Observação Interna (não será impressa no CT-e):", visible: ko.observable(layoutPadrao) });

    this.ColunaFronteira = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Fronteira:", visible: ko.observable(layoutPadrao) });
    this.ColunaKMSistema = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "KM:", visible: ko.observable(layoutPadrao) });
    this.ColunaPrioridadeUso = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Prioridade Uso:", visible: ko.observable(layoutPadrao) });
    this.ColunaTransportador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Transportador:", visible: ko.observable(layoutPadrao) });
    this.ColunaCanalEntrega = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Canal de Entrega:", visible: ko.observable(layoutPadrao) });
    this.ColunaCanalVenda = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Canal de Venda:", visible: ko.observable(layoutPadrao) });

    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(layoutPadrao && _CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });
    this.TipoPagamento = PropertyEntity({ text: "Tipo de Pagamento:", options: EnumTipoPagamentoEmissao.obterOpcoes("", "Não selecionado"), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(layoutPadrao) });

    this.FreteValidoParaQualquerOrigem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Válido para qualquer uma das origens", visible: ko.observable(layoutPadrao) });
    this.FreteValidoParaQualquerDestino = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Válido para qualquer um dos destinos", visible: ko.observable(layoutPadrao) });
    this.NaoAtualizarValoresZerados = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não atualizar os valores zerados ou não informados na planilha", visible: ko.observable(layoutPadrao) });

    this.Parametros = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivo:", val: ko.observable("") });

    this.ErrosImportacao = PropertyEntity({ type: types.map, required: false, text: "f", getType: typesKnockout.dynamic, idGrid: guid(), visible: ko.observable(false) });
    this.DownloadRetornoProcessamento = PropertyEntity({ eventClick: downloadRetornoProcessamentoTabelaFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.BaixarRetornoProcessamento, icon: "fal fa-download", visible: ko.observable(false) });
};

var CRUDImportacaoTabelaFrete = function () {
    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.LayoutImportacao = PropertyEntity({ eventClick: layoutImportacaoClick, type: types.event, text: "Layout de Importação", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadImportacaoTabelaFrete() {
    _importacaoTabelaFrete = new ImportacaoTabelaFrete();
    KoBindings(_importacaoTabelaFrete, "knockoutImportacaoTabelaFrete");

    _crudImportacaoTabelaFrete = new CRUDImportacaoTabelaFrete();
    KoBindings(_crudImportacaoTabelaFrete, "knockoutCRUDImportacaoTabelaFrete");

    new BuscarLocalidades(_importacaoTabelaFrete.Origem);
    new BuscarLocalidades(_importacaoTabelaFrete.Destino);
    new BuscarTabelasDeFrete(_importacaoTabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaCliente);
    new BuscarVigenciasTabelaFrete(_importacaoTabelaFrete.Vigencia, _importacaoTabelaFrete.TabelaFrete, retornoBuscaVigencias, true, _importacaoTabelaFrete.Empresa);
    new BuscarTiposOperacao(_importacaoTabelaFrete.TipoOperacao);
    new BuscarTransportadores(_importacaoTabelaFrete.Empresa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _importacaoTabelaFrete.Empresa.text("Empresa/Filial:");
    }

    if (_CONFIGURACAO_TMS.UtilizarLayoutImportacaoGPA)
        $("#knockoutParametros").hide();
    else
        $("#knockoutParametros").show();

    loadParametros();
}

function retornoTabelaFrete(e) {
    limparCamposParametro();
    _importacaoTabelaFrete.Parametros.list = new Array();
    recarregarGridParametros();

    _importacaoTabelaFrete.TabelaFrete.codEntity(e.Codigo);
    _importacaoTabelaFrete.TabelaFrete.val(e.Descricao);

    Global.setarFocoProximoCampo(_importacaoTabelaFrete.Vigencia.id);

    buscarDadosTabelaFrete();
    buscarVigenciaAtual();
}

function buscarDadosTabelaFrete() {
    executarReST("TabelaFrete/BuscarPorCodigo", { Codigo: _importacaoTabelaFrete.TabelaFrete.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _tabelaFrete = arg.Data;

                montarParametros();
            }
        }
    });
}

function buscarVigenciaAtual() {
    executarReST("TabelaFrete/BuscarVigenciaAtual", { TabelaFrete: _importacaoTabelaFrete.TabelaFrete.codEntity() }, function (arg) {
        if (arg.Success)
            if (arg.Data != null)
                retornoBuscaVigencias(arg.Data);
    });
}

function retornoBuscaVigencias(data) {
    _importacaoTabelaFrete.Vigencia.codEntity(data.Codigo);
    _importacaoTabelaFrete.Vigencia.val("De " + data.DataInicial + " até " + data.DataFinal);
    _importacaoTabelaFrete.Vigencia.entityDescription(_importacaoTabelaFrete.Vigencia.val());
}

function importarClick(e, sender) {
    var file = document.getElementById(_importacaoTabelaFrete.Arquivo.id);
    var formData = new FormData();

    formData.append("upload", file.files[0]);

    var dados = {
        TabelaFrete: _importacaoTabelaFrete.TabelaFrete.val() != "" ? _importacaoTabelaFrete.TabelaFrete.codEntity() : 0,
        Vigencia: _importacaoTabelaFrete.Vigencia.val() != "" ? _importacaoTabelaFrete.Vigencia.codEntity() : 0,
        TipoOperacao: _importacaoTabelaFrete.TipoOperacao.val() != "" ? _importacaoTabelaFrete.TipoOperacao.codEntity() : 0,
        Origem: _importacaoTabelaFrete.Origem.val() != "" ? _importacaoTabelaFrete.Origem.codEntity() : 0,
        ColunaCEPOrigem: _importacaoTabelaFrete.ColunaCEPOrigem.val(),
        ColunaOrigem: _importacaoTabelaFrete.ColunaOrigem.val(),
        ColunaEstadoOrigem: _importacaoTabelaFrete.ColunaEstadoOrigem.val(),
        Empresa: _importacaoTabelaFrete.Empresa.val() != "" ? _importacaoTabelaFrete.Empresa.codEntity() : 0,
        ColunaDestino: _importacaoTabelaFrete.ColunaDestino.val(),
        ColunaClienteDestino: _importacaoTabelaFrete.ColunaClienteDestino.val(),
        ColunaClienteOrigem: _importacaoTabelaFrete.ColunaClienteOrigem.val(),
        ColunaRotaDestino: _importacaoTabelaFrete.ColunaRotaDestino.val(),
        ColunaRotaOrigem: _importacaoTabelaFrete.ColunaRotaOrigem.val(),
        ColunaRegiaoDestino: _importacaoTabelaFrete.ColunaRegiaoDestino.val(),
        ColunaRegiaoOrigem: _importacaoTabelaFrete.ColunaRegiaoOrigem.val(),
        ColunaEstadoDestino: _importacaoTabelaFrete.ColunaEstadoDestino.val(),
        ColunaCEPDestino: _importacaoTabelaFrete.ColunaCEPDestino.val(),
        ColunaCEPDestinoDiasUteis: _importacaoTabelaFrete.ColunaCEPDestinoDiasUteis.val(),
        ColunaTomadorDestino: _importacaoTabelaFrete.ColunaTomadorDestino.val(),
        ColunaCodigoIntegracao: _importacaoTabelaFrete.ColunaCodigoIntegracao.val(),
        ColunaTransportador: _importacaoTabelaFrete.ColunaTransportador.val(),
        ColunaParametrosBase: _importacaoTabelaFrete.ColunaParametrosBase.val(),
        FreteValidoParaQualquerOrigem: _importacaoTabelaFrete.FreteValidoParaQualquerOrigem.val(),
        FreteValidoParaQualquerDestino: _importacaoTabelaFrete.FreteValidoParaQualquerDestino.val(),
        NaoAtualizarValoresZerados: _importacaoTabelaFrete.NaoAtualizarValoresZerados.val(),
        Destino: _importacaoTabelaFrete.Destino.val() != "" ? _importacaoTabelaFrete.Destino.codEntity() : 0,
        LinhaInicioDados: _importacaoTabelaFrete.LinhaInicioDados.val(),
        Parametros: JSON.stringify(recursiveListEntity(_importacaoTabelaFrete.Parametros)),
        ColunaPrioridadeUso: _importacaoTabelaFrete.ColunaPrioridadeUso.val(),
        ColunaFronteira: _importacaoTabelaFrete.ColunaFronteira.val(),
        ColunaKMSistema: _importacaoTabelaFrete.ColunaKMSistema.val(),
        Moeda: _importacaoTabelaFrete.Moeda.val(),
        TipoPagamento: _importacaoTabelaFrete.TipoPagamento.val(),
        ColunaLeadTime: _importacaoTabelaFrete.ColunaLeadTime.val(),
        ColunaSeg: _importacaoTabelaFrete.ColunaSeg.val(),
        ColunaTer: _importacaoTabelaFrete.ColunaTer.val(),
        ColunaQua: _importacaoTabelaFrete.ColunaQua.val(),
        ColunaQui: _importacaoTabelaFrete.ColunaQui.val(),
        ColunaSex: _importacaoTabelaFrete.ColunaSex.val(),
        ColunaSab: _importacaoTabelaFrete.ColunaSab.val(),
        ColunaDom: _importacaoTabelaFrete.ColunaDom.val(),
        ColunaCanalEntrega: _importacaoTabelaFrete.ColunaCanalEntrega.val(),
        ColunaPercentualRota: _importacaoTabelaFrete.ColunaPercentualRota.val(),
        ColunaQuantidadeEntregas: _importacaoTabelaFrete.ColunaQuantidadeEntregas.val(),
        ColunaCapacidadeOTM: _importacaoTabelaFrete.ColunaCapacidadeOTM.val(),
        ColunaDominioOTM: _importacaoTabelaFrete.ColunaDominioOTM.val(),
        ColunaTipoIntegracao: _importacaoTabelaFrete.ColunaTipoIntegracao.val(),
        ColunaPontoPlanejamentoTransporte: _importacaoTabelaFrete.ColunaPontoPlanejamentoTransporte.val(),
        ColunaCanalVenda: _importacaoTabelaFrete.ColunaCanalVenda.val(),
        ColunaTipoOperacao: _importacaoTabelaFrete.ColunaTipoOperacao.val(),
        ColunaTipoCarga: _importacaoTabelaFrete.ColunaTipoCarga.val(),
        ColunaContratoTransportador: _importacaoTabelaFrete.ColunaContratoTransportador.val(),
        ColunaGrupoCarga: _importacaoTabelaFrete.ColunaGrupoCarga.val(),
        ColunaGerenciarCapacidade: _importacaoTabelaFrete.ColunaGerenciarCapacidade.val(),
        ColunaEstruturaTabela: _importacaoTabelaFrete.ColunaEstruturaTabela.val(),
        ColunaLeadTimeDias: _importacaoTabelaFrete.ColunaLeadTimeDias.val(),
        ColunaObservacaoInterna: _importacaoTabelaFrete.ColunaObservacaoInterna.val(),
    };

    for (var propriedade in dados) {
        formData.append(propriedade, dados[propriedade]);
    }

    function EnviarFormImportacao() {
        var urlImportacao = _CONFIGURACAO_TMS.UtilizarMetodoImportacaoPorServico ? "ImportacaoTabelaFrete/ImportarPorServico?callback=?" : "ImportacaoTabelaFrete/Importar?callback=?";

        enviarArquivo(urlImportacao, null, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data.length == 0) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso");

                    _importacaoTabelaFrete.ErrosImportacao.visible(false);
                    _importacaoTabelaFrete.DownloadRetornoProcessamento.visible(false);
                }
                else {
                    _importacaoTabelaFrete.ErrosImportacao.visible(true);
                    _importacaoTabelaFrete.DownloadRetornoProcessamento.visible(true);

                    var header = [{ data: "CodigoErro", visible: false },
                    { data: "LinhaErro", title: "Linha", width: "10%", className: "text-align-left" },
                    { data: "DescricaoErro", title: "Erro", width: "90%", className: "text-align-left" },
                    ];
                    _gridErrosTabelaFrete = new BasicDataTable(_importacaoTabelaFrete.ErrosImportacao.idGrid, header, null);
                    _gridErrosTabelaFrete.CarregarGrid(retorno.Data);

                    document.getElementById(_importacaoTabelaFrete.ErrosImportacao.idGrid + "_info").style.display = "none";
                    document.getElementById(_importacaoTabelaFrete.ErrosImportacao.idGrid + "_paginate").style.display = "none";
                    _importacaoTabelaFrete.Arquivo.val("");
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }

    if (_CONFIGURACAO_TMS.UtilizarLayoutImportacaoGPA) {
        enviarArquivo("ImportacaoTabelaFrete/ImportarTabelaCliente?callback=?", dados, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso");
                else {
                    $("#knockoutImportacaoTabelaFrete").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fa-fw fa fa-info"></i><strong>Atenção!</strong> Alguns registros não foram importados:<br/>' + retorno.Msg.replace(/\n/g, "<br />") + '</p>');
                    _importacaoTabelaFrete.Arquivo.val("");
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else {
        //executarReST("ImportacaoTabelaFrete/DadosImportacao", dados, function (retorno) {
        //    if (retorno.Success) {
        //        if (retorno.Data !== false)
        EnviarFormImportacao();
        //        else
        //            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        //    }
        //    else
        //        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        //});
    }
}

function downloadRetornoProcessamentoTabelaFreteClick() {

    let tab_text = "<table border='2px'>";

    let tab = document.getElementById(_importacaoTabelaFrete.ErrosImportacao.idGrid);

    for (var i = 0; i < tab.rows.length; i++) {
        var row = tab.rows[i];

        tab_text += "<tr>"

        for (var j = 0; j < (row.cells.length); j++) {
            var cell = row.cells[j];

            tab_text += "<td>";

            if (cell.children.length > 0 && cell.children[0].tagName == "SELECT") {
                tab_text += $(cell.children[0]).find('option:selected').text();
            } else {
                tab_text += "&nbsp;" + cell.innerHTML + "&nbsp;";
            }

            tab_text += "</td>";
        }

        tab_text += "</tr>";
    }

    tab_text += "</table>";

    let uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="https://www.w3.org/TR/html40/"><meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body>{table}</body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };

    let ctx = { worksheet: "Retorno Processamento Arquivo" || 'Worksheet', table: tab_text };

    let a = document.getElementById("lnkDownloadProcessamento");

    a.download = "Retorno Processamento Arquivo.xls";
    a.href = uri + base64(format(template, ctx));

    a.click();
}

function layoutImportacaoClick() {
    if (_importacaoTabelaFrete.TabelaFrete.codEntity() > 0)
        loadLayoutImportacao();
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Para continuar é necessário selecionar uma tabela de frete");
}

function cancelarClick(e) {
    limparCamposImportacao();
}

//*******MÉTODOS*******

function limparCamposImportacao() {
    //LimparCampos(_importacaoTabelaFrete);
}
