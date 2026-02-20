using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CRM
{
    public class AgendaTarefas
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Observacao { get; set; }
        private DateTime DataInicial { get; set; }
        private DateTime DataFinal { get; set; }
        public string Colaborador { get; set; }
        public string Cliente { get; set; }

        #endregion

        #region  Propriedades com Regras

        public string DataInicialFormatada
        {
            get { return DataInicial != DateTime.MinValue ? DataInicial.ToString("dd/MM/yyyy") : ""; }
        }
        public string DataFinalFormatada
        {
            get { return DataFinal != DateTime.MinValue ? DataFinal.ToString("dd/MM/yyyy") : ""; }
        }

        #endregion
    }
}
