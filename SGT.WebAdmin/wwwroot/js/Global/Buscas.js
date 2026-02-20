/// <reference path="Grid.js" />
/// <reference path="Mensagem.js" />
/// <reference path="../Global/CRUD.js" />
/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="../../js/knockout/knockout-3.3.0.js" />

$.fn.focusNextInputField = function () {
    return this.each(function () {
        var fields = $(this).parents('form:eq(0),body').find('input,textarea,select, .btnPesquisarFiltroPesquisa');
        var index = fields.index(this);

        if (index > -1 && (index + 1) < fields.length) {
            fields.eq(index + 1).focus();
        }

        return false;
    });
};

var CriarBusca = function (idDivBusca, knoutOpcoes, knoutContent, funcaoParametrosDinamicos, focarNoPrimeiroInput, multiplaEscolha, bindingCallback, mostrarFiltrosPesquisa, modalWide, openCallback, gridCallback, limiteRegistros) {
    this._bindingCallback = bindingCallback;
    this._focarNoPrimeiroInput = focarNoPrimeiroInput;
    this._funcaoParametrosDinamicos = funcaoParametrosDinamicos;
    this._grid;
    this._gridCallback = gridCallback;
    this._idDivBusca = idDivBusca;
    this._inicializado = false;
    this._knoutContent = knoutContent;
    this._knoutOpcoes = knoutOpcoes;
    this._manipulaCallback = false;
    this._modalWide = modalWide;
    this._mostrarFiltrosPesquisa = mostrarFiltrosPesquisa;
    this._multiplaEscolha = multiplaEscolha;
    this._openCallback = openCallback;
    this._tabPressionado = false;
    this._$modal;
    this._modalWindow;
    this._limiteRegistros = limiteRegistros;

    this._multiplasEntidades = this._obterMultiplasEntidades();
};

CriarBusca.prototype = {
    AddEvents: function (grid) {
        var self = this;

        self._grid = grid;

        if (!self._knoutContent) {
            exibirMensagem("atencao", "2 - Componente Incompleto", "Por favor entre em contato com a Multisoftware");
            return;
        }

        if (self._knoutContent.idBtnSearch) {
            $('#' + self._knoutContent.idBtnSearch).on('click', function (e) {
                e.stopPropagation();

                $(this).attr("disabled", "disabled"); //desabilita e habilita novamente no fim do método para evitar bug se manter enter pressionado

                self.UpdateGrid();
                self.OpenModal();

                $(this).removeAttr("disabled");
            });

            if (self._multiplasEntidades) {
                $('#' + self._knoutContent.idBtnSearch + '_multiples_entities').on('click', function (e) {
                    e.stopPropagation();

                    $(this).attr("disabled", "disabled"); //desabilita e habilita novamente no fim do método para evitar bug se manter enter pressionado

                    try {
                        self._multiplasEntidades.OpenModal();
                    }
                    catch (excecao) {
                        console.log("Ocorreu uma falha ao carregar o modal de múltiplas entidades.");
                    }

                    $(this).removeAttr("disabled");
                });
            }
        }

        self._grid.onAfterGridLoad(function (data) { self._handlePressTab(data); });

        $("#" + self._knoutContent.id).on('change.busca', function (e) {
            if (self._knoutContent.validaEscritaBusca != null && !self._knoutContent.validaEscritaBusca)
                return;

            var campoText = self._knoutContent.val();

            if (self._tabPressionado) return;

            if (campoText == "") {
                self._knoutContent.entityDescription("");
                self._knoutContent.codEntity(self._knoutContent.defCodEntity);

                if (self._multiplasEntidades)
                    self._multiplasEntidades.ClearState();

                if (self._knoutContent.cleanEntityCallback instanceof Function)
                    self._knoutContent.cleanEntityCallback();

                return;
            }

            if (campoText != self._knoutContent.entityDescription())
                return self._campoBuscaAlterado();
        });
    },
    AddTabPressEvent: function (funcaoExecutar) {
        var self = this;

        if (!self._knoutContent) {
            exibirMensagem("atencao", "3 - Componente Incompleto", "Por favor entre em contato com a Multisoftware");
            return;
        }

        $("#" + self._knoutContent.id).on('keydown.busca', function (e) {
            e.stopPropagation();

            var keyCode = e.keyCode || e.which;

            if (keyCode == 9) {
                self._tabPressionado = true;

                if (self._funcaoParametrosDinamicos instanceof Function)
                    self._funcaoParametrosDinamicos();

                funcaoExecutar(self._tabPressionado);
            }
            else
                self._tabPressionado = false;
        });
    },
    CloseModal: function () {
        if (!this._modalWindow)
            return;

        this._modalWindow.hide();
    },
    DefCallback: function (data) {

        var self = this;

        if (self._knoutContent != null) {
            if (self._knoutContent.requiredClass)
                self._knoutContent.requiredClass("form-control");

            if (data != null) {
                self._knoutContent.codEntity(data.Codigo);
                self._knoutContent.val(data.Descricao);
            }

            $.each(self._knoutOpcoes, function (i, prop) {
                if (prop.visible) {
                    if (prop.type == types.map) {
                        prop.val(prop.def);
                    }
                    else if (prop.type == types.entity) {
                        prop.codEntity(self._knoutOpcoes[i].defCodEntity);
                        prop.val(self._knoutOpcoes[i].def);
                    }
                }
            });

            self.CloseModal();
            Global.setarFocoProximoCampo(self._knoutContent.id);
        }
        else
            exibirMensagem("atencao", "1 - Componente Incompleto", "Por favor entre em contato com a Multisoftware");
    },
    Destroy: function () {
        if (this._multiplasEntidades)
            this._multiplasEntidades.Destroy();

        if (this._inicializado) {
            this._inicializado = false;
            this._modalWindow.dispose();
            this._$modal.off();
            this._$modal.remove();
        }

        if (this._knoutContent) {
            $("#" + this._knoutContent.id).off('change.busca');
            $("#" + this._knoutContent.id).off('keydown.busca');

            if (this._knoutContent.idBtnSearch) {
                $('#' + this._knoutContent.idBtnSearch).off('click');
                $('#' + this._knoutContent.idBtnSearch + '_multiples_entities').off('click');
            }
        }
    },
    ObterUrlPesquisa: function () {
        return this._grid.GetUrl();
    },
    OpcaoPadrao: function (callback, tamanho) {
        var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: guid(), evento: "onclick", metodo: this._handleCallback(callback), tamanho: tamanho ? tamanho : "15", icone: "" };
        var menuOpcoes = new Object();

        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(opcaoEditar);

        return menuOpcoes;
    },
    OpenModal: function () {
        this._abrirModalBusca();
    },
    UpdateGrid: function (callback) {
        try {
            if (this._funcaoParametrosDinamicos instanceof Function)
                this._funcaoParametrosDinamicos();

            this._grid.CarregarGrid(callback);
        }
        catch (excecao) {
            console.log("Ocorreu uma falha ao carregar o modal.");
        }
    },
    _abrirModalBusca: function () {
        var self = this;
        var inicializar = !self._inicializado;

        if (inicializar)
            self._inicializar();

        self._modalWindow.show();

        if (inicializar && (self._bindingCallback instanceof Function))
            self._bindingCallback();
    },
    _campoBuscaAlterado: function () {
        if (this._knoutContent.entityDescription() != "")
            exibirMensagem(tipoMensagem.aviso, "Campo de Busca", "Não é possível alterar a descrição dos campos de busca, pois eles possuem um cadastro próprio.");

        this._knoutContent.val(this._knoutContent.entityDescription());
    },
    _handleCallback: function (callback) {
        var self = this;

        return function () {
            self._manipulaCallback = true;

            callback.apply(undefined, arguments);
            self._knoutContent.entityDescription(self._knoutContent.val());
            self._selecaoSimples(arguments[0]);
        };
    },
    _handlePressTab: function (data) {
        if (data != null && $.isArray(data.data) && data.data.length == 1) {
            this._knoutContent.entityDescription(this._knoutContent.val());
            this._selecaoSimples(data.data[0]);
        }
    },
    _inicializar: function () {
        var self = this;

        $('#js-page-content').append(self._obterHTML());

        self._inicializado = true;
        self._$modal = $("#" + self._idDivBusca);
        self._modalWindow = new bootstrap.Modal(document.getElementById(self._idDivBusca), { keyboard: true, backdrop: 'static' });

        KoBindings(self._knoutOpcoes, self._idDivBusca, false);

        LocalizeCurrentPage();

        self._$modal.on('hidden.bs.modal', function (e) {
            if (!self._manipulaCallback)
                self._knoutContent.val(self._knoutContent.entityDescription());
            else
                self._manipulaCallback = false;
        });

        self._$modal.on('keypress', function (e) {
            var keyCode = e.keyCode || e.which;

            if (keyCode == 13) {
                if (self._knoutOpcoes.Pesquisar)
                    $('#' + self._knoutOpcoes.Pesquisar.id).trigger('click');
            }
        });

        self._$modal.on('shown.bs.modal', function (e) {
            if ((self._focarNoPrimeiroInput == null) || (self._focarNoPrimeiroInput == true))
                $("#" + self._idDivBusca + " input, " + "#" + self._idDivBusca + " select").first().focus();

            if (self._openCallback instanceof Function)
                self._openCallback();
        });
    },
    _obterHTML: function () {
        var self = this;
        var html = '';

        //html += '<div class="modal ' + (self._modalWide ? "modal-wide" : "") + ' fade modal-busca-entidade" id="' + self._idDivBusca + '"  tabindex="-1" role="dialog" aria-hidden="true">';
        //html += '    <div class="modal-dialog modal-lg">';
        //html += '        <div class="modal-content">';
        //html += '            <div class="modal-header">';
        //html += '                <button type="button" class="close" id="' + self._idDivBusca + '_fecharModal" data-dismiss="modal" aria-hidden="true"> &times;</button>';
        //html += '                <h4 class="modal-title" id="myModalLabel" ' + (self._mostrarFiltrosPesquisa === false ? 'data-bind="text: TituloGrid.text"' : 'data-bind="text: Titulo.text"') + '></h4>';
        //html += '            </div>';
        //html += '            <div class="modal-body">';
        //html += '                <div>';
        //html += '                    <div>';
        //html += '                        <div class="smart-form">';
        //html += '                            <section style="' + (self._mostrarFiltrosPesquisa === false ? 'display: none;' : '') + '">';
        //html += '                                <div class="well">';
        //html += '                                    <fieldset>';

        html += '<div class="modal fade" id="' + self._idDivBusca + '" tabindex="-1" role="dialog" aria-hidden="true">';
        html += '   <div class="modal-dialog modal-xl modal-dialog-centered" role="document">';
        html += '       <div class="modal-content">';
        html += '           <div class="modal-header">';
        html += '              <h4 class="modal-title" ' + (self._mostrarFiltrosPesquisa === false ? 'data-bind="text: TituloGrid.text"' : 'data-bind="text: Titulo.text"') + '></h4>';
        html += '              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">';
        html += '                  <span aria-hidden="true"><i class="fal fa-times"></i></span>';
        html += '              </button>';
        html += '           </div>';
        html += '       <div class="modal-body">';
        html += '           <form>';


        var tamanhoColuna = 0;
        var abriuRow = false;
        var htmlFooter = "";
        var htmlFooterAvancado = "";
        var buscaAvancada = false;

        $.each(self._knoutOpcoes, function (i, prop) {
            var classSelecion = "";

            if (prop.visible == true || typeof prop.visible === "function") {
                if (prop.type == types.map || prop.type == types.entity || prop.type == types.multiplesEntities) {
                    classSelecion = 'class="col-12 col-lg-' + prop.col + '"';

                    if ((tamanhoColuna == 0 && prop.col < 12) || !abriuRow) {
                        html += '<div class="row">';
                        abriuRow = true;
                    }

                    if (prop.col >= 12) {
                        if (abriuRow)
                            html += '</div>';

                        html += '<div class="row">';
                        tamanhoColuna = 12;
                        abriuRow = true;
                        classSelecion = 'class="col-12"';
                    }
                    else
                        tamanhoColuna += prop.col;

                    var enable = true;
                    if (prop.enable === false)
                        enable = false;

                    if (prop.type == types.map && prop.options == null) {
                        html += '<div ' + classSelecion + '>';
                        html += '   <div class="form-group">';
                        html += '       <label class="form-label" data-bind="text: ' + i + '.text, attr: { for: ' + i + '.id }"></label>';
                        html += '       <input type="text" class="form-control" data-bind="value: ' + i + '.val, enable: ' + enable + ', valueUpdate: \'afterkeydown\', attr: { \'aria-label\': ' + i + '.text, maxlength: ' + i + '.maxlength, id: ' + i + '.id, name: ' + i + '.id }" />';
                        html += '   </div>';
                        html += '</div>';
                    }
                    else if (prop.type == types.map && prop.options != null && prop.getType == typesKnockout.selectMultiple) {
                        //html += '<div ' + classSelecion + '>';
                        //html += '    <label class="label"><b data-bind="text: ' + i + '.text"></b></label>';
                        //html += '    <label class="select">';
                        //html += '        <select data-bind="options: ' + i + '.options, optionsText: \'text\', optionsValue: \'value\', selectedOptions: ' + i + '.val, attr: { id : ' + i + '.id}" multiple="multiple" data-actions-box="false" data-selected-text-format="count > 3" title=' + prop.selectMultipleTitle + '></select>';
                        //html += '    </label>';
                        //html += '</div>';

                        html += '<div ' + classSelecion + '>';
                        html += '   <div class="form-group">';
                        html += '       <label class="form-label" data-bind="text: ' + i + '.text, attr: { for: ' + i + '.id }"></label>';
                        html += '       <select class="form-control" data-bind="options: ' + i + '.options, optionsText: \'text\', optionsValue: \'value\', selectedOptions: ' + i + '.val, attr: { id : ' + i + '.id, name: ' + i + '.id }" multiple="multiple" data-actions-box="false" data-selected-text-format="count > 3" title="' + prop.selectMultipleTitle + '"></select>';
                        html += '   </div>';
                        html += '</div>';
                    }
                    else if (prop.type == types.map && prop.options != null) {
                        //html += '<section ' + classSelecion + '>';
                        //html += '    <label class="label"><b data-bind="text: ' + i + '.text"></b></label>';
                        //html += '    <label class="select">';
                        //html += '        <select data-bind="options: ' + i + '.options, optionsText: \'text\', optionsValue: \'value\', value: ' + i + '.val, attr: { id : ' + i + '.id}"></select><i></i>';
                        //html += '    </label>';
                        //html += '</section>';

                        html += '<div ' + classSelecion + '>';
                        html += '   <div class="form-group">';
                        html += '       <label class="form-label" data-bind="text: ' + i + '.text, attr: { for: ' + i + '.id }"></label>';
                        html += '       <select class="form-control" data-bind="options: ' + i + '.options, optionsText: \'text\', optionsValue: \'value\', value: ' + i + '.val, attr: { id : ' + i + '.id, name: ' + i + '.id }"></select>';
                        html += '   </div>';
                        html += '</div>';
                    }
                    else if (prop.type == types.entity || prop.type == types.multiplesEntities) {
                        //html += '<section ' + classSelecion + ' data-bind="visible: ' + i + '.visible" >';
                        //html += '    <div class="input-group">';
                        //html += '        <label class="label"><b data-bind="text: ' + i + '.text"></b></label>';
                        //html += '        <label data-bind="attr: { class: ' + i + '.requiredClass}">';
                        //html += '            <input type="text" data-bind="value: ' + i + '.val, valueUpdate: \'afterkeydown\', attr: { maxlength: ' + i + '.maxlength, id : ' + i + '.id}" />';
                        //html += '        </label>';
                        //html += '        <div class="input-group-btn">';
                        //html += '            <button data-bind="attr: { id: ' + i + '.idBtnSearch}" class="btn btn-default btn-primary botaoBusca" type="button">';
                        //html += '                <i class="fa fa-search"></i> <span data-bind="text: ' + i + '.textBtnSearch"></span> ';
                        //html += '            </button>';
                        //html += '        </div>';
                        //html += '    </div>';
                        //html += '</section>';

                        html += '<div ' + classSelecion + ' data-bind="visible: ' + i + '.visible">';
                        html += '   <div class="form-group">';
                        html += '       <label class="form-label" data-bind="text: ' + i + '.text, attr: { for: ' + i + '.id }"></label>';
                        html += '       <div class="input-group">';
                        html += '           <input type="text" class="form-control" data-bind="value: ' + i + '.val, valueUpdate: \'afterkeydown\', attr: { \'aria-label\': ' + i + '.text, maxlength: ' + i + '.maxlength, id: ' + i + '.id, name: ' + i + '.id }" />';
                        html += '           <div class="input-group-append">';
                        html += '               <button class="btn btn-primary waves-effect waves-themed" type="button" data-bind="attr: { id: ' + i + '.idBtnSearch }">';
                        html += '                   <i class="fal fa-search"></i>';
                        html += '               </button>';
                        html += '           </div>';
                        html += '       </div>';
                        html += '   </div>';
                        html += '</div>';
                    }

                    if (abriuRow && tamanhoColuna == 12) {
                        html += '</div>';
                        tamanhoColuna = 0;
                        abriuRow = false;
                    }
                }
                else if (prop.type == types.event) {
                    if (prop.buscaAvancada == null || prop.buscaAvancada == false) {
                        let cssClass = prop.cssClass != "" ? prop.cssClass : "btn btn-primary";
                        let icon = prop.icon != "" ? prop.icon : "fal fa-search";

                        htmlFooter += '<button class="' + cssClass + ' ms-2 waves-effect waves-themed" data-bind="click: ' + i + '.eventClick, attr : { id: ' + i + '.id }" type="button">';
                        htmlFooter += '    <i class="' + icon + '"></i>&nbsp;<span data-bind="text: ' + i + '.text"></span>';
                        htmlFooter += '</button>';
                    }
                    else {
                        if (abriuRow) {
                            html += "</div>";
                            abriuRow = false;
                            tamanhoColuna = 0;
                        }

                        let cssClass = prop.cssClass;
                        let icon = prop.icon();

                        htmlFooterAvancado += '<button class="' + cssClass + ' ms-2 waves-effect waves-themed" data-bind="click: ' + i + '.eventClick, attr : { id: ' + i + '.id }" type="button">';
                        htmlFooterAvancado += '    <i data-bind="attr: { class: BuscaAvancada.icon}" class="' + icon + '"></i>&nbsp;<span data-bind="text: ' + i + '.text"></span>';
                        htmlFooterAvancado += '</button>';

                        html += '<div data-bind="id: BuscaAvancada.idFade, fadeVisible: BuscaAvancada.visibleFade">';
                        buscaAvancada = true;
                    }
                }
            }
        });

        if (abriuRow)
            html += '</div>';

        if (buscaAvancada)
            html += '</div>';

        html += '</form>';

        html += '<div class="row">';
        html += '                                    <div class="col-12 d-flex justify-content-end">';
        html += htmlFooterAvancado + htmlFooter;
        html += '                                    </div>';
        html += '</div>'
        html += '                    <div class="mt-3 ' + (self._mostrarFiltrosPesquisa === false ? 'hidden' : '') + '">';
        html += '                            <header>';
        html += '                                <h5><i class="fal fa-list"></i>&nbsp;<span data-bind="text: TituloGrid.text"></span>';

        if (self._multiplaEscolha)
            html += '                                <button type="button" id="' + self._idDivBusca + '_btnSelecionarTodos" data-bind="click: Pesquisar.selecionarTodosEventClick" class="btn btn-default waves-effect waves-themed float-end"><i class="fal fa-check"></i>&nbsp;' + Localization.Resources.Gerais.Geral.MarcarTodos + '</button>';

        html += '                                </h5>';
        html += '                            </header>';
        html += '                    </div>';
        html += '                    <table width="100%" class="table table-bordered table-hover " id="' + self._idDivBusca + '_tabelaEntidades" cellspacing="0"></table>';

        if (self._multiplaEscolha) {
            html += '                <div class="row mt-3">';
            html += '                    <section class="col-4"><input disabled type="button" id="' + self._idDivBusca + '_btnConfirmarMultiplaEscolha" data-bind="click: Pesquisar.multiploEventClick" class="btn btn-success waves-effect waves-themed" value="' + Localization.Resources.Gerais.Geral.ConfirmarSelecao + '" /></section>';
            html += '                    <section class="col-8 text-end" style="font-weight: bold; font-style: italic; color: #969696;"><span class="dataTables_info" id="' + self._idDivBusca + '_lblQtdeSelecionadosMultiplaEscolha"></span ></section>';
            html += '                </div>';
        }

        html += '            </div>';

        html += '        </div>';
        html += '    </div>';
        html += '</div>';

        return html;
    },
    _obterMultiplasEntidades: function () {
        if (this._knoutContent.type == types.multiplesEntities)
            return new CriarListaMultiplasEntidades(this, this._idDivBusca, this._knoutOpcoes, this._knoutContent, this._funcaoParametrosDinamicos, this._focarNoPrimeiroInput, this._bindingCallback, this._mostrarFiltrosPesquisa, this._modalWide, this._gridCallback, this._limiteRegistros);

        return undefined;
    },
    _selecaoSimples: function (registroSelecionado) {
        if (this._multiplasEntidades)
            this._multiplasEntidades.SelecaoSimples(registroSelecionado);
    }
};

var CriarListaMultiplasEntidades = function (instanciaBusca, idDivBusca, knoutOpcoes, knoutContent, funcaoParametrosDinamicos, focarNoPrimeiroInput, bindingCallback, mostrarFiltrosPesquisa, modalWide, gridCallback, limiteRegistros) {
    this._bindingCallback = bindingCallback;
    this._buscaInterna;
    this._config = knoutContent.multiplesEntitiesConfig;
    this._definicaoInternaEntidadesSelecionadas = false;
    this._eventoAlteracaoMultiplesEntities;
    this._focarNoPrimeiroInput = focarNoPrimeiroInput;
    this._funcaoParametrosDinamicos = funcaoParametrosDinamicos;
    this._grid;
    this._gridBuscaInterna;
    this._gridcallback = gridCallback;
    this._idBuscaInterna = idDivBusca + "buscainterna";
    this._idModal = idDivBusca + "_modal_multiples_entities";
    this._idGrid = this._idModal + "_grid";
    this._inicializado = false;
    this._instanciaBusca = instanciaBusca;
    this._knoutContent = knoutContent;
    this._knoutOpcoes = this._obterObjetoKnoutOpcoes(knoutOpcoes);
    this._ko = new KnoutMultiplasEntidades(knoutContent, this._idGrid);
    this._modalWide = modalWide;
    this._mostrarFiltrosPesquisa = mostrarFiltrosPesquisa;
    this._$modal;
    this._modalWindow;
    this._limiteRegistros = limiteRegistros;

    this._adicionarEventoAlteracaoMultiplesEntities();
};

CriarListaMultiplasEntidades.prototype = {
    ClearState: function () {
        this._definirEntidadesSelecionadas([]);
    },
    Destroy: function () {
        this._eventoAlteracaoMultiplesEntities.dispose();

        if (this._inicializado) {
            this._inicializado = false;
            this._grid.Destroy();
            this._gridBuscaInterna.Destroy();
            this._buscaInterna.Destroy();
            this._modalWindow.dispose();
            this._$modal.remove();
        }
    },
    OpenModal: function () {
        var self = this;

        self._inicializar();
        self._carregarEntidadesSelecionadas();
        self._gridBuscaInterna.AtualizarRegistrosSelecionados(new Array());

        if ((self._obterEntidadesSelecionadas().length === 0) && self._isComponenteHabilitado())
            $('#' + self._ko.Entidade.idBtnSearch).trigger("click");
        else
            self._modalWindow.show();
    },
    SelecaoSimples: function (registroSelecionado) {
        this._definirEntidadesSelecionadas([registroSelecionado], this._gridcallback);
    },
    _adicionarEventoAlteracaoMultiplesEntities: function () {
        var self = this;

        self._eventoAlteracaoMultiplesEntities = self._knoutContent.multiplesEntities.subscribe(function (entidadesSelecionadas) {
            if (!self._definicaoInternaEntidadesSelecionadas)
                self._definirEntidadesSelecionadas(entidadesSelecionadas);
        });
    },
    _carregarBuscaInterna: function () {
        var self = this;

        var objetoMultiplaSelecao = {
            basicGrid: self._grid,
            callback: function (data) { self._retornoMultiplaSelecaoInterna(data); },
            eventos: self._knoutOpcoes.Pesquisar,
            manterSelecionadosMultiPesquisa: true
        };

        self._gridBuscaInterna = new GridView(self._idBuscaInterna + "_tabelaEntidades", self._instanciaBusca.ObterUrlPesquisa(), self._knoutOpcoes, null, null, null, null, null, null, objetoMultiplaSelecao, this._limiteRegistros);
        self._buscaInterna = new CriarBusca(self._idBuscaInterna, self._knoutOpcoes, self._ko.Entidade, self._funcaoParametrosDinamicos, self._focarNoPrimeiroInput, true, self._bindingCallback, self._mostrarFiltrosPesquisa, self._modalWide);
        self._buscaInterna.AddEvents(self._gridBuscaInterna);
    },
    _carregarEntidadesSelecionadas: function () {
        var entidadesSelecionadas = this._obterEntidadesSelecionadas();

        if (this._grid)
            this._grid.CarregarGrid(entidadesSelecionadas);

        switch (entidadesSelecionadas.length) {
            case 0:
                this._preencherKnoutEntidade(null);
                break;

            case 1:
                this._preencherKnoutEntidade(entidadesSelecionadas[0]);
                break;

            default:
                var multiplosRegistrosSelecionados = {};

                multiplosRegistrosSelecionados[this._config.propDescricao] = Localization.Resources.Gerais.Geral.MultiplosRegistrosSelecionados;
                multiplosRegistrosSelecionados[this._config.propCodigo] = -1;

                this._preencherKnoutEntidade(multiplosRegistrosSelecionados);
                break;
        }
    },
    _carregarGrid: function () {
        var self = this;

        var header = [
            { data: self._config.propCodigo, visible: false },
            { data: self._config.propDescricao, title: Localization.Resources.Gerais.Geral.Descricao, width: "90%", className: "text-align-left" }
        ];

        var menuOpcoes = {
            tipo: TypeOptionMenu.link,
            opcoes: [
                {
                    descricao: Localization.Resources.Gerais.Geral.Excluir,
                    id: guid(),
                    evento: "onclick",
                    tamanho: "10",
                    icone: "",
                    metodo: function (registroSelecionado) { self._excluirClick(registroSelecionado); }
                }
            ]
        };

        if (($.isFunction(self._ko.Adicionar.enable) && !self._ko.Adicionar.enable()) || self._ko.Adicionar.enable === false)
            menuOpcoes = null;

        self._grid = new BasicDataTable(self._idGrid, header, menuOpcoes);
        self._grid.CarregarGrid([]);
    },
    _definirEntidadesSelecionadas: function (entidadesSelecionadas, callback) {
        if (!this._knoutContent.multiplesEntities)
            return console.error("multiplesEntities não foi definido como um observableArray. Sete o type como types.multiplesEntities");

        this._definicaoInternaEntidadesSelecionadas = true;
        this._knoutContent.multiplesEntities(entidadesSelecionadas);
        this._definicaoInternaEntidadesSelecionadas = false;
        this._carregarEntidadesSelecionadas();

        if (callback instanceof Function)
            callback(this._obterEntidadesSelecionadas());
    },
    _excluirClick: function (registroSelecionado) {
        var codigo = registroSelecionado[this._config.propCodigo];
        var entidadesSelecionadas = this._obterEntidadesSelecionadas();

        for (var i in entidadesSelecionadas) {
            if (entidadesSelecionadas[i][this._config.propCodigo] == codigo) {
                entidadesSelecionadas.splice(i, 1);
                break;
            }
        }

        this._definirEntidadesSelecionadas(entidadesSelecionadas, this._gridcallback);
    },
    _fecharModal: function () {
        this._modalWindow.hide();

        Global.setarFocoProximoCampo(this._knoutContent.id);
    },
    _inicializar: function () {
        if (this._inicializado)
            return;

        $('#js-page-content').append(this._obterHTML());

        this._inicializado = true;
        this._$modal = $("#" + this._idModal);
        this._modalWindow = new bootstrap.Modal(document.getElementById(this._idModal), { keyboard: true, backdrop: 'static' });

        KoBindings(this._ko, this._idModal, false);

        LocalizeCurrentPage();

        this._carregarGrid();
        this._carregarBuscaInterna();
    },
    _isComponenteHabilitado: function () {
        if (this._knoutContent.enable instanceof Function)
            return this._knoutContent.enable();

        return this._knoutContent.enable;
    },
    _obterEntidadesSelecionadas: function () {
        if (!this._knoutContent.multiplesEntities)
            return console.error("multiplesEntities não foi definido como um observableArray. Sete o type como types.multiplesEntities");

        return this._knoutContent.multiplesEntities().slice();
    },
    _obterHTML: function () {
        return $("#html_base_modal_multiples_entities").html().replace("{{id}}", this._idModal);
    },
    _obterObjetoKnoutOpcoes: function (knoutOpcoes) {
        var self = this;
        var copiaKnoutOpcoes = {};

        for (var i in knoutOpcoes) {
            if (i == "Pesquisar") continue;

            copiaKnoutOpcoes[i] = $.extend({}, knoutOpcoes[i]);
            copiaKnoutOpcoes[i].id = guid();
        }

        copiaKnoutOpcoes.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                self._gridBuscaInterna.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });

        return copiaKnoutOpcoes;
    },
    _preencherKnoutEntidade: function (dados) {
        if (!dados) {

            if ($.isFunction(this._knoutContent.val)) {
                this._knoutContent.val("");
            }

            this._knoutContent.entityDescription("");

            if ($.isFunction(this._knoutContent.codEntity))
                this._knoutContent.codEntity(0);

            return;
        }

        this._knoutContent.val(dados[this._config.propDescricao]);
        this._knoutContent.entityDescription(dados[this._config.propDescricao]);

        if ($.isFunction(this._knoutContent.codEntity))
            this._knoutContent.codEntity(dados[this._config.propCodigo]);
    },
    _retornoMultiplaSelecaoInterna: function (registrosSelecionados) {
        if (!$.isArray(registrosSelecionados))
            registrosSelecionados = [registrosSelecionados];

        var entidadesSelecionadas = this._obterEntidadesSelecionadas().concat(registrosSelecionados);

        this._definirEntidadesSelecionadas(entidadesSelecionadas, this._gridcallback);
        this._fecharModal();
    }
};

var KnoutMultiplasEntidades = function (knoutContent, idGrid) {
    this.Entidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: knoutContent.text, idBtnSearch: guid(), visible: false });
    this.Adicionar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Adicionar, idGrid: idGrid, enable: knoutContent.enable });
};
