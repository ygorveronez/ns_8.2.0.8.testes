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
/// <reference path="../../Enumeradores/EnumTipoArredondamento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoCobranca.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="VendaDireta.js" />

var _vendaDiretaParcela;
var _vendaDiretaParcelaDetalhe;
var _gridVendaDiretaParcela;
var _detalheVendaDiretaParcela;

var _parcelas = [
    { value: 1, text: "1X" },
    { value: 2, text: "2X" },
    { value: 3, text: "3X" },
    { value: 4, text: "4X" },
    { value: 5, text: "5X" },
    { value: 6, text: "6X" },
    { value: 7, text: "7X" },
    { value: 8, text: "8X" },
    { value: 9, text: "9X" },
    { value: 10, text: "10X" },
    { value: 11, text: "11X" },
    { value: 12, text: "12X" },
    { value: 13, text: "13X" },
    { value: 14, text: "14X" },
    { value: 15, text: "15X" },
    { value: 16, text: "16X" },
    { value: 17, text: "17X" },
    { value: 18, text: "18X" },
    { value: 19, text: "19X" },
    { value: 20, text: "20X" },
    { value: 21, text: "21X" },
    { value: 22, text: "22X" },
    { value: 23, text: "23X" },
    { value: 24, text: "24X" },
    { value: 25, text: "25X" },
    { value: 26, text: "26X" },
    { value: 27, text: "27X" },
    { value: 28, text: "28X" },
    { value: 29, text: "29X" },
    { value: 30, text: "30X" },
    { value: 31, text: "31X" },
    { value: 32, text: "32X" },
    { value: 33, text: "33X" },
    { value: 34, text: "34X" },
    { value: 35, text: "35X" },
    { value: 36, text: "36X" },
    { value: 37, text: "37X" },
    { value: 38, text: "38X" },
    { value: 39, text: "39X" },
    { value: 40, text: "40X" },
    { value: 41, text: "41X" },
    { value: 42, text: "42X" },
    { value: 43, text: "43X" },
    { value: 44, text: "44X" },
    { value: 45, text: "45X" },
    { value: 46, text: "46X" },
    { value: 47, text: "47X" },
    { value: 48, text: "48X" },
    { value: 49, text: "49X" },
    { value: 50, text: "50X" }
];

var VendaDiretaParcela = function () {
    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: "*Qtd. Parcelas: ", required: true, enable: ko.observable(true) });
    this.IntervaloDeDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", required: true, maxlength: 100, getType: typesKnockout.string, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data Primeiro Vencimento:", required: true, enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable(EnumTipoArredondamento.PrimeiroItem), options: EnumTipoArredondamento.ObterOpcoes(), def: EnumTipoArredondamento.PrimeiroItem, text: "*Tipo Arredonda.: ", enable: ko.observable(true) });

    this.GerarParcelas = PropertyEntity({ eventClick: AdicionarVendaDiretaParcelasClick, type: types.event, text: "Gerar Parcelas", enable: ko.observable(true), visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.local });

    this.TipoCobranca = PropertyEntity({ text: "Tipo Cobrança:", val: ko.observable(EnumTipoCobranca.Carteira), options: EnumTipoCobranca.obterOpcoes(), def: EnumTipoCobranca.Carteira, enable: ko.observable(true), eventChange: function () { tipoCobrancaVendaDiretaParcelaChange() } });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração Boleto:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    //Map
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Sequencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Parcela = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
    this.Desconto = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ValorTotal = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
};

var DetalheVendaDiretaParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ eventClick: SalvarDetalheParcelaVendaClick, type: types.event, text: "Salvar Parcela", enable: ko.observable(true), visible: ko.observable(true) });
};

function loadVendaDiretaParcela() {
    _vendaDiretaParcela = new VendaDiretaParcela();
    KoBindings(_vendaDiretaParcela, "knockoutVendaDiretaParcela");

    _detalheVendaDiretaParcela = new DetalheVendaDiretaParcela();
    KoBindings(_detalheVendaDiretaParcela, "knockoutDetalheParcelaVendaDireta");

    new BuscarBoletoConfiguracao(_vendaDiretaParcela.BoletoConfiguracao, RetornoConfiguracaoBanco);

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [{ descricao: "Detalhe", id: guid(), metodo: VerificarDetalheVendaDiretaParcela }, { descricao: "Remover", id: guid(), metodo: RemoverVendaDiretaParcela }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Sequencia", visible: false },
        { data: "Parcela", title: "Parcela", width: "15%" },
        { data: "Valor", title: "Valor", width: "17%" },
        { data: "Desconto", title: "Desconto", width: "17%" },
        { data: "DataVencimento", title: "Data Vencimento", width: "30%" },
        { data: "FormaTitulo", visible: false }
    ];

    _gridVendaDiretaParcela = new BasicDataTable(_vendaDiretaParcela.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    LimparCamposVendaDiretaParcela();
}

//*******MÉTODOS*******

function RetornoConfiguracaoBanco(data) {
    _vendaDiretaParcela.BoletoConfiguracao.codEntity(data.Codigo);
    _vendaDiretaParcela.BoletoConfiguracao.val(data.DescricaoBanco);
}

function RecarregarGridVendaDiretaParcela() {
    var data = new Array();
    var valorTotalParcelas = 0;

    $.each(_vendaDireta.Parcelas.list, function (i, parcela) {
        var parcelaGrid = new Object();

        parcelaGrid.Codigo = parcela.Codigo.val;
        parcelaGrid.Sequencia = parcela.Sequencia.val;
        parcelaGrid.Parcela = parcela.Parcela.val;
        parcelaGrid.Valor = parcela.Valor.val;
        parcelaGrid.Desconto = parcela.Desconto.val;
        parcelaGrid.DataVencimento = parcela.DataVencimento.val;
        parcelaGrid.FormaTitulo = parcela.FormaTitulo.val;

        valorTotalParcelas = valorTotalParcelas + (Globalize.parseFloat(parcela.Valor.val) + Globalize.parseFloat(parcela.Desconto.val));

        data.push(parcelaGrid);
    });

    _vendaDiretaParcela.ValorTotal.val(Globalize.format(valorTotalParcelas, "n2"));
    _gridVendaDiretaParcela.CarregarGrid(data);
}

function tipoCobrancaVendaDiretaParcelaChange() {
    if (_vendaDiretaParcela.TipoCobranca.val() == EnumTipoCobranca.Banco) {
        _vendaDiretaParcela.BoletoConfiguracao.visible(true);
        _vendaDiretaParcela.BoletoConfiguracao.required(true);

        if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento, _PermissoesPersonalizadas)) {
            _vendaDiretaParcela.QuantidadeParcelas.val(1);
            _vendaDiretaParcela.IntervaloDeDias.val("3");
            _vendaDiretaParcela.DataPrimeiroVencimento.val(Global.Data(EnumTipoOperacaoDate.Add, 3, 'd'));
        }
    }
    else {
        _vendaDiretaParcela.BoletoConfiguracao.visible(false);
        _vendaDiretaParcela.BoletoConfiguracao.required(false);
        LimparCampoEntity(_vendaDiretaParcela.BoletoConfiguracao);

        if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento, _PermissoesPersonalizadas)) {
            _vendaDiretaParcela.QuantidadeParcelas.val(1);
            _vendaDiretaParcela.IntervaloDeDias.val("1");
            _vendaDiretaParcela.DataPrimeiroVencimento.val(Global.DataAtual());
        }
    }
}

function AdicionarVendaDiretaParcelasClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_vendaDiretaParcela);

    if (valido) {
        valido = Globalize.parseFloat(_vendaDireta.ValorTotal.val()) > 0;
        if (valido) {
            _vendaDireta.Parcelas.list = [];
            var quantidadeParcelas = _vendaDiretaParcela.QuantidadeParcelas.val();
            var valorTotal = Globalize.parseFloat(_vendaDireta.ValorTotal.val());
            var valorParcela = parseFloat((valorTotal / quantidadeParcelas).toFixed(2));
            var valorDiferenca = parseFloat(valorTotal - (valorParcela * quantidadeParcelas).toFixed(2));
            var dataUltimaParcela = _vendaDiretaParcela.DataPrimeiroVencimento.val();

            var x = _vendaDiretaParcela.IntervaloDeDias.val();
            if (x.indexOf(".") >= 0) {
                var arrayDias = x.split(".");
                if (arrayDias.length != quantidadeParcelas) {
                    exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "As quantidades das parcelas não estão de acordo com o intervalo de dias informado!");
                    return;
                }
                for (var i = 0; i < arrayDias.length; i++) {
                    if (!parseInt(arrayDias[i]) > 0 || parseInt(arrayDias[i]) == NaN) {
                        exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                        return;
                    }
                }
            } else {
                var arrayDias = new Array;
                arrayDias[0] = x;
                if (!parseInt(arrayDias[0]) > 0 || parseInt(arrayDias[0]) == NaN) {
                    exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                    return;
                }
            }

            var tipoArredondamento = _vendaDiretaParcela.TipoArredondamento.val();
            for (var i = 0; i < quantidadeParcelas; i++) {

                var valor = 0;
                if (i == 0 && tipoArredondamento === EnumTipoArredondamento.PrimeiroItem)
                    valor = (valorParcela + valorDiferenca);
                else if ((i + 1) == quantidadeParcelas && tipoArredondamento === EnumTipoArredondamento.UltimoItem)
                    valor = (valorParcela + valorDiferenca);
                else
                    valor = valorParcela;

                if (i > 0) {
                    dataUltimaParcela = dataUltimaParcela.substr(6, 4) + "/" + dataUltimaParcela.substr(3, 2) + "/" + dataUltimaParcela.substr(0, 2)
                    var dataVencimento = new Date(dataUltimaParcela);
                    if (arrayDias.length > 1)
                        dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[i]));
                    else
                        dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[0]));

                    var yyyy = dataVencimento.getFullYear().toString();
                    var mm = (dataVencimento.getMonth() + 1).toString();
                    var dd = dataVencimento.getDate().toString();

                    dataUltimaParcela = (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy;
                }

                _vendaDiretaParcela.Codigo.val(guid());
                _vendaDiretaParcela.Sequencia.val(i + 1);
                _vendaDiretaParcela.Parcela.val(i + 1);
                _vendaDiretaParcela.Valor.val(Globalize.format(valor, "n2"));
                _vendaDiretaParcela.Desconto.val("0,00");
                _vendaDiretaParcela.DataVencimento.val(dataUltimaParcela);
                _vendaDiretaParcela.FormaTitulo.val(EnumFormaTitulo.Outros);

                _vendaDireta.Parcelas.list.push(SalvarListEntity(_vendaDiretaParcela));
            }

            LimparCamposVendaDiretaParcela();
        } else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os itens antes de gerar as parcelas!");
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function VerificarDetalheVendaDiretaParcela(parcela) {
    LimparCampos(_detalheVendaDiretaParcela);
    _detalheVendaDiretaParcela.Sequencia.enable(false);

    if (parcela.Codigo != "") {
        var data =
        {
            Codigo: parcela.Codigo,
            Sequencia: parcela.Sequencia,
            Valor: parcela.Valor,
            ValorDesconto: parcela.Desconto,
            DataVencimento: parcela.DataVencimento,
            FormaTitulo: parcela.FormaTitulo
        };
        var dataParcela = { Data: data };
        PreencherObjetoKnout(_detalheVendaDiretaParcela, dataParcela);
        Global.abrirModal('divDetalheParcelaVendaDireta');
    }
}

function RemoverVendaDiretaParcela(parcela) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a parcela selecionada?", function () {
        $.each(_vendaDireta.Parcelas.list, function (i, listaParcelas) {
            if (parcela.Codigo == listaParcelas.Codigo.val) {
                _vendaDireta.Parcelas.list.splice(i, 1);
                return false;
            }
        });

        RecarregarGridVendaDiretaParcela();
    });
}

function SalvarDetalheParcelaVendaClick() {
    if (ValidarCamposObrigatorios(_detalheVendaDiretaParcela)) {

        for (var i = 0; i < _vendaDireta.Parcelas.list.length; i++) {
            if (_detalheVendaDiretaParcela.Codigo.val() == _vendaDireta.Parcelas.list[i].Codigo.val) {
                _vendaDireta.Parcelas.list.splice(i, 1);
                break;
            }
        }

        _vendaDiretaParcela.Codigo.val(_detalheVendaDiretaParcela.Codigo.val());
        _vendaDiretaParcela.Sequencia.val(_detalheVendaDiretaParcela.Sequencia.val());
        _vendaDiretaParcela.Parcela.val(_detalheVendaDiretaParcela.Sequencia.val());
        _vendaDiretaParcela.Valor.val(_detalheVendaDiretaParcela.Valor.val());
        _vendaDiretaParcela.Desconto.val(_detalheVendaDiretaParcela.ValorDesconto.val());
        _vendaDiretaParcela.DataVencimento.val(_detalheVendaDiretaParcela.DataVencimento.val());
        _vendaDiretaParcela.FormaTitulo.val(_detalheVendaDiretaParcela.FormaTitulo.val());

        _vendaDireta.Parcelas.list.push(SalvarListEntity(_vendaDiretaParcela));

        LimparCamposVendaDiretaParcela();
        Global.fecharModal('divDetalheParcelaVendaDireta');
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function LimparCamposVendaDiretaParcela() {
    preencherCamposVendaDiretaOutrasAbas();

    LimparCampos(_vendaDiretaParcela);
    RecarregarGridVendaDiretaParcela();

    _vendaDiretaParcela.TipoCobranca.val(_vendaDireta.TipoCobranca.val());
    tipoCobrancaVendaDiretaParcelaChange();
    _vendaDiretaParcela.BoletoConfiguracao.codEntity(_vendaDireta.BoletoConfiguracao.codEntity());
    _vendaDiretaParcela.BoletoConfiguracao.val(_vendaDireta.BoletoConfiguracao.val());

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento, _PermissoesPersonalizadas)) {
        _vendaDiretaParcela.QuantidadeParcelas.enable(false);
        _vendaDiretaParcela.IntervaloDeDias.enable(false);
        _vendaDiretaParcela.DataPrimeiroVencimento.enable(false);
        _vendaDiretaParcela.TipoArredondamento.enable(false);

        _detalheVendaDiretaParcela.Valor.enable(false);
        _detalheVendaDiretaParcela.ValorDesconto.enable(false);
        _detalheVendaDiretaParcela.DataVencimento.enable(false);
        _detalheVendaDiretaParcela.FormaTitulo.enable(false);
        _detalheVendaDiretaParcela.SalvarParcela.enable(false);
    }
}