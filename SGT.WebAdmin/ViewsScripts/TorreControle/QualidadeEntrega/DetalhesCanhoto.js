/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />

// #region Variáveis Globais
var _knoutDetalhesCanhoto;
var _gridHistoricoCanhotos;
var _knoutArquivoExterno; 
// #endregion Variáveis Globais

// #region Classes
var DetalhesCanhoto = function () {
    this.NotasAvuso = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.NotasCanhotoAvulso.getFieldDescription ? Localization.Resources.Canhotos.Canhoto.NotasCanhotoAvulso.getFieldDescription() : "Notas Avulso", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Chave = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ChaveDeAcesso.getFieldDescription ? Localization.Resources.Canhotos.Canhoto.ChaveDeAcesso.getFieldDescription() : "Chave de Acesso", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Numero = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Numero.getFieldDescription ? Localization.Resources.Canhotos.Canhoto.Numero.getFieldDescription() : "Número", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ text: "Local de Armazenamento", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.SituacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoCanhoto ? EnumSituacaoCanhoto.Todas : 0), def: EnumSituacaoCanhoto ? EnumSituacaoCanhoto.Todas : 0, visible: ko.observable(true) });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto ? EnumSituacaoDigitalizacaoCanhoto.Todas : 0), def: EnumSituacaoDigitalizacaoCanhoto ? EnumSituacaoDigitalizacaoCanhoto.Todas : 0, visible: ko.observable(true) });
    this.SituacaoPgtoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoPgtoCanhoto ? EnumSituacaoPgtoCanhoto.Todas : 0), def: EnumSituacaoPgtoCanhoto ? EnumSituacaoPgtoCanhoto.Todas : 0, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ text: "Justificativa", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.MotivoRejeicaoDigitalizacao = PropertyEntity({ text: "Motivo de Rejeição", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto ? EnumTipoCanhoto.Todos : 0), def: EnumTipoCanhoto ? EnumTipoCanhoto.Todos : 0, visible: ko.observable(true) });
    this.NaturezaOP = PropertyEntity({ text: "Natureza da Operação", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DescricaoModalidadeFrete = PropertyEntity({ text: "Tipo de Pagamento", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Empresa", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ text: "Emitente", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Latitude = PropertyEntity({ text: ko.observable(" "), val: ko.observable(""), def: "", required: false, visible: ko.observable(false), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(" "), val: ko.observable(""), def: "", required: false, visible: ko.observable(false), maxlength: 20 });
    this.ObservacaoRecebimentoFisico = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataRecebimento = PropertyEntity({ text: "Data de Recebimento", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroProtocolo = PropertyEntity({ text: "Nº Protocolo", val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(false) });
};
// #endregion Classes

// #region Funções Internas
function loadDetalhesCanhoto() {
    var htmlHistorico =
        `<div id="KnoutDetalhesCanhoto">        
        <div class="row mt-4">
            <div class="col-12">
                <table class="table table-bordered" data-bind="attr: {id: Chave.idGrid}"></table>
            </div>
        </div>
    </div>`;

    $("#divDetalhesCanhoto").html(htmlHistorico);

    _knoutDetalhesCanhoto = new DetalhesCanhoto();
    KoBindings(_knoutDetalhesCanhoto, "KnoutDetalhesCanhoto");

    if (typeof HeaderAuditoria === 'function') {
        HeaderAuditoria("Canhoto", _knoutDetalhesCanhoto, "Numero");
    }

    if (_knoutDetalhesCanhoto && _knoutDetalhesCanhoto.Chave && _knoutDetalhesCanhoto.Chave.idGrid) {
        _gridHistoricoCanhotos = new GridView(_knoutDetalhesCanhoto.Chave.idGrid, "Canhoto/ConsultarHistoricoCanhoto", _knoutArquivoExterno, null);
    }
}

function BuscarDetalhesCanhoto(codigo, callback) {
    if (!_knoutDetalhesCanhoto) {
        loadDetalhesCanhoto();
    }

    var dados = {
        Codigo: codigo
    };

    executarReST("Canhoto/BuscarDetalhesCanhoto", dados, function (arg) {
        if (arg.Success) {
            try {
                var retorno = { Data: arg.Data };

                if (_knoutDetalhesCanhoto) {
                    PreencherObjetoKnout(_knoutDetalhesCanhoto, retorno);

                    if (_knoutArquivoExterno && _knoutArquivoExterno.Codigo) {
                        _knoutArquivoExterno.Codigo.val(codigo);
                    }

                    if (_knoutDetalhesCanhoto.LocalArmazenamento) {
                        if (!arg.Data.LocalArmazenamento) {
                            _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
                        } else {
                            var strLocal = arg.Data.LocalArmazenamento;
                            _knoutDetalhesCanhoto.LocalArmazenamento.visible(true);
                            if (arg.Data.PacoteArmazenado > 0 && Localization.Resources.Canhotos.Canhoto.NoPacoteNaPosicao) {
                                strLocal += " " + Localization.Resources.Canhotos.Canhoto.NoPacoteNaPosicao.format(arg.Data.PacoteArmazenado, arg.Data.PosicaoNoPacote);
                            }
                            _knoutDetalhesCanhoto.LocalArmazenamento.val(strLocal);
                        }
                    }

                    if (_knoutDetalhesCanhoto.SituacaoCanhoto && _knoutDetalhesCanhoto.Justificativa) {
                        if (_knoutDetalhesCanhoto.SituacaoCanhoto.val() == EnumSituacaoCanhoto.Justificado) {
                            _knoutDetalhesCanhoto.Justificativa.visible(true);
                        }
                    }

                    if (_knoutDetalhesCanhoto.SituacaoDigitalizacaoCanhoto && _knoutDetalhesCanhoto.MotivoRejeicaoDigitalizacao) {
                        if (_knoutDetalhesCanhoto.SituacaoDigitalizacaoCanhoto.val() == EnumSituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada) {
                            _knoutDetalhesCanhoto.MotivoRejeicaoDigitalizacao.visible(true);
                        }
                    }

                    if (_knoutDetalhesCanhoto.TipoCanhoto) {
                        if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.NFe) {
                            if (_knoutDetalhesCanhoto.Chave) _knoutDetalhesCanhoto.Chave.visible(true);
                            if (_knoutDetalhesCanhoto.NotasAvuso) _knoutDetalhesCanhoto.NotasAvuso.visible(false);
                            if (_knoutDetalhesCanhoto.NaturezaOP) _knoutDetalhesCanhoto.NaturezaOP.visible(true);
                        } else if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.Avulso) {
                            if (_knoutDetalhesCanhoto.Chave) _knoutDetalhesCanhoto.Chave.visible(false);
                            if (_knoutDetalhesCanhoto.NotasAvuso) _knoutDetalhesCanhoto.NotasAvuso.visible(true);
                        }
                    }

                    if (typeof _CONFIGURACAO_TMS !== 'undefined') {
                        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                            if (_knoutDetalhesCanhoto.LocalArmazenamento) _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
                            if (_knoutDetalhesCanhoto.Empresa) _knoutDetalhesCanhoto.Empresa.visible(false);
                        }

                        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                            if (_knoutDetalhesCanhoto.Empresa) _knoutDetalhesCanhoto.Empresa.visible(false);
                            if (_knoutDetalhesCanhoto.Filial) _knoutDetalhesCanhoto.Filial.visible(false);
                        }
                    }

                    if (_knoutDetalhesCanhoto.Latitude && _knoutDetalhesCanhoto.Longitude) {
                        if (_knoutDetalhesCanhoto.Latitude.val() != "" &&
                            _knoutDetalhesCanhoto.Longitude.val() != "" &&
                            _knoutDetalhesCanhoto.Latitude.val() != null &&
                            _knoutDetalhesCanhoto.Longitude.val() != null) {

                            if (typeof setarCoordenadasCanhoto === 'function') {
                                setarCoordenadasCanhoto();
                            }
                            $("#li_geoLocalizacao").show();
                        } else {
                            $("#li_geoLocalizacao").hide();
                        }
                    }

                    if (_knoutDetalhesCanhoto.ObservacaoRecebimentoFisico && typeof string !== 'undefined' && typeof string.IsNullOrWhiteSpace === 'function') {
                        _knoutDetalhesCanhoto.ObservacaoRecebimentoFisico.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.ObservacaoRecebimentoFisico.val()));
                    }

                    if (_knoutDetalhesCanhoto.DataRecebimento && typeof string !== 'undefined' && typeof string.IsNullOrWhiteSpace === 'function') {
                        _knoutDetalhesCanhoto.DataRecebimento.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.DataRecebimento.val()));
                    }

                    if (_knoutDetalhesCanhoto.NumeroProtocolo) {
                        _knoutDetalhesCanhoto.NumeroProtocolo.visible(_knoutDetalhesCanhoto.NumeroProtocolo.val() > 0);
                    }

                    if (_knoutDetalhesCanhoto.Observacao && typeof string !== 'undefined' && typeof string.IsNullOrWhiteSpace === 'function') {
                        _knoutDetalhesCanhoto.Observacao.visible(!string.IsNullOrWhiteSpace(_knoutDetalhesCanhoto.Observacao.val()));
                    }

                    if (_knoutDetalhesCanhoto.Chave && _knoutDetalhesCanhoto.Chave.idGrid) {
                        if (!_gridHistoricoCanhotos) {
                            _gridHistoricoCanhotos = new GridView(_knoutDetalhesCanhoto.Chave.idGrid, "Canhoto/ConsultarHistoricoCanhoto", _knoutArquivoExterno, null);
                        }

                        if (_gridHistoricoCanhotos && typeof _gridHistoricoCanhotos.CarregarGrid === 'function') {
                            _gridHistoricoCanhotos.CarregarGrid(function () {
                                if (callback != null) {
                                    callback();
                                }
                            });
                        } else if (callback != null) {
                            callback();
                        }
                    } else if (callback != null) {
                        callback();
                    }
                } else if (callback != null) {
                    callback();
                }
            } catch (e) {
                console.error("Erro ao processar os dados do canhoto:", e);
                if (callback != null) {
                    callback();
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            if (callback != null) {
                callback();
            }
        }
    });
}
// #endregion Funções Internas

// #region Inicialização do Módulo
(function () {
    window.DetalhesCanhotoModulo = {
        inicializar: function (knoutArquivo) {
            _knoutArquivoExterno = knoutArquivo;
        },
        detalhesCanhotoClick: function (e) {
            if (_knoutArquivoExterno) {
                _knoutArquivoExterno.Codigo.val(e.CodigoCanhoto);
                if (!_knoutDetalhesCanhoto) {
                    loadDetalhesCanhoto();
                }
                BuscarDetalhesCanhoto(e.CodigoCanhoto, function () {
                    Global.abrirModal('ModalDivDetalhesCanhoto');
                });
            }
        },
        buscarDetalhesCanhoto: BuscarDetalhesCanhoto
    };
})();
// #endregion Inicialização do Módulo