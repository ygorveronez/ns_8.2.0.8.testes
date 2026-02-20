using System;

namespace Dominio.ObjetosDeValor.WebService.Devolucao
{
    public class AtualizarLaudo
    {
        public string CodFornecedor { get; set; }

        public string Atribuicao { get; set; }

        public int NumeroLaudo { get; set; }

        public DateTime? DataVencimento { get; set; }

        public DateTime? DataCompensacao { get; set; }

        public decimal Valor { get; set; }

        public string NumeroCompensacao { get; set; }
    }
}
