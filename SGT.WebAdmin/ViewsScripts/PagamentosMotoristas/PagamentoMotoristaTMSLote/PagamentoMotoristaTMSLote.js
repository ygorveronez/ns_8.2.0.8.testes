/// <reference path="../../Consultas/Usuario.js" />
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
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoEntidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoMotoristaTMSLote;
var _pagamentoMotoristaTMSLote;
var multiplaescolhaMotorista;

var MotoristaSelecionadoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoTipoPagamentoMotorista = PropertyEntity({ val: 0, def: 0 });
    this.CodigoContaDebito = PropertyEntity({ val: 0, def: 0 });
    this.CodigoContaCredito = PropertyEntity({ val: 0, def: 0 });
    this.Motorista = PropertyEntity({ val: 0, def: 0 });
    this.CPF_Formatado = PropertyEntity({ val: "", def: "" });
    this.Nome = PropertyEntity({ val: "", def: "" });
    this.Valor = PropertyEntity({ val: 0, def: 0 });
    this.Data = PropertyEntity({ val: "", def: "" });
    this.Observacao = PropertyEntity({ val: "", def: "" });
};

var PesquisaPagamentoMotoristaTMSLote = function () {
    this.DataPagamento = PropertyEntity({ text: "*Data Pagamento: ", getType: typesKnockout.dateTime, required: true, enable: ko.observable(true), val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.TipoPagamentoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Pagamento: ", idBtnSearch: guid(), visible: true, required: true });
    this.Valor = PropertyEntity({ text: "*Valor Unitário: ", required: true, getType: typesKnockout.decimal });
    this.PlanoDeContaDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta Gerencial de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.PlanoDeContaCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta Gerencial de Saída:", idBtnSearch: guid(), required: true, val: ko.observable("") });

    this.PlanoContaDebitoCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.PlanoContaCreditoCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });

    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.SalvarMovimento2 = PropertyEntity({ eventClick: SalvarMovimentoClick, type: types.event, text: "Gerar Pagamentos", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });
    this.RatearValores = PropertyEntity({ eventClick: RatearValoresClick, type: types.event, text: "Ratear Valores", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar / Limpar", icon: ko.observable("fal fa-recycle"), idGrid: guid(), visible: ko.observable(true) });
    this.SalvarMovimento = PropertyEntity({ eventClick: SalvarMovimentoClick, type: types.event, text: "Gerar Pagamentos", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.CodigosMotoristasNaoSelecionados = PropertyEntity({ text: "", required: false, maxlength: 500000, def: ko.observable(""), val: ko.observable(0) });
    this.MotoristasNaoSelecionados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Motoristas = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.MotoristasSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.TotalSelecionado = PropertyEntity({ text: "Selecionados: ", getType: typesKnockout.int, val: ko.observable("0"), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
};

//*******EVENTOS*******


function loadPagamentoMotoristaTMSLote() {
    _pagamentoMotoristaTMSLote = new PesquisaPagamentoMotoristaTMSLote();
    KoBindings(_pagamentoMotoristaTMSLote, "knockoutPesquisaPagamentoMotoristaTMSLote", false);

    new BuscarPagamentoMotoristaTipo(_pagamentoMotoristaTMSLote.TipoPagamentoMotorista, RetornoPagamentoMotoristaTip);
    new BuscarCentroResultado(_pagamentoMotoristaTMSLote.CentroResultado);
    new BuscarPlanoConta(_pagamentoMotoristaTMSLote.PlanoDeContaDebito, "Selecione as Contas Analiticas (Débito)", "Contas Analiticas (Débito)", null, EnumAnaliticoSintetico.Analitico, null, null, _pagamentoMotoristaTMSLote.PlanoContaDebitoCodigo, null);
    new BuscarPlanoConta(_pagamentoMotoristaTMSLote.PlanoDeContaCredito, "Selecione as Contas Analiticas (Crédito)", "Contas Analiticas (Crédito)", null, EnumAnaliticoSintetico.Analitico, null, null, null, _pagamentoMotoristaTMSLote.PlanoContaCreditoCodigo);
    CriarGridPagamentoMotoristaTMSLote();
}

function RetornoPagamentoMotoristaTip(data) {
    _pagamentoMotoristaTMSLote.TipoPagamentoMotorista.codEntity(data.Codigo);
    _pagamentoMotoristaTMSLote.TipoPagamentoMotorista.val(data.Descricao);

    if (_pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity() === 0 || _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaDebito.val() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity() != data.CodigoPlanoDeContaCredito) {
        if (data.CodigoPlanoDeContaCredito > 0) {
            _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity(data.CodigoPlanoDeContaCredito);
            _pagamentoMotoristaTMSLote.PlanoDeContaDebito.val(data.DescricaoPlanoDeContaCredito);
        } else {
            LimparCampoEntity(_pagamentoMotoristaTMSLote.PlanoDeContaDebito);
        }
    }

    if (_pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity() === 0 || _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaCredito.val() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity() != data.CodigoPlanoDeContaDebito) {
        if (data.CodigoPlanoDeContaDebito > 0) {
            _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity(data.CodigoPlanoDeContaDebito);
            _pagamentoMotoristaTMSLote.PlanoDeContaCredito.val(data.DescricaoPlanoDeContaDebito);
        } else {
            LimparCampoEntity(_pagamentoMotoristaTMSLote.PlanoDeContaCredito);
        }
    }

    //_pagamentoMotoristaTMSLote.PlanoContaCreditoCodigo.val(data.PlanoContaCredito);
    //_pagamentoMotoristaTMSLote.PlanoContaDebitoCodigo.val(data.PlanoContaDebito);

    //if (_pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity() === 0 || _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaDebito.val() === "") {
    //    if (data.CodigoPlanoDeContaCredito > 0) {
    //        _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity(data.CodigoPlanoDeContaCredito);
    //        _pagamentoMotoristaTMSLote.PlanoDeContaDebito.val(data.DescricaoPlanoDeContaCredito);
    //    } else {
    //        LimparCampoEntity(_pagamentoMotoristaTMSLote.PlanoDeContaDebito);
    //    }
    //}

    //if (_pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity() === 0 || _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity() === "" || _pagamentoMotoristaTMSLote.PlanoDeContaCredito.val() === "") {
    //    if (data.CodigoPlanoDeContaDebito > 0) {
    //        _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity(data.CodigoPlanoDeContaDebito);
    //        _pagamentoMotoristaTMSLote.PlanoDeContaCredito.val(data.DescricaoPlanoDeContaDebito);
    //    } else {
    //        LimparCampoEntity(_pagamentoMotoristaTMSLote.PlanoDeContaCredito);
    //    }
    //}
}

function CriarGridPagamentoMotoristaTMSLote() {
    var somenteLeitura = false;

    _pagamentoMotoristaTMSLote.SelecionarTodos.visible(false);
    _pagamentoMotoristaTMSLote.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorMovimentos();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorMovimentos();
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pagamentoMotoristaTMSLote.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    var editarColuna = { permite: true, callback: null, atualizarRow: true };
    _gridPagamentoMotoristaTMSLote = new GridView(_pagamentoMotoristaTMSLote.Motoristas.idGrid, "PagamentoMotoristaTMSLote/PesquisaMotoristasAtivos", _pagamentoMotoristaTMSLote, null, null, 1000, null, null, null, multiplaescolha, 1000, editarColuna);
}

function AtualizarValorMovimentos() {
    _pagamentoMotoristaTMSLote.TotalSelecionado.val("0");
    _pagamentoMotoristaTMSLote.ValorTotal.val("0,00");

    var movimentosSelecionados = null;

    if (_pagamentoMotoristaTMSLote.SelecionarTodos.val()) {
        movimentosSelecionados = _gridPagamentoMotoristaTMSLote.ObterMultiplosNaoSelecionados();
    } else {
        movimentosSelecionados = _gridPagamentoMotoristaTMSLote.ObterMultiplosSelecionados();
    }
    var countSelecionados = 0;
    var valorSelecionado = 0.0;
    for (var i = 0; i < movimentosSelecionados.length; i++) {
        valorSelecionado += Globalize.parseFloat(movimentosSelecionados[i].Valor);
        countSelecionados++;
    }
    _pagamentoMotoristaTMSLote.ValorTotal.val(Globalize.format(valorSelecionado, "n2"));
    _pagamentoMotoristaTMSLote.TotalSelecionado.val(countSelecionados);
}

function buscarMotoristas() {
    _pagamentoMotoristaTMSLote.SelecionarTodos.visible(false);
    _pagamentoMotoristaTMSLote.SelecionarTodos.val(false);

    _gridPagamentoMotoristaTMSLote.CarregarGrid();
}

//*******MÉTODOS*******

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var dataEnvio = {
        Codigo: dataRow.Codigo,
        Valor: dataRow.Valor,
        CodigoTipoPagamentoMotorista: _pagamentoMotoristaTMSLote.TipoPagamentoMotorista.codEntity(),
        CodigoContaDebito: _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity(),
        CodigoContaCredito: _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity(),
        Data: _pagamentoMotoristaTMSLote.DataPagamento.val(),
        Observacao: _pagamentoMotoristaTMSLote.Observacao.val()
    };

    executarReST("PagamentoMotoristaTMSLote/AlterarValorMotorista", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data);
                _gridPagamentoMotoristaTMSLote.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridPagamentoMotoristaTMSLote.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function CarregarMotoristasNaoSelecionados() {
    _pagamentoMotoristaTMSLote.MotoristasNaoSelecionados.list = new Array();

    var motoristasNaoSelecionados;
    motoristasNaoSelecionados = _gridPagamentoMotoristaTMSLote.ObterMultiplosNaoSelecionados();

    if (motoristasNaoSelecionados.length > 0) {
        $.each(motoristasNaoSelecionados, function (i, mot) {
            var map = new MotoristaSelecionadoMap();
            map.Codigo.val = mot.Codigo;

            _pagamentoMotoristaTMSLote.MotoristasNaoSelecionados.list.push(map);
        });
    }

    _pagamentoMotoristaTMSLote.CodigosMotoristasNaoSelecionados.val(JSON.stringify(_pagamentoMotoristaTMSLote.MotoristasNaoSelecionados.list));
}

function RatearValoresClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        buscarMotoristas();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione os campos obrigatórios antes de ratear o valor.");
    }
}

function CancelarClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar / limpar todos os dados?", function () {
        LimparCamposPagamentoMotoristaTMSLote();
    });
}

var _gerandoMovimentosMotoristas = false;
function SalvarMovimentoClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja salvar os valores lançados?", function () {
        if (!_gerandoMovimentosMotoristas) {
            _gerandoMovimentosMotoristas = true;

            CarregarListaMotoristaSelecionados();

            var data = {
                MotoristasSelecionados: _pagamentoMotoristaTMSLote.MotoristasSelecionados.val(),
                DataPagamento: _pagamentoMotoristaTMSLote.DataPagamento.val()
            };

            executarReST("PagamentoMotoristaTMSLote/AdicionarMovimentacoes", data, function (arg) {
                _gerandoMovimentosMotoristas = false;
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamentos gerados com sucesso!");

                        if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 300000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }

                LimparCamposPagamentoMotoristaTMSLote();
            }, function (x) {
                _gerandoMovimentosMotoristas = false;
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
            });
        }
    });
}


function CarregarListaMotoristaSelecionados() {
    var motoristasSelecionados;
    motoristasSelecionados = _gridPagamentoMotoristaTMSLote.ObterMultiplosSelecionados();
    _pagamentoMotoristaTMSLote.MotoristasSelecionados.val("");

    if (motoristasSelecionados.length > 0) {
        var dataGrid = new Array();
        $.each(motoristasSelecionados, function (i, mot) {
            var map = new MotoristaSelecionadoMap();
            map.Codigo.val = mot.Codigo;
            map.CodigoTipoPagamentoMotorista.val = _pagamentoMotoristaTMSLote.TipoPagamentoMotorista.codEntity();
            map.CodigoContaDebito.val = _pagamentoMotoristaTMSLote.PlanoDeContaDebito.codEntity();
            map.CodigoContaCredito.val = _pagamentoMotoristaTMSLote.PlanoDeContaCredito.codEntity();
            map.CPF_Formatado.val = mot.CPF_Formatado;
            map.Nome.val = mot.Nome;
            map.Valor.val = mot.Valor;
            map.Data.val = mot.Data;
            map.Observacao.val = mot.Observacao;

            dataGrid.push(map);
        });
        _pagamentoMotoristaTMSLote.MotoristasSelecionados.val(JSON.stringify(dataGrid));
    }
}

function LimparCamposPagamentoMotoristaTMSLote() {
    LimparCampos(_pagamentoMotoristaTMSLote);
    _pagamentoMotoristaTMSLote.DataPagamento.val(Global.DataAtual());
    _pagamentoMotoristaTMSLote.CodigosMotoristasNaoSelecionados.val("");
    _pagamentoMotoristaTMSLote.MotoristasSelecionados.val("");

    _pagamentoMotoristaTMSLote.TotalSelecionado.val("0");
    _pagamentoMotoristaTMSLote.ValorTotal.val("0,00");

    _gridPagamentoMotoristaTMSLote = null;
    CriarGridPagamentoMotoristaTMSLote();
    buscarMotoristas();
}
