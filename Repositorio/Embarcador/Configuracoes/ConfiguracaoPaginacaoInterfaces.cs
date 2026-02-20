using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoPaginacaoInterfaces : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces>
    {
        public ConfiguracaoPaginacaoInterfaces(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces>();

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces BuscarPorInterface(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces interfacePaginacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces>().Where(configPagInterface => configPagInterface.Interface == interfacePaginacao);

            return query.FirstOrDefault();
        }

        public string BuscarPorDataLimiteInterface(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces interfacePaginacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces>().Where(configPagInterface => configPagInterface.Interface == interfacePaginacao);

            var configuracao = query.FirstOrDefault();

            if (configuracao == null)
                return string.Empty;

            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataLimite = dataAtual.AddDays(-configuracao.Dias);

            return dataLimite.ToString("dd/MM/yyyy");
        }
    }
}