using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class DadosRequisicaoCamil<T>
    {
        public List<T> Contabilizacao { get; set; }
        public List<T> Desbloqueio { get; set; }
        public List<T> Provisoes { get; set; }
        public List<T> Estorno { get; set; }
        public List<T> NotasDebito { get; set; }
        public long ProtocoloIntegracao { get; set; }
    }
}
