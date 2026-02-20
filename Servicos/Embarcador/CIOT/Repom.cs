using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CIOT
{
    public class Repom
    {
        #region Métodos Globais 

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao = ObterConfiguracaoRepom(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string mensagemErro;

            ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom;
            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;

            if (ciot.Motorista == null)
                ciot.Motorista = cargaCIOT.Carga.Motoristas.FirstOrDefault();

            if (ciot.Contratante == null)
                ciot.Contratante = cargaCIOT.Carga.Empresa;

            if (ciot.Veiculo == null)
            {
                ciot.Veiculo = cargaCIOT.Carga.Veiculo;
                ciot.VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
            Dominio.Entidades.Veiculo carreta = ciot.VeiculosVinculados.FirstOrDefault();
            Dominio.Entidades.Cliente proprietarioCarreta = null;
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiroCarreta = null;

            if (carreta != null && carreta.Tipo == "T" && carreta.Proprietario != null && carreta.Proprietario.CPF_CNPJ != ciot.Transportador.CPF_CNPJ)
            {
                proprietarioCarreta = carreta.Proprietario;
                modalidadeTerceiroCarreta = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carreta.Proprietario, unitOfWork);
            }

            bool sucesso = false;

            if (IntegrarProprietario(ciot.Transportador, modalidadeTerceiro, configuracao, unitOfWork, out mensagemErro) &&
                IntegrarProprietario(proprietarioCarreta, modalidadeTerceiroCarreta, configuracao, unitOfWork, out mensagemErro) &&
                IntegrarMotorista(ciot.Transportador, ciot.Motorista, configuracao, unitOfWork, out mensagemErro) &&
                IntegrarVeiculo(ciot.Transportador, ciot.Veiculo, configuracao, unitOfWork, out mensagemErro) &&
                IntegrarCarreta(ciot.Transportador, carreta, configuracao, unitOfWork, out mensagemErro) &&
                ConsultarRoteiro(cargaCIOT.Carga.Rota, configuracao, unitOfWork, out mensagemErro, out string codigoRoteiro, out string codigoPercurso) &&
                IntegrarContratoFrete(cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, configuracao, unitOfWork, out mensagemErro))
                sucesso = true;

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao = ObterConfiguracaoRepom(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string xmlEnvio = Utilidades.XML.Serializar(ObterQuitacaoContratoFrete(cargaCIOT, configuracao, unitOfWork));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.QuitaContrato(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            if (retorno)
                ciotIntegracaoArquivo.Mensagem = "Envio da quitação do CIOT.";
            else
                ciotIntegracaoArquivo.Mensagem = "Falha na quitação do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoQuitacaoContratoFrete retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoQuitacaoContratoFrete>(xmlSucesso);

                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                cargaCIOT.CIOT.Mensagem = "Quitação realizada com sucesso.";
                cargaCIOT.CIOT.DataEncerramento = DateTime.Now;

                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao = ObterConfiguracaoRepom(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string xmlEnvio = Utilidades.XML.Serializar(ObterCancelamentoContratoFrete(usuario, cargaCIOT, configuracao, unitOfWork));

            ServicoRepom.Expedicao.ExpedicaoSoapClient svcExpedicaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Expedicao.ExpedicaoSoapClient, ServicoRepom.Expedicao.ExpedicaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Expedicao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcExpedicaoRepom.CancelaContrato(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlErro);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = "Envio do cancelamento do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (retorno)
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                cargaCIOT.CIOT.Mensagem = "Cancelamento realizado com sucesso.";
                cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool InterromperCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            //mensagemErro = "Deve ser implementado o comportamento da interrupção do contrato.";
            //return false;

            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao = ObterConfiguracaoRepom(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string xmlEnvio = Utilidades.XML.Serializar(ObterInterrupcaoContratoFrete(cargaCIOT, configuracao, unitOfWork));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.InterrompeContrato(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = "Envio da interrupção do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (retorno)
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                cargaCIOT.CIOT.Mensagem = "Interrupção realizada com sucesso.";
                cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool IntegrarProprietario(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            if (proprietario == null)
                return true;

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do transportador não está configurada.";
                return false;
            }

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            try
            {
                string xmlEnvio = Utilidades.XML.Serializar(ObterContratado(proprietario, modalidade, unitOfWork));

                ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

                bool retorno = svcIntegracaoRepom.IntegraDadosCadastroNacionalANTT(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, null, null, null, ref xmlSucesso, ref xmlErro);

                if (retorno)
                {
                    return true;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                    if (retornoErro.Erros != null && retornoErro.Erros.Length > 0)
                    {
                        mensagemErro = "Falha no cadastro do contratado na Repom: " + string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro>(xmlSucesso);

                        mensagemErro = "Falha no cadastro do contratado na ANTT: " + string.Join(", ", retornoSucesso.Contratado.ANTT.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro(xmlErro + xmlSucesso);

                mensagemErro = "Ocorreu uma falha ao integrar os dados dos proprietários do CIOT.";
                return false;
            }
        }

        public bool IntegrarMotorista(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            string xmlEnvio = Utilidades.XML.Serializar(ObterMotorista(motorista, proprietario));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.IntegraDadosCadastroNacionalANTT(configuracao.CodigoCliente, configuracao.AssinaturaDigital, null, xmlEnvio, null, null, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = "Falha no cadastro do motorista na Repom: " + string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool IntegrarVeiculo(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            string xmlEnvio = Utilidades.XML.Serializar(ObterVeiculo(veiculo, proprietario));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.IntegraDadosCadastroNacionalANTT(configuracao.CodigoCliente, configuracao.AssinaturaDigital, null, null, xmlEnvio, null, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                if (retornoErro.Erros != null && retornoErro.Erros.Length > 0)
                    mensagemErro = "Falha no cadastro do veículo na Repom: " + string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro>(xmlSucesso);

                    mensagemErro = "Falha no cadastro do veículo na ANTT: " + string.Join(", ", retornoSucesso.Veiculo.ANTT.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                }

                return false;
            }
        }

        public bool IntegrarCarreta(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            if (veiculo == null)
                return true;

            string xmlEnvio = Utilidades.XML.Serializar(ObterCarreta(veiculo, proprietario));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.IntegraDadosCadastroNacionalANTT(configuracao.CodigoCliente, configuracao.AssinaturaDigital, null, null, null, xmlEnvio, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                if (retornoErro.Erros != null && retornoErro.Erros.Length > 0)
                    mensagemErro = "Falha no cadastro da carreta na Repom: " + string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoCadastro>(xmlSucesso);

                    mensagemErro = "Falha no cadastro da carreta na ANTT: " + string.Join(", ", retornoSucesso.Carreta.ANTT.Erros.Select(o => o.Codigo + " - " + o.Descricao));
                }

                return false;
            }
        }

        public bool ConsultarRoteiro(Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string codigoRoteiro, out string codigoPercurso)
        {
            codigoPercurso = null;
            codigoRoteiro = null;
            mensagemErro = null;

            if (rota == null)
            {
                mensagemErro = "A rota não foi informada.";
                return false;
            }

            Dominio.Entidades.Localidade origem = null;
            Dominio.Entidades.Localidade destino = null;

            if (rota.Remetente != null)
                origem = rota.Remetente.Localidade;
            else if (rota.LocalidadesOrigem?.Count > 0)
                origem = rota.LocalidadesOrigem.FirstOrDefault();

            if (rota.Destinatarios?.Count > 0)
                destino = rota.Destinatarios.First().Cliente.Localidade;
            else if (rota.Localidades?.Count > 0)
                destino = rota.Localidades.FirstOrDefault()?.Localidade;

            if (origem == null)
            {
                mensagemErro = "Não foi encontrada uma origem na rota da carga.";
                return false;
            }

            if (destino == null)
            {
                mensagemErro = "Não foi encontrado um destino na rota da carga.";
                return false;
            }

            string xmlSucesso = null, xmlErro = null;

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            int codigoIBGEOrigem = origem.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt();
            int codigoIBGEDestino = destino.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt();

            bool retorno = svcIntegracaoRepom.ConsultaRoteiros(configuracao.CodigoCliente, configuracao.AssinaturaDigital, origem.Estado.Sigla, "", "", codigoIBGEOrigem.ToString(), destino.Estado.Sigla, "", "", codigoIBGEDestino.ToString(), ref xmlSucesso, ref xmlErro, string.Empty);

            if (retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiro retornoConsultaRoteiro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiro>(xmlSucesso);

                if (retornoConsultaRoteiro.Roteiros == null || retornoConsultaRoteiro.Roteiros.Length <= 0)
                {
                    if (!CadastrarRoteiro(configuracao, rota, origem, destino, unitOfWork, out mensagemErro))
                        return false;
                    else
                    {
                        mensagemErro = "Solicitado o cadastro do roteiro para a Repom, aguarde.";
                        return false;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiroRoteiro roteiroUtilizar = retornoConsultaRoteiro.Roteiros.Where(o => o.CodigoClienteRoteiro == rota.Codigo.ToString()).FirstOrDefault();

                if (roteiroUtilizar == null)
                {
                    //roteiroUtilizar = retornoConsultaRoteiro.Roteiros.FirstOrDefault();

                    if (!CadastrarRoteiro(configuracao, rota, origem, destino, unitOfWork, out mensagemErro))
                        return false;
                    else
                    {
                        mensagemErro = "Solicitado o cadastro do roteiro para a Repom, aguarde.";
                        return false;
                    }
                }

                codigoRoteiro = roteiroUtilizar.CodigoRoteiro;
                codigoPercurso = roteiroUtilizar.CodigoPercurso;

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = "Falha no cadastro da carreta na Repom: " + string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool CadastrarRoteiro(Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            string xmlEnvio = Utilidades.XML.Serializar(ObterCadastroRoteiro(configuracao, rota, origem, destino));

            string xmlRetorno = string.Empty;

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            bool retorno = svcIntegracaoRepom.SolicitaRoteiro(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlRetorno);

            if (!retorno)
                mensagemErro = $"Não foi possível solicitar o cadastro do roteiro de {origem.DescricaoCidadeEstado} até {destino.DescricaoCidadeEstado} para a Repom.";

            return retorno;
        }

        public bool IntegrarContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            string xmlEnvio = Utilidades.XML.Serializar(ObterContratoFrete(cargaCIOT, codigoRoteiro, codigoPercurso, configuracao, unitOfWork));

            ServicoRepom.Expedicao.ExpedicaoSoapClient svcExpedicaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Expedicao.ExpedicaoSoapClient, ServicoRepom.Expedicao.ExpedicaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Expedicao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcExpedicaoRepom.EmiteContrato(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            if (retorno)
                ciotIntegracaoArquivo.Mensagem = "Envio realizado com sucesso.";
            else
                ciotIntegracaoArquivo.Mensagem = "Falha no envio.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoContratoFrete retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoContratoFrete>(xmlSucesso);

                cargaCIOT.CIOT.Numero = retornoSucesso.DadosViagem.CIOT.Substring(0, 12);
                cargaCIOT.CIOT.CodigoVerificador = retornoSucesso.DadosViagem.CIOT.Substring(12, 4);
                cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                cargaCIOT.CIOT.ProtocoloAutorizacao = retornoSucesso.CodigoProcessoTransporte;
                cargaCIOT.CIOT.Mensagem = "CIOT processado com sucesso.";

                repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public string ObterCaminhoConciliacaoFinanceira(DateTime dataMovimento, Repositorio.UnitOfWork unitOfWork)
        {            
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Integrações", "Repom", "Financeiro");

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, dataMovimento.ToString("yyyy-MM-dd") + ".txt");

            return caminho;
        }

        public string ObterCaminhoConciliacaoContabil(DateTime dataMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Integrações", "Repom", "Contábil");

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, dataMovimento.ToString("yyyy-MM-dd") + ".txt");

            return caminho;
        }

        public bool IntegrarConciliacaoFinanceira(DateTime dataMovimento, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(null, null, unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracaoRepom = ObterConfiguracaoRepom(configuracaoCIOT, unitOfWork);

            ServicoRepom.Conciliacao.ConciliacaoSoapClient svcConciliacao = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Conciliacao.ConciliacaoSoapClient, ServicoRepom.Conciliacao.ConciliacaoSoap>( Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Conciliacao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcConciliacao.EmiteFinanceiro(configuracaoRepom.CodigoCliente, configuracaoRepom.AssinaturaDigital, dataMovimento.ToString("dd/MM/yyyy"), ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                string caminho = ObterCaminhoConciliacaoFinanceira(dataMovimento, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoMovimentoFinanceiro retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoMovimentoFinanceiro>(xmlSucesso);

                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, retornoSucesso.Financeiro);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool IntegrarConciliacaoContabil(DateTime dataMovimento, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(null, null, unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracaoRepom = ObterConfiguracaoRepom(configuracaoCIOT, unitOfWork);

            ServicoRepom.Conciliacao.ConciliacaoSoapClient svcConciliacao = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Conciliacao.ConciliacaoSoapClient, ServicoRepom.Conciliacao.ConciliacaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Conciliacao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcConciliacao.EmiteContabil(configuracaoRepom.CodigoCliente, configuracaoRepom.AssinaturaDigital, dataMovimento.ToString("dd/MM/yyyy"), ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                string caminho = ObterCaminhoConciliacaoContabil(dataMovimento, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoMovimentoContabil retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoMovimentoContabil>(xmlSucesso);

                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, retornoSucesso.Contabil);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            string xmlEnvio = Utilidades.XML.Serializar(ObterConsultaValorPedagio(cargaCIOT, codigoRoteiro, codigoPercurso));

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.RoteiroValorTotalVPRs(configuracao.CodigoCliente, configuracao.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaValorPedagio retornoSucesso = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaValorPedagio>(xmlSucesso);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracaoCIOTRepom = ObterConfiguracaoRepom(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlEnvio = Utilidades.XML.Serializar(ObterMovimentoFinanceiro(cargaCIOT.CIOT, configuracaoCIOTRepom, justificativa, valorMovimento, unitOfWork));

            string xmlErro = string.Empty;

            bool retorno = svcIntegracaoRepom.IncluiMovimento(configuracaoCIOTRepom.CodigoCliente, configuracaoCIOTRepom.AssinaturaDigital, xmlEnvio, ref xmlErro);

            if (retorno)
                mensagemErro = "Movimento financeiro integrado com sucesso.";
            else
                mensagemErro = "Falha na integração do movimento financeiro.";

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (!retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));
            }

            return retorno;
        }

        public bool IntegrarAutorizacaoPagamento(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracaoCIOTRepom = ObterConfiguracaoRepom(ciot.ConfiguracaoCIOT, unitOfWork);

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string xmlEnvio = Utilidades.XML.Serializar(ObterAutorizaContrato(ciot, usuario, configuracaoCIOTRepom, unitOfWork));

            string xmlErro = string.Empty, xmlRetorno = string.Empty;

            bool retorno = svcIntegracaoRepom.AutorizaPagamento(configuracaoCIOTRepom.CodigoCliente, configuracaoCIOTRepom.AssinaturaDigital, xmlEnvio, ref xmlRetorno, ref xmlErro);

            if (retorno)
                mensagemErro = "Autorizacao de pagamento integrada com sucesso.";
            else
                mensagemErro = "Falha na integração da autorização de pagamento.";

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            unitOfWork.Start();

            if (!retorno)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                ciot.Mensagem = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoAutorizaContratos retornoAutorizaContratos = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoAutorizaContratos>(xmlRetorno);

                if (retornoAutorizaContratos != null && retornoAutorizaContratos.AutorizaContratos != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoAutorizaContrato retornoAutorizaContrato = retornoAutorizaContratos.AutorizaContratos.FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(retornoAutorizaContrato.CodigoContrato) && !string.IsNullOrWhiteSpace(retornoAutorizaContrato.DataPagamento) && !string.IsNullOrWhiteSpace(retornoAutorizaContrato.ValorFinal))
                    {
                        ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                        ciot.Mensagem = "Pagamento autorizado com sucesso.";
                        ciot.DataAutorizacaoPagamento = DateTime.Now;
                    }
                    else
                    {
                        ciot.Mensagem = retornoAutorizaContrato.CodigoOcorrencia + " - " + retornoAutorizaContrato.DescricaoOcorrencia;
                    }
                }
            }

            repCIOT.Atualizar(ciot);

            unitOfWork.CommitChanges();

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ConsultaValorPedagio ObterConsultaValorPedagio(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string codigoRoteiro, string codigoPercurso)
        {
            Dominio.Entidades.Veiculo carreta = cargaCIOT.Carga.VeiculosVinculados.FirstOrDefault();

            int numeroEixos = (carreta?.ModeloVeicularCarga?.NumeroEixos ?? 0) + (cargaCIOT.Carga.Veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ConsultaValorPedagio consultaValorPedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ConsultaValorPedagio()
            {
                CodigoPercurso = codigoPercurso,
                CodigoRoteiro = codigoRoteiro,
                CodigoRoteiroCliente = cargaCIOT.Carga.Rota?.Codigo.ToString(),
                NumeroEixos = numeroEixos.ToString()
            };

            if (cargaCIOT.Carga.Rota != null)
            {
                int eixosSuspensos = (carreta?.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0) + (cargaCIOT.Carga.Veiculo.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0);

                if ((cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida || cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta) && cargaCIOT.Carga.Rota.TipoCarregamentoIda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    consultaValorPedagio.EixosSuspensosIda = eixosSuspensos.ToString();
                else
                    consultaValorPedagio.EixosSuspensosIda = "0";

                if (cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta && cargaCIOT.Carga.Rota.TipoCarregamentoVolta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    consultaValorPedagio.EixosSuspensosVolta = eixosSuspensos.ToString();
                else
                    consultaValorPedagio.EixosSuspensosVolta = "0";
            }

            return consultaValorPedagio;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.InterrompeContrato ObterInterrupcaoContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.InterrompeContrato interrompeContratoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.InterrompeContrato()
            {
                CodigoFilialCliente = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                CodigoProcessoCliente = cargaCIOT.CIOT.Codigo.ToString(),
                CodigoProcessoTransporte = cargaCIOT.CIOT.ProtocoloAutorizacao,
                Motivo = "83", //Processo errôneo (há mais códigos disponíveis na Repom, por hora enviando fixo)
                Observacao = string.IsNullOrWhiteSpace(cargaCIOT.CIOT.MotivoCancelamento) ? "Processo errôneo" : cargaCIOT.CIOT.MotivoCancelamento
            };

            return interrompeContratoRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro ObterCadastroRoteiro(Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            int codigoIBGEOrigem = origem.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt();
            int codigoIBGEDestino = destino.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt();

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro cadastroRoteiroRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro()
            {
                Roteiro = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiro()
                {
                    AlteraRoteiro = "1",
                    CEPDestino = "",
                    CEPOrigem = "",
                    CodigoFilial = "",
                    CodigoLocalQuitacao = "",
                    CodigoRota = rota.Codigo.ToString(),
                    EmailUsuario = string.Empty,
                    EstadoDestino = destino.Estado.Sigla,
                    EstadoOrigem = origem.Estado.Sigla,
                    IBGEDestino = codigoIBGEDestino.ToString(),
                    IBGEOrigem = codigoIBGEOrigem.ToString(),
                    IdaVolta = rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta ? "1" : "0",
                    NomeUsuario = "",
                    Observacao = "",
                    TelefoneUsuario = "",
                    TempoPrevistoViagem = "",
                    TipoLocalQuitacao = "",
                    TipoProcessoTransporte = "0",
                    Vias = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiroVia[] {
                        //new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiroVia()
                        //{
                        //     Descricao = rota.Detalhes
                        //}
                    }
                }
            };

            return cadastroRoteiroRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CancelamentoContratoFrete ObterCancelamentoContratoFrete(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            string login = usuario?.Nome;

            if (string.IsNullOrWhiteSpace(login))
                login = Utilidades.String.Left(cargaCIOT.Carga.Operador?.Nome, 40);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CancelamentoContratoFrete cancelamentoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CancelamentoContratoFrete()
            {
                CodigoCliente = configuracao.CodigoCliente,
                CodigoFilialCliente = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                CodigoProcessoCliente = cargaCIOT.CIOT.Codigo.ToString(),
                CodigoProcessoTransporte = cargaCIOT.CIOT.ProtocoloAutorizacao,
                Login = login
            };

            return cancelamentoRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.QuitacaoContratoFrete ObterQuitacaoContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.QuitacaoContratoFrete quitacaoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.QuitacaoContratoFrete()
            {
                CodigoCliente = configuracao.CodigoCliente,
                CodigoFilial = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                CodigoFilialQuitacao = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                CodigoProcessoCliente = cargaCIOT.CIOT.Codigo.ToString(),
                CodigoProcessoTransporte = cargaCIOT.CIOT.ProtocoloAutorizacao,
                DataPrevistaPagamento = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(cargaCIOT.ContratoFrete).ToString("dd/MM/yyyy"),
                TipoQuitacao = "0",
                Avarias = "",
                Ocorrencias = "",
                PesoEntrega = "",
                PlanoPagamento = ""
            };

            return quitacaoRepom;
        }

        public Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFrete ObterContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string codigoRoteiro, string codigoPercurso, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT.Carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Cliente destinatario = null, remetente = null;

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null)
                destinatario = cargaPedido.Recebedor;
            else
                destinatario = cargaPedido.Pedido.Destinatario;

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido.Expedidor != null)
                remetente = cargaPedido.Expedidor;
            else
                remetente = cargaPedido.Pedido.Remetente;

            Dominio.Entidades.Veiculo carreta = cargaCIOT.CIOT.VeiculosVinculados.FirstOrDefault();

            string rntrcCarreta = string.Empty;

            if (carreta != null && carreta.Proprietario != null && carreta.Tipo == "T" && carreta.Proprietario?.CPF_CNPJ != cargaCIOT.CIOT.Transportador.CPF_CNPJ)
                rntrcCarreta = string.Format("{0:00000000}", carreta.RNTRC);

            DateTime dataSaida = DateTime.Now.AddHours(1);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFrete contratoFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFrete()
            {
                ConfiguracoesViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteConfiguracoesViagem()
                {
                    DataSaida = dataSaida.ToString("dd/MM/yyyy"),
                    HoraSaida = dataSaida.ToString("HH:mm")
                },
                DadosCarga = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDadosCarga()
                {
                    CodigoNCMMercadoria = !string.IsNullOrWhiteSpace(cargaCIOT.Carga.TipoDeCarga?.NCM) && cargaCIOT.Carga.TipoDeCarga.NCM.Length >= 4 ? cargaCIOT.Carga.TipoDeCarga.NCM.Substring(0, 4) : "",
                    CPFCNPJDestinatario = destinatario.CPF_CNPJ_SemFormato,
                    NomeDestinatario = Utilidades.String.Left(destinatario.Nome, 40),
                    Peso = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo).ToString("F2", cultura),
                    UnidadeMedida = "7",
                    ValorMercadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(cargaCIOT.Carga.Codigo).ToString("F2", cultura),
                    Volume = "",
                    CodigoTipoCarga = "01"
                },
                DadosContratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDadosContratado()
                {
                    CPFCNPJContratado = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                    CPFMotorista = cargaCIOT.CIOT.Motorista.CPF,
                    PlacaCavalo = cargaCIOT.CIOT.Veiculo.Placa,
                    PlacaCarreta = carreta?.Placa ?? "",
                    NumeroEixosCarreta = carreta?.ModeloVeicularCarga?.NumeroEixos.ToString() ?? "",
                    RNTRCCarreta = rntrcCarreta
                },
                DadosFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDadosFrete()
                {
                    ValorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento.ToString("F2", cultura),
                    ValorFrete = (cargaCIOT.ContratoFrete.ValorLiquidoSemAdiantamentoEImpostos - cargaCIOT.ContratoFrete.ValorTotalDescontoSaldo - cargaCIOT.ContratoFrete.ValorTotalDescontoAdiantamento + cargaCIOT.ContratoFrete.ValorTotalAcrescimoSaldo + cargaCIOT.ContratoFrete.ValorTotalAcrescimoAdiantamento).ToString("F2", cultura)
                },
                DadosOperacionais = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDadosOperacionais()
                {
                    CodigoCartao = cargaCIOT.CIOT.Motorista.NumeroCartao,
                    CodigoFilialCliente = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                    CodigoOperacao = cargaCIOT.Carga.TipoOperacao?.CodigoIntegracaoRepom,
                    CodigoPercurso = codigoPercurso,
                    CodigoRoteiro = codigoRoteiro,
                    Usuario = Utilidades.String.Left(cargaCIOT.Carga.Operador?.Nome, 50),
                    CodigoProcessoCliente = cargaCIOT.CIOT.Codigo.ToString(),
                    CodigoRoteiroCliente = string.Empty,
                    CodigoSolicitacaoRoteiro = string.Empty,
                    RoteiroIdaVolta = string.Empty,
                    CEPOrigem = Utilidades.String.OnlyNumbers(remetente.CEP),
                    CEPDestino = Utilidades.String.OnlyNumbers(destinatario.CEP),
                    DistanciaPercorrida = cargaCIOT.Carga.DadosSumarizados.Distancia.ToString("F0", cultura),
                    RoteiroPagamentoPedagio = "0"
                },
                DocumentosIntegrados = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDocumentosIntegrados
                {
                    CTes = cargaCIOT.Carga.CargaCTes.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDocumentosIntegradosConhecimento
                    {
                        CodigoFilialEmissora = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom,
                        Numero = o.CTe.Numero.ToString(),
                        Serie = o.CTe.Serie.Numero.ToString(),
                        TipoDocumento = "0",
                        NotasFiscais = (from nota in o.NotasFiscais
                                        select new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteDocumentosIntegradosNotaFiscal
                                        {
                                            CNPJDestinatario = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato,
                                            CNPJRemetente = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente?.CPF_CNPJ_SemFormato,
                                            NomeDestinatario = Utilidades.String.Left(nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.Nome, 40),
                                            NomeRemetente = Utilidades.String.Left(nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente?.Nome, 40),
                                            Numero = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                                            Serie = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave
                                        }).ToArray()
                    }).ToArray()
                },
                Movimentos = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento[] { }
            };

            if (cargaCIOT.ContratoFrete.ReterImpostosContratoFrete && cargaCIOT.ContratoFrete.TransportadorTerceiro?.Tipo == "F")
            {
                List<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento> movimentos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento>();

                if (!string.IsNullOrWhiteSpace(configuracao.CodigoMovimentoIR) && cargaCIOT.ContratoFrete.ValorIRRF > 0m)
                {
                    movimentos.Add(new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento()
                    {
                        CodigoMovimento = string.Empty,
                        CodigoMovimentoCliente = configuracao.CodigoMovimentoIR,
                        Valor = cargaCIOT.ContratoFrete.ValorIRRF.ToString("F2", cultura)
                    });
                }

                if (!string.IsNullOrWhiteSpace(configuracao.CodigoMovimentoINSS) && cargaCIOT.ContratoFrete.ValorINSS > 0m)
                {
                    movimentos.Add(new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento()
                    {
                        CodigoMovimento = string.Empty,
                        CodigoMovimentoCliente = configuracao.CodigoMovimentoINSS,
                        Valor = cargaCIOT.ContratoFrete.ValorINSS.ToString("F2", cultura)
                    });
                }

                if (!string.IsNullOrWhiteSpace(configuracao.CodigoMovimentoSEST) && cargaCIOT.ContratoFrete.ValorSEST > 0m)
                {
                    movimentos.Add(new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento()
                    {
                        CodigoMovimento = string.Empty,
                        CodigoMovimentoCliente = configuracao.CodigoMovimentoSEST,
                        Valor = cargaCIOT.ContratoFrete.ValorSEST.ToString("F2", cultura)
                    });
                }

                if (!string.IsNullOrWhiteSpace(configuracao.CodigoMovimentoSENAT) && cargaCIOT.ContratoFrete.ValorSENAT > 0m)
                {
                    movimentos.Add(new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteMovimento()
                    {
                        CodigoMovimento = string.Empty,
                        CodigoMovimentoCliente = configuracao.CodigoMovimentoSENAT,
                        Valor = cargaCIOT.ContratoFrete.ValorSENAT.ToString("F2", cultura)
                    });
                }

                if (movimentos.Count > 0)
                    contratoFrete.Movimentos = movimentos.ToArray();
            }

            if (cargaCIOT.Carga.Rota != null)
            {
                int eixosSuspensos = (carreta?.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0) + (cargaCIOT.Carga.Veiculo.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0);

                if ((cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida || cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta) && cargaCIOT.Carga.Rota.TipoCarregamentoIda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    contratoFrete.DadosContratado.EixosSuspensosIda = eixosSuspensos.ToString();
                else
                    contratoFrete.DadosContratado.EixosSuspensosIda = "0";

                if (cargaCIOT.Carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta && cargaCIOT.Carga.Rota.TipoCarregamentoVolta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    contratoFrete.DadosContratado.EixosSuspensosVolta = eixosSuspensos.ToString();
                else
                    contratoFrete.DadosContratado.EixosSuspensosVolta = "0";
            }

            if (!string.IsNullOrWhiteSpace(configuracao.CNPJIntegrador))
            {
                contratoFrete.EmissorDocumento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ContratoFreteEmissorDocumentos();
                contratoFrete.EmissorDocumento.CNPJIntegrador = configuracao.CNPJIntegrador;
            }

            return contratoFrete;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Carreta ObterCarreta(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Cliente contratado)
        {
            Dominio.Entidades.Cliente proprietario = null;

            if (veiculo.Tipo == "T" && veiculo.Proprietario != null && veiculo.Proprietario.CPF_CNPJ != contratado.CPF_CNPJ)
                proprietario = veiculo.Proprietario;
            else
                proprietario = contratado;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Carreta veiculoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Carreta()
            {
                Ano = veiculo.AnoFabricacao.ToString(),
                Cidade = proprietario.Localidade?.Descricao,
                Cor = Utilidades.String.Left(veiculo?.CorVeiculo?.Descricao, 30),
                Estado = proprietario.Localidade?.Estado?.Sigla,
                RNTRC = veiculo.RNTRC.ToString("00000000"),
                CPFCNPJContratado = proprietario.CPF_CNPJ_SemFormato,
                NumeroChassis = Utilidades.String.Left(veiculo.Chassi, 20),
                NumeroEixos = (veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                Placa = veiculo.Placa,
                RENAVAM = veiculo.Renavam
            };

            return veiculoRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Veiculo ObterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Cliente contratado)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Veiculo veiculoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Veiculo()
            {
                Ano = veiculo.AnoFabricacao.ToString(),
                Cidade = contratado.Localidade?.Descricao,
                Cor = veiculo?.CorVeiculo?.Descricao,
                Estado = contratado.Localidade?.Estado?.Sigla,
                RNTRC = veiculo.RNTRC.ToString("00000000"),
                CPFCNPJContratado = contratado.CPF_CNPJ_SemFormato,
                Marca = veiculo.Marca?.Descricao,
                Modelo = veiculo.Modelo?.Descricao,
                NumeroChassis = veiculo.Chassi,
                NumeroEixos = (veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                Placa = veiculo.Placa,
                RENAVAM = veiculo.Renavam,
                TipoVeiculo = veiculo.ModeloVeicularCarga?.TipoVeiculoRepom ?? string.Empty,
                Peso = "",
                CodigoRastreador = "",
                Rastreador = "0",
                SemiReboque = "",
                Volume = ""
            };

            return veiculoRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Motorista ObterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Cliente contratado)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Motorista motoristaRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Motorista()
            {
                Bairro = motorista.Bairro,
                CategoriaCNH = motorista.Categoria,
                CEP = Utilidades.String.OnlyNumbers(motorista.CEP),
                Cidade = Utilidades.String.Left(motorista.Localidade?.Descricao, 30),
                CNH = motorista.NumeroHabilitacao,
                CPF = motorista.CPF,
                CPFCNPJContratado = contratado.CPF_CNPJ_SemFormato,
                DataEmissaoCNH = motorista.DataHabilitacao?.ToString("dd/MM/yyyy"),
                NomeMae = "",
                NomePai = "",
                DataEmissaoRG = motorista.DataEmissaoRG?.ToString("dd/MM/yyyy"),
                Celular = "",
                OrgaoEmissorRG = motorista.OrgaoEmissorRG?.ToString("G"),
                DataNascimento = motorista.DataNascimento?.ToString("dd/MM/yyyy"),
                DataPrimeiraCNH = motorista.DataPrimeiraHabilitacao?.ToString("dd/MM/yyyy"),
                DataValidadeCNH = motorista.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy"),
                Email = Utilidades.String.Left(motorista.Email, 40),
                Endereco = Utilidades.String.Left(motorista.Endereco, 30),
                Estado = motorista.Localidade?.Estado?.Sigla,
                EstadoEmissorRG = motorista.EstadoRG?.Sigla,
                Nome = Utilidades.String.Left(motorista.Nome, 40),
                RG = motorista.RG,
                Telefone = Utilidades.String.Left(motorista.Telefone, 20),
                Naturalidade = Utilidades.String.Left(motorista.Localidade?.Descricao, 30),
                EstadoNaturalidade = motorista.Localidade?.Estado?.Sigla
            };

            return motoristaRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Contratado ObterContratado(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente contratado = repCliente.BuscarPorCPFCNPJ(cliente.CPF_CNPJ);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Contratado contratadoRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.Contratado()
            {
                CPFCNPJ = contratado.CPF_CNPJ_SemFormato,
                Tipo = contratado.Tipo == "F" ? "0" : contratado.RegimeTributario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.SimplesNacional ? "1" : "2",
                NomeFantasia = contratado.NomeFantasia,
                RazaoSocial = contratado.Nome,
                CEP = Utilidades.String.OnlyNumbers(contratado.CEP),
                Endereco = Utilidades.String.Left(contratado.Endereco + ", " + contratado.Numero + ", " + contratado.Complemento, 40),
                Bairro = Utilidades.String.Left(contratado.Bairro, 20),
                Cidade = Utilidades.String.Left(contratado.Localidade.Descricao, 30),
                Estado = contratado.Localidade.Estado.Sigla,
                Contato = Utilidades.String.Left(contratado.Contatos?.Count > 0 ? contratado.Contatos?.FirstOrDefault()?.Contato : contratado.Nome, 30),
                Dependentes = "0",
                Email = contratado.Email,
                Telefone1 = Utilidades.String.Left(contratado.Telefone1, 20),
                Telefone2 = Utilidades.String.Left(contratado.Telefone2, 20),
                INSSSimplificado = "0",
                Celular = Utilidades.String.Left(contratado.Telefone2, 20)
            };

            if (!string.IsNullOrWhiteSpace(modalidade.RNTRC))
            {
                contratadoRepom.RNTRC = modalidade.RNTRC;

                if (modalidade.DataEmissaoRNTRC.HasValue)
                    contratadoRepom.DataEmissaoRNTRC = modalidade.DataEmissaoRNTRC?.ToString("dd/MM/yyyy");

                if (modalidade.DataVencimentoRNTRC.HasValue)
                    contratadoRepom.DataVencimentoRNTRC = modalidade.DataVencimentoRNTRC?.ToString("dd/MM/yyyy");
            }

            if (!string.IsNullOrWhiteSpace(modalidade.CodigoINSS))
                contratadoRepom.CodigoINSS = modalidade.CodigoINSS;

            if (!string.IsNullOrWhiteSpace(contratado.Agencia) && !string.IsNullOrWhiteSpace(contratado.NumeroConta) && contratado.Banco != null)
            {
                string[] conta = contratado.NumeroConta.Split('-');

                contratadoRepom.DadosBancarios = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.DadosBancarios()
                {
                    Agencia = contratado.Agencia,
                    DigitoVerificadorAgencia = contratado.DigitoAgencia,
                    Banco = contratado.Banco.Numero.ToString("000"),
                    ContaCorrente = conta[0].Trim(),
                    DigitoVerificadorContaCorrente = conta.Length > 1 ? conta[1].Trim() : "",
                    CPFCNPJTitular = contratado.CPF_CNPJ_SemFormato,
                    Titular = contratado.Nome
                };
            }
            else
            {
                contratadoRepom.DadosBancarios = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.DadosBancarios()
                {
                    Agencia = string.Empty,
                    DigitoVerificadorAgencia = string.Empty,
                    Banco = string.Empty,
                    ContaCorrente = string.Empty,
                    DigitoVerificadorContaCorrente = string.Empty,
                    CPFCNPJTitular = contratado.CPF_CNPJ_SemFormato,
                    Titular = contratado.Nome
                };
            }

            return contratadoRepom;
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTRepom ObterConfiguracaoRepom(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTRepom repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepom(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao = repCIOTRepom.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.IncluiMovimentoFinanceiro ObterMovimentoFinanceiro(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.IncluiMovimentoFinanceiro movimentoFinanceiro = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.IncluiMovimentoFinanceiro()
            {
                CodigoCliente = configuracao.CodigoCliente,
                CodigoMovimento = justificativa.CodigoIntegracaoRepom,
                CodigoMovimentoCliente = justificativa.Codigo.ToString(),
                CodigoProcessoTransporte = ciot.ProtocoloAutorizacao,
                Valor = valorMovimento.ToString(cultura),
                TipoOperacao = ""
            };

            return movimentoFinanceiro;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.AutorizaContratos ObterAutorizaContrato(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.CIOT.CIOTRepom configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.AutorizaContratos autorizaContrato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.AutorizaContratos()
            {
                AutorizaContrato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.AutorizaContrato()
                {
                    CodigoContrato = ciot.ProtocoloAutorizacao,
                    CodigoProcessoTransporteCliente = ciot.Codigo.ToString(),
                    CodigoFilial = ciot.Contratante.Configuracao.CodigoFilialRepom,
                    Dias = "0",
                    Usuario = usuario?.Nome ?? "Automático"
                }
            };

            return autorizaContrato;
        }

        #endregion
    }
}
