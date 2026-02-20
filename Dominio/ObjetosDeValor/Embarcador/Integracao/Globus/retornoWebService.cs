using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class retornoWebService
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public string ProblemaIntegracao { get; set; }

        public string jsonRequisicao { get; set; }

        public string jsonRetorno { get; set; }

    }
}