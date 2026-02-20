namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public static class ClasseCorFundo
    {
        public static string Sucess(IntensidadeCor intensidade) => $"bg-success-{intensidade:D}";
        public static string Info(IntensidadeCor intensidade) => $"bg-info-{intensidade:D}";
        public static string Warning(IntensidadeCor intensidade) => $"bg-warning-{intensidade:D}";
        public static string Danger(IntensidadeCor intensidade) => $"bg-danger-{intensidade:D}";
        public static string Primary(IntensidadeCor intensidade) => $"bg-primary-{intensidade:D}";
        public static string Fusion(IntensidadeCor intensidade) => $"bg-fusion-{intensidade:D}";
        public static string Secondary(IntensidadeCor intensidade) => $"bg-secondary-{intensidade:D}";
    }

    public static class ClasseCorTexto
    {
        public static string Sucess(IntensidadeCor intensidade) => $"color-success-{intensidade:D}";
        public static string Info(IntensidadeCor intensidade) => $"color-info-{intensidade:D}";
        public static string Warning(IntensidadeCor intensidade) => $"color-warning-{intensidade:D}";
        public static string Danger(IntensidadeCor intensidade) => $"color-danger-{intensidade:D}";
        public static string Primary(IntensidadeCor intensidade) => $"color-primary-{intensidade:D}";
        public static string Fusion(IntensidadeCor intensidade) => $"color-fusion-{intensidade:D}";
        public static string Secondary(IntensidadeCor intensidade) => $"color-secondary-{intensidade:D}";
    }
    public static class ClasseCorCustomizada
    {
        public static string Fundo(string hexCode) => $"background-color: {hexCode};";
        public static string Texto(string hexCode) => $"color: {hexCode};";
    }

    /// <summary>
    /// Intensidade de cor: quanto maior, mais escuro.
    /// </summary>
    public enum IntensidadeCor
    {
        _50 = 50,
        _100 = 100,
        _200 = 200,
        _300 = 300,
        _400 = 400,
        _500 = 500,
        _600 = 600,
        _700 = 700,
        _800 = 800,
        _900 = 900
    }
}
