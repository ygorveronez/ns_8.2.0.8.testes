using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    [DataContract]
    public class ComponenteAdicional
    {
        [DataMember]
        public Componente Componente { get; set; }

        [DataMember]
        public decimal ValorComponente { get; set; }

        [DataMember]
        public bool IncluirBaseCalculoICMS { get; set; }

        [DataMember]
        public bool DescontarValorTotalAReceber { get; set; }
        
        public bool IncluirTotalReceber { get; set; }

        public string Descricao { get; set; }
    }
}
