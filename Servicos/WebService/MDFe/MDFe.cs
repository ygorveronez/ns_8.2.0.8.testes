using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.MDFe;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Servicos.WebService.MDFe
{
    public class MDFe : ServicoBase
    {

        #region Atributos Privativos

        
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;

        #endregion
        
        public MDFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MDFe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
        }

        public List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFePorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int inicioRegistro, int fimRegistro, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> mdfeIntegracao = new List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarTodosPorCarga(carga, tipoServicoMultisoftware);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                mdfeIntegracao.Add(ConverterObjetoCargaMDFe(cargaMDFe, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));

            return mdfeIntegracao;
        }

        public List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFe(int protocoloCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> mdfeIntegracao = new List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            List<int> protocolosCargas = new List<int> { protocoloCarga };

            if (configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada)
            {
                Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(_unitOfWork);
                Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(_unitOfWork);

                List<int> protocolosTransbordos = repositorioTransbordo.BuscarPorProtocoloIntegracaoCargaOrigem(protocoloCarga);
                List<int> protocolosRedespachos = repositorioRedespacho.BuscarPorProtocoloIntegracaoCargaOrigem(protocoloCarga);

                protocolosCargas.AddRange(protocolosTransbordos);
                protocolosCargas.AddRange(protocolosRedespachos);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarAutorizadosPorProtocoloCarga(protocolosCargas);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                mdfeIntegracao.Add(ConverterObjetoCargaMDFe(cargaMDFe, tipoDocumentoRetorno, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));

            return mdfeIntegracao;
        }

        private T ObterConfiguracao<T>(string configuracaoJson)
        {
            return JsonConvert.DeserializeObject<T>(configuracaoJson);
        }


        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFes(RequestMDFe requestMDFe)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>>();
           
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            if (requestMDFe.Limite > 50)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = "O limite não pode ser maior que 50";
                return retorno;
            }

            if (requestMDFe.ProtocoloIntegracaoCarga == 0)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "Por favor, informe o código de integração da carga.";
                return retorno;
            }

            string mensagem = "";

            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
            retorno.Objeto.Itens = this.BuscarMDFe(requestMDFe.ProtocoloIntegracaoCarga, requestMDFe.TipoDocumentoRetorno, requestMDFe.Inicio, requestMDFe.Limite, ref mensagem);
            retorno.Objeto.NumeroTotalDeRegistro = this.ContarMDFe(requestMDFe.ProtocoloIntegracaoCarga);
            retorno.Status = true;

            if (!string.IsNullOrWhiteSpace(mensagem))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = mensagem;
                return retorno;
            }
            else
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou MDF-es", _unitOfWork);

            return retorno;
        }


        public int ContarMDFe(int protocoloCarga)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            return repCargaMDFe.ContarAutorizadosPorProtocoloCarga(protocoloCarga);
        }

        public List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFePorMDFeManual(int protocoloMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> mdfeIntegracao = new List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> cargaMDFEs = repCargaMDFeManualMDFe.BuscarAutorizadosPorCargaMDFeManual(protocoloMDFeManual);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFe in cargaMDFEs)
                mdfeIntegracao.Add(ConverterObjetoCargaMDFeManual(cargaMDFe, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));

            return mdfeIntegracao;
        }

        public int ContarMDFePorMDFeManual(int protocoloMDFeManual)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);

            return repCargaMDFeManualMDFe.ContarAutorizadosPorCargaMDFeManual(protocoloMDFeManual);
        }

        public List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFesPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> MDFesIntegracao = new List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarMDFesPorPeriodoEmpresa(dataInicial, dataFinal, empresa, inicioRegistro, fimRegistro);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                MDFesIntegracao.Add(ConverterObjetoCargaMDFe(cargaMDFe, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));

            return MDFesIntegracao;
        }

        public int ContarMDFesPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            return repCargaMDFe.ContarMDFesPorPeriodoEmpresa(dataInicial, dataFinal, empresa);
        }

        public Dominio.ObjetosDeValor.WebService.MDFe.MDFe ConverterObjetoCargaMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipoDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Dominio.ObjetosDeValor.WebService.MDFe.MDFe mdfe = new Dominio.ObjetosDeValor.WebService.MDFe.MDFe();
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            mdfe.ProtocoloEncerramento = cargaMDFe.MDFe.ProtocoloEncerramento;
            mdfe.ProtocoloCancelamento = cargaMDFe.MDFe.ProtocoloCancelamento;
            mdfe.ProtocoloAutorizacao = cargaMDFe.MDFe.Protocolo;
            mdfe.Chave = cargaMDFe.MDFe.Chave;
            mdfe.DataEmissao = cargaMDFe.MDFe.DataEmissao.HasValue ? cargaMDFe.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            mdfe.Numero = cargaMDFe.MDFe.Numero;
            this.ObterArquivosRetorno(mdfe, cargaMDFe.MDFe, tipoDocumentoRetorno, unitOfWork, codificarUTF8);
            mdfe.Protocolo = cargaMDFe.MDFe.Codigo;
            mdfe.ProtocolosDeCTe = repDocumentoMunicipoDescarregamentoMDFe.BuscarCodigosDeCTesPorMDFe(cargaMDFe.MDFe.Codigo);
            mdfe.Serie = cargaMDFe.MDFe.Serie.Numero;
            mdfe.StatusMDFe = cargaMDFe.MDFe.Status;
            mdfe.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaMDFe.MDFe.Empresa);
            mdfe.UFDestino = cargaMDFe.MDFe.EstadoDescarregamento.Sigla;
            mdfe.UFOrigem = cargaMDFe.MDFe.EstadoCarregamento.Sigla;
            mdfe.NumeroCarga = cargaMDFe.Carga.CodigoCargaEmbarcador;
            return mdfe;
        }

        public Dominio.ObjetosDeValor.WebService.MDFe.MDFe ConverterObjetoCargaMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipoDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Dominio.ObjetosDeValor.WebService.MDFe.MDFe mdfe = new Dominio.ObjetosDeValor.WebService.MDFe.MDFe();
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            mdfe.Chave = cargaMDFe.MDFe.Chave;
            mdfe.DataEmissao = cargaMDFe.MDFe.DataEmissao.HasValue ? cargaMDFe.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            mdfe.Numero = cargaMDFe.MDFe.Numero;
            this.ObterArquivosRetorno(mdfe, cargaMDFe.MDFe, tipoDocumentoRetorno, unitOfWork, codificarUTF8);
            mdfe.Protocolo = cargaMDFe.MDFe.Codigo;
            mdfe.ProtocolosDeCTe = repDocumentoMunicipoDescarregamentoMDFe.BuscarCodigosDeCTesPorMDFe(cargaMDFe.MDFe.Codigo);
            mdfe.Serie = cargaMDFe.MDFe.Serie.Numero;
            mdfe.StatusMDFe = cargaMDFe.MDFe.Status;
            mdfe.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaMDFe.MDFe.Empresa);
            mdfe.UFDestino = cargaMDFe.MDFe.EstadoDescarregamento.Sigla;
            mdfe.UFOrigem = cargaMDFe.MDFe.EstadoCarregamento.Sigla;
            return mdfe;
        }

        private void ObterArquivosRetorno(Dominio.ObjetosDeValor.WebService.MDFe.MDFe retorno, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork, bool codificarUTF8)
        {
            Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML)
            {
                Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

                retorno.XMLAutorizacao = xmlMDFe?.XML;

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    retorno.XML = servicoMDFe.ObterStringXML(mdfe, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao, unitOfWork);
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                {
                    retorno.XML = servicoMDFe.ObterStringXML(mdfe, Dominio.Enumeradores.TipoXMLMDFe.Encerramento, unitOfWork);
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                {
                    retorno.XML = servicoMDFe.ObterStringXML(mdfe, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento, unitOfWork);
                }
            }

            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF)
            {
                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                {
                    retorno.PDF = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unitOfWork, codificarUTF8);
                }
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual GerarMDFeManual(Dominio.ObjetosDeValor.WebService.MDFe.MDFeManual mdfeManualIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, ref string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);

            if (mdfeManualIntegracao.ProtocolosDasCargas == null || mdfeManualIntegracao.ProtocolosDasCargas.Count == 0)
            {
                mensagemErro = "Necessário enviar um ou mais protocolos de carga.";
                return null;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga cargaPadrao = repCarga.BuscarPorCodigo(mdfeManualIntegracao.ProtocolosDasCargas[0]);
            if (cargaPadrao == null)
            {
                mensagemErro = "Carga protocolo " + mdfeManualIntegracao.ProtocolosDasCargas[0] + " não localizada.";
                return null;
            }

            if (cargaPadrao.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos &&
                cargaPadrao.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte &&
                cargaPadrao.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte &&
                cargaPadrao.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento &&
                cargaPadrao.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
            {
                mensagemErro = "Situação da carga " + cargaPadrao.CodigoCargaEmbarcador + " (" + cargaPadrao.DescricaoSituacaoCarga + ") não permite solicitação de MDFe.";
                unitOfWork.Rollback();
                return null;
            }

            //Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualExiste = repMDFeManual.VerificarCargaMDFeManualComCarga(cargaPadrao.Codigo);
            //if (cargaMDFeManualExiste != null && cargaMDFeManualExiste.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Cancelado)
            //{
            //    mensagemErro = "Carga " + cargaPadrao.CodigoCargaEmbarcador + " já associada a outro MDFe manual.";
            //    unitOfWork.Rollback();
            //    return null;
            //}

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cargaCTe = repCargaCTe.BuscarPrimeiroCTePorCarga(cargaPadrao.Codigo);
            //if (cargaCTe == null)
            //{
            //    mensagemErro = "Não localizado CTe para carga protocolo " + mdfeManualIntegracao.ProtocolosDasCargas[0] + ".";
            //    return null;
            //}

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual mdfeManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual();
            mdfeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.ProcessandoIntegracao;
            mdfeManual.CTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            mdfeManual.UsarDadosCTe = true;
            mdfeManual.UsarSeguroCTe = true;
            mdfeManual.TipoModalMDFe = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Rodoviario;
            mdfeManual.Empresa = cargaPadrao.Empresa;
            mdfeManual.Origem = cargaCTe?.LocalidadeInicioPrestacao ?? cargaPadrao.Filial?.Localidade;

            if (mdfeManualIntegracao.Motoristas != null && mdfeManualIntegracao.Motoristas.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in mdfeManualIntegracao.Motoristas)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));
                    if (motorista == null)
                    {
                        string mensagem = "";
                        motorista = serMotorista.SalvarMotorista(motoristaIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : mdfeManual.Empresa, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                        if (motorista == null)
                        {
                            mensagemErro = "Não é possível cadastrar motorista: " + mensagem;
                            unitOfWork.Rollback();
                            return null;
                        }
                    }

                    if (motorista != null)
                    {
                        mdfeManual.Motoristas = new List<Dominio.Entidades.Usuario>();
                        mdfeManual.Motoristas.Add(motorista);
                    }
                }
            }

            if (mdfeManualIntegracao.Veiculo != null && !string.IsNullOrWhiteSpace(mdfeManualIntegracao.Veiculo.Placa))
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(mdfeManual.Empresa.Codigo, mdfeManualIntegracao.Veiculo.Placa.Replace("-", ""));
                if (veiculo != null)
                    mdfeManual.Veiculo = veiculo;
                else
                {
                    string mensagemVeiculo = "";
                    mdfeManual.Veiculo = serWSVeiculo.SalvarVeiculo(mdfeManualIntegracao.Veiculo, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : mdfeManual.Empresa, false, ref mensagemVeiculo, unitOfWork, tipoServicoMultisoftware);
                    if (mdfeManual.Veiculo == null)
                    {
                        mensagemErro = "Não é possível cadastrar veículo " + mdfeManualIntegracao.Veiculo.Placa + " : " + mensagemVeiculo;
                        unitOfWork.Rollback();
                        return null;
                    }
                }

                if (mdfeManualIntegracao.Veiculo.Reboques != null && mdfeManualIntegracao.Veiculo.Reboques.Count > 0)
                {
                    if (mdfeManual.Reboques != null)
                        mdfeManual.Reboques.Clear();

                    mdfeManual.Reboques = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in mdfeManualIntegracao.Veiculo.Reboques)
                    {
                        if (!string.IsNullOrWhiteSpace(reboqueIntegracao.Placa))
                        {
                            Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(mdfeManual.Empresa.Codigo, reboqueIntegracao.Placa.Replace("-", ""));

                            if (reboque != null)
                                mdfeManual.Reboques.Add(reboque);
                            else
                            {
                                string mensagemVeiculo = "";

                                reboque = serWSVeiculo.SalvarVeiculo(reboqueIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : mdfeManual.Empresa, false, ref mensagemVeiculo, unitOfWork, tipoServicoMultisoftware);

                                if (string.IsNullOrWhiteSpace(mensagemVeiculo))
                                    mdfeManual.Reboques.Add(reboque);
                                else
                                {
                                    mensagemErro = "Não é possível cadastrar veículo " + reboqueIntegracao.Placa + " : " + mensagemVeiculo;
                                    unitOfWork.Rollback();
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            repMDFeManual.Inserir(mdfeManual);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = repCarga.BuscarPorCodigos(mdfeManualIntegracao.ProtocolosDasCargas);
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCargas)
            {
                if (mdfeManual.Cargas == null)
                    mdfeManual.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                mdfeManual.Cargas.Add(carga);
            }

            //for (var i = 0; i < mdfeManualIntegracao.ProtocolosDasCargas.Count(); i++)
            //{
            //    if (mdfeManual.Cargas == null)
            //        mdfeManual.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            //    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(mdfeManualIntegracao.ProtocolosDasCargas[i]);
            //    //Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = mdfeManualIntegracao.ProtocolosDasCargas[i] };
            //    mdfeManual.Cargas.Add(carga);
            //}

            if ((mdfeManual.Cargas == null || mdfeManual.Cargas.Count <= 0))
            {
                mensagemErro = "É necessário adicionar ao menos uma carga para solicitar MDFe.";
                unitOfWork.Rollback();
                return null;
            }

            if (mdfeManualIntegracao.ListaValePedagio != null && mdfeManualIntegracao.ListaValePedagio.Count > 0)
                SalvarValePedagio(ref mdfeManual, mdfeManualIntegracao.ListaValePedagio, unitOfWork);
            if (mdfeManualIntegracao.ListaCIOT != null && mdfeManualIntegracao.ListaCIOT.Count > 0)
                SalvarCIOT(ref mdfeManual, mdfeManualIntegracao.ListaCIOT, unitOfWork);

            repMDFeManual.Atualizar(mdfeManual);

            unitOfWork.CommitChanges();

            return mdfeManual;
        }

        private void SalvarValePedagio(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, List<Dominio.ObjetosDeValor.MDFe.ValePedagio> listaValePedagio, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio repValePedagioMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio(unidadeTrabalho);

            foreach (Dominio.ObjetosDeValor.MDFe.ValePedagio vale in listaValePedagio)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio val = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio();

                val.CargaMDFeManual = cargaMDFeManual;
                val.CNPJFornecedor = Utilidades.String.OnlyNumbers(vale.CNPJFornecedor);
                val.CNPJResponsavel = Utilidades.String.OnlyNumbers(vale.CNPJResponsavel);
                val.NumeroComprovante = vale.NumeroComprovante;
                val.ValorValePedagio = vale.ValorValePedagio;

                repValePedagioMDFe.Inserir(val);
            }
        }

        private void SalvarCIOT(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT repMDFeCIOT = new Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT(unidadeTrabalho);

            foreach (Dominio.ObjetosDeValor.MDFe.CIOT ciot in listaCIOT)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT cio = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT();

                cio.CargaMDFeManual = cargaMDFeManual;
                cio.NumeroCIOT = ciot.Numero;
                cio.Responsavel = Utilidades.String.OnlyNumbers(ciot.CNPJCPFResponsavel);

                repMDFeCIOT.Inserir(cio);
            }
        }

        private void SalvarDestinos(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas.ToList())
            {
                int empresa = carga.Empresa.Codigo;
                if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora.Codigo;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCargaParaMDFe(carga.Codigo, empresa, tipoServicoMultisoftware, true, false, true, true);
                ctes.AddRange((from obj in cargaCTEs select obj.CTe).ToList());
            }

            List<Dominio.Entidades.Localidade> localidadesDestino = (from obj in ctes select obj.LocalidadeTerminoPrestacao).Distinct().ToList();

            foreach (Dominio.Entidades.Localidade localidade in localidadesDestino)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino destino = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino();

                destino.CargaMDFeManual = cargaMDFeManual;
                destino.Localidade = localidade;
                destino.Ordem = 0;

                repCargaMDFeManualDestino.Inserir(destino);
            }
        }

        private bool GerarMDFe(out string erro, int codigoMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoMDFeManual);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string retorno = svcMDFe.EmitirMDFe(cargaMDFeManual, configuracaoTMS, tipoServicoMultisoftware, webServiceConsultaCTe, unidadeTrabalho);

            if (string.IsNullOrWhiteSpace(retorno))
            {
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
                erro = string.Empty;
                return true;
            }
            else
            {
                erro = retorno;
                return false;
            }
        }
    }
}
