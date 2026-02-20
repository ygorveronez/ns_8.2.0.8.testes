/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="Localidades.js" />

var _crudSimuladorFrete;
var _simuladorFrete;
var _resultado;
var _gridResultado;

var CRUDSimuladorFrete = function () {
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: true });
    this.BuscarValores = PropertyEntity({ eventClick: buscarValoresClick, type: types.event, text: "Buscar valores da tabela de preço", visible: true });
};

var Resultado = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
};

var SimuladorFrete = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Filial:", visible: true, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Operação:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Modelo Veicular:", idBtnSearch: guid(), visible: true });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Origem:", idBtnSearch: guid() });
    this.PesoBruto = PropertyEntity({ text: "*Peso Bruto:", val: ko.observable(""), def: "0,00", visible: true, required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.Distancia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.Localidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.LocalidadeOrigem = PropertyEntity({});
    this.LocalidadesDestino = PropertyEntity({});
}

function loadSimuladorFrete() {

    _simuladorFrete = new SimuladorFrete();
    KoBindings(_simuladorFrete, "knockoutDetalhes");

    _crudSimuladorFrete = new CRUDSimuladorFrete();
    KoBindings(_crudSimuladorFrete, "knockoutCRUDSimuladorFrete");

    _resultado = new Resultado();
    KoBindings(_resultado, "knockoutResultado");

    var header = [{ data: "DescricaoRota", title: "Descrição Rota", width: "30%" },        
        { data: "Distancia", title: "Distância", width: "15%" },
        { data: "Tabela", title: "Tabela de frete", width: "35%" },
        { data: "ValorFrete", title: "Valor Frete", width: "20%" },];

    _gridResultado = new BasicDataTable(_resultado.Grid.id, header);  

    new BuscarFilial(_simuladorFrete.Filial);
    new BuscarTiposOperacao(_simuladorFrete.TipoOperacao);
    new BuscarModelosVeicularesCarga(_simuladorFrete.ModeloVeicular);
    new BuscarLocalidades(_simuladorFrete.Origem, null, null, selecionouOrigem);

    loadLocalidade();

    loadRoteirizador();
}

function selecionouOrigem(data) {
    _simuladorFrete.Origem.codEntity(data.Codigo);
    _simuladorFrete.Origem.val(data.Descricao);
    _simuladorFrete.LocalidadeOrigem.val(data);
}

function limparCamposSimulacaoFrete() {
    LimparCampos(_simuladorFrete);
    limparCamposLocalidade();
}

function limparClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja limpar as informações da simulação ?", function () {
        limparCamposSimulacaoFrete();
    });
}

function preencherListasSelecao() {
    var localidades = new Array();
    var codigosLocalidades = new Array();
    $.each(_localidade.Localidade.basicTable.BuscarRegistros(), function (i, local) {
        localidades.push({ Localidade: local });
        codigosLocalidades.push(local.Codigo);
    });
    _simuladorFrete.Localidades.val(JSON.stringify(localidades));
    return codigosLocalidades;
}

function buscarValoresClick() {
    //SimularCalculoFrete
    if (ValidarCamposObrigatorios(_simuladorFrete)) {

        var localidades = preencherListasSelecao();
        if (localidades.length == 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe pelo menos uma localidade de destino.");
            return;
        }

        var distancia = parseInt(_simuladorFrete.Distancia.val());
        if (distancia == 0 && _roteirizadorSimuladorFrete.PolilinhaRota.val() == '') {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Roteirize primeiro para obter a km correta para uma melhor simulação de frete.");
            return;
        }

        var data = {
            Filial: _simuladorFrete.Filial.codEntity(),
            TipoOperacao: _simuladorFrete.TipoOperacao.codEntity(),
            ModeloVeicular: _simuladorFrete.ModeloVeicular.codEntity(),
            Origem: _simuladorFrete.Origem.codEntity(),
            Localidades: JSON.stringify(localidades), //_simuladorFrete.Localidades.val(),
            PesoBruto: _simuladorFrete.PesoBruto.val(),
            Distancia: distancia
        };

        executarReST("SimuladorFrete/SimularCalculoFrete", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Simulação realizada com sucesso.");
                    atualizarGridResultado(arg.Data);
                } else {
                    exibirMensagem("aviso", "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe os campos obrigatórios.");
    }
}

function atualizarGridResultado(data) {
    //Atualizando o DescricaoRota de acordo com as localidades ordenadas no roteirizador...
    $.each(data, function (i, item) {
        item.DescricaoRota = "(" + item.DescricaoRota + ") - " + _roteirizadorSimuladorFrete.Descricao.val();
    });
    _gridResultado.CarregarGrid(data);
}