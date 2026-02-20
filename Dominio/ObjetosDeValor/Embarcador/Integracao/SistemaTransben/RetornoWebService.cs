using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben
{
    public class RetornoWebService
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public string ProblemaIntegracao { get; set; }

        public string JsonRequisicao { get; set; }

        public string JsonRetorno { get; set; }

    }

    public class RetornoWebServiceFalha
    {
        public string Mensagem { get; set; }
        public string Error { get; set; }

    }
}