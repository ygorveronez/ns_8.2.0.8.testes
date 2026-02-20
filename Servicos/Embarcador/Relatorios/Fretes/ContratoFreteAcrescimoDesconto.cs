using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class ContratoFreteAcrescimoDesconto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto, Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto _repositorioContratoFrete;
        #endregion

        #region Construtores

        public ContratoFreteAcrescimoDesconto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
        }

        public ContratoFreteAcrescimoDesconto(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken
            ) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork, cancellationToken);

        }

        #endregion

        #region Métodos assíncronos
        public  async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioContratoFrete.ConsultarRelatorioAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteAcrescimoDesconto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioContratoFrete.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioContratoFrete.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/ContratoFreteAcrescimoDesconto";
        }
            
        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial , filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Justificativa", filtrosPesquisa.Justificativa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumCiot", filtrosPesquisa.NumCiot));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumContratoFrete", filtrosPesquisa.NumContratoFrete));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumCarga", filtrosPesquisa.NumCarga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Terceiro", filtrosPesquisa.Terceiro));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
