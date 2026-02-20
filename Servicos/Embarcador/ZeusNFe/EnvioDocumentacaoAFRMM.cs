using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.NFSe;
using Repositorio;
using Servicos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Zeus.Embarcador.ZeusNFe
{
    public class EnvioDocumentacaoAFRMM : ServicoBase
    {        
        public EnvioDocumentacaoAFRMM() : base() { }

        #region Métodos Publicos

        public static void ProcessarEnvioDocumentacaoAFRMM(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, string host, bool envioAutomatico)
        {
            try
            {
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM repEnvioDocumentacaoAFRMM = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM(unidadeTrabalho);

                List<long> codigosDocumentacaoImpressao = repEnvioDocumentacaoAFRMM.BuscarCodigosPorSituacao(TipoDocumentacaoAFRMM.FTP, SituacaoEnvioDocumentacaoLote.Aguardando, envioAutomatico, "Codigo", "asc", 0, envioAutomatico ? 10 : 3);
                for (var i = 0; i < codigosDocumentacaoImpressao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote = repEnvioDocumentacaoAFRMM.BuscarPorCodigo(codigosDocumentacaoImpressao[i]);

                    if (!ProcessarEnvioDocumentacaoLote(out string erro, documentacaoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host, true))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }

                List<long> codigosDocumentacao = repEnvioDocumentacaoAFRMM.BuscarCodigosPorSituacao(TipoDocumentacaoAFRMM.Email, SituacaoEnvioDocumentacaoLote.Aguardando, envioAutomatico, "Codigo", "asc", 0, envioAutomatico ? 10 : 3);
                for (var i = 0; i < codigosDocumentacao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote = repEnvioDocumentacaoAFRMM.BuscarPorCodigo(codigosDocumentacao[i]);

                    if (!ProcessarEnvioDocumentacaoLote(out string erro, documentacaoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host, false))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void ProcessarEnvioAutomaticoDocumentacao(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host)
        {
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracaoDocumentacao = repConfiguracao.BuscarConfiguracaoPadrao();

            if (configuracaoDocumentacao == null || configuracaoDocumentacao.QuantidadeDiasAposDescarga <= 0)
                return;

            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMM> documentacaoPendente = repCarga.BuscarDocumentacaoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga);
            for (int i = 0; i < documentacaoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoPendente[i].CodigoViagem, documentacaoPendente[i].CodigoPortoDestino, documentacaoPendente[i].CodigoPortoOrigem);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMM> documentacaoTransbordoPendente = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, 5);
            for (int i = 0; i < documentacaoTransbordoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoTransbordoPendente[i].CodigoViagem, documentacaoTransbordoPendente[i].CodigoPortoDestino, documentacaoTransbordoPendente[i].CodigoPortoOrigem, 5);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }


            documentacaoTransbordoPendente = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, 4);
            for (int i = 0; i < documentacaoTransbordoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoTransbordoPendente[i].CodigoViagem, documentacaoTransbordoPendente[i].CodigoPortoDestino, documentacaoTransbordoPendente[i].CodigoPortoOrigem, 4);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }


            documentacaoTransbordoPendente = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, 3);
            for (int i = 0; i < documentacaoTransbordoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoTransbordoPendente[i].CodigoViagem, documentacaoTransbordoPendente[i].CodigoPortoDestino, documentacaoTransbordoPendente[i].CodigoPortoOrigem, 3);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }


            documentacaoTransbordoPendente = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, 2);
            for (int i = 0; i < documentacaoTransbordoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoTransbordoPendente[i].CodigoViagem, documentacaoTransbordoPendente[i].CodigoPortoDestino, documentacaoTransbordoPendente[i].CodigoPortoOrigem, 2);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }

            documentacaoTransbordoPendente = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, 1);
            for (int i = 0; i < documentacaoTransbordoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM()
                {
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoViagem),
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.NaoEnviado,
                    PortoDestino = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoDestino),
                    PortoOrigem = repPorto.BuscarPorCodigo(documentacaoTransbordoPendente[i].CodigoPortoOrigem),
                    Usuario = null,
                    TipoDocumentacaoAFRMM = TipoDocumentacaoAFRMM.FTP
                };

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacaoAFRMCTe> conhecimentos = repCarga.BuscarDocumentacaoTransbordoAutomaticaPendente(configuracaoDocumentacao.QuantidadeDiasAposDescarga, documentacaoTransbordoPendente[i].CodigoViagem, documentacaoTransbordoPendente[i].CodigoPortoDestino, documentacaoTransbordoPendente[i].CodigoPortoOrigem, 1);
                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (conhecimentos != null && conhecimentos.Count > 0)
                {
                    foreach (var cte in conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(cte.CodigoCTe);
                        envioDocumentacaoLote.CTes.Add(cteAlterar);

                        cteAlterar.GerouRegistroDocumentacaoAFRMM = true;
                        repCTe.Atualizar(cteAlterar);
                    }
                }

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }


            return;
        }

        #endregion

        #region Métodos Privados

        private static bool ProcessarEnvioDocumentacaoLote(out string erro, Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host, bool enviarParaFTP)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);

            Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM repEnvioDocumentacaoAFRMM = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(host);

            string urlAcesso = clienteAcesso.URLAcesso;
            int codigoUsuario = documentacaoLote.Usuario?.Codigo ?? 0;
            string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos;
            string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;
            string caminhoArquivosAnexos = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" })}\\";

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = null;
            List<int> codigosCTes = new List<int>();
            if (documentacaoLote.CTes != null && documentacaoLote.CTes.Count > 0)
                codigosCTes = documentacaoLote.CTes.Select(c => c.Codigo).ToList();

            List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> listaRetornoFalha = new List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus>();

            ctes = repCTe.BuscarPorCodigo(codigosCTes);

            if (ctes != null && ctes.Count > 0)
            {
                foreach (var cte in ctes)
                    listaRetornoFalha.AddRange(EnviarEmailDocumentacaoAFRMM(documentacaoLote, urlAcesso, cte, documentacaoLote.EmailInformadoManualmente, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, unidadeTrabalho, caminhoArquivosAnexos, enviarParaFTP));

                if (listaRetornoFalha.Any(obj => !obj.Sucesso))
                {
                    documentacaoLote.QuantidadeTentativaEnvio += 1;
                    if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 3)
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                    else
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;
                    documentacaoLote.Retorno = "Não foi possível gerar todos os PDFs";

                    if (documentacaoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Documentos/DocumentacaoAFRMM", Localization.Resources.Zeus.DocumentacaoAFRMM.NaoFoiPossivelGerarDocumentacao, IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                }
                else
                {
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Finalizado;
                    documentacaoLote.Retorno = "Sucesso";
                    if (documentacaoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Documentos/DocumentacaoAFRMM", Localization.Resources.Zeus.DocumentacaoAFRMM.GeradoSucesso, IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                }

                repEnvioDocumentacaoAFRMM.Atualizar(documentacaoLote);
            }
            else
            {
                documentacaoLote.QuantidadeTentativaEnvio += 1;
                if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 3)
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                else
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;
                documentacaoLote.Retorno = "Nenhum CT-e selecionado.";
                EnviarEmailProblemaEnvioAutomatico("Nenhum CT-e localizado.", documentacaoLote, documentacaoLote.PortoDestino?.Empresa ?? null, unidadeTrabalho);
                if (documentacaoLote.Usuario != null)
                    serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Documentos/DocumentacaoAFRMM", Localization.Resources.Zeus.DocumentacaoAFRMM.NaoFoiPossivelGerarDocumentacao, IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                repEnvioDocumentacaoAFRMM.Atualizar(documentacaoLote);
            }

            foreach (Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus falhaRetorno in listaRetornoFalha)
                if (!falhaRetorno.Sucesso && falhaRetorno.TipoRetorno != TipoRetornoGerarPDFZeus.Nenhum)
                    EnviarEmailFalhaIndividual(documentacaoLote, falhaRetorno, unidadeTrabalho);

            erro = string.Empty;
            unitOfWorkAdmin.Dispose();
            return true;
        }

        private static void EnviarEmailFalhaIndividual(Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote, DocumentoRetornoPDFZeus falhaRetorno, UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracaoDocumentacaoAFRMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unidadeTrabalho);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracaoDocumentacao = repConfiguracaoDocumentacaoAFRMM.BuscarConfiguracaoPadrao();

            try
            {
                string assunto = "Problemas no envio da Documentação AFRMM";
                string corpoEmail = "Não foi possível enviar a documentação AFRMM: <br/><br/>";
                corpoEmail += "Motivo: não foi possível gerar todos os PDFs, favor verifique as configurações. <br/><br/>";
                corpoEmail += "Viagem: " + (falhaRetorno.CTe?.Viagem?.Descricao ?? "") + " <br/>";
                corpoEmail += "Porto Destino: " + (falhaRetorno.CTe?.PortoDestino?.Descricao ?? "") + " <br/>";
                corpoEmail += "Número do Manifesto: " + (falhaRetorno.CTe?.NumeroManifesto ?? "") + " <br/>";
                corpoEmail += "Número do CE: " + (falhaRetorno.CTe?.NumeroCEMercante ?? "") + " <br/>";
                corpoEmail += "Número do Controle: " + (falhaRetorno.CTe?.NumeroControle ?? "") + " <br/>";
                corpoEmail += "Chave do CTe: " + (falhaRetorno.CTe?.Chave ?? "") + " <br/>";
                corpoEmail += "Chave DANFes: " + (falhaRetorno.CTe?.Documentos.FirstOrDefault()?.ChaveNFE ?? "") + " <br/><br/><br/>";
                corpoEmail += "Nome do Usuário: " + (documentacaoLote.Usuario?.Nome ?? "Sistema") + " <br/>";
                corpoEmail += "Tentativa " + documentacaoLote.QuantidadeTentativaEnvio.ToString() + " de 3 <br/><br/>";
                corpoEmail += "Após a última tentativa deverá realizar o envio manualmente pela tela de Documentação AFRMM <br/><br/>";

                List<string> emailsEnvio = new List<string>();
                if (configuracaoDocumentacao != null && !string.IsNullOrWhiteSpace(configuracaoDocumentacao.EmailFalhaEnvio))
                    emailsEnvio.AddRange(configuracaoDocumentacao.EmailFalhaEnvio.Split(';').ToList());

                if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha envio e-mail individual: " + ex, "EnvioDocumentacaoAFRMM");
            }
        }

        private static void EnviarEmailProblemaEnvioAutomatico(string erro, Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracaoDocumentacaoAFRMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracaoDocumentacao = repConfiguracaoDocumentacaoAFRMM.BuscarConfiguracaoPadrao();
            try
            {
                string assunto = "Problemas no envio da Documentação AFRMM";
                string corpoEmail = "Não foi possível enviar a documentação AFRMM: <br/>";
                corpoEmail += "Motivo: " + erro + " <br/>";
                corpoEmail += "Viagem: " + (documentacaoLote.PedidoViagemNavio?.Descricao ?? "") + " <br/>";
                corpoEmail += "Porto Destino: " + (documentacaoLote.PortoDestino?.Descricao ?? "") + " <br/><br/><br/>";
                corpoEmail += "Tentativa " + documentacaoLote.QuantidadeTentativaEnvio.ToString() + " de 3 <br/>";
                corpoEmail += "Após a última tentativa deverá realizar o envio manualmente pela tela de Documentação AFRMM <br/><br/>";

                List<string> emailsEnvio = new List<string>();
                if (configuracaoDocumentacao != null && !string.IsNullOrWhiteSpace(configuracaoDocumentacao.EmailFalhaEnvio))
                    emailsEnvio.AddRange(configuracaoDocumentacao.EmailFalhaEnvio.Split(';').ToList());
                if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnvioDocumentacaoAFRMM");
            }
        }

        private static List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> EnviarEmailDocumentacaoAFRMM(Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM documentacaoLote, string urlAcesso, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emailsEnvio, int codigoUsuario, string stringConexao, string caminhoArquivos, string diretorioDocumentosFiscais, string caminhoRelatorios, Repositorio.UnitOfWork unitOfWork, string caminhoArquivosAnexos, bool enviarParaFTP)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracaoDocumentacaoAFRMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            List<int> codigosCTesEnvio = new List<int>();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracaoDocumentacao = repConfiguracaoDocumentacaoAFRMM.BuscarConfiguracaoPadrao();

            if (enviarParaFTP)
            {
                if (configuracaoDocumentacao == null || string.IsNullOrEmpty(configuracaoDocumentacao.DiretorioFTP) || string.IsNullOrEmpty(configuracaoDocumentacao.DiretorioFTPRedespacho) || string.IsNullOrEmpty(configuracaoDocumentacao.DiretorioFTPSubcontratacao))
                    return new List<DocumentoRetornoPDFZeus>() { new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = TipoRetornoGerarPDFZeus.Nenhum } };
            }

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null;
            if (documentacaoLote.Usuario != null)
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Usuario = documentacaoLote.Usuario
                };
            }
            else
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Empresa = documentacaoLote.PortoDestino?.Empresa ?? null
                };
            }

            string assuntoEmail = "Envio de Documentação AFRMM Nº CE " + cte.NumeroCEMercante;

            string corpoEmail = "Segue em anexo a documentação referente ao AFRMM<br/>";
            corpoEmail += "Nº CE: " + cte.NumeroCEMercante + "<br/>";
            corpoEmail += "Nº Controle: " + cte.NumeroControle + "<br/>";
            corpoEmail += "Chave CT-e: " + cte.Chave + "<br/>";

            if (cte != null && (!string.IsNullOrWhiteSpace(emailsEnvio) || enviarParaFTP))
            {
                if (documentacaoLote.EnvioAutomatico && cte.DocumentacaoAFRMMEnviadaAutomatica == true)
                    return new List<DocumentoRetornoPDFZeus>() { new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = true, TipoRetorno = TipoRetornoGerarPDFZeus.Nenhum } };

                string msgAuditoria = "Envio da documentação de AFRMM.";
                if (!enviarParaFTP)
                    msgAuditoria += " E-mail(s): " + emailsEnvio;

                if (documentacaoLote.EnvioAutomatico)
                    msgAuditoria = "Envio Automático " + msgAuditoria;
                else if (documentacaoLote.Usuario != null)
                    msgAuditoria = "Operador: " + documentacaoLote.Usuario.Nome + " " + msgAuditoria;

                string nomeArquivo = "";
                codigosCTesEnvio.Add(cte.Codigo);

                string diretorioFTP = "";
                if (enviarParaFTP && configuracaoDocumentacao != null)
                {
                    if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                        diretorioFTP = configuracaoDocumentacao.DiretorioFTPSubcontratacao;
                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || cte.SVMTerceiro)
                        diretorioFTP = configuracaoDocumentacao.DiretorioFTPRedespacho;
                    else
                        diretorioFTP = configuracaoDocumentacao.DiretorioFTP;
                }

                List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> retornoEnviarEmail = Zeus.EnviarEmailPDFTodosDocumentos(auditado, out string msgRetorno, urlAcesso, FormaEnvioDocumentacao.Padrao, codigosCTesEnvio, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, emailsEnvio, assuntoEmail, corpoEmail, nomeArquivo, caminhoArquivosAnexos, unitOfWork, true, enviarParaFTP, configuracaoDocumentacao, diretorioFTP, cte);
                bool retorno = retornoEnviarEmail.Any(obj => obj.Sucesso);

                if (!string.IsNullOrWhiteSpace(msgRetorno))
                {
                    if (documentacaoLote.EnvioAutomatico)
                        msgRetorno = "Envio Automático " + msgRetorno;
                    else if (documentacaoLote.Usuario != null)
                        msgRetorno = "Operador: " + documentacaoLote.Usuario.Nome + " " + msgRetorno;
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgRetorno, unitOfWork);
                cte.DataUltimoEnvioDocumentacaoAFRMM = DateTime.Now;
                cte.UltimoEmailEnvioDocumentacaoAFRMM = retorno && !enviarParaFTP ? emailsEnvio : msgRetorno;
                cte.DocumentacaoAFRMMEnviada = retorno;
                cte.DocumentacaoAFRMMEnviadaAutomatica = documentacaoLote.EnvioAutomatico ? retorno : false;
                cte.SituacaoEnvioDocumentacaAFRMM = documentacaoLote.EnvioAutomatico ? retorno == true ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Enviado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Falha : enviarParaFTP && retorno == true ? SituacaoEnvioDocumentacao.Enviado : retorno == false ? SituacaoEnvioDocumentacao.Falha : cte.SituacaoEnvioDocumentacaAFRMM;
                cte.SituacaoEnvioDocumentacaAFRMMEmail = !enviarParaFTP && retorno == true ? SituacaoEnvioDocumentacao.Enviado : !enviarParaFTP && retorno == false ? SituacaoEnvioDocumentacao.Falha : SituacaoEnvioDocumentacao.NaoEnviado;

                repCTe.Atualizar(cte);

                return retornoEnviarEmail;
            }
            return new List<DocumentoRetornoPDFZeus>() { new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = TipoRetornoGerarPDFZeus.Nenhum } };
        }

        #endregion
    }
}