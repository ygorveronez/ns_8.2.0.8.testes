using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.NotaFiscalEletronica
{
    public class NotaFiscalEletronicaEnvio
    {
        public string InscricaoEmpresa { get; set; }
        public string Garagem { get; set; }
        public int TipoDocumento { get; set; }
        public string Serie { get; set; }
        public int Conhecimento { get; set; }
        public int Fase { get; set; }
        public string Sitema { get; set; }
        public string Usuario { get; set; }
        public string ChaveDeAcesso { get; set; }
        public string DataEnvio { get; set; }
        public string ConteudoXML { get; set; }
        public string Protocolo { get; set; }
        public string DataProtocolo { get; set; }
        public string Status { get; set; }
        public string Recibo { get; set; }
        public string MensagemRecibo { get; set; }
        public string DataEmissao { get; set; }
        public string Versao { get; set; }
        public string Situacao { get; set; }
    }

}
