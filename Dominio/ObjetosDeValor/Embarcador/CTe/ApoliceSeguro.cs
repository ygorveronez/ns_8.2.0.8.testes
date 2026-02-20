using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ApoliceSeguro
    {
        public string NumeroApolice { get; set; }
        public string NumeroAverbacao { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FimVigencia { get; set; }
        public DateTime? DataUltimoAlerta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro Responsavel { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao SeguradoraAverbacao { get; set; }
        public Seguradora Seguradora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public decimal ValorLimiteApolice { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa GrupoPessoa { get; set; }
        public string Observacao { get; set; }
    }
}
