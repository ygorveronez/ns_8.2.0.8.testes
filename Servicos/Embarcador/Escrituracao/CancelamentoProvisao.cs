using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Escrituracao
{
    public class CancelamentoProvisao : ServicoBase
    {
        public CancelamentoProvisao(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #region Métodos Públicos

        public static void CancelarProvisaoDocumentosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

            if (!carga.CargaAgrupada)
                CancelarCargaProvisaoDocumentosCarga(carga, auditado, unitOfWork);
            else
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = repCarga.BuscarCargasOriginais(carga.Codigo);
                cargasOrigem.Add(carga);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOrigem)
                    CancelarCargaProvisaoDocumentosCarga(cargaOrigem, auditado, unitOfWork);
            }
        }


        private static void CancelarCargaProvisaoDocumentosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            repDocumentoContabil.SetarReferenciaProvisaoNulaPorCarga(carga.Codigo);
            repDocumentoProvisao.ExcluirTodosAgProvisaoPorCarga(carga.Codigo);
            bool existeDocumentoProvisionado = repDocumentoProvisao.ExisteDocumentoProvisionadoPorCarga(carga.Codigo);
            if (existeDocumentoProvisionado)
            {
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao();
                cancelamentoProvisao.DataCriacao = DateTime.Now;
                cancelamentoProvisao.Numero = repCancelamentoProvisao.ObterProximoNumero();
                cancelamentoProvisao.Carga = carga;
                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento;
                repCancelamentoProvisao.Inserir(cancelamentoProvisao);
                Servicos.Auditoria.Auditoria.Auditar(auditado, cancelamentoProvisao, null, "Adicionado pelo Cancelamento da Carga", unitOfWork);
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.SetarDocumentosSelecionados(cancelamentoProvisao, carga.Codigo, 0, DateTime.MinValue, DateTime.MinValue, 0, 0, 0, null, null, true, unitOfWork);
            }
        }

        public static void CancelarProvisaoDocumentosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

            repDocumentoContabil.SetarReferenciaProvisaoNulaPorOcorrencia(cargaOcorrencia.Codigo);
            repDocumentoProvisao.ExcluirTodosAgProvisaoPorOcorrencia(cargaOcorrencia.Codigo);

            bool existeDocumentoProvisionado = repDocumentoProvisao.ExisteDocumentoProvisionadoPorOcorrencia(cargaOcorrencia.Codigo);
            if (existeDocumentoProvisionado)
            {
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao();
                cancelamentoProvisao.DataCriacao = DateTime.Now;
                cancelamentoProvisao.Numero = repCancelamentoProvisao.ObterProximoNumero();
                cancelamentoProvisao.CargaOcorrencia = cargaOcorrencia;
                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento;
                repCancelamentoProvisao.Inserir(cancelamentoProvisao);
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.SetarDocumentosSelecionados(cancelamentoProvisao, 0, cargaOcorrencia.Codigo, DateTime.MinValue, DateTime.MinValue, 0, 0, 0, null, null, true, unitOfWork);
            }
        }

        public static void EfetuarIntegracaoCancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            bool agIntegracao = false;

            if (EfetuarIntegracaoEDICancelamentoProvisao(cancelamentoProvisao, unitOfWork))
                agIntegracao = true;

            if (EfetuarIntegracaoEspecificasCancelamentoProvisao(cancelamentoProvisao, unitOfWork))
                agIntegracao = true;

            if (agIntegracao)
                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao;
            else
                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Cancelado;

            repCancelamentoProvisao.Atualizar(cancelamentoProvisao);
        }

        public static bool EfetuarIntegracaoEspecificasCancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoDeIntegracoesParaGerar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil
            };
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracao = repTipoIntegracao.BuscarPorTipos(tipoDeIntegracoesParaGerar);

            bool possuiIntegracao = false;
            foreach (var tipo in tipoIntegracao)
            {
                switch (tipo.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        GerarIntegracaoCancelamentoProvisaoPorDocumento(cancelamentoProvisao, tipo, unitOfWork);
                        possuiIntegracao = true;
                        break;
                    default:
                        break;
                }
            }

            return possuiIntegracao;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>> LayoutEDICancelamentoProvisaoAsync(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Cliente> tomadores = await repositorioDocumentoProvisao.BuscarTomadoresCancelamentoProvisaoAsync(cancelamentoProvisao.Codigo, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from tomador in tomadores where tomador.GrupoPessoas != null select tomador.GrupoPessoas).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            for (int i = 0, s = grupoPessoas.Count; i < s; i++)
                if (grupoPessoas[i].LayoutsEDI != null)
                    layouts.AddRange(grupoPessoas[i].LayoutsEDI.ToList());

            return layouts;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>> LayoutEDICancelamentoProvisaoClienteAsync(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Cliente> tomadores = await repositorioDocumentoProvisao.BuscarTomadoresCancelamentoProvisaoAsync(cancelamentoProvisao.Codigo, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();

            for (int i = 0, s = tomadores.Count; i < s; i++)
                if (tomadores[i].LayoutsEDI != null)
                    layouts.AddRange(tomadores[i].LayoutsEDI.ToList());

            return layouts;
        }

        public static DateTime ObterDataLancamento(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao configuracaoProvisao = ObterConfiguracaoProvisao(unitOfWork);
            DateTime dataAtual = DateTime.Now.Date;
            DateTime ultimoDiaMesAnterior = new DateTime(dataAtual.Year, dataAtual.Month, day: 1).AddDays(-1);

            DateTime dataFormaMes = dataAtual.AddDays(-configuracaoProvisao?.DiasForaMes ?? 0);

            if (dataFormaMes <= ultimoDiaMesAnterior)
                return ultimoDiaMesAnterior;

            return dataAtual;
        }

        public static void ProcessarProvisoesEmCancelamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> provisoesCancelamento = repCancelamentoProvisao.BuscarProvisaoEmCancelamento();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao in provisoesCancelamento)
                    GerarFechamentoDocumentosCancelamentoProvisao(cancelamentoProvisao, unidadeTrabalho, stringConexao, tipoServicoMultisoftware);

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private static Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao ObterConfiguracaoProvisao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao repositorio = new Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao(unitOfWork);

            return repositorio.BuscarConfiguracao();
        }

        private static bool EfetuarIntegracaoEDICancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repositorioCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unitOfWork);

            // Atualiza status do cancelamentoProvisao
            cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao;

            // Cria entidade para integracao
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new Servicos.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork).LayoutEDICancelamentoProvisaoAsync(cancelamentoProvisao).GetAwaiter().GetResult();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> EDIs = (from o in layouts where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR select o).ToList();

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> cliente = new Servicos.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork).LayoutEDICancelamentoProvisaoClienteAsync(cancelamentoProvisao).GetAwaiter().GetResult();
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> EDIsCliente = (from o in cliente where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR select o).ToList();


            if (EDIsCliente.Count > 0)
            {
                int count = EDIsCliente.Count();
                for (int i = 0; i < count; i++)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao cancelamentoProvisaoEDIIntegracao = repositorioCancelamentoProvisaoEDIIntegracao.BuscarPorCancelamentoProvisaoELayoutEDI(cancelamentoProvisao.Codigo, EDIsCliente[i].LayoutEDI.Codigo);
                    if (cancelamentoProvisaoEDIIntegracao == null)
                    {
                        cancelamentoProvisaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao
                        {
                            CancelamentoProvisao = cancelamentoProvisao,
                            LayoutEDI = EDIsCliente[i].LayoutEDI,
                            TipoIntegracao = EDIsCliente[i].TipoIntegracao,
                            SequenciaIntegracao = 1,
                            ProblemaIntegracao = "",
                            NumeroTentativas = 0,
                            DataIntegracao = DateTime.Now,
                            SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        };

                        repositorioCancelamentoProvisaoEDIIntegracao.Inserir(cancelamentoProvisaoEDIIntegracao);
                    }
                }
                return count > 0;
            }
            else
            {
                int count = EDIs.Count();
                for (int i = 0; i < count; i++)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao cancelamentoProvisaoEDIIntegracao = repositorioCancelamentoProvisaoEDIIntegracao.BuscarPorCancelamentoProvisaoELayoutEDI(cancelamentoProvisao.Codigo, EDIs[i].LayoutEDI.Codigo);
                    if (cancelamentoProvisaoEDIIntegracao == null)
                    {
                        cancelamentoProvisaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao
                        {
                            CancelamentoProvisao = cancelamentoProvisao,
                            LayoutEDI = EDIs[i].LayoutEDI,
                            TipoIntegracao = EDIs[i].TipoIntegracao,
                            SequenciaIntegracao = 1,
                            ProblemaIntegracao = "",
                            NumeroTentativas = 0,
                            DataIntegracao = DateTime.Now,
                            SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        };

                        repositorioCancelamentoProvisaoEDIIntegracao.Inserir(cancelamentoProvisaoEDIIntegracao);
                    }
                }
                return count > 0;
            }
        }

        private static void GerarFechamentoDocumentosCancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repProvisaoDocumentos = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);
            Hubs.CancelamentoProvisao servicoNotificacaoCancelamentoProvisao = new Hubs.CancelamentoProvisao();

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada> provisoesCancelamentoSumarizado = repProvisaoDocumentos.BuscarCodigosPorProvisaoEmCancelamento(cancelamentoProvisao.Codigo);

            int quantidadeDocumentos = cancelamentoProvisao.QuantidadeDocsProvisao;
            int quantidadeGerados = quantidadeDocumentos - provisoesCancelamentoSumarizado.Count();

            int quantidadeTotal = 0;

            for (var i = 0; i < provisoesCancelamentoSumarizado.Count(); i++)
            {
                unidadeTrabalho.Start();

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada cancelamentoProvisaoSumarizada = provisoesCancelamentoSumarizado[i];

                GerarDocumentoContabil(cancelamentoProvisaoSumarizada, cancelamentoProvisao, unidadeTrabalho);
                repProvisaoDocumentos.SetarDocumentoMovimentoGeradoCancelamento(cancelamentoProvisaoSumarizada.Codigo);
                DisponibilizarDocumentoParaNovaProvisao(cancelamentoProvisaoSumarizada, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                if (quantidadeDocumentos < 10 || ((quantidadeTotal + 1) % 5) == 0)
                {
                    unidadeTrabalho.FlushAndClear();
                    servicoNotificacaoCancelamentoProvisao.InformarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao(cancelamentoProvisao.Codigo, quantidadeDocumentos, ((quantidadeGerados + i) + 1));
                }
                quantidadeTotal++;
            }

            EfetuarIntegracaoCancelamentoProvisao(cancelamentoProvisao, unidadeTrabalho);

            cancelamentoProvisao.DataLancamento = ObterDataLancamento(unidadeTrabalho);
            cancelamentoProvisao.GerandoMovimentoFinanceiro = false;
            cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao;

            repCancelamentoProvisao.Atualizar(cancelamentoProvisao);

            servicoNotificacaoCancelamentoProvisao.InformarCancelamentoProvisaoAtualizada(cancelamentoProvisao.Codigo);
        }

        private static void GerarDocumentoContabil(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada cancelamentoProvisaoSumarizada, Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentoContabils = repDocumentoContabil.BuscarPorDocumentoProvisao(cancelamentoProvisaoSumarizada.Codigo);
            for (int i = 0; i < documentoContabils.Count; i++)
            {
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil = documentoContabils[i].Clonar();
                documentoContabil.CancelamentoProvisao = cancelamentoProvisao;
                documentoContabil.DataRegistro = DateTime.Now;
                documentoContabil.DataLancamento = cancelamentoProvisao.DataCriacao;

                if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito)
                    documentoContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
                else
                    documentoContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito;
                repDocumentoContabil.Inserir(documentoContabil);
            }
        }

        private static void DisponibilizarDocumentoParaNovaProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada cancelamentoProvisaoSumarizada, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unidadeTrabalho);
            bool duplicar = false;

            if (cancelamentoProvisaoSumarizada.CargaOcorrencia.HasValue && cancelamentoProvisaoSumarizada.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada && cancelamentoProvisaoSumarizada.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada)
                duplicar = true;
            else if (!cancelamentoProvisaoSumarizada.CargaOcorrencia.HasValue && cancelamentoProvisaoSumarizada.Carga.HasValue && cancelamentoProvisaoSumarizada.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cancelamentoProvisaoSumarizada.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                duplicar = true;

            if (duplicar)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisaoExiste = repDocumentoProvisao.BuscarPorCodigo(cancelamentoProvisaoSumarizada.Codigo);
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = documentoProvisaoExiste.Clonar();
                documentoProvisao.CancelamentoProvisao = null;
                documentoProvisao.Provisao = null;
                documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao;
                documentoProvisao.MovimentoFinanceiroGerado = false;
                repDocumentoProvisao.Inserir(documentoProvisao);
            }
        }

        private static void GerarIntegracaoCancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao()
            {
                CancelamentoProvisao = cancelamentoProvisao,
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipo,
                ProblemaIntegracao = ""
            };

            repositorioCancelamentoProvisaoIntegracao.Inserir(integracao);

        }

        private static void GerarIntegracaoCancelamentoProvisaoPorDocumento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            List<int> documentos = repositorioDocumentoProvisao.BuscarCodigosPorCancelamentoProvisao(cancelamentoProvisao.Codigo);

            if (documentos.Count > 1)
                switch (tipo.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        documentos = documentos.Take(1).ToList();
                        break;
                    default:
                        break;
                }

            foreach (var documento in documentos)
            {
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao()
                {
                    CancelamentoProvisao = cancelamentoProvisao,
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipo,
                    ProblemaIntegracao = "",
                    DocumentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao() { Codigo = documento }
                };
                repositorioCancelamentoProvisaoIntegracao.Inserir(integracao);
            }
        }


        #endregion
    }
}
