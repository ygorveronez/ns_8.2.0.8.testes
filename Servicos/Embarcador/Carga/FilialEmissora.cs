using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class FilialEmissora
    {
        #region Métodos Privados

        private string InformarDadosCTeNaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<int> codigosCargaPedidos = null)
        {
            string retorno = "";
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

            CTeSubContratacao servicoCTeSubContratacao = new CTeSubContratacao(unitOfWork);
            string descricaoItemPeso = servicoCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);
            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro;

            if (!cargaPedido.EmitirComplementarFilialEmissora)
                cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, cargaCTe, CTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
            else
            {
                cteTerceiro = repPedidoCTeParaSubContratacao.BuscarPorCTeTerceiroFilialEmissoraCargaPedido(cargaPedido.Codigo);
                cteTerceiro.AdicionarCargaCTe(cargaCTe);
                repositorioCTeTerceiro.Atualizar(cteTerceiro);
            }

            if (string.IsNullOrEmpty(retorno))
            {
                if (cteTerceiro != null)
                {
                    cteTerceiro.Ativo = true;
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = servicoCTeSubContratacao.InserirCTeSubContratacaoFilialEmissora(cteTerceiro, cargaCTe, cargaPedido, tipoServicoMultisoftware, unitOfWork);
                    repositorioCTeTerceiro.Atualizar(cteTerceiro);

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    if (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoCarga?.GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega ?? false)
                        xmlNotasFiscais = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarXMLNotasFiscaisPorCTeEPedido(cargaCTe.Codigo, cargaPedido.Pedido.Codigo);
                    else
                        xmlNotasFiscais = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(cargaCTe.Codigo);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = xmlNotasFiscais.Count > 0 ? repositorioPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscalECargaPedido(xmlNotasFiscais, cargaPedido.Codigo) : null;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlNotasFiscais)
                        servicoCTeSubContratacao.CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, servicoCTeSubContratacao.ConverterPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao), false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, pedidosXMLNotaFiscal, codigosCargaPedidos);
                }
                else
                    retorno = "Não foi possível encontrar o CT-e de origem da filial emissora";
            }

            return retorno;
        }

        private void InformarDadosCTeNaCargaRetorno(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, bool gerarCanhotoNfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string retorno = "";
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

            CTeSubContratacao servicoCTeSubContratacao = new CTeSubContratacao(unitOfWork);

            string descricaoItemPeso = servicoCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);
            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, CTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);

            if (!string.IsNullOrEmpty(retorno))
                throw new ServicoException(retorno);

            cteParaSubContratacao.Ativo = true;
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = servicoCTeSubContratacao.InserirCTeSubContratacaoFilialEmissora(cteParaSubContratacao, cargaCTe, cargaPedido, tipoServicoMultisoftware, unitOfWork);
            repositorioCTeTerceiro.Atualizar(cteParaSubContratacao);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();//repositorioCargaPedidoXMLnotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(cargaCTe.Codigo);

            if (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoCarga?.GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega ?? false)
                xmlNotasFiscais = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarXMLNotasFiscaisPorCTeEPedido(cargaCTe.Codigo, cargaPedido.Pedido.Codigo);
            else
                xmlNotasFiscais = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(cargaCTe.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = xmlNotasFiscais.Count > 0 ? repositorioPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscalECargaPedido(xmlNotasFiscais, cargaPedido.Codigo) : null;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlNotasFiscais)
                servicoCTeSubContratacao.CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, servicoCTeSubContratacao.ConverterPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao), gerarCanhotoNfe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, pedidosXMLNotaFiscal);
        }

        #endregion

        #region Métodos Públicos

        public void GerarCTesAnteriores(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDedicado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Embarcador.CTe.CTe servicoCTe = new Embarcador.CTe.CTe(unitOfWork);

            if (cargaPedidoDedicado.PedidoCTesParaSubContratacao != null && cargaPedidoDedicado.PedidoCTesParaSubContratacao.Count() > 0)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctesAnteriores = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarCargaCTesSemFilialEmissoraPorCargaPedido(cargaPedidoAnterior.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in ctesAnteriores)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCTe.ConverterEntidadeCTeParaObjeto(cargaCTe.CTe, enviarCTeApenasParaTomador, unitOfWork);

                InformarDadosCTeNaCargaRetorno(unitOfWork, cteIntegracao, cargaPedidoDedicado, cargaCTe, true, tipoServicoMultisoftware, configuracao);
            }
        }

        public void GerarCTesAnterioresDaFilialEmissoraRetorno(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRedespacho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, bool gerarCanhotoNFe, bool semFilialEmissora = false)
        {
            if (cargaPedidoRedespacho.PedidoCTesParaSubContratacao != null && cargaPedidoRedespacho.PedidoCTesParaSubContratacao.Count() > 0)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Embarcador.CTe.CTe servicoCTe = new Embarcador.CTe.CTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctesAnteriores;

            if (semFilialEmissora)
                ctesAnteriores = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarCargaCTesSemFilialEmissoraPorCargaPedido(cargaPedidoAnterior.Codigo);
            else
                ctesAnteriores = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarCargaCTesPorCargaPedido(cargaPedidoAnterior.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in ctesAnteriores)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCTe.ConverterEntidadeCTeParaObjeto(cargaCTe.CTe, enviarCTeApenasParaTomador, unitOfWork);

                try
                {
                    InformarDadosCTeNaCargaRetorno(unitOfWork, cteIntegracao, cargaPedidoRedespacho, cargaCTe, gerarCanhotoNFe, tipoServicoMultisoftware, configuracao);
                }
                catch (ServicoException excecao)
                {
                    Log.TratarErro(excecao.Message);
                    unitOfWork.Rollback();
                    return;
                }
            }
        }

        public void GerarCTesAnterioresDaFilialEmissoraRedespacho(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRedespacho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedidoRedespacho.PedidoCTesParaSubContratacao != null && cargaPedidoRedespacho.PedidoCTesParaSubContratacao.Count() > 0)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);
            Embarcador.CTe.CTe servicoCTe = new Embarcador.CTe.CTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctesAnteriores = repositorioCargaPedidoXMLnotaFiscalCTe.BuscarCargaCTesPorCargaPedido(cargaPedidoAnterior.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in ctesAnteriores)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCTe.ConverterEntidadeCTeParaObjeto(cargaCTe.CTe, enviarCTeApenasParaTomador, unitOfWork);
                string retorno = InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedidoRedespacho, cargaCTe, tipoServicoMultisoftware, configuracao);

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    Servicos.Log.TratarErro(retorno);
                    unitOfWork.Rollback();
                    return;
                }
            }
        }

        /// <summary>
        /// vincula os Ct-es da filial emissora as cargasPedidos para emitir os Ct-es de subcontratação ou redespacho dos transportadores.
        /// </summary>
        public void GerarCTesAnterioresDaFilialEmissora(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);
            bool emiteDocumentosSempreOrigemDestinoPedido = ((carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false) && carga.EmpresaFilialEmissora == null);
            //bool emiteDocumentoSempreOrigemDestinoPedidoComFilial = carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false;
            List<int> empresasFiliaisEmissoras = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);
            List<int> cargaCtesAnteriores = repCargaCte.BuscarPorCargaCTesFilialEmissora(carga.Codigo, empresasFiliaisEmissoras);

            List<int> empresasMatriz = repCargaPedido.ObterEmpresasCarga(carga.Codigo);
            List<int> cargaCtesAnterioresMatriz = repCargaCte.BuscarPorCargaCTesFilialEmissora(carga.Codigo, empresasMatriz);

            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            for (int i = 0; i < cargaCtesAnteriores.Count; i++)
            {
                unitOfWork.FlushAndClear();

                if (repCTeTerceiro.ExistePorCargaCte(cargaCtesAnteriores[i]))
                    continue;

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCte.BuscarPorCodigo(cargaCtesAnteriores[i]);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedidoXMLnotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCte.Codigo);

                List<int> codigosCargaPedidos = (from obj in cargaPedidos select obj.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    string retorno = "";
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(cargaCte.CTe, enviarCTeApenasParaTomador, unitOfWork);

                    if (!emiteDocumentosSempreOrigemDestinoPedido)
                        retorno = InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido, cargaCte, tipoServicoMultisoftware, configuracao, codigosCargaPedidos);

                    if (cargaPedido.CargaPedidoProximoTrecho != null)
                        retorno = InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido.CargaPedidoProximoTrecho, cargaCte, tipoServicoMultisoftware, configuracao, codigosCargaPedidos);


                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        Servicos.Log.TratarErro(retorno);
                        unitOfWork.Rollback();
                        return;
                    }

                }

                unitOfWork.CommitChanges();
            }

            for (int i = 0; i < cargaCtesAnterioresMatriz.Count; i++)
            {
                unitOfWork.FlushAndClear();

                if (repCTeTerceiro.ExistePorCargaCte(cargaCtesAnterioresMatriz[i]) || !emiteDocumentosSempreOrigemDestinoPedido)
                    continue;

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCte.BuscarPorCodigo(cargaCtesAnterioresMatriz[i]);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedidoXMLnotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCte.Codigo);

                List<int> codigosCargaPedidos = (from obj in cargaPedidos select obj.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    string retorno = "";
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(cargaCte.CTe, enviarCTeApenasParaTomador, unitOfWork);

                    if (cargaPedido.CargaPedidoProximoTrecho != null)
                        retorno = InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido.CargaPedidoProximoTrecho, cargaCte, tipoServicoMultisoftware, configuracao, codigosCargaPedidos);


                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        Servicos.Log.TratarErro(retorno);
                        unitOfWork.Rollback();
                        return;
                    }

                }

                unitOfWork.CommitChanges();
            }

            unitOfWork.Start();

            carga = repCarga.BuscarPorCodigo(carga.Codigo);
            //if (emiteDocumentoSempreOrigemDestinoPedidoComFilial)
            //    carga.EmEmissaoCTeSubContratacaoFilialEmissora = false;
            //else
            carga.EmEmissaoCTeSubContratacaoFilialEmissora = true;

            carga.AgGeracaoCTesAnteriorFilialEmissora = false;
            repCarga.Atualizar(carga);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasProximoTrecho = (from obj in carga.Pedidos where obj.CargaPedidoProximoTrecho != null select obj.CargaPedidoProximoTrecho.Carga).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaProximoTrecho in cargasProximoTrecho)
            {
                if (cargaProximoTrecho.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && !(repPedidoCTeParaSubContratacao.VerificarSePedidosNaoPossuemCTeAnteriorPorCarga(cargaProximoTrecho.Codigo)))
                {
                    if (cargaProximoTrecho.Empresa != null && (cargaProximoTrecho.DataEnvioUltimaNFe.HasValue || !carga.ExigeConfirmacaoAntesEmissao))
                        cargaProximoTrecho.DataEnvioUltimaNFe = DateTime.Now.AddHours(cargaProximoTrecho.Empresa.TempoDelayHorasParaIniciarEmissao);

                    cargaProximoTrecho.AguardandoEmissaoDocumentoAnterior = false;
                }

                cargaProximoTrecho.CargaGeradaComCTeAnteriorFilialEmissora = true;
                repCarga.Atualizar(cargaProximoTrecho);
            }

            unitOfWork.CommitChanges();

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
        }

        public static List<Dominio.Entidades.Cliente> ObterTomadoresFilialEmissora(List<string> cnpjs, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Cliente> tomadores = new List<Dominio.Entidades.Cliente>();
            if (cnpjs.Count > 0)
            {
                List<double> cnpjsFilial = new List<double>();
                foreach (var cnpj in cnpjs)
                {
                    double cnpjFilial = 0;
                    double.TryParse(cnpj, out cnpjFilial);
                    cnpjsFilial.Add(cnpjFilial);
                }
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                tomadores = repCliente.BuscarPorCPFCNPJs(cnpjsFilial);
            }

            return tomadores;
        }

        public void VerificarSeLiberaEmissaoRedespacho(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            if (!repPedidoCTeParaSubContratacao.VerificarSePedidosNaoPossuemCTeAnteriorPorCarga(carga.Codigo))
            {
                if (carga.Empresa != null && (carga.DataEnvioUltimaNFe.HasValue || !carga.ExigeConfirmacaoAntesEmissao))
                    carga.DataEnvioUltimaNFe = DateTime.Now.AddHours(carga.Empresa.TempoDelayHorasParaIniciarEmissao);

                carga.AguardandoEmissaoDocumentoAnterior = false;
            }
            else
            {
                carga.DataEnvioUltimaNFe = null;
                carga.AguardandoEmissaoDocumentoAnterior = true;
            }
            repCarga.Atualizar(carga);

        }

        #endregion
    }
}
