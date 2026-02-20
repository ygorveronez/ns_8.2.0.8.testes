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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumSituacaoComissaoFuncionario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Cargo.js" />
/// <reference path="ComissaoFuncionario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _comissaoFuncionarioMotorista;
var _gridMotoristas;
var _alterarMedia;

var ComissaoFuncionarioMotorista = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), eventChange: motoristaBlur, visible: ko.observable(false), visibleGeracao: ko.observable(false), visibleFalha: ko.observable(false) });
    this.CargoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cargo:", idBtnSearch: guid(), eventChange: cargoMotoristaBlur, visible: ko.observable(false), visibleGeracao: ko.observable(false), visibleFalha: ko.observable(false) });
    this.MotoristaComDoisModelos = PropertyEntity({ getType: typesKnockout.bool, text: "Visualizar motoristas com mais que um modelo?", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.ComissaoFuncionario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MensagemFalhaGeracao = PropertyEntity({});
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotoristas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.MotoristaComDoisModelos.val.subscribe(function (novoValor) {
        carregarComissoesMotoristas();
    });
};

var AlterarMedia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Media = PropertyEntity({ getType: typesKnockout.decimal, text: "*Media: ", required: true });
    this.AlterarMedia = PropertyEntity({ type: types.event, eventClick: alterarMediaComissaoClick, enable: ko.observable(true), text: "Confirmar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadComissaoFuncionarioMotorista() {
    _comissaoFuncionarioMotorista = new ComissaoFuncionarioMotorista();
    KoBindings(_comissaoFuncionarioMotorista, "knockoutComissaoFuncionarioMotorista");
    buscarComissaoFuncionariosMotoristas();
    new BuscarMotoristas(_comissaoFuncionarioMotorista.Motorista, retornoMotorista);
    new BuscarCargos(_comissaoFuncionarioMotorista.CargoMotorista, retornoCargoMotorista);

    if (!_CONFIGURACAO_TMS.UtilizarComissaoPorCargo) {
        _comissaoFuncionarioMotorista.CargoMotorista.visibleGeracao(false);
        _comissaoFuncionarioMotorista.CargoMotorista.visible(false);
        _comissaoFuncionarioMotorista.CargoMotorista.visibleFalha(false);
        _comissaoFuncionarioMotorista.MotoristaComDoisModelos.visible(false);
    }
}

function motoristaBlur() {
    if (_comissaoFuncionarioMotorista.Motorista.val() == "") {
        _comissaoFuncionarioMotorista.Motorista.codEntity(0);
    }
    carregarComissoesMotoristas();
}

function retornoMotorista(data) {
    _comissaoFuncionarioMotorista.Motorista.val(data.Nome);
    _comissaoFuncionarioMotorista.Motorista.codEntity(data.Codigo);
    carregarComissoesMotoristas();
}

function cargoMotoristaBlur() {
    if (_comissaoFuncionarioMotorista.CargoMotorista.val() == "") {
        _comissaoFuncionarioMotorista.CargoMotorista.codEntity(0);
    }
    carregarComissoesMotoristas();
}

function retornoCargoMotorista(data) {
    _comissaoFuncionarioMotorista.CargoMotorista.val(data.Descricao);
    _comissaoFuncionarioMotorista.CargoMotorista.codEntity(data.Codigo);
    carregarComissoesMotoristas();
}

function buscarComissaoFuncionariosMotoristas() {
    var detalhes = { descricao: "Detalhes", id: guid(), metodo: detalhesComissaoFuncionarioClick, icone: "" };
    var NaoGerarComissao = { descricao: "Não Gerar Comissão", id: guid(), metodo: naoGerarComissaoClick, icone: "", visibilidade: visibilidadeNaoGerarComissao };
    var GerarComissao = { descricao: "Gerar Comissão", id: guid(), metodo: gerarComissaoClick, icone: "", visibilidade: visibilidadeGerarComissao };
    var ImprimirComissao = { descricao: "Imprimir Comissão", id: guid(), metodo: imprimirComissaoClick, icone: "", visibilidade: visibilidadeImpressao };
    var AlterarMedia = { descricao: "Alterar Média", id: guid(), metodo: alterarMediaClick, icone: "", visibilidade: visibilidadeAlterarMedia };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhes, NaoGerarComissao, GerarComissao, ImprimirComissao, AlterarMedia] };

    var configExportacao = {
        url: "ComissaoFuncionarioMotorista/ExportarPesquisa",
        titulo: "Comissão - Motoristas"
    };

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridMotoristas = new GridViewExportacao(_comissaoFuncionarioMotorista.Pesquisar.idGrid, "ComissaoFuncionarioMotorista/Pesquisa", _comissaoFuncionarioMotorista, menuOpcoes, configExportacao, null, 10, null, null, editarColuna);
}

function imprimirComissaoClick(data) {
    var data = { Codigo: data.Codigo };
    executarDownload("ComissaoFuncionarioMotorista/BaixarRelatorioMotorista", data);
}

function naoGerarComissaoClick(dataRow, row) {
    var data = { Codigo: dataRow.Codigo };

    executarReST("ComissaoFuncionarioMotorista/NaoGerarComissao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridMotoristas.AtualizarDataRow(row, dataRow);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function gerarComissaoClick(dataRow, row) {
    var data = { Codigo: dataRow.Codigo };
    executarReST("ComissaoFuncionarioMotorista/GerarComissao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridMotoristas.AtualizarDataRow(row, dataRow);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, NumeroDiasEmViagem: dataRow.NumeroDiasEmViagem, AtingiuMedia: dataRow.AtingiuMedia, ValorNormativo: dataRow.ValorNormativo };

    executarReST("ComissaoFuncionarioMotorista/AlterarDadosComissaoMotorista", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridMotoristas.AtualizarDataRow(row, dataRow);
                //_gridMotoristas.CarregarGrid();
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridMotorista.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function carregarComissoesMotoristas(callback) {
    _comissaoFuncionarioMotorista.ComissaoFuncionario.val(_comissaoFuncionario.Codigo.val());
    
    
    _gridMotoristas.CarregarGrid(callback);
}

function alterarMediaClick(dataRow, row) {
    _alterarMedia = new AlterarMedia();
    _alterarMedia.Codigo.val(dataRow.Codigo);
    _alterarMedia.Media.val(dataRow.MediaFinal);

    KoBindings(_alterarMedia, "knoutAlerarMedia");

    Global.abrirModal('divModalAlerarMedia');
}

function alterarMediaComissaoClick(data) {
    Salvar(_alterarMedia, "ComissaoFuncionarioMotorista/AlterarMedia", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalAlerarMedia');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Média alterada com sucesso.");
                _gridMotoristas.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function detalhesComissaoFuncionarioClick(dataRow, row) {
    carregarComissoesMotoristasDocumentos(dataRow, row);
}

function AtualizarProgressComissao(codigoComissao, percentual) {
    if (_comissaoFuncionario.Codigo.val() == codigoComissao) {
        SetarPercentualProcessamento(percentual);
    }
}

function LimparDetalhesComissaoMotorista() {
    SetarPercentualProcessamento(0);
    $("#liMotoristas").hide();

    _comissaoFuncionarioMotorista.Motorista.visibleGeracao(false);
    _comissaoFuncionarioMotorista.Motorista.visible(false);
    _comissaoFuncionarioMotorista.Motorista.visibleFalha(false);
    _comissaoFuncionarioMotorista.CargoMotorista.visibleGeracao(false);
    _comissaoFuncionarioMotorista.CargoMotorista.visible(false);
    _comissaoFuncionarioMotorista.CargoMotorista.visibleFalha(false);

    LimparCampos(_comissaoFuncionarioMotorista);
}

function SetarPercentualProcessamento(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _comissaoFuncionarioMotorista.PercentualProcessado.val(strPercentual);
    $("#" + _comissaoFuncionarioMotorista.PercentualProcessado.id).css("width", strPercentual)
}

function desativarEditarGridFuncionarioMotorista() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridMotoristas.SetarEditarColunas(editarColuna);
}

function habilitarEditarGridFuncionarioMotorista() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridMotoristas.SetarEditarColunas(editarColuna);
}

function visibilidadeNaoGerarComissao(data) {
    if (data.GerarComissao && _comissaoFuncionario.SituacaoComissaoFuncionario.val() != EnumSituacaoComissaoFuncionario.Finalizada && _comissaoFuncionario.SituacaoComissaoFuncionario.val() != EnumSituacaoComissaoFuncionario.Cancelada && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados, _PermissoesPersonalizadas))
        return true;
    else
        return false;
}

function visibilidadeAlterarMedia(data) {
    if (!_CONFIGURACAO_TMS.UtilizarComissaoPorCargo)
        return false;
    else {
        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ComissaoFuncionario_PermitirAlterarMedia, _PermissoesPersonalizadas))
            return true;
        else
            return false;
    }
}

function visibilidadeImpressao(data) {
    if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.Finalizada || _comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.Gerada)
        return true;
    else
        return false;
}

function visibilidadeGerarComissao(data) {
    if (!data.GerarComissao && _comissaoFuncionario.SituacaoComissaoFuncionario.val() != EnumSituacaoComissaoFuncionario.Finalizada && _comissaoFuncionario.SituacaoComissaoFuncionario.val() != EnumSituacaoComissaoFuncionario.Cancelada && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados, _PermissoesPersonalizadas))
        return true;
    else
        return false;
}
