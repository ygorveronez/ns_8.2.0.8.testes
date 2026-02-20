using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe
{
    public class FiltroConsultaMDFe
    {
        public int CodigoEmpresa { get; set; }
        public string ChaveCTe { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoDestino { get; set; }
        public Dominio.Enumeradores.StatusMDFe? Status { get; set; }
        public double CPFCNPJRemetente { get; set; }
        public string Placa { get; set; }
        public int Serie { get; set; }
        public List <int> CodigosFiliais { get; set; }
        public List <double> CodigosRecebedores { get; set; }
        public int CodigoCarga { get; set; }
    }
}
