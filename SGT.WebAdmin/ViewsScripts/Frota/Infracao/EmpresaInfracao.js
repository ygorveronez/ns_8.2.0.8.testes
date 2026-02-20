/// <reference path="Infracao.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumResponsavelPagamentoInfracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Financeiros/RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _empresaInfracao, _gridParcelasEmpresaInfracao;

/*
 * Declaração das Classes
 */

var EmpresaInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.PessoaTituloEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.TipoMovimentoTitulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo Movimento p/ Título:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.Valor = PropertyEntity({ text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(false) });
    this.CodigoBarras = PropertyEntity({ text: "Código de Barras:", maxlength: 100, val: ko.observable(""), enable: ko.observable(false) });

    this.Parcelas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.GridParcelas = PropertyEntity({ type: types.local });

    this.QuantidadeParcela = PropertyEntity({ text: "Qtd. Parcelas: ", getType: typesKnockout.int, def: "1", val: ko.observable("1"), required: false, enable: ko.observable(false) });
    this.IntervaloDiasParcela = PropertyEntity({ text: "Intervalo (dias): ", getType: typesKnockout.int, required: false, enable: ko.observable(false) });
    this.TipoArredondamentoParcela = PropertyEntity({ text: "Arredondar:", options: EnumTipoArredondamento.ObterOpcoes(), val: ko.observable(EnumTipoArredondamento.PrimeiroItem), def: EnumTipoArredondamento.PrimeiroItem, issue: 0, required: false, enable: ko.observable(false) });
    this.DataVencimentoParcela = PropertyEntity({ text: "Primeiro Vencimento: ", getType: typesKnockout.date, required: false, enable: ko.observable(false) });

    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasEmpresaClick, type: types.event, text: "Simular Parcelas", enable: ko.observable(false) });
    this.ProcessarTituloPagar = PropertyEntity({ eventClick: ProcessarTituloPagar, type: types.event, text: "Gerar Título(s)", enable: ko.observable(false), visible: ko.observable(true) });
    this.EstornarTituloPagar = PropertyEntity({ eventClick: EstornarTituloPagarClick, type: types.event, text: "Estornar Título(s)", enable: ko.observable(false), visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadEmpresaInfracao() {
    _empresaInfracao = new EmpresaInfracao();
    KoBindings(_empresaInfracao, "knockoutEmpresaInfracao");

    new BuscarTransportadores(_empresaInfracao.Empresa);
    new BuscarTipoMovimento(_empresaInfracao.TipoMovimentoTitulo);
    new BuscarClientes(_empresaInfracao.PessoaTituloEmpresa);

    LoadGridParcelasEmpresaInfracao();
}

function LoadGridParcelasEmpresaInfracao() {

    var _editableConfigDecimal = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal()
    };

    var _editableConfigDate = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.data
    };

    var editarResposta = {
        permite: true,
        callback: SalvarRetornoGridParcelasEmpresaInfracao,
        atualizarRow: true
    };

    var header = [
        { data: "CodigoBarras", visible: false },
        { data: "Codigo", visible: false },
        { data: "DT_Enable", visible: false },
        { data: "Parcela", title: "Parcela", width: "13%" },
        { data: "DataVencimento", title: "Vencimento", width: "39%", editableCell: _editableConfigDate },
        { data: "Valor", title: "Valor", width: "39%", editableCell: _editableConfigDecimal }
    ];

    _gridParcelasEmpresaInfracao = new BasicDataTable(_empresaInfracao.GridParcelas.id, header, null, { column: 1, dir: orderDir.asc }, null, null, null, null, editarResposta);

    RecarregarGridParcelasEmpresaInfracao();
}

function SalvarRetornoGridParcelasEmpresaInfracao(dataRow) {
    var data = GetParcelasEmpresaInfracao();

    for (var i in data) {
        if (data[i].Codigo.val === dataRow.Codigo) {
            data[i].DT_Enable.val = dataRow.DT_Enable;
            data[i].Parcela.val = dataRow.Parcela;
            data[i].DataVencimento.val = dataRow.DataVencimento;
            data[i].Valor.val = dataRow.Valor;
            data[i].CodigoBarras.val = dataRow.CodigoBarras;
            break;
        }
    }

    SetParcelasEmpresaInfracao(data);
}

function GetParcelasEmpresaInfracao() {
    return _empresaInfracao.Parcelas.list.slice();
}

function SetParcelasEmpresaInfracao(data) {
    return _empresaInfracao.Parcelas.list = data.slice();
}

function RecarregarGridParcelasEmpresaInfracao() {
    _gridParcelasEmpresaInfracao.CarregarGrid(_empresaInfracao.Parcelas.val());
}

/*
 * Declaração das Funções
 */

function ProcessarTituloPagar() {
    exibirConfirmacao("Confirmação", "Realmente deseja processar o(s) título(s) a pagar?", function () {
        if (ValidarCamposObrigatorios(_empresaInfracao)) {

            if (_empresaInfracao.Parcelas.list !== null)
                _empresaInfracao.Parcelas.val(JSON.stringify(_empresaInfracao.Parcelas.list));
            else
                _empresaInfracao.Parcelas.val("[]");

            _empresaInfracao.Codigo.val(_infracao.Codigo.val());

            executarReST("Infracao/ProcessarTituloPagar", RetornarObjetoPesquisa(_empresaInfracao), function (retorno) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Títulos processados com sucesso.");
                    _gridInfracao.CarregarGrid();
                    limparCamposInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    });
}

function EstornarTituloPagarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar o(s) título(s) a pagar?", function () {
        //if (ValidarCamposObrigatorios(_empresaInfracao)) {

        _empresaInfracao.Codigo.val(_infracao.Codigo.val());

        executarReST("Infracao/EstornarTituloPagar", RetornarObjetoPesquisa(_empresaInfracao), function (retorno) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Títulos estornados com sucesso.");
                _gridInfracao.CarregarGrid();
                limparCamposInfracao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 8000);
        }, null);

        //}
        //else
        //    exibirMensagemCamposObrigatorio();
    });
}

function GerarParcelasEmpresaClick() {

    var quantidade = Globalize.parseInt(_empresaInfracao.QuantidadeParcela.val());
    var tipoArredondamento = _empresaInfracao.TipoArredondamentoParcela.val();
    var dataVencimento = moment(_empresaInfracao.DataVencimentoParcela.val(), "DD/MM/YYYY");
    var intervaloDias = Globalize.parseInt(_empresaInfracao.IntervaloDiasParcela.val());
    var valorTotal = Globalize.parseFloat(_empresaInfracao.Valor.val());
    var valorTotalAposVencimento = Globalize.parseFloat(_empresaInfracao.DataVencimentoParcela.val());

    var parcelas = new Array();

    if (quantidade <= 0)
        quantidade = 1;

    if (isNaN(valorTotal))
        valorTotal = 0;

    if (isNaN(valorTotalAposVencimento))
        valorTotalAposVencimento = 0;

    if (!dataVencimento.isValid())
        dataVencimento = moment();

    if (valorTotal > 0) {
        var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidade, "n2"));
        var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidade), "n2"));

        var valorAposVencimentoParcela = Globalize.parseFloat(Globalize.format(valorTotalAposVencimento / quantidade, "n2"));
        var valorAposVencimentoDiferenca = Globalize.parseFloat(Globalize.format(valorTotalAposVencimento - (valorAposVencimentoParcela * quantidade), "n2"));

        for (var i = 0; i < quantidade; i++) {

            var parcela = {
                Codigo: guid(),
                DT_Enable: (_infracao.FaturadoTitulosEmpresa.val() === false),
                Parcela: (i + 1)
            };

            if (i > 0)
                dataVencimento = dataVencimento.add(intervaloDias, 'd');

            parcela["CodigoBarras"] = _empresaInfracao.CodigoBarras.val();
            parcela["DataVencimento"] = dataVencimento.format("DD/MM/YYYY");

            if ((tipoArredondamento == EnumTipoArredondamento.PrimeiroItem && i == 0) || (tipoArredondamento == EnumTipoArredondamento.UltimoItem && i == (quantidade - 1))) {
                parcela["Valor"] = Globalize.format(valorParcela + valorDiferenca, "n2");
            } else {
                parcela["Valor"] = Globalize.format(valorParcela, "n2");
            }

            parcelas.push(parcela);
        }

        _empresaInfracao.Parcelas.list = parcelas;
        _gridParcelasEmpresaInfracao.CarregarGrid(parcelas);
    }
}

function controlarCamposEmpresaInfracao() {
    var habilitarCampo = (_infracao.Situacao.val() === EnumSituacaoInfracao.Finalizada && _infracao.FaturadoTitulosEmpresa.val() === false);

    _empresaInfracao.PessoaTituloEmpresa.enable(habilitarCampo);
    _empresaInfracao.TipoMovimentoTitulo.enable(habilitarCampo);
    _empresaInfracao.Empresa.enable(habilitarCampo);
    _empresaInfracao.CodigoBarras.enable(habilitarCampo);
    _empresaInfracao.Valor.enable(habilitarCampo);

    _empresaInfracao.GerarParcelas.enable(habilitarCampo);
    _empresaInfracao.DataVencimentoParcela.enable(habilitarCampo);
    _empresaInfracao.IntervaloDiasParcela.enable(habilitarCampo);
    _empresaInfracao.QuantidadeParcela.enable(habilitarCampo);
    _empresaInfracao.TipoArredondamentoParcela.enable(habilitarCampo);

    _empresaInfracao.ProcessarTituloPagar.enable(habilitarCampo);
    _empresaInfracao.EstornarTituloPagar.enable(habilitarCampo);

    if (_infracao.FaturadoTitulosEmpresa.val() === true) {
        _empresaInfracao.EstornarTituloPagar.visible(true);
        _empresaInfracao.ProcessarTituloPagar.visible(false);
    } else {
        _empresaInfracao.EstornarTituloPagar.visible(false);
        _empresaInfracao.ProcessarTituloPagar.visible(true);
    }
}

function limparCamposEmpresaInfracao() {
    LimparCampos(_empresaInfracao);
    RecarregarGridParcelasEmpresaInfracao();
}

function preencherEmpresaInfracao(empresaInfracao) {
    if (empresaInfracao) {
        PreencherObjetoKnout(_empresaInfracao, { Data: empresaInfracao });
        _empresaInfracao.Parcelas.list = _empresaInfracao.Parcelas.val();
    }

    RecarregarGridParcelasEmpresaInfracao();
    controleCampoEmpresaInfracao();
}

function controleCampoEmpresaInfracao() {
    if (_dadosInfracao.TipoMovimentoTituloEmpresa.codEntity() > 0 && _empresaInfracao.TipoMovimentoTitulo.codEntity() === 0) {
        _empresaInfracao.TipoMovimentoTitulo.codEntity(_dadosInfracao.TipoMovimentoTituloEmpresa.codEntity());
        _empresaInfracao.TipoMovimentoTitulo.val(_dadosInfracao.TipoMovimentoTituloEmpresa.val());
    }
}