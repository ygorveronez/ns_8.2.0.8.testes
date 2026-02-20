using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;

namespace AdminMultisoftwareApp.Models.Grid
{
    public class Grid
    {

        public Grid()
        {

        }
        
        public Grid(int draw)
        {
            this.draw = draw;
        }

        public Grid(HttpRequestBase request)
        {
            Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Grid>(request.Params["Grid"]);
            dynamic dynGrid = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(request.Params["Grid"]);

            this.header = grid.header;
            this.inicio = dynGrid.start;
            this.limite = dynGrid.length;
            this.indiceColunaOrdena = dynGrid.order[0].column;
            this.dirOrdena = dynGrid.order[0].dir;
            this.draw = grid.draw;

            if (this.header.Count > 0)
            {
                this.header = this.header.OrderBy(obj => obj.position).ToList();
            }

            this.group = grid.group;
        }

        public Grid(int draw, int recordsTotal)
        {
            this.header = new List<Head>();
            this.draw = draw;
            this.recordsFiltered = recordsTotal;
            this.recordsTotal = recordsTotal;
        }

        public int draw { get; set; }

        public int inicio { get; set; }

        public int limite { get; set; }

        public int indiceColunaOrdena { get; set; }

        public string dirOrdena { get; set; }

        public int recordsTotal { get; set; }

        public int recordsFiltered { get; set; }

        public Group group { get; set; }

        public List<Head> header { get; set; }

        public dynamic data { get; set; }

        public dynamic dataSumarizada { get; set; }

        public void setarQuantidadeTotal(int total)
        {
            this.recordsFiltered = total;
            this.recordsTotal = total;
        }

        public void AdicionaRows(dynamic linhas)
        {

            this.data = new List<dynamic>();

            foreach (dynamic linha in linhas)
            {

                var dictionary = new Dictionary<string, dynamic>();


                if (linha.GetType().GetProperty("Codigo") != null)
                {
                    dictionary["DT_RowId"] = linha.GetType().GetProperty("Codigo").GetValue(linha, null);
                }

                // informar o hexadecimal da cor desejada; exemplo: #123FFF
                if (linha.GetType().GetProperty("DT_RowColor") != null)
                {
                    dictionary["DT_RowColor"] = linha.GetType().GetProperty("DT_RowColor").GetValue(linha, null);
                }
                else
                {
                    dictionary["DT_RowColor"] = "";
                }

                foreach (Head head in header)
                {
                    if (linha.GetType().GetProperty(head.data) != null)
                    {
                        if (linha.GetType().GetProperty(head.data).GetValue(linha, null) != null)
                        {
                            if (linha.GetType().GetProperty(head.data).GetValue(linha, null).GetType() == typeof(Column))
                            {
                                //row.Colunas.Add((Column)linha.GetType().GetProperty(head.data).GetValue(linha, null));
                            }
                            else
                            {
                                Column col = new Column();

                                {
                                    if (linha.GetType().GetProperty(head.data).GetValue(linha, null).GetType() == typeof(decimal))
                                    {
                                        dictionary[head.data] = linha.GetType().GetProperty(head.data).GetValue(linha, null).ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                                    }
                                    else
                                    {
                                        if (linha.GetType().GetProperty(head.data).GetValue(linha, null).GetType().IsEnum)
                                        {
                                            dictionary[head.data] = linha.GetType().GetProperty(head.data).GetValue(linha, null);
                                        }
                                        else
                                        {
                                            if (linha.GetType().GetProperty(head.data).GetValue(linha, null).GetType() == typeof(bool))
                                            {
                                                dictionary[head.data] = linha.GetType().GetProperty(head.data).GetValue(linha, null);
                                            }
                                            else
                                            {
                                                dictionary[head.data] = linha.GetType().GetProperty(head.data).GetValue(linha, null).ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            dictionary[head.data] = "";
                        }
                    }
                }
                this.data.Add((dynamic)dictionary);
            }
        }

        public Column RetornaColuna(string descricao, string cor)
        {
            Column col = new Column();
            col.Descricao = descricao;
            col.CorFundo = cor;
            return col;
        }

        public void AdicionarCabecalho(string propriedade, bool visivel)
        {
            preecharHead("", propriedade, 0, Align.left, false, false, false, visivel, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool visible)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, visible, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho)
        {
            preecharHead(descricao, propriedade, tamanho, Align.left, false, false, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, false, false, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, false, false, false, true, sumary, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, int posicao)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, false, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, posicao, false);
        }


        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, false, false, true, sumary, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar, bool visivel)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar);
        }
        
        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool permiteAgrupar)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao.nenhum, 0, header.Count(), permiteAgrupar);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, true, sumary, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, 0, header.Count(), false);
        }

        public void AdicionarCabecalho(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico)
        {
            preecharHead(descricao, propriedade, tamanho, alinhamento, permiteOrdenacao, ocultaTablet, ocultaPhone, visivel, sumary, CodigoDinamico, header.Count(), false);
        }

        private void preecharHead(string descricao, string propriedade, decimal tamanho, Align alinhamento, bool permiteOrdenacao, bool ocultaTablet, bool ocultaPhone, bool visivel, AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary, int CodigoDinamico, int position, bool permiteAgrupar)
        {
            Head head = new Head();
            head.title = descricao;
            head.data = propriedade;
            head.width = tamanho.ToString().Replace(",", ".") + "%";
            if (alinhamento == Align.right)
            {
                head.className = "text-align-right";
            }
            else if (alinhamento == Align.center)
            {
                head.className = "text-align-center";
            }
            else
            {
                head.className = "text-align-left";
            }

            head.orderable = permiteOrdenacao;
            head.visible = visivel;
            head.phoneHide = ocultaPhone;
            head.tabletHide = ocultaTablet;
            head.sumary = sumary;
            head.position = position;
            head.dynamicCode = CodigoDinamico;
            head.enableGroup = permiteAgrupar;
            header.Add(head);
        }

    }
}
