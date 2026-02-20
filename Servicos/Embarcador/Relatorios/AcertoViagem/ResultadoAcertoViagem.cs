using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class ResultadoAcertoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ResultadoAcertoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoViagem _repositorioAcertoViagem;

        #endregion

        #region Construtores

        public ResultadoAcertoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList, metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ResultadoAcertoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAcertoViagem.RelatorioResultadoAcertoViagem(filtrosPesquisa.SegmentoVeiculo, filtrosPesquisa.Motorista, filtrosPesquisa.GrupoPessoa, filtrosPesquisa.ModeloVeiculo, filtrosPesquisa.VeiculoTracao, filtrosPesquisa.VeiculoReboque, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAcertoViagem.ContarRelatorioResultadoAcertoViagem(filtrosPesquisa.SegmentoVeiculo, filtrosPesquisa.Motorista, filtrosPesquisa.GrupoPessoa, filtrosPesquisa.ModeloVeiculo, filtrosPesquisa.VeiculoTracao, filtrosPesquisa.VeiculoReboque, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/ResultadoAcertoViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();


            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (filtrosPesquisa.Motorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repFuncionario.BuscarPorCodigo(filtrosPesquisa.Motorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", "(" + motorista.CPF_Formatado + ") " + motorista.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.GrupoPessoa > 0)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.GrupoPessoa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupoPessoa.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

            if (filtrosPesquisa.ModeloVeiculo > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.ModeloVeiculo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", modeloVeicular.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", false));

            if (filtrosPesquisa.VeiculoReboque > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.VeiculoReboque);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", false));

            if (filtrosPesquisa.VeiculoTracao > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.VeiculoTracao);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", false));

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