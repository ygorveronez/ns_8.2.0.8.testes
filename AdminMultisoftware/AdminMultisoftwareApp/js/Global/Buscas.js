/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="../knockout/knockout-3.1.0.js" />
/// <reference path="Mensagem.js" />

$.fn.focusNextInputField = function () {
    return this.each(function () {
        var fields = $(this).parents('form:eq(0),body').find('input,textarea,select');
        var index = fields.index(this);
        if (index > -1 && (index + 1) < fields.length) {
            fields.eq(index + 1).focus();
        }
        return false;
    });
};

var CriarBusca = function (idDivBusca, knoutOpcoes, knoutContent, funcaoParametrosDinamicos, focarNoPrimeiroInput) {
    var html = '<div class="modal fade" id="' + idDivBusca + '"  tabindex="-1" role="dialog" aria-hidden="true">';
    html += '<div class="modal-dialog modal-lg"><div class="modal-content"><div class="modal-header">';
    html += '<button type="button" class="close" data-dismiss="modal" aria-hidden="true"> &times;</button>';
    html += '<h4 class="modal-title" id="myModalLabel" data-bind="text: Titulo.text"></h4>';
    html += '</div>';
    html += '<div class="modal-body"><div><div>';
    html += '<div class="smart-form">';
    html += '<section>';
    html += '<div class="well">';
    html += '<fieldset>';

    var tamanhoColuna = 0;
    var abriuRow = false;
    var htmlFooter = "";
    $.each(knoutOpcoes, function (i, prop) {
        var classSelecion = "";
        if (prop.visible == true) {
            if (prop.type == types.map || prop.type == types.entity) {

                classSelecion = 'class="col col-' + prop.col + '"';
                if (tamanhoColuna == 0 && prop.col < 12) {
                    html += '<div class="row">';
                    abriuRow = true;
                }

                if (prop.col >= 12) {
                    if (abriuRow) {
                        html += '</div>';
                    }
                    tamanhoColuna = 0;
                    abriuRow = false;
                    classSelecion = "";
                } else {
                    tamanhoColuna += prop.col;
                }

                if (prop.type == types.map) {
                    html += '<section ' + classSelecion + ' >';
                    html += '<label class="label"><b  data-bind="text: ' + i + '.text"></b></label>';
                    html += '<label class="input">';
                    html += '<input type="text" data-bind="value: ' + i + '.val, valueUpdate: \'afterkeydown\',  attr: { maxlength: ' + i + '.maxlength, id : ' + i + '.id}" />';
                    html += '</label></section>';
                } else if (prop.type == types.entity) {
                    html += '<section ' + classSelecion + ' >';
                    html += '<div class="input-group">';
                    html += '<label class="label"><b data-bind="text: ' + i + '.text"></b></label>';
                    html += '<label data-bind="attr: { class: ' + i + '.requiredClass}">';
                    html += '<input type="text" data-bind="value: ' + i + '.val, valueUpdate: \'afterkeydown\', attr: { maxlength: ' + i + '.maxlength, id : ' + i + '.id}" />';
                    html += '</label>';
                    html += '<div class="input-group-btn">';
                    html += '<button data-bind="attr: { id: ' + i + '.idBtnSearch}" class="btn btn-default btn-primary botaoBusca" type="button">';
                    html += '<i class="fa fa-search"></i> &nbsp; Buscar &nbsp;';
                    html += '</button>';
                    html += ' </div>';
                    html += '</div>';
                    html += '</section>';
                }
                if (abriuRow && tamanhoColuna == 12) {
                    html += '</div>';
                    tamanhoColuna = 0;
                    abriuRow = false;
                }
            } else if (prop.type == types.event) {
                var cssClass = prop.cssClass != "" ? prop.cssClass : "btn btn-default btn-primary btnPesquisarFiltroPesquisa";
                var icon = prop.icon != "" ? prop.icon : "fa fa-search";
                htmlFooter += '<button class="' + cssClass + '" data-bind="click: ' + i + '.eventClick, attr : { id: ' + i + '.id }" type="button">';
                htmlFooter += '<i class="' + icon + '"></i>&nbsp;<span data-bind="text: ' + i + '.text"></span>';
                htmlFooter += '</button>';
            }
        }

    });

    html += '</fieldset>';
    html += '<footer class="no-padding">';
    html += htmlFooter;
    html += '</footer></div></section></div></div></div>';
    html += '<div class="widget-body">';
    html += '<div class="widget-body no-padding">';
    html += '<div class="smart-form">';
    html += '<header>';
    html += '<h4><i class="glyphicon glyphicon-list-alt"></i>&nbsp;<span data-bind="text: TituloGrid.text"></span></h4>';
    html += '</header></div></div>';
    html += '<table width="100%" class="table table-bordered table-hover" id="' + idDivBusca + '_tabelaEntidades" cellspacing="0"></table>';
    html += '</div></div></div></div>';
    html += '</div>';

    $('body #content').append(html);


    var outraPressionada = false;

    this.OpcaoPadrao = function (callback, tamanho) {
        var editar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: callback, tamanho: tamanho != null ? tamanho : "15", icone: "" };
        var menuOpcoes = new Object();
        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(editar);
        return menuOpcoes;
    }

    this.DefCallback = function (data) {
        if (knoutContent != null) {
            if (knoutContent.requiredClass != null) {
                knoutContent.requiredClass("input");
            }
            if (data != null) {
                knoutContent.codEntity(data.Codigo);
                knoutContent.val(data.Descricao);
            }
            $.each(knoutOpcoes, function (i, prop) {
                if (prop.visible) {
                    if (prop.type == types.map) {
                        prop.val(prop.def);
                    } if (prop.type == types.entity) {
                        prop.codEntity(knoutOpcoes[i].defCodEntity);
                        prop.val(knoutOpcoes[i].def);
                    }
                }
            });
            outraPressionada = false;
            $("#" + idDivBusca).modal('hide');
            $("#" + knoutContent.id).focusNextInputField();
        } else {
            exibirMensagem("atencao", "1 - Componente Incompleto", "Por favor entre em contato com a Multisoftware")
        }
    }

    this.CloseModal = function () {
        $("#" + idDivBusca).modal('hide');
        $("#" + knoutContent.id).focusNextInputField();
    }

    this.OpenModal = function () {
        abrirModalBusca();
    }

    function abrirModalBusca() {
        $('#' + idDivBusca).modal({ keyboard: true, backdrop: 'static' });
    }

    this.AddEvents = function (grid) {
        if (knoutContent != null) {
            $('#' + knoutContent.idBtnSearch).click(function () {
                if (funcaoParametrosDinamicos != null) {
                    funcaoParametrosDinamicos();
                }
                grid.CarregarGrid();
                abrirModalBusca();
            });

            $('#' + idDivBusca).keypress(function (e) {
                var keyCode = e.keyCode || e.which;
                if (keyCode == 13) {
                    if (knoutOpcoes.Pesquisar != null) {
                        $('#' + knoutOpcoes.Pesquisar.id).trigger('click');
                    }
                }
            });

            if (focarNoPrimeiroInput == null || focarNoPrimeiroInput == true)
                $("#" + idDivBusca).on('shown.bs.modal', function (e) {
                    $("#" + idDivBusca + " input, " + "#" + idDivBusca + " select").first().focus();
                });

            $("#" + idDivBusca).on('hidden.bs.modal', function (e) {
                $("#" + knoutContent.id).focus();
            });


        } else {
            exibirMensagem("atencao", "2 - Componente Incompleto", "Por favor entre em contato com a Multisoftware")
        }
    }

    this.AddTabPressEvent = function (funcaoExecutar) {
        if (knoutContent != null) {
            $("#" + knoutContent.id).on('keydown', function (e) {
                var keyCode = e.keyCode || e.which;
                if (keyCode == 9) {
                    if (funcaoParametrosDinamicos != null) {
                        funcaoParametrosDinamicos();
                    }
                    funcaoExecutar(outraPressionada);
                } else {
                    outraPressionada = true;
                }
            });
        } else {
            exibirMensagem("atencao", "3 - Componente Incompleto", "Por favor entre em contato com a Multisoftware")
        }
    }

}


