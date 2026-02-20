using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class AverbacaoOracle
    {
        public AverbacaoOracle()
        {
            this.Info = new Resultado();
            this.Averbacoes = new List<AverbacaoCTe>();
        }
        public int CodigoCTeInterno;
        public List<AverbacaoCTe> Averbacoes;
        public Resultado Info;
    }
}
