using System.Collections.Generic;

namespace Dominio.Entidades.SINTEGRA
{
    public class _90 : Registro
    {
        #region Construtores

        public _90()
            : base("90")
        {
            this.Totalizadores = new Dictionary<string, int>();
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Empresa Empresa { get; set; }

        public IDictionary<string, int> Totalizadores { get; set; }

        public int TotalRegistros90 { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            int count = 0;

            this.EscreverDado(this.Empresa.CNPJ, 14);
            this.EscreverDado(this.Empresa.InscricaoEstadual, 14);

            foreach (KeyValuePair<string, int> total in Totalizadores)
            {
                this.EscreverDado(total.Key, 2);
                this.EscreverDado(total.Value, 8);
                count += 1;
            }

            if (count < 9)
                this.EscreverDado("", ((9 - count) * 10));

            this.EscreverDado("", 5);

            this.EscreverDado(this.TotalRegistros90, 1);

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
