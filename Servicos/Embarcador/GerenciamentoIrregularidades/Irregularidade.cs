using Dominio.Entidades.Embarcador.Documentos;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GerenciamentoIrregularidades
{
    public sealed class Irregularidade
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Irregularidade(Repositorio.UnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        #endregion Construtores

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> ObterHistoricoIrregularidade(List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicos, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade)
        {
            return historicos.Where(x => x.Irregularidade != null && x.Irregularidade.Codigo == irregularidade.Codigo && x.Irregularidade.GatilhoIrregularidade == irregularidade.GatilhoIrregularidade).ToList();
        }

        private int BuscarCodigoGrupoTipoOperacaoCarga(Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento)
        {
            return documento?.Carga?.TipoOperacao?.GrupoTipoOperacao?.Codigo ?? 0;
        }

        #endregion

        #region Métodos Públicos

        public void GerarHistoricoTratativaIrregularidade(MovimentarIrregularidade movimentarIrregularidade)
        {
            Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = movimentarIrregularidade.ControleDocumento;
            Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade();

            historicoIrregularidade.ControleDocumento = documento;
            historicoIrregularidade.DataIrregularidade = DateTime.Now;
            historicoIrregularidade.Irregularidade = movimentarIrregularidade.Irregularidade != null ? movimentarIrregularidade.Irregularidade : documento?.MotivoIrregularidade?.Irregularidade ?? null;
            historicoIrregularidade.MotivoIrregularidade = documento.MotivoIrregularidade;
            historicoIrregularidade.Observacao = string.IsNullOrEmpty(movimentarIrregularidade.Observacao) ? string.Empty : Utilidades.String.Left(movimentarIrregularidade.Observacao, 500);
            historicoIrregularidade.Porfolio = movimentarIrregularidade.Irregularidade != null ? movimentarIrregularidade.Irregularidade.PortfolioModuloControle : documento?.MotivoIrregularidade?.Irregularidade?.PortfolioModuloControle ?? null;
            historicoIrregularidade.SequenciaTrataviva = movimentarIrregularidade.Responsavel == ServicoResponsavel.Transporador ? 0 : movimentarIrregularidade.Sequencia;
            historicoIrregularidade.ServicoResponsavel = movimentarIrregularidade.Responsavel;
            historicoIrregularidade.Setor = movimentarIrregularidade.Responsavel == ServicoResponsavel.Transporador ? null : documento.Setor;
            historicoIrregularidade.SituacaoIrregularidade = SituacaoIrregularidade.AguardandoAprovacao;

            new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork).Inserir(historicoIrregularidade);

            return;
        }

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade BuscarProximaTratativaIrregularidadeEmbarcador(Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento, TipoSetorFuncionario? tiposetor, MovimentarIrregularidade movimentarIrregularidade, bool manterSetor = false)
        {
            Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, ServicoResponsavel.Embarcador);
            Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repTratativaIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(_unitOfWork);
            int sequencia = historicoIrregularidade == null ? 0 : historicoIrregularidade.SequenciaTrataviva;
            int codigoIrregularidade = historicoIrregularidade == null ? movimentarIrregularidade?.Irregularidade?.Codigo ?? 0 : historicoIrregularidade.Irregularidade?.Codigo ?? 0;
            int codigoGrupoTipoOperacao = BuscarCodigoGrupoTipoOperacaoCarga(documento);

            if (historicoIrregularidade == null)
            {
                historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, ServicoResponsavel.Transporador);
                codigoIrregularidade = historicoIrregularidade?.Irregularidade?.Codigo ?? (movimentarIrregularidade?.Irregularidade?.Codigo ?? 0);
            }

            Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade tratativaIrregularidade = repTratativaIrregularidade.BuscarPorDefinicaoTratativasIrregularidade(codigoIrregularidade, sequencia, codigoGrupoTipoOperacao, tiposetor);

            if (tratativaIrregularidade == null)
                throw new ServicoException("Falha ao buscar a Tratativa atual do Documento", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoBuscarTratativaIrregularidadeAtualDocumento);

            if (manterSetor)
                return tratativaIrregularidade;

            if (historicoIrregularidade == null || tratativaIrregularidade.ProximaSequencia == 0)
                return tratativaIrregularidade;

            Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade proximaTratativaIrregularidade = repTratativaIrregularidade.BuscarPorDefinicaoTratativasIrregularidade(codigoIrregularidade, tratativaIrregularidade.ProximaSequencia, codigoGrupoTipoOperacao, tiposetor);

            if (proximaTratativaIrregularidade == null)
                return tratativaIrregularidade;

            return proximaTratativaIrregularidade;
        }

        public void MovimentarTratativaDocumento(MovimentarIrregularidade movimentarIrregularidade, bool manterSetor = false, Dominio.Entidades.Setor setor = null)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            movimentarIrregularidade.Sequencia = 0;
            movimentarIrregularidade.ControleDocumento.Initialize();
            movimentarIrregularidade.ControleDocumento.ServicoResponsavel = movimentarIrregularidade.Responsavel;

            if (movimentarIrregularidade.Responsavel != ServicoResponsavel.Embarcador)
            {
                if (movimentarIrregularidade.Irregularidade == null)
                {
                    Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork).BuscarUltimoPorDocumentoResponsavel(movimentarIrregularidade.ControleDocumento.Codigo, ServicoResponsavel.Embarcador);
                    movimentarIrregularidade.Irregularidade = historicoIrregularidade != null ? historicoIrregularidade.Irregularidade : null;
                }

                GerarHistoricoTratativaIrregularidade(movimentarIrregularidade);
                repControleDocumento.Atualizar(movimentarIrregularidade.ControleDocumento);

                return;
            }

                if (setor == null)
                {
                    Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade tratativa = null;
                    if (movimentarIrregularidade.ControleDocumento?.MotivoIrregularidade?.TipoMotivo != TipoMotivoIrregularidade.ProblemaOperacional)
                    {
                        tratativa = BuscarProximaTratativaIrregularidadeEmbarcador(movimentarIrregularidade.ControleDocumento, null, movimentarIrregularidade, manterSetor);
                        if (tratativa != null)
                        {
                            movimentarIrregularidade.ControleDocumento.Setor = tratativa.Setor;
                            movimentarIrregularidade.Sequencia = tratativa.Sequencia;
                            movimentarIrregularidade.Irregularidade = tratativa.DefinicaoTratativasIrregularidade.Irregularidade;
                        }
                    }

                    if (tratativa == null)
                    {
                        if (!string.IsNullOrEmpty(movimentarIrregularidade.ControleDocumento?.CargaCTe?.Carga?.ExternalID1))
                            tratativa = BuscarProximaTratativaIrregularidadeEmbarcador(movimentarIrregularidade.ControleDocumento, TipoSetorFuncionario.Planejamento, movimentarIrregularidade);
                        else
                            tratativa = BuscarProximaTratativaIrregularidadeEmbarcador(movimentarIrregularidade.ControleDocumento, TipoSetorFuncionario.NaoInformado, movimentarIrregularidade);
                    }

                movimentarIrregularidade.ControleDocumento.Setor = tratativa.Setor;
                movimentarIrregularidade.Sequencia = tratativa.Sequencia;
            }
            else
            {
                Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork).BuscarUltimoPorDocumentoResponsavel(movimentarIrregularidade.ControleDocumento.Codigo, ServicoResponsavel.Embarcador);
                movimentarIrregularidade.Irregularidade = historicoIrregularidade.Irregularidade;
                movimentarIrregularidade.Sequencia = historicoIrregularidade.SequenciaTrataviva;
                movimentarIrregularidade.ControleDocumento.Setor = setor;
            }
            repControleDocumento.Atualizar(movimentarIrregularidade.ControleDocumento);
            GerarHistoricoTratativaIrregularidade(movimentarIrregularidade);

            if (_unitOfWork.IsActiveTransaction())
                _unitOfWork.Flush();
        }

        public void ValidarIrregularidadeControleDocumento(Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento)
        {
            Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> irregularidades = repositorioIrregularidade.BuscarIrregularidadesAtivas();

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = controleDocumento.CargaCTe;

            if (cargaCte == null && controleDocumento.CTe != null)
            {
                cargaCte = repositorioCargaCte.BuscarPorCTe(controleDocumento.CTe.Codigo);
                if (cargaCte != null)
                {
                    controleDocumento.CargaCTe = cargaCte;
                    repositorioControleDocumento.Atualizar(controleDocumento);
                }
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = controleDocumento.CTe;
            Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = cargaCte?.PreCTe ?? null;
            List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicoIrregularidadesAtualDocumento = repositorioHistoricoIrregularidade.BuscarPorControleDocumento(controleDocumento.Codigo);

            Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidadeSemLink = irregularidades.Find(x => x.GatilhoIrregularidade == GatilhoIrregularidade.SemLink);

            if (!(cte?.Empresa.EmissaoDocumentosForaDoSistema ?? false) && preCte == null)
            {
                if (controleDocumento.CargaCTe == null)
                {
                    try
                    {
                        _unitOfWork.Start();

                        new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                        {
                            ControleDocumento = controleDocumento,
                            Irregularidade = irregularidadeSemLink,
                            Responsavel = (irregularidadeSemLink?.SeguirAprovacaoTranspPrimeiro ?? false) ? ServicoResponsavel.Transporador : ServicoResponsavel.Embarcador,
                            Observacao = $"Cte Sem Vinculo Com a Carga"
                        });

                        GerarInconsistenciaNoControleDocumento(controleDocumento);
                        _unitOfWork.CommitChanges();
                    }

                    catch (ServicoException ex)
                    {
                        TratarServicoException(controleDocumento, ex);
                    }

                    catch (Exception ex)
                    {
                        TratarException(controleDocumento, repositorioControleDocumento, ex, "ValidarIrregularidadeControleDocumento");
                    }

                    return;
                }

                controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.Liberado;
                controleDocumento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.Verificado;
                repositorioControleDocumento.Atualizar(controleDocumento);
                return;
            }


            bool pararProcessamento = false;
            bool geroSemLink = false;
            foreach (var irregularidade in irregularidades)
            {
                try
                {
                    _unitOfWork.Start();
                    ServicoResponsavel responsavel = irregularidade.SeguirAprovacaoTranspPrimeiro ? ServicoResponsavel.Transporador : ServicoResponsavel.Embarcador;

                    List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> existeHistoricoIrregularidade = ObterHistoricoIrregularidade(historicoIrregularidadesAtualDocumento, irregularidade);

                    Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(controleDocumento.Codigo);

                    if ((irregularidade.GatilhoIrregularidade != GatilhoIrregularidade.SemLink || irregularidade.GatilhoIrregularidade != GatilhoIrregularidade.CTeCancelado) && preCte == null && existeHistoricoIrregularidade.Count == 0 && !geroSemLink)
                    {

                        new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                        {
                            ControleDocumento = controleDocumento,
                            Irregularidade = irregularidadeSemLink,
                            Responsavel = responsavel,
                            Observacao = $"Cte Sem Vinculo Com a Carga"
                        });

                        GerarInconsistenciaNoControleDocumento(controleDocumento);
                        geroSemLink = true;
                        _unitOfWork.CommitChanges();
                        continue;
                    }
                    else if ((irregularidade.GatilhoIrregularidade != GatilhoIrregularidade.SemLink || irregularidade.GatilhoIrregularidade != GatilhoIrregularidade.CTeCancelado) && preCte == null && (existeHistoricoIrregularidade.Count > 0 || (existeHistoricoIrregularidade.Count == 0 && geroSemLink)))
                    {
                        GerarInconsistenciaNoControleDocumento(controleDocumento);
                        continue;
                    }

                    if (pararProcessamento)
                        break;


                    switch (irregularidade.GatilhoIrregularidade)
                    {

                        case GatilhoIrregularidade.AliquotaICMSValorICMS:

                            if (existeHistoricoIrregularidade.Count == 0 && (((cte?.AliquotaICMS) != (preCte?.AliquotaICMS)) || ((cte?.BaseCalculoICMS) != (preCte?.BaseCalculoICMS)) || ((cte?.ValorICMS) != (preCte.ValorICMS))))
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $@"
                                Aliquota ICMS - Base Calculo ICMS - Valor ICMS. 
                                CTe: Aliquota ICMS {cte?.AliquotaICMS ?? 0} x Pre-CTe: {preCte?.AliquotaISS ?? 0} - 
                                CTe: Base Calculo ICMS {cte?.BaseCalculoICMS ?? 0} x Pre-CTe: {preCte?.BaseCalculoICMS ?? 0} - 
                                CTe: Valor ICMS {cte?.ValorICMS ?? 0} x Pre-CTe: {preCte?.ValorICMS ?? 0}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && ((cte?.AliquotaICMS) != (preCte?.AliquotaICMS)) || ((cte?.BaseCalculoICMS) != (preCte?.BaseCalculoICMS)) || ((cte?.ValorICMS) != (preCte.ValorICMS)))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.CNPJTransportadora:

                            if (existeHistoricoIrregularidade.Count == 0 && (cte?.Empresa?.Codigo) != (preCte?.Empresa?.Codigo))
                            {
                                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                                Dominio.Entidades.Empresa empresaCTe = repEmpresa.BuscarPorCodigo(cte?.Empresa?.Codigo ?? 0);
                                Dominio.Entidades.Empresa empresaPreCTe = repEmpresa.BuscarPorCodigo(preCte?.Empresa?.Codigo ?? 0);

                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Emissores incompatíveis. CTe: {empresaCTe?.Descricao ?? ""} x Pre-CTe: {empresaPreCTe?.Descricao ?? ""}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && (cte?.Empresa?.Codigo) != (preCte?.Empresa?.Codigo))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.CSTICMS:
                            if (anexos == null || anexos.Count == 0 || !anexos.Exists(x => (x?.CodigoIrregularidade ?? 0) == irregularidade.Codigo))
                            {
                                if (existeHistoricoIrregularidade.Count == 0 && (cte?.CST) != (preCte?.CST))
                                {
                                    new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                    {
                                        ControleDocumento = controleDocumento,
                                        Irregularidade = irregularidade,
                                        Responsavel = responsavel,
                                        Observacao = $"CST Diferentes. CTe: {cte?.CST ?? ""} x Pre-CTe: {preCte?.CST ?? ""}"
                                    });

                                    GerarInconsistenciaNoControleDocumento(controleDocumento);
                                    break;
                                }
                                if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                                {
                                    GerarInconsistenciaNoControleDocumento(controleDocumento);
                                    break;
                                }

                                if (existeHistoricoIrregularidade.Count > 0 && ((cte?.CST) != (preCte?.CST)))
                                {
                                    GerarInconsistenciaNoControleDocumento(controleDocumento);
                                    break;
                                }
                            }

                            if (existeHistoricoIrregularidade.Count > 0)
                                AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.MunicipioPrestacaoServico:

                            if (existeHistoricoIrregularidade.Count == 0 && ((cte?.LocalidadeInicioPrestacao?.Codigo) != (preCte?.LocalidadeInicioPrestacao?.Codigo)))
                            {
                                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

                                Dominio.Entidades.Localidade localidadeInicioPrestacaoCTe = repLocalidade.BuscarPorCodigo(cte?.LocalidadeInicioPrestacao?.Codigo ?? 0);
                                Dominio.Entidades.Localidade localidadeInicioPrestacaoPreCTe = repLocalidade.BuscarPorCodigo(preCte?.LocalidadeInicioPrestacao?.Codigo ?? 0);

                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Municipios Diferentes. CTe: {localidadeInicioPrestacaoCTe?.Descricao ?? ""} x Pre-CTe: {localidadeInicioPrestacaoPreCTe?.Descricao ?? ""}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && (cte?.LocalidadeInicioPrestacao?.Codigo) != (preCte?.LocalidadeInicioPrestacao?.Codigo))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.CFOP:
                            if (existeHistoricoIrregularidade.Count == 0 && ((cte?.CFOP?.Codigo) != (preCte?.CFOP?.Codigo)))
                            {
                                Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);

                                Dominio.Entidades.CFOP cfopCTe = repCFOP.BuscarPorCodigo(cte?.CFOP?.Codigo ?? 0);
                                Dominio.Entidades.CFOP cfopPreCTe = repCFOP.BuscarPorCodigo(preCte?.CFOP?.Codigo ?? 0);

                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"CFOP Diferentes. CTe: {cfopCTe?.Descricao ?? ""} x Pre-CTe: {cfopPreCTe?.Descricao ?? ""}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }
                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && ((cte?.CFOP?.Codigo) != (preCte?.CFOP?.Codigo)))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.Participantes:
                            bool remetenteIgual = cte.Remetente.Codigo == preCte.Remetente.Codigo;
                            bool destinatarioIgual = cte.Destinatario.Codigo == preCte.Destinatario.Codigo;
                            bool expedidorIgual = (cte?.Expedidor?.Codigo ?? 0) == (preCte?.Expedidor?.Codigo ?? 0);
                            bool recebedorIgual = (cte?.Recebedor?.Codigo ?? 0) == (preCte?.Recebedor?.Codigo ?? 0);
                            bool informacoesIguais = recebedorIgual && destinatarioIgual && expedidorIgual && remetenteIgual;
                            if (existeHistoricoIrregularidade.Count == 0 && !informacoesIguais)
                            {
                                Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(_unitOfWork);

                                Dominio.Entidades.ParticipanteCTe remetenteCTe = repParticipanteCTe.BuscarPorCodigo(cte?.Remetente?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe remetentePreCTe = repParticipanteCTe.BuscarPorCodigo(preCte?.Remetente?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe destinatarioCTe = repParticipanteCTe.BuscarPorCodigo(cte?.Destinatario?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe destinatarioPreCTe = repParticipanteCTe.BuscarPorCodigo(preCte?.Destinatario?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe expedidorCTe = repParticipanteCTe.BuscarPorCodigo(cte?.Expedidor?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe expedidorPreCTe = repParticipanteCTe.BuscarPorCodigo(preCte?.Expedidor?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe recebedorCTe = repParticipanteCTe.BuscarPorCodigo(cte?.Expedidor?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe recebedorPreCTe = repParticipanteCTe.BuscarPorCodigo(preCte?.Expedidor?.Codigo ?? 0, false);

                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $@"
                                            Remetente - Destinatario - Expedidor - Recebedor.
                                            CTe: Remetente: {remetenteCTe?.Descricao ?? ""} x Pre-CTe: {remetentePreCTe?.Descricao ?? ""} -
                                            CTe: Destinatario: {destinatarioCTe?.Descricao ?? ""} x Pre-CTe: {destinatarioPreCTe?.Descricao ?? ""} -
                                            CTe: Expedidor: {expedidorCTe?.Descricao ?? ""} x Pre-CTe: {expedidorPreCTe?.Descricao ?? ""} -
                                            CTe: Recebedor: {recebedorCTe?.Descricao ?? ""} x Pre-CTe: {recebedorPreCTe?.Descricao ?? ""}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && !informacoesIguais)
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.AliquotaISSValorISS:
                            if (existeHistoricoIrregularidade.Count == 0 && ((cte?.AliquotaISS) != (preCte?.AliquotaISS)))
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Valor Aliquaota ISS. CTe: {cte?.AliquotaISS ?? 0} x Pre-CTe: {preCte?.AliquotaISS ?? 0}"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }
                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && ((cte?.AliquotaISS) != (preCte?.AliquotaISS)))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.ValorTotalReceber:
                            bool valoresIguais = (preCte?.ValorAReceber == cte?.ValorAReceber);
                            int percentual = irregularidade.PercentualTolerancia;
                            decimal valorTorelancia = irregularidade.ValorTolerancia;

                            if (percentual > 0 && valorTorelancia > 0)
                            {
                                decimal diferenca = Math.Abs(preCte.ValorAReceber - cte.ValorAReceber);

                                if ((diferenca <= (percentual / 100) * preCte.ValorAReceber && diferenca <= valorTorelancia))
                                    valoresIguais = true;
                            }

                            if (existeHistoricoIrregularidade.Count == 0 && !valoresIguais)
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $@"Valor Total Receber Diferentes 
                                CTe:  {cte?.ValorAReceber ?? 0} x Pre-CTe: {preCte?.ValorAReceber ?? 0} "

                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && !valoresIguais)
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.ValorPrestacaoServico:
                            bool valoresIgual = (preCte?.ValorPrestacaoServico == cte?.ValorPrestacaoServico);
                            int percentualIrregularidade = irregularidade.PercentualTolerancia;
                            decimal valorTorelanciaPermitida = irregularidade.ValorTolerancia;

                            if (percentualIrregularidade > 0 && valorTorelanciaPermitida > 0)
                            {
                                decimal diferenca = Math.Abs(preCte.ValorPrestacaoServico - cte.ValorPrestacaoServico);

                                if ((diferenca <= (percentualIrregularidade / 100) * preCte.ValorPrestacaoServico && diferenca <= valorTorelanciaPermitida))
                                    valoresIgual = true;
                            }

                            if (existeHistoricoIrregularidade.Count == 0 && !valoresIgual)
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $@"Valor Prestação Servico Diferentes 
                                CTe:  {cte?.ValorPrestacaoServico ?? 0} x Pre-CTe: {preCte?.ValorPrestacaoServico ?? 0} "
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && !valoresIgual)
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.CTeCancelado://Ver Com Gustavo se ja tem MIRO DEVE GERAR IRREGULARIDADE
                            if (cte.Status != "C")
                                break;

                            var existeDocumentoFaturamentoD = repositorioDocumentoFaturamento.BuscarPorCTe(cte.Codigo);
                            List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> irregularidadesPendentes = historicoIrregularidadesAtualDocumento.Where(x => x.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao).ToList();

                            if (cte.Status == "C" && irregularidadesPendentes.Count > 0)
                            {
                                foreach (var historicoIrregularidade in historicoIrregularidadesAtualDocumento)
                                {
                                    historicoIrregularidade.SituacaoIrregularidade = SituacaoIrregularidade.CTECancelado;
                                    repositorioHistoricoIrregularidade.Atualizar(historicoIrregularidade);
                                }
                                pararProcessamento = true;

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (cte.Status == "C" && existeDocumentoFaturamentoD != null && !string.IsNullOrEmpty(existeDocumentoFaturamentoD.NumeroMiro))
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Cte Cancelado - Possui MIRO"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (cte.Status == "C")
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Cte Cancelado"
                                });

                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }
                            break;
                        case GatilhoIrregularidade.NFeVinculadaAoFrete:
                            if (anexos == null || anexos.Count == 0 || !anexos.Exists(x => (x?.CodigoIrregularidade ?? 0) == irregularidade.Codigo))
                            {
                                Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(_unitOfWork);
                                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(_unitOfWork);

                                List<Dominio.Entidades.DocumentosPreCTE> documentosPreCTe = repDocumentosPreCte.BuscarPorPreCte(preCte.Codigo);
                                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(cte.Codigo);

                                List<string> codigoNotasPreCte = documentosPreCTe.Select(x => x.ChaveNFE).ToList();
                                List<string> codigoNotasCte = documentosCTe.Select(x => x.ChaveNFE).ToList();
                                List<string> documentosSomentePreCte = codigoNotasPreCte.Where(x => !codigoNotasCte.Contains(x)).ToList();
                                List<string> documentosSomenteCte = codigoNotasCte.Where(x => !codigoNotasPreCte.Contains(x)).ToList();

                                if (existeHistoricoIrregularidade.Count == 0 && documentosSomentePreCte.Count + documentosSomenteCte.Count > 0)
                                {
                                    string obs = documentosSomentePreCte.Count > 0 ? $@"Notas somente no Pre-CTe: {string.Join(", ", codigoNotasPreCte)}. " : string.Empty;
                                    if (documentosSomenteCte.Count > 0)
                                        obs += $@"Notas somente no CT-e: {string.Join(", ", documentosSomenteCte)}";

                                    new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                    {
                                        ControleDocumento = controleDocumento,
                                        Irregularidade = irregularidade,
                                        Responsavel = responsavel,
                                        Observacao = obs
                                    });

                                    GerarInconsistenciaNoControleDocumento(controleDocumento);
                                    break;
                                }

                                if (existeHistoricoIrregularidade.Count > 0 && documentosSomentePreCte.Count + documentosSomenteCte.Count > 0)
                                {
                                    GerarInconsistenciaNoControleDocumento(controleDocumento);
                                    break;
                                }
                            }

                            if (existeHistoricoIrregularidade.Count > 0)
                                AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.TomadorFreteUnilever:
                            if (existeHistoricoIrregularidade.Count == 0 && ((cte?.Tomador?.Cliente?.Codigo) != (preCte?.Tomador?.Cliente.Codigo)))
                            {
                                Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(_unitOfWork);

                                Dominio.Entidades.ParticipanteCTe participanteCTe = repParticipanteCTe.BuscarPorCodigo(cte.Tomador?.Codigo ?? 0, false);
                                Dominio.Entidades.ParticipanteCTe participantePreCTe = repParticipanteCTe.BuscarPorCodigo(preCte.Tomador?.Codigo ?? 0, false);

                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Tomadores Diferentes. CTe: {participanteCTe?.Descricao ?? ""} x Pre-CTe: {participantePreCTe?.Descricao ?? ""}"
                                });
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && ((cte?.Tomador?.Codigo) != (preCte?.Tomador?.Codigo)))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }
                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.SemLink:
                        case GatilhoIrregularidade.SemCalculo:
                            //caso tenha cargaCte mas nao tem PRe-cte? tambem cria irregularidade?..

                            if (existeHistoricoIrregularidade.Count == 0 && !repositorioCargaCte.ExistePorCTe(cte.Codigo))
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $"Cte Sem Vinculo Com a Carga"
                                });
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && !repositorioCargaCte.ExistePorCTe(cte.Codigo))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);
                            break;
                        case GatilhoIrregularidade.MIROBloqueioR:
                            var existeDocumentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCTe(cte.Codigo);

                            if (existeDocumentoFaturamento == null)
                                break;

                            if (existeDocumentoFaturamento != null && existeDocumentoFaturamento.Bloqueio == "R" && existeHistoricoIrregularidade.Count == 0)
                            {
                                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                                {
                                    ControleDocumento = controleDocumento,
                                    Irregularidade = irregularidade,
                                    Responsavel = responsavel,
                                    Observacao = $@"Bloqueio R do documento"
                                });
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }

                            if (existeHistoricoIrregularidade.Count > 0 && existeHistoricoIrregularidade.Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada))
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;
                            }


                            if (existeHistoricoIrregularidade.Count > 0 && existeDocumentoFaturamento.Bloqueio == "R")
                            {
                                GerarInconsistenciaNoControleDocumento(controleDocumento);
                                break;

                            }

                            AprovarHistoricoIrregularidade(existeHistoricoIrregularidade, AprovadorIrregularidade.Sistema);

                            break;
                        case GatilhoIrregularidade.PendenteSubstituicaoDocumento: //Sera implementado Depois Ciclo 5

                            break;
                        case GatilhoIrregularidade.NFeCancelada: //Ira ser implementada no Ciclo 5

                            break;
                    }

                    _unitOfWork.CommitChanges();
                }

                catch (ServicoException ex)
                {
                    TratarServicoException(controleDocumento, ex);
                }

                catch (Exception ex)
                {
                    TratarException(controleDocumento, repositorioControleDocumento, ex, "ValidarIrregularidadeControleDocumentoGatilhos");
                }
            }

            bool historicoAprovado = repositorioHistoricoIrregularidade.VerificarSeDocumentoLiberadoPorHistorico(controleDocumento.Codigo);

            if (historicoAprovado)
            {
                controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.Liberado;
                controleDocumento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.Verificado;
            }

            repositorioControleDocumento.Atualizar(controleDocumento);
        }

        public void AprovarHistoricoIrregularidade(List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicos, AprovadorIrregularidade aprovador)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = null;

            foreach (Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historico in historicos)
            {
                if (controleDocumento == null)
                    controleDocumento = historico.ControleDocumento;

                historico.SituacaoIrregularidade = SituacaoIrregularidade.Aprovada;
                historico.AprovadorIrregularidade = aprovador;
                repositorioIrregularidade.Atualizar(historico);
            }

            if (controleDocumento != null)
            {
                controleDocumento.MotivoIrregularidade = null;
                controleDocumento.SituacaoAprovacaoCartaDeCorrecao = SituacaoAprovacaoCartaDeCorrecao.AguardandoAprovacao;
                repControleDocumento.Atualizar(controleDocumento);
            }

        }


        public void GerarInconsistenciaNoControleDocumento(Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento)
        {

            Repositorio.Embarcador.Documentos.ControleDocumento repositorioContoleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            controleDocumento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.Verificado;
            controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.Inconsistente;
            repositorioContoleDocumento.Atualizar(controleDocumento);
        }

        public void CriarIrregularidadeIndividualDesacordo(Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento)
        {
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork);


            if (controleDocumento?.MotivoDesacordo == null)
                return;

            var novoHistorioIrregularidade = new Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade()
            {
                ControleDocumento = controleDocumento,
                DataIrregularidade = DateTime.Now,
                MotivoIrregularidade = null,
                Irregularidade = controleDocumento.MotivoDesacordo.Irregularidade,
                Observacao = "Irregularidade criada pela emissão do desacordo do CT-e",
                SituacaoIrregularidade = SituacaoIrregularidade.AguardandoAprovacao
            };

            repositorioHistoricoIrregularidade.Inserir(novoHistorioIrregularidade);
        }

        private void TratarException(ControleDocumento controleDocumento, Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento, Exception ex, string nomeArquivoLog)
        {
            _unitOfWork.Rollback();
            Servicos.Log.TratarErro(ex, nomeArquivoLog);

            controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.InconsistenteFalhaAoProcessar;
            controleDocumento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.Verificado;

            repositorioControleDocumento.Atualizar(controleDocumento);

            _unitOfWork.CommitChanges();
        }

        private void TratarServicoException(ControleDocumento controleDocumento, ServicoException ex)
        {
            if (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoBuscarTratativaIrregularidadeAtualDocumento)
            {
                controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.InconsistenteSemTratativa;
                controleDocumento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.Verificado;
                _unitOfWork.CommitChanges();

                return;
            }

            else
                _unitOfWork.Rollback();
        }

        #endregion
    }
}
