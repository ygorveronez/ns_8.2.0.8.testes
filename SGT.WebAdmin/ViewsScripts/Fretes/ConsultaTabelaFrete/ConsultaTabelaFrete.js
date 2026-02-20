/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Enumeradores/EnumTipoModeloVeicularCarga.js" />
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
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../js/app.config.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridConsultaTabelaFrete;
var _pesquisaConsultaTabelaFrete;
var _CRUDRelatorio;

var _relatorioConsultaTabelaFrete;

var PesquisaConsultaTabelaFrete = function () {
    this.DataInicial = PropertyEntity({ text: "Data inicial de vigência: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data final de vigência: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.DataInicialAlteracao = PropertyEntity({ text: "Data inicial de Alteração: ", getType: typesKnockout.date });
    this.DataFinalAlteracao = PropertyEntity({ text: "Data final de Alteração: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tabela de Frete:", idBtnSearch: guid(), required: true, issue: 78 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58 });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16 });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16 });
    this.RegiaoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Região de Destino:", idBtnSearch: guid(), issue: 110 });
    this.RotaDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Rota de Destino:", idBtnSearch: guid(), issue: 12 });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12 });
    this.RotaOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota de Origem:", idBtnSearch: guid(), issue: 12 });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), issue: 53 });
    this.ModeloReboque = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Reboque:", idBtnSearch: guid(), issue: 44 });
    this.ModeloTracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Tração:", idBtnSearch: guid(), issue: 44 });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 55 });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 55 });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 55 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 121 });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid() });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid() });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamento.obterOpcoesPesquisa(), def: "", text: "Tipo de Pagamento: ", issue: 120 });
    this.TabelaComCargaRealizada = PropertyEntity({ text: "Apenas tabelas com cargas realizadas?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExibirHistoricoAlteracao = PropertyEntity({ text: "Exibir o histórico de alterações?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ComponentesFrete = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.dynamic, getType: typesKnockout.selectMultiple, url: "ComponenteFrete/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, def: new Array(), text: "Compontentes de Frete: ", options: ko.observable(new Array()), issue: 163 });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.SomenteEmVigencia = PropertyEntity({ text: "Somente Tabelas Vigentes.", val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.SituacaoAlteracao = PropertyEntity({ text: "Situação Alteração Tabela Frete", options: EnumSituacaoAlteracaoTabelaFrete.obterOpcoesPesquisaTabelaFrete(), val: ko.observable(EnumSituacaoAlteracaoTabelaFrete.Todas), def: EnumSituacaoAlteracaoTabelaFrete.Todas })
    this.SomenteRegistrosComValores = PropertyEntity({ text: "Somente registros com valor.", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.StatusAceiteValor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Situação do Aceite:", idBtnSearch: guid() });
    this.ContratoTransportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Contrato Transportador:", idBtnSearch: guid() });
    this.CodigoIntegracaoTabelaFreteCliente = PropertyEntity({ text: "Código de Integração: ", issue: 15 });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicialAlteracao.dateRangeLimit = this.DataFinalAlteracao;
    this.DataFinalAlteracao.dateRangeInit = this.DataInicialAlteracao;

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            GerarPreviewClick(e, this);
        }, type: types.event, text: "Consultar", idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparFiltros();
        }, type: types.event, text: "Limpar Filtros", idGrid: guid(), visible: ko.observable(true), icon: ko.observable("fa fa-recycle")
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ConfiguracaoRelatorio = PropertyEntity({
        eventClick: function (e) {
            var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

            if (valido) {
                if (e.ConfiguracaoRelatorio.visibleFade()) {
                    e.ConfiguracaoRelatorio.visibleFade(false);
                    e.ConfiguracaoRelatorio.icon("fal fa-cogs");
                } else {
                    e.ConfiguracaoRelatorio.visibleFade(true);
                    e.ConfiguracaoRelatorio.icon("fal fa-cogs");
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
            }
        }, type: types.event, text: "Configurações", idFade: guid(), icon: ko.observable("fal fa-cogs"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.TabelaFrete.codEntity.subscribe(function (novoValor) {
        trocarTabelaFrete(novoValor);
    });

    this.TabelaFrete.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _pesquisaConsultaTabelaFrete.TabelaFrete.codEntity(0);
    });
}

var CRUDRelatorio = function () {

    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function trocarTabelaFrete(codigoTabelaFrete) {
    $("#divConteudoRelatorio").html("");

    if (_gridConsultaTabelaFrete != null) {
        _gridConsultaTabelaFrete.Destroy();
        _gridConsultaTabelaFrete = null;
    }

    if (codigoTabelaFrete > 0) {
        _gridConsultaTabelaFrete = new GridView(_pesquisaConsultaTabelaFrete.Preview.idGrid, "ConsultaTabelaFrete/Pesquisa", _pesquisaConsultaTabelaFrete);
        _gridConsultaTabelaFrete.SetPermitirEdicaoColunas(true);
        _gridConsultaTabelaFrete.SetQuantidadeLinhasPorPagina(25);

        _relatorioConsultaTabelaFrete = new RelatorioGlobal("ConsultaTabelaFrete/BuscarDadosRelatorio", _gridConsultaTabelaFrete, function () {
            _relatorioConsultaTabelaFrete.loadRelatorio(function () {
                _gridConsultaTabelaFrete.CarregarGrid();

                $("#divConteudoRelatorio").show();

                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visibleFade(true);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visible(false);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().ExibirSumarios.visible(false);
            });

        }, { TabelaFrete: _pesquisaConsultaTabelaFrete.TabelaFrete.codEntity() }, null, _pesquisaConsultaTabelaFrete, false);
    } else {
        $("#" + _pesquisaConsultaTabelaFrete.Preview.idGrid).html("");
        _pesquisaConsultaTabelaFrete.ConfiguracaoRelatorio.visibleFade(false);
        _pesquisaConsultaTabelaFrete.ConfiguracaoRelatorio.icon("fal fa-plus");
    }
}

function loadConsultaTabelaFrete() {
    _pesquisaConsultaTabelaFrete = new PesquisaConsultaTabelaFrete();
    _CRUDRelatorio = new CRUDRelatorio();

    KoBindings(_pesquisaConsultaTabelaFrete, "knockoutPesquisaConsultaTabelaFrete", false);
    KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConsultaTabelaFrete", false);
    new BuscarClientes(_pesquisaConsultaTabelaFrete.Remetente);
    new BuscarClientes(_pesquisaConsultaTabelaFrete.Destinatario);
    new BuscarClientes(_pesquisaConsultaTabelaFrete.Tomador);
    new BuscarGruposPessoas(_pesquisaConsultaTabelaFrete.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLocalidades(_pesquisaConsultaTabelaFrete.LocalidadeDestino);
    new BuscarLocalidades(_pesquisaConsultaTabelaFrete.LocalidadeOrigem);
    new BuscarModelosVeicularesCarga(_pesquisaConsultaTabelaFrete.ModeloReboque, null, null, null, [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Reboque]);
    new BuscarModelosVeicularesCarga(_pesquisaConsultaTabelaFrete.ModeloTracao, null, null, null, [EnumTipoModeloVeicularCarga.Tracao]);
    new BuscarRegioes(_pesquisaConsultaTabelaFrete.RegiaoDestino);
    new BuscarTiposdeCarga(_pesquisaConsultaTabelaFrete.TipoCarga);
    new BuscarTiposOperacao(_pesquisaConsultaTabelaFrete.TipoOperacao);
    new BuscarTabelasDeFrete(_pesquisaConsultaTabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaCliente);
    new BuscarEstados(_pesquisaConsultaTabelaFrete.EstadoDestino);
    new BuscarEstados(_pesquisaConsultaTabelaFrete.EstadoOrigem);
    new BuscarEmpresa(_pesquisaConsultaTabelaFrete.Empresa);
    new BuscarRotasFrete(_pesquisaConsultaTabelaFrete.RotaOrigem);
    new BuscarRotasFrete(_pesquisaConsultaTabelaFrete.RotaDestino);
    new BuscarContratosTransporteFrete(_pesquisaConsultaTabelaFrete.ContratoTransportador);
    new BuscarClientes(_pesquisaConsultaTabelaFrete.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarStatusAssinaturaContrato(_pesquisaConsultaTabelaFrete.StatusAceiteValor);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        _pesquisaConsultaTabelaFrete.Empresa.text("Empresa/Filial:");

    if (!_FormularioSomenteLeitura && !_ConfiguracaoTabelaFrete.NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete) {
        $('#' + _pesquisaConsultaTabelaFrete.Preview.idGrid).on('click', 'tbody td', function (e) {
            e.stopPropagation();
            editCell(this);
        });
    }
}

var codigoItemEdicao = null;
var valorItemEdicao = null;
var tipoValorItemEdicao = null;
var htmlItemEdicao = null;

function editCell(cell) {
    var span = $(cell).find("span")[0];
    var precision = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 6;
    if ($(span).data("codigoItem") != null && $(span).data("tipoValor") != null) {
        codigoItemEdicao = $(span).data("codigoItem");
        tipoValorItemEdicao = $(span).data("tipoValor");

        var idTxt = guid();

        htmlItemEdicao = cell.innerHTML;
        valorItemEdicao = Globalize.parseFloat(cell.innerHTML.split("</span>")[1].trim());
        cell.innerHTML = '<input id="' + idTxt + '" type="text" value="' + Globalize.format(valorItemEdicao, "n" + precision) + '" style="width: 100%; height: 100%;" />';

        $("#" + idTxt).maskMoney(ConfigDecimal({ precision: precision }));

        switch (tipoValorItemEdicao) {
            case EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:

                $("#" + idTxt).attr("maxlength", "5");

                break;
            case EnumTipoCampoValorTabelaFrete.AumentoPercentual:

                $("#" + idTxt).attr("maxlength", "6");

                break;
            case EnumTipoCampoValorTabelaFrete.ValorFixo:
            case EnumTipoCampoValorTabelaFrete.AumentoValor:

                $("#" + idTxt).attr("maxlength", "10");

                break;
            default:
                break;
        }

        $("#" + idTxt).focus();

        $("#" + idTxt).focusout(function () {
            var valor = Globalize.parseFloat($("#" + idTxt).val());

            if (isNaN(valor) || valor == valorItemEdicao) {
                $(this).closest("td").html(htmlItemEdicao);
                htmlItemEdicao = null;
                valorItemEdicao = null;
                tipoValorItemEdicao = null;
                codigoItemEdicao = null;
            } else {
                exibirConfirmacao("Atenção!", "A consulta será recarregada, pois esta alteração pode afetar mais de uma linha na exibição. Deseja continuar?", function () {
                    executarReST("ConsultaTabelaFrete/SalvarValorItem", { CodigoItem: codigoItemEdicao, ValorItem: $("#" + idTxt).val() }, function (r) {
                        if (r.Success) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valores atualizados com sucesso!");
                            _gridConsultaTabelaFrete.CarregarGrid();
                        } else {
                            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
                            $("#" + idTxt).closest("td").html(htmlItemEdicao);
                            htmlItemEdicao = null;
                            valorItemEdicao = null;
                            tipoValorItemEdicao = null;
                            codigoItemEdicao = null;
                        }
                    });
                }, function () {
                    $("#" + idTxt).closest("td").html(htmlItemEdicao);
                    htmlItemEdicao = null;
                    valorItemEdicao = null;
                    tipoValorItemEdicao = null;
                    codigoItemEdicao = null;
                });
            }
        });
    }
}

function GerarPreviewClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _gridConsultaTabelaFrete.CarregarGrid();
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioPDFClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("ConsultaTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioExcelClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("ConsultaTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function SalvarConfiguracaoImpressao(e, sender) {
    if (_pesquisaConsultaTabelaFrete.TabelaFrete.codEntity() > 0) {
        var knoutRelatorio = _relatorioConsultaTabelaFrete.obterKnoutRelatorio();

        knoutRelatorio.Padrao.val(false);
        knoutRelatorio.Grid.val(_gridConsultaTabelaFrete.GetGrid());

        var data = { Relatorio: JSON.stringify(obterObjetoRelatorioParaConsulta(knoutRelatorio)), TabelaFrete: _pesquisaConsultaTabelaFrete.TabelaFrete.codEntity() };
        executarReST("ConsultaTabelaFrete/SalvarConfiguracaoRelatorio", data, function (r) {
            if (r.Success && r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Configuração de impressão salva com sucesso.");
            } else if (r.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Campos obrigatórios!", "É necessário selecionar uma tabela de frete para salvar a configuração de impressão.");
    }
}

function LimparFiltros() {
    LimparCampos(_pesquisaConsultaTabelaFrete);
}

function buscarVigenciaAtual(ko) {
    executarReST("TabelaFrete/BuscarVigenciaAtual", { TabelaFrete: ko.TabelaFrete.codEntity(), Empresa: ko.Empresa.codEntity() }, function (arg) {
        if (arg.Success && arg.Data != null)
            retornoBuscaVigencias(ko, arg.Data);
    });
}
function retornoTabelaFrete(e) {

    _pesquisaConsultaTabelaFrete.TabelaFrete.codEntity(e.Codigo);
    _pesquisaConsultaTabelaFrete.TabelaFrete.val(e.Descricao);
    buscarVigenciaAtual(_pesquisaConsultaTabelaFrete);
}
function retornoBuscaVigencias(ko, arg) {
    _pesquisaConsultaTabelaFrete.DataInicial.val(arg.DataInicial);
    _pesquisaConsultaTabelaFrete.DataFinal.val(arg.DataFinal);

}