/// <reference path="../../../../Enumeradores/EnumTipoIntegracaoMercadoLivre.js" />
/// <reference path="../../DocumentosEmissao/Documentos.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLIntegracaoMercadoLivre = "";
var _pesquisaIntegracaoMercadoLivre;
var _gridIntegracaoMercadoLivre;
var _limparComposicaoCargaRetiradaRotaFacility;

var PesquisaIntegracaoMercadoLivre = function () {

    this.Grid = PropertyEntity({ type: types.local });

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoIntegracaoMercadoLivre = PropertyEntity({ val: ko.observable(EnumTipoIntegracaoMercadoLivre.HandlingUnit), options: EnumTipoIntegracaoMercadoLivre.obterOpcoes(), def: EnumTipoIntegracaoMercadoLivre.HandlingUnit, enable: ko.observable(true) });
    this.HandlingUnitID = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 50, getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.HandlingUnitID.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Rota = PropertyEntity({ getType: typesKnockout.int, maxlength: 11, text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), required: false, configInt: { precision: 0, allowZero: false }, visible: ko.observable(true), enable: ko.observable(true) });
    this.Facility = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 50, getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.Facility.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarHandlingUnit = PropertyEntity({ eventClick: AdicionarHandlingUnitMercadoLivreClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.AdicionarHU), icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: PesquisarHandlingUnitMercadoLivreClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, icon: "fal fa-sync", visible: ko.observable(true), enable: ko.observable(true) });
}

function AdicionarHandlingUnitMercadoLivreClick() {
    executarReST("CargaIntegracaoMercadoLivre/VincularHandlingUnit", { CargaPedido: _pesquisaIntegracaoMercadoLivre.CargaPedido.val(), TipoIntegracaoMercadoLivre: _cargaAtual.TipoIntegracaoMercadoLivre.val(), HandlingUnitID: _pesquisaIntegracaoMercadoLivre.HandlingUnitID.val(), Rota: _pesquisaIntegracaoMercadoLivre.Rota.val(), Facility: _pesquisaIntegracaoMercadoLivre.Facility.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaEFacilityAdicionadoComSucesso);
                }
                else {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.HandlingUnitAdicionadoComSucesso);
                }

                veririficarSeCargaMudouTipoContratacao(r.Data);
                carregarGridDocumentosParaEmissao();
                limparCamposDocumentosParaEmissao();

                _pesquisaIntegracaoMercadoLivre.HandlingUnitID.val("");
                _pesquisaIntegracaoMercadoLivre.Rota.val("");
                _pesquisaIntegracaoMercadoLivre.Facility.val("");
                if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                    _pesquisaIntegracaoMercadoLivre.Rota.get$().focus();
                }
                else {
                    _pesquisaIntegracaoMercadoLivre.HandlingUnitID.get$().focus();
                }
                _gridIntegracaoMercadoLivre.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function RemoverHandlingUnitMercadoLivreClick(obj) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, RetornarMensagemRemover(), function () {
        executarReST("CargaIntegracaoMercadoLivre/RemoverHandlingUnit", { Codigo: obj.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaEFacilityRemovidoComSucesso);
                    }
                    else {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.HandlingUnitRemovidoComSucesso);
                    }

                    _gridIntegracaoMercadoLivre.CarregarGrid();
                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ConfirmarProcessamentoHandlingUnitMercadoLivreClick(obj) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteConfirmarProcessamento, function () {
        executarReST("CargaIntegracaoMercadoLivre/ConfirmarProcessamentoHandlingUnit", { Codigo: obj.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Sucesso);
                    }
                    else {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Sucesso);
                    }

                    _gridIntegracaoMercadoLivre.CarregarGrid();
                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function RetornarMensagemRemover() {

    carregarConfiguracaoMercadoLivre();

    if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {

        if (_limparComposicaoCargaRetiradaRotaFacility) 
            return Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverComposicaoCargaRetiradaRotaFacility;
        
        return Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverEstaRotaEFacility;
    }
    else {
        return Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverEsteHandlingUnit;
    }
}

function PesquisarHandlingUnitMercadoLivreClick() {
    _gridIntegracaoMercadoLivre.CarregarGrid();
}

function LoadIntegracaoMercadoLivre(carga, integracoes) {
    _pesquisaIntegracaoMercadoLivre = null;
    _gridIntegracaoMercadoLivre = null;

    let idDivIntegracaoMercadoLivre = "divIntegracaoMercadoLivre_" + carga.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";
    let divIntegracaoMercadoLivre = $("#" + idDivIntegracaoMercadoLivre);

    carregarConfiguracaoMercadoLivre();

    if (carga.CargaTransbordo.val() === true) {
        divIntegracaoMercadoLivre.addClass("d-none");
        return;
    }

    if (integracoes != null && integracoes.length > 0) {
        for (var i = 0; i < integracoes.length; i++) {
            if (integracoes[i] == EnumTipoIntegracao.MercadoLivre) {

                CarregarHTMLIntegracaoMercadoLivre().then(function () {

                    divIntegracaoMercadoLivre.html(_HTMLIntegracaoMercadoLivre);

                    _pesquisaIntegracaoMercadoLivre = new PesquisaIntegracaoMercadoLivre();
                    _pesquisaIntegracaoMercadoLivre.Carga.val(carga.Codigo.val());
                    _pesquisaIntegracaoMercadoLivre.CargaPedido.val(_documentoEmissao.CargaPedido.val());
                    _pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val(carga.TipoIntegracaoMercadoLivre.val());

                    if (_pesquisaIntegracaoMercadoLivre.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                        _pesquisaIntegracaoMercadoLivre.AdicionarHandlingUnit.text(Localization.Resources.Cargas.Carga.AdicionarRotaEFacility);
                    }
                    else {
                        _pesquisaIntegracaoMercadoLivre.AdicionarHandlingUnit.text(Localization.Resources.Cargas.Carga.AdicionarHU);
                    }

                    KoBindings(_pesquisaIntegracaoMercadoLivre, idDivIntegracaoMercadoLivre);

                    divIntegracaoMercadoLivre.removeClass("d-none");

                    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {
                        _pesquisaIntegracaoMercadoLivre.AdicionarHandlingUnit.visible(true);

                        if (carga.TipoIntegracaoMercadoLivre.val() == EnumTipoIntegracaoMercadoLivre.RotaEFacility) {
                            _pesquisaIntegracaoMercadoLivre.HandlingUnitID.visible(false);
                            _pesquisaIntegracaoMercadoLivre.Rota.visible(true);
                            _pesquisaIntegracaoMercadoLivre.Facility.visible(true);
                        }
                        else {
                            _pesquisaIntegracaoMercadoLivre.HandlingUnitID.visible(true);
                            _pesquisaIntegracaoMercadoLivre.Rota.visible(false);
                            _pesquisaIntegracaoMercadoLivre.Facility.visible(false);
                        }
                    } else {
                        let divConteudoEsquerda = $("#divConteudoEsquerda_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

                        divConteudoEsquerda.removeClass("col-lg-6");
                        divConteudoEsquerda.addClass("col-lg-12");

                        _pesquisaIntegracaoMercadoLivre.AdicionarHandlingUnit.visible(false);
                        _pesquisaIntegracaoMercadoLivre.HandlingUnitID.visible(false);
                        _pesquisaIntegracaoMercadoLivre.Rota.visible(false);
                        _pesquisaIntegracaoMercadoLivre.Facility.visible(false);
                    }

                    BuscarIntegracoesMercadoLivre(_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe);

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga))
                        _pesquisaIntegracaoMercadoLivre.HandlingUnitID.enable(false);
                });

                break;
            }
        }
    } else {
        divIntegracaoMercadoLivre.addClass("d-none");
    }

}

function CarregarHTMLIntegracaoMercadoLivre() {
    let p = new promise.Promise();

    if (_HTMLIntegracaoMercadoLivre.length == 0) {
        $.get("Content/Static/Carga/IntegracaoMercadoLivre.html?dyn=" + guid(), function (data) {
            _HTMLIntegracaoMercadoLivre = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function BuscarIntegracoesMercadoLivre(editavel) {
    let removerHandlingUnit = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: RemoverHandlingUnitMercadoLivreClick, tamanho: "20", icone: "" };
    let reprocessarHandlingUnit = { descricao: Localization.Resources.Gerais.Geral.Reprocessar, id: guid(), evento: "onclick", metodo: ReprocessarHandlingUnitMercadoLivreClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReprocessar };
    let detalhesHandlingUnit = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: DetalhesHandlingUnitMercadoLivreClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDetalhes }
    let confirmarProcessamento = { descricao: Localization.Resources.Cargas.Carga.ConfirmarProcessamento, id: guid(), evento: "onclick", metodo: ConfirmarProcessamentoHandlingUnitMercadoLivreClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoConfirmarProcessamento }
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [confirmarProcessamento, removerHandlingUnit, reprocessarHandlingUnit, detalhesHandlingUnit] };

    if (!editavel)
        menuOpcoes = null;

    _gridIntegracaoMercadoLivre = new GridView(_pesquisaIntegracaoMercadoLivre.Grid.id, "CargaIntegracaoMercadoLivre/Pesquisa", _pesquisaIntegracaoMercadoLivre, menuOpcoes, { column: 0, dir: orderDir.desc }, 5);
    _gridIntegracaoMercadoLivre.CarregarGrid();
}

function VisibilidadeOpcaoReprocessar(data) {
    if (data.Situacao == "Falha") {
        return true;
    }
    else {
        return false;
    }
}

function VisibilidadeOpcaoConfirmarProcessamento(data) {
    if (data.Situacao == "Ag. Confirmação") {
        return true;
    }
    else {
        return false;
    }
}

function VisibilidadeOpcaoDetalhes(data) {
    //if (data.Rota != 0) {
        return true;
    //}
    //else {
        //return false;
    //}
}

function HandleHUMercadoLivreAtualizadoSignalR(retorno,) {
    if (_cargaAtual != null && _gridIntegracaoMercadoLivre != null && retorno.CodigoCarga == _cargaAtual.Codigo.val()) {
        executarReST("CargaNotasFiscais/ObterInformacoesGerais", { Carga: _cargaAtual.Codigo.val() }, function (r) {
            if (r.Success) {
                loadInfoDocumentosParaEmissao(_cargaAtual, true);
                LoadIntegracaoMercadoLivre(_cargaAtual, r.Data.Integracoes);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    }
}

function ReprocessarHandlingUnitMercadoLivreClick(obj) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReprocessarEstaRotaEFacility, function () {
        executarReST("CargaIntegracaoMercadoLivre/ReprocessarHandlingUnit", { Codigo: obj.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaEFacilitySeraReprocessadoEmInstantes);
                    _gridIntegracaoMercadoLivre.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function carregarConfiguracaoMercadoLivre() {
    
    executarReST("CargaIntegracaoMercadoLivre/ObterConfiguracaoIntegracaoMercadoLivre", {}, function (r) {
        if (r.Success) {
            _limparComposicaoCargaRetiradaRotaFacility = r.Data.LimparComposicaoCargaRetiradaRotaFacility;
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}