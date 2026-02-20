using System;
using System.Collections.Generic;

namespace Dominio.Interfaces.Embarcador.Integracao
{
    public interface IIntegracaoComArquivo<TIntegracaoArquivo> where TIntegracaoArquivo : Entidades.Embarcador.Integracao.IntegracaoArquivo
    {
        DateTime DataIntegracao { get; set; }

        string ProblemaIntegracao { get; set; }

        ICollection<TIntegracaoArquivo> ArquivosTransacao { get; set; }
    }
}
