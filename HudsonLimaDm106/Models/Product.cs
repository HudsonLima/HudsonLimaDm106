﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace HudsonLimaDm106.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O	campo	nome	é	obrigatório")]
        public string nome { get; set; }

        public string descricao { get; set; }

        public string cor { get; set; }

        [Required]
        public string modelo { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "O	tamanho	máximo	do	código   é   8   caracteres")]
        public string codigo { get; set; }

        [Range(10, 999, ErrorMessage = "O	preço	deverá	ser	entre  10  e   999.")]
        public decimal preco { get; set; }

        public decimal peso { get; set; }

        public decimal altura { get; set; }

        public decimal largura { get; set; }

        public decimal comprimento { get; set; }

        public decimal diametro { get; set; }

        [StringLength(80, ErrorMessage = "O	tamanho	máximo	daurl é   80  caracteres")]
        public string Url { get; set; }      /*  O modelo de produtos deverá ter as seguintes propriedades, no mínimo:
        • Identificação única do produto;
        • Nome do produto(com validação de preenchimento obrigatório);
        • Descrição;
        • Cor;
        • Modelo em formato string (com validação de preenchimento obrigatório);
        • Código em formato string (com validação de preenchimento obrigatório);
        • Preço;
        • Peso;
        • Altura;
        • Largura;
        • Comprimento;
        • Diâmetro;
        • URL da imagem do produto em formato string.*/    
    }
}