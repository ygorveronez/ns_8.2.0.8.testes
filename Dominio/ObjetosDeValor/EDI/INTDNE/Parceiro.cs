using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTDNE
{
    public class Parceiro
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string TipoOperacao { get; set; }
        public string CNPJParceiroComercial { get; set; }
        public string TipoParceiroComercial { get; set; }
        public string CodigoParceiroComerial { get; set; }
        public string NomeParceiroComerial { get; set; }
        public string ZonaTransporte { get; set; }
        public string TipoPessoa { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string InscricaoEstadual { get; set; }
        public int STICMS { get; set; }
        public int STRegimeICMS { get; set; }
        public int STExcluriReferenciaExternaParceirosComercial { get; set; }
        public List<Notas> Notas { get; set; }

    }
}
