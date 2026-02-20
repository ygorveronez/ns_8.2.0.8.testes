using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaResponsavel
    {
        public int CodigoCategoriaResponsavel { get; set; }

        public int CodigoFuncionario { get; set; }

        public DateTime? DataVigenciaFinal { get; set; }

        public DateTime? DataVigenciaInicial { get; set; }
    }
}
