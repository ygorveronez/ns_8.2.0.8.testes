using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class NFS
    {
        public int Protocolo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadePrestacaoServico { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }
        public Dominio.Enumeradores.TipoNotaFiscalServico TipoNotaFiscalServico { get; set; }
        public NFSe NFSe { get; set; }
        public NFSManual NFSManual { get; set; }
        public List<int> ProtocolosDePedidos { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolos { get; set; }
        public ValorNFS ValoresNFS { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil> ContasContabeis { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultado;

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultadoEscrituracao;

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultadoDestinatario;

        public string ItemServico { get; set; }
        public string CodigoServico { get; set; }
        public string UltimoRetornoSEFAZ { get; set; }
        public bool Cancelada { get; set; }
        
        public List<ObjetosDeValor.CTe.Observacao> ObservacoesContribuinte { get; set; }

        public WebService.Ocorrencia.Ocorrencia Ocorrencia { get; set; }
    }
}
