﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View.CaixasDialogo
{
    public sealed partial class DetalheClienteAtual : ContentDialog
    {
        public bool ManipulacaoAtivada { get; set; }

        public DetalheClienteAtual()
        {
            this.InitializeComponent();
        }
    }
}