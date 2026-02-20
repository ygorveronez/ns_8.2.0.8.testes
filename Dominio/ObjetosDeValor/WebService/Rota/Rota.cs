using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rota
{
    public class Rota
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public List<Embarcador.Pessoas.Pessoa> Destinatarios { get; set; }
        public decimal Quilometros { get; set; }
        public int TempoViagemMinutos { get; set; }
        public bool? Ativo { get; set; }
        public string Polilinha { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }
        public List<RotaPontoPassagem> PontosPassagem { get; set; }
        public Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }
    }
}
