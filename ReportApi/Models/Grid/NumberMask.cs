namespace ReportApi.Models.Grid
{
    public class NumberMask
    {
        #region Propriedades

        public bool allowZero { get; set; }

        public int precision { get; set; }

        public bool thousandsSeparator { get; set; }

        #endregion Propriedades

        #region Construtores

        public NumberMask() : this(precision: 0, allowZero: false, thousandsSeparator: true) { }

        public NumberMask(int precision) : this(precision, allowZero: false, thousandsSeparator: true) { }

        public NumberMask(int precision, bool allowZero) : this(precision, allowZero, thousandsSeparator: true) { }

        public NumberMask(int precision, bool allowZero, bool thousandsSeparator)
        {
            this.allowZero = allowZero;
            this.precision = precision;
            this.thousandsSeparator = thousandsSeparator;
        }

        #endregion Construtores
    }
}