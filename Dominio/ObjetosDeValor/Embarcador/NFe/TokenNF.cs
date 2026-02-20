namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class TokenNF
    {
        public string Token { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao AverbacaoNF { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe? ClassificacaoNFe { get; set; }
    }
}
