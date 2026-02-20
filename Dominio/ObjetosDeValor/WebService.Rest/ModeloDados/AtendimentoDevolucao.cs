using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class AtendimentoDevolucao
    {
        public string Tratativa { get; set; }

        public string Tipo { get; set; }

        public List<EntregaNotaFiscal> NotasFiscais { get; set; }
    }
}
