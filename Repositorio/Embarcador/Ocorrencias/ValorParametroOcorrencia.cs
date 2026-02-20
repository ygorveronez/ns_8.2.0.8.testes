using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia>
    {

        public ValorParametroOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia> _Consultar(int tipoOperacao, double pessoa, int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia>();

            var result = from obj in query select obj;

            // Filtros
            if (pessoa > 0)
                result = result.Where(o => o.Pessoa.CPF_CNPJ == pessoa);

            if (grupoPessoa > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == grupoPessoa);

            if (tipoOperacao > 0)
                result = result.Where(o => o.TiposOperacao.Any(t => t.TipoOperacao.Codigo == tipoOperacao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia> Consultar(int tipoOperacao, double pessoa, int grupoPessoa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(tipoOperacao, pessoa, grupoPessoa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int tipoOperacao, double pessoa, int grupoPessoa)
        {
            var result = _Consultar(tipoOperacao, pessoa, grupoPessoa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia BuscarPorTipoOcorrenciaPessoaGrupoVigencia(int tipoOperacao, int tipoOcorrencia, double pessoa, int grupo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia>();

            var result = from obj in query
                         where
                            (
                                obj.VigenciaInicial <= data
                                && obj.VigenciaFinal >= data
                            )
                            &&
                            (
                                obj.ValorParametroHoraExtraOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia
                                || obj.ValorParametroEstadiaOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia
                                || obj.ValorParametroPernoiteOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia
                                || obj.ValorParametroEscoltaOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia
                            )
                            &&
                            (
                                (obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && (obj.Pessoa.CPF_CNPJ == pessoa || obj.Pessoa == null))
                                || (obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && (obj.GrupoPessoas.Codigo == grupo || obj.GrupoPessoas == null))
                            )
                            &&
                            (
                                tipoOperacao > 0 ? ((obj.TiposOperacao.Any(t => t.TipoOperacao.Codigo == tipoOperacao)) || (obj.TiposOperacao.Count == 0)) : 1 == 1
                            )
                         orderby obj.Pessoa descending, obj.GrupoPessoas descending
                         select obj;

            return result.FirstOrDefault();
        }
    }
}
