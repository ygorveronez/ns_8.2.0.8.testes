using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ContratoFreteAcrescimoDesconto
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Justificativa { get; set; }
        public int NumCiot { get; set; }
        public int NumContratoFrete { get; set; }
        public int NumCarga { get; set; }
        public string Terceiro { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
    }
}
