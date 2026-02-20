namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    /// <summary>
    /// Vazio: Tipo de Percuso de uma stage.
    /// #51240 Unilever
    /// </summary>
    public enum Vazio
    {
        Todos = 0,
        PercursoPreliminar = 1,
        PercursoPrincipal = 2,
        PercursoSubSeQuente = 3,
        PercursoDireto = 4,
        PercursoRegreso = 5
    }

    public static class VazioHelper
    {
        public static string ObterDescricao(this Vazio vazio)
        {
            switch (vazio)
            {
                case Vazio.PercursoDireto: return "Percurso Direto";
                case Vazio.PercursoPreliminar: return "Percurso Preliminar";
                case Vazio.PercursoPrincipal: return "Percurso Principal";
                case Vazio.PercursoRegreso: return "Percurso Regreso";
                case Vazio.PercursoSubSeQuente: return "Percurso Subsequente";
                default: return string.Empty;
            }
        }
    }
}
