/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="FilaCarregamento.js" />

// #region Objetos Globais do Arquivo

var _filaCadastro;
var _gridFilaCadastroDestino;
var _gridFilaCadastroEstadoDestino;
var _gridFilaCadastroRegiaoDestino;
var _gridFilaCadastroTipoCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FilaCadastro = function () {
    const tipo = obterTipoPadrao();

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.UtilizarProgramacaoCarga = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.UtilizarProgramacaoCarga), def: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga, getType: typesKnockout.bool });
    this.DataProgramada = PropertyEntity({ text: "*Previsão de Chegada:", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga || (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? "*" : "") + "Motorista:", idBtnSearch: guid(), required: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.Tipo = PropertyEntity({ val: ko.observable(tipo), options: _listaTipoRetornoCarga, def: tipo, text: "*Tipo: ", required: true, visible: !(tipo) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true, eventChange: filaCadastroVeiculoBlur });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Equipamento:", idBtnSearch: guid(), required: false, eventChange: filaCadastroEquipamento });

    this.Destino = PropertyEntity({ type: types.event, text: "Adicionar Destinos", idBtnSearch: guid(), idGrid: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.event, text: "Adicionar Estados", idBtnSearch: guid(), idGrid: guid() });
    this.RegiaoDestino = PropertyEntity({ type: types.event, text: "Adicionar Regiões", idBtnSearch: guid(), idGrid: guid() });
    this.TipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipos de Carga", idBtnSearch: guid(), idGrid: guid() });

    this.AreaVeiculo = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.select, text: "*Área do CD", options: ko.observable([]), visible: _CONFIGURACAO_TMS.InformarAreaCDAdicionarVeiculo });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFilaCarregamentoAdicionar() {
    _filaCadastro = new FilaCadastro();
    KoBindings(_filaCadastro, "knockoutCadastroFila");

    BuscarAreasVeiculo();
    BuscarMotoristasMobile(_filaCadastro.Motorista, undefined, undefined, undefined, _filaCadastro.Transportador);
    BuscarVeiculos(_filaCadastro.Veiculo, retornoConsultaFilaCadastroVeiculo, (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe ? _filaCadastro.Transportador : null), null, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);
    BuscarEquipamentos(_filaCadastro.Equipamento);

    loadGridFilaCarregamentoAdicionarDestino();
    loadGridFilaCarregamentoAdicionarEstadoDestino();
    loadGridFilaCarregamentoAdicionarRegiaoDestino();
    loadGridFilaCarregamentoAdicionarTipoCarga();

    configurarLayoutPorSistema();
}

function BuscarAreasVeiculo() {
    if (!_CONFIGURACAO_TMS.InformarAreaCDAdicionarVeiculo)
        return;

    executarReST("AreaVeiculo/BuscarTodos", {}, function (response) {
        if (response.Success) {
            if (response.Data) {
                _filaCadastro.AreaVeiculo.options(response.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, response.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Msg);
        }
    });
}

function loadGridFilaCarregamentoAdicionarDestino() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirFilaCarregamentoAdicionarDestinoClick(_gridFilaCadastroDestino, registroSelecionado); } };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridFilaCadastroDestino = new BasicDataTable(_filaCadastro.Destino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarLocalidades(_filaCadastro.Destino, null, null, null, _gridFilaCadastroDestino, controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino);

    _gridFilaCadastroDestino.CarregarGrid([]);
}

function loadGridFilaCarregamentoAdicionarEstadoDestino() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirFilaCarregamentoAdicionarDestinoClick(_gridFilaCadastroEstadoDestino, registroSelecionado); } };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridFilaCadastroEstadoDestino = new BasicDataTable(_filaCadastro.EstadoDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarEstados(_filaCadastro.EstadoDestino, null, _gridFilaCadastroEstadoDestino, controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino);

    _gridFilaCadastroEstadoDestino.CarregarGrid([]);
}

function loadGridFilaCarregamentoAdicionarRegiaoDestino() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirFilaCarregamentoAdicionarDestinoClick(_gridFilaCadastroRegiaoDestino, registroSelecionado); } };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridFilaCadastroRegiaoDestino = new BasicDataTable(_filaCadastro.RegiaoDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarRegioes(_filaCadastro.RegiaoDestino, null, _gridFilaCadastroRegiaoDestino, controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino);

    _gridFilaCadastroRegiaoDestino.CarregarGrid([]);
}

function loadGridFilaCarregamentoAdicionarTipoCarga() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirFilaCarregamentoAdicionarTipoCargaClick }
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridFilaCadastroTipoCarga = new BasicDataTable(_filaCadastro.TipoCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarTiposdeCarga(_filaCadastro.TipoCarga, null, null, _gridFilaCadastroTipoCarga);

    _gridFilaCadastroTipoCarga.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarFilaClick() {
    if (!ValidarCamposObrigatorios(_filaCadastro))
        return exibirMensagemCamposObrigatorio();

    const codigosDestinos = [];
    const codigosEstadosDestino = [];
    const codigosRegioesDestino = [];
    const codigosTiposCarga = [];

    _gridFilaCadastroDestino.BuscarRegistros().slice().forEach(function (destino) {
        codigosDestinos.push(destino.Codigo);
    });

    _gridFilaCadastroEstadoDestino.BuscarRegistros().slice().forEach(function (estadoDestino) {
        codigosEstadosDestino.push(estadoDestino.Codigo);
    });

    _gridFilaCadastroRegiaoDestino.BuscarRegistros().slice().forEach(function (regiaoDestino) {
        codigosRegioesDestino.push(regiaoDestino.Codigo);
    });

    _gridFilaCadastroTipoCarga.BuscarRegistros().slice().forEach(function (tipoCarga) {
        codigosTiposCarga.push(tipoCarga.Codigo);
    });

    const filaCadastrar = {
        DataProgramada: _filaCadastro.DataProgramada.val(),
        Motorista: _filaCadastro.Motorista.codEntity(),
        Equipamento: _filaCadastro.Equipamento.codEntity(),
        Tipo: _filaCadastro.Tipo.val(),
        Veiculo: _filaCadastro.Veiculo.codEntity(),
        CentroCarregamento: _pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity(),
        Filial: _pesquisaFilaCarregamentoAuxiliar.Filial.codEntity(),
        Destinos: JSON.stringify(codigosDestinos),
        EstadosDestino: JSON.stringify(codigosEstadosDestino),
        RegioesDestino: JSON.stringify(codigosRegioesDestino),
        TiposCarga: JSON.stringify(codigosTiposCarga),
        CodigoAreaVeiculo: _filaCadastro.AreaVeiculo.val()
    };

    executarReST("FilaCarregamento/AdicionarFila", filaCadastrar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado registro na fila com sucesso");

                fecharFilaModal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function adicionarFilaModalClick() {
    if ((_usuarioPossuiPermissaoAdicionar || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.TransportadorTerceiro) && ((_pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity() > 0) || (_pesquisaFilaCarregamentoAuxiliar.Filial.codEntity() > 0))) {

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            _filaCadastro.Transportador.entityDescription(_transportadorLogado.Descricao);
            _filaCadastro.Transportador.val(_transportadorLogado.Descricao);
            _filaCadastro.Transportador.codEntity(_transportadorLogado.Codigo);
        }

        Global.abrirModal('divModalCadastroFila');
        $("#divModalCadastroFila").one('hidden.bs.modal', function () {
            limparCamposFilaCadastro();
        });
    }
}

function excluirFilaCarregamentoAdicionarDestinoClick(gridDestino, registroSelecionado) {
    const listaDestino = gridDestino.BuscarRegistros().slice();

    for (let i = 0; i < listaDestino.length; i++) {
        if (registroSelecionado.Codigo == listaDestino[i].Codigo) {
            listaDestino.splice(i, 1);
            break;
        }
    }

    gridDestino.CarregarGrid(listaDestino);
    controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino();
}

function excluirFilaCarregamentoAdicionarTipoCargaClick(registroSelecionado) {
    const listaTipoCarga = _gridFilaCadastroTipoCarga.BuscarRegistros().slice();

    for (let i = 0; i < listaTipoCarga.length; i++) {
        if (registroSelecionado.Codigo == listaTipoCarga[i].Codigo) {
            listaTipoCarga.splice(i, 1);
            break;
        }
    }

    _gridFilaCadastroTipoCarga.CarregarGrid(listaTipoCarga);
}

function filaCadastroVeiculoBlur() {
    if (_filaCadastro.Veiculo.val() == "")
        LimparCampo(_filaCadastro.Transportador);
}

function filaCadastroEquipamento() {
    if (_filaCadastro.Equipamento.val() == "")
        LimparCampo(_filaCadastro.Equipamento);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino() {
    if (_gridFilaCadastroDestino.BuscarRegistros().length > 0) {
        $("#liTabCadastroFilaCidadesDestino").show();
        $("#liTabCadastroFilaEstadosDestino").hide();
        $("#liTabCadastroFilaRegioesDestino").hide();

        $(".nav-tabs a[href='#tabCadastroFilaCidadesDestino']").tab('show');
    }
    else if (_gridFilaCadastroEstadoDestino.BuscarRegistros().length > 0) {
        $("#liTabCadastroFilaCidadesDestino").hide();
        $("#liTabCadastroFilaEstadosDestino").show();
        $("#liTabCadastroFilaRegioesDestino").hide();

        $(".nav-tabs a[href='#tabCadastroFilaEstadosDestino']").tab('show');
    }
    else if (_gridFilaCadastroRegiaoDestino.BuscarRegistros().length > 0) {
        $("#liTabCadastroFilaCidadesDestino").hide();
        $("#liTabCadastroFilaEstadosDestino").hide();
        $("#liTabCadastroFilaRegioesDestino").show();

        $(".nav-tabs a[href='#tabCadastroFilaRegioesDestino']").tab('show');
    }
    else {
        $("#liTabCadastroFilaCidadesDestino").show();
        $("#liTabCadastroFilaEstadosDestino").show();
        $("#liTabCadastroFilaRegioesDestino").show();
    }
}

function fecharFilaModal() {
    Global.fecharModal("divModalCadastroFila");
}

function limparCamposFilaCadastro() {
    LimparCampos(_filaCadastro);

    _gridFilaCadastroDestino.CarregarGrid([]);
    _gridFilaCadastroEstadoDestino.CarregarGrid([]);
    _gridFilaCadastroRegiaoDestino.CarregarGrid([]);
    _gridFilaCadastroTipoCarga.CarregarGrid([]);

    controlarVisibilidadeAbasFilaCarregamentoAdicionarDestino();

    $(".nav-tabs a[href='#tabCadastroFilaCidadesDestino']").tab('show');
}

function retornoConsultaFilaCadastroVeiculo(registroSelecionado) {
    _filaCadastro.Veiculo.entityDescription(registroSelecionado.Descricao);
    console.log(registroSelecionado);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.TransportadorTerceiro)
        _filaCadastro.Veiculo.val(`${registroSelecionado.Descricao}, ${registroSelecionado.VeiculosVinculados} (${registroSelecionado.TipoFrotaDescricao})`);
    else
        _filaCadastro.Veiculo.val(`${registroSelecionado.Descricao}, ${registroSelecionado.VeiculosVinculados}`);

    _filaCadastro.Veiculo.codEntity(registroSelecionado.Codigo);

    _filaCadastro.Motorista.entityDescription(registroSelecionado.Motorista);
    _filaCadastro.Motorista.val(registroSelecionado.Motorista);
    _filaCadastro.Motorista.codEntity(registroSelecionado.CodigoMotorista);

    if (_CONFIGURACAO_TMS.UtilizarProgramacaoCarga && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)) {
        _filaCadastro.Transportador.entityDescription(registroSelecionado.Empresa);
        _filaCadastro.Transportador.val(registroSelecionado.Empresa);
        _filaCadastro.Transportador.codEntity(registroSelecionado.CodigoEmpresa);
    }
}

function configurarLayoutPorSistema() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        $("#liTabCadastroFilaCidadesDestino").hide();

}

// #endregion Funções Privadas