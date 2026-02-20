using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Documentos
{
    public class DadosDocsys : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys, Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Documentos.DadosDocsys _repositorioDadosDocsys;

        #endregion

        #region Construtores

        public DadosDocsys(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioDadosDocsys = new Repositorio.Embarcador.Documentos.DadosDocsys(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioDadosDocsys.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioDadosDocsys.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Documentos/DadosDocsys";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", false));

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", false));


            if (filtrosPesquisa.PedidoViagemNavio > 0)
            {
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio _viagem = repPedidoViagemNavio.BuscarPorCodigo(filtrosPesquisa.PedidoViagemNavio);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PedidoViagemNavio", _viagem.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PedidoViagemNavio", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}