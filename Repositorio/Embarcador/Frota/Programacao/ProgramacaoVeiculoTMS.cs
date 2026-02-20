using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.Programacao
{
    public class ProgramacaoVeiculoTMS : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS>
    {

        public ProgramacaoVeiculoTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS> ConsultarProgramacaoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                consultaVeiculo = consultaVeiculo.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                consultaVeiculo = consultaVeiculo.Skip(inicioRegistros).Take(maximoRegistros);

            return consultaVeiculo.ToList();
        }

        public int ContarConsultarProgramacaoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS filtrosPesquisa)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(filtrosPesquisa);

            return consultaVeiculo.Count();
        }
        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS> ConsultarProgramacaoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoVeiculoTMS filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroFrota))
                consulta = consulta.Where(o => o.Veiculo.NumeroFrota == filtrosPesquisa.NumeroFrota);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consulta = consulta.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoReboque > 0)
                consulta = consulta.Where(o => o.Veiculo.VeiculosVinculados.Any(p => p.Codigo == filtrosPesquisa.CodigoReboque));

            if (filtrosPesquisa.ModeloVeicular > 0)
                consulta = consulta.Where(o => o.Veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.ModeloVeicular);

            if (filtrosPesquisa.Situacoes.Count > 0)
                consulta = consulta.Where(o => filtrosPesquisa.Situacoes.Contains(o.ProgramacaoSituacao.Codigo));

            if (filtrosPesquisa.Motorista > 0)
                consulta = consulta.Where(o => o.Motorista.Codigo == filtrosPesquisa.Motorista);

            if (filtrosPesquisa.Estados.Count > 0)
                consulta = consulta.Where(o => filtrosPesquisa.Estados.Contains(o.CidadeEstado.Estado.Sigla));

            if (filtrosPesquisa.CodigoFuncionarioResponsavelCavalo > 0)
                consulta = consulta.Where(o => o.Veiculo.FuncionarioResponsavel.Codigo == filtrosPesquisa.CodigoFuncionarioResponsavelCavalo);

            if (filtrosPesquisa.CodigoMarcaCavalo > 0)
                consulta = consulta.Where(o => o.Veiculo.Marca.Codigo == filtrosPesquisa.CodigoMarcaCavalo);

            if (filtrosPesquisa.DataCadastroPlanejamentoInicial != System.DateTime.MinValue && filtrosPesquisa.DataCadastroPlanejamentoInicial != null)
                consulta = consulta.Where(o => o.DataCriacaoPlanejamento > filtrosPesquisa.DataCadastroPlanejamentoInicial);
            
            if (filtrosPesquisa.DataCadastroPlanejamentoFinal != System.DateTime.MinValue && filtrosPesquisa.DataCadastroPlanejamentoFinal != null)
                consulta = consulta.Where(o => o.DataCriacaoPlanejamento < filtrosPesquisa.DataCadastroPlanejamentoFinal);
                 
            if (filtrosPesquisa.DataDisponibilidadeInicial != System.DateTime.MinValue && filtrosPesquisa.DataDisponibilidadeInicial != null)
                consulta = consulta.Where(o => o.DataDisponivelInicio > filtrosPesquisa.DataDisponibilidadeInicial);
                    
            if (filtrosPesquisa.DataDisponibilidadeFinal != System.DateTime.MinValue && filtrosPesquisa.DataDisponibilidadeFinal != null)
                consulta = consulta.Where(o => o.DataDisponivel < filtrosPesquisa.DataDisponibilidadeFinal);

            return consulta;
        }
        #endregion
    }
}
