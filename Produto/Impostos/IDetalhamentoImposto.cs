﻿// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.Impostos
{
    public interface IDetalhamentoImposto
    {
        PrincipaisImpostos Tipo { get; }
    }
}
