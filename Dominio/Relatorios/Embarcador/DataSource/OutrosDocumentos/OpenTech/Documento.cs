using System;

namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech
{
    public class Documento
    {
        public string NumeroDocumento { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Bairro { get; set; }

        public string CEP { get; set; }

        public string Cidade { get; set; }

        public string Telefone { get; set; }

        public DateTime DataPrevistaEntrega { get; set; }

        public decimal Valor { get; set; }

        public string Situacao { get; set; }
        public int Codigo { get; set; }
    }
}
