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
/// <reference path="../../Enumeradores/EnumSituacaoControleVisita.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _controleVisita;
var _pesquisaControleVisita;
var _gridControleVisita;

var ControleVisita = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoControleVisitaPessoa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoControleVisitaPessoaFoto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true), required: false, enable: ko.observable(false), val: ko.observable("") });
    this.DataHoraEntrada = PropertyEntity({ text: "*Data Entrada:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(false) });
    this.DataHoraPrevisaoSaida = PropertyEntity({ text: "*Data Previsão Saída:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.CPF = PropertyEntity({ text: "*CPF:", getType: typesKnockout.cpf, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.Nome = PropertyEntity({ text: "*Nome:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.DataNascimento = PropertyEntity({ text: "Data Nascimento:", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Identidade = PropertyEntity({ text: "Identidade:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.OrgaoEmissor = PropertyEntity({ text: "Org. Emissor:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Estado:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Empresa:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Autorizador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Autorizador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Setor:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo Veículo:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataHoraSaida = PropertyEntity({ text: "*Data Saída:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(true), required: false });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Foto:", val: ko.observable(""), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.ImagensConferencia = PropertyEntity({ val: ko.observableArray([]) });

    this.SituacaoControleVisita = PropertyEntity({ val: ko.observable(EnumSituacaoControleVisita.Aberto), def: EnumSituacaoControleVisita.Aberto, getType: typesKnockout.int });

    // CRUD
    this.VisualizarFoto = PropertyEntity({ eventClick: VisualizarFotoClick, type: types.event, text: "Visualizar", visible: ko.observable(false) });
    this.ImprimirEtiqueta = PropertyEntity({ eventClick: ImprimirEtiquetaClick, type: types.event, text: "Etiqueta", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

var PesquisaControleVisita = function () {
    this.CPF = PropertyEntity({ text: "CPF:", getType: typesKnockout.cpf, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: "Nome:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Empresa:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoControleVisita = PropertyEntity({ val: ko.observable(EnumSituacaoControleVisita.Aberto), options: EnumSituacaoControleVisita.obterOpcoesPesquisa(), def: EnumSituacaoControleVisita.Aberto, text: "Situação: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleVisita.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadControleVisita() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaControleVisita = new PesquisaControleVisita();
    KoBindings(_pesquisaControleVisita, "knockoutPesquisaControleVisita", false, _pesquisaControleVisita.Pesquisar.id);

    // Instancia objeto principal
    _controleVisita = new ControleVisita();
    KoBindings(_controleVisita, "knockoutControleVisita");

    new BuscarEstados(_controleVisita.Estado);
    new BuscarFuncionario(_controleVisita.Autorizador);
    new BuscarSetorFuncionario(_controleVisita.Setor);

    HeaderAuditoria("ControleVisita", _controleVisita);

    $("#" + _controleVisita.PlacaVeiculo.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    // Inicia busca
    buscarDadosControleVisita();
}

function buscarDadosControleVisita() {
    executarReST("ControleVisita/BuscarDadosControleVisita", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data !== null) {
                _controleVisita.Numero.val(arg.Data.ProximoNumero);
                _controleVisita.DataHoraEntrada.val(Global.DataHoraAtual());
                buscarControleVisita();
            }
        }
    });
}

function ImprimirEtiquetaClick(e, sender) {
    executarDownload("ControleVisita/ImprimirEtiqueta", {
        Autorizador: _controleVisita.Autorizador.val(),
        CPF: _controleVisita.CPF.val(),
        Empresa: _controleVisita.Empresa.val(),
        ModeloVeiculo: _controleVisita.ModeloVeiculo.val(),
        Nome: _controleVisita.Nome.val(),
        PlacaVeiculo: _controleVisita.PlacaVeiculo.val(),
        Setor: _controleVisita.Setor.val()
    });
}

function VisualizarFotoClick(e, sender) {
    if (_controleVisita.Codigo.val() > 0) {
        var data = { Codigo: _controleVisita.Codigo.val() };
        executarReST("ControleVisita/ObterImagem", data, function (arg) {
            if (arg.Success) {
                // Do servidor, os objetos sao retornados por ordem de código
                // Aqui é ordenado por ordem da grid
                //_controleVisita.ImagensConferencia.val.removeAll();
                var imagensConferencia = [];
                for (var i in arg.Data.Imagens) {
                    var obj = arg.Data.Imagens[i];
                    var objordenado = arg.Data.Imagens.splice(i, 1);
                    imagensConferencia.push(objordenado[0]);
                }
                _controleVisita.ImagensConferencia.val(imagensConferencia);
                //setTimeout(dragg.centralize, 500);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

            finalizarRequisicao();
        });
    }
}

function adicionarClick(e, sender) {
    Salvar(_controleVisita, "ControleVisita/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (temFoto()) {
                    EnviarFoto(arg.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridControleVisita.CarregarGrid();
                    limparCamposControleVisita();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_controleVisita, "ControleVisita/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (temFoto()) {
                    EnviarFoto(arg.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridControleVisita.CarregarGrid();
                    limparCamposControleVisita();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function consultarVisitaCPF(e, sender) {
    if (_controleVisita.CPF.val() !== "" && _controleVisita.Codigo.val() === 0 && _controleVisita.SituacaoControleVisita.val() === EnumSituacaoControleVisita.Aberto) {
        var data = { CPF: _controleVisita.CPF.val() };
        executarReST("ControleVisita/BuscarDadosCPF", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.Codigo > 0)
                    BuscarControleVisitaCodigo(arg.Data.Codigo);
                else {
                    var numeroAnterior = _controleVisita.Numero.val();
                    var dataHoraEntrada = _controleVisita.DataHoraEntrada.val();
                    var dataHoraPrevisaoSaida = _controleVisita.DataHoraPrevisaoSaida.val();

                    dataPreencher = { Data: arg.Data };
                    PreencherObjetoKnout(_controleVisita, dataPreencher);

                    _controleVisita.Numero.val(numeroAnterior);
                    _controleVisita.DataHoraEntrada.val(dataHoraEntrada);
                    _controleVisita.DataHoraPrevisaoSaida.val(dataHoraPrevisaoSaida);
                }
            } else {
                //exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function cancelarClick(e) {
    limparCamposControleVisita();
}

function editarControleVisitaClick(itemGrid) {
    // Limpa os campos
    limparCamposControleVisita();

    // Seta o codigo do objeto    
    BuscarControleVisitaCodigo(itemGrid.Codigo);
}

//*******MÉTODOS*******

function BuscarControleVisitaCodigo(codigo) {
    _controleVisita.Codigo.val(codigo);
    // Busca informacoes para edicao
    BuscarPorCodigo(_controleVisita, "ControleVisita/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaControleVisita.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _controleVisita.VisualizarFoto.visible(true);
                _controleVisita.Atualizar.visible(true);
                _controleVisita.Cancelar.visible(true);
                _controleVisita.Adicionar.visible(false);
                _controleVisita.DataHoraSaida.visible(true);
                _controleVisita.DataHoraSaida.required = false;

                if (_controleVisita.SituacaoControleVisita.val() === EnumSituacaoControleVisita.Fechado) {
                    DesabilitarCampos(_controleVisita);
                    _controleVisita.Arquivo.enable(false);
                    _controleVisita.Atualizar.visible(false);
                }
                else {
                    HabilitarCampos(_controleVisita);

                    _controleVisita.Numero.enable(false);
                    _controleVisita.DataHoraEntrada.enable(false);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function buscarControleVisita() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleVisitaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridControleVisita = new GridView(_pesquisaControleVisita.Pesquisar.idGrid, "ControleVisita/Pesquisa", _pesquisaControleVisita, menuOpcoes, null);
    _gridControleVisita.CarregarGrid();
}

function EnviarFoto(codigoControleVisita) {
    var file;
    var documentos = new Array();

    file = document.getElementById(_controleVisita.Arquivo.id);
    file = file.files[0];
    var formData = new FormData();
    formData.append("upload", file);
    var data = {
        CodigoControleVisita: codigoControleVisita
    };
    enviarArquivo("ControleVisita/EnviarFoto?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");
                _gridControleVisita.CarregarGrid();
                limparCamposControleVisita();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function temFoto() {
    var valido = false;
    if (_controleVisita.Arquivo.val() !== "") {
        valido = true;
    }
    return valido;
}

function limparCamposControleVisita() {
    _controleVisita.VisualizarFoto.visible(false);
    _controleVisita.Atualizar.visible(false);
    _controleVisita.Cancelar.visible(true);
    _controleVisita.Adicionar.visible(true);
    LimparCampos(_controleVisita);
    _controleVisita.Arquivo.val("");

    HabilitarCampos(_controleVisita);
    _controleVisita.Numero.enable(false);
    _controleVisita.DataHoraEntrada.enable(false);
    _controleVisita.DataHoraSaida.visible(false);
    _controleVisita.DataHoraSaida.required = false;

    buscarDadosControleVisita();
}

function DesabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable !== null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable !== null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}