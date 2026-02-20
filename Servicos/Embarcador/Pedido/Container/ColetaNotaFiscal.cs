using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Pedido
{
    public class ColetaNotaFiscal : ServicoBase
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos

        #region Construtores

        public ColetaNotaFiscal(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ProcessarColetaNotaFiscal()
        {
            try
            {
                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaNotaFiscal repColetaNotaFiscal = new Repositorio.Embarcador.Pedidos.ColetaNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoConfiguracaoGeral.BuscarConfiguracaoPadrao();

                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal> coletaNotaFiscals = repColetaNotaFiscal.BuscarColetaNotaFiscalPendenteVinculo(configuracaoGeral.HabilitarFuncionalidadesProjetoGollum);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;

                for (int i = 0; i < coletaNotaFiscals.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal coletaNotaFiscal = repColetaNotaFiscal.BuscarPorCodigo(coletaNotaFiscals[i].CodigoColetaNotaFiscal);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(coletaNotaFiscals[i].CodigoCarga);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(coletaNotaFiscals[i].CodigoCargaPedido);

                    foreach (var xmlNotaFiscal in coletaNotaFiscal.Notas)
                    {
                        _unitOfWork.Start();

                        bool msgAlertaObservacao = false;
                        bool notaFiscalEmOutraCarga = false;
                        string retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);

                        if (string.IsNullOrEmpty(retorno) || msgAlertaObservacao)
                        {
                            xmlNotaFiscal.SemCarga = false;

                            if (xmlNotaFiscal.Codigo == 0)
                            {
                                xmlNotaFiscal.DataRecebimento = DateTime.Now;

                                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                            }
                            else
                                repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Observacao))
                            {
                                string observacaoNfe = xmlNotaFiscal.Observacao;

                                new Servicos.Embarcador.Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, observacaoNfe, _tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, xmlNotaFiscal, auditado);
                            }

                            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                                repCarga.Atualizar(cargaPedido.Carga);

                            int posLacre = 0;
                            foreach (var lacre in coletaNotaFiscal.Lacres)
                            {
                                if (posLacre == 0)
                                    cargaPedido.Pedido.LacreContainerUm = lacre;
                                if (posLacre == 1)
                                    cargaPedido.Pedido.LacreContainerDois = lacre;
                                if (posLacre == 2)
                                    cargaPedido.Pedido.LacreContainerTres = lacre;
                                posLacre++;
                            }
                            if (coletaNotaFiscal.TaraContainer > 0)
                                cargaPedido.Pedido.TaraContainer = Utilidades.String.OnlyNumbers(coletaNotaFiscal.TaraContainer.ToString("n0"));

                            if (cargaPedido.Pedido.Container != null && cargaPedido.Pedido.Container.ContainerTipo == null && cargaPedido.Pedido.ContainerTipoReserva != null)
                            {
                                cargaPedido.Pedido.Container.ContainerTipo = cargaPedido.Pedido.ContainerTipoReserva;
                                repContainer.Atualizar(cargaPedido.Pedido.Container);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Pedido.Container, null, "Informou o tipo de container do pedido via app de coleta.", _unitOfWork);
                            }

                            repPedido.Atualizar(cargaPedido.Pedido);

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, auditado);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Adicionado nota fiscal (" + xmlNotaFiscal.Chave + ") via app de coleta.", _unitOfWork);
                            coletaNotaFiscal.ColetaProcessada = true;

                            VerificarAvancoCarga(carga);
                        }
                        else
                        {
                            coletaNotaFiscal.MensagemRetorno = retorno;
                            coletaNotaFiscal.ColetaProcessada = true;
                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Tentativa de adicionado nota fiscal (" + xmlNotaFiscal.Chave + ") via app de coleta, retorno da rejeição: " + retorno, _unitOfWork);
                        }

                        if (xmlNotaFiscal?.DocumentoRecebidoViaFTP ?? false)
                        {
                            carga.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                            xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                            repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                        }
                        else
                            carga.FormaIntegracao = FormaIntegracao.OKColeta;

                        repCarga.Atualizar(carga);
                        repColetaNotaFiscal.Atualizar(coletaNotaFiscal);

                        _unitOfWork.CommitChanges();
                    }
                }

                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal> coletaNotaFiscalsSemContainer = repColetaNotaFiscal.BuscarColetaNotaFiscalPendenteVinculoSemContainer();
                for (int i = 0; i < coletaNotaFiscalsSemContainer.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal coletaNotaFiscal = repColetaNotaFiscal.BuscarPorCodigo(coletaNotaFiscalsSemContainer[i].CodigoColetaNotaFiscal);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(coletaNotaFiscalsSemContainer[i].CodigoCarga);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(coletaNotaFiscalsSemContainer[i].CodigoCargaPedido);

                    foreach (var xmlNotaFiscal in coletaNotaFiscal.Notas)
                    {
                        _unitOfWork.Start();

                        bool msgAlertaObservacao = false;
                        bool notaFiscalEmOutraCarga = false;
                        string retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);

                        if (string.IsNullOrEmpty(retorno) || msgAlertaObservacao)
                        {
                            xmlNotaFiscal.SemCarga = false;

                            if (xmlNotaFiscal.Codigo == 0)
                            {
                                xmlNotaFiscal.DataRecebimento = DateTime.Now;

                                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                            }
                            else
                                repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Observacao))
                            {
                                string observacaoNfe = xmlNotaFiscal.Observacao;

                                new Servicos.Embarcador.Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, observacaoNfe, _tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, xmlNotaFiscal, auditado);
                            }

                            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                                repCarga.Atualizar(cargaPedido.Carga);

                            int posLacre = 0;
                            foreach (var lacre in coletaNotaFiscal.Lacres)
                            {
                                if (posLacre == 0)
                                    cargaPedido.Pedido.LacreContainerUm = lacre;
                                if (posLacre == 1)
                                    cargaPedido.Pedido.LacreContainerDois = lacre;
                                if (posLacre == 2)
                                    cargaPedido.Pedido.LacreContainerTres = lacre;
                                posLacre++;
                            }

                            if (coletaNotaFiscalsSemContainer[i].CodigoContainer > 0)
                                cargaPedido.Pedido.Container = repContainer.BuscarPorCodigo(coletaNotaFiscalsSemContainer[i].CodigoContainer);

                            if (coletaNotaFiscal.TaraContainer > 0)
                                cargaPedido.Pedido.TaraContainer = Utilidades.String.OnlyNumbers(coletaNotaFiscal.TaraContainer.ToString("n0"));

                            if (cargaPedido.Pedido.Container != null && cargaPedido.Pedido.Container.ContainerTipo == null && cargaPedido.Pedido.ContainerTipoReserva != null)
                            {
                                cargaPedido.Pedido.Container.ContainerTipo = cargaPedido.Pedido.ContainerTipoReserva;
                                repContainer.Atualizar(cargaPedido.Pedido.Container);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Pedido.Container, null, "Informou o tipo de container do pedido via app de coleta.", _unitOfWork);
                            }

                            repPedido.Atualizar(cargaPedido.Pedido);

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, auditado);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Adicionado nota fiscal (" + xmlNotaFiscal.Chave + ") via app de coleta.", _unitOfWork);
                            coletaNotaFiscal.ColetaProcessada = true;

                            VerificarAvancoCarga(carga);
                        }
                        else
                        {
                            coletaNotaFiscal.MensagemRetorno = retorno;
                            coletaNotaFiscal.ColetaProcessada = true;
                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Tentativa de adicionado nota fiscal (" + xmlNotaFiscal.Chave + ") via app de coleta, retorno da rejeição: " + retorno, _unitOfWork);
                        }

                        if (xmlNotaFiscal?.DocumentoRecebidoViaFTP ?? false)
                        {
                            carga.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                            xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                            repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                        }
                        else
                            carga.FormaIntegracao = FormaIntegracao.OKColeta;

                        repCarga.Atualizar(carga);
                        repColetaNotaFiscal.Atualizar(coletaNotaFiscal);

                        _unitOfWork.CommitChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private void VerificarAvancoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.TipoOperacao?.ConfiguracaoCarga?.AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS ?? false)
                return;

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            serCarga.ValidarEmissaoDocumentosCarga(carga.Codigo, _unitOfWork, _tipoServicoMultisoftware, "", 1, false);
            serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
        }

        #endregion
    }
}
