using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaContratoVeiculoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo>
    {
        public OcorrenciaContratoVeiculoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo> BuscarPorCodigoOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo>();
            var result = from obj in query where obj.OcorrenciaContratoVeiculo.Ocorrencia.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo> Consultar(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo>();
            var result = from obj in query where obj.OcorrenciaContratoVeiculo.Ocorrencia.Codigo == codigoOcorrencia select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo>();
            var result = from obj in query where obj.OcorrenciaContratoVeiculo.Ocorrencia.Codigo == codigoOcorrencia select obj;

            return result.Count();
        }


    }
}
