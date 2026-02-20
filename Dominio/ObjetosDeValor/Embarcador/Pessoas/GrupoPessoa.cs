using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class GrupoPessoa
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public int Codigo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao> TiposOperacaoes;
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> ModelosVeiculares;
    }
}
