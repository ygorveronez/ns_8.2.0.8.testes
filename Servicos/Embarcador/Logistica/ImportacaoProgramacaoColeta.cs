using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class ImportacaoProgramacaoColeta
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public ImportacaoProgramacaoColeta(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public int ProcessarImportacaoProgramacao(Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta, List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta> pedidosImportados, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(_unitOfWork);
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento repImportacaoProgramacaoColetaAgrupamento = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(importacaoProgramacaoColeta.ClienteDestino.CPF_CNPJ_SemFormato);

            if (filial == null)
                throw new ServicoException("Não foi encontrada uma filial com o cnpj de destino " + importacaoProgramacaoColeta.ClienteDestino.CPF_CNPJ_Formatado);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> agrupamentosImportados = repImportacaoProgramacaoColetaAgrupamento.BuscarPorImportacaoProgramacaoColeta(importacaoProgramacaoColeta.Codigo);

            //Agrupa pedidos
            List<int> agrupamentos = pedidosImportados.Where(o => o.Agrupamento > 0).Select(o => o.Agrupamento).Distinct().ToList();
            List<List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta>> pedidosAgrupados = new List<List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta>>();
            foreach (int agrupamento in agrupamentos)
                pedidosAgrupados.Add(pedidosImportados.Where(o => o.Agrupamento == agrupamento).ToList());

            //Gera pedidos e a carga por agrupamento
            int totalRegistrosImportados = 0;
            foreach (List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta> agrupamentoPedidos in pedidosAgrupados)
            {
                int numeroAgrupamento = agrupamentoPedidos.Select(o => o.Agrupamento).FirstOrDefault();
                Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento importacaoProgramacaoColetaAgrupamento = agrupamentosImportados.Where(o => o.NumeroAgrupamento == numeroAgrupamento).FirstOrDefault();
                if (importacaoProgramacaoColetaAgrupamento?.Carga != null)
                    continue;

                string mensagem = "Importado com sucesso";
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                try
                {
                    if (!agrupamentoPedidos.All(o => o.Sucesso))
                        throw new ServicoException(string.Join(" - ", agrupamentoPedidos.Where(o => !o.Sucesso).Select(o => o.MensagemFalha)));

                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = GerarPedidosImportacaoPorAgrupamento(importacaoProgramacaoColeta, agrupamentoPedidos, filial, configuracao, clienteMultisoftware, auditado);

                    string mensagemRetornoCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out carga, pedidos, _unitOfWork, _tipoServicoMultisoftware, clienteMultisoftware, configuracao, true, false, false, false);
                    if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                        throw new ServicoException(mensagemRetornoCarga);

                    _unitOfWork.CommitChanges();

                    totalRegistrosImportados += pedidos.Count;
                }
                catch (BaseException ex)
                {
                    _unitOfWork.Rollback();
                    mensagem = ex.Message;
                    carga = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    _unitOfWork.Rollback();
                    mensagem = "Ocorreu uma falha ao gerar os pedidos/carga do agrupamento";
                    carga = null;
                }

                //Grava vínculo do agrupamento
                if (importacaoProgramacaoColetaAgrupamento == null)
                {
                    importacaoProgramacaoColetaAgrupamento = new Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento();
                    importacaoProgramacaoColetaAgrupamento.NumeroAgrupamento = numeroAgrupamento;
                    importacaoProgramacaoColetaAgrupamento.ImportacaoProgramacaoColeta = importacaoProgramacaoColeta;
                }

                importacaoProgramacaoColetaAgrupamento.Carga = carga;
                importacaoProgramacaoColetaAgrupamento.Mensagem = mensagem;

                if (importacaoProgramacaoColetaAgrupamento.Codigo > 0)
                    repImportacaoProgramacaoColetaAgrupamento.Atualizar(importacaoProgramacaoColetaAgrupamento);
                else
                    repImportacaoProgramacaoColetaAgrupamento.Inserir(importacaoProgramacaoColetaAgrupamento);

                SobrescreverAgrupamentoProgramacaoColetaAnterior(carga, importacaoProgramacaoColetaAgrupamento, importacaoProgramacaoColeta, auditado);
            }

            if (repImportacaoProgramacaoColetaAgrupamento.PossuiAgrupamentoComFalha(importacaoProgramacaoColeta.Codigo))
                importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta = SituacaoImportacaoProgramacaoColeta.FalhaNaGeracao;
            else
                importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta = SituacaoImportacaoProgramacaoColeta.EmAndamento;
            repImportacaoProgramacaoColeta.Atualizar(importacaoProgramacaoColeta);

            if (agrupamentosImportados.Count > 0)
                Servicos.Auditoria.Auditoria.Auditar(auditado, importacaoProgramacaoColeta, "Importou novamente a planilha", _unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(auditado, importacaoProgramacaoColeta, "Realizou a importação da planilha", _unitOfWork);

            return totalRegistrosImportados;
        }

        public void GeracaoProximasCargas(Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(_unitOfWork);
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento repImportacaoProgramacaoColetaAgrupamento = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamentoProximaCarga repImportacaoProgramacaoColetaAgrupamentoProximaCarga = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamentoProximaCarga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> agrupamentosImportados = repImportacaoProgramacaoColetaAgrupamento.BuscarPorImportacaoProgramacaoColeta(importacaoProgramacaoColeta.Codigo);

            _unitOfWork.Start();

            try
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento agrupamento in agrupamentosImportados)
                {
                    if (agrupamento.AgrupamentoNovaProgramacao != null)
                        continue;

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = GerarPedidosProximaCarga(agrupamento, configuracao, auditado);

                    string mensagemRetornoCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, _unitOfWork, _tipoServicoMultisoftware, null, configuracao, true, false, false, false);
                    if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                        throw new Exception(mensagemRetornoCarga);

                    Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamentoProximaCarga proximaCarga = new Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamentoProximaCarga()
                    {
                        Carga = carga,
                        ImportacaoProgramacaoColetaAgrupamento = agrupamento
                    };

                    repImportacaoProgramacaoColetaAgrupamentoProximaCarga.Inserir(proximaCarga);
                }

                _unitOfWork.CommitChanges();

                importacaoProgramacaoColeta.QuantidadeRepeticoesEfetuadas++;
                importacaoProgramacaoColeta.DataUltimaGeracaoAutomatica = DateTime.Now;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
            }

            if (importacaoProgramacaoColeta.QuantidadeRepeticoesEfetuadas == importacaoProgramacaoColeta.NumeroRepeticoes)
            {
                importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta = SituacaoImportacaoProgramacaoColeta.Finalizado;
                Servicos.Auditoria.Auditoria.Auditar(auditado, importacaoProgramacaoColeta, "Finalizado a geração das próximas cargas", _unitOfWork);
            }

            repImportacaoProgramacaoColeta.Atualizar(importacaoProgramacaoColeta);
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> GerarPedidosImportacaoPorAgrupamento(Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta, List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta> agrupamentoPedidos, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(importacaoProgramacaoColeta.TipoOperacao.Codigo);
            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(importacaoProgramacaoColeta.ClienteDestino.CPF_CNPJ);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta importacaoPedido in agrupamentoPedidos)
            {
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(importacaoPedido.Remetente.CPF_CNPJ);
                Dominio.Entidades.Usuario motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(importacaoPedido.Veiculo.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                {
                    Produto = importacaoProgramacaoColeta.Produto,
                    QuantidadePlanejada = importacaoPedido.QuantidadePlanejada
                };

                Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar()
                {
                    Empresa = importacaoPedido.Transportador,
                    Veiculo = importacaoPedido.Veiculo,
                    OrdemColeta = importacaoPedido.Sequencia,
                    ModeloVeicularCarga = importacaoPedido.Veiculo.ModeloVeicularCarga,
                    DataHoraCarregamento = importacaoPedido.DataCarregamento,
                    Distancia = importacaoPedido.Distancia,
                    TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente,
                    Remetente = remetente,
                    Destinatario = destinatario,
                    TipoOperacao = tipoOperacao,
                    Motoristas = motorista != null ? new List<Dominio.Entidades.Usuario>() { motorista } : null,
                    Filial = filial,
                    PedidoProduto = pedidoProduto
                };

                pedidos.Add(AdicionarPedidoImportacao(pedidoImportacaoAdicionar, importacaoProgramacaoColeta.NumeroImportacao, configuracaoTMS, clienteMultisoftware, auditado));
            }

            return pedidos;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido AdicionarPedidoImportacao(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar, int numeroImportacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(_unitOfWork);

            Servicos.Embarcador.Pedido.Pedido servPedido = new Servicos.Embarcador.Pedido.Pedido();
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
            pedido.Numero = repPedido.BuscarProximoNumero();
            pedido.CodigoPedidoCliente = "";
            pedido.DataFinalColeta = DateTime.Now;
            pedido.DataInicialColeta = DateTime.Now;
            pedido.DataPrevisaoChegadaDestinatario = pedidoImportacaoAdicionar.DataHoraDescarga;
            pedido.Adicional1 = pedidoImportacaoAdicionar.Adicional1;
            pedido.Adicional2 = pedidoImportacaoAdicionar.Adicional2;
            pedido.Adicional3 = pedidoImportacaoAdicionar.Adicional3;
            pedido.Adicional4 = pedidoImportacaoAdicionar.Adicional4;
            pedido.Adicional5 = pedidoImportacaoAdicionar.Adicional5;
            pedido.Adicional6 = pedidoImportacaoAdicionar.Adicional6;
            pedido.Adicional7 = pedidoImportacaoAdicionar.Adicional7;
            pedido.NumeroCargaEncaixar = pedidoImportacaoAdicionar.NumeroCargaEncaixar;
            pedido.NumeroControle = pedidoImportacaoAdicionar.NumeroControle;
            pedido.TipoEmbarque = pedidoImportacaoAdicionar.TipoEmbarque;
            pedido.Observacao = pedidoImportacaoAdicionar.Observacao;
            pedido.OrdemColetaProgramada = pedidoImportacaoAdicionar.OrdemColeta;
            pedido.QuebraMultiplosCarregamentos = pedidoImportacaoAdicionar.QuebraMultiplosCarregamentos;
            pedido.Deposito = pedidoImportacaoAdicionar.Deposito;
            pedido.QtVolumes = pedidoImportacaoAdicionar.QtdVolumes;
            pedido.SaldoVolumesRestante = pedidoImportacaoAdicionar.QtdVolumes;
            pedido.ValorTotalNotasFiscais = pedidoImportacaoAdicionar.ValorTotalPedido;
            pedido.NumeroNotaCliente = pedidoImportacaoAdicionar.NumeroNotaCliente;
            pedido.ObservacaoAdicional = pedidoImportacaoAdicionar.ObservacaoAdicional ?? string.Empty;
            pedido.NumeroOrdem = pedidoImportacaoAdicionar.NumeroOrdem;
            pedido.DataAlocacaoPedido = pedidoImportacaoAdicionar.DataAlocacaoPedido;
            pedido.PossuiIsca = pedidoImportacaoAdicionar.PossuiIsca;
            pedido.PossuiEtiquetagem = pedidoImportacaoAdicionar.PossuiEtiquetagem;
            pedido.Destino = pedidoImportacaoAdicionar.Cidade;
            pedido.GrossSales = pedidoImportacaoAdicionar.GrossSales;
            pedido.SituacaoAgendamentoEntregaPedido = (pedidoImportacaoAdicionar.Destinatario?.ExigeQueEntregasSejamAgendadas ?? false) ? pedidoImportacaoAdicionar.SituacaoAgendamentoEntregaPedido : SituacaoAgendamentoEntregaPedido.NaoExigeAgendamento;
            pedido.QuantidadeVolumesPrevios = pedidoImportacaoAdicionar.QuantidadeVolumesPrevios;
            pedido.CodigoPedidoCliente = pedidoImportacaoAdicionar.NumeroPedidoCliente;
            pedido.ValorTotalNotasFiscais = pedidoImportacaoAdicionar.ValorTotalNotasFiscais;
            pedido.ObservacaoInterna = pedidoImportacaoAdicionar.ObservacaoInterna ?? string.Empty;
            pedido.Distancia = pedidoImportacaoAdicionar.Distancia;

            if (pedidoImportacaoAdicionar.DataHoraCarregamento != DateTime.MinValue)
                pedido.DataCarregamentoPedido = pedidoImportacaoAdicionar.DataHoraCarregamento;
            else
                pedido.DataCarregamentoPedido = DateTime.Now;

            pedido.ProdutoPrincipal = pedidoImportacaoAdicionar.ProdutoPrincipal;

            pedido.Remetente = pedidoImportacaoAdicionar.Remetente;
            pedido.Expedidor = pedidoImportacaoAdicionar.Expedidor;
            pedido.Recebedor = pedidoImportacaoAdicionar.Recebedor;
            pedido.Destinatario = pedidoImportacaoAdicionar.Destinatario;
            pedido.LocalExpedicao = pedidoImportacaoAdicionar.LocalExpedicao;

            if ((pedido?.Destinatario?.GerarPedidoBloqueado ?? false) || configuracaoPedido.BloquearPedidoAoIntegrar)
                pedido.PedidoBloqueado = true;

            if (pedido.Destino == null && pedido.Recebedor != null && pedido.Recebedor.Localidade != null)
                pedido.Destino = pedido.Recebedor.Localidade;
            else if (pedido.Destino == null && pedido.Destinatario != null && pedido.Destinatario.Localidade != null)
                pedido.Destino = pedido.Destinatario.Localidade;

            if (pedidoImportacaoAdicionar.DataPrevEntrega != DateTime.MinValue)
                pedido.PrevisaoEntrega = pedidoImportacaoAdicionar.DataPrevEntrega;

            if (pedido.Remetente != null)
            {
                pedido.GrupoPessoas = pedidoImportacaoAdicionar.Remetente.GrupoPessoas;
                pedido.Origem = pedidoImportacaoAdicionar.Remetente.Localidade;
            }

            pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
            pedido.Empresa = pedidoImportacaoAdicionar.Empresa;

            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            if (pedido.Remetente != null)
                servPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);

            if (pedido.Destinatario != null)
                servPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);

            if (pedidoEnderecoOrigem.Localidade != null)
                repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
            if (pedidoEnderecoDestino.Localidade != null)
                repPedidoEndereco.Inserir(pedidoEnderecoDestino);

            if (pedidoEnderecoOrigem.Localidade != null)
            {
                pedido.Origem = pedidoEnderecoOrigem.Localidade;
                pedido.EnderecoOrigem = pedidoEnderecoOrigem;
            }

            if (pedidoEnderecoDestino.Localidade != null)
            {
                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }

            pedido.QtdEntregas = 1;
            pedido.PedidoTransbordo = false;
            pedido.UsarOutroEnderecoOrigem = false;
            pedido.UsarOutroEnderecoDestino = false;

            if (pedido.Destinatario != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(pedido.Destinatario.CPF_CNPJ);
                if (clienteDescarga != null && clienteDescarga.RestricoesDescarga != null)
                {
                    pedido.RestricoesDescarga = clienteDescarga.RestricoesDescarga.ToList();

                    foreach (var restricao in pedido.RestricoesDescarga)
                    {
                        string email = restricao.Email ?? "";
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            List<string> emails = email.Split(';').ToList();
                            bool sucesso = servPedido.EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, _unitOfWork, out string mensagem);
                            if (!sucesso)
                                Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                        }
                    }
                }
            }

            pedido.RotaFrete = pedidoImportacaoAdicionar.RotaFrete;

            if (pedido.RotaFrete == null && pedido.Empresa != null && pedido.Origem != null && pedido.Destino != null)
            {
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportador(pedido.Origem.Codigo, pedido.Destino.Codigo, pedidoImportacaoAdicionar.TipoOperacao?.Codigo ?? 0, pedido.Empresa?.Codigo ?? 0);
                pedido.RotaFrete = rotaFrete;
            }

            if (pedido.RotaFrete == null && pedido.Destino != null)
            {
                pedido.RotaFrete = repRotaFrete.BuscarPorLocalidade(pedido.Destino, true);

                if (pedido.RotaFrete == null)
                    pedido.RotaFrete = repRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);
            }

            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

            if (!string.IsNullOrWhiteSpace(pedidoImportacaoAdicionar.TipoCarga?.ProdutoPredominante))
                pedido.ProdutoPredominante = pedidoImportacaoAdicionar.TipoCarga.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(pedidoImportacaoAdicionar.TipoOperacao?.ProdutoPredominanteOperacao))
                pedido.ProdutoPredominante = pedidoImportacaoAdicionar.TipoOperacao.ProdutoPredominanteOperacao;
            else if (tomador?.GrupoPessoas != null && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.ProdutoPredominante))
                pedido.ProdutoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(configuracaoTMS.DescricaoProdutoPredominatePadrao))
                pedido.ProdutoPredominante = configuracaoTMS.DescricaoProdutoPredominatePadrao;
            else
                pedido.ProdutoPredominante = "Importação";

            if (pedidoImportacaoAdicionar.Filial != null)
                pedido.Filial = pedidoImportacaoAdicionar.Filial;

            pedido.AdicionadaManualmente = true;
            pedido.NumeroPaletesFracionado = pedidoImportacaoAdicionar.QuantidadePalletsFacionada;

            if (configuracaoTMS.NumeroCargaSequencialUnico)
                pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
            else
                pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(pedido.Filial);

            pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();

            pedido.PesoTotal = pedidoImportacaoAdicionar.PesoPedido;
            pedido.PesoSaldoRestante = pedidoImportacaoAdicionar.PesoPedido;
            pedido.CubagemTotal = pedidoImportacaoAdicionar.CubagemPedido;

            pedido.PreCarga = pedidoImportacaoAdicionar.PreCarga;
            pedido.ObservacaoCTe = configuracaoTMS.ObservacaoCTePadraoEmbarcador ?? "";
            pedido.Temperatura = "";
            pedido.Requisitante = RequisitanteColeta.Remetente;
            pedido.SituacaoPedido = SituacaoPedido.Aberto;

            pedido.TipoDeCarga = pedidoImportacaoAdicionar.TipoCarga;
            pedido.ModeloVeicularCarga = pedidoImportacaoAdicionar.ModeloVeicularCarga;
            pedido.CanalEntrega = pedidoImportacaoAdicionar.CanalEntrega;

            pedido.TipoOperacao = pedidoImportacaoAdicionar.TipoOperacao;

            if ((pedido.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && !string.IsNullOrWhiteSpace(pedido.TipoOperacao.ObservacaoCTe))
                pedido.ObservacaoCTe += string.Concat(" ", pedido.TipoOperacao.ObservacaoCTe);

            pedido.TipoTomador = pedidoImportacaoAdicionar.TipoTomador;
            pedido.UsarTipoTomadorPedido = true;
            if (pedidoImportacaoAdicionar.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.Usuario = pedidoImportacaoAdicionar.Usuario;
            pedido.Autor = pedidoImportacaoAdicionar.Usuario;

            pedido.NomeArquivoGerador = pedidoImportacaoAdicionar.NomeArquivoGerador;
            pedido.GuidArquivoGerador = pedidoImportacaoAdicionar.GuidArquivoGerador;
            pedido.PedidoIntegradoEmbarcador = true;
            pedido.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;
            repPedido.Inserir(pedido);

            pedido.Protocolo = pedido.Codigo;

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, "Criou Pedido via importação programação coleta nº " + numeroImportacao, _unitOfWork);

            if (pedidoImportacaoAdicionar.PedidoProduto != null)
            {
                pedidoImportacaoAdicionar.PedidoProduto.Pedido = pedido;
                repPedidoProduto.Inserir(pedidoImportacaoAdicionar.PedidoProduto);
                pedido.PesoTotal += pedidoImportacaoAdicionar.PedidoProduto.PesoUnitario * (pedidoImportacaoAdicionar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoAdicionar.PedidoProduto.Quantidade);
                pedido.PesoSaldoRestante += pedidoImportacaoAdicionar.PedidoProduto.PesoUnitario * (pedidoImportacaoAdicionar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoAdicionar.PedidoProduto.Quantidade);
            }

            repPedido.Atualizar(pedido);
            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoGerado, pedido, configuracaoTMS, clienteMultisoftware);

            if (pedido.TipoOperacao?.ProdutoEmbarcadorPadraoColeta != null && pedidoImportacaoAdicionar.PedidoProduto == null)
            {
                pedidoImportacaoAdicionar.PedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                pedidoImportacaoAdicionar.PedidoProduto.Pedido = pedido;
                pedidoImportacaoAdicionar.PedidoProduto.Produto = pedido.TipoOperacao.ProdutoEmbarcadorPadraoColeta;
                repPedidoProduto.Inserir(pedidoImportacaoAdicionar.PedidoProduto);
            }

            if (pedidoImportacaoAdicionar.Veiculo != null)
                pedido.Veiculos.Add(pedidoImportacaoAdicionar.Veiculo);

            if (pedidoImportacaoAdicionar.Reboques != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoAdicionar.Reboques)
                    pedido.Veiculos.Add(reboque);
            }
            else if (pedidoImportacaoAdicionar.Veiculo?.VeiculosVinculados.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoAdicionar.Veiculo.VeiculosVinculados)
                    pedido.Veiculos.Add(reboque);
            }

            if (pedidoImportacaoAdicionar.Motoristas != null)
            {
                if (pedido.Motoristas == null)
                    pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.Entidades.Usuario motorista in pedidoImportacaoAdicionar.Motoristas)
                    pedido.Motoristas.Add(motorista);
            }

            return pedido;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> GerarPedidosProximaCarga(Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento agrupamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(agrupamento.Carga.Codigo);

            Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta = agrupamento.ImportacaoProgramacaoColeta;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> novosPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                novosPedidos.Add(DuplicarPedido(pedido, (pedido.DataCarregamentoPedido ?? DateTime.Now.Date).AddDays(importacaoProgramacaoColeta.IntervaloDiasGeracao), importacaoProgramacaoColeta.NumeroImportacao, configuracao, auditado));

            return novosPedidos;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido DuplicarPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime dataProximoCarregamento, int numeroImportacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoNovo = pedido.Clonar();

            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoNovo);

            pedidoNovo.Numero = repPedido.BuscarProximoNumero();
            if (configuracao.NumeroCargaSequencialUnico)
                pedidoNovo.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
            else
                pedidoNovo.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(pedidoNovo.Filial);
            pedidoNovo.NumeroPedidoEmbarcador = pedidoNovo.NumeroSequenciaPedido.ToString();
            pedidoNovo.DataCarregamentoPedido = dataProximoCarregamento;
            pedidoNovo.DataCriacao = DateTime.Now;
            pedidoNovo.DataFinalColeta = DateTime.Now;
            pedidoNovo.DataInicialColeta = DateTime.Now;
            pedidoNovo.UltimaAtualizacao = DateTime.Now;
            pedidoNovo.SituacaoPedido = SituacaoPedido.Aberto;
            pedidoNovo.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;
            pedidoNovo.PedidoIntegradoEmbarcador = true;

            if (pedido.EnderecoOrigem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = pedido.EnderecoOrigem.Clonar();
                enderecoOrigem.Codigo = 0;
                repPedidoEndereco.Inserir(enderecoOrigem);
                pedidoNovo.EnderecoOrigem = enderecoOrigem;
            }

            if (pedido.EnderecoDestino != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = pedido.EnderecoDestino.Clonar();
                enderecoDestino.Codigo = 0;
                repPedidoEndereco.Inserir(enderecoDestino);
                pedidoNovo.EnderecoDestino = enderecoDestino;
            }

            if (pedido.Veiculos?.Count > 0)
            {
                if (pedidoNovo.Veiculos == null)
                    pedidoNovo.Veiculos = new List<Dominio.Entidades.Veiculo>();

                foreach (Dominio.Entidades.Veiculo reboque in pedido.Veiculos)
                    pedidoNovo.Veiculos.Add(reboque);
            }

            if (pedido.Motoristas?.Count > 0)
            {
                if (pedidoNovo.Motoristas == null)
                    pedidoNovo.Motoristas = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.Entidades.Usuario motorista in pedido.Motoristas)
                    pedidoNovo.Motoristas.Add(motorista);
            }

            repPedido.Inserir(pedidoNovo);

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoNovo, "Criou Pedido via geração próximas cargas da importação programação coleta nº " + numeroImportacao, _unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = repPedidoProduto.BuscarPorPedido(pedido.Codigo);
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoAntigo in produtos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoNovo = produtoAntigo.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(produtoNovo);
                produtoNovo.Pedido = pedidoNovo;
                repPedidoProduto.Inserir(produtoNovo);
            }

            return pedidoNovo;
        }

        private void SobrescreverAgrupamentoProgramacaoColetaAnterior(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento importacaoProgramacaoColetaAgrupamento, Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (carga == null || carga.Veiculo == null)
                return;

            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(_unitOfWork);
            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento repImportacaoProgramacaoColetaAgrupamento = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> agrupamentosAnteriores = repImportacaoProgramacaoColetaAgrupamento.BuscarProgramacaoColetaAnteriorComMesmoVeiculo(importacaoProgramacaoColeta.Codigo, carga.Veiculo.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento agrupamentoAnterior in agrupamentosAnteriores)
            {
                agrupamentoAnterior.AgrupamentoNovaProgramacao = importacaoProgramacaoColetaAgrupamento;
                repImportacaoProgramacaoColetaAgrupamento.Atualizar(agrupamentoAnterior);

                Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColetaAnterior = agrupamentoAnterior.ImportacaoProgramacaoColeta;

                Servicos.Auditoria.Auditoria.Auditar(auditado, importacaoProgramacaoColetaAnterior, $"Agrupamento {agrupamentoAnterior.NumeroAgrupamento} foi subscrito pela nova programação {importacaoProgramacaoColeta.NumeroImportacao}", _unitOfWork);

                if (repImportacaoProgramacaoColetaAgrupamento.TodosOsAgrupamentosEstaoEmNovasProgramacoes(importacaoProgramacaoColetaAnterior.Codigo))
                {
                    importacaoProgramacaoColetaAnterior.SituacaoImportacaoProgramacaoColeta = SituacaoImportacaoProgramacaoColeta.Finalizado;
                    repImportacaoProgramacaoColeta.Atualizar(importacaoProgramacaoColetaAnterior);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, importacaoProgramacaoColetaAnterior, "Finalizado automaticamente devido todos os agrupamentos estarem em novas programações", _unitOfWork);
                }
            }
        }

        #endregion
    }
}
