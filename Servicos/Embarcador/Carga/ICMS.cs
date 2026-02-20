using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class ICMS : ServicoBase
    {
        #region Construtores

        public ICMS() : base() { }        

        public ICMS(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #endregion

        #region Métodos Privados

        private bool IncluirPedagioBaseCalculoIcmsPorRegraEstado(Dominio.Entidades.Empresa empresa, string siglaEstadoOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            TipoInclusaoPedagioBaseCalculoICMS tipoInclusaoICMSPedagio = empresa?.TipoInclusaoPedagioBaseCalculoICMS ?? TipoInclusaoPedagioBaseCalculoICMS.UsarPadrao;

            if (tipoInclusaoICMSPedagio == TipoInclusaoPedagioBaseCalculoICMS.SempreIncluir)
                return true;

            if (tipoInclusaoICMSPedagio == TipoInclusaoPedagioBaseCalculoICMS.NuncaIncluir)
                return false;

            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repositorioPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo pedagioEstadoBaseCalculo = repositorioPedagioEstadoBaseCalculo.BuscarPorEstado(siglaEstadoOrigem);

            return (pedagioEstadoBaseCalculo?.IncluiBaseICMS ?? true);
        }

        private decimal ObterAliquotaInternaDifal(Dominio.Entidades.Estado ufEmitente, Dominio.Entidades.Estado ufInicioPrestacao, Dominio.Entidades.Estado ufTerminoPrestacao, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork, bool calcularInclusaoICMSAliquotaInterna, string cst, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente expedidor)
        {
            if (ufEmitente == null || ufInicioPrestacao == null || ufTerminoPrestacao == null || tomador == null || !calcularInclusaoICMSAliquotaInterna)
                return 0;

            bool naoCalcularDIFALParaCSTNaoTributavel = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoCalcularDIFALParaCSTNaoTributavel.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoCalcularDIFALParaCSTNaoTributavel.Value : false;

            if (naoCalcularDIFALParaCSTNaoTributavel && !string.IsNullOrWhiteSpace(cst) && (cst == "40" || cst == "41" || cst == "51" || cst == "040" || cst == "041" || cst == "051"))
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && tomador != null && remetente != null && tomador.CPF_CNPJ == remetente.CPF_CNPJ)
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && tomador != null && expedidor != null && tomador.CPF_CNPJ == expedidor.CPF_CNPJ)
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && (tomador?.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS || tomador?.CPF_CNPJ == expedidor?.CPF_CNPJ || tomador?.CPF_CNPJ == remetente?.CPF_CNPJ))
                return 0;

            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);

            Dominio.Entidades.Aliquota aliquotaInterna = repAliquota.BuscarParaCalculoDoICMS(ufEmitente.Sigla, ufTerminoPrestacao.Sigla, ufTerminoPrestacao.Sigla, 7);
            Dominio.Entidades.Aliquota aliquotaInterEstadual = repAliquota.BuscarParaCalculoDoICMS(ufEmitente.Sigla, ufInicioPrestacao.Sigla, ufInicioPrestacao.Sigla, 7);

            if (ufInicioPrestacao.Sigla != ufTerminoPrestacao.Sigla && tomador != null && (tomador.IE_RG == "" || tomador.IE_RG == "ISENTO"))
            {
                if (aliquotaInterna != null && aliquotaInterEstadual != null)
                {
                    decimal percentualDifal = aliquotaInterna.Percentual - aliquotaInterEstadual.Percentual;
                    if (percentualDifal > 0 || naoCalcularDIFALParaCSTNaoTributavel)
                    {
                        return aliquotaInterna.Percentual;
                    }
                    else
                        return 0;
                }
            }

            return 0;
        }

        private async Task<decimal> ObterAliquotaInternaDifalAsync(Dominio.Entidades.Estado ufEmitente, Dominio.Entidades.Estado ufInicioPrestacao, Dominio.Entidades.Estado ufTerminoPrestacao, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork, bool calcularInclusaoICMSAliquotaInterna, string cst, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente expedidor)
        {
            if (ufEmitente == null || ufInicioPrestacao == null || ufTerminoPrestacao == null || tomador == null || !calcularInclusaoICMSAliquotaInterna)
                return 0;

            bool naoCalcularDIFALParaCSTNaoTributavel = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoCalcularDIFALParaCSTNaoTributavel.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoCalcularDIFALParaCSTNaoTributavel.Value : false;

            if (naoCalcularDIFALParaCSTNaoTributavel && !string.IsNullOrWhiteSpace(cst) && (cst == "40" || cst == "41" || cst == "51" || cst == "040" || cst == "041" || cst == "051"))
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && tomador != null && remetente != null && tomador.CPF_CNPJ == remetente.CPF_CNPJ)
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && tomador != null && expedidor != null && tomador.CPF_CNPJ == expedidor.CPF_CNPJ)
                return 0;

            if (naoCalcularDIFALParaCSTNaoTributavel && (tomador?.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS || tomador?.CPF_CNPJ == expedidor?.CPF_CNPJ || tomador?.CPF_CNPJ == remetente?.CPF_CNPJ))
                return 0;

            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);

            Dominio.Entidades.Aliquota aliquotaInterna = await repAliquota.BuscarParaCalculoDoICMSAsync(ufEmitente.Sigla, ufTerminoPrestacao.Sigla, ufTerminoPrestacao.Sigla, 7);
            Dominio.Entidades.Aliquota aliquotaInterEstadual = await repAliquota.BuscarParaCalculoDoICMSAsync(ufEmitente.Sigla, ufInicioPrestacao.Sigla, ufInicioPrestacao.Sigla, 7);

            if (ufInicioPrestacao.Sigla != ufTerminoPrestacao.Sigla && tomador != null && (tomador.IE_RG == "" || tomador.IE_RG == "ISENTO"))
            {
                if (aliquotaInterna != null && aliquotaInterEstadual != null)
                {
                    decimal percentualDifal = aliquotaInterna.Percentual - aliquotaInterEstadual.Percentual;
                    if (percentualDifal > 0 || naoCalcularDIFALParaCSTNaoTributavel)
                    {
                        return aliquotaInterna.Percentual;
                    }
                    else
                        return 0;
                }
            }

            return 0;
        }

        //modelo conexão base de dados
        //private Dominio.Entidades.Embarcador.ICMS.RegraICMS ObterRegraICMS(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool filtrarTodasRegras, int codigoTipoOperacao, Dominio.Entidades.Cliente destinatarioExportacao)
        //{
        //    Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = null;
        //    int grupoRemetente = remetente.GrupoPessoas != null ? remetente.GrupoPessoas.Codigo : 0;
        //    int grupoDestinatario = destinatario.GrupoPessoas != null ? destinatario.GrupoPessoas.Codigo : 0;
        //    int grupoTomador = tomador.GrupoPessoas != null ? tomador.GrupoPessoas.Codigo : 0;
        //    int codigoProdutoEmbarcador = produtoEmbarcador != null ? produtoEmbarcador.Codigo : 0;

        //    Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

        //    int regrasCadastradas = repRegraICMS.ContarRegrasCadastradas(filtrarTodasRegras ? 0 : empresa.Codigo, DateTime.Now, 0);

        //    Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstancia(unitOfWork);

        //    if (regrasCadastradas > 5)
        //    {
        //        if (empresa != null)
        //        {

        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEmpresa = repRegraICMS.FiltrarPorEmpresa(empresa.Codigo);
        //            if (regrasEmpresa.Count > 0)
        //                regraICMS = RetornarRegraValida(regrasEmpresa, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);

        //            if (regraICMS == null && !string.IsNullOrWhiteSpace(empresa.Setor))
        //            {
        //                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasSetor = repRegraICMS.FiltrarPorSetorEmpresa(empresa.Setor);
        //                if (regrasSetor.Count > 0)
        //                    regraICMS = RetornarRegraValida(regrasSetor, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);

        //            }
        //        }

        //        List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasParticipantes = repRegraICMS.FiltrarPorParticipantes(remetente.CPF_CNPJ, destinatario.CPF_CNPJ, tomador.CPF_CNPJ, grupoRemetente, grupoDestinatario, grupoTomador);
        //        if (regrasParticipantes.Count > 0)
        //            regraICMS = RetornarRegraValida(regrasParticipantes, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);

        //        //Quando tem regra de ICMS com destino EX, busca regra sempre utilizando estado do Destinatário 
        //        if (regraICMS == null && destinatarioExportacao != null && destinatarioExportacao.Localidade != null && destinatarioExportacao.Localidade.Estado.Sigla == "EX")
        //        {
        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEstado = repRegraICMS.FiltrarPorEstados(empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla, origem.Estado.Sigla, destinatarioExportacao.Localidade.Estado.Sigla, tomador.Localidade.Estado.Sigla);
        //            if (regrasEstado.Count > 0)
        //                regraICMS = RetornarRegraValida(regrasEstado, empresa, remetente, destinatarioExportacao, tomador, origem, destinatarioExportacao.Localidade, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);
        //        }

        //        if (regraICMS == null)
        //        {
        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEstado = repRegraICMS.FiltrarPorEstados(empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla, origem.Estado.Sigla, destino.Estado.Sigla, tomador.Localidade.Estado.Sigla);
        //            if (regrasEstado.Count > 0)
        //                regraICMS = RetornarRegraValida(regrasEstado, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);
        //        }

        //        if (regraICMS == null)
        //        {
        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasAtividade = repRegraICMS.FiltrarPorAtividade(remetente.Atividade.Codigo, destinatario.Atividade.Codigo, tomador.Atividade.Codigo);
        //            if (regrasAtividade.Count > 0)
        //                regraICMS = RetornarRegraValida(regrasAtividade, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);
        //        }

        //        if (regraICMS == null)
        //        {
        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasProdutoEmbarcador = repRegraICMS.FiltrarPorProdutoEmbarcador(codigoProdutoEmbarcador);
        //            if (regrasProdutoEmbarcador.Count > 0)
        //                regraICMS = RetornarRegraValida(regrasProdutoEmbarcador, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);
        //        }
        //    }
        //    else
        //    {
        //        if (regrasCadastradas > 0)
        //        {
        //            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras = repRegraICMS.FiltrarTodasRegrasCadastradas(filtrarTodasRegras ? 0 : empresa.Codigo, DateTime.Now, 0);
        //            if (regras.Count > 0)
        //                regraICMS = RetornarRegraValida(regras, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao);
        //        }
        //    }
        //    return regraICMS;
        //}

        private Dominio.Entidades.Embarcador.ICMS.RegraICMS ObterRegraICMS(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool filtrarTodasRegras, int codigoTipoOperacao, Dominio.Entidades.Cliente destinatarioExportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal, Dominio.Enumeradores.TipoServico? tipoServico, Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento, string numeroProposta, int codigoTipoDeCarga)
        {
            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = null;
            int grupoRemetente = remetente.GrupoPessoas != null ? remetente.GrupoPessoas.Codigo : 0;
            int grupoDestinatario = destinatario.GrupoPessoas != null ? destinatario.GrupoPessoas.Codigo : 0;
            int grupoTomador = tomador.GrupoPessoas != null ? tomador.GrupoPessoas.Codigo : 0;
            int codigoProdutoEmbarcador = produtoEmbarcador != null ? produtoEmbarcador.Codigo : 0;

            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork);

            IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> queryListaRegras = regrasCalculoImpostos.ObterRegrasICMS().AsQueryable();

            if (regrasCalculoImpostos.ObterRegrasICMS().Count() > 5)
            {
                if (empresa != null)
                {
                    //Quando tem regra de ICMS com destino EX, busca regra sempre utilizando estado do Destinatário 
                    if (regraICMS == null && destinatarioExportacao != null && destinatarioExportacao.Localidade != null && destinatarioExportacao.Localidade.Estado.Sigla == "EX")
                    {
                        List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEstado = repRegraICMS.FiltrarPorEstados(empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla, origem.Estado.Sigla, destinatarioExportacao.Localidade.Estado.Sigla, tomador.Localidade.Estado.Sigla, queryListaRegras);
                        if (regrasEstado.Count > 0)
                            regraICMS = RetornarRegraValida(regrasEstado, empresa, remetente, destinatarioExportacao, tomador, origem, destinatarioExportacao.Localidade, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                    }

                    if (regraICMS == null)
                    {
                        List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEmpresa = repRegraICMS.FiltrarPorEmpresa(empresa.Codigo, queryListaRegras);
                        if (regrasEmpresa.Count > 0)
                            regraICMS = RetornarRegraValida(regrasEmpresa, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);

                    }
                    if (regraICMS == null && !string.IsNullOrWhiteSpace(empresa.Setor))
                    {
                        List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasSetor = repRegraICMS.FiltrarPorSetorEmpresa(empresa.Setor, queryListaRegras);
                        if (regrasSetor.Count > 0)
                            regraICMS = RetornarRegraValida(regrasSetor, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);

                    }
                }

                if (regraICMS == null && codigoTipoOperacao > 0)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasTipoOperacao = repRegraICMS.FiltrarPorTipoOperacao(codigoTipoOperacao, queryListaRegras);
                    if (regrasTipoOperacao.Count > 0)
                        regraICMS = RetornarRegraValida(regrasTipoOperacao, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }

                if (regraICMS == null && !string.IsNullOrWhiteSpace(numeroProposta))
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasSetor = repRegraICMS.FiltrarPorNumeroProposta(numeroProposta, queryListaRegras);
                    if (regrasSetor.Count > 0)
                        regraICMS = RetornarRegraValida(regrasSetor, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);

                }

                if (regraICMS == null)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasParticipantes = repRegraICMS.FiltrarPorParticipantes(remetente.CPF_CNPJ, destinatario.CPF_CNPJ, tomador.CPF_CNPJ, grupoRemetente, grupoDestinatario, grupoTomador, queryListaRegras);
                    if (regrasParticipantes.Count > 0)
                        regraICMS = RetornarRegraValida(regrasParticipantes, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }

                //Quando tem regra de ICMS com destino EX, busca regra sempre utilizando estado do Destinatário 
                if (regraICMS == null && destinatarioExportacao != null && destinatarioExportacao.Localidade != null && destinatarioExportacao.Localidade.Estado.Sigla == "EX")
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEstado = repRegraICMS.FiltrarPorEstados(empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla, origem.Estado.Sigla, destinatarioExportacao.Localidade.Estado.Sigla, tomador.Localidade.Estado.Sigla, queryListaRegras);
                    if (regrasEstado.Count > 0)
                        regraICMS = RetornarRegraValida(regrasEstado, empresa, remetente, destinatarioExportacao, tomador, origem, destinatarioExportacao.Localidade, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }

                if (regraICMS == null)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasEstado = repRegraICMS.FiltrarPorEstados(empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla, origem.Estado.Sigla, destino.Estado.Sigla, tomador.Localidade.Estado.Sigla, queryListaRegras);
                    if (regrasEstado.Count > 0)
                        regraICMS = RetornarRegraValida(regrasEstado, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }

                if (regraICMS == null)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasAtividade = repRegraICMS.FiltrarPorAtividade(remetente.Atividade.Codigo, destinatario.Atividade.Codigo, tomador.Atividade.Codigo, queryListaRegras);
                    if (regrasAtividade.Count > 0)
                        regraICMS = RetornarRegraValida(regrasAtividade, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }

                if (regraICMS == null)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasProdutoEmbarcador = repRegraICMS.FiltrarPorProdutoEmbarcador(codigoProdutoEmbarcador, queryListaRegras);
                    if (regrasProdutoEmbarcador.Count > 0)
                        regraICMS = RetornarRegraValida(regrasProdutoEmbarcador, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                }
            }
            else
            {
                if (regrasCalculoImpostos.ObterRegrasICMS().Count() > 0)
                {
                    List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras = repRegraICMS.FiltrarTodasRegrasCadastradas(filtrarTodasRegras ? 0 : empresa.Codigo, DateTime.Now, 0, queryListaRegras);
                    if (regras.Count > 0)
                    {
                        if (destinatarioExportacao != null && destinatarioExportacao.Localidade != null && destinatarioExportacao.Localidade.Estado.Sigla == "EX")
                            regraICMS = RetornarRegraValida(regras, empresa, remetente, destinatarioExportacao, tomador, origem, destinatarioExportacao.Localidade, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                        if (regraICMS == null)
                            regraICMS = RetornarRegraValida(regras, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga, unitOfWork);
                    }
                }
            }
            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS ObterRegraICMSMultiCTe(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;
            int grupoRemetente = remetente != null && remetente.GrupoPessoas != null ? remetente.GrupoPessoas.Codigo : 0;
            int grupoDestinatario = destinatario != null && destinatario.GrupoPessoas != null ? destinatario.GrupoPessoas.Codigo : 0;
            int grupoTomador = tomador != null && tomador.GrupoPessoas != null ? tomador.GrupoPessoas.Codigo : 0;
            int codigoProdutoEmbarcador = produtoEmbarcador != null ? produtoEmbarcador.Codigo : 0;

            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

            int regrasCadastradas = repRegraICMS.ContarRegrasCadastradas(empresa.Codigo, null, empresa.EmpresaPai != null ? empresa.EmpresaPai.Codigo : 0);

            if (regrasCadastradas > 0)
            {
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras = repRegraICMS.FiltrarTodasRegrasCadastradas(empresa.Codigo, null, empresa.EmpresaPai != null ? empresa.EmpresaPai.Codigo : 0);
                if (regras.Count > 0)
                    regraICMS = RetornarRegraValidaMultiCTe(regras, empresa, remetente, destinatario, tomador, origem, destino, grupoDestinatario, grupoRemetente, grupoTomador, codigoProdutoEmbarcador, null);
            }
            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS ObterRegraICMSMultiTMS(Dominio.Entidades.Atividade atividade, Dominio.Entidades.Estado ufEmissor, Dominio.Entidades.Estado ufOrigem, Dominio.Entidades.Estado ufDestino, Dominio.Entidades.Estado ufTomador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;

            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

            int regrasCadastradas = repRegraICMS.ContarRegrasCadastradas(atividade?.Codigo ?? 0, ufEmissor?.Sigla ?? "", ufOrigem?.Sigla ?? "", ufDestino?.Sigla ?? "", ufTomador?.Sigla ?? "", DateTime.Now.Date);

            if (regrasCadastradas > 0)
            {
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras = repRegraICMS.FiltrarTodasRegrasCadastradas(atividade?.Codigo ?? 0, ufEmissor?.Sigla ?? "", ufOrigem?.Sigla ?? "", ufDestino?.Sigla ?? "", ufTomador?.Sigla ?? "", DateTime.Now.Date);
                if (regras.Count > 0)
                    regraICMS = RetornarRegraValidaMultiTMS(regras, atividade, ufEmissor, ufOrigem, ufDestino, ufTomador);
            }
            return regraICMS;
        }

        private Dominio.Enumeradores.TipoPagamentoRegraICMS? ObterTipoPagamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento = null;
            if (cargaPedido != null && cargaPedido.Pedido != null)
            {
                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    tipoPagamento = Dominio.Enumeradores.TipoPagamentoRegraICMS.A_Pagar;
                else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                    tipoPagamento = Dominio.Enumeradores.TipoPagamentoRegraICMS.Outros;
                else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    tipoPagamento = Dominio.Enumeradores.TipoPagamentoRegraICMS.Pago;
            }
            return tipoPagamento;
        }

        private Dominio.Enumeradores.TipoServico? ObterTipoServicoPorTipoContratacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga? tipoContratacaoCarga)
        {
            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;
            if (tipoContratacaoCarga != null)
            {
                switch (tipoContratacaoCarga)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal:
                        tipoServico = Dominio.Enumeradores.TipoServico.Normal;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho:
                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario:
                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada:
                        tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio:
                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro:
                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                        break;
                    default:
                        break;
                }
            }
            return tipoServico;
        }

        private Dominio.Enumeradores.TipoServico? ObterTipoServicoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Enumeradores.TipoServico? tipoServico = null;
            if (carga != null)
            {
                if (carga.CargaSVM || carga.CargaSVMTerceiro)
                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                    tipoServico = Dominio.Enumeradores.TipoServico.Normal;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                    tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                    tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                    tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio)
                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                else if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro)
                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
            }
            return tipoServico;
        }

        private decimal ObterValorIcmsPedagioPorRegraEstado(Dominio.Entidades.Embarcador.Frete.ComponenteFreteBase componenteFrete, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo pedagioEstadoBaseCalculo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.ComponenteFreteBase> repositorioComponenteFrete = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.ComponenteFreteBase>(unitOfWork);
            TipoInclusaoPedagioBaseCalculoICMS tipoInclusaoICMSPedagio = empresa?.TipoInclusaoPedagioBaseCalculoICMS ?? TipoInclusaoPedagioBaseCalculoICMS.UsarPadrao;

            if ((tipoInclusaoICMSPedagio == TipoInclusaoPedagioBaseCalculoICMS.UsarPadrao) || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
            {
                if (componenteFrete.IncluirBaseCalculoICMS)
                {
                    componenteFrete.IncluirBaseCalculoICMS = (pedagioEstadoBaseCalculo?.IncluiBaseICMS ?? true);
                    repositorioComponenteFrete.Atualizar(componenteFrete);
                }
            }
            else if (tipoInclusaoICMSPedagio == TipoInclusaoPedagioBaseCalculoICMS.SempreIncluir)
            {
                componenteFrete.IncluirBaseCalculoICMS = true;
                repositorioComponenteFrete.Atualizar(componenteFrete);
            }
            else
            {
                componenteFrete.IncluirBaseCalculoICMS = false;
                repositorioComponenteFrete.Atualizar(componenteFrete);
            }

            return componenteFrete.IncluirBaseCalculoICMS ? componenteFrete.ValorComponente : 0m;
        }

        private Dominio.Entidades.Embarcador.ICMS.RegraICMS RetornarRegraValida(List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, int grupoDestinatario, int grupoRemetente, int grupoTomador, int codigoProdutoEmbarcador, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal, Dominio.Enumeradores.TipoServico? tipoServico, Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento, string numeroProposta, int codigoTipoDeCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraValida = null;
            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            if (regras != null && regras.Count > 0 && codigoProdutoEmbarcador > 0)
                regras = regras.OrderByDescending(o => o.ProdutosEmbarcador.Count).ToList();

            foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regra in regras)
            {
                bool valido = true;
                if (regra.Empresa != null && regra.Empresa.Codigo != (empresa != null ? empresa.Codigo : 0))
                    valido = false;
                if (!string.IsNullOrWhiteSpace(regra.SetorEmpresa) && regra.SetorEmpresa != (empresa != null ? empresa.Setor : ""))
                    valido = false;
                if (!string.IsNullOrWhiteSpace(regra.NumeroProposta) && regra.NumeroProposta != (empresa != null ? numeroProposta : ""))
                    valido = false;
                if (regra.SomenteOptanteSimplesNacional && (empresa == null || !empresa.OptanteSimplesNacional))
                    valido = false;
                if (regra.AtividadeDestinatario != null && regra.AtividadeDestinatario.Codigo != destinatario.Atividade.Codigo)
                    valido = false;
                else if (regra.AtividadeRemetente != null && regra.AtividadeRemetente.Codigo != remetente.Atividade.Codigo)
                    valido = false;
                else if (regra.AtividadeTomador != null && ((regra.AtividadeTomadorDiferente && regra.AtividadeTomador.Codigo == tomador.Atividade.Codigo) || (!regra.AtividadeTomadorDiferente && regra.AtividadeTomador.Codigo != tomador.Atividade.Codigo)))
                    valido = false;
                else if (regra.Destinatario != null && regra.Destinatario.CPF_CNPJ != destinatario.CPF_CNPJ)
                    valido = false;
                else if (regra.Remetente != null && regra.Remetente.CPF_CNPJ != remetente.CPF_CNPJ)
                    valido = false;
                else if (regra.Tomador != null && regra.Tomador.CPF_CNPJ != tomador.CPF_CNPJ)
                    valido = false;
                else if (regra.GrupoDestinatario != null && regra.GrupoDestinatario.Codigo != grupoDestinatario)
                    valido = false;
                else if (regra.GrupoRemetente != null && regra.GrupoRemetente.Codigo != grupoRemetente)
                    valido = false;
                else if (regra.GrupoTomador != null && regra.GrupoTomador.Codigo != grupoTomador)
                    valido = false;
                else if (regra.ProdutosEmbarcador != null && regra.ProdutosEmbarcador.Count > 0 && !regra.ProdutosEmbarcador.Any(o => o.Codigo == codigoProdutoEmbarcador))//else if (regra.ProdutoEmbarcador != null && regra.ProdutoEmbarcador.Codigo != codigoProdutoEmbarcador)
                    valido = false;
                else if (!regra.EstadoDestinoDiferente ? (regra.UFDestino != null && regra.UFDestino.Sigla != destino.Estado.Sigla) : (regra.UFDestino != null && regra.UFDestino.Sigla == destino.Estado.Sigla)) ///else if (regra.UFDestino != null && regra.UFDestino.Sigla != destino.Estado.Sigla)
                    valido = false;
                else if (regra.UFEmitente != null && regra.UFEmitente.Sigla != (empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla))
                    valido = false;
                else if (regra.UFEmitenteDiferente != null && regra.UFEmitenteDiferente.Sigla == (empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla))
                    valido = false;
                else if (!regra.EstadoOrigemDiferente ? (regra.UFOrigem != null && regra.UFOrigem.Sigla != origem.Estado.Sigla) : (regra.UFOrigem != null && regra.UFOrigem.Sigla == origem.Estado.Sigla))//else if (regra.UFOrigem != null && regra.UFOrigem.Sigla != origem.Estado.Sigla)
                    valido = false;
                else if (!regra.EstadoTomadorDiferente ? (regra.UFTomador != null && regra.UFTomador.Sigla != tomador.Localidade.Estado.Sigla) : (regra.UFTomador != null && regra.UFTomador.Sigla == tomador.Localidade.Estado.Sigla))
                    valido = false;
                else if (regra.TipoModal.HasValue && regra.TipoModal.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos && tipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos && regra.TipoModal != tipoModal)
                    valido = false;
                else if (regra.RegimeTributarioTomador.HasValue && ((regra.RegimeTributarioTomadorDiferente && regra.RegimeTributarioTomador == tomador.RegimeTributario) || (!regra.RegimeTributarioTomadorDiferente && regra.RegimeTributarioTomador != tomador.RegimeTributario)))
                    valido = false;
                else if (regra.TiposOperacao != null && regra.TiposOperacao.Count > 0 && !regra.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao)) //else if ((regra.TipoOperacao != null) && (regra.TipoOperacao.Codigo != codigoTipoOperacao))
                    valido = false;
                else if (regra.TiposDeCarga != null && regra.TiposDeCarga.Count > 0 && !regra.TiposDeCarga.Any(o => o.Codigo == codigoTipoDeCarga))
                    valido = false;
                else if (regra.UFOrigemIgualUFTomador && (origem.Estado.Sigla != tomador.Localidade.Estado.Sigla))
                    valido = false;
                else if (tipoServico.HasValue && regra.TipoServico.HasValue && regra.TipoServico != tipoServico.Value)
                    valido = false;
                else if (tipoPagamento.HasValue && regra.TipoPagamento.HasValue && regra.TipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoRegraICMS.Todos && regra.TipoPagamento != tipoPagamento.Value)
                    valido = false;

                if (valido)
                {
                    regraValida = regra;
                    break;
                }

            }
            return regraValida;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS RetornarRegraValidaMultiCTe(List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, int grupoDestinatario, int grupoRemetente, int grupoTomador, int codigoProdutoEmbarcador, Dominio.Enumeradores.TipoServico? tipoServico)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraValida = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            regraValida.Aliquota = -1;
            regraValida.PercentualReducaoBC = -1;
            regraValida.ValorBaseCalculoICMS = 1;
            regraValida.ValorBaseCalculoPISCOFINS = 1;

            foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regra in regras)
            {
                bool valido = true;
                if (regra.Empresa != null && regra.Empresa.Codigo != (empresa != null ? empresa.Codigo : 0))
                {
                    if ((empresa.EmpresaPai == null) || empresa.EmpresaPai.Codigo != regra.Empresa.Codigo)
                        valido = false;
                }
                if (regra.SomenteOptanteSimplesNacional && (empresa == null || !empresa.OptanteSimplesNacional))
                    valido = false;
                if (regra.AtividadeDestinatario != null && destinatario != null && regra.AtividadeDestinatario.Codigo != destinatario.Atividade.Codigo)
                    valido = false;
                else if (regra.AtividadeRemetente != null && remetente != null && remetente != null && regra.AtividadeRemetente.Codigo != remetente.Atividade.Codigo)
                    valido = false;
                else if (regra.AtividadeTomador != null && !regra.AtividadeTomadorDiferente && tomador != null && tomador != null && regra.AtividadeTomador.Codigo != tomador.Atividade.Codigo) //6350 Hercosul - Adicionado regra para quando configurado Atividade Tomador Diferente
                    valido = false;
                else if (regra.AtividadeTomador != null && regra.AtividadeTomadorDiferente && tomador != null && tomador != null && regra.AtividadeTomador.Codigo == tomador.Atividade.Codigo) //6350 Hercosul - Adicionado regra para quando configurado Atividade Tomador Diferente
                    valido = false;
                else if (regra.Destinatario != null && destinatario != null && regra.Destinatario.CPF_CNPJ != destinatario.CPF_CNPJ)
                    valido = false;
                else if (regra.Remetente != null && remetente != null && regra.Remetente.CPF_CNPJ != remetente.CPF_CNPJ)
                    valido = false;
                else if (regra.Tomador != null && tomador != null && regra.Tomador.CPF_CNPJ != tomador.CPF_CNPJ)
                    valido = false;
                else if (regra.GrupoDestinatario != null && regra.GrupoDestinatario.Codigo != grupoDestinatario)
                    valido = false;
                else if (regra.GrupoRemetente != null && regra.GrupoRemetente.Codigo != grupoRemetente)
                    valido = false;
                else if (regra.GrupoTomador != null && regra.GrupoTomador.Codigo != grupoTomador)
                    valido = false;
                else if (regra.ProdutoEmbarcador != null && regra.ProdutoEmbarcador.Codigo != codigoProdutoEmbarcador)
                    valido = false;
                else if (!regra.EstadoDestinoDiferente ? (regra.UFDestino != null && regra.UFDestino.Sigla != destino.Estado.Sigla) : (regra.UFDestino != null && regra.UFDestino.Sigla == destino.Estado.Sigla)) ///else if (regra.UFDestino != null && regra.UFDestino.Sigla != destino.Estado.Sigla)
                    valido = false;
                else if (regra.UFEmitente != null && regra.UFEmitente.Sigla != (empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla))
                    valido = false;
                else if (regra.UFEmitenteDiferente != null && regra.UFEmitenteDiferente.Sigla == (empresa != null ? empresa.Localidade.Estado.Sigla : origem.Estado.Sigla))
                    valido = false;
                else if (!regra.EstadoOrigemDiferente ? (regra.UFOrigem != null && regra.UFOrigem.Sigla != origem.Estado.Sigla) : (regra.UFOrigem != null && regra.UFOrigem.Sigla == origem.Estado.Sigla))//else if (regra.UFOrigem != null && regra.UFOrigem.Sigla != origem.Estado.Sigla)
                    valido = false;
                //else if (tomador != null && regra.UFTomador != null && regra.UFTomador.Sigla != tomador.Localidade.Estado.Sigla)
                else if (!regra.EstadoTomadorDiferente ? (tomador != null && regra.UFTomador != null && regra.UFTomador.Sigla != tomador.Localidade.Estado.Sigla) : (tomador != null && regra.UFTomador != null && regra.UFTomador.Sigla == tomador.Localidade.Estado.Sigla))
                    valido = false;
                else if (regra.TipoServico.HasValue && regra.TipoServico != Dominio.Enumeradores.TipoServico.Normal) //Adicionado para não retornar regras por tipo de serviço cadastradas no MultiEmbarcador
                    valido = false;

                if (valido)
                {
                    if ((string.IsNullOrWhiteSpace(regraValida.CST) && !string.IsNullOrWhiteSpace(regra.CST)))
                    {
                        regraValida.CST = regra.CST;
                        if (regra.CST == "60")
                            regraValida.DescontarICMSDoValorAReceber = true; // regra.DescontarICMSDoValorAReceber; //No multiCTe não é usada esta flag
                        regraValida.NaoReduzirRetencaoICMSDoValorDaPrestacao = regra.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                    }
                    if (regraValida.Aliquota <= 0 && regra.Aliquota.HasValue)
                        regraValida.Aliquota = regra.Aliquota.HasValue ? decimal.Parse(regra.Aliquota.ToString()) : -1;
                    if (regraValida.CFOP == 0 && regra.CFOP != null)
                        regraValida.CFOP = regra.CFOP.CodigoCFOP;
                    if (regraValida.PercentualReducaoBC <= 0 && regra.PercentualReducaoBC.HasValue)
                        regraValida.PercentualReducaoBC = regra.PercentualReducaoBC.HasValue ? regra.PercentualReducaoBC.Value : -1;
                    if (regra.ImprimeLeiNoCTe && !string.IsNullOrWhiteSpace(regra.DescricaoRegra))
                        regraValida.ObservacaoCTe = string.IsNullOrWhiteSpace(regraValida.ObservacaoCTe) ? regra.DescricaoRegra : string.Concat(regraValida.ObservacaoCTe, " / ", regra.DescricaoRegra);
                    if (regra.ZerarValorICMS)
                        regraValida.ValorBaseCalculoICMS = regra.ZerarValorICMS ? 0 : 1;
                    if (regra.ZerarValorICMS)
                        regraValida.ValorBaseCalculoPISCOFINS = regra.ZerarValorICMS ? 0 : 1;
                    if (regraValida.AliquotaSimples <= 0 && regra.AliquotaSimples.HasValue)
                        regraValida.AliquotaSimples = regra.AliquotaSimples.Value;
                }

            }
            return regraValida;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS RetornarRegraValidaMultiTMS(List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regras, Dominio.Entidades.Atividade atividade, Dominio.Entidades.Estado ufEmissor, Dominio.Entidades.Estado ufOrigem, Dominio.Entidades.Estado ufDestino, Dominio.Entidades.Estado ufTomador)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraValida = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            regraValida.Aliquota = -1;
            regraValida.PercentualReducaoBC = -1;
            regraValida.ValorBaseCalculoICMS = 1;
            regraValida.ValorBaseCalculoPISCOFINS = 1;

            foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regra in regras)
            {
                bool valido = true;
                if (regra.AtividadeTomador != null && !regra.AtividadeTomadorDiferente && atividade != null && regra.AtividadeTomador.Codigo != atividade.Codigo)
                    valido = false;
                else if (regra.Empresa != null && ufEmissor != null && regra.Empresa.Localidade.Estado.Sigla != ufEmissor.Sigla)
                    valido = false;
                else if (regra.Destinatario != null && ufDestino != null && regra.Destinatario.Localidade.Estado.Sigla != ufDestino.Sigla)
                    valido = false;
                else if (regra.Remetente != null && ufOrigem != null && regra.Remetente.Localidade.Estado.Sigla != ufOrigem.Sigla)
                    valido = false;
                else if (regra.Tomador != null && ufTomador != null && regra.Tomador.Localidade.Estado.Sigla != ufTomador.Sigla)
                    valido = false;
                else if (regra.UFEmitente != null && ufEmissor != null && regra.UFEmitente.Sigla != ufEmissor.Sigla)
                    valido = false;
                else if (regra.UFOrigem != null && ufOrigem != null && regra.UFOrigem.Sigla != ufOrigem.Sigla)
                    valido = false;
                else if (regra.UFDestino != null && ufDestino != null && regra.UFDestino.Sigla != ufDestino.Sigla)
                    valido = false;
                else if (regra.UFTomador != null && ufTomador != null && regra.UFTomador.Sigla != ufTomador.Sigla)
                    valido = false;

                if (valido)
                {
                    if ((string.IsNullOrWhiteSpace(regraValida.CST) && !string.IsNullOrWhiteSpace(regra.CST)))
                    {
                        regraValida.CST = regra.CST;
                        if (regra.CST == "60")
                            regraValida.DescontarICMSDoValorAReceber = true; // regra.DescontarICMSDoValorAReceber; //No multiCTe não é usada esta flag
                        regraValida.NaoReduzirRetencaoICMSDoValorDaPrestacao = regra.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                    }
                    if (regraValida.Aliquota <= 0 && regra.Aliquota.HasValue)
                        regraValida.Aliquota = regra.Aliquota.HasValue ? decimal.Parse(regra.Aliquota.ToString()) : -1;
                    if (regraValida.CFOP == 0 && regra.CFOP != null)
                        regraValida.CFOP = regra.CFOP.CodigoCFOP;
                    if (regraValida.PercentualReducaoBC <= 0 && regra.PercentualReducaoBC.HasValue)
                        regraValida.PercentualReducaoBC = regra.PercentualReducaoBC.HasValue ? regra.PercentualReducaoBC.Value : -1;
                    if (regra.ImprimeLeiNoCTe && !string.IsNullOrWhiteSpace(regra.DescricaoRegra))
                        regraValida.ObservacaoCTe = string.IsNullOrWhiteSpace(regraValida.ObservacaoCTe) ? regra.DescricaoRegra : string.Concat(regraValida.ObservacaoCTe, " / ", regra.DescricaoRegra);
                    if (regra.ZerarValorICMS)
                        regraValida.ValorBaseCalculoICMS = regra.ZerarValorICMS ? 0 : 1;
                    if (regra.ZerarValorICMS)
                        regraValida.ValorBaseCalculoPISCOFINS = regra.ZerarValorICMS ? 0 : 1;
                    if (regraValida.AliquotaSimples <= 0 && regra.AliquotaSimples.HasValue)
                        regraValida.AliquotaSimples = regra.AliquotaSimples.Value;

                    regraValida.Descricao = regra.Descricao;
                    regraValida.CodigoRegra = regra.Codigo;
                }
            }
            return regraValida;
        }

        private Dominio.Entidades.Embarcador.ICMS.RegraICMS Duplicar(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ICMS.RegraICMS repositorioTabelaFreteCliente = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSClonada = regraICMS.Clonar();

            CopiarProdutosEmbarcador(regraICMS, regraICMSClonada);
            CopiarTiposOperacao(regraICMS, regraICMSClonada);
            CopiarTiposDeCarga(regraICMS, regraICMSClonada);

            repositorioTabelaFreteCliente.Inserir(regraICMSClonada);

            return regraICMSClonada;
        }

        private void CopiarProdutosEmbarcador(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSOrigem, Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSDestino)
        {
            if (regraICMSOrigem.ProdutosEmbarcador?.Count > 0)
            {
                regraICMSDestino.ProdutosEmbarcador = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador in regraICMSOrigem.ProdutosEmbarcador)
                    regraICMSDestino.ProdutosEmbarcador.Add(produtoEmbarcador);
            }
            else
                regraICMSDestino.ProdutosEmbarcador = null;
        }

        private void CopiarTiposOperacao(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSOrigem, Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSDestino)
        {
            if (regraICMSOrigem.TiposOperacao?.Count > 0)
            {
                regraICMSDestino.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in regraICMSOrigem.TiposOperacao)
                    regraICMSDestino.TiposOperacao.Add(tipoOperacao);
            }
            else
                regraICMSDestino.TiposOperacao = null;
        }

        private void CopiarTiposDeCarga(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSOrigem, Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSDestino)
        {
            if (regraICMSOrigem.TiposDeCarga?.Count > 0)
            {
                regraICMSDestino.TiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in regraICMSOrigem.TiposDeCarga)
                    regraICMSDestino.TiposDeCarga.Add(tipoDeCarga);
            }
            else
                regraICMSDestino.TiposDeCarga = null;
        }

        private void CalcularPautaFiscal(ref decimal valorICMS, ref decimal baseCalculo, decimal percentualDescontoBC, decimal aliquotaICMS, bool incluirICMS, decimal distancia, decimal pesoKg, Dominio.Entidades.Estado estado, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);
            Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficiente = repCoeficientePautaFiscal.BuscarPorEstadoMesAno(estado.Sigla, DateTime.Today.Month, DateTime.Today.Year);
            if (coeficiente == null)
                return;

            Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaICMS = repPautaFiscal.BuscarPorEstadoTipoDeCargaDistancia(estado.Sigla, tipoDeCarga?.Codigo ?? 0, distancia);
            if (pautaICMS == null)
                return;

            decimal percentualCoeficiente = coeficiente.PercentualCoeficiente / 100;
            decimal descontoBaseCalculo = percentualDescontoBC / 100;
            decimal aliquota = aliquotaICMS / 100;

            if (pautaICMS.ValorTonelada > 0)
            {
                decimal pesoTon = pesoKg / 1000;
                baseCalculo = pesoTon * pautaICMS.ValorTonelada;
                if (percentualCoeficiente > 0 && coeficiente.Valor > 0)
                    baseCalculo = baseCalculo * (coeficiente.Valor * percentualCoeficiente);

                if (descontoBaseCalculo > 0)
                    baseCalculo = baseCalculo - (baseCalculo * descontoBaseCalculo);

                if (incluirICMS && aliquotaICMS > 0)
                    baseCalculo = baseCalculo / (1 - aliquota);

                baseCalculo = decimal.Round(baseCalculo, 2, MidpointRounding.AwayFromZero);

                valorICMS = decimal.Round(baseCalculo * aliquota, 2, MidpointRounding.AwayFromZero);
            }
            else if (pautaICMS.ValorViagem > 0)
            {
                baseCalculo = pautaICMS.ValorViagem;

                if (percentualCoeficiente > 0 && coeficiente.Valor > 0)
                    baseCalculo = baseCalculo * (coeficiente.Valor * percentualCoeficiente);

                if (descontoBaseCalculo > 0)
                    baseCalculo = baseCalculo * descontoBaseCalculo;

                baseCalculo = decimal.Round(baseCalculo, 2, MidpointRounding.AwayFromZero);

                valorICMS = decimal.Round(baseCalculo * aliquota, 2, MidpointRounding.AwayFromZero);
            }
            else
                return;
        }

        #endregion

        #region Métodos Públicos
        public decimal ObterValorLiquido(decimal valor, decimal aliquota)
        {
            valor = decimal.Round(valor * ((100 - aliquota) / 100), 2, MidpointRounding.AwayFromZero);

            return valor;
        }

        public decimal ObterPercentualRelativo(decimal valor, decimal valortotal)
        {
            return decimal.Round((valor / valortotal) * 100, 2, MidpointRounding.AwayFromZero);
        }

        public Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal, bool? incluirICMS = null, bool complementoICMS = false, Dominio.Enumeradores.TipoCTE tipoCTE = Dominio.Enumeradores.TipoCTE.Normal, List<Dominio.Entidades.Aliquota> aliquotasUfEmpresa = null, bool produtoEmbarcadorConsultar = true)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Localidade localidadeEmpresa;

            if (empresa != null)
                localidadeEmpresa = empresa.Localidade;
            else
                localidadeEmpresa = origem;

            if (produtoEmbarcador == null && cargaPedido != null && produtoEmbarcadorConsultar)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                produtoEmbarcador = repCargaPedidoProduto.BuscarProdutoComRegraICMS(cargaPedido.Codigo);
            }

            Dominio.Entidades.Cliente destinatarioExportacao = null;
            if (cargaPedido != null && cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade != null && cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = cargaPedido.Pedido.Destinatario;
            else if (destinatario != null && destinatario.Localidade != null && destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = destinatario;

            int codigoTipoOperacao = carga?.TipoOperacao?.Codigo ?? 0;
            int codigoTipoDeCarga = carga?.TipoDeCarga?.Codigo ?? 0;
            double cnpjTomador = 0;

            if (cargaPedido != null && cargaPedido.CargaOrigem.EmpresaFilialEmissora != null && ((cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa != null && empresa != null && empresa.Codigo == cargaPedido.CargaOrigem.Empresa.Codigo) || (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa == null)))
                double.TryParse(cargaPedido.CargaOrigem.EmpresaFilialEmissora.CNPJ, out cnpjTomador);
            else if (cargaPedido != null && !cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.EmpresaFilialEmissora == null && carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
                double.TryParse(carga.Filial.EmpresaEmissora.CNPJ, out cnpjTomador);

            if (cnpjTomador > 0)
            {
                Dominio.Entidades.Cliente tomadorEmissor = repCliente.BuscarPorCPFCNPJ(cnpjTomador);
                if (tomadorEmissor != null)
                    tomador = tomadorEmissor;
            }

            Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador.Atividade, destinatario.Atividade, unitOfWork, aliquotasUfEmpresa);
            retornoRegraICMS.AliquotaInternaDifal = this.ObterAliquotaInternaDifal(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador, unitOfWork, configuracao.CalcularInclusaoICMSAliquotaInterna, aliquota?.CST ?? "", remetente, cargaPedido?.Expedidor ?? null);

            if (aliquota != null)
            {
                Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento = ObterTipoPagamentoCarga(cargaPedido);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido?.TipoContratacaoCarga ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                if (cargaPedido != null && ((cargaPedido.CargaPedidoFilialEmissora && cargaPedido.Carga.Empresa != null && empresa != null && empresa.Codigo == cargaPedido.Carga.Empresa.Codigo) || (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.Carga.Empresa == null)))
                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                Dominio.Enumeradores.TipoServico? tipoServico = ObterTipoServicoPorTipoContratacaoCarga(tipoContratacaoCarga);

                string numeroProposta = "";
                if (cargaPedido != null)
                    numeroProposta = cargaPedido.Pedido?.NumeroProposta ?? "";
                else if (configuracao.UtilizaEmissaoMultimodal && carga != null && carga.Pedidos != null)
                    numeroProposta = carga.Pedidos.Select(o => o.Pedido.NumeroProposta).FirstOrDefault();

                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = ObterRegraICMS(empresa, cargaPedido?.Expedidor != null ? cargaPedido.Expedidor : remetente, destinatario, tomador, origem, destino, produtoEmbarcador, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, cargaPedido != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodalHelper.ConverterTipoCobrancaMultimodal(cargaPedido.TipoCobrancaMultimodal) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos, tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga);

                retornoRegraICMS.CFOP = aliquota.CFOP.CodigoCFOP;

                if (regraICMS != null)
                {
                    if (regraICMS.CFOP != null)
                        retornoRegraICMS.CFOP = regraICMS.CFOP.CodigoCFOP;

                    retornoRegraICMS.NaoIncluirPisCofinsBCEmComplementos = (regraICMS.NaoIncluirPisConfisNaBCParaComplementos && tipoCTE == Dominio.Enumeradores.TipoCTE.Complemento);
                    retornoRegraICMS.IncluirPisCofinsBC = regraICMS.IncluirPisConfisNaBC;
                    retornoRegraICMS.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    retornoRegraICMS.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                    retornoRegraICMS.CodigoRegra = regraICMS.Codigo;

                    if (incluiICMSBaseCalculo && (regraICMS.NaoIncluirICMSValorFrete ?? false))
                        incluiICMSBaseCalculo = false;
                }

                if (tipoContratacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada ||
                    (cargaPedido != null && tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada) ||
                    (cargaPedido?.ModeloDocumentoFiscal != null && (cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && !cargaPedido.ModeloDocumentoFiscal.CalcularImpostos)))
                {
                    if (regraICMS != null && (regraICMS.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || (cargaPedido != null && cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros))) //configuracao.UtilizaEmissaoMultimodal &&
                    {
                        if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                        {
                            if (regraICMS.CST != "SN")
                                retornoRegraICMS.CST = regraICMS.CST;
                            else
                                retornoRegraICMS.CST = "";

                            if (regraICMS.CSTIsenta && !regraICMS.Aliquota.HasValue)
                                retornoRegraICMS.Aliquota = 0;

                            if (configuracao.UtilizarRegraICMSParaDescontarValorICMS)
                            {
                                //Considerar para todas CST (Tarefa #3586 Marfrig)
                                retornoRegraICMS.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                            }
                            else
                            {
                                if (regraICMS.CST == "60")
                                    retornoRegraICMS.DescontarICMSDoValorAReceber = true;
                            }
                            retornoRegraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        }

                        if (regraICMS.Aliquota.HasValue)
                            retornoRegraICMS.Aliquota = regraICMS.Aliquota.Value;

                        if (regraICMS.PercentualReducaoBC.HasValue)
                            retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC.Value;

                        if (regraICMS.PercentualCreditoPresumido.HasValue)
                            retornoRegraICMS.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido.Value;

                        if (regraICMS.ImprimeLeiNoCTe)
                            retornoRegraICMS.ObservacaoCTe = regraICMS.DescricaoRegra;

                        if (regraICMS.ZerarValorICMS)
                            retornoRegraICMS.Aliquota = 0m;
                    }
                    else
                    {
                        retornoRegraICMS.CST = "40";
                        retornoRegraICMS.Aliquota = 0m;
                        retornoRegraICMS.ValorICMS = 0m;
                        retornoRegraICMS.ValorICMSIncluso = 0m;
                        retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                    }
                }
                else
                {
                    if (empresa == null || (!empresa.OptanteSimplesNacional || empresa.Localidade.Estado.Sigla != origem.Estado.Sigla) || complementoICMS) // segundo o diego, mesmo quando a empresa é simples nacional, se iniciar a prestação fora do seu estado o ICMS é calculado normalmente
                    {
                        retornoRegraICMS.CST = aliquota.CST;
                        retornoRegraICMS.Aliquota = aliquota.Percentual;

                        if (regraICMS != null)
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                if (regraICMS.CST != "SN")
                                    retornoRegraICMS.CST = regraICMS.CST;
                                else
                                    retornoRegraICMS.CST = "";

                                if (regraICMS.CSTIsenta && !regraICMS.Aliquota.HasValue)
                                    retornoRegraICMS.Aliquota = 0;

                                if (configuracao.UtilizarRegraICMSParaDescontarValorICMS)
                                {
                                    //Considerar para todas CST (Tarefa #3586 Marfrig)
                                    retornoRegraICMS.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                                }
                                else
                                {
                                    if (regraICMS.CST == "60")
                                        retornoRegraICMS.DescontarICMSDoValorAReceber = true;
                                }
                                retornoRegraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                            }

                            if (regraICMS.Aliquota.HasValue)
                                retornoRegraICMS.Aliquota = regraICMS.Aliquota.Value;

                            if (regraICMS.PercentualReducaoBC.HasValue)
                                retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC.Value;

                            if (regraICMS.PercentualCreditoPresumido.HasValue)
                                retornoRegraICMS.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido.Value;

                            if (regraICMS.ImprimeLeiNoCTe)
                                retornoRegraICMS.ObservacaoCTe = regraICMS.DescricaoRegra;

                            if (regraICMS.ZerarValorICMS)
                            {
                                valorParaBaseDeCalculo = 0m;
                                retornoRegraICMS.Aliquota = 0m;
                            }
                        }

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            if (carga?.TipoOperacao?.NaoIncluirICMSFrete ?? false)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 0m;
                                retornoRegraICMS.IncluirICMSBC = false;
                                percentualICMSIncluirNoFrete = 0m;
                                incluiICMSBaseCalculo = false;
                            }
                            else if ((carga.TabelaFrete == null && percentualICMSIncluirNoFrete == 0m) || (incluirICMS.HasValue && incluirICMS.Value))
                            { //tratamento realizado pois cargas da BRF e Natura que não passam pela etapa 1 estavam ficando sem % de inclusão do icms em alguns casos
                                retornoRegraICMS.PercentualInclusaoBC = 100;
                                retornoRegraICMS.IncluirICMSBC = true;
                                percentualICMSIncluirNoFrete = 100;
                                incluiICMSBaseCalculo = true;
                            }
                        }

                        if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                            (carga.TabelaFrete == null || carga.TabelaFrete.TipoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente))
                        {
                            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                            bool incluirICMSFreteInformadoManualmente = configuracaoTMS.IncluirICMSFreteInformadoManualmente;

                            if (configuracaoTMS.UtilizaEmissaoMultimodal)
                            {
                                incluirICMSFreteInformadoManualmente = configuracaoTMS.IncluirICMSFreteInformadoManualmente;
                                if (carga.TipoOperacao != null && carga.TipoOperacao.NaoIncluirICMSFrete)
                                    incluirICMSFreteInformadoManualmente = false;

                            }
                            if (incluirICMSFreteInformadoManualmente && (regraICMS?.NaoIncluirICMSValorFrete ?? false))
                                incluirICMSFreteInformadoManualmente = false;

                            if (incluirICMSFreteInformadoManualmente)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 100;
                                retornoRegraICMS.IncluirICMSBC = true;
                                percentualICMSIncluirNoFrete = 100;
                                incluiICMSBaseCalculo = true;
                            }
                            else
                            {
                                retornoRegraICMS.IncluirICMSBC = false;
                                retornoRegraICMS.PercentualInclusaoBC = 0;
                                incluiICMSBaseCalculo = false;
                                percentualICMSIncluirNoFrete = 0;
                            }

                        }

                        Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                        decimal aliquotaPisCofins = servicoPisCofins.ObterAliquotaPisConfis(retornoRegraICMS, empresa);

                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = valorParaBaseDeCalculo;

                        retornoRegraICMS.ValorICMSIncluso = CalcularICMSInclusoNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, retornoRegraICMS.AliquotaInternaDifal, aliquotaPisCofins);
                        retornoRegraICMS.ValorICMS = CalcularInclusaoICMSNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, aliquotaPisCofins);
                        retornoRegraICMS.ValorPis = servicoPisCofins.CalcularValorPis(retornoRegraICMS.AliquotaPis, retornoRegraICMS.ValorBaseCalculoPISCOFINS);
                        retornoRegraICMS.ValorCofins = servicoPisCofins.CalcularValorCofins(retornoRegraICMS.AliquotaCofins, retornoRegraICMS.ValorBaseCalculoPISCOFINS);

                        if (retornoRegraICMS.PercentualCreditoPresumido > 0m && retornoRegraICMS.ValorICMS > 0)
                            retornoRegraICMS.ValorCreditoPresumido = decimal.Round(retornoRegraICMS.ValorICMS * (retornoRegraICMS.PercentualCreditoPresumido / 100), 2, MidpointRounding.AwayFromZero);

                        retornoRegraICMS.ValorBaseCalculoICMS = valorParaBaseDeCalculo;
                    }
                    else
                    {
                        retornoRegraICMS.SimplesNacional = true;
                    }
                }

                if (retornoRegraICMS.Aliquota > 0 && retornoRegraICMS.ValorICMS > 0 && (carga?.TipoOperacao?.CalcularPautaFiscal ?? false))
                {
                    decimal valorICMSPauta = 0;
                    decimal baseCalculoICMSPauta = 0;
                    bool icmsIncluso = false;

                    CalcularPautaFiscal(ref valorICMSPauta, ref baseCalculoICMSPauta, retornoRegraICMS.PercentualReducaoBC, retornoRegraICMS.Aliquota, icmsIncluso, carga.Distancia > 0 ? carga.Distancia : carga.DadosSumarizados.Distancia, carga.DadosSumarizados.PesoTotal, origem.Estado, carga.TipoDeCarga, unitOfWork);

                    if (valorICMSPauta > retornoRegraICMS.ValorICMS)
                    {
                        retornoRegraICMS.ValorICMS = valorICMSPauta;
                        retornoRegraICMS.ValorICMSIncluso = icmsIncluso ? valorICMSPauta : 0m;
                        retornoRegraICMS.ValorBaseCalculoICMS = baseCalculoICMSPauta;
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = baseCalculoICMSPauta;
                        retornoRegraICMS.ObservacaoCTe = "ICMS calculado pela Pauta Fiscal.";
                    }
                }
            }
            else
                throw new ServicoException("Alíquota para cálculo de impostos não encontrada.");

            return retornoRegraICMS;
        }

        public Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraIcms = BuscarRegraICMSAsync(carga, cargaPedido, empresa, remetente, destinatario, tomador, origem, destino, incluiICMSBaseCalculo, percentualICMSIncluirNoFrete, valorParaBaseDeCalculo, cargaPedidoProdutos, unitOfWork, tipoServicoMultisoftware, configuracao, tabelaAliquotas, tomadoresFilialEmissora, tipoContratacao).Result;

            incluiICMSBaseCalculo = regraIcms.IncluiICMSBaseCalculo;
            percentualICMSIncluirNoFrete = regraIcms.PercentualICMSIncluirNoFrete;

            return regraIcms;
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS> BuscarRegraICMSAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, bool incluiICMSBaseCalculo, decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Localidade localidadeEmpresa;

            if (empresa != null)
                localidadeEmpresa = empresa.Localidade;
            else
                localidadeEmpresa = origem;

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;
            if (cargaPedidoProdutos != null && cargaPedido != null)
                produtoEmbarcador = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj.Produto).FirstOrDefault();


            Dominio.Entidades.Cliente destinatarioExportacao = null;
            if (cargaPedido != null && cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade != null && cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = cargaPedido.Pedido.Destinatario;
            else if (destinatario != null && destinatario.Localidade != null && destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = destinatario;

            int codigoTipoOperacao = carga?.TipoOperacao?.Codigo ?? 0;
            int codigoTipoDeCarga = carga?.TipoDeCarga?.Codigo ?? 0;

            double cnpjTomador = 0;
            if (cargaPedido != null && cargaPedido.CargaOrigem.EmpresaFilialEmissora != null && ((cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa != null && empresa != null && empresa.Codigo == cargaPedido.CargaOrigem.Empresa.Codigo) || (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa == null)))
                double.TryParse(cargaPedido.CargaOrigem.EmpresaFilialEmissora.CNPJ, out cnpjTomador);
            else if (!cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.EmpresaFilialEmissora == null && carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
                double.TryParse(carga.Filial.EmpresaEmissora.CNPJ, out cnpjTomador);

            if (cnpjTomador > 0)
            {
                if (!tomadoresFilialEmissora.Any(obj => obj.CPF_CNPJ == cnpjTomador))
                {
                    Dominio.Entidades.Cliente clienteTomador = await repCliente.BuscarPorCPFCNPJAsync(cnpjTomador);
                    if (clienteTomador != null)
                        tomadoresFilialEmissora.Add(clienteTomador);
                }

                Dominio.Entidades.Cliente tomadorEmissor = (from obj in tomadoresFilialEmissora where obj.CPF_CNPJ == cnpjTomador select obj).FirstOrDefault();
                if (tomadorEmissor != null)
                    tomador = tomadorEmissor;
            }


            Dominio.Entidades.Aliquota aliquota = (from obj in tabelaAliquotas where obj.AtividadeDestinatario == destinatario?.Atividade && obj.AtividadeTomador == tomador.Atividade && obj.UFEmitente == localidadeEmpresa.Estado && obj.UFInicioPrestacao == origem.Estado && obj.UFTerminoPrestacao == destino.Estado select obj.Aliquota).FirstOrDefault();
            if (aliquota == null && localidadeEmpresa != null && origem != null && destino != null && tomador != null && destinatario != null)
            {
                aliquota = await this.ObterAliquotaAsync(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador.Atividade, destinatario.Atividade, unitOfWork);
                if (aliquota != null)
                    tabelaAliquotas.Add(new Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota() { Aliquota = aliquota, AtividadeDestinatario = destinatario.Atividade, AtividadeTomador = tomador.Atividade, UFEmitente = localidadeEmpresa.Estado, UFInicioPrestacao = origem.Estado, UFTerminoPrestacao = destino.Estado });
            }
            else if (aliquota == null && localidadeEmpresa != null && origem != null && destino != null && tomador != null && cargaPedido.Pedido?.Recebedor != null)
            {
                aliquota = await this.ObterAliquotaAsync(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador.Atividade, cargaPedido.Pedido?.Recebedor.Atividade, unitOfWork);
                if (aliquota != null)
                    tabelaAliquotas.Add(new Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota() { Aliquota = aliquota, AtividadeDestinatario = cargaPedido.Pedido.Recebedor.Atividade, AtividadeTomador = tomador.Atividade, UFEmitente = localidadeEmpresa.Estado, UFInicioPrestacao = origem.Estado, UFTerminoPrestacao = destino.Estado });
            }
            else if (aliquota == null && localidadeEmpresa != null && origem != null && destino != null && cargaPedido.Pedido?.Recebedor != null)
            {
                aliquota = await this.ObterAliquotaAsync(localidadeEmpresa.Estado, origem.Estado, destino.Estado, cargaPedido.Pedido.Recebedor.Atividade, cargaPedido.Pedido?.Recebedor.Atividade, unitOfWork);
                if (aliquota != null)
                    tabelaAliquotas.Add(new Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota() { Aliquota = aliquota, AtividadeDestinatario = cargaPedido.Pedido.Recebedor.Atividade, AtividadeTomador = cargaPedido.Pedido.Recebedor.Atividade, UFEmitente = localidadeEmpresa.Estado, UFInicioPrestacao = origem.Estado, UFTerminoPrestacao = destino.Estado });
            }

            if (aliquota != null)
            {
                Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento = carga.EmpresaFilialEmissora != null && carga.Empresa?.Codigo == empresa?.Codigo ? Dominio.Enumeradores.TipoPagamentoRegraICMS.Outros : ObterTipoPagamentoCarga(cargaPedido);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido?.TipoContratacaoCarga ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                if (cargaPedido != null && (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false)))
                    tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
                else if (cargaPedido != null && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                    tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro;
                else if (cargaPedido != null && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.RedespachoIntermediario)
                    tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                else if (cargaPedido != null && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio)
                    tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio;
                else if (cargaPedido != null && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Redespacho)
                    tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;

                if (cargaPedido != null && ((cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa != null && empresa != null && empresa.Codigo == cargaPedido.CargaOrigem.Empresa.Codigo) || (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaOrigem.Empresa == null)))
                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                Dominio.Enumeradores.TipoServico? tipoServico = ObterTipoServicoPorTipoContratacaoCarga(tipoContratacaoCarga);

                string numeroProposta = "";
                if (cargaPedido != null)
                    numeroProposta = cargaPedido.Pedido?.NumeroProposta ?? "";
                else if (configuracao.UtilizaEmissaoMultimodal && carga != null && carga.Pedidos != null)
                    numeroProposta = carga.Pedidos.Select(o => o.Pedido.NumeroProposta).FirstOrDefault();

                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = null;
                if (destinatario != null)
                    regraICMS = ObterRegraICMS(empresa, cargaPedido?.Expedidor != null ? cargaPedido.Expedidor : remetente, destinatario, tomador, origem, destino, produtoEmbarcador, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodalHelper.ConverterTipoCobrancaMultimodal(cargaPedido.TipoCobrancaMultimodal), tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga);
                else if (cargaPedido.Pedido?.Recebedor != null && remetente != null)
                    regraICMS = ObterRegraICMS(empresa, cargaPedido?.Expedidor != null ? cargaPedido.Expedidor : remetente, cargaPedido.Pedido.Recebedor, tomador, origem, destino, produtoEmbarcador, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodalHelper.ConverterTipoCobrancaMultimodal(cargaPedido.TipoCobrancaMultimodal), tipoServico, tipoPagamento, numeroProposta, codigoTipoDeCarga);

                string cst = aliquota?.CST;
                if (regraICMS != null && !string.IsNullOrWhiteSpace(regraICMS.CST))
                    cst = regraICMS.CST;

                retornoRegraICMS.AliquotaInternaDifal = await this.ObterAliquotaInternaDifalAsync(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador, unitOfWork, configuracao.CalcularInclusaoICMSAliquotaInterna, cst, remetente, cargaPedido?.Expedidor);
                retornoRegraICMS.CFOP = aliquota.CFOP.CodigoCFOP;
                retornoRegraICMS.ObjetoCFOP = aliquota.CFOP;

                if (regraICMS != null)
                {
                    if (regraICMS.CFOP != null)
                    {
                        retornoRegraICMS.CFOP = regraICMS.CFOP.CodigoCFOP;
                        retornoRegraICMS.ObjetoCFOP = regraICMS.CFOP;
                    }

                    retornoRegraICMS.IncluirPisCofinsBC = regraICMS.IncluirPisConfisNaBC;
                    retornoRegraICMS.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    retornoRegraICMS.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                    retornoRegraICMS.CodigoRegra = regraICMS.Codigo;
                    retornoRegraICMS.Descricao = regraICMS.Descricao;

                    if (incluiICMSBaseCalculo && (regraICMS.NaoIncluirICMSValorFrete ?? false))
                        incluiICMSBaseCalculo = false;
                }

                if (tipoContratacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                    || (tipoContratacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && !cargaPedido.Carga.EmitirCTeComplementar)
                    || (cargaPedido != null && tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && !cargaPedido.Carga.EmitirCTeComplementar) || (cargaPedido?.ModeloDocumentoFiscal != null && (cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && !cargaPedido.ModeloDocumentoFiscal.CalcularImpostos)))
                {
                    if (regraICMS != null && (regraICMS.TipoServico == null || regraICMS.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || regraICMS.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || regraICMS.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || regraICMS.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)) //configuracao.UtilizaEmissaoMultimodal &&
                    {
                        if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                        {
                            if (regraICMS.CST != "SN")
                                retornoRegraICMS.CST = regraICMS.CST;
                            else
                                retornoRegraICMS.CST = "";

                            if (regraICMS.CSTIsenta && !regraICMS.Aliquota.HasValue)
                                retornoRegraICMS.Aliquota = 0;

                            if (configuracao.UtilizarRegraICMSParaDescontarValorICMS)
                            {
                                //Considerar para todas CST (Tarefa #3586 Marfrig)
                                retornoRegraICMS.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                            }
                            else
                            {
                                if (regraICMS.CST == "60")
                                    retornoRegraICMS.DescontarICMSDoValorAReceber = true;
                            }
                            retornoRegraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        }

                        if (regraICMS.Aliquota.HasValue)
                            retornoRegraICMS.Aliquota = regraICMS.Aliquota.Value;

                        if (regraICMS.PercentualReducaoBC.HasValue)
                            retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC.Value;

                        if (regraICMS.PercentualCreditoPresumido.HasValue)
                            retornoRegraICMS.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido.Value;

                        if (regraICMS.ImprimeLeiNoCTe)
                            retornoRegraICMS.ObservacaoCTe = regraICMS.DescricaoRegra;

                        if (regraICMS.ZerarValorICMS)
                            retornoRegraICMS.Aliquota = 0m;
                    }
                    else
                    {
                        retornoRegraICMS.CST = "40";
                        retornoRegraICMS.Aliquota = 0m;
                        retornoRegraICMS.ValorICMS = 0m;
                        retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                    }
                }
                else
                {
                    if (empresa == null || (!empresa.OptanteSimplesNacional || empresa.Localidade.Estado.Sigla != origem.Estado.Sigla || regraICMS?.Empresa != null)) // segundo o diego, mesmo quando a empresa é simples nacional, se iniciar a prestação fora do seu estado o ICMS é calculado normalmente
                    {
                        retornoRegraICMS.CST = aliquota.CST;
                        retornoRegraICMS.Aliquota = aliquota.Percentual;

                        if (regraICMS != null)
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                if (regraICMS.CST != "SN")
                                    retornoRegraICMS.CST = regraICMS.CST;
                                else
                                    retornoRegraICMS.CST = "";

                                if (regraICMS.CSTIsenta && !regraICMS.Aliquota.HasValue)
                                    retornoRegraICMS.Aliquota = 0;

                                if (configuracao.UtilizarRegraICMSParaDescontarValorICMS)
                                {
                                    //Considerar para todas CST (Tarefa #3586 Marfrig)
                                    retornoRegraICMS.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                                }
                                else
                                {
                                    if (regraICMS.CST == "60")
                                        retornoRegraICMS.DescontarICMSDoValorAReceber = true;
                                }
                                retornoRegraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                            }

                            if (regraICMS.Aliquota.HasValue)
                                retornoRegraICMS.Aliquota = regraICMS.Aliquota.Value;

                            if (regraICMS.PercentualReducaoBC.HasValue)
                                retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC.Value;

                            if (regraICMS.PercentualCreditoPresumido.HasValue)
                                retornoRegraICMS.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido.Value;

                            if (regraICMS.ImprimeLeiNoCTe)
                                retornoRegraICMS.ObservacaoCTe = regraICMS.DescricaoRegra;

                            if (regraICMS.ZerarValorICMS)
                            {
                                valorParaBaseDeCalculo = 0m;
                                retornoRegraICMS.Aliquota = 0m;
                            }
                        }

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            if (carga?.TipoOperacao?.NaoIncluirICMSFrete ?? false)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 0m;
                                retornoRegraICMS.IncluirICMSBC = false;
                                percentualICMSIncluirNoFrete = 0m;
                                incluiICMSBaseCalculo = false;
                            }
                            else if (carga.TabelaFrete == null && percentualICMSIncluirNoFrete == 0m)
                            { //tratamento realizado pois cargas da BRF e Natura que não passam pela etapa 1 estavam ficando sem % de inclusão do icms em alguns casos
                                retornoRegraICMS.PercentualInclusaoBC = 100;
                                retornoRegraICMS.IncluirICMSBC = true;
                                percentualICMSIncluirNoFrete = 100;
                                incluiICMSBaseCalculo = true;
                            }
                        }

                        if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                            (carga.TabelaFrete == null || carga.TabelaFrete.TipoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente))

                        {
                            bool incluirICMSFreteInformadoManualmente = configuracao.IncluirICMSFreteInformadoManualmente;

                            if (configuracao.UtilizaEmissaoMultimodal)
                            {
                                incluirICMSFreteInformadoManualmente = configuracao.IncluirICMSFreteInformadoManualmente;
                                if (carga.TipoOperacao != null && carga.TipoOperacao.NaoIncluirICMSFrete)
                                    incluirICMSFreteInformadoManualmente = false;
                            }

                            if (incluirICMSFreteInformadoManualmente && (regraICMS?.NaoIncluirICMSValorFrete ?? false))
                                incluirICMSFreteInformadoManualmente = false;

                            if (cargaPedido.CotacaoPedido != null && !cargaPedido.CotacaoPedido.IncluirValorICMSBaseCalculo)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 0m;
                                retornoRegraICMS.IncluirICMSBC = false;
                                percentualICMSIncluirNoFrete = 0m;
                                incluiICMSBaseCalculo = false;
                            }

                            else if (incluirICMSFreteInformadoManualmente)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 100;
                                retornoRegraICMS.IncluirICMSBC = true;
                                percentualICMSIncluirNoFrete = 100;
                                incluiICMSBaseCalculo = true;
                            }
                            else
                            {
                                retornoRegraICMS.IncluirICMSBC = false;
                                retornoRegraICMS.PercentualInclusaoBC = 0;
                                incluiICMSBaseCalculo = false;
                                percentualICMSIncluirNoFrete = 0;
                            }
                        }

                        Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                        decimal aliquotaPisCofins = servicoPisCofins.ObterAliquotaPisConfis(retornoRegraICMS, empresa);

                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = valorParaBaseDeCalculo;

                        bool naoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber = (regraICMS?.NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber ?? false) && retornoRegraICMS.CST == "20";

                        retornoRegraICMS.ValorICMSIncluso = CalcularICMSInclusoNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, retornoRegraICMS.AliquotaInternaDifal, aliquotaPisCofins, naoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber: naoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber);
                        retornoRegraICMS.ValorICMS = CalcularInclusaoICMSNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, aliquotaPisCofins);
                        retornoRegraICMS.ValorPis = servicoPisCofins.CalcularValorPis(retornoRegraICMS.AliquotaPis, retornoRegraICMS.ValorBaseCalculoPISCOFINS);
                        retornoRegraICMS.ValorCofins = servicoPisCofins.CalcularValorCofins(retornoRegraICMS.AliquotaCofins, retornoRegraICMS.ValorBaseCalculoPISCOFINS);

                        if (retornoRegraICMS.PercentualCreditoPresumido > 0m && retornoRegraICMS.ValorICMS > 0)
                            retornoRegraICMS.ValorCreditoPresumido = decimal.Round(retornoRegraICMS.ValorICMS * (retornoRegraICMS.PercentualCreditoPresumido / 100), 2, MidpointRounding.AwayFromZero);

                        retornoRegraICMS.ValorBaseCalculoICMS = valorParaBaseDeCalculo;
                    }
                    else
                    {
                        retornoRegraICMS.SimplesNacional = true;
                    }
                }

                if (retornoRegraICMS.Aliquota > 0 && retornoRegraICMS.ValorICMS > 0 && (carga?.TipoOperacao?.CalcularPautaFiscal ?? false))
                {
                    decimal valorICMSPauta = 0;
                    decimal baseCalculoICMSPauta = 0;
                    bool icmsIncluso = false;

                    CalcularPautaFiscal(ref valorICMSPauta, ref baseCalculoICMSPauta, retornoRegraICMS.PercentualReducaoBC, retornoRegraICMS.Aliquota, icmsIncluso, carga.Distancia > 0 ? carga.Distancia : carga.DadosSumarizados.Distancia, carga.DadosSumarizados.PesoTotal, origem.Estado, carga.TipoDeCarga, unitOfWork);

                    if (valorICMSPauta > retornoRegraICMS.ValorICMS)
                    {
                        retornoRegraICMS.ValorICMS = valorICMSPauta;
                        retornoRegraICMS.ValorICMSIncluso = icmsIncluso ? valorICMSPauta : 0m;
                        retornoRegraICMS.ValorBaseCalculoICMS = baseCalculoICMSPauta;
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = baseCalculoICMSPauta;
                        retornoRegraICMS.ObservacaoCTe = "ICMS calculado pela Pauta Fiscal.";
                    }
                }
            }
            else
            {
                if (configuracao.Pais == TipoPais.Exterior)
                    return new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

                if (!configuracao.UtilizaEmissaoMultimodal)
                    throw new ServicoException($"Alíquota para cálculo de impostos não encontrada para o pedido {cargaPedido.Pedido?.NumeroPedidoEmbarcador ?? ""}.");
            }

            retornoRegraICMS.IncluiICMSBaseCalculo = incluiICMSBaseCalculo;
            retornoRegraICMS.PercentualICMSIncluirNoFrete = percentualICMSIncluirNoFrete;

            return retornoRegraICMS;
        }

        public Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSMultiCTe(Dominio.Entidades.Empresa empresa, Dominio.Enumeradores.OpcaoSimNao simplesNacional, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, bool destinatarioExterior, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Dominio.Enumeradores.TipoServico tipoServico, string codigoProdutoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            Dominio.Entidades.Localidade localidadeEmpresa;

            if (empresa != null)
                localidadeEmpresa = empresa.Localidade;
            else
                localidadeEmpresa = origem;

            Dominio.Entidades.Atividade atividadePadrao = repAtividade.BuscarPorCodigo(3);

            Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador != null ? tomador.Atividade : atividadePadrao, destinatario != null ? destinatario.Atividade : atividadePadrao, unitOfWork);
            retornoRegraICMS.AliquotaInternaDifal = this.ObterAliquotaInternaDifal(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador, unitOfWork, false, "", null, null);
            if (aliquota != null)
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = !string.IsNullOrWhiteSpace(codigoProdutoEmbarcador) ? repProdutoEmbarcador.buscarPorCodigoEmbarcador(codigoProdutoEmbarcador) : null;

                if (destinatarioExterior) ////Quando tem regra de ICMS com destino EX, busca regra sempre utilizando estado do Destinatário 
                    destino = repLocalidade.BuscarPorEstado("EX");

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = ObterRegraICMSMultiCTe(empresa, remetente, destinatario, tomador, origem, destino, produtoEmbarcador, unitOfWork);

                retornoRegraICMS.AliquotaSimples = regraICMS != null ? regraICMS.AliquotaSimples : 0;
                retornoRegraICMS.ValorBaseCalculoICMS = 1m;
                retornoRegraICMS.ValorBaseCalculoPISCOFINS = 1m;
                if (aliquota.CFOP != null)
                    retornoRegraICMS.CFOP = aliquota.CFOP.CodigoCFOP;
                if (regraICMS != null)
                {
                    if (regraICMS.CFOP > 0)
                        retornoRegraICMS.CFOP = regraICMS.CFOP;

                    retornoRegraICMS.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    retornoRegraICMS.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                }

                if (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao && !empresa.Configuracao.UsarRegraICMSParaCteDeSubcontratacao)
                {
                    retornoRegraICMS.CST = "40";
                    retornoRegraICMS.Aliquota = 0m;
                    retornoRegraICMS.ValorICMS = 0m;
                    retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                    retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                }
                else
                {
                    if (empresa != null && empresa.EmitirTodosCTesComoSimples)
                    {
                        if (regraICMS != null && !string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                            retornoRegraICMS.ObservacaoCTe = regraICMS.ObservacaoCTe;

                        retornoRegraICMS.SimplesNacional = true;
                    }
                    else if (empresa == null || (simplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao || empresa.Localidade.Estado.Sigla != origem.Estado.Sigla))
                    {
                        retornoRegraICMS.CST = aliquota.CST;
                        retornoRegraICMS.Aliquota = aliquota.Percentual;

                        if (regraICMS != null)
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                                retornoRegraICMS.CST = regraICMS.CST;
                            if (regraICMS.Aliquota > -1)
                                retornoRegraICMS.Aliquota = regraICMS.Aliquota;

                            if (regraICMS.PercentualReducaoBC > -1)
                                retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC;

                            if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                                retornoRegraICMS.ObservacaoCTe = regraICMS.ObservacaoCTe;

                            if (regraICMS.ValorBaseCalculoICMS == 0)
                                retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                            else
                                retornoRegraICMS.ValorBaseCalculoICMS = 1m;

                            if (regraICMS.ValorBaseCalculoPISCOFINS == 0)
                                retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                            else
                                retornoRegraICMS.ValorBaseCalculoPISCOFINS = 1m;

                        }
                    }
                    else
                    {
                        if (regraICMS != null && !string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                            retornoRegraICMS.ObservacaoCTe = regraICMS.ObservacaoCTe;

                        retornoRegraICMS.SimplesNacional = true;
                    }
                }
            }
            else
            {
                throw new Exception("Alíquota para cálculo de impostos não encontrada.");
            }

            return retornoRegraICMS;
        }

        public Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSMultiTMS(int codigoAtividade, Dominio.Entidades.Estado ufEmissor, Dominio.Enumeradores.OpcaoSimNao simplesNacional, Dominio.Entidades.Estado ufOrigem, Dominio.Entidades.Estado ufDestino, Dominio.Entidades.Estado ufTomador, bool destinatarioExterior, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Dominio.Entidades.Atividade atividadePadrao = repAtividade.BuscarPorCodigo(codigoAtividade > 0 ? codigoAtividade : 3);

            Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(ufEmissor, ufOrigem, ufDestino, atividadePadrao, atividadePadrao, unitOfWork);
            retornoRegraICMS.AliquotaInternaDifal = this.ObterAliquotaInternaDifal(ufEmissor, ufOrigem, ufDestino, null, unitOfWork, false, "", null, null);

            if (aliquota != null)
            {
                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = ObterRegraICMSMultiTMS(atividadePadrao, ufEmissor, ufOrigem, ufDestino, ufTomador, unitOfWork);

                retornoRegraICMS.AliquotaSimples = regraICMS != null ? regraICMS.AliquotaSimples : 0;
                retornoRegraICMS.ValorBaseCalculoICMS = 1m;
                retornoRegraICMS.ValorBaseCalculoPISCOFINS = 1m;

                if (aliquota.CFOP != null)
                    retornoRegraICMS.CFOP = aliquota.CFOP.CodigoCFOP;

                retornoRegraICMS.CST = aliquota.CST;
                retornoRegraICMS.Aliquota = aliquota.Percentual;

                if (regraICMS != null)
                {
                    retornoRegraICMS.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    retornoRegraICMS.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                    retornoRegraICMS.Descricao = regraICMS.Descricao;

                    if (regraICMS.CFOP > 0)
                        retornoRegraICMS.CFOP = regraICMS.CFOP;

                    if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                        retornoRegraICMS.CST = regraICMS.CST;

                    if (regraICMS.Aliquota > -1)
                        retornoRegraICMS.Aliquota = regraICMS.Aliquota;

                    if (regraICMS.PercentualReducaoBC > -1)
                        retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC;

                    if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                        retornoRegraICMS.ObservacaoCTe = regraICMS.ObservacaoCTe;

                    if (regraICMS.ValorBaseCalculoICMS == 0)
                        retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                    else
                        retornoRegraICMS.ValorBaseCalculoICMS = 1m;

                    if (regraICMS.ValorBaseCalculoPISCOFINS == 0)
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                    else
                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = 1m;
                }
            }
            else
            {
                return null;
            }

            return retornoRegraICMS;
        }

        public Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS retornoRegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            Dominio.Entidades.Localidade localidadeEmpresa;
            if (empresa != null)
                localidadeEmpresa = empresa.Localidade;
            else
                localidadeEmpresa = origem;

            int codigoTipoOperacao = preCarga?.TipoOperacao?.Codigo ?? 0;
            int codigoTipoDeCarga = preCarga?.TipoDeCarga?.Codigo ?? 0;

            Dominio.Entidades.Cliente destinatarioExportacao = null;

            if (pedido != null && pedido.Destinatario != null && pedido.Destinatario.Localidade != null && pedido.Destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = pedido.Destinatario;

            Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador.Atividade, destinatario.Atividade, unitOfWork);
            retornoRegraICMS.AliquotaInternaDifal = this.ObterAliquotaInternaDifal(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador, unitOfWork, configuracao.CalcularInclusaoICMSAliquotaInterna, "", null, null);

            if (aliquota != null)
            {
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = ObterRegraICMS(empresa, remetente, destinatario, tomador, origem, destino, produtoEmbarcador, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, (pedido != null && pedido.TipoOperacao != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodalHelper.ConverterTipoCobrancaMultimodal(pedido.TipoOperacao.TipoCobrancaMultimodal)) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos), null, null, "", codigoTipoDeCarga);
                retornoRegraICMS.CFOP = aliquota.CFOP.CodigoCFOP;

                if (regraICMS != null)
                {
                    if (regraICMS.CFOP != null)
                        retornoRegraICMS.CFOP = regraICMS.CFOP.CodigoCFOP;

                    retornoRegraICMS.IncluirPisCofinsBC = regraICMS.IncluirPisConfisNaBC;
                    retornoRegraICMS.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    retornoRegraICMS.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                    retornoRegraICMS.CodigoRegra = regraICMS.Codigo;

                    if (incluiICMSBaseCalculo && (regraICMS.NaoIncluirICMSValorFrete ?? false))
                        incluiICMSBaseCalculo = false;
                }

                if (tipoContratacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                {
                    retornoRegraICMS.CST = "40";
                    retornoRegraICMS.Aliquota = 0m;
                    retornoRegraICMS.ValorICMS = 0m;
                    retornoRegraICMS.ValorICMSIncluso = 0m;
                    retornoRegraICMS.ValorBaseCalculoICMS = 0m;
                    retornoRegraICMS.ValorBaseCalculoPISCOFINS = 0m;
                }
                else
                {
                    if (empresa == null || (!empresa.OptanteSimplesNacional || empresa.Localidade.Estado.Sigla != origem.Estado.Sigla)) // segundo o diego, mesmo quando a empresa é simples nacional, se iniciar a prestação fora do seu estado o ICMS é calculado normalmente
                    {
                        retornoRegraICMS.CST = aliquota.CST;
                        retornoRegraICMS.Aliquota = aliquota.Percentual;

                        if (regraICMS != null)
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                if (regraICMS.CST != "SN")
                                    retornoRegraICMS.CST = regraICMS.CST;
                                else
                                    retornoRegraICMS.CST = "";

                                if (regraICMS.CSTIsenta && !regraICMS.Aliquota.HasValue)
                                    retornoRegraICMS.Aliquota = 0;

                                if (configuracao.UtilizarRegraICMSParaDescontarValorICMS)
                                {
                                    //Considerar para todas CST (Tarefa #3586 Marfrig)
                                    retornoRegraICMS.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                                }
                                else
                                {
                                    if (regraICMS.CST == "60")
                                        retornoRegraICMS.DescontarICMSDoValorAReceber = true;
                                }
                                retornoRegraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                            }

                            if (regraICMS.Aliquota.HasValue)
                                retornoRegraICMS.Aliquota = regraICMS.Aliquota.Value;

                            if (regraICMS.PercentualReducaoBC.HasValue)
                                retornoRegraICMS.PercentualReducaoBC = regraICMS.PercentualReducaoBC.Value;

                            if (regraICMS.PercentualCreditoPresumido.HasValue)
                                retornoRegraICMS.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido.Value;

                            if (regraICMS.ImprimeLeiNoCTe)
                                retornoRegraICMS.ObservacaoCTe = regraICMS.DescricaoRegra;

                            if (regraICMS.ZerarValorICMS)
                            {
                                valorParaBaseDeCalculo = 0m;
                                retornoRegraICMS.Aliquota = 0m;
                            }
                        }

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && preCarga.TabelaFrete == null && percentualICMSIncluirNoFrete == 0m)
                        { //tratamento realizado pois cargas da BRF e Natura que não passam pela etapa 1 estavam ficando sem % de inclusão do icms em alguns casos
                            retornoRegraICMS.PercentualInclusaoBC = 100;
                            retornoRegraICMS.IncluirICMSBC = true;
                            percentualICMSIncluirNoFrete = 100;
                            incluiICMSBaseCalculo = true;
                        }

                        if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                            (preCarga.TabelaFrete == null || preCarga.TabelaFrete.TipoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente))
                        {
                            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                            bool incluirICMSFreteInformadoManualmente = configuracaoTMS.IncluirICMSFreteInformadoManualmente;

                            if (incluirICMSFreteInformadoManualmente && (regraICMS?.NaoIncluirICMSValorFrete ?? false))
                                incluirICMSFreteInformadoManualmente = false;

                            if (incluirICMSFreteInformadoManualmente)
                            {
                                retornoRegraICMS.PercentualInclusaoBC = 100;
                                retornoRegraICMS.IncluirICMSBC = true;
                                percentualICMSIncluirNoFrete = 100;
                                incluiICMSBaseCalculo = true;
                            }
                            else
                                retornoRegraICMS.IncluirICMSBC = false;
                        }

                        Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                        decimal aliquotaPisCofins = servicoPisCofins.ObterAliquotaPisConfis(retornoRegraICMS, empresa);

                        retornoRegraICMS.ValorBaseCalculoPISCOFINS = valorParaBaseDeCalculo;

                        retornoRegraICMS.ValorICMSIncluso = CalcularICMSInclusoNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, retornoRegraICMS.AliquotaInternaDifal, aliquotaPisCofins);
                        retornoRegraICMS.ValorICMS = CalcularInclusaoICMSNoFrete(retornoRegraICMS.CST, ref valorParaBaseDeCalculo, retornoRegraICMS.Aliquota, percentualICMSIncluirNoFrete, retornoRegraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, aliquotaPisCofins);
                        retornoRegraICMS.ValorPis = servicoPisCofins.CalcularValorPis(retornoRegraICMS.AliquotaPis, retornoRegraICMS.ValorBaseCalculoPISCOFINS);
                        retornoRegraICMS.ValorCofins = servicoPisCofins.CalcularValorCofins(retornoRegraICMS.AliquotaCofins, retornoRegraICMS.ValorBaseCalculoPISCOFINS);

                        if (retornoRegraICMS.PercentualCreditoPresumido > 0m && retornoRegraICMS.ValorICMS > 0)
                            retornoRegraICMS.ValorCreditoPresumido = decimal.Round(retornoRegraICMS.ValorICMS * (retornoRegraICMS.PercentualCreditoPresumido / 100), 2, MidpointRounding.AwayFromZero);

                        retornoRegraICMS.ValorBaseCalculoICMS = valorParaBaseDeCalculo;
                    }
                    else
                    {
                        retornoRegraICMS.SimplesNacional = true;
                    }
                }
            }
            else
            {
                throw new Exception("Alíquota para cálculo de impostos não encontrada.");
            }

            return retornoRegraICMS;
        }

        public decimal CalcularICMSInclusoNoFrete(string CST, ref decimal valorBaseCalculoICMS, decimal aliquota, decimal percentualICMSIncluirNoFrete, decimal percentualReducaoBaseCalculoICMS, bool incluirICMSBase, decimal aliquotaInternaDifal, decimal aliquotaPISCOFINS = 0m, bool adicionarPISCOFINSBaseCalculoICMS = false, bool naoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber = false)
        {
            decimal percentualAliquota = CST != "40" && CST != "41" && CST != "51" && CST != "" ? (aliquotaInternaDifal > 0 ? aliquotaInternaDifal : aliquota) : 0;

            decimal percentualICMSRecolhido = incluirICMSBase ? percentualICMSIncluirNoFrete : 0;

            if (adicionarPISCOFINSBaseCalculoICMS)
                valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - aliquotaPISCOFINS) / 100)) / ((100 - percentualAliquota) / 100) - valorBaseCalculoICMS) : 0) : 0;
            else
                valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - percentualAliquota - aliquotaPISCOFINS) / 100)) - valorBaseCalculoICMS) : 0) : 0;

            valorBaseCalculoICMS = decimal.Round(valorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);

            if (percentualReducaoBaseCalculoICMS > 0 && !naoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber)
                valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);

            decimal valorICMS = valorBaseCalculoICMS * (percentualAliquota / 100);

            return valorICMS;
        }

        public decimal CalcularInclusaoICMSNoFrete(string CST, ref decimal valorBaseCalculoICMS, decimal aliquota, decimal percentualICMSIncluirNoFrete, decimal percentualReducaoBaseCalculoICMS, bool incluirICMSBase, decimal aliquotaPISCOFINS = 0m, bool adicionarPISCOFINSBaseCalculoICMS = false)
        {
            incluirICMSBase = false;

            decimal percentualAliquota = CST != "40" && CST != "41" && CST != "51" && CST != "" ? aliquota : 0;
            decimal percentualICMSRecolhido = incluirICMSBase ? percentualICMSIncluirNoFrete : 0;

            if (adicionarPISCOFINSBaseCalculoICMS)
                valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - aliquotaPISCOFINS) / 100)) / ((100 - percentualAliquota) / 100) - valorBaseCalculoICMS) : 0) : 0;
            else
                valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - percentualAliquota - aliquotaPISCOFINS) / 100)) - valorBaseCalculoICMS) : 0) : 0;

            decimal valorICMS = valorBaseCalculoICMS * (percentualAliquota / 100);

            valorBaseCalculoICMS = decimal.Round(valorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);

            return valorICMS;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> CriarProdutosCargaContidosEmRegras(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            List<int> produtosEmbarcadorRegras = new List<int>();

            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork);

            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasComProdutos = regrasCalculoImpostos.ObterRegrasICMS().Where(o => o.ProdutosEmbarcador != null && o.ProdutosEmbarcador.Count > 0).Distinct().ToList();

            if (regrasComProdutos != null && regrasComProdutos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS reg in regrasComProdutos)
                {
                    List<int> codigosRegra = reg.ProdutosEmbarcador.Select(o => o.Codigo).Distinct().ToList();
                    if (codigosRegra != null && codigosRegra.Count > 0)
                        produtosEmbarcadorRegras.AddRange(codigosRegra);
                }
            }

            if (produtosEmbarcadorRegras.Count > 0)
            {
                foreach (int codigoProduto in produtosEmbarcadorRegras)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = (from obj in listaPedidoProduto where obj.Produto.Codigo == codigoProduto select obj.Produto).FirstOrDefault();

                    if (produtoEmbarcador != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto();
                        cargaPedidoProduto.CargaPedido = cargaPedido;
                        cargaPedidoProduto.Produto = produtoEmbarcador;
                        cargaPedidoProdutos.Add(cargaPedidoProduto);
                    }
                }
            }

            return cargaPedidoProdutos;
        }

        public bool IncluirComponenteFreteBaseCalculoIcms(TipoComponenteFrete tipoComponenteFrete, Dominio.Entidades.Empresa empresa, string siglaEstadoOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoComponenteFrete == TipoComponenteFrete.PEDAGIO)
                return IncluirPedagioBaseCalculoIcmsPorRegraEstado(empresa, siglaEstadoOrigem, unitOfWork);

            return true;
        }

        public Dominio.Entidades.Aliquota ObterAliquota(Dominio.Entidades.Estado ufEmitente, Dominio.Entidades.Estado ufInicioPrestacao, Dominio.Entidades.Estado ufTerminoPrestacao, Dominio.Entidades.Atividade atividadeTomador, Dominio.Entidades.Atividade atividadeDestinatario, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Aliquota> aliquotasUfEmpresa = null)
        {
            Dominio.Entidades.Aliquota aliquota = null;

            if (aliquotasUfEmpresa != null)
            {
                aliquota = aliquotasUfEmpresa.Where(obj => obj.EstadoEmpresa.Sigla.Equals(ufEmitente.Sigla) &&
                               obj.EstadoOrigem.Sigla.Equals(ufInicioPrestacao.Sigla) &&
                               obj.EstadoDestino.Sigla.Equals(ufTerminoPrestacao.Sigla) &&
                               obj.AtividadeTomador?.Codigo == atividadeTomador.Codigo &&
                               obj.AtividadeDestinatario?.Codigo == atividadeDestinatario.Codigo).FirstOrDefault();

                if (aliquota == null)
                {
                    aliquota = aliquotasUfEmpresa.Where(obj => obj.EstadoEmpresa.Sigla.Equals(ufEmitente.Sigla) &&
                                   obj.EstadoOrigem.Sigla.Equals(ufInicioPrestacao.Sigla) &&
                                   obj.EstadoDestino.Sigla.Equals(ufTerminoPrestacao.Sigla) &&
                                   obj.AtividadeTomador.Codigo == atividadeTomador.Codigo).FirstOrDefault();
                }

                return aliquota;
            }

            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);

            aliquota = repAliquota.BuscarParaCalculoDoICMS(ufEmitente?.Sigla, ufInicioPrestacao?.Sigla, ufTerminoPrestacao?.Sigla, atividadeTomador.Codigo, atividadeDestinatario.Codigo);

            if (aliquota == null)
                aliquota = repAliquota.BuscarParaCalculoDoICMS(ufEmitente?.Sigla, ufInicioPrestacao?.Sigla, ufTerminoPrestacao?.Sigla, atividadeTomador.Codigo);

            return aliquota;
        }

        public async Task<Dominio.Entidades.Aliquota> ObterAliquotaAsync(Dominio.Entidades.Estado ufEmitente, Dominio.Entidades.Estado ufInicioPrestacao, Dominio.Entidades.Estado ufTerminoPrestacao, Dominio.Entidades.Atividade atividadeTomador, Dominio.Entidades.Atividade atividadeDestinatario, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Aliquota> aliquotasUfEmpresa = null)
        {
            Dominio.Entidades.Aliquota aliquota = null;

            if (aliquotasUfEmpresa != null)
            {
                aliquota = aliquotasUfEmpresa.FirstOrDefault(obj => obj.EstadoEmpresa.Sigla.Equals(ufEmitente.Sigla) &&
                               obj.EstadoOrigem.Sigla.Equals(ufInicioPrestacao.Sigla) &&
                               obj.EstadoDestino.Sigla.Equals(ufTerminoPrestacao.Sigla) &&
                               obj.AtividadeTomador?.Codigo == atividadeTomador.Codigo &&
                               obj.AtividadeDestinatario?.Codigo == atividadeDestinatario.Codigo);

                if (aliquota == null)
                {
                    aliquota = aliquotasUfEmpresa.FirstOrDefault(obj => obj.EstadoEmpresa.Sigla.Equals(ufEmitente.Sigla) &&
                                   obj.EstadoOrigem.Sigla.Equals(ufInicioPrestacao.Sigla) &&
                                   obj.EstadoDestino.Sigla.Equals(ufTerminoPrestacao.Sigla) &&
                                   obj.AtividadeTomador.Codigo == atividadeTomador.Codigo);
                }

                return aliquota;
            }

            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);

            aliquota = await repAliquota.BuscarParaCalculoDoICMSAsync(ufEmitente?.Sigla, ufInicioPrestacao?.Sigla, ufTerminoPrestacao?.Sigla, atividadeTomador.Codigo, atividadeDestinatario.Codigo);

            if (aliquota == null)
                aliquota = await repAliquota.BuscarParaCalculoDoICMSAsync(ufEmitente?.Sigla, ufInicioPrestacao?.Sigla, ufTerminoPrestacao?.Sigla, atividadeTomador.Codigo);

            return aliquota;
        }

        public string ObterObservacaoRegraICMS(string observacao, decimal aliquotaICMS, decimal aliquotaSimples, decimal valorFrete, decimal valorICMS, decimal baseCalculo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, decimal percentualICMSIncluirNoFrete, string produtoEmbarcador, decimal valorCreditoPresumido)
        {
            if (observacao != null)
            {
                string novaObs = observacao.Replace("#Aliquota#", aliquotaICMS.ToString("n2"))
                                           .Replace("#ValorICMS#", valorICMS.ToString("n2"))
                                           .Replace("#ValorFrete#", valorFrete.ToString("n2"))
                                           .Replace("#BaseCalculo#", baseCalculo.ToString("n2"))
                                           .Replace("#PercentualICMSIncluirNoFrete#", percentualICMSIncluirNoFrete.ToString("n2"))
                                           .Replace("#Transportadora#", empresa != null ? empresa.RazaoSocial : "")
                                           .Replace("#Tomador#", tomador != null ? tomador.Nome : "")
                                           .Replace("#Rementente#", remetente != null ? remetente.Nome : "")
                                           .Replace("#Destinatario#", destinatario != null ? destinatario.Nome : "")
                                           .Replace("#UFOrigem#", origem != null ? origem.Estado.Sigla : "")
                                           .Replace("#Produto#", produtoEmbarcador != null ? produtoEmbarcador : "")
                                           .Replace("#UFDestino#", destino != null ? destino.Estado.Sigla : "")
                                           .Replace("#AliquotaSimples#", aliquotaSimples > 0 ? aliquotaSimples.ToString("n2") : "")
                                           .Replace("#ValorFreter*AliquotaSimples#", aliquotaSimples > 0 && valorFrete > 0 ? Math.Round(valorFrete * (aliquotaSimples / 100), 2, MidpointRounding.ToEven).ToString("n2") : "")
                                           .Replace("#ValorCreditoPresumido#", valorCreditoPresumido > 0 ? valorCreditoPresumido.ToString("n2") : "")
                                           .Replace("#ValorICMSMenosValorCreditoPresumido#", valorICMS > 0 ? (valorICMS - valorCreditoPresumido).ToString("n2") : "");

                return novaObs;
            }
            else
            {
                return "";
            }
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> ObterProdutosCargaContidosEmRegras(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork);

            List<int> produtosEmbarcadorRegras = new List<int>();

            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasComProdutos = regrasCalculoImpostos.ObterRegrasICMS().Where(o => o.ProdutosEmbarcador != null && o.ProdutosEmbarcador.Count > 0).Distinct().ToList();

            if (regrasComProdutos != null && regrasComProdutos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS reg in regrasComProdutos)
                {
                    List<int> codigosRegra = reg.ProdutosEmbarcador.Select(o => o.Codigo).Distinct().ToList();

                    if (codigosRegra != null && codigosRegra.Count > 0)
                        produtosEmbarcadorRegras.AddRange(codigosRegra);
                }
            }

            if (produtosEmbarcadorRegras.Count > 0)
                cargaPedidoProdutos = repCargaPedidoProduto.BuscarProdutoComRegraICMSPorCarga(carga.Codigo, produtosEmbarcadorRegras);

            return cargaPedidoProdutos;
        }

        public decimal ObterValorIcmsComponenteFrete(Dominio.Entidades.Embarcador.Frete.ComponenteFreteBase componenteFrete, Dominio.Entidades.Empresa empresa, string siglaEstadoOrigem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = null)
        {
            if (componenteFrete.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO)
            {
                Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo pedagioEstadoBaseCalculo = null;
                if (pedagioEstadosBaseCalculo != null)
                    pedagioEstadoBaseCalculo = pedagioEstadosBaseCalculo.Where(o => o.Estado.Sigla == (siglaEstadoOrigem)).FirstOrDefault();
                else
                {
                    Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repositorioPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
                    pedagioEstadoBaseCalculo = repositorioPedagioEstadoBaseCalculo.BuscarPorEstado(siglaEstadoOrigem);
                }

                return ObterValorIcmsPedagioPorRegraEstado(componenteFrete, empresa, pedagioEstadoBaseCalculo, unitOfWork, tipoServicoMultisoftware);
            }

            return componenteFrete.IncluirBaseCalculoICMS ? componenteFrete.ValorComponente : 0m;
        }

        public decimal ObterValorIcmsComponenteFrete(Dominio.Entidades.Embarcador.Frete.ComponenteFreteBase componenteFrete, Dominio.Entidades.Empresa empresa, string siglaEstadoOrigem, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (componenteFrete.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO)
            {
                Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo pedagioEstadoBaseCalculo = (from o in pedagioEstadosBaseCalculo where o.Estado.Sigla == siglaEstadoOrigem select o).FirstOrDefault();

                return ObterValorIcmsPedagioPorRegraEstado(componenteFrete, empresa, pedagioEstadoBaseCalculo, unitOfWork, tipoServicoMultisoftware);
            }

            return componenteFrete.IncluirBaseCalculoICMS ? componenteFrete.ValorComponente : 0m;
        }

        //public bool VerificarIncluiICMSFrete(Dominio.Entidades.Cliente rementete, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        //{
        //    bool incluirICMSBase = true;
        //    Repositorio.Embarcador.ICMS.SubstituicaoTributariaEstado repSTEstado = new Repositorio.Embarcador.ICMS.SubstituicaoTributariaEstado(unitOfWork);
        //    if (empresa != null)
        //    {
        //        if (rementete.Localidade.Estado.Sigla != empresa.Localidade.Estado.Sigla)
        //        {
        //            Dominio.Entidades.Embarcador.ICMS.SubstituicaoTributariaEstado stEstado = repSTEstado.BuscarPorEstado(rementete.Localidade.Estado.Sigla);
        //            if (stEstado != null)
        //            {
        //                incluirICMSBase = false;
        //            }
        //        }
        //    }
        //    return incluirICMSBase;
        //}

        public Dominio.Entidades.Embarcador.ICMS.RegraICMS DuplicarParaAlteracao(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSClonada = Duplicar(regraICMS, unitOfWork);

            regraICMSClonada.DataAlteracao = DateTime.Now;
            regraICMSClonada.RegraOriginaria = regraICMS;
            regraICMSClonada.Tipo = TipoRegraICMS.Alteracao;

            repositorioRegraICMS.Atualizar(regraICMSClonada);

            return regraICMSClonada;
        }

        public void AplicarAlteracoes(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

            if (regraICMS.RegraOriginaria != null)
            {
                Utilidades.Object.CopiarPropriedadesObjeto(regraICMS, regraICMS.RegraOriginaria);
                CopiarProdutosEmbarcador(regraICMS, regraICMS.RegraOriginaria);
                CopiarTiposOperacao(regraICMS, regraICMS.RegraOriginaria);
                CopiarTiposDeCarga(regraICMS, regraICMS.RegraOriginaria);

                regraICMS.RegraOriginaria.DataAlteracao = null;
                regraICMS.RegraOriginaria.RegraOriginaria = null;
                regraICMS.RegraOriginaria.Tipo = TipoRegraICMS.Ativa;

                repositorioRegraICMS.Atualizar(regraICMS.RegraOriginaria);
            }
            else
                regraICMS.Tipo = TipoRegraICMS.Ativa;
        }
        public Dominio.Entidades.Embarcador.ICMS.RegraICMS ObterRegraICMSCotacaoPedido(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool filtrarTodasRegras, int codigoTipoOperacao, Dominio.Entidades.Cliente destinatarioExportacao, int codigoTipoDeCarga)
        {
            return ObterRegraICMS(empresa, remetente, destinatario, tomador, origem, destino, produtoEmbarcador, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos, null, null, null, codigoTipoDeCarga);
        }

        public decimal ObterAliquotaICMSPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido == null || pedido.Remetente == null || pedido.Destinatario == null)
            {
                return 0m;
            }

            Dominio.Entidades.Cliente destinatario = pedido.Destinatario;
            Dominio.Entidades.Localidade destino = pedido.Destinatario.Localidade;
            Dominio.Entidades.Cliente remetente = pedido.Remetente;
            Dominio.Entidades.Localidade origem = pedido.Remetente.Localidade;
            Dominio.Entidades.Cliente tomador;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas;

            if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
            {
                grupoPessoas = pedido.Remetente?.GrupoPessoas;
                tomador = pedido.Remetente;
            }
            else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
            {
                grupoPessoas = pedido.Destinatario?.GrupoPessoas;
                tomador = pedido.Destinatario;
            }
            else
            {
                grupoPessoas = pedido.Tomador?.GrupoPessoas;
                tomador = pedido.Tomador;
            }

            Dominio.Entidades.Cliente destinatarioExportacao = null;
            if (destinatario.Localidade != null && destinatario.Localidade.Estado.Sigla == "EX")
                destinatarioExportacao = destinatario;

            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = ObterRegraICMS(pedido.Empresa, remetente, destinatario, tomador, origem, destino, null, unitOfWork, true, pedido.TipoOperacao?.Codigo ?? 0, destinatarioExportacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos, null, null, null, pedido.TipoDeCarga?.Codigo ?? 0);

            if (regraICMS != null)
            {
                return regraICMS.Aliquota.HasValue && regraICMS.Aliquota > 0 ? regraICMS.Aliquota.Value : 0m;
            }
            else
            {
                Dominio.Entidades.Localidade localidadeEmpresa = new Dominio.Entidades.Localidade();
                if (pedido.Empresa != null)
                    localidadeEmpresa = pedido.Empresa.Localidade;
                else
                    localidadeEmpresa = origem;

                Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(localidadeEmpresa.Estado, origem.Estado, destino.Estado, tomador.Atividade, destinatario.Atividade, unitOfWork);

                return aliquota.Percentual > 0 ? aliquota.Percentual : 0m;
            }
        }
        #endregion
    }
}
