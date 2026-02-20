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
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumTipoVeiculo.js" />
/// <reference path="../../Consultas/CheckListTipo.js" />
/// <reference path="GuaritaTMSReboque.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridguaritaTMS;
var _guaritaTMS;
var _pesquisaguaritaTMS;
var _CRUDGuaritaTMS;

var PesquisaGuaritaTMS = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Reboque = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.KMAtual = PropertyEntity({ text: "KM de Lançamento: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.TipoEntradaSaida = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "Entrada/Saída: " });
    this.PlacaVeiculoTerceiro = PropertyEntity({ text: "Placa Veículo Terceiro: ", getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridguaritaTMS.CarregarGrid();
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
};

var GuaritaTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Operador:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.OrdemServicoFrota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Serviço:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Motorista:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Veículo:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Empresa:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.CheckListTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Check List:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.KMAtual = PropertyEntity({ text: "*KM Atual: ", required: true, getType: typesKnockout.int, type: types.int, configInt: { precision: 0, allowZero: true }, enable: ko.observable(true) });
    this.NumeroFrota = PropertyEntity({ text: "N° Frota: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.MotoristaTerceiro = PropertyEntity({ text: "Motorista Terceiro:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false), required: ko.observable(false) });
    this.DataSaidaEntrada = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date, type: types.date, enable: ko.observable(false) });
    this.HoraSaidaEntrada = PropertyEntity({ text: "*Hora: ", required: true, getType: typesKnockout.time, type: types.time, enable: ko.observable(false) });
    this.TipoEntradaSaida = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Entrada), options: EnumEntradaSaida.obterOpcoes(), def: EnumEntradaSaida.Entrada, text: "*Tipo: ", enable: ko.observable(false), eventChange: TipoEntradaSaidaChange });

    this.FinalizouViagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Finalizou Viagem?", enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.RetornouComReboque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Retornou com o Reboque?", enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.VeiculoVazio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "O veículo está vazio?", enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.HorarioInformadoManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Data/Hora Informados Manualmente?", enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarCheckList = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja gerar check list automaticamente após este lançamento?", enable: ko.observable(true), visible: ko.observable(true) });
    this.EntrouCarregado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "O veículo entrou carregado?", enable: ko.observable(true), visible: ko.observable(true) });
    this.AlterarSituacaoVeiculoParaLiberado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja alterar a situação do veículo para Liberado?", enable: ko.observable(true), visible: ko.observable(false) });
    this.AlterarReboquesVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja alterar o vínculo dos reboques do veículo?", enable: ko.observable(true), visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500, enable: ko.observable(true) });

    this.Reboques = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TipoVeiculo = PropertyEntity({ val: ko.observable(EnumTipoVeiculoProprietario.Proprio), options: EnumTipoVeiculoProprietario.obterOpcoes(), def: EnumTipoVeiculoProprietario.Proprio, text: "*Tipo Veículo: ", enable: ko.observable(true), eventChange: TipoVeiculoChange });
    this.PlacaVeiculoTerceiro = PropertyEntity({ text: "Veículo Terceiro: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.HorarioInformadoManualmente.val.subscribe(function (novoValor) {
        if (!novoValor) {

            if (_guaritaTMS.Codigo.val() <= 0)
                BuscarDadosPrincipais();

            _guaritaTMS.DataSaidaEntrada.enable(false);
            _guaritaTMS.HoraSaidaEntrada.enable(false);

        } else if (novoValor) {
            _guaritaTMS.DataSaidaEntrada.enable(true);
            _guaritaTMS.HoraSaidaEntrada.enable(true);
        }
    });

    this.GerarCheckList.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _guaritaTMS.CheckListTipo.required(false);
            LimparCampoEntity(_guaritaTMS.CheckListTipo);
        } else if (novoValor) {
            _guaritaTMS.CheckListTipo.required(true);
        }
    });

    this.OrdemServicoFrota.codEntity.subscribe(function () {
        visibleCampoAlterarSituacaoVeiculo()
    });
    this.TipoEntradaSaida.val.subscribe(function () {
        visibleCampoAlterarSituacaoVeiculo()
    });

    this.AlterarReboquesVeiculo.val.subscribe(function (novoValor) {
        $("#liTabReboques").hide();
        if (novoValor)
            $("#liTabReboques").show();
    });

    this.Veiculo.codEntity.subscribe(
        function (novoValor) {
            if (novoValor > 0) {
                _guaritaTMS.NumeroFrota.visible(true);
                RetornoNumeroFrota();
            }
            else
                _guaritaTMS.NumeroFrota.visible(false);
        }
    );

    this.TipoVeiculo.val.subscribe(
        function (novoValor) {
            var terceiro = novoValor == EnumTipoVeiculoProprietario.Terceiro
            _guaritaTMS.Motorista.visible(!terceiro);
            _guaritaTMS.Motorista.required(!terceiro);
            _guaritaTMS.TipoEntradaSaida.enable(terceiro);
            _guaritaTMS.MotoristaTerceiro.visible(terceiro);

        }
    );
};

var CRUDGuaritaTMS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadGuaritaTMS() {
    $.get("Content/Static/Logistica/GuaritaTMS.html?dyn=" + guid(), function (dataConteudo) {
        $("#conteudoGuarita").html(dataConteudo);

        _guaritaTMS = new GuaritaTMS();
        KoBindings(_guaritaTMS, "knockoutCadastroGuaritaTMS");

        _CRUDGuaritaTMS = new CRUDGuaritaTMS();
        KoBindings(_CRUDGuaritaTMS, "knockoutCRUDGuaritaTMS");

        new BuscarCargas(_guaritaTMS.Carga, RetornoCargas);
        new BuscarOrdemServico(_guaritaTMS.OrdemServicoFrota, RetornoOrdemServico);
        new BuscarMotoristas(_guaritaTMS.Motorista, RetornoMotorista);
        new BuscarVeiculos(_guaritaTMS.Veiculo, RetornarVeiculo);
        new BuscarCheckListTipo(_guaritaTMS.CheckListTipo);
        new BuscarEmpresa(_guaritaTMS.Empresa, null, true);

        HeaderAuditoria("GuaritaTMS", _guaritaTMS);

        _pesquisaguaritaTMS = new PesquisaGuaritaTMS();
        KoBindings(_pesquisaguaritaTMS, "knockoutPesquisaGuaritaTMS", false, _pesquisaguaritaTMS.Pesquisar.id);

        new BuscarCargas(_pesquisaguaritaTMS.Carga);
        new BuscarOrdemServico(_pesquisaguaritaTMS.OrdemServico);
        new BuscarMotoristas(_pesquisaguaritaTMS.Motorista);
        new BuscarVeiculos(_pesquisaguaritaTMS.Veiculo);
        new BuscarVeiculos(_pesquisaguaritaTMS.Reboque);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            _guaritaTMS.Carga.visible(false);
            _guaritaTMS.OrdemServicoFrota.visible(false);
            _pesquisaguaritaTMS.Carga.visible(false);
            _pesquisaguaritaTMS.OrdemServico.visible(false);
        }

        loadGuaritaTMSReboque();

        buscarguaritaTMSs();
        BuscarDadosPrincipais();
    });
}

function RetornoMotorista(data) {
    _guaritaTMS.Motorista.codEntity(data.Codigo);
    _guaritaTMS.Motorista.val(data.Descricao);

    executarReST("GuaritaTMS/RetornarVeiculoMotorista", { CodigoMotorista: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                RetornarVeiculo(r.Data);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
        }
    });
}

function BuscarDadosPrincipais() {
    if (_guaritaTMS.Codigo.val() === 0 || _guaritaTMS.Codigo.val() === "" || _guaritaTMS.Codigo.val() === undefined) {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data !== null) {
                    _guaritaTMS.Operador.codEntity(arg.Data.Codigo);
                    _guaritaTMS.Operador.val(arg.Data.Nome);
                    _guaritaTMS.DataSaidaEntrada.val(arg.Data.DataAtual);
                    _guaritaTMS.HoraSaidaEntrada.val(arg.Data.HoraAtual);
                }

                BuscarDadosEmpresa();
            }
        });
    }
}

function BuscarDadosEmpresa() {
    executarReST("GuaritaTMS/ObterEmpresaPadraoGuarita", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;
                _guaritaTMS.Empresa.codEntity(data.Empresa.Codigo);
                _guaritaTMS.Empresa.val(data.Empresa.Descricao);
            }
        }
    });
}

function TipoEntradaSaidaChange(e, sender) {
    if (_guaritaTMS.TipoEntradaSaida.val() === EnumEntradaSaida.Entrada)
        visibleCamposTipoEntradaSaida(true);
    else
        visibleCamposTipoEntradaSaida(false);
}

function TipoVeiculoChange(e, sender) {
    if (_guaritaTMS.TipoVeiculo.val() === EnumTipoVeiculoProprietario.Proprio) {
        _guaritaTMS.Veiculo.visible(true);
        _guaritaTMS.PlacaVeiculoTerceiro.visible(false);
    } else {
        _guaritaTMS.Veiculo.visible(false);
        _guaritaTMS.PlacaVeiculoTerceiro.visible(true);
        _guaritaTMS.NumeroFrota.visible(false);
    }
}

function RetornarVeiculo(data) {
    _guaritaTMS.Veiculo.codEntity(data.Codigo);

    if (data.Reboque != "")
        _guaritaTMS.Veiculo.val(data.Placa + " (" + data.Reboque + ")");
    else if (data.Tracao != "" && data.CodigoTracao > 0) {
        _guaritaTMS.Veiculo.val(data.Tracao + " (" + data.Placa + ")");
        _guaritaTMS.Veiculo.codEntity(data.CodigoTracao);
    }
    else
        _guaritaTMS.Veiculo.val(data.Placa);

    ConsultarGuaritaVeiculo(data.Codigo);
}

function RetornoOrdemServico(data) {
    _guaritaTMS.OrdemServicoFrota.codEntity(data.Codigo);
    _guaritaTMS.OrdemServicoFrota.val(data.Numero);
    if (data.CodigoVeiculo > 0) {
        _guaritaTMS.Veiculo.codEntity(data.CodigoVeiculo);
        _guaritaTMS.Veiculo.val(data.Veiculo);
        ConsultarGuaritaVeiculo(data.CodigoVeiculo);
    }
    if (data.CodigoMotorista > 0) {
        _guaritaTMS.Motorista.codEntity(data.CodigoMotorista);
        _guaritaTMS.Motorista.val(data.Motorista);
    }
}

function RetornoCargas(data) {
    _guaritaTMS.Carga.codEntity(data.Codigo);
    _guaritaTMS.Carga.val(data.CodigoCargaEmbarcador);
    if (data.CodigoVeiculo > 0) {
        _guaritaTMS.Veiculo.codEntity(data.CodigoVeiculo);
        _guaritaTMS.Veiculo.val(data.Veiculo);
        ConsultarGuaritaVeiculo(data.CodigoVeiculo);
    }
    if (data.CodigoMotorista > 0) {
        _guaritaTMS.Motorista.codEntity(data.CodigoMotorista);
        _guaritaTMS.Motorista.val(data.Motorista);
    }
}

function RetornoNumeroFrota() {
    executarReST("GuaritaTMS/RetornarNumeroFrota", { CodigoVeiculo: _guaritaTMS.Veiculo.codEntity() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _guaritaTMS.NumeroFrota.val(r.Data.NumeroFrota);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
        }
    });
}

function ConsultarGuaritaVeiculo(codigoVeiculo) {
    if (codigoVeiculo > 0) {
        executarReST("GuaritaTMS/ConsultarGuaritaVeiculo", { CodigoVeiculo: codigoVeiculo, CodigoGuarita: _guaritaTMS.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _guaritaTMS.TipoEntradaSaida.enable(false);
                    if (r.Data.TipoEntradaSaida === EnumEntradaSaida.Entrada) {
                        _guaritaTMS.TipoEntradaSaida.val(EnumEntradaSaida.Saida);
                        visibleCamposTipoEntradaSaida(false);

                        if (_CONFIGURACAO_TMS.PreencherUltimoKMEntradaGuaritaTMS)
                            _guaritaTMS.KMAtual.val(r.Data.KMAtual);
                    }
                    else {
                        _guaritaTMS.TipoEntradaSaida.val(EnumEntradaSaida.Entrada);
                        visibleCamposTipoEntradaSaida(true);
                    }
                } else {
                    _guaritaTMS.TipoEntradaSaida.enable(true);
                }

                CarregarReboquesAtuaisVeiculo(codigoVeiculo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
            }
        });
    }
}

function CarregarReboquesAtuaisVeiculo(codigoVeiculo) {
    if (codigoVeiculo > 0) {
        executarReST("GuaritaTMS/CarregarReboquesAtuaisVeiculo", { CodigoVeiculo: codigoVeiculo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _guaritaTMS.Reboques.val(arg.Data.Reboques);
                    RecarregarGridGuaritaTMSReboque();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function visibleCampoAlterarSituacaoVeiculo() {
    if (_guaritaTMS.TipoEntradaSaida.val() === EnumEntradaSaida.Entrada && _guaritaTMS.OrdemServicoFrota.codEntity() > 0)
        _guaritaTMS.AlterarSituacaoVeiculoParaLiberado.visible(true);
    else {
        _guaritaTMS.AlterarSituacaoVeiculoParaLiberado.visible(false);
        _guaritaTMS.AlterarSituacaoVeiculoParaLiberado.val(false);
    }
}

function adicionarClick(e, sender) {
    ValidarCheckListGerado(e, sender, false);
}

function atualizarClick(e, sender) {
    ValidarCheckListGerado(e, sender, true);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o registro do veículo selecionado?", function () {
        ExcluirPorCodigo(_guaritaTMS, "GuaritaTMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com Sucesso");
                _gridguaritaTMS.CarregarGrid();
                limparCamposguaritaTMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposguaritaTMS();
    BuscarDadosPrincipais();
}

//*******MÉTODOS*******

function ValidarCheckListGerado(e, sender, atualizar) {
    if (_guaritaTMS.TipoEntradaSaida.val() === EnumEntradaSaida.Saida && _guaritaTMS.Carga.codEntity() > 0 && _guaritaTMS.Veiculo.codEntity() > 0) {
        executarReST("GuaritaTMS/ValidarCheckListEntrada", { CodigoVeiculo: _guaritaTMS.Veiculo.codEntity(), CodigoCarga: _guaritaTMS.Carga.codEntity() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (r.Data.ContemCheckListEntrada) {
                        if (atualizar) {
                            AtualizarGuaritaTMS(e, sender);
                        }
                        else {
                            InserirGuaritaTMS(e, sender);
                        }
                    } else {
                        exibirConfirmacao("Atenção!", "Não foi gerado Check-List para esta carga e veículo em sua entrada, deseja realmente salvar o registro da saída?", function () {
                            if (atualizar) {
                                AtualizarGuaritaTMS(e, sender);
                            }
                            else {
                                InserirGuaritaTMS(e, sender);
                            }
                        }, function () {
                            return;
                        });
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
            }
        });
    } else {
        if (atualizar) {
            AtualizarGuaritaTMS(e, sender);
        }
        else {
            InserirGuaritaTMS(e, sender);
        }
    }
}

function AtualizarGuaritaTMS(e, sender) {
    preencherListasSelecaoGuaritaTMSReboque();
    Salvar(_guaritaTMS, "GuaritaTMS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com Sucesso");
                _gridguaritaTMS.CarregarGrid();
                limparCamposguaritaTMS();
                BuscarDadosPrincipais();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function InserirGuaritaTMS(e, sender) {
    preencherListasSelecaoGuaritaTMSReboque();
    Salvar(_guaritaTMS, "GuaritaTMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso");
                _gridguaritaTMS.CarregarGrid();
                limparCamposguaritaTMS();
                BuscarDadosPrincipais();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function buscarguaritaTMSs() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarguaritaTMS, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridguaritaTMS = new GridView(_pesquisaguaritaTMS.Pesquisar.idGrid, "GuaritaTMS/Pesquisa", _pesquisaguaritaTMS, menuOpcoes, null);
    _gridguaritaTMS.CarregarGrid();
}

function editarguaritaTMS(guaritaTMSGrid) {
    limparCamposguaritaTMS();
    _guaritaTMS.Codigo.val(guaritaTMSGrid.Codigo);
    _guaritaTMS.HorarioInformadoManualmente.enable(false);

    //SAC/AtendimentoCliente visualiza a Guarita também
    BuscarPorCodigo(_guaritaTMS, "GuaritaTMS/BuscarPorCodigo", function (arg) {
        _pesquisaguaritaTMS.ExibirFiltros.visibleFade(false);
        _CRUDGuaritaTMS.Atualizar.visible(true);
        _CRUDGuaritaTMS.Cancelar.visible(true);
        _CRUDGuaritaTMS.Excluir.visible(true);
        _CRUDGuaritaTMS.Adicionar.visible(false);

        if (_guaritaTMS.TipoEntradaSaida.val() === EnumEntradaSaida.Entrada)
            visibleCamposTipoEntradaSaida(true);
        else
            visibleCamposTipoEntradaSaida(false);
        _guaritaTMS.GerarCheckList.visible(false);

        RecarregarGridGuaritaTMSReboque();
    }, null);
}

function limparCamposguaritaTMS() {
    _CRUDGuaritaTMS.Atualizar.visible(false);
    _CRUDGuaritaTMS.Cancelar.visible(true);
    _CRUDGuaritaTMS.Excluir.visible(false);
    _CRUDGuaritaTMS.Adicionar.visible(true);

    LimparCampos(_guaritaTMS);
    limparCamposGuaritaTMSReboque();

    SetarEnableCamposKnockout(_guaritaTMS, true);

    _guaritaTMS.TipoEntradaSaida.enable(false);
    _guaritaTMS.DataSaidaEntrada.enable(false);
    _guaritaTMS.HoraSaidaEntrada.enable(false);
    _guaritaTMS.Operador.enable(false);
    _guaritaTMS.Carga.requiredClass("form-control");
    _guaritaTMS.OrdemServicoFrota.requiredClass("form-control");
    _guaritaTMS.GerarCheckList.visible(true);

    visibleCamposTipoEntradaSaida(true);
    Global.ResetarAbas();
}

function visibleCamposTipoEntradaSaida(visible) {
    _guaritaTMS.FinalizouViagem.visible(visible);
    _guaritaTMS.RetornouComReboque.visible(visible);
    _guaritaTMS.EntrouCarregado.visible(visible);
}
