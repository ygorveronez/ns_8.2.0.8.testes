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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumSituacaoComissaoFuncionario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="ComissaoFuncionario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _comissaoFuncionarioMotoristaDocumento;
var _gridDocumentos;
var _rowGridMotorista;
var _dataGridMotorista;

var ComissaoFuncionarioMotoristaDocumento = function () {
    this.ComissaoFuncionarioMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscarCargas = PropertyEntity({ type: types.map, required: false, text: "Adicionar cargas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true)  });
    this.BuscarOcorrencias = PropertyEntity({ type: types.map, required: false, text: "Adicionar ocorrências", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadComissaoFuncionarioMotoristaDocumento() {
    _comissaoFuncionarioMotoristaDocumento = new ComissaoFuncionarioMotoristaDocumento();
    KoBindings(_comissaoFuncionarioMotoristaDocumento, "knockoutComissaoFuncionarioMotoristaDocumento");
    buscarComissaoFuncionariosMotoristasDocumentos();

    _gridDocumentos.SetPermitirEdicaoColunas(true);
    

    if (_CONFIGURACAO_TMS.UtilizarComissaoPorCargo) {
        _comissaoFuncionarioMotoristaDocumento.BuscarOcorrencias.visible(false);
        _comissaoFuncionarioMotoristaDocumento.BuscarCargas.visible(false);
    }

    new BuscarOcorrenciaComissaoFuncionario(_comissaoFuncionarioMotoristaDocumento.BuscarOcorrencias, RetornoInserirOcorrenciaComissaoFuncionario, null, _comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista);
    new BuscarCargaComissaoFuncionario(_comissaoFuncionarioMotoristaDocumento.BuscarCargas, RetornoInserirCargaComissaoFuncionario, null, _comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista, _comissaoFuncionarioMotoristaDocumento.Motorista);
}

function RetornoInserirOcorrenciaComissaoFuncionario(data) {
    var data = { CodigoOcorrencia: data.Codigo, Codigo: _comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.val() };
    executarReST("ComissaoFuncionarioMotoristaDocumento/AdicionarDocumentoComissaoMotorista", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridDocumentos.CarregarGrid();
                _gridMotoristas.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento adicionado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RetornoInserirCargaComissaoFuncionario(data) {
    var data = { CodigoCarga: data.Codigo, Codigo: _comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.val() };
    executarReST("ComissaoFuncionarioMotoristaDocumento/AdicionarDocumentoComissaoMotorista", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridDocumentos.CarregarGrid();
                _gridMotoristas.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento adicionado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarComissaoFuncionariosMotoristasDocumentos() {
    var excluir = { descricao: "Excluir", id: "clasExcluir", evento: "onclick", metodo: excluircomissaoFuncionarioMotoristaDocumento, tamanho: "5", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var editarColuna = { permite: true, callback: callbackEditarMotoristasDocumentosColuna, atualizarRow: false };
    _gridDocumentos = new GridView(_comissaoFuncionarioMotoristaDocumento.Pesquisar.idGrid, "ComissaoFuncionarioMotoristaDocumento/Pesquisa", _comissaoFuncionarioMotoristaDocumento, menuOpcoes, null, null, null, null, null, null, null, editarColuna);
}

function excluircomissaoFuncionarioMotoristaDocumento(data) {
    var data = { Codigo: data.Codigo };
    executarReST("ComissaoFuncionarioMotoristaDocumento/RemoverComissaoMotoristaDocumento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridDocumentos.CarregarGrid();
                _gridMotoristas.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function callbackEditarMotoristasDocumentosColuna(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, ValoFreteLiquido: dataRow.ValoFreteLiquido, PercentualExecucao: dataRow.PercentualExecucao };
    executarReST("ComissaoFuncionarioMotoristaDocumento/AlterarDadosComissaoMotoristaDocumento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.dyncomissaoFuncionarioMotoristaDocumento)
                _gridDocumentos.AtualizarDataRow(row, dataRow, callbackTabPress);

                CompararEAtualizarGridEditableDataRow(_dataGridMotorista, arg.Data.dynComissaoFuncionarioMotorista)
                _gridMotoristas.AtualizarDataRow(_rowGridMotorista, _dataGridMotorista);
            } else {
                _gridDocumentos.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            _gridDocumentos.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function carregarComissoesMotoristasDocumentos(dataRow, row) {
    _rowGridMotorista = row;
    _dataGridMotorista = dataRow;
    _comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.val(dataRow.Codigo);
    _comissaoFuncionarioMotoristaDocumento.Motorista.val(dataRow.CodigoMotorista);
    _gridDocumentos.SetPermitirEdicaoColunas(true);
    _gridDocumentos.SetSalvarPreferenciasGrid(true);
    _gridDocumentos.CarregarGrid(function () {
        Global.abrirModal('knockoutComissaoFuncionarioMotoristaDocumento');
    });
}

function desativarEditarGridDocumentos() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridDocumentos.SetarEditarColunas(editarColuna);
}

function habilitarEditarGridDocumentos() {
    var editarColuna = { permite: true, callback: callbackEditarMotoristasDocumentosColuna, atualizarRow: false };
    _gridDocumentos.SetarEditarColunas(editarColuna);
}