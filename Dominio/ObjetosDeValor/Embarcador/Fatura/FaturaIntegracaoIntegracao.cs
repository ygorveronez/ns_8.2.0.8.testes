using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FaturaIntegracaoIntegracao
    {
        public int CodigoFatura { get; set; }
        public string NomeArquivo { get; set; }
        public bool IniciouConexaoExterna { get; set; }        
        public int ReenviarAutomaticamenteOutraVezAposMinutos { get; set; }
        public string LayoutEDI { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }        
        public ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura TipoIntegracaoFatura { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }
        public int Tentativas { get; set; }
        public DateTime? DataEnvio { get; set; }
        public string MensagemRetorno { get; set; }
        public string CodigoIntegracaoIntegradora { get; set; }
        public bool UsarCST { get; set; }
        public bool ModeloCTe { get; set; }
        public string TipoImposto { get; set; }
        public string EmailEnvio { get; set; }

    }
}
