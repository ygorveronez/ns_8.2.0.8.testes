using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class CTeAgrupado
    {
        #region Métodos Globais

        public static void ProcessarCTesAgrupadosEmEmissao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<int> codigosCargaCTesAgrupados = repCargaCTeAgrupado.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.EmEmissao, 2);

            if (codigosCargaCTesAgrupados.Count > 0)
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                foreach (int codigoCargaCTeAgrupado in codigosCargaCTesAgrupados)
                {
                    Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(codigoCargaCTeAgrupado, false);

                    GerarCTeAgrupado(cargaCTeAgrupado, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                    unitOfWork.FlushAndClear();
                }
            }
        }

        public static void ProcessarCTesAgrupadosEmCancelamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<int> codigosCargaCTesAgrupados = repCargaCTeAgrupado.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.EmCancelamento, 2);

            foreach (int codigoCargaCTeAgrupado in codigosCargaCTesAgrupados)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(codigoCargaCTeAgrupado, false);

                List<int> codigosCargaCTeAgrupadoCTe = repCargaCTeAgrupadoCTe.BuscarCodigosPorCargaCTeAgrupado(codigoCargaCTeAgrupado);

                if (cargaCTeAgrupado.EnviouDocumentosParaCancelamento)
                {
                    if (repCargaCTeAgrupadoCTe.ExistePorCargaCTeAgrupadoESituacao(cargaCTeAgrupado.Codigo, new List<string>() { "K", "L" }))
                        continue;

                    if (repCargaCTeAgrupadoCTe.ExistePorCargaCTeAgrupadoESituacaoDiff(cargaCTeAgrupado.Codigo, new List<string>() { "I", "Z", "C" }))
                    {
                        cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Finalizado;
                        cargaCTeAgrupado.Mensagem = "Não foi possível cancelar/anular/inutilizar todos os documentos.";

                        repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                        continue;
                    }

                    unitOfWork.Start();

                    foreach (int codigoCargaCTeAgrupadoCTe in codigosCargaCTeAgrupadoCTe)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cargaCTeAgrupadoCTe = repCargaCTeAgrupadoCTe.BuscarPorCodigo(codigoCargaCTeAgrupadoCTe, false);

                        if (cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada &&
                            cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Anulado)
                            continue;

                        if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out string erro, cargaCTeAgrupadoCTe.CTe, tipoServicoMultisoftware, unitOfWork))
                        {
                            unitOfWork.Rollback();

                            cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupado.Codigo, false);

                            cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Finalizado;
                            cargaCTeAgrupado.Mensagem = erro;

                            repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                            continue;
                        }

                        Servicos.Embarcador.Financeiro.DocumentoFaturamento.CancelarDocumentoFaturamentoPorCTe(cargaCTeAgrupadoCTe.CTe, unitOfWork, tipoServicoMultisoftware);
                    }

                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Cancelado;
                    cargaCTeAgrupado.Mensagem = "Cancelamento realizado com sucesso.";

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                    Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado serIntegracaoCargaCTeAgrupado = new Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado(unitOfWork, tipoServicoMultisoftware);
                    serIntegracaoCargaCTeAgrupado.IniciarIntegracoesComDocumentosCancelamento(cargaCTeAgrupado);

                    unitOfWork.CommitChanges();
                }
                else
                {
                    if (!CancelarCTes(out string erro, cargaCTeAgrupado, codigosCargaCTeAgrupadoCTe, unitOfWork, tipoServicoMultisoftware))
                    {
                        cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Finalizado;
                        cargaCTeAgrupado.Mensagem = erro;

                        repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                        continue;
                    }

                    cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupado.Codigo, false);

                    cargaCTeAgrupado.EnviouDocumentosParaCancelamento = true;

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);
                }

                unitOfWork.FlushAndClear();
            }
        }

        #endregion

        #region Métodos Privados

        private static bool CancelarCTes(out string erro, Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, List<int> codigosCargaCTeAgrupadoCTe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.NFSe svcNFSe = new Servicos.NFSe();

            foreach (int codigoCargaCTeAgrupadoCTe in codigosCargaCTeAgrupadoCTe)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cargaCTeAgrupadoCTe = repCargaCTeAgrupadoCTe.BuscarPorCodigoComFetch(codigoCargaCTeAgrupadoCTe);

                if (cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada ||
                    cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada ||
                    cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Anulado ||
                    cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento ||
                    cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                    continue;

                if (cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                    cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    DateTime dataCancelamento = DateTime.Now;

                    unitOfWork.Start();

                    if (cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                        cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                        cargaCTeAgrupadoCTe.CTe.Status = "I";
                    else
                        cargaCTeAgrupadoCTe.CTe.Status = "C";

                    cargaCTeAgrupadoCTe.CTe.DataRetornoSefaz = dataCancelamento;
                    cargaCTeAgrupadoCTe.CTe.DataCancelamento = dataCancelamento;
                    cargaCTeAgrupadoCTe.CTe.ObservacaoCancelamento = cargaCTeAgrupado.MotivoCancelamento;

                    repCTe.Atualizar(cargaCTeAgrupadoCTe.CTe);

                    svcCTe.AjustarAverbacoesParaCancelamento(cargaCTeAgrupadoCTe.CTe.Codigo, unitOfWork);

                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();

                    continue;
                }

                if (cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada)
                {
                    if (cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cargaCTeAgrupadoCTe.CTe.SistemaEmissor).CancelarCte(cargaCTeAgrupadoCTe.CTe.Codigo, cargaCTeAgrupadoCTe.CTe.Empresa.Codigo, cargaCTeAgrupado.MotivoCancelamento, unitOfWork))
                        {
                            erro = $"Não foi possível cancelar o CT-e {cargaCTeAgrupadoCTe.CTe.Numero}.";
                            return false;
                        }
                    }
                    else if (cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        if (!svcNFSe.CancelarNFSe(cargaCTeAgrupadoCTe.CTe.Codigo, unitOfWork))
                        {
                            erro = $"Não foi possível cancelar a NFS-e {cargaCTeAgrupadoCTe.CTe.Numero}.";
                            return false;
                        }
                    }
                }
                else if (cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                         cargaCTeAgrupadoCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                {
                    if (cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        if (!svcCTe.Inutilizar(cargaCTeAgrupadoCTe.CTe.Codigo, cargaCTeAgrupadoCTe.CTe.Empresa.Codigo, cargaCTeAgrupado.MotivoCancelamento, tipoServicoMultisoftware, unitOfWork))
                        {
                            erro = $"Não foi possível inutilizar o CT-e {cargaCTeAgrupadoCTe.CTe.Numero}.";
                            return false;
                        }
                    }
                    else if (cargaCTeAgrupadoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        DateTime dataCancelamento = DateTime.Now;

                        unitOfWork.Start();

                        cargaCTeAgrupadoCTe.CTe.Status = "I";
                        cargaCTeAgrupadoCTe.CTe.DataRetornoSefaz = dataCancelamento;
                        cargaCTeAgrupadoCTe.CTe.DataCancelamento = dataCancelamento;
                        cargaCTeAgrupadoCTe.CTe.ObservacaoCancelamento = cargaCTeAgrupado.MotivoCancelamento;

                        repCTe.Atualizar(cargaCTeAgrupadoCTe.CTe);

                        unitOfWork.CommitChanges();
                    }
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        private static void GerarCTeAgrupado(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga repCargaCTeAgrupadoCarga = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga(unitOfWork);

            List<int> codigosCargaCTeAgrupadoCarga = repCargaCTeAgrupadoCarga.BuscarCodigosPorCargaCTeAgrupadoSemCTeGerado(cargaCTeAgrupado.Codigo);

            if (codigosCargaCTeAgrupadoCarga.Count <= 0)
            {
                if (repCargaCTeAgrupadoCTe.ExistePorCargaCTeAgrupadoESituacao(cargaCTeAgrupado.Codigo, new List<string>() { "R", "C", "Z" }))
                {
                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Rejeitado;
                    cargaCTeAgrupado.Mensagem = "Existem documentos rejeitados/cancelados/anulados.";

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);
                }
                else if (!repCargaCTeAgrupadoCTe.ExistePorCargaCTeAgrupadoESituacaoDiff(cargaCTeAgrupado.Codigo, "A"))
                {
                    Servicos.Embarcador.Carga.Documentos svcDocumentos = new Carga.Documentos(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> ctesGerados = repCargaCTeAgrupadoCTe.BuscarPorCargaCTeAgrupado(cargaCTeAgrupado.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> cargaCTeAgrupadoCargas = repCargaCTeAgrupadoCarga.BuscarPorCargaCTeAgrupado(cargaCTeAgrupado.Codigo);

                    unitOfWork.Start();

                    RatearCTeAgrupadoEntreCargas(cargaCTeAgrupado, cargaCTeAgrupadoCargas, unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cteAgrupadoGerado in ctesGerados)
                    {
                        if (!svcDocumentos.GerarMovimentoAutorizacaoCTe(out string erro, cteAgrupadoGerado.CTe, tipoServicoMultisoftware, unitOfWork, false))
                        {
                            unitOfWork.Rollback();
                            throw new Exception(erro);
                        }

                        Servicos.Log.TratarErro($"GerarCTeAgrupado inserindo documento faturamento - CTe {cteAgrupadoGerado?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                        Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(null, cteAgrupadoGerado.CTe, null, null, null, cargaCTeAgrupado, false, false, false, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    }

                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.AgIntegracao;
                    cargaCTeAgrupado.GerandoIntegracoes = true;

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                    unitOfWork.CommitChanges();
                }

                return;
            }

            if (cargaCTeAgrupado.GerarCTePorCarga)
            {
                for (int i = 0; i < codigosCargaCTeAgrupadoCarga.Count; i++)
                {
                    if (!GerarCTeAgrupado(out string mensagem, cargaCTeAgrupado, new List<int> { codigosCargaCTeAgrupadoCarga[i] }, configuracaoTMS, tipoServicoMultisoftware, unitOfWork))
                    {
                        cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupado.Codigo, false);

                        cargaCTeAgrupado.Mensagem = mensagem;
                        cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Rejeitado;

                        repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                        return;
                    }
                }
            }
            else
            {
                if (!GerarCTeAgrupado(out string mensagem, cargaCTeAgrupado, codigosCargaCTeAgrupadoCarga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork))
                {
                    cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupado.Codigo, false);

                    cargaCTeAgrupado.Mensagem = mensagem;
                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Rejeitado;

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                    return;
                }
            }
        }

        private static bool GerarCTeAgrupado(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, List<int> codigosCargasCTeAgrupado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            mensagem = string.Empty;

            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga repCargaCTeAgrupadoCarga = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> cargaCTeAgrupadoCargas = repCargaCTeAgrupadoCarga.BuscarPorCodigo(codigosCargasCTeAgrupado);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarAutorizadosPorCargas(cargaCTeAgrupadoCargas.Select(o => o.Carga.Codigo).ToList());

            if (cargaCTes.Count <= 0)
            {
                mensagem = $"Nenhum documento encontrado na(s) carga(s) {string.Join(", ", cargaCTeAgrupadoCargas.Select(o => o.Carga.CodigoCargaEmbarcador))} para geração do CT-e.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeBase = cargaCTes.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.Carga cargaBase = cargaCTeBase.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoBase = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaBase.Codigo);

            List<Dominio.Entidades.Veiculo> veiculos = cargaCTeAgrupadoCargas.Select(o => o.Carga.Veiculo).ToList();
            veiculos.AddRange(cargaCTeAgrupadoCargas.Select(o => o.Carga.VeiculosVinculados).SelectMany(o => o));

            veiculos = veiculos.Distinct().ToList();

            IEnumerable<int> codigosCTes = cargaCTes.Select(o => o.CTe.Codigo);
            IEnumerable<int> codigosCargaCTes = cargaCTes.Select(o => o.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponenteFrete.BuscarPorCargaCTes(codigosCargaCTes);
            List<Dominio.Entidades.InformacaoCargaCTE> informacoesCargaCTe = repInformacaoCargaCTe.ObterSumarizadosPorCTes(codigosCTes);
            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTes(codigosCTes);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAgrupado = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

            cargaCTeBase.CTe.CopyProperties(cteAgrupado);

            cteAgrupado.Codigo = 0;
            cteAgrupado.Chave = "";
            cteAgrupado.NumeroRecibo = "";
            cteAgrupado.DataAutorizacao = null;
            cteAgrupado.DataIntegracao = null;
            cteAgrupado.DataRetornoSefaz = null;
            cteAgrupado.DataEmissao = DateTime.Now;
            cteAgrupado.Log = "";
            cteAgrupado.LogIntegracao = "";
            cteAgrupado.MensagemStatus = null;
            cteAgrupado.Status = "P";
            cteAgrupado.CodigoCTeIntegrador = 0;
            cteAgrupado.Peso = cargaCTes.Sum(o => o.CTe.Peso);
            cteAgrupado.MetrosCubicos = cargaCTes.Sum(o => o.CTe.MetrosCubicos);
            cteAgrupado.Pallets = cargaCTes.Sum(o => o.CTe.Pallets);
            cteAgrupado.Volumes = cargaCTes.Sum(o => o.CTe.Volumes);
            cteAgrupado.ValorTotalMercadoria = cargaCTes.Sum(o => o.CTe.ValorTotalMercadoria);
            cteAgrupado.ValorFrete = cargaCTes.Sum(o => o.CTe.ValorFrete);
            cteAgrupado.ValorPrestacaoServico = cteAgrupado.ValorFrete + componentesFrete.Where(c => c.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && 
                                                                                                     c.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS)
                                                                                         .Sum(c => c.ValorComponente);
            cteAgrupado.ValorAReceber = cteAgrupado.ValorPrestacaoServico;
            cteAgrupado.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
            cteAgrupado.Moeda = null;
            cteAgrupado.ValorCotacaoMoeda = null;
            cteAgrupado.ValorTotalMoeda = null;
            cteAgrupado.ObservacoesGerais = cargaCTeAgrupado.ObservacaoCTe;
            cteAgrupado.TipoControle = 1;

            if (cteAgrupado.LocalidadeEmissao.Estado.Sigla == cteAgrupado.LocalidadeInicioPrestacao.Estado.Sigla)
                cteAgrupado.Serie = cteAgrupado.Empresa.Configuracao.SerieIntraestadual;
            else
                cteAgrupado.Serie = cteAgrupado.Empresa.Configuracao.SerieInterestadual;

            Utilidades.Object.DefinirListasGenericasComoNulas(cteAgrupado);

            Servicos.Embarcador.CTe.CTe.SetarParticipantesCTe(cargaCTeBase.CTe, cteAgrupado);

            cteAgrupado.TomadorPagador = cteAgrupado.Tomador;

            SetarImpostos(cteAgrupado, cargaBase, cargaPedidoBase, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

            unitOfWork.Start();
            cteAgrupado.Numero = svcCTe.ObterProximoNumero(cteAgrupado, repCTe);
            repCTe.Inserir(cteAgrupado);

            Servicos.Embarcador.CTe.CTe.DuplicarSeguros(cargaCTeBase.CTe, cteAgrupado, unitOfWork);

            AdicionarComponentes(cteAgrupado, componentesFrete, unitOfWork);
            AdicionarDocumentos(cteAgrupado, documentosCTe, unitOfWork);
            AdicionarVeiculos(cteAgrupado, veiculos, unitOfWork);
            AdicionarDocumentosOriginarios(cteAgrupado, cargaCTes, unitOfWork);

            foreach (Dominio.Entidades.InformacaoCargaCTE informacaoCargaCTe in informacoesCargaCTe)
            {
                informacaoCargaCTe.CTE = cteAgrupado;

                repInformacaoCargaCTe.Inserir(informacaoCargaCTe);
            }

            Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cargaCTeAgrupadoCTe = new Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe()
            {
                CargaCTeAgrupado = cargaCTeAgrupado,
                CTe = cteAgrupado
            };

            repCargaCTeAgrupadoCTe.Inserir(cargaCTeAgrupadoCTe);


            for (int i = 0; i < cargaCTeAgrupadoCargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga cargaCTeAgrupadoCarga = cargaCTeAgrupadoCargas[i];

                cargaCTeAgrupadoCarga.CargaCTeAgrupadoCTe = cargaCTeAgrupadoCTe;

                repCargaCTeAgrupadoCarga.Atualizar(cargaCTeAgrupadoCarga);
            }

            unitOfWork.CommitChanges();

            svcCTe.Emitir(ref cteAgrupado, unitOfWork);

            unitOfWork.FlushAndClear();

            return true;
        }

        private static void AdicionarComponentes(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            if (cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteImpostos = new Dominio.Entidades.ComponentePrestacaoCTE()
                {
                    ComponenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS),
                    Nome = "IMPOSTOS",
                    Valor = cte.ValorICMS,
                    CTE = cte
                };

                repComponentePrestacaoCTe.Inserir(componenteImpostos);
            }

            Dominio.Entidades.ComponentePrestacaoCTE componenteFreteValor = new Dominio.Entidades.ComponentePrestacaoCTE()
            {
                ComponenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.FRETE),
                Nome = "FRETE VALOR",
                Valor = cte.ValorFrete,
                CTE = cte
            };

            repComponentePrestacaoCTe.Inserir(componenteFreteValor);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesAdicionais = componentesFrete.Where(o => o.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && o.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS).GroupBy(o => new { o.IncluirBaseCalculoICMS, o.TipoComponenteFrete, o.DescricaoComponente, IncluirTotalReceber = !o.NaoSomarValorTotalAReceber }).Select(o => new Dominio.Entidades.ComponentePrestacaoCTE()
            {
                IncluiNaBaseDeCalculoDoICMS = o.Key.IncluirBaseCalculoICMS,
                ComponenteFrete = o.FirstOrDefault()?.ComponenteFrete,
                CTE = cte,
                IncluiNoTotalAReceber = o.Key.IncluirTotalReceber,
                Nome = o.Key.DescricaoComponente,
                Valor = o.Sum(c => c.ValorComponente)
            }).ToList();

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteAdicional in componentesAdicionais)
                repComponentePrestacaoCTe.Inserir(componenteAdicional);
        }

        public static void SetarImpostos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS();

            bool incluirICMS = true;
            decimal percentualICMSIncluir = 100m;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMS(carga, cargaPedido, cte.Empresa, cte.Expedidor?.Cliente ?? cte.Remetente.Cliente, cte.Recebedor?.Cliente ?? cte.Destinatario.Cliente, cte.TomadorPagador.Cliente, cte.LocalidadeInicioPrestacao, cte.LocalidadeTerminoPrestacao, ref incluirICMS, ref percentualICMSIncluir, cte.ValorAReceber, null, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal, true);

            cte.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
            cte.NaturezaDaOperacao = cte.CFOP.NaturezaDaOperacao;
            cte.PercentualICMSIncluirNoFrete = regraICMS.PercentualInclusaoBC;
            cte.IncluirICMSNoFrete = regraICMS.IncluirICMSBC ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.ValorPresumido = regraICMS.ValorCreditoPresumido;
            cte.CST = regraICMS.CST;
            cte.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
            cte.ValorICMS = regraICMS.ValorICMS;
            cte.AliquotaICMS = regraICMS.Aliquota;
            cte.PercentualReducaoBaseCalculoICMS = regraICMS.PercentualReducaoBC;
            cte.ValorPresumido = regraICMS.ValorCreditoPresumido;
            cte.ExibeICMSNaDACTE = !regraICMS.NaoImprimirImpostosDACTE;
            cte.RegraICMS = regraICMS.CodigoRegra > 0 ? new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = regraICMS.CodigoRegra } : null;

            if (regraICMS.IncluirICMSBC)
            {
                decimal valorICMSIncluir = regraICMS.ValorICMSIncluso > 0m ? regraICMS.ValorICMSIncluso : regraICMS.ValorICMS;
                decimal valorICMSRecolhido = Math.Round(valorICMSIncluir * (regraICMS.PercentualInclusaoBC / 100), 2, MidpointRounding.AwayFromZero);

                if (configuracaoTMS.UtilizarRegraICMSParaDescontarValorICMS)
                {
                    if (!regraICMS.DescontarICMSDoValorAReceber)
                        cte.ValorAReceber += valorICMSRecolhido;
                }
                else
                {
                    if (cte.CST != "60")
                        cte.ValorAReceber += valorICMSRecolhido;
                }

                if (!regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao)
                    cte.ValorPrestacaoServico += valorICMSRecolhido;

                cte.PercentualICMSIncluirNoFrete = regraICMS.PercentualInclusaoBC;
            }
            else
            {
                if (configuracaoTMS.UtilizarRegraICMSParaDescontarValorICMS)
                {
                    if (cte.ValorICMS > 0m && regraICMS.DescontarICMSDoValorAReceber) //cte.ICMS.CST == "60" // Considerar para todas CST (Tarefa #3586 Marfrig)
                        cte.ValorAReceber = cte.ValorAReceber - cte.ValorICMS;
                }
                else
                {
                    if (cte.ValorICMS > 0m && cte.CST == "60")
                        cte.ValorAReceber = cte.ValorAReceber - cte.ValorICMS;
                }
            }

            if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
            {
                if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
                    cte.ObservacoesGerais += " / ";

                cte.ObservacoesGerais += regraICMS.ObservacaoCTe;
            }
        }

        private static void AdicionarDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentosCTeOriginal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento = documentosCTeOriginal.FirstOrDefault().ModeloDocumentoFiscal;

            foreach (Dominio.Entidades.DocumentosCTE documentoCTeOriginal in documentosCTeOriginal)
            {
                if (documentoCTeOriginal.ModeloDocumentoFiscal != modeloDocumento)
                    continue;

                Dominio.Entidades.DocumentosCTE documentoCTe = documentoCTeOriginal.Clonar();

                documentoCTe.CTE = cte;

                repDocumentoCTe.Inserir(documentoCTe);
            }
        }

        private static void AdicionarVeiculos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.Veiculo> veiculos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                if (veiculo == null)
                    continue;

                Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE()
                {
                    CTE = cte,
                    Veiculo = veiculo
                };

                veiculoCTe.SetarDadosVeiculo(veiculo);

                repVeiculoCTe.Inserir(veiculoCTe);
            }
        }

        private static void AdicionarDocumentosOriginarios(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repCTeDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                if (cargaCTe == null)
                    continue;

                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                {
                    CTe = cte,
                    Numero = cargaCTe.CTe.Numero,
                    DataEmissao = cargaCTe.CTe.DataEmissao,
                    Serie = cargaCTe.CTe.Serie?.Numero.ToString() ?? null
                };

                repCTeDocumentoOriginario.Inserir(documentoOriginario);
            }
        }

        private static void RatearCTeAgrupadoEntreCargas(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> cargaCTeAgrupadoCargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            int countCargas = cargaCTeAgrupadoCargas.Count - 1;

            for (int i = 0; i <= countCargas; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga cargaCTeAgrupadoCarga = cargaCTeAgrupadoCargas[i];

                if (cargaCTeAgrupadoCarga.CargaCTeAgrupadoCTe == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                {
                    Carga = cargaCTeAgrupadoCarga.Carga,
                    CargaCTeAgrupado = cargaCTeAgrupado,
                    CargaOrigem = cargaCTeAgrupadoCarga.Carga,
                    CTe = cargaCTeAgrupadoCarga.CargaCTeAgrupadoCTe.CTe,
                    GerouControleFaturamento = true,
                    GerouMovimentacaoAutorizacao = true,
                    GerouTituloAutorizacao = true,
                    SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe
                };

                repCargaCTe.Inserir(cargaCTe);
            }
        }

        #endregion
    }
}
