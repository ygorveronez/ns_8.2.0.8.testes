var paginaAtual = 1;
var paginaAux;
var paginaAtualServidor = 1;
var quantidadeRegistrosPagina = 50;
var quantidadeMostrar = 10;
var jogarParaoUltimo = false;

function CriarGridView(urlDados, dados, idtabela, divContainer, divPaginacao, colunasNovas, colunasParaEsconder, dadosProntos, agruparOpcoes) {
    if (urlDados != null || dadosProntos != null) {
        ResetarPaginacao();
    }
    if (urlDados) {
        var path = "";
        if (document.location.pathname.split("/").length > 1) {
            var paths = document.location.pathname.split("/");
            for (var i = 0; (paths.length - 1) > i; i++) {
                if (paths[i] != "") {
                    path += "/" + paths[i];
                }
            }
        }
        $('#' + divContainer).data('urlConsulta', document.location.protocol + "//" + document.location.host + path + urlDados);
    }
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
    if (agruparOpcoes)
        $('#' + divContainer).data('agruparOpcoes', agruparOpcoes);

    if (!dadosProntos) {
        $.ajax({
            type: "POST",
            url: $('#' + divContainer).data('urlConsulta'),
            data: $('#' + divContainer).data('dadosJson'),
            dataType: 'jsonp',
            success: function (data) {
                if (data) {
                    if (data.SessaoExpirada) {
                        location.href = "Logout.aspx";
                    }
                }
                RenderizarTabela(data, divContainer);
            },
            error: function (erro) {
                //jAlert(erro.statusText, 'Erro');
                location.href = 'Logon.aspx?ReturnUrl=' + document.location.pathname;
            }
        });
    }
    else
        RenderizarTabela(dadosProntos, divContainer);
}

function RenderizarTabela(dados, divContainer) {
    if (dados.Sucesso) {
        var html = '<table id="' + $('#' + divContainer).data('idTabela') + '" style="table-layout:fixed;"><thead><tr class="topoGrid">';
        for (var col in dados.Campos) {
            var valores = dados.Campos[col].split('|');
            if (valores.length == 2)
                html += "<th style='width:" + valores[1] + "%;'>" + valores[0] + "</th>";
            else
                html += "<th>" + dados.Campos[col] + "</th>";
        }
        html += "<th class='jsonObject' style='display:none;'>Json Object Coluna</th>";
        html += '</tr></thead><tbody>';

        for (var i = 0; i < dados.Objeto.length; i++) {
            html += "<tr class='linha'>";
            for (var item in dados.Objeto[i]) {
                if (typeof dados.Objeto[i][item] != 'object') {
                    if (typeof dados.Objeto[i][item] == 'string') {
                        if (dados.Objeto[i][item].indexOf("\/Date(") >= 0) {
                            var date = jQuery.globalEval(dados.Objeto[i][item].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                            dados.Objeto[i][item] = date.format('dd/MM/yyyy');
                        }
                    }
                    if (typeof dados.Objeto[i][item] != 'number')
                        html += "<td title='" + dados.Objeto[i][item] + "' style='overflow: hidden;text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
                    else
                        html += "<td title='" + dados.Objeto[i][item] + "' style='text-align:center;overflow: hidden;text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
                }
                else
                    html += "<td title='" + dados.Objeto[i][item] + "' style='overflow: hidden;text-overflow: ellipsis; white-space: nowrap;'>" + dados.Objeto[i][item] + "</td>";
            }
            html += "<td class='jsonObject' style='display:none;'>" + JSON.stringify(dados.Objeto[i]) + "</td>";
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
    var agruparOpcoes = $('#' + divContainer).data('agruparOpcoes');
    var colunasEsconder = $('#' + divContainer).data('colunasParaEsconder');
    var divPaginacao = $('#' + divContainer).data('divPaginacao');

    if (divPaginacao) {
        $('#' + idtabela).dataTable({
            "bPaginate": true,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "iDisplayLength": quantidadeMostrar
        });
    }
    else {
        $('#' + idtabela).dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false
        });
    }


    //Colunas Hide;
    var oTable = $('#' + idtabela).dataTable();
    for (var indice in colunasEsconder) {
        oTable.fnSetColumnVis(colunasEsconder[indice], false, true);
    }
    if (paginaAtual > paginaAux) {
        paginaAux = paginaAtual % 5;
        for (var i = paginaAux; i > 1; i--) {
            oTable.fnPageChange('next');
        }
    }
    if (jogarParaoUltimo) {
        oTable.fnPageChange('last');
        jogarParaoUltimo = false;
        paginaAux = paginaAux - paginaAux % 5;
        if (paginaAtual < paginaAux) {
            for (var i = paginaAtual; i < paginaAux; i++) {
                oTable.fnPageChange('previous');
            }
        }
    }

    //colunas extras;
    if (registros > 0)
        AdicionarColunas(true, divContainer, agruparOpcoes);
}

function AdicionarColunas(adicionarHeader, divContainer, agruparOpcoes) {
    if (agruparOpcoes)
        return AdicionarColunasAgrupadas(adicionarHeader, divContainer);
    else
        return AdicionarColunasSeparado(adicionarHeader, divContainer);
}

function AdicionarColunasSeparado(adicionarHeader, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var colunasAdicionais = $('#' + divContainer).data('colunasAdicionais');

    var quantidadeColunasHeader = $('#' + idtabela + ' thead tr th').length;
    var quantidadeColunasBody = $('#' + idtabela + ' tbody tr:eq(0) td').length;

    if (adicionarHeader == true || (quantidadeColunasBody != quantidadeColunasHeader)) {

        for (var indice in colunasAdicionais) {

            var nCloneTd = document.createElement('td');
            nCloneTd.className = 'opcoes';
            nCloneTd.innerHTML = '<a>' + colunasAdicionais[indice].Descricao + '</a>';

            if (adicionarHeader) {
                var nCloneTh = document.createElement('th');
                nCloneTh.innerHTML = 'Opções';
                $('#' + idtabela + ' thead tr').each(function () {
                    this.appendChild(nCloneTh);
                });
            }

            var oTable = $('#' + idtabela).dataTable();
            $('#' + idtabela + ' tbody tr').each(function () {
                var td = nCloneTd.cloneNode(true);
                var objeto = JSON.parse($(this).find("td[class='jsonObject']").html());
                $(td).bind('click', objeto, colunasAdicionais[indice].Evento);
                this.appendChild(td);
            });
        }
    }
}


function AdicionarColunasAgrupadas(adicionarHeader, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var colunasAdicionais = $('#' + divContainer).data('colunasAdicionais');

    var quantidadeColunasHeader = $('#' + idtabela + ' thead tr th').length;
    var quantidadeColunasBody = $('#' + idtabela + ' tbody tr:eq(0) td').length;

    if (adicionarHeader == true || (quantidadeColunasBody != quantidadeColunasHeader)) {
        var tamanhoDeCadaColunaAdicional = (100 - $('#' + divContainer).data('tamanhoTotalColunas'));

        var nCloneTd = document.createElement('td');
        nCloneTd.setAttribute("style", "text-overflow: ellipsis; white-space: nowrap;")

        if (adicionarHeader) {
            var nCloneTh = document.createElement('th');
            nCloneTh.innerHTML = 'Opções';
            nCloneTh.style.width = "3%";
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
                    buttonDropdown.className = "btn btn-default btn-xs btn-block dropdown-toggle text-right";
                    buttonDropdown.type = "button";
                    buttonDropdown.setAttribute("data-toggle", "dropdown");
                    buttonDropdown.innerHTML = '<span class="menu-list">' + '<span class="bar"></span><span class="bar"></span><span class="bar"></span>' + '</span> <span class="caret"></span>';

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
    var html = '<span>';
    html += '<label>Mostrando ' + paginaAtual + ' de ' + paginasTotal + '</label>';
    if (paginaAtual != 1) {
        html += '<a href="javascript:Paginar(' + totalRegistros + ', 1, \'' + divContainer + '\');">Primeiro</a>';
        html += '<a href="javascript:Paginar(' + totalRegistros + ', ' + (paginaAtual - 1) + ', \'' + divContainer + '\');">Anterior</a>';
    }
    else {
        html += '<a href="#" class="aspNetDisabled">Primeiro</a>';
        html += '<a href="#" class="aspNetDisabled">Anterior</a>';
    }

    html += CriarListaPaginas(paginasTotal, totalRegistros, divContainer);

    if ((paginaAtual + 1) <= paginasTotal) {
        html += '<a href="javascript:Paginar(' + totalRegistros + ', ' + (paginaAtual + 1) + ', \'' + divContainer + '\');">Próximo</a>';
        html += '<a href="javascript:Paginar(' + totalRegistros + ', ' + paginasTotal + ', \'' + divContainer + '\');">Último</a>';
    }
    else {
        html += '<a href="#" class="aspNetDisabled">Próximo</a>';
        html += '<a href="#" class="aspNetDisabled">Último</a>';
    }
    html += '</span>';
    $('#' + $('#' + divContainer).data('divPaginacao')).html(html);
}

function CriarListaPaginas(paginasTotal, totalRegistros, divContainer) {
    var html = '';
    var valorMaximo = 5;
    var contador = 0;
    var pagina = paginaAtual - 2;

    if (paginasTotal == 1)
        html = '<span class="current">1</span>';
    else {
        var indice = 0;
        for (var i = pagina; (indice < valorMaximo) && (i <= paginasTotal) ; i++) {
            if (i > 0) {
                indice++;
                if (i == paginaAtual)
                    html += '<span class="current">' + paginaAtual + '</span>';
                else
                    html += '<a href="javascript:Paginar(' + totalRegistros + ', ' + i + ', \'' + divContainer + '\');">' + i + '</a>';
            }
        }
    }
    return html;
}

function Paginar(totalRegistros, pagina, divContainer) {
    var idtabela = $('#' + divContainer).data('idTabela');
    var agruparOpcoes = $('#' + divContainer).data('agruparOpcoes');
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
            AdicionarColunas(false, divContainer, agruparOpcoes);
        }
        else {
            var NumeroPagina = quantidadeRegistrosPagina / quantidadeMostrar;
            NumeroPagina = paginaAtual / NumeroPagina;
            paginaAtualServidor = Math.ceil(NumeroPagina);
            if (paginaAtual == paginasMostrar)
                jogarParaoUltimo = true;
            CriarGridView(null, null, null, divContainer);
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
            AdicionarColunas(false, divContainer, agruparOpcoes);
        }
        else {
            jogarParaoUltimo = true;
            var NumeroPagina = quantidadeRegistrosPagina / quantidadeMostrar;
            NumeroPagina = paginaAtual / NumeroPagina;
            paginaAtualServidor = Math.ceil(NumeroPagina);
            CriarGridView(null, null, null, divContainer);
        }
    }
}