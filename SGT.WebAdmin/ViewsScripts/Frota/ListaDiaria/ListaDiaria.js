var filtroSituacoesRota = {
    Todos: -1,
    Roteirizado: 1,
    NaoRoteirizado: 2,
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: filtroSituacoesRota.Todos },
            { text: "Roteirizado", value: filtroSituacoesRota.Roteirizado },
            { text: "Não Roteirizado", value: filtroSituacoesRota.NaoRoteirizado }
        ];
    }
};

var filtroStatus = {
    Todos: -1,
    Disponivel: 1,
    Indisponivel: 2,
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: filtroStatus.Todos },
            { text: "Disponível", value: filtroStatus.Disponivel },
            { text: "Indisponível", value: filtroStatus.Indisponivel }
        ];
    }
};

var ListaDiariaPesquisa = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filiais:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportadores:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.ModeloVeicular = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelos Veiculares:", idBtnSearch: guid(), visible: ko.observable(true) });
    //this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(filtroStatus.Todos), options: filtroStatus.obterOpcoesPesquisa(), def: filtroStatus.Todos, text: "Status: " });
    this.SituacaoRota = PropertyEntity({ val: ko.observable(filtroSituacoesRota.Todos), options: filtroSituacoesRota.obterOpcoesPesquisa(), def: filtroSituacoesRota.Todos, text: "Situação: " });
    this.DataInicial = PropertyEntity({ text: "Data de:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data até:", getType: typesKnockout.date });
    this.Rodizio = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Rodízio" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridListaDiaria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Pesquisar Listas", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var FiltrosListaDiariaPesquisa = function () {
    this.CodigoPlanejamento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoFilial = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0) });
    this.Data = PropertyEntity({ val: ko.observable(''), type: types.map, codEntity: ko.observable(0), required: false });
    this.Roteirizado = PropertyEntity({ val: ko.observable(filtroSituacoesRota.Todos), def: filtroSituacoesRota.Todos });

    this.AdicionarVeiculo = PropertyEntity({
        type: types.multiplesEntities,
        text: "Adicionar",
        codEntity: ko.observable(0),
        idBtnSearch: guid(),
        enable: ko.observable(true),
        visible: ko.observable(false),
        idGrid: guid()
    });
}

var ModalJustificativa = function () {
    this.ListaDiariaVeiculoSelecionado = PropertyEntity({ val: ko.observable(0), type: types.map, codEntity: ko.observable(0), required: false, visible: false });
    this.JustificativaDeIndisponibilidadeDeFrota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa de Indisponibilidade de Frota:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: salvarJustificativaClick, type: types.event, text: "Confirmar", idGrid: guid() });
}

var ModalReplicarVeiculo = function () {
    this.CodigoPlanejamentoFrotaVeiculoDia = PropertyEntity({ val: ko.observable(0), type: types.map, required: false });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Confirmar = PropertyEntity({
        eventClick: function () {
            duplicarRegistro(this.CodigoPlanejamentoFrotaVeiculoDia.val(), _modalReplicarVeiculo.DataInicial.val(), _modalReplicarVeiculo.DataFinal.val());
        }, type: types.event, text: "Confirmar", idGrid: guid()
    });
}

var ModalExcluirVeiculo = function () {
    this.CodigoPlanejamentoFrotaVeiculoDia = PropertyEntity({ val: ko.observable(0), type: types.map, required: false });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Confirmar = PropertyEntity({
        eventClick: function () {
            excluirRegistros(this.CodigoPlanejamentoFrotaVeiculoDia.val(), _modalExcluirVeiculo.DataInicial.val(), _modalExcluirVeiculo.DataFinal.val());
        }, type: types.event, text: "Excluir", idGrid: guid()
    });
}

var ModalEditar = function () {
    this.CodigoPlanejamentoFrotaVeiculoDia = PropertyEntity({ val: ko.observable(0), type: types.map, required: false });
    this.ObservacaoMarfrig = PropertyEntity({ text: 'Observação:', val: ko.observable(''), maxlength: 500, type: types.map, required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.ObservacaoTransportador = PropertyEntity({ enable: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador), text: ko.observable('Observação Transportador:'), val: ko.observable(''), maxlength: 500, type: types.map, required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.RotaDeConhecimento = PropertyEntity({ text: 'Rota de Conhecimento:', val: ko.observable(''), maxlength: 500, type: types.map, required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Confirmar = PropertyEntity({
        eventClick: function () {
            salvarModalEditar();
        }, type: types.event, text: "Confirmar", idGrid: guid()
    });
}

var _pesquisaListaDiaria;
var _filtrosListaDiariaPesquisa;
var _modalJustificativa;
var _modalEditar;

var _gridListaDiaria;
var _gridListaDiariaVeiculos;
var _modalExcluirVeiculo;

function RetornoBuscaVeiculos(veiculos) {
    let arrayVeiculos = [];
    console.log(veiculos);
    for (let i = 0; i < veiculos.length; i++) {
        arrayVeiculos.push(veiculos[i].Codigo);
    }
    executarReST("ListaDiaria/AdicionarVeiculo", {
        CodigoVeiculos: JSON.stringify(arrayVeiculos),
        Data: _filtrosListaDiariaPesquisa.Data.val(),
        CodigoPlanejamento: _filtrosListaDiariaPesquisa.CodigoPlanejamento.val()
    }, function (retorno) {

        if (!retorno.Success) {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            return;
        }
        if (!retorno.Data) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            return;
        }
        let arrPlacasCadastradas = retorno.Data.Sucesso;
        let arrMsgTesteFrio = retorno.Data.MsgTesteFrio;
        let arrPlacasJaCadastradasNaData = retorno.Data.JaExiste;
        let arrMsgModeloVeicular = retorno.Data.MsgModeloVeicular;

        if (arrPlacasCadastradas.length > 0)
            exibirMensagem(tipoMensagem.ok, "Sucesso", 'Placas ' + arrPlacasCadastradas.join() + ' cadastradas com sucesso.', 12000);

        if(arrPlacasJaCadastradasNaData.length > 0)
            exibirMensagem(tipoMensagem.atencao, "Atenção", 'Não foi possível adicionar, o veículo já está configurado nessa data em outra filial.', 12000);
        
        for (let i = 0; i < arrMsgTesteFrio.length; i++)
            exibirMensagem(tipoMensagem.atencao, "Atenção", arrMsgTesteFrio[i], 12000);
        
        for (let i = 0; i < arrMsgModeloVeicular.length; i++)
            exibirMensagem(tipoMensagem.atencao, "Atenção", arrMsgModeloVeicular[i], 12000);
        
        _gridListaDiariaVeiculos.CarregarGrid();        
    });
}

function LoadListaDiaria() {
    _pesquisaListaDiaria = new ListaDiariaPesquisa();
    KoBindings(_pesquisaListaDiaria, "knockoutPesquisaListaDiaria");

    _filtrosListaDiariaPesquisa = new FiltrosListaDiariaPesquisa();
    KoBindings(_filtrosListaDiariaPesquisa, "knockoutPesquisaListaDiariaVeiculos");

    _modalJustificativa = new ModalJustificativa();
    KoBindings(_modalJustificativa, "knockoutModalJustificativa");

    _modalReplicarVeiculo = new ModalReplicarVeiculo();
    KoBindings(_modalReplicarVeiculo, "knockoutModalReplicarVeiculo");

    _modalExcluirVeiculo = new ModalExcluirVeiculo();
    KoBindings(_modalExcluirVeiculo, "knockoutModalExcluir");

    _modalEditar = new ModalEditar();
    KoBindings(_modalEditar, "knockoutModalEditar");

    HeaderAuditoria("PlanejamentoFrotaDia", _filtrosListaDiariaPesquisa, "CodigoPlanejamento");

    new BuscarJustificativaDeIndisponibilidadeDeFrota(_modalJustificativa.JustificativaDeIndisponibilidadeDeFrota);

    new BuscarVeiculos(_filtrosListaDiariaPesquisa.AdicionarVeiculo, RetornoBuscaVeiculos, null, _filtrosListaDiariaPesquisa.ModeloVeicular, null, null, null, null, null, null, null, null, null, true, null, null, null, true);

    new BuscarTransportadores(_pesquisaListaDiaria.Transportador);
    //new BuscarTiposOperacao(_pesquisaListaDiaria.TipoOperacao);
    new BuscarModelosVeicularesCarga(_pesquisaListaDiaria.ModeloVeicular);
    new BuscarFilial(_pesquisaListaDiaria.Filial);

    BuscarListaDiaria();
}

function BuscarListaDiaria() {
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar", id: guid(), evento: "onclick", metodo: function (row) {
                    _filtrosListaDiariaPesquisa.CodigoPlanejamento.val(row.CodigoPlanejamento);
                    _filtrosListaDiariaPesquisa.CodigoFilial.val(row.CodigoFilial);
                    _filtrosListaDiariaPesquisa.ModeloVeicular.val(row.ModeloVeicular);
                    _filtrosListaDiariaPesquisa.ModeloVeicular.codEntity(row.CodigoModeloVeicular);
                    _filtrosListaDiariaPesquisa.Transportador.multiplesEntities(_pesquisaListaDiaria.Transportador.multiplesEntities());
                    _filtrosListaDiariaPesquisa.Data.val(row.Data);
                    _filtrosListaDiariaPesquisa.Roteirizado.val(_pesquisaListaDiaria.SituacaoRota.val());
                    _filtrosListaDiariaPesquisa.AdicionarVeiculo.visible(true);

                    $('#tab-Veiculos').html('Veiculos do dia ' + row.Data);
                    $('#divVeiculos').slideDown();
                    _pesquisaListaDiaria.ExibirFiltros.visibleFade(false);
                    _gridListaDiariaVeiculos.CarregarGrid();
                }, tamanho: "20", icone: ""
            }
        ]
    };

    var menuListaDiariaVeiculos = {
        tipo: TypeOptionMenu.list,
        opcoes: [
            {
                descricao: "Editar", id: guid(), evento: "onclick", metodo: function (param1) {
                    let codPlanejamentoFrotaVeiculoDia = param1.Codigo;
                    _modalEditar.CodigoPlanejamentoFrotaVeiculoDia.val(codPlanejamentoFrotaVeiculoDia);
                    Global.abrirModal('divModalEditar');
                    $("#divModalEditar").one('hidden.bs.modal', function () {
                        LimparCampos(_modalEditar);
                    });
                }, tamanho: "20", icone: ""
            },
            {
                descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (param1) {
                    let codPlanejamentoFrotaVeiculoDia = param1.Codigo;
                    _modalReplicarVeiculo.DataInicial.val(_filtrosListaDiariaPesquisa.Data.val());
                    _modalReplicarVeiculo.DataFinal.val(_filtrosListaDiariaPesquisa.Data.val());
                    _modalReplicarVeiculo.CodigoPlanejamentoFrotaVeiculoDia.val(codPlanejamentoFrotaVeiculoDia);
                    $('#tituloModalReplicar').html('Replicar veículo ' + param1.Placa);
                    Global.abrirModal('divModalReplicar');
                    $("#divModalReplicar").one('hidden.bs.modal', function () {
                        LimparCampos(_modalReplicarVeiculo);
                        $('#tituloModalReplicar').html('Replicar veículo');
                    });

                }, tamanho: "20", icone: ""
            },
            {
                descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (param1) {
                    let codPlanejamentoFrotaVeiculoDia = param1.Codigo;
                    _modalExcluirVeiculo.DataInicial.val(_filtrosListaDiariaPesquisa.Data.val());
                    _modalExcluirVeiculo.DataFinal.val(_filtrosListaDiariaPesquisa.Data.val());
                    _modalExcluirVeiculo.CodigoPlanejamentoFrotaVeiculoDia.val(codPlanejamentoFrotaVeiculoDia);
                    $('#tituloModalExcluir').html('Excluir veículo ' + param1.Placa);
                    Global.abrirModal('divModalExcluir');
                    $("#divModalExcluir").one('hidden.bs.modal', function () {
                        LimparCampos(_modalExcluirVeiculo);
                        $('#tituloModalExcluir').html('Excluir veículo');
                    });

                }, tamanho: "20", icone: ""
            }
        ]
    };

    var configExportacaoPesquisaAgrupada = {
        url: "ListaDiaria/ExportarPesquisaAgrupada",
        titulo: "Lista Diária de Veículos"
    };

    _gridListaDiaria = new GridView(_pesquisaListaDiaria.Pesquisar.idGrid, "ListaDiaria/PesquisaAgrupada", _pesquisaListaDiaria, menuOpcoes, null, null, null, null, null, null, null, null, configExportacaoPesquisaAgrupada);
    _gridListaDiaria.CarregarGrid();

    let totalRegistrosPorPagina = 12;
    let limiteRegistros = 60;
    let multiplaEscolha = null;

    let infoEditar = {
        functionPermite: null,
        permite: true,
        callback: callbackEditarGridRow,
        atualizarRow: false
    };

    var configExportacaoEmbarcador = {
        url: "ListaDiaria/ExportarPesquisaEmbarcador",
        titulo: "Lista Diária de Veículos"
    };
    var configExportacaoTransportador = {
        url: "ListaDiaria/ExportarPesquisaTransportador",
        titulo: "Lista Diária de Veículos"
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        _gridListaDiariaVeiculos = new GridView(_filtrosListaDiariaPesquisa.AdicionarVeiculo.idGrid, "ListaDiaria/PesquisaEmbarcador", _filtrosListaDiariaPesquisa, menuListaDiariaVeiculos, null, totalRegistrosPorPagina, null, null, null, multiplaEscolha, limiteRegistros, infoEditar, configExportacaoEmbarcador);
    else
        _gridListaDiariaVeiculos = new GridView(_filtrosListaDiariaPesquisa.AdicionarVeiculo.idGrid, "ListaDiaria/Pesquisa", _filtrosListaDiariaPesquisa, menuListaDiariaVeiculos, null, totalRegistrosPorPagina, null, null, null, multiplaEscolha, limiteRegistros, infoEditar, configExportacaoTransportador);

    _gridListaDiariaVeiculos.SetPermitirEdicaoColunas(true);
    _gridListaDiariaVeiculos.SetSalvarPreferenciasGrid(true);
    _gridListaDiariaVeiculos.SetHabilitarScrollHorizontal(true, 140);

    //_gridMonitoramento.SetPermitirEdicaoColunas(true);
    //_gridMonitoramento.SetScrollHorizontal(true);
}

function duplicarRegistro(codPlanejamentoFrotaVeiculoDia, dataInicial, dataFinal) {
    executarReST("ListaDiaria/ReplicarRegistro", {
        CodigoPlanejamentoFrotaDiaVeiculo: codPlanejamentoFrotaVeiculoDia,
        DataInicial: dataInicial,
        DataFinal: dataFinal
    }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro replicado com sucesso.");
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirRegistros(codPlanejamentoFrotaVeiculoDia, dataInicial, dataFinal) {
    executarReST("ListaDiaria/ExcluirRegistros", {
        CodigoPlanejamentoFrotaDiaVeiculo: codPlanejamentoFrotaVeiculoDia,
        DataInicial: dataInicial,
        DataFinal: dataFinal
    }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarModalEditar() {
    executarReST("ListaDiaria/Salvar", {
        CodigoPlanejamentoFrotaDiaVeiculo: _modalEditar.CodigoPlanejamentoFrotaVeiculoDia.val(),
        ObservacaoMarfrig: _modalEditar.ObservacaoMarfrig.val(),
        ObservacaoTransportador: _modalEditar.ObservacaoTransportador.val(),
        RotaDeConhecimento: _modalEditar.RotaDeConhecimento.val()
    }, function (retorno) {
        Global.fecharModal("divModalEditar");
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro atualizado com sucesso.");
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarJustificativaClick(obj, event) {
    let param1 = _modalJustificativa.ListaDiariaVeiculoSelecionado.val();
    let param2 = obj.JustificativaDeIndisponibilidadeDeFrota.codEntity();
    if (!param1 || !param2) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione a justificativa.");
        return;
    }
    executarReST("ListaDiaria/DefinirJustificativa", {
        CodigoPlanejamentoFrotaDiaVeiculo: _modalJustificativa.ListaDiariaVeiculoSelecionado.val(),
        CodigoJustificativa: obj.JustificativaDeIndisponibilidadeDeFrota.codEntity()
    }, function (retorno) {
        Global.fecharModal("divModalJustificativa");
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Justificativa de Indisponibilidade alterada com sucesso");
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function callbackEditarGridRow(dataRow, row, header) {
    if (header.data != "Indisponivel" && header.data != "Roteirizado") {
        _gridListaDiariaVeiculos.DesfazerAlteracaoDataRow(row);
        return;
    }
    if (header.data == "Indisponivel") {
        MarcarOuDesmarcarIndisponivel(dataRow);
        return;
    }
    if (header.data == "Roteirizado") {
        MarcarOuDesmarcarRoteirizado(dataRow);
        return;
    }
}

function MarcarOuDesmarcarRoteirizado(dataRow) {
    executarReST("ListaDiaria/MarcarOuDesmarcarRoteirizado", {
        CodigoPlanejamentoFrotaDiaVeiculo: dataRow.Codigo,
        Roteirizado: dataRow.Roteirizado
    }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado roteirizado com sucesso.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function MarcarOuDesmarcarIndisponivel(dataRow) {
    if (dataRow.Indisponivel) {
        _modalJustificativa.ListaDiariaVeiculoSelecionado.val(dataRow.Codigo);

        Global.abrirModal('divModalJustificativa');

        $("#divModalJustificativa").one('hidden.bs.modal', function () {
            limparCamposJustificativa();
            _gridListaDiariaVeiculos.CarregarGrid(); //Nao remover, senão buga o check se marcar e fechar o modal e depois desmarcar
        });

        return;
    }
    executarReST("ListaDiaria/MarcarComoDisponivel", { CodigoPlanejamentoFrotaDiaVeiculo: dataRow.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                return;
            }
            _gridListaDiaria.CarregarGrid();
            _gridListaDiariaVeiculos.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterada a disponibilidade com sucesso.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparCamposJustificativa() {
    LimparCampos(_modalJustificativa);
    //_grid.CarregarGrid([]);
}

function DumpValues() {
    console.log(_gridListaDiariaVeiculos.BuscarRegistros())
}
