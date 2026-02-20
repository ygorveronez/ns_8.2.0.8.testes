namespace Dominio.ObjetosDeValor.Embarcador.Consulta
{
    public sealed class ParametroConsulta
    {
        public string DirecaoAgrupar { get; set; }

        public string DirecaoOrdenar { get; set; }

        public int InicioRegistros { get; set; }

        public int LimiteRegistros { get; set; }

        public string PropriedadeAgrupar { get; set; }

        public string PropriedadeOrdenar { get; set; }

    }
}
