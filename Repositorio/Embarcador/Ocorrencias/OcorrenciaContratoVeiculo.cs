using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaContratoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>
    {

        public OcorrenciaContratoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo BuscarPorCodigoVeiculoEOcorrencia(int veiculo, int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == veiculo && obj.Ocorrencia.Codigo == ocorrencia select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>();
            var result = from obj in query
                         where obj.Ocorrencia.Codigo == ocorrencia
                         select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo> ConsultarDeOcorrencia(int ocorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>();
            var result = from obj in query
                         where obj.Ocorrencia.Codigo == ocorrencia
                         select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);
            
            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo {
                Codigo = o.Codigo,
                CodigoVeiculo = o.Veiculo.Codigo,
                Veiculo = o.Veiculo.Placa,
                QuantidadeDias = o.QuantidadeDias,
                ValorDiaria = o.ValorDiaria,
                ValorQuinzena = o.ValorQuinzena,
                Total = o.ValorQuinzena > 0 ? o.ValorQuinzena : o.ValorDiaria * o.QuantidadeDias,
                QuantidadeDocumentos = o.QuantidadeDocumentos,
                ValorDocumentos = o.ValorDocumentos
            }).ToList();
        }

        public int ContarConsultaDeOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo>();
            var result = from obj in query
                         where obj.Ocorrencia.Codigo == ocorrencia
                         select obj;

            return result.Count();
        }
    }
}
