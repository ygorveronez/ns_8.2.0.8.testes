using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaBoletoGeracao
    {
        public DateTime DataVencimentoInicial { get; set; }
     
        public DateTime DataVencimentoFinal { get; set; }
        
        public DateTime DataEmissaoInicial { get; set; }
        
        public DateTime DataEmissaoFinal { get; set; }
        
        public double CnpjPessoa { get; set; }
        
        public bool SomentePendentes { get; set; }
        
        public bool SomenteSemRemessa { get; set; }
        
        public int CodigoRemessa { get; set; }
        
        public int CodigoConhecimento { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public FormaTitulo FormaTitulo { get; set; }
        
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroOS { get; set; }
        
        public string NumeroCarga { get; set; }

        public int NumeroNota { get; set; }

        public string NumeroControleCliente { get; set; }

        public string NumeroControle { get; set; }

        public TipoPropostaMultimodal TipoProposta { get; set; }

        public int CodigoTerminalOrigem { get; set; }

        public int CodigoTerminalDestino { get; set; }

        public int CodigoViagem { get; set; }

        public int CodigoFatura { get; set; }

        public int CodigoOperadorFatura { get; set; }

        public List<TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }

        public int CodigoConfiguracaoBoleto { get; set; }
    }
}
