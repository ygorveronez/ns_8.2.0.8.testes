using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga
{
    public class DocumentoImpressaoCarga
    {

        public DateTime Emissao { get; set; }
        public DateTime Criacao { get; set; }
        public string Operador { get; set; }
        public string Filial { get; set; }
        public string TipoDeOperacao { get; set; }
        public string PlacaCaminhaoCavalo { get; set; }
        public string Frota { get; set; }
        public string Empresa { get; set; }
        public string ModeloVeicular { get; set; }
        public string Motorista { get; set; }
        public string CPF { get; set; }
        public string CargaTMS { get; set; }
        public DateTime DataPrevisaoSaida { get; set; }
        public byte[] CodigoBarrasPlaca { get; set; }

    }
}
