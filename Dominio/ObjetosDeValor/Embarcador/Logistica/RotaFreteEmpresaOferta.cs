using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RotaFreteEmpresaOferta
    {
        public Entidades.Empresa Empresa { get; set; }
        
        public string Descricao { get; set; }
        
        public int Ordem { get; set; }
        
        public decimal PercentualCargas { get; set; }
        
        public decimal PercentualConfigurado { get; set; }

        public int Prioridade { get; set; }

        public TipoHistoricoOfertaTransportador Tipo { get; set; }
    }
}
