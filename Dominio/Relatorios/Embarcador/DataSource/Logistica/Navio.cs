using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class Navio
    {
        #region Propriedades
        
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        private bool Status { get; set; }
        public string CodigoIrin { get; set; }
        public string CodigoEmbarcacao { get; set; }
        private TipoEmbarcacao TipoEmbarcacao { get; set; }
        public string CodigoDocumentacao { get; set; }
        public string CodigoIMO { get; set; }
        public string NavioID { get; set; }

        #endregion

        #region Propriedades com Regras

        public string StatusDescricao
        {
            get { return Status ? "Ativo" : "Inativo"; }
        }

        public string TipoEmbarcacaoDescricao
        {
            get { return TipoEmbarcacao.ObterDescricao(); }
        }

        #endregion

    }
}
