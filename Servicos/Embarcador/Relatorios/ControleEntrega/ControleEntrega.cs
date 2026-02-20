using Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.ControleEntrega
{
    public class ControleEntrega : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega, Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega>
    {
        #region Atributos Privados

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega _repControleEntrega;

        #endregion

        #region

        public ControleEntrega(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega> ConsultarRegistros(FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repControleEntrega.ConsultarRelatorioControleEntrega(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repControleEntrega.ContarConsultaRelatorioControleEntrega(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/ControleEntrega";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrencias = filtrosPesquisa.CodigosTipoOcorrencia.Count > 0 ? repTipoDeOcorrencia.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOcorrencia) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : null;
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculos.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculos) : null;
            List<Dominio.Entidades.Usuario> motoristas = filtrosPesquisa.CodigosMotoristas.Count > 0 ? repUsuario.BuscarMotoristaPorCodigo(filtrosPesquisa.CodigosMotoristas) : null;

            parametros.Add(new Parametro("DataOcorrencia", filtrosPesquisa.DataOcorrenciaInicial, filtrosPesquisa.DataOcorrenciaFinal));
            parametros.Add(new Parametro("TipoOcorrencia", tiposDeOcorrencias?.Select(o => o.Descricao)));
            parametros.Add(new Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Parametro("TipoOperacao", tiposOperacao?.Select(o => o.Descricao)));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("NumeroNotaFiscal", filtrosPesquisa.NumeroNotaFiscal));
            parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Parametro("Veiculos", veiculos?.Select(o => o.Placa)));
            parametros.Add(new Parametro("Motoristas", motoristas?.Select(o => o.Nome)));
            parametros.Add(new Parametro("DataPrevisaoEntrega", filtrosPesquisa.DataPrevisaoEntregaInicial, filtrosPesquisa.DataPrevisaoEntregaFinal));
            parametros.Add(new Parametro("UFOrigem", filtrosPesquisa.UFsOrigem));
            parametros.Add(new Parametro("UFDestino", filtrosPesquisa.UFsDestino));

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
