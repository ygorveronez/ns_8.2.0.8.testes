/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Abastecimento.js" />
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Consultas/BombaAbastecimento.js" />
/// <reference path="../../Enumeradores/EnumModoAbastecimento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

let _movimentacaoAbastecimento;
let _pesquisaMovimentacaoAbastecimento;
let _gridMovimentacaoAbastecimento;
let _abastecimento;

let PesquisaMovimentacaoAbastecimento = function () {
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.LocalArmazenamento, idBtnSearch: guid()});
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Veiculo, idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.DataInicial, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.DataFinal, getType: typesKnockout.date });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Empresa, idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentacaoAbastecimento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
   
};

let MovimentacaoAbastecimento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Empresa, idBtnSearch: guid(), enable: false, visible: true });
    this.Data = PropertyEntity({ getType: typesKnockout.dateTime, required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.DataAbastecimento, enable: true, visible: true, val: ko.observable("") });
    this.TipoAbastecimento = PropertyEntity({ text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.TipoAbastecimento, val: ko.observable(EnumModoAbastecimento.Interno), options: EnumModoAbastecimento.obterOpcoes(), def: EnumModoAbastecimento.Interno, enable: false, visible: true });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.LocalArmazenamento, idBtnSearch: guid(), enable: true, visible: true });
    this.BombaAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.BombaAbastecimento, idBtnSearch: guid(), enable: true, visible: true });
    this.TipoDeOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.TipoDeOleo, idBtnSearch: guid(), enable: false, visible: true });
    this.Saldo = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,0000"), required: false, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Saldo, configDecimal: { precision: 4, allowZero: false }, maxlength: 10, enable: ko.observable(false), visible: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Veiculo, idBtnSearch: guid(), enable: ko.observable(true), visible: undefined });
    this.Prefixo = PropertyEntity({ getType: typesKnockout.text, val: ko.observable(""), required: false, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Prefixo, enable: ko.observable(false), visible: true });
    this.QuantidadeLitros = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,0000"), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Litros, configDecimal: { precision: 4, allowZero: false }, maxlength: 10, enable: ko.observable(true), visible: true });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), required: true, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Valor, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: ko.observable(false), visible: ko.observable(true) });
    this.Hodometro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("0"), required: false, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.Hodometro, configInt: { precision: 0, allowZero: true }, maxlength: 10, def: "0", enable: ko.observable(true), visible: ko.observable(true) });
    this.QuantidadeArla32 = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,0000"), required: false, text: Localization.Resources.AbastecimentoInterno.MovimentacaoAbastecimento.QuantidadeArla32, configDecimal: { precision: 4, allowZero: false }, maxlength: 10, enable: ko.observable(true), visible: ko.observable(false) });

    //this.ValorTotal.val.subscribe(function (novoValor) {
    //    ConverterValorOriginal();
    //});

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar/Cancelar", visible: ko.observable(true) });
   
};

//*******EVENTOS*******

function loadMovimentacaoAbastecimento() {

    _pesquisaMovimentacaoAbastecimento = new PesquisaMovimentacaoAbastecimento();
    KoBindings(_pesquisaMovimentacaoAbastecimento, "knockoutPesquisaAbastecimento", false, _pesquisaMovimentacaoAbastecimento.Pesquisar.id);

    new BuscarLocalArmazenamentoProduto(_pesquisaMovimentacaoAbastecimento.LocalArmazenamento);
    new BuscarVeiculos(_pesquisaMovimentacaoAbastecimento.Veiculo);
    new BuscarEmpresa(_pesquisaMovimentacaoAbastecimento.Empresa);


    _movimentacaoAbastecimento = new MovimentacaoAbastecimento();
    KoBindings(_movimentacaoAbastecimento, "knockoutCadastroMovimentacaoAbastecimento");

    new BuscarEmpresa(_movimentacaoAbastecimento.Empresa);
    new BuscarLocalArmazenamentoProduto(_movimentacaoAbastecimento.LocalArmazenamento, RetornoLocalArmazenamento);
    new BuscarBombaAbastecimento(_movimentacaoAbastecimento.BombaAbastecimento, RetornoBombaAbastecimento);
    new BuscarVeiculos(_movimentacaoAbastecimento.Veiculo, retornoVeiculo, _movimentacaoAbastecimento.Empresa);

    BuscarMovimentacaoAbastecimento();

    HeaderAuditoria("MovimentacaoAbastecimento", _movimentacaoAbastecimento);

}

function RetornoBombaAbastecimento(bombaAbastecimento) {
    _movimentacaoAbastecimento.BombaAbastecimento.val(bombaAbastecimento.Descricao);
    _movimentacaoAbastecimento.BombaAbastecimento.codEntity(bombaAbastecimento.Codigo);

    _movimentacaoAbastecimento.TipoDeOleo.val(bombaAbastecimento.TipoOleo);
    _movimentacaoAbastecimento.TipoDeOleo.codEntity(bombaAbastecimento.CodigoTipoOleo);

    executarReST("LocalArmazenamentoHistorico/BuscarSaldoTanqueHistorico", { Codigo: _movimentacaoAbastecimento.LocalArmazenamento.codEntity() }, function (arg) {
        if (!arg.Success) {
            _movimentacaoAbastecimento.Saldo.val(0);
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg, 10);
        } else {
            _movimentacaoAbastecimento.Saldo.val(arg.Data.SaldoAtual);
        }
    });

}

function RetornoLocalArmazenamento(localArmazenamentoProduto) {
    _movimentacaoAbastecimento.LocalArmazenamento.val(localArmazenamentoProduto.Descricao);
    _movimentacaoAbastecimento.LocalArmazenamento.codEntity(localArmazenamentoProduto.Codigo);

    _movimentacaoAbastecimento.Empresa.val(localArmazenamentoProduto.Empresa);
    _movimentacaoAbastecimento.Empresa.codEntity(localArmazenamentoProduto.CodigoEmpresa);
}

function retornoVeiculo(data) {
    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        if (data.TipoPropriedade == "T")
            _movimentacaoAbastecimento.Veiculo.val(data.Placa + " (TERCEIRO) " + data.ModeloVeicularCarga);
        else
            _movimentacaoAbastecimento.Veiculo.val(data.Placa + " (PRÓPRIO) " + data.ModeloVeicularCarga);
        _movimentacaoAbastecimento.Veiculo.codEntity(data.Codigo);

        if (data.NumeroFrota && data.NumeroFrota !== "") {
            _movimentacaoAbastecimento.Prefixo.val(data.NumeroFrota);
        } else {
            _movimentacaoAbastecimento.Prefixo.val(data.Placa);
        }

        if (data.Arla32) {
            _movimentacaoAbastecimento.QuantidadeArla32.visible(true);
        }
        _movimentacaoAbastecimento.QuantidadeArla32.val(0.000);

    }
    
}

function adicionarClick(e, sender) {
    Salvar(_movimentacaoAbastecimento, "MovimentacaoAbastecimentoSaida/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMovimentacaoAbastecimento.CarregarGrid();
                limparCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(_movimentacaoAbastecimento, "MovimentacaoAbastecimentoSaida/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMovimentacaoAbastecimento.CarregarGrid();
                limparCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a movimentação do veículo " + _movimentacaoAbastecimento.Veiculo.val() + "?", function () {
        ExcluirPorCodigo(_movimentacaoAbastecimento, "MovimentacaoAbastecimentoSaida/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMovimentacaoAbastecimento.CarregarGrid();
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCampos();
}

//*******MÉTODOS*******


async function calculaLitrosAbascimento(e) {
    _movimentacaoAbastecimento.Valor.val(0.00);
    let litros = 0;

    if (_movimentacaoAbastecimento.QuantidadeLitros.val() != null) {
        litros = parseFloat(_movimentacaoAbastecimento.QuantidadeLitros.val());
    }

    if (litros > 0) {
        if (_movimentacaoAbastecimento.TipoDeOleo.codEntity() == null || _movimentacaoAbastecimento.Data.val() == null)
            exibirMensagem(tipoMensagem.aviso, "Atenção", "É necessário informar o tipo de óleo e data para buscar a tabela de preços");

        try {
            executarReST("TabelaPrecoCombustivel/BuscarTabelaPrecoPorVigencia", {
                CodigoTipoOleo: _movimentacaoAbastecimento.TipoDeOleo.codEntity(),
                Data: _movimentacaoAbastecimento.Data.val()
            }, function (arg) {
                if (!arg.Success) {
                    _movimentacaoAbastecimento.Valor.val(0.00);
                    exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg, 10);
                } else {
                    let valorUnitario = parseFloat(arg.Data.ValorInterno.replace(',', '.'));

                    if (valorUnitario > 0) {
                        _movimentacaoAbastecimento.Valor.val(formatarStrFloat(parseFloat(litros * valorUnitario).toFixed(2)));
                    }
                }
            });

        } catch (error) {
            _movimentacaoAbastecimento.Valor.val(0.00);
            exibirMensagem(tipoMensagem.aviso, "Erro", "Não foi possível obter o valor unitário.", 10);
        }
    }
}


function BuscarMovimentacaoAbastecimento() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMovimentacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMovimentacaoAbastecimento = new GridView(_pesquisaMovimentacaoAbastecimento.Pesquisar.idGrid, "MovimentacaoAbastecimentoSaida/Pesquisa", _pesquisaMovimentacaoAbastecimento, menuOpcoes, null, 10, null, null, null, null, null, null, null);
    _gridMovimentacaoAbastecimento.CarregarGrid();
}

function editarMovimentacao(movimentacaoGrid) {
    limparCampos();
    _movimentacaoAbastecimento.Codigo.val(movimentacaoGrid.Codigo);
    BuscarPorCodigo(_movimentacaoAbastecimento, "MovimentacaoAbastecimentoSaida/BuscarPorCodigo", function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMovimentacaoAbastecimento.ExibirFiltros.visibleFade(false);
                _movimentacaoAbastecimento.Atualizar.visible(true);
                _movimentacaoAbastecimento.Excluir.visible(true);
                _movimentacaoAbastecimento.Adicionar.visible(false);
              
                RecarregarGridMovimentacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

    }, null);
}

function RecarregarGridMovimentacao() {
    _gridMovimentacaoAbastecimento.CarregarGrid();
}

function limparCampos() {
    _movimentacaoAbastecimento.Atualizar.visible(false);
    _movimentacaoAbastecimento.Excluir.visible(false);
    _movimentacaoAbastecimento.Adicionar.visible(true);
    _movimentacaoAbastecimento.QuantidadeArla32.visible(false);
    LimparCampos(_movimentacaoAbastecimento);

    BuscarMovimentacaoAbastecimento();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function formatarStrFloat(valor) {
    return valor.replace(".", ",");
}
