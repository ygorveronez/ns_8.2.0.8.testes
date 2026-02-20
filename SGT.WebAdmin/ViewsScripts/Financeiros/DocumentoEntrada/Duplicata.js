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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="DocumentoEntrada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDuplicata, _duplicata;

var Duplicata = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Sequencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 15, required: true, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "*Data de Vencimento:", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });
    this.NumeroBoleto = PropertyEntity({ text: "Boleto:", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoBarrasParaLinhaDigitavel = PropertyEntity({
        eventClick: CodigoBarrasParaLinhaDigitavelNFClick, type: types.event, text: ko.observable("Código de Barras para Linha Digitável"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-check-double"
    });
    this.LinhaDigitavelParaCodigoBarras = PropertyEntity({
        eventClick: LinhaDigitavelParaCodigoBarrasNFClick, type: types.event, text: ko.observable("Linha Digitável para Código de Barras"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-check-double"
    });
    this.ValidarValorBruto = PropertyEntity({
        eventClick: ValidarValorBrutoNFClick, type: types.event, text: ko.observable("Validar Valor Bruto"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-check-double"
    });

    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });
    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.SaldoContaAdiantamento = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00") });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarDuplicataClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarDuplicataClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirDuplicataClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarDuplicataClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadDuplicata() {

    _duplicata = new Duplicata();
    KoBindings(_duplicata, "knockoutDuplicatas");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarDuplicataClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Sequencia", title: "Sequência", width: "10%" },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "DataVencimento", title: "Data de Vencimento", width: "10%" },
        { data: "Valor", title: "Valor", width: "10%" },
        { data: "CodigoTitulo", title: "Cód. Título", width: "10%" },
        { data: "ValorTitulo", title: "Valor Título", width: "10%" },
        { data: "StatusTitulo", title: "Status Título", width: "10%" },
        { data: "DataPagamento", title: "Data Pagamento", width: "10%" },
        { data: "FormaTitulo", visible: false },
        { data: "NumeroBoleto", visible: false },
        { data: "Portador", title: "Portador", width: "20%" },
        { data: "Observacao", visible: false }
    ];

    _gridDuplicata = new BasicDataTable(_duplicata.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_duplicata.Portador, null, true);

    RecarregarGridDuplicata();
}

function RecarregarGridDuplicata() {

    var data = new Array();

    $.each(_documentoEntrada.Duplicatas.list, function (i, duplicata) {
        var duplicataGrid = new Object();

        duplicataGrid.Codigo = duplicata.Codigo.val;
        duplicataGrid.Sequencia = duplicata.Sequencia.val;
        duplicataGrid.Numero = duplicata.Numero.val;
        if (duplicata.NumeroBoleto != undefined)
            duplicataGrid.NumeroBoleto = duplicata.NumeroBoleto.val;
        else
            duplicataGrid.NumeroBoleto = "";
        duplicataGrid.DataVencimento = duplicata.DataVencimento.val;
        duplicataGrid.Valor = duplicata.Valor.val;
        if (duplicata.CodigoTitulo != undefined) {
            duplicataGrid.CodigoTitulo = duplicata.CodigoTitulo.val;
            duplicataGrid.ValorTitulo = duplicata.ValorTitulo.val;
            duplicataGrid.StatusTitulo = duplicata.StatusTitulo.val;
            duplicataGrid.DataPagamento = duplicata.DataPagamento.val;
        } else {
            duplicataGrid.CodigoTitulo = "";
            duplicataGrid.ValorTitulo = "";
            duplicataGrid.StatusTitulo = "";
            duplicataGrid.DataPagamento = "";
        }
        if (duplicata.FormaTitulo != undefined)
            duplicataGrid.FormaTitulo = duplicata.FormaTitulo.val;
        else
            duplicataGrid.FormaTitulo = EnumFormaTitulo.Outros;
        duplicataGrid.Portador = duplicata.Portador.val;
        duplicataGrid.Observacao = duplicata.Observacao.val;

        data.push(duplicataGrid);
    });

    _gridDuplicata.CarregarGrid(data);
}

function EditarDuplicataClick(data) {
    for (var i = 0; i < _documentoEntrada.Duplicatas.list.length; i++) {
        if (data.Codigo == _documentoEntrada.Duplicatas.list[i].Codigo.val) {
            var duplicata = _documentoEntrada.Duplicatas.list[i];

            _duplicata.Codigo.val(duplicata.Codigo.val);
            _duplicata.Sequencia.val(duplicata.Sequencia.val);
            _duplicata.Numero.val(duplicata.Numero.val);
            _duplicata.NumeroBoleto.val(duplicata.NumeroBoleto.val);
            _duplicata.DataVencimento.val(duplicata.DataVencimento.val);
            _duplicata.Valor.val(duplicata.Valor.val);
            _duplicata.FormaTitulo.val(duplicata.FormaTitulo.val);
            _duplicata.Portador.val(duplicata.Portador.val);
            _duplicata.Portador.codEntity(duplicata.Portador.codEntity);
            _duplicata.Observacao.val(duplicata.Observacao.val);

            _duplicata.Adicionar.visible(false);
            _duplicata.Atualizar.visible(true);
            _duplicata.Excluir.visible(true);
            _duplicata.Cancelar.visible(true);

            break;
        }
    }
}

function CodigoBarrasParaLinhaDigitavelNFClick(e, sender) {
    if (_duplicata.NumeroBoleto.val() !== "") {
        var linhaDigitavel = calcula_linha(_duplicata.NumeroBoleto.val());
        if (linhaDigitavel !== undefined && linhaDigitavel !== "")
            _duplicata.NumeroBoleto.val(linhaDigitavel);
    }
}

function LinhaDigitavelParaCodigoBarrasNFClick(e, sender) {
    if (_duplicata.NumeroBoleto.val() !== "") {
        var codigoBarras = calcula_barra(_duplicata.NumeroBoleto.val());
        if (codigoBarras !== undefined && codigoBarras !== "")
            _duplicata.NumeroBoleto.val(codigoBarras);
    }
}

function ValidarValorBrutoNFClick(e, sender) {
    if (_duplicata.NumeroBoleto.val() !== "" && _duplicata.Valor.val() !== "") {
        validar_valor(_duplicata.NumeroBoleto.val(), _duplicata.Valor.val());
    }
}

function ExcluirDuplicataClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a duplicata " + _duplicata.Numero.val() + "?", function () {
        for (var i = 0; i < _documentoEntrada.Duplicatas.list.length; i++) {
            if (_duplicata.Codigo.val() == _documentoEntrada.Duplicatas.list[i].Codigo.val) {
                _documentoEntrada.Duplicatas.list.splice(i, 1);
                break;
            }
        }

        LimparCamposDuplicata();
        RecarregarGridDuplicata();
    });
}

function AtualizarDuplicataClick(e, sender) {

    var arredondaPrimeiraParcela = true;
    var realizaRateio = false;

    var numeroParcelasComRateio = obterNumeroParcelasRateio();
    var valorDiferenca = obterValorDiferencaParcelasDuplicatas();

    if (validaValorParcelasDuplicatas()) {

        if (valorDiferenca != 0) {
           
            for (var i = 0; i < _documentoEntrada.Duplicatas.list.length; i++) {
                if (_duplicata.Codigo.val() == _documentoEntrada.Duplicatas.list[i].Codigo.val) {
                    //Altera o valor informado
                    _documentoEntrada.Duplicatas.list[i] = SalvarListEntity(_duplicata);
                    realizaRateio = true;
                } else {

                    //Realiza o Rateio nas próximas parcelas
                    if (realizaRateio) {

                        var valorFormatado;

                        if (arredondaPrimeiraParcela) {
                            valorFormatado = Globalize.format(Math.ceil((valorDiferenca / numeroParcelasComRateio) * 100) / 100, "n2");
                            arredondaPrimeiraParcela = false;
                        } else {
                            valorFormatado = formataValorComDuasCasasDecimaisSemArredondamento((valorDiferenca / numeroParcelasComRateio));
                        }

                        _documentoEntrada.Duplicatas.list[i].Valor.val = valorFormatado;
                    }
                }

            }
        } else {

            for (var i = 0; i < _documentoEntrada.Duplicatas.list.length; i++) {
                
                if (_duplicata.Codigo.val() == _documentoEntrada.Duplicatas.list[i].Codigo.val) {
                    _documentoEntrada.Duplicatas.list[i] = SalvarListEntity(_duplicata);
                }
            }
        }
        
        LimparCamposDuplicata();
        RecarregarGridDuplicata();
    }

}

function obterNumeroParcelasRateio() {

    var numeroParcelasComRateio = _documentoEntrada.Duplicatas.list.reduce((acumulador, item) => {
        return item.Sequencia.val > _duplicata.Sequencia.val() ? acumulador + 1: acumulador;
    }, 0);

    return numeroParcelasComRateio;
}

function obterValorDiferencaParcelasDuplicatas() {

    var valorTotalDocumentoEntrada = _documentoEntrada.ValorTotal.val();
    var valorDiferenca;

    var somaParcelasAnteriores = _documentoEntrada.Duplicatas.list.reduce((acumulador, item) => {
        return item.Sequencia.val < _duplicata.Sequencia.val() ? acumulador + Globalize.parseFloat(item.Valor.val) : acumulador;
    }, 0);

    if (_duplicata.Codigo.val() == _documentoEntrada.Duplicatas.list[0].Codigo.val) {
        valorDiferenca = (Globalize.parseFloat(valorTotalDocumentoEntrada) - Globalize.parseFloat(_duplicata.Valor.val()));
    } else {
        valorDiferenca = (Globalize.parseFloat(valorTotalDocumentoEntrada) - (Globalize.parseFloat(_duplicata.Valor.val()) + somaParcelasAnteriores));
    }

    return valorDiferenca;
}

function validaValorParcelasDuplicatas() {

    var valorTotalDocumentoEntrada = Globalize.parseFloat(_documentoEntrada.ValorTotal.val());
    var valorTotalParcelas = 0;
   
    var somaParcelasAnteriores = _documentoEntrada.Duplicatas.list.reduce((acumulador, item) => {
        return item.Sequencia.val < _duplicata.Sequencia.val() ? acumulador + Globalize.parseFloat(item.Valor.val) : acumulador;
    }, 0);

    valorTotalParcelas = (somaParcelasAnteriores + Globalize.parseFloat(_duplicata.Valor.val()));

    //Válida se o valor digitado é maior que o valor total do Documento de Entrada
    if (Globalize.parseFloat(_duplicata.Valor.val()) > valorTotalDocumentoEntrada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", `Valor da duplicata informada (${_duplicata.Valor.val()}) deve ser menor que o valor total do documento de entrada (${formataValorComDuasCasasDecimaisSemArredondamento(valorTotalDocumentoEntrada)}).`);
        return false;
    }

    //Válida se o total de parcelas é maior que o total do Documento de Entrada
    if (valorTotalParcelas > valorTotalDocumentoEntrada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", `Valor total de duplicadas (${formataValorComDuasCasasDecimaisSemArredondamento(valorTotalParcelas)}) deve ser menor ou igual ao valor total do documento de entrada (${formataValorComDuasCasasDecimaisSemArredondamento(valorTotalDocumentoEntrada)}).`);
        return false;
    }

    return true;
}

function formataValorComDuasCasasDecimaisSemArredondamento(valor) {

    var valorFormatado;
    const ordemGrandeza = Math.pow(10, 2);

    valorFormatado = Math.trunc(valor * ordemGrandeza) / ordemGrandeza;

    return valorFormatado.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function CancelarDuplicataClick(e, sender) {
    LimparCamposDuplicata();
}

function AdicionarDuplicataClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_duplicata);

    if (valido) {
        _duplicata.Codigo.val(guid());
        _duplicata.Sequencia.val(0);
        if (_documentoEntrada.Duplicatas.list != null && _documentoEntrada.Duplicatas.list != undefined && _documentoEntrada.Duplicatas.list.length > 0)
            _duplicata.Sequencia.val(_documentoEntrada.Duplicatas.list.length + 1);

        _documentoEntrada.Duplicatas.list.push(SalvarListEntity(_duplicata));

        RecarregarGridDuplicata();
        LimparCamposDuplicata();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposDuplicata() {
    _duplicata.Adicionar.visible(true);
    _duplicata.Atualizar.visible(false);
    _duplicata.Excluir.visible(false);
    _duplicata.Cancelar.visible(false);
    LimparCampos(_duplicata);
}