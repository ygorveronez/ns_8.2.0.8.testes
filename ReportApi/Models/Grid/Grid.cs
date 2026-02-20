using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Dynamic;

namespace ReportApi.Models.Grid
{
    public class Grid
    {
        #region Construtores

        public Grid()
        {

        }

        public Grid(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, List<Head> headerPadrao, bool buscarCabecalhoPorCodigoDinamico)
        {
            this.header = new List<Head>();

            foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna in relatorio.Colunas)
            {
                Head headPadrao;

                if (buscarCabecalhoPorCodigoDinamico)
                    headPadrao = (from o in headerPadrao where (o.dynamicCode > 0 && o.dynamicCode == coluna.CodigoDinamico) || (o.dynamicCode <= 0 && o.data == coluna.Propriedade) select o).FirstOrDefault();
                else
                    headPadrao = (from o in headerPadrao where o.data == coluna.Propriedade select o).FirstOrDefault();

                if (headPadrao != null)
                {
                    decimal tamanhoColuna = (coluna.Tamanho < 0m) ? 0m : coluna.Tamanho;

                    Head head = new Head();

                    head.data = coluna.Propriedade;
                    head.name = coluna.Propriedade;
                    head.dataTypeExportacao = coluna.DataTypeExportacao;
                    head.decimalPrecision = coluna.PrecisaoDecimal;
                    head.dynamicCode = coluna.CodigoDinamico;
                    head.enableGroup = coluna.PermiteAgrupamento;
                    head.orderable = headPadrao.orderable;
                    head.phoneHide = headPadrao.phoneHide;
                    head.position = coluna.Posicao;
                    head.sumary = coluna.TipoSumarizacao;
                    head.tabletHide = headPadrao.phoneHide;
                    head.title = coluna.Titulo;
                    head.visible = coluna.Visivel;
                    head.width = ((tamanhoColuna == 0) && coluna.Visivel) ? headPadrao.widthDefault : tamanhoColuna.ToString().Replace(",", ".") + "%";
                    head.widthDefault = headPadrao.widthDefault;
                    head.useTextFormat = coluna.UtilizarFormatoTexto;

                    if (coluna.Alinhamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.right)
                        head.className = "text-end";
                    else if (coluna.Alinhamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.center)
                        head.className = "text-center";
                    else
                        head.className = "text-start";

                    header.Add(head);
                }
            }

            foreach (Head head in headerPadrao)
            {
                bool headAdicionado;

                if (buscarCabecalhoPorCodigoDinamico)
                    headAdicionado = relatorio.Colunas.Any(o => (head.dynamicCode > 0 && o.CodigoDinamico == head.dynamicCode) || (head.dynamicCode <= 0 && o.Propriedade == head.data));
                else
                    headAdicionado = relatorio.Colunas.Any(o => o.Propriedade == head.data);

                if (!headAdicionado)
                {
                    head.visible = false;
                    head.position = header.Count;

                    header.Add(head);
                }
            }
        }

        public Grid(int draw)
        {
            this.draw = draw;
        }

        public Grid(HttpRequestBase request)
        {
            if (request != null)
            {
                Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Grid>(request.Params["Grid"]);
                dynamic dynGrid = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(request.Params["Grid"]);

                this.header = (grid.header?.Count > 0) ? grid.header.OrderBy(obj => obj.position).ToList() : new List<Head>();
                this.inicio = dynGrid.start ?? 0;
                this.limite = dynGrid.length ?? 0;
                this.indiceColunaOrdena = dynGrid.order?[0].column ?? 0;
                this.dirOrdena = dynGrid.order?[0].dir;
                this.draw = grid.draw;
                this.tituloExportacao = dynGrid.tituloExportacao;
                this.modelo = dynGrid.modelo ?? 0;

                this.group = grid.group ?? new Group();
            }
        }

        public Grid(int draw, int recordsTotal)
        {
            this.header = new List<Head>();
            this.draw = draw;
            this.recordsFiltered = recordsTotal;
            this.recordsTotal = recordsTotal;
        }

        #endregion

        #region Propriedade

        public string id { get; set; }

        public int draw { get; set; }

        public int inicio { get; set; }

        public int limite { get; set; }

        public int indiceColunaOrdena { get; set; }

        public string dirOrdena { get; set; }

        public string tituloExportacao { get; set; }

        public int recordsTotal { get; set; }

        public int recordsFiltered { get; set; }

        public Group group { get; set; }

        public List<Head> header { get; set; }
        public List<Tab> listTabs { get; set; }

        public dynamic data { get; set; }

        private List<dynamic> DataRows { get; set; }

        public string extensaoExcel { get; set; }

        public string extensaoCSV { get; set; }

        public dynamic dataSumarizada { get; set; }

        public bool scrollHorizontal { get; set; }

        public int modelo { get; set; }

        #endregion

        #region Métodos Privados 

        private Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico, int position, bool permiteAgrupar, EditableCell editableCell, bool adicionarAoAgrupamentoQuandoInvisivel, int decimalPrecision, string dataTypeExportacao, bool useTextFormat, string className = null, Tab tab = null)
        {
            Head head = new Head();

            head.title = descricao;
            head.data = propriedade;
            head.name = propriedade;
            head.decimalPrecision = decimalPrecision;
            head.dataTypeExportacao = dataTypeExportacao;
            head.width = tamanho.ToString().Replace(",", ".") + "%";
            head.widthDefault = head.width;
            head.useTextFormat = useTextFormat;
            string css = (!string.IsNullOrEmpty(className) ? className + " " : "");

            if (alinhamento == Align.right)
                head.className = css + "text-end";
            else if (alinhamento == Align.center)
                head.className = css + "text-center";
            else
                head.className = css + "text-start";

            head.orderable = permiteOrdenacao;
            head.visible = visivel;
            head.phoneHide = ocultaPhone;
            head.tabletHide = ocultaTablet;
            head.sumary = sumary;
            head.position = position;
            head.editableCell = editableCell != null ? editableCell : new EditableCell();
            head.dynamicCode = CodigoDinamico;
            head.enableGroup = permiteAgrupar;
            head.insertInGroupByWhenInvisible = adicionarAoAgrupamentoQuandoInvisivel;
            head.tabName = tab != null ? tab.name : "";
            header.Add(head);

            return head;
        }

        private byte[] GerarCSVGrid(List<Head> cabecalhos, List<dynamic> registros)
        {
            StringBuilder conteudoArquivo = new StringBuilder();
            List<Head> cabecalhosVisiveis = new List<Head>();
            int totalCabecalhos = cabecalhos.Count();

            for (var i = 0; i < totalCabecalhos; i++)
            {
                Head cabecalho = cabecalhos[i];

                if (!cabecalho.visible)
                    continue;

                conteudoArquivo.Append(cabecalho.title);

                if (this.indiceColunaOrdena == i)
                    conteudoArquivo.Append($" ({(this.dirOrdena == "asc" ? "Crescente" : "Decrescente")})");

                if ((i + 1) < totalCabecalhos)
                    conteudoArquivo.Append(";");

                cabecalhosVisiveis.Add(cabecalho);
            }

            int totalRegistros = registros.Count();
            int totalCabecalhosVisiveis = cabecalhosVisiveis.Count();
            
            conteudoArquivo.AppendLine("");

            for (var indiceLinha = 0; indiceLinha < totalRegistros; indiceLinha++)
            {
                dynamic registro = registros[indiceLinha];

                for (var indiceColuna = 0; indiceColuna < totalCabecalhosVisiveis; indiceColuna++)
                {
                    Head cabecalho = cabecalhosVisiveis[indiceColuna];
                    dynamic valorPropriedade;

                    //Se foi colunas adicionadas dinamicamente sem valor... no grid.AdicionaRows(rows);
                    if (ObterValorPropriedade(registro, cabecalho.data, out valorPropriedade))
                    {
                        if (valorPropriedade != null)
                        {
                            dynamic tipoPropriedade = valorPropriedade.GetType();

                            if (tipoPropriedade == typeof(string))
                            {
                                valorPropriedade = ((string)valorPropriedade).Replace("\n", " ");
                                valorPropriedade = ((string)valorPropriedade).Replace("\r", " ");
                                valorPropriedade = ((string)valorPropriedade).Replace(";", "|");
                            }

                            if ((tipoPropriedade == typeof(String)) && cabecalho.useTextFormat)
                                conteudoArquivo.Append($"= \"{valorPropriedade?.ToString() ?? ""}\"");
                            else
                                conteudoArquivo.Append(valorPropriedade);
                        }

                        if ((indiceColuna + 1) < totalCabecalhosVisiveis)
                            conteudoArquivo.Append(";");
                    }
                    else
                    {
                        dynamic valor = this.data[indiceLinha][cabecalho.data];

                        if (valor != null)
                        {
                            if (cabecalho.useTextFormat)
                                conteudoArquivo.Append($"= \"{valor?.ToString() ?? ""}\"");
                            else
                                conteudoArquivo.Append(valor);
                        }

                        if ((indiceColuna + 1) < totalCabecalhosVisiveis)
                            conteudoArquivo.Append(";");
                    }
                }

                if ((indiceLinha + 1) < totalRegistros)
                    conteudoArquivo.AppendLine("");
            }

            string caminho = this.ObterCaminho();
            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string extensao = "." + this.extensaoCSV;
            string arquivoTemporario = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensao);

            Utilidades.IO.FileStorageService.Storage.WriteAllText(arquivoTemporario, conteudoArquivo.ToString(), Encoding.UTF8);
            
            byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoTemporario);

            return arquivoBinario;
        }

        private string ObterCaminho()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoArquivos"], "GridExportacao");

            return caminho;
        }

        private bool ObterValorPropriedade(dynamic linha, string nomePropriedade, out object valor)
        {
            if (linha is ExpandoObject)
            {
                if (((IDictionary<string, object>)linha).TryGetValue(nomePropriedade, out object valorPropriedade))
                {
                    valor = valorPropriedade;
                    return true;
                }
            }
            else if (linha.GetType().GetProperty(nomePropriedade) != null)
            {
                valor = linha.GetType().GetProperty(nomePropriedade).GetValue(linha, null);
                return true;
            }

            valor = null;
            return false;
        }

        #endregion

        #region Métodos Públicos

        public void AdicionaRows(dynamic linhas)
        {
            this.DataRows = new List<dynamic>();
            this.data = new List<dynamic>();

            if (linhas == null)
                return;

            foreach (dynamic linha in linhas)
            {
                this.DataRows.Add(linha);
                var dictionary = new Dictionary<string, dynamic>();

                if (ObterValorPropriedade(linha, "Codigo", out object codigo))
                    dictionary["DT_RowId"] = codigo;

                if (ObterValorPropriedade(linha, "DT_RowId", out object idLinha))
                    dictionary["DT_RowId"] = idLinha;

                if (ObterValorPropriedade(linha, "DT_RowClass", out object classeLinha))
                    dictionary["DT_RowClass"] = classeLinha;
                else
                    dictionary["DT_RowClass"] = "";

                if (ObterValorPropriedade(linha, "DT_RowColor", out object corLinha))
                    dictionary["DT_RowColor"] = corLinha;
                else
                    dictionary["DT_RowColor"] = "";

                if (ObterValorPropriedade(linha, "DT_Enable", out object linhaHabilitada))
                    dictionary["DT_Enable"] = linhaHabilitada;
                else
                    dictionary["DT_Enable"] = true;

                if (ObterValorPropriedade(linha, "DT_FontColor", out object corFonte))
                    dictionary["DT_FontColor"] = corFonte;
                else
                    dictionary["DT_FontColor"] = "";

                foreach (Head head in header)
                {
                    dynamic valorPropriedade;

                    if (!ObterValorPropriedade(linha, head.data, out valorPropriedade))
                        continue;

                    if (valorPropriedade == null)
                    {
                        dictionary[head.data] = "";
                        continue;
                    }

                    var tipoValorPropriedade = valorPropriedade.GetType();

                    if (tipoValorPropriedade == typeof(Column))
                    {
                        //row.Colunas.Add((Column)collumnValue);
                    }
                    else
                    {
                        dynamic valorPropriedadeFormatado = null;

                        if (!string.IsNullOrWhiteSpace(head.numberFormat) && (tipoValorPropriedade == typeof(decimal) || tipoValorPropriedade == typeof(double)))
                            valorPropriedadeFormatado = valorPropriedade.ToString(head.numberFormat);
                        else if (tipoValorPropriedade == typeof(decimal))
                            valorPropriedadeFormatado = valorPropriedade.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                        else if (tipoValorPropriedade.IsEnum)
                            valorPropriedadeFormatado = valorPropriedade;
                        else if (tipoValorPropriedade == typeof(bool))
                            valorPropriedadeFormatado = valorPropriedade;
                        else if (tipoValorPropriedade == typeof(DateTime) && valorPropriedade != null)
                        {
                            if (valorPropriedade != DateTime.MinValue)
                                valorPropriedadeFormatado = valorPropriedade.ToString(head.dateTimePattern);
                            else
                                valorPropriedadeFormatado = "";
                        }
                        else
                            valorPropriedadeFormatado = valorPropriedade.ToString();

                        dictionary[head.data] = valorPropriedadeFormatado;
                    }
                }

                this.data.Add((dynamic)dictionary);
            }
        }

        public void AplicarPreferenciasGrid(Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGrid)
        {
            if (preferenciaGrid == null || string.IsNullOrWhiteSpace(preferenciaGrid.Dados))
                return;

            Dominio.ObjetosDeValor.Grid.PreferenciaGrid preferencia = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Grid.PreferenciaGrid>(preferenciaGrid.Dados);

            if (preferencia == null)
                return;

            int total = preferencia.columns.Count;
            int totalHead = header.Count;

            for (int i = 0; i < total; i++)
            {
                for (int j = 0; j < totalHead; j++)
                {
                    if (!string.IsNullOrWhiteSpace(header[j].name) && preferencia.columns[i].name == header[j].name)
                    {
                        header[j].visible = preferencia.columns[i].visible;

                        if (!string.IsNullOrWhiteSpace(preferencia.columns[i].width))
                            header[j].width = preferencia.columns[i].width;

                        if (preferencia.columns[i].position > 0)
                            header[j].position = preferencia.columns[i].position;

                        break;
                    }
                }
            }

            header = header.OrderBy(o => o.position).ToList();

            // Ajusta as posições das colunas mantendo únicas
            for (int i = 0; i < totalHead; i++)
            {
                for (int j = i + 1; j < totalHead; j++)
                {
                    if (header[i].position >= header[j].position)
                    {
                        int newPosition = header[i].position;

                        for (int k = j; k < totalHead; k++)
                            header[k].position = ++newPosition;
                    }
                }
            }

            this.scrollHorizontal = preferencia.scrollHorizontal;
        }

        //internal void AdicionaRows(Func<Grid> obterGridPesquisa)
        //{
        //    throw new NotImplementedException();
        //}

        public byte[] GerarExcel()
        {
            this.extensaoExcel = "xlsx";
            this.extensaoCSV = "csv";
            List<Head> cabecalhos = new List<Head>();

            foreach (Head head in this.header)
            {
                Head cabecalho = new Head();

                cabecalho.title = head.title;
                cabecalho.visible = head.visible;
                cabecalho.data = head.data;
                string className = head.className.Replace("text-", "");

                if (className == "start")
                    cabecalho.className = "left";
                else if (className == "end")
                    cabecalho.className = "right";
                else
                    cabecalho.className = className;

                cabecalho.useTextFormat = head.useTextFormat;

                if (head.visible)
                {
                    decimal tamanhoColuna = (!string.IsNullOrWhiteSpace(head.width)) ? head.width.Replace(".", ",").Replace("%", "").ToDecimal() : 0;

                    if (tamanhoColuna > 0)
                        cabecalho.width = head.width.Replace(".", ",").Replace("%", "");
                    else
                        cabecalho.width = head.widthDefault.Replace(".", ",").Replace("%", "");
                }
                else
                    cabecalho.width = "0,00";

                cabecalhos.Add(cabecalho);
            }

            return this.GerarCSVGrid(cabecalhos, this.DataRows);
            // Task.Factory.StartNew(() => this.GerarExcelGrid(cabecalhos, this.data, stringConexao));
        }

        public Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return ObterParametrosConsulta(ObterPropriedadeOrdenar: null);
        }

        public Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta(Func<string, string> ObterPropriedadeOrdenar)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoAgrupar = group?.enable ?? false ? group.dirOrdena : dirOrdena,
                DirecaoOrdenar = dirOrdena,
                InicioRegistros = inicio,
                LimiteRegistros = limite,
                PropriedadeAgrupar = group?.enable ?? false ? group.propAgrupa : ""
            };

            parametrosConsulta.PropriedadeOrdenar = (ObterPropriedadeOrdenar == null) ? header[indiceColunaOrdena].data : ObterPropriedadeOrdenar(header[indiceColunaOrdena].data);

            return parametrosConsulta;
        }

        public void setarQuantidadeTotal(int total)
        {
            this.recordsFiltered = total;
            this.recordsTotal = total;
        }

        #endregion

        #region Métodos Públicos - Adicionar Cabeçalho

        public Head AdicionarCabecalho(string propriedade, bool visivel)
        {
            return AdicionarCabecalho("", propriedade, 0, Align.left, false, false, false, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade)
        {
            return AdicionarCabecalho(descricao, propriedade, 0, Align.left, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string propriedade, bool visivel, bool adicionarAoAgrupamentoQuandoInvisivel)
        {
            return AdicionarCabecalho("", propriedade, 0, Align.left, false, false, false, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, adicionarAoAgrupamentoQuandoInvisivel, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool visible)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, visible, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, Align.left, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, false, false, false, true, sumary, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool visible,Tab tab)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, visible, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false, null, tab);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, string className)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false, className);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, int posicao)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, posicao, false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, true, sumary, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, bool visible)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, visible, sumary, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar, bool visivel)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar, bool visivel, int decimalPrecision)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar, null, false, decimalPrecision, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar, bool visivel, string dataTypeExportacao)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar, null, false, 0, dataTypeExportacao, false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar, bool visivel, EditableCell editableCell)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar, editableCell, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, sumary, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, 0, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, CodigoDinamico, header.Count(), false, null, false, 0, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico, int decimalPrecision)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, CodigoDinamico, header.Count(), false, null, false, decimalPrecision, "", false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico, int decimalPrecision, string dataTypeExportacao)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, CodigoDinamico, header.Count(), false, null, false, decimalPrecision, dataTypeExportacao, false);
        }

        public Head AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico, int decimalPrecision, string dataTypeExportacao, bool useTextFormat)
        {
            return AdicionarCabecalho(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, CodigoDinamico, header.Count(), false, null, false, decimalPrecision, dataTypeExportacao, useTextFormat);
        }

        public Head Prop(string propriedade)
        {
            return AdicionarCabecalho("", propriedade, 0, Align.left, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false, null, false, 0, "", false);
        }

        public void OcultarCabecalho(string headerName)
        {
            int total = header.Count;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(header[i].name) && header[i].name == headerName)
                {
                    header[i].visible = false;
                    break;
                }
            }
        }

        public void OcultarCabecalhos(string[] headerNames)
        {
            int total = header.Count;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(header[i].name) && headerNames.Contains(header[i].name))
                {
                    header[i].visible = false;
                }
            }
        }

        public void DesabilitarTodasOrdenacoes()
        {
            int total = header.Count;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(header[i].name))
                {
                    header[i].orderable = false;
                }
            }
        }

        #region Métodos Públicos - Adicionar Tab

        // Se for necessário adicionar parâmetros, por favor, torne esse método privado, assim como o "private AdicionarCabecalho"
        public void AdicionarTab(string descricao, out Tab newTab)
        {
            newTab = new Tab();
            newTab.name = descricao;
            listTabs.Add(newTab);
        }

        #endregion

        #endregion

    }
}
