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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _blocoCarregamento;
var _gridBlocoCarregamento;
var BlocoCarregamento = function () {

    this.Carregamento = PropertyEntity({ idGrid: guid() });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.CubagemDoBloco.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PesoDoBloco.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal });
    this.GerarBlocos = PropertyEntity({ eventClick: gerarBlocosClick, type: types.event, text: Localization.Resources.Cargas.MontagemCarga.GerarBlocos.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
}

function loadBlocosCarregamento() {
    _blocoCarregamento = new BlocoCarregamento();
    KoBindings(_blocoCarregamento, "knoutBlocoCarregamento");

    var quantidadePorPagina = 300;

    var inforEditarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarBlocoGrid
    };

    _gridBlocoCarregamento = new GridView(_blocoCarregamento.Carregamento.idGrid, "MontagemCarga/PesquisaBlocoCarregamento", _blocoCarregamento, null, null, quantidadePorPagina, null, null, null, null, quantidadePorPagina, inforEditarColuna, null, null, null, callbackRowBlocosCarregamento);
}

function callbackRowBlocosCarregamento(nRow, aData) {
    var linha = $(nRow);

    if (aData.LinhasCarregamentoExpandir > 1) {
        linha.find("td:eq(0)").attr('rowspan', aData.LinhasCarregamentoExpandir).css('vertical-align', 'middle');
        linha.find("td:eq(1)").attr('rowspan', aData.LinhasCarregamentoExpandir).css('vertical-align', 'middle');
    }
    if (aData.LinhasCarregamentoExpandir == 0) {
        linha.find("td:eq(0)").css('display', 'none');
        linha.find("td:eq(1)").css('display', 'none');
    }

    if (aData.LinhasOrdemCarregamentoExpandir > 1) {
        linha.find("td:eq(2)").attr('rowspan', aData.LinhasOrdemCarregamentoExpandir).css('vertical-align', 'middle');
        linha.find("td:eq(3)").attr('rowspan', aData.LinhasOrdemCarregamentoExpandir).css('vertical-align', 'middle');
    }
    else if (aData.LinhasOrdemCarregamentoExpandir == 0) {
        linha.find("td:eq(2)").css('display', 'none');
        linha.find("td:eq(3)").css('display', 'none');
    }

    if (aData.LinhasCarregamentoSegundoTrechoExpandir > 1) {
        linha.find("td:eq(4)").attr('rowspan', aData.LinhasCarregamentoSegundoTrechoExpandir).css('vertical-align', 'middle');
        linha.find("td:eq(5)").attr('rowspan', aData.LinhasCarregamentoSegundoTrechoExpandir).css('vertical-align', 'middle');
    }
    else if (aData.LinhasCarregamentoSegundoTrechoExpandir == 0) {
        linha.find("td:eq(4)").css('display', 'none');
        linha.find("td:eq(5)").css('display', 'none');
    }
}

function AtualizarBlocoGrid(dataRow, row, head, callbackTabPress) {
    var bloco = dataRow.Bloco;
    var dados = {
        Carregamento: _blocoCarregamento.Carregamento.val(),
        Codigo: dataRow.Codigo,
        Bloco: bloco
    };

    if (bloco == "")
        return ExibirErroDataRow(_gridBlocoCarregamento, row, Localization.Resources.Cargas.MontagemCarga.BlocoNaoPodeEstarVazio, tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso);
    
    executarReST("MontagemCarga/AlterarBloco", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridBlocoCarregamento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
        }
    }, null);
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridBlocoCarregamento.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function gerarBlocosClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaGerarOsBlocos, function () {
        Salvar(_blocoCarregamento, "MontagemCarga/GerarBlocos", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _gridBlocoCarregamento.CarregarGrid(function () {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Sucesso, Localization.Resources.Cargas.MontagemCarga.BlocosGeradosComSucesso);
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
            }
        });
    });
}

function abrirGerarBlocoClick() {
    var codigo = _carregamento.Carregamento.codEntity();
    if (codigo == 0) {
        atualizarCarregamentoClick(abrirGerarBlocoClick);
    } else {

        if (!_carregamento.CarregamentoRedespacho.val()) {
            limparBlocos();

            _blocoCarregamento.Carregamento.val(_carregamento.Carregamento.codEntity());

            _gridBlocoCarregamento.CarregarGrid(function () {
                Global.abrirModal("divModalBloco");
            });
        }
    }
}

function limparBlocos() {
    LimparCampos(_blocoCarregamento);
}