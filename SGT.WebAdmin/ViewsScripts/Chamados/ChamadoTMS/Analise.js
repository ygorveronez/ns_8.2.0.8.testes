/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="ChamadoTMS.js" />
/// <reference path="DocumentoAnalise.js" />
/// <reference path="AutorizacaoAnalise.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _analise;
var _gridAnalises;
var _CRUDAnalise;

var Analise = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });//Código da análise
    this.CodigoChamado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", text: "*Observação:", required: ko.observable(true), enable: ko.observable(false) });
    this.DataAnalise = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: "Data Análise:", enable: ko.observable(false) });

    this.Analises = PropertyEntity({ type: types.local, idGrid: guid() });

    this.Salvar = PropertyEntity({ eventClick: salvarAnaliseClick, type: types.event, text: "Salvar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparAnaliseClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAnaliseClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var CRUDAnalise = function () {
    this.Delegar = PropertyEntity({ eventClick: abrirModalDelegarClick, type: types.event, text: "Delegar", visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: ko.observable("Liberar Ocorrência"), visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ eventClick: fecharSemOcorrenciaClick, type: types.event, text: ko.observable("Fechar (sem ocorrência)"), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var Delegar = function () {
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Usuário:", idBtnSearch: guid() });

    this.Delegar = PropertyEntity({ type: types.event, eventClick: delegarClick, text: "Delegar" });
};

//*******EVENTOS*******

function loadAnalise() {
    _analise = new Analise();
    KoBindings(_analise, "tabAnalise");

    _CRUDAnalise = new CRUDAnalise();
    KoBindings(_CRUDAnalise, "knockoutCRUDAnalise");

    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    new BuscarFuncionario(_delegar.Usuario);

    $("#divModalDelegar").on('hidden.bs.modal', function () {
        LimparCampoEntity(_delegar.Usuario);
    });

    loadDocumentoAnalise();
    loadAutorizacaoAnalise();

    BuscarAnalises();
}

function abrirModalDelegarClick(e) {
    Global.abrirModal('divModalDelegar');
}

function delegarClick() {
    var dados = {
        Codigo: _chamadoTMS.Codigo.val(),
        Usuario: _delegar.Usuario.codEntity()
    };

    if (dados.Usuario == 0)
        return exibirMensagem(tipoMensagem.aviso, "Delegar", "Nenhum usuário selecionado.");

    executarReST("ChamadoTMSAnalise/Delegar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalDelegar');
                BuscarChamadoTMSPorCodigo(_chamadoTMS.Codigo.val());
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function finalizarClick(e, sender) {
    /*exibirConfirmacao("Confirmação", "Você realmente deseja liberar o lançamento de ocorrência para esse chamado?", function () {        
        executarReST("ChamadoTMSAnalise/AbrirOcorrencia", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarChamadoTMSPorCodigo(_chamadoTMS.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });*/
}

function fecharSemOcorrenciaClick(e, sender) {
    /*exibirConfirmacao("Confirmação", "Você realmente deseja finalizar esse chamado?", function () {
        executarReST("ChamadoTMSAnalise/FinalizarChamado", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _chamadoTMS.Situacao.val(EnumSituacaoChamadoTMS.Finalizado);
                    SetarEtapaChamado();
                    _gridChamados.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });*/
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar esse chamado?", function () {
        executarReST("ChamadoTMSAnalise/CancelarChamado", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _chamadoTMS.Situacao.val(EnumSituacaoChamadoTMS.Cancelado);
                    SetarEtapaChamado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function salvarAnaliseClick(e, sender) {
    Salvar(_analise, "ChamadoTMSAnalise/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                limparAnaliseClick();
                _gridAnalises.CarregarGrid();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Análise salva com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function editarAnaliseClick(dataRow) {
    executarReST("ChamadoTMSAnalise/BuscarPorCodigo", { Codigo: dataRow.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _analise.Codigo.val(arg.Data.Codigo);
                _analise.Observacao.val(arg.Data.Observacao);
                _analise.DataAnalise.val(arg.Data.DataAnalise);

                ControleCamposAnalise(false);
                _analise.Observacao.enable(true);
                _analise.Limpar.visible(true);
                _analise.Salvar.visible(true);
                _analise.Excluir.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function excluirAnaliseClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Análise?", function () {
        ExcluirPorCodigo(_analise, "ChamadoTMSAnalise/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAnalises.CarregarGrid();
                    limparAnaliseClick();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function limparAnaliseClick() {
    _analise.Codigo.val(0);
    _analise.Observacao.val("");
    _analise.DataAnalise.val("");

    AvaliarRegras();
    _analise.Limpar.visible(false);
    _analise.Excluir.visible(false);
}

//*******MÉTODOS*******

function EditarAnalise(data) {
    _analise.CodigoChamado.val(data.Codigo);

    PreencherObjetoKnout(_documentoAnalise, { Data: data.DocumentoAnalise });
    PreencherObjetoKnout(_autorizacaoAnalise, { Data: data.AutorizacaoAnalise });

    CarregarAnexosDocumentoAnalise(data);

    if (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.EmAnalise && (data.CodigoResponsavel == _CONFIGURACAO_TMS.CodigoUsuarioLogado || data.CodigoAutor == _CONFIGURACAO_TMS.CodigoUsuarioLogado) && !_CONFIGURACAO_TMS.PermitirAssumirChamadoDeOutroResponsavel)
        _CRUDAnalise.Delegar.visible(true);
    else
        _CRUDAnalise.Delegar.visible(false);

    _gridAnalises.CarregarGrid();

    AvaliarRegras();
}

function AvaliarRegras() {
    var situacao = _chamadoTMS.Situacao.val();

    if ((situacao === EnumSituacaoChamadoTMS.EmAnalise || situacao === EnumSituacaoChamadoTMS.AguardandoAutorizacao) && _chamadoTMS.PodeEditar.val())
        ControleCamposAnalise(true);
    else
        ControleCamposAnalise(false);

    $("#liAutorizacaoAnalise").show();
    ControleCamposCRUDAnalise(false);

    if (situacao === EnumSituacaoChamadoTMS.EmAnalise)
        $("#liAutorizacaoAnalise").hide();
    else if (situacao === EnumSituacaoChamadoTMS.LiberadaOcorrencia)
        ControleCamposCRUDAnalise(true);
}

function BuscarAnalises() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAnaliseClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridAnalises = new GridView(_analise.Analises.idGrid, "ChamadoTMSAnalise/Pesquisa", _analise, menuOpcoes);
}

function ControleCamposAnalise(status) {
    _analise.Observacao.enable(status);
    _analise.Salvar.visible(status);

    ControleCamposDocumentoAnalise(status);
    ControleCamposAutorizacaoAnalise(status);
}

function ControleCamposCRUDAnalise(status) {
    _CRUDAnalise.Finalizar.visible(status);
    _CRUDAnalise.Fechar.visible(status);
    _CRUDAnalise.Cancelar.visible(status);
}

function LimparCamposAnalise() {
    _CRUDAnalise.Delegar.visible(false);
    LimparCampos(_analise);
    ControleCamposAnalise(true);

    limparCamposDocumentoAnalise();
    limparCamposAutorizacaoAnalise();
}