using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public sealed class ControleValePallet
    {
        public string Cidade { get; set; }
        public string Cliente { get; set; }
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Filial { get; set; }
        public string FilialCnpj { get; set; }
        public string FilialCodigoIntegracao { get; set; }
        public string Motorista { get; set; }
        public int Numero { get; set; }
        public int NumeroNfe { get; set; }
        public int Quantidade { get; set; }
        public string Representante { get; set; }
        public string Situacao { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
    }
}
