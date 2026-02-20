using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{
    public class retornoWebService
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public string ProblemaIntegracao { get; set; }

        public string jsonRequisicao { get; set; }

        public string jsonRetorno { get; set; }
    }
}