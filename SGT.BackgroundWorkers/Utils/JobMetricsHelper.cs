using StatsdClient;
using System;

public static class JobMetricsHelper
{
    private static bool _isInitialized = false;
    private static readonly object _lock = new object();

    /// <summary>
    /// Configura o cliente StatsD. Deve ser chamado uma única vez no Program.cs.
    /// </summary>
    public static void Initialize(string host, int? port, string prefix)
    {
        // Padrão de trava para garantir que a inicialização aconteça apenas uma vez de forma segura.
        lock (_lock)
        {
            if (_isInitialized) return;

            try
            {
                var statsDConfig = new StatsdConfig
                {
                    // Lê as configurações do appsettings.json
                    StatsdServerName = host ?? "localhost",
                    StatsdPort = port.GetValueOrDefault(8125),
                    Prefix = prefix ?? "sgt.webadmin"
                };

                DogStatsd.Configure(statsDConfig);
                _isInitialized = true;

                // Log ou Console.WriteLine para confirmar que o helper foi inicializado.
                Servicos.Log.GravarInfo($"{nameof(JobMetricsHelper)} inicializado com sucesso.");
            }
            catch (Exception ex)
            {
                // Em caso de erro na inicialização, logue o problema mas não quebre a aplicação.
                Servicos.Log.TratarErro($"Erro ao inicializar o {nameof(JobMetricsHelper)}: {ex}");                
            }
        }
    }

    /// <summary>
    /// Registra um conjunto completo de métricas para uma execução de job.
    /// </summary>
    public static void RecordJobExecutionMetrics(string jobName, string tenantId, TimeSpan duration, double cpuTimeMs, long finalMemoryMb, bool wasSuccessful)
    {
        // Se o cliente não foi inicializado, não faz nada.
        if (!_isInitialized) return;

        try
        {
            var tags = new[] 
            {
                $"job_name:{jobName}",
                $"tenant_id:{tenantId}",
                $"status:{(wasSuccessful ? "success" : "failure")}"
            };

            // Quantidade de execuções (separado por sucesso e falha)
            DogStatsd.Increment("job.execution.count", tags: tags);

            // Tempo de execução
            DogStatsd.Histogram("job.duration.ms", duration.TotalMilliseconds, tags: tags);

            // Consumo de CPU (Delta)
            DogStatsd.Gauge("job.cpu_time.ms", cpuTimeMs, tags: tags);

            // Consumo de Memória (Final)
            DogStatsd.Gauge("job.memory.mb", finalMemoryMb, tags: tags);
        }
        catch (Exception ex)
        {
            // Loga qualquer erro inesperado no envio de métricas sem quebrar o job.
            Servicos.Log.TratarErro($"Erro ao enviar métricas via StatsD para o job {jobName}: {ex}");
        }
    }

    /// <summary>
    /// Sanitiza uma string para ser usada como um valor de tag "tenant_id" no StatsD, 
    /// seguindo as convenções do DogStatsD.
    /// </summary>
    /// <param name="rawValue">O valor bruto da tag (ex: nome fantasia do cliente).</param>
    /// <returns>Uma string segura para ser usada como valor de tag.</returns>
    public static string SanitizeTenantId(string rawValue)
    {
        // 1. Lida com entradas nulas ou vazias, retornando um valor padrão.
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return "unknown";
        }

        // 2. Converte para minúsculas.
        var sanitized = rawValue.ToLowerInvariant();

        // 3. Remove acentos, transformando "Comércio de Maçãs" em "comercio de macas".
        //    Isso é feito normalizando a string para separar os caracteres base dos acentos
        //    e depois removendo os acentos.
        var normalizedString = sanitized.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder(normalizedString.Length);
        foreach (var c in normalizedString)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        sanitized = sb.ToString().Normalize(System.Text.NormalizationForm.FormC);

        // 4. Substitui os separadores mais comuns por underscore.
        sanitized = sanitized.Replace(' ', '_');
        sanitized = sanitized.Replace('.', '_');
        sanitized = sanitized.Replace('-', '_');

        // 5. Remove todos os caracteres que não são letras, números ou underscore.
        var allowedChars = new System.Text.StringBuilder(sanitized.Length);
        foreach (var c in sanitized)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                allowedChars.Append(c);
            }
        }
        sanitized = allowedChars.ToString();

        // 6. Garante que não haja underscores duplicados.
        while (sanitized.Contains("__"))
        {
            sanitized = sanitized.Replace("__", "_");
        }

        // 7. Remove underscores no início ou no fim.
        sanitized = sanitized.Trim('_');

        // 8. Garante que a tag comece com uma letra (requisito do DogStatsD).
        if (sanitized.Length > 0 && !char.IsLetter(sanitized[0]))
        {
            sanitized = "t_" + sanitized; // Adiciona um prefixo "t" (de tenant ou tag).
        }

        // 9. Trunca a tag para o limite de 200 caracteres do Datadog.
        if (sanitized.Length > 200)
        {
            sanitized = sanitized.Substring(0, 200);
        }

        // 10. Garante que não retornemos uma string vazia após todas as limpezas.
        if (string.IsNullOrEmpty(sanitized))
        {
            return "invalid";
        }

        return sanitized;
    }
}