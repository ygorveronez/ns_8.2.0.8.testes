using AdminMultisoftware.Dominio.Enumeradores;
using com.maersk.billableitemspostrequest;
using Dominio.Entidades.Embarcador.Fatura;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.CTe;
using Repositorio;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.CTe
{
    public class CTe : ServicoBase
    {
        #region Propriedades Privadas
        
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;

        #endregion

        #region Construtores
                
        public CTe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CTe(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }
        
        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTesPorBoleto(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem, Dominio.Entidades.Empresa transportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            if (titulo.ConhecimentoDeTransporteEletronico != null)
                ctesIntegracao.Add(ConverterObjetoCTe(titulo.ConhecimentoDeTransporteEletronico, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));
            else if (titulo.FaturaDocumento != null && titulo.FaturaDocumento.Fatura != null)
            {
                List<Dominio.Entidades.Empresa> empresas = null;

                if (transportador != null)
                    empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(transportador);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarCTesGeradosPorFatura(titulo.FaturaDocumento.Fatura.Codigo, empresas?.Select(o => o.Codigo).ToList(), inicioRegistro, fimRegistro);
                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    ctesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));
            }

            return ctesIntegracao;
        }

        public int ContarCTesPorBoleto(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.Entidades.Empresa transportador, Repositorio.UnitOfWork unitOfWork)
        {
            if (titulo.ConhecimentoDeTransporteEletronico != null)
                return 1;
            else if (titulo.FaturaDocumento != null && titulo.FaturaDocumento.Fatura != null)
            {
                List<Dominio.Entidades.Empresa> empresas = null;

                if (transportador != null)
                    empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(transportador);

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                return repCargaCTe.ContarCTesGeradosPorFatura(titulo.FaturaDocumento.Fatura.Codigo, empresas?.Select(o => o.Codigo).ToList());
            }
            else
                return 0;
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem, Dominio.Entidades.Empresa transportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            List<Dominio.Entidades.Empresa> empresas = null;

            if (transportador != null)
                empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(transportador);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarCTesGeradosPorCarga(carga.Codigo, empresas?.Select(o => o.Codigo).ToList(), inicioRegistro, fimRegistro, tipoServicoMultisoftware);

            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = cargaCTes.Count > 0 ? repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList()) : new List<Dominio.Entidades.CTeContaContabilContabilizacao>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                ctesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

            return ctesIntegracao;
        }

        public int ContarCTesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa transportador, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = null;

            if (transportador != null)
                empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(transportador);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            return repCargaCTe.ContarCTesGeradosPorCarga(carga.Codigo, empresas?.Select(o => o.Codigo).ToList());
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTesPorOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem, Dominio.Entidades.Empresa transportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCteComplementoinfo = repCTeComplementoInfo.BuscarCTesPorOcorrencia(cargaOcorrencia.Codigo, "", "", inicioRegistro, fimRegistro);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplemento in cargaCteComplementoinfo)
                ctesIntegracao.Add(ConverterObjetoCargaCTeComplementoInfo(cargaCTeComplemento, tipoDocumentoRetorno, true, unitOfWork));

            return ctesIntegracao;
        }

        public int ContarCTesPorOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplemento = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            return repCargaCTeComplemento.ContarPorCTEsOcorrencia(cargaOcorrencia.Codigo);
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao> BuscarAverbacaoCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, double remetente, double destinatario, int inicioRegistro, int fimRegistro, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao> CTesIntegracaoAverbacao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            foreach (var cargaPed in cargaPedido)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarAverbacaoCTesPorCargaPedido(cargaPed.Codigo, cargaPed.Carga.Codigo, remetente, destinatario, true, inicioRegistro, fimRegistro);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                {
                    CTesIntegracaoAverbacao.Add(ConverterObjetoCargaCTeAverbacao(cargaCTe, tipoDocumentoRetorno, unitOfWork));
                }
            }

            return CTesIntegracaoAverbacao;
        }

        public int ContarAverbacaoCTes(int protocoloCarga, int protocoloPedido, double remetente, double destinatario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, CodigoPedido);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocoloCarga, protocoloPedido);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

            return repCargaPedidoXMLNotaFiscalCTe.ContarAverbacaoCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.Carga.Codigo, remetente, destinatario, true);
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> BuscarFaturaCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, double remetente, double destinatario, int inicioRegistro, int fimRegistro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>();
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            foreach (var cargaPed in cargaPedido)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPed.Codigo, cargaPed.Carga.Codigo, remetente, destinatario, true, false, inicioRegistro, fimRegistro);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                {
                    CTesIntegracao.Add(ConverterObjetoCargaCTeFatura(cargaCTe, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, null));
                }
            }

            if (CTesIntegracao.Count > 0)
                return CTesIntegracao.OrderBy(o => o.DataEmissaoFatura).ToList();
            else
                return CTesIntegracao;
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTes(bool consultaMultimodal, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, double remetente, double destinatario, int inicioRegistro, int fimRegistro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            bool somenteCTes = !configuracaoGeral.HabilitarFuncionalidadesProjetoGollum && !configuracaoTMS.UtilizaEmissaoMultimodal;

            foreach (var cargaPedido in listaCargaPedido)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

                if (consultaMultimodal)
                    cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedidoMultiModal(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo, remetente, destinatario, true, somenteCTes, inicioRegistro, fimRegistro);
                else
                    cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo, remetente, destinatario, true, somenteCTes, inicioRegistro, fimRegistro);

                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                {
                    CTesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));
                }
            }

            return CTesIntegracao;
        }

        public int ContarCTes(bool consultaMultimodal, int protocoloCarga, int protocoloPedido, double remetente, double destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocoloCarga, protocoloPedido);

            bool somenteCTes = !configuracaoGeral.HabilitarFuncionalidadesProjetoGollum && !configuracaoTMS.UtilizaEmissaoMultimodal;

            if (consultaMultimodal)
                return cargaPedido == null ? 0 : repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorCargaPedidoMultiModal(new List<int> { cargaPedido.Codigo }, new List<int> { cargaPedido.CargaOrigem.Codigo }, remetente, destinatario, true, somenteCTes);
            else
                return cargaPedido == null ? 0 : repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo, remetente, destinatario, true, true);
        }

        public int ContarCTes(bool consultaMultimodal, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido, double remetente, double destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            List<int> cargaPedidoCodigos = listaCargaPedido.Select(cargaPedido => cargaPedido.Codigo).ToList();
            List<int> cargaPedidoOrigemCodigos = listaCargaPedido.Select(cargaPedido => cargaPedido.CargaOrigem.Codigo).ToList();

            bool somenteCTes = !configuracaoGeral.HabilitarFuncionalidadesProjetoGollum && !configuracaoTMS.UtilizaEmissaoMultimodal;

            if (consultaMultimodal)
                return repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorCargaPedidoMultiModal(cargaPedidoCodigos, cargaPedidoOrigemCodigos, remetente, destinatario, true, somenteCTes);
            else
                return repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorCargaPedido(cargaPedidoCodigos, cargaPedidoOrigemCodigos, remetente, destinatario, true, true);
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTesPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, string codigoTipoOperacao, string situacao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS, bool considerarHora = false)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, inicioRegistro, fimRegistro, configuracaoWS, considerarHora);
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                CTesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

            return CTesIntegracao;
        }

        public int ContarCTesPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS, bool considerarHora = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS, considerarHora);
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTesAlteradosPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, string codigoTipoOperacao, string situacao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesAlteradosPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, inicioRegistro, fimRegistro, configuracaoWS);
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                CTesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

            return CTesIntegracao;
        }

        public int ContarCTesAlteradosPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarCTesAlteradosPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS);
        }
        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarOutrosDocsPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarOutrosDocsPorPeriodoEmpresa(dataInicial, dataFinal, empresa, inicioRegistro, fimRegistro);

            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                CTesIntegracao.Add(ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

            return CTesIntegracao;
        }

        public int ContarOutrosDocsPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarOutrosDocsPorPeriodoEmpresa(dataInicial, dataFinal, empresa);
        }

        public Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao ConverterObjetoCargaCTeAverbacao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);

            Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao cte = new Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao();
            cte.Chave = cargaCTe.CTe.Chave;
            cte.Numero = cargaCTe.CTe.Numero;
            cte.NumeroControle = cargaCTe.CTe.NumeroControle;
            cte.Protocolo = cargaCTe.CTe.Codigo;
            foreach (var averbacao in averbacoes)
            {
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = null;
                if (averbacao.ArquivosTransacao != null && averbacao.ArquivosTransacao.Count == 1)
                    arquivoIntegracao = averbacao.ArquivosTransacao.Select(o => o.ArquivoResposta).FirstOrDefault();
                else if (averbacao.ArquivosTransacao != null && averbacao.ArquivosTransacao.Count > 1)
                    arquivoIntegracao = averbacao.ArquivosTransacao.Select(o => o.ArquivoResposta).LastOrDefault();

                if (arquivoIntegracao != null)
                {
                    if (averbacao.Forma == Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                        cte.XMLAverbacaoProvisoria = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(arquivoIntegracao);
                    else
                        cte.XMLAverbacaoDefinitiva = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(arquivoIntegracao);
                }
            }
            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.CTe.CTeFatura ConverterObjetoCargaCTeFatura(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8, Dominio.Entidades.Embarcador.Fatura.Fatura faturaIntegracao)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            Dominio.ObjetosDeValor.WebService.CTe.CTeFatura cte = new Dominio.ObjetosDeValor.WebService.CTe.CTeFatura();

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;

            if (cargaCTe.CTe.TomadorPagador != null && cargaCTe.CTe.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && cargaCTe.CTe.TomadorPagador != null && cargaCTe.CTe.TomadorPagador.Cliente != null && cargaCTe.CTe.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, cargaCTe.CTe.TomadorPagador.GrupoPessoas.Codigo);

            cte.GerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;
            if (acordoFaturamento != null && cargaCTe.Carga.TipoOperacao != null && cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
            {
                if (cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                    cte.GerarFaturamentoAVista = acordoFaturamento.LongoCursoGerarFaturamentoAVista;
                if (cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                    cte.GerarFaturamentoAVista = acordoFaturamento.CustoExtraGerarFaturamentoAVista;
            }

            cte.Chave = cargaCTe.CTe.Chave;
            cte.DataEmissao = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Numero = cargaCTe.CTe.Numero;
            cte.NumeroControle = cargaCTe.CTe.NumeroControle;
            cte.PDFCTe = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(cargaCTe.CTe, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = cargaCTe.CTe.Codigo;
            cte.CNPJEmissor = cargaCTe.CTe.Empresa.CNPJ_SemFormato;
            cte.NumeroBooking = cargaCTe.CTe.NumeroBooking;
            cte.RolagemCarga = faturaIntegracao.RolagemCarga;

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cargaCTe.CTe.Titulo;
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao != null ? repFatura.BuscarPorCodigo(faturaIntegracao.Codigo) : null;

            if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                cte.NumeroBoleto = titulo.NossoNumero;
            else if (cargaCTe.CTe.Fatura != null && cargaCTe.CTe.Fatura.Parcelas != null && cargaCTe.CTe.Fatura.Parcelas.Count > 0 && cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo > 0)
            {
                fatura = cargaCTe.CTe.Fatura;
                titulo = repTitulo.BuscarPorCodigo(cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }
            if (string.IsNullOrWhiteSpace(cte.NumeroBoleto))
            {
                titulo = repTitulo.BuscarTituloDocumentoPorCTe(cargaCTe.CTe.Codigo, false, faturaIntegracao.Codigo);
                if (titulo == null)
                    titulo = repTitulo.BuscarPorCTe(cargaCTe.CTe.Codigo, false);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }
            if (fatura == null)
                fatura = repFatura.BuscarPorCTe(cargaCTe.CTe.Codigo);
            if (fatura == null)
                fatura = repFatura.BuscarCanceladoPorCTe(cargaCTe.CTe.Codigo);

            cte.ProtocoloCTe = cargaCTe.CTe.Codigo;
            cte.ChaveCTe = cargaCTe.CTe.Chave;
            cte.DataEmissao = cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
            cte.DataEvento = cargaCTe.CTe.DataRetornoSefaz.HasValue ? cargaCTe.CTe.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            cte.MensagemSefaz = cargaCTe.CTe.Status != "A" ? cargaCTe.CTe.MensagemStatus?.MensagemDoErro ?? cargaCTe.CTe.MensagemRetornoSefaz : string.Empty;
            cte.SituacaoCTe = cargaCTe.CTe.DescricaoStatus;
            cte.ValorCTe = cargaCTe.CTe.ValorAReceber;
            cte.NumeroCarga = cargaCTe?.Carga.CodigoCargaEmbarcador ?? string.Empty;

            cte.NumeroFaturaIndividual = titulo?.Codigo ?? 0;
            cte.NumeroFaturaAgrupado = fatura?.Numero ?? 0;
            cte.SituacaoFatura = fatura != null ? fatura.Situacao : titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento;

            cte.PDFFatura = this.ObterFaturaPDF(fatura, codificarUTF8, unitOfWork);
            cte.PDFBoleto = this.ObterBoletoPDF(titulo, codificarUTF8, unitOfWork);
            cte.DataVencimento = titulo != null ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : cargaCTe.CTe.DataPreviaVencimento.HasValue ? cargaCTe.CTe.DataPreviaVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.DataEmissaoFatura = fatura != null ? fatura.DataFatura : titulo != null ? titulo.DataEmissao : null;

            cte.DataEmissaoFormatado = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("yyyy/MM/dd HH:mm") : "";
            cte.DataVencimentoFormatado = titulo != null ? titulo.DataVencimento.Value.ToString("yyyy/MM/dd HH:mm") : "";
            cte.DataEmissaoFaturaFormatado = fatura != null ? fatura.DataFatura.ToString("yyyy/MM/dd HH:mm") : titulo != null ? titulo.DataEmissao.Value.ToString("yyyy/MM/dd HH:mm") : "";

            cte.ValorIndividual = cargaCTe.CTe.ValorAReceber;
            cte.ValorAgrupado = fatura?.Total ?? 0;
            if (fatura != null)
                cte.Desconto = repFaturaDocumentoAcrescimoDesconto.BuscarDescontoPorFaturaDocumento(fatura.Codigo, cargaCTe.CTe.Codigo);
            else
                cte.Desconto = 0;
            if (cte.ValorAgrupado > 0)
                cte.ValorTotal = cte.ValorAgrupado - cte.Desconto;
            else
                cte.ValorTotal = cte.ValorIndividual;
            cte.Favorecido = titulo != null && titulo.Pessoa != null ? serWSPessoa.ConverterObjetoPessoa(titulo.Pessoa) : serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Tomador);
            cte.Empresa = fatura != null && fatura.Empresa != null ? serWSEmpresa.ConverterObjetoEmpresa(fatura.Empresa) : titulo != null && titulo.Empresa != null ? serWSEmpresa.ConverterObjetoEmpresa(titulo.Empresa) : serWSEmpresa.ConverterObjetoEmpresa(cargaCTe.CTe.Empresa);
            cte.Viagem = serWSCarga.ConverterObjetoViagem(cargaCTe.CTe.Viagem);
            cte.Banco = titulo != null && titulo.BoletoConfiguracao != null ? titulo.BoletoConfiguracao.NumeroBanco : fatura != null && fatura.Banco != null ? fatura.Banco.Numero.ToString("D") : "";

            return cte;
        }

        private string ObterFaturaPDF(Fatura fatura, bool codificarUtf8, UnitOfWork unitOfWork)
        {

            var result = ReportRequest
                .WithType(ReportType.Fatura)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoFatura", fatura.Codigo.ToString())
                .CallReport();

            if (Utilidades.IO.FileStorageService.Storage.Exists(result.FullPath))
            {
                byte[] dacte = null;
                dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(result.FullPath);
                string stringDacte = null;

                if (dacte != null)
                {
                    if (codificarUtf8)
                        stringDacte = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, dacte));


                    else
                        stringDacte = Convert.ToBase64String(dacte);
                }

                if (!string.IsNullOrWhiteSpace(stringDacte))
                    return stringDacte;
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe ConverterObjetoFaturasCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe cte = new Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe();
            cte.Chave = cargaCTe.CTe.Chave;
            cte.DataEmissao = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Numero = cargaCTe.CTe.Numero;
            cte.NumeroControle = cargaCTe.CTe.NumeroControle;
            cte.PDFCTe = this.ObterRetornoPDF(cargaCTe.CTe, codificarUTF8, unitOfWork);
            cte.Protocolo = cargaCTe.CTe.Codigo;

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cargaCTe.CTe.Titulo;
            Dominio.Entidades.Embarcador.Fatura.Fatura faturaFechada = null;

            if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                cte.NumeroBoleto = titulo.NossoNumero;
            else if (cargaCTe.CTe.Fatura != null && cargaCTe.CTe.Fatura.Parcelas != null && cargaCTe.CTe.Fatura.Parcelas.Count > 0 && cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo > 0)
            {
                faturaFechada = cargaCTe.CTe.Fatura;
                titulo = repTitulo.BuscarPorCodigo(cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }
            if (string.IsNullOrWhiteSpace(cte.NumeroBoleto))
            {
                titulo = repTitulo.BuscarTituloDocumentoPorCTe(cargaCTe.CTe.Codigo);
                if (titulo == null)
                    titulo = repTitulo.BuscarPorCTe(cargaCTe.CTe.Codigo);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }
            if (faturaFechada == null)
                faturaFechada = repFatura.BuscarPorCTe(cargaCTe.CTe.Codigo);

            cte.NumeroFaturaIndividual = titulo?.Codigo ?? 0;
            cte.PDFBoleto = this.ObterBoletoPDF(titulo, codificarUTF8, unitOfWork);
            cte.DataVencimento = titulo != null ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.DataEmissaoFormatado = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("yyyy/MM/dd HH:mm") : "";
            cte.DataVencimentoFormatado = titulo != null ? titulo.DataVencimento.Value.ToString("yyyy/MM/dd HH:mm") : "";
            cte.ValorIndividual = cargaCTe.CTe.ValorAReceber;

            if (faturaFechada != null)
                cte.Desconto = repFaturaDocumentoAcrescimoDesconto.BuscarDescontoPorFaturaDocumento(faturaFechada.Codigo, cargaCTe.CTe.Codigo);
            else
                cte.Desconto = 0;
            if (faturaFechada != null && faturaFechada.Total > 0)
                cte.ValorTotal = faturaFechada.Total - cte.Desconto;
            else
                cte.ValorTotal = cte.ValorIndividual;
            cte.Favorecido = titulo != null && titulo.Pessoa != null ? serWSPessoa.ConverterObjetoPessoa(titulo.Pessoa) : serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Tomador);
            cte.Empresa = faturaFechada != null && faturaFechada.Empresa != null ? serWSEmpresa.ConverterObjetoEmpresa(faturaFechada.Empresa) : titulo != null && titulo.Empresa != null ? serWSEmpresa.ConverterObjetoEmpresa(titulo.Empresa) : serWSEmpresa.ConverterObjetoEmpresa(cargaCTe.CTe.Empresa);
            cte.Viagem = serWSCarga.ConverterObjetoViagem(cargaCTe.CTe.Viagem);
            cte.Banco = titulo != null && titulo.BoletoConfiguracao != null ? titulo.BoletoConfiguracao.NumeroBanco : faturaFechada != null && faturaFechada.Banco != null ? faturaFechada.Banco.Numero.ToString("D") : "";

            cte.Faturas = new List<Dominio.ObjetosDeValor.WebService.CTe.Faturas>();
            List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarTodasPorCTe(cargaCTe.CTe.Codigo);
            if (faturas != null && faturas.Count > 0)
            {
                foreach (var fatura in faturas)
                {
                    Dominio.ObjetosDeValor.WebService.CTe.Faturas fat = new Dominio.ObjetosDeValor.WebService.CTe.Faturas()
                    {
                        NumeroFaturaAgrupado = fatura?.Numero ?? 0,
                        SituacaoFatura = fatura != null ? fatura.Situacao : titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                        PDFFatura = this.ObterFaturaPDF(fatura, codificarUTF8, unitOfWork),
                        DataEmissaoFatura = fatura != null ? fatura.DataFatura : titulo != null ? titulo.DataEmissao : null,
                        DataEmissaoFaturaFormatado = fatura != null ? fatura.DataFatura.ToString("yyyy/MM/dd HH:mm") : titulo != null ? titulo.DataEmissao.Value.ToString("yyyy/MM/dd HH:mm") : "",
                        ValorAgrupado = fatura?.Total ?? 0
                    };
                    cte.Faturas.Add(fat);
                }
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.CTe.CTe ConverterObjetoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);
            Servicos.WebService.Financeiro.TipoMovimento serWSTipoMovimento = new Servicos.WebService.Financeiro.TipoMovimento(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == cargaCTe.CTe.Codigo select obj).ToList();

            Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repCTe.BuscarDadosProdutoEmbarcador(cargaCTe.CTe.Codigo);
            Dominio.ObjetosDeValor.WebService.CTe.CTe cte = new Dominio.ObjetosDeValor.WebService.CTe.CTe();

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();
            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            cte.ItemServico = cargaCTe.CTe.ItemServico;

            decimal valor = cargaCTe.CTe.ValorAReceber;
            if (cargaCTe.CTe.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = cargaCTe.CTe.CentroResultado.PlanoContabilidade };
                if (cargaCTe.CTe.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = cargaCTe.CTe.ValorAReceber - cargaCTe.CTe.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = cargaCTe.CTe.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = cargaCTe.CTe.ValorMaximoCentroContabilizacao;
                }
            }

            if (cargaCTe.CTe.CentroResultadoFaturamento != null)
                cte.CentroResultadoFaturamento = new Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado() { Codigo = cargaCTe.CTe.CentroResultadoFaturamento.Codigo, Descricao = cargaCTe.CTe.CentroResultadoFaturamento.Descricao, PlanoContabilidade = cargaCTe.CTe.CentroResultadoFaturamento.PlanoContabilidade };

            if (cargaCTe.CTe.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = cargaCTe.CTe.CentroResultadoEscrituracao.PlanoContabilidade };

            if (cargaCTe.CTe.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = cargaCTe.CTe.CentroResultadoDestinatario.PlanoContabilidade };

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(cargaCTe.CTe.Codigo);

            cte.SituacaoPagamento = documentoFaturamento?.Pagamento?.Situacao.ObterDescricao();
            cte.NumeroPagamento = documentoFaturamento?.Pagamento?.Numero ?? 0;
            cte.Chave = cargaCTe.CTe.Chave;
            cte.ProtocoloAutorizacao = cargaCTe.CTe.Protocolo;
            cte.CFOP = cargaCTe.CTe.CFOP.CodigoCFOP;
            cte.DataEmissao = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaCTe.CTe.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(cargaCTe.CTe.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(cargaCTe.CTe.LocalidadeInicioPrestacao);
            cte.Lotacao = cargaCTe.CTe.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = cargaCTe.CTe.ModeloDocumentoFiscal.Numero;
            cte.Numero = cargaCTe.CTe.Numero;
            cte.NumeroControle = cargaCTe.CTe.NumeroControle;
            cte.NumeroCarga = cargaCTe.Carga.CodigoCargaEmbarcador;
            cte.NumeroCargaOrigem = cargaCTe.Carga.CargaAgrupada ? cargaCTe.CargaOrigem.CodigoCargaEmbarcador : null;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(cargaCTe.CTe, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = cargaCTe.CTe.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Remetente);
            cte.Serie = cargaCTe.CTe.Serie.Numero;
            cte.SituacaoCTeSefaz = cargaCTe.CTe.SituacaoCTeSefaz;
            cte.MensagemRetornoSefaz = cargaCTe.CTe.MensagemStatus?.MensagemDoErro ?? cargaCTe.CTe.MensagemRetornoSefaz;
            cte.TipoCTE = cargaCTe.CTe.TipoCTE;
            cte.TipoServico = cargaCTe.CTe.TipoServico;
            cte.TipoTomador = cargaCTe.CTe.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTe.CTe.Tomador);
            cte.Peso = cargaCTe.CTe.Peso;
            cte.PesoCubado = cargaCTe.CTe.PesoCubado;
            cte.PesoFaturado = cargaCTe.CTe.PesoFaturado;
            cte.DocumentoGlobalizado = cargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim;

            if (cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> ctes = repCTeSVMMultimodal.BuscarPorCTeSVM(cargaCTe.CTe.Codigo);
                if (ctes != null && ctes.Count > 0)
                {
                    cte.ChavesCTeCTM = new List<string>();
                    foreach (var cteCTM in ctes)
                    {
                        if (cteCTM != null && cteCTM.CTeMultimodal != null && !string.IsNullOrWhiteSpace(cteCTM.CTeMultimodal.Chave))
                            cte.ChavesCTeCTM.Add(cteCTM.CTeMultimodal.Chave);
                    }
                }
            }
            else if (cargaCTe.CTe.TipoModal == TipoModal.Multimodal)
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> ctes = repCTeSVMMultimodal.BuscarPorCTeMultiModal(cargaCTe.CTe.Codigo);
                if (ctes != null && ctes.Count > 0)
                {
                    cte.ChavesCTeSVM = new List<string>();
                    foreach (var cteSVM in ctes)
                    {
                        if (cteSVM != null && cteSVM.CTeSVM != null && !string.IsNullOrWhiteSpace(cteSVM.CTeSVM.Chave))
                            cte.ChavesCTeSVM.Add(cteSVM.CTeSVM.Chave);
                    }
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cargaCTe.CTe.Titulo;

            if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                cte.NumeroBoleto = titulo.NossoNumero;
            else if (cargaCTe.CTe.Fatura != null && cargaCTe.CTe.Fatura.Parcelas != null && cargaCTe.CTe.Fatura.Parcelas.Count > 0 && cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo > 0)
            {
                titulo = repTitulo.BuscarPorCodigo(cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }
            if (string.IsNullOrWhiteSpace(cte.NumeroBoleto))
            {
                titulo = repTitulo.BuscarPorCTeModeloNovo(cargaCTe.CTe.Codigo);
                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }

            cte.NumeroTitulo = titulo?.Codigo ?? 0;
            cte.PDFBoleto = this.ObterBoletoPDF(titulo, codificarUTF8, unitOfWork);
            cte.TipoDocumentoFiscal = cargaCTe.CTe.ModeloDocumentoFiscal?.Abreviacao ?? "";
            cte.MunicipioColeta = cargaCTe.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = cargaCTe.CTe.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = cargaCTe.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";
            cte.TipoIcms = cargaCTe.CTe.CST == "91" ? "90" : cargaCTe.CTe.CST;
            cte.AliquotaCofins = cargaCTe.CTe.AliquotaCOFINS;
            cte.FlagIsentoIcms = cargaCTe.CTe.CST == "40";
            cte.AliquotaPis = cargaCTe.CTe.AliquotaPIS;
            cte.ValorReducao = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorIcmsReduzido = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorBaseIcmsRemetente = 0;
            cte.ValorIcmsRemetente = 0;
            cte.ValorBaseIcmsDestinatario = cargaCTe.CTe.ValorICMSUFDestino > 0 ? cargaCTe.CTe.BaseCalculoICMS : 0;
            cte.ValorIcmsDestinatario = cargaCTe.CTe.ValorICMSUFDestino;
            cte.ValorBaseIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim > 0 ? cargaCTe.CTe.BaseCalculoICMS : 0;
            cte.FlagDebitoPisCofins = false;
            cte.FlagDebitoIcms = cargaCTe.CTe.CST == "60";
            cte.FlagIsentoPisCofins = cargaCTe.CTe.AliquotaPIS > 0 ? false : true;
            cte.ValorIss = cargaCTe.CTe.ValorINSS;
            cte.InticativoRetencaoIss = cargaCTe.CTe.ISSRetido;
            cte.DataEmbarque = cargaCTe.CTe.DataInicioPrestacaoServico.HasValue ? cargaCTe.CTe.DataInicioPrestacaoServico.Value.ToString("dd/MM/yyyy HH:mm") : "";
            if (cargaCTe.CTe.Viagem != null && cargaCTe.CTe.TerminalOrigem != null && cargaCTe.CTe.PortoOrigem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repSchedule.BuscarPorViagemPortoTerminal(cargaCTe.CTe.Viagem.Codigo, cargaCTe.CTe.PortoOrigem.Codigo, cargaCTe.CTe.TerminalOrigem.Codigo);
                if (schedule != null && schedule.DataPrevisaoChegadaNavio.HasValue)
                    cte.DataETA = schedule.DataPrevisaoChegadaNavio.Value.ToString("dd/MM/yyyy HH:mm");
                if (string.IsNullOrWhiteSpace(cte.DataEmbarque) && schedule != null && schedule.DataPrevisaoSaidaNavio.HasValue)
                    cte.DataEmbarque = schedule.DataPrevisaoSaidaNavio.Value.ToString("dd/MM/yyyy HH:mm");
            }
            cte.FlagIsendoContribuicoes = cargaCTe.CTe.CST == "41";
            cte.PercentualIcmsRemetente = cargaCTe.CTe.PercentualICMSPartilha;
            cte.AliquotaIcmsRemetente = cargaCTe.CTe.AliquotaICMSInterna;
            cte.PercentualIcmsDestinatario = 0;
            cte.AliquotaIcmsDestinatario = 0;
            cte.AliquotaIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim > 0 && cargaCTe.CTe.BaseCalculoICMS > 0 ? Math.Round(((cargaCTe.CTe.ValorICMSFCPFim * 100) / cargaCTe.CTe.BaseCalculoICMS), 2) : 0;
            cte.ValorIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim;
            cte.SFCSacado = cargaCTe.CTe.TomadorPagador?.CPF_CNPJ_SemFormato ?? "";
            cte.PrazoPgtoSacado = titulo != null && titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.PrazoPgtoCliente = titulo != null && titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.DataPreviaVencimento = cargaCTe.CTe.DataPreviaVencimento.HasValue ? cargaCTe.CTe.DataPreviaVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.ValorBaseIcms = cargaCTe.CTe.BaseCalculoICMS;
            cte.AliquotaISS = cargaCTe.CTe.AliquotaICMS;

            if (cargaCTe.CargaCTeComplementoInfo != null && cargaCTe.CargaCTeComplementoInfo.CargaOcorrencia != null)
                cte.NumeroOcorrencia = cargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia;

            cte.CTeFaturadoExclusivamente = false;
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;
            if (cargaCTe.CTe.TomadorPagador != null && cargaCTe.CTe.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && cargaCTe.CTe.TomadorPagador != null && cargaCTe.CTe.TomadorPagador.Cliente != null && cargaCTe.CTe.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, cargaCTe.CTe.TomadorPagador.GrupoPessoas.Codigo);
            if (acordoFaturamento != null && cargaCTe.Carga != null && cargaCTe.Carga.Pedidos != null && cargaCTe.Carga.Pedidos.Count > 0)
            {
                if (cargaCTe.Carga.Pedidos.Any(c => c.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.BLLongoCurso))
                    cte.CTeFaturadoExclusivamente = acordoFaturamento.FaturamentoPermissaoExclusivaLongoCurso;
                else
                    cte.CTeFaturadoExclusivamente = acordoFaturamento.FaturamentoPermissaoExclusivaCabotagem;

                if (cargaCTe.Carga.TipoOperacao != null && cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                {
                    if (cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        cte.CTeFaturadoExclusivamente = acordoFaturamento.FaturamentoPermissaoExclusivaLongoCurso;
                    if (cargaCTe.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        cte.CTeFaturadoExclusivamente = acordoFaturamento.FaturamentoPermissaoExclusivaCustoExtra;
                }
            }

            cte.DataAutorizacao = cargaCTe.CTe.DataAutorizacao.HasValue ? cargaCTe.CTe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataEntrega = cargaCTe.CTe.DataEntrega.HasValue ? cargaCTe.CTe.DataEntrega.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataColeta = cargaCTe.CTe.DataColeta.HasValue ? cargaCTe.CTe.DataColeta.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataPrevisaoEntrega = cargaCTe.CTe.DataPrevistaEntrega.HasValue ? cargaCTe.CTe.DataPrevistaEntrega.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.NumeroManifesto = cargaCTe.CTe.NumeroManifesto;
            cte.NumeroManifestoTransbordo = cargaCTe.CTe.NumeroManifestoTransbordo;
            cte.NumeroCEMercante = cargaCTe.CTe.NumeroCEMercante;
            cte.ProdutoPredominante = cargaCTe.CTe.ProdutoPredominanteCTe;
            cte.DataCancelamento = cargaCTe.CTe.DataCancelamento.HasValue ? cargaCTe.CTe.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataAnulacao = cargaCTe.CTe.DataAnulacao.HasValue ? cargaCTe.CTe.DataAnulacao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataEmissaoFatura = cargaCTe.CTe.Fatura != null ? cargaCTe.CTe.Fatura.DataFatura.ToString("dd/MM/yyyy HH:mm") : "";
            cte.DataEmissaoBoleto = titulo != null ? titulo.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.ProtocoloCancelamentoInutilizacao = cargaCTe.CTe.ProtocoloCancelamentoInutilizacao;
            cte.PossuiCTeComplementar = cargaCTe.CTe.PossuiCTeComplementar;

            if (configuracao.UtilizaEmissaoMultimodal)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCTe.Codigo);

                cte.ValorTaxaFeeder = cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0m;
                cte.Afretamento = cargaPedido?.Pedido?.EmbarqueAfretamentoFeeder ?? false;
                cte.NumeroProtocoloANTAQ = cargaPedido?.Pedido?.ProtocoloANTAQFeeder ?? "";
                cte.NumeroManifestoFEEDER = cargaPedido?.Pedido?.NumeroManifestoFeeder ?? "";
                cte.NumeroCEFEEDER = cargaPedido?.Pedido?.NumeroCEFeeder ?? "";

                cte.NumeroCTeSubstituto = cargaCTe.CTe.NumeroCTeSubstituto;
                cte.NumeroControleCTeSubstituto = cargaCTe.CTe.NumeroControleCTeSubstituto;

                cte.NumeroCTeAnulacao = cargaCTe.CTe.NumeroCTeAnulacao;
                cte.NumeroControleCTeAnulacao = cargaCTe.CTe.NumeroControleCTeAnulacao;

                cte.NumeroCTeComplementar = cargaCTe.CTe.NumeroCTeComplementar;
                cte.NumeroControleCTeComplementar = cargaCTe.CTe.NumeroControleCTeComplementar;

                cte.NumeroCTeDuplicado = cargaCTe.CTe.NumeroCTeDuplicado;
                cte.NumeroControleCTeDuplicado = cargaCTe.CTe.NumeroControleCTeDuplicado;

                cte.NumeroCTeOriginal = cargaCTe.CTe.NumeroCTeOriginal;
                cte.NumeroControleCTeOriginal = cargaCTe.CTe.NumeroControleCTeOriginal;

                cte.NumeroMDFeVinculado = cargaCTe.CTe.ListaMDFes;
                cte.DataEmissaoMDFeVinculado = cargaCTe.CTe.DataEmissaoMDFe.HasValue ? cargaCTe.CTe.DataEmissaoMDFe.Value.ToString("dd/MM/yyyy HH:mm") : "";
                cte.StatusMDFeVinculado = cargaCTe.CTe.StatusMDFe;
            }

            Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = repTituloDocumento.BuscarPrimeiroPorCTe(cargaCTe.CTe.Codigo);
            if (tituloDocumento != null)
            {
                cte.CTeTitulo = new CTeTitulo()
                {
                    BoletoConfiguracao = tituloDocumento.Titulo.BoletoConfiguracao?.DescricaoBanco ?? "",
                    BoletoEnviadoPorEmail = tituloDocumento.Titulo.BoletoEnviadoPorEmail,
                    BoletoGeradoAutomaticamente = tituloDocumento.Titulo.BoletoGeradoAutomaticamente,
                    Codigo = tituloDocumento.Titulo.Codigo,
                    BoletoStatusTitulo = tituloDocumento.Titulo.BoletoStatusTitulo,
                    DataAlteracao = tituloDocumento.Titulo.DataAlteracao,
                    DataBaseCRT = tituloDocumento.Titulo.DataBaseCRT,
                    DataEmissao = tituloDocumento.Titulo.DataEmissao,
                    DataLancamento = tituloDocumento.Titulo.DataLancamento,
                    DataProgramacaoPagamento = tituloDocumento.Titulo.DataProgramacaoPagamento,
                    DataVencimento = tituloDocumento.Titulo.DataVencimento,
                    Empresa = null,
                    EnviarDocumentacaoFaturamentoCTe = tituloDocumento.Titulo.EnviarDocumentacaoFaturamentoCTe,
                    FormaTitulo = tituloDocumento.Titulo.FormaTitulo,
                    GerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false,
                    GrupoPessoas = serWSPessoa.ConverterObjetoGrupoPessoa(tituloDocumento.Titulo.GrupoPessoas),
                    MoedaCotacaoBancoCentral = tituloDocumento.Titulo.MoedaCotacaoBancoCentral,
                    NumeroDocumentoTituloOriginal = tituloDocumento.Titulo.NumeroDocumentoTituloOriginal,
                    Pessoa = serWSPessoa.ConverterObjetoPessoa(tituloDocumento.Titulo.Pessoa),
                    Sequencia = tituloDocumento.Titulo.Sequencia,
                    StatusTitulo = tituloDocumento.Titulo.StatusTitulo,
                    TipoDocumentoTituloOriginal = tituloDocumento.Titulo.TipoDocumentoTituloOriginal,
                    TipoMovimento = serWSTipoMovimento.ConverterObjetoTipoMovimento(tituloDocumento.Titulo.TipoMovimento, unitOfWork),
                    TipoTitulo = tituloDocumento.Titulo.TipoTitulo,
                    Usuario = tituloDocumento.Titulo.Usuario?.CPF,
                    Valor = tituloDocumento.Titulo.Valor,
                    ValorMoedaCotacao = tituloDocumento.Titulo.ValorMoedaCotacao,
                    ValorOriginal = tituloDocumento.Titulo.ValorOriginal,
                    ValorOriginalMoedaEstrangeira = tituloDocumento.Titulo.ValorOriginalMoedaEstrangeira,
                    ValorPendente = tituloDocumento.Titulo.ValorPendente,
                    ValorTituloOriginal = tituloDocumento.Titulo.ValorTituloOriginal,
                    ValorTotal = tituloDocumento.Titulo.ValorTotal,
                    NossoNumero = tituloDocumento.Titulo.NossoNumero
                };
            }

            cte.DocumentosAnterior = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeAnterior>();
            cte.OcorreuSinistroAvaria = cargaCTe.CTe.OcorreuSinistroAvaria;
            cte.TipoModal = cargaCTe.CTe.TipoModal;

            if (cargaCTe.Carga.TipoOperacao != null)
            {
                cte.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao();
                cte.TipoOperacao.Descricao = cargaCTe.Carga.TipoOperacao.Descricao;
                cte.TipoOperacao.CodigoIntegracao = cargaCTe.Carga.TipoOperacao.CodigoIntegracao;
            }

            cte.NaturezaOperacao = "";
            cte.NaturezaServico = "";
            if (cte.Modelo != "57" && !string.IsNullOrWhiteSpace(cargaCTe.CTe.ModeloDocumentoFiscal?.Relatorio))
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0, cargaCTe.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", cargaCTe.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0, cargaCTe.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", 0, 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0, "", cargaCTe.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0, "", 0, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0, "", 0, 0);
                if (configuracaoNFSe != null)
                {
                    cte.NaturezaServico = configuracaoNFSe?.ServicoNFSe?.Descricao ?? string.Empty;
                    cte.NaturezaOperacao = configuracaoNFSe?.NaturezaNFSe?.Descricao ?? string.Empty;
                }
            }
            decimal pesoTotalCTeAnterior = 0;

            if (!(configuracaoCargaIntegracao?.NaoRetornarDocumentosAnteriores ?? false))
            {
                List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnteriores = repDocAnterior.BuscarPorCTe(cargaCTe.CTe.Codigo);
                if (documentosAnteriores != null && documentosAnteriores.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> lstCteTerceiro = repCTeTerceiro.BuscarPorChave(documentosAnteriores.Select(x => x.Chave).ToList());
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> lstXmlNota = repXMLNotaFiscal.BuscarPorChave(documentosAnteriores.Select(x => x.Chave).ToList());
                    List<Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador> lstProdutoEmbarcador = repCTe.BuscarDadosProdutoEmbarcador(documentosAnteriores.Select(x => x.Chave).ToList());

                    foreach (var documento in documentosAnteriores)
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = lstCteTerceiro.Where(x => x.ChaveAcesso == documento.Chave).FirstOrDefault();
                        if (produtoEmbarcador == null)
                            produtoEmbarcador = lstProdutoEmbarcador.Where(x => x.ChaveNF == documento.Chave).FirstOrDefault();

                        if (cteTerceiro != null)
                        {
                            pesoTotalCTeAnterior += cteTerceiro.Peso;
                            Dominio.ObjetosDeValor.WebService.CTe.CTeAnterior cteAnterior = new Dominio.ObjetosDeValor.WebService.CTe.CTeAnterior()
                            {
                                ChaveCTe = documento.Chave,
                                CNPJCPF = cteTerceiro.TransportadorTerceiro?.CPF_CNPJ_SemFormato ?? "",
                                DataHoraEmissao = cteTerceiro.DataEmissao > DateTime.MinValue ? cteTerceiro.DataEmissao.ToString("dd/MM/yyyy HH:mm") : "",
                                IERG = cteTerceiro.TransportadorTerceiro?.IE_RG ?? "",
                                Numero = cteTerceiro.Numero,
                                PesoTotal = cteTerceiro.Peso,
                                Serie = cteTerceiro.Serie,
                                TipoDocumento = "55",
                                ValorMercadoria = cteTerceiro.ValorTotalMercadoria,
                                ValorTotal = cteTerceiro.ValorAReceber,
                                Notas = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeAnteriorNota>()
                            };
                            if (cteTerceiro.CTesTerceiroNFes != null && cteTerceiro.CTesTerceiroNFes.Count > 0)
                            {
                                foreach (var nota in cteTerceiro.CTesTerceiroNFes)
                                {
                                    if (nota != null)
                                    {
                                        Dominio.ObjetosDeValor.WebService.CTe.CTeAnteriorNota nfe = new Dominio.ObjetosDeValor.WebService.CTe.CTeAnteriorNota()
                                        {
                                            ChaveNFe = nota.Chave,
                                            Numero = nota.Numero,
                                            ReferenciaEDI = nota.NumeroReferenciaEDI,
                                            Serie = nota.Serie,
                                            PINSuframa = nota.PINSuframa,
                                            NCMPredominante = nota.NCM,
                                            CodigoProduto = produtoEmbarcador != null ? produtoEmbarcador.CodigoDocumentacao : ""
                                        };

                                        if (string.IsNullOrWhiteSpace(nfe.NCMPredominante) || string.IsNullOrWhiteSpace(nfe.ReferenciaEDI))
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = lstXmlNota.Where(x => x.Chave == nfe.ChaveNFe).FirstOrDefault();

                                            if (xmlNota != null && string.IsNullOrWhiteSpace(nfe.NCMPredominante) && !string.IsNullOrWhiteSpace(xmlNota.XML) && xmlNota.XML.Contains("NCM"))
                                            {
                                                if (xmlNota.XML.Contains("<NCM>"))
                                                    nfe.NCMPredominante = xmlNota.XML.Substring(xmlNota.XML.IndexOf("<NCM>") + 6, 4);
                                                else if (xmlNota.XML.Contains("\"NCM\":"))
                                                    nfe.NCMPredominante = xmlNota.XML.Substring(xmlNota.XML.IndexOf("\"NCM\":") + 6, 4);
                                            }
                                            if (xmlNota != null && string.IsNullOrWhiteSpace(nfe.ReferenciaEDI) && !string.IsNullOrWhiteSpace(xmlNota.NumeroReferenciaEDI))
                                                nfe.ReferenciaEDI = xmlNota.NumeroReferenciaEDI;
                                        }

                                        if (string.IsNullOrWhiteSpace(nfe.NCMPredominante) && produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM) && produtoEmbarcador.CodigoNCM.Length == 4)
                                            nfe.NCMPredominante = produtoEmbarcador.CodigoNCM;

                                        cteAnterior.Notas.Add(nfe);
                                    }
                                }

                            }
                            cte.DocumentosAnterior.Add(cteAnterior);
                        }
                        else
                        {
                            int.TryParse(documento.Numero, out int numeroDocAnterior);
                            if (!string.IsNullOrWhiteSpace(documento.Numero) && cte.DocumentosAnterior != null && cte.DocumentosAnterior.Count > 0 && cte.DocumentosAnterior.Any(c => c.ChaveCTe == documento.Numero))
                                continue;
                            Dominio.ObjetosDeValor.WebService.CTe.CTeAnterior cteAnterior = new Dominio.ObjetosDeValor.WebService.CTe.CTeAnterior()
                            {
                                ChaveCTe = documento.Numero,
                                CNPJCPF = documento.Emissor?.CPF_CNPJ_Formatado ?? "",
                                DataHoraEmissao = documento.DataEmissao.HasValue && documento.DataEmissao.Value > DateTime.MinValue ? documento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                IERG = documento.Emissor?.IE_RG ?? "",
                                Numero = numeroDocAnterior,
                                PesoTotal = 0,
                                Serie = documento.Serie,
                                TipoDocumento = documento.Tipo,
                                ValorMercadoria = 0,
                                ValorTotal = 0,
                                Notas = null
                            };
                            cte.DocumentosAnterior.Add(cteAnterior);
                        }
                    }
                }
            }

            cte.ChaveCTeVinculado = cargaCTe.CTe.ChaveCTESubComp;
            if ((cargaCTe.CargaOrigem?.Protocolo ?? 0) > 0)
                cte.ProtocoloCarga = cargaCTe.CargaOrigem?.Protocolo ?? 0;
            else if ((cargaCTe.Carga?.Protocolo ?? 0) > 0)
                cte.ProtocoloCarga = cargaCTe.Carga?.Protocolo ?? 0;
            else if ((cargaCTe.CargaOrigem?.Codigo ?? 0) > 0)
                cte.ProtocoloCarga = cargaCTe.CargaOrigem?.Codigo ?? 0;

            if (!(configuracaoCargaIntegracao?.NaoRetornarDocumentosAnteriores ?? false))
            {
                cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
                if (cargaCTe.CTe.Containers != null && cargaCTe.CTe.Containers.Count > 0)
                {
                    foreach (var container in cargaCTe.CTe.Containers)
                    {
                        if (container.Container != null)
                        {
                            bool containerEstaNoCTe = (container.Documentos == null || container.Documentos.Count == 0 || cargaCTe.CTe.Documentos == null || cargaCTe.CTe.Documentos.Count == 0);
                            if (!containerEstaNoCTe)
                                containerEstaNoCTe = cargaCTe.CTe.Documentos.Any(d => container.Documentos.Any(c => c.Chave == d.ChaveNFE));

                            if (!containerEstaNoCTe)
                                containerEstaNoCTe = cargaCTe.CTe.Documentos.Any(d => d.ChaveNFE == "" || d.ChaveNFE == null);

                            if (containerEstaNoCTe)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                                cont = serWSCarga.ConverterObjetoContainer(container.Container);
                                if (cont != null)
                                {
                                    cont.Lacre1 = container.Lacre1;
                                    cont.Lacre2 = container.Lacre2;
                                    cont.Lacre3 = container.Lacre3;
                                    cont.Volume = 0;
                                    if (produtoEmbarcador != null && produtoEmbarcador.MetroCubito > 0)
                                    {
                                        decimal pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0)
                                            pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0)
                                            pesoContainer = pesoTotalCTeAnterior;

                                        if (pesoContainer <= 0 && cargaCTe.CTe.QuantidadesCarga != null && cargaCTe.CTe.QuantidadesCarga.Any(o => o.UnidadeMedida == "01"))
                                            pesoContainer = cargaCTe.CTe.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);

                                        if (pesoContainer > 0)
                                        {
                                            cont.Volume = produtoEmbarcador.MetroCubito > 0 ? pesoContainer / produtoEmbarcador.MetroCubito : 0;
                                            cont.PesoLiquido = pesoContainer;
                                            cont.DencidadeProduto = produtoEmbarcador.MetroCubito;
                                        }
                                    }
                                    cte.Containeres.Add(cont);
                                }
                            }
                        }
                    }
                }
            }

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFrete = repCargaCTeComponentesFrete.BuscarPorSemComposicaoFreteLiquidoCargaCTe(cargaCTe.Codigo);
            cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in cargaCTeComponentesFrete)
            {
                cte.ValorFrete.ComponentesAdicionais.Add(serComponenteFrete.ConverterObjetoComponenteCargaCTe(componente));
            }
            cte.ValorFrete.FreteProprio = Math.Round(cargaCTe.CTe.ValorFrete, 2);
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = cargaCTe.CTe.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = cargaCTe.CTe.CST == "91" ? "90" : cargaCTe.CTe.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = cargaCTe.CTe.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = cargaCTe.CTe.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = cargaCTe.CTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = cargaCTe.CTe.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = Math.Round(cargaCTe.CTe.ValorAReceber, 2, MidpointRounding.AwayFromZero);
            cte.ValorFrete.ValorPrestacaoServico = Math.Round(cargaCTe.CTe.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero);
            cte.ValorFrete.ValorAReceberSemImpostoIncluso = Math.Round(cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? cargaCTe.CTe.ValorAReceber - cargaCTe.CTe.ValorICMS : cargaCTe.CTe.ValorAReceber, 2, MidpointRounding.AwayFromZero);

            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                cte.ValorFrete.ISS = new Dominio.ObjetosDeValor.Embarcador.ISS.ISS();
                cte.ValorFrete.ISS.Aliquota = cargaCTe.CTe.AliquotaISS;
                cte.ValorFrete.ISS.IncluirISSBaseCalculo = cargaCTe.CTe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                cte.ValorFrete.ISS.PercentualRetencao = cargaCTe.CTe.PercentualISSRetido;
                cte.ValorFrete.ISS.ValorBaseCalculoISS = cargaCTe.CTe.BaseCalculoISS;
                cte.ValorFrete.ISS.ValorISS = cargaCTe.CTe.ValorISS;
                cte.ValorFrete.ISS.ValorRetencaoISS = cargaCTe.CTe.ValorISSRetido;

                cte.NumeroRPS = cargaCTe.CTe.RPS?.Numero ?? 0;
                cte.SerieRPS = cargaCTe.CTe.RPS?.Serie;
            }

            cte.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS();
            cte.ValorFrete.IBSCBS.CST = cargaCTe.CTe.CSTIBSCBS;
            cte.ValorFrete.IBSCBS.ClassificacaoTributaria = cargaCTe.CTe.ClassificacaoTributariaIBSCBS;
            cte.ValorFrete.IBSCBS.BaseCalculo = cargaCTe.CTe.BaseCalculoIBSCBS;
            cte.ValorFrete.IBSCBS.AliquotaIBSEstadual = cargaCTe.CTe.AliquotaIBSEstadual;
            cte.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual = cargaCTe.CTe.PercentualReducaoIBSEstadual;
            cte.ValorFrete.IBSCBS.ValorIBSEstadual = cargaCTe.CTe.ValorIBSEstadual;
            cte.ValorFrete.IBSCBS.AliquotaIBSMunicipal = cargaCTe.CTe.AliquotaIBSMunicipal;
            cte.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal = cargaCTe.CTe.PercentualReducaoIBSMunicipal;
            cte.ValorFrete.IBSCBS.ValorIBSMunicipal = cargaCTe.CTe.ValorIBSMunicipal;
            cte.ValorFrete.IBSCBS.AliquotaCBS = cargaCTe.CTe.AliquotaCBS;
            cte.ValorFrete.IBSCBS.PercentualReducaoCBS = cargaCTe.CTe.PercentualReducaoCBS;
            cte.ValorFrete.IBSCBS.ValorCBS = cargaCTe.CTe.ValorCBS;

            if (cargaCTe.CTe.ValorTotalDocumentoFiscal > 0)
                cte.ValorFrete.ValorTotalDocumentoFiscal = cargaCTe.CTe.ValorTotalDocumentoFiscal;
            else if (cargaCTe.CTe.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = cargaCTe.CTe.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(cargaCTe?.CTe?.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (cargaCTe.CTe?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + cargaCTe.CTe.ValorIBSMunicipal + cargaCTe.CTe.ValorIBSEstadual + cargaCTe.CTe.ValorCBS;

                cte.ValorFrete.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            cte.ValorTotalMercadoria = cargaCTe.CTe.ValorTotalMercadoria;
            cte.VersaoCTE = cargaCTe.CTe.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(cargaCTe.CTe, unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTe.CTe, "A", unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTe.CTe, "C", unitOfWork) : "";

            cte.MotivoCancelamento = cargaCTe.CTe.ObservacaoCancelamento;
            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                cte.NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroPrefeituraNFSe) ? cargaCTe.CTe.NumeroPrefeituraNFSe : cargaCTe.CTe.Numero.ToString();

            cte.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
            foreach (Dominio.Entidades.Usuario motorista in cargaCTe.Carga.Motoristas)
                cte.Motoristas.Add(serWSMotorista.ConverterObjetoMotorista(motorista));

            cte.NumeroOS = cargaCTe.CTe.NumeroOS;
            cte.CodigoIntegracaoOperador = cargaCTe.Carga.Operador?.CodigoIntegracao;
            cte.ObservacaoJustificativaAprovacaoCarga = cargaCTe.Carga.Observacao;
            cte.CodigoIntegracaoJustificativaAprovacaoCarga = cargaCTe.Carga.JustificativaAutorizacaoCarga?.CodigoIntegracao;

            cte.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(cargaCTe.Carga.Veiculo, cargaCTe.Carga.VeiculosVinculados != null && cargaCTe.Carga.VeiculosVinculados.Count > 0 ? cargaCTe.Carga.VeiculosVinculados.ToList() : null, unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);
            cte.ProtocolosDePedidos = new List<int>();
            List<int> codigosCargasPedidoCTe = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoCTe)
            {
                cte.ProtocolosDePedidos.Add(cargaPedido.Pedido.Codigo);
                codigosCargasPedidoCTe.Add(cargaPedido.Codigo);
            }

            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();

            if (cargaCTe.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento || (cargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !configuracao.NaoRetornarNotasEmDocumentoComplementar))
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFIscal = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidoComFetch(codigosCargasPedidoCTe);


                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                    Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();

                    if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    {
                        nota.ChaveNFe = documentoCTe.ChaveNFE;
                        nota.Numero = documentoCTe.Numero;
                        nota.Serie = documentoCTe.Serie;

                        if (cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Exists(c => c.ChaveNFe == nota.ChaveNFe))
                            continue;

                        pedido = pedidosXMLNotaFIscal.Find(pedidoXMLNotaFiscal => pedidoXMLNotaFiscal.XMLNotaFiscal.Chave == documentoCTe.ChaveNFE)?.CargaPedido?.Pedido;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                            nota.Numero = documentoCTe.Numero;
                        if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                            nota.Serie = documentoCTe.Serie;
                        if (!string.IsNullOrWhiteSpace(nota.Serie) && !string.IsNullOrWhiteSpace(nota.Numero) && cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Any(c => c.Numero == nota.Numero && c.Serie == nota.Serie))
                            continue;
                        if (!string.IsNullOrWhiteSpace(documentoCTe.Descricao) && !string.IsNullOrWhiteSpace(nota.Numero))
                        {
                            nota.Descricao = documentoCTe.Descricao;
                            if (cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Exists(c => c.Descricao == nota.Descricao && nota.Numero == c.Numero))
                                continue;
                        }
                        else if (!string.IsNullOrWhiteSpace(documentoCTe.Descricao))
                        {
                            nota.Descricao = documentoCTe.Descricao;
                            if (cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Exists(c => c.Descricao == nota.Descricao))
                                continue;
                        }
                    }

                    if (cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = cargaCTe.CTe.XMLNotaFiscais.FirstOrDefault(o => o.Chave == documentoCTe.ChaveNFE);

                        if (xmlNota != null)
                        {
                            nota.Volume = xmlNota.Volumes;
                            nota.PINSuframa = xmlNota.PINSUFRAMA;
                            nota.NCMPredominante = xmlNota.NCM;
                            nota.NumeroReferenciaEDI = xmlNota.NumeroReferenciaEDI;
                            nota.NumeroControleCliente = xmlNota.NumeroControleCliente;
                            if (!string.IsNullOrWhiteSpace(xmlNota.NumeroCanhoto))
                                nota.NumeroCanhoto = xmlNota.NumeroCanhoto;
                            else if (xmlNota.Numero > 0 && xmlNota.Emitente != null)
                                nota.NumeroCanhoto = xmlNota.Numero.ToString("D").PadLeft(9, '0') + xmlNota.SerieOuSerieDaChave.PadLeft(3, '0') + xmlNota.Emitente.CPF_CNPJ.ToString().PadLeft(14, '0');
                        }
                    }

                    if (string.IsNullOrWhiteSpace(nota.NCMPredominante) && produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM) && produtoEmbarcador.CodigoNCM.Length == 4)
                        nota.NCMPredominante = produtoEmbarcador.CodigoNCM;
                    nota.CodigoProduto = produtoEmbarcador != null ? produtoEmbarcador.CodigoDocumentacao : "";

                    if (!string.IsNullOrWhiteSpace(pedido?.NumeroPedidoEmbarcador ?? ""))
                        nota.NumeroPedidoEmbarcador = pedido?.NumeroPedidoEmbarcador;

                    if (!string.IsNullOrWhiteSpace(pedido?.Adicional1 ?? ""))
                        nota.ProcImportacao = pedido?.Adicional1;

                    if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                        cte.Documentos.Add(nota);
                }
            }

            bool isUtilizarXCampoSomenteNoRedespacho = cargaCTe.Carga.TipoOperacao?.UtilizarXCampoSomenteNoRedespacho ?? false;
            bool isFlagXCampoValido = !isUtilizarXCampoSomenteNoRedespacho || (isUtilizarXCampoSomenteNoRedespacho && cargaCTe.Carga.Redespacho != null);
            if (!string.IsNullOrWhiteSpace(cargaCTe.Carga.TipoOperacao?.DocumentoXCampo) && !string.IsNullOrWhiteSpace(cargaCTe.Carga.TipoOperacao?.DocumentoXTexto) && isFlagXCampoValido)
            {
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>
                {
                    new Dominio.ObjetosDeValor.CTe.Observacao()
                    {
                        Identificador = cargaCTe.Carga.TipoOperacao.DocumentoXCampo,
                        Descricao = cargaCTe.Carga.TipoOperacao.DocumentoXTexto
                    }
                };
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.CTe.CTe ConverterObjetoCargaCTeComplementoInfo(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, bool codificarUTF8, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            Dominio.ObjetosDeValor.WebService.CTe.CTe cte = new Dominio.ObjetosDeValor.WebService.CTe.CTe();
            cte.Chave = cargaCTeComplementoInfo.CTe.Chave;
            cte.CFOP = cargaCTeComplementoInfo.CTe.CFOP.CodigoCFOP;
            cte.DataEmissao = cargaCTeComplementoInfo.CTe.DataEmissao.HasValue ? cargaCTeComplementoInfo.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaCTeComplementoInfo.CTe.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(cargaCTeComplementoInfo.CTe.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao);
            cte.Lotacao = cargaCTeComplementoInfo.CTe.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Numero;
            cte.Numero = cargaCTeComplementoInfo.CTe.Numero;
            cte.NumeroControle = cargaCTeComplementoInfo.CTe.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(cargaCTeComplementoInfo.CTe, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = cargaCTeComplementoInfo.CTe.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Remetente);
            cte.Serie = cargaCTeComplementoInfo.CTe.Serie.Numero;
            cte.SituacaoCTeSefaz = cargaCTeComplementoInfo.CTe.SituacaoCTeSefaz;
            cte.TipoCTE = cargaCTeComplementoInfo.CTe.TipoCTE;
            cte.TipoServico = cargaCTeComplementoInfo.CTe.TipoServico;
            cte.TipoTomador = cargaCTeComplementoInfo.CTe.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Tomador);

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = cargaCTeComplementoInfo.CTe.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = cargaCTeComplementoInfo.CTe.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = cargaCTeComplementoInfo.CTe.CST == "91" ? "90" : cargaCTeComplementoInfo.CTe.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = cargaCTeComplementoInfo.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = cargaCTeComplementoInfo.CTe.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = cargaCTeComplementoInfo.CTe.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = cargaCTeComplementoInfo.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = cargaCTeComplementoInfo.CTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = cargaCTeComplementoInfo.CTe.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = cargaCTeComplementoInfo.CTe.ValorICMS;

            cte.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS
            {
                AliquotaCBS = cargaCTeComplementoInfo.CTe.AliquotaCBS,
                AliquotaIBSEstadual = cargaCTeComplementoInfo.CTe.AliquotaIBSEstadual,
                AliquotaIBSMunicipal = cargaCTeComplementoInfo.CTe.AliquotaIBSMunicipal,
                BaseCalculo = cargaCTeComplementoInfo.CTe.BaseCalculoIBSCBS,
                ClassificacaoTributaria = cargaCTeComplementoInfo.CTe.ClassificacaoTributariaIBSCBS,
                CST = cargaCTeComplementoInfo.CTe.CSTIBSCBS,
                PercentualReducaoCBS = cargaCTeComplementoInfo.CTe.PercentualReducaoCBS,
                PercentualReducaoIBSEstadual = cargaCTeComplementoInfo.CTe.PercentualReducaoIBSEstadual,    
                PercentualReducaoIBSMunicipal = cargaCTeComplementoInfo.CTe.PercentualReducaoIBSMunicipal, 
                ValorCBS = cargaCTeComplementoInfo.CTe.ValorCBS,
                ValorIBSEstadual = cargaCTeComplementoInfo.CTe.ValorIBSEstadual,
                ValorIBSMunicipal = cargaCTeComplementoInfo.CTe.ValorIBSMunicipal
            };

            if (cargaCTeComplementoInfo.CTe.ValorTotalDocumentoFiscal > 0)
                cte.ValorFrete.ValorTotalDocumentoFiscal = cargaCTeComplementoInfo.CTe.ValorTotalDocumentoFiscal;
            else if (cargaCTeComplementoInfo.CTe.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = cargaCTeComplementoInfo.CTe.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(cargaCTeComplementoInfo.CTe?.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (cargaCTeComplementoInfo.CTe?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + cargaCTeComplementoInfo.CTe.ValorIBSMunicipal + cargaCTeComplementoInfo.CTe.ValorIBSEstadual + cargaCTeComplementoInfo.CTe.ValorCBS;

                cte.ValorFrete.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            cte.ValorFrete.ValorTotalAReceber = cargaCTeComplementoInfo.CTe.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = cargaCTeComplementoInfo.CTe.ValorPrestacaoServico;
            cte.ProtocoloAutorizacao = cargaCTeComplementoInfo.CTe.Protocolo;
            cte.DataAutorizacao = cargaCTeComplementoInfo.CTe.DataAutorizacao.HasValue ? cargaCTeComplementoInfo.CTe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : "";

            cte.DataEmbarque = cargaCTeComplementoInfo.CTe.DataInicioPrestacaoServico.HasValue ? cargaCTeComplementoInfo.CTe.DataInicioPrestacaoServico.Value.ToString("dd/MM/yyyy HH:mm") : "";
            if (cargaCTeComplementoInfo.CTe.Viagem != null && cargaCTeComplementoInfo.CTe.TerminalOrigem != null && cargaCTeComplementoInfo.CTe.PortoOrigem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repSchedule.BuscarPorViagemPortoTerminal(cargaCTeComplementoInfo.CTe.Viagem.Codigo, cargaCTeComplementoInfo.CTe.PortoOrigem.Codigo, cargaCTeComplementoInfo.CTe.TerminalOrigem.Codigo);
                if (schedule != null && schedule.DataPrevisaoChegadaNavio.HasValue)
                    cte.DataETA = schedule.DataPrevisaoChegadaNavio.Value.ToString("dd/MM/yyyy HH:mm");
                if (string.IsNullOrWhiteSpace(cte.DataEmbarque) && schedule != null && schedule.DataPrevisaoSaidaNavio.HasValue)
                    cte.DataEmbarque = schedule.DataPrevisaoSaidaNavio.Value.ToString("dd/MM/yyyy HH:mm");
            }
            cte.DataPreviaVencimento = cargaCTeComplementoInfo.CTe.DataPreviaVencimento.HasValue ? cargaCTeComplementoInfo.CTe.DataPreviaVencimento.Value.ToString("dd/MM/yyyy") : "";

            cte.ValorTotalMercadoria = cargaCTeComplementoInfo.CTe.ValorTotalMercadoria;
            cte.VersaoCTE = cargaCTeComplementoInfo.CTe.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(cargaCTeComplementoInfo.CTe, unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTeComplementoInfo.CTe, "A", unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTeComplementoInfo.CTe, "C", unitOfWork) : "";

            cte.MotivoCancelamento = cargaCTeComplementoInfo.CTe.ObservacaoCancelamento;
            if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                cte.NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(cargaCTeComplementoInfo.CTe.NumeroPrefeituraNFSe) ? cargaCTeComplementoInfo.CTe.NumeroPrefeituraNFSe : cargaCTeComplementoInfo.CTe.Numero.ToString();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

            cte.ProtocolosDePedidos = new List<int>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoCTe)
                {
                    cte.ProtocolosDePedidos.Add(cargaPedido.Pedido.Protocolo);
                }
            }

            return cte;
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.Titulo> ConverterObjetoTitulo(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.Titulo> titulos = new List<Dominio.ObjetosDeValor.WebService.CTe.Titulo>();
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                titulos.Add(new Dominio.ObjetosDeValor.WebService.CTe.Titulo()
                {
                    CTe = ConverterObjetoCTe(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, unitOfWork, false),
                    DataPagamento = cte.Titulo?.DataProgramacaoPagamento?.ToString("dd/MM/yyyy") ?? "",
                    DataVencimento = cte.Titulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                    ValorTitulo = cte.Titulo?.ValorPago ?? 0,
                    StatusPagamento = cte.Titulo?.StatusTitulo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto
                });
            }

            return titulos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal ConverterObjetoCTeNormal(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal();

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;

            if (conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(conhecimento.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null && conhecimento.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, conhecimento.TomadorPagador.GrupoPessoas.Codigo);

            cte.GerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;

            cte.ProtocoloCTe = conhecimento.Codigo;
            cte.ChaveCTe = conhecimento.Chave;
            cte.DataEmissao = conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
            cte.DataEvento = conhecimento.DataRetornoSefaz.HasValue ? conhecimento.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            cte.MensagemSefaz = conhecimento.Status != "A" ? conhecimento.MensagemStatus?.MensagemDoErro ?? conhecimento.MensagemRetornoSefaz : string.Empty;
            cte.SituacaoCTe = conhecimento.DescricaoStatus;
            cte.ValorCTe = conhecimento.ValorAReceber;
            cte.NumeroCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;
            cte.Evento = "CT-e Normal";
            cte.NumeroBooking = conhecimento?.NumeroBooking ?? string.Empty;
            cte.NumeroContainer = conhecimento?.Container ?? string.Empty;
            cte.NumeroLacre = conhecimento?.LacreContainer ?? string.Empty;
            cte.NumeroCTe = conhecimento?.Numero ?? 0;
            cte.Serie = conhecimento?.Serie?.Numero ?? 0;
            cte.Peso = conhecimento?.Peso ?? 0;
            cte.CNPJEmitente = conhecimento?.Empresa?.CNPJ ?? string.Empty;
            cte.TipoModal = conhecimento.TipoModal;
            cte.RolagemCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.RolagemCarga ?? false;

            cte.Chave = conhecimento.Chave;
            cte.CFOP = conhecimento.CFOP.CodigoCFOP;
            cte.DataEmissao = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(conhecimento.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeInicioPrestacao);
            cte.Lotacao = conhecimento.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = conhecimento.ModeloDocumentoFiscal.Numero;
            cte.Numero = conhecimento.Numero;
            cte.NumeroControle = conhecimento.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(conhecimento, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = conhecimento.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Remetente);
            cte.Serie = conhecimento.Serie.Numero;
            cte.SituacaoCTeSefaz = conhecimento.SituacaoCTeSefaz;
            cte.TipoCTE = conhecimento.TipoCTE;
            cte.TipoServico = conhecimento.TipoServico;
            cte.TipoTomador = conhecimento.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Tomador);
            cte.Peso = conhecimento.Peso;
            cte.PesoCubado = conhecimento.PesoCubado;
            cte.PesoFaturado = conhecimento.PesoFaturado;

            cte.MunicipioColeta = conhecimento.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = conhecimento.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = conhecimento.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";


            cte.ItemServico = conhecimento.ItemServico;

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();


            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == conhecimento.Codigo select obj).ToList();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            decimal valor = conhecimento.ValorAReceber;
            if (conhecimento.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultado.PlanoContabilidade };
                if (conhecimento.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = conhecimento.ValorAReceber - conhecimento.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = conhecimento.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = conhecimento.ValorMaximoCentroContabilizacao;
                }
            }

            if (conhecimento.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = conhecimento.CentroResultadoEscrituracao.PlanoContabilidade };

            if (conhecimento.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultadoDestinatario.PlanoContabilidade };

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = conhecimento.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = conhecimento.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = conhecimento.CST == "91" ? "90" : conhecimento.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = conhecimento.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = conhecimento.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = conhecimento.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = conhecimento.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = conhecimento.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = conhecimento.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = conhecimento.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = conhecimento.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = conhecimento.ValorPrestacaoServico;

            cte.ValorTotalMercadoria = conhecimento.ValorTotalMercadoria;
            cte.VersaoCTE = conhecimento.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(conhecimento, unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "C", unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "A", unitOfWork) : "";
            cte.MotivoCancelamento = conhecimento.ObservacaoCancelamento;

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();
            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();
                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                }

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    cte.Documentos.Add(nota);
            }

            cte.ChaveCTeVinculado = conhecimento.ChaveCTESubComp;
            cte.ProtocoloCarga = repCargaCTe.BuscarPorCTe(conhecimento.Codigo)?.Carga?.Protocolo ?? 0;
            cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                foreach (var container in conhecimento.Containers)
                {
                    if (container.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cte.Containeres.Add(cont);
                        }
                    }
                }
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar ConverterObjetoCTeComplementar(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCteComplementar = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar();

            Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cteComplementar = repCteComplementar.BuscarPorCTeGerado(conhecimento.Codigo);

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;

            if (conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(conhecimento.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null && conhecimento.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, conhecimento.TomadorPagador.GrupoPessoas.Codigo);

            cte.GerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;

            cte.ProtocoloCTe = conhecimento.Codigo;
            cte.ChaveCTe = conhecimento.Chave;
            cte.DataEmissao = conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
            cte.DataEvento = conhecimento.DataRetornoSefaz.HasValue ? conhecimento.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            cte.MensagemSefaz = conhecimento.Status != "A" ? conhecimento.MensagemStatus?.MensagemDoErro ?? conhecimento.MensagemRetornoSefaz : string.Empty;
            cte.SituacaoCTe = conhecimento.DescricaoStatus;
            cte.ValorCTe = conhecimento.ValorAReceber;
            cte.NumeroCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;
            cte.Evento = "CT-e Complementar";
            cte.NumeroBooking = conhecimento?.NumeroBooking ?? string.Empty;
            cte.NumeroContainer = conhecimento?.Container ?? string.Empty;
            cte.NumeroLacre = conhecimento?.LacreContainer ?? string.Empty;
            cte.NumeroCTeOriginal = cteComplementar?.CTeOriginal?.Numero ?? 0;
            cte.SerieCTeOriginal = cteComplementar.CTeOriginal?.Serie?.Numero ?? 0;
            cte.NumeroCTeComplementar = cteComplementar.CTeGerado?.Numero ?? 0;
            cte.SerieCTeComplementar = cteComplementar.CTeGerado?.Numero ?? 0;
            cte.Peso = conhecimento?.Peso ?? 0;
            cte.CNPJEmitente = conhecimento.Empresa?.CNPJ ?? string.Empty;
            cte.TipoModal = conhecimento.TipoModal;
            cte.RolagemCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.RolagemCarga ?? false;

            cte.Chave = conhecimento.Chave;
            cte.CFOP = conhecimento.CFOP.CodigoCFOP;
            cte.DataEmissao = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(conhecimento.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeInicioPrestacao);
            cte.Lotacao = conhecimento.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = conhecimento.ModeloDocumentoFiscal.Numero;
            cte.Numero = conhecimento.Numero;
            cte.NumeroControle = conhecimento.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(conhecimento, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = conhecimento.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Remetente);
            cte.Serie = conhecimento.Serie.Numero;
            cte.SituacaoCTeSefaz = conhecimento.SituacaoCTeSefaz;
            cte.TipoCTE = conhecimento.TipoCTE;
            cte.TipoServico = conhecimento.TipoServico;
            cte.TipoTomador = conhecimento.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Tomador);
            cte.Peso = conhecimento.Peso;
            cte.PesoCubado = conhecimento.PesoCubado;
            cte.PesoFaturado = conhecimento.PesoFaturado;

            cte.MunicipioColeta = conhecimento.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = conhecimento.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = conhecimento.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";


            cte.ItemServico = conhecimento.ItemServico;

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();


            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == conhecimento.Codigo select obj).ToList();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            decimal valor = conhecimento.ValorAReceber;
            if (conhecimento.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultado.PlanoContabilidade };
                if (conhecimento.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = conhecimento.ValorAReceber - conhecimento.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = conhecimento.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = conhecimento.ValorMaximoCentroContabilizacao;
                }
            }

            if (conhecimento.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = conhecimento.CentroResultadoEscrituracao.PlanoContabilidade };

            if (conhecimento.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultadoDestinatario.PlanoContabilidade };

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = conhecimento.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = conhecimento.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = conhecimento.CST == "91" ? "90" : conhecimento.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = conhecimento.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = conhecimento.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = conhecimento.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = conhecimento.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = conhecimento.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = conhecimento.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = conhecimento.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = conhecimento.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = conhecimento.ValorPrestacaoServico;

            cte.ValorTotalMercadoria = conhecimento.ValorTotalMercadoria;
            cte.VersaoCTE = conhecimento.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(conhecimento, unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "C", unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "A", unitOfWork) : "";
            cte.MotivoCancelamento = conhecimento.ObservacaoCancelamento;

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();
            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();
                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                }

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    cte.Documentos.Add(nota);
            }

            cte.ChaveCTeVinculado = conhecimento.ChaveCTESubComp;
            cte.ProtocoloCarga = repCargaCTe.BuscarPorCTe(conhecimento.Codigo)?.Carga?.Protocolo ?? 0;
            cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                foreach (var container in conhecimento.Containers)
                {
                    if (container.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cte.Containeres.Add(cont);
                        }
                    }
                }
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento ConverterObjetoCTeCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento();

            cte.ProtocoloCTe = conhecimento.Codigo;
            cte.ChaveCTe = conhecimento.Chave;
            cte.DataEmissao = conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
            cte.DataEvento = conhecimento.DataRetornoSefaz.HasValue ? conhecimento.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            cte.MensagemSefaz = conhecimento.Status != "A" ? conhecimento.MensagemStatus?.MensagemDoErro ?? conhecimento.MensagemRetornoSefaz : string.Empty;
            cte.SituacaoCTe = conhecimento.DescricaoStatus;
            cte.ValorCTe = conhecimento.ValorAReceber;
            cte.NumeroCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;
            cte.Evento = "Cancelamento";
            cte.NumeroBooking = conhecimento.NumeroBooking ?? string.Empty;
            cte.NumeroContainer = conhecimento.Container ?? string.Empty;
            cte.NumeroLacre = conhecimento.LacreContainer ?? string.Empty;
            cte.NumeroCTe = conhecimento.Numero;
            cte.Serie = conhecimento.Serie.Numero;
            cte.Peso = conhecimento.Peso;
            cte.CNPJEmitente = conhecimento.Empresa?.CNPJ ?? string.Empty;
            cte.TipoModal = conhecimento.TipoModal;
            cte.RolagemCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.RolagemCarga ?? false;

            cte.Chave = conhecimento.Chave;
            cte.CFOP = conhecimento.CFOP.CodigoCFOP;
            cte.DataEmissao = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(conhecimento.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeInicioPrestacao);
            cte.Lotacao = conhecimento.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = conhecimento.ModeloDocumentoFiscal.Numero;
            cte.Numero = conhecimento.Numero;
            cte.NumeroControle = conhecimento.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(conhecimento, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = conhecimento.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Remetente);
            cte.Serie = conhecimento.Serie.Numero;
            cte.SituacaoCTeSefaz = conhecimento.SituacaoCTeSefaz;
            cte.TipoCTE = conhecimento.TipoCTE;
            cte.TipoServico = conhecimento.TipoServico;
            cte.TipoTomador = conhecimento.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Tomador);
            cte.Peso = conhecimento.Peso;
            cte.PesoCubado = conhecimento.PesoCubado;
            cte.PesoFaturado = conhecimento.PesoFaturado;

            cte.MunicipioColeta = conhecimento.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = conhecimento.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = conhecimento.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";


            cte.ItemServico = conhecimento.ItemServico;

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();


            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == conhecimento.Codigo select obj).ToList();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            decimal valor = conhecimento.ValorAReceber;
            if (conhecimento.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultado.PlanoContabilidade };
                if (conhecimento.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = conhecimento.ValorAReceber - conhecimento.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = conhecimento.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = conhecimento.ValorMaximoCentroContabilizacao;
                }
            }

            if (conhecimento.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = conhecimento.CentroResultadoEscrituracao.PlanoContabilidade };

            if (conhecimento.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultadoDestinatario.PlanoContabilidade };

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = conhecimento.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = conhecimento.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = conhecimento.CST == "91" ? "90" : conhecimento.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = conhecimento.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = conhecimento.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = conhecimento.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = conhecimento.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = conhecimento.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = conhecimento.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = conhecimento.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = conhecimento.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = conhecimento.ValorPrestacaoServico;

            cte.ValorTotalMercadoria = conhecimento.ValorTotalMercadoria;
            cte.VersaoCTE = conhecimento.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(conhecimento, unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "C", unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "A", unitOfWork) : "";
            cte.MotivoCancelamento = conhecimento.ObservacaoCancelamento;

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();
            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();
                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                }

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    cte.Documentos.Add(nota);
            }

            cte.ChaveCTeVinculado = conhecimento.ChaveCTESubComp;
            cte.ProtocoloCarga = repCargaCTe.BuscarPorCTe(conhecimento.Codigo)?.Carga?.Protocolo ?? 0;
            cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                foreach (var container in conhecimento.Containers)
                {
                    if (container.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cte.Containeres.Add(cont);
                        }
                    }
                }
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual ConverterObjetoCTeManual(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCteGerado = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);

            Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cteRelacao = repCteGerado.BuscarPorCTeOriginal(conhecimento.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual();

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;

            if (conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(conhecimento.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null && conhecimento.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, conhecimento.TomadorPagador.GrupoPessoas.Codigo);

            cte.GerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;

            cte.ProtocoloCTe = conhecimento.Codigo;
            cte.ChaveCTe = conhecimento.Chave;
            cte.DataEmissao = conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
            cte.DataEvento = conhecimento.DataRetornoSefaz.HasValue ? conhecimento.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            cte.MensagemSefaz = conhecimento.Status != "A" ? conhecimento.MensagemStatus?.MensagemDoErro ?? conhecimento.MensagemRetornoSefaz : string.Empty;
            cte.SituacaoCTe = conhecimento.DescricaoStatus;
            cte.ValorCTe = conhecimento.ValorAReceber;
            cte.NumeroCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;
            cte.NumeroBooking = conhecimento.NumeroBooking;
            cte.CNPJEmitente = conhecimento.Empresa?.CNPJ ?? string.Empty;
            cte.Evento = "Autorização";
            cte.TipoModal = conhecimento.TipoModal;
            cte.RolagemCarga = conhecimento.CargaCTes?.FirstOrDefault().Carga.RolagemCarga ?? false;

            if (cteRelacao != null)
                cte.Evento = cteRelacao.TipoCTeGerado == TipoCTeGerado.Substituicao ? "CT-e Substituto" : "CT-e Anulação";
            if (conhecimento.Status != "A")
                cte.Evento = "Cancelamento";

            if (cteRelacao?.TipoCTeGerado == TipoCTeGerado.Anulacao && cteRelacao?.CTeGerado != null)
            {
                cte.NumeroLacreAnulado = cteRelacao.CTeGerado.LacreContainer ?? string.Empty;
                cte.NumeroCTeAnulado = cteRelacao.CTeGerado.Numero;
                cte.SerieCTeAnulado = cteRelacao.CTeGerado.Serie.Numero;
                cte.PesoCTeAnulado = cteRelacao.CTeGerado.Peso;
                cte.NumeroConatainerCTeSubstituto = cteRelacao.CTeGerado.Container ?? string.Empty;
            }
            if (cteRelacao?.TipoCTeGerado == TipoCTeGerado.Substituicao && cteRelacao?.CTeGerado != null)
            {
                cte.NumeroLacreCTeSubstituto = cteRelacao.CTeGerado.LacreContainer ?? string.Empty;
                cte.NumeroCTeSubstituto = cteRelacao.CTeGerado.Numero;
                cte.SerieCTeSubstituto = cteRelacao.CTeGerado.Serie.Numero;
                cte.PesoCTeSubstituto = cteRelacao.CTeGerado.Peso;
                cte.NumeroConatainerCTeSubstituto = cteRelacao.CTeGerado.Container ?? string.Empty;
            }

            cte.Chave = conhecimento.Chave;
            cte.CFOP = conhecimento.CFOP.CodigoCFOP;
            cte.DataEmissao = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(conhecimento.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeInicioPrestacao);
            cte.Lotacao = conhecimento.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = conhecimento.ModeloDocumentoFiscal.Numero;
            cte.Numero = conhecimento.Numero;
            cte.NumeroControle = conhecimento.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(conhecimento, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = conhecimento.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Remetente);
            cte.Serie = conhecimento.Serie.Numero;
            cte.SituacaoCTeSefaz = conhecimento.SituacaoCTeSefaz;
            cte.TipoCTE = conhecimento.TipoCTE;
            cte.TipoServico = conhecimento.TipoServico;
            cte.TipoTomador = conhecimento.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Tomador);
            cte.Peso = conhecimento.Peso;
            cte.PesoCubado = conhecimento.PesoCubado;
            cte.PesoFaturado = conhecimento.PesoFaturado;

            cte.MunicipioColeta = conhecimento.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = conhecimento.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = conhecimento.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";


            cte.ItemServico = conhecimento.ItemServico;

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();


            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == conhecimento.Codigo select obj).ToList();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            decimal valor = conhecimento.ValorAReceber;
            if (conhecimento.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultado.PlanoContabilidade };
                if (conhecimento.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = conhecimento.ValorAReceber - conhecimento.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = conhecimento.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = conhecimento.ValorMaximoCentroContabilizacao;
                }
            }

            if (conhecimento.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = conhecimento.CentroResultadoEscrituracao.PlanoContabilidade };

            if (conhecimento.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultadoDestinatario.PlanoContabilidade };

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = conhecimento.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = conhecimento.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = conhecimento.CST == "91" ? "90" : conhecimento.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = conhecimento.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = conhecimento.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = conhecimento.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = conhecimento.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = conhecimento.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = conhecimento.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = conhecimento.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = conhecimento.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = conhecimento.ValorPrestacaoServico;

            cte.ValorTotalMercadoria = conhecimento.ValorTotalMercadoria;
            cte.VersaoCTE = conhecimento.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(conhecimento, unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "C", unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "A", unitOfWork) : "";
            cte.MotivoCancelamento = conhecimento.ObservacaoCancelamento;

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();
            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();
                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                }

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    cte.Documentos.Add(nota);
            }

            cte.ChaveCTeVinculado = conhecimento.ChaveCTESubComp;
            cte.ProtocoloCarga = repCargaCTe.BuscarPorCTe(conhecimento.Codigo)?.Carga?.Protocolo ?? 0;
            cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                foreach (var container in conhecimento.Containers)
                {
                    if (container.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cte.Containeres.Add(cont);
                        }
                    }
                }
            }

            return cte;
        }

        public com.maersk.BillableItemsPostRequest ConverterObjetoCTeAvroCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Repositorio.UnitOfWork unitOfWork, bool cteCancelado, bool cteComplementar, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia? cargaOcorrencia)
        {
            com.maersk.BillableItemsPostRequest cTeAvro = new com.maersk.BillableItemsPostRequest();
            com.maersk.billableitemspostrequest.BillableHeader billableHeader = new com.maersk.billableitemspostrequest.BillableHeader();
            //List<com.maersk.billableitemspostrequest.BillableItems> billableItems = new List<com.maersk.billableitemspostrequest.BillableItems>();

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(conhecimento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceita? tipoReceita = null;

            if (!cteComplementar)
                tipoReceita = cargaPedido?.Carga?.TipoOperacao?.ConfiguracaoEmissao?.TipoReceita;
            else if (cargaOcorrencia != null)
                tipoReceita = cargaOcorrencia?.TipoOcorrencia?.TipoReceita;

            if (conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(conhecimento.TomadorPagador.Cliente.CPF_CNPJ, 0);
            if (acordoFaturamento == null && conhecimento.TomadorPagador != null && conhecimento.TomadorPagador.Cliente != null && conhecimento.TomadorPagador.GrupoPessoas != null)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, conhecimento.TomadorPagador.GrupoPessoas.Codigo);

            var gerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;

            billableHeader.triggerType = DefinirDescricaoTriggerType(cteComplementar, cteCancelado, conhecimento.TipoCTE == TipoCTE.Substituto);
            cTeAvro.productSpecification = productSpecificationType.Generic;
            billableHeader.sourceSystem = "MTMS";
            billableHeader.messageCreationDatetime = FormatarCampoDataAtualNFTP();

            cTeAvro.billableHeader = billableHeader;
            cTeAvro.billableItems = PopularObjetoCTeCreation(conhecimento, gerarFaturamentoAVista, cargaPedido, unitOfWork, conhecimento.TipoCTE == TipoCTE.Substituto, cteCancelado, cteComplementar, tipoReceita, cargaOcorrencia);

            return cTeAvro;
        }

        public string FormatarCampoDataAtualNFTP()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                var brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
                return brasiliaTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
            catch (TimeZoneNotFoundException)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                var brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
                return brasiliaTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
        }

        public static class SourceSystemTransactionGenerator
        {
            public static string GerarIdentificador(int sequencia)
            {
                string dataFormatada = FormatarCampoNFTP();
                string sequenciaFormatada = sequencia.ToString("D7");
                string rawIdentifier = $"M{dataFormatada}{sequenciaFormatada}";
                return Regex.Replace(rawIdentifier, "[^a-zA-Z0-9]", "");
            }

            public static string FormatarCampoNFTP()
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    var brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
                    return brasiliaTime.ToString("ddMMyyyyHHmmss");
                }
                catch (TimeZoneNotFoundException)
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                    var brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
                    return brasiliaTime.ToString("ddMMyyyyHHmmss");
                }
            }
        }


        public anl.documentation.ctetransport.TransportDocumentationCTE ConverterObjetoCTeAvro(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.SeguroCTE repSeguroCTE = new Repositorio.SeguroCTE(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);

            anl.documentation.ctetransport.TransportDocumentationCTE cte = new anl.documentation.ctetransport.TransportDocumentationCTE();

            cte.statusCte = ConverterStatusCTE(conhecimento.Status);
            cte.bookingNumber = conhecimento.NumeroBooking;
            cte.internalProviderNumber = conhecimento.NumeroControle;
            cte.documentNumber = conhecimento.Numero.ToString();
            cte.cteSeriesNumber = conhecimento.Serie.Numero.ToString();
            cte.transportMode = ConverterTipoModal(conhecimento.TipoModal);
            cte.cteDocumentModel = conhecimento.ModeloDocumentoFiscal?.Numero ?? "";
            cte.issueDatetime = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.Ticks : 0;
            cte.cteType = ConverterTipoCTE(conhecimento.TipoCTE);
            cte.cteServiceType = ConverterTipoServico(conhecimento.TipoServico);
            cte.cteIndicatorGlobalized = conhecimento.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim;
            cte.cteKey = conhecimento.ChaveAcesso;
            cte.cteAuthProtocol = conhecimento.Protocolo;
            cte.effectiveFromDatetime = conhecimento.DataAutorizacao.HasValue ? conhecimento.DataAutorizacao.Value.Ticks : 0;

            cte.cteProvisionStart = new List<anl.documentation.ctetransport.cteProvisionStart>();
            anl.documentation.ctetransport.cteProvisionStart cteProvisionStart = new anl.documentation.ctetransport.cteProvisionStart()
            {
                cityCode = !string.IsNullOrWhiteSpace(conhecimento.LocalidadeInicioPrestacao.CodigoCidade) ? conhecimento.LocalidadeInicioPrestacao.CodigoCidade : "",
                cityName = conhecimento.LocalidadeInicioPrestacao.Descricao,
                subdivisionCode = conhecimento.LocalidadeInicioPrestacao.Estado.Sigla,
                subdivisionName = conhecimento.LocalidadeInicioPrestacao.Estado.Nome,
                subdivisionTypeCode = conhecimento.LocalidadeInicioPrestacao.Estado.CodigoEstado
            };
            cte.cteProvisionStart.Add(cteProvisionStart);

            cte.cteProvisionEnd = new List<anl.documentation.ctetransport.cteProvisionEnd>();
            anl.documentation.ctetransport.cteProvisionEnd cteProvisionEnd = new anl.documentation.ctetransport.cteProvisionEnd()
            {
                cityCode = !string.IsNullOrWhiteSpace(conhecimento.LocalidadeTerminoPrestacao.CodigoCidade) ? conhecimento.LocalidadeTerminoPrestacao.CodigoCidade : "",
                cityName = conhecimento.LocalidadeTerminoPrestacao.Descricao,
                subdivisionCode = conhecimento.LocalidadeTerminoPrestacao.Estado.Sigla,
                subdivisionName = conhecimento.LocalidadeTerminoPrestacao.Estado.Nome,
                subdivisionTypeCode = conhecimento.LocalidadeTerminoPrestacao.Estado.CodigoEstado,
            };
            cte.cteProvisionEnd.Add(cteProvisionEnd);

            cte.cteShipper = serWSPessoa.ConverterObjetoParticipantecteShipper(conhecimento.Remetente, Dominio.Enumeradores.TipoPessoaCTe.Remetente);
            cte.cteRecipient = serWSPessoa.ConverterObjetoParticipantecteRecipient(conhecimento.Destinatario, Dominio.Enumeradores.TipoPessoaCTe.Destinatario);
            cte.cteSender = serWSPessoa.ConverterObjetoParticipantecteSender(conhecimento.Expedidor, Dominio.Enumeradores.TipoPessoaCTe.Expedidor);
            cte.cteReceiver = serWSPessoa.ConverterObjetoParticipantecteReceiver(conhecimento.Recebedor, Dominio.Enumeradores.TipoPessoaCTe.Recebedor);

            cte.cteConsigneeType = ConverterTipoTomador(conhecimento.TipoTomador);
            cte.cteConsignee = serWSPessoa.ConverterObjetoParticipantecteConsignee(conhecimento.Tomador, null);

            cte.productName = conhecimento.ProdutoPredominante;
            cte.totalCargoValue = (double)conhecimento.ValorTotalMercadoria;

            List<Dominio.Entidades.InformacaoCargaCTE> informacaoCargaCTEs = repInformacaoCargaCTE.BuscarPorCTe(conhecimento.Codigo);
            List<anl.documentation.ctetransport.cteWeights> cteWeights = new List<anl.documentation.ctetransport.cteWeights>();
            if (informacaoCargaCTEs != null && informacaoCargaCTEs.Count > 0)
            {
                foreach (Dominio.Entidades.InformacaoCargaCTE informacaoCargaCTe in informacaoCargaCTEs)
                {
                    anl.documentation.ctetransport.cteWeights weight = new anl.documentation.ctetransport.cteWeights();

                    weight.itemQuantity = (long)informacaoCargaCTe.Quantidade;
                    weight.itemQuantityUnit = ConverterUnidadeMedia(informacaoCargaCTe.UnidadeMedida);
                    weight.totalNetWeight = (double)conhecimento.PesoLiquido;
                    weight.grossWeight = (double)conhecimento.PesoFaturado;

                    cteWeights.Add(weight);
                }
            }
            cte.cteWeights = cteWeights;

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentes = repComponenteCTe.BuscarPorCTe(conhecimento.Codigo);
            anl.documentation.ctetransport.cteValues values = new anl.documentation.ctetransport.cteValues();
            if (componentes != null && componentes.Count > 0)
            {
                values.freightCost = (double)conhecimento.ValorFrete;
                values.totalFreightCost = (double)conhecimento.ValorPrestacaoServico;
                values.unpaidAmount = (double)conhecimento.ValorAReceber;

                //List<anl.documentation.ctetransport.cabotService> cabotServices = new List<anl.documentation.ctetransport.cabotService>();
                anl.documentation.ctetransport.cabotService cabotServices = new anl.documentation.ctetransport.cabotService();
                foreach (Dominio.Entidades.ComponentePrestacaoCTE componente in componentes)
                {
                    cabotServices = new anl.documentation.ctetransport.cabotService()
                    {
                        cabotServiceType = componente.Nome,
                        cabotServiceValue = (double)componente.Valor
                    };
                }
                values.cabotService = cabotServices;
            }

            cte.cteValues = new List<anl.documentation.ctetransport.cteValues>();
            cte.cteValues.Add(values);

            cte.cteTaxes = new List<anl.documentation.ctetransport.cteTaxes>();
            anl.documentation.ctetransport.cteTaxes cteTaxes = new anl.documentation.ctetransport.cteTaxes()
            {
                cteCfop = conhecimento.CFOP.CFOPComExtensao,
                taxIdentificationNumberTypeCode = conhecimento.CST,
                totalTaxAmount = (double)conhecimento.BaseCalculoICMS,
                taxRate = (double?)conhecimento.AliquotaICMS,
                taxAmount = (double)conhecimento.ValorICMS,
                taxRateRedBC = (double)conhecimento.PercentualReducaoBaseCalculoICMS,
                taxIcmsST = conhecimento.CST == "60" ? (double)conhecimento.BaseCalculoICMS : 0,
            };
            cte.cteTaxes.Add(cteTaxes);

            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                cte.cteLinkedDocuments = new List<anl.documentation.ctetransport.cteLinkedDocuments>();
                foreach (var container in conhecimento.Containers)
                {
                    foreach (var nota in container.Documentos)
                    {
                        anl.documentation.ctetransport.cteLinkedDocuments linkedDocument = new anl.documentation.ctetransport.cteLinkedDocuments();

                        anl.documentation.ctetransport.docLinked docsLinked = new anl.documentation.ctetransport.docLinked()
                        {
                            docLinkedType = nota.TipoDocumento == Dominio.Enumeradores.TipoDocumentoCTe.NFe ? "NF-e" : "NF",
                            docLinkedKey = nota.Chave,
                            docLinkedNumber = nota.Numero
                        };
                        linkedDocument.docLinked = docsLinked;
                        linkedDocument.containerNumber = container.Container?.Numero ?? "";
                        linkedDocument.isoContainerSizeTypeName = container?.Container?.ContainerTipo?.Descricao ?? "";
                        linkedDocument.sealNumber = container?.Lacre1 ?? "";

                        cte.cteLinkedDocuments.Add(linkedDocument);
                    }
                }
            }
            else
            {

                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
                anl.documentation.ctetransport.cteLinkedDocuments linkedDocument = new anl.documentation.ctetransport.cteLinkedDocuments();
                if (documentosCTe != null && documentosCTe.Count > 0)
                {
                    cte.cteLinkedDocuments = new List<anl.documentation.ctetransport.cteLinkedDocuments>();

                    foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
                    {
                        anl.documentation.ctetransport.docLinked docsLinked = new anl.documentation.ctetransport.docLinked()
                        {
                            docLinkedType = documentoCTe.ModeloDocumentoFiscal?.Abreviacao ?? "",
                            docLinkedKey = documentoCTe.ChaveNFE,
                            docLinkedNumber = documentoCTe.Numero
                        };
                        linkedDocument.docLinked = docsLinked;
                        linkedDocument.containerNumber = conhecimento.Containers.FirstOrDefault()?.Numero ?? "";
                        linkedDocument.isoContainerSizeTypeName = conhecimento.Containers.FirstOrDefault()?.Container?.ContainerTipo?.Descricao ?? "";
                        linkedDocument.sealNumber = conhecimento.Containers.FirstOrDefault()?.Lacre1 ?? "";

                        cte.cteLinkedDocuments.Add(linkedDocument);
                    }
                }
            }
            cte.ctePortStart = new List<anl.documentation.ctetransport.ctePortStart>();
            anl.documentation.ctetransport.ctePortStart ctePortStart = new anl.documentation.ctetransport.ctePortStart()
            {
                portCode = conhecimento.PortoOrigem?.CodigoIntegracao ?? "",
                portName = conhecimento.PortoOrigem?.Descricao ?? ""
            };
            cte.ctePortStart.Add(ctePortStart);

            cte.ctePortTranshipment = new List<anl.documentation.ctetransport.ctePortTranshipment>();
            anl.documentation.ctetransport.ctePortTranshipment ctePortTranshipment = new anl.documentation.ctetransport.ctePortTranshipment()
            {
                portCode = conhecimento.PortoPassagemUm?.CodigoIntegracao ?? "",
                portName = conhecimento.PortoPassagemUm?.Descricao ?? ""
            };
            cte.ctePortTranshipment.Add(ctePortTranshipment);

            cte.ctePortEnd = new List<anl.documentation.ctetransport.ctePortEnd>();
            anl.documentation.ctetransport.ctePortEnd ctePortEnd = new anl.documentation.ctetransport.ctePortEnd()
            {
                portCode = conhecimento.PortoDestino?.CodigoIntegracao ?? "",
                portName = conhecimento.PortoDestino?.Descricao ?? ""
            };
            cte.ctePortEnd.Add(ctePortEnd);

            cte.text = conhecimento.ObservacoesGerais;

            cte.cteTransportSpecification = new List<anl.documentation.ctetransport.cteTransportSpecification>();
            anl.documentation.ctetransport.cteTransportSpecification cteTransportSpecification = new anl.documentation.ctetransport.cteTransportSpecification()
            {
                cteMTLCertificate = conhecimento.Empresa?.COTM ?? "",
                cteNegotiable = conhecimento.IndicadorNegociavel == Dominio.Enumeradores.OpcaoSimNao.Sim,
                valueAFRMM = (double?)conhecimento.ValorPrestacaoAFRMM
            };
            cte.cteTransportSpecification.Add(cteTransportSpecification);

            cte.cteVessel = new List<anl.documentation.ctetransport.cteVessel>();
            anl.documentation.ctetransport.cteVessel cteVessel = new anl.documentation.ctetransport.cteVessel()
            {
                vesselName = conhecimento.Navio?.Descricao ?? conhecimento.Viagem?.Navio?.Descricao ?? "",
                voyageNumber = conhecimento.Viagem?.NumeroViagem.ToString() ?? "",
                direction = ConverterDirecao(conhecimento.Viagem?.DirecaoViagemMultimodal) ?? anl.documentation.ctetransport.directionType.NORTH
            };
            cte.cteVessel.Add(cteVessel);

            List<Dominio.Entidades.SeguroCTE> seguros = repSeguroCTE.BuscarPorCTe(conhecimento.Codigo);
            List<anl.documentation.ctetransport.cteInsurance> cteInsurances = new List<anl.documentation.ctetransport.cteInsurance>();
            if (seguros != null && seguros.Count > 0)
            {
                foreach (Dominio.Entidades.SeguroCTE seguro in seguros)
                {
                    anl.documentation.ctetransport.cteInsurance insurance = new anl.documentation.ctetransport.cteInsurance();

                    insurance.insurerTax = seguro.CNPJSeguradora;
                    insurance.insurerName = seguro.NomeSeguradora;
                    insurance.insurerPolicyNumber = seguro.NumeroApolice;
                    insurance.insurerAverbationNumber = seguro.NumeroAverbacao;

                    cteInsurances.Add(insurance);
                }
            }
            cte.cteInsurance = cteInsurances;


            string xml = ObterRetornoXML(conhecimento, unitOfWork);

            cte.cteText = Utilidades.XML.ObterConteudoTag(xml, "infAdFisco");
            if (string.IsNullOrWhiteSpace(cte.cteText))
                cte.cteText = "";

            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.CTe.CTe ConverterObjetoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.WebService.CTe.CTe cte = new Dominio.ObjetosDeValor.WebService.CTe.CTe();
            cte.Chave = conhecimento.Chave;
            cte.CFOP = conhecimento.CFOP.CodigoCFOP;
            cte.DataEmissao = conhecimento.DataEmissao.HasValue ? conhecimento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Destinatario = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Destinatario);
            cte.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(conhecimento.Empresa);
            cte.Expedidor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Expedidor);
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(conhecimento.LocalidadeInicioPrestacao);
            cte.Lotacao = conhecimento.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.Modelo = conhecimento.ModeloDocumentoFiscal.Numero;
            cte.Numero = conhecimento.Numero;
            cte.NumeroControle = conhecimento.NumeroControle;
            cte.PDF = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(conhecimento, codificarUTF8, unitOfWork) : "";
            cte.Protocolo = conhecimento.Codigo;
            cte.Recebedor = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Recebedor);
            cte.Remetente = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Remetente);
            cte.Serie = conhecimento.Serie.Numero;
            cte.SituacaoCTeSefaz = conhecimento.SituacaoCTeSefaz;
            cte.TipoCTE = conhecimento.TipoCTE;
            cte.TipoServico = conhecimento.TipoServico;
            cte.TipoTomador = conhecimento.TipoTomador;
            cte.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(conhecimento.Tomador);
            cte.Peso = conhecimento.Peso;
            cte.PesoCubado = conhecimento.PesoCubado;
            cte.PesoFaturado = conhecimento.PesoFaturado;

            cte.MunicipioColeta = conhecimento.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = conhecimento.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = conhecimento.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";


            cte.ItemServico = conhecimento.ItemServico;

            cte.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();


            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == conhecimento.Codigo select obj).ToList();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                cte.ContasContabeis.Add(contaContabil);
            }

            decimal valor = conhecimento.ValorAReceber;
            if (conhecimento.CentroResultado != null)
            {
                cte.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultado.PlanoContabilidade };
                if (conhecimento.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = conhecimento.ValorAReceber - conhecimento.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        cte.CentroResultado.Valor = conhecimento.ValorAReceber;
                        valor = 0;
                    }
                    else
                        cte.CentroResultado.Valor = conhecimento.ValorMaximoCentroContabilizacao;
                }
            }

            if (conhecimento.CentroResultadoEscrituracao != null)
                cte.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = conhecimento.CentroResultadoEscrituracao.PlanoContabilidade };

            if (conhecimento.CentroResultadoDestinatario != null)
                cte.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = conhecimento.CentroResultadoDestinatario.PlanoContabilidade };

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.FreteProprio = conhecimento.ValorFrete;
            cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cte.ValorFrete.ICMS.Aliquota = conhecimento.AliquotaICMS;

            cte.ValorFrete.ICMS.CST = conhecimento.CST == "91" ? "90" : conhecimento.CST;
            cte.ValorFrete.ICMS.IncluirICMSBC = conhecimento.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ObservacaoCTe = conhecimento.ObservacoesGerais;
            cte.ValorFrete.ICMS.PercentualInclusaoBC = conhecimento.PercentualICMSIncluirNoFrete; cte.ValorFrete.ICMS.PercentualReducaoBC = conhecimento.PercentualReducaoBaseCalculoICMS;
            cte.ValorFrete.ICMS.SimplesNacional = conhecimento.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cte.ValorFrete.ICMS.ValorBaseCalculoICMS = conhecimento.BaseCalculoICMS;
            cte.ValorFrete.ICMS.ValorICMS = conhecimento.ValorICMS;
            cte.ValorFrete.ValorTotalAReceber = conhecimento.ValorAReceber;
            cte.ValorFrete.ValorPrestacaoServico = conhecimento.ValorPrestacaoServico;

            cte.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS();
            cte.ValorFrete.IBSCBS.CST = conhecimento.CSTIBSCBS;
            cte.ValorFrete.IBSCBS.ClassificacaoTributaria = conhecimento.ClassificacaoTributariaIBSCBS;
            cte.ValorFrete.IBSCBS.BaseCalculo = conhecimento.BaseCalculoIBSCBS;
            cte.ValorFrete.IBSCBS.AliquotaIBSEstadual = conhecimento.AliquotaIBSEstadual;
            cte.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual = conhecimento.PercentualReducaoIBSEstadual;
            cte.ValorFrete.IBSCBS.ValorIBSEstadual = conhecimento.ValorIBSEstadual;
            cte.ValorFrete.IBSCBS.AliquotaIBSMunicipal = conhecimento.AliquotaIBSMunicipal;
            cte.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal = conhecimento.PercentualReducaoIBSMunicipal;
            cte.ValorFrete.IBSCBS.ValorIBSMunicipal = conhecimento.ValorIBSMunicipal;
            cte.ValorFrete.IBSCBS.AliquotaCBS = conhecimento.AliquotaCBS;
            cte.ValorFrete.IBSCBS.PercentualReducaoCBS = conhecimento.PercentualReducaoCBS;
            cte.ValorFrete.IBSCBS.ValorCBS = conhecimento.ValorCBS;

            if (conhecimento.ValorTotalDocumentoFiscal > 0)
                cte.ValorFrete.ValorTotalDocumentoFiscal = conhecimento.ValorTotalDocumentoFiscal;
            else if (conhecimento.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = conhecimento.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(conhecimento?.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCBS?.SomarImpostosDocumento ?? false) || (conhecimento?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + conhecimento.ValorIBSMunicipal + conhecimento.ValorIBSEstadual + conhecimento.ValorCBS;

                cte.ValorFrete.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }


            cte.ValorTotalMercadoria = conhecimento.ValorTotalMercadoria;
            cte.VersaoCTE = conhecimento.Versao;
            cte.XML = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXML(conhecimento, unitOfWork) : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "C", unitOfWork) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(conhecimento, "A", unitOfWork) : "";
            cte.MotivoCancelamento = conhecimento.ObservacaoCancelamento;
            cte.ProtocolosDePedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarProtocoloPedidosPorCTe(conhecimento.Codigo);

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(conhecimento.Codigo);
            cte.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();
            List<int> codigosCargasPedidoCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCodigosCargaPedidoPorCargaCTe(conhecimento.Codigo);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFIscal = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (codigosCargasPedidoCTe.Any())
                pedidosXMLNotaFIscal = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidoComFetch(codigosCargasPedidoCTe);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repCTe.BuscarDadosProdutoEmbarcador(conhecimento.Codigo);

            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = null;
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();

                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                {
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                    nota.Numero = documentoCTe.Numero;
                    nota.Serie = documentoCTe.Serie;

                    if (cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Exists(c => c.ChaveNFe == nota.ChaveNFe))
                        continue;

                    var objNotaFiscal = pedidosXMLNotaFIscal.Find(pedidoXMLNotaFiscal => pedidoXMLNotaFiscal.XMLNotaFiscal.Chave == documentoCTe.ChaveNFE);

                    pedido = objNotaFiscal?.CargaPedido?.Pedido;
                    xmlNota = objNotaFiscal?.XMLNotaFiscal;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                    if (!string.IsNullOrWhiteSpace(nota.Serie) && !string.IsNullOrWhiteSpace(nota.Numero) && cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Any(c => c.Numero == nota.Numero && c.Serie == nota.Serie))
                        continue;
                    if (!string.IsNullOrWhiteSpace(documentoCTe.Descricao))
                    {
                        nota.Descricao = documentoCTe.Descricao;
                        if (cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Exists(c => c.Descricao == nota.Descricao))
                            continue;
                    }
                }

                if (xmlNota != null)
                {
                    nota.Volume = xmlNota.Volumes;
                    nota.PINSuframa = xmlNota.PINSUFRAMA;
                    nota.NCMPredominante = xmlNota.NCM;
                    nota.NumeroReferenciaEDI = xmlNota.NumeroReferenciaEDI;
                    nota.NumeroControleCliente = xmlNota.NumeroControleCliente;
                    if (!string.IsNullOrWhiteSpace(xmlNota.NumeroCanhoto))
                        nota.NumeroCanhoto = xmlNota.NumeroCanhoto;
                    else if (xmlNota.Numero > 0 && xmlNota.Emitente != null)
                        nota.NumeroCanhoto = xmlNota.Numero.ToString("D").PadLeft(9, '0') + xmlNota.SerieOuSerieDaChave.PadLeft(3, '0') + xmlNota.Emitente.CPF_CNPJ.ToString().PadLeft(14, '0');

                }

                if (string.IsNullOrWhiteSpace(nota.NCMPredominante) && produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM) && produtoEmbarcador.CodigoNCM.Length == 4)
                    nota.NCMPredominante = produtoEmbarcador.CodigoNCM;
                nota.CodigoProduto = produtoEmbarcador != null ? produtoEmbarcador.CodigoDocumentacao : "";

                if (!string.IsNullOrWhiteSpace(pedido?.NumeroPedidoEmbarcador ?? ""))
                    nota.NumeroPedidoEmbarcador = pedido?.NumeroPedidoEmbarcador;

                if (!string.IsNullOrWhiteSpace(pedido?.Adicional1 ?? ""))
                    nota.ProcImportacao = pedido?.Adicional1;

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    cte.Documentos.Add(nota);
            }

            cte.ChaveCTeVinculado = conhecimento.ChaveCTESubComp;
            cte.ProtocoloCarga = repCargaCTe.BuscarProtocoloCargaPorCTe(conhecimento.Codigo);
            cte.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (conhecimento.Containers != null && conhecimento.Containers.Count > 0)
            {
                foreach (var container in conhecimento.Containers)
                {
                    if (container.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cte.Containeres.Add(cont);
                        }
                    }
                }
            }

            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RealizarAnulacaoGerencial(RequestAnulacaoGerencial requestAnulacaoGerencial)
        {

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            var configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarCargaChaveCTeEProtocoloCarga(requestAnulacaoGerencial.ProtocoloCarga, requestAnulacaoGerencial.ChaveCte);

            if (cargaCTeOriginal == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("CT-e não encontrado.");

            if (cargaCTeOriginal.CTe.Status != "A")
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A situação do CT-e ({cargaCTeOriginal.CTe.DescricaoStatus}) não permite a emissão de CT-e de anulação gerencial.");

            if (cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Encerrada)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A situação da carga ({cargaCTeOriginal.Carga.SituacaoCarga.ObterDescricao()}) não permite a emissão de CT-e de anulação gerencial.");

            if (cargaCTeOriginal.Carga.CargaTransbordo)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.");

            if (!Servicos.Embarcador.CTe.CTe.VerificarSeCTeEstaAptoParaCancelamento(out string mensagemErro, cargaCTeOriginal, _unitOfWork, true, true))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);

            _unitOfWork.Start();

            cargaCTeOriginal.CTe.AnuladoGerencialmente = true;
            cargaCTeOriginal.CTe.Status = "Z";
            cargaCTeOriginal.CTe.DataRetornoSefaz = DateTime.Now;
            cargaCTeOriginal.CTe.DataAnulacao = DateTime.Now;
            cargaCTeOriginal.CTe.ObservacaoCancelamento = requestAnulacaoGerencial.Motivo;

            repCTe.Atualizar(cargaCTeOriginal.CTe);

            if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out string erro, cargaCTeOriginal, _tipoServicoMultisoftware, _unitOfWork, "", false, true))
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(erro);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível gerar a movimentação de anulação do CT-e.");
            }

            if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cargaCTeOriginal, _tipoServicoMultisoftware, _unitOfWork))
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(erro);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível reverter os itens em aberto na anulação do CT-e");
            }

            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeOriginal, null, "CT-e anulado gerencialmente pela tela de CT-e Manual", _unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeOriginal.CTe, null, "CT-e anulado gerencialmente pela tela de CT-e Manual", _unitOfWork);

            if (configuracao.DeixarCargaPendenteDeIntegracaoAposCTeManual)
            {
                cargaCTeOriginal.Carga.CargaIntegradaEmbarcador = false;
                repCarga.Atualizar(cargaCTeOriginal.Carga);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeOriginal.Carga, null, "Alterou a carga para pendente de integração devido a anulação gerencial.", _unitOfWork);
            }

            _unitOfWork.CommitChanges();

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCTe(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, int protocoloCarga)
        {
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(_unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPrimeiro();

            if (protocoloCarga <= 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Protocolo da carga não localizado.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloCarga);

            if (carga == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Carga não localizada para o protocolo informado.");

            if (string.IsNullOrWhiteSpace(cte.Xml))
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e {cte.Numero} não possui um XML.", false);
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(cte.Chave);
            if (conhecimento == null)
            {
                System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.Xml);

                object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, carga.Empresa.Codigo, string.Empty, string.Empty, _unitOfWork, null, true, false, _tipoServicoMultisoftware, true, null, conhecimento?.NumeroControle ?? "");

                if (retornoInserir.GetType() == typeof(string))
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos((string)retornoInserir, false);
                }

                conhecimento = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;
            }

            if (conhecimento == null)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos($"CT-e não importado.", false);
            }

            cte.Codigo = conhecimento.Codigo;
            if (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Anulado || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.AnuladoGerencialmente || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Inutilizada)
            {
                conhecimento.Cancelado = "S";
                conhecimento.Status = cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada ? "C" : cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Anulado || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.AnuladoGerencialmente ? "Z" : "I";
                conhecimento.ProtocoloCancelamentoInutilizacao = cte.ProtocoloCancelamentoInutilizacao;
                conhecimento.Log += string.Concat(" / CT-e de cancelamento importado com sucesso em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                conhecimento.DataRetornoSefaz = cte.DataCancelamento;
                conhecimento.DataCancelamento = cte.DataCancelamento;
                conhecimento.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(cte.MensagemRetornoSefaz);

                repCTe.Atualizar(conhecimento);
            }
            else if (!serCTe.EmitirCTeManualmente(cte, carga.Codigo, usuario, _auditado, configuracaoTMS, out string msgRetorno, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, true))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(msgRetorno);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Sucesso");
        }

        public string ObterRetornoPDF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool codificarUTF8, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
            {
                if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                {
                    byte[] arquivo = new Servicos.Embarcador.Relatorios.OutrosDocumentos(unitOfWork).ObterPdf(cte);

                    if (codificarUTF8)
                        return Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                    else
                        return Convert.ToBase64String(arquivo);
                }
                else
                    return string.Empty;
            }
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                bool buscarSomentePorNumero = false;
                string nomeArquivo = (!buscarSomentePorNumero ? (cte.Codigo.ToString() + "_") : "") + cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatorios, "NFSe", cte.Empresa.CNPJ, nomeArquivo);

                byte[] danfse = null;
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                else
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                    danfse = svcNFSe.ObterDANFSECTe(cte.Codigo, unitOfWork, buscarSomentePorNumero);
                }
                if (danfse != null)
                {
                    if (codificarUTF8)
                        return Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, danfse));
                    else
                        return Convert.ToBase64String(danfse);
                }
                else
                    return string.Empty;
            }
            else if (cte.Status.Equals("A") || cte.Status.Equals("F"))
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                return servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, unitOfWork, codificarUTF8);
            }
            else
            {
                return string.Empty;
            }
        }

        public string ObterRetornoXMLPorStatus(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string status, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeDeTrabalho);

            Dominio.Entidades.XMLCTe xml = null;

            if (status == "A")
                xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
            else if (status == "C" || status == "I")
                xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);

            if (xml != null)
            {
                if (!xml.XMLArmazenadoEmArquivo)
                    return xml.XML ?? string.Empty;
                else
                {
                    Servicos.CTe serCTe = new Servicos.CTe(unidadeDeTrabalho);

                    string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(cte, status, unidadeDeTrabalho);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        return Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);
                    else
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarDadosDoMercante(Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(dadosDoMercante.ChaveCte);

            if (cte == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi encontrado nenhum CT-e com a chave: {dadosDoMercante.ChaveCte}");

            if (!string.IsNullOrEmpty(dadosDoMercante.NumeroManifestoTransbordo))
                cte.NumeroManifestoTransbordo = dadosDoMercante.NumeroManifestoTransbordo;

            if (!string.IsNullOrEmpty(dadosDoMercante.NumeroManifesto))
                cte.NumeroManifesto = dadosDoMercante.NumeroManifesto;

            if (!string.IsNullOrEmpty(dadosDoMercante.NumeroCe))
                cte.NumeroCEMercante = dadosDoMercante.NumeroCe;

            repCTe.Atualizar(cte, _auditado);

            serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cte.Codigo, _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Dados Atualizado com Sucesso!");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarPrevisaoPagamentoCTe(List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> informarPrevisaoPagamentoCTe)
        {
            return InformarPrevisaoPagamentoCTe(informarPrevisaoPagamentoCTe, out _);
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>> InformarPrevisoesPagamentosCTe(List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> informarPrevisoesPagamentosCTe)
        {
            InformarPrevisaoPagamentoCTe(informarPrevisoesPagamentosCTe, out List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> retorno);
            return Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarBloqueioDocumento(int protocoloCTe, string dataBloqueio, string observacao)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(protocoloCTe);

                if (cte == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi encontrado nenhum CT-e com o protocolo: {protocoloCTe}");

                if (cte.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e informado não está autorizado no Sefaz");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cte.Titulo ?? new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                if (titulo.StatusTitulo == StatusTitulo.Quitada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não é possível provisionar um título já quitado");

                DateTime dataPagto;
                if (!DateTime.TryParseExact(dataBloqueio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagto))
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de previsão de pagamento não esta em um formato correto (dd/MM/yyyy)");

                _unitOfWork.Start();

                titulo.DataVencimento = dataPagto;
                titulo.DataProgramacaoPagamento = dataPagto;
                titulo.StatusTitulo = StatusTitulo.Bloqueado;
                titulo.Empresa = cte.Empresa;
                titulo.Observacao = observacao;
                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));
                titulo.Sequencia = 1;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                titulo.ValorOriginal = cte.ValorAReceber;
                titulo.ValorPendente = cte.ValorAReceber;
                titulo.IntegradoERP = false;

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = cte.TipoAmbiente;

                if (titulo.Codigo == 0)
                {
                    titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;
                    repTitulo.Inserir(titulo);
                    cte.Titulo = titulo;
                    repCTe.Atualizar(cte);
                }
                else
                    repTitulo.Atualizar(titulo);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Informou data de bloqueio do CT-e!");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao tentar provisionar o pagamento do CT-e.");
            }

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarDesbloqueioDocumento(Dominio.ObjetosDeValor.WebService.CTe.InformarDesbloqueioDocumento informarDesbloqueioDocumento)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(informarDesbloqueioDocumento.ProtocoloCTe);

                if (cte == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi encontrado nenhum CT-e com o protocolo: {informarDesbloqueioDocumento.ProtocoloCTe}");

                if (cte.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e informado não está autorizado no Sefaz");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cte.Titulo ?? new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorTitulo(titulo.Codigo);

                if (documentoFaturamento == null)
                {
                    documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCTe(cte.Codigo);
                }

                if (titulo.StatusTitulo == StatusTitulo.Quitada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não é possível provisionar um título já quitado");

                DateTime dataPagto;
                if (!DateTime.TryParseExact(informarDesbloqueioDocumento.DataDesbloqueio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagto))
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de previsão de pagamento não esta em um formato correto (dd/MM/yyyy)");

                _unitOfWork.Start();

                if (configuracaoFinanceiro.NaoPermitirGerarLotesPagamentosDocumentosBloqueados && documentoFaturamento != null)
                {
                    documentoFaturamento.PagamentoDocumentoBloqueado = false;
                    documentoFaturamento.DataLiberacaoPagamento = DateTime.Now;

                    repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);
                }

                titulo.DataVencimento = dataPagto;
                titulo.DataProgramacaoPagamento = dataPagto;
                titulo.StatusTitulo = StatusTitulo.EmAberto;
                titulo.Empresa = cte.Empresa;
                titulo.Observacao = informarDesbloqueioDocumento.Observacao;
                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));
                titulo.Sequencia = 1;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                titulo.ValorOriginal = cte.ValorAReceber;
                titulo.ValorPendente = cte.ValorAReceber;
                titulo.IntegradoERP = false;

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = cte.TipoAmbiente;

                if (titulo.Codigo == 0)
                {
                    titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;
                    repTitulo.Inserir(titulo);
                    cte.Titulo = titulo;
                    repCTe.Atualizar(cte);
                }
                else
                    repTitulo.Atualizar(titulo);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Informou data de desbloqueio do CT-e!");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao tentar provisionar o pagamento do CT-e.");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarPagamentoCTe(Dominio.ObjetosDeValor.WebService.CTe.ConfirmarPagamentoCTe confirmarPagamentoCTe)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(confirmarPagamentoCTe.ProtocoloCTe);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                if (cte == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi encontrado nenhum CT-e com o protocolo: {confirmarPagamentoCTe.ProtocoloCTe}");

                if (cte.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e informado não está autorizado no Sefaz");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cte.Titulo ?? new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                if (titulo.StatusTitulo == StatusTitulo.Quitada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não é possível provisionar um título já quitado");

                bool permitirRemoverDataPrevisaoPagamento = configuracaoWebService?.PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe ?? false;
                bool permitirPreencherSomenteDataPagamento = configuracaoWebService?.DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe ?? false;

                DateTime? dataPagto = null;
                if (!string.IsNullOrWhiteSpace(confirmarPagamentoCTe.DataPagamento))
                {
                    if (!DateTime.TryParseExact(confirmarPagamentoCTe.DataPagamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime tempDataPagto))
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de previsão de pagamento não está em um formato correto (dd/MM/yyyy)");

                    dataPagto = tempDataPagto;
                }
                else if (permitirRemoverDataPrevisaoPagamento)
                {
                    dataPagto = null;
                }
                else
                {
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("É necessário informar uma data de previsão de pagamento.");
                }

                Servicos.Log.GravarInfo($"Confirmando pagamento - protocolo " + confirmarPagamentoCTe.ProtocoloCTe + " com data pagto " + dataPagto.ToString() + " - status da configuração: " + permitirPreencherSomenteDataPagamento + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "WebServiceCTes");

                _unitOfWork.Start();

                titulo.Initialize();

                if (!permitirPreencherSomenteDataPagamento)
                    titulo.DataVencimento = dataPagto;

                titulo.DataLiquidacao = dataPagto;
                titulo.DataBaseLiquidacao = dataPagto;
                titulo.Observacao = confirmarPagamentoCTe.Observacao;
                titulo.NumeroPagamento = confirmarPagamentoCTe.NumeroPagamento;
                titulo.DataAlteracao = DateTime.Now;
                titulo.IntegradoERP = false;
                titulo.Valor = confirmarPagamentoCTe.ValorParcelaPaga;
                titulo.SequenciaPaga = confirmarPagamentoCTe.SequenciaParcelaPaga;

                int quantidadeParcelas = repositorioCargaCTeComplementoInfo.BuscarQuantidadeParcelaPorCTe(cte.Codigo);

                if (quantidadeParcelas == 0 && confirmarPagamentoCTe.SequenciaParcelaPaga > 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Documento não possui parcelas");

                if (quantidadeParcelas > 0 && quantidadeParcelas != confirmarPagamentoCTe.SequenciaParcelaPaga)
                {
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.ValorPago += confirmarPagamentoCTe.ValorParcelaPaga;
                    titulo.ValorPendente -= confirmarPagamentoCTe.ValorParcelaPaga;
                }
                else
                {
                    titulo.ValorPago = titulo.ValorOriginal;
                    titulo.ValorPendente = 0;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                }

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = cte.TipoAmbiente;

                if (titulo.Codigo == 0)
                {
                    titulo.DataVencimento = permitirPreencherSomenteDataPagamento ? null : dataPagto;
                    titulo.DataProgramacaoPagamento = dataPagto;
                    titulo.Empresa = cte.Empresa;
                    titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));
                    titulo.Sequencia = 1;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.ValorOriginal = cte.ValorAReceber;
                    titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;

                    repTitulo.Inserir(titulo, _auditado, null, "Adicionado pelo método ConfirmarPagamentoCTe");

                    cte.Titulo = titulo;
                    repCTe.Atualizar(cte);
                }
                else
                    repTitulo.Atualizar(titulo, _auditado, null, "Atualizado pelo método ConfirmarPagamentoCTe");

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Confirmou o pagamento do CT-e");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao tentar provisionar o pagamento do CT-e.");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarDadosCTeAnteriores(Dominio.ObjetosDeValor.WebService.CTe.DadosCTes dadosCtes)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            if (dadosCtes.ProtocoloDaCarga == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Protocolo da Carga não informado");

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorProtocoloCarga(dadosCtes.ProtocoloDaCarga);

            if (cargaPedido == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Carga com o protocolo não encontrada: {dadosCtes.ProtocoloDaCarga}");

            if (dadosCtes.ListaCte.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Por favor informar CTes que serão adicionados na carga");

            string retorno = string.Empty;

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte in dadosCtes.ListaCte)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                retorno = serCTeSubContratacao.InformarDadosCTeNaCarga(_unitOfWork, cte, cargaPedido, _tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);
                if (!string.IsNullOrEmpty(retorno))
                {
                    Servicos.Log.TratarErro($"Retornou erro: {retorno}", "IntegrarDadosCTeAnteriores");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(retorno))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(retorno);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            serCTeSubContratacao.CriarNotasFiscaisDaCargaPedido(cargaPedido, _tipoServicoMultisoftware, configuracao, _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            if (limite > 50)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

            if (protocolo == null)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

            if (protocolo.protocoloIntegracaoCarga <= 0 && protocolo.protocoloIntegracaoPedido <= 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Por favor, informe os códigos de integração.");

            Repositorio.Cliente repositorioCliente = new(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Cliente remetente = null;
            Dominio.Entidades.Cliente destinatario = null;

            if (protocolo.Remetente != null)
            {
                remetente = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Remetente.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                if (remetente == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(
                        $"Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}."
                    );
            }

            if (protocolo.Destinatario != null)
            {
                destinatario = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Destinatario.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                if (destinatario == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(
                        "Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}."
                    );
            }

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new(_unitOfWork);


            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            if (configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada)
            {
                Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new(_unitOfWork);
                Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new(_unitOfWork);

                List<int> protocolos = listaCargaPedido.Select(cargaPedido => cargaPedido.Carga.Protocolo).ToList();

                List<int> protocolosTransbordos = repositorioTransbordo.BuscarPorProtocoloIntegracaoCargaOrigem(protocolos);
                List<int> protocolosRedespachos = repositorioRedespacho.BuscarPorProtocoloIntegracaoCargaOrigem(protocolos);
                List<int> protocolosTransbordosRedespachos = protocolosTransbordos.Concat(protocolosRedespachos).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoTransbordosRedespachos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(
                    protocolosTransbordosRedespachos,
                    protocolo.protocoloIntegracaoPedido
                );

                listaCargaPedido.AddRange(listaCargaPedidoTransbordosRedespachos);
            }

            if (listaCargaPedido == null || listaCargaPedido.Count == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Os protocolos informados não existem na base Multisoftware.");

            if (
                !configuracao.RetornarCargaPendenciaEmissao &&
                !configuracao.AgruparCargaAutomaticamente &&
                listaCargaPedido.Any(o => o.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && !o.Carga.AgImportacaoCTe)
            )
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Os documentos do protocolo informado ainda estão em sendo emitidos.");

            if (remetente != null)
            {

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumento = repositorioCargaOcorrenciaDocumento.BuscarOcorrenciasPendentesCargaRemetenteDestinatario(protocolo.protocoloIntegracaoCarga, remetente.CPF_CNPJ, 0);

                if (cargaOcorrenciaDocumento.Count > 0)
                {
                    if (cargaOcorrenciaDocumento.FirstOrDefault().CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Rejeitada)
                    {
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(
                            $"A Ocorrência foi rejeitada para o CT-e do remetente {remetente.Nome} na Carga.",
                            codigoMensagem: 302
                        );
                    }
                    else if (cargaOcorrenciaDocumento.Any(obj => obj.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Finalizada))
                    {
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(
                            $"Existem ocorrências não aprovadas para o CT-e do remetente {remetente.Nome} na Carga.",
                            codigoMensagem: 301
                        );
                    }
                }
            }

            bool consultaMultiModal = configuracao.UtilizaEmissaoMultimodal || configuracaoGeral.HabilitarFuncionalidadesProjetoGollum;

            int totalRegistros = ContarCTes(consultaMultiModal, listaCargaPedido, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, _unitOfWork);

            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> listaCte = totalRegistros > 0 ? BuscarCTes(consultaMultiModal, listaCargaPedido, tipoDocumentoRetorno, remetente?.CPF_CNPJ ?? 0d, destinatario?.CPF_CNPJ ?? 0d, inicio, limite, _unitOfWork) : new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>()
            {
                Itens = listaCte,
                NumeroTotalDeRegistro = totalRegistros
            };

            Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou CT-es", _unitOfWork);

            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> BuscarFaturaCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            if (limite > 50)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

            if (protocolo == null)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

            if (protocolo.protocoloIntegracaoCarga <= 0 && protocolo.protocoloIntegracaoPedido <= 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Por favor, informe os códigos de integração.");

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Cliente remetente = null;
            Dominio.Entidades.Cliente destinatario = null;

            if (protocolo.Remetente != null)
            {
                double.TryParse(Utilidades.String.OnlyNumbers(protocolo.Remetente.CPFCNPJ), out double cnpj);

                remetente = repCliente.BuscarPorCPFCNPJ(cnpj);

                if (remetente == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ " + protocolo.Remetente.CPFCNPJ + ".");
            }

            if (protocolo.Destinatario != null)
            {
                double.TryParse(Utilidades.String.OnlyNumbers(protocolo.Destinatario.CPFCNPJ), out double cnpj);

                destinatario = repCliente.BuscarPorCPFCNPJ(cnpj);

                if (destinatario == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ " + protocolo.Remetente.CPFCNPJ + ".");
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

            if (cargaPedido == null)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Os protocolos informados não existem na base Multisoftware.");

            if (cargaPedido.All(o => o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos))
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Os documentos do protocolo informado ainda estão em sendo emitidos.");

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>()
            {
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            if (remetente != null)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarOcorrenciasPendentesCargaRemetenteDestinatario(protocolo.protocoloIntegracaoCarga, remetente.CPF_CNPJ, 0);

                if (cargaOcorrenciaDocumento.Count > 0)
                {
                    if (cargaOcorrenciaDocumento.FirstOrDefault().CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada)
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Os documentos do protocolo informado ainda estão em sendo emitidos.", 302);
                    else if (cargaOcorrenciaDocumento.Any(obj => obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada))
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoDadosInvalidos("Existem ocorrências não aprovadas para o CT-e do remetente " + remetente.Nome + " na Carga.", 301);
                }
            }

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou Fatura CT-es", _unitOfWork);

            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>>.CriarRetornoSucesso(new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>()
            {
                Itens = BuscarFaturaCTes(cargaPedido, tipoDocumentoRetorno, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, inicio, limite, _unitOfWork),
                NumeroTotalDeRegistro = ContarCTes(false, protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, _unitOfWork)
            });
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorCarga(RequestCtePorCarga dadosRequest, Dominio.Entidades.WebService.Integradora integradora)
        {
            Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>()
            {
                Itens = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>(),
                NumeroTotalDeRegistro = 0
            };

            if (dadosRequest.Limite > 50)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50");

            if (dadosRequest.ProtocoloCarga == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Por favor, informe o protocolo de integração da carga.");

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            if (integradora.Empresa != null)
                carga = repCarga.BuscarPorCodigo(dadosRequest.ProtocoloCarga);
            else
                carga = repCarga.BuscarPorProtocolo(dadosRequest.ProtocoloCarga);

            if (carga == null)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos($"O protocolo informado ({dadosRequest.ProtocoloCarga}) não é de uma carga válida, por favor verifique.");

            string mensagem = "";

            retorno.Itens = this.BuscarCTesPorCarga(carga, dadosRequest.TipoDocumentoRetorno, dadosRequest.Inicio, dadosRequest.Limite, ref mensagem, integradora.Empresa, _tipoServicoMultisoftware, _unitOfWork);
            retorno.NumeroTotalDeRegistro = this.ContarCTesPorCarga(carga, integradora.Empresa, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(mensagem);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Consultou CT-es por Carga", _unitOfWork);

            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorOcorrencia(RequestCtePorOcorrencia dadosRequestCteOcorrencia, Dominio.Entidades.WebService.Integradora integradora)
        {
            Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>()
            {
                Itens = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>(),
                NumeroTotalDeRegistro = 0
            };

            if (dadosRequestCteOcorrencia.Limite > 50)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50");

            if (dadosRequestCteOcorrencia.ProtocoloIntegracaoOcorrencia == 0)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("Por favor, informe o protocolo de integração da ocorrência.");

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOocorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null;

            if (integradora.Empresa != null)
                cargaOcorrencia = repCargaOocorrencia.BuscarPorCodigo(dadosRequestCteOcorrencia.ProtocoloIntegracaoOcorrencia);

            if (cargaOcorrencia == null)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos($"O protocolo informado ( {dadosRequestCteOcorrencia.ProtocoloIntegracaoOcorrencia.ToString()}) não é de uma ocorrência válida, por favor verifique.");

            string mensagem = "";

            retorno.Itens = this.BuscarCTesPorOcorrencia(cargaOcorrencia, dadosRequestCteOcorrencia.TipoDocumentoRetorno, dadosRequestCteOcorrencia.Inicio, dadosRequestCteOcorrencia.Limite, ref mensagem, integradora.Empresa, _tipoServicoMultisoftware, _unitOfWork);
            retorno.NumeroTotalDeRegistro = this.ContarCTesPorOcorrencia(cargaOcorrencia, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos(mensagem);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Consultou CT-es por Ocorrencia", _unitOfWork);

            return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCTesAnteriores(Dominio.ObjetosDeValor.WebService.CTe.RequestCteAnteriores requestCteAnteriores)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCteTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Pacote.Pacote servicoPacote = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware);

            if (string.IsNullOrWhiteSpace(requestCteAnteriores.loggi_key))
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar a identificação do Pacote (loggi Key)");

            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repositorioCargaPedidoPacote.BuscarCargaPedidoPacotePorLoggiKey(requestCteAnteriores.loggi_key);

            Retorno<bool> retornoIntegracao = new Retorno<bool>();
            retornoIntegracao.DataRetorno = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            retornoIntegracao.Mensagem = "";
            retornoIntegracao.Status = true;
            string retorno = "";

            byte[] byteArray = Encoding.ASCII.GetBytes(Utilidades.String.Base64Decode(requestCteAnteriores.xml));
            MemoryStream stream = new MemoryStream(byteArray);
            object objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(stream);
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe objetoCte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);

            if (objetoCte == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos("Formato do CT-e inválido.");

            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(_unitOfWork, ref retorno, null, objetoCte, null, false, 0, false, false, _tipoServicoMultisoftware, requestCteAnteriores.loggi_key);
            if (string.IsNullOrWhiteSpace(retorno))
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote in cargaPedidoPacotes)
                    retorno += new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, null, configuracao).GetAwaiter().GetResult();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = (from obj in cargaPedidoPacotes where obj.CargaPedido != null select obj.CargaPedido.Carga).FirstOrDefault();
                if (carga != null)
                    servicoPacote.VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(carga, _auditado).GetAwaiter().GetResult();
            }


            if (!string.IsNullOrWhiteSpace(retorno))
                return Retorno<bool>.CriarRetornoDadosInvalidos(retorno);

            return retornoIntegracao;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite, string codigoTipoOperacao, string situacao, Dominio.Entidades.WebService.Integradora integradora, bool considerarHora = false)
        {
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (limite <= 50)
                {
                    DateTime _dataInicial = dataInicial.ToDateTime();
                    DateTime _dataFinal = dataFinal.ToDateTime();

                    if (_dataInicial != DateTime.MinValue && _dataFinal != DateTime.MinValue)
                    {
                        Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
                        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWS = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS = repConfiguracaoWS.BuscarConfiguracaoPadrao();

                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaos = repTipoIntegracao.BuscarTipos();

                        string mensagem = "";

                        bool somentePosIntegracao = tipoIntegracaos.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

                        retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>
                        {
                            Itens = serWSCTe.BuscarCTesPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, tipoDocumentoRetorno, inicio, limite, codigoTipoOperacao, situacao, ref mensagem, _unitOfWork, configuracaoWS, considerarHora),
                            NumeroTotalDeRegistro = serWSCTe.ContarCTesPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, integradora.Cliente?.CPF_CNPJ ?? 0, somentePosIntegracao, codigoTipoOperacao, situacao, _unitOfWork, configuracaoWS, considerarHora)
                        };

                        retorno.Status = true;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou CT-es", _unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "É obrigatório informar a data inicial e a data final.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os CTes";
            }
            finally
            {
                _unitOfWork.Dispose();
            }
            return retorno;
        }

        public string VincularCteAnteriorNaCarga(List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> Ctes, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool trechoAnterior, bool proximoTrecho)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte in Ctes)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                string ret = serCTeSubContratacao.InformarDadosCTeNaCarga(_unitOfWork, cte, cargaPedido, _tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);
                if (!string.IsNullOrWhiteSpace(ret))
                    return ret;
            }

            serCTeSubContratacao.CriarNotasFiscaisDaCargaPedido(cargaPedido, _tipoServicoMultisoftware, configuracao, _unitOfWork);

            if (cargaPedido.CargaPedidoProximoTrecho != null && !trechoAnterior)
                VincularCteAnteriorNaCarga(Ctes, cargaPedido.CargaPedidoProximoTrecho, configuracao, false, true);

            if (cargaPedido.CargaPedidoTrechoAnterior != null && !proximoTrecho)
                VincularCteAnteriorNaCarga(Ctes, cargaPedido.CargaPedidoTrechoAnterior, configuracao, true, false);

            bool enviouTodas = true;
            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
            {
                if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) <= 0)
                    enviouTodas = false;
            }
            else
            {
                if (repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cargaPedido.Codigo) <= 0)
                    enviouTodas = false;
            }

            if (enviouTodas)
            {
                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                repCargaPedido.Atualizar(cargaPedido);
            }

            FinalizarEnvioDosCTesAnteriores(cargaPedido);

            return "";
        }

        public void FinalizarEnvioDosCTesAnteriores(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

            if (carga.SituacaoCarga != SituacaoCarga.AgNFe || !repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                return;

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                if (carga.AguardarIntegracaoEtapaTransportador)
                    return;

                if ((carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && !(carga.TipoOperacao?.NaoExigeConformacaoDasNotasEmissao ?? false))
                    return;
            }
            else if ((carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && carga.ExigeConfirmacaoAntesEmissao)
                return;

            carga.ProcessandoDocumentosFiscais = true;
            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;

            repositorioCarga.Atualizar(carga);
        }

        public string ObterRetornoXML(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CTe serCte = new Servicos.CTe(unidadeDeTrabalho);
            bool deveBuscarXML = cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe;

            if (!deveBuscarXML)
                return string.Empty;

            if (cte.Status == "A")
                return serCte.ObterStringXMLAutorizacao(cte, unidadeDeTrabalho);

            else if (cte.Status == "C")
                return serCte.ObterStringXMLCancelamento(cte, unidadeDeTrabalho);

            return string.Empty;
        }

        public string ObterRetornoXMLAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CTe serCte = new Servicos.CTe(unidadeDeTrabalho);
            return serCte.ObterStringXMLAutorizacao(cte, unidadeDeTrabalho);
        }


        public Retorno<Paginacao<CTeComplementar>> BuscarCTesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, string codificarUTF8, int inicio, int limite, string dataIncial = null, string dataFinal = null)
        {
            if (limite > 50)
                throw new ServicoException("O limite não pode ser maior que 50");
            else if (limite == 0)
                limite = 50;

            DateTime.TryParseExact(dataIncial, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataInicialConvertida);
            DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataFinalConvertida);

            if (!string.IsNullOrWhiteSpace(dataIncial) && dataInicialConvertida == DateTime.MinValue)
                throw new ServicoException("A dataInicial não está no formato correto (dd/MM/yyyy)");

            if (!string.IsNullOrWhiteSpace(dataFinal) && dataFinalConvertida == DateTime.MinValue)
                throw new ServicoException("A dataFinal não está no formato correto (dd/MM/yyyy)");

            Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            bool vCodificarUTF8 = false;

            if (string.IsNullOrWhiteSpace(codificarUTF8) || codificarUTF8 == "S")
                vCodificarUTF8 = true;

            Paginacao<CTeComplementar> paginacaoRetorno = new Paginacao<CTeComplementar>() { Itens = new List<CTeComplementar>() };

            paginacaoRetorno.NumeroTotalDeRegistro = repositorioCargaCTeComplementoInfo.ContarCTesAguardandoIntegracao(dataInicialConvertida, dataFinalConvertida);

            if (paginacaoRetorno.NumeroTotalDeRegistro > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTeComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesAguardandoIntegracao(inicio, limite, dataInicialConvertida, dataFinalConvertida);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTeComplementoInfo)
                {
                    CTeComplementar cTeComplementar = new CTeComplementar();
                    cTeComplementar.ProtocoloCTeComplementado = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Codigo;
                    cTeComplementar.CTe = serWSCTe.ConverterObjetoCargaCTeComplementoInfo(cargaCTeComplementoInfo, tipoDocumentoRetorno, vCodificarUTF8, _unitOfWork);

                    if (cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia != null)
                    {
                        cTeComplementar.Ocorrencia = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia
                        {
                            Protocolo = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Codigo,
                            CodigoIntegracao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoProceda,
                            Descricao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Descricao,
                            NumeroOcorrencia = cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia,
                            ProtocoloOcorrencia = cargaCTeComplementoInfo.CargaOcorrencia.Codigo,
                            CPFResponsavel = cargaCTeComplementoInfo.CargaOcorrencia.UsuarioResponsavelAprovacao?.CPF,
                            NumeroCargaEmbarcador = cargaCTeComplementoInfo.CargaOcorrencia.Carga?.CodigoCargaEmbarcador ?? "",
                            NomeResponsavel = cargaCTeComplementoInfo.CargaOcorrencia.UsuarioResponsavelAprovacao?.Nome,
                            ProtocoloCarga = cargaCTeComplementoInfo.CargaOcorrencia.Carga?.Protocolo ?? 0
                        };
                    }

                    paginacaoRetorno.Itens.Add(cTeComplementar);
                }
            }

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou CT-es complementares aguardando integração", _unitOfWork);

            return Retorno<Paginacao<CTeComplementar>>.CriarRetornoSucesso(paginacaoRetorno);
        }

        public Retorno<List<SituacaoCTe>> VerificarSituacaoCTe(Dominio.ObjetosDeValor.Embarcador.CTe.VerificarSituacaoCTe verificarSituacaoCTe)
        {

            Retorno<List<SituacaoCTe>> retorno = new Retorno<List<SituacaoCTe>>();
            UnitOfWork unitOfWork = new UnitOfWork(_unitOfWork.StringConexao);
            ConhecimentoDeTransporteEletronico repositorioCte = new ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Documentos.GestaoDocumento repGestaoDocumentos = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

            try
            {
                if (verificarSituacaoCTe.ChavesCTe.Count == 0)
                    throw new ServicoException("É obrigatório informar pelo menos uma chave CT-e para consulta.");

                if (verificarSituacaoCTe.ChavesCTe.Count > 25)
                    throw new ServicoException("O limite não pode ser maior que 25");

                retorno.Objeto = new List<SituacaoCTe>();

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repositorioCte.BuscarPorChaves(verificarSituacaoCTe.ChavesCTe);

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTes)
                {
                    SituacaoCTe situacaoCTe = new SituacaoCTe();
                    situacaoCTe.ChaveCTe = cte.Chave;

                    if (cte != null)
                    {
                        situacaoCTe.ProtocoloCTe = cte.Codigo;

                        if (cte.Status == "C" || cte.Status == "K")
                            situacaoCTe.SituacaoDocumento = SituacaoDocumento.Rejeitado;
                        else
                        {
                            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento documento = repGestaoDocumentos.BuscarPorCTe(cte.Codigo);

                            if (documento == null || documento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Aprovado || documento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.AprovadoComDesconto)
                                situacaoCTe.SituacaoDocumento = SituacaoDocumento.Aprovado;
                            else if (documento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Inconsistente)
                                situacaoCTe.SituacaoDocumento = SituacaoDocumento.Inconsistente;
                            else
                                situacaoCTe.SituacaoDocumento = SituacaoDocumento.Rejeitado;
                        }
                    }
                    else
                        situacaoCTe.SituacaoDocumento = SituacaoDocumento.NaoLocalizado;

                    retorno.Objeto.Add(situacaoCTe);
                }

                retorno.Status = true;
            }
            catch (BaseException excecao)
            {
                retorno.Status = false;
                retorno.Mensagem = excecao.Message;
                retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Falha ao verificar situação dos CTes";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoCTeComplementar(int protocoloCTe)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorCTe(protocoloCTe);

                if (cargaCTeComplementoInfo != null)
                {
                    if (!cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador)
                    {
                        cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador = true;
                        repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeComplementoInfo, "Confirmou integração com CT-e complementar", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Objeto = true;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "A integração já foi confirmada anteriormente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocólo do CT-e informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar confirmar a integração do CT-e complementar.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesSubstitutosAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, string codificarUTF8, int inicio, int limite)
        {
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);
            try
            {
                if (limite <= 50)
                {

                    Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                    bool vCodificarUTF8 = false;
                    if (string.IsNullOrWhiteSpace(codificarUTF8) || codificarUTF8 == "S")
                        vCodificarUTF8 = true;

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>();
                    retorno.Status = true;
                    List<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar> cargasCTeSubstitutos = repCargaCTeComplementoInfo.BuscarCTesSubstitutosAguardandoIntegracao();
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaCTeComplementoInfo.ContarCTesSubstitutosAguardandoIntegracao();
                    retorno.Objeto.Itens = cargasCTeSubstitutos;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou CT-es Substitutos aguardando integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os CTes substitutos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoCTeSubstituto(int protocoloCTe)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorCTe(protocoloCTe);

                if (cargaCTeComplementoInfo != null)
                {
                    if (!cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador)
                    {
                        cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador = true;
                        repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeComplementoInfo, "Confirmou integração com CT-e complementar", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Objeto = true;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "A integração já foi confirmada anteriormente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocólo do CT-e informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar confirmar a integração do CT-e complementar.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> EnviarArquivoXMLCTe(Stream arquivo)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);
                Retorno<string> retorno = new Retorno<string>();

                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao, string.Concat(nomeArquivo, ".xml"));

                using (StreamReader reader = new StreamReader(arquivo))
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, RemoveTroublesomeCharacters(reader.ReadToEnd()), Encoding.UTF8);

                arquivo.Close();
                arquivo.Dispose();

                using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
                    Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(stream, unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware, nomeArquivo, false, false, _auditado);

                retorno.Status = true;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Objeto = nomeArquivo;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(int inicio, int limite, Dominio.Entidades.WebService.Integradora integradora)
        {
            Retorno<Paginacao<int>> retorno = new Retorno<Paginacao<int>>();
            try
            {
                if (limite <= 100)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    int codigoTransportadora = integradora?.Empresa?.Codigo ?? 0;

                    retorno.Objeto = new Paginacao<int>();
                    retorno.Objeto.NumeroTotalDeRegistro = repCTe.ContarCTesCanceladosAguardandoConsulta(configuracao.RetornarCTeIntulizadoNoFluxoCancelamento, codigoTransportadora);
                    retorno.Objeto.Itens = new List<int>();
                    if (retorno.Objeto.NumeroTotalDeRegistro > 0)
                    {
                        List<int> protocolos = repCTe.BuscarListaCodigosCTesCanceladosAguardandoConsulta(configuracao.RetornarCTeIntulizadoNoFluxoCancelamento, codigoTransportadora, inicio, limite);
                        foreach (int codigo in protocolos)
                            retorno.Objeto.Itens.Add(codigo);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou o protocolo dos CT-es cancelados aguardando confirmacao", _unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a CTes";
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTePorProtocolo(int protocoloCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Dominio.Entidades.WebService.Integradora integradora)
        {
            Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (protocoloCTe > 0)
                {
                    Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                    Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(_unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(_unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    int codigoTransportadora = integradora?.Empresa?.Codigo ?? 0;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(protocoloCTe, codigoTransportadora);
                    if (cargaCTe != null)
                    {
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(cargaCTe.CTe.Codigo);
                        retorno.Objeto = serWSCTe.ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou CT-e por protocolo", _unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informado não é de um CT-e existente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe o Protocolo do CT-e.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar o CTe";
            }
            return retorno;
        }

        public Retorno<bool> ConfirmarConsultaCTeCancelado(int protocoloCTe, Dominio.Entidades.WebService.Integradora integradora)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.RetornoXMLCTe repRetornoXMLCTe = new Repositorio.RetornoXMLCTe(_unitOfWork);

            try
            {
                int codigoTransportadora = integradora?.Empresa?.Codigo ?? 0;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoTransportadora, protocoloCTe);

                if (cte != null)
                {
                    Dominio.Entidades.RetornoXMLCTe retornoXMLCTe = new Dominio.Entidades.RetornoXMLCTe();
                    retornoXMLCTe.CTe = cte;
                    retornoXMLCTe.Status = cte.Status;
                    repRetornoXMLCTe.Inserir(retornoXMLCTe);

                    retorno.Objeto = true;
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cte, "Confirmou consulta de CT-es cancelados", _unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo do CTe não localizado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Falha ao confirmar consulta CTe Cancelado";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public async Task<Retorno<bool>> CancelarCTeIndividualAsync(CancelarCTeIndividual cancelarCTeIndividual, Dominio.Entidades.WebService.Integradora integradora, CancellationToken cancellationToken)
        {
            try
            {
                ConhecimentoDeTransporteEletronico repositorioCTe = new ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

                if (string.IsNullOrWhiteSpace(cancelarCTeIndividual.ChaveCte) || cancelarCTeIndividual.ChaveCte.Length != 44)
                    throw new ServicoException($"Chave do CT-e inválida ({cancelarCTeIndividual.ChaveCte}).");

                if (string.IsNullOrWhiteSpace(cancelarCTeIndividual.Justificativa) || cancelarCTeIndividual.Justificativa.Trim().Length < 20)
                    throw new ServicoException($"Justificativa inválida ({cancelarCTeIndividual.Justificativa}).");

                if (cancelarCTeIndividual.ProtocoloCarga <= 0)
                    throw new ServicoException("Protocolo do CT-e inválido.");

                int codigoEmpresaCte = integradora.Empresa?.Codigo ?? 0;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioCTe.BuscarPorChaveAsync(codigoEmpresaCte, cancelarCTeIndividual.ChaveCte) ?? throw new ServicoException("CT-e não encontrado.");

                if (cte.Status != "A")
                    throw new ServicoException("O status do CT-e não permite o cancelamento do mesmo.");

                if (cte.StatusMDFe == StatusMDFe.Autorizado || cte.StatusMDFe == StatusMDFe.Encerrado)
                    throw new ServicoException($"O status do MDFe não permite o cancelamento ({cte.StatusMDFe.ObterDescricao()})");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(cancelarCTeIndividual.ProtocoloCarga) ?? throw new ServicoException("Carga não encontrada.");

                if (carga.SituacaoCarga == SituacaoCarga.Encerrada)
                    throw new ServicoException($"Não é possível cancelar CT-e da carga na situação {carga.SituacaoCarga.ObterDescricao()}");

                if (carga.Protocolo != cte.ProtocoloCarga)
                    throw new ServicoException("CTe informado não pertence a carga.");

                Servicos.CTe servicoCte = new Servicos.CTe(_unitOfWork);

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, cancelarCTeIndividual.Justificativa, _unitOfWork))
                    throw new ServicoException($"Não foi possível enviar o CT-e ({cte.Chave}) para cancelamento.");

                await Auditoria.Auditoria.AuditarAsync(_auditado, carga, $"Solicitado cancelamento do CT-e ({cte.Chave}) via Integração", _unitOfWork, cancellationToken);

                return Retorno<bool>.CriarRetornoSucesso(true, $"CT-e ({cte.Chave}) em processo de cancelamento.");
            }
            catch (BaseException excecao)
            {
                Log.TratarErro($"Tentativa cancelamento CTe: {cancelarCTeIndividual} " + excecao); // Temporário
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro("Falha ao cancelar o CTe: " + excecao);

                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao cancelar o CT-e.");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public List<string> TratarErroHandlind(com.maersk.BillableItemsPostRequest objetoAvro, Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            var erros = new List<string>();

            foreach (var obj in objetoAvro.billableItems)
            {
                var camposObrigatorios = new Dictionary<string, string?>
                {
                    { "payToParty", obj.payToParty },
                    { "billToParty", obj.billToParty },
                    { "consignee", obj.consignee },
                    { "portOfDischarge", obj.portOfDischarge },
                    { "portOfLoading", obj.portOfLoading },
                    { "placeOfDelivery", obj.placeOfDelivery },
                    { "placeOfReceipt", obj.placeOfReceipt },
                    { "vesselCode", obj.vesselCode },
                    { "vesselCodeFirstMotherVessel", obj.vesselCodeFirstMotherVessel },
                    { "vesselCodeLastMotherVessel", obj.vesselCodeLastMotherVessel }
                };

                if (conhecimento.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    camposObrigatorios.Remove("consignee");
                    camposObrigatorios.Remove("vesselCodeFirstMotherVessel");
                    camposObrigatorios.Remove("vesselCodeLastMotherVessel");
                }
                else if (conhecimento.ModeloDocumentoFiscal.Abreviacao == "DAT")
                {
                    camposObrigatorios.Remove("placeOfDelivery");
                    camposObrigatorios.Remove("placeOfReceipt");
                    camposObrigatorios.Remove("vesselCode");
                    camposObrigatorios.Remove("consignee");
                    camposObrigatorios.Remove("vesselCodeFirstMotherVessel");
                    camposObrigatorios.Remove("vesselCodeLastMotherVessel");
                }

                foreach (var campo in camposObrigatorios)
                {
                    if (string.IsNullOrWhiteSpace(campo.Value))
                    {
                        erros.Add($"Dados obrigatórios não informados (“{campo.Key}”). - Nrº Pedido: {cargaPedido?.Pedido?.Numero}");
                    }
                }
            }

            return erros;
        }


        #endregion

        #region Métodos Privados

        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }

        private string ObterBoletoPDF(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, bool codificarUTF8, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (titulo != null && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                {
                    byte[] dacte = null;
                    dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);
                    string stringDacte = null;

                    if (dacte != null)
                    {
                        if (codificarUTF8)
                            stringDacte = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, dacte));
                        else
                            stringDacte = Convert.ToBase64String(dacte);
                    }

                    if (!string.IsNullOrWhiteSpace(stringDacte))
                        return stringDacte;
                    else
                        return string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return string.Empty;
            }
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoArquivo == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                _configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
                return _configuracaoArquivo;
            }

            return _configuracaoArquivo;
        }

        private anl.documentation.ctetransport.transportModeType? ConverterTipoModal(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal)
        {
            switch (tipoModal)
            {
                case TipoModal.Rodoviario: return anl.documentation.ctetransport.transportModeType.ROAD;
                case TipoModal.Aereo: return anl.documentation.ctetransport.transportModeType.AIR;
                case TipoModal.Aquaviario: return anl.documentation.ctetransport.transportModeType.MARITIME;
                case TipoModal.Ferroviario: return anl.documentation.ctetransport.transportModeType.RAIL;
                case TipoModal.Multimodal: return anl.documentation.ctetransport.transportModeType.MULTI_MODAL;
                default: return null;
            }
        }

        private anl.documentation.ctetransport.itemQuantityUnitType ConverterUnidadeMedia(string unidadeMedida)
        {
            switch (unidadeMedida)
            {
                case "00": return anl.documentation.ctetransport.itemQuantityUnitType.METER;
                case "01": return anl.documentation.ctetransport.itemQuantityUnitType.KILOGRAM;
                //case "02": return anl.documentation.ctetransport.itemQuantityUnitType.; Ton
                case "03": return anl.documentation.ctetransport.itemQuantityUnitType.UNIT;
                //case "04": return anl.documentation.ctetransport.itemQuantityUnitType.; LT
                //case "05": return anl.documentation.ctetransport.itemQuantityUnitType.MULTI_MODAL; MMBTU
                default: return anl.documentation.ctetransport.itemQuantityUnitType.KILOGRAM;
            }
        }

        private anl.documentation.ctetransport.directionType? ConverterDirecao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal? direcao)
        {
            switch (direcao)
            {
                case DirecaoViagemMultimodal.Sul: return anl.documentation.ctetransport.directionType.SOUTH;
                case DirecaoViagemMultimodal.Norte: return anl.documentation.ctetransport.directionType.NORTH;
                case DirecaoViagemMultimodal.Oeste: return anl.documentation.ctetransport.directionType.WEST;
                case DirecaoViagemMultimodal.Leste: return anl.documentation.ctetransport.directionType.EAST;
                default: return null;
            }
        }
        public com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType ConverterDirecaoParaCardinalDirection(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal? direcao)
        {
            switch (direcao)
            {
                case DirecaoViagemMultimodal.Sul: return com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType.SOUTH;
                case DirecaoViagemMultimodal.Norte: return com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType.NORTH;
                case DirecaoViagemMultimodal.Oeste: return com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType.WEST;
                case DirecaoViagemMultimodal.Leste: return com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType.EAST;
                default: return com.maersk.billableitemspostrequest.billableitems.cardinalDirectionType.EAST;
            }
        }

        private anl.documentation.ctetransport.cteTypeEnum? ConverterTipoCTE(Dominio.Enumeradores.TipoCTE? tipoCTE)
        {
            switch (tipoCTE)
            {
                case Dominio.Enumeradores.TipoCTE.Anulacao: return anl.documentation.ctetransport.cteTypeEnum.Annulment;
                case Dominio.Enumeradores.TipoCTE.Complemento: return anl.documentation.ctetransport.cteTypeEnum.Complement;
                case Dominio.Enumeradores.TipoCTE.Normal: return anl.documentation.ctetransport.cteTypeEnum.Normal;
                case Dominio.Enumeradores.TipoCTE.Substituto: return anl.documentation.ctetransport.cteTypeEnum.Substitute;
                default: return null;
            }
        }

        private anl.documentation.ctetransport.cteServiceTypeEnum? ConverterTipoServico(Dominio.Enumeradores.TipoServico? tipoServico)
        {
            switch (tipoServico)
            {
                case Dominio.Enumeradores.TipoServico.Normal: return anl.documentation.ctetransport.cteServiceTypeEnum.Normal;
                case Dominio.Enumeradores.TipoServico.SubContratacao: return anl.documentation.ctetransport.cteServiceTypeEnum.Subcontracting;
                case Dominio.Enumeradores.TipoServico.Redespacho: return anl.documentation.ctetransport.cteServiceTypeEnum.Redispatch;
                case Dominio.Enumeradores.TipoServico.RedIntermediario: return anl.documentation.ctetransport.cteServiceTypeEnum.IntermediateRed;
                case Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal: return anl.documentation.ctetransport.cteServiceTypeEnum.MultimodalLinkedServ;
                case Dominio.Enumeradores.TipoServico.TransporteDePessoas: return anl.documentation.ctetransport.cteServiceTypeEnum.TransportOfPeople;
                case Dominio.Enumeradores.TipoServico.TransporteDeValores: return anl.documentation.ctetransport.cteServiceTypeEnum.TransportOfValues;
                case Dominio.Enumeradores.TipoServico.ExcessoDeBagagem: return anl.documentation.ctetransport.cteServiceTypeEnum.ExcessBaggage;
                default: return null;
            }
        }

        private anl.documentation.ctetransport.ConsigneeType? ConverterTipoTomador(Dominio.Enumeradores.TipoTomador? tipoTomador)
        {
            switch (tipoTomador)
            {
                case Dominio.Enumeradores.TipoTomador.Remetente: return anl.documentation.ctetransport.ConsigneeType.Shipper;
                case Dominio.Enumeradores.TipoTomador.Expedidor: return anl.documentation.ctetransport.ConsigneeType.Sender;
                case Dominio.Enumeradores.TipoTomador.Recebedor: return anl.documentation.ctetransport.ConsigneeType.Receiver;
                case Dominio.Enumeradores.TipoTomador.Destinatario: return anl.documentation.ctetransport.ConsigneeType.Recipient;
                case Dominio.Enumeradores.TipoTomador.Outros: return anl.documentation.ctetransport.ConsigneeType.Others;
                case Dominio.Enumeradores.TipoTomador.Intermediario: return anl.documentation.ctetransport.ConsigneeType.Intermediary;
                case Dominio.Enumeradores.TipoTomador.Tomador: return anl.documentation.ctetransport.ConsigneeType.Consigne;
                case Dominio.Enumeradores.TipoTomador.NaoInformado: return anl.documentation.ctetransport.ConsigneeType.NoInformed;
                default: return null;
            }
        }

        private anl.documentation.ctetransport.statusCteEnum? ConverterStatusCTE(string statusCTe)
        {
            switch (statusCTe)
            {
                case "A": return anl.documentation.ctetransport.statusCteEnum.Authorized;
                case "C": return anl.documentation.ctetransport.statusCteEnum.Canceled;
                //case "": return anl.documentation.ctetransport.statusCteEnum.Substituted;
                default: return null;
            }
        }

        private Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarPrevisaoPagamentoCTe(List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> informarPrevisoesPagamentosCTe, out List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> retornoLista)
        {
            retornoLista = new List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

            try
            {
                foreach (Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe informarPrevisaoPagamentoCTe in informarPrevisoesPagamentosCTe)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(informarPrevisaoPagamentoCTe.ProtocoloCTe);

                    if (cte == null)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi encontrado nenhum CT-e com o protocolo: {informarPrevisaoPagamentoCTe.ProtocoloCTe}");

                    if (cte.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e informado não está autorizado no Sefaz");

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cte.Titulo ?? new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    if (titulo.StatusTitulo == StatusTitulo.Quitada)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não é possível provisionar um título já quitado");

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    bool permitirRemoverDataPrevisaoPagamento = configuracaoWebService?.PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe ?? false;
                    bool permitirPreencherSomenteDataVencimento = configuracaoWebService?.DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe ?? false;

                    DateTime? dataPagto = null;
                    if (!string.IsNullOrWhiteSpace(informarPrevisaoPagamentoCTe.DataPrevisaoPagamento))
                    {
                        if (!DateTime.TryParseExact(informarPrevisaoPagamentoCTe.DataPrevisaoPagamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime tempDataPagto))
                            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de previsão de pagamento não está em um formato correto (dd/MM/yyyy)");

                        dataPagto = tempDataPagto;
                    }
                    else if (permitirRemoverDataPrevisaoPagamento)
                    {
                        dataPagto = null;
                    }
                    else
                    {
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("É necessário informar uma data de previsão de pagamento.");
                    }

                    Servicos.Log.GravarInfo($"Informando previsão de pagamento - protocolo " + informarPrevisaoPagamentoCTe.ProtocoloCTe + " com data pagto " + dataPagto.ToString() + " - status da configuração: " + permitirPreencherSomenteDataVencimento + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "WebServiceCTes");

                    _unitOfWork.Start();

                    titulo.Initialize();

                    if (!permitirPreencherSomenteDataVencimento)
                        titulo.DataLiquidacao = dataPagto;

                    titulo.DataEmissao = DateTime.Now;
                    titulo.DataVencimento = dataPagto;
                    titulo.DataProgramacaoPagamento = dataPagto;
                    titulo.Empresa = cte.Empresa;
                    titulo.Observacao = informarPrevisaoPagamentoCTe.Observacao;
                    titulo.NumeroFatura = informarPrevisaoPagamentoCTe.NumeroFatura;
                    titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));
                    titulo.Sequencia = informarPrevisaoPagamentoCTe.SequenciaParcela > 0 ? informarPrevisaoPagamentoCTe.SequenciaParcela : 1;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.ValorOriginal = cte.ValorAReceber;
                    titulo.IntegradoERP = false;

                    if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        titulo.TipoAmbiente = cte.TipoAmbiente;

                    if (titulo.Codigo == 0)
                    {
                        titulo.ValorPendente = cte.ValorAReceber;
                        titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;

                        repTitulo.Inserir(titulo, _auditado, null, "Adicionado pelo método InformarPrevisaoPagamentoCTe");

                        cte.Titulo = titulo;
                        repCTe.Atualizar(cte);
                    }
                    else
                        repTitulo.Atualizar(titulo, _auditado, null, "Atualizado pelo método InformarPrevisaoPagamentoCTe");


                    _unitOfWork.CommitChanges();

                    retornoLista.Add(new Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe()
                    {
                        ProtocoloCTe = titulo.Codigo,
                        DataPrevisaoPagamento = titulo.DataProgramacaoPagamento.ToString(),
                        NumeroFatura = titulo.NumeroFatura
                    });
                }
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Informou previsão de pagamento do CT-e");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao tentar provisionar o pagamento do CT-e.");
            }
        }

        private string DefinirDataIntegracao(DefinicaoDataEnvioIntegracao? definicaoDataEnvioIntegracao, Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var data = "";
            if (conhecimento == null || !definicaoDataEnvioIntegracao.HasValue)
                return "";

            switch (definicaoDataEnvioIntegracao.Value)
            {
                case DefinicaoDataEnvioIntegracao.EtaPol:
                    data = conhecimento.DataChegada?.ToString("yyyy-MM-dd") ?? "";
                    break;
                case DefinicaoDataEnvioIntegracao.EtdPol:
                    data = conhecimento.DataPartida?.ToString("yyyy-MM-dd") ?? "";
                    break;
                case DefinicaoDataEnvioIntegracao.EtaPod:
                    data = conhecimento.PedidoViagemNavioSchedule?.DataPrevisaoChegadaNavio?.ToString("yyyy-MM-dd") ?? "";
                    break;
                case DefinicaoDataEnvioIntegracao.EtsPod:
                    data = conhecimento.PedidoViagemNavioSchedule?.DataPrevisaoSaidaNavio?.ToString("yyyy-MM-dd") ?? "";
                    break;
                case DefinicaoDataEnvioIntegracao.ColetaJo:
                    data = carga.DataCarregamentoCarga?.ToString("yyyy-MM-dd") ?? "";
                    break;
                case DefinicaoDataEnvioIntegracao.EntregaJo:
                    data = conhecimento.PedidoViagemNavioSchedule?.DataPrevisaoChegadaNavio?.AddDays(5).ToString("yyyy-MM-dd") ?? "";
                    break;
                default:
                    break;
            }

            return data;
        }

        private string DefinirTipoContainer(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return "";

            numero = numero.Length >= 2 ? numero.Substring(numero.Length - 2) : numero;

            //if (numero == "20")
            //{
            //    return "8.6";
            //}
            //else
            //{
            //    return "9.6";
            //}

            switch (numero)
            {
                case "DC":
                    numero = "DRY";
                    break;
                case "HC":
                    numero = "DRY";
                    break;
                case "TK":
                    numero = "TANK";
                    break;
                case "RF":
                    numero = "REEF";
                    break;
                case "RH":
                    numero = "REEF";
                    break;
                case "OH":
                    numero = "OPEN";
                    break;
                case "OT":
                    numero = "OPEN";
                    break;
                case "FH":
                    numero = "FLAT";
                    break;
                case "FR":
                    numero = "FLAT";
                    break;
                case "20":
                    numero = "8.6";
                    break;
                case "40":
                    numero = "9.6";
                    break;
                default:
                    break;
            }


            return numero;
        }

        private string DefinirTipoContainerSize(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return "";

            numero = numero.Length >= 2 ? numero.Substring(0, 2) : numero;

            if (numero == "20")
            {
                return "8.6";
            }
            else
            {
                return "9.6";
            }

            return numero;
        }

        private string DefinirModalTipoProposta(ModalPropostaMultimodal? modalPropostaMultimodal)
        {

            switch (modalPropostaMultimodal)
            {
                case ModalPropostaMultimodal.PortoPorto:
                    return "CY-CY";
                case ModalPropostaMultimodal.PortoPorta:
                    return "CY-SD";
                case ModalPropostaMultimodal.PortaPorto:
                    return "SD-CY";
                case ModalPropostaMultimodal.PortaPorta:
                    return "SD-SD";
                default:
                    return "";
            }

            //switch (numero)
            //{
            //    case "DC":
            //        numero = "DRY";
            //        break;
            //    case "HC":
            //        numero = "DRY";
            //        break;
            //    case "TK":
            //        numero = "TANK";
            //        break;
            //    case "RF":
            //        numero = "REEF";
            //        break;
            //    case "RH":
            //        numero = "REEF";
            //        break;
            //    case "OH":
            //        numero = "OPEN";
            //        break;
            //    case "OT":
            //        numero = "OPEN";
            //        break;
            //    case "FH":
            //        numero = "FLAT";
            //        break;
            //    case "FR":
            //        numero = "FLAT";
            //        break;
            //    case "20":
            //        numero = "8.6";
            //        break;
            //    case "40":
            //        numero = "9.6";
            //        break;
            //    default:
            //        break;
            //}


            //return numero;
        }

        private List<com.maersk.billableitemspostrequest.BillableItems> PopularObjetoCTeCreation(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, bool gerarFaturamentoAVista, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, bool substituto, bool cteCancelado, bool cteComplementar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceita? tipoReceita, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia? cargaOcorrencia)
        {
            int sequencia = 0;
            List<com.maersk.billableitemspostrequest.BillableItems> billableItems = new List<com.maersk.billableitemspostrequest.BillableItems>();
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork, _cancellationToken);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimentoDeTransporte = repConhecimentoDeTransporteEletronico.BuscarPorCodigo(conhecimento.Codigo, true);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior = !string.IsNullOrWhiteSpace(conhecimento.ChaveCTESubComp) ? repConhecimentoDeTransporteEletronico.BuscarPorChave(conhecimento.ChaveCTESubComp) : null;
            BillableItems billableItem = new BillableItems();

            foreach (var componente in conhecimento.ComponentesPrestacao)
            {
                if (componente?.ComponenteFrete != null)
                {
                    if (!componente.ComponenteFrete.EnviarComponenteNFTP)
                    {
                        billableItem = PopouparBillableItem(++sequencia, integracaoEMP, conhecimento, componente, cargaPedido, gerarFaturamentoAVista, substituto, cteCancelado, cteComplementar, null, cteAnterior, unitOfWork, tipoReceita, cargaOcorrencia);
                        if (billableItem != null)
                            billableItems.Add(billableItem);
                    }
                }
                else
                {
                    billableItem = PopouparBillableItem(++sequencia, integracaoEMP, conhecimento, componente, cargaPedido, gerarFaturamentoAVista, substituto, cteCancelado, cteComplementar, null, cteAnterior, unitOfWork, tipoReceita, cargaOcorrencia);
                    if (billableItem != null)
                        billableItems.Add(billableItem);
                }

                componente.SourceSystemTransactionIdentifier = billableItem?.sourceSystemTransactionIdentifier ?? "";
                componente.NovoCampoIdentificador = billableItem?.sourceSystemTransactionIdentifier ?? "";
                repComponentePrestacaoCTE.Atualizar(componente);
            }

            billableItem = PopouparBillableItem(++sequencia, integracaoEMP, conhecimento, null, cargaPedido, gerarFaturamentoAVista, substituto, cteCancelado, cteComplementar, conhecimento.ValorPrestacaoServico, cteAnterior, unitOfWork, tipoReceita, cargaOcorrencia);
            if (billableItem != null)
                billableItems.Add(billableItem);

            conhecimentoDeTransporte.NovoCampoIdentificador = billableItem?.sourceSystemTransactionIdentifier ?? "";
            repConhecimentoDeTransporteEletronico.Atualizar(conhecimentoDeTransporte);

            return billableItems;
        }

        private com.maersk.billableitemspostrequest.BillableItems PopouparBillableItem(int sequencia, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP,
            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.Entidades.ComponentePrestacaoCTE? componente, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool gerarFaturamentoAVista,
              bool substituto, bool cteCancelado, bool cteComplementar, decimal? valorAReceber, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior, UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceita? tipoReceita, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia? cargaOcorrencia)
        {
            Repositorio.ComponentePrestacaoCTE repComponente = new ComponentePrestacaoCTE(unitOfWork);

            DefinicaoDataEnvioIntegracao? definicaoDataEnvioIntegracao;
            DefinicaoDataEnvioIntegracao? definicaoDataEnvioIntegracaoEmbarque;
            string chargeCode = "";

            var listaViagens = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>()
                {
                    conhecimento?.Viagem,
                    conhecimento?.ViagemPassagemUm,
                    conhecimento?.ViagemPassagemDois,
                    conhecimento?.ViagemPassagemTres,
                    conhecimento?.ViagemPassagemQuatro,
                    conhecimento?.ViagemPassagemCinco
                };

            if (componente == null && valorAReceber.HasValue && valorAReceber.Value > 0)
            {
                definicaoDataEnvioIntegracao = integracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.DefinicaoDataEnvioIntegracao;
                definicaoDataEnvioIntegracaoEmbarque = integracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.DefinicaoDataEnvioIntegracaoEmbarque;
                chargeCode = integracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.ChargeId ?? "";
                if (integracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.EnviarComponenteNFTP ?? false)
                    return null;
            }
            else if (componente?.Nome == "FRETE VALOR")
            {
                definicaoDataEnvioIntegracao = integracaoEMP?.ComponenteFreteNFTPEMP?.DefinicaoDataEnvioIntegracao;
                definicaoDataEnvioIntegracaoEmbarque = integracaoEMP?.ComponenteFreteNFTPEMP?.DefinicaoDataEnvioIntegracaoEmbarque;
                chargeCode = integracaoEMP?.ComponenteFreteNFTPEMP?.ChargeId ?? "";
                if (integracaoEMP?.ComponenteFreteNFTPEMP?.EnviarComponenteNFTP ?? false)
                    return null;
            }
            else if (componente?.Nome == "IMPOSTOS")
            {
                definicaoDataEnvioIntegracao = integracaoEMP?.ComponenteImpostoNFTPEMP?.DefinicaoDataEnvioIntegracao;
                definicaoDataEnvioIntegracaoEmbarque = integracaoEMP?.ComponenteImpostoNFTPEMP?.DefinicaoDataEnvioIntegracaoEmbarque;
                chargeCode = integracaoEMP?.ComponenteImpostoNFTPEMP?.ChargeId ?? "";
                if (integracaoEMP?.ComponenteImpostoNFTPEMP?.EnviarComponenteNFTP ?? false)
                    return null;
            }
            else
            {
                definicaoDataEnvioIntegracao = componente?.ComponenteFrete?.DefinicaoDataEnvioIntegracao;
                definicaoDataEnvioIntegracaoEmbarque = componente?.ComponenteFrete?.DefinicaoDataEnvioIntegracaoEmbarque;
                chargeCode = componente?.ComponenteFrete?.ChargeId ?? "";
                if (componente?.ComponenteFrete?.EnviarComponenteNFTP ?? false)
                    return null;
            }

            var remarkSped = string.Empty;

            if (cteComplementar && cargaOcorrencia is not null)
            {
                remarkSped = cargaOcorrencia.TipoOcorrencia?.RemarkSped != null
                                 ? RemarkSpedHelper.ObterDescricao(cargaOcorrencia.TipoOcorrencia?.RemarkSped.Value) : "";
            }
            else
            {
                remarkSped = cargaPedido.Carga?.TipoOperacao?.ConfiguracaoCarga?.RemarkSped != null
                                 ? RemarkSpedHelper.ObterDescricao(cargaPedido.Carga.TipoOperacao.ConfiguracaoCarga.RemarkSped.Value) : "";
            }


            sequencia++;

            com.maersk.billableitemspostrequest.BillableItems billableItem = new com.maersk.billableitemspostrequest.BillableItems();
            string identificadorUnico = SourceSystemTransactionGenerator.GerarIdentificador(sequencia);

            billableItem.sourceSystemTransactionIdentifier = identificadorUnico;
            billableItem.referenceSourceSystemTransactionIdentifier = "";
            billableItem.chargeCode = chargeCode;
            billableItem.baseRate = componente != null && componente?.Valor > 0 ? Convert.ToDouble(componente?.Valor) : Convert.ToDouble(conhecimento.ValorPrestacaoServico);
            billableItem.billingQuantity = conhecimento?.Containers?.Count();
            billableItem.unitOfMeasurementCode = "CNT";
            billableItem.billableItemAmount = valorAReceber.HasValue && valorAReceber.Value > 0 && componente == null ? Convert.ToDouble(valorAReceber.Value) : Convert.ToDouble(componente?.Valor);
            billableItem.isoCurrencyCode = "BRL";
            billableItem.invoicingIsoCurrencyCode = "BRL";
            billableItem.totalInvoiceAmount = Convert.ToDouble(conhecimento?.ValorPrestacaoServico);
            billableItem.companyCode = "6000";
            billableItem.travelMode = cargaPedido?.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ? "SD-SD" : cargaPedido?.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorto ? "SD-CY" : cargaPedido?.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta ? "CY-SD" : "CY-CY";
            billableItem.collectionBusinessUnit = conhecimento?.Empresa?.CodigoEmpresa;
            billableItem.cteNumber = conhecimento?.Numero.ToString();
            billableItem.cteControlNumber = conhecimento?.NumeroControle;
            billableItem.fiscalDocumentNumbeLinkedToCTe = string.Join(", ", conhecimento?.Documentos.Select(c => $"{c.Numero} - {c.Serie}"));
            billableItem.jobOrderNumber = conhecimento?.NumeroOS;
            billableItem.iBgeCodeSender = conhecimento?.Expedidor?.Localidade?.CodigoIBGE.ToString() ?? conhecimento?.Remetente?.Localidade?.CodigoIBGE.ToString();
            billableItem.iBgeCodeRecipient = conhecimento?.Recebedor?.Localidade?.CodigoIBGE.ToString() ?? conhecimento?.Destinatario?.Localidade?.CodigoIBGE.ToString();
            billableItem.taxIndicatorCst = conhecimento?.CST;
            billableItem.accessKey = conhecimento?.Chave;
            billableItem.externalReference = conhecimento?.Documentos?.FirstOrDefault()?.Numero;
            billableItem.fiscalDocumentNumber = $"{conhecimento?.Numero}";
            billableItem.nfSeIssuanceNumber = "";
            billableItem.invoiceDueDate = gerarFaturamentoAVista ? DateTime.Now.ToString("yyyy-MM-dd") : "";
            billableItem.estimatedArrivalDate = DefinirDataIntegracao(definicaoDataEnvioIntegracao, conhecimento, cargaPedido?.Carga) ?? "";
            billableItem.estimatedDepartureDate = DefinirDataIntegracao(definicaoDataEnvioIntegracaoEmbarque, conhecimento, cargaPedido?.Carga) ?? "";
            billableItem.sourceSystemTransactionDate = conhecimento.DataEmissao.HasValue ? conhecimento?.DataEmissao.Value.ToString("yyyy-MM-dd") : "";
            billableItem.timeOfOrigin = conhecimento.DataEmissao.HasValue ? conhecimento?.DataEmissao.Value.ToString("HH:mm:ss") : "";
            billableItem.bookingNumber = conhecimento?.NumeroBooking;
            billableItem.uniqueBillOfLading = $"ANRM{int.Parse(conhecimento?.Empresa?.CodigoEmpresa ?? "0").ToString("D3")}{(conhecimento?.Numero ?? 0).ToString().PadLeft(7, '0')}{(conhecimento?.Serie?.Numero ?? 0).ToString("D2")}";
            //billableItem.customerPayer = conhecimento.TomadorPagador.Cliente.CMDID;
            billableItem.billToParty = conhecimento?.TomadorPagador?.Cliente?.CMDID;
            billableItem.consignee = conhecimento?.Recebedor?.Cliente?.CMDID ?? conhecimento?.Destinatario?.Cliente?.CMDID;
            billableItem.customerReceiver = conhecimento?.Recebedor?.Cliente?.CMDID ?? conhecimento?.Destinatario?.Cliente?.CMDID;
            billableItem.portOfDischarge = conhecimento?.PortoDestino?.RKST ?? "";
            billableItem.portOfLoading = conhecimento?.PortoOrigem?.RKST ?? "";
            billableItem.placeOfDelivery = cargaPedido?.Destino?.RKST ?? "";
            billableItem.placeOfReceipt = cargaPedido?.Origem?.RKST ?? "";
            billableItem.vesselName = conhecimento?.Viagem?.Navio?.Descricao;
            billableItem.vesselCode = conhecimento?.Viagem?.Navio?.CodigoNavio?.ToString();
            billableItem.cardinalDirection = ConverterDirecaoParaCardinalDirection(conhecimento?.Viagem?.DirecaoViagemMultimodal);
            billableItem.voyageNumber = $"{conhecimento?.Viagem?.NumeroViagem}{conhecimento?.Viagem?.DirecaoViagemMultimodal.ObterAbreviacao() ?? ""}";
            billableItem.bundleReference = ""; // Implementar depois
            billableItem.payToParty = conhecimento?.TomadorPagador?.Cliente?.CMDID;
            billableItem.bookingMaerskFeederNumber = conhecimento?.BookingReference ?? "";
            billableItem.ceNumber = cargaPedido?.Pedido?.NumeroCEFeeder;
            billableItem.manifestNumber = cargaPedido?.Pedido?.NumeroManifestoFeeder;
            billableItem.pTSpedRemark = $"Receita de {remarkSped} {conhecimento.Numero}";
            billableItem.revenueType = tipoReceita.HasValue ? TipoReceitaHelper.ObterDescricao(tipoReceita) : "";
            billableItem.equipmentNumber = string.Join(", ", conhecimento?.Containers?.Select(c => (c.Container?.Numero) ?? ""));
            billableItem.equipmentType = string.Join(", ", conhecimento?.Containers?.Select(c => $"{DefinirTipoContainer(c.Container?.ContainerTipo?.Descricao) ?? ""}"));
            billableItem.equipmentSize = string.Join(", ", conhecimento?.Containers?.Select(c => $"{DefinirTipoContainerSize(c.Container?.ContainerTipo?.Descricao) ?? ""}"));
            billableItem.proposalNumber = cargaPedido?.Pedido?.CodigoProposta;
            billableItem.proposalType = cargaPedido?.TipoPropostaMultimodal.ObterDescricaoNoCode() ?? "";
            billableItem.vesselCodeFirstMotherVessel = $"{listaViagens?.FirstOrDefault()?.Navio?.CodigoNavio?.ToString() ?? ""}";
            billableItem.vesselCodeLastMotherVessel = $"{listaViagens?.Where(x => x != null).LastOrDefault()?.Navio?.CodigoNavio?.ToString() ?? ""}";
            billableItem.vesselNameFirstMotherVessel = listaViagens?.FirstOrDefault()?.Navio?.Descricao ?? "";
            billableItem.vesselNameLastMotherVessel = listaViagens?.Where(x => x != null).LastOrDefault()?.Navio?.Descricao ?? "";
            billableItem.voyageCodeFirstMotherVessel = $"{listaViagens?.FirstOrDefault()?.NumeroViagem.ToString() ?? ""}{listaViagens?.FirstOrDefault()?.DirecaoViagemMultimodal.ObterAbreviacao() ?? ""}";
            billableItem.voyageCodeLastMotherVessel = $"{listaViagens?.Where(x => x != null).LastOrDefault()?.NumeroViagem.ToString() ?? ""}{listaViagens?.Where(x => x != null).LastOrDefault()?.DirecaoViagemMultimodal.ObterAbreviacao() ?? ""}";

            if (cteComplementar)
            {
                billableItem.triggerType = "Complementary CTe Creation";
                billableItem.triggerDocumentType = "CTe Complementar";
                billableItem.referenceCteAccessKeyNumber = conhecimento?.ChaveCTESubComp;
            }
            if (cteCancelado)
            {
                billableItem.sourceSystemTransactionIdentifier = "";
                billableItem.referenceSourceSystemTransactionIdentifier = componente?.NovoCampoIdentificador ?? conhecimento?.NovoCampoIdentificador ?? "";
                billableItem.triggerDocumentType = cteComplementar ? "CTe Complementar" : "CTe Normal";
                billableItem.triggerType = cteComplementar ? "Complementary CTe Cancellation" : "CTe Cancellation";
                billableItem.cancelReason = conhecimento?.ObservacaoCancelamento;
            }
            else if (substituto)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteAnterior = null;
                if (cteAnterior != null && componente != null)
                    componenteAnterior = repComponente.BuscarPrimeiroPorCTeDescricao(cteAnterior.Codigo, componente.Nome);

                billableItem.triggerDocumentType = "CTe Substituto";
                billableItem.triggerType = "Substitute CTe Creation";
                billableItem.referenceCteAccessKeyNumber = conhecimento?.ChaveCTESubComp;
                billableItem.referenceSourceSystemTransactionIdentifier = componenteAnterior?.NovoCampoIdentificador ?? cteAnterior?.NovoCampoIdentificador ?? "";
            }
            else
            {
                if ((billableItem.triggerType == null || billableItem.triggerType == string.Empty) && (billableItem.referenceCteAccessKeyNumber == null || billableItem.referenceCteAccessKeyNumber == string.Empty))
                {
                    billableItem.triggerDocumentType = "CTe Normal";
                    billableItem.triggerType = "CTe Creation";
                }
            }

            return billableItem;
        }


        private string DefinirDescricaoTriggerType(bool cteComplementar, bool cteCancelado, bool substituto)
        {
            var descricao = string.Empty;

            if (cteComplementar && !cteCancelado)
            {
                descricao = "Complementary CTe Creation";
            }
            if (cteCancelado)
            {
                descricao = cteComplementar ? "Complementary CTe Cancellation" : "CTe Cancellation";
            }
            else if (substituto)
            {
                descricao = "Substitute CTe Creation";
            }
            else
            {
                if (descricao == string.Empty)
                {
                    descricao = "CTe Creation";
                }
            }

            return descricao;
        }

        #endregion Métodos Privados
    }
}
