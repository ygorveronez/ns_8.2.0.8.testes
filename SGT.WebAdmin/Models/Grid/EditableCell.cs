namespace SGT.WebAdmin.Models.Grid
{
    public class EditableCell
    {
        #region Propriedades

        public bool editable { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid type { get; set; }

        public string mask { get; set; }

        public int maxlength { get; set; }

        public NumberMask numberMask { get; set; }

        public int precisionDecimal {  get; set; }

        #endregion Propriedades

        #region Construtores

        public EditableCell()
        {
            SetDefautNumberMask();
        }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid) : this(tipoColunaGrid, mascaraCampo: "", tamanhoCampo: 0, precisionDecimal: 2) { }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid, int tamanhoCampo, int precisionDecimal = 2) : this(tipoColunaGrid, mascaraCampo: "", tamanhoCampo, precisionDecimal) { }

        public EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid tipoColunaGrid, string mascaraCampo, int tamanhoCampo, int precisionDecimal = 2)
        {
            this.editable = true;
            this.type = tipoColunaGrid;
            this.mask = mascaraCampo;
            this.maxlength = tamanhoCampo;
            this.precisionDecimal = precisionDecimal;

            SetDefautNumberMask();
        }

        #endregion Construtores

        #region Métodos Privados

        private void SetDefautNumberMask()
        {
            if(this.type == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal)
                this.numberMask = new NumberMask(this.precisionDecimal);
            else
                this.numberMask = new NumberMask();
        }

        #endregion Métodos Privados
    }
}