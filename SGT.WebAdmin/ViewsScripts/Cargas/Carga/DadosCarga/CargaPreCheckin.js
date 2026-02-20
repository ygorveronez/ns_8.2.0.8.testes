//#region Variaveis Globais
var _etapasPreCheckin;
var _etapaAtualPreCheckin;
var _HTMLCargaPreCheckin = "";
var _visibilidadeTab = false;
var _gridAgrupamentoStagesColeta;
var _gridAgrupamentoStagesEntrega;
var _gridAgrupamentoStagesTransferencia;
var _agrupamentoStages;
var _agrupamentoStagesEntrega;
var _agrupamentoStagesTransferencia;
var _geracaoTransportes;
var _divModalAgrupamento;
var _agrupamentoCteNfs;
var _gridAgrupamentoCteNfs;
//#endregion

//#region Etapas Prechekin
var EtapaCargaPreCheckin = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Coleta", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: EtapaColeta,
        step: ko.observable(1),
        tooltip: ko.observable("É onde ira ser visualizada as stages e feito a vinculação das placas."),
        tooltipTitle: ko.observable("Coleta")
    });
    this.Etapa2 = PropertyEntity({
        text: "Transferencia", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: EtapaTranferenciaClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde sera visualizada o local onde chegarão os transportadores."),
        tooltipTitle: ko.observable("Transferencia")
    });
    this.Etapa3 = PropertyEntity({
        text: "Entrega", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: EtapaEntregaClick,
        step: ko.observable(4),
        tooltip: ko.observable("Local de destino."),
        tooltipTitle: ko.observable("Entrega")
    });

}
//#endregion

//#region Construtores
var AgrupamentoStagesPreChekin = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0) })
    this.GridAgrupamentoStages = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    this.ConfirmarPlacas = PropertyEntity({ eventClick: (e) => ConfirmarPlacas(false, e), type: types.event, text: "Confirmar Placas", visible: ko.observable(true) });
    this.RemoverPlacas = PropertyEntity({ eventClick: RemoverPlacas, type: types.event, text: "Remover Placas", visible: ko.observable(true) });
    this.LiberarComFalha = PropertyEntity({ eventClick: (e) => ConfirmarPlacas(true, e), type: types.event, text: "Liberar Com Falha", visible: ko.observable(true) });

}

var GeracaoTransportes = function () {
    this.GerarTransportes = PropertyEntity({ eventClick: () => GerarTransportes(true), type: types.event, text: "Gerar Transportes", visible: ko.observable(false) });
}

var DivModalAgrupamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.TipoAgrupamento = PropertyEntity({ val: ko.observable("") });
    this.Placa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Placa:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid() });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "2° Reboque:", idBtnSearch: guid() });
    this.Salvar = PropertyEntity({ eventClick: () => SalvarVeiculoPlaca(""), type: types.event, text: "Salvar", visible: ko.observable(true) });
}

var AgrupamentoCteNfs = function () {
    this.CodigoCarga = PropertyEntity({val: ko.observable(0)})
    this.GridAgrupamentoCteNfs = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    this.ConfirmarPlacas = PropertyEntity({ eventClick: () => ConfirmarPlacas(false), type: types.event, text: "Confirmar Placas", visible: ko.observable(true) });
    this.RemoverPlacas = PropertyEntity({ eventClick: RemoverPlacas, type: types.event, text: "Remover Placas", visible: ko.observable(true) });
}
//#endregion

//#region Funções Iniciais
function loadEtapasPreCheckin(carga, etapaTransportador) {
    ObterQuantidadeDeStages(carga).then(() => {
        $.get("Content/Static/Carga/CargaPreCheckin.html?dyn=" + guid(), function (data) {
            let idDivCargaPreCheckin;

            if (etapaTransportador)
                idDivCargaPreCheckin = carga.EtapaDadosTransportador.idGrid
            else
                idDivCargaPreCheckin = carga.EtapaInicioTMS.idGrid;

            data = data.replaceAll("#IDDadosTransporte", idDivCargaPreCheckin);

            if (!_visibilidadeTab)
                return;

            if (!etapaTransportador)
                $("#liTabCargaDadosTransportePreCheckin_" + idDivCargaPreCheckin).removeClass("d-none");
            else
                carga.EtapaDadosTransportadorPreCheckin.visible(true);

            $("#tabEtapasPreChekin_" + idDivCargaPreCheckin).html(data);

            _etapasPreCheckin = new EtapaCargaPreCheckin();
            KoBindings(_etapasPreCheckin, "knockoutDadosTransportePreCheckin_" + idDivCargaPreCheckin);

            _agrupamentoStages = new AgrupamentoStagesPreChekin();
            KoBindings(_agrupamentoStages, "knockoutAgrupamentoStagesColeta_" + idDivCargaPreCheckin);

            _agrupamentoStagesEntrega = new AgrupamentoStagesPreChekin();
            KoBindings(_agrupamentoStagesEntrega, "knockoutAgrupamentoStagesEntrega_" + idDivCargaPreCheckin);

            _agrupamentoStagesTransferencia = new AgrupamentoStagesPreChekin();
            KoBindings(_agrupamentoStagesTransferencia, "knockoutAgrupamentoStagesTransferencia_" + idDivCargaPreCheckin);

            _geracaoTransportes = new GeracaoTransportes();
            KoBindings(_geracaoTransportes, "knockoutGeracaoTransportes_" + idDivCargaPreCheckin);

            if (etapaTransportador)
                _geracaoTransportes.GerarTransportes.visible(true);

            _agrupamentoCteNfs = new AgrupamentoCteNfs();
            KoBindings(_agrupamentoCteNfs, "knockoutAgrupamentoCteNfs_" + idDivCargaPreCheckin);

            _divModalAgrupamento = new DivModalAgrupamento();
            KoBindings(_divModalAgrupamento, "knoutModalAgrupamentoStage");

            _agrupamentoCteNfs.CodigoCarga.val(carga.Codigo.val());
            _agrupamentoStages.CodigoCarga.val(carga.Codigo.val());
            _agrupamentoStagesEntrega.CodigoCarga.val(carga.Codigo.val());
            _agrupamentoStagesTransferencia.CodigoCarga.val(carga.Codigo.val());



            new BuscarMotoristas(_divModalAgrupamento.Motorista);
            new BuscarVeiculos(_divModalAgrupamento.Placa);
            new BuscarVeiculos(_divModalAgrupamento.Reboque);
            new BuscarVeiculos(_divModalAgrupamento.SegundoReboque, null, null, null, null, null, null, null, null, null, null, "1");

            $("#" + _etapasPreCheckin.Etapa1.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapasPreCheckin.Etapa1.idTab + " .step").attr("class", "step yellow");
            $("#" + _etapasPreCheckin.Etapa1.idTab).click();

            loadGridCteNfsPreChekin();
            loadGrisAgrupamentos();
            PequisarStagesAgrupamento(carga.Codigo.val());
            ObterCteNfeTransferencia(carga.Codigo.val());
            VisualizacaoEtapa();
        });


    });

}

function CriarGridAgrupamentos(knoutGrid, idGrid, tipo) {

    let editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: (e) => AtualizarPlacaMotorista(e, tipo)
    };

    var editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.string,
        numberMask: null
    };

    let detalhes = { descricao: "Detalhes", id: guid(), metodo: (e) => AbriModalAgrupamento(e, tipo), icone: "" };
    let recalcularfrete = { descricao: "Recalcular Frete", id: guid(), metodo: (e) => RecalcularFreteAgrupamento(e), icone: "", visibilidade: validarVisibilidadeRecalcularFrete };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoMotorista", visible: false },
        { data: "CodigoPlaca", visible: false },
        { data: "CodigoReboque", visible: false },
        { data: "CodigoSegundoReboque", visible: false },
        { data: "FalhaCalculoFrete", visible: false },
        { data: "CargaDT", visible: false },
        { data: "NumeroStage", title: "Numeros Stages", width: "15%", className: "text-align-center" },
        { data: "Fornecedor", title: "Fornecedor", width: "20%", className: "text-align-right" },
        { data: "Endereco", title: "Destino", width: "20%", className: "text-align-center" },
        { data: "Placa", title: "Placa", width: "15%", className: "text-align-right", editableCell: editable },
        { data: "Reboque", title: "Reboque", width: "15%", className: "text-align-right", editableCell: editable },
        { data: "SegundoReboque", title: "2° Reboque", width: "15%", className: "text-align-right", editableCell: editable },
        { data: "Motorista", title: "Motorista", width: "15%", className: "text-align-right", editableCell: editable },
        { data: "Frete", title: "Frete", width: "10%", className: "text-align-right", editableCell: editable },
    ];

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhes, recalcularfrete] };
    return new BasicDataTable(idGrid, header, menuOpcoes, null, null, null, null, null, editarColuna);
}

function loadGrisAgrupamentos() {
    _gridAgrupamentoStagesColeta = CriarGridAgrupamentos(_gridAgrupamentoStagesColeta, _agrupamentoStages.GridAgrupamentoStages.idGrid, "Coleta");
    _gridAgrupamentoStagesEntrega = CriarGridAgrupamentos(_gridAgrupamentoStagesEntrega, _agrupamentoStagesEntrega.GridAgrupamentoStages.idGrid, "Entrega");
    _gridAgrupamentoStagesTransferencia = CriarGridAgrupamentos(_gridAgrupamentoStagesTransferencia, _agrupamentoStagesTransferencia.GridAgrupamentoStages.idGrid, "Transferencia");

    _gridAgrupamentoStagesColeta.CarregarGrid([]);
    _gridAgrupamentoStagesEntrega.CarregarGrid([]);
    _gridAgrupamentoStagesTransferencia.CarregarGrid([]);
}


function loadGridCteNfsPreChekin() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "SituacaoCheckin", visible: false },
        { data: "NumeroCTe", title: "Número CT-e", width: "10%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "10%", className: "text-align-center" },
        { data: "Peso", title: "Peso", width: "10%", className: "text-align-center" },
        { data: "SituacaoCheckinDescricao", title: "Situação", width: "10%", className: "text-align-center" }
    ];

    const confirmar = { descricao: "Confirmar", id: guid(), metodo: confirmarCteNfsPreChekin, icone: "", visibilidade: isPermitirConfirmarPreChekin };
    const recusar = { descricao: "Recusar Formação", id: guid(), metodo: recusarCteNfsPreChekin, icone: "", visibilidade: isPermitirRecusarPreChekin };
    const auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CargaCTe"), icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [confirmar, recusar, auditar] };

    _gridAgrupamentoCteNfs = new BasicDataTable(_agrupamentoCteNfs.GridAgrupamentoCteNfs.idGrid, header, menuOpcoes);
    _gridAgrupamentoCteNfs.CarregarGrid([]);

}


//#endregion

//#region Funções Etapas
function EtapaColeta(e, sender) {
    _etapaAtualPreCheckin = 1;
    VerificarBotoesEtapasPreCheckin();
}

function EtapaTranferenciaClick(e, sender) {
    _etapaAtualPreCheckin = 2;
    VerificarBotoesEtapasPreCheckin();
}

function EtapaEntregaClick(e, sender) {
    _etapaAtualPreCheckin = 3;
    VerificarBotoesEtapasPreCheckin();
}
//#endregion

//#region Funções Auxiliares
function LimparOcultarAbasPreCheckin() {

    $("#" + _etapasPreCheckin.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasPreCheckin.Etapa1.idTab + " .step").attr("class", "step");
}

function VisualizacaoEtapa() {
    if (!_etapasPreCheckin)
        return;

    LimparOcultarAbasPreCheckin();

    $("#" + _etapasPreCheckin.Etapa1.idTab + " .step").attr("class", "step green");
    $("#" + _etapasPreCheckin.Etapa3.idTab + " .step").attr("class", "step green");
    $("#" + _etapasPreCheckin.Etapa3.idTab).attr("disabled", false);

    $("#" + _etapasPreCheckin.Etapa1.idTab + " .step").attr("class", "step green");
    $("#" + _etapasPreCheckin.Etapa2.idTab + " .step").attr("class", "step green");
    $("#" + _etapasPreCheckin.Etapa2.idTab).attr("disabled", false);

}

function validarVisibilidadeRecalcularFrete(registroSelecionado) {
    return (
        (registroSelecionado.FalhaCalculoFrete)
    );
}

function VerificarBotoesEtapasPreCheckin() {
    let etapaAtual = 1;

    if (etapaAtual == 1) {
        HabilitarTodosBotoesCargaPreCheckin(false);
        //Comportamento quando estiver na etapa
    }
    if (etapaAtual == 2) {
        HabilitarTodosBotoesCargaPreCheckin(false);
        //Comportamento quando estiver na etapa
    }
    if (etapaAtual == 3) {
        HabilitarTodosBotoesCargaPreCheckin(false);
        //Comportamento quando estiver na etapa
    }
}

function HabilitarTodosBotoesCargaPreCheckin(v) {
    if (_FormularioSomenteLeitura)
        v = false;
    //Aqui bloquear os campo para não poder ser editados depois 
}

function LimparEtapasPreChekin() {
    LimparOcultarAbasPreCheckin();
    VerificarBotoesEtapasPreCheckin();

    $("#" + _etapasPreCheckin.Etapa1.idTab).click();
    $("#" + _etapasPreCheckin.Etapa1.idTab).tab("show");
}

function LimparModal() {
    LimparCampos(_divModalAgrupamento);
}

function AbriModalAgrupamento(e, tipo) {
    _divModalAgrupamento.Codigo.val(e.Codigo);
    PreecherModalAgrupamento(e, tipo);
    Global.abrirModal("divModalDetalhesAgrupamento");
}

function RecalcularFreteAgrupamento(e) {
    RecalcularFrete(e);
}

function VeficaSePodeLiberarEtapaTransferencia() {
    let existeRegistrosIntegrados = _cargaPreChekinIntegracao.TotalIntegrado.val();
    return existeRegistrosIntegrados > 0;
}
//#endregion

//#region Requisições
function ObterQuantidadeDeStages(carga) {
    let p = new promise.Promise();
    executarReST("Pedido/ObterQuantidadeDeStages", { Codigo: carga.Codigo.val() }, (arg) => {

        if (!arg.Success) {
            p.done();
            return exibirMensagem(tipoMensagem.falha, arg.Msg);
        }
        if ((arg.Data.Quantidade > 1 || arg.Data.TipoPreChekin) && !arg.Data.CargaFilha)
            _visibilidadeTab = true;
        else
            _visibilidadeTab = false;

        if (arg.Data.Quantidade > 1 && arg.Data.TipoPreChekin) {
            carga.Veiculo.required = false;
            carga.Motorista.required = false;
            carga.Veiculo.visible(false);
            carga.AdicionarMotoristas.visible(false);
        }

        if (arg.Data.InformarLacreNosDadosTransporte)
            _visibilidadeTabLacre = true;

        p.done();
    })
    return p;
}

function RecalcularFrete(registro) {
    console.log(registro);
    exibirConfirmacao("Confirmação", "Você realmente deseja recalcular frete desta etapa?", function () {
        executarReST("Pedido/RecalcularFreteAgrupamentoStage", { Codigo: registro.Codigo, Carga: registro.CargaDT }, (arg) => {
            if (!arg.Success) {
                return exibirMensagem(tipoMensagem.falha, arg.Msg);
            }

            return exibirMensagem(tipoMensagem.ok, "Sucesso", "Recalculo do frete da etapa finalizado com sucesso.");
            PequisarStagesAgrupamento(registro.CargaDT);
        });
    });
}

function PequisarStagesAgrupamento(codigoCarga) {
    executarReST("Pedido/ObterListaStagesAgrupadas", { Codigo: codigoCarga }, (arg) => {
        if (!arg.Success)
            return;

        _gridAgrupamentoStagesColeta.CarregarGrid(arg.Data.StagesColetas);
        _gridAgrupamentoStagesTransferencia.CarregarGrid(arg.Data.StagesTransferencia);
        _gridAgrupamentoStagesEntrega.CarregarGrid(arg.Data.StagesEntregas);
    });
}

function GerarTransportes() {
    exibirConfirmacao("Confirmação", "Você realmente deseja gerar os transportes de todas as etapas?", function () {
        executarReST("CargaPrechekinIntegracao/GerarTransportes", { Carga: _cargaAtual.Codigo.val(), PermitirIntegrarConFalha: true }, (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

            return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        });
    });
}


function ConfirmarPlacas(liberarComFalha, event) {
    executarReST("CargaPrechekinIntegracao/ConfirmarPlacasPreChekin", { Carga: event.CodigoCarga.val(), PermitirIntegrarConFalha: liberarComFalha }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}

function RemoverPlacas(event) {

    executarReST("CargaPrechekinIntegracao/RemoverPlacasPreChekin", { Carga: event.CodigoCarga.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}

function ValidarPlaca(placa) {
    executarReST("Veiculo/BuscarPorPlaca", { Placa: placa }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);
    });
}

function SalvarVeiculoPlaca() {
    executarReST("Pedido/SalvarPlacaEMotoristaStageAgrupada", {
        CodigoAgrupamento: _divModalAgrupamento.Codigo.val(),
        CodigoPlaca: _divModalAgrupamento.Placa.codEntity(),
        CodigoMotorista: _divModalAgrupamento.Motorista.codEntity(),
        CodigoReboque: _divModalAgrupamento.Reboque.codEntity(),
        CodigoSegundoReboque: _divModalAgrupamento.SegundoReboque.codEntity(),
    }, (arg) => {

        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);
            return;
        }

        let grid;
        switch (_divModalAgrupamento.TipoAgrupamento.val()) {
            case "Entrega":
                grid = _gridAgrupamentoStagesEntrega;
                break;
            case "Coleta":
                grid = _gridAgrupamentoStagesColeta;
                break;
            case "Transferencia":
                grid = _gridAgrupamentoStagesTransferencia;
                break;
        }

        let lista = grid.BuscarRegistros();

        $.each(lista, (index, item) => {
            if (item.Codigo == _divModalAgrupamento.Codigo.val()) {

                item.Placa = _divModalAgrupamento.Placa.val();
                item.CodigoPlaca = _divModalAgrupamento.Placa.codEntity();
                item.CodigoReboque = _divModalAgrupamento.Reboque.codEntity();
                item.Reboque = _divModalAgrupamento.Reboque.val();
                item.CodigoSegundoReboque = _divModalAgrupamento.SegundoReboque.codEntity();
                item.SegundoReboque = _divModalAgrupamento.SegundoReboque.val();
                item.Motorista = _divModalAgrupamento.Motorista.val();
                item.CodigoMotorista = _divModalAgrupamento.Motorista.codEntity();

                return false;
            }
        });

        grid.CarregarGrid(lista);
        LimparModal();

        Global.fecharModal("divModalDetalhesAgrupamento");
    })
}

function AtualizarPlacaMotorista(e, tipo) {
    executarReST("Pedido/SalvarPlacaEMotoristaStageAgrupada", {
        CodigoAgrupamento: e.Codigo,
        Placa: e.Placa,
        Reboque: e.Reboque,
        CpfMotorista: e.Motorista,
        SegundoReboque: e.SegundoReboque,
    }, (arg) => {

        let grid;

        switch (tipo) {
            case "Entrega":
                grid = _gridAgrupamentoStagesEntrega;
                break;
            case "Coleta":
                grid = _gridAgrupamentoStagesColeta;
                break;
            case "Transferencia":
                grid = _gridAgrupamentoStagesTransferencia;
                break;
        }

        let lista = grid.BuscarRegistros();

        if (!arg.Success) {

            //caso não tenha encontrado alguma entidade, limpa a célula da grid
            $.each(lista, (index, item) => {

                if (item.Codigo == e.Codigo) {
                    for (let propName in item) {

                        let prop = item[propName];

                        if (propName.includes('Codigo') && prop <= 0)
                            item[propName.replace('Codigo','')] = '';
                    }
                }
            });

            grid.CarregarGrid(lista);

            exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);
            return;
        }

        $.each(lista, (index, item) => {
            if (item.Codigo == e.Codigo) {
                let data = arg.Data;

                item.Placa = data.Placa;
                item.CodigoPlaca = data.CodigoPlaca;
                item.Motorista = data.Motorista;
                item.CodigoMotorista = data.CodigoMotorista;
                item.CodigoReboque = data.CodigoReboque;
                item.Reboque = data.Reboque;
                item.CodigoSegundoReboque = data.CodigoSegundoReboque;
                item.SegundoReboque = data.SegundoReboque;

                return false;
            }
        });

        grid.CarregarGrid(lista);

        LimparModal();
    })
}

function ObterCteNfeTransferencia(codigoCarga) {
    executarReST("CargaPreChekin/ObterCteNfeTransferencia", { CodigoCarga: codigoCarga }, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        _gridAgrupamentoCteNfs.CarregarGrid(retorno.Data);
    });
}

function confirmarCteNfsPreChekin(registroSelecionado, linhaSelecionada) {
    exibirConfirmacao("Confirmação", "Você realmente deseja confirmar a formação?", function () {
        executarReST("CargaPreChekin/ConfirmarPreCheckin", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Checkin confirmado com sucesso.");
                    _gridAgrupamentoCteNfs.AtualizarDataRow(linhaSelecionada, retorno.Data);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function recusarCteNfsPreChekin(registroSelecionado, linhaSelecionada) {
    exibirConfirmacao("Confirmação", "Você realmente deseja recusar a formação?", function () {
        executarReST("CargaPreChekin/RecusarPreCheckin", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Recusa de checkin solicitada com sucesso.");
                    _gridAgrupamentoCteNfs.AtualizarDataRow(linhaSelecionada, retorno.Data);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function isPermitirConfirmarPreChekin(registroSelecionado) {
    return (
        (registroSelecionado.SituacaoCheckin == EnumSituacaoCheckin.SemConfirmacao) ||
        (registroSelecionado.SituacaoCheckin == EnumSituacaoCheckin.SemRegraAprovacao) ||
        (registroSelecionado.SituacaoCheckin == EnumSituacaoCheckin.RecusaReprovada)
    );
}

function isPermitirRecusarPreChekin(registroSelecionado) {
    return (
        (registroSelecionado.SituacaoCheckin == EnumSituacaoCheckin.SemConfirmacao) ||
        (registroSelecionado.SituacaoCheckin == EnumSituacaoCheckin.RecusaReprovada)
    );
}

function PreecherModalAgrupamento(e, tipo) {
    _divModalAgrupamento.Reboque.val(e.Reboque);
    _divModalAgrupamento.Reboque.codEntity(e.CodigoReboque);
    _divModalAgrupamento.Placa.val(e.Placa);
    _divModalAgrupamento.Placa.codEntity(e.CodigoPlaca);
    _divModalAgrupamento.Motorista.val(e.Motorista);
    _divModalAgrupamento.Motorista.codEntity(e.CodigoMotorista);
    _divModalAgrupamento.SegundoReboque.val(e.SegundoReboque);
    _divModalAgrupamento.SegundoReboque.codEntity(e.CodigoSegundoReboque);
    _divModalAgrupamento.TipoAgrupamento.val(tipo);
}
//#endregion