using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public sealed class VigenciaAnexo
    {
        public int Codigo { get; set; }

        public DateTime? DataFinalVigencia { get; set; }

        public DateTime DataInicialVigencia { get; set; }

        public string Descricao { get; set; }

        public string NomeArquivo { get; set; }

        public string Vigencia {
            get { return $"De {DataInicialVigencia.ToString("dd/MM/yyyy")}{(DataFinalVigencia.HasValue ? $" até {DataFinalVigencia.Value.ToString("dd/MM/yyyy")}" : "")}";  }
        }
    }
}
