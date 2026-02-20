using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EBSComissaoMotorista
    {
        public static Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotorista GerarEBS(Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario, int codigoEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao tipoEBSComissao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotorista comissaoMotorista = new Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotorista();

            if (tipoEBSComissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao.Comissao)
                comissaoMotorista.Comissoes = repComissaoFuncionarioMotorista.BuscarPorComissaoParaEBS(comissaoFuncionario.Codigo, codigoEvento);
            else if (tipoEBSComissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao.Diaria)
                comissaoMotorista.Comissoes = repComissaoFuncionarioMotorista.BuscarDiariasPorComissaoParaEBS(comissaoFuncionario.Codigo, codigoEvento);
            else if (tipoEBSComissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao.Media)
                comissaoMotorista.Comissoes = repComissaoFuncionarioMotorista.BuscarMediasPorComissaoParaEBS(comissaoFuncionario.Codigo, codigoEvento);
            else if (tipoEBSComissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao.Produtividade)
                comissaoMotorista.Comissoes = repComissaoFuncionarioMotorista.BuscarProdutividadePorComissaoParaEBS(comissaoFuncionario.Codigo, codigoEvento);

            comissaoMotorista.Comissoes.OrderBy(o => o.CodigoConvertido);

            return comissaoMotorista;
        }
    }
}
