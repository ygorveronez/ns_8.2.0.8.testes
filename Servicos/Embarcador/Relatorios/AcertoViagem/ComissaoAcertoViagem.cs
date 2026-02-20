using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class ComissaoAcertoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ComissaoAcertoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoViagem _repositorioAcertoViagem;

        #endregion

        #region Construtores

        public ComissaoAcertoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ComissaoAcertoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAcertoViagem.RelatorioComissaoAcertoViagem(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar,parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAcertoViagem.ContarComissaoAcertoViagem(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/ComissaoAcertoViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
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

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue || filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue ? filtrosPesquisa.DataVencimentoInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataVencimentoFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", false));

            if (filtrosPesquisa.CodigoAcertoViagem > 0)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(filtrosPesquisa.CodigoAcertoViagem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", acerto.Numero.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", "(" + motorista.CPF_Formatado + ") " + motorista.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmento);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", segmento.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.ExibirOcorrencias)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirOcorrencias", "Sim", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirOcorrencias", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataInicialAcertoFormatada")
                return "DataInicialAcerto";

            if (propriedadeOrdenarOuAgrupar == "DataFinalAcertoFormatada")
                return "DataFinalAcerto";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}