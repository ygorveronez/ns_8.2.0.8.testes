using Dominio.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Administrativo
{
    public class LogAcesso
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Usuario { get; set; }
        private string CPFUsuario { get; set; }
        public string IPAcesso { get; set; }
        public string Login { get; set; }
        private Dominio.Enumeradores.TipoLogAcesso Tipo { get; set; }
        private DateTime Data { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string CPFUsuarioFormatado 
        {
            get { return !string.IsNullOrWhiteSpace(CPFUsuario) ? CPFUsuario.ToString().ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        #endregion
    }
}
