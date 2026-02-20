using Dominio.Entidades.Embarcador.Pessoas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.SIC
{
    public class IntegracaoSIC
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoSIC(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ConsultarVeiculos(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC ConsultarVeiculos Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            Dominio.Entidades.Veiculo objVeiculoAuditoria = new Dominio.Entidades.Veiculo();
            try
            {
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);

                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                List<ServicoSIC.DadosCadastroIntegracaoDTC> dadosCadastroVeiculos = ObterListaCadastro(configuracaoIntegracaoSIC, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.VEICULO, token);

                List<int> idsRecebidos = new List<int>();

                foreach (ServicoSIC.DadosCadastroIntegracaoDTC cadastroVeiculo in dadosCadastroVeiculos)
                    SalvarDadosVeiculo(cadastroVeiculo, auditado, ref idsRecebidos);

                if (idsRecebidos.Count > 0)
                {
                    ServicoSIC.RespostaDTCint retorno = ConfirmarItensRecebidos(configuracaoIntegracaoSIC, idsRecebidos.ToArray(), Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.VEICULO, token);

                    Log.TratarErro($"{retorno.Mensagem} | Ids de Cadastros Veículo Confirmados: {string.Join(", ", idsRecebidos)} ", "IntegracaoSIC");
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objVeiculoAuditoria, null, $"Erro ao consultar Veiculo pela integração SIC - {excecao.Message}", _unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objVeiculoAuditoria, null, $"Erro ao consultar Veiculo pela integração SIC - {excecao.Message}", _unitOfWork);
            }
            Servicos.Log.TratarErro($"Integração SIC ConsultarVeiculos Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        public void ConsultarMotoristas(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC ConsultarMotoristas Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            try
            {
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);

                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                List<ServicoSIC.DadosCadastroIntegracaoDTC> dadosCadastroMotorista = ObterListaCadastro(configuracaoIntegracaoSIC, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.MOTORISTA, token);

                List<int> idsRecebidos = new List<int>();

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                foreach (ServicoSIC.DadosCadastroIntegracaoDTC cadastroMotorista in dadosCadastroMotorista)
                    SalvarDadosMotorista(cadastroMotorista, auditado, ref idsRecebidos);

                if (idsRecebidos.Count > 0)
                {
                    ServicoSIC.RespostaDTCint retorno = ConfirmarItensRecebidos(configuracaoIntegracaoSIC, idsRecebidos.ToArray(), Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.MOTORISTA, token);

                    Log.TratarErro($"{retorno.Mensagem} | Ids de Cadastros Motorista Confirmados: {string.Join(", ", idsRecebidos)} ", "IntegracaoSIC");
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            Servicos.Log.TratarErro($"Integração SIC ConsultarMotoristas Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        public void ConsultarClientes(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC ConsultarClientes Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            try
            {
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);

                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                List<ServicoSIC.DadosCadastroIntegracaoDTC> dadosCadastroCliente = ObterListaCadastro(configuracaoIntegracaoSIC, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.CLIENTE, token);

                List<int> idsRecebidos = new List<int>();

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                foreach (ServicoSIC.DadosCadastroIntegracaoDTC cadastroCliente in dadosCadastroCliente)
                    SalvarDadosCliente(cadastroCliente, auditado, ref idsRecebidos);

                if (idsRecebidos.Count > 0)
                {
                    ServicoSIC.RespostaDTCint retorno = ConfirmarItensRecebidos(configuracaoIntegracaoSIC, idsRecebidos.ToArray(), Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.CLIENTE, token);

                    Log.TratarErro($"{retorno.Mensagem} | Ids de Cadastros Cliente Confirmados: {string.Join(", ", idsRecebidos)} ", "IntegracaoSIC");
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            Servicos.Log.TratarErro($"Integração SIC ConsultarClientes Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        public void ConsultaInformacaoProtocoloInclusaoCadastro(PessoaIntegracao integracaoPendente, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC ConsultaInformacaoProtocoloInclusaoCadastro Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            Servicos.ServicoSIC.RespostaDTCInformacaoInclusaoCadastroDTC protocoloResponse = null;
            try
            {
                // padrao de integrações 
                integracaoPendente.DataIntegracao = DateTime.Now;
                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                // logica de requisição 
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);
                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                System.ServiceModel.EndpointAddress endpointAddress = ObterEndpoint(configuracaoIntegracaoSIC.URLIntegracaoSIC, false);
                System.ServiceModel.BasicHttpBinding binding = ObterBinding(configuracaoIntegracaoSIC.URLIntegracaoSIC);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicoSIC.IntegracaoClient integracaoClient = new ServicoSIC.IntegracaoClient(binding, endpointAddress);

                InspectorBehavior inspector = new InspectorBehavior();
                integracaoClient.Endpoint.EndpointBehaviors.Add(inspector);
                ServicoSIC.Header header = new ServicoSIC.Header();
                header.TokenAplicacao = token;

                //chamado do serviço
                protocoloResponse = integracaoClient.ObterInformacaoProtocoloInclusaoCadastro(header, integracaoPendente.Protocolo);

                // logica de salvamento
                integracaoPendente.StatusIntegracao = (StatusIntegracaoSIC)protocoloResponse.Retorno.Status;

                if (integracaoPendente.StatusIntegracao == StatusIntegracaoSIC.IntegracaoConcluida)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.Pessoa.CodigoIntegracao = protocoloResponse.Retorno.listaDetalheCadastro.FirstOrDefault().CodigoSap;
                }
                else if (integracaoPendente.StatusIntegracao == StatusIntegracaoSIC.ErroProcessamento)
                {
                    integracaoPendente.ProblemaIntegracao = string.Join(", ", protocoloResponse.Mensagem);
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.NumeroTentativas++;
                }
                else
                {
                    integracaoPendente.ProblemaIntegracao = StatusIntegracaoSIC.IntegracaoConcluida.ObterDescricao();
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                }
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                integracaoPendente.NumeroTentativas++;
            }
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).Atualizar(integracaoPendente);
            Servicos.Log.TratarErro($"Integração SIC ConsultaInformacaoProtocoloInclusaoCadastro Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        public void SalvarListaClienteTerceiro(PessoaIntegracao integracaoPendente, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC SalvarListaClienteTerceiro Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            Servicos.ServicoSIC.RespostaDTCstring protocoloResponse = null;
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                // padrao de integrações 
                integracaoPendente.NumeroTentativas++;
                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                // logica de requisição 
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);
                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                System.ServiceModel.EndpointAddress endpointAddress = ObterEndpoint(configuracaoIntegracaoSIC.URLIntegracaoSIC, false);
                System.ServiceModel.BasicHttpBinding binding = ObterBinding(configuracaoIntegracaoSIC.URLIntegracaoSIC);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicoSIC.IntegracaoClient integracaoClient = new ServicoSIC.IntegracaoClient(binding, endpointAddress);

                integracaoClient.Endpoint.EndpointBehaviors.Add(inspector);
                ServicoSIC.Header header = new ServicoSIC.Header();
                header.TokenAplicacao = token;

                Servicos.ServicoSIC.DadosCadastroIntegracaoDTC[] listaCadastro = ObterPessoa(integracaoPendente.Pessoa);

                //chamada do serviço
                protocoloResponse = integracaoClient.SalvarListaClienteTerceiro(header, listaCadastro);

                // logica de salvamento
                if (string.IsNullOrEmpty(protocoloResponse.Retorno))
                {
                    integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar receber protocolo.";
                    integracaoPendente.Protocolo = "";
                    integracaoPendente.StatusIntegracao = StatusIntegracaoSIC.ErroProcessamento;
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.DataIntegracao = DateTime.Now;
                }
                else
                {
                    integracaoPendente.Protocolo = protocoloResponse.Retorno;
                    integracaoPendente.StatusIntegracao = StatusIntegracaoSIC.ArmazenamentoSolicitado;
                    integracaoPendente.NumeroTentativas = 0;
                    integracaoPendente.ProblemaIntegracao = "";
                }
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
                integracaoPendente.StatusIntegracao = StatusIntegracaoSIC.ErroProcessamento;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                integracaoPendente.StatusIntegracao = StatusIntegracaoSIC.ErroProcessamento;
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).Atualizar(integracaoPendente);
            Servicos.Log.TratarErro($"Integração SIC SalvarListaClienteTerceiro Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        public void ConsultarClientesTerceiro(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            if (configuracaoIntegracaoSIC.TipoCadastroClientesTerceirosSIC)
            {
                Servicos.Log.TratarErro($"Integração SIC ConsultarClientesTerceiros Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
                try
                {
                    ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);

                    string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                    List<ServicoSIC.DadosCadastroIntegracaoDTC> dadosCadastroCliente = ObterListaCadastro(configuracaoIntegracaoSIC, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.CLIENTETERCEIRO, token);

                    List<int> idsRecebidos = new List<int>();

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                    foreach (ServicoSIC.DadosCadastroIntegracaoDTC cadastroCliente in dadosCadastroCliente)
                        SalvarDadosCliente(cadastroCliente, auditado, ref idsRecebidos);

                    if (idsRecebidos.Count > 0)
                    {
                        ServicoSIC.RespostaDTCint retorno = ConfirmarItensRecebidos(configuracaoIntegracaoSIC, idsRecebidos.ToArray(), Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.CLIENTETERCEIRO, token);

                        Log.TratarErro($"{retorno.Mensagem} | Ids de Cadastros Cliente Terceiros Confirmados: {string.Join(", ", idsRecebidos)} ", "IntegracaoSIC");
                    }
                }
                catch (ServicoException excecao)
                {
                    Log.TratarErro(excecao, "IntegracaoSIC");
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "IntegracaoSIC");
                }
                Servicos.Log.TratarErro($"Integração SIC ConsultarClientesTerceiros Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            }
        }

        private string ObterDDDjsl(string fone)
        {
            fone = fone.Trim();
            if (fone.Substring(0, 1) == "(")
                return fone.Substring(1, 2);
            else if (fone.Length > 9)
                return fone.Substring(0, 2);
            else
                return string.Empty;
        }

        private string ObterFoneSemDDDjsl(string fone)
        {
            fone = fone.Trim();
            if (fone.Substring(0, 1) == "(")
                return fone.Substring(4, fone.Length - 4);
            else if (fone.Length > 9)
                return fone.Substring(2, fone.Length - 2);
            else
                return fone;
        }

        private ServicoSIC.DadosCadastroIntegracaoDTC[] ObterPessoa(Dominio.Entidades.Cliente pessoa)
        {
            ServicoSIC.DadosCadastroIntegracaoDTC DadosCadastro = new ServicoSIC.DadosCadastroIntegracaoDTC();
            DadosCadastro.CategoriaClienteDtc = ServicoSIC.ENUMCATEGORIACLIENTEDTC.TERCEIRO;
            DadosCadastro.DadosCadastraisDtc = new ServicoSIC.CadastroIntegracaoDadosCadastraisDTC();
            DadosCadastro.DadosContatoDtc = new ServicoSIC.CadastroIntegracaoDadosContatoDTC();
            DadosCadastro.DadosContatoDtc.ListaInformacoesContato = new Servicos.ServicoSIC.DadosContatoInformacoesContatoDTC[1];
            ServicoSIC.DadosContatoInformacoesContatoDTC InformacoesContato = new ServicoSIC.DadosContatoInformacoesContatoDTC();


            if (pessoa.Tipo == "F")
            {
                DadosCadastro.DadosCadastraisDtc.CONTRIBUINTE_ICMS_SAPID = "99";
                if (pessoa.Localidade.Estado.Sigla == "EX")
                    DadosCadastro.TipoPessoaDtc = ServicoSIC.EnumTipoPessoaDTC.PessoaEstrangeiraFisica;
                else
                {
                    DadosCadastro.TipoPessoaDtc = ServicoSIC.EnumTipoPessoaDTC.PessoaFisica;
                    DadosCadastro.DadosCadastraisDtc.CPF = pessoa.CPF_CNPJ_SemFormato;
                }
            }
            else
            {
                DadosCadastro.DadosCadastraisDtc.CONTRIBUINTE_ICMS_SAPID = "01";
                if (pessoa.Localidade.Estado.Sigla == "EX")
                    DadosCadastro.TipoPessoaDtc = ServicoSIC.EnumTipoPessoaDTC.PessoaEstrangeiraJuridica;
                else
                {
                    DadosCadastro.TipoPessoaDtc = ServicoSIC.EnumTipoPessoaDTC.PessoaJuridica;
                    DadosCadastro.DadosCadastraisDtc.CNPJ = pessoa.CPF_CNPJ_SemFormato;
                }
            }


            DadosCadastro.DadosCadastraisDtc.DESPACHANTE_ADUANEIRO = "0";
            DadosCadastro.DadosCadastraisDtc.INSCRICAOESTADUAL = pessoa.IE_RG;
            DadosCadastro.DadosCadastraisDtc.MICROEMPRESA = "0";
            DadosCadastro.DadosCadastraisDtc.NOMEFANTASIA = pessoa.NomeFantasia;
            DadosCadastro.DadosCadastraisDtc.RAZAOSOCIAL = pessoa.Nome;
            DadosCadastro.Observacao = "CADASTRO INTEGRADO VIA INTEGRA LEGADOS";
            DadosCadastro.Status = 1;
            DadosCadastro.TipoCadastroDtc = ServicoSIC.ENUMTIPOCADASTRODTC.CLIENTETERCEIRO;


            // contanto
            InformacoesContato.CONTATOTELEFONE = "CONTATO";
            InformacoesContato.DDD = ObterDDDjsl(pessoa.Telefone1);
            InformacoesContato.NUMEROTELEFONE = ObterFoneSemDDDjsl(pessoa.Telefone1);
            InformacoesContato.REFERENCIATELEFONE = "REFERENCIA";
            InformacoesContato.TIPOTELEFONE = "FIXO";
            DadosCadastro.DadosContatoDtc.ListaInformacoesContato[0] = InformacoesContato;


            //endereço 
            DadosCadastro.DadosEnderecoDtc = new ServicoSIC.CadastroIntegracaoDadosEnderecoDTC();
            DadosCadastro.DadosEnderecoDtc.BAIRRO = pessoa.Bairro;
            DadosCadastro.DadosEnderecoDtc.CEP = pessoa.CEP;
            DadosCadastro.DadosEnderecoDtc.CODIGOMUNICIPIO = pessoa.Localidade.CodigoIBGE.ToString();
            DadosCadastro.DadosEnderecoDtc.MUNICIPIO = pessoa.Localidade.Descricao.ToString();

            DadosCadastro.DadosEnderecoDtc.COMPLEMENTO = pessoa.Complemento;
            DadosCadastro.DadosEnderecoDtc.LOGRADOURO = pessoa.Endereco;
            DadosCadastro.DadosEnderecoDtc.MESOREGIAO = "";
            DadosCadastro.DadosEnderecoDtc.MICROREGIAO = "";
            DadosCadastro.DadosEnderecoDtc.NUMERO = pessoa.Numero;
            DadosCadastro.DadosEnderecoDtc.PAIS = pessoa.Localidade.Estado.Sigla == "EX" ? pessoa.Localidade.Descricao : "BRASIL";
            DadosCadastro.DadosEnderecoDtc.UF = pessoa.Localidade.Estado.Sigla;

            ServicoSIC.DadosCadastroIntegracaoDTC[] ret = new ServicoSIC.DadosCadastroIntegracaoDTC[1];
            ret[0] = DadosCadastro;

            return ret;
        }

        public void ConsultarTransportadorTerceiro(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC)
        {
            Servicos.Log.TratarErro($"Integração SIC ConsultarTransportadorTerceiro Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
            try
            {
                ValidarConfiguracoesIntegracao(configuracaoIntegracaoSIC);

                string token = ObterTokenAplicacao(configuracaoIntegracaoSIC);

                List<ServicoSIC.DadosCadastroIntegracaoDTC> dadosCadastroCliente = ObterListaCadastro(configuracaoIntegracaoSIC, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.TRANSPORTADORA, token);

                List<int> idsRecebidos = new List<int>();

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                foreach (ServicoSIC.DadosCadastroIntegracaoDTC cadastroCliente in dadosCadastroCliente)
                    SalvarDadosTransportadorTerceiro(cadastroCliente, auditado, ref idsRecebidos);

                if (idsRecebidos.Count > 0)
                {
                    ServicoSIC.RespostaDTCint retorno = ConfirmarItensRecebidos(configuracaoIntegracaoSIC, idsRecebidos.ToArray(), Servicos.ServicoSIC.ENUMTIPOCADASTRODTC.TRANSPORTADORA, token);

                    Log.TratarErro($"{retorno.Mensagem} | Ids de Cadastros Transportador Confirmados: {string.Join(", ", idsRecebidos)} ", "IntegracaoSIC");
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSIC");
            }
            Servicos.Log.TratarErro($"Integração SIC ConsultarTransportadorTerceiro Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "IntegracaoSIC");
        }

        #endregion

        #region Métodos Privados

        #region Requisição
        private void ValidarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracao)
        {
            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoSIC)
                throw new ServicoException("Não existe configuração de integração disponível para SIC");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoSIC))
                throw new ServicoException("A URL não está configurada para a integração com SIC");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.LoginSIC) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaSIC))
                throw new ServicoException("O usuário e a senha devem estar preenchidos na configuração de integração SIC");
        }

        private string ObterTokenAplicacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracao)
        {
            ServicoSICSeguranca.SegurancaClient segurancaClient = ObterClientAutenticacao(configuracaoIntegracao.URLIntegracaoSIC);
            InspectorBehavior inspector = new InspectorBehavior();
            segurancaClient.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoSICSeguranca.Header header = new ServicoSICSeguranca.Header();

            var responseLogIn = segurancaClient.LogInAplicacao(header, configuracaoIntegracao.LoginSIC, configuracaoIntegracao.SenhaSIC);

            return responseLogIn.Retorno.CodigoAcesso;
        }

        private List<ServicoSIC.DadosCadastroIntegracaoDTC> ObterListaCadastro(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracao, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC tipoCadastro, string tokenAplicacao)
        {
            ServicoSIC.IntegracaoClient integracaoClient = ObterClientRequisicao(configuracaoIntegracao.URLIntegracaoSIC);
            InspectorBehavior inspector = new InspectorBehavior();
            integracaoClient.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoSIC.Header header = new ServicoSIC.Header();
            header.TokenAplicacao = tokenAplicacao;

            Servicos.ServicoSIC.ENUMPESSOAEMPRESADTC empresa = configuracaoIntegracao.EmpresaSIC.ToEnum<ServicoSIC.ENUMPESSOAEMPRESADTC>();

            var responseCadastroVeiculos = integracaoClient.ObterListaCadastrosSIC(header, tipoCadastro, empresa);

            if (responseCadastroVeiculos.Retorno != null)
                return responseCadastroVeiculos.Retorno.ToList();

            return new List<ServicoSIC.DadosCadastroIntegracaoDTC>();
        }

        private ServicoSIC.RespostaDTCint ConfirmarItensRecebidos(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracao, int[] idsRecebidos, Servicos.ServicoSIC.ENUMTIPOCADASTRODTC tipoCadastro, string tokenAplicacao)
        {
            ServicoSIC.IntegracaoClient integracaoClient = ObterClientRequisicao(configuracaoIntegracao.URLIntegracaoSIC);
            InspectorBehavior inspector = new InspectorBehavior();
            integracaoClient.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoSIC.Header header = new ServicoSIC.Header();
            header.TokenAplicacao = tokenAplicacao;

            return integracaoClient.ConfirmarRecebimentoListaCadastrosSIC(header, tipoCadastro, idsRecebidos); ;
        }

        private ServicoSICSeguranca.SegurancaClient ObterClientAutenticacao(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = ObterEndpoint(url, true);
            System.ServiceModel.BasicHttpBinding binding = ObterBinding(url);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return new ServicoSICSeguranca.SegurancaClient(binding, endpointAddress);
        }

        private ServicoSIC.IntegracaoClient ObterClientRequisicao(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = ObterEndpoint(url, false);
            System.ServiceModel.BasicHttpBinding binding = ObterBinding(url);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return new ServicoSIC.IntegracaoClient(binding, endpointAddress);
        }

        private System.ServiceModel.BasicHttpBinding ObterBinding(string url)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            binding.SendTimeout = new TimeSpan(0, 20, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return binding;
        }

        private System.ServiceModel.EndpointAddress ObterEndpoint(string url, bool autenticar)
        {
            if (!url.EndsWith("/"))
                url += "/";

            if (autenticar)
                url += "Servicos/Seguranca/SegurancaSVC.svc";
            else
                url += "Servicos/Integracao/IntegracaoSVC.svc";

            return new System.ServiceModel.EndpointAddress(url);
        }
        #endregion

        private void SalvarDadosVeiculo(ServicoSIC.DadosCadastroIntegracaoDTC dadosCadastroVeiculo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, ref List<int> idsRecebidos)
        {
            Dominio.Entidades.Veiculo objVeiculo = new Dominio.Entidades.Veiculo();

            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Servicos.Embarcador.Veiculo.Veiculo serVeiculo = new Servicos.Embarcador.Veiculo.Veiculo(_unitOfWork);

                bool inserir = false;
                objVeiculo = repVeiculo.BuscarPorPlaca(dadosCadastroVeiculo.DadosCadastraisVeiculoDtc.PLACA);

                bool situacaoAnterior = true;
                if (objVeiculo == null)
                {
                    inserir = true;
                    objVeiculo = new Dominio.Entidades.Veiculo();
                    objVeiculo.Placa = dadosCadastroVeiculo.DadosCadastraisVeiculoDtc.PLACA;
                    objVeiculo.Ativo = true;
                }
                else
                {
                    situacaoAnterior = objVeiculo.Ativo;
                }

                objVeiculo.TipoVeiculo = dadosCadastroVeiculo.DadosCadastraisVeiculoDtc.IMPLEMENTO ?? "";
                objVeiculo.TipoRodado = ObterTipoRodado(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.TIPOVEICULO ?? "", objVeiculo.TipoVeiculo);
                objVeiculo.TipoCarroceria = ObterTipoCarroceria(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.CARROCERIA);
                objVeiculo.Renavam = dadosCadastroVeiculo.DadosCadastraisVeiculoDtc.RENAVAM ?? "";
                objVeiculo.ModeloVeicularCarga = ObterModeloVeicularCarga(dadosCadastroVeiculo.DadosCadastraisVeiculoDtc);
                objVeiculo.Chassi = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.CHASSI ?? "";

                objVeiculo.AnoModelo = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.ANOMODELO?.ToInt() ?? 0;
                objVeiculo.AnoFabricacao = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.ANOFABRICACAO?.ToInt() ?? 0;
                objVeiculo.CorVeiculo = ObterCorVeiculo(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.COR ?? "");

                objVeiculo.DataLicenca = dadosCadastroVeiculo.DadosCadastraisVeiculoDtc.VENCIMENTOLICENCIAMENTO.ToDateTime("yyyy-MM-dd HH:mm:ss");

                objVeiculo.TipoCombustivel = ObterTipoCombustivel(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.COMBUSTIVEL ?? "");
                objVeiculo.SegmentoVeiculo = ObterSegmentoVeiculo(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.CATEGORIA ?? "");
                objVeiculo.Estado = ObterEstado(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.UF ?? "");
                objVeiculo.Marca = ObterMarcaVeiculo(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.FABRICANTE ?? "");
                objVeiculo.Modelo = ObterModeloVeiculo(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.MODELO ?? "");
                objVeiculo.CapacidadeKG = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.CAPACIDADE.ToInt() ?? 0;
                objVeiculo.Tara = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.TARA?.ToInt() ?? 0;
                objVeiculo.Proprietario = ObterProprietarioVeiculo(dadosCadastroVeiculo.DadosCadastraisVeiculoDtc);
                objVeiculo.RNTRC = dadosCadastroVeiculo.DadosCadastraisVeiculoDtc?.RNTCTRANSPORTADORA?.ToInt() ?? 0;
                objVeiculo.Tipo = ObterTipoVeiculo(dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.VINCULOVEICULO ?? "");
                objVeiculo.NumeroFrota = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.NUMEROFROTAJSL ?? "";

                string observacaoVeiculo = dadosCadastroVeiculo?.DadosCadastraisVeiculoDtc?.OBSERVACAOLICENCIAMENTO ?? "";
                objVeiculo.Observacao = PreencherObservacaoVeiculo(observacaoVeiculo, dadosCadastroVeiculo.DadosComplementoVeiculoDtc);

                if (objVeiculo.DataLicenca == null || objVeiculo.DataLicenca < DateTime.MinValue.AddYears(1969))
                    objVeiculo.DataLicenca = DateTime.MinValue.AddYears(1969);

                if (!TemErros(dadosCadastroVeiculo, repVeiculo.Validator(objVeiculo)))
                {
                    if (inserir)
                        repVeiculo.Inserir(objVeiculo);
                    else
                    {
                        repVeiculo.Atualizar(objVeiculo);
                        Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(objVeiculo, situacaoAnterior, MetodosAlteracaoVeiculo.SalvarDadosVeiculo_IntegracaoSIC, null, _unitOfWork);
                    }
                    Servicos.Auditoria.Auditoria.Auditar(auditado, objVeiculo, null, "Veiculo recebido pela integração SIC", _unitOfWork);
                    idsRecebidos.Add(dadosCadastroVeiculo.ID);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoSIC");
                Log.TratarErro(JsonConvert.SerializeObject(dadosCadastroVeiculo), "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objVeiculo, null, "Erro ao cadastrar veiculo pela integração SIC", _unitOfWork);
            }
        }

        private void SalvarDadosMotorista(ServicoSIC.DadosCadastroIntegracaoDTC dadosCadastroMotorista, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, ref List<int> idsRecebidos)
        {
            Dominio.Entidades.Usuario objMotorista = new Dominio.Entidades.Usuario();
            try
            {
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);

                bool inserir = false;
                string cpf = dadosCadastroMotorista.DadosCadastraisDtc.CPF.ObterSomenteNumeros();
                objMotorista = repMotorista.BuscarPorCPF(cpf);

                if (objMotorista == null)
                {
                    inserir = true;
                    objMotorista = new Dominio.Entidades.Usuario();
                    objMotorista.CPF = cpf;
                    objMotorista.Tipo = "M";
                    objMotorista.Bloqueado = false;
                }

                objMotorista.RG = dadosCadastroMotorista?.DadosCadastraisDtc?.RG ?? "";
                objMotorista.Nome = dadosCadastroMotorista?.DadosCadastraisDtc?.NOME ?? "";
                if (!string.IsNullOrEmpty(dadosCadastroMotorista?.DadosCadastraisDtc?.ORGAOEMISSOR ?? ""))
                    objMotorista.OrgaoEmissorRG = dadosCadastroMotorista.DadosCadastraisDtc.ORGAOEMISSOR.ToEnum<Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG>();

                objMotorista.EstadoRG = ObterEstado(dadosCadastroMotorista?.DadosCadastraisDtc?.UFEMISSAO ?? "");
                objMotorista.DataEmissaoRG = dadosCadastroMotorista.DadosCadastraisDtc.DATAEMISSAO.ToNullableDateTime("dd/MM/yyyy HH:mm:ss");
                objMotorista.DataNascimento = dadosCadastroMotorista.DadosCadastraisDtc.DATANASCIMENTO.ToNullableDateTime("dd/MM/yyyy HH:mm:ss");
                objMotorista.PIS = dadosCadastroMotorista?.DadosCadastraisDtc?.PIS ?? "";
                objMotorista.NumeroRegistroHabilitacao = dadosCadastroMotorista?.DadosCadastraisDtc?.CNHREGISTRO ?? "";
                objMotorista.RenachHabilitacao = Regex.Replace(dadosCadastroMotorista?.DadosCadastraisDtc?.CNHRENACH ?? "", "[a-zA-Z]", "");
                objMotorista.Categoria = dadosCadastroMotorista?.DadosCadastraisDtc?.CNHCATEGORIA ?? "";
                objMotorista.DataPrimeiraHabilitacao = dadosCadastroMotorista.DadosCadastraisDtc.CNHDATA_PRIMEIRA_HABILITACAO.ToNullableDateTime("yyyy-MM-dd HH:mm:ss");
                objMotorista.DataHabilitacao = dadosCadastroMotorista.DadosCadastraisDtc.CNHDATA_EXPEDICAO.ToNullableDateTime("yyyy-MM-dd HH:mm:ss");
                objMotorista.DataVencimentoHabilitacao = dadosCadastroMotorista.DadosCadastraisDtc.CNHDATA_VENCIMENTO.ToNullableDateTime("yyyy-MM-dd HH:mm:ss");
                objMotorista.TipoMotorista = ObterTipoMotorista(dadosCadastroMotorista?.DadosCadastraisDtc?.VINCULO_MOTORISTA ?? "");
                objMotorista.NumeroMatricula = dadosCadastroMotorista?.DadosCadastraisDtc?.NUMERO_REGISTRO ?? "";
                objMotorista.DataAdmissao = dadosCadastroMotorista.DadosCadastraisDtc.DATAADMISSAO.ToNullableDateTime("yyyy-MM-dd HH:mm:ss");
                objMotorista.Localidade = ObterLocalidade(dadosCadastroMotorista);
                objMotorista.CEP = dadosCadastroMotorista?.DadosEnderecoDtc?.CEP ?? "";
                objMotorista.Bairro = dadosCadastroMotorista?.DadosEnderecoDtc?.BAIRRO ?? "";
                objMotorista.Endereco = dadosCadastroMotorista?.DadosEnderecoDtc?.LOGRADOURO ?? "";
                objMotorista.NumeroEndereco = dadosCadastroMotorista?.DadosEnderecoDtc?.NUMERO ?? "";
                objMotorista.Complemento = dadosCadastroMotorista?.DadosEnderecoDtc?.COMPLEMENTO ?? "";

                objMotorista.Email = ObterListaEmail(dadosCadastroMotorista?.DadosContatoDtc?.ListaEMAIL?.ToList() ?? new List<string>());

                objMotorista.Telefone = ObterTelefone(dadosCadastroMotorista?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.DDD ?? "", dadosCadastroMotorista?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.NUMEROTELEFONE ?? "");
                objMotorista.Cargo = dadosCadastroMotorista.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.DEPART_CONTATO_DESCRICAO ?? "";
                objMotorista.Status = (dadosCadastroMotorista?.Status ?? 0) == 1 ? "A" : "I";


                if (objMotorista.DataEmissaoRG == null || objMotorista.DataEmissaoRG < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataEmissaoRG = DateTime.MinValue.AddYears(1969);

                if (objMotorista.DataNascimento == null || objMotorista.DataNascimento < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataNascimento = DateTime.MinValue.AddYears(1969);

                if (objMotorista.DataPrimeiraHabilitacao == null || objMotorista.DataPrimeiraHabilitacao < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataPrimeiraHabilitacao = DateTime.MinValue.AddYears(1969);

                if (objMotorista.DataHabilitacao == null || objMotorista.DataHabilitacao < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataHabilitacao = DateTime.MinValue.AddYears(1969);

                if (objMotorista.DataVencimentoHabilitacao == null || objMotorista.DataVencimentoHabilitacao < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataVencimentoHabilitacao = DateTime.MinValue.AddYears(1969);

                if (objMotorista.DataAdmissao == null || objMotorista.DataAdmissao < DateTime.MinValue.AddYears(1969))
                    objMotorista.DataAdmissao = DateTime.MinValue.AddYears(1969);

                if (!TemErros(dadosCadastroMotorista, repMotorista.Validator(objMotorista)))
                {
                    if (inserir)
                        repMotorista.Inserir(objMotorista);
                    else
                        repMotorista.Atualizar(objMotorista);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, objMotorista, null, "Motorista recebido pela integração SIC", _unitOfWork);

                    idsRecebidos.Add(dadosCadastroMotorista.ID);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoSIC");
                Log.TratarErro(JsonConvert.SerializeObject(dadosCadastroMotorista), "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objMotorista, null, "Erro ao cadastrar motorista pela integração SIC", _unitOfWork);
            }
        }

        private void SalvarDadosCliente(ServicoSIC.DadosCadastroIntegracaoDTC dadosCadastroCliente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, ref List<int> idsRecebidos)
        {
            Dominio.Entidades.Cliente objCliente = new Dominio.Entidades.Cliente();
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);

                bool inserir = false;
                bool ClientePF = dadosCadastroCliente.TipoPessoaDtc == Servicos.ServicoSIC.EnumTipoPessoaDTC.PessoaFisica;
                bool ClientePJ = dadosCadastroCliente.TipoPessoaDtc == Servicos.ServicoSIC.EnumTipoPessoaDTC.PessoaJuridica;

                string cpfCnpj = dadosCadastroCliente.DadosCadastraisDtc.CPF.ObterSomenteNumeros();
                if (string.IsNullOrWhiteSpace(cpfCnpj))
                    cpfCnpj = dadosCadastroCliente.DadosCadastraisDtc.CNPJ.ObterSomenteNumeros();
                if (string.IsNullOrWhiteSpace(cpfCnpj))
                    cpfCnpj = dadosCadastroCliente.DadosCadastraisDtc.CODIGOSAP.ObterSomenteNumeros();

                objCliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj.ToDouble());

                if (objCliente == null)
                {
                    inserir = true;
                    objCliente = new Dominio.Entidades.Cliente();
                    objCliente.CPF_CNPJ = cpfCnpj.ToDouble();
                    objCliente.Tipo = ClientePF ? "F" : ClientePJ ? "J" : "E";
                    objCliente.Bloqueado = false;
                }

                objCliente.CodigoIntegracao = dadosCadastroCliente?.DadosCadastraisDtc?.CODIGOSAP ?? "";

                objCliente.Nome = dadosCadastroCliente?.DadosCadastraisDtc?.NOME ?? "";
                if (string.IsNullOrWhiteSpace(objCliente.Nome))
                    objCliente.Nome = dadosCadastroCliente?.DadosCadastraisDtc?.RAZAOSOCIAL ?? "";

                objCliente.Atividade = repAtividade.BuscarPorCodigo(3);
                objCliente.NomeFantasia = dadosCadastroCliente?.DadosCadastraisDtc?.NOMEFANTASIA ?? "";
                if ((dadosCadastroCliente?.DadosCadastraisDtc?.Lista_SAPID_DESCRICAO_EMPRESA?.FirstOrDefault()?.SAPID ?? null) != null)
                    objCliente.ClientePai = repCliente.BuscarPorCodigoIntegracao(dadosCadastroCliente.DadosCadastraisDtc.Lista_SAPID_DESCRICAO_EMPRESA.FirstOrDefault().SAPID);

                objCliente.IE_RG = dadosCadastroCliente?.DadosCadastraisDtc?.INSCRICAOESTADUAL ?? "";
                objCliente.InscricaoMunicipal = dadosCadastroCliente?.DadosCadastraisDtc?.INSCRICAOMUNICIPAL ?? "";
                objCliente.InscricaoSuframa = dadosCadastroCliente?.DadosCadastraisDtc?.SUFRAMA ?? "";
                objCliente.Categoria = ObterCategoriaPessoa(dadosCadastroCliente.DadosCadastraisDtc);
                objCliente.RG_Passaporte = dadosCadastroCliente?.DadosCadastraisDtc?.RG ?? "";
                if (!string.IsNullOrEmpty(dadosCadastroCliente?.DadosCadastraisDtc?.ORGAOEMISSOR ?? ""))
                    objCliente.OrgaoEmissorRG = dadosCadastroCliente.DadosCadastraisDtc.ORGAOEMISSOR.ToEnum<Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG>();

                objCliente.EstadoRG = ObterEstado(dadosCadastroCliente.DadosCadastraisDtc.UFEMISSAO);
                objCliente.Sexo = dadosCadastroCliente.DadosCadastraisDtc.SEXO.ToNullableEnum<Dominio.ObjetosDeValor.Enumerador.Sexo>();

                objCliente.EstadoCivil = dadosCadastroCliente.DadosCadastraisDtc.ESTADOCIVIL.ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil>();
                objCliente.NaoDescontaIRRF = (dadosCadastroCliente?.DadosCadastraisDtc?.RETEMIR ?? "") == "1" ? "SIM" : "NÃO";
                objCliente.IndicadorIE = ObterIndicadorIE(dadosCadastroCliente?.DadosCadastraisDtc?.CONTRIBUINTE_ICMS_SAPID ?? "");
                objCliente.RegimeTributario = ObterRegimeTributario(dadosCadastroCliente?.DadosCadastraisDtc?.REGIMETRIBUTARIO_SAPID ?? "");

                objCliente.Localidade = ObterLocalidade(dadosCadastroCliente);

                objCliente.CEP = dadosCadastroCliente?.DadosEnderecoDtc?.CEP ?? "";
                objCliente.Bairro = dadosCadastroCliente?.DadosEnderecoDtc?.BAIRRO ?? "";
                objCliente.Endereco = dadosCadastroCliente?.DadosEnderecoDtc.LOGRADOURO ?? "";
                objCliente.Numero = dadosCadastroCliente?.DadosEnderecoDtc?.NUMERO ?? "";
                objCliente.Complemento = dadosCadastroCliente?.DadosEnderecoDtc?.COMPLEMENTO ?? "";

                objCliente.Email = ObterListaEmail(dadosCadastroCliente?.DadosContatoDtc?.ListaEMAIL?.ToList() ?? new List<string>());

                objCliente.Telefone1 = ObterTelefone(dadosCadastroCliente?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.DDD ?? "", dadosCadastroCliente?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.NUMEROTELEFONE ?? "");

                objCliente.Ativo = dadosCadastroCliente.Status == 1;
                if (!TemErros(dadosCadastroCliente, repCliente.Validator(objCliente)))
                {
                    if (inserir)
                        repCliente.Inserir(objCliente);
                    else
                        repCliente.Atualizar(objCliente);

                    AdicionarModalidadeCliente(objCliente);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, objCliente, null, "Cliente recebido pela integração SIC", _unitOfWork);

                    idsRecebidos.Add(dadosCadastroCliente.ID);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoSIC");
                Log.TratarErro(JsonConvert.SerializeObject(dadosCadastroCliente), "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objCliente, null, "Erro ao cadastrar cliente pela integração SIC", _unitOfWork);
            }
        }

        private void SalvarDadosTransportadorTerceiro(ServicoSIC.DadosCadastroIntegracaoDTC dadosCadastroCliente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, ref List<int> idsRecebidos)
        {
            Dominio.Entidades.Cliente objCliente = new Dominio.Entidades.Cliente();
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

                bool inserir = false;
                bool ClientePF = dadosCadastroCliente.TipoPessoaDtc == Servicos.ServicoSIC.EnumTipoPessoaDTC.PessoaFisica;
                bool ClientePJ = dadosCadastroCliente.TipoPessoaDtc == Servicos.ServicoSIC.EnumTipoPessoaDTC.PessoaJuridica;

                string cpfCnpj = dadosCadastroCliente.DadosCadastraisDtc.CPF.ObterSomenteNumeros();
                if (string.IsNullOrWhiteSpace(cpfCnpj))
                    cpfCnpj = dadosCadastroCliente.DadosCadastraisDtc.CNPJ.ObterSomenteNumeros();

                if (string.IsNullOrWhiteSpace(cpfCnpj))
                    throw new ServicoException("CPF/CNPJ não informado");

                objCliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj.ToDouble());

                if (objCliente == null)
                {
                    inserir = true;
                    objCliente = new Dominio.Entidades.Cliente();
                    objCliente.CPF_CNPJ = cpfCnpj.ToDouble();
                    objCliente.Tipo = ClientePF ? "F" : ClientePJ ? "J" : "E";
                    objCliente.Bloqueado = false;
                    objCliente.DataCadastro = DateTime.Now;
                }

                objCliente.CodigoIntegracao = dadosCadastroCliente?.DadosCadastraisDtc?.CODIGOSAP ?? "";
                if ((dadosCadastroCliente?.DadosCadastraisDtc?.Lista_SAPID_DESCRICAO_EMPRESA?.FirstOrDefault()?.SAPID ?? "") != "")
                    objCliente.GrupoPessoas = repGrupoPessoas.BuscarPorCodigoIntegracao(dadosCadastroCliente.DadosCadastraisDtc.Lista_SAPID_DESCRICAO_EMPRESA.FirstOrDefault().SAPID);


                objCliente.Nome = dadosCadastroCliente?.DadosCadastraisDtc?.NOME ?? "";
                if (string.IsNullOrWhiteSpace(objCliente.Nome))
                    objCliente.Nome = dadosCadastroCliente?.DadosCadastraisDtc?.RAZAOSOCIAL ?? "";

                objCliente.Atividade = repAtividade.BuscarPorCodigo(3);
                objCliente.NomeFantasia = dadosCadastroCliente?.DadosCadastraisDtc?.NOMEFANTASIA ?? "";
                objCliente.IE_RG = dadosCadastroCliente?.DadosCadastraisDtc?.INSCRICAOESTADUAL ?? "";
                objCliente.InscricaoMunicipal = dadosCadastroCliente?.DadosCadastraisDtc?.INSCRICAOMUNICIPAL ?? "";
                objCliente.InscricaoSuframa = dadosCadastroCliente?.DadosCadastraisDtc?.SUFRAMA ?? "";

                objCliente.RG_Passaporte = dadosCadastroCliente?.DadosCadastraisDtc?.RG ?? "";
                if (!string.IsNullOrEmpty(dadosCadastroCliente?.DadosCadastraisDtc?.ORGAOEMISSOR ?? null))
                    objCliente.OrgaoEmissorRG = dadosCadastroCliente.DadosCadastraisDtc.ORGAOEMISSOR.ToEnum<Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG>();

                objCliente.EstadoRG = ObterEstado(dadosCadastroCliente.DadosCadastraisDtc.UFEMISSAO);
                objCliente.Sexo = dadosCadastroCliente.DadosCadastraisDtc.SEXO.ToNullableEnum<Dominio.ObjetosDeValor.Enumerador.Sexo>();
                objCliente.DataNascimento = dadosCadastroCliente.DadosCadastraisDtc.DATANASCIMENTO.ToNullableDateTime();
                objCliente.EstadoCivil = dadosCadastroCliente.DadosCadastraisDtc.ESTADOCIVIL.ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil>();
                objCliente.PISPASEP = dadosCadastroCliente?.DadosCadastraisDtc?.PIS ?? "";

                objCliente.RegimeTributario = ObterRegimeTributario(dadosCadastroCliente?.DadosCadastraisDtc?.REGIMETRIBUTARIO_SAPID ?? "");

                objCliente.Localidade = ObterLocalidade(dadosCadastroCliente);
                objCliente.CEP = dadosCadastroCliente?.DadosEnderecoDtc?.CEP ?? "";
                objCliente.Bairro = dadosCadastroCliente?.DadosEnderecoDtc?.BAIRRO ?? "";
                objCliente.Endereco = dadosCadastroCliente?.DadosEnderecoDtc?.LOGRADOURO ?? "";
                objCliente.Numero = dadosCadastroCliente?.DadosEnderecoDtc?.NUMERO ?? "";
                objCliente.Complemento = dadosCadastroCliente?.DadosEnderecoDtc?.COMPLEMENTO ?? "";

                objCliente.Email = ObterListaEmail(dadosCadastroCliente?.DadosContatoDtc?.ListaEMAIL?.ToList() ?? new List<string>());
                objCliente.Telefone1 = ObterTelefone(dadosCadastroCliente?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.DDD ?? "", dadosCadastroCliente?.DadosContatoDtc?.ListaInformacoesContato?.FirstOrDefault()?.NUMEROTELEFONE ?? "");

                if (dadosCadastroCliente.DadosFinanceirosDtc.listaInformacoesBancarias != null)
                {
                    objCliente.Agencia = dadosCadastroCliente.DadosFinanceirosDtc.listaInformacoesBancarias.FirstOrDefault().AGENCIA ?? "";
                    objCliente.NumeroConta = dadosCadastroCliente.DadosFinanceirosDtc.listaInformacoesBancarias.FirstOrDefault().CONTA ?? "";
                    objCliente.DigitoAgencia = dadosCadastroCliente.DadosFinanceirosDtc.listaInformacoesBancarias.FirstOrDefault().CONTA_DIGITO ?? "";
                    objCliente.Banco = ObterBanco(dadosCadastroCliente.DadosFinanceirosDtc.listaInformacoesBancarias.FirstOrDefault());
                }

                objCliente.Ativo = dadosCadastroCliente.Status == 1;

                if (!TemErros(dadosCadastroCliente, repCliente.Validator(objCliente)))
                {
                    if (inserir)
                        repCliente.Inserir(objCliente);
                    else
                        repCliente.Atualizar(objCliente);

                    ObterContatos(dadosCadastroCliente.DadosContatoDtc, objCliente);
                    AdicionarModalidadeTransportadorTerceiro(objCliente, dadosCadastroCliente.DadosCadastraisDtc);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, objCliente, null, "Transportador Terceiro recebido pela integração SIC", _unitOfWork);

                    idsRecebidos.Add(dadosCadastroCliente.ID);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoSIC");
                Log.TratarErro(JsonConvert.SerializeObject(dadosCadastroCliente), "IntegracaoSIC");
                Servicos.Auditoria.Auditoria.Auditar(auditado, objCliente, null, "Erro ao cadastrar transportador pela integração SIC", _unitOfWork);
            }
        }


        private string ObterTipoRodado(string tipoRodado, string tipoVeiculo)
        {
            if (!string.IsNullOrEmpty(tipoVeiculo) && tipoVeiculo != "1")
            {
                switch (tipoRodado)
                {
                    case "Não Aplicável": return "00";
                    case "Truck": return "01";
                    case "Toco": return "02";
                    case "Cavalo Mecânico": return "03";
                    case "VAN": return "04";
                    case "Utilitario": return "05";
                    case "Outros": return "06";
                    default: return "06";
                }
            }
            return "00";
        }

        private string ObterTipoCarroceria(string tipoCarroceria)
        {
            switch (tipoCarroceria)
            {
                case "Cegonheira":
                    return "00";
                case "Sem Carroceria":
                    return "00";
                case "Canavieiro - Inteira":
                    return "01";
                case "Florestal":
                    return "01";
                case "Grade Baixa":
                    return "01";
                case "Grade Baixa Extensiva":
                    return "01";
                case "Guincho":
                    return "01";
                case "Mesa Rebaixada":
                    return "01";
                case "Pescoço Removível":
                    return "01";
                case "Plana":
                    return "01";
                case "Plataforma":
                    return "01";
                case "Poliguindaste":
                    return "01";
                case "Roll On/Off":
                    return "01";
                case "Cana Picada":
                    return "01";
                case "Canavieiro - Picada":
                    return "03";
                case "Graneleiro":
                    return "03";
                case "Bebida Roll Up":
                    return "05";
                case "Bebida Roll Up - Rebaixada":
                    return "05";
                case "Sider Asa Delta":
                    return "05";
                case "Sider Plano":
                    return "05";
                case "Sider Rebaixado":
                    return "05";
                case "Top Sider":
                    return "05";
                case "Double Deck Sider":
                    return "05";
                case "Basculante":
                    return "02";
                case "Baú Carga seca":
                    return "02";
                case "Baú Carga seca Rebaixada":
                    return "02";
                case "Baú Refrigerado":
                    return "02";
                case "Baú Refrigerado Rebaixado":
                    return "02";
                case "Betoneira":
                    return "02";
                case "Compactador":
                    return "02";
                case "Inloader":
                    return "02";
                case "Silo":
                    return "02";
                case "Silo Basculante":
                    return "02";
                case "Tanque":
                    return "02";
                case "Tanque Basculante":
                    return "02";
                case "Tanque Isotérmico":
                    return "02";
                case "Tanque Isotérmico Basculante":
                    return "02";
                case "FLORESTAL":
                    return "02";
                case "Double Deck Baú":
                    return "02";
                case "Porta Contêiner 20":
                    return "04";
                case "Porta Contêiner 40":
                    return "04";
                default:
                    return "00";
            }
        }

        private string ObterTipoCombustivel(string tipoCombustivel)
        {
            switch (tipoCombustivel)
            {
                case "Não Aplicável": return "";
                case "DIESEL S50": return "I";
                case "DIESEL COMUM": return "D";
                case "GASOLINA": return "G";
                case "ETANOL": return "E";
                case "ALCOOL/GASOLINA": return "G";
                case "QUEROSENE": return "O";
                case "GNV": return "O";
                default: return "O";
            }
        }

        private Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo ObterSegmentoVeiculo(string categoria)
        {
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = repSegmentoVeiculo.BuscarPorDescricao(categoria);

            if (segmentoVeiculo == null)
            {
                segmentoVeiculo = new Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo();
                segmentoVeiculo.Descricao = categoria;
                segmentoVeiculo.Ativo = true;

                repSegmentoVeiculo.Inserir(segmentoVeiculo);
            }

            return segmentoVeiculo;
        }

        private Dominio.Entidades.MarcaVeiculo ObterMarcaVeiculo(string fabricante)
        {
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Dominio.Entidades.MarcaVeiculo marca = repMarcaVeiculo.BuscarPorCodigoIntegracao(fabricante);

            if (marca == null)
            {
                marca = new Dominio.Entidades.MarcaVeiculo();
                marca.Descricao = fabricante;
                marca.CodigoIntegracao = fabricante;
                marca.Status = "A";

                repMarcaVeiculo.Inserir(marca);
            }

            return marca;
        }

        private Dominio.Entidades.ModeloVeiculo ObterModeloVeiculo(string modelo)
        {
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Dominio.Entidades.ModeloVeiculo modeloVeiculo = repModeloVeiculo.BuscarPorCodigoIntegracao(modelo);

            if (modeloVeiculo == null)
            {
                modeloVeiculo = new Dominio.Entidades.ModeloVeiculo();
                modeloVeiculo.Descricao = modelo;
                modeloVeiculo.CodigoIntegracao = modelo;
                modeloVeiculo.Status = "A";

                repModeloVeiculo.Inserir(modeloVeiculo);
            }

            return modeloVeiculo;
        }

        private Dominio.Entidades.Embarcador.Veiculos.CorVeiculo ObterCorVeiculo(string cor)
        {
            Repositorio.Embarcador.Veiculos.CorVeiculo repMarcaVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo = repMarcaVeiculo.ConsultarPorDescricao(cor);

            if (corVeiculo == null)
            {
                corVeiculo = new Dominio.Entidades.Embarcador.Veiculos.CorVeiculo();
                corVeiculo.Descricao = cor;

                repMarcaVeiculo.Inserir(corVeiculo);
            }

            return corVeiculo;
        }

        private string ObterTipoVeiculo(string tipo)
        {
            switch (tipo)
            {
                case "Terceiro": return "T";
                case "Próprio": return "P";
                default: return "";
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga(Servicos.ServicoSIC.CadastroIntegracaoDadosCadastraisVeiculoDTC dadosCadastraisVeiculo)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(dadosCadastraisVeiculo.CARROCERIA);

            if (modeloVeicular == null)
            {

                if (dadosCadastraisVeiculo.IMPLEMENTO == "0")
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularPorTipoEixo = repModeloVeicularCarga.buscarPorCodigoIntegracaoEEixo(dadosCadastraisVeiculo.IMPLEMENTO, dadosCadastraisVeiculo.EIXO);
                    if (modeloVeicularPorTipoEixo == null)
                    {
                        modeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();
                        modeloVeicular.CodigoIntegracao = "Tração";
                        modeloVeicular.Descricao = "Tração " + dadosCadastraisVeiculo.EIXO;
                        modeloVeicular.Ativo = true;
                        modeloVeicular.CapacidadePesoTransporte = 0;
                        modeloVeicular.ToleranciaPesoExtra = 0;
                        modeloVeicular.ToleranciaPesoMenor = 0;
                        modeloVeicular.VeiculoPaletizado = false;
                        modeloVeicular.ModeloControlaCubagem = false;
                        modeloVeicular.ModeloTracaoReboquePadrao = false;

                        repModeloVeicularCarga.Inserir(modeloVeicular);
                    }

                }
                else
                {
                    modeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();
                    modeloVeicular.CodigoIntegracao = dadosCadastraisVeiculo.CARROCERIA;
                    modeloVeicular.Descricao = dadosCadastraisVeiculo.CARROCERIA + dadosCadastraisVeiculo.EIXO;
                    modeloVeicular.Ativo = true;
                    modeloVeicular.CapacidadePesoTransporte = 0;
                    modeloVeicular.ToleranciaPesoExtra = 0;
                    modeloVeicular.ToleranciaPesoMenor = 0;
                    modeloVeicular.VeiculoPaletizado = false;
                    modeloVeicular.ModeloControlaCubagem = false;
                    modeloVeicular.ModeloTracaoReboquePadrao = false;

                    repModeloVeicularCarga.Inserir(modeloVeicular);
                }

            }

            return modeloVeicular;
        }

        private Dominio.Entidades.Cliente ObterProprietarioVeiculo(Servicos.ServicoSIC.CadastroIntegracaoDadosCadastraisVeiculoDTC dadosCadastraisVeiculo)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            string cpfCnpj = Utilidades.String.OnlyNumbers(dadosCadastraisVeiculo.IDENTIFICACAOTRANSPORTADORA);

            Dominio.Entidades.Cliente proprietaro = repCliente.BuscarPorCPFCNPJ(cpfCnpj.ToDouble());

            if (proprietaro == null)
                proprietaro = repCliente.BuscarPorNome(dadosCadastraisVeiculo.NOMETRANSPORTADORA);

            return proprietaro;
        }

        private string PreencherObservacaoVeiculo(string observacaoVeiculo, Servicos.ServicoSIC.CadastroIntegracaoDadosComplementoVeiculoDTC dadosComplementares)
        {
            StringBuilder observacao = new StringBuilder();

            observacao.Append($" Parede: {dadosComplementares.PAREDE}, Estrado:{dadosComplementares.ESTRADO}, Isolamento: {dadosComplementares.ISOLAMENTO}, Suspensão: {dadosComplementares.SUSPENSAO}, ")
            .Append($"Tomada de Força: {dadosComplementares.TOMADA_DE_FORCA}, Engate Pneumático: {dadosComplementares.ENGATE_PNEUMATICO}, Piso: {dadosComplementares.PISO}, Revestimento: {dadosComplementares.REVESTIMENTO}, ")
            .Append($"Comprimento: {dadosComplementares.COMPRIMENTO}, Largura: {dadosComplementares.LARGURA}, Volume: {dadosComplementares.VOLUME}, Compr. Pesc: {dadosComplementares.COMPRIMENTOPESCOCO}, ")
            .Append($"Alt Pesc: {dadosComplementares.ALTURAPESCOCO}, LocK Container: {dadosComplementares.LOCKCONTAINER}, Moto Compressor: {dadosComplementares.MOTOCOMPRESSOR}, TAG Sem Parar: {dadosComplementares.TAGSEMPARAR}, ")
            .Append($"Num. Sem Parar: {dadosComplementares.NUMEROSEMPARAR}, Divisor intermediário: {dadosComplementares.DIVISORINTERMEDIARIO}, Doble Deck: {dadosComplementares.DOBLEDECK}, ")
            .Append($"Qtd de Pallets: {dadosComplementares.QUANTIDADEPALLETS}, Munk: {dadosComplementares.MUNK}, Capacidade Munck: {dadosComplementares.CAPACIDADEMUNCK}, Alcance Vertical: {dadosComplementares.ALCANCEVERTICAL}, ")
            .Append($"Berço para Bobinas: {dadosComplementares.BERCOBOBINAS}, Rampa Hidráulica: {dadosComplementares.RAMPAHIDRAULICA}, Refrigerador: {dadosComplementares.REFRIGERADOR}, Fabricante Refrigerador: {dadosComplementares.FABICANTEREFRIGERADOR}, ")
            .Append($"Modelo Refrigerador: {dadosComplementares.MODLOREFRIGERADOR}, Plataforma Elevatória: {dadosComplementares.PLATAFORMAETAVATORIA}, Capacidade Plataforma Elevatória: {dadosComplementares.CAPACIDADEPLATAFORMAELEVATORIA}.");

            observacaoVeiculo += observacao.ToString();

            return observacaoVeiculo.Left(500);
        }

        private Dominio.Entidades.Localidade ObterLocalidade(ServicoSIC.DadosCadastroIntegracaoDTC dadosCadastro)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(dadosCadastro.DadosEnderecoDtc.CODIGOMUNICIPIO.ToInt());

            string uf = !string.IsNullOrWhiteSpace(dadosCadastro.DadosEnderecoDtc.UF) ? dadosCadastro.DadosEnderecoDtc.UF : "EX";

            if (localidade == null)
                if (!string.IsNullOrWhiteSpace(dadosCadastro.DadosEnderecoDtc.MUNICIPIO))
                    localidade = repLocalidade.BuscarPorCidadeUF(Utilidades.String.RemoveAccents(dadosCadastro.DadosEnderecoDtc.MUNICIPIO), uf);

            uf = !string.IsNullOrWhiteSpace(dadosCadastro.DadosCadastraisDtc.UFNATURAL) ? dadosCadastro.DadosCadastraisDtc.UFNATURAL : "EX";

            if (localidade == null)
                if (!string.IsNullOrWhiteSpace(dadosCadastro.DadosCadastraisDtc.MUNICIPIONATURAL))
                    localidade = repLocalidade.BuscarPorCidadeUF(Utilidades.String.RemoveAccents(dadosCadastro.DadosCadastraisDtc.MUNICIPIONATURAL), uf);

            return localidade;
        }

        private Dominio.Entidades.Estado ObterEstado(string uf)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            return repEstado.BuscarPorSigla(uf);
        }

        private string ObterTelefone(string ddd, string numeroTelefone)
        {
            string foneFormatado = string.Empty;

            if (!string.IsNullOrEmpty(ddd))
                foneFormatado = ddd;

            foneFormatado += numeroTelefone;

            return foneFormatado.ObterTelefoneFormatado();
        }

        private string ObterListaEmail(List<string> emails)
        {
            string email = string.Join(";", emails);

            return email.Left(200);
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista ObterTipoMotorista(string vinculoMotorista)
        {
            switch (vinculoMotorista)
            {
                case "Funcionário": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio;
                case "Terceiro": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro;
                case "Agregado": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro;
                default: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio;
            }
        }

        private Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa ObterCategoriaPessoa(Servicos.ServicoSIC.CadastroIntegracaoDadosCadastraisDTC dadosCadastraisDTC)
        {
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = repCategoriaPessoa.BuscarPorCodigoIntegracao(dadosCadastraisDTC.CLASSECLIENTE_SAPID);

            if (categoriaPessoa == null && !string.IsNullOrWhiteSpace(dadosCadastraisDTC.CLASSECLIENTE_DESCRICAO) && !string.IsNullOrWhiteSpace(dadosCadastraisDTC.CLASSECLIENTE_SAPID))
            {
                categoriaPessoa = new Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa();
                categoriaPessoa.Descricao = dadosCadastraisDTC.CLASSECLIENTE_DESCRICAO;
                categoriaPessoa.CodigoIntegracao = dadosCadastraisDTC.CLASSECLIENTE_SAPID;

                repCategoriaPessoa.Inserir(categoriaPessoa);
            }

            return categoriaPessoa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? ObterIndicadorIE(string indicadorIE)
        {
            switch (indicadorIE)
            {
                case "01": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                case "02": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento;
                case "99": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                default: return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario? ObterRegimeTributario(string regimeTributario)
        {
            switch (regimeTributario)
            {
                case "0001": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.SimplesNacional;
                case "0003": return Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.LucroReal;
                default: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado;
            }
        }

        private void ObterContatos(Servicos.ServicoSIC.CadastroIntegracaoDadosContatoDTC dadosContato, Dominio.Entidades.Cliente pessoa)
        {
            Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(_unitOfWork);

            if (pessoa.Contatos == null && dadosContato.ListaInformacoesContato.Length > 0)
            {
                foreach (var contato in dadosContato.ListaInformacoesContato)
                {
                    Dominio.Entidades.Embarcador.Contatos.PessoaContato contatoPessoa = new Dominio.Entidades.Embarcador.Contatos.PessoaContato();

                    contatoPessoa.Pessoa = pessoa;
                    contatoPessoa.Ativo = true;
                    contatoPessoa.Contato = contato.CONTATOTELEFONE;
                    contatoPessoa.Cargo = contato.DEPART_CONTATO_DESCRICAO;
                    contatoPessoa.Telefone = ObterTelefone(contato.DDD, contato.NUMEROTELEFONE);
                    contatoPessoa.TiposContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
                    contatoPessoa.TiposContato.Add(ObterTipoContato(contato.TIPOTELEFONE));

                    repPessoaContato.Inserir(contatoPessoa);
                }
            }
        }

        private Dominio.Entidades.Embarcador.Contatos.TipoContato ObterTipoContato(string tipoTelefone)
        {
            if (string.IsNullOrEmpty(tipoTelefone))
                return new Dominio.Entidades.Embarcador.Contatos.TipoContato();

            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(_unitOfWork);
            Dominio.Entidades.Embarcador.Contatos.TipoContato tipoContato = repTipoContato.BuscarPorDescricao(tipoTelefone);

            if (tipoContato == null)
            {
                tipoContato = new Dominio.Entidades.Embarcador.Contatos.TipoContato();
                tipoContato.Descricao = tipoTelefone;
                tipoContato.Ativo = true;

                repTipoContato.Inserir(tipoContato);
            }

            return tipoContato;
        }

        private Dominio.Entidades.Banco ObterBanco(Servicos.ServicoSIC.DadosFinanceirosInformacoesBancariasDTC dadosFinanceiros)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(_unitOfWork);
            Dominio.Entidades.Banco banco = repBanco.BuscarPorNumero(dadosFinanceiros.BANCO_SAPID.ToInt());

            if (banco == null)
            {
                banco = new Dominio.Entidades.Banco();
                banco.Descricao = dadosFinanceiros.BANCO_DESCRICAO;
                banco.Numero = dadosFinanceiros.BANCO_SAPID.ToInt();
                banco.CodigoIntegracao = dadosFinanceiros.BANCO_SAPID;
                repBanco.Inserir(banco);
            }

            return banco;
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas RetornarModalidadePessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, pessoa.CPF_CNPJ);

            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.Cliente = pessoa;
                modalidadePessoas.TipoModalidade = tipoModalidade;
                repModalidadePessoas.Inserir(modalidadePessoas);
            }
            return modalidadePessoas;
        }

        private void AdicionarModalidadeTransportadorTerceiro(Dominio.Entidades.Cliente transportador, Servicos.ServicoSIC.CadastroIntegracaoDadosCadastraisDTC dadosCadastro)
        {
            bool inserir = false;
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(transportador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            if (modalidadeTransportadoraPessoas == null)
            {
                inserir = true;
                modalidadeTransportadoraPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
            }

            modalidadeTransportadoraPessoas.DataVencimentoRNTRC = dadosCadastro.VENCIMENTO_RNTRC.ToNullableDateTime("yyyy-MM-dd");
            modalidadeTransportadoraPessoas.ModalidadePessoas = modalidadePessoas;
            modalidadeTransportadoraPessoas.RNTRC = dadosCadastro.RNTRC;
            modalidadeTransportadoraPessoas.PercentualAbastecimentoFretesTerceiro = 0;
            modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro = 0;
            modalidadeTransportadoraPessoas.PercentualCobranca = 0;

            if (inserir)
                repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas);
            else
                repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);
        }

        private void AdicionarModalidadeCliente(Dominio.Entidades.Cliente cliente)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            if (modalidadeClientePessoas == null)
                modalidadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

            modalidadeClientePessoas.ModalidadePessoas = modalidadePessoas;

            if (modalidadeClientePessoas.Codigo == 0)
                repModalidadeClientePessoas.Inserir(modalidadeClientePessoas);
            else
                repModalidadeClientePessoas.Atualizar(modalidadeClientePessoas);
        }

        private bool TemErros(object obj, List<Repositorio.Global.Dicionario.MapProperty> lstErros)
        {
            if (lstErros != null && lstErros.Count() > 0)
            {
                Log.TratarErro("################################################################", "IntegracaoSICValidacoes");
                Log.TratarErro(JsonConvert.SerializeObject(obj), "IntegracaoSICValidacoes");
                foreach (var erro in lstErros)
                    Log.TratarErro($"CAMPO:{erro.PropoertyName}. ERRO: {erro.MsgError}. VALOR:{(erro.Value == null ? "" : erro.Value)} ", "IntegracaoSICValidacoes");

                Log.TratarErro("################################################################", "IntegracaoSICValidacoes");
                return true;
            }
            else
                return false;

        }


        #endregion
    }

}


