(function ($) {
    $.fn.importador = function (config) {
        "use strict";

        var Plugin = function (container) {

            /**
			 * Referencia global ä classe para acessos de outros contextos
			 */
            var _this = this;



            /**
			 * Variaveis privadas de acesso intenro
			 */
            var _private = {
                /**
				 *  Enumerados simples para ação e para valor de select
				 */
                removecoluna: "removecoluna",
                selecionecoluna: "selecionecoluna",
                classeeditando: "form-editing",

                /**
				 * Array de erros
				 */
                erros: [],

                /**
				 * Controle de fluxo para inicializacao
				 * Usado apenas para setar o primeiro select
				 */
                setandoColunas: false,

                /**
				 * Enumerados para codigo de tecla
				 */
                escKey: 27,
                enterKey: 13,

                /**
				 * Numero de colunas maxima
				 */
                colunas: 0,

                /**
				 * Elemento thead
				 */
                $head: null,

                /**
				 * Array de elementos thead th
				 */
                $th: [],


                $select: [],

                /**
				 * Elemento tbody
				 */
                $body: null,

                /**
				 * Campos usados no select para impedir campos repitidos
				 */
                camposUsados: [],

                /**
				 * Tamanho padrao, quando nao ha opcao selecionada
				 */
                widthPadrao: 150,

                /**
				 * Salva HTML original do container para usar no metodo destrui
				 */
                originalHTML: '',


                /**
				 * Um match referente ao container do plugin para incluir o tooltip
				 */
                tooltipContainer: '',


                /**
				 * Texto para quando não há dados
				 */
                tabelavazia: '<b>Nenhum registro.</b>',


                /**
				 * Texto quando colunas obrigatórias não foram selecionadas
				 */
                msgcolunaobrigatoria: 'Colunas com * são obrigatórias',


                /**
				 * contem a informação após o processamento.
				 */
                statusLinha: [],
            };



            //-- Metodos privados

            /**
			 * Dentro dos dados, busca a linha com maior numero de coluna
			 */
            var numeroDeColunas = function (dados) {
                // Verifica o maior numero de colunas
                var maiorNCols = 0;

                for (var linha in dados) {
                    if (dados[linha].length > maiorNCols)
                        maiorNCols = dados[linha].length;
                }
                return maiorNCols + 2;
            }

            var buscarCamposPorProp = function (proName, value) {
                // Busca o campo
                for (var i in _private.configuracao) {
                    var regra = _private.configuracao[i];
                    if (regra[proName] == value)
                        return regra;
                }

                return null;
            }

            /**
			 * Elemento container geral (tabela)
			 */
            this.$this = $(container);

            /**
			 * Acesso aos dados
			 */
            this.dados = config.dados;

            /**
			 * Metodo contrsturo
			 */
            this.init = function (config) {
                // Mescla configuracao do metodo
                for (var v in config)
                    if (v != "dados") _private[v] = config[v];

                // Salva o HTML
                _private.originalHTML = this.$this.html();
                this.$this.html('');

                // Inicia montagem da tabela
                this.montaTabela();

                // Vincula eventos de editar matriz
                this.eventoEditar();

                // Para colocar os valores de select inicial
                // É necessário de sabilitar a renderização do width da tabela
                if (_private.selectPadrao) {
                    _private.setandoColunas = true;
                    this.selectPadrao();
                    _private.setandoColunas = false;
                } else {
                    // Caso contrario, dispara o evento change dos selects para ajustar as colunas
                    _this.ExecutarOnChage();
                }

                // Apos modifica todos select, renderiza width da coluna
                this.tamanhoTabela();
            }

            /**
			 * Metodo para renderizar a grid de dados
			 */
            this.rerender = function () {
                // Basta remover o corpo
                _private.$body.remove();
                _private.$body = null;

                // Para render a tabela
                this.montaTabela();

                // Seta o tamanho da tabela
                this.tamanhoTabela();
            }

            /**
			 * Retorna os options possiveis
			 */
            this.optionsSelect = function () {
                var options = [];
                for (var i = 0; i < _private.configuracao.length; i++) {
                    var config = _private.configuracao[i];

                    if ($.inArray(config.Propriedade, _private.camposUsados) < 0)
                        options.push(config);
                }

                return options;
            }

            /**
			 * Gera options por array
			 */
            this.geraOptionsPorArray = function (campos) {
                var options = [];

                // Option selecione a coluna
                (function () {
                    var option = $("<option />", { value: _private.selecionecoluna }).text("-- Selecione --");
                    options.push(option);
                }());


                // Gera as opcoes dos campos
                campos.forEach(function (config, i) {
                    var option = $("<option />", { value: config.Propriedade });
                    option.text(config.Descricao + (config.Obrigatorio === true ? "*" : ""));

                    options.push(option);
                });


                // Option remover coluna
                (function () {
                    var option = $("<option />", { value: _private.removecoluna }).text("Remover Coluna");
                    options.push(option);
                }());
                return options;
            }

            /**
			 * Gera o select para a coluna
			 */
            this.montaSelect = function () {
                var select = $("<select />", { class: "form-control" });
                var optionsDisponiveis = this.optionsSelect();
                var options = this.geraOptionsPorArray(optionsDisponiveis);

                select.append(options);

                return select;
            }

            /**
			 * Metodo que inicia a montagem do thead e tbody
			 */
            this.montaTabela = function () {
                _private.colunas = numeroDeColunas(this.dados);

                this.montaThead();
                this.montaTbody();
                this.renderizaTabela();
            }

            /**
			 * Gera o numero de colunas e prepara o select de cada uma
			 * Thead é renderizado apenas uma vez 
			 * Quando removido uma coluna, apenas é removido o TH e a coluna na grid
			 */
            this.montaThead = function () {
                if (_private.$head == null) {
                    _private.$head = $("<thead />");
                    _private.$head.append("<tr></tr>");

                    // Coluna para numero da linha
                    (function () {
                        var th = $("<th />", { class: "text-center" }).css('min-width', '50px');
                        th.text('#');
                        _private.$th.push(th);
                    }());

                    // Colunas para opcao
                    for (var i = 1; i < _private.colunas - 1; i++) {
                        (function () {
                            var th = $("<th />");
                            th.data('index', i - 1);
                            _private.$select.push(_this.selectColuna(th));
                            _private.$th.push(th);
                        }());
                    }

                    //adiciona coluna para a mensagem de retorno
                    (function () {
                        var th = $("<th />", { class: "text-center" }).css('min-width', '100px');
                        th.text('Retorno');
                        _private.$th.push(th);
                    }());

                    // Colunas para opcao de remover linha
                    (function () {
                        var th = $("<th />", { class: "text-center" }).css('min-width', '85px');
                        th.text('Remover');
                        _private.$th.push(th);
                    }());


                    // Insere as colunas de cabecalho no cabecalho da tabela
                    _private.$head.find("tr").append(_private.$th);
                } else {
                    for (var i = 1; i < _private.$th.length - 1; i++) {
                        _private.$th[i].data('index', i - 1);
                    }
                }
            }

            /**
			 * Busca todos os dados importados e gera a grid de informacoes
			 * Lembrando que existe uma coluna no inicio com o numero da coluan e uma coluna no inicio para remover a linha
			 */
            this.montaTbody = function () {
                if (_private.$body == null) {
                    _private.$body = $("<tbody />");
                    var trs = [];

                    // Quando nao ha colunas
                    if (_private.$th.length > 2) {
                        for (var i = 0; i < this.dados.length; i++) {
                            (function () {
                                var tr = $("<tr />");
                                if (_private.statusLinha.length > 0) {
                                    if (_private.statusLinha[i].processou) {
                                        tr.addClass("bg-success-100");
                                    } else {
                                        tr.addClass("bg-danger-100");
                                    }
                                }

                                // Numero da linha
                                (function () {
                                    var td = $("<td />", { class: "info text-center line-info" }).html("<strong>" + (i + 1) + "</strong>");
                                    //if (_private.statusLinha.length > 0) {
                                    //    if (!_private.statusLinha[i].processou) {
                                    //        td.tooltip({
                                    //            container: _private.tooltipContainer,
                                    //            title: _private.statusLinha[i].mensagemFalha,
                                    //        });
                                    //    }
                                    //}
                                    tr.append(td);

                                }());


                                // Informacao na matriz
                                for (var j = 0; j < _this.dados[i].length; j++) {
                                    (function () {
                                        var td = $("<td />");
                                        // Valor
                                        var valor = _this.dados[i][j];

                                        // Seta exios
                                        td.data("i", i);
                                        td.data("j", j);

                                        // Seta valor
                                        td.text(valor);

                                        // Se for apenas numero, formata direta
                                        if (!isNaN(valor) && valor != "")
                                            td.addClass('text-right');

                                        tr.append(td);
                                    }());
                                }

                                // Mensagem de retorno do processamento
                                (function () {
                                    var td = $("<td />", { class: "text-left line-info" });

                                    if (_private.statusLinha.length > 0) {
                                        if (!_private.statusLinha[i].processou) {
                                            td.text(_private.statusLinha[i].mensagemFalha);
                                            td.attr('title', _private.statusLinha[i].mensagemFalha);
                                        }
                                        else {
                                            td.text("Processado com sucesso.");
                                            td.attr('title', "Processado com sucesso.");
                                        }
                                    }

                                    tr.append(td);
                                }());

                                // Opcao de remover linha
                                (function () {
                                    var td = $("<td />", { class: "text-center line-info" });
                                    var btn = $('<a />', { href: "#" }).data('index', i).html('Remover');

                                    btn.on('click', function (e) {
                                        if (e && e.preventDefault) e.preventDefault();
                                        _this.buttonOnClick(btn);
                                    });

                                    td.append(btn);
                                    tr.append(td);
                                }());

                                trs.push(tr);
                            }());
                        }
                    }

                    // Nao tem nenhum dado
                    if (trs.length == 0) {
                        (function () {
                            var tr = $("<tr />");
                            var td = $("<td />");

                            // Tamnho completo
                            td.attr('colspan', _private.$th.length);

                            // Centraliza
                            td.addClass('text-center line-info');

                            // Texto vazio
                            td.html(_private.tabelavazia);

                            // Inlui DOM
                            tr.append(td);
                            trs.push(tr);
                        }());
                    }

                    _private.$body.append(trs);
                }
            }

            /**
			 * Percorre os cabecalhos e seta um valor inicial
			 * Cada um tem um valor diferente
			 */
            this.selectPadrao = function () {
                for (var i = 1; i < _private.$th.length - 2; i++) {
                    var select = _private.$th[i].find('select');

                    if (i <= _private.configuracao.length) {
                        var valEq = select.find('option:eq(1)').val();

                        if (valEq != _private.removecoluna)
                            select.val(valEq);
                    }

                    select.trigger('change');
                }
            }

            /**
			 * Recebe o th e coloca o select e instancia os eventos de mudanca de valor
			 */
            this.selectColuna = function (th) {
                var self = this;
                var $select = this.montaSelect();
                th.append($select);

                $select.on('change', function () {
                    _this.selectOnChange($select, th, true);
                });
                return $select;
            };

            /**
			 * Pega os elementos privados e insere no DOM
			 */
            this.renderizaTabela = function () {
                this.$this.append(_private.$head);
                this.$this.append(_private.$body);
            };



            //-- Validation

            /**
			 * Busca a configuração pela propriedade
			 */
            this.campoPorPropriedade = function (propriedade) {
                return buscarCamposPorProp("Propriedade", propriedade);
            }

            /**
			 * Busca a configuração pela id
			 */
            this.campoPorId = function (id) {
                return buscarCamposPorProp("Id", id);
            }

            /**
			 * Busca a configuração pelo eixo J
			 */
            this.campoPorColuna = function (index) {
                var th = _private.$th[index + 1];
                var propriedadeHead = th.find('select').val();

                if (propriedadeHead != _private.selecionecoluna) {
                    // Busca as regras
                    for (var i in _private.configuracao) {
                        var regra = _private.configuracao[i];
                        if (regra.Propriedade == propriedadeHead)
                            return regra;
                    }
                }

                return {};
            }

            /**
			 * Busca todos os dados da linha mapeados por id.
             * Ex: {KmAtual: 0, Proprietario: 0} // Colunas sem selecao sao descartadas
			 */
            this.DadosLinha = function (i) {
                /**
                 * Armazena o id da coluans para evitar acessos constantes
                 */
                var cacheId = {};

                var dadosDaLinha = {};
                this.dados[i].forEach(function (coluna, j) {
                    var valorColuna = _private.$th[j + 1].find('select').val();

                    // Se a configuracao por ID ainda nao foi selecionada
                    if (!cacheId[j])
                        // Busca a configuracao do select que esta na mesma coluna que o dados em questao
                        cacheId[j] = _this.campoPorPropriedade(valorColuna);

                    if (valorColuna != _private.selecionecoluna) {
                        dadosDaLinha[valorColuna] = {
                            Propriedade: valorColuna,
                            valor: coluna,
                            Descricao: cacheId[j].Descricao
                        };
                    }
                });

                return dadosDaLinha;
            }

            /**
			 * Recebe o valor da matriz, configuracao do eixo
			 * Prepara a regra e parametros 
			 * Executa validacao
			 * Chama o CB da validacao
			 */
            this.validaCampo = function (input, obj, i, j, callback) {
                var regras = "Regras" in obj ? obj.Regras : "";
                var mensagens = [];

                if (regras.length > 0) {
                    // Percorre as regras
                    for (var r in regras) {
                        var nomeRegra = regras[r];
                        var params = [];

                        // Pega parametros
                        if (nomeRegra.split(':').length > 1) {
                            var nomeEParams = nomeRegra.split(':');
                            nomeRegra = nomeEParams[0];
                            params = nomeEParams[1].split(',');
                        }

                        if ($.isFunction(_private.regras[nomeRegra])) {
                            var dadosLinha = _this.DadosLinha(i);
                            var msg = _private.regras[nomeRegra](input, params, obj, dadosLinha);
                            if (typeof msg == "string")
                                mensagens.push(msg);
                        }
                    }
                    callback(mensagens.length > 0, mensagens);
                } else {
                    // Se nao tem regras, é valido
                    callback(false, mensagens);
                }
            }

            /**
			 * Percorre todos os cabecalhos e verifica se todos obrigatorsio estao setados
			 */
            this.validaCabecalhos = function () {
                var camposObrigatorios = [];

                _private.configuracao.forEach(function (config) {
                    if (config.Obrigatorio)
                        camposObrigatorios.push(config.Propriedade);
                });

                for (var i = 1; i < _private.$th.length - 2; i++) {
                    var id = _private.$th[i].find('select').val();
                    var index = $.inArray(id, camposObrigatorios);

                    if (index >= 0) {
                        camposObrigatorios.splice(index, 1);
                    }
                }

                return camposObrigatorios.length == 0;
            }

            /**
			 * Percorre todos os cabecalhos sem valor setado e coloca tooltip
			 */
            this.tooltipsCabecalhos = function () {
                for (var i = 1; i < _private.$th.length - 2; i++) {
                    var $th = _private.$th[i];
                    var $select = $th.find('select');

                    if ($select.val() == _private.selecionecoluna)
                        $th
                            .addClass('bg-danger-100')
                            //.tooltip({
                            //    container: _private.tooltipContainer,
                            //    title: _private.msgcolunaobrigatoria,
                            //});
                }
            }

            /**
			 * Percorre todos os cabecalhos com tooltip e remove
			 */
            this.removeTooltipsColunas = function () {
                for (var i = 1; i < _private.$th.length - 2; i++)
                    _private.$th[i]
                        .removeClass('bg-danger-100')
                        //.tooltip('destroy');
            }

            /**
			 * Percorre todos os cabecalhos com tooltip e remove
			 */
            this.removeTooltipsDados = function () {
                _private.$body.find("tr td.bg-danger-100").each(function () {
                    var td = $(this);

                    td.removeClass("bg-danger-100");
                    //td.tooltip('destroy');
                });
            }

            /**
			 * Percorre todos os campos para efetuar a validacao individual
			 */
            this.validaDados = function () {
                var Dados = this.dados;
                var cacheCampos = {};
                var valido = true;
                _private.$body.find("tr").each(function (i) {
                    var tr = $(this);

                    tr.find("td:not(.line-info)").each(function (j) {
                        var td = $(this);
                        var campo;

                        if (!(j in cacheCampos))
                            cacheCampos[j] = _this.campoPorColuna(j);

                        campo = cacheCampos[j];

                        // Executa validacao com o conteudo
                        _this.validaCampo(Dados[i][j], campo, i, j, function (err, msg) {
                            // Coloca o erro no campo se invalido e exibe mensage
                            if (err) {
                                td.addClass("bg-danger-100");

                                // Objeto erro
                                var dataError = {
                                    th: td,
                                    dado: Dados[i][j],
                                    i: i,
                                    j: j,
                                    mensagem: msg[0],
                                    campo: campo
                                };

                                //td.tooltip({
                                //    container: _private.tooltipContainer,
                                //    title: msg[0],
                                //});

                                valido = false;
                            } else {
                                td.removeClass("bg-danger-100");
                                //td.tooltip('destroy');
                            }
                        });
                    });
                });

                // Retorna se tem erro
                return valido;
            }

            /**
			 * Retorna o array de erros
			 */
            this.buscaErros = function () {
                return _private.erros;
            }



            //-- Matriz

            /**
			 * Remove dados da matriz no eixo j e remvoe o cabecalho da coluna
			 */
            this.removeMatrizJ = function (indexJ) {
                indexJ = parseInt(indexJ);

                for (var i = 0; i < this.dados.length; i++) {
                    this.dados[i].splice(indexJ, 1);
                }

                _private.$th[indexJ + 1].remove();
                _private.$th.splice(indexJ + 1, 1);

                this.rerender();
            }

            /**
			 * Remove dados da linha
			 */
            this.removeMatrizI = function (indexI) {
                indexI = parseInt(indexI);

                this.dados.splice(indexI, 1);

                this.rerender();
            }

            /**
			 * Remove dados da matriz no eixo j e remvoe o cabecalho da coluna
			 */
            this.removeMatrizJMelhorado = function (indexJ) { // Em processo de melhorias
                indexJ = parseInt(indexJ);

                // Remove a linha dos dados
                for (var i = 0; i < this.dados.length; i++)
                    this.dados[i].splice(indexJ, 1);

                // Remove a colunas
                _private.$th[indexJ + 1].remove();
                _private.$th.splice(indexJ + 1, 1);

                // Remove do DOM
                _private.$body.find('tr td:nth-child(' + (indexJ + 2) + ')').remove(); // + 2 pqe o seletor é baseado no css (onde o index começa com 1 e não 0)

                // Fix valor do index
                for (var i = indexJ + 1; i < _private.$th.length; i++)
                    _private.$th[i].data('index', i);
            }

            /**
			 * Remove dados da linha
			 */
            this.removeMatrizIMelhorado = function (indexI) { // Em processo de melhorias
                indexI = parseInt(indexI);

                // Remove a linha dos dados
                this.dados.splice(indexI, 1);

                // Remove do DOM
                this.$this.find('.row-' + indexI).remove();
            }

            /**
			 * Atuliza os dados recebidos
             * Atualzia tabela
			 */
            this.refresh = function (data) {

                this.dados = $.merge([], data);
                this.rerender();
            }


            /**
			 * Percorrer todas colunas e pega o ID da coluna e retorna como um array
			 */
            this.getColunas = function () {
                var arrayOrder = [];
                var prop;

                for (var i = 1; i < (_private.$th.length - 2); i++) {
                    prop = _private.$th[i].find("select").val();

                    if (prop == _private.selecionecoluna) {
                        prop = 0;
                    } else {
                        var coluna = this.campoPorPropriedade(prop);
                        prop = coluna.Id;
                    }

                    arrayOrder.push(prop);
                }

                return arrayOrder;
            }

            this.setStatus = function (retornoLinhas) {
                //var lineBody = _private.$body.find("tr:eq(" + index + ")");
                _private.statusLinha = retornoLinhas;

                _this.rerender();

            }

            /**
			 * Percorrer todas colunas e seta o valor de acordo com o ID passado
			 */
            this.setColunas = function (arrayOrder) {
                // Valida entrada
                if (typeof arrayOrder == "string")
                    arrayOrder = arrayOrder.split("|");

                if (!$.isArray(arrayOrder))
                    return console.error("Função 'importador.setColunas' espera uma string ou um array!");

                if (arrayOrder.length == 0)
                    return console.error("Função 'importador.setColunas' espera um array com pelo menos 1 valor!");

                // Evita disparar evento a cada manipulacao
                _private.setandoColunas = true;

                // Props validas
                var props = [];

                // Limpa todas props setadas
                _private.$head.find("select").val(_private.selecionecoluna);
                _this.ExecutarOnChage();

                // Busca as props
                for (var i = 0; i < arrayOrder.length; i++) {
                    var coluna = this.campoPorId(arrayOrder[i]);
                    var prop;

                    if (coluna == null)
                        prop = _private.selecionecoluna;
                    else
                        prop = coluna.Propriedade;

                    props.push(prop);
                }

                // Seta as props
                var size = _private.$th.length - 2;
                for (var i = 1; i < size; i++) {
                    var $select = _private.$select[i - 1];

                    // Se existe algum select 
                    // E o n de propriedades é compatível com o n de colunas

                    if ($select != null && i <= props.length)
                        $select.val(props[i - 1]); // Seta a propriedade
                }

                // Finaliza
                _private.setandoColunas = false;

                // Dispara o evento change dos selects para ajustar as colunas
                _this.ExecutarOnChage();

                // Seta options permitidos
                _this.LiberaOptionsNaoUtilizados(props);
            }



            /**
			 * Invoca o método on change do select para ajustar o tamanho
			 */
            this.ExecutarOnChage = function () {
                for (var i = 1; i < _private.$th.length - 2; i++) {
                    _this.selectOnChange(_private.$select[i - 1], _private.$th[i], false);
                }
            }



            /**
			 * Busca Propriedades não utilizadas
             * Preenche em todos selects que não foram utilizados
			 */
            this.LiberaOptionsNaoUtilizados = function (propsJaUtilizadas) {
                var propsDisponiveis = this.optionsSelect();
                
                for (var i = 0; i < _private.$select.length; i++) {
                    if (_private.$select[i].val() == _private.selecionecoluna)
                        this.renderizaOption(_private.$select[i], propsDisponiveis);
                }
            }


            /**
			 * Remove toda tabela
			 * Esvazia os cabecalhos
			 */
            this.destruirInstancia = function () {
                if (_private.$head != null)
                    _private.$head.remove();
                _private.$head = null;

                if (_private.$body != null)
                    _private.$body.remove();
                _private.$body = null;

                while (_private.$th.length > 0)
                    _private.$th.pop();

                this.$this.off('dblclick');

                this.$this.html(_private.originalHTML);
                this.$this.css('min-width', '');
                this.$this = null;
            }



            //-- Campos

            /**
			 * Adiciona id do campo como EM USO
			 */
            this.campoEmUso = function (id) {
                var index = $.inArray(id, _private.camposUsados);

                if (index < 0)
                    _private.camposUsados.push(id);
            }

            /**
			 * Remove campo do uso
			 */
            this.campoEmDesuso = function (id) {
                var index = $.inArray(id, _private.camposUsados);

                if (index >= 0)
                    _private.camposUsados.splice(index, 1);
            }

            /**
			 * Alterna os options dos selects
			 */
            this.renderizaOptionsSelect = function () {
                var camposDisponiveis = this.optionsSelect();
                for (var i = 1; i < _private.$th.length - 2; i++)
                    this.renderizaOption(_private.$th[i].find('select'), camposDisponiveis);
            }

            /**
			 * Metodo para alterar os options individualmente
			 */
            this.renderizaOption = function (select, optionsDisponiveis) {
                // Valor do select
                var valor = select.val();

                // Array de options
                var _options = [];

                // Config do campos especifico
                var configSelect = this.campoPorPropriedade(valor);

                // Mescla se existir
                if (configSelect != null) {
                    // Coloca o select atual como primeiro item
                    optionsDisponiveis = [configSelect].concat(optionsDisponiveis);
                }

                // Array de options
                var options = this.geraOptionsPorArray(optionsDisponiveis);

                // Limpa os options
                select.find('option').remove();

                // Insere as opcoes
                select.append(options);

                // Recoloca o valor
                select.val(valor);
            }



            //-- Events

            /**
			 * Vincula evento de editar o campo
			 */
            this.eventoEditar = function () {
                this.$this.on('dblclick', 'tbody td:not(.line-info)', this.editarTd);
            }

            /**
			 * Evendo de mudanca no valor do select
			 */
            this.selectOnChange = function ($this, th, renderizar) {
                // Remove Tooltip dos cabecalhos
                this.removeTooltipsColunas();

                // Pega index do eixo
                var index = th.data('index');

                // Auxiliar do valor
                var novoValor = $this.val();

                // Pega o valor antigo
                var valorAntigo = $this.data('select');

                // Remove de campo usado
                if (valorAntigo)
                    this.campoEmDesuso(valorAntigo);

                // Se o select é remoção da coluna
                if (novoValor == _private.removecoluna) {

                    // Remove coluna
                    this.removeMatrizJ(index);
                } else {
                    // Pega configuracoes
                    var campo = this.campoPorColuna(index);

                    // Seta o valor
                    $this.data('select', novoValor);


                    // Remove de campo usado
                    this.campoEmUso(novoValor);

                    // Reajusta o campo
                    var width = typeof campo.Tamanho != "undefined" ? campo.Tamanho : _private.widthPadrao;

                    th.css('min-width', width);
                    th.attr('data-width', width);
                    this.tamanhoTabela();
                }

                if (renderizar)
                    this.renderizaOptionsSelect(renderizar);
            }

            /**
			 * Seta o width com a soma do tamanho dos campos
			 */
            this.tamanhoTabela = function () {
                if (_private.setandoColunas) return;

                var totalWidth = 160; // 80 para cada coluna das pontas

                for (var i = 1; i < _private.$th.length - 1; i++) {
                    var w = _private.$th[i].attr('data-width');
                    w = parseInt(w);

                    if (!isNaN(w))
                        totalWidth += w;
                }

                //this.$this.css('width', totalWidth);
            }

            /**
			 * Cria o campo e vincula eventos para alterar o valor do campo
			 */
            this.editarTd = function () {
                var $this = $(this);

                // Verifica se o campo ja esta sendo editado, se nao adiciona classe de edicao
                if ($this.hasClass(_private.classeeditando))
                    return false; // Campo ja em edicao
                else
                    $this.addClass(_private.classeeditando);

                var valor = $this.html();
                var input = $("<input />", { class: "form-control", type: "text" }).val(valor);
                var setaValor = function (val, novoValor) {
                    // Remove o input
                    input.remove();

                    // Coloca valor no html
                    $this.html(val);

                    // Remove a classe de edicao
                    $this.removeClass(_private.classeeditando);

                    // Muda valor no objeto de dados
                    var i = $this.data('i');
                    var j = $this.data('j');

                    // Atauliza o valor na matriz
                    _this.dados[i][j] = val;

                    // Se for apenas numero, formata direta
                    if (!isNaN(val) && val != "")
                        $this.addClass('text-right');

                    // Valida novo valor
                    if (novoValor) {
                        var campo = _this.campoPorColuna(j);

                        // Executa validacao com o conteudo atualziado apenas se esta invalido
                        if ($this.hasClass("bg-danger-100")) {
                            _this.validaCampo(val, campo, i, j, function (err, msg) {
                                // Remove a classe de erro se validou
                                if (!err) {
                                    $this.removeClass("bg-danger-100");
                                    //$this.tooltip("destroy");
                                }
                            });
                        }
                    }
                }

                // Remove classe de formatacao
                $this.removeClass('text-right');

                // Limpa campo e oloca o input
                $this.html('').append(input);

                // Foca o input e seta eventos
                input.trigger('focus');
                input.on('blur', function () {
                    setaValor(input.val(), true);
                });
                input.on('keyup', function (e) {
                    if (e.which == _private.escKey || e.keyCode == _private.escKey)
                        setaValor(valor, false);
                    else if (e.which == _private.enterKey || e.keyCode == _private.enterKey) {
                        if (e && e.preventDefault) e.preventDefault();
                        setaValor(input.val(), true);
                    }
                });
            }

            /**
			 * Evento de remover linha
			 */
            this.buttonOnClick = function ($this) {
                var index = $this.data('index');

                this.removeMatrizI(index);
            }





            //-- Montador de SQL

            /**
			 * Busca os dados e retorno 
			 */
            this.montaRetorno = function (adicionarRetornoProcessamento) {
                /**
                 * Armazena o id da coluans para evitar acessos constantes
                 */
                var cacheId = {};
                var Retorno = {
                    nome: _private["nome"],
                    tamanho: _private["tamanho"],
                    tipo: _private["tipo"],
                    dados: []
                };

                //  ----------- ----------- -----------
                // |     A     |     B     |     C     |
                //  ----------- ----------- -----------
                // |     D     |     E     |     F     |
                //  ----------- ----------- -----------
                this.dados.forEach(function (linha, i) {
                    var _colunas = [];

                    //  ----------- ----------- -----------
                    // |     A     |     B     |     C     |
                    //  ----------- ----------- -----------
                    linha.forEach(function (coluna, j) {
                        //  -----------
                        // |     A     |
                        //  -----------
                        var propriedadeColunas = _private.$th[j + 1].find('select').val();

                        // Se a configuracao por ID ainda nao foi selecionada
                        if (!cacheId[j])
                            // Busca a configuracao do select que esta na mesma coluna que o dados em questao
                            cacheId[j] = _this.campoPorPropriedade(propriedadeColunas);

                        if (cacheId[j] != null) {
                            _colunas.push({
                                NomeCampo: cacheId[j].Propriedade,
                                Valor: coluna,
                            });
                        }

                    });

                    if (adicionarRetornoProcessamento === true) {
                        if (_private.statusLinha.length > 0) {
                            var msgRetorno = "";
                            if (!_private.statusLinha[i].processou)
                                msgRetorno = _private.statusLinha[i].mensagemFalha;
                            else
                                msgRetorno = "Processado com sucesso.";

                            _colunas.push({
                                NomeCampo: "MensagemRetornoProcessamento",
                                Valor: msgRetorno,
                            });
                        }
                    }

                    if (_colunas.length > 0)
                        Retorno.dados.push({
                            Colunas: _colunas
                        });
                });

                return Retorno;
            }





            //-- Construtor

            /**
			 * Inicia classe passando parametros
			 */
            this.init(config);



            //-- Public Methods

            /**
			 * Retorna os metodos acessiveis externamente
			 */
            return {
                /**
				 * Retorna a montagem dos dados para gerar a importacao
				 */
                processa: function (adicionarMensagemRetornoProcessamento) {
                    return _this.montaRetorno(adicionarMensagemRetornoProcessamento);
                },

                /**
				 * Tooltips das Colunas
				 */
                tooltipsColunas: function () {
                    return _this.tooltipsCabecalhos();
                },

                /**
				 * Remove os tooltips das Colunas
				 */
                removeTooltipsColunas: function () {
                    return _this.removeTooltipsColunas();
                },

                /**
				 * Valida cabeçalhos
				 */
                validarColunas: function () {
                    return _this.validaCabecalhos();
                },

                /**
				 * Valida todos os campos
				 */
                validarDados: function () {
                    return _this.validaDados();
                },

                /**
				 * Valida todos os campos
				 */
                removeTooltipsDados: function () {
                    return _this.removeTooltipsDados();
                },

                /**
				 * Retorna o array de erros
				 */
                erros: function () {
                    return _this.buscaErros();
                },

                /**
				 * Destroi a instancia
				 */
                destruir: function () {
                    _this.destruirInstancia();
                    return null;
                },

                /**
                 * Atualiza dados e renderiza
                 */
                refresh: function (DATA) {
                    _this.refresh(DATA);
                    return null;
                },

                /**
                 * Seta colunas padrões dos campos
                 */
                setColunas: function (arrayOrder) {
                    _this.setColunas(arrayOrder);
                    return null;
                },

                /**
                 * Retorna um array com a ordem das colunas
                 */
                getColunas: function () {
                    return _this.getColunas();
                },

                setStatus: function (retornoLinhas) {
                    _this.setStatus(retornoLinhas);
                }
            }
        };


        /**
		 * Instancia plugin
		 */
        return new Plugin(this);;
    }
}(jQuery));