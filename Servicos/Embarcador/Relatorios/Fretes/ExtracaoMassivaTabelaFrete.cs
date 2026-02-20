using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class ExtracaoMassivaTabelaFrete : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente, Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete>
    {
        #region Atributos

        private readonly Servicos.Embarcador.Frete.Consulta.ExtracaoMassivaTabelaFrete _servicoExtracaoMassivaTabelaFrete;
        private readonly Repositorio.Embarcador.Frete.TabelaFreteCliente _repositorioTabelaFreteCliente;

        #endregion

        #region Construtores

        public ExtracaoMassivaTabelaFrete(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            _servicoExtracaoMassivaTabelaFrete = new Frete.Consulta.ExtracaoMassivaTabelaFrete(unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var listaConsultaTabelaFrete = _repositorioTabelaFreteCliente.ConsultaExtracaoMassiva(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
            _servicoExtracaoMassivaTabelaFrete.CarregarDadosAnteriores(filtrosPesquisa, propriedadesAgrupamento, listaConsultaTabelaFrete);

            InicializarPropriedadesNulas(listaConsultaTabelaFrete);

            return listaConsultaTabelaFrete;
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTabelaFreteCliente.ContarConsultaExtracaoMassiva(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/ExtracaoMassivaTabelaFrete";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = repositorioTabelaFrete.BuscarPorCodigos(filtrosPesquisa.CodigosTabelasFrete);
            
            parametros.Add(new Parametro("TabelaFrete", tabelasFrete.Select(o => o.Descricao)));
            parametros.Add(new Parametro("DataInicialAlteracao", filtrosPesquisa.DataInicialAlteracao?.ToString("dd/MM/yyyy") ?? ""));
            parametros.Add(new Parametro("DataFinalAlteracao", filtrosPesquisa.DataFinalAlteracao?.ToString("dd/MM/yyyy") ?? ""));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion

        #region Métodos Privados

        static void InicializarPropriedadesNulas(List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> lista)
        {
            foreach (var obj in lista)
            {
                PropertyInfo[] propriedades = obj.GetType().GetProperties();
                foreach (var propriedade in propriedades)
                {
                    if (propriedade.GetValue(obj) == null)
                    {
                        if (propriedade.PropertyType == typeof(string))
                        {
                            propriedade.SetValue(obj, "");
                        }
                        else if (propriedade.PropertyType == typeof(int?) || propriedade.PropertyType == typeof(int))
                        {
                            propriedade.SetValue(obj, 0);
                        }
                        else if (propriedade.PropertyType == typeof(DateTime?) || propriedade.PropertyType == typeof(DateTime))
                        {
                            propriedade.SetValue(obj, DateTime.MinValue);
                        }
                        else if (propriedade.PropertyType == typeof(double?) || propriedade.PropertyType == typeof(double))
                        {
                            propriedade.SetValue(obj, 0.0);
                        }
                    }
                }
            }
        }

        #endregion
    }
}