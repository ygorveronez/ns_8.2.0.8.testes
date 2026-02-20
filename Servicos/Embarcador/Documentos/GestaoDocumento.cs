using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Documentos
{
    public sealed class GestaoDocumento
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento _configuracaoEmissaoDocumentos;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public GestaoDocumento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, auditado: null) { }

        public GestaoDocumento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, auditado: null) { }

        public GestaoDocumento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador: null, auditado: auditado) { }

        public GestaoDocumento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void CriarAprovacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new GestaoDocumentoAprovacao(_unitOfWork);
            servicoGestaoDocumentoAprovacao.CriarAprovacao(gestaoDocumento, tipoServicoMultisoftware);

            if (gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Aprovado)
                Aprovar(gestaoDocumento, "Aprovação autorizada por regra de autorização", tipoServicoMultisoftware, _configuracaoEmbarcador);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento ObterConfiguracaoEmissaoDocumentos()
        {
            if (_configuracaoEmissaoDocumentos == null)
                _configuracaoEmissaoDocumentos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoEmissaoDocumentos;
        }

        private SituacaoGestaoDocumento ObterSituacaoGestaoDocumentoAprovado(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            return (gestaoDocumento.ValorDesconto > 0) ? SituacaoGestaoDocumento.AprovadoComDesconto : SituacaoGestaoDocumento.Aprovado;
        }

        private void VerificarInconsistenciaCTe(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            MotivoInconsistenciaGestaoDocumento motivoInconsistencia = MotivoInconsistenciaGestaoDocumento.EnvioPosteriorCarga;

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = gestaoDocumento.CTe;
                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
                byte[] xml = servicoCTe.ObterXMLAutorizacao(cte, _unitOfWork);

                if (xml != null)
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoEmissaoDocumentos = ObterConfiguracaoEmissaoDocumentos();

                    using (MemoryStream stream = new MemoryStream(xml))
                    {
                        object cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(stream);

                        if (cteLido != null)
                        {
                            if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                            {
                                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                StringBuilder mensagemErro = new StringBuilder();
                                Carga.PreCTe servicoPreCte = new Carga.PreCTe(_unitOfWork);

                                servicoPreCte.ValidarCTeV300(cteProc, gestaoDocumento.PreCTe, mensagemErro, ref motivoInconsistencia, configuracaoEmbarcador, configuracaoEmissaoDocumentos, _unitOfWork);
                            }
                            else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                            {
                                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                StringBuilder mensagemErro = new StringBuilder();
                                Carga.PreCTe servicoPreCte = new Carga.PreCTe(_unitOfWork);

                                servicoPreCte.ValidarCTeV400(cteProc, gestaoDocumento.PreCTe, mensagemErro, ref motivoInconsistencia, configuracaoEmbarcador, configuracaoEmissaoDocumentos, _unitOfWork);
                            }
                        }
                    }
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }

            gestaoDocumento.MotivoInconsistenciaGestaoDocumento = motivoInconsistencia;
        }

        private void VerificarReversaoAprovacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);

            if (gestaoDocumento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!gestaoDocumento.SituacaoGestaoDocumento.IsAprovado())
                throw new ServicoException("Não é possível desfazer a aprovação do documento em sua atual situação.");

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCTe(gestaoDocumento.CTe.Codigo, TipoLiquidacao.PagamentoTransportador, SituacaoDocumentoFaturamento.Liquidado);

            if (documentoFaturamento != null)
                throw new ServicoException("Não é possível desfazer a aprovação do documento pois ele já foi liquidado em um pagamento.");

            if (gestaoDocumento.CargaCTe != null)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> documentosOcorrencia = repositorioCargaOcorrenciaDocumento.BuscarPorCargaCTe(gestaoDocumento.CargaCTe.Codigo);

                if (documentosOcorrencia.Count > 0)
                    throw new ServicoException("Não é possível desfazer a aprovação do documento pois ele está sendo utilizado em uma ocorrência.");
            }
        }

        #endregion

        #region Métodos Públicos

        public void Aprovar(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, string observacaoAprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool ajustarValorFreteAprovacaoManual = false)
        {
            Carga.PreCTe servicoCargaPreCTe = new Carga.PreCTe(_unitOfWork);

            if (gestaoDocumento.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                servicoCargaPreCTe.SetarDocumentoOriginario(gestaoDocumento.CTe, _unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);

            gestaoDocumento.ObservacaoAprovacao = observacaoAprovacao;
            gestaoDocumento.SituacaoGestaoDocumento = ObterSituacaoGestaoDocumentoAprovado(gestaoDocumento);

            if (gestaoDocumento.CargaCTe != null)
            {
                gestaoDocumento.CargaCTe.CTe = gestaoDocumento.CTe;

                servicoCargaPreCTe.VincularPedidoXMLNotaAoCTe(gestaoDocumento.CTe, gestaoDocumento.CargaCTe.Carga, gestaoDocumento.CargaCTe, _unitOfWork);
                servicoCargaPreCTe.VincularConfiguracaoContabil(gestaoDocumento.CTe, gestaoDocumento.CargaCTe.PreCTe, _unitOfWork);
                repositorioCargaCTe.Atualizar(gestaoDocumento.CargaCTe);
                servicoCargaPreCTe.VerificarEnviouTodosDocumentos(_unitOfWork, gestaoDocumento.CargaCTe.Carga, tipoServicoMultisoftware, configuracao, ajustarValorFreteAprovacaoManual);
            }
            else
            {
                gestaoDocumento.CargaCTeComplementoInfo.CTe = gestaoDocumento.CTe;
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repositorioCargaCTe.BuscarPorCargaCTeComplementoInfo(gestaoDocumento.CargaCTeComplementoInfo.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                {
                    cargaCTe.CTe = gestaoDocumento.CTe;
                    servicoCargaPreCTe.VincularPedidoXMLNotaAoCTe(gestaoDocumento.CTe, cargaCTe.Carga, cargaCTe, _unitOfWork);
                    servicoCargaPreCTe.VincularConfiguracaoContabil(gestaoDocumento.CTe, cargaCTe.PreCTe, _unitOfWork);
                    repositorioCargaCTe.Atualizar(cargaCTe);
                }

                repositorioCargaCTeComplementoInfo.Atualizar(gestaoDocumento.CargaCTeComplementoInfo);
                Carga.PreCTe.VerificarEnviouTodosPreDocumentos(gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia, _unitOfWork);
            }

            repositorioGestaoDocumento.Atualizar(gestaoDocumento, _auditado);
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CriarCTe(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, Stream xml)
        {
            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cteProc.CTe.infCte.emit.CNPJ);

                //if (empresa == null)
                //    empresa = repEmpresa.BuscarTodasEmpresaPai("", 0).FirstOrDefault();

                if (empresa == null)
                    throw new ServicoException($"Empresa {cteProc.CTe.infCte.emit.CNPJ} não cadastrada.");

                if (!empresa.EmissaoDocumentosForaDoSistema)
                    throw new ServicoException($"Empresa {empresa.CNPJ} não configurada para emitir fora do sistema.");

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
                var retorno = servicoCTe.GerarCTeAnterior(empresa, cteProc, xml, _unitOfWork, string.Empty, string.Empty, false, true, false);

                if (retorno == null)
                    throw new ServicoException("O arquivo enviado não é um CT-e válido, por favor verifique.");

                if (retorno.GetType() == typeof(string))
                    throw new ServicoException((string)retorno);

                if ((retorno.GetType() != typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico)) && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxy") && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor"))
                    throw new ServicoException("Conhecimento de transporte inválido.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;

                return cte;
            }
            catch (ServicoException)
            {
                return null;
            }
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CriarCTe(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, Stream xml)
        {
            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cteProc.CTe.infCte.emit.Item);

                if (empresa == null)
                    throw new ServicoException($"Empresa {cteProc.CTe.infCte.emit.Item} não cadastrada.");

                if (!empresa.EmissaoDocumentosForaDoSistema)
                    throw new ServicoException($"Empresa {empresa.CNPJ} não configurada para emitir fora do sistema.");

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
                var retorno = servicoCTe.GerarCTeAnterior(empresa, cteProc, xml, _unitOfWork, string.Empty, string.Empty, false, true, false);

                if (retorno == null)
                    throw new ServicoException("O arquivo enviado não é um CT-e válido, por favor verifique.");

                if (retorno.GetType() == typeof(string))
                    throw new ServicoException((string)retorno);

                if ((retorno.GetType() != typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico)) && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxy") && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor"))
                    throw new ServicoException("Conhecimento de transporte inválido.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;

                return cte;
            }
            catch (ServicoException)
            {
                return null;
            }
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CriarCTe(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc cteProc, Stream xml)
        {
            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cteProc.CTeOS.infCte.emit.CNPJ);

                if (empresa == null)
                    throw new ServicoException($"Empresa {cteProc.CTeOS.infCte.emit.CNPJ} não cadastrada.");

                if (!empresa.EmissaoDocumentosForaDoSistema)
                    throw new ServicoException($"Empresa {empresa.CNPJ} não configurada para emitir fora do sistema.");

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
                var retorno = servicoCTe.GerarCTeAnterior(empresa, cteProc, xml, _unitOfWork);

                if (retorno == null)
                    throw new ServicoException("O arquivo enviado não é um CT-e válido, por favor verifique.");

                if (retorno.GetType() == typeof(string))
                    throw new ServicoException((string)retorno);

                if ((retorno.GetType() != typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico)) && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxy") && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor"))
                    throw new ServicoException("Conhecimento de transporte inválido.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;

                return cte;
            }
            catch (ServicoException)
            {
                return null;
            }
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CriarCTe(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc cteProc, Stream xml)
        {
            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cteProc.CTeOS.infCte.emit.CNPJ);

                if (empresa == null)
                    throw new ServicoException($"Empresa {cteProc.CTeOS.infCte.emit.CNPJ} não cadastrada.");

                if (!empresa.EmissaoDocumentosForaDoSistema)
                    throw new ServicoException($"Empresa {empresa.CNPJ} não configurada para emitir fora do sistema.");

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
                var retorno = servicoCTe.GerarCTeAnterior(empresa, cteProc, xml, _unitOfWork);

                if (retorno == null)
                    throw new ServicoException("O arquivo enviado não é um CT-e válido, por favor verifique.");

                if (retorno.GetType() == typeof(string))
                    throw new ServicoException((string)retorno);

                if ((retorno.GetType() != typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico)) && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxy") && (retorno.GetType().Name != "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor"))
                    throw new ServicoException("Conhecimento de transporte inválido.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;

                return cte;
            }
            catch (ServicoException)
            {
                return null;
            }
        }


        public void CriarInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MotivoInconsistenciaGestaoDocumento motivoInconsistenciaGestao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            CriarInconsitencia(cte, motivoInconsistenciaGestao, tipoServicoMultisoftware, cargaCTe: null, cargaCTeComplementoInfo: null, detalhesInconsistencia: "");
        }

        public void CriarInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MotivoInconsistenciaGestaoDocumento motivoInconsistenciaGestao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string detalhesInconsistencia)
        {
            CriarInconsitencia(cte, motivoInconsistenciaGestao, tipoServicoMultisoftware, cargaCTe: null, cargaCTeComplementoInfo: null, detalhesInconsistencia: detalhesInconsistencia);
        }

        public void CriarInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MotivoInconsistenciaGestaoDocumento motivoInconsistenciaGestao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            CriarInconsitencia(cte, motivoInconsistenciaGestao, tipoServicoMultisoftware, cargaCTe, cargaCTeComplementoInfo: null, detalhesInconsistencia: "");
        }

        public void CriarInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MotivoInconsistenciaGestaoDocumento motivoInconsistenciaGestao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo)
        {
            CriarInconsitencia(cte, motivoInconsistenciaGestao, tipoServicoMultisoftware, cargaCTe: null, cargaCTeComplementoInfo: cargaCTeComplementoInfo, detalhesInconsistencia: "");
        }

        public void CriarInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MotivoInconsistenciaGestaoDocumento motivoInconsistenciaGestao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, string detalhesInconsistencia)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Repositorio.Embarcador.Documentos.GestaoDocumentoHistoricoCTe repositorioGestaoDocumentoHistoricoCTe = new Repositorio.Embarcador.Documentos.GestaoDocumentoHistoricoCTe(_unitOfWork);

            bool substituicaoCTe = false;
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe gestaoDocumentoHistoricoCTe = null;

            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCTe(cte.Codigo);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumentoAnterior = cargaCTe != null ? repositorioGestaoDocumento.BuscarPorCargaCTe(cargaCTe.Codigo) : null;

            if ((gestaoDocumento == null || (gestaoDocumento != null && gestaoDocumento.CargaCTe != null && cargaCTe != null && gestaoDocumento.CargaCTe.Codigo != cargaCTe.Codigo)) &&
                gestaoDocumentoAnterior != null && gestaoDocumentoAnterior.CTe.Codigo != cte.Codigo)
            {
                substituicaoCTe = true;

                gestaoDocumentoHistoricoCTe = new Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe()
                {
                    GestaoDocumento = gestaoDocumentoAnterior,
                    CTe = gestaoDocumentoAnterior.CTe,
                    Data = DateTime.Now
                };

                gestaoDocumento = gestaoDocumentoAnterior;
                gestaoDocumento.CTe = cte;
            }
            else if ((gestaoDocumento != null) && (gestaoDocumento.SituacaoGestaoDocumentoAprovada || (gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Rejeitado)))
                return;

            Repositorio.DocumentosCTE repositorioDocumentosCTe = new Repositorio.DocumentosCTE(_unitOfWork);
            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repositorioDocumentosCTe.BuscarPorCTe(cte.Codigo);

            if (gestaoDocumento == null)
                gestaoDocumento = new Dominio.Entidades.Embarcador.Documentos.GestaoDocumento()
                {
                    CTe = cte
                };

            gestaoDocumento.CargaCTe = cargaCTe;
            gestaoDocumento.CargaCTeComplementoInfo = cargaCTeComplementoInfo;
            gestaoDocumento.DetalhesInconsistencia = detalhesInconsistencia;
            gestaoDocumento.MotivoInconsistenciaGestaoDocumento = motivoInconsistenciaGestao;
            gestaoDocumento.NFeRecebida = string.Join(", ", (from documento in documentosCTe where !string.IsNullOrEmpty(documento.Numero.ToString()) select documento.Numero.ToString()).Distinct());
            gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.Inconsistente;
            gestaoDocumento.QuantidadeImportacoesCTe++;
            gestaoDocumento.DataImportacaoCTe = DateTime.Now;

            if (gestaoDocumento.Codigo == 0)
            {
                repositorioGestaoDocumento.Inserir(gestaoDocumento);
                CriarAprovacao(gestaoDocumento, tipoServicoMultisoftware);
            }

            repositorioGestaoDocumento.Atualizar(gestaoDocumento);

            if (substituicaoCTe)
            {
                if (gestaoDocumentoHistoricoCTe != null)
                    repositorioGestaoDocumentoHistoricoCTe.Inserir(gestaoDocumentoHistoricoCTe);

                CriarAprovacao(gestaoDocumento, tipoServicoMultisoftware);
            }
        }

        public void ColocarEmTratativa(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);

            gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.EmTratativa;

            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
        }

        public void DefinirCargaCTeComplementoInfoPorPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCTeComplementoInfo.PreCTe == null)
                return;

            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCTeComplementado(cargaCTeComplementoInfo.PreCTe.ChaveCTESubComp);

            if (gestaoDocumento == null)
                return;

            gestaoDocumento.CargaCTeComplementoInfo = cargaCTeComplementoInfo;

            VerificarInconsistenciaCTe(gestaoDocumento);

            if (gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Inconsistente || gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.EmTratativa)
                CriarAprovacao(gestaoDocumento, tipoServicoMultisoftware);

            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
        }

        public void DefinirCargaCTePorPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCTe.PreCTe == null)
                return;

            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento;

            if (
                cargaCTe.PreCTe.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho ||
                cargaCTe.PreCTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ||
                cargaCTe.PreCTe.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario
            )
            {
                Repositorio.DocumentoDeTransporteAnteriorPreCTE repositorioDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(_unitOfWork);
                string chaveCte = repositorioDocumentoDeTransporteAnteriorPreCTE.BuscarChaveCTeAnteriorPorPreCte(cargaCTe.PreCTe.Codigo);
                gestaoDocumento = repositorioGestaoDocumento.BuscarPorCTeAnterior(chaveCte);
            }
            else
            {
                Repositorio.DocumentosPreCTE repositorioDocumentosPreCTE = new Repositorio.DocumentosPreCTE(_unitOfWork);
                string chaveNFe = repositorioDocumentosPreCTE.BuscarChaveNFePorPreCte(cargaCTe.PreCTe.Codigo);
                gestaoDocumento = repositorioGestaoDocumento.BuscarPorNotaFiscal(chaveNFe);
            }

            if (gestaoDocumento == null)
                return;

            gestaoDocumento.CargaCTe = cargaCTe;

            VerificarInconsistenciaCTe(gestaoDocumento);

            if (gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Inconsistente || gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.EmTratativa)
                CriarAprovacao(gestaoDocumento, tipoServicoMultisoftware);

            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
        }

        public void DesfazerAprovacao(int codigoGestaoDocumento)
        {
            Carga.PreCTe servicoCargaPreCTe = new Carga.PreCTe(_unitOfWork);
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento, true);

            VerificarReversaoAprovacao(gestaoDocumento);

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new GestaoDocumentoAprovacao(_unitOfWork);

            gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.Inconsistente;
            gestaoDocumento.ObservacaoAprovacao = string.Empty;

            if (gestaoDocumento.CargaCTe != null)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                gestaoDocumento.CargaCTe.CTe = null;
                gestaoDocumento.CargaCTe.Carga.AgImportacaoCTe = true;

                if (gestaoDocumento.CargaCTe.Carga.SituacaoCarga.IsSituacaoCargaFaturada())
                    gestaoDocumento.CargaCTe.Carga.LiberadaSemTodosPreCTes = true;

                repositorioCargaCTe.Atualizar(gestaoDocumento.CargaCTe);
                repositorioCarga.Atualizar(gestaoDocumento.CargaCTe.Carga);
                servicoCargaPreCTe.DesvincularConfiguracaoContabil(gestaoDocumento.CTe, gestaoDocumento.CargaCTe.PreCTe, _unitOfWork);
                new Hubs.Carga().InformarCargaAtualizada(gestaoDocumento.CargaCTe.Carga.Codigo, TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
            }
            else
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repositorioCargaCTe.BuscarPorCargaCTeComplementoInfo(gestaoDocumento.CargaCTeComplementoInfo.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                {
                    cargaCTe.CTe = null;

                    repositorioCargaCTe.Atualizar(cargaCTe);
                    servicoCargaPreCTe.DesvincularConfiguracaoContabil(gestaoDocumento.CTe, cargaCTe.PreCTe, _unitOfWork);
                }

                gestaoDocumento.CargaCTeComplementoInfo.CTe = null;
                gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia.AgImportacaoCTe = true;
                gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.EmEmissaoCTeComplementar;

                repositorioCargaCTeComplementoInfo.Atualizar(gestaoDocumento.CargaCTeComplementoInfo);
                repositorioCargaOcorrencia.Atualizar(gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia);
                new Hubs.Ocorrencia().InformarOcorrenciaAtualizada(gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia.Codigo);
            }

            servicoGestaoDocumentoAprovacao.RemoverAprovacao(gestaoDocumento);
            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
            Auditoria.Auditoria.Auditar(_auditado, gestaoDocumento, "Revertida a aprovação do documento", _unitOfWork);
        }

        public void DesfazerRejeicao(int codigoGestaoDocumento)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento, true);

            if (gestaoDocumento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Rejeitado)
                throw new ServicoException("Não é possível desfazer a rejeição do documento em sua atual situação.");

            GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new GestaoDocumentoAprovacao(_unitOfWork);

            gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.Inconsistente;

            servicoGestaoDocumentoAprovacao.RemoverAprovacao(gestaoDocumento);
            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
            Auditoria.Auditoria.Auditar(_auditado, gestaoDocumento, "Revertida a rejeição do documento", _unitOfWork);
        }

        public void RemoverInconsitencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCTe(cte.Codigo);

            if ((gestaoDocumento == null) || (gestaoDocumento.MotivoInconsistenciaGestaoDocumento == MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria))
                return;

            GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new GestaoDocumentoAprovacao(_unitOfWork);

            servicoGestaoDocumentoAprovacao.RemoverAprovacao(gestaoDocumento);
            repositorioGestaoDocumento.Deletar(gestaoDocumento);
        }

        #endregion
    }
}
