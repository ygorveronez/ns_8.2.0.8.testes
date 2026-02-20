using System;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class RegistrosTrackingDocumentacao
    {
        public Int64 Codigo { get; set; }
        public int TipoMultimodal { get; set; }
        public string Tipo { get; set; }
        public int CodigoVVD { get; set; }
        public string VVD { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public string PortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public string PortoDestino { get; set; }
        public bool CargaPerigosa { get; set; }
        public string IMO { get; set; }
        public string DataGeracao { get; set; }
        public int CodigoOperadorCarga { get; set; }
        public string OperadorCarga { get; set; }
    }
}
