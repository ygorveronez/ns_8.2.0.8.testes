using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador
{
    public class FiltroPesquisaOcorrenciaIntegracaoEmbarcador
    {
        public List<int> CodigoEmpresa { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador> Situacao { get; set; }
        public string NumeroOcorrencia { get; set; }
        public string NumeroOcorrenciaEmbarcador{ get; set; }
        public DateTime? DataInicialOcorrencia { get; set; }
        public DateTime? DataFinalOcorrencia { get; set; }
        public List<int> CodigoGrupoPessoa { get; set; }
    }
}
