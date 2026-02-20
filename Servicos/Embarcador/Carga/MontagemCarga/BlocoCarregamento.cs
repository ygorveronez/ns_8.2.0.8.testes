using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public sealed class BlocoCarregamento
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private string _blocoAtual;
        private decimal _cubagemBloco;
        private int _ordemCarregamento;
        private decimal _pesoBloco;
        private bool _primeiraPassagem;

        #endregion

        #region Construtores

        public BlocoCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarOrdemEntrega(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamento)
        {
            if (blocosCarregamento.Count > 0)
            {
                int maiorOrdemCarregamento = blocosCarregamento.Max(o => o.OrdemCarregamento);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamento in blocosCarregamento)
                    blocoCarregamento.OrdemEntrega = (maiorOrdemCarregamento + 1 - blocoCarregamento.OrdemCarregamento);
            }
        }

        private void GerarBlocosCarregamento(
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento,
            decimal pesoBlocos,
            decimal cubagem,
            decimal pesoRequest)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoConfiguracaoMontagemCarga = new(_unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> listaBlocoCarregamentoSegundoTrecho = repositorioBlocoCarregamento.BuscarBlocoCarregamentoSegundoTrechoPorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
            int totalBlocosCarregamento =  repositorioBlocoCarregamento.ContarPorCarregamento(carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesPedidos = ObterClientesPedido(carregamento);

            if (totalBlocosCarregamento > 0 && configuracaoMontagemCarga.ConsiderarSomentePesoOuCubagemAoGerarBloco && pesoRequest <= 0 && cubagem <= 0)
                throw new ServicoException("Peso do bloco ou Cubagem do bloco é obrigatório quando a configuração 'Considerar somente peso ou cubagem ao gerar bloco' estiver habilitada");

            List<double> cpfCnpjsClientesEntregas = (from cl in clientesPedidos select cl.Cliente.CPF_CNPJ).ToList();
            List<double> cpfCnpjsRecebedores = new List<double>();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamentoAdicionar = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamentoSegundoTrechoAdicionar = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            InicializarAtributosControle();

            if (configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor)
            {
                cpfCnpjsClientesEntregas = new List<double>();

                // o clientesPedidos possui a ordem roteirizada.. e "devemos" obdecer
                for (int i = 0; i < clientesPedidos.Count; i++)
                {
                    double cpfCnpjClienteEntrega = clientesPedidos[i].Cliente.CPF_CNPJ;
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (
                        from carregamentoPedido in carregamento.Pedidos
                        where (
                            (
                                (carregamentoPedido.Pedido.Recebedor == null && carregamentoPedido.Pedido.Destinatario.CPF_CNPJ == cpfCnpjClienteEntrega) ||
                                (carregamentoPedido.Pedido.Recebedor != null && carregamentoPedido.Pedido.Recebedor.CPF_CNPJ == cpfCnpjClienteEntrega)
                            )
                        )
                        select carregamentoPedido.Pedido
                    ).ToList();

                    for (int j = 0; j < pedidos.Count; j++)
                    {
                        double cpfCnpj = (pedidos[j].Recebedor?.CPF_CNPJ ?? pedidos[j].Destinatario.CPF_CNPJ);
                        if (!cpfCnpjsClientesEntregas.Contains(cpfCnpj))
                            cpfCnpjsClientesEntregas.Add(cpfCnpj);

                        if ((pedidos[j].Recebedor?.CPF_CNPJ ?? 0) != 0 && !cpfCnpjsRecebedores.Contains(cpfCnpj))
                            cpfCnpjsRecebedores.Add(cpfCnpj);
                    }
                }
            }

            // Se o CNPJ do grupo anterior foi um recebedor.... e o próximo não é.. vamos zerar e gerar novo bloco...
            bool recebedorAnterior = false;

            for (int i = 0; i < (cpfCnpjsClientesEntregas.Count - (configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor ? 0 : 1)); i++) // Desconsidera o último (Remetente)
            {
                double cpfCnpjClienteEntrega = cpfCnpjsClientesEntregas[i];
                List<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> listaBlocoCarregamentoSegundoTrechoCliente = listaBlocoCarregamentoSegundoTrecho.Where(o => o.CpfCnpjExpedidor == cpfCnpjClienteEntrega).ToList();

                if (listaBlocoCarregamentoSegundoTrechoCliente.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho carregamentoSegundoTrecho in listaBlocoCarregamentoSegundoTrechoCliente)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = ObterPedidoCarregamentoSegundoTrechoPorExpedidor(carregamento, carregamentoSegundoTrecho.CodigoPedido);

                        if (pedido != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamentoSegundoTrecho = ObterBlocoCarregamentoSegundoTrechoAdicionar(
                                carregamentoSegundoTrecho,
                                pedido,
                                pesoBlocos,
                                cubagem,
                                blocosCarregamentoAdicionar,
                                configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor,
                                configuracaoMontagemCarga.ConsiderarSomentePesoOuCubagemAoGerarBloco);

                            blocosCarregamentoSegundoTrechoAdicionar.Add(blocoCarregamentoSegundoTrecho);
                            blocosCarregamentoAdicionar.Add(ObterBlocoCarregamentoAdicionar(carregamento, pedido, pesoBlocos, cubagem, blocoCarregamentoSegundoTrecho));
                        }
                    }

                    _ordemCarregamento++;
                }

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPedidosCarregamentoSemSegundoTrecho(carregamento, cpfCnpjClienteEntrega, listaBlocoCarregamentoSegundoTrechoCliente);

                for (int p = 0; p < pedidos.Count; p++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidos[p];
                    blocosCarregamentoAdicionar.Add(ObterBlocoCarregamentoAdicionar(
                        carregamento,
                        pedido,
                        pesoBlocos,
                        cubagem,
                        blocosCarregamentoAdicionar,
                        configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor,
                        configuracaoMontagemCarga.ConsiderarSomentePesoOuCubagemAoGerarBloco));
                }

                recebedorAnterior = cpfCnpjsRecebedores.Contains(cpfCnpjClienteEntrega);
            }

            AtualizarOrdemEntrega(blocosCarregamentoAdicionar);

            try
            {
                _unitOfWork.Start();

                RemoverBlocosExistentes(carregamento, listaBlocoCarregamentoSegundoTrecho);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamentoSegundoTrecho in blocosCarregamentoSegundoTrechoAdicionar)
                    repositorioBlocoCarregamento.Inserir(blocoCarregamentoSegundoTrecho);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamento in blocosCarregamentoAdicionar)
                    repositorioBlocoCarregamento.Inserir(blocoCarregamento);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private void InicializarAtributosControle()
        {
            _blocoAtual = string.Empty;
            _cubagemBloco = 0;
            _ordemCarregamento = 1;
            _pesoBloco = 0;
            _primeiraPassagem = true;
        }

        private bool IsAdicionarNovoBloco(
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido,
            decimal pesoBlocos,
            decimal cubagem,
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamento,
            bool gerarUnicoBlocoPorRecebedor,
            bool considerarSomentePesoOuCubagemAoGerarBloco)
        {
            if (_primeiraPassagem)
            {
                _primeiraPassagem = false;
                return true;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosBlocoAtual = (from obj in blocosCarregamento where obj.Bloco == _blocoAtual select obj.Pedido).ToList();

            if (gerarUnicoBlocoPorRecebedor && pedido.Recebedor != null)
            {
                if (pedidosBlocoAtual.Count == 0)
                    return true;
                else if (pedidosBlocoAtual.Any(x => x.Recebedor != pedido.Recebedor))
                    return true;
                else
                    return false;
            }

            // Mesma cidade somente quando for "Destinatário" e não tiver recebedor..
            bool outraCidade = false;
            bool outroDestinatario = false;
            if (pedido.Recebedor == null)
            {
                if (pedidosBlocoAtual.Count > 0)
                {
                    int codigoLocalidade = pedido.Destinatario?.Localidade?.Codigo ?? 0;
                    //Se for da mesma cidade.. (Destinatarios diferentes.. considera o peso/ocupação) qquando mesmo destinatário mantem no mesmo bloco..
                    outraCidade = pedidosBlocoAtual.Any(x => x.Destinatario.Localidade.Codigo != codigoLocalidade);
                    outroDestinatario = pedidosBlocoAtual.Any(x => x.Destinatario != pedido.Destinatario);
                }
            }

            bool utilizarBlocoInteiro = (cubagem > 0 && pedido.CubagemTotal > cubagem) || (pesoBlocos > 0 && pedido.PesoTotal > pesoBlocos);
            bool cubagemUltrapassaDisponivelBloco = (cubagem > 0 && ((_cubagemBloco + pedido.CubagemTotal) > cubagem));
            bool pesoUltrapassaDisponivelBloco = ((_pesoBloco + pedido.PesoTotal) > pesoBlocos);

            if (considerarSomentePesoOuCubagemAoGerarBloco)
            {
                return pesoUltrapassaDisponivelBloco || cubagemUltrapassaDisponivelBloco;
            }

            if (gerarUnicoBlocoPorRecebedor)
                return (utilizarBlocoInteiro && outroDestinatario) || (cubagemUltrapassaDisponivelBloco && outroDestinatario) || (pesoUltrapassaDisponivelBloco && outroDestinatario) || outraCidade;
            else
                return utilizarBlocoInteiro || cubagemUltrapassaDisponivelBloco || pesoUltrapassaDisponivelBloco || outraCidade;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento ObterBlocoCarregamentoAdicionar(
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento,
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido,
            decimal pesoBlocos,
            decimal cubagem,
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamento,
            bool gerarUnicoBlocoPorRecebedor,
            bool considerarSomentePesoOuCubagemAoGerarBloco)
        {
            if (IsAdicionarNovoBloco(pedido, pesoBlocos, cubagem, blocosCarregamento, gerarUnicoBlocoPorRecebedor, considerarSomentePesoOuCubagemAoGerarBloco))
            {
                _blocoAtual = ObterSequenciaLetra(_blocoAtual);
                _cubagemBloco = 0;
                _pesoBloco = 0;
            }

            _cubagemBloco += pedido.CubagemTotal;
            _pesoBloco += pedido.PesoTotal;

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento
            {
                Bloco = _blocoAtual,
                Carregamento = carregamento,
                OrdemCarregamento = _ordemCarregamento,
                Pedido = pedido
            };

            _ordemCarregamento++;

            return blocoCarregamento;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento ObterBlocoCarregamentoAdicionar(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, decimal pesoBlocos, decimal cubagem, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamentoSegundoTrecho)
        {
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento
            {
                BlocoCarregamentoSegundoTrecho = blocoCarregamentoSegundoTrecho,
                Bloco = blocoCarregamentoSegundoTrecho.Bloco,
                Carregamento = carregamento,
                OrdemCarregamento = _ordemCarregamento,
                Pedido = pedido
            };

            return blocoCarregamento;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento ObterBlocoCarregamentoSegundoTrechoAdicionar(
            Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho carregamentoSegundoTrecho,
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido,
            decimal pesoBlocos,
            decimal cubagem,
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCarregamento,
            bool gerarUnicoBlocoPorRecebedor,
            bool considerarSomentePesoOuCubagemAoGerarBloco)
        {
            if (IsAdicionarNovoBloco(pedido, pesoBlocos, cubagem, blocosCarregamento, gerarUnicoBlocoPorRecebedor, considerarSomentePesoOuCubagemAoGerarBloco))
            {
                _blocoAtual = ObterSequenciaLetra(_blocoAtual);
                _cubagemBloco = 0;
                _pesoBloco = 0;
            }

            _cubagemBloco += pedido.CubagemTotal;
            _pesoBloco += pedido.PesoTotal;

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamentoSegundoTrecho = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento
            {
                Bloco = _blocoAtual,
                Carregamento = ObterCarregamento(carregamentoSegundoTrecho.CodigoCarregamentoSegundoTrecho),
                OrdemCarregamento = carregamentoSegundoTrecho.OrdemCarregamento,
                OrdemEntrega = carregamentoSegundoTrecho.OrdemEntrega,
                Pedido = pedido
            };

            return blocoCarregamentoSegundoTrecho;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento ObterCarregamento(int codigoCarregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioCarregamento.BuscarPorCodigo(codigoCarregamento);

            if (carregamento == null)
                throw new ServicoException("Falha ao buscar carregamento");

            return carregamento;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> ObterClientesPedido(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamentoRoteirizacao == null)
                throw new ServicoException("Antes de gerar os blocos de carregamento é necessário roterizar a carga");

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repositorioCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesPedidos = repositorioCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo).OrderByDescending(obj => obj.Ordem).ToList();

            return clientesPedidos;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoCarregamentoSegundoTrechoPorExpedidor(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, int codigoPedido)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = (
                from carregamentoPedido in carregamento.Pedidos
                where (carregamentoPedido.Pedido.Codigo == codigoPedido)
                select carregamentoPedido.Pedido
            ).FirstOrDefault();

            return pedido;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosCarregamentoSemSegundoTrecho(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, double cpfCnpjClienteEntrega, List<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> listaBlocoCarregamentoSegundoTrecho)
        {
            List<int> listaCOdigoPedidoCarregamentoSegundoTrecho = (from blocoCarregamentoSegundoTrecho in listaBlocoCarregamentoSegundoTrecho select blocoCarregamentoSegundoTrecho.CodigoPedido).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (
                from carregamentoPedido in carregamento.Pedidos
                where (
                    (
                        (carregamentoPedido.Pedido.Recebedor == null && carregamentoPedido.Pedido.Destinatario.CPF_CNPJ == cpfCnpjClienteEntrega) ||
                        (carregamentoPedido.Pedido.Recebedor != null && carregamentoPedido.Pedido.Recebedor.CPF_CNPJ == cpfCnpjClienteEntrega) ||
                        (carregamentoPedido.Pedido.Recebedor == null && cpfCnpjClienteEntrega == 0)
                    ) &&
                    !listaCOdigoPedidoCarregamentoSegundoTrecho.Contains(carregamentoPedido.Pedido.Codigo)
                )
                select carregamentoPedido.Pedido
            ).ToList();

            return pedidos;
        }

        private decimal ObterPesoBlocos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, decimal pesoBlocos)
        {
            if (pesoBlocos > 0)
                return pesoBlocos;

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesPedidos = ObterClientesPedido(carregamento);
            List<double> clientesEntrega = (from o in clientesPedidos select o.Cliente.CPF_CNPJ).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidos = (
                from carregamentoPedido in carregamento.Pedidos
                where (
                    (carregamentoPedido.Pedido.Recebedor == null && carregamentoPedido.Pedido.Destinatario != null && clientesEntrega.Contains(carregamentoPedido.Pedido.Destinatario.CPF_CNPJ)) ||
                    (carregamentoPedido.Pedido.Recebedor != null && clientesEntrega.Contains(carregamentoPedido.Pedido.Recebedor.CPF_CNPJ))
                )
                select carregamentoPedido
            ).ToList();

            pesoBlocos = pedidos.Sum(obj => obj.Pedido.PesoTotal);

            if (pesoBlocos == 0)
                throw new ServicoException("Sem informação para obter peso de divisão de blocos.");

            return pesoBlocos;
        }

        private char ObterProximaLetra(char letra)
        {
            string sequenciaAlfabeto = ObterSequenciaAlfabeto();
            int posicao = sequenciaAlfabeto.IndexOf(letra);

            return sequenciaAlfabeto[posicao + 1];
        }

        private string ObterSequenciaAlfabeto()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        private string ObterSequenciaLetra(string sequenciaAtual)
        {
            string sequenciaAlfabeto = ObterSequenciaAlfabeto();

            if (string.IsNullOrWhiteSpace(sequenciaAtual))
                return sequenciaAlfabeto.First().ToString();

            bool fimSequencia = false;
            bool incremetado = false;
            List<char> novaSequencia = new List<char>();

            for (int i = sequenciaAtual.Length - 1; i >= 0; i--)
            {
                char letra = sequenciaAtual[i];

                if (letra == sequenciaAlfabeto.Last() && !incremetado)
                {
                    novaSequencia.Add(sequenciaAlfabeto.First());
                    fimSequencia = true;
                    continue;
                }

                if (!incremetado)
                {
                    incremetado = true;
                    letra = ObterProximaLetra(letra);
                }

                fimSequencia = false;
                novaSequencia.Add(letra);
            }

            if (fimSequencia)
                novaSequencia.Insert(0, sequenciaAlfabeto.First());
            else
                novaSequencia.Reverse();

            return String.Join("", novaSequencia);
        }

        private void RemoverBlocosExistentes(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, IList<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> listaBlocoCarregamentoSegundoTrecho)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(_unitOfWork);
            List<int> listaCodigoCarregamento = listaBlocoCarregamentoSegundoTrecho.Select(o => o.CodigoCarregamentoSegundoTrecho).Distinct().ToList();

            listaCodigoCarregamento.Add(carregamento.Codigo);

            repositorioBlocoCarregamento.RemoverPorCodigosCarregamento(listaCodigoCarregamento);
        }

        #endregion

        #region Métodos Públicos

        public void Gerar(int codigoCarregamento, decimal pesoBlocos, decimal cubagem)
        {
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = ObterCarregamento(codigoCarregamento);

            if (carregamento.CarregamentoRedespacho)
                throw new ServicoException("Não é possível gerar blocos para carregamentos de redespacho");

            if (carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                throw new ServicoException("O carregamento está aguardando aprovação para gerar carga e não pode ser alterado");

            decimal pesoRequest = pesoBlocos;

            pesoBlocos = ObterPesoBlocos(carregamento, pesoBlocos);

            GerarBlocosCarregamento(carregamento, pesoBlocos, cubagem, pesoRequest);
        }

        #endregion
    }
}
