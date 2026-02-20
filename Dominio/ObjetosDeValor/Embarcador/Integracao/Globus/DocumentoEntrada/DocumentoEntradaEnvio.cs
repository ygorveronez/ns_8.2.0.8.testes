using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.DocumentoEntrada
{
    public class DocumentoEntradaEnvio
    {
        public string InscricaoEmpresa { get; set; }
        public int Fase { get; set; }
        public long Documento { get; set; }
        public int Serie { get; set; }
        public string Sistema { get; set; }
        public string Usuario { get; set; }
        public int CodigoModelo { get; set; }
        public string ConteudoXml { get; set; }
        public string ChaveDeAcesso { get; set; }
        public string DataEmissao { get; set; }
    }

}
