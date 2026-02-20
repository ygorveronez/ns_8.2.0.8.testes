using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaImportacaoEmbarcador : ServicoBase
    {
        public CargaImportacaoEmbarcador(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #region Métodos Globais

        public static bool ImportarCargasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            List<int> codigosTiposOperacoes = repTipoOperacao.BuscarCodigosTiposOperacoesIntegrarEmbarcador(tipoServicoMultisoftware);

            foreach (int codigoTipoOperacao in codigosTiposOperacoes)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                    if (!ImportarCargasEmbarcador(out mensagemErro, tipoOperacao, unitOfWork))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        public static bool ImportarCTesCargasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            List<long> codigosCargas = repCargaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaCTes, 100);

            foreach (long codigoCarga in codigosCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador carga = repCargaIntegracaoEmbarcador.BuscarPorCodigo(codigoCarga, false);

                try
                {
                    if (!ImportarCTesCargaEmbarcador(out mensagemErro, carga, tipoServicoMultisoftware, unitOfWork, configuracaoTMS))
                        Servicos.Log.TratarErro($"Falha ao importar CT-e da carga (código {carga.Codigo} - número {carga.NumeroCarga}): {mensagemErro}");
                }
                catch (ServicoException ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);

                    carga.Mensagem = ex.Message;
                    carga.Situacao = SituacaoCargaIntegracaoEmbarcador.Problemas;

                    repCargaIntegracaoEmbarcador.Atualizar(carga);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        public async Task<bool> ImportarMDFesCargasEmbarcadorAsync()
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repositorioCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(_unitOfWork, _cancellationToken);

            List<long> codigosCargas = repositorioCargaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaMDFes, 100);

            foreach (long codigoCarga in codigosCargas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador carga = await repositorioCargaIntegracaoEmbarcador.BuscarPorCodigoAsync(codigoCarga, false);

                    string mensagemErro = await ImportarMDFesCargaEmbarcadorAsync(carga);

                    if (!string.IsNullOrEmpty(mensagemErro))
                        Servicos.Log.TratarErro($"Falha ao importar MDF-e da carga (código {carga.Codigo} - número {carga.NumeroCarga}): {mensagemErro}");
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    Servicos.Log.TratarErro(ex);
                }

                _unitOfWork.FlushAndClear();
            }

            return true;
        }

        public static bool GerarCargasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            List<long> codigosCargas = repCargaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgGeracaoCarga, 100);

            foreach (long codigoCarga in codigosCargas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador carga = repCargaIntegracaoEmbarcador.BuscarPorCodigo(codigoCarga, false);

                    if (!GerarCargaEmbarcador(out mensagemErro, carga, tipoServicoMultisoftware, unitOfWork))
                        Servicos.Log.TratarErro($"Falha ao gerar a carga (código {carga.Codigo} - número {carga.NumeroCarga}): {mensagemErro}");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        public static bool ImportarCancelamentosEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            List<int> codigosTiposOperacoes = repTipoOperacao.BuscarCodigosTiposOperacoesIntegrarEmbarcador(tipoServicoMultisoftware);

            foreach (int codigoTipoOperacao in codigosTiposOperacoes)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                    if (!ImportarCancelamentosEmbarcador(out mensagemErro, tipoOperacao, unitOfWork))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        public static bool ImportarCTesCanceladosEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            List<long> codigosCargas = repCargaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados, 100);

            foreach (long codigoCarga in codigosCargas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador carga = repCargaIntegracaoEmbarcador.BuscarPorCodigo(codigoCarga, false);

                    if (!ImportarCTesCanceladosEmbarcador(out mensagemErro, carga, tipoServicoMultisoftware, unitOfWork))
                        Servicos.Log.TratarErro($"Falha ao importar CT-e cancelado da carga (código {carga.Codigo} - número {carga.NumeroCarga}): {mensagemErro}");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        public static bool GerarCancelamentosEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            List<long> codigosCargas = repCargaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgGeracaoCancelamento, 100);

            foreach (long codigoCarga in codigosCargas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador carga = repCargaIntegracaoEmbarcador.BuscarPorCodigo(codigoCarga, false);

                    if (!GerarCancelamentoEmbarcador(out mensagemErro, carga, tipoServicoMultisoftware, unitOfWork))
                        Servicos.Log.TratarErro($"Falha ao gerar o cancelamento (código {carga.Codigo} - número {carga.NumeroCarga}): {mensagemErro}");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        #endregion

        #region Métodos Privados 

        private static bool VincularDocumentosEmCargaExistente(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            mensagemErro = null;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.IntegracaoCargaMultiEmbarcador,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(unitOfWork);

            if (cargaIntegracaoEmbarcador.CTes.Count <= 0)
            {
                cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Problemas;
                cargaIntegracaoEmbarcador.Mensagem = $"Nenhum documento fiscal foi encontrado para esta carga do embarcador.";

                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                unitOfWork.CommitChanges();

                return true;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais = repCargaIntegracaoEmbarcadorPedidoNotaFiscal.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

            if (string.IsNullOrWhiteSpace(cargaIntegracaoEmbarcador.TipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador))
            {
                cargaPedido = repCargaPedido.BuscarPrimeiroPorGrupoPessoasEVeiculo(cargaIntegracaoEmbarcador.TipoOperacao.GrupoPessoas, cargaIntegracaoEmbarcador.Veiculo, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                if (cargaPedido == null)
                {
                    cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Problemas;
                    cargaIntegracaoEmbarcador.Mensagem = $"Não foi encontrado um pedido compatível com o grupo de pessoas {cargaIntegracaoEmbarcador.TipoOperacao.GrupoPessoas.Descricao} e tração {cargaIntegracaoEmbarcador.Veiculo.Placa} para vincular os documentos da carga do embarcador {cargaIntegracaoEmbarcador.NumeroCarga}.";

                    repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                    return true;
                }
            }

            int countDocumentosVinculados = 0,
                countMDFesVinculados = 0;

            unitOfWork.Start();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in cargaIntegracaoEmbarcador.CTes)
            {
                if (cargaPedido == null)
                {
                    if (!string.IsNullOrWhiteSpace(cargaIntegracaoEmbarcador.TipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador) &&
                        !string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
                    {
                        Regex pattern = new Regex(cargaIntegracaoEmbarcador.TipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador, RegexOptions.IgnoreCase);

                        Match match = pattern.Match(cte.ObservacoesGerais);

                        cte.NumeroPedido = match.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(cte.NumeroPedido))
                    {
                        if (cargaIntegracaoEmbarcador?.TipoOperacao.GrupoPessoas != null)
                            cargaPedido = repCargaPedido.BuscarPrimeiroPorNumeroPedidoEmbarcadorEGrupoPessoas(cte.NumeroPedido, cargaIntegracaoEmbarcador.TipoOperacao.GrupoPessoas, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);
                        else
                            cargaPedido = repCargaPedido.BuscarPrimeiroPorNumeroPedidoEmbarcador(cte.NumeroPedido, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);
                    }

                    if (cargaPedido == null)
                    {
                        cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Problemas;
                        cargaIntegracaoEmbarcador.Mensagem = $"Não foi encontrado um pedido com o número de pedido do embarcador '{cte.NumeroPedido}' para vincular os documentos da carga do embarcador '{cargaIntegracaoEmbarcador.NumeroCarga}'.";

                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                        unitOfWork.CommitChanges();

                        return true;
                    }
                }

                if (repCargaPedidoDocumentoCTe.ExistePorCTe(cte.Codigo) ||
                    repCargaCTe.ExisteAutorizadoPorCTe(cte.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                {
                    CargaPedido = cargaPedido,
                    CTe = cte
                };

                repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedidoDocumentoCTe.CargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga pela integração com o embarcador.", unitOfWork);

                if (cte.CentroResultadoFaturamento == null)
                    cte.CentroResultadoFaturamento = cargaPedidoDocumentoCTe.CargaPedido.Pedido.CentroResultado;

                if (!cte.PossuiPedidoSubstituicao)
                    cte.PossuiPedidoSubstituicao = cargaPedidoDocumentoCTe.CargaPedido.Pedido.Substituicao ?? false;

                cte.CTeSemCarga = false;

                repCTe.Atualizar(cte);

                countDocumentosVinculados++;
            }

            foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in cargaIntegracaoEmbarcador.MDFes)
            {
                if (repCargaPedidoDocumentoMDFe.ExistePorMDFe(mdfe.Codigo) ||
                    repCargaMDFe.ExisteAutorizadoPorMDFe(mdfe.Codigo))
                    continue;

                if (cargaPedido == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe()
                {
                    CargaPedido = cargaPedido,
                    MDFe = mdfe
                };

                repCargaPedidoDocumentoMDFe.Inserir(cargaPedidoDocumentoMDFe, auditado);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedidoDocumentoMDFe.CargaPedido.Carga, $"Vinculou o MDF-e {mdfe.Descricao} à carga pela integração com o embarcador.", unitOfWork);

                mdfe.MDFeSemCarga = false;

                repMDFe.Atualizar(mdfe);

                countMDFesVinculados++;
            }

            if (!AvancarCargaAutomaticamente(out mensagemErro, cargaIntegracaoEmbarcador, cargaIntegracaoEmbarcadorPedidoNotasFiscais, cargaPedido, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoPedido))
                return false;

            cargaIntegracaoEmbarcador.Carga = cargaPedido?.Carga;

            if (cargaIntegracaoEmbarcador.Cancelamento)
                cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados;
            else
                cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Finalizado;

            cargaIntegracaoEmbarcador.Mensagem = "Documentos vinculados à carga.";

            if (cargaIntegracaoEmbarcador.CTes.Count > 0)
                cargaIntegracaoEmbarcador.Mensagem += $" {countDocumentosVinculados}/{cargaIntegracaoEmbarcador.CTes.Count} documentos.";

            if (cargaIntegracaoEmbarcador.MDFes.Count > 0)
                cargaIntegracaoEmbarcador.Mensagem += $" {countMDFesVinculados}/{cargaIntegracaoEmbarcador.MDFes.Count} MDF-es.";

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            unitOfWork.CommitChanges();

            return true;
        }

        private static bool GerarCargaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga();
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                ValidarVincularValePedagioCargaEmbarcador(cargaIntegracaoEmbarcador, unitOfWork);
            }

            if (cargaIntegracaoEmbarcador.CTes.Count == 0)
            {
                cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Finalizado;
                cargaIntegracaoEmbarcador.Mensagem = "Finalizado pois não foram importados documentos para esta carga.";

                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                return true;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            if (cargaIntegracaoEmbarcador.TipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador ?? false)
                return VincularDocumentosEmCargaExistente(out mensagemErro, cargaIntegracaoEmbarcador, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoPedido);

            IEnumerable<int> codigosCTes = cargaIntegracaoEmbarcador.CTes.Select(o => o.Codigo);

            if (repCargaCTe.ExisteAutorizadoPorCTe(codigosCTes) ||
                repCargaPedidoDocumentoCTe.ExistePorCTe(codigosCTes))
            {
                cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Problemas;
                cargaIntegracaoEmbarcador.Mensagem = "Não é possível gerar a carga pois os documentos já estão vinculados em uma carga.";

                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                mensagemErro = cargaIntegracaoEmbarcador.Mensagem;
                return false;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais = repCargaIntegracaoEmbarcadorPedidoNotaFiscal.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterCargaEmCargaIntegracao(cargaIntegracaoEmbarcador, unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            StringBuilder erro = new StringBuilder();

            Dominio.Entidades.Embarcador.Filiais.Filial filial = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cargaIntegracao.TransportadoraEmitente != null ? repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.TransportadoraEmitente.CNPJ) : repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref erro, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, true, null, configuracaoTMS);

            if (erro.Length == 0 || protocoloPedidoExistente > 0)
            {
                serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref erro, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref erro, ref codigoCargaExistente, unitOfWork, tipoServicoMultisoftware, false, true, null, configuracaoTMS, null, "", filial, tipoOperacao);

                if (cargaPedido != null)
                {
                    serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref erro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                    if (cargaIntegracao.FecharCargaAutomaticamente)
                        cargaPedido.Carga.CargaFechada = true;

                    cargaPedido.Pedido.NumeroPedidoEmbarcador = cargaIntegracaoEmbarcador.NumeroCarga;

                    int.TryParse(cargaIntegracao.NumeroCarga, out int sequecia);
                    cargaPedido.Carga.NumeroSequenciaCarga = sequecia;

                    cargaPedido.Carga.Empresa = cargaIntegracaoEmbarcador.Empresa;
                    cargaPedido.Carga.TipoDeCarga = cargaIntegracaoEmbarcador.TipoCarga ?? cargaIntegracaoEmbarcador.TipoOperacao.TipoDeCargaPadraoOperacao;
                    cargaPedido.Carga.TipoOperacao = cargaIntegracaoEmbarcador.TipoOperacao;
                    cargaPedido.Carga.ModeloVeicularCarga = cargaIntegracaoEmbarcador.VeiculosVinculados.Where(o => o.ModeloVeicularCarga != null).Select(o => o.ModeloVeicularCarga).FirstOrDefault() ?? cargaIntegracaoEmbarcador.Veiculo?.ModeloVeicularCarga;
                    cargaPedido.Carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                    cargaPedido.Carga.Veiculo = cargaIntegracaoEmbarcador.Veiculo;
                    cargaPedido.Carga.VeiculosVinculados = cargaIntegracaoEmbarcador.VeiculosVinculados.ToList();
                    cargaPedido.Carga.Motoristas = cargaIntegracaoEmbarcador.Motoristas.ToList();

                    if (cargaPedido.Carga.Veiculo?.Tipo == "T")
                        cargaPedido.Carga.FreteDeTerceiro = true;
                    else if (cargaPedido.Carga.ProvedorOS != null)
                        cargaPedido.Carga.FreteDeTerceiro = true;

                    if (cargaPedido.Pedido.CentroResultado == null)
                        cargaPedido.Pedido.CentroResultado = cargaPedido.Carga.Veiculo?.CentroResultado;

                    cargaPedido.CTeEmitidoNoEmbarcador = true;
                    cargaPedido.Peso = cargaIntegracao.PesoBruto;
                    cargaPedido.PesoLiquido = cargaIntegracao.PesoLiquido;

                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in cargaIntegracaoEmbarcador.CTes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                        if (cte.CentroResultadoFaturamento == null)
                            cte.CentroResultadoFaturamento = cargaPedido.Pedido.CentroResultado;

                        if (!cte.PossuiPedidoSubstituicao)
                            cte.PossuiPedidoSubstituicao = cargaPedido.Pedido.Substituicao ?? false;

                        cte.CTeSemCarga = false;

                        repCTe.Atualizar(cte);
                    }

                    Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                    Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                    Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> cargasIntegracaoEmbarcadorValePedagioIntegracao = repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaPedagioIntegracao = null;
                    if (cargaIntegracaoEmbarcador.Carga != null)
                        cargaPedagioIntegracao = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(cargaIntegracaoEmbarcador.Carga.Codigo);

                    foreach (var cargaIntegracaoEmbarcadorValePedagioIntegracao in cargasIntegracaoEmbarcadorValePedagioIntegracao)
                    {
                        if (cargaPedagioIntegracao != null && cargaPedagioIntegracao.Any()
                           && cargaPedagioIntegracao.Where(o => o.NumeroValePedagio == cargaIntegracaoEmbarcadorValePedagioIntegracao.NumeroValePedagio).Any())
                        {
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                        cargaIntegracaoValePedagio.Carga = cargaPedido.Carga;
                        cargaIntegracaoValePedagio.TipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);
                        cargaIntegracaoValePedagio.ProblemaIntegracao = "Importado do Embarcador";
                        cargaIntegracaoValePedagio.DataIntegracao = DateTime.Now;
                        cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaIntegracaoValePedagio.PedagioIntegradoEmbarcador = true;
                        cargaIntegracaoValePedagio.RotaFrete = null;

                        cargaIntegracaoValePedagio.SituacaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.SituacaoValePedagio;
                        cargaIntegracaoValePedagio.NumeroValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.NumeroValePedagio;
                        cargaIntegracaoValePedagio.IdCompraValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.IdCompraValePedagio;
                        cargaIntegracaoValePedagio.ValorValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.ValorValePedagio;
                        cargaIntegracaoValePedagio.Observacao1 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao1;
                        cargaIntegracaoValePedagio.Observacao2 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao2;
                        cargaIntegracaoValePedagio.Observacao3 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao3;
                        cargaIntegracaoValePedagio.Observacao4 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao4;
                        cargaIntegracaoValePedagio.Observacao5 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao5;
                        cargaIntegracaoValePedagio.Observacao6 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao6;
                        cargaIntegracaoValePedagio.RotaTemporaria = cargaIntegracaoEmbarcadorValePedagioIntegracao.RotaTemporaria;
                        cargaIntegracaoValePedagio.CodigoIntegracaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoIntegracaoValePedagio;
                        cargaIntegracaoValePedagio.TipoRota = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoRota;
                        cargaIntegracaoValePedagio.TipoCompra = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoCompra;
                        cargaIntegracaoValePedagio.CompraComEixosSuspensos = cargaIntegracaoEmbarcadorValePedagioIntegracao.CompraComEixosSuspensos;
                        cargaIntegracaoValePedagio.CodigoRoteiro = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoRoteiro;
                        cargaIntegracaoValePedagio.CodigoPercurso = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoPercurso;
                        cargaIntegracaoValePedagio.QuantidadeEixos = cargaIntegracaoEmbarcadorValePedagioIntegracao.QuantidadeEixos;
                        cargaIntegracaoValePedagio.RecebidoPorIntegracao = cargaIntegracaoEmbarcadorValePedagioIntegracao.RecebidoPorIntegracao;
                        cargaIntegracaoValePedagio.ValidaCompraRemoveuComponentes = cargaIntegracaoEmbarcadorValePedagioIntegracao.ValidaCompraRemoveuComponentes;
                        cargaIntegracaoValePedagio.NomeTransportador = cargaIntegracaoEmbarcadorValePedagioIntegracao.NomeTransportador;
                        cargaIntegracaoValePedagio.TipoPercursoVP = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoPercursoVP;
                        cargaIntegracaoValePedagio.CnpjMeioPagamento = cargaIntegracaoEmbarcadorValePedagioIntegracao.CnpjMeioPagamento;
                        repositorioCargaIntegracaoValePedagio.Inserir(cargaIntegracaoValePedagio);

                        cargaIntegracaoEmbarcadorValePedagioIntegracao.CargaIntegracaoValePedagio = cargaIntegracaoValePedagio;
                        repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.Atualizar(cargaIntegracaoEmbarcadorValePedagioIntegracao);
                    }

                    Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio repositorioCargaIntegracaoEmbarcadorValePedagio = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> cargasIntegracaoEmbarcadorValePedagio = repositorioCargaIntegracaoEmbarcadorValePedagio.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaPedagio = null;
                    if (cargaIntegracaoEmbarcador.Carga != null)
                        cargaPedagio = repositorioCargaValePedagio.BuscarPorCarga(cargaIntegracaoEmbarcador.Carga.Codigo);

                    foreach (var cargaIntegracaoEmbarcadorValePedagio in cargasIntegracaoEmbarcadorValePedagio)
                    {

                        if (cargaPedagio != null
                            && cargaPedagio.Any()
                            && cargaPedagio.Where(o => o.NumeroComprovante == cargaIntegracaoEmbarcadorValePedagio.NumeroComprovante).Any())
                        {
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();

                        if (cargaIntegracaoEmbarcadorValePedagio.CodigoIntegracaoValePedagioEmbarcador != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao cargaIntegracaoEmbarcadorValePedagioIntegracao = repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.BuscarPorCargaEmbarcadorCodigoIntegracaoValePedagioEmbarcador(cargaIntegracaoEmbarcadorValePedagio.CargaIntegracaoEmbarcador.Codigo, (int)cargaIntegracaoEmbarcadorValePedagio.CodigoIntegracaoValePedagioEmbarcador);

                            if (cargaIntegracaoEmbarcadorValePedagioIntegracao != null)
                                cargaValePedagio.CargaIntegracaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.CargaIntegracaoValePedagio;
                        }

                        cargaValePedagio.Carga = cargaPedido.Carga;
                        cargaValePedagio.Fornecedor = cargaIntegracaoEmbarcadorValePedagio.Fornecedor;
                        cargaValePedagio.Responsavel = cargaIntegracaoEmbarcadorValePedagio.Responsavel;
                        cargaValePedagio.NumeroComprovante = cargaIntegracaoEmbarcadorValePedagio.NumeroComprovante;
                        cargaValePedagio.CodigoAgendamentoPorto = cargaIntegracaoEmbarcadorValePedagio.CodigoAgendamentoPorto;
                        cargaValePedagio.Valor = cargaIntegracaoEmbarcadorValePedagio.Valor;
                        cargaValePedagio.TipoCompra = cargaIntegracaoEmbarcadorValePedagio.TipoCompra;
                        cargaValePedagio.QuantidadeEixos = cargaIntegracaoEmbarcadorValePedagio.QuantidadeEixos;
                        cargaValePedagio.NaoIncluirMDFe = cargaIntegracaoEmbarcadorValePedagio.NaoIncluirMDFe;

                        repositorioCargaValePedagio.Inserir(cargaValePedagio);
                    }

                    foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in cargaIntegracaoEmbarcador.MDFes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe()
                        {
                            CargaPedido = cargaPedido,
                            MDFe = mdfe
                        };

                        repCargaPedidoDocumentoMDFe.Inserir(cargaPedidoDocumentoMDFe);

                        mdfe.MDFeSemCarga = false;

                        repMDFe.Atualizar(mdfe);
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                    repCargaPedido.Atualizar(cargaPedido);
                    repCarga.Atualizar(cargaPedido.Carga);

                    if (!AvancarCargaAutomaticamente(out mensagemErro, cargaIntegracaoEmbarcador, cargaIntegracaoEmbarcadorPedidoNotasFiscais, cargaPedido, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoPedido))
                        return false;

                    cargaIntegracaoEmbarcador.Carga = cargaPedido.Carga;
                }
                else
                {
                    unitOfWork.Rollback();

                    mensagemErro = erro.ToString();
                    return false;
                }
            }
            else
            {
                unitOfWork.Rollback();

                mensagemErro = erro.ToString();
                return false;
            }

            if (cargaIntegracaoEmbarcador.Cancelamento)
                cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados;
            else
                cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Finalizado;

            cargaIntegracaoEmbarcador.Mensagem = "Carga gerada com sucesso.";

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            unitOfWork.CommitChanges();

            return true;
        }

        private static bool AvancarCargaAutomaticamente(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            bool cargaDeRetorno = cargaIntegracaoEmbarcador.CTes.Count > 0 && cargaIntegracaoEmbarcador.CTes.All(o => o.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento);

            mensagemErro = AvancarEtapaCargaAutomaticamente(cargaPedido.Carga, cargaPedido, cargaDeRetorno, cargaIntegracaoEmbarcador.Cancelamento, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoPedido, cargaIntegracaoEmbarcadorPedidoNotasFiscais);

            if (!string.IsNullOrEmpty(mensagemErro))
                return false;

            GerarDocumentosParaNFSManual(cargaIntegracaoEmbarcador, cargaPedido.Carga, cargaPedido, unitOfWork, tipoServicoMultisoftware);

            return true;
        }

        public static string AvancarEtapaCargaAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool cargaDeRetorno, bool cancelamento, ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais = null)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao svcCargaLocalPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);

            string mensagemErro = svcCTesImportados.CriarNotasFiscaisDaCarga(cargaPedido, tipoServicoMultisoftware, unitOfWork, true, cargaIntegracaoEmbarcadorPedidoNotasFiscais, configuracaoTMS);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                unitOfWork.Rollback();
                return mensagemErro;
            }

            carga.ValorICMS = cargaPedido.ValorICMS;
            carga.ValorFrete = cargaPedido.ValorFrete;
            carga.ValorFreteAPagar = cargaPedido.ValorFreteAPagar;
            carga.ValorFreteLiquido = carga.ValorFrete;
            carga.ValorFreteEmbarcador = cargaPedido.ValorFreteAPagar;
            carga.PossuiPendencia = false;

            Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, unitOfWork, "", tipoServicoMultisoftware, false);

            if (!cancelamento)
            {
                svcCargaLocalPrestacao.VerificarEAjustarLocaisPrestacao(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, unitOfWork, tipoServicoMultisoftware, configuracaoPedido, cargaDeRetorno);
                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                #region Adicionar Carga Entrega

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido };

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargasPedido, true, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                #endregion
            }

            svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

            if (!cancelamento && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, tipoServicoMultisoftware, "");
            }

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 40 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

                if (!(configuracaoTMS.BloquearEmissaoComContratoFreteZerado &&
                      carga.FreteDeTerceiro &&
                      (contratoFrete?.ValorFreteSubcontratacao ?? 0m) <= 0m) &&

                    repCargaComponenteFrete.ContarComponentesInvalidosPorCarga(carga.Codigo) <= 0)
                {
                    if (cancelamento)
                    {
                        carga.DataInicioEmissaoDocumentos = DateTime.Now;
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                        carga.DataInicioGeracaoCTes = DateTime.Now;
                        carga.SituacaoCarga = SituacaoCarga.PendeciaDocumentos;
                        carga.PossuiPendencia = true;

                        svcCTesImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork, false);
                    }
                    else
                    {
                        string retornoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                        if (string.IsNullOrWhiteSpace(retornoCIOT))
                        {
                            carga.DataInicioEmissaoDocumentos = DateTime.Now;
                            carga.DataEnvioUltimaNFe = DateTime.Now;
                            carga.problemaAverbacaoCTe = false;
                            carga.CTesEmDigitacao = false;
                            carga.DataInicioGeracaoCTes = DateTime.Now;
                        }
                        else
                            Servicos.Log.TratarErro($"Carga: {carga.CodigoCargaEmbarcador} - não foi possível gerar/vincular o CIOT - {retornoCIOT}");
                    }
                }
            }
            else
            {
                svcCTesImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork, false);

                carga.DataInicioEmissaoDocumentos = DateTime.Now;
                carga.DataEnvioUltimaNFe = DateTime.Now;
                carga.DataInicioGeracaoCTes = DateTime.Now;
                carga.SituacaoCarga = SituacaoCarga.EmTransporte;
                carga = servicoCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unitOfWork);
            }

            repCarga.Atualizar(carga);

            return mensagemErro;
        }

        private static void GerarDocumentosParaNFSManual(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido repCargaIntegracaoEmbarcadorPedido = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Pedido.NotaFiscal svcNotaFiscalPedido = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> pedidos = repCargaIntegracaoEmbarcadorPedido.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

            cargaPedido.PossuiNFSManual = pedidos.Exists(o => o.PossuiNFSManual);
            cargaPedido.PossuiCTe = pedidos.Exists(o => o.PossuiCTe);
            cargaPedido.PossuiNFS = pedidos.Exists(o => o.PossuiNFS);

            repCargaPedido.Atualizar(cargaPedido);

            //if (carga.DadosSumarizados == null)
            //    carga.DadosSumarizados = new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();

            //carga.DadosSumarizados.PossuiNFSManual = cargaPedido.PossuiNFSManual;
            //carga.DadosSumarizados.PossuiCTe = cargaPedido.PossuiCTe;
            //carga.DadosSumarizados.PossuiNFS = cargaPedido.PossuiNFS;

            //repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido pedido in pedidos)
            {
                if (!pedido.PossuiNFSManual) continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> notasFiscais = repCargaIntegracaoEmbarcadorPedidoNotaFiscal.BuscarPorCargaIntegracaoEmbarcadorPedido(pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal notaFiscal in notasFiscais)
                {
                    bool gerarNFSManual = false;

                    if ((pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada ||
                        pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos ||
                        pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual) && notaFiscal.PossuiNFSManual)
                        gerarNFSManual = true;
                    else if ((pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado ||
                              pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual ||
                              pedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado) && pedido.PossuiNFSManual)
                        gerarNFSManual = true;

                    if (!gerarNFSManual)
                        continue;

                    carga.AgNFSManual = true;

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal
                    {
                        TipoOperacaoNotaFiscal = notaFiscal.TipoOperacaoNotaFiscal,
                        Emitente = notaFiscal.Emitente,
                        Destinatario = notaFiscal.Destinatario,
                        XML = "",
                        CNPJTranposrtador = "",
                        PlacaVeiculoNotaFiscal = "",
                        Rota = "",
                        NaturezaOP = "",
                        Peso = notaFiscal.Peso,
                        PesoBaseParaCalculo = notaFiscal.Peso,
                        Descricao = "",
                        nfAtiva = true,
                        DataEmissao = notaFiscal.DataEmissao,
                        Numero = notaFiscal.Numero,
                        Serie = notaFiscal.Serie,
                        Chave = notaFiscal.Chave,
                        TipoEmissao = Utilidades.Chave.ObterTipoEmissao(notaFiscal.Chave).ToString().ToEnum<TipoEmissaoNotaFiscal>(),
                        ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido
                    };

                    if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                    else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;

                    if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                    {
                        xmlNotaFiscal.Modelo = "55";
                        xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                    }
                    else
                    {
                        xmlNotaFiscal.Modelo = "99";
                        xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;
                    }

                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = svcNotaFiscalPedido.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                    Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via carga importada do embarcador", unitOfWork);

                    pedidoXMLNotaFiscal.ValorFrete = notaFiscal.ValorFrete;

                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual()
                    {
                        Carga = carga,
                        CargaOrigem = carga,
                        Chave = xmlNotaFiscal.Chave,
                        DataEmissao = xmlNotaFiscal.DataEmissao,
                        Descricao = xmlNotaFiscal.Descricao,
                        Destinatario = xmlNotaFiscal.Destinatario,
                        LocalidadePrestacao = xmlNotaFiscal.Destinatario.Localidade ?? xmlNotaFiscal.Emitente.Localidade ?? cargaPedido.Destino,
                        Numero = xmlNotaFiscal.Numero,
                        Peso = xmlNotaFiscal.Peso,
                        PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                        Remetente = xmlNotaFiscal.Emitente,
                        Serie = xmlNotaFiscal.Serie,
                        ValorFrete = pedidoXMLNotaFiscal.ValorFrete,
                        ValorPrestacaoServico = pedidoXMLNotaFiscal.ValorFrete,
                        Tomador = xmlNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente,
                        ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo(xmlNotaFiscal.Modelo)
                    };

                    Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(documentoEmissaoNFSManual, repCargaDocumentoEmissaoNFSManual, unitOfWork);
                }
            }
        }

        private static Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterCargaEmCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido repCargaIntegracaoEmbarcadorPedido = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido pedido = repCargaIntegracaoEmbarcadorPedido.BuscarPrimeiroPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaIntegracaoEmbarcador.CTes.FirstOrDefault();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.NumeroPedidoEmbarcador = cargaIntegracaoEmbarcador.NumeroCarga;
            cargaIntegracao.NumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
            cargaIntegracao.CFOP = cte?.CFOP?.CodigoCFOP ?? 0;
            cargaIntegracao.TransportadoraEmitente = serEmpresa.ConverterObjetoEmpresa(cargaIntegracaoEmbarcador.Empresa);
            cargaIntegracao.UsarOutroEnderecoOrigem = false;
            cargaIntegracao.UsarOutroEnderecoDestino = false;
            cargaIntegracao.Remetente = serPessoa.ConverterObjetoPessoa(pedido.Remetente);
            cargaIntegracao.Destinatario = serPessoa.ConverterObjetoPessoa(pedido.Destinatario);
            cargaIntegracao.Tomador = serPessoa.ConverterObjetoPessoa(pedido.Tomador);
            cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(pedido.Recebedor);
            cargaIntegracao.Expedidor = serPessoa.ConverterObjetoPessoa(pedido.Expedidor);
            cargaIntegracao.DataCriacaoCarga = cargaIntegracaoEmbarcador.DataCriacaoCarga?.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.NumeroPaletes = 0;
            cargaIntegracao.PesoTotalPaletes = 0;
            cargaIntegracao.ValorTotalPaletes = 0;
            cargaIntegracao.PesoBruto = repInformacaoCargaCTe.ObterPesoKg(cte?.Codigo ?? 0);
            cargaIntegracao.CubagemTotal = 0;
            cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
            {
                CodigoProduto = "1",
                DescricaoProduto = !string.IsNullOrWhiteSpace(cte?.ProdutoPredominanteCTe) ? cte.ProdutoPredominanteCTe : "Diversos"
            };
            cargaIntegracao.TipoTomador = pedido.TipoTomador;
            cargaIntegracao.TipoPagamento = pedido.TipoPagamento;

            cargaIntegracao.FecharCargaAutomaticamente = true;

            return cargaIntegracao;
        }

        private static bool ImportarCTesCargaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            mensagemErro = null;

            if (cargaIntegracaoEmbarcador.CTes != null)
                cargaIntegracaoEmbarcador.CTes.Clear();
            else
                cargaIntegracaoEmbarcador.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (!ConsultarCTes(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
                return false;

            if (configuracaoTMS.GerarNFSeImportacaoEmbarcador &&
                !ConsultarNFSes(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
                return false;

            unitOfWork.Start();

            if (!configuracaoTMS.GerarNFSeImportacaoEmbarcador &&
                !GerarDocumentosMunicipais(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
            {
                unitOfWork.Rollback();
                return false;
            }

            cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaMDFes;

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            unitOfWork.CommitChanges();

            return true;
        }

        private static bool ConsultarNFSes(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.ServicoSGT.NFS.NFSClient svcNFSe = ObterClientNFSes(cargaIntegracaoEmbarcador.TipoOperacao.URLIntegracaoMultiEmbarcador, cargaIntegracaoEmbarcador.TipoOperacao.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            do
            {
                if (cargaIntegracaoEmbarcador.TipoOperacao.UtilizarGeracaoDeNFSeAvancada ?? false)
                {
                    if (!GerarNFSeCompleta(svcNFSe, out mensagemErro, ref limite, ref inicio, cargaIntegracaoEmbarcador, unitOfWork))
                        return false;
                }
                else
                {
                    if (!GerarNFSe(svcNFSe, out mensagemErro, ref limite, ref inicio, cargaIntegracaoEmbarcador, unitOfWork))
                        return false;
                }

                inicio += 50;
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool GerarNFSe(Servicos.ServicoSGT.NFS.NFSClient svcNFSe, out string mensagemErro, ref int limite, ref int inicio, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            ServicoSGT.NFS.RetornoOfPaginacaoOfNFSYRsas_SFX51p1vPsU retorno = svcNFSe.BuscarNFSsPorCarga(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, inicio, limite);

            if (!retorno.Status)
            {
                cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                mensagemErro = retorno.Mensagem;
                return false;
            }

            foreach (Dominio.ObjetosDeValor.WebService.NFS.NFS nfse in retorno.Objeto.Itens)
            {
                if (nfse?.NFSe == null || (nfse.NFSe.StatusNFSe != Dominio.Enumeradores.StatusNFSe.Autorizado && nfse.NFSe.StatusNFSe != Dominio.Enumeradores.StatusNFSe.Cancelado))
                    continue;

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

                if (empresa == null)
                {
                    mensagemErro = $"Emitente da NFS-e ({nfse.TransportadoraEmitente.CNPJ} - {nfse.TransportadoraEmitente.RazaoSocial}) não encontrado.";
                    return false;
                }

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = repCTe.BuscarNFSe(nfse.NFSe.Numero, nfse.NFSe.Serie, empresa.Codigo, nfse.NFSe.CodigoVerificacao);

                if (nfseGerada == null)
                    nfseGerada = GerarNFSe(nfse, empresa, unitOfWork);

                cargaIntegracaoEmbarcador.CTes.Add(nfseGerada);
            }

            mensagemErro = null;
            limite = retorno.Objeto.NumeroTotalDeRegistro;

            return true;
        }

        private static bool GerarNFSeCompleta(Servicos.ServicoSGT.NFS.NFSClient svcNFSe, out string mensagemErro, ref int limite, ref int inicio, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            ServicoSGT.NFS.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcNFSe.BuscarNFSsCompletasPorCarga(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, inicio, limite);

            if (!retorno.Status)
            {
                cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                mensagemErro = retorno.Mensagem;
                return false;
            }

            foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe nfse in retorno.Objeto.Itens)
            {
                if (nfse.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada && nfse.SituacaoCTeSefaz != SituacaoCTeSefaz.Cancelada)
                    continue;

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

                if (empresa == null)
                {
                    mensagemErro = $"Emitente da NFS-e ({nfse.TransportadoraEmitente.CNPJ} - {nfse.TransportadoraEmitente.RazaoSocial}) não encontrado.";
                    return false;
                }

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = repCTe.BuscarNFSe(nfse.Numero, nfse.Serie, empresa.Codigo, nfse.ProtocoloAutorizacao);

                if (nfseGerada == null)
                    nfseGerada = GerarNFSe(nfse, empresa, unitOfWork);

                cargaIntegracaoEmbarcador.CTes.Add(nfseGerada);
            }

            mensagemErro = null;
            limite = retorno.Objeto.NumeroTotalDeRegistro;

            return true;
        }

        private static Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSe(Dominio.ObjetosDeValor.WebService.NFS.NFS nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.NFS.NFS svcNFSe = new Servicos.WebService.NFS.NFS(unitOfWork);

            try
            {
                unitOfWork.Start();

                nfse.UltimoRetornoSEFAZ = "NFS-e importada automaticamente do embarcador";
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = svcNFSe.GerarNFSe(nfse, empresa, unitOfWork);

                unitOfWork.CommitChanges();
                return cte;
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private static Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSe(Dominio.ObjetosDeValor.WebService.CTe.CTe nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.NFS.NFS svcNFSe = new Servicos.WebService.NFS.NFS(unitOfWork);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = svcNFSe.GerarNFSe(nfse, empresa, unitOfWork);

                unitOfWork.CommitChanges();
                return cte;
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private static bool ConsultarCTes(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.ServicoSGT.CTe.CTeClient svcWSCTe = ObterClientCTes(cargaIntegracaoEmbarcador.TipoOperacao.URLIntegracaoMultiEmbarcador, cargaIntegracaoEmbarcador.TipoOperacao.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            do
            {
                ServicoSGT.CTe.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcWSCTe.BuscarCTesPorCarga(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, inicio, 50);

                if (!retorno.Status)
                {
                    cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                    repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in retorno.Objeto.Itens)
                {
                    if (cte.Modelo != "57" || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Inutilizada)
                        continue;

                    if (string.IsNullOrWhiteSpace(cte.XMLAutorizacao))
                    {
                        mensagemErro = $"O CT-e {cte.Numero} não possui um XML.";

                        cargaIntegracaoEmbarcador.Mensagem = mensagemErro;
                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
                        return false;
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteInserido = repCTe.BuscarPorChave(cte.Chave, cargaIntegracaoEmbarcador.Empresa.Codigo);

                    if (cteInserido == null)
                    {
                        System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.XMLAutorizacao);

                        object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, cargaIntegracaoEmbarcador.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false, tipoServicoMultisoftware, true);

                        if (retornoInserir.GetType() == typeof(string))
                        {
                            cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                            mensagemErro = (string)retornoInserir;
                            return false;
                        }

                        cteInserido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;
                    }

                    cargaIntegracaoEmbarcador.CTes.Add(cteInserido);
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool GerarDocumentosMunicipais(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Cliente serCliente = new Servicos.Cliente();
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido repCargaIntegracaoEmbarcadorPedido = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> cargaIntegracaoEmbarcadorPedidos = repCargaIntegracaoEmbarcadorPedido.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido cargaIntegracaoEmbarcadorPedido in cargaIntegracaoEmbarcadorPedidos)
            {
                if (!cargaIntegracaoEmbarcadorPedido.PossuiNFS)
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais = null;

                if (cargaIntegracaoEmbarcadorPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada ||
                    cargaIntegracaoEmbarcadorPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos ||
                    cargaIntegracaoEmbarcadorPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                    cargaIntegracaoEmbarcadorPedidoNotasFiscais = repCargaIntegracaoEmbarcadorPedidoNotaFiscal.BuscarPorCargaIntegracaoEmbarcadorPedidoComOperacaoMunicipal(cargaIntegracaoEmbarcadorPedido.Codigo);
                else
                    cargaIntegracaoEmbarcadorPedidoNotasFiscais = repCargaIntegracaoEmbarcadorPedidoNotaFiscal.BuscarPorCargaIntegracaoEmbarcadorPedido(cargaIntegracaoEmbarcadorPedido.Codigo);

                if (cargaIntegracaoEmbarcadorPedidoNotasFiscais.Count <= 0)
                    continue;

                if (cargaIntegracaoEmbarcador.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal == null)
                {
                    mensagemErro = "Não existe uma configuração de modelo de documento municipal (outros) para o tipo de operação " + cargaIntegracaoEmbarcador.TipoOperacao.Descricao + ".";
                    return false;
                }

                Dominio.Entidades.Localidade localidadePrestacao = cargaIntegracaoEmbarcadorPedido.Destinatario.Localidade;

                Dominio.ObjetosDeValor.CTe.CTe cteIntegrar = new Dominio.ObjetosDeValor.CTe.CTe()
                {
                    CodigoIBGECidadeInicioPrestacao = localidadePrestacao.CodigoIBGE,
                    CodigoIBGECidadeTerminoPrestacao = localidadePrestacao.CodigoIBGE,
                    DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Remetente = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcadorPedido.Remetente, null),
                    Expedidor = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcadorPedido.Expedidor, null),
                    Recebedor = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcadorPedido.Recebedor, null),
                    Destinatario = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcadorPedido.Destinatario, null),
                    Tomador = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcadorPedido.Tomador, null),
                    Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
                    {
                        CNPJ = cargaIntegracaoEmbarcador.Empresa.CNPJ,
                        Atualizar = false
                    },
                    Documentos = (from obj in cargaIntegracaoEmbarcadorPedidoNotasFiscais
                                  select new Dominio.ObjetosDeValor.CTe.Documento()
                                  {
                                      ChaveNFE = obj.Chave,
                                      DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                      Numero = obj.Numero.ToString(),
                                      ModeloDocumentoFiscal = "55",
                                      Peso = obj.Peso,
                                      Serie = obj.Serie,
                                      Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                                      Valor = obj.Valor
                                  }).ToList(),
                    TipoCTe = Dominio.Enumeradores.TipoCTE.Normal,
                    TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato,
                    TipoPagamento = cargaIntegracaoEmbarcadorPedido.TipoPagamento,
                    TipoServico = Dominio.Enumeradores.TipoServico.Normal,
                    ValorFrete = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.ValorFrete),
                    ValorAReceber = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.ValorFrete),
                    ValorTotalPrestacaoServico = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.ValorFrete),
                    ExibeICMSNaDACTE = true,
                    Motoristas = (from obj in cargaIntegracaoEmbarcador.Motoristas
                                  select new Dominio.ObjetosDeValor.CTe.Motorista()
                                  {
                                      CPF = obj.CPF,
                                      Nome = obj.Nome
                                  }).ToList(),
                    Peso = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.Peso),
                    ProdutoPredominante = "Diversos",
                    QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>()
                        {
                                new Dominio.ObjetosDeValor.CTe.QuantidadeCarga()
                                {
                                    Descricao = "Quilogramas",
                                    UnidadeMedida = "01",
                                    Quantidade = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.Peso)
                                }
                        },
                    TipoTomador = cargaIntegracaoEmbarcadorPedido.TipoTomador,
                    ValorTotalMercadoria = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Sum(o => o.Valor)
                };

                cteIntegrar.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();

                if (cargaIntegracaoEmbarcador.Veiculo != null)
                {
                    cteIntegrar.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo()
                    {
                        Placa = cargaIntegracaoEmbarcador.Veiculo.Placa,
                        CapacidadeKG = cargaIntegracaoEmbarcador.Veiculo.CapacidadeKG,
                        CapacidadeM3 = cargaIntegracaoEmbarcador.Veiculo.CapacidadeM3,
                        Chassi = cargaIntegracaoEmbarcador.Veiculo.Chassi,
                        Proprietario = serCliente.ObterClienteCTE(cargaIntegracaoEmbarcador.Veiculo.Proprietario, null),
                        Renavam = cargaIntegracaoEmbarcador.Veiculo.Renavam,
                        RNTRCProprietario = cargaIntegracaoEmbarcador.Veiculo.RNTRC,
                        Tara = cargaIntegracaoEmbarcador.Veiculo.Tara,
                        TipoCarroceria = cargaIntegracaoEmbarcador.Veiculo.TipoCarroceria,
                        TipoPropriedade = cargaIntegracaoEmbarcador.Veiculo.Tipo,
                        TipoProprietario = cargaIntegracaoEmbarcador.Veiculo.TipoProprietario,
                        TipoRodado = cargaIntegracaoEmbarcador.Veiculo.TipoRodado,
                        TipoVeiculo = cargaIntegracaoEmbarcador.Veiculo.TipoVeiculo,
                        UF = cargaIntegracaoEmbarcador.Veiculo.Estado?.Sigla
                    });

                    foreach (Dominio.Entidades.Veiculo reboque in cargaIntegracaoEmbarcador.VeiculosVinculados)
                    {
                        cteIntegrar.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo()
                        {
                            Placa = reboque.Placa,
                            CapacidadeKG = reboque.CapacidadeKG,
                            CapacidadeM3 = reboque.CapacidadeM3,
                            Chassi = reboque.Chassi,
                            Proprietario = serCliente.ObterClienteCTE(reboque.Proprietario, null),
                            Renavam = reboque.Renavam,
                            RNTRCProprietario = reboque.RNTRC,
                            Tara = reboque.Tara,
                            TipoCarroceria = reboque.TipoCarroceria,
                            TipoPropriedade = reboque.Tipo,
                            TipoProprietario = reboque.TipoProprietario,
                            TipoRodado = reboque.TipoRodado,
                            TipoVeiculo = reboque.TipoVeiculo,
                            UF = reboque.Estado?.Sigla
                        });
                    }
                }

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = svcCTe.GerarCTePorObjeto(cteIntegrar, 0, unitOfWork, "1", 0, "A", cargaIntegracaoEmbarcador.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal, 0, tipoServicoMultisoftware, cargaIntegracaoEmbarcador.Empresa);

                cargaIntegracaoEmbarcador.CTes.Add(cte);
            }

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            mensagemErro = null;
            return true;
        }

        private async Task<string> ImportarMDFesCargaEmbarcadorAsync(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repositorioCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(_unitOfWork, _cancellationToken);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(_unitOfWork, _cancellationToken);

            Servicos.ServicoSGT.MDFe.MDFeClient servicoWSMDFe = ObterClientMDFes(cargaIntegracaoEmbarcador.TipoOperacao.URLIntegracaoMultiEmbarcador, cargaIntegracaoEmbarcador.TipoOperacao.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            if (cargaIntegracaoEmbarcador.MDFes != null)
                cargaIntegracaoEmbarcador.MDFes.Clear();
            else
                cargaIntegracaoEmbarcador.MDFes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            do
            {
                ServicoSGT.MDFe.RetornoOfPaginacaoOfMDFeUBGeyIFJ51p1vPsU retorno = await servicoWSMDFe.BuscarMDFesPorCargaAsync(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, 0, 50);

                if (!retorno.Status)
                {
                    cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                    await repositorioCargaIntegracaoEmbarcador.AtualizarAsync(cargaIntegracaoEmbarcador);

                    return retorno.Mensagem;
                }

                foreach (Dominio.ObjetosDeValor.WebService.MDFe.MDFe mdfe in retorno.Objeto.Itens)
                {
                    if (mdfe.StatusMDFe != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                        mdfe.StatusMDFe != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                        mdfe.StatusMDFe != Dominio.Enumeradores.StatusMDFe.Encerrado)
                        continue;

                    if (string.IsNullOrWhiteSpace(mdfe.XMLAutorizacao))
                    {
                        cargaIntegracaoEmbarcador.Mensagem = $"O MDF-e {mdfe.Numero} não possui um XML.";

                        await repositorioCargaIntegracaoEmbarcador.AtualizarAsync(cargaIntegracaoEmbarcador);

                        return cargaIntegracaoEmbarcador.Mensagem;
                    }

                    System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(mdfe.XMLAutorizacao);

                    object arquivoMDFe = MultiSoftware.MDFe.Servicos.Leitura.Ler(memoryStream);
                    MultiSoftware.MDFe.v300.mdfeProc objetoMDFe = null;

                    if (arquivoMDFe != null)
                    {
                        Type tipoArquivo = arquivoMDFe.GetType();

                        if (tipoArquivo == typeof(MultiSoftware.MDFe.v300.mdfeProc))
                        {
                            objetoMDFe = (MultiSoftware.MDFe.v300.mdfeProc)arquivoMDFe;

                            if (Utilidades.String.OnlyNumbers(objetoMDFe.MDFe.infMDFe.emit.Item) != Utilidades.String.OnlyNumbers(cargaIntegracaoEmbarcador.Empresa.CNPJ))
                                continue;
                        }
                    }

                    object retornoInserir = objetoMDFe != null ? await servicoMDFe.GerarMDFeAnteriorAsync(cargaIntegracaoEmbarcador.Empresa, objetoMDFe, memoryStream, false) : await servicoMDFe.GerarMDFeAnteriorAsync(memoryStream, cargaIntegracaoEmbarcador.Empresa.Codigo, null, false);

                    if (retornoInserir is string)
                    {
                        cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;

                        await repositorioCargaIntegracaoEmbarcador.AtualizarAsync(cargaIntegracaoEmbarcador);
                        return (string)retornoInserir;
                    }

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeInserido = (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)retornoInserir;

                    cargaIntegracaoEmbarcador.MDFes.Add(mdfeInserido);
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            await _unitOfWork.StartAsync();

            if (cargaIntegracaoEmbarcador.TipoOperacao.NaoGerarCargaMultiEmbarcador ?? false)
            {
                if (cargaIntegracaoEmbarcador.Cancelamento)
                    cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados;
                else
                    cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Finalizado;
            }
            else
                cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgGeracaoCarga;

            await repositorioCargaIntegracaoEmbarcador.AtualizarAsync(cargaIntegracaoEmbarcador);

            _unitOfWork.CommitChangesAsync();

            return string.Empty;
        }

        private static bool ImportarCargasEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            ServicoSGT.Carga.CargasClient svcCarga = ObterClientCarga(tipoOperacao.URLIntegracaoMultiEmbarcador, tipoOperacao.TokenIntegracaoMultiEmbarcador);

            ServicoSGT.Carga.RetornoOfArrayOfCargaVQbIXuKl retorno;

            if (tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador ?? false)
                retorno = svcCarga.BuscarCargasPorTransportadorV2(tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador);
            else
                retorno = svcCarga.BuscarCargasPorTransportador();

            if (!retorno.Status)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }

            DateTime dataConsulta = retorno.DataRetorno.ToNullableDateTime() ?? DateTime.Now;

            foreach (Dominio.ObjetosDeValor.WebService.Carga.Carga carga in retorno.Objeto)
            {
                StringBuilder mensagem = new StringBuilder();
                bool sucesso = true;

                DateTime dataCriacaoCarga = carga.DataCriacaoCarga?.ToDateTime() ?? DateTime.MinValue;

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador = repCargaIntegracaoEmbarcador.BuscarPorProtocoloEGrupoPessoas(carga.Protocolo, tipoOperacao.GrupoPessoas.Codigo);

                if ((tipoOperacao.DataInicialCargasMultiEmbarcador.HasValue && tipoOperacao.DataInicialCargasMultiEmbarcador > dataCriacaoCarga) ||
                    (cargaIntegracaoEmbarcador != null &&
                    cargaIntegracaoEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Pendente &&
                    cargaIntegracaoEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Problemas))
                {
                    svcCarga.ConfirmarIntegracaoCargaTransportador(carga.Protocolo);
                    continue;
                }

                unitOfWork.Start();

                if (cargaIntegracaoEmbarcador == null)
                    cargaIntegracaoEmbarcador = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador();

                sucesso = SetarInformacoesCargaIntegracaoEmbarcador(out mensagem, false, cargaIntegracaoEmbarcador, carga, tipoOperacao, dataConsulta, unitOfWork);

                if (sucesso)
                {
                    cargaIntegracaoEmbarcador.Mensagem = "Integrado com sucesso.";
                    cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaCTes;
                }
                else
                {
                    cargaIntegracaoEmbarcador.Mensagem = mensagem.ToString();
                    cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Problemas;
                }

                unitOfWork.CommitChanges();

                if (sucesso)
                    svcCarga.ConfirmarIntegracaoCargaTransportador(cargaIntegracaoEmbarcador.Protocolo);

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        private static bool SetarInformacoesCargaIntegracaoEmbarcador(out StringBuilder mensagem, bool cancelamento, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, DateTime dataConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            mensagem = new StringBuilder();
            bool sucesso = true;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            cargaIntegracaoEmbarcador.DataConsulta = dataConsulta;
            cargaIntegracaoEmbarcador.DataCriacaoCarga = carga.DataCriacaoCarga?.ToDateTime();
            cargaIntegracaoEmbarcador.DataPrevisaoEntrega = carga.DataPrevisaoEntrega?.ToDateTime();
            cargaIntegracaoEmbarcador.Destino = carga.Destino;
            cargaIntegracaoEmbarcador.Protocolo = carga.Protocolo;
            cargaIntegracaoEmbarcador.NumeroCarga = carga.NumeroCarga;
            cargaIntegracaoEmbarcador.Origem = carga.Origem;
            cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Pendente;
            cargaIntegracaoEmbarcador.TipoOperacao = tipoOperacao;
            cargaIntegracaoEmbarcador.Empresa = repEmpresa.BuscarPorCNPJ(carga.Transportador.CNPJ);

            if (cargaIntegracaoEmbarcador.Empresa == null)
                Servicos.Log.TratarErro($"Importação de cargas do embarcador: Empresa com o CNPJ {carga.Transportador.CNPJ} não está cadastrada no ambiente.");

            cargaIntegracaoEmbarcador.Cancelamento = cancelamento;
            cargaIntegracaoEmbarcador.TipoCarga = ObterTipoCargaEmbarcador(tipoOperacao, carga, unitOfWork);

            if (cargaIntegracaoEmbarcador.Empresa == null)
            {
                sucesso = false;
                mensagem.Append($"A empresa {carga.Transportador.CNPJ} - {carga.Transportador.RazaoSocial} não está cadastrada. ");
            }

            if (cargaIntegracaoEmbarcador.Codigo > 0)
                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
            else
                repCargaIntegracaoEmbarcador.Inserir(cargaIntegracaoEmbarcador);

            if (!cancelamento || (tipoOperacao?.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos ?? false))
            {
                if (!SetarVeiculos(carga, cargaIntegracaoEmbarcador, mensagem, unitOfWork))
                    sucesso = false;

                if (!SetarMotoristas(carga, cargaIntegracaoEmbarcador, mensagem, unitOfWork))
                    sucesso = false;

                if (!SetarIntegracaoValePedagio(carga, cargaIntegracaoEmbarcador, mensagem, unitOfWork))
                    sucesso = false;

                if (!SetarValePedagio(carga, cargaIntegracaoEmbarcador, mensagem, unitOfWork))
                    sucesso = false;
            }

            SalvarPedidos(carga, cargaIntegracaoEmbarcador, unitOfWork);

            return sucesso;
        }

        private static bool ImportarCancelamentosEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            ServicoSGT.Carga.CargasClient svcCarga = ObterClientCarga(tipoOperacao.URLIntegracaoMultiEmbarcador, tipoOperacao.TokenIntegracaoMultiEmbarcador);

            ServicoSGT.Carga.RetornoOfArrayOfCargaCancelamentog6I2ue_Sm retorno = svcCarga.BuscarCargasCanceladasPorTransportador();

            if (!retorno.Status)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }

            foreach (Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento cancelamento in retorno.Objeto)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador = repCargaIntegracaoEmbarcador.BuscarPorProtocoloETipoOperacao(cancelamento.ProtocoloCarga, tipoOperacao.Codigo);

                if (cargaIntegracaoEmbarcador != null && (cargaIntegracaoEmbarcador.Situacao != SituacaoCargaIntegracaoEmbarcador.Problemas || cargaIntegracaoEmbarcador.Carga != null))
                {
                    if (!ValidarSePodeCancelarCarga(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork))
                    {
                        if (cargaIntegracaoEmbarcador.Carga != null && (cargaIntegracaoEmbarcador.Carga.SituacaoCarga == SituacaoCarga.Cancelada || cargaIntegracaoEmbarcador.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                        {
                            cargaIntegracaoEmbarcador.Mensagem = "A carga já está cancelada/anulada, não sendo possível importar o cancelamento do embarcador.";
                            cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Cancelado;

                            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
                        }
                        else
                        {
                            cargaIntegracaoEmbarcador.Mensagem = mensagemErro;
                            cargaIntegracaoEmbarcador.ProtocoloCancelamento = cancelamento.ProtocoloCancelamento;
                            cargaIntegracaoEmbarcador.DataCancelamento = cancelamento.DataCancelamento;
                            cargaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(cancelamento.MotivoCancelamento, 250);

                            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                            continue;
                        }
                    }
                    else
                    {
                        cargaIntegracaoEmbarcador.ProtocoloCancelamento = cancelamento.ProtocoloCancelamento;
                        cargaIntegracaoEmbarcador.DataCancelamento = cancelamento.DataCancelamento;
                        cargaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(cancelamento.MotivoCancelamento, 250);
                        cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados;

                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
                    }
                }
                else
                {
                    if (cancelamento.PossuiDocumentoCancelado)
                    {
                        ServicoSGT.Carga.RetornoOfCargaVQbIXuKl retornoConsultaCarga = svcCarga.BuscarCargaPorTransportador(cancelamento.ProtocoloCarga);

                        if (!retornoConsultaCarga.Status)
                        {
                            mensagemErro = retornoConsultaCarga.Mensagem;
                            return false;
                        }

                        DateTime dataConsulta = retorno.DataRetorno.ToDateTime();

                        Dominio.ObjetosDeValor.WebService.Carga.Carga carga = retornoConsultaCarga.Objeto;

                        unitOfWork.Start();

                        if (cargaIntegracaoEmbarcador == null)
                            cargaIntegracaoEmbarcador = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador();

                        if (SetarInformacoesCargaIntegracaoEmbarcador(out StringBuilder mensagem, true, cargaIntegracaoEmbarcador, carga, tipoOperacao, dataConsulta, unitOfWork))
                        {
                            cargaIntegracaoEmbarcador.Mensagem = "Integrado com sucesso à partir do cancelamento.";
                            cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgConsultaCTes;
                            cargaIntegracaoEmbarcador.ProtocoloCancelamento = cancelamento.ProtocoloCancelamento;
                            cargaIntegracaoEmbarcador.DataCancelamento = cancelamento.DataCancelamento;
                            cargaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(cancelamento.MotivoCancelamento, 250);

                            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
                        }
                        else
                        {
                            unitOfWork.Rollback();

                            Servicos.Log.TratarErro($"Não importou a carga à partir do cancelamento (Código: {cancelamento.ProtocoloCarga}, Código Cancelamento: {cancelamento.ProtocoloCancelamento}): {mensagem}");

                            continue;
                            //mensagemErro = mensagem.ToString();
                            //return false;
                        }

                        unitOfWork.CommitChanges();
                    }
                }

                svcCarga.ConfirmarIntegracaoCancelamentoTransportador(cancelamento.ProtocoloCancelamento);
            }

            return true;
        }

        private static bool ValidarSePodeCancelarCarga(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitirCancelamento = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            if (cargaIntegracaoEmbarcador.Situacao != SituacaoCargaIntegracaoEmbarcador.Finalizado)
            {
                mensagem = $"A situação da integração ({cargaIntegracaoEmbarcador.Situacao.ObterDescricao()}) não permite que seja integrado o cancelamento.";
                return false;
            }

            if (cargaIntegracaoEmbarcador.Carga != null && !situacoesPermitirCancelamento.Contains(cargaIntegracaoEmbarcador.Carga.SituacaoCarga))
            {
                mensagem = $"A situação da carga {cargaIntegracaoEmbarcador.Carga.CodigoCargaEmbarcador} ({cargaIntegracaoEmbarcador.Carga.SituacaoCarga.ObterDescricao()}) não permite que a mesma seja cancelada.";
                return false;
            }

            mensagem = "";
            return true;
        }

        private static bool ImportarCTesCanceladosEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            mensagemErro = null;

            if (cargaIntegracaoEmbarcador.Carga == null && !(cargaIntegracaoEmbarcador.TipoOperacao?.NaoGerarCargaMultiEmbarcador ?? false))
            {
                cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgGeracaoCancelamento;

                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                return true;
            }

            if (!ConsultarCTesCancelamento(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork, tipoServicoMultisoftware) ||
                !ConsultarNFSesCancelamento(out mensagemErro, cargaIntegracaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
            {
                cargaIntegracaoEmbarcador.Mensagem = mensagemErro;
                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                return false;
            }

            cargaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.AgGeracaoCancelamento;

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            return true;
        }

        private static bool ConsultarCTesCancelamento(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.ServicoSGT.CTe.CTeClient svcWSCTe = ObterClientCTes(cargaIntegracaoEmbarcador.TipoOperacao.URLIntegracaoMultiEmbarcador, cargaIntegracaoEmbarcador.TipoOperacao.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            do
            {
                ServicoSGT.CTe.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcWSCTe.BuscarCTesPorCarga(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, 0, 50);

                if (!retorno.Status)
                {
                    cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                    repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in retorno.Objeto.Itens)
                {
                    if (cte.Modelo != "57")
                        continue;

                    if (string.IsNullOrWhiteSpace(cte.XMLCancelamento))
                    {
                        mensagemErro = $"O CT-e {cte.Numero} não possui um XML de cancelamento/inutilização.";

                        cargaIntegracaoEmbarcador.Mensagem = mensagemErro;
                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);
                        return false;
                    }

                    System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.XMLCancelamento.Replace("ProcInutCTe", "procInutCTe"));

                    object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, cargaIntegracaoEmbarcador.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false, tipoServicoMultisoftware, true, cte);

                    if (retornoInserir == null)
                    {
                        cargaIntegracaoEmbarcador.Mensagem = $"Não foi possível ler o XML de cancelamento/inutilização do CT-e {cte.Numero}.";
                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                    else if (retornoInserir.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteInserido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;

                        if (cargaIntegracaoEmbarcador.Carga != null)
                        {
                            if (cteInserido.Status == "I" && !repCargaCTe.ExistePorCargaECTe(cargaIntegracaoEmbarcador.Carga.Codigo, cteInserido.Codigo))
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                                {
                                    Carga = cargaIntegracaoEmbarcador.Carga,
                                    CargaOrigem = cargaIntegracaoEmbarcador.Carga,
                                    CTe = cteInserido,
                                    DataVinculoCarga = DateTime.Now,
                                    SistemaEmissor = SistemaEmissor.OutrosEmissores
                                };

                                repCargaCTe.Inserir(cargaCTe);

                                if (cargaCTe.CTe != null)
                                {
                                    new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).GeracaoControleDocumento(cargaCTe.CTe);
                                }
                            }
                        }
                    }
                    else if (retornoInserir.GetType() == typeof(string))
                    {
                        cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                        repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool ConsultarNFSesCancelamento(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.ServicoSGT.NFS.NFSClient svcNFSe = ObterClientNFSes(cargaIntegracaoEmbarcador.TipoOperacao.URLIntegracaoMultiEmbarcador, cargaIntegracaoEmbarcador.TipoOperacao.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            do
            {
                ServicoSGT.NFS.RetornoOfPaginacaoOfNFSYRsas_SFX51p1vPsU retorno = svcNFSe.BuscarNFSsPorCarga(cargaIntegracaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, inicio, limite);

                if (!retorno.Status)
                {
                    cargaIntegracaoEmbarcador.Mensagem = retorno.Mensagem;
                    repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.NFS.NFS nfse in retorno.Objeto.Itens)
                {
                    if (nfse?.NFSe == null || nfse.NFSe.StatusNFSe != Dominio.Enumeradores.StatusNFSe.Cancelado)
                        continue;

                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

                    if (empresa == null)
                    {
                        mensagemErro = $"Emitente da NFS-e ({nfse.TransportadoraEmitente.CNPJ} - {nfse.TransportadoraEmitente.RazaoSocial}) não encontrado.";
                        return false;
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = null;

                    if (cargaIntegracaoEmbarcador.Carga != null)
                        nfseGerada = repCargaCTe.BuscarNFSePorCarga(cargaIntegracaoEmbarcador.Carga.Codigo, nfse.NFSe.Numero, nfse.NFSe.Serie, empresa.Codigo, nfse.NFSe.CodigoVerificacao);
                    else
                        nfseGerada = repCTe.BuscarNFSe(nfse.NFSe.Numero, nfse.NFSe.Serie, empresa.Codigo, nfse.NFSe.CodigoVerificacao);

                    if (nfseGerada == null)
                    {
                        mensagemErro = $"NFS-e não encontrada ({nfse.NFSe.Numero}-{nfse.NFSe.Serie}).";
                        return false;
                    }

                    if (nfseGerada.Status == "A")
                    {
                        nfseGerada.Status = "C";
                        nfseGerada.DataCancelamento = nfse.NFSe.DataCancelamento?.ToNullableDateTime() ?? DateTime.Now;
                        nfseGerada.ObservacaoCancelamento = nfse.NFSe.MotivoCancelamento;

                        repCTe.Atualizar(nfseGerada);
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool GerarCancelamentoEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            var repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            var repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            if (cargaIntegracaoEmbarcador.Carga != null)
            {
                var cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaIntegracaoEmbarcador.Carga.Codigo);

                if (cargaCancelamento == null)
                {
                    cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento()
                    {
                        Carga = cargaIntegracaoEmbarcador.Carga,
                        DataCancelamento = cargaIntegracaoEmbarcador.DataCancelamento,
                        DataEnvioCancelamento = DateTime.Now,
                        MotivoCancelamento = $"Cancelamento importado do MultiEmbarcador ({cargaIntegracaoEmbarcador.MotivoCancelamento})",
                        Situacao = SituacaoCancelamentoCarga.EmCancelamento,
                        Tipo = TipoCancelamentoCarga.Cancelamento,
                        SituacaoCargaNoCancelamento = cargaIntegracaoEmbarcador.Carga?.SituacaoCarga ?? SituacaoCarga.Nova,
                        CancelarDocumentosEmitidosNoEmbarcador = true,
                        DuplicarCarga = cargaIntegracaoEmbarcador.TipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador ?? false
                    };

                    repCargaCancelamento.Inserir(cargaCancelamento);

                    cargaIntegracaoEmbarcador.CargaCancelamento = cargaCancelamento;
                }
            }

            cargaIntegracaoEmbarcador.Situacao = SituacaoCargaIntegracaoEmbarcador.Cancelado;
            cargaIntegracaoEmbarcador.Mensagem = "Cancelamento gerado com sucesso.";

            repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracaoEmbarcador);

            return true;
        }

        private static bool SetarVeiculos(Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, StringBuilder mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Veiculo != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                cargaIntegracaoEmbarcador.Veiculo = repVeiculo.BuscarPorPlaca(carga.Veiculo.Placa);

                if (cargaIntegracaoEmbarcador.Veiculo == null)
                {
                    mensagem.Append("O veículo ").Append(carga.Veiculo.Placa).Append(" não está cadastrado. ");
                    return false;
                }

                if (cargaIntegracaoEmbarcador.VeiculosVinculados == null)
                    cargaIntegracaoEmbarcador.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                else
                    cargaIntegracaoEmbarcador.VeiculosVinculados.Clear();

                foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboque in carga.Veiculo.Reboques)
                {
                    Dominio.Entidades.Veiculo reboqueCadastrado = repVeiculo.BuscarPorPlaca(reboque.Placa);

                    if (reboqueCadastrado != null)
                        cargaIntegracaoEmbarcador.VeiculosVinculados.Add(reboqueCadastrado);
                    else
                    {
                        mensagem.Append("O reboque ").Append(reboque.Placa).Append(" não está cadastrado. ");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool SetarMotoristas(Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, StringBuilder mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Motoristas != null)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                if (cargaIntegracaoEmbarcador.Motoristas == null)
                    cargaIntegracaoEmbarcador.Motoristas = new List<Dominio.Entidades.Usuario>();
                else
                    cargaIntegracaoEmbarcador.Motoristas.Clear();

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motorista in carga.Motoristas)
                {
                    string cpf = Utilidades.String.OnlyNumbers(motorista.CPF).PadLeft(11, '0');

                    Dominio.Entidades.Usuario motoristaCadastrado = repUsuario.BuscarMotoristaPorCPF(cpf);

                    if (motoristaCadastrado != null)
                        cargaIntegracaoEmbarcador.Motoristas.Add(motoristaCadastrado);
                    else
                    {
                        mensagem.Append("O motorista ").Append(cpf).Append(" - ").Append(motorista.Nome).Append(" não está cadastrado. ");
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool SetarValePedagio(Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, StringBuilder mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            bool retorno = true;

            if (carga.CargaValePedagio != null)
            {
                Servicos.Cliente servicoCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio repositorioCargaIntegracaoEmbarcadorValePedagio = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio(unitOfWork);

                foreach (Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio cargaValePedagio in carga.CargaValePedagio)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio cargaIntegracaoEmbarcadorValePedagio = repositorioCargaIntegracaoEmbarcadorValePedagio.BuscarPorCargaEmbarcadorCodigoValePedagioEmbarcador(cargaIntegracaoEmbarcador.Codigo, cargaValePedagio.CodigoValePedagioEmbarcador);

                    bool bAdicionar = false;
                    if (cargaIntegracaoEmbarcadorValePedagio == null)
                    {
                        bAdicionar = true;

                        cargaIntegracaoEmbarcadorValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio();
                        cargaIntegracaoEmbarcadorValePedagio.CargaIntegracaoEmbarcador = cargaIntegracaoEmbarcador;
                    }

                    cargaIntegracaoEmbarcadorValePedagio.Fornecedor = repositorioCliente.BuscarPorCPFCNPJ(cargaValePedagio.Fornecedor.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                    if (cargaIntegracaoEmbarcadorValePedagio.Fornecedor == null)
                    {
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoFornecedor = servicoCliente.ConverterObjetoValorPessoa(cargaValePedagio.Fornecedor, string.Empty, unitOfWork, cargaIntegracaoEmbarcador?.Empresa?.Codigo ?? 0);
                        if (retornoFornecedor.Status)
                        {
                            cargaIntegracaoEmbarcadorValePedagio.Fornecedor = retornoFornecedor.cliente;
                        }
                        else
                        {
                            mensagem.Append("Vale pedágio fornecedor ").Append(cargaValePedagio.Fornecedor.CPFCNPJ).Append(": ").Append(retornoFornecedor.Mensagem);
                            retorno = false;
                        }
                    }

                    if (cargaValePedagio.Responsavel != null)
                    {
                        cargaIntegracaoEmbarcadorValePedagio.Responsavel = repositorioCliente.BuscarPorCPFCNPJ(cargaValePedagio.Responsavel.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                        if (cargaIntegracaoEmbarcadorValePedagio.Responsavel == null)
                        {
                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoResponsavel = servicoCliente.ConverterObjetoValorPessoa(cargaValePedagio.Responsavel, string.Empty, unitOfWork, cargaIntegracaoEmbarcador?.Empresa?.Codigo ?? 0);
                            if (retornoResponsavel.Status)
                            {
                                cargaIntegracaoEmbarcadorValePedagio.Responsavel = retornoResponsavel.cliente;
                            }
                            else
                            {
                                mensagem.Append("Vale pedágio responsável ").Append(cargaValePedagio.Fornecedor.CPFCNPJ).Append(": ").Append(retornoResponsavel.Mensagem);
                                retorno = false;
                            }
                        }
                    }

                    cargaIntegracaoEmbarcadorValePedagio.NumeroComprovante = cargaValePedagio.NumeroComprovante;
                    cargaIntegracaoEmbarcadorValePedagio.CodigoAgendamentoPorto = cargaValePedagio.CodigoAgendamentoPorto;
                    cargaIntegracaoEmbarcadorValePedagio.Valor = cargaValePedagio.Valor;
                    cargaIntegracaoEmbarcadorValePedagio.TipoCompra = cargaValePedagio.TipoCompra;
                    cargaIntegracaoEmbarcadorValePedagio.QuantidadeEixos = cargaValePedagio.QuantidadeEixos;
                    cargaIntegracaoEmbarcadorValePedagio.NaoIncluirMDFe = cargaValePedagio.NaoIncluirMDFe;
                    cargaIntegracaoEmbarcadorValePedagio.CodigoIntegracaoValePedagioEmbarcador = cargaValePedagio.CodigoIntegracaoValePedagioEmbarcador;

                    if (bAdicionar)
                        repositorioCargaIntegracaoEmbarcadorValePedagio.Inserir(cargaIntegracaoEmbarcadorValePedagio);
                    else
                        repositorioCargaIntegracaoEmbarcadorValePedagio.Atualizar(cargaIntegracaoEmbarcadorValePedagio);
                }
            }

            return retorno;
        }

        private static bool SetarIntegracaoValePedagio(Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, StringBuilder mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            bool retorno = true;

            if (carga.CargaIntegracaoValePedagio != null)
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao(unitOfWork);

                foreach (Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in carga.CargaIntegracaoValePedagio)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao cargaIntegracaoEmbarcadorValePedagioIntegracao = repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.BuscarPorCargaEmbarcadorCodigoIntegracaoValePedagioEmbarcador(cargaIntegracaoEmbarcador.Codigo, cargaIntegracaoValePedagio.CodigoIntegracaoValePedagioEmbarcador);

                    bool bAdicionar = false;
                    if (cargaIntegracaoEmbarcadorValePedagioIntegracao == null)
                    {
                        bAdicionar = true;

                        cargaIntegracaoEmbarcadorValePedagioIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao();
                        cargaIntegracaoEmbarcadorValePedagioIntegracao.CargaIntegracaoEmbarcador = cargaIntegracaoEmbarcador;
                        cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoIntegracaoValePedagioEmbarcador = cargaIntegracaoValePedagio.CodigoIntegracaoValePedagioEmbarcador;
                    }

                    cargaIntegracaoEmbarcadorValePedagioIntegracao.SituacaoValePedagio = cargaIntegracaoValePedagio.SituacaoValePedagio;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.NumeroValePedagio = cargaIntegracaoValePedagio.NumeroValePedagio;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.IdCompraValePedagio = cargaIntegracaoValePedagio.IdCompraValePedagio;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.ValorValePedagio = cargaIntegracaoValePedagio.ValorValePedagio;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao1 = cargaIntegracaoValePedagio.Observacao1;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao2 = cargaIntegracaoValePedagio.Observacao2;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao3 = cargaIntegracaoValePedagio.Observacao3;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao4 = cargaIntegracaoValePedagio.Observacao4;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao5 = cargaIntegracaoValePedagio.Observacao5;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao6 = cargaIntegracaoValePedagio.Observacao6;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.RotaTemporaria = cargaIntegracaoValePedagio.RotaTemporaria;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoIntegracaoValePedagio = cargaIntegracaoValePedagio.CodigoIntegracaoValePedagio;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoRota = cargaIntegracaoValePedagio.TipoRota;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoCompra = cargaIntegracaoValePedagio.TipoCompra;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CompraComEixosSuspensos = cargaIntegracaoValePedagio.CompraComEixosSuspensos;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoRoteiro = cargaIntegracaoValePedagio.CodigoRoteiro;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoPercurso = cargaIntegracaoValePedagio.CodigoPercurso;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.QuantidadeEixos = cargaIntegracaoValePedagio.QuantidadeEixos;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.RecebidoPorIntegracao = cargaIntegracaoValePedagio.RecebidoPorIntegracao;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.ValidaCompraRemoveuComponentes = cargaIntegracaoValePedagio.ValidaCompraRemoveuComponentes;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.NomeTransportador = cargaIntegracaoValePedagio.NomeTransportador;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoPercursoVP = cargaIntegracaoValePedagio.TipoPercursoVP;
                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CnpjMeioPagamento = cargaIntegracaoValePedagio.CnpjMeioPagamento;

                    if (bAdicionar)
                        repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.Inserir(cargaIntegracaoEmbarcadorValePedagioIntegracao);
                    else
                        repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.Atualizar(cargaIntegracaoEmbarcadorValePedagioIntegracao);
                }
            }

            return retorno;
        }

        private static void SalvarPedidos(Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pessoa.Pessoa svcPessoa = new Pessoa.Pessoa();
            Servicos.Cliente svcCliente = new Cliente();

            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido repCargaIntegracaoEmbarcadorPedido = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido(unitOfWork);

            foreach (Dominio.ObjetosDeValor.WebService.Carga.Pedido pedido in carga.Pedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido cargaIntegracaoEmbarcadorPedido = repCargaIntegracaoEmbarcadorPedido.BuscarPorCargaIntegracaoEmbarcadorEProtocolo(cargaIntegracaoEmbarcador.Codigo, pedido.Protocolo);

                if (cargaIntegracaoEmbarcadorPedido == null)
                    cargaIntegracaoEmbarcadorPedido = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido();

                cargaIntegracaoEmbarcadorPedido.NumeroPedido = pedido.NumeroPedido;
                cargaIntegracaoEmbarcadorPedido.Protocolo = pedido.Protocolo;
                cargaIntegracaoEmbarcadorPedido.TipoPagamento = pedido.TipoPagamento;
                cargaIntegracaoEmbarcadorPedido.TipoTomador = pedido.TipoTomador;
                cargaIntegracaoEmbarcadorPedido.CargaIntegracaoEmbarcador = cargaIntegracaoEmbarcador;
                cargaIntegracaoEmbarcadorPedido.TipoRateio = pedido.TipoRateio;
                cargaIntegracaoEmbarcadorPedido.PossuiCTe = pedido.PossuiCTe;
                cargaIntegracaoEmbarcadorPedido.PossuiNFS = pedido.PossuiNFS;
                cargaIntegracaoEmbarcadorPedido.PossuiNFSManual = pedido.PossuiNFSManual;

                if (pedido.Remetente != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(pedido.Remetente, "Remetente", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedido.Remetente = retornoVerificacao.cliente;
                }

                if (pedido.Expedidor != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(pedido.Expedidor, "Expedidor", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedido.Expedidor = retornoVerificacao.cliente;
                }

                if (pedido.Recebedor != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(pedido.Recebedor, "Recebedor", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedido.Recebedor = retornoVerificacao.cliente;
                }

                if (pedido.Destinatario != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(pedido.Destinatario, "Destinatario", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedido.Destinatario = retornoVerificacao.cliente;
                }

                if (pedido.Tomador != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(pedido.Tomador, "Tomador", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedido.Tomador = retornoVerificacao.cliente;
                }

                if (cargaIntegracaoEmbarcadorPedido.Codigo > 0)
                    repCargaIntegracaoEmbarcadorPedido.Atualizar(cargaIntegracaoEmbarcadorPedido);
                else
                    repCargaIntegracaoEmbarcadorPedido.Inserir(cargaIntegracaoEmbarcadorPedido);

                SalvarNotasFiscais(pedido, cargaIntegracaoEmbarcadorPedido, unitOfWork);
            }
        }

        private static void SalvarNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido cargaIntegracaoEmbarcadorPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(unitOfWork);

            Servicos.Cliente svcCliente = new Cliente();

            repCargaIntegracaoEmbarcadorPedidoNotaFiscal.DeletarPorCargaIntegracaoEmbarcadorPedido(cargaIntegracaoEmbarcadorPedido.Codigo);

            if (pedido.NotasFiscais.Count > 500)
                pedido.NotasFiscais = pedido.NotasFiscais.Take(500).ToList();

            foreach (Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscal in pedido.NotasFiscais)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal cargaIntegracaoEmbarcadorPedidoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal()
                {
                    CargaIntegracaoEmbarcadorPedido = cargaIntegracaoEmbarcadorPedido,
                    Chave = notaFiscal.Chave,
                    Numero = notaFiscal.Numero,
                    Peso = notaFiscal.Peso,
                    PossuiCTe = notaFiscal.PossuiCTe,
                    PossuiNFS = notaFiscal.PossuiNFS,
                    PossuiNFSManual = notaFiscal.PossuiNFSManual,
                    Serie = notaFiscal.Serie,
                    TipoOperacaoNotaFiscal = notaFiscal.TipoOperacaoNotaFiscal,
                    Valor = notaFiscal.Valor,
                    ValorFrete = notaFiscal.ValorFrete,
                    DataEmissao = notaFiscal.DataEmissao
                };

                if (notaFiscal.Emitente != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(notaFiscal.Emitente, "Emitente", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedidoNotaFiscal.Emitente = retornoVerificacao.cliente;
                }

                if (notaFiscal.Destinatario != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacao = svcCliente.ConverterObjetoValorPessoa(notaFiscal.Destinatario, "Destinatario", unitOfWork, 0, false);

                    if (retornoVerificacao.Status)
                        cargaIntegracaoEmbarcadorPedidoNotaFiscal.Destinatario = retornoVerificacao.cliente;
                }

                if (cargaIntegracaoEmbarcadorPedidoNotaFiscal.Codigo > 0)
                    repCargaIntegracaoEmbarcadorPedidoNotaFiscal.Atualizar(cargaIntegracaoEmbarcadorPedidoNotaFiscal);
                else
                    repCargaIntegracaoEmbarcadorPedidoNotaFiscal.Inserir(cargaIntegracaoEmbarcadorPedidoNotaFiscal);
            }
        }

        private static Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoCargaEmbarcador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoOperacao.GrupoPessoas == null || string.IsNullOrWhiteSpace(carga?.TipoCarga?.CodigoIntegracao))
                return tipoOperacao.TipoDeCargaPadraoOperacao;

            Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador repGrupoPessoasTipoCargaEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador grupoPessoasTipoCargaEmbarcador = repGrupoPessoasTipoCargaEmbarcador.BuscarPorGrupoPessoasECodigoTipoCargaEmbarcador(tipoOperacao.GrupoPessoas.Codigo, carga.TipoCarga.CodigoIntegracao);

            if (grupoPessoasTipoCargaEmbarcador == null)
                return tipoOperacao.TipoDeCargaPadraoOperacao;

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga grupoPessoasTipoCargaEmbarcadorTipoCarga = repGrupoPessoasTipoCargaEmbarcadorTipoCarga.BuscarPorTipoCargaEmbarcador(grupoPessoasTipoCargaEmbarcador.Codigo);

            if (grupoPessoasTipoCargaEmbarcadorTipoCarga == null)
                return tipoOperacao.TipoDeCargaPadraoOperacao;

            return grupoPessoasTipoCargaEmbarcadorTipoCarga.TipoCarga;
        }

        private static ServicoSGT.Carga.CargasClient ObterClientCarga(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Cargas.svc";

            ServicoSGT.Carga.CargasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new ServicoSGT.Carga.CargasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                client = new ServicoSGT.Carga.CargasClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.CTe.CTeClient ObterClientCTes(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "CTe.svc";

            ServicoSGT.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.NFS.NFSClient ObterClientNFSes(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "NFS.svc";

            ServicoSGT.NFS.NFSClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.NFS.NFSClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                client = new ServicoSGT.NFS.NFSClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.MDFe.MDFeClient ObterClientMDFes(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "MDFe.svc";

            ServicoSGT.MDFe.MDFeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.MDFe.MDFeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                client = new ServicoSGT.MDFe.MDFeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static void ValidarVincularValePedagioCargaEmbarcador(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (cargaIntegracaoEmbarcador != null && cargaIntegracaoEmbarcador.Carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> cargasIntegracaoEmbarcadorValePedagioIntegracao = repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

                var cargaPedagioIntegracao = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(cargaIntegracaoEmbarcador.Carga.Codigo);


                foreach (var cargaIntegracaoEmbarcadorValePedagioIntegracao in cargasIntegracaoEmbarcadorValePedagioIntegracao)
                {
                    if (cargaPedagioIntegracao != null && cargaPedagioIntegracao.Any()
                        && cargaPedagioIntegracao.Where(o => o.NumeroValePedagio == cargaIntegracaoEmbarcadorValePedagioIntegracao.NumeroValePedagio).Any())
                    {
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                    cargaIntegracaoValePedagio.Carga = cargaIntegracaoEmbarcador.Carga;
                    cargaIntegracaoValePedagio.TipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);
                    cargaIntegracaoValePedagio.ProblemaIntegracao = "Importado do Embarcador";
                    cargaIntegracaoValePedagio.DataIntegracao = DateTime.Now;
                    cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracaoValePedagio.PedagioIntegradoEmbarcador = true;
                    cargaIntegracaoValePedagio.RotaFrete = null;

                    cargaIntegracaoValePedagio.SituacaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.SituacaoValePedagio;
                    cargaIntegracaoValePedagio.NumeroValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.NumeroValePedagio;
                    cargaIntegracaoValePedagio.IdCompraValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.IdCompraValePedagio;
                    cargaIntegracaoValePedagio.ValorValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.ValorValePedagio;
                    cargaIntegracaoValePedagio.Observacao1 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao1;
                    cargaIntegracaoValePedagio.Observacao2 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao2;
                    cargaIntegracaoValePedagio.Observacao3 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao3;
                    cargaIntegracaoValePedagio.Observacao4 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao4;
                    cargaIntegracaoValePedagio.Observacao5 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao5;
                    cargaIntegracaoValePedagio.Observacao6 = cargaIntegracaoEmbarcadorValePedagioIntegracao.Observacao6;
                    cargaIntegracaoValePedagio.RotaTemporaria = cargaIntegracaoEmbarcadorValePedagioIntegracao.RotaTemporaria;
                    cargaIntegracaoValePedagio.CodigoIntegracaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoIntegracaoValePedagio;
                    cargaIntegracaoValePedagio.TipoRota = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoRota;
                    cargaIntegracaoValePedagio.TipoCompra = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoCompra;
                    cargaIntegracaoValePedagio.CompraComEixosSuspensos = cargaIntegracaoEmbarcadorValePedagioIntegracao.CompraComEixosSuspensos;
                    cargaIntegracaoValePedagio.CodigoRoteiro = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoRoteiro;
                    cargaIntegracaoValePedagio.CodigoPercurso = cargaIntegracaoEmbarcadorValePedagioIntegracao.CodigoPercurso;
                    cargaIntegracaoValePedagio.QuantidadeEixos = cargaIntegracaoEmbarcadorValePedagioIntegracao.QuantidadeEixos;
                    cargaIntegracaoValePedagio.RecebidoPorIntegracao = cargaIntegracaoEmbarcadorValePedagioIntegracao.RecebidoPorIntegracao;
                    cargaIntegracaoValePedagio.ValidaCompraRemoveuComponentes = cargaIntegracaoEmbarcadorValePedagioIntegracao.ValidaCompraRemoveuComponentes;
                    cargaIntegracaoValePedagio.NomeTransportador = cargaIntegracaoEmbarcadorValePedagioIntegracao.NomeTransportador;
                    cargaIntegracaoValePedagio.TipoPercursoVP = cargaIntegracaoEmbarcadorValePedagioIntegracao.TipoPercursoVP;
                    cargaIntegracaoValePedagio.CnpjMeioPagamento = cargaIntegracaoEmbarcadorValePedagioIntegracao.CnpjMeioPagamento;
                    repositorioCargaIntegracaoValePedagio.Inserir(cargaIntegracaoValePedagio);

                    cargaIntegracaoEmbarcadorValePedagioIntegracao.CargaIntegracaoValePedagio = cargaIntegracaoValePedagio;
                    repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.Atualizar(cargaIntegracaoEmbarcadorValePedagioIntegracao);
                    unitOfWork.CommitChanges();
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaPedagio = null;
                if (cargaIntegracaoEmbarcador.Carga != null)
                    cargaPedagio = repositorioCargaValePedagio.BuscarPorCarga(cargaIntegracaoEmbarcador.Carga.Codigo);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio repositorioCargaIntegracaoEmbarcadorValePedagio = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> cargasIntegracaoEmbarcadorValePedagio = repositorioCargaIntegracaoEmbarcadorValePedagio.BuscarPorCargaIntegracaoEmbarcador(cargaIntegracaoEmbarcador.Codigo);

                foreach (var cargaIntegracaoEmbarcadorValePedagio in cargasIntegracaoEmbarcadorValePedagio)
                {

                    if (cargaPedagio != null
                        && cargaPedagio.Any()
                        && cargaPedagio.Where(o => o.NumeroComprovante == cargaIntegracaoEmbarcadorValePedagio.NumeroComprovante).Any())
                    {
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();

                    if (cargaIntegracaoEmbarcadorValePedagio.CodigoIntegracaoValePedagioEmbarcador != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao cargaIntegracaoEmbarcadorValePedagioIntegracao = repositorioCargaIntegracaoEmbarcadorValePedagioIntegracao.BuscarPorCargaEmbarcadorCodigoIntegracaoValePedagioEmbarcador(cargaIntegracaoEmbarcadorValePedagio.CargaIntegracaoEmbarcador.Codigo, (int)cargaIntegracaoEmbarcadorValePedagio.CodigoIntegracaoValePedagioEmbarcador);

                        if (cargaIntegracaoEmbarcadorValePedagioIntegracao != null)
                            cargaValePedagio.CargaIntegracaoValePedagio = cargaIntegracaoEmbarcadorValePedagioIntegracao.CargaIntegracaoValePedagio;
                    }

                    cargaValePedagio.Carga = cargaIntegracaoEmbarcador.Carga;
                    cargaValePedagio.Fornecedor = cargaIntegracaoEmbarcadorValePedagio.Fornecedor;
                    cargaValePedagio.Responsavel = cargaIntegracaoEmbarcadorValePedagio.Responsavel;
                    cargaValePedagio.NumeroComprovante = cargaIntegracaoEmbarcadorValePedagio.NumeroComprovante;
                    cargaValePedagio.CodigoAgendamentoPorto = cargaIntegracaoEmbarcadorValePedagio.CodigoAgendamentoPorto;
                    cargaValePedagio.Valor = cargaIntegracaoEmbarcadorValePedagio.Valor;
                    cargaValePedagio.TipoCompra = cargaIntegracaoEmbarcadorValePedagio.TipoCompra;
                    cargaValePedagio.QuantidadeEixos = cargaIntegracaoEmbarcadorValePedagio.QuantidadeEixos;
                    cargaValePedagio.NaoIncluirMDFe = cargaIntegracaoEmbarcadorValePedagio.NaoIncluirMDFe;

                    repositorioCargaValePedagio.Inserir(cargaValePedagio);
                    unitOfWork.CommitChanges();
                }

            }
        }

        #endregion
    }
}
