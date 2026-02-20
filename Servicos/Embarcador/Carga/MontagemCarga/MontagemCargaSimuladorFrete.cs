using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public class MontagemCargaSimuladorFrete
    {
        #region Variáveis Privadas

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;
        private Repositorio.UnitOfWork _unitOfWork;
        private string _stringConexao;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        public MontagemCargaSimuladorFrete(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoIntegracao = configuracaoIntegracao;
            _stringConexao = stringConexao;
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        public bool ExigeIsca(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga configuracaoTipoOperacaoCarga, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente, decimal valorTotal)
        {
            if (configuracaoTipoOperacaoCarga == null)
                return false;

            if (configuracaoTipoOperacaoCargaEstadoCliente != null)
                return configuracaoTipoOperacaoCargaEstadoCliente.ExigeInformarIscaNaCargaComValorMaiorQue < valorTotal;

            return configuracaoTipoOperacaoCarga.ExigeInformarIscaNaCargaComValorMaiorQue < valorTotal;
        }

        /// <summary>
        /// Método para validar/Gerar os blocos para simulação de frete... blocos são únicos por cliente...
        /// Agrupa todos os pedidos e seus totalizadores na tabela... T_MONTAGEM_CARREGAMENTO_BLOCO
        /// Esses blocos serão utilizados para a simulação de frete....
        /// </summary>
        /// <param name="parametros">Parâmentros da sessão de roteirização</param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> ValidaMontagemCarregamentoBlocos(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido repositorioMontagemCarregamentoBlocoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido(_unitOfWork);

            // Primeiro.. vamos limpar todos os blocos da sessão
            repositorioMontagemCarregamentoBloco.DeletarTodosPorSessaoRoteirizador(parametros.SessaoRoteirizador?.Codigo ?? 0);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>();

            dynamic grupos = parametros.Pedidos.GroupBy(x => new
            {
                Cliente = x.Destinatario
            })
                .Select(g => new
                {
                    Cliente = g.Key.Cliente,
                    PesoTotal = g.Sum(x => x.PesoTotal),
                    QuantidadePalletTotal = g.Sum(x => x.TotalPallets),
                    MetroCubicoTotal = g.Sum(x => x.CubagemTotal),
                    ValorTotal = g.Sum(x => x.ValorTotalCarga),
                    VolumesTotal = g.Sum(x => x.QtVolumes)
                }).ToList();

            foreach (var resumo in grupos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDestinatario = (from obj in parametros.Pedidos
                                                                                         where obj.Destinatario.Codigo == resumo.Cliente.Codigo
                                                                                         select obj).ToList();

                //Agora vamos gerar o Bloco do Cliente/Destinatário
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco montagemCarregamentoBloco = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco()
                {
                    Cliente = resumo.Cliente,
                    MetroCubicoTotal = resumo.MetroCubicoTotal,
                    PesoTotal = resumo.PesoTotal,
                    QuantidadePalletTotal = resumo.QuantidadePalletTotal,
                    VolumesTotal = resumo.VolumesTotal,
                    ValorTotal = resumo.ValorTotal,
                    SessaoRoteirizador = parametros.SessaoRoteirizador,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Pendente,
                    ValorTotalFretes = 0,
                    LeadTimeFretes = 0
                };

                repositorioMontagemCarregamentoBloco.Inserir(montagemCarregamentoBloco);

                List<int> codigosPedidos = (from obj in pedidosDestinatario select obj.Codigo).ToList();
                repositorioMontagemCarregamentoBlocoPedido.InserirSQL(montagemCarregamentoBloco.Codigo, codigosPedidos);

                montagemCarregamentoBlocos.Add(montagemCarregamentoBloco);
            }

            return montagemCarregamentoBlocos;
        }

        /// <summary>
        /// Método para validar/Gerar os blocos para simulação de frete... blocos por carregamento...
        /// Agrupa todos os pedidos e seus totalizadores na tabela... T_MONTAGEM_CARREGAMENTO_BLOCO
        /// Esses blocos serão utilizados para a simulação de frete....
        /// </summary>
        /// <param name="parametros">Parâmentros da sessão de roteirização</param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> ValidaCarregamentoBlocos(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDoCarregamento, int codigoCentroCarregamento)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido repositorioMontagemCarregamentoBlocoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido(_unitOfWork);

            // Primeiro.. vamos limpar todos os blocos do carregamento...
            repositorioMontagemCarregamentoBloco.DeletarTodosPorCarregamento(carregamento.Codigo);

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

            List<Dominio.Entidades.Empresa> transportadores = (from obj in centroCarregamento.Transportadores
                                                               select obj.Transportador).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco>();

            foreach (Dominio.Entidades.Empresa empresa in transportadores)
            {
                //Agora vamos gerar o Bloco do Cliente/Destinatário
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco montagemCarregamentoBloco = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco()
                {
                    Cliente = (from obj in pedidosDoCarregamento select obj.Destinatario).FirstOrDefault(),
                    MetroCubicoTotal = pedidosDoCarregamento.Sum(x => x.CubagemTotal),
                    PesoTotal = pedidosDoCarregamento.Sum(x => x.PesoTotal),
                    QuantidadePalletTotal = pedidosDoCarregamento.Sum(x => x.TotalPallets),
                    VolumesTotal = pedidosDoCarregamento.Sum(x => x.QtVolumes),
                    ValorTotal = pedidosDoCarregamento.Sum(x => x.ValorTotalCarga),
                    SessaoRoteirizador = parametros.SessaoRoteirizador,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Pendente,
                    ValorTotalFretes = 0,
                    LeadTimeFretes = 0,
                    Carregamento = carregamento,
                    Transportador = empresa
                };

                repositorioMontagemCarregamentoBloco.Inserir(montagemCarregamentoBloco);

                List<int> codigosPedidos = (from obj in pedidosDoCarregamento select obj.Codigo).ToList();
                repositorioMontagemCarregamentoBlocoPedido.InserirSQL(montagemCarregamentoBloco.Codigo, codigosPedidos);

                lista.Add(montagemCarregamentoBloco);
            }
            return lista;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> GerarMontagemCarregamentoBlocoSimuladorFretePorBloco(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> blocos, ref string erro)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido repositorioMontagemCarregamentoBlocoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido(_unitOfWork);

            Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioCentroCarregamentoTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(_unitOfWork);
            Repositorio.RotaFreteEmpresaExclusiva repositorioRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(_unitOfWork);

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(_unitOfWork);
            Servicos.Embarcador.Hubs.MontagemCarga servicoNotificacaomontagemCarga = new Servicos.Embarcador.Hubs.MontagemCarga();

            // Lista com o resultado...
            //List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> montagemCarregamentoBlocoSimuladorFretePedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> centroCarregamentoTiposOperacoes = repositorioCentroCarregamentoTipoOperacao.BuscarPorCentro(parametros.CentrosCarregamento.FirstOrDefault().Codigo);
            if ((centroCarregamentoTiposOperacoes?.Count ?? 0) == 0)
            {
                erro = "Nenhum tipo de operação habilitado no Centro de Carregamento para Simulação de Frete.";
                return montagemCarregamentoBlocoSimuladorFretes;
            }

            List<double> cnpjCpfFechando = (from bloco in blocos
                                            select bloco.Cliente.CPF_CNPJ).Distinct().ToList();

            // Validando as restrições de veículos #38664
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, cnpjCpfFechando);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            if (modalidadePessoasFornecedores.Count > 0)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(_unitOfWork);
                List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);
            }

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> pedidosDosBlocos = repositorioMontagemCarregamentoBlocoPedido.BuscarPorBlocos((from obj in blocos select obj.Codigo).ToList());

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> clientesDescargas = repClienteDescarga.BuscarPorPessoas((from obj in blocos select obj.Cliente.CPF_CNPJ).Distinct().ToList());

            List<Dominio.Entidades.RotaFreteEmpresaExclusiva> rotasExclusivasRegioes = repositorioRotaFreteEmpresaExclusiva.BuscarPorRegiaoExclusivaRegiaoDestinos((from obj in blocos
                                                                                                                                                                    where obj.Cliente.Localidade.Regiao != null
                                                                                                                                                                    select obj.Cliente.Localidade.Regiao.Codigo).Distinct().ToList());

            int contador = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco bloco in blocos)
            {
                contador++;
                this.GerarLog($"[{contador} - {blocos.Count}] Iniciou Simulador Frete bloco {bloco.Codigo} - {bloco.Cliente.Descricao}");

                servicoNotificacaomontagemCarga.InformarQuantidadeProcessadosCarregamentoAutomatico(blocos.Count + 1, contador, parametros.SessaoRoteirizador.Codigo, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.GerandoSimulacoesFreteBloco, bloco.Cliente.Descricao));

                bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GerandoSimulacao;
                repositorioMontagemCarregamentoBloco.Atualizar(bloco);

                // Lista de pedidos do Bloco (Destinatário) //repositorioMontagemCarregamentoBlocoPedido.BuscarPorBloco(bloco.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> pedidosDoBloco = (from obj in pedidosDosBlocos
                                                                                                                          where obj.Bloco.Codigo == bloco.Codigo
                                                                                                                          select obj).ToList();
                pedidosDoBloco = pedidosDoBloco.OrderByDescending(x => x.Pedido.PesoTotal).ToList();

                //#38697 - Valor mínimo por carregamento.
                // #40487 - Solicitado remoção da regra....
                //if (bloco.Cliente.ValorMinimoCarga > 0)
                //{
                //    decimal valorTotalPedidosBloco = (from obj in pedidosDoBloco select obj.Pedido.GrossSales).Sum();
                //    if (valorTotalPedidosBloco < bloco.Cliente.ValorMinimoCarga)
                //    {
                //        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ValorMinimoCargaNaoAtendido;
                //        bloco.Observacao = $"Valor mínimo { (bloco.Cliente.ValorMinimoCarga ?? 0).ToString("n2") } de carga do cliente  {bloco.Cliente.CPF_CNPJ_Formatado} não atendido.";
                //        repositorioMontagemCarregamentoBloco.Atualizar(bloco);
                //        continue;
                //    }
                //}

                List<Dominio.Entidades.RotaFreteEmpresaExclusiva> rotasExclusivasRegiao = null;
                if (bloco.Cliente.Localidade.Regiao != null)    // repositorioRotaFreteEmpresaExclusiva.BuscarPorRegiaoExclusivaRegiaoDestino(bloco.Cliente.Localidade.Regiao.Codigo);
                    rotasExclusivasRegiao = (from obj in rotasExclusivasRegioes
                                             where obj.RotaFrete.RegiaoDestino.Codigo == bloco.Cliente.Localidade.Regiao.Codigo
                                             select obj).ToList();

                List<Dominio.Entidades.Empresa> transportadores = null;

                if (rotasExclusivasRegiao == null)
                {
                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                    bloco.Observacao = $"Nenhuma Rota de Frete encontrada para os pedidos do destinatário {bloco.Cliente.CPF_CNPJ_Formatado}.";
                    repositorioMontagemCarregamentoBloco.Atualizar(bloco);
                    continue;
                }
                else
                    transportadores = rotasExclusivasRegiao.Select(obj => obj.Empresa).Distinct().ToList();

                // Validando se existe transportadores habilitados para a Rota Frete do bloco de pedidos...
                if ((transportadores?.Count ?? 0) == 0)
                {
                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                    bloco.Observacao = $"Nenhum Transportador encontrado na Região do pedido {bloco.Cliente.Localidade.Regiao?.Descricao ?? "Não informada na localidade"}.";
                    repositorioMontagemCarregamentoBloco.Atualizar(bloco);
                    continue;
                }


                //#38665
                //Ao gerar um carregamento aplicar a seguinte regra no carregamento gerado:
                //1 - Se o tipo de operação do carregamento tiver tipo de carga padrão usar ele.
                //2 - Se o tipo de operação não possuir tipo de carga padrão, olhar para os destinatários dos pedidos,
                //    se todos tiverem o mesmo tipo de carga especificado aplicar o que está no cadastro do cliente,
                //    se possuírem tipos diferentes ou não possuírem tipos deixar sem
                //repClienteDescarga.BuscarPorPessoa(bloco.Cliente.Codigo);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = (from obj in clientesDescargas
                                                                                        where obj.Cliente.Codigo == bloco.Cliente.Codigo
                                                                                        select obj).FirstOrDefault();

                List<int> codigosTransportadores = (from obj in transportadores select obj.Codigo).Distinct().ToList();

                List<Dominio.Entidades.Veiculo> veiculosTransportadores = repositorioVeiculo.BuscarPorEmpresas(codigosTransportadores, "A");

                try
                {
                    // Vamos gerar uma simulação.. para cada tipo de operação....
                    foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao tipoOperacao in centroCarregamentoTiposOperacoes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente = ObterTipoConfiguracaoCargaEstadoCliente(tipoOperacao, bloco.Cliente);
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = tipoOperacao.TipoOperacao.TipoDeCargaPadraoOperacao ?? clienteDescarga?.TipoDeCarga;
                        bool tipoCargaPaletizado = tipoCarga?.Paletizado ?? false;
                        int contadorTransportador = 0;

                        foreach (Dominio.Entidades.Empresa transportador in transportadores)
                        {
                            contadorTransportador++;
                            this.GerarLog($"[{contadorTransportador} - {transportadores.Count}] Simulando Frete {tipoOperacao.TipoOperacao.Descricao} transportador {transportador.Codigo} - {transportador.Descricao} ");

                            //Validando se a empresa possui o maior modelo veicular...
                            //List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorEmpresa(transportador.Codigo, "A");
                            List<Dominio.Entidades.Veiculo> veiculos = (from obj in veiculosTransportadores
                                                                        where obj.Empresa.Codigo == transportador.Codigo
                                                                        select obj).ToList();

                            veiculos = (from v in veiculos where v.ModeloVeicularCarga != null select v).ToList();

                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosTransportador = (from o in veiculos
                                                                                                                  select o.ModeloVeicularCarga).OrderBy(x => x.CapacidadePesoTransporte).ToList();

                            if ((modelosTransportador?.Count ?? 0) == 0)
                            {
                                if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial &&
                                    bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete)
                                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                                bloco.Observacao += $"{transportador.Descricao} - Nenhum modelo veicular cadastrado/ativo para o transportador\n";
                                continue;
                            }

                            // Fitrar veículos permitidos..
                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesRestritos = (from restr in modalidadeFornecedorPessoasRestricaoModeloVeicular
                                                                                                                        where restr.ModalidadeFornecedorPessoa.ModalidadePessoas.Cliente.Codigo == bloco.Cliente.CPF_CNPJ
                                                                                                                        select restr.ModeloVeicular).ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                            if (modelosVeicularesRestritos.Count > 0)
                                modelosTransportador = modelosTransportador.Where(m => !modelosVeicularesRestritos.Any(r => r.Codigo == m.Codigo)).ToList();

                            if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                                modelosTransportador = (from obj in modelosTransportador where obj.Cubagem > 0 select obj).ToList();
                            else if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                                modelosTransportador = (from obj in modelosTransportador where obj.NumeroPaletes > 0 select obj).ToList();
                            else
                                modelosTransportador = (from obj in modelosTransportador where obj.CapacidadePesoTransporte > 0 select obj).ToList();

                            if ((modelosTransportador?.Count ?? 0) == 0)
                            {
                                if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial &&
                                    bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete)
                                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                                bloco.Observacao += $"{transportador.Descricao} - Nenhum modelo veicular do transportador permitido ou capacidade de {(tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico ? "Cubagem" : (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet ? "Pallet" : "Peso"))} não cadastrada.\n";
                                continue;
                            }

                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga menorModeloVeicularCarga = modelosTransportador[0];
                            decimal cubagemTotal = (from obj in pedidosDoBloco select obj.Pedido.CubagemTotal).Sum();
                            decimal palletTotal = (from obj in pedidosDoBloco select obj.Pedido.TotalPallets).Sum();
                            menorModeloVeicularCarga = ObterMenorModeloVeicularCarregamento(tipoOperacao, tipoCarga, menorModeloVeicularCarga, modelosTransportador, bloco.PesoTotal, cubagemTotal, palletTotal);

                            string msgErroFrete = string.Empty;
                            // Se for total do cliente, gera apenas 1x com o peso total....
                            // Se for capacidade do veiculo, gera de acordo com a quantidade de veiculos necessárias para efetuar a entrega (Do maior para o menor veículo);

                            Dominio.Entidades.RotaFrete rotaFrete = (from obj in rotasExclusivasRegiao where obj.Empresa.Codigo == transportador.Codigo && (obj.RotaFrete.TipoOperacao == null || obj.RotaFrete.TipoOperacao.Codigo == tipoOperacao.TipoOperacao.Codigo) select obj.RotaFrete).FirstOrDefault();

                            if (rotaFrete == null)
                            {
                                if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial &&
                                  bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete)
                                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                                bloco.Observacao += $"{transportador.Descricao} - Transportador não possui rota para o tipo de operação {tipoOperacao.TipoOperacao.Descricao}.\n";
                                continue;
                            }

                            if (tipoOperacao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo.TotalCliente ||
                                (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Peso && bloco.PesoTotal <= menorModeloVeicularCarga.CapacidadePesoTransporte) ||
                                (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico && cubagemTotal <= (menorModeloVeicularCarga.Cubagem - (tipoCargaPaletizado ? menorModeloVeicularCarga.ObterOcupacaoCubicaPaletes() : 0m))) ||
                                (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet && palletTotal <= menorModeloVeicularCarga.QuantidadePaletes))
                            {
                                //GerarMontagemCarregamentoBlocoSimuladorFrete(transportador, rotaFrete, tipoOperacao, menorModeloVeicularCarga, 1, bloco, pedidosDoBloco, clienteDescarga?.TipoDeCarga, ref montagemCarregamentoBlocoSimuladorFretes, ref montagemCarregamentoBlocoSimuladorFretePedidos, ref msgErroFrete);
                                GerarMontagemCarregamentoBlocoSimuladorFrete(transportador, rotaFrete, tipoOperacao, configuracaoTipoOperacaoCargaEstadoCliente, menorModeloVeicularCarga, 1, bloco, (from obj in pedidosDoBloco select obj.Pedido).ToList(), clienteDescarga?.TipoDeCarga, ref montagemCarregamentoBlocoSimuladorFretes, 0, null, ref msgErroFrete);
                            }
                            else
                            {
                                // Aqui.. precisamos achar quantos veiculos são necessários para efetuar as entregas....
                                decimal pesoTotalPedidosBloco = (from o in pedidosDoBloco select o.Pedido.PesoTotal).Sum();
                                decimal cubagemTotalPedidosBloco = (from o in pedidosDoBloco select o.Pedido.CubagemTotal).Sum();
                                decimal palletTotalPedidosBloco = (from o in pedidosDoBloco select o.Pedido.TotalPallets).Sum();

                                decimal saldoPesoTotalPedidosBloco = pesoTotalPedidosBloco;
                                decimal saldoCubagemTotalPedidosBloco = cubagemTotalPedidosBloco;
                                decimal saldoPalletTotalPedidosBloco = palletTotalPedidosBloco;

                                //vamos verificar se existe pedidos .. que o transportador não consegue carregar.. ele não irá particiar..
                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga maiorFrota = ObterModeloVeicularPeso(tipoOperacao, tipoCarga, modelosTransportador, saldoPesoTotalPedidosBloco, saldoCubagemTotalPedidosBloco, saldoPalletTotalPedidosBloco);

                                decimal maxPeso = (from o in pedidosDoBloco select o.Pedido.PesoTotal).Max();
                                decimal maxCubagem = (from o in pedidosDoBloco select o.Pedido.CubagemTotal).Max();
                                decimal maxPallet = (from o in pedidosDoBloco select o.Pedido.TotalPallets).Max();

                                // Ordenando os pedidos em ordem decresccente...
                                if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                                {
                                    pedidosDoBloco = pedidosDoBloco.OrderByDescending(x => x.Pedido.CubagemTotal).ToList();
                                    if (maxCubagem > (maiorFrota.Cubagem - (tipoCargaPaletizado ? maiorFrota.ObterOcupacaoCubicaPaletes() : 0m)))
                                        continue;
                                }
                                else if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                                {
                                    pedidosDoBloco = pedidosDoBloco.OrderByDescending(x => x.Pedido.TotalPallets).ToList();
                                    if (maxPallet > maiorFrota.NumeroPaletes)
                                        continue;
                                }
                                else
                                {
                                    pedidosDoBloco = pedidosDoBloco.OrderByDescending(x => x.Pedido.PesoTotal).ToList();
                                    if (maxPeso > maiorFrota.CapacidadePesoTransporte)
                                        continue;
                                }

                                List<int> pedidosBlocoCarregados = new List<int>();

                                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteFrotaPedido> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteFrotaPedido>();

                                // Aki vamos encontrar a quantidade necessária de veículos para carregar todas as entregas...
                                while ((tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Peso && saldoPesoTotalPedidosBloco > 0) ||
                                        (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico && saldoCubagemTotalPedidosBloco > 0) ||
                                        (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet && saldoPalletTotalPedidosBloco > 0))
                                {
                                    // Primeiro.. vamos obter o maior modelo de acordo com o saldo...
                                    maiorFrota = ObterModeloVeicularPeso(tipoOperacao, tipoCarga, modelosTransportador, saldoPesoTotalPedidosBloco, saldoCubagemTotalPedidosBloco, saldoPalletTotalPedidosBloco);

                                    //Agora.. vamos obter os pedidos que cabem nesse modelo...
                                    // detalhes.. não quebramos pedido.. então se um pedido for maior que a capacidade de transporte.. vai carregar total ...
                                    decimal pesoCarregar = 0;
                                    decimal cubagemCarregar = 0;
                                    decimal palletCarregar = 0;
                                    List<int> pedidosBlocoCarregar = new List<int>();
                                    for (int i = 0; i < pedidosDoBloco.Count; i++)
                                    {
                                        if ((pesoCarregar + pedidosDoBloco[i].Pedido.PesoTotal <= maiorFrota.CapacidadePesoTransporte &&
                                            cubagemCarregar + pedidosDoBloco[i].Pedido.CubagemTotal <= (maiorFrota.Cubagem - (tipoCargaPaletizado ? maiorFrota.ObterOcupacaoCubicaPaletes() : 0m)) &&
                                            palletCarregar + pedidosDoBloco[i].Pedido.TotalPallets <= maiorFrota.NumeroPaletes &&
                                            !pedidosBlocoCarregados.Contains(pedidosDoBloco[i].Codigo))) //||
                                                                                                         //(pedidosDoBloco[i].Pedido.PesoTotal > maiorFrota.CapacidadePesoTransporte && pesoCarregar == 0)||
                                                                                                         //(pedidosDoBloco[i].Pedido.CubagemTotal > maiorFrota.Cubagem && cubagemCarregar == 0 )||
                                                                                                         //(pedidosDoBloco[i].Pedido.TotalPallets > maiorFrota.NumeroPaletes && palletCarregar == 0))
                                        {
                                            pesoCarregar += pedidosDoBloco[i].Pedido.PesoTotal;
                                            cubagemCarregar += pedidosDoBloco[i].Pedido.CubagemTotal;
                                            palletCarregar += pedidosDoBloco[i].Pedido.TotalPallets;
                                            pedidosBlocoCarregar.Add(pedidosDoBloco[i].Codigo);
                                        }
                                    }

                                    //Agoraa.. vamos localizar o menor modelo veicular capaz de carregar...
                                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga frota = ObterModeloVeicularPeso(tipoOperacao, tipoCarga, modelosTransportador, pesoCarregar, cubagemCarregar, palletCarregar);
                                    if (frota == null)
                                        break;

                                    saldoPesoTotalPedidosBloco -= pesoCarregar;         // frota.CapacidadePesoTransporte;
                                    saldoCubagemTotalPedidosBloco -= cubagemCarregar;   // frota.Cubagem;
                                    saldoPalletTotalPedidosBloco -= palletCarregar;     // frota.QuantidadePaletes;
                                    pedidosBlocoCarregados.AddRange(pedidosBlocoCarregar);

                                    if (pedidosBlocoCarregar.Count > 0)
                                        resultado.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteFrotaPedido()
                                        {
                                            ModeloVeicular = frota,
                                            BlocoPedidos = (from obj in pedidosDoBloco where pedidosBlocoCarregar.Contains(obj.Codigo) select obj).ToList()
                                        });
                                    else
                                        break;

                                }

                                // Agora que temos a quantidade de modelos disponíveis.. vamos gerar os MontagemCarregamentoBlocoSimuladorFrete
                                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteFrotaPedido frota in resultado)
                                {
                                    string msgFreteTemp = string.Empty;
                                    // Precisamos achar os pedidos/peso para completar a capacidade do veículo..
                                    GerarMontagemCarregamentoBlocoSimuladorFrete(transportador, rotaFrete, tipoOperacao, configuracaoTipoOperacaoCargaEstadoCliente, frota.ModeloVeicular, 1, bloco, (from obj in frota.BlocoPedidos select obj.Pedido).ToList(), clienteDescarga?.TipoDeCarga, ref montagemCarregamentoBlocoSimuladorFretes, 0, null, ref msgFreteTemp);

                                    // Se der erro no simular frete... vamos remover o transportador da disputa..
                                    if (!string.IsNullOrEmpty(msgFreteTemp))
                                    {
                                        this.RemoverTransportadorDisputa(transportador, bloco, tipoOperacao, ref montagemCarregamentoBlocoSimuladorFretes);
                                        msgErroFrete = msgFreteTemp;
                                        break;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(msgErroFrete))
                            {
                                if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete;
                                bloco.Observacao += transportador.Descricao + " - " + msgErroFrete;
                            }
                            else
                            {
                                //Quando gerar pelo menos uma simulação.. Gerado Parcial.
                                bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial;
                            }
                        }
                    }

                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete &&
                        bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Sucesso;
                }
                catch (Dominio.Excecoes.Embarcador.ServicoException excecao)
                {
                    erro = excecao.Message;
                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Erro;
                }
                catch (Exception ex)
                {
                    erro = "Ocorreu um erro ao gerar a simulação de frete.";
                    Servicos.Log.TratarErro(ex, "MontagemCarga");

                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Erro;
                }
                if (bloco?.Observacao?.Length > 5000)
                    bloco.Observacao = bloco.Observacao.Substring(0, 5000);
                repositorioMontagemCarregamentoBloco.Atualizar(bloco);
            }

            return montagemCarregamentoBlocoSimuladorFretes;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> GerarSimuladorFretePorCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> blocos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDoCarregamento, int distanciaKm, ref string erro)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(_unitOfWork);

            Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioCentroCarregamentoTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(_unitOfWork);

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> centroCarregamentoTiposOperacoes = repositorioCentroCarregamentoTipoOperacao.BuscarPorCentro(parametros.CentrosCarregamento.FirstOrDefault().Codigo);
            if ((centroCarregamentoTiposOperacoes?.Count ?? 0) == 0)
            {
                erro = "Nenhum tipo de operação habilitado no Centro de Carregamento para Simulação de Frete.";
                return montagemCarregamentoBlocoSimuladorFretes;
            }

            List<double> cnpjCpfFechando = (from bloc in pedidosDoCarregamento
                                            select bloc.Destinatario.CPF_CNPJ).Distinct().ToList();

            //// Validando as restrições de veículos #38664
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, cnpjCpfFechando);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            if (modalidadePessoasFornecedores.Count > 0)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(_unitOfWork);
                List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);
            }

            List<int> codigosTransportadores = (from obj in blocos select obj.Transportador.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Veiculo> veiculosTransportadores = repositorioVeiculo.BuscarPorEmpresas(codigosTransportadores, "A");

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco bloco in blocos)
            {
                try
                {
                    // Vamos gerar uma simulação.. para cada tipo de operação....
                    foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao tipoOperacao in centroCarregamentoTiposOperacoes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = tipoOperacao.TipoOperacao.TipoDeCargaPadraoOperacao ?? carregamento?.TipoDeCarga;
                        bool tipoCargaPaletizado = tipoCarga?.Paletizado ?? false;
                        int contadorTransportador = 0;

                        contadorTransportador++;
                        this.GerarLog($"[{contadorTransportador}] Simulando Frete {tipoOperacao.TipoOperacao.Descricao} transportador {bloco.Transportador.Codigo} - {bloco.Transportador.Descricao} ");

                        //Validando se a empresa possui o maior modelo veicular...
                        List<Dominio.Entidades.Veiculo> veiculos = (from obj in veiculosTransportadores
                                                                    where obj.Empresa.Codigo == bloco.Transportador.Codigo
                                                                    select obj).ToList();

                        veiculos = (from v in veiculos where v.ModeloVeicularCarga != null select v).ToList();

                        List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosTransportador = (from o in veiculos
                                                                                                              select o.ModeloVeicularCarga).OrderBy(x => x.CapacidadePesoTransporte).ToList();

                        if ((modelosTransportador?.Count ?? 0) == 0 && carregamento.ModeloVeicularCarga == null)
                        {
                            if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial &&
                                bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete)
                                bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                            if (!(bloco?.Observacao ?? string.Empty).Contains($"{bloco.Transportador.Descricao} - Nenhum modelo veicular cadastrado/ativo para o transportador"))
                                bloco.Observacao += $"{bloco.Transportador.Descricao} - Nenhum modelo veicular cadastrado/ativo para o transportador\n";
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga menorModeloVeicularCarga = carregamento.ModeloVeicularCarga;

                        if (menorModeloVeicularCarga == null)
                        {
                            // Fitrar veículos permitidos..
                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesRestritos = (from restr in modalidadeFornecedorPessoasRestricaoModeloVeicular
                                                                                                                        where pedidosDoCarregamento.Any(x => x.Destinatario.CPF_CNPJ == restr.ModalidadeFornecedorPessoa.ModalidadePessoas.Cliente.Codigo)
                                                                                                                        select restr.ModeloVeicular).ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                            if (modelosVeicularesRestritos.Count > 0)
                                modelosTransportador = modelosTransportador.Where(m => !modelosVeicularesRestritos.Any(r => r.Codigo == m.Codigo)).ToList();

                            if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                                modelosTransportador = (from obj in modelosTransportador where obj.Cubagem > 0 select obj).ToList();
                            else if (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                                modelosTransportador = (from obj in modelosTransportador where obj.NumeroPaletes > 0 select obj).ToList();
                            else
                                modelosTransportador = (from obj in modelosTransportador where obj.CapacidadePesoTransporte > 0 select obj).ToList();

                            if ((modelosTransportador?.Count ?? 0) == 0)
                            {
                                if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial &&
                                    bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete)
                                    bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto;
                                bloco.Observacao = $"{bloco.Transportador.Descricao} - Nenhum modelo veicular do transportador permitido ou capacidade de {(tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico ? "Cubagem" : (tipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet ? "Pallet" : "Peso"))} não cadastrada.\n";
                                continue;
                            }

                            menorModeloVeicularCarga = modelosTransportador[0];
                            decimal cubagemTotal = (from obj in pedidosDoCarregamento select obj.CubagemTotal).Sum();
                            decimal palletTotal = (from obj in pedidosDoCarregamento select obj.TotalPallets).Sum();
                            menorModeloVeicularCarga = ObterMenorModeloVeicularCarregamento(tipoOperacao, tipoCarga, menorModeloVeicularCarga, modelosTransportador, carregamento.PesoCarregamento, cubagemTotal, palletTotal);
                        }

                        string msgErroFrete = string.Empty;

                        GerarMontagemCarregamentoBlocoSimuladorFrete(bloco.Transportador,
                                                                     null,
                                                                     tipoOperacao,
                                                                     null,
                                                                     menorModeloVeicularCarga,
                                                                     1,
                                                                     bloco,
                                                                     pedidosDoCarregamento,
                                                                     carregamento?.TipoDeCarga,
                                                                     ref montagemCarregamentoBlocoSimuladorFretes,
                                                                     distanciaKm,
                                                                     carregamento,
                                                                     ref msgErroFrete);


                        if (!string.IsNullOrEmpty(msgErroFrete))
                        {
                            if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                                bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete;
                            bloco.Observacao += bloco.Transportador.Descricao + " - " + (tipoOperacao.TipoOperacao?.Descricao ?? string.Empty) + " - " + msgErroFrete;
                        }
                        else
                        {
                            //Quando gerar pelo menos uma simulação.. Gerado Parcial.
                            bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial;
                        }

                    }

                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.ErroCalcularFrete &&
                        bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.CadastroIncompleto)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Sucesso;
                }
                catch (Dominio.Excecoes.Embarcador.ServicoException excecao)
                {
                    erro = excecao.Message;
                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Erro;
                }
                catch (Exception ex)
                {
                    erro = "Ocorreu um erro ao gerar a simulação de frete.";
                    Servicos.Log.TratarErro(ex, "MontagemCarga");

                    if (bloco.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.GeradoParcial)
                        bloco.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco.Erro;
                }
                if (bloco?.Observacao?.Length > 5000)
                    bloco.Observacao = bloco.Observacao.Substring(0, 5000);
                repositorioMontagemCarregamentoBloco.Atualizar(bloco);
            }
            return montagemCarregamentoBlocoSimuladorFretes;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> GerarMontagemCargaGrupoPedido(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> vencedoresBlocoSimuladorFretes)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> totalJaCarregado = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete vencedorBloco in vencedoresBlocoSimuladorFretes)
            {
                // Todos os pedidos do bloco Simulado e vencedor...
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> pedidosBlocoSimuladoFrete = repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorBlocoSimuladorFrete(vencedorBloco.Codigo);

                for (int i = 0; i < vencedorBloco.Quantidade; i++)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> pedidosSimuladosParaOCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();
                    if (vencedorBloco.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo.TotalCliente)
                        pedidosSimuladosParaOCarregamento = pedidosBlocoSimuladoFrete;
                    else
                        pedidosSimuladosParaOCarregamento = ObterPedidosBlocoPesos(pedidosBlocoSimuladoFrete, vencedorBloco.ModeloVeicularCarga, totalJaCarregado, (i + 1 == vencedorBloco.Quantidade));

                    totalJaCarregado.AddRange(pedidosBlocoSimuladoFrete);

                    resultado.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido()
                    {
                        CodigoFilial = (from p in pedidosSimuladosParaOCarregamento select p.Pedido?.Filial?.Codigo ?? 0).FirstOrDefault(),
                        DataCarregamento = (from p in pedidosSimuladosParaOCarregamento select p.Pedido.DataCarregamentoPedido)?.Min() ?? DateTime.Now,
                        ModeloVeicular = vencedorBloco.ModeloVeicularCarga,
                        Pedidos = (from o in pedidosSimuladosParaOCarregamento select o.Pedido).ToList(),
                        PedidosPesos = (from o in pedidosSimuladosParaOCarregamento select o.Pedido).ToDictionary(t => t.Codigo, t => t.PesoTotal),
                        PontosDeApoio = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto>(),
                        Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>(),
                        QtdeEntregas = 1,
                        Transportador = vencedorBloco.Transportador,
                        TipoOperacao = vencedorBloco.TipoOperacao,
                        ValorFreteVencedor = vencedorBloco.ValorTotal,
                        ExigeIsca = vencedorBloco.ExigeIsca,
                        TipoDeCarga = vencedorBloco.TipoDeCarga,
                        MontagemCarregamentoBloco = vencedorBloco.Bloco
                    });
                }
            }

            return resultado;
        }

        /// <summary>
        /// Método que gera o Ranking de acordo com o valor do frete por Bloco, Tipo de Operação e Transportador...
        /// </summary>
        /// <param name="montagemCarregamentoBlocos">Blocos gerados</param>
        /// <param name="montagemCarregamentoBlocoSimuladorFretes">Simulações de fretes geradas para cada bloco...</param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> GerarVencedoresBlocoSimuladorFrete(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador simuladorFreteCriterioSelecaoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> vencedores = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete>();

            // Agora.. vamos escolher os "melhores" de acordo com cada bloco...
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco bloco in montagemCarregamentoBlocos)
            {
                decimal valorTotalVencedor = 0;
                int leadTimeVencedor = 0;
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> simulacoesFretesBloco = (from o in montagemCarregamentoBlocoSimuladorFretes
                                                                                                                                             where o.Bloco.Codigo == bloco.Codigo
                                                                                                                                             select o).ToList();

                if (simuladorFreteCriterioSelecaoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum)
                    simulacoesFretesBloco = montagemCarregamentoBlocoSimuladorFretes;

                // Ordenando de acordo com o valor do frete...
                dynamic resumo = simulacoesFretesBloco.GroupBy(x => new
                {
                    Bloco = (simuladorFreteCriterioSelecaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum ? x.Bloco : null),
                    x.Tipo,
                    x.TipoOperacao,
                    x.Transportador
                }).Select(g => new
                {
                    Bloco = g.Key.Bloco,
                    Tipo = g.Key.Tipo,
                    TipoOperacao = g.Key.TipoOperacao,
                    Transportador = g.Key.Transportador,
                    LeadTime = g.Sum(x => x.LeadTimeTotalSimulacao),
                    ValorTotal = g.Sum(x => x.ValorTotalSimulacao)
                }).OrderBy(o => o.ValorTotal).ToList();

                if (simuladorFreteCriterioSelecaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.MenorPrazoEntrega)
                    resumo = simulacoesFretesBloco.GroupBy(x => new
                    {
                        Bloco = (simuladorFreteCriterioSelecaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum ? x.Bloco : null),
                        x.Tipo,
                        x.TipoOperacao,
                        x.Transportador
                    }).Select(g => new
                    {
                        Bloco = g.Key.Bloco,
                        Tipo = g.Key.Tipo,
                        TipoOperacao = g.Key.TipoOperacao,
                        Transportador = g.Key.Transportador,
                        LeadTime = g.Sum(x => x.LeadTimeTotalSimulacao),
                        ValorTotal = g.Sum(x => x.ValorTotalSimulacao)
                    }).OrderBy(o => o.LeadTime).ToList();

                int ranking = 1;
                foreach (dynamic res in resumo)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> itensRanking = (from o in simulacoesFretesBloco
                                                                                                                                        where o.Bloco.Codigo == (res.Bloco?.Codigo ?? 0) &&
                                                                                                                                                o.Tipo == res.Tipo &&
                                                                                                                                                o.TipoOperacao.Codigo == res.TipoOperacao.Codigo &&
                                                                                                                                                o.Transportador.Codigo == res.Transportador.Codigo
                                                                                                                                        select o).ToList();
                    if (simuladorFreteCriterioSelecaoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum)
                        itensRanking = (from o in simulacoesFretesBloco
                                        where o.Tipo == res.Tipo &&
                                                o.TipoOperacao.Codigo == res.TipoOperacao.Codigo &&
                                                o.Transportador.Codigo == res.Transportador.Codigo
                                        select o).ToList();                    

                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete item in itensRanking)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete()
                        {
                            Bloco = item.Bloco,
                            ModeloVeicularCarga = item.ModeloVeicularCarga,
                            Grupo = 1,
                            Quantidade = item.Quantidade,
                            TipoOperacao = item.TipoOperacao,
                            Tipo = item.Tipo,
                            Transportador = item.Transportador,
                            ValorTotal = item.ValorTotal,
                            LeadTime = item.LeadTime,
                            ExigeIsca = item.ExigeIsca,
                            TipoDeCarga = item.TipoDeCarga,
                            Vencedor = (ranking == 1),
                            Ranking = ranking
                        };

                        repositorioMontagemCarregamentoBlocoSimuladorFrete.Inserir(montagemCarregamentoBlocoSimuladorFrete);

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> itensInserir = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

                        //Agora devemos salvar os itens/Pedidos...
                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in item.Pedidos)
                        {
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido montagemCarregamentoBlocoSimuladorFretePedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido()
                            {
                                Pedido = pedido,
                                SimuladorFrete = montagemCarregamentoBlocoSimuladorFrete,
                                MetroCubico = pedido.CubagemTotal,
                                Peso = pedido.PesoTotal,
                                QuantidadePallet = pedido.TotalPallets,
                                Volumes = pedido.QtVolumes
                            };
                            itensInserir.Add(montagemCarregamentoBlocoSimuladorFretePedido);
                        }

                        repositorioMontagemCarregamentoBlocoSimuladorFretePedido.InserirSQL(itensInserir);

                        if (montagemCarregamentoBlocoSimuladorFrete.Ranking == 1)
                            vencedores.Add(montagemCarregamentoBlocoSimuladorFrete);

                        if (ranking == 1)
                        {
                            valorTotalVencedor += res.ValorTotal;
                            leadTimeVencedor += res.LeadTime;
                        }
                    }
                    ranking++;
                }

                bloco.ValorTotalFretes = valorTotalVencedor;
                bloco.LeadTimeFretes = leadTimeVencedor;
                repositorioMontagemCarregamentoBloco.Atualizar(bloco);

                //Se for por carregamento.. retorna apenas 1 vencedor.. e já gerou para todos..
                if (simuladorFreteCriterioSelecaoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum)
                    break;
            }

            return vencedores;
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterMenorModeloVeicularCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloAtual, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis, decimal pesoTotal, decimal cubagem, decimal pallet)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> todosModelosAtendem = null;

            if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
            {
                bool tipoCargaPaletizado = tipoCarga?.Paletizado ?? false;

                todosModelosAtendem = (from modelo in modelosDisponiveis
                                       where (modelo.Cubagem - (tipoCargaPaletizado ? modelo.ObterOcupacaoCubicaPaletes() : 0m)) >= cubagem
                                       select modelo).OrderBy(x => x.Cubagem).ToList();

            }
            else if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
            {
                todosModelosAtendem = (from modelo in modelosDisponiveis
                                       where modelo.QuantidadePaletes >= pallet
                                       select modelo).OrderBy(x => x.QuantidadePaletes).ToList();
            }
            else
            {
                todosModelosAtendem = (from modelo in modelosDisponiveis
                                       where modelo.CapacidadePesoTransporte >= pesoTotal
                                       select modelo).OrderBy(x => x.CapacidadePesoTransporte).ToList();

            }

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga outroModeloMelhorOcupacao = null;
            if (todosModelosAtendem?.Count > 0)
                outroModeloMelhorOcupacao = todosModelosAtendem[0];

            if (outroModeloMelhorOcupacao != null)
                modeloAtual = outroModeloMelhorOcupacao;

            return modeloAtual;
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularPeso(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis, decimal pesoTotal, decimal cubagem, decimal pallet)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosOrdenados = (from modelo in modelosDisponiveis select modelo).OrderByDescending(x => x.CapacidadePesoTransporte).ToList();

            if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                modelosOrdenados = (from modelo in modelosDisponiveis select modelo).OrderByDescending(x => x.Cubagem).ToList();
            else if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                modelosOrdenados = (from modelo in modelosDisponiveis select modelo).OrderByDescending(x => x.QuantidadePaletes).ToList();

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga maiorModelo = modelosOrdenados[0];

            if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
            {
                bool tipoCargaPaletizado = tipoCarga?.Paletizado ?? false;

                if (cubagem >= (maiorModelo.Cubagem - (tipoCargaPaletizado ? maiorModelo.ObterOcupacaoCubicaPaletes() : 0m)))
                    return maiorModelo;
                else
                    return ObterMenorModeloVeicularCarregamento(centroCarregamentoTipoOperacao, tipoCarga, maiorModelo, modelosDisponiveis, pesoTotal, cubagem, pallet);
            }
            else if (centroCarregamentoTipoOperacao.CentroCarregamento.TipoOcupacaoMontagemCarregamentoVRP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
            {
                if (pallet >= maiorModelo.QuantidadePaletes)
                    return maiorModelo;
                else
                    return ObterMenorModeloVeicularCarregamento(centroCarregamentoTipoOperacao, tipoCarga, maiorModelo, modelosDisponiveis, pesoTotal, cubagem, pallet);
            }
            else
            {
                if (pesoTotal >= maiorModelo.CapacidadePesoTransporte)
                    return maiorModelo;
                else
                    return ObterMenorModeloVeicularCarregamento(centroCarregamentoTipoOperacao, tipoCarga, maiorModelo, modelosDisponiveis, pesoTotal, cubagem, pallet);
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete ObterSimulacaoFrete(Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento, Dominio.Entidades.RotaFrete rotaFrete, out string msgFrete)
        {
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete();

            int leadTime = 0;
            decimal valorFrete = Servicos.Embarcador.Carga.Frete.CalcularFretePorCarregamento(cotacaoFreteCarregamento, rotaFrete, out msgFrete, _unitOfWork, _stringConexao, _tipoServicoMultisoftware, _configuracaoEmbarcador, ref leadTime, out Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo);
            decimal pesoTotal = cotacaoFreteCarregamento.PesoBruto;
            decimal valorTotalCarga = 0;
            if (pesoTotal == 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(cotacaoFreteCarregamento.Pedidos);
                pesoTotal = pedidos.Sum(p => p.PesoTotal);
                valorTotalCarga = (pedidos.Sum(obj => obj.Produtos.Sum(p => p.ValorProduto * p.Quantidade)));
            }
            decimal percentualSobValorMercadoria = (valorTotalCarga > 0) ? (valorFrete * 100) / valorTotalCarga : 0;

            simulacaoFrete.PesoFrete = pesoTotal;
            simulacaoFrete.ValorMercadoria = valorTotalCarga;
            simulacaoFrete.ValorFrete = valorFrete;
            simulacaoFrete.LeadTime = leadTime;
            simulacaoFrete.Distancia = cotacaoFreteCarregamento.Distancia;
            simulacaoFrete.ValorPorPeso = valorFrete / (pesoTotal == 0 ? 1 : pesoTotal);
            simulacaoFrete.PercentualSobValorMercadoria = percentualSobValorMercadoria;
            simulacaoFrete.SucessoSimulacao = string.IsNullOrWhiteSpace(msgFrete);

            return simulacaoFrete;
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado ObterTipoConfiguracaoCargaEstadoCliente(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao tipoOperacao, Dominio.Entidades.Cliente cliente)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado repositorioConfiguracaoTipoOperacaoCargaEstado = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado(_unitOfWork);

            int codigoConfiguraaoTipoOperacao = tipoOperacao?.TipoOperacao?.ConfiguracaoCarga?.Codigo ?? 0;
            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado> configuracoesTipoOperacaoCargaEstado = new List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado>();

            if (codigoConfiguraaoTipoOperacao > 0)
                configuracoesTipoOperacaoCargaEstado = repositorioConfiguracaoTipoOperacaoCargaEstado.BuscarPorConfiguracao(codigoConfiguraaoTipoOperacao);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente = null;
            if (!string.IsNullOrEmpty(cliente?.Localidade?.Estado?.Sigla ?? ""))
                configuracaoTipoOperacaoCargaEstadoCliente = (from obj in configuracoesTipoOperacaoCargaEstado
                                                              where obj.Estado.Sigla == cliente.Localidade.Estado.Sigla
                                                              select obj).FirstOrDefault();

            return configuracaoTipoOperacaoCargaEstadoCliente;
        }

        private void GerarLog(string msg)
        {
            Servicos.Log.TratarErro(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - " + msg, "MontagemCargaSimuladorFrete");
        }

        private void GerarMontagemCarregamentoBlocoSimuladorFrete(Dominio.Entidades.Empresa transportador, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga menorModeloVeicularCarga, int quantidadeModelo, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco bloco, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDoBloco, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCargaDestinatario, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes, int distanciaQuilometros, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, ref string erro)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento = null;
            if (carregamento != null)
            {
                cotacaoFreteCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento()
                {
                    CarregamentoRedespacho = carregamento.CarregamentoRedespacho,
                    Distancia = distanciaQuilometros,
                    ModeloVeicularCarga = carregamento?.ModeloVeicularCarga?.Codigo ?? menorModeloVeicularCarga?.Codigo ?? 0,
                    Pedidos = (from o in pedidosDoBloco select o.Codigo).ToList(),
                    TipoDeCarga = carregamento?.TipoDeCarga?.Codigo ?? centroCarregamentoTipoOperacao.TipoOperacao.TipoDeCargaPadraoOperacao?.Codigo ?? tipoDeCargaDestinatario?.Codigo ?? 0,
                    TipoOperacao = carregamento?.TipoOperacao?.Codigo ?? centroCarregamentoTipoOperacao.TipoOperacao.Codigo,
                    Transportador = transportador?.Codigo ?? 0,
                    Veiculo = carregamento?.Veiculo?.Codigo ?? 0,
                    Filial = (carregamento?.SessaoRoteirizador?.Filial?.Codigo ?? carregamento?.Filial?.Codigo) ?? 0
                };
            }
            else
            {
                cotacaoFreteCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento()
                {
                    PesoBruto = bloco.PesoTotal / quantidadeModelo,
                    Distancia = (int)(rotaFrete?.Quilometros ?? distanciaQuilometros),
                    ModeloVeicularCarga = menorModeloVeicularCarga?.Codigo ?? 0,
                    Pedidos = (from o in pedidosDoBloco select o.Codigo).ToList(),
                    TipoDeCarga = centroCarregamentoTipoOperacao.TipoOperacao.TipoDeCargaPadraoOperacao?.Codigo ?? tipoDeCargaDestinatario?.Codigo ?? 0,
                    TipoOperacao = centroCarregamentoTipoOperacao.TipoOperacao.Codigo,
                    Transportador = transportador.Codigo
                };
            }

            // Calculando o valor de frete simulado...
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = ObterSimulacaoFrete(cotacaoFreteCarregamento, rotaFrete, out erro);
            if (!simulacaoFrete.SucessoSimulacao)
                return;

            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete()
            {
                Bloco = bloco,
                ModeloVeicularCarga = menorModeloVeicularCarga,
                Grupo = 1,
                Quantidade = quantidadeModelo,
                TipoOperacao = centroCarregamentoTipoOperacao.TipoOperacao,
                Tipo = centroCarregamentoTipoOperacao.Tipo,
                Transportador = transportador,
                ValorTotal = simulacaoFrete.ValorFrete,
                LeadTime = simulacaoFrete.LeadTime,
                ExigeIsca = ExigeIsca(centroCarregamentoTipoOperacao.TipoOperacao?.ConfiguracaoCarga, configuracaoTipoOperacaoCargaEstadoCliente, (from obj in pedidosDoBloco select obj.ValorTotalNotasFiscais).Sum()), // bloco.ValorTotal),
                TipoDeCarga = (centroCarregamentoTipoOperacao.TipoOperacao.TipoDeCargaPadraoOperacao ?? tipoDeCargaDestinatario)
            };

            // Relacionar todos os pedidos ao bloco simulado de frete..
            // Se for Total Do cliente,, o peso, pallet, m3 não estaram informados então é o total do pedido.. 
            // Quando parcial, vai vir calculado a qtde correspondente a cada pedido..
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBloco in pedidosDoBloco)
                montagemCarregamentoBlocoSimuladorFrete.Pedidos.Add(pedidoBloco);

            montagemCarregamentoBlocoSimuladorFretes.Add(montagemCarregamentoBlocoSimuladorFrete);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> ObterPedidosBlocoPesos(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> pedidosDoBloco, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> montagemCarregamentoBlocoSimuladorFretePedidos, bool ultimaCargaModelo)
        {
            pedidosDoBloco = pedidosDoBloco.OrderBy(x => x.Pedido.PesoTotal).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> resultado = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            decimal totalPesoCarregado = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido pedidoBloco in pedidosDoBloco)
            {
                decimal saldoPeso = 0;
                decimal saldoCubagem = 0;
                decimal saldoPallet = 0;
                decimal saldoVolumes = 0;
                ObterSaldoPedidoCarregar(pedidoBloco, montagemCarregamentoBlocoSimuladorFretePedidos, ref saldoPeso, ref saldoCubagem, ref saldoPallet, ref saldoVolumes);
                //Já carregou todo o pedido...
                if (saldoPeso <= (decimal)0.5)
                    continue;

                // Caso o pedido possa ser completamente carregado... ?
                if (saldoPeso + totalPesoCarregado <= modeloVeicularCarga.CapacidadePesoTransporte || ultimaCargaModelo)
                {
                    resultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido()
                    {
                        SimuladorFrete = pedidoBloco.SimuladorFrete,
                        Pedido = pedidoBloco.Pedido,
                        Peso = saldoPeso,
                        MetroCubico = saldoCubagem,
                        QuantidadePallet = saldoPallet,
                        Volumes = saldoVolumes
                    });
                    totalPesoCarregado += saldoPeso;
                }
                else
                {
                    // Fracionar o pedido em mais de um carregamento..
                    decimal pesoUnitario = 0;
                    if (pedidoBloco.Pedido.QtVolumes > 0)
                        pesoUnitario = (pedidoBloco.Pedido.PesoTotal / pedidoBloco.Pedido.QtVolumes);

                    decimal capacidadeCarregar = modeloVeicularCarga.CapacidadePesoTransporte - totalPesoCarregado;
                    if (pesoUnitario > 0 && pesoUnitario <= capacidadeCarregar)
                    {
                        decimal qtdeCarregar = 1;
                        while (qtdeCarregar * pesoUnitario <= capacidadeCarregar)
                            qtdeCarregar++;

                        qtdeCarregar--;

                        resultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido()
                        {
                            SimuladorFrete = pedidoBloco.SimuladorFrete,
                            Pedido = pedidoBloco.Pedido,
                            Peso = qtdeCarregar * pesoUnitario,
                            MetroCubico = (pedidoBloco.Pedido.CubagemTotal / pedidoBloco.Pedido.QtVolumes) * qtdeCarregar,
                            QuantidadePallet = (pedidoBloco.Pedido.TotalPallets / pedidoBloco.Pedido.QtVolumes) * qtdeCarregar,
                            Volumes = qtdeCarregar
                        });
                        totalPesoCarregado += qtdeCarregar * pesoUnitario;
                    }
                }
            }
            return resultado;
        }

        private void ObterSaldoPedidoCarregar(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido pedidoBloco, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> montagemCarregamentoBlocoSimuladorFretePedidos, ref decimal saldoPeso, ref decimal saldoCubagem, ref decimal saldoPallet, ref decimal saldoVolumes)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> carregados = (from o in montagemCarregamentoBlocoSimuladorFretePedidos where o.Pedido.Codigo == pedidoBloco.Pedido.Codigo select o).ToList();
            saldoPeso = pedidoBloco.Peso - carregados.Sum(x => x.Peso);
            saldoCubagem = pedidoBloco.MetroCubico - carregados.Sum(x => x.MetroCubico);
            saldoPallet = pedidoBloco.QuantidadePallet - carregados.Sum(x => x.QuantidadePallet);
            saldoVolumes = pedidoBloco.Volumes - carregados.Sum(x => x.Volumes);
        }

        private void RemoverTransportadorDisputa(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco bloco, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes)
        {
            montagemCarregamentoBlocoSimuladorFretes.RemoveAll(x => x.Transportador.Codigo == transportador.Codigo &&
                                                                    x.Bloco.Codigo == bloco.Codigo &&
                                                                    x.TipoOperacao.Codigo == centroCarregamentoTipoOperacao.TipoOperacao.Codigo &&
                                                                    x.Tipo == centroCarregamentoTipoOperacao.Tipo);
        }
    }
}
