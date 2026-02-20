using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using Repositorio.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Localidade : RepositorioBase<Dominio.Entidades.Localidade>
    {
        #region Construtores
        private CancellationToken _cancellationToken;

        public Localidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Localidade(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Localidade> Consulta(string cidade, int codigoIBGE, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Descricao", string dirOrdenacao = "asc", string uf = "", bool somenteEmpresa = false, int empresa = 0, List<string> estados = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(cidade))
                result = result.Where(obj => obj.Descricao.Contains(cidade));

            if (codigoIBGE > 0)
                result = result.Where(obj => obj.CodigoIBGE == codigoIBGE);

            if (!string.IsNullOrWhiteSpace(uf))
                result = result.Where(obj => obj.Estado.Sigla == uf);

            if (somenteEmpresa)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (estados != null && estados.Count > 0)
                result = result.Where(obj => estados.Contains(obj.Estado.Sigla));

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();

            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Localidade>();
            //criteria.CreateAlias("Estado", "estado");

            //if (!string.IsNullOrWhiteSpace(cidade))
            //    criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", cidade, NHibernate.Criterion.MatchMode.Anywhere));

            //if (codigoIBGE > 0)
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("CodigoIBGE", codigoIBGE));

            //if (!string.IsNullOrWhiteSpace(uf))
            //    criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Estado.Sigla", uf, NHibernate.Criterion.MatchMode.Anywhere));
            ////criteria.Add(NHibernate.Criterion.Expression.Eq("Estado.Sigla", uf));

            //if (dirOrdenacao == "asc")
            //    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propOrdenacao));
            //else
            //    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propOrdenacao));

            //criteria.SetMaxResults(maximoRegistros);
            //criteria.SetFirstResult(inicioRegistros);

            //return criteria.List<Dominio.Entidades.Localidade>();
        }

        public int ContarConsulta(string cidade, int codigoIBGE, string uf = "", bool somenteEmpresa = false, int empresa = 0, List<string> estados = null)
        {
            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Localidade>();

            //if (!string.IsNullOrWhiteSpace(cidade))
            //    criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", cidade, NHibernate.Criterion.MatchMode.Anywhere));

            //if (codigoIBGE > 0)
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("CodigoIBGE", codigoIBGE));

            //if (!string.IsNullOrWhiteSpace(uf))
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("Estado.Sigla", uf));

            //criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());

            //return criteria.UniqueResult<int>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(cidade))
                result = result.Where(obj => obj.Descricao.Contains(cidade));

            if (codigoIBGE > 0)
                result = result.Where(obj => obj.CodigoIBGE == codigoIBGE);

            if (estados != null && estados.Count > 0)
                result = result.Where(obj => estados.Contains(obj.Estado.Sigla));

            if (!string.IsNullOrWhiteSpace(uf))
                result = result.Where(obj => obj.Estado.Sigla == uf);

            if (somenteEmpresa)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.Count();
        }

        public List<Dominio.Entidades.Localidade> ConsultaPolos(string cidade, int codigoIBGE, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, string uf = "")
        {
            var consultaLocalidade = ConsultaPolos(cidade, codigoIBGE, uf);

            return ObterLista(consultaLocalidade, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsultaPolos(string cidade, int codigoIBGE, string uf = "")
        {
            var consultaLocalidade = ConsultaPolos(cidade, codigoIBGE, uf);

            return consultaLocalidade.Count();
        }

        public Dominio.Entidades.Localidade BuscarPorCodigo(int codigo, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.Codigo == codigo).FirstOrDefault();


            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.Regiao).FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorCodigoFetch(int codigo, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.Codigo == codigo).FirstOrDefault();


            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.Regiao).Fetch(o => o.Pais).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Localidade> BuscarPorCodigoAsync(int codigo, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.Codigo == codigo).FirstOrDefault();

            return await this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().
                Where(x => x.Codigo == codigo).FirstOrDefaultAsync(_cancellationToken);
        }

        public bool ContemLocalidadeDuplicada(int codigo, int codigoPais, string descricao, string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            query = query.Where(obj => obj.Pais.Codigo == codigoPais && obj.Descricao == descricao && obj.Estado.Sigla == uf);
            if (codigo > 0)
                query = query.Where(c => c.Codigo != codigo);
            return query.Any();
        }

        public List<Dominio.Entidades.Localidade> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Localidade>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Localidade>();

            List<Dominio.Entidades.Localidade> resultado = new List<Dominio.Entidades.Localidade>();

            for (int i = 0; i < codigos.Count; i += pageSize)
            {
                var bloco = codigos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                    .Where(loc => bloco.Contains(loc.Codigo));

                var resultadoBloco = await query.ToListAsync(_cancellationToken);

                resultado.AddRange(resultadoBloco);
            }
            return resultado;
        }

        public List<Dominio.Entidades.Localidade> BuscarLocalidadeSemCoordenada(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where (obj.Latitude == 0 || obj.Longitude == 0 || obj.Latitude == null || obj.Longitude == null) && obj.DataAtualizacao == null select obj;
            return result.Take(limite).ToList();
        }

        public Dominio.Entidades.Localidade buscarPorCodigoEmbarcador(string codigoEmbarcador, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.CodigoLocalidadeEmbarcador == codigoEmbarcador).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CodigoLocalidadeEmbarcador == codigoEmbarcador).FirstOrDefault();
        }

        public Dominio.Entidades.Localidade buscarPorCodigoCidadeo(string codigoCidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.CodigoCidade == codigoCidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade buscarPorCodigoDocumento(string codigoDocumento, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.CodigoDocumento == codigoDocumento).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CodigoDocumento == codigoDocumento).FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> BuscarTodosComRegiao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query select obj;
            return result
                .Fetch(o => o.Regiao)
                .Fetch(o => o.Estado)
                .Fetch(o => o.OutrasDescricoes)
                .Fetch(o => o.Empresa)
                .ToList();

            //OutrasDescricoes

            //return lstLocalidades.Where(obj => obj.Estado.Sigla.Equals(uf) 
            //&& (obj.Descricao.Equals(descricao) || 
            //obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefault();


        }

        public int BuscarPorMaiorCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            return query.Select(obj => obj.Codigo).Max();
        }

        public List<Dominio.Entidades.Localidade> BuscarPorUF(string uf, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Sigla.Equals(uf) select obj;

            if (empresa > 0)
                result = result.Where(o => (o.Empresa == null || o.Empresa.Codigo == empresa));

            return result.OrderBy(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Localidade BuscarPrimeiraPorUF(string uf)
        {
            IQueryable<Dominio.Entidades.Localidade> result = ObterQueryBuscarPrimeiraPorUF(uf);

            return result.OrderBy(o => o.Descricao).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Localidade> BuscarPrimeiraPorUFDescricaoAsync(string uf, string descricao)
        {
            IQueryable<Dominio.Entidades.Localidade> query = ObterQueryBuscarPrimeiraPorUF(uf);

            query = query.Where(localidade => localidade.Descricao.Equals(descricao));

            return await query.OrderBy(o => o.Descricao).FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Localidade> BuscarPorUFDescricao(string uf, string descricao, int empresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Sigla.Equals(uf) && obj.Descricao.Equals(descricao) select obj;

            if (empresa > 0)
                result = result.Where(o => (o.Empresa == null || o.Empresa.Codigo == empresa));

            return result.OrderBy(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Localidade BuscarPorCodigoIBGE(int codigoIBGE, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.CodigoIBGE == codigoIBGE && obj.Empresa == null).FirstOrDefault();
            else
                return this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CodigoIBGE == codigoIBGE && obj.Empresa == null).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Localidade> BuscarPorCodigoIBGEAsync(int codigoIBGE, CancellationToken cancellationToken)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CodigoIBGE == codigoIBGE && obj.Empresa == null).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Localidade BuscarPorCodigoIBGE(int codigoIBGE, int pais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.CodigoIBGE == codigoIBGE && obj.Pais.Codigo == pais && obj.Empresa == null select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorDescricaoEUF(string descricao, string uf, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.Estado.Sigla.Equals(uf) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefault();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Sigla.Equals(uf) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null orderby obj.Descricao select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Localidade>> BuscarPorDescricoesEUFAsync(List<(string Cidade, string Estado)> listaDescricaoUF, List<Dominio.Entidades.Localidade> lstLocalidades = null, int pageSize = 500)
        {
            if (listaDescricaoUF == null || listaDescricaoUF.Count == 0)
                return new List<Dominio.Entidades.Localidade>();

            List<(string Descricao, string UF)> paresNormalizados = listaDescricaoUF
                .Select(p => (Descricao: p.Cidade.Trim(), UF: p.Estado.Trim()))
                .Distinct()
                .ToList();

            if (lstLocalidades != null)
            {
                return lstLocalidades
                    .Where(loc =>
                        loc.Empresa == null &&
                        paresNormalizados.Any(p =>
                            loc.Estado.Sigla.Equals(p.UF, StringComparison.OrdinalIgnoreCase) &&
                            (loc.Descricao.Equals(p.Descricao, StringComparison.OrdinalIgnoreCase) ||
                             (loc.OutrasDescricoes != null && loc.OutrasDescricoes.Contains(p.Descricao)))))
                    .OrderBy(loc => loc.Descricao)
                    .ToList();
            }

            List<Dominio.Entidades.Localidade> resultado = new List<Dominio.Entidades.Localidade>();

            for (int i = 0; i < paresNormalizados.Count; i += pageSize)
            {
                var bloco = paresNormalizados.Skip(i).Take(pageSize).ToList();

                var ufs = bloco.Select(p => p.UF).Distinct().ToList();
                var descricoes = bloco.Select(p => p.Descricao).Distinct().ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                    .Where(loc =>
                        loc.Empresa == null &&
                        ufs.Contains(loc.Estado.Sigla) &&
                        descricoes.Contains(loc.Descricao));

                var resultadoBloco = await query.ToListAsync(CancellationToken);
                resultado.AddRange(resultadoBloco);
            }

            return resultado;
        }

        public Dominio.Entidades.Localidade BuscarPorDescricaoEEstado(string descricao, string estado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Nome.Contains(estado) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null orderby obj.Descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorDescricaoAbreviacaoUF(string descricao, string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            query = from obj in query where obj.Estado.Abreviacao.Equals(uf) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null orderby obj.Descricao select obj;
            return query.FirstOrDefault();
        }

        public bool VerificarSeExisteLocalidade(string descricao, string uf)
        {
            IQueryable<Dominio.Entidades.Localidade> query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            query = query.Where(obj => obj.Empresa == null && obj.Estado.Sigla.Equals(uf) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)));

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Localidade BuscarPorDescricaoEPais(string descricao, string siglaPais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Pais.Abreviacao.Equals(siglaPais) && (obj.Descricao.Equals(descricao) || obj.OutrasDescricoes.Contains(descricao)) && obj.Empresa == null orderby obj.Descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Descricao.Equals(descricao) orderby obj.Descricao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Localidade BuscarPrimeiraPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorCEP(string CEP, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.CEP.StartsWith(CEP) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CEP.StartsWith(CEP) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefault();
        }


        public async Task<Dominio.Entidades.Localidade> BuscarPorCEPAsync(string CEP, List<Dominio.Entidades.Localidade> lstLocalidades = null)
        {
            if (lstLocalidades != null)
                return lstLocalidades.Where(obj => obj.CEP.StartsWith(CEP) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefault();

            return await this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>().Where(obj => obj.CEP.StartsWith(CEP) && obj.Empresa == null).OrderBy(obj => obj.Descricao).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Localidade BuscarPorEstado(string estado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Nome.Contains(estado) orderby obj.Descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorPais(int pais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Pais.Codigo == pais && obj.Empresa == null orderby obj.Descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> ConsultaBrasil(string cidade, int codigoIBGE, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Descricao", string dirOrdenacao = "asc", string uf = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            query = query.Where(o => o.Estado.Sigla != "EX");

            if (!string.IsNullOrWhiteSpace(cidade))
                query = query.Where(o => o.Descricao.Contains(cidade));

            if (codigoIBGE > 0)
                query = query.Where(o => o.CodigoIBGE == codigoIBGE);

            if (!string.IsNullOrWhiteSpace(uf))
                query = query.Where(o => o.Estado.Sigla == uf);

            return query.Fetch(o => o.Estado)
                        .Fetch(o => o.LocalidadePolo).OrderBy(propOrdenacao + " " + dirOrdenacao)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .ToList();
        }

        public int ContarConsultaBrasil(string cidade, int codigoIBGE, string uf = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            query = query.Where(o => o.Estado.Sigla != "EX");

            if (!string.IsNullOrWhiteSpace(cidade))
                query = query.Where(o => o.Descricao.Contains(cidade));

            if (codigoIBGE > 0)
                query = query.Where(o => o.CodigoIBGE == codigoIBGE);

            if (!string.IsNullOrWhiteSpace(uf))
                query = query.Where(o => o.Estado.Sigla == uf);

            return query.Count();
        }

        public Dominio.Entidades.Localidade BuscarPorEmpresaEDescricao(int empresa, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where (obj.Empresa.Codigo == empresa || obj.Empresa == null) && obj.Descricao == descricao select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorCodigoEmpresa(int empresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Empresa.Codigo == empresa && obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> BuscarPorCodigoIBGE(IEnumerable<int> codigosIBGE)
        {
            IQueryable<Dominio.Entidades.Localidade> query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            query = query.Where(o => codigosIBGE.Contains(o.CodigoIBGE));

            return query.ToList();
        }

        public Dominio.Entidades.Localidade BuscarPorCidadeUF(string nomeCidade, string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where obj.Estado.Sigla.ToLower().Equals(uf.ToLower()) && obj.Descricao.ToLower().Contains(nomeCidade.ToLower()) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidade BuscarPorDescricaoECodigoIBGE(string descricao, int codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao == descricao);

            if (codigoIBGE > 0)
                query = query.Where(o => o.CodigoIBGE == codigoIBGE);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> BuscarLocalidadesComCoordenadas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();
            var result = from obj in query where (obj.Latitude != null && obj.Longitude != null) || (obj.Latitude > 0 && obj.Longitude > 0) select obj;
            return result.ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Localidade> ObterQueryBuscarPrimeiraPorUF(string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>();

            var result = from obj in query where obj.Estado.Sigla.Equals(uf) && obj.Empresa == null select obj;

            return result;
        }

        private IQueryable<Dominio.Entidades.Localidade> ConsultaPolos(string cidade, int codigoIBGE, string uf = "")
        {
            var consultaLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                .Where(localidade => localidade.LocalidadePolo != null);

            var consultaLocalidadePolo = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                .Where(localidadePolo => consultaLocalidade.Any(localidade => localidade.LocalidadePolo.Codigo == localidadePolo.Codigo));

            if (!string.IsNullOrWhiteSpace(cidade))
                consultaLocalidadePolo = consultaLocalidadePolo.Where(localidadePolo => localidadePolo.Descricao.Contains(cidade));

            if (codigoIBGE > 0)
                consultaLocalidadePolo = consultaLocalidadePolo.Where(localidadePolo => localidadePolo.CodigoIBGE == codigoIBGE);

            if (!string.IsNullOrWhiteSpace(uf))
                consultaLocalidadePolo = consultaLocalidadePolo.Where(localidadePolo => localidadePolo.Estado.Sigla == uf);

            return consultaLocalidadePolo;
        }

        #endregion Métodos Privados

        #region Métodos Importacao de Tabela de Frete

        public Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade BuscarPorCodigoParaImportacaoTabelaFrete(int codigo)
        {
            var consultaLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                .Where(o => o.Codigo == codigo);

            return consultaLocalidade
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade()
                {
                    Codigo = o.Codigo,
                    CodigoIbge = o.CodigoIBGE,
                    Descricao = o.Descricao,
                    EstadoSigla = (o.Estado != null) ? o.Estado.Sigla : "",
                    PaisAbreviacao = (o.Pais != null) ? o.Pais.Abreviacao : "",
                    PaisNome = (o.Pais != null) ? o.Pais.Nome : "",
                    PossuiPais = (o.Pais != null)
                })
                .FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade BuscarPorDescricaoEUfParaImportacaoTabelaFrete(string descricao, string uf)
        {
            var consultaLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                .Where(o =>
                    o.Estado.Sigla.Equals(uf) &&
                    (o.Descricao.Equals(descricao) || o.OutrasDescricoes.Contains(descricao)) &&
                    o.Empresa == null
                );
            return consultaLocalidade
                .OrderBy(o => o.Descricao)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade()
                {
                    Codigo = o.Codigo,
                    CodigoIbge = o.CodigoIBGE,
                    Descricao = o.Descricao,
                    EstadoSigla = (o.Estado != null) ? o.Estado.Sigla : "",
                    PaisAbreviacao = (o.Pais != null) ? o.Pais.Abreviacao : "",
                    PaisNome = (o.Pais != null) ? o.Pais.Nome : "",
                    PossuiPais = (o.Pais != null)
                })
                .FirstOrDefault();
        }

        #endregion Métodos Importacao de Tabela de Frete

        #region Métodos Relatório

        public IList<Dominio.Relatorios.Embarcador.DataSource.Localidades.Localidade> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaLocalidade().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Localidades.Localidade)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Localidades.Localidade>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaLocalidade().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion Métodos Relatório
    }
}
