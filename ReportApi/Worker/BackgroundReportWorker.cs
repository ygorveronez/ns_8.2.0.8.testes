using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReportApi.options;

namespace ReportApi.Worker;

public abstract class BackgroundReportWorker : BackgroundService
{
    #region Atributos

    protected readonly DatabaseOptions _option;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio _configuracaoRelatorio;

    #endregion Atributos

    #region Propriedades

    protected AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
    {
        get { return ConnectionFactory.TipoServicoMultisoftware; }
    }

    protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
    {
        get { return ConnectionFactory.Cliente; }
    }

    #endregion Propriedades

    #region Construtores

    protected BackgroundReportWorker(IOptions<DatabaseOptions> option, IServiceProvider serviceProvider, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio)
    {
        _option = option.Value;
        _serviceProvider = serviceProvider;
        _configuracaoRelatorio = configuracaoRelatorio;

        CultureConfig.RegisterCulture();
        ConnectionFactory.ConfigureFileStorage();
    }

    #endregion Construtores
}
