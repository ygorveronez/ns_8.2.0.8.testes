namespace Dominio.ObjetosDeValor.Embarcador.Imposto.OutrasAliquotas
{
    public class DadosOutrasAliquotas
    {
        #region Propriedades Publicas
        public int Codigo { get; set; }
        public int CodigoTipoOperacao { get; set; }

        public bool StatusAtividade { get; set; }

        public string CST { get; set; }

        public string CodigoClassificacaoTributaria { get; set; }
        public string CodigoIndicadorOperacao { get; set; }

        public string TipoOperacao { get; set; }

        public bool ZerarBase { get; set; }

        public bool Exportacao { get; set; }

        public bool CalcularImpostoDocumento { get; set; }
        #endregion

        #region Propriedades Com Regra
        public string StatusAtividadeFormatado
        {
            get
            {
                return StatusAtividade ? "Ativo" : "Inativo";
            }
        }

        public string CalcularImpostoDocumentoFormatado
        {
            get
            {
                return CalcularImpostoDocumento ? "Sim" : "Não";
            }
        }
        #endregion

    }

}
