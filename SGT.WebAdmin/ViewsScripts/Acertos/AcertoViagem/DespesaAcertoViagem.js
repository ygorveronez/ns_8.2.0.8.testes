/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="../../Consultas/Justificativa.js" />
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
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Pedagio.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />
/// <reference path="DiariaAcertoViagem.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _despesaAcertoViagem;
var _gridDespesa;
var _HTMLDespesaAcertoViagem;
var _HTMLDespesaDoVeiculo;
var _despesaDoVeiculo;
var _novaDespesa;
var _justificativaDespesa;
var _codigoNotaSelecionada;

var JustificativaDespesa = function () {
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: SalvarJustificativaClick, type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarJustificativaClick, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

var DespesaAcertoViagem = function () {
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PagoEmpresa = PropertyEntity({ text: "Pago pela Empresa: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PagoMotorista = PropertyEntity({ text: "Pago pelo Motorista: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PagoAdiantamento = PropertyEntity({ text: "Pago pelo Adiantamento: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.RetornarPedagio = PropertyEntity({ eventClick: RetornarPedagioClick, type: types.event, text: "Retornar Pedágios", visible: ko.observable(false), enable: ko.observable(true) });
    this.Fechamento = PropertyEntity({ eventClick: FechamentoClick, type: types.event, text: "Salvar Outras Despesas", visible: ko.observable(true), enable: ko.observable(true) });
};

var DespesaDoVeiculo = function () {
    this.Veiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });

    this.DespesasComNota = PropertyEntity({ type: types.map, required: false, text: "Buscar despesas lançadas em documento de entrada", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Despesas = PropertyEntity({ type: types.map, required: false, text: "Despesas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarDespesa = PropertyEntity({eventClick: InformarDespesaClick, type: types.event, text: "Adicionar nova despesa", visible: ko.observable(true), enable: ko.observable(false)});
};

var AdicionarDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: true, issue: 54, enable: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 10, text: "Número Documento:", enable: ko.observable(true) });
    this.DataDespesa = PropertyEntity({ getType: typesKnockout.dateTime, required: true, dataLimit: _acertoViagem.DataFinal, text: "*Data da Despesa:", issue: 2, enable: ko.observable(true) });
    this.ProdutoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto/Serviço:", idBtnSearch: guid(), required: false, issue: 140, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, visible: ko.observable(true), enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*Quantidade:"), maxlength: 10, visible: ko.observable(true), enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TiposDespesas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Despesas Acerto de Viagem", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 300, text: "Observação:", enable: ko.observable(true) });
    this.TipoPagamento = PropertyEntity({ text: "Tipo do pagamento: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, enable: ko.observable(true) });
    this.DespesaPagaPeloAdiantamento = PropertyEntity({ getType: typesKnockout.bool, text: "Esta despesa foi paga pelo adiantamento?", val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDespesaClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.DespesaPagaPeloAdiantamento.val.subscribe(function (novoValor) {
        if (novoValor)
            _novaDespesa.TipoPagamento.val("Adiantamento");
        else
            _novaDespesa.TipoPagamento.val("");
    });
};


//*******EVENTOS*******

function loadDespesaAcertoViagem() {
    $("#contentDespesaAcertoViagem").html("");
    var idDiv = guid();
    $("#contentDespesaAcertoViagem").append(_HTMLDespesaAcertoViagem.replace(/#despesaAcertoViagem/g, idDiv));
    _despesaAcertoViagem = new DespesaAcertoViagem();
    KoBindings(_despesaAcertoViagem, idDiv);

    _justificativaDespesa = new JustificativaDespesa();
    KoBindings(_justificativaDespesa, "divModalSelecaoJustificativa");

    _novaDespesa = new AdicionarDespesa();
    KoBindings(_novaDespesa, "knoutAdicionarDespesa");

    new BuscarProdutoTMS(_novaDespesa.ProdutoServico, RetornoProduto);
    new BuscarJustificativas(_novaDespesa.Justificativa, null, null, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas);
    new BuscarTiposDespesas(_novaDespesa.TiposDespesas);
    new BuscarClientes(_novaDespesa.Fornecedor, RetornoFornecedor, false, [EnumModalidadePessoa.Fornecedor]);

    new BuscarJustificativas(_justificativaDespesa.Justificativa, null, null, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _novaDespesa.MoedaCotacaoBancoCentral.visible(true);
        _novaDespesa.DataBaseCRT.visible(true);
        _novaDespesa.ValorMoedaCotacao.visible(true);
        _novaDespesa.ValorOriginalMoedaEstrangeira.visible(true);
    }
    if (_CONFIGURACAO_TMS.GerarReciboDetalhadoAcertoViagem) {
        _novaDespesa.DespesaPagaPeloAdiantamento.val(true);
        _novaDespesa.DespesaPagaPeloAdiantamento.def = true;
    }

    CarregarVeiculosDespesa();
    CarregarDespesaVeiculo(0);

    carregarConteudosFechamentoHTML(loadFechamentoAcertoViagem);
    loadDiariaAcertoViagem();
}


function CancelarJustificativaClick(e, sender) {
    Global.fecharModal('divModalSelecaoJustificativa');
    LimparCampos(_justificativaDespesa);
    _codigoNotaSelecionada = 0;
}

function SalvarJustificativaClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        var dataEnvio = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: _despesaDoVeiculo.CodigoVeiculo.val(),
            DocumentoEntrada: _codigoNotaSelecionada,
            Justificativa: e.Justificativa.codEntity()
        };
        executarReST("AcertoDespesa/InserirOutrasDespesasDeNota", dataEnvio, function (arg) {
            if (arg.Success) {
                _codigoNotaSelecionada = 0;
                Global.fecharModal("divModalSelecaoJustificativa");
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Outras despesas lançadas com sucesso.");
                LimparCampos(_justificativaDespesa);
                carregarDadosCridDespesas();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function CarregarVeiculosDespesa() {
    _despesaAcertoViagem.Veiculos.val(_acertoViagem.ListaVeiculos.val());
}

function CarregarDespesaVeiculo(indice) {
    var idDiv = guid();
    $("#contentDespesasDoVeiculo").html(_HTMLDespesaDoVeiculo.replace("#knoutDespesasDoVeiculo", idDiv + "_knoutDespesasDoVeiculo"));

    _despesaDoVeiculo = new DespesaDoVeiculo();
    KoBindings(_despesaDoVeiculo, idDiv + "_knoutDespesasDoVeiculo");

    var classActive = '"';
    if (_despesaAcertoViagem.Veiculos.val().length > 1) {
        $("#divResultadoDespesaAcertoViagem").attr("style", "margin-top: 38px");
        var html = "";
        $.each(_despesaAcertoViagem.Veiculos.val(), function (i, veiculo) {
            if (i == indice)
                html += '<li class="active" class="nav-item">';
            else
                html += '<li>';
            if (i == indice)
                html += '<a href="javascript:void(0);" class="nav-link active" data-bs-toggle="tab" onclick="CarregarDespesaVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + veiculo.Placa + '</span></a>';
            else
                html += '<a href="javascript:void(0);" class="nav-link" data-bs-toggle="tab" onclick="CarregarDespesaVeiculo(' + i + ')"><span class="hidden-mobile hidden-tablet">' + veiculo.Placa + '</span></a>';
            html += '</li>';
        });
        $("#" + _despesaDoVeiculo.Veiculo.idTab).html(html);
        $("#" + _despesaDoVeiculo.Veiculo.idTab).show();
    } else {
        $("#divResultadoDespesaAcertoViagem").attr("style", "margin-top: 0px");
    }

    if (_despesaAcertoViagem.Veiculos.val().length > 0)
        _despesaDoVeiculo.CodigoVeiculo.val(_despesaAcertoViagem.Veiculos.val()[indice].Codigo);
    else
        _despesaDoVeiculo.CodigoVeiculo.val(0);
    _despesaDoVeiculo.CodigoAcerto.val(_acertoViagem.Codigo.val());

    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoOutraDespesa", null, _despesaAcertoViagem), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverDespesaClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarDespesaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: VisibilidadeOpcaoAuditoria() ? TypeOptionMenu.list : TypeOptionMenu.link, opcoes: [excluir, editar, auditar], descricao: "Opções", tamanho: 10 };

    new BuscarDocumentoEntrada(_despesaDoVeiculo.DespesasComNota, RetornoInserirOutraDespesa, _acertoViagem.Codigo, _despesaDoVeiculo.CodigoVeiculo);

    _gridDespesa = new GridView(_despesaDoVeiculo.Despesas.idGrid, "AcertoDespesa/PesquisarOutrasDespesas", _despesaDoVeiculo, menuOpcoes, null);
    carregarDadosCridDespesas();
    VerificarBotoes();
    //Global.ResetarAbas();
}

function InformarDespesaClick(e, data) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novaDespesa);
    _novaDespesa.Adicionar.text("Adicionar");
    Global.abrirModal('divAdicionarDespesa');
    if (!_CONFIGURACAO_TMS.HabilitarControlarOutrasDespesas) {
        $("#TiposDespesasAcerto").hide();
    }
}

function RetornoFornecedor(data) {
    if (data !== null) {
        _novaDespesa.Fornecedor.val(data.Nome);
        _novaDespesa.Fornecedor.codEntity(data.Codigo);

        if (_novaDespesa.DespesaPagaPeloAdiantamento.val() == true) {
            _novaDespesa.TipoPagamento.val("Adiantamento");
        } else {
            var novaData = {
                Codigo: data.Codigo
            };
            executarReST("Pessoa/ConsultaFaturamentoFornecedor", novaData, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== null)
                        _novaDespesa.TipoPagamento.val(arg.Data.Tipo);
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", "Problemas ao consultar o tipo do faturamento do fornecedor.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        }
    }
}

function RetornoProduto(data) {
    if (data !== null) {
        _novaDespesa.ProdutoServico.val(data.Descricao);
        _novaDespesa.ProdutoServico.codEntity(data.Codigo);
        if (data.CodigoUnidadeMedida === 7) {//Serviço                        
            $("#divValorOutrasDespesas").attr("class", "col col-xs-12 col-sm-12 col-md-6 col-lg-6");
            _novaDespesa.Quantidade.visible(false);
            _novaDespesa.Quantidade.required = false;
            _novaDespesa.Quantidade.text("Quantidade:");
        } else {
            AjustarCamposValorQuantidade();
        }
    }
}

function RetornoInserirOutraDespesa(data) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    _codigoNotaSelecionada = data.Codigo;
    Global.abrirModal("divModalSelecaoJustificativa");
}


function calcularTotais(dados) {
    var total = 0;
    var pagoMotorista = 0;
    var pagoEmpresa = 0;
    var pagoAdiantamento = 0;

    if (dados && dados.data && dados.data.length > 0) {
        $.each(dados.data, function (i, despesa) {
            var valor = Globalize.parseFloat(despesa.Valor) || parseFloat(despesa.Valor.replace(',', '.'));
            var quantidade = Globalize.parseFloat(despesa.Quantidade) || parseFloat(despesa.Quantidade.replace(',', '.'));

            if (quantidade > 0) {
                valor = valor * quantidade;
            }

            total += valor;

            if (despesa.DespesaPagaPeloAdiantamento) {
                pagoAdiantamento += valor;
            } else if (despesa.FaturamentoFornecedor) {
                pagoEmpresa += valor;
            } else {
                pagoMotorista += valor;
            }
        });
    }
    _despesaAcertoViagem.PagoAdiantamento.val(Globalize.format(pagoAdiantamento, "n2"));
    _despesaAcertoViagem.PagoEmpresa.val(Globalize.format(pagoEmpresa, "n2"));
    _despesaAcertoViagem.PagoMotorista.val(Globalize.format(pagoMotorista, "n2"));
    _despesaAcertoViagem.ValorTotal.val(Globalize.format(total, "n2"));
}


function AdicionarDespesaClick(e, sender) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (_CONFIGURACAO_TMS.HabilitarControlarOutrasDespesas) {
        e.TiposDespesas.required = true;
    }
    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVeiculo: _despesaDoVeiculo.CodigoVeiculo.val(),
            Produto: e.ProdutoServico.codEntity(),
            Fornecedor: e.Fornecedor.codEntity(),
            DataDespesa: e.DataDespesa.val(),
            Valor: e.Valor.val(),
            Quantidade: e.Quantidade.val(),
            NumeroDocumento: e.NumeroDocumento.val(),
            Observacao: e.Observacao.val(),
            Justificativa: e.Justificativa.codEntity(),
            TipoDespesa: e.TiposDespesas.codEntity(),
            CodigoDespesa: e.Codigo.val(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val(),
            DespesaPagaPeloAdiantamento: e.DespesaPagaPeloAdiantamento.val()
        };
        executarReST("AcertoDespesa/InserirOutrasDespesas", data, function (arg) {
            if (arg.Success) {
                _novaDespesa.Adicionar.text("Adicionar");
                LimparCampos(e);
                AjustarCamposValorQuantidade();
                $("#" + e.Fornecedor.id).focus();
                carregarDadosCridDespesas();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}


function carregarDadosCridDespesas() {
    _gridDespesa.CarregarGrid(function (dados) {
        calcularTotais(dados);
    });
}

function EditarDespesaClick(e) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novaDespesa);
    _novaDespesa.Adicionar.text("Atualizar");

    _novaDespesa.Codigo.val(e.Codigo);
    _novaDespesa.Fornecedor.codEntity(e.CodigoFornecedor);
    _novaDespesa.Fornecedor.val(e.Fornecedor);
    _novaDespesa.NumeroDocumento.val(e.NumeroDocumento);
    _novaDespesa.DataDespesa.val(e.DataEHora);
    _novaDespesa.ProdutoServico.codEntity(e.CodigoProduto);
    _novaDespesa.ProdutoServico.val(e.Produto);
    _novaDespesa.Valor.val(e.Valor);
    _novaDespesa.Quantidade.val(e.Quantidade);
    _novaDespesa.Justificativa.codEntity(e.CodigoJustificativa);
    _novaDespesa.Justificativa.val(e.Justificativa);
    _novaDespesa.TiposDespesas.codEntity(e.CodigoTipoDespesa);
    _novaDespesa.TiposDespesas.val(e.TipoDespesa);
    _novaDespesa.Observacao.val(e.Observacao);
    _novaDespesa.TipoPagamento.val(e.PagoPor);
    _novaDespesa.DespesaPagaPeloAdiantamento.val(e.DespesaPagaPeloAdiantamento);

    _novaDespesa.MoedaCotacaoBancoCentral.val(e.CodigoMoedaCotacaoBancoCentral);
    _novaDespesa.DataBaseCRT.val(e.DataBaseCRT);
    _novaDespesa.ValorMoedaCotacao.val(e.ValorMoedaCotacao);
    _novaDespesa.ValorOriginalMoedaEstrangeira.val(e.ValorOriginalMoedaEstrangeira);

    _novaDespesa.Codigo.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Fornecedor.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.NumeroDocumento.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.DataDespesa.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.ProdutoServico.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Valor.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Quantidade.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Justificativa.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.TiposDespesas.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Observacao.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.TipoPagamento.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.Adicionar.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.DespesaPagaPeloAdiantamento.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);

    _novaDespesa.MoedaCotacaoBancoCentral.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.DataBaseCRT.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.ValorMoedaCotacao.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);
    _novaDespesa.ValorOriginalMoedaEstrangeira.enable(_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado);

    Global.abrirModal('divAdicionarDespesa');
}

function RemoverDespesaClick(e) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a despesa " + e.Produto + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoDespesa/RmoverOutrasDespesas", data, function (arg) {
            if (arg.Success) {

                carregarDadosCridDespesas();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function FechamentoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.OutrasDespesas);
    Salvar(_acertoViagem, "AcertoDespesa/AtualizarDespesas", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Outras despesas salvas com sucesso.");

                //$("#" + _etapaAcertoViagem.Etapa6.idTab).click();

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.OutrasDespesas);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function RetornarPedagioClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Pedagios);
    Salvar(_acertoViagem, "AcertoDespesa/AtualizarDespesas", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Outras despesas salvas com sucesso.");

                $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaAcertoViagem.Etapa4.idTab).click();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);

}

//*******MÉTODOS*******

function CarregarDespesas(data) {
    recarregarGridDespesas();
}

function recarregarGridDespesas() {
    CarregarVeiculosDespesa();
    CarregarDespesaVeiculo(0);
}


function CalcularMoedaEstrangeiraDespesa() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novaDespesa.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novaDespesa.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novaDespesa.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorDespesa();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorDespesa() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novaDespesa.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novaDespesa.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novaDespesa.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalDespesa() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novaDespesa.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novaDespesa.Valor.val());
        var valorOriginalMoedaEstrangeira = Globalize.parseFloat(_novaDespesa.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0 && valorOriginalMoedaEstrangeira <= 0) {
            _novaDespesa.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function carregarConteudosDespesaHTML(calback) {
    $.get("Content/Static/Acerto/DespesaAcertoViagem.html?dyn=" + guid(), function (data) {
        _HTMLDespesaAcertoViagem = data;
        $.get("Content/Static/Acerto/DespesaDoVeiculo.html?dyn=" + guid(), function (data) {
            _HTMLDespesaDoVeiculo = data;
            $.get("Content/Static/Acerto/DiariaAcertoViagem.html?dyn=" + guid(), function (data) {
                _HTMLDiariaAcertoViagem = data;
                calback();
            });
        });
    });
}

function AjustarCamposValorQuantidade() {
    $("#divValorOutrasDespesas").attr("class", "col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _novaDespesa.Quantidade.visible(true);
    _novaDespesa.Quantidade.required = true;
    _novaDespesa.Quantidade.text("*Quantidade:");
}
