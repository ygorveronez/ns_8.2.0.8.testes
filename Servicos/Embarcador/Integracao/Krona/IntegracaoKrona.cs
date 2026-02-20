using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.Krona
{
    public sealed class IntegracaoKrona
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoKrona(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private WebResponse EnviarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, string jsonRequisicao)
        {
            WebRequest requisicao = WebRequest.Create(configuracaoIntegracao.URLIntegracaoKrona);
            byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(jsonRequisicao);

            requisicao.Method = "POST";
            requisicao.ContentLength = byteArrayDadosRequisicao.Length;
            requisicao.ContentType = "application/json";

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();

            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao.GetResponse();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Autenticacao ObterAutenticacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Empresa == null)
                throw new ServicoException("Não existe um transportador informado para a carga.");

            Repositorio.EmpresaIntegracaoKrona repositorioEmpresaIntegracaoKrona = new Repositorio.EmpresaIntegracaoKrona(_unitOfWork);
            Dominio.Entidades.EmpresaIntegracaoKrona empresaIntegracaoKrona = repositorioEmpresaIntegracaoKrona.BuscarPorEmpresa(carga.Empresa.Codigo);

            if ((empresaIntegracaoKrona == null) || string.IsNullOrWhiteSpace(empresaIntegracaoKrona.Usuario) || string.IsNullOrWhiteSpace(empresaIntegracaoKrona.Usuario))
                throw new ServicoException("Não existe configuração de integração com a Krona para o transportador da carga.");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Autenticacao()
            {
                Login = empresaIntegracaoKrona.Usuario.Left(100),
                Senha = empresaIntegracaoKrona.Senha.Left(40)
            };
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (
                !(configuracaoIntegracao?.PossuiIntegracaoKrona ?? false) ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoKrona)
            )
                throw new ServicoException("Não existe configuração de integração disponível para a Krona.");

            return configuracaoIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Destino ObterDestinos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> destinatariosPorRotaFrete)
        {
            List<Dominio.Entidades.Cliente> destinatarios = null;

            if (destinatariosPorRotaFrete != null)
            {
                destinatarios = new List<Dominio.Entidades.Cliente>();
                foreach (var drf in destinatariosPorRotaFrete)
                    destinatarios.Add(drf.Cliente);
            }

            if (destinatarios == null)
            {
                Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
                destinatarios = servicoCargaDadosSumarizados.ObterDestinatarios(carga.Codigo, _unitOfWork);
            }

            if ((destinatarios == null) || (destinatarios.Count == 0))
                throw new ServicoException("Não existem destinatários informados para a carga.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Destino destinos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Destino();

            for (int i = 0; i < Math.Min(destinatarios.Count, 50); i++)
            {
                Dominio.Entidades.Cliente destinatario = destinatarios[i];
                Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade()
                {
                    Codigo = "",
                    Cnpj = FormatarCNPJCPF(destinatario),
                    EnderecoBairro = destinatario.Bairro.Left(30) ?? "",
                    EnderecoCep = string.IsNullOrWhiteSpace(destinatario.CEP) ? "" : string.Format(@"{0:00000\-000}", int.Parse(destinatario.CEP.ObterSomenteNumeros())),
                    EnderecoCidade = destinatario.Localidade?.Descricao.Left(50) ?? "",
                    EnderecoComplemento = destinatario.Complemento.Left(20) ?? "",
                    EnderecoNumero = destinatario.Numero.Left(7) ?? "",
                    EnderecoRua = destinatario.Endereco.Left(30) ?? "",
                    EnderecoUf = destinatario.Localidade?.Estado?.Sigla ?? "",
                    Latitude = "",
                    Longitude = "",
                    NomeFantasia = destinatario.NomeFantasia.Left(50) ?? "",
                    RazaoSocial = destinatario.Nome.Left(50) ?? "",
                    Responsavel = "",
                    ResponsavelCargo = "",
                    ResponsavelCelular = "",
                    ResponsavelEmail = "",
                    ResponsavelTelefone = "",
                    TelefonePrincipal = destinatario.Celular?.Replace(" ", "") ?? "",
                    TelefoneSecundario = "",
                    Tipo = "OUTROS",
                    Unidade = "Unidade"
                };

                destinos.GetType().GetProperty($"Destino{(i + 1)}")?.SetValue(destinos, destino);
            }

            return destinos;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Motorista ObterMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();

            if (motorista == null)
                throw new ServicoException("Não existe um motorista informado para a carga.");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Motorista()
            {
                Aso = DateTime.MinValue,
                Capacitacao = DateTime.MinValue,
                Cdd = DateTime.MinValue,
                CnhCategoria = motorista.Categoria.Left(2) ?? "",
                CnhNumero = motorista.NumeroHabilitacao.Left(14) ?? "",
                CnhVencimento = motorista.DataHabilitacao ?? DateTime.Now,
                Cpf = motorista.CPF_Formatado,
                DataNascimento = motorista.DataNascimento ?? DateTime.Now,
                EnderecoBairro = motorista.Bairro.Left(30) ?? "",
                EnderecoCep = string.IsNullOrWhiteSpace(motorista.CEP) ? "" : string.Format(@"{0:00000\-000}", int.Parse(motorista.CEP)),
                EnderecoCidade = motorista.Localidade?.Descricao.Left(50) ?? "",
                EnderecoComplemento = motorista.Complemento.Left(20) ?? "",
                EnderecoNumero = motorista.NumeroEndereco.Left(7) ?? "",
                EnderecoRua = motorista.Endereco.Left(30) ?? "",
                EnderecoUf = motorista.Localidade?.Estado.Sigla ?? "",
                Escolaridade = "",
                EstadoCivil = motorista.EstadoCivil.HasValue ? motorista.EstadoCivil.Value.ObterDescricao() : "",
                Mopp = DateTime.MinValue,
                Nextel = "",
                Nome = motorista.Nome.Left(40) ?? "",
                NomeMae = "",
                OrgaoEmissor = "",
                Rg = motorista.RG,
                TelefoneCelular = motorista.Celular?.Replace(" ", "") ?? "",
                TelefoneFixo = "",
                Vinculo = "CARRETEIRO"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade ObterOrigem(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> destinatariosPorRotaFrete)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (carga.Empresa == null)
                    throw new ServicoException("Não existe uma empresa informada para a carga.");

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(carga.Empresa.Codigo);

                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade()
                {
                    Cnpj = "0" + empresa.CNPJ_Formatado,
                    Codigo = "",
                    EnderecoBairro = empresa.Bairro.Left(30),
                    EnderecoCep = string.IsNullOrWhiteSpace(empresa.CEP) ? "" : string.Format(@"{0:00000\-000}", int.Parse(empresa.CEP)),
                    EnderecoCidade = empresa.Localidade?.Descricao.Left(50) ?? "",
                    EnderecoComplemento = empresa.Complemento.Left(20) ?? "",
                    EnderecoNumero = empresa.Numero.Left(7) ?? "",
                    EnderecoRua = empresa.Endereco.Left(30) ?? "",
                    EnderecoUf = empresa.Localidade?.Estado?.Sigla ?? "",
                    Latitude = "",
                    Longitude = "",
                    NomeFantasia = empresa.NomeFantasia.Left(50) ?? "",
                    RazaoSocial = empresa.RazaoSocial.Left(50) ?? "",
                    Responsavel = "",
                    ResponsavelCargo = "",
                    ResponsavelCelular = "",
                    ResponsavelEmail = "",
                    ResponsavelTelefone = "",
                    TelefonePrincipal = empresa.Telefone.Replace(" ", "") ?? "",
                    TelefoneSecundario = "",
                    Tipo = "OUTROS",
                    Unidade = "Unidade"
                };
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem primeiroPontoPassagem = destinatariosPorRotaFrete.FirstOrDefault();

                Dominio.Entidades.Cliente cliente = primeiroPontoPassagem.Cliente;

                if (cliente == null)
                    throw new ServicoException("Não existe pontos de origem para a carga.");

                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade()
                {
                    Cnpj = "0" + cliente.CPF_CNPJ_Formatado,
                    Codigo = "",
                    EnderecoBairro = cliente?.Bairro.Left(30) ?? "",
                    EnderecoCep = string.IsNullOrWhiteSpace(cliente?.CEP) ? "" : string.Format(@"{0:00000\-000}", cliente.CEP),
                    EnderecoCidade = carga.Filial.Localidade?.Descricao.Left(50) ?? "",
                    EnderecoComplemento = cliente?.Complemento.Left(20) ?? "",
                    EnderecoNumero = cliente?.Numero.Left(7) ?? "",
                    EnderecoRua = cliente?.Endereco.Left(30) ?? "",
                    EnderecoUf = carga.Filial.Localidade?.Estado?.Sigla ?? "",
                    Latitude = "",
                    Longitude = "",
                    NomeFantasia = carga.Filial.Descricao.Left(50) ?? "",
                    RazaoSocial = carga.Filial.Descricao.Left(50) ?? "",
                    Responsavel = "",
                    ResponsavelCargo = "",
                    ResponsavelCelular = "",
                    ResponsavelEmail = "",
                    ResponsavelTelefone = "",
                    TelefonePrincipal = cliente?.Celular?.Replace(" ", "") ?? "",
                    TelefoneSecundario = "",
                    Tipo = "OUTROS",
                    Unidade = "Unidade"
                };

                destinatariosPorRotaFrete.Remove(primeiroPontoPassagem);
            }

            return retorno;
        }

        private string ObterPlacaFormatada(string placa)
        {
            if (string.IsNullOrEmpty(placa))
                return placa;

            if (placa.Contains("-"))
                return placa;

            return placa.Insert(3, "-");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Requisicao ObterRequisicao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.KronaService kronaService = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.KronaService();
            Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.ElementAt(0) : null;
            Dominio.Entidades.Veiculo segundoReboque = carga.VeiculosVinculados?.Count > 1 ? carga.VeiculosVinculados.ElementAt(1) : null;
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> destinatariosPorRotaFrete = repCargaRotaFretePontosPassagem.ObterDestinatariosDaCargaRotaFretePorCarga(carga.Codigo);

            kronaService.Autenticacao = ObterAutenticacao(carga);
            kronaService.Origem = ObterOrigem(carga, tipoServicoMultisoftware, destinatariosPorRotaFrete);
            kronaService.Destinos = ObterDestinos(carga, destinatariosPorRotaFrete);
            kronaService.Motorista = ObterMotorista(carga);
            kronaService.Reboque = ObterVeiculo(reboque);
            kronaService.SegundoReboque = ObterVeiculo(segundoReboque);
            kronaService.Transportador = ObterTransportador(carga);
            kronaService.Veiculo = ObterVeiculo(carga.Veiculo);
            kronaService.Viagem = ObterViagem(carga, tipoServicoMultisoftware);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Requisicao()
            {
                KronaService = kronaService
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.RequisicaoCancelamento ObterRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.KronaServiceCancelamento kronaService = new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.KronaServiceCancelamento();

            kronaService.Autenticacao = ObterAutenticacao(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga);
            kronaService.Viagem = ObterViagem(cargaCancelamentoCargaIntegracao);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.RequisicaoCancelamento()
            {
                KronaService = kronaService
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade ObterTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Entidade()
            {
                Cnpj = "0" + carga.Empresa.CNPJ_Formatado,
                Codigo = "",
                EnderecoBairro = carga.Empresa.Bairro.Left(30) ?? "",
                EnderecoCep = string.IsNullOrWhiteSpace(carga.Empresa.CEP) ? "" : string.Format(@"{0:00000\-000}", int.Parse(carga.Empresa.CEP)),
                EnderecoCidade = carga.Empresa.Localidade?.Descricao.Left(50) ?? "",
                EnderecoComplemento = carga.Empresa.Complemento.Left(20) ?? "",
                EnderecoNumero = carga.Empresa.Numero.Left(7) ?? "",
                EnderecoRua = carga.Empresa.Endereco.Left(30) ?? "",
                EnderecoUf = carga.Empresa.Localidade?.Estado?.Sigla ?? "",
                Latitude = "",
                Longitude = "",
                NomeFantasia = carga.Empresa.NomeFantasia.Left(50) ?? "",
                RazaoSocial = carga.Empresa.RazaoSocial.Left(50) ?? "",
                Responsavel = "",
                ResponsavelCargo = "",
                ResponsavelCelular = "",
                ResponsavelEmail = "",
                ResponsavelTelefone = "",
                TelefonePrincipal = carga.Empresa.Telefone?.Replace(" ", "") ?? "",
                TelefoneSecundario = "",
                Tipo = "TRANSPORTADOR",
                Unidade = "Unidade"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Veiculo ObterVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Veiculo
            {
                Ano = veiculo.AnoModelo.ToString(),
                Capacidade = "",
                Comunicacao = veiculo?.TipoComunicacaoRastreador?.Descricao?.ToUpper() ?? "OUTROS",
                ComunicacaoSecundaria = "",
                Cor = veiculo?.CorVeiculo?.Descricao.Left(15)?.ToUpper() ?? "",
                EnderecoBairro = veiculo.Empresa?.Bairro.Left(30) ?? "",
                EnderecoCep = string.IsNullOrWhiteSpace(veiculo.Empresa?.CEP) ? "" : string.Format(@"{0:00000\-000}", int.Parse(veiculo.Empresa.CEP)),
                EnderecoCidade = veiculo.Empresa?.Localidade?.Descricao.Left(50) ?? "",
                EnderecoComplemento = veiculo.Empresa?.Complemento.Left(20) ?? "",
                EnderecoNumero = veiculo.Empresa?.Numero.Left(7) ?? "",
                EnderecoRua = veiculo.Empresa?.Endereco.Left(30) ?? "",
                EnderecoUf = veiculo.Empresa?.Localidade?.Estado?.Sigla ?? "",
                Fixo = "N",
                FrotaNumero = "",
                FrotaTransportador = "",
                IdRastreador = veiculo?.NumeroEquipamentoRastreador ?? "",
                IdRastreadorSecundario = "",
                Marca = veiculo.Marca?.Descricao.Left(13)?.ToUpper() ?? "",
                Modelo = veiculo.ModeloVeicularCarga?.Descricao.Left(13)?.ToUpper() ?? "",
                NumeroAntt = "",
                Placa = ObterPlacaFormatada(veiculo.Placa.ObterPlacaFormatada()),
                Proprietario = veiculo.Proprietario?.Descricao.Left(40) ?? "",
                ProprietarioCpfCnpj = veiculo.Proprietario != null ? FormatarCNPJCPF(veiculo.Proprietario) : "",
                Renavan = veiculo.Renavam ?? "",
                Tecnologia = veiculo?.TecnologiaRastreador?.Descricao?.ToUpper() ?? "",
                TecnologiaSecundaria = "",
                Tipo = veiculo?.ModeloVeicularCarga?.Descricao?.ToUpper() ?? "",
                ValidadeAntt = veiculo?.DataValidadeANTT ?? DateTime.MinValue
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Viagem ObterViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIsca repositorioCargaIsca = new Repositorio.Embarcador.Cargas.CargaIsca(_unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repIntegracaoMotorista = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarUltimaCargaEntrega(carga.Codigo);

            decimal valorCarga = repositorioPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
            bool existeNotaComClassificacaoNFEletronicos = repositorioPedidoXMLNotaFiscal.VerificarSeExisteNota(carga.Codigo, ClassificacaoNFe.NFEletronicos);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIsca> iscas = repositorioCargaIsca.BuscarPorCarga(carga.Codigo);
            List<(string Id, string Descricao)> localizadores = new List<(string Id, string Descricao)>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIsca cargaIsca in iscas)
                localizadores.Add((cargaIsca.Isca.CodigoIntegracao, cargaIsca.Isca.Descricao));

            while (localizadores.Count < 9)
                localizadores.Add(("", ""));

            string liberacao = carga.ProtocoloIntegracaoGR;
            if (string.IsNullOrWhiteSpace(liberacao) && carga.Motoristas?.Count > 0)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracoesMotorista = repIntegracaoMotorista.BuscarPorMotoristaETipo(carga.Motoristas.FirstOrDefault().Codigo, TipoIntegracao.Telerisco);
                if (integracoesMotorista != null && !string.IsNullOrWhiteSpace(integracoesMotorista.Protocolo))
                    liberacao = integracoesMotorista.Protocolo;
            }

            string identificadorMercadoria = carga.TipoDeCarga?.IdentificacaoMercadoriaKrona?.Identificador.ToString() ?? "";
            if (configuracao.ExibirClassificacaoNFe)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                //Quando tiver NFe com classificação NF Eletronicos deve ser 85, demais casos dweve ser 83
                if ((from o in pedidosNotasFiscais where o.XMLNotaFiscal.ClassificacaoNFe == ClassificacaoNFe.NFEletronicos select o).Any())
                    identificadorMercadoria = "85";
                else
                    identificadorMercadoria = "83";
            }

            DateTime dataInicioViagemPrevista = carga.Pedidos.FirstOrDefault()?.Pedido.DataPrevisaoSaida ?? carga.DataInicioViagemPrevista ?? DateTime.Now;
            DateTime dataFimViagemPrevista = cargaEntrega?.DataPrevista ?? carga.DataFimViagemPrevista ?? DateTime.Now;

            if (dataFimViagemPrevista < dataInicioViagemPrevista || dataFimViagemPrevista == dataInicioViagemPrevista)
                dataFimViagemPrevista = dataInicioViagemPrevista.AddDays(10);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Viagem()
            {
                DocaOrigem = "",
                Ffp = "",
                IdentificadorMercadoria = identificadorMercadoria,
                IdLocalizadorDoisPrimeiro = localizadores[3].Id,
                IdLocalizadorDoisSegundo = localizadores[4].Id,
                IdLocalizadorDoisTerceiro = localizadores[5].Id,
                IdLocalizadorTresPrimeiro = localizadores[6].Id,
                IdLocalizadorTresSegundo = localizadores[7].Id,
                IdLocalizadorTresTerceiro = localizadores[8].Id,
                IdLocalizadorUmPrimeiro = localizadores[0].Id,
                IdLocalizadorUmSegundo = localizadores[1].Id,
                IdLocalizadorUmTerceiro = localizadores[2].Id,
                Liberacao = liberacao,
                LocalizadorDoisPrimeiro = localizadores[3].Descricao,
                LocalizadorDoisSegundo = localizadores[4].Descricao,
                LocalizadorDoisTerceiro = localizadores[5].Descricao,
                LocalizadorTresPrimeiro = localizadores[6].Descricao,
                LocalizadorTresSegundo = localizadores[7].Descricao,
                LocalizadorTresTerceiro = localizadores[8].Descricao,
                LocalizadorUmPrimeiro = localizadores[0].Descricao,
                LocalizadorUmSegundo = localizadores[1].Descricao,
                LocalizadorUmTerceiro = localizadores[2].Descricao,
                NumeroCliente = carga.CodigoCargaEmbarcador,
                Observacao = "",
                Percurso = "RODOVIÁRIO",
                PrevisaoFim = dataFimViagemPrevista,
                PrevisaoInicio = dataInicioViagemPrevista,
                Rastreada = "S",
                Rota = carga.Rota?.Descricao.Left(255) ?? "",
                TipoCliente = "EMBARCADOR",
                TipoViagem = existeNotaComClassificacaoNFEletronicos ? "DISTRIBUICAO ELETRONICOS" : (carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? "DISTRIBUICAO LOJA"),
                Valor = valorCarga
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.ViagemCancelamento ObterViagem(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            string protocoloIntegracaoCarga = ObterProtocoloIntegracaoCarga(cargaCancelamentoCargaIntegracao);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.ViagemCancelamento()
            {
                Id = protocoloIntegracaoCarga,
                Cancelar = true,
                MotivoCancelamento = cargaCancelamentoCargaIntegracao.CargaCancelamento.MotivoCancelamento
            };
        }

        private string ObterProtocoloIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repositorioCargaIntegracao.BuscarPorCargaETipoIntegracao(cancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, cancelamentoCargaIntegracao.TipoIntegracao.Codigo);

            if (string.IsNullOrWhiteSpace(cargaIntegracao?.Protocolo))
                throw new ServicoException("Naõ existe o protocolo de integração da carga com a Infolog");

            return cargaIntegracao.Protocolo;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.RequisicaoCancelamento dadosRequisicao = ObterRequisicao(cancelamentoCargaIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao);

                string mensagemErroIntegracao = string.Empty;
                string protocoloIntegracao = string.Empty;

                if (((HttpWebResponse)retornoRequisicao).StatusCode == HttpStatusCode.OK)
                {
                    using (System.IO.Stream streamDadosRetornoRequisicao = retornoRequisicao.GetResponseStream())
                    {
                        System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                        jsonRetorno = leitorDadosRetornoRequisicao.ReadToEnd();
                        KeyValuePair<string, string> dadosRetornoRequisicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonRetorno).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(dadosRetornoRequisicao.Key))
                        {
                            if (dadosRetornoRequisicao.Key.Contains("Erro"))
                                mensagemErroIntegracao = dadosRetornoRequisicao.Value;
                            else if (dadosRetornoRequisicao.Key.Contains("Protocolo"))
                                protocoloIntegracao = dadosRetornoRequisicao.Value;
                        }
                    }

                    cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cancelamentoCargaIntegracao.Protocolo = protocoloIntegracao;
                }
                else
                {
                    cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cancelamentoCargaIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração com a Krona. HTTP Status {(int)((HttpWebResponse)retornoRequisicao).StatusCode}";
                }

                retornoRequisicao.Close();

                servicoArquivoTransacao.Adicionar(cancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Krona";

                servicoArquivoTransacao.Adicionar(cancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCancelamentoCargaIntegracao.Atualizar(cancelamentoCargaIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Krona.Requisicao dadosRequisicao = ObterRequisicao(cargaIntegracao.Carga, tipoServicoMultisoftware);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao);

                string mensagemErroIntegracao = string.Empty;
                string protocoloIntegracao = string.Empty;

                if (((HttpWebResponse)retornoRequisicao).StatusCode == HttpStatusCode.OK)
                {
                    using (System.IO.Stream streamDadosRetornoRequisicao = retornoRequisicao.GetResponseStream())
                    {
                        System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                        jsonRetorno = leitorDadosRetornoRequisicao.ReadToEnd();
                        KeyValuePair<string, string> dadosRetornoRequisicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonRetorno).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(dadosRetornoRequisicao.Key))
                        {
                            if (dadosRetornoRequisicao.Key.Contains("Erro"))
                                mensagemErroIntegracao = dadosRetornoRequisicao.Value;
                            else if (dadosRetornoRequisicao.Key.Contains("Protocolo"))
                                protocoloIntegracao = dadosRetornoRequisicao.Value;
                        }
                    }
                }
                else
                    mensagemErroIntegracao = $"Ocorreu uma falha ao realizar a integração com a Krona. HTTP Status {(int)((HttpWebResponse)retornoRequisicao).StatusCode}";

                retornoRequisicao.Close();

                if (string.IsNullOrWhiteSpace(mensagemErroIntegracao))
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaIntegracao.Protocolo = protocoloIntegracao;
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErroIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Krona";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private string FormatarCNPJCPF(Dominio.Entidades.Cliente cliente)
        {
            if (cliente.Tipo.Equals("E"))
            {
                return "000.000.000/0000-00";
            }
            else
            {
                return cliente.Tipo.Equals("J") ? String.Format(@"{0:000\.000\.000\/0000\-00}", cliente.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", cliente.CPF_CNPJ);
            }
        }

        #endregion
    }
}
