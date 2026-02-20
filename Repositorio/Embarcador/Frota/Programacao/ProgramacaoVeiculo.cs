using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.Programacao
{
    public class ProgramacaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo>
    {

        public ProgramacaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo> _Consultar(int codigoEmpresa, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo> Consultar(int codigoEmpresa, string descricao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, descricao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao)
        {
            var result = _Consultar(codigoEmpresa, descricao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo> ConsultarProgramacaoVeiculo(string numeroFrota, int codigoVeiculo, int codigoReboque, int codigoModeloVeicular, int codigoSituacao, int codigoMotorista, int codigoLicenciamento, int codigoAlocacao, int codigoEspecialidade, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, codigoSituacao, codigoMotorista, codigoLicenciamento, codigoAlocacao, codigoEspecialidade);

            return consultaVeiculo
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros).ToList();
        }

        public int ContarConsultarProgramacaoVeiculo(string numeroFrota, int codigoVeiculo, int codigoReboque, int codigoModeloVeicular, int codigoSituacao,  int codigoMotorista, int codigoLicenciamento, int codigoAlocacao, int codigoEspecialidade)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, codigoSituacao, codigoMotorista, codigoLicenciamento, codigoAlocacao, codigoEspecialidade);

            return consultaVeiculo.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo> ConsultarProgramacaoVeiculo(string numeroFrota, int codigoVeiculo, int codigoReboque, int codigoModeloVeicular, int codigoSituacao, int codigoMotorista, int codigoLicenciamento, int codigoAlocacao, int codigoEspecialidade)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo>();

            consulta = consulta.Where(o => o.DataTermino == null);

            if (!string.IsNullOrWhiteSpace(numeroFrota))
                consulta = consulta.Where(o => o.Veiculo.NumeroFrota == numeroFrota);

            if (codigoVeiculo > 0)
                consulta = consulta.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoReboque > 0)
                consulta = consulta.Where(o => o.Veiculo.VeiculosVinculados.Any(p => p.Codigo == codigoReboque));

            if (codigoModeloVeicular > 0)
                consulta = consulta.Where(o => o.Veiculo.ModeloVeicularCarga.Codigo == codigoModeloVeicular);

            if (codigoSituacao > 0)
                consulta = consulta.Where(o => o.ProgramacaoSituacao.Codigo == codigoSituacao);

            if (codigoMotorista > 0)
                consulta = consulta.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (codigoLicenciamento > 0)
                consulta = consulta.Where(o => o.ProgramacaoLicenciamento.Codigo == codigoLicenciamento);

            if (codigoAlocacao > 0)
                consulta = consulta.Where(o => o.ProgramacaoAlocacao.Codigo == codigoAlocacao);

            if (codigoEspecialidade > 0)
                consulta = consulta.Where(o => o.ProgramacaoEspecialidade.Codigo == codigoEspecialidade);

            return consulta;
        }
    }
}
