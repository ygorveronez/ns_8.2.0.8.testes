using AdminMultisoftware.Dominio.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class TipoDeOcorrenciaDeCTe : RepositorioBase<Dominio.Entidades.TipoDeOcorrenciaDeCTe>, Dominio.Interfaces.Repositorios.TipoDeOcorrenciaDeCTe
    {
        private CancellationToken _cancellationToken;
        public TipoDeOcorrenciaDeCTe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoDeOcorrenciaDeCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }

        #region Métodos Públicos

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.TipoDeOcorrenciaDeCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigoProceda(string codigoProceda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where (obj.CodigoProceda == codigoProceda) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigoIntegracaoAuxiliar(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.CodigoIntegracaoAuxiliar == codigoIntegracao && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarCodigosIntegracaoAuxiliarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Ativo);
            result = result.Where(obj => obj.UsarMobile);
            result = result.Where(obj => obj.CodigoIntegracaoAuxiliar != null && obj.CodigoIntegracaoAuxiliar != string.Empty);

            return result.Select(o => o.CodigoIntegracaoAuxiliar).ToList();
        }
        public List<string> BuscarIdsSuperAppAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Ativo);
            result = result.Where(obj => obj.UsarMobile);
            result = result.Where(obj => obj.IdSuperApp != null && obj.IdSuperApp != string.Empty);

            return result.Select(o => o.IdSuperApp).ToList();
        }
        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorEntregaRealizada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.EntregaRealizada == true select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorMotivoChamado(int codigoMotivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>()
                .Where(obj => obj.MotivoChamado.Codigo == codigoMotivo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.OutroTomador)
                .Fetch(obj => obj.OutroEmitente)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Pessoa)
                .Fetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }
        public async Task<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result
                .Fetch(obj => obj.OutroTomador)
                .Fetch(obj => obj.OutroEmitente)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Pessoa)
                .Fetch(obj => obj.GrupoPessoas)
                .FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarOcorrenciasEntrega(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, bool usadoParaMotivoRejeicaoColetaEntrega, int codigoCargaEntrega, bool bloquearVisualizacaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.TipoOcorrenciaControleEntrega && obj.Ativo select obj;

            if (tipoAplicacaoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Todos)
                result = result.Where(obj => obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega);

            if (usadoParaMotivoRejeicaoColetaEntrega)
                result = result.Where(obj => obj.UsadoParaMotivoRejeicaoColetaEntrega);

            //filtra por ocorrências que não tem canal de entrega, ou que tenham o mesmo canal de entrega da entrega que está sendo lançada a ocorrência
            if (codigoCargaEntrega > 0)
            {
                var queryCargaEntregaPed = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

                queryCargaEntregaPed = queryCargaEntregaPed.Where(cargaEntregaPed => cargaEntregaPed.CargaEntrega.Codigo == codigoCargaEntrega);

                result = result.Where(tipoOcorrencia => queryCargaEntregaPed.Any(cargaEntregaPed => tipoOcorrencia.CanaisDeEntrega.Any(canalEntrega => canalEntrega.Codigo == cargaEntregaPed.CargaPedido.Pedido.CanalEntrega.Codigo)) || tipoOcorrencia.CanaisDeEntrega.Count == 0);
            }

            if (bloquearVisualizacaoTransportador)
                result = result.Where(obj => obj.BloquearVisualizacaoTipoOcorrenciaTransportador.Value == false || obj.BloquearVisualizacaoTipoOcorrenciaTransportador.HasValue == false);

            return result.ToList();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, bool usadoParaMotivoRejeicaoColetaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.TipoOcorrenciaControleEntrega && obj.UsarMobile && obj.Ativo select obj;

            if (tipoAplicacaoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Todos)
                result = result.Where(obj => obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega);

            if (usadoParaMotivoRejeicaoColetaEntrega)
                result = result.Where(obj => obj.UsadoParaMotivoRejeicaoColetaEntrega);

            return result.ToList();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarTodasOcorrenciasUltilizadasControleEntregaAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            query = query.Where(ocorrencia => ocorrencia.Ativo && ocorrencia.TipoOcorrenciaControleEntrega);

            return query.ToList();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> BuscarTipoOcorrenciaMobile()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.Ativo == true && obj.UsarMobile == true select obj;
            return result.ToList();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            return result.FirstOrDefault();
        }

        public bool ExisteBloqueioTransportadorPorComponente(int codigoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.ComponenteFrete.Codigo == codigoComponente && obj.BloquearVisualizacaoTipoOcorrenciaTransportador == true select obj;
            return result.Count() > 0;
        }

        public async Task<bool> ExisteBloqueioTransportadorPorComponenteAsync(int codigoComponente)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>()
                .Where(obj => obj.ComponenteFrete.Codigo == codigoComponente && obj.BloquearVisualizacaoTipoOcorrenciaTransportador == true).CountAsync(_cancellationToken) > 0;
        }

        public List<(int CodigoComponente, bool PossuiBloqueio)> ExisteBloqueioTransportadorPorComponentes(List<int> codigosComponentes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>()
                .Where(obj => codigosComponentes.Contains(obj.ComponenteFrete.Codigo));

            return query.Select(obj => ValueTuple.Create(
                    obj.Codigo,
                    obj.BloquearVisualizacaoTipoOcorrenciaTransportador ?? false
                )).ToList();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigoIntegracaoCliente(int codigoEmpresa, string codigoIntegracao, double? cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.OutroEmitente.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (cnpjCliente == null)
                result = result.Where(o => o.Pessoa == null);
            else
            if (cnpjCliente > 0)
                result = result.Where(o => o.Pessoa.CPF_CNPJ == cnpjCliente);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarPorCodigoIntegracaoDescricaoeCliente(int codigoEmpresa, string codigoIntegracao, string descricao, double? cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.OutroEmitente.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao == descricao);

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (cnpjCliente == null)
                result = result.Where(o => o.Pessoa == null);
            else
            if (cnpjCliente > 0)
                result = result.Where(o => o.Pessoa.CPF_CNPJ == cnpjCliente);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> Consultar(bool somentecomCodigoIntegracao, bool somenteOcorrenciaPorCarga, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query where obj.Ativo select obj;

            if (somentecomCodigoIntegracao)
                result = result.Where(obj => obj.CodigoIntegracao != null && obj.CodigoIntegracao != "");

            if (somenteOcorrenciaPorCarga)
                result = result.Where(obj => obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga);

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(bool somentecomCodigoIntegracao, bool somenteOcorrenciaPorCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query where obj.Ativo select obj;

            if (somentecomCodigoIntegracao)
                result = result.Where(obj => obj.CodigoIntegracao != null && obj.CodigoIntegracao != "");

            if (somenteOcorrenciaPorCarga)
                result = result.Where(obj => obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga);

            return result.Count();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> Consultar(int codigoEmpresa, string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.OutroEmitente.Codigo == codigoEmpresa);
            else
                result = result.Where(o => o.OutroEmitente == null);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.OrderBy(o => o.CodigoProceda).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.OutroEmitente.Codigo == codigoEmpresa);
            else
                result = result.Where(o => o.OutroEmitente == null);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarTipoOcorrenciaDiferencaValorFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            var result = from obj in query where obj.Ativo == true && obj.OcorrenciaDiferencaValorFechamento == true select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.TipoDeOcorrenciaDeCTe> _Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.AcessoTerceiro)
                query = query.Where(obj => obj.OcorrenciaTerceiros == true);


            if (filtrosPesquisa.TipoAplicacaoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Todos)
                query = query.Where(obj => obj.TipoAplicacaoColetaEntrega == filtrosPesquisa.TipoAplicacaoColetaEntrega);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas || o.GrupoPessoas == null);
            else if (filtrosPesquisa.CodigoGrupoPessoasIgual > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasIgual);
            else if (filtrosPesquisa.ValidarSomenteDisponiveisParaCarga)
                query = query.Where(o => o.GrupoPessoas == null);

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == filtrosPesquisa.CpfCnpjPessoa || o.Pessoa == null);
            else if (filtrosPesquisa.CpfCnpjPessoaIgual > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == filtrosPesquisa.CpfCnpjPessoa);
            else if (filtrosPesquisa.ValidarSomenteDisponiveisParaCarga)
                query = query.Where(o => o.Pessoa == null);

            if (filtrosPesquisa.Finalidades.Count > 0)
            {
                var finalidadesLimpas = (from f in filtrosPesquisa.Finalidades where f != null select f).ToList();
                query = query.Where(o => filtrosPesquisa.Finalidades.Contains(null) ? o.FinalidadeTipoOcorrencia == null || finalidadesLimpas.Contains(o.FinalidadeTipoOcorrencia) : filtrosPesquisa.Finalidades.Contains(o.FinalidadeTipoOcorrencia));
            }

            if (filtrosPesquisa.OrigemOcorrencia.HasValue)
                query = query.Where(o => o.OrigemOcorrencia == filtrosPesquisa.OrigemOcorrencia.Value);

            if (filtrosPesquisa.FiltrarContratoFrete)
            {
                DateTime vigencia = DateTime.Now.Date;
                var queryContrato = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
                var resultContrato = from obj in queryContrato
                                     where
                                        obj.DataInicial.Date <= vigencia
                                        && obj.DataFinal.Date >= vigencia
                                        && obj.Ativo
                                     select obj;

                if (filtrosPesquisa.Transportador > 0)
                    resultContrato = resultContrato.Where(o => o.Transportador.Codigo == filtrosPesquisa.Transportador);

                var listaContratoCodigos = resultContrato.SelectMany(o => o.TiposOcorrencia.Select(s => s.Codigo)).ToList();

                query = query.Where(o => o.OrigemOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato ||
                                        (o.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato && listaContratoCodigos.Contains(o.Codigo)));

            }

            if (filtrosPesquisa.TipoDocumentoCreditoDebito.HasValue)
                query = query.Where(o => o.ModeloDocumentoFiscal == null || o.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == filtrosPesquisa.TipoDocumentoCreditoDebito.Value);

            if (filtrosPesquisa.CodigosTipoOperacaoColeta?.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosTipoOperacaoColeta.Contains(o.TipoOperacaoColeta.Codigo));

            if (filtrosPesquisa.CodigosMotivoChamado?.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosMotivoChamado.Contains(o.MotivoChamado.Codigo));

            if (!filtrosPesquisa.NaoUtilizarFlagsControleEntrega)
            {
                if (filtrosPesquisa.SomenteOcorrenciaUtilizadaControleEntrega || filtrosPesquisa.TipoOcorrenciaControleEntrega)
                    query = query.Where(obj => obj.TipoOcorrenciaControleEntrega == true);
                else if (filtrosPesquisa.SomenteOcorrenciasQueNaoUtilizamControleEntrega)
                    query = query.Where(obj => ((bool?)obj.TipoOcorrenciaControleEntrega ?? false) == false);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                var queryTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                    .Where(o => o.Codigo == filtrosPesquisa.CodigoTipoOperacao);
                query = query.Where(o => queryTipoOperacao.Any(tp => !tp.TiposOcorrencia.Any() || tp.TiposOcorrencia.Any(to => to.Codigo == o.Codigo)));
            }

            if (!filtrosPesquisa.UsuarioAdministrador)
            {
                if (filtrosPesquisa.CodigoPerfilAcesso > 0)
                {
                    var consultaTipoOcorrenciaPerfilAcesso = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso>()
                        .Where(obj => obj.PerfilAcesso.Codigo == filtrosPesquisa.CodigoPerfilAcesso);

                    var consultaTodosTipoOcorrenciaPerfilAcesso = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso>();

                    List<int> codigosOcorrenciasPerfilAcesso = consultaTodosTipoOcorrenciaPerfilAcesso.Select(obj => obj.TipoDeOcorrenciaDeCTe.Codigo).ToList();

                    List<int> codigosOcorrenciasComPerfilDeAcesso = consultaTipoOcorrenciaPerfilAcesso.Select(x => x.TipoDeOcorrenciaDeCTe.Codigo).ToList();

                    query = query.Where(obj => !codigosOcorrenciasPerfilAcesso.Contains(obj.Codigo) || codigosOcorrenciasComPerfilDeAcesso.Contains(obj.Codigo));
                }
            }

            if (filtrosPesquisa.BloquearVisualizacaoTransportador)
                query = query.Where(obj => obj.BloquearVisualizacaoTipoOcorrenciaTransportador.Value == false || obj.BloquearVisualizacaoTipoOcorrenciaTransportador.HasValue == false);

            if (filtrosPesquisa.NaoPermitirQueTransportadorSelecioneTipoOcorrencia)
                query = query.Where(obj => ((bool?)obj.NaoPermitirQueTransportadorSelecioneTipoOcorrencia ?? false) == false);

            if (filtrosPesquisa.FiltrarApenasOcorrenciasPermitidasNoPortalDoCliente)
                query = query.Where(obj => obj.PermitirSelecionarEssaOcorrenciaNoPortalDoCliente);

            return query;
        }

        #endregion

        #region Relatório de Tipos de Ocorrências

        public int ContarConsultaRelatorioTipoOcorrencia(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtroPesquisa)
        {
            string sql = ObterSelectConsultaRelatorioTipoOcorrencia(filtroPesquisa, true, agrupamentos, null);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia.TipoOcorrencia> ConsultarRelatorioTipoOcorrencia(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {  
            string sql = ObterSelectConsultaRelatorioTipoOcorrencia(filtroPesquisa, false, agrupamentos, parametrosConsulta);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia.TipoOcorrencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia.TipoOcorrencia>();
        }

        private string ObterSelectConsultaRelatorioTipoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtroPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty,
                   query = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioTipoOcorrencia(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);
            SetarWhereRelatorioTipoOcorrencia(ref where, ref groupBy, ref joins, filtroPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                {
                    SetarSelectRelatorioTipoOcorrencia(parametrosConsulta.PropriedadeAgrupar, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(parametrosConsulta.PropriedadeAgrupar))
                        orderBy = parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                {
                    if (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta.PropriedadeAgrupar && select.Contains(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeOrdenar != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (count)
                query += "SELECT DISTINCT(COUNT(0) OVER ())";
            else
                query += "SELECT " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty);

            query += " FROM T_OCORRENCIA TipoOcorrencia " + joins + " WHERE 1 = 1" + where;

            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            if (!count)
            {
                if (orderBy.Length > 0)
                    query += " ORDER BY " + orderBy;
                else
                    query += " ORDER BY 1 ASC ";

                if (parametrosConsulta.LimiteRegistros > 0)
                    query += " OFFSET " + parametrosConsulta.InicioRegistros.ToString() + " ROWS FETCH NEXT " + parametrosConsulta.LimiteRegistros.ToString() + " ROWS ONLY";
            }

            return query;
        }

        private void SetarSelectRelatorioTipoOcorrencia(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains("Codigo,"))
                    {
                        select += "TipoOcorrencia.OCO_CODIGO Codigo, ";
                        groupBy += "TipoOcorrencia.OCO_CODIGO, ";
                    }
                    break;
                case "CodigoIntegracao":
                    if (!select.Contains("CodigoIntegracao,"))
                    {
                        select += "TipoOcorrencia.OCO_CODIGO_INTEGRACAO CodigoIntegracao, ";
                        groupBy += "TipoOcorrencia.OCO_CODIGO_INTEGRACAO, ";
                    }
                    break;
                case "Descricao":
                    if (!select.Contains("Descricao,"))
                    {
                        select += "TipoOcorrencia.OCO_DESCRICAO Descricao, ";
                        groupBy += "TipoOcorrencia.OCO_DESCRICAO, ";
                    }
                    break;
                case "CodigoProceda":
                    if (!select.Contains("CodigoProceda,"))
                    {
                        select += "TipoOcorrencia.OCO_COD_PROCEDA CodigoProceda, ";
                        groupBy += "TipoOcorrencia.OCO_COD_PROCEDA, ";
                    }
                    break;
                case "Tipo":
                    if (!select.Contains("Tipo,"))
                    {
                        select += "CASE TipoOcorrencia.OCO_TIPO WHEN 'F' THEN 'Final' ELSE 'Pendente' END Tipo, ";
                        groupBy += "TipoOcorrencia.OCO_TIPO, ";
                    }
                    break;
                case "Situacao":
                    if (!select.Contains("Situacao,"))
                    {
                        select += "CASE TipoOcorrencia.OCO_ATIVO WHEN 1 THEN 'Ativo' ELSE 'Inativo' END Situacao, ";
                        groupBy += "TipoOcorrencia.OCO_ATIVO, ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas,"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoPessoas "))
                            joins += "left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = TipoOcorrencia.GRP_CODIGO ";
                    }
                    break;
                case "NomePessoa":
                    if (!select.Contains("NomePessoa, "))
                    {
                        select += "Pessoa.CLI_NOME NomePessoa, ";
                        groupBy += "Pessoa.CLI_NOME, ";

                        if (!joins.Contains(" Pessoa "))
                            joins += "left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = TipoOcorrencia.CLI_CGCCPF ";
                    }
                    break;
                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains("CPFCNPJPessoa, "))
                    {
                        select += "Pessoa.CLI_CGCCPF CPFCNPJPessoa, Pessoa.CLI_FISJUR TipoPessoa, ";
                        groupBy += "Pessoa.CLI_CGCCPF, Pessoa.CLI_FISJUR, ";

                        if (!joins.Contains(" Pessoa "))
                            joins += "left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = TipoOcorrencia.CLI_CGCCPF ";
                    }
                    break;
                case "ComponenteFrete":
                    if (!select.Contains("ComponenteFrete, "))
                    {
                        select += "ComponenteFrete.CFR_DESCRICAO ComponenteFrete, ";
                        groupBy += "ComponenteFrete.CFR_DESCRICAO, ";

                        if (!joins.Contains(" ComponenteFrete "))
                            joins += "left join T_COMPONENTE_FRETE ComponenteFrete on ComponenteFrete.CFR_CODIGO = TipoOcorrencia.CFR_CODIGO ";
                    }
                    break;

                case "GrupoOcorrencia":
                    if (!select.Contains("GrupoOcorrencia, "))
                    {
                        select += "GrupoOcorrencia.GTO_DESCRICAO GrupoOcorrencia, ";
                        groupBy += "GrupoOcorrencia.GTO_DESCRICAO, ";

                        if (!joins.Contains(" GrupoOcorrencia"))
                            joins += "left join T_GRUPO_TIPO_OCORRENCIA GrupoOcorrencia on GrupoOcorrencia.GTO_CODIGO = TipoOcorrencia.GTO_CODIGO ";
                    }
                    break;

                case "DescricaoAuxiliar":
                    if (!select.Contains("DescricaoAuxiliar, "))
                    {
                        select += "TipoOcorrencia.OCO_DESCRICAO_AUXILIAR DescricaoAuxiliar, ";
                        groupBy += "TipoOcorrencia.OCO_DESCRICAO_AUXILIAR, ";
                    }
                    break;

                case "CodigoIntegracaoAuxiliar":
                    if (!select.Contains("CodigoIntegracaoAuxiliar, "))
                    {
                        select += "TipoOcorrencia.OCO_CODIGO_INTEGRACAO_AUXILIAR CodigoIntegracaoAuxiliar, ";
                        groupBy += "TipoOcorrencia.OCO_CODIGO_INTEGRACAO_AUXILIAR, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioTipoOcorrencia(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtroPesquisa)
        {
            if (!string.IsNullOrWhiteSpace(filtroPesquisa.Descricao))
                where += " AND TipoOcorrencia.OCO_DESCRICAO LIKE '%" + filtroPesquisa.Descricao + "%'";

            if (filtroPesquisa.Situacao.HasValue)
                where += " AND TipoOcorrencia.OCO_ATIVO = " + (filtroPesquisa.Situacao.Value ? "1" : "0");

            if (filtroPesquisa.CpfCnpjPessoa > 0d)
                where += " AND TipoOcorrencia.CLI_CGCCPF = " + filtroPesquisa.CpfCnpjPessoa.ToString();

            if (filtroPesquisa.CodigoGrupoPessoas > 0)
                where += " AND TipoOcorrencia.GRP_CODIGO = " + filtroPesquisa.CodigoGrupoPessoas.ToString();

            if (filtroPesquisa.AcessoTerceiro)
                where += " AND TipoOcorrencia.OcorrenciaTerceiros = 1";

            if (filtroPesquisa.CodigoFilial.Any(codigo => codigo == -1))
            {
                if (!joins.Contains(" CargaOcorrencia "))
                {
                    joins += "left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.oco_codigo = TipoOcorrencia.oco_codigo ";
                }

                if (!joins.Contains(" Carga "))
                {
                    joins += "left join T_CARGA Carga on Carga.CAR_CODIGO = CargaOcorrencia.CAR_CODIGO ";
                }

                where += $@"AND (Carga.FIL_CODIGO IN ({string.Join(", ", filtroPesquisa.CodigoFilial)}) OR EXISTS ( SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                            FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                            LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                            WHERE carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                            AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtroPesquisa.CodigoRecebedor)})))";
            }
        }

        #endregion
    }
}
