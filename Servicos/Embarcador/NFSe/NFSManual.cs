using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.NFSe
{
    public class NFSManual : ServicoBase
    {
        #region Construtores
       
        public NFSManual(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Privados

        private static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaFiltrada, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> autorizacoes = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            int menorPrioridadeAprovacao = listaFiltrada.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores == 0)
                    continue;

                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao autorizacao = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao
                    {
                        LancamentoNFSManual = nfsManual,
                        Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                        Usuario = aprovador,
                        RegrasAutorizacaoNFSManual = regra,
                    };

                    repositorioAutorizacao.Inserir(autorizacao);
                    autorizacoes.Add(autorizacao);

                    if (!autorizacao.Bloqueada)
                        NotificarAprovador(nfsManual, aprovador, tipoServicoMultisoftware, unitOfWork);
                }
            }
        }

        private static List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> FiltraRegrasComRequisitos(List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> regras, out bool alcadaComRequisito)
        {
            alcadaComRequisito = false;

            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> regrasComRequisitos = (from o in regras where o.Requisito != null select o).ToList();
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> regrasFiltradas = (from o in regras where o.Requisito == null select o).ToList();

            foreach (Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regraComRequisito in regrasComRequisitos)
            {
                if (regrasFiltradas.Contains(regraComRequisito.Requisito))
                {
                    alcadaComRequisito = true;
                    regrasFiltradas.Add(regraComRequisito);
                }
            }

            return regrasFiltradas;
        }

        private static bool GerarMovimentosAutorizacaoNFSeManual(out string erro, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Carga.Documentos svcDocumentos = new Carga.Documentos(unitOfWork);

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                if (!svcDocumentos.GerarMovimentoAutorizacaoCTe(out erro, cte, tipoServicoMultisoftware, unitOfWork, false))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        private static bool GerarMovimentosCancelamentoNFSeManual(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Servicos.Embarcador.Financeiro.DocumentoFaturamento.CancelarDocumentoFaturamentoPorCTe(cte, unitOfWork, tipoServicoMultisoftware);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out erro, cte, tipoServicoMultisoftware, unitOfWork))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        private static void LiberarParaNovaNFS(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada repCargaDocumentoParaEmissaoNFSManualCancelada = new Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            for (int i = 0; i < lancamentoNFSManual.Documentos.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = lancamentoNFSManual.Documentos[i];
                cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual = null;
                cargaDocumentoParaEmissaoNFSManual.CTe = null;
                repCargaDocumentoParaEmissaoNFSManual.Atualizar(cargaDocumentoParaEmissaoNFSManual);

                Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada cargaDocumentoParaEmissaoNFSManualCancelada = new Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada
                {
                    CargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual,
                    LancamentoNFSManual = lancamentoNFSManual
                };
                repCargaDocumentoParaEmissaoNFSManualCancelada.Inserir(cargaDocumentoParaEmissaoNFSManualCancelada);
            }

            List<Dominio.Entidades.NFSe> notasVinculadas = (from obj in lancamentoNFSManual.Documentos where obj.DocumentosNFSe != null select obj.DocumentosNFSe.NFSe).Distinct().ToList();
            foreach (Dominio.Entidades.NFSe nota in notasVinculadas)
            {
                nota.Status = Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual;
                repNFSe.Atualizar(nota);
            }

            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
            repositorioLancamentoNFSManualDesconto.CancelarPorLancamentoNFSManual(lancamentoNFSManual.Codigo);
        }

        private static void NotificarAprovacaoFinalizada(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            bool rejeitada = (nfsManual.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Reprovada);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: nfsManual.Usuario,
                usuarioGerouNotificacao: usuario,
                codigoObjeto: nfsManual.Codigo,
                URLPagina: "NFS/NFSManual",
                titulo: Localization.Resources.NFSe.NFSeManual.TituloNFSeManual,
                nota: string.Format(Localization.Resources.NFSe.NFSeManual.SolicitacaoLancamentoFoi, nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada)),
                icone: rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: unitOfWork
            );
        }

        private static void NotificarAprovador(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Dominio.Entidades.Usuario aprovador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovador,
                usuarioGerouNotificacao: null,
                codigoObjeto: nfsManual.Codigo,
                URLPagina: "NFS/NFSManual",
                titulo: "NFS Manual",
                nota: string.Format(Localization.Resources.NFSe.NFSeManual.SolicitacaoAprovacaoFinalizar, nfsManual.DadosNFS.Numero.ToString(), nfsManual.DadosNFS.Serie.Numero),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: unitOfWork
            );
        }

        private static List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Repositorio.UnitOfWork unitOfWork, out bool alcadaComRequisito)
        {
            Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaRegras = new List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaFiltrada = new List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();

            if (nfsManual.Filial != null)
            {
                //Regra por filial
                List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaRegraFilial = repRegrasAutorizacaoNFSManual.BuscarRegraPorFilial(nfsManual.Filial.Codigo, nfsManual.DataCriacao);
                listaRegras.AddRange(listaRegraFilial);
            }

            //Regra por Transportadora
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaRegraTransportadora = repRegrasAutorizacaoNFSManual.BuscarRegraPorTransportadora(nfsManual.Transportador.Codigo, nfsManual.DataCriacao);
            listaRegras.AddRange(listaRegraTransportadora);

            //Regra por Tomador
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaRegraTomador = repRegrasAutorizacaoNFSManual.BuscarRegraPorTomador(nfsManual.Tomador.CPF_CNPJ, nfsManual.DataCriacao);
            listaRegras.AddRange(listaRegraTomador);

            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaRegraValorPrestacaoServico = repRegrasAutorizacaoNFSManual.BuscarRegraPorValorPrestacaoServico(nfsManual.DadosNFS.ValorFrete, nfsManual.DataCriacao);
            listaRegras.AddRange(listaRegraValorPrestacaoServico);

            if (listaRegras.Distinct().Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorFilial)
                    {
                        bool valido = false;
                        if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Filial.Codigo == nfsManual.Filial.Codigo))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Filial.Codigo == nfsManual.Filial.Codigo))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Filial.Codigo != nfsManual.Filial.Codigo))
                            valido = true;
                        else if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Filial.Codigo != nfsManual.Filial.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTransportadora)
                    {
                        bool valido = false;
                        if (regra.RegrasTransportadora.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Transportadora.Codigo == nfsManual.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Transportadora.Codigo != nfsManual.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Transportadora.Codigo != nfsManual.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Transportadora.Codigo != nfsManual.Transportador.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTomador)
                    {
                        bool valido = false;
                        if (regra.RegrasTomador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Tomador.CPF_CNPJ == nfsManual.Tomador.CPF_CNPJ))
                            valido = true;
                        else if (regra.RegrasTomador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Tomador.CPF_CNPJ != nfsManual.Tomador.CPF_CNPJ))
                            valido = true;
                        else if (regra.RegrasTomador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Tomador.CPF_CNPJ != nfsManual.Tomador.CPF_CNPJ))
                            valido = true;
                        else if (regra.RegrasTomador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Tomador.CPF_CNPJ != nfsManual.Tomador.CPF_CNPJ))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValorPrestacaoServico)
                    {
                        bool valido = false;
                        if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Valor == nfsManual.DadosNFS.ValorFrete))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Valor == nfsManual.DadosNFS.ValorFrete))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && o.Valor != nfsManual.DadosNFS.ValorFrete))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && o.Valor != nfsManual.DadosNFS.ValorFrete))
                            valido = true;
                        if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && nfsManual.DadosNFS.ValorFrete >= o.Valor))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && nfsManual.DadosNFS.ValorFrete >= o.Valor))
                            valido = true;
                        if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && nfsManual.DadosNFS.ValorFrete <= o.Valor))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && nfsManual.DadosNFS.ValorFrete <= o.Valor))
                            valido = true;
                        if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && nfsManual.DadosNFS.ValorFrete > o.Valor))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && nfsManual.DadosNFS.ValorFrete > o.Valor))
                            valido = true;
                        if (regra.RegrasValorPrestacaoServico.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.E && nfsManual.DadosNFS.ValorFrete < o.Valor))
                            valido = true;
                        else if (regra.RegrasValorPrestacaoServico.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual.Ou && nfsManual.DadosNFS.ValorFrete < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return FiltraRegrasComRequisitos(listaFiltrada, out alcadaComRequisito);
        }

        private static void GravarLogDocumentoParaEmissaoNFSManualDuplicado(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento)
        {
            string logDocumento = "Descricao:" + (documento.Descricao ?? "")
                + ";Chave:" + (documento.Chave ?? "")
                + ";Numero:" + (documento.Numero)
                + ";Serie:" + (documento.Serie ?? "")
                + ";ValorFrete:" + documento.ValorFrete
                + ";Peso:" + documento.Peso
                + ";Carga:" + (documento.Carga?.Codigo ?? 0)
                + ";PedidoXMLNotaFiscal:" + (documento.PedidoXMLNotaFiscal?.Codigo ?? 0)
                + ";PedidoCTeParaSubContratacao:" + (documento.PedidoCTeParaSubContratacao?.Codigo ?? 0)
                + ";CTe:" + (documento.CTe?.Codigo ?? 0)
                + ";Tomador:" + (documento.Tomador?.Codigo ?? 0)
                + ";Destinatario:" + (documento.Destinatario?.Codigo ?? 0)
                + ";Remetente:" + (documento.Remetente?.Codigo ?? 0)
                + ";LocalidadePrestacao:" + (documento.LocalidadePrestacao?.Codigo ?? 0)
                + ";ModeloDocumentoFiscal:" + (documento.ModeloDocumentoFiscal?.Codigo ?? 0)
                + ";LancamentoNFSManual:" + (documento.LancamentoNFSManual?.Codigo ?? 0)
                + ";DataEmissao:" + documento.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss")
                + ";DocumentosNFSe:" + (documento.DocumentosNFSe?.Codigo ?? 0)
                + ";CargaCTe:" + (documento.CargaCTe?.Codigo ?? 0)
                + ";ValorISS:" + documento.ValorISS
                + ";ValorRetencaoISS:" + documento.ValorRetencaoISS
                + ";ValorPrestacaoServico:" + documento.ValorPrestacaoServico
                + ";BaseCalculoISS:" + documento.BaseCalculoISS
                + ";CargaOcorrencia:" + (documento.CargaOcorrencia?.Codigo ?? 0)
                + ";RateouValorFrete:" + (documento.RateouValorFrete ? "S" : "N")
                + ";CargaOrigem:" + (documento.CargaOrigem?.Codigo ?? 0)
                + ";Moeda:" + (documento.Moeda ?? MoedaCotacaoBancoCentral.Todas)
                + ";ValorCotacaoMoeda:" + (documento.ValorCotacaoMoeda ?? 0)
                + ";ValorTotalMoeda:" + (documento.ValorTotalMoeda ?? 0)
                + ";FechamentoFrete:" + (documento.FechamentoFrete?.Codigo ?? 0)
                + ";DocResidual:" + (documento.DocResidual ? "S" : "N")
                + ";NumeroPedidoCliente:" + (documento.NumeroPedidoCliente ?? "")
                + ";AlterouValorFreteNFsManual:" + (documento.AlterouValorFreteNFsManual ? "S" : "N")
                + ";CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem:" + (documento.CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem?.Codigo ?? 0)
                + ";CargaCTeComplementoInfo:" + (documento.CargaCTeComplementoInfo?.Codigo ?? 0)
                + ";PercentualAliquotaISS:" + documento.PercentualAliquotaISS
                + ";CargaPedido:" + (documento.CargaPedido?.Codigo ?? 0)
                + ";DataEnvioUltimoAlertaNFsePendente:" + (documento.DataEnvioUltimoAlertaNFsePendente?.ToString("dd/MM/yyyy HH:mm:ss") ?? "")
                + ";";

            Servicos.Log.TratarErro($"Documento duplicado: {logDocumento}", "DocumentoNFSManualDuplicado");
        }

        #endregion

        #region Métodos Públicos

        public static bool LiberarProximaPrioridadeAprovacao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual origemAprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesBloqueadas(origemAprovacao.Codigo);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(alcada => alcada.RegrasAutorizacaoNFSManual.PrioridadeAprovacao).Min();

                foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.RegrasAutorizacaoNFSManual.PrioridadeAprovacao == menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAutorizacao.Atualizar(aprovacao);

                        foreach (Dominio.Entidades.Usuario aprovador in aprovacao.RegrasAutorizacaoNFSManual.Aprovadores)
                            NotificarAprovador(origemAprovacao, aprovador, tipoServicoMultisoftware, unitOfWork);
                    }
                }

                return false;
            }

            return true;
        }

        public static bool VerificarRegrasAutorizacaoNFS(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaFiltrada = Servicos.Embarcador.NFSe.NFSManual.VerificarRegrasAutorizacao(nfsManual, unitOfWork, out bool alcadaComRequisito);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.NFSe.NFSManual.CriarRegrasAutorizacao(listaFiltrada, nfsManual, tipoServicoMultisoftware, unitOfWork);
                nfsManual.AlcadaComRequisito = alcadaComRequisito;
                repLancamentoNFSManual.Atualizar(nfsManual);

                return true;
            }

            return false;
        }

        public void VerificarPendenciasEmissaoLancamento(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            // Repositorios
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSIntegracao repLancamentoNFSIntegracao = new Repositorio.Embarcador.NFS.LancamentoNFSIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();
            // Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
            // Busca os documentos do lancamento
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from o in repLancamentoNFSManual.ConsultarPorLancamento(lancamento.Codigo, 0, 0) select o.CTe).ToList();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            // Verifica se todos foram autorizados
            bool autorizado = true;
            bool rejeicao = false;
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                // Se um dos CTes nao esta autorizado, o fluxo nao segue
                if (cte.Status != "A")
                    autorizado = false;

                // Se um dos CTes esta rejeitado, o lancamento fica como erro emissao
                if (cte.Status == "R")
                    rejeicao = true;
            }

            if (autorizado)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarCargasPorLancamento(lancamento.Codigo); //(from obj in lancamento.Documentos select obj.Carga).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    if (repositorioCargaDocumentoParaEmissaoNFSManual.ContarNFSNaoEmitidosPorCarga(carga.Codigo) == 0)
                    {
                        Log.TratarErro("Carga Ag NF setou para false " + " Carga " + carga.CodigoCargaEmbarcador + " ag = " + carga.AgNFSManual.ToString(), "AgNFSManual");

                        carga.AgNFSManual = false;
                        repositorioCarga.Atualizar(carga);
                    }
                }

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> fechamentosFrete = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarFechamentosFretePorLancamento(lancamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete in fechamentosFrete)
                {
                    if (repositorioCargaDocumentoParaEmissaoNFSManual.ContarNFSNaoEmitidosPorFechamentoFrete(fechamentoFrete.Codigo) == 0)
                    {
                        fechamentoFrete.AguardandoNFSManual = false;
                        repositorioFechamentoFrete.Atualizar(fechamentoFrete);
                    }
                }

                //todo: quando for usar no TMS é necessário implementar aqui o movimento financeiro a sumarização do CT-e e o Titulo automático.
                if (!GerarMovimentosAutorizacaoNFSeManual(out string erro, ctes, unitOfWork, tipoServicoMultisoftware, StringConexao))
                {
                    Servicos.Log.TratarErro("Falha ao gerar movimentação para o lançamento código " + lancamento.Codigo + ": " + erro);
                    lancamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.FalhaEmissao;
                }
                else
                {
                    Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(lancamento, tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(lancamento, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                    int canhotosPendentesDigitalizacao = repCanhoto.ContarCanhotosPendentesDigitalizacaoPorCTe(lancamento.CTe.Codigo);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = BuscarCargaPorCTe(lancamento.CTe, unitOfWork);

                    Servicos.Log.GravarInfo($"VerificarPendenciasEmissaoLancamento inserindo documento faturamento - Carga {carga?.Codigo ?? 0} -  CTe {lancamento?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(carga, lancamento.CTe, null, lancamento, null, null, false, false, (canhotosPendentesDigitalizacao == 0), configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                    // Altera o lancamento pra ag integracao
                    lancamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgIntegracao;
                    lancamento.GerandoIntegracoes = true;
                }

                // Atualiza lancamento
                repLancamentoNFSManual.Atualizar(lancamento);
            }
            else if (rejeicao)
            {
                // Altera o lancamento pra erro emissao
                lancamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.FalhaEmissao;

                // Atualiza lancamento
                repLancamentoNFSManual.Atualizar(lancamento);
            }

            // Integracao com SignalR
            svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamento.Codigo);

            unitOfWork.CommitChanges();
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                return repCargaCTe.BuscarCargaPorCTe(cte.Codigo);
            }
            catch
            {
                Servicos.Log.TratarErro("NFSManual - Falha ao buscar a carga pelo código CTe " + (cte?.Codigo ?? 0));
                return null;
            }
        }

        public void VerificarNFSEmCancelamento(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada repCargaDocumentoParaEmissaoNFSManualCancelada = new Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                Servicos.Embarcador.Carga.RateioNFSManual svcRateioNFSManual = new Servicos.Embarcador.Carga.RateioNFSManual();
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                if (nfsManualCancelamento.CancelouDocumentos)
                {
                    unitOfWork.Start();

                    nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.AgIntegracao;
                    nfsManualCancelamento.MotivoRejeicaoCancelamento = "";

                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                    unitOfWork.CommitChanges();

                    svcNFSManual.InformarNFSManualAtualizadoCancelamento(nfsManualCancelamento.Codigo, unitOfWork.StringConexao);
                }

                if (nfsManualCancelamento.LancamentoNFSManual.CTe != null && nfsManualCancelamento.LancamentoNFSManual.CTe.Status == "A")
                {
                    nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.CancelamentoRejeitado;
                    nfsManualCancelamento.MotivoRejeicaoCancelamento = "Não foi possível cancelar as NFS, por favor, tente novamente.";
                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                    svcNFSManual.InformarNFSManualAtualizadoCancelamento(nfsManualCancelamento.Codigo, unitOfWork.StringConexao);
                }

                if (nfsManualCancelamento.LancamentoNFSManual.CTe == null || nfsManualCancelamento.LancamentoNFSManual.CTe.Status == "C")
                {
                    svcRateioNFSManual.ReverterRateioNaEstruturaDasCargasNoLancamentoManual(nfsManualCancelamento.LancamentoNFSManual, unitOfWork);

                    nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(nfsManualCancelamento.Codigo);

                    unitOfWork.Start();

                    if (nfsManualCancelamento.LancamentoNFSManual.CTe != null &&
                        (!GerarMovimentosCancelamentoNFSeManual(out string erro, nfsManualCancelamento.LancamentoNFSManual.CTe, unitOfWork, tipoServicoMultisoftware, StringConexao)))
                    {
                        unitOfWork.Rollback();

                        nfsManualCancelamento.MotivoRejeicaoCancelamento = erro;
                        nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.CancelamentoRejeitado;

                        repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                        svcNFSManual.InformarNFSManualAtualizadoCancelamento(nfsManualCancelamento.Codigo, unitOfWork.StringConexao);

                        return;
                    }

                    Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(nfsManualCancelamento.LancamentoNFSManual, unitOfWork);

                    nfsManualCancelamento.CancelouDocumentos = true;
                    nfsManualCancelamento.GerandoIntegracoes = true;
                    nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.AgIntegracao;
                    nfsManualCancelamento.MotivoRejeicaoCancelamento = "";

                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                    nfsManualCancelamento.LancamentoNFSManual.SituacaoNoCancelamento = nfsManualCancelamento.LancamentoNFSManual.Situacao;
                    nfsManualCancelamento.LancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada;

                    if (nfsManualCancelamento.LancamentoNFSManual.CTe != null)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarDocumentoAtivoPorCTe(nfsManualCancelamento.LancamentoNFSManual.CTe.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentoFaturamentos)
                        {
                            documentoFaturamento.DataCancelamento = nfsManualCancelamento.LancamentoNFSManual.CTe.DataCancelamento;
                            documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                            repDocumentoFaturamento.Atualizar(documentoFaturamento);
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from o in nfsManualCancelamento.LancamentoNFSManual.Documentos where o.FechamentoFrete == null select o.Carga).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        carga.AgNFSManual = true;
                        repCarga.Atualizar(carga);
                    }

                    for (int i = 0; i < nfsManualCancelamento.LancamentoNFSManual.Documentos.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = nfsManualCancelamento.LancamentoNFSManual.Documentos[i];
                        cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual = null;
                        cargaDocumentoParaEmissaoNFSManual.CTe = null;
                        cargaDocumentoParaEmissaoNFSManual.RateouValorFrete = false;
                        repCargaDocumentoParaEmissaoNFSManual.Atualizar(cargaDocumentoParaEmissaoNFSManual);

                        Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada cargaDocumentoParaEmissaoNFSManualCancelada = new Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada();
                        cargaDocumentoParaEmissaoNFSManualCancelada.CargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual;
                        cargaDocumentoParaEmissaoNFSManualCancelada.LancamentoNFSManual = nfsManualCancelamento.LancamentoNFSManual;
                        repCargaDocumentoParaEmissaoNFSManualCancelada.Inserir(cargaDocumentoParaEmissaoNFSManualCancelada);
                    }

                    repLancamentoNFSManual.Atualizar(nfsManualCancelamento.LancamentoNFSManual);

                    repositorioLancamentoNFSManualDesconto.CancelarPorLancamentoNFSManual(nfsManualCancelamento.LancamentoNFSManual.Codigo);

                    unitOfWork.CommitChanges();

                    svcNFSManual.InformarNFSManualAtualizadoCancelamento(nfsManualCancelamento.Codigo, unitOfWork.StringConexao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void GerarRegistrosLancamentoManual(Dominio.Entidades.NFSe nfse, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (nfse.Documentos == null || nfse.Documentos.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            int quantidadeDocumentos = nfse.Documentos.Count();
            decimal valorRateado = Math.Round((nfse.ValorServicos / quantidadeDocumentos), 2, MidpointRounding.AwayFromZero);
            decimal diferenteRateio = nfse.ValorServicos - (valorRateado * quantidadeDocumentos);

            unitOfWork.Start();
            for (var i = 0; i < quantidadeDocumentos; i++)
            {
                Dominio.Entidades.DocumentosNFSe documento = nfse.Documentos[i];
                Dominio.Entidades.Cliente remetenteNFe = null;
                Dominio.Entidades.Cliente destinatarioNFe = null;

                int.TryParse(documento.Numero, out int numero);
                List<Dominio.Entidades.DocumentosNFSe> listaDocumentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);

                if (listaDocumentos != null && listaDocumentos.Count > 0 && !string.IsNullOrWhiteSpace(listaDocumentos.FirstOrDefault().Emitente))
                    remetenteNFe = repCliente.BuscarPorCPFCNPJ(double.Parse(listaDocumentos.FirstOrDefault().Emitente));

                if (listaDocumentos != null && listaDocumentos.Count > 0 && !string.IsNullOrWhiteSpace(listaDocumentos.FirstOrDefault().Destino))
                    destinatarioNFe = repCliente.BuscarPorCPFCNPJ(double.Parse(listaDocumentos.FirstOrDefault().Destino));

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumento = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual()
                {
                    DocumentosNFSe = documento,
                    Carga = carga,
                    CargaOrigem = carga,
                    Tomador = nfse.Tomador != null ? repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Tomador.CPF_CNPJ)) : null,
                    Remetente = remetenteNFe ?? repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Tomador.CPF_CNPJ)),
                    Destinatario = destinatarioNFe ?? repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Tomador.CPF_CNPJ)),
                    LocalidadePrestacao = nfse.LocalidadePrestacaoServico,
                    Chave = documento.Chave,
                    Numero = numero,
                    Serie = documento.Serie,
                    DataEmissao = documento.DataEmissao.Value,
                    ValorFrete = valorRateado + (i == 0 ? diferenteRateio : 0), // Adicionar a diferença rateio no primeiro complementendo
                    ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55"),
                    Descricao = ""
                };
                Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaDocumento, repCargaDocumentoParaEmissaoNFSManual, unitOfWork);
            }
            unitOfWork.CommitChanges();
        }

        public static byte[] GerarImpressaoRelacaoDocumentos(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            return ReportRequest.WithType(ReportType.RelacaoDocumentosNFSManual)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigolancamentoNFSManual", lancamentoNFSManual.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public static void CalcularValores(Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarDocumentosNFSManual(dadosNFSManual.Codigo);

            if (documentos != null && documentos.Count > 0)
            {
                decimal valorTotalFreteBruto = documentos.Sum(obj => obj.ValorFrete);
                decimal valorTotalFrete = valorTotalFreteBruto - dadosNFSManual.ValorDescontos;

                dadosNFSManual.ValorFrete = valorTotalFrete;
                dadosNFSManual.ValorReceber = valorTotalFrete;
                dadosNFSManual.ValorTotalMoeda = documentos.Sum(o => o.ValorTotalMoeda ?? 0m);
            }
            else
            {
                dadosNFSManual.ValorFrete = 0m;
                dadosNFSManual.ValorReceber = 0m;
                dadosNFSManual.ValorTotalMoeda = 0m;
            }
        }

        public static void CalcularRetencaoISS(Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS)
        {
            decimal percentualRetencao = dadosNFS.PercentualRetencao;

            if (percentualRetencao > 0m && percentualRetencao <= 100m)
            {
                decimal valorISS = dadosNFS.ValorISS;
                decimal valorRetencao = valorISS * (percentualRetencao / 100);

                dadosNFS.ValorRetido = Math.Round(valorRetencao, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                dadosNFS.PercentualRetencao = 0m;
                dadosNFS.ValorRetido = 0m;
            }
        }

        public static void CalcularISS(Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS)
        {
            decimal aliquota = dadosNFS.AliquotaISS;
            decimal baseCalculo = dadosNFS.ValorFrete;
            bool incluirBC = dadosNFS.IncluirISSBC;

            decimal valorISS = baseCalculo * (aliquota / 100m);

            if (incluirBC)
            {
                baseCalculo += (aliquota > 0m ? ((baseCalculo / ((100m - aliquota) / 100m)) - baseCalculo) : 0m);
                valorISS = baseCalculo * (aliquota / 100m);
            }


            if (dadosNFS.TipoArredondamentoISS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual.ParaCima)
                valorISS = valorISS.RoundUp(2);
            else if (dadosNFS.TipoArredondamentoISS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual.ParaBaixo)
                valorISS = valorISS.RoundDown(2);

            dadosNFS.ValorISS = valorISS;
            dadosNFS.ValorBaseCalculo = Math.Round(baseCalculo, 2, MidpointRounding.AwayFromZero);

            CalcularRetencaoISS(dadosNFS);
        }

        /* VerificarSituacaoOcorrencia
         * Verificar a situacao da ocorrencia
         * Itera todas as regras da ocorrencia, se uma estiver rejeitada, rejeita a ocorrencia
         * Caso o numero minimo de aprovadores foi alcancado, regra esta aprovada
         * Caso o numero de pendentes for menor que o numero minimo, a carga automaticamente é rejeitada
         */
        public static void VerificarSituacaoNFS(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Usuario usuario)
        {
            try
            {
                if (lancamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao)
                    return;

                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();
                List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> regras = repLancamentoNFSAutorizacao.BuscarRegrasLancamentoDesbloqueadas(lancamento.Codigo);
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regra in regras)
                {
                    int aprovacoes = repLancamentoNFSAutorizacao.ContarAprovacoesLancamento(lancamento.Codigo, regra.Codigo);
                    int rejeitadas = repLancamentoNFSAutorizacao.ContarRejeitadas(lancamento.Codigo, regra.Codigo);
                    int necessariosParaAprovar = regra.NumeroAprovadores;

                    if (rejeitadas > 0)
                        rejeitada = true;

                    if (aprovacoes < necessariosParaAprovar)
                        aprovada = false;
                }

                if (aprovada || rejeitada)
                {
                    if (aprovada)
                    {
                        if (LiberarProximaPrioridadeAprovacao(lancamento, tipoServicoMultisoftware, unitOfWork))
                            lancamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgEmissao;
                    }
                    else
                    {
                        lancamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Reprovada;

                        LiberarParaNovaNFS(lancamento, unitOfWork);
                    }

                    repLancamentoNFSManual.Atualizar(lancamento);

                    if (lancamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao)
                        NotificarAprovacaoFinalizada(lancamento, usuario, tipoServicoMultisoftware, unitOfWork);

                    // Integracao com SignalR
                    svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamento.Codigo);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        /* EfetuarAprovacao
         * Aprova a autorizacao da carga
         */
        public static void EfetuarAprovacao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao solicitacao, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

            solicitacao.Data = DateTime.Now;
            solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada;

            // Atualiza os dados
            repLancamentoNFSAutorizacao.Atualizar(solicitacao);

            // Faz verificacao se a carga esta aprovada
            if (verificarSeEstaAprovado)
                VerificarSituacaoNFS(solicitacao.LancamentoNFSManual, unitOfWork, tipoServicoMultisoftware, stringConexao, usuario);

            // Notifica usuario que criou a ocorrencia
            NotificarAlteracao(true, solicitacao.LancamentoNFSManual, unitOfWork, tipoServicoMultisoftware, stringConexao, usuario);
        }

        public static bool IsPermitirAprovacaoOuReprovacaoNFS(Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao solicitacao, int codigoUsuario)
        {
            return !solicitacao.Bloqueada && (solicitacao.Usuario != null) && (solicitacao.Usuario.Codigo == codigoUsuario) && (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente);
        }

        /* NotificarAlteracao
         * Envia notificacao para o autor da ocorrencia
         */
        public static void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Usuario usuario)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string mensagem = string.Format(Localization.Resources.NFSe.NFSeManual.UsuarioSolicitacaoNota, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), lancamento.DadosNFS.Numero.ToString(), lancamento.DadosNFS.Serie.Numero);
                serNotificacao.GerarNotificacao(lancamento.Usuario, usuario, lancamento.Codigo, "NFS/NFSManual", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public void EnviarEmailTransportadorReprovacaoNFS(List<int> codigosLancamentoNFSsManuais)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(unitOfWork);

            foreach (int codigo in codigosLancamentoNFSsManuais)
            {
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repositorioLancamentoNFSManual.BuscarPorCodigo(codigo);

                if (lancamentoNFSManual == null)
                    continue;

                StringBuilder corpoEmail = new StringBuilder();

                corpoEmail.AppendLine(string.Format(Localization.Resources.NFSe.NFSeManual.PrezadoCNPJTransportador, lancamentoNFSManual.Transportador.CNPJ_Formatado, lancamentoNFSManual.Transportador.Descricao)).AppendLine()
                    .AppendLine(string.Format(Localization.Resources.NFSe.NFSeManual.NFSEnviadaAprovacaoReprovadaVerifiqueMotivoRefaçaProcesso, lancamentoNFSManual.DadosNFS?.Numero.ToString() ?? string.Empty));

                Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = $"A NFS {(lancamentoNFSManual.DadosNFS?.Numero.ToString() ?? string.Empty)} anexada foi reprovada.",
                    Empresa = lancamentoNFSManual.Transportador,
                    Mensagem = corpoEmail.ToString(),
                    NotificarSomenteEmailPrincipal = true
                };

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
            }
        }

        public void VerificarNFSePendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoNFSeManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(TipoConfiguracaoAlerta.PendenciaNfsManual);

            if (configuracaoAlerta == null)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoSemNFsManual = repositorioCargaDocumentoNFSeManual.BuscarPorNfseSemDocumentos(configuracaoAlerta.DiasRepetirAlerta);

            if (cargaDocumentoParaEmissaoSemNFsManual.Count > 0)
                EnviarEmailAlertarTransportadorPendenciaNFS(cargaDocumentoParaEmissaoSemNFsManual, configuracaoAlerta, unitOfWork);
        }

        private void EnviarEmailAlertarTransportadorPendenciaNFS(List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoSemNFsManual, Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoNFSeManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                DateTime dataAtual = DateTime.Now.Date;

                List<Dominio.Entidades.Empresa> transportadores = cargaDocumentoParaEmissaoSemNFsManual.Select(x => x.Carga.Empresa).Distinct().ToList();

                foreach (Dominio.Entidades.Empresa transportador in transportadores)
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosPendentes = cargaDocumentoParaEmissaoSemNFsManual.Where(x => x.Carga.Empresa == transportador).ToList();
                    System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();

                    string assuntoEmail = $"NFSe Manual pendente de lançamento{transportador.RazaoSocial}";
                    corpoEmail.Append("<span style=\"width: 100%; display: inline-block\">Olá, </span>");
                    corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">Prezado(a) Sr(a) {transportador.RazaoSocial}, Até o momento, você possui {documentosPendentes.Count} pendências no MultiEmbarcador, conforme relatório abaixo: </span>");
                    corpoEmail.Append("<div style=\"margin: 30px 0;\">");
                    corpoEmail.Append("<table style=\"border: 1px solid #b9b5b5; border-collapse: collapse;\">");
                    corpoEmail.Append("<thead style=\"background-color: #d9e1f2; color: black;\">");
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Número Carga </th>");
                    corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Transportador  </th>");
                    corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Número NFSe </th>");
                    corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Destinatário </th>");
                    corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Tomador </th>");
                    corpoEmail.Append("</tr>");
                    corpoEmail.Append("</thead>");
                    corpoEmail.Append("<tbody>");

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual nfsmanualCarga in documentosPendentes)
                    {
                        corpoEmail.Append("<tr>");
                        corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{nfsmanualCarga.Carga.CodigoCargaEmbarcador ?? ""}</td>");
                        corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{transportador.RazaoSocial ?? ""}</td>");
                        corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{nfsmanualCarga.Numero.ToString() ?? ""}</td>");
                        corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{nfsmanualCarga.Destinatario.Nome ?? ""}</td>");
                        corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{nfsmanualCarga.Tomador.Nome ?? ""}</td>");
                        corpoEmail.Append("</tr>");
                    }
                    corpoEmail.Append("</tbody>");
                    corpoEmail.Append("</table>");

                    string mensagemEmailNfsPendente = corpoEmail.ToString();

                    foreach (Dominio.Entidades.Usuario usuario in configuracaoAlerta.Usuarios)
                        Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, usuario.Email, null, null, assuntoEmail, mensagemEmailNfsPendente, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);

                    if (configuracaoAlerta.AlertarTransportador)
                        Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, transportador.Email ?? string.Empty, null, null, assuntoEmail, mensagemEmailNfsPendente, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual nfsmanualCarga in cargaDocumentoParaEmissaoSemNFsManual)
                {
                    nfsmanualCarga.DataEnvioUltimoAlertaNFsePendente = dataAtual;
                    repositorioCargaDocumentoNFSeManual.Atualizar(nfsmanualCarga);

                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        public static void ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento, Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual repConfiguracaoNFSeManual = new Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual configuracaoNFSeManual = repConfiguracaoNFSeManual.BuscarConfiguracaoPadrao();

            if(!(configuracaoNFSeManual?.ValidarExistenciaParaInserirNFSe ?? false))
            {
                repositorioCargaDocumentoParaEmissaoNFSManual.Inserir(documento);
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaExistenciaDocumentoNFSManual filtroPesquisaExistenciaDocumentoNFSManual = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaExistenciaDocumentoNFSManual()
            {
                CodigoDocumentoNFSe = documento.DocumentosNFSe?.Codigo ?? 0,
                CodigoCarga = documento.Carga?.Codigo ?? 0,
                ValorFrete = documento.ValorFrete,
                Numero = documento.Numero,
                CodigoXMLNotaFiscal = documento.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Codigo ?? 0,
                CodigoCTe = documento.CTe?.Codigo ?? 0,
                Chave = documento.Chave ?? "",
                CodigoModeloDocumentoFiscal = documento?.ModeloDocumentoFiscal?.Codigo ?? 0,
                CodigoPedidoCTeParaSubContratacao = documento?.PedidoCTeParaSubContratacao?.Codigo ?? 0,
                CodigoCargaOcorrencia = documento?.CargaOcorrencia?.Codigo ?? 0,
                DataEmissao = documento?.DataEmissao ?? DateTime.Now.Date,
                Serie = documento?.Serie ?? ""
            };

            try
            {
                if ((documento.LancamentoNFSManual?.Codigo ?? 0) == 0
                    && repositorioCargaDocumentoParaEmissaoNFSManual.ExisteDocumentoPendente(filtroPesquisaExistenciaDocumentoNFSManual))
                {
                    GravarLogDocumentoParaEmissaoNFSManualDuplicado(documento);
                    return;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha na validação da carga {filtroPesquisaExistenciaDocumentoNFSManual.CodigoCarga} e CTe {filtroPesquisaExistenciaDocumentoNFSManual.CodigoCTe}:" + ex.Message, "DocumentoNFSManualDuplicado");
            }

            repositorioCargaDocumentoParaEmissaoNFSManual.Inserir(documento);
        }

        #endregion
    }
}
