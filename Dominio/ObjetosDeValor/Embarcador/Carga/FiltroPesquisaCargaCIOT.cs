using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaCIOT
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
        public List <int> CodigosFiliais { get; set; }
        public List <double> CodigosRecebedores { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public Enumeradores.RegimeTributario RegimeTributario { get; set; }
    }
}
