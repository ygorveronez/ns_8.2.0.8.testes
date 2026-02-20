using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PortalCliente
{
    public class PortalClienteView
    {

        public string CaminhoLogo { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string CNPJ { get; set; }

        public string DestinatarioNome { get; set; }

        public string ChaveNFe { get; set; }
        
        public int CodigoNotaFiscal { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public string DataEmissao { get; set; }

        public decimal ValorNota { get; set; }

        public ICollection<ParcelaView> Parcelas { get; set; }

    }
}
