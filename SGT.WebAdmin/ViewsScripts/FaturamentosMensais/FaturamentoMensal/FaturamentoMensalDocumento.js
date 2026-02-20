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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="FaturamentoMensalEtapa.js" />
/// <reference path="FaturamentoMensalBoleto.js" />
/// <reference path="FaturamentoMensal.js" />
/// <reference path="FaturamentoMensalEnvioEmail.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoDocumentos;
var _gridGeracaoDocumentos;

var GeracaoDocumentos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaFaturamentoMensal.Etapa2), def: EnumEtapaFaturamentoMensal.Etapa2, getType: typesKnockout.int });

    this.FaturamentoParaGeracaoDocumento = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.NaoGerarDocumento = PropertyEntity({ val: ko.observable(false), text: "Não gerar documento fiscal de todos faturamentos?", def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarDocumentos = PropertyEntity({ eventClick: GerarDocumentosClick, type: types.event, text: "Gerar Documentos", visible: ko.observable(false), enable: ko.observable(true) });
    this.AtualizarDocumentos = PropertyEntity({ eventClick: AtualizarDocumentosClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.AutorizarDocumentos = PropertyEntity({ eventClick: AutorizarDocumentosClick, type: types.event, text: "Autorizar Documentos", visible: ko.observable(false), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaFaturamentoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaFaturamentoNaoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TotalizadorValorDocumento = PropertyEntity({ text: "Total Documento(s): ", getType: typesKnockout.string, val: ko.observable("0,00"), enable: ko.observable(true), visible: true });

    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoDocumentoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarFaturamentoMensalClick, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparCamposFaturamentoMensal, type: types.event, text: "Limpar/Novo Faturamento", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadGeracaoDocumentos() {
    _geracaoDocumentos = new GeracaoDocumentos();
    KoBindings(_geracaoDocumentos, "knockoutGeracaoDocumentos");
}

function AtualizarProgressDocumentos(percentual, codigoFaturamentoMensal) {
    if (_geracaoDocumentos.Codigo.val() == codigoFaturamentoMensal) {
        SetarPercentualProcessamentoDocumentos(percentual);

        $("#fdsDocumentos").hide();
        $("#fdsPercentualDocumentos").show();

        _geracaoDocumentos.Proximo.enable(false);
        _geracaoDocumentos.Cancelar.enable(false);
    }
}

function SetarPercentualProcessamentoDocumentos(percentual) {
    finalizarRequisicao();
    var strPercentual = parseInt(percentual) + "%";
    _geracaoDocumentos.PercentualProcessado.val(strPercentual);
    $("#" + _geracaoDocumentos.PercentualProcessado.id).css("width", strPercentual)
}

function VerificarSeDocumentosNotificadaEstaSelecionada(codigoFaturamentoMensal) {
    if (_geracaoDocumentos.Codigo.val() == codigoFaturamentoMensal) {

        $("#fdsDocumentos").show();
        $("#fdsPercentualDocumentos").hide();
        _geracaoDocumentos.Proximo.enable(true);
        _geracaoDocumentos.Cancelar.enable(true);

        SetarPercentualProcessamentoDocumentos(0);

        buscarFaturamentoMensalDocumento();
    }
}

function CancelarFaturamentoMensalClick(e, sender) {
    if (_geracaoDocumentos == undefined || _geracaoDocumentos.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Selecione um faturamento para cancelar o mesmo.");
        return;
    }

    exibirConfirmacao("Confirmação", "ATENÇÃO! Só será possível cancelar o faturamento se não houver títulos gerados, deseja realmente cancelar?", function () {
        var data = { Codigo: _geracaoDocumentos.Codigo.val() };
        executarReST("FaturamentoMensal/CanelarFaturamentoMensal", data, function (arg) {
            if (arg.Success) {
                _selecaoFaturamento.Status.val(EnumStatusFaturamentoMensal.Cancelado);
                limparCamposFaturamentoMensal();
                buscarFaturamentoMensal();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    });
}

function ProximoGeracaoDocumentoClick(e, sender) {
    if (_geracaoDocumentos == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Retorne a etapa anterior para a geração dos documentos.");
        return;
    }

    var data = { Codigo: _geracaoDocumentos.Codigo.val() };
    executarReST("FaturamentoMensal/IniciarGeracaoBoleto", data, function (arg) {
        if (arg.Success) {
            $("#knockoutGeracaoBoleto").show();
            _selecaoFaturamento.Status.val(EnumStatusFaturamentoMensal.DocumentosAutorizados);
            _etapaAtual = 3;
            $("#" + _etapaFaturamentoMensal.Etapa3.idTab).click();
            buscarFaturamentoMensalBoleto();

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa geração de documentos concluída, siga a etapa 3.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function AtualizarDocumentosClick(e, sender) {
    buscarFaturamentoMensalDocumento();
}

function AutorizarDocumentosClick(e, sender) {
    var data = { Codigo: _geracaoDocumentos.Codigo.val() };

    SetarPercentualProcessamentoDocumentos(0);
    AtualizarProgressDocumentos(0, _geracaoDocumentos.Codigo.val());

    executarReST("FaturamentoMensal/AutorizarDocumentosFaturamentoMensal", data, function (arg) {
        if (arg.Success) {
            //VerificarSeDocumentosNotificadaEstaSelecionada(_geracaoDocumentos.Codigo.val());
            //buscarFaturamentoMensalDocumento();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de autorização dos documentos iniciado.");
        } else {
            //VerificarSeDocumentosNotificadaEstaSelecionada(_geracaoDocumentos.Codigo.val());
            //buscarFaturamentoMensalDocumento();
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function GerarDocumentosClick(e, sender) {
    if (_geracaoDocumentos == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Geração de Documentos", "Retorne a etapa anterior para a geração dos documentos.");
        return;
    }
    _geracaoDocumentos.ListaFaturamentoSelecionados.val("");
    _geracaoDocumentos.ListaFaturamentoNaoSelecionados.val("");

    var documentosSelecionados = _gridGeracaoDocumentos.ObterMultiplosSelecionados();
    var documentosNaoSelecionados = _gridGeracaoDocumentos.ObterMultiplosNaoSelecionados();

    if (documentosSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(documentosSelecionados, function (i, faturamento) {
            var obj = new Object();
            obj.Codigo = faturamento.Codigo;
            obj.CodigoFaturamentoCliente = faturamento.CodigoFaturamentoCliente;
            obj.CodigoFaturamentoClienteServico = faturamento.CodigoFaturamentoClienteServico;
            obj.CodigoConfiguracaoBanco = faturamento.CodigoConfiguracaoBanco;
            obj.CodigoTitulo = faturamento.CodigoTitulo;
            obj.CodigoNotaFiscal = faturamento.CodigoNotaFiscal;
            obj.CodigoNotaFiscalServico = faturamento.CodigoNotaFiscalServico;
            obj.Status = faturamento.Status;
            obj.Pessoa = faturamento.Pessoa;
            obj.GrupoFaturamento = faturamento.GrupoFaturamento;
            obj.Banco = faturamento.Banco;
            obj.DiaFaturamento = faturamento.DiaFaturamento;
            obj.DataVencimento = faturamento.DataVencimento;
            obj.NumeroBoleto = faturamento.NumeroBoleto;
            obj.NumeroNota = faturamento.NumeroNota;
            obj.NumeroNotaServico = faturamento.NumeroNotaServico;
            obj.Valor = faturamento.Valor;
            obj.Observacao = faturamento.Observacao;

            dataGrid.push(obj);
        });
        _geracaoDocumentos.ListaFaturamentoSelecionados.val(JSON.stringify(dataGrid));
    }

    if (documentosNaoSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(documentosNaoSelecionados, function (j, faturamento) {
            var obj = new Object();
            obj.Codigo = faturamento.Codigo;
            obj.CodigoFaturamentoCliente = faturamento.CodigoFaturamentoCliente;
            obj.CodigoFaturamentoClienteServico = faturamento.CodigoFaturamentoClienteServico;
            obj.CodigoConfiguracaoBanco = faturamento.CodigoConfiguracaoBanco;
            obj.CodigoTitulo = faturamento.CodigoTitulo;
            obj.CodigoNotaFiscal = faturamento.CodigoNotaFiscal;
            obj.CodigoNotaFiscalServico = faturamento.CodigoNotaFiscalServico;
            obj.Status = faturamento.Status;
            obj.Pessoa = faturamento.Pessoa;
            obj.GrupoFaturamento = faturamento.GrupoFaturamento;
            obj.Banco = faturamento.Banco;
            obj.DiaFaturamento = faturamento.DiaFaturamento;
            obj.DataVencimento = faturamento.DataVencimento;
            obj.NumeroBoleto = faturamento.NumeroBoleto;
            obj.NumeroNota = faturamento.NumeroNota;
            obj.NumeroNotaServico = faturamento.NumeroNotaServico;
            obj.Valor = faturamento.Valor;
            obj.Observacao = faturamento.Observacao;

            dataGrid.push(obj);
        });
        _geracaoDocumentos.ListaFaturamentoNaoSelecionados.val(JSON.stringify(dataGrid));
    }

    var data = { ListaDocumentosSelecionados: _geracaoDocumentos.ListaFaturamentoSelecionados.val(), ListaDocumentosNaoSelecionados: _geracaoDocumentos.ListaFaturamentoNaoSelecionados.val(), Codigo: _geracaoDocumentos.Codigo.val(), NaoGerarDocumento: _geracaoDocumentos.NaoGerarDocumento.val() };
    executarReST("FaturamentoMensal/GerarDocumentosFaturamentoMensal", data, function (arg) {
        if (arg.Success) {
            buscarFaturamentoMensalDocumento();

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Geração dos documentos concluída.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}

//*******MÉTODOS*******

function buscarFaturamentoMensalDocumento(faturamentoSelecionado) {

    if (_geracaoDocumentos.Codigo.val() > 0) {

        $("#fdsDocumentos").show();
        $("#fdsPercentualDocumentos").hide();
        _geracaoDocumentos.Proximo.enable(true);
        _geracaoDocumentos.Cancelar.enable(true);

        var data = { Codigo: _geracaoDocumentos.Codigo.val() };
        executarReST("FaturamentoMensal/BuscarFaturamentoSelecionado", data, function (arg) {
            if (arg.Success) {
                var naoselecionados = new Array();

                $.each(arg.Data, function (i, obj) {
                    var objCTe = {
                        DT_RowId: obj.Codigo, Codigo: obj.Codigo,
                        CodigoFaturamentoCliente: obj.CodigoFaturamentoCliente,
                        CodigoFaturamentoClienteServico: obj.CodigoFaturamentoClienteServico,
                        CodigoConfiguracaoBanco: obj.CodigoConfiguracaoBanco,
                        CodigoTitulo: obj.CodigoTitulo,
                        CodigoNotaFiscal: obj.CodigoNotaFiscal,
                        CodigoNotaFiscalServico: obj.CodigoNotaFiscalServico,
                        Status: obj.Status,
                        Pessoa: obj.Pessoa,
                        GrupoFaturamento: obj.GrupoFaturamento,
                        Banco: obj.Banco,
                        DiaFaturamento: obj.DiaFaturamento,
                        DataVencimento: obj.DataVencimento,
                        NumeroBoleto: obj.NumeroBoleto,
                        NumeroNota: obj.NumeroNota,
                        NumeroNotaServico: obj.NumeroNotaServico,
                        Valor: obj.Valor,
                        Observacao: obj.Observacao
                    };

                    naoselecionados.push(objCTe);
                });

                var somenteLeitura = false;
                _geracaoDocumentos.AtualizarDocumentos.visible(true);
                _geracaoDocumentos.AutorizarDocumentos.visible(true);
                _geracaoDocumentos.GerarDocumentos.visible(true);
                _geracaoDocumentos.SelecionarTodos.visible(false);
                _geracaoDocumentos.SelecionarTodos.val(false);

                var multiplaescolha = {
                    basicGrid: null,
                    eventos: function () { },
                    selecionados: naoselecionados,
                    naoSelecionados: new Array(),
                    SelecionarTodosKnout: _geracaoDocumentos.SelecionarTodos,
                    somenteLeitura: somenteLeitura,
                }

                var configExportacao = {
                    url: "FaturamentoMensal/ExportarPesquisaFaturamentoMensal",
                    titulo: "Documentos Faturamento Mensal"
                };

                _gridGeracaoDocumentos = new GridView(_geracaoDocumentos.FaturamentoParaGeracaoDocumento.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _geracaoDocumentos, null, null, 50, null, null, null, multiplaescolha, null, null, configExportacao);
                _gridGeracaoDocumentos.CarregarGrid();
                CalcularTotalDocumentos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    } else
        $("#knockoutGeracaoDocumentos").hide();
}

function limparCamposFaturamentoMensalDocumento() {
    LimparCampos(_geracaoDocumentos);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function CalcularTotalDocumentos() {
    var documentos = _gridGeracaoDocumentos.ObterMultiplosSelecionados();
    var valorTotal = 0.0;

    if (documentos.length > 0) {
        $.each(documentos, function (i, documento) {
            valorTotal = valorTotal + parseFloat(documento.Valor.toString().replace(".", "").replace(",", "."));
            _geracaoDocumentos.TotalizadorValorDocumento.val(Globalize.format(valorTotal, "n2"));
        });

    } else
        _geracaoDocumentos.TotalizadorValorDocumento.val("0,00");
}