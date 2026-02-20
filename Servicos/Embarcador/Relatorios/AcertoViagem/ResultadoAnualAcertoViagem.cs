using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class ResultadoAnualAcertoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAnualAcertoViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ResultadoAnualAcertoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoViagem _repositorioAcertoViagem;

        #endregion

        #region Construtores

        public ResultadoAnualAcertoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList, metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ResultadoAnualAcertoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAnualAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAcertoViagem.RelatorioResultadoAnualAcertoViagem(filtrosPesquisa.SegmentoVeiculo,filtrosPesquisa.DataInicial,filtrosPesquisa.DataFinal, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAnualAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAcertoViagem.ContarRelatorioResultadoAnualAcertoViagem(filtrosPesquisa.SegmentoVeiculo, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/ResultadoAnualAcertoViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAnualAcertoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAcerto", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAcerto", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.SegmentoVeiculo > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.SegmentoVeiculo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", segmento.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}