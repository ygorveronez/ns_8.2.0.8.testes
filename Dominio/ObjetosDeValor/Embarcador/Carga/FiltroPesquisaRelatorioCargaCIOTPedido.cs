using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioCargaCIOTPedido
    {
        public string Carga { get; set; }

        public DateTime DataEncerramentoInicial { get; set; }

        public DateTime DataEncerramentoFinal { get; set; }

        public string Numero { get; set; }

        public double Proprietario { get; set; }

        public DateTime DataAberturaInicial { get; set; }

        public DateTime DataAberturaFinal { get; set; }

        public int Veiculo { get; set; }

        public int Motorista { get; set; }

        public Enumeradores.SituacaoCIOT? Situacao { get; set; }

        public int Transportador { get; set; }
    }
}
