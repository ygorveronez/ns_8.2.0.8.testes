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
/// <reference path="PedidoVendaEtapa.js" />
/// <reference path="PedidoVenda.js" />
/// <reference path="PedidoVendaItens.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

var _modalDetalheParcelaPedido;
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

var ParcelaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Sequencia = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Parcela = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Valor = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Desconto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataVencimento = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.FormaTitulo = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var DetalheParcelaPedido = function (pedidoVenda) {
    var instancia = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: function () { instancia.SalvarParcelaClick(); }, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, "knoutDetalheParcelaPedido");
        _modalDetalheParcelaPedido = new bootstrap.Modal(document.getElementById("divDetalheParcelaPedido"), { backdrop: true, keyboard: true });
    };

    this.SalvarParcelaClick = function () {
        if (ValidarCamposObrigatorios(instancia)) {

            for (var i = 0; i < pedidoVenda.ListaParcelas.list.length; i++) {
                if (instancia.Codigo.val() == pedidoVenda.ListaParcelas.list[i].Codigo.val) {
                    pedidoVenda.ListaParcelas.list.splice(i, 1);
                    break;
                }
            }

            var listaParcelasGrid = new ParcelaMap();

            listaParcelasGrid.Codigo.val = instancia.Codigo.val();
            listaParcelasGrid.Sequencia.val = instancia.Sequencia.val();
            listaParcelasGrid.Parcela.val = instancia.Sequencia.val();
            listaParcelasGrid.Valor.val = instancia.Valor.val();
            listaParcelasGrid.Desconto.val = instancia.ValorDesconto.val();
            listaParcelasGrid.DataVencimento.val = instancia.DataVencimento.val();
            listaParcelasGrid.FormaTitulo.val = instancia.FormaTitulo.val();

            pedidoVenda.ListaParcelas.list.push(listaParcelasGrid);

            pedidoVenda.ParcelamentoPedido.RecarregarGrid();
            LimparCampos(instancia);
            divDetalheParcelaPedido.hide();

        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
        }
    };
};

var ParcelamentoPedido = function (pedidoVenda, detalheParcela) {

    var instancia = this;

    this.CondicaoPagamentoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Condição de pagamento", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FormaPagamento = PropertyEntity({ text: "Forma de Pagamento utilizada: ", required: false, maxlength: 500, getType: typesKnockout.string, enable: ko.observable(true), visible: ko.observable(false) });
    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: "*Qtd. Parcelas: ", required: true, enable: ko.observable(true) });
    this.IntervaloDeDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", required: true, maxlength: 100, getType: typesKnockout.string, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data Primeiro Vencimento:", required: true, enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable(EnumTipoArredondamento.PrimeiroItem), options: EnumTipoArredondamento.ObterOpcoes(), def: EnumTipoArredondamento.PrimeiroItem, text: "*Tipo Arredonda.: ", enable: ko.observable(true) });

    this.GerarParcelas = PropertyEntity({ eventClick: function () { instancia.AdicionarParcelas(); }, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });

    this.ParcelasPedido = PropertyEntity({ type: types.local, id: guid() });

    this.Gravar = PropertyEntity({ eventClick: GravarPedidoVendaItensClick, type: types.event, text: "Gravar Pedido", visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarParcelas = function () {
        var valido = ValidarCamposObrigatorios(instancia);
        if (valido)
            valido = parseFloat(pedidoVenda.ValorTotal.val().toString().replace(".", "").replace(",", ".")) > 0;
        if (valido) {
            pedidoVenda.ListaParcelas.list = [];
            var quantidadeParcelas = instancia.QuantidadeParcelas.val();
            var valorTotal = parseFloat(pedidoVenda.ValorTotal.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
            valorTotal = parseFloat(valorTotal);
            var valorParcela = (valorTotal / quantidadeParcelas).toFixed(2);
            valorParcela = parseFloat(valorParcela);
            var valorDiferenca = valorTotal - (valorParcela * quantidadeParcelas).toFixed(2);
            valorDiferenca = parseFloat(valorDiferenca);
            var dataUltimaParcela = instancia.DataPrimeiroVencimento.val();

            var x = instancia.IntervaloDeDias.val();
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

            var tipoArredondamento = instancia.TipoArredondamento.val();

            for (var i = 0; i < quantidadeParcelas; i++) {

                var valor = 0;
                if (i == 0 && tipoArredondamento === EnumTipoArredondamento.PrimeiroItem)
                    valor = (valorParcela + valorDiferenca).toFixed(2);
                else if ((i + 1) == quantidadeParcelas && tipoArredondamento === EnumTipoArredondamento.UltimoItem)
                    valor = (valorParcela + valorDiferenca).toFixed(2);
                else
                    valor = valorParcela.toFixed(2);

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

                var listaParcelasGrid = new ParcelaMap();

                listaParcelasGrid.Codigo.val = guid();
                listaParcelasGrid.Sequencia.val = i + 1;
                listaParcelasGrid.Parcela.val = i + 1;
                listaParcelasGrid.Valor.val = instancia.mvalor(valor.replace(".", ","));
                listaParcelasGrid.Desconto.val = "0,00";
                listaParcelasGrid.DataVencimento.val = dataUltimaParcela;
                listaParcelasGrid.FormaTitulo.val = EnumFormaTitulo.Outros;

                pedidoVenda.ListaParcelas.list.push(listaParcelasGrid);
            }
            instancia.RecarregarGrid();
            var formaPagamento = instancia.FormaPagamento.val();
            var condicaoPagamentoCodigo = instancia.CondicaoPagamentoPadrao.codEntity();
            var condicaoPagamentoDescricao = instancia.CondicaoPagamentoPadrao.val();
            LimparCampos(instancia);
            instancia.FormaPagamento.val(formaPagamento);
            instancia.CondicaoPagamentoPadrao.codEntity(condicaoPagamentoCodigo);
            instancia.CondicaoPagamentoPadrao.val(condicaoPagamentoDescricao);
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios, e verifique o total da nota!");
        }
    };

    this.LimparCamposParcelamentoPedido = function (v) {
        LimparCampos(instancia);
        pedidoVenda.ListaParcelas.list = new Array();
        instancia.RecarregarGrid();
    };

    this.mvalor = function (v) {
        v = v.replace(/\D/g, "");
        v = v.replace(/(\d)(\d{8})$/, "$1.$2");
        v = v.replace(/(\d)(\d{5})$/, "$1.$2");

        v = v.replace(/(\d)(\d{2})$/, "$1,$2");
        return v;
    };

    this.Load = function () {
        KoBindings(instancia, "knockoutParcelasPedido");

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [{ descricao: "Detalhe", id: guid(), metodo: instancia.Detalhe }, { descricao: "Remover", id: guid(), metodo: instancia.Remover }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Sequencia", visible: false },
            { data: "Parcela", title: "Parcela", width: "15%" },
            { data: "Valor", title: "Valor", width: "17%" },
            { data: "Desconto", title: "Desconto", width: "17%" },
            { data: "DataVencimento", title: "Data Vencimento", width: "30%" },
            { data: "FormaTitulo", visible: false }
        ];

        pedidoVenda.GridParcelas = new BasicDataTable(instancia.ParcelasPedido.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 10);

        instancia.RecarregarGrid();
    };

    this.Detalhe = function (parcela) {
        LimparCampos(detalheParcela);
        detalheParcela.Sequencia.enable(false);

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
            PreencherObjetoKnout(detalheParcela, dataParcela);
            _modalDetalheParcelaPedido.show();
        }
    };

    this.Remover = function (parcela) {
        exibirConfirmacao("Confirmação", "Realmente deseja remover a parcela selecionada?", function () {
            for (var i = 0; i < pedidoVenda.ListaParcelas.list.length; i++) {
                if (parcela.Codigo == pedidoVenda.ListaParcelas.list[i].Codigo.val) {
                    pedidoVenda.ListaParcelas.list.splice(i, 1);
                    break;
                }
            }
            pedidoVenda.ParcelamentoPedido.RecarregarGrid();
        });
    };

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each(pedidoVenda.ListaParcelas.list, function (i, listaParcelas) {
            var listaParcelasGrid = new Object();

            listaParcelasGrid.Codigo = listaParcelas.Codigo.val;
            listaParcelasGrid.Sequencia = listaParcelas.Sequencia.val;
            listaParcelasGrid.Parcela = listaParcelas.Sequencia.val;
            listaParcelasGrid.Valor = listaParcelas.Valor.val;
            listaParcelasGrid.Desconto = listaParcelas.Desconto.val;
            listaParcelasGrid.DataVencimento = listaParcelas.DataVencimento.val;
            listaParcelasGrid.FormaTitulo = listaParcelas.FormaTitulo.val;

            data.push(listaParcelasGrid);
        });

        pedidoVenda.GridParcelas.CarregarGrid(data);
    };

    this.CalcularDataPrimeiroVencimento = function () {
        var x = instancia.IntervaloDeDias.val().trim();

        if (x != "") {
            var arrayDias = "";
            if (x.indexOf(".") >= 0) {
                arrayDias = x.split(".");
            } else {
                arrayDias = new Array;
                arrayDias[0] = x;
            }

            if (arrayDias[0].trim() != "")
                instancia.DataPrimeiroVencimento.val(Global.Data(EnumTipoOperacaoDate.Add, arrayDias[0], 'd'));
        }
    };
};