using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao;

public class IntegracaoEnvioProgramado : ServicoBase
{
    #region Propriedades

    private readonly Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado _repositorioIntegracaoEnvioProgramado;

    #endregion

    #region Construtores

    public IntegracaoEnvioProgramado(UnitOfWork unitOfWork) : base(unitOfWork)
    {
        _repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
    }

    #endregion

    #region Metodos Privados

    private bool AdicionarParaIntegracao(AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
    {
        if (!UtilizaIntegracaoProgramada(adicionarIntegracaoEnvioProgramado, tipoIntegracao.Tipo))
            return false;

        if (VerificarSePossuiIntegracao(adicionarIntegracaoEnvioProgramado, tipoIntegracao.Tipo))
            return true;

        bool bloquearEnvio = VerificarSeGeraIntegracaoBloqueada(tipoIntegracao.Tipo);

        DateTime dataProgramada = ObterDataProgramada(adicionarIntegracaoEnvioProgramado, tipoIntegracao.Tipo, bloquearEnvio);
        if (dataProgramada <= new DateTime(1970, 1, 1))
            dataProgramada = DateTime.Now.AddHours(24);

        Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramada = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado(dataProgramada);

        integracaoEnvioProgramada.TipoEntidadeIntegracao = adicionarIntegracaoEnvioProgramado.TipoEntidadeIntegracao;
        integracaoEnvioProgramada.Carga = adicionarIntegracaoEnvioProgramado.Carga;
        integracaoEnvioProgramada.CargaOcorrencia = adicionarIntegracaoEnvioProgramado.CargaOcorrencia;
        integracaoEnvioProgramada.CTe = adicionarIntegracaoEnvioProgramado.CTe;
        integracaoEnvioProgramada.NumeroTentativas = 0;
        integracaoEnvioProgramada.ProblemaIntegracao = "";
        integracaoEnvioProgramada.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
        integracaoEnvioProgramada.TipoIntegracao = tipoIntegracao;
        integracaoEnvioProgramada.EnvioAntecipado = false;
        integracaoEnvioProgramada.EnvioBloqueado = bloquearEnvio;
        integracaoEnvioProgramada.TipoDocumento = adicionarIntegracaoEnvioProgramado.CTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.Nenhum;

        _repositorioIntegracaoEnvioProgramado.Inserir(integracaoEnvioProgramada);

        return true;
    }

    private DateTime ObterDataProgramada(AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado, TipoIntegracao enumTipoIntegracao, bool bloquearEnvio)
    {
        if (bloquearEnvio)
            return DateTime.Now;

        return adicionarIntegracaoEnvioProgramado.TipoEntidadeIntegracao switch
        {
            TipoEntidadeIntegracao.Carga => ObterDataProgramadaCarga(adicionarIntegracaoEnvioProgramado.Carga, enumTipoIntegracao),
            TipoEntidadeIntegracao.CargaOcorrencia => ObterDataProgramadaCargaOcorrencia(adicionarIntegracaoEnvioProgramado.CargaOcorrencia, enumTipoIntegracao),
            TipoEntidadeIntegracao.CTe => ObterDataProgramadaCTe(adicionarIntegracaoEnvioProgramado.CTe, enumTipoIntegracao),
            _ => DateTime.Now.AddHours(24),
        };
    }

    private DateTime ObterDataProgramadaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoIntegracao enumTipoIntegracao)
    {
        DateTime dataPadrao = DateTime.Now.AddHours(24);

        if (enumTipoIntegracao == TipoIntegracao.Carrefour)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            DateTime DataEmissaoUltimoCte = repositorioCargaCte.BuscarDataUltimaEmissaoPorCarga(carga.Codigo, "A");

            return DataEmissaoUltimoCte.AddHours(24);
        }

        if (enumTipoIntegracao == TipoIntegracao.GrupoSC)
        {
            return DateTime.Now.AddMinutes(5);
        }

        if (enumTipoIntegracao == TipoIntegracao.Mars)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            return DateTime.Now.AddMinutes(configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? 5);
        }

        if (enumTipoIntegracao == TipoIntegracao.ObramaxCTE || enumTipoIntegracao == TipoIntegracao.ObramaxNFE)
            return DateTime.Now.AddHours(24);

        return dataPadrao;
    }

    private DateTime ObterDataProgramadaCargaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, TipoIntegracao enumTipoIntegracao)
    {
        DateTime dataPadrao = DateTime.Now.AddHours(24);

        if (enumTipoIntegracao == TipoIntegracao.Mars)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            return DateTime.Now.AddMinutes(configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? 5);
        }

        return dataPadrao;
    }

    private DateTime ObterDataProgramadaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe, TipoIntegracao enumTipoIntegracao)
    {
        DateTime dataPadrao = DateTime.Now.AddHours(24);

        if (enumTipoIntegracao == TipoIntegracao.GrupoSC)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            double minutosACadaTentativa = configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? 5;

            return CTe.DataAutorizacao.Value.AddMinutes(minutosACadaTentativa);
        }

        if (enumTipoIntegracao == TipoIntegracao.Carrefour)
        {
            double minutosACadaTentativa = 10080.00;
            return CTe.DataAutorizacao.Value.AddMinutes(minutosACadaTentativa);
        }

        if (enumTipoIntegracao == TipoIntegracao.Mars)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            return DateTime.Now.AddMinutes(configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? 5);
        }

        return dataPadrao;
    }

    private bool UtilizaIntegracaoProgramada(AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado, TipoIntegracao enumTipoIntegracao)
    {
        return adicionarIntegracaoEnvioProgramado.TipoEntidadeIntegracao switch
        {
            TipoEntidadeIntegracao.Carga => UtilizaIntegracaoProgramadaCarga(adicionarIntegracaoEnvioProgramado.Carga, enumTipoIntegracao),
            TipoEntidadeIntegracao.CargaOcorrencia => UtilizaIntegracaoProgramadaOcorrencia(adicionarIntegracaoEnvioProgramado.CargaOcorrencia, enumTipoIntegracao),
            TipoEntidadeIntegracao.CTe => UtilizaIntegracaoProgramadaCTe(enumTipoIntegracao),
            _ => false,
        };
    }

    private bool UtilizaIntegracaoProgramadaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoIntegracao enumTipoIntegracao)
    {
        List<TipoIntegracao> integracoes = new List<TipoIntegracao>()
        {
            TipoIntegracao.Carrefour,
            TipoIntegracao.ObramaxCTE,
            TipoIntegracao.ObramaxNFE,
            TipoIntegracao.WeberChile
        };

        if (integracoes.Contains(enumTipoIntegracao))
            return carga.TipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false;

        return false;
    }

    private bool VerificarSeGeraIntegracaoBloqueada(TipoIntegracao enumTipoIntegracao)
    {
        List<TipoIntegracao> integracoes = new List<TipoIntegracao>()
        {
            TipoIntegracao.WeberChile
        };

        if (integracoes.Contains(enumTipoIntegracao))
            return true;

        return false;
    }

    private bool UtilizaIntegracaoProgramadaCTe(TipoIntegracao enumTipoIntegracao)
    {
        List<TipoIntegracao> integracoes = new List<TipoIntegracao>()
        {
            TipoIntegracao.GrupoSC,
            TipoIntegracao.Mars,
            TipoIntegracao.Carrefour
        };

        if (integracoes.Contains(enumTipoIntegracao))
            return true;

        return false;
    }

    private bool UtilizaIntegracaoProgramadaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, TipoIntegracao enumTipoIntegracao)
    {
        List<TipoIntegracao> integracoes = new List<TipoIntegracao>()
        {
            //Vazia Temporariamente
        };

        if (integracoes.Contains(enumTipoIntegracao))
            return cargaOcorrencia.Carga.TipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false;

        return false;
    }

    private bool VerificarSePossuiIntegracao(AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado, TipoIntegracao enumTipoIntegracao)
    {
        return adicionarIntegracaoEnvioProgramado.TipoEntidadeIntegracao switch
        {
            TipoEntidadeIntegracao.Carga => _repositorioIntegracaoEnvioProgramado.ExistePorCarga(adicionarIntegracaoEnvioProgramado.Carga.Codigo, enumTipoIntegracao),
            TipoEntidadeIntegracao.CargaOcorrencia => _repositorioIntegracaoEnvioProgramado.ExistePorCargaOcorrencia(adicionarIntegracaoEnvioProgramado.CargaOcorrencia.Codigo, enumTipoIntegracao),
            TipoEntidadeIntegracao.CTe => _repositorioIntegracaoEnvioProgramado.ExistePorCTe(adicionarIntegracaoEnvioProgramado.CTe.Codigo, enumTipoIntegracao),
            _ => false
        };
    }

    #endregion

    #region Metodos Publicos

    public bool AdicionarCargaParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
    {
        AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado = new AdicionarIntegracaoEnvioProgramado(TipoEntidadeIntegracao.Carga)
        {
            Carga = carga
        };

        return AdicionarParaIntegracao(adicionarIntegracaoEnvioProgramado, tipoIntegracao);
    }

    public bool AdicionarOcorrenciaParaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
    {
        AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado = new AdicionarIntegracaoEnvioProgramado(TipoEntidadeIntegracao.CargaOcorrencia)
        {
            CargaOcorrencia = cargaOcorrencia
        };

        return AdicionarParaIntegracao(adicionarIntegracaoEnvioProgramado, tipoIntegracao);
    }

    public bool AdicionarCTeParaIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null)
    {
        AdicionarIntegracaoEnvioProgramado adicionarIntegracaoEnvioProgramado = new AdicionarIntegracaoEnvioProgramado(TipoEntidadeIntegracao.CTe)
        {
            CTe = cte,
            Carga = carga,
            CargaOcorrencia = cargaOcorrencia
        };

        return AdicionarParaIntegracao(adicionarIntegracaoEnvioProgramado, tipoIntegracao);
    }

    public void AdicionarCTesLoteParaIntegracao(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null)
    {
        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
        {
            AdicionarCTeParaIntegracao(cte, tipoIntegracao, carga, cargaOcorrencia);
        }
    }

    public List<int> VerificarIntegracaoesCargaPendentes()
    {
        int numeroTentativas = 2;
        double minutosACadaTentativa = 5;
        int numeroRegistrosPorVez = 15;

        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado = _repositorioIntegracaoEnvioProgramado.BuscarIntegracoesPendentes(TipoEntidadeIntegracao.Carga, numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez);

        List<int> codigosCargas = integracoesEnvioProgramado.Select(x => x.Carga.Codigo).Distinct().ToList();

        foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoPendente in integracoesEnvioProgramado)
        {
            switch (integracaoPendente.TipoIntegracao.Tipo)
            {
                case TipoIntegracao.Carrefour:
                    new Servicos.Embarcador.Integracao.Carrefour.IntegracaoCarrefour(_unitOfWork).IntegrarCarga(integracaoPendente, integracaoPendente.Carga, aguardaRecebimento: false);
                    break;
                case TipoIntegracao.ObramaxCTE:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(_unitOfWork, string.Empty).IntegrarCTeCargaEnvioProgramado(integracaoPendente);
                    break;
                case TipoIntegracao.ObramaxNFE:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(_unitOfWork, string.Empty).IntegrarNFSeCargaEnvioProgramado(integracaoPendente);
                    break;
                case TipoIntegracao.WeberChile:
                    new Servicos.Embarcador.Integracao.WeberChile.IntegracaoWeberChile(_unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                default:
                    integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar por envio programado";
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.NumeroTentativas++;
                    integracaoPendente.DataIntegracao = DateTime.Now;
                    _repositorioIntegracaoEnvioProgramado.Atualizar(integracaoPendente);
                    break;
            }

        }

        return codigosCargas;
    }

    public void VerificarIntegracaoesCTePendentes()
    {
        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

        bool utilizarQuantidadeMinima = repTipoIntegracao.ExistePorTipo(TipoIntegracao.GrupoSC);

        int numeroTentativas = 2;
        double minutosPadraoCadaTentativa = 5;
        double minutosACadaTentativa = utilizarQuantidadeMinima ? minutosPadraoCadaTentativa : (configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? minutosPadraoCadaTentativa);
        int numeroRegistrosPorVez = 100;

        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado = _repositorioIntegracaoEnvioProgramado.BuscarIntegracoesPendentesPorTipoDocumento(TipoEntidadeIntegracao.CTe, numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez, Dominio.Enumeradores.TipoDocumento.CTe);

        if (integracoesEnvioProgramado?.Count == 0)
            return;

        var gruposIntegracao = integracoesEnvioProgramado.GroupBy(i => i.TipoIntegracao).ToList();

        foreach (var grupo in gruposIntegracao)
        {
            switch (integracoesEnvioProgramado.Select(x => x.TipoIntegracao.Tipo).FirstOrDefault())
            {
                case TipoIntegracao.GrupoSC:
                    new Servicos.Embarcador.Integracao.GrupoSC.IntegracaoGrupoSC(_unitOfWork).IntegrarCTes(grupo.ToList());
                    break;
                case TipoIntegracao.Mars:
                    new Servicos.Embarcador.Integracao.Mars.IntegracaoMars(_unitOfWork).IntegrarCTes(grupo.ToList());
                    break;
                default:
                    foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoPendente in grupo.ToList())
                    {
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar por envio programado";
                        integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        _repositorioIntegracaoEnvioProgramado.Atualizar(integracaoPendente);
                    }
                    break;
            }
        }
    }

    public void VerificarIntegracaoesNFSePendentes()
    {
        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

        bool utilizarQuantidadeMinima = repTipoIntegracao.ExistePorTipo(TipoIntegracao.GrupoSC);

        int numeroTentativas = 2;
        double minutosPadraoCadaTentativa = 5;
        double minutosACadaTentativa = utilizarQuantidadeMinima ? minutosPadraoCadaTentativa : (configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? minutosPadraoCadaTentativa);
        int numeroRegistrosPorVez = 100;

        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado = _repositorioIntegracaoEnvioProgramado.BuscarIntegracoesPendentesPorTipoDocumento(TipoEntidadeIntegracao.CTe, numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez, Dominio.Enumeradores.TipoDocumento.NFSe);

        if (integracoesEnvioProgramado?.Count == 0)
            return;

        var gruposIntegracao = integracoesEnvioProgramado.GroupBy(i => i.TipoIntegracao).ToList();

        foreach (var grupo in gruposIntegracao)
        {
            switch (integracoesEnvioProgramado.Select(x => x.TipoIntegracao.Tipo).FirstOrDefault())
            {
                case TipoIntegracao.GrupoSC:
                    new Servicos.Embarcador.Integracao.GrupoSC.IntegracaoGrupoSC(_unitOfWork).IntegrarNFSes(grupo.ToList());
                    break;

                default:
                    foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoPendente in grupo.ToList())
                    {
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar por envio programado";
                        integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        _repositorioIntegracaoEnvioProgramado.Atualizar(integracaoPendente);
                    }
                    break;
            }
        }
    }

    public void VerificarIntegracaoesOutrosDocumentosPendentes()
    {
        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

        bool utilizarQuantidadeMinima = repTipoIntegracao.ExistePorTipo(TipoIntegracao.GrupoSC);

        int numeroTentativas = 2;
        double minutosPadraoCadaTentativa = 5;
        double minutosACadaTentativa = utilizarQuantidadeMinima ? minutosPadraoCadaTentativa : (configuracaoCarga.TempoMinutosParaEnvioProgramadoIntegracao ?? minutosPadraoCadaTentativa);
        int numeroRegistrosPorVez = 100;

        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado = _repositorioIntegracaoEnvioProgramado.BuscarIntegracoesPendentesPorTipoDocumento(TipoEntidadeIntegracao.CTe, numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez, Dominio.Enumeradores.TipoDocumento.Outros);

        if (integracoesEnvioProgramado?.Count == 0)
            return;

        List<IGrouping<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao, Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>> gruposIntegracao =
            integracoesEnvioProgramado.GroupBy(i => i.TipoIntegracao).ToList();

        foreach (IGrouping<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao, Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> grupo in gruposIntegracao)
        {
            switch (integracoesEnvioProgramado.Select(x => x.TipoIntegracao.Tipo).FirstOrDefault())
            {
                case TipoIntegracao.GrupoSC:
                    new Servicos.Embarcador.Integracao.GrupoSC.IntegracaoGrupoSC(_unitOfWork).IntegrarOutrosDocumentos(grupo.ToList());
                    break;

                default:
                    foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoPendente in grupo.ToList())
                    {
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar por envio programado";
                        integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        _repositorioIntegracaoEnvioProgramado.Atualizar(integracaoPendente);
                    }
                    break;
            }
        }
    }

    public void VerificarIntegracaoesOcorrenciaPendentes()
    {
        int numeroTentativas = 2;
        double minutosACadaTentativa = 5;
        int numeroRegistrosPorVez = 10;

        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado = _repositorioIntegracaoEnvioProgramado.BuscarIntegracoesPendentes(TipoEntidadeIntegracao.CargaOcorrencia, numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez);

        foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoPendente in integracoesEnvioProgramado)
        {
            switch (integracaoPendente.TipoIntegracao.Tipo)
            {
                default:
                    integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar por envio programado";
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.NumeroTentativas++;
                    integracaoPendente.DataIntegracao = DateTime.Now;
                    _repositorioIntegracaoEnvioProgramado.Atualizar(integracaoPendente);
                    break;
            }
        }
    }

    public void AdicionarIntegracaoProgramadaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
    {
        Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

        Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado serIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);

        serIntegracaoEnvioProgramado.AdicionarCTesLoteParaIntegracao(repositorioCargaCTeComplementoInfo.BuscarCTePorOcorrencia(cargaOcorrencia.Codigo), tipoIntegracao, cargaOcorrencia.Carga, cargaOcorrencia);
    }

    #endregion
}
