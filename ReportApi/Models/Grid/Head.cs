namespace ReportApi.Models.Grid
{
    public class Head
    {
        public Head()
        {
            this.dateTimePattern = "dd/MM/yyyy HH:mm:ss";
        }

        public string title { get; set; }
        public string data { get; set; }
        public string name { get; set; }
        public string width { get; set; }
        public string widthDefault { get; set; }
        public bool orderable { get; set; }
        public bool visible { get; set; }
        public string className { get; set; }
        public string dateTimePattern { get; set; }
        public bool tabletHide { get; set; }
        public bool phoneHide { get; set; }
        public int position { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao sumary { get; set; }
        public bool enableGroup { get; set; }
        public int dynamicCode { get; set; }
        public int decimalPrecision { get; set; }
        public string dataTypeExportacao { get; set; }
        public EditableCell editableCell { get; set; }
        public bool insertInGroupByWhenInvisible { get; set; }
        public string numberFormat { get; set; }
        public bool useTextFormat { get; set; }
        public string tabName { get; set; }

        /// <summary>
        /// Define o nome do campo (Descritivo)
        /// </summary>
        /// <param name="nome">Valor padrão: ""</param>
        public Head Nome(string nome)
        {
            this.title = nome;
            if (!string.IsNullOrWhiteSpace(nome))
                this.visible = true;

            return this;
        }

        /// <summary>
        /// Define se a coluna aparece na grid
        /// Define se vem marcada (true) ou não (false) nos relatórios
        /// </summary>
        /// <param name="visible">Valor padrão: true</param>
        public Head Visibilidade(bool visible)
        {
            this.visible = visible;

            return this;
        }

        /// <summary>
        /// Define se a coluna aparece na grid
        /// Define se a coluna aparece em diferentes dimensões
        /// Define se vem marcada (true) ou não (false) nos relatórios
        /// </summary>
        /// <param name="visible">Valor padrão: true</param>
        /// <param name="phone">>Valor padrão: true</param>
        /// <param name="table">>Valor padrão: true</param>
        public Head Visibilidade(bool visible, bool phone, bool table)
        {
            this.visible = visible;
            this.phoneHide = !phone;
            this.tabletHide = !table;

            return this;
        }

        /// <summary>
        /// Define se a coluna deve ser oculta (sobrepõe visibilidade)
        /// </summary>
        /// <param name="ocultar">Não há valor padrão</param>
        public Head Ocultar(bool ocultar)
        {
            if (ocultar)
            {
                this.visible = false;
                this.title = "";
            }

            return this;
        }

        /// <summary>
        /// Define se o campo possui algum timpo de sumarizador
        /// </summary>
        /// <param name="summary">Valor padrão: nenhum</param>
        public Head Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao summary)
        {
            this.sumary = summary;

            return this;
        }

        /// <summary>
        /// Define se o campo pode ser ordenado ou agrupado
        /// </summary>
        /// <param name="ordenar">Valor padrão: true</param>
        /// <param name="agrupar">Valor padrão: false</param>
        public Head OrdAgr(bool ordenar, bool agrupar)
        {
            this.orderable = ordenar;
            this.enableGroup = agrupar;

            return this;
        }

        /// <summary>
        /// Define se o campo deve ser inserido na cláusula group by quando não estiver visível
        /// </summary>
        /// <param name="adicionar">Valor padrão: false</param>
        public Head AdicionarAoAgrupamentoQuandoInvisivel(bool adicionar)
        {
            this.insertInGroupByWhenInvisible = adicionar;

            return this;
        }

        /// <summary>
        /// Define se o campo pode ser agrupado
        /// </summary>
        /// <param name="agrupar">Valor padrão: false</param>
        public Head Agr(bool agrupar)
        {
            this.enableGroup = agrupar;

            return this;
        }

        /// <summary>
        /// Define se o campo pode ser ordenado
        /// </summary>
        /// <param name="ordenar">Valor padrão: true</param>
        public Head Ord(bool ordenar)
        {
            this.orderable = ordenar;

            return this;
        }

        /// <summary>
        /// Define se o campo deve utilizar a formatação do tipo texto
        /// </summary>
        /// <param name="formatoTexto">Valor padrão: false</param>
        public Head UtilizarFormatoTexto(bool formatoTexto)
        {
            this.useTextFormat = formatoTexto;

            return this;
        }

        /// <summary>
        /// Define o tamanho do campo
        /// </summary>
        /// <param name="tamanho">Valor padrão: 0</param>
        public Head Tamanho(decimal tamanho)
        {
            this.width = tamanho.ToString().Replace(",", ".") + "%";
            this.widthDefault = this.width;

            return this;
        }

        /// <summary>
        /// Define o tipo de alinhamento do campo
        /// </summary>
        /// <param name="alinhamento">Valor padrão: left</param>
        public Head Align(Models.Grid.Align alinhamento)
        {
            if (alinhamento == Models.Grid.Align.right)
                this.className = "text-align-right";
            else if (alinhamento == Models.Grid.Align.center)
                this.className = "text-align-center";
            else
                this.className = "text-align-left";

            return this;
        }

        public Head DataTypeExportacao(string dataType)
        {
            this.dataTypeExportacao = dataType;

            return this;
        }

        /// <summary>
        /// Define as opcoes editaveis do campo
        /// </summary>
        /// <param name="editable">Valor padrão: null</param>
        public Head Editable(EditableCell editable)
        {
            this.editableCell = editable;

            return this;
        }

        /// <summary>
        /// Remove a edição do campo
        /// </summary>
        public Head Uneditable()
        {
            this.editableCell = new EditableCell();

            return this;
        }

        /// <summary>
        /// Exibe apenas a data em colunas no tipo DateTime
        /// </summary>
        public Head DateTimeOnlyDate()
        {
            this.dateTimePattern = "dd/MM/yyyy";

            return this;
        }

        /// <summary>
        /// Exibe apenas a hora em colunas no tipo DateTime
        /// </summary>
        public Head DateTimeOnlyTime()
        {
            this.dateTimePattern = "HH:mm:ss";

            return this;
        }

        /// <summary>
        /// Exibe um formato específico no tipo DateTime
        /// </summary>
        public Head DateTimePattern(string pattern)
        {
            this.dateTimePattern = pattern;

            return this;
        }

        /// <summary>
        /// Formato de conversão para decimal/double
        /// </summary>
        public Head NumberFormat(string format)
        {
            this.numberFormat = format;

            return this;
        }
    }
}
