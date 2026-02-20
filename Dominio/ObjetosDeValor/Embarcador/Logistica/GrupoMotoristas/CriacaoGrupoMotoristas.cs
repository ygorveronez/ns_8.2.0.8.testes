using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class CriacaoGrupoMotoristas : Interfaces.Embarcador.Logistica.GrupoMotoristas.ISalvamentoGrupoMotoristas
    {
        public Entidades.Embarcador.Logistica.GrupoMotoristas GrupoMotoristas { get; set; }
        public List<FuturoRelacionamentoGrupoMotoristas> Motoristas { get; set; }
        public List<Enumeradores.TipoIntegracao> TiposIntegracao { get; set; }
    }
}
