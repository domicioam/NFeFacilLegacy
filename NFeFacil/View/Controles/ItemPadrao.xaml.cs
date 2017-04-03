﻿using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NFeFacil.View.Controles
{
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "Icone")]
    public sealed partial class ItemPadrao : UserControl
    {
        public ItemPadrao()
        {
            this.InitializeComponent();
        }

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public IconElement Icone { get; set; }
    }
}
