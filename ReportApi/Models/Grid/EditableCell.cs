namespace ReportApi.Models.Grid
{
    public class EditableCell
    {
        #region Propriedades

        public bool editable { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid type { get; set; }

        public string mask { get; set; }

        public int maxlength { get; set; }

        public NumberMask numberMask { get; set; }

        #endregion Propriedades

        #region Construtores

        public EditableCell()
        {
            SetDefautNumberMask();
        }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid) : this(tipoColunaGrid, mascaraCampo: "", tamanhoCampo: 0) { }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid,  int tamanhoCampo) : this(tipoColunaGrid, mascaraCampo: "", tamanhoCampo) { }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid, string mascaraCampo, int tamanhoCampo)
        {
            this.editable = true;
            this.type = tipoColunaGrid;
            this.mask = mascaraCampo;
            this.maxlength = tamanhoCampo;

            SetDefautNumberMask();
        }

        #endregion Construtores

        #region Métodos Privados

        private void SetDefautNumberMask()
        {
            if(this.type == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal)
                this.numberMask = new NumberMask(2);
            else
                this.numberMask = new NumberMask();
        }

        #endregion Métodos Privados
    }
}