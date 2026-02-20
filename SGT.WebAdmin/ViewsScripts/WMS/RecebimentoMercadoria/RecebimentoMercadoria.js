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
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../../../js/plugin/dropzone/dropzone-amd-module.min.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/CargaMDFeManual.js" />
/// <reference path="../../Consultas/MDFe.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRecebimentoMercadoria.js" />
/// <reference path="../../Enumeradores/EnumTipoRecebimentoMercadoria.js" />
/// <reference path="Mercadoria.js" />
/// <reference path="Volume.js" />
/// <reference path="DropZone.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRecebimentoMercadoria;
var _recebimentoMercadoria;
var _pesquisaRecebimentoMercadoria;
var _crudRecebimentoMercadoria;
var _captcha;
var pesoUnitarioProduto = "";

var _situacaoRecebimento = [{ text: "Todos", value: EnumSituacaoRecebimentoMercadoria.Todos },
{ text: "Finalizado", value: EnumSituacaoRecebimentoMercadoria.Finalizado },
{ text: "Iniciado", value: EnumSituacaoRecebimentoMercadoria.Iniciado },
{ text: "Cancelado", value: EnumSituacaoRecebimentoMercadoria.Cancelado }];

var _situacao = [{ text: "Iniciado", value: EnumSituacaoRecebimentoMercadoria.Iniciado },
{ text: "Finalizado", value: EnumSituacaoRecebimentoMercadoria.Finalizado },
{ text: "Cancelado", value: EnumSituacaoRecebimentoMercadoria.Cancelado }];

var _tipoRecebimento = [{ text: "Mercadoria", value: EnumTipoRecebimentoMercadoria.Mercadoria },
{ text: "Volume", value: EnumTipoRecebimentoMercadoria.Volume }];

var _pesquisaTipoRecebimento = [{ text: "Todos", value: 0 },
{ text: "Mercadoria", value: EnumTipoRecebimentoMercadoria.Mercadoria },
{ text: "Volume", value: EnumTipoRecebimentoMercadoria.Volume }];

var PesquisaRecebimentoMercadoria = function () {
    this.Data = PropertyEntity({ text: "Data: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRecebimentoMercadoria.Todos), options: _situacaoRecebimento, def: EnumSituacaoRecebimentoMercadoria.Todos, text: "Situação: " });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "MDFe:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRecebimento = PropertyEntity({ val: ko.observable(0), options: _pesquisaTipoRecebimento, def: 0, text: "Tipo Recebimento: " });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroNota = PropertyEntity({ text: "Número Nota: ", maxlength: 100 });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 500 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRecebimentoMercadoria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RecebimentoMercadoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, type: types.int });
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), text: ko.observable("NF-e(s) para emissão"), enable: ko.observable(true), eventClick: verificarReceitaClick, visible: ko.observable(false) });
    this.Chave = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: "Chave de Acesso: ", def: "", maxlength: 54, required: false, visible: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.MDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "MDFe:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });

    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 500, visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRecebimentoMercadoria.Iniciado), options: _situacao, def: EnumSituacaoRecebimentoMercadoria.Iniciado, text: "*Situação: ", required: true, enable: ko.observable(true) });

    this.TipoRecebimento = PropertyEntity({ val: ko.observable(EnumTipoRecebimentoMercadoria.Mercadoria), options: _tipoRecebimento, def: EnumTipoRecebimentoMercadoria.Mercadoria, text: "*Tipo Recebimento: ", required: true, enable: ko.observable(true), eventChange: TipoRecebimentoChange });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veiculo:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Operador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Produto do Embarcador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(true) });
}

var CRUDRecebimentoMercadoria = function () {
    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadRecebimentoMercadoria() {
    _recebimentoMercadoria = new RecebimentoMercadoria();
    KoBindings(_recebimentoMercadoria, "knockoutCadastroRecebimentoMercadoria");

    _crudRecebimentoMercadoria = new CRUDRecebimentoMercadoria();
    KoBindings(_crudRecebimentoMercadoria, "knockoutCRUDRecebimentoMercadoria");

    _pesquisaRecebimentoMercadoria = new PesquisaRecebimentoMercadoria();
    KoBindings(_pesquisaRecebimentoMercadoria, "knockoutPesquisaRecebimentoMercadoria", _pesquisaRecebimentoMercadoria.Pesquisar.id);


    new BuscarCargas(_pesquisaRecebimentoMercadoria.Carga);
    new BuscarMDFes(_pesquisaRecebimentoMercadoria.MDFe);
    new BuscarMDFes(_recebimentoMercadoria.MDFe, RetornoMDFe);
    new BuscarFuncionario(_pesquisaRecebimentoMercadoria.Usuario);
    new BuscarVeiculos(_pesquisaRecebimentoMercadoria.Veiculo);
    new BuscarProdutos(_pesquisaRecebimentoMercadoria.ProdutoEmbarcador)


    new BuscarFuncionario(_recebimentoMercadoria.Usuario);
    new BuscarVeiculos(_recebimentoMercadoria.Veiculo, RetornoVeiculo);
    new BuscarProdutos(_recebimentoMercadoria.ProdutoEmbarcador, retornoProdutoEmbarcador)
    new BuscarEmpresa(_recebimentoMercadoria.Empresa);

    buscarRecebimentoMercadorias();
    BuscarUsuarioLogado();
    LoadMercadoria();
    loadDropZone();
    LoadVolume();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _recebimentoMercadoria.Empresa.visible(false);
        _recebimentoMercadoria.Empresa.required(false);
        _recebimentoMercadoria.TipoRecebimento.val(EnumTipoRecebimentoMercadoria.Volume);        
        new BuscarCargaLiberadasGuarita(_recebimentoMercadoria.Carga, RetornoCarga);
        TipoRecebimentoChange(_recebimentoMercadoria);
    }
    else {
        new BuscarCargas(_recebimentoMercadoria.Carga, RetornoCarga);
    }
}

function retornoProdutoEmbarcador(data) {
    _recebimentoMercadoria.ProdutoEmbarcador.val(data.Descricao);
    _recebimentoMercadoria.ProdutoEmbarcador.codEntity(data.Codigo);
    pesoUnitarioProduto = data.PesoUnitario;
}

function verificarReceitaClick(e, sender) { }

function BuscarUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _recebimentoMercadoria.Usuario.codEntity(arg.Data.Codigo);
                _recebimentoMercadoria.Usuario.val(arg.Data.Nome);
            }
        }
    });
}

function TipoRecebimentoChange(e, sender) {
    if (_recebimentoMercadoria.TipoRecebimento.val() == EnumTipoRecebimentoMercadoria.Mercadoria) {
        $("#liTabMercadoria").show();
        $("#litabVolume").hide();
        _recebimentoMercadoria.ProdutoEmbarcador.text("*Produto do Embarcador");
        _recebimentoMercadoria.ProdutoEmbarcador.required(true);
    }
    else {
        $("#liTabMercadoria").hide();
        $("#litabVolume").show();
        _recebimentoMercadoria.ProdutoEmbarcador.text("*Produto do Embarcador");
        _recebimentoMercadoria.ProdutoEmbarcador.required(true);
    }
}

function RetornoVeiculo(data) {
    if (data != null) {
        _recebimentoMercadoria.Veiculo.codEntity(data.Codigo);
        if (data.Reboque != "")
            _recebimentoMercadoria.Veiculo.val(data.Placa + " (" + data.Reboque + ")");
        else if (data.Tracao != "" && data.CodigoTracao > 0) {
            _recebimentoMercadoria.Veiculo.val(data.Tracao + " (" + data.Placa + ")");
            _recebimentoMercadoria.Veiculo.codEntity(data.CodigoTracao);
        }
        else
            _recebimentoMercadoria.Veiculo.val(data.Placa);
    }
}

function RetornoMDFe(dataRetorno) {
    var tudoCerto = true;
    tudoCerto = ValidarCamposObrigatorios(_recebimentoMercadoria);
    if (tudoCerto) {
        tudoCerto = ValidarCampoObrigatorioEntity(_recebimentoMercadoria.ProdutoEmbarcador);
        if (tudoCerto) {
            var data = {
                CodigoMDFe: dataRetorno.Codigo,
                CodigoRecebimento: _recebimentoMercadoria.Codigo.val(),
                CodigoProduto: _recebimentoMercadoria.ProdutoEmbarcador.codEntity(),
                Situacao: _recebimentoMercadoria.Situacao.val(),
                TipoRecebimento: _recebimentoMercadoria.TipoRecebimento.val(),
                Usuario: _recebimentoMercadoria.Usuario.codEntity(),
                Observacao: _recebimentoMercadoria.Observacao.val()
            };
            executarReST("RecebimentoMercadoria/ValidarMDFeSelecionada", data, function (arg) {
                if (arg.Success) {
                    _recebimentoMercadoria.Carga.codEntity(arg.Data.CodigoCarga);
                    _recebimentoMercadoria.Carga.val(arg.Data.CodigoCargaEmbarcador);

                    _recebimentoMercadoria.MDFe.codEntity(arg.Data.CodigoMDFe);
                    _recebimentoMercadoria.MDFe.val(arg.Data.Numero);

                    _recebimentoMercadoria.Veiculo.codEntity(arg.Data.CodigoVeiculo);
                    _recebimentoMercadoria.Veiculo.val(arg.Data.Placa);

                    _recebimentoMercadoria.Codigo.val(arg.Data.CodigoRecebimento);

                    if (_recebimentoMercadoria.TipoRecebimento.val() == EnumTipoRecebimentoMercadoria.Mercadoria) {
                        var data = { CodigoMDFe: dataRetorno.Codigo, CodigoRecebimento: _recebimentoMercadoria.Codigo.val() };
                        executarReST("RecebimentoMercadoria/BuscarProdutosMDFe", data, function (arg) {
                            if (arg.Success) {
                                if (arg.Data != false) {
                                    _gridMercadoria.CarregarGrid();

                                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga lançada com sucesso.");
                                } else {
                                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 200000);
                                }
                            } else {
                                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                            }
                        });
                    } else {
                        var data = { CodigoMDFe: dataRetorno.Codigo, CodigoRecebimento: _recebimentoMercadoria.Codigo.val(), CodigoProdutoEmbarcador: _recebimentoMercadoria.ProdutoEmbarcador.codEntity() };
                        executarReST("RecebimentoMercadoria/BuscarVolumesMDFe", data, function (arg) {
                            if (arg.Success) {
                                if (arg.Data != false) {
                                    console.log("codigo merc BuscarVolumesMDFe ", _recebimentoMercadoria.Codigo.val());
                                    _gridVolume.CarregarGrid();

                                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga lançada com sucesso.");
                                } else {
                                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 200000);
                                }
                            } else {
                                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                            }
                        });
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    limparCamposRecebimentoMercadoria();
                }
            });
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione o Produto antes de selecionar um MDFe.");
        }
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione os campos obrigatórios.");
    }
}

function RetornoCarga(dataRetorno) {
    var tudoCerto = true;
    tudoCerto = ValidarCamposObrigatorios(_recebimentoMercadoria);
    if (tudoCerto) {
        tudoCerto = ValidarCampoObrigatorioEntity(_recebimentoMercadoria.ProdutoEmbarcador);
        if (tudoCerto) {
            var data = {
                CodigoCarga: dataRetorno.Codigo,
                CodigoRecebimento: _recebimentoMercadoria.Codigo.val(),
                CodigoProduto: _recebimentoMercadoria.ProdutoEmbarcador.codEntity(),
                Situacao: _recebimentoMercadoria.Situacao.val(),
                TipoRecebimento: _recebimentoMercadoria.TipoRecebimento.val(),
                Usuario: _recebimentoMercadoria.Usuario.codEntity(),
                Observacao: _recebimentoMercadoria.Observacao.val()
            };
            executarReST("RecebimentoMercadoria/ValidarCargaSelecionada", data, function (arg) {
                if (arg.Success) {
                    _recebimentoMercadoria.Carga.codEntity(arg.Data.CodigoCarga);
                    _recebimentoMercadoria.Carga.val(arg.Data.CodigoCargaEmbarcador);

                    _recebimentoMercadoria.MDFe.codEntity(arg.Data.CodigoMDFe);
                    _recebimentoMercadoria.MDFe.val(arg.Data.Numero);

                    _recebimentoMercadoria.Veiculo.codEntity(arg.Data.CodigoVeiculo);
                    _recebimentoMercadoria.Veiculo.val(arg.Data.Placa);

                    _recebimentoMercadoria.Empresa.codEntity(arg.Data.CodigoEmpresa);
                    _recebimentoMercadoria.Empresa.val(arg.Data.Empresa);

                    _recebimentoMercadoria.Codigo.val(arg.Data.CodigoRecebimento);

                    if (_recebimentoMercadoria.TipoRecebimento.val() == EnumTipoRecebimentoMercadoria.Mercadoria) {
                        var data = { CodigoCarga: dataRetorno.Codigo, CodigoRecebimento: _recebimentoMercadoria.Codigo.val() };
                        executarReST("RecebimentoMercadoria/BuscarProdutosCarga", data, function (arg) {
                            if (arg.Success) {
                                if (arg.Data != false) {
                                    _gridMercadoria.CarregarGrid();
                                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga lançada com sucesso.");
                                } else {
                                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 200000);
                                }
                            } else {
                                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                            }
                        });
                    } else {
                        var data = { CodigoCarga: dataRetorno.Codigo, CodigoRecebimento: _recebimentoMercadoria.Codigo.val(), CodigoProdutoEmbarcador: _recebimentoMercadoria.ProdutoEmbarcador.codEntity() };
                        executarReST("RecebimentoMercadoria/BuscarVolumesCarga", data, function (arg) {
                            if (arg.Success) {
                                if (arg.Data != false) {
                                    console.log("codigo merc BuscarVolumesCarga ", _recebimentoMercadoria.Codigo.val());
                                    _gridVolume.CarregarGrid();
                                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga lançada com sucesso.");
                                } else {
                                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 200000);
                                }
                            } else {
                                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                            }
                        });
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    limparCamposRecebimentoMercadoria();
                }
            });
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione o Produto antes de selecionar a carga.");
        }
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione os campos obrigatórios.");
    }
}
function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar e validar o recebimento feito?", function () {
        resetarTabs();
        _recebimentoMercadoria.Situacao.val(EnumSituacaoRecebimentoMercadoria.Finalizado);
        Salvar(_recebimentoMercadoria, "RecebimentoMercadoria/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    limparCamposRecebimentoMercadoria();
                    buscarRecebimentoMercadorias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    if (arg.Data.ContemVolumesFaltantes && arg.Data.LiberarAutorizacao) {
                        _volume.AutorizarVolumes.visible(true);
                        $("#myTab a:eq(2)").tab("show");
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender);
    });
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(_recebimentoMercadoria, "RecebimentoMercadoria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                limparCamposRecebimentoMercadoria();
                buscarRecebimentoMercadorias();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposRecebimentoMercadoria();
}

//*******MÉTODOS*******

function buscarRecebimentoMercadorias() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRecebimentoMercadoria, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRecebimentoMercadoria = new GridView(_pesquisaRecebimentoMercadoria.Pesquisar.idGrid, "RecebimentoMercadoria/Pesquisa", _pesquisaRecebimentoMercadoria, menuOpcoes, null);
    _gridRecebimentoMercadoria.CarregarGrid();
}

function editarRecebimentoMercadoria(recebimentoMercadoriaGrid) {
    limparCamposRecebimentoMercadoria();
    _recebimentoMercadoria.Codigo.val(recebimentoMercadoriaGrid.Codigo);
    BuscarPorCodigo(_recebimentoMercadoria, "RecebimentoMercadoria/BuscarPorCodigo", function (arg) {
        LimparDropzone();
        LimparCamposMercadoria();
        //LimparCamposVolume();
        _pesquisaRecebimentoMercadoria.ExibirFiltros.visibleFade(false);
        _crudRecebimentoMercadoria.Cancelar.visible(true);
        _crudRecebimentoMercadoria.Adicionar.visible(true);
        _gridMercadoria.CarregarGrid();
        console.log("codigo merc editarRecebimentoMercadoria 1", _recebimentoMercadoria.Codigo.val());
        _gridVolume.CarregarGrid();

        if (_recebimentoMercadoria.Situacao.val() != EnumSituacaoRecebimentoMercadoria.Iniciado) {
            DesabilitarCampos(_recebimentoMercadoria);
            DesabilitarCampos(_mercadoria);
            DesabilitarCampos(_adicionarMercadoria);
            DesabilitarCampos(_volume);
            DesabilitarCampos(_adicionarVolume);

            _crudRecebimentoMercadoria.Cancelar.visible(true);
            _crudRecebimentoMercadoria.Adicionar.visible(false);
            $("#divDropzone").hide();
        }

        if (_recebimentoMercadoria.TipoRecebimento.val() == EnumTipoRecebimentoMercadoria.Mercadoria) {
            $("#liTabMercadoria").show();
            $("#litabVolume").hide();

        }
        else {
            $("#liTabMercadoria").hide();
            $("#litabVolume").show();
        }
        //console.log("codigo merc editarRecebimentoMercadoria 2", _recebimentoMercadoria.Codigo.val());
        //_gridVolume.CarregarGrid();
    }, null);
}

function limparCamposRecebimentoMercadoria() {
    pesoUnitarioProduto = "";
    _crudRecebimentoMercadoria.Cancelar.visible(true);
    _crudRecebimentoMercadoria.Adicionar.visible(true);
    _volume.AutorizarVolumes.visible(false);
    LimparDropzone();
    $("#divDropzone").show();
    $("#liTabMercadoria").show();
    $("#litabVolume").hide();
    _recebimentoMercadoria.ProdutoEmbarcador.text("Produto do Embarcador");
    _recebimentoMercadoria.ProdutoEmbarcador.required(false);

    HabilitarCampos(_recebimentoMercadoria);
    HabilitarCampos(_mercadoria);
    HabilitarCampos(_adicionarMercadoria);
    HabilitarCampos(_volume);
    HabilitarCampos(_adicionarVolume);

    _adicionarVolume.QuantidadeConferida.enable(false);
    _adicionarVolume.QuantidadeFaltante.enable(false);
    _adicionarVolume.MetroCubico.enable(false);


    resetarTabs();
    BuscarUsuarioLogado();

    LimparCampos(_recebimentoMercadoria);
    LimparCamposMercadoria();
    LimparCamposVolume();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _recebimentoMercadoria.TipoRecebimento.val(EnumTipoRecebimentoMercadoria.Volume);
        TipoRecebimentoChange(_recebimentoMercadoria);
    }
}

function LimparDropzone() {

}

function DesabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}