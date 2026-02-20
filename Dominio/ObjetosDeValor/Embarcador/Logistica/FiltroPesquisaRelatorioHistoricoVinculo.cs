using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioHistoricoVinculo
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Veiculo { get; set; }
        public int Motorista { get; set; }
        public int Pedido { get; set; }
        public int Carga { get; set; }
        public int FilaCarregamento { get; set; }
        public LocalVinculo? LocalVinculo { get; set; }
    }
}