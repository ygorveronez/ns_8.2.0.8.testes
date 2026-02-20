/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/AdiantamentoMotorista.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Consultas/Abastecimento.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _documento;
var _ocorrenciaDocumento;
var _gridDocumentos;
var _gridDescontosAcrescimos;
var _gridOcorrenciasDocumento;
var _gridAdiantamentos;
var _gridAbastecimentos;
var $ocorrenciasDocumento;
var posicaoRetornoDocumento = 0;
var documentoGridRemover;
var dataGridRetorno;

var Documento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Tipo de Ocorrência:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Documentos = PropertyEntity({ type: types.map, required: false, text: "Adicionar manualmente documento(s) emitido(s) ao agregado", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.BuscarDocumentos = PropertyEntity({ eventClick: BuscarDocumentosClick, type: types.map, required: false, text: "Buscar documentos emitido ao agregado", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: this.Documentos.idGrid, enable: ko.observable(true) });
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RecalcularValor = PropertyEntity({ eventClick: RecalcularValorClick, type: types.map, required: false, text: "Recalcular valor de todos os documentos", getType: typesKnockout.dynamic, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Adiantamentos = PropertyEntity({ type: types.map, required: false, text: "Adicionar adiantamento gerado ao agregado", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.Abastecimentos = PropertyEntity({ type: types.map, required: false, text: "Adicionar abastecimento gerado ao agregado", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.DescontosAcrescimos = PropertyEntity({ type: types.map, required: false, text: "Descontos / Acréscimos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Justificativa:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor:", maxlength: 10, enable: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarDescontoAcrescimo = PropertyEntity({ eventClick: AdicionarDescontoAcrescimoClick, type: types.event, text: "Desconto / Acréscimo", visible: ko.observable(true), enable: ko.observable(true) });

    this.TotalPagamento = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(+) Total fretes a pagar terceiro:" });
    this.TotalAdiantamentos = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(-) Total adiantamentos:" });
    this.TotalAbastecimentos = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(-) Total abastecimentos:" });
    this.ValorAcrescimo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(+) Total acréscimos:" });
    this.ValorDesconto = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(-) Total descontos:" });
    this.Saldo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "(=) Saldo a pagar ao terceiro:" });
    this.ValorSaldo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Saldo do Contrato:" });
}

var OcorrenciaDocumento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ocorrencias = PropertyEntity({ type: types.map, required: false, text: "Ocorrências do Documento", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******
function loadDocumento() {
    _documento = new Documento();
    KoBindings(_documento, "knockoutDocumento");

    _ocorrenciaDocumento = new OcorrenciaDocumento();
    KoBindings(_ocorrenciaDocumento, "knockoutOcorrenciaDocumento");
    $ocorrenciasDocumento = $("#divModalOcorrenciaDocumento");

    if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado) {
        _documento.TipoOcorrencia.visible(false);
        _documento.RecalcularValor.visible(false);
        if (!_CONFIGURACAO_TMS.HabilitarLayoutFaturaPagamentoAgregado) {
            _documento.AdicionarDescontoAcrescimo.visible(false);
            _documento.Valor.visible(false);
            _documento.Justificativa.visible(false);
        }
        $("#divAbastecimentos").show();
        $("#divResumo").show();
        $("#divAdiantamentos").show();
    }
    else {
        $("#divAbastecimentos").hide();
        $("#divResumo").hide();
        $("#divAdiantamentos").show();
    }

    new BuscarTipoOcorrencia(_documento.TipoOcorrencia);
    new BuscarJustificativas(_documento.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    CarregarGridDocumentos();
    CarregarGridAdiantamentos();
    CarregarGridAbastecimentos();
    CarregarGridDescontoAcrescimo();
}

function AtualizarProgressDocumentosAgregado(percentual, codigoPagamentoAgregado) {
    if (_pagamento.Codigo.val() == codigoPagamentoAgregado) {
        SetarPercentualProcessamentoDocumentosAgregado(percentual);

        $("#fdsDocumentos").hide();
        $("#fdsPercentualDocumentos").show();

        _CRUDPagamento.Finalizar.enable(false);
    }
}

function SetarPercentualProcessamentoDocumentosAgregado(percentual) {
    finalizarRequisicao();
    var strPercentual = parseInt(percentual) + "%";
    _documento.PercentualProcessado.val(strPercentual);
    $("#" + _documento.PercentualProcessado.id).css("width", strPercentual)
}

function VerificarSeDocumentosagregadoNotificadaEstaSelecionada(codigoPagamentoAgregado) {
    if (_pagamento.Codigo.val() == codigoPagamentoAgregado) {

        $("#fdsDocumentos").show();
        $("#fdsPercentualDocumentos").hide();
        _CRUDPagamento.Finalizar.enable(true);

        SetarPercentualProcessamentoDocumentosAgregado(0);

        LimparCamposLancamento();
        BuscarPagamentoPorCodigo(codigoPagamentoAgregado);
    }
}


//*******MÉTODOS*******

function CarregarGridDescontoAcrescimo() {
    var editar = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverDescontoAcrescimoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDescontosAcrescimos = new GridView(_documento.DescontosAcrescimos.idGrid, "PagamentoAgregado/PesquisarDescontoAcrescimo", _documento, menuOpcoes, null, null, null);
    _gridDescontosAcrescimos.CarregarGrid();
}

function CarregarGridOcorrenciasDocumento() {
    _gridOcorrenciasDocumento = new GridView(_ocorrenciaDocumento.Ocorrencias.idGrid, "PagamentoAgregado/PesquisarOcorrenciasDocumento", _ocorrenciaDocumento, null, null, null, null);
    _gridOcorrenciasDocumento.CarregarGrid();
}

function RemoverDescontoAcrescimoClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir desconto / acréscimo selecionado?", function () {
        var data = {
            Codigo: e.Codigo
        };

        executarReST("PagamentoAgregado/RemoverDescontoAcrescimo", data, function (arg) {
            if (arg.Success) {

                _gridDescontosAcrescimos.CarregarGrid();
                if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado)
                    ObterResumo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function CarregarGridAdiantamentos() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverAdiantamentoClick(_documento.Adiantamentos, data)
        }, tamanho: "10", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir] };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoAdiantamento", visible: false },
    { data: "CodigoPagamento", visible: false },
    { data: "CodigoAdiantamentoPagamentoAgregado", visible: false },
    { data: "Numero", title: "Número", width: "12%", className: "text-align-center" },
    { data: "Valor", title: "Valor", width: "12%", className: "text-align-right" },
    { data: "Descricao", title: "Descrição", width: "20%", className: "text-align-left" },
    { data: "Data", title: "Data", width: "12%", className: "text-align-center" },
    { data: "Motorista", title: "Motorista", width: "20%", className: "text-align-left" }
    ];

    _gridAdiantamentos = new BasicDataTable(_documento.Adiantamentos.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    _documento.Adiantamentos.basicTable = _gridAdiantamentos;

    new BuscarAdiantamentosAgregado(_documento.Adiantamentos, RetornoInserirAdiantamento, _gridAdiantamentos, _pagamento.Cliente, _pagamento.DataInicial, _pagamento.DataFinal, _pagamento.Codigo);

    recarregarGridAdiantamentos();
}

function RetornoInserirAdiantamento(data) {
    if (data != null) {
        var dataGrid = _gridAdiantamentos.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {

            var obj = new Object();
            if (data[i].CodigoAdiantamentoPagamentoAgregado == 0)
                obj.CodigoAdiantamentoPagamentoAgregado = guid();
            else
                obj.CodigoAdiantamentoPagamentoAgregado = data[i].CodigoAdiantamentoPagamentoAgregado;
            obj.CodigoAdiantamento = data[i].CodigoAdiantamento;
            obj.CodigoPagamento = data[i].CodigoPagamento;
            obj.Codigo = data[i].Codigo;
            obj.Numero = data[i].Numero;
            obj.Valor = data[i].Valor;
            obj.Descricao = data[i].Descricao;
            obj.Data = data[i].Data;
            obj.Motorista = data[i].Motorista;

            dataGrid.push(obj);
        }
        _gridAdiantamentos.CarregarGrid(dataGrid);
    }
}

function CarregarGridAbastecimentos() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverAbastecimentoClick(_documento.Abastecimentos, data)
        }, tamanho: "10", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir] };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoAbastecimento", visible: false },
    { data: "CodigoPagamento", visible: false },
    { data: "CodigoAbastecimentoPagamentoAgregado", visible: false },
    { data: "Data", title: "Data", width: "12%", className: "text-align-center" },
    { data: "Fornecedor", title: "Fornecedor", width: "20%", className: "text-align-left" },
    { data: "Documento", title: "Documento", width: "12%", className: "text-align-center" },
    { data: "KM", title: "KM", width: "12%", className: "text-align-right" },
    { data: "Litros", title: "Litros", width: "12%", className: "text-align-right" },
    { data: "ValorUnitario", title: "Vlr. Unitário", width: "12%", className: "text-align-right" },
    { data: "ValorTotal", title: "Vlr. Total", width: "12%", className: "text-align-right" }
    ];

    _gridAbastecimentos = new BasicDataTable(_documento.Abastecimentos.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    _documento.Abastecimentos.basicTable = _gridAbastecimentos;

    new BuscarAbastecimentosAgregado(_documento.Abastecimentos, RetornoInserirAbastecimento, _gridAbastecimentos, _pagamento.Cliente, _pagamento.DataInicial, _pagamento.DataFinal, _pagamento.Codigo, _pagamento.Veiculo);

    recarregarGridAbastecimentos();
}

function RetornoInserirAbastecimento(data) {
    if (data != null) {
        var dataGrid = _gridAbastecimentos.BuscarRegistros();
        var valorAbastecimento = 0;

        for (var i = 0; i < data.length; i++) {

            var obj = new Object();
            if (data[i].CodigoAbastecimentoPagamentoAgregado == 0)
                obj.CodigoAbastecimentoPagamentoAgregado = guid();
            else
                obj.CodigoAbastecimentoPagamentoAgregado = data[i].CodigoAbastecimentoPagamentoAgregado;
            obj.CodigoAbastecimento = data[i].CodigoAbastecimento;
            obj.CodigoPagamento = data[i].CodigoPagamento;
            obj.Codigo = data[i].Codigo;

            obj.Data = data[i].Data;
            obj.Fornecedor = data[i].Fornecedor;
            obj.Documento = data[i].Documento;
            obj.KM = data[i].KM;
            obj.Litros = data[i].Litros;
            obj.ValorUnitario = data[i].ValorUnitario;
            obj.ValorTotal = data[i].ValorTotal;

            var valor = Globalize.parseFloat(obj.ValorTotal);
            if (isNaN(valor))
                valor = 0;

            valorAbastecimento += valor;

            dataGrid.push(obj);
        }
        _gridAbastecimentos.CarregarGrid(dataGrid);

        if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado) {
            var totalAbastecimento = Globalize.parseFloat(_documento.TotalAbastecimentos.val());
            if (isNaN(totalAbastecimento))
                totalAbastecimento = 0;

            totalAbastecimento += valorAbastecimento;
            _documento.TotalAbastecimentos.val(Globalize.format(totalAbastecimento, "n2"));

            var totalSaldo = Globalize.parseFloat(_documento.Saldo.val());
            if (isNaN(totalSaldo))
                totalSaldo = 0;

            totalSaldo -= valorAbastecimento;
            _documento.Saldo.val(Globalize.format(totalSaldo, "n2"));
        }
    }
}

function recarregarGridAbastecimentos() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pagamento.ListaAbastecimentos.val())) {
        $.each(_pagamento.ListaAbastecimentos.val(), function (i, abastecimento) {
            var obj = new Object();

            obj.CodigoAbastecimentoPagamentoAgregado = abastecimento.CodigoAbastecimentoPagamentoAgregado;
            obj.Codigo = abastecimento.Codigo;
            obj.CodigoAbastecimento = abastecimento.CodigoAbastecimento;
            obj.CodigoPagamento = abastecimento.CodigoPagamento;

            obj.Data = abastecimento.Data;
            obj.Fornecedor = abastecimento.Fornecedor;
            obj.Documento = abastecimento.Documento;
            obj.KM = abastecimento.KM;
            obj.Litros = abastecimento.Litros;
            obj.ValorUnitario = abastecimento.ValorUnitario;
            obj.ValorTotal = abastecimento.ValorTotal;

            data.push(obj);
        });
    }
    _gridAbastecimentos.CarregarGrid(data);
}

function RemoverAdiantamentoClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o adiantamento selecionado?", function () {
        var adiantamentoGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < adiantamentoGrid.length; i++) {
            if (sender.Codigo == adiantamentoGrid[i].Codigo) {
                adiantamentoGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(adiantamentoGrid);
    });
}

function RemoverAbastecimentoClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o abastecimento selecionado?", function () {
        var abastecimentoGrid = e.basicTable.BuscarRegistros();
        var valor = 0;
        for (var i = 0; i < abastecimentoGrid.length; i++) {
            if (sender.Codigo == abastecimentoGrid[i].Codigo) {

                valor = Globalize.parseFloat(abastecimentoGrid[i].ValorTotal);
                if (isNaN(valor))
                    valor = 0;

                abastecimentoGrid.splice(i, 1);

                break;
            }
        }
        e.basicTable.CarregarGrid(abastecimentoGrid);

        if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado) {
            var totalAbastecimento = Globalize.parseFloat(_documento.TotalAbastecimentos.val());
            if (isNaN(totalAbastecimento))
                totalAbastecimento = 0;

            totalAbastecimento -= valor;
            _documento.TotalAbastecimentos.val(Globalize.format(totalAbastecimento, "n2"));

            var totalSaldo = Globalize.parseFloat(_documento.Saldo.val());
            if (isNaN(totalSaldo))
                totalSaldo = 0;

            totalSaldo += valor;
            _documento.Saldo.val(Globalize.format(totalSaldo, "n2"));
        }
    });
}

function recarregarGridAdiantamentos() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pagamento.ListaAdiantamentos.val())) {
        $.each(_pagamento.ListaAdiantamentos.val(), function (i, adiantamento) {
            var obj = new Object();

            obj.CodigoAdiantamentoPagamentoAgregado = adiantamento.CodigoAdiantamentoPagamentoAgregado;
            obj.Codigo = adiantamento.Codigo;
            obj.CodigoAdiantamento = adiantamento.CodigoAdiantamento;
            obj.CodigoPagamento = adiantamento.CodigoPagamento;
            obj.Numero = adiantamento.Numero;
            obj.Valor = adiantamento.Valor;
            obj.Descricao = adiantamento.Descricao;
            obj.Data = adiantamento.Data;
            obj.Motorista = adiantamento.Motorista;

            data.push(obj);
        });
    }
    _gridAdiantamentos.CarregarGrid(data);
}

function CarregarGridDocumentos() {

    var baixarDACTE = {
        descricao: "Baixa DACTE", id: guid(), evento: "onclick", metodo: function (data) {
            BaixarDACTEClick(_documento.Documentos, data)
        }, tamanho: "10", icone: ""
    };
    var baixarXML = {
        descricao: "Baixa XML", id: guid(), evento: "onclick", metodo: function (data) {
            BaixarXMLClick(_documento.Documentos, data)
        }, tamanho: "10", icone: ""
    };
    var visualizarOcorrencias = {
        descricao: "Visualizar Ocorrências", id: guid(), evento: "onclick", metodo: function (data) {
            VisualizarOcorrenciasClick(_documento.Documentos, data)
        }, tamanho: "10", icone: ""
    };
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverDocumentoClick(_documento.Documentos, data)
        }, tamanho: "10", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDACTE, baixarXML, visualizarOcorrencias, excluir] };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoDocumentoPagamentoAgregado", visible: false },
    { data: "CodigoDocumento", visible: false },
    { data: "CodigoPagamento", visible: false },
    { data: "Carga", title: "Carga", width: "8%", className: "text-align-left" },
    { data: "Numero", title: "Número", width: "12%", className: "text-align-center" },
    { data: "Serie", title: "Série", width: "8%", className: "text-align-center" },
    { data: "CIOT", title: "Número CIOT", width: "8%", className: "text-align-left" },
    { data: "ValorCTe", title: "Valor CT-e", width: "10%", className: "text-align-right" },
    { data: "ValorPagamento", title: "Valor Pagamento", width: "10%", className: "text-align-right" },
    { data: "ValorAdiantamento", title: "Valor Adiantamento", width: "10%", className: "text-align-right" },
    { data: "ValorSaldo", title: "Saldo Contrato", width: "10%", className: "text-align-right" },
    { data: "Remetente", visible: false },
    { data: "Destinatario", title: "Destinatário", width: "20%", className: "text-align-left" },
    { data: "UltimaOcorrencia", visible: false },
    { data: "Status", title: "Situação CT-e", width: "5%", className: "text-align-left" },
    { data: "Motorista", title: "Motorista", width: "15%", className: "text-align-left" }
    ];

    _gridDocumentos = new BasicDataTable(_documento.Documentos.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    _documento.Documentos.basicTable = _gridDocumentos;
    //_documento.BuscarDocumentos.basicTable = _gridDocumentos;

    new BuscarDocumentosDeAgregado(_documento.Documentos, RetornoInserirDocumento, _gridDocumentos, _pagamento.Cliente, _pagamento.DataInicial, _pagamento.DataFinal, _pagamento.Codigo);
    //new BuscarDocumentosDeAgregado(_documento.BuscarDocumentos, RetornoInserirDocumento, _gridDocumentos, _pagamento.Cliente, _pagamento.DataInicial, _pagamento.DataFinal, _pagamento.Codigo, _documento.TipoOcorrencia);

    recarregarGridDocumentos();
}

function RetornoInserirDocumento(dataRetorno) {
    if (dataRetorno != null) {
        dataGridRetorno = _gridDocumentos.BuscarRegistros();
        for (var i = 0; i < dataRetorno.length; i++) {

            var data = {
                Codigo: _pagamento.Codigo.val(),
                Documento: dataRetorno[i].Codigo
            };
            posicaoRetornoDocumento = i;
            executarReST("PagamentoAgregado/AdicionarDocumentoManual", data, function (arg) {
                if (arg.Success) {

                    var obj = new Object();
                    obj.CodigoDocumentoPagamentoAgregado = arg.Data.Codigo;
                    obj.CodigoDocumento = dataRetorno[posicaoRetornoDocumento].CodigoDocumento;
                    obj.CodigoPagamento = dataRetorno[posicaoRetornoDocumento].CodigoPagamento;
                    obj.Codigo = dataRetorno[posicaoRetornoDocumento].Codigo;
                    obj.Numero = dataRetorno[posicaoRetornoDocumento].Numero;
                    obj.Serie = dataRetorno[posicaoRetornoDocumento].Serie;
                    obj.ValorCTe = dataRetorno[posicaoRetornoDocumento].ValorCTe;
                    obj.Carga = arg.Data.Carga;
                    obj.CIOT = arg.Data.CIOT;
                    obj.ValorPagamento = dataRetorno[posicaoRetornoDocumento].ValorPagamento;
                    obj.ValorAdiantamento = dataRetorno[posicaoRetornoDocumento].ValorAdiantamento;
                    obj.ValorSaldo = dataRetorno[posicaoRetornoDocumento].ValorSaldo;
                    obj.Remetente = dataRetorno[posicaoRetornoDocumento].Remetente;
                    obj.Destinatario = dataRetorno[posicaoRetornoDocumento].Destinatario;
                    obj.UltimaOcorrencia = dataRetorno[posicaoRetornoDocumento].UltimaOcorrencia;
                    obj.Status = dataRetorno[posicaoRetornoDocumento].Status;
                    obj.Motorista = dataRetorno[posicaoRetornoDocumento].Motorista;

                    dataGridRetorno.push(obj);
                    if (dataRetorno.length == posicaoRetornoDocumento + 1)
                        _gridDocumentos.CarregarGrid(dataGridRetorno);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                }
            });
        }

        var data = {
            Codigo: _pagamento.Codigo.val()
        };
        executarReST("PagamentoAgregado/AlterarStatusPagamentoAgregado", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde o início do cálculo ao agregado.");
                AtualizarProgressDocumentosAgregado(1, _pagamento.Codigo.val());

            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    }
}

function BaixarDACTEClick(e, sender) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadDACTE", data);
}

function BaixarXMLClick(e, sender) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadXML", data);
}

function VisualizarOcorrenciasClick(e, sender) {
    _ocorrenciaDocumento.Codigo.val(sender.Codigo);
    CarregarGridOcorrenciasDocumento()
    $ocorrenciasDocumento.modal("show");
    $detalhesAutorizacao.one('hidden.bs.modal', function () {
        LimparCampos(_ocorrenciaDocumento);
    });
}

function RemoverDocumentoClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o documento selecionado?", function () {
        documentoGridRemover = e.basicTable.BuscarRegistros();

        for (var i = 0; i < documentoGridRemover.length; i++) {
            if (sender.Codigo == documentoGridRemover[i].Codigo) {

                var data = {
                    Codigo: _pagamento.Codigo.val(),
                    CodigoDocumento: documentoGridRemover[i].CodigoDocumentoPagamentoAgregado
                };
                executarReST("PagamentoAgregado/RemoverDocumento", data, function (arg) {
                    if (arg.Success) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso.");
                        documentoGridRemover.splice(i, 1);
                        _gridDocumentos.CarregarGrid(documentoGridRemover);

                        if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado)
                            ObterResumo();

                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                    }
                });
                break;
            }
        }
    });
}

function recarregarGridDocumentos() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pagamento.ListaDocumentos.val())) {
        $.each(_pagamento.ListaDocumentos.val(), function (i, documento) {
            var obj = new Object();

            obj.Codigo = documento.Codigo;
            obj.CodigoDocumentoPagamentoAgregado = documento.CodigoDocumentoPagamentoAgregado;
            obj.CodigoDocumento = documento.CodigoDocumento;
            obj.CodigoPagamento = documento.CodigoPagamento;
            obj.Numero = documento.Numero;
            obj.Serie = documento.Serie;
            obj.ValorCTe = documento.ValorCTe;
            obj.Carga = documento.Carga;
            obj.CIOT = documento.CIOT;
            obj.ValorAdiantamento = documento.ValorAdiantamento;
            obj.ValorSaldo = documento.ValorSaldo;
            obj.ValorPagamento = documento.ValorPagamento;
            obj.Remetente = documento.Remetente;
            obj.Destinatario = documento.Destinatario;
            obj.UltimaOcorrencia = documento.UltimaOcorrencia;
            obj.Status = documento.Status;
            obj.Motorista = documento.Motorista;

            data.push(obj);
        });
    }
    _gridDocumentos.CarregarGrid(data);
}

function RecalcularValorClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja recalcular os valores a pagar de todos os documentos?", function () {
        var data = {
            Codigo: _pagamento.Codigo.val()
        };
        executarReST("PagamentoAgregado/RecalcularPagamentoAgregado", data, function (arg) {
            if (arg.Success) {
                $("#fdsDocumentos").hide();
                $("#fdsPercentualDocumentos").show();

                _CRUDPagamento.Finalizar.enable(false);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde o início do cálculo ao agregado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    });
}


function BuscarDocumentosClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    var tudoCerto = true;
    _documento.TipoOcorrencia.requiredClass("form-control");

    if (!_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado) {
        if (_documento.TipoOcorrencia.val() == "" || _documento.TipoOcorrencia.codEntity() == 0) {
            _documento.TipoOcorrencia.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informe o tipo da ocorrência!");
            tudoCerto = false;
            return;
        }
    }

    if (tudoCerto) {
        var data = {
            Codigo: _pagamento.Codigo.val(),
            TipoOcorrencia: _documento.TipoOcorrencia.codEntity()
        };
        executarReST("PagamentoAgregado/IniciarCalculoDocumento", data, function (arg) {
            if (arg.Success) {

                $("#fdsDocumentos").hide();
                $("#fdsPercentualDocumentos").show();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Inicio do cálculo ao pagamento do agregado.");

            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    }

}

function AdicionarDescontoAcrescimoClick(e, sender) {
    if (_pagamento.Situacao.val() != EnumSituacaoPagamentoAgregado.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Status do pagamento não permite alteração.");
        return;
    }

    var tudoCerto = true;

    _documento.Justificativa.requiredClass("form-control");
    _documento.Valor.requiredClass("form-control");

    if (_documento.Justificativa.val() == "" || _documento.Justificativa.codEntity() == 0 || _documento.Valor.val() < 0 || _documento.Valor.val() == 0) {
        _documento.Valor.requiredClass("form-control is-invalid");
        _documento.Justificativa.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informe o valor e a justificativa!");
        tudoCerto = false;
        return;
    }

    if (tudoCerto) {
        var data = {
            Codigo: _pagamento.Codigo.val(),
            Valor: _documento.Valor.val(),
            Justificativa: _documento.Justificativa.codEntity()
        };
        executarReST("PagamentoAgregado/InserirDescontoAcrescimo", data, function (arg) {
            if (arg.Success) {

                _documento.Valor.val("");
                LimparCampoEntity(_documento.Justificativa);
                _gridDescontosAcrescimos.CarregarGrid();
                if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado)
                    ObterResumo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    }
}

function EditarDadosDocumento(data) {
    _documento.Codigo.val(data.Codigo);
    if (data.DadosDocumento != null) {
        PreencherObjetoKnout(_documento, { Data: data.DadosDocumento });

        _documento.TotalPagamento.val(data.DadosPagamento.TotalPagamento);
        _documento.TotalAdiantamentos.val(data.DadosPagamento.TotalAdiantamentos);
        _documento.TotalAbastecimentos.val(data.DadosPagamento.TotalAbastecimentos);
        _documento.ValorAcrescimo.val(data.DadosPagamento.ValorAcrescimo);
        _documento.ValorDesconto.val(data.DadosPagamento.ValorDesconto);
        _documento.Saldo.val(data.DadosPagamento.Saldo);
        _documento.ValorSaldo.val(data.DadosPagamento.TotalSaldo);

        recarregarGridDocumentos();
        recarregarGridAdiantamentos();
        recarregarGridAbastecimentos();
    } else {
        _documento.Emitir.visible(false);
    }
    _gridDescontosAcrescimos.CarregarGrid();
}

function ControleCamposDocumento(status) {
    _documento.TipoOcorrencia.enable(status);
    _documento.Documentos.enable(status);
    _documento.BuscarDocumentos.enable(status);
    _documento.RecalcularValor.enable(status);
    _documento.Adiantamentos.enable(status);
    _documento.Abastecimentos.enable(status);
    _documento.DescontosAcrescimos.enable(status);
    _documento.Justificativa.enable(status);
    _documento.Valor.enable(status);
    _documento.AdicionarDescontoAcrescimo.enable(status);
}

function LimparCamposDadosDocumento() {
    LimparCampos(_documento);
    _gridDescontosAcrescimos.CarregarGrid();
    recarregarGridDocumentos();
    recarregarGridAdiantamentos();
    recarregarGridAbastecimentos();
    ControleCamposDocumento(true);
}

function ObterResumo() {
    var data = {
        Codigo: _pagamento.Codigo.val()
    };
    executarReST("PagamentoAgregado/ObterResumo", data, function (arg) {
        if (arg.Success) {

            _documento.TotalPagamento.val(arg.Data.TotalPagamento);
            _documento.TotalAdiantamentos.val(arg.Data.TotalAdiantamentos);
            _documento.TotalAbastecimentos.val(arg.Data.TotalAbastecimentos);
            _documento.ValorAcrescimo.val(arg.Data.ValorAcrescimo);
            _documento.ValorDesconto.val(arg.Data.ValorDesconto);
            _documento.Saldo.val(arg.Data.Saldo);
            _documento.ValorSaldo.val(arg.Data.ValorSaldo);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}