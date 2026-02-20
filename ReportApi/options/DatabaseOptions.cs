namespace ReportApi.options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public int QuantidadeRelatoriosParalelo { get; set; }
    public DatabaseOptions(string connectionString, int quantidadeRelatorioParalelo)
    {
        ConnectionString = connectionString;
        QuantidadeRelatoriosParalelo = quantidadeRelatorioParalelo;
    }
}


