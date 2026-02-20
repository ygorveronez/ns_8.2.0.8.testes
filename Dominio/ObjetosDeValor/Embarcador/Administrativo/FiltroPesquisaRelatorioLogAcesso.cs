using System;

namespace Dominio.ObjetosDeValor.Embarcador.Administrativo
{
    public class FiltroPesquisaRelatorioLogAcesso
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoUsuario { get; set; }
        public Dominio.Enumeradores.TipoLogAcesso? TipoAcesso { get; set; }
    }
}
