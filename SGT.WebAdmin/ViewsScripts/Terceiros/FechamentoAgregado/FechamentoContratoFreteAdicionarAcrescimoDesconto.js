/// <reference path="FechamentoAgregado.js" />
/// <reference path="EtapaFechamentoAgregado.js" />
/// <reference path="Etapa1SelecaoCIOT.js" />
/// <reference path="Etapa2Consolidacao.js" />
/// <reference path="Etapa3Integracao.js" />
/// <reference path="../../Consultas/ContratoFreteAcrescimoDesconto.js" />
/// <reference path="../../Consultas/ContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAcrescimosDescontos;
var _novoAdicionarFechamentoContratoFreteAcrescimoDesconto;

var AdicionarFechamentoContratoFreteAcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.TipoJustificativa = PropertyEntity({ text: ko.observable("Adicionar Contrato Frete Acréscimo/Desconto"), val: ko.observable(EnumTipoJustificativa.Acrescimo), options: EnumTipoJustificativa.obterOpcoesPesquisa(), def: EnumTipoJustificativa.Acrescimo });
    this.ContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Contrato de Frete:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.AcrescimosDescontos = PropertyEntity({ type: types.event, text: ko.observable("Adicionar Acréscimo/Desconto"), idBtnSearch: guid() });
    this.SalvarAcrescimosDescontos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarFechamentoContratoFreteAcrescimoDescontoClick, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadAdicionarFechamentoContratoFreteAcrescimoDesconto() {
    _novoAdicionarFechamentoContratoFreteAcrescimoDesconto = new AdicionarFechamentoContratoFreteAcrescimoDesconto();
    KoBindings(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto, "knoutAdicionarFechamentoContratoFreteAcrescimoDesconto");

    BuscarContratoFrete(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.ContratoFrete, null, null, [EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada], _novoFechamentoContratoFreteAcrescimoDesconto.NumeroCIOT);

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirAcrescimoDescontoClick(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos, data)
            }
        }]
    };

    let header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridAcrescimosDescontos = new BasicDataTable(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarContratoFreteAcrescimoDesconto(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos, null, _gridAcrescimosDescontos, _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa, _novoFechamentoContratoFreteAcrescimoDesconto.CodigoTransportadorContratoFreteOrigem);

    _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos.basicTable = _gridAcrescimosDescontos;

    recarregarGridAcrescimoDesconto();
}

function AdicionarFechamentoContratoFreteAcrescimoDescontoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Informe os campos obrigatórios!");
        return;
    }

    _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.SalvarAcrescimosDescontos.val(JSON.stringify(_gridAcrescimosDescontos.BuscarRegistros()));
    let dados = RetornarObjetoPesquisa(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto);

    executarReST("FechamentoAgregado/VincularContratoFreteAcrescimoDesconto", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/desconto vinculado com sucesso!");

                BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
                if (_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa.val() == EnumTipoJustificativa.Acrescimo) {
                    GridAcrescimo();
                }
                else {
                    GridDesconto();
                }

                Global.fecharModal('divAdicionarFechamentoContratoFreteAcrescimoDesconto');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AbrirModalAdicionarFechamentoContratoFreteAcrescimo(e, sender) {
    AbrirModalAdicionarFechamentoContratoFreteAcrescimoDescontoClick(EnumTipoJustificativa.Acrescimo);
}

function AbrirModalAdicionarFechamentoContratoFreteDesconto(e, sender) {
    AbrirModalAdicionarFechamentoContratoFreteAcrescimoDescontoClick(EnumTipoJustificativa.Desconto);
}


function AbrirModalAdicionarFechamentoContratoFreteAcrescimoDescontoClick(tipoJustificativa) {
    LimparCampos(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto);
    _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa.val(tipoJustificativa);

    if (_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa.val() == EnumTipoJustificativa.Acrescimo) {
        _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa.text("Adicionar Contrato Frete Acréscimo");
        _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos.text("Adicionar Acréscimo");
    }
    else {
        _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.TipoJustificativa.text("Adicionar Contrato Frete Desconto");
        _novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos.text("Adicionar Desconto");
    }

    Global.abrirModal('divAdicionarFechamentoContratoFreteAcrescimoDesconto');
}

//*******MÉTODOS*******

function recarregarGridAcrescimoDesconto() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos.val())) {
        $.each(_novoAdicionarFechamentoContratoFreteAcrescimoDesconto.AcrescimosDescontos.val(), function (i, operador) {
            let acrescimosDescontosGrid = new Object();
            acrescimosDescontosGrid.Codigo = operador.Tipo.Codigo;
            acrescimosDescontosGrid.Descricao = operador.Tipo.Descricao;

            data.push(acrescimosDescontosGrid);
        });
    }

    _gridAcrescimosDescontos.CarregarGrid(data);
}


function excluirAcrescimoDescontoClick(knoutAcrescimoDesconto, data) {
    let acrescimosDescontosGrid = knoutAcrescimoDesconto.basicTable.BuscarRegistros();

    for (let i = 0; i < acrescimosDescontosGrid.length; i++) {
        if (data.Codigo == acrescimosDescontosGrid[i].Codigo) {
            acrescimosDescontosGrid.splice(i, 1);
            break;
        }
    }

    knoutAcrescimoDesconto.basicTable.CarregarGrid(acrescimosDescontosGrid);
}