using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Documentos
{
    public sealed class ConsultaDocumento
    {
        #region Atributos Privados

        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public ConsultaDocumento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion

        #region Métodos Privados

        private void ConsultarPorChave(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chave, out string msgAlerta, bool atualizarDadosNotaFiscal, ClassificacaoNFe classificacaoNFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgAlerta = "";
            Documento servicoDocumento = new Documento();

            if (!servicoDocumento.ValidarChave(chave))
                throw new ServicoException("A chave informada não é válida");

            string tipoDocumento = chave.Substring(20, 2);
            string tipoDocumentoNFe = "55";
            string tipoDocumentoCTe = "57";

            if (tipoDocumento == tipoDocumentoNFe)
                ConsultarNotaFiscalPorChave(cargaPedido, chave, out msgAlerta, atualizarDadosNotaFiscal, classificacaoNFe, tipoServicoMultisoftware);
            else if (tipoDocumento == tipoDocumentoCTe)
                ConsultarCTePorChave(cargaPedido, chave, out msgAlerta); //throw new ServicoException("O tipo de documento CT-e não está disponível para consulta");
            else
                throw new ServicoException("A chave informada não é válida");
        }

        #endregion

        #region Métodos Privados Nota Fiscal

        private void ConsultarNotaFiscal(int codigoCargaPedido, int codigoXmlNotaFiscal, out string msgAlerta)
        {
            Pedido.NotaFiscal servicoCargaNotaFiscal = new Pedido.NotaFiscal(_unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoParcialxml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

                _unitOfWork.Start();
                msgAlerta = "";

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigoXmlNotaFiscal);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial existeNotaParcialPorChaveNota = repCargaPedidoParcialxml.BuscarPorChaveNota(xmlNotaFiscal.Chave);
                if (existeNotaParcialPorChaveNota != null && existeNotaParcialPorChaveNota.XMLNotaFiscal == null)
                {
                    existeNotaParcialPorChaveNota.XMLNotaFiscal = xmlNotaFiscal;
                    existeNotaParcialPorChaveNota.CargaPedido = cargaPedido;
                    repCargaPedidoParcialxml.Atualizar(existeNotaParcialPorChaveNota);
                }

                servicoCargaNotaFiscal.InformarDadosNotaCarga(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, out msgAlerta, _auditado);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, xmlNotaFiscal, "Adicionado manualmente por Chave/Número", _unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, null, "Adicionou Documento NF-e manualmente por Chave/Número.", _unitOfWork);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private void ConsultarCTePorChave(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chave, out string msgAlerta)
        {
            msgAlerta = "";
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(_unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeParaSubContratacao = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = repCTeParaSubContratacao.BuscarPorChave(chave, true);
            if (cteParaSubContratacao == null)
                cteParaSubContratacao = repCTeParaSubContratacao.BuscarPorChave(chave, false);

            if (cteParaSubContratacao != null)
            {
                //if (!cteParaSubContratacao.Ativo)
                //{
                //    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = cteParaSubContratacao.Clonar();
                //}
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                msgAlerta = serCTeSubContratacao.InformarDadosCTeNaCarga(_unitOfWork, null, cargaPedido, _tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao, true, cteParaSubContratacao);
            }
            else
                msgAlerta = "CT-e não localizado";
        }

        private void ConsultarNotaFiscalPorChave(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chave, out string msgAlerta, bool atualizarDadosNotaFiscal, ClassificacaoNFe classificacaoNFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

            msgAlerta = "";

             xmlNotaFiscal = ObterNotaFiscal(chave);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = ObterNotaFiscalPorDocumentoDestinado(chave, tipoServicoMultisoftware);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = ObterNotaFiscalPorSerpro(chave);
            else if (atualizarDadosNotaFiscal && xmlNotaFiscal != null && string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                xmlNotaFiscal = ObterNotaFiscalPorSerpro(chave);

            if (atualizarDadosNotaFiscal && xmlNotaFiscal == null)
                xmlNotaFiscal = ObterNotaFiscal(chave);

            if (xmlNotaFiscal == null)
                throw new ServicoException("Não foi possível encontrar nenhuma nota fiscal com a chave informada");

            xmlNotaFiscal.ClassificacaoNFe = classificacaoNFe;
            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

            ConsultarNotaFiscal(cargaPedido.Codigo, xmlNotaFiscal.Codigo, out msgAlerta);
        }

        private void ConsultarNotaFiscalPorNumero(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int numero, out string msgAlerta, ClassificacaoNFe classificacaoNFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            msgAlerta = "";
            double cpfCnpjEmitente = cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0d;
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroEEmitente(numero, cpfCnpjEmitente);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = ObterNotaFiscalPorDocumentoDestinado(numero, cargaPedido.Pedido.Remetente?.CPF_CNPJ_SemFormato, tipoServicoMultisoftware);

            if (xmlNotaFiscal == null)
                throw new ServicoException("Não foi possível encontrar nenhuma nota fiscal com o número informado");

            xmlNotaFiscal.ClassificacaoNFe = classificacaoNFe;

            xmlNotaFiscal.ClassificacaoNFe = classificacaoNFe;
            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

            ConsultarNotaFiscal(cargaPedido.Codigo, xmlNotaFiscal.Codigo, out msgAlerta);
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterNotaFiscal(string chave)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            return repositorioXmlNotaFiscal.BuscarPorChave(chave);
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterNotaFiscalPorSerpro(string chave)
        {
            string msgRetorno = "";
            string token = "";
            string arquivoNotaFiscal = "";
            if (!Servicos.Embarcador.Integracao.Serpro.IntegracaoSerpro.Realizarlogin(_unitOfWork, out msgRetorno, out token, false))
            {
                Servicos.Log.TratarErro(msgRetorno);
                return null;
            }
            else
            {
                arquivoNotaFiscal = Servicos.Embarcador.Integracao.Serpro.IntegracaoSerpro.BaixarJSONPelaChave(_unitOfWork, out msgRetorno, token, chave);
                if (string.IsNullOrWhiteSpace(arquivoNotaFiscal))
                {
                    Servicos.Log.TratarErro(msgRetorno);
                    return null;
                }
                else
                {
                    NFe.NFe servicoNFe = new NFe.NFe();
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = servicoNFe.BuscarDadosNotaFiscal(arquivoNotaFiscal, _unitOfWork);
                    if (xmlNotaFiscal != null && xmlNotaFiscal.Codigo == 0)
                    {
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                        repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);
                    }
                    return xmlNotaFiscal;
                }
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterNotaFiscalPorDocumentoDestinado(string chave, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa()
            {
                Cancelado = false,
                Chave = chave,
                DataEmissaoInicial = DateTime.Now.AddDays(-20),
                TiposDocumento = new List<TipoDocumentoDestinadoEmpresa>() { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }
            };

            return ObterNotaFiscalPorDocumentoDestinado(filtrosPesquisa, tipoServicoMultisoftware);
        }

        private Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterNotaFiscalPorDocumentoDestinado(int numero, string cpfCnpjEmitente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa()
            {
                Cancelado = false,
                CpfCnpjFornecedor = cpfCnpjEmitente,
                DataEmissaoInicial = DateTime.Now.AddDays(-20),
                NumeroDe = numero,
                TiposDocumento = new List<TipoDocumentoDestinadoEmpresa>() { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }
            };

            return ObterNotaFiscalPorDocumentoDestinado(filtrosPesquisa, tipoServicoMultisoftware);
        }

        private Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterNotaFiscalPorDocumentoDestinado(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                LimiteRegistros = 1
            };
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repositorioDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
           
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repositorioDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, parametrosConsulta).FirstOrDefault();

            if (documentoDestinado == null)
                return null;

            string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(diretorioDocumentosFiscais, "NFe", "nfeProc", $"{documentoDestinado.Chave}.xml");
            byte[] arquivo = null;

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
            else
            {
                DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _unitOfWork.StringConexao, null, out string mensagemErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                else
                    return null;
            }

            using (MemoryStream arquivoEmMemoria = new MemoryStream(arquivo))
            {
                StreamReader leitorXML = new StreamReader(arquivoEmMemoria);
                NFe.NFe servicoNFe = new NFe.NFe();
                string xmlString = leitorXML.ReadToEnd();
                servicoNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, leitorXML, _unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, configuracao?.UtilizarValorFreteNota ?? false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false);

                if (xmlNotaFiscal != null && xmlNotaFiscal.Codigo == 0)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

                    repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);
                    servicoNFe.SalvarProdutosNota(xmlString,xmlNotaFiscal,_auditado,_tipoServicoMultisoftware,_unitOfWork);
                }

                return xmlNotaFiscal;
            }
        }

        #endregion

        #region Métodos Públicos

        public void Consultar(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chaveOuNumero, out string msgAlerta, bool atualizarDadosNotaFiscal, ClassificacaoNFe classificacaoNFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgAlerta = "";
            chaveOuNumero = chaveOuNumero.ObterSomenteNumeros();

            if (string.IsNullOrWhiteSpace(chaveOuNumero))
                throw new ServicoException("A chave ou número do documento deve ser informada");

            chaveOuNumero = chaveOuNumero.Trim();

            if (chaveOuNumero.Length == 44)
                ConsultarPorChave(cargaPedido, chaveOuNumero, out msgAlerta, atualizarDadosNotaFiscal, classificacaoNFe, tipoServicoMultisoftware);
            else
            {
                int numero = chaveOuNumero.ToInt();

                if (numero <= 0)
                    throw new ServicoException("A chave ou número do documento são inválidos.");

                ConsultarNotaFiscalPorNumero(cargaPedido, numero, out msgAlerta, classificacaoNFe, tipoServicoMultisoftware);
            }
        }

        #endregion
    }
}
