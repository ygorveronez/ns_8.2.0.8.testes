var paginaAtual = 1;
var paginaAux;
var paginaAtualServidor = 1;
var quantidadeRegistrosPagina = 50;
var quantidadeMostrar = 10;
var jogarParaoUltimo = false;

function CriarGridView(urlDados, dados, idtabela, divContainer, divPaginacao, colunasNovas, colunasParaEsconder, dadosProntos, colunasEsconderMobile, _quantidadeRegistrosPagina) {

    if (urlDados != null || dadosProntos != null) {
        ResetarPaginacao();
    }

    if (_quantidadeRegistrosPagina != null)
        quantidadeRegistrosPagina = _quantidadeRegistrosPagina;
    else
        quantidadeRegistrosPagina = 50;

    if (urlDados)
        $('#' + divContainer).data('urlConsulta', urlDados);

    if (dados)
        $('#' + divContainer).data('dadosJson', dados);
    else {
        var dadosJson = $('#' + divContainer).data('dadosJson');
        if (dadosJson != null) {
            dadosJson.inicioRegistros = (paginaAtualServidor - 1) * quantidadeRegistrosPagina;
            $('#' + divContainer).data('dadosJson', dadosJson);
        }
    }

    if (idtabela)
        $('#' + divContainer).data('idTabela', idtabela);

    if (divPaginacao)
        $('#' + divContainer).data('divPaginacao', divPaginacao);

    if (colunasNovas)
        $('#' + divContainer).data('colunasAdicionais', colunasNovas);

    if (colunasParaEsconder)
        $('#' + divContainer).data('colunasParaEsconder', colunasParaEsconder);

    if (colunasEsconderMobile)
        $('#' + divContainer).data('colunasEsconderMobile', colunasEsconderMobile);

    if (!dadosProntos)
        executarRest($('#' + divContainer).data('urlConsulta'), $('#' + divContainer).data('dadosJson'), function (data) {
            RenderizarTabela(data, divContainer);
        });
    else
        RenderizarTabela(dadosProntos, divContainer);
}

function RenderizarTabela(dados, divContainer) {
    if (dados.Sucesso) {
        var html = '<table id="' + $('#' + divContainer).data('idTabela') + '" class="table table-bordered table-hover table-condensed table-striped" style="table-layout: fixed;"><thead><tr>';
        var tamanhoTotalColunas = 0;
        for (var col in dados.Campos) {
            var valores = dados.Campos[col];
            if (typeof valores == "string")
                valores = valores.split('|');
            else
                valores = [];
            if (valores.length == 2) {
                html += "<th style='width:" + valores[1] + "%; max-width: 100px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>" + valores[0] + "</th>";
                tamanhoTotalColunas += parseInt(valores[1]);
            }
            else
                html += "<th>" + dados.Campos[col] + "</th>";
        }
        $('#' + divContainer).data('tamanhoTotalColunas', tamanhoTotalColunas);
        html += "<th class='jsonObject' style='display: none;'>JSON</th>";
        html += '</tr></thead><tbody>';

        for (var i = 0; i < dados.Objeto.length; i++) {
            html += "<tr>";
            for (var item in dados.Objeto[i]) {
                if (typeof dados.Objeto[i][item] != 'object') {
                    if (typeof dados.Objeto[i][item] == 'string') {
                        if (dados.Objeto[i][item].indexOf("\/Date(") >= 0) {
                            var date = jQuery.globalEval(dados.Objeto[i][item].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                            dados.Objeto[i][item] = Globalize ? Globalize.format(date, 'dd/MM/yyyy') : date.toString();
                        }
                    }
                    if (typeof dados.Objeto[i][item] != 'number')
                        html += "<td title='" + dados.Objeto[i][item] + "' style='overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
                    else
                        html += "<td title='" + dados.Objeto[i][item] + "' style='text-align: center; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
                }
                else
                    html += "<td title='" + dados.Objeto[i][item] + "' style='overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
            }
            html += "<td class='jsonObject' style='display: none;'>" + JSON.stringify(dados.Objeto[i]) + "</td>";
            html += "</tr>";
        }

        html += '</tbody></table>';

        $('#' + divContainer)[0].innerHTML = html;

        FormatarTabela(divContainer, dados.Objeto.length);

        CriarPaginacao(dados.TotalRegistros, divContainer);
    }
    else
        //TODO Verificar a melhor forma de tratar o erro.
        jAlert(dados.Erro, 'Erro');
}

function FormatarTabela(divContainer, registros) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var colunasEsconder = $('#' + divContainer).data('colunasParaEsconder');
    var colunasEsconderMobile = $('#' + divContainer).data('colunasEsconderMobile');
    var divPaginacao = $('#' + divContainer).data('divPaginacao');
    var $container = $('#' + idtabela);

    if (divPaginacao) {
        $container.dataTable({
            "bPaginate": true,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "iDisplayLength": quantidadeMostrar,
            "aoColumnDefs": [{ "sClass": "hidden-xs", "aTargets": (colunasEsconderMobile != null ? colunasEsconderMobile : []) }]
        });
    }
    else {
        $container.dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "aoColumnDefs": [{ "sClass": "hidden-xs", "aTargets": (colunasEsconderMobile != null ? colunasEsconderMobile : []) }]
        });
    }
    
    //Colunas Hide;
    var oTable = $('#' + idtabela).dataTable();
    for (var indice in colunasEsconder) {
        oTable.fnSetColumnVis(colunasEsconder[indice], false, true);
    }

    var paginasConsulta = quantidadeRegistrosPagina / quantidadeMostrar;

    if (paginaAtual > paginaAux) {
        paginaAux = paginaAtual % paginasConsulta;
        if (paginaAux == 0)
          paginaAux = paginaAtual - paginaAux;
        for (var i = paginaAux; i > 1; i--) {
            oTable.fnPageChange('next');
        }
    }
    if (jogarParaoUltimo) {
        oTable.fnPageChange('last');
        jogarParaoUltimo = false;
        paginaAux = paginaAux - (paginaAux % paginasConsulta);
        if (paginaAtual < paginaAux) {
            for (var i = paginaAtual; i < paginaAux; i++) {
                oTable.fnPageChange('previous');
            }
        }
    }

    //colunas extras;
    if (registros > 0)
        AdicionarColunas(true, divContainer);

    // Corrige o numero de colunas quando nao ha registros
    var totalDeColunas = $container.find("thead th").length;
    if ($container.find(".dataTables_empty").length > 0)
        $container.find(".dataTables_empty").attr("colspan", totalDeColunas - 1);
}


function AdicionarColunas(adicionarHeader, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var colunasAdicionais = $('#' + divContainer).data('colunasAdicionais');

    var quantidadeColunasHeader = $('#' + idtabela + ' thead tr th').length;
    var quantidadeColunasBody = $('#' + idtabela + ' tbody tr:eq(0) td').length;

    if (adicionarHeader == true || (quantidadeColunasBody != quantidadeColunasHeader)) {
        var tamanhoDeCadaColunaAdicional = (100 - $('#' + divContainer).data('tamanhoTotalColunas'));

        var nCloneTd = document.createElement('td');
        nCloneTd.setAttribute("style", "text-overflow: ellipsis; white-space: nowrap;")

        if (adicionarHeader && colunasAdicionais != null) {
            var nCloneTh = document.createElement('th');
            nCloneTh.innerHTML = 'Opções';
            nCloneTh.style.width = tamanhoDeCadaColunaAdicional.toFixed(0).toString() + "%";
            nCloneTh.style.maxWidth = "100px";
            nCloneTh.style.overflow = "hidden";
            nCloneTh.style.textOverflow = "ellipsis";
            nCloneTh.style.whiteSpace = "nowrap";
            $('#' + idtabela + ' thead tr').each(function () {
                this.appendChild(nCloneTh);
            });
        }

        if (colunasAdicionais != null) {
            if (colunasAdicionais.length == 1) {
                var oTable = $('#' + idtabela).dataTable();
                $('#' + idtabela + ' tbody tr').each(function () {
                    var objeto = JSON.parse($(this).find("td[class='jsonObject']").html());

                    var button = document.createElement("button");
                    button.type = "button";
                    button.className = "btn btn-default btn-xs btn-block";
                    button.innerHTML = colunasAdicionais[0].Descricao;
                    $(button).bind('click', objeto, colunasAdicionais[0].Evento);

                    var td = nCloneTd.cloneNode(true);
                    td.appendChild(button);

                    this.appendChild(td);
                });

            } else {

                var oTable = $('#' + idtabela).dataTable();

                $('#' + idtabela + ' tbody tr').each(function () {
                    var ulDropdown = document.createElement("ul");
                    ulDropdown.className = "dropdown-menu";
                    ulDropdown.style.left = "auto";
                    ulDropdown.style.right = "0";
                    ulDropdown.setAttribute("role", "menu");

                    for (var indice in colunasAdicionais) {
                        var objeto = JSON.parse($(this).find("td[class='jsonObject']").html());

                        var liDropdown = document.createElement("li");
                        var aDropdown = document.createElement("a");
                        aDropdown.innerHTML = colunasAdicionais[indice].Descricao;
                        aDropdown.style.cursor = "pointer";

                        $(aDropdown).bind('click', objeto, colunasAdicionais[indice].Evento);

                        liDropdown.appendChild(aDropdown);
                        ulDropdown.appendChild(liDropdown);
                    }

                    var buttonDropdown = document.createElement("button");
                    buttonDropdown.className = "btn btn-default btn-xs btn-block dropdown-toggle";
                    buttonDropdown.type = "button";
                    buttonDropdown.setAttribute("data-toggle", "dropdown");
                    buttonDropdown.innerHTML = '<span class="glyphicon glyphicon-th-list"></span> <span class="caret"></span>';

                    var divDropdown = document.createElement("div");
                    divDropdown.className = "btn-group btn-block";
                    divDropdown.appendChild(buttonDropdown);
                    divDropdown.appendChild(ulDropdown);

                    var td = nCloneTd.cloneNode(true);
                    td.appendChild(divDropdown);

                    this.appendChild(td);
                });

            }
        }
    }
}
function ResetarPaginacao() {
    paginaAtual = 1;
    paginaAtualServidor = 1;
    paginaAux = undefined;
}
function CriarPaginacao(totalRegistros, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var paginasTotal = Math.ceil(totalRegistros / quantidadeMostrar);
    var html = '<label class="hidden-xs pull-left">Mostrando ' + (paginasTotal <= 0 ? "0" : paginaAtual) + ' de ' + paginasTotal + '</label><ul class="pagination pull-right" style="margin: 0;">';
    if (paginaAtual != 1) {
        html += '<li><a href="javascript:Paginar(' + totalRegistros + ', 1, \'' + divContainer + '\');">Primeiro</a></li>';
        html += '<li><a href="javascript:Paginar(' + totalRegistros + ', ' + (paginaAtual - 1) + ', \'' + divContainer + '\');">Anterior</a></li>';
    }
    else {
        html += '<li class="disabled"><a href="javascript:void(0);">Primeiro</a></li>';
        html += '<li class="disabled"><a href="javascript:void(0);">Anterior</a></li>';
    }

    html += CriarListaPaginas(paginasTotal, totalRegistros, divContainer);

    if ((paginaAtual + 1) <= paginasTotal) {
        html += '<li><a href="javascript:Paginar(' + totalRegistros + ', ' + (paginaAtual + 1) + ', \'' + divContainer + '\');">Próximo</a></li>';
        html += '<li><a href="javascript:Paginar(' + totalRegistros + ', ' + paginasTotal + ', \'' + divContainer + '\');">Último</a></li>';
    }
    else {
        html += '<li class="disabled"><a href="javascript:void(0);">Próximo</a></li>';
        html += '<li class="disabled"><a href="javascript:void(0);">Último</a></li>';
    }
    html += '</ul>';
    $('#' + $('#' + divContainer).data('divPaginacao')).html(html);
}

function CriarListaPaginas(paginasTotal, totalRegistros, divContainer) {
    var html = '';
    var valorMaximo = quantidadeRegistrosPagina == 50 ? 5 : 4;
    var contador = 0;
    var pagina = paginaAtual - (quantidadeRegistrosPagina == 50 ? 2 : 1);

    if (paginasTotal == 1)
        html = '<li class="active"><a href="javascript:void(0);">1</a></li>';
    else {
        var indice = 0;
        for (var i = pagina; (indice < valorMaximo) && (i <= paginasTotal) ; i++) {
            if (i > 0) {
                indice++;
                if (i == paginaAtual)
                    html += '<li class="active"><a href="javascript:void(0);">' + paginaAtual + '</a></li>';
                else
                    html += '<li><a href="javascript:Paginar(' + totalRegistros + ', ' + i + ', \'' + divContainer + '\');">' + i + '</a></li>';
            }
        }
    }
    return html;
}

function Paginar(totalRegistros, pagina, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var paginasMostrar = Math.ceil(totalRegistros / quantidadeMostrar);
    var paginasTotal = Math.ceil(totalRegistros / quantidadeRegistrosPagina);
    paginaAux = paginaAtual;
    if (pagina > paginaAtual) {
        paginaAtual = pagina;
        if ((quantidadeRegistrosPagina * paginaAtualServidor) >= (paginaAtual * quantidadeMostrar)) {
            oTable = $('#' + idtabela).dataTable();
            for (var i = paginaAux; i < paginaAtual; i++) {
                oTable.fnPageChange('next');
            }
            CriarPaginacao(totalRegistros, divContainer);
            AdicionarColunas(false, divContainer);
        }
        else {
            var NumeroPagina = quantidadeRegistrosPagina / quantidadeMostrar;
            NumeroPagina = paginaAtual / NumeroPagina;
            paginaAtualServidor = Math.ceil(NumeroPagina);
            if (paginaAtual == paginasMostrar)
                jogarParaoUltimo = true;
            CriarGridView(null, null, null, divContainer, null, null, null, null, null, quantidadeRegistrosPagina);
        }
    }
    else {
        paginaAtual = pagina;
        if (((quantidadeRegistrosPagina * paginaAtualServidor) - (paginaAtual * quantidadeMostrar)) < quantidadeRegistrosPagina) {
            oTable = $('#' + idtabela).dataTable();
            for (var i = paginaAux; i > paginaAtual; i--) {
                oTable.fnPageChange('previous');
            }
            CriarPaginacao(totalRegistros, divContainer);
            AdicionarColunas(false, divContainer);
        }
        else {
            jogarParaoUltimo = true;
            var NumeroPagina = quantidadeRegistrosPagina / quantidadeMostrar;
            NumeroPagina = paginaAtual / NumeroPagina;
            paginaAtualServidor = Math.ceil(NumeroPagina);
            CriarGridView(null, null, null, divContainer, null, null, null, null, null, quantidadeRegistrosPagina);
        }
    }
}