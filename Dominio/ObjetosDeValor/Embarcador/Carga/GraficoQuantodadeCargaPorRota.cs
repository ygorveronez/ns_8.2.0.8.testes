using CsvHelper.Configuration.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class GraficoQuantodadeCargaPorRota
    {
        [Name("Quantidade")]
        [Index(1)]
        public virtual int Quantidade { get; set; }

        [Name("Rota")]
        [Index(0)]
        public virtual string Rota { get; set; }        
    }
}
