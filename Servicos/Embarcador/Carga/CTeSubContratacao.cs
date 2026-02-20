using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using NHibernate.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class CTeSubContratacao : ServicoBase
    {                
        public CTeSubContratacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro CriarCTeTerceiro(Repositorio.UnitOfWork unitOfWork, ref string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, string descricaoItemPeso = null, bool utilizarPrimeiraUnidadeMedidaPeso = false, int codigoCargaPedido = 0, bool documentoRecebidoPorFTP = false, bool documentoRecebidoPorEmail = false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, string identificacaoPacote = "", Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS;

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);
            if (cacheObjetoValorCTe.ConfiguracaoTMS == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
            }
            else
                configuracaoTMS = cacheObjetoValorCTe.ConfiguracaoTMS;

            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
            if (configuracaoTMS.PermitirAutalizarNotaFiscalCarga && codigoCargaPedido > 0)
                cteTerceiro = repPedidoCTeParaSubContratacao.BuscarPorCTeTerceiroPorChaveECargaPedido(CTe.Chave, codigoCargaPedido);
            if (cteTerceiro == null)
                cteTerceiro = repCTeTerceiro.BuscarPorChave(CTe.Chave, false, cacheObjetoValorCTe.LstCtesTerceiro);

            Servicos.Embarcador.CTe.CTe serCTe = new Embarcador.CTe.CTe(unitOfWork);

            if (configuracaoTMS.SempreBuscaCTePorChaveEmIntegracaoViaWS)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(CTe.Chave);
                if (cte != null)
                    CTe = serCTe.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);
            }

            if (cteTerceiro == null)
            {
                cteTerceiro = new Dominio.Entidades.Embarcador.CTe.CTeTerceiro();
                cteTerceiro.DocumentoRecebidoViaFTP = documentoRecebidoPorFTP;
                cteTerceiro.DocumentoRecebidoViaEmail = documentoRecebidoPorEmail;
                cteTerceiro.IdentifacaoPacote = identificacaoPacote;
                cteTerceiro.AdicionarCargaCTe(CargaCTe);

                mensagem += preencherParticipantes(ref cteTerceiro, unitOfWork, CTe, null, tipoServicoMultisoftware, cacheObjetoValorCTe, objetoValorPersistente);
                if (string.IsNullOrEmpty(mensagem))
                {
                    mensagem += preencherDadosCTe(ref cteTerceiro, unitOfWork, CTe);
                    if (string.IsNullOrEmpty(mensagem))
                    {
                        cteTerceiro.Ativo = true;
                        if (objetoValorPersistente == null)
                            repCTeTerceiro.Inserir(cteTerceiro);
                        else
                            objetoValorPersistente.Inserir(cteTerceiro);

                        mensagem += PreencherQuantidades(ref cteTerceiro, unitOfWork, CTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso, cacheObjetoValorCTe, objetoValorPersistente);
                        mensagem += PreencherSeguros(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                        if (CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado)
                        {
                            mensagem += PreencherEntregasSimplificado(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                        }
                        else
                        {
                            mensagem += PreencherOutrosDocumentos(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                            mensagem += PreencherNotasFiscais(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                            mensagem += PreencherNFes(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                            PreencherComponentesFrete(cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                        }

                        mensagem += SalvarXMLCTe(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe);//angelopendenciadeindice ***
                        SetarNumeroPedidoObservacaoCTeTerceiro(cteTerceiro, cteTerceiro.Emitente?.Cliente, unitOfWork);
                        SetarNumeroPedidoObservacaoCTeSubcontratacao(cteTerceiro, cteTerceiro.Emitente?.Cliente, unitOfWork);
                        return cteTerceiro;
                    }
                }
                return null;
            }
            else
            {
                cteTerceiro.AdicionarCargaCTe(CargaCTe);

                mensagem += preencherParticipantes(ref cteTerceiro, unitOfWork, CTe, null, tipoServicoMultisoftware, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += preencherDadosCTe(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe);

                if (cteTerceiro.Peso <= 0m)
                {
                    cteTerceiro.Peso = ObterPesoDaSubContratacao(cteTerceiro, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso, unitOfWork);
                    cteTerceiro.DescricaoItemPeso = descricaoItemPeso;
                }

                mensagem += PreencherQuantidades(ref cteTerceiro, unitOfWork, CTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += PreencherSeguros(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += PreencherOutrosDocumentos(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += PreencherNotasFiscais(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += PreencherNFes(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);
                mensagem += SalvarXMLCTe(ref cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe);//angelopendenciadeindice ***

                PreencherComponentesFrete(cteTerceiro, unitOfWork, CTe, cacheObjetoValorCTe, objetoValorPersistente);

                SetarNumeroPedidoObservacaoCTeTerceiro(cteTerceiro, cteTerceiro.Emitente?.Cliente, unitOfWork);
                SetarNumeroPedidoObservacaoCTeSubcontratacao(cteTerceiro, cteTerceiro.Emitente?.Cliente, unitOfWork);

                cteTerceiro.DocumentoRecebidoViaFTP = documentoRecebidoPorFTP;
                cteTerceiro.DocumentoRecebidoViaEmail = documentoRecebidoPorEmail;
                cteTerceiro.IdentifacaoPacote = identificacaoPacote;
                cteTerceiro.Ativo = true;

                if (objetoValorPersistente == null)
                    repCTeTerceiro.Atualizar(cteTerceiro);
                else
                    objetoValorPersistente.Atualizar(cteTerceiro);

                return cteTerceiro;
            }
        }

        public string InformarDadosCTeNaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, bool encaixeCTe = false, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Carga(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhotos.Canhoto(unitOfWork);

            bool situacaoPermiteEnvioCTe = ((cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe || encaixeCTe) && cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
                || ((serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, tipoServicoMultisoftware) || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe) && !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete);

            if (!situacaoPermiteEnvioCTe)
                return "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite o envio de CT-es para SubContratação";

            string retorno = "";
            string descricaoItemPeso = ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);
            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao;
            if (cteTerceiro == null)
                cteParaSubContratacao = CriarCTeTerceiro(unitOfWork, ref retorno, null, CTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso, cargaPedido?.Codigo ?? 0, false, false, tipoServicoMultisoftware);
            else
                cteParaSubContratacao = cteTerceiro;

            if (!string.IsNullOrEmpty(retorno))
                return retorno;

            retorno += ValidarRegrasCTeParaSubContratacao(cteParaSubContratacao, cargaPedido, unitOfWork, tipoServicoMultisoftware);

            if (!string.IsNullOrEmpty(retorno))
                return retorno;

            cteParaSubContratacao.Ativo = true;
            pedidoCTeParaSubContratacao = InserirCTeSubContratacaoCargaPedido(cteParaSubContratacao, cargaPedido, tipoServicoMultisoftware, unitOfWork);
            repPedido.Atualizar(cargaPedido.Pedido);
            repCarga.Atualizar(cargaPedido.Carga);
            repositorioCTeTerceiro.Atualizar(cteParaSubContratacao);

            svcCanhoto.SalvarCanhotoCTe(cteParaSubContratacao, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, unitOfWork);

            return retorno;
        }

        public void ExcluirNotasFiscaisDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            (List<int> codigosPedidosCTeSub, List<int> codigosPedidosXML) = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarCodigosPedidoXMLNotaFiscalPorCargaPedido(cargaPedido.Codigo);

            repPedidoXMLNotaFiscalComponenteFrete.DeletarPorPedidoCTeParaSubContratacao(cargaPedido.Codigo);

            for (int i = 0; i < codigosPedidosCTeSub.Count; i += 1000)
                repPedidoCTeParaSubContratacaoPedidoNotaFiscal.DeletarPorCodigosEntidade(codigosPedidosCTeSub.Skip(i).Take(1000).ToList());

            for (int i = 0; i < codigosPedidosCTeSub.Count; i += 1000)
                repPedidoCTeParaSubContratacaoPedidoNotaFiscal.DeletarPorPedidosXMLNotaFiscal(codigosPedidosXML.Skip(i).Take(1000).ToList());

            for (int i = 0; i < codigosPedidosXML.Count; i += 1000)
                repPedidoXMLNotaFiscal.DeletarPorPedidoCTeParaSubContratacao(codigosPedidosXML.Skip(i).Take(1000).ToList());

        }

        public void CriarNotasFiscaisDaCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, bool utilizarInsertEmMassa = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[i].Codigo);
                CriarNotasFiscaisDaCargaPedido(cargaPedido, tipoServicoMultisoftware, configuracao, unitOfWork, controlarTransacao: true, utilizarInsertEmMassa: utilizarInsertEmMassa);
            }
        }

        public void CriarNotasFiscaisDaCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, bool controlarTransacao = false, bool utilizarInsertEmMassa = false)
        {
            Log.TratarErro($"Iniciou CriarNotasFiscaisDaCargaPedido da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}", "ConfirmarEnvioDosDocumentos");

            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeParaSubContratacaoOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeParaSubcontratacaoNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubcontratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

            Servicos.Embarcador.NFe.NFe serNFe = new Embarcador.NFe.NFe(unitOfWork);

            string descricaoItemPeso = ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedidoParaProcessamento(cargaPedido.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.PedidoCTeParaSubcontratacaoPedidoXMLNotaFiscal> pedidoCTesParaSubcontratacaoNotaFiscal = repPedidoCTeParaSubcontratacaoPedidoNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNFe> nfesCTeTerceiro = repCTeParaSubcontratacaoNFe.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroOutrosDocumentos> outrosDocumentosCTeTerceiro = repCTeParaSubContratacaoOutrosDocumentos.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNotaFiscal> notasFiscaisCTeTerceiro = repCTeParaSubContratacaoNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCTeTerceiro = repCTeTerceiroQuantidade.BuscarPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = nfesCTeTerceiro.Count > 0 ? repXMLNotaFiscal.BuscarPorCargaPedidoComCTeTerceiro(cargaPedido.Codigo) : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscalOutrosDocumentos = repXMLNotaFiscal.BuscarPorCargaPedidoETipoDocumentos(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoSemFetch(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> lstInsertxmlNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> lstUpdatexmlNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> lstPedidoXMLNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (!utilizarInsertEmMassa)
            {
                lstInsertxmlNotaFiscal = null;
                lstUpdatexmlNotaFiscal = null;
                lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal = null;
                lstPedidoXMLNotaFiscal = null;
            }

            if (controlarTransacao)
                unitOfWork.Start();

            Log.TratarErro($"Iniciou for pedidosubcontratacao da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo} com {pedidoCTesParaSubcontratacao.Count} documentos", "ConfirmarEnvioDosDocumentos");

            int contadorRegistros = 0;
            for (int i = 0; i < pedidoCTesParaSubcontratacao.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidoCTesParaSubcontratacao[i];
                contadorRegistros++;

                decimal pesoKG = pedidoCTeParaSubContratacao.CTeTerceiro.Peso > 0m ? pedidoCTeParaSubContratacao.CTeTerceiro.Peso : ObterPesoDaSubContratacao(quantidadesCTeTerceiro.Where(o => o.CTeTerceiro.Codigo == pedidoCTeParaSubContratacao.CTeTerceiro.Codigo).ToList(), descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
                bool contemNFe = false;
                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNFe> cteParaSubContratacaoNFes = nfesCTeTerceiro.Where(o => o.CTeTerceiro.Codigo == pedidoCTeParaSubContratacao.CTeTerceiro.Codigo).ToList();

                for (int j = 0; j < cteParaSubContratacaoNFes.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNFe nfe = cteParaSubContratacaoNFes[j];
                    contemNFe = true;

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = xmlNotasFiscais.Find(o => o.Chave == nfe.Chave);

                    if (xmlNotaFiscal == null)
                    {
                        xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(pedidoCTeParaSubContratacao.CTeTerceiro.Remetente, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario, pesoKG / cteParaSubContratacaoNFes.Count, cargaPedido, unitOfWork);

                        xmlNotaFiscal.DataEmissao = pedidoCTeParaSubContratacao.CTeTerceiro.DataEmissao;
                        xmlNotaFiscal.Numero = nfe.Chave.Length == 44 ? int.Parse(nfe.Chave.Substring(25, 9)) : 0;
                        xmlNotaFiscal.Serie = nfe.Chave.Length == 44 ? int.Parse(nfe.Chave.Substring(22, 3)).ToString() : "";
                        xmlNotaFiscal.Chave = nfe.Chave;
                        xmlNotaFiscal.Protocolo = nfe.Protocolo;
                        xmlNotaFiscal.nfAtiva = true;
                        xmlNotaFiscal.Modelo = "55";
                        xmlNotaFiscal.Valor = nfe.ValorTotal;
                        if (xmlNotaFiscal.Peso == 0 && nfe.Peso > 0)
                            xmlNotaFiscal.Peso = nfe.Peso;
                        xmlNotaFiscal.Volumes = (int)nfe.Volumes;
                        if (!string.IsNullOrWhiteSpace(nfe.NCM))
                            xmlNotaFiscal.NCM = nfe.NCM;
                        if (!string.IsNullOrWhiteSpace(nfe.PINSuframa))
                            xmlNotaFiscal.PINSUFRAMA = nfe.PINSuframa;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroReferenciaEDI))
                            xmlNotaFiscal.NumeroReferenciaEDI = nfe.NumeroReferenciaEDI;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroControleCliente))
                            xmlNotaFiscal.NumeroControleCliente = nfe.NumeroControleCliente;
                        xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                    }
                    else
                    {
                        if (xmlNotaFiscal.Peso == 0 && nfe.Peso > 0)
                            xmlNotaFiscal.Peso = nfe.Peso;
                        if (xmlNotaFiscal.Valor == 0 && nfe.ValorTotal > 0)
                            xmlNotaFiscal.Valor = nfe.ValorTotal;
                        if (xmlNotaFiscal.Volumes == 0 && (int)nfe.Volumes > 0)
                            xmlNotaFiscal.Volumes = (int)nfe.Volumes;
                        if (!string.IsNullOrWhiteSpace(nfe.NCM))
                            xmlNotaFiscal.NCM = nfe.NCM;
                        if (!string.IsNullOrWhiteSpace(nfe.PINSuframa))
                            xmlNotaFiscal.PINSUFRAMA = nfe.PINSuframa;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroReferenciaEDI))
                            xmlNotaFiscal.NumeroReferenciaEDI = nfe.NumeroReferenciaEDI;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroControleCliente))
                            xmlNotaFiscal.NumeroControleCliente = nfe.NumeroControleCliente;
                    }

                    CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, pedidoCTeParaSubContratacao, (!cargaPedido.Carga.TipoOperacao?.NaoGerarCanhoto ?? false), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, pedidoXMLNotasFiscais, null, pedidoCTesParaSubcontratacaoNotaFiscal, configuracaoCanhoto, lstInsertxmlNotaFiscal, lstUpdatexmlNotaFiscal, lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal, lstPedidoXMLNotaFiscal);
                }

                if (!contemNFe)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroOutrosDocumentos> cteParaSubContratacaoOutrosDocumentos = outrosDocumentosCTeTerceiro.Where(o => o.CTeTerceiro.Codigo == pedidoCTeParaSubContratacao.CTeTerceiro.Codigo).ToList();
                    for (int j = 0; j < cteParaSubContratacaoOutrosDocumentos.Count; j++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroOutrosDocumentos outrosDoc = cteParaSubContratacaoOutrosDocumentos[j];
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                        if (xmlNotaFiscalOutrosDocumentos.Count > 0)
                            xmlNotaFiscal = xmlNotaFiscalOutrosDocumentos.Find(o => o.Numero.ToString() == outrosDoc.Numero);

                        if (xmlNotaFiscal == null)
                        {
                            xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(pedidoCTeParaSubContratacao.CTeTerceiro.Remetente, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario, pesoKG / cteParaSubContratacaoOutrosDocumentos.Count, cargaPedido, unitOfWork);

                            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;
                            if (outrosDoc.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Declaracao)
                                xmlNotaFiscal.Modelo = "00";
                            else if (outrosDoc.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Dutoviario)
                                xmlNotaFiscal.Modelo = "10";
                            else
                                xmlNotaFiscal.Modelo = "99";
                            xmlNotaFiscal.DataEmissao = DateTime.Now;
                            xmlNotaFiscal.Descricao = outrosDoc.Descricao;
                            xmlNotaFiscal.nfAtiva = true;
                            xmlNotaFiscal.Serie = outrosDoc.CTeTerceiro != null ? outrosDoc.CTeTerceiro.Serie : "";
                            int numeroNota = 0;
                            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(outrosDoc.Numero)))
                                int.TryParse(Utilidades.String.OnlyNumbers(outrosDoc.Numero), out numeroNota);
                            xmlNotaFiscal.Numero = numeroNota;

                            if (!string.IsNullOrWhiteSpace(outrosDoc.NCM))
                                xmlNotaFiscal.NCM = outrosDoc.NCM;
                            xmlNotaFiscal.Valor = outrosDoc.Valor;
                        }

                        CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, pedidoCTeParaSubContratacao, (!cargaPedido.Carga.TipoOperacao?.NaoGerarCanhoto ?? false), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, pedidoXMLNotasFiscais, null, pedidoCTesParaSubcontratacaoNotaFiscal, configuracaoCanhoto, lstInsertxmlNotaFiscal, lstUpdatexmlNotaFiscal, lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal, lstPedidoXMLNotaFiscal);
                    }

                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNotaFiscal> cteParaSubContratacaoNotasFiscais = notasFiscaisCTeTerceiro.Where(o => o.CTeTerceiro.Codigo == pedidoCTeParaSubContratacao.CTeTerceiro.Codigo).ToList();
                    for (int j = 0; j < cteParaSubContratacaoNotasFiscais.Count; j++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNotaFiscal notaFiscal = cteParaSubContratacaoNotasFiscais[j];
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(pedidoCTeParaSubContratacao.CTeTerceiro.Remetente, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario, pesoKG / cteParaSubContratacaoNotasFiscais.Count, cargaPedido, unitOfWork);

                        if (notaFiscal.DataEmissao != DateTime.MinValue)
                            xmlNotaFiscal.DataEmissao = notaFiscal.DataEmissao;
                        else
                            xmlNotaFiscal.DataEmissao = DateTime.Now;

                        xmlNotaFiscal.Numero = int.Parse(Utilidades.String.OnlyNumbers(notaFiscal.Numero));
                        xmlNotaFiscal.Serie = notaFiscal.Serie;
                        xmlNotaFiscal.Valor = notaFiscal.ValorTotal;
                        xmlNotaFiscal.Modelo = "01";
                        xmlNotaFiscal.Peso = notaFiscal.Peso;
                        xmlNotaFiscal.CFOP = notaFiscal.CFOP;
                        xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NotaFiscal;

                        CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, pedidoCTeParaSubContratacao, (!cargaPedido.Carga.TipoOperacao?.NaoGerarCanhoto ?? false), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, pedidoXMLNotasFiscais, null, pedidoCTesParaSubcontratacaoNotaFiscal, configuracaoCanhoto, lstInsertxmlNotaFiscal, lstUpdatexmlNotaFiscal, lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal, lstPedidoXMLNotaFiscal);
                    }
                }

                if (contadorRegistros > 500)
                {
                    unitOfWork.Flush();
                    contadorRegistros = 0;
                    Log.TratarErro($"For posição {i} de {pedidoCTesParaSubcontratacao.Count} pedidosubcontratacao da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}", "ConfirmarEnvioDosDocumentos");
                }
            }

            Log.TratarErro($"Finalizou for pedidosubcontratacao da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}", "ConfirmarEnvioDosDocumentos");

            if (lstInsertxmlNotaFiscal?.Count > 0 || lstUpdatexmlNotaFiscal?.Count > 0)
            {
                if (lstInsertxmlNotaFiscal?.Count > 0)
                {
                    Log.TratarErro($"Iniciou insert em massa da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}, com {lstInsertxmlNotaFiscal.Count} notas", "ConfirmarEnvioDosDocumentos");
                    repXMLNotaFiscal.Inserir(lstInsertxmlNotaFiscal, "T_XML_NOTA_FISCAL", 200);
                }

                if (lstUpdatexmlNotaFiscal?.Count > 0)
                {
                    Log.TratarErro($"Iniciou update individual da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}, com {lstUpdatexmlNotaFiscal.Count} notas", "ConfirmarEnvioDosDocumentos");
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in lstUpdatexmlNotaFiscal)
                        repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                }

                Log.TratarErro($"Iniciou insert em massa da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}, com {lstPedidoXMLNotaFiscal.Count} pedidosXML", "ConfirmarEnvioDosDocumentos");

                repPedidoXMLNotaFiscal.Inserir(lstPedidoXMLNotaFiscal, "T_PEDIDO_XML_NOTA_FISCAL", 100);
                repPedidoCTeParaSubcontratacaoPedidoNotaFiscal.Inserir(lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal, "T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL");

                Log.TratarErro($"Finalizou insert em massa da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}", "ConfirmarEnvioDosDocumentos");
            }

            if (controlarTransacao)
            {
                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }

            Log.TratarErro($"Finalizou CriarNotasFiscaisDaCargaPedido da carga {cargaPedido.Carga.Codigo}, cargaPedido {cargaPedido.Codigo}", "ConfirmarEnvioDosDocumentos");
        }

        public void CriarPedidoCTeParaSubContratacaoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, bool gerarCanhoto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal tipoNotaFiscal, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, List<int> codigosCargaPedidos = null, List<Dominio.ObjetosDeValor.Embarcador.CTe.PedidoCTeParaSubcontratacaoPedidoXMLNotaFiscal> pedidoCTesParaSubcontratacaoNotaFiscal = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = null
            , List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> lstInsertxmlNotaFiscal = null
            , List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> lstUpdatexmlNotaFiscal = null
            , List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal = null
            , List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> lstPedidoXMLNotaFiscal = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(unitOfWork);

            if (gerarCanhoto)
            {// canhoto ainda nao esta preparado para gerar insert por lista 
                lstInsertxmlNotaFiscal = null;
                lstUpdatexmlNotaFiscal = null;
                lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal = null;
                lstPedidoXMLNotaFiscal = null;
            }

            bool xmlNotaFiscalInserido = false;
            if (xmlNotaFiscal.Codigo == 0)
            {
                xmlNotaFiscal.DataRecebimento = DateTime.Now;
                if (lstInsertxmlNotaFiscal != null)
                    lstInsertxmlNotaFiscal.Add(xmlNotaFiscal);
                else
                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                xmlNotaFiscalInserido = true;
            }
            else
            {
                if (!xmlNotaFiscal.nfAtiva)
                    xmlNotaFiscal.nfAtiva = true;

                if (lstUpdatexmlNotaFiscal != null)
                    lstUpdatexmlNotaFiscal.Add(xmlNotaFiscal);
                else
                    repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            }

            if (gerarCanhoto)
            {
                if (configuracaoCanhoto == null)
                    configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto, pedidoCTeParaSubContratacao.CTeTerceiro.Numero.ToString(), cargaPedido.PedidoEncaixado);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;
            if (!xmlNotaFiscalInserido)
            {
                if (codigosCargaPedidos != null && codigosCargaPedidos.Count > 1)
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscalCargaPedidos(xmlNotaFiscal.Codigo, codigosCargaPedidos);
                else if (pedidoXMLNotasFiscais?.Count > 0)
                    pedidoXMLNotaFiscal = pedidoXMLNotasFiscais.Find(o => o.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo && o.CargaPedido.Codigo == cargaPedido.Codigo);
            }

            bool existeRegistroPedidoCTeParaSubContratacaoPedidoNotaFiscal = false;

            if (pedidoXMLNotaFiscal == null)
            {
                pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                pedidoXMLNotaFiscal.TipoNotaFiscal = tipoNotaFiscal;
                pedidoXMLNotaFiscal.PercentualAliquota = pedidoCTeParaSubContratacao.PercentualAliquota;
                pedidoXMLNotaFiscal.PercentualAliquotaInternaDifal = pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal;
                pedidoXMLNotaFiscal.CFOP = pedidoCTeParaSubContratacao.CFOP != null ? new Dominio.Entidades.CFOP() { Codigo = pedidoCTeParaSubContratacao.CFOP.CodigoCFOP } : null;
                pedidoXMLNotaFiscal.CST = pedidoCTeParaSubContratacao.CST;
                pedidoXMLNotaFiscal.ObservacaoRegraICMSCTe = pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe;
                pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo = pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo;
                pedidoXMLNotaFiscal.PercentualReducaoBC = pedidoCTeParaSubContratacao.PercentualReducaoBC;
                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal != null ? new Dominio.Entidades.ModeloDocumentoFiscal() { Codigo = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.Codigo } : null;
                pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
                pedidoXMLNotaFiscal.PossuiCTe = pedidoCTeParaSubContratacao.PossuiCTe;
                pedidoXMLNotaFiscal.PossuiNFS = pedidoCTeParaSubContratacao.PossuiNFS;
                pedidoXMLNotaFiscal.PossuiNFSManual = pedidoCTeParaSubContratacao.PossuiNFSManual;
                pedidoXMLNotaFiscal.BaseCalculoISS = pedidoCTeParaSubContratacao.BaseCalculoISS;
                pedidoXMLNotaFiscal.ValorRetencaoISS = pedidoCTeParaSubContratacao.ValorRetencaoISS;
                pedidoXMLNotaFiscal.PercentualAliquotaISS = pedidoCTeParaSubContratacao.PercentualAliquotaISS;
                pedidoXMLNotaFiscal.PercentualRetencaoISS = pedidoCTeParaSubContratacao.PercentualRetencaoISS;
                pedidoXMLNotaFiscal.IncluirISSBaseCalculo = pedidoCTeParaSubContratacao.IncluirISSBaseCalculo;
                pedidoXMLNotaFiscal.BaseCalculoIR = pedidoCTeParaSubContratacao.BaseCalculoIR;
                pedidoXMLNotaFiscal.AliquotaIR = pedidoCTeParaSubContratacao.AliquotaIR;
                pedidoXMLNotaFiscal.ReterIR = pedidoCTeParaSubContratacao.ReterIR;
                pedidoXMLNotaFiscal.ValorIR = pedidoCTeParaSubContratacao.ValorIR;
                pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = pedidoCTeParaSubContratacao.CTeTerceiro.CST == "60" && (pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber + pedidoCTeParaSubContratacao.CTeTerceiro.ValorICMS == pedidoCTeParaSubContratacao.CTeTerceiro.ValorPrestacaoServico);
                
                if (pedidoCTeParaSubContratacao.IBSCBS != null)
                {
                    Dominio.ObjetosDeValor.CTe.IBSCBS impostoIBSCBS = pedidoCTeParaSubContratacao.IBSCBS;
                    pedidoXMLNotaFiscal.ValorCBS = impostoIBSCBS.ValorCBS;
                    pedidoXMLNotaFiscal.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                    pedidoXMLNotaFiscal.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                    pedidoXMLNotaFiscal.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                    pedidoXMLNotaFiscal.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                    pedidoXMLNotaFiscal.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                    pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                    pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                    pedidoXMLNotaFiscal.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                    pedidoXMLNotaFiscal.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutrasAliquotas);
                    pedidoXMLNotaFiscal.NBS = impostoIBSCBS.NBS;
                    pedidoXMLNotaFiscal.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                    pedidoXMLNotaFiscal.CSTIBSCBS = impostoIBSCBS.CSTIBSCBS;
                    pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributariaIBSCBS;
                }

                if (lstPedidoXMLNotaFiscal != null)
                    lstPedidoXMLNotaFiscal.Add(pedidoXMLNotaFiscal);
                else
                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);
            }
            else
            {
                //se o pedidoxmlnotafiscal não existe (caso acima), não precisa buscar, pois não vai ter vínculo ainda... já se ele existe, faz a consulta
                if (pedidoCTesParaSubcontratacaoNotaFiscal == null)
                    existeRegistroPedidoCTeParaSubContratacaoPedidoNotaFiscal = repPedidoCTeParaSubContratacaoNotaFiscal.ExistePorCTeSubcontratacaoENota(pedidoXMLNotaFiscal.Codigo, pedidoCTeParaSubContratacao.Codigo);
                else
                    existeRegistroPedidoCTeParaSubContratacaoPedidoNotaFiscal = pedidoCTesParaSubcontratacaoNotaFiscal.Any(o => o.CodigoPedidoCTeParaSubcontratacao == pedidoCTeParaSubContratacao.Codigo && o.CodigoPedidoXMLNotaFiscal == pedidoXMLNotaFiscal.Codigo);
            }

            if (!existeRegistroPedidoCTeParaSubContratacaoPedidoNotaFiscal)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal
                {
                    PedidoCTeParaSubContratacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao() { Codigo = pedidoCTeParaSubContratacao.Codigo },
                    PedidoXMLNotaFiscal = pedidoXMLNotaFiscal
                };

                if (lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal != null)
                    lstInsertPedidoCTeParaSubContratacaoPedidoNotaFiscal.Add(pedidoCTeParaSubContratacaoNotaFiscal);
                else
                    repPedidoCTeParaSubContratacaoNotaFiscal.Inserir(pedidoCTeParaSubContratacaoNotaFiscal);
            }
        }

        public string ObterDescricaoItemPeso(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso)
        {
            utilizarPrimeiraUnidadeMedidaPeso = false;

            if (cargaPedido.Carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
            {
                utilizarPrimeiraUnidadeMedidaPeso = cargaPedido.Carga.TipoOperacao.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;

                if (!string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoOperacao.DescricaoItemPesoCTeSubcontratacao))
                    return cargaPedido.Carga.TipoOperacao.DescricaoItemPesoCTeSubcontratacao;
            }
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        utilizarPrimeiraUnidadeMedidaPeso = tomador.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                        return tomador.DescricaoItemPesoCTeSubcontratacao;
                    }
                    else if (tomador.GrupoPessoas != null)
                    {
                        utilizarPrimeiraUnidadeMedidaPeso = tomador.GrupoPessoas.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                        return tomador.GrupoPessoas.DescricaoItemPesoCTeSubcontratacao;
                    }
                }
            }

            return null;
        }

        public void SetarConfiguracaoModalAereo(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte)
        {
            if (cte.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo)
                return;

            if ((cte.NotasFiscais == null || cte.NotasFiscais.Count <= 0) &&
                (cte.OutrosDocumentos == null || cte.OutrosDocumentos.Count <= 0) &&
                (cte.NFEs == null || cte.NFEs.Count <= 0))
            {
                cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                    {
                        DataEmissao =cte.DataEmissao,
                        Descricao = string.Empty,
                        Numero = "1",
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                        Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0m
                    }
                };
            }
        }

        public bool SetarConfiguracaoCTeRedespachoIntermediario(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            bool importarCTeRedespachoIntermediario = false;
            Dominio.Entidades.Cliente expedidor = null;
            Dominio.Entidades.Cliente recebedor = null;
            Dominio.Entidades.Cliente emitente = null;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                importarCTeRedespachoIntermediario = cargaPedido.Carga.TipoOperacao.ImportarRedespachoIntermediario;
                expedidor = cargaPedido.Carga.TipoOperacao.ExpedidorImportacaoRedespachoIntermediario;
                recebedor = cargaPedido.Carga.TipoOperacao.RecebedorImportacaoRedespachoIntermediario;
                emitente = cargaPedido.Carga.TipoOperacao.EmitenteImportacaoRedespachoIntermediario;
            }
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        importarCTeRedespachoIntermediario = tomador.ImportarRedespachoIntermediario;
                        expedidor = tomador.ExpedidorImportacaoRedespachoIntermediario;
                        recebedor = tomador.RecebedorImportacaoRedespachoIntermediario;
                        emitente = tomador.EmitenteImportacaoRedespachoIntermediario;
                    }
                    else if (tomador.GrupoPessoas != null)
                    {
                        importarCTeRedespachoIntermediario = tomador.GrupoPessoas.ImportarRedespachoIntermediario;
                        expedidor = tomador.GrupoPessoas.ExpedidorImportacaoRedespachoIntermediario;
                        recebedor = tomador.GrupoPessoas.RecebedorImportacaoRedespachoIntermediario;
                        emitente = tomador.GrupoPessoas.EmitenteImportacaoRedespachoIntermediario;
                    }
                }
            }

            if (!importarCTeRedespachoIntermediario)
            {
                erro = "Não é possível importar um CT-e de Redespacho Intermediário.";
                return false;
            }

            if (cte.Remetente == null && cte.Expedidor != null)
            {
                cte.Remetente = cte.Expedidor;
                cte.Expedidor = null;
            }

            if (cte.Destinatario == null && cte.Recebedor != null)
            {
                cte.Destinatario = cte.Recebedor;
                cte.Recebedor = null;
            }

            if (emitente != null)
                cte.Emitente = Servicos.Embarcador.CTe.CTe.ObterEmpresaCTe(emitente);

            if (expedidor != null)
            {
                cte.Expedidor = Servicos.Embarcador.CTe.CTe.ObterParticipanteCTe(expedidor);
                cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;
            }

            if (recebedor != null)
            {
                cte.Recebedor = Servicos.Embarcador.CTe.CTe.ObterParticipanteCTe(recebedor);
                cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;
            }

            if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) && cte.Recebedor != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = repClienteOutroEndereco.BuscarPorPessoa(cte.Recebedor.CPFCNPJ.ToDouble())?.FirstOrDefault();

                if (endereco != null)
                {
                    cte.LocalidadeFimPrestacao = new Dominio.ObjetosDeValor.Localidade
                    {
                        IBGE = endereco.Localidade.CodigoIBGE,
                        Descricao = endereco.Localidade.Descricao,
                        SiglaUF = endereco.Localidade.Estado.Sigla
                    };
                }
            }

            if ((cte.NotasFiscais == null || cte.NotasFiscais.Count == 0) &&
                (cte.NFEs == null || cte.NFEs.Count == 0) &&
                (cte.OutrosDocumentos == null || cte.OutrosDocumentos.Count == 0) &&
                cte.DocumentosAnteriores != null &&
                cte.DocumentosAnteriores.Count > 0)
            {
                cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();

                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior in cte.DocumentosAnteriores)
                {
                    if (!string.IsNullOrWhiteSpace(documentoAnterior.ChaveAcesso) && documentoAnterior.ChaveAcesso.Length == 44)
                    {
                        cte.OutrosDocumentos.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                        {
                            DataEmissao = DateTime.Now,
                            Descricao = string.Empty,
                            Numero = documentoAnterior.ChaveAcesso.Substring(25, 9),
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                            Valor = Math.Round(cte.InformacaoCarga.ValorTotalCarga / cte.DocumentosAnteriores.Count, 2, MidpointRounding.AwayFromZero)
                        });
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        public bool ValidarComponentesCTesSubcontratacao(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repCTeTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrincipal = cargaPedidos.FirstOrDefault();

            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Embarcador.CTe.CTEsImportados(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete svcComponenteFrete = new ComponetesFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao = svcCTesImportados.ObterConfiguracoesComponentes(cargaPedidoPrincipal);

            svcComponenteFrete.RemoverComponentesCarga(carga, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.ValorFrete = 0m;
                cargaPedido.ValorFreteAPagar = 0m;

                svcComponenteFrete.RemoverComponentesCargaPedido(cargaPedido, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete> componentesAgrupadosCargaPedido = repCTeTerceiroComponenteFrete.BuscarComponentesAgrupadosPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete componente in componentesAgrupadosCargaPedido)
                {
                    if (configuracao.DescricaoComponenteFreteLiquido?.ToLower() == componente.Descricao.ToLower())
                    {
                        cargaPedido.ValorFrete += componente.Valor;
                        cargaPedido.ValorFreteAPagar += componente.Valor;
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente configuracaoComponente = configuracao.Componentes.Where(o => o.OutraDescricaoCTe.ToLower() == componente.Descricao.ToLower()).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;

                    if (configuracaoComponente != null)
                        componenteFrete = repComponenteFrete.BuscarPorCodigo(configuracaoComponente.Codigo);

                    if (componenteFrete == null)
                        componenteFrete = repComponenteFrete.BuscarPorDescricao(componente.Descricao);

                    if (componenteFrete == null)
                    {
                        erro = $"O componente {componente.Descricao} não foi encontrado, nas configurações da operação e dos componentes do sistema, para ser vinculado à carga. Configure-o e tente novamente.";
                        return false;
                    }

                    svcComponenteFrete.AdicionarCargaPedidoComponente(cargaPedido, componente.Valor, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, componenteFrete.TipoComponenteFrete, componenteFrete, true, false, null, null, componente.Descricao, false, false, false, unitOfWork, false, null, 0, null, false, false);
                    svcComponenteFrete.AdicionarComponenteFreteCarga(carga, componenteFrete, componente.Valor, 0m, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, componenteFrete.TipoComponenteFrete, null, true, false, null, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, false, false, null, false, null, 0m, 0m);
                }

                repCargaPedido.Atualizar(cargaPedido);
            }

            erro = string.Empty;
            return true;
        }

        public static Dominio.Entidades.Cliente ObterTomadorCTeParaSubcontratacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null)
        {
            Dominio.Entidades.Cliente tomador = pedidoCTeParaSubcontratacao.CTeTerceiro.Emitente?.Cliente;

            if (carga.TipoOperacao?.TomadorCTeSubcontratacaoDeveSerDoCTeOriginal ?? false)
                tomador = pedidoCTeParaSubcontratacao.CTeTerceiro.Tomador?.Cliente;
            else if (cargaPedido.Tomador != null)
            {
                if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    tomador = cargaPedido.Tomador;
                else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (modeloDocumentoEmitir == null)
                        modeloDocumentoEmitir = ObterModeloDocumentoEmitir(cargaPedido, pedidoCTeParaSubcontratacao);

                    if ((modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe) ||
                        cargaPedido.Tomador.CPF_CNPJ_SemFormato.StartsWith(pedidoCTeParaSubcontratacao.CTeTerceiro.Emitente.CPF_CNPJ.Substring(0, 8)))
                        tomador = cargaPedido.Tomador;
                }
            }

            if (tomador == null)
                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

            return tomador;
        }

        public void VincularCTeTerceiroACargaPedido(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfigGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfigGeralCarga.BuscarPrimeiroRegistro();

            if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario && !SetarConfiguracaoCTeRedespachoIntermediario(out string erro, cargaPedido, cte, unitOfWork))
                throw new ServicoException(erro);

            SetarConfiguracaoModalAereo(cte);

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado && (cte.Entregas?.Count ?? 0) <= 0)
            {
                throw new ServicoException("Não é possível importar um CT-e que não tenha notas fiscais vinculadas.");
            }
            else if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Simplificado && (cte.OutrosDocumentos?.Count ?? 0) <= 0 && (cte.NotasFiscais?.Count ?? 0) <= 0 && (cte.NFEs?.Count ?? 0) <= 0)
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> nfes = repCTeTerceiroNFe.BuscarPorChave(cte.Chave);
                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> outrosDocumentos = repCTeTerceiroOutrosDocumentos.BuscarPorChave(cte.Chave);

                if (nfes != null && nfes.Count > 0)
                {
                    cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();
                    foreach (var nfe in nfes)
                    {
                        int.TryParse(nfe.Numero, out int numeroNota);
                        Dominio.ObjetosDeValor.Embarcador.CTe.NFe nota = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                        {
                            DataEmissao = cte.DataEmissao,
                            Chave = nfe.Chave,
                            Numero = numeroNota
                        };
                        cte.NFEs.Add(nota);
                    }
                }
                else if (outrosDocumentos != null && outrosDocumentos.Count > 0)
                {
                    cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                    foreach (var outroDocumento in outrosDocumentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                        {
                            DataEmissao = cte.DataEmissao,
                            Descricao = outroDocumento.Descricao,
                            Numero = outroDocumento.Numero,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                            Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0,
                        };
                        cte.OutrosDocumentos.Add(outroDoc);
                    }
                }
                else if ((cte.DocumentosAnteriores?.Count ?? 0) > 0 || configuracaoEmbarcador.UtilizaEmissaoMultimodal || (configuracaoGeralCarga?.GerarOutrosDocumentosNaImportacaoDeCTeComplementar ?? false))
                {
                    cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                    Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                    {
                        DataEmissao = cte.DataEmissao,
                        Descricao = cte.Numero.ToString(),
                        Numero = cte.Numero.ToString(),
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                        Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0
                    };
                    cte.OutrosDocumentos.Add(outroDoc);
                }
                else
                    throw new ServicoException("Não é possível importar um CT-e que não tenha notas fiscais vinculadas.");
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
            string retorno = InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

            if (!string.IsNullOrEmpty(retorno))
                throw new ServicoException(retorno);
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao ConverterPedidoCTeParaSubContratacao(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao
            {
                Codigo = pedidoCTeParaSubcontratacao.Codigo,
                AliquotaIR = pedidoCTeParaSubcontratacao.AliquotaIR,
                BaseCalculoIR = pedidoCTeParaSubcontratacao.BaseCalculoIR,
                BaseCalculoISS = pedidoCTeParaSubcontratacao.BaseCalculoISS,
                CST = pedidoCTeParaSubcontratacao.CST,
                IncluirICMSBaseCalculo = pedidoCTeParaSubcontratacao.IncluirICMSBaseCalculo,
                IncluirISSBaseCalculo = pedidoCTeParaSubcontratacao.IncluirISSBaseCalculo,
                ObservacaoRegraICMSCTe = pedidoCTeParaSubcontratacao.ObservacaoRegraICMSCTe,
                PercentualAliquota = pedidoCTeParaSubcontratacao.PercentualAliquota,
                PercentualAliquotaInternaDifal = pedidoCTeParaSubcontratacao.PercentualAliquotaInternaDifal,
                PercentualAliquotaISS = pedidoCTeParaSubcontratacao.PercentualAliquotaISS,
                PercentualIncluirBaseCalculo = pedidoCTeParaSubcontratacao.PercentualIncluirBaseCalculo,
                PercentualReducaoBC = pedidoCTeParaSubcontratacao.PercentualReducaoBC,
                PercentualRetencaoISS = pedidoCTeParaSubcontratacao.PercentualRetencaoISS,
                PossuiCTe = pedidoCTeParaSubcontratacao.PossuiCTe,
                PossuiNFS = pedidoCTeParaSubcontratacao.PossuiNFS,
                PossuiNFSManual = pedidoCTeParaSubcontratacao.PossuiNFSManual,
                ReterIR = pedidoCTeParaSubcontratacao.ReterIR,
                ValorIR = pedidoCTeParaSubcontratacao.ValorIR,
                ValorRetencaoISS = pedidoCTeParaSubcontratacao.ValorRetencaoISS,
                CFOP = pedidoCTeParaSubcontratacao.CFOP == null ? null : new Dominio.ObjetosDeValor.Embarcador.Financeiro.CFOP()
                {
                    CodigoCFOP = pedidoCTeParaSubcontratacao.CFOP.Codigo
                },
                ModeloDocumentoFiscal = pedidoCTeParaSubcontratacao.ModeloDocumentoFiscal == null ? null : new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ModeloDocumentoFiscal()
                {
                    Codigo = pedidoCTeParaSubcontratacao.ModeloDocumentoFiscal.Codigo
                },
                CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                {
                    Codigo = pedidoCTeParaSubcontratacao.CTeTerceiro.Codigo,
                    CST = pedidoCTeParaSubcontratacao.CTeTerceiro.CST,
                    DataEmissao = pedidoCTeParaSubcontratacao.CTeTerceiro.DataEmissao,
                    Numero = pedidoCTeParaSubcontratacao.CTeTerceiro.Numero,
                    Peso = pedidoCTeParaSubcontratacao.CTeTerceiro.Peso,
                    Serie = pedidoCTeParaSubcontratacao.CTeTerceiro.Serie,
                    ValorAReceber = pedidoCTeParaSubcontratacao.CTeTerceiro.ValorAReceber,
                    ValorICMS = pedidoCTeParaSubcontratacao.CTeTerceiro.ValorICMS,
                    ValorPrestacaoServico = pedidoCTeParaSubcontratacao.CTeTerceiro.ValorPrestacaoServico,
                    Remetente = new Dominio.ObjetosDeValor.Cliente()
                    {
                        CPFCNPJ = pedidoCTeParaSubcontratacao.CTeTerceiro.Remetente.CPF_CNPJ_SemFormato,
                        Tipo = pedidoCTeParaSubcontratacao.CTeTerceiro.Remetente.Cliente != null ? pedidoCTeParaSubcontratacao.CTeTerceiro.Remetente.Cliente.Tipo : string.Empty
                    },
                    Destinatario = new Dominio.ObjetosDeValor.Cliente()
                    {
                        CPFCNPJ = pedidoCTeParaSubcontratacao.CTeTerceiro.Destinatario.CPF_CNPJ_SemFormato
                    }
                },
                IBSCBS = new Dominio.ObjetosDeValor.CTe.IBSCBS
                {
                    AliquotaCBS = pedidoCTeParaSubcontratacao.AliquotaCBS,
                    AliquotaIBSEstadual =  pedidoCTeParaSubcontratacao.AliquotaIBSEstadual,
                    AliquotaIBSMunicipal = pedidoCTeParaSubcontratacao.AliquotaIBSMunicipal,
                    BaseCalculoIBSCBS = pedidoCTeParaSubcontratacao.BaseCalculoIBSCBS,
                    ClassificacaoTributariaIBSCBS = pedidoCTeParaSubcontratacao.ClassificacaoTributariaIBSCBS,
                    CodigoIndicadorOperacao = pedidoCTeParaSubcontratacao.CodigoIndicadorOperacao,
                    CSTIBSCBS = pedidoCTeParaSubcontratacao.CSTIBSCBS,
                    CodigoOutrasAliquotas = pedidoCTeParaSubcontratacao?.OutrasAliquotas?.Codigo ?? 0,
                    NBS = pedidoCTeParaSubcontratacao.NBS,
                    PercentualReducaoCBS = pedidoCTeParaSubcontratacao.PercentualReducaoCBS,
                    PercentualReducaoIBSEstadual = pedidoCTeParaSubcontratacao.PercentualReducaoIBSEstadual,
                    PercentualReducaoIBSMunicipal = pedidoCTeParaSubcontratacao.PercentualReducaoIBSMunicipal,
                    ValorCBS = pedidoCTeParaSubcontratacao.ValorCBS,
                    ValorIBSEstadual = pedidoCTeParaSubcontratacao.ValorIBSEstadual,
                    ValorIBSMunicipal = pedidoCTeParaSubcontratacao.ValorIBSMunicipal
                }
            };
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void ObterValorNotasFiscais(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, out bool utilizarValorCalculado, out decimal valorNotaFiscalCalculado, out decimal valorNotaFiscalRestanteCalculado)
        {
            utilizarValorCalculado = cte.NFEs.All(o => o.Valor <= 0m);
            valorNotaFiscalCalculado = 0m;
            valorNotaFiscalRestanteCalculado = 0m;

            if (utilizarValorCalculado)
            {
                decimal valorTotalCarga = cte.InformacaoCarga?.ValorTotalCarga ?? 0m;
                int quantidadeNotasFiscais = cte.NFEs.Count;

                if (valorTotalCarga > 0m && quantidadeNotasFiscais > 0)
                {
                    valorNotaFiscalCalculado = Math.Floor(valorTotalCarga / quantidadeNotasFiscais * 100) / 100;
                    valorNotaFiscalRestanteCalculado = valorTotalCarga - (valorNotaFiscalCalculado * quantidadeNotasFiscais);
                }
            }
        }

        private string PreencherNFes(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }
            string retorno = "";
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeParaSubContratacaoNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            repCTeParaSubContratacaoNFe.DeletarPorCTeTerceiro(cteTerceiro.Codigo, objetoValorPersistente);
            if (cte.NFEs != null)
            {
                ObterValorNotasFiscais(cte, out bool utilizarValorNotaCalculado, out decimal valorNotaCalculado, out decimal valorNotaRestanteCalculado);

                for (int i = 0; i < cte.NFEs.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe = cte.NFEs[i];

                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteParaSubContratacaoNFe = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe();

                    cteParaSubContratacaoNFe.CTeTerceiro = cteTerceiro;

                    if (nfe.DataEmissao != DateTime.MinValue)
                        cteParaSubContratacaoNFe.DataEmissao = nfe.DataEmissao;
                    else
                        cteParaSubContratacaoNFe.DataEmissao = DateTime.Now;

                    cteParaSubContratacaoNFe.Chave = nfe.Chave.Trim().Replace(" ", "");
                    cteParaSubContratacaoNFe.Numero = nfe.Numero.ToString();
                    cteParaSubContratacaoNFe.Serie = nfe.SerieDaChave;

                    if (utilizarValorNotaCalculado)
                    {
                        cteParaSubContratacaoNFe.ValorTotal = valorNotaCalculado;

                        if (i == cte.NFEs.Count - 1)
                            cteParaSubContratacaoNFe.ValorTotal += valorNotaRestanteCalculado;
                    }
                    else
                        cteParaSubContratacaoNFe.ValorTotal = nfe.Valor;

                    cteParaSubContratacaoNFe.Peso = nfe.Peso;
                    cteParaSubContratacaoNFe.PesoCubado = nfe.PesoCubado;
                    cteParaSubContratacaoNFe.Volumes = nfe.Volumes;
                    cteParaSubContratacaoNFe.NumeroRomaneio = nfe.NumeroRomaneio;
                    cteParaSubContratacaoNFe.NumeroPedido = nfe.NumeroPedido;
                    cteParaSubContratacaoNFe.Protocolo = nfe.Protocolo;
                    cteParaSubContratacaoNFe.NumeroReferenciaEDI = nfe.NumeroReferenciaEDI;
                    cteParaSubContratacaoNFe.NumeroControleCliente = nfe.NumeroControleCliente;
                    cteParaSubContratacaoNFe.PINSuframa = nfe.PINSuframa;
                    cteParaSubContratacaoNFe.NCM = nfe.NCMPredominante;

                    if (objetoValorPersistente == null)
                        repCTeParaSubContratacaoNFe.Inserir(cteParaSubContratacaoNFe);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacaoNFe);

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(nfe.Chave.Trim().Replace(" ", ""));
                    if (xmlNotaFiscal != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nfe.NCMPredominante))
                            xmlNotaFiscal.NCM = nfe.NCMPredominante;
                        if (!string.IsNullOrWhiteSpace(nfe.PINSuframa))
                            xmlNotaFiscal.PINSUFRAMA = nfe.PINSuframa;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroReferenciaEDI))
                            xmlNotaFiscal.NumeroReferenciaEDI = nfe.NumeroReferenciaEDI;
                        if (!string.IsNullOrWhiteSpace(nfe.NumeroControleCliente))
                            xmlNotaFiscal.NumeroControleCliente = nfe.NumeroControleCliente;
                        repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                    }
                }
            }

            return retorno;
        }

        private string PreencherNotasFiscais(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }
            string retorno = "";
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            repCTeParaSubContratacaoNotaFiscal.DeletarPorCTeTerceiro(cteTerceiro.Codigo, objetoValorPersistente);

            if (CTe.NotasFiscais != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal in CTe.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal cteParaSubContratacaoNotaFiscal = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal();
                    cteParaSubContratacaoNotaFiscal.CTeTerceiro = cteTerceiro;
                    if (notaFiscal.DataEmissao != DateTime.MinValue)
                        cteParaSubContratacaoNotaFiscal.DataEmissao = notaFiscal.DataEmissao;
                    else
                        cteParaSubContratacaoNotaFiscal.DataEmissao = DateTime.Now;

                    cteParaSubContratacaoNotaFiscal.Numero = notaFiscal.Numero;
                    cteParaSubContratacaoNotaFiscal.Serie = notaFiscal.Serie;
                    cteParaSubContratacaoNotaFiscal.ValorTotal = notaFiscal.Valor;
                    cteParaSubContratacaoNotaFiscal.Peso = notaFiscal.Peso;
                    cteParaSubContratacaoNotaFiscal.CFOP = notaFiscal.CFOP;
                    if (objetoValorPersistente == null)
                        repCTeParaSubContratacaoNotaFiscal.Inserir(cteParaSubContratacaoNotaFiscal);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacaoNotaFiscal);
                }
            }

            return retorno;
        }

        private string PreencherEntregasSimplificado(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }
            string retorno = "";
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            repCTeParaSubContratacaoNotaFiscal.DeletarPorCTeTerceiro(cteTerceiro.Codigo, objetoValorPersistente);

            throw new Exception("Processo de CT-e Simplificado Respacho/SubContratacação não homologado.");

            if (CTe.Entregas != null)
            {
                /*
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal in CTe.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal cteParaSubContratacaoNotaFiscal = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal();
                    cteParaSubContratacaoNotaFiscal.CTeTerceiro = cteTerceiro;
                    if (notaFiscal.DataEmissao != DateTime.MinValue)
                        cteParaSubContratacaoNotaFiscal.DataEmissao = notaFiscal.DataEmissao;
                    else
                        cteParaSubContratacaoNotaFiscal.DataEmissao = DateTime.Now;

                    cteParaSubContratacaoNotaFiscal.Numero = notaFiscal.Numero;
                    cteParaSubContratacaoNotaFiscal.Serie = notaFiscal.Serie;
                    cteParaSubContratacaoNotaFiscal.ValorTotal = notaFiscal.Valor;
                    cteParaSubContratacaoNotaFiscal.Peso = notaFiscal.Peso;
                    cteParaSubContratacaoNotaFiscal.CFOP = notaFiscal.CFOP;
                    if (objetoValorPersistente == null)
                        repCTeParaSubContratacaoNotaFiscal.Inserir(cteParaSubContratacaoNotaFiscal);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacaoNotaFiscal);
                }
                */
            }

            return retorno;
        }

        private string PreencherOutrosDocumentos(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }
            string retorno = "";
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeParaSubContratacaoOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            repCTeParaSubContratacaoOutrosDocumentos.DeletarPorCTeTerceiro(cteTerceiro.Codigo, objetoValorPersistente);


            if (CTe.OutrosDocumentos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc in CTe.OutrosDocumentos)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos cteParaSubContratacaoOutrosDocumentos = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos();
                    cteParaSubContratacaoOutrosDocumentos.CTeTerceiro = cteTerceiro;
                    cteParaSubContratacaoOutrosDocumentos.Descricao = outroDoc.Descricao;
                    cteParaSubContratacaoOutrosDocumentos.Numero = outroDoc.Numero;
                    cteParaSubContratacaoOutrosDocumentos.Tipo = outroDoc.Tipo;
                    cteParaSubContratacaoOutrosDocumentos.Valor = outroDoc.Valor;
                    if (objetoValorPersistente == null)
                        repCTeParaSubContratacaoOutrosDocumentos.Inserir(cteParaSubContratacaoOutrosDocumentos);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacaoOutrosDocumentos);
                }
            }
            return retorno;
        }

        private string PreencherSeguros(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }

            string retorno = "";
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            repCTeParaSubContratacaoSeguro.DeletarPorCTeTerceiro(cteParaSubContratacao.Codigo, objetoValorPersistente);

            if (CTe.Seguros != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro in CTe.Seguros)
                {
                    if (!string.IsNullOrWhiteSpace(seguro.Seguradora) || !string.IsNullOrWhiteSpace(seguro.Apolice))
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro cteParaSubContratacaoSeguro = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro();
                        cteParaSubContratacaoSeguro.CTeTerceiro = cteParaSubContratacao;
                        cteParaSubContratacaoSeguro.NomeSeguradora = seguro.Seguradora != null ? System.Web.HttpUtility.HtmlDecode(seguro.Seguradora) : "";
                        cteParaSubContratacaoSeguro.NumeroApolice = seguro.Apolice != null ? seguro.Apolice : "";
                        cteParaSubContratacaoSeguro.NumeroAverbacao = seguro.Averbacao != null ? seguro.Averbacao : "";
                        cteParaSubContratacaoSeguro.Tipo = seguro.ResponsavelSeguro;
                        cteParaSubContratacaoSeguro.Valor = seguro.Valor;
                        if (objetoValorPersistente == null)
                            repCTeParaSubContratacaoSeguro.Inserir(cteParaSubContratacaoSeguro);
                        else
                            objetoValorPersistente.Inserir(cteParaSubContratacaoSeguro);
                    }
                }
            }
            return retorno;
        }

        private void PreencherComponentesFrete(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }
            Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repCTeTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(unitOfWork);

            repCTeTerceiroComponenteFrete.DeletarPorCTeTerceiro(cteParaSubContratacao.Codigo, objetoValorPersistente);

            if (cte.ValorFrete?.ComponentesAdicionais != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteFrete in cte.ValorFrete.ComponentesAdicionais)
                {
                    if (!string.IsNullOrWhiteSpace(componenteFrete.Componente?.Descricao))
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete cteTerceiroComponenteFrete = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete
                        {
                            CTeTerceiro = cteParaSubContratacao,
                            Descricao = componenteFrete.Componente.Descricao,
                            Valor = componenteFrete.ValorComponente
                        };
                        if (objetoValorPersistente == null)
                            repCTeTerceiroComponenteFrete.Inserir(cteTerceiroComponenteFrete);
                        else
                            objetoValorPersistente.Inserir(cteTerceiroComponenteFrete);

                    }
                }
            }
        }

        private string SalvarXMLCTe(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }

            string retorno = "";

            if (string.IsNullOrWhiteSpace(CTe.Xml))
                return retorno;

            Repositorio.Embarcador.CTe.CTeTerceiroXML repCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(unitOfWork);

            if (repCTeTerceiroXML.ExisteCteTerceiroXML(cteTerceiro.Codigo))
                return retorno;

            Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML();
            cteTerceiroXML.CTeTerceiro = cteTerceiro;
            cteTerceiroXML.XML = CTe.Xml;
            repCTeTerceiroXML.Inserir(cteTerceiroXML);

            return retorno;
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML Passo1De2CriacaoCTeTerceiroXMLPorPacote(Dominio.Entidades.Embarcador.Cargas.Pacote pacote, string xml)
        {
            if (xml != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML();
                cteTerceiroXML.XML = xml;
                cteTerceiroXML.Pacote = pacote;
                return cteTerceiroXML;
            }
            return null;
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML Passo1De2CriacaoCTeTerceiroXMLPorPacoteWebHook(Dominio.Entidades.Embarcador.Cargas.PacoteWebHook pacoteWebHook, string xml)
        {
            if (xml != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML();
                cteTerceiroXML.XML = xml;
                cteTerceiroXML.PacoteWebHook = pacoteWebHook;
                return cteTerceiroXML;
            }
            return null;
        }

        private string PreencherQuantidades(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, string descricaoItemPeso, bool utilizarPrimeiraUnidadeMedidaPeso, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }

            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeparaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);

            repCTeparaSubContratacaoQuantidade.DeletarPorCTeTerceiro(cteParaSubContratacao.Codigo, objetoValorPersistente);

            string retorno = "";
            decimal pesoKG = 0m;

            if (CTe.QuantidadesCarga?.Count > 0)
            {
                int posicaoCTe = 0;
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade in CTe.QuantidadesCarga)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteParaSubContratacaoQuantidade = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade
                    {
                        CTeTerceiro = cteParaSubContratacao,
                        Quantidade = quantidade.Quantidade,
                        QuantidadeOriginal = quantidade.Quantidade,
                        TipoMedida = quantidade.Medida,
                        Unidade = quantidade.Unidade
                    };

                    if (objetoValorPersistente == null)
                        repCTeparaSubContratacaoQuantidade.Inserir(cteParaSubContratacaoQuantidade);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacaoQuantidade);


                    pesoKG += ObterPesoEmKG(cteParaSubContratacaoQuantidade, CTe.QuantidadesCarga.Count, posicaoCTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
                    posicaoCTe++;
                }
            }

            cteParaSubContratacao.DescricaoItemPeso = descricaoItemPeso;
            cteParaSubContratacao.Peso = pesoKG;

            if (objetoValorPersistente == null)
                repCTeTerceiro.Atualizar(cteParaSubContratacao);
            else
                objetoValorPersistente.Atualizar(cteParaSubContratacao);

            return retorno;
        }

        private string preencherDadosCTe(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null)
        {
            if (cacheObjetoValorCTe == null)
            {
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
                cacheObjetoValorCTe.CacheAtivo = false;
            }

            if (!cacheObjetoValorCTe.CacheAtivo)
                Log.TratarErro($"Inicio preencherDadosCTe cte: {cte.Codigo} cfop: {cte.CFOP} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacaoDadosCTe");

            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            string retorno = "";
            cteParaSubContratacao.ChaveAcesso = cte.Chave;
            cteParaSubContratacao.Numero = cte.Numero;
            cteParaSubContratacao.Serie = cte.Serie;

            if (!string.IsNullOrWhiteSpace(cteParaSubContratacao.ChaveAcesso) && cteParaSubContratacao.ChaveAcesso.Length == 44)
            {
                if (cteParaSubContratacao.Numero <= 0)
                {
                    int numero = 0;

                    if (int.TryParse(cteParaSubContratacao.ChaveAcesso.Substring(25, 9), out numero))
                        cteParaSubContratacao.Numero = numero;
                }

                if (string.IsNullOrWhiteSpace(cteParaSubContratacao.Serie))
                    cteParaSubContratacao.Serie = cteParaSubContratacao.ChaveAcesso.Substring(22, 3);
            }

            cteParaSubContratacao.AliquotaICMS = cte.ValorFrete?.ICMS?.Aliquota ?? 0m;
            cteParaSubContratacao.BaseCalculoICMS = cte.ValorFrete?.ICMS?.ValorBaseCalculoICMS ?? 0m;

            cteParaSubContratacao.ChaveCTEReferenciado = cte.ChaveCTeComplementado;

            //if (CTe.NFEs != null)
            //{
            //    //cteParaSubContratacao.ChavesNFe = new List<string>();
            //    foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe in CTe.NFEs)
            //    {


            //        cteParaSubContratacao.ChavesNFe.Add(nfe.Chave);
            //    }
            //}

            cteParaSubContratacao.CST = cte.ValorFrete?.ICMS?.CST ?? string.Empty;

            if (cte.DataEmissao != DateTime.MinValue)
                cteParaSubContratacao.DataEmissao = cte.DataEmissao;
            else
                cteParaSubContratacao.DataEmissao = DateTime.Now;

            cteParaSubContratacao.InformacaoAdicionalContribuinte = cte.InformacaoAdicionalContribuinte;
            cteParaSubContratacao.InformacaoAdicionalFisco = cte.InformacaoAdicionalFisco;

            if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario)
                cteParaSubContratacao.Lotacao = cte.ModalRodoviario?.Lotacao ?? false;

            cteParaSubContratacao.OutrasCaracteristicasDaCarga = cte.InformacaoCarga?.OutrasCaracteristicas;

            cteParaSubContratacao.PercentualReducaoBaseCalculoICMS = cte.ValorFrete?.ICMS?.PercentualReducaoBC ?? 0m;
            cteParaSubContratacao.ProdutoPredominante = cte.InformacaoCarga?.ProdutoPredominante ?? string.Empty;
            cteParaSubContratacao.SimplesNacional = cte.Emitente.SimplesNacional;
            cteParaSubContratacao.TipoCTE = cte.TipoCTE;
            cteParaSubContratacao.TipoPagamento = cte.TipoPagamento;
            cteParaSubContratacao.TipoServico = cte.TipoServico;
            cteParaSubContratacao.TipoTomador = cte.TipoTomador;
            cteParaSubContratacao.ValorAReceber = cte.ValorFrete?.ValorTotalAReceber ?? 0m;
            cteParaSubContratacao.ValorICMS = cte.ValorFrete?.ICMS?.ValorICMS ?? 0m;
            cteParaSubContratacao.ValorPrestacaoServico = cte.ValorFrete?.ValorPrestacaoServico ?? 0m;
            cteParaSubContratacao.ValorTotalMercadoria = cte.InformacaoCarga?.ValorTotalCarga ?? 0m;
            cteParaSubContratacao.ValorTotalMercadoriaOriginal = cte.InformacaoCarga?.ValorTotalCarga ?? 0m;
            cteParaSubContratacao.Versao = cte.Versao;
            cteParaSubContratacao.NumeroRomaneio = cte.NumeroRomaneio;
            cteParaSubContratacao.NumeroPedido = cte.NumeroPedido;
            cteParaSubContratacao.NumeroOperacionalConhecimentoAereo = cte.ModalAereo?.NumeroOperacionalConhecimentoAereo;
            cteParaSubContratacao.NumeroMinuta = cte.ModalAereo?.NumeroMinuta;
            cteParaSubContratacao.Modal = cte.TipoModal;
            cteParaSubContratacao.ObservacaoGeral = cte.InformacaoAdicionalContribuinte;

            preencherInicioFimPrestacao(ref cteParaSubContratacao, unitOfWork, cte, cacheObjetoValorCTe.lstLocalidades);

            if (cte.CFOP > 0)
            {
                cteParaSubContratacao.CFOP = repCFOP.BuscarPorCFOP(cte.CFOP, Dominio.Enumeradores.TipoCFOP.Saida, cacheObjetoValorCTe.lstCFOP);
                if (!cacheObjetoValorCTe.CacheAtivo)
                    Log.TratarErro($"Buca CFOP cte: {cte.Codigo} cfop: {cteParaSubContratacao.CFOP?.Codigo ?? 0} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacaoDadosCTe");
            }
            else
            {
                Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
                Dominio.Entidades.Aliquota aliquota = serICMS.ObterAliquota(cteParaSubContratacao.Emitente.Localidade.Estado, cteParaSubContratacao.LocalidadeInicioPrestacao.Estado, cteParaSubContratacao.LocalidadeTerminoPrestacao.Estado, cteParaSubContratacao.Tomador.Atividade, cteParaSubContratacao.Destinatario.Atividade, unitOfWork);
                cteParaSubContratacao.CFOP = aliquota.CFOP;
                if (!cacheObjetoValorCTe.CacheAtivo)
                    Log.TratarErro($"CFOP ALÍQUOTA cte: {cte.Codigo} cfop: {cteParaSubContratacao.CFOP?.Codigo ?? 0} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacaoDadosCTe");
            }

            return retorno;
        }

        private string preencherInicioFimPrestacao(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            string retorno = "";
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorDescricaoEUF(CTe.LocalidadeInicioPrestacao.Descricao, CTe.LocalidadeInicioPrestacao.SiglaUF, lstLocalidades);
            if (origem == null)
                origem = cteParaSubContratacao.Expedidor != null ? cteParaSubContratacao.Expedidor.Localidade : cteParaSubContratacao.Remetente.Localidade;

            Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorDescricaoEUF(CTe.LocalidadeFimPrestacao.Descricao, CTe.LocalidadeFimPrestacao.SiglaUF, lstLocalidades);
            if (destino == null)
                destino = cteParaSubContratacao.Recebedor != null ? cteParaSubContratacao.Recebedor.Localidade : cteParaSubContratacao.Destinatario.Localidade;

            cteParaSubContratacao.LocalidadeInicioPrestacao = origem;
            cteParaSubContratacao.LocalidadeTerminoPrestacao = destino;

            return retorno;
        }

        private bool cacheAtivo(Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe)
        {
            return cacheObjetoValorCTe != null && cacheObjetoValorCTe.CacheAtivo;
        }

        private string preencherParticipantes(ref Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            if (!cacheAtivo(cacheObjetoValorCTe))
                Log.TratarErro($"Inicio preencherParticipantes - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
            string retorno = "";

            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);

            Servicos.Cliente servicoCliente = new Servicos.Cliente(StringConexao);
            int codEmpresa = 0;
            if (cargaPedido != null)
                codEmpresa = cargaPedido.Carga.Empresa != null ? cargaPedido.Carga.Empresa.Codigo : 0;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoRemetente = servicoCliente.ConverterObjetoValorPessoa(CTe.Remetente, "Remetente", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);

            if (!cacheAtivo(cacheObjetoValorCTe))
                Log.TratarErro($"retornoConversaoRemetente - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");

            if (retornoConversaoRemetente.Status)
            {
                cteParaSubContratacao.Remetente = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoRemetente.cliente);
                if (objetoValorPersistente == null)
                    repParticipanteCTe.Inserir(cteParaSubContratacao.Remetente);
                else
                    objetoValorPersistente.Inserir(cteParaSubContratacao.Remetente);
            }
            else if (CTe.Expedidor != null)
            {
                retornoConversaoRemetente = servicoCliente.ConverterObjetoValorPessoa(CTe.Expedidor, "Remetente", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
                if (!cacheAtivo(cacheObjetoValorCTe))
                    Log.TratarErro($"Expedidor - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
                if (retornoConversaoRemetente.Status)
                {
                    cteParaSubContratacao.Remetente = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoRemetente.cliente);
                    if (objetoValorPersistente == null)
                        repParticipanteCTe.Inserir(cteParaSubContratacao.Remetente);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacao.Remetente);
                }
                else
                    retorno += retornoConversaoRemetente.Mensagem;
            }
            else
                retorno += retornoConversaoRemetente.Mensagem;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoDestinatario = servicoCliente.ConverterObjetoValorPessoa(CTe.Destinatario, "Destinatário", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
            if (!cacheAtivo(cacheObjetoValorCTe))
                Log.TratarErro($"retornoConversaoDestinatario - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
            if (retornoConversaoDestinatario.Status)
            {
                cteParaSubContratacao.Destinatario = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoDestinatario.cliente);
                if (objetoValorPersistente == null)
                    repParticipanteCTe.Inserir(cteParaSubContratacao.Destinatario);
                else
                    objetoValorPersistente.Inserir(cteParaSubContratacao.Destinatario);

            }
            else if (CTe.Recebedor != null)
            {
                retornoConversaoDestinatario = servicoCliente.ConverterObjetoValorPessoa(CTe.Recebedor, "Destinatário", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
                if (retornoConversaoDestinatario.Status)
                {
                    cteParaSubContratacao.Destinatario = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoDestinatario.cliente);
                    if (objetoValorPersistente == null)
                        repParticipanteCTe.Inserir(cteParaSubContratacao.Destinatario);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacao.Destinatario);
                }
                else
                    retorno += retornoConversaoDestinatario.Mensagem;
            }
            else
                retorno += retornoConversaoDestinatario.Mensagem;

            if (CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoTomador = servicoCliente.ConverterObjetoValorPessoa(CTe.Tomador, "Tomador", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
                if (!cacheAtivo(cacheObjetoValorCTe))
                    Log.TratarErro($"retornoConversaoTomador - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
                if (retornoConversaoTomador.Status)
                {
                    cteParaSubContratacao.OutrosTomador = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoTomador.cliente);
                    if (objetoValorPersistente == null)
                        repParticipanteCTe.Inserir(cteParaSubContratacao.OutrosTomador);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacao.OutrosTomador);
                }
                else
                    retorno += retornoConversaoTomador.Mensagem;
            }

            if (CTe.Expedidor != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoExpedidor = servicoCliente.ConverterObjetoValorPessoa(CTe.Expedidor, "Expedidor", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
                if (!cacheAtivo(cacheObjetoValorCTe))
                    Log.TratarErro($"retornoConversaoExpedidor - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
                if (retornoConversaoExpedidor.Status)
                {
                    cteParaSubContratacao.Expedidor = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoExpedidor.cliente);
                    if (objetoValorPersistente == null)
                        repParticipanteCTe.Inserir(cteParaSubContratacao.Expedidor);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacao.Expedidor);
                }
                else
                    retorno += retornoConversaoExpedidor.Mensagem;
            }

            if (CTe.Recebedor != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoRecebedor = servicoCliente.ConverterObjetoValorPessoa(CTe.Recebedor, "Recebedor", unitOfWork, codEmpresa, false, false, null, tipoServicoMultisoftware, false, false, null, cacheObjetoValorCTe, objetoValorPersistente);
                if (!cacheAtivo(cacheObjetoValorCTe))
                    Log.TratarErro($"retornoConversaoRecebedor - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
                if (retornoConversaoRecebedor.Status)
                {
                    cteParaSubContratacao.Recebedor = servicoCliente.ConverterClienteParaParticipanteCTe(retornoConversaoRecebedor.cliente);
                    if (objetoValorPersistente == null)
                        repParticipanteCTe.Inserir(cteParaSubContratacao.Recebedor);
                    else
                        objetoValorPersistente.Inserir(cteParaSubContratacao.Recebedor);
                }
                else
                    retorno += retornoConversaoRecebedor.Mensagem;
            }
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoEmissor = servicoCliente.ConverterParaTransportadorTerceiro(CTe.Emitente, "Emitente do CT-e", unitOfWork, false, null, null, cacheObjetoValorCTe, objetoValorPersistente);
            if (!cacheAtivo(cacheObjetoValorCTe))
                Log.TratarErro($"retornoConversaoEmissor - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
            if (retornoConversaoEmissor.Status)
            {
                cteParaSubContratacao.TransportadorTerceiro = retornoConversaoEmissor.cliente;
                cteParaSubContratacao.Emitente = servicoCliente.ConverterClienteParaParticipanteCTe(cteParaSubContratacao.TransportadorTerceiro);
                if (objetoValorPersistente == null)
                    repParticipanteCTe.Inserir(cteParaSubContratacao.Emitente);
                else
                    objetoValorPersistente.Inserir(cteParaSubContratacao.Emitente);
            }
            else
                retorno += retornoConversaoEmissor.Mensagem;

            if (!cacheAtivo(cacheObjetoValorCTe))
                Log.TratarErro($"Fim preencherParticipantes - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
            return retorno;
        }

        public string ValidarRegrasCTeParaSubContratacao(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            //todo: criar aqui as regras dos CTes de Sub Contratacao que serão permitidas ou não por embarcador no TMS
            string mensagem = "";

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeParaSubContratacao = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeTerceiroNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (!repCTeTerceiroNFe.ExistePorCTeTerceiro(cteParaSubContratacao.Codigo) &&
                !repCTeTerceiroOutrosDocumentos.ExistePorCTeTerceiro(cteParaSubContratacao.Codigo) &&
                !repCTeTerceiroNotaFiscal.ExistePorCTeTerceiro(cteParaSubContratacao.Codigo))
                return "Este CT-e de subcontratação não possui nenhuma NF-e/NF/Outros Documentos vinculados, não sendo possível adicioná-lo à carga.";

            if (!configuracao.PermitirAutalizarNotaFiscalCarga && !cargaPedido.PedidoEncaixado)
            {
                if (repPedidoCTeParaSubContratacao.ContarPorCargaEChave(cargaPedido.Carga.Codigo, cteParaSubContratacao.ChaveAcesso) > 0)
                    return $"Este CT-e de subcontratação (chave: {cteParaSubContratacao.ChaveAcesso}) já foi informado nesta carga.";

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<string> chavesNFe = repCTeTerceiroNFe.BuscarChavePorCTeTerceiro(cteParaSubContratacao.Codigo);
                    List<int> numeroCTes = repPedidoCTeParaSubContratacao.BuscarNumeroPorCargaEChaveNFe(cargaPedido.Carga.Codigo, chavesNFe);

                    if (numeroCTes.Any())
                        return $"Este CT-e de subcontratação possui notas fiscais que já estão vinculadas ao(s) CT-e(s) {string.Join(", ", numeroCTes)}. Não é possível adicionar CT-es emitidos com as mesmas notas na mesma carga.";
                }
            }

            int numeroNF = repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);

            if (numeroNF > 0 && !cargaPedido.Carga.CargaRecebidaDeIntegracao)
                return "Não é possível enviar CT-es anteriores para o pedido pois o mesmo possui notas fiscais vinculadas e ele, remova as notas fiscais para poder enviar os CT-es anteriores.";
            else
            {
                if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                {
                    if (!serCarga.AtualizarTipoServicoCargaMultimodal(cteParaSubContratacao, cargaPedido, unitOfWork, out string msgRetornoTipoServico))
                        return msgRetornoTipoServico;
                }
                else if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                {
                    if (cargaPedido.Expedidor != null && cargaPedido.Recebedor != null)
                    {
                        if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                        else
                            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                    }
                    else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                    if ((configuracao.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao) || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false))
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                    if (!cargaPedido.Carga.UtilizarCTesAnterioresComoCTeFilialEmissora)
                        cargaPedido.CargaPedidoFilialEmissora = false;
                }

                cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                string raizCNPJTransportadorTerceiro = string.Empty;
                bool alterarTomador = true;

                if (!string.IsNullOrWhiteSpace(cteParaSubContratacao.TransportadorTerceiro?.CPF_CNPJ_SemFormato) && cteParaSubContratacao.TransportadorTerceiro.CPF_CNPJ_SemFormato.Length > 8)
                    raizCNPJTransportadorTerceiro = cteParaSubContratacao.TransportadorTerceiro.CPF_CNPJ_SemFormato.Substring(0, 8);

                // ConsiderarTomadorPedido
                if ((cargaPedido.Carga?.TipoOperacao?.ConsiderarTomadorPedido ?? false) &&
                    cargaPedido.Pedido?.Tomador != null)
                {
                    cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                    alterarTomador = false;
                }

                // NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao
                if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao ?? false) &&
                    cargaPedido.Pedido.Tomador != null &&
                    !string.IsNullOrWhiteSpace(raizCNPJTransportadorTerceiro) &&
                    cargaPedido.Pedido.Tomador.CPF_CNPJ_SemFormato.StartsWith(raizCNPJTransportadorTerceiro))
                {
                    cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                    alterarTomador = false;
                }

                if (alterarTomador)
                {
                    cargaPedido.Tomador = cteParaSubContratacao.TransportadorTerceiro;

                    if (cargaPedido.Carga.TipoOperacao?.TomadorCTeSubcontratacaoDeveSerDoCTeOriginal ?? false)
                    {
                        if (!cteParaSubContratacao.Tomador.CPF_CNPJ.StartsWith(cteParaSubContratacao.Emitente.CPF_CNPJ.Substring(0, 8)))
                            return $"O tomador do CT-e ({cteParaSubContratacao.Tomador.CPF_CNPJ_Formatado}) não pertence à mesma raiz de CNPJ do emitente do CT-e ({cteParaSubContratacao.Emitente.CPF_CNPJ_Formatado}). É necessário importar as notas fiscais para emitir um CT-e normal.";

                        cargaPedido.Tomador = cteParaSubContratacao.Tomador.Cliente;
                    }

                    if (configuracao.UtilizaEmissaoMultimodal && cargaPedido?.Pedido?.Tomador != null)
                        cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                    else if (cargaPedido.Carga.CargaDestinadaCTeComplementar && cargaPedido?.Pedido?.Tomador != null)
                        cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                }

                cargaPedido.Pedido.UsarTipoPagamentoNF = false;
                cargaPedido.Pedido.PedidoSubContratado = true;

                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (cargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscal != null)
                        cargaPedido.ModeloDocumentoFiscal = cargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscal;

                    if (cargaPedido.Carga.TipoOperacao.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cargaPedido.Carga.TipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal && cargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal != null)
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = cargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal;
                }
                else if (cteParaSubContratacao.TransportadorTerceiro.NaoUsarConfiguracaoEmissaoGrupo || cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas == null)
                {
                    if (cteParaSubContratacao.TransportadorTerceiro.ModeloDocumentoFiscal != null)
                        cargaPedido.ModeloDocumentoFiscal = cteParaSubContratacao.TransportadorTerceiro.ModeloDocumentoFiscal;

                    if (cteParaSubContratacao.TransportadorTerceiro.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cteParaSubContratacao.TransportadorTerceiro.UtilizarOutroModeloDocumentoEmissaoMunicipal && cteParaSubContratacao.TransportadorTerceiro.ModeloDocumentoFiscalEmissaoMunicipal != null)
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = cteParaSubContratacao.TransportadorTerceiro.ModeloDocumentoFiscalEmissaoMunicipal;
                }
                else
                {
                    if (cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas != null)
                    {
                        if (cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.ModeloDocumentoFiscal != null)
                            cargaPedido.ModeloDocumentoFiscal = cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.ModeloDocumentoFiscal;

                        if (cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal && cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal != null)
                            cargaPedido.ModeloDocumentoFiscalIntramunicipal = cteParaSubContratacao.TransportadorTerceiro.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal;
                    }
                }
            }

            cargaPedido.Pedido.SubContratante = cteParaSubContratacao.TransportadorTerceiro;

            unitOfWork.Flush();

            serCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, unitOfWork);

            repCargaPedido.Atualizar(cargaPedido);

            if (cargaPedido.Carga.EmpresaFilialEmissora != null)
            {
                if (!repCargaPedido.VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCarga(cargaPedido.Carga.Codigo))
                {
                    cargaPedido.Carga.EmpresaFilialEmissora = null;
                    cargaPedido.Carga.AgValorRedespacho = false;
                    repCarga.Atualizar(cargaPedido.Carga);
                }
            }

            bool naoValidarNotaFiscalExistente = false;
            bool naoValidarNotasFiscaisComDiferentesPortos = false;
            int codigoPortoOrigem = cargaPedido.Pedido?.Porto?.Codigo ?? 0;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Carga.TipoOperacao.NaoValidarNotasFiscaisComDiferentesPortos;
            else if (cargaPedido.Pedido.Remetente != null && (cargaPedido.Pedido.Remetente.NaoUsarConfiguracaoEmissaoGrupo || cargaPedido.Pedido.Remetente.GrupoPessoas == null))
                naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Pedido.Remetente.NaoValidarNotasFiscaisComDiferentesPortos;
            else if (cargaPedido.Pedido.Remetente != null)
                naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Pedido.Remetente.GrupoPessoas.NaoValidarNotasFiscaisComDiferentesPortos;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                naoValidarNotaFiscalExistente = cargaPedido.Carga.TipoOperacao.NaoValidarNotaFiscalExistente;
            else if (cargaPedido.Pedido.Remetente != null && (cargaPedido.Pedido.Remetente.NaoUsarConfiguracaoEmissaoGrupo || cargaPedido.Pedido.Remetente.GrupoPessoas == null))
                naoValidarNotaFiscalExistente = cargaPedido.Pedido.Remetente.NaoValidarNotaFiscalExistente;
            else if (cargaPedido.Pedido.Remetente != null)
                naoValidarNotaFiscalExistente = cargaPedido.Pedido.Remetente.GrupoPessoas.NaoValidarNotaFiscalExistente;

            if (!naoValidarNotaFiscalExistente && configuracao.PermitirAutalizarNotaFiscalCarga)
                naoValidarNotaFiscalExistente = true;

            if (configuracao.UtilizaEmissaoMultimodal && naoValidarNotasFiscaisComDiferentesPortos && codigoPortoOrigem > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoExistente = repPedidoCTeParaSubContratacao.BuscarPorCTeSubContratacaoAtivo(cteParaSubContratacao.Codigo, codigoPortoOrigem);

                if (pedidoCTeParaSubContratacaoExistente != null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    mensagem += $"Já existe um CT-e para subcontratação com esta chave ({cteParaSubContratacao.ChaveAcesso}) informado em outro pedido na carga ({pedidoCTeParaSubContratacaoExistente.CargaPedido?.Carga.CodigoCargaEmbarcador}) com o mesmo Porto de Origem ({cargaPedido.Pedido?.Porto?.Descricao}).";
            }
            else if (!naoValidarNotaFiscalExistente && !naoValidarNotasFiscaisComDiferentesPortos && !cargaPedido.PedidoEncaixado)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoExistente = repPedidoCTeParaSubContratacao.BuscarPorCTeSubContratacaoAtivo(cteParaSubContratacao.Codigo);

                if (pedidoCTeParaSubContratacaoExistente != null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    mensagem += $"Já existe um CT-e para subcontratação com esta chave ({cteParaSubContratacao.ChaveAcesso}) informado em outro pedido na carga ({pedidoCTeParaSubContratacaoExistente.CargaPedido?.Carga.CodigoCargaEmbarcador}).";
            }

            //Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cteParaSubContratacao.Emitente.CPF_CNPJ);

            //#if DEBUG
            //            mensagem = "";
            //            empresa = null;
            //#endif

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracao.UtilizaEmissaoMultimodal)
            {
                //if (empresa != null)
                //{
                //    mensagem += "Não é permitido subContratar um CT-e emitido por " + empresa.RazaoSocial + " (" + empresa.CNPJ_Formatado + " ), pois a mesma pertence ao seu grupo de empresas.";
                //}

                if (cargaPedido.Pedido.SubContratante != null && cargaPedido.Pedido.SubContratante.CPF_CNPJ != cteParaSubContratacao.TransportadorTerceiro.CPF_CNPJ)
                {
                    mensagem += "Não é permitido adicionar um CT-e de " + cargaPedido.Pedido.SubContratante.Nome + "(" + cargaPedido.Pedido.SubContratante.CPF_CNPJ_Formatado + " ), pois a carga foi subContratada por " + cteParaSubContratacao.TransportadorTerceiro.Nome + " (" + cteParaSubContratacao.TransportadorTerceiro.CPF_CNPJ + ").";
                }
            }

            Dominio.Entidades.ParticipanteCTe remetenteCTe = cteParaSubContratacao.Expedidor != null && cteParaSubContratacao.Expedidor.Localidade.Codigo == cargaPedido.Pedido?.Remetente?.Localidade.Codigo ? cteParaSubContratacao.Expedidor : cteParaSubContratacao.Remetente != null ? cteParaSubContratacao.Remetente : cteParaSubContratacao.Expedidor;

            if (cargaPedido.Pedido.Destinatario == null)
            {
                Dominio.Entidades.ParticipanteCTe destinatarioCTe = cteParaSubContratacao.Recebedor != null /*&& cteParaSubContratacao.Recebedor.Localidade.Codigo == cargaPedido.Pedido.Destinatario.Localidade.Codigo*/ ? cteParaSubContratacao.Recebedor : cteParaSubContratacao.Destinatario;
                cargaPedido.Pedido.Destinatario = destinatarioCTe.Cliente != null ? repCliente.BuscarPorCPFCNPJ(destinatarioCTe.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(destinatarioCTe.CPF_CNPJ_SemFormato));
                if (cargaPedido.Pedido.Recebedor != null)
                    cargaPedido.Destino = cargaPedido.Pedido.Recebedor.Localidade;
                else
                    cargaPedido.Destino = destinatarioCTe.Localidade;
                repPedido.Atualizar(cargaPedido.Pedido);
            }

            if (cargaPedido.Pedido.Remetente == null)
            {
                //if (cargaPedido.Pedido.Remetente.Localidade.Codigo == remetenteCTe.Localidade.Codigo)
                //{
                cargaPedido.Pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                cargaPedido.Pedido.Remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(remetenteCTe.CPF_CNPJ_SemFormato));
                //}
                //else
                //{
                //    mensagem = "A origem do CT-e (" + remetenteCTe.Localidade.DescricaoCidadeEstado + ") é diferente da origem informada para a carga (" + cargaPedido.Origem.DescricaoCidadeEstado + ")";
                //}
            }
            else
            {
                if (cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado != remetenteCTe.CPF_CNPJ_Formatado)
                {
                    if (cargaPedido.Pedido.Remetente.Localidade.Codigo == remetenteCTe.Localidade.Codigo)
                    {
                        if (!cargaPedido.PedidoEncaixado)
                        {
                            cargaPedido.Pedido.Remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(remetenteCTe.CPF_CNPJ_SemFormato));
                            repPedido.Atualizar(cargaPedido.Pedido);
                        }
                    }
                    else if (!configuracao.NaoValidarTomadorCTeSubcontratacaoComTomadorPedido)
                    {
                        Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();//se o tomador é o mesmo permite adicionar mais ctes
                        Dominio.Entidades.Cliente emitenteCTe = repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubContratacao.Emitente.CPF_CNPJ_SemFormato));
                        if ((emitenteCTe.GrupoPessoas == null || tomador.GrupoPessoas == null || tomador.GrupoPessoas.Codigo != emitenteCTe.GrupoPessoas.Codigo) && !(cargaPedido.Carga.TipoOperacao?.PermitirMultiplosRemetentesPedido ?? false))
                        {
                            //#if !DEBUG
                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)//todo, ver melhor forma de garantir isso
                                mensagem = "O remetente da Nota/CT-e não é o mesmo do pedido.";
                            //#endif
                        }

                    }
                }
            }

            return mensagem;
        }

        private void SetarNumeroPedidoObservacaoCTeSubcontratacao(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cteTerceiro?.ObservacaoGeral))
                return;

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = tomador?.GrupoPessoas;
            if (grupoPessoa == null || !grupoPessoa.LerNumeroPedidoObservacaoCTeSubcontratacao || string.IsNullOrWhiteSpace(grupoPessoa.RegexNumeroPedidoObservacaoCTeSubcontratacao))
                return;

            Regex regex = new Regex(grupoPessoa.RegexNumeroPedidoObservacaoCTeSubcontratacao, RegexOptions.IgnoreCase);
            Match match = regex.Match(cteTerceiro.ObservacaoGeral);

            if (!match.Success)
                return;

            cteTerceiro.NumeroPedido = Utilidades.String.Left(match.Value.Trim(), 150);
        }

        private void SetarNumeroPedidoObservacaoCTeTerceiro(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cteTerceiro?.ObservacaoGeral))
                return;

            if (tomador == null || tomador.GrupoPessoas == null || !tomador.GrupoPessoas.LerNumeroCargaObservacaoCTeSubcontratacao || string.IsNullOrWhiteSpace(tomador.GrupoPessoas.RegexNumeroCargaObservacaoCTeSubcontratacao))
                return;

            Regex regex = new Regex(tomador.GrupoPessoas.RegexNumeroCargaObservacaoCTeSubcontratacao, RegexOptions.IgnoreCase);

            Match match = regex.Match(cteTerceiro.ObservacaoGeral);

            if (match.Success)
                cteTerceiro.NumeroCarga = Utilidades.String.Left(match.Value.Trim(), 150);
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao InserirCTeSubContratacaoCargaPedido(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = null, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            if (configuracao == null)
                configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao;

            if (pedidoCTesParaSubContratacao != null)
                pedidoCTeParaSubContratacao = pedidoCTesParaSubContratacao.Where(o => o.CTeTerceiro.Codigo == cteParaSubContratacao.Codigo).FirstOrDefault();
            else
                pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCTeSubContratacaoECargaPedido(cteParaSubContratacao.Codigo, cargaPedido.Codigo);

            if (pedidoCTeParaSubContratacao == null)
                pedidoCTeParaSubContratacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao();
            else if (!configuracao.PermitirAutalizarNotaFiscalCarga)
                return pedidoCTeParaSubContratacao;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
            if ((carga.TipoOperacao?.EmiteCTeFilialEmissora ?? false) && carga.UtilizarCTesAnterioresComoCTeFilialEmissora)
            {
                if (!EmissorCTeAnteriorPertenceFilialEmissora(cteParaSubContratacao, carga.Filial.EmpresaEmissora))
                {
                    Servicos.Log.TratarErro($"CT-e Anterior não vinculado na carga {carga.Codigo} pois raiz CNPJ emissor diferente da raiz CNPJ Filial Emissora", "CTeSubContratacao");
                    return pedidoCTeParaSubContratacao;
                }
            }

            pedidoCTeParaSubContratacao.CTeTerceiro = cteParaSubContratacao;
            pedidoCTeParaSubContratacao.CargaPedido = cargaPedido;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && (!cargaPedido.CargaPedidoFilialEmissora && cargaPedido.CargaPedidoTrechoAnterior == null) && configuracao.UsarValorCTeAnteriorSubcontratacao)//todo:rever isso é uma regra fixa para a Danone
            {
                decimal aliquota = (cargaPedido.PercentualAliquota / 100);

                decimal baseCalculo = cteParaSubContratacao.ValorAReceber;
                if (cargaPedido.CST == "60")
                {
                    baseCalculo = Math.Round((baseCalculo / (1 - aliquota)), 2, MidpointRounding.AwayFromZero);
                    pedidoCTeParaSubContratacao.ValorFrete = cteParaSubContratacao.ValorAReceber;
                }
                else
                    pedidoCTeParaSubContratacao.ValorFrete = cteParaSubContratacao.ValorAReceber - cteParaSubContratacao.ValorICMS;

                pedidoCTeParaSubContratacao.ValorICMS = Math.Round(baseCalculo * aliquota, 2, MidpointRounding.AwayFromZero);
                pedidoCTeParaSubContratacao.BaseCalculoICMS = baseCalculo;
                pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                pedidoCTeParaSubContratacao.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOP;
                pedidoCTeParaSubContratacao.CST = cargaPedido.CST;
                pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                pedidoCTeParaSubContratacao.PercentualAliquota = cargaPedido.PercentualAliquota;
                pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;

                pedidoCTeParaSubContratacao.PossuiCTe = cargaPedido.PossuiCTe;
                pedidoCTeParaSubContratacao.PossuiNFS = cargaPedido.PossuiNFS;
                pedidoCTeParaSubContratacao.PossuiNFSManual = cargaPedido.PossuiNFSManual;

                pedidoCTeParaSubContratacao.BaseCalculoISS = cargaPedido.BaseCalculoISS;
                pedidoCTeParaSubContratacao.ValorISS = cargaPedido.ValorISS;
                pedidoCTeParaSubContratacao.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;
                pedidoCTeParaSubContratacao.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
                pedidoCTeParaSubContratacao.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
                pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, baseCalculo);
                servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBS(pedidoCTeParaSubContratacao, impostoIBSCBS);
            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && (cargaPedido.CargaPedidoFilialEmissora || cargaPedido.CargaPedidoTrechoAnterior != null || !configuracao.UsarValorCTeAnteriorSubcontratacao))
            {
                if (cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada || (configuracao.UsarImpostosIntegracaoSubcontratacao && !string.IsNullOrWhiteSpace(cargaPedido.CST)))
                {
                    pedidoCTeParaSubContratacao.ValorFrete = cargaPedido.ValorFrete;
                    pedidoCTeParaSubContratacao.ValorICMS = cargaPedido.ValorICMS;
                    pedidoCTeParaSubContratacao.PercentualAliquota = cargaPedido.PercentualAliquota;
                    pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
                    pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOP;
                    pedidoCTeParaSubContratacao.CST = cargaPedido.CST;

                    pedidoCTeParaSubContratacao.BaseCalculoICMS = cargaPedido.BaseCalculoICMS;
                    pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                    pedidoCTeParaSubContratacao.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                    pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;

                    pedidoCTeParaSubContratacao.PossuiCTe = cargaPedido.PossuiCTe;
                    pedidoCTeParaSubContratacao.PossuiNFS = cargaPedido.PossuiNFS;
                    pedidoCTeParaSubContratacao.PossuiNFSManual = cargaPedido.PossuiNFSManual;

                    pedidoCTeParaSubContratacao.BaseCalculoISS = cargaPedido.BaseCalculoISS;
                    pedidoCTeParaSubContratacao.ValorISS = cargaPedido.ValorISS;
                    pedidoCTeParaSubContratacao.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;
                    pedidoCTeParaSubContratacao.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
                    pedidoCTeParaSubContratacao.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
                    pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;

                    servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBSComTributacaoDefinidaEValores(pedidoCTeParaSubContratacao, cargaPedido);
                }
                else
                {
                    pedidoCTeParaSubContratacao.ValorFrete = cargaPedido.ValorFrete;
                    pedidoCTeParaSubContratacao.ValorICMS = 0;
                    pedidoCTeParaSubContratacao.CST = "40";
                    pedidoCTeParaSubContratacao.PercentualAliquota = 0;//cteParaSubContratacao.AliquotaICMS;
                    pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = 0;
                    pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOP;

                    servicoPedidoCTeParaSubContratacao.ZerarCamposImpostoIBSCBS(pedidoCTeParaSubContratacao);
                }
            }

            if (objetoValorPersistente == null)
            {
                if (pedidoCTeParaSubContratacao.Codigo > 0)
                    repPedidoCTeParaSubContratacao.Atualizar(pedidoCTeParaSubContratacao);
                else
                    repPedidoCTeParaSubContratacao.Inserir(pedidoCTeParaSubContratacao);
            }
            else
            {
                if (pedidoCTeParaSubContratacao.Codigo > 0)
                    objetoValorPersistente.Atualizar(pedidoCTeParaSubContratacao);
                else
                    objetoValorPersistente.Inserir(pedidoCTeParaSubContratacao);
            }

            //Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoCTeParaSubContratacao, false, tipoServicoMultisoftware, unitOfWork);
            return pedidoCTeParaSubContratacao;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao InserirCTeSubContratacaoFilialEmissora(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao();

            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTePorNota serCTeNotaFiscal = new CTePorNota(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            pedidoCTeParaSubContratacao.CTeTerceiro = cteParaSubContratacao;
            pedidoCTeParaSubContratacao.CargaPedido = cargaPedido;

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes = null;
            if (cargaPedido.Carga.CargaSVM || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual || cargaCTe == null)
            {
                pedidoCTeParaSubContratacao.ValorFrete = cargaPedido.ValorFrete;
                pedidoCTeParaSubContratacao.ValorICMS = cargaPedido.ValorICMS;
                pedidoCTeParaSubContratacao.PercentualAliquota = cargaPedido.PercentualAliquota;
                pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;

                if (!cargaPedido.Carga.EmitirCTeComplementar)
                {
                    pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOP;
                    pedidoCTeParaSubContratacao.CST = cargaPedido.CST;
                }
                else
                {
                    pedidoCTeParaSubContratacao.CFOP = cteParaSubContratacao.CFOP;
                    pedidoCTeParaSubContratacao.CST = cteParaSubContratacao.CST;
                }

                pedidoCTeParaSubContratacao.BaseCalculoICMS = cargaPedido.BaseCalculoICMS;
                pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                pedidoCTeParaSubContratacao.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;

                pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe;
                if (cargaPedido.RegraICMS != null)
                    pedidoCTeParaSubContratacao.RegraICMS = cargaPedido.RegraICMS;

                pedidoCTeParaSubContratacao.PossuiCTe = cargaPedido.PossuiCTe;
                pedidoCTeParaSubContratacao.PossuiNFS = cargaPedido.PossuiNFS;
                pedidoCTeParaSubContratacao.PossuiNFSManual = cargaPedido.PossuiNFSManual;

                pedidoCTeParaSubContratacao.BaseCalculoISS = cargaPedido.BaseCalculoISS;
                pedidoCTeParaSubContratacao.ValorISS = cargaPedido.ValorISS;
                pedidoCTeParaSubContratacao.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;
                pedidoCTeParaSubContratacao.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
                pedidoCTeParaSubContratacao.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
                pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;

                servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBSComTributacaoDefinidaEValores(pedidoCTeParaSubContratacao, cargaPedido);

                if (!cargaPedido.Carga.CargaSVM)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponenteFrete.BuscarPorCargaPedido(cargaPedido.Codigo, true, cargaPedido.ModeloDocumentoFiscal, false);

                    componentes = serCargaPedido.BuscarCargaPedidoComponentesFrete(cargaPedido, unitOfWork, false);
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                if (cargaPedido.CargaPedidoTrechoAnterior == null)
                    pedidosXMLNotaFiscal = cargaCTe.NotasFiscais.Select(obj => obj.PedidoXMLNotaFiscal).Distinct().ToList();
                else
                    pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                pedidoCTeParaSubContratacao.ValorFrete = pedidosXMLNotaFiscal.Sum(obj => obj.ValorFrete);
                pedidoCTeParaSubContratacao.ValorICMS = pedidosXMLNotaFiscal.Sum(obj => obj.ValorICMS);
                pedidoCTeParaSubContratacao.PercentualAliquota = pedidosXMLNotaFiscal.FirstOrDefault().PercentualAliquota;
                pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = pedidosXMLNotaFiscal.FirstOrDefault().PercentualAliquotaInternaDifal;
                pedidoCTeParaSubContratacao.CFOP = pedidosXMLNotaFiscal.FirstOrDefault().CFOP;
                pedidoCTeParaSubContratacao.CST = pedidosXMLNotaFiscal.FirstOrDefault().CST;
                pedidoCTeParaSubContratacao.BaseCalculoICMS = pedidosXMLNotaFiscal.Sum(obj => obj.BaseCalculoICMS);
                pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = pedidosXMLNotaFiscal.FirstOrDefault().PercentualIncluirBaseCalculo;
                pedidoCTeParaSubContratacao.PercentualReducaoBC = pedidosXMLNotaFiscal.FirstOrDefault().PercentualReducaoBC;
                pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = pedidosXMLNotaFiscal.FirstOrDefault().ModeloDocumentoFiscal;
                pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = pedidosXMLNotaFiscal.FirstOrDefault().IncluirICMSBaseCalculo;
                pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = pedidosXMLNotaFiscal.FirstOrDefault().ObservacaoRegraICMSCTe;
                if (pedidosXMLNotaFiscal.FirstOrDefault().RegraICMS != null)
                    pedidoCTeParaSubContratacao.RegraICMS = pedidosXMLNotaFiscal.FirstOrDefault().RegraICMS;

                pedidoCTeParaSubContratacao.PossuiCTe = pedidosXMLNotaFiscal.FirstOrDefault().PossuiCTe;
                pedidoCTeParaSubContratacao.PossuiNFS = pedidosXMLNotaFiscal.FirstOrDefault().PossuiNFS;
                pedidoCTeParaSubContratacao.PossuiNFSManual = pedidosXMLNotaFiscal.FirstOrDefault().PossuiNFSManual;

                pedidoCTeParaSubContratacao.BaseCalculoISS = pedidosXMLNotaFiscal.Sum(obj => obj.BaseCalculoISS);
                pedidoCTeParaSubContratacao.ValorISS = pedidosXMLNotaFiscal.Sum(obj => obj.ValorISS);
                pedidoCTeParaSubContratacao.ValorRetencaoISS = pedidosXMLNotaFiscal.Sum(obj => obj.ValorRetencaoISS);
                pedidoCTeParaSubContratacao.PercentualAliquotaISS = pedidosXMLNotaFiscal.FirstOrDefault().PercentualAliquotaISS;
                pedidoCTeParaSubContratacao.PercentualRetencaoISS = pedidosXMLNotaFiscal.FirstOrDefault().PercentualRetencaoISS;
                pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = pedidosXMLNotaFiscal.FirstOrDefault().IncluirISSBaseCalculo;

                servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBSComTributacaoDefinidaEValores(pedidoCTeParaSubContratacao, pedidosXMLNotaFiscal);

                componentes = serCTeNotaFiscal.BuscarXMLPedidoComponentesFreteAgrupados(pedidosXMLNotaFiscal, false, unitOfWork);
            }

            repPedidoCTeParaSubContratacao.Inserir(pedidoCTeParaSubContratacao);
            CriarComponentes(pedidoCTeParaSubContratacao, componentes, unitOfWork);

            return pedidoCTeParaSubContratacao;
        }

        private void CriarComponentes(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes, Repositorio.UnitOfWork unitOfWork)
        {
            if (componentes == null || componentes.Count == 0)
                return;
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDinamico in componentes)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete();
                pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete = componenteDinamico.ComponenteFrete;
                pedidoCteParaSubContratacaoComponenteFrete.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete = componenteDinamico.TipoComponenteFrete;
                pedidoCteParaSubContratacaoComponenteFrete.ValorComponente = componenteDinamico.ValorComponente;
                pedidoCteParaSubContratacaoComponenteFrete.Percentual = componenteDinamico.Percentual;
                pedidoCteParaSubContratacaoComponenteFrete.TipoValor = componenteDinamico.TipoValor;
                pedidoCteParaSubContratacaoComponenteFrete.DescontarValorTotalAReceber = componenteDinamico.DescontarValorTotalAReceber;
                pedidoCteParaSubContratacaoComponenteFrete.AcrescentaValorTotalAReceber = componenteDinamico.AcrescentaValorTotalAReceber;
                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalAReceber = componenteDinamico.NaoSomarValorTotalAReceber;
                pedidoCteParaSubContratacaoComponenteFrete.DescontarDoValorAReceberValorComponente = componenteDinamico.DescontarDoValorAReceberValorComponente;
                pedidoCteParaSubContratacaoComponenteFrete.DescontarDoValorAReceberOICMSDoComponente = componenteDinamico.DescontarDoValorAReceberOICMSDoComponente;
                pedidoCteParaSubContratacaoComponenteFrete.ValorICMSComponenteDestacado = componenteDinamico.ValorICMSComponenteDestacado;
                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalPrestacao = componenteDinamico.NaoSomarValorTotalPrestacao;
                pedidoCteParaSubContratacaoComponenteFrete.RateioFormula = componenteDinamico.RateioFormula;
                pedidoCteParaSubContratacaoComponenteFrete.IncluirBaseCalculoICMS = componenteDinamico.IncluirBaseCalculoImposto;
                pedidoCteParaSubContratacaoComponenteFrete.OutraDescricaoCTe = componenteDinamico.OutraDescricaoCTe;
                pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscal = componenteDinamico.ModeloDocumentoFiscal;
                pedidoCteParaSubContratacaoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = componenteDinamico.IncluirIntegralmenteContratoFreteTerceiro;

                //angelo pendencia pedidoCteParaSubContratacaoComponenteFrete.descon = componenteDinamico.DescontarValorTotalAReceber frete liquido;

                repPedidoCteParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);
            }
        }

        private static Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoEmitir(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao)
        {
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

            if (pedidoCTeParaSubContratacao.PossuiNFSManual)
                modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
            else
                modeloDocumentoEmitir = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal;

            return modeloDocumentoEmitir;
        }

        /// <summary>
        /// Armazena as unidades do CTe e retorna um peso aproximado dos CT-es
        /// </summary>
        private decimal ObterPesoTotalDaSubContratacao(List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            string descricaoItemPeso = ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new CargaPedido(unitOfWork);
            decimal pesoKg = 0;
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            int posicaoCTe = 0;
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCTe = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);

                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidade in quantidadesCTe)
                {
                    pesoKg += ObterPesoEmKG(quantidade, quantidadesCTe.Count, posicaoCTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
                    posicaoCTe++;

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = (from obj in cargaPedidoQuantidades where obj.Unidade == quantidade.Unidade select obj).FirstOrDefault();
                    if (cargaPedidoQuantidade == null)
                    {
                        cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
                        cargaPedidoQuantidade.CargaPedido = cargaPedido;
                        cargaPedidoQuantidade.Quantidade = quantidade.Quantidade;
                        cargaPedidoQuantidade.Unidade = quantidade.Unidade;
                        cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                    }
                    else
                        cargaPedidoQuantidade.Quantidade += quantidade.Quantidade;
                }
            }

            serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedidoQuantidades, cargaPedido, unitOfWork);

            return pesoKg;
        }

        private decimal ObterPesoEmKG(Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidade, int quantidadesCTe, int posicaoCTe, string descricaoItemPeso, bool utilizarPrimeiraUnidadeMedidaPeso)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade
            {
                Quantidade = quantidade.Quantidade,
                Unidade = quantidade.Unidade,
                TipoMedida = quantidade.TipoMedida
            };

            return ObterPesoEmKG(cteTerceiroQuantidade, quantidadesCTe, posicaoCTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
        }

        private decimal ObterPesoEmKG(Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade quantidade, int quantidadesCTe, int posicaoCTe, string descricaoItemPeso, bool utilizarPrimeiraUnidadeMedidaPeso)
        {
            if (!string.IsNullOrWhiteSpace(descricaoItemPeso) && quantidade.TipoMedida.ToLower() != descricaoItemPeso.ToLower())
                return 0m;

            if (utilizarPrimeiraUnidadeMedidaPeso && posicaoCTe > 0)
                return 0m;

            decimal pesoKg = 0m;

            if (quantidade.Unidade == Dominio.Enumeradores.UnidadeMedida.KG)
                pesoKg = quantidade.Quantidade;
            else if (quantidade.Unidade == Dominio.Enumeradores.UnidadeMedida.TON)
                pesoKg = quantidade.Quantidade * 1000;//não mudar, o cliente deve enviar a quantidade em toneladas também
            else if (quantidade.Unidade == Dominio.Enumeradores.UnidadeMedida.UN)
            {
                if (quantidadesCTe == 1)
                    pesoKg = quantidade.Quantidade;
            }
            else
                pesoKg += quantidade.Quantidade;

            return pesoKg;
        }

        private decimal ObterPesoDaSubContratacao(List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCTeTerceiro, string descricaoItemPeso, bool utilizarPrimeiraUnidadeMedidaPeso)
        {
            decimal pesoKg = 0m;

            int posicaoCTe = 0;
            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade quantidade in quantidadesCTeTerceiro)
            {
                pesoKg += ObterPesoEmKG(quantidade, quantidadesCTeTerceiro.Count, posicaoCTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
                posicaoCTe++;
            }

            return pesoKg;
        }

        private decimal ObterPesoDaSubContratacao(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, string descricaoItemPeso, bool utilizarPrimeiraUnidadeMedidaPeso, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> lstCTeTerceiroQuantidade = null)
        {
            decimal pesoKg = 0m;
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCTe = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(cteTerceiro.Codigo, lstCTeTerceiroQuantidade);

            int posicaoCTe = 0;
            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidade in quantidadesCTe)
            {
                pesoKg += ObterPesoEmKG(quantidade, quantidadesCTe.Count, posicaoCTe, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);
                posicaoCTe++;
            }

            return pesoKg;
        }

        private bool EmissorCTeAnteriorPertenceFilialEmissora(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Empresa empresaEmissora)
        {

            string RaizCNPJEmissorCTe = cteTerceiro.Emitente.CPF_CNPJ_SemFormato.Substring(0, 8);
            string RaizCNPJFilialEmissora = empresaEmissora.CNPJ_SemFormato.Substring(0, 8);

            if (RaizCNPJEmissorCTe != RaizCNPJFilialEmissora)
                return false;

            return true;
        }

        #endregion Métodos Privados
    }
}
