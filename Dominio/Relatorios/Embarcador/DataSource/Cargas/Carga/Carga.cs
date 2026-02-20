using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class Carga
    {
        public int Codigo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string Filial { get; set; }
        public string OrigemDestino { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string DataCarregamento { get; set; }
        public string Motorista { get; set; }
        public string NumeroCTes { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> Integracoes { get; set; }
    }
}
