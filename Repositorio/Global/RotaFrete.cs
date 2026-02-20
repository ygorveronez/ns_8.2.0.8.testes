using Dominio.Entidades.Embarcador.Pessoas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using LinqKit;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class RotaFrete : RepositorioBase<Dominio.Entidades.RotaFrete>, Dominio.Interfaces.Repositorios.RotaFrete
    {
        public RotaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RotaFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.RotaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.RotaFrete> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.RotaFrete BuscarParaProcessamento(double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query
                         where
                         obj.Remetente.CPF_CNPJ == remetente
                         && obj.Destinatarios.Any(d => d.Cliente.CPF_CNPJ == destinatario)
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.RotaFrete>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.RotaFrete>();

            List<int> codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.RotaFrete> resultado = new List<Dominio.Entidades.RotaFrete>();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                    .Where(r => bloco.Contains(r.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorLocalidades(List<int> codigosLocalidades)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(obj => obj.LocalidadesOrigem.Any(x => codigosLocalidades.Contains(x.Codigo)) ||
                              obj.Localidades.Any(x => codigosLocalidades.Contains(x.Localidade.Codigo)));

            return query.ToList();
        }

        public Dominio.Entidades.RotaFrete BuscarPorDescricaoParcial(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            query = query.Where(o => o.Descricao.Contains(descricao));

            return query.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.RotaFrete>> BuscarPorDescricoesParcialAsync(List<string> descricoesParciais, int pageSize = 2000)
        {
            if (descricoesParciais == null || descricoesParciais.Count == 0)
                return new List<Dominio.Entidades.RotaFrete>();

            List<Dominio.Entidades.RotaFrete> resultado = new List<Dominio.Entidades.RotaFrete>();
            var descricoes = descricoesParciais.Select(d => d.Trim()).Distinct().ToList();

            for (int i = 0; i < descricoes.Count; i += pageSize)
            {
                var bloco = descricoes.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                    .Where(r => bloco.Any(desc => r.Descricao.Contains(desc)));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public List<Dominio.Entidades.RotaFrete> BuscarPorDistribuidorParaExclusao(double distribuidor, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Distribuidor.CPF_CNPJ == distribuidor && !codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.RotaFrete BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.RotaFrete>> BuscarPorCodigosIntegracaoAsync(List<string> codigosIntegracao, int pageSize = 2000)
        {
            if (codigosIntegracao == null || codigosIntegracao.Count == 0)
                return new List<Dominio.Entidades.RotaFrete>();

            var codigos = codigosIntegracao.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.RotaFrete> resultado = new List<Dominio.Entidades.RotaFrete>();

            for (int i = 0; i < codigos.Count; i += pageSize)
            {
                var bloco = codigos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                    .Where(r => bloco.Contains(r.CodigoIntegracao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.RotaFrete BuscarAtivaPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao && obj.Ativo);

            return query.Fetch(o => o.ClientesOrigem).FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorCodigoIntegracaoEIntegradora(string codigoIntegracao, int codigoIntegradora)
        {
            IQueryable<Dominio.Entidades.RotaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            query = query.Where(obj => obj.CodigoIntegracao == codigoIntegracao && obj.Integradora.Codigo == codigoIntegradora);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorCodigoIntegracao(int codigoEmpresa, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && codigoIntegracao.Contains(obj.CodigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorCodigoIntegracaoDuplicado(int codigo, string codigoIntegracao)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (codigo > 0)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Codigo != codigo);

            return consultaRotaFrete.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarRotasPorOrigemDestino(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            IQueryable<Dominio.Entidades.RotaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            query = query.Where(obj => obj.Localidades.Any(o => o.Localidade.Codigo == destino.Codigo) &&
                                       obj.LocalidadesOrigem.Any(o => o.Codigo == origem.Codigo) &&
                                       obj.Ativo);

            return query.ToList();
        }

        public Dominio.Entidades.RotaFrete BuscarPorOrigemDestino(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, bool ativo)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && !((bool?)o.RotaExclusivaCompraValePedagio ?? false)
                    && o.Localidades.Any(l => l.Localidade.Codigo == destino.Codigo) && o.LocalidadesOrigem.Any(l => l.Codigo == origem.Codigo));

            return consultaRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorOrigemDestinoDestinatario(int codigoOrigem, int codigoDestino, double cnpjCpfDestinatario)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && o.LocalidadesOrigem.Any(l => l.Codigo == codigoOrigem) && o.Localidades.Any(l => l.Localidade.Codigo == codigoDestino)
                    && (o.Destinatarios.All(d => d.Cliente.CPF_CNPJ == cnpjCpfDestinatario) || (o.ValidarParaQualquerDestinatarioInformado && o.Destinatarios.Any(d => d.Cliente.CPF_CNPJ == cnpjCpfDestinatario))));

            return consultaRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorLocalidade(Dominio.Entidades.Localidade localidade, bool ativo)
        {
            var consultaRotaFreteLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteLocalidade>();
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o =>
                    o.Ativo == ativo && !((bool?)o.RotaExclusivaCompraValePedagio ?? false) &&
                    consultaRotaFreteLocalidade.Count(l => l.RotaFrete.Codigo == o.Codigo) > 1 &&
                    consultaRotaFreteLocalidade.Any(l => l.RotaFrete.Codigo == o.Codigo && l.Localidade.Codigo == localidade.Codigo)
                );

            return consultaRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorOrigemEEstadoDestino(int origem, string uf, bool ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Estados.Any(o => o.Sigla == uf) && obj.LocalidadesOrigem.Any(o => o.Codigo == origem) && obj.Ativo == ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorEstado(string uf, bool ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Estados.Any(o => o.Sigla == uf) && obj.Ativo == ativo && !((bool?)obj.RotaExclusivaCompraValePedagio ?? false) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorRemetenteDestinatario(double cpfCnpjRemetente, double cpfCnpjDestinatario)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && (o.Remetente.CPF_CNPJ == cpfCnpjRemetente) && o.Destinatarios.Any(d => d.Cliente.CPF_CNPJ == cpfCnpjDestinatario));

            return consultaRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.RotaFrete BuscarPorOrigemDestinoTipoOperacaoTransportador(int codigoOrigem, int codigoDestino, int codigoTipoOperacao, int codigoTransportador)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && o.LocalidadesOrigem.Any(origem => origem.Codigo == codigoOrigem) && o.Localidades.Any(d => d.Localidade.Codigo == codigoDestino));

            if (codigoTipoOperacao > 0)
                consultaRotaFrete = consultaRotaFrete.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            if (codigoTransportador > 0)
            {
                var consultaEmpresasExclusivas = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                    .Where(obj => obj.Empresa.Codigo == codigoTransportador && obj.RotaFrete.Ativo && obj.RotaFrete.LocalidadesOrigem.Any(origem => origem.Codigo == codigoOrigem) && obj.RotaFrete.Localidades.Any(d => d.Localidade.Codigo == codigoDestino));

                if (codigoTipoOperacao > 0)
                    consultaEmpresasExclusivas = consultaEmpresasExclusivas.Where(obj => obj.RotaFrete.TipoOperacao.Codigo == codigoTipoOperacao);

                return consultaEmpresasExclusivas.Select(obj => obj.RotaFrete).FirstOrDefault();
            }

            consultaRotaFrete = consultaRotaFrete.Where(obj => !((bool?)obj.RotaExclusivaCompraValePedagio ?? false));

            return consultaRotaFrete.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.RotaFrete> BuscarPorOrigemDestinoTipoOperacaoTransportadorAsync(int codigoOrigem, int codigoDestino, int codigoTipoOperacao, int codigoTransportador)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && o.LocalidadesOrigem.Any(origem => origem.Codigo == codigoOrigem) && o.Localidades.Any(d => d.Localidade.Codigo == codigoDestino));

            if (codigoTipoOperacao > 0)
                consultaRotaFrete = consultaRotaFrete.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            if (codigoTransportador > 0)
            {
                var consultaEmpresasExclusivas = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                    .Where(obj => obj.Empresa.Codigo == codigoTransportador && obj.RotaFrete.Ativo && obj.RotaFrete.LocalidadesOrigem.Any(origem => origem.Codigo == codigoOrigem) && obj.RotaFrete.Localidades.Any(d => d.Localidade.Codigo == codigoDestino));

                if (codigoTipoOperacao > 0)
                    consultaEmpresasExclusivas = consultaEmpresasExclusivas.Where(obj => obj.RotaFrete.TipoOperacao.Codigo == codigoTipoOperacao);

                return consultaEmpresasExclusivas.Select(obj => obj.RotaFrete).FirstOrDefault();
            }

            consultaRotaFrete = consultaRotaFrete.Where(obj => !((bool?)obj.RotaExclusivaCompraValePedagio ?? false));

            return await consultaRotaFrete.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.RotaFrete BuscarPorRemetentesDestinatario(List<Dominio.Entidades.Cliente> remetentes, double cpfCnpjDestinatario)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && remetentes.Contains(o.Remetente) && o.Destinatarios.Any(d => d.Cliente.CPF_CNPJ == cpfCnpjDestinatario));

            return consultaRotaFrete.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFrete> BuscaPendentesRoteirizacao(int limite, int tentativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query
                         where
                         obj.SituacaoDaRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando ||
                         (obj.SituacaoDaRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro && obj.TentativasIntegracao < tentativas)
                         select obj;
            return result.Take(limite).ToList();
        }

        public List<Dominio.Entidades.RotaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result
                .OrderBy(propOrdena + " " + dirOrdena).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.Fronteiras)
                .Fetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestino(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem() { Localidade = destino }
            };

            return BuscarPorOrigemEDestinos(null, origem, null, null, localidadesDestinoOrdenadas, null, null, false, null);
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestinos(Dominio.Entidades.Localidade origem, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas, List<Dominio.Entidades.Estado> estadosDestino)
        {
            return BuscarPorOrigemEDestinos(null, origem, null, null, localidadesDestinoOrdenadas, estadosDestino, null, false, null);
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestinos(Dominio.Entidades.Localidade origem, Dominio.Entidades.Cliente remetente, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino)
        {
            return BuscarPorOrigemEDestinos(null, origem, new List<Dominio.Entidades.Cliente>() { remetente }, destinatarios, localidadesDestino, estadosDestino, null, false, null);
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestinos(GrupoPessoas grupoPessoaPrincipal, List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino, TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao, bool rotaExclusivaCompraValePedagio, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga)
        {
            return BuscarPorOrigemEDestinos(grupoPessoaPrincipal, null, remetentes, destinatarios, localidadesDestino, estadosDestino, tipoUltimoPontoRoteirizacao, rotaExclusivaCompraValePedagio, veiculo, tipoDeCarga);
        }
        public async Task<List<Dominio.Entidades.RotaFrete>> BuscarPorOrigemEDestinosAsync(GrupoPessoas grupoPessoaPrincipal, List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino, TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao, bool rotaExclusivaCompraValePedagio, Dominio.Entidades.Veiculo veiculo)
        {
            return BuscarPorOrigemEDestinos(grupoPessoaPrincipal, null, remetentes, destinatarios, localidadesDestino, estadosDestino, tipoUltimoPontoRoteirizacao, rotaExclusivaCompraValePedagio, veiculo);
        }

        public Dominio.Entidades.RotaFrete BuscarPorCodigoIntegracaoETipoIntegracao(string codigoIntegracao, int codigoTipoIntegracao, bool adicionadoViaIntegracao)
        {
            IQueryable<Dominio.Entidades.RotaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            query = query.Where(obj => obj.CodigoIntegracao == codigoIntegracao && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.AdicionadoViaIntegracao == adicionadoViaIntegracao);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarPorClienteComoRemetenteOuDestinatario(Dominio.Entidades.Cliente cliente)
        {
            IQueryable<Dominio.Entidades.RotaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            query = query.Where(obj => obj.Remetente.CPF_CNPJ == cliente.CPF_CNPJ || obj.Destinatarios.Any(d => d.Cliente.CPF_CNPJ == cliente.CPF_CNPJ));

            return query.ToList();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarRotasFreteFiltradas(List<Dominio.Entidades.RotaFrete> rotas, int codigoEmpresa)
        {
            List<int> codigosRotaFrete = rotas.Select(o => o.Codigo).ToList();
            List<Dominio.Entidades.RotaFrete> resultado = new List<Dominio.Entidades.RotaFrete>();

            int take = 1000;
            int start = 0;

            while (start < codigosRotaFrete.Count)
            {
                List<int> codigos = codigosRotaFrete.Skip(start).Take(take).ToList();

                var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                    .Where(rotaFrete => codigos.Contains(rotaFrete.Codigo));

                if (codigoEmpresa > 0)
                {
                    var consultaRotaFreteEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                        .Where(o => o.Empresa.Codigo == codigoEmpresa);

                    consultaRotaFrete = consultaRotaFrete
                        .Where(rotaFrete =>
                            ((bool?)rotaFrete.PossuiTransportadoresExclusivos ?? false) == false ||
                            consultaRotaFreteEmpresaExclusiva.Where(transportadorExclusivo => transportadorExclusivo.RotaFrete.Codigo == rotaFrete.Codigo).Any()
                         );
                }

                resultado.AddRange(consultaRotaFrete.ToList());
                start += take;
            }

            foreach (var rota in resultado.Where(x => rotas.Where(x => x.DestinoExato).Select(x => x.Codigo).Contains(x.Codigo)).ToList())
                rota.DestinoExato = true;

            return resultado
                .OrderByDescending(rotaFrete => rotaFrete.TipoOperacao != null)
                .OrderByDescending(rotaFrete => rotaFrete.PossuiTransportadoresExclusivos)
                .OrderByDescending(rotaFrete => rotaFrete.PossuiVeiculosInformados)
                .OrderByDescending(rotaFrete => rotaFrete.DestinoExato)
                .ToList();

        }

        public bool PossuiRotaFreteExclusivaCompraValePedagio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Ativo && obj.RotaExclusivaCompraValePedagio select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarArquivoHistoricoPorCodigo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var result = from obj in query where obj.Codigo == codigoArquivo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFrete> ConsultarRotasFretePracaPedagio(int numeroDiasConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && o.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido && (o.DataConsultaPracasPedido <= DateTime.Now.AddDays(-numeroDiasConsulta) || !o.DataConsultaPracasPedido.HasValue));

            return query.Take(100).ToList();
        }

        public List<Dominio.Entidades.RotaFrete> BuscarRotasFreteComDistruibuicaoDeCargasOfertadosComTransportadores()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo && o.Empresas.Count() > 0);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestinos(GrupoPessoas grupoPessoaPrincipal, Dominio.Entidades.Localidade origem, List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino, TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao, bool rotaExclusivaCompraValePedagio, Dominio.Entidades.Veiculo veiculo)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo);

            Dominio.Entidades.Cliente remetente = remetentes?.FirstOrDefault();
            List<Dominio.Entidades.Cliente> coletas = (remetentes?.Count > 1) ? (from o in remetentes where o.Codigo != remetente.Codigo select o).ToList() : new List<Dominio.Entidades.Cliente>();
            List<string> listaUfDestino = (estadosDestino?.Count > 0) ? estadosDestino.Select(o => o.Sigla).Distinct().ToList() : new List<string>();
            List<int> codigosRegiao = (localidadesDestino?.Count > 0) ? localidadesDestino.Where(obj => obj.Localidade.Regiao != null).Select(o => o.Localidade.Regiao.Codigo).Distinct().ToList() : new List<int>();
            int codigoRegiaoDestino = 0;

            if (codigosRegiao.Count == 1)
                codigoRegiaoDestino = codigosRegiao.FirstOrDefault();

            if (grupoPessoaPrincipal != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas.Codigo == grupoPessoaPrincipal.Codigo || o.GrupoPessoas == null);
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas == null);

            if (remetente != null)
            {
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null || o.Remetente.CPF_CNPJ == remetente.CPF_CNPJ);
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Any(lo => lo.Codigo == remetente.Localidade.Codigo) || !o.LocalidadesOrigem.Any());
            }
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null);

            if (coletas?.Count > 0)
                consultaRotaFrete = consultaRotaFrete.Where(o => (o.Coletas.Count == coletas.Count && o.Coletas.All(c => coletas.Contains(c))) || (!o.Coletas.Any()));
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Coletas.Count == 0);

            if (origem != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Any(lo => lo.Codigo == origem.Codigo) || !o.LocalidadesOrigem.Any());

            if (tipoUltimoPontoRoteirizacao.HasValue)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.TipoUltimoPontoRoteirizacao == tipoUltimoPontoRoteirizacao.Value);

            consultaRotaFrete = consultaRotaFrete.Where(DestinosExatos(destinatarios, localidadesDestino, listaUfDestino, codigoRegiaoDestino));

            if (rotaExclusivaCompraValePedagio)
                consultaRotaFrete = consultaRotaFrete.Where(obj => obj.RotaExclusivaCompraValePedagio);
            else
                consultaRotaFrete = consultaRotaFrete.Where(obj => !((bool?)obj.RotaExclusivaCompraValePedagio ?? false));

            if (veiculo != null)
            {
                var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>();
                consultaRotaFrete = consultaRotaFrete.Where(obj => consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo && o.Veiculo.Codigo == veiculo.Codigo) || !consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo));
            }

            List<int> codigos = consultaRotaFrete.WithOptions(o => o.SetTimeout(240)).Select(x => x.Codigo).ToList();
            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = (int)Math.Ceiling((double)codigos.Count / quantidadeRegistrosConsultarPorVez);

            List<Dominio.Entidades.RotaFrete> rotaFreteRetornar = new List<Dominio.Entidades.RotaFrete>();

            for (int i = 0; i < quantidadeConsultas; i++)
                rotaFreteRetornar.AddRange(this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>().Where(x => codigos.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(x.Codigo)).ToList());

            return rotaFreteRetornar;
        }

        private List<Dominio.Entidades.RotaFrete> BuscarPorOrigemEDestinos(GrupoPessoas grupoPessoaPrincipal, Dominio.Entidades.Localidade origem, List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino, TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao, bool rotaExclusivaCompraValePedagio, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo);

            Dominio.Entidades.Cliente remetente = remetentes?.FirstOrDefault();
            List<Dominio.Entidades.Cliente> coletas = (remetentes?.Count > 1) ? (from o in remetentes where o.Codigo != remetente.Codigo select o).ToList() : new List<Dominio.Entidades.Cliente>();
            List<string> listaUfDestino = (estadosDestino?.Count > 0) ? estadosDestino.Select(o => o.Sigla).Distinct().ToList() : new List<string>();
            List<int> codigosRegiao = (localidadesDestino?.Count > 0) ? localidadesDestino.Where(obj => obj.Localidade.Regiao != null).Select(o => o.Localidade.Regiao.Codigo).Distinct().ToList() : new List<int>();
            int codigoRegiaoDestino = 0;

            if (codigosRegiao.Count == 1)
                codigoRegiaoDestino = codigosRegiao.FirstOrDefault();

            if (grupoPessoaPrincipal != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas.Codigo == grupoPessoaPrincipal.Codigo || o.GrupoPessoas == null);
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas == null);

            if (remetente != null)
            {
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null || o.Remetente.CPF_CNPJ == remetente.CPF_CNPJ);
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Any(lo => lo.Codigo == remetente.Localidade.Codigo) || !o.LocalidadesOrigem.Any());
            }
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null);

            if (coletas?.Count > 0)
                consultaRotaFrete = consultaRotaFrete.Where(o => (o.Coletas.Count == coletas.Count && o.Coletas.All(c => coletas.Contains(c))) || (!o.Coletas.Any()));
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Coletas.Count == 0);

            if (origem != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Any(lo => lo.Codigo == origem.Codigo) || !o.LocalidadesOrigem.Any());

            if (tipoUltimoPontoRoteirizacao.HasValue)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.TipoUltimoPontoRoteirizacao == tipoUltimoPontoRoteirizacao.Value);

            consultaRotaFrete = consultaRotaFrete.Where(DestinosExatos(destinatarios, localidadesDestino, listaUfDestino, codigoRegiaoDestino));

            if (rotaExclusivaCompraValePedagio)
                consultaRotaFrete = consultaRotaFrete.Where(obj => obj.RotaExclusivaCompraValePedagio);
            else
                consultaRotaFrete = consultaRotaFrete.Where(obj => !((bool?)obj.RotaExclusivaCompraValePedagio ?? false));

            if (veiculo != null)
            {
                var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>();
                consultaRotaFrete = consultaRotaFrete.Where(obj => consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo && o.Veiculo.Codigo == veiculo.Codigo) || !consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo));
            }

            if (tipoDeCarga != null)
            {
                consultaRotaFrete = consultaRotaFrete.Where(rotaFrete => !rotaFrete.Restricoes.Any(restricao => restricao.TipoDeCarga.Codigo == tipoDeCarga.Codigo));
            }

            List<int> codigos = consultaRotaFrete.WithOptions(o => o.SetTimeout(240)).Select(x => x.Codigo).ToList();
            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = (int)Math.Ceiling((double)codigos.Count / quantidadeRegistrosConsultarPorVez);

            List<Dominio.Entidades.RotaFrete> rotaFreteRetornar = new List<Dominio.Entidades.RotaFrete>();

            for (int i = 0; i < quantidadeConsultas; i++)
                rotaFreteRetornar.AddRange(this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>().Where(x => codigos.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(x.Codigo)).ToList());

            List<Dominio.Entidades.RotaFrete> rotasComColetas = rotaFreteRetornar
                .Where(o =>
                    (o.Coletas?.Count ?? 0) == (coletas?.Count ?? 0) &&
                    (o.Coletas != null && coletas != null && o.Coletas.All(c => coletas.Contains(c)))
                )
                .ToList();

            if (rotasComColetas.Count > 0)
                return rotasComColetas;

            return rotaFreteRetornar;
        }
        //Async Method
        private Task<List<Dominio.Entidades.RotaFrete>> BuscarPorOrigemEDestinosAsync(GrupoPessoas grupoPessoaPrincipal, Dominio.Entidades.Localidade origem, List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino, List<Dominio.Entidades.Estado> estadosDestino, TipoUltimoPontoRoteirizacao? tipoUltimoPontoRoteirizacao, bool rotaExclusivaCompraValePedagio, Dominio.Entidades.Veiculo veiculo)
        {
            var consultaRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>()
                .Where(o => o.Ativo);

            Dominio.Entidades.Cliente remetente = remetentes?.FirstOrDefault();
            List<Dominio.Entidades.Cliente> coletas = (remetentes?.Count > 1) ? (from o in remetentes where o.Codigo != remetente.Codigo select o).ToList() : new List<Dominio.Entidades.Cliente>();
            List<string> listaUfDestino = (estadosDestino?.Count > 0) ? estadosDestino.Select(o => o.Sigla).Distinct().ToList() : new List<string>();
            List<int> codigosRegiao = (localidadesDestino?.Count > 0) ? localidadesDestino.Where(obj => obj.Localidade.Regiao != null).Select(o => o.Localidade.Regiao.Codigo).Distinct().ToList() : new List<int>();
            int codigoRegiaoDestino = 0;

            if (codigosRegiao.Count == 1)
                codigoRegiaoDestino = codigosRegiao.FirstOrDefault();

            if (grupoPessoaPrincipal != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas == grupoPessoaPrincipal || o.GrupoPessoas == null);
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.GrupoPessoas == null);

            if (remetente != null)
            {
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null || o.Remetente == remetente);
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Contains(remetente.Localidade) || !o.LocalidadesOrigem.Any());
            }
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Remetente == null);

            if (coletas?.Count > 0)
                consultaRotaFrete = consultaRotaFrete.Where(o => (o.Coletas.Count == coletas.Count && o.Coletas.All(c => coletas.Contains(c))) || (!o.Coletas.Any()));
            else
                consultaRotaFrete = consultaRotaFrete.Where(o => o.Coletas.Count == 0);

            if (origem != null)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.LocalidadesOrigem.Contains(origem) || !o.LocalidadesOrigem.Any());

            if (tipoUltimoPontoRoteirizacao.HasValue)
                consultaRotaFrete = consultaRotaFrete.Where(o => o.TipoUltimoPontoRoteirizacao == tipoUltimoPontoRoteirizacao.Value);

            consultaRotaFrete = consultaRotaFrete.Where(DestinosExatos(destinatarios, localidadesDestino, listaUfDestino, codigoRegiaoDestino));

            if (rotaExclusivaCompraValePedagio)
                consultaRotaFrete = consultaRotaFrete.Where(obj => obj.RotaExclusivaCompraValePedagio);
            else
                consultaRotaFrete = consultaRotaFrete.Where(obj => !((bool?)obj.RotaExclusivaCompraValePedagio ?? false));

            if (veiculo != null)
            {
                var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>();
                consultaRotaFrete = consultaRotaFrete.Where(obj => consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo && o.Veiculo == veiculo) || !consultaRotaFreteVeiculo.Any(o => o.RotaFrete.Codigo == obj.Codigo));
            }

            List<int> codigos = consultaRotaFrete.WithOptions(o => o.SetTimeout(240)).Select(x => x.Codigo).ToList();
            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = (int)Math.Ceiling((double)codigos.Count / quantidadeRegistrosConsultarPorVez);

            List<Dominio.Entidades.RotaFrete> rotaFreteRetornar = new List<Dominio.Entidades.RotaFrete>();

            for (int i = 0; i < quantidadeConsultas; i++)
                rotaFreteRetornar.AddRange((IEnumerable<Dominio.Entidades.RotaFrete>)this.SessionNHiBernate
                    .Query<Dominio.Entidades.RotaFrete>()
                    .Where(x => codigos.Skip(i * quantidadeRegistrosConsultarPorVez)
                    .Take(quantidadeRegistrosConsultarPorVez)
                    .Contains(x.Codigo)).ToListAsync());

            return Task.FromResult(rotaFreteRetornar);
        }
        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> DestinosExatos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> clientes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas, List<string> estados, int codigoRegiaoDestino)
        {
            Expression<Func<Dominio.Entidades.RotaFrete, bool>> destinosExatos = null;

            if (clientes?.Count > 0)
                destinosExatos = ClientesDestinoExatos(clientes).Or(ClientesDestinoContidos(clientes));

            if (localidadesDestinoOrdenadas?.Count > 0)
            {
                if (destinosExatos == null)
                    destinosExatos = LocalidadesDestinoExatas(localidadesDestinoOrdenadas);
                else
                    destinosExatos = destinosExatos.Or(LocalidadesDestinoExatas(localidadesDestinoOrdenadas));
            }

            if (estados.Count > 0)
            {
                if (destinosExatos == null)
                    destinosExatos = EstadosDestinoExatos(estados);
                else
                    destinosExatos = destinosExatos.Or(EstadosDestinoExatos(estados));
            }

            if (codigoRegiaoDestino > 0)
            {
                if (destinosExatos == null)
                    destinosExatos = RegiaoExata(codigoRegiaoDestino);
                else
                    destinosExatos = destinosExatos.Or(RegiaoExata(codigoRegiaoDestino));
            }

            return destinosExatos;
        }

        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> ClientesDestinoExatos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> clientesDestino)
        {
            var clientesExatos = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            clientesExatos = clientesExatos.And(o => o.Destinatarios.Count == clientesDestino.Count);

            var clientesExatosClientes = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            foreach (var clienteDestino in clientesDestino)
                clientesExatosClientes = clientesExatosClientes.And(o => o.Destinatarios.Any(l => l.Cliente.CPF_CNPJ == clienteDestino.Cliente.CPF_CNPJ && l.Ordem == clienteDestino.Ordem));

            clientesExatos = clientesExatos.And(clientesExatosClientes);

            return clientesExatos;
        }

        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> ClientesDestinoContidos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> clientesDestino)
        {
            var clientesContidos = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            clientesContidos = clientesContidos.And(o => o.ValidarParaQualquerDestinatarioInformado);

            List<double?> cpfsCnpjs = clientesDestino
                           .Select(cd => cd.Cliente?.CPF_CNPJ)
                           .Where(cpfCnpj => cpfCnpj != null)
                           .ToList();

            if (cpfsCnpjs.IsNullOrEmpty())
                return clientesContidos;

            clientesContidos = clientesContidos.And(o =>
                o.Destinatarios.Any(l => cpfsCnpjs.Contains(l.Cliente.CPF_CNPJ))
            );

            return clientesContidos;
        }

        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> LocalidadesDestinoExatas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino)
        {
            var localidadesExatas = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            localidadesExatas = localidadesExatas.And(o => o.Localidades.Count == localidadesDestino.Count);

            var localidadesExatasLocalidades = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem destino in localidadesDestino)
                localidadesExatasLocalidades = localidadesExatasLocalidades.And(o => o.Localidades.Any(l => l.Localidade.Codigo == destino.Localidade.Codigo && ((int?)l.Ordem ?? 0) == destino.Ordem));

            localidadesExatas = localidadesExatas.And(localidadesExatasLocalidades);

            return localidadesExatas;
        }

        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> RegiaoExata(int regiaoDestino)
        {
            var regiaoExata = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            regiaoExata = regiaoExata.And(o => o.RegiaoDestino.Codigo == regiaoDestino);

            return regiaoExata;
        }

        private Expression<Func<Dominio.Entidades.RotaFrete, bool>> EstadosDestinoExatos(List<string> estadosDestino)
        {
            //var estadosExatos = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            //estadosExatos = estadosExatos.And(o => o.Estados.Count == estadosDestino.Count);

            var estadosExatosEstados = PredicateBuilder.True<Dominio.Entidades.RotaFrete>();

            foreach (string estadoDestino in estadosDestino)
                estadosExatosEstados = estadosExatosEstados.And(o => o.Estados.Any(l => l.Sigla == estadoDestino));

            return estadosExatosEstados;

            //estadosExatos = estadosExatos.And(estadosExatosEstados);

            //return estadosExatos;
        }

        private IQueryable<Dominio.Entidades.RotaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.FilialDistribuidora))
                result = result.Where(o => o.FilialDistribuidora.Contains(filtrosPesquisa.FilialDistribuidora));

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas || o.GrupoPessoas == null);

            if (filtrosPesquisa.CodigoGrupoPessoas == 0 && filtrosPesquisa.SomenteGrupo)
                result = result.Where(o => o.GrupoPessoas == null);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao.Equals(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Remetente > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ == filtrosPesquisa.Remetente);

            if (filtrosPesquisa.CodigoOrigem > 0)
                result = result.Where(o => o.LocalidadesOrigem.Any(L => L.Codigo == filtrosPesquisa.CodigoOrigem));

            if (filtrosPesquisa.CodigoDestino > 0)
                result = result.Where(o => o.Localidades.Any(L => L.Localidade.Codigo == filtrosPesquisa.CodigoDestino));

            if (filtrosPesquisa.CodigosCidadeRemetente?.Count > 0 && filtrosPesquisa.CodigosCidadeDestinatario?.Count > 0 && filtrosPesquisa.CodigosDestinatario.Count > 0)
            {
                result = result.Where(o => (o.LocalidadesOrigem.Any(origem => filtrosPesquisa.CodigosCidadeRemetente.Contains(origem.Codigo))
                                      && o.Localidades.Any(destino => filtrosPesquisa.CodigosCidadeDestinatario.Contains(destino.Localidade.Codigo)))
                                      || o.Destinatarios.Any(destinatario => filtrosPesquisa.CodigosDestinatario.Contains(destinatario.Cliente.CPF_CNPJ))
                );
            }
            else
            {
                if (filtrosPesquisa.CodigosCidadeRemetente?.Count > 0)
                    result = result.Where(o => o.LocalidadesOrigem.Any(origem => filtrosPesquisa.CodigosCidadeRemetente.Contains(origem.Codigo)));

                if (filtrosPesquisa.CodigosCidadeDestinatario?.Count > 0)
                    result = result.Where(o => o.Localidades.Any(destino => filtrosPesquisa.CodigosCidadeDestinatario.Contains(destino.Localidade.Codigo)));

                if (filtrosPesquisa.CodigosDestinatario.Count > 0)
                    result = result.Where(o => o.Destinatarios.Any(destinatario => filtrosPesquisa.CodigosDestinatario.Contains(destinatario.Cliente.CPF_CNPJ)));
            }

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.CEPDestino > 0)
                result = result.Where(obj => obj.CEPsDestino.Any(cpe => cpe.CEPFinal >= filtrosPesquisa.CEPDestino && cpe.CEPInicial <= filtrosPesquisa.CEPDestino));

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                var queryEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>();
                queryEmpresaExclusiva = queryEmpresaExclusiva.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

                result = result.Where(o => queryEmpresaExclusiva.Any(empresaExclusiva => empresaExclusiva.RotaFrete.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.RotaExclusivaCompraValePedagio.HasValue)
            {
                if (filtrosPesquisa.RotaExclusivaCompraValePedagio.Value)
                    result = result.Where(o => o.RotaExclusivaCompraValePedagio);
                else
                    result = result.Where(o => !((bool?)o.RotaExclusivaCompraValePedagio ?? false));
            }

            if (filtrosPesquisa.SituacaoRoteirizacao != SituacaoRoteirizacao.Todas)
                result = result.Where(x => x.SituacaoDaRoteirizacao == filtrosPesquisa.SituacaoRoteirizacao);

            return result;
        }

        #endregion

        #region Relatório Rota Frete

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery query = new ConsultaRotaFrete().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery query = new ConsultaRotaFrete().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery query = new ConsultaRotaFrete().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}