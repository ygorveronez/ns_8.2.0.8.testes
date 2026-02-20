/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
/// <reference path="Ocorrencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVeiculosContrato;
var _gridVeiculosContrato;
var _buscarContratoFreteFranquia;

/*
 * Declaração das Funções de Inicialização
 */

function loadContratoFreteOcorrencia() {
    _buscarContratoFreteFranquia = new BuscarContratoFreteFranquia(_ocorrencia.ContratoFreteTransportador, _ocorrencia, retornoConsultaContratoFrete);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function boxMotoristaContratoChange() {
    // Obtem total dos motoristas
    var valoresMotorista = Globalize.parseFloat(_ocorrencia.ValorQuinzenalMotorista.val() + "");
    if (valoresMotorista == 0)
        valoresMotorista = _ocorrencia.QuantidadeMotorista.val() * Globalize.parseFloat(_ocorrencia.ValorDiarioMotorista.val() + "") * _ocorrencia.QuantidadeDiasMotorista.val();

    // Formata
    _ocorrencia.TotalMotorista.val(Globalize.format(valoresMotorista, "n2"));

    CalcularValorOcorrenciaContrato();
}

/*
 * Declaração das Funções Públicas
 */

function BuscarContratoFreteTransportadorReferenteAosDados() {
    if (_ocorrencia.Empresa.codEntity() > 0) {
        _ocorrencia.ContratoFreteTransportador.enable(true);
        _buscarContratoFreteFranquia.ExecuteSearch();

        setTimeout(function () {
        }, 5000);
    }
    else
        _ocorrencia.ContratoFreteTransportador.enable(false);
}

function BuscarMotoristaContrato(callback) {
    var dado = {
        Ocorrencia: _ocorrencia.Codigo.val(),
        TipoOcorrencia: _ocorrencia.TipoOcorrencia.codEntity()
    };
    executarReST("OcorrenciaContratoMotorista/BuscarDadosContrato", dado, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (callback) callback();

                _ocorrencia.QuantidadeMotorista.val(arg.Data.QuantidadeMotoristas);
                _ocorrencia.ValorDiarioMotorista.val(arg.Data.ValorDiario);
                _ocorrencia.ValorQuinzenalMotorista.val(arg.Data.ValorQuinzenal);
                //_ocorrencia.QuantidadeDiasMotorista.val(arg.Data.QuantidadeDiasMotorista);
                _ocorrencia.TotalMotorista.val(arg.Data.TotalOcorrenciaMotorista);
                _ocorrencia.DescontarValoresOutrasCargas.enable(arg.Data.DescontarValoresOutrasCargas);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function CalcularValorOcorrenciaContrato() {
    var total = 0;

    if (configuracaoTipoOcorrencia.OcorrenciaComVeiculo)
        total = CalcularValorOcorrenciaContratoComVeiculo();
    else
        total = CalcularValorOcorrenciaContratoListaVeiculos();

    // Formata
    _ocorrencia.ValorOcorrencia.val(Globalize.format(total, "n2"));
}

function GetGridVeiculosContrato() {
    if (!_gridVeiculosContrato || !("BuscarRegistros" in _gridVeiculosContrato))
        return "[]";

    var copiaDados = _gridVeiculosContrato.BuscarRegistros().slice();
    var data = copiaDados.map(function (veiculo) {
        return {
            ValorDiaria: Globalize.parseFloat(veiculo.ValorDiaria + ""),
            ValorQuinzena: Globalize.parseFloat(veiculo.ValorQuinzena + ""),
            Total: Globalize.parseFloat(veiculo.Total + ""),
            Codigo: veiculo.Codigo,
            CodigoVeiculo: veiculo.CodigoVeiculo,
            QuantidadeDias: veiculo.QuantidadeDias,
            QuantidadeDocumentos: veiculo.QuantidadeDocumentos,
            ValorDocumentos: veiculo.ValorDocumentos,
            Veiculo: veiculo.Veiculo
        };
    });
    return JSON.stringify(data);
}

function RecarregarGridImprodutividade(callback) {
    if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.OcorrenciaDestinadaFranquias) {
        _gridVeiculosImprodutivos = new GridView(_ocorrencia.VeiculosImprodutivos.idGrid, "Ocorrencia/ObterVeiculosImprodutivos", _ocorrencia);
        _gridVeiculosImprodutivos.CarregarGrid(callback);
    }
}

function SubscribersImprodutividade() {
    _ocorrencia.ContratoFreteTransportador.codEntity.subscribe(function (novoCodigoContratoFrete) {
        RecarregarGridImprodutividade();

        if (novoCodigoContratoFrete == 0) {
            _ocorrencia.ContratoFreteTransportador.periodoAcordo = EnumPeriodoAcordoContratoFreteTransportador.NaoPossui;

            preencherDadosPeriodoOcorrencia();
            controlarPeriodoOcorrencia();
        }
    });

    _ocorrencia.Empresa.codEntity.subscribe(function (novoCodigoTransportador) {
        RecarregarGridImprodutividade();
    });
}

function VerificarDescontoOutrasCargasContrato() {
    if (_ocorrencia.DescontarValoresOutrasCargas.enable()) {
        var totalValorDocumentos = ObtemDescontoVeiculosContrato();
        var totalDocumentos = ObtemDocumentosVeiculosContrato();

        if (totalValorDocumentos > 0) {
            var msg = Localization.Resources.Ocorrencias.Ocorrencia.DescontoDeValorReferenteQuantiadeDocumentoEmitidos;
            msg = msg.replace("#valor#", Globalize.format(totalValorDocumentos, "n2"));
            msg = msg.replace("#quantidade#", totalDocumentos);

            _ocorrencia.DescontarValoresOutrasCargas.val(msg);

            CalcularValorOcorrenciaContrato();
        }
        else {
            _ocorrencia.DescontarValoresOutrasCargas.val("");
        }
    } else {
        _ocorrencia.DescontarValoresOutrasCargas.val("");
    }
}

/*
 * Declaração das Funções Privadas
 */

function AtualizarControleGridContratoVeiculos(dataRow, row) {
    var dias = parseInt(dataRow.QuantidadeDias);

    var dataVeiculos = _gridVeiculosContrato.BuscarRegistros().slice().map(function (veiculo) {
        if (veiculo.Codigo == dataRow.Codigo) {
            veiculo.QuantidadeDias = dias + "";
            valorDiaria = Globalize.parseFloat(veiculo.ValorDiaria + "");
            valorQuinzena = Globalize.parseFloat(veiculo.ValorQuinzena + "");

            if (valorQuinzena == 0 && valorDiaria > 0)
                veiculo.Total = Globalize.format(valorDiaria * dias, "n2");
        }
        return veiculo;
    });
    _gridVeiculosContrato.CarregarGrid(dataVeiculos);

    CalcularValorOcorrenciaContrato();
}

function BuscarVeiculosContrato(callback) {
    var anexo = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Anexar, id: guid(), metodo: anexarVeiculoContrato, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes,
        tamanho: 4,
        opcoes: [anexo]
    };

    var ko_ocorrencia = {
        Ocorrencia: _ocorrencia.Codigo,
    };

    if (_gridVeiculosContrato != null && _gridVeiculosContrato.Destroy)
        _gridVeiculosContrato.Destroy();

    if (_ocorrencia.Codigo.val() == 0) {
        CarregarBasciTableVeiculosContrato(callback, menuOpcoes);
    } else {
        _gridVeiculosContrato = new GridView(_ocorrencia.VeiculosContrato.idGrid, "OcorrenciaContratoVeiculo/Pesquisa", ko_ocorrencia, menuOpcoes);
        _gridVeiculosContrato.CarregarGrid(callback);
    }
}

function CalcularValorOcorrenciaContratoComVeiculo() {
    var total = 0;

    var kmExcedente = Globalize.parseFloat(_ocorrencia.ParametroTexto.val() + "") || 0;
    var valorPorKmExcedente = _ocorrencia.ContratoFreteTransportador.Contrato != null ? Globalize.parseFloat(_ocorrencia.ContratoFreteTransportador.Contrato.ValorKmExcedente + "") : 0;

    return kmExcedente * valorPorKmExcedente;
}

function CalcularValorOcorrenciaContratoListaVeiculos() {
    var valoresVeiculos = 0;
    var valoresMotorista = 0;
    var valorDesconto = 0;
    var total = 0;

    // Obtem totais nos veículos
    _gridVeiculosContrato.BuscarRegistros().slice().forEach(function (veiculo) {
        valoresVeiculos += Globalize.parseFloat(veiculo.Total + "");
    });

    // Obtem total dos motoristas
    valoresMotorista = Globalize.parseFloat(_ocorrencia.ValorQuinzenalMotorista.val() + "");
    if (valoresMotorista == 0)
        valoresMotorista = _ocorrencia.QuantidadeMotorista.val() * Globalize.parseFloat(_ocorrencia.ValorDiarioMotorista.val() + "") * _ocorrencia.QuantidadeDiasMotorista.val();

    // Obtem desconto (se tiver)
    if (_ocorrencia.DescontarValoresOutrasCargas.enable())
        valorDesconto = ObtemDescontoVeiculosContrato();

    // Cacula o total
    total = valoresVeiculos + valoresMotorista - valorDesconto;

    return total;
}

function CarregarBasciTableVeiculosContrato(callback, menuOpcoes) {
    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarControleGridContratoVeiculos,
    };

    var editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: ConfigInt(),
        allowZero: false,
        precision: 0,
        thousands: ".",
        type: 1
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Veiculo", title: Localization.Resources.Ocorrencias.Ocorrencia.Veiculo, width: "15%", className: "text-align-center" },
        { data: "ValorDiaria", title: Localization.Resources.Ocorrencias.Ocorrencia.ValorDiaria, width: "20%", className: "text-align-right" },
        { data: "ValorQuinzena", title: Localization.Resources.Ocorrencias.Ocorrencia.ValorQuinzena, width: "20%", className: "text-align-right" },
        { data: "QuantidadeDias", title: Localization.Resources.Ocorrencias.Ocorrencia.Dias, width: "15%", className: "text-align-right", editableCell: editable },
        { data: "Total", title: Localization.Resources.Ocorrencias.Ocorrencia.Total, width: "15%", className: "text-align-right" },
    ];

    // Quandos não for uma ocorrencia criada, usa basic table
    _gridVeiculosContrato = new BasicDataTable(_ocorrencia.VeiculosContrato.idGrid, header, menuOpcoes, null, null, null, null, null, editarColuna);

    var dados = {
        PeriodoInicio: _ocorrencia.PeriodoInicio.val(),
        PeriodoFim: _ocorrencia.PeriodoFim.val(),
        TipoOcorrencia: _ocorrencia.TipoOcorrencia.codEntity()
    };

    executarReST("OcorrenciaContratoVeiculo/ConsultaVeiculos", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridVeiculosContrato.CarregarGrid(arg.Data);
                CalcularValorOcorrenciaContrato();
                if (callback) callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ObtemDocumentosVeiculosContrato() {
    return _mapreducer("QuantidadeDocumentos");
} 

function ObtemDescontoVeiculosContrato() {
    return _mapreducer("ValorDocumentos");
}

function retornoConsultaContratoFrete(contratoSelecionado) {
    var dataInicialPeriodo = Global.criarData(obterPeriodoInicio());
    var dataFinalPeriodo = Global.criarData(obterPeriodoFim());
    var dataInicialContrato = Global.criarData(contratoSelecionado.DataInicial);
    var dataFinalContrato = Global.criarData(contratoSelecionado.DataFinal);
    
    _ocorrencia.ContratoFreteTransportador.entityDescription(contratoSelecionado.Descricao);
    _ocorrencia.ContratoFreteTransportador.val(contratoSelecionado.Descricao);
    _ocorrencia.ContratoFreteTransportador.periodoAcordo = contratoSelecionado.PeriodoAcordo;
    _ocorrencia.ContratoFreteTransportador.codEntity(contratoSelecionado.Codigo);

    if ((dataInicialContrato > dataInicialPeriodo) || (dataFinalContrato < dataFinalPeriodo))
        preencherDadosPeriodoOcorrencia(contratoSelecionado.DataInicial);

    controlarPeriodoOcorrencia();
}

function _mapreducer(prop) {
    // Slice garante que o array seja copiado sem manter referencia original
    var dadosVeiculos = _gridVeiculosContrato.BuscarRegistros().slice();

    // Map cria o array de valores a partir do array de objetos
    var arr = dadosVeiculos.map(function (obj) { return obj[prop]; });

    // Reduce sumariza todas informacoes
    return arr.reduce(function (a, b) { return a + b; }, 0);
}