using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class Chamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.Chamado>
    {
        #region Construtores

        public Chamado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Chamado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa)
        {
            var consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            if (filtrosPesquisa.CodigoCarga > 0)
                consultaChamado = consultaChamado.Where(o => o.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.SomenteValoresPendentes)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

                consultaChamado = consultaChamado.Where(o => !consultaChamadoOcorrencia.Any(c => c.Chamado.Codigo == o.Codigo) ||
                                                            consultaChamadoOcorrencia.Any(c => c.Chamado.Codigo == o.Codigo && (c.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Anulada ||
                                                            c.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada || c.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Rejeitada)) && o.Valor > 0);
            }

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaChamado = consultaChamado.Where(o => o.SetorResponsavel.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.NumeroLote > 0)
                consultaChamado = consultaChamado.Where(o => o.LoteChamadoOcorrencia.NumeroLote == filtrosPesquisa.NumeroLote);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (!filtrosPesquisa.CodigosTransportador.IsNullOrEmpty())
            {
                List<int> codigosEmpresa = ObterCodigosEmpresasParaPesquisa(filtrosPesquisa.CodigosTransportador);
                codigosEmpresa.AddRange(filtrosPesquisa.CodigosTransportador);
                consultaChamado = consultaChamado.Where(o => codigosEmpresa.Contains(o.Empresa.Codigo));
            }
            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
            {
                if (filtrosPesquisa.CodigosFilial?.Count > 0)
                    consultaChamado = consultaChamado.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo) || o.Carga.Pedidos.Any(ped => ped.Recebedor != null && filtrosPesquisa.CodigosRecebedor.Contains(ped.Recebedor.CPF_CNPJ)));
            }
            else if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    consultaChamado = consultaChamado.Where(o => o.Carga.CodigoCargaEmbarcador.Contains(filtrosPesquisa.CodigoCargaEmbarcador) || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));
                else
                    consultaChamado = consultaChamado.Where(o => o.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.CodigoCargaEmbarcador) || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));
            }

            if (filtrosPesquisa.CpfCnpjCliente > 0d)
                consultaChamado = consultaChamado.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjCliente);

            if (filtrosPesquisa.CpfCnpjTomador > 0d)
                consultaChamado = consultaChamado.Where(o => o.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaChamado = consultaChamado.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario);

            if (filtrosPesquisa.CodigoGrupoPessoasCliente > 0)
                consultaChamado = consultaChamado.Where(o => o.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasCliente);

            if (filtrosPesquisa.CodigoGrupoPessoasTomador > 0)
                consultaChamado = consultaChamado.Where(o => o.Tomador.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomador);

            if (filtrosPesquisa.CodigoGrupoPessoasDestinatario > 0)
                consultaChamado = consultaChamado.Where(o => o.Destinatario.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasDestinatario);

            if (filtrosPesquisa.CodigoResponsavel > 0)
                consultaChamado = consultaChamado.Where(o => o.Responsavel.Codigo == filtrosPesquisa.CodigoResponsavel);

            if (filtrosPesquisa.CodigoResponsavelPorRegra > 0)
                consultaChamado = consultaChamado.Where(o => o.Responsavel.Codigo == filtrosPesquisa.CodigoResponsavelPorRegra || o.Responsavel == null);

            if (filtrosPesquisa.CodigosMotivoChamado?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.CodigosMotivoChamado.Contains(o.MotivoChamado.Codigo));

            if (filtrosPesquisa.SituacaoChamado != SituacaoChamado.Todas)
            {
                if (filtrosPesquisa.SituacaoChamado == SituacaoChamado.EmTratativa)
                    consultaChamado = consultaChamado.Where(o => o.Situacao == SituacaoChamado.Aberto && o.Responsavel != null);
                else if (filtrosPesquisa.SituacaoChamado == SituacaoChamado.Aberto)
                    consultaChamado = consultaChamado.Where(o => o.Situacao == SituacaoChamado.Aberto && o.Responsavel == null);
                else
                    consultaChamado = consultaChamado.Where(o => o.Situacao == filtrosPesquisa.SituacaoChamado);
            }

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataFinal.Value);

            if (filtrosPesquisa.NotaFiscal > 0)
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.NotasFiscais.Any(n => n.XMLNotaFiscal.Numero == filtrosPesquisa.NotaFiscal));

                consultaChamado = consultaChamado.Where(o => (consultaCargaPedido.Any(n => n.Carga.Codigo == o.Carga.Codigo) && o.CargaEntrega == null)
                ||
                o.CargaEntrega.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == filtrosPesquisa.NotaFiscal));
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaChamado = consultaChamado.Where(o => o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(vei => vei.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                consultaChamado = consultaChamado.Where(o => o.CargaEntrega != null ? o.CargaEntrega.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.CargaPedido.Pedido.FilialVenda.Codigo)) :
                                                             o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

            if (filtrosPesquisa.CodigosOcorrencia?.Count > 0)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                    .Where(o => filtrosPesquisa.CodigosOcorrencia.Contains(o.CargaOcorrencia.Codigo));

                consultaChamado = consultaChamado.Where(o => consultaChamadoOcorrencia.Any(c => c.Chamado.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.ComOcorrencia != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
                if (filtrosPesquisa.ComOcorrencia == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    consultaChamado = consultaChamado.Where(o => consultaChamadoOcorrencia.Any(c => c.Chamado.Codigo == o.Codigo));
                else
                    consultaChamado = consultaChamado.Where(o => !consultaChamadoOcorrencia.Any(c => c.Chamado.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.ComDevolucao.HasValue)
            {
                if (filtrosPesquisa.ComDevolucao.Value)
                    consultaChamado = consultaChamado.Where(o => o.CargaEntrega.DevolucaoParcial && o.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao);
                else
                    consultaChamado = consultaChamado.Where(o => !o.CargaEntrega.DevolucaoParcial && o.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao);
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaChamado = consultaChamado.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CpfCnpjClienteResponsavel > 0d)
                consultaChamado = consultaChamado.Where(o => o.ClienteResponsavel.CPF_CNPJ == filtrosPesquisa.CpfCnpjClienteResponsavel);

            if (filtrosPesquisa.CodigoGrupoPessoasResponsavel > 0)
                consultaChamado = consultaChamado.Where(o => o.GrupoPessoasResponsavel.Codigo == filtrosPesquisa.CodigoGrupoPessoasResponsavel);

            if (filtrosPesquisa.ComNotaFiscalServico.HasValue)
            {
                if (filtrosPesquisa.ComNotaFiscalServico.Value)
                    consultaChamado = consultaChamado.Where(o => o.Anexos.Any(a => a.NotaFiscalServico));
                else
                    consultaChamado = consultaChamado.Where(o => o.Anexos.Any(a => ((bool?)a.NotaFiscalServico ?? false) == false));// || !o.Anexos.Any());
            }

            if (filtrosPesquisa.AguardandoTratativaDoCliente)
                consultaChamado = consultaChamado.Where(o => (o.AguardandoTratativaDoCliente && o.CargaEntrega.Pedidos.Any(pedido =>
                (pedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && pedido.CargaPedido.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.FornecedorLogado) ||
                (pedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && pedido.CargaPedido.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.FornecedorLogado) ||
                (pedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && pedido.CargaPedido.Recebedor.CPF_CNPJ == filtrosPesquisa.FornecedorLogado) ||
                (pedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && pedido.CargaPedido.Expedidor.CPF_CNPJ == filtrosPesquisa.FornecedorLogado) ||
                (pedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && pedido.CargaPedido.Tomador.CPF_CNPJ == filtrosPesquisa.FornecedorLogado)))
                || (o.Pedido != null && o.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.FornecedorLogado));

            if (filtrosPesquisa.ComResponsavel.HasValue)
            {
                if (filtrosPesquisa.ComResponsavel.Value)
                    consultaChamado = consultaChamado.Where(o => o.Responsavel != null || o.SetorResponsavel != null);
                else
                    consultaChamado = consultaChamado.Where(o => o.Responsavel == null && o.SetorResponsavel == null);
            }

            if (filtrosPesquisa.ComNovaMovimentacao.HasValue)
            {
                if (filtrosPesquisa.ComNovaMovimentacao.Value)
                    consultaChamado = consultaChamado.Where(o => o.NovaMovimentacao);
                else
                    consultaChamado = consultaChamado.Where(o => o.NovaMovimentacao != true);
            }

            if (filtrosPesquisa.CodigoPedido > 0)
                consultaChamado = consultaChamado.Where(o => o.Pedido.Codigo == filtrosPesquisa.CodigoPedido || o.Carga.Pedidos.Any(ped => ped.Pedido.Codigo == filtrosPesquisa.CodigoPedido));

            if (filtrosPesquisa.DataInicialAgendamentoPedido.HasValue)
                consultaChamado = consultaChamado.Where(o => o.Pedido.DataAgendamento.Value.Date >= filtrosPesquisa.DataInicialAgendamentoPedido.Value);

            if (filtrosPesquisa.DataFinalAgendamentoPedido.HasValue)
                consultaChamado = consultaChamado.Where(o => o.Pedido.DataAgendamento.Value.Date <= filtrosPesquisa.DataFinalAgendamentoPedido.Value);

            if (filtrosPesquisa.DataInicialColetaPedido.HasValue)
                consultaChamado = consultaChamado.Where(o => o.Pedido.DataCarregamentoPedido.Value.Date >= filtrosPesquisa.DataInicialColetaPedido.Value);

            if (filtrosPesquisa.DataFinalColetaPedido.HasValue)
                consultaChamado = consultaChamado.Where(o => o.Pedido.DataCarregamentoPedido.Value.Date <= filtrosPesquisa.DataFinalColetaPedido.Value);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedidoCliente))
                consultaChamado = consultaChamado.Where(o => o.Pedido.CodigoPedidoCliente == filtrosPesquisa.NumeroPedidoCliente);

            if (filtrosPesquisa.SomenteCargasCriticas)
                consultaChamado = consultaChamado.Where(o => o.Carga.Monitoramento.Any(monitoramento => monitoramento.Critico));

            if (filtrosPesquisa.SomenteAtendimentoComMsgNaoLida)
            {
                consultaChamado = consultaChamado.Where(o =>
                this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>()
                .Where(m => m.MensagemLida == false && m.Carga.Codigo == o.Carga.Codigo)
                .Any());


            }
            if (filtrosPesquisa.ClienteComplementar?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.ClienteComplementar.Contains(o.Pedido.Destinatario.CPF_CNPJ));

            if (filtrosPesquisa.CanalVenda > 0)
                consultaChamado = consultaChamado.Where(o => o.Pedido.CanalVenda.Codigo == filtrosPesquisa.CanalVenda);

            if (filtrosPesquisa.ModalTransporte > 0)
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.TipoCobrancaMultimodal == filtrosPesquisa.ModalTransporte);

                consultaChamado = consultaChamado.Where(o => consultaCargaPedido.Any(n => n.Carga.Codigo == o.Carga.Codigo));
            }

            if (filtrosPesquisa.SetorEscalationList.Count > 0)
            {
                var consultaGatilhosTempoList = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>()
                    .Where(o => filtrosPesquisa.SetorEscalationList.Contains(o.Setor.Codigo));

                consultaChamado = consultaChamado.Where(o => consultaGatilhosTempoList.Any(n => n.MotivoChamado.Codigo == o.MotivoChamado.Codigo && n.Nivel == o.Nivel));
            }

            if (filtrosPesquisa.Vendedores?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.Vendedores.Contains(o.Pedido.FuncionarioVendedor.Codigo));

            if (filtrosPesquisa.MesoRegiao?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.MesoRegiao.Contains(o.Cliente.MesoRegiao.Codigo));

            if (filtrosPesquisa.Regiao?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.Regiao.Contains(o.Cliente.Regiao.Codigo));

            if (filtrosPesquisa.UFDestino?.Count > 0)
                consultaChamado = consultaChamado.Where(o => filtrosPesquisa.UFDestino.Contains(o.Cliente.Localidade.Estado.Sigla));

            if (filtrosPesquisa.Parqueada.HasValue)
                consultaChamado = consultaChamado.Where(o => o.Carga.Parqueada == filtrosPesquisa.Parqueada);

            if (filtrosPesquisa.CodigoTiposCausadoresOcorrencia > 0)
                consultaChamado = consultaChamado.Where(obj => obj.TiposCausadoresOcorrencia.Codigo == filtrosPesquisa.CodigoTiposCausadoresOcorrencia);

            if (filtrosPesquisa.CodigoCausasMotivoCamado > 0)
                consultaChamado = consultaChamado.Where(obj => obj.CausasMotivoChamado.Codigo == filtrosPesquisa.CodigoCausasMotivoCamado);

            if (!string.IsNullOrEmpty(filtrosPesquisa.EscritorioVendas))
            {
                List<string> listaEscritorios = filtrosPesquisa.EscritorioVendas.Split(',').ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> queryClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
                queryClienteComplementar = queryClienteComplementar.Where(cliente => listaEscritorios.Contains(cliente.EscritorioVendas) || filtrosPesquisa.EscritorioVendas.Contains(cliente.EscritorioVendas));

                consultaChamado = consultaChamado.Where(o => queryClienteComplementar.Select(cliente => cliente.Cliente.CPF_CNPJ).Any(codigo => codigo == o.Cliente.CPF_CNPJ));
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.Matriz))
            {
                List<string> listaEscritorios = filtrosPesquisa.Matriz.Split(',').ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> queryClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
                queryClienteComplementar = queryClienteComplementar.Where(cliente => listaEscritorios.Contains(cliente.Matriz) || filtrosPesquisa.Matriz.Contains(cliente.Matriz));

                consultaChamado = consultaChamado.Where(o => queryClienteComplementar.Select(cliente => cliente.Cliente.CPF_CNPJ).Any(codigo => codigo == o.Cliente.CPF_CNPJ));
            }

            return consultaChamado;
        }

        private object? GetNestedPropertyValue(object obj, string propertyPath)
        {
            foreach (string part in propertyPath.Split('.'))
            {
                if (obj == null) return null;

                Type type = obj.GetType();
                PropertyInfo prop = type.GetProperty(part);
                if (prop == null) return null;

                obj = prop.GetValue(obj, null);
            }

            return obj;
        }

        private List<int> ObterCodigosEmpresasParaPesquisa(List<int> codigosTransportador)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(emp => codigosTransportador.Contains(emp.Codigo) && emp.MostrarOcorrenciasFiliaisMatriz)
                .SelectMany(x => x.Filiais.Select(f => f.Codigo))
                .ToList();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.Chamado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Carga)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Carga)
                .FirstOrDefaultAsync(CancellationToken);
        }


        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCodigos(List<int> codigos)
        {
            var consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaChamado.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorPendentesNotificacaoPorUsuarioMobile(int usuarioMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Motoristas.Any(mt => mt.CodigoMobile == usuarioMobile) && obj.NotificacaoMotoristaMobile select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarAtendimentosPorCargaComEntrega(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.CargaEntrega != null && obj.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Atendimento select obj;
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.MotivoChamado)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarAtendimentosPorEntrega(int entrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == entrega && obj.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Atendimento select obj;
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.MotivoChamado)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarAtendimentosPorEntregaEmAberto(int entrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == entrega && obj.Situacao != SituacaoChamado.Finalizado && obj.Situacao != SituacaoChamado.Cancelada && obj.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Atendimento select obj;
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.MotivoChamado)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }


        public Dominio.Entidades.Embarcador.Chamados.Chamado BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCargaEMotivo(int carga, int motivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.MotivoChamado.Codigo == motivo select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarPorCargaEMotivoAsync(int codigoCarga, int codigoMotivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && obj.MotivoChamado.Codigo == codigoMotivo);

            return query.ToListAsync(CancellationToken);
        }

        public bool JaExisteAtendimentoPorCargaMotivo(int carga, int motivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = query.Any(obj => obj.Carga.Codigo == carga && obj.MotivoChamado.Codigo == motivo);
            return result;
        }

        public Task<bool> JaExisteAtendimentoPorCargaMotivoAsync(int carga, int motivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(obj => obj.Carga.Codigo == carga && obj.MotivoChamado.Codigo == motivo);

            return query.AnyAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCargaMotivoDestinatario(int carga, int motivo, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.MotivoChamado.Codigo == motivo && obj.Destinatario.CPF_CNPJ == destinatario select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarPorCargaMotivoDestinatarioAsync(int codigoCarga, int codigoMotivo, double cnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(obj => obj.Carga.Codigo == codigoCarga &&
                              obj.MotivoChamado.Codigo == codigoMotivo &&
                              obj.Destinatario.CPF_CNPJ == cnpjDestinatario
                );

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarListaPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.OrderByDescending(obj => obj.Codigo).Fetch(obj => obj.Responsavel).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarListaPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.OrderByDescending(obj => obj.Codigo).Fetch(obj => obj.Responsavel).ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarListaPorCargaECriticidadeAsync(int codigoCarga, int codgoChamadoIgnorar, int numeroCriticidadeAtendimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(obj => obj.Carga.Codigo == codigoCarga &&
                              obj.Codigo != codgoChamadoIgnorar &&
                              obj.MotivoChamado.NumeroCriticidadeAtendimento <= numeroCriticidadeAtendimento &&
                              obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada
                );

            return query.OrderByDescending(obj => obj.Codigo).Fetch(obj => obj.Responsavel).ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Chamados.Chamado BuscarPorCargaEntrega(int cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == cargaEntrega select obj;
            return result.OrderByDescending(obj => obj.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where cargas.Contains(obj.Carga.Codigo) select obj;
            return result.OrderByDescending(obj => obj.Codigo).Fetch(obj => obj.Responsavel).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarListaPorCargaEntrega(int cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == cargaEntrega select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarAbertosOuEmTratativaPorCargaEntrega(int cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where obj.CargaEntrega.Codigo == cargaEntrega &&
                               (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa)
                         select obj;
            return result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarListaPorCargaEntrega(int codigoCargaEntrega, TipoMotivoAtendimento tipoMotivoAtendimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.MotivoChamado.TipoMotivoAtendimento == tipoMotivoAtendimento select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.Chamado>> BuscarListaPorCargaEntregaAsync(int codigoCargaEntrega, TipoMotivoAtendimento tipoMotivoAtendimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.MotivoChamado.TipoMotivoAtendimento == tipoMotivoAtendimento);

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarListaPorCargasDoMotorista(int codigoMotorista, List<TipoMotivoAtendimento> tipoMotivoAtendimentos = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            var result = from obj in query where obj.Carga.Motoristas.Any(o => o.Codigo == codigoMotorista) select obj;

            if (tipoMotivoAtendimentos != null && tipoMotivoAtendimentos.Count > 0)
                result = result.Where(obj => tipoMotivoAtendimentos.Contains(obj.MotivoChamado.TipoMotivoAtendimento));

            return result.OrderByDescending(obj => obj.Codigo)
                .Fetch(obj => obj.MotivoChamado)
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Filial)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.LoteChamadoOcorrencia.Codigo == codigoLote select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosPendentesDeRetorno(DateTime horaBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where
                         (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa)
                         && !obj.Notificado
                         && obj.DataRetorno.HasValue
                         && obj.DataRetorno.Value > horaBase.AddMinutes(15)
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Chamados.Chamado BuscarPorNumero(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>> BuscarChamadosPorGestaodevolucaoAsync(List<long> codigosDevolucao)
        {
            string sql = @$"
                        select 
                            Chamado.CHA_CODIGO Codigo,
                            Chamado.CHA_NUMERO NumeroChamado,
                            Chamado.CHA_DATA_CRICAO DataCriacao,
                            Chamado.CHA_SITUACAO Situacao,
                            MotivoChamado.MCH_DESCRICAO MotivoChamadoDescricao,
	                        NFe.NFX_CODIGO CodigoNotaFiscal,
	                        NFe.NF_NUMERO NumeroNotaFiscal
                        from T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL GestaoDevolucaoNotaFiscal
                            join T_CHAMADO_XML_NOTA_FISCAL ChamadoNotaFiscal on ChamadoNotaFiscal.NFX_CODIGO = GestaoDevolucaoNotaFiscal.NFX_CODIGO
                            join T_CHAMADOS Chamado on Chamado.CHA_CODIGO = ChamadoNotaFiscal.CHA_CODIGO
                            join T_MOTIVO_CHAMADA MotivoChamado on MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO
                            join T_XML_NOTA_FISCAL NFe on NFe.NFX_CODIGO = ChamadoNotaFiscal.NFX_CODIGO
                        where GDV_CODIGO in ({string.Join(", ", codigosDevolucao)})
                        order by Chamado.CHA_NUMERO, NFe.NF_NUMERO;"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado)));

            return query.SetTimeout(6000).ListAsync<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>(CancellationToken);
        }

        public int TotalChamadosAberto()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa select obj;
            return result.Count();
        }

        public int TotalEncerradosNoPeriodo(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where
                         (obj.Situacao == SituacaoChamado.Finalizado || obj.Situacao == SituacaoChamado.LiberadaOcorrencia || obj.Situacao == SituacaoChamado.RecusadoPeloCliente)
                         select obj;

            if (dataInicio.HasValue)
                result = result.Where(o => o.DataFinalizacao.Value.Date >= dataInicio.Value);

            if (dataFim.HasValue)
                result = result.Where(o => o.DataFinalizacao.Value.Date <= dataFim.Value);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarUltimosChamados(DateTime horaBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where
                         (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa)
                         && obj.DataCriacao < horaBase
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosSemRetorno(DateTime horaBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where
                         (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa)
                         && obj.DataRetorno.HasValue
                         && obj.DataRetorno.Value <= horaBase
                         select obj;
            return result.ToList();
        }

        public void AjustarSequenciaSeNecessario()
        {
            var queryMax = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            int maxNumeroColuna = queryMax.Any() ? queryMax.Max(c => c.Numero) : 0;

            var query = this.SessionNHiBernate.CreateSQLQuery("SELECT current_value AS Valor FROM sys.sequences WHERE name = 'SEQ_NUMERO_CHAMADO'")
                                              .AddScalar("Valor", NHibernateUtil.Int64);

            long? valorAtualSequencia = query.UniqueResult<long?>();

            if (maxNumeroColuna > (long)valorAtualSequencia)
            {
                var queryAlterRestartSequence = this.SessionNHiBernate.CreateSQLQuery($"ALTER SEQUENCE SEQ_NUMERO_CHAMADO RESTART WITH {maxNumeroColuna + 1}");
                queryAlterRestartSequence.ExecuteUpdate();
            }

        }

        public long BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery("SELECT NEXT VALUE FOR SEQ_NUMERO_CHAMADO");
            return query.UniqueResult<long>();
        }

        public Task<long> BuscarProximoNumeroAsync()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery("SELECT NEXT VALUE FOR SEQ_NUMERO_CHAMADO");

            return query.UniqueResultAsync<long>(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> consultaChamado = Consultar(filtrosPesquisa);

            List<int> codigoChamadoPaginado = consultaChamado
                .Select(o => o.Codigo)
                .Skip(parametrosConsulta.InicioRegistros)
                .Take(parametrosConsulta.LimiteRegistros)
                .ToList();

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> result = new();
            int limite = 500;

            int start = 0;
            while (start < codigoChamadoPaginado.Count)
            {
                List<int> batch = codigoChamadoPaginado.Skip(start).Take(limite).ToList();

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> consultaLote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                    .Where(o => batch.Contains(o.Codigo))
                    .Fetch(o => o.Carga).ThenFetch(o => o.Empresa).ThenFetch(o => o.Localidade)
                    .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                    .Fetch(o => o.Cliente).ThenFetch(cliente => cliente.MesoRegiao)
                    .Fetch(o => o.Cliente).ThenFetch(cliente => cliente.Regiao)
                    .Fetch(o => o.Tomador)
                    .Fetch(o => o.Destinatario)
                    .Fetch(o => o.Responsavel)
                    .Fetch(o => o.Pedido)
                    .Fetch(o => o.MotivoChamado)
                    .ToList();

                result.AddRange(consultaLote);

                start += limite;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
            {
                string prop = parametrosConsulta.PropriedadeOrdenar;
                bool asc = parametrosConsulta.DirecaoOrdenar == "asc";

                result = asc
                    ? result.OrderBy(obj => GetNestedPropertyValue(obj, prop)).ToList()
                    : result.OrderByDescending(obj => GetNestedPropertyValue(obj, prop)).ToList();
            }

            return result;
        }


        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa)
        {
            var consultaChamado = Consultar(filtrosPesquisa);

            return consultaChamado.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo> RelacaoChamadosPorMotivo(DateTime dataInicio, DateTime dataFim)
        {
            string pattern = "yyyy-MM-dd";

            string sql = $@"SELECT T.Motivo, SUM(T.Valor) Valor FROM (
                            SELECT 
	                            Motivo.MCH_DESCRICAO Motivo,
	                            (SELECT ISNULL(SUM(ocorrencia.COC_VALOR_OCORRENCIA), 0.0)
                                   FROM T_CARGA_OCORRENCIA ocorrencia
                                   JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.COC_CODIGO = ocorrencia.COC_CODIGO
                                   WHERE chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO) Valor
                            FROM T_CHAMADOS Chamado
                            JOIN T_MOTIVO_CHAMADA Motivo ON Motivo.MCH_CODIGO = Chamado.MCH_CODIGO
                            WHERE CAST(Chamado.CHA_DATA_CRICAO AS DATE) BETWEEN '{dataInicio.ToString(pattern)}' AND '{dataFim.ToString(pattern)}'
                            AND EXISTS (SELECT chamadoOcorrencia.CHA_CODIGO FROM T_CHAMADO_OCORRENCIA chamadoOcorrencia where chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO)
                            ) AS T
                            GROUP BY T.Motivo
                            ORDER BY T.Motivo";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorMotivo>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorUsuarios> RelacaoChamadosPorUsuario(DateTime dataInicio, DateTime dataFim)
        {
            string pattern = "yyyy-MM-dd";

            string sql = @" SELECT 
	                            Funcionario.FUN_CODIGO Codigo,
	                            Funcionario.FUN_NOME Responsavel,
	                            (
		                            (SELECT COUNT(_chamado.CHA_CODIGO)
		                            FROM T_CHAMADOS _chamado 
		                            WHERE _chamado.FUN_RESPONSAVEL = Chamado.FUN_RESPONSAVEL AND _chamado.CHA_SITUACAO = 1
                                    AND _chamado.CHA_DATA_CRICAO BETWEEN '" + dataInicio.ToString(pattern) + @" 00:00:00' AND '" + dataFim.ToString(pattern) + @" 23:59:59')
	                            ) Abertos,
	                            (
		                            (SELECT COUNT(_chamado.CHA_CODIGO)
		                            FROM T_CHAMADOS _chamado 
		                            WHERE _chamado.FUN_RESPONSAVEL = Chamado.FUN_RESPONSAVEL AND (_chamado.CHA_SITUACAO = 2 OR _chamado.CHA_SITUACAO = 4)
                                    AND _chamado.CHA_DATA_CRICAO BETWEEN '" + dataInicio.ToString(pattern) + @" 00:00:00' AND '" + dataFim.ToString(pattern) + @" 23:59:59')
	                            ) Finalizados														
                            FROM 
	                            T_CHAMADOS Chamado

                            JOIN T_FUNCIONARIO Funcionario ON Funcionario.FUN_CODIGO = Chamado.FUN_RESPONSAVEL

                            WHERE 
	                            Chamado.CHA_DATA_CRICAO BETWEEN '" + dataInicio.ToString(pattern) + @" 00:00:00' AND '" + dataFim.ToString(pattern) + @" 23:59:59'

                            GROUP BY
	                            Funcionario.FUN_CODIGO,
	                            Funcionario.FUN_NOME,
	                            Chamado.FUN_RESPONSAVEL";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorUsuarios)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadosPorUsuarios>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.RelacaoOcorrencia> RelacaoPorValorOcorrencia(DateTime dataInicio, DateTime dataFim)
        {
            string pattern = "yyyy-MM-dd";

            string sql = $@" SELECT 
	                            TipoOcorrencia.OCO_DESCRICAO Motivo,

	                            (
		                            ISNULL((SELECT SUM(ocorrencia.COC_VALOR_OCORRENCIA)
		                            FROM T_CARGA_OCORRENCIA ocorrencia
                                    JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia ON chamadoOcorrencia.COC_CODIGO = ocorrencia.COC_CODIGO
                                    JOIN T_CHAMADOS chamado ON chamado.CHA_CODIGO = chamadoOcorrencia.CHA_CODIGO
		                            WHERE 
			                            ocorrencia.COC_SITUACAO_OCORRENCIA = 2
			                            AND ocorrencia.OCO_CODIGO = TipoOcorrencia.OCO_CODIGO
                                        AND CAST(chamado.CHA_DATA_CRICAO AS DATE) BETWEEN '{dataInicio.ToString(pattern)}' AND '{dataFim.ToString(pattern)}'
		                            GROUP BY ocorrencia.OCO_CODIGO), 0.0)
	                            ) Aberto,
	                            (
		                            ISNULL((SELECT SUM(ocorrencia.COC_VALOR_OCORRENCIA)
		                            FROM T_CARGA_OCORRENCIA ocorrencia
                                    JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia ON chamadoOcorrencia.COC_CODIGO = ocorrencia.COC_CODIGO
                                    JOIN T_CHAMADOS chamado ON chamado.CHA_CODIGO = chamadoOcorrencia.CHA_CODIGO
		                            WHERE 
			                            ocorrencia.COC_SITUACAO_OCORRENCIA = 15
			                            AND ocorrencia.OCO_CODIGO = TipoOcorrencia.OCO_CODIGO
                                        AND CAST(chamado.CHA_DATA_CRICAO AS DATE) BETWEEN '{dataInicio.ToString(pattern)}' AND '{dataFim.ToString(pattern)}'
		                            GROUP BY ocorrencia.OCO_CODIGO), 0.0)
	                            ) Finalizado

                            FROM T_OCORRENCIA TipoOcorrencia
                            JOIN T_CARGA_OCORRENCIA Ocorrencia ON Ocorrencia.OCO_CODIGO = TipoOcorrencia.OCO_CODIGO
                            WHERE EXISTS (SELECT chamadoOcorrencia.COC_CODIGO FROM T_CHAMADO_OCORRENCIA chamadoOcorrencia where chamadoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO)
                            GROUP BY
	                            TipoOcorrencia.OCO_CODIGO,
	                            TipoOcorrencia.OCO_DESCRICAO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.RelacaoOcorrencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.RelacaoOcorrencia>();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> ChamadosMesmaCargaMotivoCliente(int codigoCarga, int codigoMotivoChamado, double cliente, int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Codigo != codigoChamado
                                     && (o.Situacao != SituacaoChamado.Cancelada && o.Situacao != SituacaoChamado.FalhaIntegracao));

            if (codigoMotivoChamado > 0)
                query = query.Where(o => o.MotivoChamado.Codigo == codigoMotivoChamado);

            if (cliente > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cliente);

            return query.ToList();
        }

        public bool ContemChamadoMesmaCargaMotivoClienteValor(int codigoCarga, int codigoMotivoChamado, decimal valor, double cliente, int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Valor == valor && o.Codigo != codigoChamado);

            if (codigoMotivoChamado > 0)
                query = query.Where(o => o.MotivoChamado.Codigo == codigoMotivoChamado);

            if (cliente > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cliente);

            return query.Any();
        }

        public bool ContemChamadoMesmoMotorista(int codigoMotivoChamado, double motorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            query = query.Where(
                o => o.Motorista.Codigo == motorista &&
                o.MotivoChamado.PagamentoMotoristaTipo.PermitirMultiplosPagamentosAbertosParaMesmoMotorista == true &&
                o.MotivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista == true &&
                o.Situacao == SituacaoChamado.Aberto
            );

            return query.Any();
        }

        public bool ContemChamadoMesmaCargaEntregaMotivoSituacao(int codigoCarga, int codigoEntrega, int codigoMotivo, SituacaoChamado situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.MotivoChamado.Codigo == codigoMotivo && o.Situacao == situacao);

            if (codigoEntrega > 0)
                query = query.Where(o => o.CargaEntrega.Codigo == codigoEntrega);

            return query.Any();
        }

        public bool ContemChamadoDoTipoReentrega(int codigoCarga, int codigoEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaEntrega.Codigo == codigoEntrega && o.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega);

            return query.Any();
        }

        public Task<bool> ContemChamadoDoTipoReentregaAsync(int codigoCarga, int codigoEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaEntrega.Codigo == codigoEntrega && o.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega);

            return query.AnyAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosEmAbertoPorRegraAnalise(int codigoRegraAnalise)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query
                         where
                         (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa)
                         && obj.RegrasAnalise.Any(reg => reg.Codigo == codigoRegraAnalise)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarAtendimentosPendenteDeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(o => o.AguardandoIntegracao);

            return query.ToList();
        }

        public bool PossuiChamadosAbertosPorEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntrega && (obj.Situacao == SituacaoChamado.Aberto || obj.Situacao == SituacaoChamado.EmTratativa) select obj;
            return result.Any();
        }
        public bool ExistePorCargaMotivoChamadoCliente(int codigoCarga, int codigoMotivoChamado, double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                       obj.MotivoChamado.Codigo == codigoMotivoChamado &&
                                       obj.Cliente.CPF_CNPJ == codigoCliente);
            return query.Select(obj => obj.Codigo).Any();
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento> ObterModeloDadosAtendimentos(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroChamado)
        {
            int limiteRegistros = 1000;
            IQueryable<Dominio.Entidades.Embarcador.Chamados.Chamado> consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            if (dataInicioCriacao != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(chamado => chamado.DataCriacao >= dataInicioCriacao);

            if (dataFimCriacao != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(chamado => chamado.DataCriacao <= dataFimCriacao);

            if (numeroChamado > 0)
                consultaChamado = consultaChamado.Where(chamado => chamado.Numero == numeroChamado);

            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento> chamados = consultaChamado
                .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                .Select(chamado => new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento()
                {
                    Protocolo = chamado.Codigo,
                    Numero = chamado.Numero,
                    DataCriacao = chamado.DataCriacao,
                    DataFinalizacao = (chamado.Situacao == SituacaoChamado.Finalizado) ? chamado.DataFinalizacao : null,
                    Situacao = chamado.Situacao.ObterDescricao(),
                    Observacao = chamado.Observacao,
                    ProtocoloCarga = (chamado.Carga == null) ? 0 : chamado.Carga.Protocolo,
                    CodigoEntrega = (chamado.CargaEntrega == null) ? 0 : chamado.CargaEntrega.Codigo,
                    DataEstorno = chamado.Estornado ? chamado.DataEstorno : null,
                    UsuarioEstorno = (!chamado.Estornado || (chamado.UsuarioEstorno == null)) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Usuario()
                    {
                        CpfCnpj = chamado.UsuarioEstorno.CPF,
                        Nome = chamado.UsuarioEstorno.Nome,
                    },
                    Motivo = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.MotivoAtendimento()
                    {
                        CodigoIntegracao = chamado.MotivoChamado.CodigoIntegracao,
                        Descricao = chamado.MotivoChamado.Descricao
                    },
                    Cliente = (chamado.Cliente == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Cliente()
                    {
                        CpfCnpj = chamado.Cliente.Tipo == "J" ? String.Format(@"{0:00000000000000}", chamado.Cliente.CPF_CNPJ) : String.Format(@"{0:00000000000}", chamado.Cliente.CPF_CNPJ),
                        Nome = chamado.Cliente.Nome,
                        IE = chamado.Cliente.IE_RG,
                        GrupoPessoas = (chamado.Cliente.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.GrupoPessoas()
                        {
                            CodigoIntegracao = chamado.Cliente.GrupoPessoas.CodigoIntegracao,
                            Descricao = chamado.Cliente.GrupoPessoas.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Endereco()
                        {
                            Logradouro = chamado.Cliente.Endereco,
                            Bairro = chamado.Cliente.Bairro,
                            Numero = chamado.Cliente.Numero,
                            Latitude = chamado.Cliente.Latitude,
                            Longitude = chamado.Cliente.Longitude,
                            Localidade = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                            {
                                Descricao = chamado.Cliente.Localidade.Descricao,
                                CodigoIbge = chamado.Cliente.Localidade.CodigoIBGE,
                                Regiao = (chamado.Cliente.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                                {
                                    Descricao = chamado.Cliente.Localidade.Regiao.Descricao,
                                    CodigoIntegracao = chamado.Cliente.Localidade.Regiao.CodigoIntegracao
                                },
                                Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                                {
                                    Descricao = chamado.Cliente.Localidade.Estado.Nome,
                                    Sigla = chamado.Cliente.Localidade.Estado.Sigla,
                                }
                            }
                        }
                    },
                    Responsavel = (chamado.Responsavel == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Usuario()
                    {
                        CpfCnpj = chamado.Responsavel.CPF,
                        Nome = chamado.Responsavel.Nome,
                    },
                    SetorResponsavel = (chamado.SetorResponsavel == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Setor()
                    {
                        Descricao = chamado.SetorResponsavel.Descricao
                    },
                    Devolucao = ((chamado.MotivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.Devolucao) && (chamado.MotivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.ReentregarMesmaCarga)) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.AtendimentoDevolucao()
                    {
                        Tratativa = chamado.TratativaDevolucao.ObterDescricao(),
                        Tipo = ((chamado.CargaEntrega != null) && chamado.CargaEntrega.DevolucaoParcial) ? TipoColetaEntregaDevolucao.Parcial.ObterDescricao() : TipoColetaEntregaDevolucao.Total.ObterDescricao()
                    }
                })
                .ToList();

            if (chamados.Count > 0)
            {
                List<(int ProtocoloChamado, int ProtocoloOcorrencia)> chamadoOcorrencias = new List<(int ProtocoloChamado, int ProtocoloOcorrencia)>();
                List<(int ProtocoloChamado, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ChamadoAnalise Analise)> chamadoAnalises = new List<(int ProtocoloChamado, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ChamadoAnalise Analise)>();
                List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaNotaFiscal EntregaNotaFiscal)> chamadoDevolucaoNotasFiscais = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaNotaFiscal EntregaNotaFiscal)>();

                for (int registroInicial = 0; registroInicial < chamados.Count; registroInicial += limiteRegistros)
                {
                    List<int> protocolosChamados = chamados.Select(chamado => chamado.Protocolo).Skip(registroInicial).Take(limiteRegistros).ToList();
                    List<int> codigosEntregas = chamados.Select(chamado => chamado.CodigoEntrega).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                        .Where(chamadoOcorrencia => protocolosChamados.Contains(chamadoOcorrencia.Chamado.Codigo));

                    chamadoOcorrencias.AddRange(consultaChamadoOcorrencia
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(chamadoOcorrencia => ValueTuple.Create(
                            chamadoOcorrencia.Chamado.Codigo,
                            chamadoOcorrencia.CargaOcorrencia.Codigo
                        ))
                        .ToList()
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> consultaChamadoAnalise = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>()
                        .Where(analise => protocolosChamados.Contains(analise.Chamado.Codigo));

                    chamadoAnalises.AddRange(consultaChamadoAnalise
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(analise => ValueTuple.Create(
                            analise.Chamado.Codigo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ChamadoAnalise()
                            {
                                Autor = (analise.Autor == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Usuario()
                                {
                                    CpfCnpj = analise.Autor.CPF,
                                    Nome = analise.Autor.Nome,
                                },
                                DataCriacao = analise.DataCriacao,
                                DataRetorno = analise.DataRetorno,
                                Observacao = analise.Observacao
                            }
                        ))
                        .ToList()
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> consultaCargaEntregaNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                        .Where(entregaNotaFiscal => codigosEntregas.Contains(entregaNotaFiscal.CargaEntrega.Codigo));

                    chamadoDevolucaoNotasFiscais.AddRange(consultaCargaEntregaNotaFiscal
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(entregaNotaFiscal => ValueTuple.Create(
                            entregaNotaFiscal.CargaEntrega.Codigo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaNotaFiscal()
                            {
                                Chave = entregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                                Valor = entregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor,
                                SituacaoEntrega = (entregaNotaFiscal.SituacaoEntregaNotaFiscalChamado ?? entregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal).ObterDescricao()
                            }
                        ))
                        .ToList()
                    );
                }

                foreach (Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento chamado in chamados)
                {
                    chamado.ProtocolosOcorrencias = chamadoOcorrencias
                        .Where(chamadoOcorrencia => chamadoOcorrencia.ProtocoloChamado == chamado.Protocolo)
                        .Select(chamadoOcorrencia => chamadoOcorrencia.ProtocoloOcorrencia)
                        .ToList();

                    chamado.Analises = chamadoAnalises
                        .Where(chamadoAnalise => chamadoAnalise.ProtocoloChamado == chamado.Protocolo)
                        .Select(chamadoAnalise => chamadoAnalise.Analise)
                        .ToList();

                    if (chamado.Devolucao != null)
                        chamado.Devolucao.NotasFiscais = chamadoDevolucaoNotasFiscais
                            .Where(entregaNotaFiscal => entregaNotaFiscal.CodigoEntrega == chamado.CodigoEntrega)
                            .Select(entregaNotaFiscal => entregaNotaFiscal.EntregaNotaFiscal)
                            .ToList();
                }
            }

            return chamados;
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadoAtendimentosEmAbertoPorNotasFiscais(List<int> numerosNfs)
        {
            var consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                .Where(x => x.Situacao != SituacaoChamado.Finalizado && x.Situacao != SituacaoChamado.Cancelada
                && x.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Atendimento);

            consultaChamado = consultaChamado
                .Where(o =>
              (o.CargaEntrega == null && o.XMLNotasFiscais.Any(x => numerosNfs.Contains(x.Numero)))
              ||
              (o.CargaEntrega != null && o.CargaEntrega.NotasFiscais.Any(nf => numerosNfs.Contains(nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero))));

            return consultaChamado.ToList();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> BuscarChamadosPorCodigoNotaFiscaECarga(int codigoCarga, int CodigoNotaFiscal)
        {
            string sql = @$" SELECT Chamado.CHA_CODIGO Codigo,
                                    Chamado.CHA_NUMERO NumeroChamado,
                                    Chamado.CHA_DATA_CRICAO DataCriacao,
                                    Chamado.CHA_SITUACAO Situacao,
                                    MotivoChamado.MCH_DESCRICAO MotivoChamadoDescricao,
                                    NFe.NFX_CODIGO CodigoNotaFiscal,
                                    NFe.NF_NUMERO NumeroNotaFiscal
                               FROM T_CHAMADO_XML_NOTA_FISCAL ChamadoNotaFiscal 
                               JOIN T_CHAMADOS Chamado ON Chamado.CHA_CODIGO = ChamadoNotaFiscal.CHA_CODIGO
                               JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO
                               JOIN T_XML_NOTA_FISCAL NFe ON NFe.NFX_CODIGO = ChamadoNotaFiscal.NFX_CODIGO
                              WHERE ChamadoNotaFiscal.NFX_CODIGO = {CodigoNotaFiscal} AND Chamado.CAR_CODIGO = {codigoCarga}"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> BuscarChamadosPorCodigosNotasFiscais(List<int> codigosNotasFiscais)
        {
            string sql = @$" SELECT Chamado.CHA_CODIGO Codigo,
                                    Chamado.CHA_NUMERO NumeroChamado,
                                    Chamado.CHA_DATA_CRICAO DataCriacao,
                                    Chamado.CHA_SITUACAO Situacao,
                                    MotivoChamado.MCH_DESCRICAO MotivoChamadoDescricao,
                                    NFe.NFX_CODIGO CodigoNotaFiscal,
                                    NFe.NF_NUMERO NumeroNotaFiscal
                               FROM T_CHAMADO_XML_NOTA_FISCAL ChamadoNotaFiscal 
                               JOIN T_CHAMADOS Chamado ON Chamado.CHA_CODIGO = ChamadoNotaFiscal.CHA_CODIGO
                               JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO
                               JOIN T_XML_NOTA_FISCAL NFe ON NFe.NFX_CODIGO = ChamadoNotaFiscal.NFX_CODIGO
                              WHERE ChamadoNotaFiscal.NFX_CODIGO in ({string.Join(", ", codigosNotasFiscais)})"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>();
        }


        #endregion

        #region Relatório Chamados

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadoOcorrencia> ConsultarChamadoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaChamadoOcorrencia(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadoOcorrencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadoOcorrencia>();
        }

        public int ContarConsultaChamadoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaChamadoOcorrencia(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaChamadoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            bool existeNFAtendimento = propriedades.Exists(c => c.Propriedade == "NFAtendimento");

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaChamadoOcorrencia(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count, existeNFAtendimento);

            SetarWhereRelatorioConsultaChamadoOcorrencia(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaChamadoOcorrencia(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_CHAMADOS Chamado ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaChamadoOcorrencia(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count, bool existeCampoNFAtendimento = false)
        {
            SetarJoinPadraoParaCamposDevolucao(propriedade, ref joins, ref groupBy);

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Chamado.CHA_CODIGO Codigo, ";
                        groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;
                case "ValorChamado":
                    if (!select.Contains(" ValorChamado, "))
                    {
                        select += "Chamado.CHA_VALOR ValorChamado, ";
                        groupBy += "Chamado.CHA_VALOR, ";
                    }
                    break;
                case "DescricaoAosCuidadosDo":
                    if (!select.Contains(" AosCuidadosDo, "))
                    {
                        select += "Chamado.CHA_AOS_CUIDADOS_DO AosCuidadosDo, ";
                        groupBy += "Chamado.CHA_AOS_CUIDADOS_DO, ";
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" DadosSumarizados "))
                            joins += " LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON Carga.CDS_CODIGO = DadosSumarizados.CDS_CODIGO";

                        select += "DadosSumarizados.CDS_ORIGENS Origem, ";
                        groupBy += "DadosSumarizados.CDS_ORIGENS, ";
                    }
                    break;
                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" DadosSumarizados "))
                            joins += " LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON Carga.CDS_CODIGO = DadosSumarizados.CDS_CODIGO";

                        select += "DadosSumarizados.CDS_DESTINOS Destino, ";
                        groupBy += "DadosSumarizados.CDS_DESTINOS, ";
                    }
                    break;
                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += "Cliente.CLI_NOME Cliente, ";
                        groupBy += "Cliente.CLI_NOME, ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                        select += "Destinatario.CLI_NOME Destinatario, ";
                        groupBy += "Destinatario.CLI_NOME, ";
                    }
                    break;
                case "CNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                        select += "Destinatario.CLI_FISJUR TipoDestinatario, Destinatario.CLI_CGCCPF CPFCNPJDestinatario, ";
                        groupBy += "Destinatario.CLI_FISJUR, Destinatario.CLI_CGCCPF, ";
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        if (!joins.Contains(" Tomador "))
                            joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = Chamado.CLI_CGCCPF_TOMADOR";

                        select += "Tomador.CLI_NOME Tomador, ";
                        groupBy += "Tomador.CLI_NOME, ";
                    }
                    break;
                case "DataCriacaoFormatado":
                case "TempoTotal":
                    if (!select.Contains(" DataCriacao, "))
                    {
                        select += "Chamado.CHA_DATA_CRICAO DataCriacao, ";
                        groupBy += "Chamado.CHA_DATA_CRICAO, ";
                    }
                    break;
                case "DataRetornoFormatado":
                    if (!select.Contains(" DataRetorno, "))
                    {
                        if (!joins.Contains(" ChamadoAnalise "))
                            joins += " LEFT JOIN T_CHAMADO_ANALISES ChamadoAnalise ON ChamadoAnalise.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += "ChamadoAnalise.ANC_DATA_RETORNO DataRetorno, ";

                        if (!groupBy.Contains("ChamadoAnalise.ANC_DATA_RETORNO,"))
                            groupBy += "ChamadoAnalise.ANC_DATA_RETORNO, ";
                    }
                    break;
                case "MotivoChamado":
                    if (!select.Contains(" MotivoChamado, "))
                    {
                        if (!joins.Contains(" MotivoChamado "))
                            joins += " LEFT JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO";

                        select += "MotivoChamado.MCH_DESCRICAO MotivoChamado, ";
                        groupBy += "MotivoChamado.MCH_DESCRICAO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "Chamado.CHA_NUMERO Numero, ";
                        groupBy += "Chamado.CHA_NUMERO, ";
                    }
                    break;
                case "NumeroPallet":
                    if (!select.Contains(" NumeroPallet, "))
                    {
                        select += "Chamado.CHA_NUMERO_PALLET NumeroPallet, ";
                        groupBy += "Chamado.CHA_NUMERO_PALLET, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Chamado.CHA_OBSERVACAO Observacao, ";
                        groupBy += "Chamado.CHA_OBSERVACAO, ";
                    }
                    break;
                case "DescricaoResponsavelChamado":
                    if (!select.Contains(" ResponsavelChamado, "))
                    {
                        select += "Chamado.CHA_RESPONSAVEL_CHAMADO ResponsavelChamado, ";
                        groupBy += "Chamado.CHA_RESPONSAVEL_CHAMADO, ";
                    }
                    break;
                case "Representante":
                    if (!select.Contains(" Representante, "))
                    {
                        if (!joins.Contains(" Representante "))
                            joins += " LEFT JOIN T_REPRESENTANTE Representante ON Representante.REP_CODIGO = Chamado.REP_CODIGO";

                        select += "Representante.REP_DESCRICAO Representante, ";
                        groupBy += "Representante.REP_DESCRICAO, ";
                    }
                    break;
                case "DescricaoResponsavelOcorrencia":
                    if (!select.Contains(" ResponsavelOcorrencia, "))
                    {
                        select += "Chamado.CHA_RESPONSAVEL_OCORRENCIA ResponsavelOcorrencia, ";
                        groupBy += "Chamado.CHA_RESPONSAVEL_OCORRENCIA, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += "Chamado.CHA_SITUACAO Situacao, ";
                        groupBy += "Chamado.CHA_SITUACAO, ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        if (!joins.Contains(" Transportador "))
                            joins += " LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Chamado.EMP_CODIGO";

                        select += "Transportador.EMP_RAZAO Transportador, ";
                        groupBy += "Transportador.EMP_RAZAO, ";
                    }
                    break;
                case "DescricaoVeiculoCarregado":
                    if (!select.Contains(" VeiculoCarregado, "))
                    {
                        select += "Chamado.CHA_VEICULO_CARREGADO VeiculoCarregado, ";
                        groupBy += "Chamado.CHA_VEICULO_CARREGADO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " LEFT JOIN T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados ON CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO";

                        select += "CargaDadosSumarizados.CDS_VEICULOS Placa, ";
                        groupBy += "CargaDadosSumarizados.CDS_VEICULOS, ";
                    }
                    break;
                case "DataCarregamentoFormatado":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += "Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ";
                        groupBy += "Carga.CAR_DATA_CARREGAMENTO, ";
                    }
                    break;
                case "DataFinalizacaoFormatado":
                    if (!select.Contains(" DataFinalizacao, "))
                    {
                        select += "Chamado.CHA_DATA_FINALIZACAO DataFinalizacao, ";

                        if (!groupBy.Contains("Chamado.CHA_DATA_FINALIZACAO,"))
                            groupBy += "Chamado.CHA_DATA_FINALIZACAO, ";
                    }
                    break;
                case "DataEntregaFormatado":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += "Carga.CAR_DATA_TERMINO_CARGA DataEntrega, ";
                        groupBy += "Carga.CAR_DATA_TERMINO_CARGA, ";
                    }
                    break;
                case "StatusAtrasoCarga":
                    if (!select.Contains(" DiasAtraso, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" CargaJanela "))
                            joins += " LEFT JOIN T_CARGA_JANELA_CARREGAMENTO CargaJanela ON CargaJanela.CAR_CODIGO = Carga.CAR_CODIGO";

                        select += "DATEDIFF((DAY), CargaJanela.CJC_DATA_CARREGAMENTO_PROGRAMADA, CargaJanela.CJC_INICIO_CARREGAMENTO) DiasAtraso, ";
                        groupBy += "Carga.CAR_CODIGO, CargaJanela.CJC_DATA_CARREGAMENTO_PROGRAMADA, CargaJanela.CJC_INICIO_CARREGAMENTO, ";
                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                    }
                    break;
                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' +  CAST(ocorrencia.COC_NUMERO_CONTRATO AS NVARCHAR(2000))
                                        FROM T_CARGA_OCORRENCIA ocorrencia
				                        JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.COC_CODIGO = ocorrencia.COC_CODIGO
				                        WHERE chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO
                                        FOR XML PATH('')), 3, 2000
                                    ) NumeroOcorrencia, ";

                        groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;
                case "MotivoOcorrencia":
                    if (!select.Contains(" MotivoOcorrencia, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' +  CAST(tipoOcorrencia.OCO_DESCRICAO AS NVARCHAR(2000))
                                        FROM T_OCORRENCIA tipoOcorrencia
                                        JOIN T_CARGA_OCORRENCIA ocorrencia ON ocorrencia.OCO_CODIGO = tipoOcorrencia.OCO_CODIGO
                                        JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.COC_CODIGO = ocorrencia.COC_CODIGO
					                    WHERE chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO
                                        FOR XML PATH('')), 3, 2000
                                    ) MotivoOcorrencia, ";

                        groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;

                case "ValorDevItem": // // Devolução de ITENS de uma nota

                    if (!select.Contains("subValorDevItem.ValorDevItem"))
                        select += @"subValorDevItem.ValorDevItem, ";

                    if (!joins.Contains("subValorDevItem"))
                    {
                        joins += @" OUTER APPLY (SELECT SUM(cap.CPP_VALOR_DEVOLUCAO) AS ValorDevItem
                                                 FROM T_CARGA_ENTREGA cae
			                                     JOIN T_CARGA_ENTREGA_PRODUTO cap ON cae.CEN_CODIGO = cap.CEN_CODIGO
			                                     WHERE cae.CAR_CODIGO = Chamado.CAR_CODIGO 
                                                   AND cap.CPP_QUANTIDADE_DEVOLUCAO > 0 
                                                   AND cap.CPP_NF_DEVOLUCAO > 0
                                                   AND Chamado.CEN_CODIGO is not null
                                    ) subValorDevItem ";
                    }

                    if (!groupBy.Contains("subValorDevItem.ValorDevItem,"))
                        groupBy += "subValorDevItem.ValorDevItem, ";

                    break;

                case "ValorDevNota": // // Devolução de uma NOTA completa

                    if (!select.Contains("subValorDevNota.ValorDevNota"))
                        select += @"subValorDevNota.ValorDevNota, ";

                    if (!joins.Contains("subValorDevNota"))
                    {
                        joins += @" OUTER APPLY (SELECT SUM(DISTINCT xnf.NF_VALOR) AS ValorDevNota 
			                                     FROM T_CARGA_ENTREGA_NOTA_FISCAL cef
			                                     JOIN  T_CARGA_ENTREGA cen ON cef.CEN_CODIGO = cen.CEN_CODIGO
			                                     JOIN  T_CARGA_CTE cct ON cen.CAR_CODIGO = cct.CAR_CODIGO
			                                     JOIN  T_CTE con ON cct.CON_CODIGO = con.CON_CODIGO
			                                     JOIN  T_CTE_XML_NOTAS_FISCAIS cxm ON con.CON_CODIGO = cxm.CON_CODIGO
			                                     JOIN T_XML_NOTA_FISCAL xnf ON cxm.NFX_CODIGO = xnf.NFX_CODIGO
			                                     WHERE cen.CAR_CODIGO = Chamado.CAR_CODIGO
			                                     AND xnf.NF_SITUACAO_ENTREGA = 3
                                                 AND Chamado.CEN_CODIGO is not null
                                    ) subValorDevNota ";
                    }

                    if (!groupBy.Contains("subValorDevNota.ValorDevNota"))
                        groupBy += "subValorDevNota.ValorDevNota, ";

                    break;

                case "ValorDevTotal": // Devolução de uma carga completa

                    if (!select.Contains("subValorDevTotal.ValorDevTotal"))
                        select += @"subValorDevTotal.ValorDevTotal, ";

                    if (!joins.Contains("subValorDevTotal"))
                    {
                        joins += @" OUTER APPLY (
	                                    SELECT 
		                                    CASE
			                                    WHEN 
				                                    COUNT(DISTINCT xnf.NFX_CODIGO) = (SELECT COUNT(DISTINCT xnf2.NFX_CODIGO) FROM T_CARGA_ENTREGA_NOTA_FISCAL cef2
												                                      INNER JOIN T_CARGA_ENTREGA cen2 ON cef2.CEN_CODIGO = cen2.CEN_CODIGO
												                                      INNER JOIN T_CARGA_CTE cct2 ON cen2.CAR_CODIGO = cct2.CAR_CODIGO
												                                      INNER JOIN T_CTE con2 ON cct2.CON_CODIGO = con2.CON_CODIGO
												                                      INNER JOIN T_CTE_XML_NOTAS_FISCAIS cxm2 ON con2.CON_CODIGO = cxm2.CON_CODIGO
												                                      INNER JOIN T_XML_NOTA_FISCAL xnf2 ON cxm2.NFX_CODIGO = xnf2.NFX_CODIGO
												                                      WHERE cen2.CAR_CODIGO = Chamado.CAR_CODIGO AND xnf2.NF_SITUACAO_ENTREGA = 3)
			                                    THEN 
				                                    (SELECT SUM(DISTINCT xnf3.NF_VALOR) FROM T_CARGA_ENTREGA_NOTA_FISCAL cef3
				                                     INNER JOIN T_CARGA_ENTREGA cen3 ON cef3.CEN_CODIGO = cen3.CEN_CODIGO
				                                     INNER JOIN T_CARGA_CTE cct3 ON cen3.CAR_CODIGO = cct3.CAR_CODIGO
				                                     INNER JOIN T_CTE con3 ON cct3.CON_CODIGO = con3.CON_CODIGO
				                                     INNER JOIN T_CTE_XML_NOTAS_FISCAIS cxm3 ON con3.CON_CODIGO = cxm3.CON_CODIGO
				                                     INNER JOIN T_XML_NOTA_FISCAL xnf3 ON cxm3.NFX_CODIGO = xnf3.NFX_CODIGO
				                                     WHERE cen3.CAR_CODIGO = Chamado.CAR_CODIGO AND xnf3.NF_SITUACAO_ENTREGA = 3)
			                                    ELSE NULL
		                                    END AS ValorDevTotal
	                                    FROM T_CARGA_ENTREGA_NOTA_FISCAL cef
	                                    INNER JOIN T_CARGA_ENTREGA cen ON cef.CEN_CODIGO = cen.CEN_CODIGO
	                                    INNER JOIN T_CARGA_CTE cct ON cen.CAR_CODIGO = cct.CAR_CODIGO
	                                    INNER JOIN T_CTE con ON cct.CON_CODIGO = con.CON_CODIGO
	                                    INNER JOIN T_CTE_XML_NOTAS_FISCAIS cxm ON con.CON_CODIGO = cxm.CON_CODIGO
	                                    INNER JOIN T_XML_NOTA_FISCAL xnf ON cxm.NFX_CODIGO = xnf.NFX_CODIGO
	                                    WHERE cen.CAR_CODIGO = Chamado.CAR_CODIGO
                                        AND Chamado.CEN_CODIGO is not null

                                    ) subValorDevTotal  ";
                    }

                    if (!groupBy.Contains("subValorDevTotal.ValorDevTotal"))
                        groupBy += "subValorDevTotal.ValorDevTotal, ";

                    break;

                case "Notas":
                    if (!select.Contains(" Notas, "))
                    {
                        select +=
                            @"ISNULL(
                                (CASE
                                    WHEN Chamado.CEN_CODIGO IS NOT NULL THEN
                                        (SUBSTRING(
                                            (SELECT DISTINCT ', ' + CAST(_xmlnotafiscal.NF_NUMERO AS NVARCHAR(2000))
                                            FROM T_CARGA_PEDIDO _cargaPedido
					                        JOIN T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CEN_CODIGO = Chamado.CEN_CODIGO
                                            JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal ON _pedidoxmlnotafiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                            JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO
                                            WHERE (_cargaEntregaPedido.CPE_CODIGO IS NULL OR _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO) 
					                        FOR XML PATH('')), 3, 2000))
                                    ELSE
                                        (SUBSTRING(
                                            (SELECT DISTINCT ', ' + CAST(_xmlnotafiscal.NF_NUMERO AS NVARCHAR(2000))
                                            FROM T_CARGA_PEDIDO _cargaPedido
                                            JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                            JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal ON _pedidoxmlnotafiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                            JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO
                                            WHERE _cargaPedido.CAR_CODIGO = Chamado.CAR_CODIGO
                                                AND (Chamado.CLI_CGCCPF IS NULL OR _pedido.CLI_CODIGO = Chamado.CLI_CGCCPF)
                                            FOR XML PATH('')), 3, 2000))
                                END),
                                ''
                            ) AS Notas,";

                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                        if (!groupBy.Contains("Chamado.CEN_CODIGO,"))
                            groupBy += "Chamado.CEN_CODIGO, ";
                        if (!groupBy.Contains("Chamado.CLI_CGCCPF,"))
                            groupBy += "Chamado.CLI_CGCCPF, ";
                    }
                    break;
                case "CTes":
                    if (!select.Contains(" CTes, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(_cte.CON_NUM AS NVARCHAR(2000)) + ' - ' + CAST(emp.ESE_NUMERO as NVARCHAR(2000))                                    
                                            FROM T_CARGA_CTE _cargaCTe                                    
                                            LEFT JOIN T_CTE _cte ON _cte.CON_CODIGO = _cargaCTe.CON_CODIGO              
				                            LEFT JOIN T_EMPRESA_SERIE emp on _cte.CON_SERIE = emp.ESE_CODIGO 
                                            WHERE _cargaCTe.CAR_CODIGO = Chamado.CAR_CODIGO FOR XML PATH('')), 3, 2000) CTes, ";

                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + _motorista.FUN_NOME
		                                FROM T_CARGA_MOTORISTA _cargamotorista
		                                LEFT JOIN T_FUNCIONARIO _motorista ON _motorista.FUN_CODIGO = _cargamotorista.CAR_MOTORISTA
		                                WHERE _cargamotorista.CAR_CODIGO = Chamado.CAR_CODIGO FOR XML PATH('')), 3, 2000
	                                ) Motorista, ";

                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                    }
                    break;
                case "EnderecoCliente":
                    if (!select.Contains(" EnderecoCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += @"Cliente.CLI_ENDERECO EnderecoCliente, ";
                        groupBy += "Cliente.CLI_ENDERECO, ";
                    }
                    break;
                case "BairroCliente":
                    if (!select.Contains(" BairroCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += @"Cliente.CLI_BAIRRO BairroCliente, ";
                        groupBy += "Cliente.CLI_BAIRRO, ";
                    }
                    break;
                case "CidadeCliente":
                    if (!select.Contains(" CidadeCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        if (!joins.Contains(" LocalidadeCliente "))
                            joins += " LEFT JOIN T_LOCALIDADES LocalidadeCliente ON LocalidadeCliente.LOC_CODIGO = Cliente.LOC_CODIGO";

                        select += "LocalidadeCliente.LOC_DESCRICAO CidadeCliente, ";
                        groupBy += "LocalidadeCliente.LOC_DESCRICAO, ";
                    }
                    break;
                case "EstadoCliente":
                    if (!select.Contains(" EstadoCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        if (!joins.Contains(" LocalidadeCliente "))
                            joins += " LEFT JOIN T_LOCALIDADES LocalidadeCliente ON LocalidadeCliente.LOC_CODIGO = Cliente.LOC_CODIGO";

                        select += "LocalidadeCliente.UF_SIGLA EstadoCliente, ";
                        groupBy += "LocalidadeCliente.UF_SIGLA, ";
                    }
                    break;
                case "CEPCliente":
                    if (!select.Contains(" CEPCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += @"Cliente.CLI_CEP CEPCliente, ";
                        groupBy += "Cliente.CLI_CEP, ";
                    }
                    break;
                case "LatitudeCliente":
                    if (!select.Contains(" LatitudeCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += @"Cliente.CLI_LATIDUDE LatitudeCliente, ";
                        groupBy += "Cliente.CLI_LATIDUDE, ";
                    }
                    break;
                case "LongitudeCliente":
                    if (!select.Contains(" LongitudeCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += @"Cliente.CLI_LONGITUDE LongitudeCliente, ";
                        groupBy += "Cliente.CLI_LONGITUDE, ";
                    }
                    break;
                case "DataCriacaoCargaFormatado":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += @"Carga.CAR_DATA_CRIACAO DataCriacaoCarga, ";
                        groupBy += "Carga.CAR_DATA_CRIACAO, ";
                    }
                    break;
                case "TipoOperacaoCarga":
                    if (!select.Contains(" TipoOperacaoCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" TipoOperacao "))
                            joins += " LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO";

                        select += @"TipoOperacao.TOP_DESCRICAO TipoOperacaoCarga, ";
                        groupBy += "TipoOperacao.TOP_DESCRICAO, ";
                    }
                    break;
                case "FilialCarga":
                    if (!select.Contains(" FilialCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" Filial "))
                            joins += " LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO";

                        select += "Filial.FIL_DESCRICAO FilialCarga, ";
                        groupBy += "Filial.FIL_DESCRICAO, ";
                    }
                    break;

                case "DataAssumicaoPrimeiroAtendimentoFormatado":
                    if (!select.Contains(" DataAssumicaoPrimeiroAtendimento, "))
                    {
                        select += "Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO DataAssumicaoPrimeiroAtendimento, ";

                        if (!groupBy.Contains("Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO,"))
                            groupBy += "Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO, ";
                    }
                    break;

                case "TempoAtendimentoFormatado":
                    if (!select.Contains(" TempoAtendimento, "))
                    {
                        select += "DATEDIFF(second, Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO, Chamado.CHA_DATA_FINALIZACAO) TempoAtendimento, ";

                        if (!groupBy.Contains("Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO,"))
                            groupBy += "Chamado.CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO, ";

                        if (!groupBy.Contains("Chamado.CHA_DATA_FINALIZACAO,"))
                            groupBy += "Chamado.CHA_DATA_FINALIZACAO, ";
                    }
                    break;

                case "Responsavel":
                    if (!select.Contains(" Responsavel, "))
                    {
                        if (!joins.Contains(" Responsavel "))
                            joins += " LEFT JOIN T_FUNCIONARIO Responsavel ON Responsavel.FUN_CODIGO = Chamado.FUN_RESPONSAVEL";

                        if (!joins.Contains(" Setor "))
                            joins += " LEFT JOIN T_SETOR Setor ON Setor.SET_CODIGO = Chamado.SET_RESPONSAVEL ";

                        select += @"COALESCE(Responsavel.FUN_NOME, Setor.SET_DESCRICAO, '') Responsavel, ";
                        groupBy += "Responsavel.FUN_NOME, ";
                        groupBy += "Setor.SET_DESCRICAO, ";
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " LEFT JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = Chamado.FUN_CODIGO";

                        select += @"Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;

                case "ValorDesconto":
                    if (!select.Contains(" ValorDesconto, "))
                    {
                        select += "Chamado.CHA_VALOR_DESCONTO ValorDesconto, ";
                        groupBy += "Chamado.CHA_VALOR_DESCONTO, ";
                    }
                    break;

                case "TotalDeHoras":
                    if (!select.Contains(" TotalDeHoras, "))
                    {
                        if (!joins.Contains(" ChamadoData "))
                            joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += @"RIGHT('0' + CONVERT(varchar(4),SUM(DATEDIFF(MINUTE, ChamadoData.CDA_DATA_INICIO, ChamadoData.CDA_DATA_FIM))/60),3) + ':' + ";
                        select += @"RIGHT('0' + CONVERT(varchar(2),SUM(DATEDIFF(MINUTE, ChamadoData.CDA_DATA_INICIO, ChamadoData.CDA_DATA_FIM))%60),2) TotalDeHoras, ";
                        groupBy += "ChamadoData.CHA_CODIGO, ";
                    }
                    break;

                case "ObservacaoAnalise":
                    if (!select.Contains(" ObservacaoAnalise, "))
                    {
                        if (!joins.Contains(" ChamadoAnalise "))
                            joins += " LEFT JOIN T_CHAMADO_ANALISES ChamadoAnalise ON ChamadoAnalise.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += @"ChamadoAnalise.ANC_OBSERVACAO ObservacaoAnalise, ";
                        groupBy += "ChamadoAnalise.ANC_OBSERVACAO, ";
                    }
                    break;

                case "FilialVenda":
                    if (!select.Contains(" FilialVenda, "))
                    {
                        if (!select.Contains(" Carga_Pedido, "))
                            joins += " LEFT JOIN T_CARGA_PEDIDO Carga_Pedido ON Carga_Pedido.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!select.Contains(" _Pedido, "))
                            joins += " LEFT JOIN T_PEDIDO _Pedido ON _Pedido.PED_CODIGO = Carga_Pedido.PED_CODIGO";

                        if (!select.Contains(" FilialVenda, "))
                            joins += " LEFT JOIN T_FILIAL FilialVenda ON FilialVenda.FIL_CODIGO = _Pedido.FIL_CODIGO_VENDA";

                        select += "FilialVenda.FIL_DESCRICAO FilialVenda, ";
                        groupBy += "FilialVenda.FIL_DESCRICAO, ";
                    }
                    break;

                case "DataChegadaDiariaFormatada":
                    if (!select.Contains(" DataChegadaDiaria, "))
                    {
                        if (!joins.Contains(" ChamadoData "))
                            joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += "ChamadoData.CDA_DATA_INICIO DataChegadaDiaria, ";
                        groupBy += "ChamadoData.CDA_DATA_INICIO, ";
                    }
                    break;

                case "DataSaidaDiariaFormatada":
                    if (!select.Contains(" DataSaidaDiaria, "))
                    {
                        if (!joins.Contains(" ChamadoData "))
                            joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += "ChamadoData.CDA_DATA_FIM DataSaidaDiaria, ";
                        groupBy += "ChamadoData.CDA_DATA_FIM, ";
                    }
                    break;
                case "ValorDiariaAutomatica":
                    if (!select.Contains(" ValorDiariaAutomatica, "))
                    {
                        if (!joins.Contains(" DiariaAutomatica "))
                            joins += " LEFT JOIN T_DIARIA_AUTOMATICA DiariaAutomatica ON DiariaAutomatica.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += "DiariaAutomatica.DAU_VALOR_DIARIA ValorDiariaAutomatica, ";
                        groupBy += "DiariaAutomatica.DAU_VALOR_DIARIA, ";
                    }
                    break;
                case "TotalDeHorasDiariaAutomatica":
                    if (!select.Contains(" TotalDeHorasDiariaAutomatica, "))
                    {
                        if (!joins.Contains(" DiariaAutomatica "))
                            joins += " LEFT JOIN T_DIARIA_AUTOMATICA DiariaAutomatica ON DiariaAutomatica.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += @"CAST(DiariaAutomatica.DAU_TEMPO_TOTAL / 60 AS VARCHAR(8)) + ':' + FORMAT(DiariaAutomatica.DAU_TEMPO_TOTAL % 60, 'D2') TotalDeHorasDiariaAutomatica, ";
                        groupBy += "CAST(DiariaAutomatica.DAU_TEMPO_TOTAL / 60 AS VARCHAR(8)) + ':' + FORMAT(DiariaAutomatica.DAU_TEMPO_TOTAL % 60, 'D2'), ";
                    }
                    break;

                case "GrupoPessoasCliente":
                    if (!select.Contains(" GrupoPessoasCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        if (!select.Contains(" GrupoPessoasCliente, "))
                            joins += " LEFT JOIN T_GRUPO_PESSOAS GrupoPessoasCliente ON GrupoPessoasCliente.GRP_CODIGO = Cliente.GRP_CODIGO";

                        select += "GrupoPessoasCliente.GRP_DESCRICAO GrupoPessoasCliente, ";
                        groupBy += "GrupoPessoasCliente.GRP_DESCRICAO, ";
                    }
                    break;

                case "GrupoPessoasTomador":
                    if (!select.Contains(" GrupoPessoasTomador, "))
                    {
                        if (!joins.Contains(" Tomador "))
                            joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = Chamado.CLI_CGCCPF_TOMADOR";

                        if (!select.Contains(" GrupoPessoasTomador, "))
                            joins += " LEFT JOIN T_GRUPO_PESSOAS GrupoPessoasTomador ON GrupoPessoasTomador.GRP_CODIGO = Tomador.GRP_CODIGO";

                        select += "GrupoPessoasTomador.GRP_DESCRICAO GrupoPessoasTomador, ";
                        groupBy += "GrupoPessoasTomador.GRP_DESCRICAO, ";
                    }
                    break;

                case "GrupoPessoasDestinatario":
                    if (!select.Contains(" GrupoPessoasDestinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                        if (!select.Contains(" GrupoPessoasDestinatario, "))
                            joins += " LEFT JOIN T_GRUPO_PESSOAS GrupoPessoasDestinatario ON GrupoPessoasDestinatario.GRP_CODIGO = Destinatario.GRP_CODIGO";

                        select += "GrupoPessoasDestinatario.GRP_DESCRICAO GrupoPessoasDestinatario, ";
                        groupBy += "GrupoPessoasDestinatario.GRP_DESCRICAO, ";
                    }
                    break;

                case "TipoMotivoChamado":
                case "DescricaoTipoMotivoChamado":
                    if (!select.Contains(" TipoMotivoChamado, "))
                    {
                        if (!joins.Contains(" MotivoChamado "))
                            joins += " LEFT JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO";

                        select += "MotivoChamado.MCH_TIPO_MOTIVO_ATENDIMENTO TipoMotivoChamado, ";
                        groupBy += "MotivoChamado.MCH_TIPO_MOTIVO_ATENDIMENTO, ";
                    }
                    break;

                case "CodigoIntegracaoCliente":
                    if (!select.Contains(" CodigoIntegracaoCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                        select += "Cliente.CLI_CODIGO_INTEGRACAO CodigoIntegracaoCliente, ";
                        groupBy += "Cliente.CLI_CODIGO_INTEGRACAO, ";
                    }
                    break;

                case "CodigoIntegracaoTomador":
                    if (!select.Contains(" CodigoIntegracaoTomador, "))
                    {
                        if (!joins.Contains(" Tomador "))
                            joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = Chamado.CLI_CGCCPF_TOMADOR";

                        select += "Tomador.CLI_CODIGO_INTEGRACAO CodigoIntegracaoTomador, ";
                        groupBy += "Tomador.CLI_CODIGO_INTEGRACAO, ";
                    }
                    break;

                case "CodigoIntegracaoDestinatario":
                    if (!select.Contains(" CodigoIntegracaoDestinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                        select += "Destinatario.CLI_CODIGO_INTEGRACAO CodigoIntegracaoDestinatario, ";
                        groupBy += "Destinatario.CLI_CODIGO_INTEGRACAO, ";
                    }
                    break;

                case "ModeloVeicularCarga":
                    if (!select.Contains(" ModeloVeicularCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" ModeloVeicular "))
                            joins += " LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Carga.MVC_CODIGO = ModeloVeicular.MVC_CODIGO";

                        select += "ModeloVeicular.MVC_DESCRICAO ModeloVeicularCarga, ";
                        groupBy += "ModeloVeicular.MVC_DESCRICAO, ";
                    }
                    break;

                case "DataChegadaMotorista":
                case "DataChegadaMotoristaFormatada":
                    if (!select.Contains(" DataChegadaMotorista, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" CargaEntrega "))
                            joins += " LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO";

                        select += "CargaEntrega.CEN_DATA_ENTRADA_RAIO DataChegadaMotorista, ";
                        groupBy += "CargaEntrega.CEN_DATA_ENTRADA_RAIO, ";
                    }
                    break;

                case "TratativaAtendimento":
                case "DescricaoTratativaAtendimento":
                    if (!select.Contains(" TratativaAtendimento, "))
                    {
                        select += "Chamado.CHA_TRATATIVA_DEVOLUCAO TratativaAtendimento, ";
                        groupBy += "Chamado.CHA_TRATATIVA_DEVOLUCAO, ";
                    }
                    break;

                case "CodigoIntegracaoClienteResponsavel":
                    if (!select.Contains(" CodigoIntegracaoClienteResponsavel, "))
                    {
                        if (!joins.Contains(" ClienteResponsavel "))
                            joins += " LEFT JOIN T_CLIENTE ClienteResponsavel ON ClienteResponsavel.CLI_CGCCPF = Chamado.CLI_CGCCPF_RESPONSAVEL";

                        select += "ClienteResponsavel.CLI_CODIGO_INTEGRACAO CodigoIntegracaoClienteResponsavel, ";
                        groupBy += "ClienteResponsavel.CLI_CODIGO_INTEGRACAO, ";
                    }
                    break;

                case "CNPJClienteResponsavelFormatado":
                    if (!select.Contains(" CPFCNPJClienteResponsavel, "))
                    {
                        if (!joins.Contains(" ClienteResponsavel "))
                            joins += " LEFT JOIN T_CLIENTE ClienteResponsavel ON ClienteResponsavel.CLI_CGCCPF = Chamado.CLI_CGCCPF_RESPONSAVEL";

                        select += "ClienteResponsavel.CLI_FISJUR TipoClienteResponsavel, ClienteResponsavel.CLI_CGCCPF CPFCNPJClienteResponsavel, ";
                        groupBy += "ClienteResponsavel.CLI_FISJUR, ClienteResponsavel.CLI_CGCCPF, ";
                    }
                    break;

                case "ClienteResponsavel":
                    if (!select.Contains(" ClienteResponsavel, "))
                    {
                        if (!joins.Contains(" ClienteResponsavel "))
                            joins += " LEFT JOIN T_CLIENTE ClienteResponsavel ON ClienteResponsavel.CLI_CGCCPF = Chamado.CLI_CGCCPF_RESPONSAVEL";

                        select += "ClienteResponsavel.CLI_NOME ClienteResponsavel, ";
                        groupBy += "ClienteResponsavel.CLI_NOME, ";
                    }
                    break;

                case "GrupoPessoasResponsavel":
                    if (!select.Contains(" GrupoPessoasResponsavel, "))
                    {
                        if (!select.Contains(" GrupoPessoasResponsavel, "))
                            joins += " LEFT JOIN T_GRUPO_PESSOAS GrupoPessoasResponsavel ON GrupoPessoasResponsavel.GRP_CODIGO = Chamado.GRP_CODIGO_RESPONSAVEL";

                        select += "GrupoPessoasResponsavel.GRP_DESCRICAO GrupoPessoasResponsavel, ";
                        groupBy += "GrupoPessoasResponsavel.GRP_DESCRICAO, ";
                    }
                    break;

                case "DataRetencaoInicioFormatada":
                    if (!select.Contains(" DataRetencaoInicio, "))
                    {
                        select += "Chamado.CHA_DATA_RETENCAO_INICIO DataRetencaoInicio, ";
                        groupBy += "Chamado.CHA_DATA_RETENCAO_INICIO, ";
                    }
                    break;

                case "DataRetencaoFimFormatada":
                    if (!select.Contains(" DataRetencaoFim, "))
                    {
                        select += "Chamado.CHA_DATA_RETENCAO_FIM DataRetencaoFim, ";
                        groupBy += "Chamado.CHA_DATA_RETENCAO_FIM, ";
                    }
                    break;

                case "DataReentregaFormatada":
                    if (!select.Contains(" DataReentrega, "))
                    {
                        select += "Chamado.CHA_DATA_REENTREGA DataReentrega, ";
                        groupBy += "Chamado.CHA_DATA_REENTREGA, ";
                    }
                    break;

                case "PesoCarga":
                    if (!select.Contains(" PesoCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += @"(SELECT 
                                SUM(_cargaPedido.PED_PESO)       
                            FROM
                                T_CARGA_PEDIDO _cargaPedido      
                            WHERE
                                _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ) PesoCarga,";

                        groupBy += "Carga.CAR_CODIGO, ";
                    }
                    break;

                case "PeriodoJanelaDescarga":
                    if (!select.Contains(" PeriodoJanelaDescarga, "))
                    {
                        select += @"(SELECT top 1 (clienteDescarga.CLD_HORA_INICIO_DESCARGA + ' - ' + clienteDescarga.CLD_HORA_LIMETE_DESCARGA) 
                                    FROM T_CLIENTE_DESCARGA clienteDescarga 
                                    WHERE clienteDescarga.CLD_HORA_INICIO_DESCARGA <> '' AND clienteDescarga.CLD_HORA_INICIO_DESCARGA IS NOT NULL
                                          AND clienteDescarga.CLD_HORA_LIMETE_DESCARGA <> '' AND clienteDescarga.CLD_HORA_LIMETE_DESCARGA IS NOT NULL
                                          AND clienteDescarga.CLI_CGCCPF_ORIGEM = Chamado.CLI_CGCCPF AND clienteDescarga.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO) PeriodoJanelaDescarga, ";

                        if (!groupBy.Contains("Chamado.CLI_CGCCPF,"))
                            groupBy += "Chamado.CLI_CGCCPF, ";
                        if (!groupBy.Contains("Chamado.CLI_CGCCPF_DESTINATARIO,"))
                            groupBy += "Chamado.CLI_CGCCPF_DESTINATARIO, ";
                    }
                    break;

                case "TipoDevolucao":
                    if (!select.Contains(" DevolucaoParcial, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " LEFT JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        select += "ChamadoCargaEntrega.CEN_DEVOLUCAO_PARCIAL DevolucaoParcial, ";
                        groupBy += "ChamadoCargaEntrega.CEN_DEVOLUCAO_PARCIAL, ";
                    }
                    break;

                case "DescricaoRota":
                    if (!select.Contains(" DescricaoRota, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" RotaFrete "))
                            joins += " LEFT JOIN T_ROTA_FRETE RotaFrete ON RotaFrete.ROF_CODIGO = Carga.ROF_CODIGO";

                        select += "RotaFrete.ROF_DESCRICAO DescricaoRota, ";
                        groupBy += "RotaFrete.ROF_DESCRICAO, ";
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {

                        select += @"SUBSTRING((SELECT DISTINCT ', ' +  Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                        FROM T_CARGA_PEDIDO CargaPedido
                                        JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                        WHERE CargaPedido.CAR_CODIGO = Chamado.CAR_CODIGO
                                        FOR XML PATH('')), 3, 2000
                                    ) NumeroPedidoEmbarcador, ";

                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                    }
                    break;

                case "CNPJClienteFormatado":
                    if (!select.Contains(" CNPJCliente, "))
                    {
                        if (!joins.Contains(" Cliente "))
                            joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CHAMADO.CLI_CGCCPF";

                        select += "Cliente.CLI_FISJUR TipoCliente, Cliente.CLI_CGCCPF CNPJCliente, ";
                        groupBy += "Cliente.CLI_FISJUR, Cliente.CLI_CGCCPF, ";
                    }
                    break;

                case "JustificativaOcorrencia":
                    if (!select.Contains(" JustificativaOcorrencia, "))
                    {
                        if (!joins.Contains(" ChamadoAnalise "))
                        {
                            joins += " LEFT JOIN T_CHAMADO_ANALISES ChamadoAnalise on ChamadoAnalise.CHA_CODIGO = Chamado.CHA_CODIGO";
                            joins += " LEFT JOIN T_JUSTIFICATIVA_OCORRENCIA JustificativaOcorrencia on JustificativaOcorrencia.JTO_CODIGO = ChamadoAnalise.JTO_CODIGO";
                        }

                        select += "JustificativaOcorrencia.JTO_DESCRICAO JustificativaOcorrencia, ";
                        groupBy += "JustificativaOcorrencia.JTO_DESCRICAO, ";
                    }
                    break;

                case "DataPrevistaEntregaRetorno":
                    if (!select.Contains(" DataPrevistaEntregaRetorno, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR(10), Pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + CONVERT(VARCHAR(8), Pedido.PED_PREVISAO_ENTREGA, 108)
                                        FROM T_CARGA_PEDIDO CargaPedido
                                        JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                        WHERE CargaPedido.CAR_CODIGO = Chamado.CAR_CODIGO
                                        FOR XML PATH('')), 3, 2000
                                    ) DataPrevistaEntregaRetorno, ";

                        if (!groupBy.Contains("Chamado.CAR_CODIGO,"))
                            groupBy += "Chamado.CAR_CODIGO, ";
                    }
                    break;

                case "PossuiAnexoNFSe":
                    if (!select.Contains(" PossuiAnexoNFSe, "))
                    {
                        select += @"ISNULL((SELECT TOP(1) 'Sim' FROM T_CHAMADO_ANEXOS Anexo where Anexo.ACH_NOTA_FISCAL_SERVICO = 1 and Anexo.CHA_CODIGO = Chamado.CHA_CODIGO), 'Não') PossuiAnexoNFSe, ";

                        groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;
                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";
                        if (!joins.Contains(" TipoCarga "))
                            joins += " LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON Carga.TCG_CODIGO = TipoCarga.TCG_CODIGO";

                        select += "TipoCarga.TCG_DESCRICAO TipoCarga, ";
                        groupBy += "TipoCarga.TCG_DESCRICAO, ";
                    }
                    break;
                case "GeneroMotivoChamado":
                    if (!select.Contains(" GeneroMotivoChamado, "))
                    {
                        if (!joins.Contains(" MotivoChamado "))
                            joins += " LEFT JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO";
                        if (!joins.Contains(" GeneroMotivoChamado "))
                            joins += " LEFT JOIN T_GENERO_MOTIVO_CHAMADO GeneroMotivoChamado ON GeneroMotivoChamado.GMC_CODIGO = MotivoChamado.GMC_CODIGO ";

                        select += "GeneroMotivoChamado.GMC_DESCRICAO GeneroMotivoChamado, ";
                        groupBy += "GeneroMotivoChamado.GMC_DESCRICAO, ";
                    }
                    break;
                case "AreaEnvolvidaMotivoChamado":
                    if (!select.Contains(" AreaEnvolvidaMotivoChamado, "))
                    {
                        if (!joins.Contains(" MotivoChamado "))
                            joins += " LEFT JOIN T_MOTIVO_CHAMADA MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO";
                        if (!joins.Contains(" AreaEnvolvidaMotivoChamado "))
                            joins += " LEFT JOIN T_AREA_ENVOLVIDA_MOTIVO_CHAMADO AreaEnvolvidaMotivoChamado ON AreaEnvolvidaMotivoChamado.AEM_CODIGO = MotivoChamado.AEM_CODIGO ";

                        select += "AreaEnvolvidaMotivoChamado.AEM_DESCRICAO AreaEnvolvidaMotivoChamado, ";
                        groupBy += "AreaEnvolvidaMotivoChamado.AEM_DESCRICAO, ";
                    }
                    break;
                case "MotivoProcesso":
                    if (!select.Contains(" MotivoProcesso, "))
                    {
                        if (!joins.Contains(" MotivoProcesso "))
                            joins += " LEFT JOIN T_OCORRENCIA MotivoProcesso ON MotivoProcesso.OCO_CODIGO = Chamado.OCO_CODIGO_MOTIVO_PROCESSO";

                        select += "MotivoProcesso.OCO_DESCRICAO MotivoProcesso, ";
                        groupBy += "MotivoProcesso.OCO_DESCRICAO, ";
                    }
                    break;

                case "QuantidadeDivergencia":
                    if (!select.Contains(" QuantidadeDivergencia, "))
                    {
                        select += "Chamado.CHA_QUANTIDADE_DIVERGENCIA QuantidadeDivergencia, ";
                        groupBy += "Chamado.CHA_QUANTIDADE_DIVERGENCIA, ";
                    }
                    break;

                case "ModeloVeicularCargaEntrega":
                    if (!select.Contains(" ModeloVeicularCargaEntrega, "))
                    {
                        if (!joins.Contains(" ModeloVeicularCarga "))
                            joins += " LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON ModeloVeicularCarga.MVC_CODIGO = Chamado.MVC_CODIGO";

                        select += "ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCargaEntrega, ";
                        groupBy += "ModeloVeicularCarga.MVC_DESCRICAO, ";
                    }
                    break;
                case "GrupoTipoOcorrencia":
                    if (!select.Contains(" GrupoTipoOcorrencia, "))
                    {
                        if (!joins.Contains(" TipoOcorrencia "))
                            joins += " LEFT JOIN T_OCORRENCIA TipoOcorrencia ON Chamado.OCO_CODIGO_MOTIVO_PROCESSO = TipoOcorrencia.OCO_CODIGO";
                        if (!joins.Contains(" GrupoTipoOcorrencia, "))
                            joins += " LEFT JOIN T_GRUPO_TIPO_OCORRENCIA GrupoTipoOcorrencia ON TipoOcorrencia.GTO_CODIGO =  GrupoTipoOcorrencia.GTO_CODIGO";

                        select += "GrupoTipoOcorrencia.GTO_DESCRICAO GrupoTipoOcorrencia, ";
                        groupBy += "GrupoTipoOcorrencia.GTO_DESCRICAO, ";
                    }

                    break;

                case "Anexos":
                    if (!select.Contains(" Anexos, "))
                    {
                        select += "(CASE WHEN (SELECT COUNT(*) FROM T_CHAMADO_ANEXOS ChamadoAnexos WHERE ChamadoAnexos.CHA_CODIGO = Chamado.CHA_CODIGO) > 0 THEN 'Sim' ELSE 'Não' END) Anexos, ";

                        if (!groupBy.Contains("Chamado.CHA_CODIGO,"))
                            groupBy += "Chamado.CHA_CODIGO, ";
                    }

                    break;
                case "MotivoDevolucao":
                    if (!select.Contains(" MotivoDevolucao, "))
                    {
                        if (!joins.Contains(" MotivoDevolucao "))
                            joins += " LEFT JOIN T_MOTIVO_DEVOLUCAO_ENTREGA MotivoDevolucaoEntrega ON MotivoDevolucaoEntrega.MDE_CODIGO = Chamado.MDE_CODIGO";

                        select += "MotivoDevolucaoEntrega.MDE_DESCRICAO MotivoDevolucao, ";

                        if (!groupBy.Contains("MotivoDevolucaoEntrega.MDE_DESCRICAO,"))
                            groupBy += "MotivoDevolucaoEntrega.MDE_DESCRICAO, ";
                    }
                    break;

                case "DocumentoComplementar":

                    select += !select.Contains("CargaOcorrenciaOuterApply.DocumentoComplementar") ? "ISNULL(CargaOcorrenciaOuterApply.DocumentoComplementar, 0) as DocumentoComplementar, " : "";

                    joins += !joins.Contains("CargaOcorrenciaOuterApply") ? @" OUTER APPLY (
                                                                        	SELECT DISTINCT co.COC_NUMERO_CONTRATO AS DocumentoComplementar
                                                                        	FROM T_CARGA_OCORRENCIA co
                                                                        	inner join T_CHAMADO_OCORRENCIA cho
                                                                        	on co.COC_CODIGO = cho.COC_CODIGO
                                                                        	WHERE cho.CHA_CODIGO = Chamado.CHA_CODIGO 
                                                                        	) CargaOcorrenciaOuterApply " : "";

                    groupBy += !groupBy.Contains("CargaOcorrenciaOuterApply.DocumentoComplementar") ? "CargaOcorrenciaOuterApply.DocumentoComplementar, " : "";

                    break;

                case "ValorJaIncluso":

                    select += !select.Contains("CargaComplementoOuterApply.ValorJaIncluso") ? "ISNULL(CargaComplementoOuterApply.ValorJaIncluso, 0) as ValorJaIncluso, " : "";

                    joins += !joins.Contains("CargaComplementoOuterApply") ?
                        @" OUTER APPLY (
                            SELECT  DISTINCT  cf.CCF_VALOR_COMPONENTE AS ValorJaIncluso
                            	FROM T_CARGA_OCORRENCIA co
                            	left join T_CARGA_COMPONENTES_FRETE cf
                            	on cf.CAR_CODIGO = co.CAR_CODIGO
                            	and cf.CFR_CODIGO = CO.CFR_CODIGO
                            	WHERE CF.CAR_CODIGO = chamado.CAR_CODIGO
                            	) CargaComplementoOuterApply " : "";

                    groupBy += !groupBy.Contains("CargaComplementoOuterApply.ValorJaIncluso") ? "CargaComplementoOuterApply.ValorJaIncluso, " : "";

                    break;

                case "CPFMotorista":

                    select += !select.Contains("CargaMotorista.CPFMotorista") ? "ISNULL(CargaMotorista.CPFMotorista, '') as CPFMotorista, " : "";

                    joins += !joins.Contains("CargaMotorista") ?
                                @" OUTER APPLY (SELECT DISTINCT mt.FUN_CPF as CPFMotorista, 
                                                       CASE 
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 0 THEN 'Todos'
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 1 THEN 'Proprio'
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 2 THEN 'Terceiro'
                                                           ELSE NULL 
                                                       END as TipoMotorista
                                                       FROM T_FUNCIONARIO mt
                                                       WHERE mt.FUN_CODIGO = Chamado.FUN_MOTORISTA
                                                    ) AS CargaMotorista " : "";

                    groupBy += !groupBy.Contains("CargaMotorista.CPFMotorista") ? "CargaMotorista.CPFMotorista, " : "";

                    break;
                case "TipoMotorista":

                    select += !select.Contains("CargaMotorista.TipoMotorista") ? "ISNULL(CargaMotorista.TipoMotorista, '') as TipoMotorista, " : "";

                    joins += !joins.Contains("CargaMotorista") ?
                                @" OUTER APPLY (SELECT DISTINCT mt.FUN_CPF as CPFMotorista, 
                                                       CASE 
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 0 THEN 'Todos'
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 1 THEN 'Proprio'
                                                           WHEN mt.FUN_TIPO_MOTORISTA = 2 THEN 'Terceiro'
                                                           ELSE NULL 
                                                       END as TipoMotorista
                                                       FROM T_FUNCIONARIO mt
                                                       WHERE mt.FUN_CODIGO = Chamado.FUN_MOTORISTA
                                                    ) AS CargaMotorista " : "";

                    groupBy += !groupBy.Contains("CargaMotorista.TipoMotorista") ? "CargaMotorista.TipoMotorista, " : "";

                    break;
                case "NFAtendimento":

                    select += !select.Contains("NfxNFAtendimento.NF_NUMERO") ? "ISNULL(NfxNFAtendimento.NF_NUMERO, 0) as NFAtendimento, " : "";

                    joins += !joins.Contains("NFAtendimento")
                        ? @" LEFT JOIN T_CHAMADO_XML_NOTA_FISCAL NFAtendimento ON Chamado.CHA_CODIGO = NFAtendimento.CHA_CODIGO
                             LEFT JOIN T_XML_NOTA_FISCAL NfxNFAtendimento ON NFAtendimento.NFX_CODIGO = NfxNFAtendimento.NFX_CODIGO " : "";

                    groupBy += !groupBy.Contains("NfxNFAtendimento.NF_NUMERO") ? "NfxNFAtendimento.NF_NUMERO, " : "";


                    break;
                case "CodigoSIF":
                    select += !select.Contains("ServicoInspecaoFederal.SIF_CODIGO_SIF") ? "ISNULL(ServicoInspecaoFederal.SIF_CODIGO_SIF, '') as CodigoSIF, " : "";

                    joins += !joins.Contains("ServicoInspecaoFederal")
                        ? @"LEFT JOIN T_SERVICO_INSPECAO_FEDERAL ServicoInspecaoFederal on Chamado.SIF_CODIGO = ServicoInspecaoFederal.SIF_CODIGO" : "";

                    groupBy += !groupBy.Contains("ServicoInspecaoFederal.SIF_CODIGO_SIF") ? "ServicoInspecaoFederal.SIF_CODIGO_SIF, " : "";

                    break;

                case "DescricaoSIF":
                    select += !select.Contains("ServicoInspecaoFederal.SIF_DESCRICAO") ? "ISNULL(ServicoInspecaoFederal.SIF_DESCRICAO, '') as DescricaoSIF, " : "";

                    joins += !joins.Contains("ServicoInspecaoFederal")
                        ? @"LEFT JOIN T_SERVICO_INSPECAO_FEDERAL ServicoInspecaoFederal on Chamado.SIF_CODIGO = ServicoInspecaoFederal.SIF_CODIGO" : "";

                    groupBy += !groupBy.Contains("ServicoInspecaoFederal.SIF_DESCRICAO") ? "ServicoInspecaoFederal.SIF_DESCRICAO, " : "";

                    break;
                case "ValorOcorrencia":
                    if (!select.Contains(" ValorOcorrencia, "))
                    {
                        if (!joins.Contains(" ChamadoOcorrencia "))
                            joins += " LEFT JOIN T_CHAMADO_OCORRENCIA ChamadoOcorrencia ON ChamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO";

                        if (!joins.Contains(" CargaOcorrencia "))
                            joins += " LEFT JOIN T_CARGA_OCORRENCIA CargaOcorrencia ON CargaOcorrencia.COC_CODIGO = ChamadoOcorrencia.COC_CODIGO";

                        select += "CargaOcorrencia.COC_VALOR_OCORRENCIA ValorOcorrencia, ";
                        groupBy += !groupBy.Contains("CargaOcorrencia.COC_VALOR_OCORRENCIA") ? "CargaOcorrencia.COC_VALOR_OCORRENCIA, " : "";
                    }
                    break;

                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto, "))
                    {

                        select += @" (
                                    SELECT STUFF((
                                        SELECT DISTINCT ', ' + CAST(ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR AS NVARCHAR)
                                        FROM T_CARGA_ENTREGA_PRODUTO CargaProduto
                                        JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador ON ProdutoEmbarcador.PRO_CODIGO = CargaProduto.PRO_CODIGO
                                        WHERE CargaProduto.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                                ) AS CodigoProduto, ";

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy += "CargaEntrega.CEN_CODIGO, ";
                    }
                    break;

                case "DescricaoProduto":
                    if (!select.Contains(" DescricaoProduto, "))
                    {
                        select += @" (
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + ProdutoEmbarcador.GRP_DESCRICAO
                            FROM T_CARGA_ENTREGA_PRODUTO CargaProduto
                            JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador ON ProdutoEmbarcador.PRO_CODIGO = CargaProduto.PRO_CODIGO
                            WHERE CargaProduto.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS DescricaoProduto, ";

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO, "))
                            groupBy += "CargaEntrega.CEN_CODIGO, "; ;
                    }
                    break;

                case "QuantidadeDevolucao":
                    if (!select.Contains(" QuantidadeDevolucao, "))
                    {
                        select += @"(SELECT SUM(CONVERT(INT,ProdutoDevolvido.CPP_QUANTIDADE_DEVOLUCAO)) 
                                       FROM T_CARGA_ENTREGA AS CargaEntrega
                                 INNER JOIN T_CARGA_ENTREGA_PRODUTO_CHAMADO AS ProdutoDevolvido 
                                         ON CargaEntrega.CEN_CODIGO = ProdutoDevolvido.CEN_CODIGO
                                 INNER JOIN T_XML_NOTA_FISCAL AS NotaChamado 
                                         ON ProdutoDevolvido.NFX_CODIGO = NotaChamado.NFX_CODIGO ";
                        select += @"  WHERE CargaEntrega.CAR_CODIGO = Chamado.CAR_CODIGO ";

                        if (existeCampoNFAtendimento)
                            select += @" AND NotaChamado.NFX_CODIGO = NFAtendimento.NFX_CODIGO ";

                        select += @"   ) QuantidadeDevolucao, ";

                        if (existeCampoNFAtendimento && !groupBy.Contains("NFAtendimento.NFX_CODIGO"))
                            groupBy += "CargaEntrega.CEN_CODIGO, ";

                    }
                    break;

                case "ValorDevolucao":
                    if (!select.Contains(" ValorDevolucao, "))
                    {
                        select += @"(SELECT SUM(ProdutoDevolvido.CPP_VALOR_DEVOLUCAO) 
                                       FROM T_CARGA_ENTREGA AS CargaEntrega
                                 INNER JOIN T_CARGA_ENTREGA_PRODUTO_CHAMADO AS ProdutoDevolvido 
                                         ON CargaEntrega.CEN_CODIGO = ProdutoDevolvido.CEN_CODIGO
                                 INNER JOIN T_XML_NOTA_FISCAL AS NotaChamado 
                                         ON ProdutoDevolvido.NFX_CODIGO = NotaChamado.NFX_CODIGO ";
                        select += @"  WHERE CargaEntrega.CAR_CODIGO = Chamado.CAR_CODIGO ";

                        if (existeCampoNFAtendimento)
                            select += @" AND NotaChamado.NFX_CODIGO = NFAtendimento.NFX_CODIGO ";

                        select += @"   ) ValorDevolucao, ";

                        if (existeCampoNFAtendimento && !groupBy.Contains("NFAtendimento.NFX_CODIGO"))
                            groupBy += "NFAtendimento.NFX_CODIGO, ";
                    }
                    break;

                case "NumeroNFD":
                    if (!select.Contains(" NumeroNFD, "))
                    {
                        select += @"(
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_NUMERO AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                    ) AS NumeroNFD, ";


                    }
                    break;

                case "SerieNFD":
                    if (!select.Contains(" SerieNFD, "))
                    {
                        select += @"  (
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_SERIE AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS SerieNFD, ";

                    }
                    break;

                case "ChaveNFD":
                    if (!select.Contains(" ChaveNFD, "))
                    {
                        select += @"(
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_CHAVE_NFE AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS ChaveNFD, ";


                    }
                    break;

                case "DataEmissaoNFD":
                    if (!select.Contains(" DataEmissaoNFD, "))
                    {
                        select += @"(
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CONVERT(NVARCHAR, EntregaDevolucao.CND_DATA_EMISSAO, 120)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS DataEmissaoNFD, ";


                    }
                    break;

                case "ValorTotalProdutoNFD":
                    if (!select.Contains(" ValorTotalProdutoNFD, "))
                    {
                        select += @" (
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_VALOR_TOTAL_PRODUTOS AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                    ) AS ValorTotalProdutoNFD, ";

                    }
                    break;

                case "ValorTotalNFNFD":
                    if (!select.Contains(" ValorTotalNFNFD, "))
                    {
                        select += @"
                        (
                            SELECT STUFF((
                                SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_VALOR_TOTAL AS NVARCHAR)
                                FROM T_CARGA Carga  
                                JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS ValorTotalNFNFD, ";


                    }
                    break;

                case "PesoDevolvidoNFD":
                    if (!select.Contains(" PesoDevolvidoNFD, "))
                    {
                        select += @"(
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(EntregaDevolucao.CND_PESO_DEVOLVIDO AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS PesoDevolvidoNFD, ";


                    }
                    break;

                case "NFeOrigem":
                    if (!select.Contains(" NFeOrigem, "))
                    {
                        select += @"(
                        SELECT STUFF((
                            SELECT DISTINCT ', ' + CAST(NotaFiscal.NF_NUMERO AS NVARCHAR)
                            FROM T_CARGA Carga  
                            JOIN T_CARGA_ENTREGA CargaEntrega ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                            JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO EntregaDevolucao ON EntregaDevolucao.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = EntregaDevolucao.NFX_CODIGO
                            WHERE Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                        ) AS NFeOrigem, ";

                    }
                    break;

                case "QuantidadeTotalDevolvidaNoChamado":
                    if (!select.Contains(" QuantidadeTotalDevolvidaNoChamado, "))
                    {
                        select += @"
                                    (SELECT COALESCE(
                                        SUM(NULLIF(produto.CPP_QUANTIDADE_DEVOLUCAO - cargaEntregaProdutoChamado.CPP_QUANTIDADE_DEVOLUCAO, NULL)), 
                                        0) AS QuantidadeDevolucaoCalculada
                                     FROM T_CARGA_ENTREGA_PRODUTO_CHAMADO cargaEntregaProdutoChamado
                                     INNER JOIN T_CARGA_ENTREGA_PRODUTO produto
                                         ON cargaEntregaProdutoChamado.PRO_CODIGO = produto.PRO_CODIGO 
                                         AND produto.NFX_CODIGO = cargaEntregaProdutoChamado.NFX_CODIGO
                                         AND produto.CEN_CODIGO = cargaEntregaProdutoChamado.CEN_CODIGO
                                     WHERE cargaEntregaProdutoChamado.CHA_CODIGO = Chamado.CHA_CODIGO
                                    ) QuantidadeTotalDevolvidaNoChamado,";

                        if (!groupBy.Contains("Chamado.CHA_CODIGO,"))
                            groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;
                case "QuantidadeValePallet":
                    if (!select.Contains(" QuantidadeValePallet, "))
                    {
                        if (!joins.Contains(" valePallet "))
                            joins += " LEFT JOIN T_PALLET_VALE_PALLET valePallet ON valePallet.CHA_CODIGO = Chamado.CHA_CODIGO";

                        select += "valePallet.VLP_QUANTIDADE QuantidadeValePallet, ";
                        groupBy += "valePallet.VLP_QUANTIDADE, ";
                    }
                    break;

                case "Estadia":
                case "EstadiaFormatado":
                    if (!select.Contains(" Estadia, "))
                    {
                        select += "Chamado.CHA_ESTADIA Estadia, ";
                        groupBy += "Chamado.CHA_ESTADIA, ";
                    }
                    break;

                case "SenhaDevolucao":
                    if (!select.Contains(" SenhaDevolucao, "))
                    {
                        select += "Chamado.CHA_SENHA_DEVOLUCAO SenhaDevolucao, ";
                        groupBy += "Chamado.CHA_SENHA_DEVOLUCAO, ";
                    }
                    break;
            }
        }

        private void SetarJoinPadraoParaCamposDevolucao(string propriedade, ref string joins, ref string groupBy)
        {
            switch (propriedade)
            {
                case "CodigoProduto":
                case "DescricaoProduto":
                case "QuantidadeDevolucao":
                case "ValorDevolucao":
                case "NumeroNFD":
                case "SerieNFD":
                case "ChaveNFD":
                case "DataEmissaoNFD":
                case "ValorTotalProdutoNFD":
                case "ValorTotalNFNFD":
                case "PesoDevolvidoNFD":
                case "NFeOrigem":

                    if (!joins.Contains(" Carga "))
                        joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                    if (!joins.Contains(" CargaEntrega "))
                        joins += " LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO";
                    if (!groupBy.Contains("Chamado.CAR_CODIGO"))
                        groupBy += "Chamado.CAR_CODIGO, ";

                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaChamadoOcorrencia(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.NumeroInicial > 0)
                where += " AND Chamado.CHA_NUMERO >= " + filtrosPesquisa.NumeroInicial;

            if (filtrosPesquisa.NumeroFinal > 0)
                where += " AND Chamado.CHA_NUMERO <= " + filtrosPesquisa.NumeroFinal;

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += " AND Chamado.EMP_CODIGO = " + filtrosPesquisa.CodigoTransportador;

            if (filtrosPesquisa.CodigoResponsavel > 0)
                where += " AND Chamado.FUN_RESPONSAVEL = " + filtrosPesquisa.CodigoResponsavel;

            if (filtrosPesquisa.Situacao != SituacaoChamado.Todas)
                where += " AND Chamado.CHA_SITUACAO = " + filtrosPesquisa.Situacao.ToString("d");

            if (filtrosPesquisa.Filiais.Any(codigo => codigo == -1))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += $@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )";
            }
            else if (filtrosPesquisa.CodigoFilial > 0)
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += " AND Carga.FIL_CODIGO = " + filtrosPesquisa.CodigoFilial;
            }

            if (filtrosPesquisa.CodigosMotivo.Count > 0)
                where += $" AND Chamado.MCH_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosMotivo)})";

            if (filtrosPesquisa.CpfCnpjCliente > 0)
                where += " AND Chamado.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjCliente;

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                where += " AND Chamado.CLI_CGCCPF_TOMADOR = " + filtrosPesquisa.CpfCnpjTomador;

            if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                where += " AND Chamado.CLI_CGCCPF_DESTINATARIO = " + filtrosPesquisa.CpfCnpjDestinatario;

            if (filtrosPesquisa.CodigoMotorista > 0)
                where += " AND Chamado.CAR_CODIGO IN (SELECT CAR_CODIGO FROM T_CARGA_MOTORISTA WHERE CAR_MOTORISTA = " + filtrosPesquisa.CodigoMotorista + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoRepresentante > 0)
                where += " AND Chamado.REP_CODIGO = " + filtrosPesquisa.CodigoRepresentante;

            if (filtrosPesquisa.Nota > 0)
            {
                where += @" AND EXISTS (
									SELECT 1
									FROM (
										SELECT 1 AS SubQuery
										WHERE Chamado.CEN_CODIGO IS NOT NULL AND EXISTS (
											SELECT 1
											FROM T_CARGA_PEDIDO _cargaPedido
											JOIN T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido ON _cargaEntregaPedido.CEN_CODIGO = Chamado.CEN_CODIGO
											JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal ON _pedidoxmlnotafiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
											JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO
											WHERE
												(_cargaEntregaPedido.CPE_CODIGO IS NULL OR _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO)
												AND _xmlnotafiscal.NF_NUMERO = " + filtrosPesquisa.Nota + @"
										)
										UNION ALL
										SELECT 1 AS SubQuery
										WHERE Chamado.CEN_CODIGO IS NULL AND EXISTS (
											SELECT 1
											FROM T_CARGA_PEDIDO _cargaPedido
											JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
											JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal ON _pedidoxmlnotafiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
											JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO
											WHERE
												_cargaPedido.CAR_CODIGO = Chamado.CAR_CODIGO
												AND (Chamado.CLI_CGCCPF IS NULL OR _pedido.CLI_CODIGO = Chamado.CLI_CGCCPF)
												AND _xmlnotafiscal.NF_NUMERO = " + filtrosPesquisa.Nota + @"
										)
									) AS SubQuery)";
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where += @" AND Chamado.CAR_CODIGO IN (
	                SELECT _cargaCTe.CAR_CODIGO
	                FROM T_CARGA_CTE _cargaCTe
	                LEFT JOIN T_CTE _cte ON _cte.CON_CODIGO = _cargaCTe.CON_CODIGO
	                WHERE _cte.CON_NUM = " + filtrosPesquisa.NumeroCTe + @"
                )";
            }

            if (filtrosPesquisa.DataCriacaoInicio != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_CRICAO AS DATE) >= '" + filtrosPesquisa.DataCriacaoInicio.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataCriacaoFim != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_CRICAO AS DATE) <= '" + filtrosPesquisa.DataCriacaoFim.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalizacaoInicio != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_FINALIZACAO AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicio.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalizacaoFim != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_FINALIZACAO AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFim.ToString(pattern) + "' ";

            if (filtrosPesquisa.GerouOcorrencia)
                where += " AND Chamado.CHA_CODIGO IN (SELECT chamadoOcorrencia.CHA_CODIGO FROM T_CHAMADO_OCORRENCIA chamadoOcorrencia WHERE chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO)";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (!joins.Contains(" VeiculoCarga "))
                    joins += " LEFT JOIN T_VEICULO VeiculoCarga ON VeiculoCarga.VEI_CODIGO = Carga.CAR_VEICULO";

                where += $@" AND (VeiculoCarga.VEI_PLACA = '{filtrosPesquisa.Placa}' OR Carga.CAR_CODIGO IN 
                                (select veiculoVinculado.CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS veiculoVinculado 
                                join T_VEICULO veiculo on veiculo.VEI_CODIGO = veiculoVinculado.VEI_CODIGO
                                where veiculo.VEI_PLACA = '{filtrosPesquisa.Placa}'))";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += $@" AND (Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} OR Carga.CAR_CODIGO IN 
                                (select veiculoVinculado.CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS veiculoVinculado 
                                where veiculoVinculado.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}))";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where += " AND Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%" + filtrosPesquisa.Carga + "%'";
                else
                    where += " AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.Carga + "'";
            }

            if (filtrosPesquisa.CodigoGrupoPessoasTomador > 0)
            {
                if (!joins.Contains(" Tomador "))
                    joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = Chamado.CLI_CGCCPF_TOMADOR";

                where += " AND Tomador.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasTomador;
            }

            if (filtrosPesquisa.CodigoGrupoPessoasCliente > 0)
            {
                if (!joins.Contains(" Cliente "))
                    joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                where += " AND Cliente.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasCliente;
            }

            if (filtrosPesquisa.CodigoGrupoPessoasDestinatario > 0)
            {
                if (!joins.Contains(" Destinatario "))
                    joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                where += " AND Destinatario.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasDestinatario;
            }

            if (filtrosPesquisa.CodigoFilialVenda > 0)
            {
                if (!joins.Contains(" Carga_Pedido "))
                    joins += " LEFT JOIN T_CARGA_PEDIDO Carga_Pedido ON Carga_Pedido.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (!joins.Contains(" _Pedido "))
                    joins += " LEFT JOIN T_PEDIDO _Pedido ON _Pedido.PED_CODIGO = Carga_Pedido.PED_CODIGO";

                if (!joins.Contains(" FilialVenda "))
                    joins += " LEFT JOIN T_FILIAL FilialVenda ON FilialVenda.FIL_CODIGO = _Pedido.FIL_CODIGO_VENDA";

                where += " AND FilialVenda.FIL_CODIGO = " + filtrosPesquisa.CodigoFilialVenda;
            }

            if (filtrosPesquisa.DataInicialChegadaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_INICIO AS DATE) >= '" + filtrosPesquisa.DataInicialChegadaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataFinalChegadaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_INICIO AS DATE) <= '" + filtrosPesquisa.DataFinalChegadaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataInicialSaidaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_FIM AS DATE) >= '" + filtrosPesquisa.DataInicialSaidaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataFinalSaidaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_FIM AS DATE) <= '" + filtrosPesquisa.DataFinalSaidaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.CpfCnpjClienteResponsavel.Count > 0)
                where += $" AND Chamado.CLI_CGCCPF_RESPONSAVEL IN ({string.Join(", ", filtrosPesquisa.CpfCnpjClienteResponsavel)})";

            if (filtrosPesquisa.CodigosGrupoPessoasResponsavel.Count > 0)
                where += $" AND Chamado.GRP_CODIGO_RESPONSAVEL IN ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoasResponsavel)})";

            if (filtrosPesquisa.SomenteAtendimentoEstornado)
                where += " AND Chamado.CHA_ESTORNADO = 1";

            if (filtrosPesquisa.PossuiAnexoNFSe.HasValue)
            {
                if (filtrosPesquisa.PossuiAnexoNFSe.Value)
                    where += " AND EXISTS(SELECT Anexo.CHA_CODIGO FROM T_CHAMADO_ANEXOS Anexo where Anexo.ACH_NOTA_FISCAL_SERVICO = 1 and Anexo.CHA_CODIGO = Chamado.CHA_CODIGO) ";
                else
                    where += " AND NOT EXISTS(SELECT Anexo.CHA_CODIGO FROM T_CHAMADO_ANEXOS Anexo where Anexo.ACH_NOTA_FISCAL_SERVICO = 1 and Anexo.CHA_CODIGO = Chamado.CHA_CODIGO) ";
            }
        }

        #endregion

        #region Gráfico de Chamados

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.GraficoChamado> GraficoChamados(DateTime dataInicio, DateTime dataFim, int responsavel, int motivo, int transportador)
        {
            string sql = ObterSelectConsultaGraficoChamado(dataInicio, dataFim, responsavel, motivo, transportador);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.GraficoChamado)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.GraficoChamado>();
        }

        private string ObterSelectConsultaGraficoChamado(DateTime dataInicio, DateTime dataFim, int responsavel, int motivo, int transportador)
        {
            string joins = string.Empty,
                   where = string.Empty;

            SetarWhereRelatorioConsultaGraficoChamado(ref where, ref joins, dataInicio, dataFim, responsavel, motivo, transportador);

            string query = @"SELECT 
	                            COUNT(*) Quantidade,
	                            Chamado.CHA_SITUACAO Situacao
                            FROM T_CHAMADOS Chamado

                            " + joins + @"

                            WHERE 1 = 1 " + where + @"
                            GROUP BY Chamado.CHA_SITUACAO
                            ORDER BY Quantidade DESC";


            return query;
        }

        private void SetarWhereRelatorioConsultaGraficoChamado(ref string where, ref string joins, DateTime dataInicio, DateTime dataFim, int responsavel, int motivo, int transportador)
        {
            string pattern = "yyyy-MM-dd";

            if (dataInicio != DateTime.MinValue)
                where += " AND Chamado.CHA_DATA_CRICAO >= '" + dataInicio.ToString(pattern) + "' ";

            if (dataFim != DateTime.MinValue)
                where += " AND Chamado.CHA_DATA_CRICAO <= '" + dataFim.ToString(pattern) + " 23:59:59' ";

            if (transportador > 0)
                where += " AND Chamado.EMP_CODIGO = " + transportador;

            if (responsavel > 0)
                where += " AND Chamado.FUN_CODIGO = " + responsavel;

            if (motivo > 0)
                where += " AND Chamado.MCH_CODIGO = " + motivo;
        }

        #endregion

        #region Relatório Chamados Devolução

        public IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao> ConsultarChamadoDevolucao(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = ObterSelectConsultaChamadoDevolucao(filtrosPesquisa, false, propriedades, parametroConsulta);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao>();
        }

        public int ContarConsultaChamadoDevolucao(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaChamadoDevolucao(filtrosPesquisa, true, propriedades);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaChamadoDevolucao(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaChamadoDevolucao(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaChamadoDevolucao(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeAgrupar))
                {
                    SetarSelectRelatorioConsultaChamadoDevolucao(parametroConsulta.PropriedadeAgrupar, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(parametroConsulta.PropriedadeAgrupar))
                        orderBy = parametroConsulta.PropriedadeAgrupar + " " + parametroConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                {
                    if (parametroConsulta.PropriedadeOrdenar != parametroConsulta.PropriedadeAgrupar && select.Contains(parametroConsulta.PropriedadeOrdenar) && parametroConsulta.PropriedadeOrdenar != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametroConsulta.PropriedadeOrdenar + " " + parametroConsulta.DirecaoOrdenar;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_CHAMADOS Chamado ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && parametroConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametroConsulta.InicioRegistros.ToString() + " ROWS FETCH NEXT " + parametroConsulta.LimiteRegistros.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaChamadoDevolucao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Chamado.CHA_CODIGO Codigo, ";
                        groupBy += "Chamado.CHA_CODIGO, ";
                    }
                    break;

                case "NumeroCte":
                    if (!select.Contains(" NumeroCte, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        if (!joins.Contains(" NotaFiscal "))
                            joins += " INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = ChamadoCargaEntregaProduto.NFX_CODIGO";

                        if (!joins.Contains(" CTeNotaFiscalXml "))
                            joins += " INNER JOIN T_CTE_XML_NOTAS_FISCAIS CTeNotaFiscalXml ON CTeNotaFiscalXml.NFX_CODIGO = NotaFiscal.NFX_CODIGO";

                        if (!joins.Contains(" CTe "))
                            joins += " INNER JOIN T_CTE CTe ON CTe.CON_CODIGO = CTeNotaFiscalXml.CON_CODIGO";

                        select += "CTe.CON_NUM NumeroCte, ";
                        groupBy += "CTe.CON_NUM, ";
                    }
                    break;

                case "NfDevolucao":
                    if (!select.Contains(" NfDevolucao, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        select += "ChamadoCargaEntregaProduto.CPP_NF_DEVOLUCAO NfDevolucao, ";
                        groupBy += "ChamadoCargaEntregaProduto.CPP_NF_DEVOLUCAO, ";
                    }
                    break;

                case "NfOrigem":
                    if (!select.Contains(" NfOrigem, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        if (!joins.Contains(" NotaFiscal "))
                            joins += " INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = ChamadoCargaEntregaProduto.NFX_CODIGO";

                        select += "NotaFiscal.NF_NUMERO NfOrigem, ";
                        groupBy += "NotaFiscal.NF_NUMERO, ";
                    }
                    break;

                case "MotivoDevolucao":
                    if (!select.Contains(" MotivoDevolucao, "))
                    {
                        if (!joins.Contains(" MotivoDevolucaoEntrega "))
                            joins += " LEFT JOIN T_MOTIVO_DEVOLUCAO_ENTREGA MotivoDevolucaoEntrega ON MotivoDevolucaoEntrega.MDE_CODIGO = Chamado.MDE_CODIGO";

                        select += "MotivoDevolucaoEntrega.MDE_DESCRICAO MotivoDevolucao, ";
                        groupBy += "MotivoDevolucaoEntrega.MDE_DESCRICAO, ";
                    }
                    break;

                case "TipoDevolucao":
                    if (!select.Contains(" DevolucaoParcial, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        select += "Chamado.CHA_SITUACAO Situacao, ";
                        select += "ChamadoCargaEntrega.CEN_DEVOLUCAO_PARCIAL DevolucaoParcial, ";
                        groupBy += "Chamado.CHA_SITUACAO, ";
                        groupBy += "ChamadoCargaEntrega.CEN_DEVOLUCAO_PARCIAL, ";
                    }
                    break;

                case "Origens":
                    if (!select.Contains(" Origens, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" DadosSumarizados "))
                            joins += " INNER JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO";

                        select += "DadosSumarizados.CDS_ORIGENS Origens, ";
                        groupBy += "DadosSumarizados.CDS_ORIGENS, ";
                    }
                    break;

                case "Destinos":
                    if (!select.Contains(" Destinos, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" DadosSumarizados "))
                            joins += " INNER JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO";

                        select += "DadosSumarizados.CDS_DESTINOS Destinos, ";
                        groupBy += "DadosSumarizados.CDS_DESTINOS, ";
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!joins.Contains(" DadosSumarizados "))
                            joins += " INNER JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO";

                        select += "DadosSumarizados.CDS_VEICULOS Veiculos, ";
                        groupBy += "DadosSumarizados.CDS_VEICULOS, ";
                    }
                    break;

                case "QuantidadeDevolucao":
                    if (!select.Contains(" QuantidadeDevolucao, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        select += "ChamadoCargaEntregaProduto.CPP_QUANTIDADE_DEVOLUCAO QuantidadeDevolucao, ";
                        groupBy += "ChamadoCargaEntregaProduto.CPP_QUANTIDADE_DEVOLUCAO, ";
                    }
                    break;

                case "ValorDevolucao":
                    if (!select.Contains(" ValorDevolucao, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        select += "ChamadoCargaEntregaProduto.CPP_VALOR_DEVOLUCAO ValorDevolucao, ";
                        groupBy += "ChamadoCargaEntregaProduto.CPP_VALOR_DEVOLUCAO, ";
                    }
                    break;

                case "ValorTotalMercadoria":
                    if (!select.Contains(" ValorTotalMercadoria, "))
                    {
                        if (!joins.Contains(" ChamadoCargaEntrega "))
                            joins += " INNER JOIN T_CARGA_ENTREGA ChamadoCargaEntrega ON ChamadoCargaEntrega.CEN_CODIGO = Chamado.CEN_CODIGO";

                        if (!joins.Contains(" ChamadoCargaEntregaProduto "))
                            joins += " INNER JOIN T_CARGA_ENTREGA_PRODUTO ChamadoCargaEntregaProduto ON ChamadoCargaEntregaProduto.CEN_CODIGO = ChamadoCargaEntrega.CEN_CODIGO";

                        if (!joins.Contains(" NotaFiscal "))
                            joins += " INNER JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = ChamadoCargaEntregaProduto.NFX_CODIGO";

                        select += "NotaFiscal.NF_VALOR_TOTAL_PRODUTOS ValorTotalMercadoria, ";
                        groupBy += "NotaFiscal.NF_VALOR_TOTAL_PRODUTOS, ";
                    }
                    break;
                case "NumeroAtendimento":
                    if (!select.Contains(" NumeroAtendimento, "))
                    {
                        select += "chamado.CHA_NUMERO NumeroAtendimento, ";
                        groupBy += "chamado.CHA_NUMERO, ";
                    }
                    break;
                case "DataAberturaAtendimentoFormatado":
                    if (!select.Contains(" DataAberturaAtendimento, "))
                    {
                        select += "chamado.CHA_DATA_CRICAO DataAberturaAtendimento, ";
                        groupBy += "chamado.CHA_DATA_CRICAO, ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        select += "carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "carga.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }
                    break;
                case "GrupoTomadorCarga":
                    if (!select.Contains(" GrupoTomadorCarga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                        if (!select.Contains(" GrupoPessoas, "))
                            joins += " LEFT JOIN T_GRUPO_PESSOAS GrupoPessoas  ON GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO";

                        select += "GrupoPessoas.GRP_DESCRICAO GrupoTomadorCarga, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";
                    }
                    break;
                case "ResponsavelAtendimento":
                    if (!select.Contains(" ResponsavelAtendimento, "))
                    {
                        if (!joins.Contains(" UsuarioResponsavel "))
                            joins += " LEFT JOIN T_FUNCIONARIO UsuarioResponsavel on usuarioResponsavel.FUN_CODIGO = chamado.FUN_RESPONSAVEL ";

                        select += "UsuarioResponsavel.FUN_NOME ResponsavelAtendimento, ";
                        groupBy += "UsuarioResponsavel.FUN_NOME, ";
                    }
                    break;
                case "QuantidadeDevolvida":
                    if (!select.Contains(" QuantidadeDevolvida, "))
                    {
                        select += @"SUM(COALESCE(ChamadoCargaEntregaProduto.CPP_QUANTIDADE_DEVOLUCAO,0)) as QuantidadeDevolvida, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaChamadoDevolucao(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" MotivoChamada "))
                joins += " LEFT JOIN T_MOTIVO_CHAMADA MotivoChamada ON MotivoChamada.MCH_CODIGO = Chamado.MCH_CODIGO";

            where += " AND MotivoChamada.MCH_TIPO_MOTIVO_ATENDIMENTO = 1 ";

            if (filtrosPesquisa.NumeroInicial > 0)
                where += " AND Chamado.CHA_NUMERO >= " + filtrosPesquisa.NumeroInicial;

            if (filtrosPesquisa.NumeroFinal > 0)
                where += " AND Chamado.CHA_NUMERO <= " + filtrosPesquisa.NumeroFinal;

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += " AND Chamado.EMP_CODIGO = " + filtrosPesquisa.CodigoTransportador;

            if (filtrosPesquisa.CodigoResponsavel > 0)
                where += " AND Chamado.FUN_RESPONSAVEL = " + filtrosPesquisa.CodigoResponsavel;

            if (filtrosPesquisa.Situacao != SituacaoChamado.Todas)
                where += " AND Chamado.CHA_SITUACAO = " + filtrosPesquisa.Situacao.ToString("d");

            if (filtrosPesquisa.Filiais.Any(codigo => codigo == -1))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += $@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )";
            }
            else if (filtrosPesquisa.CodigoFilial > 0)
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += " AND Carga.FIL_CODIGO = " + filtrosPesquisa.CodigoFilial;
            }

            if (filtrosPesquisa.CodigosMotivo.Count > 0)
                where += $" AND Chamado.MCH_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosMotivo)})";

            if (filtrosPesquisa.CpfCnpjCliente > 0)
                where += " AND Chamado.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjCliente;

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                where += " AND Chamado.CLI_CGCCPF_TOMADOR = " + filtrosPesquisa.CpfCnpjTomador;

            if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                where += " AND Chamado.CLI_CGCCPF_DESTINATARIO = " + filtrosPesquisa.CpfCnpjDestinatario;

            if (filtrosPesquisa.CodigoMotorista > 0)
                where += " AND Chamado.CAR_CODIGO IN (SELECT CAR_CODIGO FROM T_CARGA_MOTORISTA WHERE CAR_MOTORISTA = " + filtrosPesquisa.CodigoMotorista + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoRepresentante > 0)
                where += " AND Chamado.REP_CODIGO = " + filtrosPesquisa.CodigoRepresentante;

            if (filtrosPesquisa.Nota > 0)
            {
                where += @" AND EXISTS (SELECT _cargaPedido.CAR_CODIGO
	                                    FROM T_CARGA_PEDIDO _cargaPedido
                                        JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
		                                JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoxmlnotafiscal ON _pedidoxmlnotafiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                        JOIN T_XML_NOTA_FISCAL _xmlnotafiscal ON _xmlnotafiscal.NFX_CODIGO = _pedidoxmlnotafiscal.NFX_CODIGO
		                                WHERE _cargaPedido.CAR_CODIGO = Chamado.CAR_CODIGO AND (Chamado.CLI_CGCCPF IS NULL OR _pedido.CLI_CODIGO = Chamado.CLI_CGCCPF) AND _xmlnotafiscal.NF_NUMERO = " + filtrosPesquisa.Nota + @"
                )";
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where += @" AND Chamado.CAR_CODIGO IN (
	                SELECT _cargaCTe.CAR_CODIGO
	                FROM T_CARGA_CTE _cargaCTe
	                LEFT JOIN T_CTE _cte ON _cte.CON_CODIGO = _cargaCTe.CON_CODIGO
	                WHERE _cte.CON_NUM = " + filtrosPesquisa.NumeroCTe + @"
                )";
            }

            if (filtrosPesquisa.DataCriacaoInicio != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_CRICAO AS DATE) >= '" + filtrosPesquisa.DataCriacaoInicio.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataCriacaoFim != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_CRICAO AS DATE) <= '" + filtrosPesquisa.DataCriacaoFim.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalizacaoInicio != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_FINALIZACAO AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicio.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalizacaoFim != DateTime.MinValue)
                where += " AND CAST(Chamado.CHA_DATA_FINALIZACAO AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFim.ToString(pattern) + "' ";

            if (filtrosPesquisa.GerouOcorrencia)
                where += " AND Chamado.CHA_CODIGO IN (SELECT chamadoOcorrencia.CHA_CODIGO FROM T_CHAMADO_OCORRENCIA chamadoOcorrencia WHERE chamadoOcorrencia.CHA_CODIGO = Chamado.CHA_CODIGO)";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (!joins.Contains(" VeiculoCarga "))
                    joins += " LEFT JOIN T_VEICULO VeiculoCarga ON VeiculoCarga.VEI_CODIGO = Carga.CAR_VEICULO";

                where += $@" AND (VeiculoCarga.VEI_PLACA = '{filtrosPesquisa.Placa}' OR Carga.CAR_CODIGO IN 
                                (select veiculoVinculado.CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS veiculoVinculado 
                                join T_VEICULO veiculo on veiculo.VEI_CODIGO = veiculoVinculado.VEI_CODIGO
                                where veiculo.VEI_PLACA = '{filtrosPesquisa.Placa}'))";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                where += $@" AND (Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} OR Carga.CAR_CODIGO IN 
                                (select veiculoVinculado.CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS veiculoVinculado 
                                where veiculoVinculado.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}))";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where += " AND Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%" + filtrosPesquisa.Carga + "%'";
                else
                    where += " AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.Carga + "'";
            }

            if (filtrosPesquisa.CodigoGrupoPessoasTomador > 0)
            {
                if (!joins.Contains(" Tomador "))
                    joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = Chamado.CLI_CGCCPF_TOMADOR";

                where += " AND Tomador.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasTomador;
            }

            if (filtrosPesquisa.CodigoGrupoPessoasCliente > 0)
            {
                if (!joins.Contains(" Cliente "))
                    joins += " LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Chamado.CLI_CGCCPF";

                where += " AND Cliente.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasCliente;
            }

            if (filtrosPesquisa.CodigoGrupoPessoasDestinatario > 0)
            {
                if (!joins.Contains(" Destinatario "))
                    joins += " LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Chamado.CLI_CGCCPF_DESTINATARIO";

                where += " AND Destinatario.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoasDestinatario;
            }

            if (filtrosPesquisa.CodigoFilialVenda > 0)
            {
                if (!joins.Contains(" Carga_Pedido "))
                    joins += " LEFT JOIN T_CARGA_PEDIDO Carga_Pedido ON Carga_Pedido.CAR_CODIGO = Chamado.CAR_CODIGO";

                if (!joins.Contains(" _Pedido "))
                    joins += " LEFT JOIN T_PEDIDO _Pedido ON _Pedido.PED_CODIGO = Carga_Pedido.PED_CODIGO";

                if (!joins.Contains(" FilialVenda "))
                    joins += " LEFT JOIN T_FILIAL FilialVenda ON FilialVenda.FIL_CODIGO = _Pedido.FIL_CODIGO_VENDA";

                where += " AND FilialVenda.FIL_CODIGO = " + filtrosPesquisa.CodigoFilialVenda;
            }

            if (filtrosPesquisa.DataInicialChegadaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_INICIO AS DATE) >= '" + filtrosPesquisa.DataInicialChegadaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataFinalChegadaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_INICIO AS DATE) <= '" + filtrosPesquisa.DataFinalChegadaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataInicialSaidaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_FIM AS DATE) >= '" + filtrosPesquisa.DataInicialSaidaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataFinalSaidaDiaria != DateTime.MinValue)
            {
                if (!joins.Contains(" ChamadoData "))
                    joins += " LEFT JOIN T_CHAMADO_DATA ChamadoData ON ChamadoData.CHA_CODIGO = Chamado.CHA_CODIGO";

                where += " AND CAST(ChamadoData.CDA_DATA_FIM AS DATE) <= '" + filtrosPesquisa.DataFinalSaidaDiaria.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.CpfCnpjClienteResponsavel.Count > 0)
                where += $" AND Chamado.CLI_CGCCPF_RESPONSAVEL IN ({string.Join(", ", filtrosPesquisa.CpfCnpjClienteResponsavel)})";

            if (filtrosPesquisa.CodigosGrupoPessoasResponsavel.Count > 0)
                where += $" AND Chamado.GRP_CODIGO_RESPONSAVEL IN ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoasResponsavel)})";

            if (filtrosPesquisa.SomenteAtendimentoEstornado)
                where += " AND Chamado.CHA_ESTORNADO = 1";

            if (filtrosPesquisa.PossuiAnexoNFSe.HasValue)
            {
                if (filtrosPesquisa.PossuiAnexoNFSe.Value)
                    where += " AND EXISTS(SELECT Anexo.CHA_CODIGO FROM T_CHAMADO_ANEXOS Anexo where Anexo.ACH_NOTA_FISCAL_SERVICO = 1 and Anexo.CHA_CODIGO = Chamado.CHA_CODIGO) ";
                else
                    where += " AND NOT EXISTS(SELECT Anexo.CHA_CODIGO FROM T_CHAMADO_ANEXOS Anexo where Anexo.ACH_NOTA_FISCAL_SERVICO = 1 and Anexo.CHA_CODIGO = Chamado.CHA_CODIGO) ";
            }
        }

        #endregion
    }
}
