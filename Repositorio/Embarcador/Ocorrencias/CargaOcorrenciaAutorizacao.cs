using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>
    {
        #region Construtores

        public CargaOcorrenciaAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaOcorrenciaAutorizacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados
        private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarObjetoParcialPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var ocorrencias = query.Where(x => codigos.Contains(x.Codigo)).Select(obj => new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
            {
                Codigo = obj.Codigo,
                DestinatariosCTes = obj.DestinatariosCTes == null ? string.Empty : obj.DestinatariosCTes,
                Carga = (obj.Carga == null) ? new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = 0 } : new Dominio.Entidades.Embarcador.Cargas.Carga()
                {
                    Codigo = obj.Carga.Codigo,
                    Veiculo = new Dominio.Entidades.Veiculo
                    {
                        Codigo = obj.Carga.Veiculo == null ? 0 : obj.Carga.Veiculo.Codigo,
                        TipoVeiculo = obj.Carga.Veiculo == null ? string.Empty : obj.Carga.Veiculo.TipoVeiculo
                    },
                    TipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao
                    {
                        Codigo = obj.Carga.TipoOperacao == null ? 0 : obj.Carga.TipoOperacao.Codigo,
                        Descricao = obj.Carga.TipoOperacao == null ? string.Empty : obj.Carga.TipoOperacao.Descricao
                    },
                    ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga
                    {
                        Codigo = obj.Carga.ModeloVeicularCarga == null ? 0 : obj.Carga.ModeloVeicularCarga.Codigo,
                        Descricao = obj.Carga.ModeloVeicularCarga == null ? string.Empty : obj.Carga.ModeloVeicularCarga.Descricao,
                    },
                    Filial = new Dominio.Entidades.Embarcador.Filiais.Filial
                    {
                        Codigo = obj.Carga.Filial == null ? 0 : obj.Carga.Filial.Codigo,
                        Descricao = obj.Carga.Filial == null ? string.Empty : obj.Carga.Filial.Descricao
                    },
                    DadosSumarizados = new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados
                    {
                        Destinos = obj.Carga.DadosSumarizados == null ? string.Empty : obj.Carga.DadosSumarizados.Destinos,
                        Motoristas = obj.Carga.DadosSumarizados == null ? string.Empty : obj.Carga.DadosSumarizados.Motoristas,
                        Veiculos = obj.Carga.DadosSumarizados == null ? string.Empty : obj.Carga.DadosSumarizados.Veiculos
                    }
                }
            }).ToList();

            string sql = $@" select CAR_CODIGO CodigoCarga, CAR_CODIGO_CARGA_AGRUPADO CodigoCargaAgrupada from T_CARGA_CODIGOS_AGRUPADOS ";
            if (ocorrencias?.Count > 0)
                sql += $@"WHERE CAR_CODIGO in ({string.Join(",", ocorrencias.Select(x => x.Carga.Codigo).ToList())})";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            var CodigosAgrupados = consulta.List<object[]>().Select(row =>
            {
                int codigoCarga = (int)row[0];
                string codigoCargaAgrupada = (string)row[1];
                return (codigoCarga, codigoCargaAgrupada);
            }).ToList();

            foreach (var ocorrencia in ocorrencias)
            {
                ocorrencia.Carga.CodigosAgrupados = new List<string>();
                foreach (var carga in CodigosAgrupados.Where(x => x.Item1 == ocorrencia.Carga.Codigo))
                    ocorrencia.Carga.CodigosAgrupados.Add(carga.Item2);
            }

            return ocorrencias;
        }
        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaAutorizacaoOcorrencia filtrosPesquisa)
        {
            var consultaCargaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o => !o.Bloqueada);

            consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => !o.Inativa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.NumeroCarga) || obj.Carga.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.NumeroOcorrencia > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.NumeroOcorrencia == filtrosPesquisa.NumeroOcorrencia);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.DataOcorrencia.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.DataOcorrencia.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.SituacaoOcorrencia != SituacaoOcorrencia.Todas && filtrosPesquisa.SituacaoOcorrencia != SituacaoOcorrencia.AutorizacaoPendente)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.SituacaoOcorrencia == filtrosPesquisa.SituacaoOcorrencia);
            else if (filtrosPesquisa.SituacaoOcorrencia == SituacaoOcorrencia.AutorizacaoPendente)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao ||
                o.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao);

            if (filtrosPesquisa.CodigosTipoOcorrencia != null && filtrosPesquisa.CodigosTipoOcorrencia.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => filtrosPesquisa.CodigosTipoOcorrencia.Contains(obj.TipoOcorrencia.Codigo));

            if (filtrosPesquisa.CodigosTransportador != null && filtrosPesquisa.CodigosTransportador.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => filtrosPesquisa.CodigosTransportador.Contains(obj.Carga.Empresa.Codigo));

            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Carga.Filial.Codigo)
                    || obj.Carga.Pedidos.Any(ped => ped.Recebedor != null && filtrosPesquisa.CodigosRecebedor.Contains(ped.Recebedor.CPF_CNPJ)));
            else if (filtrosPesquisa.CodigosFilial != null && filtrosPesquisa.CodigosFilial.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigosDestino != null && filtrosPesquisa.CodigosDestino.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosDestino.Contains(p.Pedido.Destino.Codigo)));

            if (filtrosPesquisa.TipoDocumentoCreditoDebito != TipoDocumentoCreditoDebito.Todos)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == filtrosPesquisa.TipoDocumentoCreditoDebito);

            if (filtrosPesquisa.CodigosUsuario != null && filtrosPesquisa.CodigosUsuario.Count > 0)
            {
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.ResponsavelAutorizacao == null || filtrosPesquisa.CodigosUsuario.Contains(obj.ResponsavelAutorizacao.Codigo));
                consultaCargaOcorrenciaAutorizacao = consultaCargaOcorrenciaAutorizacao.Where(obj => filtrosPesquisa.CodigosUsuario.Contains(obj.Usuario.Codigo));
            }

            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.CodigoPedidoCliente == filtrosPesquisa.NumeroPedidoCliente));

            if (filtrosPesquisa.CodigosClienteComplementar.Any())
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosClienteComplementar.Contains((int)p.Pedido.Destinatario.Codigo)));

            if (filtrosPesquisa.CodigosVendedor.Any())
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosVendedor.Contains(p.Pedido.FuncionarioVendedor.Codigo)));

            if (filtrosPesquisa.CodigosSupervisor.Any())
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosSupervisor.Contains(p.Pedido.FuncionarioSupervisor.Codigo)));

            if (filtrosPesquisa.CodigosGerente.Any())
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosGerente.Contains(p.Pedido.FuncionarioGerente.Codigo)));

            if (filtrosPesquisa.CodigosUFDestino.Any())
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosUFDestino.Contains(p.Pedido.Destino.Estado.Sigla)));

            if (filtrosPesquisa.NumeroNF > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(obj => obj.Carga.Pedidos.Any(ped => ped.NotasFiscais.Any(nf => nf.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNF) || ped.CargaPedidoXMLNotasFiscaisParcial.Any(pa => pa.Numero == filtrosPesquisa.NumeroNF)));

            bool situacaoPendentes = filtrosPesquisa.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao ||
                filtrosPesquisa.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao ||
                filtrosPesquisa.SituacaoOcorrencia == SituacaoOcorrencia.AutorizacaoPendente;

            if (situacaoPendentes)
            {
                SituacaoOcorrenciaAutorizacao situacaoAutorizacaoPendente = SituacaoOcorrenciaAutorizacao.Pendente;
                consultaCargaOcorrenciaAutorizacao = consultaCargaOcorrenciaAutorizacao.Where(o => o.Situacao == situacaoAutorizacaoPendente);

                if (filtrosPesquisa.EtapaAutorizacao.HasValue)
                    consultaCargaOcorrenciaAutorizacao = consultaCargaOcorrenciaAutorizacao.Where(o => o.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == filtrosPesquisa.EtapaAutorizacao.Value && o.Situacao == SituacaoOcorrenciaAutorizacao.Pendente);
            }
            else if (filtrosPesquisa.EtapaAutorizacao.HasValue)
                consultaCargaOcorrenciaAutorizacao = consultaCargaOcorrenciaAutorizacao.Where(o => o.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == filtrosPesquisa.EtapaAutorizacao.Value);

            if (filtrosPesquisa.PrioridadesAprovacao?.Count > 0)
                return consultaCargaOcorrencia.Where(ocorrencia =>
                    consultaCargaOcorrenciaAutorizacao.Where(aprovacao =>
                        aprovacao.CargaOcorrencia.Codigo == ocorrencia.Codigo &&
                        (
                            (
                                aprovacao.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia &&
                                (aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao &&
                                filtrosPesquisa.PrioridadesAprovacao.Contains(ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao)
                            ) ||
                            (
                                aprovacao.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.EmissaoOcorrencia &&
                                (aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao &&
                                filtrosPesquisa.PrioridadesAprovacao.Contains(ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao)
                            )
                        )
                    ).Any()
                );

            return consultaCargaOcorrencia.Where(ocorrencia =>
                consultaCargaOcorrenciaAutorizacao.Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == ocorrencia.Codigo &&
                    (
                        (aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                        (
                            aprovacao.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia &&
                            (aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao
                        ) ||
                        (
                            aprovacao.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.EmissaoOcorrencia &&
                            (aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao
                        )
                    )
                ).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao BuscarPorCodigoComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o => o.Codigo == codigo);

            return query
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.Carga)
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.Emitente)
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.TipoOcorrencia).ThenFetch(o => o.JustificativaPadraoAprovacao)
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.Usuario)
                .Fetch(o => o.RegrasAutorizacaoOcorrencia)
                .Fetch(o => o.Usuario)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarPendentesPorOcorrenciaEUsuario(int codigo, int usuario)
        {
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o =>
                    o.CargaOcorrencia.Codigo == codigo &&
                    !o.Bloqueada &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente &&
                    o.Usuario.Codigo == usuario &&
                    (o.CargaOcorrencia.ResponsavelAutorizacao == null || o.CargaOcorrencia.ResponsavelAutorizacao.Codigo == usuario)
                );

            return consultaCargaOcorrenciaAutorizacao.ToList();
        }

        public List<int> BuscarPendentesAprovacaoAutomatica(DateTime dataBase, int inicioRegistros, int maximoRegistros)
        {
            var situacoesOcorrencias = new List<SituacaoOcorrencia>
            {
                SituacaoOcorrencia.AgAprovacao,
                SituacaoOcorrencia.AgAutorizacaoEmissao
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(ocorrencia => ocorrencia.Situacao == SituacaoOcorrenciaAutorizacao.Pendente
                            && ocorrencia.DataPrazoAprovacaoAutomatica != null
                            && ocorrencia.DataPrazoAprovacaoAutomatica <= dataBase
                            && situacoesOcorrencias.Contains(ocorrencia.CargaOcorrencia.SituacaoOcorrencia))
                .Select(ocorrencia => ocorrencia.Codigo);

            return query
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .WithOptions(o => o.SetReadOnly(true))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarPendentesPorOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query
                         where
                             obj.CargaOcorrencia.Codigo == codigoOcorrencia
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                             && obj.EtapaAutorizacaoOcorrencia == etapa
                             && obj.Bloqueada
                         select obj;

            return result
                .Fetch(o => o.RegrasAutorizacaoOcorrencia).ThenFetchMany(o => o.Aprovadores)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarPendentesPorOcorrenciaUsuarioEtapa(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o =>
                    o.CargaOcorrencia.Codigo == codigo &&
                    !o.Bloqueada &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente &&
                    o.Usuario.Codigo == usuario &&
                    o.EtapaAutorizacaoOcorrencia == etapa &&
                    (o.CargaOcorrencia.ResponsavelAutorizacao == null || o.CargaOcorrencia.ResponsavelAutorizacao.Codigo == usuario)
                );

            return consultaCargaOcorrenciaAutorizacao.Fetch(x => x.CargaOcorrencia).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarPorOcorrenciaUsuarioEtapa(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigo && !obj.Bloqueada select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            if (etapa.HasValue)
                result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapa.Value);

            return result
                .Fetch(obj => obj.MotivoRejeicaoOcorrencia)
                .Fetch(obj => obj.Usuario)
                .Fetch(obj => obj.CargaOcorrencia)
                .ThenFetch(obj => obj.ResponsavelAutorizacao)
                .Fetch(obj => obj.RegrasAutorizacaoOcorrencia)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao BuscarUltimoAprovadorOcorrencia(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query
                         where
                         obj.CargaOcorrencia.Codigo == codigo
                         && obj.EtapaAutorizacaoOcorrencia == etapa
                         && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                         && obj.Usuario != null
                         orderby obj.Data descending
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarAprovadoresOcorrencia(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query
                         where
                         obj.CargaOcorrencia.Codigo == codigo
                         && obj.EtapaAutorizacaoOcorrencia == etapa
                         && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                         && obj.Usuario != null
                         orderby obj.Data descending
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarAprovadoresOcorrenciaSemEtapa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query
                         where
                         obj.CargaOcorrencia.Codigo == codigo
                         && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                         && obj.Usuario != null
                         orderby obj.Data descending
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarDesbloqueadasNaoDelegadasPorCargaOcorrencia(int codigoCargaOcorrencia)
        {
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o =>
                    o.CargaOcorrencia.Codigo == codigoCargaOcorrencia &&
                    !o.Bloqueada &&
                    o.RegrasAutorizacaoOcorrencia != null &&
                    o.Usuario != null
                );

            return consultaCargaOcorrenciaAutorizacao
                .Fetch(obj => obj.CargaOcorrencia)
                .Fetch(obj => obj.RegrasAutorizacaoOcorrencia)
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarDesbloqueadasNaoDelegadasPorCargasOcorrenciasPaginado(List<int> codigosCargaOcorrencia)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> result = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();

            int take = 2000;
            int start = 0;
            while (start < codigosCargaOcorrencia?.Count)
            {
                List<int> tmp = codigosCargaOcorrencia.Skip(start).Take(take).ToList();

                result.AddRange(query
                    .Where(o =>
                        tmp.Contains(o.CargaOcorrencia.Codigo) &&
                        !o.Bloqueada &&
                        o.RegrasAutorizacaoOcorrencia != null &&
                        o.Usuario != null
                    )
                .Fetch(obj => obj.CargaOcorrencia)
                .Fetch(obj => obj.RegrasAutorizacaoOcorrencia)
                .Fetch(obj => obj.Usuario)
                .ToList());

                start += take;
            }

            return result;
        }

        /// <summary>
        /// Retorna regras distintas da ocorrencia da carga
        /// </summary>
        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegrasOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();

            var resultGroup = from obj in queryGroup where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj.RegrasAutorizacaoOcorrencia;
            var result = from obj in query where resultGroup.Contains(obj) && obj.EtapaAutorizacaoOcorrencia == etapa select obj;

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoOcorrencia, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Rejeitada &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa
                );

            return consultaAutorizacao.Count();
        }

        public int ContarRejeitadas(int codigoOcorrencia, int codigoRegra, int prioridadeAprovacao, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Rejeitada &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa && (
                        (aprovacao.RegrasAutorizacaoOcorrencia.Codigo == codigoRegra) ||
                        (aprovacao.RegrasAutorizacaoOcorrencia == null)
                    ) && (
                        (aprovacao.PrioridadeAprovacao == null && aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                        ((aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == prioridadeAprovacao)
                    )
                );

            return consultaAutorizacao.Count();
        }

        public int ContarPendentes(int codigoOcorrencia, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Pendente &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa
                );

            return consultaAutorizacao.Count();
        }

        public int ContarPendentes(int codigoOcorrencia, int codigoRegra, int prioridadeAprovacao, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Pendente &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa && (
                        (aprovacao.RegrasAutorizacaoOcorrencia.Codigo == codigoRegra) ||
                        (aprovacao.RegrasAutorizacaoOcorrencia == null)
                    ) && (
                        (aprovacao.PrioridadeAprovacao == null && aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                        ((aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == prioridadeAprovacao)
                    )
                );

            return consultaAutorizacao.Count();
        }

        public int ContarAprovacoesOcorrencia(int codigoOcorrencia, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa
                );

            return consultaAutorizacao.Count();
        }

        public int ContarAprovacoesOcorrencia(int codigoOcorrencia, int codigoRegra, int prioridadeAprovacao, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa && (
                        (aprovacao.RegrasAutorizacaoOcorrencia.Codigo == codigoRegra) ||
                        (aprovacao.RegrasAutorizacaoOcorrencia == null)
                    ) && (
                        (aprovacao.PrioridadeAprovacao == null && aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                        ((aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == prioridadeAprovacao)
                    )
                );

            return consultaAutorizacao.Count();
        }

        public int BuscarNumeroAprovacoesNecessarias(int codigoOcorrencia, int codigoRegra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa &&
                    aprovacao.RegrasAutorizacaoOcorrencia.Codigo == codigoRegra
                );

            int numeroAprovacoesNecessarias = aprovacoes
                .Select(aprovacao => aprovacao.NumeroAprovadores)
                .FirstOrDefault();

            return numeroAprovacoesNecessarias;
        }

        public int BuscarNumeroReprovacoesNecessarias(int codigoOcorrencia, int codigoRegra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    aprovacao.EtapaAutorizacaoOcorrencia == etapa &&
                    aprovacao.RegrasAutorizacaoOcorrencia.Codigo == codigoRegra
                );

            int numeroReprovacoesNecessarias = aprovacoes
                .Select(aprovacao => aprovacao.NumeroReprovadores)
                .FirstOrDefault();

            return numeroReprovacoesNecessarias;
        }

        public bool VerificarExistemAprovacoesPendentes(int codigoOcorrencia, int codigoUsuario, EtapaAutorizacaoOcorrencia etapa)
        {
            var consultaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(aprovacao =>
                    aprovacao.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    !aprovacao.Bloqueada &&
                    aprovacao.Situacao == SituacaoOcorrenciaAutorizacao.Pendente &&
                    aprovacao.Usuario.Codigo == codigoUsuario
                );

            var consultaAutorizacaoFiltrada = FiltrarPorEtapaAutorizacaoOcorrencia(consultaAutorizacao, etapa);
            return consultaAutorizacaoFiltrada.Any();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> FiltrarPorEtapaAutorizacaoOcorrencia(IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> consultaAutorizacao, EtapaAutorizacaoOcorrencia etapa)
        {
            switch (etapa)
            {
                case EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia:
                    return consultaAutorizacao.Where(aprovacao =>
                      (aprovacao.PrioridadeAprovacao == null && aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                      ((aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == aprovacao.CargaOcorrencia.PrioridadeAprovacaoAtualEtapaAprovacao)
                  );
                case EtapaAutorizacaoOcorrencia.EmissaoOcorrencia:
                    return consultaAutorizacao.Where(aprovacao =>
                  (aprovacao.PrioridadeAprovacao == null && aprovacao.RegrasAutorizacaoOcorrencia == null) ||
                  ((aprovacao.PrioridadeAprovacao ?? aprovacao.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao) == aprovacao.CargaOcorrencia.PrioridadeAprovacaoAtualEtapaEmissao)
              );
                default:
                    return consultaAutorizacao;
            }
        }

        public bool VerificarSePodeAprovar(int codigoOcorrencia, int codigoRegra, int codigoUsuario)
        {
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o =>
                    o.Codigo == codigoRegra &&
                    o.CargaOcorrencia.Codigo == codigoOcorrencia &&
                    !o.Bloqueada &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente &&
                    o.Usuario.Codigo == codigoUsuario &&
                    (o.CargaOcorrencia.ResponsavelAutorizacao == null || o.CargaOcorrencia.ResponsavelAutorizacao.Codigo == codigoUsuario)
                );

            return consultaCargaOcorrenciaAutorizacao.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ConsultarAutorizacoesPorOcorrenciaEEtapa(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.EtapaAutorizacaoOcorrencia == etapa select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Fetch(obj => obj.RegrasAutorizacaoOcorrencia)
                .Fetch(obj => obj.Usuario)
                .Fetch(obj => obj.MotivoRejeicaoOcorrencia)
                .Skip(inicioRegistros)
                .Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            if (etapa != null)
                result = result.Where(obj => obj.EtapaAutorizacaoOcorrencia == etapa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarObjetoParcialPorCodigosPaginado(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> result = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int take = 2000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                result.AddRange(BuscarObjetoParcialPorCodigos(tmp));

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCodigosFetchPaginado(List<int> codigos, string propOrdenacao, string dirOrdenacao)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> result = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int take = 2000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                result.AddRange(BuscarPorCodigosFetch(tmp));

                start += take;
            }

            var queryable = result.AsQueryable();

            queryable = queryable
                .OrderByDescending(o => o.DataPrazoAprovacao)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return queryable.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCodigosFetch(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>().Where(x => codigos.Contains(x.Codigo));

            return query
                .Fetch(obj => obj.Usuario)
                .ThenFetch(obj => obj.ClienteTerceiro)
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.ContratoFrete)
                .ThenFetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }
        public List<int> BuscarCodigosOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaAutorizacaoOcorrencia filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            result = result
                .OrderByDescending(o => o.DataPrazoAprovacao)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Select(obj => obj.Codigo).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaAutorizacaoOcorrencia filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ResponsavelOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.Usuario != null select obj;

            return result.Fetch(obj => obj.Usuario).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ResponsavelOcorrenciasPaginado(List<int> ocorrencias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> result = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();

            int take = 2000;
            int start = 0;
            while (start < ocorrencias?.Count)
            {
                List<int> tmp = ocorrencias.Skip(start).Take(take).ToList();

                result.AddRange((from obj in query where tmp.Contains(obj.CargaOcorrencia.Codigo) && obj.Usuario != null select obj).Fetch(obj => obj.Usuario).ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarTodosPorOcorrenciaEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query
                         where
                            obj.CargaOcorrencia.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario
                         select obj;
            return result.ToList();
        }

        public void DeletarPorOcorrencia(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete CargaOcorrenciaAutorizacao aprovacao where aprovacao.CargaOcorrencia.Codigo = :codigo ")
                        .SetInt32("codigo", codigo)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete CargaOcorrenciaAutorizacao aprovacao where aprovacao.CargaOcorrencia.Codigo = :codigo ")
                            .SetInt32("codigo", codigo)
                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public bool VerificarSeUsuarioEstaNaRegraOcorrencia(int codigoOcorrencia, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.Usuario.Codigo == codigoUsuario && !obj.Bloqueada select obj;

            return result.Any();
        }

        public Task<bool> VerificarSeUsuarioEstaNaRegraOcorrenciaAsync(int codigoOcorrencia, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.Usuario.Codigo == codigoUsuario && !obj.Bloqueada select obj;

            return result.AnyAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao BuscarPorGuid(string guid)
        {
            var consultaCargaOcorrenciaAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>()
                .Where(o => o.GuidOcorrencia == guid);

            return consultaCargaOcorrenciaAutorizacao.FirstOrDefault();
        }

        public bool ExistePorMotivoRejeicaoSemPermissaoETipoOcorrenciaECarga(int codigoTipoOcorrencia, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();
            return query.Where(x => x.CargaOcorrencia.TipoOcorrencia.Codigo == codigoTipoOcorrencia &&
                                    x.CargaOcorrencia.Carga.Codigo == codigoCarga &&
                                    x.MotivoRejeicaoOcorrencia.Tipo == AprovacaoRejeicao.Rejeicao && x.MotivoRejeicaoOcorrencia.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao).Any();
        }

        public void AtualizarCargaOcorrenciaAprovada(int codigoCargaOcorrenciaAutorizacao, string motivo, int codigoMotivoRejeicao, int codigoCentroResultado)
        {
            motivo = string.IsNullOrEmpty(motivo) ? "null" : motivo;
            string codigoMotivoAtualizar = codigoMotivoRejeicao > 0 ? codigoMotivoRejeicao.ToString() : "null";
            string centroResultadoAtualizar = codigoCentroResultado > 0 ? codigoCentroResultado.ToString() : "null";
            string sql = $@"UPDATE T_CARGA_OCORRENCIA_AUTORIZACAO
                            SET COA_DATA = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',
                                COA_SITUACAO = {(int)SituacaoOcorrenciaAutorizacao.Aprovada}, 
                                COA_MOTIVO = '{motivo}', 
                                MRO_CODIGO = {codigoMotivoAtualizar}, 
                                CRE_CODIGO = {centroResultadoAtualizar}
                                WHERE COA_CODIGO = {codigoCargaOcorrenciaAutorizacao}";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.ExecuteUpdate();
        }

        #endregion
    }
}
