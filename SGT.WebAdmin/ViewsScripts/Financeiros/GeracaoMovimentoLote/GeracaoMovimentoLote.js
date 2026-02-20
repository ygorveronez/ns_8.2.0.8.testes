/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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

var _gridGeracaoMovimentoLote;
var _geracaoMovimentoLote;
var multiplaescolhaMotorista;

var _tipoMovimento = [{ text: "Saída da fixa do motorista", value: EnumTipoMovimentoEntidade.Saida }, { text: "Entrada na fixa do motorista", value: EnumTipoMovimentoEntidade.Entrada }];

var MotoristaSelecionadoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoTipoMovimento = PropertyEntity({ val: 0, def: 0 });
    this.CodigoCentroResultado = PropertyEntity({ val: 0, def: 0 });
    this.CodigoContaDebito = PropertyEntity({ val: 0, def: 0 });
    this.CodigoContaCredito = PropertyEntity({ val: 0, def: 0 });
    this.Motorista = PropertyEntity({ val: 0, def: 0 });
    this.TipoMovimentoEntidade = PropertyEntity({ val: 1, def: 1 });
    this.CPF_Formatado = PropertyEntity({ val: "", def: "" });
    this.Nome = PropertyEntity({ val: "", def: "" });
    this.Valor = PropertyEntity({ val: 0, def: 0 });
    this.Data = PropertyEntity({ val: "", def: "" });
    this.Documento = PropertyEntity({ val: "", def: "" });
    this.Observacao = PropertyEntity({ val: "", def: "" });
}

var PesquisaGeracaoMovimentoLote = function () {
    this.QuantidadeTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeNaoSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento: ", idBtnSearch: guid(), visible: true });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado: ", idBtnSearch: guid(), visible: true });
    this.ContaDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta Gerencial de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.ContaCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta Gerencial de Saída:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.DataLancamento = PropertyEntity({ text: "*Data Lançamento: ", getType: typesKnockout.date, required: true, enable: ko.observable(true), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.DataMovimento = PropertyEntity({ text: "*Data Movimentação: ", getType: typesKnockout.date, required: true, enable: ko.observable(true), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.Valor = PropertyEntity({ text: "*Valor Unitário: ", required: true, getType: typesKnockout.decimal });
    this.Documento = PropertyEntity({ text: "Documento: ", required: false, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "*Observação: ", required: true, maxlength: 500 });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filtro por Motorista:", idBtnSearch: guid(), required: false, val: ko.observable("") });
    this.TipoMovimentoEntidade = PropertyEntity({ text: "*Tipo Movimento: ", val: ko.observable(EnumTipoMovimentoEntidade.Saida), options: _tipoMovimento, def: EnumTipoMovimentoEntidade.Saida, required: true });

    this.SalvarMovimento2 = PropertyEntity({ eventClick: SalvarMovimentoClick, type: types.event, text: "Salvar Movimentos", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });
    this.RatearValores = PropertyEntity({ eventClick: RatearValoresClick, type: types.event, text: "Ratear Valores", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar / Limpar", icon: ko.observable("fal fa-recycle"), idGrid: guid(), visible: ko.observable(true) });
    this.SalvarMovimento = PropertyEntity({ eventClick: SalvarMovimentoClick, type: types.event, text: "Salvar Movimentos", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.CodigosMotoristasNaoSelecionados = PropertyEntity({ text: "", required: false, maxlength: 500000, def: ko.observable(""), val: ko.observable(0) });
    this.MotoristasNaoSelecionados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Motoristas = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    //this.MotoristasSelecionados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.MotoristasSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.CodigoAlteradoValor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorAlterado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });

    this.TotalSelecionado = PropertyEntity({ text: "Selecionados: ", getType: typesKnockout.int, val: ko.observable("0"), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
}

//*******EVENTOS*******


function loadGeracaoMovimentoLote() {
    _geracaoMovimentoLote = new PesquisaGeracaoMovimentoLote();
    KoBindings(_geracaoMovimentoLote, "knockoutPesquisaGeracaoMovimentoLote", false);

    HeaderAuditoria("MovimentoFinanceiro", _geracaoMovimentoLote);

    new BuscarPlanoConta(_geracaoMovimentoLote.ContaDebito, "Selecione as Contas Analiticas (Débito)", "Contas Analiticas (Débito)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_geracaoMovimentoLote.ContaCredito, "Selecione as Contas Analiticas (Crédito)", "Contas Analiticas (Crédito)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTipoMovimento(_geracaoMovimentoLote.TipoMovimento, null, null, RetornoTipoMovimento);
    new BuscarCentroResultado(_geracaoMovimentoLote.CentroResultado, "Selecione as Contas Analiticas", "Contas Analiticas", null, EnumAnaliticoSintetico.Analitico, _geracaoMovimentoLote.TipoMovimento);
    new BuscarMotorista(_geracaoMovimentoLote.Motorista, RetornoBuscarMotorista);

    CriarGridGeracaoMovimentoLote();
}

function RetornoBuscarMotorista(data) {
    _geracaoMovimentoLote.Motorista.codEntity(data.Codigo);
    _geracaoMovimentoLote.Motorista.val(data.Nome);
}

function CriarGridGeracaoMovimentoLote() {
    var somenteLeitura = false;

    _geracaoMovimentoLote.SelecionarTodos.visible(false);
    _geracaoMovimentoLote.SelecionarTodos.val(false);

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
        SelecionarTodosKnout: _geracaoMovimentoLote.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    var editarColuna = { permite: true, callback: null, atualizarRow: true };
    _gridGeracaoMovimentoLote = new GridView(_geracaoMovimentoLote.Motoristas.idGrid, "GeracaoMovimentoLote/PesquisaMotoristasAtivos", _geracaoMovimentoLote, null, null, 1000, null, null, null, multiplaescolha, 1000, editarColuna);
}

function AtualizarValorMovimentos() {
    _geracaoMovimentoLote.TotalSelecionado.val("0");
    _geracaoMovimentoLote.ValorTotal.val("0,00");

    var movimentosSelecionados = null;

    if (_geracaoMovimentoLote.SelecionarTodos.val()) {
        movimentosSelecionados = _gridGeracaoMovimentoLote.ObterMultiplosNaoSelecionados();
    } else {
        movimentosSelecionados = _gridGeracaoMovimentoLote.ObterMultiplosSelecionados();
    }
    var countSelecionados = 0;
    var valorSelecionado = 0.0;
    for (var i = 0; i < movimentosSelecionados.length; i++) {
        valorSelecionado += Globalize.parseFloat(movimentosSelecionados[i].Valor);
        countSelecionados++;
    }
    _geracaoMovimentoLote.ValorTotal.val(Globalize.format(valorSelecionado, "n2"));
    _geracaoMovimentoLote.TotalSelecionado.val(countSelecionados);
}

function buscarMotoristas() {
    _geracaoMovimentoLote.SelecionarTodos.visible(false);
    _geracaoMovimentoLote.SelecionarTodos.val(false);

    _gridGeracaoMovimentoLote.CarregarGrid();
}

//*******MÉTODOS*******

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var dataEnvio = {
        Codigo: dataRow.Codigo,
        Valor: dataRow.Valor,
        CodigoTipoMovimento: _geracaoMovimentoLote.TipoMovimento.codEntity(),
        CodigoCentroResultado: _geracaoMovimentoLote.CentroResultado.codEntity(),
        CodigoContaDebito: _geracaoMovimentoLote.ContaDebito.codEntity(),
        CodigoContaCredito: _geracaoMovimentoLote.ContaCredito.codEntity(),
        Motorista: _geracaoMovimentoLote.Motorista.codEntity(),
        Data: _geracaoMovimentoLote.DataMovimento.val(),
        Documento: _geracaoMovimentoLote.Documento.val(),
        Observacao: _geracaoMovimentoLote.Observacao.val(),
        DataLancamento: _geracaoMovimentoLote.DataLancamento.val()
    };

    executarReST("GeracaoMovimentoLote/AlterarValorMotorista", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data);
                _gridGeracaoMovimentoLote.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridGeracaoMovimentoLote.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function CarregarMotoristasNaoSelecionados() {
    _geracaoMovimentoLote.MotoristasNaoSelecionados.list = new Array();

    var motoristasNaoSelecionados;
    motoristasNaoSelecionados = _gridGeracaoMovimentoLote.ObterMultiplosNaoSelecionados();

    if (motoristasNaoSelecionados.length > 0) {
        $.each(motoristasNaoSelecionados, function (i, mot) {
            var map = new MotoristaSelecionadoMap();
            map.Codigo.val = mot.Codigo;

            _geracaoMovimentoLote.MotoristasNaoSelecionados.list.push(map);
        });
    }

    _geracaoMovimentoLote.CodigosMotoristasNaoSelecionados.val(JSON.stringify(_geracaoMovimentoLote.MotoristasNaoSelecionados.list));
}

function RetornoTipoMovimento(data) {
    if (data != null) {
        _geracaoMovimentoLote.TipoMovimento.codEntity(data.Codigo);
        _geracaoMovimentoLote.TipoMovimento.val(data.Descricao);

        if (data.CodigoDebito > 0) {
            _geracaoMovimentoLote.ContaDebito.codEntity(data.CodigoDebito);
            _geracaoMovimentoLote.ContaDebito.val(data.PlanoDebito);
        }
        if (data.CodigoCredito > 0) {
            _geracaoMovimentoLote.ContaCredito.codEntity(data.CodigoCredito);
            _geracaoMovimentoLote.ContaCredito.val(data.PlanoCredito);
        }
        if (data.CodigoResultado > 0) {
            _geracaoMovimentoLote.CentroResultado.codEntity(data.CodigoResultado);
            _geracaoMovimentoLote.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_geracaoMovimentoLote.CentroResultado);
        }
    }
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
        
        LimparCamposGeracaoMovimento();
    });
}

var _gerandoMovimentosMotoristas = false;
function SalvarMovimentoClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja salvar os valores lançados?", function () {
        if (!_gerandoMovimentosMotoristas) {
            _gerandoMovimentosMotoristas = true;

            CarregarListaMotoristaSelecionados();

            var comRelatorio = true;

            var data = {
                MotoristasSelecionados: _geracaoMovimentoLote.MotoristasSelecionados.val(),
                ComRelatorio: comRelatorio,
                DataLancamento: _geracaoMovimentoLote.DataLancamento.val()
            };

            executarReST("GeracaoMovimentoLote/AdicionarMovimentacoes", data, function (arg) {
                _gerandoMovimentosMotoristas = false;
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Movimentos gerados com sucesso!");
                    LimparCamposGeracaoMovimento();
                } else {
                    LimparCamposGeracaoMovimento();
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, function (x) {
                _gerandoMovimentosMotoristas = false;
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
            });
        }
    });
}


function CarregarListaMotoristaSelecionados() {
    var motoristasSelecionados;
    motoristasSelecionados = _gridGeracaoMovimentoLote.ObterMultiplosSelecionados();
    _geracaoMovimentoLote.MotoristasSelecionados.val("");

    if (motoristasSelecionados.length > 0) {
        var dataGrid = new Array();
        $.each(motoristasSelecionados, function (i, mot) {
            var map = new MotoristaSelecionadoMap();
            map.Codigo.val = mot.Codigo;
            map.CodigoTipoMovimento.val = _geracaoMovimentoLote.TipoMovimento.codEntity(),
                map.CodigoCentroResultado.val = _geracaoMovimentoLote.CentroResultado.codEntity(),
                map.CodigoContaDebito.val = _geracaoMovimentoLote.ContaDebito.codEntity(),
                map.CodigoContaCredito.val = _geracaoMovimentoLote.ContaCredito.codEntity(),
                map.CPF_Formatado.val = mot.CPF_Formatado;
            map.Nome.val = mot.Nome;
            map.Valor.val = mot.Valor;
            map.Data.val = mot.Data;
            map.Observacao.val = mot.Observacao;
            map.Documento.val = mot.Documento;
            map.TipoMovimentoEntidade.val = _geracaoMovimentoLote.TipoMovimentoEntidade.val();

            dataGrid.push(map);
        });
        _geracaoMovimentoLote.MotoristasSelecionados.val(JSON.stringify(dataGrid));
    }
}

function LimparCamposGeracaoMovimento() {
    LimparCampos(_geracaoMovimentoLote);
    _geracaoMovimentoLote.DataMovimento.val(Global.DataAtual());
    _geracaoMovimentoLote.DataLancamento.val(Global.DataAtual());
    _geracaoMovimentoLote.CodigosMotoristasNaoSelecionados.val("");
    _geracaoMovimentoLote.MotoristasSelecionados.val("");

    _geracaoMovimentoLote.TotalSelecionado.val("0");
    _geracaoMovimentoLote.ValorTotal.val("0,00");

    _gridGeracaoMovimentoLote = null;
    CriarGridGeracaoMovimentoLote();
    buscarMotoristas();
}
