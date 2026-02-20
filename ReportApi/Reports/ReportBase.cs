using System;
using System.Collections.Generic;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Interfaces;
using ReportApi.Storage;

namespace ReportApi.Reports;

public abstract class ReportBase : IReport
{
    protected readonly Repositorio.UnitOfWork _unitOfWork;
    protected readonly Servicos.Embarcador.Relatorios.RelatorioReportService _servicoRelatorioReportService;
    protected readonly IStorage _storage;
    private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
    
    public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador
    {
        get
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return this._configuracaoEmbarcador;
        }
    }
    public ReportBase(Repositorio.UnitOfWork unitOfWork, Servicos.Embarcador.Relatorios.RelatorioReportService servicoRelatorioReportService, IStorage storage)
    {
    
        _storage = storage;
        _unitOfWork = unitOfWork;
        _servicoRelatorioReportService = servicoRelatorioReportService;
    }

    public ReportResult Process(Dictionary<string, string> extraData)
    {
        try
        {
            return InternalProcess(extraData);
        }
        catch (Exception e)
        {
            Servicos.Log.TratarErro(e);
            return new ReportResult
            {
                Id = Guid.NewGuid(),
                ErrorMessage = e.Message,
                ExecutionStatus = ExecutionStatus.Fail,
                FileName = String.Empty,
                FullPath = String.Empty
            };
        }
    }


    protected string GetFullPath(string fileId, FileType fileType)
    {
        Guid.TryParse(fileId, out Guid id);
        return GetFullPath(id, fileType);
    }

    protected string GetFullPath(Guid fileId, FileType fileType)
    {
        var filename = $"{fileId}{fileType.GetExtension()}";
        return Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoRelatoriosEmbarcador"], filename);
    }

    private string GetFileName(Guid id, FileType fileType)
    {
        var filename = $"{id}{fileType.GetExtension()}";
        return filename;
    }

    protected ReportResult PrepareReportResult(FileType fileType, Guid? id = null)
    {
        id ??= Guid.NewGuid();
        var filename = GetFileName(id.Value, fileType);
        var fullPath = GetFullPath(id.Value, fileType);
        return new ReportResult
        {
            Id = Guid.NewGuid(),
            ErrorMessage = string.Empty,
            ExecutionStatus = ExecutionStatus.Finished,
            FileName = filename,
            FullPath = fullPath
        };
    }
    
    protected ReportResult PrepareReportResult(FileType fileType, byte[] content)
    {
        var result = PrepareReportResult(fileType);
        _storage.SaveFile(result.FullPath, content);
        return result;
    }
    
    protected ReportResult PrepareReportResult(FileType fileType, string guid)
    {
        Guid.TryParse(guid, out Guid id);
        var result = PrepareReportResult(fileType, id);
        
        return result;
    }

    public abstract ReportResult InternalProcess(Dictionary<string, string> extraData);

    public Dominio.Entidades.Usuario BuscarUsuario(int id)
    {
        var usuariorepo = new Repositorio.Usuario(_unitOfWork);
        return usuariorepo.BuscarPorCodigo(id);
    }

    public Dominio.Entidades.Empresa BuscarEmpresa(int id)
    {
        var empresaRepository = new Repositorio.Empresa(_unitOfWork);
        return empresaRepository.BuscarPorCodigo(id);
    }
}