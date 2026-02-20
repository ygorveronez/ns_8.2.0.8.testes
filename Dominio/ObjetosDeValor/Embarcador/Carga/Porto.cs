namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Porto
    {
        public int Codigo { get; set; }
        public string CodigoDocumento { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string CodigoIATA { get; set; }
        public Embarcador.Localidade.Endereco Localidade { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public bool Atualizar { get; set; }

        public bool InativarCadastro { get; set; }
        public string CodigoMercante { get; set; }
        public int QuantidadeHorasFaturamentoAutomatico { get; set; }
        public bool AtivarDespachanteComoConsignatario { get; set; }
        public bool DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino { get; set; }
        public bool DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem { get; set; }
        public int? DiasAntesDoPodParaEnvioDaDocumentacao { get; set; }
        public string RKST { get; set; }

    }
}
