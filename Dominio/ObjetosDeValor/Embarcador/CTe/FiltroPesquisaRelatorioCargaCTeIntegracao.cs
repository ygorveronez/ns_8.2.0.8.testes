using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaRelatorioCargaCTeIntegracao
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataIntegracaoInicial { get; set; }
        public DateTime DataIntegracaoFinal { get; set; }
        public int GrupoPessoas { get; set; }
        public int CTe { get; set; }
        public int Carga { get; set; }
        public int TipoIntegracao { get; set; }
    }
}
