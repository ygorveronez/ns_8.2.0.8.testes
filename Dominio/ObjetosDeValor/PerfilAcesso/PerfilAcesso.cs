using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.PerfilAcesso
{
    public class PerfilAcesso
    {
        public int CodigoEmpresa { get; set; }
        public int CodigoPerfilAcessoTransportador { get; set; }
        public bool TransportadorAdministrador { get; set; }
        public List<int> CodigosFormularios { get; set; }
        public List<int> Modulos { get; set; }
    }
}
