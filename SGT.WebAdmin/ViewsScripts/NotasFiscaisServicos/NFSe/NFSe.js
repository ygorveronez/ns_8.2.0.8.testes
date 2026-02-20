/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/SerieEmpresa.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Servico.js" />
/// <reference path="Substituicao.js" />
/// <reference path="ListaServico.js" />
/// <reference path="Valor.js" />
/// <reference path="Parcelamento.js" />
/// <reference path="../../Consultas/NaturezaNFSe.js" />

var _HTMLEmissaoNFSe = "";

var PrincipalNFSe = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.CodigoPedidoVenda = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, text: "*Número:", enable: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Emissão:", required: true, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa (Tomador):", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.CidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cidade Prestação do Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 5000, enable: ko.observable(true) });
};

var CRUDNFSe = function () {
    this.Emitir = PropertyEntity({ type: types.event, text: "Autorizar Emissão", visible: ko.observable(true), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

function EmissaoNFSe(codigoNFSe, callbackInit, permissoes, codigosPedidoVenda) {

    var emissaoNFSe = this;

    this.LoadEmissaoNFSe = function () {
        emissaoNFSe.IdModal = (guid());
        emissaoNFSe.IdKnockoutNFSe = "knockoutNFSe_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutCRUDNFSe = "knockoutCRUDNFSe_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutListaServico = "knockoutListaServico_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutServico = "knockoutServico_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutValor = "knockoutValor_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutSubstituicao = "knockoutSubstituicao_" + emissaoNFSe.IdModal;
        emissaoNFSe.IdKnockoutParcelamento = "knockoutParcelamento_" + emissaoNFSe.IdModal;

        emissaoNFSe.NFSe = new PrincipalNFSe();
        emissaoNFSe.NFSe.IdModal.val(emissaoNFSe.IdModal);
        emissaoNFSe.NFSe.DataEmissao.val(moment().format("DD/MM/YYYY HH:mm"));
        emissaoNFSe.ListaServico = new ListaServico(emissaoNFSe);
        emissaoNFSe.Servico = new Servico(emissaoNFSe, emissaoNFSe.ListaServico);
        emissaoNFSe.Valor = new Valor(emissaoNFSe);
        emissaoNFSe.Substituicao = new Substituicao(emissaoNFSe);
        emissaoNFSe.DetalheParcela = new DetalheParcela(emissaoNFSe);
        emissaoNFSe.Parcelamento = new Parcelamento(emissaoNFSe, emissaoNFSe.Valor, emissaoNFSe.DetalheParcela);

        emissaoNFSe.CRUDNFSe = new CRUDNFSe();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            emissaoNFSe.NFSe.Empresa.visible(true);
            emissaoNFSe.NFSe.Empresa.required(true);
        }

        if (codigoNFSe != null && codigoNFSe > 0)
            emissaoNFSe.NFSe.Codigo.val(codigoNFSe);

        executarReST("NotaFiscalServico/BuscarDadosEmpresa", null, function (r) {
            if (r.Success) {
                if (codigoNFSe != null && codigoNFSe > 0)
                    emissaoNFSe.NFSe.Codigo.val(codigoNFSe);
                else {
                    if (r.Data.Serie != null) {
                        emissaoNFSe.NFSe.Serie.codEntity(r.Data.Serie.Codigo);
                        emissaoNFSe.NFSe.Serie.val(r.Data.Serie.Descricao);
                    }
                    if (r.Data.Empresa != null) {
                        emissaoNFSe.NFSe.Empresa.codEntity(r.Data.Empresa.Codigo);
                        emissaoNFSe.NFSe.Empresa.val(r.Data.Empresa.Descricao);
                    }
                    if (r.Data.CidadeEmpresa != null) {
                        emissaoNFSe.NFSe.CidadePrestacao.codEntity(r.Data.CidadeEmpresa.Codigo);
                        emissaoNFSe.NFSe.CidadePrestacao.val(r.Data.CidadeEmpresa.Descricao);
                        emissaoNFSe.Servico.LocalidadeServico.codEntity(r.Data.CidadeEmpresa.Codigo);
                        emissaoNFSe.Servico.LocalidadeServico.val(r.Data.CidadeEmpresa.Descricao);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });

        emissaoNFSe.RenderizarModalNFSe(emissaoNFSe, callbackInit);
    }

    this.SetarPermissoes = function () {

        if (permissoes != null) {
            //Desenvolver
        }
    };

    this.DestivarNFSe = function () {
        DesabilitarCamposInstanciasNFSe(emissaoNFSe.NFSe);
    }

    this.DestivarCRUDNFSe = function () {
        DesabilitarCamposInstanciasNFSe(emissaoNFSe.CRUDNFSe);
    }

    this.BuscarDadosNFSe = function () {
        executarReST("NotaFiscalServico/BuscarPorCodigo", { Codigo: emissaoNFSe.NFSe.Codigo.val() }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoNFSe.NFSe, { Data: r.Data.NFSe });

                if (r.Data.Valor != null)
                    PreencherObjetoKnout(emissaoNFSe.Valor, { Data: r.Data.Valor });
                if (r.Data.Substituicao != null)
                    PreencherObjetoKnout(emissaoNFSe.Substituicao, { Data: r.Data.Substituicao });

                emissaoNFSe.Servicos = new Array();
                if (r.Data.Servicos != null)
                    emissaoNFSe.Servicos = r.Data.Servicos;
                emissaoNFSe.ListaServico.RecarregarGrid();

                emissaoNFSe.Parcelas = new Array();
                if (r.Data.Parcelas != null)
                    emissaoNFSe.Parcelas = r.Data.Parcelas;
                emissaoNFSe.Parcelamento.RecarregarGrid();

                emissaoNFSe.SetarPermissoes();

                emissaoNFSe.NFSe.Serie.enable(false);
                emissaoNFSe.NFSe.Empresa.enable(false);
                emissaoNFSe.Servico.LocalidadeServico.codEntity(r.Data.NFSe.CidadePrestacao.Codigo);
                emissaoNFSe.Servico.LocalidadeServico.val(r.Data.NFSe.CidadePrestacao.Descricao);

                if (callbackInit != null)
                    callbackInit();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    }


    this.CarregarDadosPedido = function () {
        executarReST("NotaFiscalServico/CarregarPedidoVendaPorCodigo", { Codigos: JSON.stringify(codigosPedidoVenda) }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoNFSe.NFSe, { Data: r.Data.NFSe });
                emissaoNFSe.NFSe.CodigoPedidoVenda.val(JSON.stringify(codigosPedidoVenda));

                if (r.Data.Observacao != null)
                    emissaoNFSe.NFSe.Observacao.val(r.Data.Observacao.ObservacaoNFSe);

                var totalizadorValorTotalServicos = 0;
                var totalizadorBCISS = 0;
                var totalizadorAliquotaISS = 0;
                var totalizadorValorISS = 0;
                emissaoNFSe.Servicos = new Array();
                if (r.Data.Servicos != null) {
                    for (var i = 0; i < r.Data.Servicos.length; i++) {
                        var item = r.Data.Servicos[i];

                        totalizadorValorTotalServicos += Globalize.parseFloat(item.ValorTotal);
                        totalizadorBCISS += Globalize.parseFloat(item.BCISS);
                        totalizadorAliquotaISS = Globalize.parseFloat(item.AliquotaISS);
                        totalizadorValorISS += Globalize.parseFloat(item.ValorISS);

                        emissaoNFSe.Servicos[i] = item;
                    }
                }
                emissaoNFSe.ListaServico.RecarregarGrid();

                emissaoNFSe.Valor.ValorTotalServicos.val(Globalize.format(totalizadorValorTotalServicos, "n2"));
                emissaoNFSe.Valor.ValorTotalLiquido.val(Globalize.format(totalizadorValorTotalServicos, "n2"));
                emissaoNFSe.Valor.BaseISS.val(Globalize.format(totalizadorBCISS, "n2"));
                emissaoNFSe.Valor.AliquotaISS.val(Globalize.format(totalizadorAliquotaISS, "n2"));
                emissaoNFSe.Valor.ValorISS.val(Globalize.format(totalizadorValorISS, "n2"));

                emissaoNFSe.Parcelas = new Array();
                if (r.Data.Parcelas != null)
                    emissaoNFSe.Parcelas = r.Data.Parcelas;
                emissaoNFSe.Parcelamento.RecarregarGrid();

                emissaoNFSe.SetarPermissoes();

                if (callbackInit != null)
                    callbackInit();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    }

    this.FecharModal = function () {
        emissaoNFSe.ModalNFSe.hide();
    }

    this.Destroy = function () {
        emissaoNFSe.ModalNFSe.dispose();
        $("#" + emissaoNFSe.IdModal).remove();
        emissaoNFSe = null;
    }

    this.RenderizarModalNFSe = function () {
        emissaoNFSe.ObterHTMLEmissaoNFSe().then(function () {
            var html = _HTMLEmissaoNFSe.replace(/#divModalNFSe/g, emissaoNFSe.IdModal);
            $('#js-page-content').append(html);
            emissaoNFSe.CRUDNFSe.Cancelar.eventClick = function (e) {
                emissaoNFSe.FecharModal();
            };

            KoBindings(emissaoNFSe.NFSe, emissaoNFSe.IdKnockoutNFSe);
            KoBindings(emissaoNFSe.CRUDNFSe, emissaoNFSe.IdKnockoutCRUDNFSe);

            new BuscarClientes(emissaoNFSe.NFSe.Pessoa, function (data) {
                emissaoNFSe.NFSe.Pessoa.codEntity(data.Codigo);
                emissaoNFSe.NFSe.Pessoa.val(data.Nome + " (" + data.Localidade + ")");
            }, false);
            new BuscarLocalidadesBrasil(emissaoNFSe.NFSe.CidadePrestacao, null, null, function (data) {
                emissaoNFSe.NFSe.CidadePrestacao.codEntity(data.Codigo);
                emissaoNFSe.NFSe.CidadePrestacao.val(data.Descricao);

                emissaoNFSe.Servico.LocalidadeServico.codEntity(data.Codigo);
                emissaoNFSe.Servico.LocalidadeServico.val(data.Descricao);
            });
            new BuscarNaturezaNFSe(emissaoNFSe.NFSe.NaturezaOperacao, null, null, emissaoNFSe.NFSe.Pessoa, null, "S");
            new BuscarSerieEmpresa(emissaoNFSe.NFSe.Serie, null, null, null, emissaoNFSe.NFSe.Empresa, 2);
            new BuscarEmpresa(emissaoNFSe.NFSe.Empresa, function (data) {
                emissaoNFSe.NFSe.Empresa.codEntity(data.Codigo);
                emissaoNFSe.NFSe.Empresa.val(data.RazaoSocial);
            }, null);

            if (!emissaoNFSe.ModalNFSe)
                emissaoNFSe.ModalNFSe = new bootstrap.Modal(document.getElementById(emissaoNFSe.IdModal), { backdrop: true, keyboard: true });
            
            emissaoNFSe.ModalNFSe.show();

            $('#' + emissaoNFSe.IdModal).on('hidden.bs.modal', function () {
                emissaoNFSe.Destroy();
            });

            emissaoNFSe.Servico.Load();
            emissaoNFSe.ListaServico.Load();
            emissaoNFSe.Valor.Load();
            emissaoNFSe.Substituicao.Load();
            emissaoNFSe.DetalheParcela.Load();
            emissaoNFSe.Parcelamento.Load();

            if (emissaoNFSe.NFSe.Codigo.val() != "" && emissaoNFSe.NFSe.Codigo.val() > 0) {
                emissaoNFSe.BuscarDadosNFSe();
            } else if (codigosPedidoVenda != null && codigosPedidoVenda.length > 0) {
                emissaoNFSe.CarregarDadosPedido();
            } else if (callbackInit != null) {
                emissaoNFSe.SetarPermissoes();
                callbackInit();
            }

        });

    }

    this.ObterHTMLEmissaoNFSe = function () {
        var p = new promise.Promise();
        if (_HTMLEmissaoNFSe == "") {
            $.get("Content/Static/NotaFiscalServico/NotaFiscalServico.html?dyn=" + emissaoNFSe.IdModal, function (data) {
                _HTMLEmissaoNFSe = data;
                p.done();
            });
        } else {
            p.done();
        }
        return p;
    }

    this.VerificarSePossuiPermissao = function (permissao) {
        var existe = false;
        $.each(permissoes, function (i, permissaoDaLista) {
            if (permissao == permissaoDaLista) {
                existe = true;
                return false;
            }
        });
        return existe;
    }

    setTimeout(function () {
        emissaoNFSe.LoadEmissaoNFSe();
    }, 50);
}

function DesabilitarCamposInstanciasNFSe(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCamposInstanciasNFSe(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}

function ObterObjetoNFSe(emissaoNFSe) {
    var nfse = new Object();
    nfse.NFSe = RetornarObjetoPesquisa(emissaoNFSe.NFSe);
    nfse.Servicos = emissaoNFSe.Servicos;
    nfse.Valor = RetornarObjetoPesquisa(emissaoNFSe.Valor);
    nfse.Substituicao = RetornarObjetoPesquisa(emissaoNFSe.Substituicao);
    nfse.Parcelas = emissaoNFSe.Parcelas;

    return JSON.stringify(nfse);
}