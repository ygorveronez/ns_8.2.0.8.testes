/// <reference path="NFe.js" />
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

var DetalheParcela = function (nfe) {
    var instancia = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 22, enable: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor Desconto:", maxlength: 10, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: function () { instancia.SalvarParcelaClick(); }, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, "knoutDetalheParcela");
    };

    this.SalvarParcelaClick = function () {
        if (ValidarCamposObrigatorios(instancia)) {

            for (var i = 0; i < nfe.Parcelas.length; i++) {
                if (instancia.Codigo.val() == nfe.Parcelas[i].Codigo) {
                    nfe.Parcelas.splice(i, 1);
                    break;
                }
            }

            nfe.Parcelas.push({
                Codigo: instancia.Codigo.val(),
                Sequencia: instancia.Sequencia.val(),
                DataEmissao: instancia.DataEmissao.val(),
                CodigoStatus: 1,
                Acrescimo: "0,00",
                Parcela: instancia.Sequencia.val(),
                Valor: instancia.Valor.val(),
                Desconto: instancia.ValorDesconto.val(),
                DataVencimento: instancia.DataVencimento.val(),
                DescricaoSituacao: "Em Aberto",
                FormaTitulo: instancia.FormaTitulo.val()
            });

            nfe.Parcelamento.RecarregarGrid();

            LimparCampos(instancia);
            Global.fecharModal('divDetalheParcela');

        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
        }
    };
};

var Parcelamento = function (nfe, totalizador, detalheParcela) {

    var instancia = this;

    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: "*Qtd. Parcelas: ", required: true });
    this.IntervaloDeDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", required: true, maxlength: 100, getType: typesKnockout.string });
    this.DataPrimeiroVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data Primeiro Vencimento:", required: true, enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable(EnumTipoArredondamento.PrimeiroItem), options: EnumTipoArredondamento.ObterOpcoes(), def: EnumTipoArredondamento.PrimeiroItem, text: "*Tipo Arredonda.: " });

    this.GerarParcelas = PropertyEntity({ eventClick: function () { instancia.AdicionarParcelas(); }, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });

    this.ParcelasNFe = PropertyEntity({ type: types.local, id: guid() });

    this.AdicionarParcelas = function () {
        var valido = ValidarCamposObrigatorios(instancia);
        if (valido)
            valido = Globalize.parseFloat(totalizador.ValorTotalNFe.val()) > 0;
        if (valido) {
            nfe.Parcelas = new Array;
            var quantidadeParcelas = instancia.QuantidadeParcelas.val();
            var valorTotal = Globalize.parseFloat(totalizador.ValorTotalNFe.val());
            var valorParcela = parseFloat((valorTotal / quantidadeParcelas).toFixed(2));
            var valorDiferenca = parseFloat(valorTotal - (valorParcela * quantidadeParcelas).toFixed(2));
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

            //var intervaloDeDias = parseInt(instancia.IntervaloDeDias.val());
            var tipoArredondamento = instancia.TipoArredondamento.val();

            for (var i = 0; i < quantidadeParcelas; i++) {

                var valor = 0;
                if (i == 0 && tipoArredondamento === EnumTipoArredondamento.PrimeiroItem)
                    valor = (valorParcela + valorDiferenca);
                else if ((i + 1) == quantidadeParcelas && tipoArredondamento === EnumTipoArredondamento.UltimoItem)
                    valor = (valorParcela + valorDiferenca);
                else
                    valor = valorParcela;

                if (i > 0) {
                    dataUltimaParcela = dataUltimaParcela.substr(6, 4) + "/" + dataUltimaParcela.substr(3, 2) + "/" + dataUltimaParcela.substr(0, 2);
                    var dataVencimento = new Date(dataUltimaParcela);
                    //dataVencimento.setDate(dataVencimento.getDate() + intervaloDeDias);
                    if (arrayDias.length > 1)
                        dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[i]));
                    else
                        dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[0]));

                    var yyyy = dataVencimento.getFullYear().toString();
                    var mm = (dataVencimento.getMonth() + 1).toString();
                    var dd = dataVencimento.getDate().toString();

                    dataUltimaParcela = (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy;
                }

                nfe.Parcelas.push({
                    Codigo: guid(),
                    Sequencia: i + 1,
                    DataEmissao: moment().format("DD/MM/YYYY"),
                    CodigoStatus: 1,
                    Acrescimo: "0,00",
                    Parcela: i + 1,
                    Valor: Globalize.format(valor, "n2"),
                    Desconto: "0,00",
                    DataVencimento: dataUltimaParcela,
                    DescricaoSituacao: "Em Aberto",
                    FormaTitulo: EnumFormaTitulo.Outros
                });
            }
            instancia.RecarregarGrid();
            LimparCampos(instancia);
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios, e verifique o total da nota!");
        }
    };

    this.Load = function () {
        nfe.Parcelas = new Array();

        KoBindings(instancia, nfe.IdKnockoutParcelamento);

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [{ descricao: "Detalhe", id: guid(), metodo: instancia.Detalhe }, { descricao: "Remover", id: guid(), metodo: instancia.Remover }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Sequencia", visible: false },
            { data: "DataEmissao", visible: false },
            { data: "CodigoStatus", visible: false },
            { data: "Acrescimo", visible: false },
            { data: "Parcela", title: "Parcela", width: "5%" },
            { data: "Valor", title: "Valor", width: "12%" },
            { data: "Desconto", title: "Desconto", width: "12%" },
            { data: "DataVencimento", title: "Data Vencimento", width: "20%" },
            { data: "DescricaoSituacao", title: "Situação", width: "30%" },
            { data: "FormaTitulo", visible: false }];

        nfe.GridParcelas = new BasicDataTable(instancia.ParcelasNFe.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 10);

        instancia.RecarregarGrid();
    };

    this.DestivarParcelamento = function () {
        DesabilitarCamposInstanciasNFe(instancia);
        nfe.GridParcelas.CarregarGrid(nfe.Parcelas, false);
    };

    this.Detalhe = function (parcela) {
        LimparCampos(detalheParcela);

        if (parcela.Codigo != "") {
            var data =
            {
                Codigo: parcela.Codigo,
                Sequencia: parcela.Sequencia,
                Valor: parcela.Valor,
                ValorDesconto: parcela.Desconto,
                DataEmissao: parcela.DataEmissao,
                DataVencimento: parcela.DataVencimento,
                FormaTitulo: parcela.FormaTitulo
            };
            var dataParcela = { Data: data };
            PreencherObjetoKnout(detalheParcela, dataParcela);
            Global.abrirModal('divDetalheParcela');
        }
    };

    this.Remover = function (parcela) {
        exibirConfirmacao("Confirmação", "Realmente deseja remover a parcela selecionada?", function () {
            for (var i = 0; i < nfe.Parcelas.length; i++) {
                if (parcela.Codigo == nfe.Parcelas[i].Codigo) {
                    nfe.Parcelas.splice(i, 1);
                    break;
                }
            }
            nfe.Parcelamento.RecarregarGrid();
        });
    };

    this.RecarregarGrid = function () {
        nfe.GridParcelas.CarregarGrid(nfe.Parcelas);
    };
};