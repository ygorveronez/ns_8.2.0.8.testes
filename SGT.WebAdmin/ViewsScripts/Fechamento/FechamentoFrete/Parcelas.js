///// <reference path="../../../js/Global/CRUD.js" />
///// <reference path="../../../js/Global/Rest.js" />
///// <reference path="../../../js/Global/Mensagem.js" />
///// <reference path="../../../js/Global/Grid.js" />
///// <reference path="../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

////*******MAPEAMENTO KNOUCKOUT*******
//var _parcelas;
//var _gridParcelas;

//var _qtdParcelas = [
//    { text: "1", value: 1 },
//    { text: "2", value: 2 },
//    { text: "3", value: 3 },
//    { text: "4", value: 4 },
//    { text: "5", value: 5 },
//    { text: "6", value: 6 },
//    { text: "7", value: 7 },
//    { text: "8", value: 8 },
//    { text: "9", value: 9 },
//    { text: "10", value: 10 },
//    { text: "11", value: 11 },
//    { text: "12", value: 12 },
//    { text: "13", value: 13 },
//    { text: "14", value: 14 },
//    { text: "15", value: 15 },
//    { text: "16", value: 16 },
//    { text: "17", value: 17 },
//    { text: "18", value: 18 },
//    { text: "19", value: 19 },
//    { text: "20", value: 20 }
//];

//var _tipoArredondamentoFechamento = [
//    { text: "Primeira", value: 1 },
//    { text: "Última", value: 2 }
//];

//var Parcelas = function () {
//    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
//    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable("1"), options: _qtdParcelas, text: "Qtd. Parcelas: ", def: "1", enable: ko.observable(true) });
//    this.IntervaloDeDias = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Intervalo de Dias:", maxlength: 10, enable: ko.observable(true) });
//    this.DataPrimeiroVencimento = PropertyEntity({ text: "Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
//    this.TipoArredondamento = PropertyEntity({ val: ko.observable("1"), options: _tipoArredondamentoFechamento, text: "Arredondar Valor: ", def: "1", enable: ko.observable(true) });

//    this.GerarParcelas = PropertyEntity({ eventClick: GerarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });

//    this.Parcelas = PropertyEntity({ type: types.map, required: false, text: "Parcelas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
//}

////*******EVENTOS*******
//function LoadParcelas() {
//    _parcelas = new Parcelas();
//    KoBindings(_parcelas, "knockoutParcelas");

//    CarregarGridParcelas();
//}

//function GerarParcelasClick(e, sender) {
//    if (_fechamentoFrete.Codigo.val() == 0) return;

//    if (_fechamentoFrete.Situacao.val() != EnumSituacaoFechamentoFrete.Aberto) 
//        return exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível gerar parcelas de faturas em aberto.");
    
//    if (ValidarCamposObrigatorios(_parcelas)) {
//        exibirConfirmacao("Confirmação", "Realmente deseja gerar as parcelas?", function () {
//            var data = {
//                Codigo: _fechamentoFrete.Codigo.val(),
//                QuantidadeParcelas: _parcelas.QuantidadeParcelas.val(),
//                IntervaloDeDias: _parcelas.IntervaloDeDias.val(),
//                DataPrimeiroVencimento: _parcelas.DataPrimeiroVencimento.val(),
//                TipoArredondamento: _parcelas.TipoArredondamento.val()
//            };
            
//            executarReST("FechamentoFreteParcelas/GerarParcelas", data, function (arg) {
//                if (arg.Success) {
//                    if (arg.Data !== false) {
//                        _gridParcelas.CarregarGrid();
//                    }
//                    else
//                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//                } else {
//                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//                }
//            });
//        });
//    } else
//        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
//}

//function carregarParcelasFechamentoFrete() {
//    _gridParcelas.CarregarGrid();
//}

////*******METODOS*******
//function CarregarGridParcelas() {
    
//    var menuOpcoes = {
//        tipo: TypeOptionMenu.link,
//        opcoes: [
//            {
//                descricao: "Editar",
//                id: guid(),
//                evento: "onclick",
//                tamanho: "10",
//                icone: "",
//                metodo: EditarParcelas
//            }
//        ]
//    };
//    _gridParcelas = new GridView(_parcelas.Parcelas.idGrid, "FechamentoFreteParcelas/PesquisaParcelaFechamento", _fechamentoFrete, null)
//}

//function LimparCamposParcelas() {
//    //_parcelas.Atualizar.visible(false);
//    //_parcelas.Excluir.visible(false);
//    //_parcelas.Adicionar.visible(true);
//    _parcelas.GerarParcelas.visible(false);
//    LimparCampos(_parcelas);
//}

//function EditarParcelas(data) {
//    LimparCamposParcelas();
//    _parcelas.Codigo.val(data.Codigo);
//    //BuscarPorCodigo(_parcelas, "FechamentoFreteParcelas/BuscarPorCodigo", function (arg) {
//    //    _parcelas.Atualizar.visible(true);
//    //    _parcelas.Excluir.visible(true);
//    //    _parcelas.Adicionar.visible(false);
//    //}, null);
//}