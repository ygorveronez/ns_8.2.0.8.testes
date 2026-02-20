using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaIntegradoraIntegracaoRetorno
    {
        public int CodigoIntegradora { get; set; }
        public string NumeroIdentificacao { get; set; }
        public bool? Sucesso { get; set; }
        public bool? PossuiCarga { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
