/// <reference path="libs/jquery-2.0.2.js" />
/// <reference path="Rest.js" />
/// <reference path="CRUD.js" />
/// <reference path="libs/jquery.globalize.js" />
/// <reference path="libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoSumarizacao.js" />

var TypeOptionMenu = { list: "list", link: "link", button: "button" }
var orderDir = { asc: "asc", desc: "desc" }
var ConfigRowsSelect = { permiteSelecao: false, marcarTodos: false }
var Language = {
    "emptyTable": "Nenhum registro encontrado",
    "info": "Exibindo _START_ até _END_ de _TOTAL_ registros",
    "infoEmpty": "",
    "infoFiltered": "(filtered from _MAX_ total entries)",
    "infoPostFix": "",
    "thousands": ".",
    "lengthMenu": "Montrando _MENU_ registros",
    "loadingRecords": "Carregando...",
    "processing": "Processando...",
    "search": "Search:",
    "zeroRecords": "Nenhum registro encontrado",
    "paginate": {
        "first": "Incio",
        "last": "Fim",
        "next": "Próxima",
        "previous": "Anterior"
    },
    "aria": {
        "sortAscending": ": activate to sort column ascending",
        "sortDescending": ": activate to sort column descending"
    }
}


var GridViewExportacao = function (idElemento, url, knout, menuOpcoes, configExportacao, ordenacaoPadrao, quantidadePorPagina, infoMultiplaSelecao, limiteRegistro, infoEditarColuna) {
    return new GridView(idElemento, url, knout, menuOpcoes, ordenacaoPadrao, quantidadePorPagina, null, null, null, infoMultiplaSelecao, limiteRegistro, infoEditarColuna, configExportacao);
};

var GridView = function (idElemento, url, knout, menuOpcoes, ordenacaoPadrao, quantidadePorPagina, callback, mostrarInfo, draggableRows) {
    var callbackEdicaoColuna = null;
    this.SetCallbackEdicaoColunas = function (callback) { //quando é necessário fazer algo espefico ao ocultar ou exibir uma coluna setar esse callback
        callbackEdicaoColuna = callback;
    };

    var permitirEdicaoColunas = false;
    this.SetPermitirEdicaoColunas = function (permitir) {
        permitirEdicaoColunas = permitir;
    };

    this.SetQuantidadeLinhasPorPagina = function (numero) {
        quantidadePorPagina = numero;
    };


    var cacheLastRequest;
    var FnPipeline = function (opts) {
        // Configuration options
        var conf = $.extend({
            pages: 5,     // number of pages to cache
            url: '',      // script url
            data: null,   // function or object with parameters to send to the server
            // matching how `ajax.data` works in DataTables
            method: 'GET', // Ajax HTTP method
            initCacheLastJson: null,
            initCacheLastRequest: null,
            initCacheUpper: null,
            initCacheLower: -1,
            initDadosPesquisa: null
        }, opts);

        // Private variables for storing the cache

        var cacheLower = conf.initCacheLower;
        var cacheUpper = conf.initCacheUpper;
        cacheLastRequest = conf.initCacheLastRequest;
        var cacheLastJson = conf.initCacheLastJson;

        return function (request, drawCallback, settings) {
            var ajax = false;
            var requestStart = request.start;
            var drawStart = request.start;
            var requestLength = request.length;
            var requestEnd = requestStart + requestLength;

            if (settings.clearCache) {
                // API requested that the cache be cleared
                ajax = true;
                settings.clearCache = false;
            }
            else if (cacheLower < 0 || requestStart < cacheLower || requestEnd > cacheUpper) {
                // outside cached data - need to make a request
                ajax = true;
            }
            else if (JSON.stringify(request.order) !== JSON.stringify(cacheLastRequest.order)) {
                // properties changed (ordering, columns, searching)
                ajax = true;
            }

            // Store the request for checking next time around
            cacheLastRequest = $.extend(true, {}, request);

            if (ajax) {
                // Need data from the server
                if (requestStart < cacheLower) {
                    requestStart = requestStart - (requestLength * (conf.pages - 1));

                    if (requestStart < 0) {
                        requestStart = 0;
                    }
                }

                cacheLower = requestStart;
                cacheUpper = requestStart + (requestLength * conf.pages);

                request.start = requestStart;
                request.length = requestLength * conf.pages;

                // Provide the same `data` options as DataTables.
                if ($.isFunction(conf.data)) {
                    // As a function it is executed with the data object as an arg
                    // for manipulation. If an object is returned, it is used as the
                    // data object to submit
                    var d = conf.data(request);
                    if (d) {
                        $.extend(request, d);
                    }
                }
                else if ($.isPlainObject(conf.data)) {
                    // As an object, the data given extends the default
                    $.extend(request, conf.data);
                }
                var dadosPesquisa = conf.initDadosPesquisa;
                dadosPesquisa.Grid = request;
                dadosPesquisa.Grid.header = header;
                dadosPesquisa.Grid.group = group;
                grid = dadosPesquisa.Grid;
                dadosPesquisa.Grid = JSON.stringify(grid)
                settings.jqXHR = executarReST(conf.url, dadosPesquisa, function (jsonData) {
                    if (jsonData.Success) {
                        var json = jsonData.Data;
                        cacheLastJson = $.extend(true, {}, json);

                        if (cacheLower != drawStart) {
                            json.data.splice(0, drawStart - cacheLower);
                        }
                        json.data.splice(requestLength, json.data.length);

                        drawCallback(json);
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", jsonData.Msg);
                    }

                });
            }
            else {
                json = $.extend(true, {}, cacheLastJson);
                json.draw = request.draw; // Update the echo for each response
                json.data.splice(0, requestStart - cacheLower);
                json.data.splice(requestLength, json.data.length);

                drawCallback(json);
            }
        }
    };

    $.fn.dataTable.Api.register('clearPipeline()', function () {
        return this.iterator('table', function (settings) {
            settings.clearCache = true;
        });
    });
    var ColunaOrdenaPadrao;
    var numeroRegistro = 0;
    this.NumeroRegistros = function () {
        return numeroRegistro;
    };

    if (mostrarInfo == null)
        mostrarInfo = true;

    var table;
    var header = new Array();

    var group = { enable: false, propAgrupa: "", dirOrdena: orderDir.desc };
    this.SetGroup = function (groupSer) {
        group = groupSer;
    }

    var grid;
    this.GetGrid = function () {
        return grid;
    }
    this.SetGrid = function (gridSer) {
        grid = gridSer;
        header = grid.header;
        group = grid.group;
        ordenacaoPadrao = { column: grid.indiceColunaOrdena, dir: grid.dirOrdena };
        ColunaOrdenaPadrao = header[grid.indiceColunaOrdena].data;
    }
    
    
    
    this.CarregarGrid = function (loadCallback) {

        var breakpointDefinition = {
            tablet: 1024,
            phone: 480
        };

        var dadosGrid = new Object();
        dadosGrid.draw = 1;
        dadosGrid.start = 0;
        dadosGrid.length = 50;
        dadosGrid.order = new Array();
        dadosGrid.group = group;
        dadosGrid.header = header;
        if (ordenacaoPadrao != null) {
            dadosGrid.order.push({ column: ordenacaoPadrao.column, dir: ordenacaoPadrao.dir });
        } else {
            dadosGrid.order.push({ column: 0, dir: orderDir.desc });
        }


        var dadosPesquisa = RetornarObjetoPesquisa(knout);
        dadosPesquisa.Grid = JSON.stringify(dadosGrid);
        grid = dadosGrid;
        executarReST(url, dadosPesquisa, function (retorno) {
            if (retorno.Success) {
                retorno.Data.header.sort(SortByPosition);
                header = retorno.Data.header;
                var json = retorno.Data;


                json.draw = retorno.Data.draw;
                numeroRegistro = retorno.Data.recordsTotal;
                json.recordsTotal = retorno.Data.recordsTotal;
                json.recordsFiltered = retorno.Data.recordsFiltered;

                var resolucao = window.innerWidth || canvasEl.clientWidth;
                if (resolucao <= breakpointDefinition.phone) {
                    $.each(retorno.Data.header, function (i, head) {
                        if (head.phoneHide) {
                            head.visible = false;
                        }
                    });
                } else if (resolucao <= breakpointDefinition.tablet) {
                    $.each(retorno.Data.header, function (i, head) {
                        if (head.tabletHide) {
                            head.visible = false;
                        }
                    });
                }

                var ListColumnDefs = retornarOpcoes(menuOpcoes, retorno.Data.header.length);

                var pipeline = FnPipeline({
                    url: url,
                    pages: 5,
                    method: "POST",
                    initCacheLastRequest: $.extend(true, {}, dadosGrid),
                    initCacheLastJson: $.extend(true, {}, json),
                    initCacheUpper: grid.start + (10 * 5),
                    initCacheLower: grid.start,
                    initDadosPesquisa: dadosPesquisa
                });

                var initRows = new Array();
                var limit = 5;
                if (quantidadePorPagina != null) {
                    limit = quantidadePorPagina;
                }

                if (json.recordsTotal < limit) {
                    limit = json.recordsTotal;
                }

                for (var i = 0; i < limit; i++) {
                    initRows.push(retorno.Data.data[i]);
                }
                if (loadCallback != null) {
                    loadCallback(retorno.Data);
                }
                var sDom = "<'dt-toolbar'<'col-xs-12 col-sm-6'f><'col-xs-12 col-sm-6'l>r>t<'dt-toolbar-footer'<'col-xs-12 col-sm-6'i><'col-xs-12 col-sm-6'p>>";
                if (!mostrarInfo) {
                    sDom = "<'dt-toolbar'<'col-xs-12 col-sm-6'f><'col-xs-12 col-sm-6'l>r>t<'dt-toolbar-footer'<'col-xs-12 col-sm-0'i><'col-xs-12 col-sm-12'p>>";
                }



                $('#' + idElemento).html("");
                table = $('#' + idElemento).DataTable({
                    "processing": false,
                    "serverSide": true,
                    "destroy": true,
                    "ajax": pipeline,
                    "data": initRows,
                    "autoWidth": false,
                    "sDom": sDom,
                    "iDisplayLength": limit,
                    "order": [[dadosGrid.order[0].column, dadosGrid.order[0].dir]],
                    "columns": retorno.Data.header,
                    "deferLoading": retorno.Data.recordsTotal,
                    "columnDefs": ListColumnDefs,
                    "bFilter": false, "bLengthChange": false, "paging": true,
                    "info": mostrarInfo, "search": false,
                    "language": Language,
                    "colReorder": permitirEdicaoColunas ? {
                        "reorderCallback": function () {
                            header = ReorganizarColunas(table, header);
                            header.sort(SortByPosition);

                            cacheLastRequest.order[0].column = table.context[0].aaSorting[0][0];
                            cacheLastRequest.order[0].dir = table.context[0].aaSorting[0][1];
                            grid.header = header;
                            if (ordenacaoPadrao != null)
                                ordenacaoPadrao.column = buscarIndiceDataColuna(header, ColunaOrdenaPadrao);
                            else
                                ordenacaoPadrao = { column: table.context[0].aaSorting[0][0], dir: table.context[0].aaSorting[0][1] };

                            dadosGrid.order = cacheLastRequest.order;
                        }
                    } : false,
                    "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                        adicionarToolTip(nRow, idElemento, false);
                        if (aData.DT_RowColor != "") {
                            $(nRow).css('background-color', aData.DT_RowColor)
                        };
                        if (draggableRows == true) {
                            $(nRow).draggable({
                                helper: "clone",
                                revert: true,
                                cursor: "move"
                            });
                        }
                    },
                    "drawCallback": group.enable ? function (settings) {
                        var api = this.api();
                        var rows = api.rows({ page: 'current' }).nodes();
                        var last = null;

                        var numeroCabecalho = 0;
                        $.each(retorno.Data.header, function (i, cabecalho) {
                            if (cabecalho.visible == null || cabecalho.visible) {
                                numeroCabecalho++;
                            } 
                        });

                        api.column(buscarIndiceDataColuna(header, group.propAgrupa), { page: 'current' }).data().each(function (grupo, i) {
                            if (last !== grupo) {
                                $(rows).eq(i).before(
                                    '<tr class="group" style="background-color:#C0C0C0"><td colspan="' + numeroCabecalho + '"><b>' + grupo + '</b></td></tr>'
                                );
                                last = grupo;
                            }
                        });
                    } : null
                });

                if (permitirEdicaoColunas) {
                    var html = "<div>";
                    $.each(header, function (i, cabecalho) {
                        var style = "text-decoration:line-through";
                        if (cabecalho.visible == null || cabecalho.visible) {
                            style = "";
                        }
                        html += '<a class="toggle-vis" style="' + style + '; cursor:pointer; margin-top:10px" data-column="' + cabecalho.data + '">' + cabecalho.title + '</a>&nbsp;&nbsp;';
                    });
                    html += "</div>";
                    $('#' + idElemento).before(html);

                    $('a.toggle-vis').on('click', function (e) {
                        e.preventDefault();
                        var indice;
                        var valor = $(this).attr('data-column');
                        $.each(header, function (i, cabecalho) {
                            if (cabecalho.data == valor) {
                                indice = cabecalho.position;
                                return false;
                            }
                        });
                        var column = table.column(indice);
                        column.visible(!column.visible());

                        header[column[0]].visible = column.visible();
                        if (header[column[0]].visible) {
                            $(this).css("text-decoration", "none");
                        }
                        else {
                            $(this).css("text-decoration", "line-through");
                        }
                        grid.header = header;
                        table.draw();

                        if (callbackEdicaoColuna != null)
                            callbackEdicaoColuna();
                    });
                }

                adicionarEventOpcoes(table, idElemento, menuOpcoes);
                if (callback != null) {
                    callback();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                $('#' + idElemento).html("<thead><tr><th></th>t<tr></thead><tbody><tr><td class='dataTables_empty'>Falha ao preencher a tabela.</td></tr></tbody>");

            }
        });

    };

    this.GridViewTable = function () {
        return table;
    };

}

function buscarIndiceDataColuna(header, propAgrupar) {
    var indice = 0;
    $.each(header, function (i, cabecalho) {
        if (cabecalho.data == propAgrupar) {
            indice = cabecalho.position;
            return false;
        }
    });
    return indice;
}

function ReorganizarColunas(table, header) {
    if (table != null) {
        $.each(table.colReorder.order(), function (i, ordem) {
            header[ordem].position = i;
        });
    }
    return header;
}

function SortByPosition(a, b) {
    var aName = a.position;
    var bName = b.position;
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
var GridReordering = function (info, idContent, headHTML, bodyHTML, callback) {
    var dataTable;
    var headHTML;
    var HTMLBase = '<table width="100%" class="table table-bordered table-hover" id="tableorder_' + idContent + '" >';
    HTMLBase += '<thead>' + headHTML + '</thead>';
    HTMLBase += '<tbody></tbody>';
    HTMLBase += '</table>';
    $("#" + idContent).html(HTMLBase);
    $("#tableorder_" + idContent + " tbody").html(bodyHTML);

    dataTable = this.CarregarGrid = function () {
        dataTable = $("#tableorder_" + idContent).dataTable({
            "destroy": true,
            "bSort": true,
            "bFilter": false, "bLengthChange": false, "paging": false,
            "info": true, "search": false, "language": { "info": info, "zeroRecords": "Nenhum registro encontrado", "infoEmpty": "" }
        }).rowReordering();
    }

    this.ObterOrdencao = function () {
        var lista = new Array();
        $(dataTable.fnGetNodes()).each(function () {
            var item = new Object();
            item.id = this.id;
            item.posicao = parseInt(dataTable.fnGetData(this, 0));
            lista.push(item);
        });
        return lista;
    }

    this.RecarregarGrid = function (bodyHTML) {
        $("#" + idContent).html("");
        $("#" + idContent).html(HTMLBase);
        $("#tableorder_" + idContent + " tbody").html(bodyHTML);
        this.CarregarGrid();
    }

    this.LimparGrid = function () {
        $("#" + idContent).html("");
        $("#" + idContent).html(HTMLBase);
        $("#tableorder_" + idContent + " tbody").html("");
        this.CarregarGrid();
    }

}

var BasicDataTable = function (idElemento, header, menuOpcoes, ordenacaoPadrao, configRowsSelect) {
    var table;
    var listaSelecionadas = new Array();
    this.CarregarGrid = function (data) {

        if (configRowsSelect != null)
            ConfigRowsSelect = configRowsSelect;
        else
            ConfigRowsSelect = { permiteSelecao: false, marcarTodos: false };

        if (ConfigRowsSelect.permiteSelecao) {
            var colunaSelecao = {
                title: '<input type="checkbox" name="select_all">',
                render: function (data, type, row) {
                    return '<input type="checkbox" class="editor-active">';
                }, className: "text-align-center", width: "5%", orderable: false
            }
            header.unshift(colunaSelecao);
        }

        var ListColumnDefs = retornarOpcoes(menuOpcoes, header.length);

        order = new Array();
        if (ordenacaoPadrao != null) {
            order.push({ column: ordenacaoPadrao.column, dir: ordenacaoPadrao.dir });
        } else {
            order.push({ column: 0, dir: orderDir.desc });
        }
        $('#' + idElemento).html("");
        table = $('#' + idElemento).DataTable({
            "destroy": true,
            "data": data,
            "bAutoWidth": false,
            "columns": header,
            "columnDefs": ListColumnDefs,
            "order": [[order[0].column, order[0].dir]],
            "bFilter": false, "bLengthChange": false, "paging": true,
            "info": true, "search": false,
            "language": Language,
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                adicionarToolTip(nRow, idElemento, true);
                if (aData.DT_RowColor != "") {
                    $(nRow).css('background-color', aData.DT_RowColor)
                }
            }
        });
        adicionarEventOpcoes(table, idElemento, menuOpcoes);

        if (ConfigRowsSelect.permiteSelecao) {
            $("#" + idElemento + ' tbody td').css("cursor", "pointer");
            $("#" + idElemento + ' tbody').on('click', 'input[type="checkbox"]', function (e) {
                var $row = $(this).closest('tr');
                var data = table.row($row).data();
                if (this.checked) {
                    AdicionarElementoBasicTable(data, listaSelecionadas);
                    if (table.context.length == listaSelecionadas.length) {
                        $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", true);
                    }
                    $row.css("background-color", "rgba(181, 213, 179, 1)");

                } else {
                    RemoverElementoBasicTable(data, listaSelecionadas);
                    $row.css('background-color', "");
                    $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", false);
                }
            });
            $("#" + idElemento).on('click', 'tbody td', function (e) {
                var numeroCabecalho = 0;
                var cell = $(this);
                $.each(header, function (i, cabecalho) {
                    if (cabecalho.visible == null || cabecalho.visible) {
                        numeroCabecalho++;
                    }
                });
                if (cell[0].cellIndex < numeroCabecalho) {
                    $(this).parent().find('input[type="checkbox"]').trigger('click');
                }
            });
            $("#" + idElemento + ' thead input[name="select_all"]').on('click', function (e) {
                if (this.checked) {
                    $("#" + idElemento + ' tbody input[type="checkbox"]:not(:checked)').trigger('click');
                } else {
                    $("#" + idElemento + ' input[type="checkbox"]:checked').trigger('click');
                }
                e.stopPropagation();
            });
            if (ConfigRowsSelect.marcarTodos)
                $("#" + idElemento + ' thead input[name="select_all"]').trigger('click');
        }
    };
    this.BasicTable = function () {
        return table;
    };
    this.ListaSelecionados = function () {
        return listaSelecionadas;
    }
}


function AdicionarElementoBasicTable(data, listaSelecionadas) {
    listaSelecionadas.push(data);
}

function RemoverElementoBasicTable(data, listaSelecionadas) {
    if (data.Codigo != null) {
        $.each(listaSelecionadas, function (i, obj) {
            if (obj.Codigo == data.Codigo) {
                listaSelecionadas.splice(i, 1);
                return false;
            }
        });
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha de Componente", "O componente está incompleto, por favor, entre em contato com a Multisoftware");
    }
}


function adicionarToolTip(nRow, idElemento, basicTable) {
    $.each(nRow.cells, function (i, col) {
        if ($(col).children().length == 0) { // não exibir tooltip quando for opções da grid
            var sTip = $(col).text();
            this.setAttribute('title', sTip);
            if (!basicTable) { // todo: ver porque quando é basic table não funciona o tooltip jquery
                this.setAttribute('data-toggle', 'tooltip');
                this.setAttribute('data-container', '#' + idElemento);
                $(this).tooltip({
                    track: true,
                    delay: 500
                });
            }
        } else {
            if ($(this)[0].firstChild.localName == "div")
                $(this).css("overflow", "visible");
        }
    });
}


function adicionarEventOpcoes(table, idElemento, menuOpcoes) {
    $('#' + idElemento + ' tbody').unbind('click');
    if (menuOpcoes != null) {
        if (menuOpcoes.tipo == TypeOptionMenu.link) {
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                $('#' + idElemento + ' tbody').on('click', 'a.' + opcao.id, function () {
                    var data = table.row($(this).parents('tr')).data();
                    opcao.metodo(data);
                });
            });
        } if (menuOpcoes.tipo == TypeOptionMenu.list) {
            //' + opcao.id + '_' + i + '
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                $('#' + idElemento + ' tbody').on('click', 'a.' + opcao.id, function () {
                    var data = table.row($(this).parents('tr')).data();
                    opcao.metodo(data);
                });
            });
        }
        if (menuOpcoes.tipo == TypeOptionMenu.button) {
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                $('#' + idElemento + ' tbody').on('click', 'button.' + opcao.id, function () {
                    var data = table.row($(this).parents('tr')).data();
                    opcao.metodo(data);
                });
            });
        }
    }
}

function retornarOpcoes(menuOpcoes, colunasCabecalho) {
    var ListColumnDefs = new Array();
    if (menuOpcoes != null) {

        if (menuOpcoes.tipo == TypeOptionMenu.link) {
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                var colDef = new Object();
                colDef.targets = [colunasCabecalho + i];
                colDef.data = null;
                colDef.orderable = false;
                colDef.title = opcao.descricao;
                colDef.className = "sorting_disabled_opcao text-align-center";
                if (opcao.tamanho != null) {
                    colDef.width = opcao.tamanho + "%";
                }
                colDef.defaultContent = "<a href='javascript:;' class='" + opcao.id + "'>" + opcao.descricao + "</a>";
                ListColumnDefs.push(colDef);
            });
        } else {
            if (menuOpcoes.tipo == TypeOptionMenu.list) {
                var html = '<div class="btn-group btn-block">';
                html += '<button class="btn btn-default btn-xs btn-block dropdown-toggle" data-toggle="dropdown"><i class="glyphicon glyphicon-list"></i>&nbsp;<i class="caret"></i></button>';
                html += '<ul class="dropdown-menu" role="menu" style="left: auto; right: 0px;">';
                $.each(menuOpcoes.opcoes, function (i, opcao) {
                    html += '<li><a style="cursor: pointer;" class="' + opcao.id + '">' + opcao.descricao + '</a></li>';
                });
                html += '</ul></div>';

                var colDef = new Object();
                colDef.targets = [colunasCabecalho];
                colDef.data = null;
                colDef.orderable = false;
                colDef.title = menuOpcoes.descricao;
                colDef.className = "sorting_disabled_opcao text-align-center";
                if (menuOpcoes.tamanho != null) {
                    colDef.width = menuOpcoes.tamanho + "%";
                }
                colDef.defaultContent = html;
                ListColumnDefs.push(colDef);
            } else {
                if (menuOpcoes.tipo == TypeOptionMenu.button) {
                    $.each(menuOpcoes.opcoes, function (i, opcao) {
                        var colDef = new Object();
                        colDef.targets = [colunasCabecalho + i];
                        colDef.data = null;
                        colDef.orderable = false;
                        colDef.title = opcao.descricao;
                        colDef.className = "sorting_disabled_opcao text-align-center";
                        if (opcao.tamanho != null) {
                            colDef.width = opcao.tamanho + "%";
                        }
                        colDef.defaultContent = "<button class='btn btn-default botaoGrid " + opcao.id + "'>" + opcao.descricao + "</button>";
                        ListColumnDefs.push(colDef);
                    });
                }
            }
        }
    }
    return ListColumnDefs;
}



