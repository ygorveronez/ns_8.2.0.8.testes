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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Abastecimento.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAbastecimento.js" />
/// <reference path="../../Enumeradores/EnumTipoAbastecimento.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="AbastecimentoAnexo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAbastecimento;
var _abastecimento;
var _pesquisaAbastecimento;
var _gridAbastecimentos;
var _PermissoesPersonalizadas;
var _isServicoFornecedor;

var PesquisaAbastecimento = function () {
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posto:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAbastecimento.Todos), options: EnumSituacaoAbastecimento.obterOpcoesPesquisaRequisicao(_CONFIGURACAO_TMS.HabilitarEnvioAbastecimentoExterno), def: EnumSituacaoAbastecimento.Todos });
    this.Documento = PropertyEntity({ text: "Documento:", getType: typesKnockout.string, enable: ko.observable(true) });
    this.TipoAbastecimento = PropertyEntity({ val: ko.observable(EnumTipoAbastecimento.Todos), options: EnumTipoAbastecimento.obterOpcoesPesquisa(), def: EnumTipoAbastecimento.Todos, text: "Tipo: ", eventChange: tipoAbastecimentoChange, issue: 250, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAbastecimento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Abastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Veículo:", idBtnSearch: guid(), issue: 143, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Motorista:", idBtnSearch: guid(), issue: 145, enable: ko.observable(true), visible: ko.observable(true) });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Posto:", idBtnSearch: guid(), issue: 171, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAbastecimento.Aberto), options: EnumSituacaoAbastecimento.obterOpcoes(), def: EnumSituacaoAbastecimento.Aberto });

    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Horimetro = PropertyEntity({ text: "Horímetro:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });

    this.TipoAbastecimento = PropertyEntity({ val: ko.observable(EnumTipoAbastecimento.Combustivel), options: EnumTipoAbastecimento.obterOpcoes(), def: EnumTipoAbastecimento.Combustivel, text: "*Tipo: ", enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Produto:", idBtnSearch: guid(), issue: 140, enable: ko.observable(true), eventChange: produtoBlur });
    this.Data = PropertyEntity({ text: "*Data:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.Documento = PropertyEntity({ text: "Documento:", required: false, getType: typesKnockout.string, enable: ko.observable(true), visible: ko.observable(true)  });
    this.KM = PropertyEntity({ text: "*Quilometragem:", required: true, getType: typesKnockout.int, maxlength: 10, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.Litros = PropertyEntity({ text: "*Litros:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 10, enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 8, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 10, enable: ko.observable(true), visible: ko.observable(true)  });
    this.TipoMovimento = PropertyEntity({ text: "*Movimento Financeiro:", type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), issue: 364, enable: ko.observable(true) });
    this.MediaCombustivel = PropertyEntity({ text: "Média:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable(0.0000), maxlength: 8, enable: ko.observable(false), visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ text: "Status:", required: false, getType: typesKnockout.text, val: ko.observable(""), enable: ko.observable(false), visible: ko.observable(false) });
    this.MotivoInconsistencia = PropertyEntity({ text: "Motivo Inconsistência:", required: false, getType: typesKnockout.text, val: ko.observable(""), enable: ko.observable(false), visible: ko.observable(false), maxlength: 2000 });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, getType: typesKnockout.text, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), maxlength: 5000 });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.ConfiguracaoAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Importação:", idBtnSearch: guid() });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorUnitarioMoedaEstrangeira = PropertyEntity({ text: "Valor Unit. Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(false), configDecimal: { precision: 4, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Local de Armazenamento:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) });
    this.Requisicao = PropertyEntity({ text: ko.observable("Requisição "), getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarEnvioAbastecimentoExterno) });

    this.OrdemCompra = PropertyEntity({ text: ko.observable("Requisição: "), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false) });

    this.Requisicao.val.subscribe(function (novoValor) {
        MontarTelaComoRequisicao(novoValor);
    });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        //CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        //CalcularMoedaEstrangeira();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        //ConverterValor();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        //ConverterValor();
    });

    //this.ValorTotal.val.subscribe(function (novoValor) {
    //    ConverterValorOriginal();
    //});

    //Aba Agrupamento
    this.GridAbastecimentos = PropertyEntity({ type: types.local });
    this.Abastecimentos = PropertyEntity({ type: types.event, text: "Abastecimentos", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ListaAbastecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: reabrirClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar/Cancelar", visible: ko.observable(true) });
    this.AtualizarKM = PropertyEntity({ eventClick: atualizarKmClick, type: types.event, text: "Atualizar KM", visible: ko.observable(false) });
    this.AtualizarHorimetro = PropertyEntity({ eventClick: atualizarHorimetroClick, type: types.event, text: "Atualizar Horímetro", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAbastecimento() {

    _pesquisaAbastecimento = new PesquisaAbastecimento();
    KoBindings(_pesquisaAbastecimento, "knockoutPesquisaAbastecimento", false, _pesquisaAbastecimento.Pesquisar.id);

    _abastecimento = new Abastecimento();
    KoBindings(_abastecimento, "knockoutCadastroAbastecimento");

    BuscarVeiculos(_pesquisaAbastecimento.Veiculo);
    BuscarVeiculos(_abastecimento.Veiculo, retornoSelecaoVeiculo, null, null, null, null, null, null, null, null, null, null, _abastecimento.Data);

    BuscarEquipamentos(_pesquisaAbastecimento.Equipamento);
    BuscarEquipamentos(_abastecimento.Equipamento, RetornoBuscarEquipamentos);

    BuscarMotoristas(_pesquisaAbastecimento.Motorista);
    BuscarMotoristas(_abastecimento.Motorista);

    BuscarClientes(_pesquisaAbastecimento.Posto, null, false, [EnumModalidadePessoa.Fornecedor]);
    BuscarClientes(_abastecimento.Posto, RetornoPosto, false, [EnumModalidadePessoa.Fornecedor], null, null, null, null, null, null, null, null, _abastecimento.TipoAbastecimento);

    BuscarConfiguracaoAbastecimento(_abastecimento.ConfiguracaoAbastecimento);
    BuscarTipoMovimento(_abastecimento.TipoMovimento);

    BuscarProdutoTMS(_abastecimento.Produto, RetornoProdutoTMS, _abastecimento.TipoAbastecimento);
    BuscarCentroResultado(_abastecimento.CentroResultado);
    BuscarCentroResultado(_pesquisaAbastecimento.CentroResultado);
    BuscarTransportadores(_pesquisaAbastecimento.Empresa, null, null, null, null, null, null, null, null, null, true);

    BuscarLocalArmazenamentoProduto(_abastecimento.LocalArmazenamento);

    if (_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa)
        _pesquisaAbastecimento.Empresa.visible(true);

    let menuOpcoesAbastecimento = {
        tipo: TypeOptionMenu.link, tamanho: 4, opcoes: [{
            descricao: "Desagrupar", id: guid(), metodo: function (data) {
                ExcluirAbastecimentosClick(_abastecimento.Abastecimentos, data);
            }
        }]
    };

    let headerAbastecimentos = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Veículo", width: "10%" },
        { data: "TipoVeiculo", title: "Tipo Veículo", width: "10%" },
        { data: "Posto", title: "Posto", width: "28%" },
        { data: "Data", title: "Data Abastecimento", width: "15%" },
        { data: "DescricaoEquipamento", title: "Equipamento", width: "10%" },
        { data: "KM", title: "KM", width: "10%" },
        { data: "NumeroAcertos", title: "Acerto", width: "8%", visible: false },
        { data: "Horimetro", title: "Horímetro", width: "10%" },
        { data: "Litros", title: "Litros", width: "10%" },
        { data: "DescricaoTipoAbastecimento", title: "Tipo", width: "12%" },
        { data: "MediaCombustivel", title: "Média", width: "10%", visible: false },
        { data: "CodigoEquipamento", visible: false },
        { data: "MediaHorimetro", title: "Média Horímetro", width: "12%", visible: false }
    ];

    _gridAbastecimentos = new BasicDataTable(_abastecimento.GridAbastecimentos.id, headerAbastecimentos, menuOpcoesAbastecimento, { column: 1, dir: orderDir.desc });

    BuscarAbastecimentos(_abastecimento.Abastecimentos, AdicionarAbastecimentoAgrupadoClick, _gridAbastecimentos, _abastecimento.Veiculo, _abastecimento.Produto, EnumSituacaoAbastecimento.Aberto, _abastecimento.Codigo);

    _abastecimento.Abastecimentos.basicTable = _gridAbastecimentos;
    RecarregarGridAbastecimentos();

    HeaderAuditoria("Abastecimento", _abastecimento);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _abastecimento.MoedaCotacaoBancoCentral.visible(true);
        _abastecimento.DataBaseCRT.visible(true);
        _abastecimento.ValorMoedaCotacao.visible(true);
        _abastecimento.ValorUnitarioMoedaEstrangeira.visible(true);
        _abastecimento.ValorOriginalMoedaEstrangeira.visible(true);
    }

    carregaSubscribes();
    buscarAbastecimento();
    loadAnexosAbastecimento();
    HabilitarDesabilitarTabAnexo();
    MontarTelaComoRequisicao(false);

    $('#liTabAgrupamento').hide();
}

function produtoBlur() {
    if (_abastecimento.Produto.val() == "") {
        _abastecimento.ValorUnitario.val(0.0000);
    }
}

function ExcluirAbastecimentosClick(e, dataSelecao) {
    if (_abastecimento.Situacao.val() !== EnumSituacaoAbastecimento.Aberto) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este abastecimento não se encontra em aberto.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja desagrupar o abastecimento selecionado?", function () {
        let data = {
            Codigo: dataSelecao.Codigo,
            CodigoAbastecimento: _abastecimento.Codigo.val()
        };
        executarReST("Abastecimento/RemoverAbastecimento", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                _abastecimento.Litros.val(arg.Data.Litros);
                calculaLitrosAbascimento();
                let abastecimentoGrid = e.basicTable.BuscarRegistros();
                for (let i = 0; i < abastecimentoGrid.length; i++) {
                    if (dataSelecao.Codigo == abastecimentoGrid[i].Codigo) {
                        abastecimentoGrid.splice(i, 1);
                        break;
                    }
                }
                e.basicTable.CarregarGrid(abastecimentoGrid);
            }
        });
    });
}

function AdicionarAbastecimentoAgrupadoClick(r) {
    if (_abastecimento.Situacao.val() !== EnumSituacaoAbastecimento.Aberto) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este abastecimento não se encontra em aberto.");
        return;
    }

    if (r !== null) {
        let abastecimentos = _gridAbastecimentos.BuscarRegistros();
        for (let i = 0; i < r.length; i++)
            abastecimentos.push({
                Codigo: r[i].Codigo,
                Placa: r[i].Placa,
                TipoVeiculo: r[i].TipoVeiculo,
                Posto: r[i].Posto,
                Data: r[i].Data,
                DescricaoEquipamento: r[i].DescricaoEquipamento,
                KM: r[i].KM,
                NumeroAcertos: r[i].NumeroAcertos,
                Horimetro: r[i].Horimetro,
                Litros: r[i].Litros,
                DescricaoTipoAbastecimento: r[i].DescricaoTipoAbastecimento,
                MediaCombustivel: r[i].MediaCombustivel,
                CodigoEquipamento: r[i].CodigoEquipamento,
                MediaHorimetro: r[i].MediaHorimetro,
                DescricaoSituacao: r[i].DescricaoSituacao,
                Observacao: r[i].Observacao,
                MotivoInconsistencia: r[i].MotivoInconsistencia
            });

        _gridAbastecimentos.CarregarGrid(abastecimentos);

        let data = {
            Abastecimentos: JSON.stringify(abastecimentos),
            CodigoAbastecimento: _abastecimento.Codigo.val()
        };
        executarReST("Abastecimento/AdicionarAbastecimento", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                _abastecimento.Litros.val(arg.Data.Litros);
                calculaLitrosAbascimento();
            }
        });
    }
}

function VerificarSeServicoFornecedor() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) 
        _isServicoFornecedor = true;
    else
        _isServicoFornecedor = false;
       
}

function MontarTelaComoRequisicao(novoValor) {
    VerificarSeServicoFornecedor();

    if (_isServicoFornecedor) {
        _abastecimento.Adicionar.visible(false);
        _abastecimento.Excluir.visible(false);
    }
    if (_abastecimento.Codigo.val() != 0) {

        if (novoValor && _isServicoFornecedor) {
            _abastecimento.Requisicao.visible(false);
            _abastecimento.Data.visible(true);
            _abastecimento.Horimetro.visible(true);
            _abastecimento.Motorista.visible(true);             
            _abastecimento.Documento.visible(true);
            _abastecimento.KM.visible(true);
            _abastecimento.ValorTotal.visible(true);
            _abastecimento.ValorUnitario.visible(true);
            _abastecimento.Observacao.visible(true);

            _abastecimento.Horimetro.enable(false);
            _abastecimento.CentroResultado.enable(false);
            _abastecimento.Litros.enable(false);
            _abastecimento.Produto.enable(false);
            _abastecimento.Posto.enable(false);
            _abastecimento.TipoAbastecimento.enable(false);
            _abastecimento.Equipamento.enable(false);
            _abastecimento.Veiculo.enable(false);

        }
        else {
            _abastecimento.Requisicao.visible(_CONFIGURACAO_TMS.HabilitarEnvioAbastecimentoExterno);
            _abastecimento.Data.visible(!novoValor);
            _abastecimento.CentroResultado.visible(!novoValor);
            _abastecimento.Documento.visible(!novoValor);
            _abastecimento.KM.visible(!novoValor);
            _abastecimento.ValorTotal.visible(!novoValor);
            _abastecimento.ValorUnitario.visible(!novoValor);
            _abastecimento.Observacao.visible(!novoValor);

            _abastecimento.Motorista.required(false);
            _abastecimento.Motorista.visible(true);
            _abastecimento.Horimetro.enable(true);
            _abastecimento.CentroResultado.enable(true);
            _abastecimento.Litros.enable(true);
            _abastecimento.Produto.enable(true);
            _abastecimento.Posto.enable(true);
            _abastecimento.TipoAbastecimento.enable(true);
            _abastecimento.Equipamento.enable(true);
            _abastecimento.Veiculo.enable(true);
        }
    }
    else {
        
        _abastecimento.Requisicao.visible(_CONFIGURACAO_TMS.HabilitarEnvioAbastecimentoExterno);
        _abastecimento.Data.visible(!novoValor);
        _abastecimento.Horimetro.visible(!novoValor);
        
        _abastecimento.CentroResultado.visible(!novoValor);
        _abastecimento.Documento.visible(!novoValor);
        _abastecimento.KM.visible(!novoValor);
        _abastecimento.ValorTotal.visible(!novoValor);
        _abastecimento.ValorUnitario.visible(!novoValor);
        _abastecimento.Observacao.visible(!novoValor);

        _abastecimento.Motorista.required(false);
        _abastecimento.Motorista.visible(true);
        _abastecimento.Litros.enable(true);
        _abastecimento.Produto.enable(true);
        _abastecimento.Posto.enable(true);
        _abastecimento.TipoAbastecimento.enable(true);
        _abastecimento.Equipamento.enable(true);
        _abastecimento.Veiculo.enable(true);

    }
    
}
function RecarregarGridAbastecimentos() {
    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_abastecimento.ListaAbastecimentos.val())) {
        $.each(_abastecimento.ListaAbastecimentos.val(), function (i, abastecimento) {
            let abastecimentoGrid = new Object();

            abastecimentoGrid.Codigo = abastecimento.Codigo;
            abastecimentoGrid.Placa = abastecimento.Placa;
            abastecimentoGrid.tipoVeiculo = abastecimento.TipoVeiculo;
            abastecimentoGrid.Posto = abastecimento.Posto;
            abastecimentoGrid.Data = abastecimento.Data;
            abastecimentoGrid.DescricaoEquipamento = abastecimento.DescricaoEquipamento;
            abastecimentoGrid.KM = abastecimento.KM;
            abastecimentoGrid.NumeroAcertos = abastecimento.NumeroAcertos;
            abastecimentoGrid.Horimetro = abastecimento.Horimetro;
            abastecimentoGrid.Litros = abastecimento.Litros;
            abastecimentoGrid.DescricaoTipoAbastecimento = abastecimento.DescricaoTipoAbastecimento;
            abastecimentoGrid.MediaCombustivel = abastecimento.MediaCombustivel;
            abastecimentoGrid.CodigoEquipamento = abastecimento.CodigoEquipamento;
            abastecimentoGrid.MediaHorimetro = abastecimento.MediaHorimetro;

            data.push(abastecimentoGrid);
        });
    }

    _gridAbastecimentos.CarregarGrid(data);
}

function RetornoBuscarEquipamentos(data) {
    _abastecimento.Equipamento.val(data.Descricao);
    _abastecimento.Equipamento.codEntity(data.Codigo);

    if (data.CodigoCentroResultado > 0) {
        _abastecimento.CentroResultado.codEntity(data.CodigoCentroResultado);
        _abastecimento.CentroResultado.val(data.CentroResultado);
    }

    if (_abastecimento.Motorista.codEntity() === undefined || _abastecimento.Motorista.codEntity() === null || _abastecimento.Motorista.codEntity() === 0) {
        let data = { Codigo: _abastecimento.Equipamento.codEntity() };
        executarReST("Veiculo/BuscaMotoristaConjunto", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    if (arg.Data.Motorista != null & arg.Data.Motorista != "" & arg.Data.CodigoMotorista > 0) {
                        _abastecimento.Motorista.val(arg.Data.Motorista);
                        _abastecimento.Motorista.codEntity(arg.Data.CodigoMotorista);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    _abastecimento.Horimetro.val(0);
}


function ControleEquipamento(tipoVeiculo, buscarEquipamentoPadrao) {
    if (tipoVeiculo == "Reboque") {
        _abastecimento.Equipamento.visible(true);
        _abastecimento.Horimetro.visible(true);
    }
    else {
        _abastecimento.Equipamento.visible(true);
        _abastecimento.Horimetro.visible(true);
    }
    if (buscarEquipamentoPadrao && _abastecimento.Veiculo.codEntity() > 0) {
        let data = { Codigo: _abastecimento.Veiculo.codEntity() };
        executarReST("Veiculo/BuscaEquipamentoPadrao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _abastecimento.Equipamento.val(arg.Data.Descricao);
                    _abastecimento.Equipamento.codEntity(arg.Data.Codigo);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function RetornoPosto(data) {
    _abastecimento.Posto.val(data.Descricao);
    _abastecimento.Posto.codEntity(data.Codigo);
    
    if (data.CodigoCombustivel > 0) {
        LimparCampoEntity(_abastecimento.Produto);
        _abastecimento.Produto.codEntity(data.CodigoCombustivel);
        _abastecimento.Produto.val(data.DescricaoCombustivel);

        _abastecimento.ValorUnitario.val(data.ValorCombustivel);

        if (_abastecimento.Produto.val() != null && _abastecimento.Produto.val() != "" && _abastecimento.Posto.val() != null && _abastecimento.Posto.val() != "") {
            executarReST("Produto/CustoProduto", { Produto: _abastecimento.Produto.codEntity(), Filial: _abastecimento.Posto.codEntity(), Data: _abastecimento.Data.val() }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data.ValorMoedaCotacao != "0,0000000000" && retorno.Data.ValorMoedaCotacao != "")
                        _abastecimento.ValorMoedaCotacao.val(retorno.Data.ValorMoedaCotacao);
                    _abastecimento.MoedaCotacaoBancoCentral.val(retorno.Data.MoedaCotacaoBancoCentral);
                    
                    if (parseFloat(_abastecimento.ValorUnitario.val()) == 0)
                        _abastecimento.ValorUnitario.val(retorno.Data.UltimoCusto);

                    if (parseFloat(retorno.Data.UltimoCusto) != 0 && parseFloat(retorno.Data.UltimoCusto) != 0 != parseFloat(_abastecimento.ValorUnitario.val()))
                        _abastecimento.ValorUnitario.val(retorno.Data.UltimoCusto);
                }
            }, null);
        }
    }

    if (_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) {
        executarReST("Cliente/ObterDetalhesPorCPFCNPJ", { CPF_CNPJ: _abastecimento.Posto.codEntity() }, function (retorno) {
            if (retorno.Success) {
                ObrigarLocalArmazenamento(retorno.Data.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento);
            }
        }, null);
    }
}

function RetornoProdutoTMS(data) {
    let produtoAnterior = _abastecimento.Produto.codEntity();
    _abastecimento.Produto.val(data.Descricao);
    _abastecimento.Produto.codEntity(data.Codigo);
    let alterouProduto = (produtoAnterior != _abastecimento.Produto.codEntity())
    if (_abastecimento.ValorUnitario.val() == "" || _abastecimento.ValorUnitario.val() == null || _abastecimento.ValorUnitario.val() == "0,0000" || alterouProduto) {
        if (_abastecimento.Produto.val() != null && _abastecimento.Produto.val() != "" && _abastecimento.Posto.val() != null && _abastecimento.Posto.val() != "") {
            executarReST("Produto/CustoProduto", { Produto: _abastecimento.Produto.codEntity(), Filial: _abastecimento.Posto.codEntity(), Data: _abastecimento.Data.val() }, function (retorno) {
                if (retorno.Success)
                    _abastecimento.ValorUnitario.val(retorno.Data.UltimoCusto);
            }, null);
        }
        else
            _abastecimento.ValorUnitario.val(data.UltimoCustoCombustivel);
    }
}

function tipoAbastecimentoChange(e, sender) {
    LimparCampoEntity(_abastecimento.Produto);
}

function importarClick(e, sender) {
    let file = document.getElementById(_abastecimento.Arquivo.id);

    let formData = new FormData();
    formData.append("upload", file.files[0]);

    _abastecimento.ConfiguracaoAbastecimento.requiredClass("form-control");
    _abastecimento.Arquivo.requiredClass("form-control");

    if (_abastecimento.ConfiguracaoAbastecimento.val() != "" && _abastecimento.Arquivo.val() != "") {
        enviarArquivo("Abastecimento/ImportarAbastecimentos?callback=?", { Codigo: _abastecimento.ConfiguracaoAbastecimento.codEntity() }, formData, function (arg) {
            if (arg.Success) {
                if (arg.Msg == "Importação dos abastecimentos foi realizada com sucesso.") {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                } else {
                    $('#contentInformacaoImportacao').html("");
                    $('#contentInformacaoImportacao').html(arg.Msg);
                
                    Global.abrirModal("ModalImportacaoAbastecimento");
                }

                //exibirMensagem(tipoMensagem.aviso, "Sucesso", arg.Msg, 1000000);//"Importação dos abastecimentos foi realizada com sucesso.");
                _abastecimento.ConfiguracaoAbastecimento.requiredClass("form-control");
                _abastecimento.Arquivo.requiredClass("form-control");
                _abastecimento.Arquivo.val("");
                buscarAbastecimento();
            } else {
                $('#contentInformacaoImportacao').html("");
                $('#contentInformacaoImportacao').html(arg.Msg);
                
                Global.abrirModal("ModalImportacaoAbastecimento");
                //exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios para a importação!");
        if (_abastecimento.ConfiguracaoAbastecimento.val() == "")
            _abastecimento.ConfiguracaoAbastecimento.requiredClass("form-control is-invalid");
        if (_abastecimento.Arquivo.val() == "")
            _abastecimento.Arquivo.requiredClass("form-control is-invalid");
    }
}

function calculaLitrosAbascimento(e) {
    let litros = 0;
    let valorTotal = 0;
    let valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (litros > 0) {
        if (valorUnitario > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        } else if (valorTotal > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorUnitarioAbascimento(e) {
    let litros = 0;
    let valorTotal = 0;
    let valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorUnitario > 0) {
        if (litros > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    } else if (valorTotal > 0) {
        if (litros > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorTotalAbascimento(e) {
    let litros = 0;
    let valorTotal = 0;
    let valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorTotal > 0) {
        if (litros > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    } else if (valorUnitario > 0) {
        if (litros > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    }
}

function retornoSelecaoVeiculo(data) {

    if (_CONFIGURACAO_TMS.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && _abastecimento.Data.val() != "" && _abastecimento.Data.val() != null && (_abastecimento.Motorista.codEntity() == 0 || _abastecimento.Motorista.codEntity == null)) {
        executarReST("Infracao/BuscarMotorista", { Veiculo: data.Codigo, Data: _abastecimento.Data.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _abastecimento.Motorista.codEntity(retorno.Data.Codigo);
                    _abastecimento.Motorista.val(retorno.Data.Nome);
                }
                else if (data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
                    _abastecimento.Motorista.val(data.Motorista);
                    _abastecimento.Motorista.codEntity(data.CodigoMotorista);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }

    if (!_CONFIGURACAO_TMS.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
        _abastecimento.Motorista.val(data.Motorista);
        _abastecimento.Motorista.codEntity(data.CodigoMotorista);
    }

    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        if (data.TipoPropriedade == "T")
            _abastecimento.Veiculo.val(data.Placa + " (TERCEIRO) " + data.ModeloVeicularCarga);
        else
            _abastecimento.Veiculo.val(data.Placa + " (PRÓPRIO) " + data.ModeloVeicularCarga);
        _abastecimento.Veiculo.codEntity(data.Codigo);
        tipoAbastecimentoChange();
        if (parseFloat(data.UltimoKMAbastecimento) > 0)
            _abastecimento.KM.val(parseFloat(data.UltimoKMAbastecimento));
        else
            _abastecimento.KM.val(0);
    }

    if (data.CodigoCentroResultado > 0) {
        _abastecimento.CentroResultado.codEntity(data.CodigoCentroResultado);
        _abastecimento.CentroResultado.val(data.CentroResultado);
    }

    ControleEquipamento(data.TipoVeiculo, true);

    // Exibe movimento financeiro quando veiculo for de terceiro
    if (data.TipoPropriedade == "T") {
        _abastecimento.TipoMovimento.visible(true);
        _abastecimento.TipoMovimento.required(true);
    }
    else {
        _abastecimento.TipoMovimento.visible(false);
        _abastecimento.TipoMovimento.required(false);
    }

    MontarTelaComoRequisicao(_abastecimento.Requisicao.val());
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(_abastecimento, "Abastecimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridAbastecimento.CarregarGrid();
                limparCamposAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function finalizarOrdemCompra() {
    resetarTabs();
    executarReST("OrdemCompra/FinalizarPorRequisicao", { Codigo: _abastecimento.OrdemCompra.val() }, function (arg) {
    if (arg.Success) {
        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }
    }, null);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(_abastecimento, "Abastecimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                if (_isServicoFornecedor == true)
                    finalizarOrdemCompra();
                _gridAbastecimento.CarregarGrid();
                limparCamposAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarKmClick() {
    resetarTabs();
    executarReST("Abastecimento/AtualizarKilometragem", { Codigo: _abastecimento.Codigo.val(), KM: _abastecimento.KM.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAbastecimento.CarregarGrid();
                limparCamposAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function atualizarHorimetroClick() {
    resetarTabs();
    executarReST("Abastecimento/AtualizarHorimetro", { Codigo: _abastecimento.Codigo.val(), Horimetro: _abastecimento.Horimetro.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAbastecimento.CarregarGrid();
                limparCamposAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function excluirClick(e, sender) {
    resetarTabs();
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o abastecimento do veículo " + _abastecimento.Veiculo.val() + "?", function () {
        ExcluirPorCodigo(_abastecimento, "Abastecimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAbastecimento.CarregarGrid();
                    limparCamposAbastecimento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function reabrirClick(e, sender) {
    resetarTabs();
    exibirConfirmacao("Confirmação", "Tem certeza que deseja reabrir o abastecimento do veículo " + _abastecimento.Veiculo.val() + "?", function () {
        ExcluirPorCodigo(_abastecimento, "Abastecimento/Reabrir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reaberto com sucesso");
                    _gridAbastecimento.CarregarGrid();

                    _abastecimento.Atualizar.visible(true);
                    _abastecimento.Excluir.visible(true);
                    _abastecimento.Reabrir.visible(false);
                    _abastecimento.AtualizarKM.visible(false);
                    _abastecimento.AtualizarHorimetro.visible(false);

                    AlternarCampos(true);
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
    limparCamposAbastecimento();
}

//*******MÉTODOS*******

function carregaSubscribes() {
    // Exibe movimento financeiro quando veiculo por de terceiro
    _abastecimento.Veiculo.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0) {
            _abastecimento.TipoMovimento.visible(false);
            _abastecimento.TipoMovimento.required(false);
        }
    });
    _abastecimento.Veiculo.val.subscribe(function (novoValor) {
        if (novoValor == "") _abastecimento.Veiculo.codEntity(0);
    });
}

function editarAbastecimento(abastecimentoGrid) {
    limparCamposAbastecimento();
    _abastecimento.Codigo.val(abastecimentoGrid.Codigo);
    BuscarPorCodigo(_abastecimento, "Abastecimento/BuscarPorCodigo", function (arg) {
        ControleEquipamento(arg.Data.TipoVeiculo, false);

        _abastecimento.MediaCombustivel.visible(true);
        _abastecimento.DescricaoSituacao.visible(true);
        _abastecimento.MotivoInconsistencia.visible(true);
        _abastecimento.OrdemCompra.visible(_isServicoFornecedor);

        _pesquisaAbastecimento.ExibirFiltros.visibleFade(false);

        if (arg.Data.Situacao == EnumSituacaoAbastecimento.Aberto)
            $('#liTabAgrupamento').show();
        else
            $('#liTabAgrupamento').hide();

        // Acoes de acordo com a situacao
        if (arg.Data.Situacao == EnumSituacaoAbastecimento.Fechado || arg.Data.Situacao == EnumSituacaoAbastecimento.Agrupado) {
            _abastecimento.Atualizar.visible(false);
            _abastecimento.Excluir.visible(false);
            _abastecimento.Reabrir.visible(true);

            AlternarCampos(false);

            if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarQuilometragemAbastecimentoFechado, _PermissoesPersonalizadas)) {
                _abastecimento.KM.enable(true);
                _abastecimento.AtualizarKM.visible(true);
            }
            else
                _abastecimento.AtualizarKM.visible(false);

            if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarHorimetroAbastecimentoFechado, _PermissoesPersonalizadas)) {
                _abastecimento.Horimetro.enable(true);
                _abastecimento.AtualizarHorimetro.visible(true);
            }
            else
                _abastecimento.AtualizarHorimetro.visible(false);

        } else {
            _abastecimento.Atualizar.visible(true);

            if (!_isServicoFornecedor)
                _abastecimento.Excluir.visible(true);
            else
                _abastecimento.Data.val(null);

            _abastecimento.Reabrir.visible(false);
            _abastecimento.AtualizarKM.visible(false);
            _abastecimento.AtualizarHorimetro.visible(false);
        }
        AbaImportar(false);

        if (_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) 
            ObrigarLocalArmazenamento(arg.Data.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento);        

        // Tipo Movimento
        if (arg.Data.Veiculo != null && arg.Data.Veiculo != undefined && arg.Data.Veiculo.Tipo == "T") {
            _abastecimento.TipoMovimento.visible(true);
            _abastecimento.TipoMovimento.required(true);
        }
        else {
            _abastecimento.TipoMovimento.visible(false);
            _abastecimento.TipoMovimento.required(false);
        }

        if (arg.Data.Situacao == EnumSituacaoAbastecimento.Inconsistente) {
            _abastecimento.MotivoInconsistencia.visible(true);
        } else {
            _abastecimento.MotivoInconsistencia.visible(false);
        }

        _abastecimento.Adicionar.visible(false);
        resetarTabs();
        _abastecimento.ValorMoedaCotacao.val(arg.Data.ValorMoedaCotacao);
        _abastecimento.ValorUnitarioMoedaEstrangeira.val(arg.Data.ValorUnitarioMoedaEstrangeira);
        RecarregarGridAbastecimentos();

        HabilitarDesabilitarTabAnexo();
        EditarListarAnexosAbastecimento(arg);
    }, null);
}

function buscarAbastecimento() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAbastecimento, tamanho: "20", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    let configExportacao = {
        url: "Abastecimento/ExportarPesquisa",
        titulo: "Abastecimento"
    };

    _gridAbastecimento = new GridView(_pesquisaAbastecimento.Pesquisar.idGrid, "Abastecimento/Pesquisa", _pesquisaAbastecimento, menuOpcoes, { column: 4, dir: orderDir.desc }, null, null, null, null, null, null, null, configExportacao);
    _gridAbastecimento.CarregarGrid();
}

function limparCamposAbastecimento() {
    _abastecimento.Atualizar.visible(false);
    _abastecimento.Reabrir.visible(false);
    _abastecimento.Excluir.visible(false);
    _abastecimento.Adicionar.visible(true);
    _abastecimento.AtualizarKM.visible(false);
    _abastecimento.AtualizarHorimetro.visible(false);
    _abastecimento.Equipamento.visible(true);
    _abastecimento.Horimetro.visible(true);
    LimparCampos(_abastecimento);
    _abastecimento.MediaCombustivel.visible(false);
    _abastecimento.DescricaoSituacao.visible(false);
    _abastecimento.MotivoInconsistencia.visible(false);
    resetarTabs();
    ObrigarLocalArmazenamento(false);
    AlternarCampos(true);
    AbaImportar(true);
    RecarregarGridAbastecimentos();
    $('#liTabAgrupamento').hide();
    $("#liTabAnexos").hide();
    limparAnexosAbastecimentoTela();
}

function AbaImportar(status) {
    if (status)
        $("#liTabImportacao").show();
    else
        $("#liTabImportacao").hide();
}

function ObrigarLocalArmazenamento(status) {
    _abastecimento.LocalArmazenamento.required(status);
    if (status)
        _abastecimento.LocalArmazenamento.text("*Local de Armazenamento:");
    else
        _abastecimento.LocalArmazenamento.text("Local de Armazenamento:");
}

function AlternarCampos(status) {
    _abastecimento.Veiculo.enable(status);
    _abastecimento.Motorista.enable(status);
    _abastecimento.Posto.enable(status);
    _abastecimento.TipoAbastecimento.enable(status);
    _abastecimento.Produto.enable(status);
    _abastecimento.Data.enable(status);
    _abastecimento.KM.enable(status);
    _abastecimento.Litros.enable(status);
    _abastecimento.ValorUnitario.enable(status);
    _abastecimento.ValorTotal.enable(status);
    _abastecimento.TipoMovimento.enable(status);
    _abastecimento.Documento.enable(status);
    _abastecimento.MoedaCotacaoBancoCentral.enable(status);
    _abastecimento.DataBaseCRT.enable(status);
    _abastecimento.ValorMoedaCotacao.enable(status);
    _abastecimento.ValorUnitarioMoedaEstrangeira.enable(false);
    _abastecimento.ValorOriginalMoedaEstrangeira.enable(status);
    _abastecimento.Equipamento.enable(status);
    _abastecimento.Horimetro.enable(status);
    _abastecimento.Abastecimentos.enable(status);
    _abastecimento.Observacao.enable(status);
    _abastecimento.CentroResultado.enable(status);
    _abastecimento.LocalArmazenamento.enable(status);
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        if (_abastecimento.Codigo.val() == null || _abastecimento.Codigo.val() == 0)
            _abastecimento.ValorUnitario.val("0,0000");
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _abastecimento.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _abastecimento.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data != undefined && r.Data > 0)
                    _abastecimento.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}
function HabilitarDesabilitarTabAnexo() {

    if (_CONFIGURACAO_TMS.HabilitarEnvioAbastecimentoExterno && _abastecimento.Codigo.val() != 0)
        $("#liTabAnexos").show();
    else
        $("#liTabAnexos").hide();

}
function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        let valorMoedaCotacao = Globalize.parseFloat(_abastecimento.ValorMoedaCotacao.val());
        let valorOriginal = Globalize.parseFloat(_abastecimento.ValorOriginalMoedaEstrangeira.val());
        let litros = Globalize.parseFloat(_abastecimento.Litros.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
            if (litros > 0)
                _abastecimento.ValorUnitarioMoedaEstrangeira.val(Globalize.format(valorOriginal / litros, "n4"));
            //calculaValorUnitarioAbascimento();
        }
    }
}

function ConverterValorOriginal() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        let valorMoedaCotacao = Globalize.parseFloat(_abastecimento.ValorMoedaCotacao.val());
        let valorOriginal = Globalize.parseFloat(_abastecimento.ValorTotal.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _abastecimento.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}