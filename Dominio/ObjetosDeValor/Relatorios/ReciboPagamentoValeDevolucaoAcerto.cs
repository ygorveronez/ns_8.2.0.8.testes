using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class ReciboPagamentoValeDevolucaoAcerto
    {
        public int NumeroAcerto { get; set; }

        public int NumeroDocumento { get; set; }

        public DateTime Data { get; set; }

        public decimal Valor { get; set; }
    }
}
