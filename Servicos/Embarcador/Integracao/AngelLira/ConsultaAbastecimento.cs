using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;


namespace Servicos.Embarcador.Integracao.AngelLira
{
    public class ConsultaAbastecimento
    {
        public static void ConsultarAbastecimentosPendentes(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();

            if (integracaoAngelLira == null || !integracaoAngelLira.ConsultarPosicaoAbastecimento)
                return;

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculosConsultar = repVeiculo.BuscarCodigosVeiculosConsultarAbastecimento();
            if (veiculosConsultar == null || veiculosConsultar.Count == 0)
                return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngellira repConsultaAbastecimento = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngellira(unitOfWork);
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira repRetornoConsulta = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

            string url = "";
            Servicos.WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = ObterCabecalhoStatus(unitOfWork, out url, integracaoAngelLira);
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> postosConveniados = repModalidadePessoas.BuscarPostosConveniados();

            var locais_postos = postosConveniados?.Where(c => c.Cliente.Latitude != null && c.Cliente.Longitude != null)
                                                  .GroupBy(x => new { x.Cliente.CPF_CNPJ, x.Cliente.Latitude, x.Cliente.Longitude })
                                                  .Select(g => new
                                                  {
                                                      id = g.Key.CPF_CNPJ,
                                                      latitude = ObterLatitudeOuLongitude(g.Key.Latitude),
                                                      longitude = ObterLatitudeOuLongitude(g.Key.Longitude)
                                                  }).ToList() ?? null;

            if (validationSoapHeader == null)
            {
                Servicos.Log.TratarErro("Não foram configurados os dados de integração com a AngelLira", "AngelLira");
                return;
            }

            DateTime dataInicial = DateTime.Now.AddHours(-4);
            DateTime dataFinal = DateTime.Now;
            Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira consultaAbastecimento = CriarRegistrosConsultaAbastecimento(dataInicial, dataFinal, unitOfWork);
            (Servicos.WSAngelLiraStatus.Veiculos[] abastecimentosVeiculosCadastrados, InspectorBehavior inspector) = ObterAbastecimentosDeVeiculosCadastrados(url, validationSoapHeader, veiculosConsultar, dataInicial, dataFinal);
            CriarRegistrosConsultaVeiculo(consultaAbastecimento, veiculosConsultar, unitOfWork);

            if (abastecimentosVeiculosCadastrados != null && abastecimentosVeiculosCadastrados.Length > 0)
            {
                foreach (var abastecimentoVeiculo in abastecimentosVeiculosCadastrados)
                {
                    if (abastecimentoVeiculo.Hodometro <= 0)
                        continue;

                    Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira retornoAbastecimento = repRetornoConsulta.BuscarRetorno(abastecimentoVeiculo.Placa, abastecimentoVeiculo.Coordenada, abastecimentoVeiculo.DATAHORA, abastecimentoVeiculo.Hodometro);

                    if (retornoAbastecimento == null)
                        retornoAbastecimento = new Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira();

                    retornoAbastecimento.Condutor = Utilidades.String.OnlyNumbers(abastecimentoVeiculo.Condutor.Trim());
                    retornoAbastecimento.ConsultaAbastecimentoAngelLira = consultaAbastecimento;
                    retornoAbastecimento.Cordenada = abastecimentoVeiculo.Coordenada;
                    retornoAbastecimento.DataHora = abastecimentoVeiculo.DATAHORA;
                    retornoAbastecimento.Odometro = abastecimentoVeiculo.Hodometro;
                    retornoAbastecimento.Placa = abastecimentoVeiculo.Placa;
                    retornoAbastecimento.Veiculo = veiculosConsultar.FirstOrDefault(v => v.Placa == abastecimentoVeiculo.Placa);
                    retornoAbastecimento.Motorista = repMotorista.BuscarPorCPF(retornoAbastecimento.Condutor);
                    retornoAbastecimento.Latitude = !string.IsNullOrEmpty(retornoAbastecimento.Cordenada) && retornoAbastecimento.Cordenada.Split(',').Count() > 1 ? retornoAbastecimento.Cordenada.Split(',')[0] : "";
                    retornoAbastecimento.Longitude = !string.IsNullOrEmpty(retornoAbastecimento.Cordenada) && retornoAbastecimento.Cordenada.Split(',').Count() > 1 ? retornoAbastecimento.Cordenada.Split(',')[1] : "";
                    retornoAbastecimento.Posto = null;

                    double latitude = ObterLatitudeOuLongitude(retornoAbastecimento.Latitude);
                    double longitude = ObterLatitudeOuLongitude(retornoAbastecimento.Longitude);
                    if (locais_postos != null && locais_postos.Count > 0)
                    {
                        locais_postos = locais_postos.Where(x => !double.IsNaN(x.latitude) && !double.IsNaN(x.longitude)).OrderBy(x => CalcularDistancia(latitude, longitude, x.latitude, x.longitude)).ToList();
                        if (locais_postos != null && locais_postos.Count > 0)
                            retornoAbastecimento.Posto = repCliente.BuscarPorCPFCNPJ(locais_postos.FirstOrDefault().id);
                    }
                    if (retornoAbastecimento.Abastecimento == null)
                        retornoAbastecimento.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    retornoAbastecimento.Abastecimento = repAbastecimento.BuscarAbastecimento(retornoAbastecimento.Veiculo.Codigo, retornoAbastecimento.Posto.CPF_CNPJ, retornoAbastecimento.DataHora.AddDays(-1), retornoAbastecimento.DataHora.AddDays(1));

                    if (retornoAbastecimento.Codigo == 0)
                        repRetornoConsulta.Inserir(retornoAbastecimento);
                    else
                        repRetornoConsulta.Atualizar(retornoAbastecimento);

                }
            }
            else
            {
                Servicos.Log.TratarErro("Nenhum abastecimento localizado", "AngelLira");
                Servicos.Log.TratarErro(inspector.LastRequestXML, "AngelLira");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "AngelLira");
            }

            consultaAbastecimento.MensagemRetorno = "Consulta realizada para " + veiculosConsultar.Count().ToString() + " veículos";
            repConsultaAbastecimento.Atualizar(consultaAbastecimento);

        }

        public static void ProcessarAbastecimentosPendentes(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira repRetorno = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira> retornosPendentes = repRetorno.BuscarRetornosPendentesProcessamento();
            if (retornosPendentes != null && retornosPendentes.Count > 0)
            {
                for (int i = 0; i < retornosPendentes.Count; i++)
                {
                    try
                    {
                        if (retornosPendentes[i].Abastecimento == null)
                        {
                            retornosPendentes[i].SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            repRetorno.Atualizar(retornosPendentes[i]);
                        }
                        else
                        {
                            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(retornosPendentes[i].Abastecimento.Codigo, true);
                            if (abastecimento.Situacao != "F" && abastecimento.Situacao != "G")
                            {
                                if (retornosPendentes[i].Motorista != null && (abastecimento.Motorista == null || abastecimento.Motorista.Codigo != retornosPendentes[i].Motorista.Codigo))
                                {
                                    abastecimento.MotoristaAnterior = abastecimento.Motorista;
                                    abastecimento.Motorista = retornosPendentes[i].Motorista;

                                    Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Alterou o motorista via integração Angellira. De: " + (abastecimento.MotoristaAnterior?.Descricao ?? "Nenhum") + " Para: " + abastecimento.Motorista.Descricao, unitOfWork);
                                }
                                if (retornosPendentes[i].Odometro > 0 && retornosPendentes[i].Odometro != abastecimento.Kilometragem)
                                {
                                    abastecimento.KilometragemAnteriorAlteraecao = abastecimento.Kilometragem;
                                    abastecimento.Kilometragem = retornosPendentes[i].Odometro;

                                    Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Alterou o KM via integração Angellira. De: " + (abastecimento.KilometragemAnteriorAlteraecao.ToString("n0")) + " Para: " + abastecimento.Kilometragem.ToString("n0"), unitOfWork);
                                }
                                if (!abastecimento.Data.HasValue || retornosPendentes[i].DataHora.Date != abastecimento.Data.Value.Date)
                                {
                                    abastecimento.DataAnterior = abastecimento.Data;
                                    abastecimento.Data = retornosPendentes[i].DataHora;

                                    Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Alterou a data via integração Angellira. De: " + (abastecimento.DataAnterior.HasValue ? abastecimento.DataAnterior.Value.ToString("dd/MM/yyyy HH:mm:ss") : "Nunhuma") + " Para: " + abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);
                                }

                                abastecimento.Situacao = "A";
                                abastecimento.DataAlteracao = DateTime.Now;

                                Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, configuracaoTMS);

                                abastecimento.Integrado = false;

                                repAbastecimento.Atualizar(abastecimento, auditado);
                            }

                            retornosPendentes[i].SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            repRetorno.Atualizar(retornosPendentes[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "AngelLira");
                    }
                }
            }

        }

        #region Métodos Privados

        public static Servicos.WSAngelLira.ValidationSoapHeader ObterCabecalho(Repositorio.UnitOfWork unidadeTrabalho, out string url, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira)
        {
            url = "";
            if (integracaoAngelLira != null)
            {
                Servicos.WSAngelLira.ValidationSoapHeader validationSoapHeader = new Servicos.WSAngelLira.ValidationSoapHeader();
                validationSoapHeader.homologacao = integracaoAngelLira.Homologacao;
                validationSoapHeader.userCod = integracaoAngelLira.Usuario;
                validationSoapHeader.userPwd = integracaoAngelLira.Senha;
                url = integracaoAngelLira.URLAcesso;
                if (string.IsNullOrEmpty(url))
                    url = "https://api.angellira.com.br/ws-soap/";

                return validationSoapHeader;
            }
            else
                return null;
        }
        public static Servicos.WSAngelLiraStatus.ValidationSoapHeader ObterCabecalhoStatus(Repositorio.UnitOfWork unidadeTrabalho, out string url, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira)
        {
            url = "";
            if (integracaoAngelLira != null)
            {
                Servicos.WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = new Servicos.WSAngelLiraStatus.ValidationSoapHeader();
                validationSoapHeader.homologacao = integracaoAngelLira.Homologacao;
                validationSoapHeader.userCod = integracaoAngelLira.Usuario;
                validationSoapHeader.userPwd = integracaoAngelLira.Senha;
                url = integracaoAngelLira.URLAcesso;
                if (string.IsNullOrEmpty(url))
                    url = "https://api.angellira.com.br/ws-soap/";

                return validationSoapHeader;
            }
            else
                return null;
        }
        public static Servicos.WSAngelLiraStatus.WSStatusSoapClient ObterWSStatusClient(string url)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "WSStatus.asmx";

            Servicos.WSAngelLiraStatus.WSStatusSoapClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                client = new Servicos.WSAngelLiraStatus.WSStatusSoapClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                client = new Servicos.WSAngelLiraStatus.WSStatusSoapClient(binding, endpointAddress);
            }

            return client;
        }

        private static double ObterLatitudeOuLongitude(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            return double.Parse(value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 == lat2 && lon1 == lon2)
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515;
                dist = dist * 1.609344 * 1000;//Metros
                return dist;
            }
        }

        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira CriarRegistrosConsultaAbastecimento(DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngellira repConsultaAbastecimento = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngellira(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira consultaAbastecimento = new Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira()
            {
                DataConsulta = DateTime.Now,
                DataFinal = dataFinal,
                DataInicial = dataInicial,
                MensagemRetorno = ""
            };
            repConsultaAbastecimento.Inserir(consultaAbastecimento);

            return consultaAbastecimento;
        }

        private static void CriarRegistrosConsultaVeiculo(Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira consultaAbastecimento, List<Dominio.Entidades.Veiculo> veiculosConsultar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelliraVeiculo repConsultaAbastecimentoVeiculo = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelliraVeiculo(unitOfWork);

            for (int i = 0; i < veiculosConsultar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo consultaVeiculo = new Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo()
                {
                    ConsultaAbastecimentoAngelLira = consultaAbastecimento,
                    Veiculo = veiculosConsultar[i]
                };
                repConsultaAbastecimentoVeiculo.Inserir(consultaVeiculo);
            }
        }

        private static (Servicos.WSAngelLiraStatus.Veiculos[], InspectorBehavior) ObterAbastecimentosDeVeiculosCadastrados(string url, Servicos.WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader, List<Dominio.Entidades.Veiculo> veiculosConsultar, DateTime dataInicial, DateTime dataFinal)
        {
            Servicos.WSAngelLiraStatus.WSStatusSoapClient wsStatusSoapClient = ObterWSStatusClient(url);
            InspectorBehavior inspector = new InspectorBehavior();
            wsStatusSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                Servicos.WSAngelLiraStatus.Veiculos[] veiculosAbastecidos = wsStatusSoapClient.GetLocalAbastecimento(validationSoapHeader, null, dataInicial, dataFinal);

                var placasConsultar = new HashSet<string>(veiculosConsultar.Select(vc => vc.Placa));
                Servicos.WSAngelLiraStatus.Veiculos[] veiculosFiltrados = veiculosAbastecidos.Where(va => placasConsultar.Contains(va.Placa)).ToArray();

                return (veiculosFiltrados, inspector);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML, "AngelLira");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "AngelLira");
                Servicos.Log.TratarErro(ex);

                throw new ServicoException(ex.Message);

            }

        }

        #endregion
    }
}
