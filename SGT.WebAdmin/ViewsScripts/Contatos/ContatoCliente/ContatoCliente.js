function ContatoCliente(idButton, knoutCodigoObjeto, tipoObjeto) {
    var self = this;

    self.idButton = idButton;
    self.knoutCodigoObjeto = knoutCodigoObjeto;
    self.tipoObjeto = tipoObjeto;
    self.idModalContatoCliente = guid();
    self.idKnockoutContatoCliente = guid();
    self.idKnockoutCRUDContatoCliente = guid();
    self.idKnockoutPesquisaContatoCliente = guid();
    self.idKnockoutResumoContatoCliente = guid();

    self.ResumoContatoClienteModel = function () {
        this.Titulo = PropertyEntity({ val: ko.observable(""), def: "" });
    };

    self.PesquisaContatoClienteModel = function () {
        this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
        this.CodigoObjeto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
        this.TipoObjeto = PropertyEntity({ val: ko.observable(0), def: 0 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                self.GridContatoCliente.CarregarGrid();
            }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
        });
    };

    self.ContatoClienteModel = function () {
        var situacoesContato = self.SituacoesContato || [];
        var situacaoContatoDefault = situacoesContato.length > 0 ? situacoesContato[0].value : "";

        this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
        this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 1000 });
        this.Contato = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contato:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
        this.ContatoSemCadastro = PropertyEntity({ text: "Contato sem cadastro:", required: false, maxlength: 150 });
        this.Data = PropertyEntity({ text: "*Data do Contato:", required: true, getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
        this.DataPrevistaRetorno = PropertyEntity({ text: "Previsão de Retorno:", required: false, getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
        this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo de Contato:", options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true) });
        this.SituacaoContato = PropertyEntity({ text: "Situação:", options: situacoesContato, val: ko.observable(situacaoContatoDefault), def: situacaoContatoDefault, issue: 0, visible: ko.observable(true) });

        this.Pessoa = PropertyEntity({ val: ko.observable(0), def: 0 });
        this.GrupoPessoas = PropertyEntity({ val: ko.observable(0), def: 0 });
        this.CodigoObjeto = PropertyEntity({ val: ko.observable(0), def: 0 });
        this.TipoObjeto = PropertyEntity({ val: ko.observable(0), def: 0 });
    };

    self.CRUDContatoClienteModel = function () {
        this.Adicionar = PropertyEntity({ eventClick: self.Adicionar, type: types.event, text: "Adicionar", visible: ko.observable(true) });
        this.Atualizar = PropertyEntity({ eventClick: self.Atualizar, type: types.event, text: "Atualizar", visible: ko.observable(false) });
        this.Excluir = PropertyEntity({ eventClick: self.Excluir, type: types.event, text: "Excluir", visible: ko.observable(false) });
        this.Cancelar = PropertyEntity({ eventClick: self.Cancelar, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    };

    self.Load = function () {
        var p = new promise.Promise();

        self.CarregarHTML().then(function () {
            self.CarregarSituacoes().then(function () {

                self.ContatoCliente = new self.ContatoClienteModel();
                KoBindings(self.ContatoCliente, self.idKnockoutContatoCliente);

                self.PesquisaContatoCliente = new self.PesquisaContatoClienteModel();
                KoBindings(self.PesquisaContatoCliente, self.idKnockoutPesquisaContatoCliente);

                self.ResumoContatoCliente = new self.ResumoContatoClienteModel();
                KoBindings(self.ResumoContatoCliente, self.idKnockoutResumoContatoCliente);

                self.CRUDContatoCliente = new self.CRUDContatoClienteModel();
                KoBindings(self.CRUDContatoCliente, self.idKnockoutCRUDContatoCliente);

                self.MontarGridContatoCliente();

                new BuscarContatosPessoa(self.ContatoCliente.Contato, self.ContatoCliente.Pessoa, self.ContatoCliente.GrupoPessoas, self.RetornoConsultaContato);

                $("#" + self.idButton).unbind();
                $("#" + self.idButton).on('click', function () { self.Show(); });

                p.done();
            });
        });

        return p;
    };

    self.CarregarHTML = function () {
        var p = new promise.Promise();

        $.get("Content/Static/Contatos/ContatoCliente.html?dyn=" + guid(), function (data) {

            data = data
                .replace(/#idModal/g, self.idModalContatoCliente)
                .replace(/#knockoutResumoItem/g, self.idKnockoutResumoContatoCliente)
                .replace(/#knockoutContatoCliente/g, self.idKnockoutContatoCliente)
                .replace(/#knockoutPesquisaContatoCliente/g, self.idKnockoutPesquisaContatoCliente)
                .replace(/#knockoutCRUDContatoCliente/g, self.idKnockoutCRUDContatoCliente);

            $("#js-page-content").append(data);

            p.done();

        });

        return p;
    };

    self.CarregarSituacoes = function () {
        var p = new promise.Promise();

        executarReST("SituacaoContato/BuscarTodos", {}, function (r) {
            if (r.Success) {
                if (r.Data) {
                    self.SituacoesContato = r.Data;
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }

            p.done();
        });

        return p;
    }

    self.RetornoConsultaContato = function (contato) {
        executarReST("PessoaContato/BuscarPorCodigo", { Codigo: contato.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {

                    self.ContatoCliente.Contato.val(r.Data.Contato);
                    self.ContatoCliente.Contato.codEntity(r.Data.Codigo);

                    $("#" + self.ContatoCliente.TipoContato.id).selectpicker('val', r.Data.TipoContato);

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    }

    self.CarregarDetalhesResumo = function () {
        var p = new promise.Promise();

        executarReST("ContatoCliente/ObterDetalhesDocumento", { CodigoObjeto: self.knoutCodigoObjeto.val(), TipoObjeto: self.tipoObjeto }, function (r) {
            if (r.Success) {
                if (r.Data) {

                    self.ContatoCliente.GrupoPessoas.val(r.Data.GrupoPessoas.Codigo);
                    self.ContatoCliente.GrupoPessoas.def = r.Data.GrupoPessoas.Codigo;
                    self.ContatoCliente.Pessoa.val(r.Data.Pessoa.Codigo);
                    self.ContatoCliente.Pessoa.def = r.Data.Pessoa.Codigo;

                    var titulo = "Contato referente "

                    switch (self.tipoObjeto) {
                        case EnumTipoDocumentoContatoCliente.Bordero:
                            titulo += "ao borderô ";
                            break;
                        case EnumTipoDocumentoContatoCliente.Fatura:
                            titulo += "à fatura ";
                            break;
                        case EnumTipoDocumentoContatoCliente.Titulo:
                            titulo += "ao título ";
                            break;
                        default:
                            titulo += "ao item ";
                            break;
                    }

                    titulo += r.Data.Numero;

                    if (r.Data.GrupoPessoas.Codigo != null && r.Data.GrupoPessoas.Codigo > 0)
                        titulo += " do(a) " + r.Data.GrupoPessoas.Descricao;
                    else if (r.Data.Pessoa.Codigo != null && r.Data.Pessoa.Codigo > 0)
                        titulo += " do(a) " + r.Data.Pessoa.Descricao;

                    titulo += ".";

                    self.ResumoContatoCliente.Titulo.val(titulo);

                    p.done();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });

        return p;
    }

    self.MontarGridContatoCliente = function () {

        var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: self.EditarContatoCliente, tamanho: "8", icone: "" };

        var menuOpcoes = {
            tipo: TypeOptionMenu.link,
            opcoes: [editar]
        };

        self.GridContatoCliente = new GridView(self.PesquisaContatoCliente.Pesquisar.idGrid, "ContatoCliente/Pesquisa", self.PesquisaContatoCliente, menuOpcoes, { column: 1, dir: orderDir.desc }, 5);
    }

    self.EditarContatoCliente = function (contatoCliente) {
        self.LimparCampos();

        self.ContatoCliente.Codigo.val(contatoCliente.Codigo);

        BuscarPorCodigo(self.ContatoCliente, "ContatoCliente/BuscarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    self.CRUDContatoCliente.Atualizar.visible(true);
                    self.CRUDContatoCliente.Excluir.visible(true);
                    self.CRUDContatoCliente.Cancelar.visible(true);
                    self.CRUDContatoCliente.Adicionar.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    }

    self.Adicionar = function () {
        Salvar(self.ContatoCliente, "ContatoCliente/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    self.GridContatoCliente.CarregarGrid();
                    self.LimparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    self.Atualizar = function () {
        Salvar(self.ContatoCliente, "ContatoCliente/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                    self.GridContatoCliente.CarregarGrid();
                    self.LimparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    self.Excluir = function () {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir esse contato?", function () {
            ExcluirPorCodigo(self.ContatoCliente, "ContatoCliente/ExcluirPorCodigo", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                        self.GridContatoCliente.CarregarGrid();
                        self.LimparCampos();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }

            }, null);
        });
    }

    self.Cancelar = function () {
        self.LimparCampos();
    }

    self.LimparCampos = function () {
        self.CRUDContatoCliente.Adicionar.visible(true);
        self.CRUDContatoCliente.Atualizar.visible(false);
        self.CRUDContatoCliente.Excluir.visible(false);
        self.CRUDContatoCliente.Cancelar.visible(false);

        LimparCampos(self.ContatoCliente);
        LimparCampos(self.PesquisaContatoCliente);
    }

    self.Show = function () {

        var codigo = Globalize.parseInt(self.knoutCodigoObjeto.val().toString());

        if (isNaN(codigo) || codigo <= 0) {
            var mensagem = "Só é possível visualizar/adicionar contatos realizados ao editar ";

            switch (self.tipoObjeto) {
                case EnumTipoDocumentoContatoCliente.Bordero:
                    mensagem += "um borderô.";
                    break;
                case EnumTipoDocumentoContatoCliente.Fatura:
                    mensagem += "uma fatura.";
                    break;
                case EnumTipoDocumentoContatoCliente.Titulo:
                    mensagem += "um título.";
                    break;
                default:
                    mensagem += "um item.";
                    break;
            }

            exibirMensagem(tipoMensagem.aviso, "Atenção!", mensagem);

            return;
        }

        self.LimparCampos();

        self.CarregarDetalhesResumo().then(function () {

            self.PesquisaContatoCliente.CodigoObjeto.val(codigo);
            self.PesquisaContatoCliente.CodigoObjeto.def = codigo;
            self.PesquisaContatoCliente.TipoObjeto.val(self.tipoObjeto);
            self.PesquisaContatoCliente.TipoObjeto.def = self.tipoObjeto;

            self.ContatoCliente.CodigoObjeto.val(codigo);
            self.ContatoCliente.CodigoObjeto.def = codigo;
            self.ContatoCliente.TipoObjeto.val(self.tipoObjeto);
            self.ContatoCliente.TipoObjeto.def = self.tipoObjeto;

            self.GridContatoCliente.CarregarGrid();

            if (!self.ModalContatoCliente)
                self.ModalContatoCliente = new bootstrap.Modal(document.getElementById(self.idModalContatoCliente), { backdrop: 'static', keyboard: true });

            self.ModalContatoCliente.show();
        });
    }

    self.Hide = function () {
        self.LimparCampos();
        self.ModalContatoCliente.hide();
    }

    self.HideButton = function () {
        $("#" + self.idButton).addClass("hidden");
    }

    self.ShowButton = function () {
        $("#" + self.idButton).removeClass("hidden");
    }

    self.Load();
}