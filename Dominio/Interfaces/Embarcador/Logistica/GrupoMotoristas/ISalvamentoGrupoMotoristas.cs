using System.Collections.Generic;

namespace Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas
{
    public interface ISalvamentoGrupoMotoristas
    {
        Entidades.Embarcador.Logistica.GrupoMotoristas GrupoMotoristas { get; }
        List<ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> TiposIntegracao { get; set; }
    }
}
