using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoMotorista
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoGrupoModeloVeicularCarga { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public bool RetornarMotoristaComReboqueAtrelado { get; set; }
    }
}
