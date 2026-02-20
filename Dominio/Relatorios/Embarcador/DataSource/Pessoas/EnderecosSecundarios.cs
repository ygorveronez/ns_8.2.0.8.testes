using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class EnderecosSecundarios
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CodigoEmbarcador { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Numero { get; set; }
        public string NomeCliente { get; set; }
        private double CPFCliente { get; set; }
        public string CPFClienteFormatado { get { return CPFCliente.ToString().ObterCpfFormatado(); } }

        

        #endregion
    }
}
