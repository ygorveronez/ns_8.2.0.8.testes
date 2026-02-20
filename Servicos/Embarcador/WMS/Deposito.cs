namespace Servicos.Embarcador.WMS
{
    public class Deposito
    {
        public static void AtulizarAbreviacaoPosicao(Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            repDepositoPosicao.AtualizarAbreviacoesCascata(0, 0, 0, posicao.Codigo);
            //string abreviacao = "";
            //abreviacao += posicao.Descricao;
            //abreviacao += "." + posicao.Bloco.Descricao;
            //abreviacao += "." + posicao.Bloco.Rua.Descricao;
            //abreviacao += "." + posicao.Bloco.Rua.Deposito.Descricao;
            //return abreviacao;
        }

        public static void AtulizarAbreviacaoBloco(Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            repDepositoPosicao.AtualizarAbreviacoesCascata(0, 0, bloco.Codigo, 0);
        }

        public static void AtulizarAbreviacaoRua(Dominio.Entidades.Embarcador.WMS.DepositoRua rua, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            repDepositoPosicao.AtualizarAbreviacoesCascata(0, rua.Codigo, 0, 0);
        }

        public static void AtulizarAbreviacaoDeposito(Dominio.Entidades.Embarcador.WMS.Deposito deposito, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            repDepositoPosicao.AtualizarAbreviacoesCascata(deposito.Codigo, 0, 0, 0);
        }

      
    }
}
