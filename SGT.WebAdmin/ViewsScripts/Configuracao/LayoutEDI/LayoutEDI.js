/// <reference path="../../../js/plugin/promise/promise.js" />
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
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />

var _tipoIntegracao;

var ConfiguracaoLayoutEDIModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ type: types.string, val: ko.observable(0), def: 0, visible: ko.observable(false) });

    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.LayoutEDI.DescricaoLayoutEDI.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(_tipoIntegracao[0].value), options: _tipoIntegracao, text: Localization.Resources.Configuracao.LayoutEDI.Integracao.getFieldDescription(), def: _tipoIntegracao[0].value, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true), issue: 267 });

    this.EnderecoFTP = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.Endereco.getRequiredFieldDescription(), maxlength: 150, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.Usuario.getRequiredFieldDescription(), maxlength: 50, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.LayoutEDI.Senha.getRequiredFieldDescription()), maxlength: 50, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Porta = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.Porta.getRequiredFieldDescription(), maxlength: 10, def: "21", val: ko.observable("21"), enable: ko.observable(false), visible: ko.observable(true), required: ko.observable(false) });
    this.Diretorio = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.Diretorio.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Passivo = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.FTPPassivo, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.SSL = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.SSL, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.UtilizarSFTP = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.SFTP, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.UtilizarLeituraArquivos = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.LerArquivosDesteFTP, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.AdicionarEDIFilaProcessamento = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.AdicionarEDIEmFilaDeProcessamento, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.CriarComNomeTemporaraio = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.CriarArquivoComNomeTemporarioDepoisDoEnvioConcluidoSeraRenomeadoParaNomeOficial, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.CertificadoChavePrivada = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.CertificadoChavePrivada, type: types.file, codEntity: ko.observable(0), val: ko.observable(""), accept: ".txt,.ppk", required: ko.observable(false), visible: ko.observable(false) });
    this.CertificadoChavePrivadaBase64 = PropertyEntity({ text: ko.observable(""), type: types.string, codEntity: ko.observable(0), val: ko.observable(""), visible: ko.observable(false) });
    this.NomeArquivo = PropertyEntity({ text: ko.observable("Selecione um certificado"), val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(false) });

    this.UtilizarSFTP.val.subscribe(function (novoValor) {
        instancia.Configuracao.Senha.required(true);
        instancia.Configuracao.Senha.text(Localization.Resources.Configuracao.LayoutEDI.Senha.getRequiredFieldDescription());
        if (novoValor) {
            instancia.Configuracao.Senha.required(false);
            instancia.Configuracao.Senha.text("Senha");
        }

    })

    this.CertificadoChavePrivada.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor == '')
            return;

        var nomeArquivo = novoValor.replace("C:\\fakepath\\", "");

        instancia.Configuracao.NomeArquivo.text(nomeArquivo);
        instancia.Configuracao.NomeArquivo.val(nomeArquivo);

        var arquivo = document.getElementById(instancia.Configuracao.CertificadoChavePrivada.id);

        if (arquivo.files.length > 0)
            getBase64File(instancia.Configuracao.CertificadoChavePrivadaBase64, arquivo.files[0]);
    });

    this.Emails = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.Emails.getRequiredFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.EmailsAlertaLeituraEDI = PropertyEntity({ text: Localization.Resources.Configuracao.LayoutEDI.EmailsDeAlertaDeLeituraDeEDI.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });

    this.TestarConexaoFTP = PropertyEntity({ eventClick: function () { instancia.TestarConexaoFTP(); }, type: types.event, text: Localization.Resources.Configuracao.LayoutEDI.TestarConexao, visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: instancia.AdicionarLayoutEDI, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.AtualizarLayoutEDI, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.ExcluirLayoutEDI, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.CancelarLayoutEDI, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.TipoIntegracao.val.subscribe(function (novoValor) {
        instancia.ChangeTipoIntegracao(novoValor);
    });
};

function ConfiguracaoLayoutEDI(idContent, knockoutConfiguracao, fnCallback) {
    var instancia = this;    
    this.RetornarValores = function (valores) {
        instancia.KnockoutConfiguracao.val(JSON.stringify(valores));
    };

    this.SetarValores = function (valores) {
        instancia.RetornarValores(valores);
        instancia.RecarregarGrid(valores);
    };

    this.Limpar = function () {
        instancia.LimparCampos();

        instancia.KnockoutConfiguracao.val(JSON.stringify(new Array()));

        instancia.RecarregarGrid(new Array());
        instancia.LimparCampos();
    };

    this.LimparCampos = function () {
        LimparCampos(instancia.Configuracao);

        instancia.Configuracao.CertificadoChavePrivada.val("");
        instancia.Configuracao.CertificadoChavePrivadaBase64.val("");

        $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").addClass("hidden");
        $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").addClass("hidden");

        instancia.Configuracao.Adicionar.visible(true);
        instancia.Configuracao.Atualizar.visible(false);
        instancia.Configuracao.Excluir.visible(false);
        instancia.Configuracao.Cancelar.visible(false);
    };

    this.RecarregarGrid = function (valores) {
        instancia.GridLayoutEDI.CarregarGrid(valores);
    };

    this.Load = function () {
        var p = new promise.Promise();

        LoadLocalizationResources("Configuracao.LayoutEDI").then(function () {

            instancia.KnockoutConfiguracao = knockoutConfiguracao;
            instancia.Configuracao = new ConfiguracaoLayoutEDIModel(instancia);
            instancia.RegistrosSelecionados = new Array();
            instancia.GridLayoutEDI = null;

            $.get("Content/Static/Configuracao/LayoutEDI.html?dyn=" + guid(), function (data) {
                $("#" + idContent).html(data);

                KoBindings(instancia.Configuracao, idContent);

                LocalizeCurrentPage();

                var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: instancia.EditarLayoutEDI }] };

                var header = [{ data: "Codigo", visible: false },
                { data: "DescricaoLayoutEDI", title: Localization.Resources.Configuracao.LayoutEDI.DescricaoLayoutEDI, width: "50%" },
                { data: "DescricaoTipoIntegracao", title: Localization.Resources.Configuracao.LayoutEDI.Integracao, width: "30%" }];

                instancia.GridLayoutEDI = new BasicDataTable(instancia.Configuracao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

                new BuscarLayoutsEDI(instancia.Configuracao.LayoutEDI);

                instancia.RecarregarGrid(new Array());
                instancia.RetornarValores(new Array());

                p.done();

                if (fnCallback != null)
                    fnCallback();
            });

            return p;
        });
    };

    this.ObterTiposIntegracao = function () {
        var p = new promise.Promise();

        executarReST("TipoIntegracao/BuscarTodos", {
            Tipos: JSON.stringify([
                EnumTipoIntegracao.NaoPossuiIntegracao,
                EnumTipoIntegracao.FTP,
                EnumTipoIntegracao.Email,
                EnumTipoIntegracao.Michelin
            ])
        }, function (r) {
            if (r.Success) {
                _tipoIntegracao = new Array();

                for (var i = 0; i < r.Data.length; i++)
                    _tipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }

            p.done();
        });

        return p;
    };

    this.ChangeTipoIntegracao = function (tipo) {
        if (tipo == EnumTipoIntegracao.Email) {

            instancia.Configuracao.Emails.visible(true);
            instancia.Configuracao.Emails.required(true);
            instancia.Configuracao.EmailsAlertaLeituraEDI.visible(false);

            instancia.Configuracao.EnderecoFTP.visible(false);
            instancia.Configuracao.Usuario.visible(false);
            instancia.Configuracao.Senha.visible(false);
            instancia.Configuracao.Porta.visible(false);
            instancia.Configuracao.Diretorio.visible(false);
            instancia.Configuracao.Passivo.visible(false);
            instancia.Configuracao.UtilizarSFTP.visible(false);
            instancia.Configuracao.CertificadoChavePrivada.visible(false);
            instancia.Configuracao.NomeArquivo.visible(false)
            instancia.Configuracao.SSL.visible(false);
            instancia.Configuracao.UtilizarLeituraArquivos.visible(false);
            instancia.Configuracao.AdicionarEDIFilaProcessamento.visible(false);
            instancia.Configuracao.TestarConexaoFTP.visible(false);

            instancia.Configuracao.EnderecoFTP.required(false);
            instancia.Configuracao.Porta.required(false);

        } else if (tipo == EnumTipoIntegracao.FTP) {

            instancia.Configuracao.EnderecoFTP.visible(true);
            instancia.Configuracao.Usuario.visible(true);
            instancia.Configuracao.Senha.visible(true);
            instancia.Configuracao.Porta.visible(true);
            instancia.Configuracao.Diretorio.visible(true);
            instancia.Configuracao.Passivo.visible(true);
            instancia.Configuracao.UtilizarSFTP.visible(true);
            instancia.Configuracao.SSL.visible(true);
            instancia.Configuracao.TestarConexaoFTP.visible(true);
            instancia.Configuracao.CertificadoChavePrivada.visible(true);
            instancia.Configuracao.NomeArquivo.visible(true)

            instancia.Configuracao.UtilizarLeituraArquivos.visible(true);
            instancia.Configuracao.AdicionarEDIFilaProcessamento.visible(false);
            instancia.Configuracao.EnderecoFTP.required(true);
            instancia.Configuracao.Porta.required(true);

            instancia.Configuracao.Emails.visible(false);
            instancia.Configuracao.Emails.required(false);
            instancia.Configuracao.EmailsAlertaLeituraEDI.visible(true);

        } else {

            instancia.Configuracao.Emails.visible(false);
            instancia.Configuracao.EmailsAlertaLeituraEDI.visible(false);

            instancia.Configuracao.EnderecoFTP.visible(false);
            instancia.Configuracao.Usuario.visible(false);
            instancia.Configuracao.Senha.visible(false);
            instancia.Configuracao.Porta.visible(false);
            instancia.Configuracao.Diretorio.visible(false);
            instancia.Configuracao.Passivo.visible(false);
            instancia.Configuracao.TestarConexaoFTP.visible(false);
            instancia.Configuracao.UtilizarSFTP.visible(false);
            instancia.Configuracao.SSL.visible(false);
            instancia.Configuracao.UtilizarLeituraArquivos.visible(false);
            instancia.Configuracao.AdicionarEDIFilaProcessamento.visible(false);
            instancia.Configuracao.EnderecoFTP.required(false);
            instancia.Configuracao.Porta.required(false);
            instancia.Configuracao.Emails.required(false);
            instancia.Configuracao.CertificadoChavePrivada.visible(false);
            instancia.Configuracao.NomeArquivo.visible(false)

        }
    };

    this.TestarConexaoFTP = function () {
        executarReST("FTP/TestarConexao", {
            Host: instancia.Configuracao.EnderecoFTP.val(),
            Porta: instancia.Configuracao.Porta.val(),
            Diretorio: instancia.Configuracao.Diretorio.val(),
            Usuario: instancia.Configuracao.Usuario.val(),
            Senha: instancia.Configuracao.Senha.val(),
            Passivo: instancia.Configuracao.Passivo.val(),
            UtilizarSFTP: instancia.Configuracao.UtilizarSFTP.val(),
            SSL: instancia.Configuracao.SSL.val(),
            CertificadoChavePrivadaBase64: instancia.Configuracao.CertificadoChavePrivadaBase64.val()
        }, function (r) {
            if (r.Success) {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").removeClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").addClass("hidden");
            } else {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").addClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").removeClass("hidden");
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

    this.AdicionarLayoutEDI = function () {
        var valido = ValidarCamposObrigatorios(instancia.Configuracao);

        var layoutsEDI = instancia.KnockoutConfiguracao.val() != "" ? JSON.parse(instancia.KnockoutConfiguracao.val()) : new Array();

        if (valido) {

            if (instancia.Configuracao.TipoIntegracao.val() == EnumTipoIntegracao.Email) {
                var emails = instancia.Configuracao.Emails.val().split(";");
                for (var i = 0; i < emails.length; i++) {
                    if (ValidarEmail(emails[i].trim()) === false) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.EmailInvalido, Localization.Resources.Configuracao.LayoutEDI.EmailEstaInvalido.format(emails[i]));
                        return;
                    }
                }
            }
            else if (instancia.Configuracao.TipoIntegracao.val() == EnumTipoIntegracao.FTP) {
                if (instancia.Configuracao.UtilizarSFTP.val() && (instancia.Configuracao.CertificadoChavePrivadaBase64.val().length + instancia.Configuracao.Senha.val().length == 0)) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.SenhaECertificadoVazios, Localization.Resources.Configuracao.LayoutEDI.SenhaOuCertificadoObrigatorio);
                    return;
                }
            }

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
                for (var i = 0; i < layoutsEDI.length; i++) {
                    if (instancia.Configuracao.LayoutEDI.codEntity() == layoutsEDI[i].CodigoLayoutEDI) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.LayoutEDIJaCadastrado, Localization.Resources.Configuracao.LayoutEDI.EsteLayoutEDIJaFoiCadastrado);
                        return;
                    }
                }
            }

            layoutsEDI.push({
                Codigo: guid(),
                CodigoLayoutEDI: instancia.Configuracao.LayoutEDI.codEntity(),
                DescricaoLayoutEDI: instancia.Configuracao.LayoutEDI.val(),
                TipoIntegracao: instancia.Configuracao.TipoIntegracao.val(),
                DescricaoTipoIntegracao: $("#" + instancia.Configuracao.TipoIntegracao.id + " option:selected").text(),
                EnderecoFTP: instancia.Configuracao.EnderecoFTP.val(),
                Usuario: instancia.Configuracao.Usuario.val(),
                Senha: instancia.Configuracao.Senha.val(),
                Porta: instancia.Configuracao.Porta.val(),
                Diretorio: instancia.Configuracao.Diretorio.val(),
                Passivo: instancia.Configuracao.Passivo.val(),
                UtilizarSFTP: instancia.Configuracao.UtilizarSFTP.val(),
                CertificadoChavePrivada: instancia.Configuracao.CertificadoChavePrivada.val(),
                CertificadoChavePrivadaBase64: instancia.Configuracao.CertificadoChavePrivadaBase64.val(),
                SSL: instancia.Configuracao.SSL.val(),
                UtilizarLeituraArquivos: instancia.Configuracao.UtilizarLeituraArquivos.val(),
                AdicionarEDIFilaProcessamento: instancia.Configuracao.AdicionarEDIFilaProcessamento.val(),
                CriarComNomeTemporaraio: instancia.Configuracao.CriarComNomeTemporaraio.val(),
                Emails: instancia.Configuracao.Emails.val(),
                EmailsAlertaLeituraEDI: instancia.Configuracao.EmailsAlertaLeituraEDI.val()
            });

            instancia.KnockoutConfiguracao.val(JSON.stringify(layoutsEDI));

            instancia.RecarregarGrid(layoutsEDI);
            instancia.LimparCampos();

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.AtualizarLayoutEDI = function () {
        var valido = ValidarCamposObrigatorios(instancia.Configuracao);

        var layoutsEDI = instancia.KnockoutConfiguracao.val() != "" ? JSON.parse(instancia.KnockoutConfiguracao.val()) : new Array();        
        if (valido) {

            if (instancia.Configuracao.TipoIntegracao.val() == EnumTipoIntegracao.Email) {
                var emails = instancia.Configuracao.Emails.val().split(";");
                for (var i = 0; i < emails.length; i++) {
                    if (ValidarEmail(emails[i].trim()) === false) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.EmailInvalido, Localization.Resources.Configuracao.LayoutEDI.EmailEstaInvalido.format(emails[i]));
                        return;
                    }
                }
            }
            else if (instancia.Configuracao.TipoIntegracao.val() == EnumTipoIntegracao.FTP) {              
               
                if (instancia.Configuracao.UtilizarSFTP.val() && ((instancia.Configuracao.CertificadoChavePrivadaBase64.val()?.length ?? 0) + (instancia.Configuracao.Senha.val()?.length ?? 0) === 0))
                {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.SenhaECertificadoVazios, Localization.Resources.Configuracao.LayoutEDI.SenhaOuCertificadoObrigatorio);
                    return;
                }
            }

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
                for (var i = 0; i < layoutsEDI.length; i++) {
                    if (instancia.Configuracao.Codigo.val() != layoutsEDI[i].Codigo && instancia.Configuracao.LayoutEDI.codEntity() == layoutsEDI[i].CodigoLayoutEDI) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.LayoutEDI.LayoutEDIJaCadastrado, Localization.Resources.Configuracao.LayoutEDI.EsteLayoutEDIJaFoiCadastrado);
                        return;
                    }
                }
            }
            
            for (var i = 0; i <= layoutsEDI.length; i++) {                
                if (instancia.Configuracao.Codigo.val() == layoutsEDI[i].Codigo) {
                    layoutsEDI[i] = {
                        Codigo: guid(),
                        CodigoLayoutEDI: instancia.Configuracao.LayoutEDI.codEntity(),
                        DescricaoLayoutEDI: instancia.Configuracao.LayoutEDI.val(),
                        TipoIntegracao: instancia.Configuracao.TipoIntegracao.val(),
                        DescricaoTipoIntegracao: $("#" + instancia.Configuracao.TipoIntegracao.id + " option:selected").text(),
                        EnderecoFTP: instancia.Configuracao.EnderecoFTP.val(),
                        Usuario: instancia.Configuracao.Usuario.val(),
                        Senha: instancia.Configuracao.Senha.val(),
                        Porta: instancia.Configuracao.Porta.val(),
                        Diretorio: instancia.Configuracao.Diretorio.val(),
                        Passivo: instancia.Configuracao.Passivo.val(),
                        UtilizarSFTP: instancia.Configuracao.UtilizarSFTP.val(),
                        CertificadoChavePrivada: instancia.Configuracao.CertificadoChavePrivada.val(),
                        CertificadoChavePrivadaBase64: instancia.Configuracao.CertificadoChavePrivadaBase64.val(),
                        SSL: instancia.Configuracao.SSL.val(),
                        UtilizarLeituraArquivos: instancia.Configuracao.UtilizarLeituraArquivos.val(),
                        AdicionarEDIFilaProcessamento: instancia.Configuracao.AdicionarEDIFilaProcessamento.val(),
                        CriarComNomeTemporaraio: instancia.Configuracao.CriarComNomeTemporaraio.val(),
                        Emails: instancia.Configuracao.Emails.val(),
                        EmailsAlertaLeituraEDI: instancia.Configuracao.EmailsAlertaLeituraEDI.val()
                    };

                    break;
                }
            }
        }
        instancia.KnockoutConfiguracao.val(JSON.stringify(layoutsEDI));
        instancia.RecarregarGrid(layoutsEDI);
        instancia.LimparCampos();
    };

    this.ExcluirLayoutEDI = function () {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Configuracao.LayoutEDI.DesejaRealmenteExcluirLayoutSuasConfiguracoes.format(instancia.Configuracao.LayoutEDI.val()), Excluir);

    };

    function Excluir() {
        var layoutsEDI = instancia.KnockoutConfiguracao != null ? JSON.parse(instancia.KnockoutConfiguracao.val()) : new Array();

        for (var i = 0; i <= layoutsEDI.length; i++) {
            if (instancia.Configuracao.Codigo.val() == layoutsEDI[i].Codigo) {
                layoutsEDI.splice(i, 1);
                break;
            }
        }

        instancia.KnockoutConfiguracao.val(JSON.stringify(layoutsEDI));
        instancia.RecarregarGrid(layoutsEDI);
        instancia.LimparCampos();
    };

    this.CancelarLayoutEDI = function () {
        instancia.LimparCampos();
    };

    this.EditarLayoutEDI = function (layout) {
        instancia.Configuracao.Codigo.val(layout.Codigo);
        instancia.Configuracao.LayoutEDI.codEntity(layout.CodigoLayoutEDI);
        instancia.Configuracao.LayoutEDI.val(layout.DescricaoLayoutEDI);
        instancia.Configuracao.TipoIntegracao.val(layout.TipoIntegracao);
        instancia.Configuracao.EnderecoFTP.val(layout.EnderecoFTP);
        instancia.Configuracao.Usuario.val(layout.Usuario);
        instancia.Configuracao.Senha.val(layout.Senha);
        instancia.Configuracao.Porta.val(layout.Porta);
        instancia.Configuracao.Diretorio.val(layout.Diretorio);
        instancia.Configuracao.Passivo.val(layout.Passivo);
        instancia.Configuracao.UtilizarSFTP.val(layout.UtilizarSFTP);

        instancia.Configuracao.CertificadoChavePrivadaBase64.val(layout.CertificadoChavePrivadaBase64);
        instancia.Configuracao.CertificadoChavePrivada.val(layout.CertificadoChavePrivada);

        instancia.Configuracao.SSL.val(layout.SSL);
        instancia.Configuracao.Emails.val(layout.Emails);
        instancia.Configuracao.EmailsAlertaLeituraEDI.val(layout.EmailsAlertaLeituraEDI);
        instancia.Configuracao.UtilizarLeituraArquivos.val(layout.UtilizarLeituraArquivos);
        instancia.Configuracao.AdicionarEDIFilaProcessamento.val(layout.AdicionarEDIFilaProcessamento);
        instancia.Configuracao.CriarComNomeTemporaraio.val(layout.CriarComNomeTemporaraio);

        instancia.Configuracao.Adicionar.visible(false);
        instancia.Configuracao.Atualizar.visible(true);
        instancia.Configuracao.Excluir.visible(true);
        instancia.Configuracao.Cancelar.visible(true);
    };

    instancia.ObterTiposIntegracao().then(function () {
        return instancia.Load();
    });

    this.SalvarAnexoLayoutEDI = function () {
        console.log("");
    };
};

function getBase64File(element, file) {
    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        let encoded = reader.result.toString().replace(/^data:(.*,)?/, '');
        if ((encoded.length % 4) > 0) {
            encoded += '='.repeat(4 - (encoded.length % 4));
        }
        element.val(encoded);
    }
}