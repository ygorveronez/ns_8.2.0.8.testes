using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public sealed class AlteracaoPedido : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido _configuracaoPedido;


        #endregion

        #region Construtores

        public AlteracaoPedido(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public AlteracaoPedido(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido AdicionarAlteracaoPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido)
        {
            if (alteracaoPedido.ProtocoloIntegracaoPedido <= 0)
                throw new ServicoException($"O protocolo de integração do pedido deve ser informado para solicitar a alteração");

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedidoPendenteAprovacao = repositorioAlteracaoPedido.BuscarPendentePorPedido(alteracaoPedido.ProtocoloIntegracaoPedido);

            if (alteracaoPedidoPendenteAprovacao != null)
                throw new ServicoException($"Já existe uma alteração para o pedido com o protocolo de integração {alteracaoPedido.ProtocoloIntegracaoPedido} pendente de aprovação");

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(alteracaoPedido.ProtocoloIntegracaoPedido);

            if (pedido == null)
                throw new ServicoException($"Não foi possível encontrar o pedido com o protocolo de integração {alteracaoPedido.ProtocoloIntegracaoPedido}");

            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedidoAdicionar = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido()
            {
                DataCriacao = DateTime.Now,
                Companhia = alteracaoPedido.Companhia?.Trim(),
                DataETA = alteracaoPedido.ETA?.ToNullableDateTime(),
                Destinatario = AdicionarAlteracaoPedidoPessoa(alteracaoPedido.Destinatario, TipoAlteracaoPedidoPessoa.Destinatario),
                NumeroNavio = alteracaoPedido.Navio?.Trim(),
                Ordem = alteracaoPedido.Ordem?.Trim(),
                Pedido = pedido,
                PesoTotal = alteracaoPedido.PesoBrutoTotal,
                PortoChegada = alteracaoPedido.PortoChegada?.Trim(),
                PortoSaida = alteracaoPedido.PortoSaida?.Trim(),
                PrevisaoEntrega = alteracaoPedido.DataPrevisaoEntrega?.ToNullableDateTime(),
                Recebedor = AdicionarAlteracaoPedidoPessoa(alteracaoPedido.Recebedor, TipoAlteracaoPedidoPessoa.Recebedor),
                Remetente = AdicionarAlteracaoPedidoPessoa(alteracaoPedido.Remetente, TipoAlteracaoPedidoPessoa.Remetente),
                Reserva = alteracaoPedido.Reserva?.Trim(),
                Resumo = alteracaoPedido.Resumo?.Trim(),
                Temperatura = alteracaoPedido.Temperatura?.Trim(),
                TipoEmbarque = alteracaoPedido.TipoEmbarque?.Trim(),
                Vendedor = alteracaoPedido.Vendedor?.Trim()
            };

            repositorioAlteracaoPedido.Inserir(alteracaoPedidoAdicionar);

            return alteracaoPedidoAdicionar;
        }

        private Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente AdicionarAlteracaoPedidoPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, TipoAlteracaoPedidoPessoa tipoPessoa)
        {
            if (pessoa == null)
                return null;

            string codigoIntegracao = pessoa.CodigoIntegracao?.Trim();
            double cpfCnpj = pessoa.CPFCNPJ?.ObterSomenteNumeros().ToDouble() ?? 0d;

            if (string.IsNullOrWhiteSpace(codigoIntegracao) && (cpfCnpj <= 0d))
                throw new ServicoException($"O código de integração ou CPF/CNPJ do {tipoPessoa.ObterDescricao().ToLower()} deve ser informado");

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = null;
            Dominio.Entidades.Atividade atividadeClienteExterior = null;

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                cliente = repositorioCliente.BuscarPorCodigoIntegracao(codigoIntegracao);

            if ((cliente == null ) && (cpfCnpj > 0d))
                cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if ((cliente == null) && pessoa.ClienteExterior)
                cliente = repositorioCliente.BuscarPorRazaoExterior(pessoa.RazaoSocial, pessoa.Endereco?.Logradouro ?? "");

            if (cliente == null)
            {
                if (!pessoa.ClienteExterior)
                    throw new ServicoException($"Não foi possível encontrar o {tipoPessoa.ObterDescricao().ToLower()} informado");

                atividadeClienteExterior = Atividade.ObterAtividade(codigoEmpresa: 0, tipoCliente: "E", stringConexao: _unitOfWork.StringConexao, codigoAtividade: 0, unitOfWork: _unitOfWork);

                if (atividadeClienteExterior == null)
                    throw new ServicoException($"A atividade do {tipoPessoa.ObterDescricao().ToLower()} é inválida");
            }

            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente pessoaAdicionar = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente()
            {
                AtividadeClienteExterior = atividadeClienteExterior,
                Cliente = cliente,
                ClienteExterior = pessoa.ClienteExterior,
                CodigoIntegracao = codigoIntegracao,
                CpfCnpj = cpfCnpj,
                IeRg = string.IsNullOrWhiteSpace(pessoa.RGIE) ? "ISENTO" : pessoa.RGIE?.Trim(),
                Nome = pessoa.RazaoSocial?.Trim()
            };

            if (string.IsNullOrWhiteSpace(pessoaAdicionar.Nome))
                throw new ServicoException($"O nome do {tipoPessoa.ObterDescricao().ToLower()} deve ser informado");

            if (pessoaAdicionar.Nome.Length < 3)
                throw new ServicoException($"O nome do {tipoPessoa.ObterDescricao().ToLower()} deve conter mais que 2 caracteres");

            if (pessoaAdicionar.Nome.Length > 80)
                pessoaAdicionar.Nome = pessoaAdicionar.Nome.Substring(0, 80);

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente repositorioAlteracaoPedidoCliente = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente(_unitOfWork);

            pessoaAdicionar.Endereco = AdicionarAlteracaoPedidoPessoaEndereco(pessoa, tipoPessoa);

            repositorioAlteracaoPedidoCliente.Inserir(pessoaAdicionar);

            return pessoaAdicionar;
        }

        private Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco AdicionarAlteracaoPedidoPessoaEndereco(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, TipoAlteracaoPedidoPessoa tipoPessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco = pessoa.Endereco;

            if (endereco == null)
            {
                if (pessoa.ClienteExterior)
                    throw new ServicoException($"O endereço do {tipoPessoa.ObterDescricao().ToLower()} deve ser informado");

                return null;
            }

            if (endereco.Cidade == null)
                throw new ServicoException($"A localidade do {tipoPessoa.ObterDescricao().ToLower()} deve ser informada");

            if (string.IsNullOrWhiteSpace(endereco.Cidade.CodigoIntegracao) && (endereco.Cidade.IBGE <= 0))
                throw new ServicoException($"O código de integração ou do IBGE da localidade do {tipoPessoa.ObterDescricao().ToLower()} deve ser informado");

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = null;

            if (endereco.Cidade.IBGE > 0)
            {
                localidade = repositorioLocalidade.BuscarPorCodigoIBGE(endereco.Cidade.IBGE);

                if (localidade == null)
                    throw new ServicoException($"Não foi possível encontrar a localidade através do código do IBGE informado no endereço do {tipoPessoa.ObterDescricao().ToLower()}");
            }
            else
            {
                localidade = repositorioLocalidade.buscarPorCodigoEmbarcador(endereco.Cidade.CodigoIntegracao.Trim());

                if (localidade == null)
                    throw new ServicoException($"Não foi possível encontrar a localidade através do código de integração informado no endereço do {tipoPessoa.ObterDescricao().ToLower()}");
            }

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco repositorioAlteracaoPedidoEndereco = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco enderecoAdicionar = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco()
            {
                Bairro = endereco.Bairro?.Trim(),
                Cep = endereco.CEP?.Trim(),
                Localidade = localidade,
                Logradouro = endereco.Logradouro?.Trim(),
                Numero = endereco.Numero?.Trim()
            };

            if (string.IsNullOrWhiteSpace(enderecoAdicionar.Bairro) || (pessoa.ClienteExterior && enderecoAdicionar.Bairro.Length < 3))
                enderecoAdicionar.Bairro = "S/B";
            else
            {
                if (enderecoAdicionar.Bairro.Length < 2)
                    throw new ServicoException($"O bairro do {tipoPessoa.ObterDescricao().ToLower()} deve conter mais que 1 caracter");

                if (enderecoAdicionar.Bairro.Length > 60)
                    enderecoAdicionar.Bairro = enderecoAdicionar.Bairro.Substring(0, 60);
            }

            if (string.IsNullOrWhiteSpace(enderecoAdicionar.Logradouro))
                throw new ServicoException($"O logradouro do {tipoPessoa.ObterDescricao().ToLower()} deve ser informado");

            if (enderecoAdicionar.Logradouro.Length < 3)
                enderecoAdicionar.Logradouro = $"Logradouro {enderecoAdicionar.Logradouro}";
            else if (enderecoAdicionar.Logradouro.Length > 80)
                enderecoAdicionar.Logradouro = enderecoAdicionar.Logradouro.Substring(0, 80);

            if (string.IsNullOrWhiteSpace(enderecoAdicionar.Numero))
                enderecoAdicionar.Numero = "S/N";

            repositorioAlteracaoPedidoEndereco.Inserir(enderecoAdicionar);

            return enderecoAdicionar;
        }

        private void AplicarAlteracao(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AplicarAlteracaoPedido(alteracaoPedido);
            AtualizarCargas(alteracaoPedido, cargas, tipoServicoMultisoftware);
        }

        private void AplicarAlteracaoPedido(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Pedido servicoPedido = new Pedido();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = alteracaoPedido.Pedido;

            pedido.Companhia = alteracaoPedido.Companhia;
            pedido.DataETA = alteracaoPedido.DataETA;
            pedido.Destinatario = AplicarAlteracaoPedidoPessoa(alteracaoPedido.Destinatario);
            pedido.NumeroNavio = alteracaoPedido.NumeroNavio;
            pedido.Ordem = alteracaoPedido.Ordem;
            pedido.PesoTotal = alteracaoPedido.PesoTotal;
            pedido.PortoChegada = alteracaoPedido.PortoChegada;
            pedido.PortoSaida = alteracaoPedido.PortoSaida;
            pedido.PrevisaoEntrega = alteracaoPedido.PrevisaoEntrega;
            pedido.Recebedor = AplicarAlteracaoPedidoPessoa(alteracaoPedido.Recebedor);
            pedido.Remetente = AplicarAlteracaoPedidoPessoa(alteracaoPedido.Remetente);
            pedido.Reserva = alteracaoPedido.Reserva;
            pedido.Resumo = alteracaoPedido.Resumo;
            pedido.Temperatura = alteracaoPedido.Temperatura;
            pedido.TipoEmbarque = alteracaoPedido.TipoEmbarque;
            pedido.Vendedor = alteracaoPedido.Vendedor;

            if (!pedido.UsarOutroEnderecoOrigem && (pedido.Remetente != null))
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;

                    repositorioPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                }
            }

            if (!pedido.UsarOutroEnderecoDestino)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = null;

                if (pedido.Recebedor != null)
                {
                    pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);
                }
                else if (pedido.Destinatario != null)
                {
                    pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
                }

                if (pedidoEnderecoDestino?.Localidade != null)
                {
                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                    pedido.EnderecoDestino = pedidoEnderecoDestino;

                    repositorioPedidoEndereco.Inserir(pedidoEnderecoDestino);
                }
            }

            repositorioPedido.Atualizar(pedido);
        }

        private Dominio.Entidades.Cliente AplicarAlteracaoPedidoPessoa(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente pessoa)
        {
            if (pessoa == null)
                return null;

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            if (pessoa.Cliente == null)
            {
                pessoa.Cliente = repositorioCliente.BuscarPorRazaoExterior(pessoa.Nome, pessoa.Endereco?.Logradouro ?? "");

                if (pessoa.Cliente != null)
                {
                    Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente repositorioAlteracaoPedidoCliente = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente(_unitOfWork);

                    repositorioAlteracaoPedidoCliente.Atualizar(pessoa);
                }
            }

            if (pessoa.Cliente != null)
            {
                Dominio.Entidades.Cliente cliente = pessoa.Cliente;

                cliente.IE_RG = pessoa.IeRg;
                cliente.Nome = pessoa.Nome;

                if (pessoa.Endereco != null && !cliente.NaoAtualizarDados)
                {
                    cliente.Bairro = pessoa.Endereco.Bairro;
                    cliente.CEP = pessoa.Endereco.Cep;
                    cliente.Endereco = pessoa.Endereco.Logradouro;
                    cliente.Localidade = pessoa.Endereco.Localidade;
                    cliente.Numero = pessoa.Endereco.Numero;
                }

                repositorioCliente.Atualizar(cliente);

                return cliente;
            }

            return AplicarAlteracaoPedidoPessoaExterior(pessoa);
        }

        private Dominio.Entidades.Cliente AplicarAlteracaoPedidoPessoaExterior(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente pessoa)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente()
            {
                Atividade = pessoa.AtividadeClienteExterior,
                Ativo = true,
                Bairro = pessoa.Endereco.Bairro,
                CEP = pessoa.Endereco.Cep,
                Cidade = pessoa.Endereco.Localidade.Descricao,
                CodigoIntegracao = pessoa.CodigoIntegracao,
                CPF_CNPJ = pessoa.CpfCnpj > 0d ? pessoa.CpfCnpj : repositorioCliente.BuscarPorProximoExterior(),
                DataCadastro = DateTime.Now,
                Email = "",
                EmailStatus = "I",
                EmailContador = "",
                EmailContadorStatus = "I",
                EmailContato = "",
                EmailContatoStatus = "I",
                Endereco = pessoa.Endereco.Logradouro,
                IE_RG = pessoa.IeRg,
                IndicadorIE = pessoa.IeRg == "ISENTO" ? IndicadorIE.ContribuinteIsento : IndicadorIE.ContribuinteICMS,
                Localidade = pessoa.Endereco.Localidade,
                Nome = pessoa.Nome,
                Numero = !string.IsNullOrEmpty(pessoa.Endereco.Numero) ? pessoa.Endereco.Numero : "S/N",
                Pais = pessoa.Endereco.Localidade.Pais,
                Tipo = "E",
                TipoLocalizacao = TipoLocalizacao.endereco
            };

            repositorioCliente.Inserir(cliente);

            return cliente;
        }

        private void AtualizarCargas(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(_unitOfWork);

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Carga.CargaLocaisPrestacao(_unitOfWork);
            Carga.Rota servicoRota = new Carga.Rota(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = ObterConfiguracaoPedido();

            for (int i = 0; i < cargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = carga.Pedidos.ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoAtualizar = (from o in cargasPedido where o.Pedido.Codigo == alteracaoPedido.Pedido.Codigo select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoAtualizar)
                {
                    cargaPedido.Destino = alteracaoPedido.Pedido.Destino;
                    cargaPedido.Origem = alteracaoPedido.Pedido.Origem;
                    cargaPedido.Recebedor = alteracaoPedido.Pedido.Recebedor;

                    repositorioCargaPedido.Atualizar(cargaPedido);
                }

                Carga.RotaFrete.SetarRotaFreteCarga(carga, cargasPedido, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                servicoRota.DeletarPercursoDestinosCarga(carga, _unitOfWork);
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargasPedido, _unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargasPedido, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);

                if (!carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.AgNFe))
                {
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                    carga.CalculandoFrete = true;
                    carga.DataInicioCalculoFrete = DateTime.Now;
                }
                else if ((carga.SituacaoCarga == SituacaoCarga.CalculoFrete) && !carga.CalculandoFrete)
                {
                    carga.CalculandoFrete = true;
                    carga.DataInicioCalculoFrete = DateTime.Now;
                }

                if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                    carga.CarregamentoIntegradoERP = false;

                    repositorioCarga.Atualizar(carga);
            }
        }

        private void CriarAprovacao(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> regras = ObterRegrasAutorizacao(alteracaoPedido);

            if (regras.Count > 0)
                CriarRegrasAprovacao(alteracaoPedido, regras, tipoServicoMultisoftware);
            else
                throw new ServicoException("Não foi encontrada nenhuma regra para aprovar a alteração de pedido");
        }

        private void CriarAprovacaoTransportador(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Empresa> transportadores = (from o in cargas where o.Empresa != null select o.Empresa).Distinct().ToList();

            if (transportadores.Count == 0)
            {
                alteracaoPedido.Situacao = SituacaoAlteracaoPedido.Aprovada;
                AplicarAlteracao(alteracaoPedido, cargas, tipoServicoMultisoftware);
                return;
            }

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(_unitOfWork);

            foreach (Dominio.Entidades.Empresa transportador in transportadores)
            {
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador aprovacaoTransportador = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador()
                {
                    AlteracaoPedido = alteracaoPedido,
                    DataCriacao = DateTime.Now,
                    Situacao = SituacaoAlcadaRegra.Pendente,
                    Transportador = transportador
                };

                repositorioAprovacaoTransportador.Inserir(aprovacaoTransportador);
            }
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido aprovacao = new Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido()
                        {
                            OrigemAprovacao = alteracaoPedido,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = alteracaoPedido.DataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(alteracaoPedido, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido()
                    {
                        OrigemAprovacao = alteracaoPedido,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = alteracaoPedido.DataCriacao,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            alteracaoPedido.Situacao = existeRegraSemAprovacao ? SituacaoAlteracaoPedido.AguardandoAprovacao : SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }


        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido ObterConfiguracaoPedido()
        {
            if (_configuracaoPedido == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
                _configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            }

            return _configuracaoPedido;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, alteracaoPedido.Pedido.Filial?.Codigo))
                    continue;

                if (regra.RegraPorTipoCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoCarga, alteracaoPedido.Pedido.TipoDeCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, alteracaoPedido.Pedido.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, 0))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private void ValidarSituacaoCargasPermitemAlterarPedido(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasNaoPermitemAlteracao = (
                from o in cargas
                where !servicoCarga.VerificarSeCargaEstaNaLogistica(o, tipoServicoMultisoftware) && (o.SituacaoCarga != SituacaoCarga.AgNFe)
                select o
            ).ToList();

            if (cargasNaoPermitemAlteracao.Count > 0)
            {
                if (cargasNaoPermitemAlteracao.Count == 1)
                    throw new ServicoException($"A situação da carga {cargasNaoPermitemAlteracao.FirstOrDefault().CodigoCargaEmbarcador} não permite a alteração de pedido");

                throw new ServicoException($"A situação das cargas ({string.Join(", ", (from o in cargasNaoPermitemAlteracao select o.CodigoCargaEmbarcador))}) não permitem a alteração de pedido");
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: alteracaoPedido.Codigo,
                URLPagina: "Pedidos/Pedido",
                titulo: Localization.Resources.Pedidos.Pedido.AlteracaoPedido,
                nota: string.Format(Localization.Resources.Pedidos.Pedido.CriadaSolicitacaoParaAlteracaoPedido, alteracaoPedido.Pedido.NumeroPedidoEmbarcador),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedidoAdicionada = AdicionarAlteracaoPedido(alteracaoPedido);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedido(alteracaoPedidoAdicionada.Pedido.Codigo);

            ValidarSituacaoCargasPermitemAlterarPedido(cargas, tipoServicoMultisoftware);
            CriarAprovacao(alteracaoPedidoAdicionada, tipoServicoMultisoftware);

            if (alteracaoPedidoAdicionada.Situacao == SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador)
                CriarAprovacaoTransportador(alteracaoPedidoAdicionada, cargas, tipoServicoMultisoftware);

            repositorioAlteracaoPedido.Atualizar(alteracaoPedidoAdicionada);
        }

        public void Aplicar(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedido(alteracaoPedido.Pedido.Codigo);

            ValidarSituacaoCargasPermitemAlterarPedido(cargas, tipoServicoMultisoftware);
            AplicarAlteracao(alteracaoPedido, cargas, tipoServicoMultisoftware);
        }

        public void ValidarAlteracaoPedidoPendenteAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedidoPendenteAprovacao = repositorioAlteracaoPedido.BuscarPendentePorCarga(carga.Codigo);

            if (alteracaoPedidoPendenteAprovacao != null)
                throw new ServicoException($"Existe uma alteração do pedido {alteracaoPedidoPendenteAprovacao.Pedido.NumeroPedidoEmbarcador} pendente de aprovação");
        }

        public void VerificarAprovacaoTransportador(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (alteracaoPedido.Situacao != SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedido(alteracaoPedido.Pedido.Codigo);

            CriarAprovacaoTransportador(alteracaoPedido, cargas, tipoServicoMultisoftware);
        }

        #endregion
    }
}
