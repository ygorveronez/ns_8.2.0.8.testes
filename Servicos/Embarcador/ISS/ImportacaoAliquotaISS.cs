using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.ISS
{
    public sealed class ImportacaoAliquotaISS
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores

        public ImportacaoAliquotaISS(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _configuracao = configuracao;
        }

        #endregion

        #region Métodos Privados

        private string ObterDescricao()
        {
            var descricaoAliquotaISS = string.Empty;

            if (_dados.TryGetValue("Descricao", out var descricao))
                descricaoAliquotaISS = (string)descricao;

            if (string.IsNullOrWhiteSpace(descricaoAliquotaISS))
                throw new ImportacaoException("Descrição não informada.");

            return descricaoAliquotaISS;
        }

        private Dominio.Entidades.Localidade ObterLocalidade()
        {
            string descricaoLocalidade = string.Empty;
            int codigoIBGELocalidade = 0;

            if (_dados.TryGetValue("DescricaoLocalidade", out var descricao))
                descricaoLocalidade = (string)descricao;

            if (_dados.TryGetValue("CodigoIBGELocalidade", out var codigo))
                codigoIBGELocalidade = ((string)codigo).ToInt();

            if (string.IsNullOrWhiteSpace(descricaoLocalidade) && codigoIBGELocalidade == 0)
                throw new ImportacaoException("Descrição da cidade e Código do IBGE da cidade não foram informados, informe um dos dois para prosseguir.");

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorDescricaoECodigoIBGE(descricaoLocalidade, codigoIBGELocalidade);

            return localidade;
        }

        private decimal ObterAliquota()
        {
            decimal aliquota = 0;

            if (_dados.TryGetValue("Aliquota", out var valorAliquota))
                aliquota = ((string)valorAliquota).ToDecimal();

            if (aliquota == 0)
                throw new ImportacaoException("Favor informar capacidade(KG) superior a 0, campo obrigatório!");

            return aliquota;
        }

        private DateTime? ObterDataInicio()
        {
            DateTime? dataInicio = null;

            if (_dados.TryGetValue("DataInicial", out var dataInicioValor))
                dataInicio = ((string)dataInicioValor).ToNullableDateTime();

            return dataInicio;
        }

        private DateTime? ObterDataFim()
        {
            DateTime? dataFim = null;

            if (_dados.TryGetValue("DataInicial", out var dataFimValor))
                dataFim = ((string)dataFimValor).ToNullableDateTime();

            return dataFim;
        }

        private bool ObterRetemISS()
        {
            bool retemISS = false;

            string RetemISSConversao = string.Empty;
            if (_dados.TryGetValue("RetemISS", out var retemISSValor))
                RetemISSConversao = (string)retemISSValor;

            if (string.IsNullOrWhiteSpace(RetemISSConversao))
                return retemISS;

            if (RetemISSConversao.ToLower().Equals("sim"))
                return true;

            return retemISS;
        }

        private Dominio.Entidades.Embarcador.ISS.AliquotaISS ObterAliquotaISS()
        {
            var descricaoAliquotaISS = string.Empty;

            if (_dados.TryGetValue("Descricao", out var descricao))
                descricaoAliquotaISS = (string)descricao;

            if (string.IsNullOrWhiteSpace(descricaoAliquotaISS))
                throw new ImportacaoException("Descrição não informada.");

            Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(_unitOfWork);
            Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = repositorioAliquotaISS.BuscarPorDescricao(descricaoAliquotaISS);

            if (aliquotaISS == null)
            {
                aliquotaISS = new Dominio.Entidades.Embarcador.ISS.AliquotaISS();
                aliquotaISS.Descricao = descricaoAliquotaISS;
            }
            else
                aliquotaISS.Initialize();

            return aliquotaISS;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.ISS.AliquotaISS ObterAliquotaISSImportar()
        {
            Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = ObterAliquotaISS();

            aliquotaISS.Descricao = ObterDescricao();
            aliquotaISS.Ativo = true;
            aliquotaISS.Localidade = ObterLocalidade();
            aliquotaISS.Aliquota = ObterAliquota();
            aliquotaISS.DataInicio = ObterDataInicio();
            aliquotaISS.DataFim = ObterDataFim();
            aliquotaISS.RetemISS = ObterRetemISS();

            return aliquotaISS;
        }

        #endregion
    }
}