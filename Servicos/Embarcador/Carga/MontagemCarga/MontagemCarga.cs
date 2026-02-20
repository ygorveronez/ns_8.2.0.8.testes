using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Servicos.Embarcador.Notificacao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public class MontagemCarga
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga _configuracaoMontagemCarga;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;
        private Repositorio.UnitOfWork _unitOfWork;
        private string _stringConexao;
        private List<int> _codigosCarregamentosGerandoCarga;
        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> _pedidosProdutos;
        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> _carregamentoPedidoProdutosAtendidos;
        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> _carregamentosSessao;
        private Dictionary<int, decimal> _saldoPedido;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private int _codigoSessaoRoteirizador;

        #endregion

        #region Contrutores

        public MontagemCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, stringConexao: null) { }

        public MontagemCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, string stringConexao = null)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
            _stringConexao = stringConexao;
            _configuracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork).BuscarPrimeiroRegistro();
        }

        #endregion

        #region Métodos Privados

        private void AdicionarPesoCarregadoDicionario(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, decimal pesoCarregar, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> retornoPedidos, ref Dictionary<int, decimal> pedidosPesosCarregamento)
        {
            if (!retornoPedidos.Contains(pedido))
                retornoPedidos.Add(pedido);

            if (!pedidosPesosCarregamento.ContainsKey(pedido.Codigo))
                pedidosPesosCarregamento[pedido.Codigo] = pesoCarregar;
            else
                pedidosPesosCarregamento[pedido.Codigo] += pesoCarregar;
        }

        /// <summary>
        /// Procedimento para adicionar os recursos disponíveis para adicionar para a api Roteirizar.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="disponibilidadeDiaUtilizar"></param>
        /// <param name="temposCarregamentoCentro"></param>
        /// <param name="codigoTipoCarga"></param>
        /// <param name="tipoOcupacao"></param>
        /// <param name="agrupados"></param>
        private void ApiOrToolsDisponibilidadeUtilizar(ref GoogleOrTools.Api api, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento> temposCarregamentoCentro, int codigoTipoCarga, TipoOcupacaoMontagemCarregamentoVRP tipoOcupacao, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupados)
        {
            // Percentual máximo de ocupação do modelo veicular..
            int percentual = 100;

            for (int i = 0; i < disponibilidadeDiaUtilizar.Count; i++)
            {
                //Validar se esse modelo.. atende algum destinatario..
                bool existeSemRestricao = api.Locais.Exists(x => x.Deposito == false && (x.VeiculosRestritos?.Count ?? 0) == 0);
                //Se existe algum destinatário .. sem nenhum modelo restrito.. segue o baile..
                if (!existeSemRestricao)
                {
                    foreach (GoogleOrTools.Local loc in api.Locais)
                    {
                        if (loc.Deposito) continue;

                        if (loc.VeiculosRestritos == null) loc.VeiculosRestritos = new List<int>();

                        existeSemRestricao = !loc.VeiculosRestritos.Contains(disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo);

                        if (existeSemRestricao)
                            break;
                    }
                }

                int qtdeMaxEntregas = api.QtdeMaximaEntregas;

                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento tempoCarregamento = (from tc in temposCarregamentoCentro
                                                                                                                                         where tc.CodigoTipoCarga == codigoTipoCarga &&
                                                                                                                                               tc.CodigoModeloVeicular == disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo
                                                                                                                                         select tc).FirstOrDefault();

                if (tempoCarregamento == null)
                    tempoCarregamento = (from tc in temposCarregamentoCentro
                                         where tc.CodigoTipoCarga == codigoTipoCarga
                                         select tc).FirstOrDefault();

                if (tempoCarregamento == null)
                    tempoCarregamento = (from tc in temposCarregamentoCentro
                                         where tc.CodigoModeloVeicular == disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo
                                         select tc).FirstOrDefault();

                if (tempoCarregamento != null)
                    qtdeMaxEntregas = tempoCarregamento.QuantidadeUtilizar;


                int qtdeModelo = (disponibilidadeDiaUtilizar[i].Quantidade - (from gr in agrupados where gr.ModeloVeicular.Codigo == disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo select gr).Count());

                int modeloVeicularCarregaTipoCarga = 1;
                if (codigoTipoCarga > 0)
                    modeloVeicularCarregaTipoCarga = (from tempo in temposCarregamentoCentro
                                                      where tempo.CodigoModeloVeicular == disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo && tempo.CodigoTipoCarga == codigoTipoCarga
                                                      select tempo).Count();

                if (qtdeModelo > 0 && existeSemRestricao && (qtdeMaxEntregas + api.QtdeMaximaEntregas) > 0 && modeloVeicularCarregaTipoCarga > 0)
                {
                    long capacVeiculo = (long)((disponibilidadeDiaUtilizar[i].ModeloVeicular.CapacidadePesoTransporte + disponibilidadeDiaUtilizar[i].ModeloVeicular.ToleranciaPesoExtra) * (percentual / 100));
                    if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                        capacVeiculo = (long)disponibilidadeDiaUtilizar[i].ModeloVeicular.Cubagem * 1000;
                    else if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                        capacVeiculo = (long)disponibilidadeDiaUtilizar[i].ModeloVeicular.NumeroPaletes * 1000;

                    if (capacVeiculo > 0)
                    {
                        api.Veiculos.Add(new GoogleOrTools.Veiculo()
                        {
                            Codigo = disponibilidadeDiaUtilizar[i].Codigo,
                            CodigoModelo = disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo,
                            Descricao = disponibilidadeDiaUtilizar[i].ModeloVeicular.Descricao,
                            Capacidade = capacVeiculo,
                            Quantidade = qtdeModelo,
                            QtdeMaximaEntregas = (qtdeMaxEntregas > 0 ? qtdeMaxEntregas : api.QtdeMaximaEntregas)
                        });
                    }
                }
            }
        }

        private void ApiOrToolsLocaisDistintosVisitar(ref GoogleOrTools.Api api, dynamic locais_distintos, TipoOcupacaoMontagemCarregamentoVRP tipoOcupacao, TipoMontagemCarregamentoVRP vrp, bool agruparMesmoDestinatario, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosFechando, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centroDescarregamentos, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            //Obter a minima data de previsão de entrega.. para calcular a penalidade.. somando com o nível de prioridade do canal de entrega.
            DateTime dataMinimaPrevEntrega = DateTime.Now.Date;
            List<DateTime> datasPrevisaoEntrega = (from obj in pedidosFechando
                                                   where obj.PrevisaoEntrega.HasValue
                                                   select obj.PrevisaoEntrega.Value.Date).ToList();
            if ((datasPrevisaoEntrega?.Count ?? 0) > 0)
                dataMinimaPrevEntrega = datasPrevisaoEntrega.Min();

            foreach (var local_entrega in locais_distintos)
            {
                double total = (double)local_entrega.peso_total;
                if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                    total = (double)local_entrega.metro_total * 1000;
                else if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                    total = (double)local_entrega.pallet_total * 1000;

                long cnpjDestinatario = (long)local_entrega.id;

                if (!agruparMesmoDestinatario)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoTmp = pedidosFechando.Find(x => x.Codigo == (int)local_entrega.id);
                    if (pedidoTmp != null)
                    {
                        if (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Entrega)
                            cnpjDestinatario = (pedidoTmp.Recebedor != null ? pedidoTmp.Recebedor.Codigo : pedidoTmp.Destinatario.Codigo);
                        else
                            cnpjDestinatario = (long)pedidoTmp.Remetente.Codigo;
                    }

                    if (pedidoTmp.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false)
                        total = 0.1;
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosLocal = (from obj in pedidosFechando
                                                                                      where local_entrega.pedidos.Contains(obj.Codigo)
                                                                                      select obj).ToList();

                    if (pedidosLocal.Any(x => x.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false))
                        total = 0.1;

                }

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesRestritos = (from restr in modalidadeFornecedorPessoasRestricaoModeloVeicular
                                                                                                            where restr.ModalidadeFornecedorPessoa.ModalidadePessoas.Cliente.Codigo == cnpjDestinatario
                                                                                                            select restr.ModeloVeicular).ToList();

                //Se não possui restrição.. vamos ver os veiculos permitidos da Janela de Descarga.
                if (modelosVeicularesRestritos?.Count == 0)
                {
                    modelosVeicularesRestritos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from centro in centroDescarregamentos
                                                                                                          where (centro.Destinatario?.CPF_CNPJ ?? 0) == cnpjDestinatario
                                                                                                          select centro).FirstOrDefault();

                    //Se o destinatário possuir um centro de descarregamento.
                    if (centroDescarregamento != null)
                    {
                        if (centroDescarregamento.VeiculosPermitidos?.Count > 0)
                        {
                            // Modelos veiculares da disponibilidade de frota do centro de carregamento
                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis = (from modelo in disponibilidadeDiaUtilizar
                                                                                                                select modelo.ModeloVeicular).Distinct().ToList();

                            //Localizando os modelos do centro.. que não estão nos modelos permitidos do centro de descarga...
                            modelosVeicularesRestritos = modelosDisponiveis.FindAll(x => !centroDescarregamento.VeiculosPermitidos.Any(d => d.Codigo == x.Codigo)).ToList();
                        }
                    }
                }

                // Para cada pedido (DataPrevisaoEntrega - DataMinimaPedidos).TotalDays + prioridade canal.
                // TODO: Ver para atualizar a API com o atributo Pedidos, para passar a lista de pedidos.. para quando vier o resultado.. ficar mais fácil de localizar os pedidos de cada carregamento
                GoogleOrTools.Local local = new GoogleOrTools.Local()
                {
                    Codigo = (long)local_entrega.id,
                    Latitude = local_entrega.latitude,
                    Longitude = local_entrega.longitude,
                    PesoTotal = total,
                    CodigoAux = (long)local_entrega.ponto_apoio + local_entrega.endereco,
                    VeiculosRestritos = (from res in modelosVeicularesRestritos select res.Codigo).Distinct().ToList(),
                    Pedidos = local_entrega.pedidos,
                    TipoPonto = local_entrega.tipo_ponto,
                    PedidosConfig = (from obj in pedidosFechando
                                     where local_entrega.pedidos.Contains(obj.Codigo)
                                     select new GoogleOrTools.Pedido()
                                     {
                                         Codigo = obj.Codigo,
                                         CodigoCanalEntrega = obj.CanalEntrega?.Codigo ?? 0,
                                         CanalEntreaga = obj.CanalEntrega?.Descricao ?? string.Empty,
                                         LimiteCanalEntrega = obj.CanalEntrega?.QuantidadePedidosPermitidosNoCanal ?? 0,
                                         Prioridade = (obj.CanalEntrega?.NivelPrioridade ?? 99) + ((int)((obj?.PrevisaoEntrega?.Date ?? DateTime.Now.Date) - dataMinimaPrevEntrega).TotalDays)
                                     }).ToList()
                };

                if (vrp == TipoMontagemCarregamentoVRP.VrpTimeWindows)
                {
                    int time = 10;
                    int start = 0;
                    int end = 1200;

                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from centro in centroDescarregamentos
                                                                                                          where (centro.Destinatario?.CPF_CNPJ ?? 0) == cnpjDestinatario
                                                                                                          select centro).FirstOrDefault();

                    if (centroDescarregamento == null)
                    {
                        List<int> codigosCanaisEntregaCliente = (from pedido in pedidosFechando
                                                                 where pedido.CanalEntrega != null &&
                                                                 (pedido.Recebedor != null ? pedido.Recebedor.Codigo : pedido.Destinatario.Codigo) == cnpjDestinatario
                                                                 select pedido.CanalEntrega?.Codigo ?? 0).Distinct().ToList();
                        if (codigosCanaisEntregaCliente?.Count > 0)
                            centroDescarregamento = (from centro in centroDescarregamentos where codigosCanaisEntregaCliente.Contains(centro.CanalEntrega?.Codigo ?? 0) select centro).FirstOrDefault();
                    }

                    if (centroDescarregamento != null)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento> tempos = centroDescarregamento.TemposDescarregamento.ToList();
                        if (tempos?.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = (from pedido in pedidosFechando
                                                                                         where (pedido.Recebedor != null ? pedido.Recebedor.Codigo : pedido.Destinatario.Codigo) == cnpjDestinatario &&
                                                                                               pedido.TipoDeCarga != null
                                                                                         select pedido.TipoDeCarga).FirstOrDefault();
                            Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento tempo = null;
                            if (tipoCarga != null)
                            {
                                tempo = (from t in tempos where t.TipoCarga.Codigo == tipoCarga.Codigo select t).FirstOrDefault();
                                if (tempo == null)
                                    tempo = (from t in tempos select t).FirstOrDefault();
                            }
                            if (tempo != null)
                            {
                                time = tempo.Tempo;
                                if (tempo.TipoTempo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TempoDescarregamentoTipoTempo.Tonelada)
                                    time *= ((int)local_entrega.peso_total / 1000);
                            }
                        }
                    }
                    local.Janela = new GoogleOrTools.TimeWindow()
                    {
                        time = time,
                        start = start,
                        end = end
                    };
                }

                api.Locais.Add(local);
            }
        }

        private void ApiOrToolsDefinirMenorVeiculoResultado(int codigoTipoCarga, bool tipoCargaPaletizado, TipoOcupacaoMontagemCarregamentoVRP tipoOcupacao, int qtdeMaximaEntregas, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento> temposCarregamentoCentro, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centroDescarregamentos, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupadosParcial, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> todosAgrupados)
        {

            for (int i = 0; i < agrupadosParcial?.Count; i++)
            {
                decimal totalPesoCarregado = agrupadosParcial[i].PedidosPesos.Sum(x => x.Value);

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> veiculos = (
                    from veiculo in disponibilidadeDiaUtilizar
                    where (veiculo.ModeloVeicular.CapacidadePesoTransporte + veiculo.ModeloVeicular.ToleranciaPesoExtra) >= totalPesoCarregado
                    select veiculo
                ).OrderBy(x => (x.ModeloVeicular.CapacidadePesoTransporte + x.ModeloVeicular.ToleranciaPesoExtra)).ToList();

                if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                {
                    decimal metroCarregado = agrupadosParcial[i].Pedidos.Sum(x => x.CubagemTotal);

                    veiculos = (
                        from veiculo in disponibilidadeDiaUtilizar
                        where veiculo.ModeloVeicular.Cubagem >= (metroCarregado + (tipoCargaPaletizado ? veiculo.ModeloVeicular.ObterOcupacaoCubicaPaletes() : 0m))
                        select veiculo
                    ).OrderBy(x => x.ModeloVeicular.Cubagem).ToList();
                }
                else if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                {
                    decimal palletCarregado = agrupadosParcial[i].Pedidos.Sum(x => x.TotalPallets);

                    veiculos = (
                        from veiculo in disponibilidadeDiaUtilizar
                        where veiculo.ModeloVeicular.NumeroPaletes >= palletCarregado
                        select veiculo
                    ).OrderBy(x => x.ModeloVeicular.NumeroPaletes).ToList();
                }

                for (int j = 0; j < veiculos?.Count; j++)
                {
                    int codigoDisponibilidade = veiculos[j].Codigo;
                    int codigoModeloVeicular = veiculos[j].ModeloVeicular.Codigo;

                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota frota = (
                        from d in disponibilidadeDiaUtilizar
                        where d.Codigo == codigoDisponibilidade
                        select d
                    ).FirstOrDefault();

                    if (agrupadosParcial[i].ModeloVeicular.Codigo == frota.ModeloVeicular.Codigo)
                        break;
                    else
                    {
                        //Analisar se o total de carregamentos dos agrupados... 
                        int qtdeCarregamentosModelo = (from agr in todosAgrupados where agr.ModeloVeicular.Codigo == frota.ModeloVeicular.Codigo select agr).Count();
                        if (qtdeCarregamentosModelo < frota.Quantidade)
                        {

                            int qtdeMaxEntregas = qtdeMaximaEntregas;

                            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento tempoCarregamento = (from tc in temposCarregamentoCentro
                                                                                                                                                     where tc.CodigoTipoCarga == codigoTipoCarga && tc.CodigoModeloVeicular == codigoModeloVeicular
                                                                                                                                                     select tc).FirstOrDefault();

                            if (tempoCarregamento == null && codigoTipoCarga > 0)
                                tempoCarregamento = (from tc in temposCarregamentoCentro
                                                     where tc.CodigoTipoCarga == codigoTipoCarga
                                                     select tc).FirstOrDefault();

                            if (tempoCarregamento == null)
                                tempoCarregamento = (from tc in temposCarregamentoCentro
                                                     where tc.CodigoModeloVeicular == codigoModeloVeicular
                                                     select tc).FirstOrDefault();

                            if (tempoCarregamento != null && (tempoCarregamento?.QuantidadeUtilizar ?? 0) > 0)
                                qtdeMaxEntregas = tempoCarregamento.QuantidadeUtilizar;

                            int modeloVeicularCarregaTipoCarga = 1;
                            if (codigoTipoCarga > 0)
                                modeloVeicularCarregaTipoCarga = (from tempo in temposCarregamentoCentro
                                                                  where tempo.CodigoModeloVeicular == frota.ModeloVeicular.Codigo && tempo.CodigoTipoCarga == codigoTipoCarga
                                                                  select tempo).Count();

                            if (qtdeMaxEntregas >= agrupadosParcial[i].QtdeEntregas && modeloVeicularCarregaTipoCarga > 0)
                            {

                                //Validar se nenhum destinatario possui restrição ao modelo..
                                bool restricaoVeiculoAlgumDestinatario = false;
                                for (int ped = 0; ped < agrupadosParcial[i].Pedidos.Count; ped++)
                                {
                                    long cnpjDestinatario = (agrupadosParcial[i].Pedidos[ped].Recebedor == null ? agrupadosParcial[i].Pedidos[ped].Destinatario.Codigo : agrupadosParcial[i].Pedidos[ped].Recebedor.Codigo);
                                    if (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta)
                                        cnpjDestinatario = agrupadosParcial[i].Pedidos[ped].Remetente.Codigo;

                                    List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesRestritos = (from restr in modalidadeFornecedorPessoasRestricaoModeloVeicular
                                                                                                                                where restr.ModalidadeFornecedorPessoa.ModalidadePessoas.Cliente.Codigo == cnpjDestinatario
                                                                                                                                select restr.ModeloVeicular).ToList();

                                    if (modelosVeicularesRestritos?.Count == 0)
                                    {
                                        modelosVeicularesRestritos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                                        Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from centro in centroDescarregamentos
                                                                                                                              where (centro.Destinatario?.CPF_CNPJ ?? 0) == cnpjDestinatario
                                                                                                                              select centro).FirstOrDefault();

                                        //Se o destinatário possuir um centro de descarregamento.
                                        if (centroDescarregamento != null)
                                        {
                                            if (centroDescarregamento.VeiculosPermitidos?.Count > 0)
                                            {
                                                // Modelos veiculares da disponibilidade de frota do centro de carregamento
                                                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis = (from modelo in disponibilidadeDiaUtilizar
                                                                                                                                    select modelo.ModeloVeicular).Distinct().ToList();

                                                //Localizando os modelos do centro.. que não estão nos modelos permitidos do centro de descarga...
                                                modelosVeicularesRestritos = modelosDisponiveis.FindAll(x => !centroDescarregamento.VeiculosPermitidos.Any(d => d.Codigo == x.Codigo)).ToList();
                                            }
                                        }
                                    }

                                    restricaoVeiculoAlgumDestinatario = modelosVeicularesRestritos.Exists(e => e.Codigo == frota.ModeloVeicular.Codigo);
                                    if (restricaoVeiculoAlgumDestinatario)
                                        break;
                                }
                                if (!restricaoVeiculoAlgumDestinatario)
                                {
                                    agrupadosParcial[i].ModeloVeicular = frota.ModeloVeicular;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> ApiOrToolsResultado(GoogleOrTools.ApiResultado apiResult, bool agruparMesmoDestinatario, bool gerarCarregamentoDoisDias, bool gerarCarregamentosAlemDaDispFrota, List<DateTime> datasCarregamento, int indiceData, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosFechando, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupados, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();

            for (int i = 0; i < apiResult.result.Count; i++)
            {
                int codigoDisponibilidade = apiResult.result[i].veiculo.Codigo;
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota frota = (from d in disponibilidadeDiaUtilizar where d.Codigo == codigoDisponibilidade select d).FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPeso = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                Dictionary<int, decimal> pesos = new Dictionary<int, decimal>();
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto> pontosDeApoio = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto>();
                int ini_j = 0;
                int fim_j = apiResult.result[i].itens.Count;

                int qtdeEntregas = 0;
                for (int j = ini_j; j < fim_j; j++)
                {
                    GoogleOrTools.Local item = apiResult.result[i].itens[j].item;

                    if (item.TipoPonto == TipoPontoPassagem.Coleta)
                        continue;

                    //Localizando todos os pedidos do endereço.
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos_ender = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    if (agruparMesmoDestinatario)
                    {
                        pedidos_ender = pedidosFechando.FindAll(x => (x.Recebedor?.Codigo ?? 0) == item.Codigo);
                        if (pedidos_ender?.Count == 0)
                            pedidos_ender = pedidosFechando.FindAll(x => x.Destinatario.Codigo == item.Codigo && item.CodigoAux == (x.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0));
                    }
                    else if (item.CodigoAux > 0)
                    {
                        if (!pontosDeApoio.Exists(x => x.Codigo == item.CodigoAux))
                            pontosDeApoio.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto()
                            {
                                Codigo = (int)item.CodigoAux,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude
                            });

                        if (item.Codigo == 0)
                            pedidos_ender = pedidosFechando.FindAll(x => x.Destinatario?.PontoDeApoio?.Codigo == item.CodigoAux);
                        else
                            pedidos_ender.Add(pedidosFechando.Find(x => x.Codigo == item.Codigo));
                    }
                    else
                        pedidos_ender.Add(pedidosFechando.Find(x => x.Codigo == item.Codigo));

                    qtdeEntregas += 1;
                    pedidosPeso.AddRange(pedidos_ender);
                    for (int p = 0; p < pedidos_ender.Count; p++)
                    {
                        pesos.Add(pedidos_ender[p].Codigo, pedidos_ender[p].PesoTotal);

                        for (int pp = 0; pp < pedidos_ender[p].Produtos?.Count; pp++)
                        {
                            produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto()
                            {
                                CodigoPedido = pedidos_ender[p].Codigo,
                                CodigoPedidoProduto = pedidos_ender[p].Produtos[pp].Codigo,
                                PesoPedidoProduto = pedidos_ender[p].Produtos[pp].PesoTotal,
                                QuantidadePedidoProduto = pedidos_ender[p].Produtos[pp].Quantidade,
                                QuantidadePalletPedidoProduto = pedidos_ender[p].Produtos[pp].QuantidadePalet,
                                MetroCubicoPedidoProduto = pedidos_ender[p].Produtos[pp].MetroCubico,
                                CodigoLinhaSeparacao = pedidos_ender[p].Produtos[pp]?.LinhaSeparacao?.Codigo ?? 0
                            });
                        }
                    }
                }

                DateTime dataPrevistaCarregamentoGrupo = (from p in pedidosPeso
                                                          select p.DataCarregamentoPedido)?.Min() ?? DateTime.Now;

                if (gerarCarregamentoDoisDias && datasCarregamento.Count == 2)
                    dataPrevistaCarregamentoGrupo = datasCarregamento[indiceData];
                else if (!gerarCarregamentoDoisDias && dataPrevistaCarregamentoGrupo.Date < DateTime.Now.Date)
                    dataPrevistaCarregamentoGrupo = DateTime.Now;

                if (pedidosPeso?.Count > 0)
                {
                    List<Dominio.Entidades.Empresa> transportadoresDistintos = (from o in pedidosPeso
                                                                                select o.Empresa).Distinct().ToList();

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido carregamentoGrupoPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido
                    {
                        Pedidos = pedidosPeso,
                        ModeloVeicular = frota.ModeloVeicular,
                        CodigoFilial = (from ped in pedidosPeso select ped.Filial?.Codigo ?? 0).FirstOrDefault(),
                        Transportador = ((transportadoresDistintos?.Count ?? 0) != 1 ? null : transportadoresDistintos.FirstOrDefault()),
                        DataCarregamento = dataPrevistaCarregamentoGrupo.Date,
                        PedidosPesos = pesos,
                        Produtos = produtos,
                        PontosDeApoio = pontosDeApoio,
                        QtdeEntregas = qtdeEntregas
                    };

                    //Se a configuração do centro de carregamento não permite gerar alem da disponibilidade de frota.
                    //if (totalDisponibilidadeFrota <= agrupados?.Count && !gerarCarregamentosAlemDaDispFrota)
                    if (disponibilidadeDiaUtilizar.Sum(x => x.Quantidade) <= (resultado.Count + agrupados?.Count) && !gerarCarregamentosAlemDaDispFrota)
                        break;

                    resultado.Add(carregamentoGrupoPedido);
                }
            }

            // Procedimento que verifica a quantidade minima de pedidos contemplados no agrupamento de pedidos na mesma carga.
            if ((resultado?.Count ?? 0) > 0)
            {
                for (int i = resultado.Count - 1; i >= 0; i--)
                {
                    var tipoCarga = (from obj in resultado[i].Pedidos where obj.TipoDeCarga != null select obj.TipoDeCarga).FirstOrDefault();
                    if (tipoCarga == null)
                        continue;

                    var limiteModelo = (from obj in sessaoRoteirizadorParametros.TemposCarregamento
                                        where obj.CodigoTipoCarga == tipoCarga.Codigo && obj.CodigoModeloVeicular == resultado[i].ModeloVeicular.Codigo
                                        select obj).FirstOrDefault();

                    if (limiteModelo == null)
                        continue;

                    if (limiteModelo.QuantidadeMinimaUtilizar == 0)
                        continue;

                    if (limiteModelo.QuantidadeMinimaUtilizar > resultado[i].Pedidos.Count)
                    {
                        this.SetarSessaoRoteirizadorPedidosResultado(resultado[i].Pedidos, SituacaoSessaoRoteirizadorPedido.LimitePorTipoCargaModeloNaoAtingido, ref sessaoRoteirizadorPedidosResultado);
                        resultado.RemoveAt(i);

                    }
                }
            }
            return resultado;
        }

        private void AtualizarPontoDeApoioDestinatarios(ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao config = this.ObterConfiguracaoIntegracao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = this.ObterConfiguracaoEmbarcador();

            //Vamos validar a geocodigicação dos endereços
            if (configuracaoTMS.RaioMaximoGeoLocalidadeGeoCliente > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pessoas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    double latitude_dest = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta ? ObterLatitudeOuLongitude(pedido.Remetente.Latitude) : ObterLatitudeOuLongitude(pedido.Destinatario.Latitude));
                    double longitude_dest = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta ? ObterLatitudeOuLongitude(pedido.Remetente.Longitude) : ObterLatitudeOuLongitude(pedido.Destinatario.Longitude));

                    Dominio.Entidades.Localidade localidade = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta ? pedido.Remetente?.Localidade : pedido.Destinatario?.Localidade);

                    if ((localidade?.Latitude.HasValue ?? false) && (localidade?.Latitude.HasValue ?? false))
                    {
                        double latLocalidade = (double)(localidade?.Latitude.Value ?? 0);
                        double lngLocalidade = (double)(localidade?.Longitude.Value ?? 0);
                        double distancia = Logistica.Polilinha.CalcularDistancia(latLocalidade, lngLocalidade, latitude_dest, longitude_dest) / 1000;
                        // Se a distãncia for superior ao raio máximo.. vamos geocodificar.. utilizando o Nominatim
                        if (distancia > configuracaoTMS.RaioMaximoGeoLocalidadeGeoCliente)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto tipoPonto = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();
                            tipoPonto.Cliente = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta ? pedido.Remetente : pedido.Destinatario);
                            pessoas.Add(tipoPonto);
                        }
                    }

                }

                if (pessoas?.Count > 0)
                    Servicos.Embarcador.Carga.CargaRotaFrete.AtualizarCoordenadas(pessoas, _unitOfWork, config, true, configuracaoTMS);

            }

            bool existe = pedidos.Exists(x => x.Destinatario?.AtualizarPontoApoioMaisProximoAutomaticamente ?? false);
            if (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta)
                existe = pedidos.Exists(x => x.Remetente?.AtualizarPontoApoioMaisProximoAutomaticamente ?? false);

            if (existe)
            {
                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(config.ServidorRouteOSM);

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

                List<int> filiais = pedidos.Where(o => o.Filial != null).Select(o => o.Filial.Codigo).Distinct().ToList();

                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Locais> pontosDeApoio = repositorio.BuscarPorTipoDeLocalEFiliais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.PontoDeApoio, filiais);

                if (pontosDeApoio?.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.Entidades.Cliente cliente = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Coleta ? pedido.Remetente : pedido.Destinatario);
                        if (cliente?.AtualizarPontoApoioMaisProximoAutomaticamente ?? false == true)
                        {
                            double latitude_dest = ObterLatitudeOuLongitude(cliente.Latitude);
                            double longitude_dest = ObterLatitudeOuLongitude(cliente.Longitude);

                            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                            {
                                Lat = latitude_dest,
                                Lng = longitude_dest,
                                Descricao = cliente.Nome,
                                Codigo = cliente.Codigo,
                                TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
                            };

                            Dominio.Entidades.Embarcador.Logistica.Locais melhor = null;
                            decimal menor = 0;

                            foreach (Dominio.Entidades.Embarcador.Logistica.Locais local in pontosDeApoio)
                            {
                                var areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(local.Area);
                                double latitude = areas[0].position.lat;
                                double longitude = areas[0].position.lng;

                                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint destino = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                                {
                                    Lat = latitude,
                                    Lng = longitude,
                                    Descricao = local.Descricao,
                                    Codigo = local.Codigo,
                                    TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
                                };

                                try
                                {
                                    rota.Add(origem);
                                    rota.Add(destino);
                                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao percurso = rota.Roteirizar();
                                    if (melhor == null || (percurso?.Distancia ?? 0) < menor)
                                    {
                                        melhor = local;
                                        menor = percurso.Distancia;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro(ex);
                                }
                                finally
                                {
                                    rota.Clear();
                                }
                            }

                            //Atualizando o cliente com o ponto de apoio mais próximo...
                            if (melhor != null)
                            {
                                cliente.PontoDeApoio = melhor;
                                repositorioCliente.Atualizar(cliente);
                                melhor = null;
                                menor = 0;
                            }
                        }
                    }
                }
            }
        }

        private void AtualizarQuantidadesPedidoProdutoAtendido(int codigoPedidoProduto, ref decimal totalCarregadoProduto, ref decimal totalCarregadoProdutoPallet, ref decimal qtdeCarregadoProduto, ref decimal totalCarregadoProdutoM3)
        {
            //Vamos verificar se já foi consultado a quantidade de produtos carregados... 
            // Caso não foi pesquisado ou não está na lista.. vamos consutlar em outros carregamentos...
            var cppa = _carregamentoPedidoProdutosAtendidos.FindAll(x => x.PedidoProduto.Codigo == codigoPedidoProduto);

            totalCarregadoProduto += cppa.Sum(x => x.Peso);
            totalCarregadoProdutoPallet += cppa.Sum(x => x.QuantidadePallet);
            qtdeCarregadoProduto += cppa.Sum(x => x.Quantidade);
            totalCarregadoProdutoM3 += cppa.Sum(x => x.MetroCubico);
        }

        private void AtualizarSessaoRoteirizador(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            if (sessaoRoteirizador?.Codigo > 0)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(_unitOfWork);
                repositorioSessaoRoteirizador.Atualizar(sessaoRoteirizador);
            }
            else
                sessaoRoteirizador = null;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento GerarCarregamento(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido grupoPedido, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipoSeparacao, int numeroCarregamento)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = grupoPedido.TipoDeCarga;
            if (tipoDeCarga == null)
                tipoDeCarga = this.ObterTipoDeCargaPedidos(grupoPedido.Pedidos);

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            DateTime? data_descarregamento = grupoPedido.Pedidos?.Max(x => x.DataPrevisaoChegadaDestinatario) ?? null;
            decimal pesoTotalCarregamento = grupoPedido.PedidosPesos.Sum(p => p.Value);

            List<int> pedidosDesconsideraPeso = (from obj in grupoPedido.Pedidos where (obj?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false) == true select obj.Codigo).ToList();
            decimal pesoDesconsidera = (from p in grupoPedido.PedidosPesos
                                        where pedidosDesconsideraPeso.Contains(p.Key)
                                        select p.Value).Sum();

            List<Dominio.Entidades.Cliente> expedidoresDistintos = (from o in grupoPedido.Pedidos
                                                                    where o.Expedidor != null
                                                                    select o.Expedidor).Distinct().ToList();

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento
            {
                DataCriacao = DateTime.Now,
                DataCarregamentoCarga = grupoPedido.DataCarregamento,
                DataDescarregamentoCarga = data_descarregamento,
                SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem,
                TipoMontagemCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga,
                AutoSequenciaNumero = (numeroCarregamento > 0 ? numeroCarregamento : new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork).ObterProximoCodigoCarregamento()),
                TipoOperacao = grupoPedido.TipoOperacao ?? grupoPedido.Pedidos.FirstOrDefault().TipoOperacao,
                TipoDeCarga = tipoDeCarga,
                ModeloVeicularCarga = grupoPedido.ModeloVeicular,
                Empresa = grupoPedido.Transportador,
                SessaoRoteirizador = sessaoRoteirizador,
                TipoSeparacao = tipoSeparacao,
                PesoCarregamento = pesoTotalCarregamento - pesoDesconsidera,  //pedidos.Sum(o => o.PesoTotal)
                ValorFrete = grupoPedido.ValorFreteVencedor,
                ExigeIsca = grupoPedido.ExigeIsca,
                MontagemCarregamentoBloco = grupoPedido.MontagemCarregamentoBloco
            };

            if (expedidoresDistintos?.Count == 1)
                carregamento.Expedidor = expedidoresDistintos.FirstOrDefault();

            if (IsDataCarregamentoNull(centrosCarregamento, grupoPedido.Pedidos.FirstOrDefault()))
                carregamento.DataCarregamentoCarga = null;

            carregamento.DataInicioViagemPrevista = DataInicioViagemPrevistaCarregamento(centrosCarregamento, carregamento.DataCarregamentoCarga);

            carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();

            repCarregamento.Inserir(carregamento);

            SalvarPedidos(carregamento, grupoPedido.Pedidos, grupoPedido.Produtos, sessaoRoteirizador.MontagemCarregamentoPedidoProduto);

            return carregamento;
        }

        private bool IsDataCarregamentoNull(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase)
        {
            if (pedidoBase == null || pedidoBase.Filial == null)
                return false;

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (from centro in centrosCarregamento where (centro?.Filial?.Codigo ?? 0) == pedidoBase.Filial.Codigo select centro).FirstOrDefault();

            if (centroCarregamento == null)
                return false;

            return centroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas;
        }

        private DateTime? DataInicioViagemPrevistaCarregamento(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, DateTime? dataCarregamento)
        {
            if (!dataCarregamento.HasValue)
                return dataCarregamento;

            TimeSpan? previsto = (from obj in centrosCarregamento select obj.HoraInicioViagemPrevista).FirstOrDefault();

            if (!previsto.HasValue)
                return null;

            return new DateTime(dataCarregamento.Value.Year, dataCarregamento.Value.Month, dataCarregamento.Value.Day, previsto.Value.Hours, previsto.Value.Minutes, 0);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> ObterCarregamentosSessao(int codigoSessaoRoteirizador)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            _carregamentosSessao = repositorioCarregamento.Consultar(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
            {
                CodigoSessaoRoteirizador = codigoSessaoRoteirizador,
                ProgramaComSessaoRoteirizador = true
            }, null);
            return _carregamentosSessao;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            if (_configuracaoMontagemCarga == null)
                _configuracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao == null)
                _configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork).Buscar();

            return _configuracaoIntegracao;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ObterPedidosProdutos(List<int> pedidos, int codigoSessaoRoteirizador)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> lista = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            if (_pedidosProdutos == null)
            {
                _pedidosProdutos = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork).BuscarPorPedidos(pedidos);
                if (codigoSessaoRoteirizador > 0)
                {
                    List<int> codigosPedidosProdutos = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(_unitOfWork).BuscarCodigosPedidoProdutoPorSessaoRoteirizador(codigoSessaoRoteirizador);
                    if (codigosPedidosProdutos.Count > 0)
                        _pedidosProdutos = (from obj in _pedidosProdutos where codigosPedidosProdutos.Contains(obj.Codigo) select obj).ToList();
                }
            }

            lista = (from produto in _pedidosProdutos
                     join pedido in pedidos on produto.Pedido.Codigo equals pedido
                     select produto).ToList();

            return lista;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> ObterPedidosProdutosCarregamentos(List<int> codigosPdidosProdutos)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
            _carregamentoPedidoProdutosAtendidos = repositorioCarregamentoPedidoProduto.BuscarPorPedidoProdutos(codigosPdidosProdutos);
            return _carregamentoPedidoProdutosAtendidos;
        }

        /// <summary>
        /// Função para retornar a prioridade de carregamento de produtos de pedidos de uma lista de pedidos.
        /// A lista será ordenada por prioridade de atendimento do Canal de entrega do pedido e Linha de separação do pedido produto.
        /// </summary>
        /// <param name="pedidos">Lista d epedidos.</param>
        /// <param name="sessaoRoteirizador">Sessão do roteirizador</param>
        /// <returns></returns>
        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> ObterFilaPrioridadesPedidosProdutos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, ref bool resumida)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = this.ObterPedidosProdutos((from ped in pedidos select ped.Codigo).ToList(), sessaoRoteirizador.Codigo);

            resumida = (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto ||
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaProduto ||
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido ||
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> filaCarregamentoPedidosCanalEntregaLinhaSeparacao = null;
            if (!resumida)
                filaCarregamentoPedidosCanalEntregaLinhaSeparacao = produtos.GroupBy(x => new
                {
                    CodigoCanalEntrega = x.Pedido?.CanalEntrega?.Codigo ?? 0,
                    PrioridadeCanalEntrega = ((x.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : x.Pedido?.CanalEntrega?.NivelPrioridade),
                    CodigoLinhaSeparacao = x.LinhaSeparacao?.Codigo ?? 0,
                    PrioridadeLinhaSeparacao = ((x.LinhaSeparacao?.NivelPrioridade ?? 0) == 0 ? 999 : x.LinhaSeparacao?.NivelPrioridade),
                    CodigoEnderecoProduto = x.EnderecoProduto?.Codigo ?? 0,
                    PrioridadeEnderecoProduto = ((x.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : x.EnderecoProduto?.NivelPrioridade),
                    CodigoPedido = x.Pedido.Codigo,
                    DataPedido = x.Pedido.DataCriacao
                })
                                                                                       .Select(g => new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento()
                                                                                       {
                                                                                           CodigoCanalEntrega = g.Key.CodigoCanalEntrega,
                                                                                           PrioridadeCanalEntrega = g.Key.PrioridadeCanalEntrega ?? 999,
                                                                                           CodigoLinhaSeparacao = g.Key.CodigoLinhaSeparacao,
                                                                                           PrioridadeLinhaSeparacao = g.Key.PrioridadeLinhaSeparacao ?? 999,
                                                                                           CodigoEnderecoProduto = g.Key.CodigoEnderecoProduto,
                                                                                           PrioridadeEnderecoProduto = g.Key.PrioridadeEnderecoProduto ?? 999,
                                                                                           CodigoPedido = g.Key.CodigoPedido,
                                                                                           DataPedido = g.Key.DataPedido ?? DateTime.Now.Date,
                                                                                           PesoTotal = g.Sum(s => s.PesoTotal),
                                                                                           Pallet = g.Sum(s => s.QuantidadePalet),
                                                                                           Metro = g.Sum(s => s.MetroCubico)
                                                                                       }).ToList();
            else
            {
                if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto)
                    filaCarregamentoPedidosCanalEntregaLinhaSeparacao = produtos.GroupBy(x => new
                    {
                        CodigoCanalEntrega = x.Pedido?.CanalEntrega?.Codigo ?? 0,
                        PrioridadeCanalEntrega = ((x.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : x.Pedido?.CanalEntrega?.NivelPrioridade),
                        CodigoLinhaSeparacao = 0,
                        PrioridadeLinhaSeparacao = 999
                    })
                                                                                               .Select(g => new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento()
                                                                                               {
                                                                                                   CodigoCanalEntrega = g.Key.CodigoCanalEntrega,
                                                                                                   PrioridadeCanalEntrega = g.Key.PrioridadeCanalEntrega ?? 999,
                                                                                                   CodigoLinhaSeparacao = g.Key.CodigoLinhaSeparacao,
                                                                                                   PrioridadeLinhaSeparacao = g.Key.PrioridadeLinhaSeparacao,
                                                                                                   PesoTotal = g.Sum(s => s.PesoTotal),
                                                                                                   Pallet = g.Sum(s => s.QuantidadePalet),
                                                                                                   Metro = g.Sum(s => s.MetroCubico)
                                                                                               }).ToList();
                else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaProduto)
                    filaCarregamentoPedidosCanalEntregaLinhaSeparacao = produtos.GroupBy(x => new
                    {
                        CodigoCanalEntrega = 0,
                        PrioridadeCanalEntrega = 999,
                        CodigoLinhaSeparacao = x.LinhaSeparacao?.Codigo ?? 0,
                        PrioridadeLinhaSeparacao = ((x.LinhaSeparacao?.NivelPrioridade ?? 0) == 0 ? 999 : x.LinhaSeparacao?.NivelPrioridade),
                    })
                                                                                               .Select(g => new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento()
                                                                                               {
                                                                                                   CodigoCanalEntrega = g.Key.CodigoCanalEntrega,
                                                                                                   PrioridadeCanalEntrega = g.Key.PrioridadeCanalEntrega,
                                                                                                   CodigoLinhaSeparacao = g.Key.CodigoLinhaSeparacao,
                                                                                                   PrioridadeLinhaSeparacao = g.Key.PrioridadeLinhaSeparacao ?? 999,
                                                                                                   PesoTotal = g.Sum(s => s.PesoTotal),
                                                                                                   Pallet = g.Sum(s => s.QuantidadePalet),
                                                                                                   Metro = g.Sum(s => s.MetroCubico)
                                                                                               }).ToList();
                else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido)
                    filaCarregamentoPedidosCanalEntregaLinhaSeparacao = produtos.GroupBy(x => new
                    {
                        CodigoCanalEntrega = 0,
                        PrioridadeCanalEntrega = 999,
                        CodigoLinhaSeparacao = 0,
                        PrioridadeLinhaSeparacao = 999,
                        CodigoEnderecoProduto = x.EnderecoProduto?.Codigo ?? 0,
                        EnderecoProduto = x.EnderecoProduto?.Descricao ?? String.Empty,
                        PrioridadeEnderecoProduto = ((x.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : x.EnderecoProduto?.NivelPrioridade),
                        DataPedido = x.Pedido.DataCriacao
                    })
                                                                                               .Select(g => new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento()
                                                                                               {
                                                                                                   CodigoCanalEntrega = g.Key.CodigoCanalEntrega,
                                                                                                   PrioridadeCanalEntrega = g.Key.PrioridadeCanalEntrega,
                                                                                                   CodigoLinhaSeparacao = g.Key.CodigoLinhaSeparacao,
                                                                                                   PrioridadeLinhaSeparacao = g.Key.PrioridadeLinhaSeparacao,
                                                                                                   CodigoEnderecoProduto = g.Key.CodigoEnderecoProduto,
                                                                                                   EnderecoProduto = g.Key.EnderecoProduto,
                                                                                                   PrioridadeEnderecoProduto = g.Key.PrioridadeEnderecoProduto ?? 999,
                                                                                                   DataPedido = g.Key.DataPedido ?? DateTime.Now.Date,
                                                                                                   PesoTotal = g.Sum(s => s.PesoTotal),
                                                                                                   Pallet = g.Sum(s => s.QuantidadePalet),
                                                                                                   Metro = g.Sum(s => s.MetroCubico)
                                                                                               }).ToList();
                else
                    filaCarregamentoPedidosCanalEntregaLinhaSeparacao = produtos.GroupBy(x => new
                    {
                        CodigoCanalEntrega = x.Pedido?.CanalEntrega?.Codigo ?? 0,
                        PrioridadeCanalEntrega = ((x.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : x.Pedido?.CanalEntrega?.NivelPrioridade),
                        CodigoLinhaSeparacao = 0,
                        PrioridadeLinhaSeparacao = 999,
                        CodigoEnderecoProduto = x.EnderecoProduto?.Codigo ?? 0,
                        EnderecoProduto = x.EnderecoProduto?.Descricao ?? string.Empty,
                        PrioridadeEnderecoProduto = ((x.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : x.EnderecoProduto?.NivelPrioridade),
                        DataPedido = x.Pedido.DataCriacao
                    })
                                                                                               .Select(g => new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento()
                                                                                               {
                                                                                                   CodigoCanalEntrega = g.Key.CodigoCanalEntrega,
                                                                                                   PrioridadeCanalEntrega = g.Key.PrioridadeCanalEntrega ?? 999,
                                                                                                   CodigoLinhaSeparacao = g.Key.CodigoLinhaSeparacao,
                                                                                                   PrioridadeLinhaSeparacao = g.Key.PrioridadeLinhaSeparacao,
                                                                                                   CodigoEnderecoProduto = g.Key.CodigoEnderecoProduto,
                                                                                                   EnderecoProduto = g.Key.EnderecoProduto,
                                                                                                   PrioridadeEnderecoProduto = g.Key.PrioridadeEnderecoProduto ?? 999,
                                                                                                   DataPedido = g.Key.DataPedido ?? DateTime.Now.Date,
                                                                                                   PesoTotal = g.Sum(s => s.PesoTotal),
                                                                                                   Pallet = g.Sum(s => s.QuantidadePalet),
                                                                                                   Metro = g.Sum(s => s.MetroCubico)
                                                                                               }).ToList();
            }

            //Ordenando a fila de carregamento conforme definição com Sidnei, que devemos Seguir a ordem do canal de entrega e depois a ordem da linha de separacao...
            if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido)
                filaCarregamentoPedidosCanalEntregaLinhaSeparacao = filaCarregamentoPedidosCanalEntregaLinhaSeparacao.OrderBy(x => x.PrioridadeCanalEntrega)
                                                                                                                     .ThenBy(x => x.PrioridadeLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoCanalEntrega)
                                                                                                                     .ThenBy(x => x.CodigoLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoPedido)
                                                                                                                     .ThenByDescending(x => x.PesoTotal)
                                                                                                                     .ToList();
            else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido)
                filaCarregamentoPedidosCanalEntregaLinhaSeparacao = filaCarregamentoPedidosCanalEntregaLinhaSeparacao.OrderBy(x => x.PrioridadeEnderecoProduto)
                                                                                                                     .ThenBy(x => x.EnderecoProduto)
                                                                                                                     .ThenBy(x => x.DataPedido)
                                                                                                                     .ThenBy(x => x.PrioridadeCanalEntrega)
                                                                                                                     .ThenBy(x => x.PrioridadeLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoEnderecoProduto)
                                                                                                                     .ThenBy(x => x.CodigoCanalEntrega)
                                                                                                                     .ThenBy(x => x.CodigoLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoPedido)
                                                                                                                     .ThenByDescending(x => x.PesoTotal)
                                                                                                                     .ToList();
            else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido)
                filaCarregamentoPedidosCanalEntregaLinhaSeparacao = filaCarregamentoPedidosCanalEntregaLinhaSeparacao.OrderBy(x => x.PrioridadeCanalEntrega)
                                                                                                                     .ThenBy(x => x.PrioridadeEnderecoProduto)
                                                                                                                     .ThenBy(x => x.EnderecoProduto)
                                                                                                                     .ThenBy(x => x.DataPedido)
                                                                                                                     .ThenBy(x => x.PrioridadeLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoCanalEntrega)
                                                                                                                     .ThenBy(x => x.CodigoEnderecoProduto)
                                                                                                                     .ThenBy(x => x.CodigoLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoPedido)
                                                                                                                     .ThenByDescending(x => x.PesoTotal)
                                                                                                                     .ToList();
            else
                filaCarregamentoPedidosCanalEntregaLinhaSeparacao = filaCarregamentoPedidosCanalEntregaLinhaSeparacao.OrderBy(x => x.PrioridadeLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.PrioridadeCanalEntrega)
                                                                                                                     .ThenBy(x => x.CodigoLinhaSeparacao)
                                                                                                                     .ThenBy(x => x.CodigoCanalEntrega)
                                                                                                                     .ThenBy(x => x.CodigoPedido)
                                                                                                                     .ThenByDescending(x => x.PesoTotal)
                                                                                                                     .ToList();

            return filaCarregamentoPedidosCanalEntregaLinhaSeparacao;
        }

        /// <summary>
        /// Método par aobter a lista de produtos de acordo com a fila de prioridade...
        /// </summary>
        /// <param name="pedidos">Lista de pedidos da vez....</param>
        /// <param name="prioridade">Prioridade da fila....</param>
        /// <param name="prioridadeMontagemCarregamento">Opção de prioridade selecionada pelo usuário.</param>
        /// <returns></returns>
        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ObterProdutosDeAcordoFilaPrioridade(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento prioridade, PrioridadeMontagemCarregamentoPedidoProduto prioridadeMontagemCarregamento)
        {
            //Localizando todos os produtos dos pedidos envolvidos...
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = this.ObterPedidosProdutos((from ped in pedidos select ped.Codigo).ToList(), _codigoSessaoRoteirizador);

            //Filtrando apenas os produtos de acordo com a fila de prioridade
            if (prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido || prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoPedidoCanalEntrega)
            {

                produtos = (from obj in produtos
                            where ((obj.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : obj.Pedido?.CanalEntrega?.NivelPrioridade) == prioridade.PrioridadeCanalEntrega &&
                                    (obj.Pedido?.CanalEntrega?.Codigo ?? 0) == prioridade.CodigoCanalEntrega &&
                                    (obj.LinhaSeparacao?.Codigo ?? 0) == prioridade.CodigoLinhaSeparacao &&
                                    ((obj.LinhaSeparacao?.NivelPrioridade ?? 0) == 0 ? 999 : obj.LinhaSeparacao?.NivelPrioridade) == prioridade.PrioridadeLinhaSeparacao &&
                                    (obj.EnderecoProduto?.Codigo ?? 0) == prioridade.CodigoEnderecoProduto &&
                                    ((obj.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : obj.EnderecoProduto?.NivelPrioridade) == prioridade.PrioridadeEnderecoProduto &&
                                    obj.Pedido.Codigo == prioridade.CodigoPedido &&
                                    obj.Pedido.DataCriacao == prioridade.DataPedido
                            select obj).ToList();

            }
            else if (prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto)
            {

                produtos = (from obj in produtos
                            where ((obj.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : obj.Pedido?.CanalEntrega?.NivelPrioridade) == prioridade.PrioridadeCanalEntrega &&
                                    (obj.Pedido?.CanalEntrega?.Codigo ?? 0) == prioridade.CodigoCanalEntrega
                            select obj).ToList();

            }
            else if (prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaProduto)
            {

                produtos = (from obj in produtos
                            where (obj.LinhaSeparacao?.Codigo ?? 0) == prioridade.CodigoLinhaSeparacao &&
                                    ((obj.LinhaSeparacao?.NivelPrioridade ?? 0) == 0 ? 999 : obj.LinhaSeparacao?.NivelPrioridade) == prioridade.PrioridadeLinhaSeparacao
                            select obj).ToList();

            }
            else if (prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido)
            {
                produtos = (from obj in produtos
                            where (obj.EnderecoProduto?.Codigo ?? 0) == prioridade.CodigoEnderecoProduto &&
                                    ((obj.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : obj.EnderecoProduto?.NivelPrioridade) == prioridade.PrioridadeEnderecoProduto &&
                                    obj.Pedido.DataCriacao == prioridade.DataPedido
                            select obj).ToList();

            }
            else if (prioridadeMontagemCarregamento == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido)
            {
                produtos = (from obj in produtos
                            where ((obj.Pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : obj.Pedido?.CanalEntrega?.NivelPrioridade) == prioridade.PrioridadeCanalEntrega &&
                                    (obj.Pedido?.CanalEntrega?.Codigo ?? 0) == prioridade.CodigoCanalEntrega &&
                                    (obj.EnderecoProduto?.Codigo ?? 0) == prioridade.CodigoEnderecoProduto &&
                                    ((obj.EnderecoProduto?.NivelPrioridade ?? 0) == 0 ? 999 : obj.EnderecoProduto?.NivelPrioridade) == prioridade.PrioridadeEnderecoProduto &&
                                    obj.Pedido.DataCriacao == prioridade.DataPedido
                            select obj).ToList();
            }

            return produtos;
        }

        private void ObterProdutosCarregarDeAcordoFilaPrioridades(List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> filaCarregamentoPedidosCanalEntregaLinhaSeparacao, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAnalisar, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> montagemCargaGrupoPedidoProdutos, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaParametros montagemCargaParametros, ref decimal peso, ref decimal pallet, ref decimal cubagem, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> retornoPedidos, ref Dictionary<int, decimal> pedidosPesosCarregamento)
        {
            //Agora vamos carregar..
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento item in filaCarregamentoPedidosCanalEntregaLinhaSeparacao)
            {
                //Linhas de separação contidas no carregamento atual...
                List<int> linhasSeparacaoNoCarregamento = (from x in montagemCargaGrupoPedidoProdutos
                                                           select x.CodigoLinhaSeparacao).Distinct().ToList();
                if (!montagemCargaParametros.filaMontagemPedidoProdutoResumida)
                {
                    //Obtendo o pedido da fila de carregamento
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidosAnalisar.Find(x => x.Codigo == item.CodigoPedido);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutosFilaPrioridade = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                    if (!montagemCargaParametros.filaMontagemPedidoProdutoResumida)
                        pedidosProdutosFilaPrioridade = ObterProdutosDeAcordoFilaPrioridade(new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, item, montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto);

                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDesteGrupo = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                    if (pedidosProdutosFilaPrioridade.Count == 0)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDestePedidoDesteGrupo = (from obj in listaProdutosDesteGrupo where obj.CodigoPedido == pedido.Codigo select obj).ToList();
                        //Obtem os produtos passiveis de carregamento e suas quantiadades respecitvas,...
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidoProdutos = ObterPedidoProdutosPorPeso(pedido, null, peso, pallet, cubagem, montagemCargaParametros, linhasSeparacaoNoCarregamento, ref sessaoRoteirizadorPedidosResultado, item.CodigoLinhaSeparacao, listaProdutosDestePedidoDesteGrupo);
                        if (pedidoProdutos?.Count > 0)
                        {
                            decimal pesoCarregar = pedidoProdutos.Sum(x => x.PesoPedidoProduto);
                            decimal palletCarregar = pedidoProdutos.Sum(x => x.QuantidadePalletPedidoProduto);
                            decimal cubagemCarregar = pedidoProdutos.Sum(x => x.MetroCubicoPedidoProduto);
                            peso += pesoCarregar;
                            pallet += palletCarregar;
                            cubagem += cubagemCarregar;
                            this.AdicionarPesoCarregadoDicionario(pedido, pesoCarregar, ref retornoPedidos, ref pedidosPesosCarregamento);
                            montagemCargaGrupoPedidoProdutos.AddRange(pedidoProdutos);
                            listaProdutosDesteGrupo.AddRange(pedidoProdutos);
                        }

                        if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                            break;
                        if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                            break;
                        if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                            break;
                    }
                    else
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidosProdutosFilaPrioridade)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDestePedidoDesteGrupo = (from obj in listaProdutosDesteGrupo where obj.CodigoPedido == pedidoProduto.Pedido.Codigo select obj).ToList();
                            //Obtem os produtos passiveis de carregamento e suas quantiadades respecitvas,...
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidoProdutos = ObterPedidoProdutosPorPeso(pedido, pedidoProduto, peso, pallet, cubagem, montagemCargaParametros, linhasSeparacaoNoCarregamento, ref sessaoRoteirizadorPedidosResultado, item.CodigoLinhaSeparacao, listaProdutosDestePedidoDesteGrupo);
                            if (pedidoProdutos?.Count > 0)
                            {
                                decimal pesoCarregar = pedidoProdutos.Sum(x => x.PesoPedidoProduto);
                                decimal palletCarregar = pedidoProdutos.Sum(x => x.QuantidadePalletPedidoProduto);
                                decimal cubagemCarregar = pedidoProdutos.Sum(x => x.MetroCubicoPedidoProduto);
                                peso += pesoCarregar;
                                pallet += palletCarregar;
                                cubagem += cubagemCarregar;
                                this.AdicionarPesoCarregadoDicionario(pedido, pesoCarregar, ref retornoPedidos, ref pedidosPesosCarregamento);
                                montagemCargaGrupoPedidoProdutos.AddRange(pedidoProdutos);
                                listaProdutosDesteGrupo.AddRange(pedidoProdutos);
                            }

                            if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                                break;
                            if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                                break;
                            if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                                break;
                        }
                    }
                }
                else
                {
                    //Obtendo todos os pedidos do canal de entrega....
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDaVez = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    //Consultado todos os produtos dos pedidos do canal de entrega.
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                    if (montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido)
                    {
                        pedidosDaVez = (from ped in pedidosAnalisar where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        //Consultado todos os produtos dos pedidos do canal de entrega.
                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDaVez, item, montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.EnderecoProduto?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.EnderecoProduto?.Codigo ?? 0)
                                           .ThenBy(x => x.Pedido.DataCriacao).ToList();
                    }
                    else if (montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido)
                    {
                        pedidosDaVez = (from ped in pedidosAnalisar where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        //Consultado todos os produtos dos pedidos do canal de entrega.
                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDaVez, item, montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.Pedido?.CanalEntrega?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.Pedido?.CanalEntrega?.CodigoIntegracao ?? "")
                                           .ThenBy(x => x.EnderecoProduto?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.EnderecoProduto?.Codigo ?? 0)
                                           .ThenBy(x => x.Pedido.DataCriacao).ToList();
                    }
                    else if (montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto)
                    {
                        pedidosDaVez = (from ped in pedidosAnalisar where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        //Consultado todos os produtos dos pedidos do canal de entrega.
                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDaVez, item, montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.LinhaSeparacao?.NivelPrioridade ?? 999).ThenBy(x => x.Produto.Codigo).ToList();
                    }
                    else
                    {
                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosAnalisar, item, montagemCargaParametros.prioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.Pedido?.CanalEntrega?.NivelPrioridade ?? 999).ThenBy(x => x.Pedido?.CanalEntrega?.Codigo ?? 999).ToList();
                    }

                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDesteGrupo = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in produtos)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDestePedidoDesteGrupo = (from obj in listaProdutosDesteGrupo where obj.CodigoPedido == produto.Pedido.Codigo select obj).ToList();

                        //Obtem os produtos passiveis de carregamento e suas quantiadades respecitvas,...
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidoProdutos = ObterPedidoProdutosPorPeso(produto.Pedido, produto, peso, pallet, cubagem, montagemCargaParametros, linhasSeparacaoNoCarregamento, ref sessaoRoteirizadorPedidosResultado, item.CodigoLinhaSeparacao, listaProdutosDestePedidoDesteGrupo);
                        if (pedidoProdutos?.Count > 0)
                        {
                            decimal pesoCarregar = pedidoProdutos.Sum(x => x.PesoPedidoProduto);
                            decimal palletCarregar = pedidoProdutos.Sum(x => x.QuantidadePalletPedidoProduto);
                            decimal cubagemCarregar = pedidoProdutos.Sum(x => x.MetroCubicoPedidoProduto);
                            peso += pesoCarregar;
                            pallet += palletCarregar;
                            cubagem += cubagemCarregar;
                            this.AdicionarPesoCarregadoDicionario(produto.Pedido, pesoCarregar, ref retornoPedidos, ref pedidosPesosCarregamento);
                            montagemCargaGrupoPedidoProdutos.AddRange(pedidoProdutos);
                            listaProdutosDesteGrupo.AddRange(pedidoProdutos);
                        }

                        if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                            break;
                        if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                            break;
                        if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                            break;
                    }
                }

                if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                    break;
                if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                    break;
                if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                    break;
            }
        }

        private void ObterGruposPedidos(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, ref string erro, ref string msgAviso, Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, ref decimal maiorCapacidadeVeicular, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa) //Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros) //List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, ref string erro, ref string msgAviso, Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, ref decimal maiorCapacidadeVeicular, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar, List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (from item in centrosCarregamento select item).FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP vrp = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP : centroCarregamento.TipoMontagemCarregamentoVRP);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP tipoOcupacao = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoOcupacaoMontagemCarregamentoVRP : centroCarregamento.TipoOcupacaoMontagemCarregamentoVRP);
            bool dispFrotaCentroDescCliente = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente : centroCarregamento.UtilizarDispFrotaCentroDescCliente);

            disponibilidadeDiaUtilizar = (from disp in disponibilidadeDiaUtilizar where disp.Quantidade > 0 select disp).ToList();
            int qtde = (from it in disponibilidadeDiaUtilizar
                        select it.Quantidade).Sum();

            if (qtde == 0 && !dispFrotaCentroDescCliente)
            {
                erro = "1 - Nenhuma disponibilidade de frota disponível" + (sessaoRoteirizador != null ? " para a sessão de roteirização." : ".");
                return;
            }

            bool gerarCarregamentosAlemDaDispFrota = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.GerarCarregamentosAlemDaDispFrota : centroCarregamento.GerarCarregamentosAlemDaDispFrota);

            int carregamentoTempoMaximoRota = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.CarregamentoTempoMaximoRota : centroCarregamento.CarregamentoTempoMaximoRota);

            bool considerarTempoDeslocamentoCD = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.ConsiderarTempoDeslocamentoCD : centroCarregamento.ConsiderarTempoDeslocamentoPrimeiraEntrega);
            int quantidadeMaximaEntregasRoteirizar = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar : centroCarregamento.QuantidadeMaximaEntregasRoteirizar);
            bool gerarCarregamentoDoisDias = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.GerarCarregamentoDoisDias : centroCarregamento.GerarCarregamentoDoisDias);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = this.ObterConfiguracaoIntegracao();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupados = listaGrupoPedidos;

            if (vrp == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP.Nenhum || vrp == TipoMontagemCarregamentoVRP.Prioridades)
            {
                var grupos = (from pedido in pedidos
                              select new
                              {
                                  CodigoFilial = pedido.Filial?.Codigo,
                                  Filial = pedido.Filial?.Descricao,
                                  CodigoDeTipoCargaPrincipal = pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo,
                                  CodigoDeTipoCarga = ((pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo ?? 0) == 0 ? pedido.TipoDeCarga?.Codigo : 0),
                                  TipoCargaPaletizado = (pedido.TipoDeCarga?.TipoCargaPrincipal?.Paletizado ?? pedido.TipoDeCarga?.Paletizado ?? false),
                                  CodigoTipoOperacao = pedido.TipoOperacao?.Codigo,
                                  CodigoRotaFrete = pedido?.RotaFrete?.Codigo,
                                  Prioridade = ((pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : pedido?.CanalEntrega?.NivelPrioridade),
                                  CodigoDeposito = pedido?.Deposito?.Codigo,
                                  CodigoAgrupamento = pedido?.CodigoAgrupamentoCarregamento ?? string.Empty
                              }).Distinct().ToList();

                if (vrp == TipoMontagemCarregamentoVRP.Prioridades)
                {
                    int? defaultNull = null;
                    grupos = (from pedido in pedidos
                              select new
                              {
                                  CodigoFilial = pedido.Filial?.Codigo,
                                  Filial = pedido.Filial?.Descricao,
                                  CodigoDeTipoCargaPrincipal = pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo,
                                  CodigoDeTipoCarga = ((pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo ?? 0) == 0 ? pedido.TipoDeCarga?.Codigo : 0),
                                  TipoCargaPaletizado = false,
                                  CodigoTipoOperacao = defaultNull,
                                  CodigoRotaFrete = defaultNull,
                                  Prioridade = defaultNull,
                                  CodigoDeposito = defaultNull,
                                  CodigoAgrupamento = pedido?.CodigoAgrupamentoCarregamento ?? string.Empty
                              }).Distinct().ToList();
                }

                if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                {
                    int? codigoTipoOperacao = null;
                    grupos = (from pedido in pedidos
                              select new
                              {
                                  CodigoFilial = pedido.Filial?.Codigo,
                                  Filial = pedido.Filial?.Descricao,
                                  CodigoDeTipoCargaPrincipal = pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo,
                                  CodigoDeTipoCarga = ((pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo ?? 0) == 0 ? pedido.TipoDeCarga?.Codigo : 0),
                                  TipoCargaPaletizado = (pedido.TipoDeCarga?.TipoCargaPrincipal?.Paletizado ?? pedido.TipoDeCarga?.Paletizado ?? false),
                                  CodigoTipoOperacao = codigoTipoOperacao,
                                  CodigoRotaFrete = pedido?.RotaFrete?.Codigo,
                                  Prioridade = ((pedido?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : pedido?.CanalEntrega?.NivelPrioridade),
                                  CodigoDeposito = pedido?.Deposito?.Codigo,
                                  CodigoAgrupamento = pedido?.CodigoAgrupamentoCarregamento ?? string.Empty
                              }).Distinct().ToList();
                }
                //Nível de prioridade de fechamento de cargas.
                grupos = grupos.OrderBy(x => x.Prioridade).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidoCarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                if (!dispFrotaCentroDescCliente)
                {

                    foreach (var grupo in grupos)
                    {
                        foreach (var transp in disponibilidadeDiaUtilizar)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGrupo = null;
                            if (grupo.CodigoDeTipoCargaPrincipal.HasValue && vrp != TipoMontagemCarregamentoVRP.Prioridades)
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal &&
                                                                  o.TipoOperacao?.Codigo == grupo?.CodigoTipoOperacao &&
                                                                  (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                  o.Deposito?.Codigo == grupo?.CodigoDeposito &&
                                                                  (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();
                            else if (vrp != TipoMontagemCarregamentoVRP.Prioridades)
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga &&
                                                                  o.TipoOperacao?.Codigo == grupo?.CodigoTipoOperacao &&
                                                                  (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                  o.Deposito?.Codigo == grupo?.CodigoDeposito &&
                                                                  (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();
                            else if (grupo.CodigoDeTipoCargaPrincipal.HasValue && vrp == TipoMontagemCarregamentoVRP.Prioridades)
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal &&
                                                                  (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();
                            else if (vrp == TipoMontagemCarregamentoVRP.Prioridades)
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga &&
                                                                  (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();

                            if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                            {
                                if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                                    pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                      o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal &&
                                                                      (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                      //(o?.CanalEntrega?.NivelPrioridade ?? 999) == grupo.Prioridade &&
                                                                      o.Deposito?.Codigo == grupo?.CodigoDeposito &&
                                                                      (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();
                                else
                                    pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                      o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga &&
                                                                      (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                      //(o?.CanalEntrega?.NivelPrioridade ?? 999) == grupo.Prioridade &&
                                                                      o.Deposito?.Codigo == grupo?.CodigoDeposito &&
                                                                      (o.CodigoAgrupamentoCarregamento ?? string.Empty) == grupo.CodigoAgrupamento).ToList();
                            }
                            //Filtrar pedidos que não estão adicionados no retorno..
                            pedidosGrupo = pedidosGrupo.FindAll(x => !agrupados.Exists(c => c.Pedidos.Any(p => p.Codigo == x.Codigo))).ToList();

                            if (transp?.Transportador != null)
                                pedidosGrupo = pedidosGrupo.FindAll(x => (x?.RotaFrete?.Empresas?.Any(e => e?.Empresa?.Codigo == transp?.Transportador?.Codigo) ?? false)).ToList();

                            while (pedidosGrupo.Count > 0)
                            {
                                DateTime data_prev_carregamento_grupo = (from p in pedidosGrupo
                                                                         select p.DataCarregamentoPedido)?.Min() ?? DateTime.Now;

                                if (data_prev_carregamento_grupo.Date < DateTime.Now.Date)
                                    data_prev_carregamento_grupo = DateTime.Now;

                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = null;
                                if (vrp == TipoMontagemCarregamentoVRP.Nenhum)
                                {
                                    modeloVeicular = ObterModeloVeicularDisponibilidade(grupo.CodigoFilial ?? 0, transp?.Transportador?.Codigo ?? 0, data_prev_carregamento_grupo, centrosCarregamento, disponibilidadeDiaUtilizar, listaGrupoPedidos, sessaoRoteirizadorParametros?.TipoOcupacaoMontagemCarregamentoVRP ?? TipoOcupacaoMontagemCarregamentoVRP.Peso, ref erro);
                                    Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular = null;

                                    if (!string.IsNullOrEmpty(erro) && agrupados.Count == 0)
                                        break;

                                    if (modeloVeicular == null)
                                    {
                                        if (!gerarCarregamentosAlemDaDispFrota)
                                            break;

                                        tipoCargaModeloVeicular = repTipoCargaModeloVeicular.ConsultarPrimeiroPorTipoDeCarga(grupo.CodigoDeTipoCarga ?? 0);
                                        modeloVeicular = tipoCargaModeloVeicular?.ModeloVeicularCarga;
                                    }

                                    if (modeloVeicular == null && agrupados.Count == 0)
                                    {
                                        erro = tipoCargaModeloVeicular != null ? $"Não existe modelo veicular para o tipo de carga {tipoCargaModeloVeicular?.Descricao}" : $"Não existe modelo veicular para o centro de carregamento da filial {grupo.Filial}";
                                        break;
                                    }
                                }
                                else
                                {
                                    foreach (var disponibilidade in disponibilidadeDiaUtilizar)
                                    {
                                        int qtdeCarregamentosModelo = (from agr in agrupados
                                                                       where agr.ModeloVeicular.Codigo == disponibilidade.ModeloVeicular.Codigo
                                                                       select agr).Count();

                                        if (disponibilidade.Quantidade > qtdeCarregamentosModelo)
                                            modeloVeicular = disponibilidade.ModeloVeicular;
                                    }
                                }

                                if (modeloVeicular != null)
                                {
                                    int qtdeMaxEntregasCentroCarregamento = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar : centroCarregamento?.QuantidadeMaximaEntregasRoteirizar ?? 100);
                                    if (qtdeMaxEntregasCentroCarregamento == 0) qtdeMaxEntregasCentroCarregamento = 100;

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar nivelQuebraProdutoRoteirizar = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.NivelQuebraProdutoRoteirizar : centroCarregamento?.NivelQuebraProdutoRoteirizar ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar.Item);

                                    decimal pesoMaximo = modeloVeicular.CapacidadePesoTransporte;
                                    if (maiorCapacidadeVeicular < pesoMaximo)
                                        maiorCapacidadeVeicular = pesoMaximo;

                                    Dictionary<int, decimal> pedidosPesosCarregamento = new Dictionary<int, decimal>();
                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidosProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPeso = ObterPedidosPorPeso(modeloVeicular,
                                                                                                                        pedidosGrupo,
                                                                                                                        ref pedidosPesosCarregamento,
                                                                                                                        ref pedidosProdutos,
                                                                                                                        ref msgAviso,
                                                                                                                        ref sessaoRoteirizadorPedidosResultado,
                                                                                                                        nivelQuebraProdutoRoteirizar,
                                                                                                                        sessaoRoteirizador,
                                                                                                                        listaGrupoPedidos,
                                                                                                                        linhasSeparacaoAgrupa,
                                                                                                                        null,
                                                                                                                        null,
                                                                                                                        grupo.TipoCargaPaletizado,
                                                                                                                        centroCarregamento,
                                                                                                                        sessaoRoteirizadorParametros);

                                    if (pedidosPeso.Count == 0)
                                        break;

                                    if (pedidosPeso.Count > 0 && pedidosGrupo.Count > 0)
                                    {
                                        //Aki, vamos validar a disponibilidade de carros, pegar o que dá maior ocupação... (pegar o menor maior que o peso...)
                                        var outroModeloMelhorOcupacao = ObterModeloVeicularDisponibilidadeMaiorMenor(grupo.CodigoFilial ?? 0, transp?.Transportador?.Codigo ?? 0, data_prev_carregamento_grupo, centrosCarregamento, disponibilidadeDiaUtilizar, listaGrupoPedidos, pedidosPesosCarregamento.Sum(s => s.Value), sessaoRoteirizadorParametros?.TipoOcupacaoMontagemCarregamentoVRP ?? TipoOcupacaoMontagemCarregamentoVRP.Peso, pedidosProdutos);

                                        if (outroModeloMelhorOcupacao != null && vrp != TipoMontagemCarregamentoVRP.Prioridades)
                                            modeloVeicular = outroModeloMelhorOcupacao;

                                        Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido carregamentoGrupoPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido
                                        {
                                            Pedidos = pedidosPeso,
                                            ModeloVeicular = modeloVeicular,
                                            CodigoFilial = grupo.CodigoFilial ?? 0,
                                            Transportador = transp?.Transportador,
                                            DataCarregamento = data_prev_carregamento_grupo.Date,
                                            PedidosPesos = pedidosPesosCarregamento,
                                            Produtos = pedidosProdutos
                                        };
                                        agrupados.Add(carregamentoGrupoPedido);
                                    }
                                    RemoverLista(pedidosGrupo, pedidosPeso, pedidosPesosCarregamento, /*pedidosProdutos, */ agrupados, sessaoRoteirizador);

                                    //Filtrando os pedidos que já não possuem uma validação de retorno..
                                    // e não pode existir nos agrupados.. pois em carregamentos de produtos pallet < 1 não quebra.. e pode estar carregando outro produto
                                    // contudo não devemos remover da lista para que o pedido vá no próximo carregamento
                                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> temp = sessaoRoteirizadorPedidosResultado;
                                    temp.RemoveAll(x => agrupados.Exists(r => r.Pedidos.Exists(p => p.Codigo == x.Pedido.Codigo)));
                                    pedidosGrupo = pedidosGrupo.FindAll(x => !temp.Exists(r => r.Pedido.Codigo == x.Codigo));
                                }
                                else
                                    break;
                            }
                        }
                    }

                }
                else // Utiliza os veiculos do centro de descarregamento do cliente... não controla qtde de veiculos.
                {
                    foreach (var grupo in grupos)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGrupo = null;
                        if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                            pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                              o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal &&
                                                              o.TipoOperacao?.Codigo == grupo?.CodigoTipoOperacao &&
                                                              (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                              o.Deposito?.Codigo == grupo?.CodigoDeposito).ToList();
                        else
                            pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                              o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga &&
                                                              o.TipoOperacao?.Codigo == grupo?.CodigoTipoOperacao &&
                                                              (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                              o.Deposito?.Codigo == grupo?.CodigoDeposito).ToList();

                        if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                        {
                            if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal &&
                                                                  (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                  //(o?.CanalEntrega?.NivelPrioridade ?? 999) == grupo.Prioridade &&
                                                                  o.Deposito?.Codigo == grupo?.CodigoDeposito).ToList();
                            else
                                pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                                  o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga &&
                                                                  (_configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa || o.RotaFrete?.Codigo == grupo?.CodigoRotaFrete) &&
                                                                  //(o?.CanalEntrega?.NivelPrioridade ?? 999) == grupo.Prioridade &&
                                                                  o.Deposito?.Codigo == grupo?.CodigoDeposito).ToList();
                        }

                        //Filtrar pedidos que não estão adicionados no retorno..
                        pedidosGrupo = pedidosGrupo.FindAll(x => !agrupados.Exists(c => c.Pedidos.Any(p => p.Codigo == x.Codigo))).ToList();

                        Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
                        Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

                        int codigoTipoCarga = grupo.CodigoDeTipoCarga ?? 0;
                        if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                            codigoTipoCarga = grupo.CodigoDeTipoCargaPrincipal.Value;

                        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tiposCargaModeloVeicular = repTipoCargaModeloVeicular.ConsultarPorTipoCarga(codigoTipoCarga);

                        List<double> destinatarios = (from destino in pedidosGrupo
                                                      select destino?.Destinatario?.CPF_CNPJ ?? 0).Distinct().ToList();

                        List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarga = repCentroDescarregamento.BuscarPorDestinatarios(destinatarios);
                        if (pedidosGrupo.Count > 0 && ((centrosDescarga == null) || (centrosDescarga.Count == 0)))
                        {
                            erro += $"1 - Não existe centro de descarga para o destino {destinatarios.FirstOrDefault()}";
                            break;
                        }

                        while (pedidosGrupo.Count > 0)
                        {
                            destinatarios = (from destino in pedidosGrupo
                                             select destino?.Destinatario?.CPF_CNPJ ?? 0).Distinct().ToList();

                            centrosDescarga = centrosDescarga.FindAll(x => destinatarios.Contains(x.Destinatario.CPF_CNPJ));

                            if (centrosDescarga.Count == 0)
                            {
                                erro += $"1 - Não existe centro de descarga para algum dos destinatários, por favor verifique";
                                break;
                            }

                            DateTime data_prev_carregamento_grupo = (from p in pedidosGrupo
                                                                     select p.DataCarregamentoPedido)?.Min() ?? DateTime.Now;

                            if (data_prev_carregamento_grupo.Date < DateTime.Now.Date)
                                data_prev_carregamento_grupo = DateTime.Now;

                            //Vamos pegar o maior modelo veicular dos centros de descarga...
                            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarga = centrosDescarga.Where(x => x.VeiculosPermitidos?.Count > 0).FirstOrDefault();
                            if (centroDescarga == null)
                            {
                                erro += $"Centro de descarga {centrosDescarga[0].Descricao} não possui cadastro de veículos permitios.";
                                break;
                            }

                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidos = (from modelo in centroDescarga.VeiculosPermitidos
                                                                                                               select modelo).OrderByDescending(p => p.CapacidadePesoTransporte).ToList();

                            if ((modelosPermitidos == null) || (modelosPermitidos.Count == 0))
                            {
                                erro = $"Não existe cadastro de veículos permitidos para o centro de descarga {(from centro in centrosDescarga select centro.Descricao).FirstOrDefault()} .";
                                break;
                            }

                            modelosPermitidos = modelosPermitidos.Where(x => tiposCargaModeloVeicular.Any(t => t.ModeloVeicularCarga.Codigo == x.Codigo)).ToList();

                            if ((modelosPermitidos == null) || (modelosPermitidos.Count == 0))
                            {
                                erro = $"Não existe cadastro de veículos permitidos para o centro de descarga {(from centro in centrosDescarga select centro.Descricao).FirstOrDefault()} para o Tipo de Carga {repTipoDeCarga.BuscarPorCodigo(codigoTipoCarga)?.Descricao ?? codigoTipoCarga.ToString()}.";
                                break;
                            }

                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = modelosPermitidos[0];

                            if (!string.IsNullOrEmpty(erro) && agrupados.Count == 0)
                                break;

                            if (modeloVeicular != null)
                            {
                                //Primeiro, vamos fechar as cargas "Simples" por produto.
                                ObterPedidoProdutoCargaSimples(pedidosGrupo, modelosPermitidos, centrosDescarga, tiposCargaModeloVeicular, sessaoRoteirizador, ref sessaoRoteirizadorPedidosResultado, agrupados);

                                int qtdeMaxEntregasCentroCarregamento = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar : (centroCarregamento?.QuantidadeMaximaEntregasRoteirizar ?? 100));
                                if (qtdeMaxEntregasCentroCarregamento == 0)
                                {
                                    erro += $"O máximo de entregas no centro de carregamento {centroCarregamento?.Descricao} não está configurado.";
                                    break;
                                }

                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar nivelQuebraProdutoRoteirizar = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.NivelQuebraProdutoRoteirizar : centroCarregamento?.NivelQuebraProdutoRoteirizar ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar.Item);

                                decimal pesoMaximo = modeloVeicular.CapacidadePesoTransporte;
                                if (maiorCapacidadeVeicular < pesoMaximo)
                                    maiorCapacidadeVeicular = pesoMaximo;

                                Dictionary<int, decimal> pedidosPesosCarregamento = new Dictionary<int, decimal>();
                                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidosProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();
                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPeso = ObterPedidosPorPeso(modeloVeicular,
                                                                                                                    pedidosGrupo,
                                                                                                                    ref pedidosPesosCarregamento,
                                                                                                                    ref pedidosProdutos,
                                                                                                                    ref msgAviso,
                                                                                                                    ref sessaoRoteirizadorPedidosResultado,
                                                                                                                    nivelQuebraProdutoRoteirizar,
                                                                                                                    sessaoRoteirizador,
                                                                                                                    listaGrupoPedidos,
                                                                                                                    linhasSeparacaoAgrupa,
                                                                                                                    centrosDescarga,
                                                                                                                    tiposCargaModeloVeicular,
                                                                                                                    grupo.TipoCargaPaletizado,
                                                                                                                    centroCarregamento,
                                                                                                                    sessaoRoteirizadorParametros);

                                if (pedidosPeso.Count == 0)
                                    break;

                                if (pedidosPeso.Count > 0 && pedidosGrupo.Count > 0)
                                {
                                    //Localizando o primeiro destinatário dos pedidos selecionados para o carregamento
                                    double destinatario = (from destino in pedidosPeso
                                                           select destino?.Destinatario?.CPF_CNPJ ?? 0).FirstOrDefault();

                                    List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescargaDestinatario = centrosDescarga.Where(x => x.Destinatario.CPF_CNPJ == destinatario).ToList();
                                    if ((centrosDescargaDestinatario == null) || (centrosDescargaDestinatario.Count == 0))
                                    {
                                        erro += $"2 - Não existe centro de descarga para o destino {destinatario}";
                                        break;
                                    }

                                    //Vamos pegar o maior modelo veicular dos centros de descarga...
                                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescargaDestinatario = centrosDescargaDestinatario.Where(x => x.VeiculosPermitidos?.Count > 0).FirstOrDefault();
                                    if (centroDescargaDestinatario == null)
                                    {
                                        erro += $"Centro de descarga {centrosDescargaDestinatario[0].Descricao} não possui cadastro de veículos permitios.";
                                        break;
                                    }

                                    //Aki, vamos validar a disponibilidade de carros, pegar o que dá maior ocupação... (pegar o menor maior que o peso...)
                                    List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidosDestinatario = (from modelo in centroDescargaDestinatario.VeiculosPermitidos
                                                                                                                                   select modelo).OrderByDescending(p => p.CapacidadePesoTransporte).ToList();

                                    modelosPermitidosDestinatario = modelosPermitidosDestinatario.Where(x => tiposCargaModeloVeicular.Any(t => t.ModeloVeicularCarga.Codigo == x.Codigo)).ToList();

                                    if ((modelosPermitidosDestinatario == null) || (modelosPermitidosDestinatario.Count == 0))
                                    {
                                        erro = $"Não existe cadastro de veículos permitidos para o centro de descarga {(from centro in centrosDescargaDestinatario select centro.Descricao).FirstOrDefault()} para o Tipo de Carga {repTipoDeCarga.BuscarPorCodigo(codigoTipoCarga)?.Descricao ?? codigoTipoCarga.ToString()}.";
                                        break;
                                    }

                                    modeloVeicular = ObterMenorModeloVeicularCarregamento(modeloVeicular, modelosPermitidosDestinatario, pedidosPesosCarregamento, pedidosProdutos, sessaoRoteirizador.MontagemCarregamentoPedidoProduto, grupo.TipoCargaPaletizado);

                                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido carregamentoGrupoPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido
                                    {
                                        Pedidos = pedidosPeso,
                                        ModeloVeicular = modeloVeicular,
                                        CodigoFilial = grupo.CodigoFilial ?? 0,
                                        Transportador = null,
                                        DataCarregamento = data_prev_carregamento_grupo.Date,
                                        PedidosPesos = pedidosPesosCarregamento,
                                        Produtos = pedidosProdutos
                                    };
                                    agrupados.Add(carregamentoGrupoPedido);
                                }
                                RemoverLista(pedidosGrupo, pedidosPeso, pedidosPesosCarregamento, agrupados, sessaoRoteirizador);

                                //Filtrando os pedidos que já não possuem uma validação de retorno..
                                // e não pode existir nos agrupados.. pois em carregamentos de produtos pallet < 1 não quebra.. e pode estar carregando outro produto
                                // contudo não devemos remover da lista para que o pedido vá no próximo carregamento
                                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> temp = sessaoRoteirizadorPedidosResultado;
                                temp.RemoveAll(x => agrupados.Exists(r => r.Pedidos.Exists(p => p.Codigo == x.Pedido.Codigo)));
                                pedidosGrupo = pedidosGrupo.FindAll(x => !temp.Exists(r => r.Pedido.Codigo == x.Codigo));

                            }
                            else
                                break;
                        }

                    }
                }
            }
            else
            {
                this.AtualizarPontoDeApoioDestinatarios(ref pedidos, sessaoRoteirizador);

                bool agruparMesmoDestinatario = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.AgruparPedidosMesmoDestinatario : centroCarregamento.AgruparPedidosMesmoDestinatario);
                GoogleOrTools.EnumOcupacaoVeiculo ocupacao = GoogleOrTools.EnumOcupacaoVeiculo.CAPACIDADE_MAX;
                GoogleOrTools.EnumFirstSolutionStrategy strategy = GoogleOrTools.EnumFirstSolutionStrategy.PathCheapestArc;

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento> temposCarregamentoCentro = null;
                if ((sessaoRoteirizadorParametros?.TemposCarregamento?.Count ?? 0) > 0)
                    temposCarregamentoCentro = sessaoRoteirizadorParametros.TemposCarregamento;
                else
                    temposCarregamentoCentro = (from item in centroCarregamento.TemposCarregamento.ToList()
                                                select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento()
                                                {
                                                    Codigo = item.Codigo,
                                                    CodigoModeloVeicular = item.ModeloVeicular?.Codigo ?? 0,
                                                    DescricaoModeloVeicular = item.ModeloVeicular?.Descricao ?? string.Empty,
                                                    CodigoTipoCarga = item.TipoCarga?.Codigo ?? 0,
                                                    DescricaoTipoCarga = item.TipoCarga?.Descricao ?? string.Empty,
                                                    Quantidade = item.QuantidadeMaximaEntregasRoteirizar,
                                                    QuantidadeUtilizar = item.QuantidadeMaximaEntregasRoteirizar
                                                }).ToList();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.ServidorRouteGoogleOrTools))
                {
                    erro = "Não existe configuração do servidor VRP Capacidade e Janela.";
                    return;
                }

                GoogleOrTools.Api api = new GoogleOrTools.Api(configuracaoIntegracao.ServidorRouteGoogleOrTools, configuracaoIntegracao.ServidorRouteOSM);
                api.DesconsiderarTempoDeslocamentoDeposito = !considerarTempoDeslocamentoCD;
                api.TipoRota = (considerarTempoDeslocamentoCD ? GoogleOrTools.EnumCargaTpRota.ComRetorno : GoogleOrTools.EnumCargaTpRota.SemIda);
                api.QtdeMaximaEntregas = quantidadeMaximaEntregasRoteirizar;

                GoogleOrTools.Local deposito = new GoogleOrTools.Local()
                {
                    Codigo = (long)centroCarregamento.Codigo,
                    Deposito = true,
                    Latitude = ObterLatitudeOuLongitude(centroCarregamento.Latitude),
                    Longitude = ObterLatitudeOuLongitude(centroCarregamento.Longitude),
                    Janela = new GoogleOrTools.TimeWindow(0, 1440, 0),
                    TipoPonto = TipoPontoPassagem.Coleta
                };

                if (deposito.Latitude == 0 || deposito.Longitude == 0)
                {
                    List<Dominio.Entidades.Cliente> remetentesPedidos = (from pedido in pedidos select pedido.Remetente).Distinct().ToList();
                    Dominio.Entidades.Cliente remetente = remetentesPedidos.FirstOrDefault();
                    deposito.Latitude = ObterLatitudeOuLongitude(remetente.Latitude);
                    deposito.Longitude = ObterLatitudeOuLongitude(remetente.Longitude);
                }

                //Aqui.. temos a capacidade máxima de carregamento de todos os recursos disponíveis.
                decimal capac_carga_recursos = disponibilidadeDiaUtilizar.Sum(x => x.Quantidade * x.ModeloVeicular.CapacidadePesoTransporte);

                var grupos = (from pedido in pedidos
                              select new
                              {
                                  CodigoFilial = pedido.Filial?.Codigo,
                                  Filial = pedido.Filial?.Descricao,
                                  CodigoDeTipoCargaPrincipal = pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo,
                                  CodigoDeTipoCarga = ((pedido.TipoDeCarga?.TipoCargaPrincipal?.Codigo ?? 0) == 0 ? pedido.TipoDeCarga?.Codigo : 0),
                                  TipoCargaPaletizado = (pedido.TipoDeCarga?.TipoCargaPrincipal?.Paletizado ?? pedido.TipoDeCarga?.Paletizado ?? false)
                              }).Distinct().ToList();

                bool localizacaoColeta = ((sessaoRoteirizador?.TipoRoteirizacaoColetaEntrega ?? TipoRoteirizacaoColetaEntrega.Entrega) != TipoRoteirizacaoColetaEntrega.Entrega);

                foreach (var grupo in grupos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGrupo = null;
                    int codigoTipoCarga = 0;
                    if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                    {
                        codigoTipoCarga = grupo?.CodigoDeTipoCargaPrincipal ?? 0;
                        pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                          o.TipoDeCarga?.TipoCargaPrincipal?.Codigo == grupo?.CodigoDeTipoCargaPrincipal).ToList();
                    }
                    else
                    {
                        codigoTipoCarga = grupo?.CodigoDeTipoCarga ?? 0;
                        pedidosGrupo = pedidos.Where(o => o.Filial?.Codigo == grupo?.CodigoFilial &&
                                                          o.TipoDeCarga?.Codigo == grupo?.CodigoDeTipoCarga).ToList();
                    }

                    //Filtrar pedidos que não estão adicionados no retorno..
                    pedidosGrupo = pedidosGrupo.FindAll(pg => !agrupados.Exists(c => c.Pedidos.Any(p => p.Codigo == pg.Codigo))).ToList();
                    if (pedidosGrupo?.Count == 0)
                        continue;

                    var datasCarregamento = (from pedido in pedidosGrupo
                                             select pedido?.DataCarregamentoPedido?.Date ?? DateTime.Now.Date).Distinct().ToList();

                    //Quando tem mais de uma data de carregamento, devemos agrupar..
                    // Verificar e classificar se a previsão de etnrega pode ir para frente..
                    // No caso do leite.. pedidos 48 horas, podem ser no dia 01 ou no dia 02
                    // então no pedido vai constar data carregamento 10/06/2020 e a previsão de entrega até 11/06/2020 23:59
                    // quando é 24 horas a data de carregamento vai ser a mesma da data de previsão de entrega...
                    // devemos separar alguns pedidos...
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos1 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos2 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                    int qtdeCarregamentosSessaoAndamento = 0;
                    decimal pesoTotalCarregadoSessaoAndamento = 0;
                    //Leite
                    if (datasCarregamento.Count == 2 && gerarCarregamentoDoisDias)
                    {
                        qtdeCarregamentosSessaoAndamento = (from car in _carregamentosSessao
                                                            where car.SituacaoCarregamento == SituacaoCarregamento.EmMontagem
                                                            select car).Count();

                        pesoTotalCarregadoSessaoAndamento = (from car in _carregamentosSessao
                                                             where car.SituacaoCarregamento == SituacaoCarregamento.EmMontagem
                                                             select car.PesoCarregamento).Sum();

                        //Separando os 24 horas....
                        for (int i = 0; i < datasCarregamento.Count; i++)
                        {
                            var tmp = pedidosGrupo.FindAll(x => (x?.DataCarregamentoPedido?.Date ?? DateTime.Now.Date) == datasCarregamento[i].Date &&
                                                                (x?.PrevisaoEntrega?.Date ?? DateTime.Now.Date) == datasCarregamento[i].Date);
                            if (i % 2 == 0)
                                pedidos1.AddRange(tmp);
                            else
                                pedidos2.AddRange(tmp);
                        }

                        //Agora vamos pegar todos os demais pedidos para separar..
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> outros = pedidosGrupo.Where(p => !pedidos1.Exists(u => u.Codigo == p.Codigo) &&
                                                                                                           !pedidos2.Exists(d => d.Codigo == p.Codigo)).ToList();

                        decimal saldo = outros.Sum(x => x.PesoTotal);

                        var locais_outros = outros.GroupBy(x => new { x.Codigo, x.Destinatario.Latitude, x.Destinatario.Longitude })
                                                  .Select(g => new
                                                  {
                                                      id = g.Key.Codigo,
                                                      latitude = ObterLatitudeOuLongitude(g.Key.Latitude),
                                                      longitude = ObterLatitudeOuLongitude(g.Key.Longitude)
                                                  }).ToList();

                        //Ordenanos pelos mais distantes......
                        // vamos pegar os mais distantes do destino. para tentar agrupar...no dia 01...
                        locais_outros = locais_outros.OrderByDescending(x => Logistica.Polilinha.CalcularDistancia(deposito.Latitude, deposito.Longitude, x.latitude, x.longitude)).ToList();
                        for (int i = 0; i < locais_outros.Count; i++)
                        {
                            var vizinhos = locais_outros.Where(x => x.id != locais_outros[i].id)
                                                        .OrderBy(x => Logistica.Polilinha.CalcularDistancia(locais_outros[i].latitude, locais_outros[i].longitude, x.latitude, x.longitude)).ToList();
                            decimal separadoDia1 = 0;
                            for (int j = 0; j < vizinhos.Count; j++)
                            {
                                var ped = pedidosGrupo.FirstOrDefault(x => x.Codigo == vizinhos[j].id);
                                pedidos1.Add(ped);
                                separadoDia1 += ped.PesoTotal;
                                // Total separando para o dia 1 for aior que o saldo total pendente - totalEmcarregamento.. vamos descontar do dia 01...
                                if (separadoDia1 >= ((saldo - pesoTotalCarregadoSessaoAndamento) / 2))
                                    break;
                            }
                            if (separadoDia1 >= ((saldo - pesoTotalCarregadoSessaoAndamento) / 2))
                                break;
                        }
                        var restoDoResto = pedidosGrupo.Where(p => !pedidos1.Exists(u => u.Codigo == p.Codigo) &&
                                                                   !pedidos2.Exists(d => d.Codigo == p.Codigo)).ToList();

                        pedidos2.AddRange(restoDoResto);
                    }

                    int totalDisponibilidadeFrota = 0;

                    for (int xx = 0; xx < 2; xx++)
                    {
                        api.Veiculos = new List<GoogleOrTools.Veiculo>();
                        api.Locais = new List<GoogleOrTools.Local>();

                        if (!gerarCarregamentoDoisDias && xx > 0)
                            break;
                        if (xx > 0 && (pedidos1?.Count + pedidos2?.Count) == 0)
                            break;

                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosFechando = pedidosGrupo;

                        if (gerarCarregamentoDoisDias)
                        {
                            if (xx == 0 && pedidos1?.Count > 0)
                                pedidosFechando = pedidos1;
                            else if (pedidos2?.Count > 0)
                                pedidosFechando = pedidos2;
                        }

                        //pedidosFechando = pedidosFechando.OrderByDescending(x => x.PesoTotal).ToList();
                        pedidosFechando = pedidosFechando.OrderBy(x => x.PrevisaoEntrega).ThenBy(x => x.DataEntrega).ThenBy(x => x.Codigo).ToList();

                        decimal peso_sem_carga = pedidosFechando.Sum(x => x.PesoTotal);

                        //Vamos chegar os pontos de apoio do destinatário
                        List<int> codigosPontosDeApoio = (from ped in pedidosFechando
                                                          select ped.Destinatario?.PontoDeApoio?.Codigo ?? 0).Distinct().ToList();

                        ////Lista de CNPJ Fechando...
                        //List<double> cnpjCpfFechando = (from ped in pedidosFechando
                        //                                select ped.Destinatario.CPF_CNPJ).Distinct().ToList();

                        //localizando os endereços distintos e somando o peso total dos pedidos do endereço para 
                        // Agrupar na mesma carga....
                        dynamic locais_distintos_tmp = new List<dynamic>();
                        locais_distintos_tmp = pedidosFechando.Where(x => x.Destinatario.PontoDeApoio == null).Select(g => new
                        {
                            id = (long)g.Codigo,
                            latitude = ObterLatitudeOuLongitudePedido(g, true, localizacaoColeta),
                            longitude = ObterLatitudeOuLongitudePedido(g, false, localizacaoColeta),
                            peso_total = g.PesoTotal,
                            metro_total = g.CubagemTotal,
                            pallet_total = g.TotalPallets,
                            ponto_apoio = (int)(g.Destinatario.PontoDeApoio?.Codigo ?? 0),
                            endereco = (int)(!g.UsarOutroEnderecoDestino ? 0 : g.EnderecoDestino.ClienteOutroEndereco.Codigo),
                            pedidos = new List<int>() { g.Codigo },
                            tipo_ponto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
                        }).ToList();

                        //Agora vamos adicionar a lista
                        codigosPontosDeApoio = (from ponto in codigosPontosDeApoio
                                                where ponto > 0
                                                select ponto).Distinct().ToList();

                        for (int a = 0; a < codigosPontosDeApoio.Count; a++)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ppa = (from x in pedidosFechando
                                                                                     where x.Destinatario?.PontoDeApoio?.Codigo == codigosPontosDeApoio[a]
                                                                                     select x).ToList();
                            Dominio.Entidades.Embarcador.Logistica.Locais local = ppa[0].Destinatario.PontoDeApoio;
                            var areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(local.Area);

                            foreach (var p in ppa)
                            {
                                locais_distintos_tmp.Add(new
                                {
                                    id = (long)p.Codigo,
                                    latitude = areas[0].position.lat,
                                    longitude = areas[0].position.lng,
                                    peso_total = p.PesoTotal,
                                    metro_total = p.CubagemTotal,
                                    pallet_total = p.TotalPallets,
                                    ponto_apoio = (int)codigosPontosDeApoio[a],
                                    endereco = (int)0,
                                    pedidos = new List<int>() { p.Codigo },
                                    tipo_ponto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Apoio
                                });
                            }
                        }

                        // TODO: Ver para pegar os códigos dos pedidos.. quando agrupar..
                        if (agruparMesmoDestinatario)
                            locais_distintos_tmp = pedidosFechando.GroupBy(x => new { Codigo = (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Entrega ? (x.Recebedor != null ? x.Recebedor.Codigo : x.Destinatario.Codigo) : x.Remetente.Codigo), Latitude = ObterLatitudeOuLongitudePedido(x, true, localizacaoColeta), Longitude = ObterLatitudeOuLongitudePedido(x, false, localizacaoColeta), PontoDeApoio = x.Destinatario.PontoDeApoio?.Codigo ?? 0, Endereco = (int)(!x.UsarOutroEnderecoDestino ? 0 : x.EnderecoDestino.ClienteOutroEndereco.Codigo) })
                                                  .Select(g => new
                                                  {
                                                      id = g.Key.Codigo,
                                                      latitude = g.Key.Latitude,
                                                      longitude = g.Key.Longitude,
                                                      peso_total = g.Sum(s => s.PesoTotal),
                                                      metro_total = g.Sum(s => s.CubagemTotal),
                                                      pallet_total = g.Sum(s => s.TotalPallets),
                                                      ponto_apoio = g.Key.PontoDeApoio,
                                                      endereco = g.Key.Endereco,
                                                      pedidos = (from t in pedidosFechando
                                                                 where (t.Recebedor != null ? t.Recebedor.Codigo : t.Destinatario.Codigo) == g.Key.Codigo
                                                                 select t.Codigo).ToList(),
                                                      tipo_ponto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
                                                  }).ToList();

                        // Adicionando os remetentes para a api calcular o tempo e km...
                        if (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.ColetaEntrega)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ppa = (from x in pedidosFechando
                                                                                     where x.Remetente != null
                                                                                     select x).Distinct().ToList();
                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido p in ppa)
                            {
                                locais_distintos_tmp.Add(new
                                {
                                    id = (long)p.Codigo,
                                    latitude = ObterLatitudeOuLongitude(p.Remetente.Latitude),
                                    longitude = ObterLatitudeOuLongitude(p.Remetente.Longitude),
                                    peso_total = (decimal)0.01,
                                    metro_total = (decimal)0.01,
                                    pallet_total = (decimal)0.01,
                                    ponto_apoio = (int)0,
                                    endereco = (int)0,
                                    pedidos = new List<int>() { p.Codigo },
                                    tipo_ponto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
                                });
                            }
                        }

                        var maiorCV = (from d in disponibilidadeDiaUtilizar
                                       select d.ModeloVeicular.CapacidadePesoTransporte).Max();
                        if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                            maiorCV = (from d in disponibilidadeDiaUtilizar
                                       select d.ModeloVeicular.Cubagem).Max();
                        else if (tipoOcupacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                            maiorCV = (from d in disponibilidadeDiaUtilizar
                                       select d.ModeloVeicular.NumeroPaletes ?? 0).Max();

                        maiorCapacidadeVeicular = maiorCV;

                        dynamic locais_distintos = locais_distintos_tmp;

                        List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centroDescarregamentos = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();

                        // Coletando os cnpjs distintos das entregas...
                        List<double> cnpjs = new List<double>();
                        foreach (var local_entrega in locais_distintos)
                        {
                            double cnpjDestinatario = (double)local_entrega.id;
                            if (!agruparMesmoDestinatario)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoTmp = pedidosFechando.Find(x => x.Codigo == (int)local_entrega.id);
                                if (pedidoTmp != null)
                                {
                                    if (sessaoRoteirizador.TipoRoteirizacaoColetaEntrega == TipoRoteirizacaoColetaEntrega.Entrega)
                                        cnpjDestinatario = (double)(pedidoTmp.Recebedor == null ? pedidoTmp.Destinatario.Codigo : pedidoTmp.Recebedor.Codigo);
                                    else
                                        cnpjDestinatario = (double)pedidoTmp.Remetente.Codigo;
                                }
                            }
                            cnpjs.Add(cnpjDestinatario);
                        }

                        Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
                        centroDescarregamentos = repositorioCentroDescarregamento.BuscarPorDestinatarios(cnpjs);

                        List<int> codigoCanaisEntrega = (from canal in pedidosFechando select canal.CanalEntrega?.Codigo ?? 0).Distinct().ToList();
                        List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> temp = repositorioCentroDescarregamento.BuscarPorCanaisEntrega(codigoCanaisEntrega);

                        temp = temp.FindAll(x => !cnpjs.Contains(x.Destinatario?.CPF_CNPJ ?? 0)).ToList();
                        centroDescarregamentos.AddRange(temp);

                        //Agora vamos ver se for fornecedor.. para validar as restriçoes de veiculos..
                        Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, cnpjs);

                        List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
                        if (modalidadePessoasFornecedores.Count > 0)
                        {
                            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(_unitOfWork);
                            List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                            modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);
                        }

                        api.Locais.Add(deposito);

                        // Adicionando os locais a visitar..
                        this.ApiOrToolsLocaisDistintosVisitar(ref api, locais_distintos, tipoOcupacao, vrp, agruparMesmoDestinatario, pedidosFechando, modalidadeFornecedorPessoasRestricaoModeloVeicular, centroDescarregamentos, disponibilidadeDiaUtilizar, sessaoRoteirizador);

                        // Adicionando os recursos/disponibilidade de frota.
                        this.ApiOrToolsDisponibilidadeUtilizar(ref api, disponibilidadeDiaUtilizar, temposCarregamentoCentro, codigoTipoCarga, tipoOcupacao, agrupados);

                        int qtde_recursos_disp = (from veiculo in api.Veiculos
                                                  select veiculo.Quantidade).Sum();

                        if (qtde_recursos_disp == 0)
                        { // Nenhum veículo disponível
                            string descricao = string.Empty;
                            if (grupo.CodigoDeTipoCargaPrincipal.HasValue)
                                descricao = (from obj in pedidosGrupo where obj.TipoDeCarga != null && obj.TipoDeCarga.TipoCargaPrincipal.Codigo == codigoTipoCarga select obj.TipoDeCarga.TipoCargaPrincipal.Descricao).FirstOrDefault();
                            else
                                descricao = (from obj in pedidosGrupo where obj.TipoDeCarga != null && obj.TipoDeCarga.Codigo == codigoTipoCarga select obj.TipoDeCarga.Descricao).FirstOrDefault();

                            if (descricao != "")
                                erro += "Nenhum veículo disponível para o tipo de carga " + descricao;
                            else
                                erro += "Nenhum veículo disponível";

                            return;
                        }

                        if (totalDisponibilidadeFrota == 0)
                            totalDisponibilidadeFrota = qtde_recursos_disp;

                        api.Strategy = strategy;

                        // Resolvendo o problema de montagem de carga...
                        GoogleOrTools.ApiResultado apiResult = null;
                        if (vrp != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP.VrpTimeWindows)
                            apiResult = api.CVRP(gerarCarregamentosAlemDaDispFrota);
                        else
                            apiResult = api.CVRPTW(true, carregamentoTempoMaximoRota);

                        if (apiResult != null)
                        {
                            if (apiResult.status == false)
                                throw new Dominio.Excecoes.Embarcador.ServicoException(apiResult.msg.Replace("CVRPTW - ", "").Replace("CVRP - ", ""));

                            // Extraindo o resultado e adicionado a lista de agrupados.
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> resultadoParcial = this.ApiOrToolsResultado(apiResult, agruparMesmoDestinatario, gerarCarregamentoDoisDias, gerarCarregamentosAlemDaDispFrota, /*totalDisponibilidadeFrota, */datasCarregamento, xx, pedidosFechando, disponibilidadeDiaUtilizar, agrupados, sessaoRoteirizadorParametros, ref sessaoRoteirizadorPedidosResultado);

                            //Agora vamos trocar o veiculo para pegar o menor veiculo de acordo com o total carregado.
                            this.ApiOrToolsDefinirMenorVeiculoResultado(codigoTipoCarga, grupo.TipoCargaPaletizado, tipoOcupacao, api.QtdeMaximaEntregas, disponibilidadeDiaUtilizar, temposCarregamentoCentro, modalidadeFornecedorPessoasRestricaoModeloVeicular, centroDescarregamentos, resultadoParcial, sessaoRoteirizador, agrupados);

                            VerificacaoDaQuantidadeMinimaDoModeloVeicular(resultadoParcial);

                            agrupados.AddRange(resultadoParcial);
                        }
                        else
                        {
                            decimal totalCarregar = 0;// locais_distintos.Sum(x => x.peso_total);
                            foreach (var itemlocal in locais_distintos)
                                totalCarregar += itemlocal.peso_total;

                            if (totalCarregar > capac_carga_recursos)
                                erro = string.Format("Não foi possível gerar os carregamentos pois a capacidade total de carregamento [{0}] é inferior ao volume total a carregar [{1}].", (int)capac_carga_recursos, (int)totalCarregar);
                            else
                                erro = "Não foi possível encontrar uma melhor otimização para os pedidos e veículos disponíveis.";
                        }
                        System.GC.Collect();
                    }
                }
            }
            listaGrupoPedidos = agrupados;
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterMenorModeloVeicularCarregamento(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloAtual, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis, Dictionary<int, decimal> pedidosPesosCarregamento, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidosProdutos, bool montagemCarregamentoPedidoProduto, bool tipoCargaPaletizado)
        {
            decimal pesoTotal = pedidosPesosCarregamento.Sum(s => s.Value);
            decimal palletTotal = 0;
            decimal cubagemTotal = 0;
            if (montagemCarregamentoPedidoProduto)
            {
                pesoTotal = pedidosProdutos.Sum(s => s.PesoPedidoProduto);
                palletTotal = pedidosProdutos.Sum(s => s.QuantidadePalletPedidoProduto);
                cubagemTotal = pedidosProdutos.Sum(s => s.MetroCubicoPedidoProduto);
            }
            // Fechou o modelo CT30BS3, 90 toeladas, 90 pallets e 80 cubagem.. depois esta trocando para o menor maior peso..
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> todosModelosAtendem = (from modelo in modelosDisponiveis
                                                                                                 where modelo.CapacidadePesoTransporte >= pesoTotal &&
                                                                                                 palletTotal <= (!modelo.VeiculoPaletizado ? 0 : modelo.NumeroPaletes ?? 0) &&
                                                                                                 cubagemTotal <= (!modelo.ModeloControlaCubagem ? 0 : modelo.Cubagem - (tipoCargaPaletizado ? modelo.ObterOcupacaoCubicaPaletes() : 0m))
                                                                                                 select modelo).OrderBy(x => x.CapacidadePesoTransporte).ToList();

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga outroModeloMelhorOcupacao = null;
            if (todosModelosAtendem?.Count > 0)
                outroModeloMelhorOcupacao = todosModelosAtendem[0];

            if (outroModeloMelhorOcupacao != null)
                modeloAtual = outroModeloMelhorOcupacao;

            return modeloAtual;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> ObterDisponibilidadeFrotaUtilizar(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros)
        {
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();
            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota disp in disponibilidadeDia)
                disponibilidadeDiaUtilizar.Add(disp.Clonar());

            if (sessaoRoteirizadorParametros != null)
            {
                //Atualizando a disponibilidade de frota... do centro de acordo com os parametros alterados na sessão de roteirização..
                if (sessaoRoteirizadorParametros.DisponibilidadesFrota?.Count > 0)
                {
                    for (int i = 0; i < disponibilidadeDiaUtilizar.Count; i++)
                    {
                        if (!disponibilidadeDiaUtilizar[i].ModeloVeicular.Ativo)
                        {
                            disponibilidadeDiaUtilizar[i].Quantidade = -1;
                            continue;
                        }
                        //Problema tekbond, aonde iniciaram a sessão com truck e toco depois foi alterado no centro de carregamento para utilizar apenas VUC.
                        Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosDisponibilidadeFrota utilizar = (from disp in sessaoRoteirizadorParametros.DisponibilidadesFrota
                                                                                                                                           where disp.CodigoModeloVeicular == disponibilidadeDiaUtilizar[i].ModeloVeicular.Codigo &&
                                                                                                                                                 disp.CodigoTransportador == (disponibilidadeDiaUtilizar[i].Transportador?.Codigo ?? 0)
                                                                                                                                           select disp).FirstOrDefault();
                        if (utilizar != null)
                            disponibilidadeDiaUtilizar[i].Quantidade = utilizar.QuantidadeUtilizar;
                        else if ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete)
                            disponibilidadeDiaUtilizar[i].Quantidade = -1;
                    }
                }
            }

            return disponibilidadeDiaUtilizar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> ObterGruposPedidos(List<int> filiais, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, DateTime dataPrevistaCarregamento, ref bool dispFrotaCentroDescCliente, ref string erro, ref string msgAviso, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, ref decimal maiorCapacidadeVeicular, List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa, ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            dispFrotaCentroDescCliente = (from item in centrosCarregamento select item.UtilizarDispFrotaCentroDescCliente).FirstOrDefault();
            bool montagemPorPedidoProduto = (from item in centrosCarregamento select item.MontagemCarregamentoPedidoProduto).FirstOrDefault();
            sessaoRoteirizador.MontagemCarregamentoPedidoProduto = montagemPorPedidoProduto;

            if (centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoIntegral))
                sessaoRoteirizador.MontagemCarregamentoPedidoProduto = false;

            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia = ObterModeloVeicularDisponibilidade(centrosCarregamento, dataPrevistaCarregamento);

            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = ObterSessaoRoteirizadorParametros(sessaoRoteirizador);

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar = ObterDisponibilidadeFrotaUtilizar(disponibilidadeDia, sessaoRoteirizadorParametros);

            if ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros obterGrupoPedidosParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros()
                {
                    CentrosCarregamento = centrosCarregamento,
                    DisponibilidadeDia = disponibilidadeDia,
                    DisponibilidadeDiaUtilizar = disponibilidadeDiaUtilizar.Where(x => x.Quantidade >= 0).ToList(),
                    LinhasSeparacaoAgrupa = linhasSeparacaoAgrupa,
                    GruposPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>(),
                    SessaoRoteirizador = sessaoRoteirizador,
                    SessaoRoteirizadorParametros = sessaoRoteirizadorParametros,
                    SessaoRoteirizadorPedidosSituacao = sessaoRoteirizadorPedidosResultado,
                    Pedidos = (from obj in pedidos where (!obj.PedidoBloqueado || _configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado) select obj).ToList()
                };

                return ObterGruposPedidosSimulaFrete(obterGrupoPedidosParametros, ref erro);

            }
            else
            {
                if ((disponibilidadeDia == null || disponibilidadeDia.Count == 0) && !dispFrotaCentroDescCliente)
                {
                    erro = "Nenhuma disponibilidade de frota cadastrada para o centro de carregamento da filial.";
                    return new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();
                }

                //Lista com o resultado
                var listaGrupoPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();

                //Aqui vamos tratar se a filial possui Região roteirização.
                // Para as filiais que possuir a micro região. vamos usa-la para agrupar.. para as filiais que não possui vamos seguir normal;
                var repLocais = new Repositorio.Embarcador.Logistica.Locais(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Locais> microRegiaoRoteirizacao = repLocais.BuscarPorTipoDeLocalEFiliais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.MicroRegiaoRoteirizacao, filiais);
                List<int> filiaisComMicroRegiaoRoteirizacao = new List<int>();
                List<int> pedidosEmMicroRegiao = new List<int>();

                //bool montagemColetaEntrega = (sessaoRoteirizador?.MontagemCarregamentoColetaEntrega ?? false);
                bool localizacaoColeta = ((sessaoRoteirizador?.TipoRoteirizacaoColetaEntrega ?? TipoRoteirizacaoColetaEntrega.Entrega) == TipoRoteirizacaoColetaEntrega.Coleta);
                if (microRegiaoRoteirizacao?.Count > 0)
                {
                    filiaisComMicroRegiaoRoteirizacao = (from item in microRegiaoRoteirizacao select item.Filial.Codigo).Distinct().ToList();

                    foreach (var local in microRegiaoRoteirizacao)
                    {
                        //Contem mais de uma microregião
                        var areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(local.Area);

                        foreach (var area in areas)
                        {
                            //Agora vamos pegar todos os pedidos contidos na area.
                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosMicroRegiao = null;

                            switch (area.type)
                            {
                                case "circle":
                                    var pontoRaio = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.center.lat, Longitude = area.center.lng };
                                    pedidosMicroRegiao = pedidos.FindAll(x => Logistica.Distancia.ValidarNoRaio(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                    {
                                        Latitude = ObterLatitudeOuLongitudePedido(x, true, localizacaoColeta),     // ObterLatitudeOuLongitude(x.Destinatario?.Latitude),
                                        Longitude = ObterLatitudeOuLongitudePedido(x, false, localizacaoColeta)    // ObterLatitudeOuLongitude(x.Destinatario?.Longitude)
                                    }, pontoRaio, area.radius / 1000)).ToList();
                                    break;
                                case "rectangle":
                                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> listaPontosRetangulo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.east });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.west });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.west });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.east });
                                    pedidosMicroRegiao = pedidos.FindAll(x => Logistica.Distancia.ValidarPoligono(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                    {
                                        Latitude = ObterLatitudeOuLongitudePedido(x, true, localizacaoColeta),     // ObterLatitudeOuLongitude(x.Destinatario?.Latitude),
                                        Longitude = ObterLatitudeOuLongitudePedido(x, false, localizacaoColeta)    // ObterLatitudeOuLongitude(x.Destinatario?.Longitude)
                                    }, listaPontosRetangulo.ToArray())).ToList();
                                    break;
                                case "polygon":
                                    var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                                    foreach (var ponto in area.paths)
                                        listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ponto.lat, Longitude = ponto.lng });
                                    pedidosMicroRegiao = pedidos.FindAll(x => Logistica.Distancia.ValidarPoligono(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                    {
                                        Latitude = ObterLatitudeOuLongitudePedido(x, true, localizacaoColeta),     // ObterLatitudeOuLongitude(x.Destinatario?.Latitude),
                                        Longitude = ObterLatitudeOuLongitudePedido(x, false, localizacaoColeta)    // ObterLatitudeOuLongitude(x.Destinatario?.Longitude)
                                    }, listaPontos.ToArray())).ToList();
                                    break;
                            }
                            //Existe pedidos dentro de uma micro região de roteirização
                            if (pedidosMicroRegiao?.Count > 0)
                            {
                                pedidosMicroRegiao = pedidosMicroRegiao.FindAll(x => !listaGrupoPedidos.Exists(c => c.Pedidos.Any(p => p.Codigo == x.Codigo))).ToList();

                                if (pedidosMicroRegiao.Count > 0)
                                {
                                    ObterGruposPedidos(centrosCarregamento, ref pedidosMicroRegiao, ref listaGrupoPedidos, ref erro, ref msgAviso, repTipoCargaModeloVeicular, ref sessaoRoteirizadorPedidosResultado, ref maiorCapacidadeVeicular, sessaoRoteirizador, sessaoRoteirizadorParametros, disponibilidadeDiaUtilizar, linhasSeparacaoAgrupa);  //obterGrupoPedidosParametros); //  centrosCarregamento, disponibilidadeDia, ref pedidosMicroRegiao, ref listaGrupoPedidos, ref erro, ref msgAviso, repTipoCargaModeloVeicular, ref sessaoRoteirizadorPedidosResultado, ref maiorCapacidadeVeicular, sessaoRoteirizador, sessaoRoteirizadorParametros, disponibilidadeDiaUtilizar, linhasSeparacaoAgrupa);

                                    List<int> codigosPedidosMicroRegiao = (from item in pedidosMicroRegiao select item.Codigo).ToList();

                                    pedidosEmMicroRegiao.AddRange(codigosPedidosMicroRegiao);

                                    if (!string.IsNullOrEmpty(erro) && listaGrupoPedidos.Count == 0)
                                        return new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();
                                }
                            }
                        }
                    }
                }

                //Filtrar pedidos que não estão adicionados no retorno
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> demaisPedidos = pedidos.FindAll(x => !listaGrupoPedidos.Exists(c => c.Pedidos.Any(p => p.Codigo == x.Codigo))).ToList();

                //Se não misturar pedidos de dentro com fora de micro-regiões.
                demaisPedidos = demaisPedidos.FindAll(x => !pedidosEmMicroRegiao.Any(p => p == x.Codigo)).ToList();

                if (demaisPedidos?.Count > 0)
                {
                    ObterGruposPedidos(centrosCarregamento, ref demaisPedidos, ref listaGrupoPedidos, ref erro, ref msgAviso, repTipoCargaModeloVeicular, ref sessaoRoteirizadorPedidosResultado, ref maiorCapacidadeVeicular, sessaoRoteirizador, sessaoRoteirizadorParametros, disponibilidadeDiaUtilizar, linhasSeparacaoAgrupa);

                    if (!string.IsNullOrEmpty(erro) && listaGrupoPedidos.Count == 0)
                        return new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();
                }

                return listaGrupoPedidos;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> ObterGruposPedidosSimulaFrete(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros, ref string erro)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();

            MontagemCargaSimuladorFrete servicoMontagemCargaSimuladorFrete = new MontagemCargaSimuladorFrete(this.ObterConfiguracaoEmbarcador(), this.ObterConfiguracaoIntegracao(), _tipoServicoMultisoftware, _stringConexao, _unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos = servicoMontagemCargaSimuladorFrete.ValidaMontagemCarregamentoBlocos(parametros);

            if ((montagemCarregamentoBlocos?.Count ?? 0) == 0)
            {
                erro = "Não foi possível gerar nenhum bloco de carregamento para simulação de frete.";
                return resultado;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = servicoMontagemCargaSimuladorFrete.GerarMontagemCarregamentoBlocoSimuladorFretePorBloco(parametros, montagemCarregamentoBlocos, ref erro);

            if ((montagemCarregamentoBlocoSimuladorFretes?.Count ?? 0) == 0)
            {
                erro = "Não foi possível gerar nenhuma simulação de frete para gerar carregamentos. " + erro;
                return resultado;
            }

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> vencedoresBlocoSimuladorFretes = servicoMontagemCargaSimuladorFrete.GerarVencedoresBlocoSimuladorFrete(montagemCarregamentoBlocos, montagemCarregamentoBlocoSimuladorFretes);

            resultado = servicoMontagemCargaSimuladorFrete.GerarMontagemCargaGrupoPedido(vencedoresBlocoSimuladorFretes);

            return resultado;
        }

        private void GerarSimulacaoFreteCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros parametros, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDoCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador simuladorFreteCriterioSelecaoTransportador, int codigoCentroCarregamento, int distanciaKm, ref string erro, bool vencedorSimuladorFreteEmpresaPedido = false)
        {
            MontagemCargaSimuladorFrete servicoMontagemCargaSimuladorFrete = new MontagemCargaSimuladorFrete(this.ObterConfiguracaoEmbarcador(), this.ObterConfiguracaoIntegracao(), _tipoServicoMultisoftware, _stringConexao, _unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos = servicoMontagemCargaSimuladorFrete.ValidaCarregamentoBlocos(parametros, carregamento, pedidosDoCarregamento, codigoCentroCarregamento);

            if ((montagemCarregamentoBlocos?.Count ?? 0) == 0)
            {
                erro = "Não foi possível gerar nenhum bloco de carregamento para simulação de frete.";
                return;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = servicoMontagemCargaSimuladorFrete.GerarSimuladorFretePorCarregamento(parametros, montagemCarregamentoBlocos, carregamento, pedidosDoCarregamento, distanciaKm, ref erro);

            if ((montagemCarregamentoBlocoSimuladorFretes?.Count ?? 0) == 0)
            {
                erro = "Não foi possível gerar nenhuma simulação de frete para gerar carregamentos. " + erro;
                return;
            }

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> vencedorBlocoSimuladorFretes = servicoMontagemCargaSimuladorFrete.GerarVencedoresBlocoSimuladorFrete(montagemCarregamentoBlocos, montagemCarregamentoBlocoSimuladorFretes, simuladorFreteCriterioSelecaoTransportador);

            //Devemos relacionar o vencedor ao carregamento... ;)
            if (vencedorBlocoSimuladorFretes.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete vencedor = vencedorBlocoSimuladorFretes[0];
                if (vencedorSimuladorFreteEmpresaPedido && carregamento.Empresa != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete simulouEmpresaCarregamento = (from o in vencedorBlocoSimuladorFretes
                                                                                                                                            where o.Transportador.Codigo == carregamento.Empresa.Codigo && o.ValorTotal > 0
                                                                                                                                            select o).FirstOrDefault();
                    if (simulouEmpresaCarregamento != null)
                    {
                        Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(_unitOfWork);
                        vencedor.Vencedor = false;
                        repositorioMontagemCarregamentoBlocoSimuladorFrete.Atualizar(vencedor);
                        vencedor = simulouEmpresaCarregamento;
                        vencedor.Vencedor = true;
                        repositorioMontagemCarregamentoBlocoSimuladorFrete.Atualizar(vencedor);
                    }
                }
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
                carregamento.Empresa = vencedor.Transportador;
                carregamento.ValorFrete = vencedor.ValorTotalSimulacao;
                carregamento.MontagemCarregamentoBloco = (from obj in montagemCarregamentoBlocos where obj.Transportador == carregamento.Empresa select obj).FirstOrDefault();
                repCarregamento.Atualizar(carregamento);
            }
        }

        /// <summary>
        /// Obtem as coordenadas do Destino.. Quando possuir recebedor, retorna recebedor, quando não possuir, retorna o destinatário.
        /// Para casos de coleta entrega, retorna a coordenada do "Remetente"
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="lat"></param>
        /// <param name="coletaEntrega"></param>
        /// <returns></returns>
        private double ObterLatitudeOuLongitudePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool lat, bool coletaEntrega)
        {
            if (coletaEntrega)
                return ObterLatitudeOuLongitude((lat ? pedido.Remetente.Latitude : pedido.Remetente.Longitude));

            if (lat)
            {
                if (pedido.Recebedor != null)
                    return ObterLatitudeOuLongitude(pedido.Recebedor.Latitude);
                else
                    return ObterLatitudeOuLongitude(!pedido.UsarOutroEnderecoDestino ? pedido.Destinatario.Latitude : pedido.EnderecoDestino.ClienteOutroEndereco.Latitude);
            }
            else
            {
                if (pedido.Recebedor != null)
                    return ObterLatitudeOuLongitude(pedido.Recebedor.Longitude);
                else
                    return ObterLatitudeOuLongitude(!pedido.UsarOutroEnderecoDestino ? pedido.Destinatario.Longitude : pedido.EnderecoDestino.ClienteOutroEndereco.Longitude);
            }
        }

        private double ObterLatitudeOuLongitude(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            return double.Parse(value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> ObterLinhasSeparacaoAgrupa(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa repositorioLinhaSeparacaoAgrupa = new Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa(_unitOfWork);

            return repositorioLinhaSeparacaoAgrupa.BuscarTodos(sessaoRoteirizador?.Filial?.Codigo ?? 0);
        }

        private List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> ObterModeloVeicularDisponibilidade(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, DateTime dataEntrega)
        {
            List<int> codigosCentroCarregamento = centrosCarregamento.Select(o => o.Codigo).ToList();
            Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota repositorioCentroCarregamentoDisponibilidadeFrota = new Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(dataEntrega);

            return repositorioCentroCarregamentoDisponibilidadeFrota.BuscarPorCentrosDeCarregamentoEDia(codigosCentroCarregamento, diaSemana);
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularDisponibilidade(int codigoFilial, int codigoTransportador, DateTime data_prev_carregamento, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, TipoOcupacaoMontagemCarregamentoVRP tipoOcupacaoMontagemCarregamento, ref string erro)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregametnoFilial = centrosCarregamento.Where(o => o.Filial.Codigo == codigoFilial).FirstOrDefault();

            if (centroCarregametnoFilial == null)
                return null;

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadesDiaCentro = disponibilidadeDia.Where(o =>
                o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                (o.Transportador?.Codigo ?? 0) == codigoTransportador
            ).OrderByDescending(o => o?.ModeloVeicular?.CapacidadePesoTransporte).ToList();

            if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                disponibilidadesDiaCentro = disponibilidadeDia.Where(o =>
                    o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                    (o.Transportador?.Codigo ?? 0) == codigoTransportador
                ).OrderByDescending(o => o?.ModeloVeicular?.Cubagem).ToList();
            else if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                disponibilidadesDiaCentro = disponibilidadeDia.Where(o =>
                    o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                    (o.Transportador?.Codigo ?? 0) == codigoTransportador
                ).OrderByDescending(o => o?.ModeloVeicular?.NumeroPaletes).ToList();

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota disponibilidadeDiaCentro = null;
            DateTime dataIni = data_prev_carregamento.Date;
            DateTime dataFim = dataIni.AddDays(1);

            foreach (var disponibilidade in disponibilidadesDiaCentro)
            {
                var quantidadeUtilizada = repCarregamento.BuscarQuantidadePorDataFilialeModeloVeicular(dataIni, dataFim, disponibilidade?.CentroCarregamento?.Filial?.Codigo ?? 0, disponibilidade?.ModeloVeicular?.Codigo ?? 0);
                var quantidadeUtilizando = (
                    from item in listaGrupoPedidos
                    where item?.ModeloVeicular?.Codigo == disponibilidade?.ModeloVeicular?.Codigo && item?.DataCarregamento == dataIni
                    select item
                ).Count();

                if (quantidadeUtilizada + quantidadeUtilizando < disponibilidade.Quantidade)
                {
                    disponibilidadeDiaCentro = disponibilidade;
                    break;
                }
            }

            //Vamos ver se não existe outra disponibilidade para o centro.
            bool sem_frota = true;
            if (disponibilidadeDiaCentro == null && disponibilidadeDia.Count > disponibilidadesDiaCentro.Count)
                sem_frota = false;

            if (sem_frota && (centroCarregametnoFilial != null) && (centroCarregametnoFilial.DisponibilidadesFrota.Count > 0) && (disponibilidadeDiaCentro == null))
                erro = $"Centro de carregamento {centroCarregametnoFilial.Descricao} não tem disponibilidade para esta esta data";

            return disponibilidadeDiaCentro?.ModeloVeicular;
        }

        /// <summary>
        /// Procedimento para verificar o Menor Modelo veicular disponível para o peso máximo agrupado para os pedidos
        /// </summary>
        /// <param name="codigoFilial">Código da filial de carregamento/sessão roteirizador</param>
        /// <param name="codigoTransportador">Código do transportador em caso de grupo de pedido com rota por transportador</param>
        /// <param name="data_prev_carregamento">Data prevista do carregamento</param>
        /// <param name="centrosCarregamento">Centros de carregamento da filial</param>
        /// <param name="disponibilidadeDia">Disponibilida de frota</param>
        /// <param name="listaGrupoPedidos">Lista dos grupos de pedidos já separador para gerar os carregamentos</param>
        /// <param name="pesoTotalPedidos">Peso total da carga....</param>
        /// <param name="_unitOfWork"></param>
        /// <returns></returns>
        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularDisponibilidadeMaiorMenor(int codigoFilial, int codigoTransportador, DateTime data_prev_carregamento, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, decimal pesoTotalPedidos, TipoOcupacaoMontagemCarregamentoVRP tipoOcupacaoMontagemCarregamento, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidosProdutos)
        {

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregametnoFilial = centrosCarregamento.Where(o => o.Filial.Codigo == codigoFilial).FirstOrDefault();

            if (centroCarregametnoFilial == null)
                return null;

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadesDiaCentro = disponibilidadeDia.Where(
                o => o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                (o.Transportador?.Codigo ?? 0) == codigoTransportador
            ).OrderByDescending(o => o?.ModeloVeicular?.CapacidadePesoTransporte).ToList();

            if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                disponibilidadesDiaCentro = disponibilidadeDia.Where(o =>
                    o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                    (o.Transportador?.Codigo ?? 0) == codigoTransportador
                ).OrderByDescending(o => o?.ModeloVeicular?.Cubagem).ToList();
            else if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                disponibilidadesDiaCentro = disponibilidadeDia.Where(o =>
                    o.CentroCarregamento?.Codigo == centroCarregametnoFilial?.Codigo &&
                    (o.Transportador?.Codigo ?? 0) == codigoTransportador
                ).OrderByDescending(o => o?.ModeloVeicular?.NumeroPaletes).ToList();

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> tmp = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();
            DateTime dataIni = data_prev_carregamento.Date;
            DateTime dataFim = dataIni.AddDays(1);

            foreach (var disponibilidade in disponibilidadesDiaCentro)
            {
                var quantidadeUtilizada = repCarregamento.BuscarQuantidadePorDataFilialeModeloVeicular(dataIni, dataFim, disponibilidade?.CentroCarregamento?.Filial?.Codigo ?? 0, disponibilidade?.ModeloVeicular?.Codigo ?? 0);
                var quantidadeUtilizando = (
                    from item in listaGrupoPedidos
                    where item?.ModeloVeicular?.Codigo == disponibilidade?.ModeloVeicular?.Codigo && item?.DataCarregamento == dataIni
                    select item
                ).Count();

                if (quantidadeUtilizada + quantidadeUtilizando < disponibilidade.Quantidade)
                    tmp.Add(disponibilidade);
            }

            if (tmp.Count == 0)
                return null;
            else if (tmp.Count == 1)
                return tmp[0]?.ModeloVeicular;
            else
            {
                tmp = tmp.OrderBy(x => x?.ModeloVeicular?.CapacidadePesoTransporte).ToList();
                if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                    tmp = tmp.OrderBy(x => x?.ModeloVeicular?.Cubagem).ToList();
                else if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                    tmp = tmp.OrderBy(x => x?.ModeloVeicular?.QuantidadePaletes).ToList();

                if (pedidosProdutos == null) pedidosProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                decimal cubagemProdutos = (from o in pedidosProdutos select o.MetroCubicoPedidoProduto).Sum();
                decimal palletProdutos = (from o in pedidosProdutos select o.QuantidadePalletPedidoProduto).Sum();

                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico && cubagemProdutos > 0 && tmp[i]?.ModeloVeicular?.Cubagem > cubagemProdutos)
                        return tmp[i]?.ModeloVeicular;
                    else if (tipoOcupacaoMontagemCarregamento == TipoOcupacaoMontagemCarregamentoVRP.Pallet && palletProdutos > 0 && tmp[i]?.ModeloVeicular?.QuantidadePaletes > palletProdutos)
                        return tmp[i]?.ModeloVeicular;
                    else if (tmp[i]?.ModeloVeicular?.CapacidadePesoTransporte > pesoTotalPedidos && ((cubagemProdutos + palletProdutos == 0) || ((tmp[i]?.ModeloVeicular?.Cubagem > cubagemProdutos || tmp[i]?.ModeloVeicular?.Cubagem == 0) && (tmp[i]?.ModeloVeicular?.QuantidadePaletes > palletProdutos || tmp[i]?.ModeloVeicular?.QuantidadePaletes == 0))))
                        return tmp[i]?.ModeloVeicular;
                }
                return null;
            }
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidos(List<int> codigosPedido)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            return repositorioPedido.BuscarPorCodigosComOutroEndereco(codigosPedido);
        }

        private void ObterPedidoProdutoCargaSimples(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGrupo, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidos, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarga, List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tiposCargaModeloVeicular, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupados)
        {
            List<double> destinatarios = (from destino in pedidosGrupo
                                          where destino.Destinatario != null
                                          select destino.Destinatario.CPF_CNPJ).Distinct().ToList();

            foreach (double destinatario in destinatarios)
            {
                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescargaDestinatario = centrosDescarga.Where(x => x.Destinatario.CPF_CNPJ == destinatario).ToList();
                if ((centrosDescargaDestinatario == null) || (centrosDescargaDestinatario.Count == 0))
                    continue;

                //Vamos pegar o maior modelo veicular dos centros de descarga...
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescargaDestinatario = centrosDescargaDestinatario.Where(x => x.VeiculosPermitidos?.Count > 0).FirstOrDefault();

                //Aki, vamos validar a disponibilidade de carros, pegar o que dá maior ocupação... (pegar o menor maior que o peso...)
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidosDestinatario = (from modelo in centroDescargaDestinatario.VeiculosPermitidos
                                                                                                               select modelo).OrderByDescending(p => p.CapacidadePesoTransporte).ToList();

                modelosPermitidosDestinatario = modelosPermitidosDestinatario.Where(x => tiposCargaModeloVeicular.Any(t => t.ModeloVeicularCarga.Codigo == x.Codigo)).ToList();

                if ((modelosPermitidosDestinatario == null) || (modelosPermitidosDestinatario.Count == 0))
                    continue;

                // Agora vamos obter os pedidos deste detinatário...
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDestino = (from obj in pedidosGrupo
                                                                                    where obj.Destinatario.CPF_CNPJ == destinatario
                                                                                    select obj).ToList();

                bool resumida = false;
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> filaCarregamentoPedidosCanalEntregaLinhaSeparacao = ObterFilaPrioridadesPedidosProdutos(pedidosDestino, sessaoRoteirizador, ref resumida);


                //Agora vamos carregar..
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento item in filaCarregamentoPedidosCanalEntregaLinhaSeparacao)
                {
                    //Obtendo todos os pedidos do canal de entrega....
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDaVez = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    //Consultado todos os produtos dos pedidos do canal de entrega.
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                    if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido)
                    {
                        ////pedidosDaVez = (from ped in pedidosDestino where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        ////Consultado todos os produtos dos pedidos do canal de entrega.
                        //produtos = this.ObterPedidosProdutos((from ped in pedidosDestino select ped.Codigo).ToList());

                        ////Filtrando os pedidos somente do endereço de entrega da fila...
                        //produtos = (from obj in produtos
                        //            where (obj?.EnderecoProduto?.Codigo ?? 0) == item.CodigoEnderecoProduto
                        //            select obj).ToList();

                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDestino, item, sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.EnderecoProduto?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.EnderecoProduto?.Codigo ?? 0)
                                           .ThenBy(x => x.Pedido.DataCriacao).ToList();
                    }
                    else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido)
                    {
                        pedidosDaVez = (from ped in pedidosDestino where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        //Consultado todos os produtos dos pedidos do canal de entrega.
                        // produtos = this.ObterPedidosProdutos((from ped in pedidosDaVez select ped.Codigo).ToList());
                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDaVez, item, sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.Pedido?.CanalEntrega?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.Pedido?.CanalEntrega?.CodigoIntegracao ?? "")
                                           .ThenBy(x => x.EnderecoProduto?.NivelPrioridade ?? 999)
                                           .ThenBy(x => x.EnderecoProduto?.Codigo ?? 0)
                                           .ThenBy(x => x.Pedido.DataCriacao).ToList();
                    }
                    else if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto == PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto)
                    {
                        pedidosDaVez = (from ped in pedidosDestino where (ped?.CanalEntrega?.Codigo ?? 0) == item.CodigoCanalEntrega select ped).ToList();
                        //Consultado todos os produtos dos pedidos do canal de entrega.
                        //produtos = this.ObterPedidosProdutos((from ped in pedidosDaVez select ped.Codigo).ToList());

                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDaVez, item, sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.LinhaSeparacao?.NivelPrioridade ?? 999).ThenBy(x => x.Produto.Codigo).ToList();
                    }
                    else
                    {
                        //produtos = this.ObterPedidosProdutos((from ped in pedidosDestino select ped.Codigo).ToList());
                        //Agora.. vamos filtrar somente os produtos dos pedidos da linha de separação ordenada....
                        //produtos = (from pro in produtos where (pro?.LinhaSeparacao?.Codigo ?? 0) == item.CodigoLinhaSeparacao select pro).ToList();

                        produtos = ObterProdutosDeAcordoFilaPrioridade(pedidosDestino, item, sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto);

                        // Ordenandos os produtos pelos pedidos mais antigos...
                        produtos = produtos.OrderBy(x => x.Pedido?.CanalEntrega?.NivelPrioridade ?? 999).ThenBy(x => x.Pedido?.CanalEntrega?.Codigo ?? 999).ToList();
                    }

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in produtos)
                    {
                        //Agora.. vamos validar se o total do produto.. possui algum modelo veicular que atende....
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = (from obj in modelosPermitidos
                                                                                                       where (!obj.VeiculoPaletizado || (obj.NumeroPaletes >= produto.QuantidadePalet &&
                                                                                                                                         obj.NumeroPaletes * (decimal)0.95 <= produto.QuantidadePalet)) &&
                                                                                                             (!obj.ModeloControlaCubagem || (obj.Cubagem >= produto.MetroCubico &&
                                                                                                                                             obj.Cubagem * (decimal)0.95 <= produto.MetroCubico)) &&
                                                                                                             obj.CapacidadePesoTransporte >= produto.PesoTotal &&
                                                                                                             obj.CapacidadePesoTransporte * (decimal)0.95 <= produto.PesoTotal
                                                                                                       select obj).FirstOrDefault();

                        //Se localizou algum modelo veicular para uma carga denominada "Simples"
                        if (modeloVeicularCarga != null)
                        {
                            DateTime dataCarregamento = (from p in pedidosDestino
                                                         select p.DataCarregamentoPedido)?.Min() ?? DateTime.Now;

                            if (dataCarregamento.Date < DateTime.Now.Date)
                                dataCarregamento = DateTime.Now;

                            Dictionary<int, decimal> pedidosPesosCarregamento = new Dictionary<int, decimal>() { { produto.Pedido.Codigo, produto.PesoTotal } };
                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPeso = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { produto.Pedido };

                            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido carregamentoGrupoPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido
                            {
                                Pedidos = pedidosPeso,
                                ModeloVeicular = modeloVeicularCarga,
                                CodigoFilial = produto.Pedido?.Filial?.Codigo ?? 0,
                                Transportador = null,
                                DataCarregamento = dataCarregamento.Date,
                                PedidosPesos = pedidosPesosCarregamento,
                                Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto()
                                    {
                                         CodigoLinhaSeparacao = produto?.LinhaSeparacao?.Codigo ?? 0,
                                         CodigoPedido = produto.Pedido.Codigo,
                                         CodigoPedidoProduto = produto.Codigo,
                                         MetroCubicoPedidoProduto = produto.MetroCubico,
                                         PesoPedidoProduto = produto.PesoTotal,
                                         QuantidadePalletPedidoProduto = produto.QuantidadePalet,
                                         QuantidadePedidoProduto = produto.Quantidade
                                    }
                                }
                            };
                            agrupados.Add(carregamentoGrupoPedido);

                            RemoverLista(pedidosGrupo, pedidosPeso, pedidosPesosCarregamento, agrupados, sessaoRoteirizador);

                            //Filtrando os pedidos que já não possuem uma validação de retorno..
                            // e não pode existir nos agrupados.. pois em carregamentos de produtos pallet < 1 não quebra.. e pode estar carregando outro produto
                            // contudo não devemos remover da lista para que o pedido vá no próximo carregamento
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> temp = sessaoRoteirizadorPedidosResultado;
                            temp.RemoveAll(x => agrupados.Exists(r => r.Pedidos.Exists(p => p.Codigo == x.Pedido.Codigo)));
                            pedidosGrupo = pedidosGrupo.FindAll(x => !temp.Exists(r => r.Pedido.Codigo == x.Codigo));
                        }
                    }
                }
            }
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosPorPeso(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularAtual, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, ref Dictionary<int, decimal> pedidosPesosCarregamento, ref List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> montagemCargaGrupoPedidoProdutos, ref string msgAviso, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar nivelQuebraProdutoRoteirizar, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarga, List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tiposCargaModeloVeicular, bool tipoCargaPaletizado, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP tipoMontagemCarregamento = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP : centroCarregamento?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP tipoOcupacao = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoOcupacaoMontagemCarregamentoVRP : centroCarregamento?.TipoOcupacaoMontagemCarregamentoVRP ?? TipoOcupacaoMontagemCarregamentoVRP.Peso);
            bool dispFrotaCentroDescCliente = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente : centroCarregamento?.UtilizarDispFrotaCentroDescCliente ?? false);

            bool agruparPedidosMesmoDestinatario = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.AgruparPedidosMesmoDestinatario : centroCarregamento?.AgruparPedidosMesmoDestinatario ?? false);

            int qtdeMaxEntregasCentroCarregamento = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar : centroCarregamento?.QuantidadeMaximaEntregasRoteirizar ?? 100);
            if (qtdeMaxEntregasCentroCarregamento == 0) qtdeMaxEntregasCentroCarregamento = 100;

            if (tipoMontagemCarregamento == TipoMontagemCarregamentoVRP.Prioridades && modeloVeicularAtual != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento> temposCarregamentoCentro = null;
                if ((sessaoRoteirizadorParametros?.TemposCarregamento?.Count ?? 0) > 0)
                    temposCarregamentoCentro = sessaoRoteirizadorParametros.TemposCarregamento;
                else
                    temposCarregamentoCentro = (from item in centroCarregamento.TemposCarregamento.ToList()
                                                select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento()
                                                {
                                                    Codigo = item.Codigo,
                                                    CodigoModeloVeicular = item.ModeloVeicular?.Codigo ?? 0,
                                                    DescricaoModeloVeicular = item.ModeloVeicular?.Descricao ?? string.Empty,
                                                    CodigoTipoCarga = item.TipoCarga?.Codigo ?? 0,
                                                    DescricaoTipoCarga = item.TipoCarga?.Descricao ?? string.Empty,
                                                    Quantidade = item.QuantidadeMaximaEntregasRoteirizar,
                                                    QuantidadeUtilizar = item.QuantidadeMaximaEntregasRoteirizar
                                                }).ToList();

                //Localizar o máximo da entregas do modelo...
                if (temposCarregamentoCentro != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento tempoModelo = (from obj in temposCarregamentoCentro
                                                                                                                                       where obj.CodigoModeloVeicular == modeloVeicularAtual.Codigo
                                                                                                                                       select obj).FirstOrDefault();

                    if ((tempoModelo?.QuantidadeUtilizar ?? 0) > 0)
                        qtdeMaxEntregasCentroCarregamento = tempoModelo.QuantidadeUtilizar;
                }
            }

            DateTime dataPrevisaoEntregaNull = DateTime.Now.Date.AddYears(50);

            if (tipoMontagemCarregamento == TipoMontagemCarregamentoVRP.Prioridades)
            {
                if (sessaoRoteirizador.PrioridadeMontagemCarregamentoPedido == PrioridadeMontagemCarregamentoPedido.CanalEntregaPrevisaoEntrega)
                    pedidos = pedidos.OrderBy(z => z.CanalEntrega?.NivelPrioridade ?? 999).ThenBy(z => z.PrevisaoEntrega ?? dataPrevisaoEntregaNull).ThenByDescending(o => o.PesoTotal).ToList();
                else
                    pedidos = pedidos.OrderBy(z => z.PrevisaoEntrega ?? dataPrevisaoEntregaNull).ThenBy(z => z.CanalEntrega?.NivelPrioridade ?? 999).ThenByDescending(o => o.PesoTotal).ToList();
            }
            else
                pedidos = pedidos.OrderBy(z => z.Destinatario.Codigo).ThenByDescending(o => o.PesoTotal).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> retornoPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            if (pedidos.Count == 0)
                return retornoPedidos;

            if (tipoMontagemCarregamento == TipoMontagemCarregamentoVRP.Prioridades)
            {
                int contadorEntregas = 0;

                decimal peso = 0;
                decimal pallet = 0;
                decimal cubagem = 0;

                decimal pesoMaximo = modeloVeicularAtual.CapacidadePesoTransporte;
                decimal palletMaximo = (!modeloVeicularAtual.VeiculoPaletizado ? 0 : modeloVeicularAtual.NumeroPaletes ?? 0);
                decimal ocupacaoCubicaPaletes = (tipoCargaPaletizado ? modeloVeicularAtual.ObterOcupacaoCubicaPaletes() : 0m);
                decimal cubagemMaximo = (!modeloVeicularAtual.ModeloControlaCubagem ? 0 : modeloVeicularAtual.Cubagem - ocupacaoCubicaPaletes);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarregar = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    if (!agruparPedidosMesmoDestinatario)
                        pedidosCarregar.Add(pedido);
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTemp = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                        if (pedido.Recebedor != null)
                            pedidosTemp = (from ped in pedidos
                                           where ped.Recebedor != null && ped.Recebedor.Codigo == pedido.Recebedor.Codigo &&
                                                 (ped.CanalEntrega?.NivelPrioridade ?? 999) == (pedido.CanalEntrega?.NivelPrioridade ?? 999) &&
                                                 (ped.PrevisaoEntrega ?? dataPrevisaoEntregaNull) == (pedido.PrevisaoEntrega ?? dataPrevisaoEntregaNull)
                                           select ped).ToList();
                        else
                            pedidosTemp = (from ped in pedidos
                                           where ped.Recebedor == null && ped.Destinatario.Codigo == pedido.Destinatario.Codigo &&
                                                 (ped.CanalEntrega?.NivelPrioridade ?? 999) == (pedido.CanalEntrega?.NivelPrioridade ?? 999) &&
                                                 (ped.PrevisaoEntrega ?? dataPrevisaoEntregaNull) == (pedido.PrevisaoEntrega ?? dataPrevisaoEntregaNull)
                                           select ped).ToList();

                        //Remover os pedidos que já estão foram utilizados....
                        pedidosTemp.RemoveAll(x => retornoPedidos.Any(y => y.Codigo == x.Codigo));
                    }

                    if (pedidosCarregar.Count > 0)
                    {
                        bool excedeCapacidadeVeiculo = false;
                        decimal totalPesoPedidosDestino = pedidosCarregar.Sum(x => x.PesoTotal);
                        decimal totalCubagemPedidosDestino = pedidosCarregar.Sum(x => x.CubagemTotal);
                        decimal totalPalletPedidosDestino = pedidosCarregar.Sum(x => x.TotalPallets);

                        decimal pesosDesconsiderar = (from obj in pedidosCarregar
                                                      where obj.CanalEntrega != null && obj.CanalEntrega.NaoUtilizarCapacidadeVeiculoMontagemCarga
                                                      select obj.PesoTotal).Sum();

                        decimal cubagemDesconsiderar = (from obj in pedidosCarregar
                                                        where obj.CanalEntrega != null && obj.CanalEntrega.NaoUtilizarCapacidadeVeiculoMontagemCarga
                                                        select obj.CubagemTotal).Sum();

                        decimal palletsDesconsiderar = (from obj in pedidosCarregar
                                                        where obj.CanalEntrega != null && obj.CanalEntrega.NaoUtilizarCapacidadeVeiculoMontagemCarga
                                                        select obj.TotalPallets).Sum();

                        totalPesoPedidosDestino -= pesosDesconsiderar;
                        totalCubagemPedidosDestino -= cubagemDesconsiderar;
                        totalPalletPedidosDestino -= palletsDesconsiderar;

                        if (tipoOcupacao == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                            excedeCapacidadeVeiculo = (totalCubagemPedidosDestino > cubagemMaximo);
                        else if (tipoOcupacao == TipoOcupacaoMontagemCarregamentoVRP.Pallet)
                            excedeCapacidadeVeiculo = (totalPalletPedidosDestino > palletMaximo);
                        else
                            excedeCapacidadeVeiculo = (totalPesoPedidosDestino > pesoMaximo);

                        if (excedeCapacidadeVeiculo)
                        {
                            SetarSessaoRoteirizadorPedidosResultado(pedidosCarregar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular, ref sessaoRoteirizadorPedidosResultado);
                            continue;
                        }

                        bool consegueCarregarTodosDestinatario = false;
                        if (tipoOcupacao == TipoOcupacaoMontagemCarregamentoVRP.Peso)
                        {
                            if (peso + totalPesoPedidosDestino <= pesoMaximo)
                                consegueCarregarTodosDestinatario = true;
                        }
                        else if (tipoOcupacao == TipoOcupacaoMontagemCarregamentoVRP.MetroCubico)
                        {
                            if (cubagem + totalCubagemPedidosDestino <= cubagemMaximo)
                                consegueCarregarTodosDestinatario = true;
                        }
                        else
                        {
                            if (pallet + totalPalletPedidosDestino <= palletMaximo)
                                consegueCarregarTodosDestinatario = true;
                        }

                        if (!consegueCarregarTodosDestinatario)
                            continue;

                        for (int t = 0; t < pedidosCarregar.Count; t++)
                        {
                            if ((pedidosCarregar[t].CanalEntrega?.Codigo ?? 0) > 0 && (pedidosCarregar[t].CanalEntrega?.QuantidadePedidosPermitidosNoCanal ?? 0) > 0)
                            {
                                int qtdeCarregadoCanal = (from p in retornoPedidos
                                                          where p.CanalEntrega.Codigo == pedidosCarregar[t].CanalEntrega?.Codigo
                                                          select p).Count();

                                int totalCanalDestinatario = (from c in pedidosCarregar
                                                              where c.CanalEntrega.Codigo == pedidosCarregar[t].CanalEntrega?.Codigo
                                                              select c).Count();

                                if (qtdeCarregadoCanal + totalCanalDestinatario > pedidosCarregar[t].CanalEntrega.QuantidadePedidosPermitidosNoCanal)
                                {
                                    consegueCarregarTodosDestinatario = false;
                                    break;
                                }
                            }
                        }

                        if (!consegueCarregarTodosDestinatario)
                            continue;

                        peso += totalPesoPedidosDestino;
                        pallet += totalPalletPedidosDestino;
                        cubagem += totalCubagemPedidosDestino;
                        retornoPedidos.AddRange(pedidosCarregar);
                        foreach (var pedd in pedidosCarregar)
                            pedidosPesosCarregamento[pedd.Codigo] = pedd.PesoTotal;

                        contadorEntregas++;
                        if (contadorEntregas >= qtdeMaxEntregasCentroCarregamento)
                            break;

                        if ((pesoMaximo * (decimal)0.95) < peso)
                            break;
                        if (palletMaximo > 0 && pallet >= palletMaximo * (decimal)0.95)
                            break;
                        if (cubagemMaximo > 0 && cubagem >= cubagemMaximo * (decimal)0.95)
                            break;
                    }
                }
            }
            else
            {
                var destinos = (from pedido in pedidos
                                select new
                                {
                                    pedido.Destinatario?.Codigo,
                                    pedido.Destinatario?.CPF_CNPJ,
                                    Latitude = double.Parse(pedido.Destinatario?.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),
                                    Longitude = double.Parse(pedido.Destinatario?.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)
                                }).Distinct().ToList();

                var remetente = (from ped in pedidos
                                 where ped.Remetente != null
                                 select ped.Remetente).FirstOrDefault();

                var lat = remetente == null ? 0 : double.Parse(remetente?.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                var lng = remetente == null ? 0 : double.Parse(remetente?.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                //Ordenando pelos pedidos mais próximos do remetente...
                destinos = destinos.OrderBy(x => Logistica.Polilinha.CalcularDistancia(lat, lng, x.Latitude, x.Longitude)).ToList();

                int contDestinatarios = 0;

                decimal peso = 0;
                decimal pallet = 0;
                decimal cubagem = 0;

                foreach (var destino in destinos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDestino = pedidos.Where(x => x.Destinatario?.Codigo == destino.Codigo &&
                                                                                                          !retornoPedidos.Exists(c => c.Codigo == x.Codigo)).ToList();

                    //Achar o modelo veicular permitido do centro de descarga..
                    if (dispFrotaCentroDescCliente && centrosDescarga != null)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescargaDestinatario = centrosDescarga.Where(x => x.Destinatario.CPF_CNPJ == destino.CPF_CNPJ).ToList();
                        if ((centrosDescargaDestinatario == null) || (centrosDescargaDestinatario.Count == 0))
                        {
                            SetarSessaoRoteirizadorPedidosResultado(pedidosDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.CentroDescarregamentoSemVeiculoPermitido, ref sessaoRoteirizadorPedidosResultado);
                            continue; //return retornoPedidos;
                        }

                        //Vamos pegar o maior modelo veicular dos centros de descarga...
                        Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescargaDestinatario = centrosDescargaDestinatario.Where(x => x.VeiculosPermitidos?.Count > 0).FirstOrDefault();
                        if (centroDescargaDestinatario == null)
                        {
                            SetarSessaoRoteirizadorPedidosResultado(pedidosDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.CentroDescarregamentoSemVeiculoPermitido, ref sessaoRoteirizadorPedidosResultado);
                            continue; //return retornoPedidos;
                        }

                        //Aki, vamos validar a disponibilidade de carros, pegar o que dá maior ocupação... (pegar o menor maior que o peso...)
                        List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidosDestinatario = (from modelo in centroDescargaDestinatario.VeiculosPermitidos
                                                                                                                       select modelo).OrderByDescending(p => p.CapacidadePesoTransporte).ToList();

                        modelosPermitidosDestinatario = modelosPermitidosDestinatario.Where(x => tiposCargaModeloVeicular.Any(t => t.ModeloVeicularCarga.Codigo == x.Codigo)).ToList();

                        if ((modelosPermitidosDestinatario == null) || (modelosPermitidosDestinatario.Count == 0))
                        {
                            SetarSessaoRoteirizadorPedidosResultado(pedidosDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.CentroDescarregamentoSemVeiculoPermitido, ref sessaoRoteirizadorPedidosResultado);
                            continue; //return retornoPedidos;
                        }

                        modeloVeicularAtual = modelosPermitidosDestinatario[0];
                    }

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaParametros montagemCargaParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaParametros()
                    {
                        codigoSessaoRoteirizador = sessaoRoteirizador?.Codigo ?? 0,
                        tipoMontagemCarregamentoPedidoProduto = sessaoRoteirizador?.TipoMontagemCarregamentoPedidoProduto ?? TipoMontagemCarregamentoPedidoProduto.AMBOS,
                        prioridadeMontagemCarregamentoPedidoProduto = sessaoRoteirizador?.PrioridadeMontagemCarregamentoPedidoProduto ?? PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido,
                        linhasSeparacaoAgrupa = linhasSeparacaoAgrupa,
                        listaGrupoPedidos = listaGrupoPedidos,
                        nivelQuebraProdutoRoteirizar = nivelQuebraProdutoRoteirizar,
                        tipoStatusEstoqueMontagemCarregamentoPedidoProduto = sessaoRoteirizador?.TipoStatusEstoqueMontagemCarregamentoPedidoProduto ?? TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos
                    };
                    montagemCargaParametros.pesoMaximo = modeloVeicularAtual.CapacidadePesoTransporte;
                    montagemCargaParametros.palletMaximo = (!modeloVeicularAtual.VeiculoPaletizado ? 0 : modeloVeicularAtual.NumeroPaletes ?? 0);
                    decimal ocupacaoCubicaPaletes = (tipoCargaPaletizado ? modeloVeicularAtual.ObterOcupacaoCubicaPaletes() : 0m);
                    montagemCargaParametros.cubagemMaximo = (!modeloVeicularAtual.ModeloControlaCubagem ? 0 : modeloVeicularAtual.Cubagem - ocupacaoCubicaPaletes);

                    if (!sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                    {
                        var pedidosMaiorCapacidade = pedidosDestino.Where(x => x.PesoTotal > montagemCargaParametros.pesoMaximo);
                        foreach (var pedido in pedidosMaiorCapacidade)
                        {
                            if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular))
                                sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                {
                                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular,
                                    Pedido = pedido
                                });
                        }

                        pedidosDestino.RemoveAll(x => x.PesoTotal > montagemCargaParametros.pesoMaximo);

                        foreach (var pedido in pedidosDestino)
                        {
                            if (peso + pedido.PesoTotal <= montagemCargaParametros.pesoMaximo)
                            {
                                if (peso > 0 && ((montagemCargaParametros.pesoMaximo * (decimal)0.8) < (peso + pedido.PesoTotal)))
                                    break;
                                if (pallet > 0 && (montagemCargaParametros.palletMaximo > 0 && (pallet + pedido.NumeroPaletes) >= montagemCargaParametros.palletMaximo * (decimal)0.95))
                                    break;
                                if (cubagem > 0 && (montagemCargaParametros.cubagemMaximo > 0 && (cubagem + pedido.CubagemTotal) >= montagemCargaParametros.cubagemMaximo * (decimal)0.95))
                                    break;

                                peso += pedido.PesoTotal;
                                pallet += pedido.NumeroPaletes;
                                cubagem += pedido.CubagemTotal;
                                retornoPedidos.Add(pedido);
                                pedidosPesosCarregamento[pedido.Codigo] = pedido.PesoTotal;
                            }
                        }
                        if ((montagemCargaParametros.pesoMaximo * (decimal)0.8) < peso)
                            break;
                        if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.95)
                            break;
                        if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.95)
                            break;
                    }
                    else
                    {
                        //Ordenando so pedidos do destinatário por nível de prioridade
                        pedidosDestino = pedidosDestino.OrderBy(x => ((x?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : x?.CanalEntrega?.NivelPrioridade)).ToList();

                        bool resumida = false;
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> filaCarregamentoPedidosCanalEntregaLinhaSeparacao = ObterFilaPrioridadesPedidosProdutos(pedidosDestino, sessaoRoteirizador, ref resumida);
                        montagemCargaParametros.filaMontagemPedidoProdutoResumida = resumida;

                        ObterProdutosCarregarDeAcordoFilaPrioridades(filaCarregamentoPedidosCanalEntregaLinhaSeparacao, pedidosDestino, ref montagemCargaGrupoPedidoProdutos, montagemCargaParametros, ref peso, ref pallet, ref cubagem, ref sessaoRoteirizadorPedidosResultado, ref retornoPedidos, ref pedidosPesosCarregamento);

                        if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                            break;
                        if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                            break;
                        if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                            break;
                    }

                    contDestinatarios = (from pedRet in retornoPedidos
                                         select pedRet?.Destinatario?.Codigo ?? 0).Distinct().Count();

                    if (contDestinatarios == 0)
                        continue;

                    if (contDestinatarios >= qtdeMaxEntregasCentroCarregamento)
                        break;

                    //Localizando os vizinhos próximos...
                    var vizinhos = destinos.Where(x => x.Codigo != destino.Codigo)
                                           .OrderBy(x => Logistica.Polilinha.CalcularDistancia(destino.Latitude, destino.Longitude, x.Latitude, x.Longitude)).ToList();
                    foreach (var vizinho in vizinhos)
                    {
                        var pedVizinho = pedidos.Where(x => x.Destinatario?.Codigo == vizinho.Codigo &&
                                                            !retornoPedidos.Exists(c => c.Codigo == x.Codigo)).ToList();

                        // Antes da divisão de pedido
                        if (!sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                        {
                            var pedidosMaiorCapacidade = pedVizinho.Where(x => x.PesoTotal > montagemCargaParametros.pesoMaximo);
                            foreach (var pedido in pedidosMaiorCapacidade)
                            {
                                if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular))
                                    sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                    {
                                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular,
                                        Pedido = pedido
                                    });
                            }

                            pedVizinho.RemoveAll(x => x.PesoTotal > montagemCargaParametros.pesoMaximo);

                            var pesoVizinho = pedVizinho.Sum(x => x.PesoTotal);
                            var palletVizinho = pedVizinho.Sum(x => x.NumeroPaletes);
                            var cubagemVizinho = pedVizinho.Sum(x => x.CubagemTotal);
                            if (pesoVizinho + peso <= montagemCargaParametros.pesoMaximo)
                            {
                                peso += pesoVizinho;
                                pallet += palletVizinho;
                                cubagem += cubagemVizinho;
                                retornoPedidos.AddRange(pedVizinho);
                                for (int i = 0; i < pedVizinho.Count; i++)
                                    pedidosPesosCarregamento[pedVizinho[i].Codigo] = pedVizinho[i].PesoTotal;
                            }
                            if ((montagemCargaParametros.pesoMaximo * (decimal)0.8) < peso)
                                break;
                            if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.95)
                                break;
                            if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.95)
                                break;
                        }
                        else
                        {
                            var pedidosVizinho = pedidos.Where(x => x.Destinatario?.Codigo == vizinho.Codigo &&
                                                                    !retornoPedidos.Exists(c => c.Codigo == x.Codigo)).ToList();

                            //Ordenando so pedidos do destinatário por nível de prioridade
                            pedidosVizinho = pedidosVizinho.OrderBy(x => ((x?.CanalEntrega?.NivelPrioridade ?? 0) == 0 ? 999 : x?.CanalEntrega?.NivelPrioridade)).ToList();

                            bool resumida = false;
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaFilaPedidoProdutosCarregamento> filaCarregamentoPedidosCanalEntregaLinhaSeparacao = ObterFilaPrioridadesPedidosProdutos(pedidosVizinho, sessaoRoteirizador, ref resumida);
                            montagemCargaParametros.filaMontagemPedidoProdutoResumida = resumida;

                            ObterProdutosCarregarDeAcordoFilaPrioridades(filaCarregamentoPedidosCanalEntregaLinhaSeparacao, pedidosVizinho, ref montagemCargaGrupoPedidoProdutos, montagemCargaParametros, ref peso, ref pallet, ref cubagem, ref sessaoRoteirizadorPedidosResultado, ref retornoPedidos, ref pedidosPesosCarregamento);

                            if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                                break;
                            if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                                break;
                            if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                                break;
                        }

                        //Para cada visinho adicionado .. vamos validar o limite.
                        contDestinatarios = (from pedRet in retornoPedidos
                                             select pedRet?.Destinatario?.Codigo ?? 0).Distinct().Count();

                        if (contDestinatarios >= qtdeMaxEntregasCentroCarregamento)
                            break;

                    }

                    if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < peso)
                        break;
                    if (montagemCargaParametros.palletMaximo > 0 && pallet >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                        break;
                    if (montagemCargaParametros.cubagemMaximo > 0 && cubagem >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                        break;

                    contDestinatarios = (from pedRet in retornoPedidos
                                         select pedRet?.Destinatario?.Codigo ?? 0).Distinct().Count();

                    if (contDestinatarios >= qtdeMaxEntregasCentroCarregamento)
                        break;
                }

            }

            return retornoPedidos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> ObterPedidoProdutosPorPeso(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto, decimal pesoAtualCarregamento, decimal palletAtualCarregamento, decimal cubagemAtualCarregamento, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaParametros montagemCargaParametros, List<int> linhasSeparacaoCarregamento, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado, int codigoLinhaSeparacao, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> listaProdutosDestePedidoDesteGrupo)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> result = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            if (pedidoProduto == null)
                produtosPedido = this.ObterPedidosProdutos(new List<int>() { pedido.Codigo }, montagemCargaParametros.codigoSessaoRoteirizador);
            else
                produtosPedido.Add(pedidoProduto);

            if ((pedido == null) || (pedido.Produtos == null) || (produtosPedido.Count == 0))
                return result;

            produtosPedido.OrderByDescending(x => x.PesoTotal);

            decimal pesoCarregando = pesoAtualCarregamento;
            decimal palletCarregando = palletAtualCarregamento;
            decimal cubagemCarregando = cubagemAtualCarregamento;

            // Se o pedido não permite quebra.. vamos ver se todos ele cabe nno carregamento (Todos os produtos)
            if (!pedido.QuebraMultiplosCarregamentos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> todosProdutosPedido = this.ObterPedidosProdutos(new List<int>() { pedido.Codigo }, montagemCargaParametros.codigoSessaoRoteirizador);

                //Aki temos os totalizados dos produtos do pedido
                decimal pesoProdutos = (from obj in todosProdutosPedido select obj.PesoTotal).Sum();
                decimal palletProdutos = (from obj in todosProdutosPedido select obj.QuantidadePalet).Sum();
                decimal cubagemProdutos = (from obj in todosProdutosPedido select obj.MetroCubico).Sum();

                //Agora, vamos ver quanto destes produtos já está neste carregamento para descontar...
                if (pesoProdutos > montagemCargaParametros.pesoMaximo || (palletProdutos > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) || (cubagemProdutos > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                {
                    if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos))
                        sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos,
                            Pedido = pedido
                        });
                    return result;
                }

                //Agora... vamos ver se cabe no carregaento em andamento...

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> tmp = montagemCargaParametros.listaGrupoPedidos.FindAll(x => x.Produtos.Exists(c => c.CodigoPedido == pedido.Codigo)).ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> tmp2 = listaProdutosDestePedidoDesteGrupo.FindAll(x => x.CodigoPedido == pedido.Codigo).ToList();

                decimal pesoPedidoCarregadoCarregamento = (from item in tmp
                                                           select item.Produtos.Where(p => p.CodigoPedido == pedido.Codigo).Sum(s => s.PesoPedidoProduto)).Sum() +
                                                          (from item in tmp2 select item.PesoPedidoProduto).Sum();

                decimal palletPedidoCarregadoCarregamento = (from item in tmp
                                                             select item.Produtos.Where(p => p.CodigoPedido == pedido.Codigo).Sum(s => s.QuantidadePalletPedidoProduto)).Sum() +
                                                            (from item in tmp2 select item.QuantidadePalletPedidoProduto).Sum();

                decimal cubagemPedidoCarregadoCarregamento = (from item in tmp
                                                              select item.Produtos.Where(p => p.CodigoPedido == pedido.Codigo).Sum(s => s.MetroCubicoPedidoProduto)).Sum() +
                                                             (from item in tmp2 select item.MetroCubicoPedidoProduto).Sum();

                if ((pesoCarregando - pesoPedidoCarregadoCarregamento + pesoProdutos > montagemCargaParametros.pesoMaximo && montagemCargaParametros.pesoMaximo > 0) ||
                   (palletCarregando - palletPedidoCarregadoCarregamento + palletProdutos > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) ||
                   (cubagemCarregando - cubagemPedidoCarregadoCarregamento + cubagemProdutos > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                {
                    // Adicionado condição para setar erro sometne se o carregamento for vazio..
                    if (pesoCarregando == 0 && !sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos))
                        sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos,
                            Pedido = pedido
                        });
                    return result;
                }
            }

            // Vamos percorrer os produtos do pedido e validar a quantidade e peso
            // Quando o produto possuir só uma quantidade... e seu peso for maior que o carregamento.. vamos dividir e manter a quantidade...
            // Validar se o pedido/Produto não é paletizado.

            Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem _repositorioProdutoEmbarcadorEstoqueArmazen = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(_unitOfWork);
            int contProdutoNaoCarregaMotivoEstoque = 0;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido in produtosPedido)
            {
                if ((produtoPedido?.LinhaSeparacao?.Codigo ?? 0) != codigoLinhaSeparacao && codigoLinhaSeparacao > 0)
                    continue;

                //Se a linha de separação do produto não roteiriza, devemos pular ele... 
                if (!(produtoPedido?.LinhaSeparacao?.Roteiriza ?? true))
                {
                    if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.LinhaSeparacaoProdutoNaoRoteiriza))
                        sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.LinhaSeparacaoProdutoNaoRoteiriza,
                            Pedido = pedido
                        });
                    continue;
                }

                decimal qtdeCarregarControleSaldoProduto = produtoPedido.Quantidade;
                decimal estoqueDisponivel = -1;

                if (montagemCargaParametros.tipoStatusEstoqueMontagemCarregamentoPedidoProduto != TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos)
                {
                    estoqueDisponivel = _repositorioProdutoEmbarcadorEstoqueArmazen.BuscarEstoqueDisponivelPorFilialProdutoArmazem(produtoPedido?.FilialArmazem?.Filial.Codigo ?? 0, produtoPedido.Produto.Codigo, produtoPedido?.FilialArmazem?.Codigo ?? 0);

                    // #45737 
                    // Parcial: é a cartela de pedidos onde a quantidade solicitada é maior que o estoque. 
                    // Total: é a cartela de pedidos onde a quantidade solicitada é igual ou menor que o estoque.
                    // A ideia aqui é desconsiderar os produtos que não se encaixam na regra escolhida a cima.
                    if ((montagemCargaParametros.tipoStatusEstoqueMontagemCarregamentoPedidoProduto == TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueParcial && estoqueDisponivel >= produtoPedido.Quantidade) ||
                        (montagemCargaParametros.tipoStatusEstoqueMontagemCarregamentoPedidoProduto == TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueTotal && estoqueDisponivel < produtoPedido.Quantidade))
                    {
                        contProdutoNaoCarregaMotivoEstoque++;
                        continue;
                    }
                    //Precisa testar esse tratamento, pois.. em caso de não ter estoque disponível e carregar parcial, o saldo irá entrar em outro carregamento. (Eu acho).
                    //else if (montagemCargaParametros.tipoStatusEstoqueMontagemCarregamentoPedidoProduto == TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueParcial)
                    //    qtdeCarregarControleSaldoProduto = estoqueDisponivel;
                }

                //Verificando todos os grupos que possuem o pedidoProduto
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> tmp = montagemCargaParametros.listaGrupoPedidos.FindAll(x => x.Produtos.Exists(c => c.CodigoPedidoProduto == produtoPedido.Codigo)).ToList();
                decimal totalCarregadoProduto = (from item in tmp
                                                 select item.Produtos.Where(p => p.CodigoPedidoProduto == produtoPedido.Codigo).Sum(s => s.PesoPedidoProduto)).Sum();

                decimal totalCarregadoProdutoPallet = (from item in tmp
                                                       select item.Produtos.Where(p => p.CodigoPedidoProduto == produtoPedido.Codigo).Sum(s => s.QuantidadePalletPedidoProduto)).Sum();

                decimal totalCarregadoProdutoM3 = (from item in tmp
                                                   select item.Produtos.Where(p => p.CodigoPedidoProduto == produtoPedido.Codigo).Sum(s => s.MetroCubicoPedidoProduto)).Sum();

                decimal qtdeCarregadoProduto = (from item in tmp
                                                select item.Produtos.Where(p => p.CodigoPedidoProduto == produtoPedido.Codigo).Sum(s => s.QuantidadePedidoProduto)).Sum();

                //Agora vamos ver o que já foi carregado de cada pedidoProduto.. para adicionar ao total carregado
                AtualizarQuantidadesPedidoProdutoAtendido(produtoPedido.Codigo, ref totalCarregadoProduto, ref totalCarregadoProdutoPallet, ref qtdeCarregadoProduto, ref totalCarregadoProdutoM3);

                //Se o peso total do produto e sua quantidade não foi completamenten carregado.. (Peso problema de arredondamento, adicionando produto com "0" quantidade no carregamento.)
                if (totalCarregadoProduto < produtoPedido.PesoTotal && qtdeCarregadoProduto < qtdeCarregarControleSaldoProduto)
                {
                    decimal palletProdutoCarregar = produtoPedido.QuantidadePalet - totalCarregadoProdutoPallet;
                    decimal pesoProdutoCarregar = produtoPedido.PesoTotal - totalCarregadoProduto;
                    decimal pesoPorPallet = (pesoProdutoCarregar / (palletProdutoCarregar == 0 ? 1 : palletProdutoCarregar));
                    decimal pesoPorUnidade = (produtoPedido.PesoTotal / qtdeCarregarControleSaldoProduto);

                    //Quando pallet fechado não podemos rachar o pedido/produto em mais de um carregamento e nem quando for inferior a 1 pallet..
                    if (palletProdutoCarregar > 0 && ((produtoPedido.PalletFechado || produtoPedido.QuantidadePalet <= 1) && (pesoPorPallet < montagemCargaParametros.pesoMaximo || produtoPedido.QuantidadePalet > 1) && montagemCargaParametros.tipoMontagemCarregamentoPedidoProduto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto.PALLETS_ABERTOS))
                    {
                        //#50781
                        if (!produtoPedido.PalletFechado && montagemCargaParametros.tipoMontagemCarregamentoPedidoProduto == TipoMontagemCarregamentoPedidoProduto.PALLETS_FECHADO)
                        {
                            if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletAberto))
                                sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                {
                                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletAberto,
                                    Pedido = pedido
                                });
                            continue;
                        }

                        //Pedido produto com Pallet não fechado...
                        decimal qtdeProdutoCarregar = qtdeCarregarControleSaldoProduto - qtdeCarregadoProduto;
                        decimal metroCubicoProdutoCarregar = produtoPedido.MetroCubico - totalCarregadoProdutoM3;

                        if ((pesoProdutoCarregar + pesoCarregando > montagemCargaParametros.pesoMaximo && montagemCargaParametros.pesoMaximo > 0) ||
                            (palletProdutoCarregar + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) ||
                            (metroCubicoProdutoCarregar + cubagemCarregando > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                        {
                            //Possui apenas 1 pallet e seu peso n~çao cabe no veiculos
                            // ou se o peso a carregar do pedido mais o peso carregado for superior a capacidade máxima do veiculo e não permite quebrar o pedido
                            if ((palletProdutoCarregar <= 1 || !pedido.QuebraMultiplosCarregamentos) && produtoPedido.QuantidadePalet > 0 && produtoPedido.QuantidadePalet > palletProdutoCarregar)
                            {
                                pesoProdutoCarregar = 0;
                                if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos))
                                    sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                    {
                                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos,
                                        Pedido = pedido
                                    });
                            }
                            else
                            {
                                // Arredondando para inteiro.. pois problema quando pallet fracionado.
                                decimal qtdePorPallet = Math.Round(qtdeProdutoCarregar / (palletProdutoCarregar == 0 ? 1 : palletProdutoCarregar));
                                decimal cubagemPorPallet = (metroCubicoProdutoCarregar / (palletProdutoCarregar == 0 ? 1 : palletProdutoCarregar));

                                // Quantidade de pallets.
                                decimal qtde = 1;
                                //Não é possivel adicionar nenhuma unidede deste produto pois excede a capacidade 
                                // ou se nao tem espaço para mais 1 pallet
                                if (pesoPorPallet + pesoCarregando > montagemCargaParametros.pesoMaximo ||
                                    (montagemCargaParametros.palletMaximo > 0 && 1 + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) ||
                                    (montagemCargaParametros.cubagemMaximo > 0 && cubagemPorPallet + cubagemCarregando > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                                    pesoProdutoCarregar = 0;
                                else
                                {
                                    while (qtde * pesoPorPallet + pesoCarregando < montagemCargaParametros.pesoMaximo)
                                    {
                                        qtde += 1;
                                        if ((qtde * qtdePorPallet + qtdeCarregadoProduto) > qtdeCarregarControleSaldoProduto)
                                            break;
                                        if (qtde + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0)
                                            break;
                                        if ((qtde * cubagemPorPallet + cubagemCarregando) > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0)
                                            break;
                                    }
                                    qtde -= 1;
                                    pesoProdutoCarregar = (pesoPorPallet * qtde);
                                    qtdeProdutoCarregar = (decimal)((int)(qtde * qtdePorPallet));
                                    //var fator = (qtde / (produto.QuantidadePalet == 0 ? 1 : produto.QuantidadePalet));
                                    palletProdutoCarregar = qtde;
                                    metroCubicoProdutoCarregar = qtde * cubagemPorPallet;
                                }
                            }
                        }

                        if (pesoProdutoCarregar > 0)
                        {
                            result.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto()
                            {
                                CodigoPedido = pedido.Codigo,
                                CodigoPedidoProduto = produtoPedido.Codigo,
                                PesoPedidoProduto = pesoProdutoCarregar,
                                QuantidadePedidoProduto = (decimal)((int)qtdeProdutoCarregar),
                                QuantidadePalletPedidoProduto = palletProdutoCarregar,
                                MetroCubicoPedidoProduto = metroCubicoProdutoCarregar,
                                CodigoLinhaSeparacao = produtoPedido?.LinhaSeparacao?.Codigo ?? 0
                            });

                            pesoCarregando += pesoProdutoCarregar;
                            palletCarregando += palletProdutoCarregar;
                            cubagemCarregando += metroCubicoProdutoCarregar;

                            //Atualizando alista de retorno com as linhas de separação do carregamento
                            if (!linhasSeparacaoCarregamento.Contains(produtoPedido?.LinhaSeparacao?.Codigo ?? 0))
                                linhasSeparacaoCarregamento.Add(produtoPedido?.LinhaSeparacao?.Codigo ?? 0);
                        }
                    }
                    else if (montagemCargaParametros.tipoMontagemCarregamentoPedidoProduto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto.PALLETS_FECHADO && produtoPedido.PalletFechado == false)
                    {
                        //Pedido produto com Pallet não fechado...
                        pesoProdutoCarregar = produtoPedido.PesoTotal - totalCarregadoProduto;
                        decimal qtdeProdutoCarregar = qtdeCarregarControleSaldoProduto - qtdeCarregadoProduto;
                        decimal qtdePalletProdutoCarregar = produtoPedido.QuantidadePalet - totalCarregadoProdutoPallet;
                        decimal metroCubicoProdutoCarregar = produtoPedido.MetroCubico - totalCarregadoProdutoM3;

                        decimal palletPorItem = produtoPedido.QuantidadePalet / (qtdeCarregarControleSaldoProduto == 0 ? 1 : qtdeCarregarControleSaldoProduto);
                        decimal cubagemPorItem = produtoPedido.MetroCubico / (qtdeCarregarControleSaldoProduto == 0 ? 1 : qtdeCarregarControleSaldoProduto);

                        bool quebrouPedidoProduto = false;

                        if ((pesoProdutoCarregar + pesoCarregando > montagemCargaParametros.pesoMaximo && montagemCargaParametros.pesoMaximo > 0) ||
                            (montagemCargaParametros.palletMaximo > 0 && qtdePalletProdutoCarregar + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) ||
                            (montagemCargaParametros.cubagemMaximo > 0 && metroCubicoProdutoCarregar + cubagemCarregando > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                        {
                            if (!pedido.QuebraMultiplosCarregamentos && pesoCarregando == 0)
                            {
                                pesoProdutoCarregar = 0;
                                if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos))
                                    sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                    {
                                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos,
                                        Pedido = pedido
                                    });
                            }
                            else if (qtdeProdutoCarregar <= 1)
                                pesoProdutoCarregar = montagemCargaParametros.pesoMaximo - pesoCarregando;
                            else
                            {
                                //Vamos dividir pela quantidade.. para ver quanto carregar..
                                //Quantidade de produtos...
                                int qtde = 1;

                                decimal pesoUnidades = produtoPedido.PesoUnitario;
                                //Não é possivel adicionar nenhuma unidede deste produto pois excede a capacidade 
                                if ((pesoUnidades + pesoCarregando > montagemCargaParametros.pesoMaximo && montagemCargaParametros.pesoMaximo > 0) ||
                                    (montagemCargaParametros.palletMaximo > 0 && palletPorItem + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0) ||
                                    (montagemCargaParametros.cubagemMaximo > 0 && cubagemPorItem + cubagemCarregando > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0))
                                    pesoProdutoCarregar = 0;
                                else
                                {
                                    int itemCarregarIncremento = 1;
                                    if (montagemCargaParametros.nivelQuebraProdutoRoteirizar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar.Caixa)
                                        itemCarregarIncremento = produtoPedido.Produto.QuantidadeCaixa;

                                    if (qtdeProdutoCarregar < itemCarregarIncremento)
                                        itemCarregarIncremento = (int)qtdeProdutoCarregar;

                                    while (qtde * pesoUnidades + pesoCarregando < montagemCargaParametros.pesoMaximo)
                                    {
                                        qtde += itemCarregarIncremento; // 1;
                                        if ((qtde + qtdeCarregadoProduto) > qtdeCarregarControleSaldoProduto)//if (qtde > produto.Quantidade)
                                            break;
                                        if (qtde * palletPorItem + palletCarregando > montagemCargaParametros.palletMaximo && montagemCargaParametros.palletMaximo > 0)
                                            break;
                                        if (qtde * cubagemPorItem + cubagemCarregando > montagemCargaParametros.cubagemMaximo && montagemCargaParametros.cubagemMaximo > 0)
                                            break;
                                    }
                                    qtde -= itemCarregarIncremento + (qtde > 1 ? 0 : 1);// 1;
                                    pesoProdutoCarregar = (pesoUnidades * qtde);
                                    qtdeProdutoCarregar = qtde;
                                    qtdePalletProdutoCarregar = qtde * palletPorItem;
                                    metroCubicoProdutoCarregar = qtde * cubagemPorItem;

                                    quebrouPedidoProduto = (qtde < qtdeCarregarControleSaldoProduto - qtdeCarregadoProduto);
                                }
                            }
                        }

                        if (pesoProdutoCarregar > 0 && (!quebrouPedidoProduto || produtoPedido.QuantidadePalet > 1 || produtoPedido.QuantidadePalet == 0))
                        {
                            result.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto()
                            {
                                CodigoPedido = pedido.Codigo,
                                CodigoPedidoProduto = produtoPedido.Codigo,
                                PesoPedidoProduto = pesoProdutoCarregar,
                                QuantidadePedidoProduto = (decimal)((int)qtdeProdutoCarregar),
                                QuantidadePalletPedidoProduto = qtdePalletProdutoCarregar,
                                MetroCubicoPedidoProduto = metroCubicoProdutoCarregar,
                                CodigoLinhaSeparacao = produtoPedido?.LinhaSeparacao?.Codigo ?? 0
                            });

                            pesoCarregando += pesoProdutoCarregar;
                            palletCarregando += qtdePalletProdutoCarregar;
                            cubagemCarregando += metroCubicoProdutoCarregar;

                            //Atualizando alista de retorno com as linhas de separação do carregamento
                            if (!linhasSeparacaoCarregamento.Contains(produtoPedido?.LinhaSeparacao?.Codigo ?? 0))
                                linhasSeparacaoCarregamento.Add(produtoPedido?.LinhaSeparacao?.Codigo ?? 0);
                        }
                        else if (quebrouPedidoProduto && produtoPedido.QuantidadePalet < 1)
                            continue;
                    }
                    else
                        SetarSessaoRoteirizadorPedidosResultado(new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { produtoPedido.Pedido }, (produtoPedido.PalletFechado ? SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletFechado : SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletAberto), ref sessaoRoteirizadorPedidosResultado);

                    //Se atingir 95% de ocupação, vamos pausar...
                    if ((montagemCargaParametros.pesoMaximo * (decimal)0.97) < pesoCarregando)
                        break;
                    if (montagemCargaParametros.palletMaximo > 0 && palletCarregando >= montagemCargaParametros.palletMaximo * (decimal)0.97)
                        break;
                    if (montagemCargaParametros.cubagemMaximo > 0 && cubagemCarregando >= montagemCargaParametros.cubagemMaximo * (decimal)0.97)
                        break;
                }
            }

            // Significa.. que está solicitando carregamento controle estoque... porem algum produto não entrou no carregamento
            if (contProdutoNaoCarregaMotivoEstoque > 0) // == produtosPedido.Count)
                if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.ProdutosPedidoNaoAtendemControleEstoqueSolicitado))
                    sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                    {
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.ProdutosPedidoNaoAtendemControleEstoqueSolicitado,
                        Pedido = pedido
                    });

            return result;
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador ObterSessaoRoteirizador(int codigoSessaoRoteirizador)
        {
            if (codigoSessaoRoteirizador > 0)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(_unitOfWork);

                return repositorioSessaoRoteirizador.BuscarPorCodigo(codigoSessaoRoteirizador, false);
            }

            return new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador();
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoDeCargaPedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

            //Primeiro vamos ver os tipos de cargas distintos dos pedidos
            List<int> codigos = (from obj in pedidos
                                 select obj.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoDeCargas = repositorioTipoDeCarga.BuscarPorCodigosPedidos(codigos);

            if (tipoDeCargas.Count == 0)
                return null;

            if ((tipoDeCargas?.Count ?? 0) == 1)
                return tipoDeCargas.FirstOrDefault();
            else
            {
                //Agora, vamos ver se os tipos de carga possui o mesmo tipo de carga principal... se Sim .. assume ele..
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoDeCargasPrincipal = (from tipo in tipoDeCargas
                                                                                               select tipo.TipoCargaPrincipal).Distinct().ToList();

                // Os tipos de cargas não possuem nenhum tipo de carga principal
                int qtdeTipoCargaPrincipal = (tipoDeCargasPrincipal?.Count ?? 0);

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = null;

                if (qtdeTipoCargaPrincipal == 0)
                    tipoDeCarga = tipoDeCargas?.FirstOrDefault();
                else if (qtdeTipoCargaPrincipal == 1)
                {
                    if (tipoDeCargasPrincipal[0] != null)
                        tipoDeCarga = tipoDeCargasPrincipal.FirstOrDefault();
                    else
                        tipoDeCarga = tipoDeCargas?.FirstOrDefault();
                }
                else
                    tipoDeCarga = tipoDeCargas?.FirstOrDefault();

                return tipoDeCarga;
            }
        }

        private void ReiniciarUnitOfWork()
        {
            _unitOfWork.Dispose();
            _unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
        }

        private void RemoverBlocosCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(_unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> listaBlocoCarregamentoSegundoTrecho = repositorioBlocoCarregamento.BuscarBlocoCarregamentoSegundoTrechoPorCarregamento(carregamento.Codigo);
            List<int> listaCodigoCarregamento = listaBlocoCarregamentoSegundoTrecho.Select(o => o.CodigoCarregamentoSegundoTrecho).Distinct().ToList();

            listaCodigoCarregamento.Add(carregamento.Codigo);

            repositorioBlocoCarregamento.RemoverPorCodigosCarregamento(listaCodigoCarregamento);
        }

        private void RemoverCarregamentoRoteirizacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamentoRoteirizacao == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repositorioCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repositorioCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> listaCarregamentoRoteirizacaoClienteRota = repositorioCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> listaCarregamentoRoteirizacaoPontosPassagem = repositorioCarregamentoRoteirizacaoPontosPassagem.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota carregamentoRoteirizacaoClienteRota in listaCarregamentoRoteirizacaoClienteRota)
                repositorioCarregamentoRoteirizacaoClientesRota.Deletar(carregamentoRoteirizacaoClienteRota);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem carregamentoRoteirizacaoPontosPassagem in listaCarregamentoRoteirizacaoPontosPassagem)
                repositorioCarregamentoRoteirizacaoPontosPassagem.Deletar(carregamentoRoteirizacaoPontosPassagem);

            repositorioCarregamentoRoteirizacao.Deletar(carregamentoRoteirizacao);
        }

        private void RemoverLista(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> lista, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaRemover, Dictionary<int, decimal> pedidosPesosCarregamento, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {

            if (!sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido item in listaRemover)
                {
                    _saldoPedido[item.Codigo] -= pedidosPesosCarregamento[item.Codigo];
                    if (_saldoPedido[item.Codigo] <= 0)
                        lista.Remove(item);
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido item in listaRemover)
                {
                    bool existeProdutoPendente = false;

                    //Verificando se o pedido/produto foi carregado completamente...
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = this.ObterPedidosProdutos(new List<int>() { item.Codigo }, sessaoRoteirizador.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto prod in produtos)
                    {
                        // Se o produto não roteiriza. devemos pular ele...
                        if (!(prod?.LinhaSeparacao?.Roteiriza ?? true))
                            continue;

                        // Se a sessão estiver roteirizando apenas produtos com pallets fechados.. e o produto não for pallet fechando.. pula
                        if (sessaoRoteirizador.TipoMontagemCarregamentoPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto.PALLETS_FECHADO && prod.PalletFechado == false)
                            continue;

                        // Se a sessão for de produtos de pallets abertos.. e o produto for pallet fechando.. pula...
                        if (sessaoRoteirizador.TipoMontagemCarregamentoPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto.PALLETS_ABERTOS && prod.PalletFechado == true)
                            continue;

                        //Verificando todos os grupos que possuem o pedidoProduto
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> pp = listaGrupoPedidos.FindAll(x => x.Produtos.Exists(c => c.CodigoPedidoProduto == prod.Codigo)).ToList();

                        // Somando o total e quantidade que já foram carregados do pedido
                        decimal pesoTotalCarregadoProduto = (from t in pp select t.Produtos.Where(p => p.CodigoPedidoProduto == prod.Codigo).Sum(s => s.PesoPedidoProduto)).Sum();
                        decimal qtdeTotalCarregadoProduto = (from t in pp select t.Produtos.Where(p => p.CodigoPedidoProduto == prod.Codigo).Sum(s => s.QuantidadePedidoProduto)).Sum();
                        //Se o peso total do produto ou a quantidade total carregado for inferior a quantidade do pedidoProduto
                        if (pesoTotalCarregadoProduto < prod.PesoTotal && qtdeTotalCarregadoProduto < prod.Quantidade)
                        {
                            existeProdutoPendente = true;
                            break;
                        }
                        else if (pesoTotalCarregadoProduto < prod.PesoTotal)
                        {
                            int x = 0;
                        }
                    }

                    //Atualizando o saldo do pedido, e removendo quanto não existe mais produto pendente.
                    _saldoPedido[item.Codigo] -= pedidosPesosCarregamento[item.Codigo];

                    if (!existeProdutoPendente)
                        lista.Remove(item);
                }
            }
        }

        private void RemoverSimulacaoFrete(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacao = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = repositorioSimulacao.BuscarPorCarregamento(carregamento.Codigo);

            if (simulacaoFrete?.Codigo > 0)
                repositorioSimulacao.Deletar(simulacaoFrete);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RoteirizarPedidos(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido grupo, string servidorOSRM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool ordenar, bool roteirizacaoPedidosOrigemRecebedor, long codigoClientePrimeiraEntrega = 0)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Cliente> remetentes = (from pedido in grupo.Pedidos
                                                          where pedido.Remetente != null || pedido.Expedidor != null || pedido.Recebedor != null
                                                          select roteirizacaoPedidosOrigemRecebedor && pedido.Recebedor != null ? pedido.Recebedor : pedido.Expedidor != null ? pedido.Expedidor : (pedido.Remetente.ClientePai != null ? pedido.Remetente.ClientePai : pedido.Remetente)).Distinct().ToList();

            //Primeiro.. vamos ver todos os pedidos que possui um recebedor distinto e agrupar..
            List<Dominio.Entidades.Cliente> recebedores = (from pedido in grupo.Pedidos
                                                           where pedido.Recebedor != null && !roteirizacaoPedidosOrigemRecebedor
                                                           select pedido.Recebedor).Distinct().ToList();

            if (recebedores.Count == 0 && carregamento?.Recebedor != null)
                recebedores.Add(carregamento.Recebedor);

            //Agora vamos adicionar os destinatários que não possuem recebedor e não é para entregar ....
            var destinatarios = (from pedido in grupo.Pedidos
                                 where (pedido.Recebedor == null || roteirizacaoPedidosOrigemRecebedor) && pedido.Destinatario != null
                                 select new
                                 {
                                     Cliente = pedido.Destinatario,
                                     pedido.UsarOutroEnderecoDestino,
                                     ClienteOutroEndereco = (!pedido.UsarOutroEnderecoDestino ? null : pedido?.EnderecoDestino?.ClienteOutroEndereco)
                                 }).Distinct().ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> remetentesPedidos = (from obj in remetentes select Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(obj, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta, false, null, 0)).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatariosPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
            if (recebedores?.Count > 0)
                destinatariosPedidos.AddRange((from obj in recebedores select Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(obj, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega, false, null, 0, codigoClientePrimeiraEntrega)).ToList());

            if (destinatarios?.Count > 0)
                destinatariosPedidos.AddRange((from obj in destinatarios select Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(obj.Cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega, false, (obj.UsarOutroEnderecoDestino ? obj.ClienteOutroEndereco : null), 0, codigoClientePrimeiraEntrega)).ToList());

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida = remetentesPedidos.FirstOrDefault();
            remetentesPedidos.Remove(pontoPartida);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatariosPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();

            if (grupo.PontosDeApoio?.Count > 0)
            {
                ordenar = false;

                foreach (var ped in grupo.Pedidos)
                {
                    if ((ped.Destinatario?.PontoDeApoio?.Codigo ?? 0) == 0)
                    {
                        if (!destinatariosPontos.Exists(x => (!x.UsarOutroEndereco && x.Cliente.Codigo == ped.Destinatario.Codigo) || (x.UsarOutroEndereco && x.Codigo == ped.EnderecoDestino.ClienteOutroEndereco.Codigo)))
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto dest = (from obj in destinatariosPedidos where (!obj.UsarOutroEndereco && obj.Cliente.Codigo == ped.Destinatario.Codigo) || (obj.UsarOutroEndereco && obj.Codigo == ped.EnderecoDestino.ClienteOutroEndereco.Codigo) select obj).FirstOrDefault();
                            if (dest != null)
                                destinatariosPontos.Add(dest);
                        }
                    }
                    else
                    {
                        int codigoPontoApoio = ped.Destinatario.PontoDeApoio.Codigo;
                        if (!destinatariosPontos.Exists(x => x.Codigo == codigoPontoApoio))
                        {
                            var ponto = (from p in grupo.PontosDeApoio
                                         where p.Codigo == codigoPontoApoio
                                         select p).FirstOrDefault();

                            var pedidosDoPonto = (from pedido in grupo.Pedidos
                                                  where pedido.Destinatario?.PontoDeApoio?.Codigo == codigoPontoApoio
                                                  select pedido).ToList();

                            destinatariosPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                            {
                                TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem,
                                Codigo = codigoPontoApoio,
                                Cliente = new Dominio.Entidades.Cliente()
                                {
                                    CPF_CNPJ = ponto.Codigo,
                                    CodigoIntegracao = ponto.Codigo.ToString(),
                                    Latitude = ponto.Latitude.ToString(),
                                    Longitude = ponto.Longitude.ToString(),
                                    Nome = pedidosDoPonto[0].Destinatario?.PontoDeApoio?.Descricao ?? "Ponto de apoio",
                                    Tipo = "PontoApoio"
                                }
                            });

                            var locais_distintos = pedidosDoPonto.GroupBy(x => new { x.Destinatario.Codigo, x.Destinatario.Latitude, x.Destinatario.Longitude })
                                                           .Select(g => new
                                                           {
                                                               id = g.Key.Codigo,
                                                               latitude = ObterLatitudeOuLongitude(g.Key.Latitude),
                                                               longitude = ObterLatitudeOuLongitude(g.Key.Longitude),
                                                               peso_total = g.Sum(s => s.PesoTotal)
                                                           }).ToList();


                            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = this.ObterConfiguracaoIntegracao();
                            GoogleOrTools.Api api = new GoogleOrTools.Api(configuracaoIntegracao.ServidorRouteGoogleOrTools, configuracaoIntegracao.ServidorRouteOSM);
                            api.Veiculos = new List<GoogleOrTools.Veiculo>();
                            api.Locais = new List<GoogleOrTools.Local>();

                            //Adicionando os veículos.
                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisorias = grupo.ModeloVeicular?.DivisoesCapacidade?.ToList();
                            if (divisorias?.Count > 0)
                            {
                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisoria = divisorias.FirstOrDefault();
                                api.Veiculos.Add(new GoogleOrTools.Veiculo() { Codigo = grupo.ModeloVeicular.Codigo, Quantidade = 1, Capacidade = (long)divisoria.Quantidade });
                            }
                            else
                                api.Veiculos.Add(new GoogleOrTools.Veiculo() { Codigo = grupo.ModeloVeicular.Codigo, Quantidade = 1, Capacidade = (long)grupo.ModeloVeicular.CapacidadePesoTransporte });

                            GoogleOrTools.Local deposito = new GoogleOrTools.Local()
                            {
                                Codigo = (long)ponto.Codigo,
                                Deposito = true,
                                Latitude = ponto.Latitude,
                                Longitude = ponto.Longitude
                            };

                            api.Locais.Add(deposito);
                            foreach (var local_entrega in locais_distintos)
                            {
                                double qtdeOrigem = (double)local_entrega.peso_total;
                                while (qtdeOrigem > 0)
                                {
                                    double qtdeCarregar = qtdeOrigem;
                                    if (qtdeCarregar > api.Veiculos[0].Capacidade)
                                        qtdeCarregar = api.Veiculos[0].Capacidade;

                                    api.Locais.Add(new GoogleOrTools.Local()
                                    {
                                        Codigo = (long)local_entrega.id,
                                        Latitude = local_entrega.latitude,
                                        Longitude = local_entrega.longitude,
                                        PesoTotal = (double)qtdeCarregar,
                                        Janela = new GoogleOrTools.TimeWindow() { time = 10, start = 360, end = 1200 }
                                    });
                                    qtdeOrigem -= qtdeCarregar;
                                }
                            }

                            GoogleOrTools.ApiResultado apiResultado = api.CVRP(true, 3000);

                            if (apiResultado != null)
                            {
                                if (apiResultado.status == false)
                                    throw new Exception(apiResultado.msg);

                                for (int i = 0; i < apiResultado.result.Count; i++)
                                {
                                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPeso = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                                    Dictionary<int, decimal> pesos = new Dictionary<int, decimal>();
                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto>();

                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto> pontosDeApoio = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto>();
                                    int ini_j = 0;
                                    int fim_j = apiResultado.result[i].itens.Count;
                                    for (int j = ini_j; j < fim_j; j++)
                                    {
                                        //if ((int)apiResultado.result[i].itens[j].item >= locais_distintos.Count)
                                        //    continue;
                                        //Localizando o volume total do endereço
                                        var item = apiResultado.result[i].itens[j].item;
                                        //Localizando todos os pedidos do endereço.
                                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos_ender = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                                        pedidos_ender = pedidosDoPonto.FindAll(x => x.Destinatario.Codigo == item.Codigo);
                                        //destinatarios.Add(pedidos_ender[0].Destinatario);
                                        Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto desti = Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(pedidos_ender[0].Destinatario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega, false, pedidos_ender[0].EnderecoDestino?.ClienteOutroEndereco, 0);
                                        destinatariosPontos.Add(desti);
                                    }

                                    destinatariosPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                    {
                                        TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem,
                                        Codigo = codigoPontoApoio,
                                        Cliente = new Dominio.Entidades.Cliente()
                                        {
                                            CPF_CNPJ = ponto.Codigo,
                                            CodigoIntegracao = ponto.Codigo.ToString(),
                                            Latitude = ponto.Latitude.ToString(),
                                            Longitude = ponto.Longitude.ToString(),
                                            Nome = pedidosDoPonto[0].Destinatario?.PontoDeApoio?.Descricao ?? "Ponto de apoio",
                                            Tipo = "PontoApoio"
                                        }
                                    });

                                }
                            }
                            else
                                throw new Exception("Não foi possível gerar uma rota apartir de um ponto de apoio.");

                        }
                    }
                }
            }
            else
            {
                destinatariosPontos.AddRange(remetentesPedidos);
                destinatariosPontos.AddRange(destinatariosPedidos);
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = Servicos.Embarcador.Carga.CargaRotaFrete.GerarRoteirizacao(pontoPartida, destinatariosPontos, null, tipoUltimoPontoRoteirizacao, servidorOSRM, "", false, false, ordenar, _unitOfWork);

            #region  " Tarefa #28800 "

            if ((grupo?.ModeloVeicular?.VelocidadeMedia ?? 0) > 0 && respostaRoteirizacao != null && (respostaRoteirizacao?.TempoMinutos ?? 0) > 0)
            {
                decimal velocidadeMediaCarro = respostaRoteirizacao.Distancia * (60 / (decimal)respostaRoteirizacao.TempoMinutos);
                respostaRoteirizacao.TempoMinutos = (int)(velocidadeMediaCarro * (respostaRoteirizacao.TempoMinutos / (decimal)grupo.ModeloVeicular.VelocidadeMedia));
                respostaRoteirizacao.TempoHoras = (int)Math.Ceiling((decimal)respostaRoteirizacao.TempoMinutos / 60);
            }

            int tempoPorEntrega = 0;
            if ((grupo?.ModeloVeicular?.Codigo ?? 0) > 0 && centroCarregamento != null)
            {
                List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento> temposCarregamentoCentro = centroCarregamento?.TemposCarregamento.ToList() ?? new List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();
                Dominio.Entidades.Embarcador.Logistica.TempoCarregamento tempoModelo = (from obj in temposCarregamentoCentro where obj.ModeloVeicular?.Codigo == grupo.ModeloVeicular.Codigo select obj).FirstOrDefault();
                if (tempoModelo != null)
                    tempoPorEntrega = tempoModelo.Tempo;
            }

            if (tempoPorEntrega > 0)
            {
                // Agora vamos contar. quantas entregas possui para somar o tempo... na rota..
                int tempoEntregaMinutos = destinatariosPontos.Count * tempoPorEntrega;
                respostaRoteirizacao.TempoMinutos += tempoEntregaMinutos;
                respostaRoteirizacao.TempoHoras = respostaRoteirizacao.TempoMinutos / 60;
            }

            #endregion " Tarefa #28800 "

            return respostaRoteirizacao;

        }

        private void SalvarPedidos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> pedidoProdutos, bool montagemCarregamentoPedidoProduto)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            pedidos = pedidos.Distinct().ToList();

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                //Agora vamos salvar os produtos
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> produtosPedidoCarregamento = (from prod in pedidoProdutos
                                                                                                                            where prod.CodigoPedido == pedido.Codigo
                                                                                                                            select prod).ToList();

                // Problema ASSAI, se montagem por pedido produto.. para remover quando não tem mais item..
                if (montagemCarregamentoPedidoProduto && (produtosPedidoCarregamento?.Count ?? 0) == 0)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                {
                    Carregamento = carregamento,
                    Pedido = pedido,
                    Peso = (!montagemCarregamentoPedidoProduto ? pedido.PesoTotal : (from obj in pedidoProdutos where obj.CodigoPedido == pedido.Codigo select obj.PesoPedidoProduto).Sum()),
                    Pallet = (!montagemCarregamentoPedidoProduto ? pedido.TotalPallets : (from obj in pedidoProdutos where obj.CodigoPedido == pedido.Codigo select obj.QuantidadePalletPedidoProduto).Sum())
                };

                if ((pedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == pedido.TipoPaleteCliente select o).FirstOrDefault();
                    if (tipoDetalhePalete != null)
                        carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                }

                if (pedido?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false)
                    carregamentoPedido.Peso = 0;

                carregamentoPedidos.Add(carregamentoPedido);

                if (produtosPedidoCarregamento?.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto prod in produtosPedidoCarregamento)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto pedProdCarreg = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto()
                        {
                            CarregamentoPedido = carregamentoPedido,
                            PedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto() { Codigo = prod.CodigoPedidoProduto },
                            Peso = prod.PesoPedidoProduto,
                            Quantidade = prod.QuantidadePedidoProduto,
                            QuantidadePallet = prod.QuantidadePalletPedidoProduto,
                            MetroCubico = prod.MetroCubicoPedidoProduto,
                            QuantidadeOriginal = prod.QuantidadePedidoProduto,
                            QuantidadePalletOriginal = prod.QuantidadePalletPedidoProduto,
                            MetroCubicoOriginal = prod.MetroCubicoPedidoProduto
                        };
                        //repCarregamentoPedidoProduto.Inserir(pedProdCarreg);
                        carregamentoPedidoProdutos.Add(pedProdCarreg);
                    }
                }

                if (carregamentoPedido.Pedido.Filial != null)
                {
                    if (!filiais.Contains(carregamentoPedido.Pedido.Filial))
                        filiais.Add(carregamentoPedido.Pedido.Filial);
                }

                if (carregamentoPedido.Pedido.Destinatario != null)
                {
                    if (carregamentoPedido.Pedido.Recebedor != null && !carregamento.CarregamentoRedespacho)
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Recebedor))
                            destinatarios.Add(carregamentoPedido.Pedido.Recebedor);
                    }
                    else
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Destinatario))
                            destinatarios.Add(carregamentoPedido.Pedido.Destinatario);
                    }
                }
            }

            if (carregamentoPedidos.Count > 0)
                repCarregamentoPedido.InserirSQL(carregamentoPedidos);

            if (carregamentoPedidoProdutos.Count > 0)
                repCarregamentoPedidoProduto.InserirSQL(carregamentoPedidoProdutos);

            carregamento.Filiais = string.Join(",", (from obj in filiais select obj.Descricao).ToList());
            carregamento.Filial = carregamento.Filial ?? filiais.FirstOrDefault();
            carregamento.Destinatarios = string.Join(",", (from obj in destinatarios select obj.Descricao).ToList());
            carregamento.Destinos = string.Join(",", (from obj in destinatarios select obj.Localidade.DescricaoCidadeEstado).Distinct().ToList());

            repCarregamento.Atualizar(carregamento);
        }

        private void SetarSessaoRoteirizadorPedidosResultado(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido situacao, ref List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado)
        {
            foreach (var pedido in pedidosDestino)
            {
                if (!sessaoRoteirizadorPedidosResultado.Any(x => x.Pedido.Codigo == pedido.Codigo && x.Situacao == situacao))
                    sessaoRoteirizadorPedidosResultado.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                    {
                        Situacao = situacao,
                        Pedido = pedido
                    });
            }
        }

        private void VerificacaoDaQuantidadeMinimaDoModeloVeicular(List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> agrupados)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaAgrupado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>();
            Repositorio.Embarcador.Logistica.TempoCarregamento repTempoCarregamento = new Repositorio.Embarcador.Logistica.TempoCarregamento(_unitOfWork);
            foreach (var agrupado in agrupados)
            {
                Dominio.Entidades.Embarcador.Logistica.TempoCarregamento tempoDeCarregamento = repTempoCarregamento.BuscarModeloVeicularCarga(agrupado.ModeloVeicular.Codigo);
                if (tempoDeCarregamento != null && tempoDeCarregamento.QuantidadeMinimaEntregasRoteirizar < agrupado.QtdeEntregas)
                    listaAgrupado.Add(agrupado);
            }

            if (!(agrupados.Count == listaAgrupado.Count))
                agrupados = listaAgrupado;

        }

        private void ValidarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool montagemCarregamentoPedidoProduto, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware/*, Repositorio.UnitOfWork unitOfWork*/)
        {
            if (carregamento.VeiculoBloqueado)
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.VeiculoSelecionadoDoCarregamentoNaoPossuiLicencaAtivaParaTransporteFavorSoliciteLiberacaoDaViagem, carregamento.NumeroCarregamento));

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && configuracaoEmbarcador.RoteirizacaoObrigatoriaMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                //Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.APIGoogle);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                //#33331
                bool roteirizacaoOpcionalTipoOperacao = false;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carregamento.TipoOperacao;
                if (tipoOperacao == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = (from o in carregamentoPedidos
                                                                                              where o.Pedido?.TipoOperacao != null
                                                                                              select o.Pedido?.TipoOperacao
                                                                                             ).Distinct().ToList();

                    roteirizacaoOpcionalTipoOperacao = tiposOperacoes.Any(x => x.NaoExigeRoteirizacaoMontagemCarga);
                }
                else
                    roteirizacaoOpcionalTipoOperacao = tipoOperacao.NaoExigeRoteirizacaoMontagemCarga;

                if ((configuracaoEmbarcador.TipoMontagemCargaPadrao == TipoMontagemCarga.NovaCarga) && (carregamentoRoteirizacao == null) && !roteirizacaoOpcionalTipoOperacao)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioRoteirizarCarregamentoAntesDeSeguirComCarga, carregamento.NumeroCarregamento));
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carregamento != null && carregamento.TipoOperacao != null)
                repositorioCarregamentoPedido.AtualizarTipoOperacaoPedidosDoCarregamento(carregamento.Codigo, carregamento.TipoOperacao.Codigo);

            if (configuracaoEmbarcador.ObrigatorioGeracaoBlocosParaCarregamento && !carregamento.CarregamentoRedespacho)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocoCarregamentosExiste = repositorioBlocoCarregamento.BuscarPorCarregamento(carregamento.Codigo);

                if (blocoCarregamentosExiste.Count == 0)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioGerarBlocosDeCarregamentoAntesDeSeguirComCarga, carregamento.NumeroCarregamento));
            }

            ////#49354-Validação para não permitir gerar carga cquando valor > limite..
            decimal valorTotalMercadoriasPedidos = 0;
            decimal valorLimiteCarga = 99999999;

            if (carregamentoPedidos.Count > 0)
            {
                valorTotalMercadoriasPedidos = (from ped in carregamentoPedidos
                                                select ped.Pedido.ValorTotalNotasFiscais).Sum();

                var tiposOperacoes = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoOperacao != null
                    select o.Pedido?.TipoOperacao?.Descricao
                ).Distinct().ToList();

                if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && tiposOperacoes?.Count > 1 && !configuracaoEmbarcador.PermitirTiposOperacoesDistintasMontagemCarga)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCargaDoCarregamentoComPedidosDeTipoDeOperacoesDiferentes, carregamento.NumeroCarregamento, string.Join(",", tiposOperacoes)));

                var tiposCargas = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoDeCarga != null
                    select o.Pedido?.TipoDeCarga?.Descricao
                ).Distinct().ToList();

                if (tiposCargas?.Count > 1 && carregamento.TipoDeCarga == null)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCargaDoCarregamentoComPedidosDeTiposDeCargasDiferentes, carregamento.NumeroCarregamento, string.Join(",", tiposCargas)));

                var pedidosCancelados = (
                    from o in carregamentoPedidos
                    where o.Pedido?.SituacaoPedido == SituacaoPedido.Cancelado
                    select o.Pedido.NumeroPedidoEmbarcador
                ).Distinct().ToList();

                if (pedidosCancelados?.Count > 0)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoForamCancelados, carregamento.NumeroCarregamento, string.Join(",", pedidosCancelados)));

                if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.Pallet)
                {
                    var pedidosTotalmenteCarregados = (
                        from o in carregamentoPedidos
                        where o.Carregamento.Codigo != carregamento.Codigo && (o.Pedido.PedidoTotalmenteCarregado || o.Pedido.PalletSaldoRestante < 0) && (o.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false) == (carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false)
                        select o.Pedido.NumeroPedidoEmbarcador
                    ).Distinct().ToList();

                    if (pedidosTotalmenteCarregados?.Count > 0)
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoJaForamProgramadoCarga, carregamento.NumeroCarregamento, string.Join(",", pedidosTotalmenteCarregados)));
                }
                else
                {
                    var pedidosTotalmenteCarregados = (
                        from o in carregamentoPedidos
                        where o.Carregamento.Codigo != carregamento.Codigo && (o.Pedido.PedidoTotalmenteCarregado || o.Pedido.PesoSaldoRestante < 0) && (o.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false) == (carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false)
                        select o.Pedido.NumeroPedidoEmbarcador
                    ).Distinct().ToList();

                    if (pedidosTotalmenteCarregados?.Count > 0)
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoJaForamProgramadoCarga, carregamento.NumeroCarregamento, string.Join(",", pedidosTotalmenteCarregados)));
                }

                if (carregamentoPedidos.Any(x => x.Pedido.PedidoBloqueado))
                    throw new ServicoException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCargaNaoPodeSerGerada);

                if (carregamentoPedidos.Any(x => x.Pedido.PedidoRestricaoData))
                    throw new ServicoException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCargaNaoPodeSerGerada);

                if (carregamentoPedidos.Any(x => !x.Pedido.PedidoLiberadoMontagemCarga))
                    throw new ServicoException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosNaoLiberadosParaMontagemCarregamentoNaoPodeSerGerado);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && carregamento.ModeloVeicularCarga != null)
                {
                    //#17664 Validar se os destinatários são fornecedores... e se não possui restrição para o modelo veicular
                    List<double> cnpjsDesnatarios = (from ped in carregamentoPedidos
                                                     where ped.Pedido.Destinatario != null
                                                     select ped.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();

                    Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, cnpjsDesnatarios);

                    if (modalidadePessoasFornecedores.Count > 0)
                    {
                        Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(_unitOfWork);
                        List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);

                        if (modalidadeFornecedorPessoasRestricaoModeloVeicular.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular modalidade in modalidadeFornecedorPessoasRestricaoModeloVeicular)
                            {
                                if (modalidade.ModeloVeicular.Codigo == carregamento.ModeloVeicularCarga.Codigo &&
                                    (modalidade.TipoOperacao == null || (modalidade.TipoOperacao?.Codigo ?? 0) == (carregamento.TipoOperacao?.Codigo ?? 0)))
                                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsFornecedoresDoCarregamentoPossuemRestricoesDeModeloVeicular, modalidade?.ModalidadeFornecedorPessoa?.ModalidadePessoas?.Cliente?.NomeCNPJ ?? "", carregamento.NumeroCarregamento, carregamento.ModeloVeicularCarga.Descricao));
                            }
                        }
                    }

                    //#14515 validar se o veículo possui Tolerância minima para carregamento... e o peso total for inferior.., não permitir gerar..
                    //#23986 Não permitir peso acima da capacidade                    
                    if (configuracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga)
                    {
                        decimal toleranciaMinima = carregamento.ModeloVeicularCarga.ToleranciaPesoMenor;
                        decimal toleranciaMaxima = carregamento.ModeloVeicularCarga.ToleranciaPesoExtra;

                        decimal pesoMaximo = carregamento.ModeloVeicularCarga.CapacidadePesoTransporte + toleranciaMaxima;

                        decimal pesoCarregamento = carregamento.PesoCarregamento;
                        if (pesoCarregamento < toleranciaMinima)
                            throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaDoCarregamentoComPesoInferiorToleranciaMinimaDoModeloVeicular, carregamento.NumeroCarregamento, toleranciaMinima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));

                        if (pesoCarregamento > pesoMaximo)
                            throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaDoCarregamentoComPesoSuperiorToleranciaMaximaDoModeloVeicular, carregamento.NumeroCarregamento, toleranciaMaxima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));
                    }
                }

                //#40069
                // Quando os pedidos possui um número de pedido de devolução.. e o não possui o pedido de devolução relacionado,
                // não possibilitar gerar a carga até que o pedido de devolução não seja relacionado.
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosComPedidoDevolucaoSemPedidoDevolucao = (from obj in carregamentoPedidos
                                                                                                                 where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroPedidoDevolucao) && obj.Pedido.PedidoDevolucao == null
                                                                                                                 select obj.Pedido).ToList();

                if (pedidosComPedidoDevolucaoSemPedidoDevolucao.Count > 0)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaPoisPedidoDeDevolucaoAindaNaoFoiInformado, (from obj in pedidosComPedidoDevolucaoSemPedidoDevolucao
                                                                                                                                                                                  select obj.NumeroPedidoEmbarcador).FirstOrDefault()));
            }

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(_unitOfWork);

            if ((carregamento.Empresa == null) && (carregamento.GrupoTransportador == null) && configuracaoEmbarcador.TransportadorObrigatorioMontagemCarga)
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TransportadorObrigatorioDoCarregamento, carregamento.NumeroCarregamento));

            if (carregamento.GrupoTransportador == null && configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga && !repositorioCarregamentoApolice.ExistePorCarregamento(carregamento.Codigo))
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ApoliceDeSeguroObrigatoriaDoCarregamento, carregamento.NumeroCarregamento));

            if ((carregamento.TipoDeCarga == null) && configuracaoEmbarcador.TipoCargaObrigatorioMontagemCarga)
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCargaObrigatorioDoCarregamento, carregamento.NumeroCarregamento));

            if ((carregamento.TipoOperacao == null) && configuracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga)
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacaoObrigatorioaDoCarregamento, carregamento.NumeroCarregamento));

            if (configuracaoEmbarcador.SimulacaoFreteObrigatorioMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacao = repositorioSimulacaoFrete.BuscarPorCarregamento(carregamento.Codigo);

                if (simulacao?.SucessoSimulacao == false)
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioGerarSimulacaoDeFreteDoCarregamentoAntesDeGerarCarga, carregamento.NumeroCarregamento));
            }

            if (carregamento.TipoOperacao != null)
                valorLimiteCarga = (carregamento.TipoOperacao?.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 99999999);
            else if (carregamentoPedidos.Count > 0)
                valorLimiteCarga = (from ped in carregamentoPedidos
                                    where (ped.Pedido.TipoOperacao?.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 0) > 0
                                    select ped.Pedido.TipoOperacao.ConfiguracaoCarga.ValorLimiteNaCarga)?.FirstOrDefault() ?? 99999999;

            if (!string.IsNullOrEmpty(carregamento?.SessaoRoteirizador?.Parametros ?? null))
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(carregamento.SessaoRoteirizador.Parametros);

                if ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete)
                    if (valorTotalMercadoriasPedidos > valorLimiteCarga)
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ValorLimiteExcedidoCarregamento, carregamento.NumeroCarregamento));
            }

            // #33061 - Se for montagem carregamento por pedido produto, vamos validar se tem produto relacionado ao pedido.
            if (montagemCarregamentoPedidoProduto)
            {
                //Vamos buscar todos os produtos do carregamento, para validar se algum pedido está sem produto ou sua quantidade é "0".
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidosProdutos = repositorioCarregamentoPedidoProduto.BuscarPorCarregamento(carregamento.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = (from cpp in carregamentoPedidosProdutos
                                                                                                                                    where cpp.CarregamentoPedido.Codigo == carregamentoPedido.Codigo
                                                                                                                                    select cpp).ToList();
                    if (carregamentoPedidoProdutos.Count == 0)
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoDoCarregamentoEstaSemNenhumProdutoRelacionadoPorFavorVerifique, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamento.NumeroCarregamento));

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in carregamentoPedidoProdutos)
                        if (carregamentoPedidoProduto.Quantidade <= 0)
                            throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ProdutoDoPedidoEstaComQuantidadeIncorretaNoCarregamentoPorFavorVerifique, carregamentoPedidoProduto.PedidoProduto.Produto.Descricao, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamento.NumeroCarregamento));

                }
            }

            List<double> destinatariosFinal = (from obj in carregamentoPedidos
                                               where obj.Pedido.Destinatario != null
                                               select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();

            ////Se apenas 1 destinatário (#38697 MATTEL)
            //// Tarefa #40487 solicitando remoção da regra....

            //if (carregamento.SessaoRoteirizador != null && destinatariosFinal.Count == 1)
            //{
            //    // Analisar o centro.. se é por fimulação de frete..
            //    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            //    Repositorio.Embarcador.Logistica.Locais repositorioLocais = new Repositorio.Embarcador.Logistica.Locais(_unitOfWork);
            //    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(new List<int> { carregamento.SessaoRoteirizador.Filial.Codigo });

            //    bool montagemPorSimulacaoFrete = centrosCarregamento.Exists(x => x.TipoMontagemCarregamentoVRP == TipoMontagemCarregamentoVRP.SimuladorFrete);
            //    if (montagemPorSimulacaoFrete)
            //    {
            //        decimal valorMinimoCargaDestinatario = (from obj in carregamentoPedidos select obj.Pedido.Destinatario.ValorMinimoCarga).FirstOrDefault() ?? 0;
            //        decimal valorTotalPedidosCarga = (from obj in carregamentoPedidos select obj.Pedido.GrossSales).Sum();
            //        if (valorMinimoCargaDestinatario > 0 && valorMinimoCargaDestinatario > valorTotalPedidosCarga)
            //            throw new ServicoException(string.Format("Valor mínimo da carga não atingido para o carregamento {0}.", carregamento.NumeroCarregamento));
            //    }
            //}

            this.ValidarRestricaoVeiculos(carregamento, carregamentoPedidos, _unitOfWork);
        }

        private void ValidarRestricaoVeiculos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (carregamento.SessaoRoteirizador == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = null;

            if (!string.IsNullOrEmpty(carregamento.SessaoRoteirizador.Parametros))
                sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(carregamento.SessaoRoteirizador.Parametros);

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(new List<int> { carregamento.SessaoRoteirizador.Filial.Codigo });

            if ((centrosCarregamento?.Count ?? 0) == 0)
                return;

            if (carregamento.ModeloVeicularCarga == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP vrp = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP : centrosCarregamento.FirstOrDefault().TipoMontagemCarregamentoVRP);

            if (vrp != TipoMontagemCarregamentoVRP.VrpCapacity && vrp != TipoMontagemCarregamentoVRP.VrpTimeWindows)
                return;

            List<double> destinatariosFinal = (from obj in carregamentoPedidos
                                               where obj.Pedido.Recebedor != null
                                               select obj.Pedido.Recebedor.CPF_CNPJ).Distinct().ToList();

            destinatariosFinal.AddRange((from obj in carregamentoPedidos
                                         where obj.Pedido.Recebedor == null
                                         select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList());

            //Agora vamos ver se for fornecedor.. para validar as restriçoes de veiculos..
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, destinatariosFinal);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            if (modalidadePessoasFornecedores.Count > 0)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);
                List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular modal in modalidadeFornecedorPessoasRestricaoModeloVeicular)
            {
                if (modal.ModeloVeicular.Codigo == carregamento.ModeloVeicularCarga.Codigo &&
                    (modal.TipoOperacao == null || (modal.TipoOperacao?.Codigo ?? 0) == (carregamento.TipoOperacao?.Codigo ?? 0)))
                    throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioPossuiRestricaoDoModeloVeicularNaAbaFornecedorDoCadastroDeClientesPorFavorVerifique, (from obj in modalidadePessoasFornecedores
                                                                                                                                                                                                               where obj.Codigo == modal.ModalidadeFornecedorPessoa.ModalidadePessoas.Codigo
                                                                                                                                                                                                               select obj.Cliente.Descricao).FirstOrDefault(), carregamento.ModeloVeicularCarga.Descricao));
            }

            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centroDescarregamentos = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            centroDescarregamentos = repositorioCentroDescarregamento.BuscarPorDestinatarios(destinatariosFinal);

            List<int> codigoCanaisEntrega = (from canal in carregamentoPedidos select canal.Pedido.CanalEntrega?.Codigo ?? 0).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> temp = repositorioCentroDescarregamento.BuscarPorCanaisEntrega(codigoCanaisEntrega);

            temp = temp.FindAll(x => !destinatariosFinal.Contains(x.Destinatario?.CPF_CNPJ ?? 0)).ToList();
            centroDescarregamentos.AddRange(temp);

            //Se não possui restrição.. vamos ver os veiculos permitidos da Janela de Descarga.
            foreach (double cnpjDestinatario in destinatariosFinal)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from centro in centroDescarregamentos
                                                                                                      where (centro.Destinatario?.CPF_CNPJ ?? 0) == cnpjDestinatario
                                                                                                      select centro).FirstOrDefault();

                //Se o destinatário possuir um centro de descarregamento.
                if (centroDescarregamento != null && centroDescarregamento.VeiculosPermitidos.Count > 0)
                {
                    if (!centroDescarregamento.VeiculosPermitidos.Any(x => x.Codigo == carregamento.ModeloVeicularCarga.Codigo))
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioPossuiRestricaoDoModeloVeicularNaAbaFornecedorDoCadastroDeClientesPorFavorVerifique, centroDescarregamento.Destinatario.Descricao, carregamento.ModeloVeicularCarga.Descricao));
                }
            }
        }

        private void DefinirDadosPelosPedidosDoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            bool exigirDefinicaoReboquePedido = (carregamento.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carregamento.ModeloVeicularCarga?.NumeroReboques > 1);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = carregamentoPedidos.FirstOrDefault(o => o.Pedido.Codigo == cargaPedido.Pedido.Codigo);
                // Adicionado pois na TELHANORTE tem pedido de devolução, ai a cargaPedido possui 2 pedidos e o carregamento possui apenas 1 pedido.
                if (carregamentoPedido == null)
                    continue;

                if (carregamentoPedido.TipoCarregamentoPedido != TipoCarregamentoPedido.NaoDefinido)
                    cargaPedido.TipoCarregamentoPedido = carregamentoPedido.TipoCarregamentoPedido;

                if (exigirDefinicaoReboquePedido)
                    cargaPedido.NumeroReboque = carregamentoPedido.NumeroReboque;

                repositorioCargaPedido.Atualizar(cargaPedido);
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaPorCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Filiais.Filial filial, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Enumeradores.LoteCalculoFrete calcularFreteLote, bool carregamentoComMultiplasFiliais, bool montagemCargaPorPedidoProduto, Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades, string sequencialCargaAlfanumerico)
        {
            if (_codigosCarregamentosGerandoCarga == null)
                _codigosCarregamentosGerandoCarga = new List<int>();

            int codigoCarregamento = carregamento.Codigo;
            Dominio.Entidades.Usuario usuario = propriedades.Usuario;
            bool carregamentoGeradoViaWebService = propriedades.GeradoViaWs;

            try
            {
                Servicos.Log.TratarErro("Iniciou gerar Montagem " + carregamento.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "MontagemCarga");

                if (_codigosCarregamentosGerandoCarga.Contains(codigoCarregamento))
                    throw new ServicoException($"Carga referente ao carregamento {carregamento.NumeroCarregamento} já está sendo gerada, por favor aguarde.");

                _codigosCarregamentosGerandoCarga.Add(codigoCarregamento);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorio.BuscarPrimeiroRegistro();
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos where obj.Pedido.Filial == filial || obj.Pedido.Filial == null select obj.Pedido).ToList();
                List<int> codigosPedidos = (from obj in carregamentoPedidos select obj.Pedido.Codigo).ToList();

                if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && (pedidos == null || pedidos.Count == 0))
                    pedidos = repPedido.BuscarPorCodigos(codigosPedidos);

                string possuiPrefixo = !string.IsNullOrEmpty(configuracaoCarga.PrefixoParaCargasGeradasViaCarregamento) ? configuracaoCarga.PrefixoParaCargasGeradasViaCarregamento : string.Empty;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = pedidos.FirstOrDefault();
                bool permitirQueTransportadorAjusteCargaNoSegundoTrecho = pedidoBase?.TipoOperacao?.ConfiguracaoTransportador?.PermitirTransportadorAjusteCargaSegundoTrecho ?? false;

                Dominio.Entidades.Empresa empresa = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe &&
                    usuario.Empresa != null &&
                    permitirQueTransportadorAjusteCargaNoSegundoTrecho ?
                    usuario.Empresa : carregamento.Empresa ?? carregamento.Veiculo?.Empresa;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga
                {
                    CalcularFreteLote = calcularFreteLote,
                    Carregamento = carregamento,
                    CargaColeta = carregamento.CarregamentoColeta,
                    CargaDeComplemento = carregamentoComMultiplasFiliais && _configuracaoEmbarcador.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento,
                    Empresa = empresa,
                    Filial = filial,
                    OperadorInsercao = usuario,
                    Operador = usuario,
                    Rota = carregamento.Rota,
                    TipoCondicaoPagamento = carregamento.TipoCondicaoPagamento,
                    DataInicioViagemPrevista = carregamento.DataInicioViagemPrevista,
                    CodigoAlfanumericoCarga = sequencialCargaAlfanumerico,
                    TipoOperacao = carregamento.TipoOperacao ?? pedidoBase?.TipoOperacao
                };

                new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);

                if (!string.IsNullOrEmpty(sequencialCargaAlfanumerico))
                    carga.CodigoCargaEmbarcador = sequencialCargaAlfanumerico;
                else if (_configuracaoEmbarcador.NumeroCargaSequencialUnico && _configuracaoEmbarcador.UtilizarNumeroSequencialCargaNoCarregamento)
                {
                    carga.CodigoCargaEmbarcador = carregamento.NumeroCarregamento;
                    carga.NumeroSequenciaCarga = carregamento.AutoSequenciaNumero;
                }
                else if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) || _configuracaoEmbarcador.NumeroCargaSequencialUnico)
                {
                    carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork, null, carga.TipoOperacao);
                    carga.CodigoCargaEmbarcador = $"{carga.TipoOperacao?.ConfiguracaoCarga?.AdicionaPrefixoCodigoCarga ?? string.Empty}{carga.NumeroSequenciaCarga}{possuiPrefixo}";
                }
                else
                {
                    //#58808-DECATHLON- Cargas sendo gerada com o mesmo número por importação de arquivo e montagem carga.
                    if (repCarga.ExisteCarga(carregamento.NumeroCarregamento, filial?.CodigoFilialEmbarcador ?? string.Empty, false))
                    {
                        carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);
                        carga.CodigoCargaEmbarcador = $"{carga.NumeroSequenciaCarga}{possuiPrefixo}";
                    }
                    else
                    {
                        carga.CodigoCargaEmbarcador = $"{carregamento.NumeroCarregamento}{possuiPrefixo}";
                        carga.NumeroSequenciaCarga = carregamento.AutoSequenciaNumero;
                    }
                }

                DateTime? dataCarregamentoCarga = carregamento.DataCarregamentoCarga;
                DateTime? dataDescarregamentoCarga = carregamento.DataDescarregamentoCarga;

                if (carregamentoComMultiplasFiliais && _configuracaoEmbarcador.PermitirAlterarDataCarregamentoCargaNoPedido)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFilial = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial dadosPorFilial = repositorioCarregamentoFilial.BuscarPorCarregamentoEFilial(carregamento.Codigo, filial?.Codigo ?? 0);

                    if (dadosPorFilial != null)
                    {
                        dataCarregamentoCarga = dadosPorFilial.DataCarregamentoCarga;
                        dataDescarregamentoCarga = dadosPorFilial.DataDescarregamentoCarga;

                        if (dadosPorFilial.Empresa != null)
                            carga.Empresa = dadosPorFilial.Empresa;
                    }

                    if (carga.CargaDeComplemento && ((dadosPorFilial?.DataCarregamentoCarga == null) || (dadosPorFilial?.DataDescarregamentoCarga == null)))
                        throw new ServicoException($"A data de carregamento e descarregamento deve ser informada para a filial {filial.Descricao}");
                }

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    carga.DataPrevisaoTerminoCarga = dataDescarregamentoCarga;

                carga.TipoDeCarga = carregamento.TipoDeCarga ?? carregamento.PreCarga?.TipoDeCarga ?? pedidoBase?.TipoDeCarga;
                carga.CarregamentoIntegradoERP = carga.TipoOperacao?.SelecionarRetiradaProduto ?? false;
                carga.CargaDePreCarga = carga?.TipoOperacao?.GerarCargaViaMontagemDoTipoPreCarga ?? false;
                carga.CargaDestinadaCTeComplementar = carga?.TipoOperacao?.OperacaoDestinadaCTeComplementar ?? false;
                carga.CargaSVMTerceiro = pedidoBase?.PedidoDeSVMTerceiro ?? false;
                carga.CargaTakeOrPay = pedidoBase?.PedidoTakeOrPay ?? false;
                carga.CargaDemurrage = pedidoBase?.PedidoDemurrage ?? false;
                carga.CargaDetention = pedidoBase?.PedidoDetention ?? false;
                carga.DataInicioViagemPrevista = carregamento.DataInicioViagemPrevista ?? null;

                if (carga.TipoDeCarga?.BloquearMontagemCargaComPedidoProvisorio ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosProvisorios = pedidos.Where(pedido => pedido.Provisorio).ToList();

                    if (pedidosProvisorios.Count > 0)
                        throw new ServicoException($"O tipo de carga {carga.TipoDeCarga.Descricao} não permite pedidos provisórios (pedidos: {string.Join(", ", pedidosProvisorios.Select(pedido => pedido.NumeroPedidoEmbarcador))})");
                }

                if (carga.CargaDestinadaCTeComplementar)
                {
                    string numeroOSMae = pedidoBase != null ? repPedido.BuscarNumeroOSMae(pedidoBase.Codigo) : string.Empty;
                    if (!string.IsNullOrWhiteSpace(numeroOSMae))
                        serCarga.VincularMotoristaVeiculosOSMae(carga, pedidoBase, numeroOSMae, _unitOfWork, tipoServicoMultisoftware, _configuracaoEmbarcador);
                }

                if (carga.CargaSVMTerceiro)
                    carga.CargaSVM = false;

                if (_configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    carga.NaoExigeVeiculoParaEmissao = true;
                    carga.NaoGerarMDFe = true;
                    carga.PedidoViagemNavio = carregamento.PedidoViagemNavio;
                }

                Servicos.Embarcador.Seguro.Seguro.InformarValorSeguroCarga(carga, apolicesSeguro, carregamento.ModeloVeicularCarga, _unitOfWork);

                Servicos.Log.TratarErro("Iniciou Pedidos " + carregamento.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "MontagemCarga");
                string retorno = serCarga.CriarCargaPorPedidos(ref carga, pedidos, tipoServicoMultisoftware, apolicesSeguro, _unitOfWork, _configuracaoEmbarcador, null, montagemCargaPorPedidoProduto, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, carregamentoPedidos);
                Servicos.Log.TratarErro("Finalizou Pedidos " + carregamento.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "MontagemCarga");

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new ServicoException(retorno);

                CriarFronteirasNaCargaPorCarregamento(carga, carregamento);

                carga.DataCarregamentoCarga = dataCarregamentoCarga;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carregamento.TipoOperacao ?? repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(carregamento.TipoDeCarga?.Codigo ?? 0);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                if (tipoOperacao?.Expedidor != null || tipoOperacao?.Recebedor != null)
                {

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargasPedido)
                    {
                        cargaPedido.Expedidor = tipoOperacao.Expedidor ?? cargaPedido.Expedidor;
                        cargaPedido.Recebedor = tipoOperacao.Recebedor ?? cargaPedido.Recebedor;
                        cargaPedido.Origem = tipoOperacao.Expedidor?.Localidade ?? cargaPedido.Origem;
                        cargaPedido.Destino = tipoOperacao.Recebedor?.Localidade ?? cargaPedido.Destino;
                        cargaPedido.TipoEmissaoCTeParticipantes = (tipoOperacao.Expedidor != null && tipoOperacao.Recebedor != null) ?
                                                                   TipoEmissaoCTeParticipantes.ComExpedidorERecebedor :
                                                                   tipoOperacao.Expedidor != null ?
                                                                   TipoEmissaoCTeParticipantes.ComExpedidor :
                                                                   TipoEmissaoCTeParticipantes.ComRecebedor;

                        repositorioCargaPedido.Atualizar(cargaPedido);
                    }
                }

                new Servicos.Embarcador.Integracao.IntegracaoCarregamento(_unitOfWork).AdicionarIntegracoesCarregamento(carregamento, carregamentoPedidos, StatusCarregamentoIntegracao.Inserir, carregamentoGeradoViaWebService);

                DefinirDadosPelosPedidosDoCarregamento(carregamento, listaCargasPedido, carregamentoPedidos);

                ProcessamentoPorGeracaoCarga(carregamento, carga, pedidoBase?.Codigo ?? 0, tipoServicoMultisoftware, clienteMultisoftware, listaCargasPedido, configuracaoGeralCarga, propriedades);

                Servicos.Log.TratarErro("Finalizou Montagem " + " " + carregamento.Codigo + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "MontagemCarga");

                return carga;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_codigosCarregamentosGerandoCarga.Contains(codigoCarregamento))
                    _codigosCarregamentosGerandoCarga.Remove(codigoCarregamento);
            }
        }

        private void CriarFronteirasNaCargaPorCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);

            repCargaFronteira.DeletarPorCarga(carga.Codigo);

            if (carregamento.Fronteiras?.Count > 0)
            {
                foreach (Dominio.Entidades.Cliente fronteiraCarregamento in carregamento.Fronteiras)
                {
                    repCargaFronteira.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                    {
                        Carga = carga,
                        Fronteira = fronteiraCarregamento
                    });
                }
            }
        }

        private void OcorreuErroGerarCarga(int codigoCarregamento, bool gerarCargaCarregamentoBackground)
        {
            if (!gerarCargaCarregamentoBackground)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.GerandoCargaBackground)
                {
                    carregamento.SituacaoCarregamento = SituacaoCarregamento.EmMontagem;
                    repCarregamento.Atualizar(carregamento);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessamentoPorGeracaoCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoPedidoInicial, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRoteirizacao repCargaRoteirizacao = new Repositorio.Embarcador.Cargas.CargaRoteirizacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota repCargaRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(_unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaIndicador servicoCargaIndicador = new CargaIndicador(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamento.PreCarga != null)
            {
                carregamento.PreCarga.Carga = carga;
                repPreCarga.Atualizar(carregamento.PreCarga);
            }

            if (carregamento.Motoristas != null)
                new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork).AdicionarMotoristas(carga, carregamento.Motoristas.ToList());

            if (carregamento.Ajudantes != null)
            {
                carga.Ajudantes = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.Entidades.Usuario ajudante in carregamento.Ajudantes)
                    carga.Ajudantes.Add(ajudante);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido pedidoInicial = cargaPedidos.Where(x => x.Pedido.Codigo == codigoPedidoInicial).FirstOrDefault();
            pedidoInicial.InicioDaCarga = true;
            repCargaPedido.Atualizar(pedidoInicial);

            if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) && (carregamento.Veiculo != null || (carregamento.Motoristas != null && carregamento.Motoristas.Count > 0)))
                servicoCargaIndicador.DefinirIndicadorDadosTransporte(carga, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? CargaIndicadorVeiculoMotorista.InformadoEmbarcador : CargaIndicadorVeiculoMotorista.InformadoTransportador);

            carga.Veiculo = carregamento.Veiculo;
            carga.ModeloVeicularCarga = carregamento.ModeloVeicularCarga;
            carga.Empresa = carregamento.Empresa;

            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            if (carregamento.Veiculo != null)
            {
                if (carregamento.Veiculo.TipoVeiculo == "0")
                    carga.Veiculo = carregamento.Veiculo;
                else
                {
                    if (carregamento.Veiculo.VeiculosTracao != null && carregamento.Veiculo.VeiculosTracao.Count > 0)
                        carga.Veiculo = carregamento.Veiculo.VeiculosTracao.FirstOrDefault();
                    else
                        carga.Veiculo = carregamento.Veiculo;
                }

                if (carga.Empresa == null && carga.Veiculo != null && carga.Veiculo.Empresa != null)
                    carga.Empresa = carga.Veiculo.Empresa;

                if (carga.ModeloVeicularCarga == null && carga.Veiculo.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                    carga.ModeloVeicularCarga = carga.Veiculo.ModeloVeicularCarga;

                if (carga.Veiculo.VeiculosVinculados != null)
                {
                    if (carga.VeiculosVinculados != null)
                        carga.VeiculosVinculados.Clear();
                    else
                        carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.Entidades.Veiculo reboque in carga.Veiculo.VeiculosVinculados)
                    {
                        if (carga.ModeloVeicularCarga == null && reboque.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                            carga.ModeloVeicularCarga = reboque.ModeloVeicularCarga;

                        carga.VeiculosVinculados.Add(reboque);
                    }

                }
            }

            serCarga.VerificarModeloVeicularDiferenteDoSolicitado(carga);

            if (carregamentoRoteirizacao != null && carregamento.Expedidor == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao cargaRoteirizacao = new Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao
                {
                    DistanciaKM = carregamentoRoteirizacao.DistanciaKM,
                    TipoRota = carregamentoRoteirizacao.TipoRota,
                    TipoUltimoPontoRoteirizacao = carregamentoRoteirizacao.TipoUltimoPontoRoteirizacao,
                    Carga = carga
                };

                repCargaRoteirizacao.Inserir(cargaRoteirizacao);

                serCarga.SetarRotaCarregamentoNaCarga(carga, cargaPedidos, carregamentoRoteirizacao, _configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> carregamentoRoteirizacaoClientesRota = repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota clientesRota in carregamentoRoteirizacaoClientesRota)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota cargaRoteirizacaoClientesRota = new Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota()
                    {
                        CargaRoteirizacao = cargaRoteirizacao,
                        Cliente = clientesRota.Cliente,
                        Ordem = clientesRota.Ordem
                    };
                    repCargaRoteirizacaoClientesRota.Inserir(cargaRoteirizacaoClientesRota);
                }
            }

            if (carga.Empresa != null)
                servicoCargaIndicador.DefinirIndicadorTransportador(carga, CargaIndicadorTransportador.InformadoManualmente);

            CopiarValoresFreteCargaDeTrechoAnterior(carga, cargaPedidos, carregamento);

            bool adicionarJanelas = !_configuracaoEmbarcador.BloquearGeracaoCargaComJanelaCarregamentoExcedente && !configuracaoGeralCarga.UtilizarProgramacaoCarga;

            serCarga.FecharCarga(carga, _unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, adicionarJanelaCarregamento: adicionarJanelas, adicionarJanelaDescarregamento: adicionarJanelas, propriedades: propriedades);

            carga.CargaFechada = true;
            Servicos.Log.TratarErro("11 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

            carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado;

            if (carga.DadosSumarizados != null)
                carga.DadosSumarizados.ObservacaoInformadaPeloTransportador = carregamento.ObservacaoTransportador;

            repCarga.Atualizar(carga);
            repCarregamento.Atualizar(carregamento);
        }

        private bool ValidarMotoristaGR(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string msgErro)
        {
            msgErro = string.Empty;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            List<int> codigosMotorista = repCargaMotorista.BuscarCodigoMotoristasPorCarga(codigoCarga);
            if (codigosMotorista == null || codigosMotorista.Count == 0)
                return true;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoGR = null;
            foreach (int codigoMotorista in codigosMotorista)
            {
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                if (_configuracaoEmbarcador.ValidarDataLiberacaoSeguradora && !motorista.DataValidadeLiberacaoSeguradora.HasValue)
                {
                    msgErro = ("O motorista não possui uma data de limite da seguradora configurada, por isso não é possível informar este motorista para transportar essa carga, verifique e tente novamente.");
                    return false;
                }

                if (_configuracaoEmbarcador.ValidarDataLiberacaoSeguradora && (motorista.DataValidadeLiberacaoSeguradora.HasValue && motorista.DataValidadeLiberacaoSeguradora.Value.AddDays(1).AddMinutes(-1) < DateTime.Now))
                {
                    msgErro = ($"A data de limite do motorista na seguradora está válido até {motorista.DataValidadeLiberacaoSeguradora.Value.ToString("dd/MM/yyyy")}, por isso não é possível informar este motorista para transportar essa carga, verifique e tente novamente.");
                    return false;
                }

                retornoGR = ValidarMotoristaGR(false, carga, motorista, tipoServicoMultisoftware);

                if (retornoGR != null)
                    msgErro = retornoGR.Mensagem;

                repCarga.Atualizar(carga);
            }
            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarMotoristaGR(bool liberarComProblema, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            carga.ProblemaIntegracaoGrMotoristaVeiculo = false;
            carga.LiberarComProblemaAverbacao = false;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoGR = new Servicos.Embarcador.Transportadores.MotoristaGR(_unitOfWork).Validar(carga, motorista, tipoServicoMultisoftware);
            if (retornoGR != null)
            {
                carga.MensagemProblemaIntegracaoGrMotoristaVeiculo = retornoGR.Mensagem;
                carga.ProtocoloIntegracaoGR = retornoGR.Protocolo;
                if (!retornoGR.Sucesso)
                {
                    carga.ProblemaIntegracaoGrMotoristaVeiculo = true;
                    carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = liberarComProblema;
                }
            }
            return retornoGR;
        }

        private void CopiarValoresFreteCargaDeTrechoAnterior(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (!_configuracaoMontagemCarga.ExibirListagemNotasFiscais)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<int> codigosPedidos = cargaPedidos.Select(x => x.Pedido.Codigo).ToList();

            bool liberarPedidoPorRecebedor = (carga.TipoOperacao?.ConfiguracaoCarga?.LiberarPedidoComRecebedorParaMontagemCarga ?? false) || repositorioCargaPedido.ExisteLiberarPedidoRecebedorNaCargaAnterior(codigosPedidos, carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoTrechoAnterior> ultimaCargaDeCadaPedido;

            if (liberarPedidoPorRecebedor)
            {
                if (carregamento.Expedidor == null)
                    return;

                ultimaCargaDeCadaPedido = repositorioCargaPedido.BuscarCargaPedidoTrechoAnteriorPorPedidoRecebedor(codigosPedidos, carregamento.Expedidor.CPF_CNPJ, carga.Codigo).ToList();
            }
            else
            {
                if (carga.Empresa == null)
                    return;

                // TODO: ToList, IList não tem o método Exists()
                ultimaCargaDeCadaPedido = repositorioCargaPedido.BuscarCargaPedidoTrechoAnteriorPorPedido(codigosPedidos, carga.Empresa.CNPJ.ToDouble(), carga.Codigo).ToList();
            }

            if (ultimaCargaDeCadaPedido.Count == 0 || ultimaCargaDeCadaPedido.Exists(o => o.CodigoCargaPedido == 0))
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> ctesCargaTrechoAnterior = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedido(ultimaCargaDeCadaPedido.Select(o => o.CodigoCargaPedido).ToList());
            if (ctesCargaTrechoAnterior.Count == 0)
                throw new ServicoException("Não foi encontrado CT-es em cargas dos pedidos selecionados!");

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisCarga = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            decimal valorFreteCarga = 0;
            decimal valorFretePagarCarga = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                int codigoCargaPedidoCargaTrechoAnterior = ultimaCargaDeCadaPedido.Where(x => x.CodigoPedido == cargaPedido.Pedido.Codigo).Select(x => x.CodigoCargaPedido).FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> ctesDoPedidoTrechoAnterior = ctesCargaTrechoAnterior.Where(x => x.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedidoCargaTrechoAnterior).ToList();
                if (ctesDoPedidoTrechoAnterior.Count == 0)
                {
                    string mensagemRetorno = $"expedidor é o transportador {carga.Empresa?.RazaoSocial}";

                    if (liberarPedidoPorRecebedor)
                        mensagemRetorno = $"recebedor é o expedidor {cargaPedido.Expedidor.Nome}";

                    throw new ServicoException($"Não foi encontrado CT-e de carga anterior com o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} e onde o {mensagemRetorno}");
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrechoAnterior = ctesDoPedidoTrechoAnterior.FirstOrDefault().PedidoXMLNotaFiscal.CargaPedido;

                if (cargaPedidoTrechoAnterior.ValorFrete == 0 && cargaPedidoTrechoAnterior.ValorFreteAPagar == 0)
                    throw new ServicoException($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} não possui valor de frete!");

                cargaPedido.CTesEmitidos = true;
                cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                cargaPedido.CargaPedidoTrechoAnterior = cargaPedidoTrechoAnterior;
                cargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = cargaPedido;

                CopiarValoresCargaPedidoTrechoAnterior(cargaPedido, cargaPedidoTrechoAnterior);
                VincularCTesJaEmitidosNaCargaDeTrechoAnterior(carga, cargaPedido, notasFiscaisCarga, ctesDoPedidoTrechoAnterior);

                repositorioCargaPedido.Atualizar(cargaPedido);
                repositorioCargaPedido.Atualizar(cargaPedidoTrechoAnterior);

                valorFreteCarga += cargaPedido.ValorFrete;
                valorFretePagarCarga += cargaPedido.ValorFreteAPagar;
            }

            if (valorFreteCarga == 0 && valorFretePagarCarga == 0)
                throw new ServicoException("Não foi encontrado pedidos com valores de frete do trecho anterior!");

            carga.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;
            carga.ValorFrete = valorFreteCarga;
            carga.ValorFreteAPagar = valorFretePagarCarga;
        }

        private void CopiarValoresCargaPedidoTrechoAnterior(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrechoAnterior)
        {
            cargaPedido.ValorFrete = cargaPedidoTrechoAnterior.ValorFrete;
            cargaPedido.ValorFreteAPagar = cargaPedidoTrechoAnterior.ValorFreteAPagar;

            cargaPedido.PercentualIncluirBaseCalculo = cargaPedidoTrechoAnterior.PercentualIncluirBaseCalculo;
            cargaPedido.IncluirICMSBaseCalculo = cargaPedidoTrechoAnterior.IncluirICMSBaseCalculo;
            cargaPedido.BaseCalculoICMS = cargaPedidoTrechoAnterior.BaseCalculoICMS;
            cargaPedido.PercentualAliquota = cargaPedidoTrechoAnterior.PercentualAliquota;
            cargaPedido.ValorICMS = cargaPedidoTrechoAnterior.ValorICMS;
            cargaPedido.CST = cargaPedidoTrechoAnterior.CST;
            cargaPedido.CFOP = cargaPedidoTrechoAnterior.CFOP;

            cargaPedido.PercentualIncluirBaseCalculo = cargaPedidoTrechoAnterior.PercentualIncluirBaseCalculo;
            cargaPedido.IncluirISSBaseCalculo = cargaPedidoTrechoAnterior.IncluirISSBaseCalculo;
            cargaPedido.BaseCalculoISS = cargaPedidoTrechoAnterior.BaseCalculoISS;
            cargaPedido.PercentualAliquotaISS = cargaPedidoTrechoAnterior.PercentualAliquotaISS;
            cargaPedido.ValorISS = cargaPedidoTrechoAnterior.ValorISS;
        }

        private void VincularCTesJaEmitidosNaCargaDeTrechoAnterior(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> ctesDoPedidoTrechoAnterior)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = notasFiscaisCarga.Where(x => x.CargaPedido.Codigo == cargaPedido.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTrechoAnterior = ctesDoPedidoTrechoAnterior.Select(x => x.CargaCTe).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeTrechoAnterior in cargaCTesTrechoAnterior)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeProximoTrecho = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                {
                    Carga = carga,
                    CargaOrigem = cargaCTeTrechoAnterior.Carga,
                    CTe = cargaCTeTrechoAnterior.CTe,
                    DataVinculoCarga = DateTime.Now,
                    GerouCanhoto = true,
                    GerouControleFaturamento = true,
                    SistemaEmissor = SistemaEmissor.MultiCTe,
                    CargaCTeTrechoAnterior = cargaCTeTrechoAnterior
                };

                repositorioCargaCTe.Inserir(cargaCTeProximoTrecho);

                cargaCTeTrechoAnterior.CargaCTeProximoTrecho = cargaCTeProximoTrecho;
                repositorioCargaCTe.Atualizar(cargaCTeTrechoAnterior);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> notasDoCte = ctesDoPedidoTrechoAnterior.Where(x => x.CargaCTe.Codigo == cargaCTeTrechoAnterior.Codigo).ToList();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in notasDoCte)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalProximoTrecho = notasFiscais.Find(o => o.XMLNotaFiscal.Codigo == cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo);
                    if (pedidoXMLNotaFiscalProximoTrecho == null)
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeSubstituicao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
                    {
                        CargaCTe = cargaCTeProximoTrecho,
                        PedidoXMLNotaFiscal = pedidoXMLNotaFiscalProximoTrecho
                    };

                    repositorioCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTeSubstituicao);

                    VincularCanhotosANovaCarga(cargaPedidoXMLNotaFiscalCTeSubstituicao);
                }
            }
        }

        private void VincularCanhotosANovaCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork).SalvarCanhotoNota(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal, cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.CargaPedido, null, null, _tipoServicoMultisoftware, _configuracaoEmbarcador, _unitOfWork, configuracaoCanhoto, null);
        }
        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento AgruparCarregamentos(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao)
        {
            if (carregamentos.Count == 0)
                throw new ServicoException("Informe pelo menos 2 carregamentos a serem agrupados.");
            else if (carregamentos.Count == 1)
                return carregamentos[0];

            if (_tipoServicoMultisoftware == 0)
                _tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioMontagemCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
            Repositorio.RotaFreteEmpresaExclusiva repositorioRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(_unitOfWork);

            MontagemCargaSimuladorFrete servicoMontagemCargaSimuladorFrete = new MontagemCargaSimuladorFrete(this.ObterConfiguracaoEmbarcador(), this.ObterConfiguracaoIntegracao(), _tipoServicoMultisoftware, _stringConexao, _unitOfWork);

            // Bucando os pedidos e produtos relacionados aos carregamentos para agrupar no novo..
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioMontagemCarregamentoPedido.BuscarPorCarregamentos((from obj in carregamentos select obj.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidosProdutos = repositorioMontagemCarregamentoPedidoProduto.BuscarPorCarregamentos((from obj in carregamentos select obj.Codigo).ToList());

            decimal pesoTotal = (from obj in carregamentoPedidos select obj.Peso).Sum();
            decimal cubagemTotal = (from obj in carregamentoPedidos select obj.Pedido.CubagemTotal).Sum();
            decimal palletTotal = (from obj in carregamentoPedidos select obj.Pedido.TotalPallets).Sum();
            decimal valorTotalCarga = (from obj in carregamentoPedidos select obj.Pedido.ValorTotalNotasFiscais).Sum();

            Dominio.Entidades.Empresa transportador = (from obj in carregamentos select obj.Empresa).FirstOrDefault();

            List<Dominio.Entidades.RotaFreteEmpresaExclusiva> rotasExclusivasRegiao = null;

            Dominio.Entidades.Cliente cliente = (from obj in carregamentoPedidos select obj.Pedido.Destinatario).FirstOrDefault();

            if (cliente.Localidade.Regiao != null)
                rotasExclusivasRegiao = repositorioRotaFreteEmpresaExclusiva.BuscarPorRegiaoExclusivaRegiaoDestino(cliente.Localidade.Regiao.Codigo);

            if (rotasExclusivasRegiao == null)
                throw new ServicoException($"Nenhuma Rota de Frete encontrada para os pedidos do destinatário {cliente.CPF_CNPJ_Formatado}.");

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = (from obj in carregamentos select obj.ModeloVeicularCarga).FirstOrDefault();

            Dominio.Entidades.RotaFrete rotaFrete = (from obj in rotasExclusivasRegiao where obj.Empresa.Codigo == transportador.Codigo && (obj.RotaFrete.TipoOperacao == null || obj.RotaFrete.TipoOperacao.Codigo == tipoOperacao.Codigo) select obj.RotaFrete).FirstOrDefault();

            //Validando se a empresa possui o maior modelo veicular...
            List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorEmpresa(transportador.Codigo, "A");
            veiculos = (from v in veiculos where v.ModeloVeicularCarga != null select v).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosTransportador = (from o in veiculos
                                                                                                  select o.ModeloVeicularCarga).OrderBy(x => x.CapacidadePesoTransporte).ToList();

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = centroCarregamentoTipoOperacao.CentroCarregamento;

            modeloVeicularCarga = servicoMontagemCargaSimuladorFrete.ObterMenorModeloVeicularCarregamento(centroCarregamentoTipoOperacao, tipoOperacao.TipoDeCargaPadraoOperacao, modeloVeicularCarga, modelosTransportador, pesoTotal, cubagemTotal, palletTotal);

            Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento()
            {
                PesoBruto = pesoTotal,
                Distancia = (int)rotaFrete.Quilometros,
                ModeloVeicularCarga = modeloVeicularCarga.Codigo,
                Pedidos = (from o in carregamentoPedidos select o.Pedido.Codigo).ToList(),
                TipoDeCarga = tipoOperacao.TipoDeCargaPadraoOperacao?.Codigo ?? 0,
                TipoOperacao = tipoOperacao?.Codigo ?? 0,
                Transportador = transportador?.Codigo ?? 0
            };

            // Calculando o valor de frete simulado...
            string erro = string.Empty;
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = servicoMontagemCargaSimuladorFrete.ObterSimulacaoFrete(cotacaoFreteCarregamento, rotaFrete, out erro);
            if (!simulacaoFrete.SucessoSimulacao)
                throw new ServicoException(erro);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente = servicoMontagemCargaSimuladorFrete.ObterTipoConfiguracaoCargaEstadoCliente(centroCarregamentoTipoOperacao, cliente);
            bool exigeIsca = servicoMontagemCargaSimuladorFrete.ExigeIsca(tipoOperacao?.ConfiguracaoCarga, configuracaoTipoOperacaoCargaEstadoCliente, valorTotalCarga);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento
            {
                DataCriacao = DateTime.Now,
                DataCarregamentoCarga = (from obj in carregamentos select obj.DataCarregamentoCarga).Min(),
                DataDescarregamentoCarga = (from obj in carregamentos select obj.DataDescarregamentoCarga).Min(),
                SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem,
                TipoMontagemCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga,
                AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork).ObterProximoCodigoCarregamento(),
                TipoOperacao = (from obj in carregamentos select obj.TipoOperacao).FirstOrDefault(),
                TipoDeCarga = (from obj in carregamentos select obj.TipoDeCarga).FirstOrDefault(),
                ModeloVeicularCarga = modeloVeicularCarga,
                Empresa = (from obj in carregamentos select obj.Empresa).FirstOrDefault(),
                SessaoRoteirizador = (from obj in carregamentos select obj.SessaoRoteirizador).FirstOrDefault(),
                TipoSeparacao = (from obj in carregamentos select obj.TipoSeparacao).FirstOrDefault(),
                PesoCarregamento = (from obj in carregamentos select obj.PesoCarregamento).Sum(),
                ValorFrete = simulacaoFrete.ValorFrete,
                ExigeIsca = exigeIsca,
            };

            carregamento.DataInicioViagemPrevista = DataInicioViagemPrevistaCarregamento(new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>() { centroCarregamento }, carregamento.DataCarregamentoCarga);

            carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();

            repositorioMontagemCarregamento.Inserir(carregamento);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos select obj.Pedido).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto> grupoPedidoProdutos = (from obj in carregamentoPedidosProdutos
                                                                                                                 select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedidoProduto()
                                                                                                                 {
                                                                                                                     CodigoPedidoProduto = obj.PedidoProduto.Codigo,
                                                                                                                     PesoPedidoProduto = obj.Peso,
                                                                                                                     QuantidadePedidoProduto = obj.Quantidade,
                                                                                                                     QuantidadePalletPedidoProduto = obj.QuantidadePallet,
                                                                                                                     MetroCubicoPedidoProduto = obj.MetroCubico
                                                                                                                 }).ToList();

            SalvarPedidos(carregamento, pedidos, grupoPedidoProdutos, carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPonto = ObterTipoUltimoPontoRoteirizacao(pedidos);

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = RoteirizarPedidos((from obj in pedidos
                                                                                                                                    select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido()
                                                                                                                                    {
                                                                                                                                        Pedidos = pedidos,
                                                                                                                                        ModeloVeicular = carregamento.ModeloVeicularCarga
                                                                                                                                    }).FirstOrDefault(), _configuracaoIntegracao.ServidorRouteOSM, tipoUltimoPonto, centroCarregamento, null, true, carregamento.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false);

            GerarRotaCarregamento(carregamento, respostaRoteirizacao, tipoUltimoPonto, carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento item in carregamentos)
                repositorioMontagemCarregamento.ExcluirCarregamento(item.Codigo, false);

            return carregamento;
        }

        public void AtualizarSituacaoExigeIscaPorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedido.Codigo);
            if (carga != null)
                AtualizarSituacaoExigeIscaPorCarga(carga);
        }

        public void AtualizarSituacaoExigeIscaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carga.Carregamento;

            if (carregamento == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscal = repositorioPedidoXmlNotaFiscal.BuscarPorPedidos(pedidos.Select(p => p.Codigo).ToList());

            decimal valorTotalNotasFiscais = pedidosXmlNotaFiscal.Select(obj => obj.XMLNotaFiscal.Valor).Sum();

            Dominio.Entidades.Cliente cliente = (from pedido in pedidos select pedido.Destinatario).FirstOrDefault();

            Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao = repositorioTipoOperacao.BuscarPorFilialTipoOperacao(carga.Filial.Codigo, carga.TipoOperacao.Codigo);

            MontagemCargaSimuladorFrete servicoMontagemCargaSimuladorFrete = new MontagemCargaSimuladorFrete(this.ObterConfiguracaoEmbarcador(), this.ObterConfiguracaoIntegracao(), _tipoServicoMultisoftware, _stringConexao, _unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstadoCliente = servicoMontagemCargaSimuladorFrete.ObterTipoConfiguracaoCargaEstadoCliente(centroCarregamentoTipoOperacao, cliente);

            carregamento.ExigeIsca = servicoMontagemCargaSimuladorFrete.ExigeIsca(centroCarregamentoTipoOperacao?.TipoOperacao?.ConfiguracaoCarga, configuracaoTipoOperacaoCargaEstadoCliente, valorTotalNotasFiscais);

            repositorioCarregamento.Atualizar(carregamento);
        }

        public void AtualizarDadosSumarizadosCarregamentoPorPedidoAtualizado(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarPorPedido(pedido.Codigo);

            if (carregamentos.Count == 0)
                return;

            Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade historicoAlteracaoPeso = pedido.GetChangeByPropertyName("PesoTotal");
            decimal? pesoOriginalPedido = null;

            if (historicoAlteracaoPeso != null)
                pesoOriginalPedido = historicoAlteracaoPeso.De.ToDecimal();

            List<int> codigosCarregamento = (from o in carregamentos select o.Codigo).ToList();
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> listaCarregamentoPedido = repositorioCarregamentoPedido.BuscarPorCarregamentos(codigosCarregamento);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> listaCarregamentoPedidoPorCarregamento = (from o in listaCarregamentoPedido where o.Carregamento.Codigo == carregamento.Codigo select o).ToList();
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
                List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

                //peso pode ser por carregamentopedido ou carregamentopedidoproduto (se existir)
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in listaCarregamentoPedidoPorCarregamento)
                {
                    if ((carregamentoPedido.Pedido.Filial != null) && !filiais.Contains(carregamentoPedido.Pedido.Filial))
                        filiais.Add(carregamentoPedido.Pedido.Filial);

                    if (carregamentoPedido.Pedido.Destinatario != null)
                    {
                        if (carregamentoPedido.Pedido.Recebedor != null && !carregamento.CarregamentoRedespacho)
                        {
                            if (!destinatarios.Contains(carregamentoPedido.Pedido.Recebedor))
                                destinatarios.Add(carregamentoPedido.Pedido.Recebedor);
                        }
                        else if (!destinatarios.Contains(carregamentoPedido.Pedido.Destinatario))
                            destinatarios.Add(carregamentoPedido.Pedido.Destinatario);
                    }

                    if ((carregamentoPedido.Pedido.Codigo == pedido.Codigo) && pesoOriginalPedido.HasValue && (carregamentoPedido.Peso == pesoOriginalPedido.Value))
                    {
                        carregamento.PesoCarregamento -= carregamentoPedido.Peso;
                        carregamento.PesoCarregamento += pedido.PesoTotal;

                        carregamentoPedido.Peso = pedido.PesoTotal;
                        carregamentoPedido.Pallet = pedido.TotalPallets;

                        repositorioCarregamentoPedido.Atualizar(carregamentoPedido);
                    }
                }

                carregamento.Filiais = string.Join(",", (from o in filiais select o.Descricao).ToList());
                carregamento.Destinatarios = string.Join(",", (from o in destinatarios select o.Descricao).ToList());
                carregamento.Destinos = string.Join(",", (from o in destinatarios select o.Localidade.DescricaoCidadeEstado).Distinct().ToList());

                repositorioCarregamento.Atualizar(carregamento);
            }
        }

        public string GerarCarregamentoAutomatico(List<int> codigosPedido, int codigoSessaoRoteirizador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (codigosPedido.Count == 0)
                return "";

            _carregamentoPedidoProdutosAtendidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            _tipoServicoMultisoftware = tipoServicoMultisoftware;

            _codigoSessaoRoteirizador = codigoSessaoRoteirizador;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = ObterSessaoRoteirizador(codigoSessaoRoteirizador);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPedidos(codigosPedido);
            if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
            {
                _pedidosProdutos = this.ObterPedidosProdutos(codigosPedido, codigoSessaoRoteirizador);
                _carregamentoPedidoProdutosAtendidos = this.ObterPedidosProdutosCarregamentos((from prod in _pedidosProdutos select prod.Codigo).ToList());
            }
            _carregamentosSessao = ObterCarregamentosSessao(codigoSessaoRoteirizador);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosInconsistenciaGrupoProduto = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa = ObterLinhasSeparacaoAgrupa(sessaoRoteirizador);
            DateTime dataPrevistaCarregamento = (from o in pedidos select o.DataCarregamentoPedido)?.Min() ?? DateTime.Now;

            _saldoPedido = new Dictionary<int, decimal>();
            _saldoPedido = (from pedido in pedidos select pedido).ToDictionary(t => t.Codigo, t => t.PesoSaldoRestante);

            List<int> filiais = pedidos.Where(o => o.Filial != null).Select(o => o.Filial.Codigo).Distinct().ToList();

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(filiais);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(_unitOfWork);
            Servicos.Embarcador.Hubs.MontagemCarga servicoNotificacaomontagemCarga = new Servicos.Embarcador.Hubs.MontagemCarga();

            string erro = "";
            string msgAviso = "";
            decimal maiorCapacidadeVeicular = 0;
            bool dispFrotaCentroDescCliente = false;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidosResultado = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> grupoPedidos = ObterGruposPedidos(filiais, centrosCarregamento, pedidos, dataPrevistaCarregamento, ref dispFrotaCentroDescCliente, ref erro, ref msgAviso, ref sessaoRoteirizadorPedidosResultado, ref maiorCapacidadeVeicular, linhasSeparacaoAgrupa, ref sessaoRoteirizador);

            AtualizarSessaoRoteirizador(sessaoRoteirizador);

            if (grupoPedidos != null)
                sessaoRoteirizadorPedidosResultado = sessaoRoteirizadorPedidosResultado.FindAll(p => !grupoPedidos.Any(g => g.Pedidos.Any(i => i.Codigo == p.Pedido.Codigo)));

            int totalGrupos = grupoPedidos.Count();

            if (!string.IsNullOrWhiteSpace(erro) && totalGrupos == 0)
            {
                servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado(erro, codigoSessaoRoteirizador);
                return erro;
            }

            servicoNotificacaomontagemCarga.InformarQuantidadeProcessadosCarregamentoAutomatico(totalGrupos, 0, codigoSessaoRoteirizador, Localization.Resources.Cargas.MontagemCargaMapa.GerandoCarregamentos);

            Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipoSeparacao = null;
            int numeroCarregamento = 0;

            if (totalGrupos > 0 && codigoSessaoRoteirizador > 0)
            {
                //Verificando se exige tipo separação, vamos buscar o padrão do cadastro para informar no carregamento automatico
                if (configuracaoEmbarcador.ExigirTipoSeparacaoMontagemCarga)
                    tipoSeparacao = new Repositorio.Embarcador.Cargas.TipoSeparacao(_unitOfWork).BuscarPadrao();

                int proximoNumeroCarregamento = new MontagemCarga(_unitOfWork).ObterProximoCodigoCarregamento();
                int maximoReservaNumeroCarregamento = repositorioSessaoRoteirizador.MaximaReservaNumeroCarregamentoMontagem();

                if (proximoNumeroCarregamento > maximoReservaNumeroCarregamento)
                    numeroCarregamento = proximoNumeroCarregamento;
                else
                    numeroCarregamento = (maximoReservaNumeroCarregamento + 1);

                sessaoRoteirizador.ReservaNumeroCarregamentoMontagem = numeroCarregamento + totalGrupos - 1;
                AtualizarSessaoRoteirizador(sessaoRoteirizador);
            }

            int grupoAtual = 1;

            int tentativasGerarCarregamento = 0;
            int tempo = 1000;

            //foreach (var grupoPedido in grupoPedidos)
            for (int i = 0; i < grupoPedidos.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido grupoPedido = grupoPedidos[i];

                // Problema ASSAI, ver no remover pedido lista.. se montagem por pedido produto.. para remover quando não tem mais item..
                if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                {
                    // Filtrando somente os pedidos que possuem algum registro de produto...
                    grupoPedido.Pedidos = (from obj in grupoPedido.Pedidos
                                           where grupoPedido.Produtos.Any(pp => pp.CodigoPedido == obj.Codigo)
                                           select obj).ToList();
                    if (grupoPedido.Pedidos.Count == 0)
                        continue;
                }

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosInconsistenciaGrupoPedidos = ObterPedidosInconsistenciaGrupoProduto(grupoPedido.ModeloVeicular.Codigo, (from obj in grupoPedido.Pedidos select obj.Codigo).ToList());

                grupoPedido.Pedidos = grupoPedido.Pedidos.Where(o => !pedidosInconsistenciaGrupoPedidos.Any(p => p.Codigo == o.Codigo)).ToList();

                pedidosInconsistenciaGrupoProduto.AddRange(pedidosInconsistenciaGrupoPedidos);

                if (grupoPedido.Pedidos.Count == 0)
                    continue;

                if (configuracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga && grupoPedido.ModeloVeicular != null)
                {
                    if (centrosCarregamento != null)
                    {
                        if (centrosCarregamento.Any(x => x.NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga))
                        {
                            decimal pesoTotalCarregamento = grupoPedido.PedidosPesos.Sum(p => p.Value);

                            List<int> pedidosDesconsideraPeso = (from obj in grupoPedido.Pedidos where (obj?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false) == true select obj.Codigo).ToList();
                            decimal pesoDesconsidera = (from p in grupoPedido.PedidosPesos
                                                        where pedidosDesconsideraPeso.Contains(p.Key)
                                                        select p.Value).Sum();

                            decimal pesoCarregamento = pesoTotalCarregamento - pesoDesconsidera;
                            if (pesoCarregamento < grupoPedido.ModeloVeicular.ToleranciaPesoMenor || pesoCarregamento > (grupoPedido.ModeloVeicular.CapacidadePesoTransporte + grupoPedido.ModeloVeicular.ToleranciaPesoExtra))
                            {
                                sessaoRoteirizadorPedidosResultado.AddRange((from o in grupoPedido.Pedidos
                                                                             select new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido()
                                                                             {
                                                                                 Pedido = o,
                                                                                 SessaoRoteirizador = sessaoRoteirizador,
                                                                                 Situacao = SituacaoSessaoRoteirizadorPedido.NaoAtendeuCapacidadeModeloVeicular
                                                                             }).ToList());
                                grupoPedido.Pedidos.Clear();
                                continue;
                            }
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPonto = ObterTipoUltimoPontoRoteirizacao(grupoPedido.Pedidos);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (from obj in centrosCarregamento where obj.Filial.Codigo == grupoPedido.CodigoFilial select obj).FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = RoteirizarPedidos(grupoPedido, configuracaoIntegracao.ServidorRouteOSM, tipoUltimoPonto, centroCarregamento, null, true, sessaoRoteirizador.RoteirizacaoRedespacho);

                while (true)
                {
                    try
                    {
                        _unitOfWork.FlushAndClear();
                        _unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = GerarCarregamento(centrosCarregamento, grupoPedido, sessaoRoteirizador, tipoSeparacao, numeroCarregamento);
                        GerarRotaCarregamento(carregamento, respostaRoteirizacao, tipoUltimoPonto, sessaoRoteirizador.MontagemCarregamentoPedidoProduto);

                        _unitOfWork.CommitChanges();

                        servicoNotificacaomontagemCarga.InformarQuantidadeProcessadosCarregamentoAutomatico(totalGrupos, grupoAtual, codigoSessaoRoteirizador, Localization.Resources.Cargas.MontagemCargaMapa.GerandoCarregamentos);

                        numeroCarregamento = 0;
                        grupoAtual++;
                        tentativasGerarCarregamento = 0;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador simuladorFreteCriterioSelecaoTransportador = SimuladorFreteCriterioSelecaoTransportador.Nenhum;
                        if (centroCarregamento != null)
                            simuladorFreteCriterioSelecaoTransportador = centroCarregamento.SimuladorFreteCriterioSelecaoTransportador;

                        //Se for para simular frete tbm... vamos lá..
                        if ((sessaoRoteirizador?.Codigo ?? 0) > 0 && simuladorFreteCriterioSelecaoTransportador != SimuladorFreteCriterioSelecaoTransportador.Nenhum)
                        {
                            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDia = ObterModeloVeicularDisponibilidade(centrosCarregamento, dataPrevistaCarregamento);

                            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = ObterSessaoRoteirizadorParametros(sessaoRoteirizador);

                            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeDiaUtilizar = ObterDisponibilidadeFrotaUtilizar(disponibilidadeDia, sessaoRoteirizadorParametros);

                            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros obterGrupoPedidosParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.ObterGrupoPedidosParametros()
                            {
                                CentrosCarregamento = centrosCarregamento,
                                DisponibilidadeDia = disponibilidadeDia,
                                DisponibilidadeDiaUtilizar = disponibilidadeDiaUtilizar.Where(x => x.Quantidade >= 0).ToList(),
                                LinhasSeparacaoAgrupa = linhasSeparacaoAgrupa,
                                GruposPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido>(),
                                SessaoRoteirizador = sessaoRoteirizador,
                                SessaoRoteirizadorParametros = sessaoRoteirizadorParametros,
                                SessaoRoteirizadorPedidosSituacao = sessaoRoteirizadorPedidosResultado,
                                Pedidos = (from obj in pedidos where (!obj.PedidoBloqueado || _configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado) select obj).ToList()
                            };

                            string erros = string.Empty;
                            GerarSimulacaoFreteCarregamento(obterGrupoPedidosParametros, carregamento, grupoPedido.Pedidos, simuladorFreteCriterioSelecaoTransportador, centroCarregamento.Codigo, (int)respostaRoteirizacao.Distancia, ref erros, _configuracaoMontagemCarga.VencedorSimuladorFreteEmpresaPedido);
                        }
                        break;
                    }
                    catch (ServicoException ex)
                    {
                        _unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex);

                        erro = "Erro ao gravar dados do carregamento";
                        if (codigoSessaoRoteirizador > 0)
                        {
                            sessaoRoteirizador.ReservaNumeroCarregamentoMontagem = 0;
                            AtualizarSessaoRoteirizador(sessaoRoteirizador);
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex);
                        if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                        {
                            System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                            if (excecao.Number == 1205)
                            {
                                Servicos.Log.TratarErro("DeadLock2, tentativa " + tentativasGerarCarregamento + " ex: " + excecao.Message);

                                tentativasGerarCarregamento++;
                                System.Threading.Thread.Sleep(tempo * tentativasGerarCarregamento);

                                if (tentativasGerarCarregamento >= 5)
                                {
                                    if (codigoSessaoRoteirizador > 0)
                                    {
                                        sessaoRoteirizador.ReservaNumeroCarregamentoMontagem = 0;
                                        AtualizarSessaoRoteirizador(sessaoRoteirizador);
                                    }
                                    erro = "Erro ao gravar dados do carregamento";
                                    throw;
                                }
                                ReiniciarUnitOfWork();
                            }
                            else
                            {
                                erro = "Erro ao gravar dados do carregamento";
                                throw;
                            }
                        }
                        else
                        {
                            erro = "Erro ao gravar dados do carregamento";
                            if (codigoSessaoRoteirizador > 0)
                            {
                                sessaoRoteirizador.ReservaNumeroCarregamentoMontagem = 0;
                                AtualizarSessaoRoteirizador(sessaoRoteirizador);
                            }
                            throw;
                        }
                    }
                }
            }

            int pedidosPesoMaiorCapacidade = 0;

            if (codigoSessaoRoteirizador > 0)
            {
                sessaoRoteirizador.ReservaNumeroCarregamentoMontagem = 0;
                AtualizarSessaoRoteirizador(sessaoRoteirizador);

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(_unitOfWork);

                repositorioSessaoRoteirizadorPedido.AtualizarSituacao(codigoSessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.OK);

                if (grupoPedidos != null)
                    sessaoRoteirizadorPedidosResultado = sessaoRoteirizadorPedidosResultado.FindAll(p => !grupoPedidos.Any(g => g.Pedidos.Any(i => i.Codigo == p.Pedido.Codigo)));

                var situacoes = (from resultado in sessaoRoteirizadorPedidosResultado select resultado.Situacao).Distinct().ToList();

                for (int i = 0; i < situacoes.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido situacao = situacoes[i];
                    var pedidosSituacao = (from sitPedido in sessaoRoteirizadorPedidosResultado where sitPedido.Situacao == situacao select sitPedido.Pedido.Codigo).ToList();

                    repositorioSessaoRoteirizadorPedido.AtualizarSituacao(codigoSessaoRoteirizador, pedidosSituacao, situacao);

                    if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular)
                        pedidosPesoMaiorCapacidade = pedidosSituacao.Count;
                }

                //Lista para atualizar a situação no pedido sessão roteirizador.
                var outrosPedidos = pedidos;

                //Agora vamos ver os demais pedidos que não estão nas cargas.. e vamos gravar a situação como AusenciaDeDisponibilidadeDeFrota
                if (grupoPedidos != null)
                    outrosPedidos = outrosPedidos.FindAll(p => !grupoPedidos.Any(g => g.Pedidos.Any(i => i.Codigo == p.Codigo)));

                //Remover dos outros pedidos, aqueles que só possuem produtos com linha de separação que não roteiriza
                if (outrosPedidos != null)
                {
                    //Não existe na lista de retorno.. validação.
                    outrosPedidos = outrosPedidos.FindAll(p => !sessaoRoteirizadorPedidosResultado.Exists(g => g.Pedido.Codigo == p.Codigo));

                    if (sessaoRoteirizador.MontagemCarregamentoPedidoProduto)
                    {
                        //outrosPedidos = outrosPedidos.FindAll(p => p.Produtos.Any(o => o?.LinhaSeparacao?.Roteiriza ?? false == true));
                        outrosPedidos = (from ped in outrosPedidos
                                         join prod in _pedidosProdutos on ped.Codigo equals prod.Pedido.Codigo
                                         where (prod?.LinhaSeparacao?.Roteiriza ?? false) == true
                                         select ped).ToList();
                    }
                }

                if (outrosPedidos?.Count > 0 && string.IsNullOrEmpty(erro))
                {

                    outrosPedidos = outrosPedidos.Where(o => !pedidosInconsistenciaGrupoProduto.Any(p => p.Codigo == o.Codigo)).ToList();
                    var ids = (from outro in outrosPedidos select outro.Codigo).Distinct().ToList();

                    repositorioSessaoRoteirizadorPedido.AtualizarSituacao(
                        codigoSessaoRoteirizador, ids,
                        (!dispFrotaCentroDescCliente ?
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.AusenciaDeDisponibilidadeDeFrota :
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.CentroDescarregamentoSemVeiculoPermitido)
                    );
                }

                if (pedidosInconsistenciaGrupoProduto?.Count > 0 && string.IsNullOrEmpty(erro))
                {
                    var ids = (from pedido in pedidosInconsistenciaGrupoProduto select pedido.Codigo).Distinct().ToList();

                    repositorioSessaoRoteirizadorPedido.AtualizarSituacao(
                        codigoSessaoRoteirizador, ids, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.AusenciaDeDisponibilidadeDeFrotaParaGrupoProduto
                    );
                }
            }

            //string retorno = (grupoAtual > 1 ? "" : erro) + (!string.IsNullOrEmpty(msgAviso) ? "\n" + msgAviso : string.Empty);
            string aux = "";
            if (grupoAtual > 1 && !string.IsNullOrEmpty(erro))
                aux = "Foram gerados " + (grupoAtual - 1) + " carregamentos.\n" + erro;
            else if (!string.IsNullOrEmpty(erro))
                aux = erro;

            string retorno = aux + (!string.IsNullOrEmpty(msgAviso) ? "\n" + msgAviso : string.Empty);

            servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado(retorno, codigoSessaoRoteirizador, pedidosPesoMaiorCapacidade, maiorCapacidadeVeicular);

            if (_unitOfWork != null)
                _unitOfWork.Dispose();

            return erro;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem ObterDadosPesagem(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem carregamentoDadosPesagem = new Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem();

            if (carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto == true)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtos = repositorioCarregamentoPedidoProduto.BuscarPorCarregamento(carregamento.Codigo);

                carregamentoDadosPesagem.Peso = produtos.Sum(o => o.Peso);
                carregamentoDadosPesagem.Cubagem = produtos.Sum(o => o.MetroCubico);
                carregamentoDadosPesagem.Pallet = produtos.Sum(o => o.QuantidadePallet);
            }
            else
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamento = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

                carregamentoDadosPesagem.Peso = pedidosCarregamento.Sum(o => o.Peso);
                carregamentoDadosPesagem.Cubagem = pedidosCarregamento.Sum(o => o.Pedido.CubagemTotal);
                carregamentoDadosPesagem.Pallet = pedidosCarregamento.Sum(o => o.Pedido.NumeroPaletes + o.Pedido.NumeroPaletesFracionado);
            }

            if (carregamento.ModeloVeicularCarga != null)
            {
                carregamentoDadosPesagem.Cubagem += (carregamento.TipoDeCarga?.Paletizado ?? false) ? carregamento.ModeloVeicularCarga.ObterOcupacaoCubicaPaletes() : 0m;

                if (carregamento.ModeloVeicularCarga.ModeloControlaCubagem)
                {
                    carregamentoDadosPesagem.CapacidadeCubagem = carregamento.ModeloVeicularCarga.Cubagem;
                    carregamentoDadosPesagem.PercentualOcupacaoCubagem = (carregamentoDadosPesagem.Cubagem * 100) / carregamentoDadosPesagem.CapacidadeCubagem;
                    carregamentoDadosPesagem.PossuiCubagem = true;
                    carregamentoDadosPesagem.ToleranciaMinimaCubagem = carregamento.ModeloVeicularCarga.ToleranciaMinimaCubagem;

                    if (carregamentoDadosPesagem.Cubagem >= carregamentoDadosPesagem.ToleranciaMinimaCubagem)
                        carregamentoDadosPesagem.SituacaoCubagem = (carregamentoDadosPesagem.Cubagem > carregamentoDadosPesagem.CapacidadeCubagem) ? SituacaoPesagemCarregamento.Excedida : SituacaoPesagemCarregamento.Aprovada;
                    else
                        carregamentoDadosPesagem.SituacaoCubagem = SituacaoPesagemCarregamento.Reprovada;
                }

                if (carregamento.ModeloVeicularCarga.VeiculoPaletizado)
                {
                    carregamentoDadosPesagem.CapacidadePallet = (decimal)carregamento.ModeloVeicularCarga.NumeroPaletes;
                    carregamentoDadosPesagem.PercentualOcupacaoPallet = (carregamentoDadosPesagem.Pallet * 100) / carregamentoDadosPesagem.CapacidadePallet;
                    carregamentoDadosPesagem.PossuiPallet = true;
                    carregamentoDadosPesagem.ToleranciaMinimaPallet = (decimal)carregamento.ModeloVeicularCarga.ToleranciaMinimaPaletes;

                    if (carregamentoDadosPesagem.Pallet >= carregamentoDadosPesagem.ToleranciaMinimaPallet)
                        carregamentoDadosPesagem.SituacaoPallet = (carregamentoDadosPesagem.Pallet > carregamentoDadosPesagem.CapacidadePallet) ? SituacaoPesagemCarregamento.Excedida : SituacaoPesagemCarregamento.Aprovada;
                    else
                        carregamentoDadosPesagem.SituacaoPallet = SituacaoPesagemCarregamento.Reprovada;
                }

                carregamentoDadosPesagem.CapacidadePeso = carregamento.ModeloVeicularCarga.CapacidadePesoTransporte;
                carregamentoDadosPesagem.PercentualOcupacaoPeso = (carregamentoDadosPesagem.Peso * 100) / carregamentoDadosPesagem.CapacidadePeso;
                carregamentoDadosPesagem.ToleranciaMinimaPeso = carregamento.ModeloVeicularCarga.ToleranciaPesoMenor;

                if (carregamentoDadosPesagem.Peso >= carregamentoDadosPesagem.ToleranciaMinimaPeso)
                    carregamentoDadosPesagem.SituacaoPeso = (carregamentoDadosPesagem.Peso > carregamentoDadosPesagem.CapacidadePeso) ? SituacaoPesagemCarregamento.Excedida : SituacaoPesagemCarregamento.Aprovada;
                else
                    carregamentoDadosPesagem.SituacaoPeso = SituacaoPesagemCarregamento.Reprovada;
            }

            return carregamentoDadosPesagem;
        }

        public void RemoverDadosRelacionadosAosPedidos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool limparDadosRoteirizacao)
        {
            if (limparDadosRoteirizacao)
                RemoverCarregamentoRoteirizacao(carregamento);
            RemoverSimulacaoFrete(carregamento);
            RemoverBlocosCarregamento(carregamento);
        }

        public void RemoverPedido(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (carregamento == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> listaCarregamentoPedido = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            if (!listaCarregamentoPedido.Any(o => o.Pedido.Codigo == pedido.Codigo))
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in listaCarregamentoPedido)
            {
                if (carregamentoPedido.Pedido.Codigo == pedido.Codigo)
                {
                    carregamento.PesoCarregamento = (carregamento.PesoCarregamento > carregamentoPedido.Peso) ? carregamento.PesoCarregamento - carregamentoPedido.Peso : 0m;

                    if (configuracaoEmbarcador.LiberarPedidosParaMontagemCargaCancelada)
                    {
                        pedido.SituacaoPedido = SituacaoPedido.Aberto;
                        pedido.PesoSaldoRestante += carregamentoPedido.Peso;
                    }

                    repositorioCarregamentoPedido.ExcluirCarregamentoPedido(carregamentoPedido.Codigo);
                }
                else
                {
                    if ((carregamentoPedido.Pedido.Filial != null) && !filiais.Contains(carregamentoPedido.Pedido.Filial))
                        filiais.Add(carregamentoPedido.Pedido.Filial);

                    if (carregamentoPedido.Pedido.Destinatario != null)
                    {
                        if (carregamentoPedido.Pedido.Recebedor != null && !carregamento.CarregamentoRedespacho)
                        {
                            if (!destinatarios.Contains(carregamentoPedido.Pedido.Recebedor))
                                destinatarios.Add(carregamentoPedido.Pedido.Recebedor);
                        }
                        else if (!destinatarios.Contains(carregamentoPedido.Pedido.Destinatario))
                            destinatarios.Add(carregamentoPedido.Pedido.Destinatario);
                    }
                }
            }

            // Tratar no futuro a remoção da sessão de roteirização ;)
            // Tratar no futuro o cancelamento da reserva da sessão de roteirização ;)

            RemoverDadosRelacionadosAosPedidos(carregamento, true);

            carregamento.Filiais = string.Join(",", (from o in filiais select o.Descricao).ToList());
            carregamento.Filial = carregamento.Filial ?? filiais.FirstOrDefault();
            carregamento.Destinatarios = string.Join(",", (from o in destinatarios select o.Descricao).ToList());
            carregamento.Destinos = string.Join(",", (from o in destinatarios select o.Localidade.DescricaoCidadeEstado).Distinct().ToList());

            repositorioCarregamento.Atualizar(carregamento);
        }

        public void RemoverPedidoTodosCarregamentos(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarPorPedido(pedido.Codigo);

            if (carregamentos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentos)
                RemoverPedido(carregamento, pedido);
        }

        public bool CancelarCarregamentos(List<int> listaCodigoCarregamento, ref bool valida, ref string msg_erro, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            return this.CancelarCarregamentos(listaCodigoCarregamento, ref valida, ref msg_erro, true, usuario, true, false, unitOfWork);
        }

        public bool CancelarCarregamentos(List<int> listaCodigoCarregamento, ref bool valida, ref string msg_erro, bool portalRetira, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            return this.CancelarCarregamentos(listaCodigoCarregamento, ref valida, ref msg_erro, true, usuario, true, portalRetira, unitOfWork);
        }

        public bool CancelarCarregamentos(List<int> listaCodigoCarregamento, ref bool valida, ref string msg_erro, bool ajustaCarregamentoPedido, Dominio.Entidades.Usuario usuario, bool excluirMontagensSimulacoesFrete, bool portalRetira, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool possuiIntegracaoSaintgobin = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.SaintGobain);

            List<SituacaoCarregamento> situacoesPermitidasCliente = new List<SituacaoCarregamento>();
            //#54609-PPC
            if (portalRetira)
                situacoesPermitidasCliente.Add(SituacaoCarregamento.FalhaIntegracao);

            //#58065 - PPC - Segundo Joel, pode permitir cancelar o carregamento, desde que a carga não estaja com o status BLoqueado.. integração SAP.
            if (possuiIntegracaoSaintgobin && portalRetira)
                situacoesPermitidasCliente.Add(SituacaoCarregamento.Fechado);

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            List<int> carregamentosNaoPermiteCancelar = repositorioCarregamento.NaoPermitirCancelarCarregamentos(listaCodigoCarregamento, situacoesPermitidasCliente);

            if (carregamentosNaoPermiteCancelar?.Count > 0)
            {
                List<SituacaoCarregamento> situacoesAssai = new List<SituacaoCarregamento>() { SituacaoCarregamento.Fechado, SituacaoCarregamento.GerandoCargaBackground };

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarCodigos(carregamentosNaoPermiteCancelar);
                if ((carregamentos.Any(x => x.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false)))
                {
                    listaCodigoCarregamento.RemoveAll(x => carregamentosNaoPermiteCancelar.Any(y => x == y));
                    carregamentosNaoPermiteCancelar = (from car in carregamentos
                                                       where !situacoesAssai.Contains(car.SituacaoCarregamento)
                                                       select car.Codigo).ToList();
                }

                if (carregamentosNaoPermiteCancelar?.Count > 0)
                {
                    msg_erro = string.Format("Existe{0} {1} carregamento{2} com situação na qual não permite cancelar.", (carregamentosNaoPermiteCancelar.Count > 1 ? "m" : ""), carregamentosNaoPermiteCancelar.Count, (carregamentosNaoPermiteCancelar.Count > 1 ? "s" : ""));
                    valida = true;
                    return false;
                }
            }

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioPedidosCarregamentos = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamento = repositorioPedidosCarregamentos.BuscarPorCarregamentos(listaCodigoCarregamento);

            var carregamento = (from car in pedidosCarregamento select car.Carregamento).FirstOrDefault();
            if (carregamento?.SessaoRoteirizador != null)
            {
                if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != usuario.Codigo && usuario.TipoComercial != TipoComercial.Gerente)
                {
                    msg_erro = string.Format("Não é permitido cancelar carregamentos pois a sessão de roteirização está aberta pelo usuário {0}.", carregamento.SessaoRoteirizador.UsuarioAtual.Nome);
                    valida = true;
                    return false;
                }
            }

            if (ajustaCarregamentoPedido)
            {
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarPorCodigos(listaCodigoCarregamento);

                foreach (int codigoCarregamento in listaCodigoCarregamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamentoFiltrada = (from o in pedidosCarregamento where o.Carregamento.Codigo == codigoCarregamento select o).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedidoCarregamento in pedidosCarregamentoFiltrada)
                    {
                        if ((pedidoCarregamento.Carregamento?.Cargas?.Count() ?? 0) > 0)
                            pedidoCarregamento.Pedido.PesoSaldoRestante += pedidoCarregamento.Peso;

                        if (pedidoCarregamento.Carregamento.CarregamentoRedespacho || (pedidoCarregamento.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false))
                            pedidoCarregamento.Pedido.PedidoRedespachoTotalmenteCarregado = false;
                        else
                        {
                            pedidoCarregamento.Pedido.PedidoTotalmenteCarregado = false;
                            //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                            Servicos.Log.TratarErro($"Pedido {pedidoCarregamento.Pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedidoCarregamento.Pedido.PesoSaldoRestante} - Peso Total.: {pedidoCarregamento.Pedido.PesoTotal} - Totalmente carregado.: {pedidoCarregamento.Pedido.PedidoTotalmenteCarregado}. MontagemCarga.CancelarCarregamentos", "SaldoPedido");
                        }

                        repositorioPedido.Atualizar(pedidoCarregamento.Pedido);
                    }

                    carregamento = carregamentos.Find(x => x.Codigo == codigoCarregamento);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = pedidosCarregamentoFiltrada.Select(pc => pc.Pedido).ToList();
                    servicoCarga.VisibilidadesDasCargas(pedidos, carregamento, unitOfWork, true);
                }
            }

            repositorioCarregamento.ExcluirCarregamentos(listaCodigoCarregamento, excluirMontagensSimulacoesFrete);

            return true;
        }

        public void GerarRotaCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, bool montagemPedidoProduto)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao
            {
                Carregamento = carregamento,
                DistanciaKM = respostaRoteirizacao.Distancia,
                TipoRota = "",
                PolilinhaRota = respostaRoteirizacao.Polilinha,
                TempoDeViagemEmMinutos = respostaRoteirizacao.TempoMinutos,
                TipoUltimoPontoRoteirizacao = tipoUltimoPontoRoteirizacao
            };

            string pontosDaRota = respostaRoteirizacao.PontoDaRota;

            repCarregamentoRoteirizacao.Inserir(carregamentoRoteirizacao);

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCargaRoteirizacao.SetarPontosPassagem(carregamentoRoteirizacao, pontosDaRota, !montagemPedidoProduto, _unitOfWork);

            if (string.IsNullOrWhiteSpace(pontosDaRota))
                return;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosDaRota);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> rotasClientes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();

            int ordem = 1;
            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota ponto in pontos)
            {
                if (ponto.tipoponto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem &&
                    ponto.tipoponto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio &&
                    ponto.tipoponto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Apoio &&
                    ponto.tipoponto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balanca)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota rotasCliente = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota();

                    //if (ponto.codigo > 0)
                    //    rotasCliente.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(ponto.codigo.ToString())));

                    //if (rotasCliente.Cliente == null && ponto.codigo_cliente > 0)
                    //    rotasCliente.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(ponto.codigo_cliente.ToString())));

                    //Foi adicionado o codigo_cliente e o em algum processo está passando o ponto.Codigo um valor que não é o código do cliente
                    // gerando erro ao tentar incluir.. erro de FK.
                    if (!montagemPedidoProduto)
                    {
                        double cli_cgccpf = repCliente.ValidaCPFCNPJCliente(double.Parse(Utilidades.String.OnlyNumbers(ponto.codigo.ToString())), double.Parse(Utilidades.String.OnlyNumbers(ponto.codigo_cliente.ToString())));
                        if (cli_cgccpf > 0)// && !ponto.usarOutroEndereco)
                            rotasCliente.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = cli_cgccpf };
                    }
                    else
                    {
                        if (ponto.codigo_cliente > 0)
                            rotasCliente.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = ponto.codigo_cliente };
                        else if (ponto.codigo > 0)
                            rotasCliente.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = ponto.codigo };
                    }

                    if (ponto.usarOutroEndereco && ponto.codigo > 0)
                        rotasCliente.OutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco() { Codigo = (int)ponto.codigo };

                    if (rotasCliente.Cliente == null)
                        continue;

                    rotasCliente.CarregamentoRoteirizacao = carregamentoRoteirizacao;
                    rotasCliente.Ordem = ordem;
                    rotasCliente.Coleta = (ponto.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta);
                    //repCarregamentoRoteirizacaoClientesRota.Inserir(rotasCliente);
                    //repCarregamentoRoteirizacaoClientesRota.InserirSQL(rotasCliente);
                    rotasClientes.Add(rotasCliente);
                    ordem++;
                }
            }
            if (rotasClientes.Count > 0)
                repCarregamentoRoteirizacaoClientesRota.InserirSQL(rotasClientes);
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RoteirizarPedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, string servidorOSRM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool ordenar, bool roteirizacaoPedidosOrigemRecebedor, long codigoClientePrimeiraEntrega = 0)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido grupo = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido()
            {
                Pedidos = pedidos,
                ModeloVeicular = modeloVeicularCarga
            };

            //Validar pontos de apoio..
            List<Dominio.Entidades.Cliente> destinatarios = (from pedido in pedidos
                                                             where pedido.Destinatario != null || pedido.Recebedor != null
                                                             select pedido.Recebedor != null && !roteirizacaoPedidosOrigemRecebedor ? pedido.Recebedor : pedido.Destinatario).Distinct().ToList();

            // Analisando os pontos de apoio cadastrados para os destinatarios...
            List<Dominio.Entidades.Embarcador.Logistica.Locais> pontosDeApoio = (from apoio in destinatarios
                                                                                 where apoio.PontoDeApoio != null
                                                                                 select apoio.PontoDeApoio).Distinct().ToList();

            // Convertendo para a lista de retorno da roteirização...
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto> pontos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto>();

            foreach (Dominio.Entidades.Embarcador.Logistica.Locais local in pontosDeApoio)
            {
                var areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(local.Area);
                pontos.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPonto()
                {
                    Codigo = local.Codigo,
                    Latitude = areas[0].position.lat,
                    Longitude = areas[0].position.lng
                });
            }

            grupo.PontosDeApoio = pontos;

            return this.RoteirizarPedidos(grupo, servidorOSRM, tipoUltimoPontoRoteirizacao, centroCarregamento, carregamento, ordenar, roteirizacaoPedidosOrigemRecebedor, codigoClientePrimeiraEntrega);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosInconsistenciaGrupoProduto(int codigoModeloVeicular, List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosInconsistentes = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork).BuscarPorCodigo(codigoModeloVeicular);

            if ((modeloVeicular?.GruposProdutos.Count ?? 0) > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork).BuscarPorCodigos(codigosPedidos);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    List<int> listaGrupos = (from obj in pedido.Produtos select obj.Produto.GrupoProduto.Codigo).Distinct().ToList();

                    if (!listaGrupos.All(l => (modeloVeicular.GruposProdutos.Select(g => g.Codigo).Contains(l))))
                        pedidosInconsistentes.Add(pedido);
                }
            }

            return pedidosInconsistentes;
        }

        public int ObterProximoCodigoCarregamento()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            if (repositorioConfiguracaoEmbarcador.UtilizaNumeroSequencialCargaCarregamento())
                return Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);

            return repositorioCarregamento.BuscarProximoCodigo();
        }

        public void VerificarPedidosColetaEntrega(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (!(carregamento.TipoOperacao?.PedidoColetaEntrega ?? false) && !carregamento.Pedidos.Any(o => o.Pedido.PedidoColetaEntrega))
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidosNotasFiscais = repositorioCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> carregamentoNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            carregamentoPedidosNotasFiscais.ForEach(nf => carregamentoNotasFiscais.AddRange(nf.NotasFiscais));

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> pedidoNotasFiscais = carregamentoPedido.Pedido.NotasFiscais.ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisEmSegundoTrecho = repositorioPedidoXMLNotaFiscal.BuscarNotasColetaEntregaEmSegundoTrecho(carregamentoPedido.Pedido.Codigo);
                List<int> notasFiscaisEmSengudoTrecho = pedidoXMLNotasFiscaisEmSegundoTrecho.Select(o => o.XMLNotaFiscal.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasPendentes = pedidoNotasFiscais.Where(o => !notasFiscaisEmSengudoTrecho.Contains(o.Codigo)).ToList();

                //Quando não selecionar as notas que devem ser enviadas, todas serão enviadas, porém não gera registro no CarregamentoPedidoNotaFiscal. Neste caso deve desconsiderar o restante das notas e considerar como totalmente carregado...
                if (pedidoNotasFiscais.Count > 0 && notasPendentes.Count > 0 && carregamentoNotasFiscais.Count == 0 && !(carregamento.TipoOperacao?.PedidoColetaEntrega ?? false))
                {
                    carregamentoNotasFiscais = notasPendentes;
                    notasPendentes = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                }

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidoJaAdicionado = repCarregamentoPedido.BuscarPorPedido(carregamentoPedido.Pedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoJaAdicionado in carregamentosPedidoJaAdicionado)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentosPedidoNotasFiscaisJaAdicionado = repositorioCarregamentoPedidoNotaFiscal.BuscarPorCarregamentoPedido(carregamentoPedidoJaAdicionado.Codigo);
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> carregamentoNotasFiscaisJaAdicionado = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                    carregamentosPedidoNotasFiscaisJaAdicionado.ForEach(nf => carregamentoNotasFiscaisJaAdicionado.AddRange(nf.NotasFiscais));

                    if (carregamento.Codigo != carregamentoPedidoJaAdicionado.Carregamento.Codigo && !(carregamento.TipoOperacao?.PedidoColetaEntrega ?? false) && carregamentoNotasFiscais.Any(o => carregamentoNotasFiscaisJaAdicionado.Contains(o)))
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoJaFoiAdicionadoOutraCargaParaTipoDeOperacao, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamentoPedidoJaAdicionado.Carregamento.Codigo, carregamento.TipoOperacao.Descricao));

                    if (carregamentoNotasFiscaisJaAdicionado.Count > 0)
                        notasPendentes.RemoveAll(nota => carregamentoNotasFiscaisJaAdicionado.Contains(nota));
                }

                if (notasPendentes.Count > 0)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                    Servicos.Log.TratarErro($"Pedido {carregamentoPedido.Pedido.NumeroPedidoEmbarcador} - Liberado saldo pedido {carregamentoPedido.Pedido.PesoSaldoRestante}. Totalmente carregado : False. MontagemCargaController.VerificarPedidosColetaEntrega", "SaldoPedido");
                    carregamentoPedido.Pedido.PedidoTotalmenteCarregado = false;
                }
                else
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                    Servicos.Log.TratarErro($"Pedido {carregamentoPedido.Pedido.NumeroPedidoEmbarcador} - Liberado saldo pedido {carregamentoPedido.Pedido.PesoSaldoRestante}. Totalmente carregado : True. MontagemCargaController.VerificarPedidosColetaEntrega", "SaldoPedido");
                    carregamentoPedido.Pedido.PedidoTotalmenteCarregado = true;
                }

                carregamentoPedido.Pedido.PedidoColetaEntrega = true;

                repPedido.Atualizar(carregamentoPedido.Pedido);
            }
        }

        public void AjustarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento situacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga repCarregamentoCarga = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargas = repCarregamentoCarga.BuscarPorCarregamento(carregamento.Codigo);
            if (carregamentoCargas.Count > 0)
            {
                List<string> codigosAgrupados = new List<string>();
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga in carregamentoCargas)
                    codigosAgrupados.Add(carregamentoCarga.Carga.CodigoCargaEmbarcador);

                string codigoCarregamento = codigosAgrupados[0];
                string diferenca = "";
                int indiceDife = 0;
                for (int i = 1; i < codigosAgrupados.Count; i++)
                {
                    for (int j = 0; j < codigosAgrupados[i].Length; j++)
                    {

                        if (codigosAgrupados[i].Length > j && codigosAgrupados[i][j] != codigosAgrupados[0][j])
                        {
                            indiceDife = j;
                            break;
                        }
                    }
                    for (int j = indiceDife; j < codigosAgrupados[i].Length; j++)
                        diferenca += codigosAgrupados[i][j];


                    codigoCarregamento += "/" + diferenca;

                }
                carregamento.NumeroCarregamento = Utilidades.String.Left(codigoCarregamento, 300);
            }
            else
                carregamento.SituacaoCarregamento = situacaoCarregamento;

            carregamento.CarregamentoIntegradoERP = false;

            repCarregamento.Atualizar(carregamento);
        }

        public string AtualizarPedidosCargaCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool montagemCargaPorPedidoProduto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos where obj.Pedido.Filial == filial select obj.Pedido).ToList();
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carregamento.Codigo);

            string retorno = serCarga.CriarCargaPorPedidos(ref carga, pedidos, tipoServicoMultisoftware, apolicesSeguro, _unitOfWork, _configuracaoEmbarcador, null, montagemCargaPorPedidoProduto, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, carregamentoPedidos);

            new Servicos.Embarcador.Integracao.IntegracaoCarregamento(_unitOfWork).AdicionarIntegracoesCarregamento(carregamento, carregamentoPedidos, StatusCarregamentoIntegracao.Atualizar, carregamentoGeradoViaWebService: false);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            DefinirDadosPelosPedidosDoCarregamento(carregamento, listaCargasPedido, carregamentoPedidos);

            if (!string.IsNullOrWhiteSpace(retorno))
                return null;

            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga();
            ProcessamentoPorGeracaoCarga(carregamento, carga, pedidos.First().Codigo, tipoServicoMultisoftware, clienteMultisoftware, listaCargasPedido, configuracaoGeralCarga, propriedades);

            return retorno;
        }

        public string CalcularFreteTodoCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            //Comentado pois foi feito para a Danone e não é mais utilizado (RODRIGO)
            //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            //if (carga.Carregamento != null)
            //{
            //    if (repCarga.VerificarCargasSemCalculoPorCarregamento(carga.Carregamento.Codigo, carga.Codigo))
            //        return "Existem cargas do mesmo carregamento que não estão na etapa de cálculo de frete, não é possível cálcular o frete do carregamento até que todas as suas cargas estejam na etapa de cálculo de frete.";

            //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaCarregamento in carga.Carregamento.CargasFrete)
            //    {
            //        if (cargaCarregamento.Codigo != carga.Codigo)
            //        {
            //            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
            //            {
            //                carga.CalculandoFrete = true;
            //                repCarga.Atualizar(carga);
            //            }
            //        }
            //    }
            //}
            return "";
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno GerarCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades, string urlAcessoCliente, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto modoMontagemCargaPedidoProduto = propriedades.MontagemCargaPedidoProduto;
            Dominio.Entidades.Usuario usuario = propriedades.Usuario;

            Servicos.Log.TratarErro("Montagem: GerarCarga - " + carregamento.NumeroCarregamento, "MontagemCarga");

            if (carregamento.SituacaoCarregamento == SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                throw new ServicoException($"O carregamento nº {carregamento.NumeroCarregamento} precisa ser aprovado para gerar carga");

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(_unitOfWork);

            CarregamentoAprovacao servicoCarregamentoAprovacao = new CarregamentoAprovacao(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisOrdenada = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            ValidarCapacidadeModeloVeicularCarga(carregamento);
            ValidarDisponibilidadeVeiculo(carregamentoPedidos);

            if (servicoCarregamentoAprovacao.IsCriarAprovacaoCarregamento(carregamento))
            {
                servicoCarregamentoAprovacao.CriarAprovacao(carregamento, tipoServicoMultisoftware);

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                    return new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno()
                    {
                        CarregamentoAguardandoAprovacao = true,
                        NumeroCarregamento = carregamento.NumeroCarregamento
                    };
            }

            if (filiais.Count > 1)
            {
                Servicos.Log.TratarErro("Montagem: GerarCarga - " + carregamento.NumeroCarregamento + " filiais.Count > 1 ", "MontagemCarga");
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                if (carregamentoRoteirizacao != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota primeiraColeta = repCarregamentoRoteirizacaoClientesRota.BuscarPrimeiraColeta(carregamentoRoteirizacao.Codigo);

                    if (primeiraColeta != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoPrimeiraColeta = (from obj in carregamentoPedidos where (obj.Pedido.Remetente != null && obj.Pedido.Remetente.CPF_CNPJ == primeiraColeta.Cliente.CPF_CNPJ) || (obj.Pedido.Recebedor != null && obj.Pedido.Recebedor.CPF_CNPJ == primeiraColeta.Cliente.CPF_CNPJ) select obj.Pedido).FirstOrDefault();

                        if (pedidoPrimeiraColeta?.Filial != null)
                            filiaisOrdenada.Add(pedidoPrimeiraColeta?.Filial);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiais)
            {
                if (!filiaisOrdenada.Contains(filial))
                    filiaisOrdenada.Add(filial);
            }

            bool carregamentoComMultiplasFiliais = false;
            string sequencialCargaAlfanumerico = servicoCarga.ObtemProximoSequencialAlfanumericoCarga(_unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (filiaisOrdenada.Count > 1)
                    throw new ServicoException("Não é possível gerar a carga com pedidos de filiais diferentes");

                bool calcularPorLote = (carregamento != null && carregamentoPedidos != null && carregamentoPedidos.Count > 0 && carregamentoPedidos.Count >= _configuracaoEmbarcador.QuantidadeCargaPedidoProcessamentoLote);
                if (usuario == null && carregamento != null && carregamentoPedidos != null && carregamentoPedidos.Count > 0 && carregamentoPedidos.Any(p => p.Pedido.Usuario != null))
                    usuario = carregamentoPedidos.FirstOrDefault().Pedido?.Usuario;

                bool montagemCargaPorPedidoProduto = modoMontagemCargaPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Sim;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarCargaPorCarregamento(carregamento, filiaisOrdenada.FirstOrDefault(), carregamentoPedidos, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoGeralCarga, calcularPorLote ? Dominio.Enumeradores.LoteCalculoFrete.Integracao : Dominio.Enumeradores.LoteCalculoFrete.Padrao, carregamentoComMultiplasFiliais, montagemCargaPorPedidoProduto, propriedades, string.Empty);

                if (usuario != null)
                    new Servicos.Embarcador.Carga.CargaOperador(_unitOfWork).Atualizar(carga, usuario, tipoServicoMultisoftware);

                if (carga != null)
                {
                    cargas.Add(carga);
                    if (!ValidarMotoristaGR(carga.Codigo, tipoServicoMultisoftware, out string msgErro))
                        Servicos.Log.TratarErro("Erro validação GR " + msgErro, "MontagemCarga");

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                    if (cargaPedidos != null && cargaPedidos.Count > 0)
                        Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, _configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                }
            }
            else
            {
                Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(_unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);

                carregamentoComMultiplasFiliais = filiaisOrdenada.Count > 1;

                foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiaisOrdenada)
                {
                    bool montagemCargaPorPedidoProduto = false;

                    if (modoMontagemCargaPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Sim)
                        montagemCargaPorPedidoProduto = true;
                    else if (modoMontagemCargaPedidoProduto == Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar)
                    {
                        Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(filial.Codigo);
                        montagemCargaPorPedidoProduto = centroCarregamento?.MontagemCarregamentoPedidoProduto ?? false;
                    }

                    if (propriedades.Usuario == null && carregamento != null)
                        propriedades.Usuario = carregamento.SessaoRoteirizador?.Usuario;

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarCargaPorCarregamento(carregamento, filial, carregamentoPedidos, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoGeralCarga, Dominio.Enumeradores.LoteCalculoFrete.Padrao, carregamentoComMultiplasFiliais, montagemCargaPorPedidoProduto, propriedades, sequencialCargaAlfanumerico);

                    if (carga != null)
                    {
                        cargas.Add(carga);
                        servicoIntegracaoOrdemEmbarqueMarfrig.AdicionarCargaIntegracaoOrdemEmbarque(carga);
                    }
                }
            }

            Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = null;

            if (cargas.Count > 1)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    carga.CalculandoFrete = false;
                    carga.DataInicioCalculoFrete = null;
                }

                cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(_unitOfWork, _configuracaoEmbarcador, configuracaoGeralCarga).AgruparCargas(null, cargas, null, carregamento.NumeroCarregamento, filiaisOrdenada.FirstOrDefault(), null, carregamento.Empresa ?? carregamento.Veiculo?.Empresa, tipoServicoMultisoftware, ClienteMultisoftware, limparOrdemColetaEntrega: false, carregamento);

                if (auditado != null)
                    Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, $"Criada pelo agrupamento das cargas {string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList())} na montagem de carga", _unitOfWork);
            }
            else
            {
                cargaAgrupada = cargas.FirstOrDefault();

                if (auditado != null)
                    if (carregamento.ImportadaComDocumentacaoDuplicadaMontagemFeeder)
                        Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CriadaNaMontagemDeCargaUsandoParametroDuplicidade, cargaAgrupada.Codigo), _unitOfWork);
                    else
                        Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CriadaNaMontagemDeCarga, cargaAgrupada.Codigo), _unitOfWork);
            }

            if (carregamento.ValorFreteManual > 0m)
            {
                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaAgrupada.Codigo);

                cargaAgrupada.CalculandoFrete = false;
                cargaAgrupada.DataInicioCalculoFrete = null;
                cargaAgrupada.ValorFrete = carregamento.ValorFreteManual;
                cargaAgrupada.ValorFreteOperador = carregamento.ValorFreteManual;
                cargaAgrupada.TipoFreteEscolhido = TipoFreteEscolhido.Operador;

                if ((cargaAgrupada.SituacaoCarga == SituacaoCarga.CalculoFrete) && !cargaAgrupada.ExigeNotaFiscalParaCalcularFrete)
                    cargaAgrupada.SituacaoCarga = SituacaoCarga.AgTransportador;

                servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaAgrupada, cargaPedidos, _configuracaoEmbarcador, false, _unitOfWork, tipoServicoMultisoftware);
            }

            bool motoristaPrecisaConfirmarCarga = cargaAgrupada.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false && carregamento.TempoLimiteConfirmacaoMotorista != null && carregamento.TempoLimiteConfirmacaoMotorista.TotalSeconds > 0;
            if (motoristaPrecisaConfirmarCarga)
            {
                cargaAgrupada.DataLimiteConfirmacaoMotorista = DateTime.Now.Add(carregamento.TempoLimiteConfirmacaoMotorista);
                cargaAgrupada.SituacaoCarga = SituacaoCarga.Nova;

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

                //ja vamos criar a msg de carga sem a confirmação do motorista que vai servir como bloqueio na etapa 1;
                servicoMensagemAlerta.Adicionar(cargaAgrupada, TipoMensagemAlerta.CargaSemConfirmacaoMotorista, "Carga aguardando a confirmação do motorista");
            }

            if (_configuracaoEmbarcador.BloquearGeracaoCargaComJanelaCarregamentoExcedente || configuracaoGeralCarga.UtilizarProgramacaoCarga)
            {
                if (cargaAgrupada.Empresa == null)
                {
                    if (!new PreCarga.PreCarga(_unitOfWork).VincularPrimeiraPreCarga(cargaAgrupada, tipoServicoMultisoftware))
                    {
                        if (configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente)
                        {
                            try
                            {
                                new Logistica.FilaCarregamentoVeiculo(_unitOfWork, auditado?.Usuario, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware)).AlocarParaPrimeiroDaFila(cargaAgrupada, tipoServicoMultisoftware);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCarga.AdicionarCargaJanelaCarregamento(cargaAgrupada, _configuracaoEmbarcador, tipoServicoMultisoftware, _unitOfWork, auditado, propriedades);

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento()
                {
                    PermitirHorarioDescarregamentoComLimiteAtingido = propriedades?.PermitirHorarioDescarregamentoComLimiteAtingido ?? false,
                    PermitirHorarioDescarregamentoInferiorAoAtual = propriedades?.PermitirHorarioDescarregamentoInferiorAoAtual ?? false
                };

                try
                {
                    Log.TratarErro($"Chamanda do método AdicionarCargaJanelaDescarregamento via montagem de carga", "GeracaoJanelaDescarga");
                    servicoCarga.AdicionarCargaJanelaDescarregamento(cargaAgrupada, cargaJanelaCarregamento, _configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware, configuracoesDescarregamento);
                    Log.TratarErro($"Terminada a chamada do método AdicionarCargaJanelaDescarregamento sem exceção via montagem de carga", "GeracaoJanelaDescarga");
                }
                catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioDescarregamentoIndisponivel)
                {
                    if (!(propriedades?.PermitirGerarCargaSemJanelaDescarregamento ?? false))
                        throw;

                    Log.TratarErro($"Carga gerada sem descarga automática via montagem de carga. Motivo: {excecao.Message}", "GeracaoJanelaDescarga");

                    if (auditado != null)
                        Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, $"Carga gerada sem descarga automática. Motivo: {excecao.Message}", _unitOfWork);
                }

                servicoCarga.AtualizarDataEstufagemDadosTransporteMaritimo(cargaAgrupada, _unitOfWork, true); //foi colocado aqui pois esse processo é da marfrig...
            }

            repositorioCarga.Atualizar(cargaAgrupada);

            /* Se tudo deu certo na hora da criação da carga e o motorista precisa confirmar ela,
            vamos enviar uma notificação para ele ter a possibilidade de confirmar */
            if (motoristaPrecisaConfirmarCarga)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(cargaAgrupada.Codigo);
                if (cargaMotoristas != null && cargaMotoristas.Count > 0)
                {
                    List<Dominio.Entidades.Usuario> motoristaUser = new List<Dominio.Entidades.Usuario>();
                    foreach (var cargaMotorista in cargaMotoristas)
                        motoristaUser.Add(cargaMotorista.Motorista);

                    NotificacaoMTrack notificacaoMTrack = new NotificacaoMTrack(_unitOfWork);
                    notificacaoMTrack.NotificarMudancaCarga(cargaAgrupada, motoristaUser, AdminMultisoftware.Dominio.Enumeradores.MobileHubs.CargaMotoristaNecessitaConfirmar, true, 0);
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaAgrupada.Codigo);
                Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEmailCargaTransportador(cargaAgrupada, cargaPedidos, _unitOfWork);
            }

            return new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno()
            {
                NumeroCarregamento = carregamento.NumeroCarregamento,
                NumerosCargasGeradas = string.Join(", ", cargas.Select(o => o.CodigoCargaEmbarcador))
            };
        }

        public void GerarCargaEmLote(List<int> codigosCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, int codigoSessaoRoteirizador, bool gerarCargaCarregamentoBackground, string urlAcessoCliente)
        {
            Servicos.Log.TratarErro("Montagem: GerarCargaEmLote - " + string.Join(",", codigosCarregamento), "MontagemCarga");
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(_unitOfWork);
            Servicos.Embarcador.Hubs.MontagemCarga servicoNotificacaomontagemCarga = new Servicos.Embarcador.Hubs.MontagemCarga();

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = repSessaoRoteirizador.BuscarPorCodigo(codigoSessaoRoteirizador);
            Dominio.Entidades.Usuario usuario = sessaoRoteirizador?.Usuario;

            try
            {
                if (codigosCarregamento != null)
                    codigosCarregamento = codigosCarregamento.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            if (!gerarCargaCarregamentoBackground)
                servicoNotificacaomontagemCarga.InformarQuantidadeProcessadosCargaEmLote(codigosCarregamento.Count, 0, codigoSessaoRoteirizador);

            int count = 0;

            string erro = "";
            List<int> codigosProcessados = new List<int>();
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

            ObterConfiguracaoEmbarcador();

            //Guarda para informar o HUB em caso de erro...
            int codigoCarregamentoAtual = 0;
            string numeroCarregamento = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
            {
                MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar,
                Usuario = usuario
            };

            foreach (int codigoCarregamento in codigosCarregamento)
            {
                if (!codigosProcessados.Contains(codigoCarregamento))
                {
                    _unitOfWork.FlushAndClear();

                    codigoCarregamentoAtual = codigoCarregamento;
                    codigosProcessados.Add(codigoCarregamento);
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
                    List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in carregamentoPedidos select obj.Pedido.Filial).Distinct().ToList();

                    try
                    {
                        numeroCarregamento = carregamento.NumeroCarregamento;

                        _unitOfWork.Start();

                        Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno retorno = GerarCarga(carregamento, filiais, carregamentoPedidos, tipoServicoMultisoftware, ClienteMultisoftware, null, propriedades, urlAcessoCliente);
                        Servicos.Log.TratarErro($"Montagem: {codigoCarregamento} | carga(s): {(retorno.CarregamentoAguardandoAprovacao ? "Aguardando Aprovação" : retorno.NumerosCargasGeradas)}", "MontagemCarga");

                        _unitOfWork.CommitChanges();

                        count++;

                        if (gerarCargaCarregamentoBackground)
                            servicoNotificacaomontagemCarga.InformarCargaBackgroundFinalizado("", codigoSessaoRoteirizador, codigoCarregamento, carregamento.NumeroCarregamento);
                        else
                            servicoNotificacaomontagemCarga.InformarQuantidadeProcessadosCargaEmLote(codigosCarregamento.Count, count, codigoSessaoRoteirizador);
                    }
                    catch (ServicoException excecao)
                    {
                        erro = excecao.Message;
                        _unitOfWork.Rollback();
                        OcorreuErroGerarCarga(codigoCarregamento, gerarCargaCarregamentoBackground);
                        break;
                    }
                    catch (Exception excecao)
                    {
                        erro = $"Ocorreu um erro ao gerar a carga para o carregamento {carregamento?.NumeroCarregamento} ";
                        _unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao);
                        OcorreuErroGerarCarga(codigoCarregamento, gerarCargaCarregamentoBackground);
                        break;
                    }
                }
            }

            if (!gerarCargaCarregamentoBackground)
                servicoNotificacaomontagemCarga.InformarCargaEmLoteFinalizado(erro, codigoSessaoRoteirizador);
            else if (!string.IsNullOrEmpty(erro))
                servicoNotificacaomontagemCarga.InformarCargaBackgroundFinalizado(erro, codigoSessaoRoteirizador, codigoCarregamentoAtual, numeroCarregamento);
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros ObterSessaoRoteirizadorParametros(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = null;
            if (!string.IsNullOrEmpty(sessaoRoteirizador.Parametros))
                sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(sessaoRoteirizador.Parametros);

            return sessaoRoteirizadorParametros;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao ObterTipoUltimoPontoRoteirizacao(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            // Alteração do tipo ultimo ponto para quando todos os pedidos do carregamento for do mesmo tipo de operação, roteirizar com o tipo ultimo ponto do tipo de operação
            // caso possuir mais de um tipo de operação, vamos pegar a configuração geral do sistema.
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from ped in pedidos
                                                                                     where ped.TipoOperacao != null
                                                                                     select ped.TipoOperacao).Distinct().ToList();

            if (tiposOperacao?.Count == 1)
                tipoUltimoPonto = tiposOperacao[0]?.TipoUltimoPontoRoteirizacao ?? tipoUltimoPonto;
            else
                tipoUltimoPonto = _configuracaoEmbarcador?.TipoUltimoPontoRoteirizacao ?? tipoUltimoPonto;

            return tipoUltimoPonto;
        }

        public void ValidarCapacidadeModeloVeicularCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (carregamento.ModeloVeicularCarga == null)
                return;

            if (!_configuracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamento = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
            decimal pesoTotal = pedidosCarregamento.Sum(o => o.Peso);
            decimal cubagemTotal = pedidosCarregamento.Sum(o => o.Pedido.CubagemTotal);
            decimal totalPallets = pedidosCarregamento.Sum(o => o.Pallet);// o.Pedido.NumeroPaletes + o.Pedido.NumeroPaletesFracionado);

            if (carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto == true)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCPP = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
                var produtos = repCPP.BuscarPorCarregamento(carregamento.Codigo);
                pesoTotal = produtos.Sum(o => o.Peso);
                cubagemTotal = produtos.Sum(o => o.MetroCubico);
                totalPallets = produtos.Sum(o => o.QuantidadePallet);
            }

            cubagemTotal += (carregamento.TipoDeCarga?.Paletizado ?? false) ? carregamento.ModeloVeicularCarga.ObterOcupacaoCubicaPaletes() : 0m;

            decimal capacidadeMaxima = (carregamento.ModeloVeicularCarga.CapacidadePesoTransporte + carregamento.ModeloVeicularCarga.ToleranciaPesoExtra);
            if (pesoTotal > capacidadeMaxima)
                throw new ServicoException($"O peso {pesoTotal.ToString(".00")} do carregamento {carregamento.NumeroCarregamento} excede a capacidade {capacidadeMaxima.ToString(".00")} do modelo veicular {carregamento.ModeloVeicularCarga.Descricao}.");

            if (carregamento.ModeloVeicularCarga.VeiculoPaletizado && (totalPallets > carregamento.ModeloVeicularCarga.NumeroPaletes))
                throw new ServicoException($"O número {totalPallets.ToString(".00")} de pallets do carregamento {carregamento.NumeroCarregamento} excede a capacidade {carregamento.ModeloVeicularCarga.NumeroPaletes.Value.ToString(".00")} do modelo veicular {carregamento.ModeloVeicularCarga.Descricao}.");

            if (carregamento.ModeloVeicularCarga.ModeloControlaCubagem && (cubagemTotal > carregamento.ModeloVeicularCarga.Cubagem))
                throw new ServicoException($"A cubagem {cubagemTotal.ToString(".00")} do carregamento {carregamento.NumeroCarregamento} excede a capacidade {carregamento.ModeloVeicularCarga.Cubagem.ToString(".00")} do modelo veicular {carregamento.ModeloVeicularCarga.Descricao}.");
        }

        public void ValidarDisponibilidadeVeiculo(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = carregamentoPedidos.Select(o => o.Pedido).ToList();

            Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao ordemServicoVeiculoManutencao = new Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao(_unitOfWork);
            ordemServicoVeiculoManutencao.VeiculoIndisponivelParaTransporte(pedidos);
        }

        public void GerarCargaCarregamentoWS(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            ValidarCarregamento(carregamento, (carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false), configuracaoMontagemCarga, configuracaoEmbarcador, tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in carregamentoPedidos select obj.Pedido.Filial).Distinct().ToList();

            bool gerarCargaBackground = false;
            if ((carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false) == true)
                gerarCargaBackground = true;

            if (!gerarCargaBackground)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                {
                    MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar,
                    Usuario = null
                };

                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno retorno = GerarCarga(carregamento, filiais, carregamentoPedidos, tipoServicoMultisoftware, clienteMultisoftware, auditado, propriedades, clienteAcesso.URLAcesso);
                VerificarPedidosColetaEntrega(carregamento);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos select obj.Pedido).ToList();
                servicoCarga.VisibilidadesDasCargas(pedidos, carregamento, _unitOfWork, false);


                //#43261
                // Devemos enviar um email, quando os pedidos possuem um transportador que esteja com a flag Contratante marcada...
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repositorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repositorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                int codigoTransportadoraPadraoContratacao = configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0;

                bool existeTransportadoraPadraoContratacaoPedidos = carregamentoPedidos.Any(x => (x.Pedido?.Empresa?.Codigo ?? 0) == codigoTransportadoraPadraoContratacao);
                if (existeTransportadoraPadraoContratacaoPedidos)
                {
                    if ((configuracaoTransportador?.ExisteTransportadorPadraoContratacao ?? false) && (codigoTransportadoraPadraoContratacao == (configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0)) && ((carregamento?.Empresa?.Codigo ?? 0) != (configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0)))
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEmailTransportadorRetira(carregamento, carregamentoPedidos, "Agendamento coleta", _unitOfWork);
                }

                //return new JsonpResult(new
                //{
                //	retorno.CarregamentoAguardandoAprovacao,
                //	retorno.NumerosCargasGeradas,
                //	GerandoCargaBackground = false
                //});
            }
            else
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(_unitOfWork);

                carregamento.SituacaoCarregamento = SituacaoCarregamento.GerandoCargaBackground;
                repositorioCarregamento.Atualizar(carregamento);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento montagemCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento()
                {
                    Carregamento = carregamento
                };
                repositorioMontagemCarregamento.Inserir(montagemCarregamento);

                //return new JsonpResult(new
                //{
                //	CarregamentoAguardandoAprovacao = false,
                //	NumerosCargasGeradas = string.Empty,
                //	GerandoCargaBackground = true
                //});
            }
        }

        #endregion Métodos Públicos
    }
}